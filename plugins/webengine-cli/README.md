# WebEngine CLI plugin

This plugin provides agent skills for the `webengine` .NET tool and its local
daemon. It deliberately contains no MCP configuration, for environments where
MCP servers are not permitted.

The skill checks for the `webengine` command on first use. If it is missing,
the agent installs the stable NuGet package. When no stable package is
available, it retries with `--prerelease`. Installation and SDK, permission,
or network errors are reported instead of being hidden by an unconditional
fallback.

```powershell
webengine --version
```

The daemon owns browser sessions across CLI invocations. The current web
surface supports:

```powershell
webengine web session open --headless --json
webengine web session list --json
webengine web navigate --session <id> --url https://example.test --json
webengine web inspect --session <id> --json
webengine web click --session <id> --ref ref=3 --json
webengine web type --session <id> --selector '#search' --text 'example' --json
webengine web type --session <id> --selector '#notes' --text-file .\notes.txt --json
Get-Content .\notes.txt -Raw | webengine web type --session <id> --selector '#notes' --stdin --json
webengine web key --session <id> --selector '#search' --key Enter --json
webengine web select --session <id> --selector '#country' --text 'France' --json
webengine web actions --session <id> --json
webengine web session close --session <id> --json
```

For `web type`, use exactly one text source: `--text`, `--text-file`, or
`--stdin`. File input is UTF-8 and file/stdin content is preserved as one
value, including CR/LF characters. Use `web key --key Enter` (or another
supported named key) when a form action requires an intentional key press
instead of relying on newline characters in a single-line input.
File and stdin input avoid command-line exposure but do not bypass the action
log; only recognized password fields are redacted.

Mobile/Appium commands are not yet exposed by this plugin. Never place
passwords, tokens, encryption keys, or real personal data in command-line
arguments, and stop before irreversible submissions.
