using System.ComponentModel;

namespace AxaFrance.WebEngine.Mcp.Tools.Selenium.Models;

/// <summary>
/// Describes a single action in a bulk operation sequence.
/// ActionType must be one of: Click, TypeText, SetText, Clear, SelectByText, SelectByValue, Check, Uncheck.
/// </summary>
public sealed class BulkAction
{
    [Description("""
        The type of action to perform. Must be one of:
        - Click         : Clicks the element
        - TypeText      : Types text (with optional clear first, defaults true)
        - SetText       : Clears then sets text in field
        - Clear         : Clears an input field
        - SelectByText  : Selects <select> option by visible text
        - SelectByValue : Selects <select> option by value attribute
        - Check         : Checks a checkbox or selects a radio button
        - Uncheck       : Unchecks a checkbox
        """)]
    public string ActionType { get; set; } = string.Empty;

    [Description("The element to act on, identified from actual DOM inspection. Never guessed.")]
    public ElementDescriptor Element { get; set; } = new();

    [Description("Value used by TypeText, SetText, SelectByText, SelectByValue actions. Not needed for Click, Clear, Check, Uncheck.")]
    public string? Value { get; set; }

    [Description("For TypeText only: whether to clear the field before typing (default: true).")]
    public bool ClearFirst { get; set; } = true;

    [Description("Optional label to identify this action in the result log (e.g. 'Fill email', 'Select country').")]
    public string? Label { get; set; }
}
