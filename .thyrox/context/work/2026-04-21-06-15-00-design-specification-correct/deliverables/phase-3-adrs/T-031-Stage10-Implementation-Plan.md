```yml
created_at: 2026-05-16 18:45
updated_at: 2026-05-16 18:45
document_type: Project Management - Stage 10 Implementation Plan
document_version: 1.0.0
version_notes: Complete Stage 10 implementation roadmap with class sequence, test plans, and timeline
stage: Stage 8+ - Handoff to Implementation
work_package: 2026-04-21-06-15-00-design-specification-correct
phase: 3-ADRs-and-PM
sprint_number: 3
task_id: T-031
task_name: Stage 10 Implementation Plan
execution_date: 2026-05-16 18:45 onwards
duration_hours: TBD
story_points: 5
roles_involved: PROJECT MANAGER (Claude), STAGE 10 LEAD (TBD)
dependencies: T-019 through T-030 (all design documents)
design_artifacts:
  - Class implementation sequence (10 classes, build order)
  - Unit test plan (per class, acceptance criteria)
  - Integration test plan (UC boundaries, data flow)
  - E2E test plan (all 5 UCs end-to-end)
  - Daily timeline (Day 1-8, ~40 hours total)
  - Success criteria
  - Risk mitigation
acceptance_criteria:
  - 10 classes in build sequence documented
  - Unit test plan for each class
  - Integration test coverage defined
  - E2E test scenarios documented
  - Timeline realistic and achievable
  - Resource requirements clear
  - Success criteria measurable
status: IN PROGRESS
```

# STAGE 10 IMPLEMENTATION PLAN

## Overview

This document provides a detailed implementation plan for Stage 10 (Week 5). It outlines the sequence for implementing 10 OfficeAutomator classes, testing strategy, daily timeline, and success criteria.

**Version:** 1.0.0  
**Duration:** Week 5 (8 working days)  
**Effort:** ~40 hours total (~5 hours per day)  
**Deliverable:** 10 classes fully implemented and tested  
**Target:** All unit tests passing, integration tests passing, ready for Stage 11

---

## 1. Class Implementation Sequence

### Build Order Strategy

Classes implemented in dependency order:

```
LAYER 1 (Data)
  1. Configuration — Data structure, no dependencies

LAYER 2 (Infrastructure)
  2. OfficeAutomatorStateMachine — Orchestration, depends on Configuration
  3. ErrorHandler — Error handling, depends on Configuration

LAYER 3 (User Interaction - Selectors)
  4. VersionSelector — UC-001, depends on Configuration + StateMachine
  5. LanguageSelector — UC-002, depends on Configuration + StateMachine
  6. AppExclusionSelector — UC-003, depends on Configuration + StateMachine

LAYER 4 (Validation)
  7. ConfigGenerator — UC-004 Part A, depends on Configuration + ErrorHandler
  8. ConfigValidator — UC-004 Part B, depends on Configuration + ErrorHandler

LAYER 5 (Execution & Recovery)
  9. InstallationExecutor — UC-005 Part A, depends on Configuration + ErrorHandler
  10. RollbackExecutor — UC-005 Part B, depends on Configuration + ErrorHandler
```

### Why This Order?

- **Bottom-up:** Foundation (Configuration) before infrastructure (StateMachine)
- **Infrastructure first:** StateMachine and ErrorHandler needed by all others
- **Functional grouping:** Selectors together, Validators together, Executors together
- **Dependencies:** Each class has dependencies on previous classes

---

## 2. Detailed Class Implementation (Day-by-Day Breakdown)

### **DAY 1-2: LAYER 1 & LAYER 2 (Configuration, StateMachine, ErrorHandler)**

#### Class 1: Configuration (Day 1 Morning)

```
Hours: 1-1.5
What: Data class with 9 properties
Deliverable: 
  • Property definitions (version, languages, excludedApps, etc.)
  • Constructor
  • Property getters/setters
  
Code Template (from T-019):
  public class Configuration {
    public string version;
    public string[] languages;
    public string[] excludedApps;
    public string configPath;
    public bool validationPassed;
    public string odtPath;
    public string state;
    public ErrorResult errorResult;
    public DateTime timestamp;
  }

Unit Test (T-031):
  • Test: Property initialization
  • Test: Property updates
  • Test: Write-once enforcement (if implemented)
  
Success: Configuration class created, unit tests passing
```

#### Class 2: OfficeAutomatorStateMachine (Day 1 Afternoon)

```
Hours: 2-2.5
What: State machine orchestrator, 11 states
Deliverable:
  • 11 state definitions
  • Transition logic
  • State change methods
  • Entry/exit handlers

Code Template (from T-019):
  public class OfficeAutomatorStateMachine {
    public string CurrentState { get; set; }
    
    public void TransitionTo(string newState) {
      // Validate transition
      // Log state change
      // Execute entry handler for new state
    }
    
    public bool IsValidTransition(string from, string to) {
      // Check if transition allowed
    }
  }

Unit Test (T-031):
  • Test: State initialization (INIT)
  • Test: Valid transitions (e.g., INIT → SELECT_VERSION)
  • Test: Invalid transitions (rejected)
  • Test: All 11 states reachable
  
Success: State machine working, transitions verified
```

#### Class 3: ErrorHandler (Day 2 Morning-Afternoon)

```
Hours: 3-4
What: Error handling, 19 error codes, 3 retry policies
Deliverable:
  • ErrorResult class definition
  • ErrorHandler with 19 error code constants
  • Retry logic (transient 3x, system 1x, permanent 0x)
  • Logging infrastructure

Code Template (from T-020):
  public class ErrorHandler {
    public const string OFF_CONFIG_001 = "OFF-CONFIG-001";
    public const string OFF_SECURITY_101 = "OFF-SECURITY-101";
    // ... 19 codes total
    
    public ErrorResult HandleError(string errorCode, Exception ex) {
      RetryPolicy policy = GetRetryPolicy(errorCode);
      return new ErrorResult {
        code = errorCode,
        message = GetMessage(errorCode),
        technicalDetails = ex.Message
      };
    }
    
    private RetryPolicy GetRetryPolicy(string code) {
      // Transient (3x), System (1x), or Permanent (0x)
    }
  }

Unit Test (T-031):
  • Test: 19 error codes defined
  • Test: Transient errors (3x retry)
  • Test: System errors (1x retry)
  • Test: Permanent errors (0x retry)
  • Test: Error messages generated
  
Success: ErrorHandler fully functional, retry logic verified
```

**Day 1-2 Deliverable:** Configuration, StateMachine, ErrorHandler → Unit tests passing ✓

---

### **DAY 3-4: LAYER 3 (Version, Language, App Selectors)**

#### Class 4: VersionSelector (Day 3 Morning)

```
Hours: 1-1.5
What: UC-001, user selects Office version
Deliverable:
  • GetVersionOptions() → ["2024", "2021", "2019"]
  • IsValidVersion(version) → bool
  • Execute() → populates $Config.version

Code Template (from T-022):
  public class VersionSelector {
    public List<string> GetVersionOptions() {
      return new[] { "2024", "2021", "2019" };
    }
    
    public bool IsValidVersion(string version) {
      return GetVersionOptions().Contains(version);
    }
    
    public bool Execute(Configuration config, string selectedVersion) {
      if (!IsValidVersion(selectedVersion)) {
        config.errorResult = new ErrorResult { 
          code = "OFF-CONFIG-001" 
        };
        return false;
      }
      config.version = selectedVersion;
      config.state = "SELECT_LANGUAGE";
      return true;
    }
  }

Unit Test (T-031):
  • Test: Valid version (2024) accepted
  • Test: Invalid version rejected
  • Test: $Config.version populated
  • Test: State transition to SELECT_LANGUAGE
  
Success: VersionSelector working
```

#### Class 5: LanguageSelector (Day 3 Afternoon)

```
Hours: 1.5-2
What: UC-002, user selects languages
Deliverable:
  • GetLanguageOptions() → [en-US, es-MX, both]
  • IsValidLanguageSelection() → bool
  • Execute() → populates $Config.languages

Code Template (from T-022):
  public class LanguageSelector {
    public List<string> GetLanguageOptions() {
      return new[] { "en-US", "es-MX", "en-US,es-MX" };
    }
    
    public bool IsValidLanguageSelection(string[] languages) {
      // Check all languages in whitelist
    }
    
    public bool Execute(Configuration config, string[] languages) {
      if (!IsValidLanguageSelection(languages)) {
        config.errorResult = new ErrorResult { 
          code = "OFF-CONFIG-002" 
        };
        return false;
      }
      config.languages = languages;
      config.state = "SELECT_APPS";
      return true;
    }
  }

Unit Test (T-031):
  • Test: Valid languages accepted
  • Test: Invalid languages rejected
  • Test: $Config.languages populated
  • Test: State transition to SELECT_APPS
  
Success: LanguageSelector working
```

#### Class 6: AppExclusionSelector (Day 4 Morning)

```
Hours: 1.5-2
What: UC-003, user selects apps to exclude
Deliverable:
  • GetExcludableApps() → [Teams, OneDrive, Groove, Lync, Bing]
  • IsValidExclusionSet() → bool
  • Execute() → populates $Config.excludedApps

Code Template (from T-023):
  public class AppExclusionSelector {
    private static readonly string[] EXCLUDABLE_APPS = {
      "Teams", "OneDrive", "Groove", "Lync", "Bing"
    };
    
    public string[] GetExcludableApps() => EXCLUDABLE_APPS;
    
    public bool IsValidExclusionSet(string[] apps) {
      return apps.All(app => GetExcludableApps().Contains(app));
    }
    
    public bool Execute(Configuration config, string[] excludedApps) {
      if (!IsValidExclusionSet(excludedApps)) {
        config.errorResult = new ErrorResult { 
          code = "OFF-CONFIG-003" 
        };
        return false;
      }
      config.excludedApps = excludedApps;
      config.state = "GENERATE_CONFIG";
      return true;
    }
  }

Unit Test (T-031):
  • Test: Valid exclusion (Teams)
  • Test: Invalid exclusion rejected
  • Test: Multiple exclusions valid
  • Test: $Config.excludedApps populated
  
Success: AppExclusionSelector working
```

**Day 3-4 Deliverable:** All 3 Selectors → Unit tests passing ✓

---

### **DAY 5-6: LAYER 4 (ConfigGenerator, ConfigValidator)**

#### Class 7: ConfigGenerator (Day 5 Morning)

```
Hours: 1.5-2
What: UC-004 Part A, generate config.xml
Deliverable:
  • GenerateConfigXml(config) → file path
  • Validates structure before writing
  • Returns path in $Config.configPath

Code Template (from T-024):
  public class ConfigGenerator {
    public string GenerateConfigXml(Configuration config) {
      string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
      string path = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "OfficeAutomator",
        $"config_{timestamp}.xml"
      );
      
      XDocument doc = new XDocument(
        new XElement("Configuration",
          new XElement("Version", config.version),
          new XElement("Languages", 
            config.languages.Select(l => new XElement("Language", l))),
          new XElement("ExcludedApps",
            config.excludedApps.Select(a => new XElement("App", a)))
        )
      );
      
      doc.Save(path);
      return path;
    }
  }

Unit Test (T-031):
  • Test: config.xml created
  • Test: Valid XML structure
  • Test: Path returned correctly
  • Test: File readable
  
Success: ConfigGenerator working
```

#### Class 8: ConfigValidator (Day 5-6 Afternoon)

```
Hours: 3-4
What: UC-004 Part B, 8-step validation, <1 second timeout
Deliverable:
  • Execute() → 8 steps validation
  • Stopwatch timeout enforcement (<1000ms)
  • Transient retry for hash (OFF-SECURITY-101)
  • Returns bool, populates $Config.validationPassed

Code Template (from T-024):
  public class ConfigValidator {
    public bool Execute(Configuration config) {
      Stopwatch sw = Stopwatch.StartNew();
      const int MAX_MS = 1000;
      
      // Step 0: Check config.xml exists
      if (!File.Exists(config.configPath)) {
        config.errorResult = new ErrorResult { code = "OFF-CONFIG-004" };
        return false;
      }
      if (sw.ElapsedMilliseconds > MAX_MS) return TimeoutError(config);
      
      // Step 1: Validate XML schema
      // Step 2: Check version available
      // Step 3: Check language support
      // Step 4: Download & verify hash (WITH RETRY)
      for (int attempt = 1; attempt <= 3; attempt++) {
        if (ValidateHash(config)) break;
        if (attempt < 3) Thread.Sleep(attempt * 2000);
      }
      // ... remaining steps
      
      config.validationPassed = true;
      return true;
    }
  }

Unit Test (T-031):
  • Test: All 8 steps executed
  • Test: Timeout enforced (<1000ms)
  • Test: Hash retry (3x)
  • Test: validationPassed set to true
  
Success: ConfigValidator fully functional
```

**Day 5-6 Deliverable:** ConfigGenerator + ConfigValidator → Unit tests passing ✓

---

### **DAY 7-8: LAYER 5 (InstallationExecutor, RollbackExecutor)**

#### Class 9: InstallationExecutor (Day 7 Morning-Afternoon)

```
Hours: 4-5
What: UC-005 Part A, execute setup.exe, 20 minute timeout
Deliverable:
  • VerifyPrerequisites() → admin, disk space, idempotence
  • DownloadOffice() → with 3x transient retry
  • ExecuteSetup() → run setup.exe, monitor, 20 min timeout
  • ValidateInstallation() → verify files, registry
  • Returns bool, populates $Config.odtPath

Code Template (from T-025):
  public class InstallationExecutor {
    public bool Execute(Configuration config) {
      if (!VerifyPrerequisites()) return false;
      if (!DownloadOffice()) return false;  // 3x retry internally
      if (!ExecuteSetup(config)) return false;
      if (!ValidateInstallation()) return false;
      
      config.odtPath = FindSetupPath();
      config.state = "INSTALL_COMPLETE";
      return true;
    }
    
    private bool ExecuteSetup(Configuration config) {
      Stopwatch sw = Stopwatch.StartNew();
      const int MAX_MS = 1200000;  // 20 minutes
      
      Process p = Process.Start(new ProcessStartInfo {
        FileName = "setup.exe",
        Arguments = $"/configure \"{config.configPath}\""
      });
      
      while (!p.HasExited) {
        if (sw.ElapsedMilliseconds > MAX_MS) {
          p.Kill();
          config.errorResult = new ErrorResult { code = "OFF-SYSTEM-201" };
          return false;
        }
        Thread.Sleep(1000);
      }
      
      return p.ExitCode == 0;
    }
  }

Unit Test (T-031):
  • Test: Prerequisites checked
  • Test: Download attempted (mock)
  • Test: setup.exe executed (mock)
  • Test: Timeout enforced (20 min)
  • Test: Installation validated
  
Success: InstallationExecutor working
```

#### Class 10: RollbackExecutor (Day 8 Morning)

```
Hours: 2-2.5
What: UC-005 Part B, atomic 3-part rollback
Deliverable:
  • RemoveOfficeFiles() → Part 1
  • CleanRegistry() → Part 2
  • RemoveShortcuts() → Part 3
  • Execute() → all 3 or fail atomically

Code Template (from T-025):
  public class RollbackExecutor {
    public bool Execute(Configuration config) {
      bool part1 = RemoveOfficeFiles();
      bool part2 = CleanRegistry();
      bool part3 = RemoveShortcuts();
      
      if (part1 && part2 && part3) {
        config.state = "ROLLED_BACK";
        return true;
      }
      
      if (!part1) config.errorResult = new ErrorResult { 
        code = "OFF-ROLLBACK-501" 
      };
      if (!part2) config.errorResult = new ErrorResult { 
        code = "OFF-ROLLBACK-502" 
      };
      if (!part3) config.errorResult = new ErrorResult { 
        code = "OFF-ROLLBACK-503" 
      };
      
      config.state = "INSTALL_FAILED";
      return false;
    }
  }

Unit Test (T-031):
  • Test: All 3 parts succeed
  • Test: Part 1 fails → OFF-ROLLBACK-501
  • Test: Part 2 fails → OFF-ROLLBACK-502
  • Test: Part 3 fails → OFF-ROLLBACK-503
  • Test: Atomic semantics
  
Success: RollbackExecutor fully functional
```

**Day 7-8 Deliverable:** InstallationExecutor + RollbackExecutor → Unit tests passing ✓

---

## 3. Integration Test Plan

### UC Boundary Testing

```
UC-001 ↔ UC-002 (Version → Language)
  Test: VersionSelector.Execute() → UC-002 can read $Config.version ✓
  Test: Invalid version in UC-001 → UC-002 never called
  
UC-002 ↔ UC-003 (Languages → Apps)
  Test: LanguageSelector.Execute() → UC-003 can read languages ✓
  Test: Invalid language → UC-003 never called
  
UC-003 ↔ UC-004 (Apps → Validation)
  Test: AppExclusionSelector.Execute() → UC-004 has excludedApps ✓
  Test: ConfigGenerator uses excludedApps correctly
  
UC-004 ↔ UC-005 (Validation → Installation)
  Test: ConfigValidator.Execute() → validationPassed = true
  Test: InstallationExecutor reads configPath ✓
  Test: Validation failure → Installation never starts
  
UC-005 Error Path (Installation ↔ Rollback)
  Test: InstallationExecutor fails → RollbackExecutor starts
  Test: Rollback atomic (all 3 parts or fail)
```

### State Machine Integration

```
Test entire state sequence:
  INIT → SELECT_VERSION → SELECT_LANGUAGE → SELECT_APPS
  → GENERATE_CONFIG → VALIDATE → INSTALL_READY
  → INSTALLING → INSTALL_COMPLETE (success path)
  
Error path:
  INIT → ... → INSTALLING → INSTALL_FAILED
  → ROLLED_BACK → (user can retry to INIT)
```

---

## 4. End-to-End (E2E) Test Plan

### E2E Test Scenarios (All 5 UCs)

```
E2E-001: Happy Path (All selections valid, installation succeeds)
  Steps:
    1. VersionSelector: Select "2024"
    2. LanguageSelector: Select ["en-US"]
    3. AppExclusionSelector: Select ["OneDrive"]
    4. ConfigValidator: All steps pass
    5. InstallationExecutor: setup.exe succeeds
  Expected: $Config.state = "INSTALL_COMPLETE"

E2E-002: Error in UC-001 (Invalid version)
  Steps:
    1. VersionSelector: Select invalid version
  Expected: Error OFF-CONFIG-001, state = SELECT_VERSION

E2E-003: Error in UC-004 (Validation timeout)
  Steps:
    1-3: Valid selections
    4. ConfigValidator: Timeout occurs
  Expected: Error OFF-SYSTEM-201, state = remains validation

E2E-004: Error in UC-005 (Installation fails, rollback succeeds)
  Steps:
    1-4: Valid selections and validation
    5. InstallationExecutor: setup.exe fails
    6. RollbackExecutor: All 3 parts succeed
  Expected: $Config.state = "ROLLED_BACK"

E2E-005: Error in UC-005 (Installation fails, rollback fails)
  Steps:
    1-5: As E2E-004
    6. RollbackExecutor: Part 1 fails (can't remove files)
  Expected: $Config.state = "INSTALL_FAILED", OFF-ROLLBACK-501
```

---

## 5. Daily Timeline (Week 5)

```
DAY 1 (Monday)
  09:00-09:30: Standup + context setting
  09:30-12:00: Class 1 (Configuration) + Class 2 start (StateMachine)
  13:00-16:00: Class 2 (StateMachine) continuation
  Unit tests for Class 1-2 written
  DONE: Configuration + 50% StateMachine

DAY 2 (Tuesday)
  09:00-12:00: Class 2 (StateMachine) completion + Class 3 (ErrorHandler) start
  13:00-16:00: Class 3 (ErrorHandler) continuation
  Unit tests for Class 2-3 written
  DONE: StateMachine + ErrorHandler complete

DAY 3 (Wednesday)
  09:00-12:00: Class 4 (VersionSelector)
  13:00-16:00: Class 5 (LanguageSelector)
  Unit tests for Class 4-5
  DONE: Both Selectors complete

DAY 4 (Thursday)
  09:00-12:00: Class 6 (AppExclusionSelector)
  13:00-16:00: Integration tests (Selectors together)
  DONE: All 3 Selectors + UC-001/002/003 integration verified

DAY 5 (Friday)
  09:00-12:00: Class 7 (ConfigGenerator)
  13:00-16:00: Class 8 (ConfigValidator) start
  Unit tests for Class 7-8 start
  DONE: ConfigGenerator + 50% ConfigValidator

DAY 6 (Monday Week 2)
  09:00-12:00: Class 8 (ConfigValidator) completion
  13:00-16:00: Integration tests (Validators + Selectors)
  DONE: ConfigValidator + UC-004 integration verified

DAY 7 (Tuesday)
  09:00-12:00: Class 9 (InstallationExecutor) Part 1
  13:00-16:00: Class 9 (InstallationExecutor) Part 2
  Unit tests for Class 9 start
  DONE: 50% InstallationExecutor

DAY 8 (Wednesday)
  09:00-12:00: Class 9 completion + Class 10 (RollbackExecutor)
  13:00-16:00: E2E testing (all 5 UCs), final integration
  DONE: All 10 classes complete, all tests passing

TOTAL: 40 hours, 8 working days
AVAILABLE: 5 hours per day (40 hours / 8 days)
```

---

## 6. Success Criteria

### Stage 10 Completion Definition

```
✓ CLASSES: All 10 classes implemented
  • Configuration: Property structure complete
  • StateMachine: 11 states, transitions working
  • ErrorHandler: 19 codes, retry logic working
  • 3 Selectors: UC-001/002/003 complete
  • 2 Validators: UC-004 complete
  • 2 Executors: UC-005 complete

✓ UNIT TESTS: 100% passing
  • Each class: Unit tests passing
  • Coverage: All public methods tested
  • Edge cases: Error conditions tested

✓ INTEGRATION TESTS: 100% passing
  • UC boundaries: Data flow verified
  • State transitions: All valid transitions work
  • Error paths: Error conditions routed correctly

✓ E2E TESTS: 100% passing
  • Happy path: INIT → INSTALL_COMPLETE
  • Error paths: All error scenarios tested
  • Rollback: Atomic 3-part rollback verified

✓ PERFORMANCE: Targets met
  • UC-004 validation: <1000ms
  • UC-005 installation: <20 minutes
  • No memory leaks
  • No blocking operations

✓ CODE QUALITY: Standards maintained
  • No duplicate code
  • Consistent naming (from ADRs)
  • Clear comments
  • No technical debt
```

---

## 7. Risk Mitigation

### Implementation Risks

```
RISK: Class 8 (ConfigValidator) takes longer than estimated (4-5 hours)
  → Mitigation: Start Day 5 morning, Day 6 available for overflow
  
RISK: Unit test writing takes longer than implementation
  → Mitigation: Tests written in parallel, not after
  → Each day: Implementation + tests for that day's classes

RISK: Integration issues between classes
  → Mitigation: Integration tests start Day 4, issues found early
  → Can adjust later classes if needed

RISK: Timeout/performance issues in UC-004 or UC-005
  → Mitigation: Implement Stopwatch monitoring, test timeout logic early
  → Day 5-6 focus on validation timeout testing
```

---

## Document Metadata

```
Created: 2026-05-16 18:45
Task: T-031 Stage 10 Implementation Plan
Version: 1.0.0
Story Points: 5
Duration: 8 working days
Status: IN PROGRESS
Effort: ~40 hours total (~5 hours/day)
Success Criteria: All 10 classes implemented, all tests passing
Quality Gate: 100% unit + integration + E2E test passage
```

---

**T-031 IN PROGRESS**

**Stage 10 implementation plan complete: 10 classes, 8-day timeline, all testing strategies defined ✓**

