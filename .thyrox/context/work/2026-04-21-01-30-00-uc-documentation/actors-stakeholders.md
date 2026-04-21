```yml
type: Artefacto de Descubrimiento
stage: Stage 1 - DISCOVER
work_package: 2026-04-21-01-30-00-uc-documentation
created_at: 2026-04-21 01:30:00
updated_at: 2026-04-21 01:45:00
```

# ACTORS & STAKEHOLDERS ANALYSIS

## Actores Primarios

### 1. IT Administrator / Sistema Administrator

**Descripción:** Usuario que instala y configura Office en múltiples máquinas corporativas.

**Responsabilidades:**
- Descargar Office LTSC
- Seleccionar versión (2024, 2021, 2019)
- Elegir idiomas por región
- Excluir aplicaciones no deseadas (Teams, OneDrive)
- Validar integridad de instalación
- Ejecutar instalación en máquinas cliente

**Motivación:**
- Instalar rápido sin errores
- Control granular sobre configuración
- Validación de integridad
- Logging completo para auditoría

**Necesidades:**
- Interfaz clara (GUI o CLI)
- Ejecución confiable
- Documentación detallada de errores
- Capacidad de repetir proceso idénticamente (idempotencia)

**Frecuencia de uso:** Alta (cada vez que nuevo usuario/máquina)

---

### 2. System Administrator (Automatización)

**Descripción:** Usuario que automatiza instalaciones vía scripts/políticas de grupo.

**Responsabilidades:**
- Integrar OfficeAutomator en pipelines de deployment
- Validar comportamiento automático
- Monitorear instalaciones masivas
- Mantener configuraciones de referencia

**Motivación:**
- Automatización sin intervención manual
- Logs estructurados para parsing
- Salida predecible y máquinas idénticas

**Necesidades:**
- Salida estructurada (JSON/XML)
- Códigos de error claros
- Modo no-interactivo
- Timeouts configurables

**Frecuencia de uso:** Media (setup inicial, cambios de política)

---

## Actores Secundarios

### 3. QA / Tester

**Descripción:** Usuario que valida que OfficeAutomator instala correctamente.

**Responsabilidades:**
- Validar cada UC funciona como se especifica
- Verificar criterios de aceptación
- Registrar bugs
- Crear casos de prueba

**Necesidades:**
- Documentación clara de UCs
- Criterios de aceptación específicos
- Casos de error bien definidos
- Logs detallados para debugging

---

### 4. Developer

**Descripción:** Desarrollador que mantiene/extiende OfficeAutomator.

**Responsabilidades:**
- Implementar UCs
- Mantener código limpio
- Escribir tests
- Documentar decisiones técnicas

**Necesidades:**
- UCs sin ambigüedad
- Criterios de aceptación testeable
- Documentación arquitectónica
- ADRs de decisiones

---

### 5. Product Owner / Manager

**Descripción:** Responsable de features y prioridades.

**Responsabilidades:**
- Aprobar UCs
- Priorizar trabajo
- Validar que UCs resuelven problema original

**Necesidades:**
- UCs claros y alcanzables
- Definición de DONE objetiva
- Impacto comercial documentado

---

## Actores Externos / Sistemas

### 6. Microsoft Office Deployment Tool (ODT)

**Descripción:** Herramienta oficial de Microsoft que OfficeAutomator envuelve.

**Interfaz:**
- Descarga desde CDN de Microsoft
- Acepta configuration.xml
- Retorna códigos de error estándar

**Comportamiento:**
- Determinista
- Validación de input
- Logging a %temp%

---

### 7. GitHub Actions (Validación)

**Descripción:** Pipeline de CI que valida commits.

**Interfaz:**
- `.github/workflows/validate.yml`
- Valida conventional commits
- Verifica syntax PowerShell

---

## Matriz de Actores → UC

| Actor | UC-001 | UC-002 | UC-003 | UC-004 | UC-005 |
|-------|--------|--------|--------|--------|--------|
| IT Admin | Primario | Primario | Primario | Primario | Primario |
| Sys Admin (Automation) | Primario | Primario | Primario | Primario | Primario |
| QA | Secundario | Secundario | Secundario | Secundario | Secundario |
| Developer | N/A | N/A | N/A | N/A | N/A |
| ODT | Soporte | Soporte | Soporte | Soporte | Primario |

---

## Preguntas clave respondidas en DISCOVER

- [x] ¿Quién usa OfficeAutomator? IT Admins, Sys Admins
- [x] ¿Cuándo lo usan? Durante deployment de máquinas nuevas
- [x] ¿Por qué? Para instalar Office correctamente, rápido, validado
- [x] ¿Qué necesitan? Interfaz clara, control granular, logging
- [x] ¿Cómo lo miden (éxito)? Instalaciones sin errores, usuarios finales con Office funcional
- [x] ¿Qué puede fallar? Descarga corrupta, versión incompatible, validación fallida
- [x] ¿Quién valida que funciona? QA, IT Admin

---

## Restricciones identificadas

1. **ODT obligatorio:** OfficeAutomator depende de Microsoft ODT
2. **Conexión a internet:** Requiere acceso a CDN de Microsoft
3. **Permisos:** Requiere admin local en máquina cliente
4. **PowerShell:** Requiere PS 5.1+
5. **Windows:** Solo Windows 10/11, Server 2016+

---

## Risk Register Inicial

| Riesgo | Probabilidad | Impacto | Mitigación |
|--------|--------------|---------|-----------|
| Descarga ODT interrumpida | Media | Alto | Reintentos, caché local |
| Validación integridad falla | Baja | Alto | Verificación SHA256 |
| No idempotente (ejecutar 2x falla) | Media | Medio | Detectar estado previo, skip |
| Configuration.xml inválido | Baja | Alto | Validación XSD previa |
| Timeout insuficiente en red lenta | Media | Medio | Timeouts configurables |
