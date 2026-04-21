<#
.SYNOPSIS
    OfficeAutomator Main PowerShell Script - Complete Office Installation Orchestrator

.DESCRIPTION
    Main entry point for OfficeAutomator OPTION B implementation.
    Orchestrates the complete Office installation workflow including:
    - Phase 1: Prerequisites validation
    - Phase 2: User selection menus (version, language, apps)
    - Phase 3: Configuration generation
    - Phase 4: Installation execution
    - Phase 5: Error handling and rollback
    
    This script coordinates all 6 helper functions (Scripts 1-6) and the
    OfficeAutomator.Core.dll C# library to provide a complete installation solution.

.PARAMETER Version
    (Optional) Pre-select Office version to skip menu:
    - 'PerpetualVL2024'
    - 'Subscription2024'
    - 'PerpetualVL2021'
    
.PARAMETER Language
    (Optional) Pre-select language code to skip menu:
    - 'es-ES', 'es-MX', 'en-US', 'fr-FR', etc.

.PARAMETER LogPath
    (Optional) Path to log file
    Default: $env:TEMP\OfficeAutomator.log

.EXAMPLE
    PS> .\OfficeAutomator.PowerShell.Script.ps1
    # Interactive: Shows all menus

.EXAMPLE
    PS> .\OfficeAutomator.PowerShell.Script.ps1 -Version PerpetualVL2024 -Language es-MX
    # Non-interactive: Uses provided parameters

.NOTES
    Author: Claude (AI Assistant)
    Date: 2026-04-21
    Version: 1.0
    
    REQUIREMENTS:
      - PowerShell 5.1+
      - Administrator privileges
      - .NET 8.0+ runtime
      - OfficeAutomator.Core.dll (loaded by Script 1)
    
    PHASES:
      Phase 1: Load Core DLL (Script 1)
      Phase 2: Validate prerequisites (Script 2)
      Phase 3: Initialize logging (Script 3)
      Phase 4: Display menus for user selections (Script 4)
      Phase 5: Orchestrate installation (Script 5)
      Phase 6: Handle rollback on failure (Script 6)
    
    HELPER SCRIPTS (dot-sourced):
      - OfficeAutomator.CoreDll.Loader.ps1
      - OfficeAutomator.Validation.Environment.ps1
      - OfficeAutomator.Logging.Handler.ps1
      - OfficeAutomator.Menu.Display.ps1
      - OfficeAutomator.Execution.Orchestration.ps1
      - OfficeAutomator.Execution.RollbackHandler.ps1
    
    ERROR CODES: See PHASE4-EXPANSION §10

.LINK
    Phase 4 Design: §8.7 "Script 7: PowerShell.Script MAIN"
    All UCs: UC-001 through UC-005
#>

param(
    [Parameter(Mandatory = $false)]
    [ValidateSet('PerpetualVL2024', 'Subscription2024', 'PerpetualVL2021')]
    [string]$Version,
    
    [Parameter(Mandatory = $false)]
    [string]$Language,
    
    [Parameter(Mandatory = $false)]
    [string]$LogPath = "$env:TEMP\OfficeAutomator.log"
)

#Requires -RunAsAdministrator
#Requires -Version 5.1

# ═════════════════════════════════════════════════════════════════════════════
# PHASE 1: INITIALIZATION
# ═════════════════════════════════════════════════════════════════════════════

# Get script root directory
$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path

# Dot-source helper functions
$helperScripts = @(
    'OfficeAutomator.CoreDll.Loader.ps1',
    'OfficeAutomator.Validation.Environment.ps1',
    'OfficeAutomator.Logging.Handler.ps1',
    'OfficeAutomator.Menu.Display.ps1',
    'OfficeAutomator.Execution.Orchestration.ps1',
    'OfficeAutomator.Execution.RollbackHandler.ps1'
)

Write-Host "OfficeAutomator - Office Installation Orchestrator" -ForegroundColor Cyan
Write-Host "=================================================" -ForegroundColor Cyan
Write-Host ""

# Load each helper script
foreach ($script in $helperScripts) {
    $scriptPath = Join-Path $scriptRoot "functions\$script"
    if (Test-Path $scriptPath) {
        . $scriptPath
        Write-Host "✓ Loaded: $script" -ForegroundColor Green
    }
    else {
        Write-Host "✗ ERROR: Cannot find $script" -ForegroundColor Red
        exit 1
    }
}

Write-Host ""

# ═════════════════════════════════════════════════════════════════════════════
# PHASE 2: LOAD CORE DLL
# ═════════════════════════════════════════════════════════════════════════════

Write-Host "Phase 1: Loading Core DLL..." -ForegroundColor Cyan
$dllPath = Join-Path $scriptRoot "..\src\OfficeAutomator.Core\bin\Release\net8.0\OfficeAutomator.Core.dll"

if (-not (Load-OfficeAutomatorCoreDll -DllPath $dllPath)) {
    Write-Host "✗ Failed to load Core DLL" -ForegroundColor Red
    exit 2001
}

Write-Host "✓ Core DLL loaded successfully" -ForegroundColor Green
Write-Host ""

# ═════════════════════════════════════════════════════════════════════════════
# PHASE 3: VALIDATE PREREQUISITES
# ═════════════════════════════════════════════════════════════════════════════

Write-Host "Phase 2: Validating prerequisites..." -ForegroundColor Cyan

if (-not (Test-PrerequisitesMet -DllPath $dllPath)) {
    Write-Host "✗ Prerequisites validation failed" -ForegroundColor Red
    Write-LogEntry "Prerequisites validation failed" -Level "ERROR" -LogPath $LogPath
    exit 2001
}

Write-Host "✓ All prerequisites validated" -ForegroundColor Green
Write-Host ""

# ═════════════════════════════════════════════════════════════════════════════
# PHASE 4: USER SELECTIONS
# ═════════════════════════════════════════════════════════════════════════════

Write-Host "Phase 3: User selections..." -ForegroundColor Cyan
Write-LogEntry "Starting Office installation workflow" -Level "INFO" -LogPath $LogPath

# Version selection
if (-not $Version) {
    $versionOptions = @('PerpetualVL2024', 'Subscription2024', 'PerpetualVL2021')
    $versionIndex = Show-Menu -Title "Select Office Version" -Options $versionOptions
    $Version = $versionOptions[$versionIndex - 1]
}

Write-Host "✓ Selected version: $Version" -ForegroundColor Green

# Language selection
if (-not $Language) {
    $languageOptions = @('es-MX', 'es-ES', 'en-US', 'fr-FR', 'de-DE', 'it-IT')
    $langIndex = Show-Menu -Title "Seleccione idioma / Select language" -Options $languageOptions
    $Language = $languageOptions[$langIndex - 1]
}

Write-Host "✓ Selected language: $Language" -ForegroundColor Green

# Excluded apps selection
$appsOptions = @('Teams', 'Outlook', 'OneNote', 'Access', 'Publisher', 'Visio', 'None')
$appsIndex = Show-Menu -Title "Select apps to exclude" -Options $appsOptions
$ExcludedApps = if ($appsOptions[$appsIndex - 1] -eq 'None') { @() } else { @($appsOptions[$appsIndex - 1]) }

Write-Host "✓ Selected exclusions: $(if ($ExcludedApps.Count -eq 0) { 'None' } else { $ExcludedApps -join ', ' })" -ForegroundColor Green
Write-Host ""

# ═════════════════════════════════════════════════════════════════════════════
# PHASE 5: CONFIGURATION & INSTALLATION
# ═════════════════════════════════════════════════════════════════════════════

Write-Host "Phase 4: Installation..." -ForegroundColor Cyan
Write-Host ""

# Build configuration object
$configuration = @{
    Version = $Version
    Language = $Language
    ExcludedApps = $ExcludedApps
    InstallPath = 'C:\Program Files\Microsoft Office'
    ConfigurationPath = Join-Path $env:TEMP 'OfficeDeploymentTool.xml'
}

Write-LogEntry "Configuration prepared: Version=$Version, Language=$Language" -Level "INFO" -LogPath $LogPath

# Execute installation
if (Invoke-OfficeInstallation -Configuration $configuration -LogPath $LogPath) {
    Write-Host ""
    Write-Host "═══════════════════════════════════════════════" -ForegroundColor Green
    Write-Host "✓ Installation completed successfully!" -ForegroundColor Green
    Write-Host "═══════════════════════════════════════════════" -ForegroundColor Green
    Write-LogEntry "Installation completed successfully" -Level "SUCCESS" -LogPath $LogPath
    exit 0
}
else {
    Write-Host ""
    Write-Host "═══════════════════════════════════════════════" -ForegroundColor Red
    Write-Host "✗ Installation failed. Starting rollback..." -ForegroundColor Red
    Write-Host "═══════════════════════════════════════════════" -ForegroundColor Red
    
    # Attempt rollback
    if (Invoke-RollbackOnFailure -Configuration $configuration -LogPath $LogPath) {
        Write-Host "✓ Rollback completed successfully" -ForegroundColor Yellow
        exit 1
    }
    else {
        Write-Host "✗ Rollback failed. Manual cleanup may be required." -ForegroundColor Red
        exit 1006
    }
}
