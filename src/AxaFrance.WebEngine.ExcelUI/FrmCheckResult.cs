// Copyright (c) 2016-2022 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// Modified By: YUAN Huaxing, at: 2022-5-13 18:26
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AxaFrance.WebEngine.ExcelUI
{
    public partial class FrmCheckResult : Form
    {
        public FrmCheckResult(List<string> parameters, List<string> testParameters, List<string> testcases)
        {
            InitializeComponent();

            //Select parameters listed in PARAMS but not used in TestCase
            var paramsNotInTest = parameters.Where(x => !testParameters.Contains(x));

            //Select parameters used in TestCase but not listed in PARAMS.
            var paramsNotRefered = testParameters.Where(x => !parameters.Contains(x));
            var duplicateKeys = from x in testcases group x by x into grouped where grouped.Count() > 1 select grouped.Key;

            StringBuilder problems = new StringBuilder();
            if (paramsNotInTest.Count() > 0)
            {
                problems.AppendLine("Following parameters are listed in PARAMS Tab but not referred in the current Tab: ");
                problems.AppendLine(string.Join(", ", paramsNotInTest.ToArray()));
                problems.AppendLine("Please valorise them in your test case.");
                problems.AppendLine();
            }

            if (paramsNotRefered.Count() > 0)
            {
                problems.AppendLine("Following parameters are used in your test case but not referred in PARAMS:");
                problems.AppendLine(string.Join(", ", paramsNotRefered.ToArray()));
                problems.AppendLine("Please refer these parameters in the worksheet PARAMS");
                problems.AppendLine();
            }

            if (duplicateKeys.Count() > 0)
            {
                problems.AppendLine("Some of your test cases have duplicated names:");
                problems.AppendLine(string.Join(", ", duplicateKeys.ToArray()));
                problems.AppendLine("Please fixe this issue before launching the test. If there are duplicated names, Test Framework will select the first one in the list when used.");
                problems.AppendLine();
            }

            if (problems.Length == 0)
            {
                problems.AppendLine("Congratulations, There are no problems detected !");
            }

            txtProblems.Text = problems.ToString();

        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
