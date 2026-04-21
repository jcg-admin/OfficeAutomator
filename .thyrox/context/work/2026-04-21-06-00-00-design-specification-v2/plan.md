```yml
created_at: 2026-04-21 06:00:00
project: THYROX
work_package: 2026-04-21-06-00-00-design-specification-v2
phase: Phase 7 — DESIGN/SPECIFY
author: Claude
status: Borrador
version: 1.0.0
```

# PLAN: Stage 7 DESIGN/SPECIFY v2 (CORRECTAMENTE)

## Objetivo

Crear 5 diseños de Casos de Uso (UC-001 a UC-005) para OfficeAutomator, aplicando TODAS las convenciones del proyecto de forma exhaustiva. Approach: Archivo por archivo, esperando confirmación antes de continuar.

## Precondiciones checkeadas

- [x] REGLAS_DESARROLLO_OFFICEAUTOMATOR.md (1057 líneas) ✓
- [x] convention-mermaid-diagrams.md (335 líneas) ✓
- [x] convention-versioning.md (295 líneas) ✓
- [x] convention-professional-documentation.md (252 líneas) ✓
- [x] convention-naming.md (232 líneas) ✓
- [x] metadata-standards.md (220 líneas) ✓
- [x] thyrox-invariants.md (142 líneas) ✓
- [x] commit-conventions.md (60 líneas) ✓
- [x] calibration-verified-numbers.md (57 líneas) ✓

**Total:** 9 archivos, 2650 líneas de convenciones TODOS leídos

## Convenciones aplicables a Stage 7 UCs

| Convención | Aplica | Requerimiento clave |
|-----------|--------|-------------------|
| REGLAS_DESARROLLO | SÍ | Fail-Fast, Idempotence, Transparency, [ERROR]/[WARN]/[INFO] format |
| convention-mermaid | SÍ | Dark theme, NO emojis, paleta oscura |
| convention-versioning | SÍ | v1.0.0, git tags, CHANGELOG |
| convention-professional | SÍ | Estructura estándar en TODOS los UCs |
| convention-naming | SÍ | Kebab-case, SIN prefijos numéricos |
| metadata-standards | SÍ | Bloque yml (NO ---), campos requeridos |
| thyrox-invariants | SÍ | WP structure, decisions, artifacts |
| commit-conventions | SÍ | type(scope): message |
| calibration-verified | SÍ | Números con source/justificación |

## Estructura de Stage 7 (thyrox-compliant)

```
.thyrox/context/work/2026-04-21-06-00-00-design-specification-v2/
├── design/
│   ├── overall-architecture.md
│   ├── uc-001-select-version.md
│   ├── uc-002-select-language.md
│   ├── uc-003-exclude-applications.md
│   ├── uc-004-validate-configuration.md
│   └── uc-005-install-office.md
├── analyze/
│   └── convenciones-compliance-audit.md
├── PLAN.md (este archivo)
├── plan-execution/
│   └── stage-7-task-plan.md
└── track/
    └── stage-7-exit-criteria.md
```

## Artefactos a crear (en orden)

### FASE 1: ARQUITECTURA (3 documentos)

```
1. design/overall-architecture.md
   Descripción: Arquitectura global (6 capas, flujo de datos, principios core)
   Metadata: 
   - version: 1.0.0
   - status: Borrador
   Esperado: ~400 líneas, 1 diagrama Mermaid dark
   Validación: ✓ Estructura estándar, ✓ Referencias a Stage 6, ✓ Sin emojis

2. plan-execution/stage-7-task-plan.md
   Descripción: Desglose de tareas para crear 5 UCs
   Metadata:
   - status: Borrador
   Esperado: T-001 a T-020 (checkboxes)
   Validación: ✓ Usa template plan-execution.md, ✓ Timestamps para cada T-NNN

3. analyze/convenciones-compliance-audit.md
   Descripción: Auditoría de aplicación de convenciones ANTES de aprobar
   Metadata:
   - status: Borrador
   Esperado: Tabla con ✓/✗ para cada convención
   Validación: ✓ Referencia a metadata-standards.md
```

### FASE 2: UCs (5 documentos)

```
4. design/uc-001-select-version.md
   Descripción: Select Office Version (2024/2021/2019)
   Metadata:
   - uc_id: UC-001
   - version: 1.0.0
   - status: Borrador
   - dependencies: [UC-002, UC-003, Stage 6: scope-statement.md]
   - constraints: [Version selection only, no custom versions]
   Esperado: ~250 líneas, estructura estándar
   Validación: ✓ Sección References, ✓ Sección IN/OUT OF SCOPE, ✓ Exit Criteria

5. design/uc-002-select-language.md
   Similar a UC-001, con adicionales:
   - Sección "Future Extensions" (v1.1: 6 idiomas más)
   Validación: ✓ Multi-idioma support (max 2 v1.0.0)

6. design/uc-003-exclude-applications.md
   Similar, con:
   - Tabla de apps por versión
   - Defaults specification
   Validación: ✓ Compatibility matrix referenced

7. design/uc-004-validate-configuration.md ⭐ CRÍTICO
   Especial: 8-step validation, 3 fases
   Esperado: ~400 líneas
   Validación: ✓ Todas las secciones, ✓ Anti-Microsoft-OCT-bug step 6

8. design/uc-005-install-office.md
   Especial: Idempotence implementation
   Esperado: ~350 líneas
   Validación: ✓ 3 Modos explícitos, ✓ Idempotence patterns
```

### FASE 3: CIERRE (2 documentos)

```
9. track/stage-7-exit-criteria.md
   Descripción: Checklist de completitud Stage 7
   Metadata:
   - status: Borrador
   Esperado: [x] 9/9 documentos creados, [x] 9/9 convenciones aplicadas
   Validación: ✓ Checkboxes para cada convención

10. design/stage-7-changelog.md
    Descripción: Changelog de Stage 7 artifacts
    Metadata:
    - version: 1.0.0
    Esperado: Registro de todas las versiones y cambios
    Validación: ✓ SemVer formato
```

## Protocolo de validación PRE-CREACIÓN (para CADA archivo)

Antes de crear cada archivo, DEBO:

1. **Convención-naming**: ¿El nombre cumple kebab-case, sin prefijos numéricos?
2. **metadata-standards**: ¿Tengo TODOS los campos requeridos?
3. **convention-professional**: ¿Cuál es la estructura estándar esperada?
4. **convention-versioning**: ¿Qué versión inicial debo usar (siempre 1.0.0)?
5. **Stage directory**: ¿Es design/, analyze/, o track/?
6. **THYROX compliance**: ¿WP y phase correctos en metadata?

## Protocolo de validación POST-CREACIÓN (para CADA archivo)

Después de crear cada archivo, DEBO verificar:

- [ ] Metadata completa (bloque yml, NO ---)
- [ ] Nombre en kebab-case
- [ ] Ubicación correcta (design/, analyze/, etc.)
- [ ] Estructura estándar aplicada
- [ ] Sección References con citas a Stage 6, Stage 1, overall-architecture
- [ ] Sección IN/OUT OF SCOPE (si es UC)
- [ ] Sección Exit Criteria (si es UC)
- [ ] NO emojis en texto (pueden haber en tablas si estan asociados a significado técnico, NO decorativos)
- [ ] Números con source (si menciono timeouts, retries, etc.)
- [ ] Sin SPECULATIVE claims sin evidencia

## Timeline (con esperas de confirmación)

| Fase | Duración | Descripción |
|------|----------|-------------|
| **Pre-creación** | 5 min | Leer TODAS las convenciones (ya hecho) |
| **FASE 1** | 15 min | 3 documentos (overall-architecture, task-plan, audit) |
| **Espera confirmación FASE 1** | ∞ | Esperar tu OK antes de continuar |
| **FASE 2** | 45 min | 5 UCs (UC-001 a UC-005) |
| **Espera confirmación FASE 2** | ∞ | Esperar tu OK |
| **FASE 3** | 10 min | 2 documentos (exit-criteria, changelog) |
| **Espera confirmación FASE 3** | ∞ | Esperar tu OK |
| **Git commit** | 2 min | Commit con message type(design): Stage 7... |

**Total:** ~77 minutos + 3 esperas de confirmación

## Checklist pre-ejecución

- [x] 9 convenciones leídas completamente
- [x] Estructura de WP v2 creada
- [x] Protocolo de validación pre/post definido
- [x] Timeline establecido
- [x] Listo para crear FASE 1

## Próximo paso

**Espero tu confirmación antes de crear FASE 1 (overall-architecture.md)**

¿Aprobado el plan? ¿Cambios requeridos?

---

**Plan versión:** 1.0.0
**Estado:** Listo para ejecución (esperando confirmación)
**WP:** 2026-04-21-06-00-00-design-specification-v2
**Fecha:** 2026-04-21 06:00:00

