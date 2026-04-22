```yml
created_at: 2026-04-22 11:00:00
project: OfficeAutomator
work_package: 2026-04-22-06-37-03-resolve-csharp-compilation-errors
phase: Phase 12 — STANDARDIZE
author: Claude
status: Borrador
```

# Phase 12 STANDARDIZE — Reusable Patterns & System Propagation

---

## Executive Summary

**Work Package:** C# Compilation Error Resolution  
**Duration:** 70 minutes across 2 sessions  
**Outcome:** 0 compilation errors, 220/220 tests passing, 3-layer architecture validated

This document captures **4 major reusable patterns** discovered during this WP and identifies **which system guidelines, skills, and ADRs should be updated** to propagate this knowledge.

---

## Pattern 1: Three-Layer Architecture (Bash → PowerShell → C#)

### Definition

Separate concerns by execution layer:

```
Layer 0 (System Bootstrap)    — Bash scripts at ROOT level
  ├── setup.sh                 (Install .NET SDK, system dependencies)
  └── verify-environment.sh    (Pre-flight validation)

Layer 1 (Automation)           — PowerShell scripts in /scripts/
  ├── Install-OfficeAutomator.ps1
  └── functions/               (Helpers, UC-001 through UC-005)

Layer 2 (Core Logic)           — C# DLL in src/
  └── OfficeAutomator.Core.dll (Business logic, loaded via reflection)
```

### Key Insight

**Users intuitively expect execution order from directory structure:**
- Root level = "Run this first" (bootstrapping)
- Subdirectories = "Run this after Layer 0 succeeds" (features)

### Implementation Pattern

**README.md should document:**
```markdown
## Getting Started

### Layer 0: System Setup (Run First)
$ ./setup.sh              # Install .NET SDK
$ ./verify-environment.sh # Validate system

### Layer 1: Automation (Run After)
$ pwsh
PS> . scripts/Install-OfficeAutomator.ps1
PS> Invoke-OfficeAutomator

### Layer 2: Core Logic (Transparent)
(Loaded automatically by Layer 1 via reflection)
```

### Where This Pattern Lives

**Already Created:**
- ✅ `adr-scripts-directory-structure.md` (decision documented)
- ✅ Scripts physically located at root level (implemented)
- ✅ Makefile updated with new paths (verified)

**Needs Creation:**
- 📋 `.claude/rules/three-layer-architecture.md` — System-wide guideline
- 📋 `.thyrox/guidelines/project-structure.instructions.md` — New templates
- 📋 Update README.md with Layer 0/1/2 sections

### Propagation Target

| Target | Action | Files |
|--------|--------|-------|
| Guidelines | Create new | `.thyrox/guidelines/architecture-three-layer.instructions.md` |
| ADR | Already done | `.thyrox/context/decisions/adr-scripts-directory-structure.md` |
| Rules | Create new | `.claude/rules/architecture-three-layer.md` |
| README | Update | Add "Getting Started" with Layer 0/1/2 sections |

### Evidence

**Classification:** INFERRED (architectural best practice)  
**Observable:** Scripts moved, Makefile updated, execution verified with `make verify-env`  
**Confidence:** HIGH (standard in multi-language projects)

---

## Pattern 2: Compilation Cache Prevention (dotnet clean)

### Definition

Stale .dll files in build artifacts can cause tests to fail with incorrect errors.

**Root Cause:** 
```
Old source code → compile → generates DLL in bin/
New source code → edit files → but dotnet reuses old DLL
→ Tests fail with "state not reached" despite correct code
```

**Solution:**
```bash
dotnet clean     # Delete ALL build artifacts (bin/, obj/)
dotnet build     # Recompile from scratch
dotnet test      # Run tests with fresh IL
```

### Key Insight

**Test failures are NOT always code defects.** Before investigating test logic, verify build artifact freshness.

### Implementation Pattern

**Makefile target:**
```makefile
.PHONY: test clean

clean:
	@echo "Cleaning artifacts..."
	@dotnet clean
	@rm -rf src/*/bin src/*/obj

test: clean
	@echo "Running tests..."
	@dotnet test

# OR: Full lifecycle
verify: clean
	@dotnet build
	@dotnet test
```

**CI/CD Pipeline:**
```yaml
test-job:
  steps:
    - run: dotnet clean           # ALWAYS first
    - run: dotnet build
    - run: dotnet test
    - report: Full test counts
```

**Developer Documentation:**
```markdown
## Troubleshooting

### If tests fail locally but pass in CI:
Try: 
  $ make clean && make test

### If you see stale test failures:
The dotnet compiler may be caching old artifacts.
Solution:
  $ dotnet clean
  $ dotnet build
  $ dotnet test
```

### Where This Pattern Lives

**Already Created:**
- ✅ `test-investigation-resolution.md` (evidence from Phase 11)
- ✅ Makefile updated with `clean` target
- ✅ All tests verified passing after clean build

**Needs Creation:**
- 📋 `.claude/rules/compilation-cache-prevention.md` — System rule
- 📋 Update Makefile template with clean-before-test pattern
- 📋 Update CI/CD pipeline template

### Propagation Target

| Target | Action | Files |
|--------|--------|-------|
| Rules | Create new | `.claude/rules/compilation-cache-prevention.md` |
| Guidelines | Create new | `.thyrox/guidelines/csharp-build-practices.instructions.md` |
| README | Update | Add "Troubleshooting" section with cache prevention |
| CI/CD | Update | Add `dotnet clean` step before tests |

### Evidence

**Classification:** PROVEN (reproduced and fixed)  
**Observable:** 
- E2E_013 test fails with stale cache
- Same test passes after `dotnet clean && dotnet build`
- 220/220 tests confirmed passing  
**Confidence:** CRITICAL (affects all .NET projects)

---

## Pattern 3: TDD Cycle Completeness (Red-Green-Refactor)

### Definition

Test-Driven Development has three distinct phases:

```
RED Phase       → Write failing test, see specific error
GREEN Phase     → Implement minimal fix, test passes
REFACTOR Phase  → Improve code clarity WITHOUT adding features
```

### Key Insight

**Cycle completeness requires discipline:**
- RED must be SPECIFIC ("this assertion fails at line X")
- GREEN must be MINIMAL ("smallest change that makes test pass")
- REFACTOR must NOT add features ("only improve clarity")

### TDD Cycles in This WP

| Cycle | Feature | RED | GREEN | REFACTOR |
|-------|---------|-----|-------|----------|
| 1 | XML Declaration | Test expects `<?xml` | Add prepend in ConfigGenerator | No changes |
| 2 | 11-State Coverage | Test reaches only 8 states | Add conditional branches in GetPathToState | Simplify helper logic |
| 3 | E2E Error Recovery | Invalid state sequence | Complete valid path from INIT → INSTALL_READY | Clarify test comments |

### Implementation Pattern

**For C# features in next WP:**

```csharp
// STEP 1: RED — Write test expecting feature
[Fact]
public void ConfigGenerator_Should_Prepend_XmlDeclaration() {
    var generator = new ConfigGenerator();
    var xml = generator.Generate(...);
    
    // This assertion FAILS (RED)
    Assert.StartsWith("<?xml", xml);
}

// STEP 2: GREEN — Implement minimal fix
public string Generate(OfficeConfiguration config) {
    // Minimal: just prepend the declaration
    return "<?xml version=\"1.0\"?>" + GenerateContent(config);
}
// Test now PASSES (GREEN)

// STEP 3: REFACTOR — Improve clarity
public string Generate(OfficeConfiguration config) {
    var xmlDeclaration = "<?xml version=\"1.0\"?>";
    var content = GenerateContent(config);
    return xmlDeclaration + content;
}
// Renamed for clarity, behavior unchanged
```

**Prevention Checklist:**
```
RED Phase:
  [ ] Test is specific (exact assertion at exact line)
  [ ] Test fails with clear error message
  [ ] Error is reproducible

GREEN Phase:
  [ ] Change is minimal (1-5 lines)
  [ ] Only fix, no feature additions
  [ ] Test passes immediately

REFACTOR Phase:
  [ ] Improve clarity (variable names, extract methods)
  [ ] NO new features
  [ ] NO test changes
  [ ] All tests still pass
  [ ] Run full test suite
```

### Where This Pattern Lives

**Already Created:**
- ✅ `resolve-csharp-compilation-errors-tdd-analysis.md` (detailed cycles)
- ✅ All 3 TDD cycles completed and documented
- ✅ 220/220 tests passing validates cycle quality

**Needs Creation:**
- 📋 `.thyrox/guidelines/csharp-tdd-guide.instructions.md` — TDD methodology
- 📋 `.claude/rules/tdd-cycle-completeness.md` — System rule
- 📋 Update Phase 10 EXECUTE guidelines with TDD pattern

### Propagation Target

| Target | Action | Files |
|--------|--------|-------|
| Guidelines | Create new | `.thyrox/guidelines/csharp-tdd-guide.instructions.md` |
| Rules | Create new | `.claude/rules/tdd-cycle-completeness.md` |
| Phase 10 | Update | Add TDD pattern section to EXECUTE skill |
| ADR | Consider | New ADR: "TDD as standard for C# features" |

### Evidence

**Classification:** PROVEN (220/220 tests validate cycles)  
**Observable:**
- All 3 TDD cycles completed successfully
- No test failures, no regressions
- Code quality improved through refactoring
- Final test count: 220/220 passing  
**Confidence:** HIGH (directly validated by test suite)

---

## Pattern 4: State Machine Validation (Minimal Path Testing)

### Definition

Finite State Machines (FSMs) have limited validation needs:

```
FSM with 11 states × infinity possible transitions
BUT: Only 11 valid paths needed (one per state)

Pattern: Test one path to each state, verify reachability
```

### Key Insight

**FSM testing doesn't require combinatorial explosion:**
- Dictionary-based transitions = deterministic
- If path A→B→C works, all paths work
- Need ≥1 valid path to each state, not all paths

### Implementation Pattern

**Test Design:**
```csharp
[Fact]
public void StateMachine_All_11_States_Reachable() {
    var sm = new OfficeAutomatorStateMachine();
    var allStates = new[] {
        "INIT", "DOWNLOAD_READY", "VALIDATE_READY",
        "INSTALL_READY", /* ... 7 more states */
    };
    
    // Instead of: 11 × 10 = 110 possible tests
    // Do this: 11 tests (one per state)
    foreach (var state in allStates) {
        var path = sm.GetPathToState(state);  // Get minimal path
        
        // Verify path is valid
        var current = sm.CurrentState;
        foreach (var nextState in path) {
            Assert.True(sm.TransitionTo(nextState));
            Assert.Equal(nextState, sm.CurrentState);
        }
        
        sm.Reset();  // For next iteration
    }
}

// Test matrix:
// 11 states × 1 path per state = 11 transitions tested
// SUFFICIENT because transition rules are deterministic
```

**Prevention Checklist:**
```
FSM Test Coverage:
  [ ] All states reachable (test coverage = num states)
  [ ] At least one valid path per state
  [ ] Transition dictionary verified
  [ ] Invalid transitions rejected (negative tests)
  [ ] No infinite loops
  [ ] State invariants maintained
```

### Where This Pattern Lives

**Already Created:**
- ✅ `StateMachine_All_11_States_Reachable` test (11 states verified)
- ✅ `GetPathToState()` helper (generates minimal paths)
- ✅ All E2E tests with state transitions (20 tests)

**Needs Creation:**
- 📋 `.thyrox/guidelines/csharp-state-machine-patterns.instructions.md` — FSM testing
- 📋 `.claude/rules/fsm-minimal-path-testing.md` — System rule
- 📋 ADR: "FSM test coverage = num states" (architectural decision)

### Propagation Target

| Target | Action | Files |
|--------|--------|-------|
| Guidelines | Create new | `.thyrox/guidelines/csharp-state-machine-patterns.instructions.md` |
| Rules | Create new | `.claude/rules/fsm-minimal-path-testing.md` |
| ADR | Create new | `adr-fsm-test-coverage-strategy.md` |
| Phase 10 | Update | Add FSM testing section to EXECUTE guidelines |

### Evidence

**Classification:** INFERRED (architectural pattern, not new discovery)  
**Observable:**
- 12 state machine tests, all passing
- 11 states reachable from INIT
- All E2E workflows exercise state transitions
- No test failures related to state logic  
**Confidence:** HIGH (validated by 220/220 test pass rate)

---

## System Updates Required

### 1. New Files to Create (Guidelines)

| Path | Purpose | Priority |
|------|---------|----------|
| `.thyrox/guidelines/architecture-three-layer.instructions.md` | Three-layer architecture guidelines | HIGH |
| `.thyrox/guidelines/csharp-build-practices.instructions.md` | Build artifact management | HIGH |
| `.thyrox/guidelines/csharp-tdd-guide.instructions.md` | TDD methodology for C# | MEDIUM |
| `.thyrox/guidelines/csharp-state-machine-patterns.instructions.md` | FSM testing patterns | MEDIUM |

### 2. New Files to Create (Rules)

| Path | Purpose | Priority |
|------|---------|----------|
| `.claude/rules/three-layer-architecture.md` | Architecture constraints | HIGH |
| `.claude/rules/compilation-cache-prevention.md` | Cache avoidance pattern | HIGH |
| `.claude/rules/tdd-cycle-completeness.md` | TDD discipline rules | MEDIUM |
| `.claude/rules/fsm-minimal-path-testing.md` | FSM test coverage rules | MEDIUM |

### 3. Files to Update (ADRs)

| Path | Change | Priority |
|------|--------|----------|
| (already exists) `.thyrox/context/decisions/adr-scripts-directory-structure.md` | Complete (ready) | DONE |
| (create new) `adr-fsm-test-coverage-strategy.md` | New architectural decision | MEDIUM |

### 4. Files to Update (Skills)

| Path | Change | Priority |
|------|--------|----------|
| `.claude/skills/workflow-execute/SKILL.md` | Add TDD + FSM sections | MEDIUM |

### 5. Files to Update (Project Docs)

| Path | Change | Priority |
|------|--------|----------|
| `README.md` | Add Layer 0/1/2 sections, troubleshooting | HIGH |
| `Makefile` | Already updated ✅ | DONE |

### 6. Files to Update (CI/CD Templates)

| Path | Change | Priority |
|------|---------|----------|
| `.github/workflows/test.yml` (if exists) | Add `dotnet clean` step | HIGH |

---

## Summary: Patterns Adopted

| Pattern | Status | Confidence | Next Step |
|---------|--------|-----------|-----------|
| Three-Layer Architecture | READY | HIGH | Propagate to guidelines, update README |
| Compilation Cache Prevention | READY | CRITICAL | Propagate to rules, CI/CD, README |
| TDD Cycle Completeness | READY | HIGH | Propagate to guidelines, Phase 10 |
| State Machine Validation | READY | HIGH | Propagate to guidelines, create ADR |

---

## Recommended Closing Sequence

1. **Human approval:** Review this patterns document
2. **Create guidelines:** 4 new files in `.thyrox/guidelines/`
3. **Create rules:** 4 new files in `.claude/rules/`
4. **Update ADRs:** Create `adr-fsm-test-coverage-strategy.md`
5. **Update README:** Add Layer 0/1/2, troubleshooting sections
6. **Commit:** `chore(standardize): Phase 12 STANDARDIZE — 4 patterns propagated to system`
7. **Close WP:** Execute `close-wp.sh` and state update scripts

---

**Phase 12 Status:** PATTERNS DOCUMENTED, AWAITING APPROVAL FOR PROPAGATION
