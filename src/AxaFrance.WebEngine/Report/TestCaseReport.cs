﻿// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace AxaFrance.WebEngine.Report
{
    /// <summary>
    /// Test report for a single test case
    /// </summary>
    [Serializable]
    [XmlRoot(Namespace = GlobalConstants.XmlNamespace)]
    public class TestCaseReport
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="TestCaseReport"/> class.
        /// </summary>
        public TestCaseReport()
        {
            StartTime = DateTime.Now;
            EndTime = DateTime.Now;
            Result = Result.None;
        }

        /// <summary>
        /// The unique identifier of the report item, generated automaticaly and used for showing report
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// The name of test case
        /// </summary>
        public string TestName { get; set; }

        /// <summary>
        /// The log of the test case.
        /// </summary>
        public string Log { get; set; }

        /// <summary>
        /// overall test result for the current test case.
        /// </summary>
        public Result Result { get; set; }

        /// <summary>
        /// The <see cref="DateTime"/> when the test is started for execution.
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// The <see cref="DateTime"/> when the test execution for the test case ends.
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Calculates the Duration of the current test case;
        /// </summary>
        public TimeSpan Duration
        {
            get
            {
                return EndTime - StartTime;
            }
        }

        /// <summary>
        /// Calculates the Duration of the current test case and convert to readable text.
        /// </summary>
        public string DurationText
        {
            get
            {
                if (Duration.TotalSeconds <= 0)
                {
                    return string.Empty;
                }
                else if (Duration.TotalSeconds < 1)
                {
                    return "(<1s)";
                }
                else if (Duration.TotalSeconds <= 60)
                {
                    return string.Format("({0}s)", (int)Duration.TotalSeconds);
                }
                else
                {
                    return string.Format("({0}m{1}s)", (int)(Duration.TotalSeconds / 60), (int)(Duration.TotalSeconds % 60));
                }
            }
        }

        /// <summary>
        /// Test data has used during the text execution
        /// </summary>
        public List<Variable> TestData { get; set; } = new List<Variable>();

        /// <summary>
        /// Contexte variables genereted during the test execution.
        /// </summary>
        public List<Variable> ContextValues { get; set; } = new List<Variable>();

        /// <summary>
        /// Global Contexte variables genereted during the test execution
        /// </summary>
        public List<Variable> GlobalContextValues { get; set; } = new List<Variable>();


        /// <summary>
        /// The result of the test steps 
        /// </summary>
        public List<ActionReport> ActionReports { get; set; } = new List<ActionReport>();

        /// <summary>
        /// Attach an external file to the test report.
        /// </summary>
        /// <param name="fileName">the fullpath of the file.</param>
        /// <param name="fileType">the type of the file, possible values: AccessibilityReport, ResourceUsage</param>
        public void AttachFile(string fileName, string fileType)
        {
            AttachedData.Add(new AdditionalData()
            {
                Name = fileType,
                Value = File.ReadAllBytes(fileName)
            });
        }

        /// <summary>
        /// Attach an object to the test report
        /// </summary>
        /// <param name="o">object to attach, the object will be serialized as json</param>
        /// <param name="fileType">the type of the file, possible values: AccessibilityReport, ResourceUsage</param>
        public void Attach(object o, string fileType)
        {
            AttachedData.Add(new AdditionalData()
            {
                Name = fileType,
                Value = System.Text.Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(o))
            });
        }

        /// <summary>
        /// Additional data attached to the test report. for example: accessibility scan results, resource usage report, etc...
        /// </summary>
        public List<AdditionalData> AttachedData { get; set; } = new List<AdditionalData>();
    }
}
