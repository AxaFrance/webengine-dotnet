// Copyright (c) 2016-2026 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AxaFrance.WebEngine;

namespace AxaFrance.WebEngine.ExcelAddin
{
    /// <summary>
    /// Settings persisted for the Excel add-in.
    /// </summary>
    public sealed class AddinSettings
    {
        /// <summary>
        /// Full path where test data and environment variables are exported.
        /// </summary>
        public string ExportPath { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

        /// <summary>
        /// Folder containing WebRunner.exe or WebRunner.jar.
        /// </summary>
        public string WebRunnerPath { get; set; } = string.Empty;

        /// <summary>
        /// Compiled test project (DLL) or packaged Java test project (JAR).
        /// </summary>
        public string TestAssembly { get; set; } = string.Empty;

        /// <summary>
        /// Browser used by default when launching tests.
        /// </summary>
        public BrowserType Browser { get; set; } = BrowserType.ChromiumEdge;

        /// <summary>
        /// Last application id used for mobile testing.
        /// </summary>
        public string? AppId { get; set; }

        /// <summary>
        /// Last device used for mobile testing.
        /// </summary>
        public string? Device { get; set; }

        /// <summary>
        /// Worksheets that are not test data and must not be exported.
        /// </summary>
        public string[] ExcludedTabs { get; set; } = { "NOTE", "DATA_TABLE", "PARAMS", "ENV" };

        /// <summary>
        /// Creates an independent copy of these settings.
        /// </summary>
        public AddinSettings Clone()
        {
            return new AddinSettings
            {
                ExportPath = ExportPath,
                WebRunnerPath = WebRunnerPath,
                TestAssembly = TestAssembly,
                Browser = Browser,
                AppId = AppId,
                Device = Device,
                ExcludedTabs = ExcludedTabs == null ? Array.Empty<string>() : (string[])ExcludedTabs.Clone(),
            };
        }

        /// <summary>
        /// Returns true when the given worksheet must be ignored.
        /// </summary>
        public bool IsExcludedSheet(string sheetName)
        {
            return ExcludedTabs != null
                && ExcludedTabs.Any(x => string.Equals(x, sheetName, StringComparison.OrdinalIgnoreCase));
        }
    }
}
