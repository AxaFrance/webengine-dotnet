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
            driver.Navigate().GoToUrl("https://webengine-test.azurewebsites.net/Step1.html");
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
            //initialize the page model with current driver
            MyPageModel page = new MyPageModel(driver);

            //choose the option by value="fr" in the select
            page.SelectLanguage.SelectByValue("fr");
            //choose the radiobutton where the value is "Coffee"
            page.RadioChooseToBuy.CheckByValue("Coffee");
            //click on the Next button
            page.NextButton.Click();

            //Verify if the current page title is page 2 
            Assert.IsTrue(page.Page2Title.Exists());
            //The above assertion will not fail because exists will wait until the second page has loaded within the timeout

        }
    }
}