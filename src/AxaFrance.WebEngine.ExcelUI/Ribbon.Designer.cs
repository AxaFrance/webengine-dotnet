// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using Microsoft.Office.Tools.Ribbon;
using System;

namespace AxaFrance.WebEngine.ExcelUI
{
    partial class Ribbon : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public Ribbon()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Ribbon));
            this.tabWebEngine = this.Factory.CreateRibbonTab();
            this.grpNoCode = this.Factory.CreateRibbonGroup();
            this.btnStartDriveByExcelNow = this.Factory.CreateRibbonButton();
            this.driveSettings = this.Factory.CreateRibbonButton();
            this.BtGotoDriveHelp = this.Factory.CreateRibbonButton();
            this.KeepassHelp = this.Factory.CreateRibbonButton();
            this.grpKeyWord = this.Factory.CreateRibbonGroup();
            this.gryExport = this.Factory.CreateRibbonGallery();
            this.btnExportSelection = this.Factory.CreateRibbonButton();
            this.btnExportAll = this.Factory.CreateRibbonButton();
            this.btnExportAllTabs = this.Factory.CreateRibbonButton();
            this.BtnExportFilter = this.Factory.CreateRibbonButton();
            this.btnExportEnvironmentVariable = this.Factory.CreateRibbonButton();
            this.gryTools = this.Factory.CreateRibbonGallery();
            this.btnCodeGeneration = this.Factory.CreateRibbonButton();
            this.btnCodeGenerationJava = this.Factory.CreateRibbonButton();
            this.btnDataCheck = this.Factory.CreateRibbonButton();
            this.btnDataSync = this.Factory.CreateRibbonButton();
            this.separator2 = this.Factory.CreateRibbonSeparator();
            this.btnStartNow = this.Factory.CreateRibbonButton();
            this.separator1 = this.Factory.CreateRibbonSeparator();
            this.btnSettings = this.Factory.CreateRibbonButton();
            this.gryHelp = this.Factory.CreateRibbonGallery();
            this.btnAbout = this.Factory.CreateRibbonButton();
            this.btnHelp = this.Factory.CreateRibbonButton();
            this.btnFeedback = this.Factory.CreateRibbonButton();
            this.generateGherkin = this.Factory.CreateRibbonButton();
            this.tabKeyWord = this.Factory.CreateRibbonTab();
            this.grpTabChoiceKeyWord = this.Factory.CreateRibbonGroup();
            this.cbShowKeyword2 = this.Factory.CreateRibbonCheckBox();
            this.cbShowNoCode2 = this.Factory.CreateRibbonCheckBox();
            this.cbShowInov2 = this.Factory.CreateRibbonCheckBox();
            this.grpTabChoice = this.Factory.CreateRibbonGroup();
            this.cbShowKeyword = this.Factory.CreateRibbonCheckBox();
            this.cbShowNoCode = this.Factory.CreateRibbonCheckBox();
            this.cbShowInov = this.Factory.CreateRibbonCheckBox();
            this.tabNoCode = this.Factory.CreateRibbonTab();
            this.grpTools = this.Factory.CreateRibbonGroup();
            this.BtChooseFile = this.Factory.CreateRibbonButton();
            this.btExclude = this.Factory.CreateRibbonButton();
            this.BtTargetHelp = this.Factory.CreateRibbonButton();
            this.grpContacts = this.Factory.CreateRibbonGroup();
            this.BtSendMail = this.Factory.CreateRibbonButton();
            this.group2 = this.Factory.CreateRibbonGroup();
            this.lbVersion = this.Factory.CreateRibbonLabel();
            this.tabGherkinSample = this.Factory.CreateRibbonTab();
            this.group1 = this.Factory.CreateRibbonGroup();
            this.btGenerateGherkin = this.Factory.CreateRibbonButton();
            this.tabWebEngine.SuspendLayout();
            this.grpNoCode.SuspendLayout();
            this.grpKeyWord.SuspendLayout();
            this.tabKeyWord.SuspendLayout();
            this.grpTabChoiceKeyWord.SuspendLayout();
            this.grpTabChoice.SuspendLayout();
            this.tabNoCode.SuspendLayout();
            this.grpTools.SuspendLayout();
            this.grpContacts.SuspendLayout();
            this.group2.SuspendLayout();
            this.tabGherkinSample.SuspendLayout();
            this.group1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabWebEngine
            // 
            this.tabWebEngine.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            resources.ApplyResources(this.tabWebEngine, "tabWebEngine");
            this.tabWebEngine.Name = "tabWebEngine";
            // 
            // grpNoCode
            // 
            this.grpNoCode.Items.Add(this.btnStartDriveByExcelNow);
            this.grpNoCode.Items.Add(this.driveSettings);
            this.grpNoCode.Items.Add(this.BtGotoDriveHelp);
            this.grpNoCode.Items.Add(this.KeepassHelp);
            resources.ApplyResources(this.grpNoCode, "grpNoCode");
            this.grpNoCode.Name = "grpNoCode";
            // 
            // btnStartDriveByExcelNow
            // 
            this.btnStartDriveByExcelNow.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnStartDriveByExcelNow.Image = global::AxaFrance.WebEngine.ExcelUI.Properties.Resources.no_code_light;
            resources.ApplyResources(this.btnStartDriveByExcelNow, "btnStartDriveByExcelNow");
            this.btnStartDriveByExcelNow.Name = "btnStartDriveByExcelNow";
            this.btnStartDriveByExcelNow.ShowImage = true;
            this.btnStartDriveByExcelNow.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnStartDriveByExcelNow_Click_1);
            // 
            // driveSettings
            // 
            this.driveSettings.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.driveSettings.Image = global::AxaFrance.WebEngine.ExcelUI.Properties.Resources.drive_by_settings;
            resources.ApplyResources(this.driveSettings, "driveSettings");
            this.driveSettings.Name = "driveSettings";
            this.driveSettings.OfficeImageId = "AddInCommandsMenu";
            this.driveSettings.ShowImage = true;
            this.driveSettings.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.driveSettings_Click);
            // 
            // BtGotoDriveHelp
            // 
            this.BtGotoDriveHelp.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            resources.ApplyResources(this.BtGotoDriveHelp, "BtGotoDriveHelp");
            this.BtGotoDriveHelp.Name = "BtGotoDriveHelp";
            this.BtGotoDriveHelp.OfficeImageId = "Help";
            this.BtGotoDriveHelp.ShowImage = true;
            this.BtGotoDriveHelp.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.BtGotoDriveHelp_Click);
            // 
            // KeepassHelp
            // 
            this.KeepassHelp.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            resources.ApplyResources(this.KeepassHelp, "KeepassHelp");
            this.KeepassHelp.Name = "KeepassHelp";
            this.KeepassHelp.OfficeImageId = "Help";
            this.KeepassHelp.ShowImage = true;
            this.KeepassHelp.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.KeepassHelp_Click);
            // 
            // grpKeyWord
            // 
            this.grpKeyWord.Items.Add(this.gryExport);
            this.grpKeyWord.Items.Add(this.gryTools);
            this.grpKeyWord.Items.Add(this.separator2);
            this.grpKeyWord.Items.Add(this.btnStartNow);
            this.grpKeyWord.Items.Add(this.separator1);
            this.grpKeyWord.Items.Add(this.btnSettings);
            this.grpKeyWord.Items.Add(this.gryHelp);
            this.grpKeyWord.Items.Add(this.generateGherkin);
            resources.ApplyResources(this.grpKeyWord, "grpKeyWord");
            this.grpKeyWord.Name = "grpKeyWord";
            // 
            // gryExport
            // 
            this.gryExport.Buttons.Add(this.btnExportSelection);
            this.gryExport.Buttons.Add(this.btnExportAll);
            this.gryExport.Buttons.Add(this.btnExportAllTabs);
            this.gryExport.Buttons.Add(this.BtnExportFilter);
            this.gryExport.Buttons.Add(this.btnExportEnvironmentVariable);
            this.gryExport.ColumnCount = 1;
            this.gryExport.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            resources.ApplyResources(this.gryExport, "gryExport");
            this.gryExport.Name = "gryExport";
            this.gryExport.OfficeImageId = "ExportXmlFile";
            this.gryExport.ShowImage = true;
            // 
            // btnExportSelection
            // 
            resources.ApplyResources(this.btnExportSelection, "btnExportSelection");
            this.btnExportSelection.Name = "btnExportSelection";
            this.btnExportSelection.OfficeImageId = "ExportXmlFile";
            this.btnExportSelection.ShowImage = true;
            this.btnExportSelection.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnExportSelection_Click);
            // 
            // btnExportAll
            // 
            resources.ApplyResources(this.btnExportAll, "btnExportAll");
            this.btnExportAll.Name = "btnExportAll";
            this.btnExportAll.OfficeImageId = "ExportSavedExports";
            this.btnExportAll.ShowImage = true;
            this.btnExportAll.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnExportAll_Click);
            // 
            // btnExportAllTabs
            // 
            resources.ApplyResources(this.btnExportAllTabs, "btnExportAllTabs");
            this.btnExportAllTabs.Name = "btnExportAllTabs";
            this.btnExportAllTabs.OfficeImageId = "ExportSavedExports";
            this.btnExportAllTabs.ShowImage = true;
            // 
            // BtnExportFilter
            // 
            resources.ApplyResources(this.BtnExportFilter, "BtnExportFilter");
            this.BtnExportFilter.Name = "BtnExportFilter";
            this.BtnExportFilter.OfficeImageId = "ExportXmlFile";
            this.BtnExportFilter.ShowImage = true;
            this.BtnExportFilter.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.BtnExportFilter_Click);
            // 
            // btnExportEnvironmentVariable
            // 
            resources.ApplyResources(this.btnExportEnvironmentVariable, "btnExportEnvironmentVariable");
            this.btnExportEnvironmentVariable.Name = "btnExportEnvironmentVariable";
            this.btnExportEnvironmentVariable.OfficeImageId = "ListPublish";
            this.btnExportEnvironmentVariable.ShowImage = true;
            this.btnExportEnvironmentVariable.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnExportEnvironmentVariable_Click);
            // 
            // gryTools
            // 
            this.gryTools.Buttons.Add(this.btnCodeGeneration);
            this.gryTools.Buttons.Add(this.btnCodeGenerationJava);
            this.gryTools.Buttons.Add(this.btnDataCheck);
            this.gryTools.Buttons.Add(this.btnDataSync);
            this.gryTools.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            resources.ApplyResources(this.gryTools, "gryTools");
            this.gryTools.Name = "gryTools";
            this.gryTools.OfficeImageId = "ToolboxGallery";
            this.gryTools.ShowImage = true;
            // 
            // btnCodeGeneration
            // 
            resources.ApplyResources(this.btnCodeGeneration, "btnCodeGeneration");
            this.btnCodeGeneration.Name = "btnCodeGeneration";
            this.btnCodeGeneration.OfficeImageId = "AfterInsert";
            this.btnCodeGeneration.ShowImage = true;
            this.btnCodeGeneration.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnCodeGenerationCSharp_Click);
            // 
            // btnCodeGenerationJava
            // 
            resources.ApplyResources(this.btnCodeGenerationJava, "btnCodeGenerationJava");
            this.btnCodeGenerationJava.Name = "btnCodeGenerationJava";
            this.btnCodeGenerationJava.OfficeImageId = "AfterInsert";
            this.btnCodeGenerationJava.ShowImage = true;
            this.btnCodeGenerationJava.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnCodeGenerationJava_Click);
            // 
            // btnDataCheck
            // 
            resources.ApplyResources(this.btnDataCheck, "btnDataCheck");
            this.btnDataCheck.Name = "btnDataCheck";
            this.btnDataCheck.OfficeImageId = "CheckWorkflow";
            this.btnDataCheck.ShowImage = true;
            this.btnDataCheck.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnDataCheck_Click);
            // 
            // btnDataSync
            // 
            resources.ApplyResources(this.btnDataSync, "btnDataSync");
            this.btnDataSync.Name = "btnDataSync";
            this.btnDataSync.OfficeImageId = "SynchronizeAll";
            this.btnDataSync.ShowImage = true;
            this.btnDataSync.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnDataMerge_Click);
            // 
            // separator2
            // 
            this.separator2.Name = "separator2";
            // 
            // btnStartNow
            // 
            this.btnStartNow.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            resources.ApplyResources(this.btnStartNow, "btnStartNow");
            this.btnStartNow.Name = "btnStartNow";
            this.btnStartNow.OfficeImageId = "PlayFromPage";
            this.btnStartNow.ShowImage = true;
            this.btnStartNow.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnStartNow_Click);
            // 
            // separator1
            // 
            this.separator1.Name = "separator1";
            // 
            // btnSettings
            // 
            this.btnSettings.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            resources.ApplyResources(this.btnSettings, "btnSettings");
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.OfficeImageId = "AddInCommandsMenu";
            this.btnSettings.ShowImage = true;
            this.btnSettings.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnSettings_Click);
            // 
            // gryHelp
            // 
            this.gryHelp.Buttons.Add(this.btnAbout);
            this.gryHelp.Buttons.Add(this.btnHelp);
            this.gryHelp.Buttons.Add(this.btnFeedback);
            this.gryHelp.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            resources.ApplyResources(this.gryHelp, "gryHelp");
            this.gryHelp.Name = "gryHelp";
            this.gryHelp.OfficeImageId = "Help";
            this.gryHelp.ShowImage = true;
            // 
            // btnAbout
            // 
            resources.ApplyResources(this.btnAbout, "btnAbout");
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.OfficeImageId = "Info";
            this.btnAbout.ShowImage = true;
            this.btnAbout.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnAbout_Click);
            // 
            // btnHelp
            // 
            resources.ApplyResources(this.btnHelp, "btnHelp");
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.OfficeImageId = "Help";
            this.btnHelp.ShowImage = true;
            this.btnHelp.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnHelp_Click);
            // 
            // btnFeedback
            // 
            resources.ApplyResources(this.btnFeedback, "btnFeedback");
            this.btnFeedback.Name = "btnFeedback";
            this.btnFeedback.OfficeImageId = "OmsEmoticonInsertGallery";
            this.btnFeedback.ShowImage = true;
            this.btnFeedback.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnFeedback_Click);
            // 
            // generateGherkin
            // 
            resources.ApplyResources(this.generateGherkin, "generateGherkin");
            this.generateGherkin.Name = "generateGherkin";
            // 
            // tabKeyWord
            // 
            this.tabKeyWord.Groups.Add(this.grpKeyWord);
            this.tabKeyWord.Groups.Add(this.grpTabChoiceKeyWord);
            resources.ApplyResources(this.tabKeyWord, "tabKeyWord");
            this.tabKeyWord.Name = "tabKeyWord";
            // 
            // grpTabChoiceKeyWord
            // 
            this.grpTabChoiceKeyWord.Items.Add(this.cbShowKeyword2);
            this.grpTabChoiceKeyWord.Items.Add(this.cbShowNoCode2);
            this.grpTabChoiceKeyWord.Items.Add(this.cbShowInov2);
            resources.ApplyResources(this.grpTabChoiceKeyWord, "grpTabChoiceKeyWord");
            this.grpTabChoiceKeyWord.Name = "grpTabChoiceKeyWord";
            // 
            // cbShowKeyword2
            // 
            resources.ApplyResources(this.cbShowKeyword2, "cbShowKeyword2");
            this.cbShowKeyword2.Name = "cbShowKeyword2";
            this.cbShowKeyword2.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.cbShowKeyword2_Click);
            // 
            // cbShowNoCode2
            // 
            resources.ApplyResources(this.cbShowNoCode2, "cbShowNoCode2");
            this.cbShowNoCode2.Name = "cbShowNoCode2";
            this.cbShowNoCode2.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.cbShowNoCode2_Click);
            // 
            // cbShowInov2
            // 
            resources.ApplyResources(this.cbShowInov2, "cbShowInov2");
            this.cbShowInov2.Name = "cbShowInov2";
            this.cbShowInov2.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.cbShowInov2_Click);
            // 
            // grpTabChoice
            // 
            this.grpTabChoice.Items.Add(this.cbShowKeyword);
            this.grpTabChoice.Items.Add(this.cbShowNoCode);
            this.grpTabChoice.Items.Add(this.cbShowInov);
            resources.ApplyResources(this.grpTabChoice, "grpTabChoice");
            this.grpTabChoice.Name = "grpTabChoice";
            // 
            // cbShowKeyword
            // 
            resources.ApplyResources(this.cbShowKeyword, "cbShowKeyword");
            this.cbShowKeyword.Name = "cbShowKeyword";
            this.cbShowKeyword.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.cbShowKeyword_Click);
            // 
            // cbShowNoCode
            // 
            resources.ApplyResources(this.cbShowNoCode, "cbShowNoCode");
            this.cbShowNoCode.Name = "cbShowNoCode";
            this.cbShowNoCode.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.cbShowNoCode_Click);
            // 
            // cbShowInov
            // 
            resources.ApplyResources(this.cbShowInov, "cbShowInov");
            this.cbShowInov.Name = "cbShowInov";
            this.cbShowInov.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.cbShowInov_Click);
            // 
            // tabNoCode
            // 
            this.tabNoCode.Groups.Add(this.grpNoCode);
            this.tabNoCode.Groups.Add(this.grpTools);
            this.tabNoCode.Groups.Add(this.grpTabChoice);
            this.tabNoCode.Groups.Add(this.grpContacts);
            this.tabNoCode.Groups.Add(this.group2);
            resources.ApplyResources(this.tabNoCode, "tabNoCode");
            this.tabNoCode.Name = "tabNoCode";
            // 
            // grpTools
            // 
            this.grpTools.Items.Add(this.BtChooseFile);
            this.grpTools.Items.Add(this.btExclude);
            this.grpTools.Items.Add(this.BtTargetHelp);
            resources.ApplyResources(this.grpTools, "grpTools");
            this.grpTools.Name = "grpTools";
            // 
            // BtChooseFile
            // 
            this.BtChooseFile.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            resources.ApplyResources(this.BtChooseFile, "BtChooseFile");
            this.BtChooseFile.Name = "BtChooseFile";
            this.BtChooseFile.OfficeImageId = "FileOpen";
            this.BtChooseFile.ShowImage = true;
            this.BtChooseFile.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.BtChooseFile_Click);
            // 
            // btExclude
            // 
            this.btExclude.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            resources.ApplyResources(this.btExclude, "btExclude");
            this.btExclude.Name = "btExclude";
            this.btExclude.OfficeImageId = "FileOpen";
            this.btExclude.ShowImage = true;
            this.btExclude.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btExclude_Click);
            // 
            // BtTargetHelp
            // 
            this.BtTargetHelp.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            resources.ApplyResources(this.BtTargetHelp, "BtTargetHelp");
            this.BtTargetHelp.Name = "BtTargetHelp";
            this.BtTargetHelp.OfficeImageId = "AccessFormWizard";
            this.BtTargetHelp.ShowImage = true;
            this.BtTargetHelp.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.BtTargetHelp_Click);
            // 
            // grpContacts
            // 
            this.grpContacts.Items.Add(this.BtSendMail);
            resources.ApplyResources(this.grpContacts, "grpContacts");
            this.grpContacts.Name = "grpContacts";
            // 
            // BtSendMail
            // 
            this.BtSendMail.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            resources.ApplyResources(this.BtSendMail, "BtSendMail");
            this.BtSendMail.Name = "BtSendMail";
            this.BtSendMail.OfficeImageId = "NewMessageToAttendees";
            this.BtSendMail.ShowImage = true;
            this.BtSendMail.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.BtSendMail_Click);
            // 
            // group2
            // 
            this.group2.Items.Add(this.lbVersion);
            resources.ApplyResources(this.group2, "group2");
            this.group2.Name = "group2";
            // 
            // lbVersion
            // 
            resources.ApplyResources(this.lbVersion, "lbVersion");
            this.lbVersion.Name = "lbVersion";
            // 
            // tabGherkinSample
            // 
            this.tabGherkinSample.Groups.Add(this.group1);
            resources.ApplyResources(this.tabGherkinSample, "tabGherkinSample");
            this.tabGherkinSample.Name = "tabGherkinSample";
            // 
            // group1
            // 
            this.group1.Items.Add(this.btGenerateGherkin);
            resources.ApplyResources(this.group1, "group1");
            this.group1.Name = "group1";
            // 
            // btGenerateGherkin
            // 
            this.btGenerateGherkin.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btGenerateGherkin.Image = global::AxaFrance.WebEngine.ExcelUI.Properties.Resources.cucumber;
            resources.ApplyResources(this.btGenerateGherkin, "btGenerateGherkin");
            this.btGenerateGherkin.Name = "btGenerateGherkin";
            this.btGenerateGherkin.ShowImage = true;
            this.btGenerateGherkin.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btGenerateGherkin_Click);
            // 
            // Ribbon
            // 
            this.Name = "Ribbon";
            this.RibbonType = "Microsoft.Excel.Workbook";
            this.Tabs.Add(this.tabWebEngine);
            this.Tabs.Add(this.tabKeyWord);
            this.Tabs.Add(this.tabNoCode);
            this.Tabs.Add(this.tabGherkinSample);
            this.Close += new System.EventHandler(this.Ribbon_Close);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.Ribbon_Load);
            this.tabWebEngine.ResumeLayout(false);
            this.tabWebEngine.PerformLayout();
            this.grpNoCode.ResumeLayout(false);
            this.grpNoCode.PerformLayout();
            this.grpKeyWord.ResumeLayout(false);
            this.grpKeyWord.PerformLayout();
            this.tabKeyWord.ResumeLayout(false);
            this.tabKeyWord.PerformLayout();
            this.grpTabChoiceKeyWord.ResumeLayout(false);
            this.grpTabChoiceKeyWord.PerformLayout();
            this.grpTabChoice.ResumeLayout(false);
            this.grpTabChoice.PerformLayout();
            this.tabNoCode.ResumeLayout(false);
            this.tabNoCode.PerformLayout();
            this.grpTools.ResumeLayout(false);
            this.grpTools.PerformLayout();
            this.grpContacts.ResumeLayout(false);
            this.grpContacts.PerformLayout();
            this.group2.ResumeLayout(false);
            this.group2.PerformLayout();
            this.tabGherkinSample.ResumeLayout(false);
            this.tabGherkinSample.PerformLayout();
            this.group1.ResumeLayout(false);
            this.group1.PerformLayout();
            this.ResumeLayout(false);

        }



        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tabWebEngine;
        internal RibbonTab tabKeyWord;
        internal RibbonTab tabNoCode;
        internal RibbonGroup grpKeyWord;
        internal RibbonGallery gryExport;
        internal RibbonButton btnExportSelection;
        internal RibbonButton btnExportAll;
        private RibbonButton btnExportAllTabs;
        private RibbonButton BtnExportFilter;
        internal RibbonButton btnExportEnvironmentVariable;
        internal RibbonGallery gryTools;
        internal RibbonButton btnCodeGeneration;
        private RibbonButton btnCodeGenerationJava;
        private RibbonButton btnDataCheck;
        private RibbonButton btnDataSync;
        internal RibbonSeparator separator2;
        internal RibbonButton btnStartNow;
        internal RibbonSeparator separator1;
        internal RibbonButton btnSettings;
        internal RibbonGallery gryHelp;
        private RibbonButton btnAbout;
        private RibbonButton btnHelp;
        private RibbonButton btnFeedback;
        internal RibbonButton generateGherkin;
        internal RibbonGroup grpNoCode;
        internal RibbonButton btnStartDriveByExcelNow;
        internal RibbonButton driveSettings;
        internal RibbonButton BtGotoDriveHelp;
        internal RibbonButton KeepassHelp;
        internal RibbonButton BtTargetHelp;
        internal RibbonButton BtChooseFile;
        internal RibbonGroup grpTabChoice;
        internal RibbonCheckBox cbShowKeyword;
        internal RibbonCheckBox cbShowNoCode;
        internal RibbonGroup grpTabChoiceKeyWord;
        internal RibbonCheckBox cbShowKeyword2;
        internal RibbonCheckBox cbShowNoCode2;
        internal RibbonGroup grpContacts;
        internal RibbonButton BtSendMail;
        internal RibbonTab tabGherkinSample;
        internal RibbonGroup group1;
        internal RibbonButton btGenerateGherkin;
        internal RibbonButton btExclude;
        internal RibbonGroup grpTools;
        internal RibbonGroup group2;
        internal RibbonLabel lbVersion;
        internal RibbonCheckBox cbShowInov;
        internal RibbonCheckBox cbShowInov2;
    }

    partial class ThisRibbonCollection
    {
        internal Ribbon Ribbon
        {
            get { return this.GetRibbon<Ribbon>(); }
        }
    }
}
