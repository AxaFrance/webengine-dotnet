# WebDriver for Web based tests
With WebEngine there is a unique way to initialize Web Driver for any kind of platforms and any kind of browsers:
<xref:AxaFrance.WebEngine.Web.BrowserFactory.GetDriver(AxaFrance.WebEngine.Platform,AxaFrance.WebEngine.BrowserType)>

## Test on desktop browsers
WebEngine supports the following desktop browsers:
* `Microsoft Edge`: WebEngine will automatically download and use the Microsoft Edge driver from the official repository according to the Edge version installed on your computer.
* `Google Chrome`: WebEngine will automatically download and use the Chrome driver from the official repository according to the Chrome version installed on your computer.
* `Mozilla Firefox`: WebEngine uses geckodriver to automate Firefox.
* `Safari`: Supported on macOS. Safari's WebDriver (SafariDriver) is built into macOS and requires no additional download. Enable it once with `safaridriver --enable` in Terminal.

```csharp
var webdriver = BrowserFactory.GetDriver(AxaFrance.WebEngine.Platform.Windows, AxaFrance.WebEngine.BrowserType.ChromiumEdge);
webdriver.Navigate().GoToUrl("https://webengine-test.azurewebsites.net/");
```
The returned `webdriver` object is type of `OpenQA.Selenium.WebDriver`

> [!IMPORTANT]
> WebEngine supports the Chromium-based Microsoft Edge only. The legacy EdgeHTML-based Edge is no longer supported.

## Test on Mobile browsers
WebEngine support following desktop browsers:
* `Chrome`: for Android devices via Appium or Selenium Grid compatible platforms.
* `Safari`: for iOS based devices via Appium or Selenium Grid compatible platforms.

```csharp
var webdriver = BrowserFactory.GetDriver(AxaFrance.WebEngine.Platform.Android, AxaFrance.WebEngine.BrowserType.Chrome);
webdriver.Navigate().GoToUrl("https://webengine-test.azurewebsites.net/");
```

The returned `webdriver` object is type of `OpenQA.Selenium.Appium.AppiumDriver`. More specifically, AndroidDriver or IOSDriver according to the device connected.
It gives you the possibility to pass mobile specific commands.

To test on Mobile browsers you'll need to specify the Appium Server or Selenium Grid of which the device is connected.

For following code snippet is used to connect to a `Android` Device, named `Nexus 5` running `Android 10.1` and `Chrome` browser.

Parameters `Username` and `Password` will be used for Grid Authentication. `Device`, `OsVersion` will be used for device selection according to devices available in your device cloud.
You can use these parameters to connect to some Device cloud providers based on Selenium Grid.

```csharp
var settings = AxaFrance.WebEngine.Settings.Instance;
settings.GridServerUrl = "http://<grid-server>:<port>/wd/hub";
settings.Username = "user-of-grid";
settings.Password = "user-access-key";
settings.Device = "Nexus 5";
settings.OsVersion = "10.1";
driver = BrowserFactory.GetDriver(AxaFrance.WebEngine.Platform.Android, AxaFrance.WebEngine.BrowserType.Chrome);
driver.Navigate().GoToUrl("https://webengine-test.azurewebsites.net/");
```

> [!NOTE]
> Selenium Grid connection, username and password can be configured in [configuration files](../articles/appsettings.md).

## Firewall consideration
During test execution, the target machine must have access to the following internet addresses to download the web driver matching the installed browser version.
* For Edge: https://msedgedriver.azureedge.net
* For Chrome: https://googlechromelabs.github.io/chrome-for-testing/

If test execution is performed in a controlled environment, please configure either a proxy or firewall rules to ensure the above URLs are accessible.
WebEngine Framework respects system proxy settings.
