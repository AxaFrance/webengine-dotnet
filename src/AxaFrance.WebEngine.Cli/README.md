# AxaFrance.WebEngine.Cli

`AxaFrance.WebEngine.Cli` is the command-line and local-daemon distribution
of WebEngine. The CLI is intentionally a thin client: browser sessions are
owned by one long-running daemon so separate agent commands can reuse the
same session.

## Install

```powershell
dotnet tool install --global AxaFrance.WebEngine.Cli
```

Add `--prerelease` only when the stable package is absent from the configured
NuGet feeds. To move an older installation to a newer version, run
`dotnet tool update --global AxaFrance.WebEngine.Cli`.

The tool requires the .NET 10 SDK. Browser automation also requires
a supported browser. Selenium Manager resolves the matching WebDriver for
Edge, Chrome, or Firefox when a session is opened. Edge is the default; use
`--browser` to select another engine.

## Daemon lifecycle

The daemon owns the browser sessions. It starts automatically when a web
command needs it, serves every later command, and stops itself after an idle
period (15 minutes by default) with no open session. Set
`WEBENGINE_DAEMON_IDLE_MINUTES` to tune the timeout, or `0` to disable it:

```text
webengine -c "daemon status"
webengine -c "daemon stop"
```

Each command is a short-lived process. Use one command per shell invocation
with `-c` (the equivalent direct subcommands also work):

```text
webengine -c "web session open"
webengine -c "web navigate --session <id> --url https://example.test"
webengine -c "web inspect --session <id>"
webengine -c "web click --session <id> --ref ref=3"
webengine -c "web session close --session <id>"
```

A command failure is returned as a structured error and a nonzero exit code.
Because no command holds a process open, a timed-out command never kills the
browser: the daemon keeps the session and the next command continues.

The default `webengine` command starts a persistent shell, and
`webengine --json` a prompt-free JSON-lines shell: send one command per line
and read one response line before sending the next. These shells are useful
for interactive debugging, but agent harnesses that cannot keep a process
handle between tool calls should use `-c` instead.

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
Prefer `--text-file` or `--stdin` for multiline values; in the `-c` form,
`--stdin` consumes the command's standard input.

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
