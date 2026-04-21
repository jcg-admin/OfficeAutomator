```yml
created_at: 2026-04-21 06:35:00
project: THYROX
work_package: 2026-04-21-06-15-00-design-specification-correct
type: BA Analysis - Requirements Coverage
methodology: BABOK v3
knowledge_area: ba:requirements-analysis
author: Claude (BA Coordinator perspective)
status: Borrador
```

# PERSPECTIVA BA: Requirements Analysis View — Stage 7

## Pregunta Fundamental

**¿El PLAN v3 responde a TODOS los requisitos identificados en Stage 1 (DISCOVER) y Stage 6 (SCOPE)?**

---

## MATRIZ DE TRAZABILIDAD: Requisitos → Diseño

### 5 UCs Identificados (Stage 1)

| UC ID | Nombre | Stage 1 Requisitos | ¿Cubierto en PLAN v3? | Gap/Nota |
|-------|--------|-------------------|----------------------|----------|
| UC-001 | Select Version | Usuario selecciona 1 de 3 versiones | ✓ design/uc-001 | CUBIERTO |
| UC-002 | Select Language | Usuario selecciona 1+ idioma | ✓ design/uc-002 | CUBIERTO |
| UC-003 | Exclude Apps | Usuario excluye 0+ apps | ✓ design/uc-003 | CUBIERTO |
| UC-004 | Validate Config | Sistema valida integridad | ✓ design/uc-004 | CUBIERTO (crítico) |
| UC-005 | Install Office | Sistema ejecuta setup.exe | ✓ design/uc-005 | CUBIERTO (idempotente) |

✓ **5/5 UCs cubiertos**

---

## REQUISITOS DETALLADOS: Cobertura en PLAN v3

### UC-001: Select Version

**Requisitos de Stage 1:**
```
- Usuario selecciona entre 3 versiones (2024, 2021, 2019)
- Selección es OBLIGATORIA
- Sistema valida que versión es válida
- La versión seleccionada PERSISTE para siguientes UCs
```

**¿Cubierto en PLAN v3?**
- ✓ TIER 0: UC-Matrix-Analysis define $Config.Version
- ✓ design/uc-001: Especifica flujo completo
- ✓ design/overall-architecture: Layer 1 (UI) + Layer 3 (Config)
- ✓ overall-architecture: $Config object carries state across UCs

**Gaps identificados:**
- ¿Validación de versión está en UC-001 o UC-004? (necesita claridad)
- ¿Interfaz UI es parte del scope Stage 7? (probable: NO, es Stage 10)

**Recomendación BA:**
```
ESPECIFICAR: ¿Qué pasa si usuario intenta seleccionar versión inválida?
ESPECIFICAR: ¿Dónde vive la lista de versiones válidas? (hardcoded? externa?)
```

---

### UC-002: Select Language

**Requisitos de Stage 1:**
```
- Usuario selecciona 1 o más idiomas
- Sistema muestra solo idiomas soportados por la versión
- Selección es OBLIGATORIA (mínimo 1 idioma)
- Sistema valida idioma contra lista permitida
- Selecciones PERSISTEN
```

**¿Cubierto en PLAN v3?**
- ✓ design/uc-002: Especifica multi-language support
- ✓ TIER 0: $Config.Languages define estructura
- ✓ Stage 6 SCOPE: language-compatibility-matrix.md disponible
- ✓ Roadmap v1.1 mencionado para idiomas adicionales

**Gaps identificados:**
- ¿En v1.0.0 máximo 2 idiomas SIMULTÁNEAMENTE? (SÍ, en Stage 6 SCOPE)
- ¿Qué pasa si usuario selecciona idioma no válido para versión? (falta especificar)
- ¿Auto-detección de idioma OS dónde va? (mencionado pero no especificado dónde)

**Recomendación BA:**
```
ESPECIFICAR: Error handling si selecciona idioma incompatible
ESPECIFICAR: Cómo interactúa auto-detección con selección manual
ESPECIFICAR: Qué sucede si intenta 3+ idiomas (falla? trunca? warning?)
```

---

### UC-003: Exclude Applications

**Requisitos de Stage 1:**
```
- Usuario selecciona 0 o más aplicaciones para excluir
- Sistema muestra solo apps permitidas para excluir
- Sistema valida que cada exclusión es válida
- Exclusiones pueden ser: Teams, OneDrive, Groove, Lync, Bing
- Por defecto se excluyen: Teams, OneDrive (usuario puede cambiar)
- Exclusiones PERSISTEN
```

**¿Cubierto en PLAN v3?**
- ✓ design/uc-003: Especifica exclusiones
- ✓ TIER 0: $Config.ExcludedApps define estructura
- ✓ Stage 6 SCOPE: Exclusiones permitidas definidas
- ✓ Tabla de defaults por versión requerida

**Gaps identificados:**
- ¿Qué apps se pueden excluir por versión? (Stage 6 define parcialmente)
- ¿Qué sucede si usuario intenta excluir app no excluible? (falta especificar)
- ¿XML generado correctamente refleja exclusiones? (no mencionado)
- ¿Validación de XML contra XSD? (Stage 1 menciona, PLAN v3 asume UC-004)

**Recomendación BA:**
```
ESPECIFICAR: Matriz apps×versiones (qué se puede excluir en cada versión)
ESPECIFICAR: Validación XML syntax (¿quién lo hace? ¿UC-003 o UC-004?)
ESPECIFICAR: Comportamiento si apps excluidas no existen en versión
```

---

### UC-004: Validate Configuration ⭐ CRÍTICO

**Requisitos de Stage 1:**
```
- Sistema calcula SHA256 del archivo descargado
- Sistema compara contra hash oficial de Microsoft
- Si coincide: devuelve PASS
- Si no coincide: devuelve FAIL con código de error
- Si descarga corrupta: reintenta (máximo 3 veces)
- Logs incluyen hash calculado y hash esperado
```

**Requisitos adicionales de análisis (Stage 1 notes):**
```
- Microsoft OCT bug: cross-validation idioma-aplicación (paso 6)
- Fail-Fast: Si falla, NO proceder a UC-005
- Bloqueador: UC-005 depende de validación exitosa
```

**¿Cubierto en PLAN v3?**
- ✓ design/uc-004: 8-step validation detallado
- ✓ TIER 0: UC-Matrix-Analysis identifica UC-004 como bloqueador crítico
- ✓ adr-uc-004-8-step-validation.md: Justifica 8 pasos
- ✓ adr-fail-fast-principle.md: Documenta no-installation-on-failure
- ✓ Step 6: Anti-Microsoft-OCT-bug validación
- ✓ Step 7-8: Retry logic (3x) con backoff exponencial

**Gaps identificados:**
- ¿Paso 1: Versión válida en whitelist? (¿quién mantiene whitelist?)
- ¿Paso 2: XML schema válido? (¿contra qué XSD?)
- ¿Paso 3-4: Language-version validado? (dónde está matriz?)
- ¿Paso 5: App-version compatibility? (dónde está tabla?)
- ¿Paso 6: Microsoft OCT bug mitigation? (¿cómo especificar?)
- ¿Paso 7: Download + SHA256? (¿dónde se descarga? Microsoft oficial?)
- ¿Paso 8: Generar configuration.xml? (¿quién valida syntax?)

**Recomendación BA:**
```
CRÍTICO: UC-004 DEPENDE de DATA STRUCTURES externos:
  - Whitelist de versiones (¿dónde? en código? JSON? config?)
  - XSD para validación XML (¿Microsoft proporciona? ¿creamos?)
  - Matriz language-version (Stage 6 SCOPE tiene, referenciar explícitamente)
  - Matriz app-version (¿existe? ¿crear?)
  - Microsoft hash official (¿API? ¿file? ¿endpoint?)

ESTE ES UN GAP IMPORTANTE: ¿De dónde saca UC-004 las "verdades de negocio"?
```

---

### UC-005: Install Office

**Requisitos de Stage 1:**
```
- Sistema ejecuta setup.exe con configuration.xml
- Sistema monitorea progreso de instalación
- Sistema captura stdout/stderr
- Si éxito: retorna código 0
- Si error: retorna código de error y logs detallados
- Si ejecuta 2x: la segunda ejecución es idempotente (no reinstala)
```

**¿Cubierto en PLAN v3?**
- ✓ design/uc-005: 3 modos (Fresh, Verify, Repair)
- ✓ adr-idempotence-approach-uc-005.md: Documenta idempotencia
- ✓ TIER 0: UC-Matrix-Analysis identifica UC-005 espera UC-004
- ✓ Monitoring cada 5 segundos
- ✓ Timeout 30 minutos

**Gaps identificados:**
- ¿Verificación de instalación existente? (¿cómo detectar Office ya instalado?)
- ¿Registros donde escribir logs? (¿HKLM\Software\...? ¿archivo?)
- ¿Pre-checks: admin, disk space, network? (mencionados pero no especificados)
- ¿Post-install verification? (¿Word COM test? ¿qué exactamente?)
- ¿Cleanup: qué se borra? (config.xml sí, pero ¿qué más?)

**Recomendación BA:**
```
ESPECIFICAR: Registry keys para detectar Office existente
ESPECIFICAR: Cómo determinar si "Office ya está instalado"
ESPECIFICAR: Criterios de "Fresh" vs "Verify" vs "Repair"
```

---

## REQUISITOS TRANSVERSALES: Cobertura en PLAN v3

### Requisito: "Logging a todos los niveles"

**Stage 1 requiere:**
```
Logs incluyen información de decisiones
```

**REGLAS_DESARROLLO requiere:**
```
[INFO], [WARN], [ERROR], [BLOQUEADOR] format
Timestamps a milisegundo
```

**¿Cubierto en PLAN v3?**
- ✓ overall-architecture.md: Layer 6 (Utility/Logging)
- ✓ REGLAS_DESARROLLO_OFFICEAUTOMATOR.md: Output format defined
- ✗ **GAP:** Especificación detallada de QUÉS LOGUEAR no está

**Recomendación BA:**
```
CREAR: Document "Logging Strategy" que especifique:
  - Qué eventos loguear en cada UC
  - Qué información sensitiva NO loguear
  - Ubicación de archivos log
  - Rotación/limpieza de logs
```

---

### Requisito: "Error Handling y Retry"

**Stage 1 requiere:**
```
Si descarga corrupta: reintenta (máximo 3 veces)
```

**REGLAS_DESARROLLO requiere:**
```
Error categories: [BLOQUEADOR], [CRITICO], [RECUPERABLE]
Fail-Fast: Terminar temprano con errores específicos
```

**¿Cubierto en PLAN v3?**
- ✓ UC-004: 3x retry con exponential backoff (2s, 4s, 6s)
- ✓ adr-fail-fast-principle.md: Documenta no-install-on-failure
- ✓ adr-retry-strategy.md: Retry logic especificado
- ✗ **GAP:** Qué hace cada UC cuando recibe error de UC anterior

**Recomendación BA:**
```
ESPECIFICAR: Error propagation:
  - Si UC-001 falla → ¿qué hace UC-002?
  - Si UC-002 falla → ¿qué hace UC-003?
  - Si UC-003 falla → ¿saltar a UC-004 o abortar?
  - Si UC-004 falla → abortar (bloqueador)
```

---

### Requisito: "Idempotencia"

**Stage 1 requiere:**
```
Si ejecuta 2x: la segunda ejecución es idempotente (no reinstala)
```

**REGLAS_DESARROLLO requiere:**
```
Running 2x = Running 1x
Estado debe ser determinístico
```

**¿Cubierto en PLAN v3?**
- ✓ design/uc-005: 3 modos con "Verify" mode
- ✓ adr-idempotence-approach.md: Documenta lógica
- ✓ TIER 0: UC-Matrix-Analysis menciona state management
- ✗ **GAP:** ¿Qué pasa si usuario corre UC-001 dos veces? ¿Se reinicia todo?

**Recomendación BA:**
```
ESPECIFICAR: Scope de idempotencia:
  - ¿Toda la orchestration es idempotente?
  - ¿Solo UC-005 es idempotente?
  - ¿Qué hace UC-001→UC-002→UC-003 si se corren 2x?
```

---

## MATRIZ: Requirements Coverage Summary

| Categoría | Total | Cubiertos | % | Gaps | Prioridad |
|-----------|-------|-----------|---|------|-----------|
| 5 UCs Core | 5 | 5 | 100% | 0 | - |
| UC-specific Reqs | 25+ | ~20 | 80% | 5 | 🔴 ALTA |
| Transverse Reqs | 10 | 7 | 70% | 3 | 🔴 ALTA |
| Data Structure Reqs | 12 | 4 | 33% | 8 | 🔴 CRÍTICA |
| **TOTAL** | **52+** | **36** | **69%** | **16** | **🔴 CRÍTICA** |

---

## GAPS CRÍTICOS IDENTIFICADOS (BA Perspective)

### GAP-001: Data Structures Externos 🔴 CRÍTICO

**Problema:**
UC-004 necesita validar contra "verdades de negocio" que NO ESTÁN ESPECIFICADAS:
- Whitelist de versiones
- XSD para validación XML
- Matriz language-version compatibility
- Matriz app-version compatibility
- Microsoft official hash source

**Dónde deben especificarse:**
- Probablemente en nuevo documento: `design/data-structures-and-matrices.md`
- O en TIER 0: UC-Matrix-Analysis (expandir)

**Bloquea a:** UC-004 (no puede diseñarse sin saber dónde saca datos)

---

### GAP-002: Error Propagation Entre UCs 🔴 ALTA

**Problema:**
UC-001, 002, 003 pueden fallar, pero no está especificado qué hacer cuando fallan.

**Ejemplo:**
- Si UC-001 falla → ¿reintentar automáticamente?
- Si UC-002 falla → ¿volver a UC-001?
- Si UC-003 falla → ¿qué estado queda en $Config?

**Dónde especificar:**
- design/uc-001/002/003: Sección "Error Scenarios" (expandir)
- O nuevo documento: `design/error-propagation-strategy.md`

---

### GAP-003: Logging Strategy Detallada 🔴 ALTA

**Problema:**
REGLAS_DESARROLLO define FORMAT, pero no QUÉ loguear.

**Dónde especificar:**
- Nuevo documento: `design/logging-specification.md`
- O sección en overall-architecture.md (expandir Layer 6)

---

### GAP-004: Idempotence Scope 🔴 ALTA

**Problema:**
¿Toda la orquestación UC-001→005 es idempotente, o solo UC-005?

**Ejemplo:**
- Usuario corre: UC-001 (selecciona 2024) → UC-002 (selecciona es-ES) → aborta
- Usuario corre AGAIN: ¿comienza en UC-001 o continúa desde UC-003?

**Dónde especificar:**
- design/uc-001/002/003: Sección "Idempotence" (nuevo)
- adr-idempotence-scope.md (nuevo ADR)

---

### GAP-005: Microsoft OCT Bug Mitigation (UC-004 paso 6) 🟠 MEDIA

**Problema:**
Stage 1 identifica que "Microsoft OCT permite seleccionar combos incompatibles"
PLAN v3 menciona "Anti-Microsoft-OCT-bug: paso 6" pero NO especifica CÓMO.

**Dónde especificar:**
- design/uc-004: Step 6 (expandir mucho)
- O: design/microsoft-oct-bug-mitigation.md

---

## RECOMENDACIONES BA PRIORITIZADAS

### MUST DO (antes de Stage 10 IMPLEMENT):

1. **Crear `design/data-structures-and-matrices.md`** 🔴
   - Whitelist de versiones
   - XSD XML schema
   - Matrices de compatibility
   - Source de Microsoft hashes
   - Duración: ~2 horas

2. **Expandir UC-004 Paso 6** 🔴
   - Especificar Microsoft OCT bug mitigation
   - Duración: ~1 hora

3. **Crear `design/error-propagation-strategy.md`** 🔴
   - Qué hacer cuando UC-001, 002, 003 fallan
   - Duración: ~1.5 horas

### SHOULD DO (mejora calidad):

4. **Crear `design/logging-specification.md`** 🟠
   - Qué loguear en cada UC
   - Duración: ~1.5 horas

5. **Crear ADR: `adr-idempotence-scope.md`** 🟠
   - Definir alcance de idempotencia
   - Duración: ~1 hora

6. **Expandir UC-001, 002, 003: Error Scenarios** 🟠
   - Qué pasa cuando fallan
   - Duración: ~2 horas

---

## CONCLUSIÓN BA

**Coverage actual:** ~69% de requisitos cubiertos

**Gaps críticos:** 5 (principalmente data structures y error handling)

**Recomendación:** 
```
PLAN v3 es arquitectónicamente CORRECTO,
pero le faltan ESPECIFICACIONES DE NEGOCIO críticas:

- Dónde saca los datos para validar
- Qué hacer cuando UC-001,002,003 fallan
- Cómo se mitiga el bug de Microsoft OCT
- Scope de idempotencia
- Qué se loguea y dónde

ESTOS GAPS DEBEN CERRARSE EN STAGE 7
(si no, UC-004 no podrá diseñarse completamente)
```

---

**Análisis BA completado:** 2026-04-21 06:35:00
**Metodología:** BABOK v3 - ba:requirements-analysis
**Cobertura de requisitos:** 69% (36/52+ cubiertos)
**Gaps críticos:** 5 (deben cerrarse antes Stage 10)
**Acción recomendada:** Crear 3 nuevos documentos + expandir UCs

