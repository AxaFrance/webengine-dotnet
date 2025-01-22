// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using System;
using System.IO;
using System.Windows.Forms;

namespace AxaFrance.WebEngine.ExcelUI
{
    public partial class FrmRun : Form
    {

        public FrmRun(string tab)
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            Ribbon.SaveSettings();
            string assembly = Ribbon.Settings.TestAssembly;

            if (string.IsNullOrEmpty(assembly))
            {
                MessageBox.Show("Test Assembly is not set. Settings dialog will open, please set it in Settings and launch again the test.");
                FrmSettings fs = new FrmSettings();
                fs.ShowDialog();
                return;
            };


            BrowserType browser = GetSelectedBrowser(out string platform, out string appid, out string device);
            Ribbon.Settings.Browser = browser;
            string parameters = BuildParameter(browser, assembly, platform, device, appid);


            bool success = DetermineWebRunner(parameters, out string commandline, out string param);

            if (success)
            {
                System.Diagnostics.Process p = new System.Diagnostics.Process()
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo()
                    {
                        FileName = commandline,
                        Arguments = param,
                        WorkingDirectory = Ribbon.Settings.WebRunnerPath,
                    }
                };
                p.Start();
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            }
            else
            {
                this.DialogResult = System.Windows.Forms.DialogResult.Abort;
            }
        }

        private string BuildParameter(BrowserType browser, string assembly, string platform, string device, string appid)
        {
            string env = Ribbon.EnvironmentVariableFile;
            string testdata = Ribbon.TestDataFile;


            string parameters = string.Format(@"-a:{0} ""-data:{1}"" ""-env:{2}"" ""-browser:{3}""",
                assembly,
                testdata,
                env,
                browser
                );

            if (device != null)
            {
                parameters += $" \"-device:{device}\" \"-platform:{platform}\"";
                if (!string.IsNullOrWhiteSpace(appid))
                {
                    parameters += $" \"-appid:{appid}\"";
                }
                Ribbon.Settings.Device = device;
                Ribbon.Settings.AppId = appid;
            }


            if (cbManual.Checked)
            {
                parameters += " -m";
            }

            if (cbShowReport.Checked)
            {
                parameters += " -showreport";
            }

            return parameters;
        }

        private bool DetermineWebRunner(string parameters, out string commandline, out string param)
        {
            DirectoryInfo di = new DirectoryInfo(Ribbon.Settings.WebRunnerPath);
            var f = di.GetFiles("WebRunner.exe");
            if (f == null || f.Length == 0)
            {
                //Not .Net version/
                f = di.GetFiles("WebRunner.jar");
                if (f == null || f.Length == 0)
                {
                    MessageBox.Show($"WebRunner.exe or WebRunner.jar is not found in the given folder:\n{Ribbon.Settings.WebRunnerPath}\nPlease configure correctly in Settings.");
                    Ribbon.SaveSettings();
                    commandline = String.Empty;
                    param = String.Empty;
                    return false;
                }
                else
                {
                    param = "-jar webrunner.jar " + parameters;
                    commandline = getJavaExePath(Ribbon.Settings.WebRunnerPath);
                    return true;
                }

            }
            else
            {
                param = parameters;
                commandline = "WebRunner.exe";
                return true;
            }
        }

        /// <summary>
        /// This functions gets the java.exe path.
        /// If a jre is provided in the webrunner.jar folder, that version of jre will be used instead of default jre specified in the path.
        /// </summary>
        /// <param name="webRunnerPath">path of the webrunner.jar</param>
        /// <returns>the commandline of java.exe to be used.</returns>
        private string getJavaExePath(string webRunnerPath)
        {
            FileInfo fi = new FileInfo(Path.Combine(webRunnerPath, "jre", "bin", "java.exe"));
            if (fi.Exists)
            {
                return fi.FullName;
            }
            else
            {
                return "java.exe";
            }
        }

        private BrowserType GetSelectedBrowser(out string platform, out string appid, out string device)
        {
            BrowserType browser;
            platform = appid = device = null;
            if (rbIE.Checked)
            {
                browser = BrowserType.InternetExplorer;
            }
            else if (rbFirefox.Checked)
            {
                browser = BrowserType.Firefox;
            }
            else if (rbEdge.Checked)
            {
                browser = BrowserType.ChromiumEdge;
            }
            else if (rbAndroidNative.Checked)
            {
                browser = BrowserType.AndroidNative;
                device = txtDeviceName.Text;
                appid = txtAppPackage.Text;
                platform = "Android";
            }
            else if (rbIOSNative.Checked)
            {
                browser = BrowserType.IOSNative;
                device = txtDeviceName.Text;
                appid = txtAppPackage.Text;
                platform = "iOS";
            }
            else if (rbChrome.Checked)
            {
                browser = BrowserType.Chrome;
            }
            else if (rbChromeAndroid.Checked)
            {
                browser = BrowserType.Chrome;
                platform = "Android";
                device = txtDeviceName.Text;
            }
            else if (rbSafari.Checked)
            {
                browser = BrowserType.Safari;
                platform = "iOS";
            }
            else
            {
                browser = BrowserType.ChromiumEdge;
            }
            return browser;
        }

        private void FrmRun_Load(object sender, EventArgs e)
        {
            string message;
            if (Ribbon.TestCases.Count > 2)
            {
                message = Ribbon.TestCases[0] + ", " + Ribbon.TestCases[1] + " & " + (Ribbon.TestCases.Count - 2) + " & ...";
            }
            else
            {
                message = string.Join(" & ", Ribbon.TestCases);
            }
            lblSelectedTests.Text = message;
            switch (Ribbon.Settings.Browser)
            {
                case BrowserType.InternetExplorer:
                    rbIE.Checked = true;
                    break;
                case BrowserType.Firefox:
                    rbFirefox.Checked = true;
                    break;
                case BrowserType.Chrome:
                    rbChrome.Checked = true;
                    break;
                case BrowserType.IOSNative:
                    rbIOSNative.Checked = true;
                    break;
                case BrowserType.AndroidNative:
                    rbAndroidNative.Checked = true;
                    break;
                case BrowserType.Safari:
                    rbSafari.Checked = true;
                    break;
                case BrowserType.ChromiumEdge:
                default:
                    rbEdge.Checked = true;
                    break;
            }

            if (Ribbon.Settings.Device != null && Ribbon.Settings.Browser == BrowserType.Chrome)
            {
                rbChromeAndroid.Checked = true;
            }
        }

    }
}
