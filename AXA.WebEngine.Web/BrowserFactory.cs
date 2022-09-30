using Microsoft.Win32;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.iOS;
using OpenQA.Selenium.Safari;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using static System.Environment;

[assembly: InternalsVisibleTo("WebRunner")]
[assembly: InternalsVisibleTo("WebEngine.Test")]

namespace AXA.WebEngine.Web
{
    /// <summary>
    /// BrowserHelper is a tooling class helps user to get the Selenium WebDriver object from the given context : platform, browserType.
    /// </summary>
    public static class BrowserFactory
    {
        static string workingDirectory = System.Environment.GetFolderPath(SpecialFolder.ApplicationData) + "\\AXA.WebEngine";

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
        public static WebDriver GetDriver(Platform platform, AXA.WebEngine.BrowserType browserType)
        {
            Settings.Instance.Platform = platform;
            Settings.Instance.Browser = browserType;
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11;
            
            //Apple Safari driver does not support Click(), we need to active UseJavaScriptClick if target browser is Safari and do JS Click instead.
            if(browserType == BrowserType.Safari)
            {
                Settings.Instance.UseJavaScriptClick = true;
            }

            if (platform == Platform.Windows || platform == Platform.MacOS)
            {
                switch (browserType)
                {
                    case BrowserType.Chrome:
                        return GetChromeDriver();
                    case BrowserType.ChromiumEdge:
                        return GetEdgeDriver();
                    case BrowserType.Firefox:
                        return GetFirefoxDriver();
                    case BrowserType.InternetExplorer:
                        return GetIEDriver();
                    case BrowserType.Safari:
                        return GetSafariDriver();
                    default:
                        throw new PlatformNotSupportedException($"The browser {browserType} is not yet supported by WebEngine Framework.");
                }
                
            }
            else
            {
                return ConnectToDevice();
            }
        }

        private static WebDriver GetSafariDriver()
        {
            SafariOptions op = new SafariOptions()
            {
                AcceptInsecureCertificates = true,
            };
            SafariDriver driver = new SafariDriver(op);
            return driver;
        }

        private static WebDriver ConnectToDevice()
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

            DebugLogger.WriteLine($"Connecting to device: {s.Device ?? "Not specified"}, os: {s.OsVersion ?? "Not Specified"} ");

            string appiumServerAddress = s.GridServerUrl;
            if (appiumServerAddress == null)
            {
                appiumServerAddress = "http://localhost:4723/wd/hub";
            }

            AddAdditionalCapabilities(options, s);
            AddPlatformSpecificOptions(appiumServerAddress, options, s);

            if (s.Platform == Platform.Android)
            {
                return new AndroidDriver(new Uri(appiumServerAddress), options, new TimeSpan(0, 3, 0));

            }
            else if (s.Platform == Platform.iOS)
            {
                return new IOSDriver(new Uri(appiumServerAddress), options, new TimeSpan(0, 3, 0));
            }
            else
            {
                DebugLogger.WriteError($"The platform {s.Platform} is not supported.");
                throw new NotSupportedException(s.Platform.ToString());
            }
        }

        /// <summary>
        /// Add additional capabilities specified in appsettings.json
        /// </summary>
        /// <param name="options">the appium option object</param>
        /// <param name="s">Settings instance</param>
        private static void AddAdditionalCapabilities(AppiumOptions options, Settings s)
        {
            foreach(var cap in s.Capabilities)
            {
                options.AddAdditionalAppiumOption(cap.Key, cap.Value);
            }
        }

        private static void AddPlatformSpecificOptions(string appiumServerAddress, AppiumOptions options, Settings s)
        {
            if (appiumServerAddress.ToLower().Contains("browserstack.com"))
            {
                AddBrowserStackOptions(options, s);
            }
            //Add here the support for other platforms
        }

        private static void AddBrowserStackOptions(AppiumOptions options, Settings s)
        {
            Dictionary<string, object> browserstackOptions = new Dictionary<string, object>();
            browserstackOptions.Add("userName", s.Username);
            browserstackOptions.Add("accessKey", s.Password);
            options.AddAdditionalAppiumOption("bstack:options", browserstackOptions);

            var assembly = GlobalConstants.LoadedAssemblies.FirstOrDefault();
            var name = assembly?.GetName();
            if (name != null) {
                options.AddAdditionalAppiumOption("project", name.Name);
                options.AddAdditionalAppiumOption("build", name.Version.ToString()) ;
                options.AddAdditionalAppiumOption("name", name.FullName);
            }
        }

        private static WebDriver GetIEDriver()
        {
            OpenQA.Selenium.IE.InternetExplorerDriver ieDriver = new OpenQA.Selenium.IE.InternetExplorerDriver(CurrentDirectory);
            return ieDriver;
        }

        private static WebDriver GetEdgeDriver()
        {
            object path = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\msedge.exe", "", null);
            var version = FileVersionInfo.GetVersionInfo(path.ToString()).FileVersion;
            DebugLogger.WriteLine($"Edge: {version} installed. Checking WebDriver for the browser.");
            var edgeDriver = GetEdgeDriver(version);
            return edgeDriver;
        }

        private static WebDriver GetEdgeDriver(string version)
        {
            string downloadUrl = $"https://msedgedriver.azureedge.net/{version}/edgedriver_win64.zip";
            string file = $"{workingDirectory}\\Edge\\{version}\\Webdriver.zip";
            string folder = $"{workingDirectory}\\Edge\\{version}\\Extracted";

            using (WebClient c = new WebClient())
            {
                if (!System.IO.Directory.Exists(folder))
                {
                    System.IO.Directory.CreateDirectory(folder);
                    c.DownloadFile(downloadUrl, file);
                    System.IO.Compression.ZipFile.ExtractToDirectory(file, folder);
                }
                else
                {
                    DebugLogger.WriteLine($"Folder containing the current version of webdriver exists already.");
                }
                OpenQA.Selenium.Edge.EdgeOptions options = new OpenQA.Selenium.Edge.EdgeOptions()
                {
                    AcceptInsecureCertificates = true,
                    
                };

                OpenQA.Selenium.Edge.EdgeDriver cd = new OpenQA.Selenium.Edge.EdgeDriver(folder, options);
                return cd;
            }
        }

        private static WebDriver GetChromeDriver()
        {
            object path = Registry.GetValue(@"HKEY_LOCAL_MACHINE\Software\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe", "", null);
            if (path == null)
            {
                path = Registry.GetValue(@"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\chrome.exe", "", null);
            }
            var version = FileVersionInfo.GetVersionInfo(path.ToString()).FileVersion;
            DebugLogger.WriteLine($"Chrome {version} installed");
            var chromeDriver = GetChromeDriver(version);
            return chromeDriver;
        }
        private static WebDriver GetChromeDriver(string version)
        {
            string downloadUrl = null;
            string v = version.Substring(0, version.LastIndexOf("."));
            string latestVersion = $"https://chromedriver.storage.googleapis.com/LATEST_RELEASE_{v}";

            using (WebClient c = new WebClient())
            {
                string lv = c.DownloadString(latestVersion);
                downloadUrl = $"https://chromedriver.storage.googleapis.com/{lv}/chromedriver_win32.zip";
                string file = $"{workingDirectory}\\ChromeDriver\\{lv}\\Webdriver.zip";
                string folder = $"{workingDirectory}\\ChromeDriver\\{lv}\\Extracted";
                if (!System.IO.Directory.Exists(folder))
                {
                    System.IO.Directory.CreateDirectory(folder);
                    c.DownloadFile(downloadUrl, file);
                    System.IO.Compression.ZipFile.ExtractToDirectory(file, folder);
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
                OpenQA.Selenium.Chrome.ChromeDriver cd = new OpenQA.Selenium.Chrome.ChromeDriver(folder, options);
                return cd;
            }
        }

        private static WebDriver GetFirefoxDriver()
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
            catch(WebDriverTimeoutException) { }
        }

    }


}
