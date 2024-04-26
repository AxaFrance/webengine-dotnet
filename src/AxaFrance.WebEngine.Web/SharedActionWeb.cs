// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using Axe.Extended.HtmlReport;
using Axe.Extended.Selenium;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using System;
using System.Drawing;
using System.IO;

namespace AxaFrance.WebEngine.Web
{
    /// <summary>
    /// <see cref="SharedActionWeb"/> implements privately the generic <see cref="SharedActionBase"/> interface and exposes new abstract methods
    /// The intention of add this level of abstraction is to make the WebEngine framework commun for other and no dependence.
    /// <para>Base class of all shared actions using Selenium. All shared actions must be derived from this class and implement DoAction, DoCheckpoint method.</para>
    /// <para>In order to verify if all parameters are given when calling DoAction() and DoCheckpoint() methods, please specify RequiredParameters Property.</para>
    /// </summary>
    public abstract class SharedActionWeb : SharedActionBase
    {
        /// <summary>
        /// The shared <see cref="IWebDriver"/> object for the current Selenium test Action
        /// </summary>
        protected WebDriver Browser { get; set; }


        /// <summary>
        /// Runs an accessibility check on current location.
        /// </summary>
        /// <param name="pageName">Name of the current page on report</param>
        public void RunAccessibilityTest(string pageName)
        {
            if(testCase is TestCaseWeb tw && tw.IsAccessibilityTestEnabled && tw.reportBuilder != null)
            {
                DebugLogger.WriteLine("[A11Y] Analyzing accessibility issues on current page.");
                tw.reportBuilder.WithSelenium(Browser, pageName).WithRgaaExtension().Build();                
            }
        }

        /// <summary>
        /// Run the current test action.
        /// </summary>
        /// <param name="Context">An WebDriver object</param>
        public override void DoAction(object Context)
        {
            try
            {
                if (Context is WebDriver driver)
                {

                    Browser = driver;
                    DoAction();
                    if (this.ActionResult == WebEngine.Result.Failed || this.ActionResult == Result.CriticalError)
                    {
                        this.Screenshot($"Error in {this.GetType().Name} DoAction");
                    }

                }
                else
                {
                    throw new WebEngineGeneralException("Given context is null or not an Selenium WebDriver object");
                }
            }
            catch (OpenQA.Selenium.NoSuchElementException notfound)
            {
                this.ActionResult = Result.CriticalError;
                string information = "[ERROR] A Web element is not present: " + notfound.ToString();
                DebugLogger.WriteError(information);
                this.Information.AppendLine(information);
            }
        }

        /// <summary>
        /// Run the checkpoint after running DoAction() method to check if the value is 
        /// </summary>
        /// <param name="Context">The current testing context. For Web testing, it is the Browser object.</param>
        /// <returns>A <see cref="bool"/> value indicates if the checkpoint is passed.</returns>
        public override bool DoCheckpoint(object Context)
        {
            if (Context is WebDriver driver)
            {

                Browser = driver;
                var checkpoint = DoCheckpoint();
                if (!checkpoint)
                {
                    this.Screenshot($"Error in {this.GetType().Name} DoCheckpoint");
                }
                return checkpoint;
            }
            else
            {
                throw new WebEngineGeneralException("The given Context is not an LeanFT IBrowser object");
            }
        }

        /// <summary>
        /// Generates screenshot and add it in the test report.
        /// </summary>
        /// <param name="name">the name of the screenshot</param>
        protected override void Screenshot(string name)
        {

            if (Browser != null)
            {
                try
                {
                    var image = Browser.GetScreenshot();
                    this.Screenshots.Add(new Report.ScreenshotReport()
                    {
                        Name = name,
                        Data = image.AsByteArray
                    });
                }
                catch (Exception ex)
                {
                    DebugLogger.WriteError($"Screenshot Error: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Generates screenshot and add it in the test report.
        /// </summary>
        protected void Screenshot()
        {
            Screenshot(string.Empty);
        }

        /// <summary>
        /// Generates screenshot on the given element and add it to the test report.
        /// </summary>
        /// <param name="element">The element which screenshot should be generated.</param>
        protected void Screenshot(ElementDescription element)
        {
            var content = element.GetScreenshot();
            this.Screenshots.Add(new AxaFrance.WebEngine.Report.ScreenshotReport()
            {
                Name = "Error Message",
                Data = content
            });
        }

        /// <summary>
        /// Implement your test script in this method to interact with your application (Click the links, Fill forms, Click buttons, ...)
        /// Of course, you can do any actions needed.
        /// </summary>
        public abstract void DoAction();

        /// <summary>
        /// This method is called after DoAction in order to check whether the action is correctly running.
        /// </summary>
        /// <returns>true if there is not blocking errors, otherwise: false</returns>
        /// <remarks>if the check cannot be run correctly, an exception will be thrown</remarks>
        public abstract bool DoCheckpoint();

        /// <summary>
        /// Constructor with giving runtime parameters (Test Data)
        /// </summary>
        /// <param name="_parameters"></param>
        public SharedActionWeb(Variable[] _parameters)
            : base(_parameters)
        {

        }

        /// <summary>
        /// Gets an information whether the test is currently running on a mobile device. 
        /// </summary>
        protected bool IsMobile
        {
            get
            {
                if (Browser is AppiumDriver ad)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Constructor with no runtime parameters given.
        /// Warning: Parameter Property of the Action must be assigned before running the test.
        /// </summary>
        public SharedActionWeb()
            : base(new Variable[0])
        {

        }
        
    }
}
