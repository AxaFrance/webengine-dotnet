# Test Configuration File

To simplify command line arguments, some parameters can be specified in a configuration file `appsettings.json`. 
If a parameter is provided in both side, the value provided in command-line will be taken account.

```json
{
  "LogDir": null,
  "GridConnection": "http://localhost:4723/wd/hub",
  "GridForDesktop": false,
  "Username": "",
  "Password": "",
  "PackageUploadTargetUrl": "https://yourcloudprovider.com/apploader/upload",
  "AllowInsecureCertificate": false,
  "PackageName": "my.company.myapplication",
  "EncryptionKey": null,
  "BrowserVersion": null,
  "Capabilities": {
    "additionalCapability1": "value1",
    "additionalCapability2": "value2"
  },
  "chromeOptions": [ "incognito" ],
  "firefoxOptions": [],
  "edgeOptions": [ "inprivate", "start-maximized" ],
  "safariOptions": []
}
```

## General Options
* `LogDir`: The directory to store the test report. if the value is null, then the default temporary folder will be used.
* `GridConnection`: The Selenium Grid/Appium Server Url to run Mobile based tests. 
* `GridForDesktop`: Normal Web Desktop execution will run locally and ignore selenium grid connection. If you want to run Desktop Web tests on a Selenium Grid, set this value to `true`. Mobile test will not be affected by this value.
* `Username`: The username of Selenium Grid authentication.
* `Password`: The password of Selenium Grid authentication.
* `PackageUploadTargetUrl`: The url handler to upload the app package (.apk or .ipa)
* `PackageName`: The name of the app package (often in *com.company.product* format)
* `EncryptionKey`: If you want to secure password used in your script, you can encrypt it via the customized Encryption Key.
If the value is `null`, WebEngine will use default encryption key.
* `Capabilities`: To provided necessary Appium capabilities or options for special need.

## Options for browsers
* `chromeOptions`, `firefoxOptions`, `edgeOptions` and `safariOptions`: These are default options for each browser. when `BrowserFactory.GetDriver` is called, driver options of selected browser will be used. For example, if running tests on Chrome with above settings, Chrome will be launched in `incongnito` mode, if running tests on Edge, the browser will be launched in `imprivate` mode with window maximized by default. 


> [!NOTE]
> Modifying capabilities may affect the behavior for Web Mobile and App Mobile testing, use it only when it's necessary.
