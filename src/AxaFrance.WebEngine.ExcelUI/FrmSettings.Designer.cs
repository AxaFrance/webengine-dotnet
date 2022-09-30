// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
namespace AxaFrance.WebEngine.ExcelUI
{
    partial class FrmSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmSettings));
            this.btnSave = new System.Windows.Forms.Button();
            this.btnAnnuler = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.txtExportFolder = new System.Windows.Forms.TextBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.txtWebRunnerFolder = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnParcourirWebRunner = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtTestAssembly = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnSave.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
            this.btnSave.FlatAppearance.BorderSize = 2;
            this.btnSave.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightGray;
            this.btnSave.Name = "btnSave";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnAnnuler
            // 
            resources.ApplyResources(this.btnAnnuler, "btnAnnuler");
            this.btnAnnuler.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnAnnuler.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnAnnuler.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
            this.btnAnnuler.FlatAppearance.BorderSize = 2;
            this.btnAnnuler.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightGray;
            this.btnAnnuler.Name = "btnAnnuler";
            this.btnAnnuler.UseVisualStyleBackColor = false;
            this.btnAnnuler.Click += new System.EventHandler(this.btnAnnuler_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.ForeColor = System.Drawing.Color.SteelBlue;
            this.label1.Name = "label1";
            // 
            // txtExportFolder
            // 
            resources.ApplyResources(this.txtExportFolder, "txtExportFolder");
            this.txtExportFolder.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtExportFolder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtExportFolder.Name = "txtExportFolder";
            // 
            // btnBrowse
            // 
            resources.ApplyResources(this.btnBrowse, "btnBrowse");
            this.btnBrowse.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnBrowse.FlatAppearance.BorderColor = System.Drawing.Color.Gainsboro;
            this.btnBrowse.FlatAppearance.BorderSize = 2;
            this.btnBrowse.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightGray;
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.UseVisualStyleBackColor = false;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtWebRunnerFolder
            // 
            resources.ApplyResources(this.txtWebRunnerFolder, "txtWebRunnerFolder");
            this.txtWebRunnerFolder.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtWebRunnerFolder.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtWebRunnerFolder.Name = "txtWebRunnerFolder";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.ForeColor = System.Drawing.Color.SteelBlue;
            this.label3.Name = "label3";
            // 
            // btnParcourirWebRunner
            // 
            resources.ApplyResources(this.btnParcourirWebRunner, "btnParcourirWebRunner");
            this.btnParcourirWebRunner.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnParcourirWebRunner.FlatAppearance.BorderColor = System.Drawing.Color.Gainsboro;
            this.btnParcourirWebRunner.FlatAppearance.BorderSize = 2;
            this.btnParcourirWebRunner.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightGray;
            this.btnParcourirWebRunner.Name = "btnParcourirWebRunner";
            this.btnParcourirWebRunner.UseVisualStyleBackColor = false;
            this.btnParcourirWebRunner.Click += new System.EventHandler(this.btnParcourirWebRunner_Click);
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.WhiteSmoke;
            resources.ApplyResources(this.label2, "label2");
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Name = "label2";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.ForeColor = System.Drawing.Color.SteelBlue;
            this.label4.Name = "label4";
            // 
            // txtTestAssembly
            // 
            resources.ApplyResources(this.txtTestAssembly, "txtTestAssembly");
            this.txtTestAssembly.BackColor = System.Drawing.Color.WhiteSmoke;
            this.txtTestAssembly.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTestAssembly.Name = "txtTestAssembly";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Name = "label5";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.ForeColor = System.Drawing.Color.Black;
            this.label6.Name = "label6";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.ForeColor = System.Drawing.Color.Black;
            this.label7.Name = "label7";
            // 
            // FrmSettings
            // 
            this.AcceptButton = this.btnSave;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.White;
            this.CancelButton = this.btnAnnuler;
            this.ControlBox = false;
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtTestAssembly);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnParcourirWebRunner);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtWebRunnerFolder);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.txtExportFolder);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnAnnuler);
            this.Controls.Add(this.btnSave);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FrmSettings";
            this.Load += new System.EventHandler(this.FrmSettings_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnAnnuler;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtExportFolder;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.FolderBrowserDialog folderDialog;
        private System.Windows.Forms.TextBox txtWebRunnerFolder;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnParcourirWebRunner;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtTestAssembly;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
    }
}