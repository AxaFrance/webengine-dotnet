using Axe.Extended.HtmlReport;
using Deque.AxeCore.Commons;
using Deque.AxeCore.Selenium;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Xml.Linq;

namespace Axe.Extended.Selenium
{
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


        /// <summary>
        /// Takes the screenshot. this function will be called by <see cref="PageReportBuilder.GetScreenshot" delegation/>
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
                    //it contains shadow dom iFrame
                    throw new NotImplementedException("Shadow Dom element not yet implemented.");
                }
                else
                {
                    //it contains iFrame
                    var iFrameSelector = node.Target.FrameSelectors.ToArray();

                    //TODO: Unable to locate and screenshot elements inside frames.
                    //At current state, 
                    element = driver.FindElement(By.CssSelector(iFrameSelector[0]));
                    /*
                    foreach(var s in iFrameSelector)
                    {
                        element = driver.FindElement(By.CssSelector(s));
                        if(element.TagName == "iframe")
                        {
                            driver.SwitchTo().Frame(element);
                        }
                        else
                        {
                            element = driver.FindElement(By.CssSelector(s));
                        }
                    }*/
                }

            }
            else
            {
                var cssSelector = node.Target?.Selector;
                var xPath = node.XPath?.Selector;

                //find given element
                try
                {
                    element = driver.FindElement(By.CssSelector(cssSelector));
                    if (element == null)
                    {
                        element = driver.FindElement(By.XPath(xPath));
                    }
                }catch(Exception ex)
                {
                    Console.WriteLine("[Ally] Unable to get element from cssSelector or xPath");
                    //sometimes the cssSelector provided by axe can not be used by selenium.
                    //in this case can't make screenshot on the element.
                }
            }

            if (element != null && element is WebElement we)
            {
                try
                {
                    var screenshot = options.UseAdvancedScreenshot ? AdvancedScreenshot(driver, we, options) : we.GetScreenshot().AsByteArray;
                    driver.SwitchTo().DefaultContent(); //goes back to default context (leaving iframes)
                    return screenshot;
                }
                catch (Exception ex)
                {
                    //in some cases (hidden element, 0 height element, etc. the screeshot is not possible, leave these cases behind.
                    Console.WriteLine("[Ally] Unable to get screenshot:" + ex.ToString());
                    return new byte[0];
                }
            }
            else
            {
                driver.SwitchTo().DefaultContent();
                throw new WebDriverException("The element can not be converted to type WebElement for screenshot.");
            }


        }

        private static byte[] AdvancedScreenshot(WebDriver driver, WebElement element, PageReportOptions options)
        {
            BringToView(element, driver);
            var imageViewPort = driver.GetScreenshot();
            var windowSize = driver.Manage().Window.Size;
            var locatable = (ILocatable)element;

            var location = locatable.Coordinates.LocationInViewport; //location and size are for 100% dpi
            var size = element.Size;
            if (size.Height != 0 && size.Width != 0)
            {
                var screenshot = MarkOnImage(imageViewPort, location, size, options);
                return screenshot;
            }
            Console.WriteLine("[Ally] Unable to get screenshot: Element has 0 height or width.");
            return new byte[0];
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
                    using (var data = bitmap.Encode(SKEncodedImageFormat.Png, 100))
                    {
                        return data.ToArray();
                    }
                }
            }
        }
    }
}