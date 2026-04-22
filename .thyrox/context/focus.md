```yml
type: Dirección de Trabajo
updated_at: 2026-04-22 05:10:00
project: OfficeAutomator
```

# FOCUS.md - Dirección Actual de Trabajo

## Work Package Actual

**ÉPICA N: Project Structure Cleanup & Validation**

**WP ID:** 2026-04-22-04-46-55-project-structure-cleanup

**Estado:** Phase 1 DISCOVER - COMPLETADO

---

## Problema Identificado

- Root directory contaminado con 8 archivos fuera de lugar
- README.md con 3 errores de rutas + 8 emojis (violación de convención)
- 72 work packages en .thyrox/context/work/: 59 del framework THYROX + 13 del proyecto
- Necesario: limpiar root, corregir docs, eliminar WPs históricos del framework

---

## Análisis Completado en Phase 1 DISCOVER

1. **project-structure-cleanup-analysis.md** ✓
   - 8 archivos a mover/eliminar
   - 3 errores en README
   - 15 archivos analizados

2. **Deep-Review 1: Validación Adversarial** ✓
   - Corrigió 4 decisiones críticas
   - Directory.Build.props → MANTENER en root (no mover)
   - nuget.config → MANTENER en root (no mover)
   - fix-failing-tests.ps1 → ELIMINAR (no mover a scripts/)
   - .txt files → ARCHIVAR en WPs (no eliminar)

3. **Risk Register** ✓
   - 4 riesgos identificados con mitigaciones
   - R-001: Pérdida de contenido (Baja probabilidad, Alto impacto)
   - R-002: Ruptura de referencias (Media probabilidad, Medio impacto)
   - R-003: CI/CD paths (Baja probabilidad, Alto impacto)
   - R-004: Usuario confundido (Media probabilidad, Bajo impacto)

4. **Final Corrected Decisions** ✓
   - 8 decisiones de archivo (root cleanup)
   - 66 WPs a ELIMINAR vs 6 WPs a MANTENER
   - Corte en: 2026-04-21-01-30-00-uc-documentation (inicio del proyecto)
   - 4 correcciones en README
   - Orden de ejecución para Phase 8

---

## Decisiones Finalizadas

### Root Directory (8 cambios)

| Archivo | Decisión | Razón |
|---------|----------|-------|
| EXECUTION_GUIDE.md | Mover a docs/ | Documentación proyecto |
| TEST_EXECUTION_REPORT.md | Mover a docs/ | Reportes testing |
| ESTRUCTURA_PROYECTO.txt | Mover a WP histórico | Archivar con trazabilidad |
| PROYECTO_ESTRUCTURA_ACTUAL.txt | Mover a WP histórico | Snapshot histórico |
| SPRINT1-CORRECTION-COMPLETE.txt | Mover a WP histórico | Cierre de sprint |
| fix-failing-tests.ps1 | ELIMINAR | Script temporal sin referencias |
| Directory.Build.props | MANTENER en root | Estándar .NET, infraestructura |
| nuget.config | MANTENER en root | Config NuGet repositorio-wide |

### Work Packages (72 → 6)

**A ELIMINAR: 66 WPs**
- 59 THYROX framework (2026-03-27 a 2026-04-20)
- 1 test WP (test-pdca-worktree)
- 2 design-specification abortadas
- 4 option-b analysis

**A MANTENER: 6 WPs**
1. 2026-04-21-01-30-00-uc-documentation
2. 2026-04-21-03-00-00-scope-definition
3. 2026-04-21-06-15-00-design-specification-correct
4. 2026-04-21-11-15-00-project-structure-reorganization
5. 2026-04-22-04-46-55-project-structure-cleanup (actual)
6. (+1 futuro si hay más fases)

### README.md (4 cambios)

1. Línea 31, 35: `OfficeAutomator/OfficeAutomator.Core` → `src/OfficeAutomator.Core`
2. Líneas 126-129: `OfficeAutomator.Core/` → `docs/`
3. Línea 259: "All documentation..." → "Documentation is in docs/"
4. 8 emojis checkmark removidos

---

## Próximos Pasos

**Phase 8: PLAN EXECUTION** - Crear tareas atómicas (T-NNN)
- Mover archivos a docs/
- Mover .txt a WPs históricos
- Eliminar fix-failing-tests.ps1
- Corregir README.md
- Eliminar 66 WPs
- Validar estructura final

**Estimado:** 1-2 horas de ejecución

---

## Riesgos (Mitigados)

TODOS los riesgos de ejecución son BAJOS gracias a git:
- Pérdida de contenido: Git preserva todo
- Ruptura de links: Búsqueda exhaustiva incluida
- CI/CD paths: Scripts validados post-movimiento
- Usuario confundido: README actualizado

**Status:** LISTO PARA PHASE 8 PLAN EXECUTION

