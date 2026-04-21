```yml
type: Convención de Proyecto
category: Versionado Semántico
version: 1.0.0
purpose: Define esquema de versionado X.Y.Z para código y documentación
updated_at: 2026-04-21 01:40:00
applies_to: OfficeAutomator v1.0.0+
spec: https://semver.org/
```

# CONVENCIÓN 2: SEMANTIC VERSIONING - Control de Versiones

## Principio Core

**OfficeAutomator usa Semantic Versioning 2.0.0 (semver)**

Formato: `MAJOR.MINOR.PATCH`

Ejemplos:
- `1.0.0` - Primera release
- `1.1.0` - Nuevas características
- `1.1.1` - Bug fix
- `2.0.0` - Breaking changes

---

## Definiciones (Semver 2.0.0)

### MAJOR (Primera cifra)
Incrementa cuando hay **cambios incompatibles** (breaking changes).

Ejemplos de breaking changes:
- Cambiar firma de función pública (parámetros/retorno)
- Remover funcionalidad existente
- Cambiar comportamiento fundamental de UC

**Incrementa:** `1.0.0` → `2.0.0`

### MINOR (Segunda cifra)
Incrementa cuando se agregan **nuevas características compatibles** (backward-compatible).

Ejemplos:
- Nuevo UC
- Nueva versión de Office soportada
- Nuevo idioma soportado

**Incrementa:** `1.0.0` → `1.1.0`
**Se reinicia:** `1.1.0` → `2.0.0`

### PATCH (Tercera cifra)
Incrementa por **bug fixes y mejoras** sin nuevas características.

Ejemplos:
- Corregir validación fallida
- Mejorar logging
- Optimizar descargas
- Mejorar mensajes de error

**Incrementa:** `1.0.0` → `1.0.1`
**Se reinicia:** `1.0.1` → `1.1.0` o `2.0.0`

---

## Aplicación en OfficeAutomator

### Archivos que llevan versión semántica

**En frontmatter YAML:**

```yaml
version: 1.0.0
```

Archivos con versión:
- `README.md`
- `CHANGELOG.md`
- Todos los archivos en `.claude/` (CLAUDE.md, ARCHITECTURE.md, skills/*/SKILL.md)
- Documentación en `docs/`
- `.claude/rules/*`
- `.claude/references/*`
- `.thyrox/context/project-state.md`

**NO llevan versión:**
- Archivos en Work Packages (`.thyrox/context/work/*/`)
- Logs
- Archivos temporales
- Artefactos de sesión

---

## Ciclo de versiones en OfficeAutomator

### Fase 1: Documentación (v1.0.0-alpha)

```
v1.0.0-alpha.1 — Estructura base + discovery de UCs
v1.0.0-alpha.2 — Documentación de UCs completa
v1.0.0-alpha.3 — Criterios de aceptación definidos
```

### Fase 2: Implementación (v1.0.0-beta)

```
v1.0.0-beta.1 — Funciones básicas implementadas
v1.0.0-beta.2 — Validación funcionando
v1.0.0-beta.3 — Instalación funcionando
v1.0.0-beta.4 — Tests pasando
```

### Fase 3: Release (v1.0.0)

```
v1.0.0 — Release estable
v1.0.1 — Bug fix
v1.1.0 — Nuevo idioma soportado
v1.1.1 — Mejora de logging
v2.0.0 — Cambio arquitectónico mayor
```

---

## Relación: Version en YAML vs Git Tags

| Lugar | Formato | Significado | Ejemplo |
|-------|---------|------------|---------|
| Frontmatter YAML | `version: X.Y.Z` | Versión del documento/código | `version: 1.0.0` |
| Git tag | `v1.0.0` | Release en repositorio | `git tag v1.0.0` |
| CHANGELOG.md | `## [1.0.0] - YYYY-MM-DD` | Historiales de releases | `## [1.0.0] - 2026-05-01` |

**Regla:** Versión en YAML y Git tag DEBEN coincidir.

---

## Incremento de versión por tipo de work

| Tipo de Trabajo | Incremento | Ejemplo |
|-----------------|-----------|---------|
| Documentación de UC | MINOR | `1.0.0` → `1.1.0` |
| Bug fix en validación | PATCH | `1.0.0` → `1.0.1` |
| Nueva función PowerShell | MINOR | `1.0.0` → `1.1.0` |
| Redesign de arquitectura | MAJOR | `1.0.0` → `2.0.0` |
| Optimización sin cambios API | PATCH | `1.0.0` → `1.0.1` |
| Cambio de parámetro función | MAJOR | `1.0.0` → `2.0.0` |

---

## Pre-release y Build Metadata (opcional)

### Pre-release (versiones de prueba)

Formato: `X.Y.Z-{identificador}`

Ejemplos:
- `1.0.0-alpha` - Muy temprano
- `1.0.0-alpha.1` - Secuencia de alphas
- `1.0.0-beta` - Beta testing
- `1.0.0-rc.1` - Release candidate

**Regla:** Pre-releases no se suben a producción.

### Build metadata (información extra, opcional)

Formato: `X.Y.Z+{metadata}`

Ejemplos:
- `1.0.0+build.20260421` - Información de compilación
- `1.0.0+git.a1b2c3d` - Hash de git

---

## Flujo de Incremento Típico

```
1.0.0 (Release estable)
  ↓ (bug fix)
1.0.1 (Patch release)
  ↓ (bug fix)
1.0.2 (Patch release)
  ↓ (nueva característica)
1.1.0 (Minor release - reset patch a 0)
  ↓ (nueva característica)
1.2.0 (Minor release)
  ↓ (breaking change)
2.0.0 (Major release - reset minor y patch a 0)
```

---

## Cambios de versión en OfficeAutomator

### Cuándo incrementar MAJOR

- Cambiar firma de función pública
- Remover UC completamente
- Cambiar flujo fundamental de instalación
- Incompatibilidad con versiones anteriores de Office

### Cuándo incrementar MINOR

- Agregar nuevo UC
- Agregar soporte para nuevo idioma
- Agregar nueva versión de Office
- Nueva funcionalidad compatible

### Cuándo incrementar PATCH

- Corregir bug en validación
- Mejorar manejo de errores
- Optimizar performance
- Mejorar logging/mensajes
- Actualizar documentación (sin cambios API)

---

## Archivo CHANGELOG.md

Ejemplo formato (Keep a Changelog):

```markdown
# CHANGELOG - OfficeAutomator

## [1.0.0] - 2026-05-01

### Added
- Soporte para Office LTSC 2024
- Soporte para Office LTSC 2021
- Soporte para Office LTSC 2019
- 5 Use Cases documentados y funcionales
- GUI WPF
- Validación SHA256
- Idempotencia garantizada

### Fixed
- N/A (primera release)

### Changed
- N/A (primera release)

## [0.9.0-beta.1] - 2026-04-25

### Added
- Validación de input completa
- Tests básicos
- Documentación de UCs

---
```

---

## Commits y Versión

**Convención:** Los commits incrementan versión automáticamente basado en tipo:

```bash
feat(requirements): agregar UC-006    # → MINOR
fix(validation): corregir idioma      # → PATCH
docs: actualizar README               # → PATCH (sí, docs también)
refactor!: cambiar arquitectura       # → MAJOR (! indica breaking)
```

---

## Verificación de Versión

Antes de release, verificar:

```bash
# 1. YAML version
grep "^version:" .claude/CLAUDE.md

# 2. CHANGELOG actualizado
head -20 CHANGELOG.md

# 3. Git tag alineado
git tag | grep v1.0.0

# 4. README menciona versión
grep version README.md
```

---

## Resumen

**Semantic Versioning en OfficeAutomator:**
- X = Breaking changes
- Y = Nuevas características
- Z = Bug fixes
- Vive en frontmatter YAML: `version: X.Y.Z`
- Sincronizado con Git tags: `vX.Y.Z`
- Documentado en CHANGELOG.md

**Resultado:** Control claro de qué cambió en cada versión.

