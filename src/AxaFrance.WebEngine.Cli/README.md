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
Chrome, Edge, or Firefox when a session is opened.

## Daemon lifecycle

```powershell
webengine daemon start --json
webengine daemon status --json
webengine daemon stop --json
```

## Web commands

The current release exposes persistent desktop web sessions:

```powershell
webengine web session open --headless --json
webengine web session list --json
webengine web navigate --session <id> --url https://example.test --json
webengine web inspect --session <id> --json
webengine web click --session <id> --ref ref=3 --json
webengine web type --session <id> --selector '#query' --text 'example' --json
webengine web type --session <id> --selector '#notes' --text-file .\notes.txt --json
Get-Content .\notes.txt -Raw | webengine web type --session <id> --selector '#notes' --stdin --json
webengine web key --session <id> --selector '#query' --key Enter --json
webengine web select --session <id> --selector '#country' --text 'France' --json
webengine web actions --session <id> --json
webengine web session close --session <id> --json
```

`web type` requires exactly one of `--text`, `--text-file`, or `--stdin`.
File input is read as UTF-8 and both file and stdin content are sent as one
value, so embedded CR/LF characters are preserved without being placed in the
daemon command line. These modes do not bypass the action log: only recognized
password fields are redacted. Use `web key` for intentional Enter, Tab, Escape,
Backspace, Delete, Space, Home, End, PageUp, PageDown, or arrow-key actions.

Inspection references are invalidated after navigation and after every
action. Inspect again before using another `--ref`. The action log records
the element metadata used by each successful action, and secret field values
are redacted.

Mobile/Appium commands are not exposed yet. They will be added through the
shared WebEngine automation service without changing the CLI/daemon contract.

Use `--pipe <name>` for development or for isolating multiple local daemon
instances. The default pipe name is scoped to the current user.
