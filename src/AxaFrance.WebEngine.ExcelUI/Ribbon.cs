// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using AxaFrance.WebEngine.ExcelUI.Properties;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Tools.Ribbon;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;
using YamlDotNet.RepresentationModel;
using static System.Environment;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.LinkLabel;
using Button = System.Windows.Forms.Button;
using Label = System.Windows.Forms.Label;
using TextBox = Microsoft.Office.Interop.Excel.TextBox;

namespace AxaFrance.WebEngine.ExcelUI
{

    //generate comments for all the functions
    /// <summary>
    /// 
    public partial class Ribbon
    {
        private const string Drive_Help_SheetName = "DriveByExcel_Help";
        internal static AddinSettings Settings = new AddinSettings();
        bool noPopup = false;
        int VariableRow = 1;
        int VariableColumn = 1;
        int TestCaseStartColumn = 3;
        int TestCaseRow = 3;

        //create enum for the different languages
        public enum NoCodeCmdLangage
        {
            English,
            French
        }

        private bool isNoCode = false;

        internal static string settingFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AxaFrance.WebEngine", "excelWithDriveSetting.xml");

        internal static string TestDataFile;
        internal static List<string> TestCases = new List<string>();
        internal static string EnvironmentVariableFile;
        internal static string categoryName;




        private CommandBar GetCellContextMenu()
        {
            return Globals.ThisAddIn.Application.CommandBars["Cell"];
        }

        private void ResetCellMenu()
        {
            GetCellContextMenu().Reset(); // reset the cell context menu back to the default
        }

        private void Application_SheetBeforeRightClick(object Sh, Range Target, ref bool Cancel)
        {
            ResetCellMenu();  // reset the cell context menu back to the default

            if (Target.Cells.Column == 3)   // sample code: if only a single cell is selected
            {
               AddExampleMenuItem();
            }
        }

        private void AddExampleMenuItem()
        {
            MsoControlType menuItem = MsoControlType.msoControlButton;
            CommandBarButton exampleMenuItem = (CommandBarButton)GetCellContextMenu().Controls.Add(menuItem, "1", "2", 1, true);

            exampleMenuItem.Style = MsoButtonStyle.msoButtonIconAndCaption;
            exampleMenuItem.Caption = "Modifier L'identification du champ";
            exampleMenuItem.Click += new Microsoft.Office.Core._CommandBarButtonEvents_ClickEventHandler(exampleMenuItemClick);
        }

    

        void exampleMenuItemClick(Microsoft.Office.Core.CommandBarButton Ctrl, ref bool CancelDefault)
        {
            FrmTargetEdit frmNoCode = new FrmTargetEdit();
            frmNoCode.ShowDialog();
        }



        private void Ribbon_Load(object sender, RibbonUIEventArgs e)
        {
            ResetCellMenu();  // reset the cell context menu back to the default

            // Call this function is the user right clicks on a cell
            Globals.ThisAddIn.Application.SheetBeforeRightClick += new Microsoft.Office.Interop.Excel.AppEvents_SheetBeforeRightClickEventHandler(Application_SheetBeforeRightClick);

            //initializeDriveByCommandList();
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
            List<AxaFrance.WebEngine.Variable> parameters = new List<AxaFrance.WebEngine.Variable>();
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
                        parameters.Add(new AxaFrance.WebEngine.Variable()
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
            List<string> exportedNames, duplicatedNames;
            TestSuiteData list = InitializeTestDataList(filter, filterType, out exportedNames, out duplicatedNames);

            if (list == null)
            {
                return;
            }

            if (duplicatedNames.Count > 0)
            {
                MessageBox.Show(string.Format("There are duplicated test cases: {0}\nPlease fix the issue before export can be executed.", string.Join(", ", duplicatedNames.ToArray())), "Test data Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            list.TestDataList.RemoveAll(x => !exportedNames.Contains(x.TestName));

            SaveTestDataToFile(list);
        }

        private TestSuiteData InitializeTestDataList(string filter, bool filterType, out List<string> exportedNames, out List<string> duplicatedNames)
        {
            var filters = filter.Split(';');
            exportedNames = new List<string>();
            duplicatedNames = new List<string>();
            Workbook workbook = Globals.ThisAddIn.Application.ActiveWorkbook;
            Worksheet worksheet = workbook.ActiveSheet;
            categoryName = worksheet.Name;
            if (!Settings.ExcludedTabs.Contains(categoryName))
            {
                int maxColumns = worksheet.UsedRange.Columns.Count;
                TestSuiteData list = GetSelectedTestDataList(filterType, filters, exportedNames, duplicatedNames, worksheet);
                return list;
            }
            else
            {
                return null;
            }
        }

        public TestSuiteData GetSelectedTestDataList(bool filterType, string[] filters, List<string> exportedNames, List<string> duplicatedNames, Worksheet worksheet)
        {
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
            return list;
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
            List<AxaFrance.WebEngine.Variable>[] testcases_parameters = new List<AxaFrance.WebEngine.Variable>[testcase_count];
            for (int i = 0; i < testcase_count; i++)
            {
                testcases[i] = new TestData();
                testcases_parameters[i] = new List<AxaFrance.WebEngine.Variable>();
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
                        testcases_parameters[i].Add(new AxaFrance.WebEngine.Variable()
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
                isNoCode = false;
                FrmRun run = new FrmRun(categoryName, isNoCode);
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
            List<AxaFrance.WebEngine.Variable> parameters = new List<AxaFrance.WebEngine.Variable>();
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

                    // End generation 
                    if (string.IsNullOrWhiteSpace(paramname) && string.IsNullOrWhiteSpace(description))
                    {
                        break;
                    }
                    //Ignore all names has white-spaces
                    if (paramname.Contains(' ')) continue;

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
            Process.Start("https://github.com/AxaFrance/webengine-dotnet");
        }

        private void btnFeedback_Click(object sender, RibbonControlEventArgs e)
        {
            Process.Start("https://github.com/AxaFrance/webengine-dotnet/issues");
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
                List<AxaFrance.WebEngine.Variable> parameters = new List<AxaFrance.WebEngine.Variable>();
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

        private void btnStartDriveByExcelNow_Click(object sender, RibbonControlEventArgs e)
        {
            try
            {
                noPopup = true;
                btnExportEnvironmentVariable_Click(null, null);
                btnExportSelection_Click(null, null);
                FrmRun run = new FrmRun(categoryName, isNoCode);
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

        public static class Prompt
        {
            public static Boolean ShowDialog(string text, string moreText, string caption)
            {
                Form prompt = new Form()
                {
                    Width = 650,
                    Height = 150,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    Text = caption,
                    StartPosition = FormStartPosition.CenterScreen
                };
                Label textLabel = new Label() { Left = 10, Top = 10, Width = 600, Height = 20, Text = text };
                Label moreTextLabel = new Label() { Left = 10, Top = 30, Width = 600, Height = 20, Text = moreText };
                moreTextLabel.Font = new System.Drawing.Font(Label.DefaultFont, FontStyle.Bold);
                Button confirmation = new Button() { Text = "Ok", Left = 350, Width = 100, Top = 70, DialogResult = DialogResult.OK };
                Button cancel = new Button() { Text = "Annuler", Left = 150, Width = 100, Top = 70, DialogResult = DialogResult.Cancel };
                confirmation.Click += (sender, e) => { prompt.Close(); };
                prompt.Controls.Add(textLabel);
                prompt.Controls.Add(moreTextLabel);
                prompt.Controls.Add(confirmation);
                prompt.Controls.Add(cancel);
                prompt.AcceptButton = confirmation;

                return prompt.ShowDialog() == DialogResult.OK;
            }
        }

        private void btnStartDriveByExcelNow_Click_1(object sender, RibbonControlEventArgs e)
        {
            //initializeDriveByCommandList();
            isNoCode = true;

            String col = "";

            Worksheet worksheet = Globals.ThisAddIn.Application.ActiveSheet;
            Range selectedrange = Globals.ThisAddIn.Application.Selection;
            int colCOunt = selectedrange.Columns.Count;
            if (colCOunt > 10)
            {
                MessageBox.Show("Vous avez sélectionné plus de 10 colonnes, veuillez réduire le nombre de tests sélectionnés SVP.");
                return;
            }
            
            show_FrmRun(col);

        }

        private void show_FrmRun(string noCodeColInfo)
        {
            try
            {
                noPopup = true;
                if (!isNoCode)
                {
                    btnExportEnvironmentVariable_Click(null, null);
                    btnExportSelection_Click(null, null);
                }
                List<string> list = new List<string>();
                List<string> selected = new List<string>();
                FrmRun run = new FrmRun(categoryName, isNoCode, noCodeColInfo);
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

        private static void initializeDriveByCommandList()
        {
            //Prompt Excecution bar
            String workingDirectory = System.Environment.GetFolderPath(SpecialFolder.ApplicationData) + "\\AxaFrance.WebEngine";
            Form form = new Form();
            form.Width = 700;
            form.Height = 150;
            FlowLayoutPanel prompt = new FlowLayoutPanel();
            prompt.Width = 700;
            prompt.Height = 100;
            form.Text = "Chargement de la liste des commandes...";
            Label textLabel = new Label() { Width = 600, Text = "Téléchargement des dernières commandes" };
            prompt.Controls.Add(textLabel);
            ProgressBar dwl = new ProgressBar();
            dwl.Show();
            prompt.Controls.Add(dwl);
            form.Controls.Add(prompt);
            form.Show();
            dwl.PerformLayout();

            //Check and init values sheet First line
            Worksheet s = Globals.ThisAddIn.Application.ActiveSheet;
            if (!s.Name.Equals(Drive_Help_SheetName))
            {
                updateCell(s.Cells[1, 1], "Nom du champ", "Nom du champ");
                updateCell(s.Cells[1, 2], "Commande/Action", "liste des commandes possibles");
                updateCell(s.Cells[1, 3], "Identification du champ", "Identification du champ (Id, Xpath, ...)");
                updateCell(s.Cells[1, 4], "Optionnel", "Indique si la ligne est ignoré en cas d'erreur");
                updateCell(s.Cells[1, 5], "Référence/Exclusion", "Indique si la ligne est à prendre en compte par rapport à la colonne des valeurs choisie.\n Un '!' indique que la ligne est à ignorée");
                updateCell(s.Cells[1, 6], "Liste_1_des_valeurs", "Colonne des valeurs de votre JDD, vous pouvez modifier le nom de cette colonne");
                updateCell(s.Cells[1, 7], "Liste 2 des valeurs", "Colonne des valeurs de votre JDD, vous pouvez modifier le nom de cette colonne");

            }
            dwl.PerformLayout();


            //Donwload command file and init Command List Column 
            Dictionary<string, string> cmds = new Dictionary<string, string>(); //new string[] { "open", "send keys", "click", "select", "call", "if", "else if", "else", "end if", "save data", "assert exist", "assert selected", "assert content", "assert checked", "screenshot", "wait", "end scenario" };

            string cmdFile = FrmRun.getNoCodeRunnerFiles(workingDirectory, ((int)FrmRun.GetJarOrCmdYaml.cmdYaml), dwl);
            using (var reader = new StreamReader(cmdFile))
            {
                // Load the stream
                YamlStream yaml = new YamlDotNet.RepresentationModel.YamlStream();
                yaml.Load(reader);                

                for (int i = 0; i < yaml.Documents.First().AllNodes.Count(); i++)
                {
                    try
                    {
                        string cmd = yaml.Documents.First().AllNodes.First()[yaml.Documents.First().AllNodes.ElementAt(i)]["VALUE"][Ribbon.Settings.NoCodeCmdLangage.ToUpper()].ToString();
                        string desc = yaml.Documents.First().AllNodes.First()[yaml.Documents.First().AllNodes.ElementAt(i)]["DESCRIPTION"].ToString();

                        if (desc.ToLowerInvariant().Contains(NoCodeCmdLangage.French.ToString().ToLowerInvariant()) 
                            && desc.ToLowerInvariant().Contains(NoCodeCmdLangage.English.ToString().ToLowerInvariant()))
                        {
                            desc = yaml.Documents.First().AllNodes.First()[yaml.Documents.First().AllNodes.ElementAt(i)]["DESCRIPTION"][Ribbon.Settings.NoCodeCmdLangage.ToUpper()].ToString();
                        }
                        if (!cmds.ContainsKey(cmd))
                        {
                            cmds.Add(cmd, desc);
                        }
                    }
                    catch (Exception)
                    {
                        //Nothing
                    }
                }

            }

            dwl.PerformStep();
            form.Dispose();

            //Optional list Column validation
            Range rng = Globals.ThisAddIn.Application.ActiveSheet.Columns[4];
            string[] options = null;
            
            options = new string[] { "optional", "optional and depends on previous", "" };            

            updateRangeValidation(rng, options);

            //Create or Update Help Sheet
            Worksheet newHelp = null;
            try
            {
                newHelp = Globals.ThisAddIn.Application.ActiveWorkbook.Worksheets[Drive_Help_SheetName];
            }
            catch (Exception)
            {
                newHelp = Globals.ThisAddIn.Application.ActiveWorkbook.Worksheets.Add();
                newHelp.Name = Drive_Help_SheetName;
            }


            updateCell(newHelp.Cells[1, 1], "Commandes", null);
            newHelp.Cells[1, 1].ColumnWidth = 20;
            updateCell(newHelp.Cells[1, 2], "Description", null);
            newHelp.Cells[1, 2].ColumnWidth = 100;
            updateCell(newHelp.Cells[1, 4], "Initialisation des colonnes", null);
            newHelp.Cells[2, 4] = "En cliquant sur le bouton (1), la 1ere ligne de la feuille courante sera initiatlisé avec les valeurs en (2). Vous pouvez ensuite";
            newHelp.Cells[3, 4] = "Vous pouvez ensuite écrire votre scénario puis l'exécuter avec le lanceur NoCode (3)";

            updateCell(newHelp.Cells[12, 4], "Identification du champ", null);
            newHelp.Cells[18, 4] = "Nous vous recommandons d'utiliser les ID pour identifier les champs";
            newHelp.Cells[19, 4] = "Lorsqu'il n'y a pas d'ID fixes, et qu'une combinaison d'attributs n'est pas possibles, vous pouvez récupérer le xpath ainsi :";
            newHelp.Cells[20, 4] = "'-Clic droit sur l’élément(1) puis cliquer sur Inspecter(2)";

            newHelp.Cells[45, 4] = "'- Puis, répéter l'opération 1 et 2 si vous n'êtes pas directement sur l'élement comme indiqué en (3) ci-dessous";
            newHelp.Cells[46, 4] = "'- Puis clic droit sur l'élément souhaité (3) , puis Copier le Xpath (4 et 5)";
            newHelp.Cells[47, 4] = "'- Eviter au maximum la copie du Xpayh complet";



            //Command description
            int commandIndex = 1;
            for (commandIndex = 1; commandIndex <= cmds.Count; commandIndex++)
            {
                string cmd = cmds.Keys.ElementAt(commandIndex - 1);
                newHelp.Cells[commandIndex + 1, 1] = cmd;
                newHelp.Cells[commandIndex + 1, 2] = cmds[cmd];
            }

            newHelp.Cells[commandIndex + 1, 1] = "Fichier Exemple d'utilisation des commandes";
            newHelp.Cells[commandIndex + 1, 2].Formula = "=HYPERLINK(\"https://axa365.sharepoint.com/:x:/s/automateamcommunauteautomaticiens/EXqarYlHmM5FjuKSLr0AZpABBQBibhg0VkGvh1PiYhDx6A?e=fLfh9E\")";

            //Command list validation
            rng = Globals.ThisAddIn.Application.ActiveSheet.Columns[2];
            updateRangeValidation(rng, cmds.Keys.ToArray());

            //Identification help images
            string initbuttonFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AxaFrance.WebEngine", "initButtonHelp.png");
            string inspectHelpFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AxaFrance.WebEngine", "inspectHelp.png");
            string getxpathFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AxaFrance.WebEngine", "getxpath.png");
            Resources.initButtonpng.Save(initbuttonFile);
            addDriveHelpImage(initbuttonFile, newHelp, 4, 4);
            Resources.inspect.Save(inspectHelpFile);
            addDriveHelpImage(inspectHelpFile, newHelp, 23, 4);
            Resources.getxpath.Save(getxpathFile);
            addDriveHelpImage(getxpathFile, newHelp, 47, 4);
            UpdateToKeePassHelp(false);
        }


        public static void UpdateToKeePassHelp(bool gotoHelp)
        {
            Worksheet newHelp = null;
            try
            {
                newHelp = Globals.ThisAddIn.Application.ActiveWorkbook.Worksheets[Drive_Help_SheetName];
                updateCell(newHelp.Cells[65, 4], "Utilisation de variables Keepass : #{variable}# ou #{/Sous-Groupe/variable_sous_groupe}#", null);
                newHelp.Cells[1, 4].ColumnWidth = 120;

                String keepassHelp = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AxaFrance.WebEngine", "KeePass Value.png");
                String keepassHelpSousGroup = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AxaFrance.WebEngine", "KeePass sous-groupe Value.png");
                Resources.KeePass_Value.Save(keepassHelp);
                addDriveHelpImage(keepassHelp, newHelp, 66, 4);
                Resources.KeePass_sous_groupe_Value.Save(keepassHelpSousGroup);
                addDriveHelpImage(keepassHelpSousGroup, newHelp, 80, 4);
                if (gotoHelp)
                {
                    newHelp.Activate();
                    newHelp.Application.ActiveWindow.ScrollRow = 65;
                }
            }
            catch (Exception)
            {
                initializeDriveByCommandList();
            }

        }

        private static void gotoDriveHelp()
        {
            Worksheet newHelp = null;
            try
            {
                newHelp = Globals.ThisAddIn.Application.ActiveWorkbook.Worksheets[Drive_Help_SheetName];
                newHelp.Activate();
                newHelp.Application.ActiveWindow.ScrollRow = 1;
            }
            catch (Exception)
            {
                initializeDriveByCommandList();
            }
        }

        private static void addDriveHelpImage(string initbuttonFile, Worksheet newHelp, int rowPosition, int colPosition)
        {
            object missing = System.Reflection.Missing.Value;
            Microsoft.Office.Interop.Excel.Range picPosition = newHelp.Cells[rowPosition, colPosition]; // retrieve the range for picture insert
            Microsoft.Office.Interop.Excel.Pictures pc = newHelp.Pictures(missing) as Microsoft.Office.Interop.Excel.Pictures;
            Microsoft.Office.Interop.Excel.Picture pic = null;
            pic = pc.Insert(initbuttonFile, missing);
            pic.Left = Convert.ToDouble(picPosition.Left);
            pic.Top = Convert.ToDouble(picPosition.Top);
            pic.Placement = Microsoft.Office.Interop.Excel.XlPlacement.xlMove;
        }

        private static void updateCell(Range cell, string name, string description)
        {
            try
            {
                if (String.IsNullOrEmpty(cell.FormulaLocal))
                {
                    cell.FormulaLocal = name;
                }
                cell.ClearComments();
                if (description != null)
                {
                    cell.AddComment(description);
                }
                cell.Interior.Color = Color.FromArgb(68, 114, 196);
                if (cell.Column == 1 || cell.Column == 3)
                {
                    cell.ColumnWidth = 40;
                }
                else
                {
                    cell.ColumnWidth = 20;
                }
                cell.Font.Color = Color.White;
                cell.Font.Bold = true;
            }
            catch (Exception)
            {


            }

        }

        private static void updateRangeValidation(Range rng, string[] values)
        {
            rng.Validation.Delete();
            if (values.Length < 10)
            {
                rng.Validation.Add(XlDVType.xlValidateList, XlDVAlertStyle.xlValidAlertWarning, XlFormatConditionOperator.xlEqual, Formula1: string.Join(";", values));
            }
            else
            {
                rng.Validation.Add(XlDVType.xlValidateList, XlDVAlertStyle.xlValidAlertInformation, XlFormatConditionOperator.xlEqual, Formula1: "='"+ Drive_Help_SheetName + "'!$A$2:$A$" + (values.Length+1));
            }
            rng.Validation.ErrorTitle = "Value Error!!";
            rng.Validation.ErrorMessage = "Please select a value from dropdown list.";

            rng.Validation.ShowError = false;
            rng.Validation.InCellDropdown = true;
            rng.Validation.IgnoreBlank = true;
        }

        private void driveSettings_Click(object sender, RibbonControlEventArgs e)
        {
            isNoCode = true;
            Worksheet worksheet = Globals.ThisAddIn.Application.ActiveSheet;
            chooseLanguage();
            initializeDriveByCommandList();
        }

        private void chooseLanguage()
        {
            Form chooseLangageForm = new Form();
            chooseLangageForm.Width = 600;
            chooseLangageForm.Height = 110;
            FlowLayoutPanel prompt = new FlowLayoutPanel();
            prompt.Width = 600;
            prompt.Height = 100;
            chooseLangageForm.Text = "Choix de la langue des commandes";
            System.Windows.Forms.Button closebtn = new Button();
            closebtn.Text = "Done";

            System.Windows.Forms.ListBox langageList = new System.Windows.Forms.ListBox();
            langageList.Width = 400;
            langageList.AccessibilityObject.Name = "Langage";
            langageList.Items.Add(NoCodeCmdLangage.French);
            langageList.Items.Add(NoCodeCmdLangage.English);
            if (Ribbon.Settings.NoCodeCmdLangage == null)
            {
                langageList.SelectedIndex = 0;
            }                
            else if (Ribbon.Settings.NoCodeCmdLangage == NoCodeCmdLangage.French.ToString())
            {
                langageList.SelectedIndex = 0;
            }
            else
            {
                langageList.SelectedIndex = 1;
            }
            closebtn.Click += (btnsender, args) =>
            {
                Ribbon.Settings.NoCodeCmdLangage = langageList.SelectedItem.ToString();
                chooseLangageForm.Close();
            };
            chooseLangageForm.Controls.Add(prompt);

            langageList.KeyPress += new KeyPressEventHandler(chooseLangage_KeyPress);
            prompt.Controls.Add(langageList);   
            prompt.Controls.Add(closebtn);


            chooseLangageForm.ShowDialog();
            langageList.Focus();
        }

        private void chooseLangage_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                Ribbon.Settings.NoCodeCmdLangage = ((System.Windows.Forms.ListBox)sender).SelectedItem.ToString(); 
                //close parent form
                ((Form)((System.Windows.Forms.ListBox)sender).Parent.Parent).Close();
            }
        }

        private void BtTargetHelp_Click(object sender, RibbonControlEventArgs e)
        {
            FrmTargetEdit frmNoCode = new FrmTargetEdit();
            frmNoCode.ShowDialog();
        }

        private void KeepassHelp_Click(object sender, RibbonControlEventArgs e)
        {
            Ribbon.UpdateToKeePassHelp(true);
        }

        private void BtGotoDriveHelp_Click(object sender, RibbonControlEventArgs e)
        {
            Ribbon.gotoDriveHelp();
        }
    }
}
