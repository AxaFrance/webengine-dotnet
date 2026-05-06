using System.Collections.Concurrent;
using AxaFrance.WebEngine.Mcp.Configuration;
using AxaFrance.WebEngine.Mcp.Tools.Selenium.Models;
using Microsoft.Extensions.Options;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;

namespace AxaFrance.WebEngine.Mcp.Tools.Selenium;

public sealed class SeleniumSessionManager : IDisposable
{
    private sealed class SessionEntry(IWebDriver driver, string browserType, bool isHeadless)
    {
        public IWebDriver Driver { get; } = driver;
        public string BrowserType { get; } = browserType;
        public bool IsHeadless { get; } = isHeadless;
        public DateTime CreatedAt { get; } = DateTime.UtcNow;
        public DateTime LastActivityAt { get; set; } = DateTime.UtcNow;
    }

    internal sealed record HtmlCacheEntry(string Html, string Url, DateTime CapturedAt);

    private readonly ConcurrentDictionary<string, SessionEntry> _sessions = new();
    private readonly ConcurrentDictionary<string, HtmlCacheEntry> _htmlCache = new();
    private readonly ConcurrentDictionary<string, List<ActionLogEntry>> _actionLogs = new();
    private readonly SeleniumOptions _options;
    private readonly ILogger<SeleniumSessionManager> _logger;

    public SeleniumSessionManager(IOptions<SeleniumOptions> options, ILogger<SeleniumSessionManager> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    /// <summary>Creates a new browser session and returns its metadata.</summary>
    public BrowserSession StartSession(string browserType = "Chrome", bool? headless = null)
    {
        var sessionId = Guid.NewGuid().ToString("N")[..12];
        var isHeadless = headless ?? _options.Headless;
        var driver = CreateDriver(browserType, isHeadless);

        driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(_options.PageLoadTimeoutSeconds);

        var entry = new SessionEntry(driver, browserType, isHeadless);
        _sessions[sessionId] = entry;

        _logger.LogInformation("Started session {Id} ({Browser}, headless={Headless})", sessionId, browserType, isHeadless);

        return ToDto(sessionId, entry);
    }

    /// <summary>Retrieves the WebDriver for an active session, updating last-activity timestamp.</summary>
    public IWebDriver GetDriver(string sessionId)
    {
        if (!_sessions.TryGetValue(sessionId, out var entry))
            throw new InvalidOperationException($"Browser session '{sessionId}' not found or has expired.");

        entry.LastActivityAt = DateTime.UtcNow;
        return entry.Driver;
    }

    /// <summary>Lists all currently active sessions.</summary>
    public List<BrowserSession> GetActiveSessions()
        => _sessions.Select(kvp => ToDto(kvp.Key, kvp.Value)).ToList();

    /// <summary>Caches the cleaned HTML for a session so GetPageHtmlChunk can serve slices without recomputing.</summary>
    internal void SetHtmlCache(string sessionId, string html, string url)
        => _htmlCache[sessionId] = new HtmlCacheEntry(html, url, DateTime.UtcNow);

    /// <summary>Returns the cached HTML entry for the session, or null if not cached.</summary>
    internal HtmlCacheEntry? GetHtmlCache(string sessionId)
        => _htmlCache.TryGetValue(sessionId, out var entry) ? entry : null;

    /// <summary>
    /// Appends a successfully executed action entry to the session log.
    /// StepNumber is assigned automatically (1-based, incremented per session).
    /// </summary>
    internal void AppendActionLog(string sessionId, ActionLogEntry entry)
    {
        var list = _actionLogs.GetOrAdd(sessionId, _ => []);
        lock (list)
        {
            entry.StepNumber = list.Count + 1;
            list.Add(entry);
        }
    }

    /// <summary>Returns a read-only snapshot of the action log for the session.</summary>
    internal IReadOnlyList<ActionLogEntry> GetActionLog(string sessionId)
        => _actionLogs.TryGetValue(sessionId, out var list) ? list.AsReadOnly() : [];

    /// <summary>Closes a session, quitting the browser and freeing resources.</summary>
    public bool CloseSession(string sessionId)
    {
        if (!_sessions.TryRemove(sessionId, out var entry))
            return false;

        _htmlCache.TryRemove(sessionId, out _);
        _actionLogs.TryRemove(sessionId, out _);

        try { entry.Driver.Quit(); }
        catch (Exception ex) { _logger.LogWarning(ex, "Error quitting driver for session {Id}", sessionId); }

        entry.Driver.Dispose();
        _logger.LogInformation("Closed session {Id}", sessionId);
        return true;
    }

    private IWebDriver CreateDriver(string browserType, bool headless) =>
        browserType.Trim().ToUpperInvariant() switch
        {
            "FIREFOX" => CreateFirefoxDriver(headless),
            "EDGE" => CreateEdgeDriver(headless),
            _ => CreateChromeDriver(headless)
        };

    private static ChromeDriver CreateChromeDriver(bool headless)
    {
        var opts = new ChromeOptions();
        if (headless)
            opts.AddArguments("--headless=new", "--no-sandbox", "--disable-dev-shm-usage");
        return new ChromeDriver(opts);
    }

    private static FirefoxDriver CreateFirefoxDriver(bool headless)
    {
        var opts = new FirefoxOptions();
        if (headless) opts.AddArgument("--headless");
        return new FirefoxDriver(opts);
    }

    private static EdgeDriver CreateEdgeDriver(bool headless)
    {
        var opts = new EdgeOptions();
        if (headless) opts.AddArguments("--headless=new");
        return new EdgeDriver(opts);
    }

    private static BrowserSession ToDto(string sessionId, SessionEntry entry) => new()
    {
        SessionId = sessionId,
        BrowserType = entry.BrowserType,
        IsHeadless = entry.IsHeadless,
        CreatedAt = entry.CreatedAt,
        LastActivityAt = entry.LastActivityAt,
        CurrentUrl = SafeGetUrl(entry.Driver)
    };

    private static string SafeGetUrl(IWebDriver driver)
    {
        try { return driver.Url; }
        catch { return string.Empty; }
    }

    public void Dispose()
    {
        foreach (var id in _sessions.Keys.ToList())
            CloseSession(id);
    }
}
