```yml
type: Guía de Desarrollo
language: C# (.NET 8.0)
layer: Layer 2 — Core Logic
version: 1.0.0
purpose: Define clean code standards for C# in OfficeAutomator
applies_to: src/OfficeAutomator.Core/, tests/OfficeAutomator.Core.Tests/
updated_at: 2026-04-22 16:00:00
```

# GUÍA DE DESARROLLO — C# (.NET 8.0)

**Clean Code Principles for OfficeAutomator Core Logic**

---

## TABLA DE CONTENIDOS

1. [Filosofía General](#filosofía-general)
2. [Convenciones C# Específicas](#convenciones-c-específicas)
3. [Clean Code Principles](#clean-code-principles)
4. [Estructura de Clases](#estructura-de-clases)
5. [Métodos y Funciones](#métodos-y-funciones)
6. [Nombres Revelan Intención](#nombres-revelan-intención)
7. [Manejo de Errores](#manejo-de-errores)
8. [Documentación XML](#documentación-xml)
9. [Pruebas Unitarias (TDD)](#pruebas-unitarias-tdd)
10. [Async/Await Patterns](#asyncawait-patterns)
11. [Anti-Patrones en C#](#anti-patrones-en-c)
12. [Checklist de Calidad](#checklist-de-calidad)

---

## FILOSOFÍA GENERAL

### Core Principle

**Automate Office installation with clear, maintainable, testable C# code.**

El código C# es la capa de lógica núcleo. Debe ser:

```
THREE PILLARS:

1. CLARITY
   - Nombres autoevidentes
   - Métodos pequeños (<20 líneas)
   - Responsabilidad única

2. TESTABILITY
   - Constructor injection (sin hardcoding)
   - Métodos puros cuando sea posible
   - Interfaces para mocks

3. MAINTAINABILITY
   - Sin código duplicado
   - Patrones consistentes
   - Documentación XML clara
```

---

## CONVENCIONES C# ESPECÍFICAS

### Indentación y Espaciado

**Obligatorio:**
- 4 espacios por nivel de indentación (NOT tabs)
- Usar EditorConfig en .editorconfig

```csharp
public class OfficeAutomator
{
    private readonly ILogger _logger;    // 4 spaces
    
    public OfficeAutomator(ILogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public async Task<bool> ExecuteAsync()
    {
        var result = await ValidateAsync();
        if (!result)
        {
            return false;
        }
        
        return true;
    }
}
```

### Convención de Nomenclatura

| Tipo | Convención | Ejemplo | Explicación |
|------|-----------|---------|------------|
| **Clase** | PascalCase | `OfficeAutomatorStateMachine` | Always PascalCase |
| **Interface** | I + PascalCase | `IConfigValidator` | Prefix with I |
| **Método público** | PascalCase | `ValidateConfiguration()` | Always PascalCase |
| **Método privado** | PascalCase | `ValidateLanguageSupport()` | Same as public |
| **Variable local** | camelCase | `isValid`, `configPath` | Lowercase first letter |
| **Campo privado** | _camelCase | `_logger`, `_stateMachine` | Underscore prefix |
| **Constante** | SCREAMING_SNAKE_CASE | `MAX_RETRY_COUNT` | All caps |
| **Enum** | PascalCase | `InstallerState.Ready` | PascalCase members |
| **Namespace** | PascalCase | `OfficeAutomator.Core.Services` | Dots for hierarchy |

### Llaves y Braces

**Obligatorio: Always on New Line (Allman style)**

```csharp
// ✓ CORRECTO
public void ValidateConfiguration()
{
    if (configuration == null)
    {
        throw new ArgumentNullException(nameof(configuration));
    }
    
    foreach (var app in configuration.Applications)
    {
        ValidateApplication(app);
    }
}

// ❌ INCORRECTO (K&R style)
public void ValidateConfiguration() {
    if (configuration == null) {
        throw new ArgumentNullException(nameof(configuration));
    }
}
```

### Línea Máxima

**100 caracteres por línea** (hard limit)

```csharp
// ✓ CORRECTO (95 chars)
public async Task<ConfigurationResult> ValidateAndGenerateConfigAsync(
    OfficeConfiguration config,
    string outputPath)
{
    // implementation
}

// ❌ INCORRECTO (120+ chars)
public async Task<ConfigurationResult> ValidateAndGenerateConfigurationFileFromUserInputAndOutputToSystemPathAsync(OfficeConfiguration config) { ... }
```

---

## CLEAN CODE PRINCIPLES

### 1. Single Responsibility Principle (SRP)

**Una clase = Una responsabilidad**

```csharp
// ❌ MAL - Hace demasiado
public class OfficeInstaller
{
    public void ValidateConfiguration() { /* ... */ }
    public void GenerateXml() { /* ... */ }
    public void ExecuteInstallation() { /* ... */ }
    public void LogResults() { /* ... */ }
    public void SaveToDatabase() { /* ... */ }
}

// ✓ BIEN - Responsabilidades separadas
public class ConfigValidator
{
    public bool Validate(OfficeConfiguration config) { /* ... */ }
}

public class ConfigurationXmlGenerator
{
    public string Generate(OfficeConfiguration config) { /* ... */ }
}

public class InstallationExecutor
{
    public async Task<bool> ExecuteAsync(string configPath) { /* ... */ }
}

public class ExecutionLogger
{
    public void LogResult(ExecutionResult result) { /* ... */ }
}
```

### 2. Reveal Intent (Nombres Descriptivos)

```csharp
// ❌ MAL
public class OfficeAutomator
{
    private bool v;
    private int rc;
    private string cfg;
    
    public bool Process(string c, int mx)
    {
        // ¿Qué significa v, rc, cfg, c, mx?
    }
}

// ✓ BIEN
public class OfficeAutomator
{
    private bool isConfigurationValid;
    private int maxRetryCount;
    private string configurationPath;
    
    public bool ProcessInstallation(string configPath, int maxRetries)
    {
        // Intención clara
    }
}
```

### 3. Fail-Fast Principle

**Validar ANTES de actuar**

```csharp
// ✓ BIEN - Fail-fast
public class ConfigValidator
{
    public bool Validate(OfficeConfiguration config)
    {
        // Validar primero
        if (config == null)
            throw new ArgumentNullException(nameof(config));
        
        if (string.IsNullOrEmpty(config.Version))
            throw new InvalidOperationException("Version is required");
        
        if (!IsVersionSupported(config.Version))
            return false;
        
        // Luego proceder
        return ValidateLanguages(config.Languages);
    }
}
```

### 4. DRY Principle (Don't Repeat Yourself)

```csharp
// ❌ MAL - Repetición
public class LanguageValidator
{
    public bool IsLanguageSupportedFor2024(string lang)
    {
        return lang == "es-ES" || lang == "en-US" || lang == "fr-FR";
    }
    
    public bool IsLanguageSupportedFor2021(string lang)
    {
        return lang == "es-ES" || lang == "en-US" || lang == "fr-FR";
    }
    
    public bool IsLanguageSupportedFor2019(string lang)
    {
        return lang == "es-ES" || lang == "en-US" || lang == "fr-FR";
    }
}

// ✓ BIEN - Función reutilizable
public class LanguageValidator
{
    private readonly Dictionary<string, string[]> _supportedLanguagesByVersion = 
        new()
        {
            { "2024", new[] { "es-ES", "en-US", "fr-FR" } },
            { "2021", new[] { "es-ES", "en-US", "fr-FR" } },
            { "2019", new[] { "es-ES", "en-US", "fr-FR" } }
        };
    
    public bool IsLanguageSupported(string version, string language)
    {
        return _supportedLanguagesByVersion.TryGetValue(version, out var langs)
            && langs.Contains(language);
    }
}
```

### 5. Keep It Simple (KISS)

```csharp
// ❌ MAL - Overcomplicated
public bool ValidateConfiguration(OfficeConfiguration config)
{
    var result = true;
    try
    {
        if (config != null && !string.IsNullOrEmpty(config.Version) 
            && (config.Version == "2024" || config.Version == "2021" 
            || config.Version == "2019"))
        {
            result = true;
        }
        else
        {
            result = false;
        }
    }
    catch
    {
        result = false;
    }
    return result;
}

// ✓ BIEN - Simple y claro
public bool ValidateConfiguration(OfficeConfiguration config)
{
    if (config?.Version == null)
        return false;
    
    var validVersions = new[] { "2024", "2021", "2019" };
    return validVersions.Contains(config.Version);
}
```

---

## ESTRUCTURA DE CLASES

### Orden de Miembros

```csharp
public class ConfigurationValidator
{
    // 1. Constantes y campos estáticos
    private const int MaxRetries = 3;
    private static readonly ILogger Logger = LoggerFactory.Create();
    
    // 2. Campos privados (readonly primero)
    private readonly IConfigValidator _validator;
    private readonly IFileService _fileService;
    
    // 3. Propiedades (auto-properties, then manual)
    public string ConfigPath { get; }
    
    public bool IsValid
    {
        get => _isValid;
        private set => _isValid = value;
    }
    
    // 4. Constructores (público primero, luego privado)
    public ConfigurationValidator(IConfigValidator validator, IFileService fileService)
    {
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
    }
    
    private ConfigurationValidator(string configPath)
    {
        ConfigPath = configPath;
    }
    
    // 5. Métodos públicos
    public async Task<bool> ValidateAsync(string configPath)
    {
        return await InternalValidateAsync(configPath);
    }
    
    // 6. Métodos privados
    private async Task<bool> InternalValidateAsync(string configPath)
    {
        // implementation
    }
    
    // 7. Operadores y comparadores (si aplica)
    public static bool operator ==(ConfigurationValidator a, ConfigurationValidator b)
    {
        return a?.ConfigPath == b?.ConfigPath;
    }
}
```

---

## MÉTODOS Y FUNCIONES

### Tamaño Ideal

**Máximo 20 líneas por método**

```csharp
// ❌ MAL (50 líneas en un método)
public void InstallOffice(Configuration config)
{
    // Validación (10 líneas)
    // Descargar (15 líneas)
    // Configurar (15 líneas)
    // Ejecutar (10 líneas)
}

// ✓ BIEN (cada método ~5-10 líneas)
public async Task InstallOfficeAsync(Configuration config)
{
    ValidateConfiguration(config);
    await DownloadOdtAsync(config);
    await ConfigureInstallationAsync(config);
    await ExecuteSetupAsync(config);
}

private void ValidateConfiguration(Configuration config)
{
    if (config == null)
        throw new ArgumentNullException(nameof(config));
    
    if (!IsVersionSupported(config.Version))
        throw new InvalidOperationException($"Unsupported version: {config.Version}");
}
```

### Parámetros

**Máximo 3 parámetros; si hay más, usar objeto**

```csharp
// ❌ MAL (5 parámetros)
public bool Configure(string version, string language, string[] exclusions, 
                      bool useProxy, string proxyUrl)
{
    // Demasiados parámetros
}

// ✓ BIEN (1 parámetro objeto)
public bool Configure(ConfigurationRequest request)
{
    // Parámetro único, claro
}

public class ConfigurationRequest
{
    public string Version { get; set; }
    public string Language { get; set; }
    public string[] ExcludedApplications { get; set; }
    public ProxySettings Proxy { get; set; }
}
```

### Retorno de Valores

```csharp
// ❌ MAL - Retorna bool y modifica estado
public bool ValidateAndUpdate(Configuration config)
{
    if (!IsValid(config))
        return false;
    
    _configuration = config;  // Side effect!
    return true;
}

// ✓ BIEN - Puro (no modifica estado)
public bool Validate(Configuration config)
{
    return IsVersionValid(config.Version) 
        && IsLanguageValid(config.Language);
}

public void UpdateConfiguration(Configuration config)
{
    _configuration = config;
}
```

---

## NOMBRES REVELAN INTENCIÓN

### Criterios de Balance

Nombres en C# deben equilibrar:

| Criterio | Target | Nota |
|----------|--------|------|
| **Revela intención** | Claro | Contexto sin verbosidad excesiva |
| **Pronunciable** | 2-4 palabras máximo | Evita nombres imposibles de leer en voz alta |
| **Buscable** | Específico | grep-friendly, IntelliSense-friendly |
| **Scope apropiado** | Variable | Variables locales pueden ser más cortas |
| **Convención .NET** | Requerido | camelCase (local), PascalCase (public) |

### Métodos Booleanos

**Usar prefijos: `Is`, `Has`, `Can`, `Should` (máximo 3 palabras)**

```csharp
// ✓ BIEN - Claro, pronunciable, 2-3 palabras
public bool IsVersionSupported(string version) { }
public bool HasConfig() { }
public bool CanInstall() { }
public bool ShouldRetry(int attempts) { }

// ⚠ DEMASIADO VERBOSO - Dissertación
public bool IsVersionSupported_AndNotDeprecated_AndCompatibleWithCurrentOS() { }
public bool HasValidAndCompleteConfiguration() { }

// ❌ INCORRECTO - Ambiguo o sin sufijo boolean
public bool Version(string v) { }
public bool Valid() { }
```

### Métodos de Acción

**Usar verbos claros: `Get`, `Set`, `Create`, `Execute`, `Validate` (máximo 3 palabras)**

```csharp
// ✓ BIEN - Claro, 2-3 palabras
public string GetConfigPath() { }
public void SetLogLevel(LogLevel level) { }
public Configuration CreateDefault() { }
public async Task ExecuteAsync() { }
public ValidationResult Validate(string lang) { }

// ⚠ VERBOSIDAD - Demasiadas palabras
public string GetCompletePathToConfigurationFileInSystemDirectory() { }
public Configuration CreateDefaultConfigurationWithAllRequiredSettings() { }
public async Task ExecuteTheOfficeInstallationProcessAsync() { }

// ❌ AMBIGUO
public string Config() { }
public void Log(LogLevel l) { }
public Configuration Default() { }
public async Task DoAsync() { }
```

### Variables Locales

**Balance scope vs. claridad:**

```csharp
// ✓ BIEN - Variables locales pueden ser más cortas (scope es claro)
public void ProcessConfig(string path)
{
    var isValid = _validator.Validate(path);
    var retries = 3;
    var dir = Path.GetDirectoryName(path);
    
    // Contexto está claro: estamos en ProcessConfig
}

// ✓ BIEN - Variables con mayor scope necesitan más contexto
private IConfigValidator _validator;
private int _maxRetryCount = 3;
private string _configurationPath = "";

public void SetupPaths(string basePath)
{
    _configurationPath = Path.Combine(basePath, "config.xml");
}

// ❌ INCORRECTO - Ambiguo, sin contexto
var v = _validator.Validate(config);  // ¿v qué es?
var max = 3;                           // ¿max qué cosa?
var p = Path.Combine(dir, "config");   // ¿p qué?
```

### Convención por Scope en Métodos

```csharp
public class OfficeInstaller
{
    // Campos privados: descriptivos y contextuales
    private readonly IConfigValidator _configValidator;
    private readonly ILogger _logger;
    private int _maxRetryAttempts = 3;
    
    // Parámetros: lo más concisos posible (scope es el método)
    public ValidationResult Validate(string language, string version)
    {
        // Variables locales: cortas OK, contexto es claro
        var isSupported = CheckSupport(language, version);
        var errors = new List<string>();
        
        if (!isSupported)
        {
            errors.Add($"Language {language} not supported");
        }
        
        return new ValidationResult { IsValid = errors.Count == 0, Errors = errors };
    }
    
    // Método privado: puede ser más conciso (scope = esta clase)
    private bool CheckSupport(string lang, string ver)
    {
        return _supportedLanguages.Contains(lang) && 
               _supportedVersions.Contains(ver);
    }
}
```

---

## MANEJO DE ERRORES

### Excepciones Específicas

```csharp
// ✓ BIEN - Excepciones específicas
public void ValidateConfiguration(Configuration config)
{
    if (config == null)
        throw new ArgumentNullException(nameof(config));
    
    if (string.IsNullOrEmpty(config.Version))
        throw new InvalidOperationException("Version is required");
    
    if (!IsVersionSupported(config.Version))
        throw new NotSupportedException($"Version not supported: {config.Version}");
}

// ❌ INCORRECTO - Excepciones genéricas
public void ValidateConfiguration(Configuration config)
{
    if (config == null)
        throw new Exception("Config is null");
    
    if (string.IsNullOrEmpty(config.Version))
        throw new Exception("Version is required");
}
```

### Try-Catch Patterns

```csharp
// ✓ BIEN - Específico y logged
public async Task<bool> InstallAsync(string configPath)
{
    try
    {
        return await ExecuteSetupAsync(configPath);
    }
    catch (FileNotFoundException ex)
    {
        _logger.LogError($"Configuration file not found: {configPath}");
        return false;
    }
    catch (InvalidOperationException ex)
    {
        _logger.LogError($"Invalid configuration: {ex.Message}");
        return false;
    }
}

// ❌ INCORRECTO - Silencia errores
public async Task<bool> InstallAsync(string configPath)
{
    try
    {
        return await ExecuteSetupAsync(configPath);
    }
    catch
    {
        return false;  // Silencia todo!
    }
}
```

---

## DOCUMENTACIÓN XML

### Obligatorio para Público

```csharp
/// <summary>
/// Validates the Office configuration file.
/// </summary>
/// <param name="config">The configuration to validate</param>
/// <returns>true if valid; false otherwise</returns>
/// <exception cref="ArgumentNullException">Thrown if config is null</exception>
/// <remarks>
/// This method performs exhaustive validation including:
/// - Version support check
/// - Language compatibility verification
/// - Application exclusion validation
/// </remarks>
public bool ValidateConfiguration(OfficeConfiguration config)
{
    if (config == null)
        throw new ArgumentNullException(nameof(config));
    
    return IsValid(config);
}
```

### No para Privado

```csharp
// ✓ BIEN - Sin XML para métodos privados
private bool IsVersionSupported(string version)
{
    return _supportedVersions.Contains(version);
}

// ❌ INCORRECTO - Documentación innecesaria
/// <summary>
/// Check if version is supported
/// </summary>
private bool IsVersionSupported(string version)
{
    return _supportedVersions.Contains(version);
}
```

---

## PRUEBAS UNITARIAS (TDD)

### Red-Green-Refactor Cycle

```csharp
// RED: Test fails with specific assertion
[Fact]
public void Validate_WithNullConfiguration_ThrowsArgumentNullException()
{
    var validator = new ConfigurationValidator();
    
    Assert.Throws<ArgumentNullException>(() => 
        validator.Validate(null));
}

// GREEN: Minimal implementation passes test
public bool Validate(OfficeConfiguration config)
{
    if (config == null)
        throw new ArgumentNullException(nameof(config));
    
    return true;
}

// REFACTOR: Improve clarity without changing behavior
public bool Validate(OfficeConfiguration config)
{
    if (config == null)
        throw new ArgumentNullException(nameof(config), "Configuration cannot be null");
    
    return ValidateVersionAndLanguage(config);
}

private bool ValidateVersionAndLanguage(OfficeConfiguration config)
{
    return IsVersionSupported(config.Version) 
        && IsLanguageSupported(config.Language);
}
```

### Naming Test Methods

**Pattern: `MethodName_Scenario_ExpectedResult`**

```csharp
[Fact]
public void Validate_WithValidConfiguration_ReturnsTrue() { }

[Fact]
public void Validate_WithNullConfiguration_ThrowsArgumentNullException() { }

[Fact]
public void Validate_WithUnsupportedVersion_ReturnsFalse() { }

[Theory]
[InlineData("2024")]
[InlineData("2021")]
public void Validate_WithSupportedVersion_ReturnsTrue(string version) { }
```

---

## ASYNC/AWAIT PATTERNS

### Obligatorio para I/O

```csharp
// ✓ BIEN - Async para operaciones I/O
public async Task<Configuration> LoadConfigurationAsync(string path)
{
    var content = await File.ReadAllTextAsync(path);
    return JsonSerializer.Deserialize<Configuration>(content);
}

// ❌ INCORRECTO - Bloqueante
public Configuration LoadConfiguration(string path)
{
    var content = File.ReadAllText(path);  // Bloquea el thread
    return JsonSerializer.Deserialize<Configuration>(content);
}
```

### ConfigureAwait

```csharp
// ✓ BIEN - ConfigureAwait(false) en biblioteca
public async Task<bool> ValidateAsync(Configuration config)
{
    var result = await _validator.ValidateAsync(config)
        .ConfigureAwait(false);
    
    return result;
}
```

---

## ANTI-PATRONES EN C#

### 1. Magic Strings

```csharp
// ❌ MAL
if (config.Version == "2024") { }
var path = "C:\\Temp\\Office\\config.xml";

// ✓ BIEN
private const string SupportedVersion2024 = "2024";
private const string DefaultConfigPath = "C:\\Temp\\Office\\config.xml";

if (config.Version == SupportedVersion2024) { }
var path = DefaultConfigPath;
```

### 2. God Objects

```csharp
// ❌ MAL - Una clase que hace todo
public class OfficeAutomator
{
    public void ValidateConfiguration() { }
    public void GenerateXml() { }
    public void Download() { }
    public void Execute() { }
    public void Rollback() { }
    public void Log() { }
}

// ✓ BIEN - Responsabilidades separadas
public class ConfigValidator { }
public class XmlGenerator { }
public class OdtDownloader { }
public class InstallationExecutor { }
public class RollbackManager { }
public class ExecutionLogger { }
```

### 3. Null Reference Exceptions

```csharp
// ❌ MAL
public void Process(Configuration config)
{
    var language = config.Language;  // NullReferenceException si config es null
}

// ✓ BIEN - Null check
public void Process(Configuration config)
{
    if (config?.Language == null)
        throw new ArgumentException("Configuration language is required");
    
    var language = config.Language;
}

// ✓ MEJOR - Validar en constructor
public class Processor
{
    private readonly Configuration _config;
    
    public Processor(Configuration config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }
}
```

### 4. Mixing Concerns

```csharp
// ❌ MAL - UI y lógica mezcladas (no aplica en Layer 2, pero ejemplo)
public void InstallWithUI(Configuration config)
{
    Console.WriteLine("Installing...");
    var result = Execute(config);
    Console.WriteLine(result ? "Success" : "Failed");
}

// ✓ BIEN - Separar responsabilidades
public async Task<bool> InstallAsync(Configuration config)
{
    return await ExecuteAsync(config);
}

// UI llama a InstallAsync() e imprime por su cuenta
```

---

## CHECKLIST DE CALIDAD

### Antes de cada Commit

- [ ] Clase tiene responsabilidad única (SRP)
- [ ] Métodos < 20 líneas
- [ ] Métodos < 3 parámetros (o usan objeto)
- [ ] Nombres revelan intención
- [ ] Sin variables ambiguas (v, x, tmp)
- [ ] Sin code duplication (DRY)
- [ ] Manejo de errores robusto (throw específico)
- [ ] Documentación XML en métodos públicos
- [ ] Tests escritos y pasando (TDD)
- [ ] Sin null reference risks
- [ ] Sin magic strings (usar constantes)
- [ ] Sin side effects inesperados
- [ ] ConfigureAwait(false) en métodos async
- [ ] No mezcla de concernos (UI ≠ lógica)

### Antes de Pull Request

- [ ] Branch actualizado con main
- [ ] Todos los tests pasan (dotnet test)
- [ ] No hay código muerto o comentado
- [ ] Documentación actualizada
- [ ] CHANGELOG.md actualizado
- [ ] Code coverage ≥ 80%
- [ ] EditorConfig respetado (dotnet format)
- [ ] No warnings en build

---

## EJEMPLOS REALES: ANTES vs DESPUÉS

### Ejemplo 1: Configuration Validator

**ANTES (Sucio)**
```csharp
public class ConfigValidator
{
    private ILogger l;
    private Dictionary<string, string[]> sv;
    
    public ConfigValidator(ILogger logger)
    {
        l = logger;
        sv = new Dictionary<string, string[]>();
        sv.Add("2024", new string[] { "es-ES", "en-US" });
        sv.Add("2021", new string[] { "es-ES", "en-US" });
    }
    
    public bool V(OfficeConfiguration c)
    {
        try
        {
            if (c == null) return false;
            if (c.Version == null) return false;
            if (!sv.ContainsKey(c.Version)) return false;
            if (c.Languages == null) return false;
            foreach (var lang in c.Languages)
            {
                if (!sv[c.Version].Contains(lang))
                {
                    return false;
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            l.Log("Error: " + ex);
            return false;
        }
    }
}
```

**DESPUÉS (Limpio)**
```csharp
/// <summary>
/// Validates Office configuration for version and language support.
/// </summary>
public class ConfigurationValidator
{
    private readonly ILogger _logger;
    private readonly Dictionary<string, IReadOnlySet<string>> _supportedLanguagesByVersion;
    
    public ConfigurationValidator(ILogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        _supportedLanguagesByVersion = new Dictionary<string, IReadOnlySet<string>>
        {
            { "2024", new HashSet<string> { "es-ES", "en-US" } },
            { "2021", new HashSet<string> { "es-ES", "en-US" } }
        };
    }
    
    /// <summary>
    /// Validates the configuration.
    /// </summary>
    /// <exception cref="ArgumentNullException">Thrown if configuration is null</exception>
    /// <returns>true if valid; false otherwise</returns>
    public bool Validate(OfficeConfiguration configuration)
    {
        if (configuration == null)
            throw new ArgumentNullException(nameof(configuration));
        
        try
        {
            return ValidateVersionAndLanguages(configuration);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Validation failed: {ex.Message}");
            return false;
        }
    }
    
    private bool ValidateVersionAndLanguages(OfficeConfiguration configuration)
    {
        if (!IsVersionSupported(configuration.Version))
            return false;
        
        if (configuration.Languages == null || configuration.Languages.Length == 0)
            return false;
        
        return AreLanguagesSupportedForVersion(configuration.Version, configuration.Languages);
    }
    
    private bool IsVersionSupported(string version)
    {
        return _supportedLanguagesByVersion.ContainsKey(version);
    }
    
    private bool AreLanguagesSupportedForVersion(string version, string[] languages)
    {
        var supportedLanguages = _supportedLanguagesByVersion[version];
        return languages.All(lang => supportedLanguages.Contains(lang));
    }
}
```

---

## RESUMEN

**Five Core C# Clean Code Principles:**

1. **CLARITY:** Nombres autoevidentes, métodos pequeños
2. **SRP:** Una clase = una responsabilidad
3. **DRY:** No repetir código
4. **TESTABILITY:** Constructor injection, métodos puros
5. **MAINTAINABILITY:** XML docs, estructura clara

**C# es la capa de lógica núcleo. Cada línea cuenta.**

---

**Versión:** 1.0.0  
**Última actualización:** 2026-04-22  
**Aplicable a:** Layer 2 — OfficeAutomator.Core (.NET 8.0)
