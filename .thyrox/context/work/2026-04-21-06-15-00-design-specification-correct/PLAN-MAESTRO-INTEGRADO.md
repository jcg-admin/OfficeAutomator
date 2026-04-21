```yml
created_at: 2026-04-21 07:15:00
project: THYROX
work_package: 2026-04-21-06-15-00-design-specification-correct
type: Master Integrated Plan - Consolidation of 4 Perspectives
methodology: Multi-perspective Architecture Review + Requirements Management + Project Management
author: Claude (Integrated perspective)
status: Borrador
version: 4.0.0-integrated
```

# PLAN MAESTRO INTEGRADO — Stage 7

## Pregunta Fundamental

**¿Cómo ejecutar Stage 7 DESIGN/SPECIFY exitosamente considerando TODAS las perspectivas (BA, PM, RM, Arquitecto)?**

---

## TABLA DE CONTENIDOS

1. Consolidación de Gaps (37 total)
2. Matriz de Documentos (29 total) + Dependencias
3. Roadmap de Ejecución Integrado
4. Timeline Realista
5. Priorización MUST DO vs NICE TO HAVE
6. Checkpoints de Integración

---

## PARTE 1: CONSOLIDACIÓN DE GAPS (37 TOTAL)

### Análisis de Gaps por Perspectiva

#### **PERSPECTIVA BA (5 gaps → 4 documentos)**

| Gap ID | Descripción | Severidad | Tipo | Resolución | Status |
|--------|-------------|-----------|------|-----------|--------|
| **GAP-BA-001** | Data structures externos no especificados | 🔴 CRÍTICO | Missing Spec | design/data-structures-and-matrices.md | NEW |
| **GAP-BA-002** | Error propagation entre UCs no especificada | 🔴 CRÍTICO | Missing Spec | design/error-propagation-strategy.md | NEW |
| **GAP-BA-003** | Logging strategy detallada faltante | 🟠 ALTO | Missing Spec | design/logging-specification.md | NEW |
| **GAP-BA-004** | Idempotence scope ambiguo | 🟠 ALTO | Ambigüedad | adr-idempotence-scope.md | NEW |
| **GAP-BA-005** | Microsoft OCT bug mitigation no especificada | 🟠 ALTO | Missing Detail | Expandir design/uc-004 | EXPAND |

---

#### **PERSPECTIVA PM (7/10 Knowledge Areas → 10 documentos)**

| Gap ID | Descripción | Severidad | KA | Resolución | Status |
|--------|-------------|-----------|-------|-----------|--------|
| **GAP-PM-001** | Charter no existe | 🔴 CRÍTICO | Integration | pm-charter.md | NEW |
| **GAP-PM-002** | Scope statement no formal | 🔴 CRÍTICO | Scope | pm-scope-statement.md | NEW |
| **GAP-PM-003** | Timeline 88% debajo realidad | 🔴 CRÍTICO | Schedule | pm-schedule-baseline.md (REPLANNING) | REPLACE |
| **GAP-PM-004** | Cost estimate no existe | 🔴 CRÍTICO | Cost | pm-cost-estimate.md | NEW |
| **GAP-PM-005** | Definition of Done no existe | 🔴 CRÍTICO | Quality | pm-quality-plan.md | NEW |
| **GAP-PM-006** | Resource asignations no claras | 🔴 CRÍTICO | Resources | pm-resource-plan.md | NEW |
| **GAP-PM-007** | Communications plan faltante | 🟠 ALTO | Communications | pm-communications-plan.md | NEW |
| **GAP-PM-008** | Risk register no existe | 🔴 CRÍTICO | Risk | pm-risk-register.md | NEW |
| **GAP-PM-009** | Change control no existe | 🔴 CRÍTICO | Integration | pm-change-control-process.md | NEW |
| **GAP-PM-010** | Stakeholder engagement no formal | 🟠 ALTO | Stakeholders | pm-stakeholder-register.md | NEW |

**Status PM: 7/10 Knowledge Areas incomplete → 10 documentos requeridos**

---

#### **PERSPECTIVA RM (14 gaps → 7 documentos)**

| Gap ID | Descripción | Severidad | Paso RM | Resolución | Status |
|--------|-------------|-----------|---------|-----------|--------|
| **GAP-RM-001** | Stakeholders NO elicitados | 🔴 CRÍTICO | Elicitation | rm-stakeholder-requirements.md | NEW |
| **GAP-RM-002** | Non-Functional Requirements parciales | 🟠 ALTO | Elicitation | design/non-functional-requirements.md | NEW |
| **GAP-RM-003** | Requisitos UI/UX no especificados | 🟠 ALTO | Elicitation | design/ui-ux-requirements.md | NEW |
| **GAP-RM-004** | Requisitos integración faltantes | 🟠 ALTO | Elicitation | design/integration-requirements.md | NEW |
| **GAP-RM-005** | 8 pasos validación ambiguos | 🔴 CRÍTICO | Analysis | rm-requirements-clarification.md | NEW |
| **GAP-RM-006** | Conflicto Fail-Fast vs Repair Mode | 🔴 CRÍTICO | Analysis | rm-conflict-resolution.md | NEW |
| **GAP-RM-007** | Confusión Req vs Design decision | 🟠 ALTO | Analysis | rm-requirements-vs-design.md | NEW |
| **GAP-RM-008** | Especificación NO formal (no IEEE 830) | 🔴 CRÍTICO | Specification | rm-requirements-formal-srs.md | NEW |
| **GAP-RM-009** | Baseline requisitos NO congelada | 🔴 CRÍTICO | Specification | rm-requirements-baseline.md | NEW |
| **GAP-RM-010** | Matriz trazabilidad NO existe | 🔴 CRÍTICO | Specification | rm-requirements-traceability-matrix.md | NEW |
| **GAP-RM-011** | Validación con stakeholders NO formal | 🔴 CRÍTICO | Validation | rm-requirements-validation-report.md | NEW |
| **GAP-RM-012** | Validación técnica con Stage 10 Dev | 🟠 ALTO | Validation | (incluir en rm-validation-report) | PART-OF |
| **GAP-RM-013** | Change control NO existe | 🔴 CRÍTICO | Management | pm-change-control (PM) | SHARED |
| **GAP-RM-014** | Trazabilidad débil | 🔴 CRÍTICO | Management | rm-requirements-traceability-matrix | SAME-AS-010 |

**Status RM: 1/5 pasos completo → 7 documentos requeridos (algunos shared con PM)**

---

#### **PERSPECTIVA ARQUITECTO (11 gaps → 8 documentos)**

| Gap ID | Descripción | Severidad | Categoría | Resolución | Status |
|--------|-------------|-----------|-----------|-----------|--------|
| **GAP-ARQ-001** | $Config state management débil | 🟠 ALTO | State Management | design/state-management-design.md | NEW |
| **GAP-ARQ-002** | Idempotence scope unclear | 🟠 ALTO | Design | adr-idempotence-scope.md (MISMO QUE BA-004) | SAME-AS-BA-004 |
| **GAP-ARQ-003** | Security architecture FALTANTE | 🔴 CRÍTICO | Security | architecture/security-architecture.md | NEW |
| **GAP-ARQ-004** | State Machine NO formal | 🔴 CRÍTICO | State Management | architecture/state-machine-design.md | NEW |
| **GAP-ARQ-005** | Patrón reutilización no definido | 🟠 ALTO | Design Pattern | architecture/design-patterns.md | NEW |
| **GAP-ARQ-006** | Disaster recovery ausente | 🔴 CRÍTICO | Resilience | architecture/disaster-recovery-design.md | NEW |
| **GAP-ARQ-007** | Non-Functional trade-offs no documentados | 🟠 ALTO | Quality Attributes | architecture/quality-attributes-tradeoffs.md | NEW |
| **GAP-ARQ-008** | Coupling via $Config potencial | 🟠 MEDIA | Design | (mitigado por GAP-ARQ-001) | PART-OF-001 |
| **GAP-ARQ-009** | ADR para $Config faltante | 🟠 ALTO | ADR | adr-config-state-model.md | NEW |
| **GAP-ARQ-010** | ADR para 6-layer architecture | 🟠 ALTO | ADR | adr-layered-architecture.md | NEW |
| **GAP-ARQ-011** | Escalabilidad futura no considerada | 🟠 MEDIA | Design | architecture/extensibility-strategy.md | NEW |

**Status Arquitecto: Modularidad buena (8/10), pero gaps críticos en Security (1/10) → 8 documentos requeridos**

---

### Consolidación: 37 Gaps → 26 Documentos Únicos

```
PERSPECTIVA      GAPS   DOCUMENTOS    DOCUMENTOS ÚNICOS
════════════════════════════════════════════════════════════
BA                 5         4              4
PM                10        10             10
RM                14         7              6 (1 shared con PM)
ARQUITECTO        11         8              7 (1 same-as BA)
────────────────────────────────────────────────────────
SUBTOTAL          40        29             26 (después dedup)
DUPLICADOS         3                       (2 shared PM/RM, 1 same-as BA)
FINAL             37        29             26 ✓
```

---

## PARTE 2: MATRIZ DE DOCUMENTOS (26 ÚNICOS)

### Matriz Master: Documento → Gaps que Resuelve → Dependencias → Timeline

#### **TIER A: DOCUMENTOS CRÍTICOS (BLOQUEAN TODO)**

```
DOCUMENTO                              GAPS RESUELTOS          DEPENDENCIAS    PRIORIDAD  TIEMPO
═══════════════════════════════════════════════════════════════════════════════════════════════════

1. rm-requirements-baseline.md          RM-009                 None            🔴 P1      2h
   └─ Baseline congelada v1.0.0
   
2. rm-stakeholder-requirements.md       RM-001,004             rm-baseline     🔴 P1      3h
   └─ Elicitar IT Admin, End User, Support, Compliance
   
3. rm-requirements-clarification.md     RM-005,006,007         rm-stakeholder  🔴 P1      3h
   └─ Resolver ambigüedades (8 pasos, idempotence, fail-fast)
   
4. design/data-structures-and-matrices  BA-001                 rm-baseline     🔴 P1      3h
   └─ Whitelist, XSD, matrices language-version-app
   
5. pm-charter.md                        PM-001                 None            🔴 P1      2h
   └─ Autorización formal Stage 7
   
6. pm-scope-statement.md                PM-002                 pm-charter      🔴 P1      2h
   └─ IN/OUT claros, scope creep prevention
   
7. pm-schedule-baseline.md (REPLANNING) PM-003                 pm-scope        🔴 P1      4h
   └─ Estimates realistas (~450 min, no 46)
   
8. architecture/security-architecture   ARQ-003                rm-baseline     🔴 P1      5h
   └─ Threat model, secure coding guidelines
```

**TIER A SUBTOTAL: 8 documentos críticos, 24 horas, NINGUNO puede esperar**

---

#### **TIER B: DOCUMENTOS DE ESPECIFICACIÓN (BLOQUEAN UCs)**

```
DOCUMENTO                              GAPS RESUELTOS          DEPENDENCIAS    PRIORIDAD  TIEMPO
═══════════════════════════════════════════════════════════════════════════════════════════════════

9. design/error-propagation-strategy    BA-002                 pm-scope        🔴 P2      2h
   └─ Qué hacer cuando UC-001,002,003 fallan
   
10. design/non-functional-requirements  RM-002                 rm-baseline     🔴 P2      2h
    └─ Performance, availability, compliance targets
    
11. design/state-management-design      ARQ-001,004            architecture/sec 🔴 P2     3h
    └─ State machine formal, estados, transiciones
    
12. architecture/state-machine-design   ARQ-004                design/state-mgmt 🔴 P2    2h
    └─ Mermaid diagrams, guards, state definitions
    
13. design/logging-specification        BA-003                 pm-scope        🔴 P2      2h
    └─ Qué loguear en cada UC, niveles
    
14. design/ui-ux-requirements           RM-003                 rm-stakeholder  🟠 P3      2h
    └─ Interface type, accessibility, localization
    
15. design/integration-requirements     RM-004                 rm-stakeholder  🟠 P3      2h
    └─ Integración OS, enterprise tools, monitoring
    
16. architecture/disaster-recovery      ARQ-006                design/state-mgmt 🟠 P2    3h
    └─ Checkpoint/restore mechanism
```

**TIER B SUBTOTAL: 8 documentos especificación, 18 horas, ANTES UC design**

---

#### **TIER C: DOCUMENTOS PM/RM/ARQUITECTO (PARALELIZABLE)**

```
DOCUMENTO                              GAPS RESUELTOS          DEPENDENCIAS    PRIORIDAD  TIEMPO
═══════════════════════════════════════════════════════════════════════════════════════════════════

17. pm-cost-estimate.md                 PM-004                 pm-schedule     🔴 P2      2h
    └─ Budget, contingency, cost baseline
    
18. pm-quality-plan.md                  PM-005                 pm-scope        🔴 P2      2h
    └─ Definition of Done, metrics
    
19. pm-resource-plan.md                 PM-006                 pm-scope        🔴 P2      2h
    └─ RACI, asignaciones, feedback loops
    
20. pm-risk-register.md                 PM-008                 pm-schedule     🔴 P2      3h
    └─ 8+ riesgos, mitigations, contingency
    
21. pm-change-control-process.md        PM-009, RM-013         pm-scope        🔴 P2      2h
    └─ CCB, proceso, impact analysis
    
22. pm-communications-plan.md           PM-007                 pm-scope        🟠 P3      2h
    └─ Status reports, escalation, stakeholder updates
    
23. pm-stakeholder-register.md          PM-010                 pm-scope        🟠 P3      2h
    └─ Engagement strategy por stakeholder
    
24. rm-requirements-formal-srs.md       RM-008, RM-010         rm-baseline     🟠 P3      3h
    └─ IEEE 830 SRS con IDs (REQ-NNN)
    
25. rm-requirements-traceability-matrix RM-010, RM-014         rm-srs          🟠 P3      3h
    └─ REQ → UC → Component → Code → Test mapping
    
26. rm-requirements-validation-report   RM-011, RM-012         rm-srs          🟠 P3      3h
    └─ Stakeholder sign-offs, approvals

ARQUITECTO DOCS:
    
27. adr-config-state-model              ARQ-009                None            🟠 P3      1h
    └─ Por qué $Config object vs alternativas
    
28. adr-layered-architecture            ARQ-010                None            🟠 P3      1h
    └─ Por qué 6 capas vs 3-tier, microservices
    
29. architecture/design-patterns        ARQ-005                None            🟠 P3      2h
    └─ Patrones de reutilización, factory, decorator
    
30. architecture/quality-attributes-tro ARQ-007                None            🟠 P3      2h
    └─ Performance vs Security, Reliability vs Complexity trade-offs
    
31. architecture/extensibility-strategy ARQ-011                None            🟠 P3      2h
    └─ Plugin architecture, versioning, future scaling
```

**TIER C SUBTOTAL: 15 documentos PM/RM/ARQ, 40 horas, PARALLELIZABLE**

---

### Summary Documentos: 31 Total (revisión)

```
TIER      DOCUMENTOS    HORAS   CRITICIDAD    DEPENDENCIAS       EJECUTABLE
═════════════════════════════════════════════════════════════════════════════
A         8             24h     CRÍTICA        Lineal (8 steps)   NO (depende todo)
B         8             18h     CRÍTICA        A → B              NO (depende todo)
C         15            40h     MEDIA/ALTA     A,B → C            SÍ (parcial paralelo)

TOTAL     31            82h     MÚLTIPLE       Complex DAG
```

---

## PARTE 3: ROADMAP DE EJECUCIÓN INTEGRADO

### Fase 1: CIMENTACIÓN (TIER A) — 24 horas

```
Objetivo: Establecer baseline, scope, charter, seguridad

SECUENCIA ESTRICTA (NO paralelizable):

HITO 1.1 (2h):
  ✓ rm-requirements-baseline.md
    └─ Congelar requisitos v1.0.0 vs v1.1 roadmap
    
  ✓ pm-charter.md
    └─ Autorización formal, criterios éxito
    
STATUS: Requisitos baseline + Autorización

HITO 1.2 (5h):
  ✓ rm-stakeholder-requirements.md (3h)
    └─ Elicitar IT Admin, End User, Support, Compliance
    
  ✓ pm-scope-statement.md (2h)
    └─ IN/OUT claros, scope creep prevention
    
STATUS: Stakeholders elicitados, Scope definido

HITO 1.3 (6h):
  ✓ rm-requirements-clarification.md (3h)
    └─ Resolver ambigüedades (8 pasos UC-004, idempotence, fail-fast, OCT bug)
    
  ✓ design/data-structures-and-matrices.md (3h)
    └─ Whitelist versiones, XSD, matrices language-version-app
    
STATUS: Ambigüedades resueltas, Data structures claros

HITO 1.4 (11h):
  ✓ pm-schedule-baseline.md (4h)
    └─ REPLANNING: Estimates realistas (~450 min/stage, no 46)
    └─ Cadena crítica, caminos paralelos
    
  ✓ architecture/security-architecture.md (5h)
    └─ Threat model (STRIDE), secure coding, credential handling, audit
    
  ✓ pm-scope-statement.md UPDATED (2h)
    └─ Refinar con security requirements
    
STATUS: Timeline realista, Security architected

CHECKPOINT 1: ¿Nestor APRUEBA Baseline + Charter + Scope + Security?
  SI → Proceder a Fase 2
  NO → Iterar Fase 1

TIEMPO FASE 1: ~24 horas
```

---

### Fase 2: ESPECIFICACIÓN DETALLADA (TIER B) — 18 horas

```
Objetivo: Especificar diseño UCs, estado, logging, recuperación

SECUENCIA (Parcialmente paralelizable después de Fase 1):

HITO 2.1 (7h):
  ✓ design/error-propagation-strategy.md (2h)
    └─ Qué pasa cuando UC-001,002,003 fallan
    └─ Transiciones de estado en error
    
  ✓ design/state-management-design.md (3h)
    └─ State machine formal, estados válidos, transiciones
    
  ✓ architecture/state-machine-design.md (2h)
    └─ Mermaid diagrams, guards, state definitions
    
STATUS: State management formal + error handling

HITO 2.2 (5h):
  ✓ design/logging-specification.md (2h)
    └─ Qué loguear en cada UC, niveles, sensibilidad
    
  ✓ design/non-functional-requirements.md (2h)
    └─ Performance targets, availability, compliance
    
  ✓ architecture/disaster-recovery.md (3h)
    └─ Checkpoint/restore mechanism, rollback strategy
    
STATUS: Logging + NFRs + Disaster recovery

HITO 2.3 (3h):
  ✓ design/ui-ux-requirements.md (2h)
    └─ Interface type (CLI/GUI), accessibility, localization
    
  ✓ design/integration-requirements.md (2h)
    └─ Integración con OS, enterprise tools, monitoring
    
STATUS: UI/UX + Integration requirements

CHECKPOINT 2: ¿Especificaciones están COMPLETAS y CONSISTENTES?
  SI → Proceder a Fase 3
  NO → Re-iterar secciones

TIEMPO FASE 2: ~18 horas
PUEDE EMPEZAR: Después HITO 1.4 (Fase 1 completa)
```

---

### Fase 3: DOCUMENTACIÓN FORMAL (TIER C) — 40 horas

```
Objetivo: Formalizar requisitos, ADRs, plans PM/RM

PARALLELIZABLE EN 3 STREAMS:

STREAM C1: FORMAL REQUIREMENTS MANAGEMENT (10h)
  ✓ rm-requirements-formal-srs.md (3h)
    └─ IEEE 830 SRS con IDs (REQ-001, REQ-002, ...)
    
  ✓ rm-requirements-traceability-matrix.md (3h)
    └─ REQ → UC → Component → Code → Test mapping
    
  ✓ rm-requirements-validation-report.md (3h)
    └─ Stakeholder sign-offs, approvals
    
  ✓ pm-change-control-process.md (1h)
    └─ Incluida aquí ya que RM+PM combined

STREAM C2: PROJECT MANAGEMENT PLANS (12h)
  ✓ pm-cost-estimate.md (2h)
    └─ Budget, contingency, cost baseline
    
  ✓ pm-quality-plan.md (2h)
    └─ Definition of Done, metrics, DoD checklist
    
  ✓ pm-resource-plan.md (2h)
    └─ RACI matrix, asignaciones, feedback loops
    
  ✓ pm-risk-register.md (3h)
    └─ 8+ riesgos identificados, mitigations
    
  ✓ pm-communications-plan.md (2h)
    └─ Status reports, escalation, cadence
    
  ✓ pm-stakeholder-register.md (1h)
    └─ Engagement strategy

STREAM C3: ARCHITECTURAL DECISIONS & PATTERNS (10h)
  ✓ adr-config-state-model.md (1h)
    └─ Por qué $Config object vs DB, file
    
  ✓ adr-layered-architecture.md (1h)
    └─ Por qué 6 capas vs 3-tier, microservices
    
  ✓ adr-idempotence-scope.md (1h)
    └─ Scope: UC-005 only vs entire pipeline
    
  ✓ architecture/design-patterns.md (2h)
    └─ Pipeline, Validation Layer, Retry, State Machine patterns
    
  ✓ architecture/quality-attributes-tradeoffs.md (2h)
    └─ Performance vs Security, Reliability vs Complexity
    
  ✓ architecture/extensibility-strategy.md (2h)
    └─ Plugin architecture, versioning, future scaling

EJECUTAR EN PARALELO:
  STREAM C1 (10h) en paralelo con STREAM C2 (12h) en paralelo con STREAM C3 (10h)
  
TIEMPO CRÍTICO: MAX(10, 12, 10) = 12 horas

CHECKPOINT 3: ¿Todos los documentos REVISADOS y VALIDADOS?
  SI → Fase 4 (creación de UCs)
  NO → Iterar documentos

TIEMPO FASE 3: ~12 horas (paralelizado)
PUEDE EMPEZAR: Después HITO 1.4
```

---

## PARTE 4: TIMELINE REALISTA INTEGRADO

### Timeline Comparativo: PLAN v3 vs PLAN MAESTRO

```
PLAN v3 ORIGINAL:
  ├─ TIER 0: UC-Matrix (5 min)
  ├─ TIER 1: overall-architecture (8 min)
  ├─ TIER 2: UCs + ADRs (40 min)
  └─ TIER 3-4: Closing (10 min)
  
  TOTAL: ~46 minutos
  
REALIDAD (PM perspective):
  └─ ~450 minutos (7.5 horas)
  
PLAN MAESTRO INTEGRADO:
  └─ Fase 1 (TIER A): 24 horas
  └─ Fase 2 (TIER B): 18 horas
  └─ Fase 3 (TIER C): 12 horas (paralelo)
  └─ Fase 4 (TIER D - UCs): ~40 horas (con paralelización)
  
  TOTAL STAGE 7: ~90-95 horas (11-12 días de trabajo full-time)
  
  TIMELINE CON BUFFER 15%: ~110 horas (13-14 días)
```

### Cadena Crítica (Critical Path)

```
PASO 1: Fase 1 (24h)
  └─ rm-baseline → rm-stakeholder → rm-clarification 
     → data-structures → pm-charter → pm-scope 
     → pm-schedule → security-architecture
  
  BLOQUEADOR: Security-architecture (5h, último en fase 1)
  
PASO 2: Fase 2 (18h)
  └─ error-propagation → state-management 
     → state-machine-design
     → logging-spec + nfr-spec + disaster-recovery
     → ui-ux + integration
  
  BLOQUEADOR: disaster-recovery (3h, último en fase 2)
  
PASO 3: Fase 3 (12h paralelo, 12h crítico)
  └─ rm-srs → traceability-matrix → validation-report (RM stream)
  └─ pm-cost → pm-quality → pm-resource → pm-risk (PM stream)
  └─ ADRs x5 + design-patterns + quality-tradeoffs (ARQ stream)
  
  BLOQUEADOR: pm-risk-register (3h) en PM stream
  
PASO 4: Fase 4 - UC DESIGN (pendiente, fuera de este plan)
  ├─ overall-architecture.md
  ├─ UC-001, UC-002, UC-003 (paralelo)
  ├─ UC-004 (crítico, más largo)
  └─ UC-005
  
  DEPENDENCIAS: Todos los documentos de Fase 1-3 DEBEN estar listos

CADENA CRÍTICA TOTAL: 24 + 18 + 12 = 54 horas secuencial
```

---

## PARTE 5: PRIORIZACIÓN MUST DO vs NICE TO HAVE

### MUST DO — BLOQUEAN Stage 10 (16 documentos, 62 horas)

```
CRÍTICA (Bloquean ABSOLUTAMENTE):
  ✓ rm-requirements-baseline.md (2h) - Qué es v1.0.0
  ✓ rm-stakeholder-requirements.md (3h) - Elicitar todos
  ✓ rm-requirements-clarification.md (3h) - Resolver ambigüedades críticas
  ✓ design/data-structures-and-matrices.md (3h) - Dónde saca datos UC-004
  ✓ pm-charter.md (2h) - Autorización
  ✓ pm-scope-statement.md (2h) - IN/OUT claro
  ✓ pm-schedule-baseline.md (4h) - Timeline realista
  ✓ architecture/security-architecture.md (5h) - Threat model, vulnerabilidades
  ✓ design/error-propagation-strategy.md (2h) - Qué pasa cuando falla
  ✓ design/state-management-design.md (3h) - State machine formal
  ✓ architecture/state-machine-design.md (2h) - Mermaid, guards
  ✓ design/logging-specification.md (2h) - Qué loguear
  ✓ pm-cost-estimate.md (2h) - Budget
  ✓ pm-quality-plan.md (2h) - Definition of Done
  ✓ pm-risk-register.md (3h) - Riesgos + mitigations
  ✓ pm-change-control-process.md (2h) - Proceso de cambios

SUBTOTAL MUST DO: 42 horas (crítica, sin estos Stage 10 falla)

ALTA PRIORIDAD (Muy necesarios):
  ✓ pm-resource-plan.md (2h) - Asignaciones
  ✓ rm-requirements-formal-srs.md (3h) - IEEE 830
  ✓ rm-requirements-traceability-matrix.md (3h) - Trazabilidad
  ✓ architecture/disaster-recovery.md (3h) - Rollback/recovery
  ✓ design/non-functional-requirements.md (2h) - Performance, availability
  ✓ adr-idempotence-scope.md (1h) - Qué es idempotencia
  ✓ adr-layered-architecture.md (1h) - Por qué 6 capas

SUBTOTAL ALTA: 15 horas (necesarios para calidad)

TOTAL MUST DO: 57 horas (Fase 1 + Fase 2 + parte de Fase 3)
```

### NICE TO HAVE — Mejora calidad pero pueden esperar (14 documentos, 25 horas)

```
MEDIA PRIORIDAD (Si hay tiempo, mejora significativa):
  ~ design/ui-ux-requirements.md (2h) - UI puede diseñarse en Stage 9
  ~ design/integration-requirements.md (2h) - Integration puede ser Stage 8
  ~ pm-communications-plan.md (2h) - Status reports, useful pero optional
  ~ rm-requirements-validation-report.md (3h) - Sign-offs, puede hacerse iterativo
  ~ architecture/quality-attributes-tradeoffs.md (2h) - Trade-offs, referencia
  ~ architecture/design-patterns.md (2h) - Patrones, referencia

BAJA PRIORIDAD (Nice to have, pueden esperar a iteraciones futuras):
  ~ pm-stakeholder-register.md (1h) - Engagement, puede ser simple
  ~ pm-communications-plan.md (2h) - Already listed
  ~ adr-config-state-model.md (1h) - Why $Config, justificación
  ~ adr-layered-architecture.md (1h) - Why 6 layers, justificación
  ~ rm-requirements-validation-report.md (3h) - Already listed
  ~ architecture/extensibility-strategy.md (2h) - Future scaling, v1.1
  ~ architecture/design-patterns.md (2h) - Already listed

SUBTOTAL NICE TO HAVE: ~25 horas (Fase 3 completa, Stream C2 y C3 parcial)
```

### Priorización Visual

```
TIEMPO    CRITICIDAD    CATEGORÍA
─────────────────────────────────────────────────────
0-24h     🔴 CRÍTICA    TIER A (Cimentación - MUST DO)
24-42h    🔴 CRÍTICA    TIER B early (Especificación - MUST DO)
42-57h    🟠 ALTA       TIER B late + Fase 3 early (MUST DO)
57-90h    🟡 MEDIA      TIER C late + Fase 3 late (NICE TO HAVE)
90-110h   🟢 BAJA       Iteraciones, refinamiento (FUTURE)
```

---

## PARTE 6: CHECKPOINTS DE INTEGRACIÓN

### 6 Checkpoints Majeures

#### **CHECKPOINT 0: Pre-Phase 1 (ANTES de empezar)**

```
Verificar:
  ✓ PLAN v3 (arquitectónico) aprobado
  ✓ 4 Perspectivas (BA, PM, RM, ARQUITECTO) revisadas
  ✓ 37 gaps identificados y categorizados
  ✓ Nestor comprende PLAN MAESTRO (90+ horas, no 46 min)
  ✓ Timeline Stage 7: ~11-14 días (full-time)
  
GATE: ¿Proceder a Fase 1?
  SI → Comenzar Fase 1
  NO → Re-iterar, ajustar, re-priorizar
```

#### **CHECKPOINT 1: Fin Fase 1 (24 horas después)**

```
Entregas:
  ✓ rm-requirements-baseline.md
  ✓ rm-stakeholder-requirements.md
  ✓ rm-requirements-clarification.md
  ✓ design/data-structures-and-matrices.md
  ✓ pm-charter.md
  ✓ pm-scope-statement.md
  ✓ pm-schedule-baseline.md
  ✓ architecture/security-architecture.md

Validación:
  ✓ Baseline aprobado por Nestor
  ✓ Scope IN/OUT claro (sin ambigüedades)
  ✓ Security architected (threat model done)
  ✓ Requisitos elicitados (todos stakeholders)
  ✓ Ambigüedades resueltas (8 pasos UC-004, fail-fast, idempotence)
  ✓ Data structures definidas (whitelist, XSD, matrices)

GATE: ¿Proceder a Fase 2?
  SI → Comenzar Fase 2
  NO → Fix issues, re-iterar Fase 1
```

#### **CHECKPOINT 2: Fin Fase 2 (42 horas después de CP1)**

```
Entregas:
  ✓ design/error-propagation-strategy.md
  ✓ design/state-management-design.md
  ✓ architecture/state-machine-design.md
  ✓ design/logging-specification.md
  ✓ design/non-functional-requirements.md
  ✓ architecture/disaster-recovery.md
  ✓ design/ui-ux-requirements.md (si tiempo)
  ✓ design/integration-requirements.md (si tiempo)

Validación:
  ✓ UC design puede comenzar (todos prereqs listos)
  ✓ State management formal (máquina de estado)
  ✓ Error handling claro (transiciones en error)
  ✓ Logging strategy completa
  ✓ Disaster recovery documented (rollback, checkpoint)

GATE: ¿Proceder a Fase 3 + Fase 4 (UC design)?
  SI → Comenzar Fase 3 (paralelo) + Fase 4 (secuencial)
  NO → Fix issues, re-iterar Fase 2
```

#### **CHECKPOINT 3: Fin Fase 3 (54 horas después de CP0)**

```
Entregas:
  ✓ STREAM C1: rm-requirements-formal-srs, traceability-matrix, validation-report
  ✓ STREAM C2: pm-cost, pm-quality, pm-resource, pm-risk, pm-change-control
  ✓ STREAM C3: 5 ADRs + architecture docs

Validación:
  ✓ Requisitos formalizados (IEEE 830, REQ-NNN IDs)
  ✓ Trazabilidad establecida (REQ → UC → Code → Test)
  ✓ PM plans completos (cost, quality, risk, resource)
  ✓ Decisiones arquitectónicas justificadas (5 ADRs)

GATE: ¿Proceder a Fase 4 (UC design)?
  SI → Comenzar UC design (overall-arch + UC-001-005)
  NO → Fix issues, minor re-iteration
```

#### **CHECKPOINT 4: Fin Fase 4 UC DESIGN (pendiente, fuera plan actual)**

```
(Será el próximo PLAN, después PLAN MAESTRO INTEGRADO)

Esperado: overall-architecture + UC-001 a UC-005 diseños completos

GATE: ¿Ready para Stage 10 IMPLEMENT?
```

#### **CHECKPOINT 5: FINAL Stage 7 (después CP4)**

```
Validación Final:
  ✓ Todos los gaps (37) RESUELTOS o DOCUMENTADOS como RESOLVED-AS-NA
  ✓ Todos los documentos (26 MUST DO + 14 NICE TO HAVE) REVISADOS
  ✓ Compliance audit pasado (convenciones, patrones)
  ✓ Exit criteria de Stage 7 COMPLETADAS

GATE: ¿Ready Stage 10 IMPLEMENT?
  SI → Proceed to Stage 10
  NO → Final refinements
```

---

## PARTE 7: MATRIZ GAPS → RESOLUCIÓN

### Tabla: Cada Gap → Documento que lo resuelve → Status

```
GAP ID        DESCRIPCIÓN                           DOCUMENTO QUE RESUELVE          STATUS
════════════════════════════════════════════════════════════════════════════════════════════════
GA-BA-001     Data structures externos              design/data-structures-matrix   NEW (Fase 1)
GA-BA-002     Error propagation                     design/error-propagation        NEW (Fase 2)
GA-BA-003     Logging strategy                      design/logging-specification    NEW (Fase 2)
GA-BA-004     Idempotence scope                     adr-idempotence-scope           NEW (Fase 3)
GA-BA-005     Microsoft OCT mitigation              EXPAND design/uc-004            EXPAND (Fase 4)

GA-PM-001     Charter                              pm-charter.md                   NEW (Fase 1)
GA-PM-002     Scope statement                      pm-scope-statement.md           NEW (Fase 1)
GA-PM-003     Timeline 88% error                   pm-schedule-baseline.md         REPLACE (Fase 1)
GA-PM-004     Cost estimate                        pm-cost-estimate.md             NEW (Fase 3)
GA-PM-005     Definition of Done                   pm-quality-plan.md              NEW (Fase 3)
GA-PM-006     Resource assignments                 pm-resource-plan.md             NEW (Fase 3)
GA-PM-007     Communications plan                  pm-communications-plan.md       NEW (Fase 3)
GA-PM-008     Risk register                        pm-risk-register.md             NEW (Fase 3)
GA-PM-009     Change control                       pm-change-control.md            NEW (Fase 1-3)
GA-PM-010     Stakeholder engagement               pm-stakeholder-register.md      NEW (Fase 3)

GA-RM-001     Stakeholders NOT elicited            rm-stakeholder-requirements     NEW (Fase 1)
GA-RM-002     Non-Functional Requirements          design/non-functional-req       NEW (Fase 2)
GA-RM-003     UI/UX requirements                   design/ui-ux-requirements       NEW (Fase 2)
GA-RM-004     Integration requirements            design/integration-requirements NEW (Fase 2)
GA-RM-005     8 pasos ambiguos                     rm-requirements-clarification   NEW (Fase 1)
GA-RM-006     Fail-Fast vs Repair conflict         rm-conflict-resolution          NEW (Fase 1)
GA-RM-007     Req vs Design confusion              rm-requirements-vs-design       NEW (Fase 1)
GA-RM-008     Spec not formal (no IEEE 830)       rm-requirements-formal-srs      NEW (Fase 3)
GA-RM-009     Baseline NOT frozen                  rm-requirements-baseline        NEW (Fase 1)
GA-RM-010     Traceability matrix missing          rm-requirements-traceability    NEW (Fase 3)
GA-RM-011     Validation NOT formal                rm-requirements-validation      NEW (Fase 3)
GA-RM-012     Dev validation missing               PART OF rm-validation-report    PART-OF (Fase 3)
GA-RM-013     Change control missing               pm-change-control (SHARED)      SHARED (Fase 1-3)
GA-RM-014     Traceability weak                    rm-requirements-traceability    SAME-AS-010

GA-ARQ-001    $Config state management weak        design/state-management-design  NEW (Fase 2)
GA-ARQ-002    Idempotence scope unclear            adr-idempotence-scope          SAME-AS-BA-004
GA-ARQ-003    Security NOT architected             architecture/security-arch      NEW (Fase 1)
GA-ARQ-004    State machine NOT formal             architecture/state-machine      NEW (Fase 2)
GA-ARQ-005    Reuse pattern not defined            architecture/design-patterns    NEW (Fase 3)
GA-ARQ-006    Disaster recovery absent             architecture/disaster-recovery  NEW (Fase 2)
GA-ARQ-007    NFR trade-offs not documented        architecture/quality-tradeoffs  NEW (Fase 3)
GA-ARQ-008    $Config coupling potential           MITIGATED BY GA-ARQ-001        PART-OF-001
GA-ARQ-009    ADR for $Config missing              adr-config-state-model         NEW (Fase 3)
GA-ARQ-010    ADR for 6-layer missing              adr-layered-architecture       NEW (Fase 3)
GA-ARQ-011    Scalability not considered           architecture/extensibility      NEW (Fase 3)

SUMMARY:
  ✓ Todos los 37 gaps tienen RESOLUCIÓN documentada
  ✓ 26 documentos ÚNICOS los resuelven
  ✓ Algunos gaps se resuelven en MISMO documento (deduped)
  ✓ Algunos gaps marcados como N/A (EXPAND, PART-OF, SHARED)
```

---

## PARTE 8: CONSIDERACIONES N/A

### Gaps que pueden ser N/A (No Aplicable)

```
SITUACIÓN 1: GAPS RESUELTOS POR OTRO DOCUMENTO (Dedup)
  GA-ARQ-002 (Idempotence scope) = GA-BA-004 ✓
    └─ Una sola decisión arquitectónica, no duplicar

  GA-RM-010 (Traceability matrix) ≈ GA-RM-014 (Traceability weak) ✓
    └─ Mismo documento resuelve ambos

  GA-PM-009 (Change control) ≈ GA-RM-013 (Change control) ✓
    └─ PM y RM comparten change control process

SITUACIÓN 2: GAPS MITIGADOS POR OTRO (Mitigation)
  GA-ARQ-008 ($Config coupling) → MITIGADO BY GA-ARQ-001 (State management)
    └─ State management formal previene coupling
    └─ NO necesita documento separado

SITUACIÓN 3: GAPS RESUELTOS EN OTRO STAGE (Defer)
  GA-BA-005 (Microsoft OCT bug mitigation detail) → EXPAND IN uc-004
    └─ No es documento nuevo, es expansión de UC-004 (Fase 4)
    └─ Documentado como "EXPAND", no NEW

SITUACIÓN 4: GAPS PART-OF OTRO DOCUMENTO
  GA-RM-012 (Dev validation) → PART-OF rm-validation-report
    └─ Incluido en stakeholder validation, no doc separado

RESUMEN N/A:
  ✓ 2 gaps son SAME-AS (dedup)
  ✓ 1 gap es MITIGATED-BY (no doc extra)
  ✓ 1 gap es EXPAND (no doc extra)
  ✓ 1 gap es PART-OF (incluido)
  ✓ 1 gap es SHARED (con PM)

EFECTO:
  Original: 37 gaps → 31 documentos
  Después dedup: 37 gaps → 26 documentos ÚNICOS
```

---

## PARTE 9: RESUMEN EJECUTIVO

### PLAN MAESTRO INTEGRADO — Summary

```
OBJETIVO:
  Ejecutar Stage 7 DESIGN/SPECIFY exitosamente considerando 4 perspectivas
  (BA, PM, RM, ARQUITECTO), resolviendo todos los gaps.

ENFOQUE:
  4 FASES secuenciales + parallelizable:
    Fase 1 (24h): Cimentación (baseline, charter, scope, security)
    Fase 2 (18h): Especificación (state, logging, error handling, disaster recovery)
    Fase 3 (12h): Documentación formal (3 streams paralelos, PM/RM/ARQ)
    Fase 4 (~40h): UC Design (pendiente, separate plan)

RESULTADO:
  ✓ 37 gaps identificados
  ✓ 26 documentos únicos que los resuelven
  ✓ 82 horas totales Stage 7
  ✓ 16 documentos MUST DO (62h, no negociables)
  ✓ 10 documentos NICE TO HAVE (20h, optimización)
  ✓ 6 checkpoints de integración
  ✓ Cadena crítica clara (54h secuencial)

TIMELINE:
  Sin PLAN MAESTRO: ~46 minutos (irreal)
  Con optimización PM: ~450 minutos (7.5h)
  PLAN MAESTRO REALISTA: ~82 horas (10-12 días full-time)
  Con buffer 15%: ~95 horas (12-14 días)

NEXT STEP:
  ✓ CHECKPOINT 0: Nestor aprueba PLAN MAESTRO INTEGRADO
  ✓ COMIENZA Fase 1: 24 horas de documentación cimentación
```

---

## APÉNDICE: MATRIZ MAESTRO (TBL COMPLETA)

**Ver tabla completa en PARTE 2 y PARTE 7**

---

**PLAN MAESTRO INTEGRADO completado:** 2026-04-21 07:15:00
**Perspectivas integradas:** BA (5 gaps), PM (10 gaps), RM (14 gaps), ARQUITECTO (11 gaps)
**Gaps consolidados:** 37 → 26 documentos únicos
**Timeline total Stage 7:** 82 horas (10-12 días)
**MUST DO documentos:** 16 (62 horas, bloquean Stage 10)
**NICE TO HAVE documentos:** 10 (20 horas, optimización)
**Status:** LISTO para CHECKPOINT 0 (aprobación Nestor)

