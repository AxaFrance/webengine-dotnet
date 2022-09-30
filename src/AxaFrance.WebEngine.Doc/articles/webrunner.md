# Launch Test with Web Runner
`WebRunner` is the component to run automated test and provided rich XML based report.
Base on the programming language used to build test automation solution, the command-line has a little difference.
Meanwhile, all behaviors and arguments remain the same.

> [!WARNING]
> Spacing characters are not accepted as parameter values. Parameter with space must be enclosed in double-quotes.

# [.NET](#tab/netcore)
```batch
webrunner "-a:<testproject>" [-data:<testdata>] [-env:<env_ariable>] [-browser:<browser-type>] [optional_arguments]
```
# [Java](#tab/java)
```batch
java -jar webrunner.jar "-a:<testproject>" [-data:<testdata>] [-env:<env_ariable>] [-browser:<browser-type>] [optional_arguments]
```
***

## Prerequisites
To run tests build with WebEngine Framework, is is required to provide compiled test automation solution.
* `-a:<testproject>`: the filename or full path of compiled test project: DLL library for .NET, or a JAR package for Java.

If the test is Data-Driven, it is required to provide at least one of following files:
* `-data:<testdata>`: Test Data to be used for data-driven test execution, in XML format. The file can be exported via `Excel Add-in`
* `-env:<env_variable>`: Environment Variables in XML format, to store test environment related test data, such as URLs, Hostnames or Credentials.


### Examples
#### Run test on desktop browsers
For following command line launches the solution in `Firefox` browser and generate test reports in `C:\Temp`
```batch
WebRunner.exe "-a:MyProject.dll" "-data:Data.xml" "-env:Staging.xml" -browser:Firefox "-outputDir:C:\Temp"
```
#### Run webapp test on Android Emulator
Following command line launches same test under emulated `Android` device (requires Android Emulator + Appium Server on local) using `Chrome` browser.
```batch
WebRunner.exe "-a:MyProject.dll" "-data:Data.xml" "-env:Staging.xml" -platform:Android -browser:Chrome "-device:Emulator" "-outputDir:C:\Temp"
```
#### Run app test on Selenium compatible Device Cloud
Following command line installs the application package, then launches test under `iPhone 12`, using provided device cloud.
```batch
WebRunner.exe "-a:AppProject.dll" "-data:Data.xml" "-env:Staging.xml" -platform:iOS -browser:IOSNative "-device:iPhone 12" "-grid:http://seleniumgrid.com/wd/hub" "-appId:C:\app_under_test.ipa"
```


## Optional parameters

> [!NOTE]
> Some parameters can be provided in the configuration file `appsettings.json`. please refer to [Test Configuration](appsettings.md)

### Commun parameters
##### -browser:\<browser>
Indicates the browser to run test on. see: <xref:AxaFrance.WebEngine.BrowserType>.
##### -platform:\<platform>
The platform for test execution: see: <xref:AxaFrance.WebEngine.Platform>. Default value is `Windows` stands for Desktop Browsers.
##### -outputDir:\<outputFolder>
Indicates the folder to store output of test execution and test report. This parameter is can be defined in `appsetting.json`.
##### -m
Manual debug mode, use to debug test scenarios locally. When the test is failed test will pause for manual intervention before clean-up process.
##### junit:\<junitPath>
In additional of default XML report, generates a JUnit 2.6 compliant test report. Useful to publish test result on an Continuous Integration Platform
##### -showReport
Launches `Report Viewer` after test execution.

### Parameters for Mobile testing
To run tests on Mobile device, `-platform` must be specified to `Android` or `iOS`.

##### -grid:\<gridUrl>
Indicates the Selenium Grid to connect to device cloud. Default value is `http://localhost:4723/wd/hub` stands for local Appium Server
If you are using cloud-based device cloud, please refer to service providers documentation.

##### -username:\<username>
Indicates the username to be used for Selenium Grid authentication.

##### -password:\<password>
Indicates the password to be used for Selenium Grid authentication.

##### -device:\<deviceName>
Indicates the device name for device selection. for example: `iPhone Xs`, `Huawei P30`. Refers your cloud provider.

##### -osVersion:\<version>
Indicate the version of the OS for device selection. for example: `14.1`, `9.0`. Refers your cloud provider.

### Where to find WebRunner package?
WebRunner package is installed via NuGet Package (.NET) or Maven (Java).
After compilation of your test automation project, it will be found in project output folder.
