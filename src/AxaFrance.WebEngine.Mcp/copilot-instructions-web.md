---
applyTo: '**/*.cs'
author: 'Huaxing YUAN'
---
# Copilot – Web Test Automation (WebEngine + MCP Selenium)

> **Language rule:** Always reply to the user and write code comments in the same language the user writes in.


Framework: https://github.com/AxaFrance/webengine-dotnet

---

## 1. SUPPORTED TEST APPROACHES

| Approach | When to use |
|---|---|
| **Linear Scripting** | Simple or unit scenarios — direct script via PageObjectModel |
| **BDD / Gherkin** | Scenarios written in Gherkin with `Reqnroll` (SpecFlow is no longer maintained) |
| **Keyword-Driven** | Scenarios expressed with reusable keywords via `SharedActionWeb` |

Analyse the project structure to determine the approach in use and state it before generating code.

---

## 2. MCP SELENIUM WORKFLOW → SCRIPT GENERATION

When the user asks to explore a page or execute a scenario, **use MCP Selenium first** to interact with the live browser, then generate WebEngine artefacts from the captured data.

### Step 1 — Start the session
```
StartSession(browserType: 'Chrome', headless: false)
→ returns sessionId
```

### Step 2 — Navigate and inspect
```
NavigateTo(sessionId, url)
GetAccessibilitySnapshot(sessionId)        ← default tool (compact)
GetActionableHtml(sessionId)               ← if snapshot is insufficient
GetPageHtml(sessionId)                     ← if HTML ≤ 30 KB and prose context needed
GetPageHtmlChunk(sessionId, chunkIndex:0)  ← if HTML > 30 KB
```

### Step 3 — Interact with elements (read the returned tag)
Every successful action returns a line:
```
Element tag: <input id="email" name="email" type="text" class="form-control">
```
**Use this HTML tag to build the matching `WebElementDescription`** — it is the ground truth of the live DOM. Never guess a locator.

For multiple interactions on the same page:
```
ExecuteBulkActions(sessionId, [ ... ])
→ Results[].ElementTag contains the HTML tag of each resolved element
```

### Step 4 — Close the session and retrieve the action log
```
CloseSession(sessionId)
→ returns the path of the JSON file: C:\Users\...\selenium-actions-<id>-<date>.json
```
The JSON file contains for each action:
- `ElementTag`: opening HTML tag of the element (source for PageModels)
- `StableLocator`: stable locators extracted from the live DOM (id, name, data-testid, aria-label…)
- `ActionType`, `Value`, `Url`, `StepNumber`

**Communicate this path to the user** and offer to open it to generate PageModels and test scripts.

### Step 5 — Generate WebEngine artefacts
From the `ElementTag` and `StableLocator` entries in the log:
1. Create or enrich the **PageModels** (see §4)
2. Generate the **test script** according to the detected approach (see §5)

---

## 3. LOCATOR STRATEGY (PRIORITY ORDER)

Always derive locators from a captured HTML tag or snapshot — **never guess**.

| Priority | Locator | Example from captured tag |
|---|---|---|
| 1 | `Id` | `id="submitBtn"` |
| 2 | `Name` | `name="email"` |
| 3 | Test attribute | `data-testid="next-step"` |
| 4 | Aria attribute | `aria-label="Validate"` |
| 5 | `TagName` + `InnerText` | `<button>Continue</button>` |
| 6 | `TagName` + `Name`/`Id` | unique combination |
| 7 | `LinkText` | for `<a>` only |
| 8 | `ClassName` | only if stable |
| 9 | `CssSelector` / `XPath` | **last resort only** |

---

## 4. PAGEOBJECT MODELS (WebEngine)

### Absolute rules
- **Never** place a `WebElementDescription` directly in a `SharedAction` or a test script.
- **Do not** pass the driver in the `WebElementDescription` constructor when inside a `PageModel` (the `PageModel` handles it).
- Properties must have `get; set;`.
- The `PageModel` constructor takes a `WebDriver` (not `IWebDriver`).

### HTML tag → WebElementDescription mapping

| Captured tag | WebElementDescription |
|---|---|
| `<input id="email">` | `Id = "email"` |
| `<input name="phone" type="tel">` | `Name = "phone"` |
| `<button data-testid="submit">` | `Attributes = [new HtmlAttribute("data-testid","submit")]` |
| `<select id="country">` | `Id = "country"` |
| `<input type="checkbox" id="agree">` | `Id = "agree"` |
| `<a>Legal notices</a>` | `LinkText = "Legal notices"` |
| `<div aria-label="menu">` | `Attributes = [new HtmlAttribute("aria-label","menu")]` |

### Generated PageModel example

```csharp
using AxaFrance.WebEngine.Web;

namespace MyProject.PageModels
{
    public class LoginPage : PageModel
    {
        // Source: <input id="username" name="username" type="text">
        public WebElementDescription UserName { get; set; } = new WebElementDescription
        {
            Id = "username"
        };

        // Source: <input id="password" name="password" type="password">
        public WebElementDescription Password { get; set; } = new WebElementDescription
        {
            Name = "password"
        };

        // Source: <button data-testid="login-submit" type="submit">Sign in</button>
        public WebElementDescription ButtonLogin { get; set; } = new WebElementDescription
        {
            Attributes = new HtmlAttribute[] { new HtmlAttribute("data-testid", "login-submit") }
        };

        // Source: <span id="error-msg" class="alert alert-danger">
        public WebElementDescription ErrorMessage { get; set; } = new WebElementDescription
        {
            Id = "error-msg"
        };

        public LoginPage(WebDriver driver) : base(driver) { }
    }
}
```

---

## 5. RULES PER APPROACH

### 5.1 Linear Scripting
- Use PageModels directly — **no** `SharedActionWeb`, **no** `TestCaseWeb`.
- Obtain the driver via `BrowserFactory`:
  ```csharp
  var driver = BrowserFactory.GetDriver(Platform.Windows, BrowserType.Chrome);
  ```
- Example:
  ```csharp
  var driver = BrowserFactory.GetDriver(Platform.Windows, BrowserType.Chrome);
  driver.Navigate().GoToUrl("https://example.com");
  var page = new LoginPage(driver);
  page.UserName.SetValue("user@test.com");
  page.Password.SetSecure("encryptedPwd");
  page.ButtonLogin.Click();
  ```

### 5.2 BDD / Gherkin (Reqnroll)
- Ask for the step definitions class if the user does not provide it.
- Use PageModels inside steps — may delegate to `SharedActionWeb` for reuse.
- Step example:
  ```csharp
  [Given("the user is logged in as {string}")]
  public void GivenUserLoggedIn(string user)
  {
      var page = new LoginPage(_driver);
      page.UserName.SetValue(user);
      page.Password.SetSecure(GetParameter("EncPassword"));
      page.ButtonLogin.Click();
  }
  ```

### 5.3 Keyword-Driven
Folder structure (inside the project folder, not the solution root):
```
PageModels/    ← WebElementDescription + PageModel
Actions/       ← SharedActionWeb (reusable)
TestCases/     ← TestCaseWeb with TestSteps[]
TestData/      ← XML test data files
```

**TestCase:**
```csharp
[Description("Car insurance quote")]
public class TC_InsuranceQuote : TestCaseWeb
{
    public TC_InsuranceQuote()
    {
        TestSteps = new TestStep[]
        {
            new TestStep { Action = nameof(Login) },
            new TestStep { Action = nameof(FillVehicle) },
            new TestStep { Action = nameof(ValidateQuote) },
        };
    }
}
```

**SharedAction (never put WebElementDescription here):**
```csharp
public class Login : SharedActionWeb
{
    public override Variable[]? RequiredParameters => null;

    public override void DoAction()
    {
        Browser.Navigate().GoToUrl(GetParameter(ParameterList.URL));
        var page = new LoginPage(Browser);
        page.UserName.SetValue(GetParameter(ParameterList.USER));
        page.Password.SetSecure(GetParameter(ParameterList.ENC_PASSWORD));
        page.ButtonLogin.Click();
    }

    public override bool DoCheckpoint()
    {
        var page = new LoginPage(Browser);
        if (page.ErrorMessage.Exists(5))
        {
            Information.AppendLine("Login error detected.");
            return false;
        }
        return true;
    }
}
```

**ParameterList:**
```csharp
public static class ParameterList
{
    /// <summary>Target environment URL</summary>
    public static string URL { get; } = "URL";
    /// <summary>User login</summary>
    public static string USER { get; } = "USER";
    /// <summary>Encrypted password</summary>
    public static string ENC_PASSWORD { get; } = "ENC_PASSWORD";
}
```

---

## 6. WebElementDescription ACTIONS (quick reference)

```csharp
element.Click();
element.SetValue("texte");           // input texte standard
element.SetSecure("chiffré");        // input password avec valeur chiffrée
element.SendKeys("texte");           // frappe caractère par caractère
element.Clear();
element.SelectByText("Option");      // <select> par texte visible
element.SelectByValue("val");        // <select> par attribut value
element.SelectByIndex(2);           // <select> par index
element.CheckByValue("radio_val");   // groupe de radio buttons
element.Exists(timeoutSec);          // vérifie la présence (retourne bool)
element.MouseHover();
element.ScrollIntoView();
```

---

## 7. TEST DATA FORMAT (Keyword-Driven)

```xml
<?xml version="1.0" encoding="utf-8"?>
<TestSuiteData xmlns="http://www.axa.fr/WebEngine/2022">
  <TestData>
    <TestName>Devis_Auto_Standard</TestName>
    <Data>
      <Variable><Name>TESTCASE</Name><Value>Devis_Auto_Standard</Value></Variable>
      <Variable><Name>URL</Name><Value>https://www.example.com/devis</Value></Variable>
      <Variable><Name>USER</Name><Value>testuser@axa.fr</Value></Variable>
    </Data>
  </TestData>
</TestSuiteData>
```

---

## 8. REQUIRED NUGET PACKAGES

| Package | Usage |
|---|---|
| `AxaFrance.WebEngine.Web` | Web tests (Selenium) |
| `AxaFrance.WebEngine.Runner` | Keyword-Driven approach only |

Check their presence in the project; install them or ask the user to do so if missing.

---

## 9. DOM INSPECTION POLICY (MCP Selenium)

| Tool | When to use |
|---|---|
| `GetAccessibilitySnapshot` | **First choice** — compact view, identifies interactive elements |
| `GetActionableHtml` | Less noise than full HTML, ideal for forms |
| `GetPageHtml` | If HTML ≤ 30 KB and prose context is needed |
| `GetPageHtmlChunk` | If HTML > 30 KB — reads in 10 KB slices |
| `TakeScreenshot` | Visual verification or debugging an unexpected state |

**Re-inspect only when:**
- An expected element is absent or not interactable.
- The URL changes or the page size grows significantly after an action.
- An error/validation message must be verified.
- A locator fails and more context is needed.

**Batched actions:** if a single snapshot reveals multiple interactions, submit them all in one `ExecuteBulkActions` call before re-inspecting.
