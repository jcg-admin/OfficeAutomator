# Documentación de OfficeAutomator

**Versión:** 1.0.0 (Planificación)  
**Estado:** En desarrollo  
**Última actualización:** 2026-04-21

---

## Navegación rápida por dominio

| Dominio | Sección | Propósito | Estado |
|---------|---------|-----------|--------|
| [introduction/](./introduction/) | 1 | Qué es el proyecto, visión, objetivos | ⚪ Pendiente |
| [requirements/](./requirements/) | 1.1 | Funcionalidades, casos de uso, especificaciones | ⚪ Pendiente |
| [quality-goals/](./quality-goals/) | 1.2 | Metas de calidad, atributos de calidad | ⚪ Pendiente |
| [stakeholders/](./stakeholders/) | 1.3 | Actores, roles, responsabilidades | ⚪ Pendiente |
| [constraints/](./constraints/) | 2 | Restricciones técnicas, organizacionales | ⚪ Pendiente |
| [context-scope/](./context-scope/) | 3 | Contexto del sistema, alcance | ⚪ Pendiente |
| [solution-strategy/](./solution-strategy/) | 4 | Estrategia de solución, decisiones clave | ⚪ Pendiente |
| [architecture/](./architecture/) | 5-7,9 | Diseño, componentes, flujos, despliegue | ⚪ Pendiente |
| [crosscutting-concepts/](./crosscutting-concepts/) | 8 | Patrones, principios, idiomas | ⚪ Pendiente |
| [quality-scenarios/](./quality-scenarios/) | 10 | Tests, escenarios, métricas de calidad | ⚪ Pendiente |
| [risks-technical-debt/](./risks-technical-debt/) | 11 | Riesgos, deuda técnica, mitigación | ⚪ Pendiente |
| [glossary/](./glossary/) | 12 | Vocabulario, términos del dominio | ⚪ Pendiente |

---

## Rutas de lectura por rol

### Para Product Manager / Responsable de Producto
1. [introduction/](./introduction/) - Entiende qué es y por qué existe
2. [requirements/](./requirements/) - Funcionalidades y alcance
3. [quality-goals/](./quality-goals/) - Metas de calidad esperadas
4. [risks-technical-debt/](./risks-technical-debt/) - Riesgos y deuda técnica

### Para Arquitecto / Technical Lead
1. [introduction/](./introduction/) - Contexto general
2. [solution-strategy/](./solution-strategy/) - Decisiones estratégicas
3. [architecture/](./architecture/) - Diseño y componentes
4. [constraints/](./constraints/) - Limitaciones técnicas
5. [quality-scenarios/](./quality-scenarios/) - Escenarios de calidad

### Para Desarrollador
1. [requirements/](./requirements/) - Qué hay que implementar
2. [architecture/](./architecture/) - Cómo está construido
3. [crosscutting-concepts/](./crosscutting-concepts/) - Patrones y principios
4. [quality-scenarios/](./quality-scenarios/) - Tests y validaciones
5. [glossary/](./glossary/) - Vocabulario técnico

### Para QA / Tester
1. [requirements/](./requirements/) - Requisitos a validar
2. [quality-scenarios/](./quality-scenarios/) - Casos de prueba
3. [glossary/](./glossary/) - Terminología
4. [constraints/](./constraints/) - Limitaciones conocidas

---

## Estructura de directorios

```
docs/
├── INDEX.md                              ← Este archivo (mapa maestro)
│
├── introduction/                         ← Section 1: Qué es el proyecto
├── requirements/                         ← Section 1.1: Qué hace
├── quality-goals/                        ← Section 1.2: Qué tan bien
├── stakeholders/                         ← Section 1.3: Para quién
├── constraints/                          ← Section 2: Qué nos limita
├── context-scope/                        ← Section 3: Dónde se sitúa
├── solution-strategy/                    ← Section 4: Cómo lo resolvemos
├── architecture/                         ← Sections 5-7,9: Diseño
├── crosscutting-concepts/                ← Section 8: Conceptos transversales
├── quality-scenarios/                    ← Section 10: Cómo probamos calidad
├── risks-technical-debt/                 ← Section 11: Riesgos y deuda
├── glossary/                             ← Section 12: Vocabulario
│
├── _archive/                             ← Documentación antigua (v1, v2, etc)
├── _methodology/                         ← Procesos, frameworks, convenciones
└── _tools/                               ← Ejemplos, templates, guías
```

---

## Principios de esta documentación

### Estructura plana por dominio (OPCIÓN B)
- Todos los dominios están al mismo nivel (raíz)
- Cada carpeta representa UN dominio único
- Sin meta-contenedores
- Navegación clara y directa

### Autoexplicativa
- Los nombres son autoexplicativos
- Máximo 2 niveles de profundidad
- Links bidireccionales entre dominios

### Living Documentation
- Se actualiza junto con el código
- Refleja decisiones reales
- Incluye cambios y evolución

---

## Estados de completitud

| Símbolo | Significado |
|---------|-----------|
| ⚪ | No iniciado |
| 🟡 | En progreso |
| 🟢 | Completado |
| 🔴 | Requiere revisión |

---

## Cómo usar esta documentación

1. **Primera vez:** Lee [introduction/](./introduction/) para contexto
2. **Necesitas saber cómo hacer algo:** Busca en [architecture/](./architecture/)
3. **Necesitas entender requisitos:** Ve a [requirements/](./requirements/)
4. **Necesitas verificar calidad:** Consulta [quality-scenarios/](./quality-scenarios/)
5. **No entiende un término:** Busca en [glossary/](./glossary/)

---

## Contribución a documentación

- Las contribuciones son bienvenidas
- Ver [_methodology/](../_methodology/) para convenciones
- Mantener coherencia con estructura plana
- Actualizar INDEX.md al agregar contenido nuevo

---

## Historial de cambios

| Fecha | Cambio | Autor |
|-------|--------|-------|
| 2026-04-21 | Creación inicial de estructura | Sistema |
| - | - | - |

---

**Última actualización:** 21 de abril de 2026

Estructura basada en **OPCIÓN B: Plana por Dominio** según guía estándar de arquitectura.
