```yml
created_at: 2026-04-22 05:12:00
project: OfficeAutomator
work_package: 2026-04-22-04-46-55-project-structure-cleanup
phase: Phase 8 — PLAN EXECUTION
author: Claude Code
status: Aprobado
```

# Phase 8: PLAN EXECUTION — Project Structure Cleanup

## Overview

Atomic task plan for executing the structural cleanup decisions from Phase 1: DISCOVER.
All tasks are ordered sequentially with minimal risk (all git-preserved).

**Total estimated execution time:** 30-45 minutes
**Risk level:** VERY LOW (git preserves all changes)
**Reversibility:** 100% (git history)

---

## Task Dependency Graph

```
T-001 (Move .txt) ──┐
T-002 (Move docs) ──┼── T-004 (Update README) ── T-005 (Delete WPs) ── T-006 (Clean INDEX) ── T-007 (Validate)
T-003 (Delete .ps1)─┘
```

**Execution order:** T-001 → T-002 → T-003 → T-004 → T-005 → T-006 → T-007

---

## Tasks

### T-001: Move .txt files to work packages (trazabilidad preserved)

**Status:** [ ] Pending

**Description:** Archive 3 historical .txt files from root to their respective work packages.
Preserves git history while cleaning root.

**Files to move:**

1. `ESTRUCTURA_PROYECTO.txt` → `.thyrox/context/work/2026-04-21-11-15-00-project-structure-reorganization/`
   - Reason: WP sobre reorganización de estructura del proyecto
   - Content type: Artifact histórico
   - Commit message: `chore(root): move ESTRUCTURA_PROYECTO.txt to WP for historical context`

2. `PROYECTO_ESTRUCTURA_ACTUAL.txt` → `.thyrox/context/work/2026-04-21-11-15-00-project-structure-reorganization/`
   - Reason: Snapshot histórico de estructura
   - Content type: Artifact histórico
   - Commit message: `chore(root): move PROYECTO_ESTRUCTURA_ACTUAL.txt to WP for trazabilidad`

3. `SPRINT1-CORRECTION-COMPLETE.txt` → `.thyrox/context/work/2026-04-21-06-15-00-design-specification-correct/`
   - Reason: Documento de cierre de sprint, contexto histórico de correcciones
   - Content type: Artifact histórico
   - Commit message: `chore(root): move SPRINT1-CORRECTION-COMPLETE.txt to design-specification WP`

**Validation:** `git status` should show 3 renamed files

---

### T-002: Move documentation files to docs/

**Status:** [ ] Pending

**Description:** Move 2 documentation files from root to `docs/` directory.

**Files to move:**

1. `EXECUTION_GUIDE.md` → `docs/EXECUTION_GUIDE.md`
   - Content: Documentación de proyecto
   - Commit message: `docs(root): move EXECUTION_GUIDE.md to docs/`

2. `TEST_EXECUTION_REPORT.md` → `docs/TEST_EXECUTION_REPORT.md`
   - Content: Reportes de testing
   - Commit message: `docs(root): move TEST_EXECUTION_REPORT.md to docs/`

**Validation:** 
- `test -f docs/EXECUTION_GUIDE.md && echo OK`
- `test -f docs/TEST_EXECUTION_REPORT.md && echo OK`

---

### T-003: Delete fix-failing-tests.ps1

**Status:** [ ] Pending

**Description:** Remove temporary utility script from root.
Git preserves it in history if needed.

**File to delete:** `fix-failing-tests.ps1`
- Reason: Script temporal de utilidad sin referencias activas
- Evidence: Zero grep results in .cs, .csproj, .ps1, .sh, .md, config

**Validation:** `test ! -f fix-failing-tests.ps1 && echo OK`

**Commit message:** `chore(root): remove temporary fix-failing-tests.ps1 script`

---

### T-004: Update README.md (fix paths + remove emojis)

**Status:** [ ] Pending

**Description:** Correct 3 path errors and remove 8 emoji checkmarks.

**Corrections:**

1. **Lines 31, 35:** Path error
   - OLD: `cd OfficeAutomator/OfficeAutomator.Core`
   - NEW: `cd src/OfficeAutomator.Core`

2. **Lines 126-129:** Documentation location
   - OLD: Reference to `OfficeAutomator.Core/`
   - NEW: Change to `docs/`

3. **Line 259:** Documentation statement
   - OLD: "All documentation is in the `OfficeAutomator.Core/` directory"
   - NEW: "Documentation is in docs/"

4. **Remove 8 emoji checkmarks:**
   - Line 77: `✓ ALL TESTS PASSED ✓` → remove emojis
   - Line 81: emoji in table → remove
   - Lines 134, 140, 146, 152, 159: `✓ UC-00X` → remove emojis
   - Line 295: `✓ Windows | ✓ macOS | ✓ Linux | ✓ Docker | ✓ Cloud` → remove all ✓
   - Lines 316-317: Table Project Status → remove emojis

**Commit message:** `docs(readme): fix 3 path errors and remove emoji checkmarks (convention compliance)`

**Validation:** `grep -E '✓|✗|❌' README.md | wc -l` should be 0

---

### T-005: Delete 66 work packages (historical cleanup)

**Status:** [ ] Pending

**Description:** Remove THYROX framework WPs, test WPs, and aborted iterations.
Leaves 6 OfficeAutomator project WPs intact.

**Categories to delete:**

1. **THYROX Framework (59 WPs):** 2026-03-27 through 2026-04-20
   - Pattern: project: THYROX in metadata
   - Date range: Before 2026-04-21-01-30-00 (the corte point)
   - Action: `rm -rf .thyrox/context/work/2026-03-27-* .thyrox/context/work/2026-04-0[1-9]*-* .thyrox/context/work/2026-04-1[0-9]*-* .thyrox/context/work/2026-04-2[0]-*`

2. **Test WP (1):** `test-pdca-worktree-2026-04-16-20-06-26`
   - Reason: Testing del framework THYROX, not project
   - Action: `rm -rf .thyrox/context/work/test-pdca-worktree-2026-04-16-20-06-26`

3. **Aborted design-specification (2):**
   - `2026-04-21-04-30-00-design-specification` (v1, replaced 1.5h later)
   - `2026-04-21-06-00-00-design-specification-v2` (v2, replaced 15 min later)
   - Action: `rm -rf .thyrox/context/work/2026-04-21-04-30-00-design-specification .thyrox/context/work/2026-04-21-06-00-00-design-specification-v2`

4. **Option-B Analysis (4):** (Evaluated but NOT adopted)
   - `2026-04-21-14-30-00-option-b-powershell-wrapper-analysis`
   - `2026-04-21-14-31-00-option-b-requirements-specification`
   - `2026-04-21-14-32-00-option-b-detailed-design`
   - `2026-04-21-14-33-00-option-b-dependency-analysis-plan`
   - Reason: NO referencias en design-specification-correct (alternativa evaluada pero NO adoptada)
   - Action: `rm -rf .thyrox/context/work/2026-04-21-14-3*`

**WPs to KEEP (6 total):**
1. ✓ 2026-04-21-01-30-00-uc-documentation
2. ✓ 2026-04-21-03-00-00-scope-definition
3. ✓ 2026-04-21-06-15-00-design-specification-correct
4. ✓ 2026-04-21-11-15-00-project-structure-reorganization
5. ✓ 2026-04-22-04-46-55-project-structure-cleanup (current WP)

**Validation:** `find .thyrox/context/work -maxdepth 1 -type d -name "2026*" | wc -l` should be 5 (+ current = 6 total visible)

**Commit message:** `chore(wp): remove 66 historical work packages (THYROX framework + aborted iterations)`

---

### T-006: Delete obsolete INDEX.md

**Status:** [ ] Pending

**Description:** Remove obsolete work package index.
With 6 WPs post-cleanup, an index is unnecessary.

**File to delete:** `/.thyrox/context/work/INDEX.md`

**Validation:** `test ! -f .thyrox/context/work/INDEX.md && echo OK`

**Commit message:** `chore(work): remove obsolete work package INDEX.md`

---

### T-007: Validate final structure

**Status:** [ ] Pending

**Description:** Verify cleanup was successful.
Final state: clean root, 6 WPs, no broken references.

**Validations:**

1. **Root cleanup (0 contaminants expected):**
   ```bash
   # Should find ZERO files
   ls -la | grep -E "^-.*\.(txt|ps1|props|config)$" | grep -v Directory.Build.props | grep -v nuget.config
   ```

2. **README.md has no emojis:**
   ```bash
   grep -E '✓|✗|❌' README.md | wc -l  # Should be 0
   ```

3. **6 WPs remain:**
   ```bash
   find .thyrox/context/work -maxdepth 1 -type d -name "2026*" | wc -l  # Should be 5 (current + 4 kept)
   ```

4. **Reference checks (if any broken links found, repair):**
   ```bash
   # Check for references to moved files
   grep -r "EXECUTION_GUIDE.md" docs/ 2>/dev/null | grep -v "EXECUTION_GUIDE.md:"
   grep -r "TEST_EXECUTION_REPORT.md" docs/ 2>/dev/null | grep -v "TEST_EXECUTION_REPORT.md:"
   grep -r "ESTRUCTURA_PROYECTO.txt" .thyrox/context/work/ 2>/dev/null
   ```

5. **Git status clean:**
   ```bash
   git status | grep -q "nothing to commit" && echo OK
   ```

**Expected output:** All checks pass, clean working tree

**Commit message (if fixes needed):** `chore(validation): final structure validation and reference repairs`

---

## Execution Summary

| Task | Description | Status | Commit |
|------|-------------|--------|--------|
| T-001 | Move .txt to WPs | [ ] | 3 files renamed |
| T-002 | Move docs to docs/ | [ ] | 2 files moved |
| T-003 | Delete fix-failing-tests.ps1 | [ ] | 1 file removed |
| T-004 | Fix README paths + emojis | [ ] | 1 file modified |
| T-005 | Delete 66 WPs | [ ] | 66 dirs removed |
| T-006 | Delete INDEX.md | [ ] | 1 file removed |
| T-007 | Final validation | [ ] | validate only |

**Total commits:** 6 (one per task, plus validation)

---

## Risk Mitigation (All LOW risk)

| Risk | Mitigation | Status |
|------|-----------|--------|
| Lost content | `git mv` for moves, `git rm` for deletions | Implemented |
| Broken references | Exhaustive grep before/after for key files | Included in T-007 |
| CI/CD paths | No CI/CD active yet; scripts will adapt | Validated |
| User confusion | README updated + detailed commit messages | Included |

---

## Post-Execution Checklist

- [ ] All 7 tasks executed and committed
- [ ] git log shows 6 clean commits with descriptive messages
- [ ] Final structure matches Phase 1 decisions
- [ ] Root has 0 contaminants (only .git, .claude, .thyrox, src, docs, *.md, *.sln, *.json)
- [ ] 6 WPs visible (5 historical + 1 current cleanup)
- [ ] No broken references found
- [ ] Branch pushed to remote

---

## Metadata

**Phase:** 8 — PLAN EXECUTION
**WP:** 2026-04-22-04-46-55-project-structure-cleanup
**Created:** 2026-04-22 05:12:00
**Estimated Duration:** 30-45 minutes
**Reversibility:** 100% (git history)
**Risk Assessment:** VERY LOW
