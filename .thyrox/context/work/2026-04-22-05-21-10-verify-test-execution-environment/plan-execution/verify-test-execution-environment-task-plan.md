```yml
created_at: 2026-04-22 10:45:00
project: OfficeAutomator
work_package: 2026-04-22-05-21-10-verify-test-execution-environment
phase: Phase 8 — PLAN EXECUTION
author: Claude
status: Borrador
```

# PHASE 8: PLAN EXECUTION — Task Decomposition

## Overview

**IMPORTANT:** This document describes the TASK PLAN for Phases 9-11:
- **Phase 9 PILOT/VALIDATE:** Execute T-001 to T-011 (file creation + preliminary tests)
- **Phase 10 IMPLEMENT:** Execute real .NET installation and test suite
- **Phase 11 TRACK:** Document results

Each task is **idempotent** (run 2x = run 1x) and **verification-driven** (observable criteria, not subjective).

**Total work:** 12 tasks across 3 categories
- **File creation/verification:** T-001 through T-008 (7 new files + 2 updates, all idempotent)
- **Preliminary testing:** T-009 through T-011 (component validation, NO real .NET install)
- **Integration & commit:** T-012 (final verification)

---

## Dependency Graph (DAG)

```
T-001 (.editorconfig)
  ├─→ (no deps)

T-002 (verify-environment.sh)
T-003 (setup.sh)
T-004 (cleanup.sh)
  ├─→ (no deps, parallel)
  
  ├─→ T-005 (Makefile) ← DEPENDS ON T-002, T-003
      └─→ T-006 (global.json) ← DEPENDS ON T-005
      └─→ T-007 (README.md) ← DEPENDS ON T-005
      └─→ T-008 (CONTRIBUTING.md) ← DEPENDS ON T-005

T-009 (Test individual scripts) ← DEPENDS ON T-002, T-003, T-004
  └─→ T-010 (Test Makefile targets) ← DEPENDS ON T-005, T-009
      └─→ T-011 (Test full idempotency) ← DEPENDS ON T-010

T-012 (Commit & verify) ← DEPENDS ON T-001 through T-011
```

**Critical path:** T-002 → T-003 → T-005 → T-010 → T-011 → T-012 (6 sequential)

**Parallelizable:** T-001, T-002, T-003, T-004 can be created in parallel

---

## Task List

### Category 1: File Creation (T-001 to T-008)

#### [T-001] Create or verify .editorconfig (IDEMPOTENT)
- **Status:** [ ] Pending
- **activeForm:** Creating or verifying .editorconfig
- **Path:** `OfficeAutomator/.editorconfig`
- **Size:** ~30 lines
- **Content source:** Phase 5 STRATEGY S-001 (Makefile strategy section)
- **Idempotency pattern:**
  - IF file exists AND contains `[Makefile] indent_style = tab` THEN exit 0
  - ELSE create/overwrite with correct content
- **Done criteria (observable):**
  - [ ] Command: `grep -q "indent_style = tab" .editorconfig && echo "OK" || echo "FAIL"`
  - [ ] Output: `OK`
  - [ ] Command: `grep -q "\[Makefile\]" .editorconfig && echo "OK" || echo "FAIL"`
  - [ ] Output: `OK`
  - [ ] File is committed
- **Dependencies:** None
- **Risk:** None
- **Notes:** Standalone, can be created first. Running 2x produces same result.

---

#### [T-002] Create or verify scripts/verify-environment.sh (IDEMPOTENT)
- **Status:** [ ] Pending
- **activeForm:** Creating or verifying scripts/verify-environment.sh
- **Path:** `OfficeAutomator/scripts/verify-environment.sh`
- **Size:** ~40 lines
- **Content source:** Phase 5 STRATEGY S-002 (verify-environment.sh strategy)
- **Idempotency pattern:**
  - IF file exists AND is executable AND contains VP-001, VP-002, VP-003 checks THEN exit 0
  - ELSE create/overwrite with correct content and make executable
- **Done criteria (observable):**
  - [ ] Command: `test -x scripts/verify-environment.sh && echo "OK" || echo "FAIL"`
  - [ ] Output: `OK` (file exists and is executable)
  - [ ] Command: `bash scripts/verify-environment.sh 2>&1 | grep -q "All pre-flight checks passed" && echo "OK" || echo "FAIL"`
  - [ ] Output: `OK` (script runs successfully on current system)
  - [ ] File is committed
- **Dependencies:** None (scripts/ directory must exist)
- **Risk:** Network check may be slow (~5s timeout) — acceptable for PILOT phase
- **Notes:** Script must exit 0 on success, 1 on failure. Running 2x on same system = same output.

---

#### [T-003] Create or verify scripts/setup.sh (IDEMPOTENT)
- **Status:** [ ] Pending
- **activeForm:** Creating or verifying scripts/setup.sh
- **Path:** `OfficeAutomator/scripts/setup.sh`
- **Size:** ~50 lines
- **Content source:** Phase 5 STRATEGY S-003 (setup.sh idempotent pattern)
- **Idempotency pattern:**
  - IF file exists AND is executable AND contains idempotent check pattern THEN exit 0
  - ELSE create/overwrite with correct content
- **Done criteria (observable):**
  - [ ] Command: `test -x scripts/setup.sh && echo "OK" || echo "FAIL"`
  - [ ] Output: `OK` (file exists and is executable)
  - [ ] Command: `grep -q "command -v dotnet" scripts/setup.sh && echo "OK" || echo "FAIL"`
  - [ ] Output: `OK` (idempotency check pattern present)
  - [ ] Command: `grep -q "DOTNET_ROOT" scripts/setup.sh && echo "OK" || echo "FAIL"`
  - [ ] Output: `OK` (environment variable setup present)
  - [ ] File is committed
- **Dependencies:** None (scripts/ directory must exist)
- **Risk:** Script requires curl/internet access (validated by T-002 pre-flight)
- **Notes:** Script contains idempotency check: if .NET 8.0.* exists, exit 0 without reinstalling. This is critical for Phase 9 testing and Phase 10 real execution.

---

#### [T-004] Create or verify scripts/cleanup.sh (IDEMPOTENT, OPTIONAL)
- **Status:** [ ] Pending
- **activeForm:** Creating or verifying scripts/cleanup.sh
- **Path:** `OfficeAutomator/scripts/cleanup.sh`
- **Size:** ~20 lines
- **Content source:** Phase 6 PLAN P-004 (optional cleanup script)
- **Idempotency pattern:**
  - IF file exists AND is executable THEN exit 0
  - ELSE create/overwrite with correct content
- **Done criteria (observable):**
  - [ ] Command: `test -x scripts/cleanup.sh && echo "OK" || echo "FAIL"`
  - [ ] Output: `OK` (file exists and is executable)
  - [ ] Command: `bash scripts/cleanup.sh && echo "OK" || echo "FAIL"` (runs without error)
  - [ ] Output: `OK`
  - [ ] File is committed
- **Dependencies:** None (scripts/ directory must exist)
- **Risk:** Cleanup is destructive — test carefully, but does not affect .NET installation
- **Notes:** OPTIONAL task — can defer to Phase 10 if needed for timeline. Idempotent: running 2x produces same result (no bins/objs left).

---

#### [T-005] Create or verify Makefile (IDEMPOTENT)
- **Status:** [ ] Pending
- **activeForm:** Creating or verifying Makefile
- **Path:** `OfficeAutomator/Makefile`
- **Size:** ~100 lines
- **Content source:** Phase 5 STRATEGY S-001 (Makefile strategy)
- **Idempotency pattern:**
  - IF file exists AND contains .PHONY targets THEN exit 0
  - ELSE create/overwrite with correct content
- **Done criteria (observable):**
  - [ ] Command: `test -f Makefile && echo "OK" || echo "FAIL"`
  - [ ] Output: `OK`
  - [ ] Command: `make help 2>&1 | grep -q "setup\|test\|clean" && echo "OK" || echo "FAIL"`
  - [ ] Output: `OK` (targets are discoverable)
  - [ ] Command: `grep -q "\.PHONY:" Makefile && echo "OK" || echo "FAIL"`
  - [ ] Output: `OK` (.PHONY declaration present)
  - [ ] File is committed
- **Dependencies:** T-002, T-003 (scripts must exist for Makefile to reference them)
- **Risk:** Makefile tab indentation is fragile — verified by .editorconfig from T-001
- **Notes:** Central orchestration point. Running 2x produces same targets and same behavior.

---

#### [T-006] Verify or update global.json (IDEMPOTENT)
- **Status:** [ ] Pending
- **activeForm:** Verifying or updating global.json
- **Path:** `OfficeAutomator/global.json`
- **Size:** ~10 lines
- **Content source:** Phase 5 STRATEGY S-004 (global.json strategy)
- **Idempotency pattern:**
  - IF file exists AND contains `"version": "8.0.100"` AND `"rollForward": "patch"` THEN exit 0 (no change)
  - ELSE update/create with correct content
- **Done criteria (observable):**
  - [ ] Command: `test -f global.json && echo "OK" || echo "FAIL"`
  - [ ] Output: `OK`
  - [ ] Command: `grep -q '"version".*"8.0' global.json && echo "OK" || echo "FAIL"`
  - [ ] Output: `OK` (version is set to 8.0.x)
  - [ ] Command: `grep -q '"rollForward".*"patch"' global.json && echo "OK" || echo "FAIL"`
  - [ ] Output: `OK` (rollForward is patch)
  - [ ] File is committed (if changes made) or note added that no changes needed
- **Dependencies:** None (standalone verification, no dependency on Makefile)
- **Risk:** File may already be correct — verify before committing
- **Notes:** Version pinning guarantees all developers get .NET 8.0.1xx (exact patch). Running 2x: first may update, second detects no changes needed.

---

#### [T-007] Verify or update README.md (IDEMPOTENT)
- **Status:** [ ] Pending
- **activeForm:** Verifying or updating README.md
- **Path:** `OfficeAutomator/README.md`
- **Size:** Add ~5-10 lines
- **Content source:** Phase 6 PLAN U-001 (README update)
- **Idempotency pattern:**
  - IF file exists AND contains Quick Start section with `make setup` THEN exit 0 (no change)
  - ELSE insert Quick Start section early in file
- **Done criteria (observable):**
  - [ ] Command: `test -f README.md && echo "OK" || echo "FAIL"`
  - [ ] Output: `OK`
  - [ ] Command: `grep -q "make setup" README.md && echo "OK" || echo "FAIL"`
  - [ ] Output: `OK` (Quick Start section present)
  - [ ] Command: `grep -q "CONTRIBUTING.md" README.md && echo "OK" || echo "FAIL"`
  - [ ] Output: `OK` (reference to CONTRIBUTING.md present)
  - [ ] Markdown syntax valid (no broken links)
  - [ ] File is committed (if changes made) or note added
- **Dependencies:** None (can be independent, but documenting Makefile targets from T-005)
- **Risk:** Existing README may have different structure — must preserve all original content
- **Notes:** Update is minimal addition. Running 2x: first inserts section, second detects section exists and skips.

---

#### [T-008] Create or verify docs/CONTRIBUTING.md (IDEMPOTENT)
- **Status:** [ ] Pending
- **activeForm:** Creating or verifying docs/CONTRIBUTING.md
- **Path:** `OfficeAutomator/docs/CONTRIBUTING.md`
- **Size:** ~150 lines
- **Content source:** Phase 5 STRATEGY S-005 (CONTRIBUTING.md strategy)
- **Idempotency pattern:**
  - IF file exists AND contains Quick Start, Prerequisites, Troubleshooting sections THEN exit 0
  - ELSE create/overwrite with correct content
- **Done criteria (observable):**
  - [ ] Command: `test -f docs/CONTRIBUTING.md && echo "OK" || echo "FAIL"`
  - [ ] Output: `OK`
  - [ ] Command: `grep -q "Quick Start" docs/CONTRIBUTING.md && echo "OK" || echo "FAIL"`
  - [ ] Output: `OK`
  - [ ] Command: `grep -q "Prerequisites" docs/CONTRIBUTING.md && echo "OK" || echo "FAIL"`
  - [ ] Output: `OK`
  - [ ] Command: `grep -q "Troubleshooting" docs/CONTRIBUTING.md && echo "OK" || echo "FAIL"`
  - [ ] Output: `OK`
  - [ ] Markdown syntax valid (no broken internal links)
  - [ ] File is committed
- **Dependencies:** None (documents Makefile, but doesn't require Makefile to exist)
- **Risk:** Documentation accuracy is critical — new developers rely on this
- **Notes:** Primary developer onboarding guide. Running 2x: first creates, second detects sections exist and skips.

---

### Category 2: Preliminary Testing — PHASE 9 PILOT/VALIDATE (T-009 to T-011)

**IMPORTANT:** These tasks are PHASE 9 PILOT validation, NOT Phase 10 implementation.
- T-009 to T-011 verify scripts are correct and Makefile targets work
- They do NOT execute real .NET installation (that's Phase 10)
- They are idempotent and observable

#### [T-009] Validate individual scripts are executable and working (IDEMPOTENT)
- **Status:** [ ] Pending
- **activeForm:** Validating individual scripts
- **Scope:** Test each script independently — verify executability, not real installation
- **Phase:** Phase 9 PILOT/VALIDATE
- **Done criteria (observable, idempotent):**
  - [ ] **scripts/verify-environment.sh validation:**
    - Command: `bash scripts/verify-environment.sh 2>&1 | head -1`
    - Expected: Should not error (exit 0) or show pre-flight failure (exit 1)
    - Idempotent: Running 2x on same system produces same output
  - [ ] **scripts/setup.sh validation:**
    - Command: `bash scripts/setup.sh --version 2>&1 | head -1` OR check script syntax
    - Expected: Script should be syntactically correct
    - Idempotent: Not executing install, just validating script exists
  - [ ] **scripts/cleanup.sh validation:**
    - Command: `bash -n scripts/cleanup.sh && echo "OK"` (syntax check, no execution)
    - Expected: `OK` (script is syntactically valid)
    - Idempotent: No side effects
- **Dependencies:** T-002, T-003, T-004 (scripts must exist)
- **Risk:** Pre-flight validation only — NOT real .NET installation
- **Notes:** Running 2x produces identical output. This is PILOT testing, not implementation.

---

#### [T-010] Validate Makefile targets are discoverable and invocable (IDEMPOTENT)
- **Status:** [ ] Pending
- **activeForm:** Validating Makefile targets
- **Scope:** Verify Makefile targets exist and are invocable (NOT executing them)
- **Phase:** Phase 9 PILOT/VALIDATE
- **Done criteria (observable, idempotent, NO side effects):**
  - [ ] **make help validation:**
    - Command: `make help 2>&1`
    - Expected: Outputs "setup", "test", "clean", "help"
    - Idempotent: Command has no side effects, can run 2x with same output
  - [ ] **Makefile syntax validation:**
    - Command: `make -n setup 2>&1 | head -3` (dry-run, shows what would execute)
    - Expected: Shows command structure without executing
    - Idempotent: No actual execution (dry-run only)
  - [ ] **Makefile structure validation:**
    - Command: `grep -c "^\.PHONY:" Makefile && echo "OK"`
    - Expected: `1` + `OK` (PHONY declaration exists)
    - Idempotent: No side effects
- **Dependencies:** T-005 (Makefile must exist)
- **Risk:** None — validation only, no installation
- **Notes:** This is PILOT testing. Real `make setup` execution (with .NET installation) happens in Phase 10. Running 2x produces identical validation results.

---

#### [T-011] Validate idempotency design in scripts (IDEMPOTENT)
- **Status:** [ ] Pending
- **activeForm:** Validating idempotency design in scripts
- **Scope:** Verify scripts CONTAIN idempotency patterns (not executing real install)
- **Phase:** Phase 9 PILOT/VALIDATE
- **Done criteria (observable, idempotent, NO installation):**
  - [ ] **scripts/setup.sh idempotency check pattern:**
    - Command: `grep -A2 "command -v dotnet" scripts/setup.sh | head -5`
    - Expected: Script contains `if command -v dotnet` check before installing
    - Idempotent: No side effects, just reading code
  - [ ] **scripts/setup.sh version match pattern:**
    - Command: `grep "8.0" scripts/setup.sh && echo "OK"`
    - Expected: Script checks for .NET 8.0.* (not generic version)
    - Idempotent: No side effects, just reading code
  - [ ] **scripts/setup.sh early exit pattern:**
    - Command: `grep -q "exit 0" scripts/setup.sh && echo "OK"`
    - Expected: Script exits cleanly if already installed
    - Idempotent: No side effects, just reading code
- **Dependencies:** T-003 (scripts/setup.sh must exist)
- **Risk:** None — validation only
- **Notes:** This validates DESIGN of idempotency, not real execution. Actual idempotency test (running make setup twice with real .NET install) happens in Phase 10. Running 2x produces identical validation results.

---

### Category 3: Integration & Commit (T-012)

#### [T-012] Commit and verify all changes (IDEMPOTENT)
- **Status:** [ ] Pending
- **activeForm:** Committing and verifying changes
- **Scope:** Final integration and git operations for Phase 9 PILOT
- **Phase:** Phase 9 PILOT/VALIDATE (closure)
- **Done criteria (observable, idempotent):**
  - [ ] Command: `git status --short` shows no untracked files (or only .gitignore entries)
  - [ ] Expected: Only staged/committed files listed
  - [ ] Command: `test -x scripts/verify-environment.sh && echo "OK" || echo "FAIL"`
  - [ ] Expected: `OK` (scripts are executable)
  - [ ] Command: `test -x scripts/setup.sh && echo "OK" || echo "FAIL"`
  - [ ] Expected: `OK`
  - [ ] Command: `git log --oneline -5 | head -1`
  - [ ] Expected: Shows latest commit with descriptive message
  - [ ] Command: `git push origin {branch} 2>&1 | grep -q "up to date\|committed" && echo "OK"`
  - [ ] Expected: `OK` (push succeeded or already up-to-date)
- **Dependencies:** T-001 through T-011 (all files created/verified)
- **Risk:** Push may fail if branch protection exists — verify beforehand
- **Notes:** Running 2x: first commits changes, second detects no changes (idempotent). This closes Phase 9 PILOT/VALIDATE.

---

## Summary

| Category | Tasks | Phase | Idempotent? | Duration | Risk |
|----------|-------|-------|------------|----------|------|
| File Creation | T-001 to T-008 | Phase 9 PILOT | ✓ YES (all idempotent) | 30-45 min | Low — validation only |
| Preliminary Testing | T-009 to T-011 | Phase 9 PILOT | ✓ YES (validation only, no side effects) | 10-15 min | Low — no installation |
| Integration | T-012 | Phase 9 PILOT | ✓ YES (idempotent git ops) | 5-10 min | Low — branch push |
| **TOTAL** | **12 tasks** | **Phase 9 PILOT/VALIDATE** | **✓ ALL IDEMPOTENT** | **45-70 min** | **Low** |

**CRITICAL CLARIFICATION:**
- **Phase 8 (COMPLETED):** Created this task-plan with atomic, idempotent, observable tasks
- **Phase 9 (NEXT):** Execute T-001 to T-012 — file creation + preliminary validation (NO .NET installation)
- **Phase 10 (AFTER):** Real implementation — execute `make setup` (installs .NET), `make test` (220+ tests)
- **Phase 11 (FINAL):** Document results and close work package

---

## Exit Criteria — Phase 9 PILOT/VALIDATE → Phase 10 IMPLEMENT

✓ All 8 files created/verified (T-001 to T-008) with observable criteria
✓ All files are idempotent (running 2x = running 1x, no side effects)
✓ Makefile structure validated: targets discoverable, dry-run executable (T-010)
✓ Scripts structure validated: executability, idempotency patterns present (T-009, T-011)
✓ Global.json version pinning verified (T-006)
✓ README.md Quick Start section added (T-007)
✓ All changes committed with conventional messages (T-012)
✓ Git history clean, no merge conflicts

**CRITICAL:** Phase 9 PILOT is validation-only. No .NET installation occurs in Phase 9.
Real installation and test execution happens in Phase 10 IMPLEMENT.

**Gate decision:** Ready for Phase 10 IMPLEMENT (actual .NET 8.0 installation + `make test` execution)?

---

## Phase Responsibilities

### Phase 8 (COMPLETED): Create atomic, idempotent, observable task-plan
- ✓ Decomposed scope into 12 tasks with clear done criteria
- ✓ Made each task idempotent (run 2x = run 1x)
- ✓ Made criteria observable (grep, test, bash commands, not subjective)
- ✓ Separated PILOT (Phase 9) from IMPLEMENT (Phase 10)

### Phase 9 (NEXT): PILOT/VALIDATE — execute T-001 to T-012
- Execute file creation tasks (T-001 to T-008)
- Validate scripts are correct and Makefile structure is sound (T-009 to T-011)
- Commit changes (T-012)
- **NO .NET installation, NO make test execution**

### Phase 10 (AFTER Phase 9): IMPLEMENT — real installation + tests
- Execute `make setup` (installs .NET 8.0 to ~/.dotnet)
- Verify .NET 8.0 installation successful
- Execute `make test` against actual 220+ test suite
- Capture results, analyze failures
- **Real execution, real side effects, real outcomes**

### Phase 11 (FINAL): TRACK/EVALUATE — document results
- Lessons learned
- Issues encountered and resolutions
- Close work package
