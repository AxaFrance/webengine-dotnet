using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AxaFrance.WebEngine.ExcelUI
{
    public partial class FrmUploadFile : Form
    {
        int currentRow = 0;
        Dictionary<String, int> colNameIndex = new Dictionary<string, int>();

        public FrmUploadFile()
        {
            InitializeComponent();
        }

        private void openfileButton_Click(object sender, EventArgs e)
        {
            var result = openFileDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                tb.Text = openFileDialog.FileName;
            }
        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            currentRow = Globals.ThisAddIn.Application.ActiveCell.Row;

            foreach (string colname in checkedListBox.CheckedItems)
            {
                Globals.ThisAddIn.Application.ActiveSheet.Cells[currentRow,
                    colNameIndex[colname]] = tb.Text;
            }
            this.Close();
        }

        private void FrmUploadFile_Load(object sender, EventArgs e)
        {
            FrmRun.InitializeTestCasesList(checkedListBox, lb, colNameIndex);
        }
    }
}
