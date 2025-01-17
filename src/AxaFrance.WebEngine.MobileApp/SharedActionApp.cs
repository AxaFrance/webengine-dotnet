// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using System;

namespace AxaFrance.WebEngine.MobileApp
{
    /// <summary>
    /// SharedAction as keyword to be implemented by test automation Engineer.
    /// </summary>
    public abstract class SharedActionApp : SharedActionBase
    {

        AppiumDriver appiumDriver = null;

        /// <inheritdoc/>
        protected override void Screenshot(string name)
        {
            if (appiumDriver != null)
            {
                try
                {
                    var image = appiumDriver.GetScreenshot();
                    this.Screenshots.Add(new Report.ScreenshotReport()
                    {
                        Name = name,
                        Data = image.AsByteArray
                    });
                }
                catch
                {
                    //if screenshot can not be stored, it doesn't matter.
                }
            }
            else
            {
                DebugLogger.WriteError("Cannot make screenshot because the AppiumDriver is null");
            }
        }

        /// <inheritdoc />
        protected void Screenshot()
        {
            Screenshot(string.Empty);
        }

        /// <summary>
        /// Run the implemented test script for the current test action (Keyword) using the Appium driver provited as context.
        /// </summary>
        /// <param name="Context">An AppiumDriver object, which refereces to an connection to the Mobile Application. </param>
        public override void DoAction(object Context)
        {
            try
            {
                if (Context is AppiumDriver ad)
                {
                    appiumDriver = ad;
                    DoAction(appiumDriver);
                    if (this.ActionResult == WebEngine.Result.Failed || this.ActionResult == Result.CriticalError)
                    {
                        this.Screenshot($"Error in {this.GetType().Name} DoAction");
                    }
                }
                else
                {
                    throw new ElementNotSelectableException("The context is not a valid AppiumDriver");
                }
            }
            catch (Exception ex)
            {
                this.ActionResult = Result.CriticalError;
                string information = "[ERROR] A error occured when running this action: " + ex.ToString();
                DebugLogger.WriteError(information);
                this.Information.AppendLine(information);
            }
        }

        /// <summary>
        /// Checkpoint is executed after the test script DoAction to verify if there are blocking errors.
        /// </summary>
        /// <param name="Context">The current testing context, AppiumDriver. </param>
        /// <returns>A <see cref="bool"/> value indicates if there are blocking errors..</returns>
        public override bool DoCheckpoint(object Context)
        {
            if (Context is AppiumDriver appiumDriver)
            {
                var checkpoint = DoCheckpoint(appiumDriver);
                if (!checkpoint)
                {
                    this.Screenshot($"Error in {this.GetType().Name} DoCheckpoint");
                }
                return checkpoint;
            }
            else
            {
                throw new ElementNotSelectableException("The context is not a valid AppiumDriver");
            }
        }

        /// <summary>
        /// Implement DoAction() method to interact with your application (Click the links, Fill forms, Click buttons, ...)
        /// This action will be called if you are working on Android or IOS.
        /// Please note that if you test targets both Android and IOS, you should implement
        /// </summary>
        public abstract void DoAction(AppiumDriver driver);


        /// <summary>
        /// This method should be called after calling DoAction() Method in order to check whether the action is correctly running.
        /// </summary>
        /// <returns>true if the check passed or false if the check failed</returns>
        /// <remarks>if the check cannot be run correctly, an exception will be thrown</remarks>
        public abstract bool DoCheckpoint(AppiumDriver driver);

        /// <summary>
        /// Constructor with giving runtime parameters (Test Data)
        /// </summary>
        /// <param name="_parameters"></param>
        protected SharedActionApp(Variable[] _parameters)
            : base(_parameters)
        {

        }

        /// <summary>
        /// Constructor with no runtime parameters given.
        /// Warning: Parameter Property of the Action must be assigned before running the test.
        /// </summary>
        protected SharedActionApp()
            : base(new Variable[0])
        {

        }

        /// <summary>
        /// The NativeAppContext const = "NATIVE_APP".
        /// </summary>
        protected const string NativeAppContext = "NATIVE_APP";

        /// <summary>
        /// For Hybird application, use this method to switch to one of Webview or switch back to NATIVE_APP.
        /// </summary>
        /// <param name="targetContext">the prefix of the target context to switch.</param>
        public void SwitchContext(string targetContext)
        {
            AppFactory.SwitchContext(appiumDriver, targetContext);
        }

    }
}
