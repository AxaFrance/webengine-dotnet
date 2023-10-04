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
            this.grpTestData = this.Factory.CreateRibbonGroup();
            this.grpExecution = this.Factory.CreateRibbonGroup();
            this.grpHelp = this.Factory.CreateRibbonGroup();
            this.NoCodeHelp = this.Factory.CreateRibbonGroup();
            this.separator1 = this.Factory.CreateRibbonSeparator();
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
            this.btnStartNow = this.Factory.CreateRibbonButton();
            this.btnStartDriveByExcelNow = this.Factory.CreateRibbonButton();
            this.btnSettings = this.Factory.CreateRibbonButton();
            this.gryHelp = this.Factory.CreateRibbonGallery();
            this.btnAbout = this.Factory.CreateRibbonButton();
            this.btnHelp = this.Factory.CreateRibbonButton();
            this.btnFeedback = this.Factory.CreateRibbonButton();
            this.generateGherkin = this.Factory.CreateRibbonButton();
            this.driveSettings = this.Factory.CreateRibbonButton();
            this.BtGotoDriveHelp = this.Factory.CreateRibbonButton();
            this.KeepassHelp = this.Factory.CreateRibbonButton();
            this.BtTargetHelp = this.Factory.CreateRibbonButton();
            this.tabWebEngine.SuspendLayout();
            this.grpTestData.SuspendLayout();
            this.grpExecution.SuspendLayout();
            this.grpHelp.SuspendLayout();
            this.NoCodeHelp.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabWebEngine
            // 
            this.tabWebEngine.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tabWebEngine.Groups.Add(this.grpTestData);
            this.tabWebEngine.Groups.Add(this.grpExecution);
            this.tabWebEngine.Groups.Add(this.grpHelp);
            this.tabWebEngine.Groups.Add(this.NoCodeHelp);
            resources.ApplyResources(this.tabWebEngine, "tabWebEngine");
            this.tabWebEngine.Name = "tabWebEngine";
            // 
            // grpTestData
            // 
            this.grpTestData.Items.Add(this.gryExport);
            this.grpTestData.Items.Add(this.gryTools);
            resources.ApplyResources(this.grpTestData, "grpTestData");
            this.grpTestData.Name = "grpTestData";
            // 
            // grpExecution
            // 
            this.grpExecution.Items.Add(this.btnStartNow);
            this.grpExecution.Items.Add(this.btnStartDriveByExcelNow);
            resources.ApplyResources(this.grpExecution, "grpExecution");
            this.grpExecution.Name = "grpExecution";
            // 
            // grpHelp
            // 
            this.grpHelp.Items.Add(this.btnSettings);
            this.grpHelp.Items.Add(this.gryHelp);
            this.grpHelp.Items.Add(this.generateGherkin);
            resources.ApplyResources(this.grpHelp, "grpHelp");
            this.grpHelp.Name = "grpHelp";
            // 
            // NoCodeHelp
            // 
            this.NoCodeHelp.Items.Add(this.driveSettings);
            this.NoCodeHelp.Items.Add(this.BtGotoDriveHelp);
            this.NoCodeHelp.Items.Add(this.KeepassHelp);
            this.NoCodeHelp.Items.Add(this.separator1);
            this.NoCodeHelp.Items.Add(this.BtTargetHelp);
            resources.ApplyResources(this.NoCodeHelp, "NoCodeHelp");
            this.NoCodeHelp.Name = "NoCodeHelp";
            // 
            // separator1
            // 
            this.separator1.Name = "separator1";
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
            // btnStartNow
            // 
            this.btnStartNow.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            resources.ApplyResources(this.btnStartNow, "btnStartNow");
            this.btnStartNow.Name = "btnStartNow";
            this.btnStartNow.OfficeImageId = "PlayFromPage";
            this.btnStartNow.ShowImage = true;
            this.btnStartNow.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnStartNow_Click);
            // 
            // btnStartDriveByExcelNow
            // 
            this.btnStartDriveByExcelNow.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnStartDriveByExcelNow.Image = global::AxaFrance.WebEngine.ExcelUI.Properties.Resources.drive_by_excel;
            resources.ApplyResources(this.btnStartDriveByExcelNow, "btnStartDriveByExcelNow");
            this.btnStartDriveByExcelNow.Name = "btnStartDriveByExcelNow";
            this.btnStartDriveByExcelNow.ShowImage = true;
            this.btnStartDriveByExcelNow.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnStartDriveByExcelNow_Click_1);
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
            // BtTargetHelp
            // 
            this.BtTargetHelp.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            resources.ApplyResources(this.BtTargetHelp, "BtTargetHelp");
            this.BtTargetHelp.Name = "BtTargetHelp";
            this.BtTargetHelp.OfficeImageId = "AccessFormWizard";
            this.BtTargetHelp.ShowImage = true;
            this.BtTargetHelp.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.BtTargetHelp_Click);
            // 
            // Ribbon
            // 
            this.Name = "Ribbon";
            this.RibbonType = "Microsoft.Excel.Workbook";
            this.Tabs.Add(this.tabWebEngine);
            this.Close += new System.EventHandler(this.Ribbon_Close);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.Ribbon_Load);
            this.tabWebEngine.ResumeLayout(false);
            this.tabWebEngine.PerformLayout();
            this.grpTestData.ResumeLayout(false);
            this.grpTestData.PerformLayout();
            this.grpExecution.ResumeLayout(false);
            this.grpExecution.PerformLayout();
            this.grpHelp.ResumeLayout(false);
            this.grpHelp.PerformLayout();
            this.NoCodeHelp.ResumeLayout(false);
            this.NoCodeHelp.PerformLayout();
            this.ResumeLayout(false);

        }



        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tabWebEngine;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup grpTestData;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnExportAll;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnExportSelection;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnExportEnvironmentVariable;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup grpHelp;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnSettings;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup grpExecution;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnCodeGeneration;
        internal Microsoft.Office.Tools.Ribbon.RibbonGallery gryTools;
        internal Microsoft.Office.Tools.Ribbon.RibbonGallery gryExport;
        internal Microsoft.Office.Tools.Ribbon.RibbonGallery gryHelp;
        private Microsoft.Office.Tools.Ribbon.RibbonButton btnAbout;
        private Microsoft.Office.Tools.Ribbon.RibbonButton btnHelp;
        private Microsoft.Office.Tools.Ribbon.RibbonButton btnFeedback;
        private Microsoft.Office.Tools.Ribbon.RibbonButton btnDataCheck;
        private Microsoft.Office.Tools.Ribbon.RibbonButton btnDataSync;
        private Microsoft.Office.Tools.Ribbon.RibbonButton BtnExportFilter;
        private Microsoft.Office.Tools.Ribbon.RibbonButton btnExportAllTabs;
        private Microsoft.Office.Tools.Ribbon.RibbonButton btnCodeGenerationJava;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnStartNow;
        internal RibbonButton btnStartDriveByExcelNow;
        internal RibbonButton driveSettings;
        internal RibbonButton generateGherkin;
        internal RibbonGroup NoCodeHelp;
        internal RibbonButton BtTargetHelp;
        internal RibbonButton KeepassHelp;
        internal RibbonSeparator separator1;
        internal RibbonButton BtGotoDriveHelp;
    }

    partial class ThisRibbonCollection
    {
        internal Ribbon Ribbon
        {
            get { return this.GetRibbon<Ribbon>(); }
        }
    }
}
