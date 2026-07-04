namespace AxaFrance.WebEngine.Mcp.Tools.Selenium.Models;

/// <summary>Result for a single action inside a bulk operation.</summary>
public sealed class BulkActionResult
{
    /// <summary>Zero-based position in the submitted action list.</summary>
    public int Index { get; set; }

    /// <summary>The ActionType that was executed (e.g. "Click", "TypeText").</summary>
    public string ActionType { get; set; } = string.Empty;

    /// <summary>Optional label supplied by the caller for easy identification.</summary>
    public string? Label { get; set; }

    /// <summary>"Success" or "Failed".</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>Error message when Status is "Failed"; null on success.</summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// The opening HTML tag of the element as it appeared in the live DOM at action time,
    /// e.g. &lt;input id="email" name="email" type="text"&gt;. Null on failure or for non-element actions.
    /// </summary>
    public string? ElementTag { get; set; }
}

/// <summary>Aggregate result returned after executing a bulk action sequence.</summary>
public sealed class BulkOperationResult
{
    /// <summary>Per-action results in execution order.</summary>
    public List<BulkActionResult> Results { get; set; } = [];

    /// <summary>True when every action succeeded.</summary>
    public bool AllSucceeded { get; set; }

    /// <summary>Number of actions that succeeded.</summary>
    public int SucceededCount { get; set; }

    /// <summary>Number of actions that failed.</summary>
    public int FailedCount { get; set; }

    /// <summary>Final URL after the last executed action.</summary>
    public string FinalUrl { get; set; } = string.Empty;

    /// <summary>Cleaned HTML size in KB after the last executed action.</summary>
    public double HtmlSizeKB { get; set; }

    /// <summary>Accessibility snapshot size in KB after the last executed action.</summary>
    public double SnapshotSizeKB { get; set; }

    /// <summary>
    /// Hint for the LLM: which inspection method to prefer next.
    /// "GetPageHtml" when HtmlSizeKB &lt; 30, otherwise "GetAccessibilitySnapshot".
    /// </summary>
    public string RecommendedInspectionMethod { get; set; } = string.Empty;
}
