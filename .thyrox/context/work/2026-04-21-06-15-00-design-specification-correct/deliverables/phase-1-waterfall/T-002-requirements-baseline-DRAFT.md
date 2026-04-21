```yml
created_at: 2026-04-21 08:45:00
task_id: T-002
task_name: rm-requirements-baseline.md DRAFT
work_package: 2026-04-21-06-15-00-design-specification-correct
phase: 1-Waterfall-Cimentacion
execution_date: 2026-04-28 10:00 (Week 1, Day 1, Monday after T-001 kickoff)
duration_hours: 5
duration_estimate: Exact (Monday 10:00-12:00 + 14:00-16:00, Tuesday 09:15-12:00)
roles_involved: BA (Claude)
dependencies: T-001 (kickoff complete, decision GO)
story_points: N/A (Phase 1 waterfall)
acceptance_criteria:
  - Baseline v1.0.0 documented (all features locked)
  - Roadmap v1.1 separated (future enhancements)
  - Clear IN (v1.0.0) vs OUT (v1.1) separation
  - All requisites referenced to Stage 1 UC-Matrix
  - No ambiguities (clear requirements only)
  - Document ready for review (metadata, formatting correct)
exit_criteria:
  - Draft complete and ready for T-007 consolidation
  - Ready for CP1 approval gate
status: READY FOR EXECUTION
version: 1.0.0
```

# T-002: rm-requirements-baseline.md DRAFT

## Task Overview

Establish the **requirements baseline** for OfficeAutomator Stage 7, separating confirmed requirements for v1.0.0 (frozen, in scope) from future roadmap v1.1 (out of scope, mentioned for context only).

This document becomes the **single source of truth** for what Stage 7 will design and Stage 10 will implement.

**Duration:** 5 hours (Monday 10:00-12:00 + 14:00-16:00, Tuesday 09:15-12:00)
**Input:** [UC-Matrix from Stage 1](../../../.thyrox/context/work/2026-04-14-01-30-00-uc-documentation/use-case-matrix.md)
**Output:** rm-requirements-baseline.md (draft, ~60-80 lines)
**Quality Gate:** Metadata correct, no emojis, relative links valid, ready for T-007 consolidation

---

## Work Steps

### Step 1: Analyze Stage 1 UC-Matrix (30 minutes)

**Input Source:**
```
Reference: .thyrox/context/work/.../use-case-matrix.md (from Stage 1)
```

**Extract:**
- UC-001: Select Version (v1.0.0 requirement)
- UC-002: Select Language (v1.0.0 requirement)
- UC-003: Exclude Applications (v1.0.0 requirement)
- UC-004: Validate Configuration (v1.0.0 requirement, CRITICAL)
- UC-005: Install Office (v1.0.0 requirement)

**Dependencies:**
- UC-001 → UC-002 → UC-003 → UC-004 (mandatory sequence)
- UC-004 → UC-005 (blocker: UC-004 must succeed)

**Success Criteria for this step:**
- All 5 UCs identified
- Dependencies clear
- No confusion between v1.0.0 and v1.1

---

### Step 2: Define v1.0.0 Core Requirements (90 minutes)

**Requirements Elicitation (apply SKILL: rm-elicitation)**

From Stage 1 UC-Matrix, extract core requirements:

#### **Functional Requirements — v1.0.0**

```
REQ-F-001: System shall support version selection (2024, 2021, 2019)
  └─ UC: UC-001
  └─ Scope: v1.0.0 (FROZEN)
  └─ Status: Confirmed

REQ-F-002: System shall support language selection
  └─ UC: UC-002
  └─ Supported languages: [From Stage 1 context: 2 languages v1.0.0, 4 in v1.1]
  └─ Scope: v1.0.0
  └─ Status: Confirmed

REQ-F-003: System shall allow exclusion of Microsoft Office applications
  └─ UC: UC-003
  └─ Applications: Teams, Outlook, Word, Excel, PowerPoint, OneNote, Access (list from Stage 1)
  └─ Scope: v1.0.0
  └─ Status: Confirmed

REQ-F-004: System shall validate configuration before installation
  └─ UC: UC-004
  └─ Validation steps: 8 detailed steps (from T-001 kickoff clarification)
  └─ Scope: v1.0.0 CRITICAL (blocker for UC-005)
  └─ Status: Confirmed

REQ-F-005: System shall install Office using ODT (Office Deployment Tool)
  └─ UC: UC-005
  └─ Precondition: UC-004 validation PASSED
  └─ Scope: v1.0.0
  └─ Status: Confirmed
```

#### **Non-Functional Requirements — v1.0.0**

```
REQ-NF-001: Security - Credential handling
  └─ Description: User authentication token (not password) stored encrypted in %APPDATA%
  └─ Scope: v1.0.0
  └─ Status: Confirmed (from T-001 security architecture kickoff)

REQ-NF-002: Reliability - Rollback capability
  └─ Description: If UC-005 (install) fails, system can rollback to pre-installation state
  └─ Scope: v1.0.0
  └─ Status: Confirmed

REQ-NF-003: Compliance - Audit logging
  └─ Description: All major actions (version select, validation, install) logged
  └─ Scope: v1.0.0
  └─ Status: Confirmed

REQ-NF-004: Usability - Error messages
  └─ Description: User-friendly error messages for all failure scenarios
  └─ Scope: v1.0.0
  └─ Status: Confirmed
```

#### **Data Requirements — v1.0.0**

```
REQ-DATA-001: Version whitelist
  └─ Description: Validated list of supported Office versions (2024, 2021, 2019)
  └─ Source: design/data-structures-and-matrices.md (T-006)
  └─ Scope: v1.0.0
  └─ Status: Confirmed (referenced in T-006)

REQ-DATA-002: Language-version compatibility matrix
  └─ Description: Which languages are compatible with which Office versions
  └─ Source: design/data-structures-and-matrices.md (T-006)
  └─ Languages for v1.0.0: [2 languages confirmed in T-001]
  └─ Scope: v1.0.0
  └─ Status: Confirmed

REQ-DATA-003: Microsoft Office Customization Tool (OCT) schema
  └─ Description: Valid XSD schema for configuration.xml generated by system
  └─ Source: design/data-structures-and-matrices.md (T-006)
  └─ Scope: v1.0.0 (critical for UC-004)
  └─ Status: Confirmed
```

**Success Criteria for this step:**
- All 5 functional requirements documented
- All 4 non-functional requirements documented
- All 3 data requirements documented
- Total: 12 v1.0.0 requirements (baseline)
- Each requirement has UC mapping
- No ambiguities, all confirmed

---

### Step 3: Define v1.1 Roadmap (Separate, NOT in scope) (60 minutes)

**Explicit OUT OF SCOPE items for v1.0.0 (documented for context):**

```
ROADMAP ITEMS — v1.1 (FUTURE, NOT Stage 7 scope)

REQ-F-006-ROADMAP: Additional language support
  └─ Requirement: Support 4 languages total (2 more than v1.0.0)
  └─ UC: UC-002 (extension)
  └─ Scope: v1.1 roadmap (NOT v1.0.0)
  └─ Status: Documented but OUT OF SCOPE

REQ-F-007-ROADMAP: Custom configuration XML editing
  └─ Requirement: Allow users to edit configuration.xml before installation
  └─ UC: NEW (not in Stage 1 v1.0.0)
  └─ Scope: v1.1 roadmap (NOT v1.0.0)
  └─ Status: Documented but OUT OF SCOPE

REQ-F-008-ROADMAP: Automated software updates
  └─ Requirement: Auto-update OfficeAutomator to latest version
  └─ UC: NEW (not in Stage 1 v1.0.0)
  └─ Scope: v1.1 roadmap (NOT v1.0.0)
  └─ Status: Documented but OUT OF SCOPE

REQ-NF-005-ROADMAP: Multi-language UI
  └─ Requirement: UI translated to 4 languages (v1.0.0 is English only, implied)
  └─ Scope: v1.1 roadmap (NOT v1.0.0)
  └─ Status: Documented but OUT OF SCOPE

EXPLICIT CONFIRMATION: These items are NOT part of Stage 7 v1.0.0 design.
                       They are documented for stakeholder context only.
```

**Success Criteria for this step:**
- v1.1 roadmap documented separately
- Clear "NOT Stage 7 scope" marker on each item
- Prevents scope creep (requirement not in baseline = not in scope)

---

### Step 4: Create Document Structure (60 minutes)

**Document: rm-requirements-baseline.md**

Structure (apply conventions from SKILL rm-elicitation):

```markdown
---yml---
Title: Requirements Baseline v1.0.0
Version: 1.0.0 (FROZEN)
Date: 2026-04-28 (after T-001 kickoff approval)

---

# REQUIREMENTS BASELINE v1.0.0 — OfficeAutomator

## 1. Overview

Brief statement of what v1.0.0 includes and what it does NOT include.

Example:
  "OfficeAutomator v1.0.0 is a Windows-based tool that automates Office deployment
   using Microsoft ODT (Office Deployment Tool). It guides users through version
   selection, language selection, application exclusion, validation, and installation.
   
   v1.0.0 supports 2 languages and 3 Office versions (2024, 2021, 2019).
   
   v1.0.0 does NOT include: Custom XML editing, auto-updates, 4-language support.
   (Those are v1.1 roadmap items.)"

## 2. Scope Statement

### In Scope (v1.0.0 — FROZEN)
- Requirement X
- Requirement Y
- ...

### Out of Scope (v1.1 Roadmap — NOT this stage)
- Future item X
- Future item Y
- ...

## 3. Functional Requirements (v1.0.0)

REQ-F-001: Version Selection
  • Description
  • UC: UC-001
  • Status: Confirmed

[... all 5 functional requirements ...]

## 4. Non-Functional Requirements (v1.0.0)

REQ-NF-001: Security - Credential Handling
  • Description
  • Status: Confirmed

[... all 4 non-functional requirements ...]

## 5. Data Requirements (v1.0.0)

REQ-DATA-001: Version Whitelist
  • Description
  • Source: design/data-structures-and-matrices.md

[... all 3 data requirements ...]

## 6. Dependencies

UC Dependencies:
  UC-001 → UC-002 → UC-003 → UC-004 (mandatory chain)
  UC-004 → UC-005 (blocker: UC-004 success required)

Requirement Dependencies:
  All 5 functional reqs must be met for v1.0.0 complete

## 7. Acceptance Criteria (for Stage 7 design)

Stage 7 design is COMPLETE when:
  ✓ All 12 requirements (5F + 4NF + 3D) are addressed in UCs
  ✓ No requirements are missing or undefined
  ✓ UC-005 install succeeds ONLY if UC-004 validation passed
  ✓ Security (REQ-NF-001) is architected
  ✓ Rollback (REQ-NF-002) is designed

## 8. Change Control

This baseline is FROZEN as of [CP1 approval date].

Any changes to v1.0.0 requirements require:
  1. Issue identification (what changed, why)
  2. Impact analysis (what else breaks)
  3. CCB (Change Control Board) review
  4. Nestor (PO) approval
  5. All affected documents updated

## 9. References

- Stage 1 UC-Matrix: [link to stage 1 use-case-matrix.md]
- T-001 Kickoff Notes: [link to T-001-kickoff-planning.md]
- T-006 Data Structures: design/data-structures-and-matrices.md (to be created)
- [SKILL rm-elicitation]: /tmp/projects/.../rm-elicitation/SKILL.md

## 10. Approval

Baseline approved by:
  - Product Owner (Nestor): [APPROVAL PENDING CP1 gate]
  - Date: [After CP1 approval, 2026-04-28 or later]
  - Status: FROZEN (no further changes without CCB)

---

END BASELINE v1.0.0
```

**Success Criteria for this step:**
- All 10 sections present
- Metadata yml correct (no --- frontmatter)
- No emojis
- Relative links ready
- Clear "FROZEN" status
- Ready for consolidation (T-007)

---

### Step 5: Quality Review (30 minutes)

**Self-review checklist:**

```
METADATA:
  ☑ yml block at top (correct format)
  ☑ task_id, task_name present
  ☑ acceptance_criteria listed
  ☑ version field set to "1.0.0"
  ☑ status field set to appropriate value

CONTENT:
  ☑ Overview clear (what IS in scope, what ISN'T)
  ☑ All 5 functional requirements documented
  ☑ All 4 non-functional requirements documented
  ☑ All 3 data requirements documented
  ☑ UC mapping present on each requirement
  ☑ v1.1 roadmap explicitly separated and marked "OUT OF SCOPE"
  ☑ Dependencies documented
  ☑ Change control process clear
  ☑ Approval section ready for CP1 gate

FORMATTING:
  ☑ No emojis (text only)
  ☑ Headers consistent (## level 2 for sections)
  ☑ Code blocks for technical content
  ☑ Bullet points readable
  ☑ Line length reasonable

REFERENCES:
  ☑ Relative links format [text](./file.md) ready
  ☑ Stage 1 UC-Matrix referenced
  ☑ Future documents referenced with correct paths
  ☑ SKILL references included

CONVENCIONES (9 archivos aplicadas):
  ☑ No emojis ✓
  ☑ Metadata yml (no ---) ✓
  ☑ Professional language ✓
  ☑ Naming kebab-case ✓
  ☑ No speculative claims ✓
  ☑ Stage references ✓
  ☑ Consistent formatting ✓
  ☑ Clear intent ✓
  ☑ Action-oriented ✓
```

**If any item fails:** Fix before proceeding to consolidation.

---

## Deliverable Specification

### File Name
```
rm-requirements-baseline.md
```

### File Location
```
/tmp/projects/OfficeAutomator/.thyrox/context/work/2026-04-21-06-15-00-design-specification-correct/
deliverables/phase-1-waterfall/
└─ rm-requirements-baseline.md
```

### File Size Estimate
```
Content: ~60-80 lines (markdown)
Final size: ~4-5 KB
```

### Version Control
```
Version: 1.0.0 (DRAFT after T-002)
         will become 1.0.0 APPROVED after CP1 gate (T-008)
Status: DRAFT → ready for T-007 consolidation
```

---

## Success Criteria

This task (T-002) is COMPLETE when:

```
☑ Document created and saved to correct location
☑ Metadata yml present and correct
☑ All 12 requirements documented (5F + 4NF + 3D)
☑ v1.0.0 and v1.1 clearly separated
☑ No ambiguities (every requirement is clear, measurable)
☑ UC mappings present
☑ Relative links functional
☑ Quality checklist 100% passed
☑ Ready for T-007 consolidation step
☑ Ready for CP1 approval gate
```

---

## Next Task

After T-002 complete:
- T-003: Daily standups continue (recurring)
- T-004: rm-stakeholder-requirements.md DRAFT
- T-005: rm-requirements-clarification.md DRAFT
- T-006: design/data-structures-and-matrices.md DRAFT
- T-007: Consolidation + Polish
- T-008: CP1 APPROVAL GATE

---

**T-002 Task Ready for Execution**

**Duration:** 5 hours
**When:** Monday 10:00 + Tuesday 09:15 (after T-001 kickoff approval)
**Quality Target:** DoD 100% compliance
**Blocking Gate:** CP1 approval (Friday Day 5)

