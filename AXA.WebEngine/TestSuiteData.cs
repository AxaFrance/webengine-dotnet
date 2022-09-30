// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace AXA.WebEngine
{
    /// <summary>
    /// The test data of a test suite, which contains data for one or several test cases.
    /// Each test data is identified by its name
    /// Each TestSuiteData are represented in XML file
    /// </summary>
    [Serializable]
    [XmlRoot(Namespace = GlobalConstants.XmlNamespace)]
    public class TestSuiteData
    {
        /// <summary>
        /// A list of test data (one TestData element contains all parameter needed for a single test case)
        /// </summary>
        [XmlElement("TestData")]
        public List<TestData> TestDataList { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public TestSuiteData()
        {
            TestDataList = new List<TestData>();
        }

        /// <summary>
        /// Get the test data for a single test case from the loaded XML File
        /// </summary>
        /// <param name="name">name of the test case</param>
        /// <returns>the TestData structure which contains a list of Parameters (Test Data)</returns>
        public TestData GetTestData(string name)
        {
            return TestDataList.First(x => x.TestName == name);
        }

        /// <summary>
        /// Set the value of an variable in given test case
        /// </summary>
        /// <param name="testName">Name of the test case</param>
        /// <param name="parameterName">Name of the parameter</param>
        /// <param name="value">Value of the parameter</param>
        public void SetValue(string testName, string parameterName, string value)
        {
            TestData td = GetTestData(testName);
            td.SetValue(parameterName, value);
        }

        /// <summary>
        /// Get the value of an variable in given test case
        /// </summary>
        /// <param name="testName">Name of the test case</param>
        /// <param name="parameterName">Name of the parameter</param>
        /// <returns></returns>
        public string GetValue(string testName, string parameterName)
        {
            TestData td = GetTestData(testName);
            return td.GetValue(parameterName);
        }

        /// <summary>
        /// Load the test data from an XML file.
        /// </summary>
        /// <param name="filePath">the full path of the file</param>
        /// <returns>The filename (without path)</returns>
        public static string LoadFrom(string filePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(TestSuiteData));
            using (StreamReader sr = new StreamReader(filePath))
            {
                _instance = (TestSuiteData)serializer.Deserialize(sr);
            }
            FileInfo fi = new FileInfo(filePath);
            return fi.Name;
        }

        private static TestSuiteData _instance = null;

        /// <summary>
        /// Initialize Test Data as Empty.
        /// </summary>
        public static void InitializeEmpty()
        {
            _instance = new TestSuiteData();
        }

        /// <summary>
        /// Get the instance of the current loaded TestSuiteData.
        /// </summary>
        public static TestSuiteData Current
        {
            get
            {
                if (_instance == null)
                {
                    InitializeEmpty();
                }
                return _instance;
            }
        }
    }
}
