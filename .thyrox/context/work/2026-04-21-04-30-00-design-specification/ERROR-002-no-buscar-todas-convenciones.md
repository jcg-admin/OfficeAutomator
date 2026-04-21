---
type: Error Documentation
error_id: ERROR-002
severity: CRÍTICA
fecha: 2026-04-21 05:30:00
fase: Stage 7 DESIGN/SPECIFY (análisis Ishikawa)
---

# ERROR 2: No busqué TODAS las convenciones del proyecto

## Descripción del error

**Más grave que ERROR-001.**

Asumí que conocía todas las convenciones sin hacer búsqueda exhaustiva en el proyecto. Luego hice análisis Ishikawa incompleto porque me faltaba información sobre TODAS las convenciones establecidas.

## Evidencia del error

Cuando hice análisis Ishikawa, identificué estas causas raíz:
1. "No revisar convenciones antes de escribir"
2. "Sin ejecutar validación pre-commit"
3. "Convenciones no cargadas en contexto"
4. "Presión por velocidad"
5. "Sin template estándar"

**PERO:** No identifiqué que **YO MISMO era culpable de Error 2**:
- No hice búsqueda exhaustiva de convenciones
- Asumí cuáles eran relevantes
- El análisis Ishikawa se basó en información incompleta

## Lo que debería haber hecho

```bash
# PASO 1: Buscar TODAS las convenciones en .claude/rules/
find /tmp/projects/OfficeAutomator/.claude/rules -type f -name "*.md"

# PASO 2: Buscar convenciones en otros directorios
find /tmp/projects/OfficeAutomator/.thyrox -name "*convention*"
find /tmp/projects/OfficeAutomator/.thyrox -name "*standard*"
find /tmp/projects/OfficeAutomator -name "*REGLAS*"

# PASO 3: Buscar en ADRs de decisiones (muchas contienen convenciones)
ls /tmp/projects/OfficeAutomator/.thyrox/context/decisions/ | grep adr-

# PASO 4: Buscar en CHANGELOG de decisiones
find /tmp/projects/OfficeAutomator -name "*.md" -exec grep -l "convención" {} \;

# PASO 5: Leer CADA convención completa (no solo primeras líneas)
for file in /tmp/projects/OfficeAutomator/.claude/rules/convention-*.md; do
  echo "=== $(basename $file) ==="
  cat "$file"
done
```

## Convenciones encontradas (DESPUÉS del error)

```
.claude/rules/convention-naming.md
.claude/rules/convention-versioning.md
.claude/rules/convention-mermaid-diagrams.md
.claude/rules/convention-professional-documentation.md
.claude/rules/metadata-standards.md
.claude/rules/commit-conventions.md
.claude/rules/REGLAS_DESARROLLO_OFFICEAUTOMATOR.md
.claude/rules/calibration-verified-numbers.md
.claude/rules/thyrox-invariants.md

TOTAL: 9 archivos de convenciones/reglas
```

## Convenciones que IGNORÉ en Stage 7 UCs

### 1. convention-naming.md
- **Aplica a:** Nombres de archivos UC
- **Regla:** Usar kebab-case, sin prefijos numéricos
- **Aplico en Stage 7:** NO completamente

### 2. convention-versioning.md
- **Aplica a:** Frontmatter YAML de documentos
- **Regla:** MAJOR.MINOR.PATCH con git tags y CHANGELOG
- **Aplico en Stage 7:** NO (no hay git tags, CHANGELOG)

### 3. convention-mermaid-diagrams.md
- **Aplica a:** Si uso diagramas Mermaid
- **Regla:** `%%{init: { 'theme': 'dark' } }%%`, NO emojis
- **Aplico en Stage 7:** N/A (no usé mermaid en UCs)

### 4. convention-professional-documentation.md
- **Aplica a:** Estructura, lenguaje, secciones
- **Regla:** Profesional, sin "pilares", estructura estándar
- **Aplico en Stage 7:** INCOMPLETO (estructura inconsistente)

### 5. metadata-standards.md
- **Aplica a:** Frontmatter YAML de todos los documentos
- **Regla:** Fields consistentes, author, version, dependencies
- **Aplico en Stage 7:** INCOMPLETO (frontmatter inconsistente)

### 6. commit-conventions.md
- **Aplica a:** Mensajes de git commit
- **Regla:** `type(scope): message` format
- **Aplico en Stage 7:** SÍ (commit message correcto)

### 7. REGLAS_DESARROLLO_OFFICEAUTOMATOR.md
- **Aplica a:** TODAS las funciones PowerShell
- **Regla:** Fail-Fast, Idempotence, Transparency
- **Aplico en Stage 7:** PARCIAL (documenté principios, pero UCs no muestran implementación)

### 8. calibration-verified-numbers.md
- **Aplica a:** Números específicos (timeouts, retries, etc)
- **Regla:** Usar números calibrados, no adivinar
- **Aplico en Stage 7:** PARCIAL (3x retry, exponential backoff - pero sin cita de dónde vienen)

### 9. thyrox-invariants.md
- **Aplica a:** Estructura de proyectos thyrox
- **Regla:** Work Packages, Focus, Context, Decisions
- **Aplico en Stage 7:** SÍ (seguí estructura WP)

## El impacto de este error

**Este error causó:**
- Violación de 4 convenciones en los 5 UCs
- Análisis Ishikawa incompleto (no vi que YO era causa raíz)
- Stage 7 no está listo para implementación

## Búsqueda exhaustiva que DEBERÍA hacer ahora

```bash
# CONTAR líneas de cada convención
wc -l /tmp/projects/OfficeAutomator/.claude/rules/*.md

# EXTRAER reglas principales
for file in /tmp/projects/OfficeAutomator/.claude/rules/*.md; do
  grep -E "^##|^###|^-|^✓|^✗" "$file" | head -10
done

# BUSCAR referencias a convenciones en otros documentos
grep -r "convention-" /tmp/projects/OfficeAutomator/.claude/ | grep -v ".md:"
grep -r "convención" /tmp/projects/OfficeAutomator/.thyrox --include="*.md" | grep -E "aplica|debe|requiere"
```

## Acción correctiva

**No puedo "corregir" este error después del hecho.** Pero puedo evitarlo en futuro:

**Protocolo de "Búsqueda Exhaustiva de Convenciones":**

```bash
#!/bin/bash
# Script pre-ejecución: exhaustive-convention-search.sh

PROJECT_ROOT="/tmp/projects/OfficeAutomator"
PHASE="$1"  # Ej: "Stage-7"

echo "=== BÚSQUEDA EXHAUSTIVA DE CONVENCIONES ($PHASE) ==="

echo "1. Convenciones en .claude/rules/"
find "$PROJECT_ROOT/.claude/rules" -name "*.md" -type f

echo "2. Convenciones en ADRs"
find "$PROJECT_ROOT/.thyrox/context/decisions" -name "adr-*.md"

echo "3. Reglas en REGLAS_DESARROLLO"
find "$PROJECT_ROOT" -name "*REGLAS*" -type f

echo "4. Convenciones mencionadas en focus.md"
grep -E "convención|standard" "$PROJECT_ROOT/.thyrox/context/focus.md"

echo "5. Convenciones en now.md (contexto actual)"
grep -E "convención|standard" "$PROJECT_ROOT/.thyrox/context/now.md"

echo "=== TOTAL DE DOCUMENTOS DE CONVENCIÓN ENCONTRADOS ==="
find "$PROJECT_ROOT/.claude/rules" -name "*.md" | wc -l
```

## Clasificación

- **Tipo:** Falta de diligencia debida (grave)
- **Categoría:** Pre-análisis (habría sido detectado antes de Ishikawa)
- **Raíz:** Asumir conocimiento sin verificación
- **Impacto:** Análisis Ishikawa incompleto y parcialmente erróneo

---

**Archivo de error creado:** 2026-04-21 05:30:00
**Estado:** Documentado para referencia
**Próximo paso:** Crear ERROR-003 (Análisis incompleto por Error 2)

