// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;
using System.IO;
using AXA.WebEngine;
using Microsoft.Office.Interop.Excel;
using System.Windows.Forms;
using System.Diagnostics;

namespace AXA.WebEngine.ExcelUI
{
    public partial class Ribbon
    {
        internal static AddinSettings Settings = new AddinSettings();
        bool noPopup = false;
        int VariableRow = 1;
        int VariableColumn = 1;
        int TestCaseStartColumn = 3;
        int TestCaseRow = 3;
        internal static string settingFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AXA.WebEngine", "excelSetting.xml");

        internal static string TestDataFile;
        internal static List<string> TestCases = new List<string>();
        internal static string EnvironmentVariableFile;
        internal static string categoryName;

        private void Ribbon_Load(object sender, RibbonUIEventArgs e)
        {
            if (File.Exists(settingFile))
            {
                using (var stream = File.OpenRead(settingFile))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(AddinSettings));
                    AddinSettings settings = null;
                    try
                    {
                        settings = (AddinSettings)serializer.Deserialize(stream);
                    }
                    catch
                    {
                        settings = new AddinSettings();
                    }
                    Ribbon.Settings = settings;
                }
            }
            else
            {
                Ribbon.Settings = new AddinSettings();
            }

        }

        internal static void SaveSettings()
        {
            Directory.CreateDirectory(new FileInfo(settingFile).Directory.FullName);
            using (var stream = new FileStream(settingFile, FileMode.Create, FileAccess.Write))
            {
                MemoryStream ms = new MemoryStream();
                XmlSerializer serializer = new XmlSerializer(typeof(AddinSettings));
                serializer.Serialize(ms, Ribbon.Settings);
                stream.Write(ms.ToArray(), 0, (int)ms.Length);
                stream.Close();
            }
        }

        private void btnSettings_Click(object sender, RibbonControlEventArgs e)
        {
            FrmSettings settings = new FrmSettings();
            settings.ShowDialog();
        }

        private void Ribbon_Close(object sender, EventArgs e)
        {
            SaveSettings();
        }

        private void btnExportEnvironmentVariable_Click(object sender, RibbonControlEventArgs e)
        {
            Workbook workbook = Globals.ThisAddIn.Application.ActiveWorkbook;
            List<AXA.WebEngine.Variable> parameters = new List<AXA.WebEngine.Variable>();
            try
            {
                Worksheet worksheet = workbook.Sheets["ENV"];
                foreach (Range row in worksheet.UsedRange.Rows)
                {
                    string variableName, variableValue;
                    Range cell_name = row.Cells[1];
                    object name = cell_name.Value2;
                    variableName = name != null ? name.ToString().Trim() : null;
                    Range cell_value = row.Cells[2];
                    object value = cell_value.Value2;
                    variableValue = value != null ? value.ToString().Trim() : null;
                    if (!string.IsNullOrEmpty(variableName))
                    {
                        parameters.Add(new AXA.WebEngine.Variable()
                        {
                            Name = variableName,
                            Value = variableValue,
                        });
                    }
                }
                EnvironmentVariables variables = new EnvironmentVariables();
                variables.Variables = parameters;
                EnvironmentVariableFile = Settings.ExportPath + "ENV.xml";

                if (!noPopup)
                {
                    SaveFileDialog sfd = new SaveFileDialog()
                    {
                        InitialDirectory = Settings.ExportPath,
                        FileName = "ENV.xml",
                        CheckPathExists = true,
                        OverwritePrompt = true,
                        AddExtension = true,
                        Filter = "XML Files (*.xml)|*.xml",
                        Title = "Export Environment Variables",
                    };

                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        EnvironmentVariableFile = sfd.FileName;
                        using (StreamWriter sw = new StreamWriter(EnvironmentVariableFile))
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(EnvironmentVariables));
                            serializer.Serialize(sw, variables);
                            sw.Close();
                        }
                        MessageBox.Show("Les variables d'environnement sont exporté dans le fichier:\n" + EnvironmentVariableFile);
                    }
                }
                else
                {
                    using (StreamWriter sw = new StreamWriter(EnvironmentVariableFile))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(EnvironmentVariables));
                        serializer.Serialize(sw, variables);
                        sw.Close();
                    }
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("Export de variables échoué, message d'erreur:" + ex.Message);
            }
        }

        /// <summary>
        /// Returns if the test case is eligible to be selected by a filter
        /// </summary>
        /// <param name="filter">The filters of selection</param>
        /// <param name="filterType">
        /// True: export only matched test cases, False: export not matched test cases.
        /// </param>
        /// <param name="testCaseName">the test case name to be selected.</param>
        /// <returns></returns>
        private bool testCaseEligible(string[] filter, bool filterType, string testCaseName)
        {
            if (filterType)
            {
                return filter.Any(x => testCaseName.IndexOf(x, StringComparison.InvariantCultureIgnoreCase) > 0);
            }
            else
            {
                if (filter == null || filter.Length == 0) return true;
                return !filter.Any(x => testCaseName.IndexOf(x, StringComparison.InvariantCultureIgnoreCase) > 0);
            }
        }

        /// <summary>
        /// Export all test cases of the current Tab
        /// </summary>
        /// <param name="filter">filter in text form</param>
        /// <param name="filterType">
        /// True: export only matched test cases, False: export not matched test cases.
        /// </param>
        private void ExportAll(string filter, bool filterType)
        {
            var filters = filter.Split(';');
            List<string> exportedNames = new List<string>();
            List<string> duplicatedNames = new List<string>();
            Workbook workbook = Globals.ThisAddIn.Application.ActiveWorkbook;
            Worksheet worksheet = workbook.ActiveSheet;
            categoryName = worksheet.Name;
            if (Settings.ExcludedTabs.Contains(categoryName))
            {
                return;
            }
            int maxColumns = worksheet.UsedRange.Columns.Count;
            TestSuiteData list = new TestSuiteData();
            list.TestDataList = ReadVariables(worksheet, null);
            TestCases.Clear();
            foreach (var t in list.TestDataList)
            {
                if (testCaseEligible(filters, filterType, t.TestName))
                {

                    if (exportedNames.Contains(t.TestName))
                    {
                        duplicatedNames.Add(t.TestName);
                    }
                    else
                    {
                        exportedNames.Add(t.TestName);
                    }
                    TestCases.Add("\"" + t.TestName + "\"");
                }
            }

            if (duplicatedNames.Count > 0)
            {
                MessageBox.Show(string.Format("There are duplicated test cases: {0}\nPlease fix the issue before export can be executed.", string.Join(", ", duplicatedNames.ToArray())), "Test data Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            list.TestDataList.RemoveAll(x => !exportedNames.Contains(x.TestName));

            SaveTestDataToFile(list);
        }

        private void SaveTestDataToFile(TestSuiteData list)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(TestSuiteData));
            TestDataFile = Settings.ExportPath + categoryName + ".xml";


            if (!noPopup)
            {
                SaveFileDialog sfd = new SaveFileDialog()
                {
                    InitialDirectory = Settings.ExportPath,
                    FileName = categoryName + ".xml",
                    CheckPathExists = true,
                    OverwritePrompt = true,
                    AddExtension = true,
                    Filter = "XML Files (*.xml)|*.xml",
                    Title = "Export Test Data",
                };

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    TestDataFile = sfd.FileName;
                    StreamWriter sw = new StreamWriter(TestDataFile);
                    serializer.Serialize(sw, list);
                    sw.Close();

                }
                MessageBox.Show("Test Data has exported to the following file:\n" + TestDataFile + "\n\nFile path has already copied to clipboard.", "Export Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            else
            {
                StreamWriter sw = new StreamWriter(TestDataFile);
                serializer.Serialize(sw, list);
                sw.Close();
            }
        }

        private void btnExportAll_Click(object sender, RibbonControlEventArgs e)
        {
            ExportAll(string.Empty, false);
        }

        private List<TestData> ReadVariables(Worksheet worksheet, int[] columns)
        {
            int maxColumns = GetMaxColumns(worksheet);
            int testcase_count = maxColumns - TestCaseStartColumn;
            TestData[] testcases = new TestData[testcase_count];
            List<AXA.WebEngine.Variable>[] testcases_parameters = new List<AXA.WebEngine.Variable>[testcase_count];
            for (int i = 0; i < testcase_count; i++)
            {
                testcases[i] = new TestData();
                testcases_parameters[i] = new List<AXA.WebEngine.Variable>();
            }
            List<string> paramNames = new List<string>();
            int currentRow = 1;
            foreach (Range row in worksheet.UsedRange.Rows)
            {
                if (currentRow >= TestCaseRow)
                {
                    Range param_name = row.Cells[VariableColumn];
                    //Exit reading if cannot read the name of the variable.
                    string variableName = null;
                    try
                    {
                        variableName = param_name.get_Value().ToString();
                    }
                    catch { break; }
                    if (string.IsNullOrEmpty(variableName)) break;

                    for (int column = TestCaseStartColumn; column < maxColumns; column++)
                    {
                        int i = column - TestCaseStartColumn;
                        if (columns == null || columns.Contains(i))
                        {

                        }
                        else
                        {
                            continue;
                        }

                        Range cell = row.Cells[column];
                        object obj = cell.get_Value();
                        string variableValue = null;
                        if (obj != null)
                        {
                            variableValue = obj.ToString();
                        }

                        if (variableValue == null) variableValue = string.Empty;
                        testcases_parameters[i].Add(new AXA.WebEngine.Variable()
                        {
                            Name = variableName,
                            Value = variableValue
                        });
                    }
                }
                currentRow++;
            }

            for (int i = 0; i < testcase_count; i++)
            {
                if (columns == null || columns.Contains(i))
                {

                }
                else
                {
                    continue;
                }
                testcases[i].Data = testcases_parameters[i].ToArray();
                testcases[i].TestName = testcases_parameters[i].First(x => x.Name == "TESTCASE").Value;
            }
            List<TestData> testdata = new List<TestData>(testcases);
            testdata.RemoveAll(x => x.TestName == null);
            return testdata;
        }

        private int GetMaxColumns(Worksheet worksheet)
        {
            Range row = worksheet.Rows[3];
            int currentColumn = TestCaseStartColumn;
            int count = 0;
            object value = null;
            do
            {
                Range cell = row.Cells[currentColumn];
                value = cell.get_Value();
                currentColumn++;
                count++;
            } while (value != null && !string.IsNullOrWhiteSpace(value.ToString()));

            return count - 1 + TestCaseStartColumn;
        }

        private void btnExportSelection_Click(object sender, RibbonControlEventArgs e)
        {
            List<string> exportedNames = new List<string>();
            List<string> duplicatedNames = new List<string>();
            List<int> selectedColumns = new List<int>();

            Workbook workbook = Globals.ThisAddIn.Application.ActiveWorkbook;
            Worksheet worksheet = workbook.ActiveSheet;
            categoryName = worksheet.Name;
            Range selectedRange = Globals.ThisAddIn.Application.Selection;
            foreach (Range cell in selectedRange.Cells)
            {
                int column = cell.Column - TestCaseStartColumn;
                selectedColumns.Add(column);
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
                TestCases.Add("\"" + t.TestName + "\"");
            }

            if (duplicatedNames.Count > 0)
            {
                MessageBox.Show(string.Format("There are duplicated test cases: {0}\nPlease fix the issue before export can be executed.", string.Join(", ", duplicatedNames.ToArray())), "Test data Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            SaveTestDataToFile(list);

        }

        private void btnStop_Click(object sender, RibbonControlEventArgs e)
        {
            Process[] process = Process.GetProcessesByName("WebRunner");
            if (process != null && process.Length > 0)
            {
                if (MessageBox.Show("Stopping the current test will L’arrêt de test va killer tous les test en cours d'execution.Voulez-vous continuer?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {


                    foreach (Process p in process)
                    {
                        p.Kill();
                    }
                }
            }

        }

        private void btnStartNow_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                noPopup = true;
                btnExportEnvironmentVariable_Click(null, null);
                btnExportSelection_Click(null, null);
                FrmRun run = new FrmRun(categoryName);
                run.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occurred: " + ex.Message);
            }
            finally
            {
                noPopup = false;
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

        private void CodeGenerationGeneric(string classBegin, string singleLineCommentTemplate, string multiLineSeparator, string propertyTemplate)
        {
            StringBuilder sb = new StringBuilder();
            Workbook workbook = Globals.ThisAddIn.Application.ActiveWorkbook;
            List<AXA.WebEngine.Variable> parameters = new List<AXA.WebEngine.Variable>();
            try
            {
                Worksheet worksheet = workbook.Sheets["PARAMS"];
                #region append content
                sb.AppendLine(classBegin);
                foreach (Range row in worksheet.UsedRange.Rows)
                {
                    string paramname, description;
                    Range cell_name = row.Cells[1];
                    object name = cell_name.Value2;
                    paramname = name != null ? name.ToString().Trim() : null;
                    Range cell_value = row.Cells[2];
                    object value = cell_value.Value2;
                    description = value != null ? value.ToString().Trim() : null;

                    //Ignore all names has Spaces
                    if (paramname.Contains(' ')) continue;
                    if (string.IsNullOrWhiteSpace(paramname) && string.IsNullOrWhiteSpace(description))
                    {
                        break;
                    }

                    //Add description in comment;
                    if (description == null)
                    {
                        description = string.Format(singleLineCommentTemplate, paramname);
                    }
                    else
                    {
                        string content = description.Replace("\n", multiLineSeparator);
                        description = string.Format(singleLineCommentTemplate, content);
                    }
                    sb.AppendLine(description);
                    sb.AppendLine(string.Format(propertyTemplate, paramname, paramname));
                    sb.AppendLine();

                }
                sb.AppendLine("}");
                Clipboard.SetText(sb.ToString());
                MessageBox.Show("The code generation for parameter list is complete.\nThe code is copied to your clipboard and you can paste it in your code.");
                #endregion

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error when generation code. please make sure all parameters are present in [PARAMS] tab\n" + ex.Message);
            }
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
            Process.Start("http://guilde-test.axa-fr.intraxa/web-engine/html/0e6dacc1-0943-4f90-8222-431b2afb7693.htm");
        }

        private void btnFeedback_Click(object sender, RibbonControlEventArgs e)
        {
            Process.Start("https://github.axa.com/AXA-GS-QA/WebEngine-Framework-issues");
        }

        private void btnAbout_Click(object sender, RibbonControlEventArgs e)
        {
            FrmAbout about = new FrmAbout();
            about.ShowDialog();
        }

        private void btnSettingsSmall_Click(object sender, RibbonControlEventArgs e)
        {
            FrmSettings settings = new FrmSettings();
            settings.ShowDialog();
        }

        private void btnDataCheck_Click(object sender, RibbonControlEventArgs e)
        {
            List<string> parameters = GetParameters();
            List<string> testcases = GetTestCases();
            List<string> testParameters = GetTestParameters();
            FrmCheckResult fcr = new FrmCheckResult(parameters, testParameters, testcases);
            fcr.ShowDialog();
        }

        private List<string> GetTestCases()
        {
            List<string> parameters = new List<string>();
            Workbook workbook = Globals.ThisAddIn.Application.ActiveWorkbook;
            Worksheet worksheet = workbook.ActiveSheet;
            if (Settings.ExcludedTabs.Contains(worksheet.Name.ToUpper()))
            {
                throw new WebEngineGeneralException("The current worksheet is not test data but a special excluded worksheet. ");
            }
            Range r = worksheet.Rows[3];
            int i = 1;
            foreach (Range cell in r.Cells)
            {
                if (i >= TestCaseStartColumn)
                {

                    string variableName;
                    object name = cell.Value2;
                    variableName = name != null ? name.ToString().Trim() : null;
                    if (!string.IsNullOrEmpty(variableName))
                    {
                        parameters.Add(variableName);
                    }
                    else
                    {
                        break;
                    }
                }
                i++;
            }
            return parameters;

        }

        private List<string> GetParameters()
        {
            List<string> parameters = new List<string>();
            Workbook workbook = Globals.ThisAddIn.Application.ActiveWorkbook;
            Worksheet worksheet = workbook.Sheets["PARAMS"];
            int i = 1;
            foreach (Range row in worksheet.UsedRange.Rows)
            {
                if (i > 1)
                {
                    string variableName;
                    Range cell_name = row.Cells[1];
                    object name = cell_name.Value2;
                    variableName = name != null ? name.ToString().Trim() : null;
                    if (!string.IsNullOrEmpty(variableName))
                    {
                        parameters.Add(variableName);
                    }
                    else
                    {
                        break;
                    }
                }
                i++;
            }
            return parameters;
        }

        private List<string> GetTestParameters()
        {
            List<string> parameters = new List<string>();
            Workbook workbook = Globals.ThisAddIn.Application.ActiveWorkbook;
            Worksheet worksheet = workbook.ActiveSheet;
            if (Settings.ExcludedTabs.Contains(worksheet.Name.ToUpper()))
            {
                throw new WebEngineGeneralException("The current worksheet is not test data but a special excluded worksheet. ");
            }
            int currentRow = 1;
            foreach (Range row in worksheet.UsedRange.Rows)
            {
                if (currentRow >= TestCaseRow)
                {
                    Range param_name = row.Cells[VariableColumn];
                    //Exit reading if cannot read the name of the variable.
                    string variableName = null;
                    try
                    {
                        variableName = param_name.get_Value().ToString();
                        variableName = variableName.Trim();
                        parameters.Add(variableName);
                    }
                    catch { break; }

                }
                currentRow++;
            }

            return parameters;
        }

        private void btnDataMerge_Click(object sender, RibbonControlEventArgs e)
        {

            if (MessageBox.Show("This function will create missing parameters in your active test suite, This action cannot be reversed.\nDo you want to continue?", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Workbook workbook = Globals.ThisAddIn.Application.ActiveWorkbook;
                List<AXA.WebEngine.Variable> parameters = new List<AXA.WebEngine.Variable>();
                try
                {
                    #region Get Parameters in PARAM

                    Worksheet worksheet = workbook.Sheets["PARAMS"];
                    foreach (Range row in worksheet.UsedRange.Rows)
                    {
                        string paramname, description;
                        Range cell_name = row.Cells[1];
                        object name = cell_name.Value2;
                        paramname = name != null ? name.ToString().Trim() : null;
                        Range cell_value = row.Cells[2];
                        object value = cell_value.Value2;
                        description = value != null ? value.ToString().Trim() : null;

                        //Ignore all names has Spaces
                        if (paramname.Contains(' ')) continue;
                        if (string.IsNullOrWhiteSpace(paramname) && string.IsNullOrWhiteSpace(description))
                        {
                            break;
                        }

                        parameters.Add(new Variable(paramname, description));

                    }
                    #endregion

                    #region Get Parameters in Active Sheet
                    Worksheet sheet = workbook.ActiveSheet;
                    if (!Ribbon.Settings.ExcludedTabs.Contains(sheet.Name.ToUpper()))
                    {
                        var testParams = GetParametersInSheet(sheet);
                        SyncParams(parameters, testParams, sheet);
                    }
                    else
                    {
                        MessageBox.Show("This function can only be used in Test DATA worksheets.");
                    }

                    #endregion
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error when synchronizing . please make sure all parameters are present in [PARAMS] tab\n" + ex.Message);
                }
            }
        }

        /// <summary>
        /// Synchronize Parameters defined in PARAM worksheet and Test Suite worksheet
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="testParams"></param>
        /// <param name="sheet"></param>
        private void SyncParams(List<Variable> parameters, List<string> testParams, Worksheet sheet)
        {
            foreach (var p in parameters)
            {
                if (!testParams.Contains(p.Name))
                {
                    int index = parameters.IndexOf(p) + this.TestCaseRow + VariableRow - 1;
                    Range range = sheet.get_Range("A" + index).EntireRow;
                    range.Insert(XlInsertShiftDirection.xlShiftDown, Type.Missing);
                    Range cell = sheet.get_Range("A" + index);
                    cell.set_Value(XlRangeValueDataType.xlRangeValueDefault, p.Name);
                }
            }
        }

        private List<string> GetParametersInSheet(Worksheet worksheet)
        {
            List<string> paramNames = new List<string>();
            int currentRow = 1;
            foreach (Range row in worksheet.UsedRange.Rows)
            {
                if (currentRow >= TestCaseRow)
                {
                    Range param_name = row.Cells[VariableColumn];
                    //Exit reading if cannot read the name of the variable.
                    string variableName = null;
                    try
                    {
                        variableName = param_name.get_Value().ToString();
                    }
                    catch { break; }
                    if (string.IsNullOrEmpty(variableName)) break;
                    paramNames.Add(variableName);
                }
                currentRow++;
            }
            return paramNames;

        }

        private void BtnExportFilter_Click(object sender, RibbonControlEventArgs e)
        {
            FrmSelectFilter frmFilter = new FrmSelectFilter();
            var result = frmFilter.ShowDialog();
            if (result == DialogResult.OK)
            {
                ExportAll(frmFilter.Filter, frmFilter.FilterType);
            }
        }
    }
}
