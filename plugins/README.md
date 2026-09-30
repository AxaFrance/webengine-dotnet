# Plugins

Two MCP plugins and one CLI plugin are distributed from this repository.
The MCP plugins use one binary (`src/AxaFrance.WebEngine.Mcp`,
`webengine-mcp` → NuGet `AxaFrance.WebEngine.Mcp`, run via `dnx`). The CLI
plugin uses the separate `webengine` .NET tool and local daemon.

| Plugin | Profile | Tools | Bundled skills |
|---|---|---|---|
| `webengine-web/` | `--profile web` (Selenium) | 29 | `webengine-web`, `webengine-scaffold` |
| `webengine-mobile/` | `--profile mobile` (Appium) | 17 | `webengine-mobile`, `webengine-scaffold` |
| `webengine-cli/` | local `webengine` daemon | web sessions and actions | `webengine-cli` |

Each folder carries **both** packaging formats (they coexist):
- `plugin.json` + `mcp.json` → Agent Plugins 1.0 (VS Code/Copilot)
- `.codex-plugin/plugin.json` + `.mcp.json` → Codex (+ `config.toml.snippet` for manual `config.toml`)

Repo marketplace (Codex/ChatGPT desktop): `.agents/plugins/marketplace.json`.
Copilot CLI marketplace: `.github/plugin/marketplace.json`.
MCP Registry descriptor: `server.json`.
Workspace dogfood (local build): `.vscode/mcp.json`.

Transports: `--transport stdio` for all local MCP plugins (`http` kept for
backward compat at `/mcp`). The CLI plugin contains no MCP configuration and
communicates with the local daemon through the `webengine` command.
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

## CLI-only distribution

Use this path when the organization does not permit MCP servers:

```powershell
dotnet tool install --global AxaFrance.WebEngine.Cli --prerelease
copilot plugin install webengine-cli@webengine-plugins
webengine daemon start --json
```

The CLI tool currently requires the .NET 10 SDK. The daemon lifecycle is the
first implementation slice. Web session commands are now available for
Chrome, Edge, and Firefox. For multiline text, use exactly one of
`--text`, `--text-file`, or `--stdin`; use `web key --key Enter` when an
intentional key press is required. Mobile/Appium commands will follow after
the shared automation service is extracted from the MCP project.
