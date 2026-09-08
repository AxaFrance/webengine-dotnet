# Locators (mobile)

Priority: `AccessibilityId` (`content-desc`) > `Id` (short resource-id) > `UIAutomatorSelector` (Android) / `IosClassChain` / `IosPredicate` (iOS) > `Text` > `ClassName`+attribute > `XPath` (last resort).

| Captured tag | AppElementDescription |
|---|---|
| `content-desc="login_button"` | `AccessibilityId = "login_button"` |
| `resource-id="com.app:id/email"` | `Id = "email"` (strip package) |
| `text="Sign in"` + Button class | `Text="Sign in", ClassName="android.widget.Button"` |
| No stable id (Android) | `UIAutomatorSelector = "new UiSelector().text(\"Email\")"` |
| iOS label Email | ``IosClassChain = "**/XCUIElementTypeTextField[`label == 'Email'`]"`` |

Native types: EditText/TextField, Button, CheckBox, Switch, Spinner/PickerWheel, TextView/Cell. Combine attributes for uniqueness; `Index` only when unavoidable.
