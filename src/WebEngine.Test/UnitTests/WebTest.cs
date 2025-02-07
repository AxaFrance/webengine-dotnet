// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using AxaFrance.WebEngine;
using AxaFrance.WebEngine.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;

namespace WebEngine.Test.UnitTests
{

    [TestClass]
    public class WebTest
    {

        static WebDriver driver = null;

        [ClassCleanup]
        public static void Cleanup()
        {
            try
            {
                driver?.Quit();
            }
            catch { }
            try
            {
                driver?.Close();
            }
            catch { }
            try
            {
                driver?.Dispose();
            }
            catch { }
        }

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            if (driver == null)
            {
                driver = BrowserFactory.GetDriver(AxaFrance.WebEngine.Platform.Windows, BrowserType.Chrome);
            }

            driver.Navigate().GoToUrl("https://axafrance.github.io/webengine-dotnet/demo/Test.html");
        }


        [TestMethod]
        public void ElementTypeing()
        {
            WebElementDescription inputBox = new WebElementDescription(driver)
            {
                Id = "inputValue"
            };

            inputBox.SetValue("abc");
            var value1 = inputBox.GetProperty("value");
            inputBox.SetValue("def");
            var value2 = inputBox.GetProperty("value");
            inputBox.SendKeys("abc");
            var value3 = inputBox.GetProperty("value");
            Assert.AreEqual("abc", value1);
            Assert.AreEqual("def", value2);
            Assert.AreEqual("defabc", value3);
        }

        [TestMethod]
        public void ElementIsEnabled()
        {
            WebElementDescription inputBox = new WebElementDescription(driver)
            {
                Id = "inputValue"
            };
            var enabled = inputBox.IsEnabled;
            Assert.IsTrue(enabled);
        }


        [TestMethod]
        public void SecurePassword()
        {
            WebElementDescription passwordBox = new WebElementDescription(driver)
            {
                Id = "password"
            };

            WebElementDescription inputBox = new WebElementDescription(driver)
            {
                Id = "inputValue"
            };

            var password = Encrypter.Encrypt("password");
            passwordBox.SetSecure(password); // -> OK
            try
            {
                inputBox.SetSecure(password);    //-> Error with Not supported Exception
            }
            catch
            {
                return;
            }
            Assert.Fail("no exception if setsecure is used on normal text box");
        }

        [TestMethod]
        public void PageModelTest()
        {
            TestPageModel model = new TestPageModel(driver);
            var element = model.desc.GetText();
            var element2 = model.descWithAttribute.GetText();
            Assert.IsTrue(element.Contains("first line"));
            Assert.IsTrue(element2.Contains("first line"));
        }

        public class TestPageModel : PageModel
        {
            public TestPageModel(WebDriver driver) : base(driver)
            {
            }

            [FindsBy(How.ClassName, "class1 class2")]
            public WebElementDescription descWithAttribute { get; set; }

            public WebElementDescription desc { get; set; } = new WebElementDescription()
            {
                ClassName = "class1 class2"
            };
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
        public void ElementBySelector()
        {
            WebElementDescription desc = new WebElementDescription(driver)
            {
                CssSelector = "#divDrop2",
            };
            var element = desc.GetText();


            Assert.IsTrue(element.Contains("drag target 2"));
        }

        [TestMethod]
        public void ElementByXPath()
        {
            WebElementDescription desc = new WebElementDescription(driver)
            {
                XPath = "/html/body/label[3]",
            };
            var element = desc.GetText();
            Assert.AreEqual("CSS", element);
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
            Assert.IsTrue(button.GetProperty("value") == "OK");
            var innerhtml = button.GetInnerHtml();
            Assert.IsTrue(innerhtml == "");
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
        public void DropDown_SelectByText()
        {
            WebElementDescription desc = new WebElementDescription(driver)
            {
                TagName = "select",
                Name = "cars"
            };
            desc.SelectByText("Mercedes-Benz");
            OpenQA.Selenium.Support.UI.SelectElement se = new OpenQA.Selenium.Support.UI.SelectElement(desc.FindElement());
            string mb = se.SelectedOption.Text;
            Assert.AreEqual("Mercedes-Benz", mb);
        }

        [TestMethod]
        public void DropDown_SelectByValue()
        {
            WebElementDescription desc = new WebElementDescription(driver)
            {
                TagName = "select",
                Name = "cars"
            };
            desc.SelectByValue("mercedes");
            OpenQA.Selenium.Support.UI.SelectElement se = new OpenQA.Selenium.Support.UI.SelectElement(desc.FindElement());
            string mb = se.SelectedOption.Text;
            Assert.AreEqual("Mercedes-Benz", mb);
        }

        [TestMethod]
        public void DropDown_SelectByIndex()
        {
            WebElementDescription desc = new WebElementDescription(driver)
            {
                TagName = "select",
                Name = "cars"
            };
            desc.SelectByIndex(2);
            OpenQA.Selenium.Support.UI.SelectElement se = new OpenQA.Selenium.Support.UI.SelectElement(desc.FindElement());
            string mb = se.SelectedOption.Text;
            Assert.AreEqual("Mercedes-Benz", mb);
        }

        [TestMethod]
        public void RadioGroup()
        {
            var radioGroup = new WebElementDescription(driver)
            {
                Name = "fav_language"
            };
            var check = radioGroup.CheckByValue("CSS");
            var value = check.GetDomProperty("checked");
            var check2 = radioGroup.CheckByValue("HTML");
            var value2 = check2.GetDomProperty("checked");
            var value3 = check.GetDomProperty("checked");
            Assert.AreEqual(string.Compare("true", value, true), 0);
            Assert.AreEqual(string.Compare("true", value2, true), 0);
            Assert.AreNotEqual(string.Compare("true", value3, true), 0);

        }

        [TestMethod]
        public void Checkbox()
        {
            var check = new WebElementDescription(driver)
            {
                TagName = "input",
                Id = "scales"
            };
            Assert.IsTrue(check.IsSelected);

            var check2 = new WebElementDescription(driver)
            {
                TagName = "input",
                Name = "horns"
            };
            Assert.IsFalse(check2.IsSelected);
            check2.Click();
            Assert.IsTrue(check2.IsSelected);
            check.Click();
            Assert.IsFalse(check.IsSelected);
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
            driver.Navigate().GoToUrl("https://axafrance.github.io/webengine-dotnet/demo/Test.html");
            var customDiv = new WebElementDescription(driver)
            {
                Attributes = new HtmlAttribute[]
                {
                    new HtmlAttribute("custom_name", "scroll_intoview"),
                }
            };
            Assert.IsTrue(customDiv.Exists());
            Assert.IsTrue(customDiv.IsDisplayed);
            Assert.IsFalse(customDiv.IsVisibleInViewPort);
            customDiv.ScrollIntoView();
            Assert.IsTrue(customDiv.IsVisibleInViewPort);
        }

        [TestMethod]
        public void IFrame()
        {
            try
            {
                WebElementDescription frame = new WebElementDescription(driver)
                {
                    TagName = "iframe",
                };


                WebElementDescription title = new WebElementDescription(driver)
                {
                    TagName = "h1",
                };

                WebElementDescription iframe_input = new WebElementDescription(driver)
                {
                    Id = "iframe_button"
                };


                WebElementDescription input = new WebElementDescription(driver)
                {
                    Id = "inputValue"
                };

                var element = frame.FindElement();
                driver.SwitchTo().Frame(element);
                var text = title.GetText();
                iframe_input.ScrollIntoView();
                iframe_input.SetValue("hello 2");
                var screenshot = iframe_input.GetScreenshot();
                Assert.IsTrue(text.Contains("content inside the"));
                Assert.IsNotNull(screenshot);
                driver.SwitchTo().ParentFrame();

                input.SetValue("hello world");
            }
            catch
            {
                driver.Navigate().GoToUrl("https://axafrance.github.io/webengine-dotnet/demo/Test.html");
                throw;
            }
        }

    }
}
