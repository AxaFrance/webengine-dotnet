# Best Practices in WebEngine Framework

A key goal of WebEngine Framework is to embed best practices directly into the framework rather than leaving them as separate guidelines that teams may or may not follow.
This article explains each practice, why it matters, and exactly how the framework implements it so you benefit automatically.

## 1. Automatic WebDriver Management

**Problem with plain Selenium:** Every developer must manually download the correct ChromeDriver, EdgeDriver or GeckoDriver that matches their installed browser version.
When a browser auto-updates, tests break until the matching driver is also updated. In a team this creates friction and inconsistency.

**How WebEngine solves it:** <xref:AxaFrance.WebEngine.Web.BrowserFactory.GetDriver(AxaFrance.WebEngine.Platform,AxaFrance.WebEngine.BrowserType)> handles everything automatically.

```csharp
// One line — no driver download, no version tracking.
var driver = BrowserFactory.GetDriver(Platform.Windows, BrowserType.ChromiumEdge);
```

At runtime the factory:
1. Detects the installed browser version on the machine.
2. Downloads the matching driver from the official repository if not already cached.
3. Initializes and returns a ready-to-use `WebDriver`.

> [!TIP]
> The same `BrowserFactory` API works for desktop browsers (Windows, macOS), mobile browsers (Android Chrome, iOS Safari) and cloud Selenium Grids — your test code never changes.

---

## 2. Robust Element Identification with Multiple Locators

**Problem with plain Selenium:** A single locator (e.g. an XPath) is fragile. Minor HTML changes break tests, and maintenance cost grows quickly.

**How WebEngine solves it:** <xref:AxaFrance.WebEngine.Web.WebElementDescription> lets you combine multiple independent locators. The framework filters the DOM by each property in turn, so elements are identified more precisely and are resilient to partial HTML changes.

```csharp
// Identify by TagName AND ClassName — both must match.
var panel = new WebElementDescription(driver)
{
    TagName = "div",
    ClassName = "summary-panel"
};

// Identify using a non-standard HTML attribute (common in JS frameworks).
var customButton = new WebElementDescription(driver)
{
    Attributes = new HtmlAttribute[]
    {
        new HtmlAttribute("data-action", "submit")
    }
};
```

> [!NOTE]
> You can use any combination of `Id`, `Name`, `TagName`, `ClassName`, `CssSelector`, `XPath`, `InnerText`, `LinkText` and `Attributes` in a single description.
> The more locators you provide, the more precisely and robustly the element will be identified.

---

## 3. Built-in Retry and Synchronization

**Problem with plain Selenium:** Modern web applications update the DOM asynchronously. Without explicit waits or retry logic, tests throw `StaleElementReferenceException` or `NoSuchElementException` when the page is still loading or JavaScript is still running.
Developers typically add `Thread.Sleep()` calls or complex `WebDriverWait` chains, which are brittle and slow.

**How WebEngine solves it:** Every action on a `WebElementDescription` is wrapped in a **synchronized retry loop**. If the action fails because the DOM is updating, the framework automatically retries until the element is stable or the configurable timeout expires.

This means:

```csharp
// No Thread.Sleep(), no WebDriverWait — the framework handles it.
page.NextButton.Click();

// Waits until the title element exists (e.g. after a page navigation).
Assert.IsTrue(page.Page2Title.Exists());
```

See the full list of synchronized actions in [Development Status](dev-status.md).

> [!IMPORTANT]
> The retry pattern is transparent. You write clean, linear test logic and the framework absorbs the timing complexity.

---

## 4. Page Model Pattern (Separation of Concerns)

**Problem with plain Selenium:** Locators embedded directly inside test methods are impossible to reuse. When a UI changes, you must find and update every test that uses that element.

**How WebEngine solves it:** WebEngine formalizes the **Page Object Model** pattern through <xref:AxaFrance.WebEngine.Web.PageModel>. Each page or screen is represented by a class. Element descriptions live in the page model; test logic lives in actions or test cases.

```csharp
// Page model class — all element descriptions in one place.
public class LoginPage : PageModel
{
    public LoginPage(WebDriver driver) : base(driver) { }

    public WebElementDescription Username = new WebElementDescription
    {
        Id = "username"
    };
    public WebElementDescription Password = new WebElementDescription
    {
        Id = "password"
    };
    public WebElementDescription LoginButton = new WebElementDescription
    {
        TagName = "button",
        InnerText = "Login"
    };
}

// Test script — reads like a specification, no locators in sight.
var page = new LoginPage(driver);
page.Username.SetValue("admin@example.com");
page.Password.SetSecure(encryptedPassword);
page.LoginButton.Click();
```

Benefits:
- A UI change requires updating only the page model, not every test that uses the element.
- Test scripts are readable by non-developers (QA analysts, business users).
- Page models are reused across Linear Scripting, Gherkin, Keyword-Driven and Data-Driven approaches.

For detailed guidance, see [Organize UI Elements With PageModel](../tutorials/page-model.md).

---

## 5. Secure Credential Handling

**Problem with plain Selenium / test frameworks:** Passwords are often stored in plain text inside test scripts or test data files — a security risk especially for repositories and CI/CD pipelines.

**How WebEngine solves it:** The framework provides built-in encryption and a `SetSecure` method that decrypts and types the password on the fly, so the clear-text value never appears in source code, logs or test data.

```csharp
// Encrypt once (e.g. from command line: WebRunner.exe -encrypt mypassword)
// Then store the cipher text in your test data or appsettings.json.

var passwordBox = new WebElementDescription(driver) { Id = "password" };
passwordBox.SetSecure("Iw6buAX7oY97dbk/w3gXLA=="); // decrypts internally, never logged
```

Key rules enforced by the framework:
- `SetSecure` **only works on `<input type="password">` elements**. Calling it on any other element throws an exception, preventing accidental exposure.
- The decryption key can be supplied at runtime via `-encryptionKey` parameter, keeping it out of source control entirely.

See [Secure Passwords](../tutorials/secure-password.md) for the full guide.

---

## 6. Test Data Externalization

**Problem with plain Selenium:** Test data embedded in the test script is not reusable. Adding a new test case means duplicating code. Coverage cannot grow without touching scripts.

**How WebEngine solves it:** The framework separates test data from test logic. Test data is stored in **Excel spreadsheets** under a standardized format and loaded at runtime by the Test Runner.

- `PARAMS` sheet: Describes all possible parameters and their meaning.
- `ENV` sheet: Environment-dependent values (URLs, server names) — the same test suite runs on Dev, QA and Production just by switching the environment column.
- `TEST_SUITE` sheet: Lists test cases and assigns parameter values to each. Adding a new test case is adding a column in Excel — no code change required.

```
Test Coverage without code change:
┌────────────────────┬──────────────────┬──────────────────┬──────────────────┐
│ Parameter          │ TC_Apartment     │ TC_House         │ TC_Apartment+Acc │
├────────────────────┼──────────────────┼──────────────────┼──────────────────┤
│ HomeType           │ Apartment        │ House            │ Apartment        │
│ HasPreviousAccident│ false            │ false            │ true             │
│ ...                │ ...              │ ...              │ ...              │
└────────────────────┴──────────────────┴──────────────────┴──────────────────┘
```

See [Data-Driven Testing (.NET)](../tutorials/data-driven-cs.md) and the [Excel Add-in](excel-addin.md) for details.

---

## 7. Structured Project Organization

**Problem with plain Selenium:** Without conventions, a test project accumulates classes in random locations, making onboarding and maintenance hard.

**How WebEngine solves it:** The framework recommends a consistent folder structure, and the GitHub Copilot instructions file enforces it automatically when AI assistance is used.

```
<TestProject>
├── PageModels/       # One class per page/screen — WebElementDescriptions only
├── Actions/          # Reusable keywords / SharedActions
├── TestCases/        # Test case classes — sequences of actions + assertions
├── TestData/         # Excel or XML test data files
└── <OtherFiles>
```

This structure is compatible with all four test approaches: Linear Scripting, Gherkin, Keyword-Driven and Data-Driven.

---

## 8. AI-Assisted Test Authoring

**How WebEngine solves it:** WebEngine ships a **GitHub Copilot instructions file** and an **MCP server** that expose the framework conventions directly to AI assistants.

- The [Copilot instructions file](github-copilot.md) teaches Copilot to generate code that follows all the above best practices automatically: PageModels using `WebElementDescription`, actions in dedicated classes, test data externalized.
- The [MCP server](../articles/webengine-web.md) (`AxaFrance.WebEngine.Mcp`) exposes live Selenium and Appium capabilities to AI assistants, allowing them to inspect, interact with, and reason about a running browser session — enabling AI-driven exploratory testing.

---

## 9. Choosing the Right Test Approach

WebEngine supports four test approaches. Each has a clear use case:

| Approach | Best for | Reusability | Data separation |
|---|---|---|---|
| **Linear Scripting** | Simple scenarios, unit-style tests | Low | No |
| **Gherkin / BDD** | Feature acceptance tests, BDD teams | Medium | Partial |
| **Keyword-Driven** | Complex end-to-end tests, shared actions | High | Optional |
| **Keyword + Data-Driven** | Large regression suites, multi-environment | Highest | Yes (Excel) |

> [!TIP]
> For most professional test automation projects, the **Keyword-Driven + Data-Driven** combination is recommended. It gives the highest reusability, the lowest maintenance cost, and allows test coverage to grow without touching any code.

---

## Summary

| Best practice | Framework feature |
|---|---|
| No manual driver downloads | `BrowserFactory.GetDriver` |
| Robust element identification | Multi-locator `WebElementDescription` |
| Automatic retry & synchronization | Synchronized actions on `WebElementDescription` |
| Separation of locators and logic | `PageModel` pattern |
| Secure credential handling | `SetSecure` + `Encrypter` |
| Test data externalization | Excel format + `WebRunner` data loading |
| Consistent project structure | Folder conventions + Copilot instructions |
| AI-assisted authoring | MCP server + Copilot instructions file |
