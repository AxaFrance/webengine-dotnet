// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using System;
using System.Reflection;

namespace AxaFrance.WebEngine
{
    internal class TestAssemblyHelper
    {
        internal static TestSuite LoadAssembly(string assemblyPath)
        {
            if (!assemblyPath.Contains(":"))
            {
                //relative path;
                string path = System.IO.Directory.GetCurrentDirectory();
                assemblyPath = path + "\\" + assemblyPath;
            }
            Assembly assembly = Assembly.LoadFrom(assemblyPath);
            GlobalConstants.LoadedAssemblies.Add(assembly);
            Type[] types = assembly.GetTypes();
            foreach (Type t in types)
            {
                if (t.IsSubclassOf(typeof(TestSuite)))
                {
                    TestSuite testSuite = Activator.CreateInstance(t) as TestSuite;
                    return testSuite;
                }
            }
            string errorMessage = "[ERROR] No class derived from AxaFrance.WebEngine.TestSuite has been found in the assembly.";
            DebugLogger.WriteError(errorMessage);
            throw new WebEngineGeneralException(errorMessage);

        }
    }
}
