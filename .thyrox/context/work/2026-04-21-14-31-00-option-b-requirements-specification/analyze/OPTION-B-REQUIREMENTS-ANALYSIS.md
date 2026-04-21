```yaml
phase: Phase 2 ANALYZE
wp: 2026-04-21-14-31-00-option-b-requirements-specification
methodology: ba-requirements-analysis
date_created: 2026-04-21
status: IN_PROGRESS
depends_on: 2026-04-21-14-30-00-option-b-powershell-wrapper-analysis
```

# Phase 2: ANALYZE - Requirements Analysis & Use Case Specification

## 1. Executive Summary

This document specifies the requirements for OPTION B (PowerShell Wrapper) by modeling and specifying how the 5 existing Use Cases (UC-001 to UC-005) will be accessed and executed through a PowerShell frontend.

**Key Insight:** OPTION B does NOT create new Use Cases. Instead, it creates a **PowerShell presentation layer** that invokes the existing, tested UC-001 to UC-005 from OfficeAutomator.Core.dll.

---

## 2. Use Case Model - PowerShell Frontend Integration

### UC-001: Version Selection via PowerShell Menu

**Level:** User Goal (High)  
**Scope:** OfficeAutomator.PowerShell.Script.ps1 + OfficeAutomator.Menu.Display.ps1  
**Primary Actor:** Sysadmin  
**Stakeholders:** IT Manager, IT Support

#### Pre-conditions
- OfficeAutomator.PowerShell.Script.ps1 is running
- Prerequisite checks (admin, DLL, .NET) have passed
- OfficeAutomator.Core.dll is loaded in memory

#### Flow of Events

**Main Success Scenario:**
1. PowerShell script shows menu: "Seleccione versión de Office"
2. Menu displays options: "1. Office LTSC 2024", "2. Office LTSC 2021", "3. Office LTSC 2019"
3. Sysadmin enters number (1-3)
4. PowerShell validates input (must be 1, 2, or 3)
5. If valid: PowerShell calls `$versionSelector.Execute($config)` (VersionSelector.cs from C#)
6. VersionSelector updates configuration object
7. State machine transitions to next UC
8. Display success message: "✓ Version selected: Office LTSC 2024"
9. Continue to UC-002

**Alternative Flows:**
- **Alt 1a:** Sysadmin enters invalid number (e.g., "5")
  - PowerShell shows error: "Opción inválida. Por favor ingrese 1, 2 o 3"
  - Redisplay menu (loop until valid)

- **Alt 1b:** User presses Ctrl+C
  - PowerShell catches interrupt, shows: "Instalación cancelada"
  - Cleanup and exit gracefully

#### Post-conditions
- Configuration.Version is set (PerpetualVL2024, PerpetualVL2021, or PerpetualVL2019)
- State machine has transitioned to ready for UC-002
- Log entry written: "[timestamp] [INFO] Version selected: {version}"

#### Special Requirements
- Menu must be displayed in color (Cyan for prompts, Green for success)
- Input validation must be case-insensitive (accept "1" or "1 ")
- Must work with both PowerShell 5.1 and PowerShell 7.x

---

### UC-002: Language Selection via PowerShell Menu

**Level:** User Goal (High)  
**Scope:** OfficeAutomator.PowerShell.Script.ps1 + OfficeAutomator.Menu.Display.ps1  
**Primary Actor:** Sysadmin  
**Depends on:** UC-001 completed

#### Pre-conditions
- UC-001 completed successfully
- Configuration.Version is set
- LanguageSelector.cs is instantiated

#### Flow of Events

**Main Success Scenario:**
1. PowerShell shows menu: "Seleccione idioma de Office"
2. Menu displays: "1. Spanish (es-ES)", "2. English (en-US)", etc. (5 languages)
3. Sysadmin enters number (1-5)
4. PowerShell validates input
5. If valid: PowerShell calls `$languageSelector.Execute($config)` (LanguageSelector.cs)
6. LanguageSelector updates configuration
7. Display: "✓ Language selected: Spanish (es-ES)"
8. Continue to UC-003

#### Post-conditions
- Configuration.Language is set (es-ES, en-US, etc.)
- State machine ready for UC-003
- Log entry written

#### Special Requirements
- Support 5 languages minimum
- Must display language codes for clarity
- Validation: 1-5 only

---

### UC-003: App Exclusion Selection via PowerShell Menu

**Level:** User Goal (High)  
**Scope:** OfficeAutomator.PowerShell.Script.ps1 + OfficeAutomator.Menu.Display.ps1  
**Primary Actor:** Sysadmin  
**Depends on:** UC-002 completed

#### Pre-conditions
- UC-001 and UC-002 completed
- Configuration.Version and Configuration.Language are set
- AppExclusionSelector.cs is instantiated

#### Flow of Events

**Main Success Scenario:**
1. PowerShell shows menu: "Seleccione aplicaciones a excluir"
2. Menu displays 5 apps: "1. Teams", "2. OneDrive", "3. Groove", "4. Lync", "5. Bing"
3. Sysadmin enters: "1,2" (comma-separated) OR presses Enter for none
4. PowerShell parses and validates input (1-5 or empty)
5. If valid: PowerShell calls `$appSelector.Execute($config)`
6. AppExclusionSelector updates configuration with exclusion list
7. Display: "✓ Exclusions set: Teams, OneDrive"
8. Continue to UC-004

#### Alternative Flows
- **Alt 3a:** User presses Enter (no exclusions)
  - Configuration.ExcludedApps remains empty
  - Display: "✓ No exclusions set"

- **Alt 3b:** User enters invalid number (e.g., "1,2,10")
  - PowerShell shows error: "Número 10 inválido"
  - Redisplay menu

#### Post-conditions
- Configuration.ExcludedApps contains selected exclusions (or empty)
- State machine ready for UC-004
- Log entry written

#### Special Requirements
- Support comma-separated input (1,2,3)
- Support pressing Enter for no exclusions
- Validation: 1-5 or empty

---

### UC-004: Configuration Validation (No User Interaction)

**Level:** System Goal  
**Scope:** OfficeAutomator.Execution.Orchestration.ps1 calls ConfigValidator.cs  
**Primary Actor:** OfficeAutomator.Core.dll (automatic)  
**Depends on:** UC-001, UC-002, UC-003 completed

#### Pre-conditions
- UC-001 to UC-003 completed
- Configuration object fully populated
- Sysadmin has confirmed "¿Continuar con la instalación?" with "S"

#### Flow of Events

**Main Success Scenario (8 Validation Steps):**
1. PowerShell displays: "Validando configuración..."
2. PowerShell calls `$validator.Execute($config)` (ConfigValidator.cs)
3. ConfigValidator executes 8 internal validation steps:
   - Step 1: Version exists (PerpetualVL2024, 2021, 2019)
   - Step 2: Language is supported (5 languages)
   - Step 3: Disk space >= 5 GB available
   - Step 4: Network connectivity (ping Microsoft servers)
   - Step 5: Download integrity (mock hash validation)
   - Step 6: Admin privileges verified
   - Step 7: PowerShell >= 5.1
   - Step 8: setup.exe will be valid
4. PowerShell displays progress: "[████░░░░░░] 20% - Step 1/8..."
5. All 8 steps pass
6. ConfigValidator returns `true`
7. PowerShell displays: "✓ Validación completada"
8. Continue to UC-005

**Error Scenario:**
- If any step fails (e.g., Step 3 - Disk space < 5GB):
  - ConfigValidator returns `false` with ErrorCode (e.g., 003)
  - PowerShell displays: "ERROR 003: Espacio insuficiente en disco"
  - Execution stops, no installation attempted
  - Log entry written with error details
  - User can fix issue and retry

#### Post-conditions (Success)
- Configuration validated completely
- 8 validation checks passed
- ConfigValidator.Validate() returns true
- State machine ready for UC-005

#### Post-conditions (Failure)
- Specific error code generated (001-008)
- Installation blocked (safety gate)
- User guided to fix issue
- System in safe state (no partial changes)

#### Special Requirements
- All 8 steps must execute in sequence
- If any step fails: STOP immediately (no partial validation)
- Provide clear error messages with actionable guidance
- Log all 8 steps regardless of pass/fail

---

### UC-005: Office Installation with Automatic Rollback

**Level:** System Goal  
**Scope:** OfficeAutomator.Execution.Orchestration.ps1 calls InstallationExecutor & RollbackExecutor  
**Primary Actor:** OfficeAutomator.Core.dll (automatic)  
**Depends on:** UC-004 validation passed

#### Pre-conditions
- UC-004 validation passed (all 8 steps OK)
- Configuration fully validated
- System ready for installation

#### Flow of Events

**Main Success Scenario:**
1. PowerShell displays: "[████░░░░░░] 10% - Iniciando instalación..."
2. PowerShell calls `$executor.Execute($config)` (InstallationExecutor.cs)
3. InstallationExecutor:
   - Creates temp folder: %TEMP%\ODT\
   - Downloads OfficeDeploymentTool.exe (150 MB)
     - Timeout: 300 seconds
     - Retry: 3 attempts if timeout
   - Validates hash SHA256
   - Extracts: OfficeDeploymentTool.exe /quiet /extract
   - Generates configuration.xml from $config
   - Executes: setup.exe /configure %TEMP%\ODT\configuration.xml
   - Monitors progress in real-time
4. PowerShell shows progress bars:
   - "[████░░░░░░] 30% - Downloading Office..."
   - "[████████░░] 50% - Installing Office..."
   - "[████████████████░░] 85% - Finalizing..."
5. setup.exe completes successfully (exit code 0)
6. InstallationExecutor returns `true`
7. PowerShell displays: "[██████████] 100% - ¡Instalación completada!"
8. UC-005 SUCCESS

**Error Scenario - Automatic Rollback:**
1. setup.exe fails (exit code != 0)
2. InstallationExecutor returns `false`
3. PowerShell detects failure
4. PowerShell displays: "Instalación fallida, ejecutando rollback..."
5. PowerShell calls `$rollback.Execute()` (RollbackExecutor.cs)
6. RollbackExecutor executes 4-part rollback (atomic):
   - Part 1: Delete Office installation files
   - Part 2: Clean registry (HKLM\SOFTWARE\Microsoft\Office\*)
   - Part 3: Delete shortcuts (Desktop, Start Menu)
   - Part 4: Clean temp files (%TEMP%\ODT\, etc.)
7. If any part fails: STOP and log error (atomicity guarantee)
8. PowerShell displays: "Sistema restaurado a estado anterior"
9. UC-005 FAILED (but system is clean)

#### Post-conditions (Success)
- Office LTSC installed with correct version/language/exclusions
- Teams and OneDrive excluded as requested
- Log file saved with success details
- System ready for use

#### Post-conditions (Failure)
- Installation rolled back completely
- System in clean state (no partial/broken installation)
- Log file saved with error details and rollback info
- User can retry after fixing issue

#### Special Requirements
- Download timeout: 300 seconds (vs 60 in old MSOI)
- Download retries: 3 attempts max
- Hash validation: Must validate SHA256 before extraction
- Progress bars: Show actual progress, not fake
- Rollback: All-or-nothing (atomic) - if any part fails, revert all
- Logging: Log every major action (download, extract, execute, rollback)

---

## 3. User Stories (PowerShell Perspective)

### Epic: "As a Sysadmin, I want to install Office using a professional PowerShell script, so that I have a reliable, documented installation process"

---

### User Story 1: Version Selection Menu

```
As a Sysadmin,
I want to see a menu to select Office version (2024, 2021, 2019),
So that I can choose the appropriate version for my environment.

Acceptance Criteria:

Given the script is running and prerequisites are validated
When the version selection menu is displayed
Then I should see:
  - Clear menu header: "Seleccione versión de Office"
  - 3 numbered options: 1. Office LTSC 2024, 2. Office LTSC 2021, 3. Office LTSC 2019
  - Prompt: "Seleccione una opción:"

Given I enter a valid option (1, 2, or 3)
When I press Enter
Then the system should:
  - Display success: "✓ Version selected: Office LTSC 2024"
  - Save version to configuration
  - Log the selection with timestamp
  - Proceed to language selection

Given I enter an invalid option (e.g., 5, "abc", empty)
When I press Enter
Then the system should:
  - Display error: "Opción inválida. Por favor ingrese 1, 2 o 3"
  - Re-display the menu
  - Not proceed until valid input
```

**INVEST Evaluation:**
- **I - Independent:** Can be implemented separately (Show-Menu function)
- **N - Negotiable:** UI details flexible (colors, format)
- **V - Valuable:** Directly enables version selection feature
- **E - Estimable:** Dev can estimate 2-3 hours for implementation + tests
- **S - Small:** Fits in sprint (single menu)
- **T - Testable:** Can mock user input with Pester
- **INVEST Score:** ✓ All criteria met

---

### User Story 2: Configuration Summary & Confirmation

```
As a Sysadmin,
I want to review my selections (version, language, exclusions) before installation,
So that I can confirm everything is correct before making system changes.

Acceptance Criteria:

Given all 3 menus (version, language, exclusions) are completed
When the summary screen is displayed
Then I should see:
  - "════════════════════════════════════════"
  - "Resumen de configuración:"
  - Versión: Office LTSC 2024
  - Idioma: Spanish (es-ES)
  - Exclusiones: Teams, OneDrive
  - "════════════════════════════════════════"
  - "¿Continuar con la instalación? (S/N):"

Given I enter "S"
When I press Enter
Then the system should:
  - Proceed to validation
  - Log confirmation

Given I enter "N"
When I press Enter
Then the system should:
  - Display: "Instalación cancelada"
  - Exit gracefully
  - Log cancellation
  - Not make any changes
```

**INVEST Evaluation:** ✓ All criteria met

---

### User Story 3: Automatic Validation with Progress

```
As a Sysadmin,
I want the system to validate my configuration automatically (8 checks),
So that I know the system can proceed safely before starting the download.

Acceptance Criteria:

Given configuration is confirmed with "S"
When validation starts
Then I should see:
  - Progress bar: "[████░░░░░░] 20% - Validating configuration..."
  - Each of 8 steps logged but displayed as single progress indicator

Given all 8 validation checks pass
When validation completes
Then I should see:
  - "✓ Configuración válida"
  - System proceeds to installation

Given any validation step fails (e.g., insufficient disk space)
When that step fails
Then I should see:
  - "ERROR 003: Espacio insuficiente en disco"
  - Clear guidance: "Libere al menos 5 GB en la unidad C:"
  - Execution stops (no installation attempt)
```

**INVEST Evaluation:** ✓ All criteria met

---

### User Story 4: Real-Time Installation Progress

```
As a Sysadmin,
I want to see real-time progress while Office is being installed,
So that I know the installation is proceeding and how long until completion.

Acceptance Criteria:

Given validation passed and installation started
When download begins (OfficeDeploymentTool.exe)
Then I should see:
  - "[████░░░░░░] 20% - Downloading OfficeDeploymentTool.exe..."
  - Progress updates every 5 seconds
  - Shows: "Descargado: 50/150 MB"

Given download completes and installation begins
When setup.exe starts
Then I should see:
  - "[████████░░] 50% - Installing Office..."
  - Progress bars update as setup.exe reports progress

Given installation completes
When setup.exe exits successfully
Then I should see:
  - "[██████████] 100% - ¡Instalación completada!"
  - Timestamp and log file location
```

**INVEST Evaluation:** ✓ All criteria met

---

### User Story 5: Automatic Rollback on Failure

```
As a Sysadmin,
I want the system to automatically undo changes if installation fails,
So that my system is not left in a broken/partial state.

Acceptance Criteria:

Given installation fails (setup.exe returns error)
When the failure is detected
Then the system should:
  - Display: "Instalación fallida, ejecutando rollback..."
  - Not show confusing errors to the user

Given rollback executes
When all 4 rollback parts complete
Then the system should:
  - Delete Office installation files
  - Clean registry entries
  - Delete shortcuts
  - Clean temporary files
  - Display: "Sistema restaurado a estado anterior"
  - Log complete details of what was cleaned

Given any rollback part fails
When that part fails
Then the system should:
  - Stop rolling back (no partial cleanup)
  - Log the failure with details
  - Alert user: "Rollback incomplete - manual intervention may be needed"
```

**INVEST Evaluation:** ✓ All criteria met

---

### User Story 6: Detailed Logging for Troubleshooting

```
As an IT Support engineer,
I want a detailed log file with timestamps and all actions,
So that I can troubleshoot failures without asking the Sysadmin for details.

Acceptance Criteria:

Given the script executes any action (menu, validation, install)
When the action completes
Then the system should:
  - Write to log: %TEMP%\OfficeAutomator_yyyyMMdd_HHmmss.log
  - Format: [2026-04-21 10:15:30] [INFO/SUCCESS/WARNING/ERROR] message

Given an error occurs
When the error is logged
Then the log should contain:
  - Error code (e.g., 003)
  - Error message (human readable)
  - Stack trace (for developers)
  - Suggested fix (for IT team)

Given the script completes
When the user exits
Then the log file path should be displayed:
  - "Log saved: C:\Users\Admin\AppData\Local\Temp\OfficeAutomator_20260421_101530.log"
```

**INVEST Evaluation:** ✓ All criteria met

---

## 4. Requirements Specification

### Functional Requirements

| ID | Requirement | Source | UC | Priority |
|----|-------------|--------|----|----|
| REQ-001 | Load OfficeAutomator.Core.dll at startup | User Story 1 | - | CRITICAL |
| REQ-002 | Display version selection menu (3 options) | User Story 1 | UC-001 | CRITICAL |
| REQ-003 | Validate version input (1-3 only) | User Story 1 | UC-001 | CRITICAL |
| REQ-004 | Display language selection menu (5 options) | User Story 1 | UC-002 | CRITICAL |
| REQ-005 | Validate language input (1-5 only) | User Story 1 | UC-002 | CRITICAL |
| REQ-006 | Display app exclusion menu (5 apps) | User Story 1 | UC-003 | CRITICAL |
| REQ-007 | Validate exclusion input (comma-separated, 1-5) | User Story 1 | UC-003 | CRITICAL |
| REQ-008 | Display configuration summary | User Story 2 | - | CRITICAL |
| REQ-009 | Ask for confirmation before proceeding | User Story 2 | - | CRITICAL |
| REQ-010 | Call ConfigValidator for 8-step validation | User Story 3 | UC-004 | CRITICAL |
| REQ-011 | Display validation progress bar | User Story 3 | UC-004 | HIGH |
| REQ-012 | Call InstallationExecutor for installation | User Story 4 | UC-005 | CRITICAL |
| REQ-013 | Display real-time installation progress | User Story 4 | UC-005 | HIGH |
| REQ-014 | Download timeout: 300 seconds (with 3 retries) | User Story 4 | UC-005 | CRITICAL |
| REQ-015 | Call RollbackExecutor if installation fails | User Story 5 | UC-005 | CRITICAL |
| REQ-016 | Guarantee atomic rollback (all-or-nothing) | User Story 5 | UC-005 | CRITICAL |
| REQ-017 | Write log file to %TEMP%\OfficeAutomator_*.log | User Story 6 | - | CRITICAL |
| REQ-018 | Log format: [timestamp] [LEVEL] message | User Story 6 | - | CRITICAL |
| REQ-019 | Display log file path at completion | User Story 6 | - | HIGH |

### Non-Functional Requirements

| ID | Requirement | Target | Rationale |
|----|-------------|--------|-----------|
| NFQR-001 | PowerShell version support | 5.1, 7.x | Backward compatible + modern |
| NFQR-002 | .NET runtime requirement | 8.0+ | Support Core.dll dependency |
| NFQR-003 | Installation time (happy path) | < 30 minutes | Acceptable for IT workflows |
| NFQR-004 | Code coverage (tests) | > 90% | Professional quality |
| NFQR-005 | Memory usage | < 500 MB | No resource issues |
| NFQR-006 | Error messages | In Spanish + English | User accessibility |
| NFQR-007 | Color support (menus) | ANSI colors | Visual clarity |
| NFQR-008 | Admin privilege check | Required at startup | Safety gate |

---

## 5. Design Options Evaluation

### Option A: Monolithic Script (Rejected)
```
Single 1000+ line PowerShell file
Pros: Simple to understand initially
Cons: Hard to maintain, hard to test, violation of SRP
Decision: REJECTED - Too rigid
```

### Option B: Modular Functions (SELECTED) ⭐
```
Main script: OfficeAutomator.PowerShell.Script.ps1 (300 lines)
Functions: Separate .ps1 files in scripts/functions/
  - OfficeAutomator.Menu.Display.ps1
  - OfficeAutomator.Validation.Environment.ps1
  - OfficeAutomator.Execution.Orchestration.ps1
  - OfficeAutomator.Execution.RollbackHandler.ps1
  - OfficeAutomator.Logging.Handler.ps1
  - OfficeAutomator.CoreDll.Loader.ps1

Pros:
  - Easy to test individual functions
  - Each function has single responsibility
  - Easy to reuse across scripts
  - Professional naming
  - Maintainable

Cons: Slightly more file management

Decision: SELECTED ✓
```

### Option C: PowerShell Module (Future)
```
Import-Module OfficeAutomator
Install-Office -Version 2024 -Language es-ES

Timeline: PHASE 2 (future)
```

---

## 6. Traceability Matrix

| UC | User Story | REQ | Test | Implementation |
|----|-----------|-----|------|-----------------|
| UC-001 | Story 1 | REQ-002, REQ-003 | OfficeAutomator.PowerShell.Integration.Tests.ps1 | OfficeAutomator.Menu.Display.ps1 |
| UC-002 | Story 1 | REQ-004, REQ-005 | OfficeAutomator.PowerShell.Integration.Tests.ps1 | OfficeAutomator.Menu.Display.ps1 |
| UC-003 | Story 1 | REQ-006, REQ-007 | OfficeAutomator.PowerShell.Integration.Tests.ps1 | OfficeAutomator.Menu.Display.ps1 |
| UC-004 | Story 3 | REQ-010, REQ-011 | OfficeAutomator.PowerShell.Integration.Tests.ps1 | OfficeAutomator.Execution.Orchestration.ps1 |
| UC-005 | Story 4, 5 | REQ-012 to REQ-016 | OfficeAutomator.PowerShell.EndToEnd.Tests.ps1 | OfficeAutomator.Execution.Orchestration.ps1 |
| Log | Story 6 | REQ-017 to REQ-019 | OfficeAutomator.PowerShell.Integration.Tests.ps1 | OfficeAutomator.Logging.Handler.ps1 |

---

## 7. Next Steps

### Tollgate Review
Before proceeding to DESIGN phase:

1. **Requirements Validation**
   - [ ] All 6 User Stories are clear and achievable
   - [ ] All 19 Functional Requirements are necessary
   - [ ] All 8 Non-Functional Requirements are realistic
   - [ ] Traceability matrix is complete

2. **Design Option Approval**
   - [ ] Option B (Modular Functions) is the chosen design
   - [ ] File structure is approved
   - [ ] Naming convention is approved

3. **UC Approval**
   - [ ] All 5 UC flows are understood
   - [ ] Alternative flows are acceptable
   - [ ] Error scenarios are covered

---

**Status:** Phase 2 ANALYZE Complete - Requirements Fully Specified  
**Ready for:** Phase 3 DESIGN (Architecture & Detailed Design)  
**Prepared by:** Claude  
**Date:** 2026-04-21

