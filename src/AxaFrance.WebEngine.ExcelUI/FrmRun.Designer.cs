// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
namespace AxaFrance.WebEngine.ExcelUI
{
    partial class FrmRun
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmRun));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblSelectedTests = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.cbManual = new System.Windows.Forms.CheckBox();
            this.rbIOSNative = new System.Windows.Forms.RadioButton();
            this.rbAndroidNative = new System.Windows.Forms.RadioButton();
            this.rbEdge = new System.Windows.Forms.RadioButton();
            this.rbChrome = new System.Windows.Forms.RadioButton();
            this.rbFirefox = new System.Windows.Forms.RadioButton();
            this.rbIE = new System.Windows.Forms.RadioButton();
            this.txtDeviceName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtAppPackage = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cbShowReport = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.rbChromeAndroid = new System.Windows.Forms.RadioButton();
            this.rbSafari = new System.Windows.Forms.RadioButton();
            this.appLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.dwlProgressBar = new System.Windows.Forms.ProgressBar();
            this.dwlrobotLabel = new System.Windows.Forms.Label();
            this.devicesLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.label8 = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.driveSettingLayout = new System.Windows.Forms.FlowLayoutPanel();
            this.label9 = new System.Windows.Forms.Label();
            this.txtOutputFolder = new System.Windows.Forms.TextBox();
            this.btnOutputBrowse = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.txtPropertiesFile = new System.Windows.Forms.TextBox();
            this.btnPropFile = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.txtKeepassFile = new System.Windows.Forms.TextBox();
            this.keepassFileLocate = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.txtKeepassPassword = new System.Windows.Forms.TextBox();
            this.folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.cbCloseBrowser = new System.Windows.Forms.CheckBox();
            this.cbListeTestsCases = new System.Windows.Forms.CheckedListBox();
            this.cbGetBeta = new System.Windows.Forms.CheckBox();
            this.cbForce = new System.Windows.Forms.CheckBox();
            this.appLayout.SuspendLayout();
            this.devicesLayout.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.driveSettingLayout.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.ForeColor = System.Drawing.Color.SteelBlue;
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.ForeColor = System.Drawing.Color.SteelBlue;
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.ForeColor = System.Drawing.Color.SteelBlue;
            this.label3.Name = "label3";
            // 
            // lblSelectedTests
            // 
            resources.ApplyResources(this.lblSelectedTests, "lblSelectedTests");
            this.lblSelectedTests.Name = "lblSelectedTests";
            // 
            // btnStart
            // 
            this.btnStart.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnStart.FlatAppearance.BorderColor = System.Drawing.Color.Gainsboro;
            this.btnStart.FlatAppearance.BorderSize = 2;
            this.btnStart.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gainsboro;
            this.btnStart.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSkyBlue;
            resources.ApplyResources(this.btnStart, "btnStart");
            this.btnStart.ForeColor = System.Drawing.Color.Black;
            this.btnStart.Name = "btnStart";
            this.btnStart.UseVisualStyleBackColor = false;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // cbManual
            // 
            resources.ApplyResources(this.cbManual, "cbManual");
            this.cbManual.Checked = true;
            this.cbManual.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbManual.Name = "cbManual";
            this.cbManual.UseVisualStyleBackColor = true;
            // 
            // rbIOSNative
            // 
            this.rbIOSNative.BackgroundImage = global::AxaFrance.WebEngine.ExcelUI.Properties.Resources.ios;
            resources.ApplyResources(this.rbIOSNative, "rbIOSNative");
            this.rbIOSNative.Name = "rbIOSNative";
            this.rbIOSNative.UseVisualStyleBackColor = false;
            // 
            // rbAndroidNative
            // 
            this.rbAndroidNative.BackgroundImage = global::AxaFrance.WebEngine.ExcelUI.Properties.Resources.android;
            resources.ApplyResources(this.rbAndroidNative, "rbAndroidNative");
            this.rbAndroidNative.Name = "rbAndroidNative";
            this.rbAndroidNative.UseVisualStyleBackColor = false;
            // 
            // rbEdge
            // 
            this.rbEdge.BackgroundImage = global::AxaFrance.WebEngine.ExcelUI.Properties.Resources.edge;
            resources.ApplyResources(this.rbEdge, "rbEdge");
            this.rbEdge.Name = "rbEdge";
            this.rbEdge.UseVisualStyleBackColor = false;
            // 
            // rbChrome
            // 
            this.rbChrome.BackgroundImage = global::AxaFrance.WebEngine.ExcelUI.Properties.Resources.chrome;
            resources.ApplyResources(this.rbChrome, "rbChrome");
            this.rbChrome.Name = "rbChrome";
            this.rbChrome.UseVisualStyleBackColor = false;
            // 
            // rbFirefox
            // 
            this.rbFirefox.BackgroundImage = global::AxaFrance.WebEngine.ExcelUI.Properties.Resources.Firefox;
            resources.ApplyResources(this.rbFirefox, "rbFirefox");
            this.rbFirefox.Name = "rbFirefox";
            this.rbFirefox.UseVisualStyleBackColor = false;
            // 
            // rbIE
            // 
            this.rbIE.BackgroundImage = global::AxaFrance.WebEngine.ExcelUI.Properties.Resources.ie;
            resources.ApplyResources(this.rbIE, "rbIE");
            this.rbIE.Checked = true;
            this.rbIE.Name = "rbIE";
            this.rbIE.TabStop = true;
            this.rbIE.UseVisualStyleBackColor = false;
            // 
            // txtDeviceName
            // 
            resources.ApplyResources(this.txtDeviceName, "txtDeviceName");
            this.txtDeviceName.Name = "txtDeviceName";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.ForeColor = System.Drawing.Color.SteelBlue;
            this.label4.Name = "label4";
            // 
            // txtAppPackage
            // 
            resources.ApplyResources(this.txtAppPackage, "txtAppPackage");
            this.txtAppPackage.Name = "txtAppPackage";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.ForeColor = System.Drawing.Color.SteelBlue;
            this.label5.Name = "label5";
            // 
            // cbShowReport
            // 
            resources.ApplyResources(this.cbShowReport, "cbShowReport");
            this.cbShowReport.Checked = true;
            this.cbShowReport.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbShowReport.Name = "cbShowReport";
            this.cbShowReport.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // rbChromeAndroid
            // 
            this.rbChromeAndroid.BackgroundImage = global::AxaFrance.WebEngine.ExcelUI.Properties.Resources.chrome;
            resources.ApplyResources(this.rbChromeAndroid, "rbChromeAndroid");
            this.rbChromeAndroid.Name = "rbChromeAndroid";
            this.rbChromeAndroid.UseVisualStyleBackColor = false;
            // 
            // rbSafari
            // 
            this.rbSafari.BackgroundImage = global::AxaFrance.WebEngine.ExcelUI.Properties.Resources.safari;
            resources.ApplyResources(this.rbSafari, "rbSafari");
            this.rbSafari.Name = "rbSafari";
            this.rbSafari.UseVisualStyleBackColor = false;
            // 
            // appLayout
            // 
            this.appLayout.Controls.Add(this.label5);
            this.appLayout.Controls.Add(this.txtDeviceName);
            this.appLayout.Controls.Add(this.label4);
            this.appLayout.Controls.Add(this.txtAppPackage);
            resources.ApplyResources(this.appLayout, "appLayout");
            this.appLayout.Name = "appLayout";
            // 
            // dwlProgressBar
            // 
            resources.ApplyResources(this.dwlProgressBar, "dwlProgressBar");
            this.dwlProgressBar.Name = "dwlProgressBar";
            // 
            // dwlrobotLabel
            // 
            resources.ApplyResources(this.dwlrobotLabel, "dwlrobotLabel");
            this.dwlrobotLabel.Name = "dwlrobotLabel";
            this.dwlrobotLabel.UseWaitCursor = true;
            // 
            // devicesLayout
            // 
            this.devicesLayout.Controls.Add(this.label8);
            this.devicesLayout.Controls.Add(this.label7);
            this.devicesLayout.Controls.Add(this.rbSafari);
            this.devicesLayout.Controls.Add(this.rbChromeAndroid);
            this.devicesLayout.Controls.Add(this.rbIOSNative);
            this.devicesLayout.Controls.Add(this.rbAndroidNative);
            resources.ApplyResources(this.devicesLayout, "devicesLayout");
            this.devicesLayout.Name = "devicesLayout";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.ForeColor = System.Drawing.Color.Black;
            this.label8.Name = "label8";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.label6);
            this.flowLayoutPanel1.Controls.Add(this.rbIE);
            this.flowLayoutPanel1.Controls.Add(this.rbFirefox);
            this.flowLayoutPanel1.Controls.Add(this.rbChrome);
            this.flowLayoutPanel1.Controls.Add(this.rbEdge);
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // driveSettingLayout
            // 
            this.driveSettingLayout.Controls.Add(this.label9);
            this.driveSettingLayout.Controls.Add(this.txtOutputFolder);
            this.driveSettingLayout.Controls.Add(this.btnOutputBrowse);
            this.driveSettingLayout.Controls.Add(this.label10);
            this.driveSettingLayout.Controls.Add(this.txtPropertiesFile);
            this.driveSettingLayout.Controls.Add(this.btnPropFile);
            this.driveSettingLayout.Controls.Add(this.label11);
            this.driveSettingLayout.Controls.Add(this.txtKeepassFile);
            this.driveSettingLayout.Controls.Add(this.keepassFileLocate);
            this.driveSettingLayout.Controls.Add(this.label12);
            this.driveSettingLayout.Controls.Add(this.txtKeepassPassword);
            resources.ApplyResources(this.driveSettingLayout, "driveSettingLayout");
            this.driveSettingLayout.Name = "driveSettingLayout";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.ForeColor = System.Drawing.Color.SteelBlue;
            this.label9.Name = "label9";
            // 
            // txtOutputFolder
            // 
            resources.ApplyResources(this.txtOutputFolder, "txtOutputFolder");
            this.txtOutputFolder.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtOutputFolder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtOutputFolder.Name = "txtOutputFolder";
            // 
            // btnOutputBrowse
            // 
            resources.ApplyResources(this.btnOutputBrowse, "btnOutputBrowse");
            this.btnOutputBrowse.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnOutputBrowse.FlatAppearance.BorderColor = System.Drawing.Color.Gainsboro;
            this.btnOutputBrowse.FlatAppearance.BorderSize = 2;
            this.btnOutputBrowse.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightGray;
            this.btnOutputBrowse.Name = "btnOutputBrowse";
            this.btnOutputBrowse.UseVisualStyleBackColor = false;
            this.btnOutputBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.ForeColor = System.Drawing.Color.SteelBlue;
            this.label10.Name = "label10";
            // 
            // txtPropertiesFile
            // 
            resources.ApplyResources(this.txtPropertiesFile, "txtPropertiesFile");
            this.txtPropertiesFile.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtPropertiesFile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPropertiesFile.Name = "txtPropertiesFile";
            // 
            // btnPropFile
            // 
            resources.ApplyResources(this.btnPropFile, "btnPropFile");
            this.btnPropFile.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnPropFile.FlatAppearance.BorderColor = System.Drawing.Color.Gainsboro;
            this.btnPropFile.FlatAppearance.BorderSize = 2;
            this.btnPropFile.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightGray;
            this.btnPropFile.Name = "btnPropFile";
            this.btnPropFile.UseVisualStyleBackColor = false;
            this.btnPropFile.Click += new System.EventHandler(this.button1_Click);
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.ForeColor = System.Drawing.Color.SteelBlue;
            this.label11.Name = "label11";
            // 
            // txtKeepassFile
            // 
            resources.ApplyResources(this.txtKeepassFile, "txtKeepassFile");
            this.txtKeepassFile.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtKeepassFile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtKeepassFile.Name = "txtKeepassFile";
            // 
            // keepassFileLocate
            // 
            resources.ApplyResources(this.keepassFileLocate, "keepassFileLocate");
            this.keepassFileLocate.BackColor = System.Drawing.Color.WhiteSmoke;
            this.keepassFileLocate.FlatAppearance.BorderColor = System.Drawing.Color.Gainsboro;
            this.keepassFileLocate.FlatAppearance.BorderSize = 2;
            this.keepassFileLocate.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightGray;
            this.keepassFileLocate.Name = "keepassFileLocate";
            this.keepassFileLocate.UseVisualStyleBackColor = false;
            this.keepassFileLocate.Click += new System.EventHandler(this.keepassFileLocate_Click);
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.ForeColor = System.Drawing.Color.SteelBlue;
            this.label12.Name = "label12";
            // 
            // txtKeepassPassword
            // 
            resources.ApplyResources(this.txtKeepassPassword, "txtKeepassPassword");
            this.txtKeepassPassword.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtKeepassPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtKeepassPassword.Name = "txtKeepassPassword";
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog";
            // 
            // cbCloseBrowser
            // 
            resources.ApplyResources(this.cbCloseBrowser, "cbCloseBrowser");
            this.cbCloseBrowser.Checked = true;
            this.cbCloseBrowser.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbCloseBrowser.Name = "cbCloseBrowser";
            this.cbCloseBrowser.UseVisualStyleBackColor = true;
            // 
            // cbListeTestsCases
            // 
            this.cbListeTestsCases.CheckOnClick = true;
            this.cbListeTestsCases.FormattingEnabled = true;
            resources.ApplyResources(this.cbListeTestsCases, "cbListeTestsCases");
            this.cbListeTestsCases.Name = "cbListeTestsCases";
            this.cbListeTestsCases.ThreeDCheckBoxes = true;
            // 
            // cbGetBeta
            // 
            resources.ApplyResources(this.cbGetBeta, "cbGetBeta");
            this.cbGetBeta.Name = "cbGetBeta";
            this.cbGetBeta.UseVisualStyleBackColor = true;
            // 
            // cbForce
            // 
            resources.ApplyResources(this.cbForce, "cbForce");
            this.cbForce.Name = "cbForce";
            this.cbForce.UseVisualStyleBackColor = true;
            // 
            // FrmRun
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.cbForce);
            this.Controls.Add(this.cbGetBeta);
            this.Controls.Add(this.cbListeTestsCases);
            this.Controls.Add(this.cbCloseBrowser);
            this.Controls.Add(this.driveSettingLayout);
            this.Controls.Add(this.appLayout);
            this.Controls.Add(this.dwlrobotLabel);
            this.Controls.Add(this.dwlProgressBar);
            this.Controls.Add(this.cbShowReport);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.cbManual);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.devicesLayout);
            this.Controls.Add(this.lblSelectedTests);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FrmRun";
            this.Load += new System.EventHandler(this.FrmRun_Load);
            this.appLayout.ResumeLayout(false);
            this.appLayout.PerformLayout();
            this.devicesLayout.ResumeLayout(false);
            this.devicesLayout.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.driveSettingLayout.ResumeLayout(false);
            this.driveSettingLayout.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblSelectedTests;
        private System.Windows.Forms.RadioButton rbIE;
        private System.Windows.Forms.RadioButton rbFirefox;
        private System.Windows.Forms.RadioButton rbChrome;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.CheckBox cbManual;
        private System.Windows.Forms.RadioButton rbEdge;
        private System.Windows.Forms.RadioButton rbAndroidNative;
        private System.Windows.Forms.RadioButton rbIOSNative;
        private System.Windows.Forms.TextBox txtDeviceName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtAppPackage;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox cbShowReport;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.RadioButton rbChromeAndroid;
        private System.Windows.Forms.RadioButton rbSafari;
        private System.Windows.Forms.FlowLayoutPanel appLayout;
        private System.Windows.Forms.ProgressBar dwlProgressBar;
        private System.Windows.Forms.Label dwlrobotLabel;
        private System.Windows.Forms.FlowLayoutPanel devicesLayout;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel driveSettingLayout;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtOutputFolder;
        private System.Windows.Forms.Button btnOutputBrowse;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtPropertiesFile;
        private System.Windows.Forms.Button btnPropFile;
        private System.Windows.Forms.FolderBrowserDialog folderDialog;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtKeepassFile;
        private System.Windows.Forms.Button keepassFileLocate;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtKeepassPassword;
        private System.Windows.Forms.CheckBox cbCloseBrowser;
        private System.Windows.Forms.CheckedListBox cbListeTestsCases;
        private System.Windows.Forms.CheckBox cbGetBeta;
        private System.Windows.Forms.CheckBox cbForce;
    }
}