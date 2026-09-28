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

Prerequisite (both agents): .NET 10 SDK 10.0.100 or later + package on NuGet (`dnx AxaFrance.WebEngine.Mcp ...`). Modern .NET is not included with Windows by default. Maintainers can prepare a package with `powershell -ExecutionPolicy Bypass -File scripts/prepare-mcp-release.ps1`; the guarded `mcp-v<version>` workflow publishes NuGet and MCP Registry metadata.

## Option A — GitHub Copilot CLI

1. Add the repository marketplace:

   ```text
   copilot plugin marketplace add AxaFrance/webengine-dotnet
   ```

2. Install the web profile:

   ```text
   copilot plugin install webengine-web@webengine-plugins
   ```

   For a direct source install, use `AxaFrance/webengine-dotnet:plugins/webengine-web`.

3. Enable the plugin; its MCP server starts automatically.
4. Verify with `copilot plugin list`. In VS Code, `MCP: List Servers` should show `webengine-web` and `Chat: Configure Skills` should show its skills.

For VS Code, use `Chat: Install Plugin From Source` with `AxaFrance/webengine-dotnet` or the direct plugin path `AxaFrance/webengine-dotnet:plugins/webengine-web`.

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
