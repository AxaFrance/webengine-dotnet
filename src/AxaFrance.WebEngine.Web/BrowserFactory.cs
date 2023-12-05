using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.ImageComparison;
using OpenQA.Selenium.Appium.iOS;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.Safari;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using static System.Environment;

[assembly: InternalsVisibleTo("WebRunner")]
[assembly: InternalsVisibleTo("WebEngine.Test")]

namespace AxaFrance.WebEngine.Web
{
    /// <summary>
    /// BrowserHelper is a tooling class helps user to get the Selenium WebDriver object from the given context : platform, browserType.
    /// </summary>
    public static class BrowserFactory
    {


        static string workingDirectory = System.Environment.GetFolderPath(SpecialFolder.ApplicationData) + "\\AxaFrance.WebEngine";

        /// <summary>
        /// The unique function initialize the WebDriver on the target platform and browser and returns the WebDriver, AndroidDriver or IOSDriver according to contexte.
        /// </summary>
        /// <param name="platform">Android, iOS or Windows</param>
        /// <param name="browserType">The browserType of which the test should be executed on.</param>
        /// <returns>WebDriver object</returns>
        /// <remarks>
        /// <para>
        /// If you are another test framework, you must provide additional information to <see cref="Settings.Instance"/>, especially when testing on Mobile Devices.
        /// </para>
        /// <para>
        /// This method will take account Desktop browsers and Mobile Devices. If <paramref name="platform"/> is valorized to 'Android'
        /// the WebDriver object will be type of <see cref="AndroidDriver"/>, allows doing specific controls on the device. If <paramref name="platform"/> is
        /// valorized to 'iOS', the WebDriver will be typeof <see cref="IOSDriver"/>. Otherwise, An ordinary <see cref="WebDriver"/> us returned.
        /// </para>
        /// <para>
        /// When using mobile devices: under Android the native automation framework is UiAutomator2, under IOS the native automation framework used is XCUITest
        /// </para>
        /// </remarks>
        /// <exception cref="PlatformNotSupportedException" />
        public static WebDriver GetDriver(Platform platform, AxaFrance.WebEngine.BrowserType browserType)
        {
            return GetDriver(platform, browserType, new string[0]);
        }

        /// <summary>
        /// The unique function initialize the WebDriver on the target platform and browser and returns the WebDriver, AndroidDriver or IOSDriver according to contexte.
        /// </summary>
        /// <param name="platform">Android, iOS or Windows</param>
        /// <param name="browserType">The browserType of which the test should be executed on.</param>
        /// <param name="browserOptions">A List of webDriver options you want to use for Web testings.</param>
        /// <returns>WebDriver object</returns>
        /// <remarks>
        /// <para>
        /// If you are another test framework, you must provide additional information to <see cref="Settings.Instance"/>, especially when testing on Mobile Devices.
        /// </para>
        /// <para>
        /// This method will take account Desktop browsers and Mobile Devices. If <paramref name="platform"/> is valorized to 'Android'
        /// the WebDriver object will be type of <see cref="AndroidDriver"/>, allows doing specific controls on the device. If <paramref name="platform"/> is
        /// valorized to 'iOS', the WebDriver will be typeof <see cref="IOSDriver"/>. Otherwise, An ordinary <see cref="WebDriver"/> us returned.
        /// </para>
        /// <para>
        /// When using mobile devices: under Android the native automation framework is UiAutomator2, under IOS the native automation framework used is XCUITest
        /// </para>
        /// </remarks>
        /// <exception cref="PlatformNotSupportedException" />
        public static WebDriver GetDriver(Platform platform, BrowserType browserType, IEnumerable<string> browserOptions)
        {
            Settings.Instance.Platform = platform;
            Settings.Instance.Browser = browserType;

            //Merge browser options provided by this function and appsettings.json
            var arguments = Settings.Instance.BrowserOptions.ToList();
            foreach (var option in browserOptions)
            {
                if (!arguments.Contains(option))
                {
                    arguments.Add(option);
                }
            }



#if NET48_OR_GREATER || NET6_0_OR_GREATER
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 |  SecurityProtocolType.Tls13;
#else
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
#endif


            //Apple Safari driver does not support Click(), we need to active UseJavaScriptClick if target browser is Safari and do JS Click instead.
            if (browserType == BrowserType.Safari)
            {
                Settings.Instance.UseJavaScriptClick = true;
            }

            if (platform == Platform.Windows || platform == Platform.MacOS)
            {
                if (Settings.Instance.GridForDesktop)
                {
                    return ConnectToGridUsingRemoteDriver(arguments);
                }
                else
                {
                    switch (browserType)
                    {
                        case BrowserType.Chrome:
                            return GetChromeDriver(arguments);
                        case BrowserType.ChromiumEdge:
                            return GetEdgeDriver(arguments);
                        case BrowserType.Firefox:
                            return GetFirefoxDriver(arguments);
                        case BrowserType.InternetExplorer:
                            return GetIEDriver();
                        case BrowserType.Safari:
                            return GetSafariDriver(arguments);
                        default:
                            throw new PlatformNotSupportedException($"The browser {browserType} is not yet supported by WebEngine Framework.");
                    }
                }
            }
            else if (Settings.Instance.UseAppiumForWebMobile)
            {
                Settings.Instance.UseJavaScriptClick = true;
                return ConnectToGridUsingAppiumDriver(arguments);
            }
            else
            {
                Settings.Instance.UseJavaScriptClick = true;
                return ConnectToGridUsingRemoteDriver(arguments);
            }
        }

        private static WebDriver ConnectToGridUsingRemoteDriver(List<string> arguments)
        {
            Settings s = Settings.Instance;
            var options = GetDriverOption(s.Browser, arguments);
            options.PlatformName = s.Platform.ToString();
            options.AddAdditionalOption("newCommandTimeout", 90);
            options.AddAdditionalOption("nativeWebScreenshot", "true");

            string remoteServerAddress = s.GridServerUrl;
            if (remoteServerAddress == null)
            {
                remoteServerAddress = "http://localhost:4723/wd/hub";
            }

            DebugLogger.WriteLine($"Connecting to Grid: {remoteServerAddress}, Browser: {s.Browser}, Platform: {s.Platform}");
            AddPlatformSpecificOptions(remoteServerAddress, s);
            AddAdditionalCapabilities(options, s);
            if (options != null)
            {
                var capa = options.ToCapabilities();
                return new OpenQA.Selenium.Remote.RemoteWebDriver(new Uri(s.GridServerUrl), capa);
            }
            else
            {
                throw new NotSupportedException($"The framework does not support {s.Browser} yet on desktop grid.");
            }
        }

        private static DriverOptions GetDriverOption(BrowserType browser, List<string> arguments)
        {
            switch (browser)
            {
                case BrowserType.Safari:
                    var options = new SafariOptions();
                    return options;
                case BrowserType.Chrome:
                    var chromeOptions = new ChromeOptions();
                    chromeOptions.AddArguments(arguments);
                    return chromeOptions;
                case BrowserType.ChromiumEdge:
                    var edgeOptions = new EdgeOptions();
                    edgeOptions.AddArguments(arguments);
                    return edgeOptions;
                case BrowserType.Firefox:
                    var firefoxOptions = new FirefoxOptions();
                    firefoxOptions.AddArguments(arguments);
                    return firefoxOptions;
            }
            return null;
        }

        private static WebDriver GetSafariDriver(IEnumerable<string> browserOptions)
        {
            SafariOptions op = new SafariOptions()
            {
                AcceptInsecureCertificates = true,
            };
            SafariDriver driver = new SafariDriver(op);
            return driver;
        }

        private static WebDriver ConnectToGridUsingAppiumDriver(List<string> optionsFromSettings)
        {
            Settings s = Settings.Instance;
            AppiumOptions options = new AppiumOptions()
            {
                PlatformName = s.Platform.ToString(),
                AutomationName = s.Platform == Platform.Android ? "UiAutomator2" : "Safari",
                BrowserName = s.Browser.ToString(),
            };

            if (!string.IsNullOrEmpty(s.Device))
            {
                options.DeviceName = s.Device;

            }
            options.AddAdditionalAppiumOption("newCommandTimeout", 90);
            options.AddAdditionalAppiumOption("nativeWebScreenshot", "true");

            DebugLogger.WriteLine($"Connecting to device: {s.Device ?? "Not specified"}, Platform: {s.Platform}, OS Version: {s.OsVersion ?? "Not Specified"} ");

            string appiumServerAddress = s.GridServerUrl;
            if (appiumServerAddress == null)
            {
                appiumServerAddress = "http://localhost:4723/wd/hub";
            }

            AddPlatformSpecificOptions(appiumServerAddress, s);
            AddAdditionalCapabilities(options, s);

            if (s.Platform == Platform.Android)
            {
                return new AndroidDriver(new Uri(appiumServerAddress), options, new TimeSpan(0, 3, 0));
            }
            else if (s.Platform == Platform.iOS)
            {
                Settings.Instance.UseJavaScriptClick = true;
                return new IOSDriver(new Uri(appiumServerAddress), options, new TimeSpan(0, 3, 0));
            }
            else
            {
                DebugLogger.WriteError($"The platform {s.Platform} is not supported.");
                throw new NotSupportedException(s.Platform.ToString());
            }
        }

        /// <summary>
        /// Add additional capabilities specified in <see cref="Settings"/> (loaded from appsettings.json)
        /// </summary>
        /// <param name="options">the appium option object</param>
        /// <param name="s">Settings instance</param>
        private static void AddAdditionalCapabilities(AppiumOptions options, Settings s)
        {
            foreach (var cap in s.Capabilities)
            {
                if (cap.Value is JObject jo)
                {
                    var dict = jo.ToObject<Dictionary<string, object>>();
                    options.AddAdditionalAppiumOption(cap.Key, dict);
                }
                else
                {
                    options.AddAdditionalAppiumOption(cap.Key, cap.Value);
                }
            }
        }

        /// <summary>
        /// Add additional capabilities specified in <see cref="Settings"/> (loaded from appsettings.json)
        /// </summary>
        /// <param name="options">the appium option object</param>
        /// <param name="s">Settings instance</param>
        private static void AddAdditionalCapabilities(DriverOptions options, Settings s)
        {
            foreach (var cap in s.Capabilities)
            {
                if (cap.Value is JObject jo)
                {
                    var dict = jo.ToObject<Dictionary<string, object>>();
                    options.AddAdditionalOption(cap.Key, dict);
                }
                else
                {
                    options.AddAdditionalOption(cap.Key, cap.Value);
                }
            }
        }

        private static void AddPlatformSpecificOptions(string appiumServerAddress, Settings s)
        {
            if (appiumServerAddress.IsBrowserStack())
            {
                AddBrowserStackOptions(s);
            }
            //Add here the support for other platforms such as saucelabs and other customized providers.
        }

        /// <summary>
        /// Adds specifics options for browserstack platform. These options will be stored in `bstack:options` capability.
        /// </summary>
        /// <param name="s">Settings object (which may already contains bstack:options capability)</param>
        private static void AddBrowserStackOptions(Settings s)
        {
            Dictionary<string, object> browserstackOptions = new Dictionary<string, object>
            {
                { "userName", s.Username },
                { "accessKey", s.Password }
            };
            var os = GetBrowserStackOSName(s.Platform);
            if (!string.IsNullOrEmpty(os))
            {
                browserstackOptions.Add("os", os); //Windows = "Windows", MacOS = "OS X", Android, iOS
            }
            if (!string.IsNullOrEmpty(s.OsVersion))
            {
                browserstackOptions.Add("osVersion", s.OsVersion); //for MacOS: Monterey, Big Sur, ... for Windows: 10, For iOS or Android: 13.1, ...
            }

            browserstackOptions.Add("browserName", GetBrowserStackBrowserName(s.Browser)); //Safari, InternetExplorer
            browserstackOptions.Add("deviceName", s.Device);
            if (!string.IsNullOrEmpty(s.BrowserVersion))
            {
                browserstackOptions.Add("browserVersion", s.BrowserVersion); //13.0
            }
            //browserstackOptions.Add("seleniumVersion", "4.7.0");             //Same as installed selenium package;
            var assembly = GlobalConstants.LoadedAssemblies.FirstOrDefault();
            var name = assembly?.GetName();
            if (name != null)
            {
                browserstackOptions.Add("projectName", name.Name);
                browserstackOptions.Add("buildName", name.Version.ToString());
                browserstackOptions.Add("sessionName", name.FullName);
            }

            string bstackOptions = "bstack:options";

            if (s.Capabilities.ContainsKey(bstackOptions))
            {
                Console.WriteLine($"Merging {bstackOptions} capabilities with values from appsetting.json");
                var dic = s.Capabilities[bstackOptions];
                Console.WriteLine($"The type of {bstackOptions} from appsetting.json is {dic.GetType().FullName}");
                if (dic is JObject jo)
                {
                    var dictionary = jo.ToObject<Dictionary<string, object>>();
                    Console.WriteLine("bstack:options from appsetting.json: " + dictionary.Count);
                    Console.WriteLine("bstack:options auto-generated: " + browserstackOptions.Count);
                    foreach (var kv in dictionary)
                    {
                        browserstackOptions[kv.Key] = kv.Value;
                    }
                }
                else if (dic is Dictionary<string, object> dic2)
                {
                    Console.WriteLine("bstack:options from appsetting.json: " + dic2.Count);
                    Console.WriteLine("bstack:options auto-generated: " + browserstackOptions.Count);
                    foreach (var kv in dic2)
                    {
                        browserstackOptions[kv.Key] = kv.Value;
                    }
                }
                else
                {
                    Console.WriteLine("The type of bstack:options from appsetting.json is not JObject: " + dic.GetType().FullName);
                }
            }
            s.Capabilities[bstackOptions] = browserstackOptions;
            Console.WriteLine("bstack:options after merging: " + browserstackOptions.Count);
        }

        /// <summary>
        /// Gets the parameter 'OS' used for browerstack according to platform.
        /// </summary>
        /// <param name="platform">Platform name</param>
        /// <returns>browerstack </returns>
        private static string GetBrowserStackOSName(Platform platform)
        {
            switch (platform)
            {
                case Platform.Windows:
                    return "Windows";
                case Platform.MacOS:
                    return "OS X";
                case Platform.iOS:
                    return "ios";
                case Platform.Android:
                    return "android";
                default:
                    return null;
            }
        }

        private static object GetBrowserStackBrowserName(BrowserType browser)
        {
            switch (browser)
            {
                case BrowserType.InternetExplorer:
                    return "IE";
                case BrowserType.Chrome:
                    return "Chrome";
                case BrowserType.Firefox:
                    return "Firefox";
                case BrowserType.ChromiumEdge:
                    return "Edge";
                case BrowserType.Safari:
                    return "Safari";
                default:
                    throw new NotSupportedException($"{browser} is not a valid value on Desktop Web Browser on Browserstack platform");
            }
        }

        private static WebDriver GetIEDriver()
        {
            throw new Exception("Internet Explorer is not supported anymore. Please update the browser and run tests in one of supported browsers: Chrome, Edge, Firefox and Safari");
        }

        private static WebDriver GetEdgeDriver(IEnumerable<string> browserOptions)
        {
            object path = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\msedge.exe", "", null);
            var version = FileVersionInfo.GetVersionInfo(path.ToString()).FileVersion;
            DebugLogger.WriteLine($"Edge: {version} installed. Checking WebDriver for the browser.");
            var edgeDriver = GetEdgeDriver(version, browserOptions);
            return edgeDriver;
        }

        private static WebDriver GetEdgeDriver(string version, IEnumerable<string> browserOptions)
        {
            string downloadUrl = $"https://msedgedriver.azureedge.net/{version}/edgedriver_win64.zip";
            string file = $"{workingDirectory}\\Edge\\{version}\\Webdriver.zip";
            string folder = $"{workingDirectory}\\Edge\\{version}\\Extracted";
            DirectoryInfo di = new DirectoryInfo(folder);
            using (WebClient c = new WebClient())
            {
                if (!(di.Exists && di.GetFiles().Any()))
                {
                    DebugLogger.WriteLine($"Downloading driver and install into {folder}.");
                    System.IO.Directory.CreateDirectory(folder);
                    c.DownloadFile(downloadUrl, file);
                    System.IO.Compression.ZipFile.ExtractToDirectory(file, folder);
                }
                else
                {
                    DebugLogger.WriteLine($"Folder containing the current version of webdriver.");
                }
                OpenQA.Selenium.Edge.EdgeOptions options = new OpenQA.Selenium.Edge.EdgeOptions()
                {
                    AcceptInsecureCertificates = true,
                };
                options.AddArguments(browserOptions);

                OpenQA.Selenium.Edge.EdgeDriver cd = new OpenQA.Selenium.Edge.EdgeDriver(folder, options);
                return cd;
            }
        }

        private static WebDriver GetChromeDriver(IEnumerable<string> browserOptions)
        {
            object path = Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe", "", null);
            if (path == null)
            {
                path = Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe", "", null);
            }
            var version = FileVersionInfo.GetVersionInfo(path.ToString()).FileVersion;
            DebugLogger.WriteLine($"Detected installed Chrome Version: {version}");
            var chromeDriver = GetChromeDriver(version, browserOptions);
            return chromeDriver;
        }
        private static WebDriver GetChromeDriver(string version, IEnumerable<string> browserOptions)
        {
            string downloadUrl = null;
            int major = int.Parse(version.Substring(0, version.IndexOf(".")));
            string v = version.Substring(0, version.LastIndexOf("."));
            string getVersionUrl;
            if (major >= 115)
            {
                getVersionUrl = $"https://googlechromelabs.github.io/chrome-for-testing/LATEST_RELEASE_{v}";
            }
            else
            {
                getVersionUrl = $"https://chromedriver.storage.googleapis.com/LATEST_RELEASE_{v}";
            }

            using (WebClient c = new WebClient())
            {
                string driverVersion = c.DownloadString(getVersionUrl);
                if (major >= 115)
                {
                    downloadUrl = $"https://edgedl.me.gvt1.com/edgedl/chrome/chrome-for-testing/{driverVersion}/win64/chromedriver-win64.zip";
                }
                else
                {
                    downloadUrl = $"https://chromedriver.storage.googleapis.com/{driverVersion}/chromedriver_win32.zip";
                }
                string file = $"{workingDirectory}\\ChromeDriver\\{driverVersion}\\Webdriver.zip";
                string folder = $"{workingDirectory}\\ChromeDriver\\{driverVersion}\\Extracted";
                DirectoryInfo di = new DirectoryInfo(folder);
                //fix bug #48: Deadlock if webdriver download fails
                //Checks if the filder is not empty, or we try to download again the webdriver.
                //Same implementation for Edge Driver.
                if (!(di.Exists && di.GetFiles().Any())) 
                {
                    var di = System.IO.Directory.CreateDirectory(folder);
                    c.DownloadFile(downloadUrl, file);
                    System.IO.Compression.ZipFile.ExtractToDirectory(file, folder);
                    //new version of chromedriver are in a folder, copy them to root folder of Extracted.

                    if (di.GetDirectories().Any())
                    {
                        foreach (var subdir in di.GetDirectories())
                        {
                            foreach (var fil in subdir.GetFiles())
                            {
                                fil.CopyTo(Path.Combine(folder, fil.Name));
                            }
                        }
                    }
                }
                else
                {
                    DebugLogger.WriteLine($"The Chrome WebDriver already exsits");
                }
                OpenQA.Selenium.Chrome.ChromeOptions options = new OpenQA.Selenium.Chrome.ChromeOptions()
                {
                    AcceptInsecureCertificates = true,
                };
                options.AddArgument($"user-data-dir={GetEnvironmentVariable("LOCALAPPDATA")}\\Google\\Chrome\\User Data");
                options.AddArgument("dns-prefetch-disable");
                options.AddArgument("homepage=about:blank");
                options.AddArgument("disable-popup-blocking");
                options.AddArgument("no-default-browser-check");
                if (browserOptions != null) options.AddArguments(browserOptions);
                OpenQA.Selenium.Chrome.ChromeDriver cd = new OpenQA.Selenium.Chrome.ChromeDriver(folder, options);
                return cd;
            }
        }

        private static WebDriver GetFirefoxDriver(IEnumerable<string> browserOptions)
        {
            object path = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\firefox.exe", "", null);
            if (path == null)
            {
                path = Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\firefox.exe", "", null);
            }

            var directories = System.IO.Directory.GetDirectories($"{Environment.GetEnvironmentVariable("APPDATA")}\\Mozilla\\Firefox\\Profiles");
            var d = directories.First(x => x.EndsWith("default"));
            OpenQA.Selenium.Firefox.FirefoxProfile profile = new OpenQA.Selenium.Firefox.FirefoxProfile(d);
            OpenQA.Selenium.Firefox.FirefoxOptions firefoxOptions = new OpenQA.Selenium.Firefox.FirefoxOptions();
            firefoxOptions.AcceptInsecureCertificates = true;
            firefoxOptions.BrowserExecutableLocation = path?.ToString();
            firefoxOptions.Profile = profile;
            if (browserOptions != null) firefoxOptions.AddArguments(browserOptions);
            OpenQA.Selenium.Firefox.FirefoxDriver driver = new OpenQA.Selenium.Firefox.FirefoxDriver(firefoxOptions);
            return driver;
        }

        /// <summary>
        /// Syncrohnize the browser and wait until the document readystate is complete
        /// </summary>
        /// <param name="driver">WebDriver to use</param>
        public static void Sync(this WebDriver driver)
        {
            try
            {
                Thread.Sleep(2000);
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(Settings.Instance.SynchronzationTimeout));
                wait.Until(x =>
                {
                    var status = driver.ExecuteScript("return document.readyState");
                    return status.ToString() == "complete";
                });

            }
            catch (WebDriverTimeoutException) { }
        }

        /// <summary>
        /// Determin if the url belongs to BrowserStack
        /// </summary>
        /// <param name="url">the Uri to test</param>
        /// <returns>True or False</returns>
        internal static bool IsBrowserStack(this string url)
        {
            return url?.ToLower().Contains(".browserstack.com") ?? false;
        }

    }


}
