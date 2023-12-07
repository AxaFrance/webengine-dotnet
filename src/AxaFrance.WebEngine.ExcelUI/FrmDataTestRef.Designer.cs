namespace AxaFrance.WebEngine.ExcelUI
{
    partial class FrmDataTestRef
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
            this.label1 = new System.Windows.Forms.Label();
            this.btOK = new System.Windows.Forms.Button();
            this.btCancel = new System.Windows.Forms.Button();
            this.cbListAllDataSet = new System.Windows.Forms.CheckedListBox();
            this.cbSelectAll = new System.Windows.Forms.CheckBox();
            this.lbWarning = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(355, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Selectionner les colonnes à inclure/exclure pour cette ligne";
            // 
            // btOK
            // 
            this.btOK.Location = new System.Drawing.Point(212, 220);
            this.btOK.Name = "btOK";
            this.btOK.Size = new System.Drawing.Size(75, 23);
            this.btOK.TabIndex = 2;
            this.btOK.Text = "OK";
            this.btOK.UseVisualStyleBackColor = true;
            this.btOK.Click += new System.EventHandler(this.button1_Click);
            // 
            // btCancel
            // 
            this.btCancel.Location = new System.Drawing.Point(293, 220);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(75, 23);
            this.btCancel.TabIndex = 3;
            this.btCancel.Text = "Cancel";
            this.btCancel.UseVisualStyleBackColor = true;
            this.btCancel.Click += new System.EventHandler(this.button2_Click);
            // 
            // cbListAllDataSet
            // 
            this.cbListAllDataSet.FormattingEnabled = true;
            this.cbListAllDataSet.Location = new System.Drawing.Point(16, 108);
            this.cbListAllDataSet.MultiColumn = true;
            this.cbListAllDataSet.Name = "cbListAllDataSet";
            this.cbListAllDataSet.Size = new System.Drawing.Size(352, 106);
            this.cbListAllDataSet.TabIndex = 4;
            // 
            // cbSelectAll
            // 
            this.cbSelectAll.AutoSize = true;
            this.cbSelectAll.Location = new System.Drawing.Point(16, 73);
            this.cbSelectAll.Name = "cbSelectAll";
            this.cbSelectAll.Size = new System.Drawing.Size(116, 20);
            this.cbSelectAll.TabIndex = 5;
            this.cbSelectAll.Text = "Inclure/Exclure";
            this.cbSelectAll.UseVisualStyleBackColor = true;
            this.cbSelectAll.CheckedChanged += new System.EventHandler(this.cbSelectAll_CheckedChanged);
            // 
            // lbWarning
            // 
            this.lbWarning.Location = new System.Drawing.Point(16, 33);
            this.lbWarning.Name = "lbWarning";
            this.lbWarning.Size = new System.Drawing.Size(352, 37);
            this.lbWarning.TabIndex = 7;
            this.lbWarning.Text = "*";
            // 
            // FrmDataTestRef
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(378, 266);
            this.Controls.Add(this.lbWarning);
            this.Controls.Add(this.cbSelectAll);
            this.Controls.Add(this.cbListAllDataSet);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btOK);
            this.Controls.Add(this.label1);
            this.Name = "FrmDataTestRef";
            this.Text = "Inclure/Exclure";
            this.Load += new System.EventHandler(this.FrmDataTestRef_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btOK;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.CheckedListBox cbListAllDataSet;
        private System.Windows.Forms.CheckBox cbSelectAll;
        private System.Windows.Forms.Label lbWarning;
    }
}