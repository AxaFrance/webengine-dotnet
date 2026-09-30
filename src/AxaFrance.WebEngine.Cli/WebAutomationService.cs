using System.Collections.Concurrent;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;

namespace AxaFrance.WebEngine.Cli;

internal sealed class WebAutomationService : IDisposable
{
    private sealed class SessionEntry(
        IWebDriver driver,
        string browserType,
        bool isHeadless)
    {
        public IWebDriver Driver { get; } = driver;
        public string BrowserType { get; } = browserType;
        public bool IsHeadless { get; } = isHeadless;
        public DateTimeOffset CreatedAtUtc { get; } = DateTimeOffset.UtcNow;
        public DateTimeOffset LastActivityAtUtc { get; set; } = DateTimeOffset.UtcNow;
        public object SyncRoot { get; } = new();
        public Dictionary<string, IWebElement> InspectionReferences { get; } = new(StringComparer.Ordinal);
        public List<WebActionRecord> Actions { get; } = [];
    }

    private readonly ConcurrentDictionary<string, SessionEntry> _sessions = new();

    public WebSessionInfo OpenSession(SessionOpenArguments arguments)
    {
        var normalizedBrowser = NormalizeBrowser(arguments.BrowserType);
        var driver = CreateDriver(normalizedBrowser, arguments.Headless);

        try
        {
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(45);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.Zero;

            var sessionId = Guid.NewGuid().ToString("N")[..12];
            var entry = new SessionEntry(driver, normalizedBrowser, arguments.Headless);
            _sessions[sessionId] = entry;
            return ToSessionInfo(sessionId, entry);
        }
        catch
        {
            driver.Quit();
            driver.Dispose();
            throw;
        }
    }

    public IReadOnlyList<WebSessionInfo> ListSessions()
        => _sessions
            .Select(pair => ToSessionInfo(pair.Key, pair.Value))
            .OrderBy(session => session.CreatedAtUtc)
            .ToArray();

    public WebCloseResult CloseSession(string sessionId)
    {
        if (!_sessions.TryRemove(sessionId, out var entry))
            return new WebCloseResult(sessionId, false);

        lock (entry.SyncRoot)
        {
            entry.InspectionReferences.Clear();
            try
            {
                entry.Driver.Quit();
            }
            finally
            {
                entry.Driver.Dispose();
            }
        }

        return new WebCloseResult(sessionId, true);
    }

    public WebNavigationResult Navigate(NavigateArguments arguments)
    {
        var uri = ValidateHttpUrl(arguments.Url);
        return WithSession(arguments.SessionId, entry =>
        {
            entry.Driver.Navigate().GoToUrl(uri);
            WaitForDocumentReady(entry.Driver);
            InvalidateInspection(entry);
            return new WebNavigationResult(
                arguments.SessionId,
                entry.Driver.Url,
                entry.Driver.Title);
        });
    }

    public WebInspection Inspect(InspectArguments arguments)
    {
        var mode = arguments.Mode.Trim().ToLowerInvariant();
        if (mode is not ("actionable" or "snapshot"))
            throw new ArgumentException("Inspection mode must be 'actionable' or 'snapshot'.");

        var limit = Math.Clamp(arguments.Limit, 1, 500);
        return WithSession(arguments.SessionId, entry =>
        {
            InvalidateInspection(entry);
            var elements = mode == "snapshot"
                ? CaptureElements(entry, limit)
                : CaptureElements(entry, limit);

            return new WebInspection(
                arguments.SessionId,
                entry.Driver.Url,
                entry.Driver.Title,
                elements);
        });
    }

    public WebPageSource GetHtml(string sessionId)
        => WithSession(sessionId, entry =>
        {
            var html = entry.Driver.PageSource;
            const int maxLength = 200_000;
            var truncated = html.Length > maxLength;
            return new WebPageSource(
                sessionId,
                entry.Driver.Url,
                entry.Driver.Title,
                truncated ? html[..maxLength] : html,
                truncated);
        });

    public WebActionResult Click(ElementActionArguments arguments)
        => WithElement(arguments.SessionId, arguments.Reference, arguments.Selector, (entry, element, info) =>
        {
            element.Click();
            return RecordAction(entry, "click", element, info, null);
        });

    public WebActionResult Type(TypeArguments arguments)
    {
        if (arguments.Text is null)
            throw new ArgumentException("The text value is required.");

        return WithElement(arguments.SessionId, arguments.Reference, arguments.Selector, (entry, element, info) =>
        {
            if (arguments.ClearFirst)
                element.Clear();

            element.SendKeys(arguments.Text);
            return RecordAction(entry, "type", element, info, IsSecretField(element) ? "[redacted]" : arguments.Text);
        });
    }

    public WebActionResult SendKey(KeyArguments arguments)
    {
        var key = ResolveKey(arguments.Key);
        return WithElement(arguments.SessionId, arguments.Reference, arguments.Selector, (entry, element, info) =>
        {
            element.SendKeys(key);
            return RecordAction(entry, "key", element, info, arguments.Key!.Trim());
        });
    }

    public WebActionResult Select(SelectArguments arguments)
    {
        if (string.IsNullOrWhiteSpace(arguments.Text) && string.IsNullOrWhiteSpace(arguments.Value))
            throw new ArgumentException("Use either a visible option text or an option value.");

        return WithElement(arguments.SessionId, arguments.Reference, arguments.Selector, (entry, element, info) =>
        {
            var select = new SelectElement(element);
            if (!string.IsNullOrWhiteSpace(arguments.Text))
                select.SelectByText(arguments.Text);
            else
                select.SelectByValue(arguments.Value!);

            return RecordAction(
                entry,
                "select",
                element,
                info,
                arguments.Text ?? arguments.Value);
        });
    }

    public IReadOnlyList<WebActionRecord> GetActions(string sessionId)
        => WithSession(sessionId, entry =>
        {
            return (IReadOnlyList<WebActionRecord>)entry.Actions.ToArray();
        });

    public void Dispose()
    {
        foreach (var sessionId in _sessions.Keys.ToArray())
            CloseSession(sessionId);
    }

    private WebActionResult WithElement(
        string sessionId,
        string? reference,
        string? selector,
        Func<SessionEntry, IWebElement, WebElementInfo, WebActionResult> action)
        => WithSession(sessionId, entry =>
        {
            try
            {
                var element = ResolveElement(entry, reference, selector);
                var info = DescribeElement(element, reference);
                return action(entry, element, info);
            }
            finally
            {
                InvalidateInspection(entry);
            }
        });

    private T WithSession<T>(string sessionId, Func<SessionEntry, T> action)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
            throw new ArgumentException("The session ID is required.");

        if (!_sessions.TryGetValue(sessionId, out var entry))
            throw new WebAutomationException(
                "session_not_found",
                $"Web session '{sessionId}' was not found or has expired.");

        lock (entry.SyncRoot)
        {
            entry.LastActivityAtUtc = DateTimeOffset.UtcNow;
            return action(entry);
        }
    }

    private static IWebElement ResolveElement(
        SessionEntry entry,
        string? reference,
        string? selector)
    {
        if (!string.IsNullOrWhiteSpace(reference))
        {
            if (!entry.InspectionReferences.TryGetValue(reference, out var inspectedElement))
                throw new WebAutomationException(
                    "inspection_required",
                    $"Element reference '{reference}' is not available. Inspect the current page before acting.");

            return inspectedElement;
        }

        if (string.IsNullOrWhiteSpace(selector))
            throw new ArgumentException("Use either a reference from inspection or a CSS selector.");

        var matches = entry.Driver.FindElements(By.CssSelector(selector));
        if (matches.Count == 0)
            throw new WebAutomationException(
                "element_not_found",
                $"No element matched CSS selector '{selector}'.");

        return matches[0];
    }

    private static IReadOnlyList<WebElementInfo> CaptureElements(SessionEntry entry, int limit)
    {
        var candidates = entry.Driver.FindElements(By.CssSelector(
            "input, textarea, select, button, a, label, [role='button'], [role='link'], [role='radio'], [role='checkbox'], [role='option'], [contenteditable='true']"));
        var result = new List<WebElementInfo>(Math.Min(candidates.Count, limit));

        foreach (var element in candidates)
        {
            if (result.Count >= limit)
                break;

            if (!IsVisible(element))
                continue;

            var reference = $"ref={result.Count + 1}";
            entry.InspectionReferences[reference] = element;
            result.Add(DescribeElement(element, reference));
        }

        return result;
    }

    private static WebActionResult RecordAction(
        SessionEntry entry,
        string action,
        IWebElement element,
        WebElementInfo info,
        string? value)
    {
        var record = new WebActionRecord(
            entry.Actions.Count + 1,
            DateTimeOffset.UtcNow,
            action,
            value,
            info);
        entry.Actions.Add(record);

        return new WebActionResult(
            action,
            entry.Driver.Url,
            entry.Driver.Title,
            info,
            value);
    }

    private static WebElementInfo DescribeElement(IWebElement element, string? reference)
    {
        var tagName = element.TagName;
        var id = Attribute(element, "id");
        var name = Attribute(element, "name");
        var type = Attribute(element, "type");
        var ariaLabel = Attribute(element, "aria-label");
        var role = Attribute(element, "role");
        var placeholder = Attribute(element, "placeholder");
        var testId = Attribute(element, "data-testid");
        var text = Trim(element.Text);
        var selector = BuildSelector(tagName, id, name, testId, ariaLabel);

        return new WebElementInfo(
            reference,
            tagName,
            id,
            name,
            type,
            ariaLabel,
            role,
            placeholder,
            testId,
            text,
            selector,
            IsEnabled(element),
            IsSelected(element));
    }

    private static string? BuildSelector(
        string tagName,
        string? id,
        string? name,
        string? testId,
        string? ariaLabel)
    {
        if (!string.IsNullOrWhiteSpace(id))
            return $"#{EscapeCssIdentifier(id)}";
        if (!string.IsNullOrWhiteSpace(testId))
            return $"[{EscapeCssAttribute("data-testid", testId)}]";
        if (!string.IsNullOrWhiteSpace(name))
            return $"{tagName}[{EscapeCssAttribute("name", name)}]";
        if (!string.IsNullOrWhiteSpace(ariaLabel))
            return $"{tagName}[{EscapeCssAttribute("aria-label", ariaLabel)}]";
        return tagName;
    }

    private static string EscapeCssAttribute(string name, string value)
        => $"{name}=\"{EscapeCssValue(value)}\"";

    private static string EscapeCssIdentifier(string value)
    {
        var builder = new StringBuilder(value.Length);

        for (var index = 0; index < value.Length; index++)
        {
            var character = value[index];
            if (character == '\0')
            {
                builder.Append('\uFFFD');
                continue;
            }

            var isFirst = index == 0;
            var isSecond = index == 1;
            var startsWithDash = value[0] == '-';
            if ((isFirst && char.IsDigit(character))
                || (isSecond && startsWithDash && char.IsDigit(character)))
            {
                builder.Append('\\');
                builder.Append(((int)character).ToString("X", System.Globalization.CultureInfo.InvariantCulture));
                builder.Append(' ');
                continue;
            }

            if (character < 0x20 || character == 0x7F)
            {
                builder.Append('\\');
                builder.Append(((int)character).ToString("X", System.Globalization.CultureInfo.InvariantCulture));
                builder.Append(' ');
                continue;
            }

            if (character >= 0x80
                || character == '-'
                || character == '_'
                || char.IsLetterOrDigit(character))
            {
                builder.Append(character);
                continue;
            }

            builder.Append('\\');
            builder.Append(character);
        }

        return builder.ToString();
    }

    private static string EscapeCssValue(string value)
        => value.Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("\"", "\\\"", StringComparison.Ordinal);

    private static string? Attribute(IWebElement element, string name)
        => Trim(element.GetAttribute(name));

    private static string? Trim(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static bool IsVisible(IWebElement element)
    {
        try
        {
            return element.Displayed;
        }
        catch (StaleElementReferenceException)
        {
            return false;
        }
    }

    private static bool IsEnabled(IWebElement element)
    {
        try
        {
            return element.Enabled;
        }
        catch (StaleElementReferenceException)
        {
            return false;
        }
    }

    private static bool IsSelected(IWebElement element)
    {
        try
        {
            return element.Selected;
        }
        catch (StaleElementReferenceException)
        {
            return false;
        }
    }

    private static bool IsSecretField(IWebElement element)
        => string.Equals(Attribute(element, "type"), "password", StringComparison.OrdinalIgnoreCase)
            || string.Equals(Attribute(element, "autocomplete"), "current-password", StringComparison.OrdinalIgnoreCase)
            || string.Equals(Attribute(element, "autocomplete"), "new-password", StringComparison.OrdinalIgnoreCase);

    private static string ResolveKey(string? key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("The --key value is required.");

        return key.Trim().ToLowerInvariant() switch
        {
            "enter" or "return" => Keys.Enter,
            "tab" => Keys.Tab,
            "escape" or "esc" => Keys.Escape,
            "backspace" => Keys.Backspace,
            "delete" or "del" => Keys.Delete,
            "space" => Keys.Space,
            "home" => Keys.Home,
            "end" => Keys.End,
            "pageup" or "page-up" => Keys.PageUp,
            "pagedown" or "page-down" => Keys.PageDown,
            "arrowup" or "up" => Keys.ArrowUp,
            "arrowdown" or "down" => Keys.ArrowDown,
            "arrowleft" or "left" => Keys.ArrowLeft,
            "arrowright" or "right" => Keys.ArrowRight,
            _ => throw new ArgumentException(
                "Unsupported key. Use Enter, Tab, Escape, Backspace, Delete, Space, Home, End, PageUp, PageDown, or an Arrow key.")
        };
    }

    private static void InvalidateInspection(SessionEntry entry)
        => entry.InspectionReferences.Clear();

    private static void WaitForDocumentReady(IWebDriver driver)
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
        wait.Until(current =>
        {
            if (current is not IJavaScriptExecutor scriptExecutor)
                return true;

            return string.Equals(
                scriptExecutor.ExecuteScript("return document.readyState")?.ToString(),
                "complete",
                StringComparison.OrdinalIgnoreCase);
        });
    }

    private static Uri ValidateHttpUrl(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)
            || uri.Scheme is not ("http" or "https"))
        {
            throw new ArgumentException("The URL must be an absolute HTTP or HTTPS URL.");
        }

        return uri;
    }

    private static string NormalizeBrowser(string? browser)
    {
        var normalized = string.IsNullOrWhiteSpace(browser) ? "Chrome" : browser.Trim();
        return normalized.ToLowerInvariant() switch
        {
            "chrome" => "Chrome",
            "edge" or "chromiumedge" => "Edge",
            "firefox" => "Firefox",
            _ => throw new ArgumentException("Browser must be Chrome, Edge, or Firefox.")
        };
    }

    private static IWebDriver CreateDriver(string browser, bool headless)
        => browser switch
        {
            "Chrome" => CreateChromeDriver(headless),
            "Edge" => CreateEdgeDriver(headless),
            "Firefox" => CreateFirefoxDriver(headless),
            _ => throw new ArgumentException($"Browser '{browser}' is not supported.")
        };

    private static ChromeDriver CreateChromeDriver(bool headless)
    {
        var options = new ChromeOptions
        {
            AcceptInsecureCertificates = true
        };
        options.AddArgument("--window-size=1440,1200");
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-gpu");
        if (headless)
            options.AddArgument("--headless=new");
        return new ChromeDriver(options);
    }

    private static EdgeDriver CreateEdgeDriver(bool headless)
    {
        var options = new EdgeOptions
        {
            AcceptInsecureCertificates = true
        };
        options.AddArgument("--window-size=1440,1200");
        if (headless)
            options.AddArgument("--headless=new");
        return new EdgeDriver(options);
    }

    private static FirefoxDriver CreateFirefoxDriver(bool headless)
    {
        var options = new FirefoxOptions
        {
            AcceptInsecureCertificates = true
        };
        options.AddArgument("--width=1440");
        options.AddArgument("--height=1200");
        if (headless)
            options.AddArgument("--headless");
        return new FirefoxDriver(options);
    }

    private static WebSessionInfo ToSessionInfo(string sessionId, SessionEntry entry)
        => new(
            sessionId,
            entry.BrowserType,
            entry.IsHeadless,
            entry.CreatedAtUtc,
            entry.LastActivityAtUtc,
            SafeUrl(entry.Driver));

    private static string SafeUrl(IWebDriver driver)
    {
        try
        {
            return driver.Url;
        }
        catch (WebDriverException)
        {
            return string.Empty;
        }
    }
}

internal sealed class WebAutomationException(string code, string message) : Exception(message)
{
    public string Code { get; } = code;
}

internal sealed record WebSessionInfo(
    string SessionId,
    string BrowserType,
    bool IsHeadless,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset LastActivityAtUtc,
    string CurrentUrl);

internal sealed record WebCloseResult(string SessionId, bool Closed);

internal sealed record WebNavigationResult(string SessionId, string Url, string Title);

internal sealed record WebInspection(
    string SessionId,
    string Url,
    string Title,
    IReadOnlyList<WebElementInfo> Elements);

internal sealed record WebPageSource(
    string SessionId,
    string Url,
    string Title,
    string Html,
    bool Truncated);

internal sealed record WebElementInfo(
    string? Reference,
    string TagName,
    string? Id,
    string? Name,
    string? Type,
    string? AriaLabel,
    string? Role,
    string? Placeholder,
    string? TestId,
    string? Text,
    string? Selector,
    bool Enabled,
    bool Selected);

internal sealed record WebActionRecord(
    int Step,
    DateTimeOffset TimestampUtc,
    string Action,
    string? Value,
    WebElementInfo Element);

internal sealed record WebActionResult(
    string Action,
    string Url,
    string Title,
    WebElementInfo Element,
    string? Value);
