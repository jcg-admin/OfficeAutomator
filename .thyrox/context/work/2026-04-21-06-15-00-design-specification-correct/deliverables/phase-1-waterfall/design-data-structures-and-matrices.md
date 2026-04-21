```yml
created_at: 2026-05-01 09:00
document_type: Data Structures & Matrices Design
stage: Stage 7 - DESIGN/SPECIFY
work_package: 2026-04-21-06-15-00-design-specification-correct
phase: Phase 1 - Waterfall Cimentacion
task_id: T-006
task_name: design/data-structures-and-matrices.md DRAFT
execution_date: 2026-05-01 09:00-12:00, 14:00-15:00 (Thursday, Week 1, Day 4)
duration_hours: 6
duration_estimate: Exact (structure design + matrix creation)
roles_involved: BA (Claude) + ARCHITECT (Claude)
dependencies: T-005 (clarifications), T-002 (baseline)
data_structures_defined: 8
matrices_created: 5
acceptance_criteria:
  - All data structures defined (objects, enums, constants)
  - All matrices created (version×language, app×version, etc)
  - Microsoft XSD reference documented
  - Error code catalog complete
  - Logging redaction rules detailed
  - Ready for T-007 consolidation
status: DRAFT (ready for consolidation)
version: 1.0.0-DRAFT
```

# DESIGN: DATA STRUCTURES & MATRICES

## Overview

This document defines the core data structures, matrices, and reference data that Stage 7 design and Stage 10 implementation will use. All structures derived from requirements clarifications (T-002, T-004, T-005).

**Source:** Requirements baseline (T-002), Stakeholder analysis (T-004), Clarifications (T-005)
**Scope:** v1.0.0 only (v1.1 expansions documented as future)
**Format:** Pseudocode structures, matrix definitions, and reference tables

---

## 1. Core Data Structures

### 1.1 Configuration Object ($Config)

```
Object: Configuration
  Properties:
    - version: string
        Values: "2024" | "2021" | "2019"
        Required: YES (enforced in UC-001)
        Source: T-005 Clarification 1
        Default: null
    
    - languages: string[]
        Values: ["en-US", "es-MX"] (v1.0.0)
        Required: YES (at least 1 language, enforced in UC-002)
        Source: T-005 Clarification 1
        Default: []
    
    - excludedApps: string[]
        Values: ["Teams", "OneDrive", "Groove", "Lync", "Bing"]
        Required: NO (0 or more apps, set in UC-003)
        Source: T-005 Clarification 2
        Default: ["Teams", "OneDrive"] (hardcoded defaults)
    
    - configPath: string
        Values: Full path to generated configuration.xml
        Required: YES (after UC-003)
        Default: null
        Format: "C:\ProgramData\OfficeAutomator\config_{timestamp}.xml"
    
    - validationPassed: boolean
        Values: true | false
        Required: YES (after UC-004)
        Default: false
        Blocker: UC-005 cannot execute if false
    
    - odtPath: string
        Values: Full path to Office Deployment Tool
        Required: YES (resolved before UC-005)
        Default: null
        Source: Microsoft ODT installation
    
    - state: enum
        Values: INIT | SELECT_VERSION | SELECT_LANGUAGE | SELECT_APPS | 
                GENERATE_CONFIG | VALIDATE | INSTALL_READY | INSTALLING | 
                INSTALL_COMPLETE | INSTALL_FAILED | ROLLED_BACK
        Required: YES (state machine)
        Default: INIT
        Source: T-019 (state machine design)
    
    - timestamp: datetime
        Values: ISO 8601 format
        Required: YES
        Default: Current time at initialization

Lifecycle:
  - INIT: $Config created
  - UC-001: version set
  - UC-002: languages set
  - UC-003: excludedApps set, configuration.xml generated (configPath set)
  - UC-004: validationPassed set to true/false
  - UC-005: state transitions INSTALL_READY → INSTALLING → INSTALL_COMPLETE or INSTALL_FAILED
  - UC-005 failure: Rollback triggered, state = ROLLED_BACK

State Machine:
  INIT → SELECT_VERSION → SELECT_LANGUAGE → SELECT_APPS → GENERATE_CONFIG 
  → VALIDATE (UC-004) → INSTALL_READY → INSTALLING → INSTALL_COMPLETE
  
  Error branch:
  VALIDATE (fail) → Return error, stay in VALIDATE (user can retry)
  INSTALLING (fail) → Rollback triggered, state = ROLLED_BACK
```

### 1.2 Error Result Object

```
Object: ErrorResult
  Properties:
    - code: string
        Format: "OFF-{Category}-{Number}"
        Required: YES
        Source: T-005 Clarification 7
        Example: "OFF-NETWORK-301"
    
    - category: enum
        Values: CONFIG | SECURITY | SYSTEM | NETWORK | INSTALL | ROLLBACK
        Required: YES
        Source: T-005 Clarification 7
    
    - isTransient: boolean
        Values: true (retry-able) | false (permanent)
        Required: YES
        Default: false
        Source: T-005 Clarification 3
    
    - shortDescription: string
        Required: YES
        Example: "Network timeout during hash validation"
        Format: One sentence, user-friendly
    
    - technicalDetails: string
        Required: NO (may be empty for user-facing errors)
        Example: "TCP timeout after 30 seconds connecting to download.microsoft.com"
    
    - suggestedAction: string
        Required: YES
        Format: "Check internet connection and try again" or "Contact IT Help Desk"
    
    - timestamp: datetime
        Required: YES
        Format: ISO 8601
    
    - retryCount: integer
        Values: 0 (no retries attempted) or 1-3 (retries made)
        Required: NO
        Source: T-005 Clarification 3

Example (user sees):
  ErrorResult {
    code: "OFF-NETWORK-301",
    shortDescription: "Network timeout during hash validation",
    suggestedAction: "Check internet connection and try again"
  }

Example (IT Admin sees in logs):
  ErrorResult {
    code: "OFF-NETWORK-301",
    shortDescription: "Network timeout during hash validation",
    technicalDetails: "TCP timeout after 30s to download.microsoft.com:443",
    suggestedAction: "Check firewall rules, retry when network stable",
    retryCount: 3,
    timestamp: "2026-05-01T10:15:32Z"
  }
```

### 1.3 ValidationResult Object

```
Object: ValidationResult
  Properties:
    - passed: boolean
        Values: true | false
        Required: YES
        Logic: ALL validation steps must pass for overall PASS
    
    - steps: ValidationStep[]
        Required: YES
        Contains: 8 validation steps (see Section 2: Validation Matrix)
    
    - failedStepIndex: integer
        Values: -1 (all passed) or 0-7 (index of failed step)
        Default: -1
        Required: YES
    
    - failureReason: string
        Required: NO (only if passed=false)
    
    - duration: integer
        Values: Milliseconds (< 1000 ms baseline)
        Required: YES
        Source: T-005 Clarification 3
    
    - timestamp: datetime
        Required: YES

ValidationStep:
  - stepNumber: integer (0-7)
  - name: string (human-readable)
  - status: enum (PASS | FAIL | SKIP | RETRY)
  - duration: integer (milliseconds)
  - message: string (result details)
  - retryAttempts: integer (0 if not retried)
```

### 1.4 InstallationResult Object

```
Object: InstallationResult
  Properties:
    - succeeded: boolean
        Values: true | false
        Required: YES
    
    - exitCode: integer
        Values: 0 (success) or non-zero (failure code)
        Required: YES
        Source: Microsoft ODT documentation
    
    - componentsInstalled: string[]
        Example: ["Excel", "Word", "PowerPoint"]
        Required: YES (even on failure, logs partial installs)
    
    - duration: integer
        Values: Seconds
        Required: YES
    
    - errorOutput: string
        Required: NO (only if succeeded=false)
        Contains: stdout/stderr from setup.exe
    
    - idempotenceApplied: boolean
        Values: true (Office already existed, skipped install) | false (installed fresh)
        Default: false
        Source: T-005 Clarification 6
    
    - rollbackApplied: boolean
        Values: true (failure triggered rollback) | false (no rollback needed)
        Default: false
        Source: T-005 Clarification 4
    
    - rollbackSuccessful: boolean
        Values: true | false (if rollbackApplied=true)
        Required: NO (only if rollback attempted)
```

### 1.5 LogEntry Object

```
Object: LogEntry
  Properties:
    - timestamp: datetime (ISO 8601)
    - level: enum (INFO | WARN | ERROR)
    - component: string (UC-001, UC-002, ..., UC-005)
    - message: string (log message)
    - data: object (optional structured data)
      - tier: enum (FULL | REDACTED)
        Source: T-005 Clarification 8
        Values:
          FULL: Contains all data (IT Admin only)
          REDACTED: Sensitive data masked (Support team)
      
    - redactionRules: array (if tier=REDACTED)
        Applied redactions:
          - Tokens replaced with [REDACTED_TOKEN]
          - Hashes replaced with [REDACTED_HASH]
          - Passwords replaced with [REDACTED_PASSWORD]
    
    - source: string (machine name where log originated)
    - userId: string (Windows user who triggered action)
    - sessionId: string (unique session identifier)

Storage:
  Tier 1 (FULL): %APPDATA%\OfficeAutomator\logs\{machine}_{date}.log
  Tier 2 (REDACTED): %APPDATA%\OfficeAutomator\logs\redacted\{machine}_{date}_redacted.log
  
  Format: UTF-8 text, one LogEntry per line, JSON format
  Retention: 90 days minimum
  Source: T-005 Clarification 8
```

---

## 2. Validation Step Definitions (UC-004)

```
UC-004 executes 8 validation steps (sequential):

Step 0: Version Whitelist Check
  Input: $Config.version
  Check: Is version in [2024, 2021, 2019]?
  Pass: Version found in whitelist
  Fail: Version not in whitelist → OFF-CONFIG-001
  Duration: ~10ms
  Source: T-005 Clarification 5

Step 1: XML Schema Validation
  Input: Generated configuration.xml
  Check: XML valid per Microsoft official XSD?
  Pass: XML structure correct
  Fail: XML malformed → OFF-CONFIG-004
  Duration: ~50ms
  Source: T-005 Clarification 5
  Schema Reference: Microsoft Office Customization Tool XSD
    URL: https://docs.microsoft.com/en-us/deployoffice/

Step 2: Language-Version Compatibility Check
  Input: $Config.version, $Config.languages
  Check: All selected languages compatible with version?
  Reference: Language×Version Matrix (Section 3.2)
  Pass: All languages valid for version
  Fail: Language not supported for version → OFF-CONFIG-002
  Duration: ~10ms

Step 3: Application-Version Compatibility Check
  Input: $Config.version, $Config.excludedApps
  Check: All excluded apps compatible with version?
  Reference: Application×Version Matrix (Section 3.3)
  Pass: All exclusions valid for version
  Fail: App not compatible → OFF-CONFIG-003
  Duration: ~10ms

Step 4: Hash Validation (Microsoft Official)
  Input: Downloaded Office package file
  Action: Calculate SHA256 of file
  Check: Does hash match Microsoft official hash?
  Reference: Microsoft Official Hash List (Section 3.4)
  Pass: Hash matches (package integrity verified)
  Fail: Hash mismatch → OFF-SECURITY-102 (package corrupted)
  Duration: ~500ms (file I/O dependent)
  Retry: Up to 3 times on transient failure (network timeout)
    Backoff: 2s, 4s, 6s between retries
  Source: T-005 Clarification 3
  Transient Failure: Network timeout, service unavailable (retryable)
  Permanent Failure: Hash mismatch (download corrupted, don't retry)

Step 5: Office Customization Tool (OCT) Validation
  Input: configuration.xml
  Action: Run OCT validation against config
  Check: OCT accepts configuration (syntax & rules)?
  Pass: Configuration valid per OCT
  Fail: OCT validation failed → OFF-CONFIG-004
  Duration: ~100ms
  Note: OCT may have additional rules beyond XSD

Step 6: System Requirements Check
  Input: Target machine state
  Check 6a: Administrator privilege available?
    Fail → OFF-SECURITY-101
  Check 6b: Sufficient disk space (> 4GB free)?
    Fail → OFF-SYSTEM-201
  Check 6c: Office not already installed?
    If installed: Set idempotenceApplied=true, return PASS
    Source: T-005 Clarification 6
  Duration: ~50ms

Step 7: User Authorization (Confirm)
  Input: User interaction
  Action: Display summary, ask user to confirm
  Check: User clicks "Proceed" or "Cancel"?
  Pass: User confirmed
  Fail: User cancelled → Cancel installation (not an error)
  Duration: ~0ms (user time)

Total Duration: < 1000ms (< 1 second)
Source: T-005 Clarification 3
```

---

## 3. Reference Matrices

### 3.1 Version Whitelist

```
Supported Office Versions (v1.0.0):

| Version | Release Year | Status | Support |
|---------|------------|--------|---------|
| 2024    | 2023       | Current | ✓ v1.0.0 |
| 2021    | 2020       | Mainstream | ✓ v1.0.0 |
| 2019    | 2018       | Extended | ✓ v1.0.0 |
| 2016    | 2015       | Out of Support | ✗ v1.1+ |
| 365     | Ongoing    | Cloud-only | ✗ Not supported |

v1.0.0 Constraint: Only 2024, 2021, 2019
v1.1 Roadmap: May add 2016 or 365 in future

Validation Rule (UC-004 Step 0):
  IF version NOT IN [2024, 2021, 2019]:
    RETURN ERROR OFF-CONFIG-001
```

### 3.2 Language × Version Compatibility Matrix

```
Language Support by Office Version (v1.0.0):

| Language | Code | 2024 | 2021 | 2019 |
|----------|------|------|------|------|
| English (US) | en-US | ✓ | ✓ | ✓ |
| Spanish (Mexico) | es-MX | ✓ | ✓ | ✓ |
| [v1.1: Portuguese] | pt-BR | ✗ | ✗ | ✗ |
| [v1.1: French] | fr-FR | ✗ | ✗ | ✗ |

v1.0.0 Constraint: Only en-US and es-MX
v1.1 Roadmap: Expand to 4 languages

Validation Rule (UC-004 Step 2):
  FOR EACH language IN $Config.languages:
    IF NOT compatibility_matrix[version][language]:
      RETURN ERROR OFF-CONFIG-002
```

### 3.3 Application × Version Compatibility Matrix

```
Excluded Applications by Office Version (v1.0.0):

| Application | Code | 2024 | 2021 | 2019 | Default Exclude |
|-------------|------|------|------|------|-----------------|
| Teams | Teams | ✓ | ✓ | ✓ | YES |
| OneDrive | OneDrive | ✓ | ✓ | ✓ | YES |
| Groove | Groove | ✓ | ✓ | ✓ | NO |
| Lync | Lync | ✓ | ✓ | ✓ | NO |
| Bing | Bing | ✓ | ✓ | ✓ | NO |

Default Exclusion Rules:
  By default, UC-003 pre-selects: Teams, OneDrive
  User can override defaults (add/remove exclusions)

Validation Rule (UC-004 Step 3):
  FOR EACH app IN $Config.excludedApps:
    IF NOT compatibility_matrix[version][app]:
      RETURN ERROR OFF-CONFIG-003

XML Translation (UC-003):
  Each excluded app translated to ODT syntax:
    Teams → <ExcludeApp ID="Teams"/>
    OneDrive → <ExcludeApp ID="OneDrive"/>
    (etc, per Microsoft ODT documentation)
```

### 3.4 Microsoft Official Hash Reference

```
Office Package Hashes (SHA256) — v1.0.0

Format: version | language | excluded_apps_hash | official_sha256

Example:
  2024 | en-US | {no exclusions} | 7e49f8a0c4d3b1e2f5a6c9d0e1f2a3b4...
  2024 | es-MX | {no exclusions} | a1b2c3d4e5f6a7b8c9d0e1f2a3b4c5d6...
  2021 | en-US | {Teams excluded} | f5f6a7b8c9d0e1f2a3b4c5d6e7f8a9b0...
  2021 | es-MX | {Teams+OneDrive} | c5d6e7f8a9b0c1d2e3f4a5b6c7d8e9f0...

Source:
  Downloaded from Microsoft official servers during UC-004
  Cached locally for offline validation (optional)
  
Updated: When new Office versions released

Validation Rule (UC-004 Step 4):
  hash_calculated = SHA256(downloaded_package_file)
  IF hash_calculated != official_hash_reference:
    IF network_available:
      RETRY up to 3 times (exponential backoff)
    ELSE:
      RETURN ERROR OFF-SECURITY-102 (permanent failure)
```

### 3.5 Installation Idempotence Detection

```
Office Installation Existence Check (UC-004 Step 6, UC-005 Entry):

Registry Key Checks:
  
  Key 1: HKEY_LOCAL_MACHINE\Software\Microsoft\Office\RegistrationDB
    Presence: Indicates Office installed
    Version info: If present, extract version
  
  Key 2: HKEY_LOCAL_MACHINE\Software\Microsoft\Office\{version}\FirstRun
    Presence: Indicates successful Office installation for version
  
  File Path Checks:
    C:\Program Files\Microsoft Office\Office16\WINWORD.EXE (2019)
    C:\Program Files\Microsoft Office\Office17\WINWORD.EXE (2021)
    C:\Program Files\Microsoft Office\Office18\WINWORD.EXE (2024)

Idempotence Logic (UC-005):
  
  IF (Office registry keys present) AND (Office version == $Config.version):
    // Office already installed, same version
    InstallationResult.idempotenceApplied = true
    RETURN SUCCESS (no-op, skip setup.exe)
  
  ELSE IF (Office registry keys present) AND (Office version != $Config.version):
    // Different version already installed
    RETURN ERROR OFF-SYSTEM-202 ("Different Office version installed")
    User must uninstall old version first
  
  ELSE:
    // Office not installed, proceed with installation
    EXECUTE setup.exe with configuration.xml
    InstallationResult.idempotenceApplied = false

Source: T-005 Clarification 6
```

---

## 4. Error Code Catalog

### 4.1 Configuration Errors (OFF-CONFIG-*)

```
OFF-CONFIG-001: Invalid Office version selected
  Category: Configuration
  Isansient: false (permanent)
  When: UC-004 Step 0 (version whitelist check fails)
  User Message: "Office version '{version}' not supported. Use 2024, 2021, or 2019."
  Action: "Select a supported version and try again"

OFF-CONFIG-002: Language not supported for version
  Category: Configuration
  Transient: false (permanent)
  When: UC-004 Step 2 (language-version compatibility fails)
  User Message: "Language '{language}' not available for Office {version}"
  Action: "Select a compatible language"

OFF-CONFIG-003: Application exclusion not valid
  Category: Configuration
  Transient: false (permanent)
  When: UC-004 Step 3 (app-version compatibility fails)
  User Message: "Cannot exclude '{app}' from Office {version}"
  Action: "Select a different application to exclude"

OFF-CONFIG-004: Configuration XML validation failed
  Category: Configuration
  Transient: false (permanent)
  When: UC-004 Step 1 or 5 (XML schema or OCT validation fails)
  User Message: "Configuration invalid. Contact IT Help Desk."
  Action: "Contact IT Help Desk with error code OFF-CONFIG-004"
  Technical: Details from XML parser (schema error)
```

### 4.2 Security Errors (OFF-SECURITY-*)

```
OFF-SECURITY-101: Administrator rights required
  Category: Security
  Transient: false (permanent)
  When: UC-004 Step 6 (admin privilege check fails)
  User Message: "Administrator privileges required to install Office"
  Action: "Run as Administrator or contact IT Help Desk"

OFF-SECURITY-102: Hash validation failed (package corrupted)
  Category: Security
  Transient: true (will retry 3x automatically)
  When: UC-004 Step 4 (hash mismatch after retries)
  User Message: "Downloaded package appears corrupted. Please try again."
  Action: "Check internet connection and try again"
  Technical: "Expected hash: [REDACTED_HASH], Got: [REDACTED_HASH]"
  Note: Sensitive hash values redacted in logs (T-005 Clarification 8)

OFF-SECURITY-103: Unauthorized modification detected
  Category: Security
  Transient: false (permanent)
  When: UC-004 (if tamper detection implemented, future)
  User Message: "Package appears modified. Cannot proceed."
  Action: "Re-download from official source"
```

### 4.3 System Errors (OFF-SYSTEM-*)

```
OFF-SYSTEM-201: Insufficient disk space
  Category: System
  Transient: false (permanent, until user frees space)
  When: UC-004 Step 6 (disk space check fails)
  User Message: "Not enough disk space. At least 4 GB free required."
  Action: "Free up disk space and try again"

OFF-SYSTEM-202: Different Office version already installed
  Category: System
  Transient: false (permanent, user action needed)
  When: UC-005 entry (idempotence check, version mismatch)
  User Message: "Office {existing_version} is installed. Uninstall first to upgrade."
  Action: "Uninstall existing Office version or select matching version"

OFF-SYSTEM-203: Required component missing
  Category: System
  Transient: false (permanent)
  When: UC-004 Step 6 (if critical component missing)
  User Message: "Required system component not found"
  Action: "Contact IT Help Desk"
```

### 4.4 Network Errors (OFF-NETWORK-*) — Transient, Retried

```
OFF-NETWORK-301: Network timeout
  Category: Network
  Transient: true (WILL RETRY 3x with backoff)
  When: UC-004 Step 4 (hash download timeout)
  User Message: "Network timeout. Retrying... (attempt {n}/3)"
  Action: "Check internet connection. System will retry automatically."
  Backoff: 2s, 4s, 6s between retries

OFF-NETWORK-302: Connection refused
  Category: Network
  Transient: true (WILL RETRY 3x with backoff)
  When: UC-004 Step 4 (firewall blocks connection)
  User Message: "Cannot reach Microsoft servers. Retrying..."
  Action: "Check firewall rules. System will retry automatically."

OFF-NETWORK-303: Service temporarily unavailable
  Category: Network
  Transient: true (WILL RETRY 3x with backoff)
  When: UC-004 Step 4 (Microsoft service down)
  User Message: "Microsoft servers temporarily unavailable. Retrying..."
  Action: "System will retry automatically. Please wait."
```

### 4.5 Installation Errors (OFF-INSTALL-*)

```
OFF-INSTALL-401: Setup.exe failed
  Category: Installation
  Transient: false (permanent, requires investigation)
  When: UC-005 (setup.exe returns non-zero exit code)
  User Message: "Office installation failed. See logs for details."
  Action: "Contact IT Help Desk with error code OFF-INSTALL-401"
  Technical: "Exit code: {code}, Output: [logs]"
  Rollback: Attempted automatically

OFF-INSTALL-402: Installation interrupted
  Category: Installation
  Transient: true (user can retry)
  When: UC-005 (user cancels, process killed, machine restart)
  User Message: "Installation was interrupted. You can retry."
  Action: "Retry installation or contact IT Help Desk"

OFF-INSTALL-403: Unknown installation error
  Category: Installation
  Transient: false (permanent, requires investigation)
  When: UC-005 (unexpected error not caught by other handlers)
  User Message: "Unexpected error during installation"
  Action: "Contact IT Help Desk with error code OFF-INSTALL-403"
```

### 4.6 Rollback Errors (OFF-ROLLBACK-*)

```
OFF-ROLLBACK-501: Rollback started
  Category: Rollback
  Transient: false (informational, not an error)
  When: UC-005 failure triggers rollback
  User Message: "Installation failed. Removing partial installation..."
  Action: "Wait for rollback to complete"
  Status: "Rollback in progress"

OFF-ROLLBACK-502: Rollback partially succeeded
  Category: Rollback
  Transient: false (warning)
  When: Rollback succeeds but some files remain
  User Message: "Some installation files could not be removed. Partial cleanup."
  Action: "Contact IT Help Desk or perform manual cleanup"

OFF-ROLLBACK-503: Rollback failed
  Category: Rollback
  Transient: false (permanent, requires manual intervention)
  When: Rollback unable to remove files (locked, permission, etc)
  User Message: "System rollback failed. Manual intervention required."
  Action: "Contact IT Help Desk immediately"
  Critical: System left in inconsistent state, needs human fix
```

---

## 5. Logging Redaction Rules

### 5.1 Redaction by Data Type

```
Data Type | Redaction Rule | Tier 1 (IT) | Tier 2 (Support) |
-----------|----------------|------------|-----------------|
Token (JWT) | [REDACTED_TOKEN] | FULL | REDACTED |
Password | [REDACTED_PASSWORD] | FULL | REDACTED |
Hash (SHA256) | [REDACTED_HASH] | FULL | REDACTED |
User Email | [REDACTED_USER@EMAIL] | FULL | REDACTED |
User Name | user_{id} (anonymized) | FULL | ANONYMIZED |
Registry Path | FULL path | FULL | FULL (not sensitive) |
File Path | FULL path | FULL | FULL (not sensitive) |
Machine Name | FULL name | FULL | FULL (identification) |
Timestamp | ISO 8601 | FULL | FULL (timeline) |
Error Code | OFF-* | FULL | FULL (support) |
Component | UC-001, etc | FULL | FULL (diagnosis) |

Source: T-005 Clarification 8
```

### 5.2 Redaction Rules Engine

```
LogEntry Redaction Rules (applied at read-time):

Rule 1: Token Redaction
  PATTERN: "Token:\s*[a-zA-Z0-9\-_.]*"
  REPLACE: "Token: [REDACTED_TOKEN]"

Rule 2: Hash Redaction
  PATTERN: "[a-f0-9]{64}" (SHA256, 64 hex chars)
  REPLACE: "[REDACTED_HASH]"

Rule 3: Password Redaction
  PATTERN: "password\s*[:=]\s*\"[^\"]*\""
  REPLACE: "password: \"[REDACTED_PASSWORD]\""

Rule 4: Email Redaction
  PATTERN: "[a-zA-Z0-9._%\-+]+@[a-zA-Z0-9.\-]+"
  REPLACE: "[REDACTED_USER@EMAIL]"

Implementation:
  - Redaction applied when Tier 2 log requested
  - Original Tier 1 log untouched
  - Support team never sees sensitive values
  - IT Admin has full access
```

---

## 6. Reference Documents

### 6.1 External Standards

```
Microsoft Office Deployment Tool (ODT):
  - Official documentation: https://docs.microsoft.com/deployoffice/
  - Configuration.xml schema: XSD published by Microsoft
  - Version support matrix: Maintained by Microsoft
  - Language support: List per version, maintained by Microsoft
  - Hash values: Microsoft publishes official SHA256 hashes

Windows Registry Standards:
  - Registry paths for Office: HKEY_LOCAL_MACHINE\Software\Microsoft\Office\*
  - Standard keys monitored: RegistrationDB, FirstRun, etc
  - User registry: HKEY_CURRENT_USER\Software\Microsoft\Office\*

Windows System Requirements:
  - Disk space: Microsoft ODT documentation
  - Admin privileges: Required for system-wide installation
  - Supported Windows versions: See Stage 7 constraints (T-012 security)
```

### 6.2 Stage 7 Documents Using These Structures

```
Document | Uses from This Section | Details |
-----------|------------------------|---------|
T-019 (State Machine) | Configuration object, state enum | State transitions |
T-020 (Error Propagation) | ErrorResult object, Error codes | Error handling |
T-030 (Logging Spec) | LogEntry object, redaction rules | Log format & storage |
T-031 (Disaster Recovery) | Rollback scope, Installation result | Recovery procedures |
Overall Architecture | All structures | Complete system design |
UC-001-005 Design | All structures, matrices | Use case specifications |
```

---

## Document Metadata

```
Created: 2026-05-01 09:00
Version: 1.0.0-DRAFT
Status: Ready for consolidation (T-007)
Data Structures Defined: 8 core objects
Matrices Created: 5 reference matrices
Error Codes Defined: 18 unique codes
Ready for CP1 Gate: YES
```

---

**END DATA STRUCTURES & MATRICES**

