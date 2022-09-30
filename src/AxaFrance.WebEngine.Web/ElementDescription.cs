// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Threading;

namespace AxaFrance.WebEngine.Web
{
    /// <summary>
    /// Describes how to identify an test object by its attributes, like Id, Name, XPath and any conbinations of them. WebEngine Framework can localize test object with multiple attributes.
    /// For web application testing, use the derived class <see cref="WebElementDescription"/>
    /// </summary>
    public abstract class ElementDescription
    {
        /// <summary>
        /// WebDriver used to identify test objects.
        /// </summary>
        protected WebDriver driver;

        /// <summary>
        /// Implemente element identifying algorithm, without the need of object synchronization.
        /// </summary>
        /// <returns>Identified test object.</returns>
        protected abstract IWebElement InternalFindElement();

        /// <summary>
        /// Sets the Selenium WebDriver to use for object indentification. If the WebElement is declared within a <see cref="PageModel"/> there is no need to apply the WebDriver to use.
        /// </summary>
        /// <param name="driver">WebDriver to be used to locate objects </param>
        public ElementDescription UseDriver(WebDriver driver)
        {
            this.driver = driver;
            return this;
        }

        /// <summary>
        /// Gets The IWebElement with current element description, using default timeout
        /// </summary>
        /// <returns>The Web element localized by the description.</returns>
        /// <remarks>
        /// Default timeout is 
        /// </remarks>
        public IWebElement FindElement()
        {
            return FindElement(Settings.Instance.SynchronzationTimeout);
        }

        /// <summary>
        /// Gets the Web Element using curent element description, with customized timeout.
        /// </summary>
        /// <param name="timeoutSecond">object syncchronization timeout in second.</param>
        /// <returns>The indentified web element.</returns>
        public IWebElement FindElement(int timeoutSecond)
        {
            DateTime dTimeout = DateTime.Now.AddSeconds(timeoutSecond);
            Exception ex = new NoSuchElementException(this.ToString());
            while (DateTime.Now < dTimeout)
            {
                try
                {
                    var webElement = InternalFindElement();
                    return webElement;
                }
                catch (InvalidSelectorException)
                {
                    DebugLogger.WriteError("The selector is invalid. " + this.ToString());
                    throw;
                }
                catch (NoSuchElementException ex1)
                {
                    ex = ex1;
                    Thread.Sleep(1000);
                }
                catch (StaleElementReferenceException ex2)
                {
                    ex = ex2;
                    Thread.Sleep(1000);
                }
            }

            throw ex;
        }

        /// <summary>
        /// Find a sub-element of the current element, with default synchronization timeout.
        /// </summary>
        /// <param name="by">Mecanism of find sub-elements</param>
        /// <returns>The indentified web element.</returns>
        public IWebElement FindElement(By by)
        {
            var e = FindElement();
            return e.FindElement(by);
        }

        /// <summary>
        /// Find the first element within the context of the current element. with provided synchronization timeout
        /// </summary>
        /// <param name="by">Mecanism of find elements.</param>
        /// <param name="timeoutSecond">synchronization timeout in second.</param>
        /// <returns></returns>
        public IWebElement FindElement(By by, int timeoutSecond)
        {
            var e = FindElement(timeoutSecond);
            return e.FindElement(by);
        }


        /// <summary>
        /// Check if the given element exists on the screen
        /// </summary>
        /// <returns>true if the element exists, otherwise: false</returns>
        public bool Exists()
        {
            return Exists(Settings.Instance.SynchronzationTimeout);
        }

        /// <summary>
        /// Check if the given element exists on the screen, with given sychronization timeout.
        /// </summary>
        /// <param name="timeoutSecond">object synchronization timeout in second.</param>
        /// <returns>True is the element exists in the current web page, Otherwise False</returns>
        public bool Exists(int timeoutSecond)
        {
            try
            {
                FindElement(timeoutSecond);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Click on Web element.
        /// </summary>
        public void Click()
        {
            Perform(InternalClick);
        }


        /// <summary>
        /// Perform a secured action that may be influnced by web application reload behavoirs to avoid errors, such as <see cref="StaleElementReferenceException"/>
        /// </summary>
        /// <param name="action">The action to be performed. The name of the method</param>
        protected void Perform(Action action)
        {
            DateTime dTimeout = DateTime.Now.AddSeconds(Settings.Instance.SynchronzationTimeout);
            Exception ex = new TimeoutException($"Unable to perform the action {action.Method.Name} on the element {this.ToString()}");
            while (DateTime.Now < dTimeout)
            {
                try
                {
                    action();
                    return;
                }
                catch (NoSuchElementException ex2)
                {
                    ex = ex2;
                    Thread.Sleep(1000);
                }
                catch (StaleElementReferenceException ex1)
                {
                    ex = ex1;
                    Thread.Sleep(1000);
                }
            }
            throw ex;
        }

        /// <summary>
        /// Perform a secured action that may be influnced by web application reload behavoirs to avoid errors, such as <see cref="StaleElementReferenceException"/>
        /// </summary>
        /// <param name="action">The action to be performed. The name of the method</param>
        /// <param name="param">The parameter of the action.</param>

        protected IWebElement Perform(Func<string, IWebElement> action, string param)
        {
            DateTime dTimeout = DateTime.Now.AddSeconds(Settings.Instance.SynchronzationTimeout);
            Exception ex = new TimeoutException($"Unable to perform the action {action.Method.Name} on the element {this}");
            while (DateTime.Now < dTimeout)
            {
                try
                {
                    return action(param);
                }
                catch (NoSuchElementException ex2)
                {
                    ex = ex2;
                    Thread.Sleep(1000);
                }
                catch (StaleElementReferenceException ex1)
                {
                    ex = ex1;
                    Thread.Sleep(1000);
                }
            }
            throw ex;
        }

        /// <summary>
        /// Perform a secured action that may be influnced by web application reload behavoirs to avoid errors, such as <see cref="StaleElementReferenceException"/>
        /// </summary>
        /// <param name="action">The action to be performed. The name of the method</param>
        /// <param name="param">The parameter of the delegated method</param>
        protected void Perform(Action<string> action, string param)
        {
            DateTime dTimeout = DateTime.Now.AddSeconds(Settings.Instance.SynchronzationTimeout);
            Exception ex = new TimeoutException($"Unable to perform the action {action.Method.Name} on the element {this}");
            while (DateTime.Now < dTimeout)
            {
                try
                {
                    action(param);
                    return;
                }
                catch(NoSuchElementException ex2)
                {
                    ex = ex2;
                    Thread.Sleep(1000);
                }
                catch (StaleElementReferenceException ex1)
                {
                    ex = ex1;
                    Thread.Sleep(1000);
                }
            }
            throw ex;
        }

        /// <summary>
        /// Perform a secured action that may be influnced by web application reload behavoirs to avoid errors, such as <see cref="StaleElementReferenceException"/>
        /// </summary>
        /// <param name="action">The action to be performed. The name of the method</param>
        /// <param name="param">The parameter of the delegated method</param>
        protected void Perform(Action<int> action, int param)
        {
            DateTime dTimeout = DateTime.Now.AddSeconds(Settings.Instance.SynchronzationTimeout);
            Exception ex = new TimeoutException($"Unable to perform the action {action.Method.Name} on the element {this}");
            while (DateTime.Now < dTimeout)
            {
                try
                {
                    action(param);
                    return;
                }
                catch (NoSuchElementException ex2)
                {
                    ex = ex2;
                    Thread.Sleep(1000);
                }
                catch (StaleElementReferenceException ex1)
                {
                    ex = ex1;
                    Thread.Sleep(1000);
                }
            }
            throw ex;
        }

        /// <summary>
        /// Perform a secured action that may be influnced by web application reload behavoirs to avoid errors, such as <see cref="StaleElementReferenceException"/>
        /// </summary>
        /// <param name="action">The action to be performed. The name of the method</param>
        /// <returns>value returned by underlying method</returns>
        protected bool Perform(Func<bool> action)
        {
            DateTime dTimeout = DateTime.Now.AddSeconds(Settings.Instance.SynchronzationTimeout);
            Exception ex = new TimeoutException($"Unable to perform the action {action.Method.Name} on the element {this}");
            while (DateTime.Now < dTimeout)
            {
                try
                {
                    return action();
                }
                catch (NoSuchElementException ex2)
                {
                    ex = ex2;
                    Thread.Sleep(1000);
                }
                catch (StaleElementReferenceException ex1)
                {
                    ex = ex1;
                    Thread.Sleep(1000);
                }
            }
            throw ex;
        }

        /// <summary>
        /// Perform an action which returns an screenshot of the given element or context
        /// </summary>
        /// <param name="action">Delegation which returns an screenshot</param>
        /// <returns>A screenshot of given element or context.</returns>
        protected byte[] Perform(Func<byte[]> action)
        {
            DateTime dTimeout = DateTime.Now.AddSeconds(Settings.Instance.SynchronzationTimeout);
            Exception ex = new TimeoutException($"Unable to perform the action {action.Method.Name} on the element {this}");
            while (DateTime.Now < dTimeout)
            {
                try
                {
                    return action();
                }
                catch (NoSuchElementException ex2)
                {
                    ex = ex2;
                    Thread.Sleep(1000);
                }
                catch (StaleElementReferenceException ex1)
                {
                    ex = ex1;
                    Thread.Sleep(1000);
                }
            }
            throw ex;
        }

        /// <summary>
        /// Generates an screenshot of this specific element
        /// </summary>
        /// <returns>The screenshot image in RAW Binary data format.</returns>
        public byte[] GetScreenshot()
        {
            return Perform(InternalGetScreenshot);
        }

        /// <summary>
        /// Implemente screenshot generation for the current specific element
        /// </summary>
        /// <returns>The screenshot image in RAW Binary data format.</returns>
        protected abstract byte[] InternalGetScreenshot();


        /// <summary>
        /// Perform a secured action that may be influnced by web application reload behavoirs to avoid errors, such as <see cref="StaleElementReferenceException"/>
        /// </summary>
        /// <param name="action">The action to be performed. The name of the method</param>
        /// <returns>value returned by underlying method</returns>
        protected string Perform(Func<string> action)
        {
            DateTime dTimeout = DateTime.Now.AddSeconds(Settings.Instance.SynchronzationTimeout);
            Exception ex = new TimeoutException($"Unable to perform the action {action.Method.Name} on the element {this}");
            while (DateTime.Now < dTimeout)
            {
                try
                {
                    return action();
                }
                catch (NoSuchElementException ex2)
                {
                    ex = ex2;
                    Thread.Sleep(1000);
                }
                catch (StaleElementReferenceException ex1)
                {
                    ex = ex1;
                    Thread.Sleep(1000);
                }
            }
            throw ex;
        }


        /// <summary>
        /// Perform a secured action that may be influnced by web application reload behavoirs to avoid errors, such as <see cref="StaleElementReferenceException"/>
        /// </summary>
        /// <param name="action">The action to be performed. The name of the method</param>
        /// <param name="param">The parameter of the delegated method</param>
        /// <returns>value returned by underlying method</returns>
        protected string Perform(Func<string, string> action, string param)
        {
            DateTime dTimeout = DateTime.Now.AddSeconds(Settings.Instance.SynchronzationTimeout);
            Exception ex = new TimeoutException($"Unable to perform the action {action.Method.Name} on the element {this}");
            while (DateTime.Now < dTimeout)
            {
                try
                {
                    return action(param);
                }
                catch (NoSuchElementException ex2)
                {
                    ex = ex2;
                    Thread.Sleep(1000);
                }
                catch (StaleElementReferenceException ex1)
                {
                    ex = ex1;
                    Thread.Sleep(1000);
                }
            }
            throw ex;
        }

        /// <summary>
        /// Default behavior of Click
        /// </summary>
        protected virtual void InternalClick()
        {
            var e = FindElement();       
            e.Click();
            
        }

        /// <summary>
        /// Send keys (text) to the current web element. (for textbox or text areas).
        /// Sendkeys does not clear the current value in the textbox or text areas. The text will be inserted according to the cursor location.
        /// If you want to ensure the textbox contains exactly the value you want to set, use the function <see cref="SetValue(string)"/> 
        /// </summary>
        /// <param name="text">the text to send</param>
        public void SendKeys(string text)
        {
            Perform(InternalSendKeys, text);
        }

        /// <summary>
        /// Set the value of the current web element. For textbox or text areas.
        /// SetValue clears existing value in the textbox or text area before input the text in it. 
        /// If you want to append or insert the text without clearing current value, use the function <see cref="SendKeys(string)"/> 
        /// </summary>
        /// <param name="text">The text to set.</param>
        public void SetValue(string text)
        {
            Perform(InternalSetValue, text);
        }


        private void InternalSetValue(string text)
        {
            var element = InternalFindElement();
            element.Clear();
            element.SendKeys(text);
        }

        private void InternalSendKeys(string text)
        {
            var e = FindElement();
            e.SendKeys(text);
        }


        /// <summary>
        /// Get the Text attribute of the current web element.
        /// </summary>
        /// <returns>The Text of the current web element</returns>
        public string GetText()
        {
            return Perform(InternalGetText);
        }

        /// <summary>
        /// Get the Value attribute of the current web element (For Input text box for example)
        /// </summary>
        public string Value
        {
            get
            {
                return Perform(InternalGetValue);
            }
        }

        private string InternalGetValue()
        {
            var e = FindElement();
            return e.GetDomProperty("value");
        }

        private string InternalGetText()
        {
            var e = FindElement();
            return e.Text;
        }

        /// <summary>
        /// Check if the WebElement is selected. Applies only on checkboxes, options in a select element and radio buttons
        /// </summary>
        /// <returns>True is the element is selected, otherwise, false</returns>
        public bool IsSelected
        {
            get
            {
                return Perform(InternalIsSelected);
            }
        }

        private bool InternalIsSelected()
        {
            var e = FindElement();
            return e.Selected;
        }

        /// <summary>
        /// Get the value indicating whether the current element is enabled.
        /// </summary>
        /// <returns>True if the element is enabled, otherwise: False </returns>
        public bool IsEnabled
        {
            get
            {
                return Perform(InternalIsEnabled);
            }
        }

        private bool InternalIsEnabled()
        {
            var e = FindElement();
            return e.Enabled;
        }

        /// <summary>
        /// Check if The WebElement is displayed on the screen (In the current viewport or not)
        /// </summary>
        /// <returns>True if the element exists and is displayed. False if the element exsits but not displayed. <see cref="NoSuchElementException"/> if the element can't be find in the DOM.</returns>
        /// <remarks>If it is required to perform actions on non-displayed element, please bring the element into view before the action, using ScrollIntoView method.</remarks>
        public bool IsDisplayed
        {
            get
            {
                return Perform(InternalIsDisplayed);
            }
        }



        private bool InternalIsDisplayed()
        {
            DateTime dTimeout = DateTime.Now.AddSeconds(Settings.Instance.SynchronzationTimeout);
            bool value = false;
            while (DateTime.Now < dTimeout)
            {                   
                var e = FindElement();
                value = e.Displayed;
                if (value) return value;
                else Thread.Sleep(1000);
            }
            return value;

        }

        /// <summary>
        /// Clear the content of the current web element. (for text-box and text areas)
        /// </summary>
        public void Clear()
        {
            Perform(InternalClear);
        }

        private void InternalClear()
        {
            var e = FindElement();
            e.Clear();
        }

        /// <summary>
        /// Get the value of the specified DOM attribute of this element.
        /// </summary>
        /// <param name="attributeName">the name of Html DOM attribute.</param>
        /// <returns>
        /// The value of the indicated attribute.
        /// </returns>
        public string GetAttribute(string attributeName)
        {
            return Perform(InternalGetAttribute, attributeName);
        }

        private string InternalGetAttribute(string attributeName)
        {
            var e = FindElement();
            return e.GetDomAttribute(attributeName);
        }

        /// <summary>
        /// Get the value of the specified DOM property of this element.
        /// </summary>
        /// <param name="propertyName">the name of Html Property.</param>
        /// <returns>
        /// The value of the indicated attribute.
        /// </returns>
        public string GetProperty(string propertyName)
        {
            return Perform(InternalGetProperty, propertyName);
        }

        private string InternalGetProperty(string propertyName)
        {
            var e = FindElement();
            return e.GetDomProperty(propertyName);
        }

        /// <summary>
        /// Find elements within the context of the current element.
        /// </summary>
        /// <param name="by">Mecanism of find elements</param>
        /// <returns></returns>
        public IReadOnlyCollection<IWebElement> FindElements(By by)
        {
            var e = FindElement();
            return e.FindElements(by);
        }

        /// <summary>
        /// This method find all possible elements given the current selection criteria. If there is no element match an <see cref="NoSuchElementException"/> will be thrown
        /// </summary>
        /// <returns>A collection of <see cref="IWebElement"/> matches the element selection criteria</returns>
        public IReadOnlyCollection<IWebElement> FindElements()
        {
            return FindElements(Settings.Instance.SynchronzationTimeout);
        }

        /// <summary>
        /// This method find all possible elements given the current selection criteria. If there is no element match an <see cref="NoSuchElementException"/> will be thrown
        /// </summary>
        /// <param name="timeoutSecond">Timeout in seconds</param>
        /// <returns>A collection of <see cref="IWebElement"/> matches the element selection criteria</returns>
        public IReadOnlyCollection<IWebElement> FindElements(int timeoutSecond)
        {
            DateTime dTimeout = DateTime.Now.AddSeconds(timeoutSecond);
            Exception ex = new TimeoutException($"Unable to Findelement {this} within the given timeout.");
            while (DateTime.Now < dTimeout)
            {
                try
                {
                    var webElement = InternalFindElements();
                    return webElement;
                }
                catch (NoSuchElementException ex2)
                {
                    ex = ex2;
                }
                catch (StaleElementReferenceException ex1)
                {
                    ex = ex1;
                }
                catch( Exception ex3)
                {
                    ex = ex3;
                }
                Thread.Sleep(1000);
            }
            throw ex;
        }

        /// <summary>
        /// Implemente element identifying algorithm, without the need of object synchronization.
        /// </summary>
        /// <returns>Identified test objects</returns>
        protected abstract IReadOnlyCollection<IWebElement> InternalFindElements();
    }
}
