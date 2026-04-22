```yml
created_at: 2026-04-22 04:46:55
project: OfficeAutomator
work_package: 2026-04-22-04-46-55-project-structure-cleanup
current_phase: Phase 1 — DISCOVER
author: Claude Code
status: Borrador
```

# Phase 1: DISCOVER — Project Structure Cleanup Analysis

## Problem Statement

El repositorio actual tiene tres problemas principales que impiden establecer una estructura correcta y confiable del proyecto:

1. **Root contaminado** con archivos de documentación y scripts que deberían estar organizados
2. **README incorrecto** con rutas que no corresponden a la estructura real del proyecto
3. **Documentación dispersa** en múltiples ubicaciones sin coherencia clara

## Contexto

Después de Phase 10 (EXECUTE) de la rama `feature/review-branch-integration`, el proyecto OfficeAutomator contiene:

**Implementación completa:**
- 10 clases C# production-ready
- 220+ tests con cobertura del 100%
- 5 Use Cases completamente implementados (UC-001 a UC-005)
- Documentación de arquitectura en `docs/`
- Scripts de ejecución y testing

**Problema actual:**
El root del repositorio contiene archivos que deberían estar en subdirectorios específicos, y el README.md no refleja la verdadera estructura de carpetas.

---

## Hallazgos Detallados

### 1. Root Contaminado — Archivos que NO deberían estar en /

| Archivo | Tamaño | Debería estar en | Razón |
|---------|--------|-----------------|-------|
| EXECUTION_GUIDE.md | 13 KB | docs/ | Documentación de proyecto |
| TEST_EXECUTION_REPORT.md | 5.8 KB | docs/ | Reportes de testing |
| ESTRUCTURA_PROYECTO.txt | 6.2 KB | docs/ o eliminar | Archivo de texto, no markdown |
| PROYECTO_ESTRUCTURA_ACTUAL.txt | 12 KB | docs/ o eliminar | Archivo de texto, no markdown |
| SPRINT1-CORRECTION-COMPLETE.txt | 17 KB | docs/ o eliminar | Archivo de texto, no markdown |
| fix-failing-tests.ps1 | 6.3 KB | scripts/ | Script de PowerShell |
| Directory.Build.props | 652 B | src/ | Configuración de build .NET |
| nuget.config | 273 B | src/ | Configuración NuGet |

**Total archivos a mover/eliminar:** 8

**Archivos que SÍ deben estar en root:**
- README.md ✓
- OfficeAutomator.sln ✓
- global.json ✓
- .gitignore ✓
- .git/ ✓
- .claude/ ✓
- .thyrox/ ✓

### 2. README.md — Errores Identificados

#### Error 1: Rutas incorrectas en Quick Start (Líneas 31, 35)

**Actual:**
```bash
cd OfficeAutomator/OfficeAutomator.Core
```

**Real:**
```bash
cd OfficeAutomator/src/OfficeAutomator.Core
```

**Impacto:** Los usuarios seguirán las instrucciones y fallarán porque el directorio no existe.

#### Error 2: Ubicación de documentación (Líneas 126-129)

**Lo que dice el README:**
```
DOCUMENTATION:
    ├── TESTING_SETUP.md
    ├── UC_COMPLETION_VERIFICATION.md
    ├── TDD_COMPLETION_REPORT.md
    └── TEST_EXECUTION_ANALYSIS.md
```

Con ruta `OfficeAutomator.Core/`

**Realidad:**
- TESTING_SETUP.md está en `docs/TESTING_SETUP.md`
- UC_COMPLETION_VERIFICATION.md está en `docs/UC_COMPLETION_VERIFICATION.md`
- TDD_COMPLETION_REPORT.md está en `docs/TDD_COMPLETION_REPORT.md`
- TEST_EXECUTION_ANALYSIS.md está en `docs/TEST_EXECUTION_ANALYSIS.md`

**No existen en** `OfficeAutomator.Core/` ni en `src/OfficeAutomator.Core/`

**Impacto:** Usuarios no encontrarán la documentación mencionada.

#### Error 3: Línea 259 — Referencias de documentación incorrectas

**Dice:**
```
All documentation is in the `OfficeAutomator.Core/` directory:
```

**Realidad:**
- La documentación principal está en `docs/`
- Los scripts de test están en `src/OfficeAutomator.Core/`

**Impacto:** Confusión sobre dónde buscar cada cosa.

---

## Estructura Actual (Real)

```
OfficeAutomator/
├── README.md                              ← Incorrecto (rutas/referencias erróneas)
├── OfficeAutomator.sln                    ← Correcto
├── global.json                            ← Correcto
├── Directory.Build.props                  ← DEBE mover a src/
├── nuget.config                           ← DEBE mover a src/
├── EXECUTION_GUIDE.md                     ← DEBE mover a docs/
├── TEST_EXECUTION_REPORT.md               ← DEBE mover a docs/
├── ESTRUCTURA_PROYECTO.txt                ← DEBE mover/eliminar
├── PROYECTO_ESTRUCTURA_ACTUAL.txt         ← DEBE mover/eliminar
├── SPRINT1-CORRECTION-COMPLETE.txt        ← DEBE mover/eliminar
├── fix-failing-tests.ps1                  ← DEBE mover a scripts/
│
├── docs/                                  ← Correcto
│   ├── ARCHITECTURE.md
│   ├── INDEX.md
│   ├── NAMESPACING_GUIDE.md
│   ├── PROJECT_STRUCTURE_REFERENCE.md
│   ├── TDD_COMPLETION_REPORT.md
│   ├── TESTING_SETUP.md
│   ├── TEST_EXECUTION_ANALYSIS.md
│   └── UC_COMPLETION_VERIFICATION.md
│
├── scripts/                               ← Existe pero vacío/incompleto
│   └── (debería contener fix-failing-tests.ps1)
│
├── src/                                   ← Correcto
│   ├── OfficeAutomator.Core/
│   │   ├── run-tests.sh ✓
│   │   ├── run-tests.bat ✓
│   │   ├── Classes/ (10 clases)
│   │   └── Tests/ (11 clases de test)
│   │
│   └── tests/
│       ├── PowerShell/
│       │   ├── OfficeAutomator.PowerShell.EndToEnd.Tests.ps1
│       │   └── OfficeAutomator.PowerShell.Integration.Tests.ps1
│
├── .claude/                               ← Correcto
│   ├── scripts/
│   │   ├── compare-branches.sh ✓
│   │   ├── validate-commit-message.sh ✓
│   │   └── (otros)
│   ├── references/
│   │   └── commit-message-policy.md ✓
│   └── CLAUDE.md
│
└── .thyrox/                               ← Correcto
    ├── context/
    │   ├── decisions/
    │   ├── work/                          ← AQUÍ (nuevo WP)
    │   └── ...
```

---

## Impacto de la Contaminación

### En Usuarios
1. Instrucciones en README no funciona (rutas incorrectas)
2. Documentación no se encuentra (ubicación confusa)
3. Desconfianza en la estructura del proyecto
4. Pérdida de tiempo debuggeando errores de rutas

### En CI/CD
1. Scripts podrían buscar en ubicaciones equivocadas
2. Build automation podría fallar
3. Tests no se descubren automáticamente

### En Mantenimiento
1. Difícil agregar nuevos archivos de documentación
2. Inconsistencia entre lo documentado y la realidad
3. Cargo cognitivo innecesario al navegar

---

## Scope Preliminar

### In Scope (para esta ÉPICA)
1. Limpiar el root: mover/eliminar 8 archivos
2. Corregir el README: 3 errores principales
3. Validar estructura final contra la realidad

### Out of Scope (para fases posteriores)
- Reescribir documentación existente
- Cambiar estructura de `src/` o `docs/`
- Refactoring de código C#
- Cambios en configuración de compilación

---

## Artefactos a Producir (Próximas Fases)

| Fase | Artefacto | Descripción |
|------|-----------|-------------|
| STRATEGY | solution-strategy.md | Estrategia de reorganización |
| PLAN | cleanup-plan.md | Plan con scope definitivo |
| DESIGN/SPECIFY | requirements-spec.md | Especificación exacta de cambios |
| PLAN EXECUTION | task-plan.md | Tareas atómicas (T-NNN) |
| EXECUTE | Commits + git log | Movimientos y correcciones |
| TRACK | lessons-learned.md | Qué aprendemos sobre estructura |

---

## Riesgos Identificados

| Riesgo | Probabilidad | Impacto | Mitigation |
|--------|-------------|--------|-----------|
| Perder contenido al mover archivos | Baja | Alto | Git: ver diffs antes de commit |
| Ruptura de referencias en links | Media | Medio | Revisar todos los links después |
| CI/CD no encuentra scripts | Baja | Alto | Verificar paths en settings.json antes |

---

## Próximos Pasos

1. **Phase 2: MEASURE** → Hacer baseline de la calidad actual
2. **Phase 3: DIAGNOSE** → Analizar causa raíz de la contaminación
3. **Phase 5: STRATEGY** → Diseñar estrategia de reorganización
4. **Phase 6: PLAN** → Definir scope exacto
5. **Phase 8: DECOMPOSE** → Crear tareas (T-NNN)
6. **Phase 10: EXECUTE** → Ejecutar movimientos y correcciones
7. **Phase 11: TRACK** → Validar y documentar lecciones

---

## Metadata de Análisis

- **Timestamp:** 2026-04-22 04:46:55 UTC
- **Analista:** Claude Code (THYROX Phase 1)
- **Duración análisis:** ~5 minutos
- **Archivos analizados:** 15
- **Errores encontrados:** 8 (estrutura) + 3 (documentación)
- **Severidad promedio:** MEDIUM
