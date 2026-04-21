```yml
type: Error Correction Plan
created_at: 2026-05-16 10:30
source: Rigorous Deep-Review findings
phase: PHASE 2 - SPRINT 1 - CORRECTION PHASE
files_to_update: 4 (T-019, T-020, T-021, T-022)
total_lines_to_add: ~208 lines
estimated_time: 3-4 horas
blocking: YES - must complete before T-023
```

# ERROR PROPAGATION MAP & CORRECTION PLAN

## Summary

Sprint 1 rigorous analysis identified 5 critical errors affecting 4 documents. If not corrected, these errors will:
- Cause code duplication in Stage 10
- Create knowledge gaps for developers
- Lead to tech debt in implementation
- Confuse patterns for UC-003+

This document maps each error → affected files → correction action.

---

## ERROR #1: Configuration NOT Formalized as Class

### Impact: **CRITICAL**
Blocks all other pseudocode development.

### Current State
- Configuration exists only as `$Config` object in flows
- Properties mentioned but not in formal C# class
- No formal definition document

### Files Affected

**1. design-state-management-design.md (T-019)**
- Current: States defined but Configuration object undefined
- Required: Formal Configuration class definition (~50 lines)
- Action: Add Section after "State Machine Overview"
- Content needed:
  ```csharp
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
  ```
- Lines to add: ~50

**2. design-error-propagation-strategy.md (T-020)**
- Current: Assumes Configuration class exists
- Required: Reference to T-019 formal definition
- Action: Add reference in Section 8.1
- Content: "See T-019 Configuration class definition"
- Lines to change: ~5

**3. design-uc-001-002-state-flows.md (T-022)**
- Current: Tracks $Config evolution without class ref
- Required: Explicit reference to Configuration class
- Action: Add reference in Section 3.1
- Lines to change: ~3

### Correction Order
1. T-019 (add class)
2. T-020 (add reference)
3. T-022 (add reference)

### Lines to Add
**Total: +50 lines** (net impact: Configuration class formalized)

---

## ERROR #2: VersionSelector & LanguageSelector are SKELETONS

### Impact: **HIGH**
Pattern unclear for UC-003 developers, confusing pseudocode level.

### Current State
- VersionSelector: 32 lines (4 methods)
- LanguageSelector: 35 lines (4 methods)
- Many methods described but not implemented

### Files Affected

**design-uc-001-002-state-flows.md (T-022) - Section 5.1**

Current VersionSelector:
```
❌ Issues:
  • Only Execute() and IsValidVersion() fully shown
  • DisplayVersionSelectionUI() — NOT implemented
  • GetUserSelection() — NOT implemented
  • Error/logging methods too generic

✓ Required:
  • Expand to 80-100 lines (3x current)
  • Implement DisplayVersionSelectionUI() with UI structure
  • Implement GetUserSelection() with input validation
  • Add detailed comments on pre/post conditions
  • Add retry loop handling in Execute()
```

Lines to add: **+48 lines** (32 → 80)

**design-uc-001-002-state-flows.md (T-022) - Section 5.2**

Current LanguageSelector:
```
❌ Issues:
  • Only Execute() implementation shown
  • GetAvailableLanguages() — NOT implemented
  • DisplayLanguageSelectionUI() — NOT implemented
  • Version-language matrix missing from code
  • Error handling too generic

✓ Required:
  • Expand to 100-120 lines (3x current)
  • Implement GetAvailableLanguages() with matrix
  • Implement DisplayLanguageSelectionUI()
  • Implement GetUserSelection()
  • Add version-language compatibility checks
  • Add detailed error messages
```

Lines to add: **+65 lines** (35 → 100)

### Correction Order
1. VersionSelector (Section 5.1)
2. LanguageSelector (Section 5.2)

### Lines to Add
**Total: +113 lines**

---

## ERROR #3: Error Routing Incomplete in ErrorHandler

### Impact: **MEDIUM**
ErrorHandler exists but implementation details unclear.

### Current State
- ErrorHandler: 111 lines, 6 methods
- ErrorCatalog structure shown but only 2 example codes
- RetryOperation() referenced but not implemented
- Backoff calculation mentioned but not shown

### Files Affected

**design-error-propagation-strategy.md (T-020) - Section 8.1**

Current ErrorHandler Issues:
```
❌ Problems:
  • ErrorCatalog shown with only 2 codes (OFF-CONFIG-001, OFF-NETWORK-301)
  • All 18 codes mentioned in text but not in code
  • RetryOperation() referenced but method doesn't exist
  • Backoff timing (2s, 4s, 6s) mentioned but not in code
  • HandleTransientError() logic too abstract
  • HandleSystemError() logic missing

✓ Required:
  • Option A: Add all 18 error codes to ErrorCatalog in pseudocode
  • Option B: Add explicit reference "See T-021 for all 18 error codes"
  • Add RetryOperation() method (5-10 lines)
  • Add backoff calculation in HandleTransientError() (8-12 lines)
  • Expand HandleSystemError() logic (5-8 lines)
  • Add DetermineRecoveryState() complete routing logic (8-12 lines)
```

Lines to add: **+30 lines**

### Correction Action
- Add RetryOperation() method
- Add backoff timing details
- Add complete HandleSystemError() logic
- Add complete DetermineRecoveryState() routing
- Reference T-021 for error catalog

### Lines to Add
**Total: +30 lines**

---

## ERROR #4: OFF-SYSTEM-999 Missing from Catalog

### Impact: **LOW**
Orphaned fallback code, minor inconsistency.

### Current State
- OFF-SYSTEM-999 mentioned in T-020 (fallback for unknown errors)
- Not documented in T-021 error codes catalog

### Files Affected

**design-error-codes-catalog.md (T-021)**

Add new section:
```
### OFF-SYSTEM-999: Unknown/Fallback Error

Quick Info:
- Error Code: OFF-SYSTEM-999
- Category: System (Unknown)
- Severity: 3 (Medium)
- Location: Any (fallback)
- Retry: No
- User Recoverable: Maybe

User Message:
"An unexpected error occurred. Please contact IT Help Desk"

Technical Details:
Code: OFF-SYSTEM-999
Condition: Error code not recognized or unmapped
Details: Unknown_error_code=[code_value]
Root Cause: Code mapping incomplete or unrecognized error
Impact: Fallback error handling triggered

Recovery Procedure (User):
1. Note the error message and code
2. Contact IT Help Desk with details
3. Provide any context about what you were doing

Support Troubleshooting:
- This is a catchall error
- Indicates a code not mapped in OFF-* catalog
- Action: Report to development team for mapping
- Add new error code if recurrent
```

Lines to add: **+15 lines**

### Correction Order
- Add at end of catalog (after OFF-ROLLBACK-503)

### Lines to Add
**Total: +15 lines**

---

## ERROR #5: Methods Missing from Pseudocode

### Impact: **MEDIUM**
Described in comments but not implemented.

### Current State
- VersionSelector: Methods listed in comments, not in code
- LanguageSelector: Methods listed in comments, not in code

### Files Affected

**design-uc-001-002-state-flows.md (T-022)**

VersionSelector missing methods:
```
❌ Listed in comments but NOT implemented:
  • DisplayVersionSelectionUI()
  • GetUserSelection()

✓ Need to implement in Section 5.1
```

LanguageSelector missing methods:
```
❌ Listed in comments but NOT implemented:
  • GetAvailableLanguages()
  • DisplayLanguageSelectionUI()
  • GetUserSelection()

✓ Need to implement in Section 5.2
```

### Correction
- Implement all listed methods in actual code
- Not just describe them
- Match ErrorHandler/StateMachine pseudocode level

### Lines to Add
**Included in Error #2 calculations (+113 lines total)**

---

## CORRECTION PLAN: Order & Timeline

### Phase 1: Critical Dependencies (Order Matters)

```
STEP 1: T-019 - Add Configuration Class
  File: design-state-management-design.md
  Section: After Section 3
  Action: Insert Configuration class definition
  Lines: +50
  Time: 45 minutes
  Blocking: YES (step 2 & 3 depend on this)

STEP 2: T-022 - Expand VersionSelector & LanguageSelector
  File: design-uc-001-002-state-flows.md
  Sections: 5.1 & 5.2
  Action: Expand pseudocode (3x current)
  Lines: +113
  Time: 90 minutes
  Blocking: YES (UC-003 pattern depends on this)

STEP 3: T-020 - Complete ErrorHandler
  File: design-error-propagation-strategy.md
  Section: 8.1
  Action: Add missing methods & logic
  Lines: +30
  Time: 60 minutes
  Blocking: YES (Stage 10 implementation depends)
```

### Phase 2: Quality Improvements (Optional)

```
STEP 4: T-021 - Add OFF-SYSTEM-999
  File: design-error-codes-catalog.md
  Section: New section at end
  Action: Document fallback code
  Lines: +15
  Time: 20 minutes
  Blocking: NO (documentation only)

STEP 5: Update Deep-Review
  File: class-traceability-RIGOROUS-deep-review.md
  Section: Conclusions
  Action: Note corrections applied
  Lines: +20
  Time: 15 minutes
  Blocking: NO (informational)
```

---

## Summary Table

| Document | Error | Action | Lines | Time | Blocking |
|----------|-------|--------|-------|------|----------|
| T-019 | Config not formalized | Add class def | +50 | 45m | YES |
| T-022 | Skeletons (V/L Sel) | Expand pseudocode | +113 | 90m | YES |
| T-020 | Error routing incomplete | Complete ErrorHandler | +30 | 60m | YES |
| T-021 | OFF-SYSTEM-999 missing | Add entry | +15 | 20m | NO |
| Deep-Review | Outdated findings | Update conclusions | +20 | 15m | NO |

**Total Lines Added: ~208**
**Total Time: 3-4 hours**
**Blocking Tasks: 3 (must complete before T-023)**

---

## Impact of NOT Correcting

```
If we skip these corrections and proceed to T-023:

❌ Configuration class will be RE-DEFINED in UC-003/004
   → Code duplication
   → Inconsistency risk
   → Tech debt

❌ UC-003 pattern will be unclear
   → Developers must infer from T-022
   → Higher risk of implementation errors
   → Longer Stage 10 timeline

❌ ErrorHandler routing incomplete
   → Developers unclear on recovery paths
   → Possible errors in production
   → Support team confusion

❌ OFF-SYSTEM-999 orphaned
   → Support team won't know what to do with this code
   → User experience impacted

❌ Knowledge gaps accumulate
   → Each UC designed at different detail level
   → Future maintenance difficult
   → Onboarding new developers harder
```

---

## Recommendation

**EXECUTE CORRECTIONS NOW** (before T-023)

Rationale:
1. **Cost of fixing now:** 3-4 hours
2. **Cost of fixing in Stage 10:** 20+ hours
3. **Cost of shipping with tech debt:** Months of maintenance
4. **Risk reduction:** Eliminates 5 design errors
5. **Pattern clarity:** UC-003+ developers have clear examples

---

## Document Metadata

```
created_at: 2026-05-16 10:30
type: Correction Plan
files_affected: 4
total_corrections: 5 errors
total_lines: +208
estimated_time: 3-4 horas
priority: CRITICAL (block T-023 start)
gate_status: Sprint 1 must not advance until corrections done
```

---

**END ERROR PROPAGATION MAP**

**Clear plan to eliminate all 5 design errors before proceeding to T-023 ✓**

