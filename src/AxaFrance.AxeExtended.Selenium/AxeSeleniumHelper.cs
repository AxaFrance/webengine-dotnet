using AxaFrance.AxeExtended.HtmlReport;
using Deque.AxeCore.Commons;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;

namespace AxaFrance.AxeExtended.Selenium
{

    /// <summary>
    /// Helper class to implement analyze and screenshot report with Selenium WebDriver.
    /// </summary>
    public static class SeleniumHelper
    {
        /// <summary>
        /// Initializes the builder context with a Selenium WebDriver.
        /// WebDriver is used to take screenshots and other necessary manipulations to enrich report content.
        /// </summary>
        /// <param name="builder">HtmlReportBuilder object</param>
        /// <param name="driver">WebDriver object</param>
        /// <returns>The HtmlReportBuilder</returns>
        public static PageReportBuilder WithSelenium(this PageReportBuilder builder, WebDriver driver)
        {
            builder.GetScreenshot = (node, options) => ScreenShot(driver, node, options);
            builder.Analyze = (JObject config) => Analyze(builder, config, driver);
            return builder;
        }


        /// <summary>
        /// Initializes the builder context with Selenium WebDriver.
        /// </summary>
        /// <param name="builder">Overall report builder</param>
        /// <param name="driver">Selenium WebDriver</param>
        /// <param name="title">Title of you application report</param>
        /// <returns>PageReportBuilder object to analyze the current page.</returns>
        public static PageReportBuilder WithSelenium(this OverallReportBuilder builder, WebDriver driver, string title)
        {
            var option = builder.Options.Clone();
            if (title != null)
            {
                option.Title = title;
            }
            else
            {
                option.Title = driver.Title;
            }
            PageReportBuilder pageReportBuilder = new PageReportBuilder()
                .WithOptions(option)
                .WithSelenium(driver);
            builder.PageBuilders.Add(pageReportBuilder);
            return pageReportBuilder;
        }


        private static AxeResult Analyze(PageReportBuilder builder, JObject config, WebDriver driver)
        {
            AxeExtendedBuilder axeBuilder = new AxeExtendedBuilder(driver);
            if (builder.Options.Tags.Any())
            {
                axeBuilder.WithTags(builder.Options.Tags.ToArray());
            }
            axeBuilder.WithAxeConfig(config);
            var result = axeBuilder.Analyze();
            return result;
        }

        private static bool HasShadowRoot(IWebDriver driver, IWebElement element)
        {
            var shadowRoot = driver.ExecuteScript("return arguments[0].shadowRoot", element);
            return shadowRoot != null;
        }

        private static IEnumerable<IWebElement> FindElementsFromShadow(WebDriver driver, IEnumerable<string> locators, ISearchContext context = null)
        {
            if (context == null) context = driver;
            var currentLocator = locators.FirstOrDefault();
            var remainingLocators = locators.Skip(1).ToArray();
            IEnumerable<IWebElement> elements = null;
            if (context is ShadowRoot || context is IWebDriver)
            {
                elements = context.FindElements(By.CssSelector(currentLocator));
            }
            else if (context is WebElement we)
            {
                //find elements inside, or find elements from Shadowroot.
                
                if (HasShadowRoot(driver,we))
                {
                    var root = we.GetShadowRoot();
                    elements = root.FindElements(By.CssSelector(currentLocator));
                }
                else
                {
                    elements = context.FindElements(By.CssSelector(currentLocator));
                }
            }

            if (elements == null || elements.Count() == 0)
            {
                return null;
            }
            else if (remainingLocators.Count() == 0)
            {
                return elements;
            }
            else
            {
                foreach (var element in elements)
                {
                    var subElements = FindElementsFromShadow(driver, remainingLocators, element);
                    if (subElements != null)
                    {
                        return subElements;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Takes the screenshot. this function will be called by <see cref="PageReportBuilder.GetScreenshot" />
        /// </summary>
        /// <param name="driver">WebDriver</param>
        /// <param name="node">Node in AxeResult</param>
        /// <param name="options"><see cref="PageReportOptions"/> providing options for screenshot.</param>
        /// <returns></returns>
        private static byte[] ScreenShot(WebDriver driver, AxeResultNode node, PageReportOptions options)
        {
            var selectors = node.Target.FrameShadowSelectors;
            IWebElement element = null;
            if (selectors.Count > 1)
            {
                if (selectors.Any((IList<string> shadowSelectors) => shadowSelectors.Count > 1))
                {
                    //it contains shadow dom in iFrame
                    Console.WriteLine("[Ally] Elements nested in Shadow DOM within iFrame are not yet supported.");
                }
                else
                {
                    //it contains iFrame
                    Console.WriteLine("[Ally] Elements nested in iFrame are not yet supported.");
                    var iFrameSelector = node.Target.FrameSelectors.ToArray();
                    element = driver.FindElement(By.CssSelector(iFrameSelector[0]));
                }

            }
            else
            {
                var selector = node.Target.FrameShadowSelectors[0];
                if (selector.Count > 1)
                {
                    var elements = FindElementsFromShadow(driver, selector);
                    if (elements != null)
                    {
                        element = elements.FirstOrDefault();
                    }
                }
                else
                {
                    element = GetDomElement(driver, node);
                }
            }

            if (element is WebElement we)
            {
                try
                {
                    var screenshot = options.UseAdvancedScreenshot ? AdvancedScreenshot(driver, we, options) : ToWebpByteArray(we.GetScreenshot());
                    driver.SwitchTo().DefaultContent(); //goes back to default context (leaving iframes)
                    return screenshot;
                }
                catch (Exception ex)
                {
                    //in some cases (hidden element, 0 height element, etc. the screeshot is not possible, leave these cases behind.
                    Console.WriteLine("[A11y] Unable to get screenshot:" + ex.ToString());
                    return PageReportBuilder.GetRawResources("no-image.png");
                }
            }
            else
            {
                driver.SwitchTo().DefaultContent();
                Console.WriteLine("[A11y] The element can not be converted to type WebElement for screenshot.");
                return PageReportBuilder.GetRawResources("no-image.png");
            }


        }

        private static IWebElement GetDomElement(WebDriver driver, AxeResultNode node)
        {
            IWebElement element = null;
            var cssSelector = node.Target?.Selector;
            var xPath = node.XPath?.Selector;

            //find given element
            try
            {
                element = driver.FindElement(By.CssSelector(cssSelector));
                if (element is null)
                {
                    element = driver.FindElement(By.XPath(xPath));
                }
            }
            catch (Exception ex)
            {
                //sometimes the cssSelector provided by axe can not be used by selenium.
                //in this case can't make screenshot on the element.
                Console.WriteLine("[A11y] Unable to get element from cssSelector or xPath");
            }
            return element;
        }

        /// <summary>
        /// Convert selenium screenshot in png format to webp format.
        /// </summary>
        /// <param name="screenshot">Selenium screenshot object.</param>
        /// <returns>Byte array of an image in webp format</returns>
        private static byte[] ToWebpByteArray(Screenshot screenshot)
        {
            //covert screenshot in png format to webp
            using (SKBitmap bitmap = SKBitmap.Decode(screenshot.AsByteArray))
            {
                using (SKImage img = SKImage.FromBitmap(bitmap))
                {
                    using (SKData data = img.Encode(SKEncodedImageFormat.Webp, 80))
                    {
                        return data.ToArray();
                    }
                }
            }

        }

        private static byte[] AdvancedScreenshot(WebDriver driver, WebElement element, PageReportOptions options)
        {
            BringToView(element, driver);
            var imageViewPort = driver.GetScreenshot();
            var locatable = (ILocatable)element;

            var location = locatable.Coordinates.LocationInViewport; //location and size are for 100% dpi
            var size = element.Size;
            var width = size.Width == 0 ? 10 : size.Width;
            var height = size.Height == 0 ? 10 : size.Height;
            var newSize = new Size(width, height);
            var screenshot = MarkOnImage(imageViewPort, location, newSize, options);
            return screenshot;
        }

        /// <summary>
        /// Bring the element to viewport so that it can be captured by screenshot.
        /// </summary>
        /// <param name="element">The element should be taken into viewport</param>
        /// <param name="driver">WebDriver instance</param>
        private static void BringToView(WebElement element, WebDriver driver)
        {
            driver.ExecuteScript("arguments[0].scrollIntoView(true);", element);
        }

        //When using advanced screenshot, Use SkiaSharp to draw the element in question on the screenshot of containing view port.
        private static byte[] MarkOnImage(Screenshot imageViewPort, Point location, Size size, PageReportOptions options)
        {
            SKColor color = new SKColor(options.HighlightColor.R, options.HighlightColor.G, options.HighlightColor.B);
            using (SKBitmap bitmap = SKBitmap.Decode(imageViewPort.AsByteArray))
            {
                using (SKCanvas canvas = new SKCanvas(bitmap))
                {
                    canvas.DrawRect(location.X, location.Y,
                        size.Width, size.Height,
                        new SKPaint()
                        {
                            Color = color,
                            Style = SKPaintStyle.Stroke,
                            StrokeWidth = options.HighlightThickness
                        });
                    using (var data = bitmap.Encode(SKEncodedImageFormat.Webp, 80))
                    {
                        return data.ToArray();
                    }
                }
            }
        }
    }
}