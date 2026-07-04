namespace AxaFrance.WebEngine.Mcp.Tools.Appium.Models;

/// <summary>Metadata about an active Appium device session.</summary>
public sealed class AppiumSession
{
    /// <summary>MCP-assigned session identifier to pass to all subsequent tool calls.</summary>
    public string SessionId { get; set; } = string.Empty;

    /// <summary>Mobile platform: Android or iOS.</summary>
    public string Platform { get; set; } = string.Empty;

    /// <summary>Target device name or UDID.</summary>
    public string DeviceName { get; set; } = string.Empty;

    /// <summary>Path or URL of the application under test.</summary>
    public string AppPath { get; set; } = string.Empty;

    /// <summary>Appium server session ID assigned by the remote server.</summary>
    public string? AppiumSessionId { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime LastActivityAt { get; set; }
}
