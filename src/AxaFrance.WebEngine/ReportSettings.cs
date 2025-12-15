// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using System;
using System.Xml.Serialization;

namespace AxaFrance.WebEngine
{
    /// <summary>
    /// Settings of the WebEngine privides global
    /// </summary>
    [Serializable]
    [XmlRoot(Namespace = GlobalConstants.XmlNamespace)]
    public class ReportSettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether reports are saved to a unique folder suffixed by date and time.
        /// </summary>
        public bool UniqueReportFolder { get; set; } = false;

        /// <summary>
        /// If a Junit compatible report should be generated after test execution
        /// </summary>
        public bool JUnitReport { get; set; }

        /// <summary>
        /// The location of the JUnit report.
        /// </summary>
        public string JUnitReportPath { get; set; }

        /// <summary>
        /// If a Nunit compatible report shouldbe generated after test execution
        /// </summary>
        public bool NUnitReport { get; set; }

        /// <summary>
        /// The location of the NUnit report. (default location will be <see cref="Settings.LogDir"/>)
        /// </summary>
        public string NUnitReportPath { get; set; }

        /// <summary>
        /// Indicates if a HTML report should be generated after test execution
        /// </summary>
        public bool HtmlReport { get; set; }

        /// <summary>
        /// The location of the HTML report. (default location will be <see cref="Settings.LogDir"/>)
        /// </summary>
        public string HtmlReportPath { get; set; }
    }

}
