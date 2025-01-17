// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using AxaFrance.WebEngine.Report;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AxaFrance.WebEngine
{
    /// <summary>
    /// The structure of a TestSuite.
    /// <para>When launch tests with WebRunner.exe, WebEngine will scan all the derived classes of TestSuite within your test DLL. 
    /// Then it will launch every test cases of the test suite, which is defined in the property TestCases</para>
    /// </summary>
    public abstract class TestSuite
    {
        /// <summary>
        /// A list of TestCases in the Test suite.
        /// <para>When using Data driven test, please generate your test cases from the TestData dynamically. when using fixed regression test campaign, you can initialize your test cases in get method of the property. </para>
        /// </summary>
        /// <remarks>
        /// The KeyValuePair dictionary has two parameters: key and value
        /// the key parameter is type of string, its value is the key to obtain test data from the class <see cref="TestSuiteData"/>.
        /// value parameter is type of <see cref="AxaFrance.WebEngine.TestCase"/>, which defines one or more test steps (actions and sub-actions)
        /// </remarks>
        public abstract KeyValuePair<string, TestCase>[] TestCases { get; }

        /// <summary>
        /// Initialize the global context for the whole test suite.
        /// </summary>
        public virtual void Initialize(Settings s) { }

        /// <summary>
        /// Cleanup the global context for the test suite.
        /// </summary>
        public virtual void CleanUp(Settings s)
        {

        }

        /// <summary>
        /// Run the test suite and all the test cases in it.
        /// </summary>
        /// <returns></returns>
        public TestSuiteReport Run()
        {
            TestSuiteReport tsReport = new TestSuiteReport();
            tsReport.EnvironmentVariables = EnvironmentVariables.Current;
            tsReport.StartTime = DateTime.Now;
            foreach (var tc in TestCases)
            {
                string name = tc.Key;
                var test = tc.Value;
                TestCaseReport tcr = new TestCaseReport() { TestName = name, StartTime = DateTime.Now };
                tsReport.TestResult.Add(tcr);
                StringBuilder filename = new StringBuilder(name);
                Path.GetInvalidFileNameChars().ToList().ForEach(c => filename.Replace(c.ToString(), string.Empty));
                tc.Value.TestCaseLogDir = Path.Combine(Settings.Instance.LogDir, filename.ToString());
                test.Name = name;
                test.Run(tcr);
                tcr.EndTime = DateTime.Now;
            }
            tsReport.EndTime = DateTime.Now;
            return tsReport;
        }
    }
}
