using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
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
        public Dictionary<string, InspectionReference> InspectionReferences { get; } = new(StringComparer.Ordinal);
        public List<WebActionRecord> Actions { get; } = [];
    }

    private sealed record InspectionReference(IWebElement Element, WebElementInfo Info);

    private sealed record JavaScriptElementSnapshot(
        int Index,
        string TagName,
        string? Id,
        string? Name,
        string? Type,
        string? AriaLabel,
        string? Role,
        string? Placeholder,
        string? TestId,
        string? Text,
        bool Enabled,
        bool Selected,
        string? Value,
        bool? Checked,
        bool? Expanded,
        string? AccessibleName,
        string? Label,
        string? Href);

    private sealed record PageState(string Url, string Title);

    private const string ActionableSelector =
        "input, textarea, select, button, a, label, [role='button'], [role='link'], [role='radio'], [role='checkbox'], [role='option'], [role='treeitem'], [role='alert'], [role='status'], [role='dialog'], [aria-expanded], [aria-selected], [contenteditable='true'], [tabindex]:not([tabindex='-1']), [data-testid], [onclick], [style*='cursor: pointer']";

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
            var page = ReadPageState(entry.Driver);
            return new WebNavigationResult(
                arguments.SessionId,
                page.Url,
                page.Title);
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
            var page = ReadPageState(entry.Driver);

            return new WebInspection(
                arguments.SessionId,
                page.Url,
                page.Title,
                elements);
        });
    }

    public WebWaitResult Wait(WaitArguments arguments)
    {
        if (arguments.TimeoutSeconds < 1 || arguments.TimeoutSeconds > 300)
            throw new ArgumentException("The wait timeout must be between 1 and 300 seconds.");

        var conditions = (string.IsNullOrWhiteSpace(arguments.Url) ? 0 : 1)
            + (string.IsNullOrWhiteSpace(arguments.Text) ? 0 : 1)
            + (string.IsNullOrWhiteSpace(arguments.Selector) ? 0 : 1);
        if (conditions != 1)
            throw new ArgumentException("Wait requires exactly one URL, text, or CSS selector condition.");

        return WithSession(arguments.SessionId, entry =>
        {
            var wait = new WebDriverWait(entry.Driver, TimeSpan.FromSeconds(arguments.TimeoutSeconds));
            wait.Until(driver => MatchesWaitCondition(
                driver,
                arguments.Url,
                arguments.Text,
                arguments.Selector));

            InvalidateInspection(entry);
            var page = ReadPageState(entry.Driver);
            return new WebWaitResult(
                arguments.SessionId,
                page.Url,
                page.Title,
                arguments.Url,
                arguments.Text,
                arguments.Selector);
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
        => WithElement(arguments.SessionId, MergeLocator(arguments.Locator, arguments.Reference, arguments.Selector), (entry, element, info) =>
        {
            element.Click();
            return RecordAction(entry, "click", element, info, null);
        });

    public WebActionResult Type(TypeArguments arguments)
    {
        if (arguments.Text is null)
            throw new ArgumentException("The text value is required.");

        return WithElement(arguments.SessionId, MergeLocator(arguments.Locator, arguments.Reference, arguments.Selector), (entry, element, info) =>
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
        return WithElement(arguments.SessionId, MergeLocator(arguments.Locator, arguments.Reference, arguments.Selector), (entry, element, info) =>
        {
            element.SendKeys(key);
            return RecordAction(entry, "key", element, info, arguments.Key!.Trim());
        });
    }

    public WebActionResult Select(SelectArguments arguments)
    {
        if (string.IsNullOrWhiteSpace(arguments.Text) && string.IsNullOrWhiteSpace(arguments.Value))
            throw new ArgumentException("Use either a visible option text or an option value.");

        return WithElement(arguments.SessionId, MergeLocator(arguments.Locator, arguments.Reference, arguments.Selector), (entry, element, info) =>
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

    public WebActionResult SetChecked(ElementActionArguments arguments, bool desired)
        => WithElement(arguments.SessionId, MergeLocator(arguments.Locator, arguments.Reference, arguments.Selector), (entry, element, info) =>
        {
            var type = element.GetAttribute("type");
            if (!string.Equals(type, "checkbox", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(type, "radio", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException("The target element must be a checkbox or radio button.");
            }

            if (element.Selected != desired)
            {
                if (!desired && string.Equals(type, "radio", StringComparison.OrdinalIgnoreCase))
                    throw new ArgumentException("A selected radio button cannot be unchecked.");

                element.Click();
            }

            return RecordAction(entry, desired ? "check" : "uncheck", element, info, null);
        });

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
        ElementLocatorArguments locator,
        Func<SessionEntry, IWebElement, WebElementInfo, WebActionResult> action)
        => WithSession(sessionId, entry =>
        {
            try
            {
                var resolved = ResolveElement(entry, locator);
                var info = resolved.Info ?? DescribeElement(entry.Driver, resolved.Element, locator.Reference);
                return action(entry, resolved.Element, info);
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

    private static ResolvedElement ResolveElement(
        SessionEntry entry,
        ElementLocatorArguments locator)
    {
        if (!string.IsNullOrWhiteSpace(locator.Reference))
        {
            if (!entry.InspectionReferences.TryGetValue(locator.Reference, out var inspectedElement))
                throw new WebAutomationException(
                    "inspection_required",
                    $"Element reference '{locator.Reference}' is not available. Inspect the current page before acting.");

            return new ResolvedElement(inspectedElement.Element, inspectedElement.Info);
        }

        var matches = FindMatchingElements(entry.Driver, locator);
        if (matches.Count <= locator.Index)
            throw new WebAutomationException(
                "element_not_found",
                "No element matched the supplied WebEngine locator.");

        return new ResolvedElement(matches[locator.Index], null);
    }

    private static ElementLocatorArguments MergeLocator(
        ElementLocatorArguments? locator,
        string? reference,
        string? selector)
        => locator is null
            ? new ElementLocatorArguments(Reference: reference, Selector: selector)
            : locator with
            {
                Reference = locator.Reference ?? reference,
                Selector = locator.Selector ?? selector
            };

    private static List<IWebElement> FindMatchingElements(
        IWebDriver driver,
        ElementLocatorArguments locator)
    {
        if (locator.Index < 0)
            throw new ArgumentException("The locator index cannot be negative.");

        List<IWebElement>? matches = null;

        void Intersect(IEnumerable<IWebElement> candidates)
        {
            var candidateList = candidates.ToList();
            matches = matches is null
                ? candidateList
                : matches
                    .Where(current => candidateList.Any(candidate => candidate.Equals(current)))
                    .ToList();
        }

        if (!string.IsNullOrWhiteSpace(locator.Selector))
            Intersect(driver.FindElements(By.CssSelector(locator.Selector)));
        if (!string.IsNullOrWhiteSpace(locator.Id))
            Intersect(driver.FindElements(By.Id(locator.Id)));
        if (!string.IsNullOrWhiteSpace(locator.Name))
            Intersect(driver.FindElements(By.Name(locator.Name)));
        if (!string.IsNullOrWhiteSpace(locator.TagName))
            Intersect(driver.FindElements(By.TagName(locator.TagName)));
        if (!string.IsNullOrWhiteSpace(locator.LinkText))
            Intersect(driver.FindElements(By.LinkText(locator.LinkText)));
        if (!string.IsNullOrWhiteSpace(locator.XPath))
            Intersect(driver.FindElements(By.XPath(locator.XPath)));
        if (!string.IsNullOrWhiteSpace(locator.AriaLabel))
        {
            Intersect(driver.FindElements(By.CssSelector(
                $"[{EscapeCssAttribute("aria-label", locator.AriaLabel)}]")));
        }
        if (!string.IsNullOrWhiteSpace(locator.InnerText))
        {
            Intersect(driver.FindElements(By.XPath(
                $"//*[normalize-space(.)={EscapeXPathLiteral(locator.InnerText)}]")));
        }
        if (!string.IsNullOrWhiteSpace(locator.ClassName))
        {
            foreach (var className in locator.ClassName.Split(
                         ' ',
                         StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                Intersect(driver.FindElements(By.ClassName(className)));
            }
        }

        if (matches is null)
            throw new ArgumentException(
                "Provide at least one native WebEngine locator property.");

        return matches;
    }

    private static string EscapeXPathLiteral(string value)
    {
        if (!value.Contains('\'', StringComparison.Ordinal))
            return $"'{value}'";
        if (!value.Contains('"', StringComparison.Ordinal))
            return $"\"{value}\"";

        var parts = value.Split('\'');
        var expression = new StringBuilder("concat(");
        for (var index = 0; index < parts.Length; index++)
        {
            if (index > 0)
                expression.Append(", \"'\", ");

            expression.Append('\'');
            expression.Append(parts[index]);
            expression.Append('\'');
        }

        expression.Append(')');
        return expression.ToString();
    }

    private static IReadOnlyList<WebElementInfo> CaptureElements(SessionEntry entry, int limit)
    {
        var candidates = entry.Driver.FindElements(By.CssSelector(ActionableSelector)).ToArray();
        var snapshots = ReadElementSnapshots(entry.Driver, candidates, limit, visibleOnly: true);
        var result = new List<WebElementInfo>(Math.Min(candidates.Length, limit));

        foreach (var snapshot in snapshots)
        {
            if (result.Count >= limit)
                break;

            if ((uint)snapshot.Index >= (uint)candidates.Length)
                continue;

            var reference = $"ref={result.Count + 1}";
            var info = ToElementInfo(snapshot, reference);
            entry.InspectionReferences[reference] = new(candidates[snapshot.Index], info);
            result.Add(info);
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
        var page = ReadPageState(entry.Driver);
        var record = new WebActionRecord(
            entry.Actions.Count + 1,
            DateTimeOffset.UtcNow,
            action,
            value,
            info);
        entry.Actions.Add(record);

        return new WebActionResult(
            action,
            page.Url,
            page.Title,
            info,
            value);
    }

    private static bool MatchesWaitCondition(
        IWebDriver driver,
        string? url,
        string? text,
        string? selector)
    {
        if (!string.IsNullOrWhiteSpace(url))
        {
            var currentUrl = driver.Url;
            return string.Equals(currentUrl, url, StringComparison.OrdinalIgnoreCase)
                || currentUrl.Contains(url, StringComparison.OrdinalIgnoreCase);
        }

        if (!string.IsNullOrWhiteSpace(selector))
            return driver.FindElements(By.CssSelector(selector)).Any(IsVisible);

        if (driver is not IJavaScriptExecutor scriptExecutor)
            return driver.FindElement(By.TagName("body")).Text.Contains(
                text!,
                StringComparison.OrdinalIgnoreCase);

        return Convert.ToBoolean(
            scriptExecutor.ExecuteScript(
                "return (document.body?.innerText || document.body?.textContent || '').toLowerCase().includes(String(arguments[0]).toLowerCase());",
                text));
    }

    private static WebElementInfo DescribeElement(
        IWebDriver driver,
        IWebElement element,
        string? reference)
    {
        var snapshot = ReadElementSnapshots(driver, [element], 1, visibleOnly: false).SingleOrDefault()
            ?? throw new WebAutomationException(
                "element_not_found",
                "The target element could not be described.");
        return ToElementInfo(snapshot, reference);
    }

    private static WebElementInfo ToElementInfo(
        JavaScriptElementSnapshot snapshot,
        string? reference)
        => new(
            reference,
            snapshot.TagName,
            snapshot.Id,
            snapshot.Name,
            snapshot.Type,
            snapshot.AriaLabel,
            snapshot.Role,
            snapshot.Placeholder,
            snapshot.TestId,
            snapshot.Text,
            BuildSelector(snapshot.TagName, snapshot.Id, snapshot.Name, snapshot.TestId, snapshot.AriaLabel),
            snapshot.Enabled,
            snapshot.Selected,
            snapshot.Value,
            snapshot.Checked,
            snapshot.Expanded,
            snapshot.AccessibleName,
            snapshot.Label,
            snapshot.Href);

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

    private static IReadOnlyList<JavaScriptElementSnapshot> ReadElementSnapshots(
        IWebDriver driver,
        IReadOnlyList<IWebElement> elements,
        int limit,
        bool visibleOnly)
    {
        if (driver is not IJavaScriptExecutor scriptExecutor)
            throw new WebAutomationException(
                "inspection_unavailable",
                "The active browser does not support JavaScript inspection.");

        const string script =
            """
            const elements = arguments[0];
            const limit = arguments[1];
            const visibleOnly = arguments[2];
            const trim = value => {
                if (value === null || value === undefined) {
                    return null;
                }
                const text = String(value).trim();
                return text.length === 0 ? null : text;
            };
            const isVisible = element => {
                if (!element.isConnected) {
                    return false;
                }
                const style = window.getComputedStyle(element);
                const rect = element.getBoundingClientRect();
                return style.display !== 'none'
                    && style.visibility !== 'hidden'
                    && style.visibility !== 'collapse'
                    && rect.width > 0
                    && rect.height > 0;
            };
            const getLabel = element => {
                if (element.labels && element.labels.length > 0) {
                    return trim(element.labels[0].innerText || element.labels[0].textContent);
                }

                const id = element.getAttribute('id');
                if (id) {
                    const label = document.querySelector(`label[for="${CSS.escape(id)}"]`);
                    if (label) {
                        return trim(label.innerText || label.textContent);
                    }
                }

                return null;
            };
            const describe = (element, index) => {
                const visible = isVisible(element);
                if (visibleOnly && !visible) {
                    return null;
                }

                const label = getLabel(element);
                const ariaLabel = trim(element.getAttribute('aria-label'));
                const text = trim(element.innerText || element.textContent);
                const type = trim(element.getAttribute('type'));
                const expandedAttribute = element.getAttribute('aria-expanded');
                return {
                    index,
                    tagName: element.tagName.toLowerCase(),
                    id: trim(element.getAttribute('id')),
                    name: trim(element.getAttribute('name')),
                    type,
                    ariaLabel,
                    role: trim(element.getAttribute('role')),
                    placeholder: trim(element.getAttribute('placeholder')),
                    testId: trim(element.getAttribute('data-testid')),
                    text,
                    enabled: !element.matches(':disabled'),
                    selected: element.selected === true,
                    value: 'value' in element ? trim(element.value) : null,
                    checked: type === 'checkbox' || type === 'radio'
                        ? element.checked === true
                        : null,
                    expanded: expandedAttribute === null
                        ? null
                        : expandedAttribute.toLowerCase() === 'true',
                    accessibleName: ariaLabel || label || text,
                    label,
                    href: trim(element.getAttribute('href')),
                    visible
                };
            };
            const described = [];
            for (let index = 0; index < elements.length && described.length < limit; index++) {
                const element = describe(elements[index], index);
                if (element !== null) {
                    described.push(element);
                }
            }
            return JSON.stringify(described);
            """;

        var json = scriptExecutor.ExecuteScript(
            script,
            elements.ToArray(),
            Math.Clamp(limit, 1, 500),
            visibleOnly)?.ToString();

        if (string.IsNullOrWhiteSpace(json))
            return [];

        return JsonSerializer.Deserialize<List<JavaScriptElementSnapshot>>(
                   json,
                   DaemonProtocol.JsonOptions)
               ?? [];
    }

    private static bool IsSecretField(IWebElement element)
        => string.Equals(element.GetAttribute("type"), "password", StringComparison.OrdinalIgnoreCase)
            || string.Equals(element.GetAttribute("autocomplete"), "current-password", StringComparison.OrdinalIgnoreCase)
            || string.Equals(element.GetAttribute("autocomplete"), "new-password", StringComparison.OrdinalIgnoreCase);

    private static PageState ReadPageState(IWebDriver driver)
    {
        if (driver is not IJavaScriptExecutor scriptExecutor)
            return new(driver.Url, driver.Title);

        const string script =
            "return JSON.stringify({ url: window.location.href, title: document.title });";
        var json = scriptExecutor.ExecuteScript(script)?.ToString();
        if (string.IsNullOrWhiteSpace(json))
            return new(driver.Url, driver.Title);

        return JsonSerializer.Deserialize<PageState>(json, DaemonProtocol.JsonOptions)
            ?? new(driver.Url, driver.Title);
    }

    private sealed record ResolvedElement(IWebElement Element, WebElementInfo? Info);

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
        var normalized = string.IsNullOrWhiteSpace(browser) ? "Edge" : browser.Trim();
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

internal sealed record WebWaitResult(
    string SessionId,
    string Url,
    string Title,
    string? UrlCondition,
    string? TextCondition,
    string? SelectorCondition);

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
    bool Selected,
    string? Value = null,
    bool? Checked = null,
    bool? Expanded = null,
    string? AccessibleName = null,
    string? Label = null,
    string? Href = null);

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
