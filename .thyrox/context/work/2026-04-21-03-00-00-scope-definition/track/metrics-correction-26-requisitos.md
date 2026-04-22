```yml
created_at: 2026-04-22 11:15:00
project: OfficeAutomator
work_package: 2026-04-21-03-00-00-scope-definition
phase: Phase 11 — TRACK/EVALUATE (CORRECTION)
author: NestorMonroy
status: Aprobado
version: 1.0.0
correction_type: Metrics Baseline Recalculation
```

# Metrics Correction: 26 Requisitos Baseline

## Problem Statement

**Phase 4 CONSTRAINTS** used "21 requisitos totales" in coverage formula.
**Phase 5 STRATEGY** referenced "23 requisitos" in gap analysis.
**Phase 1 DISCOVER** enumerates **26 requisitos** based on acceptance criteria.

**Impact:** All derived metrics (Coverage, Completitud, etc.) are invalid until corrected.

---

## Corrected Metrics (26 as Denominator)

### Coverage Metric

**Old Formula (incorrect, using 21):**
```
Coverage = (CUBIERTO + 0.5×SUPERFICIAL) / 21
         = (13 + 0.5×6) / 21
         = (13 + 3) / 21
         = 16 / 21
         = 76.19%
```

**New Formula (correct, using 26):**
```
Coverage = (CUBIERTO + 0.5×SUPERFICIAL) / 26
         = (13 + 0.5×6) / 26
         = (13 + 3) / 26
         = 16 / 26
         = 61.54%
```

**Delta:** -14.65 percentage points

---

### Completitud Metric

**Old (using 21):**
```
Completitud = CUBIERTO / 21
            = 13 / 21
            = 61.90%
```

**New (using 26):**
```
Completitud = CUBIERTO / 26
            = 13 / 26
            = 50.00%
```

**Delta:** -11.90 percentage points

---

### Superficialidad Metric

**Old (using 21):**
```
Superficialidad = 0.5 × SUPERFICIAL / 21
                = 0.5 × 6 / 21
                = 3 / 21
                = 14.29%
```

**New (using 26):**
```
Superficialidad = 0.5 × SUPERFICIAL / 26
                = 0.5 × 6 / 26
                = 3 / 26
                = 11.54%
```

**Delta:** -2.75 percentage points

---

## Coverage Gap Analysis (Corrected)

### Current State vs Target

| Metric | Old Baseline | New Baseline | Target | Gap (corrected) |
|--------|---|---|---|---|
| Coverage | 76.19% | 61.54% | 90% | **28.46 pp** |
| Completitud | 61.90% | 50.00% | 80% | **30.00 pp** |
| Superficialidad | 14.29% | 11.54% | 10% | **1.54 pp overage** |

**Interpretation:**
- Old metrics suggested we were "76% covered" → FALSE (only 61.54%)
- New metrics reveal significant gap: 28.46 pp to reach 90% coverage target
- This explains why Phase 5 STRATEGY implementation seemed complete: metrics were misleading

---

## What This Means for Phase 5 STRATEGY

### Current Implementation Coverage

Of 26 requisitos:
- **13 fully covered** (REQ-001, 002, 003, 004, 005, 006, 007, 008, 009, 010, 011, 012, 013)
- **6 partially covered** (REQ-014, 015, 016, 017, 018, 019) = 0.5 credit each = 3 points
- **7 not covered** (REQ-020, 021, 022, 023, 024, 025, 026)

### What's Missing (7 requisitos = 26.92% gap)

Uncovered requisitos:
1. **REQ-020:** Audit logging (hash values)
2. **REQ-021:** Installation execution (setup.exe)
3. **REQ-022:** Progress monitoring
4. **REQ-023:** Output capture (stdout/stderr)
5. **REQ-024:** Success reporting
6. **REQ-025:** Error reporting
7. **REQ-026:** Idempotency guarantee (no duplicate installs)

**These are primarily UC-004 and UC-005 implementation details.**

---

## Gate Impact: Phase 4→5 Transition

### Previous Gate Status (with incorrect 21-count)
✓ PASS - Coverage 76.19% exceeded threshold

### Corrected Gate Status (with correct 26-count)
**❌ BLOCK** - Coverage 61.54% is below Phase 5 entry threshold (typically 70%)

**Requirement:** Phase 4 CONSTRAINTS gate must re-validate with corrected metrics before Phase 5 can proceed.

---

## Implementation Task Mapping

To achieve 90% coverage, the following requisitos must be fully implemented:

**Priority 1 (UC-005 core):**
- REQ-021: Execute setup.exe with configuration.xml
- REQ-022: Monitor installation progress
- REQ-023: Capture process output

**Priority 2 (UC-005 observability):**
- REQ-024: Report success status
- REQ-025: Report errors with detail
- REQ-026: Guarantee idempotency (no duplicate installs)

**Priority 3 (UC-004 completeness):**
- REQ-020: Log hash values for audit trail

---

## Corrected Baseline for Phase 5

**Authority:** Phase 1 DISCOVER enumeration (26 requisitos)
**Current State:** 61.54% coverage (13 full + 3 partial)
**Target State:** 90% coverage (23.4 requisitos equivalent)
**Gap:** 28.46 percentage points
**Estimated Effort:** Implement 7 missing requisitos (UC-004 logging, UC-005 execution, monitoring, reporting, idempotency)

---

## Action Items

### Immediate (Phase 5 STRATEGY Review)

- [ ] Acknowledge corrected requirement count (26 vs 21/23)
- [ ] Recalculate all Phase 4 and Phase 5 metrics with denominator = 26
- [ ] Update Phase 5 STRATEGY document to reflect 61.54% coverage (not 76.19%)
- [ ] Revalidate Phase 4→5 gate with corrected metrics

### Before Phase 7 DESIGN/SPECIFY

- [ ] Enumerate which 7 requisitos (REQ-020 through REQ-026) remain to be implemented
- [ ] Estimate implementation effort for each
- [ ] Create task plan (Phase 8) to close the 28.46 pp gap

### Before Phase 10 IMPLEMENT

- [ ] Verify each REQ-NNN is implemented and tested
- [ ] Recalculate final coverage: should be ≥90%

---

## Historical Record: Why 21 vs 23?

**Phase 4 CONSTRAINTS (21):** Counted only "core functional requirements" without cross-cutting concerns.
**Gap Analysis (23):** May have added some but not all non-functional requirements.
**Authoritative (26):** Includes all acceptance criteria — functional + non-functional (validation, persistence, logging, error handling, idempotency).

**Resolution:** Using 26 as the authoritative count prevents confusion and ensures comprehensive coverage tracking.

---

## Validation

Corrected metrics are mathematically sound:
- ✓ Coverage formula: (CUBIERTO + 0.5×SUPERFICIAL) / TOTAL = (13+3) / 26 = 61.54%
- ✓ Completitud: CUBIERTO / TOTAL = 13 / 26 = 50.00%
- ✓ Superficialidad: 0.5×SUPERFICIAL / TOTAL = 3 / 26 = 11.54%
- ✓ Sum: 50% + 11.54% = 61.54% ✓

---

**Effective Date:** 2026-04-22
**Authority:** Phase 1 DISCOVER (requirement-definition-and-enumeration.md)
**Status:** APPROVED AND BINDING
**Next Review:** After Phase 10 IMPLEMENT (verify 26/26 requisitos implemented)
