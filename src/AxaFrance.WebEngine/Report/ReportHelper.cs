// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;

namespace AxaFrance.WebEngine.Report
{

    /// <summary>
    /// Helper class to generate other report formats from standard Web Engine report.
    /// </summary>
    public class ReportHelper
    {
        /// <summary>
        /// Generates JUnit test result format from standard WebEngine report
        /// </summary>
        /// <param name="report">WebEngine report</param>
        /// <param name="testname">The name of the test suite</param>
        /// <param name="outputPath">The directory where the report should be generated.</param>
        public static string GenerateJUnitReport(TestSuiteReport report, string testname, string outputPath)
        {
            string extension = ".xml";
            if (testname.EndsWith(extension, StringComparison.InvariantCultureIgnoreCase)) testname = testname.Substring(0, testname.Length - extension.Length);
            var jr = new JUnit.testsuite()
            {
                name = testname ?? "WebEngine Test Suite",
                timestamp = report.StartTime,
                time = (decimal)report.Duration.TotalSeconds,
                hostname = report.HostName,
                systemout = report.SystemOut,
                systemerr = report.SystemError,
                errors = report.TestResult.Count(x => x.Result == Result.Failed),
                tests = report.TestResult.Count(),
            };
            List<JUnit.testsuiteTestcase> tests = new List<JUnit.testsuiteTestcase>();
            foreach (var tc in report.TestResult)
            {
                JUnit.testsuiteTestcase jtc = new JUnit.testsuiteTestcase()
                {
                    name = tc.TestName,
                    time = (decimal)tc.Duration.TotalSeconds,
                    classname = nameof(WebEngine.TestCase),
                };
                tests.Add(jtc);
                if (tc.Result == Result.Failed)
                {
                    jtc.Item = new JUnit.testsuiteTestcaseFailure()
                    {
                        message = tc.Log,
                        type = tc.Result.ToString()
                    };
                }
                else if (tc.Result == Result.Ignored)
                {
                    jtc.Item = new JUnit.testsuiteTestcaseSkipped()
                    {
                        message = tc.Result.ToString()
                    };
                }
                else if (tc.Result == Result.CriticalError)
                {
                    jtc.Item = new JUnit.testsuiteTestcaseFailure()
                    {
                        message = tc.Log,
                        type = tc.Result.ToString(),
                    };
                }
            }
            jr.testcase = tests.ToArray();
            string xmlContent = Serialize<JUnit.testsuite>(jr);
            Directory.CreateDirectory(outputPath);
            string reportFullPath = Path.Combine(outputPath, $"TEST-{testname}.xml");
            using(StreamWriter sw = new StreamWriter(reportFullPath))
            {
                sw.Write(xmlContent);
            }

            return reportFullPath;
        }

        /// <summary>
        /// Generates HTML report from standard WebEngine report
        /// </summary>
        /// <param name="report">WebEngine report</param>
        /// <param name="testname">The name of the test suite</param>
        /// <param name="outputPath">The directory where the report should be generated</param>
        /// <returns></returns>
        public static string GenerateHtmlReport(TestSuiteReport report, string testname, string outputPath)
        {
            var assetFolder = ExtractResource("html-report.zip");
            var xslt = Path.Combine(assetFolder, "xslt", "index.xslt");
            //conver report to xml and transform to html with xslt
            string xmlContent = Serialize<TestSuiteReport>(report);
            string htmlContent = TransformXSLT(xmlContent, xslt);
            Directory.CreateDirectory(outputPath);
            string reportFullPath = Path.Combine(outputPath, $"{testname}.html");
            using (StreamWriter sw = new StreamWriter(reportFullPath))
            {
                sw.Write(htmlContent);
            }
            //copy assets files from assetfolder to output path
            var path = Path.Combine(assetFolder, "assets");
            foreach (var file in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
            {
                string relativePath = file.Substring(assetFolder.Length + 1);
                string targetPath = Path.Combine(outputPath, relativePath);
                Directory.CreateDirectory(Path.GetDirectoryName(targetPath));
                File.Copy(file, targetPath, true);
            }
            return reportFullPath;
        }

        private static string TransformXSLT(string xmlContent, string xslt)
        {
            XslCompiledTransform transformer = new XslCompiledTransform();
            transformer.Load(xslt);
            var xmlDoc = XDocument.Parse(xmlContent);
            var htmlDoc = new XDocument();

            using (XmlReader junit = xmlDoc.CreateReader())
            {
                using (XmlWriter html = htmlDoc.CreateWriter())
                {
                    transformer.Transform(junit, html);
                }
            }
            string result = htmlDoc.ToString();
            return result;
        }

        private static string ExtractResource(string resourceName)
        {
            Assembly assembly = typeof(ReportHelper).Assembly;
            string[] names = assembly.GetManifestResourceNames();
            string name = names.FirstOrDefault(x => x.EndsWith(resourceName, StringComparison.InvariantCultureIgnoreCase));
            if (name == null) throw new FileNotFoundException($"Resource {resourceName} not found in assembly {assembly.FullName}");
            using (Stream stream = assembly.GetManifestResourceStream(name))
            {
                using(ZipArchive archive = new ZipArchive(stream))
                {
                    string outputPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                    archive.ExtractToDirectory(outputPath);
                    return outputPath;
                }
            }
        }

        private static string Serialize<T>(T jr)
        {
            XmlSerializer s = new XmlSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream())
            {
                s.Serialize(ms, jr);
                ms.Flush();
                ms.Seek(0, SeekOrigin.Begin);
                string content = Encoding.UTF8.GetString(ms.ToArray());
                return content;
            }
        }

        /// <summary>
        /// Load WebEngine Report from File
        /// </summary>
        /// <param name="filename">The path of the WebEngine Report file</param>
        /// <param name="content">The content in XML of WebEngine Report file</param>
        /// <returns>The report of type <see cref="TestSuiteReport"/></returns>
        public static TestSuiteReport LoadReport(string filename, out string content)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(TestSuiteReport));
            using (StreamReader sr = new StreamReader(filename))
            {
                content = sr.ReadToEnd();
            }
            TestSuiteReport ts = (TestSuiteReport)serializer.Deserialize(new StreamReader(filename));
            return ts;
        }
    }
}
