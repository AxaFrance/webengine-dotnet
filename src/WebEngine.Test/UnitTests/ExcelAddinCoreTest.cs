// Copyright (c) 2016-2026 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AxaFrance.WebEngine;
using AxaFrance.WebEngine.ExcelAddin;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace WebEngine.Test.UnitTests
{
    [TestClass]
    public class ExcelAddinCoreTest
    {
        [TestMethod]
        public void ReadTestDataSupportsASingleTestCase()
        {
            object[,] cells = CreateSheet(6, 3);
            cells[2, 0] = "TESTCASE";
            cells[2, 2] = "ONLY_ONE";
            cells[3, 0] = "COMMENT";
            cells[3, 2] = "Only test case";
            cells[4, 0] = "WORKFLOW";
            cells[4, 2] = "SIMPLE";

            var result = ExcelTestDataReader.ReadTestData(cells);

            Assert.AreEqual(1, result.TestData.Count);
            Assert.AreEqual("ONLY_ONE", result.TestData[0].TestName);
            Assert.AreEqual(3, result.TestData[0].Data.Length);
            Assert.AreEqual("ONLY_ONE", result.TestData[0].Data.Single(x => x.Name == "TESTCASE").Value);
            CollectionAssert.AreEqual(new[] { "ONLY_ONE" }, result.TestCaseNames.ToArray());
        }

        [TestMethod]
        public void ReadTestDataSupportsSelectedColumns()
        {
            object[,] cells = CreateSheet(4, 4);
            cells[2, 0] = "TESTCASE";
            cells[2, 2] = "TEST_A";
            cells[2, 3] = "TEST_B";
            cells[3, 0] = "WORKFLOW";
            cells[3, 2] = "SIMPLE";
            cells[3, 3] = "ADVANCED";

            var selected = ExcelTestDataReader.ReadTestData(cells, new[] { 4 });

            Assert.AreEqual(1, selected.TestData.Count);
            Assert.AreEqual("TEST_B", selected.TestData[0].TestName);
            Assert.AreEqual("ADVANCED", selected.TestData[0].Data.Single(x => x.Name == "WORKFLOW").Value);
        }

        [TestMethod]
        public void ReadTestDataDetectsDuplicates()
        {
            object[,] cells = CreateSheet(4, 4);
            cells[2, 0] = "TESTCASE";
            cells[2, 2] = "SAME_NAME";
            cells[2, 3] = "SAME_NAME";
            cells[3, 0] = "WORKFLOW";
            cells[3, 2] = "SIMPLE";
            cells[3, 3] = "ADVANCED";

            var result = ExcelTestDataReader.ReadTestData(cells);

            Assert.IsTrue(result.HasDuplicates);
            CollectionAssert.AreEqual(new[] { "SAME_NAME" }, result.DuplicateTestCaseNames.ToArray());
            Assert.AreEqual(1, result.TestData.Count);
        }

        [TestMethod]
        public void EnvironmentReaderSkipsTheOptionalHeader()
        {
            object[,] cells = CreateSheet(3, 3);
            cells[0, 0] = "NAME";
            cells[0, 1] = "VALUE";
            cells[1, 0] = "URL";
            cells[1, 1] = "https://example.test";
            cells[2, 0] = "TOKEN";
            cells[2, 1] = "abc";

            var variables = ExcelTestDataReader.ReadEnvironmentVariables(cells);

            Assert.AreEqual(2, variables.Count);
            Assert.AreEqual("URL", variables[0].Name);
            Assert.IsFalse(variables.Any(x => x.Name == "NAME"));
        }

        [TestMethod]
        public void EnvironmentReaderKeepsSheetsWithoutHeader()
        {
            object[,] cells = CreateSheet(1, 2);
            cells[0, 0] = "URL";
            cells[0, 1] = "https://example.test";

            var variables = ExcelTestDataReader.ReadEnvironmentVariables(cells);

            Assert.AreEqual(1, variables.Count);
            Assert.AreEqual("URL", variables[0].Name);
        }

        [TestMethod]
        public void ParameterDefinitionsIgnoreNamesWithSpaces()
        {
            object[,] cells = CreateSheet(3, 2);
            cells[0, 0] = "This title is ignored";
            cells[1, 0] = "TESTCASE";
            cells[1, 1] = "Test case name";
            cells[2, 0] = "INVALID NAME";
            cells[2, 1] = "Ignored because of the space";

            var definitions = ExcelTestDataReader.ReadParameterDefinitions(cells);

            Assert.AreEqual(1, definitions.Count);
            Assert.AreEqual("TESTCASE", definitions[0].Name);
        }

        [TestMethod]
        public void FilterIncludesAndExcludesMatchingNames()
        {
            var include = new TestCaseFilter("login;admin", exportMatches: true);
            var exclude = new TestCaseFilter("login;admin", exportMatches: false);

            Assert.IsTrue(include.IsMatch("TEST_LOGIN_01"));
            Assert.IsFalse(include.IsMatch("TEST_SEARCH_01"));
            Assert.IsFalse(exclude.IsMatch("TEST_LOGIN_01"));
            Assert.IsTrue(exclude.IsMatch("TEST_SEARCH_01"));
            Assert.IsTrue(new TestCaseFilter(string.Empty, exportMatches: true).IsMatch("ANY"));
        }

        [TestMethod]
        public void RunArgumentsContainAllRequestedOptions()
        {
            string arguments = RunArgumentsBuilder.BuildArguments(
                @"C:\Tests\MyTests.dll",
                @"C:\Export\TEST_SUITE.xml",
                @"C:\Export\ENV.xml",
                BrowserType.Chrome,
                platform: "Android",
                device: "Pixel 8",
                appId: "bs://app",
                manualMode: true,
                showReport: true);

            StringAssert.Contains(arguments, "-a:\"C:\\Tests\\MyTests.dll\"");
            StringAssert.Contains(arguments, "-data:\"C:\\Export\\TEST_SUITE.xml\"");
            StringAssert.Contains(arguments, "-env:\"C:\\Export\\ENV.xml\"");
            StringAssert.Contains(arguments, "-browser:\"Chrome\"");
            StringAssert.Contains(arguments, " -u ");
            StringAssert.Contains(arguments, "-platform:\"Android\"");
            StringAssert.Contains(arguments, "-device:\"Pixel 8\"");
            StringAssert.Contains(arguments, "-appid:\"bs://app\"");
            StringAssert.Contains(arguments, " -m");
            StringAssert.Contains(arguments, " -showreport");
        }

        [TestMethod]
        public void ResolveRunnerFindsTheDotNetRunner()
        {
            string folder = CreateTempFolder();
            try
            {
                File.WriteAllText(Path.Combine(folder, "WebRunner.exe"), string.Empty);

                var command = RunArgumentsBuilder.ResolveRunner(folder, "-a:test");

                Assert.AreEqual(Path.Combine(folder, "WebRunner.exe"), command.FileName);
                Assert.AreEqual(folder, command.WorkingDirectory);
            }
            finally
            {
                Directory.Delete(folder, recursive: true);
            }
        }

        [TestMethod]
        public void ResolveRunnerFindsTheBundledJavaRunner()
        {
            string folder = CreateTempFolder();
            try
            {
                File.WriteAllText(Path.Combine(folder, "WebRunner.jar"), string.Empty);
                Directory.CreateDirectory(Path.Combine(folder, "jre", "bin"));
                File.WriteAllText(Path.Combine(folder, "jre", "bin", "java.exe"), string.Empty);

                var command = RunArgumentsBuilder.ResolveRunner(folder, "-a:test");

                Assert.AreEqual(Path.Combine(folder, "jre", "bin", "java.exe"), command.FileName);
                StringAssert.Contains(command.Arguments, "-jar");
                StringAssert.Contains(command.Arguments, "WebRunner.jar");
            }
            finally
            {
                Directory.Delete(folder, recursive: true);
            }
        }

        [TestMethod]
        public void SettingsRoundTripPreservesValues()
        {
            string folder = CreateTempFolder();
            try
            {
                string path = Path.Combine(folder, "settings.xml");
                var store = new AddinSettingsStore(path);
                var settings = new AddinSettings
                {
                    ExportPath = @"C:\Export",
                    WebRunnerPath = @"C:\Runner",
                    TestAssembly = @"C:\Tests\Tests.dll",
                    Browser = BrowserType.Firefox,
                    Device = "Pixel",
                    AppId = "bs://app",
                    ExcludedTabs = new[] { "NOTE", "ENV" },
                };

                store.Save(settings);
                var loaded = store.Load();

                Assert.AreEqual(settings.ExportPath, loaded.ExportPath);
                Assert.AreEqual(settings.WebRunnerPath, loaded.WebRunnerPath);
                Assert.AreEqual(settings.TestAssembly, loaded.TestAssembly);
                Assert.AreEqual(settings.Browser, loaded.Browser);
                Assert.AreEqual(settings.Device, loaded.Device);
                Assert.AreEqual(settings.AppId, loaded.AppId);
                CollectionAssert.AreEqual(settings.ExcludedTabs, loaded.ExcludedTabs);
            }
            finally
            {
                Directory.Delete(folder, recursive: true);
            }
        }

        [TestMethod]
        public void SettingsCloneIsIndependentAndExcludedTabsAreCaseInsensitive()
        {
            var settings = new AddinSettings();
            settings.ExcludedTabs[0] = "NOTE";

            var clone = settings.Clone();
            clone.ExcludedTabs[0] = "CHANGED";

            Assert.AreEqual("NOTE", settings.ExcludedTabs[0]);
            Assert.IsTrue(settings.IsExcludedSheet("env"));
            Assert.IsTrue(settings.IsExcludedSheet("Params"));
            Assert.IsFalse(settings.IsExcludedSheet("TEST_SUITE"));
        }

        [TestMethod]
        public void CodeGenerationProducesCompilableLookingConstants()
        {
            var parameters = new[]
            {
                new ParameterDefinition("TESTCASE", "Test case name"),
                new ParameterDefinition("WORKFLOW", string.Empty),
            };

            string csharp = CodeGenerator.GenerateCSharp(parameters);
            string java = CodeGenerator.GenerateJava(parameters);

            StringAssert.Contains(csharp, "public static class ParameterList");
            StringAssert.Contains(csharp, "public static string TESTCASE {get; } = \"TESTCASE\";");
            StringAssert.Contains(java, "public class ParameterList");
            StringAssert.Contains(java, "public static final String WORKFLOW = \"WORKFLOW\";");
        }

        private static object[,] CreateSheet(int rows, int columns)
        {
            return new object[rows, columns];
        }

        private static string CreateTempFolder()
        {
            string folder = Path.Combine(Path.GetTempPath(), "webengine-excel-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(folder);
            return folder;
        }
    }
}
