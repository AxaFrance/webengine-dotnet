// Copyright (c) 2016-2026 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using AxaFrance.WebEngine;

namespace AxaFrance.WebEngine.ExcelAddin
{
    internal sealed class FrmRun : Form
    {
        private readonly List<Image> _images = new();
        private readonly RadioButton _rbEdge;
        private readonly RadioButton _rbChrome;
        private readonly RadioButton _rbFirefox;
        private readonly RadioButton _rbSafari;
        private readonly RadioButton _rbChromeAndroid;
        private readonly RadioButton _rbAndroidNative;
        private readonly RadioButton _rbIosNative;
        private readonly TextBox _txtDevice = new();
        private readonly TextBox _txtAppId = new();
        private readonly CheckBox _chkManualMode = new();
        private readonly CheckBox _chkShowReport = new();

        internal FrmRun(AddinSettings settings, IReadOnlyList<string> testCases)
        {
            UiTheme.StyleForm(this, "Run Selected Test", new Size(620, 610));

            _rbEdge = UiTheme.CreateBrowserTile("Edge", LoadTileImage("edge.png"), 14, 26);
            _rbChrome = UiTheme.CreateBrowserTile("Chrome", LoadTileImage("chrome.png"), 154, 26);
            _rbFirefox = UiTheme.CreateBrowserTile("Firefox", LoadTileImage("firefox.png"), 294, 26);
            _rbSafari = UiTheme.CreateBrowserTile("Safari", LoadTileImage("safari.png"), 434, 26);
            _rbChromeAndroid = UiTheme.CreateBrowserTile("Chrome (Android)", LoadTileImage("chrome.png"), 14, 156);
            _rbAndroidNative = UiTheme.CreateBrowserTile("Android application", LoadTileImage("android.png"), 154, 156);
            _rbIosNative = UiTheme.CreateBrowserTile("iOS application", LoadTileImage("ios.png"), 294, 156);

            BuildLayout(testCases);
            LoadSettings(settings);
            UpdateMobileFields();
        }

        internal RunOptions? Options { get; private set; }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                foreach (var image in _images)
                {
                    image.Dispose();
                }
                _images.Clear();
            }
        }

        private void BuildLayout(IReadOnlyList<string> testCases)
        {
            var header = UiTheme.CreateHeader("Select the browser or mobile platform", 16, 12, 560);

            var groupBrowser = new GroupBox
            {
                Text = "Browser / platform",
                ForeColor = UiTheme.Header,
                BackColor = Color.White,
                Location = new Point(16, 40),
                Size = new Size(588, 300),
            };

            foreach (var tile in new[] { _rbEdge, _rbChrome, _rbFirefox, _rbSafari, _rbChromeAndroid, _rbAndroidNative, _rbIosNative })
            {
                tile.CheckedChanged += (_, _) => UpdateMobileFields();
                groupBrowser.Controls.Add(tile);
            }

            var lblDevice = UiTheme.CreateFieldLabel("Device name", 16, 356);
            _txtDevice.Location = new Point(150, 353);
            _txtDevice.Size = new Size(454, 23);

            var lblAppId = UiTheme.CreateFieldLabel("Application id / path", 16, 392);
            _txtAppId.Location = new Point(150, 389);
            _txtAppId.Size = new Size(454, 23);

            _chkManualMode.Text = "Pause the test when it fails before cleaning up";
            _chkManualMode.Location = new Point(16, 426);
            _chkManualMode.Size = new Size(420, 22);

            _chkShowReport.Text = "Open the report when the test finishes";
            _chkShowReport.Location = new Point(16, 452);
            _chkShowReport.Size = new Size(420, 22);

            var lblSelected = UiTheme.CreateFieldLabel("Selected test cases", 16, 486, 180);
            string selectedText = testCases.Count == 0
                ? "(no test case exported)"
                : testCases.Count > 2
                    ? $"{testCases[0]}, {testCases[1]} and {testCases.Count - 2} more"
                    : string.Join(" & ", testCases);
            var txtSelected = new TextBox
            {
                Text = selectedText,
                ReadOnly = true,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Location = new Point(16, 508),
                Size = new Size(588, 40),
                BackColor = Color.White,
            };

            var btnStart = UiTheme.CreateButton("Launch", 410, 562, 94, 32);
            btnStart.Click += (_, _) => TryLaunch();

            var btnCancel = UiTheme.CreateButton("Cancel", 510, 562, 94, 32);
            btnCancel.DialogResult = DialogResult.Cancel;

            AcceptButton = btnStart;
            CancelButton = btnCancel;

            Controls.AddRange(new Control[]
            {
                header,
                groupBrowser,
                lblDevice,
                _txtDevice,
                lblAppId,
                _txtAppId,
                _chkManualMode,
                _chkShowReport,
                lblSelected,
                txtSelected,
                btnStart,
                btnCancel,
            });
        }

        private Bitmap? LoadTileImage(string fileName)
        {
            using var source = ResourceImages.Load(fileName);
            if (source == null)
            {
                return null;
            }

            var resized = new Bitmap(56, 56);
            using (var graphics = Graphics.FromImage(resized))
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.Clear(Color.Transparent);
                graphics.DrawImage(source, 0, 0, 56, 56);
            }

            _images.Add(resized);
            return resized;
        }

        private void LoadSettings(AddinSettings settings)
        {
            if (settings.Browser == BrowserType.Chrome && !string.IsNullOrWhiteSpace(settings.Device))
            {
                _rbChromeAndroid.Checked = true;
            }
            else
            {
                switch (settings.Browser)
                {
                    case BrowserType.Firefox:
                        _rbFirefox.Checked = true;
                        break;
                    case BrowserType.Chrome:
                        _rbChrome.Checked = true;
                        break;
                    case BrowserType.Safari:
                        _rbSafari.Checked = true;
                        break;
                    case BrowserType.AndroidNative:
                        _rbAndroidNative.Checked = true;
                        break;
                    case BrowserType.IOSNative:
                        _rbIosNative.Checked = true;
                        break;
                    case BrowserType.ChromiumEdge:
                    default:
                        _rbEdge.Checked = true;
                        break;
                }
            }

            _txtDevice.Text = settings.Device ?? string.Empty;
            _txtAppId.Text = settings.AppId ?? string.Empty;
        }

        private void UpdateMobileFields()
        {
            bool mobile = _rbChromeAndroid.Checked || _rbAndroidNative.Checked || _rbIosNative.Checked;
            _txtDevice.Enabled = mobile;
            _txtAppId.Enabled = mobile;
        }

        private void TryLaunch()
        {
            GetBrowser(out var browser, out var platform);

            bool requiresDevice = _rbChromeAndroid.Checked || _rbAndroidNative.Checked || _rbIosNative.Checked;
            string device = _txtDevice.Text.Trim();
            if (requiresDevice && string.IsNullOrWhiteSpace(device))
            {
                MessageBox.Show(
                    "A device name is required for mobile testing.",
                    "Run Selected Test",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                _txtDevice.Focus();
                return;
            }

            Options = new RunOptions
            {
                Browser = browser,
                Platform = platform,
                Device = requiresDevice ? device : null,
                AppId = requiresDevice ? _txtAppId.Text.Trim() : null,
                ManualMode = _chkManualMode.Checked,
                ShowReport = _chkShowReport.Checked,
            };

            DialogResult = DialogResult.OK;
            Close();
        }

        private void GetBrowser(out BrowserType browser, out string? platform)
        {
            platform = null;
            if (_rbChrome.Checked)
            {
                browser = BrowserType.Chrome;
            }
            else if (_rbFirefox.Checked)
            {
                browser = BrowserType.Firefox;
            }
            else if (_rbSafari.Checked)
            {
                browser = BrowserType.Safari;
            }
            else if (_rbChromeAndroid.Checked)
            {
                browser = BrowserType.Chrome;
                platform = "Android";
            }
            else if (_rbAndroidNative.Checked)
            {
                browser = BrowserType.AndroidNative;
                platform = "Android";
            }
            else if (_rbIosNative.Checked)
            {
                browser = BrowserType.IOSNative;
                platform = "iOS";
            }
            else
            {
                browser = BrowserType.ChromiumEdge;
            }
        }
    }
}
