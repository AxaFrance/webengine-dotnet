// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Security;
using static System.Environment;
using System.Security.Cryptography;
using System.Reflection;

namespace AxaFrance.WebEngine.ExcelUI
{
    public partial class FrmRun : Form
    {
        private const string NocodeSuffixRepository = "\\AxaFrance.WebEngine\\WebRunnerJar";
        private bool isNoCode = false;
        private string noCodeColInfo = null;
        private bool useTempFIleForNocode = false;


        public FrmRun(string tab, bool noCodeByExcel, bool isNoCodeSolaris)
        {
            isNoCode = noCodeByExcel;
            InitializeComponent();
        }

        public FrmRun(string tab, bool noCodeByExcel)
        {
            isNoCode = noCodeByExcel;
            InitializeComponent();
        }

        public FrmRun(string tab, bool noCodeByExcel, bool isNoCodeSolaris, string noCodeColInfo)
        {
            isNoCode = noCodeByExcel;
            this.noCodeColInfo = noCodeColInfo;
            InitializeComponent();
        }

        public FrmRun(string tab, bool noCodeByExcel, string noCodeColInfo)
        {
            isNoCode = noCodeByExcel;
            this.noCodeColInfo = noCodeColInfo;
            InitializeComponent();
        }

        public static void InitializeTestCasesList(CheckedListBox cbListeTestsCases, System.Windows.Forms.Label lblSelectedTests, Dictionary<String, int> colNameIndex)
        {
            lblSelectedTests.Text = "La liste des colonnes (tests) possibles s'arrêtent à la 1ere colonne avec titre vide!";
            cbListeTestsCases.CheckOnClick = true;

            Range selectedrange = Globals.ThisAddIn.Application.Selection;
            dynamic activeSheet = Globals.ThisAddIn.Application.ActiveSheet;

            for (int i = 6; i <= 16; i++)
            {
                
                String value = activeSheet.Cells[1,i].FormulaLocal;
                if (!String.IsNullOrEmpty(value))
                {
                    cbListeTestsCases.Items.Add(value);
                    if (colNameIndex != null)
                    {
                        colNameIndex.Add(value, i);
                    }
                }
                else
                {
                    break;
                }
            }
                
            foreach (Range cr in selectedrange.Columns)
            {
                if (cr.Column >= 6)
                {
                    string value = Globals.ThisAddIn.Application.ActiveSheet.Cells[1, cr.Column].FormulaLocal;
                    int selectedItemIndex = cr.Column - 5;
                    if (selectedItemIndex <= cbListeTestsCases.Items.Count)
                    {
                        cbListeTestsCases.SetItemChecked(selectedItemIndex-1, true);
                    }
                    else
                    {
                        cbListeTestsCases.SetItemChecked(0, true);
                    }
                }
            }

            if (cbListeTestsCases.CheckedItems.Count <= 0)
            {
                cbListeTestsCases.SetItemChecked(0, true);
            }
           // cbListeTestsCases.PerformLayout();

        }

        private void btnStart_Click(object sender, EventArgs e)
        {            
            string assembly = Ribbon.Settings.TestAssembly;

            if (string.IsNullOrEmpty(assembly) && !isNoCode)
            {
                MessageBox.Show("Test Assembly is not set. Settings dialog will open, please set it in Settings and launch again the test.");
                FrmSettings fs = new FrmSettings();
                fs.ShowDialog();
                return;
            }


            BrowserType browser = GetSelectedBrowser(out string platform, out string appid, out string device);
            if (browser == 0)
            {
                MessageBox.Show("Veuillez sélectionner un navigateur svp!");
                return;
            }
            Ribbon.Settings.Browser = browser;
            string parameters = "";

            if (isNoCode)
            {
                Microsoft.Office.Interop.Excel.Workbook workbook = Globals.ThisAddIn.Application.ActiveWorkbook;
                string outputDir = txtOutputFolder.Text;
                Ribbon.Settings.NoCodeExportPath = outputDir;

                if (outputDir.EndsWith("\\"))
                {
                    outputDir = outputDir.Substring(0, outputDir.Length - 1);
                }
                String currentPath = "";

                try
                {
                    currentPath = Path.Combine(GetWorkingDirectory(outputDir), "temp");
                    Directory.CreateDirectory(currentPath);
                    string currentDateTime = DateTime.Now.ToString("yyyyMMddHHmmss");
                    currentPath = Path.Combine(currentPath, workbook.Name + currentDateTime);
                    workbook.SaveCopyAs(currentPath);
                    useTempFIleForNocode = true;
                }
                catch { 
                    currentPath = Path.Combine(workbook.Path,workbook.Name); 
                    useTempFIleForNocode = false;
                }
                
                parameters = BuildParameter(browser, assembly, platform, device, appid, currentPath, outputDir);
            }
            else
            {
                parameters = BuildParameter(browser, assembly, platform, device, appid, null, null);
            }

            if (String.IsNullOrEmpty(parameters))
            {
                return;
            }

            bool success = DetermineWebRunner(parameters, out string commandline, out string param);


            if (success)
            {
                Process p = new System.Diagnostics.Process()
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo()
                    {
                        FileName = commandline,
                        Arguments = param,
                        WorkingDirectory = isNoCode? Ribbon.Settings.NoCodeRunnerPath : Ribbon.Settings.WebRunnerPath,
                        UseShellExecute = true,
                    }
                };
                p.Start();
                this.DialogResult = System.Windows.Forms.DialogResult.OK;
            }
            else
            {
                this.DialogResult = System.Windows.Forms.DialogResult.Abort;
            }
            Ribbon.Settings.GetNoCodeBetaRunnerFile = cbGetBeta.Checked;
            Ribbon.SaveSettings();
            
        }

        private static string checkEnvInDir(string output)
        {
            if (!String.IsNullOrEmpty(output))
            {
                string value = System.Environment.GetEnvironmentVariable("appdata");
                output = output.Replace("%appdata%", value);
            }

            return output;
        }

        private string BuildParameter(BrowserType browser, string assembly, string platform, string device, string appid, string noCodeTempFile, string outputdir)
        {

            string env = Ribbon.EnvironmentVariableFile;
            string testdata = Ribbon.TestDataFile;
            string parameters = "";

           
            if (isNoCode)
            {
                dwlrobotLabel.Visible = true;
                dwlProgressBar.Visible = true;
                Microsoft.Office.Interop.Excel.Workbook workbook = Globals.ThisAddIn.Application.ActiveWorkbook;
                StringBuilder selectedCol = new StringBuilder();
                foreach (var item in cbListeTestsCases.CheckedItems)
                {
                    selectedCol.Append(item.ToString()).Append(";");
                }
                if (selectedCol.Length == 0){
                    MessageBox.Show("Veuillez Selectionner un colonne de données ayant un titre svp!");
                    return "";
                }
                selectedCol =  selectedCol.Remove(selectedCol.Length - 1, 1);

                noCodeColInfo = workbook.ActiveSheet.Name + "[-dataColumnName:" + selectedCol + "]";

               

                parameters = string.Format(@"""-data:{0}"" ""-tc:{1}""",
                    noCodeTempFile,
                    noCodeColInfo
                    );

                
                if (!string.IsNullOrEmpty(outputdir))
                {
                    parameters += " \"-outputDir:" + checkEnvInDir(outputdir) + "\"";
                }
                if (cbShowReport.Checked)
                {
                    parameters += " \"-showReport:true\"";
                }
                Ribbon.Settings.ShowReport = cbShowReport.Checked;

                parameters += " \"-closeBrowser:" + cbCloseBrowser.Checked + "\"";
                Ribbon.Settings.CloseBrowserAfterTest = cbCloseBrowser.Checked;

                if (!String.IsNullOrEmpty(txtPropertiesFile.Text))
                {
                    parameters += "\"--spring.config.location = file:///" + checkEnvInDir(txtPropertiesFile.Text)+"\"";
                }
                else
                {
                    parameters += " \"-browser:" + browser + "\"";
                }
                Ribbon.Settings.PropertiesFilePath = txtPropertiesFile.Text;

                if (txtKeepassFile.Visible && !String.IsNullOrEmpty(txtKeepassPassword.Text))
                {
                    parameters += " \"-keepassFile:" + checkEnvInDir(txtKeepassFile.Text) + "\"";
                    parameters += " \"-keepassPassword:" + txtKeepassPassword.Text + "\"";
                }
                Ribbon.Settings.enckp = Encrypt(txtKeepassFile.Text);
                Ribbon.Settings.KeepassFilePath = "empty";
                Ribbon.Settings.enc = Encrypt(txtKeepassPassword.Text);

                if (useTempFIleForNocode)
                {
                    parameters += " \"-deleteTempFile:true\"";
                }
            }
            else
            {
                driveSettingLayout.Visible = false;
                parameters = string.Format(@"-a:{0} ""-data:{1}"" ""-env:{2}"" ""-browser:{3}""",
                   assembly,
                   testdata,
                   env,
                   browser
                   );

            }
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

            if (cbShowReport.Checked && !isNoCode)
            {
                parameters += " -showreport";
            }

            Ribbon.SaveSettings();
            return parameters;
        }

        private bool DetermineWebRunner(string parameters, out string commandline, out string param)
        {
            if (isNoCode)
            {
                dwlProgressBar.PerformLayout();
                getNoCodeRunnerFiles(txtOutputFolder.Text, ((int)GetJarOrCmdYaml.jar), cbGetBeta.Checked, dwlProgressBar, cbForce.Checked);
                param = "-jar webrunner.jar " + parameters;
                commandline = getJavaExePath(Ribbon.Settings.NoCodeRunnerPath);
                return true;
            }
            
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

        public enum GetJarOrCmdYaml : ushort
        {
            jar = 0,
            cmdYaml = 1
        }

        public static string getNoCodeRunnerFiles(String outputDir, int jarOrCmdYaml, bool getBeta, ProgressBar dwlProgressBar, bool ForceDownload)
        {
            dwlProgressBar.PerformStep();
            string noCodeArtifactPath = ConfigurationManager.AppSettings.Get("noCodeArtifactPath");
            string mavenRepoUrl = ConfigurationManager.AppSettings.Get("noCodeMavenRepository");
            string mavenSnapRepoUrl = ConfigurationManager.AppSettings.Get("noCodeMavenSnapshotRepository");
            string runnerDirectLink = ConfigurationManager.AppSettings.Get("runnerDirectLink");
            string commandDirectLink = ConfigurationManager.AppSettings.Get("commandDirectLink");
            string workingDirectory = GetWorkingDirectory(outputDir);
            string folder = workingDirectory ?? "";
            string jarSubFolder = "\\WebRunnerJar";
            if (!folder.EndsWith(jarSubFolder))
            {
                folder += jarSubFolder;
            }
            Ribbon.Settings.NoCodeRunnerPath = folder;
            Directory.CreateDirectory(folder);
            dwlProgressBar.PerformStep();
            WebClient client = new WebClient();
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls | 
                System.Net.SecurityProtocolType.Tls11 | 
                System.Net.SecurityProtocolType.Tls12 | 
                System.Net.SecurityProtocolType.Ssl3;

            if (!string.IsNullOrEmpty(noCodeArtifactPath) && !string.IsNullOrEmpty(mavenRepoUrl) && !noCodeArtifactPath.Contains("#{") && !mavenRepoUrl.Contains("#{"))
            {
                string mvnSelectedRepo = (getBeta ? mavenSnapRepoUrl : mavenRepoUrl);
                if (!mvnSelectedRepo.EndsWith("/"))
                {
                    mvnSelectedRepo += "/";
                }
                string nodeSnaptshot2 = "";
                XmlDocument doc = new XmlDocument();
                dwlProgressBar.PerformStep();
                string localmetadatafile = folder + "\\metadata.xml";
                string metadata = mvnSelectedRepo + noCodeArtifactPath + "/maven-metadata.xml";
                if (!File.Exists(localmetadatafile) || new FileInfo(localmetadatafile).Length <= 0)
                {
                    client.DownloadFile(metadata, localmetadatafile);
                }
                XmlDocument localmetadata = new XmlDocument();
                localmetadata.Load(localmetadatafile);
                Stream stream = client.OpenRead(metadata);
                doc.Load(stream);
                dwlProgressBar.PerformStep();
                int nodeCount = doc.DocumentElement.SelectNodes("/metadata/versioning/versions/version").Count;
                string nodeSnaptshot = doc.DocumentElement.SelectNodes("/metadata/versioning/versions/version")[nodeCount - 1].InnerText;
                bool downloadfile = false;
                if (ForceDownload)
                {
                    //MessageBox.Show("ForceDownload launch ok");
                    if (getBeta)
                    {
                        nodeSnaptshot2 = LoadSnapShotMetaData(noCodeArtifactPath, mvnSelectedRepo, doc, localmetadatafile, out metadata, localmetadata, nodeSnaptshot, out downloadfile);
                    }
                    client.DownloadFile(metadata, localmetadatafile);
                    downloadfile = true;
                }
                else if (getBeta)
                {
                    nodeSnaptshot2 = LoadSnapShotMetaData(noCodeArtifactPath, mvnSelectedRepo, doc, localmetadatafile, out metadata, localmetadata, nodeSnaptshot, out downloadfile);
                }
                else if (!string.IsNullOrEmpty(localmetadata.InnerText) && !localmetadata.OuterXml.Equals(doc.OuterXml))
                {
                    //MessageBox.Show("not beta download launch ok");
                    client.DownloadFile(metadata, localmetadatafile);
                    downloadfile = true;
                }
                string fileUrl = mvnSelectedRepo + noCodeArtifactPath + "/" + nodeSnaptshot + "/webengine-drive-by-excel-" + (getBeta ? nodeSnaptshot2 : nodeSnaptshot);
                string file = "";
                if (jarOrCmdYaml == 0)
                {
                    fileUrl += "-exec.jar";
                    file = folder + "\\webrunner.jar";
                    //MessageBox.Show("download value before check=" + downloadfile);
                    downloadfile = checkDownloadForJar(ForceDownload, downloadfile, file);
                }
                else
                {
                    fileUrl += "-command.yaml";
                    file = folder + "\\command.yaml";
                    downloadfile = true;
                }
                if (downloadfile)
                {
                    dwlProgressBar.PerformStep();
                    client.DownloadFile(fileUrl, file);
                    dwlProgressBar.PerformStep();
                }
                client.Dispose();
                return file;
            }
            if (jarOrCmdYaml == 1)
            {
                string commandfile = folder + "\\command.yaml";
                if (!File.Exists(commandfile) || File.GetLastWriteTime(commandfile).CompareTo(DateTime.Now.AddDays(-7.0)) <= 0 || (new FileInfo(commandfile)).Length<=0)
                {
                    dwlProgressBar.PerformStep();
                    client.DownloadFile(commandDirectLink, commandfile);
                    dwlProgressBar.Value = dwlProgressBar.Maximum;
                }
                client.Dispose();
                return commandfile;
            }
            string runnerfile = folder + "\\webrunner.jar";
            if (!File.Exists(runnerfile) || File.GetCreationTime(runnerfile).CompareTo(DateTime.Now.AddDays(-7.0)) <= 0)
            {
                dwlProgressBar.PerformStep();
                client.DownloadFile(runnerDirectLink, runnerfile);
                dwlProgressBar.Value = dwlProgressBar.Maximum;
            }
            client.Dispose();
            return runnerfile;
        }

        private static String LoadSnapShotMetaData(string noCodeArtifactPath, 
            string mvnSelectedRepo,  XmlDocument doc, 
            string localmetadatafile, out string metadata, XmlDocument localmetadata, 
            string nodeSnaptshot, out bool downloadfile)
        {
            metadata = mvnSelectedRepo + noCodeArtifactPath + "/" + nodeSnaptshot + "/maven-metadata.xml";
            WebClient client = new WebClient();
            doc.Load(client.OpenRead(metadata));
            downloadfile = false;
            //MessageBox.Show("beta localmetadata.InnerText: " + localmetadata.InnerText);
            //MessageBox.Show("beta localmetadata.OuterXml: " + localmetadata.OuterXml);
            if (!string.IsNullOrEmpty(localmetadata.InnerText) && !localmetadata.OuterXml.Equals(doc.OuterXml))
            {
                //MessageBox.Show("beta download launch ok");
                client.DownloadFile(metadata, localmetadatafile);
                localmetadata.Load(localmetadatafile);
                client.Dispose();
                downloadfile = true;
            }
            return localmetadata.DocumentElement.SelectSingleNode("/metadata/versioning/snapshotVersions/snapshotVersion[1]/value").InnerText;
        }

        private static bool checkDownloadForJar(bool Force, bool downloadfile, string file)
        {
            if (Force)
            {
                return true;
            }
            bool dwl = downloadfile ? downloadfile : (!File.Exists(file) || new FileInfo(file).Length <= 0 || File.GetLastWriteTime(file).CompareTo(DateTime.Now.AddDays(-3.0)) < 0);
            //MessageBox.Show("checkDownloadForJar launch ok" +" file date="+File.GetCreationTime(file).CompareTo(DateTime.Now.AddDays(-3.0)) +" file exist="+ File.Exists(file)+" file lenght="+ new FileInfo(file).Length + "download ="+dwl);
            return dwl;
        }


        public static string GetWorkingDirectory(string outputDir)
        {
            string workingDirectory = "";

            if (String.IsNullOrEmpty(outputDir))
            {
                workingDirectory = System.Environment.GetFolderPath(SpecialFolder.ApplicationData) + NocodeSuffixRepository;
            }
            else
            {                
                workingDirectory = checkEnvInDir(outputDir) + (outputDir.EndsWith(NocodeSuffixRepository) ? "" : NocodeSuffixRepository);
            }

            return workingDirectory;
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
                //check java exixstence in the current machine
                Process p = new System.Diagnostics.Process()
                {
                    StartInfo = new System.Diagnostics.ProcessStartInfo()
                    {
                        FileName = "java.exe",
                        Arguments = "-v"
                    }
                };
                try
                {
                    p.Start();
                }
                catch (Exception)
                {
                    MessageBox.Show("Veuillez svp installer une version de java!");
                }
                return "java.exe";
            }
        }

        private BrowserType GetSelectedBrowser(out string platform, out string appid, out string device)
        {
            BrowserType browser;
            platform = appid = device = null;
            if (rbIE.Checked)
            {
#pragma warning disable CS0618 // Le type ou le membre est obsolète
                browser = BrowserType.InternetExplorer;
#pragma warning restore CS0618 // Le type ou le membre est obsolète
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
            if (isNoCode)
            {
                message = noCodeColInfo;
                driveSettingLayout.Visible = true;
                txtOutputFolder.Text = Ribbon.Settings.NoCodeExportPath;
                txtPropertiesFile.Text = Ribbon.Settings.PropertiesFilePath;

                cbManual.Visible = false;
                cbManual.Checked = false;
                appLayout.Visible = false;
                devicesLayout.Visible = false;
                cbCloseBrowser.Visible = true;
                cbShowReport.Visible = true;
                cbCloseBrowser.Checked = Ribbon.Settings.CloseBrowserAfterTest;
                //cbShowReport.Checked = Ribbon.Settings.ShowReport;
                String encKpPath = Ribbon.Settings.enckp;
                String notEncKpPath = Ribbon.Settings.KeepassFilePath;
                if (String.IsNullOrEmpty(encKpPath))
                {
                    txtKeepassFile.Text = Ribbon.Settings.KeepassFilePath;
                }
                else
                {
                    txtKeepassFile.Text = Decrypt(encKpPath);
                }
                txtKeepassPassword.Text = Decrypt(Ribbon.Settings.enc);
                cbGetBeta.Checked = Ribbon.Settings.GetNoCodeBetaRunnerFile;
                InitializeTestCasesList(cbListeTestsCases, lblSelectedTests, null);
            }
            else
            {
                cbManual.Visible = true;
                appLayout.Visible = true;
                devicesLayout.Visible = true;
                cbCloseBrowser.Visible = false;
                cbListeTestsCases.Visible = false;
                cbForce.Visible = false;
                dwlrobotLabel.Visible = false;
                cbGetBeta.Visible = false;

                driveSettingLayout.Visible = false;
                if (Ribbon.TestCases.Count > 2)
                {
                    message = Ribbon.TestCases[0] + ", " + Ribbon.TestCases[1] + " & " + (Ribbon.TestCases.Count - 2) + " & ...";
                }
                else
                {
                    message = string.Join(" & ", Ribbon.TestCases);
                }
            }
            lblSelectedTests.Text = message;

#pragma warning disable CS0618 // Le type ou le membre est obsolète
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
                default:
                    rbEdge.Checked = true;
                    break;
            }
#pragma warning restore CS0618 // Le type ou le membre est obsolète

            if (Ribbon.Settings.Device != null && Ribbon.Settings.Browser == BrowserType.Chrome)
            {
                rbChromeAndroid.Checked = true;
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            var result = folderDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                txtOutputFolder.Text = folderDialog.SelectedPath;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var result = openFileDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                txtPropertiesFile.Text = openFileDialog.FileName;
            }
        }

        private void keepassFileLocate_Click(object sender, EventArgs e)
        {
            var result = openFileDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                txtKeepassFile.Text = openFileDialog.FileName;
                Ribbon.Settings.KeepassFilePath = openFileDialog.FileName;
            }
        }

        /// <summary>
        /// Encrypts a given password and returns the encrypted data
        /// as a base64 string.
        /// </summary>
        /// <param name="plainText">An unencrypted string that needs
        /// to be secured.</param>
        /// <returns>A base64 encoded string that represents the encrypted
        /// binary data.
        /// </returns>
        /// <remarks>This solution is not really secure as we are
        /// keeping strings in memory. If runtime protection is essential,
        /// <see cref="SecureString"/> should be used.</remarks>
        /// <exception cref="ArgumentNullException">If <paramref name="plainText"/>
        /// is a null reference.</exception>
        public string Encrypt(string plainText)
        {
            if (plainText == null) return "";

            //encrypt data
            var data = Encoding.Unicode.GetBytes(plainText);
            byte[] encrypted = ProtectedData.Protect(data, null, DataProtectionScope.CurrentUser);

            //return as base64 string
            return Convert.ToBase64String(encrypted);
        }

        /// <summary>
        /// Decrypts a given string.
        /// </summary>
        /// <param name="cipher">A base64 encoded string that was created
        /// through the <see cref="Encrypt(string)"/> or
        /// <see cref="Encrypt(SecureString)"/> extension methods.</param>
        /// <returns>The decrypted string.</returns>
        /// <remarks>Keep in mind that the decrypted string remains in memory
        /// and makes your application vulnerable per se. If runtime protection
        /// is essential, <see cref="SecureString"/> should be used.</remarks>
        /// <exception cref="ArgumentNullException">If <paramref name="cipher"/>
        /// is a null reference.</exception>
        public string Decrypt(string cipher)
        {
            if (cipher == null) return "";

            //parse base64 string
            byte[] data = Convert.FromBase64String(cipher);

            //decrypt data
            byte[] decrypted = ProtectedData.Unprotect(data, null, DataProtectionScope.CurrentUser);
            return Encoding.Unicode.GetString(decrypted);
        }

    }
}
