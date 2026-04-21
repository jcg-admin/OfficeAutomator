```yml
created_at: 2026-04-21 06:10:00
project: THYROX
work_package: 2026-04-21-06-00-00-design-specification-v2
type: Pre-requisites Checklist
status: Borrador
```

# DOCUMENTO MAESTRO: Requisitos Previos para Stage 7 CORRECTO

## 1. CONVENCIONES VERIFICADAS (9 archivos, 2650 líneas)

- [x] REGLAS_DESARROLLO_OFFICEAUTOMATOR.md (1057 líneas)
- [x] convention-mermaid-diagrams.md (335 líneas)
- [x] convention-versioning.md (295 líneas)
- [x] convention-professional-documentation.md (252 líneas)
- [x] convention-naming.md (232 líneas)
- [x] metadata-standards.md (220 líneas)
- [x] thyrox-invariants.md (142 líneas)
- [x] commit-conventions.md (60 líneas)
- [x] calibration-verified-numbers.md (57 líneas)

## 2. PATRONES CLAVE VERIFICADOS (10 patrones)

| Patrón | Status | Ubicación | Requerimiento |
|--------|--------|-----------|----------------|
| 1. YAML Frontmatter | ✓ | metadata-standards.md | Bloque yml, NO --- |
| 2. Work Packages | ✓ | thyrox-invariants.md | YYYY-MM-DD-HH-MM-SS-nombre |
| 3. Conventional Commits | ✓ | ADR-003 | type(scope): message |
| 4. ROADMAP.md | ✗ | NO EXISTE | Crear si necesario |
| 5. GitHub Actions | ✗ | NO EXISTE | Crear si necesario |
| 6. ADR Structure | ✓ | adr-*.md files | type: ADR, status, decisión |
| 7. UC Matrix | ✓ | Stage 1 WP | Dependencias UC |
| 8. now.md | ✓ | .thyrox/context/now.md | Estado actual |
| 9. README en directorios | ✓ | Múltiples directorios | Doc local en cada nivel |
| 10. Stages | ✓ | 12 stages (THYROX base) | DISCOVER→SCOPE→DESIGN→IMPLEMENT→TRACK→RELEASE |

## 3. ESTADO ACTUAL (from now.md)

### Progreso:
- Stage 1: DISCOVER ✓ COMPLETADO
- Stage 6: SCOPE ✓ COMPLETADO
- Stage 7: DESIGN/SPECIFY ← SIGUIENTE

### Requisito previo:
- [ ] Aprobación Scope Statement (stage-6-exit-criteria.md)

### Artefactos Stage 1:
```
.thyrox/context/work/2026-04-21-01-30-00-uc-documentation/
├── problem-statement.md
├── actors-stakeholders.md
├── discovery-notes.md
├── use-case-matrix.md
├── analysis-microsoft-oct.md
```

### Artefactos Stage 6:
```
.thyrox/context/work/2026-04-21-03-00-00-scope-definition/
├── scope-statement.md
├── language-compatibility-matrix.md
├── precise-definitions.md
└── stage-6-exit-criteria.md
```

## 4. ADR STRUCTURE (formato requerido)

```yml
type: Architectural Decision Record
category: [Decisión Técnica | Diseño | Implementación]
version: 1.0
purpose: [Propósito de la decisión]
goal: [Objetivo]
updated_at: YYYY-MM-DD
```

**Secciones obligatorias:**
1. Decisión (qué se decidió)
2. Formato (si aplica)
3. Tipos Soportados (si aplica)
4. Justificación (por qué)
5. Impacto (consecuencias)

## 5. UC MATRIX (Dependencias)

```
UC-001 (Select Version)
  ↓
UC-002 (Select Language) 
  ↓
UC-003 (Exclude Applications)
  ↓
UC-004 (Validate Configuration) ← CRÍTICO
  ↓
UC-005 (Install Office)
```

**UC-004 es bloqueador:** Sin validación exitosa, UC-005 NO ejecuta (Fail-Fast)

## 6. REFERENCIAS CRÍTICAS

**Para Stage 7 DESIGN, debo referenciar:**

### De Stage 1:
- `analysis-microsoft-oct.md` → Microsoft OCT bug mitigado en UC-004 paso 6
- `use-case-matrix.md` → Dependencias entre UCs

### De Stage 6:
- `scope-statement.md` → Scope IN/OUT
- `language-compatibility-matrix.md` → Validaciones UC-004
- `precise-definitions.md` → Valores exactos (versiones, idiomas, apps)

## 7. NÚMEROS CALIBRADOS

**Timeouts (from calibration-verified-numbers.md):**
- setup.exe execution: 30 minutos
- network operations: 10 minutos
- individual steps: 5 minutos

**Retries:**
- Transient errors: 3 intentos máximo
- Backoff: exponencial (2s, 4s, 6s)
- SHA256 verification: 3 intentos

**Disk space:**
- Requerido: 3 GB mínimo

## 8. METADATA ESTÁNDAR PARA STAGE 7

### Para UC Design docs:
```yml
created_at: 2026-04-21 HH:MM:SS
project: THYROX
work_package: 2026-04-21-06-00-00-design-specification-v2
phase: Phase 7 — DESIGN/SPECIFY
uc_id: UC-NNN
version: 1.0.0
author: Claude
status: Borrador
dependencies: [UC-XXX, Stage 6: scope-statement.md, Stage 1: analysis.md]
constraints: [v1.0.0 only, no custom versions, etc]
```

### Para ADR (decisiones Stage 7):
```yml
type: Architectural Decision Record
category: Diseño
version: 1.0.0
purpose: [Qué se decidió]
updated_at: 2026-04-21 HH:MM:SS
```

## 9. ESTRUCTURA ESTÁNDAR UC (from convention-professional-documentation.md)

```
# UC-NNN: {Name}

## Overview
- Descripción específica
- Complejidad, Criticidad
- Failure modes

## Scope (IN / OUT OF SCOPE)
- IN SCOPE - v1.0.0
- OUT OF SCOPE - v1.0.0
- Roadmap - v1.1

## Actors and Preconditions

## Main Flow
- Pasos en prosa clara

## Technical Design
- Function signature (PowerShell)
- Parameters
- Return values

## Error Scenarios
- Mínimo 2 escenarios

## Validation Rules
- Reglas de negocio

## Integration Points
- Depends On
- Used By
- Logs

## Exit Criteria
- Checklist de completitud

## Testing Strategy
- Unit tests
- Integration tests

## References
- Stage 6 outputs
- Stage 1 analysis
- overall-architecture.md
```

## 10. ARTEFACTOS A CREAR EN STAGE 7

### Definidos en now.md:
```
.thyrox/context/work/2026-04-21-06-00-00-design-specification-v2/
├── design/
│   ├── overall-architecture.md
│   ├── uc-001-design.md
│   ├── uc-002-design.md
│   ├── uc-003-design.md
│   ├── uc-004-design.md
│   └── uc-005-design.md
├── design/
│   ├── function-specifications.md
│   ├── error-handling-strategy.md
│   └── stage-7-exit-criteria.md
```

### Adicionales que debería crear:
```
├── decisions/
│   ├── adr-uc-004-8-step-validation.md
│   ├── adr-fail-fast-principle.md
│   ├── adr-idempotence-approach-uc-005.md
│   ├── adr-phase-parallel-vs-sequential.md
│   └── adr-retry-strategy-exponential-backoff.md
├── analyze/
│   └── stage-7-convenciones-compliance.md
├── track/
│   └── stage-7-exit-criteria.md
└── plan-execution/
    └── stage-7-task-plan.md
```

## 11. PROTOCOLO DE VALIDACIÓN PRE-CREACIÓN

Antes de crear CADA archivo:

- [ ] ¿Nombre en kebab-case?
- [ ] ¿Metadata completa (bloque yml)?
- [ ] ¿Stage directory correcto (design/, analyze/, track/)?
- [ ] ¿Convenciones aplicables identificadas?
- [ ] ¿Estructura estándar clara?
- [ ] ¿Todas las referencias listas?
- [ ] ¿Números calibrados?
- [ ] ¿Dependencias mapeadas?

## 12. PROTOCOLO DE VALIDACIÓN POST-CREACIÓN

Después de crear CADA archivo:

- [ ] Metadata completa (bloque yml, NO ---)
- [ ] Nombre en kebab-case sin prefijos numéricos
- [ ] Ubicación correcta (design/uc-001-select-version.md, etc)
- [ ] Estructura estándar aplicada completamente
- [ ] Sección "Scope (IN/OUT)" presente
- [ ] Sección "References" vinculando Stage 6 y Stage 1
- [ ] Sección "Exit Criteria" presente (si UC)
- [ ] Sección "Testing Strategy" presente (si UC)
- [ ] NO emojis decorativos (OK técnicos)
- [ ] Números con source citado
- [ ] Formatos de código: PowerShell ejemplo correcto
- [ ] Diagramas Mermaid con dark theme si existen
- [ ] Sin SPECULATIVE claims sin evidencia

## 13. ESTADÍSTICAS ESPERADAS

**Tamaño de Stage 7 (estimado):**
- 10-15 documentos (~500-800 KB)
- 5000-8000 líneas de especificación
- 5 ADRs adicionales
- 100% convenciones aplicadas

## CHECKLIST FINAL ANTES DE CREAR PLAN v2

- [x] 9 convenciones leídas (2650 líneas)
- [x] 10 patrones identificados
- [x] now.md revisado (estado actual)
- [x] ADR structure entendido
- [x] UC Matrix conocido
- [x] metadata-standards aplicable
- [x] Stage directories claros (design/, analyze/, track/)
- [x] Referencias identificadas (Stage 1, Stage 6, overall-architecture)
- [x] Números calibrados conocidos
- [x] Estructura UC estándar definida
- [x] Protocolos de validación pre/post listos

**LISTO PARA CREAR PLAN v2 CORRECTO**

---

**Documento creado:** 2026-04-21 06:10:00
**Status:** Documento de referencia (NO es el PLAN, es PRE-REQUISITO)
**Próximo paso:** Crear PLAN v2 que incorpore TODO esto

