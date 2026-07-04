namespace AxaFrance.WebEngine.Mcp.Configuration;

public sealed class AppiumOptions
{
    public const string SectionName = "Appium";
    public bool IsEnabled { get; set; } = true;
    /// <summary>Default Appium server URL (e.g. http://localhost:4723 or a cloud grid URL).</summary>
    public string DefaultServerUrl { get; set; } = "http://localhost:4723";
    /// <summary>Timeout in seconds to wait for the device session to be established.</summary>
    public int SessionTimeoutSeconds { get; set; } = 180;
    /// <summary>Maximum idle time in seconds before Appium closes the command channel.</summary>
    public int CommandTimeoutSeconds { get; set; } = 90;
}

