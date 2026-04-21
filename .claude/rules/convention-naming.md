```yml
type: Convención de Proyecto
category: Naming y Estructura de Archivos
version: 1.0.0
purpose: Define reglas de nombres para archivos, directorios, y identifiers
updated_at: 2026-04-21 01:40:00
applies_to: OfficeAutomator v1.0.0+
```

# CONVENCIÓN 1: NAMING - Nombres de Archivos y Directorios

## Principio Core

**NO usar prefijos numéricos en nombres de archivos.**

Los números pertenecen a:
- Versiones semánticas (en YAML frontmatter)
- IDs de tracking (UC-001, ADR-001, etc. en contenido)
- Timestamps (YYYY-MM-DD-HH-MM-SS en directorios de Work Packages)

**NO pertenecen a:**
- Nombres de archivos (NO: 01-file.md, 02-file.md)
- Nombres de directorios (NO: 01-feature/, 02-feature/)

---

## Reglas de Naming

### 1. Archivos de Documentación

**Formato:** `{descripcion-en-kebab-case}.md`

Ejemplos correctos:
- `actors-stakeholders.md`
- `problem-statement.md`
- `use-case-matrix.md`
- `deployment-strategy.md`
- `security-considerations.md`

Ejemplos INCORRECTOS:
- `01-actors-stakeholders.md` ← ❌ NO prefijo numérico
- `2-problem-statement.md` ← ❌ NO prefijo numérico
- `actors_and_stakeholders.md` ← ⚠ Usar kebab-case, no snake_case

---

### 2. Architecture Decision Records (ADRs)

**Formato:** `adr-{tema-en-kebab-case}.md`

Ejemplos:
- `adr-validation-strategy.md`
- `adr-error-handling.md`
- `adr-idempotence-approach.md`
- `adr-logging-format.md`

**Ubicación:** `.thyrox/context/decisions/`

Nunca: `adr-001.md`, `adr-1-validation.md`

---

### 3. Error Tracking

**Formato:** `{descripcion-error}.md`

Ejemplos:
- `offline-download-failure.md`
- `validation-timeout.md`
- `configuration-malformed.md`

**Ubicación:** `.thyrox/context/errors/`

---

### 4. Work Packages (Directorios)

**Formato:** `YYYY-MM-DD-HH-MM-SS-{nombre-en-kebab-case}`

Ejemplos:
- `2026-04-21-01-30-00-uc-documentation`
- `2026-04-22-14-00-00-powershell-module-design`
- `2026-04-25-09-30-00-function-implementation`

**Ubicación:** `.thyrox/context/work/`

**Nota:** Timestamp OBLIGATORIO (precisión hasta segundos), nunca omitir.

---

### 5. PowerShell Functions

**Formato:** `{Verb}-{Noun}.ps1`

Ejemplos:
- `Invoke-OfficeAutomator.ps1`
- `Get-OfficeDeploymentTool.ps1`
- `Validate-Configuration.ps1`
- `Write-Log.ps1`
- `Test-Idempotence.ps1`

**Ubicación:**
- Públicas: `Functions/Public/`
- Privadas: `Functions/Private/`

---

### 6. Test Files

**Formato:** `{funcionalidad}.Tests.ps1`

Ejemplos:
- `InputValidation.Tests.ps1`
- `IntegrityCheck.Tests.ps1`
- `Installation.Tests.ps1`

**Ubicación:** `Tests/`

---

### 7. Directorios de Dominio (docs/requirements/)

**Formato:** `uc-{numero:03d}-{descripcion-en-kebab-case}/`

Ejemplos:
- `uc-001-select-version/`
- `uc-002-select-language/`
- `uc-003-exclude-applications/`
- `uc-004-validate-integrity/`
- `uc-005-install-office/`

**Nota:** Aquí SÍ usamos números porque son identificadores (UC-001 es el ID formal).

---

### 8. Archivos dentro de directorios UC

**Formato:** `{aspecto}.md`

Ejemplos en `uc-001-select-version/`:
- `overview.md`
- `happy-path.md`
- `error-scenarios.md`
- `acceptance-criteria.md`
- `dependencies.md`

**NUNCA:**
- `01-overview.md` ← ❌
- `1-happy-path.md` ← ❌

---

## Casos Edge

### Nombres muy similares
Si necesitas diferenciar archivos similares, usa prefijos conceptuales, NO números:

✓ Correcto:
- `analysis-current-state.md`
- `analysis-proposed-state.md`

❌ Incorrecto:
- `01-analysis.md`
- `02-analysis.md`

---

### Secuencia ordenada
Si documentas una secuencia de pasos, NO números en filenames:

✓ Correcto:
- `setup-environment.md`
- `configure-ide.md`
- `run-tests.md`

❌ Incorrecto:
- `01-setup.md`
- `02-configure.md`
- `03-run.md`

**Alternativa:** Usar numeración DENTRO del contenido del archivo.

---

## RESUMEN

| Tipo | Patrón | Ejemplo | Ubicación |
|------|--------|---------|-----------|
| Documentación | `{desc}.md` | `problem-statement.md` | Flexible |
| ADR | `adr-{tema}.md` | `adr-idempotence.md` | `.thyrox/context/decisions/` |
| Error | `{desc}.md` | `offline-failure.md` | `.thyrox/context/errors/` |
| Work Package | `YYYY-MM-DD-HH-MM-SS-{nombre}` | `2026-04-21-01-30-00-uc-doc` | `.thyrox/context/work/` |
| PowerShell función | `{Verb}-{Noun}.ps1` | `Get-ODT.ps1` | `Functions/Public/` |
| Test | `{func}.Tests.ps1` | `Validation.Tests.ps1` | `Tests/` |
| Directorios UC | `uc-{NNN}-{desc}/` | `uc-001-select-version/` | `docs/requirements/` |
| Archivos en UC | `{aspecto}.md` | `happy-path.md` | `uc-XXX-*/` |

---

## Verificación

Antes de commitear, verifica:

```bash
# NO debería haber archivos con estos patrones:
find . -name "0[0-9]-*.md"
find . -name "[0-9]-*.md"
find . -name "[0-9][0-9]-*.md"

# Si encuentra algo, renombra sin el prefijo numérico
mv 01-file.md file.md
```

---

## Aplicación Retroactiva

**Los 4 archivos generados en Stage 1 deben renombrarse:**

Actual → Correcto:
- `01-problem-statement.md` → `problem-statement.md`
- `02-actors-stakeholders.md` → `actors-stakeholders.md`
- `03-discovery-notes.md` → `discovery-notes.md`
- `04-use-case-matrix.md` → `use-case-matrix.md`

---

Esta convención asegura:
- Consistencia en toda la base de código
- Alineación con thyrox conventions
- Claridad: números solo donde significan IDs (UC-001, ADR-001)
- Flexibilidad: orden se maneja en contenido, no en filenames
