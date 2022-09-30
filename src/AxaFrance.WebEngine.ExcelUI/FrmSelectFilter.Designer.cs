// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
namespace AxaFrance.WebEngine.ExcelUI
{
    partial class FrmSelectFilter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmSelectFilter));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtFilters = new System.Windows.Forms.TextBox();
            this.rbExportMatches = new System.Windows.Forms.RadioButton();
            this.rbExportNotMatches = new System.Windows.Forms.RadioButton();
            this.btnStart = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // txtFilters
            // 
            resources.ApplyResources(this.txtFilters, "txtFilters");
            this.txtFilters.Name = "txtFilters";
            // 
            // rbExportMatches
            // 
            resources.ApplyResources(this.rbExportMatches, "rbExportMatches");
            this.rbExportMatches.Name = "rbExportMatches";
            this.rbExportMatches.TabStop = true;
            this.rbExportMatches.UseVisualStyleBackColor = true;
            // 
            // rbExportNotMatches
            // 
            resources.ApplyResources(this.rbExportNotMatches, "rbExportNotMatches");
            this.rbExportNotMatches.Checked = true;
            this.rbExportNotMatches.Name = "rbExportNotMatches";
            this.rbExportNotMatches.TabStop = true;
            this.rbExportNotMatches.UseVisualStyleBackColor = true;
            // 
            // btnStart
            // 
            resources.ApplyResources(this.btnStart, "btnStart");
            this.btnStart.BackColor = System.Drawing.Color.WhiteSmoke;
            this.btnStart.FlatAppearance.BorderColor = System.Drawing.Color.Gainsboro;
            this.btnStart.FlatAppearance.BorderSize = 2;
            this.btnStart.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gainsboro;
            this.btnStart.FlatAppearance.MouseOverBackColor = System.Drawing.Color.LightSkyBlue;
            this.btnStart.ForeColor = System.Drawing.Color.Black;
            this.btnStart.Name = "btnStart";
            this.btnStart.UseVisualStyleBackColor = false;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // FrmSelectFilter
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.rbExportNotMatches);
            this.Controls.Add(this.rbExportMatches);
            this.Controls.Add(this.txtFilters);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FrmSelectFilter";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtFilters;
        private System.Windows.Forms.RadioButton rbExportMatches;
        private System.Windows.Forms.RadioButton rbExportNotMatches;
        private System.Windows.Forms.Button btnStart;
    }
}