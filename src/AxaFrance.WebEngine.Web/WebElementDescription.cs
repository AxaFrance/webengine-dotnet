// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace AxaFrance.WebEngine.Web
{
    /// <summary>
    /// The Object Description how to identify the Element for Web Application.
    /// </summary>
    public class WebElementDescription : ElementDescription
    {
        /// <summary>
        /// Initialize the Element. If the element is not created within a <see cref="PageModel"/>, you should use <see cref="ElementDescription.UseDriver(WebDriver)"/> before us the element.
        /// </summary>
        public WebElementDescription() { }

        /// <summary>
        /// Initialize the ElementDescription with a given WebDriver as contexte to use.
        /// </summary>
        /// <param name="driver">WebDriver instance to use.</param>
        public WebElementDescription(WebDriver driver)
        {
            UseDriver(driver);
        }

        /// <summary>
        /// The Id attribute of the HTML element.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The Name attribute of the HTML element.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// InnerText of HTML element.
        /// </summary>
        public string InnerText { get; set; }


        /// <summary>
        /// Other non-standard HTML Elements 
        /// </summary>
        public IEnumerable<HtmlAttribute> Attributes { get; set; }

        /// <summary>
        /// XPath of the element. avoid to use absolute XPATH if another identification method is available.
        /// </summary>
        public string XPath { get; set; }

        /// <summary>
        /// CssSelector of the element. avoid to use CssSelector if other identification method is available.
        /// </summary>
        public string CssSelector { get; set; }

        /// <summary>
        /// CSS class name
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// The HTML Tag name
        /// </summary>
        public string TagName { get; set; }

        /// <summary>
        /// The Text of a Hyperlink
        /// </summary>
        public string LinkText { get; set; }

        /// <summary>
        /// Gets the InnerHtml of the current element.
        /// </summary>
        /// <returns>A string representation of InnerHtml the current WebElement</returns>
        public string GetInnerHtml()
        {
            return FindElement().GetDomProperty("innerHTML");
        }

        /// <summary>
        /// Gets the OuterHtml of the current element.
        /// </summary>
        /// <returns>A string representation of InnerHtml the current WebElement</returns>
        public string GetOuterHtml()
        {
            return FindElement().GetDomProperty("outerHTML");
        }

        /// <summary>
        /// Set the value of a password textbox
        /// </summary>
        /// <param name="encrypted">A base64 formatted encrypted data. using default encrptionkey provided in appsetting.json</param>
        public void SetSecure(string encrypted)
        {
            Perform(InternalSetSecure, encrypted);
        }

        private void InternalSetSecure(string encrypted)
        {
            var element = InternalFindElement();
            var type = element.GetDomProperty("type");
            if (type == null || type.ToLower() != "password")
            {
                throw new NotSupportedException("Cannot use SetSecure on a non-password field");
            }
            else
            {
                element.Clear();
                element.SendKeys(Encrypter.Decrypt(encrypted));
            }
        }

        /// <summary>
        /// Shows a string representation of this WebElementDescription
        /// </summary>
        /// <returns>A string representation of this WebElementDescription</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(Id))
            {
                sb.Append($"Id={Id}");
            }
            if (!string.IsNullOrEmpty(Name))
            {
                sb.Append($"Name={Name}");
            }
            if (!string.IsNullOrEmpty(InnerText))
            {
                sb.AppendLine($"InnerText={InnerText}");
            }
            if (!string.IsNullOrEmpty(XPath))
            {
                sb.AppendLine($"XPath={XPath}");
            }
            if (!string.IsNullOrEmpty(TagName))
            {
                sb.AppendLine($"TagName={TagName}");
            }
            return sb.ToString();
        }


        /// <inheritdoc />
        protected override IWebElement InternalFindElement()
        {
            var elements = InternalFindElements();
            if (elements.Count > 1)
            {
                throw new InvalidSelectorException("Multiple element has found with the given selection criteria");
            }
            else
            {
                return elements.First();
            }
        }

        /// <inheritdoc />
        protected override IReadOnlyCollection<IWebElement> InternalFindElements()
        {
            IEnumerable<IWebElement> elements = null;
            ISearchContext context = driver;
            if(this.ShadowRoot != null)
            {
                ShadowRoot.UseDriver(driver);
                var root = ShadowRoot.InternalFindElements();
                if (root.Count > 1)
                {
                    throw new InvalidSelectorException("Multiple element has found with the given selection criteria for ShadowRoot");
                }else if(root.Count == 0)
                {
                    throw new NoSuchElementException("No such Shadow Root found:" + ShadowRoot.ToString());
                }
                else
                {
                    context = root.First().GetShadowRoot();
                }
            }
            if (this.Id != null) 
            {
                elements = context.FindElements(By.Id(this.Id));
            }

            if (this.Name != null)
            {
                var names = context.FindElements(By.Name(this.Name));
                if (elements == null)
                {
                    elements = names;
                }
                else
                {
                    elements = elements.Where(x => names.Contains(x));
                }
            }

            if (this.ClassName != null)
            {
                string xpath = $"//*[@class='{ClassName}']";
                var cssnames = context.FindElements(By.XPath(xpath));
                if (elements == null)
                {
                    elements = cssnames;
                }
                else
                {
                    elements = elements.Where(x => cssnames.Contains(x));
                }
            }

            if (this.LinkText != null)
            {
                var links = context.FindElements(By.LinkText(this.LinkText));
                if (elements == null)
                {
                    elements = links;
                }
                else
                {
                    elements = elements.Where(x => links.Contains(x));
                }
            }

            if (this.TagName != null)
            {
                var tagNames = context.FindElements(By.TagName(TagName.ToUpper()));
                if (elements == null)
                {
                    elements = tagNames;
                }
                else
                {
                    elements = elements.Where(x => tagNames.Contains(x));
                }
            }

            if (this.CssSelector != null)
            {
                var classes = context.FindElements(By.CssSelector(CssSelector));
                if (elements == null)
                {
                    elements = classes;
                }
                else
                {
                    elements = elements.Where(x => classes.Contains(x));
                }

            }

            if (this.XPath != null)
            {
                var xpaths = context.FindElements(By.XPath(this.XPath));
                if (elements == null)
                {
                    elements = xpaths;
                }
                else
                {
                    elements = elements.Where(x => xpaths.Contains(x));
                }
            }

            if (this.InnerText != null)
            {
                if (elements != null)
                {
                    elements = elements.Where(x => x.GetDomProperty("innerText") == InnerText);
                }
                else
                {
                    elements = context.FindElements(By.XPath($"//*[text()='{InnerText}']"));
                }
            }

            if (this.Attributes != null && this.Attributes.Any())
            {
                List<string> attributes = new List<string>();
                foreach (var a in this.Attributes)
                {
                    attributes.Add($"[{a.Name}=\"{a.Value}\"]");
                }
                string cssSelector = $"{string.Join("", attributes)}";
                var attr = context.FindElements(By.CssSelector(cssSelector));
                if (elements == null)
                {
                    elements = attr;
                }
                else
                {
                    elements = elements.Where(x => attr.Contains(x));
                }
            }

            if (elements == null || elements.Count() == 0)
            {
                throw new NoSuchElementException($"No such WebElement: {this}");
            }
            else
            {
                return new ReadOnlyCollection<IWebElement>(elements.ToList());

            }
        }


        /// <summary>
        /// Simulate a mouse hover on the indicated WebElement.
        /// </summary>
        public void MouseHover()
        {
            var e = FindElement();
            if (e is AppiumElement ae)
            {
                SafeClick(ae);
            }
            else
            {
                Actions act = new Actions(driver);
                act.MoveToElement(e).Build().Perform();
            }
        }

        /// <summary>
        /// Simulate a mouse right click on the indicated WebElement.
        /// </summary>
        public void RightClick()
        {
            var e = FindElement();
            if (driver is AppiumDriver ad)
            {
                var touch = new Actions(driver);
                touch.ClickAndHold(e).Pause(new TimeSpan(0, 0, 2)).Click();
            }
            else
            {

                Actions act = new Actions(driver);
                act.MoveToElement(e).ContextClick(e).Build().Perform();
            }
        }

        /// <summary>
        /// Drag the current element and drop it to <paramref name="element"/>.
        /// </summary>
        /// <param name="element">The target element to drop.</param>
        public void DragAndDropTo(IWebElement element)
        {
            var e = FindElement();
            Actions act = new Actions(driver);
            act.DragAndDrop(e, element).Build().Perform();
        }

        /// <summary>
        /// Scroll the current element into the visible area.
        /// </summary>
        public void ScrollIntoView()
        {
            var e = FindElements();
            var e0 = e.FirstOrDefault();
            driver.ExecuteScript("arguments[0].scrollIntoView(true);", e0);
        }

        /// <summary>
        /// Checks if the WebElement is displayed in the current viewport.
        /// This function is based on javascript function getBoundingClientRect(), it may not working on older version (67.0) of Chrome on mobile devices.
        /// </summary>
        public bool IsVisibleInViewPort
        {
            get
            {
                return Perform(InternalInViewPort);
            }
        }

        /// <summary>
        /// Describes the ShadowRoot if the element is placed in a shadow DOM.
        /// The description of the current element wil be based in the scope of the ShadowRoot.
        /// </summary>
        /// <remarks>
        /// <b>Warning:</b> When searching an element in the ShadowRoot: You can only use CssSelctor to describe the element.
        /// 
        /// </remarks>
        /// 
        public WebElementDescription ShadowRoot { get; set; }

        private bool InternalInViewPort()
        {
            //https://stackoverflow.com/questions/45243992/verification-of-element-in-viewport-in-selenium/45244889#45244889
            return (bool)(driver.ExecuteScript("var elem = arguments[0], box = elem.getBoundingClientRect(), cx = box.left + box.width / 2,  cy = box.top + box.height / 2,  e = document.elementFromPoint(cx, cy); for (; e; e = e.parentElement) { if (e === elem) return true;} return false;", this.FindElement()));
        }


        /// <summary>
        /// Drag the current element and drop it to <paramref name="element"/>. It may not work on Mobile Devices.
        /// </summary>
        /// <param name="element">The target element to drop.</param>
        public void DragAndDropTo(ElementDescription element)
        {
            var from = FindElement();
            var to = element.FindElement();

            if (driver is AppiumDriver)
            {
                //On Mobile devices, there is no click to drag and drop. Drag and drop events are raised by a Long tap.
                MobileDragAndDrop(from, to);
            }
            else
            {
                /*
                //Desktop use JS Executor because native Action does not work: ->
                Actions actions = new Actions(driver);
                actions.DragAndDrop(e1, e2).Build().Perform();
                */

                //Solution refers to: https://stackoverflow.com/questions/62571462/selenium-drag-and-drop-not-working-with-action-methods-is-there-any-alterative
                driver.ExecuteScript("function createEvent(typeOfEvent) {\n" + "var event =document.createEvent(\"CustomEvent\");\n"
                    + "event.initCustomEvent(typeOfEvent,true, true, null);\n" + "event.dataTransfer = {\n" + "data: {},\n"
                    + "setData: function (key, value) {\n" + "this.data[key] = value;\n" + "},\n"
                    + "getData: function (key) {\n" + "return this.data[key];\n" + "}\n" + "};\n" + "return event;\n"
                    + "}\n" + "\n" + "function dispatchEvent(element, event,transferData) {\n"
                    + "if (transferData !== undefined) {\n" + "event.dataTransfer = transferData;\n" + "}\n"
                    + "if (element.dispatchEvent) {\n" + "element.dispatchEvent(event);\n"
                    + "} else if (element.fireEvent) {\n" + "element.fireEvent(\"on\" + event.type, event);\n" + "}\n"
                    + "}\n" + "\n" + "function simulateHTML5DragAndDrop(element, destination) {\n"
                    + "var dragStartEvent =createEvent('dragstart');\n" + "dispatchEvent(element, dragStartEvent);\n"
                    + "var dropEvent = createEvent('drop');\n"
                    + "dispatchEvent(destination, dropEvent,dragStartEvent.dataTransfer);\n"
                    + "var dragEndEvent = createEvent('dragend');\n"
                    + "dispatchEvent(element, dragEndEvent,dropEvent.dataTransfer);\n" + "}\n" + "\n"
                    + "var source = arguments[0];\n" + "var destination = arguments[1];\n"
                    + "simulateHTML5DragAndDrop(source,destination);", from, to);
            }

        }

        private void MobileDragAndDrop(IWebElement from, IWebElement to)
        {
            Actions actions = new Actions(driver);
            actions.ClickAndHold(from).Pause(new TimeSpan(0, 0, 2)).MoveToElement(to).Release().Build().Perform();
        }

        /// <summary>
        /// Returns an WebElement as <see cref="SelectElement"/> to get more possible operations.
        /// </summary>
        /// <returns>An <see cref="SelectElement"/> object represents an HTML &lt;select&gt; tag.</returns>
        public SelectElement AsSelect()
        {
            var element = this.FindElement();
            SelectElement se = new SelectElement(element);
            return se;
        }

        /// <summary>
        /// Select an option from html tag &lt;select&gt; by its displayed text.
        /// </summary>
        /// <param name="text">the text value to be selected.</param>
        public void SelectByText(string text)
        {
            Perform(InternalSelectByText, text);
        }

        private void InternalSelectByText(string text)
        {
            this.ScrollIntoView();
            var element = this.InternalFindElement();
            SafeClick(element);
            SelectElement se = new SelectElement(element);
            se.SelectByText(text);
        }


        /// <summary>
        /// Select an option from html tag &lt;select&gt; by its displayed text.
        /// </summary>
        /// <param name="index">The 0-based index to be selected.</param>
        public void SelectByIndex(int index)
        {
            Perform(InternalSelectByIndex, index);
        }

        private void InternalSelectByIndex(int index)
        {
            this.ScrollIntoView();
            var element = this.InternalFindElement();
            SafeClick(element);
            SelectElement se = new SelectElement(element);
            se.SelectByIndex(index);

        }

        /// <summary>
        /// Select an option from html tag &lt;select&gt; by its value.
        /// </summary>
        /// <param name="value">the text value to be selected.</param>
        public void SelectByValue(string value)
        {
            Perform(InternalSelectByValue, value);
        }


        /// <summary>
        /// Checks an RadioButton from a given RadioGroup (used for radio button group, &lt;input type="radio"&gt; elements)
        /// </summary>
        /// <param name="value">the value to be checked in current Radiobutton group.</param>
        public IWebElement CheckByValue(string value)
        {
            return Perform(InternalCheckByValue, value);
        }

        private IWebElement InternalCheckByValue(string value)
        {
            this.ScrollIntoView();
            Thread.Sleep(500);
            var elements = this.InternalFindElements();
            if (elements != null)
            {
                foreach (var element in elements)
                {
                    if (element.GetDomProperty("value") == value)
                    {
                        SafeClick(element);
                        return element;
                    }
                }
            }
            throw new NoSuchElementException($"The radiogroup {this} does not exist or doesn't have an element with value {value}");
        }

        /// <summary>
        /// Overrides the default behavior of Click: Scrolling the element into view to avoid ElementClickInterceptedException
        /// </summary>

        protected override void InternalClick()
        {
            var elem = this.FindElement();
            try
            {
                SafeClick(elem);
            }
            catch (ElementClickInterceptedException)
            {
                this.ScrollIntoView();
                Thread.Sleep(500);
                SafeClick(elem);
            }

        }

        private void InternalSelectByValue(string value)
        {
            this.ScrollIntoView();
            var element = this.InternalFindElement();
            SafeClick(element);
            SelectElement se = new SelectElement(element);
            se.SelectByValue(value);
        }

        /// <inheritdoc />
        protected override byte[] InternalGetScreenshot()
        {
            var element = FindElement();
            if (element is WebElement we)
            {
                var screenshot = we.GetScreenshot();
                return screenshot.AsByteArray;
            }
            else
            {
                throw new ElementNotInteractableException($"Cannot covert IWebElement to WebElement for screenshot. {this}");
            }

        }

        private void SafeClick(IWebElement element)
        {
            try
            {
                if (Settings.Instance.UseJavaScriptClick == true)
                {
                    driver.ExecuteScript("arguments[0].click()", element);
                }
                else
                {
                    element.Click();
                }
            }
            catch (ElementNotInteractableException)
            {
                driver.ExecuteScript("arguments[0].click()", element);
            }
        }

        /// <inheritdoc />
        public override void ApplyAttribute(FindsByAttribute attr)
        {
            switch (attr.How)
            {
                case How.Id:
                    if (Id == null) Id = attr.Using;
                    break;
                case How.Name:
                    if (Name == null) Name = attr.Using;
                    break;
                case How.TagName:
                    if (TagName == null) TagName = attr.Using;
                    break;
                case How.ClassName:
                    if (ClassName == null) ClassName = attr.Using;
                    break;
                case How.LinkText:
                case How.PartialLinkText:
                    if (LinkText == null) LinkText = attr.Using;
                    break;
                case How.CssSelector:
                    if (CssSelector == null) CssSelector = attr.Using;
                    break;
                case How.XPath:
                    if (XPath == null) XPath = attr.Using;
                    break;
                case How.Custom:
                default:
                    throw new NotSupportedException("FindsByAttribute does not support Custom yet.");
            }
        }
    }
}
