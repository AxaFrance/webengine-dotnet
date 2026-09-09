using AxaFrance.WebEngine.Mcp.Configuration;
using AxaFrance.WebEngine.Mcp.Tools.Appium;
using AxaFrance.WebEngine.Mcp.Tools.Selenium;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AxaFrance.WebEngine.Mcp.Extensions;

public static class McpServiceExtensions
{
    /// <summary>
    /// Server-wide instructions surfaced to MCP clients (e.g. Codex reads the
    /// first ~512 chars). Profile-specific detail follows the common head.
    /// Keep the head self-contained: workflow + never-guess-locators rule.
    /// </summary>
    public const string ServerInstructionsHead =
        "WebEngine interactive gate: observe the live app before acting. " +
        "1) StartSession 2) GetAccessibilitySnapshot (default inspection) " +
        "3) ExecuteBulkActions for all actions from one snapshot 4) CloseSession. " +
        "Never guess locators: use ElementTag/stableLocator returned by tools. " +
        "After navigation or failed action, re-snapshot before retrying.";

    public const string ServerInstructionsWeb =
        " Web tools (Selenium): NavigateTo, GetActionableHtml/GetPageHtml[Chunk] for context, " +
        "ClickElement/TypeText/SetText/SelectFromDropdown*/Check/Uncheck, TakeScreenshot, ExecuteScript.";

    public const string ServerInstructionsMobile =
        " Mobile tools (Appium): needs platform/deviceName/appPath in StartSession; " +
        "GetPageSource[Chunk] for raw XML, Tap/LongPress/TypeText/SwipeScreen/PressBack/HideKeyboard.";

    public static string BuildInstructions(McpProfile profile) => profile switch
    {
        McpProfile.Web => ServerInstructionsHead + ServerInstructionsWeb,
        McpProfile.Mobile => ServerInstructionsHead + ServerInstructionsMobile,
        _ => ServerInstructionsHead + ServerInstructionsWeb + ServerInstructionsMobile
    };

    /// <summary>
    /// Legacy overload (backward compatible): respects appsettings IsEnabled flags.
    /// </summary>
    public static WebApplicationBuilder AddMcpToolServices(this WebApplicationBuilder builder)
        => AddMcpToolServices(builder, McpProfile.Both);

    /// <summary>
    /// HTTP host registration with explicit profile. <c>--profile web|mobile</c>
    /// overrides appsettings IsEnabled flags; <c>both</c> respects them.
    /// </summary>
    public static WebApplicationBuilder AddMcpToolServices(this WebApplicationBuilder builder, McpProfile profile)
    {
        var (seleniumEnabled, appiumEnabled) = ResolveEnabled(builder.Configuration, profile);
        LogEnabled(profile, seleniumEnabled, appiumEnabled);

        builder.Services.Configure<SeleniumOptions>(builder.Configuration.GetSection(SeleniumOptions.SectionName));
        builder.Services.Configure<AppiumOptions>(builder.Configuration.GetSection(AppiumOptions.SectionName));

        if (seleniumEnabled) builder.Services.AddSingleton<SeleniumSessionManager>();
        if (appiumEnabled) builder.Services.AddSingleton<AppiumSessionManager>();

        var mcpBuilder = builder.Services
            .AddMcpServer(options => { options.ServerInstructions = BuildInstructions(profile); })
            .WithHttpTransport();

        if (seleniumEnabled) mcpBuilder = mcpBuilder.WithTools<SeleniumTool>();
        if (appiumEnabled) mcpBuilder = mcpBuilder.WithTools<AppiumTool>();

        WarnOnCollision(profile, seleniumEnabled, appiumEnabled);
        return builder;
    }

    /// <summary>
    /// Stdio host registration (Copilot/Codex/Claude local plugins).
    /// Logging to stderr must be configured by the caller before invoking this.
    /// </summary>
    public static HostApplicationBuilder AddMcpToolServices(this HostApplicationBuilder builder, McpProfile profile)
    {
        var (seleniumEnabled, appiumEnabled) = ResolveEnabled(builder.Configuration, profile);
        LogEnabled(profile, seleniumEnabled, appiumEnabled);

        builder.Services.Configure<SeleniumOptions>(builder.Configuration.GetSection(SeleniumOptions.SectionName));
        builder.Services.Configure<AppiumOptions>(builder.Configuration.GetSection(AppiumOptions.SectionName));

        if (seleniumEnabled) builder.Services.AddSingleton<SeleniumSessionManager>();
        if (appiumEnabled) builder.Services.AddSingleton<AppiumSessionManager>();

        var mcpBuilder = builder.Services
            .AddMcpServer(options => { options.ServerInstructions = BuildInstructions(profile); })
            .WithStdioServerTransport();

        if (seleniumEnabled) mcpBuilder = mcpBuilder.WithTools<SeleniumTool>();
        if (appiumEnabled) mcpBuilder = mcpBuilder.WithTools<AppiumTool>();

        WarnOnCollision(profile, seleniumEnabled, appiumEnabled);
        return builder;
    }

    internal static (bool Selenium, bool Appium) ResolveEnabled(IConfiguration cfg, McpProfile profile)
    {
        bool seleniumFlag = cfg.GetSection(SeleniumOptions.SectionName).GetValue("IsEnabled", true);
        bool appiumFlag = cfg.GetSection(AppiumOptions.SectionName).GetValue("IsEnabled", true);
        return profile switch
        {
            McpProfile.Web => (true, false),
            McpProfile.Mobile => (false, true),
            _ => (seleniumFlag, appiumFlag)
        };
    }

    private static void LogEnabled(McpProfile profile, bool selenium, bool appium)
    {
        // Always stderr: stdout is reserved for JSON-RPC on stdio.
        Console.Error.WriteLine($"[MCP] profile={profile.ToString().ToLowerInvariant()} selenium={(selenium ? "enabled" : "DISABLED")} appium={(appium ? "enabled" : "DISABLED")}");
    }

    private static void WarnOnCollision(McpProfile profile, bool selenium, bool appium)
    {
        if (selenium && appium)
        {
            Console.Error.WriteLine("[MCP] WARNING: both Selenium and Appium tool sets are registered. " +
                "They share tool names (StartSession, CloseSession, ...). Prefer --profile web|mobile for stdio plugins.");
        }
    }
}
