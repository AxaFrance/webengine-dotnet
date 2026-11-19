// Copyright (c) 2016-2026 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
namespace AxaFrance.WebEngine.ExcelAddin
{
    /// <summary>
    /// Test case name filter used by the export-with-filter command.
    /// </summary>
    public sealed class TestCaseFilter
    {
        private readonly string[] _terms;

        /// <summary>
        /// Creates a filter from a semicolon or comma separated list of terms.
        /// </summary>
        public TestCaseFilter(string? text, bool exportMatches)
        {
            Text = text ?? string.Empty;
            ExportMatches = exportMatches;
            _terms = Text.Split(
                new[] { ';', ',' },
                StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }

        /// <summary>
        /// Raw filter text.
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// True to export matching test cases, false to export the others.
        /// </summary>
        public bool ExportMatches { get; }

        /// <summary>
        /// Returns true when the test case is retained by this filter.
        /// </summary>
        public bool IsMatch(string testCaseName)
        {
            if (_terms.Length == 0)
            {
                return true;
            }

            bool match = _terms.Any(term => testCaseName.Contains(term, StringComparison.OrdinalIgnoreCase));
            return ExportMatches ? match : !match;
        }
    }
}
