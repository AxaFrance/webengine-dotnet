// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using AxaFrance.WebEngine;
using AxaFrance.WebEngine.MobileApp;
using AxaFrance.WebEngine.Web;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.MultiTouch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebEngine.Test.UnitTests
{
    public class MobileAppStep2 : AxaFrance.WebEngine.MobileApp.SharedActionApp
    {
        public override Variable[] RequiredParameters => null;

        public override void DoAction(AppiumDriver driver)
        {
            SecondPageModel model = new SecondPageModel(driver);
            var displayed = model.ButtonWebView.Exists(1);
            if (model.ButtonWebView.ScrollIntoView())
            {
                model.ButtonWebView.Click();
                SwitchContext("WEBVIEW");
                model.FormInput.SetValue("helloworld");
                SwitchContext(NativeAppContext);
                driver.Navigate().Back();
                ActionResult = Result.Failed;
            }
            else
            {
                throw new Exception("Test Failed");
            }
        }

        public override bool DoCheckpoint(AppiumDriver driver)
        {
            return true;
        }
    }

    public class SecondPageModel : PageModel
    {
        public AppElementDescription ButtonWebView = new AppElementDescription
        {
            AccessbilityId = "WebView"
        };

        public WebElementDescription FormInput = new WebElementDescription
        {
            Name = "i_am_a_textbox"
        };

        public SecondPageModel(WebDriver driver) : base(driver)
        {
        }
    }
}
