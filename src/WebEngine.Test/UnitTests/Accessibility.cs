using AxaFrance.WebEngine.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Color = SixLabors.ImageSharp.Color;
using Point = System.Drawing.Point;
using Rectangle = SixLabors.ImageSharp.Rectangle;
using Size = System.Drawing.Size;

namespace WebEngine.Test.UnitTests
{

    [TestClass]
    [TestCategory("Mobile")]
    public class AccessibilityUnitTest
    {

        [TestMethod]
        public void RunAnalysis()
        {
            var driver = BrowserFactory.GetDriver(AxaFrance.WebEngine.Platform.Windows, AxaFrance.WebEngine.BrowserType.ChromiumEdge);
            driver.Navigate().GoToUrl("https://www.axa.fr/");
            var result = AccessibilityHelper.Analyze(driver);
            Debug.WriteLine(result.Violations.Length);
            var rootPath = "C:\\temp\\a11y";
            //count the total number of nodes in the violations with critical impact
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<html>");
            sb.AppendLine("<head><title>Accessibility Report</title>");
            sb.AppendLine("<style>");
            sb.AppendLine("body { font-family: Arial; }");
            sb.AppendLine("table { border-collapse: collapse; }");
            sb.AppendLine("table, th, td { border: 1px solid black; }");
            sb.AppendLine(".label { background-color: gray; color: white; padding: 5px; }");
            sb.AppendLine(".serious { background-color: #FF4500; color: white; padding: 5px; }");
            sb.AppendLine(".critical { background-color: #8B0000; color: white; padding: 5px; }");
            sb.AppendLine(".moderate { background-color: #FF8C00; color: white; padding: 5px; }");
            sb.AppendLine("th, td { padding: 5px; }");
            sb.AppendLine("pre {\r\n    white-space: pre-wrap;       /* Since CSS 2.1 */\r\n    white-space: -moz-pre-wrap;  /* Mozilla, since 1999 */\r\n    white-space: -pre-wrap;      /* Opera 4-6 */\r\n    white-space: -o-pre-wrap;    /* Opera 7 */\r\n    word-wrap: break-word;       /* Internet Explorer 5.5+ */\r\n}");
            sb.AppendLine("img { max-width: 100%; }");
            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("<h1>Accessibility Report: Rule Violations</h1>");
            foreach (var violation in result.Violations)
            {
                sb.AppendLine("<h2> Accessibility Rule: <span class='label'>" + violation.Id + "</span></h2>");
                sb.AppendLine("<ul>");
                sb.AppendLine($"<li>Impact: <span class='{violation.Impact}'>{violation.Impact}</span></li>");
                sb.AppendLine("<li>Description: " + HttpUtility.HtmlEncode(violation.Description) + "</li>");
                sb.AppendLine("<li>Help: " + HttpUtility.HtmlEncode(violation.Help) + "</li>");
                sb.AppendLine($"<li>HelpUrl: <a href={violation.HelpUrl}>{violation.HelpUrl}</a></li>");
                sb.AppendLine("</ul>");
                sb.AppendLine("<h2> Nodes violdated rules: " + violation.Nodes.Length + "</h2>");
                sb.AppendLine("<table><tr><th>Element</th><th>Screenshot</th></tr>");
                foreach(var node in violation.Nodes)
                {                   
                    sb.AppendLine("<tr><td>");
                    sb.AppendLine("<div>");
                    var cssSelector = node.Target;
                    var xpath = node.XPath;
                    WebElementDescription? nodeElement;
                    

                    // get nodeElement from cssSelector or xpath
                    if (cssSelector?.Selector is not null)
                    {
                        nodeElement = new WebElementDescription(driver)
                        {
                            CssSelector = cssSelector.Selector
                        };
                        sb.AppendLine("<b>CssSelector:</b> " + cssSelector.Selector);
                    }
                    else if (xpath?.Selector != null)
                    {
                        nodeElement = new WebElementDescription(driver)
                        {
                            XPath = xpath.Selector
                        };
                        sb.AppendLine("<b>XPath</b>: " + xpath.Selector);
                    }
                    else
                    {
                        continue;
                    }
                    sb.AppendLine("</div>");
                    sb.AppendLine("<pre>" + HttpUtility.HtmlEncode(node.Html) + "</pre>");
                    sb.AppendLine("</td><td>");
                    if (nodeElement != null)
                    {
                        Guid id = Guid.NewGuid();
                        var filename = id.ToString() + ".png";                        
                        nodeElement.ScrollIntoView();
                        var element = nodeElement.FindElement();
                        var locatable = (ILocatable)element;
                        var location = locatable.Coordinates.LocationInViewport;
                        var size = element.Size;
                        var screenshot = driver.GetScreenshot();
                        var content = screenshot.AsByteArray;
                        var newContent = MarkOnImage(content, location,size);
                        if (newContent is not null)
                        {
                            File.WriteAllBytes(Path.Combine(rootPath, filename), newContent);
                            sb.AppendLine("<img src=\"" + filename + "\" />");
                        }
                    }
                    sb.AppendLine("</td></tr>");

                }
                sb.AppendLine("</table>");
            }
            
            
            sb.AppendLine("</body></html>");
            File.WriteAllText(Path.Combine(rootPath, "a11y.html"), sb.ToString()); 
            Debug.WriteLine("All violations: " + result.Violations.Length);
            driver.Close();
        }

        private byte[] MarkOnImage(byte[] content, Point location, Size size)
        {
            var image = SixLabors.ImageSharp.Image.Load(content);
            var rect = new Rectangle(location.X, location.Y, size.Width, size.Height);
            image.Mutate(x => x.Draw(Color.Red, 2, rect));
            using var stream = new MemoryStream();
            image.Save(stream, new PngEncoder());
            stream.Flush();
            return stream.ToArray();
        }
    }
}
