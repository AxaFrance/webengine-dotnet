using AxaFrance.WebEngine;
using AxaFrance.WebEngine.Report;
using AxaFrance.WebEngine.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Samples.LinearScripting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebEngine.Test.UnitTests
{
    [TestClass]
    public class ResoureUsageTest
    {
        [TestMethod]
        public async void RunTestWithResourceMonitoring()
        {
            using var driver = BrowserFactory.GetDriver(AxaFrance.WebEngine.Platform.Windows, AxaFrance.WebEngine.BrowserType.Chrome);
            // Démarrer le monitoring de l'usage des ressources
            using var usageReport = BrowserFactory.StartMonitoring(driver);

            driver.Navigate().GoToUrl("https://axafrance.github.io/webengine-dotnet/demo/Step1.html");
            DrinkSelectionPageModel page = new DrinkSelectionPageModel(driver);

            page.SelectLanguage.SelectByValue("fr");
            page.RadioChooseToBuy.CheckByValue("Coffee");
            page.NextButton.Click();            
            await Task.Delay(2000);

            TestSuiteReport tsr = new TestSuiteReport();
            TestCaseReport tcr = new TestCaseReport()
            {
                TestName = "UnitTest",
                Result = AxaFrance.WebEngine.Result.Passed,
            };
            tcr.Attach(usageReport, GlobalConstants.ResourceUsageReport);
            tsr.TestResult.Add(tcr);
            var report = tsr.SaveAs(Path.GetTempPath(), "unit-test", true, out string reportFolder);
        }
    }
}
