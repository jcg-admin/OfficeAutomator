```yml
type: Convención de Diagramas
category: Visualización y Documentación
version: 1.0.0
purpose: Define estándares para Mermaid/UML en OfficeAutomator
applies_to: Todos los markdown con diagramas
updated_at: 2026-04-21 02:50:00
```

# STANDARD: MERMAID DIAGRAMS - Dark Theme, No Emojis

## Core Requirement

**MANDATORY:**
1. NO emojis (❌, ✓, 🔄, ⏳, 📊, etc)
2. NO decorative icons
3. ALWAYS use `%%{init: { 'theme': 'dark' } }%%`
4. Use `[LABEL]` for categorization
5. Colors must be readable on dark background

---

## Paleta de Colores para Tema Dark

### Colores Base

| Uso | Color | Hex | Stroke |
|-----|-------|-----|--------|
| Fondo principal | Gris oscuro | `#1e1e1e` | `#4da6ff` (azul) |
| Fase/Sección | Verde oscuro | `#2d5016` | `#90ee90` (verde claro) |
| Paso OK | Verde muy oscuro | `#1a3a1a` | `#66bb6a` (verde) |
| Paso especial | Verde marrón | `#2d4a2b` | `#ff9800` (naranja) |
| Error bloqueador | Rojo muy oscuro | `#3d1f1a` | `#ef5350` (rojo) |
| Error especial | Marrón naranja | `#5d2c1a` | `#ff6f00` (naranja oscuro) |
| Error recuperable | Marrón oscuro | `#3d2a1a` | `#ffa726` (naranja claro) |
| Retry/Loop | Verde gris | `#2d3a1a` | `#ffd54f` (amarillo) |
| Éxito | Verde muy oscuro | `#1a3a1a` | `#81c784` (verde claro) |
| Fin exitoso | Verde intenso | `#0d5d0d` | `#66bb6a` |
| Fin fallido | Rojo intenso | `#5d0d0d` | `#ef5350` |

---

## Estructura de Etiquetas (Sin Emojis)

### Categorías de Pasos

```
[PASO N]         - Paso secuencial normal
[VALIDAR]        - Validación específica
[CRITICO]        - Paso crítico (debe pasar)
[BLOQUEADOR]     - Error que detiene todo
[RECUPERABLE]    - Error que puede reintentar
[INFO]           - Información/estado
[SINCRONIZAR]    - Punto de sincronización
[RETRY]          - Reintento automático
[FASE 1]         - Fase/sección del flujo
```

### Ejemplos Correctos

```
[PASO 1] Validar versión
[VALIDAR] XML bien formado
[CRITICO] Antes de instalar
[BLOQUEADOR] Validación fallida
[RECUPERABLE] Descarga corrupta
[SINCRONIZAR] Fase 1 completada
[RETRY] Reintentar (máx 3)
```

### Ejemplos INCORRECTOS

```
❌ PASO 1 - No usar emoji
⚠ ADVERTENCIA - No usar emoji
✓ VALIDADO - No usar emoji
🔄 REINTENTANDO - No usar emoji
📁 ARCHIVO - No usar emoji
```

---

## Template Base para Diagramas Mermaid

```mermaid
%%{init: { 'theme': 'dark' } }%%
graph TD
    Start([INICIO]) --> Step1["[PASO 1] Primer paso"]
    Step1 -->|OK| Step2["[PASO 2] Segundo paso"]
    Step1 -->|ERROR| Error1["[BLOQUEADOR] Error temprano"]
    
    Step2 -->|OK| Step3["[PASO 3] Tercer paso"]
    Step2 -->|ERROR| Error2["[BLOQUEADOR] Fallo en paso 2"]
    
    Step3 -->|OK| Success["[EXITO] Completado"]
    Step3 -->|ERROR| Retry["[RETRY] Reintentar"]
    Retry -->|Intento 1,2| Step3
    Retry -->|Agotados| Error3["[RECUPERABLE] Agotado"]
    
    Success --> End["[FIN] Éxito"]
    Error1 --> EndFail["[FIN] Error"]
    Error2 --> EndFail
    Error3 --> EndFail
    
    style Start fill:#1e1e1e,stroke:#4da6ff,stroke-width:2px,color:#fff
    style Step1 fill:#1a3a1a,stroke:#66bb6a,color:#fff
    style Step2 fill:#1a3a1a,stroke:#66bb6a,color:#fff
    style Step3 fill:#1a3a1a,stroke:#66bb6a,color:#fff
    style Error1 fill:#3d1f1a,stroke:#ef5350,stroke-width:2px,color:#fff
    style Error2 fill:#3d1f1a,stroke:#ef5350,stroke-width:2px,color:#fff
    style Error3 fill:#3d2a1a,stroke:#ffa726,stroke-width:2px,color:#fff
    style Retry fill:#2d3a1a,stroke:#ffd54f,stroke-width:2px,color:#fff
    style Success fill:#1a3a1a,stroke:#81c784,stroke-width:3px,color:#fff
    style End fill:#0d5d0d,stroke:#66bb6a,stroke-width:3px,color:#fff
    style EndFail fill:#5d0d0d,stroke:#ef5350,stroke-width:3px,color:#fff
```

---

## Tipos de Diagramas Comunes

### 1. Flujo de Decisión (Decision Flow)

```mermaid
%%{init: { 'theme': 'dark' } }%%
graph TD
    Start([INICIO]) --> Validar{"[VALIDAR]<br/>Condición?"}
    Validar -->|OK| Procesar["[PASO] Procesar"]
    Validar -->|ERROR| Error["[BLOQUEADOR] Fallo validación"]
    
    Procesar -->|OK| End["[EXITO] Fin"]
    Procesar -->|ERROR| Error
    Error --> EndFail["[FIN] Error"]
    
    style Start fill:#1e1e1e,stroke:#4da6ff,stroke-width:2px,color:#fff
    style Validar fill:#2d5016,stroke:#90ee90,stroke-width:2px,color:#fff
    style Procesar fill:#1a3a1a,stroke:#66bb6a,color:#fff
    style Error fill:#3d1f1a,stroke:#ef5350,stroke-width:2px,color:#fff
    style End fill:#0d5d0d,stroke:#66bb6a,stroke-width:3px,color:#fff
    style EndFail fill:#5d0d0d,stroke:#ef5350,stroke-width:3px,color:#fff
```

### 2. Flujo Paralelo (Parallel Execution)

```mermaid
%%{init: { 'theme': 'dark' } }%%
graph TD
    Start([INICIO]) --> Fase["[FASE 1] Paralelo"]
    
    Fase --> Task1["[TAREA 1] Validar A"]
    Fase --> Task2["[TAREA 2] Validar B"]
    Fase --> Task3["[TAREA 3] Validar C"]
    
    Task1 -->|OK| Sync["[SINCRONIZAR]"]
    Task2 -->|OK| Sync
    Task3 -->|OK| Sync
    
    Task1 -->|ERROR| Error1["[BLOQUEADOR]"]
    Task2 -->|ERROR| Error2["[BLOQUEADOR]"]
    Task3 -->|ERROR| Error3["[BLOQUEADOR]"]
    
    Sync --> Next["[FASE 2] Continuar"]
    Error1 --> EndFail["[FIN] Error"]
    Error2 --> EndFail
    Error3 --> EndFail
    
    Next --> End["[EXITO] Fin"]
    
    style Start fill:#1e1e1e,stroke:#4da6ff,stroke-width:2px,color:#fff
    style Fase fill:#2d5016,stroke:#90ee90,stroke-width:2px,color:#fff
    style Task1 fill:#1a3a1a,stroke:#66bb6a,color:#fff
    style Task2 fill:#1a3a1a,stroke:#66bb6a,color:#fff
    style Task3 fill:#1a3a1a,stroke:#66bb6a,color:#fff
    style Sync fill:#1e1e1e,stroke:#4da6ff,stroke-width:2px,color:#fff
    style Error1 fill:#3d1f1a,stroke:#ef5350,stroke-width:2px,color:#fff
    style Error2 fill:#3d1f1a,stroke:#ef5350,stroke-width:2px,color:#fff
    style Error3 fill:#3d1f1a,stroke:#ef5350,stroke-width:2px,color:#fff
    style Next fill:#1a3a1a,stroke:#66bb6a,color:#fff
    style End fill:#0d5d0d,stroke:#66bb6a,stroke-width:3px,color:#fff
    style EndFail fill:#5d0d0d,stroke:#ef5350,stroke-width:3px,color:#fff
```

### 3. Retry Loop (Reintentos)

```mermaid
%%{init: { 'theme': 'dark' } }%%
graph TD
    Start([INICIO]) --> Step1["[PASO] Intentar descarga"]
    
    Step1 -->|Fallo| Retry{"[RETRY]<br/>Reintentos<br/>disponibles?"}
    Step1 -->|Exito| Success["[EXITO] Descarga OK"]
    
    Retry -->|Si: intento 1,2,3| Step1
    Retry -->|No: agotados| Error["[RECUPERABLE] Máx reintentos"]
    
    Success --> End["[FIN] Éxito"]
    Error --> EndFail["[FIN] Error recuperable"]
    
    style Start fill:#1e1e1e,stroke:#4da6ff,stroke-width:2px,color:#fff
    style Step1 fill:#1a3a1a,stroke:#66bb6a,color:#fff
    style Retry fill:#2d3a1a,stroke:#ffd54f,stroke-width:2px,color:#fff
    style Success fill:#1a3a1a,stroke:#81c784,color:#fff
    style Error fill:#3d2a1a,stroke:#ffa726,stroke-width:2px,color:#fff
    style End fill:#0d5d0d,stroke:#66bb6a,stroke-width:3px,color:#fff
    style EndFail fill:#5d0d0d,stroke:#ef5350,stroke-width:3px,color:#fff
```

---

## Errores Comunes y Correcciones

### Error 1: Usar Emojis

❌ **INCORRECTO:**
```mermaid
graph TD
    Start([INICIO]) --> OK["✓ Validado"]
    Start --> ERROR["❌ Error"]
    Start --> RUNNING["🔄 Procesando"]
```

✓ **CORRECTO:**
```mermaid
%%{init: { 'theme': 'dark' } }%%
graph TD
    Start([INICIO]) --> OK["[VALIDADO] OK"]
    Start --> ERROR["[BLOQUEADOR] Error"]
    Start --> RUNNING["[PROCESANDO] En progreso"]
```

---

### Error 2: Sin Tema Dark

❌ **INCORRECTO:**
```mermaid
graph TD
    Start([Inicio]) --> Step["Paso"]
    style Start fill:#ffffff,stroke:#000000
```

✓ **CORRECTO:**
```mermaid
%%{init: { 'theme': 'dark' } }%%
graph TD
    Start([INICIO]) --> Step["[PASO] Paso"]
    style Start fill:#1e1e1e,stroke:#4da6ff,stroke-width:2px,color:#fff
```

---

### Error 3: Colores Ilegibles

❌ **INCORRECTO:**
```mermaid
%%{init: { 'theme': 'dark' } }%%
graph TD
    Step["Paso"] 
    style Step fill:#ffffff,stroke:#000000,color:#000000
```

✓ **CORRECTO:**
```mermaid
%%{init: { 'theme': 'dark' } }%%
graph TD
    Step["[PASO] Paso"]
    style Step fill:#1a3a1a,stroke:#66bb6a,color:#fff
```

---

## Checklist para Diagramas

Antes de agregar cualquier diagrama Mermaid:

- [ ] `%%{init: { 'theme': 'dark' } }%%` presente
- [ ] NO hay emojis (❌, ✓, 🔄, etc)
- [ ] TODO usa `[ETIQUETA]` para categorizar
- [ ] Colores son legibles en fondo oscuro
- [ ] Texto en inglés MAYUSCULA O Title Case
- [ ] Nodos de error en rojo `#ef5350`
- [ ] Nodos de éxito en verde `#66bb6a`
- [ ] Nodos de inicio/fin en azul/magenta `#4da6ff`
- [ ] Bordes (stroke) tienen contraste suficiente
- [ ] Flujo es claro y fácil de seguir

---

## Referencia Rápida: Comandos Mermaid

### Sintaxis Básica

```
graph TD          # Top-Down (vertical)
graph LR          # Left-Right (horizontal)
graph TB          # Top-Bottom (vertical)
graph BT          # Bottom-Top
```

### Tipos de Nodos

```
A([CIRCLE])       # Círculo
A[RECTANGLE]      # Rectángulo
A{DIAMOND}        # Diamante (decisión)
A(ROUNDED)        # Redondeado
A[[SUBROUTINE]]   # Subrrutina
```

### Estilos

```
style A fill:#1a3a1a,stroke:#66bb6a,stroke-width:2px,color:#fff
```

Componentes:
- `fill` = color de fondo
- `stroke` = color del borde
- `stroke-width` = grosor del borde
- `color` = color del texto

---

## Ejemplos Completos Descargables

Ver documentos en proyecto:
- `uc-004-flow-corrected.md` - Flujo UC-004 completo (corregido)
- `REGLAS_DESARROLLO_OFFICEAUTOMATOR.md` - Incluye flujos de ejemplo

---

**Versión:** 1.0.0
**Última actualización:** 2026-04-21
**Aplicable a:** Todos los diagramas Mermaid en OfficeAutomator

