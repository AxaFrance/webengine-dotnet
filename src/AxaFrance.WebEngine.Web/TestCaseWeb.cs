// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using OpenQA.Selenium;
using System;
using System.Configuration;
using System.Threading;

namespace AxaFrance.WebEngine.Web
{
    /// <summary>
    /// The test case for Web Applications. 
    /// </summary>
    public abstract class TestCaseWeb : TestCase
    {
        /// <summary>
        /// Clean up script to close browsers and disconnected to the device if necessary.
        /// </summary>
        /// <returns>Additional information which can be showed in the report. (not used)</returns>
        public override string Cleanup()
        {
            WebDriver browser = this.Context as WebDriver;
            if (browser != null)
            {
                try
                {
                    browser.Quit();
                    this.Context = null;
                }
                catch
                {
                    //clean up failed may be caused by exception while the browser is exceptionally closed.
                }
            }
            return null;
        }

        /// <summary>
        /// Setup process to initialize selenium web driver according to the given settings, or connect to remote mobile devices.
        /// </summary>
        public override void Initialize()
        {
            Settings s = Settings.Instance;
            string url = s.GridServerUrl;
            if (url != null && url.Contains("browserstack"))
            {
                DebugLogger.WriteLine("Test on remote appium compatible server: Browserstack");
            }

            Thread t = new Thread(new ThreadStart(() =>
            {
                try
                {
                    DebugLogger.WriteLine("Initializing Selenium WebDriver");
                    Context = BrowserFactory.GetDriver(Settings.Instance.Platform, Settings.Instance.Browser);
                }
                catch (Exception ex)
                {
                    DebugLogger.WriteLine("There is an Exception when initializing Selenium WebDriver.");
                    DebugLogger.WriteLine(ex.ToString());
                    throw;
                }
            }));
            t.Start();
            bool terminated = t.Join(new TimeSpan(0, 3, 0));
            if (!terminated)
            {
                //LeanFT has not been initalized within 3 minutes.
                throw new WebEngineGeneralException("Selenium WebDriver has not been initialized within 3 minutes. [Critical Error]");
            }
            try
            {
                (Context as IWebDriver)?.Manage().Cookies.DeleteAllCookies();
            }
            catch
            {
                //ignore any error during the cookie removing
            }


        }

    }
}
