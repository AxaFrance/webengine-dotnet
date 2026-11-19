// Copyright (c) 2016-2026 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Windows.Forms;

namespace AxaFrance.WebEngine.ExcelAddin
{
    internal sealed class FrmFilter : Form
    {
        private readonly TextBox _txtFilter = new();
        private readonly RadioButton _rbExportMatches = new();

        internal FrmFilter()
        {
            UiTheme.StyleForm(this, "Export With Filter", new Size(470, 230));

            var header = UiTheme.CreateHeader("Filter test cases by name", 16, 12, 430);

            var lblFilter = UiTheme.CreateFieldLabel("Filter terms (separated by semicolons):", 16, 45, 430);

            _txtFilter.Location = new Point(16, 68);
            _txtFilter.Size = new Size(438, 23);

            var group = new GroupBox
            {
                Text = "Filter mode",
                ForeColor = UiTheme.Header,
                BackColor = Color.White,
                Location = new Point(16, 105),
                Size = new Size(438, 80),
            };

            _rbExportMatches.Text = "Export test cases that match";
            _rbExportMatches.Location = new Point(16, 22);
            _rbExportMatches.Size = new Size(220, 22);
            _rbExportMatches.Checked = true;

            var rbExcludeMatches = new RadioButton
            {
                Text = "Export test cases that do not match",
                Location = new Point(16, 48),
                Size = new Size(260, 22),
            };

            group.Controls.AddRange(new Control[] { _rbExportMatches, rbExcludeMatches });

            var btnOk = UiTheme.CreateButton("Export", 264, 198, 90, 30);
            btnOk.DialogResult = DialogResult.OK;
            var btnCancel = UiTheme.CreateButton("Cancel", 364, 198, 90, 30);
            btnCancel.DialogResult = DialogResult.Cancel;

            AcceptButton = btnOk;
            CancelButton = btnCancel;

            Controls.AddRange(new Control[] { header, lblFilter, _txtFilter, group, btnOk, btnCancel });
        }

        internal string FilterText => _txtFilter.Text.Trim();

        internal bool ExportMatches => _rbExportMatches.Checked;
    }
}
