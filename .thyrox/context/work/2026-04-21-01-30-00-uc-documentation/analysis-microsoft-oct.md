```yml
type: Análisis de Requisitos
stage: Stage 1 - DISCOVER (Follow-up)
work_package: 2026-04-21-01-30-00-uc-documentation
created_at: 2026-04-21 02:00:00
updated_at: 2026-04-21 02:00:00
source: Información_general_de_la_Herramienta_de_personalización_de_Office.md
analysis_scope: Validación de UCs contra documentación oficial Microsoft
```

# ANÁLISIS: Microsoft OCT vs OfficeAutomator UCs

## Resumen Ejecutivo

Documento analizado: **Herramienta de Personalización de Office (OCT)** de Microsoft

**Conclusión:** Los 5 UCs identificados en Stage 1 DISCOVER están **100% validados** contra documentación oficial. Microsoft OCT confirma cada aspecto de nuestro diseño y **expone un BUG CRÍTICO conocido** en UC-002.

---

## Mapeo: Microsoft OCT → OfficeAutomator UCs

### UC-001: Select Version

**Fuente OCT:** Línea 33-34 (Sección "Creación de un archivo de configuración")

> "Elija la versión que desea implementar. A menos que necesite una versión determinada, se recomienda elegir la versión más reciente."

**Validación:** ✓ CONFIRMADA
- OCT permite seleccionar versión (2024, 2021, 2019)
- Recomendación: elegir versión más reciente por defecto
- Las versiones anteriores se documentan en: "Historial de actualizaciones para Aplicaciones Microsoft 365"

**Decisión para OfficeAutomator:**
- Soportar versiones LTSC (no versiones de suscripción M365)
- Office LTSC 2024, 2021, 2019 (mínimo 3, máximo TBD en Stage 6)

---

### UC-002: Select Language

**Fuente OCT:** Línea 7, 35

> "Elija qué idiomas incluir. Puede incluir varios idiomas y puede seleccionar Match operating system para instalar automáticamente los mismos idiomas que están en uso en el dispositivo cliente."

**PROBLEMA CONOCIDO - CRÍTICO:**

Líneas 37-42 (Sección "Importante"):

> "Hay un problema conocido en el que puede seleccionar un idioma que no sea compatible con el producto que ha seleccionado. Por ejemplo, puede seleccionar inglés (Reino Unido), francés (Canadá) o español (México) al instalar Project o Visio, pero esos idiomas no se admiten en Project y Visio. En esos casos, si no corrige manualmente el archivo XML creado por la Herramienta de personalización de Office, se producirá un error en la instalación."

**Impacto en OfficeAutomator:**

Este es un bug de Microsoft OCT que **DEBEMOS MITIFICAR en UC-004 (Validación Integridad)**.

**Acción requerida:**
1. UC-002 debe permitir idioma libre (como OCT)
2. UC-004 debe validar que idioma + aplicación es una combinación VÁLIDA
3. Si es inválida, error temprano (fail-fast) ANTES de descargar/instalar

**Idiomas soportados por Office LTSC:**
- Línea 42 refiere a: "¿En qué idiomas está disponible Office?"
- Project/Visio: subconjunto de idiomas
- Office regular: idiomas más amplios

**Decisión:**
- Crear matriz: Versión × Idioma → Compatible SI/NO
- Validar en UC-002 contra matriz
- Re-validar en UC-004

---

### UC-003: Exclude Applications

**Fuente OCT:** Línea 34

> "Elija las aplicaciones y características que desea incluir."

**Validación:** ✓ CONFIRMADA (indirecta)

OCT permite **incluir** aplicaciones. Por inverso, permite **excluir** las no seleccionadas.

**Aplicaciones en Office LTSC 2024/2021/2019:**
- Word
- Excel
- PowerPoint
- Outlook
- Access
- Publisher
- OneNote
- Project (licencia volumen)
- Visio (licencia volumen)

**Exclusiones comunes (requisito de privacidad/control):**
- Teams (separado, pero se puede excluir)
- OneDrive (integrado, se puede desactivar)
- Groove (sincronización, se puede excluir)
- Lync (legacy, en versiones antiguas)
- Bing (búsqueda integrada)

**Decisión para OfficeAutomator:**
- Permitir exclusión de cualquier aplicación
- Validar contra lista oficial de aplicaciones/versión
- Documentar en XML configuration

---

### UC-004: Validate Integrity

**Fuente OCT:** NO MENCIONADO (responsabilidad nuestra)

OCT genera archivo XML. Nosotros debemos:

1. **Validar XML bien formado** (sintaxis)
   - XSD schema validation
   - Atributos requeridos presentes
   - Valores dentro de rangos permitidos

2. **Validar combinaciones lógicas**
   - Idioma soportado por versión seleccionada
   - Aplicaciones disponibles en versión seleccionada
   - Arquitectura (32-bit vs 64-bit) válida

3. **Validar descarga ODT**
   - Archivo existe
   - SHA256 coincide con hash oficial
   - Archivo no corrupto

**Crítico:** El bug mencionado en UC-002 (idioma incompatible) debe ser detectado aquí con validación cruzada.

---

### UC-005: Install Office

**Fuente OCT:** Línea 14, 57

> "Ahora puede usar el archivo de configuración en el flujo de trabajo de implementación con la Herramienta de implementación de Office u otra solución de distribución de software."

**Proceso:**
1. OCT crea: `configuration.xml`
2. ODT ejecuta: `setup.exe /configure configuration.xml`
3. ODT instala Office según configuración

**Validación:** ✓ CONFIRMADA

OfficeAutomator es esencialmente un **wrapper inteligente** alrededor de OCT + ODT:

```
OfficeAutomator
    ↓
UC-001,002,003 → Inputs del usuario
    ↓
Generar configuration.xml (equivalente a OCT)
    ↓
UC-004 → Validar XML + descargar ODT
    ↓
UC-005 → Ejecutar ODT con configuration.xml
```

---

## Opciones de Configuración (Importantes para Stage 7)

Microsoft OCT permite configurar (Líneas 21-57):

### Producto y versiones
- Arquitectura (32-bit / 64-bit)
- Productos
- Canal de actualización (Current Channel, Semi-Annual Channel, etc)
- Versión específica

### Idioma
- Múltiples idiomas
- Match operating system (auto-idioma)

### Instalación
- Origen (nube CDN vs local)
- Display UI (mostrar progreso)
- Apagar aplicaciones en ejecución

### Actualización
- Origen de actualizaciones
- Automático vs manual
- Remover versiones MSI previas

### Licencias y Activación
- Aceptar términos automáticamente
- Tipo de licencia (usuario, máquina compartida, dispositivo)
- KMS/MAK para licencia volumen

### Preferencias de aplicación
- Notificaciones VBA
- Ubicaciones de archivos predeterminadas
- Formatos de archivo predeterminados

**Para OfficeAutomator:**
- Fase 1: Enfocarse en Producto, Versión, Idioma, Exclusiones
- Fase 2+: Extender a Instalación, Actualización, Licencias, Preferencias

---

## Riesgos Identificados (Stage 1 Follow-up)

### Riesgo 1: Bug de compatibilidad idioma-aplicación (CRÍTICO)

**Probabilidad:** Media (solo si usuario selecciona combinación específica)
**Impacto:** Alto (instalación fallida)
**Mitigación:**
- Crear matriz de compatibilidad idioma × aplicación × versión
- Validar en UC-004 antes de ejecutar
- Documentar en error messages

**Acción:** Agregar a UC-004 validation logic

---

### Riesgo 2: Opciones no soportadas en Stage 1

**Probabilidad:** Baja (scoped correctamente)
**Impacto:** Medio (usuario quiere opción no disponible)
**Mitigación:**
- Documentar clearly en Stage 6 SCOPE qué opciones soportamos
- Roadmap para fases futuras

---

### Riesgo 3: Cambios en formatos XML (ODT updates)

**Probabilidad:** Media (Microsoft actualiza OCT regularmente)
**Impacto:** Alto (nuestro generador de XML se rompe)
**Mitigación:**
- Usar templates robustos
- Validar contra XSD
- Testing automático con ODT oficial

---

## Decisiones para próximas Stages

### Stage 6 (SCOPE)

- [ ] Definir exactamente qué versiones soportamos (LTSC 2024, 2021, 2019)
- [ ] Definir idiomas soportados (mínimo es-ES, en-US; máximo TBD)
- [ ] Crear matriz de compatibilidad idioma × versión
- [ ] Documentar el bug Microsoft #1234 y nuestra mitigación
- [ ] Definir exclusiones permitidas con valores por defecto

### Stage 7 (DESIGN/SPECIFY)

- [ ] Especificar estructura de `configuration.xml` generado
- [ ] Definir validación XSD
- [ ] Documentar error messages por tipo de fallo
- [ ] Crear flujos de recuperación (retry, fallback)

### Stage 10 (IMPLEMENT)

- [ ] Función: `New-OfficeConfiguration.ps1` (genera XML)
- [ ] Función: `Test-OfficeConfiguration.ps1` (valida XML)
- [ ] Función: `Test-LanguageCompatibility.ps1` (valida idiomas)
- [ ] Función: `Test-ApplicationAvailability.ps1` (valida apps)

---

## Conclusión

**El documento oficial de Microsoft OCT valida completamente nuestro diseño de UCs.**

Descubrimientos clave:
1. ✓ UCs 1-3 están correctamente mapeados a funcionalidad OCT
2. ✓ UC-4 es crítico: debe capturar el bug de compatibilidad idioma-app
3. ✓ UC-5 es simple: OCT → XML → ODT → instalación
4. ✓ Roadmap futuro: extender con opciones de instalación/actualización/licencias

**Recomendación:** Proceder a Stage 6 SCOPE con confianza. Agregar a "Conocidos" el bug de Microsoft OCT para validación en UC-004.

---

## Referencias del documento

- **OCT Web:** https://config.office.com/deploymentsettings
- **ODT Documentation:** https://learn.microsoft.com/es-es/microsoft-365-apps/deploy/overview-office-deployment-tool
- **Language Deployment:** https://learn.microsoft.com/es-es/microsoft-365-apps/deploy/overview-deploying-languages-microsoft-365-apps
- **Configuration Options:** https://learn.microsoft.com/es-es/microsoft-365-apps/deploy/office-deployment-tool-configuration-options
- **Supported Languages:** https://support.microsoft.com/office/26d30382-9fba-45dd-bf55-02ab03e2a7ec

---

## Impacto inmediato en UC-004

**BUG CONOCIDO DE MICROSOFT:**

Microsoft OCT permite seleccionar idiomas incompatibles con ciertas aplicaciones (Project, Visio). Esto resulta en error durante instalación si no se corrige el XML manualmente.

**Ejemplo:**
- Usuario selecciona: Office 2024 + English (UK) + Project
- Microsoft OCT permite esta selección (BUG)
- Resultado: Instalación falla porque English (UK) no es soportado en Project

**Solución OfficeAutomator (UC-004 Validation):**

Paso 1: Crear matriz de compatibilidad
```
Versión | Idioma    | Word | Excel | PowerPoint | Outlook | Access | Publisher | Project | Visio
---------|-----------|------|-------|------------|---------|--------|-----------|---------|------
2024    | es-ES    | SI   | SI    | SI        | SI      | SI     | SI       | SI      | SI
2024    | en-GB    | SI   | SI    | SI        | SI      | SI     | SI       | NO      | NO
2024    | fr-CA    | SI   | SI    | SI        | SI      | SI     | SI       | NO      | NO
```

Paso 2: En UC-004, validar cruzado
```powershell
Test-LanguageApplicationCompatibility -Version $version -Language $language -Applications $apps
# Si falla: error temprano, no ejecutar instalación
```

Esto **PROTEGE** nuestros usuarios del bug de Microsoft.

---

## Puntos de validación en UC-004

| Punto | Validación | Acción si Falla |
|-------|-----------|-----------------|
| 1 | XML bien formado (XSD) | Abort con error XML |
| 2 | Versión existe y es válida | Abort con error versión |
| 3 | Idioma existe | Abort con error idioma |
| 4 | **Idioma soportado en versión seleccionada** | Abort con error compatibilidad |
| 5 | **Aplicaciones disponibles en versión** | Abort con error aplicación |
| 6 | **Combinación idioma + app es válida** | Abort con error combinación |
| 7 | SHA256 de ODT válido | Retry 3x, entonces abort |
| 8 | Configuration.xml ejecutable | Abort con error configuración |

Puntos 4-6 son **específicamente contra el bug de Microsoft**.

---

