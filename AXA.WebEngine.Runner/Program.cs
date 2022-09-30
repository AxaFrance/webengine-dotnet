// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using AXA.WebEngine.Report;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace AXA.WebEngine.Runner
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            if (args[0].Equals("-encrypt"))
            {
                Encrypt(args[1]);
                return;
            };

            Settings settings = ParameterParser.ParseParameters(args);
            var testsuite = settings.TestSuite;
            testsuite.Initialize(settings);
            var result = testsuite.Run();
            testsuite.CleanUp(settings);
            string logFileName = $"{settings.TestSuite.GetType().Name}_{settings.DataSourceName}";
            result.HostName = Environment.MachineName;
            result.SystemError = DebugLogger.SystemError.ToString();
            result.SystemOut = DebugLogger.SystemOutput.ToString();
            if (settings.ReportSettings.JUnitReport)
            {
                var jReport = ReportHelper.GenerateJUnitReport(result, settings.DataSourceName, settings.ReportSettings.JUnitReportPath);
                DebugLogger.WriteLine("JUnit Report Generated: " + jReport);
            }

            var reportPath = result.SaveAs(settings.LogDir, logFileName, false);
            DebugLogger.WriteLine("Report Generated: " + reportPath);
            try
            {
                result.GenereteCSV(Path.Combine(settings.LogDir, "GlobalOutput.csv"), Settings.Instance.Separator, false);
            }
            catch { }
            if (settings.ShowReportAfterTest)
            {
                try
                {
                    ShowReport(reportPath);
                }
                catch (Exception ex)
                {
                    DebugLogger.WriteError("Error occured when opening the test report.");
                    DebugLogger.WriteError(ex.Message);
                }
            }
        }

        private static void Encrypt(string data)
        {
            var encryptedData = Encrypter.Encrypt(data);
            Console.WriteLine($"Encrypted data is: {encryptedData}");
        }

        private static void ShowReport(string reportPath)
        {
            string path = Path.GetDirectoryName(Assembly.GetAssembly(typeof(Program)).Location);
            string reportViewerPath = Path.Combine(path, "ReportViewer.exe");
            if (File.Exists(reportViewerPath))
            {
                System.Diagnostics.Process p = new System.Diagnostics.Process()
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo("ReportViewer.exe", $"\"{reportPath}\"")
                    {
                        WorkingDirectory = path,
                    }
                };
                p.Start();
            }

        }
    }
}
