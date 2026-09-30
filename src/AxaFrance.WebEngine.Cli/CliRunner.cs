using System.Text.Json;

namespace AxaFrance.WebEngine.Cli;

internal static class CliRunner
{
    public static async Task<int> RunAsync(CliOptions options)
    {
        if (options.Command == CliCommand.Error)
        {
            Console.Error.WriteLine(options.ErrorMessage);
            Console.Error.WriteLine(CliOptions.HelpText);
            return 2;
        }

        if (options.Command == CliCommand.Help)
        {
            Console.WriteLine(CliOptions.HelpText);
            return 0;
        }

        if (options.Command == CliCommand.Version)
        {
            var version = typeof(Program).Assembly.GetName().Version;
            Console.WriteLine(version is null ? "0.1.0" : version.ToString(3));
            return 0;
        }

        if (options.Command == CliCommand.DaemonRun)
        {
            using var host = new DaemonHost(options.PipeName);
            using var cancellation = new CancellationTokenSource();
            Console.CancelKeyPress += (_, eventArgs) =>
            {
                eventArgs.Cancel = true;
                cancellation.Cancel();
            };

            await host.RunAsync(cancellation.Token, options.Quiet);
            return 0;
        }

        var client = new DaemonClient(options.PipeName);
        DaemonResponse response;

        try
        {
            var typeText = options.Command == CliCommand.WebType
                ? await ResolveTypeTextAsync(options)
                : null;

            response = options.Command switch
            {
                CliCommand.DaemonStart => await client.StartAsync(CancellationToken.None),
                CliCommand.DaemonStatus => await client.TryPingAsync(CancellationToken.None)
                    ?? DaemonProtocol.Error(
                        string.Empty,
                        "daemon_not_running",
                        "The WebEngine daemon is not running."),
                CliCommand.DaemonStop => await client.RequestAsync(
                    "daemon.shutdown",
                    TimeSpan.FromSeconds(5),
                    CancellationToken.None),
                CliCommand.WebSessionOpen => await client.RequestAsync(
                    "web.session.open",
                    Arguments(new
                    {
                        browserType = options.Browser ?? "Chrome",
                        headless = options.Headless
                    }),
                    TimeSpan.FromSeconds(60),
                    CancellationToken.None),
                CliCommand.WebSessionList => await client.RequestAsync(
                    "web.session.list",
                    TimeSpan.FromSeconds(10),
                    CancellationToken.None),
                CliCommand.WebSessionClose => await client.RequestAsync(
                    "web.session.close",
                    Arguments(new { sessionId = RequireSession(options) }),
                    TimeSpan.FromSeconds(30),
                    CancellationToken.None),
                CliCommand.WebNavigate => await client.RequestAsync(
                    "web.navigate",
                    Arguments(new
                    {
                        sessionId = RequireSession(options),
                        url = RequireValue(options.Url, "--url")
                    }),
                    TimeSpan.FromSeconds(60),
                    CancellationToken.None),
                CliCommand.WebInspect => await client.RequestAsync(
                    "web.inspect",
                    Arguments(new
                    {
                        sessionId = RequireSession(options),
                        mode = options.InspectionMode ?? "actionable",
                        limit = options.Limit ?? 100
                    }),
                    TimeSpan.FromSeconds(30),
                    CancellationToken.None),
                CliCommand.WebHtml => await client.RequestAsync(
                    "web.html",
                    Arguments(new { sessionId = RequireSession(options) }),
                    TimeSpan.FromSeconds(30),
                    CancellationToken.None),
                CliCommand.WebClick => await client.RequestAsync(
                    "web.click",
                    Arguments(new
                    {
                        sessionId = RequireSession(options),
                        reference = RequireReferenceOrSelector(options).Reference,
                        selector = RequireReferenceOrSelector(options).Selector
                    }),
                    TimeSpan.FromSeconds(30),
                    CancellationToken.None),
                CliCommand.WebType => await client.RequestAsync(
                    "web.type",
                    Arguments(new
                    {
                        sessionId = RequireSession(options),
                        reference = RequireReferenceOrSelector(options).Reference,
                        selector = RequireReferenceOrSelector(options).Selector,
                        text = typeText,
                        clearFirst = true
                    }),
                    TimeSpan.FromSeconds(30),
                    CancellationToken.None),
                CliCommand.WebKey => await client.RequestAsync(
                    "web.key",
                    Arguments(new
                    {
                        sessionId = RequireSession(options),
                        reference = RequireReferenceOrSelector(options).Reference,
                        selector = RequireReferenceOrSelector(options).Selector,
                        key = RequireValue(options.Key, "--key")
                    }),
                    TimeSpan.FromSeconds(30),
                    CancellationToken.None),
                CliCommand.WebSelect => await client.RequestAsync(
                    "web.select",
                    Arguments(new
                    {
                        sessionId = RequireSession(options),
                        reference = RequireReferenceOrSelector(options).Reference,
                        selector = RequireReferenceOrSelector(options).Selector,
                        text = options.Text,
                        value = options.Value
                    }),
                    TimeSpan.FromSeconds(30),
                    CancellationToken.None),
                CliCommand.WebActions => await client.RequestAsync(
                    "web.actions",
                    Arguments(new { sessionId = RequireSession(options) }),
                    TimeSpan.FromSeconds(10),
                    CancellationToken.None),
                _ => DaemonProtocol.Error(string.Empty, "invalid_command", "Unsupported CLI command.")
            };
        }
        catch (ArgumentException ex)
        {
            response = DaemonProtocol.Error(string.Empty, "invalid_arguments", ex.Message);
        }
        catch (Exception ex) when (
            ex is IOException
            or InvalidDataException
            or InvalidOperationException
            or TimeoutException)
        {
            response = DaemonProtocol.Error(string.Empty, "daemon_unavailable", ex.Message);
        }

        WriteResponse(response, options.Json);
        return response.Ok ? 0 : 1;
    }

    private static JsonElement Arguments<T>(T value)
        => JsonSerializer.SerializeToElement(value, DaemonProtocol.JsonOptions);

    private static string RequireSession(CliOptions options)
        => RequireValue(options.SessionId, "--session");

    private static string RequireValue(string? value, string option)
        => string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException($"The {option} option is required.")
            : value;

    private static async Task<string> ResolveTypeTextAsync(CliOptions options)
    {
        var sourceCount = (options.Text is not null ? 1 : 0)
            + (options.TextFile is not null ? 1 : 0)
            + (options.TextFromStdin ? 1 : 0);

        if (sourceCount == 0)
            throw new ArgumentException("Use one of --text, --text-file, or --stdin.");

        if (sourceCount > 1)
            throw new ArgumentException("Use only one of --text, --text-file, or --stdin.");

        if (options.Text is not null)
            return options.Text;

        if (options.TextFromStdin)
        {
            try
            {
                return await Console.In.ReadToEndAsync();
            }
            catch (Exception ex) when (ex is IOException or ObjectDisposedException)
            {
                throw new ArgumentException("The text could not be read from standard input.", ex);
            }
        }

        try
        {
            return await File.ReadAllTextAsync(options.TextFile!);
        }
        catch (Exception ex) when (
            ex is IOException
            or UnauthorizedAccessException
            or ArgumentException)
        {
            throw new ArgumentException(
                $"The text file '{options.TextFile}' could not be read: {ex.Message}",
                ex);
        }
    }

    private static (string? Reference, string? Selector) RequireReferenceOrSelector(CliOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.Ref) && string.IsNullOrWhiteSpace(options.Selector))
            throw new ArgumentException("Use either --ref or --selector to identify the element.");

        return (options.Ref, options.Selector);
    }

    private static void WriteResponse(DaemonResponse response, bool json)
    {
        if (json)
        {
            Console.WriteLine(JsonSerializer.Serialize(response, DaemonProtocol.JsonOptions));
            return;
        }

        if (response.Ok)
        {
            Console.WriteLine(
                response.Result.HasValue
                    ? response.Result.Value.GetRawText()
                    : "WebEngine command completed.");
        }
        else
        {
            Console.Error.WriteLine($"{response.Error?.Code}: {response.Error?.Message}");
        }
    }
}
