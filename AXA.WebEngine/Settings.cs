// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security;
using System.Xml.Serialization;

[assembly: InternalsVisibleTo("WebRunner")]
[assembly: InternalsVisibleTo("WebEngine.Test")]

namespace AXA.WebEngine
{

    /// <summary>
    /// Settings of the WebEngine provides global testing parameters and behavior. settings can be loaded from external files, or provided via command line using WebRunner. 
    /// </summary>
    [Serializable]
    [XmlRoot(Namespace = GlobalConstants.XmlNamespace)]
    public class Settings
    {
        /// <summary>
        /// Determine the browser which the test will run.
        /// </summary>
        public BrowserType Browser { get; set; }

        /// <summary>
        /// Default synchronization timeout during the test. This timeout is used on Browser Synchronziation, Find Test Objects.
        /// </summary>
        public int SynchronzationTimeout { get; set; } = 20;

        /// <summary>
        /// The directory where the test framework will write the logs. WebEngine will generate a consolated report for the whole execution and may generate detailed report for teach test case.
        /// </summary>

        public string LogDir { get; set; } 


        /// <summary>
        /// The filename of the generated WebEngine XML report. not available before the log has been generated.
        /// </summary>
        public string LogFileName { get; set; }

        /// <summary>
        /// Additional capabilities for the selenium grid (if running in distance)
        /// </summary>
        public Dictionary<string, string> Capabilities { get; set; }

        private Settings() {
            string appconfig = Path.Combine(new FileInfo(typeof(Settings).Assembly.Location).DirectoryName, "appsettings.json");
            
            if (File.Exists(appconfig))
            {
                string content = File.ReadAllText(appconfig);
                var settings = Newtonsoft.Json.JsonConvert.DeserializeObject(content) as Newtonsoft.Json.Linq.JObject;
                if (settings.ContainsKey("LogDir"))
                {
                    this.LogDir = settings.Value<string>("LogDir");
                }
                if (settings.ContainsKey("GridConnection"))
                {
                    this.GridServerUrl = settings.Value<string>("GridConnection");
                }
                if (settings.ContainsKey("Username"))
                {
                    this.Username = settings.Value<string>("Username");
                }
                if (settings.ContainsKey("Password"))
                {
                    this.Password = settings.Value<string>("Password");
                }
                if (settings.ContainsKey("PackageName"))
                {
                    this.AppPackageName = settings.Value<string>("PackageName");
                }
                if (settings.ContainsKey("PackageUploadTargetUrl"))
                {
                    this.PackageUploadUrl = settings.Value<string>("PackageUploadTargetUrl");
                }
                if (settings.ContainsKey("EncryptionKey"))
                {
                    encryptionKey = settings.Value<string>("EncryptionKey");
                }
                if (settings.ContainsKey("Capabilities"))
                {
                    Capabilities = new Dictionary<string, string>();
                    var kv = settings.GetValue("Capabilities");
                    if(kv != null && kv is Newtonsoft.Json.Linq.JObject caps)
                    {
                        foreach(var cap in caps.Properties())
                        {
                            var name = cap.Name;
                            var v = cap.Value.ToString();
                            Capabilities.Add(name, v);
                        }

                    }
                }
            }

            if (string.IsNullOrEmpty(encryptionKey))
            {
                encryptionKey = "#{EncryptionKey}#";    //<- default EncryptionKey will be replaced during build via DevOps process. You can also use customized encryption key by doing it in appsettings.json
            }
            if (this.LogDir == null)
            {
                LogDir = Path.GetTempPath();
            }

        }

        internal string encryptionKey = null;

        private static Settings _settings = null;
        /// <summary>
        /// Get the active instance of Settings. Settings will be initialized by WebEngine Runner, if you are using another test framework settings should be initialized during Test Setup.
        /// </summary>
        public static Settings Instance
        {
            get
            {
                if (_settings == null)
                {
                    _settings = new Settings();
                }
                return _settings;
            }
        }

        /// <summary>
        /// [Mobile Only] The applicationId to be used (System under test)
        /// It can be: 
        /// 1. A Package Name, application already installed and will be started.
        /// 2. A local package fullpath, application will be on the device.
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// [Mobile Only] Optional. The application package name used to test Mobile Applications, for example: fr.axa.customermgmt
        /// </summary>
        public string AppPackageName { get; set; }
        
        /// <summary>
        /// When testing Mobile Application, This url is used to upload app packages such as APK or IPA.
        /// </summary>
        public string PackageUploadUrl { get; set; }

        /// <summary>
        /// [Mobile Only] Device name, Mandatary when running tests on Mobile Devices. This parameter will be passed as appium:device for device selection
        /// </summary>
        public string Device { get; set; }

        /// <summary>
        /// [Mobile Only] Version of the OS, Optional when running tests on Mobile Devices. It refers to iOS or Android version during the device selection.
        /// </summary>
        public string OsVersion { get; set; }

        /// <summary>
        /// [Mobile Only] The platform for mobile testing. Mandatary for Mobile testing: Possible values Android / iOS
        /// </summary>
        public Platform Platform { get; set; }

        /// <summary>
        /// [Grid Only] Username used to connect to remote Selenium Grid testing platforms (local Selenium Grid or cloud provider such as BrowserStack)
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// [Grid Only] Password or AccessKey used to connect to remote Selenium Grid or compatible cloud based mobile testing platforms
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// [Mobile Only] [Grid Only] Remote Selenium Grid Hub to connect.
        /// If platform is Android and IOS, this value is mandatary (default value = http://localhost:4723/wd/hub) for running tests on local Appium Server.
        /// </summary>
        public string GridServerUrl { get; set; } = "http://localhost:4723/wd/hub";

        /// <summary>
        /// [Grid Only] Run Desktop based Selenium test in the Grid (default value is false, that is always run desktop tests locally)
        /// When value is true, the framework will create Selenium Grid connection provided by <see cref="GridServerUrl"/> 
        /// </summary>
        public bool DesktopGridEnabled { get; set; } = false;

        /// <summary>
        /// Settings about report generating, including additional formats and locations.
        /// </summary>
        public ReportSettings ReportSettings { get; set; } = new ReportSettings();

        /// <summary>
        /// TestSuite to be executed. The test suite object is loaded by WebRunner.
        /// </summary>
        internal TestSuite TestSuite { get; set; }

        /// <summary>
        /// Indicates whether the test execution should done via manuel mode.
        /// When manuel mode is active, the execution will be paused before Test Case cleanup. User can do some manual manipuilation on system under test.
        /// </summary>
        public bool ManualMode { get; set; }

        /// <summary>
        /// The filename of the data source.
        /// </summary>
        public string DataSourceName { get; set; }
        
        /// <summary>
        /// If webrunner should try to show test report after test execution.
        /// </summary>
        public bool ShowReportAfterTest { get; set; }
        
        /// <summary>
        /// The CSV Seperator used during the data process. default value is semicolon (;)
        /// </summary>
        public string Separator { get; set; }

        /// <summary>
        /// Allow any HTTPS Certificate when creating Selenium Grid connection.
        /// </summary>
        public bool AllowAnyCertificate { get; set; }
        public bool UseJavaScriptClick { get; internal set; }
    }
}
