// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-8-1 10:28
using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace AXA.WebEngine.Runner
{
    internal static class ParameterParser
    {
        

        /// <summary>
        /// Parse parameters from command-line parameters and configuration file
        /// </summary>
        /// <param name="args">command-line parameters</param>
        /// <returns>The parsed settings for further use.</returns>
        internal static Settings ParseParameters(string[] args)
        {
            Settings s = Settings.Instance;
            ParseEncrypt(args, s);
            ParseAssembly(args, s);
            ParseManualMode(args, s);
            ParseHideConsole(args, s);
            ParseEnvironmentVariables(args, s);
            ParseTestData(args, s);
            ParseBrowser(args, s);
            ParsePlatform(args, s);
            ParseDevice(args, s);
            ParseOsVersion(args, s);
            ParseAppId(args, s);
            ParseGridOption(args, s);
            ParseForceClose(args, s);
            ParseOutputFolder(args, s);
            ParseReportOption(args, s);
            ParseShowReportOption(args, s);
            return s;
        }

        private static void ParseEncrypt(string[] args, Settings s)
        {
            const string arg = "-encyptionKey:";
            string encrypt = args.FirstOrDefault(x => x.StartsWith(arg, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(encrypt))
            {
                string encryptionKey = encrypt.Replace(arg, string.Empty);
                s.encryptionKey = encryptionKey;
            }
        }

        private static void ParseShowReportOption(string[] args, Settings s)
        {
            if(args.FirstOrDefault(x=> x.Equals("-showreport", StringComparison.OrdinalIgnoreCase)) != null)
            {
                s.ShowReportAfterTest = true;
                DebugLogger.WriteLine("[CONFIG] Show Report in ReportViewer after test execution.");

            }
        }

        private static void ParseReportOption(string[] args, Settings s)
        {
            const string arg = "-junit:";
            string junit = args.FirstOrDefault(x => x.StartsWith(arg, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrEmpty(junit))
            {
                string outputPath = junit.Replace(arg, string.Empty);
                outputPath = outputPath.TrimEnd('\"');
                s.ReportSettings.JUnitReport = true;
                s.ReportSettings.JUnitReportPath = outputPath;
                DebugLogger.WriteLine("[CONFIG] Generated JUnit Report: " + s.ReportSettings.JUnitReportPath);

            }

        }


        private static void ParseOutputFolder(string[] args, Settings s)
        {
            const string arg = "-outputDir:";
            string outputDir = args.FirstOrDefault(x => x.StartsWith(arg, StringComparison.InvariantCultureIgnoreCase));
            if (!string.IsNullOrEmpty(outputDir))
            {
                outputDir = outputDir.Replace(arg, string.Empty);
                s.LogDir = outputDir;
                
            }
            DebugLogger.WriteLine("[CONFIG] Output Directory: " + s.LogDir);
        }

        private static void ParseGridOption(string[] args, Settings s)
        {
            var arg = "-grid:";
            string gridSettings = args.FirstOrDefault(x => x.StartsWith(arg, StringComparison.InvariantCultureIgnoreCase));
            //if provided 
            if (!string.IsNullOrEmpty(gridSettings))
            {
                gridSettings = gridSettings.Replace(arg, string.Empty);
                s.GridServerUrl = gridSettings;
            }

            arg = "-username:";
            string username = args.FirstOrDefault(x => x.StartsWith(arg, StringComparison.InvariantCultureIgnoreCase));
            if (!string.IsNullOrEmpty(username))
            {
                username = username.Replace(arg, string.Empty);
                s.Username = username;
            }

            arg = "-password:";
            string password = args.FirstOrDefault(x => x.StartsWith(arg, StringComparison.InvariantCultureIgnoreCase));
            if (!string.IsNullOrEmpty(password))
            {
                password = password.Replace(arg, string.Empty);
                s.Password = password;
            }
        }

        private static void ParseForceClose(string[] args, Settings s)
        {
            //TODO:Parse Force Close to close all open browsers.
        }

        /// <summary>
        /// Retrives the appId parameter for 
        /// </summary>
        /// <param name="args">arguments provided from commandline</param>
        /// <param name="s">Settings object</param>

        private static void ParseAppId(string[] args, Settings s)
        {
            const string arg = "-appid:";
            string appId = args.FirstOrDefault(x => x.StartsWith(arg, StringComparison.InvariantCultureIgnoreCase));
            if (!string.IsNullOrEmpty(appId))
            {
                appId = appId.Substring(arg.Length);
                s.AppId = appId;
                DebugLogger.WriteLine("[CONFIG] Mobile applcation being tested: " + appId);
            }
            else
            {
                if (Settings.Instance.Browser == BrowserType.AndroidNative || Settings.Instance.Browser == BrowserType.IOSNative)
                {
                    const string errorMessage = "[ERROR] Test on device but Application Id not provided.";
                    DebugLogger.WriteError(errorMessage);
                    throw new WebEngineGeneralException(errorMessage);
                }
            }
        }

        private static void ParseOsVersion(string[] args, Settings s)
        {
            const string para = "-osversion:";
            string osVersion = args.FirstOrDefault(x => x.StartsWith(para, StringComparison.InvariantCultureIgnoreCase));
            if (!string.IsNullOrEmpty(osVersion))
            {
                osVersion = osVersion.Substring(para.Length);
                s.OsVersion = osVersion;
                DebugLogger.WriteLine("[CONFIG] Device OS Version: " + s.OsVersion);
            }
        }

        private static void ParseDevice(string[] args, Settings s)
        {
            const string para = "-device:";
            string device = args.FirstOrDefault(x => x.StartsWith(para));
            if (!string.IsNullOrEmpty(device))
            {
                s.Device = device.Replace(para, string.Empty);
                DebugLogger.WriteLine("[CONFIG] Test Device: " + s.Device);
            }
        }

        private static void ParsePlatform(string[] args, Settings s)
        {
            string platform = args.FirstOrDefault(x => x.StartsWith("-platform:"));
            if (!string.IsNullOrEmpty(platform))
            {
                platform = platform.Replace("-platform:", string.Empty);
                switch (platform.ToLower())
                {
                    case "android":
                        s.Platform = Platform.Android;
                        break;
                    case "ios":
                        s.Platform = Platform.iOS;
                        break;
                    default:
                        s.Platform = Platform.Windows;
                        break;
                }
            }
            else
            {
                s.Platform = Platform.Windows;
            }
            DebugLogger.WriteLine("[CONFIG] Test Platform: " + s.Platform);
        }

        private static void ParseBrowser(string[] args, Settings s)
        {
            string browser_param = args.First(x => x.StartsWith("-browser:"));
            string browser = browser_param.Replace("-browser:", string.Empty);

            switch (browser.ToUpper())
            {
                case "IE":
                case "INTERNETEXPLORER":
                    s.Browser = BrowserType.InternetExplorer;
                    DebugLogger.WriteLine("[CONFIG] Browser : Internet Explorer");
                    break;
                case "FIREFOX":
                    s.Browser = BrowserType.Firefox;
                    DebugLogger.WriteLine("[CONFIG] Browser : Firefox");
                    break;
                case "CHROME":
                    s.Browser = BrowserType.Chrome;
                    DebugLogger.WriteLine("[CONFIG] Browser : Chrome");
                    break;
                case "EDGE":
                case "EDGE_CHROMIUM":
                case "CHROMIUM_EDGE":
                case "EDGECHROMIUM":
                case "CHROMIUMEDGE":
                    s.Browser = BrowserType.ChromiumEdge;
                    DebugLogger.WriteLine("[CONFIG] Browser : Chromium Edge");
                    break;
                case "IOS_NATIVE":
                case "IOSNATIVE":
                    s.Browser = BrowserType.IOSNative;
                    DebugLogger.WriteLine("[CONFIG] Platform : IOS Native Application");
                    break;
                case "ANDROID_NATIVE":
                case "ANDROIDNATIVE":
                    s.Browser = BrowserType.AndroidNative;
                    DebugLogger.WriteLine("[CONFIG] Platform : Android");
                    break;
                case "SAFARI":
                    s.Browser = BrowserType.Safari;
                    DebugLogger.WriteLine("[CONFIG] Browser : iOS Safari based browser");
                    break;
                default:
                    string info = $"[CONFIG] Browser : {browser} is not a recognized browser";
                    DebugLogger.WriteLine(info);
                    throw new WebEngineGeneralException(info);
            }
        }
        private static void ParseTestData(string[] args, Settings s)
        {
            string dataParam = args.FirstOrDefault(x => x.StartsWith("-data:"));
            if (!string.IsNullOrEmpty(dataParam))
            {
                var dataSource = dataParam.Replace("-data:", string.Empty);
                DebugLogger.WriteLine("[CONFIG] Test Data :" + dataSource);
                s.DataSourceName = TestSuiteData.LoadFrom(dataSource);
            }
            else
            {
                DebugLogger.WriteWarning("[CONFIG] No test data is provided.");
                TestSuiteData.InitializeEmpty();
                s.DataSourceName = "NoTestData";
            }
        }

        private static void ParseEnvironmentVariables(string[] args, Settings s)
        {
            const string arg = "-env:";
            string env_param = args.FirstOrDefault(x => x.StartsWith("-env:"));
            try
            {
                if (!string.IsNullOrEmpty(env_param))
                {
                    string env = env_param.Replace(arg, string.Empty);
                    DebugLogger.WriteLine("[CONFIG] Environment Variables : " + env);
                    EnvironmentVariables.LoadFrom(env);
                }
                else
                {
                    DebugLogger.WriteLine("[CONFIG] No Environment Variables provided.");
                }
            }
            catch (Exception ex)
            {
                DebugLogger.WriteLine("[ERROR] Environment Variables are not loaded: " + ex.Message);
                throw;
            }
        }

        private static void ParseHideConsole(string[] args, Settings s)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
            {
                var v = args.FirstOrDefault(x => x == "-hide" || x == "-h");
                if (v != null)
                {
                    var process = Process.GetCurrentProcess();
                    var handle = process.MainWindowHandle;
                    if (handle != IntPtr.Zero)
                    {
                        ShowWindow(handle, 2);
                    }
                }
            }

        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

        private static void ParseManualMode(string[] args, Settings s)
        {
            var v = args.FirstOrDefault(x => x == "-m");
            if (v == null)
            {
                s.ManualMode = false;
            }
            else
            {
                s.ManualMode = true;
                DebugLogger.WriteLine("[CONFIG] Manual Mode: Test will be paused before clean-up if error occurs. User intervention needed to continue.");
            }
        }

        private static void ParseAssembly(string[] args, Settings s)
        {
            string assembly_param = args.FirstOrDefault(x => x.StartsWith("-a:"));
            if (string.IsNullOrEmpty(assembly_param))
            {
                const string info = "Test assembly is not provided. Please use -a argument to provide your test assembly.";
                DebugLogger.WriteError(info);
                throw new WebEngineGeneralException(info);
            }
            var assemblyPath = assembly_param.Replace("-a:", string.Empty);
            DebugLogger.WriteLine($"[CONFIG] Test Assembly: {assemblyPath}");
            TestSuite ts = TestAssemblyHelper.LoadAssembly(assemblyPath);
            s.TestSuite = ts;
        }
    }

}
