using System.ComponentModel;

namespace AxaFrance.WebEngine.Mcp.Tools.Appium.Models;

/// <summary>
/// Identifies a mobile UI element using one or more locator strategies.
/// Combine attributes to narrow the match.
/// Priority (highest first): IosClassChain / UIAutomatorSelector / IosPredicate, then AccessibilityId, Id, XPath, ClassName, Text.
/// </summary>
public sealed class AppElementDescriptor
{
    [Description("Element resource-id on Android or the main ID attribute on iOS")]
    public string? Id { get; set; }

    [Description("Accessibility ID: content-desc on Android, accessibilityLabel on iOS. Preferred cross-platform locator.")]
    public string? AccessibilityId { get; set; }

    [Description("Element name attribute")]
    public string? Name { get; set; }

    [Description("Class name, e.g. android.widget.Button or XCUIElementTypeButton")]
    public string? ClassName { get; set; }

    [Description("Exact visible text content of the element")]
    public string? Text { get; set; }

    [Description("XPath expression — avoid unless no other locator is available")]
    public string? XPath { get; set; }

    [Description("Content-desc attribute — Android only. Prefer AccessibilityId for cross-platform use.")]
    public string? ContentDescription { get; set; }

    [Description("iOS Class Chain selector — iOS only, faster than XPath. Example: **/XCUIElementTypeButton[`label == 'Login'`]")]
    public string? IosClassChain { get; set; }

    [Description("UIAutomator2 selector string — Android only. Example: new UiSelector().text(\"Login\")")]
    public string? UIAutomatorSelector { get; set; }

    [Description("iOS NSPredicate string — iOS only, more expressive than IosClassChain. Example: label == 'Login' AND type == 'XCUIElementTypeButton'")]
    public string? IosPredicate { get; set; }

    [Description("0-based index when multiple elements match all other criteria (default: 0)")]
    public int Index { get; set; }
}
