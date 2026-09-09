# WebEngine C# (web)

Rules: descriptions live in PageModel only (never SharedAction/test). No driver in description ctor inside PageModel. Properties `get;set;`. Ctor takes `WebDriver` (not `IWebDriver`).

```csharp
using AxaFrance.WebEngine.Web;
namespace MyProject.PageModels {
  public class LoginPage : PageModel {
    // Source: <input id="username" name="username" type="text">
    public WebElementDescription UserName { get; set; } = new() { Id = "username" };
    // Source: <button data-testid="login-submit">Sign in</button>
    public WebElementDescription ButtonLogin { get; set; } = new() {
      Attributes = new HtmlAttribute[] { new("data-testid", "login-submit") } };
    public LoginPage(WebDriver driver) : base(driver) { }
  }
}
```

- Linear: `BrowserFactory.GetDriver(Platform.Windows, BrowserType.Chrome)` + PageModels directly.
- Gherkin (Reqnroll): PageModels in steps; ask for step class if missing.
- Keyword: `TestCaseWeb.TestSteps[]` → `SharedActionWeb` (`DoAction`/`DoCheckpoint`, `GetParameter`), `ParameterList` constants. Folders: `PageModels/ Actions/ TestCases/ TestData/`.
- Element ops: `Click/SetValue/SetSecure/SendKeys/Clear/SelectByText|Value|Index/CheckByValue/Exists/MouseHover/ScrollIntoView`.
- Packages: `AxaFrance.WebEngine.Web` (+ `Runner` for keyword only).
