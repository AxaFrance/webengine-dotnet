---
name: webengine-mobile
description: Inspect and interact with live mobile apps via WebEngine MCP (Appium). Use when the user asks to observe an Android/iOS screen, tap/type/swipe, debug an app, or generate mobile UI code (Appium, WebEngine MobileApp C#).
license: MIT
metadata:
  author: axafrance
  version: "1.0"
---

# WebEngine Mobile — observe, act, generate

You drive a real device/emulator through WebEngine MCP Appium tools. Inspect first, act in bulk, generate in the user's stack.

## 1. Workflow (always this order)

1. `start_session(platform: 'Android'|'iOS', appPath, deviceName)` → `sessionId`. Ask for missing platform/app/device before proceeding.
2. `get_accessibility_snapshot(sessionId)` — default inspection (`[Role] accId="…" id="…" text="…"`).
3. `execute_bulk_actions(sessionId, [...])` — batch all same-screen actions.
4. `close_session(sessionId)` → log path. Share it.

Escalate to `get_page_source_chunk` / `get_page_source` only for missing attributes — see [inspection policy](references/inspection.md).

## 2. Locators (critical)

Priority: `AccessibilityId` (`content-desc`) > `Id` (short resource-id, strip package) > `UIAutomatorSelector` (Android) / `IosClassChain` / `IosPredicate` (iOS) > `Text` > `ClassName`+attribute > `XPath` (last resort).
Full table + native type map: [locators](references/locators.md).

## 3. Acting

- Single: `tap_element`, `long_press_element(durationMs)`, `type_text(clearFirst)`, `clear_text`, `wait_for_element`, `swipe_screen(Up|Down|Left|Right)`, `press_back` (Android), `hide_keyboard`, `take_screenshot`, `get_element_text`.
- Bulk `ActionType`: `Tap|TypeText|SetText|Clear|LongPress|SwipeUp|SwipeDown|SwipeLeft|SwipeRight`. Continues on failure.
- Every success returns `Element tag: <...xml.../>` — ground truth for codegen.

## 4. Gestures

`SwipeScreen Up` reveals content below; `Down` reveals above. After swipe, re-snapshot. `HideKeyboard` before tapping covered buttons. iOS back = tap visible back element, not `press_back`.

## 5. Generate code in the USER's stack

Ask once if unknown: language + framework (WebEngine MobileApp C#? raw Appium Java/Python?).
Mapping + PageModel rules: [codegen](references/codegen.md). Full solution scaffolding: skill `webengine-scaffold`.

## 6. Blocking points (system dialogs, onboarding, keyboard)

Protocol: failure → re-snapshot FIRST → classify (dismissible vs escalate) → dismiss once → re-snapshot → retry once.
Dismiss: permissions per scenario need, onboarding/rate-us carousels, OS popups if tappable, keyboard via `hide_keyboard` before taps.
STOP + ask human: biometric/PIN, wrong app start-state (report, don't improvise a login).
Full catalog + script setup rules: [blockers](references/blockers.md).
