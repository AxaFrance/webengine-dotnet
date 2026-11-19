// Copyright (c) 2016-2026 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using ExcelDna.Integration;

namespace AxaFrance.WebEngine.ExcelAddin
{
    public partial class ExcelAddin
    {
        /// <summary>
        /// Exports the active worksheet without prompting. Intended for automation and support.
        /// </summary>
        [ExcelCommand(
            Name = "WEBENGINE_EXPORT_CURRENT_SHEET",
            Description = "Exports every test case of the active worksheet using the configured export folder.")]
        public static void ExportCurrentSheetCommand()
        {
            (Instance ?? new ExcelAddin()).ExportSheetForAutomation();
        }

        /// <summary>
        /// Exports the test cases in the selected columns without prompting. Intended for automation and support.
        /// </summary>
        [ExcelCommand(
            Name = "WEBENGINE_EXPORT_SELECTED",
            Description = "Exports the test cases in the selected worksheet columns using the configured export folder.")]
        public static void ExportSelectedCommand()
        {
            (Instance ?? new ExcelAddin()).ExportSelectedForAutomation();
        }

        /// <summary>
        /// Exports the ENV worksheet without prompting. Intended for automation and support.
        /// </summary>
        [ExcelCommand(
            Name = "WEBENGINE_EXPORT_ENVIRONMENT",
            Description = "Exports the ENV worksheet using the configured export folder.")]
        public static void ExportEnvironmentCommand()
        {
            (Instance ?? new ExcelAddin()).ExportEnvironmentForAutomation();
        }

        private void ExportSelectedForAutomation()
        {
            try
            {
                ValidateExportSettings();
                ExportSelected(showDialog: false);
            }
            catch (Exception ex)
            {
                ShowError("Automatic selected test export failed", ex);
            }
        }

        private void ExportSheetForAutomation()
        {
            try
            {
                ValidateExportSettings();
                ExportSheet(filter: null, showDialog: false);
            }
            catch (Exception ex)
            {
                ShowError("Automatic test data export failed", ex);
            }
        }

        private void ExportEnvironmentForAutomation()
        {
            try
            {
                ValidateExportSettings();
                ExportEnvironment(showDialog: false);
            }
            catch (Exception ex)
            {
                ShowError("Automatic environment export failed", ex);
            }
        }
    }
}
