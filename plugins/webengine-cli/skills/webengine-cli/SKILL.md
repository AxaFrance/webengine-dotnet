---
name: webengine-cli
description: Use the local WebEngine CLI daemon for persistent browser automation without MCP.
---

# WebEngine CLI

Use the `webengine` command as a one-shot client of the local WebEngine
daemon. Each command is a short-lived process that talks to the daemon over a
local named pipe, so no command holds a session open in the harness and a
timed-out command never kills the browser. Do not start a browser process for
every action and do not invoke internal daemon commands directly.

Required CLI version: **3.26.282** (`AxaFrance.WebEngine.Cli` uses
`major.YY.dayOfYear.revision` builds, so later day-of-year builds satisfy this
requirement)

## Bootstrap the CLI

Before using any `webengine` command, make sure the required version is
installed. An installed tool is reused when its version satisfies the
requirement, and installed or updated otherwise:

```powershell
$toolPath = Join-Path $HOME '.dotnet/tools'
if (Test-Path -LiteralPath $toolPath) {
    $env:PATH = $toolPath + [IO.Path]::PathSeparator + $env:PATH
}

function Get-WebEngineVersion {
    $command = Get-Command webengine -ErrorAction SilentlyContinue
    if ($null -eq $command) {
        return $null
    }

    $output = & $command.Source --version 2>&1
    if ($LASTEXITCODE -ne 0) {
        return $null
    }

    $line = $output | Where-Object { $_ -match '\d+\.\d+\.\d+' } | Select-Object -First 1
    if ($null -eq $line) {
        return $null
    }

    return [Version]([regex]::Match($line, '\d+\.\d+\.\d+').Value)
}

$requiredVersion = [Version]'3.26.282'
$installedVersion = Get-WebEngineVersion

if ($null -eq $installedVersion) {
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
elseif ($installedVersion -lt $requiredVersion) {
    $stableOutput = & dotnet tool update --global AxaFrance.WebEngine.Cli 2>&1
    $stableExitCode = $LASTEXITCODE

    if ($stableExitCode -ne 0) {
        $details = $stableOutput -join [Environment]::NewLine
        if ($details -notmatch '(?i)(not found in NuGet feeds|NU1101|no stable)') {
            throw "The WebEngine CLI update failed: $details"
        }

        & dotnet tool update --global AxaFrance.WebEngine.Cli --prerelease
        if ($LASTEXITCODE -ne 0) {
            throw "The stable package was unavailable and the prerelease WebEngine CLI update also failed."
        }
    }
}

$installedVersion = Get-WebEngineVersion
if ($null -eq $installedVersion) {
    throw "The WebEngine CLI is not available after installation."
}

if ($installedVersion -lt $requiredVersion) {
    throw "The WebEngine CLI is version $installedVersion but $requiredVersion or later is required."
}

webengine --version
```

Use the equivalent commands for the host shell when PowerShell is not
available. Only fall back to `--prerelease` when the stable package is absent
from the configured NuGet feeds, and never hide SDK, permission, network, or
other installation errors as prerelease retries. Update the installed tool only
to satisfy the required version above, not on every run.

## Run commands

Run one command per shell invocation with `-c`. The first web command starts
the daemon automatically and every later command reuses it. The daemon stops
itself after an idle period with no open session:

```text
webengine -c "web session open"
webengine -c "web navigate --session <id> --url https://example.test"
webengine -c "web wait --session <id> --text ""Ready"""
webengine -c "web inspect --session <id>"
webengine -c "web click --session <id> --ref ref=3"
webengine -c "web session close --session <id>"
```

Direct subcommands (`webengine web inspect --session <id>`) are equivalent to
the `-c` form. Quote the command string with the host shell quoting rules
(PowerShell doubles a double quote inside a double-quoted string), and prefer
`--text-file` or `--stdin` for multiline or sensitive values instead of nesting
escaped quotes. A failed command returns a structured error and a nonzero exit
code; it does not end the workflow, so report it and continue only when the
next action is still valid.

The persistent JSON-lines shell (`webengine --json`) remains available for
interactive debugging, but most agent harnesses cannot keep a process handle
between tool calls, so prefer `-c` for agent workflows.

Daemon control is optional and rarely needed because the daemon starts and
stops on its own:

```powershell
webengine -c "daemon status"
webengine -c "daemon stop"
```

## Choose visible or headless mode deliberately

The default browser session is visible. Do not add `--headless` when the user
wants to observe the browser, inspect a workflow, build or maintain tests, or
debug locators and actions:

```text
webengine -c "web session open"
```

Use `--headless` for CI/CD pipelines, cloud or remote agent sessions without a
desktop display, or when the user explicitly requests a background run.

Do not silently switch an observation or test-authoring workflow to headless.
If a visible browser cannot be launched in the current environment, report
that limitation and ask before switching to headless mode.

If the daemon is unavailable, report the structured error instead of silently
starting an alternate implementation.

## Web session workflow

Use one explicit web session for a browser workflow:

```text
webengine -c "web session open"
webengine -c "web navigate --session <id> --url https://example.test"
webengine -c "web inspect --session <id>"
webengine -c "web click --session <id> --ref ref=3"
webengine -c "web type --session <id> --selector '#query' --text 'example'"
webengine -c "web check --session <id> --id agreeTerms"
webengine -c "web uncheck --session <id> --name marketing"
webengine -c "web select --session <id> --name country --text France"
webengine -c "web type --session <id> --selector '#notes' --text-file .\notes.txt"
webengine -c "web key --session <id> --selector '#query' --key Enter"
webengine -c "web actions --session <id>"
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

The current CLI web surface supports Chrome, Edge, and Firefox. Mobile/Appium
commands are not available yet.

Never place passwords, tokens, encryption keys, payment data, or real personal
data in command-line arguments. Do not submit quotes, purchases, applications,
or other irreversible transactions without explicit user authorization.
