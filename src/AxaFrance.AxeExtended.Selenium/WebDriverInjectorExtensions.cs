using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AxaFrance.AxeExtended.Selenium
{

    /// <summary>
    /// This classe if forked from Deque Axe Core project to add capability to run customized rules like other language.
    /// per https://github.com/dequelabs/axe-core-nuget/issues/146
    /// </summary>
    internal static class WebDriverInjectorExtensions
    {
        /// <summary>
        ///  Yeilds contexts such that every yield will be in a different frame context.
        ///  To be used to take actions in every frame on a page.
        /// </summary>
        /// <param name="driver">An initialized WebDriver</param>
        internal static void ForEachFrameContext(this IWebDriver driver, Action callback)
        {
            IList<IWebElement> parents = new List<IWebElement>();
            ForEachFrameContext(driver, parents, callback);

            driver.SwitchTo().DefaultContent();
        }
        private static void ForEachFrameContext(this IWebDriver driver, IList<IWebElement> parents, Action callback)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            IList<IWebElement> frames = driver.FindElements(By.TagName("iframe"));

            foreach (var frame in frames)
            {
                driver.SwitchTo().DefaultContent();

                if (parents != null)
                {
                    foreach (IWebElement parent in parents)
                    {
                        driver.SwitchTo().Frame(parent);
                    }
                }

                driver.SwitchTo().Frame(frame);
                callback();

                IList<IWebElement> localParents = parents.ToList();
                localParents.Add(frame);
                ForEachFrameContext(driver, localParents, callback);
            }
        }

        internal static object ExecuteScript(this IWebDriver driver, string script, params object[] args)
        {
            IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)driver;
            return jsExecutor.ExecuteScript(script, args);
        }

        internal static object ExecuteAsyncScript(this IWebDriver driver, string script, params object[] args)
        {
            IJavaScriptExecutor jsExecutor = (IJavaScriptExecutor)driver;
            return jsExecutor.ExecuteAsyncScript(script, args);
        }
    }
}
