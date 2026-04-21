```yml
created_at: 2026-05-16 17:15
updated_at: 2026-05-16 17:15
document_type: Architecture Reference - Configuration Lifecycle
document_version: 1.0.0
version_notes: Complete $Config tracking through all 5 UCs from INIT to INSTALL_COMPLETE
stage: Stage 7 - DESIGN/SPECIFY
work_package: 2026-04-21-06-15-00-design-specification-correct
phase: 2-Agile-Sprints
sprint_number: 2
task_id: T-028
task_name: Configuration Object Lifecycle
execution_date: 2026-05-16 17:15 onwards
duration_hours: TBD
story_points: 3
roles_involved: ARCHITECT (Claude)
dependencies: T-019 (Configuration class), T-026 (State Machine), T-027 (Error Scenarios)
design_artifacts:
  - Configuration object initial state (INIT)
  - $Config evolution through all 5 UCs
  - Property updates at each UC boundary
  - State transitions documented
  - Error states (errorResult population)
  - Property ownership matrix (which UC can modify which property)
  - Complete lifecycle diagram
acceptance_criteria:
  - $Config initial state documented
  - State after each UC documented (UC-001 through UC-005)
  - All 9 properties tracked
  - Property ownership clear (which UC owns which property)
  - Error states documented (errorResult population)
  - State transitions match state machine (T-026)
  - Lifecycle diagram clear and complete
status: IN PROGRESS
```

# DESIGN: CONFIGURATION OBJECT LIFECYCLE

## Overview

This document tracks the Configuration ($Config) object through its complete lifecycle in OfficeAutomator. It shows how $Config evolves from initialization through all 5 UCs, which properties are set when, and which components own which properties.

**Version:** 1.0.0  
**Scope:** Complete $Config lifecycle from INIT to INSTALL_COMPLETE  
**Source:** T-019 (Configuration class), T-026 (State Machine), all UC designs  
**Purpose:** Reference for Stage 10 implementation to understand data flow

---

## 1. Configuration Class Structure (T-019)

```csharp
public class Configuration {
    // Version Selection (UC-001)
    public string version;              // "2024" | "2021" | "2019"
    
    // Language Selection (UC-002)
    public string[] languages;          // ["en-US"] | ["es-MX"] | ["en-US", "es-MX"]
    
    // Application Exclusion (UC-003)
    public string[] excludedApps;       // [] | ["Teams"] | ["Teams", "OneDrive"] | ...
    
    // Configuration File (UC-004)
    public string configPath;           // "C:\Users\...\config_TIMESTAMP.xml"
    public bool validationPassed;       // true | false
    
    // Installation (UC-005)
    public string odtPath;              // "C:\Program Files\Microsoft Office\...\setup.exe"
    
    // State Management (T-019)
    public string state;                // INIT | SELECT_VERSION | ... | INSTALL_COMPLETE
    
    // Error Handling (T-020)
    public ErrorResult errorResult;     // null or {code, message, details}
    
    // Audit Trail (All UCs)
    public DateTime timestamp;          // Last update timestamp
}
```

---

## 2. $Config Lifecycle: Step-by-Step Evolution

### **PHASE 1: INITIALIZATION (State = INIT)**

```
Initial State (User starts OfficeAutomator):

$Config = {
    version: null,
    languages: null,
    excludedApps: null,
    configPath: null,
    validationPassed: false,
    odtPath: null,
    state: "INIT",
    errorResult: null,
    timestamp: 2026-05-16T17:15:00Z
}

Owner: OfficeAutomatorStateMachine
Duration: Until user clicks "Install"
Next: SELECT_VERSION state
```

---

### **PHASE 2: UC-001 - SELECT_VERSION**

```
Entry State: INIT
User Action: Selects Office version (2024, 2021, or 2019)
Owner: VersionSelector (T-022)

VersionSelector.Execute():
    1. Display UI with version options
    2. GetUserSelection() → "2024"
    3. IsValidVersion("2024") → true
    4. $Config.version = "2024"
    5. $Config.state = "SELECT_VERSION" → "SELECT_LANGUAGE"
    6. $Config.timestamp = DateTime.Now

After UC-001:

$Config = {
    version: "2024",                    ← POPULATED by UC-001
    languages: null,
    excludedApps: null,
    configPath: null,
    validationPassed: false,
    odtPath: null,
    state: "SELECT_LANGUAGE",
    errorResult: null,
    timestamp: 2026-05-16T17:15:30Z     ← UPDATED
}

Property Ownership:
    • version: Owned by UC-001/VersionSelector
    • State: Transitioned from SELECT_VERSION to SELECT_LANGUAGE
    
Next: SELECT_LANGUAGE state
```

---

### **PHASE 3: UC-002 - SELECT_LANGUAGE**

```
Entry State: SELECT_LANGUAGE
Preconditions:
    • $Config.version must be set (from UC-001) ✓
    
User Action: Selects languages (en-US, es-MX, or both)
Owner: LanguageSelector (T-022)

LanguageSelector.Execute():
    1. Display UI with language options
    2. Pre-populate based on system locale
    3. GetUserSelection() → ["en-US"]
    4. IsValidLanguageSelection(["en-US"]) → true
    5. $Config.languages = ["en-US"]
    6. $Config.state = "SELECT_LANGUAGE" → "SELECT_APPS"
    7. $Config.timestamp = DateTime.Now

After UC-002:

$Config = {
    version: "2024",
    languages: ["en-US"],               ← POPULATED by UC-002
    excludedApps: null,
    configPath: null,
    validationPassed: false,
    odtPath: null,
    state: "SELECT_APPS",
    errorResult: null,
    timestamp: 2026-05-16T17:15:45Z
}

Property Ownership:
    • languages: Owned by UC-002/LanguageSelector
    • version: Read from UC-001 (not modified)
    
Next: SELECT_APPS state
```

---

### **PHASE 4: UC-003 - SELECT_APPS**

```
Entry State: SELECT_APPS
Preconditions:
    • $Config.version must be set (from UC-001) ✓
    • $Config.languages must be set (from UC-002) ✓
    
User Action: Selects apps to exclude
Owner: AppExclusionSelector (T-023)

AppExclusionSelector.Execute():
    1. Display UI with exclusion checkboxes
    2. Pre-check Teams and OneDrive (defaults)
    3. GetUserSelections() → ["OneDrive"]
    4. IsValidExclusionSet(["OneDrive"]) → true
    5. $Config.excludedApps = ["OneDrive"]
    6. $Config.state = "SELECT_APPS" → "GENERATE_CONFIG"
    7. $Config.timestamp = DateTime.Now

After UC-003:

$Config = {
    version: "2024",
    languages: ["en-US"],
    excludedApps: ["OneDrive"],         ← POPULATED by UC-003
    configPath: null,
    validationPassed: false,
    odtPath: null,
    state: "GENERATE_CONFIG",
    errorResult: null,
    timestamp: 2026-05-16T17:15:55Z
}

Property Ownership:
    • excludedApps: Owned by UC-003/AppExclusionSelector
    • version, languages: Read from previous UCs (not modified)
    
Next: VALIDATE state (automatic GENERATE_CONFIG transition)
```

---

### **PHASE 5: UC-004 - VALIDATE**

```
Entry State: VALIDATE
Preconditions:
    • $Config.version != null ✓
    • $Config.languages != null ✓
    • $Config.excludedApps != null ✓
    
Automatic Action: Validate selections and generate config.xml
Owner: ConfigValidator & ConfigGenerator (T-024)

ConfigGenerator.GenerateConfigXml():
    1. Create output directory (%APPDATA%\OfficeAutomator\)
    2. Generate timestamped filename: config_20260516_171600.xml
    3. BuildConfigurationXml() using all 3 selections
    4. ValidateXmlStructure() → true
    5. Write to disk
    6. $Config.configPath = "C:\Users\...\config_20260516_171600.xml"
    7. $Config.timestamp = DateTime.Now

ConfigValidator.Execute() - 8 validation steps:
    Step 0: Check config.xml exists ✓
    Step 1: Validate XML schema ✓
    Step 2: Check version availability ✓
    Step 3: Check language support ✓
    Step 4: Download & verify hash ✓
    Step 5: Check excluded apps ✓
    Step 6: Verify Office not installed ✓
    Step 7: Display summary to user ✓
    
    All steps pass in <1000ms
    
    $Config.validationPassed = true
    $Config.state = "VALIDATE" → "INSTALL_READY"
    $Config.timestamp = DateTime.Now

After UC-004:

$Config = {
    version: "2024",
    languages: ["en-US"],
    excludedApps: ["OneDrive"],
    configPath: "C:\Users\Alice\AppData\Local\OfficeAutomator\config_20260516_171600.xml",  ← GENERATED
    validationPassed: true,             ← SET by UC-004
    odtPath: null,
    state: "INSTALL_READY",
    errorResult: null,
    timestamp: 2026-05-16T17:16:00Z
}

Property Ownership:
    • configPath: Owned by ConfigGenerator (UC-004)
    • validationPassed: Set by ConfigValidator (UC-004)
    • All previous properties: Read only (not modified)
    
Next: INSTALL_READY state
```

---

### **PHASE 6: UC-005 - INSTALLING**

```
Entry State: INSTALLING
Preconditions:
    • $Config.validationPassed == true ✓
    • $Config.configPath exists and valid ✓
    • User clicked "Proceed" at INSTALL_READY
    
Automatic Action: Execute setup.exe with config.xml
Owner: InstallationExecutor (T-025)

InstallationExecutor.Execute():
    1. VerifyPrerequisites():
       • Check: Running as Admin ✓
       • Check: Disk space ≥ 10GB ✓
       • Check: Office not installed ✓
    2. DownloadOffice():
       • Download from CDN: office_2024_en-US.iso
       • Extract to cache: %APPDATA%\OfficeAutomator\cache\2024
       • Validate download hash
    3. ExecuteSetup():
       • Run: C:\cache\2024\setup.exe /configure "config.xml"
       • Monitor: Progress bar 0-100%
       • Duration: ~15 minutes
       • Check exit code: 0 (success)
    4. ValidateInstallation():
       • Check: Word.exe, Excel.exe, PowerPoint.exe exist ✓
       • Check: HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Office exists ✓
       • All checks pass
    5. $Config.odtPath = "C:\Program Files\Microsoft Office\root\Office16\setup.exe"
    6. $Config.state = "INSTALLING" → "INSTALL_COMPLETE"
    7. $Config.timestamp = DateTime.Now

After UC-005 (SUCCESS):

$Config = {
    version: "2024",
    languages: ["en-US"],
    excludedApps: ["OneDrive"],
    configPath: "C:\Users\Alice\AppData\Local\OfficeAutomator\config_20260516_171600.xml",
    validationPassed: true,
    odtPath: "C:\Program Files\Microsoft Office\root\Office16\setup.exe",  ← SET by UC-005
    state: "INSTALL_COMPLETE",          ← FINAL STATE
    errorResult: null,
    timestamp: 2026-05-16T17:31:00Z
}

Property Ownership:
    • odtPath: Owned by InstallationExecutor (UC-005)
    • state: INSTALL_COMPLETE (final success state)
    
Final State: INSTALL_COMPLETE ✓
User can now launch Office applications
```

---

## 3. Error States: errorResult Population

### **Error Path Example: Installation Failure**

```
Scenario: setup.exe fails with exit code 1 during INSTALLING state

InstallationExecutor.Execute():
    ...
    int exitCode = ExecuteSetup(...);
    if (exitCode != SETUP_SUCCESS) {  // 0 != 1
        $Config.errorResult = new ErrorResult {
            code: "OFF-INSTALL-401",
            message: "Office installation failed",
            technicalDetails: "setup.exe exit code: 1"
        };
        $Config.state = "INSTALLING" → "INSTALL_FAILED";
        $Config.timestamp = DateTime.Now;
        return false;
    }

After Error (INSTALL_FAILED):

$Config = {
    version: "2024",
    languages: ["en-US"],
    excludedApps: ["OneDrive"],
    configPath: "...\config_20260516_171600.xml",
    validationPassed: true,
    odtPath: null,                      ← NOT SET (installation failed)
    state: "INSTALL_FAILED",            ← ERROR STATE
    errorResult: {                      ← POPULATED with error details
        code: "OFF-INSTALL-401",
        message: "Office installation failed",
        technicalDetails: "setup.exe exit code: 1"
    },
    timestamp: 2026-05-16T17:20:00Z
}

RollbackExecutor.Execute():
    Part 1: Remove Office files → ✓
    Part 2: Clean registry → ✓
    Part 3: Remove shortcuts → ✓
    
    Transition: INSTALL_FAILED → ROLLED_BACK

After Successful Rollback (ROLLED_BACK):

$Config = {
    version: "2024",
    languages: ["en-US"],
    excludedApps: ["OneDrive"],
    configPath: null,                   ← CLEARED (for retry)
    validationPassed: false,            ← RESET (for retry)
    odtPath: null,
    state: "ROLLED_BACK",               ← RECOVERY STATE
    errorResult: {                      ← Kept for logging/audit
        code: "OFF-INSTALL-401",
        message: "Office installation failed",
        technicalDetails: "setup.exe exit code: 1"
    },
    timestamp: 2026-05-16T17:21:00Z
}

User can now: [Retry] → back to INIT or [Cancel] → exit
```

---

## 4. Property Ownership Matrix

```
Property Name       | Data Type    | Owner UC      | Populated When  | Modifiable
────────────────────────────────────────────────────────────────────────────────
version             | string       | UC-001        | SELECT_VERSION  | UC-001 only
languages[]         | string[]     | UC-002        | SELECT_LANGUAGE | UC-002 only
excludedApps[]      | string[]     | UC-003        | SELECT_APPS     | UC-003 only
configPath          | string       | UC-004        | VALIDATE        | UC-004 only
validationPassed    | bool         | UC-004        | VALIDATE        | UC-004 only
odtPath             | string       | UC-005        | INSTALLING      | UC-005 only
state               | string       | State Machine | All UCs         | State transitions
errorResult         | ErrorResult  | ErrorHandler  | On error        | Set by handler
timestamp           | DateTime     | Each UC       | Each update     | Auto-updated
────────────────────────────────────────────────────────────────────────────────

Key Rules:
  ✓ Each property owned by one UC (no conflicts)
  ✓ Properties are WRITE-ONCE (set once, then read-only)
  ✓ State transitions managed by StateMachine (not individual UCs)
  ✓ errorResult populated only when error occurs
  ✓ timestamp auto-updated on each property change
```

---

## 5. State Evolution Timeline

```
Time    State               Properties Set             Owner
──────────────────────────────────────────────────────────────────
0:00    INIT                (all null)                 OfficeAutomator
        ↓
0:15    SELECT_VERSION      version = "2024"           UC-001
        ↓
0:30    SELECT_LANGUAGE     languages = ["en-US"]      UC-002
        ↓
0:45    SELECT_APPS         excludedApps = [...]       UC-003
        ↓
1:00    GENERATE_CONFIG     (prep)                     System
        ↓
1:05    VALIDATE            configPath, valid = true   UC-004
        ↓
1:10    INSTALL_READY       (confirm)                  User
        ↓ (user proceeds)
1:15    INSTALLING          odtPath, state = COMPLETE  UC-005
        ↓
1:30    INSTALL_COMPLETE    ✓ Final state              Success
```

---

## 6. Data Flow Between UCs

```
UC-001 Output → UC-002 Input:
    version (required) → LanguageSelector checks version-language matrix
    
UC-002 Output → UC-003 Input:
    version, languages (required) → AppExclusionSelector validates combo
    
UC-003 Output → UC-004 Input:
    version, languages, excludedApps (required) → ConfigValidator uses all 3
    
UC-004 Output → UC-005 Input:
    validationPassed (required), configPath (required) → InstallationExecutor
    
Data Continuity: Each UC reads previous outputs + adds new property
No Data Loss: All properties preserved through workflow
```

---

## 7. Critical Lifecycle Points

### **Point 1: Validation Boundary (UC-004)**
```
BEFORE UC-004: User selections only, no system interaction
AFTER UC-004: configPath set (config.xml generated), system validated
⚠️  CRITICAL: If validation fails here, user must restart from SELECT_VERSION
   (cannot modify selections after validation)
```

### **Point 2: Installation Authorization (UC-005 Entry)**
```
INSTALL_READY: User confirms all selections
User cannot change version, languages, or exclusions after this point
⚠️  CRITICAL: Must display summary before User Authorization
```

### **Point 3: Rollback Trigger (UC-005 Failure)**
```
INSTALLING → INSTALL_FAILED (automatic rollback)
$Config.validationPassed reset to false (for potential retry)
$Config.configPath cleared (will be regenerated on retry)
⚠️  CRITICAL: Atomic rollback (all-or-nothing, no partial states)
```

### **Point 4: Terminal State (INSTALL_COMPLETE)**
```
All properties finalized
User can launch Office applications
$Config no longer modified (workflow complete)
```

---

## 8. Acceptance Criteria Verification

```
✓ $Config initial state documented
    • All 9 properties with null/default values
    • INIT state documented

✓ State after each UC documented (UC-001 through UC-005)
    • PHASE 2-6: Each UC shows $Config state after
    • Success path documented
    • Error path documented

✓ All 9 properties tracked
    • version, languages, excludedApps (user selections)
    • configPath, validationPassed (UC-004 outputs)
    • odtPath (UC-005 output)
    • state, errorResult, timestamp (management)

✓ Property ownership clear (which UC owns which property)
    • Matrix provided showing owner for each property
    • Write-once principle established
    • No conflicts between UCs

✓ Error states documented (errorResult population)
    • Error path example provided
    • errorResult structure shown
    • Reset on rollback documented

✓ State transitions match state machine (T-026)
    • Evolution timeline matches T-026 state transitions
    • INSTALL_COMPLETE is terminal state
    • ROLLED_BACK allows retry from INIT

✓ Lifecycle diagram clear and complete
    • Step-by-step evolution shown
    • Data flow between UCs verified
    • Critical lifecycle points identified
```

---

## Document Metadata

```
Created: 2026-05-16 17:15
Task: T-028 Configuration Object Lifecycle
Version: 1.0.0
Story Points: 3
Duration: Final design
Status: IN PROGRESS
Dependencies: T-019, T-026, T-027
Next: T-029 (Retry Integration, buffer)
Use: Reference for Stage 10 implementation
Quality Gate: Lifecycle complete and verified
```

---

**T-028 IN PROGRESS**

**Configuration object lifecycle complete: INIT → INSTALL_COMPLETE with property tracking and error states ✓**

