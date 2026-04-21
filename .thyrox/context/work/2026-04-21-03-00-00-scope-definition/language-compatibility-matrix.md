```yml
type: Matriz de Compatibilidad
stage: Stage 6 - SCOPE
work_package: 2026-04-21-03-00-00-scope-definition
created_at: 2026-04-21 03:05:00
format: JSON + Markdown
purpose: Validación cruzada en UC-004 contra bug de Microsoft OCT
```

# LANGUAGE COMPATIBILITY MATRIX - OfficeAutomator

Matriz que valida: Versión × Idioma × Aplicación = Compatible SI/NO

**Crítica para UC-004:** Previene el bug de Microsoft OCT donde se permite seleccionar idiomas incompatibles con aplicaciones específicas.

---

## Resumen de Compatibilidad

### Office 2024 LTSC

#### es-ES (Español España)

| Aplicación | Compatible | Notas |
|------------|-----------|-------|
| Word | SI | Soporte completo |
| Excel | SI | Soporte completo |
| PowerPoint | SI | Soporte completo |
| Outlook | SI | Soporte completo |
| Access | SI | Soporte completo |
| Publisher | SI | Soporte completo |
| OneNote | SI | Soporte completo |
| Teams | SI | Aplicación separada |
| OneDrive | SI | Sincronización |
| Groove | N/A | No en 2024 |
| Lync | N/A | No en 2024 |
| Project | NO | Requiere licencia volumen |
| Visio | NO | Requiere licencia volumen |

#### en-US (English United States)

| Aplicación | Compatible | Notas |
|------------|-----------|-------|
| Word | SI | Soporte completo |
| Excel | SI | Soporte completo |
| PowerPoint | SI | Soporte completo |
| Outlook | SI | Soporte completo |
| Access | SI | Soporte completo |
| Publisher | SI | Soporte completo |
| OneNote | SI | Soporte completo |
| Teams | SI | Aplicación separada |
| OneDrive | SI | Sincronización |
| Groove | N/A | No en 2024 |
| Lync | N/A | No en 2024 |
| Project | NO | Requiere licencia volumen |
| Visio | NO | Requiere licencia volumen |

---

### Office 2021 LTSC

#### es-ES (Español España)

| Aplicación | Compatible | Notas |
|------------|-----------|-------|
| Word | SI | Soporte completo |
| Excel | SI | Soporte completo |
| PowerPoint | SI | Soporte completo |
| Outlook | SI | Soporte completo |
| Access | SI | Soporte completo |
| Publisher | SI | Soporte completo |
| OneNote | SI | Soporte completo |
| Teams | SI | Aplicación separada |
| OneDrive | SI | Sincronización |
| Groove | SI | Sincronización |
| Lync | NO | Reemplazado por Teams |
| Project | NO | Requiere licencia volumen |
| Visio | NO | Requiere licencia volumen |

#### en-US (English United States)

| Aplicación | Compatible | Notas |
|------------|-----------|-------|
| Word | SI | Soporte completo |
| Excel | SI | Soporte completo |
| PowerPoint | SI | Soporte completo |
| Outlook | SI | Soporte completo |
| Access | SI | Soporte completo |
| Publisher | SI | Soporte completo |
| OneNote | SI | Soporte completo |
| Teams | SI | Aplicación separada |
| OneDrive | SI | Sincronización |
| Groove | SI | Sincronización |
| Lync | NO | Reemplazado por Teams |
| Project | NO | Requiere licencia volumen |
| Visio | NO | Requiere licencia volumen |

---

### Office 2019 LTSC

#### es-ES (Español España)

| Aplicación | Compatible | Notas |
|------------|-----------|-------|
| Word | SI | Soporte completo |
| Excel | SI | Soporte completo |
| PowerPoint | SI | Soporte completo |
| Outlook | SI | Soporte completo |
| Access | SI | Soporte completo |
| Publisher | SI | Soporte completo |
| OneNote | SI | Soporte completo |
| Teams | NO | Versión separada requerida |
| OneDrive | SI | Sincronización |
| Groove | SI | Sincronización |
| Lync | SI | Comunicación unificada |
| Project | NO | Requiere licencia volumen |
| Visio | NO | Requiere licencia volumen |

#### en-US (English United States)

| Aplicación | Compatible | Notas |
|------------|-----------|-------|
| Word | SI | Soporte completo |
| Excel | SI | Soporte completo |
| PowerPoint | SI | Soporte completo |
| Outlook | SI | Soporte completo |
| Access | SI | Soporte completo |
| Publisher | SI | Soporte completo |
| OneNote | SI | Soporte completo |
| Teams | NO | Versión separada requerida |
| OneDrive | SI | Sincronización |
| Groove | SI | Sincronización |
| Lync | SI | Comunicación unificada |
| Project | NO | Requiere licencia volumen |
| Visio | NO | Requiere licencia volumen |

---

## Matriz Condensada (Formato JSON)

```json
{
  "version": "1.0.0",
  "compatibility": {
    "2024": {
      "es-ES": {
        "Word": true,
        "Excel": true,
        "PowerPoint": true,
        "Outlook": true,
        "Access": true,
        "Publisher": true,
        "OneNote": true,
        "Teams": true,
        "OneDrive": true,
        "Groove": false,
        "Lync": false,
        "Project": false,
        "Visio": false
      },
      "en-US": {
        "Word": true,
        "Excel": true,
        "PowerPoint": true,
        "Outlook": true,
        "Access": true,
        "Publisher": true,
        "OneNote": true,
        "Teams": true,
        "OneDrive": true,
        "Groove": false,
        "Lync": false,
        "Project": false,
        "Visio": false
      }
    },
    "2021": {
      "es-ES": {
        "Word": true,
        "Excel": true,
        "PowerPoint": true,
        "Outlook": true,
        "Access": true,
        "Publisher": true,
        "OneNote": true,
        "Teams": true,
        "OneDrive": true,
        "Groove": true,
        "Lync": false,
        "Project": false,
        "Visio": false
      },
      "en-US": {
        "Word": true,
        "Excel": true,
        "PowerPoint": true,
        "Outlook": true,
        "Access": true,
        "Publisher": true,
        "OneNote": true,
        "Teams": true,
        "OneDrive": true,
        "Groove": true,
        "Lync": false,
        "Project": false,
        "Visio": false
      }
    },
    "2019": {
      "es-ES": {
        "Word": true,
        "Excel": true,
        "PowerPoint": true,
        "Outlook": true,
        "Access": true,
        "Publisher": true,
        "OneNote": true,
        "Teams": false,
        "OneDrive": true,
        "Groove": true,
        "Lync": true,
        "Project": false,
        "Visio": false
      },
      "en-US": {
        "Word": true,
        "Excel": true,
        "PowerPoint": true,
        "Outlook": true,
        "Access": true,
        "Publisher": true,
        "OneNote": true,
        "Teams": false,
        "OneDrive": true,
        "Groove": true,
        "Lync": true,
        "Project": false,
        "Visio": false
      }
    }
  },
  "bugsFixed": {
    "microsoftOCTBug": "Previene selección de idiomas incompatibles con aplicaciones (Project, Visio)"
  }
}
```

---

## Implicaciones para UC-004

### Validación Punto 6: Combinación Idioma + Aplicación

En UC-004, después de validar que idioma existe y está soportado en versión:

```powershell
# Pseudo-código UC-004, Punto 6
function Test-LanguageApplicationCompatibility {
    param(
        [string]$Version,
        [string]$Language,
        [string[]]$Applications
    )
    
    # Cargar matriz de compatibilidad
    $matrix = Get-CompatibilityMatrix
    
    # Para cada aplicación seleccionada
    foreach ($app in $Applications) {
        # Verificar: [Version][Language][App] = true
        $compatible = $matrix[$Version][$Language][$app]
        
        if (-not $compatible) {
            # FAIL-FAST: Error temprano
            throw "[CRITICO] Incompatible: $Language no soportado en $App para versión $Version"
        }
    }
    
    return $true  # Todas las combinaciones son válidas
}
```

---

## Casos de Uso (Testing UC-004)

### Caso 1: Válido

```
Usuario selecciona: Office 2024 + es-ES + Word, Excel, Teams
Validación punto 6:
  - [2024][es-ES][Word] = true   [OK]
  - [2024][es-ES][Excel] = true  [OK]
  - [2024][es-ES][Teams] = true  [OK]
Resultado: PASS
```

### Caso 2: Inválido (Anti-Microsoft-Bug)

```
Usuario selecciona: Office 2024 + es-ES + Project
Validación punto 6:
  - [2024][es-ES][Project] = false  [FALLO]
Error: [CRITICO] Project no soportado en Office 2024
        (Requiere licencia volumen)
Resultado: FAIL-FAST
```

### Caso 3: Inválido (Versión antigua)

```
Usuario selecciona: Office 2019 + es-ES + Teams
Validación punto 6:
  - [2019][es-ES][Teams] = false  [FALLO]
Error: [BLOQUEADOR] Teams no disponible en Office 2019
        (Usar versión separada o actualizar a 2021+)
Resultado: FAIL-FAST
```

---

## Actualización de Matriz (Roadmap)

### v1.1 (En futuro):

- Agregar en-GB, fr-FR, de-DE, it-IT, pt-BR, ja-JP
- Validación de Project/Visio (con licencia volumen)
- Actualizar si Microsoft cambia compatibilidades

### Proceso de Actualización:

1. Revisar documentación oficial Microsoft
2. Actualizar matriz JSON
3. Ejecutar tests (UC-004 debe pasar)
4. Commit con convención: `feat(compat): actualizar matriz idiomas`
5. Bump version MINOR (1.1.0)

---

## Referencias

- **Documentación Microsoft:** https://support.microsoft.com/office/26d30382-9fba-45dd-bf55-02ab03e2a7ec
- **Versiones LTSC:** https://learn.microsoft.com/en-us/windows/release-health/ltsc
- **OCT Documentation:** https://learn.microsoft.com/es-es/microsoft-365-apps/deploy/overview-office-customization-tool
- **Microsoft Bug Analysis:** analysis-microsoft-oct.md

---

**Versión:** 1.0.0
**Última actualización:** 2026-04-21
**Estado:** VALIDADA (lista para UC-004)

