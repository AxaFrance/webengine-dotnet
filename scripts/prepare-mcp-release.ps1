[CmdletBinding()]
param(
    [string]$Version,
    [string]$OutputDirectory = 'artifacts\mcp',
    [switch]$NoRestore,
    [switch]$RunPublisherValidation
)

$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path -Parent $PSScriptRoot
$projectPath = Join-Path $repoRoot 'src/AxaFrance.WebEngine.Mcp/AxaFrance.WebEngine.Mcp.csproj'

if (-not (Test-Path -LiteralPath $projectPath -PathType Leaf)) {
    throw "MCP project file is missing: $projectPath"
}

[xml]$project = Get-Content -LiteralPath $projectPath -Raw
$propertyGroup = @($project.Project.PropertyGroup | Where-Object { $null -ne $_.Version }) | Select-Object -First 1
$projectVersion = [string]$propertyGroup.Version

if ([string]::IsNullOrWhiteSpace($Version)) {
    $Version = $projectVersion
}

if ($Version -ne $projectVersion) {
    throw "Requested version $Version does not match the MCP project Version $projectVersion. Update the metadata files together before packaging."
}

$validationScript = Join-Path $PSScriptRoot 'validate-mcp-metadata.ps1'
& $validationScript -RunPublisherValidation:$RunPublisherValidation
if (-not $?) {
    throw 'MCP metadata validation failed.'
}

$outputPath = if ([System.IO.Path]::IsPathRooted($OutputDirectory)) {
    $OutputDirectory
}
else {
    Join-Path $repoRoot $OutputDirectory
}

New-Item -ItemType Directory -Path $outputPath -Force | Out-Null

$packArguments = @(
    'pack',
    $projectPath,
    '--configuration',
    'Release',
    '--output',
    $outputPath,
    '--verbosity',
    'minimal'
)

if ($NoRestore) {
    $packArguments += '--no-restore'
}

Write-Output "Packing AxaFrance.WebEngine.Mcp $Version..."
& dotnet @packArguments
if ($LASTEXITCODE -ne 0) {
    throw "dotnet pack failed with exit code $LASTEXITCODE."
}

$packagePath = Join-Path $outputPath "AxaFrance.WebEngine.Mcp.$Version.nupkg"
if (-not (Test-Path -LiteralPath $packagePath -PathType Leaf)) {
    throw "Expected package was not created: $packagePath"
}

Add-Type -AssemblyName System.IO.Compression.FileSystem
$archive = [System.IO.Compression.ZipFile]::OpenRead($packagePath)
try {
    $readmeEntry = $archive.GetEntry('README.md')
    if ($null -eq $readmeEntry) {
        throw 'The NuGet package does not contain README.md.'
    }

    $reader = New-Object System.IO.StreamReader($readmeEntry.Open())
    try {
        $packagedReadme = $reader.ReadToEnd()
    }
    finally {
        $reader.Dispose()
    }
}
finally {
    $archive.Dispose()
}

$ownershipMarker = '<!-- mcp-name: io.github.AxaFrance/webengine-mcp -->'
if (-not $packagedReadme.Contains($ownershipMarker)) {
    throw "The packaged README does not contain the MCP Registry ownership marker: $ownershipMarker"
}

Write-Output "Prepared package: $packagePath"
Write-Output 'No package or registry publication was performed.'
