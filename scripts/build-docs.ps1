[CmdletBinding()]
param(
    [string]$DocFxVersion = '2.81.0',
    [string]$ConfigurationPath = 'src/AxaFrance.WebEngine.Doc/docfx.json'
)

$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path -Parent $PSScriptRoot
$configuration = Join-Path $repoRoot ($ConfigurationPath.Replace('/', [System.IO.Path]::DirectorySeparatorChar))
$toolPath = Join-Path ([System.IO.Path]::GetTempPath()) ("webengine-docfx-" + [Guid]::NewGuid().ToString('N'))

if (-not (Test-Path -LiteralPath $configuration -PathType Leaf)) {
    throw "DocFX configuration was not found: $configuration"
}

try {
    & dotnet tool install docfx --tool-path $toolPath --version $DocFxVersion --verbosity minimal
    if ($LASTEXITCODE -ne 0) {
        throw "DocFX installation failed with exit code $LASTEXITCODE."
    }

    $docfx = Join-Path $toolPath 'docfx.exe'
    & $docfx $configuration
    if ($LASTEXITCODE -ne 0) {
        throw "DocFX build failed with exit code $LASTEXITCODE."
    }
}
finally {
    if (Test-Path -LiteralPath $toolPath -PathType Container) {
        Remove-Item -LiteralPath $toolPath -Recurse -Force
    }
}
