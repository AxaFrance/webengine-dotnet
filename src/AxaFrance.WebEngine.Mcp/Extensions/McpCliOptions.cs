namespace AxaFrance.WebEngine.Mcp.Extensions;

/// <summary>
/// Tool profile selected at startup. A single binary serves two plugins:
/// <c>web</c> (Selenium) and <c>mobile</c> (Appium). <c>both</c> preserves
/// the legacy HTTP behaviour.
/// </summary>
public enum McpProfile
{
    Both,
    Web,
    Mobile
}

/// <summary>Transport selected at startup. One transport per process.</summary>
public enum McpTransport
{
    Http,
    Stdio
}

/// <summary>Minimal CLI parser for <c>--profile</c> and <c>--transport</c>.</summary>
public static class McpCliOptions
{
    public const string HelpText =
        "WebEngine MCP server - observe and interact with web/mobile apps for AI coding agents.\n" +
        "\nUsage:\n" +
        "  webengine-mcp [--profile web|mobile|both] [--transport http|stdio]\n" +
        "\nOptions:\n" +
        "  --profile web|mobile|both   Tool set to expose (default: both, respects appsettings IsEnabled flags).\n" +
        "                             web    = Selenium browser tools only  -> plugin 'webengine-web'.\n" +
        "                             mobile = Appium device tools only     -> plugin 'webengine-mobile'.\n" +
        "  --transport http|stdio      Transport to use (default: http for backward compatibility).\n" +
        "                             stdio is required for Copilot/Codex/Claude local plugins.\n" +
        "  -h, --help                Show this help.\n" +
        "\nExamples:\n" +
        "  webengine-mcp --profile web --transport stdio\n" +
        "  webengine-mcp --profile mobile --transport stdio\n" +
        "  dotnet run --project AxaFrance.WebEngine.Mcp -- --profile web --transport stdio\n";

    public static (McpProfile Profile, McpTransport Transport, bool ShowHelp) Parse(string[] args)
    {
        var profile = McpProfile.Both;
        var transport = McpTransport.Http;
        var showHelp = false;

        for (int i = 0; i < args.Length; i++)
        {
            var a = args[i].Trim();
            if (a.Equals("--profile", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
            {
                profile = args[++i].Trim().ToLowerInvariant() switch
                {
                    "web" => McpProfile.Web,
                    "mobile" => McpProfile.Mobile,
                    "app" => McpProfile.Mobile,
                    "appium" => McpProfile.Mobile,
                    "selenium" => McpProfile.Web,
                    _ => McpProfile.Both
                };
            }
            else if (a.StartsWith("--profile=", StringComparison.OrdinalIgnoreCase))
            {
                profile = a["--profile=".Length..].Trim().ToLowerInvariant() switch
                {
                    "web" => McpProfile.Web,
                    "mobile" => McpProfile.Mobile,
                    "app" => McpProfile.Mobile,
                    _ => McpProfile.Both
                };
            }
            else if (a.Equals("--transport", StringComparison.OrdinalIgnoreCase) && i + 1 < args.Length)
            {
                transport = args[++i].Trim().ToLowerInvariant() switch
                {
                    "stdio" => McpTransport.Stdio,
                    _ => McpTransport.Http
                };
            }
            else if (a.StartsWith("--transport=", StringComparison.OrdinalIgnoreCase))
            {
                transport = a["--transport=".Length..].Trim().ToLowerInvariant() switch
                {
                    "stdio" => McpTransport.Stdio,
                    _ => McpTransport.Http
                };
            }
            else if (a is "-h" or "--help" or "-?")
            {
                showHelp = true;
            }
        }

        return (profile, transport, showHelp);
    }
}
