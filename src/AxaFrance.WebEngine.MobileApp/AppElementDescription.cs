// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using AxaFrance.WebEngine.Web;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.iOS;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace AxaFrance.WebEngine.MobileApp
{
    /// <summary>
    /// The Object Description how to identify the UI Elemen for native Mobile Application
    /// </summary>
    public class AppElementDescription : ElementDescription
    {

        /// <summary>
        /// Initialize the element description. If the element is not part of PageObject, you need to call <see cref="ElementDescription.UseDriver(WebDriver)"/> to indicate with WebDriver will be used.
        /// </summary>
        public AppElementDescription()
        {

        }

        /// <summary>
        /// Initialize the element description using given WebDriver
        /// </summary>
        /// <param name="driver">WebDriver instance</param>
        public AppElementDescription(WebDriver driver)
        {
            this.UseDriver(driver);
        }

        /// <summary>
        /// Identifier of the element
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The Name property of the element. Name and ContentDescription can't be used in the same time, or Name attribute will be ignored.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// To identify element via it's AccessibilityId. Id and AccessibilityId can't be used in the same time.
        /// </summary>
        public string AccessbilityId { get; set; }

        /// <summary>
        /// To identify elements via it's content-desc. Name and ContentDescription can't be used in the same time.
        /// </summary>
        public string ContentDescription { get; set; }

        /// <summary>
        /// Text of Innertext of the element
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// XPath of the element. Please avoid to use XPATH if another identification approche is available.
        /// </summary>
        public string XPath { get; set; }

        /// <summary>
        /// Classname of the element.
        /// </summary>
        public string ClassName { get; set; }


        ///<summary>
        /// Using IOS Class Chain, only avaiable for iOS applications. When using IosClassChain, other locators will be ignored..
        ///</summary>
        public string IosClassChain { get; set; }

        /// <summary>
        /// Using UIAutomator selector, only avaiable for Android applications. When using UIAutomator selector, other locators will be ignored.
        /// </summary>
        public string UIAutomatorSelector { get; set; }

        /// <summary>
        /// Using iOS NSPredicate string, only available for iOS applications. When using IosPredicate, other locators will be ignored.
        /// More powerful than IosClassChain for complex queries.
        /// </summary>
        public string IosPredicate { get; set; }

        /// <summary>
        /// Using Android DataMatcher selector, only available for Android applications (Espresso).
        /// When using AndroidDataMatcher, other locators will be ignored.
        /// </summary>
        public string AndroidDataMatcher { get; set; }

        /// <summary>
        /// Using Image element matching (AI-based element finding).
        /// Useful when standard locators don't work. Requires Appium image plugin.
        /// </summary>
        public string ImageLocator { get; set; }

        /// <summary>
        /// Shows a string representation of this AppElementDescription
        /// </summary>
        /// <returns>A string representation of this AppElementDescription</returns>
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
            if (!string.IsNullOrEmpty(Text))
            {
                sb.AppendLine($"Text={Text}");
            }
            if (!string.IsNullOrEmpty(XPath))
            {
                sb.AppendLine($"Text={XPath}");
            }
            if (!string.IsNullOrEmpty(AccessbilityId))
            {
                sb.AppendLine($"AccessibilityId={AccessbilityId}");
            }
            if (!string.IsNullOrEmpty(ContentDescription))
            {
                sb.AppendLine($"ContentDescription={ContentDescription}");
            }
            return sb.ToString();
        }

        /// <inheritdoc/>
        protected override IWebElement InternalFindElement()
        {
            var elements = InternalFindElements();
            if (elements.Count() > 1)
            {
                throw new OpenQA.Selenium.InvalidSelectorException($"Multiple elements are found.");
            }
            else
            {
                return elements.First();
            }

        }

        /// <summary>
        /// Tries to bring current element into viewport.
        /// This method will scroll down the page  multiple times to make the element visible.
        /// If after the maximum scroll action and the element is still not visible, the method will return false.
        /// </summary>
        /// <returns>True if the element is visible after scrolling. False when the element is not visible.</returns>
        public bool ScrollIntoView(int maxSwipe = 10)
        {
            return ScrollIntoViewDown(maxSwipe);
        }

        /// <summary>
        /// Tries to bring current element into viewport.
        /// This method will scroll the page multiple times to make the element visible.
        /// If after the maximum scroll action and the element is still not visible, the method will return false.
        /// </summary>
        /// <param name="direction">The direction of the scroll</param>
        /// <param name="maxSwipe">The maximum number of swipe action to make the element visible</param>
        /// <returns>True if the element is visible after scrolling. False when the element is not visible.</returns>
        public bool ScrollIntoView(ScrollDirection direction, int maxSwipe = 10)
        {
            switch (direction)
            {
                case ScrollDirection.Up:
                    return ScrollIntoViewUp(maxSwipe);
                case ScrollDirection.Down:
                    return ScrollIntoViewDown(maxSwipe);
                case ScrollDirection.Left:
                    return ScrollIntoViewLeft(maxSwipe);
                case ScrollDirection.Right:
                    return ScrollIntoViewRight(maxSwipe);
                default:
                    return ScrollIntoViewDown(maxSwipe);
            }
        }

        /// <summary>
        /// Waits until the element is clickable (visible and enabled)
        /// </summary>
        /// <param name="timeoutSeconds">Maximum wait time in seconds</param>
        /// <returns>True if element became clickable, false otherwise</returns>
        public bool WaitUntilClickable(int timeoutSeconds = 10)
        {
            var endTime = DateTime.Now.AddSeconds(timeoutSeconds);
            while (DateTime.Now < endTime)
            {
                try
                {
                    if (this.Exists(1) && this.IsEnabled && this.IsDisplayed)
                        return true;
                }
                catch (NoSuchElementException)
                {
                    // Continue waiting
                }
                Thread.Sleep(500);
            }
            return false;
        }

        private bool ScrollIntoViewDown(int maxSwipe)
        {
            int count = 0;
            while (!this.Exists(1) && count < maxSwipe)
            {
                count++;
                ScrollDown();
            }
            return this.Exists(1);
        }

        private bool ScrollIntoViewUp(int maxSwipe)
        {
            int count = 0;
            while (!this.Exists(1) && count < maxSwipe)
            {
                count++;
                ScrollUp();
            }
            return this.Exists(1);
        }

        private bool ScrollIntoViewLeft(int maxSwipe)
        {
            int count = 0;
            while (!this.Exists(1) && count < maxSwipe)
            {
                count++;
                SwipeLeft();
            }
            return this.Exists(1);
        }

        private bool ScrollIntoViewRight(int maxSwipe)
        {
            int count = 0;
            while (!this.Exists(1) && count < maxSwipe)
            {
                count++;
                SwipeRight();
            }
            return this.Exists(1);
        }


        /// <summary>
        /// Scroll the screen downward
        /// </summary>
        public void ScrollDown()
        {
            var x = (int)(driver.Manage().Window.Size.Width * 0.5);
            GenericScroll(x, 0.8, 0.3);
        }

        /// <summary>
        /// Scroll the screen upward
        /// </summary>
        public void ScrollUp()
        {
            var x = (int)(driver.Manage().Window.Size.Width * 0.5);
            GenericScroll(x, 0.3, 0.8);
        }

        /// <summary>
        /// Performs a long press on the current element (useful for context menus, drag operations)
        /// </summary>
        /// <param name="durationSeconds">Duration of the press in seconds (default: 2)</param>
        public void LongPress(int durationSeconds = 2)
        {
            var element = FindElement();
            var finger = new PointerInputDevice(PointerKind.Touch);
            var actionSequence = new ActionSequence(finger, 0);

            actionSequence.AddAction(finger.CreatePointerMove(element, 0, 0, TimeSpan.Zero));
            actionSequence.AddAction(finger.CreatePointerDown(MouseButton.Touch));
            actionSequence.AddAction(finger.CreatePause(new TimeSpan(0, 0, durationSeconds)));
            actionSequence.AddAction(finger.CreatePointerUp(MouseButton.Touch));

            driver.PerformActions(new List<ActionSequence> { actionSequence });
        }

        /// <summary>
        /// Performs a double tap on the current element (useful for zoom, selection, special interactions)
        /// </summary>
        public void DoubleTap()
        {
            var element = FindElement();
            var finger = new PointerInputDevice(PointerKind.Touch);
            var actionSequence = new ActionSequence(finger, 0);

            // First tap
            actionSequence.AddAction(finger.CreatePointerMove(element, 0, 0, TimeSpan.Zero));
            actionSequence.AddAction(finger.CreatePointerDown(MouseButton.Touch));
            actionSequence.AddAction(finger.CreatePointerUp(MouseButton.Touch));

            // Short pause
            actionSequence.AddAction(finger.CreatePause(TimeSpan.FromMilliseconds(100)));

            // Second tap
            actionSequence.AddAction(finger.CreatePointerDown(MouseButton.Touch));
            actionSequence.AddAction(finger.CreatePointerUp(MouseButton.Touch));

            driver.PerformActions(new List<ActionSequence> { actionSequence });
        }

        /// <summary>
        /// Swipe left on the screen (useful for carousels, horizontal lists, dismissible cards).
        /// Y coordinate is calculated as the vertical center of the screen.
        /// If you want to swipe left on a specific element, please use <see cref="SwipeLeftOnElement"/> instead, which will calculate Y coordinate based on the element's position.
        /// </summary>
        public void SwipeLeft()
        {
            var y = (int)(driver.Manage().Window.Size.Height * 0.5);
            GenericSwipe(0.8, 0.2, y);
        }

        /// <summary>
        /// Performs a leftward swipe gesture on the currently located UI element.
        /// </summary>
        /// <remarks>Use this method to simulate a user swiping left on an element, typically for actions
        /// such as revealing hidden options or dismissing items. The swipe is executed horizontally across the vertical
        /// center of the element. Ensure that an element is available and visible before calling this method to avoid
        /// unexpected behavior.</remarks>
        public void SwipeLeftOnElement()
        {
            var element = FindElement();
            var y = element.Location.Y + element.Size.Height / 2;
            GenericSwipe(0.8, 0.2, y);
        }

        /// <summary>
        /// Performs a rightward swipe gesture on the currently located UI element.
        /// </summary>
        public void SwipeRightOnElement()
        {
            var element = FindElement();
            var y = element.Location.Y + element.Size.Height / 2;
            GenericSwipe(0.2, 0.8, y);
        }

        /// <summary>
        /// Swipe right on the screen (useful for carousels, horizontal lists, navigation)
        /// Y coordinate is calculated as the vertical center of the screen.
        /// If you want to swipe left on a specific element, please use <see cref="SwipeLeftOnElement"/> instead, which will calculate Y coordinate based on the element's position.
        /// </summary>
        public void SwipeRight()
        {
            var y = (int)(driver.Manage().Window.Size.Height * 0.5);
            GenericSwipe(0.2, 0.8, y);
        }

        private void GenericSwipe(double startXPercent, double endXPercent, int y)
        {
            int startX = (int)(driver.Manage().Window.Size.Width * startXPercent);
            int endX = (int)(driver.Manage().Window.Size.Width * endXPercent);

            var finger = new PointerInputDevice(PointerKind.Touch);
            var actionSequence = new ActionSequence(finger, 0);

            actionSequence.AddAction(finger.CreatePointerMove(CoordinateOrigin.Viewport, startX, y, TimeSpan.Zero));
            actionSequence.AddAction(finger.CreatePointerDown(MouseButton.Touch));
            actionSequence.AddAction(finger.CreatePointerMove(CoordinateOrigin.Viewport, endX, y, new TimeSpan(0, 0, 1)));
            actionSequence.AddAction(finger.CreatePointerUp(MouseButton.Touch));

            driver.PerformActions(new List<ActionSequence> { actionSequence });
        }

        private void GenericScroll(int x, double startYPercent, double endYPercent)
        {
            int startY = (int)(driver.Manage().Window.Size.Height * startYPercent);
            int endY = (int)(driver.Manage().Window.Size.Height * endYPercent);

            var finger = new PointerInputDevice(PointerKind.Touch);
            var actionSequence = new ActionSequence(finger, 0);

            actionSequence.AddAction(finger.CreatePointerMove(CoordinateOrigin.Viewport, x, startY, TimeSpan.Zero));
            actionSequence.AddAction(finger.CreatePointerDown(MouseButton.Touch));
            actionSequence.AddAction(finger.CreatePointerMove(CoordinateOrigin.Viewport, x, endY, new TimeSpan(0, 0, 1)));
            actionSequence.AddAction(finger.CreatePointerUp(MouseButton.Touch));


            driver.PerformActions(new List<ActionSequence> { actionSequence });
        }

        /// <summary>
        /// Drags the current element and drops it at a target element (useful for reordering lists, moving items)
        /// </summary>
        /// <param name="target">The target element to drop onto</param>
        public void DragAndDropTo(AppElementDescription target)
        {
            var fromElement = FindElement();
            var toElement = target.FindElement();

            var fromLocation = fromElement.Location;
            var toLocation = toElement.Location;

            var finger = new PointerInputDevice(PointerKind.Touch);
            var actionSequence = new ActionSequence(finger, 0);

            actionSequence.AddAction(finger.CreatePointerMove(CoordinateOrigin.Viewport,
                fromLocation.X + fromElement.Size.Width / 2,
                fromLocation.Y + fromElement.Size.Height / 2,
                TimeSpan.Zero));
            actionSequence.AddAction(finger.CreatePointerDown(MouseButton.Touch));
            actionSequence.AddAction(finger.CreatePause(new TimeSpan(0, 0, 1))); // Hold briefly
            actionSequence.AddAction(finger.CreatePointerMove(CoordinateOrigin.Viewport,
                toLocation.X + toElement.Size.Width / 2,
                toLocation.Y + toElement.Size.Height / 2,
                new TimeSpan(0, 0, 1)));
            actionSequence.AddAction(finger.CreatePointerUp(MouseButton.Touch));

            driver.PerformActions(new List<ActionSequence> { actionSequence });
        }

        /// <summary>
        /// Hides the on-screen keyboard (works on both Android and iOS)
        /// </summary>
        public void HideKeyboard()
        {
            if (driver is AndroidDriver ad)
            {
                ad.HideKeyboard();
            }
            else if (driver is IOSDriver id)
            {
                id.HideKeyboard();
            }
        }

        /// <inheritdoc />
        protected override IReadOnlyCollection<IWebElement> InternalFindElements()
        {
            IEnumerable<IWebElement> elements = null;

            // Platform-specific exclusive locators (when used, ignore all other locators)
            if (IosClassChain != null)
            {
                var chains = driver.FindElements(MobileBy.IosClassChain(IosClassChain));
                return chains;
            }

            if (IosPredicate != null)
            {
                var predicates = driver.FindElements(MobileBy.IosNSPredicate(IosPredicate));
                return predicates;
            }

            if (UIAutomatorSelector != null)
            {
                var locators = driver.FindElements(MobileBy.AndroidUIAutomator(UIAutomatorSelector));
                return locators;
            }

            if (AndroidDataMatcher != null)
            {
                var matchers = driver.FindElements(MobileBy.AndroidDataMatcher(AndroidDataMatcher));
                return matchers;
            }

            // Note: AndroidViewTag is not available in all Appium versions
            // Removed until confirmed support in current Appium.WebDriver version

            if (ImageLocator != null)
            {
                var images = driver.FindElements(MobileBy.Image(ImageLocator));
                return images;
            }

            //Progressive filtering with multiple properties

            if (this.Id != null)
            {
                elements = driver.FindElements(MobileBy.Id(this.Id));
            }

            if (this.AccessbilityId != null)
            {
                var aids = driver.FindElements(MobileBy.AccessibilityId(this.AccessbilityId));
                elements = IntersectElements(elements, aids);
            }

            if (this.ContentDescription != null)
            {
                if (driver is AndroidDriver ad)
                {
                    var cds = ad.FindElements(MobileBy.AccessibilityId(this.ContentDescription));
                    elements = IntersectElements(elements, cds);
                }
                else if (driver is IOSDriver)
                {
                    var cds = driver.FindElements(MobileBy.Name(this.ContentDescription));
                    elements = IntersectElements(elements, cds);
                }
            }
            else if (this.Name != null)
            {
                var names = driver.FindElements(MobileBy.Name(this.Name));
                elements = IntersectElements(elements, names);
            }

            if (this.ClassName != null)
            {
                var classes = driver.FindElements(MobileBy.ClassName(ClassName));
                elements = IntersectElements(elements, classes);
            }

            if (this.XPath != null)
            {
                var xpaths = driver.FindElements(MobileBy.XPath(this.XPath));
                elements = IntersectElements(elements, xpaths);
            }

            if (this.Text != null)
            {
                if (elements != null)
                {
                    // Client-side filtering when we already have elements
                    elements = elements.Where(x => x.Text == Text);
                }
                else
                {
                    // Try to find by LinkText (works for some mobile elements)
                    elements = driver.FindElements(MobileBy.LinkText(Text));
                }
            }

            if (elements == null || elements.Count() == 0)
            {
                throw new NoSuchElementException("No such WebElement");
            }
            else
            {
                IReadOnlyCollection<IWebElement> e = new List<IWebElement>(elements);
                return e;
            }
        }

        /// <inheritdoc />
        protected override byte[] InternalGetScreenshot()
        {
            var a = this.FindElement();
            if (a is AppiumElement ae)
            {
                return ae.GetScreenshot().AsByteArray;
            }
            else
            {
                throw new ElementNotInteractableException($"Cannot covert IWebElement to AppiumElement for screenshot. {this}");
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
                case How.ClassName:
                    if (ClassName == null) ClassName = attr.Using;
                    break;
                case How.LinkText:
                case How.PartialLinkText:
                    if (Text == null) Text = attr.Using;
                    break;
                case How.XPath:
                    if (XPath == null) XPath = attr.Using;
                    break;
                case How.Custom:
                default:
                    throw new NotSupportedException("FindsByAttribute does not support Custom yet.");
            }
        }

        /// <summary>
        /// Efficiently intersects two element collections using HashSet (O(n) instead of O(n²))
        /// </summary>
        private IEnumerable<IWebElement> IntersectElements(IEnumerable<IWebElement> elements, IReadOnlyCollection<IWebElement> newElements)
        {
            if (elements == null)
            {
                return newElements;
            }
            else
            {
                // Use HashSet for O(n) intersection instead of O(n²) Contains
                var elementSet = new HashSet<IWebElement>(newElements);
                return elements.Where(x => elementSet.Contains(x));
            }
        }

        /// <summary>
        /// Escapes single quotes in XPath string literals to prevent injection
        /// </summary>
        private string EscapeXPathString(string value)
        {
            if (value == null) return null;
            
            // If no single quotes, return as-is
            if (!value.Contains("'"))
            {
                return value;
            }
            
            // XPath doesn't have escape sequences, so replace with HTML entity
            return value.Replace("'", "&#39;");
        }
    }
}
