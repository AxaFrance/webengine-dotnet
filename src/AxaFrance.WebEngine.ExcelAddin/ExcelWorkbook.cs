// Copyright (c) 2016-2026 AXA France IARD / AXA France VIE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Runtime.InteropServices;
using ExcelDna.Integration;

namespace AxaFrance.WebEngine.ExcelAddin
{
    /// <summary>
    /// Snapshot of a worksheet used range, with absolute A1 coordinates.
    /// </summary>
    internal sealed record SheetSnapshot(string Name, object[,] Cells);

    /// <summary>
    /// Thin, release-safe wrapper around the Excel COM object model.
    /// </summary>
    internal static class ExcelWorkbook
    {
        internal static dynamic Application => ExcelDnaUtil.Application;

        internal static SheetSnapshot ReadActiveSheet()
        {
            dynamic? application = null;
            dynamic? sheet = null;
            try
            {
                application = Application;
                sheet = application.ActiveSheet;
                return ReadSheetCore(sheet);
            }
            finally
            {
                Release(sheet);
                Release(application);
            }
        }

        internal static SheetSnapshot ReadSheet(string sheetName)
        {
            dynamic? application = null;
            dynamic? workbook = null;
            dynamic? sheet = null;
            try
            {
                application = Application;
                workbook = application.ActiveWorkbook;
                if (workbook == null)
                {
                    throw new InvalidOperationException("No workbook is currently open.");
                }

                sheet = workbook.Worksheets[sheetName];
                return ReadSheetCore(sheet);
            }
            finally
            {
                Release(sheet);
                Release(workbook);
                Release(application);
            }
        }

        internal static IReadOnlyCollection<int> GetSelectedColumns()
        {
            dynamic? application = null;
            dynamic? selection = null;
            dynamic? areas = null;
            try
            {
                application = Application;
                selection = application.Selection;
                areas = selection.Areas;

                var columns = new SortedSet<int>();
                int areaCount = (int)areas.Count;
                for (int i = 1; i <= areaCount; i++)
                {
                    dynamic? area = null;
                    try
                    {
                        area = areas.Item(i);
                        int firstColumn = (int)area.Column;
                        int columnCount = (int)area.Columns.Count;
                        for (int column = firstColumn; column < firstColumn + columnCount; column++)
                        {
                            columns.Add(column);
                        }
                    }
                    finally
                    {
                        Release(area);
                    }
                }

                return columns;
            }
            finally
            {
                Release(areas);
                Release(selection);
                Release(application);
            }
        }

        internal static void SyncParameterRows(
            IReadOnlyList<string> parameterNames,
            IReadOnlyList<string> existingNames)
        {
            dynamic? application = null;
            dynamic? sheet = null;
            try
            {
                application = Application;
                sheet = application.ActiveSheet;

                var existing = new HashSet<string>(existingNames, StringComparer.Ordinal);
                for (int i = 0; i < parameterNames.Count; i++)
                {
                    string parameter = parameterNames[i];
                    if (!existing.Add(parameter))
                    {
                        continue;
                    }

                    int rowIndex = ExcelTestDataReader.TestCaseRow + i;
                    dynamic? row = null;
                    dynamic? cell = null;
                    try
                    {
                        row = sheet.Rows[rowIndex];
                        row.Insert();
                        cell = sheet.Cells[rowIndex, 1];
                        cell.Value2 = parameter;
                    }
                    finally
                    {
                        Release(cell);
                        Release(row);
                    }
                }
            }
            finally
            {
                Release(sheet);
                Release(application);
            }
        }

        private static SheetSnapshot ReadSheetCore(dynamic sheet)
        {
            dynamic? usedRange = null;
            dynamic? startCell = null;
            dynamic? endCell = null;
            dynamic? range = null;
            try
            {
                usedRange = sheet.UsedRange;
                int lastRow = (int)usedRange.Row + (int)usedRange.Rows.Count - 1;
                int lastColumn = (int)usedRange.Column + (int)usedRange.Columns.Count - 1;

                startCell = sheet.Cells[1, 1];
                endCell = sheet.Cells[lastRow, lastColumn];
                range = sheet.Range[startCell, endCell];

                object? value = range.Value2;
                object[,] cells = value as object[,] ?? new object[,] { { value } };
                return new SheetSnapshot((string)sheet.Name, cells);
            }
            finally
            {
                Release(range);
                Release(endCell);
                Release(startCell);
                Release(usedRange);
            }
        }

        internal static void Release(object? comObject)
        {
            if (comObject == null)
            {
                return;
            }

            try
            {
                if (Marshal.IsComObject(comObject))
                {
                    Marshal.ReleaseComObject(comObject);
                }
            }
            catch
            {
                // COM cleanup must never break an add-in command.
            }
        }
    }
}
