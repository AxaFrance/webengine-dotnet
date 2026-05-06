namespace AxaFrance.WebEngine.Mcp.Tools.Selenium.Models;

/// <summary>
/// Records a single successfully executed browser action, capturing the real DOM element
/// attributes at the moment of interaction. Used to generate test automation code.
/// </summary>
public sealed class ActionLogEntry
{
    /// <summary>1-based sequential step number within the session.</summary>
    public int StepNumber { get; set; }

    /// <summary>UTC timestamp of the action.</summary>
    public DateTime Timestamp { get; set; }

    /// <summary>Page URL at the time the action was performed.</summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>Type of action: Click, TypeText, SetText, Clear, SelectByText, SelectByValue, Check, Uncheck.</summary>
    public string ActionType { get; set; } = string.Empty;

    /// <summary>Value used by the action (text typed, option selected, etc.). Null for Click/Check/Uncheck.</summary>
    public string? Value { get; set; }

    /// <summary>Optional label supplied by the caller to describe the action intent.</summary>
    public string? Label { get; set; }

    /// <summary>
    /// The opening HTML tag of the element as it appeared in the live DOM at action time,
    /// e.g. &lt;input id="email" name="email" type="text" class="form-control"&gt;
    /// </summary>
    public string ElementTag { get; set; } = string.Empty;

    /// <summary>
    /// Stable locators extracted from the live DOM element (no ephemeral ref numbers).
    /// Suitable for use in generated automation code.
    /// </summary>
    public ElementDescriptor StableLocator { get; set; } = new();
}
