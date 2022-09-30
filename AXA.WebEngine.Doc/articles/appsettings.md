# Test Configuration File

To simplify command line arguments, some parameters can be specified in a configuration file `appsettings.json`. 
If a parameter is provided both in `appsettings.json` file and command-line argument, the value provided in command-line will be taken account.

```json
{
  "LogDir": null,
  "GridConnection": "http://localhost:4723/wd/hub",
  "Username": "",
  "Password": "",
  "PackageUploadTargetUrl": "https://api-cloud.browserstack.com/app-automate/upload",
  "PackageName": "my.company.myapplication",
  "EncryptionKey": null,
  "Capabilities": {
    "additionalCapability1": "value1",
    "additionalCapability2": "value2"
  }
}
```

* `LogDir`: The directory to store test report. This parameter is used by reports generated via WebRunner. if the value is null, then the default temporary folder will be used.
* `GridConnection`: The Selenium Grid/Appium Server Url to run Mobile based tests. 
* `Username`: The username of Selenium Grid authentication.
* `Password`: The password of Selenium Grid authentication.
* `PackageUploadTargetUrl`: The url handler to upload the app package (.apk or .ipa)
* `PackageName`: The name of the app package (often in *com.company.product* format)
* `EncryptionKey`: If you want to secure password used in your script, you can encrypt it via the customized Encryption Key.
If the value is `null`, WebEngine will use default encryption key.
* `Capabilities`: To provided necessary Appium capabilities or options for special need.

> [!NOTE]
> Modifying capabilities may affect the behavior for Web Mobile and App Mobile testing, use it only when it's necessary.
