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

After bootstrap, use one persistent JSON-lines shell for a workflow:

```text
webengine --json
```

Send one command per input line and read one JSON response line before sending
the next command. Use `webengine -c "<command>"` only when the host cannot keep
a process handle. The daemon owns browser sessions and the shell owns one
reusable named-pipe connection. Command failures are returned as structured
errors; they do not terminate the shell.

The default browser session is visible, which is useful when observing a
workflow, building tests, or debugging locators:

```text
web session open
web session list
web navigate --session <id> --url https://example.test
web wait --session <id> --text "Ready"
web inspect --session <id>
web click --session <id> --ref ref=3
web type --session <id> --selector '#search' --text 'example'
web check --session <id> --id agreeTerms
web uncheck --session <id> --name marketing
web select --session <id> --name country --text France
web select --session <id> --selector '#country' --text 'France'
web actions --session <id>
web session close --session <id>
```

Use `--headless` for CI/CD, cloud agents without a desktop display, or when
the user explicitly asks for a background run. Do not silently make an
observation or test-authoring workflow headless.

Use `web wait` for SPA routes and asynchronous page states instead of inserting
arbitrary sleeps:

```text
web wait --session <id> --url /userstory/TQSI-3457
web wait --session <id> --text "Test Cases"
web wait --session <id> --selector "[role='alert']"
```

For `web type`, use exactly one text source: `--text`, `--text-file`, or
`--stdin`. File input is UTF-8 and file/stdin content is preserved as one
value, including CR/LF characters. Use `web key --key Enter` (or another
supported named key) when a form action requires an intentional key press
instead of relying on newline characters in a single-line input.
File and stdin input avoid command-line exposure but do not bypass the action
log; only recognized password fields are redacted.
In the persistent shell, use `--text-file` for multiline values because stdin
is the command channel. Reserve `--stdin` for one-shot direct commands.

Mobile/Appium commands are not yet exposed by this plugin. Never place
passwords, tokens, encryption keys, or real personal data in command-line
arguments, and stop before irreversible submissions.
