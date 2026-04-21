---
# THESE SECTIONS EXPAND OPTION-B-DEPENDENCY-ANALYSIS-PLAN.md
# Insert after current §7 (Script Breakdown)
# Total addition: ~420-620 lines covering all 7 gaps
---

## 8. Script-by-Script Implementation Specifications

### 8.1 Script 1: OfficeAutomator.CoreDll.Loader.ps1 (50 líneas)

**Function Signature:**
```powershell
function Load-OfficeAutomatorCoreDll {
    param(
        [Parameter(Mandatory = $true)]
        [ValidateScript({ Test-Path $_ -PathType Leaf })]
        [string]$DllPath
    )
    # Implementation
}
```

**Algorithm (Pseudocode):**
```
1. Validate $DllPath exists (pre-condition check)
2. Try: Load DLL via [System.Reflection.Assembly]::LoadFrom($DllPath)
3. If LoadFrom succeeds:
   a. Try to instantiate OfficeAutomator.Core.Models.Configuration
   b. Try to instantiate OfficeAutomator.Core.Validation.ConfigValidator
   c. If both instantiate: Return $true (DLL valid)
   d. If either fails: Throw exception (DLL missing required classes)
4. If LoadFrom fails:
   a. Catch exception
   b. Write error message
   c. Throw exception to caller
```

**Error Scenarios:**
- DLL file not found → FileNotFoundException → Error Code 2001
- DLL is not valid .NET assembly → BadImageFormatException → Error Code 2002
- Required classes missing in DLL → TypeLoadException → Error Code 2003

**Return Value:**
- `$true` on success
- Throws exception on any error (no try-catch in caller expected)

**Side Effects:**
- Loads OfficeAutomator.Core.dll into current PowerShell process memory
- DLL remains loaded for lifetime of PowerShell session

**Integration Points:**
- Called ONCE by main script (§7) during Phase 1: PREREQUISITES
- No C# method invocations; pure .NET reflection

---

### 8.2 Script 2: OfficeAutomator.Validation.Environment.ps1 (80 líneas)

**Function Signatures:**
```powershell
function Test-AdminRights { }
function Test-CoreDllExists { param([string]$DllPath) }
function Test-DotNetRuntime { }
function Test-PrerequisitesMet { param([string]$DllPath) }
```

**Test-AdminRights Algorithm:**
```
1. Use [System.Security.Principal.WindowsPrincipal]
2. Get current identity
3. Check if in Administrators role
4. Return $true or $false
```

**Test-CoreDllExists Algorithm:**
```
1. Validate $DllPath parameter exists and is string
2. Check: Test-Path $DllPath -PathType Leaf
3. Return $true or $false
```

**Test-DotNetRuntime Algorithm:**
```
1. Use [System.Runtime.InteropServices.RuntimeInformation]
2. Get FrameworkDescription
3. Parse version (looking for "8.0" or higher)
4. Return $true if 8.0+, $false otherwise
```

**Test-PrerequisitesMet Algorithm:**
```
1. Call Test-AdminRights → if $false: return $false
2. Call Test-CoreDllExists → if $false: return $false
3. Call Test-DotNetRuntime → if $false: return $false
4. If all three: return $true
```

**Error Scenarios:**
- Not running as admin → Test-AdminRights returns $false → Error Code 2001
- DLL path invalid → Test-CoreDllExists returns $false → Error Code 2001
- .NET 8.0+ not installed → Test-DotNetRuntime returns $false → Error Code 2001

---

### 8.3 Script 3: OfficeAutomator.Logging.Handler.ps1 (40 líneas)

**Function Signature:**
```powershell
function Write-LogEntry {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Message,
        
        [Parameter(Mandatory = $false)]
        [ValidateSet('INFO', 'SUCCESS', 'WARNING', 'ERROR')]
        [string]$Level = 'INFO',
        
        [Parameter(Mandatory = $true)]
        [string]$LogPath
    )
}
```

**Algorithm:**
```
1. Get current timestamp: $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
2. Build log message: "$timestamp [$Level] $Message"
3. Append to $LogPath file (using Add-Content, create if not exists)
4. Write to console (using Write-Host with color based on $Level)
5. Return nothing
```

**Color Mapping:**
```
INFO    → Cyan (console input hint)
SUCCESS → Green (positive outcome)
WARNING → Yellow (caution)
ERROR   → Red (failure)
```

**Error Scenarios:**
- $LogPath directory doesn't exist → Create parent directory first
- No write permission → Exception propagates to caller
- Invalid $Level → Caught by [ValidateSet], PowerShell rejects

---

### 8.4 Script 4: OfficeAutomator.Menu.Display.ps1 (100 líneas)

**Function Signature:**
```powershell
function Show-Menu {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Title,
        
        [Parameter(Mandatory = $true)]
        [string[]]$Options
    )
    # Returns: [int] (1-based index)
}
```

**Algorithm:**
```
1. Display blank line
2. Display Title in Cyan (Write-Host -ForegroundColor Cyan)
3. Loop through $Options with index (1-based):
   For $i = 0 to $Options.Count-1:
     Write-Host "$($i+1). $($Options[$i])"
4. Display blank line
5. Loop until valid input:
   a. Read-Host prompt: "Seleccione una opción:"
   b. Try parse input as [int]
   c. If not int: Write error (Cyan), loop again
   d. If int but out of range (1 to $Options.Count): Write error, loop again
   e. If valid: Return the [int] (1-based)
```

**Error Scenarios:**
- User enters non-numeric (e.g., "abc") → Show error, loop
- User enters out-of-range number (e.g., "5" when only 3 options) → Show error, loop
- User presses Ctrl+C → PowerShell exception propagates (not caught)

**Return Value:**
- `[int]` 1-based index (1, 2, 3, etc.)

---

### 8.5 Script 5: OfficeAutomator.Execution.Orchestration.ps1 (150 líneas)

**Function Signatures:**
```powershell
function Invoke-OfficeInstallation {
    param([Parameter(Mandatory = $true)] $Configuration)
}

function Invoke-ValidationStep {
    param([Parameter(Mandatory = $true)] $Configuration)
}

function Invoke-InstallationStep {
    param([Parameter(Mandatory = $true)] $Configuration)
}

function Show-ProgressBar {
    param([int]$Percent, [string]$Message)
}
```

**Invoke-OfficeInstallation Algorithm:**
```
1. Invoke Invoke-ValidationStep($Configuration)
   - If returns $false: return $false
   - If returns $true: continue to step 2
2. Show-ProgressBar 30 "Downloading Office..."
3. Invoke Invoke-InstallationStep($Configuration)
   - If returns $true: Show-ProgressBar 100 "Completed!", return $true
   - If returns $false: return $false
```

**Invoke-ValidationStep Algorithm:**
```
1. Create ConfigValidator object: $validator = [OfficeAutomator.Core.Validation.ConfigValidator]::new()
2. Call $validator.Execute($Configuration)
3. If exception thrown: Catch, log error, return $false
4. If returns $false: Log error, return $false
5. If returns $true: return $true
```

**Invoke-InstallationStep Algorithm:**
```
1. Create InstallationExecutor object
2. Try: Call $executor.Execute($Configuration)
3. If exception: Catch, log, return $false
4. If $executor.Execute returns $false: Log error, return $false
5. If $executor.Execute returns $true: Log success, return $true
```

**Show-ProgressBar Algorithm:**
```
1. Build progress bar string: "[" + ("█" * ($Percent/10)) + ("░" * (10-$Percent/10)) + "]"
2. Build full message: "$ProgressBar $Percent% - $Message"
3. Write to console (Write-Host)
4. Return (no output)
```

---

### 8.6 Script 6: OfficeAutomator.Execution.RollbackHandler.ps1 (80 líneas)

**Function Signature:**
```powershell
function Invoke-RollbackOnFailure {
    param(
        [Parameter(Mandatory = $true)]
        $Configuration
    )
    # Returns: $true on rollback completion, $false if rollback fails
}
```

**Algorithm:**
```
1. Write-LogEntry "Starting rollback..." -Level "WARNING"
2. Create RollbackExecutor object: $rollback = [OfficeAutomator.Core.Installation.RollbackExecutor]::new()
3. Try: Call $rollback.Execute()
4. If exception during rollback:
   a. Write-LogEntry "Rollback failed: [error details]" -Level "ERROR"
   b. return $false
5. If $rollback.Execute() succeeds:
   a. Write-LogEntry "Rollback completed successfully" -Level "SUCCESS"
   b. return $true
```

**Error Scenarios:**
- RollbackExecutor throws exception → Catch, log, return $false
- RollbackExecutor.Execute() returns $false → Log error, return $false

---

### 8.7 Script 7: OfficeAutomator.PowerShell.Script.ps1 (250-300 líneas)

**Main Script Structure (High-Level):**

```powershell
#requires -RunAsAdministrator

# Phase 0: Headers, comments, requires

# Phase 1: Load all function files (dot-source pattern)
$FunctionPath = Join-Path (Split-Path -Parent $MyInvocation.MyCommand.Path) "functions"
Get-ChildItem "$FunctionPath\*.ps1" | ForEach-Object { . $_.FullName }

# Phase 2: Main function definition
function Main {
    # Phase 1: Validate prerequisites
    if (-not (Test-PrerequisitesMet -DllPath $DllPath)) {
        Write-LogEntry "Prerequisites failed" -Level "ERROR"
        exit 1
    }
    
    # Phase 2: Load DLL
    if (-not (Load-OfficeAutomatorCoreDll -DllPath $DllPath)) {
        Write-LogEntry "DLL load failed" -Level "ERROR"
        exit 1
    }
    
    # Phase 3-5: UC-001, UC-002, UC-003 (menus)
    $versionChoice = Show-Menu -Title "Seleccione versión" -Options @("2024", "2021", "2019")
    $languageChoice = Show-Menu -Title "Seleccione idioma" -Options @("es-ES", "en-US", ...)
    $appChoice = Show-Menu -Title "Seleccione exclusiones" -Options @("Teams", "OneDrive", ...)
    
    # Phase 6: Summary + Confirmation
    Show-ConfigurationSummary
    $confirm = Read-Host "¿Continuar? (S/N)"
    if ($confirm -ne "S") { exit 0 }
    
    # Phase 7-8: UC-004 (validation) + UC-005 (installation)
    if (-not (Invoke-OfficeInstallation -Configuration $config)) {
        Write-LogEntry "Installation failed, rolling back" -Level "ERROR"
        Invoke-RollbackOnFailure -Configuration $config
        exit 1
    }
    
    # Phase 9: Cleanup + results
    [System.GC]::Collect()
    Write-LogEntry "OfficeAutomator completed successfully" -Level "SUCCESS"
    Write-Host "Log: $LogPath"
    exit 0
}

# Phase 3: Call Main
Main
```

---

## 9. PowerShell-to-C# Interop Specification

### 9.1 DLL Loading Pattern (Detailed)

**File:** OfficeAutomator.CoreDll.Loader.ps1

**Implementation:**
```powershell
[System.Reflection.Assembly]::LoadFrom($DllPath) | Out-Null

# This makes available:
#   [OfficeAutomator.Core.Models.Configuration]
#   [OfficeAutomator.Core.Models.OfficeAutomatorError]
#   [OfficeAutomator.Core.Services.VersionSelector]
#   [OfficeAutomator.Core.Services.LanguageSelector]
#   [OfficeAutomator.Core.Services.AppExclusionSelector]
#   [OfficeAutomator.Core.Validation.ConfigValidator]
#   [OfficeAutomator.Core.Installation.InstallationExecutor]
#   [OfficeAutomator.Core.Installation.RollbackExecutor]
```

---

### 9.2 C# Object Creation Pattern

**Pattern 1: Parameterless Constructor (Configuration)**
```powershell
$config = New-Object OfficeAutomator.Core.Models.Configuration

# Sets properties after creation:
$config.Version = "PerpetualVL2024"
$config.Language = "es-ES"
$config.ExcludedApps = @("Teams", "OneDrive")
```

**Pattern 2: Service Object Creation (Selectors)**
```powershell
$versionSelector = New-Object OfficeAutomator.Core.Services.VersionSelector
$result = $versionSelector.Execute($config)

# Expects:
#   - $config has required properties set
#   - Execute() returns [bool]
#   - If false: Check exception in catch block
```

---

### 9.3 Exception Handling Pattern

**General Pattern:**
```powershell
try {
    $result = $selector.Execute($config)
    if (-not $result) {
        Write-LogEntry "Execution failed" -Level "ERROR"
        return $false
    }
}
catch [System.ArgumentException] {
    Write-LogEntry "Invalid argument: $($_.Exception.Message)" -Level "ERROR"
    return $false
}
catch [System.IO.FileNotFoundException] {
    Write-LogEntry "File not found: $($_.Exception.Message)" -Level "ERROR"
    return $false
}
catch [System.Exception] {
    Write-LogEntry "Unexpected error: $($_.Exception.Message)" -Level "ERROR"
    return $false
}
```

**Key Points:**
- Always catch by exception type (most specific first)
- Log full exception message
- Return $false or throw (depends on context)
- Never swallow exceptions silently

---

### 9.4 Configuration Object Structure

**Required Properties for Each UC:**

UC-001 (VersionSelector):
```
$config.Version = "PerpetualVL2024" | "PerpetualVL2021" | "PerpetualVL2019"
```

UC-002 (LanguageSelector):
```
$config.Language = "es-ES" | "en-US" | ... (5 languages total)
```

UC-003 (AppExclusionSelector):
```
$config.ExcludedApps = @("Teams", "OneDrive") # Can be empty array
```

UC-004 (ConfigValidator):
```
# All above must be set before calling ConfigValidator.Execute()
```

UC-005 (InstallationExecutor):
```
# All above must be set and validated
```

---

## 10. Error Handling Implementation Map

**Error Code → Script → Log Level → User Message → Behavior**

| Error Code | Script | C# Class | Log Level | User Message | Behavior |
|-----------|--------|----------|-----------|--------------|----------|
| **001** | Script 3 (Validation.Env) | — | ERROR | "Versión inválida seleccionada" | Exit 1 |
| **002** | Script 3 (Validation.Env) | ConfigValidator | ERROR | "Idioma no soportado: %s" | Exit 1 |
| **003** | Script 3 (Validation.Env) | ConfigValidator | ERROR | "Espacio insuficiente: necesita 5GB, disponible %sGB" | Exit 1 |
| **004** | Script 3 (Validation.Env) | ConfigValidator | ERROR | "No hay conexión de red disponible" | Exit 1 |
| **005** | Script 3 (Validation.Env) | ConfigValidator | ERROR | "Descarga corrupta (validación hash falló)" | Exit 1 |
| **006** | Script 2 (Validation.Env) | — | ERROR | "Se requieren permisos de administrador" | Exit 1 |
| **007** | Script 2 (Validation.Env) | — | ERROR | "PowerShell 5.1+ requerido, versión actual: %s" | Exit 1 |
| **008** | Script 3 (Validation.Env) | ConfigValidator | ERROR | "setup.exe inválido o corrupto" | Exit 1 |
| **1001** | Script 5 (Execution.Orchestration) | InstallationExecutor | ERROR | "Timeout descargando Office (300s excedido)" | Rollback |
| **1002** | Script 5 (Execution.Orchestration) | InstallationExecutor | ERROR | "Descarga corrupta (intento 3 de 3)" | Rollback |
| **1003** | Script 5 (Execution.Orchestration) | InstallationExecutor | ERROR | "Fallo al extraer OfficeDeploymentTool.exe" | Rollback |
| **1004** | Script 5 (Execution.Orchestration) | InstallationExecutor | ERROR | "setup.exe falló con código de error: %d" | Rollback |
| **1005** | Script 5 (Execution.Orchestration) | InstallationExecutor | ERROR | "Fallo al modificar registro" | Rollback |
| **1006** | Script 6 (Execution.RollbackHandler) | RollbackExecutor | ERROR | "Rollback falló (limpieza incompleta)" | Exit 1 |
| **2001** | Script 1 (CoreDll.Loader) | — | ERROR | "DLL no encontrada: %s" | Exit 1 |
| **2002** | Script 1 (CoreDll.Loader) | — | ERROR | "DLL no es un assembly .NET válido" | Exit 1 |
| **2003** | Script 1 (CoreDll.Loader) | — | ERROR | "Clases requeridas no encontradas en DLL" | Exit 1 |
| **2004** | Script 7 (Main) | — | ERROR | "Error inesperado: %s" | Exit 1 |

**Legend:**
- Exit 1 = Error, script terminates
- Rollback = InstallationExecutor failed, execute RollbackExecutor, then exit 1

---

## 11. Logging Configuration Specification

**Log File Configuration:**
```powershell
$LogPath = "$env:TEMP\OfficeAutomator_$(Get-Date -Format 'yyyyMMdd_HHmmss').log"
```

**File Creation:**
- Created on first Write-LogEntry call (if doesn't exist)
- Parent directory must exist (%TEMP% always exists)
- Append mode (multiple runs = single log)

**Log Levels:**
- INFO: Informational messages (script progress)
- SUCCESS: Operation completed successfully
- WARNING: Caution (e.g., rollback started)
- ERROR: Operation failed

**Console Output Colors:**
```
INFO    → Cyan
SUCCESS → Green
WARNING → Yellow
ERROR   → Red
```

**Log Format:**
```
[2026-04-21 10:15:30] [INFO] Starting OfficeAutomator PowerShell
[2026-04-21 10:15:31] [SUCCESS] Prerequisites validated
[2026-04-21 10:15:32] [ERROR] Installation failed
```

**Retention:**
- Keep all log files indefinitely
- User responsible for cleanup
- No automatic rotation or deletion

---

## 12. Test Framework Configuration

**Pester Version:** 5.x minimum

**Test File Locations:**
```
tests/PowerShell/
├── OfficeAutomator.PowerShell.Integration.Tests.ps1
├── OfficeAutomator.PowerShell.EndToEnd.Tests.ps1
└── pester.configuration.psd1
```

**Pester Configuration File (pester.configuration.psd1):**
```powershell
@{
    Run = @{
        Path = '.\tests\PowerShell\'
        PassThru = $true
        TestExtension = '.Tests.ps1'
    }
    
    TestResult = @{
        OutputFormat = 'NUnitXml'
        OutputPath = '.\test-results.xml'
    }
    
    CodeCoverage = @{
        Enabled = $true
        Path = '.\scripts\functions\*'
        OutputFormat = 'JaCoCo'
        OutputPath = '.\coverage.xml'
        CoveragePercentTarget = 90
    }
}
```

**Test Naming Convention:**
```powershell
Describe "OfficeAutomator.CoreDll.Loader" {
    Context "Load-OfficeAutomatorCoreDll function" {
        It "loads DLL successfully when file exists" {
            # Arrange
            $dllPath = ".\test-fixtures\OfficeAutomator.Core.dll"
            
            # Act
            $result = Load-OfficeAutomatorCoreDll -DllPath $dllPath
            
            # Assert
            $result | Should -Be $true
        }
    }
}
```

**Mocking C# Objects:**
```powershell
# Mock Configuration object creation
$mockConfig = @{
    Version = "PerpetualVL2024"
    Language = "es-ES"
    ExcludedApps = @("Teams")
} | ConvertTo-Json | ConvertFrom-Json

# Mock C# method calls
Mock -ModuleName OfficeAutomator.Core -CommandName "VersionSelector.Execute" -MockWith { return $true }
```

**Coverage Measurement:**
- Target: >90% overall
- Line-level coverage tracked
- Function-level coverage tracked
- All error paths tested

---

## 13. Build & Deployment Specification

**Folder Structure Required:**
```
scripts/
├── OfficeAutomator.PowerShell.Script.ps1
└── functions/
    ├── OfficeAutomator.CoreDll.Loader.ps1
    ├── OfficeAutomator.Validation.Environment.ps1
    ├── OfficeAutomator.Logging.Handler.ps1
    ├── OfficeAutomator.Menu.Display.ps1
    ├── OfficeAutomator.Execution.Orchestration.ps1
    └── OfficeAutomator.Execution.RollbackHandler.ps1
```

**Function Loading Pattern (in main script):**
```powershell
$ScriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$FunctionPath = Join-Path $ScriptPath "functions"

# Validate functions folder exists
if (-not (Test-Path $FunctionPath)) {
    Write-Error "Functions folder not found: $FunctionPath"
    exit 1
}

# Load all function files
Get-ChildItem "$FunctionPath\*.ps1" | ForEach-Object {
    . $_.FullName
}

# Validate all functions loaded
$requiredFunctions = @(
    'Load-OfficeAutomatorCoreDll',
    'Test-AdminRights',
    'Test-CoreDllExists',
    'Test-DotNetRuntime',
    'Write-LogEntry',
    'Show-Menu',
    'Invoke-OfficeInstallation',
    'Invoke-RollbackOnFailure'
)

foreach ($func in $requiredFunctions) {
    if (-not (Get-Command $func -ErrorAction SilentlyContinue)) {
        Write-Error "Function not found: $func"
        exit 1
    }
}
```

**DLL Path Verification:**
```powershell
$DllPath = ".\src\OfficeAutomator.Core\bin\Release\net8.0\OfficeAutomator.Core.dll"

if (-not (Test-Path $DllPath -PathType Leaf)) {
    Write-Error "Core DLL not found: $DllPath"
    exit 1
}
```

**Build Validation Steps (Pre-Execution):**
1. Verify PowerShell version (5.1+)
2. Verify .NET 8.0 runtime installed
3. Verify folder structure complete
4. Verify all function files present
5. Verify Core.dll exists at expected path
6. Load functions and verify all functions available
7. Load Core.dll and verify all classes available

---

## 14. UC-to-Script Mapping Table

**Complete Use Case to Script/C# Class Mapping:**

| UC # | UC Name | Script | Phase | C# Class | C# Method | Parameter | Return |
|------|---------|--------|-------|----------|-----------|-----------|--------|
| **UC-001** | Version Selection | 7 (Main) | 3 | VersionSelector | Execute() | Configuration | bool |
| **UC-002** | Language Selection | 7 (Main) | 4 | LanguageSelector | Execute() | Configuration | bool |
| **UC-003** | App Exclusion | 7 (Main) | 5 | AppExclusionSelector | Execute() | Configuration | bool |
| **UC-004** | Validation | 5 (Orch.) | 7 | ConfigValidator | Execute() | Configuration | bool |
| **UC-005** | Installation | 5 (Orch.) | 8 | InstallationExecutor | Execute() | Configuration | bool |
| **UC-005** | Rollback | 6 (Rollbk) | 8* | RollbackExecutor | Execute() | (none) | bool |

**Calling Pattern:**

```powershell
# UC-001: Version Selection
$selector1 = New-Object OfficeAutomator.Core.Services.VersionSelector
$result = $selector1.Execute($config)

# UC-004: Configuration Validation
$validator = New-Object OfficeAutomator.Core.Validation.ConfigValidator
$result = $validator.Execute($config)

# UC-005: Installation
$executor = New-Object OfficeAutomator.Core.Installation.InstallationExecutor
$result = $executor.Execute($config)

# UC-005: Rollback (if installation fails)
$rollback = New-Object OfficeAutomator.Core.Installation.RollbackExecutor
$result = $rollback.Execute()  # No parameters
```

**State Machine Transitions:**
```
UC-001 (success) → UC-002
UC-002 (success) → UC-003
UC-003 (success) → User Confirmation
Confirmation (yes) → UC-004
UC-004 (success) → UC-005
UC-005 (failure) → Rollback (UC-005 part 2)
Rollback (any) → END
```

**Acceptance Criteria Verification:**
- UC-001: User can select 3 versions, selection saved to $config.Version
- UC-002: User can select 5 languages, selection saved to $config.Language
- UC-003: User can select multiple apps, selection saved to $config.ExcludedApps
- UC-004: All 8 validation steps execute in sequence, return bool
- UC-005: Installation executes with progress bars, rollback atomic (all-or-nothing)

---

## EXPANSION COMPLETE

**Sections Added:** §8-§14  
**Lines Added:** ~550-650  
**Gaps Closed:** 7/7  
**Status:** PHASE 4 NOW READY FOR IMPLEMENTATION GATE

