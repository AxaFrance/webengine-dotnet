// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using OpenQA.Selenium.Appium;
using System;
using System.Text;

namespace AxaFrance.WebEngine.MobileApp
{
    /// <summary>
    /// The test case for Native Mobile application, of which a series of actions will be executed one after another.
    /// </summary>
    public class TestCaseApp : TestCase
    {
        /// <inheritdoc/>
        public override string Cleanup()
        {
            var l = (Context as AppiumDriver).Manage().Logs;
            DebugLogger.WriteLine($"There are following available logs: {string.Join(", ", l.AvailableLogTypes)}");
            foreach (var logType in l.AvailableLogTypes)
            {
                DebugLogger.WriteLine($"Log for {logType}");
                try
                {
                    StringBuilder sb = new StringBuilder();
                    var logs = l.GetLog(logType);
                    foreach (var log in logs)
                    {
                        if (log.Timestamp > startDate)
                        {
                            sb.AppendLine($"[{log.Level}] {log.Timestamp} {log.Message}");
                        }
                    }
                    DebugLogger.WriteLine(sb.ToString());
                }
                catch (Exception ex)
                {
                    DebugLogger.WriteLine("[ERROR] Get log error: " + ex.Message);
                }
            }

            if (Context is AppiumDriver ad)
            {
                try
                {
                    DebugLogger.WriteLine("Close the app");
                    ad.Close();
                    ad.Dispose();
                }
                catch { }
            }

            return string.Empty;

        }



        /// <inheritdoc/>
        public override void Initialize()
        {
            Context = AppFactory.GetDriver(Settings.Instance.Platform);
            startDate = DateTime.Now;
        }

        /// <summary>
        /// This variable is to note the start time of the test, in order to get the logs correctly.
        /// </summary>
        DateTime startDate;
    }
}
