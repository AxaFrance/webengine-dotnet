﻿// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26

using OpenQA.Selenium;
using System;
using System.Reflection;

namespace AxaFrance.WebEngine.Web
{
    /// <summary>
    /// Page Model is the repository to store all test objets which are been used by the test script. 
    /// Page Model contains one or more <see cref="ElementDescription"/>, each Element Description indicates how to identify the 
    /// </summary>
    public class PageModel
    {

        private IWebDriver driver;
        /// <summary>
        /// Initialize the Page model with the associated Selenium WebDriver
        /// </summary>
        /// <param name="driver">The Webdriver to use for the localization of Web Elements </param>
        public PageModel(WebDriver driver)
        {
            this.driver = driver;
            var fields = this.GetType().GetFields();
            foreach (var p in fields)
            {
                if (p.FieldType.IsSubclassOf(typeof(ElementDescription)))
                {
                    ElementDescription e = (ElementDescription)p.GetValue(this);
                    if (e == null)
                    {
                        e = (ElementDescription)Activator.CreateInstance(p.FieldType);
                        p.SetValue(this, e);
                    }
                    CheckFindsByAttribute(p, e);
                    e.UseDriver(driver);
                }
            }

            var properties = this.GetType().GetProperties();
            foreach (var p in properties)
            {
                if (p.PropertyType.IsSubclassOf(typeof(ElementDescription)))
                {
                    ElementDescription e = (ElementDescription)p.GetValue(this);
                    if (e == null)
                    {
                        e = (ElementDescription)Activator.CreateInstance(p.PropertyType);
                        p.SetValue(this, e);
                    }
                    CheckFindsByAttribute(p, e);
                    e.UseDriver(driver);
                }
            }
        }

        /// <summary>
        /// Applies the element locating logic via FindsByAttribute
        /// </summary>
        /// <param name="memberInfo">PropertyInfo which the property may be tagged with <see cref="FindsByAttribute"/></param>
        /// <param name="e">ElementDescription which the property may be tagged with <see cref="FindsByAttribute"/></param>
        private void CheckFindsByAttribute(MemberInfo memberInfo, ElementDescription e)
        {
            var attributes = memberInfo.GetCustomAttributes<FindsByAttribute>();
            if (attributes != null)
            {
                foreach (var attr in attributes)
                {
                    e.ApplyAttribute(attr);
                }
            }
        }


        /// <summary>
        /// Run accessibility test on the current page
        /// </summary>
        public void AccessibilityCheck()
        {
            Deque.AxeCore.Selenium.AxeBuilder builder = new Deque.AxeCore.Selenium.AxeBuilder(driver).WithRules();
            var results = builder.Analyze();
        }

        /// <summary>
        /// Run accessibility test on the given element
        /// </summary>
        /// <param name="context"></param>
        public void AccessibilityCheck(ElementDescription context)
        {
            Deque.AxeCore.Selenium.AxeBuilder builder = new Deque.AxeCore.Selenium.AxeBuilder(driver);
            AccessibilityCheck(context.FindElement());
        }


        /// <summary>
        /// Run accessibiity test on the given element
        /// </summary>
        /// <param name="context"></param>
        public void AccessibilityCheck(IWebElement context)
        {
            Deque.AxeCore.Selenium.AxeBuilder builder = new Deque.AxeCore.Selenium.AxeBuilder(driver);
            var results = builder.Analyze(context);
        }
    }
}
