# Commit Message Policy — OfficeAutomator

```yml
version: 1.0.0
created_at: 2026-04-22
applies_to: All commits in OfficeAutomator
validation_hook: .claude/scripts/validate-commit-message.sh
updated_at: 2026-04-22
```

## Principio

Los commits deben demostrar claramente qué cambios se realizaron. El objetivo es que cada commit sea autodescriptivo y útil para entender el historial del proyecto.

## Requisitos Mínimos

1. **Longitud:** Mínimo 10 caracteres
2. **Contenido:** Debe describir QUÉ cambió y POR QUÉ (o para QUÉ)
3. **Claridad:** Debe ser legible y específico
4. **Sin prefijos obligatorios:** No es necesario usar `feat(scope):`, `fix(scope):`, etc. al inicio

## Formatos Aceptados

Todos estos formatos son válidos:

### Formato 1: Convencional Commit
```
type(scope): description
```

Ejemplos:
```
feat(script): add compare-branches.sh utility for branch comparison
fix(validation): correct language compatibility check
docs(readme): update installation instructions
test(core): add E2E integration tests
```

### Formato 2: Descriptivo Simple
```
Description of what changed and why
```

Ejemplos:
```
Update scripts and validation infrastructure for project structure reorganization
Add Test Execution Report and NuGet configuration
Fix 4 failing tests and DLL loader path resolution
```

### Formato 3: Tipo Mayúscula + Descripción
```
TYPE: Description of changes
```

Ejemplos:
```
DOCS: Add Deep Review documentation for Scripts 2 and 5 validation
CONFIG: Centralize bin/ and obj/ to project root via Directory.Build.props
FIX: Phase 5 Testing - Fix 4 failing tests and DLL loader path resolution
FEAT: Add E2E Integration Tests - Complete Workflow Validation
```

## Lo Importante

El commit demuestre:

1. **QUE cambió** — qué archivos, funciones, componentes
2. **POR QUE cambió** — razón, propósito, contexto
3. **COMO afecta al proyecto** — si es breaking, si añade feature, si es fix

Ejemplo completo (con descripción multilinea):

```
FIX: Script 6 (Execution.RollbackHandler) - Correct Issues Found by Deep-Review

- Fixed error handling in rollback logic
- Improved logging output for debugging
- Added validation for system state before rollback
- Updated tests to cover new edge cases

Fixes issue with incomplete rollback in failed installations.
Ensures idempotency guarantee is maintained.
```

## Validación

El hook en `.claude/scripts/validate-commit-message.sh` verifica:

- Mínimo 10 caracteres descriptivos
- Contiene al menos caracteres alfanuméricos
- No es vacio ni solo whitespace

El hook NO verifica:

- Prefijo obligatorio (`feat|fix|docs|etc`)
- Formato de paréntesis `(scope)`
- Orden de palabras

## Uso en Diferentes Contextos

| Contexto | Patrón | Ejemplo |
|----------|--------|---------|
| Feature nueva | Descriptivo | Add E2E Integration Tests for Office installation |
| Bug fix | Descriptivo con contexto | Fix language compatibility validation in UC-004 |
| Documentacion | Descriptivo | Update ARCHITECTURE.md with new module structure |
| Refactoring | Descriptivo | Reorganize project structure - centralize bin/obj directories |
| Tests | Descriptivo | Add comprehensive test suite for ConfigValidator |
| Chore | Descriptivo | Update NuGet packages and rebuild configuration |

## Ejemplo Real del Proyecto

```
feat(scripts): add compare-branches.sh utility for branch comparison

Add robust bash script to compare two git branches without errors.
Automatically fetches missing branches from remote and provides
detailed output including commit count, file statistics, and history.

Handles local/remote branch resolution transparently. Output includes:
- Commits adelante count
- File change statistics
- List of modified files (sorted)
- Full commit history with graph

Usage:
  bash .claude/scripts/compare-branches.sh main HEAD
  bash .claude/scripts/compare-branches.sh main feature/branch-name
  bash .claude/scripts/compare-branches.sh develop stage-10
```

## Lo Que NO Debes Hacer

| No hagas | Razón |
|----------|-------|
| `git commit -m "update"` | Demasiado genérico, no describe cambios |
| `git commit -m "fix stuff"` | Impreciso, no especifica qué se rompió |
| `git commit -m "WIP"` | Work In Progress - no es descriptor |
| `git commit -m "as"` | Muy corto, no descriptivo |
| `git commit -m ""` | Vacio - sin información |

## Cambios en Esta Política

La política anterior requería estrictamente `type(scope): description` con tipos predefinidos (I-005).

Cambio realizado 2026-04-22:
- Se flexibilizó la validación para permitir múltiples formatos
- Se enfatiza que lo importante es que los commits demuestren cambios
- Se mantiene la claridad y descriptibilidad como requisito mínimo
- No se eliminó, se facilitó el uso de convenciones

Motivo: Los commits del proyecto ya demostraban que existen múltiples estilos válidos, todos descriptivos y útiles. La validación rígida estaba frenando la productividad sin agregar valor real.

## Referencia

- Validador: `.claude/scripts/validate-commit-message.sh`
- Convenciones generales: `.claude/rules/commit-conventions.md`
- Historial: `git log --oneline` para ver ejemplos reales
