```yml
type: Quick Start
version: 1.0
project: OfficeAutomator
updated_at: 2026-04-21
scope: Primeros 30 minutos con OfficeAutomator en Claude Code
```

# QUICK START: OfficeAutomator en Claude Code (30 minutos)

## Minuto 0-5: Setup inicial

```bash
# 1. Abre Claude Code
claude

# 2. Dentro de Claude Code, ejecuta:
/skill thyrox

# O simplemente di:
"Activar skill thyrox para OfficeAutomator"
```

Claude Code automáticamente:
- Carga `.claude/CLAUDE.md`
- Carga `.claude/skills/thyrox/SKILL.md`
- Lee `.thyrox/context/now.md`
- Muestra estado actual

---

## Minuto 5-10: Entender dónde estamos

Di en Claude Code:

```
Muéstrame el estado actual del proyecto
```

Claude Code responderá algo como:

```
OfficeAutomator v1.0.0 - Estado Actual

Última sesión: Estructura creada (2026-04-21)
Stage actual: Pendiente (estructura sin épicas aún)
Work package: Ninguno activo

Próximos pasos:
1. Crear épica de Documentación de UC
2. Documentar UC-001 a UC-005
3. Crear módulo PowerShell

Recomendación: Iniciar Stage 1 DISCOVER
```

---

## Minuto 10-20: Iniciar primera tarea

Di en Claude Code:

```
Voy a documentar los Use Cases de OfficeAutomator.
5 casos: seleccionar versión, idioma, excluir apps, validar integridad, instalar Office.
Empecemos.
```

Claude Code:

1. Activa automáticamente Stage 1: DISCOVER
2. Crea Work Package:
   ```
   .thyrox/context/work/2026-04-21-XX-XX-XX-uc-documentation/
   ```

3. Muestra guía de trabajo:
   ```
   STAGE 1: DISCOVER
   
   Objetivos:
   - Entender qué son los Use Cases en OfficeAutomator
   - Identificar actores, precondiciones, flujos
   - Definir scope de documentación
   
   Deliverables esperados:
   - Descripción de cada UC (5 total)
   - Actores involucrados
   - Precondiciones y postcondiciones
   - Casos de éxito y error
   
   Paso 1: ¿Cuáles son los actores principales?
   ```

4. Sigue el flujo interactivo del SKILL.md

---

## Minuto 20-25: Completar tarea

Después de seguir Stage 1, Claude Code dirá:

```
Stage 1 DISCOVER - COMPLETADO

Artefactos generados:
- discovery-notes.md
- actors-analysis.md
- scope-definition.md

Próximo: Stage 6 SCOPE (definir alcance detallado)

¿Continuar a Stage 6 o hacer commit primero?
```

Responde:

```
Hacer commit con los artefactos de Stage 1
```

Claude Code:

```
Commits creados:
- docs(requirements): agregar análisis de actores
- docs(requirements): definir scope de UC
- docs(requirements): notas de descubrimiento

Archivos modificados:
  .thyrox/context/work/.../discovery-notes.md
  .thyrox/context/work/.../actors-analysis.md
  ROADMAP.md
  .thyrox/context/now.md

¿Continuar a Stage 6 SCOPE?
```

---

## Minuto 25-30: Seguir al siguiente stage

Responde:

```
Sí, Stage 6 SCOPE
```

Claude Code carga Stage 6:

```
STAGE 6: SCOPE - Definir alcance y límites

Objetivos en este stage:
- Definir qué UC se documentarán completamente
- Qué está IN-SCOPE, qué OUT-OF-SCOPE
- Crear matriz de UC
- Definir orden de documentación

Archivo: docs/requirements/use-cases-matrix.md

¿Comenzamos?
```

---

## Resultado después de 30 minutos

**Sin hacer nada manualmente:**

- Work Package creado: `.thyrox/context/work/2026-04-21-XX-XX-XX-uc-documentation/`
- Estado guardado: `.thyrox/context/now.md`
- Commits hechos automáticamente
- Archivos documentados parcialmente
- ROADMAP.md actualizado
- Listo para Stage 7 en la próxima sesión

**Para continuar en próxima sesión:**

```
claude
/status

[Claude Code muestra:]
"Última sesión: UC discovery completado. Stage 6 SCOPE iniciado."
```

---

## Comandos que usaste sin saberlo

Durante esos 30 minutos, Claude Code ejecutó:

- `/skill thyrox` - Activar motor de trabajo
- `/status` - Ver estado (implícito al inicio)
- `/thyrox:discover` - Stage 1
- `/git commit` - Commits automáticos
- `/thyrox:plan` - Stage 6 (el /thyrox:plan es Scope)

**Nunca escribiste los comandos — Claude Code los ejecutó automáticamente basado en el flujo del SKILL.md**

---

## Próximas sesiones: cómo continuar

### Sesión 2: Completar documentación de UC

```
claude

[Claude Code carga estado anterior]

"Documentar UC-001 a UC-005 completamente"

[Continúa desde Stage 7: DESIGN/SPECIFY]
[Sigue hasta Stage 11: TRACK/EVALUATE]
[Commits automáticos en cada UC]
```

### Sesión 3: Implementar módulo PowerShell

```
claude

[Claude Code carga estado]

"Empezar implementación del módulo PowerShell"

[Nueva épica, nuevo Work Package]
[Stages 1-12 para código]
[Estructura Functions/Public, Functions/Private]
[Tests automáticos]
```

### Sesión 4: Release v1.0.0

```
claude

"Preparar release v1.0.0"

[Stages de finalización]
[CHANGELOG.md automático]
[GitHub release]
[Versionado]
```

---

## Estructura de archivos después de 30 minutos

```
OfficeAutomator/
├── .thyrox/context/work/
│   └── 2026-04-21-XX-XX-XX-uc-documentation/
│       ├── discovery-notes.md         (Stage 1)
│       ├── actors-analysis.md         (Stage 1)
│       ├── scope-definition.md        (Stage 6 - en progreso)
│       └── artifacts/
│
├── docs/requirements/
│   ├── use-cases-matrix.md            (parcial - Stage 6)
│   ├── uc-001-select-version/
│   ├── uc-002-select-language/
│   ├── uc-003-exclude-applications/
│   ├── uc-004-validate-integrity/
│   └── uc-005-install-office/
│
├── .thyrox/context/
│   ├── now.md                         (actualizado)
│   ├── focus.md                       (actualizado)
│   └── decisions/                     (vacío - sin decisiones aún)
│
└── ROADMAP.md                         (actualizado)
```

---

## Lo que NO hiciste manualmente

- No creaste directorios a mano
- No escribiste archivos desde cero
- No hiciste commits manualmente
- No editaste ROADMAP.md
- No actualizaste .thyrox/context/now.md
- No planificaste stages

**Todo sucedió automáticamente siguiendo el SKILL.md**

---

## Próxima sesión: cómo reanudar

```
claude

[Automáticamente Claude Code]
[Lee .thyrox/context/now.md]
[Muestra estado previo]
[Te posiciona en Stage 6]

"¿Continuamos con Stage 6 SCOPE?"

Sí, continúa

[Carga el contenido parcial de Stage 6]
[Muestra qué falta hacer]
[Sigue fluyendo...]
```

---

## Cheat Sheet de Comandos

| Comando | Qué hace |
|---------|----------|
| `/status` | Mostrar estado actual |
| `/focus` | Ver dirección del trabajo |
| `/now` | Ver archivo now.md |
| `/thyrox:discover` | Activar Stage 1 |
| `/thyrox:design` | Activar Stage 7 |
| `/thyrox:decompose` | Activar Stage 8 |
| `/thyrox:execute` | Activar Stage 10 |
| `/thyrox:track` | Activar Stage 11 |
| `/thyrox:standardize` | Activar Stage 12 |
| `/commit "mensaje"` | Hacer commit manual |
| `/git status` | Ver cambios sin commitear |

**Pero en la mayoría de casos NO los necesitas — Claude Code los ejecuta automáticamente**

---

## Resumen

**Minuto 0-5:** Activar `/skill thyrox`
**Minuto 5-10:** Ver estado actual
**Minuto 10-20:** Crear primer Work Package (DISCOVER)
**Minuto 20-25:** Completar Stage 1, hacer commits
**Minuto 25-30:** Comenzar Stage 6 SCOPE

**Resultado:** Estructura de trabajo completamente organizada, persistente, y lista para continuar en próxima sesión.

**Lo mágico:** Claude Code automáticamente valida, actualiza, y persiste TODO basado en las convenciones del SKILL.md.
