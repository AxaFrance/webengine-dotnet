// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using Newtonsoft.Json.Linq;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.iOS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("WebRunner")]
[assembly: InternalsVisibleTo("WebEngine.Test")]

namespace AxaFrance.WebEngine.MobileApp
{
    /// <summary>
    /// Helper class to manage app package uploading to cloud provider and device connection.
    /// </summary>
    public static class AppFactory
    {

        /// <summary>
        /// Upload an App package (APK or IPA) to the browserstack server
        /// </summary>
        /// <param name="username">User Name (can be retrived via app-automate.browserstack.com)</param>
        /// <param name="accessKey">Access Key (can be retrived via app-automate.browserstack.com)</param>
        /// <param name="packagePath">The Full path of package file (.apk or .ipa)</param>
        /// <param name="targetUrl">The target server to upload, default value: https://api-cloud.browserstack.com/app-automate/upload </param>
        /// <returns>app_url, used to identify the application on the target modile device</returns>
        internal static async Task<string> UploadToBrowserstack(string packagePath, string username, string accessKey, string targetUrl = "https://api-cloud.browserstack.com/app-automate/upload")
        {
            ServicePointManager.ServerCertificateValidationCallback += checkCertificate;
            DebugLogger.WriteLine($"Uploading the package {packagePath} to {targetUrl}");
            string boundary = $"--bswebengine-{DateTime.Now.Ticks.ToString("x")}";
            FileInfo fi = new FileInfo(packagePath);
            var fileContent = File.ReadAllBytes(packagePath);
            MemoryStream ms = new MemoryStream(fileContent);
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{accessKey}")));
                using (StreamContent sc = new StreamContent(ms))
                {
                    using (MultipartFormDataContent formData = new MultipartFormDataContent(boundary))
                    {
                        formData.Add(sc, "file", fi.Name);
                        var message = await httpClient.PostAsync(targetUrl, formData);
                        var code = message.StatusCode;
                        string content = await message.Content.ReadAsStringAsync();
                        printMessage(code, content);
                        var responseContent = await message.Content.ReadAsStringAsync();
                        var obj = JObject.Parse(responseContent);
                        string appId = obj.GetValue("app_url").ToString();
                        return appId;
                    }
                }
            }
        }

        /// <summary>
        /// Upload an App package (apk or ipa) to Mobile Lab server
        /// </summary>
        /// <param name="accessToken">The access key distributed by Mobile lab administrator</param>
        /// <param name="packagePath">The path of local package to upload</param>
        /// <param name="targetUrl">The endpoint of Upload Package service</param>
        /// <returns>AppId returned by Mobile Lab</returns>
        internal static async Task<string> UploadToMobileLab(string accessToken, string packagePath, string targetUrl)
        {
            ServicePointManager.ServerCertificateValidationCallback += checkCertificate;
            DebugLogger.WriteLine($"Uploading the package {packagePath} to {targetUrl}");
            string boundary = $"--mlwebengine-{DateTime.Now.Ticks.ToString("x")}";
            FileInfo fi = new FileInfo(packagePath);
            var fileContent = File.ReadAllBytes(packagePath);
            MemoryStream ms = new MemoryStream(fileContent);
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                using (StreamContent sc = new StreamContent(ms))
                {
                    using (MultipartFormDataContent formData = new MultipartFormDataContent(boundary))
                    {
                        formData.Add(sc, "file", fi.Name);
                        var message = await httpClient.PostAsync(targetUrl, formData);
                        var code = message.StatusCode;
                        string content = await message.Content.ReadAsStringAsync();
                        printMessage(code, content);
                        var responseContent = await message.Content.ReadAsStringAsync();
                        var obj = JObject.Parse(responseContent);
                        string appId = obj.GetValue("app-package").ToString();
                        string user = obj.GetValue("user").ToString();
                        return appId;
                    }
                }
            }

        }

        private static bool checkCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (Settings.Instance.AllowAnyCertificate)
            {
                return true;
            }
            else
            {
                if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors)
                {
                    foreach (var cs in chain.ChainStatus)
                    {
                        DebugLogger.WriteLine(cs.StatusInformation); //validation errors here
                    }
                }
                return sslPolicyErrors == SslPolicyErrors.None;
            }
        }

        private static void printMessage(HttpStatusCode code, string content)
        {
            DebugLogger.WriteLine($"HTTP Status From server: {code}");
            DebugLogger.WriteLine(content);
        }

        /// <summary>
        /// Gets the WebDriver to Mobile App Testing using configuration of <see cref="AxaFrance.WebEngine.Settings.Instance"/>
        /// </summary>
        /// <param name="platform">The Test platform: Android or iOS</param>
        /// <returns>Once the connection to device is established, a AppiumDriver (AndroidDriver or IOSDriver) instance will return.</returns>
        public static AppiumDriver GetDriver(Platform platform)
        {
            Settings.Instance.Platform = platform;
            switch (platform)
            {
                case Platform.iOS:
                    Settings.Instance.Browser = BrowserType.IOSNative;
                    break;
                case Platform.Android:
                    Settings.Instance.Browser = BrowserType.AndroidNative;
                    break;
                default: throw new ArgumentOutOfRangeException($"{platform} is not a valid value for Mobile App testing");
            }
            return ConnectToDevice();
        }

        /// <summary>
        /// Press the back button of the device.
        /// </summary>
        /// <param name="driver">the appiumdriver to be used.</param>
        public static void ActionBack(AppiumDriver driver)
        {
            if (driver is AndroidDriver ad)
            {
                ad.PressKeyCode(AndroidKeyCode.Back);
            }
            else if (driver is IOSDriver id)
            {
                driver.Navigate().Back();
            }
        }

        /// <summary>
        /// The NativeAppContext const = "NATIVE_APP".
        /// </summary>
        const string NativeAppContext = "NATIVE_APP";

        /// <summary>
        /// Switch current context within a hybird application (native Application with web views)
        /// </summary>
        /// <param name="appiumDriver"></param>
        /// <param name="targetContext"></param>
        /// <exception cref="WebEngineGeneralException"></exception>
        public static void SwitchContext(AppiumDriver appiumDriver, string targetContext)
        {
            var timeout = DateTime.Now.AddSeconds(Settings.Instance.SynchronzationTimeout);
            if (appiumDriver != null)
            {
                if (targetContext == NativeAppContext)
                {
                    appiumDriver.Context = targetContext;
                }
                else
                {
                    while (timeout > DateTime.Now)
                    {
                        var context = appiumDriver.Contexts.FirstOrDefault(x => x.StartsWith(targetContext));
                        if (context != null)
                        {
                            appiumDriver.Context = context;
                            return;
                        }
                        else
                        {
                            continue;
                        }
                    }
                    throw new WebEngineGeneralException("Unabled to switch to context: " + targetContext);
                }
            }
            else
            {
                throw new WebEngineGeneralException("Appium WebDriver is not initialized or disposed.");
            }
        }

        /// <summary>
        /// Connect to a specific device.
        /// </summary>
        /// <returns>returns the AppiumDriver, either of type AndroidDriver or of type IOSDriver according to test settings</returns>
        /// <remarks>
        /// The parameter `automation` is only used to determine the name of the test assembly and it's version. You can pass any classes and instance defined in your test automation project.
        /// </remarks>
        private static AppiumDriver ConnectToDevice()
        {
            AppiumDriver driver;
            Settings s = Settings.Instance;
            AppiumOptions options = new AppiumOptions()
            {
                DeviceName = s.Device,
                PlatformName = s.Platform.ToString(),
                App = s.AppId,
            };

            options.AddAdditionalAppiumOption("newCommandTimeout", 90);
            options.AddAdditionalAppiumOption("nativeWebScreenshot", true);

            if (!string.IsNullOrEmpty(s.OsVersion))
            {
                options.AddAdditionalAppiumOption("os_version", s.OsVersion);
            }



            DebugLogger.WriteLine($"Connecting to device: {s.Device}, os: {s.OsVersion} ");

            string appiumServerAddress = s.GridServerUrl;
            if (appiumServerAddress == null)
            {
                appiumServerAddress = "http://localhost:4723";
            }

            AddPlatformSpecificOptions(appiumServerAddress, options, s);

            //convert relative path to absolute path
            if (!s.AppId.Contains(":"))
            {
                s.AppId = Path.Combine(Directory.GetCurrentDirectory(), s.AppId);
                options.App = s.AppId;
            }

            if (s.Platform == Platform.Android)
            {
                options.AutomationName = "UiAutomator2";
                driver = new AndroidDriver(new Uri(appiumServerAddress), options);
            }
            else if (s.Platform == Platform.iOS)
            {
                options.AutomationName = "XCUITest";
                driver = new IOSDriver(new Uri(appiumServerAddress), options);
            }
            else
            {
                DebugLogger.WriteError($"The platform {s.Platform} is not supported for mobile testing.");
                throw new NotSupportedException(s.Platform.ToString());
            }
            return driver;
        }

        private static void AddBrowserStackOptions(AppiumOptions options, Settings s)
        {
            Dictionary<string, object> browserstackOptions = new Dictionary<string, object>();
            browserstackOptions.Add("userName", s.Username);
            browserstackOptions.Add("accessKey", s.Password);
            var assembly = GlobalConstants.LoadedAssemblies.FirstOrDefault();
            var name = assembly?.GetName();
            if (name != null)
            {
                browserstackOptions.Add("projectName", name.Name);
                browserstackOptions.Add("buildName", name.Version.ToString());
                browserstackOptions.Add("sessionName", name.FullName);
            }
            else
            {
                browserstackOptions.Add("projectName", "unknownName");
                browserstackOptions.Add("buildName", "unkonwnBuild");
                browserstackOptions.Add("sessionName", "unkonwnSession");
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
            options.AddAdditionalAppiumOption(bstackOptions, browserstackOptions);
        }

        private static void AddPlatformSpecificOptions(string appiumServerAddress, AppiumOptions options, Settings s)
        {
            if (appiumServerAddress.Contains("browserstack.com"))
            {
                AddBrowserStackOptions(options, s);
            }

        }
    }
}
