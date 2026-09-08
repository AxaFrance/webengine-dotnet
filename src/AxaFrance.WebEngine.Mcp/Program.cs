using AxaFrance.WebEngine.Mcp.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AxaFrance.WebEngine.Mcp
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var (profile, transport, showHelp) = McpCliOptions.Parse(args);
            if (showHelp)
            {
                Console.Error.WriteLine(McpCliOptions.HelpText);
                return 0;
            }

            return transport == McpTransport.Stdio
                ? await RunStdioAsync(args, profile)
                : await RunHttpAsync(args, profile);
        }

        private static async Task<int> RunHttpAsync(string[] args, McpProfile profile)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Register all MCP tool services for the selected profile and the MCP server (HTTP)
            builder.AddMcpToolServices(profile);

            var app = builder.Build();

            app.UseHttpsRedirection();

            // Map the MCP endpoint — all tools reachable at /mcp
            app.MapMcp("/mcp");

            await app.RunAsync();
            return 0;
        }

        private static async Task<int> RunStdioAsync(string[] args, McpProfile profile)
        {
            var builder = Host.CreateApplicationBuilder(args);

            // CRITICAL for stdio: all logs to stderr, stdout reserved for JSON-RPC.
            builder.Logging.AddConsole(consoleLogOptions =>
            {
                consoleLogOptions.LogToStandardErrorThreshold = LogLevel.Trace;
            });

            builder.AddMcpToolServices(profile);

            var host = builder.Build();
            await host.RunAsync();
            return 0;
        }
    }
}
