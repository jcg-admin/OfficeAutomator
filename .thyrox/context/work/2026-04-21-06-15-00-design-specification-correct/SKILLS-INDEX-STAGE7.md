```yml
created_at: 2026-04-21 08:30:00
project: THYROX - OfficeAutomator
type: SKILLS INDEX - Quick Reference for Stage 7
total_skills: 22
location: /tmp/projects/OfficeAutomator/.claude/skills/
usage: Reference this index during Stage 7 execution (T-001 to T-070)
```

# ÍNDICE DE SKILLS DISPONIBLES — Stage 7 Reference

## Overview

**22 SKILLS personalizados del proyecto THYROX** están disponibles en:
```
/tmp/projects/OfficeAutomator/.claude/skills/
```

Estos SKILLS contienen mejores prácticas, patrones, checklists y guías para cada aspecto de Stage 7.

**Cómo usar:**
```bash
# Durante cada tarea T-NNN:
view /tmp/projects/OfficeAutomator/.claude/skills/[SKILL-NAME]/SKILL.md
```

---

## TIER 1: SKILLS CRÍTICOS PARA STAGE 7

### ⭐ CORE PROJECT FRAMEWORK

```
thyrox/SKILL.md
└─ Framework principal THYROX
   • Conceptos fundamentales del proyecto
   • Rol de Thyrox vs etapas (Discover, Scope, Design, etc)
   • Cómo se aplica Thyrox en Stage 7
   • Integraciones con metodologías (Agile, RUP, Waterfall)
   
   USAR EN: T-001 kickoff, context setting
   VIEW: /tmp/projects/OfficeAutomator/.claude/skills/thyrox/SKILL.md
```

### 📋 REQUIREMENTS MANAGEMENT (5 SKILLS)

```
rm-elicitation/SKILL.md
└─ Técnicas para elicitar requisitos de stakeholders
   • Métodos: interviews, workshops, surveys, observation
   • Cómo identificar requisitos ocultos
   • Gestionar conflictos de requisitos
   
   USAR EN: T-004 (stakeholder elicitation), T-005 (clarification)
   VIEW: /tmp/projects/OfficeAutomator/.claude/skills/rm-elicitation/SKILL.md

rm-analysis/SKILL.md
└─ Analizar, priorizar y estructurar requisitos
   • Análisis de dependencias
   • Priorización (MoSCoW, value vs effort)
   • Conflictos de requisitos
   
   USAR EN: T-007 (consolidation), T-028 (SRS creation)
   VIEW: /tmp/projects/OfficeAutomator/.claude/skills/rm-analysis/SKILL.md

rm-specification/SKILL.md
└─ Especificar requisitos formalmente (IEEE 830 SRS)
   • Estructura SRS estándar
   • Cómo escribir requisitos medibles
   • Formato de requisitos funcionales vs no-funcionales
   • Requisitos de seguridad, performance, usability
   
   USAR EN: T-028 (IEEE 830 SRS formal), T-041 (quality attributes)
   VIEW: /tmp/projects/OfficeAutomator/.claude/skills/rm-specification/SKILL.md

rm-validation/SKILL.md
└─ Validar requisitos con stakeholders
   • Técnicas de validación (review, test case mapping, prototypes)
   • Obtener sign-offs formales
   • Gestionar cambios post-validación
   
   USAR EN: T-058 (validation report), T-068 (exit gate)
   VIEW: /tmp/projects/OfficeAutomator/.claude/skills/rm-validation/SKILL.md

rm-management/SKILL.md
└─ Gestionar cambios de requisitos durante proyecto
   • Change control process (CCB)
   • Impacto de cambios
   • Traceabilidad
   
   USAR EN: T-021 (schedule management), T-045 (change control), gate reviews
   VIEW: /tmp/projects/OfficeAutomator/.claude/skills/rm-management/SKILL.md
```

### 🎯 PROJECT MANAGEMENT (3 SKILLS)

```
pm-planning/SKILL.md
└─ Planificar proyecto (charter, scope, schedule, cost, risk)
   • PMBOK v6 knowledge areas
   • Estimación realista (identificar tareas críticas)
   • Planificación de recursos
   • Baseline establishment
   
   USAR EN: T-010 (charter), T-011 (scope), T-021 (schedule)
   VIEW: /tmp/projects/OfficeAutomator/.claude/skills/pm-planning/SKILL.md

pm-executing/SKILL.md
└─ Ejecutar tareas, gestionar equipo, resolver blockers
   • Liderazgo de equipo
   • Resolución de conflictos
   • Quality assurance during execution
   • Daily management
   
   USAR EN: T-019-T-062 (all sprint execution), daily standups
   VIEW: /tmp/projects/OfficeAutomator/.claude/skills/pm-executing/SKILL.md

pm-monitoring/SKILL.md
└─ Monitorear progreso (burndown, velocity, risks, schedule variance)
   • Metrics dashboard (velocity, burndown, WIP)
   • Schedule/cost/quality variance analysis
   • Risk monitoring
   • Corrective actions
   
   USAR EN: T-025 (retro), T-035 (retro), T-049 (retro), gate reviews
   VIEW: /tmp/projects/OfficeAutomator/.claude/skills/pm-monitoring/SKILL.md
```

### 👤 BUSINESS ANALYSIS (3 SKILLS)

```
ba-requirements-analysis/SKILL.md
└─ Analizar requisitos funcionales y no-funcionales
   • Descomponer requisitos en features
   • Identificar actores, precondiciones, postcondiciones
   • Matriz de trazabilidad (req → feature → code)
   
   USAR EN: T-005 (clarification), T-029 (traceability), T-052-T-056 (UC design)
   VIEW: /tmp/projects/OfficeAutomator/.claude/skills/ba-requirements-analysis/SKILL.md

ba-requirements-lifecycle/SKILL.md
└─ Ciclo de vida de requisitos (inception → elaboration → construction)
   • Cómo los requisitos evolucionan
   • Versioning de requisitos
   • Baseline management
   
   USAR EN: T-008 (CP1 baseline freeze), T-015 (CP1.5 foundation lock)
   VIEW: /tmp/projects/OfficeAutomator/.claude/skills/ba-requirements-lifecycle/SKILL.md

ba-elicitation/SKILL.md
└─ Técnicas de elicitation (misma que rm-elicitation, pero BA perspective)
   • Entrevistas estructuradas
   • Talleres de requisitos
   • Análisis de documentos
   
   USAR EN: T-004 (stakeholder requirements), T-005 (clarification)
   VIEW: /tmp/projects/OfficeAutomator/.claude/skills/ba-elicitation/SKILL.md
```

### 🔄 AGILE/SCRUM PROCESS (5 SKILLS)

```
sp-plan/SKILL.md
└─ Sprint planning (backlog refinement, story pointing, sprint goal)
   • Estimación de story points
   • Commit a velocidad
   • Sprint goal definition
   • Ceremony structure
   
   USAR EN: T-017 (S1 planning), T-026 (S2 planning), T-036 (S3 planning), T-050 (S4 planning)
   VIEW: /tmp/projects/OfficeAutomator/.claude/skills/sp-plan/SKILL.md

sp-execute/SKILL.md
└─ Ejecución de sprint (daily standups, task management, blockers)
   • Daily standup structure
   • Task breakdown
   • Blocker resolution
   • Sprint board management
   
   USAR EN: T-018, T-027, T-037, T-051 (daily standups), daily work
   VIEW: /tmp/projects/OfficeAutomator/.claude/skills/sp-execute/SKILL.md

sp-monitor/SKILL.md
└─ Monitorear sprint (burndown, velocity, capacity)
   • Burndown chart interpretation
   • Velocity tracking
   • Work-in-progress (WIP) limits
   • Sprint health indicators
   
   USAR EN: T-024 (S1 review), T-034 (S2 review), T-048 (S3 review), T-061 (S4 review)
   VIEW: /tmp/projects/OfficeAutomator/.claude/skills/sp-monitor/SKILL.md

sp-analysis/SKILL.md
└─ Analizar sprint results (retrospectives, velocity trends, forecasting)
   • Velocity analysis (trends, outliers)
   • Forecast future sprints
   • Root cause analysis (why velocity changed)
   
   USAR EN: T-025 (S1 retro), T-035 (S2 retro), T-049 (S3 retro), T-062 (S4 retro)
   VIEW: /tmp/projects/OfficeAutomator/.claude/skills/sp-analysis/SKILL.md

sp-adjust/SKILL.md
└─ Ajustar sprints (scope management, capacity rebalancing, estimation calibration)
   • Cuando agregar/quitar stories
   • Rebalancear carga
   • Mejorar estimaciones futuras
   
   USAR EN: T-025 (retro action items), T-035, T-049, T-062 (sprints adjustment)
   VIEW: /tmp/projects/OfficeAutomator/.claude/skills/sp-adjust/SKILL.md
```

### 📐 RUP - RATIONAL UNIFIED PROCESS (2 SKILLS)

```
rup-inception/SKILL.md
└─ Inception phase (establish vision, requirements, architecture baseline)
   • Vision document
   • Initial requirements gathering
   • Feasibility assessment
   • Architecture concept
   • Risk identification
   
   USAR EN: T-001-T-016 (PHASE 1, Waterfall inception equivalent)
   VIEW: /tmp/projects/OfficeAutomator/.claude/skills/rup-inception/SKILL.md

rup-elaboration/SKILL.md
└─ Elaboration phase (refine requirements, architecture, design, risk mitigation)
   • Requirement elaboration
   • Architecture refinement
   • Design patterns
   • Iterative development
   • Risk reduction
   
   USAR EN: T-017-T-062 (PHASE 2, Agile sprints are elaboration iterations)
   VIEW: /tmp/projects/OfficeAutomator/.claude/skills/rup-elaboration/SKILL.md
```

---

## TIER 2: SKILLS ÚTILES (NICE TO HAVE)

### 🔀 CHANGE PROCESS (2 SKILLS)

```
cp-plan/SKILL.md
└─ Planificar cambios o decisiones arquitectónicas
   • Impact assessment
   • Decision framework
   • Stakeholder communication
   
   USAR EN: T-012 (security decisions), T-019-T-021 (architecture decisions)
   VIEW: /tmp/projects/OfficeAutomator/.claude/skills/cp-plan/SKILL.md

cp-evaluate/SKILL.md
└─ Evaluar opciones y trade-offs
   • Matriz de decisión
   • Trade-off analysis
   • Risk evaluation
   
   USAR EN: T-041 (quality attributes tradeoffs), T-042 (extensibility strategy)
   VIEW: /tmp/projects/OfficeAutomator/.claude/skills/cp-evaluate/SKILL.md
```

### 🎯 WORKFLOW/SCOPE (1 SKILL)

```
workflow-scope/SKILL.md
└─ Definir scope del trabajo (IN vs OUT)
   • Boundary definition
   • Scope statement structure
   • Scope creep prevention
   
   USAR EN: T-011 (pm-scope-statement), gate reviews, scope questions
   VIEW: /tmp/projects/OfficeAutomator/.claude/skills/workflow-scope/SKILL.md
```

---

## HOW TO USE THIS INDEX DURING STAGE 7

### Example: T-004 (rm-stakeholder-requirements.md)

```
Before starting T-004:
  1. Review: /tmp/projects/OfficeAutomator/.claude/skills/ba-elicitation/SKILL.md
  2. Read: /tmp/projects/OfficeAutomator/.claude/skills/rm-elicitation/SKILL.md
  3. Then: Execute T-004 with best practices from SKILLS

During T-004:
  • Apply techniques from ba-elicitation/SKILL.md
  • Follow structure from rm-elicitation/SKILL.md
  • Check Definition of Done against SKILL expectations

After T-004:
  • Verify artifact quality against SKILL standards
  • If gaps: Review SKILL again, adjust document
```

### Example: T-017 (Sprint 1 Planning)

```
Before T-017:
  1. View: /tmp/projects/OfficeAutomator/.claude/skills/sp-plan/SKILL.md
  2. View: /tmp/projects/OfficeAutomator/.claude/skills/rup-elaboration/SKILL.md
  3. View: /tmp/projects/OfficeAutomator/.claude/skills/pm-planning/SKILL.md

During T-017:
  • Use sprint planning ceremony from sp-plan/SKILL.md
  • Estimate story points per SKILL guidance
  • Set sprint goal per RUP elaboration principles

After T-017:
  • Verify sprint board matches SKILL format
  • Confirm velocity forecast
```

---

## SKILLS BY TASK MAPPING

```
T-001 KICKOFF:
  → thyrox/SKILL.md (context)
  → pm-planning/SKILL.md (planning overview)
  → rup-inception/SKILL.md (phase framework)

T-002-T-007 ELICITATION & CLARIFICATION:
  → ba-elicitation/SKILL.md (techniques)
  → rm-elicitation/SKILL.md (structured approach)
  → rm-analysis/SKILL.md (prioritization)
  → rm-validation/SKILL.md (sign-off)

T-008-T-016 CHARTER, SCOPE, SECURITY:
  → pm-planning/SKILL.md (charter, scope)
  → workflow-scope/SKILL.md (scope statement)
  → cp-plan/SKILL.md (security decisions)

T-017 SPRINT 1 PLANNING:
  → sp-plan/SKILL.md (ceremony structure)
  → rup-elaboration/SKILL.md (elaboration principles)
  → pm-planning/SKILL.md (resource planning)

T-019-T-025 SPRINT 1 EXECUTION:
  → sp-execute/SKILL.md (daily standups)
  → pm-executing/SKILL.md (task execution)
  → sp-monitor/SKILL.md (burndown)
  → sp-analysis/SKILL.md (velocity)
  → sp-adjust/SKILL.md (adjustments)

T-026-T-062 SPRINTS 2-4:
  → Same SKILLS cycle (sp-plan → sp-execute → sp-monitor → sp-analysis)
  → rm-specification/SKILL.md (T-028 SRS)
  → rm-validation/SKILL.md (T-058 validation)
  → cp-evaluate/SKILL.md (T-041, T-042 architecture decisions)

T-063-T-070 PHASE 3 CLOSURE:
  → rm-management/SKILL.md (change control)
  → pm-monitoring/SKILL.md (final metrics)
  → rm-validation/SKILL.md (exit gate sign-off)
```

---

## Quick Access Command

To view any SKILL during Stage 7:

```bash
# Example:
view /tmp/projects/OfficeAutomator/.claude/skills/rm-elicitation/SKILL.md
view /tmp/projects/OfficeAutomator/.claude/skills/sp-plan/SKILL.md
view /tmp/projects/OfficeAutomator/.claude/skills/rup-elaboration/SKILL.md
```

---

## Summary

| Skill | Purpose | Used In Task |
|-------|---------|--------------|
| **thyrox** | Project framework | T-001, context |
| **rm-elicitation** | Elicit requirements | T-004, T-005 |
| **rm-analysis** | Analyze requirements | T-007, T-028 |
| **rm-specification** | Formal SRS (IEEE 830) | T-028, T-041 |
| **rm-validation** | Validate with stakeholders | T-058, T-068 |
| **rm-management** | Change control | T-021, T-045, gates |
| **pm-planning** | Project planning | T-010, T-011, T-021 |
| **pm-executing** | Execute tasks | T-019-T-062 |
| **pm-monitoring** | Monitor progress | T-025, T-035, T-049, T-062 |
| **ba-requirements-analysis** | Analyze functional reqs | T-005, T-029, T-052-T-056 |
| **ba-requirements-lifecycle** | Req lifecycle management | T-008, T-015 |
| **ba-elicitation** | Elicit techniques (BA view) | T-004, T-005 |
| **sp-plan** | Sprint planning | T-017, T-026, T-036, T-050 |
| **sp-execute** | Sprint execution | Daily standups T-018+ |
| **sp-monitor** | Monitor sprint | T-024, T-034, T-048, T-061 |
| **sp-analysis** | Analyze sprint results | T-025, T-035, T-049, T-062 |
| **sp-adjust** | Adjust sprints | T-025+, all retros |
| **rup-inception** | Inception phase | T-001-T-016 (Phase 1) |
| **rup-elaboration** | Elaboration phase | T-017-T-062 (Phase 2) |
| **cp-plan** | Plan changes/decisions | T-012, T-019-T-021 |
| **cp-evaluate** | Evaluate options | T-041, T-042 |
| **workflow-scope** | Scope definition | T-011, gate reviews |

---

**Index Created:** 2026-04-21 08:30:00
**Total Skills Referenced:** 22 TIER 1 + TIER 2
**Ready for Stage 7 Execution:** YES ✓

