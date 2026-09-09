---
name: webengine-scaffold
description: Scaffold a UI automation solution with WebEngine (C#) or a generic stack. Use when the user asks to create a test project, choose gherkin/unit/keyword/data-driven, install packages, or structure PageModels/Actions/TestCases.
license: MIT
metadata:
  author: axafrance
  version: "1.0"
---

# WebEngine Scaffold — from observation to solution

<!-- Maintainers: mirrored in plugins/webengine-web/skills/webengine-scaffold — edit both. -->

Use after `webengine-web` / `webengine-mobile` observation, or standalone when bootstrapping a project.

## 1. Ask two questions first (if unknown)

1. **Stack**: WebEngine C# (Selenium/Appium)? Or generic (Playwright TS, Appium Java/Python)? Default: follow current repo.
2. **Approach** (WebEngine C#):
   - Linear Scripting — simple/unit, PageModels directly, no SharedAction/TestCase.
   - BDD/Gherkin (Reqnroll) — `.feature` + step defs (ask for step class if missing).
   - Keyword-Driven — `PageModels/` + `Actions/SharedAction*` + `TestCases/TestCase*` + `TestData/` XML.
   - Data-Driven — parameterize with XML/Excel datasets.

Detect existing approach from project structure and state it before generating.

## 2. Packages and drivers

- Web: `AxaFrance.WebEngine.Web`; Mobile: `AxaFrance.WebEngine.MobileApp`; Keyword only: `AxaFrance.WebEngine.Runner`. Check refs; install or ask user.
- Driver: `BrowserFactory.GetDriver(Platform.Windows, BrowserType.Chrome)` / `AppFactory.GetDriver(Platform.Android)`.
- Generic stacks: Playwright `npm i -D @playwright/test`, Appium Java/Python per their docs — then map observed `ElementTag`s (see web/mobile skill codegen refs).

## 3. Generation rules (all approaches)

- Locators from captured `ElementTag` only; PageModel owns descriptions (never in SharedAction/test).
- PageModel: properties `get; set;`, no driver in description ctor, ctor takes `WebDriver`.
- SharedAction: `DoAction` + `DoCheckpoint` (Arrange-Assert), `RequiredParameters => null` unless specified, externalize data via `GetParameter` + `ParameterList`.
- Overlays are not the test but break the run: always generate a `DismissOverlays()` setup (cookie-accept + promo-close, `Exists()`-guarded, exception-safe, called in `TestInitialize`/`BeforeScenario`) plus `Exists(timeout)` guards before clicks on overlay-prone pages. See web/mobile skill `references/blockers.md`.
- Structure: [structure](references/structure.md). Test-data XML + `ParameterList`: same file.

## 4. Output order

1. PageModels from log tags. 2. Actions/steps per approach. 3. TestCases + data. 4. Run instructions.
