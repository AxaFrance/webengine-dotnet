using System.Text.Json;

namespace AxaFrance.WebEngine.Cli;

internal static class CliRunner
{
    public static async Task<int> RunAsync(CliOptions options)
    {
        using var client = new DaemonClient(options.PipeName);
        return await RunAsync(options, client);
    }

    internal static async Task<int> RunAsync(CliOptions options, DaemonClient client)
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

        DaemonResponse response;

        try
        {
            var typeText = options.Command == CliCommand.WebType
                ? await ResolveTypeTextAsync(options)
                : null;
            if (options.Command == CliCommand.WebWait)
                ValidateWait(options);
            var locator = RequiresElementLocator(options.Command)
                ? RequireLocator(options)
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
                        browserType = options.Browser ?? "Edge",
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
                        locator
                    }),
                    TimeSpan.FromSeconds(30),
                    CancellationToken.None),
                CliCommand.WebType => await client.RequestAsync(
                    "web.type",
                    Arguments(new
                    {
                        sessionId = RequireSession(options),
                        locator,
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
                        locator,
                        key = RequireValue(options.Key, "--key")
                    }),
                    TimeSpan.FromSeconds(30),
                    CancellationToken.None),
                CliCommand.WebSelect => await client.RequestAsync(
                    "web.select",
                    Arguments(new
                    {
                        sessionId = RequireSession(options),
                        locator,
                        text = options.Text,
                        value = options.Value
                    }),
                    TimeSpan.FromSeconds(30),
                    CancellationToken.None),
                CliCommand.WebCheck => await client.RequestAsync(
                    "web.check",
                    Arguments(new
                    {
                        sessionId = RequireSession(options),
                        locator
                    }),
                    TimeSpan.FromSeconds(30),
                    CancellationToken.None),
                CliCommand.WebUncheck => await client.RequestAsync(
                    "web.uncheck",
                    Arguments(new
                    {
                        sessionId = RequireSession(options),
                        locator
                    }),
                    TimeSpan.FromSeconds(30),
                    CancellationToken.None),
                CliCommand.WebWait => await client.RequestAsync(
                    "web.wait",
                    Arguments(new
                    {
                        sessionId = RequireSession(options),
                        url = options.Url,
                        text = options.Text,
                        selector = options.Selector,
                        timeoutSeconds = options.TimeoutSeconds ?? 30
                    }),
                    TimeSpan.FromSeconds((options.TimeoutSeconds ?? 30) + 5),
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
        catch (Exception ex)
        {
            Console.Error.WriteLine($"WebEngine CLI command failed: {ex}");
            response = DaemonProtocol.Error(string.Empty, "command_failed", ex.Message);
        }

        WriteResponse(response, options.Json);
        return response.Ok ? 0 : 1;
    }

    public static async Task<int> RunCommandLineAsync(string commandLine)
    {
        IReadOnlyList<string> tokens;
        try
        {
            tokens = CommandLineTokenizer.Tokenize(commandLine);
        }
        catch (FormatException ex)
        {
            Console.Error.WriteLine(ex.Message);
            return 2;
        }

        if (tokens.Count == 0)
        {
            Console.Error.WriteLine("The -c option requires a command.");
            return 2;
        }

        return await RunAsync(CliOptions.Parse(tokens.ToArray()));
    }

    public static async Task<int> RunShellAsync(ShellOptions shellOptions)
    {
        using var cancellation = new CancellationTokenSource();
        Console.CancelKeyPress += (_, eventArgs) =>
        {
            eventArgs.Cancel = true;
            cancellation.Cancel();
        };

        DaemonClient? client = null;
        string? clientPipeName = null;
        var shellPipeName = shellOptions.PipeName ?? DaemonEndpoint.DefaultPipeName;
        var interactive = !shellOptions.Json
            && !Console.IsInputRedirected
            && !Console.IsOutputRedirected;

        try
        {
            while (!cancellation.IsCancellationRequested)
            {
                if (interactive)
                {
                    Console.Write("webengine> ");
                    Console.Out.Flush();
                }

                string? line;
                try
                {
                    line = await Console.In.ReadLineAsync(cancellation.Token);
                }
                catch (OperationCanceledException) when (cancellation.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex) when (ex is IOException or ObjectDisposedException)
                {
                    WriteShellError("shell_input_failed", ex.Message, shellOptions.Json);
                    break;
                }

                if (line is null)
                    break;

                IReadOnlyList<string> tokens;
                try
                {
                    tokens = CommandLineTokenizer.Tokenize(line);
                }
                catch (FormatException ex)
                {
                    WriteShellError("invalid_arguments", ex.Message, shellOptions.Json);
                    continue;
                }

                if (tokens.Count == 0)
                    continue;

                if (tokens[0].Equals("webengine", StringComparison.OrdinalIgnoreCase))
                    tokens = tokens.Skip(1).ToArray();

                if (tokens.Count == 0)
                {
                    WriteShellError("invalid_arguments", "A shell command is required.", shellOptions.Json);
                    continue;
                }

                if (tokens[0].Equals("exit", StringComparison.OrdinalIgnoreCase)
                    || tokens[0].Equals("quit", StringComparison.OrdinalIgnoreCase))
                {
                    if (shellOptions.Json)
                    {
                        WriteResponse(
                            DaemonProtocol.Success(string.Empty, new { exiting = true }),
                            json: true);
                    }

                    break;
                }

                if (tokens[0].Equals("help", StringComparison.OrdinalIgnoreCase))
                {
                    if (shellOptions.Json)
                    {
                        WriteResponse(
                            DaemonProtocol.Success(string.Empty, new { help = CliOptions.HelpText }),
                            json: true);
                    }
                    else
                    {
                        Console.WriteLine(CliOptions.HelpText);
                    }

                    continue;
                }

                var options = CliOptions.Parse(tokens.ToArray());
                if (options.Command == CliCommand.Error)
                {
                    WriteShellError(
                        "invalid_arguments",
                        options.ErrorMessage ?? "The command is invalid.",
                        shellOptions.Json);
                    continue;
                }

                if (options.Command == CliCommand.Help)
                {
                    if (shellOptions.Json)
                    {
                        WriteResponse(
                            DaemonProtocol.Success(string.Empty, new { help = CliOptions.HelpText }),
                            json: true);
                    }
                    else
                    {
                        Console.WriteLine(CliOptions.HelpText);
                    }

                    continue;
                }

                if (options.Command == CliCommand.Version)
                {
                    var version = typeof(Program).Assembly.GetName().Version?.ToString(3) ?? "0.1.0";
                    if (shellOptions.Json)
                    {
                        WriteResponse(
                            DaemonProtocol.Success(string.Empty, new { version }),
                            json: true);
                    }
                    else
                    {
                        Console.WriteLine(version);
                    }

                    continue;
                }

                if (options.Command == CliCommand.DaemonRun)
                {
                    WriteShellError(
                        "invalid_command",
                        "Use daemon start in the shell. daemon run is a standalone command.",
                        shellOptions.Json);
                    continue;
                }

                if (options.Command == CliCommand.WebType && options.TextFromStdin)
                {
                    WriteShellError(
                        "invalid_arguments",
                        "Use --text-file for multiline input in the persistent shell. --stdin is available for one-shot commands.",
                        shellOptions.Json);
                    continue;
                }

                if (!HasOption(tokens, "--pipe"))
                    options = options with { PipeName = shellPipeName };
                if (shellOptions.Json)
                    options = options with { Json = true };

                if (client is null || !string.Equals(clientPipeName, options.PipeName, StringComparison.Ordinal))
                {
                    client?.Dispose();
                    client = new DaemonClient(options.PipeName, keepConnection: true);
                    clientPipeName = options.PipeName;
                }

                try
                {
                    await RunAsync(options, client);
                }
                catch (Exception ex)
                {
                    client.ResetConnection();
                    Console.Error.WriteLine($"WebEngine shell command failed: {ex}");
                    WriteShellError("command_failed", ex.Message, shellOptions.Json);
                }
                if (options.Command == CliCommand.DaemonStop)
                    client.ResetConnection();
            }
        }
        finally
        {
            client?.Dispose();
        }

        return 0;
    }

    private static JsonElement Arguments<T>(T value)
        => JsonSerializer.SerializeToElement(value, DaemonProtocol.JsonOptions);

    private static string RequireSession(CliOptions options)
        => RequireValue(options.SessionId, "--session");

    private static string RequireValue(string? value, string option)
        => string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException($"The {option} option is required.")
            : value;

    private static void ValidateWait(CliOptions options)
    {
        var conditions = (string.IsNullOrWhiteSpace(options.Url) ? 0 : 1)
            + (string.IsNullOrWhiteSpace(options.Text) ? 0 : 1)
            + (string.IsNullOrWhiteSpace(options.Selector) ? 0 : 1);

        if (conditions != 1)
            throw new ArgumentException("Use exactly one of --url, --text, or --selector with web wait.");
    }

    private static bool RequiresElementLocator(CliCommand command)
        => command is CliCommand.WebClick
            or CliCommand.WebType
            or CliCommand.WebKey
            or CliCommand.WebSelect
            or CliCommand.WebCheck
            or CliCommand.WebUncheck;

    private static ElementLocatorArguments RequireLocator(CliOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.Ref)
            && string.IsNullOrWhiteSpace(options.Selector)
            && string.IsNullOrWhiteSpace(options.Id)
            && string.IsNullOrWhiteSpace(options.Name)
            && string.IsNullOrWhiteSpace(options.TagName)
            && string.IsNullOrWhiteSpace(options.ElementText)
            && string.IsNullOrWhiteSpace(options.LinkText)
            && string.IsNullOrWhiteSpace(options.ClassName)
            && string.IsNullOrWhiteSpace(options.AriaLabel)
            && string.IsNullOrWhiteSpace(options.XPath))
        {
            throw new ArgumentException(
                "Provide an element locator: --ref, --selector, --id, --name, --tag, --element-text, --link-text, --class, --aria-label, or --xpath.");
        }

        return new ElementLocatorArguments(
            options.Ref,
            options.Selector,
            options.Id,
            options.Name,
            options.TagName,
            options.ElementText,
            options.LinkText,
            options.ClassName,
            options.AriaLabel,
            options.XPath,
            options.LocatorIndex ?? 0);
    }

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

    private static bool HasOption(IReadOnlyList<string> tokens, string option)
        => tokens.Any(token => token.Equals(option, StringComparison.OrdinalIgnoreCase));

    private static void WriteShellError(string code, string message, bool json)
    {
        if (json)
        {
            WriteResponse(DaemonProtocol.Error(string.Empty, code, message), json: true);
            return;
        }

        Console.Error.WriteLine($"{code}: {message}");
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
