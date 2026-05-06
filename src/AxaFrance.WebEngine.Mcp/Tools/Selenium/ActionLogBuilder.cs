using AxaFrance.WebEngine.Mcp.Tools.Selenium.Models;
using OpenQA.Selenium;

namespace AxaFrance.WebEngine.Mcp.Tools.Selenium;

/// <summary>
/// Extracts stable element information from a live DOM element to populate an <see cref="ActionLogEntry"/>.
/// All work is done on the MCP/server side � no LLM involvement.
/// </summary>
internal static class ActionLogBuilder
{
    /// <summary>
    /// Builds an <see cref="ActionLogEntry"/> from the resolved DOM element.
    /// Call this AFTER the action succeeds; do NOT call on failure.
    /// </summary>
    internal static ActionLogEntry Build(
        IWebDriver driver,
        string actionType,
        IWebElement element,
        string? value,
        string? label,
        string url)
    {
        return new ActionLogEntry
        {
            Timestamp = DateTime.UtcNow,
            Url = url,
            ActionType = actionType,
            Value = value,
            Label = label,
            ElementTag = GetOpeningTag(driver, element),
            StableLocator = ExtractStableLocator(element)
        };
    }

    /// <summary>
    /// Returns the opening HTML tag of the element (e.g. &lt;input id="q" type="text"&gt;)
    /// by extracting outerHTML via JavaScript and truncating at the first closing bracket.
    /// Falls back to a bare tag if JS fails.
    /// </summary>
    private static string GetOpeningTag(IWebDriver driver, IWebElement element)
    {
        try
        {
            var outerHtml = ((IJavaScriptExecutor)driver)
                .ExecuteScript("return arguments[0].outerHTML;", element) as string
                ?? string.Empty;

            var closeIndex = outerHtml.IndexOf('>');
            return closeIndex >= 0 ? outerHtml[..(closeIndex + 1)] : outerHtml;
        }
        catch
        {
            return $"<{element.TagName}>";
        }
    }

    /// <summary>
    /// Extracts stable, reusable locator attributes from the live DOM element.
    /// Preference order: id > name > data-testid > aria-label > tagName + innerText.
    /// Ephemeral attributes (data-mcp-ref) are intentionally excluded.
    /// </summary>
    private static ElementDescriptor ExtractStableLocator(IWebElement element)
    {
        var id = NullIfEmpty(element.GetAttribute("id"));
        var name = NullIfEmpty(element.GetAttribute("name"));
        var ariaLabel = NullIfEmpty(element.GetAttribute("aria-label"));
        var innerText = NullIfEmpty(element.Text);

        var testId = NullIfEmpty(element.GetAttribute("data-testid"))
                  ?? NullIfEmpty(element.GetAttribute("data-test"))
                  ?? NullIfEmpty(element.GetAttribute("data-cy"));

        var locator = new ElementDescriptor
        {
            TagName = element.TagName,
            Id = id,
            Name = name,
            AriaLabel = ariaLabel,
            InnerText = innerText,
        };

        if (testId is not null)
            locator.CssSelector = $"[data-testid='{testId}']";

        return locator;
    }

    private static string? NullIfEmpty(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value;
}
