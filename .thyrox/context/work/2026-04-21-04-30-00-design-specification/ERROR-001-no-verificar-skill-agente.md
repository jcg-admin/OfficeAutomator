---
type: Error Documentation
error_id: ERROR-001
severity: ALTA
fecha: 2026-04-21 05:25:00
fase: Stage 7 DESIGN/SPECIFY
---

# ERROR 1: No verifiqué skill vs agente diagrama-ishikawa

## Descripción del error

Asumí que `diagrama-ishikawa` era un **agente** (.claude/agents/) sin verificar si también existe como **skill** (.claude/skills/).

## Lo que debería haber hecho

```bash
# PASO 1: Verificar existencia de agente
find /tmp/projects/OfficeAutomator -name "*ishikawa*"
# Resultado: .claude/agents/diagrama-ishikawa.md ✓

# PASO 2: Verificar existencia de skill (FALTÓ ESTE PASO)
find /tmp/projects/OfficeAutomator/.claude/skills -name "*ishikawa*"
# Debería haber checado

# PASO 3: Leer AMBOS para entender la diferencia
view /tmp/projects/OfficeAutomator/.claude/agents/diagrama-ishikawa.md
view /tmp/projects/OfficeAutomator/.claude/skills/NOMBRE_DEL_SKILL/ishikawa.md (si existe)

# PASO 4: Leer .thyrox/registry/agents/diagrama-ishikawa.yml
view /tmp/projects/OfficeAutomator/.thyrox/registry/agents/diagrama-ishikawa.yml
```

## Lo que pasó

- ✓ Encontré el agente en `.claude/agents/`
- ✗ NO busqué en `.claude/skills/` (FALLO)
- ✗ NO consulté el registro en `.thyrox/registry/agents/` (FALLO)
- ✗ Asumí que entendía cómo usar el agente sin leer toda su documentación
- ✓ SÍ leí la documentación del agente (buena práctica)

## Convención violada

**convention-professional-documentation.md** → "Verifica completitud antes de ejecutar"

No verifiqué:
- [ ] ¿Existe como skill?
- [ ] ¿Está registrado en .thyrox/registry/?
- [ ] ¿Hay dependencias de otro skill?
- [ ] ¿Hay requisitos previos?

## Impacto

- **Bajo:** No afectó el resultado (el análisis se ejecutó)
- **Medio:** Podría haber faltado contexto importante del skill
- **Metodología:** Violó el principio de "verificación exhaustiva"

## Lectura preventiva requerida

```
.thyrox/registry/agents/diagrama-ishikawa.yml
.claude/agents/diagrama-ishikawa.md (ya leído)
Buscar en .claude/skills/ por "ishikawa" (PENDIENTE)
```

## Acción correctiva

Antes de usar cualquier agente o skill, ejecutar:

```bash
# Script de verificación
echo "=== BUSCANDO AGENTE ==="
find /tmp/projects/OfficeAutomator/.claude/agents -name "*{nombre}*"

echo "=== BUSCANDO SKILL ==="
find /tmp/projects/OfficeAutomator/.claude/skills -name "*{nombre}*"

echo "=== VERIFICANDO REGISTRY ==="
find /tmp/projects/OfficeAutomator/.thyrox/registry -name "*{nombre}*"

echo "=== LEYENDO DOCUMENTACIÓN ==="
# Leer TODOS los resultados anteriores
```

## Clasificación

- **Tipo:** Falta de diligencia debida
- **Categoría:** Pre-ejecución (habría sido detectado en checklist)
- **Raíz:** No ejecutar búsqueda exhaustiva antes de usar herramientas

---

**Archivo de error creado:** 2026-04-21 05:25:00
**Estado:** Documentado para referencia
**Próximo paso:** Crear ERROR-002

