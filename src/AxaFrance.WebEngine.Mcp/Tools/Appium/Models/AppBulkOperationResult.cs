namespace AxaFrance.WebEngine.Mcp.Tools.Appium.Models;

/// <summary>Result for a single action inside a mobile bulk operation.</summary>
public sealed class AppBulkActionResult
{
    /// <summary>Zero-based position in the submitted action list.</summary>
    public int Index { get; set; }

    /// <summary>The ActionType that was executed (e.g. "Tap", "TypeText").</summary>
    public string ActionType { get; set; } = string.Empty;

    /// <summary>Optional label supplied by the caller for easy identification.</summary>
    public string? Label { get; set; }

    /// <summary>"Success" or "Failed".</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>Error message when Status is "Failed"; null on success.</summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// XML-like opening tag of the element as it appeared in the live UI hierarchy at action time,
    /// e.g. &lt;android.widget.EditText resource-id="email" content-desc="Email" /&gt;. Null on failure or swipe actions.
    /// </summary>
    public string? ElementTag { get; set; }
}

/// <summary>Aggregate result returned after executing a mobile bulk action sequence.</summary>
public sealed class AppBulkOperationResult
{
    /// <summary>Per-action results in execution order.</summary>
    public List<AppBulkActionResult> Results { get; set; } = [];

    /// <summary>True when every action succeeded.</summary>
    public bool AllSucceeded { get; set; }

    /// <summary>Number of actions that succeeded.</summary>
    public int SucceededCount { get; set; }

    /// <summary>Number of actions that failed.</summary>
    public int FailedCount { get; set; }

    /// <summary>Page source size in KB after the last executed action.</summary>
    public double SourceSizeKB { get; set; }

    /// <summary>
    /// Hint for the LLM: which inspection method to prefer next.
    /// "GetAccessibilitySnapshot" when SourceSizeKB &lt; 60, otherwise "GetPageSourceChunk".
    /// </summary>
    public string RecommendedInspectionMethod { get; set; } = string.Empty;
}
