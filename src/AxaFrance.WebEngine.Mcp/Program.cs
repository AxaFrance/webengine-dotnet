
using AxaFrance.WebEngine.Mcp.Extensions;

namespace AxaFrance.WebEngine.Mcp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Register all MCP tool services and the MCP server
            builder.AddMcpToolServices();

            var app = builder.Build();

            app.UseHttpsRedirection();

            // Map the MCP endpoint � all tools reachable at /mcp
            app.MapMcp("/mcp");

            app.Run();
        }
    }
}
