# Tutorials & Samples
In this series of documentation, we will learn how to develop Test Automation Solutions with the WebEngine Framework, and how this Framework can help you build these solutions easier and faster.

## Selenium WebDriver
AxaFrance WebEngine Framework is based on Selenium WebDriver. In this document we assume that you know already following basic concepts of Selenium:

* Locator Strategies: That is use Class name, Css selector, Id, Name, Xpath or other locators to identify one or more specific elements in the DOM.
* Find Web Elements: By using functions of FindElement (by id, by name, by xpath ...)
* Interactions: Like Click on a button, Fill a Textbox, Select an option from Radio button group...

> [!NOTE]
> In this tutorials, we won't discuss basic concepts of Selenium and Selenium WebDriver. For these subjects, please refer to the [documentation of Selenium](https://www.selenium.dev/documentation/)
>
> For Mobile related concepts, please refer to the [documentation of Appium](https://appium.io/docs/en/about-appium/intro/)

## Basic Concepts
Understand basic concepts about
* [How to identify UI Elements on Web Application](web-elements.md)
* [How to identify UI Elements on Mobile Application](app-elements.md)
* [How to organize UI Elements into a Page Model](page-model.md)
* How to initialize Web Driver for [Web Testing](web-driver.md) and [Mobile App Testing](appium-driver.md)
* [Handling secure passwords in test data](secure-password.md)
* [Using the MCP Server for AI-assisted browser automation](../articles/github-copilot.md)

## Test Approach and sample projects

Not sure which approach to choose? Here is a quick guide:

| Approach | Best suited for | Reusability | Test data |
|---|---|---|---|
| **Linear Scripting** | Quick scripts, unit-style tests | Low | Hardcoded |
| **Gherkin / BDD** | Feature acceptance tests in BDD teams | Medium | In scenario |
| **Keyword-Driven** | Complex end-to-end tests | High | Optional |
| **Keyword + Data-Driven** | Large regression suites, multi-environment | Highest | Excel / external |

> [!TIP]
> For most professional end-to-end test projects, the **Keyword-Driven + Data-Driven** combination is recommended. It gives the highest reusability and lowest long-term maintenance cost.

* [Introduction to Keyword-Driven Test Method](keyworddriven.md)
* Workarounds by using different approach as follow

|Test Approach | .NET Sample | Java Sample|
|---------------|-------------|------------|
| Linear Scripting Approach | [Linear Scripting .NET](linear-script-cs.md) | [Linear Scripting JAVA](linear-script-java.md) |
| Structured Approach (Gherkin) | [Specflow](gherkin-cs.md) | [JUnit](gherkin-java.md) |
| Keyword Driven Approach | [Keyword Driven](keyword-driven-cs.md) | [Keyword Driven](keyword-driven-java.md) |
| Data Driven Approach | [Data Driven](data-driven-cs.md) | [Data Driven](data-driven-java.md) |

## Going further

Once you have a working test automation project, explore these topics to make your solution production-ready:

* [Test Configuration (appsettings.json)](../articles/appsettings.md) — browser options, grid connection, encryption key
* [Integrate with CI/CD pipelines](ci-cd.md) — run your tests automatically on every build
* [Built-in Best Practices](../articles/best-practices.md) — overview of all practices the framework implements for you
* [GitHub Copilot integration](../articles/github-copilot.md) — let AI generate PageModels and Actions following WebEngine conventions



