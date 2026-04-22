```yml
created_at: 2026-04-22 10:45:00
project: OfficeAutomator
work_package: 2026-04-22-06-37-03-resolve-csharp-compilation-errors
phase: Phase 11 — TRACK/EVALUATE
author: Claude
status: Aprobado
```

# Architectural Decisions — Phase 11 TRACK/EVALUATE

---

## AD-001: Scripts Directory Structure

**Decision:** Maintain separate structure for Bash (root level) and PowerShell (scripts/ subdirectory)

### Summary

During Phase 11 evaluation, architectural clarity question emerged about script organization:

**Question:** Should `setup.sh` and `verify-environment.sh` be colocated with PowerShell automation scripts in `/scripts/`?

**Decision:** NO — maintain separation at root level vs. subdirectory.

### Rationale (Three-Layer Architecture)

The OfficeAutomator architecture operates in three execution layers with distinct prerequisites:

```
Layer 0: SYSTEM SETUP (Bash at root)
├── setup.sh                    ← Installs .NET SDK, system prerequisites
├── verify-environment.sh       ← Pre-flight validation
├── Purpose: Bootstrap system dependencies
├── Privilege: Often requires sudo / apt-get
└── Prerequisites: bash, curl, tar

     ↓ (Layer 0 must succeed before Layer 1)

Layer 1: AUTOMATION (PowerShell in /scripts/)
├── scripts/Install-OfficeAutomator.ps1
├── scripts/Select-*.ps1
├── scripts/Validate-*.ps1
├── Purpose: Execute OfficeAutomator workflow (UC-001 through UC-005)
├── Privilege: Unprivileged user-level
└── Prerequisites: .NET SDK (installed by Layer 0), PowerShell

     ↓ (PowerShell loads assembly in Layer 2)

Layer 2: CORE LOGIC (C# DLL)
├── src/OfficeAutomator.Core/*.cs
├── Purpose: Business logic, validation, state machine
├── Privilege: Runs within PowerShell process context
└── Prerequisites: .NET 8.0+
```

### Organization Consequence

**Root-level scripts signal:** "Run these FIRST, before anything else"  
**Subdirectory scripts signal:** "Run these AFTER Layer 0 succeeds"

This signaling is critical for:
- **User onboarding**: New users clone → see README → run `./setup.sh` → success
- **CI/CD integration**: Build systems reference root-level scripts for bootstrap
- **Documentation**: GitHub shows root-level files prominently; subdirectories require navigation

### Correct Structure ✓

```
OfficeAutomator/
├── setup.sh                          ← Layer 0
├── verify-environment.sh             ← Layer 0
├── scripts/                          ← Layer 1 (PowerShell)
│   ├── Install-OfficeAutomator.ps1
│   ├── Select-OfficeVersion.ps1
│   └── ...
├── src/                              ← Layer 2 (C#)
│   └── OfficeAutomator.Core/
└── tests/
    └── OfficeAutomator.Core.Tests/
```

### Related Documents

- **ADR:** `adr-scripts-directory-structure.md` (formal architecture decision record)
- **Design doc:** `REGLAS_DESARROLLO_OFFICEAUTOMATOR.md` Section "ESTRUCTURA DEL PROYECTO"
- **State Machine:** `OfficeAutomatorStateMachine.cs` (enforces Layer 1 workflow sequence)

---

## AD-002: Test Coverage Standard

**Decision:** Target 220/220 tests passing (100% coverage for implemented features)

### Current Status (Phase 11)

| Category | Count | Status |
|----------|-------|--------|
| State Machine Tests | 12 | ✅ PASSING |
| ConfigGenerator Tests | 8 | ✅ PASSING (after XML declaration fix) |
| E2E Tests | 3 | ⚠ 2 passing, 1 under investigation |
| Core Validation Tests | 197 | ⚠ TBD (3 failing, investigating implementation) |
| **TOTAL** | **220** | **217 PASSING, 3 FAILING** |

### Pending Tasks

**T-001:** Identify which 3 tests are failing  
**T-002:** Analyze root causes in core implementation  
**T-003:** Fix implementation bugs  
**T-004:** Verify all 220/220 tests pass  
**T-005:** Commit with clear message  

### Criteria for Phase 11 Completion

```
GATE CRITERIA:
✅ All C# compilation errors resolved (0 CS0246, CS0029, CS1503)
✅ All using statements added to test files
✅ ConfigGenerator XML declaration prepended
✅ StateMachine transitions validated (all 11 states reachable)
✅ E2E error recovery path fixed (state sequence correct)
⏳ All 220 tests passing (217/220 current)
⏳ Implementation bugs in core classes fixed
```

---

## AD-003: TDD Methodology Applied

**Decision:** Document complete Red-Green-Refactor cycles for each feature

### Methodology

Following Test-Driven Development:
1. **RED Phase:** Identify failing test + analyze root cause
2. **GREEN Phase:** Implement minimal fix to pass test
3. **REFACTOR Phase:** Improve code quality, documentation

### Cycles Completed

| Cycle | Feature | Tests | Status |
|-------|---------|-------|--------|
| 1 | ConfigGenerator XML Declaration | 8 | ✅ RED-GREEN-REFACTOR |
| 2 | StateMachine 11-State Coverage | 12 | ✅ RED-GREEN-REFACTOR |
| 3 | E2E Error Recovery Path | 3 | ⏳ RED-GREEN (refactor pending) |

### Documentation

- **File:** `resolve-csharp-compilation-errors-tdd-analysis.md` (1,275 lines)
- **Content:** Complete Red-Green-Refactor breakdown for each cycle
- **Architecture:** Detailed three-layer integration analysis

---

## AD-004: PowerShell-C# Integration Pattern

**Decision:** Use reflection-based assembly loading with robust error handling

### Pattern

```csharp
// In PowerShell:
$dll = Join-Path $scriptRoot "src\OfficeAutomator.Core\bin\Debug\OfficeAutomator.Core.dll"
[System.Reflection.Assembly]::LoadFrom($dll)

// Now PowerShell can access C# classes:
$config = New-Object OfficeAutomator.Core.Models.OfficeConfiguration
```

### Rationale

- **No external dependencies:** Works with vanilla PowerShell + .NET SDK
- **Discoverable:** Users see DLL path in error messages if loading fails
- **Debuggable:** Stack traces show exact class/method that failed
- **Testable:** PowerShell tests can mock C# objects

### Validation

This pattern is validated by:
- ✅ E2E tests that load DLL and create C# objects
- ✅ State machine tests that verify transitions
- ✅ ConfigGenerator tests that validate XML output

---

## Summary of Decisions

| Decision | Status | Impact |
|----------|--------|--------|
| **AD-001: Scripts directory separation** | ✅ APPROVED | Clarity on layer separation |
| **AD-002: 220/220 test coverage target** | ⏳ IN PROGRESS | 217/220 passing (99.3%) |
| **AD-003: TDD methodology documentation** | ✅ APPROVED | Complete cycle analysis documented |
| **AD-004: PowerShell-C# integration** | ✅ APPROVED | Pattern validated by tests |

---

**Phase 11 Status:** Pending completion of T-001 through T-005 (3 failing tests investigation)

**Next Step:** Investigate and fix remaining 3 tests → 220/220 passing → Phase 11 complete → WP ready for closure

---

**Documented by:** Claude (THYROX Phase 11)  
**Date:** 2026-04-22 10:45:00  
**Reference:** WP 2026-04-22-06-37-03-resolve-csharp-compilation-errors  
