// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using AxaFrance.WebEngine;
using AxaFrance.WebEngine.Report;
using AxaFrance.WebEngine.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace WebEngine.Test.UnitTests
{
    [TestClass]
    public class CoreTest
    {
        [TestMethod]
        public void LoadEnvironmentVariables()
        {
            EnvironmentVariables.LoadFrom("TestData\\ENV.XML");
            var value1 = EnvironmentVariables.Current.GetValue("NOM");
            Assert.AreEqual(value1, "VALEUR");
            try
            {
                EnvironmentVariables.Current.GetValue("NOT_EXISTS");
            }
            catch (WebEngineGeneralException)
            {
                return;
            }
            throw new Exception("Test Failed, WebEngineGeneralException not thrown");
        }

        [TestMethod]
        public void LoadSettings()
        {
            var s = Settings.Instance;
            Assert.IsTrue(s.Capabilities != null);
        }

        [TestMethod]
        public void LoadTestSuiteData()
        {
            string filename = TestSuiteData.LoadFrom("TestData\\TestData.xml");
            Assert.AreEqual(filename, "TestData.xml");
            var value = TestSuiteData.Current.GetValue("Addition1", "NUMBER_1");
            Assert.AreEqual(value, "2");
            var value2 = TestSuiteData.Current.GetValue("Addition2", "ENVIRONNEMENT");
            Assert.AreEqual(value2, "UK");
        }

        [TestMethod]
        public void WebDriver_Chrome_Incognito()
        {
            using (var driver = BrowserFactory.GetDriver(Platform.Windows, BrowserType.Chrome, new string[] { "incognito" }))
            {
                driver.Navigate().GoToUrl("https://webengine-test.azurewebsites.net/");
                driver.Quit();
            }
        }

        [TestMethod]
        public void WebDriver_Edge_Incognito()
        {
            using (var driver = BrowserFactory.GetDriver(Platform.Windows, BrowserType.ChromiumEdge))
            {
                driver.Navigate().GoToUrl("https://webengine-test.azurewebsites.net/");
                driver.Quit();
            }
        }

        [TestMethod]
        public void LoadEnvironment()
        {
            EnvironmentVariables.LoadFrom("TestData\\ENV.xml");
            Assert.IsTrue(EnvironmentVariables.Current.GetValue("URL_PREPROD") == "https://www.test.co.uk");
        }

        [TestMethod]
        public void ListManipulation()
        {
            List<Variable> list = new List<Variable>();
            list.AddItem("Name", "Value");
            Assert.IsTrue(list.Item("Name") == "Value");
            list.AddItem(new Variable("Name", "Value2"));
            Assert.IsTrue(list.Item("Name") == "Value2");
            Assert.IsTrue(list.Count == 1);
        }


    }
}