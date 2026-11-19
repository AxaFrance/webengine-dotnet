// Copyright (c) 2016-2026 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace AxaFrance.WebEngine.ExcelAddin
{
    internal sealed class FrmAbout : Form
    {
        internal FrmAbout()
        {
            UiTheme.StyleForm(this, "About AXA WebEngine Excel Add-in", new Size(480, 300));

            var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown";

            var lblTitle = new Label
            {
                Text = "AXA WebEngine Excel Add-in",
                ForeColor = UiTheme.Header,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                Location = new Point(20, 20),
                Size = new Size(430, 30),
            };

            var lblVersion = new Label
            {
                Text = $"Version {version}",
                Location = new Point(22, 56),
                Size = new Size(430, 20),
            };

            var lblDescription = new Label
            {
                Text = "Manage WebEngine test data and launch data-driven test runs directly from Excel.",
                Location = new Point(22, 88),
                Size = new Size(430, 40),
            };

            var lblDna = new Label
            {
                Text = "Built with Excel-DNA (zlib license).",
                Location = new Point(22, 132),
                Size = new Size(430, 20),
            };

            var linkProject = new LinkLabel
            {
                Text = "Project home",
                Location = new Point(22, 166),
                Size = new Size(110, 20),
            };
            linkProject.LinkClicked += (_, _) => OpenUrl("https://github.com/AxaFrance/webengine-dotnet");

            var linkIssues = new LinkLabel
            {
                Text = "Report an issue",
                Location = new Point(140, 166),
                Size = new Size(110, 20),
            };
            linkIssues.LinkClicked += (_, _) => OpenUrl("https://github.com/AxaFrance/webengine-dotnet/issues");

            var linkLicense = new LinkLabel
            {
                Text = "License",
                Location = new Point(258, 166),
                Size = new Size(80, 20),
            };
            linkLicense.LinkClicked += (_, _) => OpenUrl("https://github.com/AxaFrance/webengine-dotnet/blob/develop/LICENCE.md");

            var btnClose = UiTheme.CreateButton("Close", 370, 250, 90, 30);
            btnClose.DialogResult = DialogResult.OK;

            AcceptButton = btnClose;
            CancelButton = btnClose;

            Controls.AddRange(new Control[]
            {
                lblTitle,
                lblVersion,
                lblDescription,
                lblDna,
                linkProject,
                linkIssues,
                linkLicense,
                btnClose,
            });
        }

        private static void OpenUrl(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unable to open the link", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
