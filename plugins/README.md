# Plugins

Two plugins, one binary (`src/AxaFrance.WebEngine.Mcp`, `webengine-mcp` → NuGet `AxaFrance.WebEngine.Mcp`, run via `dnx`).

| Plugin | Profile | Tools | Bundled skills |
|---|---|---|---|
| `webengine-web/` | `--profile web` (Selenium) | 29 | `webengine-web`, `webengine-scaffold` |
| `webengine-mobile/` | `--profile mobile` (Appium) | 17 | `webengine-mobile`, `webengine-scaffold` |

Each folder carries **both** packaging formats (they coexist):
- `plugin.json` + `mcp.json` → Agent Plugins 1.0 (VS Code/Copilot)
- `.codex-plugin/plugin.json` + `.mcp.json` → Codex (+ `config.toml.snippet` for manual `config.toml`)

Repo marketplace (Codex/ChatGPT desktop): `.agents/plugins/marketplace.json`.
Copilot CLI marketplace: `.github/plugin/marketplace.json`.
MCP Registry descriptor: `server.json`.
Workspace dogfood (local build): `.vscode/mcp.json`.

Transports: `--transport stdio` for all local plugins (`http` kept for backward compat at `/mcp`).
Install details per plugin: see each `README.md`.

## Prerequisites

The standard NuGet distribution uses `dnx`, which requires the **.NET 10 SDK 10.0.100 or later**. Modern .NET is not included with Windows by default. Install the SDK from [Microsoft](https://dotnet.microsoft.com/download/dotnet/10.0).

## GitHub Copilot CLI

1. Add the repository marketplace:

   ```text
   copilot plugin marketplace add AxaFrance/webengine-dotnet
   ```

2. Browse and install one profile:

   ```text
   copilot plugin marketplace browse webengine-plugins
   copilot plugin install webengine-web@webengine-plugins
   ```

   Replace `webengine-web` with `webengine-mobile` for Appium.

   For a direct source install, use `AxaFrance/webengine-dotnet:plugins/webengine-web` (or `plugins/webengine-mobile`).

3. Enable the plugin; its MCP server starts automatically.
4. Verify with `copilot plugin list`. In VS Code, `MCP: List Servers` should show the selected server and `Chat: Configure Skills` should show its skills.

For VS Code, use `Chat: Install Plugin From Source` with `AxaFrance/webengine-dotnet` or a direct plugin subdirectory.
