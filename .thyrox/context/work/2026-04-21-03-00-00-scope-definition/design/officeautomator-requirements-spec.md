```yml
created_at: 2026-04-22 06:10:00
project: OfficeAutomator
work_package: 2026-04-21-03-00-00-scope-definition
phase: Phase 7 — DESIGN/SPECIFY
author: Claude
status: Aprobado
version: 1.0.0
document_type: Requirements Specification
```

# REQUIREMENTS SPECIFICATION - OfficeAutomator v1.0.0

---

## Executive Summary

OfficeAutomator is a PowerShell-based automation framework that provides reliable, transparent, and idempotent installation of Microsoft Office LTSC for versions 2024, 2021, and 2019. The specification defines 5 use cases that guide users through version selection, language choice, application exclusions, configuration validation, and installation execution.

**Key Success Metrics:**
- Fail-Fast validation prevents silent installation failures
- Idempotent execution: run once = run twice
- Transparent logging at every decision point
- Anti-Microsoft-OCT-bug mitigation at UC-004

---

## 1. REQUIREMENTS OVERVIEW

### 1.1 Project Context

- **Project Name:** OfficeAutomator
- **Version:** v1.0.0
- **Target Users:** IT Administrators, System Integrators
- **Platform:** Windows PowerShell 5.1+, Windows 10/11, Windows Server 2019+
- **Supported Office Versions:** 2024 LTSC, 2021 LTSC, 2019 LTSC

### 1.2 Core Requirements

| Requirement | Type | Status | Priority |
|------------|------|--------|----------|
| Select Office version (2024/2021/2019) | Functional | Defined | P0-CRITICAL |
| Select installation language (es-ES, en-US) | Functional | Defined | P0-CRITICAL |
| Exclude Office applications (Teams, OneDrive, Groove, Lync, Bing) | Functional | Defined | P1-HIGH |
| Validate configuration before installation | Functional | Defined | P0-CRITICAL |
| Install Office with zero silent failures | Functional | Defined | P0-CRITICAL |
| Detect and support existing Office installations | Non-Functional | Defined | P1-HIGH |
| Generate execution logs with timestamps | Non-Functional | Defined | P2-MEDIUM |
| Support retry on transient network failures | Non-Functional | Defined | P1-HIGH |

---

## 2. USE CASES - DETAILED SPECIFICATIONS

### 2.1 UC-001: Select Office Version

**Purpose:** Allow user to choose Office LTSC version before installation.

**Actors:** IT Administrator (human), OfficeAutomator (system)

**Preconditions:**
1. PowerShell session open with Administrator privileges
2. User has executed `Invoke-OfficeAutomator.ps1`
3. System has read access to supported-versions data

**Main Flow:**

```
1. System displays available versions:
   - Office 2024 LTSC (Principal, Support: 2026-10-13)
   - Office 2021 LTSC (Secondary, Support: 2026-10-13)
   - Office 2019 LTSC (Tertiary, Support: 2025-10-13)

2. User selects version via numbered menu (1, 2, or 3)

3. System validates selection against whitelist:
   Valid: 2024, 2021, 2019
   If invalid → Error message, restart from step 1
   If valid → Proceed to step 4

4. System confirms:
   [INFO] Selected Office {version} LTSC

5. Store: $Config.Version = selected_version

6. Return to UC-002: Select Language
```

**Postconditions (Success):**
- `$Config.Version` set to one of: "2024", "2021", "2019"
- UC-002 initiated
- Logged: timestamp, selected version

**Postconditions (Failure):**
- Error displayed to user
- `$Config.Version` remains null
- UC-002 not initiated
- Logged: timestamp, error message, failed attempt

**Acceptance Criteria:**
- [x] Display 3 version options with support end dates
- [x] Accept numeric input (1, 2, 3)
- [x] Validate against supported list
- [x] Store in configuration object
- [x] Error handling with retry
- [x] Logging for audit trail

**Error Scenarios:**

| Scenario | Trigger | Recovery | Category |
|----------|---------|----------|----------|
| Invalid selection (e.g., "5", "xyz") | User enters invalid input | Prompt user to retry with valid selection | USER_ERROR |
| Timeout on user input | User does not respond within 5 minutes | Graceful exit, preserve existing config | TIMEOUT |

**Testing Strategy:**
- Unit: Valid/invalid version selections
- Integration: Version selection → Language selection flow
- Edge: Timeout, non-alphanumeric input, boundary values

---

### 2.2 UC-002: Select Office Language

**Purpose:** Allow user to specify installation language(s).

**Actors:** IT Administrator (human), OfficeAutomator (system)

**Preconditions:**
1. UC-001 completed successfully (`$Config.Version` is set)
2. System has read access to language compatibility matrix

**Main Flow:**

```
1. System displays languages for selected version:
   Version 2024: es-ES, en-US (v1.0.0)
   Version 2021: es-ES, en-US (v1.0.0)
   Version 2019: es-ES, en-US (v1.0.0)
   [Additional languages available in v1.1+]

2. User selects language(s) via menu or checkbox:
   Option A: Single language (1 or 2)
   Option B: Multiple languages (1,2)

3. System validates selection:
   - All selected languages exist for version
   - All selected languages are in approved list (v1.0.0)
   If invalid → Error message, restart from step 1
   If valid → Proceed to step 4

4. System confirms:
   [INFO] Selected languages: {lang1}, {lang2}

5. Store: $Config.Languages = @("es-ES", "en-US")

6. Return to UC-003: Exclude Applications
```

**Postconditions (Success):**
- `$Config.Languages` set to array of valid languages
- UC-003 initiated
- Logged: timestamp, selected languages

**Postconditions (Failure):**
- Error displayed
- `$Config.Languages` not updated
- UC-003 not initiated

**Acceptance Criteria:**
- [x] Display supported languages per version
- [x] Accept single or multiple selections
- [x] Validate each language against version
- [x] Validate against v1.0.0 approved list
- [x] Support future language expansion (v1.1)
- [x] Default to OS language if appropriate

**Constraints - v1.0.0:**
- Only 2 languages: es-ES (Spanish Spain), en-US (English USA)
- If user requests additional language: graceful error with v1.1 roadmap reference

**Error Scenarios:**

| Scenario | Trigger | Recovery | Category |
|----------|---------|----------|----------|
| Language not in version | User selects language not available in Office {version} | Error: "Language X not available in Office {version}" | VALIDATION |
| Language not in v1.0.0 approved | User selects language available in version but not approved yet | Error: "Language X available in v1.1" | ROADMAP |
| No language selected | User does not select any language | Error: "At least one language required" | USER_ERROR |

**Testing Strategy:**
- Unit: Language validation per version
- Integration: Version selection → Language selection with all combinations
- Edge: Empty selection, unsupported language, version mismatch

---

### 2.3 UC-003: Exclude Office Applications

**Purpose:** Allow user to select which Office applications to exclude from installation.

**Actors:** IT Administrator (human), OfficeAutomator (system)

**Preconditions:**
1. UC-002 completed successfully (`$Config.Languages` is set)
2. System has read access to application availability matrix

**Main Flow:**

```
1. System displays available applications for selected version:
   Version 2024: Teams, OneDrive, Bing, Word, Excel, PowerPoint, Outlook, Access, Publisher, Project, Visio
   Version 2021: Teams, OneDrive, Groove, Word, Excel, PowerPoint, Outlook, Access, Publisher, Project, Visio
   Version 2019: OneDrive, Groove, Lync, Word, Excel, PowerPoint, Outlook, Access, Publisher

2. System shows defaults for version:
   - Teams: exclude by default (v2024+)
   - OneDrive: exclude by default (all versions)
   - Others: include by default

3. User selects/deselects applications via checkbox menu

4. System validates selection:
   - All selected exclusions exist in version
   - No invalid app names
   If invalid → Error message, restart from step 1
   If valid → Proceed to step 5

5. System confirms:
   [INFO] Excluding applications: {app1}, {app2}

6. Store: $Config.ExcludedApps = @("Teams", "OneDrive")

7. Return to UC-004: Validate Configuration
```

**Postconditions (Success):**
- `$Config.ExcludedApps` set to array of valid applications
- UC-004 initiated
- Logged: timestamp, excluded applications

**Postconditions (Failure):**
- Error displayed
- `$Config.ExcludedApps` not updated
- UC-004 not initiated

**Acceptance Criteria:**
- [x] Display all applications available in version
- [x] Show defaults per version
- [x] Accept multi-select via checkbox or comma-separated list
- [x] Validate each application exists in version
- [x] Allow "none" (empty exclusions)
- [x] Provide ability to reset to defaults

**Version-Specific Defaults:**

| Version | Default Exclusions | Available Apps |
|---------|-------------------|-----------------|
| 2024 | Teams, OneDrive | Word, Excel, PowerPoint, Outlook, Access, Publisher, Project, Visio, Teams, OneDrive, Bing |
| 2021 | Teams, OneDrive | Word, Excel, PowerPoint, Outlook, Access, Publisher, Project, Visio, Teams, OneDrive, Groove |
| 2019 | OneDrive | Word, Excel, PowerPoint, Outlook, Access, Publisher, Project, Visio, Lync, OneDrive, Groove |

**Error Scenarios:**

| Scenario | Trigger | Recovery | Category |
|----------|---------|----------|----------|
| App not in version | User selects app not available in version | Error: "App X not available in Office {version}" | VALIDATION |
| Invalid app name | User types invalid app name | Error: "Unknown application: X" | USER_ERROR |
| All apps excluded | User selects to exclude ALL apps | Warning: proceed? (rare use case) | EDGE_CASE |

**Testing Strategy:**
- Unit: Application validation per version
- Integration: Full flow with all version/language/exclusion combinations
- Edge: Empty exclusions, all exclusions, version-specific apps

---

### 2.4 UC-004: VALIDATE CONFIGURATION (CRITICAL - BLOQUEADOR)

**Purpose:** Validate all configuration decisions (version, languages, exclusions) before installation. Implements Fail-Fast principle.

**Actors:** OfficeAutomator (system), Office Deployment Tool (external)

**Criticality:** BLOQUEADOR - blocks UC-005 if failure

**Preconditions:**
1. UC-003 completed successfully (all config values set)
2. Internet connectivity available (for ODT download)
3. 3+ GB disk space available

**Main Flow - 8 Validation Points in 3 Phases:**

```
PHASE 1: PARALLEL VALIDATION (Steps 1, 2, 5 - no dependencies)
├─ Step 1: Validate Version Exists
├─ Step 2: Validate XML Schema
└─ Step 5: Validate App-Version Compatibility

PHASE 2: SEQUENTIAL VALIDATION (Steps 3→4→6 - dependencies)
├─ Step 3: Validate Language Exists in Version
├─ Step 4: Validate Language in v1.0.0 Approved List
└─ Step 6: ANTI-MICROSOFT-OCT-BUG: Language-App Compatibility

PHASE 3: DOWNLOAD & VERIFY (Step 7 with retry)
└─ Step 7: Download ODT & Verify SHA256 (3 retries with exponential backoff)

PHASE 4: GENERATION (Step 8)
└─ Step 8: Generate & Validate configuration.xml
```

**Detailed Specifications:**

#### STEP 1: Validate Version Exists

**Validation:**
```powershell
$ValidVersions = @("2024", "2021", "2019")
if ($Config.Version -notin $ValidVersions) {
    return Error("Version {0} not supported", $Config.Version)
}
```

**Error Category:** [BLOQUEADOR]
**Exit on Failure:** YES - No recovery

---

#### STEP 2: Validate XML Schema

**Validation:** Configuration.xml matches Office Deployment Tool XML schema

**Error Category:** [BLOQUEADOR]
**Exit on Failure:** YES - No recovery

---

#### STEP 3: Validate Language Exists in Version

**Validation:**
```powershell
$SupportedLangs = Get-SupportedLanguages -Version $Config.Version
foreach ($lang in $Config.Languages) {
    if ($lang -notin $SupportedLangs) {
        return Error("Language {0} not available in {1}", $lang, $Config.Version)
    }
}
```

**Error Category:** [BLOQUEADOR]
**Exit on Failure:** YES - No recovery

---

#### STEP 4: Validate Language in v1.0.0 Approved List

**Validation:**
```powershell
$ApprovedLangs = @("es-ES", "en-US")
foreach ($lang in $Config.Languages) {
    if ($lang -notin $ApprovedLangs) {
        return Error("Language {0} available in v1.1", $lang)
    }
}
```

**Error Category:** [BLOQUEADOR]
**Exit on Failure:** YES - No recovery

---

#### STEP 5: Validate App-Version Compatibility

**Validation:**
```powershell
$AvailableApps = Get-AvailableApps -Version $Config.Version
foreach ($app in $Config.ExcludedApps) {
    if ($app -notin $AvailableApps) {
        return Error("App {0} not available in {1}", $app, $Config.Version)
    }
}
```

**Error Category:** [BLOQUEADOR]
**Exit on Failure:** YES - No recovery

---

#### STEP 6: ANTI-MICROSOFT-OCT-BUG Language-App Compatibility

**Background:** Microsoft Office Customization Tool (OCT) allows incompatible language-app combinations (e.g., English UK + Project) which cause silent installation failures. This step prevents this.

**Validation:**
```powershell
$CompatMatrix = Get-LanguageCompatibilityMatrix
foreach ($lang in $Config.Languages) {
    foreach ($app in $Config.ExcludedApps) {
        $key = "$lang`_$app"
        if (-not $CompatMatrix[$key].IsCompatible) {
            return Error("Incompatible: Language {0} + App {1}", $lang, $app)
        }
    }
}
```

**Reference:** See `analysis-microsoft-oct.md` (Stage 1 DISCOVER)

**Error Category:** [BLOQUEADOR]
**Exit on Failure:** YES - No recovery
**Special Note:** Core mitigation for Microsoft OCT bug

---

#### STEP 7: Download ODT & Verify SHA256 (With Retry)

**Validation:** Office Deployment Tool downloaded and SHA256 matches official value

**Retry Logic:**
```powershell
$MaxRetries = 3
$Attempt = 0
while ($Attempt -lt $MaxRetries) {
    $Attempt++
    try {
        $ODT = Download-OfficeDeploymentTool -Version $Config.Version
        $ActualSHA = Get-FileHash $ODT -Algorithm SHA256
        $ExpectedSHA = Get-OfficialODTSHA256 -Version $Config.Version
        
        if ($ActualSHA.Hash -eq $ExpectedSHA) {
            break  # SUCCESS
        }
    } catch {
        # Network error, retry
    }
    
    if ($Attempt -lt $MaxRetries) {
        Start-Sleep -Seconds (2 * $Attempt)  # Exponential backoff: 2s, 4s, 6s
    }
}

if ($ActualSHA.Hash -ne $ExpectedSHA) {
    return Error("[CRITICO] ODT verification failed after 3 attempts")
}
```

**Retry Strategy:** Exponential backoff (2s, 4s, 6s)
**Error Category:** [CRITICO] - Automatic retry (3x)
**Exit on Failure:** YES after 3 attempts

---

#### STEP 8: Generate & Validate configuration.xml

**Validation:** configuration.xml generated and valid for setup.exe execution

**Process:**
```powershell
$ConfigXml = Convert-ToConfigurationXml @{
    Version = $Config.Version
    Languages = $Config.Languages
    ExcludedApps = $Config.ExcludedApps
}

if (-not (Test-Path $ConfigXml)) {
    return Error("Failed to generate configuration.xml")
}

# Write to temp location for UC-005
$Config.ConfigPath = "$env:TEMP\configuration.xml"
$ConfigXml | Out-File -FilePath $Config.ConfigPath -Encoding UTF8
```

**Error Category:** [BLOQUEADOR]
**Exit on Failure:** YES - No recovery

---

**Postconditions (Success):**
- All 8 steps passed
- `$Config.ValidationPassed = $true`
- `$Config.ConfigPath` set to valid XML file path
- `$Config.ODTPath` set to downloaded setup.exe path
- UC-005 proceeds to installation

**Postconditions (Failure):**
- One or more steps failed
- `$Config.ValidationPassed = $false`
- Detailed error message displayed
- UC-005 not executed
- User must restart from UC-001

**Acceptance Criteria:**
- [x] All 8 validation points implemented
- [x] Fail-Fast on any [BLOQUEADOR] error
- [x] Automatic retry on [CRITICO] errors (3x)
- [x] Exponential backoff for network retries
- [x] Anti-Microsoft-OCT-bug mitigation at Step 6
- [x] Comprehensive logging at each step
- [x] State management (ValidationPassed flag)
- [x] Configuration file generation and validation

**Error Categories:**

| Category | Steps | Action | Recovery |
|----------|-------|--------|----------|
| [BLOQUEADOR] | 1,2,3,4,5,6,8 | Fail-Fast | Restart UC-001 |
| [CRITICO] | 7 | Auto-retry 3x | If 3x fail: Error |
| [RECUPERABLE] | — | (None in v1.0.0) | — |

---

### 2.5 UC-005: Install Office

**Purpose:** Execute Office installation with validated configuration. Idempotent execution guaranteed.

**Actors:** OfficeAutomator (system), setup.exe (Office Deployment Tool)

**Criticality:** P0-CRITICAL (main user goal)

**Preconditions:**
1. UC-004 completed successfully (`$Config.ValidationPassed = $true`)
2. `$Config.ConfigPath` and `$Config.ODTPath` are set
3. Administrator privileges confirmed
4. Office not currently installing/updating

**Main Flow:**

```
1. Pre-Installation Check:
   - Is Office already installed?
   - If YES: Version matches? Language matches?
     - If YES → Log "Already installed" and exit
     - If NO → Repair or update as needed
   - If NO → Proceed to step 2

2. Execute Installation:
   Invoke: setup.exe /configure $Config.ConfigPath
   
3. Monitor Progress:
   - Capture stdout/stderr streams
   - Log progress every 5 seconds
   - Monitor for error conditions
   - Timeout: 30 minutes total

4. Capture Installation Result:
   - Exit code from setup.exe
   - Installation logs from %TEMP%
   - Error details if failure

5. Post-Installation Verification:
   - Office installed?
   - Correct version?
   - Correct languages?
   - If all YES → Success
   - If any NO → Error

6. Return Result to User:
   - Success: Log completion, installed version, languages
   - Failure: Log error details, suggest troubleshooting
```

**Postconditions (Success):**
- Office {version} LTSC installed
- Correct languages installed
- Correct applications excluded
- All logs captured
- Return success to user

**Postconditions (Failure):**
- Office not installed or incomplete
- Detailed error message to user
- Logs available for diagnosis
- Configuration preserved for retry

**Acceptance Criteria:**
- [x] Detect existing Office installation
- [x] Idempotent execution (run 2x = run 1x)
- [x] Monitor installation progress
- [x] Capture installation logs
- [x] Verify installation result
- [x] Handle installation failures gracefully
- [x] Provide detailed error messages

**Idempotence Guarantee:**

**First Execution:**
```
$Config.Version = "2024"
Office 2024 installed
Result: Success
```

**Second Execution:**
```
$Config.Version = "2024"  // Same
Office 2024 already installed
Result: Success (verified, not reinstalled)
```

**Mismatch Scenario:**
```
Execution 1: $Config.Version = "2024" → Office 2024 installed
Execution 2: $Config.Version = "2021" → Detected mismatch
Result: Update/Repair as needed OR error with recommendation
```

**Error Scenarios:**

| Scenario | Trigger | Recovery | Category |
|----------|---------|----------|----------|
| Installation timeout | setup.exe takes > 30 minutes | Log error, prompt retry | TIMEOUT |
| Setup.exe exits with error | setup.exe returns non-zero code | Log error details, suggest troubleshooting | INSTALL_FAILURE |
| Post-install verification fails | Installed version/language mismatch | Log mismatch details, mark as failure | VERIFICATION |
| Insufficient disk space | < 3 GB free during installation | Error, cleanup partial installation | RESOURCE |

**Testing Strategy:**
- Unit: Idempotence detection logic
- Integration: Full installation flow with mocked setup.exe
- Edge: Disk full, timeout, network interruption, corrupted config

---

## 3. CROSS-FUNCTIONAL REQUIREMENTS

### 3.1 Reliability

**Requirement:** System must prevent silent installation failures.

**Mechanisms:**
- UC-004 validates all configuration before installation
- Anti-Microsoft-OCT-bug mitigation at UC-004 Step 6
- Comprehensive error checking at each UC
- Fail-Fast principle: stop immediately on validation failure

**Metrics:**
- Zero silent installation failures
- 100% of configuration errors caught before UC-005

### 3.2 Transparency

**Requirement:** User must always know what is happening.

**Mechanisms:**
- Clear menu prompts at each UC
- Confirmation messages after each decision
- Structured logging with timestamps and levels
- Differentiated output: [INFO], [SUCCESS], [WARN], [ERROR], [BLOQUEADOR]

**Metrics:**
- Every decision logged with context
- Every error with actionable remediation
- Log file always available for audit

### 3.3 Idempotence

**Requirement:** Running installation twice = running once.

**Mechanisms:**
- UC-005 detects existing Office installation
- Verify installed version/language match
- Skip reinstall if already correct
- Repair/update if mismatch detected

**Metrics:**
- Idempotent execution verified by tests
- No duplicate installations
- No data loss on repeated execution

### 3.4 Performance

**Requirement:** Installation completes in reasonable time.

**Constraints:**
- UC-004 validation: < 5 minutes total
- UC-005 installation: < 30 minutes
- UC-007 SHA256 retry: < 10 minutes per attempt

**Metrics:**
- 99% of installations complete within timeouts
- Network retry backoff reduces server load

### 3.5 Supportability

**Requirement:** System produces detailed logs for troubleshooting.

**Mechanisms:**
- Timestamp every operation (millisecond precision)
- Log function entry/exit
- Log decision points and results
- Capture setup.exe output
- Store logs in `$env:TEMP\OfficeAutomator-{timestamp}.log`

**Metrics:**
- All installation runs traced in logs
- Logs sufficient for root cause analysis
- No sensitive data in logs

---

## 4. CONSTRAINTS AND LIMITATIONS

### 4.1 Version Constraints - v1.0.0

| Item | Constraint | Justification |
|------|-----------|----------------|
| Supported Office Versions | 2024, 2021, 2019 LTSC only | Long-term support versions |
| Supported Languages | es-ES, en-US only | v1.1 will add 6+ more |
| Installation Scope | Per-machine only | Single installation context |
| Supported OS | Windows 10/11, Server 2019+ | Modern Windows versions |
| PowerShell Version | 5.1+ | Modern PowerShell |

### 4.2 Functional Constraints

| Constraint | Impact | Workaround |
|-----------|--------|-----------|
| No GUI in v1.0.0 | CLI-only interface | v1.1 will add WPF GUI |
| No Group Policy support | Single-machine deployment | v1.1 will add GP templates |
| No Intune integration | Manual deployment only | v1.1 will add Intune support |

### 4.3 Technical Constraints

| Constraint | Impact | Mitigation |
|-----------|--------|------------|
| Requires admin privileges | Standard users cannot run | Run as administrator |
| Internet required for download | Cannot use offline media | v1.1 will support pre-cached ODT |
| 3 GB disk space required | May fail on small systems | Check disk space at start |

---

## 5. REFERENCES

### Stage 1 DISCOVER Outputs
- `problem-statement.md` - Problem definition and scope
- `analysis-microsoft-oct.md` - Microsoft OCT bug analysis and mitigation
- `use-case-matrix.md` - UC dependencies and flow
- `actors-stakeholders.md` - User roles and needs

### Stage 6 SCOPE Outputs
- `scope-statement.md` - What is IN/OUT scope
- `language-compatibility-matrix.md` - Version×Language×App compatibility
- `precise-definitions.md` - Exact values for versions, languages, applications

### Related Documents
- `overall-architecture.md` - System architecture and layer design
- `REGLAS_DESARROLLO_OFFICEAUTOMATOR.md` - Code development standards
- `convention-mermaid-diagrams.md` - Diagram standards (dark theme, no emojis)

---

## 6. ACCEPTANCE CRITERIA FOR STAGE 7

- [x] All 5 UCs fully specified with requirements and acceptance criteria
- [x] Each UC has error scenarios and recovery paths
- [x] UC-004 8-point validation process documented
- [x] Idempotence guarantee for UC-005 documented
- [x] Cross-functional requirements (reliability, transparency, etc.) defined
- [x] Constraints and limitations documented
- [x] References to Stage 1 and Stage 6 outputs included
- [x] Ready for implementation in Stage 10 (IMPLEMENT)

---

**Specification Status:** COMPLETE
**Version:** 1.0.0
**Approval Status:** Ready for Gate Review
**Next Phase:** Stage 10 IMPLEMENT (Design → Code)

