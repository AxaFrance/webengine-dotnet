---
name: webengine-web
description: Inspect and interact with live web pages via WebEngine MCP (Selenium). Use when the user asks to observe a browser, click/type/select, debug a page, or generate web UI code (Playwright TS, Selenium C#/Java/Python, WebEngine C#).
license: MIT
metadata:
  author: axafrance
  version: "1.0"
---

# WebEngine Web — observe, act, generate

You drive a real browser through WebEngine MCP tools. You do NOT guess DOM. You inspect first, act in bulk, then generate code in the user's stack.

## 1. Workflow (always this order)

1. `start_session(browserType: 'Chrome', headless: false)` → `sessionId`.
2. `navigate_to(sessionId, url)`.
3. `get_accessibility_snapshot(sessionId)` — default inspection. Compact, one ref per element.
4. `execute_bulk_actions(sessionId, [...])` — submit ALL actions from one snapshot in one call.
5. `close_session(sessionId)` → action-log path. Share it with the user.

Escalate inspection only when needed — see [inspection policy](references/inspection.md).

## 2. Refs and locators (critical)

- Every element in the snapshot has `ref=N`. Pass it as `Element.Ref` — zero-guess mapping (Playwright-style).
- Refs die on re-render (React/Vue/Angular). After any mutating action, re-snapshot before new refs.
- Fallback priority: `Id` > `Name` > test attribute (`data-testid`) > `aria-label` > `TagName+InnerText` > `LinkText` (links only) > `ClassName` (stable only) > `CssSelector`/`XPath` (last resort).
- Full table + examples: [locators](references/locators.md).

## 3. Acting

- Single tools: `click_element`, `type_text`, `set_text`, `select_from_dropdown_by_text|value`, `check_element`, `uncheck_element`, `wait_for_element`, `scroll_by`, `scroll_to_element`, `click_at` (canvas last resort), `take_screenshot`, `execute_script`.
- Bulk `ActionType`: `Click|TypeText|SetText|Clear|SelectByText|SelectByValue|Check|Uncheck`. Continues on failure — check `Results[]`, retry failures individually.
- Every success returns `Element tag: <...>` — the ground truth for codegen. Never invent a locator.

## 4. Generate code in the USER's stack

Ask once if unknown: language + framework (Playwright TS? Selenium C#? WebEngine C#? Java? Python?).

- Mapping `ElementTag` → target stack: [codegen](references/codegen.md).
- For WebEngine C# (PageModel + approaches): [webengine](references/webengine.md).
- For scaffolding a full solution (gherkin/unit/keyword/data-driven): activate skill `webengine-scaffold`.

## 5. Rules

- Re-inspect only on: missing element, URL change, validation message to verify, locator failure.
- Batch same-page actions; split on navigation.
- Communicate the `close_session` log path; offer to generate PageModels/tests from it.

## 6. Blocking points (cookies, modals, traps)

Overlays are not the test but they break it. Protocol: failed/intercepted click → re-snapshot FIRST (never blind-retry) → classify (dismissible vs escalate) → dismiss once → re-snapshot (old refs are dead) → retry original action once → log dismissal as setup.
Dismiss: cookies/promo/chat/survey/sticky bars. STOP + report: CAPTCHA/bot wall, unexpected login/SSO, payment iframe. Validation errors are CHECKPOINTS (capture, report, stop — don't invent data). Full catalog + script setup rules (`DismissOverlays()` with `Exists()` guards): [blockers](references/blockers.md).
