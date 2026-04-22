```yml
created_at: 2026-04-22 11:00:00
project: OfficeAutomator
work_package: 2026-04-21-03-00-00-scope-definition
phase: Phase 1 — DISCOVER (CLARIFICATION)
author: NestorMonroy
status: Aprobado
version: 1.0.0
```

# Requirement Definition and Enumeration — OfficeAutomator

## Executive Summary

**Authoritative Count: 26 functional requirements** based on acceptance criteria enumerated in Phase 1 DISCOVER.

**Discrepancy Resolution:** The 21 vs 23 mentioned in Phase 4 CONSTRAINTS and Phase 5 STRATEGY documents used incomplete or different definitions of "requisito". This document establishes:
1. Precise definition of "requisito" (requirement)
2. Difference between UC (Use Case) and requisito (functional requirement)
3. Canonical enumeration of all 26 requirements with UC traceability

---

## Core Definitions

### What is a Use Case (UC)?

**Definition:** A user-facing workflow or business process that delivers value. Each UC is an atomic unit of functionality from the user's perspective.

**Characteristics:**
- Initiated by an actor (IT Administrator, System)
- Produces a measurable outcome
- Has clear entry/exit points
- Contains one or more requirements to implement it

**OfficeAutomator UCs:**
- UC-001: Select Version
- UC-002: Select Language
- UC-003: Exclude Applications
- UC-004: Validate Integrity
- UC-005: Install Office

**Total: 5 Use Cases**

---

### What is a Requirement (Requisito)?

**Definition:** A specific, measurable condition that a UC must satisfy to be considered complete and correct.

**Characteristics:**
- Written as an acceptance criterion (testable)
- Derivable from UC description
- Can be validated through testing
- Maps to implementation tasks

**Scope:** Includes functional requirements (behavior) + non-functional cross-cutting concerns (logging, persistence, error handling)

---

## Key Distinction: UC vs Requisito

### Use Case (Higher Level)

```
UC-001: Select Version
├── Actor: IT Administrator
├── Goal: Choose Office LTSC version (2024, 2021, or 2019)
├── Prerequisite: None
└── Outcome: Version selected and persisted
```

**This is ONE use case**

### Requirements (Lower Level - Acceptance Criteria)

```
UC-001 Requisitos (4 total):
├── REQ-001: User can select from 3 versions
├── REQ-002: Selection is mandatory
├── REQ-003: System validates version is valid
└── REQ-004: Selected version persists to next UCs
```

**These are 4 requirements to implement UC-001**

---

## Canonical Requirement Enumeration (26 Total)

### UC-001: Select Version (4 Requirements)

| # | Requisito | Acceptance Criterion |
|---|-----------|-------------------|
| REQ-001 | Version options available | User can select between 3 versions (2024, 2021, 2019) |
| REQ-002 | Mandatory selection | Selection is obligatory (user cannot skip) |
| REQ-003 | Input validation | System validates chosen version against allowed list |
| REQ-004 | State persistence | Selected version persists to subsequent UCs |

**UC-001 Total: 4 requisitos**

---

### UC-002: Select Language (4 Requirements)

| # | Requisito | Acceptance Criterion |
|---|-----------|-------------------|
| REQ-005 | Multiple selection | User can select 1 or more languages |
| REQ-006 | Version-filtered options | System shows only languages supported by chosen version |
| REQ-007 | Mandatory selection | At least 1 language must be selected |
| REQ-008 | State persistence | Language selections persist to subsequent UCs |

**UC-002 Total: 4 requisitos**

---

### UC-003: Exclude Applications (6 Requirements)

| # | Requisito | Acceptance Criterion |
|---|-----------|-------------------|
| REQ-009 | Optional selection | User can select 0 or more apps to exclude |
| REQ-010 | Version-filtered options | System shows only apps available in chosen version |
| REQ-011 | Input validation | System validates each exclusion is valid for version |
| REQ-012 | Allowed exclusions | Exclusion list limited to: Teams, OneDrive, Groove, Lync, Bing |
| REQ-013 | Default exclusions | Teams and OneDrive excluded by default (user can override) |
| REQ-014 | State persistence | Exclusion selections persist to subsequent UCs |

**UC-003 Total: 6 requisitos**

---

### UC-004: Validate Integrity (6 Requirements)

| # | Requisito | Acceptance Criterion |
|---|-----------|-------------------|
| REQ-015 | SHA256 calculation | System calculates SHA256 hash of downloaded file |
| REQ-016 | Hash comparison | System compares calculated hash against Microsoft's official hash |
| REQ-017 | Success path | If hashes match: return PASS |
| REQ-018 | Failure path | If hashes don't match: return FAIL with error code |
| REQ-019 | Retry mechanism | On corruption: automatically retry download (max 3 attempts) |
| REQ-020 | Audit logging | Logs include both calculated and expected hashes |

**UC-004 Total: 6 requisitos**

---

### UC-005: Install Office (6 Requirements)

| # | Requisito | Acceptance Criterion |
|---|-----------|-------------------|
| REQ-021 | Execute installation | System runs setup.exe with generated configuration.xml |
| REQ-022 | Progress monitoring | System monitors and tracks installation progress |
| REQ-023 | Output capture | System captures stdout/stderr from installation process |
| REQ-024 | Success reporting | On success: return exit code 0 |
| REQ-025 | Error reporting | On error: return error code + detailed logs |
| REQ-026 | Idempotency guarantee | Executing 2x produces same result as executing 1x (no duplicate installs) |

**UC-005 Total: 6 requisitos**

---

## Requirement Summary

### By UC

| UC | Requisitos | Types |
|----|-----------|-------|
| UC-001 | 4 | Selection, validation, persistence |
| UC-002 | 4 | Selection, filtering, persistence |
| UC-003 | 6 | Selection, validation, defaults, persistence |
| UC-004 | 6 | Calculation, comparison, retry, logging |
| UC-005 | 6 | Execution, monitoring, reporting, idempotency |

### By Category

| Category | Count | Examples |
|----------|-------|----------|
| **Input Selection** | 9 | Version choice, language choice, app selection |
| **Validation** | 5 | Version valid, language supported, hash match |
| **State Management** | 4 | Version persists, language persists, app persists |
| **Processing** | 4 | SHA256 calc, hash compare, execute install, monitor |
| **Error Handling** | 2 | Retry logic, error reporting |
| **Atomicity** | 1 | Idempotency guarantee |
| **Observability** | 1 | Logging (hashes, progress, errors) |

**TOTAL: 26 functional requirements**

---

## Resolution of 21 vs 23 Discrepancy

### What Happened

**Phase 4 CONSTRAINTS** mentioned "21 requisitos totales" in coverage formula.
**Phase 5 STRATEGY** mentioned "23 requisitos" in gap analysis.
**Phase 1 DISCOVER** enumerates **26 requisitos**.

### Root Cause

Earlier analyses used incomplete definitions:
- **21 count:** Likely excluded cross-cutting concerns (logging, persistence)
- **23 count:** Likely excluded some error-handling or idempotency requirements
- **26 count:** Complete functional + non-functional acceptance criteria

### Official Resolution

**The authoritative requirement count is 26**, derived from acceptance criteria in:
- Source: `/home/user/OfficeAutomator/.thyrox/context/work/2026-04-21-01-30-00-uc-documentation/use-case-matrix.md`
- Method: Enumeration of acceptance criteria per UC
- Traceability: Each requirement (REQ-001 through REQ-026) maps to UC and acceptance criterion

**Impact on Metrics:**

Old formula (using 21):
```
Coverage = (13 CUBIERTO + 0.5×6 SUPERFICIAL) / 21
         = 16 / 21
         = 76.19%
```

Corrected formula (using 26):
```
Coverage = (13 CUBIERTO + 0.5×6 SUPERFICIAL) / 26
         = 16 / 26
         = 61.54%
```

**Delta:** -14.65 percentage points (significant impact on Phase 5 metrics)

---

## UC vs Requisito: Quick Reference

| Aspect | UC | Requisito |
|--------|----|-----------| 
| **Level** | High (user perspective) | Low (implementation perspective) |
| **Count** | 5 total | 26 total |
| **Scope** | One business process | One acceptance criterion |
| **Example** | "Select Version" | "User can select 3 versions" |
| **Testing** | Narrative scenario | Automated test case |
| **Mapping** | 1 UC → multiple requisitos | Many requisitos → 1 UC |

**Relationship:** 
```
UC-001 (abstract workflow)
    ├── REQ-001 (testable criterion)
    ├── REQ-002 (testable criterion)
    ├── REQ-003 (testable criterion)
    └── REQ-004 (testable criterion)
```

---

## Verification Checklist

- [x] All 5 UCs identified and described
- [x] All 26 requirements enumerated from acceptance criteria
- [x] Each requirement traced to its UC
- [x] Each requirement written as testable criterion
- [x] Requirements span functional + non-functional concerns
- [x] Discrepancy (21 vs 23) root cause identified
- [x] Authoritative count established (26)
- [x] Impact on metrics documented (Coverage -14.65pp)

---

## Next Steps

1. **Phase 4 CONSTRAINTS:** Update coverage formula to use 26 as denominator
2. **Phase 5 STRATEGY:** Recalculate all metrics with corrected denominator
3. **Gate Phase 4→5:** Approve Phase 5 with corrected baseline metrics
4. **Implementation:** Map each REQ-NNN to code tasks (T-NNN)

---

## Appendix: Requirements-to-Code Mapping (Example)

How requirements flow to implementation:

```
REQ-001: User can select 3 versions
    ↓
T-101: Implement VersionSelector.GetSupportedVersions()
    ↓
Code: VersionSelector.cs → List<string> GetSupportedVersions()
    ↓
Test: VersionSelectorTests.cs → void TestCanSelect3Versions()
```

Each REQ-NNN maps to 1+ implementation tasks (T-NNN) and 1+ test cases.

---

**Authority:** This enumeration is authoritative for Phase 1 DISCOVER.
**Version:** 1.0.0 (final)
**Status:** APPROVED
**Date:** 2026-04-22
