// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using AxaFrance.WebEngine.Report;
using AxaFrance.AxeExtended.HtmlReport;
using Deque.AxeCore.Selenium;
using OpenQA.Selenium;
using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Threading;

namespace AxaFrance.WebEngine.Web
{
    /// <summary>
    /// The test case for Web Applications. 
    /// </summary>
    public abstract class TestCaseWeb : TestCase
    {

        /// <summary>
        /// Accessibility Report Builder for current test case.
        /// If accessibility test is enabled, the report builder will be initialized.
        /// </summary>
        internal OverallReportBuilder reportBuilder;

        /// <summary>
        /// The test case report for the current test case.
        /// </summary>
        public bool IsAccessibilityTestEnabled { get; set; }
        
        /// <summary>
        /// The title of the accessibility report.
        /// </summary>
        public string AccessibilityReportTitle { get; set; }

        /// <summary>
        /// Clean up script to close browsers and disconnected to the device if necessary.
        /// </summary>
        /// <returns>Additional information which can be showed in the report. (not used)</returns>
        public override string Cleanup()
        {
            
            if(IsAccessibilityTestEnabled && reportBuilder != null)
            {
                var a11yReport = reportBuilder.Build().Export();
                DebugLogger.WriteLine("[A11Y] Attaching Accessibility Report from:" + a11yReport) ;
                testCaseReport.AttachFile(a11yReport, "AccessibilityReport");
                DebugLogger.WriteLine("Accessibility Test Report has been attached to the test case report.");
            }
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
            var a11y = TestData.TryGetValue("Accessibility");
            if(IsTrue(a11y))
            {
                IsAccessibilityTestEnabled = true;
                AccessibilityReportTitle = TestData.TestName;
            }
            if(IsAccessibilityTestEnabled)
            {
                reportBuilder = InitializeA11yReportBuilder();
            }            
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
                    Context = BrowserFactory.GetDriver(Settings.Instance.Platform, Settings.Instance.Browser, Settings.Instance.BrowserOptions);
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

        private bool IsTrue(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return false;
            if (text.Trim().Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }else if(text.Trim().Equals("yes", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }else if(text.Trim().Equals("1", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }else if(text.Trim().Equals("on", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }else if(text.Trim().Equals("enabled", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }else if(text.Trim().Equals("oui", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        private OverallReportBuilder InitializeA11yReportBuilder()
        {
            DebugLogger.WriteLine("[A11Y] Initialize Accessibility Report Builder");
            var defaultOptions = new PageReportOptions()
            {
                HighlightColor = Color.OrangeRed,
                HighlightThickness = 5,
                UseAdvancedScreenshot = true,                
                OutputFormat = OutputFormat.Zip,
                OutputFolder = Path.Combine(Settings.Instance.LogDir, "a11y"),
                ScoringMode = ScoringMode.NonWeighted,
                //Tags = Array.Empty<string>(),
                Title = AccessibilityReportTitle ?? "Accessibility Test Report",
            };
            return new OverallReportBuilder().WithDefaultOptions(defaultOptions);
        }
    }
}
