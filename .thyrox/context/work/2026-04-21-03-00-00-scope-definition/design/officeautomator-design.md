```yml
created_at: 2026-04-22 06:15:00
project: OfficeAutomator
work_package: 2026-04-21-03-00-00-scope-definition
phase: Phase 7 — DESIGN/SPECIFY
author: Claude
status: Aprobado
version: 1.0.0
document_type: Technical Design
```

# TECHNICAL DESIGN - OfficeAutomator v1.0.0

---

## Executive Summary

This document provides the technical blueprint for implementing OfficeAutomator v1.0.0. It defines architecture, module structure, function signatures, state management, error handling, and integration points. The design emphasizes reliability, transparency, and idempotence through a layered architecture and comprehensive validation framework.

**Design Pillars:**
1. **Reliability:** Fail-Fast on validation, zero silent failures
2. **Transparency:** Clear logging and user communication
3. **Idempotence:** Safe to run multiple times

---

## 1. SYSTEM ARCHITECTURE

### 1.1 Architectural Layers

```
┌─────────────────────────────────────────────────────────────┐
│  PRESENTATION LAYER (CLI)                                    │
│  • Invoke-OfficeAutomator.ps1 (Main entry point)             │
│  • Write-OfficeLog.ps1 (Logging output)                      │
│  • User prompts and confirmations                            │
└─────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────┐
│  ORCHESTRATION LAYER (UC Flow)                               │
│  • UC-001: Select-OfficeVersion.ps1                          │
│  • UC-002: Select-OfficeLanguage.ps1                         │
│  • UC-003: Exclude-OfficeApplications.ps1                    │
│  • UC-004: Validate-OfficeConfiguration.ps1 (CRITICAL)       │
│  • UC-005: Install-Office.ps1                                │
└─────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────┐
│  CONFIGURATION LAYER                                         │
│  • Configuration object ($Config): Version, Languages, Apps  │
│  • XML generation: Convert-ToConfigurationXml.ps1            │
│  • Defaults management                                       │
└─────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────┐
│  VALIDATION LAYER (8-point validation in UC-004)            │
│  • Version validation                                        │
│  • Language-app compatibility (anti-Microsoft-OCT-bug)       │
│  • XML schema validation                                     │
│  • SHA256 integrity (3x retry with backoff)                  │
└─────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────┐
│  DATA ACCESS LAYER                                           │
│  • Get-SupportedVersions.ps1                                 │
│  • Get-SupportedLanguages.ps1                                │
│  • Get-CompatibilityMatrix.ps1                               │
│  • Data files: JSON (versions, languages, compatibility)     │
└─────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────┐
│  EXECUTION LAYER (UC-005)                                    │
│  • setup.exe invocation                                      │
│  • Process monitoring                                        │
│  • Stdout/stderr capture                                     │
│  • Installation verification                                │
└─────────────────────────────────────────────────────────────┘
                              ↓
┌─────────────────────────────────────────────────────────────┐
│  UTILITY LAYER                                               │
│  • Write-OfficeLog.ps1 (Centralized logging)                 │
│  • Error handling helpers                                    │
│  • State management functions                                │
└─────────────────────────────────────────────────────────────┘
```

### 1.2 Data Flow

```
START
  ↓
Invoke-OfficeAutomator.ps1
  ↓
UC-001: Select-OfficeVersion
  │ Output: $Config.Version
  ↓
UC-002: Select-OfficeLanguage
  │ Output: $Config.Languages
  ↓
UC-003: Exclude-OfficeApplications
  │ Output: $Config.ExcludedApps
  ↓
GENERATE configuration.xml
  ├─ Input: Version, Languages, ExcludedApps
  └─ Output: $Config.ConfigPath
  ↓
UC-004: Validate-OfficeConfiguration (CRITICAL - 8 steps)
  │ Phase 1: Parallel validation (steps 1,2,5)
  │ Phase 2: Sequential validation (steps 3→4→6)
  │ Phase 3: Download + SHA256 verify (step 7, 3x retry)
  │ Phase 4: Generate config.xml (step 8)
  ├─ Input: $Config object
  ├─ Output: $Config.ValidationPassed = $true/$false
  └─ Output: $Config.ODTPath (if success)
  ↓
[Is validation passed?]
  ├─ NO → Display error + Exit (Fail-Fast)
  └─ YES → Proceed to UC-005
  ↓
UC-005: Install-Office
  ├─ Pre-check: Is Office already installed?
  │  ├─ YES, same version/language → Log "already installed" + Exit
  │  └─ NO → Proceed
  ├─ Execute: setup.exe /configure $Config.ConfigPath
  ├─ Monitor: Progress, logs, errors
  ├─ Verify: Installation success
  └─ Output: Installation result (success/failure + logs)
  ↓
Display result to user
  ↓
END
```

---

## 2. MODULE STRUCTURE

### 2.1 Directory Organization

```
OfficeAutomator/
├── Functions/
│   ├── Public/
│   │   ├── Invoke-OfficeAutomator.ps1          [Entry point, UC orchestration]
│   │   │
│   │   ├── UC-001/
│   │   │   └── Select-OfficeVersion.ps1
│   │   │
│   │   ├── UC-002/
│   │   │   └── Select-OfficeLanguage.ps1
│   │   │
│   │   ├── UC-003/
│   │   │   └── Exclude-OfficeApplications.ps1
│   │   │
│   │   ├── UC-004/
│   │   │   ├── Validate-OfficeConfiguration.ps1 [Main UC-004 orchestrator]
│   │   │   └── Test-LanguageCompatibility.ps1   [Helper for step 6]
│   │   │
│   │   └── UC-005/
│   │       └── Install-Office.ps1
│   │
│   ├── Private/
│   │   ├── Internal/
│   │   │   ├── Get-SupportedVersions.ps1        [Data access]
│   │   │   ├── Get-SupportedLanguages.ps1       [Data access]
│   │   │   ├── Get-AvailableApps.ps1            [Data access]
│   │   │   ├── Get-CompatibilityMatrix.ps1      [Data access]
│   │   │   ├── Convert-ToConfigurationXml.ps1   [Configuration generation]
│   │   │   └── Download-OfficeDeploymentTool.ps1 [ODT download]
│   │   │
│   │   └── Validation/
│   │       ├── Test-XmlSchema.ps1               [UC-004 step 2]
│   │       ├── Test-VersionValidity.ps1         [UC-004 step 1]
│   │       ├── Test-LanguageValidity.ps1        [UC-004 step 3]
│   │       ├── Test-LanguageAppCombination.ps1  [UC-004 step 5,6]
│   │       └── Test-Sha256Integrity.ps1         [UC-004 step 7 with retry]
│   │
│   └── Logging/
│       └── Write-OfficeLog.ps1                  [Centralized logging]
│
├── Data/
│   ├── supported-versions.json                  [2024, 2021, 2019]
│   ├── supported-languages.json                 [es-ES, en-US]
│   ├── language-compatibility-matrix.json       [Version×Lang×App]
│   └── odt-checksums.json                       [SHA256 per version]
│
├── Tests/
│   ├── UC-001.Tests.ps1
│   ├── UC-002.Tests.ps1
│   ├── UC-003.Tests.ps1
│   ├── UC-004.Tests.ps1
│   ├── UC-005.Tests.ps1
│   └── Integration.Tests.ps1
│
├── Docs/
│   ├── USER_GUIDE.md
│   ├── ARCHITECTURE.md
│   └── API_REFERENCE.md
│
├── OfficeAutomator.psd1                         [Module manifest]
├── OfficeAutomator.psm1                         [Module loader]
└── README.md
```

### 2.2 Module Manifest (OfficeAutomator.psd1)

```powershell
@{
    RootModule            = 'OfficeAutomator.psm1'
    ModuleVersion         = '1.0.0'
    GUID                  = '{GUID-HERE}'
    Author                = 'OfficeAutomator Team'
    CompanyName           = 'Organization'
    Description           = 'Reliable Office LTSC automation for v2024, 2021, 2019'
    PowerShellVersion     = '5.1'
    
    FunctionsToExport     = @(
        'Invoke-OfficeAutomator',
        'Select-OfficeVersion',
        'Select-OfficeLanguage',
        'Exclude-OfficeApplications',
        'Validate-OfficeConfiguration',
        'Install-Office'
    )
    
    PrivateData = @{
        PSData = @{
            Tags       = @('Office', 'LTSC', 'Installation', 'Automation')
            LicenseUri = 'https://...'
            ProjectUri = 'https://...'
            ReleaseNotes = 'Initial v1.0.0 release'
        }
    }
}
```

---

## 3. STATE MANAGEMENT

### 3.1 Configuration Object ($Config)

**Purpose:** Single source of truth for all installation configuration throughout the workflow.

**Data Structure:**

```powershell
$Config = @{
    # User selections (UC-001, UC-002, UC-003)
    Version           = "2024"                    # UC-001 output
    Languages         = @("es-ES", "en-US")      # UC-002 output
    ExcludedApps      = @("Teams", "OneDrive")   # UC-003 output
    
    # Configuration files (UC-004)
    ConfigPath        = "C:\Temp\configuration.xml"  # Generated by UC-004
    ODTPath           = "C:\Temp\setup.exe"          # Downloaded by UC-004
    
    # Validation status (UC-004)
    ValidationPassed  = $true                    # Set by UC-004
    ValidationLog     = @(...)                   # Array of step results
    
    # Installation result (UC-005)
    InstallationPassed = $true                   # Set by UC-005
    InstallationLog    = @(...)                  # Array of install events
    
    # Metadata
    SessionId         = "{GUID}"                 # For log correlation
    StartTime         = (Get-Date)               # Session start
    LogFilePath       = "$env:TEMP\OfficeAutomator-{timestamp}.log"
}
```

### 3.2 State Transitions

**Valid State Progression:**

```
INITIAL
  ↓ (UC-001 completes)
Version = "2024"
  ↓ (UC-002 completes)
Languages = @("es-ES")
  ↓ (UC-003 completes)
ExcludedApps = @("Teams")
  ↓ (UC-004 phase 1-2)
ConfigPath = "config.xml", ValidationPassed = ???
  ↓ (UC-004 phase 3)
ODTPath = "setup.exe", ValidationPassed = ???
  ↓ (UC-004 phase 4)
ValidationPassed = $true   [SUCCESS]
  ↓ (UC-005)
InstallationPassed = $true [SUCCESS]
  ↓
COMPLETE

OR

After any UC error:
ValidationPassed = $false [FAILURE]
  ↓
RESTART from UC-001
```

### 3.3 State Persistence Across Sessions

**Session State Storage:**
- Configuration object: RAM during execution
- Installation logs: `$env:TEMP\OfficeAutomator-{timestamp}.log`
- Previous installations: Windows registry (Office install info)

**Idempotence Check (UC-005):**
```powershell
# Check if Office already installed
$installedVersion = Get-OfficeInstallationVersion
if ($installedVersion) {
    if ($installedVersion -eq $Config.Version -and
        $installedLanguages -eq $Config.Languages) {
        Log "Office {0} already installed with correct languages"
        return Success  # Idempotent: don't reinstall
    } else {
        # Mismatch: needs repair or update
        Log "Version/language mismatch detected"
        # Handle update scenario
    }
}
```

---

## 4. FUNCTION SPECIFICATIONS

### 4.1 Main Entry Point: Invoke-OfficeAutomator

**Purpose:** Orchestrate complete installation workflow

**Signature:**
```powershell
function Invoke-OfficeAutomator {
    [CmdletBinding()]
    param()
    
    # Initialize
    $Config = @{
        SessionId      = (New-Guid)
        StartTime      = Get-Date
        LogFilePath    = "$env:TEMP\OfficeAutomator-$(Get-Date -Format 'yyyyMMdd-HHmmss').log"
    }
    
    try {
        Log-Info "OfficeAutomator v1.0.0 started"
        
        # UC-001
        $Config.Version = Select-OfficeVersion
        Log-Info "Version selected: $($Config.Version)"
        
        # UC-002
        $Config.Languages = Select-OfficeLanguage -Version $Config.Version
        Log-Info "Languages selected: $($Config.Languages -join ', ')"
        
        # UC-003
        $Config.ExcludedApps = Exclude-OfficeApplications -Version $Config.Version
        Log-Info "Applications to exclude: $($Config.ExcludedApps -join ', ')"
        
        # UC-004
        $validationResult = Validate-OfficeConfiguration -Config $Config
        if (-not $validationResult.Success) {
            Log-Error "Configuration validation failed: $($validationResult.Error)"
            throw $validationResult.Error
        }
        Log-Info "Configuration validation passed"
        $Config.ValidationPassed = $true
        
        # UC-005
        $installResult = Install-Office -Config $Config
        if (-not $installResult.Success) {
            Log-Error "Installation failed: $($installResult.Error)"
            throw $installResult.Error
        }
        Log-Info "Installation completed successfully"
        
        Log-Success "OfficeAutomator completed successfully"
        Write-Host "Installation completed. Check logs at: $($Config.LogFilePath)"
        
    } catch {
        Log-Error "Fatal error: $_"
        Write-Host "Installation failed. Check logs at: $($Config.LogFilePath)"
        throw
    }
}
```

**Return:** None (logs and writes to host)

---

### 4.2 UC-001: Select-OfficeVersion

**Signature:**
```powershell
function Select-OfficeVersion {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $false)]
        [ValidateSet('2024', '2021', '2019')]
        [string]$Version
    )
    
    if ($PSBoundParameters.ContainsKey('Version')) {
        # Non-interactive mode (for scripting)
        return $Version
    }
    
    # Interactive mode
    $supportedVersions = Get-SupportedVersions
    
    Write-Host "`nSelect Office LTSC Version:`n"
    foreach ($v in $supportedVersions) {
        Write-Host "[$($v.Number)] Office $($v.Version) LTSC (Support until $($v.SupportEnd))"
    }
    
    do {
        $choice = Read-Host "Enter selection (1-3)"
        if ($choice -in @('1', '2', '3')) {
            return $supportedVersions[$choice - 1].Version
        }
        Write-Host "Invalid selection. Please enter 1, 2, or 3."
    } while ($true)
}
```

**Parameters:**
- `$Version` [string] - Optional: directly specify version (non-interactive)

**Returns:** [string] - One of: "2024", "2021", "2019"

**Error Handling:** Validates against supported list, prompts for retry if invalid

---

### 4.3 UC-002: Select-OfficeLanguage

**Signature:**
```powershell
function Select-OfficeLanguage {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [ValidateSet('2024', '2021', '2019')]
        [string]$Version,
        
        [Parameter(Mandatory = $false)]
        [string[]]$Languages
    )
    
    if ($PSBoundParameters.ContainsKey('Languages')) {
        # Non-interactive mode
        foreach ($lang in $Languages) {
            if ($lang -notin @('es-ES', 'en-US')) {
                throw "Language $lang not supported in v1.0.0"
            }
        }
        return $Languages
    }
    
    # Interactive mode
    $supportedLangs = Get-SupportedLanguages -Version $Version
    $approvedLangs = @('es-ES', 'en-US')  # v1.0.0 constraint
    
    Write-Host "`nSelect Installation Language(s):`n"
    foreach ($i, $lang in $approvedLangs | ForEach-Object { $_ } | Select-Object -IndexOnly) {
        Write-Host "[$(($i + 1))] $($lang)"
    }
    Write-Host "[12] Both (es-ES and en-US)"
    
    do {
        $choice = Read-Host "Enter selection (1, 2, or 12)"
        $selected = @()
        if ($choice -eq '1') { $selected = @('es-ES') }
        elseif ($choice -eq '2') { $selected = @('en-US') }
        elseif ($choice -eq '12') { $selected = @('es-ES', 'en-US') }
        else { Write-Host "Invalid selection"; continue }
        
        return $selected
    } while ($true)
}
```

**Parameters:**
- `$Version` [string] - Office version (from UC-001)
- `$Languages` [string[]] - Optional: directly specify languages

**Returns:** [string[]] - Array of language codes

---

### 4.4 UC-003: Exclude-OfficeApplications

**Signature:**
```powershell
function Exclude-OfficeApplications {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [ValidateSet('2024', '2021', '2019')]
        [string]$Version,
        
        [Parameter(Mandatory = $false)]
        [string[]]$ExcludedApps
    )
    
    if ($PSBoundParameters.ContainsKey('ExcludedApps')) {
        # Non-interactive mode
        $available = Get-AvailableApps -Version $Version
        foreach ($app in $ExcludedApps) {
            if ($app -notin $available) {
                throw "App $app not available in Office $Version"
            }
        }
        return $ExcludedApps
    }
    
    # Interactive mode
    $availableApps = Get-AvailableApps -Version $Version
    $defaults = Get-DefaultExclusions -Version $Version
    
    Write-Host "`nSelect Applications to Exclude:`n"
    foreach ($i, $app in $availableApps | Select-Object -IndexOnly) {
        $isDefault = if ($app -in $defaults) { " [DEFAULT]" } else { "" }
        Write-Host "[$(($i + 1))] $app$isDefault"
    }
    Write-Host "`nEnter comma-separated numbers (or press Enter for defaults)"
    
    $input = Read-Host "Selection"
    if ([string]::IsNullOrWhiteSpace($input)) {
        return $defaults  # Use defaults
    }
    
    # Parse input and validate
    $indices = $input -split ',' | ForEach-Object { [int]$_.Trim() - 1 }
    $selected = $availableApps[$indices]
    
    return $selected
}
```

**Parameters:**
- `$Version` [string] - Office version (from UC-001)
- `$ExcludedApps` [string[]] - Optional: directly specify exclusions

**Returns:** [string[]] - Array of application names to exclude

---

### 4.5 UC-004: Validate-OfficeConfiguration (CRITICAL)

**Signature:**
```powershell
function Validate-OfficeConfiguration {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [PSObject]$Config
    )
    
    $validationLog = @()
    
    try {
        # PHASE 1: Parallel Validation
        Write-Host "`nValidating configuration (Phase 1)..."
        
        # Step 1: Version
        if ($Config.Version -notin @('2024', '2021', '2019')) {
            Log-Error "[BLOQUEADOR] Version not supported: $($Config.Version)"
            return @{ Success = $false; Error = "Unsupported version" }
        }
        Log-Info "Step 1: Version valid ($($Config.Version))"
        $validationLog += @{ Step = 1; Status = "OK" }
        
        # Step 2: XML Schema
        $xmlValid = Test-XmlSchema -ConfigPath (Get-SampleConfigXml)
        if (-not $xmlValid) {
            Log-Error "[BLOQUEADOR] XML schema validation failed"
            return @{ Success = $false; Error = "Invalid XML schema" }
        }
        Log-Info "Step 2: XML schema valid"
        $validationLog += @{ Step = 2; Status = "OK" }
        
        # Step 5: App-Version Compatibility
        $availableApps = Get-AvailableApps -Version $Config.Version
        foreach ($app in $Config.ExcludedApps) {
            if ($app -notin $availableApps) {
                Log-Error "[BLOQUEADOR] App not available: $app in v$($Config.Version)"
                return @{ Success = $false; Error = "App $app not available" }
            }
        }
        Log-Info "Step 5: App-version compatibility valid"
        $validationLog += @{ Step = 5; Status = "OK" }
        
        # PHASE 2: Sequential Validation
        Write-Host "Validating configuration (Phase 2)..."
        
        # Step 3: Language exists
        $supportedLangs = Get-SupportedLanguages -Version $Config.Version
        foreach ($lang in $Config.Languages) {
            if ($lang -notin $supportedLangs) {
                Log-Error "[BLOQUEADOR] Language not available: $lang in v$($Config.Version)"
                return @{ Success = $false; Error = "Language not available" }
            }
        }
        Log-Info "Step 3: Language exists in version"
        $validationLog += @{ Step = 3; Status = "OK" }
        
        # Step 4: Language in v1.0.0 approved
        $approvedLangs = @('es-ES', 'en-US')
        foreach ($lang in $Config.Languages) {
            if ($lang -notin $approvedLangs) {
                Log-Error "[BLOQUEADOR] Language not in v1.0.0: $lang"
                return @{ Success = $false; Error = "Language not approved for v1.0.0" }
            }
        }
        Log-Info "Step 4: Language in v1.0.0 approved list"
        $validationLog += @{ Step = 4; Status = "OK" }
        
        # Step 6: Anti-Microsoft-OCT-bug
        $compatMatrix = Get-LanguageCompatibilityMatrix
        foreach ($lang in $Config.Languages) {
            foreach ($app in $Config.ExcludedApps) {
                $key = "{0}_{1}" -f $lang, $app
                if ($compatMatrix.ContainsKey($key) -and -not $compatMatrix[$key].IsCompatible) {
                    Log-Error "[BLOQUEADOR] Language-App incompatible: $lang + $app"
                    return @{ Success = $false; Error = "Incompatible language-app combination" }
                }
            }
        }
        Log-Info "Step 6: Anti-Microsoft-OCT-bug validation passed"
        $validationLog += @{ Step = 6; Status = "OK" }
        
        # PHASE 3: Download & Verify
        Write-Host "Downloading Office Deployment Tool (Phase 3)..."
        
        # Step 7: SHA256 Verify with Retry
        $maxRetries = 3
        $attempt = 0
        $sha256Valid = $false
        
        while ($attempt -lt $maxRetries) {
            $attempt++
            try {
                $odt = Download-OfficeDeploymentTool -Version $Config.Version
                $actualSha = (Get-FileHash $odt -Algorithm SHA256).Hash
                $expectedSha = Get-OfficialODTSHA256 -Version $Config.Version
                
                if ($actualSha -eq $expectedSha) {
                    $sha256Valid = $true
                    break
                }
            } catch {
                Log-Warning "Attempt $attempt failed: $_"
            }
            
            if ($attempt -lt $maxRetries) {
                $backoff = 2 * $attempt  # 2s, 4s, 6s
                Start-Sleep -Seconds $backoff
            }
        }
        
        if (-not $sha256Valid) {
            Log-Error "[CRITICO] SHA256 verification failed after $maxRetries attempts"
            return @{ Success = $false; Error = "ODT verification failed" }
        }
        Log-Info "Step 7: SHA256 verified (attempt $attempt)"
        $validationLog += @{ Step = 7; Status = "OK"; Attempts = $attempt }
        
        # PHASE 4: Generate config.xml
        Write-Host "Generating configuration.xml (Phase 4)..."
        
        # Step 8: Generate and validate
        $configXml = Convert-ToConfigurationXml -Config $Config
        if (-not (Test-Path $configXml)) {
            Log-Error "[BLOQUEADOR] Failed to generate configuration.xml"
            return @{ Success = $false; Error = "Configuration generation failed" }
        }
        
        $Config.ConfigPath = $configXml
        $Config.ODTPath = $odt
        Log-Info "Step 8: Configuration XML generated and validated"
        $validationLog += @{ Step = 8; Status = "OK" }
        
        Log-Success "All validations passed"
        return @{ 
            Success = $true
            ValidationLog = $validationLog
            ConfigPath = $configXml
            ODTPath = $odt
        }
        
    } catch {
        Log-Error "Validation exception: $_"
        return @{ Success = $false; Error = $_.Message }
    }
}
```

**Parameters:**
- `$Config` [PSObject] - Configuration object from UC-001/002/003

**Returns:** [PSObject] with properties:
- `Success` [bool] - Validation result
- `Error` [string] - Error message if failed
- `ConfigPath` [string] - Path to config.xml if success
- `ODTPath` [string] - Path to setup.exe if success
- `ValidationLog` [array] - Log of each step

**Error Categories:**
- [BLOQUEADOR]: Steps 1,2,3,4,5,6,8 - Fail immediately
- [CRITICO]: Step 7 - Auto-retry 3x with exponential backoff

---

### 4.6 UC-005: Install-Office

**Signature:**
```powershell
function Install-Office {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [PSObject]$Config
    )
    
    $installLog = @()
    
    try {
        # Pre-installation check
        Write-Host "`nChecking for existing Office installation..."
        $installedVersion = Get-OfficeInstallationVersion
        
        if ($installedVersion) {
            Log-Info "Office $installedVersion detected"
            if ($installedVersion -eq $Config.Version) {
                Log-Info "Version matches. Verification needed."
                # Verify languages match
                $installedLanguages = Get-OfficeInstalledLanguages
                if (Compare-Languages $installedLanguages $Config.Languages) {
                    Log-Success "Office $installedVersion with correct languages already installed"
                    Log-Info "Skipping reinstallation (idempotent)"
                    return @{ Success = $true; Message = "Already installed correctly" }
                }
            }
            Log-Warning "Installed version/language mismatch. Proceeding with update..."
        }
        
        # Execute installation
        Write-Host "`nStarting Office installation..."
        Log-Info "Executing: setup.exe /configure $($Config.ConfigPath)"
        
        $process = Start-Process -FilePath $Config.ODTPath `
            -ArgumentList "/configure `"$($Config.ConfigPath)`"" `
            -PassThru `
            -NoNewWindow
        
        # Monitor progress
        $timeout = 30 * 60  # 30 minutes
        $elapsed = 0
        $interval = 5       # Check every 5 seconds
        
        while (-not $process.HasExited -and $elapsed -lt $timeout) {
            Start-Sleep -Seconds $interval
            $elapsed += $interval
            Log-Info "Installation progress: $([math]::Round($elapsed / $timeout * 100, 1))%"
        }
        
        if (-not $process.HasExited) {
            Log-Error "Installation timeout after $timeout seconds"
            $process.Kill()
            return @{ Success = $false; Error = "Installation timeout" }
        }
        
        # Check exit code
        $exitCode = $process.ExitCode
        Log-Info "setup.exe exited with code: $exitCode"
        $installLog += @{ ExitCode = $exitCode; Message = "setup.exe completed" }
        
        if ($exitCode -ne 0) {
            Log-Error "Installation failed with exit code $exitCode"
            # Capture setup logs from %TEMP%
            $setupLogs = Get-SetupLogs
            Log-Info "Setup logs: $setupLogs"
            return @{ Success = $false; Error = "Setup failed (exit code $exitCode)" }
        }
        
        # Post-installation verification
        Write-Host "`nVerifying installation..."
        $verifiedVersion = Get-OfficeInstallationVersion
        $verifiedLanguages = Get-OfficeInstalledLanguages
        
        Log-Info "Installed version: $verifiedVersion"
        Log-Info "Installed languages: $($verifiedLanguages -join ', ')"
        
        if ($verifiedVersion -ne $Config.Version) {
            Log-Error "Version mismatch: expected $($Config.Version), got $verifiedVersion"
            return @{ Success = $false; Error = "Version verification failed" }
        }
        
        if (-not (Compare-Languages $verifiedLanguages $Config.Languages)) {
            Log-Error "Language mismatch: expected $($Config.Languages -join ', '), got $($verifiedLanguages -join ', ')"
            return @{ Success = $false; Error = "Language verification failed" }
        }
        
        Log-Success "Office installation verified successfully"
        return @{ 
            Success = $true
            Message = "Installation completed successfully"
            InstalledVersion = $verifiedVersion
            InstalledLanguages = $verifiedLanguages
            InstallLog = $installLog
        }
        
    } catch {
        Log-Error "Installation exception: $_"
        return @{ Success = $false; Error = $_.Message }
    }
}
```

**Parameters:**
- `$Config` [PSObject] - Configuration object with ConfigPath and ODTPath

**Returns:** [PSObject] with properties:
- `Success` [bool] - Installation result
- `Message` [string] - Success message
- `Error` [string] - Error message if failed
- `InstalledVersion` [string] - Installed Office version
- `InstalledLanguages` [string[]] - Installed language codes
- `InstallLog` [array] - Installation events log

**Idempotence:** Detects existing installation and skips reinstall if correct

---

## 5. ERROR HANDLING STRATEGY

### 5.1 Error Categories

| Category | Steps | Action | Recovery |
|----------|-------|--------|----------|
| **[BLOQUEADOR]** | 1,2,3,4,5,6,8 | Fail-Fast | User must retry from UC-001 |
| **[CRITICO]** | 7 | Auto-retry 3x | If all fail, then Fail-Fast |
| **[RECUPERABLE]** | — | Log warning | Continue if safe |

### 5.2 Retry Strategy (Step 7 only)

**Condition:** SHA256 mismatch (transient network error)

**Retry Logic:**
```
Attempt 1: Download + SHA256 check
  ├─ Success → Continue
  └─ Fail → Wait 2s, retry
Attempt 2: Download + SHA256 check
  ├─ Success → Continue
  └─ Fail → Wait 4s, retry
Attempt 3: Download + SHA256 check
  ├─ Success → Continue
  └─ Fail → [CRITICO] Error, stop UC-004
```

**Backoff:** Exponential (2s, 4s, 6s)

### 5.3 Fail-Fast Principle

**Rule:** On any [BLOQUEADOR] error:
1. Log error with context
2. Display error message to user
3. Stop UC workflow
4. Do NOT proceed to UC-005
5. Do NOT execute setup.exe

**Example:**
```
UC-004 Step 3 fails: "Language en-GB not available in Office 2024"
  → Log error
  → Display: "[ERROR] Language not supported for this version"
  → Fail-Fast (do not retry, do not continue)
  → User must restart from UC-001 with different language
```

---

## 6. LOGGING SPECIFICATION

### 6.1 Logging Standards

**Format:** `[TIMESTAMP] [LEVEL] [COMPONENT] Message`

**Example:**
```
2026-04-22 06:30:15.234 [INFO]    [UC-001]        Version selected: 2024
2026-04-22 06:30:20.567 [INFO]    [UC-002]        Languages selected: es-ES, en-US
2026-04-22 06:30:25.123 [INFO]    [UC-003]        Applications to exclude: Teams, OneDrive
2026-04-22 06:30:30.456 [INFO]    [UC-004-STEP1]  Validating version...
2026-04-22 06:30:30.789 [SUCCESS] [UC-004-STEP1]  Version valid (2024)
2026-04-22 06:30:35.123 [INFO]    [UC-004-STEP7]  Downloading ODT...
2026-04-22 06:30:40.456 [WARN]    [UC-004-STEP7]  SHA256 mismatch, retry 1/3
2026-04-22 06:30:42.789 [INFO]    [UC-004-STEP7]  Retrying download...
2026-04-22 06:30:48.123 [SUCCESS] [UC-004-STEP7]  SHA256 verified (attempt 2)
2026-04-22 06:30:50.456 [INFO]    [UC-004-STEP8]  Generating configuration.xml...
2026-04-22 06:30:52.789 [SUCCESS] [UC-004]        All validations passed
2026-04-22 06:31:00.123 [INFO]    [UC-005]        Starting installation...
2026-04-22 06:32:45.456 [SUCCESS] [UC-005]        Installation completed
```

**Log Levels:**
- **INFO:** General informational messages
- **SUCCESS:** Operation completed successfully
- **WARN:** Warning, but continuing
- **ERROR:** Error, no recovery
- **DEBUG:** Detailed diagnostic (development only)

**Log File Location:** `$env:TEMP\OfficeAutomator-{YYYYMMDD-HHmmss}.log`

### 6.2 Logging at Key Points

```powershell
function Write-OfficeLog {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [ValidateSet('INFO', 'SUCCESS', 'WARN', 'ERROR', 'DEBUG')]
        [string]$Level,
        
        [Parameter(Mandatory = $true)]
        [string]$Message,
        
        [Parameter(Mandatory = $false)]
        [string]$Component = 'CORE'
    )
    
    $timestamp = Get-Date -Format 'yyyy-MM-dd HH:mm:ss.fff'
    $logEntry = "$timestamp [$Level] [$Component] $Message"
    
    # Write to console
    $color = @{
        'INFO'    = 'White'
        'SUCCESS' = 'Green'
        'WARN'    = 'Yellow'
        'ERROR'   = 'Red'
        'DEBUG'   = 'Gray'
    }
    Write-Host $logEntry -ForegroundColor $color[$Level]
    
    # Write to log file
    Add-Content -Path $Config.LogFilePath -Value $logEntry
}
```

---

## 7. TESTING STRATEGY

### 7.1 Unit Testing

**UC-001 Tests:**
```powershell
Describe "UC-001: Select-OfficeVersion" {
    Context "Valid version selection" {
        It "should return '2024'" {
            $result = Select-OfficeVersion -Version '2024'
            $result | Should -Be '2024'
        }
    }
    Context "Invalid version" {
        It "should throw on unsupported version" {
            { Select-OfficeVersion -Version '2023' } | Should -Throw
        }
    }
}
```

**UC-004 Tests:**
```powershell
Describe "UC-004: Validate-OfficeConfiguration" {
    Context "Valid configuration" {
        It "should pass all 8 validation steps" {
            $config = @{
                Version = '2024'
                Languages = @('es-ES')
                ExcludedApps = @('Teams')
            }
            $result = Validate-OfficeConfiguration -Config $config
            $result.Success | Should -Be $true
        }
    }
    Context "Invalid language-app combination" {
        It "should fail Step 6 with error" {
            # Create config with incompatible combo
            $result = Validate-OfficeConfiguration -Config $config
            $result.Success | Should -Be $false
        }
    }
}
```

### 7.2 Integration Testing

**End-to-End Flow:**
```powershell
Describe "OfficeAutomator: Complete Workflow" {
    Context "Successful installation" {
        It "should complete all 5 UCs successfully" {
            # Mock user inputs
            Mock Read-Host { '1' }  # Select version 2024
            Mock Read-Host { '1' }  # Select language es-ES
            Mock Read-Host { '' }   # Default exclusions
            
            # Execute
            Invoke-OfficeAutomator
            
            # Verify
            (Get-OfficeInstallationVersion) | Should -Be '2024'
        }
    }
}
```

### 7.3 Edge Case Testing

- Invalid user input (non-numeric, out-of-range)
- Network timeout during ODT download
- Disk full during installation
- Insufficient admin privileges
- Office already installed (idempotence)
- Corrupted configuration.xml

---

## 8. INTEGRATION POINTS

### 8.1 External Dependencies

| Dependency | Type | Purpose | Version |
|------------|------|---------|---------|
| Microsoft ODT (setup.exe) | Executable | Office installation | Per version |
| Windows Installer (msiexec) | System | Underlying installer | Built-in |
| PowerShell runtime | Runtime | Script execution | 5.1+ |

### 8.2 Data Files

| File | Format | Content | Location |
|------|--------|---------|----------|
| supported-versions.json | JSON | Available versions with support dates | Data/ |
| supported-languages.json | JSON | Available languages per version | Data/ |
| language-compatibility-matrix.json | JSON | Version×Language×App compatibility | Data/ |
| odt-checksums.json | JSON | SHA256 values for each ODT version | Data/ |

---

## 9. PERFORMANCE SPECIFICATIONS

### 9.1 Timeouts

| Operation | Timeout | Justification |
|-----------|---------|----------------|
| User selection (UC-001/002/003) | 5 minutes | Reasonable for admin decision |
| Validation (UC-004) | 5 minutes | Network + processing |
| Installation (UC-005) | 30 minutes | Setup.exe execution |
| Single network retry | 10 minutes | Per download attempt |

### 9.2 Resource Requirements

| Resource | Requirement |
|----------|------------|
| Disk space | 3 GB minimum |
| RAM | 512 MB minimum |
| Network bandwidth | 50 Mbps recommended |
| CPU | Any modern processor |

---

## 10. SECURITY CONSIDERATIONS

### 10.1 Input Validation

**All user input must be:**
- Validated against whitelist (no freeform input)
- Length-checked (prevent buffer overflow)
- Type-checked (numeric only for selections)

**Example:**
```powershell
if ($choice -notin @('1', '2', '3')) {
    throw "Invalid input"
}
```

### 10.2 File Integrity

**SHA256 verification prevents:**
- Corrupted downloads
- Man-in-the-middle attacks
- Tampering with ODT

**Retry logic (3x) handles transient failures without compromising security**

### 10.3 Logging Security

**Logs must NOT contain:**
- Passwords or credentials
- Personal identification information
- Sensitive registry values

**Logs safely contain:**
- Timestamps
- Version/language/app selections
- Error messages
- Installation results

---

## 11. DEPLOYMENT ARCHITECTURE

### 11.1 Module Installation

```powershell
# Copy module to PSModulePath
Copy-Item -Path "OfficeAutomator" -Destination "$PROFILE\..\Modules" -Recurse

# OR register in PSModulePath
[Environment]::SetEnvironmentVariable(
    "PSModulePath",
    "$env:PSModulePath;C:\Path\To\OfficeAutomator",
    "User"
)

# Import and use
Import-Module OfficeAutomator
Invoke-OfficeAutomator
```

### 11.2 Scope

- **Per-Machine:** Single installation affects all users
- **Local only:** No network shares or roaming profiles
- **Admin required:** Must run as administrator

---

## 12. FUTURE ROADMAP (v1.1+)

| Feature | Version | Status |
|---------|---------|--------|
| Additional languages (6+) | v1.1 | Planned |
| WPF GUI | v1.1 | Planned |
| Group Policy templates | v1.1 | Planned |
| Intune integration | v1.2 | Planned |
| Project/Visio licensing | v1.1 | Planned |
| Office 365 support | v2.0 | Planned |

---

**Design Status:** COMPLETE
**Version:** 1.0.0
**Approval:** Ready for implementation (Stage 10)
**Reviewer:** Architecture Review Board

