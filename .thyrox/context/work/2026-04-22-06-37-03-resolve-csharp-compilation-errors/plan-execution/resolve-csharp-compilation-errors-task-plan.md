```yml
created_at: 2026-04-22 06:50:00
project: OfficeAutomator
work_package: 2026-04-22-06-37-03-resolve-csharp-compilation-errors
phase: Phase 8 — PLAN EXECUTION
author: Claude
status: Aprobado
```

# Phase 8 PLAN EXECUTION — Task Plan

---

## Overview

**Strategy Decision (Phase 5):** Solution A — Add missing using statements to 3 test files

**Execution Approach:** TDD + Documentation
- Phase 9: Validate fix on 1 file (PoC)
- Phase 10: Apply to all 3 files, run full test suite
- Phase 11: Document patterns and prevention strategies

**Total Tasks:** 5 (3 implementation + 1 validation + 1 documentation)
**Estimated Time:** < 1 hour total (2 min implementation + 30 min documentation)

---

## Task Dependency Graph (DAG)

```
T-001 (File 1)
    ↓
T-002 (File 2) ──┐
    ↓            │
T-003 (File 3)   │
    ↓            ↓
T-004 (Validate & Test) ← synchronization point
    ↓
T-005 (Document patterns)
    ↓
COMPLETE
```

**Execution Order:** Sequential (T-001 → T-002 → T-003 → T-004 → T-005)
**All tasks are idempotent:** Running 2x = Running 1x

---

## Tasks

### T-001: Add Using Statements to VersionSelectorTests.cs

**Status:** ⏳ Pending

**Objective:** Add missing using statements to VersionSelectorTests.cs

**File Location:** `/home/user/OfficeAutomator/tests/OfficeAutomator.Core.Tests/Services/VersionSelectorTests.cs`

**Changes Required:**

Find this block (lines 1-4):
```csharp
using Xunit;
using OfficeAutomator.Core.Services;

namespace OfficeAutomator.Tests
```

Replace with:
```csharp
using Xunit;
using OfficeAutomator.Core.Services;
using OfficeAutomator.Core.Models;   // ← ADD THIS
using OfficeAutomator.Core.Error;    // ← ADD THIS

namespace OfficeAutomator.Tests
```

**Observable Criteria:**
```bash
# Verify the using statements were added
grep -n "using OfficeAutomator.Core" /home/user/OfficeAutomator/tests/OfficeAutomator.Core.Tests/Services/VersionSelectorTests.cs | head -5
# Expected output includes:
#   1:using Xunit;
#   2:using OfficeAutomator.Core.Services;
#   3:using OfficeAutomator.Core.Models;
#   4:using OfficeAutomator.Core.Error;
```

**Idempotency:** Adding same using statement twice is safe (compiler ignores duplicates)

**Estimated Time:** 1 minute

**Dependencies:** None

---

### T-002: Add Using Statements to LanguageSelectorTests.cs

**Status:** ⏳ Pending

**Objective:** Add missing using statements to LanguageSelectorTests.cs

**File Location:** `/home/user/OfficeAutomator/tests/OfficeAutomator.Core.Tests/Services/LanguageSelectorTests.cs`

**Changes Required:** Identical to T-001 (add same 2 using statements)

**Observable Criteria:**
```bash
grep -n "using OfficeAutomator.Core" /home/user/OfficeAutomator/tests/OfficeAutomator.Core.Tests/Services/LanguageSelectorTests.cs | head -5
# Expected: Same as T-001 output
```

**Idempotency:** Yes

**Estimated Time:** 1 minute

**Dependencies:** None (can run in parallel with T-001, but sequential is safer)

---

### T-003: Add Using Statements to AppExclusionSelectorTests.cs

**Status:** ⏳ Pending

**Objective:** Add missing using statements to AppExclusionSelectorTests.cs

**File Location:** `/home/user/OfficeAutomator/tests/OfficeAutomator.Core.Tests/Services/AppExclusionSelectorTests.cs`

**Changes Required:** Identical to T-001 and T-002

**Observable Criteria:**
```bash
grep -n "using OfficeAutomator.Core" /home/user/OfficeAutomator/tests/OfficeAutomator.Core.Tests/Services/AppExclusionSelectorTests.cs | head -5
# Expected: Same pattern as T-001 and T-002
```

**Idempotency:** Yes

**Estimated Time:** 1 minute

**Dependencies:** T-001, T-002 (sequential)

---

### T-004: Build & Test Validation

**Status:** ⏳ Pending (after T-003)

**Objective:** Verify all using statements are in place and tests compile successfully

**Pre-requisites:** T-001, T-002, T-003 completed

**Commands:**

```bash
# Step 1: Clean old build artifacts
cd /home/user/OfficeAutomator
dotnet clean

# Step 2: Full rebuild (debug configuration)
dotnet build --configuration Debug
# Expected: Build succeeded. 0 errors, 0 warnings

# Step 3: Check for any remaining CS0246 errors
dotnet build 2>&1 | grep "error CS0246" | wc -l
# Expected output: 0

# Step 4: Discover all tests
dotnet test --list-tests | wc -l
# Expected output: 220+ (indicating test discovery successful)

# Step 5: Run full test suite (this will take a few minutes)
dotnet test
# Expected: All tests pass (or at least test execution succeeds without compilation errors)
```

**Observable Criteria (PROVEN):**

| Check | Command | Expected | Status |
|-------|---------|----------|--------|
| Compilation | `dotnet build 2>&1 \| tail -1` | `Build succeeded` | ⏳ Pending |
| CS0246 Errors | `dotnet build 2>&1 \| grep "CS0246" \| wc -l` | `0` | ⏳ Pending |
| Test Discovery | `dotnet test --list-tests \| wc -l` | `220+` | ⏳ Pending |
| Test Execution | `dotnet test` | `X passed, 0 failed` | ⏳ Pending |

**Idempotency:** Yes (running multiple times produces same result)

**Estimated Time:** 5 minutes (mostly waiting for dotnet build/test)

**Dependencies:** T-001, T-002, T-003

---

### T-005: Update Documentation (Phase 11 prep)

**Status:** ⏳ Pending (after T-004)

**Objective:** Document patterns and create prevention strategy for CONTRIBUTING.md

**Deliverables (created in Phase 11 TRACK):**

1. **Lessons Learned Document**
   - Pattern identified: "Selector tests import their own namespace but use classes from other namespaces"
   - Root cause: Missing using statements
   - Solution: Solution A (add using statements)
   - Why this happened: Incomplete copy-paste of test templates

2. **Prevention Strategy**
   - Update CONTRIBUTING.md with guidelines:
     - "Test files using Configuration must import: `using OfficeAutomator.Core.Models;`"
     - "Test files using ErrorHandler must import: `using OfficeAutomator.Core.Error;`"
   - Code review checklist for test files
   - Optional: Pre-commit hook to validate using statements

3. **Changelog Entry**
   - Document fix applied to 3 files
   - Timeline: Phase 5 (decision) → Phase 10 (implementation) → Phase 11 (documentation)

**Observable Criteria:**
```bash
# After Phase 11:
grep -r "using OfficeAutomator.Core" /home/user/OfficeAutomator/tests/ | wc -l
# Expected: Increased count (showing all 3 files now have proper imports)

# Check CONTRIBUTING.md was updated:
grep -i "using statement" /home/user/OfficeAutomator/CONTRIBUTING.md
# Expected: Guidelines present
```

**Idempotency:** Documentation updates are idempotent

**Estimated Time:** 30 minutes (comprehensive documentation)

**Dependencies:** T-001, T-002, T-003, T-004 (need completion evidence)

---

## Verification Checklist

### Pre-Execution Checklist (Phase 9)
- [ ] All 3 files identified
- [ ] File paths verified correct
- [ ] Using statements to add confirmed (Models, Error)
- [ ] Backup of original files (git history)

### Phase 9 PILOT/VALIDATE (1 file)
- [ ] T-001 completed (VersionSelectorTests.cs)
- [ ] Build succeeds after T-001
- [ ] No CS0246 errors for VersionSelectorTests
- [ ] Tests from VersionSelectorTests discovered successfully

### Phase 10 IMPLEMENT (all 3 files)
- [ ] T-002 completed (LanguageSelectorTests.cs)
- [ ] T-003 completed (AppExclusionSelectorTests.cs)
- [ ] T-004 completed (full build & test validation)
- [ ] 0 CS0246 errors across entire solution
- [ ] 220+ tests discovered
- [ ] Test suite executes successfully

### Phase 11 TRACK/EVALUATE
- [ ] T-005 completed (documentation)
- [ ] Lessons learned documented
- [ ] Prevention strategy in CONTRIBUTING.md
- [ ] Changelog updated
- [ ] WP marked as CLOSED

---

## Risk Mitigation

**Risk 1: Using statement already present**
- **Mitigation:** Grep before editing, skip if already present
- **Impact:** Idempotent — no harm in adding duplicate

**Risk 2: Wrong using statement added**
- **Mitigation:** Copy exact namespace from class definition
  - Configuration lives in: `OfficeAutomator.Core.Models`
  - ErrorHandler lives in: `OfficeAutomator.Core.Error`
- **Impact:** Build will fail, revert and fix

**Risk 3: Build still fails after adding using statements**
- **Mitigation:** This is unlikely (we've validated the root cause), but if it happens:
  1. Verify using statements match exactly (no typos)
  2. Run `dotnet clean` then `dotnet build`
  3. Check if other errors appear (different root cause)
  4. Escalate to Phase 3 DIAGNOSE if still broken

---

## Timeline

| Phase | Task | Time | Cumulative |
|-------|------|------|-----------|
| 9 (PILOT) | T-001 | 1 min | 1 min |
| 10 (IMPL) | T-002 | 1 min | 2 min |
| 10 (IMPL) | T-003 | 1 min | 3 min |
| 10 (IMPL) | T-004 (build/test) | 5 min | 8 min |
| 11 (TRACK) | T-005 (documentation) | 30 min | 38 min |
| **Total** | **All tasks** | **~40 min** | |

---

## Success Definition

**Phase 10 IMPLEMENT:** ✅ SUCCESS when:
- All 3 files have using statements added
- `dotnet build` returns exit code 0
- `dotnet test --list-tests` shows 220+ tests
- No CS0246 errors remain

**Phase 11 TRACK/EVALUATE:** ✅ SUCCESS when:
- Lessons learned documented
- Prevention strategy documented
- CONTRIBUTING.md updated
- WP marked CLOSED

---

## Execution Notes

**Implementation Approach:** TDD + Documentation
- Each task is atomic and independently verifiable
- All tasks are idempotent (can re-run without breaking anything)
- Observable criteria are measurable and documented
- Prevention strategy ensures this issue doesn't recur

**Quality Gates:**
- Phase 9: Validate 1 file compiles (de-risk)
- Phase 10: Validate all 3 files + full test suite
- Phase 11: Comprehensive documentation of solution and patterns

---

**Phase 8 Status: ✅ COMPLETE**

**Task Plan Created:** 5 tasks, sequential execution, ~40 minutes total

**Ready for: Phase 9 PILOT/VALIDATE**

---

**Created:** 2026-04-22 06:50:00 UTC
**Author:** Claude (THYROX Phase 8 PLAN EXECUTION)
**Status:** Aprobado

