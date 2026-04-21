```yml
created_at: 2026-04-28 10:00
document_type: Requirements Baseline
stage: Stage 7 - DESIGN/SPECIFY
work_package: 2026-04-21-06-15-00-design-specification-correct
phase: Phase 1 - Waterfall Cimentacion
version: 1.0.0-DRAFT
status: DRAFT (ready for T-007 consolidation, pending CP1 approval)
baseline_reference: Stage 1 UC-Matrix (use-case-matrix.md)
scope: v1.0.0 OfficeAutomator (FROZEN for Stage 7)
```

# REQUIREMENTS BASELINE v1.0.0 — OfficeAutomator

## Executive Summary

This document establishes the **requirements baseline** for OfficeAutomator v1.0.0, the scope that Stage 7 will design and Stage 10 will implement.

**v1.0.0 Scope:** 5 core use cases covering Office version selection, language selection, application exclusion, validation, and installation using Microsoft ODT.

**Supported Versions:** 2024, 2021, 2019
**Supported Languages (v1.0.0):** 2 languages (specific languages from Stage 6 SCOPE)
**Excluded from v1.0.0:** Custom XML editing, auto-updates, 4-language support (v1.1 roadmap)

This baseline is **FROZEN** after CP1 approval. Any changes require formal change control (CCB review).

---

## 1. Scope Statement

### v1.0.0 In Scope (Stage 7 Design + Stage 10 Implementation)

```
OfficeAutomator v1.0.0 is a Windows command-line tool that automates
Microsoft Office deployment. It guides users through configuration selection
(version, language, excluded applications), validates the configuration,
and executes installation using Microsoft Office Deployment Tool (ODT).

v1.0.0 includes:
  ✓ 5 use cases (UC-001 through UC-005)
  ✓ Version selection (2024, 2021, 2019)
  ✓ Language selection (2 languages, configured in Stage 6)
  ✓ Application exclusion (Teams, OneDrive, Groove, Lync, Bing)
  ✓ Integrity validation (SHA256 against Microsoft official hash)
  ✓ Installation execution with idempotent behavior
  ✓ Comprehensive error handling and logging
```

### v1.1 Out of Scope (Documented for Future Reference)

```
The following features are explicitly OUT OF SCOPE for v1.0.0:
  ✗ Custom XML editing UI
  ✗ Automatic OfficeAutomator updates
  ✗ Additional language support beyond 2 languages
  ✗ Graphical user interface (v1.0.0 is CLI only)
  ✗ Multi-machine deployment orchestration
  ✗ Reporting/dashboard features

These are documented in v1.1 Roadmap (Section 9) for stakeholder context.
CRITICAL: Items NOT listed in v1.0.0 scope = NOT in Stage 7 scope.
```

---

## 2. Functional Requirements — v1.0.0 (FROZEN)

All requirements derived from [Stage 1 UC-Matrix](../../../.thyrox/context/work/2026-04-21-01-30-00-uc-documentation/use-case-matrix.md).

### UC-001: Select Version

```
REQ-F-001: Version Selection UI
  Description: User (IT Admin) can select Office version from list
  Versions: 2024, 2021, 2019 (3 options, 1 must be selected)
  Mandatory: YES (selection required before proceeding)
  Validation: System validates selected version against whitelist
  State Persistence: Version selection persists for subsequent UCs
  
  Acceptance Criteria:
    ✓ 3 versions available for selection
    ✓ Selection interface clear (menu, dropdown, or equivalent)
    ✓ Validation against whitelist (data-structures-and-matrices.md)
    ✓ Error if invalid version selected
    ✓ Selection state persists in $Config object
  
  UC Reference: UC-001 (Stage 1)
  Complexity: Low
  Status: FROZEN for v1.0.0
```

### UC-002: Select Language

```
REQ-F-002: Language Selection UI
  Description: User can select 1 or more languages supported by version
  Supported Languages (v1.0.0): 2 languages (specific languages TBD in Stage 6)
  Availability: System shows only languages compatible with selected version
  Mandatory: YES (minimum 1 language required)
  Validation: Selected language validated against permitted list
  State Persistence: Selections persist for subsequent UCs
  
  Acceptance Criteria:
    ✓ Only languages compatible with selected version shown
    ✓ User can select 1 or more languages (minimum 1)
    ✓ Selection interface clear
    ✓ Validation against language whitelist
    ✓ Error if no language selected
    ✓ Language selections persist in $Config
  
  UC Reference: UC-002 (Stage 1)
  Complexity: Low
  Dependency: UC-001 must complete first
  Status: FROZEN for v1.0.0
```

### UC-003: Exclude Applications

```
REQ-F-003: Application Exclusion UI
  Description: User can exclude specific Microsoft Office applications
  Excluded Applications: Teams, OneDrive, Groove, Lync, Bing
  Default Exclusions: Teams, OneDrive (user can override)
  Optional: YES (0 or more applications can be excluded)
  Validation: Each exclusion validated against permitted exclusion list
  XML Generation: Exclusions translated to configuration.xml format
  
  Acceptance Criteria:
    ✓ All 5 exclusion options presented
    ✓ Teams and OneDrive checked by default
    ✓ User can override defaults
    ✓ Each exclusion validated
    ✓ configuration.xml generated with exclusions
    ✓ XML syntax valid (XSD validation)
    ✓ Exclusion selections persist in $Config
  
  UC Reference: UC-003 (Stage 1)
  Complexity: Medium (XML generation involved)
  Dependency: UC-002 must complete first
  Status: FROZEN for v1.0.0
```

### UC-004: Validate Configuration

```
REQ-F-004: Validate Configuration & Download Integrity
  Description: System validates configuration and downloaded Office package
  Validation Steps:
    1. Check version in whitelist (data-structures-and-matrices.md)
    2. Validate XML schema (XSD validation)
    3. Check language-version compatibility (compatibility matrix)
    4. Check app-version compatibility (exclusion matrix)
    5. Verify Microsoft official hash (SHA256)
    6. Perform Office Customization Tool (OCT) validation
    7. Check system requirements (admin rights, disk space)
    8. Final user authorization (confirm before proceeding)
  
  Retry Policy: Automatic retry up to 3 times on transient failures
  Backoff: Exponential backoff (2s, 4s, 6s between retries)
  Error Handling: Detailed error codes and messages
  Logging: All validation results logged with timestamps
  
  Acceptance Criteria:
    ✓ All 8 validation steps executed
    ✓ Each step produces pass/fail status
    ✓ Hash validation against Microsoft official hash
    ✓ Failed validation blocks UC-005
    ✓ Passed validation enables UC-005
    ✓ Retries succeed in transient failure scenarios
    ✓ Detailed logging of each step
    ✓ User receives clear pass/fail message
  
  UC Reference: UC-004 (Stage 1)
  Complexity: Medium (multiple validation checks)
  Criticality: CRITICAL — blocker for UC-005
  Dependency: UC-001, UC-002, UC-003 must complete first
  Status: FROZEN for v1.0.0
  
  Special Note: UC-004 MUST PASS for UC-005 to execute.
                If UC-004 FAILS, installation is blocked (safety measure).
```

### UC-005: Install Office

```
REQ-F-005: Execute Office Installation
  Description: System installs Microsoft Office using ODT
  Precondition: UC-004 validation MUST have PASSED
  Execution: Run setup.exe with generated configuration.xml
  Monitoring: System monitors installation progress
  Output Capture: All stdout and stderr captured
  Logging: Installation logs archived for troubleshooting
  
  Success Condition:
    Installation completes with exit code 0
    All requested components installed
    No errors in stderr
    Logs confirm successful completion
  
  Failure Condition:
    Non-zero exit code
    Any errors in stderr
    Installation incomplete
    Office already exists (see Idempotence)
  
  Idempotence:
    If UC-005 executed twice in succession:
      First execution: Full installation
      Second execution: Detects Office exists, returns success (no reinstall)
    
    Rationale: Provides safety if installation is retriggered
  
  Error Recovery:
    On failure: Capture error code and logs
    Optionally: Rollback changes (if disaster-recovery.md defines rollback)
    User notified: Clear error message with resolution guidance
  
  Acceptance Criteria:
    ✓ setup.exe executed with correct configuration.xml
    ✓ Installation progress monitored
    ✓ All stdout/stderr captured
    ✓ Exit code 0 on success
    ✓ Exit code !0 on failure
    ✓ Idempotent behavior on second execution
    ✓ Comprehensive logging
    ✓ Clear success/failure messaging
  
  UC Reference: UC-005 (Stage 1)
  Complexity: High (external process management, error handling)
  Criticality: Critical — primary goal of OfficeAutomator
  Dependency: UC-004 MUST PASS before this executes
  Status: FROZEN for v1.0.0
```

---

## 3. Non-Functional Requirements — v1.0.0 (FROZEN)

### Security Requirements

```
REQ-NF-SEC-001: Credential Handling
  Requirement: User authentication tokens stored encrypted in %APPDATA%
  Details:
    - Store token encrypted (algorithm: AES-256 recommended)
    - Token location: %APPDATA%\OfficeAutomator\
    - Never store plain-text passwords
    - Token lifetime: Session (cleared on exit)
  Status: FROZEN for v1.0.0
  Owner: architecture/security-architecture.md (T-012)
```

### Reliability Requirements

```
REQ-NF-REL-001: Rollback Capability
  Requirement: System can rollback Office installation if UC-005 fails
  Details:
    - Capture pre-installation system state
    - On failure: Attempt rollback to pre-installation state
    - Logging: Rollback actions logged for audit
    - Idempotence: Second UC-005 execution succeeds without errors
  Status: FROZEN for v1.0.0
  Owner: architecture/disaster-recovery.md (T-031)

REQ-NF-REL-002: Retry Strategy
  Requirement: Transient failures automatically retried
  Details:
    - UC-004 validation: Retry up to 3 times
    - Backoff: Exponential (2s, 4s, 6s)
    - Permanent failures: Return error immediately (no retries)
    - Logging: All retry attempts logged
  Status: FROZEN for v1.0.0
  Owner: design/error-propagation-strategy.md (T-020)
```

### Compliance & Audit Requirements

```
REQ-NF-AUDIT-001: Comprehensive Logging
  Requirement: All major actions logged for audit trail
  Details:
    - Version selection logged
    - Language selection logged
    - Application exclusions logged
    - All UC-004 validation steps logged with timestamps
    - UC-005 installation progress logged
    - Any errors/warnings logged
    - Log file location: %APPDATA%\OfficeAutomator\logs\
    - Log retention: 90 days minimum
  Status: FROZEN for v1.0.0
  Owner: design/logging-specification.md (T-030)

REQ-NF-AUDIT-002: User-Friendly Error Messages
  Requirement: Clear error messages for all failure scenarios
  Details:
    - Error message explains what failed
    - Error message suggests resolution
    - Error codes consistent and documented
    - No technical jargon in user-facing messages
  Status: FROZEN for v1.0.0
  Owner: design/error-propagation-strategy.md (T-020)
```

---

## 4. Data Requirements — v1.0.0 (FROZEN)

### Configuration Object ($Config)

```
REQ-DATA-CFG-001: Configuration Object Structure
  Definition: In-memory object holding user selections and state
  Properties:
    - Version: Selected Office version (string: "2024" | "2021" | "2019")
    - Languages: Array of selected languages
    - ExcludedApps: Array of excluded applications
    - State: Current state in workflow
    - ValidationPassed: Boolean (true if UC-004 passed)
    - ConfigPath: Path to generated configuration.xml
    - ODTPath: Path to Office Deployment Tool
  
  Lifecycle:
    - Created: At start of UC-001
    - Populated: Throughout UC-001 to UC-003
    - Validated: During UC-004
    - Used: During UC-005
    - Cleared: On successful completion or exit
  
  Status: FROZEN for v1.0.0
  Owner: design/state-management-design.md (T-019)
```

### Data Structures & Matrices

```
REQ-DATA-STRUCT-001: Version Whitelist
  Definition: Authorized list of supported Office versions
  Values: 2024, 2021, 2019
  Source: design/data-structures-and-matrices.md (T-006)
  Usage: Validate UC-001 version selection
  Status: FROZEN for v1.0.0

REQ-DATA-STRUCT-002: Language Compatibility Matrix
  Definition: Map of which languages are compatible with which versions
  Format: Version × Language matrix
  Values: 
    - v1.0.0 supports 2 languages (specific languages TBD in Stage 6)
    - Later versions may support 4+ languages (v1.1)
  Source: design/data-structures-and-matrices.md (T-006)
  Usage: Filter language options in UC-002 based on selected version
  Status: FROZEN for v1.0.0

REQ-DATA-STRUCT-003: Application Exclusion List
  Definition: Authorized list of applications allowed to be excluded
  Values: Teams, OneDrive, Groove, Lync, Bing
  Defaults: Teams and OneDrive (user can override)
  Source: design/data-structures-and-matrices.md (T-006)
  Usage: Validate UC-003 exclusion selections
  Status: FROZEN for v1.0.0

REQ-DATA-STRUCT-004: Microsoft Office Customization Tool (OCT) Schema
  Definition: XML schema (XSD) for valid configuration.xml
  Format: XML Schema Definition
  Source: design/data-structures-and-matrices.md (T-006)
  Usage: Validate UC-003 generated configuration.xml in UC-004
  Status: FROZEN for v1.0.0

REQ-DATA-STRUCT-005: Microsoft Official Hashes
  Definition: Official SHA256 hashes for Office packages
  Format: Hash mappings per version
  Source: Microsoft official documentation
  Usage: UC-004 validates downloaded package integrity
  Status: FROZEN for v1.0.0
  Reference: design/data-structures-and-matrices.md (T-006)
```

---

## 5. Use Case Dependencies

```
Execution Sequence (Mandatory):

UC-001 (Select Version)
   ↓ (must complete before)
UC-002 (Select Language)
   ↓ (must complete before)
UC-003 (Exclude Applications)
   ↓ (generates configuration.xml)
UC-004 (Validate Configuration) ← CRITICAL GATE
   ├─ If PASS → proceed to UC-005
   └─ If FAIL → block UC-005 (safety measure)
   ↓
UC-005 (Install Office)
   ↓
[Installation Complete or Failed]

Blocking Relationships:
  - UC-005 CANNOT execute if UC-004 FAILS
  - If UC-004 fails: User sees error, installation blocked
  - User can retry from UC-001 or UC-004 (TBD in error-propagation.md)
```

---

## 6. Acceptance Criteria for Stage 7 Design Completion

Stage 7 design is COMPLETE when ALL of the following are TRUE:

```
REQUIREMENT COVERAGE:
  ☑ All 5 functional requirements (REQ-F-001 to REQ-F-005) addressed in UCs
  ☑ All non-functional requirements addressed in architecture
  ☑ All data requirements defined (structures, matrices, hashes)
  ☑ No requirements missing or undefined

STATE & TRANSITIONS:
  ☑ $Config state machine defined (all valid states documented)
  ☑ Valid transitions documented (UC-001 → UC-002 → ...)
  ☑ Error transitions documented (failure states, recovery paths)
  ☑ Idempotence defined for UC-005

SECURITY & SAFETY:
  ☑ Security architecture complete (credential handling, logging, audit)
  ☑ Threat model documented (architecture/security-architecture.md)
  ☑ UC-004 blocker behavior clear (prevents UC-005 on failure)
  ☑ Rollback strategy defined (disaster-recovery.md)

VALIDATION & ERROR HANDLING:
  ☑ All 8 UC-004 validation steps specified
  ☑ Error scenarios documented (what can go wrong)
  ☑ Error recovery paths defined (retry, rollback, user notification)
  ☑ Logging strategy complete (what, where, how long)
  ☑ User-facing error messages drafted

TESTING & ACCEPTANCE:
  ☑ Acceptance criteria defined for each UC
  ☑ Testing strategy outlined
  ☑ Success/failure criteria clear
  ☑ Ready for Stage 10 Implementation
```

---

## 7. Version & Scope Control

### Versioning

```
Baseline Version: 1.0.0 (FROZEN after CP1 approval)
Format: MAJOR.MINOR.PATCH
  - MAJOR: Scope changes (new UCs, removed features)
  - MINOR: Requirements clarifications (same UCs, different behavior)
  - PATCH: Typos, document corrections (no functional changes)

Change Trigger: Any change to v1.0.0 scope requires:
  1. Issue identification (describe change)
  2. Impact analysis (what else affected)
  3. CCB review (Change Control Board approval)
  4. Nestor approval (Product Owner sign-off)
  5. All linked documents updated
  6. Version bumped (1.0.0 → 1.0.1 or 1.1.0 as appropriate)
```

### Approval & Freeze

```
Current Status: 1.0.0-DRAFT
  - Ready for consolidation (T-007)
  - Pending CP1 approval gate

Target Status: 1.0.0-APPROVED & FROZEN
  - After CP1 gate (Day 5, Friday)
  - Baseline locked for Stage 7 design
  - No changes without formal CCB
```

---

## 8. References

### Stage 1 (DISCOVER)

```
[UC-Matrix from Stage 1](../../../.thyrox/context/work/2026-04-21-01-30-00-uc-documentation/use-case-matrix.md)
  - 5 UCs identified and dependencies mapped
  - Acceptance criteria (initial)
  - Risk register (initial)
```

### Stage 6 (SCOPE) — TBD

```
The following scope items are TBD in Stage 6:
  - Specific 2 languages for v1.0.0
  - Detailed version compatibility matrix
  - Detailed language × version matrix
```

### Stage 7 (DESIGN/SPECIFY) — This Stage

```
This baseline document links to Stage 7 deliverables:
  - [SKILL rm-elicitation]: /tmp/projects/.../rm-elicitation/SKILL.md (guidance)
  - [T-006]: design/data-structures-and-matrices.md (data details)
  - [T-012]: architecture/security-architecture.md (security design)
  - [T-019]: design/state-management-design.md (state machine)
  - [T-020]: design/error-propagation-strategy.md (error handling)
  - [T-030]: design/logging-specification.md (logging details)
  - [T-031]: architecture/disaster-recovery.md (rollback design)
```

### External References

```
Microsoft Office Deployment Tool (ODT):
  - Official documentation (Microsoft Docs)
  - ODT schema and configuration options
  
Stage 10+ Implementation:
  - Will implement all 5 UCs per Stage 7 specifications
  - Will validate against this baseline
```

---

## 9. v1.1 Roadmap (Out of Scope, Reference Only)

Future enhancements documented for stakeholder context:

```
ROADMAP ITEM R1-001: Custom XML Configuration Editor
  Description: Allow power users to manually edit configuration.xml
  Rationale: Advanced scenarios requiring fine-grained control
  Complexity: High (UI design, validation, error handling)
  Scope: v1.1 (NOT v1.0.0)
  Status: FUTURE — documented for planning purposes

ROADMAP ITEM R1-002: Automatic OfficeAutomator Updates
  Description: Self-update mechanism for OfficeAutomator
  Rationale: Keep tool current with new Office versions/bug fixes
  Complexity: High (version management, rollback)
  Scope: v1.1 (NOT v1.0.0)
  Status: FUTURE — documented for planning purposes

ROADMAP ITEM R1-003: 4+ Language Support
  Description: Extend beyond 2 languages to 4+ in v1.0.0
  Rationale: Global deployment requirements
  Complexity: Medium (matrix expansion, testing)
  Scope: v1.1 (NOT v1.0.0)
  Status: FUTURE — v1.0.0 supports 2 languages only

ROADMAP ITEM R1-004: Graphical User Interface (GUI)
  Description: Replace CLI with Windows GUI
  Rationale: Non-technical users, easier experience
  Complexity: Very High (Windows Forms or WPF)
  Scope: v1.1 (NOT v1.0.0)
  Status: FUTURE — v1.0.0 is CLI only

ROADMAP ITEM R1-005: Multi-Machine Orchestration
  Description: Deploy to multiple machines from single control point
  Rationale: Enterprise deployment at scale
  Complexity: Very High (client-server, coordination)
  Scope: v1.1 (NOT v1.0.0)
  Status: FUTURE — v1.0.0 is single-machine only

CRITICAL: These v1.1 items are OUT OF SCOPE for v1.0.0.
          Do NOT design for them in Stage 7.
          If v1.1 features are requested in Stage 7: REJECT (out of scope).
```

---

## 10. Approval & Sign-Off

### Baseline Approval

```
Document: Requirements Baseline v1.0.0
Status: DRAFT (as of creation 2026-04-28)
Pending: CP1 Approval Gate (Friday, Day 5)

Approvals Required:
  ☐ Product Owner (Nestor):      [PENDING after T-007 consolidation]
  ☐ Architect (Claude):           [Will verify completeness in T-007]

Approval Criteria:
  ✓ No ambiguities (all requirements clear)
  ✓ All 5 UCs covered (UC-001 through UC-005)
  ✓ Data structures defined (structures-matrices.md created in T-006)
  ✓ v1.0.0 vs v1.1 clearly separated (no scope creep)
  ✓ Acceptance criteria testable
  ✓ Ready for Stage 7 design

Sign-Off After CP1:
  Version: 1.0.0-APPROVED & FROZEN
  Date: [After Friday Day 5, 2026-04-28]
  Locked: No changes without formal change control
```

---

## Document History

```
Version  Date                 Status    Notes
1.0.0    2026-04-28 10:00    DRAFT     Initial baseline created (T-002)
1.0.0    2026-04-28 13:00    DRAFT     Pending T-007 consolidation
1.0.0    2026-05-03 10:00    APPROVED  After CP1 gate (expected)
```

---

**END REQUIREMENTS BASELINE v1.0.0**

Created: 2026-04-28 10:00
Task: T-002 (rm-requirements-baseline.md DRAFT)
Ready For: T-007 (consolidation) → T-008 (CP1 approval)

