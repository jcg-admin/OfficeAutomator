```yml
created_at: 2026-04-22 04:46:55
project: OfficeAutomator
work_package: 2026-04-22-04-46-55-project-structure-cleanup
updated_at: 2026-04-22 04:46:55
```

# Risk Register — Project Structure Cleanup

## Risk Summary

| Risk ID | Title | Probability | Impact | Status |
|---------|-------|-------------|--------|--------|
| R-001 | Pérdida de contenido al mover archivos | Baja | Alto | MONITORED |
| R-002 | Ruptura de referencias internas en markdown | Media | Medio | MONITORED |
| R-003 | Scripts CI/CD buscan rutas antiguas | Baja | Alto | MITIGATED |
| R-004 | Usuario confundido por cambio de estructura | Media | Bajo | MITIGATED |

---

## Riesgos Detallados

### R-001: Pérdida de contenido al mover archivos

**Descripción:**
Al mover 8 archivos del root a `docs/` o `scripts/`, existe riesgo de que el contenido se pierda o se quede incompleto.

**Probabilidad:** Baja (Git garantiza recuperabilidad)
**Impacto:** Alto (Pérdida de documentación importante)

**Causas:**
- Comando incorrecto (ej: `mv` en lugar de git rename)
- Exclusión accidental en .gitignore
- Conflicto de merge no detectado

**Mitigación:**
- ✓ Usar `git mv` en lugar de comandos del SO
- ✓ Verificar con `git status` después de cada movimiento
- ✓ Revisar diffs antes de hacer commit

**Detección:**
- Verificar que los archivos aparezcan en `git status` como "renamed"
- Confirmar tamaño de archivo después del movimiento

**Dueño:** Claude Code / Nestor Monroy
**Monitoreo:** Phase 8-10 (PLAN EXECUTION → EXECUTE)

---

### R-002: Ruptura de referencias internas en markdown

**Descripción:**
Archivos en `docs/` o README.md pueden contener referencias relativas a otros documentos que se rompan después de mover archivos.

**Probabilidad:** Media (Varios markdown vinculados)
**Impacto:** Medio (Links rotos, documentación confusa)

**Causas:**
- Links relativos con rutas hardcoded (ej: `../EXECUTION_GUIDE.md`)
- Referencias en índices (INDEX.md)
- Links en README.md

**Mitigación:**
- ✓ Buscar todos los links antes de mover (grep/find)
- ✓ Usar rutas relativas correctas (`docs/...`)
- ✓ Actualizar INDEX.md si existe
- ✓ Validar links después de mover (checker de markdown)

**Detección:**
- Validador de markdown: buscar links rotos
- Revisión manual de archivos que contienen referencias cruzadas

**Dueño:** Claude Code
**Monitoreo:** Phase 8 (PLAN EXECUTION) — previo a EXECUTE

---

### R-003: Scripts CI/CD buscan rutas antiguas

**Descripción:**
Si existen pipelines CI/CD (.github/workflows/, .gitlab-ci.yml, etc.) que referencian rutas antiguas de archivos/scripts, fallarán después del movimiento.

**Probabilidad:** Baja (Proyecto es local, no tiene CI/CD activo aún)
**Impacto:** Alto (Build fallido, tests no ejecutados)

**Causas:**
- Referencias hardcoded en scripts de CI/CD
- Paths en settings.json (Claude Code) no actualizados
- Configuración de hooks no sincronizada

**Mitigación:**
- ✓ Buscar referencias en `.github/workflows/` (si existen)
- ✓ Revisar `.claude/settings.json` antes de mover
- ✓ Actualizar paths en configuración de Claude Code si es necesario
- ✓ Probar scripts localmente después del movimiento

**Detección:**
- Ejecutar scripts de test después de cada movimiento
- Verificar que `run-tests.sh` y `run-tests.bat` sigan funcionando

**Dueño:** Claude Code
**Monitoreo:** Phase 9 (PILOT/VALIDATE)

---

### R-004: Usuario confundido por cambio de estructura

**Descripción:**
El cambio en la estructura puede confundir a usuarios que esperaban encontrar archivos en ubicaciones antiguas (especialmente si clonaron el repo antes).

**Probabilidad:** Media (Documentación deficiente)
**Impacto:** Bajo (Inconveniente temporal)

**Causas:**
- README no se actualiza adecuadamente
- Falta de CHANGELOG explicando el cambio
- Documentación antigua cacheada

**Mitigación:**
- ✓ Actualizar README.md completamente
- ✓ Agregar entrada en CHANGELOG.md si existe
- ✓ Crear archivo MIGRATION.md o nota en docs/INDEX.md
- ✓ Comentarios claros en el commit

**Detección:**
- Revisar que README sea coherente con estructura real
- Verificar que INDEX.md (si existe) esté actualizado

**Dueño:** Claude Code
**Monitoreo:** Phase 11 (TRACK/EVALUATE)

---

## Risk Matrix

```
        BAJO         MEDIO        ALTO
BAJO    ✓            R-004        —
MEDIO   —            R-002        —
ALTO    —            —            R-001, R-003
```

**Riesgos críticos (ALTO impacto):** R-001, R-003
**Riesgos atentos (MEDIO impacto):** R-002
**Riesgos monitor (BAJO impacto):** R-004

---

## Plan de Mitigación por Fase

| Fase | Riesgo | Acción |
|------|--------|--------|
| DISCOVER | Todos | Identificación y documentación |
| STRATEGY | R-001, R-002, R-003 | Diseñar estrategia de movimiento |
| PLAN | Todos | Planificar validaciones |
| DESIGN | R-002, R-003 | Especificar búsquedas y reemplazos |
| PLAN EXECUTION | Todos | Preparar tareas y scripts |
| EXECUTE | R-001, R-003 | Validar después de cada movimiento |
| PILOT | R-002, R-003 | Validar links y scripts |
| TRACK | R-004 | Documentar cambios para usuarios |

---

## Monitoreo Continuo

**Fecha de revisión:** Cada fase terminal (STRATEGY, PLAN, EXECUTE, TRACK)

**Criterios de escalación:**
- Si algún archivo se pierde → ESCALADA INMEDIATA
- Si más de 3 links rotos → Pausar EXECUTE, revisar
- Si CI/CD falla → Pausar EXECUTE, revisar paths

**Propietario de riesgos:** Claude Code
**Frecuencia de revisión:** 1x por fase
