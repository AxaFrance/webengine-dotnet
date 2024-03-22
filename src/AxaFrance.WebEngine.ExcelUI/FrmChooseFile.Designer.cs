namespace AxaFrance.WebEngine.ExcelUI
{
    partial class FrmUploadFile
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
            this.checkedListBox = new System.Windows.Forms.CheckedListBox();
            this.lb = new System.Windows.Forms.Label();
            this.tb = new System.Windows.Forms.TextBox();
            this.openfileButton = new System.Windows.Forms.Button();
            this.closeBtn = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // checkedListBox
            // 
            this.checkedListBox.FormattingEnabled = true;
            this.checkedListBox.Location = new System.Drawing.Point(12, 38);
            this.checkedListBox.Name = "checkedListBox";
            this.checkedListBox.Size = new System.Drawing.Size(495, 72);
            this.checkedListBox.TabIndex = 0;
            // 
            // lb
            // 
            this.lb.AutoSize = true;
            this.lb.Location = new System.Drawing.Point(13, 2);
            this.lb.Name = "lb";
            this.lb.Size = new System.Drawing.Size(0, 16);
            this.lb.TabIndex = 1;
            // 
            // tb
            // 
            this.tb.Location = new System.Drawing.Point(12, 117);
            this.tb.Name = "tb";
            this.tb.Size = new System.Drawing.Size(398, 22);
            this.tb.TabIndex = 2;
            // 
            // openfileButton
            // 
            this.openfileButton.Location = new System.Drawing.Point(416, 115);
            this.openfileButton.Name = "openfileButton";
            this.openfileButton.Size = new System.Drawing.Size(91, 23);
            this.openfileButton.TabIndex = 3;
            this.openfileButton.Text = "Choisir";
            this.openfileButton.UseVisualStyleBackColor = true;
            this.openfileButton.Click += new System.EventHandler(this.openfileButton_Click);
            // 
            // closeBtn
            // 
            this.closeBtn.Location = new System.Drawing.Point(416, 144);
            this.closeBtn.Name = "closeBtn";
            this.closeBtn.Size = new System.Drawing.Size(91, 23);
            this.closeBtn.TabIndex = 4;
            this.closeBtn.Text = "Done";
            this.closeBtn.UseVisualStyleBackColor = true;
            this.closeBtn.Click += new System.EventHandler(this.closeBtn_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog";
            // 
            // FrmUploadFile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(519, 193);
            this.Controls.Add(this.closeBtn);
            this.Controls.Add(this.openfileButton);
            this.Controls.Add(this.tb);
            this.Controls.Add(this.lb);
            this.Controls.Add(this.checkedListBox);
            this.Name = "FrmUploadFile";
            this.Text = "Choisir un fichier";
            this.Load += new System.EventHandler(this.FrmUploadFile_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox checkedListBox;
        private System.Windows.Forms.Label lb;
        private System.Windows.Forms.TextBox tb;
        private System.Windows.Forms.Button openfileButton;
        private System.Windows.Forms.Button closeBtn;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
    }
}