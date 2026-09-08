# Inspection policy (mobile)

| Tool | When |
|---|---|
| `get_accessibility_snapshot` | First choice — one line per element, ~5-30 KB |
| `get_page_source_chunk(0, 15KB)` | Snapshot missing bounds/platform attributes |
| `get_page_source` | Rarely — full XML |
| `take_screenshot` | Visual verify / debug |

Re-inspect only: element missing, screen/activity change, alert to verify, locator failure. Batch same-screen actions via `execute_bulk_actions`.
