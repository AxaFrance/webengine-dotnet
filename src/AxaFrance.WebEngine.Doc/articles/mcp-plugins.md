# WebEngine MCP Plugins for Coding Agents

WebEngine ships two plugins that turn any compatible coding agent into a browser/device operator: the agent observes the live application through MCP, interacts with it, then generates UI code in your stack (Playwright+TS, Selenium, WebEngine C#).

| Plugin | Profile | Tools | Skills |
|---|---|---|---|
| `plugins/webengine-web` | `--profile web` (Selenium) | 29 | `webengine-web`, `webengine-scaffold` |
| `plugins/webengine-mobile` | `--profile mobile` (Appium) | 17 | `webengine-mobile`, `webengine-scaffold` |

Both plugins share one server binary (`AxaFrance.WebEngine.Mcp`, command `webengine-mcp`) and carry two packaging formats in the same folder: Agent Plugins 1.0 (`plugin.json` + `mcp.json`) for VS Code/Copilot, and `.codex-plugin/plugin.json` + `.mcp.json` for Codex.

## Prerequisites

- .NET 10 (the `dnx` launcher ships with the SDK) for the zero-install run below.
- The package published to NuGet (maintainers: `dotnet pack` + `dotnet nuget push`). Before that, use the local-build fallback in each plugin README.
- A browser (Chrome/Edge/Firefox) for web; an Appium server (default `http://localhost:4723`) for mobile.

Test the server once (no agent needed):

```powershell
dnx AxaFrance.WebEngine.Mcp --profile web --transport stdio
# or, after dotnet tool install -g AxaFrance.WebEngine.Mcp:
webengine-mcp --help
```

## GitHub Copilot (VS Code)

Full plugin (skills + server, recommended):

1. Run `Chat: Install Plugin From Source` and paste `https://github.com/AxaFrance/webengine-dotnet`, or register the marketplace `AxaFrance/webengine-dotnet` in `chat.plugins.marketplaces`, or point `chat.pluginLocations` at a local clone's `plugins/<name>` folder.
2. Enable the plugin. Its MCP server starts automatically with it.
3. Verify: `MCP: List Servers` shows `webengine-web`/`webengine-mobile`; `Chat: Configure Skills` lists the skills.

Manual (no plugin install): copy `plugins/<name>/skills/*` to `.github/skills/` and the `mcp.json` content into `.vscode/mcp.json`. This repository's own `.vscode/mcp.json` is a working example (local build).

## Codex (CLI, IDE extension, desktop)

```bash
codex plugin marketplace add AxaFrance/webengine-dotnet --sparse .agents/plugins
# then install webengine-web / webengine-mobile from the Plugins Directory
```

This repository also ships `.agents/plugins/marketplace.json` for repo-scoped installs. Manual alternative:

```bash
codex mcp add webengine-web -- dnx AxaFrance.WebEngine.Mcp --profile web --transport stdio
```

plus copying `plugins/<name>/skills/*` to `~/.codex/skills/` (restart Codex). Project-scoped MCP goes in `.codex/config.toml` (trusted projects); see `plugins/<name>/config.toml.snippet`.

## Claude Code

Copy `plugins/<name>/skills/*` to `.claude/skills/` (project) or `~/.claude/skills/` (personal), and register the server in `claude_desktop_config.json`:

```json
{ "mcpServers": { "webengine-web": {
  "command": "dnx",
  "args": ["AxaFrance.WebEngine.Mcp", "--profile", "web", "--transport", "stdio"]
} } }
```

Restart Claude Code and verify the tools list.

## OpenCode

Skills: copy `plugins/<name>/skills/*` to `.agents/skills/` (project, also picked up by Codex/Cursor) or `~/.config/opencode/skills/`. Server in `opencode.json`:

```json
{ "$schema": "https://opencode.ai/config.json",
  "mcp": { "webengine-web": { "type": "local",
    "command": ["dnx", "AxaFrance.WebEngine.Mcp", "--profile", "web", "--transport", "stdio"],
    "enabled": true } } }
```

(OpenCode v2 nests servers under `mcp.servers`; check your version's docs.)

## Cursor

Project skills in `.cursor/skills/` (global: `~/.cursor/skills/`), server in `.cursor/mcp.json` using the same stdio command as above.

## Troubleshooting

- **No tools / garbled handshake**: plain `dotnet run` prints a launch banner to stdout, which breaks MCP stdio. Use `dnx`, `dotnet tool`, a published exe, or `dotnet exec <dll>`.
- **`dnx` cannot resolve the package**: it is not on NuGet yet — use the local-build fallback from the plugin README.
- **Mobile session fails**: start the Appium server first and check `appsettings.json` (`Appium:DefaultServerUrl`).
- **Both tool sets collide** (`StartSession` twice): never expose both profiles in one process; use `--profile web|mobile`.
- **Click intercepted / stale refs**: this is page behavior, not an install issue — the skills' `references/blockers.md` (cookies, modals, SPA re-renders) covers the protocol.
