---
applyTo: '**/*.cs'
author: 'Huaxing YUAN'
---
# Context for Copilot - Test Automation
We are working on Test Automation using Selenium WebDriver customized with WebEngine Framework.
Speak with me in French.

Framework repository: https://github.com/AxaFrance/webengine-dotnet

## We use following approaches to write test cases: 
- Linear Scripting : for unit or very simple scenarios. We translate test scenario to test scripts directly.
- Behavior-Driven Development : where the test scenarios are written in gherkin format with `Reqnroll` (specflow is no more maintained).
- Keyword-Driven Testing : where the test scenario are written using keywords that represent actions or verifications in the application under test.

You will analyze the structure of the projet to determine the approach, and tell me which approach is used.

## When writing test case, follow these guidelines for every approaches :
- Do not use hardcoded locators such as xpath or css selectors in test script when possible. Use ElementDescription instead.
- Add Web elements into PageObject Models and refer it in test actions.
- Apply PageObject Model design pattern to separate UI element locators to test script.
- when suggest test script, use native functions from WebEngine Framework if possible.
- **DO NOT** create ElementDescription directly in SharedActions, add them to approprate PageModels

For linear scripting and Reqroll, DO NOT use SharedActions or TestCaseWeb/TestCaseApp classes, write the script direcly by using PageObjectModel.
For Keyword-Driven, you should use SharedActions to implemented reusable action groups.
For Gherkin, you can also use shared action inside step definition to maximum reuse.
- Use parameters in actions, and use parameter list to store all parameters used in the test case.
- In actions, perform actions using PageObject Models and their elements.

For Gherkin approach, if user not give the class of step definitions, ask user before create new one.

For a Keyword-Driven test project, it should be structured as follow based on current active project, using current projet's namespace.
- folder PageModels: store all page object models, under namespace
- folder Actions: store all SharedActions (which should be reused in different test cases)
- folder TestCases: store all test scenarios
- folder TestData: store all test data files, such as XML or Excel files
folders should be inside project folder, not the solution folder.

## Nuget Packages required:
- AxaFrance.WebEngine.Web: for web based tests (based on selenium)
- AxaFrance.WebEngine.MobileApp: for mobile apps (based on appium)
- AxeFrance.WebEngine.Runner: only used for Keyword-Driven approach
Install required packages or ask user to install them if they are not referenced.

# WebDriver or AppiumDriver
Use BrowserFactory to get WebDriver or AppiumDriver instance.
var driver = BrowserFactory.GetDriver(Platform.Windows, BrowserType.Edge);
var driver = AppFactory.GetDriver(Platform.Android)

Platform is defined in namespace: AxaFrance.WebEngine

# Use ElementDescription to find web elements

Prioritize stable identifiers such as TagName, Id, Name, or other html attributes (test-id, accessibility for example)
to make sure the locators are stable and not likely to change. you can combine multiple locators to make it unique.
Use CssSelector or XPath only when there is no other stable attributes available.

## WebElementDescription for elements on Web Page:
WebElementDescription class has following properties:
- ClassName: equivalent to html attribute class. class name can contain spaces.
- CssSelector: use Css Selector to locate the Web Element.
- Id: equivalent to html attribute id
- InnerText: The text within the html tag.
- LinkText: The text within a html hyperlink. (usually in tag a)
- Name: equivalent to html attribute name
- TagName: the name of the html tag. ex: div, a, input, ...
- XPath: use XPath to locate the Web Element/
- Attributes: any standard or non-standard html attributes.
- Attributes.Name: the name of the attribute.
- Attributes.Value: the value of the attribute.

### Actions 
WebElementDescription expose following methods to interact with web element:
* Click()
* SendKeys(string)
* SetValue(string)
* SetSecure(string) - Set the value of a password textbox using encrypted string.
* Exists()
* Clear()
* MouseHover()
* ScrollIntoView()
* SelectByIndex(int) - Select an option from html tag by its displayed text.
* SelectByText(string) - Select an option from html tag by its displayed text.
* SelectByValue(string) - Select an option from html tag by its value.
* CheckByValue(string) - Checks an RadioButton from a given RadioGroup (used for radio button group, elements)

### Example of a WebElementDescription in C# used individualy:
```csharp
using AxaFrance.WebEngine.Web;
using AxaFrance.WebEngine;

//Identify element with Id
var SelectLanguage = new WebElementDescription(driver)
{
    Id = "language"
};

//Identify element with multiple locators
var SelectLanguage = new WebElementDescription(driver)
{
    TagName = "div",
    ClassName = "class1 class2"
};

//Identify element with customized dom attributes.
var customDiv = new WebElementDescription(driver)
{
    Attributes = new HtmlAttribute[]
    {
        new HtmlAttribute("custom_name", "scroll_intoview"),
    }
};
```

## AppElementDescription to find elements on Native Application (based on Appium):
AppElementDescription class has following properties:
- AccessibilityId: the accessibility id property
- ClassName: the class property
- Id: The id property
- XPath: The xpath property
- Text: The text property
- Content-Description: The content-desc property

For specific device, you can also use:
- UIAutomatorSelector: a native selector using UIAutomator on the Android.
- IosClassChain: a native selector provided by iOS device.

The above two selectors can only be used individually, when they are provided all other selectors will be ignored.


## Organize UI Elements With PageModel
AppElementDescription and WebElementDescription can be used to create Page Model classes.
To test Web applications, use WebElementDescription instead of AppElementDescription.

Example: CalculatorPage is the model used to test android application, which derives from PageModel class.
Attention: 
- Do not passe Driver in the constructor of WebElementDescription or AppElementDescription if it is used inside a PageModel.
Page model will take care of it.
- WebElementDescription and AppElementDescription are properties, with (get and set)

```csharp
using OpenQA.Selenium;
using AxaFrance.WebEngine.Web;
using AxaFrance.WebEngine.MobileApp;

namespace YouTestProjectNS.PageModels{
	public class CalculatorPage : PageModel
	{
		public AppElementDescription Digit0 {get;set;} = new AppElementDescription
		{
			Id = "com.Android.calculator2:id/digit_0"
		};

		public AppElementDescription Equals {get;set;} = new AppElementDescription
		{
			Id = "com.Android.calculator2:id/eq"
		};

		public AppElementDescription Multiply {get;set;} = new AppElementDescription
		{
			ClassName = "Android.widget.Button",
			AccessbilityId = "multiply"
		};

		public CalculatorPage(WebDriver driver) : base(driver)
		{
		}
	}
}
```
In Page mode, we use WebDriver and not IWebDriver

# Keyword-Driven approaches in WebEngine
if use Keyword-Driven approaches, the test case will be defined like this:
1. We declare a class derived from TestCaseWeb or TestCaseApp, and test steps in the constructor.
2. Each test step is assigned with an action which is a class that derives from SharedActionWeb or SharedActionApp.
```scharp
using AxaFrance.WebEngine;
using AxaFrance.WebEngine.Web;

namespace Samples.KeywordDriven.TestCases
{
    [Description("functional name of the test case")]
    public class TC_InsuranceQuote : TestCaseWeb
    {
        public TC_InsuranceQuote()
        {
            TestSteps = new TestStep[] {
                new TestStep{ Action = nameof(Login)},
                new TestStep{ Action = nameof(SearchProspect)},
                new TestStep{ Action = nameof(Underwriting)},
                new TestStep{ Action = nameof(ChooseOfferOptions)},
                new TestStep{ Action = nameof(ValidateContract)},
                new TestStep{ Action = nameof(Logout)},
            };
        }
    }
}
```

3.  `SharedActionWeb` has DoAction and DoCheckpoint to perform actions and assertions following Arrange-Test-Assert pattern
This is an example of the LoginAction: We uses PageModels and their elements to perform actions.
Test data is externalized so we can use GetParameter("name") to get it's value
Keep RequiredParameters empty in generated code.
```csharp
using AxaFrance.WebEngine;
using AxaFrance.WebEngine.Web;
using OpenQA.Selenium;

public class Login : SharedActionWeb
    {
        public override Variable[]? RequiredParameters => null;

        // Runs the action to fill username, password and lick on login button.
        public override void DoAction()
        {
            Browser.Navigate().GoToUrl(GetParameter("URL_RECETTE"));
            PageModels.PageLogin login = new PageModels.PageLogin(Browser);
            login.UserName.SetValue(GetParameter("User"));
            login.UserName.SetSecure(GetParameter("EncPassword"));
            login.ButtonLogin.Click();
        }

        // Verifies if this action goes well.
        public override bool DoCheckpoint()
        {
            PageModels.PageLogin login = new PageModels.PageLogin(Browser);
            if (login.ErrorMessage.Exists(5) && !string.IsNullOrWhiteSpace(login.ErrorMessage.InnerText))
            {
                Information.AppendLine("Error message is shown, login failed");
                return false;
            }
            return true;
        }
    }
```

4. (SharedActionApp) in namespace AxaFrance.WebEngine.AppMobile it's exactly the same for mobile app
The only difference is that AppiumDriver is provided as parameter for DoAction() and DoCheckpoint():
public abstract void DoAction(AppiumDriver driver);
public abstract bool DoCheckpoint(AppiumDriver driver);


## Test Data
WebEngine supports XML format test data files. test data is in following structure:
Each structure TestData contains a TestName and Data with Variables used for a test case.
ParameterList is used only for Keyword Driven approach.
```xml
<?xml version="1.0" encoding="utf-8"?>
<TestSuiteData 
  xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" 
  xmlns:xsd="http://www.w3.org/2001/XMLSchema" 
  xmlns="http://www.axa.fr/WebEngine/2022">
  <TestData>
    <TestName>Addition1</TestName>
    <Data>
      <Variable>
        <Name>TESTCASE</Name>
        <Value>Addition1</Value>
      </Variable>
      <Variable>
        <Name>ENVIRONNEMENT</Name>
        <Value>FR</Value>
      </Variable>
    </Data>
  </TestData>
</TestSuiteData>
 ```

## ParameterList
To void using hardcoded variable name in the actions, we can use a class named ParameterList
to store all parameters used in the test case.
ParameterList is used only for Keyword Driven approach.
for example:
```csharp
public static class ParameterList {

	///<Summary>Parameter: Le nom de cas de test. Il doit être unique dans chaque test suite (dont une feuille Excel) </Summary>
	public static string TESTCASE {get; } = "TESTCASE";

	///<Summary>Parameter: Workflow of the current test case, decides which script will be used to test current scenario. SIMPLE for basic named based search, ADVANCED for multiple filter based search </Summary>
	public static string WORKFLOW {get; } = "WORKFLOW";

	///<Summary>Parameter: The test Environment. Possible values: DEV, TEST, STAGING </Summary>
	public static string ENVIRONMENT {get; } = "ENVIRONMENT";
}
```
When genereting script, we can use ParameterList.Variable instead of using strings in GetParameter;
