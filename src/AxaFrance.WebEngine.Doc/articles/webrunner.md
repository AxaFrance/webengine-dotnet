# Launch Test with Web Runner
`WebRunner` is the component to run automated tests and provides rich XML reports.
Based on the programming language used to build a test automation solution, the command-line has a little difference.
However, all behaviors and arguments remain the same.

> [!WARNING]
> Spacing characters are not accepted as parameter values. Parameters with spaces must be enclosed in double-quotes.

# [.NET](#tab/netcore)
```batch
webrunner "-a:<testproject>" [-data:<testdata>] [-env:<env_ariable>] [-browser:<browser-type>] [optional_arguments]
```
# [Java](#tab/java)
```batch
java -jar MyProject.jar [-data:<testdata>] [-env:<env_ariable>] [-browser:<browser-type>] [optional_arguments]
```
***

## Prerequisites
To run tests built with WebEngine Framework, you have to provide a compiled test automation solution and specify the browser type.

### Required Parameters
* `-a:<testproject>`: *(.NET only)* The filename or full path of the compiled test project DLL library.
* `-browser:<browser-type>`: The browser or platform to run the tests on. See [Browser Types](#-browserbrowser) for supported values.

### Data-Driven Testing
If your solution uses a Data-Driven approach, you can provide the following files:
* `-data:<testdata>`: Test Data to be used for data-driven test execution, in XML format. The file can be exported via the `Excel Add-in`. If not provided, tests will run without test data.
* `-env:<env_variable>`: Environment Variables in XML format, to store test environment-related test data, such as URLs, Hostnames, or Credentials.


### Examples
#### Run test on desktop browsers
The following command line launches the solution in the `Firefox` browser and generates test reports in `C:\Temp`

# [.NET](#tab/netcore)
```batch
WebRunner.exe "-a:MyProject.dll" "-data:Data.xml" "-env:Staging.xml" "-browser:Firefox" "-outputDir:C:\Temp"
```

# [Java](#tab/java)
```batch
java -jar MyProject.jar "-data:Data.xml" "-env:Staging.xml" "-browser:Firefox" "-outputDir:C:\Temp"
```
***


#### Run webapp test on Android Emulator
The following command line launches the same test under an emulated `Android` device (requires Android Emulator + Appium Server on local) using the `Chrome` browser.

# [.NET](#tab/netcore)
```batch
WebRunner.exe "-a:MyProject.dll" "-data:Data.xml" "-env:Env.xml" "-platform:Android" "-browser:Chrome" "-device:Emulator" "-outputDir:C:\Temp"
```

# [Java](#tab/java)
```batch
java -jar MyProject.jar "-data:Data.xml" "-env:Env.xml" "-platform:Android" "-browser:Chrome" "-device:Emulator" "-outputDir:C:\Temp"
```
***

#### Run app test on Selenium compatible Device Cloud
The following command line installs the application package, then launches the test under `iPhone 12`, using the provided device cloud.
```batch
WebRunner.exe "-a:AppProject.dll" "-data:Data.xml" "-env:Staging.xml" "-platform:iOS" "-browser:IOSNative" "-device:iPhone 12" "-grid:http://seleniumgrid.com/wd/hub" "-appId:C:\app_under_test.ipa"
```

#### Encrypt sensitive data
To encrypt sensitive data (such as passwords) for use in test data files:
```batch
WebRunner.exe "-encrypt" "MySecretPassword" "-encryptionKey:MyEncryptionKey"
```


## Optional parameters

> [!NOTE]
> Some parameters can be provided in the configuration file `appsettings.json` for C# and `application-properties.yml` for JAVA.

> Please refer to [Test Configuration C#](appsettings.md) for C#

> Please refer to [Test Configuration JAVA](appsettings-java.md) for Java

### Common parameters

#### -browser:\<browser>
**Required.** Specifies the browser or platform on which to run the test. Supported values:
- **Desktop Browsers:**
  - `Chrome` - Google Chrome
  - `Firefox` - Mozilla Firefox
  - `Edge` - Microsoft Edge (Chromium-based)
- **Mobile Browsers:**
  - `Safari` - iOS Safari browser
  - `Chrome` - Google Chrome
- **Mobile Native Applications:**
  - `IOSNative` - iOS native application
  - `AndroidNative` - Android native application

> [!NOTE]
> Internet Explorer is no longer supported. Please use Edge or other modern browsers.

See: <xref:AxaFrance.WebEngine.BrowserType> for more details.

#### -platform:\<platform>
Specifies the platform for test execution. Supported values:
- `Windows` (default)
- `Android`
- `iOS`

See: <xref:AxaFrance.WebEngine.Platform> for more details.

#### -outputDir:\<outputFolder>
Specifies the folder to store the output of test execution and test reports. This parameter can also be defined in `appsettings.json` for C# and `application.yml` for Java.

#### -u
Saves reports to a unique folder suffixed by date and time (format: `yyyyMMdd_hhmmss`). When this flag is present, each test run will create a separate timestamped folder, preventing reports from being overwritten.

#### -m
Specifies manual debug mode. Use this mode for debugging test scenarios locally. When a test fails, execution will pause for manual intervention before the clean-up process.

#### -h or -hide
*(Windows only)* Hides the console window during test execution.

#### -junit:\<junit-report-path>
In addition to the default XML report, generates a JUnit 2.6 compliant test report. Useful for publishing test results to Continuous Integration platforms.

#### -html:\<html-report-path>
In addition to the default XML report, generates an HTML test report. The test report will be generated in the given folder. The HTML report contains CSS, JavaScript, and screenshot files. If you want to share the report, you have to copy the whole folder.

#### -showReport
Launches `Report Viewer` after test execution to display the test results.

#### -encryptionKey:\<key>
Specifies the encryption key to use when decrypting sensitive data in test data files. Use with the `-encrypt` command to encrypt sensitive values.

### Parameters for Mobile testing
To run tests on mobile devices, specify `-platform` as `Android` or `iOS`, and use the following additional arguments:

#### -appId:\<application-path>
**Required for native mobile apps.** Specifies the path to the application package to be tested:
- For Android: `.apk` or `.aab` file
- For iOS: `.ipa` or `.app` file

This parameter is mandatory when using `-browser:AndroidNative` or `-browser:IOSNative`.

#### -grid:\<gridUrl>
Indicates the Selenium Grid URL to connect to a device cloud. Default value is `http://localhost:4723/wd/hub` for local Appium Server.
If you are using a cloud-based device cloud, please refer to your service provider's documentation.

> [!NOTE]
> When the `-grid` argument is provided, the `GridForDesktop` option will be automatically set to `true`, enabling Selenium Grid for both mobile and desktop tests.

#### -desktopGrid
Activates the usage of Selenium Grid for Web Desktop tests. When set, desktop browser tests will also run through the specified Selenium Grid.

#### -username:\<username>
Specifies the username for Selenium Grid authentication. Required by some cloud-based device providers.

#### -password:\<password>
Specifies the password for Selenium Grid authentication. Required by some cloud-based device providers.

> [!TIP]
> Use the `-encrypt` command to encrypt your password before storing it in configuration files.

#### -device:\<deviceName>
Specifies the device name for device selection. Examples: `iPhone Xs`, `Huawei P30`, `Emulator`. Refer to your cloud provider's documentation for available device names.

#### -osVersion:\<version>
Specifies the version of the operating system for device selection. Examples: `14.1`, `9.0`. Refer to your cloud provider's documentation for available OS versions.

### Special Commands

#### -encrypt
Encrypts a string value for secure storage in test data files. This must be the first argument.

**Syntax:**
```batch
WebRunner.exe "-encrypt" "<data-to-encrypt>" "-encryptionKey:<your-key>"
```

**Example:**
```batch
WebRunner.exe "-encrypt" "MyPassword123" "-encryptionKey:MySecretKey"
```

The encrypted value will be displayed in the console and can be used in your test data files.

### Where to find WebRunner package?
The WebRunner package is installed via NuGet Package (.NET) or Maven (Java).
After compilation of your test automation project, it will be found in the project output folder.
