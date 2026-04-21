```yml
created_at: 2026-04-21 08:00:00
project: THYROX
work_package: 2026-04-21-06-15-00-design-specification-correct
type: Task Plan - Atomic Work Breakdown (WBS)
methodology: Task Planner (RUP + Agile decomposition)
author: Claude (Execution perspective)
status: READY FOR EXECUTION
version: 1.0.0-task-plan
total_tasks: 87
total_hours: 265-300
```

# TASK PLAN STAGE 7 — Descomposición Atómica (T-NNN)

---

## CONCEPTO: Roles vs Personas

```
ROLES (conjunto de actividades coherentes):
  ✓ ARCHITECT: Diseña arquitectura, UCs, seguridad (Claude)
  ✓ BA: Analiza requisitos, elicita, clarifica (Claude)
  ✓ PM: Planifica, gestiona riesgos, costos (Claude)
  ✓ PRODUCT OWNER: Aprueba, toma decisiones (Nestor)
  ✓ REVIEWER: Valida calidad, convenciones (Claude + Nestor)

PERSONAS:
  ✓ Claude: Juega ARCHITECT + BA + PM + REVIEWER
  ✓ Nestor: Juega PRODUCT OWNER + REVIEWER

FLEXIBILIDAD:
  ✓ Tareas pueden reasignarse según capacidad
  ✓ Documentación emerge como subproducto
  ✓ No hay "documentación separada", es output de tareas
```

---

## PART 1: TASK BREAKDOWN STRUCTURE (WBS)

### Nivel 1: Phases

```
PHASE 1 — WATERFALL CIMENTACIÓN (T-001 a T-020)
  └─ Subtask group: Requirements Elicitation
  └─ Subtask group: Charter & Scope
  └─ Subtask group: Security Architecture

PHASE 2 — AGILE SCRUM (T-021 a T-080)
  └─ Sprint 1 (T-021 a T-035): Architecture Foundation
  └─ Sprint 2 (T-036 a T-050): Specification & PM
  └─ Sprint 3 (T-051 a T-065): ADRs & Quality Attributes
  └─ Sprint 4 (T-066 a T-080): UC Design & Closure

PHASE 3 — WATERFALL CIERRE (T-081 a T-087)
  └─ Subtask group: Compliance Audit
  └─ Subtask group: Exit Gate & Handoff
```

---

## PART 2: DETAILED TASK BREAKDOWN

### **PHASE 1 — WATERFALL CIMENTACIÓN (10 days, 70-80 hours)**

#### **Week 1: Elicitation & Clarification (Days 1-5)**

```
T-001: PROJECT KICKOFF & PLANNING
├─ Role: ARCHITECT + PRODUCT OWNER
├─ Duration: 2 hours
├─ Days: 1 (Monday 09:00-11:00)
├─ Effort: 2h (Claude), 1h (Nestor)
├─ Deliverable: Kickoff meeting notes, plan confirmation
├─ Dependencies: None (START TASK)
├─ Acceptance Criteria:
│  ✓ PLAN HYBRID approved by Nestor
│  ✓ Timeline 7 weeks confirmed
│  ✓ Ceremonies understood (daily standup, reviews, retro)
│  ✓ Escalation path clarified
└─ Status: READY

T-002: rm-requirements-baseline.md DRAFT
├─ Role: BA (ARCHITECT)
├─ Duration: 5 hours
├─ Days: 1-2 (Mon 10:00-12:00 + 14:00-16:00, Tue 09:15-12:00)
├─ Effort: 5h Claude
├─ Deliverable: rm-requirements-baseline.md (~40 lines, v1.0 vs v1.1)
├─ Dependencies: T-001 (kickoff complete)
├─ Acceptance Criteria:
│  ✓ List requisitos v1.0.0 frozen
│  ✓ List requisitos v1.1 roadmap (separate)
│  ✓ Clear separation (IN vs OUT)
│  ✓ Ready for review
│  ✓ Metadata yml correct
└─ Output: Draft baseline (~2h work + 3h review)

T-003: DAILY STANDUP - Day 1
├─ Role: ARCHITECT + PRODUCT OWNER
├─ Duration: 10 minutes
├─ Day: 1 (Mon 09:00-09:15)
├─ Effort: 0.17h (trivial)
├─ Deliverable: Standup notes, blockers identified
├─ Pattern: Repeat as T-003, T-004, ... T-020 (daily through Phase 1)
└─ Status: RECURRING

T-004: rm-stakeholder-requirements.md DRAFT
├─ Role: BA (ARCHITECT)
├─ Duration: 6 hours
├─ Days: 2-3 (Tue 09:15-12:00 + 14:00-16:00, Wed 09:15-11:00)
├─ Effort: 6h Claude
├─ Deliverable: rm-stakeholder-requirements.md (~50 lines)
├─ Content:
│  ✓ IT Admin requirements
│  ✓ End User requirements
│  ✓ Support requirements
│  ✓ Compliance requirements
│  ✓ Research + elicitation
├─ Dependencies: T-002 (baseline ready)
└─ Acceptance Criteria:
   ✓ All 4 stakeholders documented
   ✓ Clear needs per role
   ✓ Ready for clarification

T-005: rm-requirements-clarification.md DRAFT
├─ Role: BA (ARCHITECT)
├─ Duration: 7 hours
├─ Days: 3-4 (Wed 09:15-12:00 + 14:00-16:00, Thu 09:15-12:00)
├─ Effort: 7h Claude
├─ Deliverable: rm-requirements-clarification.md (~80 lines)
├─ Content (8 ambigüities resolved):
│  1. UC-004 8 pasos exactamente
│  2. Fail-Fast vs Repair Mode
│  3. Idempotence scope (UC-005 only or entire pipeline?)
│  4. Microsoft OCT bug mitigation HOW
│  5. Error handling between UCs
│  6. Rollback strategy
│  7. State management (mutable vs immutable)
│  8. Logging levels per UC
├─ Dependencies: T-004 (stakeholder requirements)
└─ Acceptance Criteria:
   ✓ 8 ambigüities documented + resolved
   ✓ No contradictions
   ✓ Clear for UC design

T-006: design/data-structures-and-matrices.md DRAFT
├─ Role: ARCHITECT
├─ Duration: 6 hours
├─ Days: 3-4 (Wed 14:00-16:00, Thu 14:00-16:00 + Fri 09:15-11:00)
├─ Effort: 6h Claude
├─ Deliverable: design/data-structures-and-matrices.md (~60 lines)
├─ Content:
│  ✓ Whitelist de versiones (2024, 2021, 2019)
│  ✓ XSD XML schema
│  ✓ Language-version compatibility matrix
│  ✓ App-version compatibility matrix
│  ✓ Microsoft hash official source
├─ Dependencies: T-005 (clarification for UC-004 data needs)
└─ Acceptance Criteria:
   ✓ All data structures defined
   ✓ Examples present
   ✓ Ready for UC implementation

T-007: DOCUMENTATION CONSOLIDATION (T-002 through T-006)
├─ Role: ARCHITECT + REVIEWER
├─ Duration: 3 hours
├─ Day: 5 (Fri 09:00-12:00)
├─ Effort: 3h Claude
├─ Deliverable: 4 documents polished + ready for review
├─ Quality Check:
│  ✓ Metadata yml correct
│  ✓ No typos
│  ✓ Formatting consistent
│  ✓ 9 conventions applied
│  ✓ No emojis
├─ Dependencies: T-006 (all elicitation complete)
└─ Acceptance Criteria:
   ✓ All 4 docs ready for approval
   ✓ Quality 100%
   ✓ Ready for CP1 gate

T-008: CHECKPOINT 1 APPROVAL MEETING
├─ Role: ARCHITECT + PRODUCT OWNER
├─ Duration: 1 hour
├─ Day: 5 (Fri 10:00-11:00)
├─ Effort: 1h Claude, 1h Nestor
├─ Deliverable: CP1 gate approval (APPROVED / CHANGES / REJECTED)
├─ Presentation:
│  ✓ rm-requirements-baseline: "v1.0.0 items frozen"
│  ✓ rm-stakeholder-requirements: "stakeholder needs documented"
│  ✓ rm-requirements-clarification: "8 ambigüities resolved"
│  ✓ design/data-structures: "data definitions clear"
├─ Outcome: GATE DECISION
├─ Dependencies: T-007 (docs ready)
└─ If APPROVED:
   → Mark BASELINE as FROZEN (no más cambios)
   → Proceed to Week 2 (T-009 onwards)
   → If REJECTED: Re-iterate Week 1

T-009: WEEK 1 SUMMARY & RETROSPECTIVE (Internal)
├─ Role: ARCHITECT
├─ Duration: 1 hour
├─ Day: 5 (Fri 11:00-12:00, if CP1 passed)
├─ Effort: 1h Claude
├─ Deliverable: Week 1 summary notes
├─ Content:
│  ✓ Velocity: 4 docs in 5 days
│  ✓ Effort: ~35 hours
│  ✓ Quality issues: None
│  ✓ Lessons learned: (for Phase 2 planning)
└─ Status: INFORMATIONAL

WEEK 1 TOTALS:
  Documents: 4 (baseline, stakeholders, clarification, data-structures)
  Hours: ~35-40 (Claude)
  Quality: DoD 100%
  Gate: CP1 APPROVAL required
  Status: IF APPROVED → Proceed to Week 2
```

#### **Week 2: Charter, Scope, Security (Days 6-10)**

```
T-010: pm-charter.md DRAFT
├─ Role: PM (ARCHITECT)
├─ Duration: 4 hours
├─ Days: 6-7 (Mon 09:15-12:00 + 14:00-15:00, Tue 09:15-10:15)
├─ Effort: 4h Claude
├─ Deliverable: pm-charter.md (~50 lines)
├─ Content:
│  ✓ Authorization: Nestor approves Stage 7
│  ✓ Objectives: SMART goals (26 docs, 7 weeks)
│  ✓ Success criteria: "Done when..."
│  ✓ Constraints: Timeline, resources, scope
│  ✓ Escalation: Who decides what
├─ Dependencies: CP1 passed (baseline frozen)
└─ Acceptance Criteria:
   ✓ Charter complete
   ✓ Authorization clear
   ✓ Ready for scope refinement

T-011: pm-scope-statement.md DRAFT
├─ Role: PM (ARCHITECT)
├─ Duration: 4 hours
├─ Days: 7-8 (Tue 10:15-12:00 + 14:00-16:00, Wed 09:15-11:00)
├─ Effort: 4h Claude
├─ Deliverable: pm-scope-statement.md (~60 lines)
├─ Content:
│  ✓ IN SCOPE: 5 UCs design, security, PM plans
│  ✓ OUT OF SCOPE: UI mockups (Stage 9), DB (Stage 8)
│  ✓ Scope creep prevention: CCB process
│  ✓ Constraints: 7 weeks, 26 docs
├─ Dependencies: T-010 (charter foundation)
└─ Acceptance Criteria:
   ✓ Scope IN/OUT clear
   ✓ No ambigüities
   ✓ Scope creep process defined

T-012: architecture/security-architecture.md DRAFT (Part 1)
├─ Role: ARCHITECT
├─ Duration: 8 hours
├─ Days: 6-8 (Mon 09:15-12:00 + 14:00-16:00, Tue 14:00-16:00, Wed 14:00-16:00)
├─ Effort: 8h Claude
├─ Deliverable: architecture/security-architecture.md (~100+ lines, comprehensive)
├─ Content:
│  1. Threat Model (STRIDE):
│     ✓ Spoofing identity threats
│     ✓ Tampering attacks
│     ✓ Repudiation risks
│     ✓ Info disclosure
│     ✓ Denial of service
│     ✓ Elevation of privilege
│  2. Security Requirements:
│     ✓ Credential handling (user token, encrypted storage)
│     ✓ Encryption (transport, at-rest)
│     ✓ Audit logging
│     ✓ Access control (per UC)
│  3. Secure Coding Guidelines:
│     ✓ Input validation (no injection)
│     ✓ Output encoding
│     ✓ Error handling (no info leakage)
│     ✓ Session management
│  4. Risk Mitigation:
│     ✓ Per threat, mitigation strategy
│     ✓ Residual risk assessment
├─ Dependencies: T-005 (clarification, UC-004 understanding)
└─ Acceptance Criteria:
   ✓ Threat model complete (8+ threats identified)
   ✓ Mitigations documented
   ✓ Security architecture ready for UC implementation

T-013: pm-scope-statement.md REFINEMENT (with security)
├─ Role: PM (ARCHITECT)
├─ Duration: 2 hours
├─ Day: 9 (Wed 14:00-16:00, after security draft reviewed)
├─ Effort: 2h Claude
├─ Deliverable: pm-scope-statement.md updated (security requirements added)
├─ Changes:
│  ✓ Add security scope items (threat model, audit)
│  ✓ Add compliance constraints
├─ Dependencies: T-012 (security architecture)
└─ Acceptance Criteria:
   ✓ Scope updated with security
   ✓ Ready for final approval

T-014: REVIEW & POLISH (T-010, T-011, T-012, T-013)
├─ Role: ARCHITECT + REVIEWER
├─ Duration: 3 hours
├─ Day: 9 (Wed 09:15-12:00)
├─ Effort: 3h Claude
├─ Deliverable: 3 documents polished (charter, scope, security)
├─ Quality Check:
│  ✓ Metadata correct
│  ✓ Cross-references between docs
│  ✓ Consistency (scope ↔ charter ↔ security)
│  ✓ No contradictions
│  ✓ Convenciones 100%
├─ Dependencies: T-013 (all drafts complete)
└─ Acceptance Criteria:
   ✓ All 3 docs ready for approval
   ✓ Quality 100%

T-015: CHECKPOINT 1.5 APPROVAL MEETING
├─ Role: ARCHITECT + PRODUCT OWNER
├─ Duration: 1 hour
├─ Day: 10 (Fri 10:00-11:00)
├─ Effort: 1h Claude, 1h Nestor
├─ Deliverable: CP1.5 gate approval (APPROVED / CHANGES)
├─ Presentation:
│  ✓ "Phase 1 COMPLETE: 7 documents frozen"
│  ✓ "Baseline locked, charter approved, scope clear, security architected"
│  ✓ "Ready for Agile sprints starting Week 3"
├─ Outcome: GATE DECISION (likely APPROVED)
├─ Dependencies: T-014 (docs ready)
└─ If APPROVED:
   → PHASE 1 FROZEN (no más cambios)
   → Begin planning Sprint 1 for Week 3

T-016: PHASE 1 CLOSURE & SPRINT PLANNING PREP
├─ Role: ARCHITECT + PM
├─ Duration: 3 hours
├─ Day: 10 (Fri 11:00-14:00, if CP1.5 passed)
├─ Effort: 3h Claude
├─ Deliverable: PHASE-1-COMPLETE summary + Sprint 1 planning materials
├─ Content:
│  ✓ PHASE-1-COMPLETE.md (summary of 7 docs)
│  ✓ Sprint 1 backlog (user stories + acceptance criteria)
│  ✓ Story point estimates
│  ✓ Definition of Done
│  ✓ Velocity forecast
├─ Dependencies: T-015 (CP1.5 passed)
└─ Ready for Sprint 1 planning (Monday Week 3, T-021)

WEEK 2 TOTALS:
  Documents: 3 (charter, scope, security)
  Hours: ~25-30 (Claude)
  Quality: DoD 100%
  Gate: CP1.5 APPROVAL required
  Status: IF APPROVED → Begin PHASE 2 (Agile sprints)

PHASE 1 GRAND TOTAL:
  Documents: 7 (baseline, stakeholders, clarification, data-structures, charter, scope, security)
  Hours: 70-80 (Claude)
  Effort: ~2 weeks
  Checkpoints: 2 (CP1, CP1.5)
  Status: FOUNDATION FROZEN
  Next: PHASE 2 (Agile sprints, Week 3)
```

---

### **PHASE 2 — AGILE SCRUM (4 Sprints, 16 days, 160-180 hours with ceremonies)**

#### **SPRINT 1: ARCHITECTURE FOUNDATION (Week 3, Days 11-15)**

```
T-017: SPRINT 1 PLANNING
├─ Role: ARCHITECT + PRODUCT OWNER + SCRUM MASTER
├─ Duration: 1 hour
├─ Day: 11 (Mon 10:00-11:00)
├─ Effort: 1h Claude, 1h Nestor
├─ Deliverable: Sprint 1 backlog committed (20 story points)
├─ Sprint Goal: "Solidify architecture: state management, error handling, PM schedule"
├─ Stories Committed:
│  1. ARQ-001: State machine design (5 pts)
│  2. ARQ-002: Error propagation (5 pts)
│  3. ARQ-003: Security details (5 pts) — optional if time
│  4. PM-001: Realistic schedule (5 pts)
├─ Definition of Done confirmed
├─ Dependencies: T-016 (Sprint 1 planning materials ready)
└─ Output: Sprint 1 board ready

T-018: DAILY STANDUP - Sprint 1 (Days 11-15)
├─ Role: ARCHITECT + PRODUCT OWNER
├─ Duration: 15 minutes × 5 days = 1.25 hours total
├─ Days: 11-15 (Mon-Fri 09:00-09:15)
├─ Effort: 0.25h Claude per day × 5 = 1.25h total
├─ Deliverable: 5 daily standup notes
├─ Pattern: Recurring per sprint (T-018 for S1, T-033 for S2, etc)
├─ Focus:
│  - Progress on stories
│  - Blockers
│  - Plan adjustments
└─ Status: ROUTINE

T-019: design/state-management-design.md (T-019 + T-020)
├─ Role: ARCHITECT
├─ Duration: 8 hours (2 days work)
├─ Days: 11-12 (Mon 09:15-12:00 + 14:00-16:00 + Tue 09:15-12:00)
├─ Effort: 8h Claude
├─ Story Points: 5
├─ Deliverable: design/state-management-design.md (~80 lines)
├─ Content:
│  1. Valid States:
│     ✓ $Config.State = "VersionSelected" | "LanguageSelected" | "AppsSelected" | "ReadyToInstall" | "Installing" | "Completed" | "Error"
│  2. Transitions:
│     ✓ VersionSelected → LanguageSelected (if version valid)
│     ✓ LanguageSelected → AppsSelected (if language valid)
│     ✓ AppsSelected → ReadyToInstall (if validation passed)
│     ✓ ReadyToInstall → Installing (start install)
│     ✓ Installing → Completed (success) | Error (failure)
│  3. Guards (preconditions):
│     ✓ transition(A→B) requires previous transitions complete
│     ✓ No jumping states
│  4. Mermaid Diagram:
│     ✓ State machine flowchart
│     ✓ Dark theme, no emojis
│  5. Error States:
│     ✓ Any state → Error (on critical failure)
│     ✓ Error → VersionSelected (user retry)
├─ Dependencies: T-012 (security understanding)
└─ Acceptance Criteria:
   ✓ State machine formal
   ✓ Mermaid diagram clear
   ✓ Error paths documented
   ✓ Ready for UC implementation
   ✓ Definition of Done 100%

T-020: design/error-propagation-strategy.md
├─ Role: ARCHITECT
├─ Duration: 8 hours (Tue-Wed)
├─ Days: 12-13 (Tue 14:00-16:00 + Wed 09:15-12:00 + 14:00-16:00)
├─ Effort: 8h Claude
├─ Story Points: 5
├─ Deliverable: design/error-propagation-strategy.md (~70 lines)
├─ Content:
│  ✓ If UC-001 fails → Return to version selection (user retry)
│  ✓ If UC-002 fails → Return to language selection (user retry)
│  ✓ If UC-003 fails → Return to apps selection (user retry)
│  ✓ If UC-004 fails → STOP (blocker, cannot proceed to UC-005)
│  ✓ If UC-005 fails → Attempt retry (UC-004 already passed)
│  ✓ Error logging strategy (what error info to capture)
│  ✓ Error recovery (rollback, checkpoint restore)
├─ Dependencies: T-019 (state machine as context)
└─ Acceptance Criteria:
   ✓ Error scenarios documented
   ✓ UC-004 blocker clearly stated
   ✓ Recovery strategies defined
   ✓ Logging details (with T-012 security)

T-021: pm-schedule-baseline.md REALISTIC ESTIMATES
├─ Role: PM (ARCHITECT)
├─ Duration: 8 hours (Wed-Thu)
├─ Days: 13-14 (Wed 09:15-11:00, Thu 09:15-12:00 + 14:00-16:00)
├─ Effort: 8h Claude
├─ Story Points: 5
├─ Deliverable: pm-schedule-baseline.md (~80 lines)
├─ Content:
│  ✓ Realistic estimates (450 min/stage, not 46 min)
│  ✓ Cadena crítica identified:
│    - UC-Matrix → overall-architecture (precondition)
│    - overall-architecture → UCs (blocker)
│    - UC-004 critical (longest, 16→120-150 min)
│    - UC-005 waits for UC-004
│  ✓ Parallel paths:
│    - UC-001, UC-002, UC-003 can run in parallel
│    - ADRs can run parallel to UCs
│    - PM docs can run parallel
│  ✓ Hitos de milestone (sprint ends)
│  ✓ Buffer (15% contingency)
├─ Dependencies: T-019, T-020 (architecture understanding)
└─ Acceptance Criteria:
   ✓ Estimates realistic
   ✓ Critical path clear
   ✓ Parallel paths identified
   ✓ Contingency included

T-022: DESIGN REVIEW CHECKPOINT (Wed 15:00-15:15)
├─ Role: ARCHITECT + PRODUCT OWNER
├─ Duration: 15 minutes
├─ Day: 13 (Wed 15:00-15:15)
├─ Effort: 0.25h each
├─ Deliverable: Feedback on Mermaid diagram + error handling
├─ Focus:
│  ✓ Show state machine diagram to Nestor
│  ✓ Validate error scenarios
│  ✓ Feedback on schedule estimates
├─ Outcome: "Looks good" or "needs changes"
├─ Dependencies: T-019, T-020 (drafts ready)
└─ If changes needed: Quick iteration before Friday

T-023: SPRINT 1 POLISH & FINALIZATION
├─ Role: ARCHITECT + REVIEWER
├─ Duration: 4 hours
├─ Day: 14 (Thu 14:00-18:00, after standup)
├─ Effort: 4h Claude
├─ Deliverable: 3 documents polished (state management, error propagation, schedule)
├─ Quality Check:
│  ✓ Metadata yml
│  ✓ Cross-references
│  ✓ Mermaid dark theme
│  ✓ No emojis
│  ✓ Convenciones 100%
├─ Dependencies: T-022 (feedback integrated)
└─ Status: Ready for Friday review

T-024: SPRINT 1 REVIEW + DEMO
├─ Role: ARCHITECT + PRODUCT OWNER
├─ Duration: 1 hour
├─ Day: 15 (Fri 15:00-16:00)
├─ Effort: 1h Claude, 1h Nestor
├─ Deliverable: Sprint 1 approved increment
├─ Demo:
│  ✓ Show state machine with Mermaid
│  ✓ Show error scenarios
│  ✓ Show schedule + critical path
├─ Nestor Feedback: "Approved" / "changes needed"
├─ Outcome: 3 documents DONE (20 story points)
├─ Metrics: Velocity = 20 pts, on target
├─ Dependencies: T-023 (docs ready)
└─ Status: APPROVED increment

T-025: SPRINT 1 RETROSPECTIVE
├─ Role: ARCHITECT + PRODUCT OWNER (+ optional SCRUM MASTER)
├─ Duration: 30 minutes
├─ Day: 15 (Fri 16:00-16:30)
├─ Effort: 0.5h each
├─ Deliverable: Retro notes + action items
├─ Reflection:
│  ✓ What went well? (daily standups, fast feedback)
│  ✓ What to improve? (Mermaid diagrams take longer)
│  ✓ Action items: Estimate Mermaid +50% time, add Wed review
├─ Velocity confirmed: 20 pts/sprint sustainable
├─ Dependencies: T-024 (sprint complete)
└─ Output: Improved for Sprint 2

SPRINT 1 TOTALS:
  Documents: 3 (state management, error propagation, schedule)
  Story Points: 20 (committed and completed)
  Hours: ~40 (Claude work) + ~5 (ceremonies)
  Quality: DoD 100%
  Velocity: 20 pts ✓
  Status: COMPLETE & APPROVED
  Next: Sprint 2 planning (Mon Week 4)
```

#### **SPRINT 2: SPECIFICATION & PM (Week 4, Days 16-20)**

```
(Similar structure to Sprint 1, but with different stories)

T-026: SPRINT 2 PLANNING
├─ Role: ARCHITECT + PRODUCT OWNER
├─ Duration: 1 hour
├─ Day: 16 (Mon 10:00-11:00)
├─ Deliverable: Sprint 2 backlog committed (20 story points)
├─ Sprint Goal: "Specify logging, NFR, disaster recovery, formal requirements start"
├─ Stories Committed:
│  1. RM-001: Formal SRS (IEEE 830) (5 pts)
│  2. RM-002: Traceability matrix (5 pts)
│  3. BA-001: Logging specification (3 pts)
│  4. ARQ-004: Disaster recovery (5 pts)
│  5. ARQ-005: Design patterns (2 pts) — buffer if needed
├─ Dependencies: T-025 (Sprint 1 retro + velocity confirmed)
└─ Output: Sprint 2 board ready

T-027-T-035: SPRINT 2 EXECUTION (similar to T-018-T-025)
├─ T-027: Daily Standups (15 min × 5 days)
├─ T-028: rm-requirements-formal-srs.md (IEEE 830 with REQ-NNN IDs) — 8h, 5 pts
├─ T-029: rm-requirements-traceability-matrix.md (REQ→UC→Code→Test) — 8h, 5 pts
├─ T-030: design/logging-specification.md (what to log per UC) — 4h, 3 pts
├─ T-031: architecture/disaster-recovery.md (checkpoint/restore) — 8h, 5 pts
├─ T-032: DESIGN REVIEW CHECKPOINT (Wed 15:00-15:15)
├─ T-033: SPRINT 2 POLISH & FINALIZATION (4h)
├─ T-034: SPRINT 2 REVIEW + DEMO (1h)
├─ T-035: SPRINT 2 RETROSPECTIVE (30 min)
├─ Velocity: ~20 story points (confirmed)
└─ Status: SPRINT 2 COMPLETE

SPRINT 2 TOTALS:
  Documents: 5 (formal SRS, traceability, logging, disaster recovery, + patterns)
  Story Points: 20
  Hours: ~40 + ~5 (ceremonies)
  Status: COMPLETE & APPROVED
  Next: Sprint 3 planning (Mon Week 5)
```

#### **SPRINT 3: ADRs & QUALITY ATTRIBUTES (Week 5, Days 23-28)**

```
(Smaller documents, more of them)

T-036: SPRINT 3 PLANNING
├─ Role: ARCHITECT + PRODUCT OWNER
├─ Duration: 1 hour
├─ Day: 23 (Mon 10:00-11:00)
├─ Deliverable: Sprint 3 backlog committed (18 story points)
├─ Sprint Goal: "Document architecture decisions and PM/RM formal plans"
├─ Stories Committed:
│  1. adr-config-state-model (1 pt) — Why $Config object
│  2. adr-layered-architecture (1 pt) — Why 6-layer
│  3. adr-idempotence-scope (1 pt) — Scope of idempotence
│  4. architecture/quality-attributes-tradeoffs (2 pts)
│  5. architecture/extensibility-strategy (2 pts)
│  6. pm-cost-estimate (2 pts)
│  7. pm-resource-plan (2 pts)
│  8. pm-risk-register (3 pts) — Detailed risk analysis
│  9. pm-change-control-process (2 pts)
│  Total: 18 pts
├─ Dependencies: T-035 (Sprint 2 complete)
└─ Output: Sprint 3 board ready

T-037-T-045: SPRINT 3 EXECUTION
├─ T-037: Daily Standups (15 min × 5 days)
├─ T-038: adr-config-state-model.md (1h, 1 pt) — Why not DB, why object
├─ T-039: adr-layered-architecture.md (1h, 1 pt) — Why 6-layer vs 3-tier
├─ T-040: adr-idempotence-scope.md (2h, 1 pt) — Scope analysis
├─ T-041: architecture/quality-attributes-tradeoffs.md (4h, 2 pts)
├─ T-042: architecture/extensibility-strategy.md (4h, 2 pts)
├─ T-043: pm-cost-estimate.md (4h, 2 pts)
├─ T-044: pm-resource-plan + pm-risk-register (10h, 5 pts combined)
├─ T-045: pm-change-control-process.md (2h, 1 pt)
├─ T-046: DESIGN REVIEW CHECKPOINT
├─ T-047: SPRINT 3 POLISH (3h)
├─ T-048: SPRINT 3 REVIEW + DEMO (1h)
├─ T-049: SPRINT 3 RETROSPECTIVE (30 min)
├─ Velocity: ~18 story points (slight dip OK, more docs)
└─ Status: SPRINT 3 COMPLETE

SPRINT 3 TOTALS:
  Documents: 9 (5 ADRs/arch docs + 4 PM docs)
  Story Points: 18
  Hours: ~35 + ~5 (ceremonies)
  Status: COMPLETE & APPROVED
  Next: Sprint 4 planning (Mon Week 6)
```

#### **SPRINT 4: UC DESIGN & CLOSURE (Week 6, Days 30-35)**

```
(Longest sprint, critical path focus)

T-050: SPRINT 4 PLANNING
├─ Role: ARCHITECT + PRODUCT OWNER
├─ Duration: 1 hour
├─ Day: 30 (Mon 10:00-11:00)
├─ Deliverable: Sprint 4 backlog committed (46 story points — ambitious)
├─ Sprint Goal: "Design all UCs + overall architecture, finalize PM/RM docs"
├─ Stories Committed:
│  1. overall-architecture.md (8 pts) — Prerequisite for UCs
│  2. UC-001-select-version (5 pts)
│  3. UC-002-select-language (5 pts)
│  4. UC-003-exclude-applications (5 pts)
│  5. UC-004-validate-configuration (10 pts) — CRITICAL PATH
│  6. UC-005-install-office (8 pts)
│  7. pm-communications-plan (2 pts)
│  8. pm-stakeholder-register (1 pt)
│  9. rm-requirements-validation-report (2 pts)
│  Total: 46 pts
├─ Critical Path Note: UC-004 must complete before UC-005
├─ Parallel Strategy:
│  ✓ overall-architecture (starts immediately, Mon)
│  ✓ UC-001, UC-002, UC-003 (parallel, start after arch reviewed)
│  ✓ UC-004 (critical, start ASAP, longest task)
│  ✓ UC-005 (depends on UC-004, wait until UC-004 50% done)
├─ Dependencies: T-049 (Sprint 3 complete)
└─ Output: Sprint 4 board ready (critical path managed)

T-051: overall-architecture.md (CRITICAL PREREQUISITE)
├─ Role: ARCHITECT
├─ Duration: 10 hours (Mon-Tue)
├─ Days: 30-31 (Mon 09:15-12:00 + 14:00-16:00, Tue 09:15-12:00 + 14:00-16:00)
├─ Effort: 10h Claude
├─ Story Points: 8
├─ Deliverable: overall-architecture.md (~120 lines)
├─ Content:
│  1. Overview: What OfficeAutomator does (1 sentence)
│  2. Architectural pattern: 6-layer (UI → Validation → Business Logic → State → Data → Config)
│  3. Component diagram: Mermaid (layers + flow)
│  4. Data flow: Config object lifecycle
│  5. Error handling: How errors propagate
│  6. Security posture: Threat model summary
│  7. Key design decisions: Reference to ADRs
│  8. Quality attributes: Performance, reliability, security
│  9. Dependencies: External tools (ODT, PowerShell, Office COM)
│  10. Future extensibility: How to add features
├─ Dependencies: All Phase 1-2 architecture work (T-019, T-020, T-012, etc)
└─ Acceptance Criteria:
   ✓ Architecture clear and implementable
   ✓ All layers documented
   ✓ Mermaid diagram ready
   ✓ UC-001-005 can be designed from this

T-052: UC-001-select-version.md (PARALLEL after overall-arch)
├─ Role: ARCHITECT + BA
├─ Duration: 8 hours (Tue-Wed)
├─ Days: 31-32 (after T-051 reviewed on Tue, start Wed)
├─ Effort: 8h Claude
├─ Story Points: 5
├─ Deliverable: UC-001-select-version.md (~100 lines, 10 sections)
├─ Content (standard UC template):
│  1. Overview: Select Office version
│  2. Actors: End User
│  3. Preconditions: Application started, no previous version selected
│  4. Basic Flow: User selects version from list (2024, 2021, 2019)
│  5. Alternative Flows: Invalid version (error), user cancels
│  6. Postconditions: $Config.Version set, state=VersionSelected
│  7. Exit Criteria: User confirmed version
│  8. Error Scenarios: Network error, invalid version
│  9. Testing Strategy: Unit test version validation, UI test flow
│  10. References: Stage 1 UC-001, design/data-structures, state-management
├─ Dependencies: T-051 (overall-arch reviewed)
└─ Acceptance Criteria:
   ✓ 10 sections complete
   ✓ Flows clear
   ✓ Error scenarios documented
   ✓ Testing strategy defined

T-053: UC-002-select-language.md (PARALLEL)
├─ Role: ARCHITECT + BA
├─ Duration: 8 hours (Wed-Thu)
├─ Days: 32-33
├─ Story Points: 5
├─ Similar structure to T-052 (UC-001)
├─ Content: Select language, precondition = UC-001 complete
├─ Dependencies: T-051 (overall-arch)

T-054: UC-003-exclude-applications.md (PARALLEL)
├─ Role: ARCHITECT + BA
├─ Duration: 8 hours (Thu-Fri)
├─ Days: 33-34
├─ Story Points: 5
├─ Similar structure to T-052
├─ Content: Exclude apps (Teams, etc), precondition = UC-002 complete
├─ Dependencies: T-051

T-055: UC-004-validate-configuration.md (CRITICAL PATH)
├─ Role: ARCHITECT + BA
├─ Duration: 16 hours (Mon-Fri, parallel to UC-001-003 but longer)
├─ Days: 30-35 (starts Mon, ends Fri, ongoing)
├─ Effort: 16h Claude (longest task in entire Stage 7)
├─ Story Points: 10 (heaviest story)
├─ Deliverable: UC-004-validate-configuration.md (~200 lines, complex)
├─ Content (10 sections + 8 detailed steps):
│  1. Overview: Validate configuration before install
│  2. Actors: System (automatic validation)
│  3. Preconditions: UC-001-003 complete, $Config fully set
│  4. Basic Flow:
│     Step 1: Check version in whitelist (design/data-structures)
│     Step 2: Validate XML schema against XSD
│     Step 3: Check language-version compatibility (matrix)
│     Step 4: Check app-version compatibility (matrix)
│     Step 5: Verify Microsoft hash for integrity
│     Step 6: Perform OCT (Office Customization Tool) validation
│              → Include bug mitigation (from T-005)
│     Step 7: Check system requirements (admin rights, disk space)
│     Step 8: Final authorization (user confirms or cancels)
│  5. Alternative Flows: Each step failure scenario
│  6. Postconditions: $Config.ValidationPassed = true, state=ReadyToInstall
│  7. Exit Criteria: Validation passed or failed
│  8. Error Scenarios: 10+ failure scenarios, mitigations
│  9. Testing Strategy: Comprehensive (unit, integration, system tests)
│  10. References: design/data-structures, design/error-propagation, etc
├─ Dependencies: All prior work (T-019, T-020, T-005, T-006, design/security)
└─ Acceptance Criteria:
   ✓ 8 steps clearly defined
   ✓ OCT bug mitigation documented
   ✓ Error scenarios exhaustive
   ✓ Testing strategy comprehensive
   ✓ Ready for Stage 10 implementation (most complex UC)

T-056: UC-005-install-office.md (DEPENDS ON UC-004)
├─ Role: ARCHITECT + BA
├─ Duration: 12 hours (Thu-Fri, waits for UC-004 partial completion)
├─ Days: 33-35 (starts Thu after UC-004 ~60% done)
├─ Effort: 12h Claude
├─ Story Points: 8
├─ Deliverable: UC-005-install-office.md (~150 lines)
├─ Content (10 sections):
│  1. Overview: Install Office using ODT
│  2. Actors: System (automatic install)
│  3. Preconditions: UC-004 validation passed, $Config ready
│  4. Basic Flow:
│     Step 1: Download ODT (Office Deployment Tool)
│     Step 2: Generate configuration.xml from $Config
│     Step 3: Execute ODT with config.xml
│     Step 4: Monitor installation progress
│     Step 5: Apply post-install configuration (languages, apps)
│     Step 6: Verify installation success
│  5. Alternative Flows: Installation failures, rollback
│  6. Postconditions: Office installed, state=Completed
│  7. Exit Criteria: Installation succeeded or failed
│  8. Error Scenarios: Network failure, disk space, permission issues
│  9. Testing Strategy: VM testing, rollback testing
│  10. References: design/error-propagation, architecture/disaster-recovery
├─ Dependencies: T-051 (overall-arch), T-055 (UC-004 ~60% done)
└─ Acceptance Criteria:
   ✓ All 6 steps clear
   ✓ ODT integration documented
   ✓ Error handling (retry, rollback) documented
   ✓ Testing strategy realistic

T-057: pm-communications-plan.md & pm-stakeholder-register.md
├─ Role: PM
├─ Duration: 4 hours (parallel to UCs, Wed-Thu)
├─ Days: 32-33
├─ Story Points: 3 (combined)
├─ Deliverable: 2 PM documents
├─ Dependencies: T-050+ (Sprint 4 ongoing)

T-058: rm-requirements-validation-report.md
├─ Role: BA + REVIEWER
├─ Duration: 4 hours (Thu-Fri)
├─ Days: 33-35
├─ Story Points: 2
├─ Deliverable: Validation report (stakeholder sign-offs)
├─ Dependencies: All UCs complete (T-052-T-056)

T-059: MID-SPRINT CHECKPOINT (Wed 15:00)
├─ Role: ARCHITECT + PRODUCT OWNER
├─ Duration: 30 minutes
├─ Day: 32 (Wed 15:00-15:30, critical sprint)
├─ Effort: 0.5h each
├─ Check: UC-004 progress (critical path)
├─ Outcome: "On track" or "need help"
├─ Dependencies: T-055 (UC-004 status)
└─ Adjustment: If blocked, escalate immediately

T-060: SPRINT 4 POLISH & FINALIZATION
├─ Role: ARCHITECT + REVIEWER
├─ Duration: 5 hours (Fri morning, after UCs)
├─ Day: 35 (Fri 09:15-14:00)
├─ Effort: 5h Claude
├─ Quality Check: All 9 documents
├─ Focus: UCs especially (most complex, most scrutiny)
├─ Dependencies: T-058 (all stories complete)

T-061: SPRINT 4 REVIEW + DEMO (EXTENDED)
├─ Role: ARCHITECT + PRODUCT OWNER
├─ Duration: 1.5 hours (Fri 15:00-16:30, extended for final sprint)
├─ Day: 35 (Fri 15:00-16:30)
├─ Effort: 1.5h Claude, 1.5h Nestor
├─ Demo:
│  ✓ overall-architecture with diagram
│  ✓ UC-001-005 (all UCs)
│  ✓ PM/RM docs finalized
│  ✓ All 26 Stage 7 docs complete
├─ Outcome: "Ready for Phase 3" or "minor fixes"
├─ Metrics: Velocity = 46 pts (stretch sprint), acceptable
├─ Dependencies: T-060 (docs ready)
└─ Status: APPROVED increment (all UCs designed)

T-062: SPRINT 4 RETROSPECTIVE (EXTENDED)
├─ Role: ARCHITECT + PRODUCT OWNER
├─ Duration: 45 minutes (Fri 16:30-17:15, extended)
├─ Day: 35
├─ Effort: 0.75h each
├─ Reflection:
│  ✓ UC-004 took longer (16h vs 10h estimate) but OK
│  ✓ Velocity slightly higher (46 pts) due to parallel work
│  ✓ Overall PHASE 2 (4 sprints): ~80 story points in 16 days ✓
├─ Action Items: Preparation for Phase 3
└─ Output: Ready for final phase

SPRINT 4 TOTALS:
  Documents: 9 (overall-arch + 5 UCs + 3 PM/RM)
  Story Points: 46 (stretch sprint)
  Hours: ~50 + ~5 (ceremonies)
  Status: COMPLETE & ALL UCs DESIGNED
  Critical: UC-004 completed (longest task)
  Next: Phase 3 (compliance audit + exit gate)

PHASE 2 GRAND TOTAL (4 Sprints):
  Documents: 20 (3+5+9+9, excluding Phase 1)
  Story Points: 100 total
  Hours: 160-180 (Claude) + 20 (ceremonies)
  Sprints: 4 (complete)
  Velocity: 20-25 pts/sprint average ✓
  Quality: DoD 100% all sprints
  Status: PHASE 2 COMPLETE, ALL UCs DESIGNED
  Next: PHASE 3 (compliance audit, exit gate)
```

---

### **PHASE 3 — WATERFALL CIERRE (Week 7, Days 36-40)**

```
T-063: COMPLIANCE AUDIT KICKOFF
├─ Role: ARCHITECT + REVIEWER
├─ Duration: 1 hour
├─ Day: 36 (Mon 09:00-10:00)
├─ Effort: 1h Claude
├─ Deliverable: Audit checklist prepared
├─ Scope: 26 documents (7 Phase 1 + 19 Phase 2)
├─ Checklist:
│  ✓ Metadata (yml block, no ---)
│  ✓ Naming (kebab-case, no numeric prefixes)
│  ✓ Conventions (9 files applied)
│  ✓ No emojis (text only)
│  ✓ Stage references (Stage 1, 6 mentioned)
│  ✓ No speculative claims
│  ✓ Diagrams (dark theme Mermaid)
│  ✓ Code syntax (PowerShell correct)
│  ✓ Cross-references (between docs valid)
├─ Dependencies: T-062 (Phase 2 complete)
└─ Output: Audit plan ready

T-064: COMPLIANCE AUDIT EXECUTION (Days 36-38)
├─ Role: ARCHITECT + REVIEWER
├─ Duration: 12 hours (Mon-Wed)
├─ Days: 36-38 (Mon 10:00-12:00 + 14:00-16:00, Tue full day, Wed morning)
├─ Effort: 12h Claude
├─ Deliverable: COMPLIANCE-AUDIT-REPORT.md
├─ Process:
│  ✓ Document 1-7 (Phase 1): Audit ~3h
│  ✓ Document 8-19 (Phase 2 early): Audit ~5h
│  ✓ Document 20-26 (Phase 2 late): Audit ~4h
│  ✓ Issues found: Note + prioritize
│  ✓ Fixes: Apply immediately (no major issues expected)
├─ Dependencies: T-063 (audit plan)
└─ Output: All 26 docs COMPLIANT (or minor fixes noted)

T-065: STAGE 7 SUMMARY & HANDOFF PREP
├─ Role: ARCHITECT + PM
├─ Duration: 4 hours
├─ Day: 39 (Thu 09:00-13:00)
├─ Effort: 4h Claude
├─ Deliverables:
│  1. STAGE-7-SUMMARY.md (~80 lines)
│     ✓ What we did (4 phases, 7 weeks, 265-300h)
│     ✓ What we delivered (26 docs, 37 gaps resolved)
│     ✓ Quality metrics (velocity, compliance, DoD)
│     ✓ Key decisions (5 ADRs)
│     ✓ Risks (mitigated)
│  2. INDEX.md (~100 lines)
│     ✓ Directory of all 26 documents
│     ✓ TIER 1-5 organization
│     ✓ Links to each doc
│  3. STAGE-10-HANDOFF-CHECKLIST.md (~60 lines)
│     ✓ What Stage 10 must read first (TIER 1)
│     ✓ What Stage 10 must implement (TIER 2)
│     ✓ Reference materials (TIER 3-5)
│     ✓ Contact info for questions
├─ Dependencies: T-064 (audit complete)
└─ Output: Handoff materials ready

T-066: FINAL QUALITY REVIEW
├─ Role: ARCHITECT + REVIEWER
├─ Duration: 3 hours
├─ Day: 39 (Thu 13:00-16:00)
├─ Effort: 3h Claude
├─ Deliverable: Final check (no issues expected)
├─ Checklist:
│  ✓ All 26 docs present
│  ✓ No broken links between docs
│  ✓ Metadata complete
│  ✓ Stage 10 can pick up and run
├─ Dependencies: T-065 (summary docs ready)
└─ Output: Everything ready for exit gate

T-067: EXIT GATE PREPARATION
├─ Role: ARCHITECT
├─ Duration: 2 hours
├─ Day: 40 (Fri 09:00-11:00)
├─ Effort: 2h Claude
├─ Deliverable: Presentation materials prepared
├─ Content:
│  ✓ Demo slides (overall-architecture, sample UCs)
│  ✓ Metrics summary (7 weeks, 26 docs, 37 gaps resolved)
│  ✓ Quality report (compliance audit PASSED)
│  ✓ Risk register (known risks mitigated)
│  ✓ Stage 10 handoff checklist
├─ Dependencies: T-066 (final review)
└─ Output: Ready to present

T-068: STAGE 7 EXIT GATE (FINAL APPROVAL)
├─ Role: ARCHITECT + PRODUCT OWNER
├─ Duration: 1.5 hours
├─ Day: 40 (Fri 10:00-11:30)
├─ Effort: 1.5h Claude, 1.5h Nestor
├─ Deliverable: Stage 7 COMPLETE approval
├─ Presentation:
│  ✓ Overall-architecture diagram
│  ✓ All 5 UCs (UC-001 through UC-005)
│  ✓ Security architecture (threat model)
│  ✓ PM/Risk management (26 docs)
│  ✓ Metrics: 7 weeks, 26 docs, 37 gaps RESOLVED
│  ✓ Quality: Compliance audit PASSED
│  ✓ Risk: All known risks MITIGATED
│  ✓ Stage 10 readiness: GO/NO-GO
├─ Checklist (EXIT CRITERIA):
│  ✓ Requirements baseline FROZEN & approved
│  ✓ Architecture designed (6 layers, threat model)
│  ✓ Security architected (STRIDE complete)
│  ✓ UCs designed (UC-001-005 complete)
│  ✓ PM plans defined (cost, resource, risk, schedule)
│  ✓ Compliance audit PASSED
│  ✓ Stakeholder validation COMPLETE
│  ✓ Stage 10 handoff ready
│  ✓ 0 open blockers
├─ Outcome: APPROVED → PROCEED TO STAGE 10
├─ Dependencies: T-068 (all prep complete)
└─ Status: STAGE 7 COMPLETE

T-069: STAGE 10 HANDOFF MEETING (Optional)
├─ Role: ARCHITECT + PRODUCT OWNER + STAGE 10 TEAM
├─ Duration: 1 hour
├─ Day: 40 (Fri 11:30-12:30)
├─ Effort: 1h each
├─ Deliverable: Stage 10 kickoff
├─ Agenda:
│  ✓ Overview of Stage 7 deliverables
│  ✓ Architecture walkthrough
│  ✓ UC-004 critical details (complex validation)
│  ✓ Q&A from Stage 10
│  ✓ Stage 10 kickoff schedule
├─ Output: Stage 10 ready to begin

T-070: STAGE 7 CLOSURE
├─ Role: ARCHITECT + PM
├─ Duration: 2 hours
├─ Day: 40 (Fri 12:30-14:30)
├─ Effort: 2h Claude
├─ Deliverable: All Stage 7 documents archived
├─ Actions:
│  ✓ Copy all 26 docs to /outputs/
│  ✓ Create STAGE-7-FINAL-REPORT.md
│  ✓ Archive project (tag in git)
│  ✓ Close Stage 7 project
│  ✓ Handoff to Stage 10 team
├─ Output: Stage 7 CLOSED, Stage 10 ready to begin

PHASE 3 TOTALS:
  Tasks: 8 (T-063 through T-070)
  Hours: ~35-40 (Claude)
  Status: STAGE 7 COMPLETE & CLOSED
  Outcome: EXIT GATE PASSED, STAGE 10 READY
  Next: Stage 10 IMPLEMENTATION begins
```

---

## PART 3: TASK SUMMARY TABLE

```
TASK ID   TITLE                                    ROLE         HOURS   STORY PTS   DAYS    STATUS
════════════════════════════════════════════════════════════════════════════════════════════════════

PHASE 1 — WATERFALL CIMENTACIÓN (70-80h)

T-001     PROJECT KICKOFF & PLANNING               ARCH+PO      2h      —          1       READY
T-002     rm-requirements-baseline.md DRAFT        BA           5h      —          1-2     READY
T-003     DAILY STANDUP (Days 1-10, recurring)     ARCH+PO      0.17h   —          1-10    ROUTINE
T-004     rm-stakeholder-requirements.md DRAFT     BA           6h      —          2-3     READY
T-005     rm-requirements-clarification.md DRAFT   BA           7h      —          3-4     READY
T-006     design/data-structures.md DRAFT          ARCH         6h      —          3-4     READY
T-007     DOCUMENTATION CONSOLIDATION (T-002-006) ARCH+REV     3h      —          5       READY
T-008     CHECKPOINT 1 APPROVAL                    ARCH+PO      1h      —          5       GATE
T-009     WEEK 1 SUMMARY & RETROSPECTIVE           ARCH         1h      —          5       ROUTINE

T-010     pm-charter.md DRAFT                      PM           4h      —          6-7     READY
T-011     pm-scope-statement.md DRAFT              PM           4h      —          7-8     READY
T-012     architecture/security-architecture DRAFT ARCH         8h      —          6-8     READY
T-013     pm-scope-statement REFINEMENT            PM           2h      —          9       READY
T-014     REVIEW & POLISH (T-010-013)              ARCH+REV     3h      —          9       READY
T-015     CHECKPOINT 1.5 APPROVAL                  ARCH+PO      1h      —          10      GATE
T-016     PHASE 1 CLOSURE & SPRINT PLANNING PREP   ARCH+PM      3h      —          10      READY

PHASE 2 — AGILE SCRUM (160-180h + 20h ceremonies)

SPRINT 1 (T-017-T-025, Week 3, Days 11-15)

T-017     SPRINT 1 PLANNING                        ARCH+PO      1h      —          11      READY
T-018     DAILY STANDUP (S1, Days 11-15)           ARCH+PO      1.25h   —          11-15   ROUTINE
T-019     design/state-management-design.md        ARCH         8h      5 pts      11-12   READY
T-020     design/error-propagation-strategy.md     ARCH         8h      5 pts      12-13   READY
T-021     pm-schedule-baseline.md REALISTIC        PM           8h      5 pts      13-14   READY
T-022     DESIGN REVIEW CHECKPOINT (Wed 3pm)       ARCH+PO      0.25h   —          13      ROUTINE
T-023     SPRINT 1 POLISH & FINALIZATION           ARCH+REV     4h      —          14      READY
T-024     SPRINT 1 REVIEW + DEMO                   ARCH+PO      1h      —          15      GATE
T-025     SPRINT 1 RETROSPECTIVE                   ARCH+PO      0.5h    —          15      ROUTINE

SPRINT 2 (T-026-T-035, Week 4, Days 16-20)

T-026     SPRINT 2 PLANNING                        ARCH+PO      1h      —          16      READY
T-027     DAILY STANDUP (S2, Days 16-20)           ARCH+PO      1.25h   —          16-20   ROUTINE
T-028     rm-requirements-formal-srs.md (IEEE 830) RM           8h      5 pts      16-18   READY
T-029     rm-requirements-traceability-matrix.md   RM           8h      5 pts      17-18   READY
T-030     design/logging-specification.md          BA           4h      3 pts      17-18   READY
T-031     architecture/disaster-recovery.md        ARCH         8h      5 pts      18-19   READY
T-032     DESIGN REVIEW CHECKPOINT (Wed 3pm)       ARCH+PO      0.25h   —          18      ROUTINE
T-033     SPRINT 2 POLISH & FINALIZATION           ARCH+REV     4h      —          19      READY
T-034     SPRINT 2 REVIEW + DEMO                   ARCH+PO      1h      —          20      GATE
T-035     SPRINT 2 RETROSPECTIVE                   ARCH+PO      0.5h    —          20      ROUTINE

SPRINT 3 (T-036-T-049, Week 5, Days 23-28)

T-036     SPRINT 3 PLANNING                        ARCH+PO      1h      —          23      READY
T-037     DAILY STANDUP (S3, Days 23-27)           ARCH+PO      1.25h   —          23-27   ROUTINE
T-038     adr-config-state-model.md                ARCH         2h      1 pt       23      READY
T-039     adr-layered-architecture.md              ARCH         2h      1 pt       23-24   READY
T-040     adr-idempotence-scope.md                 ARCH         2h      1 pt       24      READY
T-041     architecture/quality-attributes.md       ARCH         4h      2 pts      24-25   READY
T-042     architecture/extensibility-strategy.md   ARCH         4h      2 pts      25-26   READY
T-043     pm-cost-estimate.md + pm-resource-plan   PM           6h      3 pts      25-26   READY
T-044     pm-risk-register.md                      PM           4h      2 pts      26      READY
T-045     pm-change-control-process.md             PM           2h      1 pt       26-27   READY
T-046     DESIGN REVIEW CHECKPOINT (Wed 3pm)       ARCH+PO      0.25h   —          25      ROUTINE
T-047     SPRINT 3 POLISH & FINALIZATION           ARCH+REV     3h      —          27      READY
T-048     SPRINT 3 REVIEW + DEMO                   ARCH+PO      1h      —          28      GATE
T-049     SPRINT 3 RETROSPECTIVE                   ARCH+PO      0.5h    —          28      ROUTINE

SPRINT 4 (T-050-T-062, Week 6, Days 30-35)

T-050     SPRINT 4 PLANNING                        ARCH+PO      1h      —          30      READY
T-051     overall-architecture.md                  ARCH         10h     8 pts      30-31   READY
T-052     UC-001-select-version.md                 ARCH+BA      8h      5 pts      31-32   READY
T-053     UC-002-select-language.md                ARCH+BA      8h      5 pts      32-33   READY
T-054     UC-003-exclude-applications.md           ARCH+BA      8h      5 pts      33-34   READY
T-055     UC-004-validate-configuration.md         ARCH+BA      16h     10 pts     30-35   CRITICAL
T-056     UC-005-install-office.md                 ARCH+BA      12h     8 pts      33-35   READY
T-057     pm-communications + pm-stakeholder       PM           4h      3 pts      32-33   READY
T-058     rm-requirements-validation-report.md     RM           4h      2 pts      33-35   READY
T-059     MID-SPRINT CHECKPOINT (Wed 3pm)          ARCH+PO      0.5h    —          32      ROUTINE
T-060     SPRINT 4 POLISH & FINALIZATION           ARCH+REV     5h      —          35      READY
T-061     SPRINT 4 REVIEW + DEMO (EXTENDED)        ARCH+PO      1.5h    —          35      GATE
T-062     SPRINT 4 RETROSPECTIVE (EXTENDED)        ARCH+PO      0.75h   —          35      ROUTINE

PHASE 3 — WATERFALL CIERRE (35-40h)

T-063     COMPLIANCE AUDIT KICKOFF                 ARCH+REV     1h      —          36      READY
T-064     COMPLIANCE AUDIT EXECUTION               ARCH+REV     12h     —          36-38   READY
T-065     STAGE 7 SUMMARY & HANDOFF PREP           ARCH+PM      4h      —          39      READY
T-066     FINAL QUALITY REVIEW                     ARCH+REV     3h      —          39      READY
T-067     EXIT GATE PREPARATION                    ARCH         2h      —          40      READY
T-068     STAGE 7 EXIT GATE (FINAL APPROVAL)       ARCH+PO      1.5h    —          40      GATE
T-069     STAGE 10 HANDOFF MEETING (Optional)      ARCH+PO+S10  1h      —          40      OPTIONAL
T-070     STAGE 7 CLOSURE                          ARCH+PM      2h      —          40      READY

═══════════════════════════════════════════════════════════════════════════════════════════════════
TOTALS:   70 TASKS                                                ~300h+20h     35 days  READY
          4 PHASES                                                (ceremonies)
          2 GATES (CP1, CP1.5)
          4 SPRINTS (S1-S4)
          5 SPRINT REVIEWS + RETROS
          100 STORY POINTS (PHASE 2)
          26 DOCUMENTS DELIVERED
          37 GAPS RESOLVED
═══════════════════════════════════════════════════════════════════════════════════════════════════
```

---

## PART 4: DEPENDENCIES & CRITICAL PATH

### Dependency Graph

```
T-001 (KICKOFF)
  ├─ T-002 (baseline)
  │  └─ T-003 (standup daily)
  │     ├─ T-004 (stakeholders)
  │     │  └─ T-005 (clarification)
  │     │     └─ T-006 (data-structures)
  │     │        └─ T-007 (consolidation)
  │     │           └─ T-008 (CP1 gate)
  │     │              └─ T-009 (W1 retro)
  │     │                 ├─ T-010 (charter)
  │     │                 │  └─ T-011 (scope)
  │     │                 │     ├─ T-012 (security)
  │     │                 │     └─ T-013 (scope refine)
  │     │                 │        └─ T-014 (polish)
  │     │                 │           └─ T-015 (CP1.5)
  │     │                 │              └─ T-016 (closure)
  │     │                 │                 └─ T-017 (S1 planning)
  │     │                 │                    ├─ T-019 (state-mgmt)
  │     │                 │                    ├─ T-020 (error-prop)
  │     │                 │                    └─ T-021 (schedule)
  │     │                 │                       └─ T-024 (S1 review)
  │     │                 │                          └─ T-025 (S1 retro)
  │     │                 │                             └─ T-026 (S2 planning)
  │     │                 │                                ...
  │     │                 └─ (parallel to T-010-015)

CRITICAL PATH:
  T-001 → T-002 → T-005 → T-006 → T-007 → T-008 → T-009
       → T-012 → T-013 → T-015 → T-017 → T-019 → T-024
       → T-026 → T-028 → T-034 → T-036 → T-051 → T-055 → T-061
       → T-063 → T-064 → T-068 → T-070
  
  Length: ~10 days (T-001 → T-008) + 4 sprints (T-017-T-062) + 5 days (T-063-T-070)
         = 35 days total ✓

PARALLEL OPPORTUNITIES:
  T-010, T-011, T-012 can run parallel (T-009 onwards)
  T-019, T-020, T-021 can run parallel (Sprint 1)
  T-052, T-053, T-054 can run parallel (Sprint 4, while T-055 UC-004 in progress)
  T-057, T-058 can run parallel (Sprint 4)
```

---

## PART 5: ROLE ALLOCATION

### Roles & Responsibilities

```
ROLE: ARCHITECT (Role, played by Claude)
  Responsibilities:
    ✓ Design architecture (6-layer, security, state machine)
    ✓ Specify all UCs
    ✓ Create ADRs (architecture decisions)
    ✓ Review technical quality
  
  Tasks: T-001, T-002, T-004, T-005, T-006, T-007, T-012, T-013, T-014, T-015,
         T-019, T-020, T-021, T-022, T-023, T-024, T-025, T-026, T-028, T-029,
         T-031, T-032, T-033, T-034, T-038, T-039, T-040, T-041, T-042, T-046,
         T-047, T-048, T-051, T-052, T-053, T-054, T-055, T-056, T-060, T-061,
         T-063, T-064, T-066, T-067, T-068, T-070
  
  Total Hours: ~200h

ROLE: BA (Business Analyst, played by Claude)
  Responsibilities:
    ✓ Elicit and clarify requirements
    ✓ Validate against stakeholders
    ✓ Write UC flow logic
    ✓ Ensure UC testing strategy
  
  Tasks: T-002, T-004, T-005, T-006, T-030, T-052, T-053, T-054, T-055, T-056,
         T-058, T-064
  
  Total Hours: ~40h

ROLE: PM (Project Manager, played by Claude)
  Responsibilities:
    ✓ Plan project (charter, scope, schedule)
    ✓ Estimate costs, manage risks
    ✓ Change control
    ✓ Stakeholder communication
  
  Tasks: T-010, T-011, T-013, T-016, T-021, T-026, T-043, T-044, T-045, T-057,
         T-065, T-070
  
  Total Hours: ~40h

ROLE: REVIEWER (Quality Assurance, played by Claude + Nestor)
  Responsibilities:
    ✓ Verify compliance (9 conventions)
    ✓ Spot-check quality
    ✓ Ensure cross-references
  
  Tasks: T-007, T-014, T-023, T-033, T-047, T-060, T-064, T-066
  
  Total Hours: ~30h (Claude), 5h (Nestor)

ROLE: PRODUCT OWNER (Decision maker, played by Nestor)
  Responsibilities:
    ✓ Approve documents
    ✓ Provide feedback
    ✓ Gate decisions (CP1, CP1.5, etc)
    ✓ Sprint ceremonies
  
  Tasks: T-001, T-003, T-008, T-015, T-017, T-018, T-022, T-024, T-025, T-026,
         T-032, T-034, T-035, T-046, T-048, T-049, T-059, T-061, T-062, T-068
  
  Total Hours: ~50h

SUMMARY:
  Claude plays: ARCHITECT + BA + PM + REVIEWER (~310h total, 7 weeks ~45h/week)
  Nestor plays: PRODUCT OWNER (~50h total, ~7h/week)
  
  NOTE: Claude's effort is sustainable (45-50h/week is normal for intensive project)
        Nestor's effort is minimal (7h/week, low ceremony overhead) ✓
```

---

## PART 6: EFFORT SUMMARY

```
PHASE 1 (WATERFALL):
  Duration: 2 weeks (10 working days)
  Tasks: T-001 through T-016
  Effort: 70-80 hours (Claude), 2 hours (Nestor)
  Story Points: — (no sprints)
  Documents: 7
  Status: 2 gates (CP1, CP1.5)

PHASE 2 (AGILE):
  Duration: 4 weeks (20 working days)
  Tasks: T-017 through T-062
  Effort: 160-180 hours (Claude), 30 hours (Nestor)
  Story Points: 100 total (20-25 pts/sprint avg)
  Documents: 20
  Sprints: 4 (S1-S4)
  Ceremonies: 20 daily standups, 4 planning, 4 review, 4 retro, 4 design reviews
  Status: 4 sprint gates

PHASE 3 (WATERFALL):
  Duration: 1 week (5 working days)
  Tasks: T-063 through T-070
  Effort: 35-40 hours (Claude), 3 hours (Nestor)
  Story Points: — (closing phase)
  Documents: 3 summaries (summary, index, handoff)
  Status: 1 exit gate (final)

TOTAL STAGE 7:
  Duration: 7 weeks (35 working days)
  Tasks: 70 tasks (T-001 through T-070)
  Effort: 265-300 hours (Claude), ~50 hours (Nestor)
  Story Points: 100 (PHASE 2 only)
  Documents: 26 delivered (7+20-1 summary)
  Gates: 7 major checkpoints (CP1, CP1.5, S1-S4 reviews, exit)
  Quality: DoD 100% per sprint, Compliance audit PASSED
  Status: STAGE 7 COMPLETE → STAGE 10 READY
```

---

## PART 7: EXECUTION CHECKLIST

```
BEFORE DAY 1 (Pre-Phase 1):

☐ Nestor approves PLAN HYBRID WATER-SCRUM-FALL
☐ Nestor confirms availability (6-7h/week Phase 2)
☐ Claude prepares kickoff meeting materials (T-001)
☐ Calendar scheduled for all ceremonies (daily standup, sprints, gates)
☐ Definition of Done agreed (for all sprints)
☐ Product backlog organized (for Sprint 1 planning)

PHASE 1 KICKOFF (Day 1, Monday):

☐ T-001 executed: Project kickoff meeting
☐ Phase 1 timeline confirmed: 2 weeks (Days 1-10)
☐ Documents T-002 through T-006 started
☐ Daily standups begun (T-003 onwards, recurring)

PHASE 1 COMPLETION (Day 10, Friday):

☐ T-008 executed: CP1 GATE (baseline frozen)
☐ T-015 executed: CP1.5 GATE (foundation complete)
☐ 7 Phase 1 documents DELIVERED
☐ Phase 1 retrospective (T-009) completed
☐ Phase 2 (Sprint 1) planning prepared (T-016)

PHASE 2 SPRINT 1 (Week 3, Days 11-15):

☐ T-017 executed: Sprint 1 planning
☐ T-019, T-020, T-021: 3 stories (20 pts) worked on
☐ T-022 executed: Wed 3pm design review
☐ T-024 executed: Fri sprint review + demo
☐ T-025 executed: Fri sprint retrospective
☐ 3 Phase 2 documents DELIVERED

(REPEAT for Sprints 2-4: T-026-T-062)

PHASE 3 CLOSURE (Week 7, Days 36-40):

☐ T-063 executed: Audit plan prepared
☐ T-064 executed: 26 documents audited
☐ T-068 executed: STAGE 7 EXIT GATE
☐ T-070 executed: Project closure + handoff

POST STAGE 7:

☐ All 26 documents copied to /outputs/
☐ STAGE-7-FINAL-REPORT.md created
☐ Handoff meeting scheduled with Stage 10 team
☐ Stage 7 project CLOSED
☐ Stage 10 BEGINS
```

---

## SUMMARY

```
TASK PLAN STAGE 7 — EXECUTION READY

Total Tasks:       70 (T-001 through T-070)
Roles:             4 (Architect, BA, PM, Reviewer) played by Claude + Nestor
Timeline:          7 weeks (35 working days)
Effort:            265-300 hours (Claude), ~50 hours (Nestor)
Story Points:      100 (Agile phase only)
Documents:         26 delivered + 3 support (summaries)
Quality Gate:      Compliance audit PASSED + DoD 100% per sprint
Critical Path:     T-001→T-002→T-005→T-006→...→T-055→T-061→T-068→T-070

STATUS: READY FOR EXECUTION

NEXT STEP: ¿START DAY 1?
```

---

**TASK PLAN STAGE 7 completado:** 2026-04-21 08:00:00
**Status:** READY FOR EXECUTION (all 70 tasks defined, estimated, assigned)
**Methodology:** Task-Planner (RUP + Agile decomposition)
**Quality:** Tasks are atomic (2-20h estimable), traceable, and executable

