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

This task plan decomposes Phase 6 PLAN scope into 12 atomic tasks for Phase 8 execution. Each task is independently testable and has clear completion criteria.

**Total work:** 12 tasks across 3 categories
- **File creation:** T-001 through T-008 (7 new files + 2 updates)
- **Individual testing:** T-009 through T-011 (component validation)
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

#### [T-001] Create .editorconfig
- **Status:** [ ] Pending
- **activeForm:** Creating .editorconfig
- **Path:** `OfficeAutomator/.editorconfig`
- **Size:** ~30 lines
- **Content source:** Phase 5 S-001 (Makefile strategy) — `.editorconfig` section
- **Done criteria:**
  - [ ] File created at root
  - [ ] Contains tab enforcement for Makefile: `[Makefile] indent_style = tab`
  - [ ] Contains space enforcement for shell/markdown: 2-space indent
  - [ ] File is committed
- **Dependencies:** None
- **Risk:** None
- **Notes:** Standalone, can be created first

---

#### [T-002] Create scripts/verify-environment.sh
- **Status:** [ ] Pending
- **activeForm:** Creating scripts/verify-environment.sh
- **Path:** `OfficeAutomator/scripts/verify-environment.sh`
- **Size:** ~40 lines
- **Content source:** Phase 5 S-002 (verify-environment strategy)
- **Done criteria:**
  - [ ] File created in scripts/ directory
  - [ ] Executable bit set: `chmod +x scripts/verify-environment.sh`
  - [ ] VP-001 check (disk space >1GB) implemented
  - [ ] VP-002 check (network connectivity to api.nuget.org) implemented
  - [ ] VP-003 check (bash version 4.0+) implemented
  - [ ] Script exits with code 1 on ANY failure (fail-fast)
  - [ ] Script outputs success message on completion
  - [ ] File is committed
- **Dependencies:** None
- **Risk:** Network check may be slow (~5s timeout)
- **Notes:** Critical for Phase 10 pre-flight validation

---

#### [T-003] Create scripts/setup.sh
- **Status:** [ ] Pending
- **activeForm:** Creating scripts/setup.sh
- **Path:** `OfficeAutomator/scripts/setup.sh`
- **Size:** ~50 lines
- **Content source:** Phase 5 S-003 (setup.sh idempotent pattern)
- **Done criteria:**
  - [ ] File created in scripts/ directory
  - [ ] Executable bit set: `chmod +x scripts/setup.sh`
  - [ ] Idempotency check implemented: if dotnet --version matches 8.0.*, exit 0
  - [ ] Installation via dotnet-install.sh to ~/.dotnet
  - [ ] Environment variables set: DOTNET_ROOT, PATH, telemetry flags
  - [ ] Verification step: `dotnet --version` output shown
  - [ ] Error handling: script exits 1 on install failure
  - [ ] File is committed
- **Dependencies:** None
- **Risk:** Script requires curl access (handled by T-002 pre-flight)
- **Notes:** Idempotency is critical — must pass T-011 twice-run test

---

#### [T-004] Create scripts/cleanup.sh (OPTIONAL)
- **Status:** [ ] Pending
- **activeForm:** Creating scripts/cleanup.sh
- **Path:** `OfficeAutomator/scripts/cleanup.sh`
- **Size:** ~20 lines
- **Content source:** Phase 6 PLAN P-004 (optional cleanup script)
- **Done criteria:**
  - [ ] File created in scripts/ directory
  - [ ] Executable bit set
  - [ ] Removes build artifacts: rm -rf src/OfficeAutomator.Core/bin obj
  - [ ] (OPTIONAL) Removes NuGet cache: rm -rf ~/.nuget/packages
  - [ ] File is committed
- **Dependencies:** None
- **Risk:** Artifact cleanup is destructive — ensure documentation clear
- **Notes:** OPTIONAL task — can defer to Phase 10 if needed for timeline

---

#### [T-005] Create Makefile
- **Status:** [ ] Pending
- **activeForm:** Creating Makefile
- **Path:** `OfficeAutomator/Makefile`
- **Size:** ~100 lines
- **Content source:** Phase 5 S-001 (Makefile strategy)
- **Done criteria:**
  - [ ] File created at root
  - [ ] Targets defined: `.PHONY: help setup verify-env install-sdk test clean`
  - [ ] `make help` outputs list of targets
  - [ ] `make setup` calls scripts/verify-environment.sh then scripts/setup.sh
  - [ ] `make test` runs `dotnet test` from src/OfficeAutomator.Core/
  - [ ] `make clean` removes bin/ and obj/ directories
  - [ ] Tab indentation enforced (per .editorconfig from T-001)
  - [ ] All command output is descriptive
  - [ ] File is committed
- **Dependencies:** T-002, T-003 (scripts must exist)
- **Risk:** Makefile tab indentation is fragile — manual verification needed
- **Notes:** Central orchestration point — must be correct

---

#### [T-006] Update global.json (Verify/Update)
- **Status:** [ ] Pending
- **activeForm:** Updating global.json
- **Path:** `OfficeAutomator/global.json`
- **Size:** ~10 lines
- **Content source:** Phase 5 S-004 (global.json strategy)
- **Done criteria:**
  - [ ] File exists at root
  - [ ] Contains: `"version": "8.0.100"`
  - [ ] Contains: `"rollForward": "patch"`
  - [ ] JSON is valid (no trailing commas)
  - [ ] File is committed (if changes made) OR note left that no changes needed
- **Dependencies:** None (standalone verification)
- **Risk:** File may already be correct — only commit if changed
- **Notes:** Version pinning guarantees reproducibility

---

#### [T-007] Update README.md (Add Quick Start)
- **Status:** [ ] Pending
- **activeForm:** Updating README.md
- **Path:** `OfficeAutomator/README.md`
- **Size:** Add ~5-10 lines
- **Content source:** Phase 6 PLAN U-001 (README update)
- **Done criteria:**
  - [ ] File exists at root
  - [ ] Quick Start section added early (after title/description)
  - [ ] Contains: `make setup` and `make test` commands
  - [ ] References: `see [CONTRIBUTING.md](docs/CONTRIBUTING.md)` for details
  - [ ] Markdown syntax is valid
  - [ ] File is committed
- **Dependencies:** T-005 (Makefile must exist to reference)
- **Risk:** Existing README may have different structure — preserve existing content
- **Notes:** Update is minimal — preserve original README structure

---

#### [T-008] Create docs/CONTRIBUTING.md
- **Status:** [ ] Pending
- **activeForm:** Creating docs/CONTRIBUTING.md
- **Path:** `OfficeAutomator/docs/CONTRIBUTING.md`
- **Size:** ~150 lines
- **Content source:** Phase 5 S-005 (CONTRIBUTING.md strategy)
- **Done criteria:**
  - [ ] File created in docs/ directory
  - [ ] Quick Start section: `make setup`, `make test`
  - [ ] Prerequisites section: OS, Bash version, Disk, Network
  - [ ] What `make setup` does: 5 steps documented
  - [ ] Troubleshooting section: T-001 through T-005 issues with solutions
  - [ ] macOS-specific section: Bash 3.2 vs 4.0+ upgrade
  - [ ] Development workflow documented
  - [ ] File is committed
- **Dependencies:** T-005 (Makefile must exist to document)
- **Risk:** Documentation should be 100% accurate or developers get stuck
- **Notes:** Primary developer onboarding guide — must be tested in Phase 10

---

### Category 2: Individual Testing (T-009 to T-011)

#### [T-009] Test individual scripts
- **Status:** [ ] Pending
- **activeForm:** Testing individual scripts
- **Scope:** Test each script independently
- **Done criteria:**
  - [ ] **scripts/verify-environment.sh:**
    - Run on current system: `bash scripts/verify-environment.sh`
    - Should output: `✓ All pre-flight checks passed`
    - Should exit code 0
  - [ ] **scripts/setup.sh:**
    - Run on current system: `bash scripts/setup.sh`
    - Should detect if .NET already installed
    - Should output success message or "already installed"
    - Should exit code 0
  - [ ] **scripts/cleanup.sh** (if created):
    - Run: `bash scripts/cleanup.sh`
    - Should complete without error
    - Should exit code 0
- **Dependencies:** T-002, T-003, T-004
- **Risk:** Tests are manual — dependent on system state
- **Notes:** These tests are pre-integration validation

---

#### [T-010] Test Makefile targets
- **Status:** [ ] Pending
- **activeForm:** Testing Makefile targets
- **Scope:** Test each Makefile target individually
- **Done criteria:**
  - [ ] **make help:**
    - Run: `make help`
    - Should output list of available targets
    - Should be human-readable
  - [ ] **make setup:**
    - Run: `make setup`
    - Should call verify-environment.sh
    - Should call setup.sh
    - Should output final success message
    - Should exit code 0
  - [ ] **make test:**
    - Run: `make test`
    - Should invoke `dotnet test` from correct directory
    - Should report test counts
  - [ ] **make clean:**
    - Run: `make clean`
    - Should remove bin/ and obj/ directories
    - Should exit code 0
- **Dependencies:** T-005 (Makefile), T-009 (scripts validated)
- **Risk:** `make test` may fail if compilation has issues — that's Phase 10
- **Notes:** Focus on Makefile structure, not test results

---

#### [T-011] Test full idempotency
- **Status:** [ ] Pending
- **activeForm:** Testing full idempotency
- **Scope:** Run entire setup twice, verify same result
- **Done criteria:**
  - [ ] **First run:**
    - Execute: `make setup`
    - Note output
  - [ ] **Second run (immediately after):**
    - Execute: `make setup` again
    - Should output "already installed" or same success
    - Should NOT re-download or reinstall
    - Should exit code 0
  - [ ] **Verify idempotency:**
    - Both runs should have identical outcome
    - No state changes on second run (check ~/.dotnet timestamp)
- **Dependencies:** T-010 (Makefile targets must pass)
- **Risk:** Timing-sensitive — system state must be stable
- **Notes:** CRITICAL test for production readiness

---

### Category 3: Integration & Commit (T-012)

#### [T-012] Commit and verify all changes
- **Status:** [ ] Pending
- **activeForm:** Committing and verifying changes
- **Scope:** Final integration and git operations
- **Done criteria:**
  - [ ] All 8 files created/updated are in working directory
  - [ ] `git status` shows no untracked files (all should be staged or committed)
  - [ ] All files have correct permissions: scripts/ are executable (`-x`)
  - [ ] All files use conventional formatting (tabs in Makefile, spaces elsewhere)
  - [ ] Commit messages follow conventional format: `feat(verify-test-execution): create Phase 8 PLAN EXECUTION files`
  - [ ] All commits grouped logically (optional: one commit per file or one commit for all)
  - [ ] `git log` shows commits with proper messages
  - [ ] No merge conflicts
  - [ ] Push to origin completed: `git push -u origin {branch}`
- **Dependencies:** T-001 through T-011 (all tasks)
- **Risk:** Push may fail due to branch protection — handle gracefully
- **Notes:** This task closes Phase 8

---

## Summary

| Category | Tasks | Parallelizable | Duration | Risk |
|----------|-------|-----------------|----------|------|
| File Creation | T-001 to T-008 | T-001, T-002, T-003, T-004 | 30-45 min | Makefile tabs (T-005) |
| Testing | T-009 to T-011 | Sequential (dependencies) | 15-30 min | Idempotency (T-011) |
| Integration | T-012 | Sequential | 5-10 min | Push failure |
| **TOTAL** | **12 tasks** | **Parallel + sequential** | **50-85 min** | **Low** |

---

## Exit Criteria — Phase 8 → Phase 10

✓ All 7 files created with exact content from Phase 5
✓ All 2 files updated with correct additions
✓ Individual scripts pass T-009
✓ Makefile targets pass T-010
✓ Full idempotency verified in T-011
✓ All changes committed with conventional messages
✓ Git history clean, no merge conflicts

**Gate decision:** Ready for Phase 10 IMPLEMENT (actual .NET 8.0 installation + test execution)?

---

## Notes for Phase 10 IMPLEMENT

Phase 10 will:
1. Execute `make setup` on actual Ubuntu system
2. Verify .NET 8.0 installation
3. Execute `make test` and capture results
4. Document any errors or issues
5. Iterate if needed

Phase 8 (this plan) is **file creation and pre-flight testing only**.
Actual installation and testing happens in Phase 10.
