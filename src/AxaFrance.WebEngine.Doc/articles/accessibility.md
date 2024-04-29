# Accessibility testing with WebEngine
In the latested WebEngine Framework, we have integrated accessibility testing features to help you ensure your applications are accessible to all users. The Framework uses the Axe-core library to perform accessibility tests on your applications and provide you with a overall and detailed report of the issues found.

You can run accessibility tests on two different levels:
- **Page Level**: This level will run the accessibility tests on the current page.
- **User Journey Level**: This level will run the accessibility test on the entire user journey and shows you a overall test report.

## Prerequisites
Having WebEngine version 1.2 and above, working on Windows and MacOS.

## Run accessibility tests
### Run accessibility test on a single page
To run accessibility tests on a single page, you can use the following code snippet:
```csharp
// Create a new instance of webdriver using Selenium
using var driver = BrowserFactory.GetDriver(AxaFrance.WebEngine.Platform.Windows, BrowserType.ChromiumEdge);

// Navigate to the page you want to test
driver.Navigate().GoToUrl("https://www.axa.fr");

// Do your orginal functional tests here
var filename = new PageReportBuilder()
    .WithSelenium(driver)
    .Build()
    .Export();

// filename will contain the path to the generated report
```
In resume, what you need to do create a new instance of `PageReportBuilder`, pass the Selenium driver to it, and call the Build method to generate the report.

### Scan options
If you want to customize the scan options, you can use the following code snippet:
```csharp
var filename = new PageReportBuilder()
    .WithOptions(new PageReportOptions()
    {
        HighlightColor = Color.Green,
        HighlightThickness = 5,
        ScoringMode = ScoringMode.Weighted,
        Tags = PageReportOptions.WcagAATags,
        ScreenshotIncomplete = true,
        UseAdvancedScreenshot = true,
    })
    .WithSelenium(driver).Build().Export();
```
The above code snippet will run the accessibility tests with the following options:
- **HighlightColor**: When using advanced screenshot, the identified element will be highlighred with Green color.
- **HighlightThickness**: The thickness of the highlight, 5 pixels.
- **ScoringMode**: Weighted or NonWeighted.
- **UseAdvancedScreenshot**: When active, the screenshot will be fullscreen with identified elements highlighted. Otherwise, the screenshot will be taken only on the element itself. it's easier to locate elements with Advanced screenshot, but it will be slower and uses more disk space.
- **Tags**: the tags identifies the rules to use. In above example, we are scanning agating WCAG AA rules only.

To know more about the options, please refer to <xref:AxaFrance.AxeExtended.HtmlReport.PageReportOptions>.

### Run accessibility test on a user journey
To run accessibility tests on a user journey, you will need to use `OverallReportBuilder` instead of `PageReportBuilder`.
Here is an example:
```csharp
using var driver = BrowserFactory.GetDriver(AxaFrance.WebEngine.Platform.Windows, BrowserType.ChromiumEdge))
var defaultOptions = new PageReportOptions()
{
    Title = "AXA.FR",
    OutputFormat = OutputFormat.Zip
};
var builder = new OverallReportBuilder().WithDefaultOptions(defaultOptions);

//Analyze first page
driver.Navigate().GoToUrl("https://www.axa.fr/");
builder.WithSelenium(driver, "Main Page").Build();

//Analyze second page
driver.Navigate().GoToUrl("https://www.axa.fr/pro.html");
builder.WithSelenium(driver, "Pro").Build();

//Analyze third page
driver.Navigate().GoToUrl("https://www.axa.fr/pro/services-assistance.html");
builder.WithSelenium(driver, "Assistance").Build();

string report = builder.Build().Export();
```
Observing the above code snippet, you can see that we are using `OverallReportBuilder` to generate the report using a default options which will be applied to all pages. Then, we are navigating to different pages and calling the `WithSelenium` method to pass the driver, on each individual page.

Finally, we are calling the `Build` method to analyze and generate report and `Export` method to get the path to the generated report. As provided in the options, the report will be generated in a zip format.