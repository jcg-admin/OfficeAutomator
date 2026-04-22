```yml
type: Guía de Desarrollo
language: PowerShell 5.1+
layer: Layer 1 — Orchestration
version: 1.0.0
purpose: Define clean code standards for PowerShell in OfficeAutomator
applies_to: scripts/, tests/PowerShell/
updated_at: 2026-04-22 16:00:00
```

# GUÍA DE DESARROLLO — PowerShell 5.1+

**Clean Code Principles for OfficeAutomator Orchestration Layer**

---

## TABLA DE CONTENIDOS

1. [Filosofía General](#filosofía-general)
2. [Convenciones PowerShell Específicas](#convenciones-powershell-específicas)
3. [Clean Code Principles](#clean-code-principles)
4. [Estructura de Funciones](#estructura-de-funciones)
5. [Nombres Revelan Intención](#nombres-revelan-intención)
6. [Parámetros y Argumentos](#parámetros-y-argumentos)
7. [Manejo de Errores](#manejo-de-errores)
8. [Comentarios y Documentación](#comentarios-y-documentación)
9. [Pruebas (Pester)](#pruebas-pester)
10. [Anti-Patrones PowerShell](#anti-patrones-powershell)
11. [Checklist de Calidad](#checklist-de-calidad)

---

## FILOSOFÍA GENERAL

### Core Principle

**Orchestrate Office installation with clear, maintainable PowerShell scripts.**

PowerShell es la capa de orquestación. Debe ser:

```
THREE PILLARS:

1. READABILITY
   - Nombres autoevidentes
   - Funciones pequeñas (<30 líneas)
   - Sin abbreviations

2. DEBUGGABILITY
   - Verbose output apropiado
   - Error handling explícito
   - Logging en puntos clave

3. MAINTAINABILITY
   - Scripts modular
   - Sin duplicación
   - Documentation comments
```

---

## CONVENCIONES POWERSHELL ESPECÍFICAS

### Indentación

**Obligatorio: 4 espacios (no tabs)**

```powershell
function Invoke-OfficeAutomator {
    param(
        [string]$ConfigPath
    )
    
    Write-Host "Starting installation..."
    
    if (Test-Path $ConfigPath) {
        Write-Host "Configuration found"
    }
    else {
        Write-Error "Configuration not found"
    }
}
```

### Nomenclatura de Funciones

**Formato: `{Verb}-{Noun}` (Verbos aprobados de PowerShell)**

| Verbo | Uso | Ejemplo |
|-------|-----|---------|
| `Get-` | Obtener datos | `Get-OfficeVersion` |
| `Set-` | Establecer valor | `Set-Configuration` |
| `Test-` | Probar/validar | `Test-ConfigurationValidity` |
| `Invoke-` | Ejecutar acción | `Invoke-OfficeAutomator` |
| `New-` | Crear objeto | `New-ConfigurationFile` |
| `Remove-` | Eliminar | `Remove-OfficeCache` |
| `Write-` | Escribir/mostrar | `Write-Log` |
| `Add-` | Agregar | `Add-Language` |

```powershell
# ✓ BIEN - Verbos aprobados
function Get-SupportedVersions { }
function Test-Configuration { }
function Invoke-Installation { }
function Write-ExecutionLog { }

# ❌ INCORRECTO - Nombres no estándar
function GetVersions { }
function CheckConfig { }
function DoInstall { }
function LogExecution { }
```

### Convención de Variables

```powershell
# ✓ BIEN - Descriptivas
$configurationPath = "C:\Temp\config.xml"
$isConfigurationValid = $true
$maxRetryCount = 3
$supportedVersions = @("2024", "2021", "2019")

# ❌ INCORRECTO - Abreviadas
$cfg = "C:\Temp\config.xml"
$valid = $true
$max = 3
$versions = @("2024", "2021", "2019")
```

### CamelCase para Variables, PascalCase para Funciones

```powershell
# Variables: camelCase
$configPath = "..."
$isValid = $true
$retryCount = 0

# Funciones: Verb-Noun (PascalCase)
function Get-Configuration { }
function Test-ValidConfiguration { }
function Invoke-Installation { }
```

### Línea Máxima: 100 caracteres

```powershell
# ✓ CORRECTO (95 chars)
$configuration = Get-Content -Path $configPath | 
    ConvertFrom-Json

# ❌ INCORRECTO (120+ chars)
$configuration = Get-Content -Path $configPath | ConvertFrom-Json -WarningAction SilentlyContinue -ErrorAction Continue
```

---

## CLEAN CODE PRINCIPLES

### 1. Single Responsibility Principle

**Una función = Una tarea**

```powershell
# ❌ MAL - Hace demasiado
function Invoke-Installation {
    param([string]$ConfigPath)
    
    # Validar
    Test-Configuration $ConfigPath
    
    # Descargar
    Invoke-WebRequest ...
    
    # Instalar
    Start-Process setup.exe
    
    # Loguear
    Write-Log "Done"
}

# ✓ BIEN - Responsabilidades separadas
function Invoke-Installation {
    param([string]$ConfigPath)
    
    Confirm-PrerequisitesSetupAsComplete
    Invoke-OfficeSetupProcess
    Register-InstallationCompletion
}

function Confirm-PrerequisitesSetupAsComplete {
    param([string]$ConfigPath)
    
    Test-Configuration $ConfigPath
    Test-DownloadIntegrity
}

function Invoke-OfficeSetupProcess {
    Start-Process setup.exe
}

function Register-InstallationCompletion {
    Write-Log "Installation complete"
}
```

### 2. Reveal Intent

```powershell
# ❌ MAL
function Proc {
    param($c)
    
    if (Test-P $c) {
        Invoke-I $c
        Write-L "Done"
    }
    else {
        Write-L "Fail"
    }
}

# ✓ BIEN
function Invoke-OfficeAutomator {
    param([string]$ConfigPath)
    
    if (Test-ConfigurationValidity $ConfigPath) {
        Invoke-OfficeInstallation $ConfigPath
        Write-ExecutionLog "Installation completed successfully"
    }
    else {
        Write-ExecutionLog "Configuration validation failed"
        exit 1
    }
}
```

### 3. Fail-Fast Principle

```powershell
# ✓ BIEN - Validar primero
function Invoke-OfficeAutomator {
    param([string]$ConfigPath)
    
    # Validar ANTES
    if ([string]::IsNullOrEmpty($ConfigPath)) {
        throw "ConfigPath is required"
    }
    
    if (-not (Test-Path $ConfigPath)) {
        throw "Configuration file not found: $ConfigPath"
    }
    
    if (-not (Test-ConfigurationValidity $ConfigPath)) {
        throw "Configuration is invalid"
    }
    
    # LUEGO proceder
    Invoke-OfficeInstallation $ConfigPath
}
```

### 4. DRY Principle

```powershell
# ❌ MAL - Repetición
function Test-LanguageFor2024 {
    param([string]$Language)
    
    $supported = @("es-ES", "en-US", "fr-FR")
    return $supported -contains $Language
}

function Test-LanguageFor2021 {
    param([string]$Language)
    
    $supported = @("es-ES", "en-US", "fr-FR")
    return $supported -contains $Language
}

# ✓ BIEN - Función reutilizable
function Test-LanguageSupportedForVersion {
    param(
        [string]$Version,
        [string]$Language
    )
    
    $supportedLanguages = @{
        "2024" = @("es-ES", "en-US", "fr-FR")
        "2021" = @("es-ES", "en-US", "fr-FR")
        "2019" = @("es-ES", "en-US")
    }
    
    return $supportedLanguages[$Version] -contains $Language
}
```

---

## ESTRUCTURA DE FUNCIONES

### Template Estándar

```powershell
function Invoke-OfficeAutomator {
    <#
    .SYNOPSIS
        Main orchestration function for Office installation
    
    .DESCRIPTION
        Coordinates all steps: validation, download, installation, logging.
        This is the entry point for UC-001 through UC-005.
    
    .PARAMETER ConfigPath
        Path to the Office configuration XML file
    
    .PARAMETER Verbose
        If specified, enable verbose logging
    
    .OUTPUTS
        [PSCustomObject] with Success (bool), Duration (TimeSpan), Message (string)
    
    .EXAMPLE
        $result = Invoke-OfficeAutomator -ConfigPath "C:\config.xml"
        if ($result.Success) { Write-Host "Success" }
    
    .NOTES
        Part of UC-001 (Select Version) through UC-005 (Install Office)
        Related: docs/requirements/uc-001-select-version/
    #>
    
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true)]
        [string]$ConfigPath,
        
        [Parameter(Mandatory=$false)]
        [int]$MaxRetries = 3
    )
    
    begin {
        $startTime = Get-Date
        $VerbosePreference = "Continue"
    }
    
    process {
        try {
            # Validación
            Confirm-ConfigurationValidity -Path $ConfigPath
            
            # Descarga y ejecución
            Invoke-SetupProcess -ConfigPath $ConfigPath -MaxRetries $MaxRetries
            
            return @{
                Success = $true
                Duration = (Get-Date) - $startTime
                Message = "Installation completed successfully"
            }
        }
        catch {
            Write-Error "Installation failed: $_"
            
            return @{
                Success = $false
                Duration = (Get-Date) - $startTime
                Message = $_.Exception.Message
            }
        }
    }
    
    end {
        Write-Verbose "Invoke-OfficeAutomator completed in $((Get-Date) - $startTime)"
    }
}
```

### Parámetros

```powershell
# ✓ BIEN - Typed, validado, documentado
function Get-OfficeConfiguration {
    param(
        [Parameter(Mandatory=$true, ValueFromPipeline=$true)]
        [ValidateScript({ Test-Path $_ })]
        [string]$ConfigPath,
        
        [Parameter(Mandatory=$false)]
        [ValidateRange(1, 10)]
        [int]$Timeout = 5
    )
    
    process {
        Get-Content $ConfigPath | ConvertFrom-Json
    }
}

# ❌ INCORRECTO - Untyped, sin validación
function Get-OfficeConfiguration {
    param($configPath, $timeout)
    
    # ¿Qué tipo es $configPath?
    Get-Content $configPath | ConvertFrom-Json
}
```

---

## NOMBRES REVELAN INTENCIÓN

### Funciones Test (Booleanas)

**Patrón: `Test-{Noun}{Condition}`**

```powershell
# ✓ BIEN
function Test-ConfigurationValidity { }
function Test-DownloadIntegrity { }
function Test-LanguageSupport { }
function Test-SystemPrerequisites { }

# ❌ INCORRECTO
function Validate { }
function CheckConfig { }
function LanguageOK { }
function Prerequisites { }
```

### Funciones Get (Lectores)

```powershell
# ✓ BIEN
function Get-SupportedVersions { }
function Get-InstalledLanguages { }
function Get-ExecutionLogs { }

# ❌ INCORRECTO
function GetVersions { }
function FetchLanguages { }
function ReadLogs { }
```

---

## PARÁMETROS Y ARGUMENTOS

### Parámetros Obligatorios vs Opcionales

```powershell
# ✓ BIEN - Claro cuál es cuál
function Invoke-Installation {
    param(
        [Parameter(Mandatory=$true)]
        [string]$ConfigPath,
        
        [Parameter(Mandatory=$false)]
        [int]$MaxRetries = 3,
        
        [Parameter(Mandatory=$false)]
        [switch]$Verbose
    )
    
    # Implementation
}

# ❌ INCORRECTO - Ambiguo
function Invoke-Installation {
    param([string]$c, [int]$r = 3, [switch]$v)
    
    # ¿Cuál es obligatorio?
}
```

### Pipeline Support

```powershell
# ✓ BIEN - Acepta pipeline
function Get-Configuration {
    [CmdletBinding()]
    param(
        [Parameter(
            Mandatory=$true,
            ValueFromPipeline=$true,
            ValueFromPipelineByPropertyName=$true
        )]
        [string]$Path
    )
    
    process {
        Get-Content $Path | ConvertFrom-Json
    }
}

# Uso
"C:\config.xml" | Get-Configuration
```

---

## MANEJO DE ERRORES

### Try-Catch Explícito

```powershell
# ✓ BIEN - Catch específico
function Invoke-Installation {
    try {
        $config = Get-Content $ConfigPath | ConvertFrom-Json
        Invoke-Setup $config
    }
    catch [System.IO.FileNotFoundException] {
        Write-Error "Configuration file not found: $ConfigPath"
        exit 1
    }
    catch [System.InvalidOperationException] {
        Write-Error "Invalid configuration format"
        exit 1
    }
    catch {
        Write-Error "Unexpected error: $_"
        exit 1
    }
}

# ❌ INCORRECTO - Catch genérico
function Invoke-Installation {
    try {
        $config = Get-Content $ConfigPath | ConvertFrom-Json
    }
    catch {
        Write-Host "Error"  # Silencia el error
    }
}
```

### ErrorActionPreference

```powershell
# ✓ BIEN - Explícito
function Invoke-Installation {
    param([string]$ConfigPath)
    
    $previousErrorActionPreference = $ErrorActionPreference
    $ErrorActionPreference = "Stop"
    
    try {
        Test-ConfigurationValidity $ConfigPath
        Invoke-SetupProcess $ConfigPath
    }
    finally {
        $ErrorActionPreference = $previousErrorActionPreference
    }
}
```

---

## COMENTARIOS Y DOCUMENTACIÓN

### Obligatorio: Comment-Based Help

```powershell
function Invoke-OfficeAutomator {
    <#
    .SYNOPSIS
        Orchestrates Office LTSC installation process
    
    .DESCRIPTION
        Executes UC-001 through UC-005 workflow:
        1. Select version and language
        2. Exclude applications
        3. Validate configuration
        4. Execute installation
    
    .PARAMETER ConfigPath
        Path to generated configuration.xml
    
    .PARAMETER MaxRetries
        Maximum download retry attempts (default: 3)
    
    .OUTPUTS
        PSCustomObject with Success, Duration, Message properties
    
    .EXAMPLE
        Invoke-OfficeAutomator -ConfigPath "C:\config.xml"
    
    .NOTES
        Requires administrator privileges
        Part of OfficeAutomator v1.0.0
    #>
    
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true)]
        [string]$ConfigPath,
        
        [Parameter(Mandatory=$false)]
        [int]$MaxRetries = 3
    )
    
    # Implementation
}
```

### NO para Privado

```powershell
# ✓ BIEN - Sin comentarios para lógica obvia
$supportedVersions = @("2024", "2021", "2019")

# ❌ INCORRECTO - Comentarios innecesarios
# This is a comment explaining what the line does
$supportedVersions = @("2024", "2021", "2019")
```

---

## PRUEBAS (PESTER)

### Estructura Test

```powershell
Describe "Invoke-OfficeAutomator" {
    
    Context "When called with valid configuration" {
        
        It "should return success" {
            $result = Invoke-OfficeAutomator -ConfigPath "valid.xml"
            $result.Success | Should -Be $true
        }
    }
    
    Context "When called with invalid path" {
        
        It "should throw FileNotFoundException" {
            { Invoke-OfficeAutomator -ConfigPath "invalid.xml" } | 
                Should -Throw -ExceptionType System.IO.FileNotFoundException
        }
    }
    
    Context "When configuration is invalid" {
        
        It "should return failure" {
            $result = Invoke-OfficeAutomator -ConfigPath "malformed.xml"
            $result.Success | Should -Be $false
        }
    }
}
```

### Naming Pattern

**`{FunctionName}_{Scenario}_{ExpectedResult}`**

```powershell
It "Invoke-OfficeAutomator_WithValidConfiguration_ReturnsSuccess" { }
It "Invoke-OfficeAutomator_WithMissingFile_ThrowsFileNotFoundException" { }
It "Test-ConfigurationValidity_WithNullInput_ReturnsFalse" { }
```

---

## ANTI-PATRONES POWERSHELL

### 1. Magic Strings

```powershell
# ❌ MAL
if ($version -eq "2024") { ... }
$path = "C:\Program Files\Microsoft Office"

# ✓ BIEN
$SupportedVersion2024 = "2024"
$OfficeInstallationPath = "C:\Program Files\Microsoft Office"

if ($version -eq $SupportedVersion2024) { ... }
```

### 2. Positional Parameters Sin Tipo

```powershell
# ❌ MAL
function Process {
    param($config, $retries, $verbose)
    
    # ¿Qué tipo es cada parámetro?
}

# ✓ BIEN
function Process {
    param(
        [System.IO.FileInfo]$ConfigFile,
        [int]$MaxRetries = 3,
        [switch]$VerboseOutput
    )
}
```

### 3. Silenciar Errores

```powershell
# ❌ MAL - Silencia errores
$result = Get-Content $ConfigPath -ErrorAction SilentlyContinue
if ($result -eq $null) {
    Write-Host "Config not found"  # Sin info
}

# ✓ BIEN - Explícito
try {
    $result = Get-Content $ConfigPath -ErrorAction Stop
}
catch [System.IO.FileNotFoundException] {
    Write-Error "Configuration file not found: $ConfigPath"
    exit 1
}
```

### 4. Global Variables

```powershell
# ❌ MAL
$global:ConfigPath = "C:\config.xml"
$global:LogPath = "C:\logs"

function Get-Configuration {
    Get-Content $global:ConfigPath  # Acoplamiento global
}

# ✓ BIEN - Parámetros
function Get-Configuration {
    param([string]$ConfigPath)
    
    Get-Content $ConfigPath
}
```

---

## CHECKLIST DE CALIDAD

### Antes de Commit

- [ ] Función tiene responsabilidad única
- [ ] Nombres revelan intención
- [ ] Parámetros son tipados
- [ ] Parámetros marcados Mandatory/$false explícitamente
- [ ] Comment-based help presente
- [ ] Try-catch con excepciones específicas
- [ ] Sin variables globales
- [ ] Sin magic strings (constantes definidas)
- [ ] Tests escritos y pasando (Pester)
- [ ] Verbose logging en puntos clave
- [ ] Sin código muerto
- [ ] Máximo 30 líneas por función
- [ ] Máximo 100 caracteres por línea

### Antes de Pull Request

- [ ] Todos los tests Pester pasan
- [ ] Invoke-ScriptAnalyzer sin warnings
- [ ] Documentación actualizada
- [ ] CHANGELOG.md actualizado
- [ ] Sin comentarios "WIP" o "TODO" abandonados

---

## RESUMEN

**Five Core PowerShell Clean Code Principles:**

1. **CLARITY:** Nombres Verb-Noun, sin abbreviations
2. **SRP:** Una función = una tarea
3. **DEBUGGABILITY:** Logging y error handling explícito
4. **TESTABILITY:** Pester tests para cada función
5. **MAINTAINABILITY:** Comment-based help, estructura clara

**PowerShell es la capa de orquestación. Cada línea debe ser legible.**

---

**Versión:** 1.0.0  
**Última actualización:** 2026-04-22  
**Aplicable a:** Layer 1 — OfficeAutomator Scripts (PowerShell 5.1+)
