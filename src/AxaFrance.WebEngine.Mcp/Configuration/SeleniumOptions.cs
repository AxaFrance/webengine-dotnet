namespace AxaFrance.WebEngine.Mcp.Configuration;

public sealed class SeleniumOptions
{
    public const string SectionName = "Selenium";
    public bool IsEnabled { get; set; } = true;
    public string DefaultBrowser { get; set; } = "Chrome";
    public bool Headless { get; set; } = true;
    public int SessionTimeoutMinutes { get; set; } = 30;
    public int PageLoadTimeoutSeconds { get; set; } = 30;
}

