```yml
type: Artefacto de Descubrimiento
stage: Stage 1 - DISCOVER
work_package: 2026-04-21-01-30-00-uc-documentation
created_at: 2026-04-21 01:30:00
updated_at: 2026-04-21 01:45:00
analysis_depth: Initial
```

# DISCOVERY NOTES - OfficeAutomator UC Documentation

## Contexto del Proyecto

**OfficeAutomator** es un automatizador inteligente de instalación de Microsoft Office LTSC que:

1. Envuelve (wraps) la Office Deployment Tool (ODT) oficial de Microsoft
2. Simplifica la selección de versión, idioma, aplicaciones
3. Valida integridad de descargas
4. Proporciona interfaz GUI y CLI
5. Asegura idempotencia (ejecutar 2x = ejecutar 1x)

**Diferencia vs MSOI (anterior):**
- Validación robusta de input (MSOI falla)
- Verificación SHA256 (MSOI no valida)
- Idempotencia garantizada (MSOI no es idempotente)
- Logging completo (MSOI limitado)

---

## Los 5 Use Cases Identificados

### UC-001: Select Version
**Actor:** IT Administrator
**Descripción:** Usuario selecciona qué versión de Office LTSC instalar (2024, 2021, 2019)
**Importancia:** Crítica - define el producto base
**Complejidad:** Baja

### UC-002: Select Language
**Actor:** IT Administrator
**Descripción:** Usuario elige idioma(s) de instalación (es-ES, en-US, fr-FR, etc)
**Importancia:** Crítica - afecta a usuario final
**Complejidad:** Baja

### UC-003: Exclude Applications
**Actor:** IT Administrator
**Descripción:** Usuario excluye aplicaciones específicas (Teams, OneDrive, Groove, Lync)
**Importancia:** Alta - requisito de privacidad/control
**Complejidad:** Media (validación de valores permitidos)

### UC-004: Validate Integrity
**Actor:** Sistema automático + IT Administrator
**Descripción:** Sistema valida integridad SHA256 de archivo descargado vs. hash oficial
**Importancia:** Crítica - seguridad
**Complejidad:** Media (cálculo hash, comparación)

### UC-005: Install Office
**Actor:** IT Administrator
**Descripción:** Sistema ejecuta instalación de Office con configuración seleccionada
**Importancia:** Crítica - el resultado final
**Complejidad:** Alta (manejo de errores, progress tracking)

---

## Flujo General (Happy Path)

```
Inicio
  |
  v
UC-001: Select Version (2024/2021/2019)
  |
  v
UC-002: Select Language(s) (es-ES, en-US, etc)
  |
  v
UC-003: Exclude Applications (Teams, OneDrive, etc)
  |
  v
[Sistema genera configuration.xml]
  |
  v
[Sistema descarga ODT]
  |
  v
UC-004: Validate Integrity (SHA256 check)
  |
  v (si valida) | (si falla)
  v             v
UC-005:       Error Handler
Install       Retry descarga
  |           o abort
  v
Fin (success/failure)
```

---

## Interacciones Clave

### Entre UCs

**UC-001 → UC-002:**
- La versión elegida en UC-001 determina qué idiomas están disponibles
- Ejemplo: Office 2024 tiene más idiomas que 2019

**UC-002 → UC-003:**
- El idioma puede afectar qué aplicaciones están disponibles para excluir
- Ejemplo: Algunos idiomas excluyen automáticamente Bing

**UC-003 → UC-004:**
- Las exclusiones se registran en configuration.xml
- La validación en UC-004 valida que configuration.xml es sintácticamente correcto

**UC-004 → UC-005:**
- Solo si UC-004 pasa, UC-005 comienza
- Si UC-004 falla, UC-005 no se ejecuta (error early)

---

## Patrones Observados

### Validación en cascada
Cada UC valida su input y solo procede si es válido.
Ejemplo: UC-002 solo acepta idiomas soportados por la versión seleccionada en UC-001.

### Dependencias de datos
- UC-001 → data: version (string: "2024"/"2021"/"2019")
- UC-002 → data: language (string: "es-ES", "en-US", etc)
- UC-003 → data: exclusions (array: ["Teams", "OneDrive", ...])
- UC-004 → data: hash validation (boolean: pass/fail)
- UC-005 → data: success/failure code (int)

### Estados posibles
1. **Pending:** Esperando input del usuario
2. **Validating:** Validando input
3. **In-Progress:** Ejecutando (descarga/instalación)
4. **Success:** Completado sin errores
5. **Failed:** Error durante ejecución
6. **Retrying:** Reintentando tras error temporal

---

## Preguntas respondidas en DISCOVER

| Pregunta | Respuesta |
|----------|-----------|
| ¿Cuántos UCs hay? | 5 principales |
| ¿Quién los usa? | IT Admins, Sys Admins |
| ¿En qué orden? | UC-001 → UC-002 → UC-003 → UC-004 → UC-005 |
| ¿Cuál es crítica? | Todas son críticas |
| ¿Dónde está la complejidad? | UC-004 (validación) y UC-005 (instalación) |
| ¿Qué puede fallar? | Descarga corrupta, validación fallida, instalación interrumpida |
| ¿Cómo se valida éxito? | Office instalado, usuario puede usarlo, logs sin errores |

---

## Líneas de investigación para próximas sesiones

### Stage 2-3: Análisis
- Qué errores específicos retorna ODT
- Cómo recuperarse de errores
- Cuál es la tasa de fallos esperada

### Stage 6: Scope
- Qué versiones/idiomas soportamos exactamente
- Cuál es el set final de exclusiones permitidas
- Criterios de aceptación específicos

### Stage 7: Design/Specify
- Flujos de error detallados
- Criterios de aceptación por UC
- Casos edge (qué pasa si usuario selecciona combinación no válida)

### Stage 10: Implement
- PowerShell functions por UC
- Validación de input
- Manejo de errores
- Logging

---

## Notas finales

**Estado actual:** DISCOVERING - Sesión 1
**Claridad:** Alta (UCs son claros, actores identificados)
**Riesgos mayores:** Validación y manejo de errores (medios)
**Blockers:** Ninguno identificado aún
**Próximo paso:** Crear narrativas detalladas de cada UC (Stage 6: SCOPE)
