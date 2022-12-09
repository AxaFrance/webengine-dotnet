// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

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
        /// <param name="testname">The name of the test</param>
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
                    jtc.Item = new JUnit.testsuiteTestcaseError()
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
    }
}
