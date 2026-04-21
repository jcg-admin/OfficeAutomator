# OfficeAutomator

**Automatizador inteligente de instalación y configuración de Microsoft Office LTSC**

Version: 1.0.0 (Planificación)  
License: MIT  
PowerShell: 5.1+

---

## Descripción

OfficeAutomator es un módulo PowerShell profesional que automatiza e simplifica la instalación y configuración de Microsoft Office LTSC (Long Term Service Channel) en entornos empresariales. Actúa como un wrapper inteligente sobre la Office Deployment Tool (ODT) de Microsoft, agregando:

- Interfaz gráfica interactiva (WPF)
- Interfaz CLI con menús
- Validación robusta de entrada
- Sistema de logging completo
- Manejo de errores mejorado
- Idempotencia garantizada

---

## Características principales

### 1. Instalación Simplificada
- Descarga automática de ODT desde servidores Microsoft
- Generación inteligente de configuration.xml
- Soporte para múltiples versiones (2024, 2021, 2019)
- Soporte para múltiples idiomas

### 2. Control Avanzado
- Selección granular de aplicaciones
- Exclusión automática de componentes no deseados (Teams, OneDrive, etc.)
- Soporta productos adicionales (Project, Visio)
- Opciones de arquitectura (32/64 bits)

### 3. Confiabilidad
- Validación de integridad de archivos
- Cache persistente de descargas
- Manejo automático de conflictos
- Recuperación ante fallos
- Logging detallado de todas las operaciones

### 4. Profesionalismo
- Interfaz WPF moderna
- Progreso visual en tiempo real
- Mensajes de error claros y accionables
- Documentación completa
- Ejemplos de uso

---

## Requisitos

### Sistema
- **Sistema Operativo:** Windows 10/11, Windows Server 2016+
- **PowerShell:** 5.1 o superior
- **Permisos:** Administrador
- **Conexión:** Internet (para descargar ODT)

### Técnicos
- .NET Framework 4.5+
- WPF (Windows Presentation Foundation)
- Acceso a Office CDN de Microsoft

---

## Instalación rápida

### Opción 1: Copiar módulo

```powershell
# Copiar carpeta OfficeAutomator a Modules
$modulePath = Join-Path $env:USERPROFILE 'Documents\PowerShell\Modules\OfficeAutomator'
Copy-Item -Path ".\OfficeAutomator" -Destination $modulePath -Recurse

# Importar
Import-Module OfficeAutomator
```

### Opción 2: Ejecutar directamente

```powershell
cd C:\Path\To\OfficeAutomator
Import-Module .\OfficeAutomator.psd1
```

### Opción 3: Usar script ejecutable

```powershell
cd C:\Path\To\OfficeAutomator
.\Install-Office.ps1
```

---

## Uso rápido

### GUI (Recomendado)

```powershell
Import-Module OfficeAutomator
Invoke-OfficeAutomatorGUI
```

### CLI

```powershell
Import-Module OfficeAutomator
Invoke-OfficeAutomator -SourcePath "C:\Downloads" -Version 2024 -Language es-ES
```

### Script directo

```powershell
.\Install-Office.ps1
```

---

## Estructura del proyecto

```
OfficeAutomator/
├── docs/                           ← Documentación arquitectónica
│   ├── INDEX.md                    ← Mapa maestro
│   ├── introduction/               ← Descripción del proyecto
│   ├── requirements/               ← Requisitos funcionales
│   ├── architecture/               ← Diseño y componentes
│   ├── quality-scenarios/          ← Tests y validaciones
│   ├── risks-technical-debt/       ← Riesgos conocidos
│   └── ...
│
├── Functions/                      ← Funciones PowerShell
│   ├── Private/                    ← Privadas (uso interno)
│   └── Public/                     ← Públicas (API expuesta)
│
├── Tools/                          ← Utilidades y helpers
│   └── README.md                   ← Documentación de tools
│
├── Logs/                           ← Logs de ejecución
│   └── .gitkeep
│
├── Tests/                          ← Tests unitarios y funcionales
│
├── OfficeAutomator.psd1           ← Manifiesto del módulo
├── OfficeAutomator.psm1           ← Importador de funciones
├── Install-Office.ps1              ← Script ejecutable principal
├── README.md                       ← Este archivo
├── CHANGELOG.md                    ← Historial de versiones
└── LICENSE                         ← Licencia MIT
```

---

## Documentación

- **[docs/INDEX.md](./docs/INDEX.md)** - Mapa completo de documentación
- **[docs/introduction/](./docs/introduction/)** - Visión y objetivos
- **[docs/requirements/](./docs/requirements/)** - Funcionalidades
- **[docs/architecture/](./docs/architecture/)** - Diseño técnico

---

## Diferencias vs MSOI (Proyecto anterior)

| Aspecto | MSOI | OfficeAutomator |
|---------|------|-----------------|
| **Validación de entrada** | No robusta | Validada completamente |
| **Integridad de archivos** | No verificada | SHA256 validado |
| **Idempotencia** | No garantizada | Garantizada |
| **Logging** | Limitado (GUI solo) | Completo en CLI y GUI |
| **Limpieza temporal** | No realiza | Automática |
| **Timeouts** | Insuficientes | Configurables y mejorados |
| **Documentación** | README solo | Arquitectónica completa |
| **Estructura** | Monolítica | Modular (funciones) |

---

## Roadmap

### v1.0 (Actual)
- Funcionalidad base
- GUI WPF mejorada
- Logging completo
- Documentación arquitectónica
- Tests básicos

### v1.1 (Planificado)
- Caché mejorado
- Reportes de instalación
- Historial de descargas

### v2.0 (Futuro)
- Soporte para Office 365 (cloud)
- Desinstalación automática de versiones previas
- Plantillas personalizables
- API REST

---

## Contribución

Las contribuciones son bienvenidas.

1. Fork el repositorio
2. Crea una rama: `git checkout -b feature/nueva-funcionalidad`
3. Commit: `git commit -am 'Añade funcionalidad'`
4. Push: `git push origin feature/nueva-funcionalidad`
5. Pull Request

Ver [docs/_methodology/](./docs/_methodology/) para convenciones.

---

## Soporte

- **Issues:** GitHub Issues
- **Documentación:** [docs/](./docs/)
- **Troubleshooting:** [docs/risks-technical-debt/](./docs/risks-technical-debt/)

---

## Licencia

MIT License - Ver [LICENSE](./LICENSE) para detalles completos.

---

## Agradecimientos

- Microsoft por Office Deployment Tool
- Comunidad PowerShell
- Inspiración en OrganizeFiles

---

**OfficeAutomator v1.0** - 2026

Automatizando la complejidad de Office.
