```yml
type: Definiciones de Scope
stage: Stage 6 - SCOPE
work_package: 2026-04-21-03-00-00-scope-definition
created_at: 2026-04-21 03:10:00
updated_at: 2026-04-21 03:10:00
scope: Valores exactos para v1.0.0
```

# DEFINICIONES PRECISAS - Versiones, Idiomas, Exclusiones

---

## VERSIONES SOPORTADAS (v1.0.0)

### Office 2024 LTSC

**Identificador:** `2024`

**Nombre Formal:** Microsoft Office LTSC 2024

**Soporte Oficial:** 2026-10-13 (5 años desde 2021-10-13)

**URL Descarga:** CDN de Microsoft (via ODT)

**Razón de inclusión:** 
- Versión más reciente
- Soporte vigente (5+ años)
- Arquitectura 64-bit recomendada
- Todas las aplicaciones incluidas

**Cambios vs 2021:**
- Teams integrado en suite (antes separado)
- OneNote mejorado (cloud-first)
- Optimizaciones de performance

---

### Office 2021 LTSC

**Identificador:** `2021`

**Nombre Formal:** Microsoft Office LTSC 2021

**Soporte Oficial:** 2026-10-13 (5 años desde 2021-10-13)

**URL Descarga:** CDN de Microsoft (via ODT)

**Razón de inclusión:**
- Soporte vigente (equilibrio estabilidad/innovación)
- Compatible con 2024 (migración posible)
- Groove aún incluido (deprecated en 2024)
- Lync no disponible (Teams alternativa)

**Cambios vs 2019:**
- Teams integrado en suite (antes separado)
- OneNote mejorado
- Mejor rendimiento de acceso remoto

---

### Office 2019 LTSC

**Identificador:** `2019`

**Nombre Formal:** Microsoft Office LTSC 2019

**Soporte Oficial:** 2025-10-13 (5 años desde 2020-10-13)

**URL Descarga:** CDN de Microsoft (via ODT)

**Razón de inclusión:**
- Soporte vigente hasta 2025 (retrocompatibilidad)
- Máquinas legacy requieren 2019
- Groove sincronización local (preferida en algunos contextos)
- Lync aún disponible (Teams reemplazo)

**Cambios vs 2016:**
- Arquitectura moderna (64-bit)
- Mejor performance
- Teams como aplicación separada
- Groove (OneDrive Sync) incluido

---

## IDIOMAS SOPORTADOS (v1.0.0)

### Base (Obligatorio)

#### es-ES - Español (España)

**Identificador:** `es-ES` (RFC 5646)

**Nombre Formal:** Español (España)

**Soporte en versiones:** 2024, 2021, 2019

**Incluye:**
- Corrector ortográfico español
- Diccionarios español
- Fuentes ibéricas
- Teclados españoles

**Razón de inclusión:**
- Mercado hispanohablante primario
- España - mercado europeo importante
- Latinoamérica (base para pt-BR en v1.1)

**Variantes NO soportadas en v1.0.0:**
- es-MX (Español México) - Roadmap v1.1
- es-AR (Español Argentina) - Roadmap v1.1
- es-CO (Español Colombia) - Roadmap v1.1

---

#### en-US - English (United States)

**Identificador:** `en-US` (RFC 5646)

**Nombre Formal:** English (United States)

**Soporte en versiones:** 2024, 2021, 2019

**Incluye:**
- Corrector ortográfico inglés US
- Diccionarios inglés US
- Teclados QWERTY
- Formatos US (dates, currency)

**Razón de inclusión:**
- Idioma técnico global
- Mercado norteamericano
- Compatibilidad con sistemas internacionales
- Base para en-GB en v1.1

**Variantes NO soportadas en v1.0.0:**
- en-GB (English UK) - Roadmap v1.1
- en-AU (English Australia) - Roadmap v1.1
- en-CA (English Canada) - Roadmap v1.1

---

### Extensiones (Roadmap v1.1+)

| Idioma | Código | Versiones | Prioridad |
|--------|--------|-----------|-----------|
| Français | fr-FR | 2024, 2021, 2019 | Alta (mercado EMEA) |
| Deutsch | de-DE | 2024, 2021, 2019 | Alta (mercado EMEA) |
| Italiano | it-IT | 2024, 2021, 2019 | Media |
| Portugués Brasil | pt-BR | 2024, 2021, 2019 | Alta (mercado LATAM) |
| 日本語 | ja-JP | 2024, 2021, 2019 | Media (mercado APAC) |
| 中文 (简体) | zh-CN | 2024, 2021, 2019 | Media (mercado APAC) |

---

## EXCLUSIONES PERMITIDAS (v1.0.0)

### Exclusiones Válidas (Se pueden quitar)

#### 1. Microsoft Teams

**Identificador:** `Teams`

**Versiones:** 2024, 2021, 2019

**Razón de exclusión:**
- Aplicación separada disponible
- Teams web como alternativa
- Instalación posterior posible
- Reduce footprint (instalación más rápida)

**Instalación separada:** https://www.microsoft.com/en-us/microsoft-teams/

**Estado en versiones:**
- 2024: Integrado (se puede excluir)
- 2021: Integrado (se puede excluir)
- 2019: NO incluido (aplicación separada)

---

#### 2. OneDrive for Business

**Identificador:** `OneDrive`

**Versiones:** 2024, 2021, 2019

**Razón de exclusión:**
- Sincronización en nube
- Requisito de privacidad corporativa (no usar OneDrive personal)
- Groove/Sync alternativa local
- Instalación posterior posible

**Alternativas:** 
- SharePoint Sync
- Groove (Office 2021/2019)
- Sincronización manual

**Nota:** Afecta colaboración en línea (Teams, SharePoint)

---

#### 3. Groove (OneDrive Sync)

**Identificador:** `Groove`

**Versiones:** 2021, 2019

**Estado:** Deprecated en 2024 (NO disponible para excluir)

**Razón de exclusión:**
- Sincronización OneDrive local (preferencia local)
- Puede consumir recursos
- OneDrive en nube es alternativa

**Nota:** En 2024 se reemplazó con OneDrive integrado

---

#### 4. Lync

**Identificador:** `Lync`

**Versiones:** 2019

**Estado:** NO en 2021, 2024 (reemplazado por Teams)

**Razón de exclusión:**
- Comunicación unificada legacy
- Teams es reemplazo moderno
- Mantenimiento descontinuado

**Alternativa:** Microsoft Teams

---

#### 5. Bing (Search Integration)

**Identificador:** `Bing`

**Versiones:** 2024, 2021

**Estado:** NO en 2019

**Razón de exclusión:**
- Búsqueda web integrada en Outlook
- Privacidad corporativa
- Búsqueda local alternativa

**Nota:** Afecta funcionalidad de búsqueda en Outlook

---

### Exclusiones NO Permitidas en v1.0.0

| Aplicación | Razón |
|------------|-------|
| Word | Aplicación principal (no excluible) |
| Excel | Aplicación principal (no excluible) |
| PowerPoint | Aplicación principal (no excluible) |
| Outlook | Aplicación principal (no excluible) |
| Access | Aplicación principal (no excluible) |
| Publisher | Aplicación principal (no excluible) |
| OneNote | Aplicación principal (no excluible) |
| Project | Requiere licencia volumen (v1.1+) |
| Visio | Requiere licencia volumen (v1.1+) |

---

## VALORES POR DEFECTO (UC-003)

Cuando usuario inicia UC-003 sin hacer selecciones, los defaults son:

```powershell
$DefaultExclusions = @(
    "Teams",        # Aplicación separada disponible
    "OneDrive"      # Requisito de privacidad corporativa
)
```

**Razón de defaults:**
1. Teams: v2019 no lo tiene; v2024/2021 uso moderno en nube
2. OneDrive: Típicamente requiere gestión de políticas corporativas

**Usuario puede cambiar:** Seleccionar diferentes exclusiones en UC-003

---

## RESTRICCIONES Y LIMITACIONES (v1.0.0)

### Múltiples Idiomas

**Máximo soportado:** 2 idiomas simultáneos

**Razón:** 
- Reduce complejidad de validación
- Casos de uso típicos: es-ES + en-US
- v1.1+ soportará N idiomas

**Ejemplo válido:**
```
Versión: 2024
Idiomas: es-ES, en-US
Exclusiones: Teams, OneDrive
```

**Ejemplo inválido:**
```
Versión: 2024
Idiomas: es-ES, en-US, fr-FR, de-DE  [ERROR: máximo 2]
```

---

### Match Operating System

**Disponible:** SI, en UC-002

**Funcionamiento:**
- Usuario elige "Match OS" en lugar de idioma específico
- OfficeAutomator detecta idioma del SO
- Instala ese idioma automáticamente

**Ejemplo:**
```
Usuario elige: "Match Operating System"
SO es: Windows 11 es-ES
Resultado: Office instala es-ES
```

---

## VALIDACIÓN DE SCOPE (Exit Criteria)

Confirmar que Stage 6 está completo:

- [x] 3 versiones LTSC definidas (2024, 2021, 2019)
- [x] 2 idiomas base definidos (es-ES, en-US)
- [x] 5 exclusiones permitidas definidas (Teams, OneDrive, Groove, Lync, Bing)
- [x] Matriz de compatibilidad completa
- [x] Valores por defecto definidos
- [x] Limitaciones documentadas (máx 2 idiomas)
- [x] Match Operating System explicado
- [x] Exclusiones NO permitidas claras
- [x] Roadmap v1.1 delineado

---

## Flujo de Validación (UC-004)

Con estas definiciones, UC-004 puede validar:

```
Paso 1: XML bien formado
Paso 2: Versión en [2024, 2021, 2019]?
Paso 3: Idioma en [es-ES, en-US]?
Paso 4: Idioma soportado en versión?
Paso 5: Apps en [Word, Excel, PowerPoint, Outlook, Access, Publisher, OneNote, Teams, OneDrive, Groove, Lync, Bing]?
Paso 6: Combinación [Version][Language][App] = true en matriz?
Paso 7: SHA256 integridad
Paso 8: XML ejecutable
```

---

**Versión:** 1.0.0
**Estado:** DEFINICIONES FINALES
**Aprobación:** Requiere stakeholder sign-off antes de Stage 7

