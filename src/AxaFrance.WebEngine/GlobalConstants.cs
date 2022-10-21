// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using System.Collections.Generic;
using System.Reflection;

namespace AxaFrance.WebEngine
{
    internal static class GlobalConstants
    {
        /// <summary>
        /// XML Namespace (xmlns) used for XML nodes, such as Test Data and Test Report
        /// </summary>
        internal const string XmlNamespace = "http://www.axa.fr/WebEngine/2022";
        
        /// <summary>
        /// Indicates the loaded assembly (which contains SharedActions and/or TestSuites)
        /// </summary>
        public static List<Assembly> LoadedAssemblies { get; internal set; } = new List<Assembly>();
    }
}
