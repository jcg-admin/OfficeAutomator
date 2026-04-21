```yml
created_at: 2026-05-16 18:30
updated_at: 2026-05-16 18:30
document_type: Architecture - Decision Records
document_version: 1.0.0
version_notes: 5 ADRs documenting major design decisions from Stage 7
stage: Stage 7 - DESIGN/SPECIFY
work_package: 2026-04-21-06-15-00-design-specification-correct
phase: 3-ADRs-and-PM
sprint_number: 3
task_id: T-030
task_name: Architecture Decision Records
execution_date: 2026-05-16 18:30 onwards
duration_hours: TBD
story_points: 8
roles_involved: ARCHITECT (Claude)
dependencies: T-019 (State Machine), T-020 (Error Handler), T-021 (Error Codes), T-026-T-029 (All UC designs)
design_artifacts:
  - ADR-001: State Machine Architecture (11 states)
  - ADR-002: Error Handling Strategy (19 codes, retry policies)
  - ADR-003: Configuration Object Design (9 properties, write-once)
  - ADR-004: Atomic Rollback Design (3-part guarantee)
  - ADR-005: UC Integration Approach (UC boundaries, data flow)
  - Each ADR: Status, Context, Decision, Rationale, Consequences, Alternatives
acceptance_criteria:
  - All 5 ADRs documented with full context
  - Each ADR explains why decision was made
  - Alternatives considered documented
  - Consequences understood and documented
  - Decisions traceable to T-019 through T-029
  - No ambiguity about design rationale
status: IN PROGRESS
```

# ARCHITECTURE DECISION RECORDS (ADRs)

## Overview

This document contains 5 Architecture Decision Records documenting major design decisions made during Stage 7 (DESIGN/SPECIFY). Each ADR explains the context, decision made, rationale, consequences, and alternatives considered.

**Version:** 1.0.0  
**Purpose:** Provide clear rationale for Stage 7 design decisions  
**Audience:** Stage 10 developers, architects, stakeholders  
**Source:** Collective design decisions from T-019 through T-029

---

## ADR Format

Each ADR follows this structure:

```
STATUS: [Accepted | Proposed | Rejected | Deprecated]
DATE: YYYY-MM-DD
CONTEXT: Why was this decision needed?
DECISION: What did we decide?
RATIONALE: Why did we decide this way?
CONSEQUENCES: What are the implications?
ALTERNATIVES: What else did we consider?
REFERENCES: Links to supporting documents
```

---

## ADR-001: State Machine Architecture (11 States vs. Fewer)

```
STATUS: Accepted
DATE: 2026-05-16
REFERENCE: T-019 (State Management Design), T-026 (Integration & E2E)
```

### Context

OfficeAutomator needs to manage user workflow from initialization through Office installation completion or failure. The workflow must:
- Handle 5 user selections (version, language, apps, confirmation, authorization)
- Validate all selections before installation
- Execute installation and monitor progress
- Recover from failures gracefully (rollback)
- Support user cancellation at any point

Initial question: How many states does the workflow need?

Options considered:
- 3 states: INIT, RUNNING, COMPLETE (too vague)
- 6 states: INIT, SELECTING, VALIDATING, INSTALLING, COMPLETE, FAILED (too coarse)
- 11 states: Separate state per phase (chosen)

### Decision

**Implement 11 distinct states, one per workflow phase:**

```
INIT
SELECT_VERSION
SELECT_LANGUAGE
SELECT_APPS
GENERATE_CONFIG
VALIDATE
INSTALL_READY
INSTALLING
INSTALL_COMPLETE
INSTALL_FAILED
ROLLED_BACK
```

### Rationale

1. **Clarity:** Each state represents one distinct phase. No ambiguity about where user is in workflow.

2. **User Experience:** Display state-specific UI and messages. User knows exactly what's happening.

3. **Error Recovery:** Each state has clear entry/exit conditions and error handling. Makes error routing straightforward.

4. **Testability:** Each state transition can be tested independently. 11 states → ~30 transitions → testable.

5. **Maintainability:** Future stages (UC-006, UC-007) can add states without changing existing logic.

6. **Validation Timing:** Clear separation between selection phase and validation phase improves user experience (user makes all selections, then sees validation result).

### Consequences

**Positive:**
- Crystal-clear state machine (developers understand immediately)
- Easy to visualize (state diagram is straightforward)
- Easy to test (each state has clear responsibilities)
- Extensible (adding new states is easy)

**Negative:**
- More state transitions to code (vs. 3-state alternative)
- More test cases (11 states × transitions)
- Slight overhead in state management code

**Mitigation:** Consistent state machine framework (implemented in OfficeAutomatorStateMachine) reduces overhead.

### Alternatives Considered

**Alternative 1: 3-State Model (INIT, RUNNING, COMPLETE)**
- Pros: Simple, minimal transitions
- Cons: Ambiguous user state, hard to display progress, unclear error routing
- Rejected: Too vague for user-facing application

**Alternative 2: 6-State Model (INIT, SELECTING, VALIDATING, INSTALLING, COMPLETE, FAILED)**
- Pros: Simpler than 11, still reasonably clear
- Cons: Lumps multiple selections into one state, no clear separation of user actions
- Rejected: Loses granularity needed for clear UX

**Chosen: 11-State Model**
- Pros: Perfect granularity, clear UX, extensible
- Cons: More code, more tests (acceptable tradeoff)

---

## ADR-002: Error Handling Strategy (19 Codes vs. Generic Errors)

```
STATUS: Accepted
DATE: 2026-05-16
REFERENCE: T-020 (Error Propagation), T-021 (Error Codes), T-027 (Error Scenarios)
```

### Context

OfficeAutomator encounters many different errors during installation:
- Configuration errors (invalid version, language)
- Security errors (hash mismatch, certificate invalid)
- System errors (disk full, admin rights)
- Network errors (download failed, timeout)
- Installation errors (setup.exe failed)
- Rollback errors (can't delete files)

Initial question: How many error codes should we define?

Options considered:
- 1 generic code: OFF-ERROR (too vague)
- 5 codes: One per category (too coarse)
- 19 codes: Specific code per error type (chosen)

### Decision

**Implement 19 specific error codes organized in 6 categories:**

```
CONFIG (4):     OFF-CONFIG-001, OFF-CONFIG-002, OFF-CONFIG-003, OFF-CONFIG-004
SECURITY (3):   OFF-SECURITY-101, OFF-SECURITY-102
SYSTEM (4):     OFF-SYSTEM-201, OFF-SYSTEM-202, OFF-SYSTEM-203, OFF-SYSTEM-999
NETWORK (3):    OFF-NETWORK-301, OFF-NETWORK-302
INSTALL (3):    OFF-INSTALL-401, OFF-INSTALL-402, OFF-INSTALL-403
ROLLBACK (3):   OFF-ROLLBACK-501, OFF-ROLLBACK-502, OFF-ROLLBACK-503
```

Additionally: **Implement 3 retry policies (transient/system/permanent)**

```
Transient (3x retry): Temporary conditions that may resolve
System (1x retry):    Resource locks, timeouts that need single retry
Permanent (0x retry): Failures that won't resolve with retry
```

### Rationale

1. **User Communication:** Specific error codes → specific user messages. User understands what went wrong and what to do.

2. **IT Support:** Specific codes → IT support runbooks. Help desk can troubleshoot based on error code.

3. **Logging & Monitoring:** Specific codes enable tracking of error patterns. Can identify if certain errors are common.

4. **Retry Intelligence:** Retry policy based on error category. Transient errors get 3 attempts, permanent get 0.

5. **Escalation Path:** Clear escalation criteria. OFF-SECURITY-102 goes to IT Security, OFF-ROLLBACK-* goes to IT.

### Consequences

**Positive:**
- Clear error communication (both user and support)
- Smart retry policy (transient vs. permanent)
- Better logging and monitoring
- Clear escalation paths
- Easy to extend (add new codes)

**Negative:**
- More code to maintain (19 codes × error handling)
- More test cases (one per code)
- More documentation (IT runbooks for each)

**Mitigation:** ErrorHandler class centralizes retry logic. Each code defined once in ErrorHandler.

### Alternatives Considered

**Alternative 1: Single Generic Error Code**
- Pros: Minimal code
- Cons: User has no idea what went wrong, IT can't help, no retry policy
- Rejected: Unacceptable for user-facing application

**Alternative 2: 5 Error Codes (One per Category)**
- Pros: Simple, still some categorization
- Cons: Not specific enough for user messaging or IT support
- Rejected: Too vague

**Chosen: 19 Specific Codes**
- Pros: Perfect specificity, enables smart retry, clear communication
- Cons: More code/docs (acceptable, necessary)

---

## ADR-003: Configuration Object Design (9 Properties, Write-Once Principle)

```
STATUS: Accepted
DATE: 2026-05-16
REFERENCE: T-019 (Configuration class), T-028 (Config Lifecycle)
```

### Context

OfficeAutomator needs to track user selections and system state throughout workflow:
- What version did user select? (UC-001)
- What languages? (UC-002)
- What apps to exclude? (UC-003)
- Did validation pass? (UC-004)
- Where is Office installed? (UC-005)
- What state are we in? (All UCs)

Initial question: How should data flow between UCs?

Options considered:
- Method parameters (version → UC-001, then pass to UC-002, etc.) — tedious
- Global variables — causes conflicts
- Configuration object — single source of truth
- Database — overkill

### Decision

**Implement Configuration class with 9 properties (write-once per UC):**

```csharp
public class Configuration {
    public string version;              // UC-001 writes
    public string[] languages;          // UC-002 writes
    public string[] excludedApps;       // UC-003 writes
    public string configPath;           // UC-004 writes
    public bool validationPassed;       // UC-004 writes
    public string odtPath;              // UC-005 writes
    public string state;                // State machine writes
    public ErrorResult errorResult;     // ErrorHandler writes
    public DateTime timestamp;          // Each UC updates
}
```

**Additionally: Enforce write-once principle (each property owned by one UC):**

```
Property Ownership:
  version → UC-001 only
  languages → UC-002 only
  excludedApps → UC-003 only
  configPath, validationPassed → UC-004 only
  odtPath → UC-005 only
```

### Rationale

1. **Single Source of Truth:** All data in one object. No synchronization issues.

2. **Property Ownership:** Each UC owns specific properties. No conflicts, clear responsibility.

3. **Data Continuity:** Each UC reads previous UCs' outputs. Clean data flow.

4. **State Tracking:** Configuration object evolves through workflow. Easy to log, debug, trace.

5. **Error Context:** ErrorResult in Configuration provides context for errors.

### Consequences

**Positive:**
- Single source of truth (no data synchronization issues)
- Clear property ownership (no conflicts)
- Easy to trace data flow (UC-001 → UC-002 → ... → UC-005)
- Easy to log configuration at each step
- Configuration object evolves naturally

**Negative:**
- Global-like object (Configuration instance shared across all UCs)
- Must enforce write-once principle in code review

**Mitigation:** Clear architecture rules (documented in this ADR). Code review checks property ownership.

### Alternatives Considered

**Alternative 1: Method Parameters**
- Pros: Explicit data flow
- Cons: Tedious (version, languages → UC-003 params), hard to extend
- Rejected: Doesn't scale

**Alternative 2: Global Variables**
- Pros: Easy access
- Cons: Causes conflicts, hard to test, no ownership
- Rejected: Poor design

**Alternative 3: Database**
- Pros: Persistent, sophisticated
- Cons: Overkill for simple workflow, adds complexity
- Rejected: Over-engineered

**Chosen: Configuration Object (Write-Once)**
- Pros: Single source of truth, clear ownership, natural data flow
- Cons: Must enforce write-once (mitigated by code review)

---

## ADR-004: Atomic Rollback Design (3-Part Guarantee vs. Best-Effort)

```
STATUS: Accepted
DATE: 2026-05-16
REFERENCE: T-025 (UC-005), T-026 (State Machine)
```

### Context

When Office installation fails (OFF-INSTALL-401), system must clean up:
- Remove Office files from Program Files
- Remove Office registry entries
- Remove Office shortcuts

Initial question: What if cleanup fails partway?

Options considered:
- Best-effort: Delete what we can, move on (risky, leaves system inconsistent)
- Atomic: All-or-nothing (consistent, but complex)

### Decision

**Implement atomic 3-part rollback with all-or-nothing semantics:**

```
Part 1: Remove Office files (Program Files + AppData)
Part 2: Clean Office registry (HKLM + HKCU)
Part 3: Remove Office shortcuts (Start Menu + Desktop)

Success: All 3 parts succeed → State = ROLLED_BACK (can retry)
Failure: Any part fails → State = INSTALL_FAILED (CRITICAL, contact IT)
```

### Rationale

1. **System Consistency:** Either fully rolled back or not rolled back. No in-between state.

2. **User Safety:** User can trust that either installation succeeded or system is clean. No partial remnants.

3. **Retry Capability:** After successful rollback, user can retry installation. System is clean.

4. **IT Escalation:** If rollback fails, clear escalation (OFF-ROLLBACK-501/502/503). IT knows system is stuck.

5. **Compliance:** System audits expect clean removal. Partial removal is worse than complete failure.

### Consequences

**Positive:**
- System always consistent (success or rollback, never in-between)
- User can trust rollback result
- Clear IT escalation path
- Audit-friendly (clean removal or clear failure)

**Negative:**
- Complex implementation (3 parts, all-or-nothing)
- If rollback fails, user is stuck (needs IT)
- More testing needed (rollback scenarios)

**Mitigation:** Careful implementation (atomic operations, error handling). Comprehensive testing (all 3 parts, partial failures).

### Alternatives Considered

**Alternative 1: Best-Effort Cleanup**
- Pros: Simple implementation
- Cons: System may be left inconsistent (files removed, registry remains)
- Rejected: Unacceptable risk

**Alternative 2: Partial Rollback (Stop on First Failure)**
- Pros: Simple logic (fail fast)
- Cons: System partially cleaned (worse than no cleanup)
- Rejected: Worse than best-effort

**Chosen: Atomic All-or-Nothing**
- Pros: System always consistent, user can trust result
- Cons: Complex, may require IT intervention (acceptable tradeoff)

---

## ADR-005: UC Integration Approach (Boundaries & Data Flow vs. Monolithic)

```
STATUS: Accepted
DATE: 2026-05-16
REFERENCE: T-026 (Integration), T-028 (Config Lifecycle)
```

### Context

5 Use Cases need to work together:
- UC-001: Select version
- UC-002: Select language
- UC-003: Select apps
- UC-004: Validate selections
- UC-005: Install Office

Initial question: How should UCs interact?

Options considered:
- Monolithic: One giant class doing everything (UC-001 through UC-005)
- Separate classes with unclear boundaries (spaghetti)
- Clear UC boundaries with defined data flow (chosen)

### Decision

**Implement 5 separate UC classes with clear boundaries and defined data flow:**

```
UC-001 (VersionSelector) → outputs: $Config.version
  ↓
UC-002 (LanguageSelector) → outputs: $Config.languages
  ↓
UC-003 (AppExclusionSelector) → outputs: $Config.excludedApps
  ↓
UC-004 (ConfigValidator + ConfigGenerator) → outputs: $Config.configPath, validationPassed
  ↓
UC-005 (InstallationExecutor + RollbackExecutor) → outputs: $Config.odtPath, final state
```

**Each UC boundary has clear contract:**
- UC-001: Takes nothing, outputs version
- UC-002: Takes version (from UC-001), outputs languages
- UC-003: Takes version + languages (from UC-001/002), outputs excludedApps
- UC-004: Takes version + languages + excludedApps (from 001-003), outputs validationPassed + configPath
- UC-005: Takes validationPassed + configPath (from UC-004), outputs odtPath + final state

### Rationale

1. **Single Responsibility:** Each UC has one clear responsibility. Easy to understand, test, maintain.

2. **Data Flow Clarity:** Each UC's inputs/outputs defined. No hidden dependencies.

3. **Testability:** Each UC tested independently (mock inputs) and together (integration tests).

4. **Extensibility:** New UCs (UC-006, UC-007) can follow same pattern. Clear integration point.

5. **Error Isolation:** Error in UC-003 doesn't affect UC-001/002. Clear error boundaries.

### Consequences

**Positive:**
- Clear UC boundaries (no overlap)
- Clear data flow (each UC output → next UC input)
- Easy to test each UC independently
- Easy to extend (add UC-006)
- Error isolated per UC

**Negative:**
- More classes to manage (5 UC classes + 3 helpers = 8 classes total)
- Must define clear UC boundaries (requires design discipline)
- Integration testing needed (test UC interactions)

**Mitigation:** Clear documentation of UC boundaries. Integration tests verify data flow.

### Alternatives Considered

**Alternative 1: Monolithic Design**
- Pros: Single class, simple at first
- Cons: Hard to understand, hard to test, hard to extend
- Rejected: Not scalable

**Alternative 2: Unclear Boundaries**
- Pros: Flexible
- Cons: Spaghetti code, unclear dependencies, hard to maintain
- Rejected: Poor design

**Chosen: Clear UC Boundaries + Data Flow**
- Pros: Clear, testable, extensible
- Cons: More classes (acceptable, necessary)

---

## ADR Summary

```
ADR-001: 11 States vs. Fewer
  Decision: 11 distinct states (one per workflow phase)
  Why: Clarity, user experience, testability, extensibility
  
ADR-002: 19 Error Codes vs. Generic
  Decision: 19 specific codes + 3 retry policies
  Why: Clear communication, IT support, smart retry, monitoring
  
ADR-003: Configuration Object (Write-Once) vs. Parameters/Globals
  Decision: Single Configuration object, property ownership per UC
  Why: Single source of truth, clear data flow, easy to trace
  
ADR-004: Atomic Rollback (All-or-Nothing) vs. Best-Effort
  Decision: 3-part atomic rollback, consistent state
  Why: System consistency, user trust, clear escalation
  
ADR-005: UC Boundaries + Data Flow vs. Monolithic
  Decision: 5 separate UCs with clear boundaries
  Why: Single responsibility, clear dependencies, testable, extensible
```

---

## Cross-References

All ADRs are grounded in Stage 7 design documents:

```
ADR-001 → T-019 (State Machine), T-026 (State Machine Integration)
ADR-002 → T-020 (Error Handler), T-021 (Error Codes), T-027 (Error Scenarios)
ADR-003 → T-019 (Configuration Class), T-028 (Config Lifecycle)
ADR-004 → T-025 (UC-005 Rollback), T-026 (Rollback Path)
ADR-005 → T-022 to T-025 (All UCs), T-026 (UC Integration)
```

---

## Document Metadata

```
Created: 2026-05-16 18:30
Task: T-030 Architecture Decision Records
Version: 1.0.0
Story Points: 8
Status: IN PROGRESS
ADRs Documented: 5
ADR Status: All Accepted
Dependencies: T-019 through T-029
Use: Stage 10 developer reference, architectural rationale
Quality Gate: All decisions justified and documented
```

---

**T-030 IN PROGRESS**

**5 Architecture Decision Records documented: All decisions justified and traceable to Stage 7 designs ✓**

