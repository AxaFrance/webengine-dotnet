// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using System;
using System.Windows.Forms;

namespace AxaFrance.WebEngine.ExcelUI
{
    public partial class FrmSettings : Form
    {
        public FrmSettings()
        {
            InitializeComponent();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            var result = folderDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string dir = folderDialog.SelectedPath;
                txtExportFolder.Text = dir;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Ribbon.Settings.ExportPath = txtExportFolder.Text + @"\";
            Ribbon.Settings.WebRunnerPath = txtWebRunnerFolder.Text + @"\";
            Ribbon.Settings.TestAssembly = txtTestAssembly.Text.Trim();
            Ribbon.SaveSettings();
            this.DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void FrmSettings_Load(object sender, EventArgs e)
        {
            txtExportFolder.Text = Ribbon.Settings.ExportPath;
            if (txtExportFolder.Text.EndsWith("\\"))
            {
                txtExportFolder.Text = txtExportFolder.Text.Substring(0, txtExportFolder.Text.Length - 1);
            }
            txtWebRunnerFolder.Text = Ribbon.Settings.WebRunnerPath;
            if (txtWebRunnerFolder.Text.EndsWith("\\"))
            {
                txtWebRunnerFolder.Text = txtWebRunnerFolder.Text.Substring(0, txtWebRunnerFolder.Text.Length - 1);
            }
            txtTestAssembly.Text = Ribbon.Settings.TestAssembly;
        }

        private void btnAnnuler_Click(object sender, EventArgs e)
        {
            this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }

        private void btnParcourirWebRunner_Click(object sender, EventArgs e)
        {
            var result = folderDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                string path = folderDialog.SelectedPath;
                txtWebRunnerFolder.Text = path;
            }
        }
    }
}
