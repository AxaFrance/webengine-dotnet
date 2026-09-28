#!/usr/bin/env pwsh
# Copyright (c) 2026 Microsoft Corporation. All rights reserved.
# SPDX-License-Identifier: MIT
#Requires -Version 7.4

<##
.SYNOPSIS
    Applies a tag-derived version to the MCP package and registry metadata.
.DESCRIPTION
    Updates the CI checkout before packaging so the NuGet package, MCP Registry
    manifest, and package metadata all use the release tag version.
.PARAMETER Version
    The NuGet package version from the mcp-v<version> release tag.
.PARAMETER RepoRoot
    The repository root containing server.json and the MCP project.
.EXAMPLE
    ./scripts/set-mcp-release-version.ps1 -Version 3.26.268.3-preview
#>

[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [ValidateNotNullOrEmpty()]
    [string]$Version,

    [Parameter(Mandatory = $false)]
    [ValidateNotNullOrEmpty()]
    [string]$RepoRoot = (Split-Path -Parent $PSScriptRoot)
)

$ErrorActionPreference = 'Stop'

#region Functions
function Set-McpProjectVersion {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string]$ProjectPath,

        [Parameter(Mandatory = $true)]
        [string]$ReleaseVersion
    )

    [xml]$Project = Get-Content -LiteralPath $ProjectPath -Raw
    $VersionProperties = @($Project.Project.PropertyGroup | Where-Object { $null -ne $_.Version })

    if ($VersionProperties.Count -ne 1) {
        throw "Expected exactly one MCP project Version property: $ProjectPath"
    }

    $VersionProperties[0].Version = $ReleaseVersion
    $XmlSettings = New-Object System.Xml.XmlWriterSettings
    $XmlSettings.Indent = $true
    $XmlSettings.Encoding = New-Object System.Text.UTF8Encoding($false)
    $XmlWriter = [System.Xml.XmlWriter]::Create($ProjectPath, $XmlSettings)

    try {
        $Project.Save($XmlWriter)
    }
    finally {
        $XmlWriter.Dispose()
    }
}

function Set-McpRegistryVersion {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [string]$MetadataPath,

        [Parameter(Mandatory = $true)]
        [string]$ReleaseVersion
    )

    $Metadata = Get-Content -LiteralPath $MetadataPath -Raw | ConvertFrom-Json
    $Packages = @($Metadata.packages)

    if ($Packages.Count -ne 1) {
        throw "Expected exactly one MCP Registry package: $MetadataPath"
    }

    if ([string]$Packages[0].identifier -ne 'AxaFrance.WebEngine.Mcp') {
        throw "Unexpected MCP Registry package identifier: $($Packages[0].identifier)"
    }

    $Metadata.version = $ReleaseVersion
    $Metadata.packages[0].version = $ReleaseVersion
    $Metadata | ConvertTo-Json -Depth 10 | Set-Content -LiteralPath $MetadataPath -Encoding utf8NoBOM
}
#endregion Functions

#region Main Execution
if ($MyInvocation.InvocationName -ne '.') {
    try {
        $ProjectPath = Join-Path $RepoRoot 'src/AxaFrance.WebEngine.Mcp/AxaFrance.WebEngine.Mcp.csproj'
        $MetadataPath = Join-Path $RepoRoot 'server.json'

        if (-not (Test-Path -LiteralPath $ProjectPath -PathType Leaf)) {
            throw "MCP project file is missing: $ProjectPath"
        }

        if (-not (Test-Path -LiteralPath $MetadataPath -PathType Leaf)) {
            throw "MCP Registry manifest is missing: $MetadataPath"
        }

        Set-McpProjectVersion -ProjectPath $ProjectPath -ReleaseVersion $Version
        Set-McpRegistryVersion -MetadataPath $MetadataPath -ReleaseVersion $Version
        Write-Output "Applied MCP release version $Version to project and registry metadata."
        exit 0
    }
    catch {
        Write-Error -ErrorAction Continue "MCP release version update failed: $($_.Exception.Message)"
        exit 1
    }
}
#endregion Main Execution