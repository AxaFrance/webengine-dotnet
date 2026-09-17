[CmdletBinding()]
param(
    [switch]$RunPublisherValidation
)

$ErrorActionPreference = 'Stop'
$script:ValidationErrors = @()
$repoRoot = Split-Path -Parent $PSScriptRoot

function Add-ValidationError {
    param([Parameter(Mandatory = $true)][string]$Message)

    $script:ValidationErrors += $Message
}

function Read-JsonFile {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [Parameter(Mandatory = $true)][string]$Description
    )

    if (-not (Test-Path -LiteralPath $Path -PathType Leaf)) {
        Add-ValidationError "$Description is missing: $Path"
        return $null
    }

    try {
        return Get-Content -LiteralPath $Path -Raw | ConvertFrom-Json
    }
    catch {
        Add-ValidationError "$Description is not valid JSON: $Path ($($_.Exception.Message))"
        return $null
    }
}

function Get-RepositoryPath {
    param([Parameter(Mandatory = $true)][string]$RelativePath)

    return Join-Path $repoRoot ($RelativePath.Replace('/', [System.IO.Path]::DirectorySeparatorChar))
}

$marketplacePath = Get-RepositoryPath '.github/plugin/marketplace.json'
$marketplace = Read-JsonFile -Path $marketplacePath -Description 'Copilot marketplace manifest'

if ($null -ne $marketplace) {
    if ([string]$marketplace.name -ne 'webengine-plugins') {
        Add-ValidationError 'Copilot marketplace name must be webengine-plugins.'
    }

    if ([string]::IsNullOrWhiteSpace([string]$marketplace.owner.name)) {
        Add-ValidationError 'Copilot marketplace owner.name is required.'
    }

    $marketplacePlugins = @($marketplace.plugins)
    if ($marketplacePlugins.Count -ne 2) {
        Add-ValidationError 'Copilot marketplace must list exactly the web and mobile plugins.'
    }

    foreach ($marketplacePlugin in $marketplacePlugins) {
        $pluginName = [string]$marketplacePlugin.name
        $source = [string]$marketplacePlugin.source

        if ([string]::IsNullOrWhiteSpace($pluginName) -or [string]::IsNullOrWhiteSpace($source)) {
            Add-ValidationError 'Every Copilot marketplace plugin requires name and source.'
            continue
        }

        $pluginRoot = Get-RepositoryPath $source
        if (-not (Test-Path -LiteralPath $pluginRoot -PathType Container)) {
            Add-ValidationError "Marketplace source for $pluginName does not exist: $source"
            continue
        }

        $pluginManifestPath = Join-Path $pluginRoot 'plugin.json'
        $pluginManifest = Read-JsonFile -Path $pluginManifestPath -Description "$pluginName plugin manifest"
        if ($null -ne $pluginManifest) {
            if ([string]$pluginManifest.name -ne $pluginName) {
                Add-ValidationError "$pluginName plugin.json name does not match its marketplace entry."
            }

            if ([string]::IsNullOrWhiteSpace([string]$pluginManifest.version)) {
                Add-ValidationError "$pluginName plugin.json must declare a version."
            }
        }

        $mcpPath = Join-Path $pluginRoot 'mcp.json'
        $mcp = Read-JsonFile -Path $mcpPath -Description "$pluginName Agent Plugins MCP manifest"
        if ($null -eq $mcp) {
            continue
        }

        if ([string]$mcp.'$schema' -ne 'https://agent-plugins.org/schemas/1.0.0/mcp.schema.json') {
            Add-ValidationError "$pluginName mcp.json must use the Agent Plugins 1.0 MCP schema."
        }

        if ($mcp.PSObject.Properties.Name -contains 'servers') {
            Add-ValidationError "$pluginName mcp.json must not contain the unsupported servers property."
        }

        if ($mcp.PSObject.Properties.Name -contains '$comment') {
            Add-ValidationError "$pluginName mcp.json must not contain the unsupported top-level `$comment property."
        }

        $servers = @()
        if ($null -ne $mcp.mcpServers) {
            $servers = @($mcp.mcpServers.PSObject.Properties)
        }

        if ($servers.Count -ne 1) {
            Add-ValidationError "$pluginName mcp.json must declare exactly one MCP server."
            continue
        }

        $server = $servers[0].Value
        if ([string]$server.type -ne 'stdio') {
            Add-ValidationError "$pluginName MCP server transport must be stdio."
        }

        if ([string]$server.command -ne 'dnx') {
            Add-ValidationError "$pluginName MCP server command must be dnx."
        }

        $serverArgs = @($server.args | ForEach-Object { [string]$_ })
        if ($serverArgs -notcontains 'AxaFrance.WebEngine.Mcp') {
            Add-ValidationError "$pluginName MCP server must execute AxaFrance.WebEngine.Mcp."
        }

        $expectedProfile = if ($pluginName -eq 'webengine-mobile') { 'mobile' } else { 'web' }
        if ($serverArgs -notcontains '--profile' -or $serverArgs -notcontains $expectedProfile) {
            Add-ValidationError "$pluginName MCP server must select the $expectedProfile profile."
        }

        if ($serverArgs -notcontains '--transport' -or $serverArgs -notcontains 'stdio') {
            Add-ValidationError "$pluginName MCP server must explicitly select stdio transport."
        }
    }
}

$serverPath = Get-RepositoryPath 'server.json'
$serverMetadata = Read-JsonFile -Path $serverPath -Description 'MCP Registry server manifest'
$expectedRegistryName = 'io.github.AxaFrance/webengine-mcp'

if ($null -ne $serverMetadata) {
    if ([string]$serverMetadata.name -ne $expectedRegistryName) {
        Add-ValidationError "MCP Registry server name must be $expectedRegistryName."
    }

    if ([string]::IsNullOrWhiteSpace([string]$serverMetadata.version)) {
        Add-ValidationError 'MCP Registry server version is required.'
    }

    $packages = @($serverMetadata.packages)
    if ($packages.Count -ne 1) {
        Add-ValidationError 'MCP Registry server manifest must declare exactly one package.'
    }
    else {
        $package = $packages[0]
        if ([string]$package.registryType -ne 'nuget') {
            Add-ValidationError 'MCP Registry package registryType must be nuget.'
        }

        if ([string]$package.registryBaseUrl -ne 'https://api.nuget.org/v3/index.json') {
            Add-ValidationError 'MCP Registry package must use the official NuGet registry.'
        }

        if ([string]$package.identifier -ne 'AxaFrance.WebEngine.Mcp') {
            Add-ValidationError 'MCP Registry package identifier must be AxaFrance.WebEngine.Mcp.'
        }

        if ([string]$package.runtimeHint -ne 'dnx') {
            Add-ValidationError 'MCP Registry package runtimeHint must be dnx.'
        }

        if ([string]$package.transport.type -ne 'stdio') {
            Add-ValidationError 'MCP Registry package transport must be stdio.'
        }

        $packageArguments = @($package.packageArguments)
        $profileArgument = $packageArguments | Where-Object { [string]$_.name -eq '--profile' } | Select-Object -First 1
        $transportArgument = $packageArguments | Where-Object { [string]$_.name -eq '--transport' } | Select-Object -First 1

        if ($null -eq $profileArgument -or [string]$profileArgument.value -ne 'both') {
            Add-ValidationError 'MCP Registry package must select the both profile.'
        }

        if ($null -eq $transportArgument -or [string]$transportArgument.value -ne 'stdio') {
            Add-ValidationError 'MCP Registry package must explicitly select stdio transport.'
        }
    }
}

$projectPath = Get-RepositoryPath 'src/AxaFrance.WebEngine.Mcp/AxaFrance.WebEngine.Mcp.csproj'
if (-not (Test-Path -LiteralPath $projectPath -PathType Leaf)) {
    Add-ValidationError "MCP project file is missing: $projectPath"
}
else {
    try {
        [xml]$project = Get-Content -LiteralPath $projectPath -Raw
        $propertyGroup = @($project.Project.PropertyGroup | Where-Object { $null -ne $_.Version }) | Select-Object -First 1
        $projectVersion = [string]$propertyGroup.Version
        $packageId = [string]$propertyGroup.PackageId

        if ($packageId -ne 'AxaFrance.WebEngine.Mcp') {
            Add-ValidationError 'MCP project PackageId must be AxaFrance.WebEngine.Mcp.'
        }

        if ($null -ne $serverMetadata -and [string]$serverMetadata.version -ne $projectVersion) {
            Add-ValidationError 'server.json version must match the MCP project Version.'
        }

        if ($null -ne $serverMetadata -and @($serverMetadata.packages).Count -eq 1 -and
            [string]$serverMetadata.packages[0].version -ne $projectVersion) {
            Add-ValidationError 'server.json package version must match the MCP project Version.'
        }

        if ($null -ne $marketplace) {
            foreach ($marketplacePlugin in @($marketplace.plugins)) {
                $pluginPath = Get-RepositoryPath ([string]$marketplacePlugin.source)
                $manifestPath = Join-Path $pluginPath 'plugin.json'
                $manifest = Read-JsonFile -Path $manifestPath -Description "$($marketplacePlugin.name) plugin manifest"
                if ($null -ne $manifest -and [string]$manifest.version -ne [string]$marketplacePlugin.version) {
                    Add-ValidationError "$($marketplacePlugin.name) marketplace version must match plugin.json."
                }
            }
        }
    }
    catch {
        Add-ValidationError "MCP project file is not valid XML: $($_.Exception.Message)"
    }
}

$packageReadmePath = Get-RepositoryPath 'src/AxaFrance.WebEngine.Mcp/README.md'
if (-not (Test-Path -LiteralPath $packageReadmePath -PathType Leaf)) {
    Add-ValidationError "Packaged MCP README is missing: $packageReadmePath"
}
elseif ($null -ne $serverMetadata) {
    $packageReadme = Get-Content -LiteralPath $packageReadmePath -Raw
    $ownershipMarker = "<!-- mcp-name: $([string]$serverMetadata.name) -->"
    if (-not $packageReadme.Contains($ownershipMarker)) {
        Add-ValidationError "Packaged MCP README must contain the ownership marker: $ownershipMarker"
    }
}

if ($RunPublisherValidation) {
    $publisher = Get-Command mcp-publisher -ErrorAction SilentlyContinue
    if ($null -eq $publisher) {
        Add-ValidationError 'mcp-publisher was not found; install it or omit -RunPublisherValidation.'
    }
    else {
        Push-Location $repoRoot
        try {
            & mcp-publisher validate (Join-Path $repoRoot 'server.json')
            if ($LASTEXITCODE -ne 0) {
                Add-ValidationError "mcp-publisher validate failed with exit code $LASTEXITCODE."
            }
        }
        finally {
            Pop-Location
        }
    }
}

if ($script:ValidationErrors.Count -gt 0) {
    foreach ($validationError in $script:ValidationErrors) {
        Write-Error $validationError
    }

    exit 1
}

Write-Output 'MCP metadata validation passed.'
