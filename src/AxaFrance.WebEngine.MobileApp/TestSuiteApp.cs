// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.iOS;
using System.Configuration;

namespace AxaFrance.WebEngine.MobileApp
{
    /// <summary>
    /// Representes a Test suite for the native mobile application testing.
    /// </summary>
    public abstract class TestSuiteApp : TestSuite
    {
        static WebDriver driver;

        /// <summary>
        /// gets the currently using WebDriver (AndroidDriver or IOSDriver for example)
        /// </summary>
        public static WebDriver CurrentDriver
        {
            get
            {
                return driver;
            }
        }

        /// <summary>
        /// Cleanup method to close the uninstall the test application.
        /// </summary>
        /// <param name="s">Test settings (not used in this contexte)</param>
        public override void CleanUp(Settings s)
        {
            if (driver is AndroidDriver androidDriver)
            {
                try
                {
                    androidDriver.CloseApp();
                    androidDriver.Close();
                }
                catch { }
                try
                {
                    androidDriver.RemoveApp(s.AppPackageName);
                }
                catch { }
                androidDriver.Quit();
            }
            else if (driver is IOSDriver iosDriver)
            {
                try
                {
                    iosDriver.CloseApp();
                    iosDriver.Close();
                }
                catch { }
                try
                {
                    iosDriver.RemoveApp(s.AppPackageName);
                }
                catch { }
                iosDriver.Quit();
            }
        }


        /// <summary>
        /// Initialize the connection to Selenium Grid compatible device Cloud and install the application package (if using browserstack)
        /// </summary>
        /// <param name="s">The test settings.</param>
        public override void Initialize(Settings s)
        {
            //Specificity of Browserstack: if the AppId is not started with bs:, then upload the package using browserstack API
            if (s.GridServerUrl.Contains("browserstack"))
            {
                DebugLogger.WriteLine("Test on remote appium compatible server: Browserstack");
                string packagePath = s.AppId;
                if (!s.AppId.StartsWith("bs:"))
                {
                    var id = AppFactory.UploadToBrowserstack(packagePath, s.Username, s.Password, s.PackageUploadUrl).Result;
                    s.AppId = id;
                    DebugLogger.WriteLine($"Application Id privided by Browserstack is: {id}");
                }
            }
            else if (s.GridServerUrl.Contains("moiblelab"))
            {
                DebugLogger.WriteLine("Test on remote appium compatible server: Mobile Lab Service");
                string packagePath = s.AppId;
                if (!s.AppId.StartsWith("http"))
                {
                    var id = AppFactory.UploadToMobileLab(s.Password, packagePath, s.PackageUploadUrl).Result;
                    s.AppId = id;
                    DebugLogger.WriteLine($"Application Id privided by Mobile Lab service is: {id}");
                }
            }
            driver = AppFactory.GetDriver(s.Platform);
        }
    }
}
