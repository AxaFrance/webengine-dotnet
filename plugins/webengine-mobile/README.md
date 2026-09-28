# Plugin webengine-mobile (Appium)

Interactive gate between coding agents and a live Android/iOS app. Bundles skills + MCP server for **VS Code/Copilot** (Agent Plugins 1.0) and **Codex** in one folder.

```
plugins/webengine-mobile/
  plugin.json / mcp.json                # Agent Plugins 1.0 (Copilot/VS Code)
  .codex-plugin/plugin.json / .mcp.json # Codex
  skills/webengine-mobile/              # observe/act/generate skill
  skills/webengine-scaffold/            # solution scaffolding skill (mirror of web plugin's copy)
```

Prerequisites: .NET 10 SDK 10.0.100 or later + `AxaFrance.WebEngine.Mcp` on NuGet (see web plugin README for publish) + Appium server (default `http://localhost:4723`). Modern .NET is not included with Windows by default.

## Option A — GitHub Copilot CLI

1. Add the repository marketplace:

   ```text
   copilot plugin marketplace add AxaFrance/webengine-dotnet
   ```

2. Install the mobile profile:

   ```text
   copilot plugin install webengine-mobile@webengine-plugins
   ```

   For a direct source install, use `AxaFrance/webengine-dotnet:plugins/webengine-mobile`.

3. Enable the plugin; its MCP server starts automatically.
4. Verify with `copilot plugin list`. In VS Code, `MCP: List Servers` should show `webengine-mobile` and `Chat: Configure Skills` should show its skills.

For VS Code, use `Chat: Install Plugin From Source` with `AxaFrance/webengine-dotnet` or the direct plugin path `AxaFrance/webengine-dotnet:plugins/webengine-mobile`.

Manual: copy `skills/webengine-mobile` (+ `skills/webengine-scaffold`) to `.github/skills/`, and `mcp.json` into `.vscode/mcp.json`.

## Option B — Codex

```bash
codex plugin marketplace add AxaFrance/webengine-dotnet --sparse .agents/plugins
# then install webengine-mobile from the Plugins Directory
```

Manual: `codex mcp add webengine-mobile -- dnx AxaFrance.WebEngine.Mcp --profile mobile --transport stdio` + copy `skills/` to `~/.codex/skills/`.

## Local build fallback

See web plugin README (same steps with `--profile mobile`).

## Use

`start_session(platform, appPath, deviceName)` → `get_accessibility_snapshot` → `execute_bulk_actions` → `close_session`. Verify: 17 tools.
