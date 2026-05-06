# Test Configuration File

To simplify command line arguments, some parameters can be specified in a configuration file `appsettings.json`. 
If a parameter is provided in both side, the value provided in command-line will be taken account.

```json
{
  "LogDir": null,
  "GridConnection": "http://localhost:4723/wd/hub",
  "GridForDesktop": false,
  "UseAppiumForWebMobile": false,
  "Username": "",
  "Password": "",
  "PackageUploadTargetUrl": "https://yourcloudprovider.com/apploader/upload",
  "AllowInsecureCertificate": false,
  "PackageName": "my.company.myapplication",
  "EncryptionKey": null,
  "OsVersion": null,
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
* `LogDir`: The directory to store the test report. If the value is null, the default temporary folder will be used.
* `GridConnection`: The Selenium Grid or Appium Server URL used for mobile or remote desktop tests.
* `GridForDesktop`: Normal Web Desktop tests run locally and ignore Selenium Grid. Set to `true` to route desktop browser tests through the Selenium Grid as well. Mobile tests are not affected by this value.
* `UseAppiumForWebMobile`: When set to `true`, forces the framework to use Appium for mobile web browser tests (e.g., Chrome on Android, Safari on iOS) instead of a pure Selenium Grid connection. Useful when your grid provider requires Appium for mobile web.
* `Username`: The username for Selenium Grid authentication.
* `Password`: The password for Selenium Grid authentication.
* `PackageUploadTargetUrl`: The URL endpoint used to upload the app package (`.apk` or `.ipa`).
* `PackageName`: The package name of the application under test (typically in *com.company.product* format).
* `EncryptionKey`: A custom key used to encrypt and decrypt sensitive values (such as passwords) in test data files. If `null`, WebEngine uses a default built-in key. For security, avoid storing a real key in a shared configuration file — use token replacement on your DevOps platform instead.
* `AllowInsecureCertificate`: When `true`, the browser will accept self-signed or untrusted SSL certificates. Useful for testing environments that do not have a trusted certificate.
* `BrowserVersion`: Pins the browser version when connecting to a Selenium Grid that requires it.
* `Capabilities`: Additional capabilities sent to Appium or Selenium Grid for special configuration needs.

## Options for browsers
* `chromeOptions`, `firefoxOptions`, `edgeOptions` and `safariOptions`: Default launch options for each browser. When `BrowserFactory.GetDriver` is called, the options for the selected browser are applied automatically. For example, with the configuration above, Chrome will launch in `incognito` mode and Edge will launch in `inprivate` mode with the window maximized.

> [!NOTE]
> Modifying capabilities may affect the behavior for Web Mobile and App Mobile testing. Use only when necessary.
