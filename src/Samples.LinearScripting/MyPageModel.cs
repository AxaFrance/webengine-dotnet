// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-8-1 15:59

using AxaFrance.WebEngine.Web;
using OpenQA.Selenium;

namespace Samples.LinearScripting
{
    public class DrinkSelectionPageModel : PageModel
    {

        //Choose Language
        public WebElementDescription SelectLanguage = new WebElementDescription
        {
            TagName = "select",
            Id = "language"
        };

        public WebElementDescription RadioChooseToBuy = new WebElementDescription
        {
            Name = "fav_language"
        };

        //Choose to buy
        public WebElementDescription NextButton = new WebElementDescription
        {
            TagName = "button",
            Attributes = new HtmlAttribute[]
            {
                new HtmlAttribute{ Name = "onclick", Value = "testSleep()"}
            }
        };

        //Page title of next page (for verification)
        public WebElementDescription Page2Title = new WebElementDescription
        {
            TagName = "h1",
            InnerText = "Test page - Step 2"
        };

        public DrinkSelectionPageModel(WebDriver driver) : base(driver)
        {
        }

    }
}
