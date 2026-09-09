# Blocking points (mobile) — system and app overlays that are NOT the test subject

Same protocol as web: Detect → Classify → Dismiss → Re-snapshot → Retry once.
Mobile adds one rule: **never improvise around identity or system state** — wrong start state (logged-out vs logged-in, fresh install vs upgraded) invalidates the whole run. If the app is not in the expected state, report instead of improvising a login.

## Catalog

| Blocker | Detection signal | Treatment |
|---|---|---|
| System permission dialog (camera, location, notifications) | System package in source, buttons « Autoriser »/« Allow » | Accept/deny per scenario need via text locator; log as setup |
| Biometric / PIN / lock screen | Non-app screen, no app hierarchy | STOP, ask the human to authenticate, resume |
| Onboarding carousel, push-opt-in, rate-us prompt | First-launch screens before the expected start screen | Dismiss once (swipe/close), log as setup |
| Software keyboard covering the CTA | Tap fails on a button below the input | `hide_keyboard` before tapping; re-snapshot |
| OS popups (update, battery saver) | System UI outside app | Dismiss if tappable, else escalate |
| Spinner / skeleton lists | Same snapshot twice with loading indicators | `wait_for_element` on the next stable element, not a fixed sleep |
| WebView screens | Web content inside native tree | Native locators only (see skill); no context switching |
| Deep link / cold start variance | App opens on unexpected screen | Re-align: navigate within app to the expected start screen, or report wrong-state |

## For generated scripts

- Setup ensures start state (reinstall flag or logout/login keyword) before the first step.
- Permission/onboarding dismissal as exception-safe setup keywords with `Exists()` guards.
- Every tap after text input calls hide-keyboard first (page-object helper).
