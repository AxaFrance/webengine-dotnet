using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YamlDotNet.Core.Tokens;

namespace AxaFrance.WebEngine.ExcelUI
{
    public partial class FrmDataTestRef : Form
    {
        public static int ref_Col = 5;
        private const string DATA_NOT_EXIST = "Vous avez renseignez un nom de colonne qui n'existe pas!";

        public FrmDataTestRef()
        {
            InitializeComponent();
        }

        private void FrmDataTestRef_Load(object sender, EventArgs e)
        {
            FrmRun.InitializeTestCasesList(cbListAllDataSet, lbWarning, null);
            int currentRow = Globals.ThisAddIn.Application.ActiveCell.Row;
            String currenttarget = Globals.ThisAddIn.Application.Cells[currentRow, ref_Col].FormulaLocal;
            if (String.IsNullOrEmpty(currenttarget))
            {
                selectAll();
                cbSelectAll.Checked = true;
            }
            else if (currenttarget.Trim().Equals("!"))
            {
                deSelectAll();
                cbSelectAll.Checked = false;
            }
            else
            {
                deSelectAll();
                cbSelectAll.Checked = false;
                List<string> allItems = new List<string>();
                for (int i=0; i < cbListAllDataSet.Items.Count; i++)
                {
                    allItems.Add(cbListAllDataSet.Items[i].ToString());
                }
                List<String> result = mergeLists(currenttarget.Split(',').ToList(),allItems);
                foreach (string item in result)
                {
                    int index = cbListAllDataSet.Items.IndexOf(item);
                    if (index < 0)
                    {
                        continue;
                    }
                    cbListAllDataSet.SetItemChecked(index, true);
                }
            }
        }

        private void cbSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSelectAll.Checked)
            {
                selectAll();
            }
            else
            {
                deSelectAll();
            }
        }

        private void deSelectAll()
        {
            for (int i = 0; i < cbListAllDataSet.Items.Count; i++)
            {
                cbListAllDataSet.SetItemChecked(i, false);
            }
            cbSelectAll.Text = "Inclure Tout";
        }

        private void selectAll()
        {
            for (int i = 0; i < cbListAllDataSet.Items.Count; i++)
            {
                cbListAllDataSet.SetItemChecked(i, true);
            }
            cbSelectAll.Text = "Exclure Tout";
        }

        public static List<String> mergeLists(List<String> refList, List<String> allItems)
        {
            List<String> result = new List<String>();

            if (refList ==null || refList.Count==0)
            {
                result.AddRange(allItems);
            }
            else
            {
                bool containsNegation = refList.Any(value => value.Contains("!"));
                if (!containsNegation)
                {
                    result.AddRange(refList);
                }
                else
                {
                    if (refList.Count == 1)
                    {
                        foreach (String item in allItems)
                        {
                            if (!refList.Contains("!" + item))
                            {
                                result.Add(item);
                            }
                        }
                    }
                    else
                    {
                        result.AddRange((IEnumerable<string>)refList.Find(value => !value.Contains("!")).ToList());
                    }
                }
            }
            return result;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int currentRow = Globals.ThisAddIn.Application.ActiveCell.Row;

            int CheckItemsCount = cbListAllDataSet.CheckedItems.Count;
            if (CheckItemsCount == 1)
            {
                Globals.ThisAddIn.Application.Cells[currentRow, ref_Col] = cbListAllDataSet.CheckedItems[0];
            }
            else if (CheckItemsCount == 0)
            {
                Globals.ThisAddIn.Application.Cells[currentRow, ref_Col] = "!";
            }
            else if (CheckItemsCount == cbListAllDataSet.Items.Count - 1)
            {
                List<string> list = cbListAllDataSet.Items.Cast<string>().ToList();
                List<string> slist = cbListAllDataSet.CheckedItems.Cast<string>().ToList();
                list.RemoveAll(s => slist.Contains(s));

                Globals.ThisAddIn.Application.Cells[currentRow, ref_Col] = "!"+list[0];
            }
            else if(CheckItemsCount == cbListAllDataSet.Items.Count)
            {
                Globals.ThisAddIn.Application.Cells[currentRow, ref_Col] = "";
            }
            else {
                String selectedCol = "";
                foreach (String v in cbListAllDataSet.CheckedItems) 
                {
                    selectedCol = selectedCol + ";" + v;
                }
                selectedCol=selectedCol.Substring(1);
                Globals.ThisAddIn.Application.Cells[currentRow, ref_Col] = selectedCol;
            }
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
