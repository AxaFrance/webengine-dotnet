// Copyright (c) 2016-2026 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Windows.Forms;
using ExcelDna.Integration.CustomUI;

namespace AxaFrance.WebEngine.ExcelAddin
{
    /// <summary>
    /// Excel-DNA ribbon entry point.
    /// </summary>
    public partial class ExcelAddin : ExcelRibbon
    {
        private const string AddinName = "AXA WebEngine";
        private const string ProjectUrl = "https://github.com/AxaFrance/webengine-dotnet";
        private const string IssuesUrl = "https://github.com/AxaFrance/webengine-dotnet/issues";

        private static readonly AddinSettingsStore SettingsStore = new();

        /// <summary>
        /// Creates the ribbon instance and exposes it to automation commands.
        /// </summary>
        public ExcelAddin()
        {
            Instance = this;
        }

        internal static ExcelAddin? Instance { get; private set; }

        internal static AddinSettings Settings { get; set; } = SettingsStore.Load();

        internal static string? TestDataFile { get; set; }

        internal static string? EnvironmentVariableFile { get; set; }

        internal static string? CategoryName { get; set; }

        internal static IReadOnlyList<string> TestCases { get; set; } = Array.Empty<string>();

        internal static bool SilentExport { get; set; }

        /// <inheritdoc/>
        public override string GetCustomUI(string RibbonID)
        {
            using var stream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("AxaFrance.WebEngine.ExcelAddin.Ribbon.xml");
            if (stream == null)
            {
                throw new InvalidOperationException("Embedded ribbon definition not found.");
            }

            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        /// <inheritdoc/>
        public override object LoadImage(string imageId)
        {
            if (string.Equals(imageId, "WebEngineStop", StringComparison.Ordinal))
            {
                return CreateStopIcon();
            }

            return base.LoadImage(imageId);
        }

        /// <summary>
        /// Opens the settings dialog.
        /// </summary>
        public void OnSettings(IRibbonControl control)
        {
            var edited = Settings.Clone();
            using var form = new FrmSettings(edited);
            if (form.ShowDialog() == DialogResult.OK)
            {
                Settings = edited;
                SaveSettings();
            }
        }

        /// <summary>
        /// Shows add-in information.
        /// </summary>
        public void OnAbout(IRibbonControl control)
        {
            using var form = new FrmAbout();
            form.ShowDialog();
        }

        /// <summary>
        /// Opens the online documentation.
        /// </summary>
        public void OnHelp(IRibbonControl control)
        {
            OpenUrl(ProjectUrl);
        }

        /// <summary>
        /// Opens the issue tracker.
        /// </summary>
        public void OnFeedback(IRibbonControl control)
        {
            OpenUrl(IssuesUrl);
        }

        internal static void SaveSettings()
        {
            try
            {
                SettingsStore.Save(Settings);
            }
            catch (Exception ex)
            {
                ShowError("Unable to save the add-in settings", ex);
            }
        }

        internal static void ShowError(string title, Exception exception)
        {
            Debug.WriteLine(exception);
            MessageBox.Show(
                $"{exception.Message}{Environment.NewLine}{Environment.NewLine}See the About page for support links.",
                title,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        internal static void ShowWarning(string message, string title = AddinName)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        internal static void ShowInfo(string message, string title = AddinName)
        {
            MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private static Bitmap CreateStopIcon()
        {
            var bitmap = new Bitmap(32, 32);
            using var graphics = Graphics.FromImage(bitmap);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            graphics.Clear(Color.Transparent);

            using var brush = new SolidBrush(Color.FromArgb(255, 23, 43));
            graphics.FillEllipse(brush, 1, 1, 30, 30);

            using var pen = new Pen(Color.White, 4F);
            graphics.DrawRectangle(pen, 10, 10, 12, 12);
            return bitmap;
        }

        private static void OpenUrl(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch (Exception ex)
            {
                ShowError("Unable to open the link", ex);
            }
        }
    }
}
