// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-8-2 14:33
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

        // clean up the driver after each scneario
        [AfterScenario]
        public void Cleanup()
        {
            driver?.Close();
            driver?.Dispose();
        }

        [Given(@"I turn on the drink machine")]
        public void GivenITurnOnTheDrinkMachine()
        {
            driver = BrowserFactory.GetDriver(AxaFrance.WebEngine.Platform.Windows, AxaFrance.WebEngine.BrowserType.Chrome);
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
        }

        [Then(@"I got '([^']*)'")]
        public void ThenIGot(string tea)
        {
            MyPageModel pageModel = new MyPageModel(driver);
            Assert.That(pageModel.Page2Title.Exists());
        }
    }
}
