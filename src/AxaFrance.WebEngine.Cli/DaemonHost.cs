using System.IO.Pipes;
using System.Text.Json;
using OpenQA.Selenium;

namespace AxaFrance.WebEngine.Cli;

internal sealed class DaemonHost : IDisposable
{
    private readonly string _pipeName;
    private readonly DateTimeOffset _startedAtUtc = DateTimeOffset.UtcNow;
    private readonly CancellationTokenSource _stopRequested = new();

    public DaemonHost(string pipeName)
    {
        _pipeName = pipeName;
    }

    public async Task RunAsync(CancellationToken cancellationToken, bool quiet)
    {
        using var linkedCancellation = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken,
            _stopRequested.Token);
        using var automation = new WebAutomationService();

        if (!quiet)
            Console.Error.WriteLine($"WebEngine daemon listening on pipe '{_pipeName}'.");

        try
        {
            while (!linkedCancellation.IsCancellationRequested)
            {
                await using var server = new NamedPipeServerStream(
                    _pipeName,
                    PipeDirection.InOut,
                    1,
                    PipeTransmissionMode.Byte,
                    PipeOptions.Asynchronous);

                try
                {
                    await server.WaitForConnectionAsync(linkedCancellation.Token);
                }
                catch (Exception ex) when (
                    !linkedCancellation.IsCancellationRequested
                    && ex is IOException or ObjectDisposedException)
                {
                    Console.Error.WriteLine($"WebEngine daemon pipe connection failed: {ex.Message}");
                    continue;
                }

                await HandleConnectionAsync(server, linkedCancellation.Token, automation);
            }
        }
        catch (OperationCanceledException) when (linkedCancellation.IsCancellationRequested)
        {
        }
        finally
        {
            if (!quiet)
                Console.Error.WriteLine("WebEngine daemon stopped.");
        }
    }

    private async Task HandleConnectionAsync(
        NamedPipeServerStream server,
        CancellationToken cancellationToken,
        WebAutomationService automation)
    {
        using var reader = new StreamReader(server);
        using var writer = new StreamWriter(server) { AutoFlush = true };

        while (!cancellationToken.IsCancellationRequested)
        {
            string? line;
            try
            {
                line = await reader.ReadLineAsync(cancellationToken);
            }
            catch (Exception ex) when (ex is IOException or ObjectDisposedException)
            {
                return;
            }

            if (line is null)
                return;

            DaemonResponse response;
            try
            {
                var request = JsonSerializer.Deserialize<DaemonRequest>(
                    line,
                    DaemonProtocol.JsonOptions);

                response = request is null
                    ? DaemonProtocol.Error(string.Empty, "invalid_request", "The request body is empty.")
                    : HandleRequest(request, automation);
            }
            catch (JsonException ex)
            {
                response = DaemonProtocol.Error(
                    string.Empty,
                    "invalid_json",
                    $"The request is not valid JSON: {ex.Message}");
            }

            try
            {
                await writer.WriteLineAsync(JsonSerializer.Serialize(response, DaemonProtocol.JsonOptions));
            }
            catch (Exception ex) when (ex is IOException or ObjectDisposedException)
            {
                return;
            }

            if (_stopRequested.IsCancellationRequested)
                return;
        }
    }

    private DaemonResponse HandleRequest(DaemonRequest request, WebAutomationService automation)
    {
        var requestId = string.IsNullOrWhiteSpace(request.RequestId)
            ? Guid.NewGuid().ToString("N")
            : request.RequestId;

        try
        {
            return request.Command.ToLowerInvariant() switch
            {
                "daemon.ping" => DaemonProtocol.Success(
                    requestId,
                    new DaemonStatus(
                        DaemonProtocol.Version,
                        Environment.ProcessId,
                        _startedAtUtc,
                        _pipeName)),
                "daemon.shutdown" => Shutdown(requestId),
                "web.session.open" => DaemonProtocol.Success(
                    requestId,
                    automation.OpenSession(ReadArguments<SessionOpenArguments>(request))),
                "web.session.list" => DaemonProtocol.Success(
                    requestId,
                    automation.ListSessions()),
                "web.session.close" => DaemonProtocol.Success(
                    requestId,
                    automation.CloseSession(ReadArguments<SessionIdArguments>(request).SessionId)),
                "web.navigate" => DaemonProtocol.Success(
                    requestId,
                    automation.Navigate(ReadArguments<NavigateArguments>(request))),
                "web.inspect" => DaemonProtocol.Success(
                    requestId,
                    automation.Inspect(ReadArguments<InspectArguments>(request))),
                "web.html" => DaemonProtocol.Success(
                    requestId,
                    automation.GetHtml(ReadArguments<SessionIdArguments>(request).SessionId)),
                "web.click" => DaemonProtocol.Success(
                    requestId,
                    automation.Click(ReadArguments<ElementActionArguments>(request))),
                "web.type" => DaemonProtocol.Success(
                    requestId,
                    automation.Type(ReadArguments<TypeArguments>(request))),
                "web.key" => DaemonProtocol.Success(
                    requestId,
                    automation.SendKey(ReadArguments<KeyArguments>(request))),
                "web.select" => DaemonProtocol.Success(
                    requestId,
                    automation.Select(ReadArguments<SelectArguments>(request))),
                "web.check" => DaemonProtocol.Success(
                    requestId,
                    automation.SetChecked(ReadArguments<ElementActionArguments>(request), true)),
                "web.uncheck" => DaemonProtocol.Success(
                    requestId,
                    automation.SetChecked(ReadArguments<ElementActionArguments>(request), false)),
                "web.wait" => DaemonProtocol.Success(
                    requestId,
                    automation.Wait(ReadArguments<WaitArguments>(request))),
                "web.actions" => DaemonProtocol.Success(
                    requestId,
                    automation.GetActions(ReadArguments<SessionIdArguments>(request).SessionId)),
                _ => DaemonProtocol.Error(
                    requestId,
                    "unknown_command",
                    $"The daemon does not recognize command '{request.Command}'.")
            };
        }
        catch (WebAutomationException ex)
        {
            return DaemonProtocol.Error(requestId, ex.Code, ex.Message);
        }
        catch (ArgumentException ex)
        {
            return DaemonProtocol.Error(requestId, "invalid_arguments", ex.Message);
        }
        catch (NoSuchElementException ex)
        {
            return DaemonProtocol.Error(requestId, "element_not_found", ex.Message);
        }
        catch (StaleElementReferenceException ex)
        {
            return DaemonProtocol.Error(requestId, "stale_element", ex.Message);
        }
        catch (WebDriverException ex)
        {
            Console.Error.WriteLine($"WebEngine browser operation failed: {ex.Message}");
            return DaemonProtocol.Error(requestId, "web_driver_error", ex.Message);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"WebEngine daemon command failed: {ex}");
            return DaemonProtocol.Error(requestId, "command_failed", ex.Message);
        }
    }

    private static T ReadArguments<T>(DaemonRequest request)
    {
        if (request.Arguments is not { } arguments
            || arguments.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
        {
            throw new ArgumentException("Command arguments are required.");
        }

        return arguments.Deserialize<T>(DaemonProtocol.JsonOptions)
            ?? throw new ArgumentException("Command arguments could not be parsed.");
    }

    private DaemonResponse Shutdown(string requestId)
    {
        _stopRequested.Cancel();
        return DaemonProtocol.Success(requestId, new { stopping = true });
    }

    public void Dispose()
    {
        _stopRequested.Dispose();
    }
}

internal sealed record SessionOpenArguments(string BrowserType = "Edge", bool Headless = false);
internal sealed record SessionIdArguments(string SessionId);
internal sealed record NavigateArguments(string SessionId, string Url);
internal sealed record InspectArguments(string SessionId, string Mode = "actionable", int Limit = 100);
internal sealed record ElementLocatorArguments(
    string? Reference = null,
    string? Selector = null,
    string? Id = null,
    string? Name = null,
    string? TagName = null,
    string? InnerText = null,
    string? LinkText = null,
    string? ClassName = null,
    string? AriaLabel = null,
    string? XPath = null,
    int Index = 0);
internal sealed record ElementActionArguments(
    string SessionId,
    ElementLocatorArguments? Locator = null,
    string? Reference = null,
    string? Selector = null);
internal sealed record TypeArguments(
    string SessionId,
    ElementLocatorArguments? Locator = null,
    string? Reference = null,
    string? Selector = null,
    string? Text = null,
    bool ClearFirst = true);
internal sealed record KeyArguments(
    string SessionId,
    ElementLocatorArguments? Locator = null,
    string? Key = null,
    string? Reference = null,
    string? Selector = null);
internal sealed record SelectArguments(
    string SessionId,
    ElementLocatorArguments? Locator = null,
    string? Reference = null,
    string? Selector = null,
    string? Text = null,
    string? Value = null);
internal sealed record WaitArguments(
    string SessionId,
    string? Url = null,
    string? Text = null,
    string? Selector = null,
    int TimeoutSeconds = 30);
