# Blocking points (web) — overlays and traps that are NOT the test subject

Learned from real runs (e.g. axa.fr: cookie dialog intercepted the first click on « Devis Habitation »).
Rule of thumb: the agent optimizes the happy path and will retry a failed click blindly.
Never do that — an interception almost always means the DOM changed under you.

## Protocol: Detect → Classify → Dismiss → Re-snapshot → Retry once

1. **Detect**: click fails (`intercepted`, `not clickable`, `stale`) or refs suddenly match nothing → re-snapshot FIRST, do not retry the same ref.
2. **Classify** the overlay from the fresh snapshot:
   - Dismissible → step 3 (cookies, promo, chat, survey, sticky bars).
   - Escalate → STOP and report (CAPTCHA, bot wall, unexpected login/SSO, payment 3DS).
3. **Dismiss once** via its own close/accept control (prefer `Id`/test-id). One attempt only.
4. **Re-snapshot** (dismissal re-renders; old refs are dead), then retry the original action **once**.
5. **Log** the dismissal as a setup step. Never assert business behavior on an overlay.

## Catalog

| Blocker | Detection signal in snapshot | Treatment |
|---|---|---|
| Consent / cookies (OneTrust `#onetrust-accept-btn-handler`, Axeptio, custom e.g. `id="footer_tc_privacy_button"`) | `[dialog]` + accept/personalize buttons, sometimes in iframe | Accept once per session; guard with `Exists()` in scripts |
| Promo / newsletter / exit-intent modal | `[dialog]` appearing after delay/scroll, close X | Close, never fill marketing fields unless the scenario asks |
| Chat / survey widgets (Intercom, Qualtrics) | Fixed corner iframe covering CTAs | Close widget, or `scroll_to_element` (center) before clicking |
| Sticky header/footer, floating cookie-settings button | Click lands on wrong element at viewport edge | Scroll target to center, screenshot-verify |
| Unexpected login / SSO / MFA redirect | URL jumps to login domain | STOP, ask user to authenticate, then resume. Never invent credentials |
| CAPTCHA / bot wall (Datadome, Cloudflare challenge) | Checkbox « je ne suis pas un robot », « verify you are human » | STOP + report as blocked. Never attempt bypass |
| New tab opened by click | URL + snapshot unchanged after click | Known MCP gap (no switch-window tool): read target `href` from `ElementTag` and `navigate_to` it directly; note it in the report |
| Iframe content (payment, embeds) | `<iframe>` in HTML but empty snapshot region | Flag it: generated locators need frame context; inspect via `execute_script` |
| Shadow DOM (web components) | Element visible but unresolvable | Flag it: generated code needs JS piercing |
| SPA loading (skeletons, `aria-busy`) | Spinners, disabled submit (seen: « Étape suivante » disabled until valid) | Fill required fields first; `wait_for_element` on the enabled state; never click disabled |
| A/B variant | Same URL, different hero/CTA text than expected | Describe actual DOM; prefer text locators over index/position |
| Inline validation errors after submit | Error text near field | This is a CHECKPOINT, not a blocker: capture text, report, stop the scenario — do not invent data to force it through |
| File download | Success message but no new DOM | MCP has no download tool: verify via URL/message and note the limitation |
| i18n pages | Full-sentence assertions | Assert on `Id`/keys, never on full sentences |

## For generated scripts (essential)

Overlays are not the test but they break it. Every generated suite must include:
- `DismissOverlays()` in setup (`TestInitialize` / `BeforeScenario`): cookie-accept + promo-close, each `Exists()`-guarded and exception-safe.
- `Exists(timeout)` guards before clicks on pages known to show overlays.
- Conditional handling only — never hard-assert that an overlay is absent.
