using AxaFrance.WebEngine;
using AxaFrance.WebEngine.Report;
using AxaFrance.WebEngine.Web;
using OpenQA.Selenium;


namespace Samples.LinearScripting
{
    [TestClass]
    public class UnitTest1
    {
        //WebDriver object will be used for each test case.
        WebDriver? driver = null;
        TestCaseReport testCaseReport;
        static TestSuiteReport? testSuiteReport = null;

        public TestContext? TestContext { get; set; }

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            testSuiteReport = new TestSuiteReport();
        }

        [TestInitialize]
        public void TestInitialize()
        {
            //Initialize the test case report and add it to the test suite report.
            testCaseReport = new TestCaseReport
            {
                TestName = TestContext!.TestName,
                StartTime = DateTime.Now
            };
            testSuiteReport!.TestResult.Add(testCaseReport);

            //Initialize the driver by platform and browser type
            driver = BrowserFactory.GetDriver(AxaFrance.WebEngine.Platform.Windows, BrowserType.Chrome);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            //After each test case, we will save the test case report.
            testCaseReport.Result = TestContext!.CurrentTestOutcome switch
            {
                UnitTestOutcome.Passed => Result.Passed,
                UnitTestOutcome.Failed => Result.Failed,
                UnitTestOutcome.Inconclusive => Result.Ignored,
                UnitTestOutcome.Error => Result.CriticalError,
                _ => Result.None
            };

            testCaseReport.EndTime = DateTime.Now;
            //Close the driver (and browser) after each test case.
            driver?.Close();
            driver?.Dispose();
        }

        [ClassCleanup]
        public static void ClassTearDown()
        {
            //After all test cases, we will save the test suite report.
            testSuiteReport!.EndTime = DateTime.Now;
            var reportPath = testSuiteReport.SaveAs("TestReport", "TEST-MyTestReport", false);

            //you can check the test report in folder bin\Debug\net8.0\TestReport of your test project.
            DebugLogger.WriteLine($"Test Suite Report saved at: {reportPath}");
        }

        /// <summary>
        /// Example test method to select a drink and check the next page title.
        /// </summary>
        [TestMethod]
        public void SelectDrinkTest()
        {
            driver!.Navigate().GoToUrl("https://axafrance.github.io/webengine-dotnet/demo/Step1.html");


            //during the test execution, we can add some action reports to the test case report.
            var step1Report = new ActionReport
            {
                Name = "Navigate to Step 1 page",
                Result = Result.Passed,
                Log = "Navigated to Step 1 page successfully."
            };

            testCaseReport.ActionReports.Add(step1Report);

            //Create a page model for the Drink Selection page.
            DrinkSelectionPageModel page = new DrinkSelectionPageModel(driver);
            page.SelectLanguage.SelectByValue("fr");
            page.RadioChooseToBuy.CheckByValue("Coffee");
            var screenshot = driver!.GetScreenshot();
            page.NextButton.Click();

            var actionReport = new ActionReport
            {
                Name = "Select language and drink",
                Result = Result.Passed,
                Log = "Selected French language and Coffee drink.",
            };
            actionReport.Screenshots.Add(new ScreenshotReport
            {
                Name = "DrinkSelectionPageScreenshot",
                Data = screenshot.AsByteArray,
            });
            step1Report.SubActionReports.Add(actionReport);



            testCaseReport.ActionReports.Add(new ActionReport
            {
                Name = "Select language and drink",
                Result = Result.Passed,
                Log = "Selected French language and Coffee drink."
            });

            testCaseReport.Log = "This is the log of curren test case.";

            //Check if the next page is loaded correctly.
            Assert.IsTrue(page.Page2Title.Exists());
        }
    }
}