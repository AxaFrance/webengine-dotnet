// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using AxaFrance.AxeExtended.HtmlReport;
using AxaFrance.WebEngine.Report;
using Newtonsoft.Json;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
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
        /// When activated, the framework will trace downloaded resources during your test and generates a report on the resources used.
        /// </summary>
        public bool MeasureResourceUsage { get; set; }
        Dictionary<string, NetworkRequest> requestLogs = new Dictionary<string, NetworkRequest>();
        List<JSErrors> jSErrors = new List<JSErrors>();


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

            if (IsAccessibilityTestEnabled && reportBuilder != null)
            {
                var a11yReport = reportBuilder.Build().Export();
                DebugLogger.WriteLine("[A11Y] Attaching Accessibility Report from:" + a11yReport);
                testCaseReport.AttachFile(a11yReport, "AccessibilityReport");
                DebugLogger.WriteLine("Accessibility Test Report has been attached to the test case report.");
            }
            WebDriver browser = this.Context as WebDriver;
            if (MeasureResourceUsage)
            {
                var network = browser.Manage().Network;
                network.StopMonitoring();
                stopwatch?.Stop();
                testCaseReport.AttachedData.Add(
                    new AdditionalData
                    {
                        Name = "ResourceUsage",
                        Value = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(requestLogs))
                    });
            }
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

        Stopwatch stopwatch = null;

        /// <summary>
        /// Setup process to initialize selenium web driver according to the given settings, or connect to remote mobile devices.
        /// </summary>
        public override void Initialize()
        {
            var a11y = TestData.TryGetValue("Accessibility");
            var resourceUsage = TestData.TryGetValue("ResourceUsage");
            if (IsTrue(a11y))
            {
                IsAccessibilityTestEnabled = true;
                AccessibilityReportTitle = TestData.TestName;
            }
            if (IsTrue(resourceUsage))
            {
                MeasureResourceUsage = true;
            }
            if (IsAccessibilityTestEnabled)
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
                    var driver = BrowserFactory.GetDriver(Settings.Instance.Platform, Settings.Instance.Browser, Settings.Instance.BrowserOptions);
                    Context = driver;
                    if (MeasureResourceUsage)
                    {
                        DebugLogger.WriteLine("[DEBUG] Resource Usage Measurement is enabled.");
                        var network = driver.Manage().Network;
                        network.NetworkRequestSent += NetworkRequestSent;
                        network.NetworkResponseReceived += NetworkRequestReceived;
                        network.StartMonitoring();
                    }

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

        private void NetworkRequestReceived(object sender, NetworkResponseReceivedEventArgs e)
        {
            NetworkRequest request = null;
            lock (requestLogs)
            {
                if (requestLogs.ContainsKey(e.RequestId))
                {
                    request = requestLogs[e.RequestId];
                    request.Received = DateTime.Now;
                }
                else
                {
                    request = new NetworkRequest { RequestId = e.RequestId, Url = e.ResponseUrl };
                    request.Sent = request.Received = DateTime.Now;
                    requestLogs[request.RequestId] = request;
                }
                //calcualte the size of response by adding response headers and body
                request.IsCached = IsCached(requestLogs, e);
                request.Reponse = e.ResponseHeaders.Sum(x => x.Key.Length + x.Value.Length) + (e.ResponseBody?.Length ?? 0);
                request.StatusCode = e.ResponseStatusCode;
                request.ResourceType = e.ResponseResourceType;
            }
        }

        private void NetworkRequestSent(object sender, NetworkRequestSentEventArgs e)
        {
            if (stopwatch is null)
            {
                stopwatch = new Stopwatch();
                stopwatch.Start();
            }
            var request = new NetworkRequest { RequestId = e.RequestId, Url = e.RequestUrl, Method = e.RequestMethod };
            //calcualte the size of request by adding request headers and body
            request.Request = e.RequestHeaders.Sum(x => x.Key.Length + x.Value.Length) + (e.RequestPostData?.Length ?? 0);
            lock (requestLogs)
            {
                requestLogs[request.RequestId] = request;
            }
            request.Sent = DateTime.Now;

            request.TimeStamp = stopwatch.ElapsedMilliseconds;
        }

        bool IsCached(Dictionary<string, NetworkRequest> requestLogs, NetworkResponseReceivedEventArgs e)
        {
            var hasRequestOfSameUrl = requestLogs.Values.Any(x => x.Url == e.ResponseUrl && x.Reponse != 0);
            return hasRequestOfSameUrl;
        }

        private bool IsTrue(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return false;
            if (text.Trim().Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            else if (text.Trim().Equals("yes", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            else if (text.Trim().Equals("1", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            else if (text.Trim().Equals("on", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            else if (text.Trim().Equals("enabled", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            else if (text.Trim().Equals("oui", StringComparison.OrdinalIgnoreCase))
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
