using System.ComponentModel;

namespace AxaFrance.WebEngine.Mcp.Tools.Selenium.Models;

/// <summary>
/// Describes a web element using one or more identifying attributes.
/// Combine attributes to narrow the match. Prefer semantic attributes
/// (Id, InnerText, AriaLabel, Name) over CssSelector and XPath.
/// </summary>
public sealed class ElementDescriptor
{
    [Description("HTML id attribute (without the # prefix)")]
    public string? Id { get; set; }

    [Description("HTML name attribute value")]
    public string? Name { get; set; }

    [Description("HTML tag name, e.g. button, input, a, div")]
    public string? TagName { get; set; }

    [Description("Exact visible text content of the element")]
    public string? InnerText { get; set; }

    [Description("Text of a hyperlink (<a> element)")]
    public string? LinkText { get; set; }

    [Description("composite CSS class name, like in html")]
    public string? ClassName { get; set; }

    [Description("ARIA label (aria-label attribute) for accessibility-based identification")]
    public string? AriaLabel { get; set; }

    [Description("CSS selector � use only when semantic attributes above are insufficient")]
    public string? CssSelector { get; set; }

    [Description("XPath expression � use only when semantic attributes above are insufficient")]
    public string? XPath { get; set; }

    [Description("""
        Snapshot ref number (ref=N) emitted by GetAccessibilitySnapshot.
        ALWAYS prefer this over id/name/aria-label � it maps directly to the DOM node via
        data-mcp-ref=N with zero guessing (Playwright-style back-reference).
        STALENESS: refs are valid only for the DOM state at snapshot time. React/Vue/Angular
        wipe data-mcp-ref attributes on re-render. After any action that mutates the page,
        call GetAccessibilitySnapshot again before using a new ref.
        """)]
    public string? Ref { get; set; }

    [Description("0-based index when multiple elements match all other criteria (default: 0)")]
    public int Index { get; set; }
}
