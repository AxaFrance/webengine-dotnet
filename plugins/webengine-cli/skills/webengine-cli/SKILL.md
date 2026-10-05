---
name: webengine-cli
description: Use the local WebEngine CLI daemon for persistent browser automation without MCP.
---

# WebEngine CLI

Use the `webengine` command as a persistent shell over the local WebEngine
daemon. Do not start a new browser process for every action and do not invoke
internal daemon commands directly.

## Bootstrap the CLI

Before using any `webengine` command, check whether the command is available
in the current environment. If it is missing, install the global .NET tool
without requiring the user to run a separate setup command:

```powershell
$toolPath = Join-Path $HOME '.dotnet/tools'
if (Test-Path -LiteralPath $toolPath) {
    $env:PATH = $toolPath + [IO.Path]::PathSeparator + $env:PATH
}

$command = Get-Command webengine -ErrorAction SilentlyContinue
if ($null -eq $command) {
    $stableOutput = & dotnet tool install --global AxaFrance.WebEngine.Cli 2>&1
    $stableExitCode = $LASTEXITCODE

    if ($stableExitCode -ne 0) {
        $details = $stableOutput -join [Environment]::NewLine
        if ($details -notmatch '(?i)(not found in NuGet feeds|NU1101|no stable)') {
            throw "The stable WebEngine CLI installation failed: $details"
        }

        & dotnet tool install --global AxaFrance.WebEngine.Cli --prerelease
        if ($LASTEXITCODE -ne 0) {
            throw "The stable package was unavailable and the prerelease WebEngine CLI installation also failed."
        }
    }
}

if ($null -eq (Get-Command webengine -ErrorAction SilentlyContinue)) {
    throw "The WebEngine CLI is not available after installation."
}

webengine --version
```

Use the equivalent commands for the host shell when PowerShell is not
available. Retry with `--prerelease` only when the stable package is absent
from the configured NuGet feeds. Do not hide SDK, permission, network, or
other installation errors by retrying them as prerelease installations.
Do not update an already installed CLI unless the user explicitly requests it.

## Keep one shell process

After the one-time bootstrap, prefer one long-lived JSON-lines shell for the
whole workflow:

```text
webengine --json
```

Keep its standard input and output open. Send one command per input line,
without the `webengine` prefix, and read exactly one JSON response line before
sending the next command:

```text
daemon start
web session open
web navigate --session <id> --url https://example.test
web wait --session <id> --text "Ready"
web inspect --session <id>
web click --session <id> --ref ref=3
web session close --session <id>
daemon stop
exit
```

The shell keeps one named-pipe connection to the daemon, which avoids starting
a new CLI process and reconnecting for every action. Use `webengine -c
"<command>"` when the host cannot keep a process handle. Direct subcommands
remain available for compatibility, but they are slower for multi-step
workflows. A failed command returns a structured error and does not end the
shell; report it and continue only when the next action is still valid.

## Choose visible or headless mode deliberately

The default browser session is visible. Do not add `--headless` when the user
wants to observe the browser, inspect a workflow, build or maintain tests, or
debug locators and actions:

```text
web session open
```

Use `--headless` for CI/CD pipelines, cloud or remote agent sessions without a
desktop display, or when the user explicitly requests a background run:

```text
web session open --headless
```

Do not silently switch an observation or test-authoring workflow to headless.
If a visible browser cannot be launched in the current environment, report
that limitation and ask before switching to headless mode.

## Start and verify the daemon

```powershell
daemon start
daemon status
```

In a JSON-lines shell, each command returns one JSON response on stdout.
Diagnostics belong on stderr. If the daemon is unavailable, report the
structured error instead of silently starting an alternate implementation.

## Web session workflow

Use one explicit web session for a browser workflow:

```text
web session open
web navigate --session <id> --url https://example.test
web inspect --session <id>
web click --session <id> --ref ref=3
web type --session <id> --selector '#query' --text 'example'
web check --session <id> --id agreeTerms
web uncheck --session <id> --name marketing
web select --session <id> --name country --text France
web type --session <id> --selector '#notes' --text-file .\notes.txt
web key --session <id> --selector '#query' --key Enter
web actions --session <id>
web session close --session <id>
```

1. Inspect the live page before every new action sequence.
2. Prefer `--ref` values from the latest inspection; use a CSS selector only
   when the inspection returned a stable selector.
3. Use `web wait` after navigation or asynchronous actions when the page has a
   route, text, or selector that proves it is ready.
4. Re-inspect after navigation, DOM mutation, or a failed action because refs
   are intentionally invalidated after every action.
5. Use the returned `label`, `accessibleName`, `value`, `checked`, and
   `expanded` metadata before falling back to raw HTML.
6. Close the session when the task is complete.

Element actions accept the native WebEngine locator description. Prefer
`--ref`, then semantic locators such as `--id`, `--name`, `--tag`,
`--element-text`, `--link-text`, `--class`, `--aria-label`, or `--xpath`.
Use `--selector` only when those locators are insufficient, and use
`--index` when the description intentionally matches multiple elements.

For `web type`, pass exactly one of `--text`, `--text-file`, or `--stdin`.
Prefer `--text-file` or `--stdin` for multiline text so it is not exposed in
the process command line. File input is UTF-8, and embedded CR/LF characters
are preserved as one value. These modes do not bypass the action log; only
recognized password fields are redacted. Newlines are not a substitute for an
intentional key press: use `web key --key Enter`, `Tab`,
`Escape`, `Backspace`, `Delete`, `Space`, `Home`, `End`, `PageUp`, `PageDown`,
or `ArrowUp`/`ArrowDown`/`ArrowLeft`/`ArrowRight` as appropriate.

In the persistent shell, standard input is the command channel, so use
`--text-file` for multiline values. Use `--stdin` only with a one-shot direct
command or `webengine -c`, where standard input is dedicated to the text.

The current CLI web surface supports Chrome, Edge, and Firefox. Mobile/Appium
commands are not available yet.

Never place passwords, tokens, encryption keys, payment data, or real personal
data in command-line arguments. Do not submit quotes, purchases, applications,
or other irreversible transactions without explicit user authorization.
