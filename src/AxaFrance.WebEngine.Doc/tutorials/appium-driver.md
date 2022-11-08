# WebDriver for MobileApp testing
In this article, we will discuss how to test Mobile applications running on Android and iOS.
It is easier to test Web based applications, because we can just open the browser and type the URL, then we are able to work on it.
Mobile App testing is different, before launching the test script, we need to install the App package and open it in the first place.

To pass the App package, in WebEngine Framework, you can set it in <xref:AxaFrance.WebEngine.Settings.AppId>.
There are two different possibilities:
1. AppId format that supported by your cloud provider. 
2. Or to test locally, the APK pathdans les : `c:\mypackage\appv1.0.apk`

If you are connecting to a local Appium Server, the package file will be passed-through, so Appium server will install the package on the device and open it.
If you are connecting to a remote Selenium Grid, make sure the AppId is understandable by remote server.
All other parameters are exactly the same as [WebDriver for Web based tests](web-driver.md).

> [!NOTE]
> When you are using Keyword driven test approach,
> the class <xref:AxaFrance.WebEngine.MobileApp.TestSuiteApp> handles automatically app package upload to supported cloud providers and converts AppId from local path to cloud provider understandable value.

Instead of using <xref:AxaFrance.WebEngine.Web.BrowserFactory> for web based tests, we will use <xref:AxaFrance.WebEngine.MobileApp.AppFactory> for mobile app tests.

## Native automation driver
Appium uses native automation drivers to interact with the applications.
By default, WebEngine Framework uses following automation driver:
* XCUITest on iOS devices
* UIAutomation2 on Android devices

More automation driver could be considered in the future.

## Connecting to local Appium Server
To connect to a local Appium Server which connects to a single device,
we only need to specify the Server URL and the full path of the app package: 

```csharp
var settings = AxaFrance.WebEngine.Settings.Instance;
settings.GridServerUrl = "http://localhost:4723/wd/hub";
settings.AppId = "C:\\app-packages\\myapp-1.0.apk";
var driver = AppFactory.GetDriver(AxaFrance.WebEngine.Platform.Android);
```
The above code snippet will install `myapp-1.0.apk` on the device and returns `AndroidDriver` so you can write test scripts based on it.

## Connecting to On-Premise Selenium Grid
To test on on-premise selenium grid, you'll need to specify the `Settings.Device` and/or `Settings.OsVersion`

```csharp
var settings = AxaFrance.WebEngine.Settings.Instance;
settings.GridServerUrl = "http://remote-server.internal:8080/wd/hub";
settings.AppId = "C:\\app-packages\\myapp-1.0.apk";
settings.Platform = Platform.Android;
settings.Device = "Android API 30";
//settings.OsVersion = "9.0";
var driver = AppFactory.GetDriver(AxaFrance.WebEngine.Platform.Android);
```

When using on-premises selenium grid, you need to copy or upload the app-package to an accessible location for your appium nodes.
And transform `AppId` to the understandable form.

For example, using UNC paths: `settings.AppId = "\\\\SeleniumShare\\package\\myapp-1.0.apk"`;


## Connecting to BrowserStack
Browserstack is one of the supported cloud providers by the Framework.
You can use <xref:AxaFrance.WebEngine.MobileApp.AppFactory.UploadAppPackage(System.String,System.String,System.String,System.String)> to upload local app package to the remote server and gets AppId in `bs://<id>` format.
If you are working with Keyword driven test approach, the upload and conversation of AppId is automatic.
