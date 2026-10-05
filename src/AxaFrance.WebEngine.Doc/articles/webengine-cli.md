# WebEngine CLI and local daemon

The WebEngine CLI provides an MCP-free integration path for organizations
where MCP servers are not permitted. An agent invokes the `webengine` .NET
tool through a skill. The default shell stays in memory while a local daemon
owns the browser drivers and keeps sessions alive between commands.

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
```

Browser sessions use Edge by default and also support Chrome and Firefox.
Selenium Manager resolves the matching WebDriver when the browser is opened.
The first browser launch may therefore require network access to obtain a
driver.

## Daemon lifecycle

```text
webengine --json
daemon start
daemon status
```

The default `webengine` command starts an interactive shell. Use
`webengine --json` for a prompt-free JSON-lines shell. Send one command per
input line and read one JSON response line before sending the next:

```text
daemon start
web session open
web navigate --session <id> --url https://example.test
web wait --session <id> --text "Ready"
web inspect --session <id>
web click --session <id> --ref ref=3
web check --session <id> --id agreeTerms
web uncheck --session <id> --name marketing
web select --session <id> --name country --text France
web session close --session <id>
daemon stop
exit
```

The shell keeps one CLI process and named-pipe connection in memory. Use
`webengine -c "<command>"` when the host cannot keep a process handle.
Direct subcommands remain available for compatibility. Diagnostics are written
to `stderr`. Command failures return structured errors without terminating the
shell, and an abrupt client disconnect does not terminate the daemon.

## Web sessions

Open one explicit session and keep its returned ID for subsequent commands:

```text
web session open
web session list
```

Navigate and inspect:

```text
web navigate --session <id> --url https://example.test
web inspect --session <id>
web html --session <id>
```

`web inspect` returns visible form controls, links, buttons, ARIA roles, and
references such as `ref=3`. References are valid only for the inspected DOM
state and are invalidated after navigation or an action.

Act on the latest inspection:

```text
web click --session <id> --ref ref=3
web type --session <id> --selector '#query' --text 'example'
web type --session <id> --selector '#notes' --text-file .\notes.txt
web key --session <id> --selector '#query' --key Enter
web select --session <id> --selector '#country' --text 'France'
web actions --session <id>
```

The default browser session is visible for observation, test authoring, and
locator debugging. Add `--headless` for CI/CD or cloud/remote sessions
without a desktop display. Do not silently switch an observation workflow to
headless mode.

Use `web wait` for SPA routes, visible text, or CSS selectors that prove an
asynchronous page state is ready. It avoids arbitrary sleeps:

```text
web wait --session <id> --url /userstory/TQSI-3457
web wait --session <id> --text "Test Cases"
web wait --session <id> --selector "[role='alert']"
```

Each successful action records the actual element tag and available stable
attributes in the session action log. This makes it possible to review which
controls were touched without relying on guessed locators.

Inspection metadata is collected in one browser-side snapshot rather than
through separate WebDriver calls for every attribute on every candidate.
Actions using a reference reuse the metadata from the latest inspection.
Use `--limit` when a page contains many controls and the workflow only needs
the first relevant part of the inspection.

Element actions use the same native locator concepts as WebEngine:
`--ref`, `--id`, `--name`, `--tag`, `--element-text`, `--link-text`,
`--class`, `--aria-label`, `--xpath`, `--selector`, and `--index`.
Checkboxes and radio buttons use `web check` and `web uncheck`; native
`<select>` controls use `web select` with visible text or option value.

`web type` requires exactly one of `--text`, `--text-file`, or `--stdin`.
`--text-file` reads UTF-8 text and both file and stdin input are sent as one
value, preserving embedded CR/LF characters without putting the content in
the daemon command line. These modes do not bypass the action log: only
recognized password fields are redacted. Use `web key --key Enter` or another
supported named key when a form requires an intentional key press; a newline
in a single-line field is not an equivalent command.
In the persistent shell, use `--text-file` for multiline values because stdin
is the command channel. Reserve `--stdin` for one-shot direct commands.

Close sessions explicitly:

```text
web session close --session <id>
```

## Protocol and operational boundaries

The CLI and daemon exchange newline-delimited JSON over the local named pipe.
The protocol currently includes:

| Command | Purpose |
|---|---|
| `daemon.ping` | Return protocol and process status |
| `daemon.shutdown` | Stop the daemon |
| `web.session.open` | Create an Edge, Chrome, or Firefox session |
| `web.session.list` | List active sessions |
| `web.session.close` | Quit a browser session |
| `web.navigate` | Navigate an existing session |
| `web.inspect` | Return actionable visible elements |
| `web.html` | Return bounded page source |
| `web.click` | Click an inspected ref or CSS selector |
| `web.type` | Type into an inspected ref or CSS selector |
| `web.key` | Send a supported named key to an inspected ref or CSS selector |
| `web.select` | Select a native `<select>` option |
| `web.check` | Check a checkbox or radio button |
| `web.uncheck` | Uncheck a checkbox |
| `web.wait` | Wait for a URL, visible text, or CSS selector condition |
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
