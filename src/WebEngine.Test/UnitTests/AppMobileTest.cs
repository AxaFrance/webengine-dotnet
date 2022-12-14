// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using AxaFrance.WebEngine;
using AxaFrance.WebEngine.MobileApp;
using AxaFrance.WebEngine.Report;
using AxaFrance.WebEngine.Web;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebEngine.Test.UnitTests
{
    [TestClass]
    [TestCategory("Mobile")]
    public class AppMobileTest
    {

        [TestMethod]
        public void IntegrationTest()
        {
            EnvironmentVariables.LoadFrom("TestData\\ENV.xml");
            TestSuiteData.LoadFrom("TestData\\TestData.xml");

            TestSuiteApp app = new TestSuiteApp();
            Settings settings = Settings.Instance;
            settings.AppId = "TestData\\ApiDemos-debug.apk";
            settings.Platform = Platform.Android;
            settings.Browser = BrowserType.AndroidNative;
            settings.Device = "Android API 30";
            var testsuite = settings.TestSuite = app;
            testsuite.Initialize(settings);
            var result = testsuite.Run();
            testsuite.CleanUp(settings);
            var path = System.IO.Path.GetTempPath();
            var XmlReport = result.SaveAs(path, "testResult", true);
            string junitReport = ReportHelper.GenerateJUnitReport(result, "mytest", path);

            Assert.IsTrue(result.Duration.TotalSeconds > 0);
            Assert.IsTrue(result.TestResult[0].Result == Result.Failed);
            Assert.IsTrue(result.TestResult[0].ActionReports[0].Result == Result.Passed);
            Assert.IsTrue(result.TestResult[0].ActionReports[1].Result == Result.Failed);
            Assert.IsTrue(result.TestResult[0].ActionReports[2].Result == Result.Failed);
            Assert.IsTrue(System.IO.File.Exists(XmlReport));
            Assert.IsTrue(System.IO.File.Exists(junitReport));
            System.IO.File.Delete(XmlReport);
            System.IO.File.Delete(junitReport);
        }

        [TestMethod]
        public void UnitTest()
        {
            var settings = Settings.Instance;
            settings.GridServerUrl = "http://localhost:4723/wd/hub";
            settings.AppId = "TestData\\ApiDemos-debug.apk";
            var driver = AppFactory.GetDriver(Platform.Android);

            MyModel m = new MyModel(driver);
            m.menuViews.Click();
            m.menuButtons.Click();
            m.normalButton.Click();
            m.smallButton.Click();
            m.toggleButton.Click();
        }

        [TestMethod]
        public void NoValidPlatform()
        {
            try
            {
                var settings = Settings.Instance;
                settings.GridServerUrl = "http://localhost:4723/wd/hub";
                settings.AppId = "TestData\\ApiDemos-debug.apk";
                var driver = AppFactory.GetDriver(Platform.Windows);
            }
            catch (ArgumentOutOfRangeException)
            {
                return;
            }
            Assert.Fail();
        }

        [TestMethod]
        public void TestApkOnIosError()
        {
            try
            {
                var settings = Settings.Instance;
                settings.GridServerUrl = "http://localhost:4723/wd/hub";
                settings.AppId = "TestData\\ApiDemos-debug.apk";
                settings.Device = "iPhone 13";
                var driver = AppFactory.GetDriver(Platform.iOS);
            }
            catch (OpenQA.Selenium.WebDriverException)
            {
                return;
            }
            Assert.Fail();
        }


    }
}
