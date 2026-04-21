```yaml
phase: Phase 3 DESIGN
wp: 2026-04-21-14-32-00-option-b-detailed-design
methodology: Technical Architecture Design
date_created: 2026-04-21
status: IN_PROGRESS
depends_on: 
  - 2026-04-21-14-30-00-option-b-powershell-wrapper-analysis
  - 2026-04-21-14-31-00-option-b-requirements-specification
```

# Phase 3: DESIGN - Detailed Architecture & Component Specification

## 1. Architecture Overview

### System Architecture Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                    SYSADMIN (User)                          │
└────────────────────────────┬────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────┐
│  OfficeAutomator.PowerShell.Script.ps1 (ENTRY POINT)       │
│  ├─ Initialization (Load DLL, validate prerequisites)      │
│  ├─ Menu orchestration                                      │
│  └─ Progress monitoring                                     │
└────────────────────────────┬────────────────────────────────┘
                             │
                ┌────────────┼────────────┬──────────────┐
                ▼            ▼            ▼              ▼
        ┌────────────┐ ┌──────────┐ ┌─────────┐ ┌────────────┐
        │  UC-001    │ │ UC-002   │ │ UC-003  │ │ UC-004     │
        │  Version   │ │ Language │ │ App     │ │ Validation │
        │ Selection  │ │Selection │ │Exclusion│ │ (8 steps)  │
        └────────────┘ └──────────┘ └─────────┘ └────────────┘
                │            │            │              │
                └────────────┴────────────┴──────────────┘
                             │
                ┌────────────┴────────────┐
                ▼                         ▼
        ┌─────────────────┐    ┌─────────────────────┐
        │  UC-005         │    │  RollbackExecutor   │
        │  Installation   │    │  (on failure)       │
        │  & Rollback     │    │                     │
        └─────────────────┘    └─────────────────────┘
                │                         │
                └────────────┬────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────┐
│        OfficeAutomator.Core.dll (C# - LOGIC LAYER)         │
│  ├─ VersionSelector         (UC-001 implementation)        │
│  ├─ LanguageSelector        (UC-002 implementation)        │
│  ├─ AppExclusionSelector    (UC-003 implementation)        │
│  ├─ ConfigValidator         (UC-004 implementation)        │
│  ├─ InstallationExecutor    (UC-005 installation part)     │
│  └─ RollbackExecutor        (UC-005 rollback part)         │
└────────────────────────────┬────────────────────────────────┘
                             │
                ┌────────────┼──────────────┐
                ▼            ▼              ▼
        ┌────────────┐ ┌──────────┐ ┌────────────┐
        │ Office     │ │ Windows  │ │ Windows    │
        │ Deployment │ │ Registry │ │ Filesystem │
        │ Tool       │ │          │ │            │
        └────────────┘ └──────────┘ └────────────┘
```

---

## 2. Component Architecture

### 2.1 PowerShell Layer Components

```
scripts/
├── OfficeAutomator.PowerShell.Script.ps1  [MAIN SCRIPT - 250-300 líneas]
│   Responsabilidad: Orquestación de flujo, interfaz usuario, logging
│   ├─ Load prerequisites
│   ├─ Initialize configuration object
│   ├─ Menu loop (UC-001, UC-002, UC-003)
│   ├─ Summary & confirmation
│   ├─ Invoke validation (UC-004)
│   ├─ Invoke installation (UC-005)
│   ├─ Handle rollback if needed
│   └─ Display results
│
└── functions/
    ├─ OfficeAutomator.Menu.Display.ps1  [MENU HANDLER - 100 líneas]
    │  Responsabilidad: Mostrar menús, capturar entrada, validar
    │  Función: Show-Menu
    │    Parámetro: -Title, -Options
    │    Retorna: [int] - Opción seleccionada (1-based)
    │    Validación: Reintentar si entrada inválida
    │
    ├─ OfficeAutomator.Validation.Environment.ps1  [PREREQ CHECK - 80 líneas]
    │  Responsabilidad: Validar ambiente antes de empezar
    │  Funciones:
    │    - Test-AdminRights → [bool]
    │    - Test-CoreDllExists → [bool]
    │    - Test-DotNetRuntime → [bool]
    │    - Test-PrerequisitesMet → [bool]
    │
    ├─ OfficeAutomator.CoreDll.Loader.ps1  [DLL LOADER - 50 líneas]
    │  Responsabilidad: Cargar DLL, manejo de excepciones
    │  Función: Load-OfficeAutomatorCoreDll
    │    Parámetro: -DllPath
    │    Retorna: [bool]
    │    Error handling: Detailed error messages
    │
    ├─ OfficeAutomator.Logging.Handler.ps1  [LOGGING - 40 líneas]
    │  Responsabilidad: Logging centralizado
    │  Función: Write-LogEntry
    │    Parámetros: -Message, -Level, -LogPath
    │    Niveles: INFO, SUCCESS, WARNING, ERROR
    │    Salida: Pantalla + archivo
    │
    ├─ OfficeAutomator.Execution.Orchestration.ps1  [EXECUTION - 150 líneas]
    │  Responsabilidad: Orquestar UC-001 a UC-005
    │  Funciones:
    │    - Invoke-OfficeInstallation (main orchestration)
    │    - Invoke-ValidationStep (UC-004)
    │    - Invoke-InstallationStep (UC-005)
    │    - Show-ProgressBar (progress indication)
    │
    └─ OfficeAutomator.Execution.RollbackHandler.ps1  [ROLLBACK - 80 líneas]
       Responsabilidad: Ejecutar rollback automático
       Función: Invoke-RollbackOnFailure
         Parámetro: -Configuration
         Lógica: Llama RollbackExecutor.Execute() de C#
         Atomicidad: Si falla una parte, revertir todo
```

### 2.2 C# Layer (Existing - No Changes)

```
OfficeAutomator.Core.dll (11 clases, 220+ tests - EXISTENTES)

Classes Used:
├─ VersionSelector.cs
│  └─ Execute(Configuration) → bool
│
├─ LanguageSelector.cs
│  └─ Execute(Configuration) → bool
│
├─ AppExclusionSelector.cs
│  └─ Execute(Configuration) → bool
│
├─ ConfigValidator.cs
│  └─ Execute(Configuration) → bool (8-step validation)
│
├─ ConfigGenerator.cs
│  └─ Generate(Configuration) → XElement
│
├─ InstallationExecutor.cs
│  └─ Execute(Configuration) → bool (download, extract, install)
│
├─ RollbackExecutor.cs
│  └─ Execute() → bool (4-part cleanup)
│
├─ ErrorHandler.cs
│  └─ CreateError(int code, string message) → OfficeAutomatorError
│
├─ OfficeAutomatorStateMachine.cs
│  └─ TransitionTo(State) → bool
│
├─ Configuration.cs (Model)
│  └─ Properties: Version, Language, Applications, ExcludedApps
│
└─ OfficeAutomatorError.cs (Model)
   └─ Properties: ErrorCode, ErrorMessage, Timestamp
```

---

## 3. Execution Flow - Detailed

### 3.1 Main Script Flow (OfficeAutomator.PowerShell.Script.ps1)

```
1. STARTUP
   ├─ Import-Module / dot-source functions
   ├─ Set error handling: $ErrorActionPreference = 'Stop'
   ├─ Define global variables ($DllPath, $LogPath, etc.)
   └─ Call Main function

2. MAIN FUNCTION
   ├─ Write-LogEntry "Starting OfficeAutomator PowerShell"
   │
   ├─ Phase 1: PREREQUISITES
   │  ├─ Test-AdminRights
   │  │  └─ If NOT admin → Write error, Exit 1
   │  ├─ Test-CoreDllExists
   │  │  └─ If NOT exists → Write error, Exit 1
   │  ├─ Test-DotNetRuntime
   │  │  └─ If NOT 8.0+ → Write warning
   │  └─ Write-LogEntry "Prerequisites validated"
   │
   ├─ Phase 2: LOAD DLL
   │  ├─ Load-OfficeAutomatorCoreDll -DllPath $DllPath
   │  ├─ $configuration = New-Object OfficeAutomator.Core.Models.Configuration
   │  └─ Write-LogEntry "Core DLL loaded successfully"
   │
   ├─ Phase 3: UC-001 VERSION SELECTION
   │  ├─ $versionOptions = @("Office LTSC 2024", "Office LTSC 2021", "Office LTSC 2019")
   │  ├─ $versionChoice = Show-Menu -Title "Seleccione versión" -Options $versionOptions
   │  ├─ $configuration.Version = Map($versionChoice) → "PerpetualVL2024"
   │  ├─ Write-LogEntry "Version selected: $($configuration.Version)"
   │  └─ $versionSelector = New-Object OfficeAutomator.Core.Services.VersionSelector
   │      $versionSelector.Execute($configuration)
   │
   ├─ Phase 4: UC-002 LANGUAGE SELECTION
   │  ├─ $languageOptions = @("Spanish (es-ES)", "English (en-US)", ...)
   │  ├─ $languageChoice = Show-Menu -Title "Seleccione idioma" -Options $languageOptions
   │  ├─ $configuration.Language = Map($languageChoice) → "es-ES"
   │  ├─ Write-LogEntry "Language selected: $($configuration.Language)"
   │  └─ $languageSelector = New-Object OfficeAutomator.Core.Services.LanguageSelector
   │      $languageSelector.Execute($configuration)
   │
   ├─ Phase 5: UC-003 APP EXCLUSION SELECTION
   │  ├─ $appOptions = @("Teams", "OneDrive", "Groove", "Lync", "Bing")
   │  ├─ $appChoice = Show-Menu -Title "Seleccione exclusiones" -Options $appOptions
   │  ├─ $configuration.ExcludedApps = Map($appChoice) → @("Teams", "OneDrive")
   │  ├─ Write-LogEntry "Exclusions set: $($configuration.ExcludedApps -join ', ')"
   │  └─ $appSelector = New-Object OfficeAutomator.Core.Services.AppExclusionSelector
   │      $appSelector.Execute($configuration)
   │
   ├─ Phase 6: DISPLAY SUMMARY
   │  ├─ Show-ConfigurationSummary $configuration
   │  │  └─ Display all selections
   │  └─ $confirm = Read-Host "¿Continuar con instalación? (S/N)"
   │     └─ If "N" → Log "Instalación cancelada", Exit 0
   │     └─ If "S" → Continue
   │
   ├─ Phase 7: UC-004 VALIDATION
   │  ├─ Write-LogEntry "Starting validation (8 steps)..."
   │  ├─ $validator = New-Object OfficeAutomator.Core.Validation.ConfigValidator
   │  ├─ Show-ProgressBar -Percent 20 -Message "Validating configuration..."
   │  ├─ $isValid = $validator.Execute($configuration)
   │  ├─ If NOT $isValid
   │  │  ├─ Get error code from $validator
   │  │  ├─ Write-LogEntry "ERROR: Validation failed at step X"
   │  │  ├─ Show detailed error message to user
   │  │  └─ Exit 1
   │  └─ If $isValid
   │     └─ Write-LogEntry "Validation passed (all 8 steps)"
   │
   ├─ Phase 8: UC-005 INSTALLATION
   │  ├─ Write-LogEntry "Starting installation..."
   │  ├─ Invoke-OfficeInstallation -Configuration $configuration
   │  │  ├─ $executor = New-Object OfficeAutomator.Core.Installation.InstallationExecutor
   │  │  ├─ Show progress: [████░░░░░░] 30% - Downloading...
   │  │  ├─ $success = $executor.Execute($configuration)
   │  │  └─ Show progress: [████████░░] 50% - Installing...
   │  │
   │  ├─ If NOT $success (Installation Failed)
   │  │  ├─ Write-LogEntry "Installation failed, executing rollback..."
   │  │  ├─ Invoke-RollbackOnFailure -Configuration $configuration
   │  │  │  └─ $rollback = New-Object OfficeAutomator.Core.Installation.RollbackExecutor
   │  │  │     $rollback.Execute()
   │  │  ├─ Show-ProgressBar -Percent 100 -Message "Sistema restaurado"
   │  │  ├─ Write-LogEntry "Rollback completed"
   │  │  ├─ Display error message to user
   │  │  └─ Exit 1
   │  │
   │  ├─ If $success (Installation Succeeded)
   │  │  ├─ Show-ProgressBar -Percent 100 -Message "¡Instalación completada!"
   │  │  ├─ Write-LogEntry "Installation completed successfully"
   │  │  └─ Exit 0
   │
   └─ Phase 9: CLEANUP & FINAL LOG
      ├─ [System.GC]::Collect() # Release C# objects
      ├─ Write-LogEntry "OfficeAutomator completed"
      ├─ Display log file location to user
      └─ End Main
```

---

## 4. PowerShell-to-C# Interop Design

### 4.1 DLL Loading Pattern

```powershell
# In OfficeAutomator.CoreDll.Loader.ps1
function Load-OfficeAutomatorCoreDll {
    param([string]$DllPath)
    
    try {
        # Load DLL into PowerShell context
        [System.Reflection.Assembly]::LoadFrom($DllPath)
        
        # Verify classes are available
        $config = New-Object OfficeAutomator.Core.Models.Configuration
        $validator = New-Object OfficeAutomator.Core.Validation.ConfigValidator
        
        Write-Host "✓ DLL loaded successfully" -ForegroundColor Green
        return $true
    }
    catch {
        Write-Host "ERROR: Failed to load DLL - $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}
```

### 4.2 C# Object Creation Pattern

```powershell
# In OfficeAutomator.Execution.Orchestration.ps1

# UC-001 Execution
$versionSelector = New-Object OfficeAutomator.Core.Services.VersionSelector
$result = $versionSelector.Execute($configuration)
if (-not $result) {
    $error = [OfficeAutomator.Core.Models.OfficeAutomatorError]::GetLastError()
    Write-LogEntry "ERROR: Version selection failed - $($error.ErrorMessage)" -Level "ERROR"
}

# UC-004 Validation (8 steps)
$validator = New-Object OfficeAutomator.Core.Validation.ConfigValidator
$isValid = $validator.Execute($configuration)  # All 8 steps inside C#
if (-not $isValid) {
    Write-LogEntry "ERROR: Configuration validation failed" -Level "ERROR"
}

# UC-005 Installation
$executor = New-Object OfficeAutomator.Core.Installation.InstallationExecutor
$success = $executor.Execute($configuration)
if (-not $success) {
    # Rollback automatically if installation fails
    $rollback = New-Object OfficeAutomator.Core.Installation.RollbackExecutor
    $rollback.Execute()
    Write-LogEntry "ERROR: Installation failed, rollback executed" -Level "ERROR"
}
```

### 4.3 Exception Handling Pattern

```powershell
# In main script

try {
    # All C# operations
    $versionSelector.Execute($configuration)
}
catch [System.ArgumentException] {
    Write-LogEntry "ERROR: Invalid argument - $($_.Exception.Message)" -Level "ERROR"
    Exit 1
}
catch [System.IO.FileNotFoundException] {
    Write-LogEntry "ERROR: Required file not found - $($_.Exception.Message)" -Level "ERROR"
    Exit 1
}
catch {
    Write-LogEntry "ERROR: Unexpected error - $($_.Exception.Message)" -Level "ERROR"
    Exit 1
}
```

---

## 5. Error Handling Architecture

### 5.1 Error Code System

```
C# Error Codes (UC-004 Validation):
  001 - Invalid version
  002 - Language not supported
  003 - Insufficient disk space
  004 - No network connection
  005 - Download integrity failed
  006 - Administrator required
  007 - PowerShell version too old
  008 - Invalid setup.exe

Installation Error Codes (UC-005):
  1001 - Download timeout
  1002 - Download corrupted
  1003 - Extract failed
  1004 - setup.exe failed
  1005 - Registry modification failed
  1006 - Rollback failed

PowerShell Layer Error Codes:
  2001 - DLL not found
  2002 - DLL loading failed
  2003 - Invalid input
  2004 - Unexpected error
```

### 5.2 Error Flow

```
Error occurs in C#
    ↓
ErrorHandler captures exception
    ↓
Creates OfficeAutomatorError object
    ├─ ErrorCode: int
    ├─ ErrorMessage: string
    └─ Timestamp: datetime
    ↓
Propagates to PowerShell
    ↓
PowerShell catches exception
    ↓
Write-LogEntry with error details
    ↓
Display user-friendly message
    ↓
If UC-005 failed: Invoke-RollbackOnFailure
    ↓
Exit with appropriate code
```

---

## 6. Logging Architecture

### 6.1 Log File Structure

```
%TEMP%\OfficeAutomator_yyyyMMdd_HHmmss.log

Format: [timestamp] [LEVEL] message

Example:
[2026-04-21 10:15:30] [INFO] Starting OfficeAutomator PowerShell
[2026-04-21 10:15:31] [SUCCESS] Prerequisites validated
[2026-04-21 10:15:32] [INFO] Core DLL loaded successfully
[2026-04-21 10:15:33] [INFO] Menu displayed: Version Selection
[2026-04-21 10:15:35] [INFO] User selected: Office LTSC 2024
[2026-04-21 10:15:36] [SUCCESS] Version selected: PerpetualVL2024
[2026-04-21 10:15:37] [INFO] Menu displayed: Language Selection
[2026-04-21 10:15:39] [SUCCESS] Language selected: es-ES
[2026-04-21 10:15:40] [INFO] Menu displayed: App Exclusion
[2026-04-21 10:15:42] [SUCCESS] Exclusions set: Teams, OneDrive
[2026-04-21 10:15:43] [INFO] Configuration summary displayed
[2026-04-21 10:15:45] [INFO] User confirmed: Continue
[2026-04-21 10:15:46] [INFO] Starting validation (8 steps)...
[2026-04-21 10:15:47] [INFO] Step 1/8: Validate version... OK
[2026-04-21 10:15:48] [INFO] Step 2/8: Validate language... OK
[2026-04-21 10:15:49] [INFO] Step 3/8: Check disk space... 25GB available OK
[2026-04-21 10:15:50] [INFO] Step 4/8: Check network... OK
[2026-04-21 10:15:51] [INFO] Step 5/8: Validate hash... OK
[2026-04-21 10:15:52] [INFO] Step 6/8: Verify admin rights... OK
[2026-04-21 10:15:53] [INFO] Step 7/8: Check PowerShell version... 5.1 OK
[2026-04-21 10:15:54] [INFO] Step 8/8: Validate setup.exe... OK
[2026-04-21 10:15:55] [SUCCESS] Validation passed (all 8 steps)
[2026-04-21 10:15:56] [INFO] Starting installation...
[2026-04-21 10:15:57] [INFO] Downloading OfficeDeploymentTool.exe (150 MB)...
[2026-04-21 10:25:30] [INFO] Download completed (150 MB)
[2026-04-21 10:25:31] [INFO] Validating hash...
[2026-04-21 10:25:32] [SUCCESS] Hash validated
[2026-04-21 10:25:33] [INFO] Extracting setup.exe...
[2026-04-21 10:25:35] [INFO] Generating configuration.xml...
[2026-04-21 10:25:36] [INFO] Executing: setup.exe /configure...
[2026-04-21 10:25:37] [INFO] Installation in progress... (monitoring)
[2026-04-21 10:45:00] [SUCCESS] Installation completed successfully
[2026-04-21 10:45:01] [INFO] Cleaning temporary files...
[2026-04-21 10:45:02] [SUCCESS] OfficeAutomator completed

Log file location: C:\Users\Admin\AppData\Local\Temp\OfficeAutomator_20260421_101530.log
```

### 6.2 Logging Implementation

```powershell
# In OfficeAutomator.Logging.Handler.ps1

function Write-LogEntry {
    param(
        [string]$Message,
        [string]$Level = "INFO",  # INFO, SUCCESS, WARNING, ERROR
        [string]$LogPath
    )
    
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    $logMessage = "[$timestamp] [$Level] $Message"
    
    # Write to file
    Add-Content -Path $LogPath -Value $logMessage
    
    # Write to console with color
    $color = switch ($Level) {
        "INFO"    { "Cyan" }
        "SUCCESS" { "Green" }
        "WARNING" { "Yellow" }
        "ERROR"   { "Red" }
        default   { "White" }
    }
    Write-Host $logMessage -ForegroundColor $color
}
```

---

## 7. Testing Architecture

### 7.1 Unit Tests (PowerShell Functions)

```
tests/PowerShell/OfficeAutomator.PowerShell.Integration.Tests.ps1

Describe "OfficeAutomator.Menu.Display" {
  Context "Show-Menu function" {
    It "displays menu with options" {
      # Mock Read-Host to return "1"
      # Verify output contains menu items
    }
    
    It "validates numeric input (1-3)" {
      # Mock Read-Host to return "5"
      # Verify it loops and asks again
    }
  }
}

Describe "OfficeAutomator.CoreDll.Loader" {
  Context "Load-OfficeAutomatorCoreDll" {
    It "loads DLL successfully" {
      $result = Load-OfficeAutomatorCoreDll -DllPath $testDllPath
      $result | Should -Be $true
    }
    
    It "throws error if DLL not found" {
      { Load-OfficeAutomatorCoreDll -DllPath "invalid/path" } | Should -Throw
    }
  }
}

Describe "OfficeAutomator.Logging.Handler" {
  Context "Write-LogEntry" {
    It "writes to log file" {
      Write-LogEntry -Message "Test" -Level "INFO" -LogPath $testLog
      Get-Content $testLog | Should -Match "Test"
    }
  }
}
```

### 7.2 End-to-End Tests (Full Flow)

```
tests/PowerShell/OfficeAutomator.PowerShell.EndToEnd.Tests.ps1

Describe "OfficeAutomator Complete Flow" {
  Context "Happy Path (UC-001 → UC-005 Success)" {
    It "completes full installation without errors" {
      # Mock all user inputs
      # Mock OfficeDeploymentTool.exe download
      # Mock setup.exe execution
      # Verify installation completed
    }
  }
  
  Context "Error Path (Installation Failure)" {
    It "executes rollback when setup.exe fails" {
      # Mock UC-004 validation passed
      # Mock UC-005 installation fails
      # Verify RollbackExecutor.Execute() called
      # Verify system in clean state
    }
  }
}
```

---

## 8. Folder Structure Design

### 8.1 Complete Directory Layout

```
OfficeAutomator/
├── scripts/                                    [POWERSHELL LAYER]
│   ├── OfficeAutomator.PowerShell.Script.ps1
│   │   └─ Main entry point, 250-300 lines
│   │
│   └── functions/
│       ├─ OfficeAutomator.Menu.Display.ps1
│       ├─ OfficeAutomator.Validation.Environment.ps1
│       ├─ OfficeAutomator.CoreDll.Loader.ps1
│       ├─ OfficeAutomator.Logging.Handler.ps1
│       ├─ OfficeAutomator.Execution.Orchestration.ps1
│       └─ OfficeAutomator.Execution.RollbackHandler.ps1
│
├── src/                                        [C# LAYER - EXISTS]
│   └── OfficeAutomator.Core/
│       ├── bin/Release/net8.0/
│       │   └── OfficeAutomator.Core.dll
│       └── [existing source files]
│
├── tests/                                      [TESTING LAYER]
│   ├── OfficeAutomator.Core.Tests/            [C# TESTS]
│   │   └── [existing test files - 220+ tests]
│   │
│   └── PowerShell/                            [POWERSHELL TESTS]
│       ├─ OfficeAutomator.PowerShell.Integration.Tests.ps1
│       ├─ OfficeAutomator.PowerShell.EndToEnd.Tests.ps1
│       └─ pester.configuration.psd1
│
├── docs/                                       [DOCUMENTATION]
│   ├─ OPTION_B_ARCHITECTURE.md
│   ├─ UC_001_VERSION_SELECTION_POWERSHELL_FLOW.md
│   ├─ UC_002_LANGUAGE_SELECTION_POWERSHELL_FLOW.md
│   ├─ UC_003_APP_EXCLUSION_POWERSHELL_FLOW.md
│   ├─ UC_004_VALIDATION_POWERSHELL_FLOW.md
│   ├─ UC_005_INSTALLATION_ROLLBACK_POWERSHELL_FLOW.md
│   ├─ POWERSHELL_INTEGRATION_GUIDE.md
│   ├─ MIGRATION_FROM_MSOI_TO_OFFICAUTOMATOR.md
│   └─ [existing docs]
│
├── dist/                                       [DISTRIBUTION]
│   ├─ OfficeAutomator.PowerShell.zip
│   ├─ INSTALL_GUIDE.md
│   └─ README.md
│
├── README.md                                   [ROOT]
├── .gitignore                                  [GIT CONFIG]
└── [other project files]
```

---

## 9. Module Responsibility Matrix

| Module | Responsibility | Lines | Dependencies |
|--------|-----------------|-------|--------------|
| OfficeAutomator.PowerShell.Script.ps1 | Orchestration, user interaction | 250-300 | All functions |
| OfficeAutomator.Menu.Display.ps1 | Menu UI, input validation | 100 | Logging |
| OfficeAutomator.Validation.Environment.ps1 | Prerequisite checks | 80 | - |
| OfficeAutomator.CoreDll.Loader.ps1 | DLL loading | 50 | Logging |
| OfficeAutomator.Logging.Handler.ps1 | Centralized logging | 40 | - |
| OfficeAutomator.Execution.Orchestration.ps1 | UC execution | 150 | All others |
| OfficeAutomator.Execution.RollbackHandler.ps1 | Rollback execution | 80 | Core.dll |

**Total PowerShell Code:** ~700 líneas

---

## 10. Design Patterns Used

### 10.1 Patterns Applied

```
1. FACADE PATTERN
   └─ OfficeAutomator.PowerShell.Script.ps1 acts as facade
      to simplify interaction with complex C# layer

2. DELEGATION PATTERN
   └─ PowerShell delegates business logic to C# DLL
      
3. ORCHESTRATION PATTERN
   └─ Main script orchestrates flow through 5 UC
   
4. DEPENDENCY INJECTION PATTERN (Implicit)
   └─ Configuration object passed between components
   
5. STRATEGY PATTERN (Implicit in C#)
   └─ Different validation strategies per UC
   
6. CHAIN OF RESPONSIBILITY
   └─ 8-step validation chain in UC-004
```

### 10.2 Anti-Patterns Avoided

```
✗ NOT: Monolithic 1000+ line script
✓ INSTEAD: Modular functions with SRP

✗ NOT: Duplicate C# logic in PowerShell
✓ INSTEAD: Call C# from PowerShell

✗ NOT: No error handling
✓ INSTEAD: Try-catch at every integration point

✗ NOT: No logging
✓ INSTEAD: Detailed logging at every step

✗ NOT: No tests
✓ INSTEAD: 90%+ coverage with Pester + xUnit
```

---

## 11. Transition from Design to Implementation

### 11.1 Implementation Checklist

```
Phase 3 (DESIGN) → Phase 4 (IMPLEMENTATION)

BEFORE STARTING IMPLEMENTATION, VERIFY:

Architecture
  [ ] System architecture diagram approved
  [ ] Component responsibilities clear
  [ ] C# → PowerShell integration points identified
  [ ] Error handling strategy understood

Folder Structure
  [ ] Directory layout created
  [ ] File naming conventions approved
  [ ] Path references resolved

Dependencies
  [ ] Core.dll version confirmed (bin/Release/net8.0/)
  [ ] PowerShell version target confirmed (5.1+)
  [ ] .NET Runtime requirement documented (8.0+)

Testing Strategy
  [ ] Pester setup plan reviewed
  [ ] Mock strategy for C# objects planned
  [ ] Integration test scenarios documented

Documentation
  [ ] All UC flow documents exist (UC_001 to UC_005)
  [ ] Integration guide written
  [ ] Error code mapping complete
```

---

## 12. Next Steps - Ready for Implementation

### 12.1 Tollgate Criteria

Before proceeding to Phase 4 (IMPLEMENTATION):

**Architecture Review:**
- [ ] Component diagram approved
- [ ] Responsibility matrix validated
- [ ] PowerShell-to-C# interfaces clear
- [ ] Error handling design accepted

**Design Documentation:**
- [ ] Detailed flow diagrams complete
- [ ] Logging architecture specified
- [ ] Testing architecture planned
- [ ] Folder structure finalized

**Risk Assessment:**
- [ ] DLL loading in PowerShell verified feasible
- [ ] Exception handling strategy validated
- [ ] Performance implications reviewed
- [ ] Rollback atomicity guaranteed

---

**Status:** Phase 3 DESIGN Complete - Architecture Fully Specified  
**Ready for:** Phase 4 IMPLEMENTATION (Code Creation)  
**Prepared by:** Claude  
**Date:** 2026-04-21

