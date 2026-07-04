// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Tools.Ribbon;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace AxaFrance.WebEngine.ExcelUI
{
    public partial class Ribbon
    {
        #region Constants
        private const int VARIABLE_ROW = 1;
        private const int VARIABLE_COLUMN = 1;
        private const int TEST_CASE_START_COLUMN = 3;
        private const int TEST_CASE_ROW = 3;
        private const string ENV_WORKSHEET_NAME = "ENV";
        private const string PARAMS_WORKSHEET_NAME = "PARAMS";
        private const string ENV_FILENAME = "ENV.xml";
        private const string XML_FILE_FILTER = "XML Files (*.xml)|*.xml";
        private const string WEBRUNNER_PROCESS_NAME = "WebRunner";
        #endregion

        #region Fields
        internal static AddinSettings Settings = new AddinSettings();
        private bool _noPopup = false;
        internal static string SettingFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AxaFrance.WebEngine", "excelSetting.xml");

        internal static string TestDataFile;
        internal static List<string> TestCases = new List<string>();
        internal static string EnvironmentVariableFile;
        internal static string CategoryName;
        #endregion

        #region Event Handlers
        private void Ribbon_Load(object sender, RibbonUIEventArgs e)
        {
            LoadSettings();
        }

        private void Ribbon_Close(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void btnSettings_Click(object sender, RibbonControlEventArgs e)
        {
            using (FrmSettings settings = new FrmSettings())
            {
                settings.ShowDialog();
            }
        }

        private void btnExportEnvironmentVariable_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                ValidateSettingsPath();
                ExportEnvironmentVariables();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Export failed: {ex.Message}", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnExportAll_Click(object sender, RibbonControlEventArgs e)
        {
            ExportAll(string.Empty, false);
        }

        private void btnExportSelection_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                ValidateSettingsPath();
                ExportSelectedTestCases();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Export selection failed: {ex.Message}", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnStop_Click(object sender, RibbonControlEventArgs e)
        {
            StopRunningTests();
        }

        private void btnStartNow_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                _noPopup = true;
                btnExportEnvironmentVariable_Click(null, null);
                btnExportSelection_Click(null, null);
                
                using (FrmRun run = new FrmRun(CategoryName))
                {
                    run.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _noPopup = false;
            }
        }

        private void btnCodeGenerationJava_Click(object sender, RibbonControlEventArgs e)
        {
            CodeGenerationGeneric(
                "public class ParameterList {",
                "\t/** Parameter: {0} */",
                "\n\t * ",
                "\tpublic static final String {0} = \"{1}\";"
            );
        }

        private void btnCodeGenerationCSharp_Click(object sender, RibbonControlEventArgs e)
        {
            CodeGenerationGeneric(
                "public static class ParameterList {\n",
                "\t///<Summary>Parameter: {0} </Summary>",
                "\n\t/// ",
                "\tpublic static string {0} {{get; }} = \"{1}\";"
            );
        }

        private void btnHelp_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                Process.Start("https://github.com/AxaFrance/webengine-dotnet");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open help: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnFeedback_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                Process.Start("https://github.com/AxaFrance/webengine-dotnet/issues");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open feedback page: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAbout_Click(object sender, RibbonControlEventArgs e)
        {
            using (FrmAbout about = new FrmAbout())
            {
                about.ShowDialog();
            }
        }

        private void btnSettingsSmall_Click(object sender, RibbonControlEventArgs e)
        {
            using (FrmSettings settings = new FrmSettings())
            {
                settings.ShowDialog();
            }
        }

        private void btnDataCheck_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                List<string> parameters = GetParameters();
                List<string> testcases = GetTestCases();
                List<string> testParameters = GetTestParameters();
                
                using (FrmCheckResult fcr = new FrmCheckResult(parameters, testParameters, testcases))
                {
                    fcr.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Data check failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDataMerge_Click(object sender, RibbonControlEventArgs e)
        {
            if (MessageBox.Show("This function will create missing parameters in your active test suite. This action cannot be reversed.\nDo you want to continue?", 
                "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    MergeParameters();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error when synchronizing: {ex.Message}\nPlease make sure all parameters are present in [PARAMS] tab", 
                        "Synchronization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnExportFilter_Click(object sender, RibbonControlEventArgs e)
        {
            using (FrmSelectFilter frmFilter = new FrmSelectFilter())
            {
                var result = frmFilter.ShowDialog();
                if (result == DialogResult.OK)
                {
                    ExportAll(frmFilter.Filter, frmFilter.FilterType);
                }
            }
        }
        #endregion

        #region Settings Management
        private void LoadSettings()
        {
            if (File.Exists(SettingFile))
            {
                try
                {
                    using (var stream = File.OpenRead(SettingFile))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(AddinSettings));
                        Ribbon.Settings = (AddinSettings)serializer.Deserialize(stream);
                    }
                }
                catch (Exception ex)
                {
                    // If deserialization fails, use default settings
                    Ribbon.Settings = new AddinSettings();
                    Debug.WriteLine($"Failed to load settings: {ex.Message}");
                }
            }
            else
            {
                Ribbon.Settings = new AddinSettings();
            }
        }

        internal static void SaveSettings()
        {
            try
            {
                var directory = new FileInfo(SettingFile).Directory;
                if (directory != null && !directory.Exists)
                {
                    directory.Create();
                }

                using (var fileStream = new FileStream(SettingFile, FileMode.Create, FileAccess.Write))
                using (var memoryStream = new MemoryStream())
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(AddinSettings));
                    serializer.Serialize(memoryStream, Ribbon.Settings);
                    byte[] data = memoryStream.ToArray();
                    fileStream.Write(data, 0, data.Length);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to save settings: {ex.Message}");
            }
        }
        #endregion

        #region Validation
        private void ValidateSettingsPath()
        {
            if (string.IsNullOrWhiteSpace(Settings.ExportPath))
            {
                throw new InvalidOperationException("Export path is not configured. Please configure settings first.");
            }

            if (!Directory.Exists(Settings.ExportPath))
            {
                Directory.CreateDirectory(Settings.ExportPath);
            }
        }
        #endregion

        #region Environment Variables Export
        private void ExportEnvironmentVariables()
        {
            Workbook workbook = Globals.ThisAddIn.Application.ActiveWorkbook;
            Worksheet worksheet = null;
            
            try
            {
                worksheet = workbook.Sheets[ENV_WORKSHEET_NAME];
                var parameters = ReadEnvironmentVariablesFromWorksheet(worksheet);
                
                EnvironmentVariables variables = new EnvironmentVariables { Variables = parameters };
                EnvironmentVariableFile = Path.Combine(Settings.ExportPath, ENV_FILENAME);

                if (!_noPopup)
                {
                    EnvironmentVariableFile = GetSaveFilePathFromUser(Settings.ExportPath, ENV_FILENAME, "Export Environment Variables");
                    if (string.IsNullOrEmpty(EnvironmentVariableFile))
                    {
                        return; // User cancelled
                    }
                }

                SaveEnvironmentVariablesToFile(variables, EnvironmentVariableFile);

                if (!_noPopup)
                {
                    MessageBox.Show($"Environment variables exported to:\n{EnvironmentVariableFile}", 
                        "Export Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            finally
            {
                ReleaseComObject(worksheet);
            }
        }

        private List<AxaFrance.WebEngine.Variable> ReadEnvironmentVariablesFromWorksheet(Worksheet worksheet)
        {
            var parameters = new List<AxaFrance.WebEngine.Variable>();
            Range usedRange = null;
            
            try
            {
                usedRange = worksheet.UsedRange;
                int rowCount = usedRange.Rows.Count;

                for (int i = 1; i <= rowCount; i++)
                {
                    Range cellName = null;
                    Range cellValue = null;
                    
                    try
                    {
                        cellName = (Range)usedRange.Cells[i, 1];
                        cellValue = (Range)usedRange.Cells[i, 2];

                        string variableName = cellName.Value2?.ToString()?.Trim();
                        string variableValue = cellValue.Value2?.ToString()?.Trim();

                        if (!string.IsNullOrEmpty(variableName))
                        {
                            parameters.Add(new AxaFrance.WebEngine.Variable
                            {
                                Name = variableName,
                                Value = variableValue
                            });
                        }
                    }
                    finally
                    {
                        ReleaseComObject(cellName);
                        ReleaseComObject(cellValue);
                    }
                }
            }
            finally
            {
                ReleaseComObject(usedRange);
            }

            return parameters;
        }

        private void SaveEnvironmentVariablesToFile(EnvironmentVariables variables, string filePath)
        {
            using (StreamWriter sw = new StreamWriter(filePath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(EnvironmentVariables));
                serializer.Serialize(sw, variables);
            }
        }
        #endregion

        #region Test Data Export
        private void ExportAll(string filter, bool filterType)
        {
            try
            {
                ValidateSettingsPath();
                
                var filters = string.IsNullOrEmpty(filter) ? new string[0] : filter.Split(';');
                Workbook workbook = Globals.ThisAddIn.Application.ActiveWorkbook;
                Worksheet worksheet = null;

                try
                {
                    worksheet = workbook.ActiveSheet;
                    CategoryName = worksheet.Name;
                    
                    if (Settings.ExcludedTabs.Contains(CategoryName))
                    {
                        return;
                    }

                    TestSuiteData list = new TestSuiteData();
                    list.TestDataList = ReadVariables(worksheet, null);
                    
                    var (exportedNames, duplicatedNames) = FilterAndValidateTestCases(list.TestDataList, filters, filterType);

                    if (duplicatedNames.Count > 0)
                    {
                        MessageBox.Show($"There are duplicated test cases: {string.Join(", ", duplicatedNames)}\nPlease fix the issue before export can be executed.", 
                            "Test Data Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    list.TestDataList.RemoveAll(x => !exportedNames.Contains(x.TestName));
                    SaveTestDataToFile(list);
                }
                finally
                {
                    ReleaseComObject(worksheet);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Export failed: {ex.Message}", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportSelectedTestCases()
        {
            var exportedNames = new List<string>();
            var duplicatedNames = new List<string>();
            var selectedColumns = new List<int>();

            Workbook workbook = Globals.ThisAddIn.Application.ActiveWorkbook;
            Worksheet worksheet = null;
            Range selectedRange = null;

            try
            {
                worksheet = workbook.ActiveSheet;
                CategoryName = worksheet.Name;
                selectedRange = Globals.ThisAddIn.Application.Selection;

                foreach (Range cell in selectedRange.Cells)
                {
                    int column = cell.Column - TEST_CASE_START_COLUMN;
                    selectedColumns.Add(column);
                    ReleaseComObject(cell);
                }

                TestSuiteData list = new TestSuiteData();
                list.TestDataList = ReadVariables(worksheet, selectedColumns.ToArray());
                TestCases.Clear();

                foreach (var t in list.TestDataList)
                {
                    if (exportedNames.Contains(t.TestName))
                    {
                        duplicatedNames.Add(t.TestName);
                    }
                    else
                    {
                        exportedNames.Add(t.TestName);
                    }
                    TestCases.Add($"\"{t.TestName}\"");
                }

                if (duplicatedNames.Count > 0)
                {
                    MessageBox.Show($"There are duplicated test cases: {string.Join(", ", duplicatedNames)}\nPlease fix the issue before export can be executed.", 
                        "Test Data Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                SaveTestDataToFile(list);
            }
            finally
            {
                ReleaseComObject(selectedRange);
                ReleaseComObject(worksheet);
            }
        }

        private (List<string> exportedNames, List<string> duplicatedNames) FilterAndValidateTestCases(
            List<TestData> testDataList, string[] filters, bool filterType)
        {
            var exportedNames = new List<string>();
            var duplicatedNames = new List<string>();
            TestCases.Clear();

            foreach (var t in testDataList)
            {
                if (IsTestCaseEligible(filters, filterType, t.TestName))
                {
                    if (exportedNames.Contains(t.TestName))
                    {
                        duplicatedNames.Add(t.TestName);
                    }
                    else
                    {
                        exportedNames.Add(t.TestName);
                    }
                    TestCases.Add($"\"{t.TestName}\"");
                }
            }

            return (exportedNames, duplicatedNames);
        }

        private void SaveTestDataToFile(TestSuiteData list)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(TestSuiteData));
            TestDataFile = Path.Combine(Settings.ExportPath, $"{CategoryName}.xml");

            if (!_noPopup)
            {
                TestDataFile = GetSaveFilePathFromUser(Settings.ExportPath, $"{CategoryName}.xml", "Export Test Data");
                if (string.IsNullOrEmpty(TestDataFile))
                {
                    return; // User cancelled
                }

                MessageBox.Show($"Test Data has exported to the following file:\n{TestDataFile}\n\nFile path has already copied to clipboard.", 
                    "Export Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            using (StreamWriter sw = new StreamWriter(TestDataFile))
            {
                serializer.Serialize(sw, list);
            }
        }

        private string GetSaveFilePathFromUser(string initialDirectory, string fileName, string title)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.InitialDirectory = initialDirectory;
                sfd.FileName = fileName;
                sfd.CheckPathExists = true;
                sfd.OverwritePrompt = true;
                sfd.AddExtension = true;
                sfd.Filter = XML_FILE_FILTER;
                sfd.Title = title;

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    return sfd.FileName;
                }
            }
            return null;
        }
        #endregion

        #region Test Case Filtering
        private bool IsTestCaseEligible(string[] filter, bool filterType, string testCaseName)
        {
            if (filter == null || filter.Length == 0)
            {
                return !filterType; // If no filter and filterType is false, include all
            }

            bool hasMatch = filter.Any(x => testCaseName.IndexOf(x, StringComparison.InvariantCultureIgnoreCase) >= 0);
            return filterType ? hasMatch : !hasMatch;
        }
        #endregion

        #region Read Variables from Worksheet
        private List<TestData> ReadVariables(Worksheet worksheet, int[] columns)
        {
            int maxColumns = GetMaxColumns(worksheet);
            int testCaseCount = maxColumns - TEST_CASE_START_COLUMN;
            TestData[] testcases = new TestData[testCaseCount];
            List<AxaFrance.WebEngine.Variable>[] testcasesParameters = new List<AxaFrance.WebEngine.Variable>[testCaseCount];
            
            for (int i = 0; i < testCaseCount; i++)
            {
                testcases[i] = new TestData();
                testcasesParameters[i] = new List<AxaFrance.WebEngine.Variable>();
            }

            Range usedRange = null;
            try
            {
                usedRange = worksheet.UsedRange;
                int rowCount = usedRange.Rows.Count;

                for (int currentRow = 1; currentRow <= rowCount; currentRow++)
                {
                    if (currentRow >= TEST_CASE_ROW)
                    {
                        Range paramNameCell = null;
                        try
                        {
                            paramNameCell = (Range)usedRange.Cells[currentRow, VARIABLE_COLUMN];
                            string variableName = GetCellValueAsString(paramNameCell);
                            
                            if (string.IsNullOrEmpty(variableName))
                            {
                                break;
                            }

                            ReadVariableValuesForRow(usedRange, currentRow, variableName, maxColumns, columns, testcasesParameters);
                        }
                        finally
                        {
                            ReleaseComObject(paramNameCell);
                        }
                    }
                }

                PopulateTestCases(testcases, testcasesParameters, columns);
            }
            finally
            {
                ReleaseComObject(usedRange);
            }

            List<TestData> testdata = new List<TestData>(testcases);
            testdata.RemoveAll(x => x.TestName == null);
            return testdata;
        }

        private void ReadVariableValuesForRow(Range usedRange, int currentRow, string variableName, 
            int maxColumns, int[] columns, List<AxaFrance.WebEngine.Variable>[] testcasesParameters)
        {
            for (int column = TEST_CASE_START_COLUMN; column < maxColumns; column++)
            {
                int i = column - TEST_CASE_START_COLUMN;
                
                if (columns != null && !columns.Contains(i))
                {
                    continue;
                }

                Range cell = null;
                try
                {
                    cell = (Range)usedRange.Cells[currentRow, column];
                    string variableValue = GetCellValueAsString(cell) ?? string.Empty;

                    testcasesParameters[i].Add(new AxaFrance.WebEngine.Variable
                    {
                        Name = variableName,
                        Value = variableValue
                    });
                }
                finally
                {
                    ReleaseComObject(cell);
                }
            }
        }

        private void PopulateTestCases(TestData[] testcases, List<AxaFrance.WebEngine.Variable>[] testcasesParameters, int[] columns)
        {
            for (int i = 0; i < testcases.Length; i++)
            {
                if (columns != null && !columns.Contains(i))
                {
                    continue;
                }

                testcases[i].Data = testcasesParameters[i].ToArray();
                var testCaseVar = testcasesParameters[i].FirstOrDefault(x => x.Name == "TESTCASE");
                testcases[i].TestName = testCaseVar?.Value;
            }
        }

        private int GetMaxColumns(Worksheet worksheet)
        {
            Range row = null;
            try
            {
                row = (Range)worksheet.Rows[3];
                int currentColumn = TEST_CASE_START_COLUMN;
                int count = 0;

                while (true)
                {
                    Range cell = null;
                    try
                    {
                        cell = (Range)row.Cells[1, currentColumn];
                        object value = cell.Value2;
                        
                        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                        {
                            break;
                        }
                        
                        currentColumn++;
                        count++;
                    }
                    finally
                    {
                        ReleaseComObject(cell);
                    }
                }

                return count - 1 + TEST_CASE_START_COLUMN;
            }
            finally
            {
                ReleaseComObject(row);
            }
        }
        #endregion

        #region Process Management
        private void StopRunningTests()
        {
            Process[] processes = Process.GetProcessesByName(WEBRUNNER_PROCESS_NAME);
            
            if (processes != null && processes.Length > 0)
            {
                if (MessageBox.Show("Stopping the current test will terminate all running tests. Do you want to continue?", 
                    "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    foreach (Process p in processes)
                    {
                        try
                        {
                            if (!p.HasExited)
                            {
                                p.Kill();
                                p.WaitForExit(5000); // Wait up to 5 seconds
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Failed to kill process {p.Id}: {ex.Message}");
                        }
                        finally
                        {
                            p.Dispose();
                        }
                    }
                }
            }
        }
        #endregion

        #region Code Generation
        private void CodeGenerationGeneric(string classBegin, string singleLineCommentTemplate, 
            string multiLineSeparator, string propertyTemplate)
        {
            StringBuilder sb = new StringBuilder();
            Workbook workbook = Globals.ThisAddIn.Application.ActiveWorkbook;
            Worksheet worksheet = null;
            
            try
            {
                worksheet = workbook.Sheets[PARAMS_WORKSHEET_NAME];
                Range usedRange = null;
                
                try
                {
                    usedRange = worksheet.UsedRange;
                    sb.AppendLine(classBegin);
                    int rowCount = usedRange.Rows.Count;

                    for (int i = 1; i <= rowCount; i++)
                    {
                        Range cellName = null;
                        Range cellValue = null;
                        
                        try
                        {
                            cellName = (Range)usedRange.Cells[i, 1];
                            cellValue = (Range)usedRange.Cells[i, 2];

                            string paramName = cellName.Value2?.ToString()?.Trim();
                            string description = cellValue.Value2?.ToString()?.Trim();

                            // End generation if both are empty
                            if (string.IsNullOrWhiteSpace(paramName) && string.IsNullOrWhiteSpace(description))
                            {
                                break;
                            }

                            // Ignore names with spaces
                            if (!string.IsNullOrWhiteSpace(paramName) && !paramName.Contains(' '))
                            {
                                AppendParameterToCode(sb, paramName, description, singleLineCommentTemplate, 
                                    multiLineSeparator, propertyTemplate);
                            }
                        }
                        finally
                        {
                            ReleaseComObject(cellName);
                            ReleaseComObject(cellValue);
                        }
                    }

                    sb.AppendLine("}");
                    Clipboard.SetText(sb.ToString());
                    MessageBox.Show("The code generation for parameter list is complete.\nThe code is copied to your clipboard and you can paste it in your code.", 
                        "Code Generation Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                finally
                {
                    ReleaseComObject(usedRange);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error when generating code: {ex.Message}\nPlease make sure all parameters are present in [{PARAMS_WORKSHEET_NAME}] tab", 
                    "Code Generation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                ReleaseComObject(worksheet);
            }
        }

        private void AppendParameterToCode(StringBuilder sb, string paramName, string description, 
            string singleLineCommentTemplate, string multiLineSeparator, string propertyTemplate)
        {
            string comment;
            if (string.IsNullOrWhiteSpace(description))
            {
                comment = string.Format(singleLineCommentTemplate, paramName);
            }
            else
            {
                string content = description.Replace("\n", multiLineSeparator);
                comment = string.Format(singleLineCommentTemplate, content);
            }
            
            sb.AppendLine(comment);
            sb.AppendLine(string.Format(propertyTemplate, paramName, paramName));
            sb.AppendLine();
        }
        #endregion

        #region Parameter Management
        private List<string> GetTestCases()
        {
            var testCases = new List<string>();
            Workbook workbook = Globals.ThisAddIn.Application.ActiveWorkbook;
            Worksheet worksheet = null;
            Range row = null;

            try
            {
                worksheet = workbook.ActiveSheet;
                
                if (Settings.ExcludedTabs.Contains(worksheet.Name.ToUpper()))
                {
                    throw new WebEngineGeneralException("The current worksheet is not test data but a special excluded worksheet.");
                }

                row = (Range)worksheet.Rows[3];
                int columnIndex = 1;

                foreach (Range cell in row.Cells)
                {
                    try
                    {
                        if (columnIndex >= TEST_CASE_START_COLUMN)
                        {
                            string variableName = GetCellValueAsString(cell);
                            
                            if (!string.IsNullOrEmpty(variableName))
                            {
                                testCases.Add(variableName);
                            }
                            else
                            {
                                break;
                            }
                        }
                        columnIndex++;
                    }
                    finally
                    {
                        ReleaseComObject(cell);
                    }
                }
            }
            finally
            {
                ReleaseComObject(row);
                ReleaseComObject(worksheet);
            }

            return testCases;
        }

        private List<string> GetParameters()
        {
            var parameters = new List<string>();
            Workbook workbook = Globals.ThisAddIn.Application.ActiveWorkbook;
            Worksheet worksheet = null;
            Range usedRange = null;

            try
            {
                worksheet = workbook.Sheets[PARAMS_WORKSHEET_NAME];
                usedRange = worksheet.UsedRange;
                int rowCount = usedRange.Rows.Count;

                for (int i = 2; i <= rowCount; i++) // Start from row 2 to skip header
                {
                    Range cellName = null;
                    try
                    {
                        cellName = (Range)usedRange.Cells[i, 1];
                        string variableName = GetCellValueAsString(cellName);
                        
                        if (!string.IsNullOrEmpty(variableName))
                        {
                            parameters.Add(variableName);
                        }
                        else
                        {
                            break;
                        }
                    }
                    finally
                    {
                        ReleaseComObject(cellName);
                    }
                }
            }
            finally
            {
                ReleaseComObject(usedRange);
                ReleaseComObject(worksheet);
            }

            return parameters;
        }

        private List<string> GetTestParameters()
        {
            var parameters = new List<string>();
            Workbook workbook = Globals.ThisAddIn.Application.ActiveWorkbook;
            Worksheet worksheet = null;
            Range usedRange = null;

            try
            {
                worksheet = workbook.ActiveSheet;
                
                if (Settings.ExcludedTabs.Contains(worksheet.Name.ToUpper()))
                {
                    throw new WebEngineGeneralException("The current worksheet is not test data but a special excluded worksheet.");
                }

                usedRange = worksheet.UsedRange;
                int rowCount = usedRange.Rows.Count;

                for (int currentRow = TEST_CASE_ROW; currentRow <= rowCount; currentRow++)
                {
                    Range paramNameCell = null;
                    try
                    {
                        paramNameCell = (Range)usedRange.Cells[currentRow, VARIABLE_COLUMN];
                        string variableName = GetCellValueAsString(paramNameCell);
                        
                        if (!string.IsNullOrEmpty(variableName))
                        {
                            parameters.Add(variableName.Trim());
                        }
                        else
                        {
                            break;
                        }
                    }
                    finally
                    {
                        ReleaseComObject(paramNameCell);
                    }
                }
            }
            finally
            {
                ReleaseComObject(usedRange);
                ReleaseComObject(worksheet);
            }

            return parameters;
        }

        private void MergeParameters()
        {
            Workbook workbook = Globals.ThisAddIn.Application.ActiveWorkbook;
            var parameters = new List<AxaFrance.WebEngine.Variable>();
            Worksheet paramsWorksheet = null;
            Worksheet activeSheet = null;
            Range usedRange = null;

            try
            {
                // Get Parameters from PARAMS worksheet
                paramsWorksheet = workbook.Sheets[PARAMS_WORKSHEET_NAME];
                usedRange = paramsWorksheet.UsedRange;
                int rowCount = usedRange.Rows.Count;

                for (int i = 1; i <= rowCount; i++)
                {
                    Range cellName = null;
                    Range cellValue = null;
                    
                    try
                    {
                        cellName = (Range)usedRange.Cells[i, 1];
                        cellValue = (Range)usedRange.Cells[i, 2];

                        string paramName = cellName.Value2?.ToString()?.Trim();
                        string description = cellValue.Value2?.ToString()?.Trim();

                        // Ignore names with spaces
                        if (!string.IsNullOrWhiteSpace(paramName) && !paramName.Contains(' '))
                        {
                            parameters.Add(new Variable(paramName, description));
                        }

                        if (string.IsNullOrWhiteSpace(paramName) && string.IsNullOrWhiteSpace(description))
                        {
                            break;
                        }
                    }
                    finally
                    {
                        ReleaseComObject(cellName);
                        ReleaseComObject(cellValue);
                    }
                }

                ReleaseComObject(usedRange);
                usedRange = null;

                // Get Parameters in Active Sheet
                activeSheet = workbook.ActiveSheet;
                if (!Ribbon.Settings.ExcludedTabs.Contains(activeSheet.Name.ToUpper()))
                {
                    var testParams = GetParametersInSheet(activeSheet);
                    SyncParams(parameters, testParams, activeSheet);
                }
                else
                {
                    MessageBox.Show("This function can only be used in Test DATA worksheets.", 
                        "Invalid Worksheet", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            finally
            {
                ReleaseComObject(usedRange);
                ReleaseComObject(activeSheet);
                ReleaseComObject(paramsWorksheet);
            }
        }

        private void SyncParams(List<Variable> parameters, List<string> testParams, Worksheet sheet)
        {
            foreach (var p in parameters)
            {
                if (!testParams.Contains(p.Name))
                {
                    Range range = null;
                    Range cell = null;
                    
                    try
                    {
                        int index = parameters.IndexOf(p) + TEST_CASE_ROW + VARIABLE_ROW - 1;
                        range = sheet.get_Range($"A{index}").EntireRow;
                        range.Insert(XlInsertShiftDirection.xlShiftDown, Type.Missing);
                        
                        cell = sheet.get_Range($"A{index}");
                        cell.set_Value(XlRangeValueDataType.xlRangeValueDefault, p.Name);
                    }
                    finally
                    {
                        ReleaseComObject(cell);
                        ReleaseComObject(range);
                    }
                }
            }
        }

        private List<string> GetParametersInSheet(Worksheet worksheet)
        {
            var paramNames = new List<string>();
            Range usedRange = null;
            
            try
            {
                usedRange = worksheet.UsedRange;
                int rowCount = usedRange.Rows.Count;

                for (int currentRow = TEST_CASE_ROW; currentRow <= rowCount; currentRow++)
                {
                    Range paramNameCell = null;
                    try
                    {
                        paramNameCell = (Range)usedRange.Cells[currentRow, VARIABLE_COLUMN];
                        string variableName = GetCellValueAsString(paramNameCell);
                        
                        if (string.IsNullOrEmpty(variableName))
                        {
                            break;
                        }
                        
                        paramNames.Add(variableName);
                    }
                    finally
                    {
                        ReleaseComObject(paramNameCell);
                    }
                }
            }
            finally
            {
                ReleaseComObject(usedRange);
            }

            return paramNames;
        }
        #endregion

        #region Helper Methods
        private string GetCellValueAsString(Range cell)
        {
            if (cell == null) return null;
            
            try
            {
                object value = cell.Value2;
                return value?.ToString()?.Trim();
            }
            catch
            {
                return null;
            }
        }

        private void ReleaseComObject(object obj)
        {
            if (obj != null)
            {
                try
                {
                    Marshal.ReleaseComObject(obj);
                }
                catch
                {
                    // Ignore errors during COM cleanup
                }
            }
        }
        #endregion
    }
}
