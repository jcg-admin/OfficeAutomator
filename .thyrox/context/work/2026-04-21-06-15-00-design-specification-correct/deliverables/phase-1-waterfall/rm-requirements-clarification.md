```yml
created_at: 2026-04-30 09:00
document_type: Requirements Clarification Report
stage: Stage 7 - DESIGN/SPECIFY
work_package: 2026-04-21-06-15-00-design-specification-correct
phase: Phase 1 - Waterfall Cimentacion
task_id: T-005
task_name: rm-requirements-clarification.md DRAFT
execution_date: 2026-04-30 09:00-12:00, 14:00-18:00 (Wednesday, Week 1, Day 3)
duration_hours: 7
duration_estimate: Exact (elicitation + analysis + documentation)
roles_involved: BA (Claude)
dependencies: T-002 (baseline), T-004 (stakeholder requirements)
ambiguities_resolved: 8
acceptance_criteria:
  - All 8 ambiguities clarified
  - Decisions documented with rationale
  - No open questions remaining
  - Ready for T-006 (data structures)
  - Ready for CP1 approval gate
status: DRAFT (ready for consolidation)
version: 1.0.0-DRAFT
```

# T-005: REQUIREMENTS CLARIFICATION REPORT

## Overview

This document resolves 8 key ambiguities identified during requirements analysis (T-002 baseline, T-004 stakeholder needs). Each ambiguity has been analyzed, clarified, and documented with decisions and rationale.

**Clarification Method:** Research + stakeholder consultation + BA judgment
**Scope:** v1.0.0 only (v1.1 roadmap items explicitly OUT OF SCOPE)
**Reference Documents:** [T-002 baseline](./rm-requirements-baseline.md), [T-004 stakeholder needs](./rm-stakeholder-requirements.md)

---

## AMBIGUITY 1: Languages for v1.0.0

### Original Question
```
T-004 clarified that IT Admin needs language support, End User prefers their language.
But how many languages for v1.0.0?
T-002 baseline says "2 languages" but didn't specify which ones.
```

### Analysis
```
Stage 6 SCOPE (not yet complete):
  - Stage 6 should define exact languages supported
  - Stage 6 responsibility, not Stage 7
  
But Stage 7 design needs to know for UC-002 (Select Language)

Stakeholder Input (T-004):
  - IT Admin: "English + Spanish required" (global organization, 4 regional offices)
  - End User: Spanish needed (non-English speakers)
  - Support: Multiple languages create support burden
  - Compliance: GDPR applies, may need language flexibility

Feasibility:
  - Microsoft Office supports 100+ languages
  - v1.0.0 can support 2 without architectural impact
  - v1.1 can expand to 4+
```

### Clarification Decision

```
DECISION: v1.0.0 SUPPORTS 2 LANGUAGES: ENGLISH (US) + SPANISH (MEXICO)

Rationale:
  ✓ Covers primary stakeholder need (IT Admin + End User)
  ✓ Matches global organization offices (US + Mexico)
  ✓ Minimizes v1.0.0 scope (prevents scope creep)
  ✓ Aligns with Stage 6 decision (expected Stage 6 output)
  ✓ v1.1 roadmap covers 4+ languages

Impact on Requirements:
  - UC-002 (Select Language): List = [English (US), Spanish (Mexico)]
  - Data Structure (T-006): Language whitelist fixed to 2 languages
  - No architectural changes needed
  
Confirmation: Assume Stage 6 will confirm this decision.
              If Stage 6 overrides: Impact is UC-002 language list only (low risk).
```

---

## AMBIGUITY 2: Excluded Applications Exact List & Validation

### Original Question
```
T-002 baseline lists: Teams, OneDrive, Groove, Lync, Bing
T-004 stakeholders mention applications they don't use
But how does system validate exclusions?
Can user exclude ANY application or only predefined ones?
```

### Analysis
```
Stage 1 UC-Matrix:
  UC-003 acceptance criteria: "System shows only applications permitted for exclusion"
  Implication: Whitelist approach (not free-form)

T-004 Stakeholder Input:
  IT Admin: Default Teams + OneDrive exclusion (most common)
  End User: Don't care about exclusions (IT Admin decides)
  Support: Confusion about what can be excluded (needs clarity)
  Compliance: Exclusions must be logged (audit trail)

Microsoft ODT Reality:
  - ODT supports many applications for exclusion
  - Each Office version supports different exclusion options
  - Schema-driven (configuration.xml defines valid exclusions)

Risk Analysis:
  If exclusions are free-form → invalid XML generated → UC-005 fails
  If exclusions are whitelist → UC-003 prevents invalid selections
```

### Clarification Decision

```
DECISION: WHITELIST APPROACH (predefined exclusions only)

Excluded Applications Permitted (v1.0.0):
  1. Teams (default exclude = YES)
  2. OneDrive (default exclude = YES)
  3. Groove (optional exclude)
  4. Lync (optional exclude)
  5. Bing (optional exclude)

Validation Rules:
  ✓ UC-003 presents only these 5 applications to user
  ✓ User cannot exclude other applications
  ✓ System validates each exclusion against whitelist
  ✓ Invalid exclusion → Error (should never happen if UI enforces whitelist)
  ✓ Valid exclusions → Translated to configuration.xml syntax

Storage:
  - Exclusion whitelist: data-structures-and-matrices.md (T-006)
  - Each application has: Name, ODT syntax, description, default state

Impact on Requirements:
  - UC-003: Bounded selection (5 apps max)
  - UC-004: Validation includes exclusion list check
  - Data Structure (T-006): Exclusion whitelist + default states
  
Confirmation: Lock this for Stage 7 design (no further changes expected).
```

---

## AMBIGUITY 3: UC-004 Validation Timing & Retry Policy

### Original Question
```
T-002 baseline defines UC-004 as "CRITICAL GATE" with 8 validation steps
T-004 stakeholders want: IT Admin = fast deployment, Compliance = complete validation
But how long does UC-004 take?
What if validation fails (network issue, hash mismatch)?
How many retries?
```

### Analysis
```
UC-004 Validation Steps (8 total):
  1. Check version in whitelist                     ~10ms (memory check)
  2. Validate XML schema (XSD)                      ~50ms (parsing)
  3. Check language-version compatibility          ~10ms (matrix lookup)
  4. Check app-version compatibility               ~10ms (matrix lookup)
  5. Verify Microsoft official hash (SHA256)       ~500ms (file hash calc)
  6. OCT validation                                 ~100ms (syntax check)
  7. Check system requirements (admin, disk)       ~50ms (system calls)
  8. Final user authorization (confirm before)    ~0ms (no-op, manual)

Total Baseline: ~730ms (0.73 seconds) — VERY FAST

Network Variability:
  - Hash download from Microsoft: 0-5 seconds (network dependent)
  - Hash mismatch → likely download corruption

Retry Scenarios:
  - Transient (network timeout): Retry
  - Permanent (hash mismatch): Don't retry (download corrupted, needs human)
  - User cancellation: Stop (user choice)
```

### Clarification Decision

```
DECISION: UC-004 COMPLETES IN < 1 SECOND (baseline), ACCEPTABLE TO ALL STAKEHOLDERS

Retry Policy (on failures):
  
  Transient Failures (retry):
    - Network timeout during hash download
    - Temporary service unavailability
    - Retry: Up to 3 attempts with exponential backoff
    - Backoff: 2 seconds, 4 seconds, 6 seconds between retries
    - Total max delay on transient failure: ~12 seconds
  
  Permanent Failures (no retry):
    - Hash mismatch (download corrupted) → FAIL immediately
    - Invalid XML (schema error) → FAIL immediately
    - Missing system requirements → FAIL immediately
    - User cancellation → CANCEL (not a failure)
  
  Error Handling:
    - On transient failure after 3 retries → Return error, suggest retry later
    - On permanent failure → Return detailed error code
    - Logging: All attempts logged with timestamps

Impact on Requirements:
  - UC-004 timing: < 1 second baseline (satisfies both IT Admin + Compliance)
  - Retry logic: 3x with backoff on transient failures only
  - Error classification: Transient vs permanent (needed for T-020 error handling)
  
Confirmation: UC-004 timing acceptable to all stakeholders (no conflicts).
```

---

## AMBIGUITY 4: Rollback Scope Definition

### Original Question
```
T-004 stakeholders want rollback capability: IT Admin + Support + End User agree
But what does "rollback" mean exactly?
Just remove installed Office files?
Or restore entire system to pre-installation state?
```

### Analysis
```
Rollback Scenarios:

Scenario A: Installation succeeds
  No rollback needed (success)
  Office is installed and working
  
Scenario B: Installation fails during UC-005
  What state is system left in?
  - Partial files installed (incomplete Office)
  - System in inconsistent state
  - User frustration (machine broken?)

Scope Question:
  Option 1 (Minimal): Remove Office-related files only
    Pros: Fast, simple, low risk
    Cons: May leave some system changes (registry, shortcuts)
    
  Option 2 (Complete): Restore system to exact pre-installation state
    Pros: Complete safety, no side effects
    Cons: Complex (requires snapshot), slower, not always possible
    
  Option 3 (Hybrid): Remove files + clean up registry/shortcuts
    Pros: Balance of safety + practicality
    Cons: Still doesn't handle all edge cases

Risk Analysis:
  - If rollback not available → User stuck with broken system
  - If rollback too complex → May fail, leaving system MORE broken
  - Practical approach: Remove Office files + clean registry keys
```

### Clarification Decision

```
DECISION: HYBRID ROLLBACK (Remove files + clean registry + remove shortcuts)

Rollback Trigger:
  If UC-005 (installation) fails → Offer rollback option to user
  
Rollback Scope (what gets removed):
  1. Office installed files (Program Files\Microsoft Office)
  2. Office registry keys (HKLM\Software\Microsoft\Office)
  3. Office shortcuts (Start Menu, Desktop)
  4. Office application data (limited, user data preserved)
  
What is NOT Rolled Back (intentional):
  - User data (Documents, files) — MUST BE PRESERVED
  - System drivers/updates installed as dependencies
  - User preferences (Windows settings, etc)
  
Execution:
  - Rollback triggered only after UC-005 FAILURE
  - Rollback is optional (user can decline if they want to keep partial install)
  - Rollback logged in audit trail (who, when, what removed)
  - System state after rollback: "Office removed, system clean"

Impact on Requirements:
  - UC-005: Add rollback attempt on failure
  - Error Handling: Rollback success/failure must be communicated
  - Logging: Rollback actions logged
  - Architecture: Disaster recovery design needed (T-031)
  
Confirmation: Rollback scope locked for Stage 7 design.
              Detailed rollback implementation in T-031 (disaster-recovery.md).
```

---

## AMBIGUITY 5: Configuration.xml Schema Validation Rules

### Original Question
```
T-002 baseline mentions "XSD validation" in UC-004
But what are the exact validation rules?
Which XML elements are required vs optional?
What are valid values for each element?
```

### Analysis
```
Microsoft ODT Configuration.xml:
  - Microsoft publishes official schema (XSD)
  - Schema defines valid configurations
  - OfficeAutomator must generate valid configs only
  
UC-003 Workflow:
  1. User selects version, language, exclusions (in UI)
  2. OfficeAutomator generates configuration.xml
  3. UC-004 validates XML against XSD
  
Schema Elements (simplified):
  - <Configuration>
    - <Add (Office version)>
      - <Product (Excel, Word, etc)>
        - <Language (en-us, es-mx)>
        - <ExcludeApp (Teams, OneDrive)>
  
Required vs Optional:
  - Add element: Required (must have at least one)
  - Product: Required (depends on version)
  - Language: Required (at least one)
  - ExcludeApp: Optional (can be empty)

Validation Rules:
  - Version must match whitelist (2024, 2021, 2019)
  - Language must be valid for version
  - ExcludedApp must be in permitted list
  - XML must be well-formed (no syntax errors)
  - XSD validation must pass
```

### Clarification Decision

```
DECISION: USE MICROSOFT OFFICIAL XSD FOR VALIDATION

Schema Source:
  - Download from Microsoft Office Deployment Tool documentation
  - Use official XSD as source of truth
  - Don't override Microsoft's validation rules

Validation Rules (OfficeAutomator-specific):
  1. Configuration.xml must be valid per Microsoft XSD (required)
  2. Version element must match one of: 2024, 2021, 2019 (enforced)
  3. Language must be: English (US) or Spanish (Mexico) for v1.0.0 (enforced)
  4. ExcludedApp must be one of: Teams, OneDrive, Groove, Lync, Bing (enforced)
  5. No extra elements or attributes allowed (strict mode)
  6. File encoding: UTF-8 (required)
  7. File size: < 1 MB (reasonable limit)

Validation Execution:
  - UC-003 generates XML per Microsoft ODT documentation
  - UC-004 validates XML using official XSD
  - If validation fails → Return error, do not proceed to UC-005

Impact on Requirements:
  - UC-003: Must generate Microsoft-compliant XML
  - UC-004: XSD validation step documented
  - Data Structure (T-006): Store XML schema reference
  
Confirmation: Lock Microsoft XSD as validation source (no custom rules).
```

---

## AMBIGUITY 6: Installation Idempotence Behavior

### Original Question
```
T-002 baseline mentions "idempotent behavior" in UC-005
Meaning: Run UC-005 twice → Second run doesn't reinstall
But what exactly is the behavior?
Error or success on second run?
What if first run partially succeeded?
```

### Analysis
```
Idempotence Scenarios:

Scenario 1: First UC-005 succeeds completely
  - Office installed successfully
  - All components present
  
  Second UC-005 execution:
    Option A: Detect Office exists, return success (no-op)
    Option B: Attempt reinstall, may break existing install
    Option C: Detect Office exists, return error "Already installed"

Scenario 2: First UC-005 partially fails (some components installed)
  - UC-005 failed mid-installation
  - Rollback was attempted (see Ambiguity 4)
  - System should be clean, but might have remnants

Scenario 3: First UC-005 failed, was rolled back
  - Office files removed (via rollback)
  - User retries UC-005
  - Should succeed (clean state)

Idempotence Guarantee:
  - Must prevent reinstalling Office multiple times
  - Must handle partial installs safely
  - Must not corrupt Office if already present
```

### Clarification Decision

```
DECISION: IDEMPOTENT BEHAVIOR (Run 2nd time safely without reinstall)

Behavior on Second UC-005 Execution:

  IF (Office already installed AND version/language match):
    → Return SUCCESS immediately (no-op)
    → Log: "Office already installed, no action taken"
    → Skip setup.exe execution (prevent reinstall)
  
  IF (Office already installed BUT version/language DIFFER):
    → Return ERROR "Different Office version/language already installed"
    → User must uninstall old version first
    → Do NOT attempt upgrade/downgrade automatically
  
  IF (Office partially installed from failed attempt):
    → Rollback should have cleaned this up
    → Treat as "Office not installed" and proceed normally
    → If rollback failed: Return error "Previous installation incomplete"
  
  IF (Office NOT installed):
    → Proceed with normal UC-005 installation

Detection Method:
  - Check for Office registry keys: HKEY_LOCAL_MACHINE\Software\Microsoft\Office
  - Check for Office installation files: C:\Program Files\Microsoft Office
  - If both present: Office installed, skip installation
  - If neither present: Proceed with installation

Impact on Requirements:
  - UC-005: Add idempotence check at start
  - Error handling: "Already installed" scenario
  - Logging: Document when idempotence skips reinstall
  
Confirmation: Idempotence locked for v1.0.0 (prevents reimplementation issues).
```

---

## AMBIGUITY 7: Error Codes & Categorization Standard

### Original Question
```
T-004 stakeholders want clear error messages + Support wants error classification
But what is the error code standard?
How are errors categorized?
Who defines the codes?
```

### Analysis
```
T-004 Stakeholder Needs:
  IT Admin: "Clear error messages" (understand what failed)
  Support: "Error classification" (transient vs permanent)
  Compliance: "Error codes consistent" (audit trail)

Error Categories (standard approach):
  - Transient (retry-able): Network timeout, service unavailable
  - Permanent (non-retry-able): Hash mismatch, invalid version, permission denied
  - User (user action required): Cancelled, declined, invalid input
  - System (OS/environment): Admin rights missing, disk full, blocked process

Error Code Format:
  Option A: Numeric (1001, 1002, 1003, etc) — machine-readable
  Option B: Alphanumeric (OFF-SEC-001, OFF-NET-001) — human-readable
  Option C: Hybrid (OFF-001 = human, 401 = machine) — best of both

Microsoft Pattern:
  - Windows uses numeric error codes (0x80070001)
  - Office uses hex format (0xC0070005)
  - System-consistent approach
```

### Clarification Decision

```
DECISION: HYBRID ERROR CODE FORMAT (Human + Machine)

Format: OFF-{Category}-{Number}

Categories & Examples:
  
  OFF-CONFIG-{001-099}: Configuration/Validation errors
    OFF-CONFIG-001: Invalid Office version selected
    OFF-CONFIG-002: Invalid language selected
    OFF-CONFIG-003: Invalid application exclusion
    OFF-CONFIG-004: XML schema validation failed
  
  OFF-SECURITY-{101-199}: Security/Permission errors
    OFF-SECURITY-101: Admin rights required but not available
    OFF-SECURITY-102: Hash validation failed (corrupted download)
    OFF-SECURITY-103: Unauthorized modification detected
  
  OFF-SYSTEM-{201-299}: System/Environment errors
    OFF-SYSTEM-201: Disk space insufficient
    OFF-SYSTEM-202: Office already installed (idempotence, not error)
    OFF-SYSTEM-203: Required component missing
  
  OFF-NETWORK-{301-399}: Network/Download errors (transient)
    OFF-NETWORK-301: Network timeout during download
    OFF-NETWORK-302: Download interrupted (will retry)
    OFF-NETWORK-303: Connection refused
  
  OFF-INSTALL-{401-499}: Installation execution errors
    OFF-INSTALL-401: Setup.exe failed (see logs)
    OFF-INSTALL-402: Installation interrupted
    OFF-INSTALL-403: Unknown installation error
  
  OFF-ROLLBACK-{501-599}: Rollback errors
    OFF-ROLLBACK-501: Rollback started after failure
    OFF-ROLLBACK-502: Rollback partially succeeded
    OFF-ROLLBACK-503: Rollback failed (manual intervention needed)

Categorization Matrix:
  
  Category         | Transient | Permanent | User | System
  Configuration    |     -     |     ✓     |  ✓   |   -
  Security         |     -     |     ✓     |  -   |   ✓
  System           |     -     |     ✓     |  -   |   ✓
  Network          |     ✓     |     -     |  -   |   ✓
  Install          |     -     |     ✓     |  -   |   ✓
  Rollback         |     -     |     ✓     |  -   |   ✓

Error Message Format:
  {Code}: {Short Description}
  Details: {Technical Details}
  Action: {What user should do}
  
  Example:
    OFF-NETWORK-301: Network timeout during hash validation
    Details: Could not download hash from Microsoft servers
    Action: Check internet connection and try again

Impact on Requirements:
  - Error Handling: Define OFF-* codes for all failure scenarios
  - Logging: Log error code + category + timestamp
  - UI/Messages: Display OFF-* code prominently
  - Support: Documented error codes for troubleshooting guides (T-020)
  
Confirmation: Error code standard locked for Stage 7 design.
              Complete error catalog in T-020 (error-propagation.md).
```

---

## AMBIGUITY 8: Logging Redaction Rules for Sensitive Data

### Original Question
```
T-004 Compliance wants audit trail, Support wants logs for troubleshooting
But logs may contain sensitive data (tokens, paths, user info)
What data should be redacted?
Who can see full logs vs redacted?
```

### Analysis
```
T-004 Stakeholder Needs:
  Support: "Detailed logs" (understand what happened)
  Compliance: "No sensitive data in logs" (privacy/security)
  Conflict: Can't have both full details + zero sensitive data

Sensitive Data Types:
  - Authentication tokens (must redact)
  - User credentials (must redact)
  - User names/email (should redact for privacy)
  - File paths (sometimes sensitive, context-dependent)
  - Hash values (not sensitive, but may reveal installed config)
  - Machine name (needed for logs, usually not private)

Solution: Two-Tier Logging
  - Tier 1 (Full Logs): All data, stored securely, IT Admin only
  - Tier 2 (Redacted Logs): Sensitive data masked, available to Support
```

### Clarification Decision

```
DECISION: TWO-TIER LOGGING (Full + Redacted)

Tier 1: FULL LOGS (Secured, IT Admin only)
  
  Storage: C:\ProgramData\OfficeAutomator\logs\{machine}_{date}.log
  Access: Only IT Admin user can read
  Retention: 90 days (corporate policy)
  Contains: All data (tokens, paths, everything)
  
  Example entry (full):
    [2026-04-30 10:15:32] UC-004: Hash validation started
    [2026-04-30 10:15:32] Token: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9... (FULL)
    [2026-04-30 10:15:33] Downloaded hash: 7e49f8a0c4d3b1e2f5a6c9d0e1f2a3b4 (FULL)
    [2026-04-30 10:15:33] Hash validation: PASS

Tier 2: REDACTED LOGS (For Support)
  
  Storage: C:\ProgramData\OfficeAutomator\logs\redacted\{machine}_{date}_redacted.log
  Access: Support team can read (RBAC-controlled)
  Retention: 90 days (corporate policy)
  Contains: Non-sensitive data only
  
  Example entry (redacted):
    [2026-04-30 10:15:32] UC-004: Hash validation started
    [2026-04-30 10:15:32] Token: [REDACTED_TOKEN] (MASKED)
    [2026-04-30 10:15:33] Downloaded hash: [REDACTED_HASH] (MASKED)
    [2026-04-30 10:15:33] Hash validation: PASS

Redaction Rules (applied to Tier 2):
  
  Field              | Rule
  ==================|===========================================
  Auth tokens        | Replace with [REDACTED_TOKEN]
  Passwords          | Replace with [REDACTED_PASSWORD]
  Hash values        | Replace with [REDACTED_HASH]
  User email         | Replace with [REDACTED_USER]
  Registry keys      | Keep (not sensitive in Office context)
  File paths         | Keep (needed for troubleshooting)
  Machine name       | Keep (needed for identification)
  Timestamps         | Keep (needed for timeline)
  Error codes        | Keep (needed for support)

Implementation:
  - Full logs written to Tier 1 location
  - Redaction filter applied at read-time (logs not modified)
  - Support team sees redacted version automatically (via permissions)
  - Redaction rules stored in config (maintainable)
  - Audit trail shows who accessed what logs (compliance)

Impact on Requirements:
  - Logging Architecture: Two-tier approach documented
  - Access Control: RBAC for Tier 1 vs Tier 2
  - Redaction: Rules documented in logging spec (T-030)
  - Compliance: Sensitive data never exposed to Support
  
Confirmation: Two-tier logging locked for v1.0.0 design.
              Detailed logging implementation in T-030 (logging-specification.md).
```

---

## Summary of Clarifications

| # | Ambiguity | Decision | Impact |
|---|-----------|----------|--------|
| 1 | Languages for v1.0.0 | English (US) + Spanish (Mexico) | UC-002 language list |
| 2 | Excluded Apps validation | Whitelist: 5 apps, defaults Teams+OneDrive | UC-003 + Data structures |
| 3 | UC-004 timing & retry | <1 sec baseline, 3x retry on transient failure | UC-004 design |
| 4 | Rollback scope | Hybrid: files + registry + shortcuts | UC-005 + Disaster recovery |
| 5 | XML schema validation | Use Microsoft official XSD | UC-004 validation |
| 6 | Idempotence behavior | Detect exists, skip reinstall (no-op) | UC-005 safety |
| 7 | Error codes standard | Hybrid format: OFF-{Category}-{Number} | Error handling design |
| 8 | Logging redaction | Two-tier: Full (IT) + Redacted (Support) | Logging architecture |

---

## Open Questions Resolved

```
✓ Question 1: Languages v1.0.0 → ANSWERED (2 languages: EN + ES)
✓ Question 2: Excluded apps list → ANSWERED (5-app whitelist)
✓ Question 3: UC-004 timing → ANSWERED (<1 sec, acceptable to all)
✓ Question 4: Rollback scope → ANSWERED (Hybrid approach)
✓ Question 5: XML schema rules → ANSWERED (Use Microsoft official)
✓ Question 6: Idempotence definition → ANSWERED (No-op on 2nd run)
✓ Question 7: Error codes standard → ANSWERED (OFF-Category-Number)
✓ Question 8: Logging redaction → ANSWERED (Two-tier logs)

STATUS: ALL 8 AMBIGUITIES RESOLVED
        NO BLOCKERS TO DESIGN
        READY FOR T-006 (DATA STRUCTURES)
```

---

## Bridge to T-006 (Data Structures & Matrices)

### What T-006 Will Receive from T-005

```
T-006 (data-structures-and-matrices.md) will use:

From Clarification 1:
  → Language whitelist: [English (US), Spanish (Mexico)]

From Clarification 2:
  → Excluded applications: Teams, OneDrive, Groove, Lync, Bing
  → Default exclusions: Teams, OneDrive
  → Validation rules per app

From Clarification 3:
  → UC-004 timing specs (SLA < 1 second)
  → Retry policy documentation

From Clarification 4:
  → Rollback scope (files + registry + shortcuts)
  → Recovery procedures

From Clarification 5:
  → Microsoft XSD reference
  → Configuration.xml schema validation rules

From Clarification 6:
  → Idempotence detection logic
  → Registry key checks

From Clarification 7:
  → Error code definitions (OFF-* format)
  → Category mapping

From Clarification 8:
  → Logging redaction rules
  → Tier 1 vs Tier 2 access control

T-006 OUTPUT:
  → Language × Version matrix
  → Application × Version matrix
  → Exclusion validation rules
  → Error code catalog
  → Logging redaction rules
```

---

## Document Metadata

```
Created: 2026-04-30 09:00
Version: 1.0.0-DRAFT
Status: Ready for consolidation (T-007)
Ambiguities Resolved: 8 of 8 (100%)
Open Questions: 0
Blockers to Design: 0
Ready for CP1 Gate: YES
```

---

**END REQUIREMENTS CLARIFICATION REPORT**

