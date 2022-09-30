// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using AxaFrance.WebEngine.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;

namespace WebEngine.Test.UnitTests
{

    [TestClass]
    public class WebMobileTest
    {

        static WebDriver driver;

        [ClassCleanup]
        public static void Cleanup()
        {
            driver.Close();
            driver.Dispose();
        }

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            driver = BrowserFactory.GetDriver(AxaFrance.WebEngine.Platform.Android, AxaFrance.WebEngine.BrowserType.Chrome);
            driver.Navigate().GoToUrl("https://webengine-test.azurewebsites.net/");
        }

        [TestMethod]
        public void ElementByClassName()
        {
            WebElementDescription desc = new WebElementDescription(driver)
            {
                ClassName = "class1 class2"
            };
            var element = desc.GetText();
            Assert.IsTrue(element.Contains("first line"));
            Assert.IsTrue(element.Contains("second line"));
        }

        [TestMethod]
        public void ElementByLongClassName()
        {
            WebElementDescription desc = new WebElementDescription(driver)
            {
                TagName = "div",
                ClassName = "class1 class1-1 class2 very-long-class-name the quick brown fox jumps over a lazy dog",
            };
            var element = desc.GetText();
            Assert.AreEqual("TRUE", element);
        }

        [TestMethod]
        public void ElementById_AlertHandling()
        {
            WebElementDescription button = new WebElementDescription(driver)
            {
                Id = "btnButtonOk"
            };
            button.Click();
            string alertText = driver.SwitchTo().Alert().Text;
            Assert.AreEqual("hello world!", alertText);
            driver.SwitchTo().Alert().Accept();
            driver.Sync();
            Assert.IsTrue(button.GetAttribute("value") == "OK");
            var innerhtml = button.GetInnerHtml();
            Assert.IsTrue(string.IsNullOrEmpty(innerhtml));
            var outerhtml = button.GetOuterHtml();
            Assert.IsTrue(outerhtml.StartsWith("<input type=\"button\" value=\"OK\" id=\"btnButtonOk\""));
        }

        [TestMethod]
        public void DragAndDrop()
        {
            WebElementDescription drop1 = new WebElementDescription(driver)
            {
                Id = "divDrop1"
            };

            WebElementDescription drop2 = new WebElementDescription(driver)
            {
                Id = "divDrop2"
            };

            WebElementDescription source = new WebElementDescription(driver)
            {
                Id = "draggable_element",
            };
            string init = source.GetText();
            source.DragAndDropTo(drop1);
            string text1 = drop1.GetText();
            Assert.IsTrue(text1.Contains(init));
            source.DragAndDropTo(drop2);
            string text2 = drop2.GetText();
            Assert.IsTrue(text2.Contains(init));
        }

        [TestMethod]
        public void Confirm()
        {
            WebElementDescription desc = new WebElementDescription(driver)
            {
                TagName = "input",
                Attributes = new HtmlAttribute[]
                {
                    new HtmlAttribute("value", "Confirm")
                },
            };
            desc.Click();
            driver.SwitchTo().Alert().Dismiss();
        }

        [TestMethod]
        public void Input()
        {
            WebElementDescription desc = new WebElementDescription(driver)
            {
                TagName = "input",
                Attributes = new HtmlAttribute[]
                {
                    new HtmlAttribute("value", "Input")
                },
            };
            desc.Click();
            driver.SwitchTo().Alert().SendKeys("helloworld");
            driver.SwitchTo().Alert().Accept();
            WebElementDescription result = new WebElementDescription(driver)
            {
                Id = "inputValue",
            };
            var text = result.Value;
            Assert.AreEqual("helloworld", text);
        }

        [TestMethod]
        public void Hover()
        {
            WebElementDescription hover = new WebElementDescription(driver)
            {
                Id = "hover_item"
            };

            WebElementDescription display = new WebElementDescription(driver)
            {
                Id = "hover_display",
            };

            Assert.AreEqual(false, display.IsDisplayed);
            hover.MouseHover();
            Assert.AreEqual(true, display.IsDisplayed);
        }

        [TestMethod]
        public void ScrollIntoView()
        {
            WebElementDescription hover = new WebElementDescription(driver)
            {
                Attributes = new HtmlAttribute[]
                {
                    new HtmlAttribute("custom_name", "scroll_intoview"),
                }
            };
            Assert.IsTrue(hover.Exists());
            Assert.IsTrue(hover.IsDisplayed);
            Assert.IsFalse(hover.IsVisibleInViewPort);
            hover.ScrollIntoView();
            Assert.IsTrue(hover.IsVisibleInViewPort);
        }


    }
}
