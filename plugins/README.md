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
Workspace dogfood (local build): `.vscode/mcp.json`.

Transports: `--transport stdio` for all local plugins (`http` kept for backward compat at `/mcp`).
Install details per plugin: see each `README.md`.
