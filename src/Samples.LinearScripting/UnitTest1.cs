using AxaFrance.WebEngine;
using AxaFrance.WebEngine.Web;


namespace Samples.LinearScripting
{
    [TestClass]
    public class UnitTest1
    {
        //WebDriver object will be used for each test case.
        OpenQA.Selenium.WebDriver? driver = null;

        [TestInitialize]
        public void Setup()
        {
            //Initialize the driver by platform and browser type
            driver = AxaFrance.WebEngine.Web.BrowserFactory.GetDriver(
                AxaFrance.WebEngine.Platform.Windows,
                AxaFrance.WebEngine.BrowserType.Chrome);
            driver.Navigate().GoToUrl("https://axafrance.github.io/webengine-dotnet/demo/Step1.html");
        }



        [TestCleanup]
        public void Teardown()
        {
            //Close the driver (and browser) after each test case.
            driver?.Close();
            driver?.Dispose();
        }


        [TestMethod]
        public void TestMethod1()
        {
            driver = BrowserFactory.GetDriver(Platform.Windows, BrowserType.Chrome);
            driver.Navigate().GoToUrl("https://axafrance.github.io/webengine-dotnet/demo/Step1.html");
            DrinkSelectionPageModel page = new DrinkSelectionPageModel(driver);

            page.SelectLanguage.SelectByValue("fr");
            page.RadioChooseToBuy.CheckByValue("Coffee");
            page.NextButton.Click();

            Assert.IsTrue(page.Page2Title.Exists());

        }
    }
}