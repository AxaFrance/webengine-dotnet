// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-8-2 14:33
using AxaFrance.AxeExtended.HtmlReport;
using AxaFrance.AxeExtended.Selenium;
using AxaFrance.WebEngine;
using AxaFrance.WebEngine.Report;
using AxaFrance.WebEngine.Web;
using NUnit.Framework;
using OpenQA.Selenium;

namespace Samples.Gherkin.StepDefinitions
{
    [Binding]
    public class DrinkMachineStepDefinitions
    {

        // local variable to pass driver across scneario
        WebDriver driver;
        OverallReportBuilder reportBuilder;
        private readonly ScenarioContext _scenarioContext;

        public DrinkMachineStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        // clean up the driver after each scneario
        [AfterScenario]
        public void Cleanup()
        {
            //export accessibility report
            var report = reportBuilder?.Build().Export();
            TestCaseReport tcr = (TestCaseReport)_scenarioContext["report"];
            tcr.Result = _scenarioContext.TestError == null ? Result.Passed : Result.Failed;
            tcr.AttachFile(report, "AccessibilityReport");
            driver?.Close();
            driver?.Dispose();

            //save test report with embedded accessibility test result.
            TestSuiteReport tsr = new TestSuiteReport();
            tsr.TestResult.Add(tcr);
            var filename = tsr.SaveAs(Path.GetRandomFileName(), "test-report", false);
            Console.WriteLine($"Report saved to {filename}");
        }

        /// <summary>
        /// After each step, generates 
        /// </summary>
        [AfterStep]
        public void AfterStep()
        {
            var keyword = _scenarioContext.StepContext.StepInfo.StepInstance.Keyword;
            TestCaseReport tcr = (TestCaseReport)_scenarioContext["report"];
            tcr.ActionReports.Add(new ActionReport()
            {
                Name = keyword,
                StartTime = DateTime.Now,
                Result = _scenarioContext.TestError == null ? Result.Passed : Result.Failed
            });
        }

        [BeforeScenario]
        public void Setup()
        {
            driver = BrowserFactory.GetDriver(AxaFrance.WebEngine.Platform.Windows, AxaFrance.WebEngine.BrowserType.Chrome);
            reportBuilder = new OverallReportBuilder(new PageReportOptions()
            {
                Title = "Drink Machine Accessibility",
                OutputFormat = OutputFormat.Zip,
            });

            TestCaseReport tcr = new TestCaseReport()
            {
                TestName = _scenarioContext.ScenarioInfo.Title,
            };
            _scenarioContext["report"] = tcr;
        }

        [Given(@"I turn on the drink machine")]
        public void GivenITurnOnTheDrinkMachine()
        {
            driver.Navigate().GoToUrl("https://webengine-test.azurewebsites.net/Step1.html");
        }

        // Select language and drink, this function can be reused for other language and drinks.
        [When(@"I select '([^']*)' as language and order a '([^']*)'")]
        public void WhenISelectAsLanguageAndOrderA(string language, string drink)
        {
            MyPageModel pageModel = new MyPageModel(driver);
            pageModel.SelectLanguage.SelectByText(language);
            pageModel.RadioChooseToBuy.CheckByValue(drink);
            pageModel.NextButton.Click();
            reportBuilder.WithSelenium(driver, "Language").Build();
        }

        [Then(@"I got '([^']*)'")]
        public void ThenIGot(string tea)
        {
            MyPageModel pageModel = new MyPageModel(driver);
            reportBuilder.WithSelenium(driver, "Result").Build();
            Assert.That(pageModel.Page2Title.Exists());
        }
    }
}
