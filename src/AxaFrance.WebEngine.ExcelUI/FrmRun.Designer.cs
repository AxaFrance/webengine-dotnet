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
            this.txtDeviceName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtAppPackage = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cbShowReport = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.rbChromeAndroid = new System.Windows.Forms.RadioButton();
            this.rbSafari = new System.Windows.Forms.RadioButton();
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
            this.rbEdge.Checked = true;
            this.rbEdge.Name = "rbEdge";
            this.rbEdge.TabStop = true;
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
            // FrmRun
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.rbSafari);
            this.Controls.Add(this.rbChromeAndroid);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cbShowReport);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtAppPackage);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtDeviceName);
            this.Controls.Add(this.rbIOSNative);
            this.Controls.Add(this.rbAndroidNative);
            this.Controls.Add(this.rbEdge);
            this.Controls.Add(this.cbManual);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.rbChrome);
            this.Controls.Add(this.rbFirefox);
            this.Controls.Add(this.lblSelectedTests);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FrmRun";
            this.Load += new System.EventHandler(this.FrmRun_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblSelectedTests;
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
    }
}