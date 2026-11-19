// Copyright (c) 2016-2026 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using AxaFrance.WebEngine;
using ExcelDna.Integration.CustomUI;

namespace AxaFrance.WebEngine.ExcelAddin
{
    public partial class ExcelAddin
    {
        /// <summary>
        /// Exports the test cases in the selected columns.
        /// </summary>
        public void OnExportSelected(IRibbonControl control)
        {
            try
            {
                ExportSelected(showDialog: true);
            }
            catch (Exception ex)
            {
                ShowError("Export selection failed", ex);
            }
        }

        /// <summary>
        /// Exports all test cases of the active worksheet.
        /// </summary>
        public void OnExportSheet(IRibbonControl control)
        {
            try
            {
                ExportSheet(filter: null, showDialog: true);
            }
            catch (Exception ex)
            {
                ShowError("Export failed", ex);
            }
        }

        /// <summary>
        /// Exports a filtered set of test cases.
        /// </summary>
        public void OnExportFilter(IRibbonControl control)
        {
            try
            {
                using var form = new FrmFilter();
                if (form.ShowDialog() == DialogResult.OK)
                {
                    ExportSheet(new TestCaseFilter(form.FilterText, form.ExportMatches), showDialog: true);
                }
            }
            catch (Exception ex)
            {
                ShowError("Export with filter failed", ex);
            }
        }

        /// <summary>
        /// Exports the ENV worksheet.
        /// </summary>
        public void OnExportEnvironment(IRibbonControl control)
        {
            try
            {
                ExportEnvironment(showDialog: true);
            }
            catch (Exception ex)
            {
                ShowError("Environment variable export failed", ex);
            }
        }

        /// <summary>
        /// Checks parameters and test case names for consistency.
        /// </summary>
        public void OnCheckData(IRibbonControl control)
        {
            try
            {
                var dataSheet = ReadDataSheet();
                var parametersSheet = ReadParametersSheet();

                var parameters = ExcelTestDataReader.ReadParameterNames(parametersSheet.Cells);
                var testParameters = ExcelTestDataReader.ReadTestParameterNames(dataSheet.Cells);
                var testCases = ExcelTestDataReader.ReadTestCaseNames(dataSheet.Cells);

                using var form = new FrmCheckResult(parameters, testParameters, testCases);
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                ShowError("Test data check failed", ex);
            }
        }

        /// <summary>
        /// Inserts missing parameters from PARAMS into the active worksheet.
        /// </summary>
        public void OnSyncParameters(IRibbonControl control)
        {
            try
            {
                var dataSheet = ReadDataSheet();
                var parametersSheet = ReadParametersSheet();

                var parameterNames = ExcelTestDataReader
                    .ReadParameterDefinitions(parametersSheet.Cells)
                    .Select(x => x.Name)
                    .ToList();
                var existingNames = ExcelTestDataReader.ReadTestParameterNames(dataSheet.Cells);

                int missing = parameterNames.Except(existingNames, StringComparer.Ordinal).Count();
                if (missing == 0)
                {
                    ShowInfo("All parameters from PARAMS are already present in the active worksheet.");
                    return;
                }

                var answer = MessageBox.Show(
                    $"This will add {missing} missing parameter(s) to the active worksheet. This action cannot be reversed.{Environment.NewLine}Do you want to continue?",
                    "Synchronize parameters",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2);
                if (answer != DialogResult.Yes)
                {
                    return;
                }

                ExcelWorkbook.SyncParameterRows(parameterNames, existingNames);
                ShowInfo($"Synchronization complete. {missing} parameter(s) added.");
            }
            catch (Exception ex)
            {
                ShowError("Parameter synchronization failed", ex);
            }
        }

        /// <summary>
        /// Generates a C# parameter class and copies it to the clipboard.
        /// </summary>
        public void OnGenerateCSharp(IRibbonControl control)
        {
            GenerateCode(language: "C#");
        }

        /// <summary>
        /// Generates a Java parameter class and copies it to the clipboard.
        /// </summary>
        public void OnGenerateJava(IRibbonControl control)
        {
            GenerateCode(language: "Java");
        }

        private bool ExportSelected(bool showDialog)
        {
            ValidateExportSettings();
            var sheet = ReadDataSheet();
            var selectedColumns = ExcelWorkbook.GetSelectedColumns();
            if (selectedColumns.Count == 0)
            {
                throw new InvalidOperationException(
                    "Select at least one test case column (column C or later) before exporting.");
            }

            var result = ExcelTestDataReader.ReadTestData(sheet.Cells, selectedColumns);
            return SaveTestDataResult(result, sheet.Name, showDialog);
        }

        private bool ExportSheet(TestCaseFilter? filter, bool showDialog)
        {
            ValidateExportSettings();
            var sheet = ReadDataSheet();
            var result = ExcelTestDataReader.ReadTestData(
                sheet.Cells,
                selectedColumns: null,
                testCaseFilter: filter == null ? null : filter.IsMatch);

            return SaveTestDataResult(result, sheet.Name, showDialog);
        }

        private bool SaveTestDataResult(TestDataReadResult result, string sheetName, bool showDialog)
        {
            if (result.HasDuplicates)
            {
                throw new InvalidOperationException(
                    "Duplicated test cases must be fixed before export: " + string.Join(", ", result.DuplicateTestCaseNames));
            }

            if (result.TestData.Count == 0)
            {
                if (SilentExport)
                {
                    throw new InvalidOperationException("No test case was found in the current selection.");
                }

                ShowWarning("No test case was found in the current selection or filter.");
                return false;
            }

            string fileName = sheetName + ".xml";
            string? path = showDialog
                ? PromptSave(Settings.ExportPath, fileName, "Export test data")
                : Path.Combine(Settings.ExportPath, fileName);
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            var data = new TestSuiteData { TestDataList = result.TestData };
            SerializeToFile(data, path);

            TestDataFile = path;
            CategoryName = sheetName;
            TestCases = result.TestCaseNames;
            NotifyExport($"Test data exported to:{Environment.NewLine}{path}", path);
            return true;
        }

        private bool ExportEnvironment(bool showDialog)
        {
            ValidateExportSettings();

            SheetSnapshot sheet;
            try
            {
                sheet = ExcelWorkbook.ReadSheet("ENV");
            }
            catch (COMException ex)
            {
                throw new InvalidOperationException(
                    "The workbook must contain an ENV worksheet.", ex);
            }

            var variables = ExcelTestDataReader.ReadEnvironmentVariables(sheet.Cells);
            if (variables.Count == 0)
            {
                throw new InvalidOperationException("The ENV worksheet does not contain any variable.");
            }

            string? path = showDialog
                ? PromptSave(Settings.ExportPath, "ENV.xml", "Export environment variables")
                : Path.Combine(Settings.ExportPath, "ENV.xml");
            if (string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            SerializeToFile(new EnvironmentVariables { Variables = variables.ToList() }, path);

            EnvironmentVariableFile = path;
            NotifyExport($"Environment variables exported to:{Environment.NewLine}{path}", path);
            return true;
        }

        private void GenerateCode(string language)
        {
            try
            {
                var parametersSheet = ReadParametersSheet();
                var definitions = ExcelTestDataReader.ReadParameterDefinitions(parametersSheet.Cells);
                if (definitions.Count == 0)
                {
                    throw new InvalidOperationException("No parameter was found in the PARAMS worksheet.");
                }

                string code = language == "Java"
                    ? CodeGenerator.GenerateJava(definitions)
                    : CodeGenerator.GenerateCSharp(definitions);

                Clipboard.SetText(code);
                ShowInfo($"The {language} parameter class is ready and has been copied to the clipboard.");
            }
            catch (Exception ex)
            {
                ShowError("Code generation failed", ex);
            }
        }

        private SheetSnapshot ReadDataSheet()
        {
            var sheet = ExcelWorkbook.ReadActiveSheet();
            if (Settings.IsExcludedSheet(sheet.Name))
            {
                throw new InvalidOperationException(
                    $"'{sheet.Name}' is not a test data worksheet. Select a test data sheet first.");
            }

            return sheet;
        }

        private SheetSnapshot ReadParametersSheet()
        {
            try
            {
                return ExcelWorkbook.ReadSheet("PARAMS");
            }
            catch (COMException ex)
            {
                throw new InvalidOperationException(
                    "The workbook must contain a PARAMS worksheet.", ex);
            }
        }

        private void ValidateExportSettings()
        {
            if (string.IsNullOrWhiteSpace(Settings.ExportPath))
            {
                throw new InvalidOperationException(
                    "The export folder is not configured. Open Settings first.");
            }

            Directory.CreateDirectory(Settings.ExportPath);
        }

        private static string? PromptSave(string initialDirectory, string defaultFileName, string title)
        {
            using var dialog = new SaveFileDialog
            {
                InitialDirectory = initialDirectory,
                FileName = defaultFileName,
                Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*",
                DefaultExt = "xml",
                AddExtension = true,
                OverwritePrompt = true,
                CheckPathExists = true,
                Title = title,
            };

            return dialog.ShowDialog() == DialogResult.OK ? dialog.FileName : null;
        }

        private static void SerializeToFile<T>(T value, string path)
        {
            var serializer = new XmlSerializer(typeof(T));
            var settings = new XmlWriterSettings
            {
                Indent = true,
                Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
            };

            using var writer = XmlWriter.Create(path, settings);
            serializer.Serialize(writer, value);
        }

        private static void NotifyExport(string message, string path)
        {
            try
            {
                Clipboard.SetText(path);
            }
            catch
            {
                // Clipboard access can be denied; the export itself succeeded.
            }

            if (!SilentExport)
            {
                ShowInfo(message + $"{Environment.NewLine}{Environment.NewLine}The file path has been copied to the clipboard.");
            }
        }

    }
}
