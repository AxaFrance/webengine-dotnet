// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AXA.WebEngine.ExcelUI
{
    [Serializable]
    public class AddinSettings
    {
        /// <summary>
        /// Full path where TestData and Environment Variables will be exported.
        /// </summary>
        public string ExportPath { get; set; }
        
        /// <summary>
        /// Full path where WebRunner.exe is located (often the same location of your Test Automation Solution)
        /// </summary>
        public string WebRunnerPath { get; set; }
        
        /// <summary>
        /// The tabs excluded by exportall function.
        /// </summary>
        public string[] ExcludedTabs { get; set; }

        public BrowserType Browser { get; set; }

        /// <summary>
        /// Test Assembly. if the test automation solution is build with .NET, the compiled .dll file. If the test autmation solution is build with JAVA, the packaged .jar file.
        /// </summary>
        public string TestAssembly { get; set; }

        /// <summary>
        /// AppId for mobile testing. It can be the AppId or App Package Path.
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// Device used for mobile testing
        /// </summary>
        public string Device { get; set; }

        public AddinSettings()
        {
            ExportPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            ExcludedTabs = new string[] {
                                      "NOTE",
                                      "DATA_TABLE",
                                      "PARAMS",
                                      "ENV"};
            Browser = BrowserType.ChromiumEdge;
        }
    }
}
