namespace AxaFrance.WebEngine.Cli;

internal enum CliCommand
{
    Help,
    Version,
    DaemonStart,
    DaemonRun,
    DaemonStatus,
    DaemonStop,
    WebSessionOpen,
    WebSessionList,
    WebSessionClose,
    WebNavigate,
    WebInspect,
    WebHtml,
    WebClick,
    WebType,
    WebKey,
    WebSelect,
    WebCheck,
    WebUncheck,
    WebWait,
    WebActions,
    Error
}

internal sealed record CliOptions(
    CliCommand Command,
    bool Json,
    bool Quiet,
    string PipeName,
    string? ErrorMessage,
    string? SessionId = null,
    string? Url = null,
    string? Browser = null,
    bool Headless = false,
    string? Ref = null,
    string? Selector = null,
    string? Text = null,
    string? Value = null,
    string? InspectionMode = null,
    int? Limit = null,
    string? TextFile = null,
    bool TextFromStdin = false,
    string? Key = null,
    int? TimeoutSeconds = null,
    string? Id = null,
    string? Name = null,
    string? TagName = null,
    string? ElementText = null,
    string? LinkText = null,
    string? ClassName = null,
    string? AriaLabel = null,
    string? XPath = null,
    int? LocatorIndex = null)
{
    public const string HelpText =
        """
        WebEngine CLI - persistent local browser and mobile automation

        Usage:
          webengine
          webengine --json
          webengine -c "<command>"
          webengine shell [--json] [--pipe <name>]
          webengine daemon start [--json] [--pipe <name>]
          webengine daemon run [--quiet] [--pipe <name>]
          webengine daemon status [--json] [--pipe <name>]
          webengine daemon stop [--json] [--pipe <name>]
          webengine web session open [--browser <name>] [--headless] [--json]
          webengine web session list [--json]
          webengine web session close --session <id> [--json]
          webengine web navigate --session <id> --url <url> [--json]
          webengine web inspect --session <id> [--mode <actionable|snapshot>] [--json]
          webengine web html --session <id> [--json]
          webengine web click --session <id> [locator options] [--json]
          webengine web type --session <id> [locator options] (--text <text> | --text-file <path> | --stdin) [--json]
          webengine web key --session <id> [locator options] --key <name> [--json]
          webengine web select --session <id> [locator options] (--text <text> | --value <value>) [--json]
          webengine web check --session <id> [locator options] [--json]
          webengine web uncheck --session <id> [locator options] [--json]
          webengine web wait --session <id> (--url <url> | --text <text> | --selector <css>) [--timeout <seconds>] [--json]
          webengine web actions --session <id> [--json]
          webengine --version

        Shell:
          The default command starts a persistent shell. Send one command per
          line and receive one response per command. Use --json for a
          prompt-free JSON-lines shell. Use -c to run one command and exit.

        Options:
          --json          Emit a machine-readable response on stdout.
          --pipe <name>   Override the default per-user named pipe.
          --quiet         Suppress daemon lifecycle diagnostics.
          --session <id>  Target an existing web session.
          --browser <name> Browser engine: Edge (default), Chrome, or Firefox.
          --headless      Start the browser without a visible window.
          --url <url>     URL to navigate to.
          --ref <ref>     Reference returned by the latest inspection.
          --selector <css> CSS selector for an element.
          --id <id>       Native WebEngine locator: HTML id.
          --name <name>   Native WebEngine locator: HTML name.
          --tag <tag>     Native WebEngine locator: HTML tag name.
          --element-text <text>
                          Native WebEngine locator: exact visible element text.
          --link-text <text>
                          Native WebEngine locator: exact hyperlink text.
          --class <name>  Native WebEngine locator: CSS class name.
          --aria-label <text>
                          Native WebEngine locator: aria-label.
          --xpath <xpath> Native WebEngine locator: XPath expression.
          --index <n>     Zero-based match index when a locator matches many.
          --text <text>   Text to type or visible option text to select.
          --text-file <path>
                          Read UTF-8 text from a file instead of the command line.
          --stdin         Read all text from standard input.
          --key <name>    Named key to send: Enter, Tab, Escape, Backspace,
                          Delete, Space, Home, End, PageUp, PageDown, or ArrowUp/
                          ArrowDown/ArrowLeft/ArrowRight.
          --value <value> Option value to select.
          --mode <mode>   Inspection mode: actionable or snapshot.
          --limit <n>     Maximum number of inspected elements.
          --timeout <n>   Wait timeout in seconds (default: 30).
          -h, --help      Show this help.
        """;

    public static CliOptions Parse(string[] args)
    {
        var commandTokens = new List<string>();
        var json = false;
        var quiet = false;
        var pipeName = DaemonEndpoint.DefaultPipeName;
        string? sessionId = null;
        string? url = null;
        string? browser = null;
        var headless = false;
        string? reference = null;
        string? selector = null;
        string? text = null;
        string? value = null;
        string? inspectionMode = null;
        int? limit = null;
        string? textFile = null;
        var textFromStdin = false;
        string? key = null;
        int? timeoutSeconds = null;
        string? id = null;
        string? name = null;
        string? tagName = null;
        string? elementText = null;
        string? linkText = null;
        string? className = null;
        string? ariaLabel = null;
        string? xpath = null;
        int? locatorIndex = null;

        for (var index = 0; index < args.Length; index++)
        {
            var argument = args[index];
            if (argument.Equals("--json", StringComparison.OrdinalIgnoreCase))
            {
                json = true;
            }
            else if (argument.Equals("--quiet", StringComparison.OrdinalIgnoreCase))
            {
                quiet = true;
            }
            else if (argument.Equals("--pipe", StringComparison.OrdinalIgnoreCase))
            {
                if (index + 1 >= args.Length || string.IsNullOrWhiteSpace(args[++index]))
                    return Error("The --pipe option requires a non-empty name.");

                pipeName = args[index];
            }
            else if (argument.Equals("--session", StringComparison.OrdinalIgnoreCase))
            {
                sessionId = ReadValue(args, ref index, argument);
                if (sessionId is null)
                    return Error($"The {argument} option requires a non-empty value.");
            }
            else if (argument.Equals("--url", StringComparison.OrdinalIgnoreCase))
            {
                url = ReadValue(args, ref index, argument);
                if (url is null)
                    return Error($"The {argument} option requires a non-empty value.");
            }
            else if (argument.Equals("--browser", StringComparison.OrdinalIgnoreCase))
            {
                browser = ReadValue(args, ref index, argument);
                if (browser is null)
                    return Error($"The {argument} option requires a non-empty value.");
            }
            else if (argument.Equals("--headless", StringComparison.OrdinalIgnoreCase))
            {
                headless = true;
            }
            else if (argument.Equals("--ref", StringComparison.OrdinalIgnoreCase))
            {
                reference = ReadValue(args, ref index, argument);
                if (reference is null)
                    return Error($"The {argument} option requires a non-empty value.");
            }
            else if (argument.Equals("--selector", StringComparison.OrdinalIgnoreCase))
            {
                selector = ReadValue(args, ref index, argument);
                if (selector is null)
                    return Error($"The {argument} option requires a non-empty value.");
            }
            else if (argument.Equals("--id", StringComparison.OrdinalIgnoreCase))
            {
                id = ReadValue(args, ref index, argument);
                if (id is null)
                    return Error($"The {argument} option requires a non-empty value.");
            }
            else if (argument.Equals("--name", StringComparison.OrdinalIgnoreCase))
            {
                name = ReadValue(args, ref index, argument);
                if (name is null)
                    return Error($"The {argument} option requires a non-empty value.");
            }
            else if (argument.Equals("--tag", StringComparison.OrdinalIgnoreCase)
                || argument.Equals("--tag-name", StringComparison.OrdinalIgnoreCase))
            {
                tagName = ReadValue(args, ref index, argument);
                if (tagName is null)
                    return Error($"The {argument} option requires a non-empty value.");
            }
            else if (argument.Equals("--element-text", StringComparison.OrdinalIgnoreCase))
            {
                elementText = ReadValue(args, ref index, argument);
                if (elementText is null)
                    return Error($"The {argument} option requires a non-empty value.");
            }
            else if (argument.Equals("--link-text", StringComparison.OrdinalIgnoreCase))
            {
                linkText = ReadValue(args, ref index, argument);
                if (linkText is null)
                    return Error($"The {argument} option requires a non-empty value.");
            }
            else if (argument.Equals("--class", StringComparison.OrdinalIgnoreCase))
            {
                className = ReadValue(args, ref index, argument);
                if (className is null)
                    return Error($"The {argument} option requires a non-empty value.");
            }
            else if (argument.Equals("--aria-label", StringComparison.OrdinalIgnoreCase))
            {
                ariaLabel = ReadValue(args, ref index, argument);
                if (ariaLabel is null)
                    return Error($"The {argument} option requires a non-empty value.");
            }
            else if (argument.Equals("--xpath", StringComparison.OrdinalIgnoreCase))
            {
                xpath = ReadValue(args, ref index, argument);
                if (xpath is null)
                    return Error($"The {argument} option requires a non-empty value.");
            }
            else if (argument.Equals("--index", StringComparison.OrdinalIgnoreCase))
            {
                var indexText = ReadValue(args, ref index, argument);
                if (indexText is null
                    || !int.TryParse(indexText, out var parsedIndex)
                    || parsedIndex < 0)
                {
                    return Error($"The {argument} option requires a non-negative integer.");
                }

                locatorIndex = parsedIndex;
            }
            else if (argument.Equals("--text", StringComparison.OrdinalIgnoreCase))
            {
                text = ReadValue(args, ref index, argument, allowEmpty: true);
                if (text is null)
                    return Error($"The {argument} option requires a value.");
            }
            else if (argument.Equals("--text-file", StringComparison.OrdinalIgnoreCase))
            {
                textFile = ReadValue(args, ref index, argument);
                if (textFile is null)
                    return Error($"The {argument} option requires a non-empty value.");
            }
            else if (argument.Equals("--stdin", StringComparison.OrdinalIgnoreCase))
            {
                textFromStdin = true;
            }
            else if (argument.Equals("--key", StringComparison.OrdinalIgnoreCase))
            {
                key = ReadValue(args, ref index, argument);
                if (key is null)
                    return Error($"The {argument} option requires a non-empty value.");
            }
            else if (argument.Equals("--value", StringComparison.OrdinalIgnoreCase))
            {
                value = ReadValue(args, ref index, argument, allowEmpty: true);
                if (value is null)
                    return Error($"The {argument} option requires a value.");
            }
            else if (argument.Equals("--mode", StringComparison.OrdinalIgnoreCase))
            {
                inspectionMode = ReadValue(args, ref index, argument);
                if (inspectionMode is null)
                    return Error($"The {argument} option requires a non-empty value.");
            }
            else if (argument.Equals("--limit", StringComparison.OrdinalIgnoreCase))
            {
                var limitText = ReadValue(args, ref index, argument);
                if (limitText is null || !int.TryParse(limitText, out var parsedLimit) || parsedLimit < 1)
                    return Error($"The {argument} option requires a positive integer.");

                limit = parsedLimit;
            }
            else if (argument.Equals("--timeout", StringComparison.OrdinalIgnoreCase))
            {
                var timeoutText = ReadValue(args, ref index, argument);
                if (timeoutText is null
                    || !int.TryParse(timeoutText, out var parsedTimeout)
                    || parsedTimeout < 1)
                {
                    return Error($"The {argument} option requires a positive integer.");
                }

                timeoutSeconds = parsedTimeout;
            }
            else if (argument.Equals("--version", StringComparison.OrdinalIgnoreCase))
            {
                commandTokens.Add(argument);
            }
            else if (argument.Equals("-h", StringComparison.OrdinalIgnoreCase)
                || argument.Equals("--help", StringComparison.OrdinalIgnoreCase)
                || argument.Equals("-?", StringComparison.OrdinalIgnoreCase))
            {
                return new(CliCommand.Help, json, quiet, pipeName, null);
            }
            else
            {
                if (argument.StartsWith("-", StringComparison.Ordinal))
                    return Error($"Unknown option '{argument}'.");

                commandTokens.Add(argument);
            }
        }

        if (commandTokens.Count == 0)
            return new(CliCommand.Help, json, quiet, pipeName, null);

        if (commandTokens.Count == 1
            && commandTokens[0].Equals("--version", StringComparison.OrdinalIgnoreCase))
        {
            return new(CliCommand.Version, json, quiet, pipeName, null);
        }

        if (commandTokens[0].Equals("daemon", StringComparison.OrdinalIgnoreCase))
        {
            if (commandTokens.Count != 2)
                return Error("Use daemon start, daemon run, daemon status, or daemon stop.");

            var command = commandTokens[1].ToLowerInvariant() switch
            {
                "start" => CliCommand.DaemonStart,
                "run" => CliCommand.DaemonRun,
                "status" => CliCommand.DaemonStatus,
                "stop" => CliCommand.DaemonStop,
                _ => CliCommand.Error
            };

            return command == CliCommand.Error
                ? Error($"Unknown daemon command '{commandTokens[1]}'.")
                : new(command, json, quiet, pipeName, null);
        }

        if (!commandTokens[0].Equals("web", StringComparison.OrdinalIgnoreCase))
            return Error($"Unknown command '{commandTokens[0]}'.");

        var webCommand = ParseWebCommand(commandTokens);
        return webCommand == CliCommand.Error
            ? Error("Use web session open/list/close, web navigate, web inspect, web html, web click, web type, web key, web select, web check, web uncheck, web wait, or web actions.")
            : new(
                webCommand,
                json,
                quiet,
                pipeName,
                null,
                sessionId,
                url,
                browser,
                headless,
                reference,
                selector,
                text,
                value,
                inspectionMode,
                limit,
                textFile,
                textFromStdin,
                key,
                timeoutSeconds,
                id,
                name,
                tagName,
                elementText,
                linkText,
                className,
                ariaLabel,
                xpath,
                locatorIndex);
    }

    private static CliCommand ParseWebCommand(IReadOnlyList<string> tokens)
    {
        if (tokens.Count == 3
            && tokens[1].Equals("session", StringComparison.OrdinalIgnoreCase))
        {
            return tokens[2].ToLowerInvariant() switch
            {
                "open" => CliCommand.WebSessionOpen,
                "list" => CliCommand.WebSessionList,
                "close" => CliCommand.WebSessionClose,
                _ => CliCommand.Error
            };
        }

        if (tokens.Count != 2)
            return CliCommand.Error;

        return tokens[1].ToLowerInvariant() switch
        {
            "navigate" => CliCommand.WebNavigate,
            "inspect" => CliCommand.WebInspect,
            "html" => CliCommand.WebHtml,
            "click" => CliCommand.WebClick,
            "type" => CliCommand.WebType,
            "key" => CliCommand.WebKey,
            "select" => CliCommand.WebSelect,
            "check" => CliCommand.WebCheck,
            "uncheck" => CliCommand.WebUncheck,
            "wait" => CliCommand.WebWait,
            "actions" => CliCommand.WebActions,
            _ => CliCommand.Error
        };
    }

    private static string? ReadValue(
        IReadOnlyList<string> args,
        ref int index,
        string option,
        bool allowEmpty = false)
    {
        if (index + 1 >= args.Count)
            return null;

        var value = args[++index];
        return allowEmpty || !string.IsNullOrWhiteSpace(value) ? value : null;
    }

    private static CliOptions Error(string message)
        => new(CliCommand.Error, false, false, DaemonEndpoint.DefaultPipeName, message);
}
