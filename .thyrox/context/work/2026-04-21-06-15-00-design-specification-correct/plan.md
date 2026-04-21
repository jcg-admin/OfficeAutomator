```yml
created_at: 2026-04-21 06:15:00
project: THYROX
work_package: 2026-04-21-06-15-00-design-specification-correct
phase: Phase 7 — DESIGN/SPECIFY
author: Claude
status: Borrador
version: 1.0.0
updated_at: 2026-04-21 06:15:00
```

# PLAN: Stage 7 DESIGN/SPECIFY (CORRECTO)

## Requisito Previo: DOCUMENTO-MAESTRO-requisitos.md

Este plan se basa en:
- **9 convenciones** (2650 líneas verificadas)
- **10 patrones clave** del proyecto
- **Estructura thyrox-compliant** (stage directories)
- **Metadata estándar** por tipo de documento
- **Protocolos de validación** pre/post creación

Ver: `./DOCUMENTO-MAESTRO-requisitos.md` para referencia completa

---

## Objetivo

Crear diseño técnico completo para 5 Casos de Uso (UC-001 a UC-005) de OfficeAutomator, aplicando **100% de convenciones y patrones**, en nuevo WP con nomenclatura correcta, **archivo por archivo** con confirmación en cada paso.

---

## Estructura de WP (thyrox-compliant)

```
2026-04-21-06-15-00-design-specification-correct/
├── design/                          ← Stage 7 design artifacts
│   ├── overall-architecture.md
│   ├── uc-001-select-version.md
│   ├── uc-002-select-language.md
│   ├── uc-003-exclude-applications.md
│   ├── uc-004-validate-configuration.md
│   └── uc-005-install-office.md
│
├── analyze/                         ← Analysis & compliance audit
│   └── stage-7-convenciones-compliance-audit.md
│
├── decisions/                       ← Architectural Decision Records
│   ├── adr-uc-004-8-step-validation.md
│   ├── adr-fail-fast-principle.md
│   ├── adr-idempotence-approach-uc-005.md
│   ├── adr-phase-parallel-vs-sequential.md
│   └── adr-retry-strategy-exponential-backoff.md
│
├── track/                           ← Closure & criteria
│   └── stage-7-exit-criteria.md
│
├── plan-execution/                  ← Task breakdown
│   └── stage-7-task-plan.md
│
├── DOCUMENTO-MAESTRO-requisitos.md  ← Reference guide (copied)
├── plan.md                          ← This file
└── README.md                        ← Directorio description

```

---

## Artefactos a Crear (15 total)

### TIER 1: ARQUITECTURA (3 documentos)

#### 1. design/overall-architecture.md
**Metadata:**
```yml
created_at: 2026-04-21 HH:MM:SS
project: THYROX
work_package: 2026-04-21-06-15-00-design-specification-correct
phase: Phase 7 — DESIGN/SPECIFY
author: Claude
status: Borrador
version: 1.0.0
```

**Requerimientos:**
- Arquitectura de 6 capas (UI → Orchestration → Config → Validation → Execution → Utility)
- Flujo de datos end-to-end
- 3 Principios Core: Reliability, Transparency, Idempotence
- Diagrama Mermaid (dark theme, NO emojis)
- Módulo structure (Public/Private/Logging)
- State management object

**Validación pre-creación:**
- [x] Nombre kebab-case: `overall-architecture.md` ✓
- [x] Ubicación: `design/` ✓
- [x] Metadata: bloque yml ✓
- [x] Referencias a Stage 6, Stage 1 ✓

**Validación post-creación:**
- [ ] Estructura estándar aplicada
- [ ] Diagrama Mermaid con dark theme
- [ ] Sin emojis decorativos
- [ ] Referencias explicitas a scope-statement.md (Stage 6)

**Estimado:** ~400 líneas, 1 diagrama

---

#### 2. analyze/stage-7-convenciones-compliance-audit.md
**Metadata:**
```yml
created_at: 2026-04-21 HH:MM:SS
project: THYROX
work_package: 2026-04-21-06-15-00-design-specification-correct
phase: Phase 7 — DESIGN/SPECIFY
author: Claude
status: Borrador
```

**Requerimientos:**
- Tabla: Convención vs Aplicación vs Status
- Audit de todas las 9 convenciones
- Audit de 10 patrones clave
- Pre-checklist de validación
- Post-checklist de validación

**Validación pre-creación:**
- [x] Ubicación: `analyze/` ✓
- [x] Metadata: bloque yml ✓
- [x] Nombre descriptivo ✓

**Estimado:** ~300 líneas, 2 tablas

---

#### 3. plan-execution/stage-7-task-plan.md
**Metadata:**
```yml
created_at: 2026-04-21 HH:MM:SS
project: THYROX
work_package: 2026-04-21-06-15-00-design-specification-correct
phase: Phase 7 — DESIGN/SPECIFY
author: Claude
status: Borrador
```

**Requerimientos:**
- Desglose de tareas T-001 a T-020
- Cada tarea con descripción
- Timestamps estimados
- Dependencias entre tareas
- Protocolos de validación

**Validación pre-creación:**
- [x] Ubicación: `plan-execution/` ✓
- [x] Nombre: `{nombre}-task-plan.md` ✓
- [x] Metadata: bloque yml ✓

**Estimado:** ~250 líneas, T-NNN structure

---

### TIER 2: UCs (5 documentos) ⭐ CORE

#### 4. design/uc-001-select-version.md
**Metadata:**
```yml
created_at: 2026-04-21 HH:MM:SS
project: THYROX
work_package: 2026-04-21-06-15-00-design-specification-correct
phase: Phase 7 — DESIGN/SPECIFY
uc_id: UC-001
version: 1.0.0
author: Claude
status: Borrador
dependencies:
  - UC-002
  - UC-003
  - UC-004
  - UC-005
  - Stage 6: scope-statement.md
  - Stage 1: use-case-matrix.md
constraints:
  - Version selection only (no custom)
  - Max 3 versions (2024, 2021, 2019)
  - Interactive menu only
```

**Secciones obligatorias:**
1. Overview (descripción, complejidad, criticidad)
2. Scope (IN/OUT OF SCOPE, Roadmap v1.1)
3. Actors and Preconditions
4. Main Flow (pasos en prosa)
5. Technical Design (function signature PowerShell)
6. Error Scenarios (mínimo 2)
7. Validation Rules (3+ reglas)
8. Integration Points (Depends On, Used By, Logs)
9. Exit Criteria (checklist)
10. Testing Strategy (unit + integration)
11. References (Stage 6, Stage 1, overall-architecture)

**Validación pre-creación:**
- [x] Nombre: `uc-001-select-version.md` ✓
- [x] Ubicación: `design/` ✓
- [x] Metadata: uc_id, dependencies, constraints ✓

**Estimado:** ~250 líneas

---

#### 5. design/uc-002-select-language.md
**Similar a UC-001 con:**
- Additional: Language auto-detection
- Additional: Multi-language support (max 2)
- Additional: Compatibility matrix reference
- Additional: Future Extensions (v1.1: 6 más idiomas)

**Estimado:** ~280 líneas

---

#### 6. design/uc-003-exclude-applications.md
**Similar a UC-001 con:**
- Tabla: Applications por versión
- Tabla: Defaults por versión
- Sección: UI checkbox design
- Sección: State management

**Estimado:** ~320 líneas

---

#### 7. design/uc-004-validate-configuration.md ⭐ CRÍTICO
**Especial: 8-step validation architecture**

**Metadata:** (igual estructura)

**Secciones adicionales:**
- Validation Structure (3 fases)
- Phase 1: Parallel (steps 1, 2, 5)
- Phase 2: Sequential (steps 3→4→6)
- Phase 3: Retry (step 7 + 8)
- Step-by-step Specifications (8 pasos detallados)
- Error Categories: [BLOQUEADOR], [CRITICO], [RECUPERABLE]
- State Management (ValidationPassed flag)
- Logging Strategy

**Referencias a números calibrados:**
- SHA256 retry: 3 intentos (citado de calibration-verified-numbers.md)
- Backoff: exponencial 2s, 4s, 6s
- Timeouts: 10 min network, 5 min steps

**Estimado:** ~450 líneas, 2 diagramas

---

#### 8. design/uc-005-install-office.md
**Similar a UC-001 con:**
- 3 Modes: Fresh Install, Verify, Repair/Update
- Idempotence Implementation
- Pre-installation Checks (admin, disk, network)
- Progress Monitoring
- Post-installation Verification
- Cleanup Strategy

**Estimado:** ~350 líneas

---

### TIER 3: DECISIONES (5 ADRs)

#### 9-13. decisions/adr-*.md (5 archivos)

**ADR Metadata estándar:**
```yml
type: Architectural Decision Record
category: Diseño
version: 1.0.0
purpose: [Decision purpose]
updated_at: 2026-04-21 HH:MM:SS
```

**5 ADRs requeridos:**

1. **adr-uc-004-8-step-validation.md**
   - Por qué 8 pasos (no 4, no 6)
   - Por qué 3 fases (paralela, secuencial, retry)
   - Justificación: Performance vs Accuracy tradeoff

2. **adr-fail-fast-principle.md**
   - Nunca instalar con configuración inválida
   - Validación ANTES de acción
   - Justificación: Reliability + Transparency

3. **adr-idempotence-approach-uc-005.md**
   - Running 2x = Running 1x
   - Verificación de instalación existente
   - Repair vs Fresh Install logic
   - Justificación: Idempotence principle

4. **adr-phase-parallel-vs-sequential.md**
   - Parallelizar donde posible (performance)
   - Secuenciar donde necesario (accuracy)
   - Decisión en UC-004
   - Justificación: Balance performance/correctness

5. **adr-retry-strategy-exponential-backoff.md**
   - 3 intentos máximo
   - Backoff: 2s, 4s, 6s
   - Solo para transient errors (network)
   - Justificación: Recover from temporary issues

**Ubicación:** `decisions/`

**Estimado:** ~100 líneas cada una

---

### TIER 4: CLOSURE (2 documentos)

#### 14. track/stage-7-exit-criteria.md
**Metadata:** (estándar)

**Requerimientos:**
- Checklist de completitud Stage 7
- Validación de cada UC
- Validación de convenciones
- Validación de ADRs
- Gate criteria para Stage 10

**Estimado:** ~200 líneas

---

#### 15. README.md (raíz WP)
**Metadata:** N/A (README sin yml)

**Requerimientos:**
- Descripción de WP
- Índice de documentos
- Cómo usar DOCUMENTO-MAESTRO-requisitos.md
- Links a Stage 1, Stage 6
- Status de completitud

**Estimado:** ~100 líneas

---

## Timeline (con esperas de confirmación)

| Fase | Duración | Documentos | Checkpoint |
|------|----------|-----------|------------|
| **PRE** | 0 min | DOCUMENTO-MAESTRO (copiad) | [INICIO] |
| **TIER 1** | 10 min | overall-architecture, audit, task-plan | [ESPERA CONFIRMACIÓN T1] |
| **TIER 2.1** | 8 min | UC-001 + UC-002 | [ESPERA CONFIRMACIÓN T2.1] |
| **TIER 2.2** | 10 min | UC-003 + UC-004 + UC-005 | [ESPERA CONFIRMACIÓN T2.2] |
| **TIER 3** | 10 min | 5 ADRs | [ESPERA CONFIRMACIÓN T3] |
| **TIER 4** | 5 min | exit-criteria + README | [ESPERA CONFIRMACIÓN T4] |
| **VALIDACIÓN** | 5 min | Verificar 100% convenciones | [ESPERA CONFIRMACIÓN FINAL] |
| **GIT COMMIT** | 2 min | Commit con message correcto | [DONE] |

**Total:** ~50 minutos ejecución + 6 esperas de confirmación

---

## Protocolos de Validación

### PRE-CREACIÓN (para CADA archivo)

- [ ] Nombre en kebab-case, sin prefijos numéricos
- [ ] Metadata: bloque yml (NO ---)
- [ ] Stage directory correcto (design/, analyze/, decisions/, track/)
- [ ] Todos los campos requeridos en yml
- [ ] Estructura estándar identificada
- [ ] Referencias a Stage 6, Stage 1 planeadas
- [ ] Números calibrados identificados
- [ ] Dependencias mapeadas

### POST-CREACIÓN (para CADA archivo)

- [ ] Metadata: bloque yml completo
- [ ] Nombre: kebab-case sin prefijos
- [ ] Ubicación: directorio correcto
- [ ] Estructura estándar: 100% aplicada
- [ ] Secciones obligatorias: todas presentes
- [ ] References section: con links a Stage 6, Stage 1, overall-architecture
- [ ] Scope IN/OUT (si UC): presente
- [ ] Exit Criteria (si UC): presente y completo
- [ ] Testing Strategy (si UC): presente
- [ ] NO emojis decorativos
- [ ] Números con source citado
- [ ] Sin SPECULATIVE claims
- [ ] Diagramas Mermaid: dark theme
- [ ] PowerShell code: sintaxis correcta

---

## Checklist Final Pre-Ejecución

- [x] DOCUMENTO-MAESTRO-requisitos.md copiado al WP
- [x] WP nomenclatura correcta: YYYY-MM-DD-HH-MM-SS-nombre
- [x] Stage directories definidos (design/, analyze/, decisions/, track/, plan-execution/)
- [x] Metadata estándar por tipo de documento
- [x] Artefactos listados (15 total)
- [x] Timeline claro (6 esperas)
- [x] Protocolos pre/post definidos
- [x] Referencias a Stage 1, Stage 6 planeadas
- [x] Números calibrados conocidos
- [x] ADR structure claro

**LISTO PARA CREAR TIER 1 (overall-architecture.md)**

---

## Próximo Paso

**Esperando confirmación para crear TIER 1:**
1. `design/overall-architecture.md`
2. `analyze/stage-7-convenciones-compliance-audit.md`
3. `plan-execution/stage-7-task-plan.md`

¿Aprobado el PLAN v2 CORRECTO? ¿Procedo con TIER 1?

---

**Plan versión:** 2.0.0 (CORRECTO - con 9 convenciones + 10 patrones)
**Status:** Listo para ejecución (esperando confirmación)
**WP:** 2026-04-21-06-15-00-design-specification-correct
**Fecha creación:** 2026-04-21 06:15:00

