# AxaFrance.WebEngine.Mcp

WebEngine MCP server: the interactive gate between AI coding agents and your applications. Observe live web pages (Selenium) and mobile apps (Appium), interact with them, then let the agent generate UI code in any stack (Playwright+TS, Selenium, WebEngine C#).

Run without installing (.NET 10):

```powershell
dnx AxaFrance.WebEngine.Mcp --profile web --transport stdio
dnx AxaFrance.WebEngine.Mcp --profile mobile --transport stdio
```

Or install as a .NET tool:

```powershell
dotnet tool install -g AxaFrance.WebEngine.Mcp
webengine-mcp --profile web --transport stdio
```

Two plugins, one binary: `plugins/webengine-web` (29 tools, Selenium) and `plugins/webengine-mobile` (17 tools, Appium), each bundling Agent Skills + MCP server config for GitHub Copilot (Agent Plugins 1.0) and Codex. Install details in `plugins/README.md`.

---
Company: AXA France
Author: Huaxing YUAN
Repository: https://github.com/AxaFrance/webengine-dotnet
Documentation: https://axafrance.github.io/webengine-dotnet/
