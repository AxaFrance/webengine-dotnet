@echo off
if "%~1"=="" (
    set "version=1.0.0.0"
) else (
    set "version=%~1"
)

if "%~2"=="" (
    set "buildType=Debug"
) else (
    set "buildType=%~2"
)


echo Build project Version: %version%, Type: %buildType%
cd bin
setlocal enabledelayedexpansion

rem Loop through all files in the current directory and subdirectories
for /r %%f in (*) do (
    set "filename=%%~nxf"
    rem Check if the filename starts with WebRunner.
    if /i "!filename!" neq "ReportViewer.exe" (
        if /i "!filename!" neq "ReportViewer.dll" (
            if /i "!filename:~0,13!" neq "ReportViewer." (
                del "%%f"
            )
        )
    )
)
endlocal
set file=AxaFrance.WebEngine.ReportViewer.nuspec
copy ..\%file% %buildType%
cd %buildType%
echo Generate Nuget Package.
powershell -Command "(Get-Content '%file%') -replace '{{Version}}', '%version%' | Set-Content '%file%'"
cd ..
cd ..
cd ..
nuget pack AxaFrance.WebEngine.ReportViewer\bin\%buildType%
@echo on