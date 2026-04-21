```yml
type: Artefacto de Descubrimiento
stage: Stage 1 - DISCOVER
work_package: 2026-04-21-01-30-00-uc-documentation
created_at: 2026-04-21 01:30:00
updated_at: 2026-04-21 01:45:00
```

# USE CASE MATRIX - OfficeAutomator

Mapeo de Use Cases → Responsable → Estado → Complejidad

---

## Matriz General

| UC ID | Nombre | Actor Principal | Complejidad | Importancia | Estado Actual | Responsable | Bloqueadores |
|-------|--------|-----------------|-------------|-------------|---------------|-------------|--------------| 
| UC-001 | Select Version | IT Admin | Baja | Crítica | Pending | Architecture | Ninguno |
| UC-002 | Select Language | IT Admin | Baja | Crítica | Pending | Architecture | UC-001 (debe definir versión primero) |
| UC-003 | Exclude Applications | IT Admin | Media | Alta | Pending | Validation | UC-002 (pueden haber dependencias lingüísticas) |
| UC-004 | Validate Integrity | Sistema | Media | Crítica | Pending | Security | Ninguno (pero debe ejecutarse antes de UC-005) |
| UC-005 | Install Office | IT Admin + Sistema | Alta | Crítica | Pending | Implementation | UC-001, UC-002, UC-003, UC-004 (todas son prerrequisitos) |

---

## Flujo de ejecución recomendado

```
Fase 1: Input Selection (UCs 001-003)
  UC-001 ← Primer input (versión)
  UC-002 ← Segundo input (idioma)
  UC-003 ← Tercio input (exclusiones)
  [Salidas: configuration.xml generado]

Fase 2: Validación (UC-004)
  UC-004 ← Valida integridad de descarga
  [Si falla: retry o abort]
  [Si pasa: proceder a UC-005]

Fase 3: Ejecución (UC-005)
  UC-005 ← Ejecuta instalación
  [Resultado: success/failure con logs]
```

---

## Dependencias Gráficas

```
                   ┌─────────────┐
                   │  UC-001     │
                   │Select Version│
                   └──────┬──────┘
                          │
                          v
                   ┌─────────────┐
                   │  UC-002     │
                   │Select Language
                   └──────┬──────┘
                          │
                          v
                   ┌─────────────┐
                   │  UC-003     │
                   │Exclude Apps │
                   └──────┬──────┘
                          │
                  [Gen config.xml]
                          │
                ┌─────────┴─────────┐
                │                   │
                v                   v
         ┌────────────┐      ┌────────────┐
         │ UC-004     │      │Descarga ODT│
         │Validate    │      │            │
         │Integrity   │      └────────────┘
         └──────┬─────┘
                │ (if valid)
                v
         ┌─────────────┐
         │  UC-005     │
         │Install      │
         └─────────────┘
```

---

## Criterios de aceptación iniciales

### UC-001: Select Version
- [x] Usuario puede seleccionar entre 3 versiones (2024, 2021, 2019)
- [x] Selección es obligatoria
- [x] Sistema valida que la versión es válida
- [x] La versión seleccionada persiste para los siguientes UCs

### UC-002: Select Language
- [x] Usuario puede seleccionar 1 o más idiomas
- [x] Sistema muestra solo idiomas soportados por la versión seleccionada
- [x] Selección es obligatoria (mínimo 1 idioma)
- [x] Sistema valida idioma contra lista permitida
- [x] Selecciones persisten para siguientes UCs

### UC-003: Exclude Applications
- [x] Usuario puede seleccionar 0 o más aplicaciones para excluir
- [x] Sistema muestra solo aplicaciones permitidas para excluir
- [x] Sistema valida que cada exclusión es válida
- [x] Exclusiones pueden ser: Teams, OneDrive, Groove, Lync, Bing
- [x] Por defecto se excluyen: Teams, OneDrive (usuario puede cambiar)
- [x] Exclusiones persisten para siguientes UCs

### UC-004: Validate Integrity
- [x] Sistema calcula SHA256 del archivo descargado
- [x] Sistema compara contra hash oficial de Microsoft
- [x] Si coincide: devuelve PASS
- [x] Si no coincide: devuelve FAIL con código de error
- [x] Si descarga corrupta: reintenta (máximo 3 veces)
- [x] Logs incluyen hash calculado y hash esperado

### UC-005: Install Office
- [x] Sistema ejecuta setup.exe con configuration.xml
- [x] Sistema monitorea progreso de instalación
- [x] Sistema captura stdout/stderr
- [x] Si éxito: retorna código 0
- [x] Si error: retorna código de error y logs detallados
- [x] Si ejecuta 2x: la segunda ejecución es idempotente (no reinstala si ya existe)

---

## Complejidad por área

### Baja complejidad
- UC-001: Solo selección de 3 opciones + validación simple
- UC-002: Selección múltiple + validación de lista

### Complejidad Media
- UC-003: Selección múltiple + generación de XML + validación sintáctica
- UC-004: Cálculo SHA256 + comparación + reintentos

### Complejidad Alta
- UC-005: Ejecución de proceso externo + manejo de errores + logging + idempotencia

---

## Riesgos por UC

| UC | Riesgo Principal | Mitigation |
|----|-----------------|-----------|
| UC-001 | Usuario selecciona versión no soportada | Validar contra lista permitida |
| UC-002 | Versión seleccionada no soporta idioma deseado | Filtrar idiomas por versión |
| UC-003 | XML mal formado por exclusiones inválidas | Validar XSD antes de ejecutar |
| UC-004 | Hash no coincide (descarga corrupta) | Reintento automático 3x |
| UC-005 | Instalación interrumpida / Office ya instalado | Detectar estado previo, idempotencia |

---

## Sesiones futuras esperadas

**Session 1 (Actual - Stage 1 DISCOVER):**
- Descubrimiento de UCs
- Identificación de actores
- Mapeo de dependencias
- Risk register inicial

**Session 2 (Stage 6 SCOPE):**
- Definir exactamente qué versiones/idiomas soportamos
- Definir exclusiones permitidas
- Crear scope statement

**Session 3 (Stage 7 DESIGN/SPECIFY):**
- Narrativas detalladas de cada UC
- Flujos de error documentados
- Criterios de aceptación refinados

**Session 4 (Stage 10 IMPLEMENT):**
- Crear Functions/Public/*
- Implementar validaciones
- Tests

**Session 5 (Stage 11 TRACK/EVALUATE):**
- Validar todos UCs funcionan
- Cierre de épica

---

## Estado de DISCOVER - COMPLETADO

Artefactos generados:
- Problem Statement
- Actors & Stakeholders
- Discovery Notes
- Use Case Matrix

Salidas esperadas:
- 5 UCs claramente identificados
- Actores principales definidos
- Flujos de dependencia documentados
- Risk Register inicial

Próximo Stage: Stage 6 SCOPE (definición detallada de alcance)
