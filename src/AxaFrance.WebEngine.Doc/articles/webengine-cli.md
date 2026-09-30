# WebEngine CLI and local daemon

The WebEngine CLI provides an MCP-free integration path for organizations
where MCP servers are not permitted. An agent invokes the `webengine` .NET
tool through a skill. Each CLI invocation is short-lived; a local daemon owns
the browser drivers and keeps sessions alive between commands.

## Architecture

```text
Agent skill
  -> webengine CLI process
  -> local named-pipe protocol
  -> webengine daemon process
  -> Selenium WebDriver
  -> browser
```

The CLI package is `AxaFrance.WebEngine.Cli` and exposes the `webengine`
command. The same executable runs as the client and as `webengine daemon run`,
which keeps the client and daemon protocol versions aligned. The daemon is
local-only and uses a per-user named pipe by default. Use `--pipe` to isolate
development or test instances.

The CLI does not contain an MCP manifest. The existing MCP server remains a
separate distribution path.

## Installation

The standard tool package targets `net10.0` and requires the **.NET 10 SDK
10.0.100 or later**. Modern Windows installations do not include the required
SDK by default.

```powershell
dotnet tool install --global AxaFrance.WebEngine.Cli --prerelease
webengine daemon start --json
webengine daemon status --json
```

Browser sessions require Chrome, Edge, or Firefox. Selenium Manager resolves
the matching WebDriver when the browser is opened. The first browser launch
may therefore require network access to obtain a driver.

## Daemon lifecycle

```powershell
webengine daemon start --json
webengine daemon status --json
webengine daemon stop --json
```

`stdout` contains one JSON response when `--json` is used. Diagnostics are
written to `stderr`. A daemon started by `daemon start` is detached from the
invoking CLI and remains available for later commands.

## Web sessions

Open one explicit session and keep its returned ID for subsequent commands:

```powershell
webengine web session open --headless --json
webengine web session list --json
```

Navigate and inspect:

```powershell
webengine web navigate --session <id> --url https://example.test --json
webengine web inspect --session <id> --json
webengine web html --session <id> --json
```

`web inspect` returns visible form controls, links, buttons, ARIA roles, and
references such as `ref=3`. References are valid only for the inspected DOM
state and are invalidated after navigation or an action.

Act on the latest inspection:

```powershell
webengine web click --session <id> --ref ref=3 --json
webengine web type --session <id> --selector '#query' --text 'example' --json
webengine web type --session <id> --selector '#notes' --text-file .\notes.txt --json
Get-Content .\notes.txt -Raw | webengine web type --session <id> --selector '#notes' --stdin --json
webengine web key --session <id> --selector '#query' --key Enter --json
webengine web select --session <id> --selector '#country' --text 'France' --json
webengine web actions --session <id> --json
```

Each successful action records the actual element tag and available stable
attributes in the session action log. This makes it possible to review which
controls were touched without relying on guessed locators.

`web type` requires exactly one of `--text`, `--text-file`, or `--stdin`.
`--text-file` reads UTF-8 text and both file and stdin input are sent as one
value, preserving embedded CR/LF characters without putting the content in
the daemon command line. These modes do not bypass the action log: only
recognized password fields are redacted. Use `web key --key Enter` or another
supported named key when a form requires an intentional key press; a newline
in a single-line field is not an equivalent command.

Close sessions explicitly:

```powershell
webengine web session close --session <id> --json
```

## Protocol and operational boundaries

The CLI and daemon exchange newline-delimited JSON over the local named pipe.
The protocol currently includes:

| Command | Purpose |
|---|---|
| `daemon.ping` | Return protocol and process status |
| `daemon.shutdown` | Stop the daemon |
| `web.session.open` | Create a Chrome, Edge, or Firefox session |
| `web.session.list` | List active sessions |
| `web.session.close` | Quit a browser session |
| `web.navigate` | Navigate an existing session |
| `web.inspect` | Return actionable visible elements |
| `web.html` | Return bounded page source |
| `web.click` | Click an inspected ref or CSS selector |
| `web.type` | Type into an inspected ref or CSS selector |
| `web.key` | Send a supported named key to an inspected ref or CSS selector |
| `web.select` | Select a native `<select>` option |
| `web.actions` | Return the session's action log |

Errors use stable machine-readable codes such as `daemon_not_running`,
`session_not_found`, `inspection_required`, `element_not_found`, and
`web_driver_error`.

The daemon is intentionally local-only. Keep browser sessions and named-pipe
access scoped to the current user. Do not put passwords, tokens, encryption
keys, payment data, or real personal data in command-line arguments or action
logs. Exploratory workflows must stop before irreversible submissions.

## Current limits and roadmap

The current CLI slice implements desktop web sessions and the daemon
protocol. Mobile/Appium commands are not exposed yet, and the MCP and CLI
adapters do not yet share a transport-neutral automation service. The next
engineering phase should extract the existing Selenium/Appium managers and
action models into that shared service, then add mobile commands and
integration tests using a local test page.
