// Copyright (c) 2016-2026 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Text;
using System.Windows.Forms;

namespace AxaFrance.WebEngine.ExcelAddin
{
    internal sealed class FrmCheckResult : Form
    {
        internal FrmCheckResult(
            IReadOnlyList<string> parameters,
            IReadOnlyList<string> testParameters,
            IReadOnlyList<string> testCases)
        {
            UiTheme.StyleForm(this, "Check Test Data", new Size(680, 450));

            var txtProblems = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Both,
                WordWrap = false,
                Location = new Point(12, 12),
                Size = new Size(656, 375),
                BackColor = SystemColors.Window,
            };

            var btnClose = UiTheme.CreateButton("Close", 578, 408, 90, 30);
            btnClose.DialogResult = DialogResult.OK;

            AcceptButton = btnClose;
            CancelButton = btnClose;

            txtProblems.Text = BuildReport(parameters, testParameters, testCases);
            Controls.AddRange(new Control[] { txtProblems, btnClose });
        }

        private static string BuildReport(
            IReadOnlyList<string> parameters,
            IReadOnlyList<string> testParameters,
            IReadOnlyList<string> testCases)
        {
            var parametersNotInTest = parameters.Where(x => !testParameters.Contains(x)).ToList();
            var parametersNotReferenced = testParameters.Where(x => !parameters.Contains(x)).ToList();
            var duplicateKeys = testCases
                .GroupBy(x => x, StringComparer.Ordinal)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToList();

            var problems = new StringBuilder();
            if (parametersNotInTest.Count > 0)
            {
                problems.AppendLine("Following parameters are listed in PARAMS but not referenced in the current sheet:");
                problems.AppendLine(string.Join(", ", parametersNotInTest));
                problems.AppendLine("Please use them in your test cases.");
                problems.AppendLine();
            }

            if (parametersNotReferenced.Count > 0)
            {
                problems.AppendLine("Following parameters are used in the current sheet but not listed in PARAMS:");
                problems.AppendLine(string.Join(", ", parametersNotReferenced));
                problems.AppendLine("Please declare them in the PARAMS worksheet.");
                problems.AppendLine();
            }

            if (duplicateKeys.Count > 0)
            {
                problems.AppendLine("Some test cases have duplicated names:");
                problems.AppendLine(string.Join(", ", duplicateKeys));
                problems.AppendLine("Please fix this issue before launching the test.");
                problems.AppendLine();
            }

            if (problems.Length == 0)
            {
                problems.AppendLine("Congratulations, no problem was detected.");
            }

            return problems.ToString();
        }
    }
}
