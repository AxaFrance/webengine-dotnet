// Copyright (c) 2016-2026 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Reflection;
using ExcelDna.Integration;

namespace AxaFrance.WebEngine.ExcelAddin
{
    /// <summary>
    /// Worksheet functions exposed by the add-in.
    /// </summary>
    public static class AddinFunctions
    {
        /// <summary>
        /// Returns the installed add-in version. Useful to verify that the XLL is loaded.
        /// </summary>
        [ExcelFunction(
            Name = "WEBENGINE_ADDIN_VERSION",
            Description = "Returns the version of the AXA WebEngine Excel add-in.",
            Category = "WebEngine")]
        public static string AddinVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown";
        }
    }
}
