// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using System;
using System.Threading.Tasks;

namespace AxaFrance.WebEngine.MobileApp
{
    /// <summary>
    /// Representes a Test suite for the native mobile application testing.
    /// </summary>
    public abstract class TestSuiteApp : TestSuite
    {

        /// <summary>
        /// Initialize the connection to Selenium Grid compatible device Cloud and install the application package (if using browserstack)
        /// </summary>
        /// <param name="s">The test settings.</param>
        public override void Initialize(Settings s)
        {
            //Specificity of Browserstack: if the AppId is not started with bs:, then upload the package using browserstack API
            if (s.GridServerUrl.Contains("browserstack", StringComparison.OrdinalIgnoreCase))
            {
                DebugLogger.WriteLine("Test on remote appium compatible server: Browserstack");
                string packagePath = s.AppId;
                if (!s.AppId.StartsWith("bs:"))
                {
                    // Run the upload on the thread pool so a captured UI synchronization context cannot deadlock.
                    var id = Task.Run(() => AppFactory.UploadToBrowserstack(packagePath, s.Username, s.Password, s.PackageUploadUrl))
                        .GetAwaiter().GetResult();
                    s.AppId = id;
                    DebugLogger.WriteLine($"Application Id provided by Browserstack is: {id}");
                }
            }
            else if (s.GridServerUrl.Contains("mobilelab", StringComparison.OrdinalIgnoreCase)
                || s.GridServerUrl.Contains("moiblelab", StringComparison.OrdinalIgnoreCase))
            {
                DebugLogger.WriteLine("Test on remote appium compatible server: Mobile Lab Service");
                string packagePath = s.AppId;
                if (!s.AppId.StartsWith("http"))
                {
                    var id = Task.Run(() => AppFactory.UploadToMobileLab(s.Password, packagePath, s.PackageUploadUrl))
                        .GetAwaiter().GetResult();
                    s.AppId = id;
                    DebugLogger.WriteLine($"Application Id provided by Mobile Lab service is: {id}");
                }
            }
            //driver = AppFactory.GetDriver(s.Platform);
        }
    }
}
