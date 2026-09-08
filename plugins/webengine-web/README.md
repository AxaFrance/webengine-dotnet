# Plugin webengine-web (Selenium)

Interactive gate between coding agents and a live browser. Bundles skills + MCP server for **VS Code/Copilot** (Agent Plugins 1.0) and **Codex** in one folder.

```
plugins/webengine-web/
  plugin.json               # Agent Plugins 1.0 manifest (Copilot/VS Code)
  mcp.json                  # portable MCP config (VS Code discovers it)
  .codex-plugin/plugin.json # Codex manifest
  .mcp.json                 # Codex bundled MCP server
  skills/webengine-web/     # observe/act/generate skill
  skills/webengine-scaffold/# solution scaffolding skill
```

Prerequisite (both agents): .NET 10 + package on NuGet (`dnx AxaFrance.WebEngine.Mcp ...`).
Publish it once: `dotnet pack src/AxaFrance.WebEngine.Mcp -c Release` then `dotnet nuget push *.nupkg --source https://api.nuget.org/v3/index.json --api-key <key>`.

## Option A — GitHub Copilot (VS Code)

1. `Chat: Install Plugin From Source` → paste `https://github.com/AxaFrance/webengine-dotnet` (or add marketplace `AxaFrance/webengine-dotnet` via `chat.plugins.marketplaces`, or register a local clone via `chat.pluginLocations`).
2. Enable the plugin; its MCP server starts automatically (no separate trust prompt).
3. Verify: `webengine-web` in `MCP: List Servers`, skills in `Chat: Configure Skills`.

Without plugin install (manual): copy `skills/webengine-web` (+ `skills/webengine-scaffold`) to `.github/skills/`, and `mcp.json` content into `.vscode/mcp.json`.

## Option B — Codex (CLI / IDE / desktop)

```bash
codex plugin marketplace add AxaFrance/webengine-dotnet --sparse .agents/plugins
# then install webengine-web from the Plugins Directory (marketplace: WebEngine Plugins)
```

Manual alternative: `codex mcp add webengine-web -- dnx AxaFrance.WebEngine.Mcp --profile web --transport stdio`
+ copy `skills/` to `~/.codex/skills/`. This repo also ships `.agents/plugins/marketplace.json` for repo-scoped installs.

## Local build fallback (before NuGet publish)

`dotnet build src/AxaFrance.WebEngine.Mcp -c Release`, then replace the `dnx` command with
`dotnet exec <repo>/src/AxaFrance.WebEngine.Mcp/bin/Release/net10.0/AxaFrance.WebEngine.Mcp.dll --profile web --transport stdio`.
NEVER use plain `dotnet run` (its launch banner pollutes stdout). This repo's own `.vscode/mcp.json` already uses this form.

## Use

Ask the agent to observe/act in the browser; it generates code in YOUR stack (Playwright TS, Selenium, WebEngine C#). Verify: 29 tools, `initialize` returns server instructions.
