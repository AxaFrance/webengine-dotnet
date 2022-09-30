// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-8-2 14:45
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AXA.WebEngine.Web;
using OpenQA.Selenium;

namespace Samples.Gherkin
{
    public class MyPageModel : PageModel
    {
        public MyPageModel(WebDriver driver) : base(driver)
        {
        }

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
    }
}
