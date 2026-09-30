---
name: webengine-cli
description: Use the local WebEngine CLI daemon for persistent browser automation without MCP.
---

# WebEngine CLI

Use the `webengine` command as a thin client to the local WebEngine daemon.
Do not start a new browser process for every action and do not invoke internal
daemon commands directly.

## Start and verify the daemon

```powershell
webengine daemon start --json
webengine daemon status --json
```

The command returns one JSON response on stdout. Diagnostics belong on stderr.
If the daemon is unavailable, report the structured error instead of silently
starting an alternate implementation.

## Web session workflow

Use one explicit web session for a browser workflow:

```powershell
webengine web session open --headless --json
webengine web navigate --session <id> --url https://example.test --json
webengine web inspect --session <id> --json
webengine web click --session <id> --ref ref=3 --json
webengine web type --session <id> --selector '#query' --text 'example' --json
webengine web type --session <id> --selector '#notes' --text-file .\notes.txt --json
Get-Content .\notes.txt -Raw | webengine web type --session <id> --selector '#notes' --stdin --json
webengine web key --session <id> --selector '#query' --key Enter --json
webengine web actions --session <id> --json
webengine web session close --session <id> --json
```

1. Inspect the live page before every new action sequence.
2. Prefer `--ref` values from the latest inspection; use a CSS selector only
   when the inspection returned a stable selector.
3. Re-inspect after navigation, DOM mutation, or a failed action because refs
   are intentionally invalidated after every action.
4. Close the session when the task is complete.

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
