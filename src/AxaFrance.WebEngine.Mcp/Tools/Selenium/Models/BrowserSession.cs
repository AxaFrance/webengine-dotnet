namespace AxaFrance.WebEngine.Mcp.Tools.Selenium.Models;

public sealed class BrowserSession
{
    public string SessionId { get; set; } = string.Empty;
    public string BrowserType { get; set; } = string.Empty;
    public bool IsHeadless { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastActivityAt { get; set; }
    public string CurrentUrl { get; set; } = string.Empty;
}
