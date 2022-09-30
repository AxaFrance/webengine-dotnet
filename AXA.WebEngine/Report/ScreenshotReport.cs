// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-6-7 14:38

using System;
using System.Xml.Serialization;

namespace AXA.WebEngine.Report
{
    /// <summary>
    /// The screenshot that is included in the test report. Screenshot can be visualized via ReportViewer
    /// </summary>
    [Serializable]
    [XmlRoot(Namespace = GlobalConstants.XmlNamespace)]
    public class ScreenshotReport
    {
        /// <summary>
        /// The name or brief descrption of the screenshot.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The binary data of the screenshot (as png or jpg file stream)
        /// </summary>
        public byte[] Data { get; set; }
    }
}
