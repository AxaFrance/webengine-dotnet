# WebEngine MCP Plugins for Coding Agents

WebEngine ships two plugins that turn any compatible coding agent into a browser/device operator: the agent observes the live application through MCP, interacts with it, then generates UI code in your stack (Playwright+TS, Selenium, WebEngine C#).

| Plugin | Profile | Tools | Skills |
|---|---|---|---|
| `plugins/webengine-web` | `--profile web` (Selenium) | 29 | `webengine-web`, `webengine-scaffold` |
| `plugins/webengine-mobile` | `--profile mobile` (Appium) | 17 | `webengine-mobile`, `webengine-scaffold` |

Both plugins share one server binary (`AxaFrance.WebEngine.Mcp`, command `webengine-mcp`) and carry two packaging formats in the same folder: Agent Plugins 1.0 (`plugin.json` + `mcp.json`) for VS Code/Copilot, and `.codex-plugin/plugin.json` + `.mcp.json` for Codex.

## Prerequisites

- .NET 10 SDK 10.0.100 or later (the `dnx` launcher ships with the SDK) for the zero-install run below. Modern .NET is not included with Windows by default.
- The package published to NuGet (maintainers: use `scripts/prepare-mcp-release.ps1` and the guarded release workflow). Before that, use the local-build fallback in each plugin README.
- A browser (Chrome/Edge/Firefox) for web; an Appium server (default `http://localhost:4723`) for mobile.

Test the server once (no agent needed):

```powershell
dnx AxaFrance.WebEngine.Mcp --profile web --transport stdio
# or, after dotnet tool install -g AxaFrance.WebEngine.Mcp:
webengine-mcp --help
```

## GitHub Copilot CLI

The Copilot CLI marketplace is repository-hosted; there is no separate package upload. After the repository metadata is pushed to GitHub:

```bash
copilot plugin marketplace add AxaFrance/webengine-dotnet
copilot plugin marketplace browse webengine-plugins
copilot plugin install webengine-web@webengine-plugins
# or: copilot plugin install webengine-mobile@webengine-plugins
```

The catalog is `.github/plugin/marketplace.json`, and each plugin source is under `plugins/`. For a direct source install, use `AxaFrance/webengine-dotnet:plugins/webengine-web` or `AxaFrance/webengine-dotnet:plugins/webengine-mobile`.

## GitHub Copilot (VS Code)

Full plugin (skills + server):

1. Run `Chat: Install Plugin From Source` and paste `https://github.com/AxaFrance/webengine-dotnet`, or use a direct `AxaFrance/webengine-dotnet:plugins/<name>` source, or point `chat.pluginLocations` at a local clone's `plugins/<name>` folder.
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

## MCP Registry and release automation

`server.json` publishes one combined stdio server entry (`io.github.AxaFrance/webengine-mcp`) for the `both` profile. The NuGet package README contains the ownership marker required by the MCP Registry. The Copilot marketplace remains split into web and mobile profiles so their tool names do not collide.

To prepare locally without publishing:

```powershell
powershell -ExecutionPolicy Bypass -File scripts/prepare-mcp-release.ps1
```

The guarded `.github/workflows/publish-mcp.yml` workflow publishes only tags named `mcp-v<version>`. Before using it, configure the `mcp-registry-publish` GitHub environment with a `NUGET_API_KEY`, protected release/tag rules, and (recommended) a required reviewer. The job uses GitHub OIDC for `mcp-publisher`; the publishing identity must be authorized for the `io.github.AxaFrance` namespace.

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
