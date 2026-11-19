### Changelog
All notable changes to this project will be documented in this file.

#### Unreleased
+ Replaced the legacy VSTO Excel Add-in with a new Excel-DNA add-in (`AxaFrance.WebEngine.ExcelAddin`), keeping the worksheet format and settings file.
+ Restored the icon-based browser selection dialog and the original three-field settings layout.
+ Added a custom Stop icon and fixed ribbon loading by declaring the image callback on the `customUI` root.
+ Added Excel add-in automation macros and unit tests for data extraction, filters, runner arguments and settings.
+ Added a GitHub Actions workflow that publishes the packed XLL as a downloadable artifact and attaches it to `v*` releases.
+ Updated dependencies: Selenium 4.53, Appium 9, Deque Axe 4.14, ModelContextProtocol 2.4, Swashbuckle 11, WebView2 1.0.4314, and current test tooling.
+ Fixed Chrome `userPreference=` and `binarylocation=` option parsing.
+ Firefox sessions now use an isolated temporary profile by default (`profile=<path>` opts back in), avoiding migration and update prompts.
+ Fixed `EnvironmentVariables.LoadFrom(Stream)` using the wrong serializer type.
+ Hardened XML deserialization against DTD/XXE payloads.
+ Warnings and errors are no longer duplicated in system output, and report folder timestamps now use a 24-hour clock.

#### 3.22.236
+ All documentation with .NET
+ Upgraded to Selenium 4.4

#### 3.22.210
+ Support additional capabilities from configuration file
+ Support Drag and Drop in desktop browsers


#### 3.22.1
+ Complete refactoring of the existing Framework by using .NET6.0 and multi targets 
