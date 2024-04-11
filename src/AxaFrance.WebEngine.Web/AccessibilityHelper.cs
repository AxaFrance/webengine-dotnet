using Deque.AxeCore.Commons;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace AxaFrance.WebEngine.Web
{
    /// <summary>
    /// A helper class to provide accessibility verification support
    /// </summary>
    public static class AccessibilityHelper
    {
        public static AxeResult Analyze(this IWebDriver driver)
        {
            Deque.AxeCore.Selenium.AxeBuilder builder = new Deque.AxeCore.Selenium.AxeBuilder(driver);
            var result = builder.Analyze();
            return result;
        }

    }
}
