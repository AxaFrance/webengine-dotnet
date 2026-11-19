// Copyright (c) 2016-2026 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using AxaFrance.WebEngine;

namespace AxaFrance.WebEngine.ExcelAddin
{
    /// <summary>
    /// Result of reading a test data worksheet.
    /// </summary>
    public sealed class TestDataReadResult
    {
        /// <summary>
        /// Test cases read from the worksheet.
        /// </summary>
        public List<TestData> TestData { get; } = new();

        /// <summary>
        /// Names of the exported test cases, in worksheet order.
        /// </summary>
        public List<string> TestCaseNames { get; } = new();

        /// <summary>
        /// Duplicated test case names found in the selection.
        /// </summary>
        public List<string> DuplicateTestCaseNames { get; } = new();

        /// <summary>
        /// True when at least one duplicated test case name was found.
        /// </summary>
        public bool HasDuplicates => DuplicateTestCaseNames.Count > 0;
    }

    /// <summary>
    /// Definition of a parameter read from the PARAMS worksheet.
    /// </summary>
    public sealed record ParameterDefinition(string Name, string Description);

    /// <summary>
    /// Reads WebEngine test data from a worksheet snapshot.
    /// The worksheet layout is:
    /// row 3, column A: TESTCASE header; row 3, column C and onwards: test case names;
    /// column A from row 3 onwards: parameter names; other columns: parameter values.
    /// </summary>
    public static class ExcelTestDataReader
    {
        /// <summary>Row containing test case names.</summary>
        public const int TestCaseRow = 3;

        /// <summary>First column containing test case values.</summary>
        public const int TestCaseStartColumn = 3;

        /// <summary>Column containing parameter names.</summary>
        public const int VariableColumn = 1;

        /// <summary>First row containing parameter definitions in the PARAMS worksheet.</summary>
        public const int ParametersStartRow = 2;

        /// <summary>
        /// Reads test cases from a worksheet snapshot.
        /// </summary>
        /// <param name="cells">Used range values, with absolute A1 coordinates.</param>
        /// <param name="selectedColumns">Selected worksheet columns, or null to read all test case columns.</param>
        /// <param name="testCaseFilter">Optional filter applied to test case names.</param>
        public static TestDataReadResult ReadTestData(
            object[,] cells,
            IEnumerable<int>? selectedColumns = null,
            Func<string, bool>? testCaseFilter = null)
        {
            ArgumentNullException.ThrowIfNull(cells);

            var result = new TestDataReadResult();
            var columns = selectedColumns == null
                ? DetectTestCaseColumns(cells)
                : NormalizeSelectedColumns(selectedColumns);

            var seenNames = new HashSet<string>(StringComparer.Ordinal);
            int lastRow = GetLastRow(cells);

            foreach (int column in columns)
            {
                var variables = new List<Variable>();
                for (int row = TestCaseRow; row <= lastRow; row++)
                {
                    string? variableName = GetCellAsString(cells, row, VariableColumn);
                    if (string.IsNullOrWhiteSpace(variableName))
                    {
                        break;
                    }

                    variables.Add(new Variable
                    {
                        Name = variableName,
                        Value = GetCellAsString(cells, row, column) ?? string.Empty,
                    });
                }

                string? testName = variables
                    .FirstOrDefault(x => string.Equals(x.Name, "TESTCASE", StringComparison.OrdinalIgnoreCase))
                    ?.Value;

                if (string.IsNullOrWhiteSpace(testName))
                {
                    continue;
                }

                testName = testName.Trim();
                if (testCaseFilter != null && !testCaseFilter(testName))
                {
                    continue;
                }

                if (!seenNames.Add(testName))
                {
                    result.DuplicateTestCaseNames.Add(testName);
                    continue;
                }

                result.TestData.Add(new TestData
                {
                    TestName = testName,
                    Data = variables.ToArray(),
                });
                result.TestCaseNames.Add(testName);
            }

            return result;
        }

        /// <summary>
        /// Detects test case columns from row 3, starting at column C and stopping at the first blank cell.
        /// </summary>
        public static IReadOnlyList<int> DetectTestCaseColumns(object[,] cells)
        {
            ArgumentNullException.ThrowIfNull(cells);

            var columns = new List<int>();
            int lastColumn = GetLastColumn(cells);
            for (int column = TestCaseStartColumn; column <= lastColumn; column++)
            {
                if (string.IsNullOrWhiteSpace(GetCellAsString(cells, TestCaseRow, column)))
                {
                    break;
                }

                columns.Add(column);
            }

            return columns;
        }

        /// <summary>
        /// Reads test case names from row 3.
        /// </summary>
        public static IReadOnlyList<string> ReadTestCaseNames(object[,] cells)
        {
            return DetectTestCaseColumns(cells)
                .Select(column => GetCellAsString(cells, TestCaseRow, column))
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Select(name => name!.Trim())
                .ToList();
        }

        /// <summary>
        /// Reads parameter names from the PARAMS worksheet.
        /// </summary>
        public static IReadOnlyList<string> ReadParameterNames(object[,] cells)
        {
            ArgumentNullException.ThrowIfNull(cells);

            var names = new List<string>();
            int lastRow = GetLastRow(cells);
            for (int row = ParametersStartRow; row <= lastRow; row++)
            {
                string? name = GetCellAsString(cells, row, 1);
                if (string.IsNullOrWhiteSpace(name))
                {
                    break;
                }

                names.Add(name);
            }

            return names;
        }

        /// <summary>
        /// Reads parameter names used by the active test data worksheet.
        /// </summary>
        public static IReadOnlyList<string> ReadTestParameterNames(object[,] cells)
        {
            ArgumentNullException.ThrowIfNull(cells);

            var names = new List<string>();
            int lastRow = GetLastRow(cells);
            for (int row = TestCaseRow; row <= lastRow; row++)
            {
                string? name = GetCellAsString(cells, row, VariableColumn);
                if (string.IsNullOrWhiteSpace(name))
                {
                    break;
                }

                names.Add(name);
            }

            return names;
        }

        /// <summary>
        /// Reads parameter definitions from the PARAMS worksheet for code generation.
        /// </summary>
        public static IReadOnlyList<ParameterDefinition> ReadParameterDefinitions(object[,] cells)
        {
            ArgumentNullException.ThrowIfNull(cells);

            var parameters = new List<ParameterDefinition>();
            int lastRow = GetLastRow(cells);
            for (int row = ParametersStartRow; row <= lastRow; row++)
            {
                string? name = GetCellAsString(cells, row, 1);
                string? description = GetCellAsString(cells, row, 2);

                if (string.IsNullOrWhiteSpace(name) && string.IsNullOrWhiteSpace(description))
                {
                    break;
                }

                if (!string.IsNullOrWhiteSpace(name) && !name.Contains(' '))
                {
                    parameters.Add(new ParameterDefinition(name, description ?? string.Empty));
                }
            }

            return parameters;
        }

        /// <summary>
        /// Reads environment variables from the ENV worksheet.
        /// The optional NAME/VALUE header row is ignored.
        /// </summary>
        public static IReadOnlyList<Variable> ReadEnvironmentVariables(object[,] cells)
        {
            ArgumentNullException.ThrowIfNull(cells);

            var variables = new List<Variable>();
            int lastRow = GetLastRow(cells);
            int firstRow = IsEnvironmentHeader(cells, 1) ? 2 : 1;

            for (int row = firstRow; row <= lastRow; row++)
            {
                string? name = GetCellAsString(cells, row, 1);
                if (string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }

                variables.Add(new Variable
                {
                    Name = name,
                    Value = GetCellAsString(cells, row, 2) ?? string.Empty,
                });
            }

            return variables;
        }

        private static bool IsEnvironmentHeader(object[,] cells, int row)
        {
            return string.Equals(GetCellAsString(cells, row, 1), "NAME", StringComparison.OrdinalIgnoreCase)
                && string.Equals(GetCellAsString(cells, row, 2), "VALUE", StringComparison.OrdinalIgnoreCase);
        }

        private static IReadOnlyList<int> NormalizeSelectedColumns(IEnumerable<int> selectedColumns)
        {
            return selectedColumns
                .Where(column => column >= TestCaseStartColumn)
                .Distinct()
                .OrderBy(column => column)
                .ToList();
        }

        private static int GetLastRow(object[,] cells)
        {
            return cells.GetUpperBound(0) - cells.GetLowerBound(0) + 1;
        }

        private static int GetLastColumn(object[,] cells)
        {
            return cells.GetUpperBound(1) - cells.GetLowerBound(1) + 1;
        }

        private static string? GetCellAsString(object[,] cells, int row, int column)
        {
            object? value = GetCell(cells, row, column);
            return value?.ToString()?.Trim();
        }

        internal static object? GetCell(object[,] cells, int row, int column)
        {
            if (row < 1 || column < 1)
            {
                return null;
            }

            int rowIndex = cells.GetLowerBound(0) + row - 1;
            int columnIndex = cells.GetLowerBound(1) + column - 1;
            if (rowIndex > cells.GetUpperBound(0) || columnIndex > cells.GetUpperBound(1))
            {
                return null;
            }

            return cells[rowIndex, columnIndex];
        }
    }
}
