﻿using AxaFrance.AxeExtended.HtmlReport;
using AxaFrance.AxeExtended.Selenium;
using AxaFrance.WebEngine;
using AxaFrance.WebEngine.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.Extensions;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebEngine.Test.UnitTests
{

    [TestClass]
    public class AccessibilityUnitTest
    {

        [TestMethod]
        public void AuditAndExportHtml()
        {
            //Execute mon test automatisé
            using (var driver = BrowserFactory.GetDriver(AxaFrance.WebEngine.Platform.Windows, BrowserType.Chrome))
            {
                driver.Navigate().GoToUrl("https://www.axa.fr");
                try
                {
                    driver.FindElement(By.Id("footer_tc_privacy_button")).Click();
                }
                catch
                {
                    //if the cookie button does not exist, it's not a problem
                }

                var sw = Stopwatch.StartNew();

                //Effectuer une analyse d'accessibilité
                var filename = new PageReportBuilder()
                    .WithSelenium(driver).Build().Export();
                Debug.WriteLine($"Report generated in {sw.Elapsed.TotalSeconds} seconds");
                Debug.WriteLine($"Report generated at {filename}");
                Assert.IsTrue(File.Exists(filename));
            }
        }

        [TestMethod]
        public void AuditWithOptionsAndExportHtml()
        {
            //Execute mon test automatisé
            using (var driver = BrowserFactory.GetDriver(AxaFrance.WebEngine.Platform.Windows, BrowserType.ChromiumEdge))
            {
                driver.Navigate().GoToUrl("https://www.axa.fr");
                try
                {
                    driver.FindElement(By.Id("footer_tc_privacy_button")).Click();
                }
                catch
                {
                    //if the cookie button does not exist, it's not a problem
                }

                var sw = Stopwatch.StartNew();

                //Effectuer une analyse d'accessibilité
                var filename = new PageReportBuilder()
                    .WithOptions(new PageReportOptions()
                    {
                        HighlightColor = Color.Green,
                        HighlightThickness = 5,
                        ScoringMode = ScoringMode.Weighted,
                        Tags = PageReportOptions.WcagAATags,
                        ScreenshotIncomplete = true,
                        UseAdvancedScreenshot = true,
                        ReportLanguage = Language.Spanish,
                        Title = "Homepage AXA.FR"
                    })
                    .WithSelenium(driver).Build().Export();
                Debug.WriteLine($"Report generated in {sw.Elapsed.TotalSeconds} seconds");
                Debug.WriteLine($"Report generated at {filename}");
                Assert.IsTrue(File.Exists(filename));
            }
        }

        [TestMethod]
        public void AuditWithRGAAAndExportHtml()
        {
            //Execute mon test automatisé
            using (var driver = BrowserFactory.GetDriver(AxaFrance.WebEngine.Platform.Windows, BrowserType.ChromiumEdge))
            {
                driver.Navigate().GoToUrl("https://www.axa.fr");
                try
                {
                    driver.FindElement(By.Id("footer_tc_privacy_button")).Click();
                }
                catch
                {
                    //if the cookie button does not exist, it's not a problem
                }

                var sw = Stopwatch.StartNew();

                var filename = new PageReportBuilder()
                    .WithOptions(new PageReportOptions() { ReportLanguage = Language.French })
                    .WithRgaaExtension()
                    .WithSelenium(driver).Build().Export();
                Debug.WriteLine($"Report generated in {sw.Elapsed.TotalSeconds} seconds");
                Debug.WriteLine($"Report {filename}");
                Assert.IsTrue(File.Exists(filename));
            }

        }

        [TestMethod]
        public void UserJourneyTest()
        {
            using (var driver = BrowserFactory.GetDriver(AxaFrance.WebEngine.Platform.Windows, BrowserType.ChromiumEdge))
            {
                var defaultOptions = new PageReportOptions()
                {
                    Title = "AXA.FR",
                    OutputFormat = OutputFormat.Zip,
                    ReportLanguage = Language.French,
                    AdditionalTags = new RgaaTagsProvider(),
                };
                var builder = new OverallReportBuilder().WithDefaultOptions(defaultOptions);

                //Analyze first page
                driver.Navigate().GoToUrl("https://www.axa.fr/");
                try
                {
                    driver.FindElement(By.Id("footer_tc_privacy_button")).Click();
                }
                catch
                {
                    //if the cookie button does not exist, it's not a problem
                }
                builder.WithSelenium(driver, "Main Page").WithRgaaExtension().Build();

                //Analyze second page
                driver.Navigate().GoToUrl("https://www.axa.fr/pro.html");
                try
                {
                    driver.FindElement(By.Id("footer_tc_privacy_button")).Click();
                }
                catch
                {
                    //if the cookie button does not exist, it's not a problem
                }
                builder.WithSelenium(driver, "Pro").WithRgaaExtension().Build();

                //Analyze third page
                driver.Navigate().GoToUrl("https://www.axa.fr/pro/services-assistance.html");
                builder.WithSelenium(driver, "Assistance").WithRgaaExtension().Build();

                //Demarche Inondation
                driver.Navigate().GoToUrl("https://www.axa.fr/assurance-habitation/demarches-inondation.html");
                builder.WithSelenium(driver, "Inondation").WithRgaaExtension().Build();

                driver.Navigate().GoToUrl("https://www.axa.fr/pro/devis-assurance-professionnelle.html");
                builder.WithSelenium(driver, "Devis Pro").WithRgaaExtension().Build();

                driver.Navigate().GoToUrl("https://www.axa.fr/compte-bancaire.html");
                builder.WithSelenium(driver, "Compte Bancaire").WithRgaaExtension().Build();

                string report = builder.Build().Export();
                Assert.IsTrue(File.Exists(report));
                Console.WriteLine(report);
            }
        }

    }
}
