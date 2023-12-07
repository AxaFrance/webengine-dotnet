using Microsoft.Office.Interop.Excel;
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
    public partial class FrmTargetEdit : Form
    {

        public FrmTargetEdit()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            String target = "";
            List<string> targetsList = new List<string>();
            if (!String.IsNullOrEmpty(TbId.Text))
            {
                targetsList.Add("\"id\":\"" + TbId.Text + "\"");
            }
            if (!String.IsNullOrEmpty(TbName.Text))
            {
                targetsList.Add("\"name\":\"" + TbName.Text + "\"");
            }
            if (!String.IsNullOrEmpty(TbClassName.Text))
            {
                targetsList.Add("\"className\":\"" + TbClassName.Text + "\"");
            }
            if (!String.IsNullOrEmpty(TbCssSelector.Text))
            {
                targetsList.Add("\"cssSelector\":\"" + TbCssSelector.Text + "\"");
            }
            if (!String.IsNullOrEmpty(TbInnerText.Text))
            {
                targetsList.Add("\"innerText\":\"" + TbInnerText.Text + "\"");
            }
            if (!String.IsNullOrEmpty(TbXpath.Text))
            {
                targetsList.Add("\"xpath\":\"" + TbXpath.Text + "\"");
            }
            if (!String.IsNullOrEmpty(TbTagName.Text))
            {
                targetsList.Add("\"tagName\":\"" + TbTagName.Text + "\"");
            }

            List<string> attributesList = new List<string>();

            if (!String.IsNullOrEmpty(TbAriaLabel.Text))
            {
                attributesList.Add("\"aria-label\":\"" + TbAriaLabel.Text + "\"");
            }
            if (!String.IsNullOrEmpty(TbAriaRole.Text))
            {
                attributesList.Add("\"aria-role\":\"" + TbAriaRole.Text + "\"");
            }
            if (!String.IsNullOrEmpty(TbDataID.Text))
            {
                attributesList.Add("\"data-testid\":\"" + TbDataID.Text + "\"");
            }
            if (!String.IsNullOrEmpty(TbDataqsi.Text))
            {
                attributesList.Add("\"data-qsi\":\"" + TbDataqsi.Text + "\"");
            }
            if (!String.IsNullOrEmpty(TbAtrributs.Text))
            {
                attributesList.Add(TbAtrributs.Text);
            }

            if (targetsList.Count == 1 && attributesList.Count == 0 && !CbIsShadow.Checked)
            {
                target = targetsList[0];
                if (target.StartsWith("\"xpath\":\""))
                {
                    target = TbXpath.Text;
                }
                else if (target.StartsWith("\"id\":\""))
                {
                    target = TbId.Text;
                }
            }
            else
            {
                target = String.Join(",", targetsList);

                if (attributesList.Count > 0)
                {
                    String attributes = "\"attributeList\":{" + String.Join(",", attributesList) + "}";
                    if (!String.IsNullOrEmpty(target))
                    {
                        target = target + "," + attributes;
                    }
                    else
                    {
                        target = attributes;
                    }
                }

                if (CbIsShadow.Checked && !String.IsNullOrEmpty(target))
                {
                    target = target + ",\"shadowDom\":\"true\"";
                }

                if (!String.IsNullOrEmpty(target))
                {
                    target = "{" + target + "}";
                }
            }

            if (String.IsNullOrEmpty(target))
            {
                //warn user
                MessageBox.Show("Veuillez renseigner un attribut");
                return;
            }

            int currentRow = Globals.ThisAddIn.Application.ActiveCell.Row;
            Globals.ThisAddIn.Application.Cells[currentRow,3] = target;
            this.Close();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmTargetEdit_Load(object sender, EventArgs e)
        {
            this.KeyPress += new KeyPressEventHandler(FrmTargetEdit_KeyPress);

            //set all textboxes with the current cell value
            String target = Globals.ThisAddIn.Application.ActiveCell.FormulaLocal;
            if (!String.IsNullOrEmpty(target))
            {
                if (!target.StartsWith("{") && !target.Contains("//"))
                {
                    TbId.Text = target;
                    return;
                }

                if (target.Contains("//") || (target.Contains("(") && target.Contains(")")) || (target.Contains("[") && target.Contains("]")))
                {
                    TbXpath .Text = target;
                    return;
                }

                if (target.StartsWith("{") && target.EndsWith("}"))
                {
                    target = target.Substring(1, target.Length - 2);

                    string attlistlabel = "\"attributeList\":";

                    if (target.Contains(attlistlabel))
                    {

                        String attributes = target.Substring(target.IndexOf(attlistlabel) + attlistlabel.Length);

                        attributes = attributes.Substring(1, attributes.IndexOf("}") - 1);

                        String[] attributesParts = attributes.Split(',');
                        foreach (String att in attributesParts)
                        {
                            String[] keyval = att.Split(':');

                            String key = keyval[0].Replace("\"", "");
                            String value = keyval[1].Replace("\"", "");
                            switch (key)
                            {
                                case "aria-label":
                                    TbAriaLabel.Text = value;
                                    break;
                                case "aria-role":
                                    TbAriaRole.Text = value;
                                    break;
                                case "data-testid":
                                    TbDataID.Text = value;
                                    break;
                                case "data-qsi":
                                    TbDataqsi.Text = value;
                                    break;
                                default:
                                    TbAtrributs.Text = TbAtrributs.Text + "," + att;
                                    if (TbAtrributs.Text.StartsWith(","))
                                    {
                                        TbAtrributs.Text = TbAtrributs.Text.Substring(1);
                                    }
                                    if (TbAtrributs.Text.EndsWith(","))
                                    {
                                        TbAtrributs.Text = TbAtrributs.Text.Substring(0, TbAtrributs.Text.Length - 1);
                                    }
                                    break;
                            }
                        }
                        target = target.Replace(attributes, "");
                        target = target.Replace(attlistlabel, "");
                        target = target.Replace("{}", "");
                    }

                    String[] targets = target.Split(',');
                    foreach (String t in targets)
                    {
                        if (t.Contains("\"shadowDom\":\"true\""))
                        {
                            CbIsShadow.Checked = true;
                            target = target.Replace("\"shadowDom\":\"true\"", "");
                            continue;
                        }

                        else {
                            target = target.Replace("{", "").Replace("}", "");
                            String[] targetParts = t.Split(':');
                            if (targetParts.Length == 2)
                            {
                                String key = targetParts[0].Replace("\"", "");
                                String value = targetParts[1].Replace("\"", "");
                                switch (key)
                                {
                                    case "id":
                                        TbId.Text = value;
                                        break;
                                    case "name":
                                        TbName.Text = value;
                                        break;
                                    case "className":
                                        TbClassName.Text = value;
                                        break;
                                    case "cssSelector":
                                        TbCssSelector.Text = value;
                                        break;
                                    case "innerText":
                                        TbInnerText.Text = value;
                                        break;
                                    case "xpath":
                                        TbXpath.Text = value;
                                        break;
                                    case "aria-label":
                                        TbAriaLabel.Text = value;
                                        break;
                                    case "aria-role":
                                        TbAriaRole.Text = value;
                                        break;
                                    case "data-testid":
                                        TbDataID.Text = value;
                                        break;
                                    case "data-qsi":
                                        TbDataqsi.Text = value;
                                        break;
                                    case "tagName":
                                        TbTagName.Text = value;
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void FrmTargetEdit_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Escape)
            {
                this.Close();
            }
        }
    }
}
