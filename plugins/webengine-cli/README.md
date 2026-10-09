# WebEngine CLI plugin

This plugin provides agent skills for the `webengine` .NET tool and its local
daemon. It deliberately contains no MCP configuration, for environments where
MCP servers are not permitted.

The skill requires CLI version 3.26.282 or later (`AxaFrance.WebEngine.Cli`
uses `major.YY.dayOfYear.revision` builds, so later day-of-year builds also
satisfy this requirement). On first use, the agent checks
the installed `webengine` version: a satisfying version is reused, a missing or
older tool is installed or updated, and the version is verified afterwards. The
stable NuGet package is preferred; the agent retries with `--prerelease` only
when no stable package is available. Installation and SDK, permission, or
network errors are reported instead of being hidden by an unconditional
fallback. The tool is only updated to satisfy the required version, never
unprompted.

```powershell
webengine --version
```

Run one command per shell invocation with `-c`. The first web command starts
the daemon automatically, later commands reuse it, and the daemon stops itself
after an idle period with no open session:

```text
webengine -c "web session open"
webengine -c "web navigate --session <id> --url https://example.test"
webengine -c "web wait --session <id> --text ""Ready"""
webengine -c "web inspect --session <id>"
webengine -c "web click --session <id> --ref ref=3"
webengine -c "web session close --session <id>"
```

Direct subcommands (`webengine web inspect --session <id>`) are equivalent to
the `-c` form. Command failures are returned as structured errors with a
nonzero exit code; they do not end the workflow. Because no command holds a
process open in the harness, a timed-out command never kills the browser: the
daemon keeps the session and the next command continues. The persistent
JSON-lines shell (`webengine --json`) remains available for interactive
debugging, but prefer `-c` when the host cannot keep a process handle.

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
log; only recognized password fields are redacted. Prefer `--text-file` or
`--stdin` for multiline values instead of nesting escaped quotes inside the
`-c` command string.

Mobile/Appium commands are not yet exposed by this plugin. Never place
passwords, tokens, encryption keys, or real personal data in command-line
arguments, and stop before irreversible submissions.
