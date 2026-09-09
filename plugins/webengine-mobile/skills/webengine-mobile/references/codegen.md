# Codegen from ElementTag (mobile)

## WebEngine MobileApp C#

```csharp
using AxaFrance.WebEngine.MobileApp;
namespace MyProject.PageModels {
  public class LoginPage : PageModel {
    // Source: <android.widget.EditText content-desc="email_field" resource-id="com.app:id/email_input" />
    public AppElementDescription EmailField { get; set; } = new() { AccessibilityId = "email_field" };
    // Source: <android.widget.TextView resource-id="com.app:id/error_text" />
    public AppElementDescription ErrorMessage { get; set; } = new() { Id = "error_text" };
    public LoginPage(WebDriver driver) : base(driver) { }
  }
}
// driver: AppFactory.GetDriver(Platform.Android)
```

Rules: descriptions in PageModel only; ctor takes `WebDriver`. Keyword: `SharedActionApp.DoAction(AppiumDriver)/DoCheckpoint(AppiumDriver)`.

## Raw Appium (Java/Python)

`AccessibilityId`→`AppiumBy.accessibilityId`, `Id`→`AppiumBy.id("email")`, Android→`AppiumBy.androidUIAutomator(...)`, iOS→`AppiumBy.iOSClassChain(...)`, `Text`→XPath last resort.
