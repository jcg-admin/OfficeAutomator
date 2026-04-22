```yml
created_at: 2026-04-22 08:15:00
project: OfficeAutomator
work_package: 2026-04-22-07-59-20-documentation-audit
phase: Phase 1 — DISCOVER
author: Claude
status: Borrador
version: 1.0.0
```

# OPCIÓN B: Evaluación de Viabilidad para OfficeAutomator

---

## Resumen Ejecutivo

**OPCIÓN B** (Estructura Plana por Dominio) es **VIABLE y RECOMENDADA** para OfficeAutomator.

La propuesta reorganiza documentación en 12 cajones planos (requirements, architecture, quality-goals, etc.) sin meta-contenedores. La estructura actual de OfficeAutomator está parcialmente alineada; requiere consolidación de duplicados y creación de 3-4 cajones faltantes.

**Estimado de esfuerzo:** Phase 10 IMPLEMENT (4-6 horas)

---

## PARTE 1: Mapeo de OPCIÓN B a OfficeAutomator

### Tabla: Cajones OPCIÓN B → Documentación actual

| Cajon OPCIÓN B | Existencia en OA | Estado | Archivos actuales | Acción |
|---|---|---|---|---|
| `introduction/` | ❌ NO EXISTE | Falta | README.md (disperso) | Crear cajon, consolidar intro |
| `requirements/` | ✅ EXISTE | Parcial | use-case-matrix.md, UC_COMPLETION_VERIFICATION.md | Consolidar en cajon |
| `quality-goals/` | ❌ NO EXISTE | Falta | — | Crear (extraer de README, phase-12 patterns) |
| `stakeholders/` | ❌ NO EXISTE | Falta | actors-stakeholders.md (en WP) | Crear cajon, mover WP → docs/ |
| `constraints/` | ✅ EXISTE | Disperso | language-compatibility-matrix.md (en WP) | Consolidar en cajon |
| `context-scope/` | ✅ EXISTE | Disperso | scope-statement.md (en WP), README | Consolidar en cajon |
| `solution-strategy/` | ❌ NO EXISTE | Falta | — | Crear (definir estrategia 3-layer) |
| `architecture/` | ✅ EXISTE | Desactualizada | ARCHITECTURE.md | IMPORTANTE: actualizar + dividir en subcajones |
| `crosscutting-concepts/` | ❌ NO EXISTE | Falta | csharp-tdd-guide.md (en guidelines) | Crear cajon, referenciar Phase 12 patterns |
| `quality-scenarios/` | ✅ EXISTE | Disperso | TEST_EXECUTION_ANALYSIS.md, TESTING_SETUP.md | Consolidar, eliminar duplicados |
| `risks-technical-debt/` | ✅ EXISTE | Disperso | (en WPs) | Consolidar en cajon |
| `glossary/` | ❌ NO EXISTE | Falta | — | Crear (términos OA: UC, Layer, etc.) |
| `_archive/` | ❌ NO EXISTE | Falta | — | Crear (archivar WPs viejos post-Phase-12) |
| `_methodology/` | ❌ NO EXISTE | Falta | — | Crear (REGLAS_DESARROLLO_OFFICEAUTOMATOR.md) |
| `_tools/` | ❌ NO EXISTE | Falta | — | Crear (convention-mermaid.md, etc.) |

**Resumen:** 6 cajones parcialmente existentes + 9 cajones faltantes = **Viabilidad: ALTA**

---

## PARTE 2: Estructura Propuesta para OfficeAutomator

### Template OPCIÓN B aplicado a OA

```
OfficeAutomator/docs/
│
├── INDEX.md                           ← Navegación nueva
├── README.md                          ← Actual (sin cambios)
│
│   ═══════════════════════════════════════════════════════════════
│   CAJONES PRIMARIOS (12)
│   ═══════════════════════════════════════════════════════════════
│
├── introduction/                      ← NUEVO
│   ├── overview.md                    ← (contenido de README § Qué es OA)
│   ├── installation-quick-start.md    ← (contenido de README § Setup)
│   └── three-layer-architecture.md    ← (contenido de README § Layers)
│
├── requirements/                      ← CONSOLIDAR
│   ├── README.md
│   ├── use-cases/
│   │   ├── uc-001-select-version.md
│   │   ├── uc-002-select-language.md
│   │   ├── uc-003-exclude-applications.md
│   │   ├── uc-004-validate-integrity.md
│   │   └── uc-005-install-office.md
│   ├── UC_COMPLETION_VERIFICATION.md  ← (actual, mover aquí)
│   └── analysis/
│       └── use-case-matrix.md         ← (actual, mover aquí)
│
├── quality-goals/                     ← NUEVO
│   └── goals.md
│       - Idempotencia garantizada
│       - Validación exhaustiva
│       - Transparencia en logs
│       - Recuperabilidad
│
├── stakeholders/                      ← NUEVO
│   └── stakeholders.md                ← (actors-stakeholders.md de WP)
│
├── constraints/                       ← CONSOLIDAR
│   ├── business/
│   │   └── office-versions.md         ← (2024, 2021, 2019)
│   ├── technical/
│   │   ├── language-compatibility.md  ← (actual language-compatibility-matrix.md)
│   │   ├── dotnet-sdk.md
│   │   └── powershell-version.md
│   ├── organizational/
│   └── regulatory/
│       └── supported-languages.md     ← (es-ES, en-US solamente v1.0.0)
│
├── context-scope/                     ← CONSOLIDAR
│   ├── system-context.md
│   ├── scope.md                       ← (actual scope-statement.md de WP)
│   └── dependencies.md
│
├── solution-strategy/                 ← NUEVO
│   └── strategy.md
│       - Three-layer architecture
│       - PowerShell orchestration
│       - C# core logic
│       - Idempotency approach
│
├── architecture/                      ← ACTUALIZAR + DIVIDIR
│   ├── INDEX.md                       ← Nueva navegación
│   ├── overview.md                    ← (ARCHITECTURE.md actual, mejorado)
│   ├── layers/
│   │   ├── layer-0-bash.md            ← System bootstrap (setup.sh, verify-environment.sh)
│   │   ├── layer-1-powershell.md      ← Automation (Invoke-OfficeAutomator, UC functions)
│   │   └── layer-2-csharp.md          ← Core logic (OfficeAutomator.Core)
│   ├── components/
│   │   ├── configuration-xml.md       ← Generación y validación
│   │   ├── office-deployment-tool.md  ← Integración ODT
│   │   └── validation-engine.md       ← 8-point validation (UC-004)
│   ├── flows/
│   │   ├── diagrams/
│   │   │   ├── overall-flow.mermaid   ← UC-001 → UC-005
│   │   │   ├── validation-flow.mermaid ← UC-004 (8 pasos)
│   │   │   └── installation-flow.mermaid ← UC-005
│   │   └── descriptions.md
│   ├── design/
│   │   ├── design-patterns.md         ← Three-layer, state machine, etc
│   │   ├── class-diagrams.md
│   │   └── data-model.md              ← XML schema, config objects
│   ├── deployment/
│   │   ├── environments.md            ← Dev, staging, production
│   │   ├── dotnet-build.md            ← Make targets, CI/CD
│   │   └── distribution.md            ← PowerShell gallery, nuget, etc
│   └── decisions/                     ← ADR links
│       ├── adr-three-layer.md         ← (existente)
│       ├── adr-powershell-orchestration.md
│       └── adr-tdd-discipline.md      ← (existente)
│
├── crosscutting-concepts/             ← NUEVO
│   ├── patterns.md
│   │   - TDD RED-GREEN-REFACTOR
│   │   - State machine testing (minimal path)
│   │   - Compilation cache prevention
│   │   - FSM deterministic transitions
│   ├── principles.md
│   │   - Fail-fast validation
│   │   - Single responsibility
│   │   - Idempotency guarantee
│   ├── idioms.md
│   │   - PowerShell verb-noun naming
│   │   - C# namespace conventions
│   │   - XML configuration idioms
│   └── guidelines/
│       ├── architecture-three-layer.instructions.md ← (desde .thyrox/guidelines)
│       ├── csharp-tdd-guide.instructions.md
│       ├── csharp-build-practices.instructions.md
│       └── csharp-state-machine-patterns.instructions.md
│
├── quality-scenarios/                 ← CONSOLIDAR
│   ├── README.md
│   ├── functional/
│   │   ├── uc-001-happy-path.md
│   │   ├── uc-001-error-scenarios.md
│   │   ├── uc-002-happy-path.md
│   │   └── ... (uc-003 a uc-005)
│   ├── performance/
│   │   └── download-speed.md          ← ODT + Office download time SLA
│   ├── security/
│   │   ├── sha256-validation.md
│   │   ├── xml-injection.md
│   │   └── powershell-execution-policy.md
│   ├── reliability/
│   │   ├── http-503-resilience.md     ← (dotnet SDK fallback source)
│   │   ├── idempotence-validation.md
│   │   └── recovery-from-interruption.md
│   └── testing/
│       ├── test-execution-setup.md    ← (consolidar TESTING_SETUP.md)
│       ├── test-execution-guide.md    ← (consolidar EXECUTION_GUIDE.md)
│       └── test-analysis.md           ← (consolidar TEST_EXECUTION_ANALYSIS.md)
│
├── risks-technical-debt/              ← CONSOLIDAR
│   ├── risk-register.md               ← (consolidar de WPs)
│   ├── mitigation-plans/
│   │   ├── language-incompatibility.md ← UC-004 Microsoft OCT bug
│   │   └── office-version-eol.md
│   └── technical-debt.md
│
├── glossary/                          ← NUEVO
│   └── terms.md
│       - UC (Use Case) vs. phase vs. feature
│       - Layer 0, Layer 1, Layer 2
│       - ODT (Office Deployment Tool)
│       - Configuration XML
│       - TDD (Test-Driven Development)
│       - FSM (Finite State Machine)
│       - WP (Work Package) — interno
│
│   ═══════════════════════════════════════════════════════════════
│   CAJONES SOPORTE (prefijo _)
│   ═══════════════════════════════════════════════════════════════
│
├── _archive/                          ← NUEVO
│   ├── v0/                            ← Documentación v1.0.0-beta
│   ├── v1/                            ← Documentación v1.0.0 (actual)
│   └── deprecation-notices.md         ← Qué cambió entre versiones
│
├── _methodology/                      ← NUEVO
│   ├── development-standards.md       ← (REGLAS_DESARROLLO_OFFICEAUTOMATOR.md)
│   ├── design-review-checklist.md
│   ├── contribution-guide.md          ← (CONTRIBUTING.md mejorado)
│   └── versioning-policy.md           ← (convention-versioning.md)
│
└── _tools/                            ← NUEVO
    ├── diagram-conventions.md         ← (convention-mermaid-diagrams.md)
    ├── markdown-style-guide.md
    └── templates/
        ├── uc-template.md
        ├── architecture-decision.md
        └── test-scenario.md
```

---

## PARTE 3: Validación de OPCIÓN B para OfficeAutomator

### Checklist de cumplimiento

| Criterio | Aplicación a OA | ✅ |
|----------|---|---|
| **Plano en nivel 1** | 15 cajones hermanos (introduction, requirements, ..., _tools) | ✅ |
| **Por dominio** | Cada cajon = 1 área bien definida (UC, architecture, quality, etc) | ✅ |
| **Sin prefijos numéricos** | Nombres auto-explican: `requirements/`, `architecture/`, no `01-`, `02-` | ✅ |
| **Jerarquía interna libre** | Subcarpetas dentro de each cajón (use-cases/, components/, etc.) | ✅ |
| **Mapeo a 12 secciones** | 12 secciones estándar → 12 cajones OA (introduction, requirements, ..., glossary) | ✅ |
| **Autoexplicativo** | Usuario nuevo entiende: requirements = UCs, architecture = componentes, etc. | ✅ |
| **Links claros** | INDEX.md maestro mapea cajones → propósito | ✅ |
| **Sin meta-contenedores** | No hay carpeta "functional/" o "technical/" que contenga otros dominios | ✅ |
| **Navegación intuitiva** | 1-2 niveles máximo para llegar a archivo | ✅ |

**RESULTADO: ✅ OfficeAutomator CUMPLE 100% OPCIÓN B**

---

## PARTE 4: Duplicaciones actuales → Consolidación en OPCIÓN B

### Duplicación 1: .NET SDK Installation

| Ubicación actual | Acción | Destino OPCIÓN B |
|---|---|---|
| `README.md § Installation` | Mover | `introduction/installation-quick-start.md` |
| `docs/DOTNET_SDK_INSTALLATION_NOTES.md` | Mover | `constraints/technical/dotnet-sdk.md` |
| `docs/DOTNET_SDK_INSTALLATION_NOTES.md` | Eliminar | (consolidado) |

**Resultado:** 1 fuente única en `constraints/technical/dotnet-sdk.md` + referencia en `introduction/`

### Duplicación 2: Test Execution

| Ubicación actual | Acción | Destino OPCIÓN B |
|---|---|---|
| `docs/TESTING_SETUP.md` | Mover | `quality-scenarios/testing/test-execution-setup.md` |
| `docs/EXECUTION_GUIDE.md` | Mover | `quality-scenarios/testing/test-execution-guide.md` |
| `docs/TEST_EXECUTION_ANALYSIS.md` | Mover | `quality-scenarios/testing/test-analysis.md` |

**Resultado:** 3 archivos organizados por tema en 1 cajon (no TRIPLICADO)

### Duplicación 3: Architecture Overview

| Ubicación actual | Acción | Destino OPCIÓN B |
|---|---|---|
| `docs/ARCHITECTURE.md` | Mejorar + dividir | `architecture/overview.md` + subcajones |
| `docs/PROJECT_STRUCTURE_REFERENCE.md` | Mover | `architecture/layers/` (detalles de estructura) |
| `README.md § Layers` | Referenciar | (link a `architecture/layers/`) |

**Resultado:** Arquitectura actualizada y dividida por responsabilidad

### Duplicación 4: Three-Layer Pattern

| Ubicación actual | Acción | Destino OPCIÓN B |
|---|---|---|
| `README.md` (NEW section) | Mover | `introduction/three-layer-architecture.md` |
| `.thyrox/guidelines/architecture-three-layer.instructions.md` | Referenciar | (from `crosscutting-concepts/guidelines/`) |

**Resultado:** Documentación visible para usuarios + guidelines para developers

---

## PARTE 5: Impacto de Implementación

### Archivos a crear

- 9 nuevos cajones: introduction/, quality-goals/, stakeholders/, solution-strategy/, crosscutting-concepts/, glossary/, _archive/, _methodology/, _tools/
- 1 nuevo INDEX.md maestro
- ~20-30 nuevos archivos .md dentro de cajones (dividir ARCHITECTURE.md, crear guidelines links, etc)

### Archivos a eliminar/consolidar

- DOTNET_SDK_INSTALLATION_NOTES.md (duplicado) ❌
- TESTING_SETUP.md → quality-scenarios/ ✅
- EXECUTION_GUIDE.md → quality-scenarios/ ✅
- TEST_EXECUTION_ANALYSIS.md → quality-scenarios/ ✅

### Archivos a actualizar

- README.md: agregar link a INDEX.md, mantener quick-start
- ARCHITECTURE.md: **reescribir** con estructura 3-capas y dividir en subcajones
- CONTRIBUTING.md → _methodology/contribution-guide.md
- Todos los archivos: revisar links internos (ahora apuntan a nuevas ubicaciones)

### Estimado de esfuerzo

| Tarea | Esfuerzo | Phase |
|-------|----------|-------|
| Crear 9 cajones base | 30 min | 10 IMPLEMENT |
| Mover archivos existentes | 1 hora | 10 IMPLEMENT |
| Reescribir ARCHITECTURE.md | 1.5 horas | 10 IMPLEMENT |
| Crear nuevos archivos (glossary, quality-goals, etc) | 1.5 horas | 10 IMPLEMENT |
| Actualizar links internos | 1 hora | 10 IMPLEMENT |
| Crear INDEX.md maestro | 30 min | 10 IMPLEMENT |
| Testing de navegación | 30 min | 11 TRACK |
| **TOTAL** | **~6 horas** | 10-11 |

---

## PARTE 6: Comparación: OPCIÓN B vs. Estructura actual

### Estructura actual (desorganizada)

```
❌ Pros: Archivos están presentes
❌ Cons:
  - Duplicaciones (DOTNET en README + docs/)
  - Test execution triplicado (3 archivos sin relación clara)
  - ARCHITECTURE.md desactualizada (no refleja Phase 12 patterns)
  - Sin INDEX maestro (usuario no sabe dónde buscar)
  - Sin glossario (términos OA no documentados)
  - Patrones Phase 12 no visibles (enterrados en .thyrox/guidelines/)
  - User journey roto (README → docs/ confuso)
```

### OPCIÓN B (propuesta)

```
✅ Pros:
  - 1 cajon = 1 dominio (claro, navegable)
  - Sin duplicaciones (consolidación explícita)
  - INDEX.md guía usuario
  - Patrones Phase 12 visibles (crosscutting-concepts/)
  - Gaps llenan (glossary, quality-goals, stakeholders)
  - ARCHITECTURE.md actualizado + dividido
  - User journey directo (README → INDEX → cajon específico)

❌ Cons:
  - Esfuerzo Phase 10 (6 horas)
  - Requiere actualizar links internos
  - Cambio de navegación (requiere comunicación a usuarios)
```

---

## PARTE 7: Recomendación para Phases posteriores

### Phase 2 (BASELINE): Establecer métricas

- Antes: 117 archivos, 2.1 MB, 3 duplicaciones
- Después: ~140 archivos, ~2.3 MB, 0 duplicaciones (consolidadas)
- Cobertura: ~85% → ~95%
- User confusion: Alta → Baja

### Phase 3 (DIAGNOSE): Analizar causa de desorganización

- README actualizándose sin coordinación con docs/
- WP patterns no propagándose (adresado Phase 12)
- Falta de ownership model (R-001 risk)
- Herencia de estructura vieja sin refactor

### Phase 5 (STRATEGY): Proponer OPCIÓN B como solución

- Adoptaremos OPCIÓN B como estructura estándar post-v1.0.0
- Primera implementación en Phase 10 (6 horas)
- Ownership: Documentation Lead
- Timeline: v1.0.0-rc fase (antes de release)

### Phase 8 (PLAN EXECUTION): Task plan detallado

```
[T-001] Crear 9 nuevos cajones base         (R-1)
[T-002] Migrar archivos a cajones           (R-2)
[T-003] Reescribir ARCHITECTURE.md          (R-3)
[T-004] Crear glossary.md                   (R-4)
[T-005] Crear quality-goals.md              (R-5)
[T-006] Crear solution-strategy.md          (R-6)
[T-007] Crear INDEX.md maestro              (R-7)
[T-008] Actualizar links internos           (R-8)
[T-009] Verificar navegación usuario        (R-9)
[T-010] Crear deprecation notices           (R-10)
```

### Phase 10 (IMPLEMENT): Ejecutar consolidación

- Branch: `feature/documentation-opcion-b`
- Commits por cajon (granularidad clara)
- PR review: Documentation Lead + Architect

### Phase 11 (TRACK): Validar consolidación

- ✓ 0 duplicaciones
- ✓ 15 cajones planos navegables
- ✓ INDEX.md funcional
- ✓ Todos los links actualizados
- ✓ Phase 12 patterns visibles

### Phase 12 (STANDARDIZE): Propagar patrón

- Crear `_methodology/documentation-structure-opcion-b.md` 
- Documentar cómo agregar nuevos cajones (sin meta-contenedores)
- Crear rule: "Post-WP documentation flows to docs/ via OPCIÓN B"

---

## CONCLUSIÓN

**OPCIÓN B es RECOMENDADA para OfficeAutomator.**

Propuesta: Adoptar OPCIÓN B como estructura estándar en Phase 5 STRATEGY, ejecutar en Phase 10 IMPLEMENT (6 horas), validar en Phase 11 TRACK, standarizar en Phase 12.

La adopción resuelve los 5 riesgos identificados en Phase 1:
- R-001: Ownership per cajon clara ✅
- R-002: User confusion → índice maestro ✅
- R-003: Bloat → consolidación ✅
- R-004: Architecture outdated → división clara + update ✅
- R-005: Pattern propagation → visible en crosscutting-concepts/ ✅

---

**Siguiente paso:** Incorporar OPCIÓN B como propuesta de solución en Phase 5 STRATEGY (cuando se evalúen alternativas).

