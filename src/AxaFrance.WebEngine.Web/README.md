# AxaFrance.WebEngine.Web

Web automation with the AXA WebEngine Framework on Selenium WebDriver: `BrowserFactory` (no manual driver download), `WebElementDescription` locators, `PageModel`, and `SharedActionWeb` / `TestCaseWeb` for Keyword-Driven suites on desktop and mobile browsers.

```powershell
dotnet add package AxaFrance.WebEngine.Web
```

```csharp
var driver = BrowserFactory.GetDriver(Platform.Windows, BrowserType.Chrome);
driver.Navigate().GoToUrl("https://www.axa.fr");
```

---
Company: AXA France
Author: Huaxing YUAN
Repository: https://github.com/AxaFrance/webengine-dotnet
Documentation: https://axafrance.github.io/webengine-dotnet/
