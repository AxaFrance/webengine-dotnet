// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using OpenQA.Selenium;

namespace AxaFrance.WebEngine.Web
{
    /// <summary>
    /// Page Model is the repository to store all test objets which are been used by the test script. 
    /// Page Model contains one or more <see cref="ElementDescription"/>, each Element Description indicates how to identify the 
    /// </summary>
    public class PageModel
    {
        /// <summary>
        /// Initialize the Page model with the associated Selenium WebDriver
        /// </summary>
        /// <param name="driver">The Webdriver to use for the localization of Web Elements </param>
        public PageModel(WebDriver driver)
        {
            var fields = this.GetType().GetFields();
            foreach (var p in fields)
            {
                if (p.FieldType.IsSubclassOf(typeof(ElementDescription)))
                {
                    ElementDescription e = (ElementDescription)p.GetValue(this);
                    e.UseDriver(driver);
                }
            }

            var properties = this.GetType().GetProperties();
            foreach (var p in properties)
            {
                if (p.PropertyType.IsSubclassOf(typeof(ElementDescription)))
                {
                    ElementDescription e = (ElementDescription)p.GetValue(this);
                    e.UseDriver(driver);
                }
            }
        }
    }
}
