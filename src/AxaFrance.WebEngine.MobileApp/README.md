# AxaFrance.WebEngine.MobileApp

Mobile automation with the AXA WebEngine Framework on Appium: `AppFactory`, `AppElementDescription` locators, `PageModel`, and `SharedActionApp` / `TestCaseApp` for Keyword-Driven suites on Android and iOS (real devices and emulators).

```powershell
dotnet add package AxaFrance.WebEngine.MobileApp
```

```csharp
var driver = AppFactory.GetDriver(Platform.Android);
```

Requires an Appium server (default `http://localhost:4723`).

---
Company: AXA France
Author: Huaxing YUAN
Repository: https://github.com/AxaFrance/webengine-dotnet
Documentation: https://axafrance.github.io/webengine-dotnet/
