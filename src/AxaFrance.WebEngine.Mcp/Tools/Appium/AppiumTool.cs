using AxaFrance.WebEngine.MobileApp;
using AxaFrance.WebEngine.Mcp.Tools.Appium.Models;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.Android.Enums;
using OpenQA.Selenium.Appium.iOS;
using OpenQA.Selenium.Interactions;
using System.ComponentModel;

namespace AxaFrance.WebEngine.Mcp.Tools.Appium;

[McpServerToolType]
public sealed class AppiumTool(AppiumSessionManager sessionManager, ILogger<AppiumTool> logger)
{
    // ─── Session management ────────────────────────────────────────────────────

    [McpServerTool]
    [Description("""
        Starts a new Appium session by connecting to a real device or emulator.
        Returns a sessionId that must be passed to all subsequent Appium calls.

        ── RECOMMENDED WORKFLOW ──────────────────────────────────────────────────
        1. StartSession        — obtain sessionId
        2. GetAccessibilitySnapshot — compact interactive-element listing (~5-30 KB)
           • Use accId / id / text from snapshot to build element descriptors
           • If screen is complex (>60 KB source), use GetPageSourceChunk instead
        3. ExecuteBulkActions  — submit ALL actions you can derive from the snapshot
           in one call (tap button, fill text, select toggle, etc.)
           • Only failed actions are returned — retry those individually
           • After bulk, call GetAccessibilitySnapshot again if UI may have changed
        4. Repeat steps 2-3 until the scenario is complete
        5. CloseSession        — always close when done

        LOCATOR PRIORITY (highest → lowest):
          IosClassChain / UIAutomatorSelector / IosPredicate > AccessibilityId > Id > Text > XPath

        ─────────────────────────────────────────────────────────────────────────
        Examples:
          Android emulator: StartSession(platform:'Android', appPath:'/path/to/app.apk', deviceName:'emulator-5554')
          iOS simulator:    StartSession(platform:'iOS',     appPath:'/path/to/app.app', deviceName:'iPhone 15')
          Remote grid:      StartSession(platform:'Android', appPath:'bs://abc123',      deviceName:'Google Pixel 7',
                                         serverUrl:'https://hub.browserstack.com/wd/hub')
        """)]
    public AppiumSession StartSession(
        [Description("Mobile platform: 'Android' or 'iOS'")] string platform,
        [Description("Local path to APK/IPA, or cloud app reference (e.g. bs://appId for BrowserStack)")] string appPath,
        [Description("Device name, UDID, or emulator name (e.g. 'emulator-5554', 'iPhone 15 Pro')")] string deviceName,
        [Description("OS version, e.g. '14.0'. Leave empty to use the device default.")] string? osVersion = null,
        [Description("Appium server URL. Leave empty to use the configured default (http://localhost:4723/wd).")] string? serverUrl = null)
    {
        logger.LogInformation(
            "[MCP] Appium.StartSession: platform={Platform}, device={Device}, app={App}",
            platform, deviceName, appPath);
        return sessionManager.StartSession(platform, appPath, deviceName, osVersion, serverUrl);
    }

    [McpServerTool]
    [Description("Returns the list of all currently active Appium sessions.")]
    public List<AppiumSession> GetActiveSessions()
    {
        logger.LogDebug("[MCP] Appium.GetActiveSessions");
        return sessionManager.GetActiveSessions();
    }

    [McpServerTool]
    [Description("Closes an Appium session and releases the device. Always call this when finished. Returns the path of the saved action log file (if any actions were performed), which can be shared with the user for review or test script generation.")]
    public string CloseSession(
        [Description("The session ID returned by StartSession")] string sessionId)
    {
        logger.LogInformation("[MCP] Appium.CloseSession: sessionId={SessionId}", sessionId);

        string? logPath = null;
        var log = sessionManager.GetActionLog(sessionId);
        if (log.Count > 0)
        {
            logPath = Path.Combine(
                Path.GetTempPath(),
                $"appium-actions-{sessionId}-{DateTime.UtcNow:yyyyMMddHHmmss}.txt");
            File.WriteAllLines(logPath, log);
            logger.LogInformation("[MCP] Appium action log saved: {Count} entries → {Path}", log.Count, logPath);
        }

        var closed = sessionManager.CloseSession(sessionId);
        if (!closed)
            return "Session not found or already closed.";

        return logPath != null
            ? $"Session closed. Action log ({log.Count} entries) saved to: {logPath}\nYou can open this file to review all recorded element tags and locators, or use it to generate page object models and test scripts."
            : "Session closed. No actions were recorded.";
    }

    // ─── Screen inspection ─────────────────────────────────────────────────────

    [McpServerTool]
    [Description("""
        Returns a compact, one-line-per-element snapshot of the current screen.
        Only actionable or informative elements are listed (Buttons, Inputs, Text, Checkboxes, etc.).
        Container nodes are omitted; the output is typically 70-90% smaller than the raw XML source.

        Output format (one line per element):
          [Role] accId="…" id="…" text="…" hint="…" [state] bounds=[…]

        HOW TO USE:
          • accId maps to AccessibilityId (best cross-platform locator)
          • id   maps to the short resource-id (without package prefix)
          • text maps to the element's visible label or text attribute
          • Always prefer accId > id > text when building an AppElementDescriptor

        WHEN TO USE: First-pass screen inspection for any screen. Call before planning actions.
        ALTERNATIVE: GetPageSourceChunk for raw XML when you need attribute details not in the snapshot.
        """)]
    public string GetAccessibilitySnapshot(
        [Description("The session ID returned by StartSession")] string sessionId)
    {
        logger.LogInformation("[MCP] Appium.GetAccessibilitySnapshot: sessionId={SessionId}", sessionId);
        var source = sessionManager.GetDriver(sessionId).PageSource;
        sessionManager.SetSourceCache(sessionId, source);
        return AppSourceCleaner.Compact(source);
    }

    [McpServerTool]
    [Description("""
        Returns the full XML page source of the current screen (Appium UI hierarchy).
        Use this when you need raw attribute data not available in GetAccessibilitySnapshot.

        TIP: Page source can be large (>100 KB for complex screens). If so, use GetPageSourceChunk()
        to read in 15 KB slices, or GetAccessibilitySnapshot() for a faster compact overview.
        """)]
    public string GetPageSource(
        [Description("The session ID returned by StartSession")] string sessionId)
    {
        logger.LogInformation("[MCP] Appium.GetPageSource: sessionId={SessionId}", sessionId);
        var source = sessionManager.GetDriver(sessionId).PageSource;
        sessionManager.SetSourceCache(sessionId, source);
        return source;
    }

    [McpServerTool]
    [Description("""
        Returns a 15 KB slice of the current screen's XML page source.
        Fetch chunk 0 first — the response includes TotalChunks so you know how many slices exist.
        The source is cached after GetPageSource or GetAccessibilitySnapshot, so chunk calls are cheap.
        WHEN TO USE: When GetPageSource would return a very large XML (complex screens, long lists).
        """)]
    public PageSourceChunk GetPageSourceChunk(
        [Description("The session ID returned by StartSession")] string sessionId,
        [Description("Zero-based chunk index. Start at 0 to learn TotalChunks.")] int chunkIndex = 0,
        [Description("Chunk size in KB (default: 15)")] int chunkSizeKB = 15)
    {
        logger.LogDebug(
            "[MCP] Appium.GetPageSourceChunk: sessionId={SessionId}, chunk={ChunkIndex}, size={ChunkSizeKB}KB",
            sessionId, chunkIndex, chunkSizeKB);

        var driver = sessionManager.GetDriver(sessionId);
        var cached = sessionManager.GetSourceCache(sessionId);
        var source = cached?.Source ?? driver.PageSource;
        if (cached is null) sessionManager.SetSourceCache(sessionId, source);

        var chunkBytes = chunkSizeKB * 1024;
        var totalChunks = (int)Math.Ceiling((double)source.Length / chunkBytes);
        var start = chunkIndex * chunkBytes;

        if (start >= source.Length)
            return new PageSourceChunk
            {
                SessionId = sessionId, ChunkIndex = chunkIndex, TotalChunks = totalChunks,
                TotalSizeKB = Math.Round(source.Length / 1024.0, 1),
                ChunkSizeKB = chunkSizeKB, Content = string.Empty
            };

        return new PageSourceChunk
        {
            SessionId = sessionId,
            ChunkIndex = chunkIndex,
            TotalChunks = totalChunks,
            TotalSizeKB = Math.Round(source.Length / 1024.0, 1),
            ChunkSizeKB = chunkSizeKB,
            Content = source.Substring(start, Math.Min(chunkBytes, source.Length - start))
        };
    }

    [McpServerTool]
    [Description("Takes a screenshot of the current device screen and returns it as a base64-encoded PNG string.")]
    public string TakeScreenshot(
        [Description("The session ID returned by StartSession")] string sessionId)
    {
        logger.LogDebug("[MCP] Appium.TakeScreenshot: sessionId={SessionId}", sessionId);
        return ((ITakesScreenshot)sessionManager.GetDriver(sessionId)).GetScreenshot().AsBase64EncodedString;
    }

    [McpServerTool]
    [Description("Returns the visible text of a mobile element.")]
    public string GetElementText(
        [Description("The session ID returned by StartSession")] string sessionId,
        [Description("Element locator — prefer AccessibilityId, Id, or Text")] AppElementDescriptor element)
    {
        logger.LogDebug(
            "[MCP] Appium.GetElementText: sessionId={SessionId}, element={Element}",
            sessionId, element.AccessibilityId ?? element.Id ?? element.Text ?? "?");
        return Resolve(sessionManager.GetDriver(sessionId), element).Text;
    }

    // ─── Element interaction ───────────────────────────────────────────────────

    [McpServerTool]
    [Description("""
        Taps (clicks) a mobile element and returns a compact screen state.
        LOCATOR PRIORITY: AccessibilityId > Id > IosClassChain/UIAutomatorSelector > XPath.
        Example: { AccessibilityId: 'login_button' }
        """)]
    public string TapElement(
        [Description("The session ID returned by StartSession")] string sessionId,
        [Description("Element to tap — use AccessibilityId, Id, or Text from the accessibility snapshot")] AppElementDescriptor element)
    {
        logger.LogInformation(
            "[MCP ACTION] Appium.TapElement: sessionId={SessionId}, accId={AccId}, id={Id}, text={Text}",
            sessionId, element.AccessibilityId, element.Id, element.Text);
        var driver = sessionManager.GetDriver(sessionId);
        var webElement = Resolve(driver, element);
        var elementTag = GetElementXmlTag(webElement);
        webElement.Click();
        sessionManager.AppendActionLog(sessionId, $"Tap accId={element.AccessibilityId ?? element.Id ?? element.Text} tag={elementTag}");
        return GetCompactState(driver, sessionId, elementTag);
    }

    [McpServerTool]
    [Description("""
        Long-presses a mobile element (useful for context menus, drag handles).
        Returns a compact screen state after the gesture.
        """)]
    public string LongPressElement(
        [Description("The session ID returned by StartSession")] string sessionId,
        [Description("Element to long-press")] AppElementDescriptor element,
        [Description("Duration of the press in milliseconds (default: 1500)")] int durationMs = 1500)
    {
        logger.LogInformation(
            "[MCP ACTION] Appium.LongPressElement: sessionId={SessionId}, accId={AccId}, durationMs={Duration}",
            sessionId, element.AccessibilityId ?? element.Id, durationMs);
        var driver = sessionManager.GetDriver(sessionId);
        var webElement = Resolve(driver, element);
        var elementTag = GetElementXmlTag(webElement);
        var cx = webElement.Location.X + webElement.Size.Width / 2;
        var cy = webElement.Location.Y + webElement.Size.Height / 2;
        PerformLongPress(driver, cx, cy, durationMs);
        sessionManager.AppendActionLog(sessionId, $"LongPress accId={element.AccessibilityId ?? element.Id} durationMs={durationMs} tag={elementTag}");
        return GetCompactState(driver, sessionId, elementTag);
    }

    [McpServerTool]
    [Description("""
        Types text into a focusable mobile element (text field, search bar).
        Returns a compact screen state after typing.
        """)]
    public string TypeText(
        [Description("The session ID returned by StartSession")] string sessionId,
        [Description("Input element — use AccessibilityId or Id")] AppElementDescriptor element,
        [Description("Text to enter")] string text,
        [Description("Clear the field before typing (default: true)")] bool clearFirst = true)
    {
        logger.LogInformation(
            "[MCP ACTION] Appium.TypeText: sessionId={SessionId}, text={Text}, accId={AccId}, clearFirst={ClearFirst}",
            sessionId, text, element.AccessibilityId ?? element.Id, clearFirst);
        var driver = sessionManager.GetDriver(sessionId);
        var webElement = Resolve(driver, element);
        var elementTag = GetElementXmlTag(webElement);
        if (clearFirst) webElement.Clear();
        webElement.SendKeys(text);
        sessionManager.AppendActionLog(sessionId, $"TypeText accId={element.AccessibilityId ?? element.Id} value={text} tag={elementTag}");
        return GetCompactState(driver, sessionId, elementTag);
    }

    [McpServerTool]
    [Description("Clears the text from a mobile input field.")]
    public string ClearText(
        [Description("The session ID returned by StartSession")] string sessionId,
        [Description("Input element to clear")] AppElementDescriptor element)
    {
        logger.LogInformation(
            "[MCP ACTION] Appium.ClearText: sessionId={SessionId}, accId={AccId}",
            sessionId, element.AccessibilityId ?? element.Id);
        var driver = sessionManager.GetDriver(sessionId);
        Resolve(driver, element).Clear();
        return "Text cleared.";
    }

    [McpServerTool]
    [Description("""
        Waits until a mobile element appears on screen, up to a timeout in seconds.
        Returns the element text when found.
        """)]
    public string WaitForElement(
        [Description("The session ID returned by StartSession")] string sessionId,
        [Description("Element to wait for")] AppElementDescriptor element,
        [Description("Maximum wait time in seconds (default: 10)")] int timeoutSeconds = 10)
    {
        logger.LogInformation(
            "[MCP] Appium.WaitForElement: sessionId={SessionId}, accId={AccId}, timeout={Timeout}s",
            sessionId, element.AccessibilityId ?? element.Id ?? element.Text, timeoutSeconds);
        var driver = sessionManager.GetDriver(sessionId);
        var found = Resolve(driver, element, timeoutSeconds);
        return $"Element found. Text: {found.Text}";
    }

    // ─── Gestures ──────────────────────────────────────────────────────────────

    [McpServerTool]
    [Description("""
        Performs a swipe gesture on the screen in the specified direction.
        Useful for scrolling lists, carousels, or dismissing drawers.
        Returns a compact screen state after the gesture.

        Directions: Up | Down | Left | Right
        TIP: 'Up' scrolls the content upward (reveals content below), 'Down' reveals content above.
        """)]
    public string SwipeScreen(
        [Description("The session ID returned by StartSession")] string sessionId,
        [Description("Swipe direction: Up, Down, Left, or Right")] string direction,
        [Description("Swipe animation duration in milliseconds (default: 600)")] int durationMs = 600)
    {
        logger.LogInformation("[MCP ACTION] Appium.SwipeScreen: sessionId={SessionId}, direction={Direction}",
            sessionId, direction);
        var driver = sessionManager.GetDriver(sessionId);
        PerformSwipe(driver, direction, durationMs);
        sessionManager.AppendActionLog(sessionId, $"Swipe direction={direction}");
        return GetCompactState(driver, sessionId);
    }

    [McpServerTool]
    [Description("""
        Presses the hardware Back button — Android only.
        On iOS, use TapElement to tap the back navigation button visible in the page source.
        """)]
    public string PressBack(
        [Description("The session ID returned by StartSession")] string sessionId)
    {
        logger.LogInformation("[MCP ACTION] Appium.PressBack: sessionId={SessionId}", sessionId);
        var driver = sessionManager.GetDriver(sessionId);
        var platform = sessionManager.GetPlatform(sessionId);

        if (platform.Equals("Android", StringComparison.OrdinalIgnoreCase))
            ((AndroidDriver)driver).PressKeyCode(AndroidKeyCode.Back);
        else
            driver.Navigate().Back();

        sessionManager.AppendActionLog(sessionId, "PressBack");
        return GetCompactState(driver, sessionId);
    }

    [McpServerTool]
    [Description("Hides the software keyboard if it is currently displayed.")]
    public string HideKeyboard(
        [Description("The session ID returned by StartSession")] string sessionId)
    {
        logger.LogInformation("[MCP ACTION] Appium.HideKeyboard: sessionId={SessionId}", sessionId);
        var driver = sessionManager.GetDriver(sessionId);
        try
        {
            driver.HideKeyboard();
            return "Keyboard hidden.";
        }
        catch (Exception ex)
        {
            return $"Keyboard may already be hidden: {ex.Message}";
        }
    }

    // ─── Bulk actions ──────────────────────────────────────────────────────────

    [McpServerTool]
    [Description("""
        Executes a sequence of mobile interaction actions in a single call.
        WHEN TO USE: After GetAccessibilitySnapshot identified the elements you need — batch ALL
        actions for the current screen into one call to avoid repeated round-trips.
        WHEN NOT TO USE: For swipe-to-scroll followed by inspection — split those manually.

        Execution contract:
        - Actions run in order. If one fails, subsequent actions still execute (no early exit).
        - Returns per-action status: check Results[] to identify failed actions.
        - Failed actions can be retried individually using the corresponding single-action tool.
        - Returns final page source size so you know whether to call GetAccessibilitySnapshot next.

        Supported ActionTypes:
          Tap        — taps an element
          TypeText   — types text (optional ClearFirst, default true)
          SetText    — clears then sets text
          Clear      — clears a text field
          LongPress  — long-presses an element (DurationMs default 1500)
          SwipeUp / SwipeDown / SwipeLeft / SwipeRight — swipe gestures (no element needed)

        Example batch (login screen):
        [
          { "ActionType": "SetText",  "Element": { "AccessibilityId": "email_field" },    "Value": "user@example.com", "Label": "Enter email" },
          { "ActionType": "SetText",  "Element": { "AccessibilityId": "password_field" }, "Value": "secret",           "Label": "Enter password" },
          { "ActionType": "Tap",      "Element": { "AccessibilityId": "login_button" },                                "Label": "Tap login" }
        ]
        """)]
    public AppBulkOperationResult ExecuteBulkActions(
        [Description("The session ID returned by StartSession")] string sessionId,
        [Description("Ordered list of actions to execute. Use Label to identify each action in results.")] List<AppBulkAction> actions)
    {
        logger.LogInformation("[MCP BULK] Appium.ExecuteBulkActions: sessionId={SessionId}, actionCount={ActionCount}",
            sessionId, actions.Count);
        var driver = sessionManager.GetDriver(sessionId);
        var results = new List<AppBulkActionResult>();

        for (int i = 0; i < actions.Count; i++)
        {
            var action = actions[i];
            var result = new AppBulkActionResult
            {
                Index = i,
                ActionType = action.ActionType,
                Label = action.Label
            };

            try
            {
                string? elementTag = null;

                switch (action.ActionType)
                {
                    case "Tap":
                    {
                        logger.LogDebug("[MCP BULK {Index}] Tap accId={AccId} id={Id} Label={Label}",
                            i, action.Element.AccessibilityId, action.Element.Id, action.Label);
                        var el = Resolve(driver, action.Element);
                        elementTag = GetElementXmlTag(el);
                        el.Click();
                        break;
                    }
                    case "TypeText":
                    {
                        logger.LogDebug("[MCP BULK {Index}] TypeText value={Value} accId={AccId} Label={Label}",
                            i, action.Value, action.Element.AccessibilityId, action.Label);
                        var el = Resolve(driver, action.Element);
                        elementTag = GetElementXmlTag(el);
                        if (action.ClearFirst) el.Clear();
                        el.SendKeys(action.Value ?? string.Empty);
                        break;
                    }
                    case "SetText":
                    {
                        logger.LogDebug("[MCP BULK {Index}] SetText value={Value} accId={AccId} Label={Label}",
                            i, action.Value, action.Element.AccessibilityId, action.Label);
                        var el = Resolve(driver, action.Element);
                        elementTag = GetElementXmlTag(el);
                        el.Clear();
                        el.SendKeys(action.Value ?? string.Empty);
                        break;
                    }
                    case "Clear":
                    {
                        logger.LogDebug("[MCP BULK {Index}] Clear accId={AccId} Label={Label}",
                            i, action.Element.AccessibilityId, action.Label);
                        var el = Resolve(driver, action.Element);
                        elementTag = GetElementXmlTag(el);
                        el.Clear();
                        break;
                    }
                    case "LongPress":
                    {
                        logger.LogDebug("[MCP BULK {Index}] LongPress accId={AccId} durationMs={DurationMs} Label={Label}",
                            i, action.Element.AccessibilityId, action.DurationMs, action.Label);
                        var el = Resolve(driver, action.Element);
                        elementTag = GetElementXmlTag(el);
                        var cx = el.Location.X + el.Size.Width / 2;
                        var cy = el.Location.Y + el.Size.Height / 2;
                        PerformLongPress(driver, cx, cy, action.DurationMs);
                        break;
                    }
                    case "SwipeUp":
                    case "SwipeDown":
                    case "SwipeLeft":
                    case "SwipeRight":
                    {
                        var dir = action.ActionType["Swipe".Length..];
                        logger.LogDebug("[MCP BULK {Index}] Swipe direction={Dir} Label={Label}", i, dir, action.Label);
                        PerformSwipe(driver, dir, 600);
                        break;
                    }
                    default:
                        throw new ArgumentException(
                            $"Unknown ActionType '{action.ActionType}'. Supported: Tap, TypeText, SetText, Clear, LongPress, SwipeUp, SwipeDown, SwipeLeft, SwipeRight.");
                }

                result.Status = "Success";
                result.ElementTag = elementTag;
                sessionManager.AppendActionLog(sessionId,
                    $"BULK[{i}] {action.ActionType} accId={action.Element.AccessibilityId ?? action.Element.Id} label={action.Label}" +
                    (elementTag != null ? $" tag={elementTag}" : string.Empty));
            }
            catch (Exception ex)
            {
                result.Status = "Failed";
                result.ErrorMessage = ex.Message;
                logger.LogWarning("[MCP BULK {Index}] FAILED ActionType={ActionType} Label={Label}: {ErrorMessage}",
                    i, action.ActionType, action.Label, ex.Message);
            }

            results.Add(result);
        }

        string source;
        try { source = driver.PageSource; }
        catch { source = string.Empty; }

        if (!string.IsNullOrEmpty(source))
            sessionManager.SetSourceCache(sessionId, source);

        var sizeKB = string.IsNullOrEmpty(source) ? 0 : source.Length / 1024.0;
        var succeeded = results.Count(r => r.Status == "Success");
        var failed = results.Count(r => r.Status == "Failed");

        logger.LogInformation("[MCP BULK] Appium done: succeeded={Succeeded}, failed={Failed}, sourceSizeKB={SizeKB:F1}",
            succeeded, failed, sizeKB);

        return new AppBulkOperationResult
        {
            Results = results,
            AllSucceeded = failed == 0,
            SucceededCount = succeeded,
            FailedCount = failed,
            SourceSizeKB = Math.Round(sizeKB, 1),
            RecommendedInspectionMethod = sizeKB < 60
                ? "GetAccessibilitySnapshot"
                : "GetPageSourceChunk(chunkIndex:0)"
        };
    }

    // ─── Helpers ───────────────────────────────────────────────────────────────

    private static AppElementDescription Describe(AppiumDriver driver, AppElementDescriptor d)
    {
        var desc = new AppElementDescription(driver);
        if (d.Id != null) desc.Id = d.Id;
        if (d.AccessibilityId != null) desc.AccessbilityId = d.AccessibilityId;
        if (d.Name != null) desc.Name = d.Name;
        if (d.ClassName != null) desc.ClassName = d.ClassName;
        if (d.Text != null) desc.Text = d.Text;
        if (d.XPath != null) desc.XPath = d.XPath;
        if (d.ContentDescription != null) desc.ContentDescription = d.ContentDescription;
        if (d.IosClassChain != null) desc.IosClassChain = d.IosClassChain;
        if (d.UIAutomatorSelector != null) desc.UIAutomatorSelector = d.UIAutomatorSelector;
        if (d.IosPredicate != null) desc.IosPredicate = d.IosPredicate;
        return desc;
    }

    private static IWebElement Resolve(AppiumDriver driver, AppElementDescriptor d, int? timeoutSeconds = null)
    {
        var desc = Describe(driver, d);
        var all = timeoutSeconds.HasValue
            ? desc.FindElements(timeoutSeconds.Value)
            : desc.FindElements();
        if (d.Index >= all.Count)
            throw new InvalidOperationException(
                $"Index {d.Index} is out of range — {all.Count} element(s) matched.");
        return all.ElementAt(d.Index);
    }

    private static void PerformSwipe(AppiumDriver driver, string direction, int durationMs)
    {
        var size = driver.Manage().Window.Size;
        int startX, startY, endX, endY;

        switch (direction.Trim().ToUpperInvariant())
        {
            case "UP":
                startX = endX = size.Width / 2;
                startY = (int)(size.Height * 0.75);
                endY = (int)(size.Height * 0.25);
                break;
            case "DOWN":
                startX = endX = size.Width / 2;
                startY = (int)(size.Height * 0.25);
                endY = (int)(size.Height * 0.75);
                break;
            case "LEFT":
                startY = endY = size.Height / 2;
                startX = (int)(size.Width * 0.75);
                endX = (int)(size.Width * 0.25);
                break;
            case "RIGHT":
                startY = endY = size.Height / 2;
                startX = (int)(size.Width * 0.25);
                endX = (int)(size.Width * 0.75);
                break;
            default:
                throw new ArgumentException(
                    $"Unknown swipe direction '{direction}'. Use: Up, Down, Left, Right.");
        }

        var finger = new PointerInputDevice(PointerKind.Touch, "finger");
        var seq = new ActionSequence(finger, 0);
        seq.AddAction(finger.CreatePointerMove(CoordinateOrigin.Viewport, startX, startY, TimeSpan.Zero));
        seq.AddAction(finger.CreatePointerDown(MouseButton.Left));
        seq.AddAction(finger.CreatePointerMove(CoordinateOrigin.Viewport, endX, endY, TimeSpan.FromMilliseconds(durationMs)));
        seq.AddAction(finger.CreatePointerUp(MouseButton.Left));
        driver.PerformActions([seq]);
    }

    private static void PerformLongPress(AppiumDriver driver, int x, int y, int durationMs)
    {
        var finger = new PointerInputDevice(PointerKind.Touch, "finger");
        var seq = new ActionSequence(finger, 0);
        seq.AddAction(finger.CreatePointerMove(CoordinateOrigin.Viewport, x, y, TimeSpan.Zero));
        seq.AddAction(finger.CreatePointerDown(MouseButton.Left));
        seq.AddAction(finger.CreatePause(TimeSpan.FromMilliseconds(durationMs)));
        seq.AddAction(finger.CreatePointerUp(MouseButton.Left));
        driver.PerformActions([seq]);
    }

    /// <summary>
    /// Builds an XML-like opening tag string from a resolved Appium element's attributes.
    /// Safe attributes (non-throwing) are sampled; missing ones are skipped.
    /// </summary>
    private static string GetElementXmlTag(IWebElement element)
    {
        var tag = element.TagName ?? "element";
        var parts = new System.Text.StringBuilder();
        parts.Append('<').Append(tag);

        string?[] attribs = ["resource-id", "content-desc", "text", "class", "name", "label", "value", "bounds", "enabled", "focusable", "clickable"];
        foreach (var attr in attribs)
        {
            try
            {
                var val = element.GetAttribute(attr);
                if (!string.IsNullOrEmpty(val))
                    parts.Append(' ').Append(attr).Append('=').Append('"').Append(val).Append('"');
            }
            catch { /* attribute not present on this platform */ }
        }

        parts.Append(" />");
        return parts.ToString();
    }

    private string GetCompactState(AppiumDriver driver, string sessionId, string? elementTag = null)
    {
        string sourceSize;
        try
        {
            var source = driver.PageSource;
            sessionManager.SetSourceCache(sessionId, source);
            var sizeKB = source.Length / 1024.0;
            sourceSize = sizeKB < 60
                ? $"{sizeKB:F1}KB — call GetAccessibilitySnapshot() to inspect the screen."
                : $"{sizeKB:F1}KB — source is large; use GetAccessibilitySnapshot() for a compact overview or GetPageSourceChunk(chunkIndex:0) for raw XML.";
        }
        catch
        {
            sourceSize = "unavailable";
        }

        logger.LogDebug("[MCP RESULT] Appium action completed, sessionId={SessionId}", sessionId);
        var elementLine = elementTag != null ? $"\nElement tag: {elementTag}" : string.Empty;
        return $"Action completed.{elementLine}\nPage source: {sourceSize}";
    }
}
