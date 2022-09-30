// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AXA.WebEngine.ExcelUI
{
    public partial class FrmSelectFilter : Form
    {
        public FrmSelectFilter()
        {
            InitializeComponent();
        }

        public string Filter { get; set; }

        /// <summary>
        /// True: export only matched test cases, False: export not matched test cases.
        /// </summary>
        public bool FilterType { get; set; }

        private void btnStart_Click(object sender, EventArgs e)
        {
            Filter = txtFilters.Text;
            FilterType = rbExportMatches.Checked;
            this.DialogResult = DialogResult.OK;
        }
    }
}
