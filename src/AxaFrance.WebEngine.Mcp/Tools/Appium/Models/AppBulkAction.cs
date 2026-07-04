using System.ComponentModel;

namespace AxaFrance.WebEngine.Mcp.Tools.Appium.Models;

/// <summary>
/// Describes a single action in a mobile bulk operation sequence.
/// ActionType must be one of: Tap, TypeText, SetText, Clear, LongPress, SwipeUp, SwipeDown, SwipeLeft, SwipeRight.
/// </summary>
public sealed class AppBulkAction
{
    [Description("""
        The type of action to perform. Must be one of:
        - Tap          : Taps (clicks) the element
        - TypeText     : Types text into a field (optional clear first, defaults true)
        - SetText      : Clears then sets text in a field
        - Clear        : Clears an input field
        - LongPress    : Long-presses the element (duration via DurationMs, default 1500 ms)
        - SwipeUp      : Swipes up on screen (scroll content up / reveal content below)
        - SwipeDown    : Swipes down on screen (scroll content down / reveal content above)
        - SwipeLeft    : Swipes left on screen
        - SwipeRight   : Swipes right on screen
        """)]
    public string ActionType { get; set; } = string.Empty;

    [Description("The element to act on, identified from the accessibility snapshot or page source. Not required for Swipe* actions.")]
    public AppElementDescriptor Element { get; set; } = new();

    [Description("Value used by TypeText and SetText actions. Not needed for Tap, Clear, LongPress, Swipe.")]
    public string? Value { get; set; }

    [Description("For TypeText only: whether to clear the field before typing (default: true).")]
    public bool ClearFirst { get; set; } = true;

    [Description("For LongPress: duration in milliseconds (default: 1500).")]
    public int DurationMs { get; set; } = 1500;

    [Description("Optional label to identify this action in the result log (e.g. 'Tap login', 'Enter email').")]
    public string? Label { get; set; }
}
