// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using AxaFrance.WebEngine;
using AxaFrance.WebEngine.MobileApp;
using AxaFrance.WebEngine.Web;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebEngine.Test.UnitTests
{
    public class MobileAppStep1 : SharedActionApp
    {
        public override Variable[] RequiredParameters => null;

        public override void DoAction(AppiumDriver driver)
        {
            var addition = GetParameter("TESTCASE");
            if (addition != "Addition1") throw new Exception("test Failed: GetParameter");

            var env = EnvironmentVariables.Current.GetValue("URL_RECETTE");
            if (!env.Contains("fr")) throw new Exception("test Failed: EnvironmentVariables.Current.GetValue");

            MyModel m = new MyModel(driver);
            m.menuViews.Click();
            Screenshot("screenshot with name");
            m.menuButtons.Click();
            m.normalButton.Click();
            m.smallButton.Click();
            m.toggleButton.Click();
            Screenshot();
            if (m.toggleButton.GetText() == "ON")
            {
                ActionResult = Result.Passed;
            }
            else
            {
                ActionResult = Result.Failed;
                Information.AppendLine("ToggleButton is not ON");
            }
            AppFactory.ActionBack(driver); //go back to upper level

        }

        public override bool DoCheckpoint(AppiumDriver driver)
        {
            return true;
        }
    }

    public class MyModel : PageModel
    {
        public AppElementDescription menuViews = new AppElementDescription
        {
            AccessbilityId = "Views"
        };

        public AppElementDescription menuButtons = new AppElementDescription
        {
            XPath = "//android.widget.TextView[@content-desc=\"Buttons\"]"
        };

        public AppElementDescription normalButton = new AppElementDescription
        {
            Id = "io.appium.android.apis:id/button_normal"
        };

        public AppElementDescription smallButton = new AppElementDescription
        {
            ContentDescription = "Small"
        };

        public AppElementDescription toggleButton = new AppElementDescription
        {
            ClassName = "android.widget.ToggleButton",
            ContentDescription = "Toggle"
        };

        public MyModel(WebDriver driver) : base(driver)
        {
        }
    }
}
