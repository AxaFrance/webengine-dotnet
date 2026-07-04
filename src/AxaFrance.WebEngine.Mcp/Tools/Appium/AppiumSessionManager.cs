using System.Collections.Concurrent;
using AxaFrance.WebEngine.Mcp.Configuration;
using AxaFrance.WebEngine.Mcp.Tools.Appium.Models;
using Microsoft.Extensions.Options;
using AppiumDriver = OpenQA.Selenium.Appium.AppiumDriver;
using AppiumDriverOptions = OpenQA.Selenium.Appium.AppiumOptions;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.iOS;

namespace AxaFrance.WebEngine.Mcp.Tools.Appium;

public sealed class AppiumSessionManager : IDisposable
{
    private sealed class SessionEntry(AppiumDriver driver, string platform, string deviceName, string appPath)
    {
        public AppiumDriver Driver { get; } = driver;
        public string Platform { get; } = platform;
        public string DeviceName { get; } = deviceName;
        public string AppPath { get; } = appPath;
        public DateTime CreatedAt { get; } = DateTime.UtcNow;
        public DateTime LastActivityAt { get; set; } = DateTime.UtcNow;
    }

    internal sealed record SourceCacheEntry(string Source, DateTime CapturedAt);

    private readonly ConcurrentDictionary<string, SessionEntry> _sessions = new();
    private readonly ConcurrentDictionary<string, SourceCacheEntry> _sourceCache = new();
    private readonly ConcurrentDictionary<string, List<string>> _actionLogs = new();
    private readonly AppiumOptions _options;
    private readonly ILogger<AppiumSessionManager> _logger;

    public AppiumSessionManager(IOptions<AppiumOptions> options, ILogger<AppiumSessionManager> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    /// <summary>Creates a new Appium device session and returns its metadata.</summary>
    public AppiumSession StartSession(
        string platform,
        string appPath,
        string deviceName,
        string? osVersion = null,
        string? serverUrl = null)
    {
        var sessionId = Guid.NewGuid().ToString("N")[..12];
        var driver = CreateDriver(platform, appPath, deviceName, osVersion, serverUrl);
        var entry = new SessionEntry(driver, platform, deviceName, appPath);
        _sessions[sessionId] = entry;

        _logger.LogInformation(
            "Started Appium session {Id} (platform={Platform}, device={Device}, app={App})",
            sessionId, platform, deviceName, appPath);

        return ToDto(sessionId, entry);
    }

    /// <summary>Retrieves the AppiumDriver for an active session, updating last-activity timestamp.</summary>
    public AppiumDriver GetDriver(string sessionId)
    {
        if (!_sessions.TryGetValue(sessionId, out var entry))
            throw new InvalidOperationException($"Appium session '{sessionId}' not found or has been closed.");

        entry.LastActivityAt = DateTime.UtcNow;
        return entry.Driver;
    }

    /// <summary>Returns the platform (Android / iOS) of the session.</summary>
    public string GetPlatform(string sessionId)
    {
        if (!_sessions.TryGetValue(sessionId, out var entry))
            throw new InvalidOperationException($"Appium session '{sessionId}' not found or has been closed.");
        return entry.Platform;
    }

    /// <summary>Lists all currently active sessions.</summary>
    public List<AppiumSession> GetActiveSessions()
        => _sessions.Select(kvp => ToDto(kvp.Key, kvp.Value)).ToList();

    /// <summary>Caches the page source for a session so GetPageSourceChunk can serve slices without recomputing.</summary>
    internal void SetSourceCache(string sessionId, string source)
        => _sourceCache[sessionId] = new SourceCacheEntry(source, DateTime.UtcNow);

    /// <summary>Returns the cached source entry for the session, or null if not cached.</summary>
    internal SourceCacheEntry? GetSourceCache(string sessionId)
        => _sourceCache.TryGetValue(sessionId, out var entry) ? entry : null;

    /// <summary>Appends a completed action label to the session log.</summary>
    internal void AppendActionLog(string sessionId, string label)
    {
        var list = _actionLogs.GetOrAdd(sessionId, _ => []);
        lock (list) list.Add($"{DateTime.UtcNow:HH:mm:ss} {label}");
    }

    /// <summary>Returns a read-only snapshot of the action log for the session.</summary>
    internal IReadOnlyList<string> GetActionLog(string sessionId)
        => _actionLogs.TryGetValue(sessionId, out var list) ? list.AsReadOnly() : [];

    /// <summary>Closes a session, quitting the driver and freeing resources.</summary>
    public bool CloseSession(string sessionId)
    {
        if (!_sessions.TryRemove(sessionId, out var entry))
            return false;

        _sourceCache.TryRemove(sessionId, out _);
        _actionLogs.TryRemove(sessionId, out _);

        try { entry.Driver.Quit(); }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error quitting Appium driver for session {Id}", sessionId);
        }

        entry.Driver.Dispose();
        _logger.LogInformation("Closed Appium session {Id}", sessionId);
        return true;
    }

    private AppiumDriver CreateDriver(
        string platform,
        string appPath,
        string deviceName,
        string? osVersion,
        string? serverUrl)
    {
        var serverAddress = serverUrl ?? _options.DefaultServerUrl;
        var opts = new AppiumDriverOptions
        {
            DeviceName = deviceName,
            PlatformName = platform,
            App = appPath,
        };

        opts.AddAdditionalAppiumOption("newCommandTimeout", _options.CommandTimeoutSeconds);
        opts.AddAdditionalAppiumOption("nativeWebScreenshot", true);

        if (!string.IsNullOrEmpty(osVersion))
            opts.PlatformVersion = osVersion;

        var timeout = TimeSpan.FromSeconds(_options.SessionTimeoutSeconds);

        if (platform.Equals("Android", StringComparison.OrdinalIgnoreCase))
        {
            opts.AutomationName = "UiAutomator2";
            return new AndroidDriver(new Uri(serverAddress), opts, timeout);
        }

        if (platform.Equals("iOS", StringComparison.OrdinalIgnoreCase))
        {
            opts.AutomationName = "XCUITest";
            opts.AddAdditionalAppiumOption("includeSafariInWebviews", true);
            opts.AddAdditionalAppiumOption("connectHardwareKeyboard", true);
            return new IOSDriver(new Uri(serverAddress), opts, timeout);
        }

        throw new ArgumentException(
            $"Unsupported platform '{platform}'. Use 'Android' or 'iOS'.");
    }

    private static AppiumSession ToDto(string sessionId, SessionEntry entry) => new()
    {
        SessionId = sessionId,
        Platform = entry.Platform,
        DeviceName = entry.DeviceName,
        AppPath = entry.AppPath,
        AppiumSessionId = SafeGetSessionId(entry.Driver),
        CreatedAt = entry.CreatedAt,
        LastActivityAt = entry.LastActivityAt,
    };

    private static string? SafeGetSessionId(AppiumDriver driver)
    {
        try { return driver.SessionId?.ToString(); }
        catch { return null; }
    }

    public void Dispose()
    {
        foreach (var id in _sessions.Keys.ToList())
            CloseSession(id);
    }
}
