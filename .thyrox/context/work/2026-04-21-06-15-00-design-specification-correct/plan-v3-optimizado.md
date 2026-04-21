```yml
created_at: 2026-04-21 06:25:00
project: THYROX
work_package: 2026-04-21-06-15-00-design-specification-correct
phase: Phase 7 — DESIGN/SPECIFY
author: Claude
status: Borrador
version: 3.0.0
updated_at: 2026-04-21 06:25:00
```

# PLAN v3: Stage 7 DESIGN/SPECIFY (OPTIMIZADO ARQUITECTÓNICAMENTE)

## Prerequisitos Leídos

- [x] DOCUMENTO-MAESTRO-requisitos.md (guía completa)
- [x] ANÁLISIS-ARQUITECTONICO-dependencias.md (dependencias + cadena crítica)
- [x] UC-Matrix.md (Stage 1 - dependencias UC-001 a UC-005)

---

## Objetivo

Crear diseño técnico de 5 UCs respetando **CADENA CRÍTICA** y **PARALELIZACIÓN MÁXIMA**, optimizando timeline de ~100 min → ~46 min.

---

## DIAGRAMA: CADENA CRÍTICA + PARALELIZACIÓN

```
MINUTO:      0         10        20        30        40        50
             ├────────┼────────┼────────┼────────┼────────┤

TIER 0:      ██████ UC-Matrix-Analysis (5 min) - BLOQUEADOR

TIER 1:           ████████ overall-architecture (8 min) - BLOQUEADOR

TIER 2:                   ┌─ UC-004 (16 min) ← CADENA CRÍTICA
                         │  ├─ UC-001 (8 min) ◇ PARALELO
                         │  ├─ UC-002 (8 min) ◇ PARALELO
                         │  ├─ UC-003 (10 min) ◇ PARALELO
                         │  ├─ ADRs x5 (22 min) ◇ PARALELO
                         │  └─ Task-plan (10 min) ◇ PARALELO
                         │
                         └─ ████████████████ UC-004 (cadena crítica)
                                           ████████ UC-005 (espera UC-004)

TIER 3:                                            ██████ Compliance-Audit
                                                    ██████ Exit-Criteria

TIEMPO TOTAL: ~46 minutos (vs 100+ sin optimizar)
```

---

## ESTRUCTURA DETALLADA POR TIER

### TIER 0: UC MATRIX ANALYSIS (BLOQUEADOR CRÍTICO - 5 min)

**Por qué TIER 0:** Define estructura fundamental que bloquea todo lo demás.

#### 1. analyze/uc-matrix-analysis-detailed.md

**Metadata:**
```yml
created_at: 2026-04-21 HH:MM:SS
project: THYROX
work_package: 2026-04-21-06-15-00-design-specification-correct
phase: Phase 7 — DESIGN/SPECIFY
author: Claude
status: Borrador
```

**Contenido:**
1. **UC Dependency Graph** - visualización de dependencias
2. **Data Flow Diagram** - qué pasa de UC a UC
3. **$Config Object Definition** - estructura del objeto estado
4. **UC Flow Phases:**
   - Fase 1: UC-001 → UC-002 → UC-003 (Input Selection)
   - Fase 2: UC-004 (Validation - BLOQUEADOR)
   - Fase 3: UC-005 (Installation)
5. **Critical Path Analysis** - UC-004 es el cuello de botella (16 min)
6. **Parallelization Opportunities:**
   - UC-001, 002, 003 pueden ser paralelos (una vez arch lista)
   - ADRs pueden ser paralelos
   - Task-plan y README pueden ser paralelos
7. **State Object:**
   ```powershell
   $Config = @{
       Version = "2024"              # de UC-001
       Languages = @("es-ES")        # de UC-002
       ExcludedApps = @("Teams")     # de UC-003
       ConfigPath = ""               # generado por UC-003
       ODTPath = ""                  # descargado por UC-004
       ValidationPassed = $false     # UC-004 → UC-005
   }
   ```

**Validación:**
- [x] Ubicación: `analyze/`
- [x] Nombre: descriptivo de contenido
- [x] Diagrama Mermaid (dark theme)
- [x] Tabla de fases
- [x] Objeto $Config detallado

**Estimado:** ~250 líneas, 2 diagramas

**Bloquea:** overall-architecture.md (y TODO lo demás)

---

### TIER 1: ARCHITECTURE FOUNDATION (BLOQUEADOR - 8 min)

**Por qué TIER 1:** Define capas y flujo que todos los UCs seguirán.

#### 2. design/overall-architecture.md

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

**Contenido:**
1. **6-Layer Architecture:**
   - Layer 1: UI (Menu interface)
   - Layer 2: Orchestration (UC flow controller)
   - Layer 3: Configuration (XML builder)
   - Layer 4: Validation (UC-004 validator)
   - Layer 5: Execution (UC-005 installer)
   - Layer 6: Utility (logging, helpers)

2. **Data Flow Diagram** - cómo pasa $Config entre capas

3. **3 Core Principles:**
   - Reliability: Fail-Fast + validation
   - Transparency: [INFO], [WARN], [ERROR] logging
   - Idempotence: Running 2x = Running 1x

4. **Module Structure:**
   ```
   Functions/
   ├── Public/ (UC-001 a UC-005)
   ├── Private/
   │   ├── Internal/ (helpers)
   │   └── Validation/ (UC-004 steps)
   └── Logging/
   ```

5. **State Management:**
   - $Config object (carries across UCs)
   - ValidationPassed flag (UC-004 → UC-005)
   - Error handling strategy

6. **Integration Points:**
   - Each UC returns to $Config
   - UC-004 sets ValidationPassed
   - UC-005 checks ValidationPassed before executing

**Validación:**
- [x] Basado en UC-Matrix-Analysis
- [x] 6 capas claras
- [x] Data flow diagram
- [x] $Config object referenciado
- [x] Referencias a Stage 6 scope-statement.md

**Estimado:** ~400 líneas, 2-3 diagramas

**Bloquea:** UC Designs (todos)

---

### TIER 2: PARALLEL EXECUTION (DESPUÉS DE TIER 1)

**Por qué TIER 2 parallelizable:** UC-004 es el cuello de botella (16 min), otros pueden hacerse en paralelo.

#### TIER 2A: UC DESIGNS (Parcialmente parallelizable)

**Inicio:** Min 13 (después TIER 1)
**Duración total:** UC-004 (16 min) + UC-005 (8 min espera) = 24 min críticos

##### 3. design/uc-004-validate-configuration.md ⭐ CADENA CRÍTICA

**Por qué primero:** Es el más largo (16 min) y bloquea UC-005.

**Metadata:**
```yml
created_at: 2026-04-21 HH:MM:SS
uc_id: UC-004
version: 1.0.0
dependencies:
  - overall-architecture.md
  - Stage 6: language-compatibility-matrix.md
  - Stage 1: analysis-microsoft-oct.md (paso 6 mitigation)
constraints:
  - 8-step validation (3 fases)
  - Fail-Fast (no UC-005 si falla)
```

**Secciones obligatorias (10 estándar + especiales):**
1. Overview - 8-step validation, 3 fases
2. Scope (IN/OUT)
3. Actors
4. Main Flow - 8 pasos detallados
5. Technical Design - function signature
6. Error Scenarios - mínimo 3
7. Validation Rules - 8 rules (una por paso)
8. Integration Points - UC-003 input, UC-005 blocker
9. Exit Criteria - checklist
10. Testing Strategy - unit + integration
11. **SPECIAL - Validation Structure:**
    - Phase 1 (Parallel): Steps 1, 2, 5
    - Phase 2 (Sequential): Steps 3→4→6
    - Phase 3 (Retry): Steps 7, 8
12. **SPECIAL - Numbered Calibrations:**
    - SHA256 retry: 3 intentos (citado)
    - Backoff: 2s, 4s, 6s
    - Network timeout: 10 min

**Estimado:** ~450 líneas, 2-3 diagramas

**Bloquea:** UC-005

---

##### 4. design/uc-001-select-version.md ◇ PARALELO

**Inicio:** Min 13 (en PARALELO con UC-004)
**Duración:** 8 min

**Metadata:** (estándar UC)

**Secciones:**
1. Overview
2. Scope (IN/OUT) - 3 versiones solo
3. Actors - IT Admin
4. Main Flow
5. Technical Design
6. Error Scenarios - mínimo 2
7. Validation Rules
8. Integration Points - output → UC-002
9. Exit Criteria
10. Testing Strategy
11. References - Stage 6: scope-statement.md, overall-architecture.md

**Estimado:** ~250 líneas

---

##### 5. design/uc-002-select-language.md ◇ PARALELO

**Inicio:** Min 13 (en PARALELO con UC-004 y UC-001)
**Duración:** 8 min

**Special sections:**
- Scope: Max 2 idiomas v1.0.0, roadmap v1.1 (6 más)
- Compatibility matrix reference (Stage 6)
- Future Extensions

**Estimado:** ~280 líneas

---

##### 6. design/uc-003-exclude-applications.md ◇ PARALELO

**Inicio:** Min 13 (en PARALELO)
**Duración:** 10 min

**Special sections:**
- Tabla: Apps por versión
- Tabla: Defaults por versión
- Compatibility validation

**Estimado:** ~320 líneas

---

##### 7. design/uc-005-install-office.md (Espera UC-004)

**Inicio:** Min 29 (DESPUÉS UC-004 completo)
**Duración:** 8 min

**Metadata:**
```yml
dependencies:
  - UC-001, UC-002, UC-003 (input)
  - UC-004 (bloqueador - ValidationPassed flag)
```

**Special sections:**
- 3 Modes: Fresh, Verify, Repair
- Idempotence implementation
- Pre-installation checks
- Progress monitoring

**Estimado:** ~350 líneas

---

#### TIER 2B: ADRs (Totalmente parallelizable)

**Inicio:** Min 13 (en PARALELO con UC-004)
**Duración:** 22 min total (pueden hacerse entre UCs)

**5 ADRs en decisions/:**

##### 8. adr-uc-004-8-step-validation.md

**Metadata:**
```yml
type: Architectural Decision Record
category: Diseño
version: 1.0.0
purpose: 8-step validation con 3 fases
```

**Contenido:**
- Por qué 8 pasos (no 4, no 6)
- Por qué 3 fases (paralela, secuencial, retry)
- Justificación: Performance vs Accuracy
- Trade-offs explicados

**Estimado:** ~120 líneas

---

##### 9. adr-fail-fast-principle.md

**Contenido:**
- Nunca instalar con config inválida
- Validación ANTES de acción
- Justificación: Reliability core principle

**Estimado:** ~100 líneas

---

##### 10. adr-idempotence-approach-uc-005.md

**Contenido:**
- Running 2x = Running 1x
- Verificación de instalación existente
- Repair vs Fresh logic

**Estimado:** ~120 líneas

---

##### 11. adr-phase-parallel-vs-sequential.md

**Contenido:**
- Parallelizar donde posible (performance)
- Secuenciar donde necesario (correctness)
- Decisión en UC-004 validación

**Estimado:** ~110 líneas

---

##### 12. adr-retry-strategy-exponential-backoff.md

**Contenido:**
- 3 intentos máximo
- Backoff: 2s, 4s, 6s
- Solo transient errors

**Estimado:** ~100 líneas

---

#### TIER 2C: DOCUMENTATION (Totalmente parallelizable)

**Inicio:** Min 13 (en PARALELO)
**Duración:** 10-15 min total

##### 13. plan-execution/stage-7-task-plan.md

**Metadata:** (estándar)

**Contenido:**
- T-001 a T-020
- Timestamps
- Dependencias T-NNN
- Protocolos pre/post

**Estimado:** ~250 líneas

---

##### 14. README.md (raíz WP)

**Contenido:**
- Descripción WP
- Índice documentos
- Cómo usar DOCUMENTO-MAESTRO
- Links a Stage 1, 6
- Status completitud

**Estimado:** ~100 líneas

---

### TIER 3: COMPLIANCE & VALIDATION (Secuencial, después UC-005)

**Inicio:** Min 37 (DESPUÉS UC-005)
**Duración:** 5 min

#### 15. analyze/stage-7-convenciones-compliance-audit.md

**Metadata:** (estándar)

**Contenido:**
- Tabla: Convención vs Aplicación
- Audit 9 convenciones + 10 patrones
- Pre-checklist
- Post-checklist

**Estimado:** ~300 líneas

---

### TIER 4: CLOSURE (Secuencial, después Audit)

**Inicio:** Min 42 (DESPUÉS Compliance-Audit)
**Duración:** 4 min

#### 16. track/stage-7-exit-criteria.md

**Metadata:** (estándar)

**Contenido:**
- Checklist completitud Stage 7
- Validación cada UC
- Validación convenciones
- Gate criteria Stage 10

**Estimado:** ~200 líneas

---

## TIMELINE OPTIMIZADO (CON PARALELIZACIÓN)

```
MINUTO  0-5:    ██████ TIER 0: UC-Matrix-Analysis
        5-13:        ████████ TIER 1: overall-architecture
        
        13-29:             TIER 2 PARALELO:
                          ┌─ ████████████████ UC-004 (cadena crítica)
                          ├─ ████████ UC-001 (paralelo)
                          ├─ ████████ UC-002 (paralelo)
                          ├─ ██████████ UC-003 (paralelo)
                          ├─ ██████████████████████ ADRs (paralelo)
                          └─ ██████████ Task-plan (paralelo)
        
        29-37:             ████████ UC-005 (espera UC-004)
        
        37-42:             ██████ TIER 3: Compliance-Audit
        
        42-46:             ████ TIER 4: Exit-Criteria

TOTAL: ~46 minutos (vs 100+ sin optimización)
MEJORA: 54% más rápido
```

---

## SECUENCIA CORRECTA CON CHECKPOINTS

### CHECKPOINT 1: TIER 0 ✓
- Crear: UC-Matrix-Analysis-Detailed
- **Esperar confirmación antes de TIER 1**

### CHECKPOINT 2: TIER 1 ✓
- Crear: overall-architecture.md
- **Esperar confirmación antes de TIER 2**

### CHECKPOINT 3A: UC-004 (cadena crítica) ✓
- Crear: uc-004-validate-configuration.md
- **Puede empezar cuando TIER 1 listo**
- **No necesita esperar UC-001, 002, 003**

### CHECKPOINT 3B: UC-001, 002, 003 (paralelo) ✓
- Crear EN PARALELO: UC-001, UC-002, UC-003
- **Pueden empezar cuando TIER 1 listo**
- **Pueden hacerse MIENTRAS UC-004 se crea**

### CHECKPOINT 3C: ADRs (paralelo) ✓
- Crear EN PARALELO: 5 ADRs
- **Pueden empezar cuando TIER 1 listo**
- **Pueden hacerse MIENTRAS UCs se crean**

### CHECKPOINT 3D: Documentación (paralelo) ✓
- Crear EN PARALELO: Task-plan, README
- **Pueden empezar en cualquier momento**

### CHECKPOINT 4: UC-005 ✓
- Crear: uc-005-install-office.md
- **ESPERAR hasta que UC-004 esté completo**
- **Esto es BLOQUEADOR**

### CHECKPOINT 5: Compliance-Audit ✓
- Crear: stage-7-convenciones-compliance-audit.md
- **ESPERAR hasta que UC-005 esté completo**

### CHECKPOINT 6: Exit-Criteria ✓
- Crear: stage-7-exit-criteria.md
- **ESPERAR hasta que Compliance-Audit esté**

---

## PROTOCOLO DE PARALELIZACIÓN

### Qué PUEDE hacerse en PARALELO (sin esperar):

```
MIENTRAS SE HACE UC-004 (min 13-29):
  ├─ UC-001 (empieza min 13, termina min 21)
  ├─ UC-002 (empieza min 13, termina min 21)
  ├─ UC-003 (empieza min 13, termina min 23)
  ├─ ADR-1 (empieza min 13)
  ├─ ADR-2 (empieza min 13)
  ├─ ADR-3 (empieza min 13)
  ├─ ADR-4 (empieza min 13)
  ├─ ADR-5 (empieza min 13)
  ├─ Task-plan (empieza min 13)
  └─ README (empieza min 13)

DESPUÉS UC-004 COMPLETO (min 29):
  └─ UC-005 (BLOQUEADOR - no puede empezar antes)

DESPUÉS UC-005 COMPLETO (min 37):
  └─ Compliance-Audit (puede esperar, pero casi listo)

DESPUÉS Compliance-Audit (min 42):
  └─ Exit-Criteria (cierre final)
```

### Qué NO PUEDE paralelizarse:

```
UC-Matrix-Analysis → overall-architecture → UC-004 → UC-005 → Compliance → Exit
(secuencial obligatorio)
```

---

## VALIDACIÓN PRE-CREACIÓN (para CADA archivo)

- [ ] Bloquea qué: está en el documento de dependencias
- [ ] Espera qué: está identificado
- [ ] Puede ser paralelo con: está identificado
- [ ] Ubicación correcta: design/, analyze/, decisions/, track/
- [ ] Metadata completa: bloque yml con todos los campos
- [ ] Nombre kebab-case: sin prefijos numéricos
- [ ] Estructura estándar: 10+ secciones (si UC)

---

## CHECKLIST FINAL PRE-EJECUCIÓN

- [x] TIER 0 identificado (UC-Matrix-Analysis)
- [x] TIER 1 identificado (overall-architecture)
- [x] TIER 2A identificado (UC Designs - 1 crítico + 3 paralelo + 1 espera)
- [x] TIER 2B identificado (5 ADRs paralelo)
- [x] TIER 2C identificado (Task-plan, README paralelo)
- [x] TIER 3 identificado (Compliance-Audit)
- [x] TIER 4 identificado (Exit-Criteria)
- [x] Cadena crítica clara: UC-Matrix → arch → UC-004 → UC-005 → Audit → Exit
- [x] Paralelización máxima: UC-001,002,003,ADRs,Task,README en paralelo
- [x] Timeline optimizado: ~46 min vs 100+ original
- [x] Checkpoints definidos (6 total)

**LISTO PARA CREAR TIER 0 (UC-Matrix-Analysis-Detailed)**

---

## Próximo Paso

¿Aprobado el PLAN v3 OPTIMIZADO ARQUITECTÓNICAMENTE?

¿Procedo a crear **TIER 0: UC-Matrix-Analysis-Detailed**?

---

**Plan versión:** 3.0.0 (Optimizado arquitectónicamente - TIER 0 + dependencias + paralelización)
**Status:** Listo para ejecución (esperando confirmación)
**Cadena crítica:** ~42 minutos (vs 100+ sin optimizar)
**Mejora:** 54% más rápido con paralelización
**WP:** 2026-04-21-06-15-00-design-specification-correct
**Fecha creación:** 2026-04-21 06:25:00

