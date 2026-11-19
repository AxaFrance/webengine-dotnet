// Copyright (c) 2016-2026 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Drawing;
using System.Windows.Forms;

namespace AxaFrance.WebEngine.ExcelAddin
{
    internal sealed class FrmSettings : Form
    {
        private readonly AddinSettings _settings;
        private readonly TextBox _txtExportFolder = new();
        private readonly TextBox _txtWebRunnerFolder = new();
        private readonly TextBox _txtTestAssembly = new();

        internal FrmSettings(AddinSettings settings)
        {
            _settings = settings;
            UiTheme.StyleForm(this, "WebEngine Settings", new Size(620, 270));

            BuildLayout();
            LoadSettings();
        }

        private void BuildLayout()
        {
            var header = UiTheme.CreateHeader("Configure the test automation solution", 16, 12, 560);

            AddRow("Export directory", _txtExportFolder, 50, BrowseExportFolder);
            AddRow("WebRunner directory", _txtWebRunnerFolder, 95, BrowseWebRunnerFolder);
            AddRow("Test assembly", _txtTestAssembly, 140, BrowseTestAssembly);

            var hint = new Label
            {
                Text = @"Settings are stored in %APPDATA%\AxaFrance.WebEngine\excelSetting.xml.",
                Location = new Point(16, 180),
                Size = new Size(560, 18),
                ForeColor = SystemColors.GrayText,
            };

            var btnOk = UiTheme.CreateButton("OK", 410, 220, 94, 32);
            btnOk.DialogResult = DialogResult.OK;
            btnOk.Click += (_, _) => ApplySettings();

            var btnCancel = UiTheme.CreateButton("Cancel", 510, 220, 94, 32);
            btnCancel.DialogResult = DialogResult.Cancel;

            AcceptButton = btnOk;
            CancelButton = btnCancel;

            Controls.AddRange(new Control[] { header, hint, btnOk, btnCancel });
        }

        private void AddRow(string label, TextBox textBox, int top, EventHandler browseHandler)
        {
            var lbl = UiTheme.CreateFieldLabel(label, 16, top + 3);
            textBox.Location = new Point(160, top);
            textBox.Size = new Size(350, 23);

            var btn = UiTheme.CreateButton("Browse...", 520, top - 2, 84, 28);
            btn.Click += browseHandler;

            Controls.AddRange(new Control[] { lbl, textBox, btn });
        }

        private void LoadSettings()
        {
            _txtExportFolder.Text = _settings.ExportPath;
            _txtWebRunnerFolder.Text = _settings.WebRunnerPath;
            _txtTestAssembly.Text = _settings.TestAssembly;
        }

        private void BrowseExportFolder(object? sender, EventArgs e)
        {
            using var dialog = new FolderBrowserDialog
            {
                Description = "Select the folder where XML files are exported.",
                SelectedPath = _txtExportFolder.Text,
                ShowNewFolderButton = true,
            };
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                _txtExportFolder.Text = dialog.SelectedPath;
            }
        }

        private void BrowseWebRunnerFolder(object? sender, EventArgs e)
        {
            using var dialog = new FolderBrowserDialog
            {
                Description = "Select the folder containing WebRunner.exe or WebRunner.jar.",
                SelectedPath = _txtWebRunnerFolder.Text,
                ShowNewFolderButton = false,
            };
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                _txtWebRunnerFolder.Text = dialog.SelectedPath;
            }
        }

        private void BrowseTestAssembly(object? sender, EventArgs e)
        {
            using var dialog = new OpenFileDialog
            {
                Title = "Select the test assembly",
                Filter = "Test assembly (*.dll;*.jar)|*.dll;*.jar|.NET assembly (*.dll)|*.dll|Java archive (*.jar)|*.jar|All files (*.*)|*.*",
                CheckFileExists = true,
            };
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                _txtTestAssembly.Text = dialog.FileName;
            }
        }

        private void ApplySettings()
        {
            _settings.ExportPath = _txtExportFolder.Text.Trim();
            _settings.WebRunnerPath = _txtWebRunnerFolder.Text.Trim();
            _settings.TestAssembly = _txtTestAssembly.Text.Trim();
        }
    }
}
