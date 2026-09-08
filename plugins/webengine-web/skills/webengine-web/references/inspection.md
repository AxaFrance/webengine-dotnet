# Inspection policy (web)

| Tool | When |
|---|---|
| `get_accessibility_snapshot` | First choice — refs + stable attributes, ~2-5 KB |
| `get_actionable_html` | Snapshot lacks visibility context; forms (70-80% smaller than full HTML) |
| `get_page_html` (≤30 KB) | Prose/styling context needed |
| `get_page_html_chunk(i, 10KB)` | Large pages; chunk 0 first for `TotalChunks` |
| `take_screenshot` | Visual verify / debug unexpected state |

Re-inspect only: element missing, URL/size jump after action, message to verify, locator failure. Batch same-page actions via `execute_bulk_actions` before re-inspecting.
