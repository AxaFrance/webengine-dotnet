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
    /// Test report structure for the test execution
    /// </summary>
    [Serializable]
    [XmlRoot(Namespace = GlobalConstants.XmlNamespace)]
    public class TestSuiteReport
    {
        /// <summary>
        /// The list of results for each test case.
        /// </summary>
        [XmlElement(ElementName = "TestResult")]
        public List<TestCaseReport> TestResult { get; set; }

        /// <summary>
        /// Number of test cases which result are <see cref="Result.Passed"/>
        /// </summary>
        /// <remarks>
        /// The number will be valorised before saving the report on the disk.
        /// </remarks>
        public int Passed { get; set; }

        /// <summary>
        /// Number of test cases which result are <see cref="Result.Failed"/> or <see cref="Result.CriticalError"/>
        /// </summary>
        /// <remarks>
        /// The number will be valorised before saving the report on the disk.
        /// </remarks>

        public int Failed { get; set; }

        /// <summary>
        /// Number of test cases which result are <see cref="Result.Ignored"/>
        /// </summary>
        /// <remarks>
        /// The number will be valorised before saving the report on the disk.
        /// </remarks>
        public int Ignored { get; set; }


        /// <summary>
        /// The Test enviroment variables used during the test.
        /// </summary>
        public EnvironmentVariables EnvironmentVariables { get; set; }

        /// <summary>
        /// The <see cref="DateTime"/> when the test suite is started for execution.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// The <see cref="DateTime"/> when the test execution for the testsuite ends.
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Calculates the Duration of the current test suite;
        /// </summary>
        public TimeSpan Duration
        {
            get
            {
                return EndTime - StartTime;
            }
        }

        /// <summary>
        /// The hostname where the test has been executed.
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// The raw output of the test execution.
        /// </summary>
        public string SystemOut { get; set; }

        /// <summary>
        /// The raw error otput of the test execution.
        /// </summary>
        public string SystemError { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public TestSuiteReport()
        {
            TestResult = new List<TestCaseReport>();
            EnvironmentVariables = new EnvironmentVariables();
        }

        /// <summary>
        /// Write the test results into an XML file
        /// </summary>
        /// <param name="filePrefix">the prefixe of the report (without .xml extension), suffixed automatially with the current datetime.</param>
        /// <param name="path">The path where the report file will be stored.</param>
        /// <param name="uniqueName">Whether the report name should be unique. True: the report name will be suffixed with timestamp, False: the report name will be fixed</param>
        /// <returns>the report Full Path name generated.</returns>
        public string SaveAs(string path, string filePrefix, bool uniqueName)
        {
            this.Passed = TestResult.Count(x => x.Result == Result.Passed);
            this.Failed = TestResult.Count(x => x.Result == Result.Failed || x.Result == Result.CriticalError);
            this.Ignored = TestResult.Count - Passed - Failed;
            string filename;
            if (uniqueName)
            {
                filename = Path.Combine(path, filePrefix + ".xml");
            }
            else
            {
                filename = Path.Combine(path, filePrefix + "_" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".xml");
            }
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            using (StreamWriter sw = new StreamWriter(filename))
            {
                XmlSerializer xs = new XmlSerializer(typeof(TestSuiteReport));
                xs.Serialize(sw, this);
            }
            return filename;
        }


        /// <summary>
        /// Generates a CSV file contains all the ContextValues and GlobalContextValues into an CSV file.
        /// </summary>
        /// <param name="fileLocation">The file location</param>
        /// <param name="seperator">CSV separator. generally comma or semicolon.</param>
        /// <param name="sort">If the parameter should be in alphabet order.</param>
        public void GenereteCSV(string fileLocation, string seperator, bool sort)
        {
            List<List<Variable>> AllLines = new List<List<Variable>>();
            List<string> Labels = new List<string>();
            foreach (var tc in this.TestResult)
            {
                List<Variable> v = new List<Variable>();
                v.AddItem("_NAME", tc.TestName);
                v.AddItem("_RESULT", tc.Result.ToString());
                v.AddRange(tc.ContextValues);
                v.AddRange(tc.GlobalContextValues);
                AllLines.Add(v);

                foreach (var context in tc.ContextValues.Select(x => x.Name))
                {
                    if (!Labels.Contains(context))
                    {
                        Labels.Add(context);
                    }
                }


            }

            if (sort) { Labels.Sort(); }

            using (StreamWriter sw = new StreamWriter(fileLocation, false, Encoding.UTF8))
            {
                sw.WriteLine(string.Join(seperator, Labels.ToArray()));
                foreach (var line in AllLines)
                {
                    foreach (string key in Labels)
                    {
                        if (line.Exists(x => x.Name == key))
                        {
                            sw.Write(line.Find(x => x.Name == key).Value + seperator);
                        }
                        else
                        {
                            sw.Write(seperator);
                        }
                    }
                    sw.WriteLine();
                }
            }

            DebugLogger.WriteLine("CSV Test result Generated: " + fileLocation);
        }
    }
}
