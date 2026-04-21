```yaml
phase: Phase 4 IMPLEMENTATION
wp: 2026-04-21-14-33-00-option-b-dependency-analysis-plan
methodology: Dependency Analysis + TDD Planning
date_created: 2026-04-21
status: IN_PROGRESS
depends_on:
  - 2026-04-21-14-30-00-option-b-powershell-wrapper-analysis
  - 2026-04-21-14-31-00-option-b-requirements-specification
  - 2026-04-21-14-32-00-option-b-detailed-design
```

# Phase 4: Dependency Analysis + TDD Implementation Plan

## 1. Complete Dependency Graph

### 1.1 All Components (9 files)

```
POWERSHELL SCRIPTS (7):
  1. OfficeAutomator.CoreDll.Loader.ps1 (50 líneas)
  2. OfficeAutomator.Validation.Environment.ps1 (80 líneas)
  3. OfficeAutomator.Logging.Handler.ps1 (40 líneas)
  4. OfficeAutomator.Menu.Display.ps1 (100 líneas)
  5. OfficeAutomator.Execution.Orchestration.ps1 (150 líneas)
  6. OfficeAutomator.Execution.RollbackHandler.ps1 (80 líneas)
  7. OfficeAutomator.PowerShell.Script.ps1 (250-300 líneas)

TESTS (2):
  8. OfficeAutomator.PowerShell.Integration.Tests.ps1 (200 líneas)
  9. OfficeAutomator.PowerShell.EndToEnd.Tests.ps1 (200 líneas)
```

### 1.2 Dependency Map

```
┌─────────────────────────────────────────────────────────────┐
│                 7. MAIN SCRIPT                              │
│        OfficeAutomator.PowerShell.Script.ps1                │
│         (Entry point, orchestración completa)               │
└──────────────┬──────────────────────────────────────────────┘
               │ Depende de (imports):
       ┌───────┼───────┬───────────┬──────────────┬───────────┐
       │       │       │           │              │           │
       ▼       ▼       ▼           ▼              ▼           ▼
   ┌────┐ ┌────┐ ┌──────┐ ┌────────────┐ ┌────────┐ ┌────────┐
   │ 2  │ │ 3  │ │ 4    │ │ 5          │ │ 6      │ │ 1      │
   │ Val│ │Logg│ │Menu  │ │Orchestr.   │ │Rollbck │ │Loader  │
   │ Env│ │ing │ │Disp  │ │           │ │        │ │        │
   └──┬─┘ └──┬─┘ └──┬───┘ └─────┬──────┘ └───┬────┘ └────┬───┘
      │      │      │           │            │           │
      │      │      │           │    ┌───────┴───────┐   │
      │      │      │           │    │               │   │
      │      ├──────┴───────────┤    │               │   │
      │      │                  │    │               │   │
      ▼      ▼                  ▼    ▼               ▼   ▼
    ┌──────────────┐      ┌─────────────┐       ┌──────────┐
    │   CORE.DLL   │      │  C# Classes │       │  Logger  │
    │ (VersionSel) │      │  (UC-001-05)│       │ Instance │
    │ (LangSel)    │      │             │       │          │
    │ (AppExcl)    │      │ (Validator) │       │          │
    │ (Config Val) │      │ (Executor)  │       │          │
    │ (Install)    │      │ (Rollback)  │       │          │
    │ (Rollback)   │      │             │       │          │
    └──────────────┘      └─────────────┘       └──────────┘
```

---

## 2. Dependency Matrix (Detallado)

```
┌─────────────────────────────────────────────────────────────────────────┐
│ SCRIPT # │ NOMBRE                      │ DEPENDE DE        │ LÍNEAS │ LV │
├─────────────────────────────────────────────────────────────────────────┤
│ 1        │ CoreDll.Loader              │ (none)            │   50   │ L0 │
│          │ Load-OfficeAutomatorCoreDll │ [Pure function]   │        │    │
├─────────────────────────────────────────────────────────────────────────┤
│ 2        │ Validation.Environment      │ (none)            │   80   │ L0 │
│          │ Test-AdminRights            │ [Pure function]   │        │    │
│          │ Test-CoreDllExists          │                   │        │    │
│          │ Test-DotNetRuntime          │                   │        │    │
├─────────────────────────────────────────────────────────────────────────┤
│ 3        │ Logging.Handler             │ (none)            │   40   │ L0 │
│          │ Write-LogEntry              │ [Pure function]   │        │    │
│          │                             │ + file I/O        │        │    │
├─────────────────────────────────────────────────────────────────────────┤
│ 4        │ Menu.Display                │ 3 (Logging)       │  100   │ L1 │
│          │ Show-Menu                   │ [Calls Write-Log]  │        │    │
├─────────────────────────────────────────────────────────────────────────┤
│ 5        │ Execution.Orchestration     │ 1, 2, 3, 4        │  150   │ L2 │
│          │ Invoke-OfficeInstallation   │ [Calls all above]  │        │    │
│          │ Invoke-ValidationStep       │ + C# objects      │        │    │
│          │ Invoke-InstallationStep     │                   │        │    │
│          │ Show-ProgressBar            │                   │        │    │
├─────────────────────────────────────────────────────────────────────────┤
│ 6        │ Execution.RollbackHandler   │ 3, 5              │   80   │ L2 │
│          │ Invoke-RollbackOnFailure    │ [Calls Logging]   │        │    │
│          │                             │ [Calls Orchestr.]  │        │    │
├─────────────────────────────────────────────────────────────────────────┤
│ 7        │ PowerShell.Script (MAIN)    │ 1, 2, 3, 4, 5, 6  │  250-  │ L3 │
│          │ Main orchestration          │ [Imports all]      │  300   │    │
│          │ Flow control (UC-001→005)   │ + C# objects      │        │    │
├─────────────────────────────────────────────────────────────────────────┤
│ 8        │ PowerShell.Integration.Tests│ 1, 2, 3, 4, 5, 6  │  200   │ L2 │
│          │ Unit tests (Pester)         │ [Tests above]      │        │    │
│          │ Integration w/ C#           │ + mocking          │        │    │
├─────────────────────────────────────────────────────────────────────────┤
│ 9        │ PowerShell.EndToEnd.Tests   │ 7 (Main)          │  200   │ L3 │
│          │ Full flow tests (Pester)    │ [Tests complete]   │        │    │
│          │ UC-001 → UC-005 simulation  │ flow + mocking     │        │    │
└─────────────────────────────────────────────────────────────────────────┘

LEGEND:
  LV = Level (dependency depth)
  L0 = No dependencies (foundational)
  L1 = Depends on L0
  L2 = Depends on L1
  L3 = Depends on L2
```

---

## 3. Topological Sort - Optimal Creation Order

### 3.1 Level-Based Creation Order (RECOMMENDED)

```
LEVEL 0 (Foundational - No dependencies):
  ├─ Script 1: CoreDll.Loader.ps1 ⭐ CREATE FIRST
  ├─ Script 2: Validation.Environment.ps1
  └─ Script 3: Logging.Handler.ps1

LEVEL 1 (Depends on L0):
  └─ Script 4: Menu.Display.ps1

LEVEL 2 (Depends on L0-L1):
  ├─ Script 5: Execution.Orchestration.ps1
  ├─ Script 6: Execution.RollbackHandler.ps1
  └─ Test 8: PowerShell.Integration.Tests.ps1

LEVEL 3 (Depends on L2):
  ├─ Script 7: PowerShell.Script.ps1 (MAIN)
  └─ Test 9: PowerShell.EndToEnd.Tests.ps1
```

### 3.2 Creation Sequence with TDD

```
PHASE A: FOUNDATIONAL LAYER (L0 - Day 1)
─────────────────────────────────────────

1. Script 1: CoreDll.Loader
   a) Write tests: OfficeAutomator.PowerShell.Integration.Tests.ps1 (test suite 1)
      └─ Test: "Load-OfficeAutomatorCoreDll loads DLL successfully"
      └─ Test: "Load-OfficeAutomatorCoreDll throws if DLL not found"
      └─ Test: "Load-OfficeAutomatorCoreDll validates classes exist"
   
   b) Write code: OfficeAutomator.CoreDll.Loader.ps1
      └─ Function: Load-OfficeAutomatorCoreDll
      └─ Uses: [System.Reflection.Assembly]::LoadFrom()
   
   c) Run tests: pester tests/PowerShell/OfficeAutomator.PowerShell.Integration.Tests.ps1

2. Script 2: Validation.Environment
   a) Write tests: (add to Integration.Tests.ps1)
      └─ Test: "Test-AdminRights returns true if admin"
      └─ Test: "Test-AdminRights returns false if not admin"
      └─ Test: "Test-CoreDllExists returns true if DLL in path"
      └─ Test: "Test-DotNetRuntime detects .NET 8.0+"
   
   b) Write code: OfficeAutomator.Validation.Environment.ps1
      └─ Function: Test-AdminRights
      └─ Function: Test-CoreDllExists
      └─ Function: Test-DotNetRuntime
      └─ Function: Test-PrerequisitesMet (combines all)
   
   c) Run tests

3. Script 3: Logging.Handler
   a) Write tests: (add to Integration.Tests.ps1)
      └─ Test: "Write-LogEntry writes to file"
      └─ Test: "Write-LogEntry includes timestamp"
      └─ Test: "Write-LogEntry colors match level"
   
   b) Write code: OfficeAutomator.Logging.Handler.ps1
      └─ Function: Write-LogEntry
      └─ Params: -Message, -Level, -LogPath
      └─ Outputs: file + console
   
   c) Run tests

PHASE B: SUPPORT LAYER (L1 - Day 2)
───────────────────────────────────

4. Script 4: Menu.Display
   a) Write tests: (add to Integration.Tests.ps1)
      └─ Test: "Show-Menu displays title"
      └─ Test: "Show-Menu displays all options"
      └─ Test: "Show-Menu validates numeric input"
      └─ Test: "Show-Menu loops on invalid input"
   
   b) Write code: OfficeAutomator.Menu.Display.ps1
      └─ Function: Show-Menu
      └─ Params: -Title, -Options
      └─ Returns: [int] selected option
      └─ Validation: 1 to array length
   
   c) Run tests

PHASE C: EXECUTION LAYER (L2 - Day 3)
─────────────────────────────────────

5. Script 5: Execution.Orchestration
   a) Write tests: (add to Integration.Tests.ps1)
      └─ Test: "Invoke-OfficeInstallation calls VersionSelector"
      └─ Test: "Invoke-OfficeInstallation calls LanguageSelector"
      └─ Test: "Invoke-OfficeInstallation calls AppExclusionSelector"
      └─ Test: "Invoke-OfficeInstallation calls ConfigValidator"
      └─ Test: "Invoke-OfficeInstallation calls InstallationExecutor"
      └─ Test: "Invoke-OfficeInstallation handles C# exceptions"
      └─ Test: "Show-ProgressBar updates display"
   
   b) Write code: OfficeAutomator.Execution.Orchestration.ps1
      └─ Function: Invoke-OfficeInstallation
         ├─ Calls C# VersionSelector.Execute()
         ├─ Calls C# LanguageSelector.Execute()
         ├─ Calls C# AppExclusionSelector.Execute()
         ├─ Calls C# ConfigValidator.Execute()
         ├─ Calls C# InstallationExecutor.Execute()
         └─ Returns: [bool]
      
      └─ Function: Invoke-ValidationStep
         └─ Wraps ConfigValidator call
      
      └─ Function: Invoke-InstallationStep
         └─ Wraps InstallationExecutor call
      
      └─ Function: Show-ProgressBar
         └─ Displays progress indicator
   
   c) Run tests

6. Script 6: Execution.RollbackHandler
   a) Write tests: (add to Integration.Tests.ps1)
      └─ Test: "Invoke-RollbackOnFailure calls RollbackExecutor"
      └─ Test: "Invoke-RollbackOnFailure logs rollback steps"
      └─ Test: "Invoke-RollbackOnFailure handles rollback errors"
   
   b) Write code: OfficeAutomator.Execution.RollbackHandler.ps1
      └─ Function: Invoke-RollbackOnFailure
         ├─ Calls C# RollbackExecutor.Execute()
         ├─ Logs all rollback actions
         └─ Returns: [bool]
   
   c) Run tests

PHASE D: MAIN SCRIPT (L3 - Day 4)
────────────────────────────────

7. Script 7: PowerShell.Script (MAIN)
   a) Write tests: OfficeAutomator.PowerShell.EndToEnd.Tests.ps1
      └─ Test: "Full flow: UC-001 → UC-005 (happy path)"
      └─ Test: "Full flow: Invalid input loops correctly"
      └─ Test: "Full flow: Installation failure triggers rollback"
      └─ Test: "Full flow: Log file created with all actions"
   
   b) Write code: OfficeAutomator.PowerShell.Script.ps1
      ├─ Phase 1: Load prerequisites
      ├─ Phase 2: Load DLL
      ├─ Phase 3: UC-001 Version selection
      ├─ Phase 4: UC-002 Language selection
      ├─ Phase 5: UC-003 App exclusion
      ├─ Phase 6: Summary & confirmation
      ├─ Phase 7: UC-004 Validation
      ├─ Phase 8: UC-005 Installation
      └─ Phase 9: Cleanup & log
   
   c) Run tests

PHASE E: FINAL TESTS (L3 - Day 5)
────────────────────────────────

8. OfficeAutomator.PowerShell.Integration.Tests.ps1 (FINALIZE)
   └─ Run all L0, L1, L2 tests
   └─ Verify 90%+ coverage
   └─ Fix any gaps

9. OfficeAutomator.PowerShell.EndToEnd.Tests.ps1 (FINALIZE)
   └─ Run full flow tests
   └─ Verify all UC flows work
   └─ Verify error paths work

10. Code Coverage Report
    └─ Generate Pester coverage report
    └─ Target: >90% coverage
    └─ Document any exclusions
```

---

## 4. TDD Implementation Pattern

### 4.1 TDD Workflow (For Each Script)

```
STEP 1: Write Tests FIRST
─────────────────────────

File: tests/PowerShell/OfficeAutomator.PowerShell.Integration.Tests.ps1

Describe "OfficeAutomator.CoreDll.Loader" {
  Context "Load-OfficeAutomatorCoreDll function" {
    It "loads DLL successfully when file exists" {
      # ARRANGE
      $dllPath = ".\src\OfficeAutomator.Core\bin\Release\net8.0\OfficeAutomator.Core.dll"
      
      # ACT
      $result = Load-OfficeAutomatorCoreDll -DllPath $dllPath
      
      # ASSERT
      $result | Should -Be $true
    }
    
    It "throws error when DLL file not found" {
      # ARRANGE
      $invalidPath = ".\nonexistent\path\fake.dll"
      
      # ACT & ASSERT
      { Load-OfficeAutomatorCoreDll -DllPath $invalidPath } | Should -Throw
    }
    
    It "verifies OfficeAutomator.Core.Models.Configuration class exists" {
      # ARRANGE
      $dllPath = ".\src\OfficeAutomator.Core\bin\Release\net8.0\OfficeAutomator.Core.dll"
      Load-OfficeAutomatorCoreDll -DllPath $dllPath
      
      # ACT & ASSERT
      { [OfficeAutomator.Core.Models.Configuration] } | Should -Not -Throw
    }
  }
}

STEP 2: Run Tests (They FAIL - RED)
──────────────────────────────────

PS> Invoke-Pester tests/PowerShell/OfficeAutomator.PowerShell.Integration.Tests.ps1

Results:
  [-] loads DLL successfully when file exists (FAIL - function doesn't exist yet)
  [-] throws error when DLL file not found (FAIL - function doesn't exist yet)
  [-] verifies classes exist (FAIL - function doesn't exist yet)

STEP 3: Write Minimal Code to PASS Tests (GREEN)
─────────────────────────────────────────────────

File: scripts/functions/OfficeAutomator.CoreDll.Loader.ps1

function Load-OfficeAutomatorCoreDll {
    param(
        [Parameter(Mandatory = $true)]
        [string]$DllPath
    )
    
    try {
        # Load DLL
        [System.Reflection.Assembly]::LoadFrom($DllPath) | Out-Null
        
        # Verify key classes exist
        $null = [OfficeAutomator.Core.Models.Configuration]
        $null = [OfficeAutomator.Core.Validation.ConfigValidator]
        
        Write-Host "✓ DLL loaded successfully" -ForegroundColor Green
        return $true
    }
    catch {
        Write-Host "ERROR: Failed to load DLL - $($_.Exception.Message)" -ForegroundColor Red
        throw $_
    }
}

STEP 4: Run Tests Again (They PASS - GREEN)
──────────────────────────────────────────

PS> Invoke-Pester tests/PowerShell/OfficeAutomator.PowerShell.Integration.Tests.ps1

Results:
  [✓] loads DLL successfully when file exists (PASS)
  [✓] throws error when DLL file not found (PASS)
  [✓] verifies classes exist (PASS)

STEP 5: REFACTOR (BLUE - if needed)
───────────────────────────────────

# If code is messy or has duplication, clean it up while tests pass
# Tests prevent regressions during refactoring

STEP 6: ADD MORE TESTS
─────────────────────

# Once basic tests pass, add edge cases, error scenarios, etc.
# Keep adding tests → write code to pass → refactor cycle
```

---

## 5. Documentation Standard (For Each Script)

### 5.1 Script Documentation Template

```powershell
<#
.SYNOPSIS
    [One-line description of what the script does]

.DESCRIPTION
    [Detailed description of functionality, use cases, and context]
    
    This function is part of OPTION B PowerShell wrapper layer.
    Responsible for: [main responsibility]
    
    UC MAPPING: UC-001 / UC-002 / etc. (if applicable)

.PARAMETER ParameterName
    Description of what this parameter does, valid values, etc.

.EXAMPLE
    PS> Load-OfficeAutomatorCoreDll -DllPath ".\OfficeAutomator.Core.dll"
    ✓ DLL loaded successfully
    
    True

.EXAMPLE
    PS> Load-OfficeAutomatorCoreDll -DllPath "invalid.dll"
    ERROR: Failed to load DLL - File not found
    
    (Exception thrown)

.INPUTS
    [string] DLL file path

.OUTPUTS
    [bool] $true if successful, throws on error

.NOTES
    Author: Claude (AI Assistant)
    Date: 2026-04-21
    Version: 1.0
    
    DEPENDENCIES:
      - System.Reflection.Assembly (built-in)
      - OfficeAutomator.Core.dll
    
    ERROR CODES:
      - 2001: DLL not found
      - 2002: DLL loading failed (invalid .NET assembly)
      - 2003: Required classes not found in DLL
    
    RELATED SCRIPTS:
      - OfficeAutomator.Validation.Environment.ps1
      - OfficeAutomator.PowerShell.Script.ps1

.LINK
    https://github.com/your-repo/OfficeAutomator
#>

param(
    [Parameter(Mandatory = $true, HelpMessage = "Path to OfficeAutomator.Core.dll")]
    [ValidateScript({ Test-Path $_ -PathType Leaf })]
    [string]$DllPath
)

# Function code here...
```

### 5.2 Code Comments Standard

```powershell
# Use comments for WHY, not WHAT (code shows WHAT)

# ✓ GOOD:
# Retry up to 3 times because network timeouts are transient
for ($i = 0; $i -lt 3; $i++) {
    try {
        $result = Invoke-WebRequest -Uri $url
        break
    }
    catch {
        # Network error, try again
    }
}

# ✗ BAD:
# Loop 3 times
for ($i = 0; $i -lt 3; $i++) {
    # Try web request
    try {
        $result = Invoke-WebRequest -Uri $url
        # Break if successful
        break
    }
    catch {
        # Catch error
    }
}
```

### 5.3 Function Documentation

```powershell
function Show-Menu {
    <#
    .SYNOPSIS
        Display interactive menu and capture user selection
    
    .DESCRIPTION
        Shows numbered menu with validation, loops on invalid input
        until user selects valid option (1 to option count)
    
    .PARAMETER Title
        Menu header text displayed to user
    
    .PARAMETER Options
        Array of option strings (1-based indexing)
    
    .EXAMPLE
        PS> $choice = Show-Menu -Title "Select Version" -Options @("2024", "2021", "2019")
        Seleccione versión de Office
        1. 2024
        2. 2021
        3. 2019
        
        Seleccione una opción: 1
        
        $choice = 1
    
    .OUTPUTS
        [int] 1-based selected option index
    
    .NOTES
        DEPENDENCIES:
          - Write-LogEntry (from Logging.Handler)
        
        VALIDATION:
          - Input must be numeric 1-N
          - Invalid input loops with error message
          - Handles Ctrl+C gracefully
    #>
    param(
        [Parameter(Mandatory = $true)]
        [string]$Title,
        
        [Parameter(Mandatory = $true)]
        [string[]]$Options
    )
    
    # Function code...
}
```

---

## 6. Complete Implementation Plan

### 6.1 Day-by-Day Schedule

```
MONDAY (Day 1 - Foundational Layer)
────────────────────────────────────
08:00 - 09:00: Setup
  - Create scripts/ and scripts/functions/ folders
  - Create tests/PowerShell/ folder
  - Setup Pester configuration

09:00 - 12:00: Script 1 (CoreDll.Loader) - TDD
  - Write tests (30 min)
  - Write code (30 min)
  - Run & fix tests (30 min)
  - Add edge case tests (30 min)
  - COMMIT: "FEAT: Add CoreDll.Loader with tests"

12:00 - 13:00: Lunch

13:00 - 16:00: Script 2 (Validation.Environment) - TDD
  - Write tests (30 min)
  - Write code (60 min)
  - Run & fix tests (30 min)
  - COMMIT: "FEAT: Add Validation.Environment with tests"

16:00 - 17:00: Script 3 (Logging.Handler) - TDD
  - Write tests (20 min)
  - Write code (30 min)
  - Run & fix tests (10 min)
  - COMMIT: "FEAT: Add Logging.Handler with tests"

TUESDAY (Day 2 - Support Layer)
───────────────────────────────
08:00 - 12:00: Script 4 (Menu.Display) - TDD
  - Write tests (30 min)
  - Write code (60 min)
  - Run & fix tests (30 min)
  - COMMIT: "FEAT: Add Menu.Display with tests"

12:00 - 13:00: Lunch

13:00 - 17:00: Script 5 (Execution.Orchestration) - TDD
  - Write tests (45 min)
  - Write code (90 min)
  - Run & fix tests (45 min)
  - COMMIT: "FEAT: Add Execution.Orchestration with tests"

WEDNESDAY (Day 3 - Execution Layer)
────────────────────────────────────
08:00 - 12:00: Script 6 (Execution.RollbackHandler) - TDD
  - Write tests (30 min)
  - Write code (60 min)
  - Run & fix tests (30 min)
  - COMMIT: "FEAT: Add Execution.RollbackHandler with tests"

12:00 - 13:00: Lunch

13:00 - 17:00: Integration Tests Review
  - Review all Integration.Tests.ps1
  - Add missing edge cases
  - Verify 90%+ coverage
  - COMMIT: "TEST: Complete integration test suite"

THURSDAY (Day 4 - Main Script)
──────────────────────────────
08:00 - 12:00: Script 7 (PowerShell.Script MAIN) - Part 1
  - Write E2E tests (45 min)
  - Write main entry point structure (45 min)
  - Write UC-001-003 flow (30 min)

12:00 - 13:00: Lunch

13:00 - 17:00: Script 7 - Part 2
  - Write UC-004 validation flow (45 min)
  - Write UC-005 installation flow (45 min)
  - Run & fix tests (30 min)
  - COMMIT: "FEAT: Add main PowerShell.Script with tests"

FRIDAY (Day 5 - Testing & Polish)
──────────────────────────────────
08:00 - 12:00: E2E Testing
  - Run full Integration tests
  - Run full E2E tests
  - Fix any failures
  - Generate coverage report

12:00 - 13:00: Lunch

13:00 - 16:00: Documentation
  - Review all script headers
  - Add missing documentation
  - Create README for scripts/
  - Create TESTING.md for test running

16:00 - 17:00: Final Review
  - Review all commits
  - Verify folder structure
  - Final test run
  - COMMIT: "DOCS: Complete script documentation"
  - PREPARE: Branch merge to master
```

### 6.2 Testing Checklist

```
UNIT TESTS (Integration.Tests.ps1):
  ✓ Load-OfficeAutomatorCoreDll (5 tests)
  ✓ Test-AdminRights (3 tests)
  ✓ Test-CoreDllExists (2 tests)
  ✓ Test-DotNetRuntime (2 tests)
  ✓ Write-LogEntry (4 tests)
  ✓ Show-Menu (5 tests)
  ✓ Invoke-OfficeInstallation (7 tests)
  ✓ Invoke-RollbackOnFailure (3 tests)
  └─ TOTAL: ~30 unit tests

E2E TESTS (EndToEnd.Tests.ps1):
  ✓ Happy path: UC-001 → UC-005
  ✓ Error path: Invalid input
  ✓ Error path: Installation failure
  ✓ Error path: User cancellation
  ✓ Rollback atomic execution
  └─ TOTAL: ~8 E2E tests

CODE COVERAGE TARGET:
  ✓ Overall: >90%
  ✓ Scripts: >95%
  ✓ Functions: 100% (each function called)
  ✓ Error paths: 100% (each error tested)

INTEGRATION POINTS (to C#):
  ✓ [System.Reflection.Assembly]::LoadFrom() works
  ✓ C# object creation (New-Object) works
  ✓ C# method invocation works
  ✓ C# exceptions caught and handled
  ✓ Configuration object passed correctly
  ✓ Return values interpreted correctly
```

---

## 7. Commit Strategy

```
Each commit should:
  1. Add ONE script (or ONE test file)
  2. Include tests for that script
  3. Have descriptive message
  4. Pass all tests before committing

Format:
  FEAT: Add [Script Name] with tests
  
  - Function: [function names]
  - Lines: [number]
  - Tests: [number of tests]
  - Coverage: [%]
  
  UC Mapping: UC-001 / UC-002 / etc.
  Depends on: [previous scripts]

Example:
  FEAT: Add Menu.Display with tests
  
  - Function: Show-Menu
  - Lines: 100
  - Tests: 5
  - Coverage: 100%
  
  UC Mapping: UC-001, UC-002, UC-003
  Depends on: OfficeAutomator.Logging.Handler
```

---

## 8. Risk Mitigation

### 8.1 Potential Issues & Solutions

```
RISK 1: C# DLL not loading in PowerShell
  Mitigation:
    - Load-OfficeAutomatorCoreDll has comprehensive error handling
    - Tests verify DLL loading before other scripts use it
    - Fallback: detailed error message guides user

RISK 2: C# exceptions not caught properly
  Mitigation:
    - All C# calls wrapped in try-catch
    - Exception type-specific handling
    - Write-LogEntry logs all exceptions
    - Tests verify exception handling

RISK 3: Tests fail intermittently
  Mitigation:
    - No external dependencies (all mocked)
    - No timing-sensitive operations
    - Mocking handles all C# interactions
    - Deterministic test data

RISK 4: Performance degradation
  Mitigation:
    - DLL loaded once (not per function)
    - Minimal PowerShell operations
    - Direct delegation to C# for heavy lifting
    - No unnecessary loops or recursion

RISK 5: Rollback fails, leaving system dirty
  Mitigation:
    - Rollback is atomic (all-or-nothing)
    - Tests verify rollback completeness
    - If rollback fails, user is notified
    - Log contains detailed cleanup info
```

---

## 9. Quality Gates

### 9.1 Before Each Commit

```
MANDATORY CHECKS:
  [ ] All tests pass (pester)
  [ ] Code coverage >90%
  [ ] No PowerShell errors or warnings
  [ ] Documentation is complete
  [ ] Commit message is clear
  [ ] Dependencies resolved
  
MANUAL REVIEW:
  [ ] Code is readable (no cryptic logic)
  [ ] Comments explain WHY, not WHAT
  [ ] Error messages are user-friendly
  [ ] Variable names are clear
  [ ] Functions have <# .SYNOPSIS #> headers
```

### 9.2 End of Day Gates

```
DAILY STANDUP (EOD):
  [ ] All scripts created today pass tests
  [ ] Code coverage metric improved
  [ ] No blocked issues
  [ ] Documentation updated
  [ ] Commits are clean (no merge conflicts)
```

### 9.3 End of Week Gates

```
WEEK COMPLETE CRITERIA:
  [ ] All 7 scripts created
  [ ] All 2 test files complete
  [ ] Total >90% code coverage
  [ ] All tests green (100% pass rate)
  [ ] All documentation complete
  [ ] Ready for beta testing
```

---

## 10. Next Actions

### 10.1 IMMEDIATE (Today)

1. Create WP4 ✓ (this document)
2. Confirm order: **SCRIPT 1 (CoreDll.Loader) is FIRST**
3. Commit WP4
4. Begin Monday implementation

### 10.2 Start of Coding (Monday Morning)

1. Setup folder structure: `scripts/`, `scripts/functions/`, `tests/PowerShell/`
2. Create initial Pester configuration
3. Start TDD cycle: Write tests for Script 1 → Code → Pass tests

### 10.3 Key Milestones

```
END OF MONDAY:    Scripts 1, 2, 3 complete + tested
END OF TUESDAY:   Scripts 1-4 complete + tested
END OF WEDNESDAY: Scripts 1-6 complete + tested + integration tests reviewed
END OF THURSDAY:  All 7 scripts + 2 test files complete + tested
END OF FRIDAY:    >90% coverage, documentation complete, ready for beta
```

---

**Status:** Phase 4 PLAN Complete - Ready for Implementation  
**Next Step:** Create scripts/folders and begin Script 1 (CoreDll.Loader)  
**First Script:** OfficeAutomator.CoreDll.Loader.ps1 (50 líneas, L0 - No dependencies)  
**Methodology:** TDD (Tests First) + Professional Documentation  
**Prepared by:** Claude  
**Date:** 2026-04-21

