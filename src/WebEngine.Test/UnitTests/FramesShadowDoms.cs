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
                driver = BrowserFactory.GetDriver(AxaFrance.WebEngine.Platform.Windows, BrowserType.Chrome);
            }

            driver.Navigate().GoToUrl("https://axafrance.github.io/webengine-dotnet/demo/Test.html");
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
    }
}
