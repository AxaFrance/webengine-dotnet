using AxaFrance.WebEngine;
using AxaFrance.WebEngine.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebEngine.Test.UnitTests
{
    [TestClass]
    public class FramesShadowDoms
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
                driver = BrowserFactory.GetDriver(AxaFrance.WebEngine.Platform.Windows, BrowserType.ChromiumEdge);
            }

            driver.Navigate().GoToUrl("https://axafrance.github.io/webengine-dotnet/demo/shadowdom.html");
        }

        [TestMethod]
        public void ShadowDom()
        {
            WebElementDescription wed = new WebElementDescription(driver)
            {
                CssSelector = ".shadow-box",
                ShadowRoot = new WebElementDescription()
                {
                    Id = "host"
                }
            };
            var element = wed.FindElement();
            var text = element.GetDomProperty("innerText");
            Assert.AreEqual("Hello, Shadow DOM!", text);
        }

        [TestMethod]
        public void MultipleShadowDom()
        {
            WebElementDescription wed = new WebElementDescription(driver)
            {
                CssSelector = ".shadow-box",
                ShadowRoot = new WebElementDescription()
                {
                    Id = "host2"
                }
            };
            var element = wed.FindElement();
            var text = element.GetDomProperty("innerText");
            Assert.AreEqual("Hello, Shadow DOM in the second div!", text);
        }

        [TestMethod]
        public void NestedShadowDom()
        {
            WebElementDescription wed = new WebElementDescription(driver)
            {
                CssSelector = ".shadow-box",
                ShadowRoot = new WebElementDescription()
                {
                    Id = "host3",
                    ShadowRoot = new WebElementDescription()
                    {
                        Id = "host2",
                    }
                }
            };
            var element = wed.FindElement();
            var text = element.GetDomProperty("innerText");
            Assert.AreEqual("Hello, Shadow DOM in a Shadow DOM!", text);
        }

        [TestMethod]
        public void ShadowDomInFrame()
        {
            var Frame = new WebElementDescription(driver)
            {
                Id = "Frame1"
            };

            WebElementDescription wed = new WebElementDescription(driver)
            {
                CssSelector = ".shadow-box",
                ShadowRoot = new WebElementDescription()
                {
                    Id = "host"
                }
            };

            //goto Frame1
            driver.SwitchTo().Frame(Frame.FindElement());
            var element = wed.FindElement();
            var text = element.GetDomProperty("innerText");
            driver.SwitchTo().DefaultContent();
            Assert.AreEqual("Hello, Shadow DOM in Frame!", text);

            
        }
    }
}
