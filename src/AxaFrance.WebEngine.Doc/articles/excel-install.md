# Install WebEngine Add-in for Excel

The add-in is an Excel-DNA `.xll` file. It does not use VSTO, a COM registration, or an installer.

## Prerequisites

- **64-bit Excel** (Excel 2013 or later). The add-in targets 64-bit Office only.
- **.NET 8 Desktop Runtime (x64)**: [download](https://dotnet.microsoft.com/download/dotnet/8.0).

## Install

1. Download `AxaFrance.WebEngine.ExcelAddin-AddIn64-packed.xll` from the [GitHub releases page](https://github.com/AxaFrance/webengine-dotnet/releases).

> [!TIP]
> Development builds are also published by the **Build Excel add-in** GitHub Actions workflow.
> Open the latest successful run and download the `webengine-excel-addin-x64-*` artifact.
2. If the file was downloaded from the internet, right-click it, open **Properties**, and select **Unblock**.
3. In Excel, open **File > Options > Add-ins**.
4. At the bottom, select **Excel Add-ins** in the **Manage** list and click **Go...**.
5. Click **Browse...**, select the `.xll` file, and confirm.
6. The **WebEngine** tab appears in the ribbon.

## Verify the installation

Enter the following formula in any cell:

```text
=WEBENGINE_ADDIN_VERSION()
```

It returns the installed add-in version. If the formula is not recognized, check the prerequisites and make sure the add-in is enabled in the Excel Add-ins dialog.

## Enterprise deployment

For managed environments:

- Sign the XLL with your organization's code-signing certificate.
- Distribute it with your usual software deployment tool (Intune, SCCM, MSI wrapper, etc.).
- Add the XLL path to the user's Excel add-in list, or let users add it once through the Excel options.

## Troubleshooting

| Symptom | Cause / fix |
|---|---|
| The WebEngine tab does not appear | The XLL is disabled in **File > Options > Add-ins > Manage: Disabled Items**. Re-enable it. |
| "This file is blocked" | Unblock the XLL in its file properties. |
| .NET runtime error when Excel starts | Install the .NET 8 Desktop Runtime (x64). |
| The add-in does not load | Make sure Excel is 64-bit; the 32-bit XLL is not built. |
