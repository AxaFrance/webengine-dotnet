// Copyright (c) 2016-2026 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Diagnostics;
using System.Windows.Forms;
using ExcelDna.Integration.CustomUI;

namespace AxaFrance.WebEngine.ExcelAddin
{
    public partial class ExcelAddin
    {
        private const string WebRunnerProcessName = "WebRunner";

        /// <summary>
        /// Exports the selected test data and launches WebRunner.
        /// </summary>
        public void OnLaunch(IRibbonControl control)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Settings.TestAssembly))
                {
                    ShowWarning(
                        "The test assembly is not configured. The Settings dialog will open; set the test assembly and try again.");
                    OnSettings(control);
                    return;
                }

                ValidateRunSettings();

                SilentExport = true;
                try
                {
                    if (!ExportEnvironment(showDialog: false) || !ExportSelected(showDialog: false))
                    {
                        return;
                    }
                }
                finally
                {
                    SilentExport = false;
                }

                if (string.IsNullOrWhiteSpace(TestDataFile) || string.IsNullOrWhiteSpace(EnvironmentVariableFile))
                {
                    ShowWarning("Test data was not exported. The test will not be launched.");
                    return;
                }

                using var form = new FrmRun(Settings, TestCases);
                if (form.ShowDialog() != DialogResult.OK || form.Options == null)
                {
                    return;
                }

                var options = form.Options;
                Settings.Browser = options.Browser;
                if (options.Device != null)
                {
                    Settings.Device = options.Device;
                }
                if (options.AppId != null)
                {
                    Settings.AppId = options.AppId;
                }
                SaveSettings();

                string arguments = RunArgumentsBuilder.BuildArguments(
                    Settings.TestAssembly,
                    TestDataFile,
                    EnvironmentVariableFile,
                    options.Browser,
                    options.Platform,
                    options.Device,
                    options.AppId,
                    options.ManualMode,
                    options.ShowReport);

                var command = RunArgumentsBuilder.ResolveRunner(Settings.WebRunnerPath, arguments);
                Process.Start(new ProcessStartInfo
                {
                    FileName = command.FileName,
                    Arguments = command.Arguments,
                    WorkingDirectory = command.WorkingDirectory,
                    UseShellExecute = true,
                });
            }
            catch (Exception ex)
            {
                ShowError("Unable to launch the test", ex);
            }
        }

        /// <summary>
        /// Stops all running WebRunner processes.
        /// </summary>
        public void OnStop(IRibbonControl control)
        {
            try
            {
                var processes = Process.GetProcessesByName(WebRunnerProcessName);
                if (processes.Length == 0)
                {
                    ShowInfo("No WebRunner process is currently running.");
                    return;
                }

                var answer = MessageBox.Show(
                    "Stopping the current test will terminate all running tests. Do you want to continue?",
                    "Stop tests",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2);
                if (answer != DialogResult.Yes)
                {
                    return;
                }

                foreach (var process in processes)
                {
                    try
                    {
                        if (!process.HasExited)
                        {
                            process.Kill(entireProcessTree: true);
                            process.WaitForExit(5000);
                        }
                    }
                    finally
                    {
                        process.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Unable to stop the tests", ex);
            }
        }

        private void ValidateRunSettings()
        {
            if (string.IsNullOrWhiteSpace(Settings.ExportPath))
            {
                throw new InvalidOperationException("The export folder is not configured. Open Settings first.");
            }

            if (string.IsNullOrWhiteSpace(Settings.WebRunnerPath))
            {
                throw new InvalidOperationException("The WebRunner folder is not configured. Open Settings first.");
            }
        }
    }
}
