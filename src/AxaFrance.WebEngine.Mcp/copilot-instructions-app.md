---
applyTo: '**/*.cs'
author: 'Huaxing YUAN'
---
# Copilot – Mobile Test Automation (WebEngine + MCP Appium)

> **Language rule:** Always reply to the user and write code comments in the same language the user writes in.

Framework: https://github.com/AxaFrance/webengine-dotnet

---

## 1. SUPPORTED TEST APPROACHES

| Approach | When to use |
|---|---|
| **Linear Scripting** | Simple or unit scenarios — direct script via PageObjectModel |
| **BDD / Gherkin** | Scenarios written in Gherkin with `Reqnroll` (SpecFlow is no longer maintained) |
| **Keyword-Driven** | Scenarios expressed with reusable keywords via `SharedActionApp` |

Analyse the project structure to determine the approach in use and state it before generating code.

---

## 2. MCP APPIUM WORKFLOW → SCRIPT GENERATION

When the user asks to explore an application or execute a mobile scenario, **use MCP Appium first** to interact with the live device, then generate WebEngine artefacts from the captured data.

### Step 1 — Start the session
```
StartSession(platform: 'Android'|'iOS', appPath: '/path/to/app.apk', deviceName: 'emulator-5554')
→ returns sessionId
```
If the platform, app path, or device name are not provided, ask for them before proceeding.

### Step 2 — Inspect the screen
```
GetAccessibilitySnapshot(sessionId)         ← default tool (compact, ~5-30 KB)
GetPageSourceChunk(sessionId, chunkIndex:0) ← if snapshot is missing required attributes
GetPageSource(sessionId)                    ← full XML — rarely needed
```

### Step 3 — Interact with elements (read the returned tag)
Every successful action returns a line:
```
Element tag: <android.widget.EditText resource-id="com.example:id/email" content-desc="Email" text="" />
```
**Use this XML tag to build the matching `AppElementDescription`** — it is the ground truth of the live UI hierarchy. Never guess a locator.

For multiple interactions on the same screen:
```
ExecuteBulkActions(sessionId, [ ... ])
→ Results[].ElementTag contains the XML tag of each resolved element
```

### Step 4 — Close the session and retrieve the action log
```
CloseSession(sessionId)
→ returns the path of the text file: C:\Users\...\appium-actions-<id>-<date>.txt
```
The file contains for each action:
- The opening XML tag of the element (`ElementTag`)
- The action type, value, and locators used

**Communicate this path to the user** and offer to use it to generate PageModels and test scripts.

### Step 5 — Generate WebEngine artefacts
From the `ElementTag` entries in the log:
1. Create or enrich the **PageModels** (see §4)
2. Generate the **test script** according to the detected approach (see §5)

---

## 3. LOCATOR STRATEGY (PRIORITY ORDER)

Always derive locators from a captured XML tag or snapshot — **never guess**.

| Priority | Locator | Source in the XML tag |
|---|---|---|
| 1 | `AccessibilityId` | `content-desc` (Android) / `accessibilityIdentifier` (iOS) |
| 2 | `Id` | `resource-id` without the package prefix |
| 3 | `UIAutomatorSelector` | Android — expressive native selector |
| 4 | `IosClassChain` | iOS — faster than XPath |
| 5 | `IosPredicate` | iOS — most expressive |
| 6 | `Text` | exact visible text |
| 7 | `ClassName` + other attribute | `android.widget.Button` combined |
| 8 | `XPath` | **last resort — fragile on native UI** |

### Native element type reference

| Role | Android | iOS |
|---|---|---|
| Text input | `android.widget.EditText` | `XCUIElementTypeTextField` |
| Password | `android.widget.EditText` (inputType=textPassword) | `XCUIElementTypeSecureTextField` |
| Button | `android.widget.Button` | `XCUIElementTypeButton` |
| Checkbox | `android.widget.CheckBox` | `XCUIElementTypeCheckBox` |
| Switch | `android.widget.Switch` | `XCUIElementTypeSwitch` |
| Dropdown | `android.widget.Spinner` | `XCUIElementTypePickerWheel` |
| List item | `android.widget.TextView` (clickable) | `XCUIElementTypeCell` |

---

## 4. PAGEOBJECT MODELS (WebEngine Mobile)

### Absolute rules
- **Never** place an `AppElementDescription` directly in a `SharedActionApp` or a test script.
- **Do not** pass the driver in the `AppElementDescription` constructor when inside a `PageModel`.
- Properties must have `get; set;`.
- The `PageModel` constructor takes a `WebDriver` (not `AppiumDriver` — that is the WebEngine base class).

### XML tag → AppElementDescription mapping

| Captured tag | AppElementDescription |
|---|---|
| `content-desc="login_button"` | `AccessibilityId = "login_button"` |
| `resource-id="com.app:id/email"` | `Id = "email"` (package prefix stripped) |
| `text="Sign in"` + `class="android.widget.Button"` | `Text = "Sign in"` + `ClassName = "android.widget.Button"` |
| `XCUIElementTypeTextField` + `label="Email"` | `IosClassChain = "**/XCUIElementTypeTextField[\`label == 'Email'\`]"` |
| No stable id | `UIAutomatorSelector = "new UiSelector().text(\"Email\")"` |

### Generated PageModel example

```csharp
using AxaFrance.WebEngine.MobileApp;

namespace MyProject.PageModels
{
    public class LoginPage : PageModel
    {
        // Source: <android.widget.EditText content-desc="email_field" resource-id="com.app:id/email_input" />
        public AppElementDescription EmailField { get; set; } = new AppElementDescription
        {
            AccessibilityId = "email_field"
        };

        // Source: <android.widget.EditText content-desc="password_field" resource-id="com.app:id/password_input" />
        public AppElementDescription PasswordField { get; set; } = new AppElementDescription
        {
            AccessibilityId = "password_field"
        };

        // Source: <android.widget.Button content-desc="login_button" text="Sign in" />
        public AppElementDescription LoginButton { get; set; } = new AppElementDescription
        {
            AccessibilityId = "login_button"
        };

        // Source: <android.widget.TextView resource-id="com.app:id/error_text" text="" />
        public AppElementDescription ErrorMessage { get; set; } = new AppElementDescription
        {
            Id = "error_text"
        };

        public LoginPage(WebDriver driver) : base(driver) { }
    }
}
```

---

## 5. RULES PER APPROACH

### 5.1 Linear Scripting
- Use PageModels directly — **no** `SharedActionApp`, **no** `TestCaseApp`.
- Obtain the driver via `AppFactory`:
  ```csharp
  var driver = AppFactory.GetDriver(Platform.Android);
  ```
- Example:
  ```csharp
  var driver = AppFactory.GetDriver(Platform.Android);
  var page = new LoginPage(driver);
  page.EmailField.SetValue("user@test.com");
  page.PasswordField.SetValue("password");
  page.LoginButton.Click();
  ```

### 5.2 BDD / Gherkin (Reqnroll)
- Ask for the step definitions class if the user does not provide it.
- Use PageModels inside steps — may delegate to `SharedActionApp` for reuse.
- Step example:
  ```csharp
  [Given("the user is authenticated as {string}")]
  public void GivenAuthenticated(string user)
  {
      var page = new LoginPage(_driver);
      page.EmailField.SetValue(user);
      page.PasswordField.SetValue(GetParameter("PASSWORD"));
      page.LoginButton.Click();
  }
  ```

### 5.3 Keyword-Driven
Folder structure (inside the project folder, not the solution root):
```
PageModels/    ← AppElementDescription + PageModel
Actions/       ← SharedActionApp (reusable)
TestCases/     ← TestCaseApp with TestSteps[]
TestData/      ← XML test data files
```

**TestCase:**
```csharp
[Description("Mobile login and navigation")]
public class TC_LoginMobile : TestCaseApp
{
    public TC_LoginMobile()
    {
        TestSteps = new TestStep[]
        {
            new TestStep { Action = nameof(Login) },
            new TestStep { Action = nameof(NavigateHome) },
            new TestStep { Action = nameof(Logout) },
        };
    }
}
```

**SharedActionApp (never put AppElementDescription here):**
```csharp
public class Login : SharedActionApp
{
    public override Variable[]? RequiredParameters => null;

    public override void DoAction(AppiumDriver driver)
    {
        var page = new LoginPage(driver);
        page.EmailField.SetValue(GetParameter(ParameterList.USER));
        page.PasswordField.SetValue(GetParameter(ParameterList.PASSWORD));
        page.LoginButton.Click();
    }

    public override bool DoCheckpoint(AppiumDriver driver)
    {
        var page = new LoginPage(driver);
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
    /// <summary>User login</summary>
    public static string USER { get; } = "USER";
    /// <summary>Password</summary>
    public static string PASSWORD { get; } = "PASSWORD";
    /// <summary>Device name / emulator</summary>
    public static string DEVICE_NAME { get; } = "DEVICE_NAME";
}
```

---

## 6. AppElementDescription ACTIONS (quick reference)

```csharp
element.Click();                      // Tap
element.SetValue("texte");            // saisir du texte (efface avant)
element.SendKeys("texte");            // frappe caractère par caractère
element.Clear();
element.Exists(timeoutSec);           // vérifie la présence (retourne bool)
element.ScrollIntoView();
element.SelectByText("Option");       // Spinner / liste
element.SelectByValue("val");         // Spinner / liste par valeur
element.CheckByValue("radio_val");    // groupe de radio
```

---

## 7. GESTURES AND NAVIGATION (MCP Appium)

| Need | MCP Tool |
|---|---|
| Scroll down (reveal content below) | `SwipeScreen(direction: 'Up')` |
| Scroll up (reveal content above) | `SwipeScreen(direction: 'Down')` |
| Long press | `LongPressElement(element, durationMs: 1500)` |
| Hide keyboard | `HideKeyboard(sessionId)` |
| Back button (Android) | `PressBack(sessionId)` |
| Wait for element | `WaitForElement(sessionId, element, timeoutSeconds: 10)` |

---

## 8. TEST DATA FORMAT (Keyword-Driven)

```xml
<?xml version="1.0" encoding="utf-8"?>
<TestSuiteData xmlns="http://www.axa.fr/WebEngine/2022">
  <TestData>
    <TestName>Login_Android_Standard</TestName>
    <Data>
      <Variable><Name>TESTCASE</Name><Value>Login_Android_Standard</Value></Variable>
      <Variable><Name>USER</Name><Value>testuser@axa.fr</Value></Variable>
      <Variable><Name>DEVICE_NAME</Name><Value>emulator-5554</Value></Variable>
    </Data>
  </TestData>
</TestSuiteData>
```

---

## 9. REQUIRED NUGET PACKAGES

| Package | Usage |
|---|---|
| `AxaFrance.WebEngine.MobileApp` | Mobile tests (Appium) |
| `AxaFrance.WebEngine.Runner` | Keyword-Driven approach only |

Check their presence in the project; install them or ask the user to do so if missing.

---

## 10. SCREEN INSPECTION POLICY (MCP Appium)

| Tool | When to use |
|---|---|
| `GetAccessibilitySnapshot` | **First choice** — compact per-element view (~5-30 KB) |
| `GetPageSourceChunk` | If the snapshot is missing precise attributes (bounds, platform-specific attributes) |
| `GetPageSource` | Full XML — rarely needed |
| `TakeScreenshot` | Visual verification or debugging |

**Re-inspect only when:**
- An expected element is absent or not interactable.
- Navigation changes the screen (activity / view change detected from the action result).
- A validation message or alert must be verified.
- A locator fails and more context is needed.

**Batched actions:** if a single snapshot reveals multiple interactions on the same screen, submit them all in one `ExecuteBulkActions` call before re-inspecting.

### `ExecuteBulkActions` supported action types

| ActionType | Description |
|---|---|
| `Tap` | Taps an element |
| `TypeText` | Types text (clears first by default, control via `ClearFirst`) |
| `SetText` | Clears then sets text |
| `Clear` | Clears a text field |
| `LongPress` | Long-presses an element (`DurationMs`, default 1500 ms) |
| `SwipeUp` / `SwipeDown` / `SwipeLeft` / `SwipeRight` | Swipe gesture (no element needed) |
