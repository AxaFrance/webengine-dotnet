using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AxaFrance.WebEngine.ExcelUI;
using OpenQA.Selenium.Appium;
using System.Windows.Forms;
using System.Text.Json.Nodes;

namespace WebEngine.Test.UnitTests
{
    [TestClass]
    [TestCategory("NoCode")]
    class NoCodeAddinTest
    {
        [TestMethod]
        public void FillTheFieldsTest()
        {
            /**
            //generate unit test for FillTheFields
            //Arrange
            FrmTargetEdit frmTargetEdit = new FrmTargetEdit();
            //Act
            String target = "{\"id\":\"ctl00_cphMain_pickerIE_pickerDateDebutMission_ceCalendar_day_2_4\"," +
                "\"name\":\"rr\",\"innerText\":\"er\",\"xpath\":\"//3\",\"tagName\":\"div\"," +
                "\"attributeList\":{\"aria-role\":\"a\",\"data-testid\":\"z\",\"y\":\"o\"," +
                "\"title\":\"Thursday, February 15, 2024\",\"e\":\"r\"},\"shadowDom\":\"true\"}\r\n";
            //frmTargetEdit.FillTheFields(ref target);
            JsonNode targetObject = JsonNode.Parse(target);
            //FillTheFields only accept one parameter
            //Assert
            Assert.AreEqual(targetObject["id"], frmTargetEdit.Controls["TbId"].Text);
            Assert.AreEqual(targetObject["name"], frmTargetEdit.Controls["TbName"].Text);
            Assert.AreEqual(targetObject["className"], frmTargetEdit.Controls["TbClassName"].Text);
            Assert.AreEqual(targetObject["cssSelector"], frmTargetEdit.Controls["TbCssSelector"].Text);
            Assert.AreEqual(targetObject["innerText"], frmTargetEdit.Controls["TbInnerText"].Text);
            Assert.AreEqual(targetObject["xpath"], frmTargetEdit.Controls["TbXpath"].Text);
            Assert.AreEqual(targetObject["tagName"], frmTargetEdit.Controls["TbTagName"].Text);
            Assert.AreEqual(targetObject["aria-label"], frmTargetEdit.Controls["TbAriaLabel"].Text);
            Assert.AreEqual(targetObject["aria-role"], frmTargetEdit.Controls["TbAriaRole"].Text);
            Assert.AreEqual(targetObject["data-testid"], frmTargetEdit.Controls["TbDataID"].Text);
            Assert.AreEqual(targetObject["data-qsi"], frmTargetEdit.Controls["TbDataqsi"].Text);
            Assert.AreEqual(targetObject["attributeList"], frmTargetEdit.Controls["TbAtrributs"].Text);
            Assert.AreEqual(targetObject["shadowDom"], ((CheckBox)frmTargetEdit.Controls["CbIsShadow"]).Checked.ToString());
        **/
            }
    }
    //generate test method for the method above
    /* [TestMethod]
     public void button1_ClickTest()
     {
         //Arrange
         FrmTargetEdit frmTargetEdit = new FrmTargetEdit();
         //get frmTargetEdit control TbId in controls list


         frmTargetEdit.TbName.Text = "name";
         frmTargetEdit.TbClassName.Text = "className";
         frmTargetEdit.TbCssSelector.Text = "cssSelector";
         frmTargetEdit.TbInnerText.Text = "innerText";
         frmTargetEdit.TbXpath.Text = "xpath";
         frmTargetEdit.TbTagName.Text = "tagName";
         frmTargetEdit.TbAriaLabel.Text = "aria-label";
         frmTargetEdit.TbAriaRole.Text = "aria-role";
         frmTargetEdit.TbDataID.Text = "data-testid";
         frmTargetEdit.TbDataqsi.Text = "data-qsi";
         frmTargetEdit.TbAtrributs.Text = "attributeList";
         frmTargetEdit.CbIsShadow.Checked = true;

         //Act
         frmTargetEdit.button1_Click(new object(), new EventArgs());

         //Assert
         Assert.AreEqual(targetObject["{" + "\"id\":\"id\",\"name\":\"name\",\"className\":\"className\",\"cssSelector\":\"cssSelector\",\"innerText\":\"innerText\",\"xpath\":\"xpath\",\"tagName\":\"tagName\",\"attributeList\":{\"aria-label\":\"aria-label\",\"aria-role\":\"aria-role\",\"data-testid\":\"data-testid\",\"data-qsi\":\"data-qsi\",\"attributeList\":\"attributeList\"},\"shadowDom\":\"true\"}", Globals.ThisAddIn.Application.Cells[1, 3].FormulaLocal);
     }*/
}

