# WebEngine MCP Server - Complete Guide

## Overview

The **WebEngine MCP (Model Context Protocol) Server** is a bridge between AI-powered coding agents and your applications. It enables agents to:

- **Observe** application state in real-time
- **Execute** tests and interact with UI elements
- **Generate** production-ready WebEngine test scripts from observed behavior

## Running the MCP Server Locally

### Prerequisites

- .NET 8 or later
- One of the following browsers installed (for Selenium tests):
  - Google Chrome
  - Mozilla Firefox
  - Microsoft Edge
  - Safari (macOS)

### Start the Server

Navigate to the MCP project directory and run:

```powershell
dotnet run --project AxaFrance.WebEngine.Mcp
```

The server will start on **`https://localhost:5001`** by default.

### Configuration

Edit `appsettings.json` to customize:

```json
{
  "Https": {
    "Url": "https://localhost",
    "Port": 5001
  },
  "Selenium": {
    "DefaultBrowserType": "Chrome",
    "DefaultHeadless": false,
    "ImplicitWait": 10000
  },
  "Appium": {
    "DefaultPlatform": "Android",
    "DefaultApp": "com.example.app"
  }
}
```

## Philosophy: Token Efficiency

The MCP server is designed to minimize token usage while maximizing information quality:

### Three-Tier HTML Capture Strategy

1. **GetAccessibilitySnapshot** (Default - Most Compact)
   - Captures only interactive elements (inputs, buttons, links, etc.)
   - Includes element properties and stable locators
   - ~2-5 KB typical output
   - **Use by default** for most scenarios

2. **GetActionableHtml** (Medium Detail)
   - Provides interactive elements with surrounding context
   - Removes non-interactive noise (styling, ads, tracking)
   - ~10-20 KB typical output
   - **Use when** snapshot is insufficient or element visibility needs context

3. **GetPageHtml** (Full Context - Use Sparingly)
   - Complete page HTML for deep analysis
   - Only recommended for pages ≤ 30 KB
   - For larger pages, use **GetPageHtmlChunk**
   - ~50+ KB typical output
   - **Use when** prose context or styling is critical

### Batch Processing for Efficiency

Execute multiple actions in a single call using **ExecuteBulkActions**:

```
ExecuteBulkActions(sessionId, [
  { action: 'SetValue', locator: { id: 'email' }, value: 'test@example.com' },
  { action: 'SetValue', locator: { id: 'password' }, value: 'myPassword' },
  { action: 'Click', locator: { id: 'submitBtn' } }
])
```

Benefits:
- Reduces multiple round-trips to one
- Atomic transaction across related actions
- Returns element HTML tags for all actions simultaneously

### Real HTML Tags for Code Generation

Every action returns the captured HTML tag of the resolved element:

```
Element tag: <input id="email" name="email" type="text" class="form-control" data-testid="email-input">
```

This HTML tag is the **ground truth** for generating WebElementDescription locators. Never guess locators—use what the tool returns.

## Available Tools - Selenium (Web Testing)

### Session Management

| Tool | Purpose | AI Usage |
|---|---|---|
| **StartSession** | Create a new browser session | Call first; returns `sessionId` for all subsequent calls |
| **CloseSession** | Close session & retrieve action log | Call last; returns JSON log with all element tags & locators |

### DOM Inspection

| Tool | Purpose | AI Usage |
|---|---|---|
| **GetAccessibilitySnapshot** | Compact interactive elements only | **Use by default** — fast, token-efficient, element-focused |
| **GetActionableHtml** | Interactive elements + structural context | Use when snapshot lacks visibility context |
| **GetPageHtml** | Full page HTML (pages ≤30 KB) | Use for complete page analysis when size permits |
| **GetPageHtmlChunk** | 10 KB page slices | Use for large pages; fetch chunk 0 to learn total count |

### Navigation & State

| Tool | Purpose | AI Usage |
|---|---|---|
| **NavigateTo** | Navigate to URL; returns final URL | Essential for multi-page workflows |
| **GetCurrentUrl** | Get current browser URL | Quick URL verification without DOM fetch |
| **GetPageTitle** | Get page title | Quick page identity check |
| **GoBack** | Browser back button | Simulate navigation backward |
| **GoForward** | Browser forward button | Simulate navigation forward |

### Element Actions (Single)

| Tool | Purpose | AI Usage |
|---|---|---|
| **ClickElement** | Click an element; returns element HTML tag | Use for buttons, links; returns tag for PageModel generation |
| **TypeText** | Type into input field; returns element tag | Use for text fields; `clearFirst` option controls clearing |
| **SetText** | Clear then set input value; returns tag | Alternative to TypeText |
| **ClearText** | Clear input field | Use for pre-cleared field scenarios |
| **SelectFromDropdownByText** | Select option by visible text; returns tag | Use for `<select>` elements with text matching |
| **SelectFromDropdownByValue** | Select option by value attribute; returns tag | Use for `<select>` elements with value matching |
| **CheckElement** | Check checkbox/radio button; returns tag | Use for checkboxes and radio buttons |
| **UncheckElement** | Uncheck a checkbox; returns tag | Use to unselect a checkbox |

### Inspection

| Tool | Purpose | AI Usage |
|---|---|---|
| **GetElementHtml** | Get cleaned HTML of a specific element | Extract element structure for analysis |
| **GetElementText** | Get visible text of an element | Verify element content without full page fetch |

### Scrolling & Interaction

| Tool | Purpose | AI Usage |
|---|---|---|
| **ScrollBy** | Scroll page by pixel offset | Reveal off-screen content |
| **ScrollToElement** | Scroll element into view center | Position element before interaction |
| **ClickAt** | Click at viewport coordinates (x, y) | Last resort for canvas/custom widgets |
| **WaitForElement** | Wait for element (up to timeout) | Synchronize async UI loads |

### Utilities

| Tool | Purpose | AI Usage |
|---|---|---|
| **TakeScreenshot** | Screenshot as base64 PNG | Visual verification or debugging |
| **ExecuteScript** | Run JavaScript; return result as string | Custom DOM queries or state checks |
| **GetActiveSessions** | List all open sessions | Verify session state |

### Bulk Actions

| Tool | Purpose | AI Usage |
|---|---|---|
| **ExecuteBulkActions** | Run multiple actions in one call | **Primary tool for efficiency** — batch all actions from one DOM inspection |

**ExecuteBulkActions ActionTypes:** `Click` `TypeText` `SetText` `Clear` `SelectByText` `SelectByValue` `Check` `Uncheck`

**Returns per action:** `Status` (Success/Failed), `ElementTag` (HTML tag of resolved element), `ErrorMessage` (if failed)

## Available Tools - Appium (Mobile Testing)

### Session Management

| Tool | Purpose | AI Usage |
|---|---|---|
| **StartSession** | Create mobile app session (iOS/Android) | Call first; connect to real device or emulator |
| **CloseSession** | Close session & retrieve action log | Call last; returns action log with element tags |
| **GetActiveSessions** | List all open Appium sessions | Verify session state |

### Screen Inspection

| Tool | Purpose | AI Usage |
|---|---|---|
| **GetAccessibilitySnapshot** | Compact interactive elements (accessibility tree) | **Use by default** — one-line-per-element format |
| **GetPageSource** | Full XML page source (Appium UI hierarchy) | Use for raw attribute analysis |
| **GetPageSourceChunk** | 15 KB page source slices | Use for large screens; fetch chunk 0 first |

### Element State

| Tool | Purpose | AI Usage |
|---|---|---|
| **GetElementText** | Get visible text of element | Verify content without full screen fetch |
| **TakeScreenshot** | Screenshot as base64 PNG | Visual verification |

### Element Interactions (Single)

| Tool | Purpose | AI Usage |
|---|---|---|
| **TapElement** | Tap (click) an element; returns compact state | Use for buttons, cells |
| **LongPressElement** | Long-press element (context menus); returns state | Use for gestures; DurationMs parameter (default: 1500) |
| **TypeText** | Type into text field; returns state | Use for inputs; `clearFirst` option (default: true) |
| **ClearText** | Clear text field | Use before TypeText alternative |
| **WaitForElement** | Wait for element (up to timeout) | Synchronize async UI loads |

### Gestures

| Tool | Purpose | AI Usage |
|---|---|---|
| **SwipeScreen** | Swipe in direction: Up, Down, Left, Right | Scroll lists, carousels; DurationMs (default: 600) |
| **PressBack** | Press hardware Back button (Android only) | Navigation on Android; use tap for iOS back button |
| **HideKeyboard** | Hide software keyboard if open | Clean state before screenshot |

### Bulk Actions

| Tool | Purpose | AI Usage |
|---|---|---|
| **ExecuteBulkActions** | Run multiple actions in one call | **Primary tool for efficiency** — batch all actions |

**ExecuteBulkActions ActionTypes:** `Tap` `TypeText` `SetText` `Clear` `LongPress` `SwipeUp` `SwipeDown` `SwipeLeft` `SwipeRight`

**Returns per action:** `Status` (Success/Failed), `ElementTag` (XML opening tag), `ErrorMessage` (if failed)

## Action Log Format

When you close a session, the MCP server generates a JSON action log at the returned path:

```json
{
  "sessionId": "sess_12345abc",
  "startTime": "2024-01-15T10:30:00Z",
  "endTime": "2024-01-15T10:35:15Z",
  "totalDuration": 315000,
  "url": "https://example.com/login",
  "actions": [
    {
      "stepNumber": 1,
      "actionType": "Navigate",
      "url": "https://example.com/login",
      "timestamp": "2024-01-15T10:30:00Z"
    },
    {
      "stepNumber": 2,
      "actionType": "SetValue",
      "elementTag": "<input id=\"email\" name=\"email\" type=\"text\">",
      "stableLocator": { "id": "email" },
      "value": "test@example.com",
      "timestamp": "2024-01-15T10:30:02Z"
    },
    {
      "stepNumber": 3,
      "actionType": "Click",
      "elementTag": "<button id=\"submitBtn\" class=\"btn-primary\">Sign in</button>",
      "stableLocator": { "id": "submitBtn" },
      "timestamp": "2024-01-15T10:30:03Z"
    }
  ]
}
```

## Workflow: From Observation to Test Script

### Step 1: Start Session and Navigate
```
StartSession(browserType: 'Chrome', headless: false)
→ sessionId: "sess_12345abc"

NavigateTo(sessionId: "sess_12345abc", url: "https://example.com/login")
```

### Step 2: Capture Page State
```
GetAccessibilitySnapshot(sessionId: "sess_12345abc")
→ Compact HTML with all interactive elements
```

### Step 3: Execute Actions and Capture Locators
```
ExecuteBulkActions(sessionId: "sess_12345abc", actions: [
  { action: 'SetValue', locator: { id: 'email' }, value: 'test@example.com' },
  { action: 'SetValue', locator: { id: 'password' }, value: 'myPassword' },
  { action: 'Click', locator: { id: 'submitBtn' } }
])
→ Returns element HTML tags with stable locators
```

### Step 4: Retrieve Action Log
```
CloseSession(sessionId: "sess_12345abc")
→ actionLogPath: "C:\Users\...\selenium-actions-12345abc-2024-01-15.json"
```

### Step 5: Generate WebEngine Artifacts

From the action log, extract:

**1. ElementTag → WebElementDescription Mapping:**

| Captured HTML Tag | WebElementDescription |
|---|---|
| `<input id="email" name="email" type="text">` | `Id = "email"` |
| `<button data-testid="submit" class="btn">` | `Attributes = new HtmlAttribute[] { new HtmlAttribute("data-testid", "submit") }` |
| `<a href="/">Home</a>` | `LinkText = "Home"` |
| `<div aria-label="Menu">` | `Attributes = new HtmlAttribute[] { new HtmlAttribute("aria-label", "Menu") }` |

**2. Generate PageModel:**

```csharp
using AxaFrance.WebEngine.Web;

namespace MyProject.PageModels
{
    public class LoginPage : PageModel
    {
        // Source: <input id="email" name="email" type="text">
        public WebElementDescription Email { get; set; } = new WebElementDescription
        {
            Id = "email"
        };

        // Source: <input id="password" name="password" type="password">
        public WebElementDescription Password { get; set; } = new WebElementDescription
        {
            Id = "password"
        };

        // Source: <button id="submitBtn" data-testid="login-submit">Sign in</button>
        public WebElementDescription ButtonSubmit { get; set; } = new WebElementDescription
        {
            Id = "submitBtn"
        };

        public LoginPage(WebDriver driver) : base(driver) { }
    }
}
```

**3. Generate Test Script (Linear Scripting):**

```csharp
using AxaFrance.WebEngine.Web;
using MyProject.PageModels;

namespace MyProject.Tests
{
    [TestClass]
    public class LoginTests
    {
        private WebDriver _driver;

        [TestInitialize]
        public void Setup()
        {
            _driver = BrowserFactory.GetDriver(Platform.Windows, BrowserType.Chrome);
        }

        [TestMethod]
        public void TestSuccessfulLogin()
        {
            _driver.Navigate().GoToUrl("https://example.com/login");

            var page = new LoginPage(_driver);
            page.Email.SetValue("test@example.com");
            page.Password.SetValue("myPassword");
            page.ButtonSubmit.Click();

            // Verify successful login
            Assert.IsTrue(_driver.Url.Contains("/dashboard"));
        }

        [TestCleanup]
        public void Cleanup()
        {
            _driver?.Quit();
        }
    }
}
```

## Locator Strategy (Priority Order)

When generating WebElementDescription from captured HTML tags, follow this priority:

| Priority | Locator Type | Example from HTML | Usage |
|---|---|---|---|
| 1 | `Id` | `id="submitBtn"` | Unique, stable |
| 2 | `Name` | `name="email"` | Form fields, stable |
| 3 | Test Attribute | `data-testid="login"` | Added specifically for testing |
| 4 | Aria Attribute | `aria-label="Menu"` | Accessibility-based |
| 5 | TagName + InnerText | `<button>Continue</button>` | Unique text, buttons/links |
| 6 | TagName + Id/Name | Combination of stable attributes | Last stable option |
| 7 | LinkText | For `<a>` tags only | Limited to links |
| 8 | ClassName | `class="btn-primary"` | Only if proven stable |
| 9 | CssSelector / XPath | Last resort | Fragile, brittle |

## Best Practices

### 1. Use Compact Views First
- Always start with **GetAccessibilitySnapshot** to minimize tokens
- Only escalate to GetActionableHtml or GetPageHtml if needed

### 2. Batch Your Actions
- Group related interactions in a single **ExecuteBulkActions** call
- Reduces round-trips and provides atomic results

### 3. Trust the HTML Tag
- Use the `elementTag` returned from actions as the source of truth
- Never guess locators; extract them from captured HTML

### 4. Re-inspect When Needed
- After major page changes (URL change, form submission)
- When an expected element is not found
- When visual context is critical for debugging

### 5. Review the Action Log
- Always examine the JSON action log for reference
- It contains both successful and failed actions with error messages
- Use it to validate that all interactions were captured correctly

## Troubleshooting

### Session Won't Start

**Problem:** StartSession returns `success: false`

**Solutions:**
- Verify the browser is installed (`chromedriver`, `geckodriver`, etc.)
- Check that no other process is using the browser
- Review browser version compatibility with WebDriver

### Element Not Found

**Problem:** ExecuteBulkActions fails to locate an element

**Solutions:**
1. Take a screenshot with **TakeScreenshot** to verify page state
2. Capture updated state with **GetAccessibilitySnapshot**
3. Verify the locator matches the current DOM structure
4. Check for dynamic IDs or classes that change on page load

### Timeout Issues

**Problem:** Actions take too long or timeout

**Solutions:**
- Increase `ImplicitWait` in `appsettings.json`
- Add explicit waits before interacting with dynamic content
- Check network connectivity and page load times
- Verify the application is responsive

## Examples

### Example 1: Login Test Generation (Selenium)

**Step 1:** Start session and navigate
```
StartSession(browserType: 'Chrome', headless: false) 
→ sessionId: "sess_login123"

NavigateTo(sessionId: "sess_login123", url: "https://app.example.com/login")
```

**Step 2:** Inspect page
```
GetAccessibilitySnapshot(sessionId: "sess_login123")
→ Compact listing of email field, password field, login button, etc.
```

**Step 3:** Execute login actions (batch)
```
ExecuteBulkActions(sessionId: "sess_login123", actions: [
  { 
    "ActionType": "SetText", 
    "Element": { "Id": "username" }, 
    "Value": "testuser",
    "Label": "Enter username"
  },
  { 
    "ActionType": "SetText", 
    "Element": { "Id": "password" }, 
    "Value": "testpass",
    "Label": "Enter password"
  },
  { 
    "ActionType": "Click", 
    "Element": { "Id": "loginBtn" },
    "Label": "Click login"
  }
])
→ Returns ElementTag for each action (used for PageModel generation)
```

**Step 4:** Close and retrieve log
```
CloseSession(sessionId: "sess_login123")
→ Path to JSON action log with all element tags and locators
```

**Result:** Generated PageModel and test script ready to use.

### Example 2: Form Filling with Accessibility (Selenium)

**Capture accessibility snapshot:**
```
GetAccessibilitySnapshot(sessionId: "sess_form456")
```

**Returns:**
```
[input] id="name" aria-label="Your name" required
[input] id="email" aria-label="Email address" required  
[select] id="country" aria-label="Country"
[textarea] id="message" aria-label="Your message"
[button] id="submitBtn" aria-label="Send message"
```

**Execute all interactions (batch):**
```
ExecuteBulkActions(sessionId: "sess_form456", actions: [
  { "ActionType": "SetText", "Element": { "Id": "name" }, "Value": "John Doe" },
  { "ActionType": "SetText", "Element": { "Id": "email" }, "Value": "john@example.com" },
  { "ActionType": "SelectByText", "Element": { "Id": "country" }, "Value": "France" },
  { "ActionType": "SetText", "Element": { "Id": "message" }, "Value": "Hello, this is a test." },
  { "ActionType": "Click", "Element": { "Id": "submitBtn" } }
])
```

### Example 3: Mobile App Interaction (Appium)

**Step 1:** Start session and inspect
```
StartSession(platform: 'Android', appPath: '/path/to/app.apk', deviceName: 'emulator-5554')
→ sessionId: "sess_app123"

GetAccessibilitySnapshot(sessionId: "sess_app123")
→ One-line-per-element format with accId, id, text, bounds
```

**Step 2:** Execute mobile actions (batch)
```
ExecuteBulkActions(sessionId: "sess_app123", actions: [
  { "ActionType": "Tap", "Element": { "AccessibilityId": "email_field" }, "Label": "Tap email" },
  { "ActionType": "TypeText", "Element": { "AccessibilityId": "email_field" }, "Value": "user@example.com" },
  { "ActionType": "TypeText", "Element": { "AccessibilityId": "password_field" }, "Value": "secret" },
  { "ActionType": "Tap", "Element": { "AccessibilityId": "login_button" } }
])
```

**Step 3:** Close
```
CloseSession(sessionId: "sess_app123")
→ Path to action log with element tags
```

## Support & Contributing

For issues, suggestions, or contributions:
- GitHub Issues: https://github.com/AxaFrance/webengine-dotnet/issues
- Documentation: https://axafrance.github.io/webengine-dotnet/
- Main Repository: https://github.com/AxaFrance/webengine-dotnet
