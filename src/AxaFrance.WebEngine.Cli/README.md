# AxaFrance.WebEngine.Cli

`AxaFrance.WebEngine.Cli` is the command-line and local-daemon distribution
of WebEngine. The CLI is intentionally a thin client: browser sessions are
owned by one long-running daemon so separate agent commands can reuse the
same session.

## Install

```powershell
dotnet tool install --global AxaFrance.WebEngine.Cli --prerelease
```

The tool currently requires the .NET 10 SDK. Browser automation also requires
a supported browser. Selenium Manager resolves the matching WebDriver for
Edge, Chrome, or Firefox when a session is opened. Edge is the default; use
`--browser` to select another engine.

## Daemon lifecycle

```text
webengine --json
daemon start
daemon status
```

The default `webengine` command starts an interactive shell. Use
`webengine --json` for a prompt-free JSON-lines shell: send one command per
line and read one response line before sending the next. The shell keeps one
named-pipe connection open for the workflow. Use `webengine -c "<command>"`
when a host cannot keep a process handle; direct subcommands remain available
for compatibility. A command failure is returned as an error response and
does not terminate the shell.

## Web commands

The default browser session is visible:

```text
web session open
web session list
web navigate --session <id> --url https://example.test
web wait --session <id> --text "Ready"
web inspect --session <id>
web click --session <id> --ref ref=3
web type --session <id> --selector '#query' --text 'example'
web check --session <id> --id agreeTerms
web uncheck --session <id> --name marketing
web select --session <id> --name country --text France
web type --session <id> --selector '#notes' --text-file .\notes.txt
web key --session <id> --selector '#query' --key Enter
web select --session <id> --selector '#country' --text 'France'
web actions --session <id>
web session close --session <id>
```

Use `--headless` for CI/CD or cloud/remote sessions without a desktop
display. Do not silently use headless mode when the user wants to observe the
browser or build and debug tests.

Use `web wait` for a SPA route, visible text, or CSS selector that proves an
asynchronous page state is ready. This avoids arbitrary sleeps:

```text
web wait --session <id> --url /userstory/TQSI-3457
web wait --session <id> --text "Test Cases"
web wait --session <id> --selector "[role='alert']"
```

`web type` requires exactly one of `--text`, `--text-file`, or `--stdin`.
File input is read as UTF-8 and both file and stdin content are sent as one
value, so embedded CR/LF characters are preserved without being placed in the
daemon command line. These modes do not bypass the action log: only recognized
password fields are redacted. Use `web key` for intentional Enter, Tab, Escape,
Backspace, Delete, Space, Home, End, PageUp, PageDown, or arrow-key actions.
In the persistent shell, use `--text-file` for multiline values because stdin
is the command channel. Reserve `--stdin` for one-shot direct commands.

Inspection references are invalidated after navigation and after every
action. Inspect again before using another `--ref`. The action log records
the element metadata used by each successful action, and secret field values
are redacted.

Inspection metadata is collected in one browser-side snapshot instead of
issuing separate WebDriver calls for every attribute on every element.
Reference-based actions reuse that snapshot metadata; use `--limit` when a
workflow only needs a small part of a large page.
Inspection also reports accessible names, associated labels, values, checkbox
state, expanded state, links, status roles, and alert roles when the page
exposes them.

Element actions use the same native locator concepts as WebEngine:
`--ref`, `--id`, `--name`, `--tag`, `--element-text`, `--link-text`,
`--class`, `--aria-label`, `--xpath`, `--selector`, and `--index`.

Mobile/Appium commands are not exposed yet. They will be added through the
shared WebEngine automation service without changing the CLI/daemon contract.

Use `--pipe <name>` for development or for isolating multiple local daemon
instances. The default pipe name is scoped to the current user.
