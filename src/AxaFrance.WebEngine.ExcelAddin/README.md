# AxaFrance.WebEngine.ExcelAddin

Excel-DNA add-in for the AXA WebEngine test automation framework.

It manages WebEngine test data in Excel and launches data-driven test runs without VSTO.

## Features

- Export selected test case columns to WebRunner XML.
- Export all test cases of the active worksheet.
- Export test cases with include/exclude name filters.
- Export the `ENV` worksheet.
- Check parameters against the `PARAMS` worksheet and detect duplicated test cases.
- Insert missing parameters from `PARAMS` into a test data sheet.
- Generate C# or Java parameter classes.
- Launch selected tests with WebRunner (desktop and mobile browsers/apps).
- Stop running WebRunner processes.

## Worksheet layout

The format is unchanged from the former VSTO add-in:

- `PARAMS`: parameter names in column A, descriptions in column B.
- `ENV`: variable names in column A, values in column B (`NAME`/`VALUE` header optional).
- Test data sheet: row 3 contains `TESTCASE` in column A and test case names from column C.
  Column A from row 3 onwards contains parameter names; other columns contain values.

## Build

```powershell
dotnet build src/AxaFrance.WebEngine.ExcelAddin/AxaFrance.WebEngine.ExcelAddin.csproj -c Release
```

The build produces, in `bin/Release/net8.0-windows/`:

- `AxaFrance.WebEngine.ExcelAddin-AddIn64.xll` — unpacked add-in.
- `AxaFrance.WebEngine.ExcelAddin-AddIn64-packed.xll` — self-contained add-in for distribution.

The add-in targets `net8.0-windows` and requires the **.NET 8 Desktop Runtime** (x64) and 64-bit Excel.

## Install

1. Unblock the `.xll` file if it was downloaded (`Right-click > Properties > Unblock`).
2. In Excel: `File > Options > Add-ins > Manage: Excel Add-ins > Go... > Browse...`.
3. Select `AxaFrance.WebEngine.ExcelAddin-AddIn64-packed.xll`.
4. The `WebEngine` ribbon tab appears.

For enterprise deployment, sign the XLL and distribute it with your usual software deployment tool.

## Settings

Settings are stored in `%APPDATA%\AxaFrance.WebEngine\excelSetting.xml`, the same location used by the former VSTO add-in.

- **Export folder**: where `ENV.xml` and test data XML files are written.
- **WebRunner folder**: folder containing `WebRunner.exe` or `WebRunner.jar`.
- **Test assembly**: compiled test project (`.dll`) or Java test archive (`.jar`).
- **Default browser**: preselected in the launch dialog.
- **Excluded tabs**: worksheets ignored by export commands.

## Dependencies

- [Excel-DNA](https://excel-dna.net/) 1.9 — zlib license.
