using AxaFrance.WebEngine.Mcp.Configuration;
using AxaFrance.WebEngine.Mcp.Tools.Appium;
using AxaFrance.WebEngine.Mcp.Tools.Selenium;

namespace AxaFrance.WebEngine.Mcp.Extensions;

public static class McpServiceExtensions
{
    /// <summary>
    /// Registers configuration, services and the MCP server.
    /// Each tool set is registered only when its <c>IsEnabled</c> flag is <c>true</c>
    /// in the corresponding <c>appsettings.json</c> section.
    /// </summary>
    public static WebApplicationBuilder AddMcpToolServices(this WebApplicationBuilder builder)
    {
        var cfg = builder.Configuration;

        // Bind configuration sections (always, so IOptions<T> is available regardless of IsEnabled)
        builder.Services.Configure<SeleniumOptions>(cfg.GetSection(SeleniumOptions.SectionName));
        builder.Services.Configure<AppiumOptions>(cfg.GetSection(AppiumOptions.SectionName));

        // Read IsEnabled flags early from IConfiguration (defaults to true so configs that omit the
        // flag keep working exactly as before)
        bool seleniumEnabled = cfg.GetSection(SeleniumOptions.SectionName).GetValue("IsEnabled", true);
        bool appiumEnabled   = cfg.GetSection(AppiumOptions.SectionName)  .GetValue("IsEnabled", true);

        // Log enabled state at startup so it is visible in the console / output window
        Console.WriteLine($"[MCP] Selenium : {(seleniumEnabled ? "enabled" : "DISABLED")}");
        Console.WriteLine($"[MCP] Appium   : {(appiumEnabled   ? "enabled" : "DISABLED")}");

        // Register singleton services only for enabled tools
        if (seleniumEnabled) builder.Services.AddSingleton<SeleniumSessionManager>();
        if (appiumEnabled)   builder.Services.AddSingleton<AppiumSessionManager>();

        // Register MCP server and conditionally add each tool type
        var mcpBuilder = builder.Services
            .AddMcpServer()
            .WithHttpTransport();

        if (seleniumEnabled) mcpBuilder = mcpBuilder.WithTools<SeleniumTool>();
        if (appiumEnabled)   mcpBuilder = mcpBuilder.WithTools<AppiumTool>();

        return builder;
    }
}
