@echo off
setlocal EnableExtensions

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
set "scriptDir=%~dp0"
set "file=axafrance.webengine.webrunner.nuspec"
set "nuspec=%scriptDir%%file%"
set "buildDir=%scriptDir%bin\%buildType%"
set "nuget=%scriptDir%..\nuget.exe"

if not exist "%nuspec%" (
    echo ERROR: Nuspec file not found: "%nuspec%"
    exit /b 1
)

if not exist "%buildDir%" (
    echo ERROR: Build output directory not found: "%buildDir%"
    exit /b 1
)

if not exist "%nuget%" (
    echo ERROR: NuGet executable not found: "%nuget%"
    exit /b 1
)

copy /Y "%nuspec%" "%buildDir%\%file%" >nul
if errorlevel 1 (
    echo ERROR: Could not copy the nuspec file.
    exit /b 1
)

echo Generate Nuget Package.
powershell -NoProfile -Command "$path = '%buildDir%\%file%'; $content = [System.IO.File]::ReadAllText($path); $content = $content.Replace('{{version}}', '%version%'); [System.IO.File]::WriteAllText($path, $content)"
if errorlevel 1 (
    echo ERROR: Could not apply the package version.
    exit /b 1
)

"%nuget%" pack "%buildDir%\%file%" -OutputDirectory "%scriptDir%.."
set "exitCode=%ERRORLEVEL%"
endlocal & exit /b %exitCode%