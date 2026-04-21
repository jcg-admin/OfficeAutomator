```yml
type: Guía de Activación
version: 1.0
project: OfficeAutomator
updated_at: 2026-04-21
```

# CÓMO ACTIVAR CLAUDE CODE EN OFFICEAUTOMATOR

## Estructura de activación

OfficeAutomator usa la infraestructura de Claude Code copiada de THYROX:

```
.claude/
├── CLAUDE.md                    ← Contexto persistente (Level 2)
├── ARCHITECTURE.md              ← Arquitectura de Claude Code
├── settings.json                ← Configuración de plugins
├── agents/                      ← Agentes nativos (28 agentes especializados)
├── commands/                    ← Comandos /slash disponibles
├── references/                  ← Documentación global (on-demand)
├── rules/                       ← Reglas de negocio
├── scripts/                     ← Scripts de infraestructura
└── skills/                      ← 70+ skills
    └── thyrox/                  ← El motor THYROX (el más importante)
        ├── SKILL.md             ← Define 12 stages (DISCOVER → STANDARDIZE)
        ├── references/
        ├── scripts/
        └── assets/

.thyrox/
├── context/                     ← Estado persistente
│   ├── work/                    ← Work Packages (YYYY-MM-DD-HH-MM-SS-nombre/)
│   ├── decisions/               ← ADRs (Architecture Decision Records)
│   ├── errors/                  ← Error tracking
│   ├── now.md                   ← Estado actual
│   └── focus.md                 ← Dirección de trabajo
├── registry/                    ← Generadores
└── guidelines/                  ← Directivas tech-stack
```

---

## Paso 1: Activación en Claude Code

Cuando entres a Claude Code:

1. Ejecuta:
   ```
   /skill thyrox
   ```

2. O simplemente di:
   ```
   Activar skill thyrox
   ```

**Esto carga:**
- `.claude/CLAUDE.md` como contexto persistente
- `.claude/skills/thyrox/SKILL.md` como el motor de trabajo
- `.thyrox/context/now.md` como estado actual
- Todos los agentes y comandos disponibles

---

## Paso 2: Entender qué carga automáticamente

Cuando Claude Code carga OfficeAutomator, obtiene:

**Automático (siempre cargado):**
- `.claude/CLAUDE.md` - Contexto y convenciones
- `.thyrox/context/now.md` - Estado persistente
- Todos los `.claude/agents/*.md` - 28 agentes especializados
- Todos los `.claude/commands/*.md` - Comandos slash

**Bajo demanda (cuando lo necesites):**
- `.claude/references/*` - Documentación de plataforma
- `.claude/skills/*/SKILL.md` - Skills específicas
- `.thyrox/context/decisions/*` - ADRs del proyecto

---

## Paso 3: Activar el flujo de trabajo

Una vez en Claude Code:

### Opción A: Automatizado (recomendado)

```
claude > Necesito documentar los Use Cases de OfficeAutomator

Claude Code:
[Automáticamente detecta proyecto OfficeAutomator]
[Carga CLAUDE.md]
[Carga SKILL.md de thyrox]
[Ejecuta Stage 1: DISCOVER]
```

### Opción B: Manual

```
claude > /thyrox:discover

[Abre la sección DISCOVER del skill thyrox]
[Sigue los pasos definidos en SKILL.md]
[Crea Work Package en .thyrox/context/work/]
```

---

## Paso 4: Flujo de trabajo típico con OfficeAutomator

### Sesión 1: Documentación de Requirements

```
1. Activar Claude Code:
   claude

2. Decir:
   "Voy a documentar los Use Cases de OfficeAutomator"

3. Claude Code:
   - Lee CLAUDE.md
   - Lee SKILL.md de thyrox
   - Revisa .thyrox/context/now.md
   - Inicia Stage 1: DISCOVER
   - Crea WP: .thyrox/context/work/2026-04-21-XX-XX-XX-uc-documentation/

4. Sigue los pasos del SKILL.md
   - Stage 1: DISCOVER - Entender qué necesitamos
   - Stage 2: (opcional) - Baseline actual
   - Stage 6: SCOPE - Definir alcance
   - Stage 7: DESIGN/SPECIFY - Especificar cada UC
   - Stage 10: IMPLEMENT - Crear archivos .md
   - Stage 11: TRACK/EVALUATE - Revisar y cerrar

5. Commits convencionales:
   feat(requirements): documentar UC-001
   feat(requirements): documentar UC-002
   docs(requirements): crear use-cases-matrix.md
```

### Sesión 2: Implementar módulo PowerShell

```
1. Activar Claude Code:
   claude

2. Claude Code carga state anterior (de .thyrox/context/now.md):
   "Última sesión: UC documentation terminada"

3. Decir:
   "Quiero empezar a implementar el módulo PowerShell"

4. Claude Code:
   - Inicia Stage 1: DISCOVER (nueva épica)
   - Crea WP: .thyrox/context/work/2026-04-21-XX-XX-XX-powershell-module/
   - Sigue stages hasta implementación

5. En Stage 10 (IMPLEMENT):
   - Crea Functions/Public/*.ps1
   - Crea Functions/Private/*.ps1
   - Commits convencionales
```

---

## Paso 5: Comandos importantes

Una vez que Claude Code está activo, puedes usar estos comandos:

### Información y estado

```
/status                    ← Estado actual del proyecto
/focus                     ← Dirección de trabajo (de .thyrox/context/focus.md)
/now                       ← Estado persistente (de .thyrox/context/now.md)
```

### Workflow THYROX

```
/thyrox:discover          ← Explorar y analizar
/thyrox:strategy          ← Definir estrategia
/thyrox:design            ← Especificar requirements
/thyrox:decompose         ← Crear tareas atómicas
/thyrox:execute           ← Ejecutar implementación
/thyrox:track             ← Evaluar y cerrar
/thyrox:standardize       ← Documentar aprendizajes
/thyrox:audit             ← Verificar calidad antes de cerrar
```

### Gestión de archivos

```
/git status               ← Ver cambios
/git diff file.md         ← Ver diferencias
/commit "mensaje"         ← Hacer commit (usa convencionales)
```

---

## Paso 6: Actualizar estado persistente

Después de cada sesión importante, Claude Code automáticamente actualiza:

- `.thyrox/context/now.md` - Estado actual
- `.thyrox/context/focus.md` - Dirección siguiente
- `.thyrox/context/work/{WP}/` - Artefactos del Work Package
- `ROADMAP.md` - Progreso del proyecto

**No necesitas hacer nada — lo hace automáticamente basado en convenciones.**

---

## Paso 7: Workflow diario recomendado

### Mañana (Inicio de sesión)

```
claude > Estado general, ¿en qué estamos?

[Claude Code carga estado y muestra:]
- Última épica completada: UC-001 documentation
- Actual work package: .thyrox/context/work/.../
- Stage actual: Stage 6 SCOPE
- Próxima tarea: Especificar UC-002
```

### Durante el día

```
- Seguir el skill thyrox
- Hacer commits convencionales
- Actualizar .thyrox/context/now.md cuando cambies de etapa
```

### Final del día (Cierre de sesión)

```
claude > Reporte de lo que hicimos hoy

[Claude Code genera:]
- Commits hechos
- Archivos modificados
- Progreso en ROADMAP.md
- Estado para próxima sesión
```

---

## Archivos CRÍTICOS a entender

### 1. `.claude/CLAUDE.md`
- **Qué es:** Contexto persistente entre sesiones
- **Cuándo lo lee Claude Code:** Siempre, en cada sesión
- **Qué contiene:** Convenciones, estructura, decisiones framework

### 2. `.claude/skills/thyrox/SKILL.md`
- **Qué es:** El motor de trabajo (12 stages)
- **Cuándo activarlo:** `/skill thyrox` o cuando necesites estructura
- **Qué hace:** Guía el flujo de trabajo de cualquier tarea

### 3. `.thyrox/context/now.md`
- **Qué es:** Estado actual del proyecto
- **Cuándo se actualiza:** Automáticamente al cambiar de stage
- **Qué contiene:** Work package activo, stage actual, focus

### 4. `.thyrox/context/work/{WP}/`
- **Qué es:** Directorio de una épica/tarea de trabajo
- **Nomenclatura:** `YYYY-MM-DD-HH-MM-SS-nombre`
- **Contenido:** Todos los artefactos de esa tarea

### 5. `.thyrox/context/decisions/`
- **Qué es:** Architecture Decision Records (ADRs)
- **Cuándo crear:** Cuando tomas una decisión importante
- **Formato:** `adr-{tema}.md` (sin números)

---

## Checklist para primera activación

- [ ] Clonar/descargar OfficeAutomator
- [ ] Abrir en Claude Code
- [ ] Ejecutar `/skill thyrox` o mencionar "Activar thyrox"
- [ ] Revisar `.thyrox/context/now.md`
- [ ] Revisar `.thyrox/context/focus.md`
- [ ] Revisar `ROADMAP.md`
- [ ] Empezar con Stage 1 (DISCOVER) o continuar desde último stage activo

---

## Troubleshooting

### "No carga el skill thyrox"

Solución:
```
1. Verifica que .claude/skills/thyrox/SKILL.md existe
2. Ejecuta: /skill thyrox
3. Si sigue sin cargar, menciona: "Cargar contexto de .claude/"
```

### "¿Dónde está el estado de mi trabajo anterior?"

Solución:
```
Está en:
.thyrox/context/now.md        ← Estado actual
.thyrox/context/focus.md       ← Lo que estabas haciendo
.thyrox/context/work/          ← Todos tus Work Packages
ROADMAP.md                     ← Progreso general
```

### "No puedo hacer commit"

Solución:
```
1. Verifica: git status
2. Usa formato convencional: feat(requirements): descripción
3. Claude Code automáticamente valida con .github/workflows/validate.yml
```

---

## Resumen: Cómo activar Claude Code en OfficeAutomator

```
PASO 1: Abre Claude Code
PASO 2: Ejecuta /skill thyrox (o di "Activar thyrox")
PASO 3: Lee .claude/CLAUDE.md (se carga automáticamente)
PASO 4: Sigue .claude/skills/thyrox/SKILL.md (12 stages)
PASO 5: Usa /thyrox:discover, /thyrox:design, etc.
PASO 6: Crea Work Packages en .thyrox/context/work/
PASO 7: Commits convencionales (feat/fix/docs/test)
PASO 8: Claude Code actualiza .thyrox/context/ automáticamente

Resultado: Workflow completamente estructurado y persistente
```

---

**Para más detalles:** Ver `.claude/CLAUDE.md` línea 108+
**Para workflow stages:** Ver `.claude/skills/thyrox/SKILL.md`
**Para estado actual:** Ver `.thyrox/context/now.md`
