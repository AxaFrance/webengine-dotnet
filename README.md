# AXA WebEngine Framework

A Test Automation Framework (TAF) is a set of guidelines, libraries for creating and designing test cases.
It is conceptual part of the automated testing, provides common functionalities and best practices that helps Test Automation Engineers to create reliable, maintainable, and well-structured Test Automation Solutions (TAS).

AXA WebEngine Framework makes it easier to build highly effective test automation solutions for **Web**, **Mobile Web** and **Mobile App** testing.

Built by Test Guild of AXA France, available in **.NET** and **JAVA** and used in dozens of test automation projects within AXA France. We decided to open source this framework to share our knowledge on Test Automation to the community and hopes we can improve the framework (and the quality of IT Systems, or course 😉) together.

## Why WebEngine Framework?
WebEngine Framework resolves some common problems every test automation project can face, and it simplifies the writing of test scripts.
* It supports all Selenium based technologies on Desktop Browsers, Mobile Browsers on different Mobile Devices.
* Using **Browser Factory**, there is no need to download various selenium WebDriver when browser and its version changes.
* Description oriented **Web Element** identification and synchronization with browser out-of-box.
* Organizing Web Element in **Page Model** makes test scripts easy to read and to maintain.
* Fully object-oriented and compatible with **Keyword-Driven** and **Data-driven** approach.
* Compatible with other another Unit test frameworks such as NUnit, Cucumber
* Easy configuration and Set-up for Execution: Run test directly from Excel, as well as on DevOps platforms
* Graphical test report.
* **Open Source**, free usage and let’s improve it together!

## Getting started.
* Introduction: Explains common concepts of the framework.
* API Reference: Explains every class, method and property.
* Tutorials: Follow some step by step turotial to build first test automation solution using different approach.

https://axafrance.github.io/webengine-dotnet/

## Use the latest version
The Framework is distributed via Package Management:
When using .NET, please find AxaFrance.WebEngine packages on nuget.org
When using JAVA, these pacakges will be available on Maven

## MCP (Model Context Protocol) Integration

WebEngine includes an **MCP server** that enables AI-powered coding agents (like GitHub Copilot) to observe your application, execute tests, and generate UI test scripts automatically.

**Key Features:**
- **Observe**: Capture page state, inspect elements, and analyze accessibility
- **Execute**: Interact with UI elements and perform bulk actions on live applications
- **Generate**: Automatically create code in your stack (Playwright+TS, Selenium, WebEngine C#) from observed behavior

**Plugins (stdio, recommended):** one `webengine-mcp` binary, two plugins — `plugins/webengine-web` (Selenium, 29 tools) and `plugins/webengine-mobile` (Appium, 17 tools) — each bundling Agent Skills + MCP server for Copilot (Agent Plugins 1.0) and Codex. Details in `plugins/README.md`.

**Install the plugins in your coding agent:**

| Agent | Install |
|---|---|
| GitHub Copilot CLI | `copilot plugin marketplace add AxaFrance/webengine-dotnet`, then install `webengine-web@webengine-plugins` or `webengine-mobile@webengine-plugins` |
| GitHub Copilot (VS Code) | `Chat: Install Plugin From Source` → this repository or a plugin subdirectory |
| Codex (CLI/IDE/desktop) | `codex plugin marketplace add AxaFrance/webengine-dotnet --sparse .agents/plugins` |
| Claude Code | copy `plugins/<name>/skills/*` to `.claude/skills/` + stdio entry in `mcpServers` |
| OpenCode | copy skills to `.agents/skills/` + `type: local` entry in `opencode.json` |
| Cursor | copy skills to `.cursor/skills/` + entry in `.cursor/mcp.json` |

The standard NuGet distribution runs without a local repository clone via `dnx AxaFrance.WebEngine.Mcp --profile <web|mobile> --transport stdio`, but requires the **.NET 10 SDK 10.0.100 or later**. Modern .NET is not included with Windows by default. Full per-agent guide: documentation article *MCP Plugins for Coding Agents*.

**For detailed information on running the MCP server locally, available tools, and how to use it with coding agents, see [WebEngineMCP.md](src/WebEngineMCP.md).**

## WebEngine 2.0 Roadmap
We are working on the next version of WebEngine Framework, in the next versions we will bring.
- [ ] Enhanced Page-Object Model.
- [ ] Enhanced Element Description Model (with more attributes and relative-identification mode)
- [ ] Integrates Acessibility Testing with axe-core library and comprehensive reports.

To get detailed information with this roadmap, please check [WebEngine 2.0 Roadmap](WebEngine2.0.md)


## Contact us
Feel free to reach us if you want to adopt the Framework, report Bugs, or have good ideas to contribute on it.

#### Repository of .NET Project and shared components:
+ https://github.com/AxaFrance/webengine-dotnet
+ Main contributor: Huaxing YUAN [<img src="src/AxaFrance.WebEngine.Doc/images/linked-in.svg" width="16" />](https://www.linkedin.com/in/huaxing-yuan/) [<img src="src/AxaFrance.WebEngine.Doc/images/github.svg" width="16" />](https://github.com/huaxing-yuan) [<img src="src/AxaFrance.WebEngine.Doc/images/twitter.svg" width="16" />](https://twitter.com/huaxing_yuan)

#### Repository Java Project:
+ https://github.com/AxaFrance/webengine-java
+ Main contributor:
    + Joseph ARUL [<img src="src/AxaFrance.WebEngine.Doc/images/github.svg" width="16" />](https://github.com/josepharul82)
    + Jean-Prince DOTOU-SEGLA [<img src="src/AxaFrance.WebEngine.Doc/images/github.svg" width="16" />](https://github.com/JeanPrince)
