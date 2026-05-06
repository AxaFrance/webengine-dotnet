using AxaFrance.WebEngine.Web;
using AxaFrance.WebEngine.Mcp.Tools.Selenium.Models;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using OpenQA.Selenium;
using System.ComponentModel;

namespace AxaFrance.WebEngine.Mcp.Tools.Selenium;

[McpServerToolType]
public sealed class SeleniumTool(SeleniumSessionManager sessionManager, ILogger<SeleniumTool> logger)
{
    [McpServerTool]
    [Description(@"Starts a new browser session. Returns a sessionId that must be passed to all subsequent browser calls.
        Usage: StartSession(browserType: 'Chrome', headless: false)
        Returns: { SessionId: 'abc123', BrowserType: 'Chrome', IsHeadless: false }")]
    public BrowserSession StartSession(
        [Description("Browser engine to use: Chrome, Firefox, or Edge (default: Chrome)")] string browserType = "Chrome",
        [Description("Run headless without a visible UI window. Set false for visual debugging (default: false)")] bool headless = false)
    {
        logger.LogInformation("[MCP] StartSession: browserType={BrowserType}, headless={Headless}", browserType, headless);
        return sessionManager.StartSession(browserType, headless);
    }

    [McpServerTool]
    [Description("Navigates the browser to a URL and returns the final URL after any redirects.")]
    public string NavigateTo(
        [Description("The session ID returned by StartSession")] string sessionId,
        [Description("The full URL to navigate to, e.g. https://example.com")] string url)
    {
        logger.LogInformation("[MCP] NavigateTo: sessionId={SessionId}, url={Url}", sessionId, url);
        var driver = sessionManager.GetDriver(sessionId);
        driver.Navigate().GoToUrl(url);
        return driver.Url;
    }

    [McpServerTool]
    [Description("""
        Returns a compact accessibility tree of the current page.
        Each line: [role] "accessible name" value="..." [state] id="..." name="..." ref=N

        HOW TO USE REFS:
          Every interactive element gets a ref=N number. ALWAYS pass ref=N via ElementDescriptor.Ref
          when acting on elements found in this snapshot — it maps directly to the DOM node with
          zero guessing (equivalent to Playwright's ref parameter).
          Only fall back to id/name/aria-label if Ref fails.

        REF STALENESS WARNING:
          Refs are valid only for the current DOM state. React/Vue/Angular apps wipe data-mcp-ref
          attributes during virtual DOM reconciliation after any state change (typing, clicking,
          navigation). After any action that mutates the page, call GetAccessibilitySnapshot again
          before using new refs — stale refs will silently match no element.

        WHEN TO USE: For large pages (>50 KB HTML) or for a quick element ID pass.
        ALTERNATIVE: GetActionableHtml gives the same locator info in HTML form and is often
        easier to read when planning multi-step form fills.
        """)]
    public string GetAccessibilitySnapshot(
        [Description("The session ID returned by StartSession")] string sessionId)
    {
        logger.LogInformation("[MCP] GetAccessibilitySnapshot: sessionId={SessionId}", sessionId);
        return AccessibilitySnapshot.Capture(sessionManager.GetDriver(sessionId));
    }

    [McpServerTool]
    [Description(@"Returns the current page minimized HTML, together with its URL and title. Used to find stable locators for elements.
        STRATEGY: If HTML <50KB, use this method and plan multiple actions at once (fill form, select dropdowns, check boxes, click next).
        If HTML >50KB, use GetPageHtmlChunk() to read in 10KB slices, or GetAccessibilitySnapshot() for faster element identification.
        Planning multiple actions from HTML reduces round-trips and speeds up test execution.")]
    public PageContent GetPageHtml(
        [Description("The session ID returned by StartSession")] string sessionId)
    {
        logger.LogInformation("[MCP] GetPageHtml: sessionId={SessionId}", sessionId);
        var driver = sessionManager.GetDriver(sessionId);
        var html = HtmlCleaner.CleanPage(driver);
        sessionManager.SetHtmlCache(sessionId, html, driver.Url);
        return new PageContent
        {
            SessionId = sessionId,
            Url = driver.Url,
            Title = driver.Title,
            Html = html
        };
    }

    [McpServerTool]
    [Description("""
        Returns a 10 KB slice of the current page's cleaned HTML. Fetch chunk 0 first — the response
        includes TotalChunks so you know how many slices exist. Fetch further chunks only for the
        sections where you expect to find your target elements.
        WHEN TO USE: When GetPageHtml would return >30 KB (large dashboards, data tables).
        The HTML is cached after each action, so repeated chunk calls are cheap.
        """)]
    public PageHtmlChunk GetPageHtmlChunk(
        [Description("The session ID returned by StartSession")] string sessionId,
        [Description("Zero-based chunk index. Start at 0 to learn TotalChunks, then fetch additional chunks as needed.")] int chunkIndex = 0,
        [Description("Chunk size in KB (default: 10)")] int chunkSizeKB = 10)
    {
        logger.LogDebug("[MCP] GetPageHtmlChunk: sessionId={SessionId}, chunk={ChunkIndex}, size={ChunkSizeKB}KB", sessionId, chunkIndex, chunkSizeKB);
        var driver = sessionManager.GetDriver(sessionId);
        var currentUrl = driver.Url;

        var cached = sessionManager.GetHtmlCache(sessionId);
        string html;
        if (cached == null || cached.Url != currentUrl)
        {
            html = HtmlCleaner.CleanPage(driver);
            sessionManager.SetHtmlCache(sessionId, html, currentUrl);
        }
        else
        {
            html = cached.Html;
        }

        var chunkBytes = chunkSizeKB * 1024;
        var totalChunks = (int)Math.Ceiling((double)html.Length / chunkBytes);
        var start = chunkIndex * chunkBytes;

        if (start >= html.Length)
            return new PageHtmlChunk
            {
                SessionId = sessionId, Url = currentUrl,
                ChunkIndex = chunkIndex, TotalChunks = totalChunks,
                TotalSizeKB = Math.Round(html.Length / 1024.0, 1),
                ChunkSizeKB = chunkSizeKB, Content = string.Empty
            };

        return new PageHtmlChunk
        {
            SessionId = sessionId,
            Url = currentUrl,
            ChunkIndex = chunkIndex,
            TotalChunks = totalChunks,
            TotalSizeKB = Math.Round(html.Length / 1024.0, 1),
            ChunkSizeKB = chunkSizeKB,
            Content = html.Substring(start, Math.Min(chunkBytes, html.Length - start))
        };
    }

    [McpServerTool]
    [Description("""
        Returns a structural skeleton of the current page: only landmark sections, form controls,
        headings, links, tables, and images survive. Pure container tags (div, span, p, ul, li, …)
        are unwrapped — their children are hoisted so no interactive content is lost.

        WHEN TO USE: Best first-pass inspection tool for any page.
          - Typically 70–80 % smaller than GetPageHtml on SPA/Tailwind pages.
          - Contains every element you can interact with, with exact id/name/aria attributes.
          - After reading this, use GetPageHtml only if you need surrounding prose context.

        WHEN NOT TO USE:
          - When you need decorative text / paragraph content between interactive elements.
          - Use GetPageHtml or GetPageHtmlChunk for that context.
        """)]
    public PageContent GetActionableHtml(
        [Description("The session ID returned by StartSession")] string sessionId)
    {
        logger.LogInformation("[MCP] GetActionableHtml: sessionId={SessionId}", sessionId);
        var driver = sessionManager.GetDriver(sessionId);
        var html = HtmlCleaner.CleanPageCompact(driver);
        return new PageContent
        {
            SessionId = sessionId,
            Url = driver.Url,
            Title = driver.Title,
            Html = html
        };
    }

    [McpServerTool]
    [Description(@"Returns the cleaned outer HTML of a specific element (scripts, styles and base64 data removed).")]
    public string GetElementHtml(
        [Description("The session ID returned by StartSession")] string sessionId,
        [Description("Stable element descriptor using Id, Name, test-id, TagName+InnerText, or AriaLabel. Avoid CssSelector/XPath unless necessary.")] ElementDescriptor element)
    {
        logger.LogDebug("[MCP] GetElementHtml: sessionId={SessionId}, element={ElementName}", sessionId, element.Id ?? element.Name ?? element.InnerText ?? "unknown");
        var driver = sessionManager.GetDriver(sessionId);
        return HtmlCleaner.CleanElement(driver, Resolve(driver, element));
    }

    [McpServerTool]
    [Description(@"Returns the visible text of an element.")]
    public string GetElementText(
        [Description("The session ID returned by StartSession")] string sessionId,
        [Description("Stable element descriptor: prefer Id, Name, or TagName+attribute combinations")] ElementDescriptor element)
    {
        logger.LogDebug("[MCP] GetElementText: sessionId={SessionId}, element={ElementName}", sessionId, element.Id ?? element.Name ?? element.InnerText ?? "unknown");
        return Resolve(sessionManager.GetDriver(sessionId), element).Text;
    }

    [McpServerTool]
    [Description(@"Clicks an element and returns page state info (URL and sizes only, not full HTML).")]
    public string ClickElement(
        [Description("The session ID returned by StartSession")] string sessionId,
        [Description("Element to click - MUST be from actual DOM inspection, not guessed. Use Id, Name, test-id, or TagName+InnerText.")] ElementDescriptor element)
    {
        logger.LogInformation("[MCP ACTION] ClickElement: Ref={Ref}, Id={Id}, Name={Name}, Text={InnerText}, sessionId={SessionId}", element.Ref, element.Id, element.Name, element.InnerText, sessionId);
        var driver = sessionManager.GetDriver(sessionId);
        var webElement = Resolve(driver, element);
        var logEntry = ActionLogBuilder.Build(driver, "Click", webElement, null, null, driver.Url);
        webElement.Click();
        sessionManager.AppendActionLog(sessionId, logEntry);
        return GetCompactStateAfterAction(driver, sessionId, logEntry.ElementTag);
    }

    [McpServerTool]
    [Description(@"Types text into an input field and returns page state info (sizes only).
        BATCHING: If page HTML <30KB, you can execute multiple TypeText/SetText calls in sequence without re-inspecting DOM.
        After action, check returned size to decide if you need to re-inspect or continue with next action.")]
    public string TypeText(
        [Description("The session ID returned by StartSession")] string sessionId,
        [Description("Input element from actual DOM inspection - use Id, Name, or aria-label")] ElementDescriptor element,
        [Description("Text to type into the field")] string text,
        [Description("Clear the field before typing (default: true)")] bool clearFirst = true)
    {
        logger.LogInformation("[MCP ACTION] TypeText: text={Text}, Ref={Ref}, Id={Id}, Name={Name}, clearFirst={ClearFirst}, sessionId={SessionId}", text, element.Ref, element.Id, element.Name, clearFirst, sessionId);
        var driver = sessionManager.GetDriver(sessionId);
        var webElement = Resolve(driver, element);
        var logEntry = ActionLogBuilder.Build(driver, "TypeText", webElement, text, null, driver.Url);
        if (clearFirst)
            webElement.Clear();
        webElement.SendKeys(text);
        sessionManager.AppendActionLog(sessionId, logEntry);
        return GetCompactStateAfterAction(driver, sessionId, logEntry.ElementTag);
    }

    [McpServerTool]
    [Description(@"Clears the text from an input field.")]
    public string ClearText(
        [Description("The session ID returned by StartSession")] string sessionId,
        [Description("Input element with stable locators (Id, Name preferred for form fields)")] ElementDescriptor element)
    {
        logger.LogDebug("[MCP] ClearText: sessionId={SessionId}, element={ElementName}", sessionId, element.Id ?? element.Name ?? "unknown");
        var driver = sessionManager.GetDriver(sessionId);
        Resolve(driver, element).Clear();
        return "Text cleared from element";
    }

    [McpServerTool]
    [Description(@"Sets text in an input field (clears first) and returns page state info.")]
    public string SetText(
        [Description("The session ID returned by StartSession")] string sessionId,
        [Description("Input element from DOM inspection (Id or Name preferred)")] ElementDescriptor element,
        [Description("Text to set in the field")] string text)
    {
        logger.LogInformation("[MCP ACTION] SetText: text={Text}, Ref={Ref}, Id={Id}, Name={Name}, sessionId={SessionId}", text, element.Ref, element.Id, element.Name, sessionId);
        var driver = sessionManager.GetDriver(sessionId);
        var webElement = Resolve(driver, element);
        var logEntry = ActionLogBuilder.Build(driver, "SetText", webElement, text, null, driver.Url);
        webElement.Clear();
        webElement.SendKeys(text);
        sessionManager.AppendActionLog(sessionId, logEntry);
        return GetCompactStateAfterAction(driver, sessionId, logEntry.ElementTag);
    }

    [McpServerTool]
    [Description(@"Selects dropdown option by visible text and returns page state info.")]
    public string SelectFromDropdownByText(
        [Description("The session ID returned by StartSession")] string sessionId,
        [Description("Select element from DOM inspection (Id or Name for <select> tags)")] ElementDescriptor element,
        [Description("The visible text of the option")] string text)
    {
        logger.LogInformation("[MCP ACTION] SelectFromDropdownByText: text={Text}, Ref={Ref}, Id={Id}, Name={Name}, sessionId={SessionId}", text, element.Ref, element.Id, element.Name, sessionId);
        var driver = sessionManager.GetDriver(sessionId);
        var webElement = Resolve(driver, element);
        var logEntry = ActionLogBuilder.Build(driver, "SelectByText", webElement, text, null, driver.Url);
        Describe(driver, element).SelectByText(text);
        sessionManager.AppendActionLog(sessionId, logEntry);
        return GetCompactStateAfterAction(driver, sessionId, logEntry.ElementTag);
    }

    [McpServerTool]
    [Description(@"Selects dropdown option by value attribute and returns page state info.
    Example element: { Id: 'vehicleType' } with value: 'sedan', 'suv', etc.")]

    public string SelectFromDropdownByValue(
        [Description("The session ID returned by StartSession")] string sessionId,
        [Description("Select element from DOM (Id or Name)")] ElementDescriptor element,
        [Description("The value attribute of <option> (not visible text)")] string value)
    {
        logger.LogInformation("[MCP ACTION] SelectFromDropdownByValue: value={Value}, Ref={Ref}, Id={Id}, Name={Name}, sessionId={SessionId}", value, element.Ref, element.Id, element.Name, sessionId);
        var driver = sessionManager.GetDriver(sessionId);
        var webElement = Resolve(driver, element);
        var logEntry = ActionLogBuilder.Build(driver, "SelectByValue", webElement, value, null, driver.Url);
        Describe(driver, element).SelectByValue(value);
        sessionManager.AppendActionLog(sessionId, logEntry);
        return GetCompactStateAfterAction(driver, sessionId, logEntry.ElementTag);
    }

    [McpServerTool]
    [Description(@"Checks a checkbox or selects a radio button and returns page state info.
        Example: { Id: 'agreeTerms' } or { Name: 'newsletter' } or { Attributes: [{ Name: 'data-id', Value: 'consent-checkbox' }] }")]
    public string CheckElement(
        [Description("The session ID returned by StartSession")] string sessionId,
        [Description("Checkbox/radio from DOM inspection - use actual TagName (input/div/span) and attributes from DOM")] ElementDescriptor element)
    {
        logger.LogInformation("[MCP ACTION] CheckElement: Ref={Ref}, Id={Id}, Name={Name}, sessionId={SessionId}", element.Ref, element.Id, element.Name, sessionId);
        var driver = sessionManager.GetDriver(sessionId);
        var webElement = Resolve(driver, element);
        var logEntry = ActionLogBuilder.Build(driver, "Check", webElement, null, null, driver.Url);
        if (!webElement.Selected)
            webElement.Click();
        sessionManager.AppendActionLog(sessionId, logEntry);
        return GetCompactStateAfterAction(driver, sessionId, logEntry.ElementTag);
    }

    [McpServerTool]
    [Description(@"Unchecks a checkbox and returns page state info.
        Example: { Id: 'acceptMarketing' } or { Name: 'subscribe' }")]
    public string UncheckElement(
        [Description("The session ID returned by StartSession")] string sessionId,
        [Description("Checkbox from DOM inspection (Id or Name)")] ElementDescriptor element)
    {
        logger.LogInformation("[MCP ACTION] UncheckElement: Ref={Ref}, Id={Id}, Name={Name}, sessionId={SessionId}", element.Ref, element.Id, element.Name, sessionId);
        var driver = sessionManager.GetDriver(sessionId);
        var webElement = Resolve(driver, element);
        var logEntry = ActionLogBuilder.Build(driver, "Uncheck", webElement, null, null, driver.Url);
        if (webElement.Selected)
            webElement.Click();
        sessionManager.AppendActionLog(sessionId, logEntry);
        return GetCompactStateAfterAction(driver, sessionId, logEntry.ElementTag);
    }

    [McpServerTool]
    [Description("Takes a screenshot of the current browser viewport and returns it as a base64-encoded PNG string.")]
    public string TakeScreenshot(
        [Description("The session ID returned by StartSession")] string sessionId)
    {
        logger.LogDebug("[MCP] TakeScreenshot: sessionId={SessionId}", sessionId);
        return ((ITakesScreenshot)sessionManager.GetDriver(sessionId)).GetScreenshot().AsBase64EncodedString;
    }

    [McpServerTool]
    [Description("Executes JavaScript in the browser context and returns the string representation of the result.")]
    public string ExecuteScript(
        [Description("The session ID returned by StartSession")] string sessionId,
        [Description("JavaScript code to execute, e.g. 'return document.title'")] string script)
    {
        logger.LogDebug("[MCP] ExecuteScript: sessionId={SessionId}, script={Script}...", sessionId, script[..Math.Min(50, script.Length)]);
        return ((IJavaScriptExecutor)sessionManager.GetDriver(sessionId))
            .ExecuteScript(script)?.ToString() ?? string.Empty;
    }

    [McpServerTool]
    [Description("Returns the current URL of the browser tab.")]
    public string GetCurrentUrl(
        [Description("The session ID returned by StartSession")] string sessionId)
    {
        logger.LogDebug("[MCP] GetCurrentUrl: sessionId={SessionId}", sessionId);
        return sessionManager.GetDriver(sessionId).Url;
    }

    [McpServerTool]
    [Description("Returns the current page title.")]
    public string GetPageTitle(
        [Description("The session ID returned by StartSession")] string sessionId)
    {
        logger.LogDebug("[MCP] GetPageTitle: sessionId={SessionId}", sessionId);
        return sessionManager.GetDriver(sessionId).Title;
    }

    [McpServerTool]
    [Description("Navigates the browser back to the previous page and returns the new URL.")]
    public string GoBack(
        [Description("The session ID returned by StartSession")] string sessionId)
    {
        logger.LogInformation("[MCP] GoBack: sessionId={SessionId}", sessionId);
        var driver = sessionManager.GetDriver(sessionId);
        driver.Navigate().Back();
        return driver.Url;
    }

    [McpServerTool]
    [Description("Navigates the browser forward and returns the new URL.")]
    public string GoForward(
        [Description("The session ID returned by StartSession")] string sessionId)
    {
        logger.LogInformation("[MCP] GoForward: sessionId={SessionId}", sessionId);
        var driver = sessionManager.GetDriver(sessionId);
        driver.Navigate().Forward();
        return driver.Url;
    }

    [McpServerTool]
    [Description(@"Waits until an element appears in the DOM, up to a timeout in seconds.
        LOCATOR: Use the same stable locator strategy as other methods (Id, Name, test-id, TagName+InnerText).
        Example: { Id: 'loadingSpinner' } or { TagName: 'div', InnerText: 'Loading...' }")]
    public string WaitForElement(
        [Description("The session ID returned by StartSession")] string sessionId,
        [Description("Element to wait for using stable locators")] ElementDescriptor element,
        [Description("Maximum wait time in seconds (default: 10)")] int timeoutSeconds = 10)
    {
        logger.LogInformation("[MCP] WaitForElement: sessionId={SessionId}, element={ElementName}, timeout={Timeout}s", sessionId, element.Id ?? element.Name ?? element.InnerText ?? "unknown", timeoutSeconds);
        var driver = sessionManager.GetDriver(sessionId);
        var found = Resolve(driver, element, timeoutSeconds);
        return $"Element found. Text: {found.Text}";
    }

    [McpServerTool]
    [Description("""
        Clicks at specific viewport coordinates (x, y).
        Use this as last resort after TakeScreenshot when element-based ClickElement fails
        (canvas rendering, custom widgets, or no usable ARIA/HTML).
        Coordinates are viewport-relative: (0, 0) is the top-left corner of the visible area.
        """)]
    public string ClickAt(
        [Description("The session ID returned by StartSession")] string sessionId,
        [Description("Horizontal viewport coordinate in pixels (0 = left edge)")] int x,
        [Description("Vertical viewport coordinate in pixels (0 = top edge)")] int y)
    {
        logger.LogDebug("[MCP] ClickAt: sessionId={SessionId}, x={X}, y={Y}", sessionId, x, y);
        var driver = sessionManager.GetDriver(sessionId);
        ((IJavaScriptExecutor)driver).ExecuteScript(
            "var el = document.elementFromPoint(arguments[0], arguments[1]); if (el) { el.dispatchEvent(new MouseEvent('click', {bubbles:true, cancelable:true, clientX:arguments[0], clientY:arguments[1]})); }",
            (long)x, (long)y);
        return driver.Url;
    }

    [McpServerTool]
    [Description("""
        Scrolls the page by a relative pixel offset and returns the new absolute scroll position.
        Use positive deltaY to scroll down, negative to scroll up.
        Call TakeScreenshot or GetAccessibilitySnapshot after scrolling to see newly revealed content.
        """)]
    public string ScrollBy(
        [Description("The session ID returned by StartSession")] string sessionId,
        [Description("Horizontal scroll in pixels — positive = right, negative = left")] int deltaX,
        [Description("Vertical scroll in pixels — positive = down, negative = up")] int deltaY)
    {
        logger.LogDebug("[MCP] ScrollBy: sessionId={SessionId}, deltaX={DeltaX}, deltaY={DeltaY}", sessionId, deltaX, deltaY);
        var driver = sessionManager.GetDriver(sessionId);
        var pos = ((IJavaScriptExecutor)driver).ExecuteScript(
            "window.scrollBy(arguments[0], arguments[1]); return window.scrollX + ',' + window.scrollY;",
            (long)deltaX, (long)deltaY);
        return $"Scrolled to ({pos})";
    }

    [McpServerTool]
    [Description("""
        Scrolls the page until a described element is visible in the center of the viewport.
        Use before ClickAt or TakeScreenshot when the target element is off-screen.
        """)]
    public string ScrollToElement(
        [Description("The session ID returned by StartSession")] string sessionId,
        [Description("Description of the element to scroll into view")] ElementDescriptor element)
    {
        logger.LogDebug("[MCP] ScrollToElement: sessionId={SessionId}, element={ElementName}", sessionId, element.Id ?? element.Name ?? element.InnerText ?? "unknown");
        var driver = sessionManager.GetDriver(sessionId);
        var webElement = Resolve(driver, element);
        ((IJavaScriptExecutor)driver).ExecuteScript(
            "arguments[0].scrollIntoView({behavior: 'instant', block: 'center'});",
            webElement);
        return $"Element scrolled into view. Text: {webElement.Text}";
    }

    [McpServerTool]
    [Description("Returns the list of all currently active browser sessions.")]
    public List<BrowserSession> GetActiveSessions()
    {
        logger.LogDebug("[MCP] GetActiveSessions");
        return sessionManager.GetActiveSessions();
    }

    [McpServerTool]
    [Description("""
        Executes a sequence of element-interaction actions in a single call.
        WHEN TO USE: When you have identified multiple actions to perform on the current page from a prior DOM inspection.
        WHEN NOT TO USE: For navigation, inspection, scroll, screenshot — keep those as individual calls.

        Execution contract:
        - Actions run in order. If one fails, subsequent actions still execute (no early exit).
        - Returns per-action status: check Results[] to identify any failed actions.
        - Failed actions can be retried individually using the corresponding single-action tool.
        - Returns final page size info so you can decide whether to call GetPageHtml or GetAccessibilitySnapshot next.

        Supported ActionTypes: Click | TypeText | SetText | Clear | SelectByText | SelectByValue | Check | Uncheck

        Example batch (fill a form and submit):
        [
          { "ActionType": "SetText",       "Element": { "Name": "firstName" },       "Value": "John",    "Label": "Fill first name" },
          { "ActionType": "SetText",       "Element": { "Name": "lastName" },        "Value": "Doe",     "Label": "Fill last name" },
          { "ActionType": "SelectByText",  "Element": { "Id": "country" },           "Value": "France",  "Label": "Select country" },
          { "ActionType": "Check",         "Element": { "Id": "agreeTerms" },                            "Label": "Accept terms" },
          { "ActionType": "Click",         "Element": { "TagName": "button", "InnerText": "Submit" },    "Label": "Click submit" }
        ]
        """)]
    public BulkOperationResult ExecuteBulkActions(
        [Description("The session ID returned by StartSession")] string sessionId,
        [Description("Ordered list of actions to execute. Use Label to identify each action in results.")] List<BulkAction> actions)
    {
        logger.LogInformation("[MCP BULK] ExecuteBulkActions: sessionId={SessionId}, actionCount={ActionCount}", sessionId, actions.Count);
        var driver = sessionManager.GetDriver(sessionId);
        var results = new List<BulkActionResult>();

        for (int i = 0; i < actions.Count; i++)
        {
            var action = actions[i];
            var result = new BulkActionResult
            {
                Index = i,
                ActionType = action.ActionType,
                Label = action.Label
            };

            try
            {
                var webEl = Resolve(driver, action.Element);
                var logEntry = ActionLogBuilder.Build(driver, action.ActionType, webEl, action.Value, action.Label, driver.Url);
                switch (action.ActionType)
                {
                    case "Click":
                        logger.LogDebug("[MCP BULK {Index}] Click Id={Id} Name={Name} Text={InnerText} Label={Label}", i, action.Element.Id, action.Element.Name, action.Element.InnerText, action.Label);
                        webEl.Click();
                        break;

                    case "TypeText":
                        logger.LogDebug("[MCP BULK {Index}] TypeText value={Value} Id={Id} Name={Name} Label={Label}", i, action.Value, action.Element.Id, action.Element.Name, action.Label);
                        if (action.ClearFirst) webEl.Clear();
                        webEl.SendKeys(action.Value ?? string.Empty);
                        break;

                    case "SetText":
                        logger.LogDebug("[MCP BULK {Index}] SetText value={Value} Id={Id} Name={Name} Label={Label}", i, action.Value, action.Element.Id, action.Element.Name, action.Label);
                        webEl.Clear();
                        webEl.SendKeys(action.Value ?? string.Empty);
                        break;

                    case "Clear":
                        logger.LogDebug("[MCP BULK {Index}] Clear Id={Id} Name={Name} Label={Label}", i, action.Element.Id, action.Element.Name, action.Label);
                        webEl.Clear();
                        break;

                    case "SelectByText":
                        logger.LogDebug("[MCP BULK {Index}] SelectByText value={Value} Id={Id} Name={Name} Label={Label}", i, action.Value, action.Element.Id, action.Element.Name, action.Label);
                        Describe(driver, action.Element).SelectByText(action.Value ?? string.Empty);
                        break;

                    case "SelectByValue":
                        logger.LogDebug("[MCP BULK {Index}] SelectByValue value={Value} Id={Id} Name={Name} Label={Label}", i, action.Value, action.Element.Id, action.Element.Name, action.Label);
                        Describe(driver, action.Element).SelectByValue(action.Value ?? string.Empty);
                        break;

                    case "Check":
                        logger.LogDebug("[MCP BULK {Index}] Check Id={Id} Name={Name} Label={Label}", i, action.Element.Id, action.Element.Name, action.Label);
                        if (!webEl.Selected) webEl.Click();
                        break;

                    case "Uncheck":
                        logger.LogDebug("[MCP BULK {Index}] Uncheck Id={Id} Name={Name} Label={Label}", i, action.Element.Id, action.Element.Name, action.Label);
                        if (webEl.Selected) webEl.Click();
                        break;

                    default:
                        throw new ArgumentException($"Unknown ActionType '{action.ActionType}'. Supported: Click, TypeText, SetText, Clear, SelectByText, SelectByValue, Check, Uncheck.");
                }

                result.Status = "Success";
                result.ElementTag = logEntry.ElementTag;
                sessionManager.AppendActionLog(sessionId, logEntry);
            }
            catch (Exception ex)
            {
                result.Status = "Failed";
                result.ErrorMessage = ex.Message;
                logger.LogWarning("[MCP BULK {Index}] FAILED ActionType={ActionType} Label={Label}: {ErrorMessage}", i, action.ActionType, action.Label, ex.Message);
            }

            results.Add(result);
        }

        var html = HtmlCleaner.CleanPage(driver);
        var htmlSizeKB = System.Text.Encoding.UTF8.GetByteCount(html) / 1024.0;
        sessionManager.SetHtmlCache(sessionId, html, driver.Url);
        var succeeded = results.Count(r => r.Status == "Success");
        var failed = results.Count(r => r.Status == "Failed");

        logger.LogInformation("[MCP BULK] Done: succeeded={Succeeded}, failed={Failed}, url={Url}, htmlSizeKB={HtmlSizeKB:F1}", succeeded, failed, driver.Url, htmlSizeKB);

        return new BulkOperationResult
        {
            Results = results,
            AllSucceeded = failed == 0,
            SucceededCount = succeeded,
            FailedCount = failed,
            FinalUrl = driver.Url,
            HtmlSizeKB = Math.Round(htmlSizeKB, 1),
            SnapshotSizeKB = 0,
            RecommendedInspectionMethod = htmlSizeKB < 30 ? "GetPageHtml" : "GetPageHtmlChunk or GetAccessibilitySnapshot"
        };
    }

    [McpServerTool]
    [Description("Closes a browser session and releases all its resources. Always call this when finished. Returns the path of the saved action log file (if any actions were performed), which can be shared with the user for review or test script generation.")]
    public string CloseSession(
        [Description("The session ID to close")] string sessionId)
    {
        logger.LogInformation("[MCP] CloseSession: sessionId={SessionId}", sessionId);

        string? logPath = null;
        var log = sessionManager.GetActionLog(sessionId);
        if (log.Count > 0)
        {
            logPath = Path.Combine(
                Path.GetTempPath(),
                $"selenium-actions-{sessionId}-{DateTime.UtcNow:yyyyMMddHHmmss}.json");
            var json = System.Text.Json.JsonSerializer.Serialize(log,
                new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(logPath, json);
            logger.LogInformation("[MCP] Action log saved: {Count} entries → {Path}", log.Count, logPath);
        }

        var closed = sessionManager.CloseSession(sessionId);
        if (!closed)
            return "Session not found or already closed.";

        return logPath != null
            ? $"Session closed. Action log ({log.Count} entries) saved to: {logPath}\nYou can open this file to review all recorded element tags and locators, or use it to generate page object models and test scripts."
            : "Session closed. No actions were recorded.";
    }

    // Builds a WebElementDescription from an LLM-supplied ElementDescriptor.
    private static WebElementDescription Describe(IWebDriver driver, ElementDescriptor d)
    {
        var desc = new WebElementDescription((OpenQA.Selenium.WebDriver)driver);
        // Ref (data-mcp-ref) takes priority — it was set on the live DOM by GetAccessibilitySnapshot
        if (d.Ref != null)
        {
            desc.CssSelector = $"[data-mcp-ref='{d.Ref}']";
            return desc;
        }
        if (d.Id != null) desc.Id = d.Id;
        if (d.Name != null) desc.Name = d.Name;
        if (d.TagName != null) desc.TagName = d.TagName;
        if (d.InnerText != null) desc.InnerText = d.InnerText;
        if (d.LinkText != null) desc.LinkText = d.LinkText;
        if (d.ClassName != null) desc.ClassName = d.ClassName;
        if (d.CssSelector != null) desc.CssSelector = d.CssSelector;
        if (d.XPath != null) desc.XPath = d.XPath;
        if (d.AriaLabel != null)
            desc.Attributes = [new HtmlAttribute("aria-label", d.AriaLabel)];
        return desc;
    }

    // Finds the element at descriptor.Index, waiting up to timeoutSeconds if provided.
    private static IWebElement Resolve(IWebDriver driver, ElementDescriptor descriptor, int? timeoutSeconds = null)
    {
        var desc = Describe(driver, descriptor);
        var all = timeoutSeconds.HasValue
            ? desc.FindElements(timeoutSeconds.Value)
            : desc.FindElements();
        if (descriptor.Index >= all.Count)
            throw new InvalidOperationException(
                $"Index {descriptor.Index} is out of range — {all.Count} element(s) matched.");
        return all.ElementAt(descriptor.Index);
    }

    // Returns compact state info after an action (HTML size only, HTML cached for GetPageHtmlChunk).
    // elementTag: the opening HTML tag of the resolved element, included so the LLM can use it for page object / test script generation.
    private string GetCompactStateAfterAction(IWebDriver driver, string sessionId, string? elementTag = null)
    {
        var html = HtmlCleaner.CleanPage(driver);
        var htmlSizeKB = System.Text.Encoding.UTF8.GetByteCount(html) / 1024.0;
        sessionManager.SetHtmlCache(sessionId, html, driver.Url);

        logger.LogDebug("[MCP RESULT] url={Url}, htmlSizeKB={HtmlSizeKB:F1}", driver.Url, htmlSizeKB);

        var elementLine = elementTag != null ? $"\nElement tag: {elementTag}" : string.Empty;
        return $"Action completed. Current URL: {driver.Url}{elementLine}\n" +
               $"Page HTML: {htmlSizeKB:F1}KB — " +
               (htmlSizeKB < 30
                   ? "use GetPageHtml() to read the page."
                   : $"page is large; use GetPageHtmlChunk(chunkIndex:0) to read in slices or GetAccessibilitySnapshot() for element IDs.");
    }
}
