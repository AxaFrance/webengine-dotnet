// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace AXA.WebEngine.Report
{
    /// <summary>
    /// The test report of a single test action (keyword)
    /// </summary>
    [Serializable]
    [XmlRoot(Namespace = GlobalConstants.XmlNamespace)]
    public class ActionReport
    {
        /// <summary>
        /// The name of the action.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// overall test result for the current test case.
        /// </summary>
        public Result Result { get; set; }

        /// <summary>
        /// The <see cref="DateTime"/> when the action is started.
        /// </summary>

        public DateTime StartTime { get; set; }

        /// <summary>
        /// The <see cref="DateTime"/> when the action is end. (including the time used to run checkpoint)
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// Calculates the Duration of the current test action;
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
        /// Contexte variables genereted during the test execution. the value presented in the report are their final values.
        /// </summary>
        public List<Variable> ContextValues { get; set; } = new List<Variable>();

        /// <summary>
        /// The result of the subactions.
        /// </summary>
        public List<ActionReport> SubActionReports { get; set; } = new List<ActionReport>();

        /// <summary>
        /// The screenshots related to the current action.
        /// </summary>
        public List<ScreenshotReport> Screenshots { get; set; } = new List<ScreenshotReport>();

        /// <summary>
        /// RAW log of the test execution, the log will be retrived from <see cref="SharedActionBase.Information"/> after execution.
        /// </summary>
        public string Log { get; set; }
    }
}
