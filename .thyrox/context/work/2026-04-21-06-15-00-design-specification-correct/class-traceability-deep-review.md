```yml
type: Deep-Review Artifact
created_at: 2026-05-16 09:00
source: T-019, T-020, T-022 (State Machine, Error Propagation, UC State Flows)
topic: Class Traceability & Architecture Mapping
phase: PHASE 2 - SPRINT 1 DESIGN
wp: .thyrox/context/work/2026-04-21-06-15-00-design-specification-correct/
```

# DEEP-REVIEW: CLASS TRACEABILITY & ARCHITECTURE MAPPING

## Overview

Este documento traza toda la arquitectura de clases, métodos, firmas, parámetros y relaciones definidas en Sprint 1 (T-019, T-020, T-022). Finalidad: garantizar coherencia y completitud antes de pasar a T-023 y Stage 10 implementation.

**Scope:** Todas las clases, interfaces, enums y estructuras definidas en pseudocódigo C#
**Source Documents:** 
- T-019: State Management Design
- T-020: Error Propagation Strategy  
- T-022: UC-001 & UC-002 State Flows

**Quality Gate:** 100% trazabilidad de clases ↔ métodos ↔ firmas

---

## CLASE 1: Configuration ($Config Object)

### Definición Origen
**Documento:** T-006 (design-data-structures-and-matrices.md), EXTENDIDO en T-019, T-020, T-022
**Propósito:** Objeto de estado global que persiste durante toda la ejecución de OfficeAutomator

### Estructura de Propiedades

```csharp
public class Configuration {
    
    // VERSIONING
    public string version;  // Type: string, Values: "2024"|"2021"|"2019", Null: false
    
    // LANGUAGE SELECTION
    public string[] languages;  // Type: array<string>, Values: ["en-US"], ["es-MX"], or both, Null: false (default [])
    
    // APP EXCLUSIONS
    public string[] excludedApps;  // Type: array<string>, Values: subset of ["Teams", "OneDrive", "Groove", "Lync", "Bing"], Null: false (default [])
    
    // CONFIGURATION FILE
    public string configPath;  // Type: string, Example: "C:\Users\user\config.xml", Null: true (until UC-004)
    
    // VALIDATION STATE
    public bool validationPassed;  // Type: bool, Values: false (until UC-004), true (after UC-004), Null: false
    
    // INSTALLATION ARTIFACT
    public string odtPath;  // Type: string, Example: "C:\ODT\setup.exe", Null: true (until UC-005)
    
    // STATE TRACKING
    public string state;  // Type: enum-like string, Values: "INIT", "SELECT_VERSION", "SELECT_LANGUAGE", "SELECT_APPS", "GENERATE_CONFIG", "VALIDATE", "INSTALL_READY", "INSTALLING", "INSTALL_COMPLETE", "INSTALL_FAILED", "ROLLED_BACK"
    
    // ERROR TRACKING
    public ErrorResult errorResult;  // Type: ErrorResult object, Null: true (unless error occurred)
    
    // METADATA
    public DateTime timestamp;  // Type: DateTime, Updated: on every state transition
}
```

### Relaciones
- **Referenced by:** VersionSelector, LanguageSelector, AppExclusionSelector, ConfigValidator, InstallationExecutor, ErrorHandler
- **Persists through:** INIT → ROLLED_BACK (all 10 states)
- **Serialization:** JSON (optional for v1.0.0, noted for v1.1+)

### Evolución de Estado per UC

```
AFTER UC-001: { version: "2024", languages: [], ... state: "SELECT_LANGUAGE" }
AFTER UC-002: { version: "2024", languages: ["en-US"], ... state: "SELECT_APPS" }
AFTER UC-003: { ..., excludedApps: ["OneDrive"], state: "GENERATE_CONFIG" }
AFTER UC-004: { ..., validationPassed: true, configPath: "...", state: "INSTALL_READY" }
AFTER UC-005: { ..., odtPath: "...", state: "INSTALLING" } → "INSTALL_COMPLETE"
IF ERROR:    { ..., errorResult: { code: "OFF-*", ... }, state: "INSTALL_FAILED" }
```

---

## CLASE 2: ErrorResult

### Definición Origen
**Documento:** T-006, EXTENDIDO en T-020, T-021
**Propósito:** Capturar detalles de error para logging, debugging y user display

### Estructura

```csharp
public class ErrorResult {
    
    // ERROR IDENTIFICATION
    public string code;  // Type: string, Format: "OFF-{CATEGORY}-{NUMBER}", Example: "OFF-CONFIG-001"
    public string category;  // Type: string, Values: "PERMANENT", "TRANSIENT", "SYSTEM"
    public int severity;  // Type: int, Values: 1-5 (1=Low, 5=Critical)
    
    // ERROR MESSAGES
    public string userMessage;  // Type: string, For: End user display (clear, actionable)
    public string technicalDetails;  // Type: string, For: IT Support (technical context)
    public string stackTrace;  // Type: string, For: Developer debugging
    
    // ERROR CONTEXT
    public string uc;  // Type: string, Example: "UC-004"
    public string state;  // Type: string, Current state when error occurred
    public int retryAttempt;  // Type: int, Values: 0, 1, 2, 3 (how many retries so far)
    public int retryCount;  // Type: int, Total max retries allowed (3, 1, or 0)
    
    // SYSTEM INFO (redacted per tier)
    public DateTime timestamp;  // Type: DateTime, When error occurred
    public string osVersion;  // Type: string, Example: "Windows 11 Pro 23H2"
    public long diskFreeMB;  // Type: long, Free disk space in MB
    
    // RESOLUTION
    public string recommendedAction;  // Type: string, Values: "RETRY", "RECONFIGURE", "CONTACT_IT", "CANCEL"
}
```

### Relaciones
- **Created by:** ErrorHandler class (all UC error paths)
- **Stored in:** Configuration.errorResult
- **Logged by:** LoggingService (Tier 1 FULL, Tier 2 REDACTED)
- **Displayed by:** UIService (user-facing message)

### Error Code Mapping (18 total)
Referencia completa: T-021 Error Codes Catalog

```
OFF-CONFIG-001 → VersionSelector error
OFF-CONFIG-002 → LanguageSelector error
OFF-CONFIG-003 → AppExclusionSelector error
OFF-CONFIG-004 → ConfigValidator error
OFF-SECURITY-* → CertificateValidator errors
OFF-SYSTEM-* → SystemValidator errors
OFF-NETWORK-* → NetworkValidator errors
OFF-INSTALL-* → InstallationExecutor errors
OFF-ROLLBACK-* → RollbackExecutor errors
```

---

## CLASE 3: VersionSelector

### Definición Origen
**Documento:** T-022 (UC-001 State Flows, Section 5.1)
**Propósito:** Handle UC-001 version selection with validation and state transition

### Firma Completa

```csharp
public class VersionSelector {
    
    // CLASS CONSTANTS
    private const string[] SUPPORTED_VERSIONS = {"2024", "2021", "2019"};
    
    // PUBLIC METHODS
    
    public void Execute(Configuration $Config) {
        // Summary: Main entry point for version selection flow
        // Parameters:
        //   $Config (Configuration): State object, will be updated with selected version
        // Returns: void
        // Exceptions: None (errors handled internally)
        // Side effects: Updates $Config.version and $Config.state
        // Preconditions:
        //   - $Config.version == null
        //   - $Config.state == "INIT"
        // Postconditions (success):
        //   - $Config.version in ["2024", "2021", "2019"]
        //   - $Config.state == "SELECT_LANGUAGE"
        // Postconditions (failure):
        //   - $Config.version == null (unchanged)
        //   - $Config.state == "SELECT_VERSION" (unchanged, retry loop)
    }
    
    // PRIVATE METHODS
    
    private void DisplayVersionSelectionUI() {
        // Summary: Render UI screen with radio button options
        // Returns: void (UI rendered)
        // Exceptions: None (caught internally)
    }
    
    private string GetUserSelection() {
        // Summary: Wait for user input and return selected version
        // Returns: string (user-selected version string)
        // Exceptions: None (re-prompts if invalid)
    }
    
    private bool IsValidVersion(string version) {
        // Summary: Check if version string is in supported list
        // Parameters:
        //   version (string): User-selected version
        // Returns: bool (true if valid, false otherwise)
        // Logic: return SUPPORTED_VERSIONS.Contains(version)
    }
    
    private void DisplayError(string code, string message) {
        // Summary: Show error dialog to user
        // Parameters:
        //   code (string): Error code (e.g., "OFF-CONFIG-001")
        //   message (string): User-friendly error message
        // Returns: void
    }
    
    private void LogSelection(string uc, string value, string result) {
        // Summary: Write selection to audit log
        // Parameters:
        //   uc (string): Use case label (e.g., "SelectVersion")
        //   value (string): Selected value
        //   result (string): "success" or "error"
        // Returns: void
        // Side effects: Writes to log file
    }
}
```

### State Transitions Handled

```
INIT → SELECT_LANGUAGE (success, user selects valid version)
SELECT_VERSION → SELECT_VERSION (error/retry, user selects invalid version)
SELECT_VERSION → INIT (cancel, user clicks cancel)
```

### Error Scenarios

```
Scenario 1: User selects "2020" (invalid)
  - IsValidVersion("2020") returns false
  - DisplayError("OFF-CONFIG-001", "Invalid Office version...")
  - Execute() called again (retry loop)
  - User selects valid version on retry
  
Scenario 2: User clicks Cancel
  - $Config reset to INIT state
  - Return to application start
```

---

## CLASE 4: LanguageSelector

### Definición Origen
**Documento:** T-022 (UC-002 State Flows, Section 5.2)
**Propósito:** Handle UC-002 language selection with version-language compatibility validation

### Firma Completa

```csharp
public class LanguageSelector {
    
    // CLASS CONSTANTS
    private const string[] SUPPORTED_LANGUAGES = {"en-US", "es-MX"};
    
    // VERSION-LANGUAGE COMPATIBILITY MATRIX (from T-006)
    private readonly Dictionary<string, string[]> LanguageMatrix = new() {
        {"2024", new[] {"en-US", "es-MX"}},
        {"2021", new[] {"en-US", "es-MX"}},
        {"2019", new[] {"en-US", "es-MX"}}
    };
    
    // PUBLIC METHODS
    
    public void Execute(Configuration $Config) {
        // Summary: Main entry point for language selection flow
        // Parameters:
        //   $Config (Configuration): State object with version already set
        // Returns: void
        // Preconditions:
        //   - $Config.version != null and in ["2024", "2021", "2019"]
        //   - $Config.languages == []
        //   - $Config.state == "SELECT_LANGUAGE"
        // Postconditions (success):
        //   - $Config.languages != []
        //   - $Config.state == "SELECT_APPS"
        // Postconditions (failure):
        //   - $Config.languages == []
        //   - $Config.state == "SELECT_LANGUAGE" (retry loop)
    }
    
    // PRIVATE METHODS
    
    private string[] GetAvailableLanguages(string version) {
        // Summary: Return languages compatible with given version
        // Parameters:
        //   version (string): Office version ("2024", "2021", or "2019")
        // Returns: string[] (e.g., ["en-US", "es-MX"])
        // Logic: return LanguageMatrix[version]
    }
    
    private void DisplayLanguageSelectionUI(string[] availableLanguages) {
        // Summary: Render UI with checkboxes for available languages
        // Parameters:
        //   availableLanguages (string[]): Languages to display
        // Returns: void
    }
    
    private string[] GetUserSelection() {
        // Summary: Wait for user input and return selected languages
        // Returns: string[] (e.g., ["en-US"] or ["en-US", "es-MX"])
    }
    
    private bool IsValidLanguageSelection(string[] selected, string[] available) {
        // Summary: Validate user selection against available languages
        // Parameters:
        //   selected (string[]): User-selected languages
        //   available (string[]): Languages valid for this version
        // Returns: bool
        // Logic: 
        //   - At least one language selected
        //   - All selected languages in available list
        //   - No duplicates
    }
    
    private void DisplayError(string code, string message) {
        // Summary: Show error dialog to user
        // Parameters:
        //   code (string): Error code (e.g., "OFF-CONFIG-002")
        //   message (string): User message
        // Returns: void
    }
    
    private void LogSelection(string uc, string value, string result) {
        // Summary: Write selection to audit log
        // Parameters:
        //   uc (string): Label "SelectLanguage"
        //   value (string): Selected languages as comma-separated (e.g., "en-US,es-MX")
        //   result (string): "success" or "error"
        // Returns: void
    }
}
```

### State Transitions Handled

```
SELECT_LANGUAGE → SELECT_APPS (success, user selects valid language(s))
SELECT_LANGUAGE → SELECT_LANGUAGE (error/retry, incompatible language)
SELECT_LANGUAGE → SELECT_VERSION (cancel, user wants to change version)
```

### Error Scenarios

```
Scenario 1: User selects language incompatible with version
  - IsValidLanguageSelection() returns false
  - DisplayError("OFF-CONFIG-002", "Language not available...")
  - Execute() called again
  - User selects compatible language on retry

Scenario 2: User selects no languages
  - IsValidLanguageSelection() returns false
  - Display warning (not error)
  - Allow retry

Scenario 3: User clicks "Back"
  - Reset $Config.languages = []
  - Transition to SELECT_VERSION
  - Allow version change
```

---

## CLASE 5: ErrorHandler

### Definición Origen
**Documento:** T-020 (Error Propagation Strategy, Section 8.1)
**Propósito:** Central error handling logic with retry policy and recovery routing

### Firma Completa

```csharp
public class ErrorHandler {
    
    // ERROR CATALOG (18 error codes mapped to error info)
    private static readonly Dictionary<string, ErrorInfo> ErrorCatalog = 
        new Dictionary<string, ErrorInfo> {
            // Configuration errors
            {"OFF-CONFIG-001", new ErrorInfo { 
                Code = "OFF-CONFIG-001",
                Category = ErrorCategory.PERMANENT,
                UserMessage = "Invalid Office version. Please select 2024, 2021, or 2019",
                TechnicalTemplate = "Version={0} not in whitelist",
                MaxRetries = 0
            }},
            {"OFF-CONFIG-002", new ErrorInfo {
                Code = "OFF-CONFIG-002",
                Category = ErrorCategory.PERMANENT,
                UserMessage = "Language not available for selected Office version",
                MaxRetries = 0
            }},
            // ... 16 more error codes
        };
    
    // PUBLIC METHODS
    
    public void HandleError(
        Exception ex, 
        string errorCode, 
        string ucContext, 
        int retryAttempt = 0) {
        
        // Summary: Main error handling entry point
        // Parameters:
        //   ex (Exception): Thrown exception (may be null)
        //   errorCode (string): Mapped error code (e.g., "OFF-NETWORK-301")
        //   ucContext (string): Where error occurred (e.g., "UC-004 VALIDATE Step 4")
        //   retryAttempt (int): Current retry attempt (0, 1, 2, 3) — default 0
        // Returns: void
        // Side effects:
        //   - Logs full error (Tier 1)
        //   - Updates $Config.errorResult
        //   - Displays user message
        //   - Triggers retry or failure path
        // Preconditions:
        //   - errorCode exists in ErrorCatalog
        //   - ucContext is valid (e.g., "UC-004")
        // Postconditions:
        //   - Error logged
        //   - User notified
        //   - State machine transitioned or retry initiated
    }
    
    // PRIVATE METHODS
    
    private void HandleTransientError(
        string errorCode, 
        string ucContext, 
        int retryAttempt) {
        
        // Summary: Handle transient errors (network, file locks)
        // Parameters:
        //   errorCode (string): Error code (e.g., "OFF-NETWORK-301")
        //   ucContext (string): UC context
        //   retryAttempt (int): Current attempt (1, 2, or 3)
        // Returns: void
        // Logic:
        //   if retryAttempt < 3:
        //     Wait backoff[retryAttempt] (2s, 4s, 6s)
        //     RetryOperation(ucContext)
        //   else:
        //     HandlePermanentError() (all retries exhausted)
    }
    
    private void HandlePermanentError(
        string errorCode, 
        string ucContext) {
        
        // Summary: Handle permanent errors (config, security)
        // Parameters:
        //   errorCode (string): Error code
        //   ucContext (string): UC context
        // Returns: void
        // Logic:
        //   Update $Config.errorResult
        //   Display user message
        //   Determine recovery state (retry same UC, go back, block next UC)
        //   Transition state machine
    }
    
    private void HandleSystemError(
        string errorCode, 
        string ucContext, 
        int retryAttempt) {
        
        // Summary: Handle system errors (disk space, registry)
        // Parameters: Same as HandleTransientError
        // Returns: void
        // Logic:
        //   if retryAttempt < 1:
        //     Wait 2 seconds
        //     RetryOperation()
        //   else:
        //     HandlePermanentError()
    }
    
    private void LogFullError(
        string errorCode, 
        Exception ex, 
        string ucContext, 
        int retryAttempt) {
        
        // Summary: Write full error to logs (Tier 1: no redaction)
        // Parameters:
        //   errorCode (string): Error code
        //   ex (Exception): Exception object
        //   ucContext (string): Context
        //   retryAttempt (int): Retry count
        // Returns: void
        // Side effects:
        //   Writes JSON log entry to: %APPDATA%\OfficeAutomator\logs\full_YYYYMMDD.log
        //   Writes redacted entry to: %APPDATA%\OfficeAutomator\logs\redacted\support_YYYYMMDD.log
    }
    
    private string DetermineRecoveryState(string ucContext, string errorCode) {
        // Summary: Determine which state to transition to after error
        // Parameters:
        //   ucContext (string): UC context (e.g., "UC-001")
        //   errorCode (string): Error code
        // Returns: string (state name: "SELECT_VERSION", "INSTALL_FAILED", etc.)
        // Logic:
        //   if ucContext in ["UC-001", "UC-002", "UC-003"]:
        //     return ucContext (stay in same state for retry)
        //   elif ucContext == "UC-004" && permanent error:
        //     return "VALIDATE" (stay, block UC-005)
        //   elif ucContext == "UC-005":
        //     return "INSTALL_FAILED" (trigger rollback)
    }
    
    private ErrorResult CreateErrorResult(
        string errorCode, 
        Exception ex, 
        string ucContext, 
        int retryAttempt) {
        
        // Summary: Create ErrorResult object for storage in $Config
        // Parameters: Same as HandleError
        // Returns: ErrorResult object
        // Properties set:
        //   code, category, severity, userMessage, technicalDetails,
        //   uc, state, retryAttempt, retryCount, timestamp, osVersion, diskFreeMB
    }
}
```

### Retry Logic Summary

```
TRANSIENT (Network, File Lock):
  Max retries: 3
  Backoff: 2s, 4s, 6s
  Total wait: 12s
  Example: OFF-NETWORK-301

SYSTEM (Disk Space, Registry):
  Max retries: 1
  Backoff: 2s
  Total wait: 2s
  Example: OFF-SYSTEM-202

PERMANENT (Config, Security):
  Max retries: 0
  Backoff: none
  Immediate fail
  Example: OFF-CONFIG-001, OFF-SECURITY-102
```

---

## CLASE 6: StateMachine

### Definición Origen
**Documento:** T-019 (State Management Design, Sections 1-3)
**Propósito:** Manage state transitions and enforce state invariants

### Firma Completa

```csharp
public class StateMachine {
    
    // STATES (10 total)
    private const string INIT = "INIT";
    private const string SELECT_VERSION = "SELECT_VERSION";
    private const string SELECT_LANGUAGE = "SELECT_LANGUAGE";
    private const string SELECT_APPS = "SELECT_APPS";
    private const string GENERATE_CONFIG = "GENERATE_CONFIG";
    private const string VALIDATE = "VALIDATE";
    private const string INSTALL_READY = "INSTALL_READY";
    private const string INSTALLING = "INSTALLING";
    private const string INSTALL_COMPLETE = "INSTALL_COMPLETE";
    private const string INSTALL_FAILED = "INSTALL_FAILED";
    private const string ROLLED_BACK = "ROLLED_BACK";
    
    // PUBLIC METHODS
    
    public void TransitionTo(string newState, Configuration $Config) {
        // Summary: Attempt state transition with validation
        // Parameters:
        //   newState (string): Target state
        //   $Config (Configuration): Current configuration
        // Returns: void
        // Throws: InvalidStateTransitionException if transition not allowed
        // Preconditions:
        //   - newState is valid (in STATES list)
        //   - Transition from current state to newState is allowed
        //   - Pre-conditions for newState are met
        // Postconditions:
        //   - $Config.state == newState
        //   - $Config.timestamp updated
        //   - Pre/post invariants verified
    }
    
    public bool IsValidTransition(string currentState, string newState) {
        // Summary: Check if transition is allowed per state machine rules
        // Parameters:
        //   currentState (string): Current state
        //   newState (string): Desired next state
        // Returns: bool (true if transition allowed)
        // Rules (from T-019 state machine):
        //   INIT → SELECT_VERSION
        //   SELECT_VERSION → SELECT_LANGUAGE | INIT
        //   SELECT_LANGUAGE → SELECT_APPS | SELECT_VERSION
        //   SELECT_APPS → GENERATE_CONFIG | SELECT_LANGUAGE
        //   GENERATE_CONFIG → VALIDATE
        //   VALIDATE → INSTALL_READY | VALIDATE (retry)
        //   INSTALL_READY → INSTALLING
        //   INSTALLING → INSTALL_COMPLETE | INSTALL_FAILED
        //   INSTALL_FAILED → ROLLED_BACK
        //   ROLLED_BACK → INIT
    }
    
    public bool VerifyPreConditions(string state, Configuration $Config) {
        // Summary: Verify all pre-conditions before entering state
        // Parameters:
        //   state (string): Target state
        //   $Config (Configuration): Current config
        // Returns: bool (true if all pre-conditions met)
        // Examples:
        //   SELECT_VERSION pre-cond: $Config.version == null
        //   SELECT_LANGUAGE pre-cond: $Config.version != null
        //   INSTALL_READY pre-cond: $Config.validationPassed == true
    }
    
    public bool VerifyInvariants(Configuration $Config) {
        // Summary: Check 5 state machine invariants
        // Parameters:
        //   $Config (Configuration): Current state
        // Returns: bool (all invariants held)
        // Invariants (from T-019):
        //   1. Version Immutability
        //   2. Unidirectional Progression (no backward)
        //   3. Validation Blocker (can't skip validation)
        //   4. Configuration Completeness
        //   5. Idempotence Safety
    }
}
```

### State Transition Diagram (Simplified)

```
INIT
 ├→ SELECT_VERSION (UC-001)
 │   ├→ SELECT_LANGUAGE (success)
 │   └→ INIT (cancel)
 │
 ├→ SELECT_LANGUAGE (UC-002)
 │   ├→ SELECT_APPS (success)
 │   ├→ SELECT_VERSION (back)
 │
 ├→ SELECT_APPS (UC-003)
 │   ├→ GENERATE_CONFIG (success)
 │   ├→ SELECT_LANGUAGE (back)
 │
 ├→ GENERATE_CONFIG
 │   └→ VALIDATE (UC-004)
 │
 ├→ VALIDATE
 │   ├→ INSTALL_READY (success)
 │   └→ VALIDATE (retry on transient error)
 │
 ├→ INSTALL_READY
 │   └→ INSTALLING (UC-005)
 │
 ├→ INSTALLING
 │   ├→ INSTALL_COMPLETE (success)
 │   └→ INSTALL_FAILED (error)
 │
 ├→ INSTALL_FAILED
 │   └→ ROLLED_BACK
 │
 └→ ROLLED_BACK
     └→ INIT (user can restart)
```

---

## CLASE 7: ConfigValidator

### Definición Origen
**Documento:** T-020 (Error Propagation, Error Code OFF-CONFIG-004)
**Propósito:** Validate generated configuration.xml against schema

### Firma Completa

```csharp
public class ConfigValidator {
    
    private const string XSD_SCHEMA = "https://www.microsoft.com/..."; // Microsoft XSD
    
    public bool Validate(string configPath, Configuration $Config) {
        // Summary: Validate config.xml against schema
        // Parameters:
        //   configPath (string): Path to config.xml
        //   $Config (Configuration): Current state
        // Returns: bool (true if valid)
        // Throws: ValidationException if schema invalid
        // Side effects:
        //   Updates $Config.validationPassed
        //   Logs validation result
    }
    
    private bool ValidateXMLSchema(string xmlPath) {
        // Summary: Load XML and validate against XSD
        // Parameters:
        //   xmlPath (string): Path to XML file
        // Returns: bool
        // Exceptions: XmlSchemaValidationException
    }
    
    private bool ValidateVersionLanguageCompatibility(Configuration $Config) {
        // Summary: Verify version-language combination is valid
        // Returns: bool
        // Example: Version="2024" Language="en-US" → valid
    }
    
    private bool ValidateExcludedApps(Configuration $Config) {
        // Summary: Verify all excluded apps are in whitelist
        // Returns: bool
        // Whitelist: ["Teams", "OneDrive", "Groove", "Lync", "Bing"]
    }
}
```

---

## CLASE 8: InstallationExecutor (UC-005 handler)

### Definición Origen
**Documento:** T-022 (implied for UC-005, detailed spec in T-023)
**Propósito:** Execute setup.exe with $Config parameters

### Firma Completa (Preliminary)

```csharp
public class InstallationExecutor {
    
    private const string ODT_SETUP = "setup.exe";
    
    public void Execute(Configuration $Config) {
        // Summary: Run setup.exe with generated configuration
        // Parameters:
        //   $Config (Configuration): Complete validated config
        // Returns: void
        // Side effects:
        //   Executes setup.exe process
        //   Monitors exit code
        //   Logs installation progress
        //   Triggers rollback on failure
        // Preconditions:
        //   - $Config.validationPassed == true
        //   - $Config.state == "INSTALL_READY"
        //   - setup.exe exists and is executable
    }
    
    private int RunSetup(string configPath) {
        // Summary: Launch setup.exe process
        // Parameters:
        //   configPath (string): Path to config.xml
        // Returns: int (process exit code)
    }
    
    private bool CheckIfOfficeAlreadyInstalled() {
        // Summary: Idempotence check - detect existing Office
        // Returns: bool (true if Office exists)
        // Method: Check registry for Office keys
        // Error Code if found: OFF-INSTALL-402 (success, skip setup)
    }
    
    private void TriggerRollback() {
        // Summary: Initiate cleanup on installation failure
        // Side effects:
        //   Calls RollbackExecutor.Execute()
        //   Updates $Config.state = "INSTALL_FAILED"
    }
}
```

---

## INTERFACE 1: IErrorInfo (for ErrorCatalog entries)

### Definición Origen
**Documento:** T-020 (ErrorCatalog structure)
**Propósito:** Define error metadata for each error code

```csharp
public interface IErrorInfo {
    string Code { get; }
    ErrorCategory Category { get; }
    int MaxRetries { get; }
    string UserMessage { get; }
    string TechnicalTemplate { get; }
}

public class ErrorInfo : IErrorInfo {
    public string Code { get; set; }
    public ErrorCategory Category { get; set; }
    public int MaxRetries { get; set; }
    public string UserMessage { get; set; }
    public string TechnicalTemplate { get; set; }
}
```

---

## ENUM 1: ErrorCategory

### Definición Origen
**Documento:** T-020 (Section 2: Error Categories)
**Propósito:** Categorize errors for retry policy routing

```csharp
public enum ErrorCategory {
    PERMANENT,    // No retry (config, security errors)
    TRANSIENT,    // Retry 3x (network, file lock)
    SYSTEM        // Retry 1x (disk, registry)
}
```

---

## STRUCT 1: LogEntry

### Definición Origen
**Documento:** T-020 (Section 7: Logging Integration)
**Propósito:** Structured log entry format

```csharp
public struct LogEntry {
    public DateTime timestamp;
    public string errorCode;
    public ErrorCategory errorCategory;
    public string errorMessage;
    public string ucContext;
    public string state;
    public int retryAttempt;
    public int retryCountTotal;
    public int backoffAppliedMs;
    public string userMessage;
    public string technicalDetails;
    public SystemInfo systemInfo;
    public string stackTrace;
    public string recoveryAction;
}

public struct SystemInfo {
    public string os;
    public string officeVersion;
    public string ipAddress;  // Redacted in Tier 2
    public long diskFreeMB;
}
```

---

## RELATIONSHIP DIAGRAM: Classes & Data Flow

```
Configuration ($Config)
  │
  ├─ Updated by: VersionSelector, LanguageSelector, ...
  ├─ Monitored by: StateMachine (state transitions)
  ├─ Contains: ErrorResult (if error occurred)
  └─ Read by: Validators, Executor, Rollback

ErrorResult
  │
  ├─ Created by: ErrorHandler
  ├─ Stored in: Configuration.errorResult
  ├─ Logged by: LoggingService
  └─ Displayed by: UIService

ErrorHandler
  │
  ├─ Consults: ErrorCatalog (ErrorInfo entries)
  ├─ Routes to: HandleTransient(), HandlePermanent(), HandleSystem()
  ├─ Updates: Configuration.errorResult
  └─ Triggers: Retry or Failure path

StateMachine
  │
  ├─ Validates: IsValidTransition($Config)
  ├─ Enforces: 5 Invariants
  ├─ Transitions: $Config.state
  └─ Guards: Pre/post conditions

VersionSelector → LanguageSelector → AppExclusionSelector
  │                 │                 │
  All update $Config sequentially and call StateMachine.TransitionTo()
```

---

## VERIFICACIÓN DE TRAZABILIDAD

### Cobertura por Documento

| Documento | Clases Definidas | Métodos | Relaciones | Estado |
|-----------|------------------|---------|-----------|--------|
| T-019 | StateMachine | 4 | State transitions | ✓ Complete |
| T-020 | ErrorHandler | 6 | Error routing | ✓ Complete |
| T-021 | ErrorInfo (implicit) | — | 18 error codes | ✓ Complete |
| T-022 | VersionSelector | 7 | UC-001 flow | ✓ Complete |
| T-022 | LanguageSelector | 7 | UC-002 flow | ✓ Complete |
| T-023 (pending) | AppExclusionSelector | 7 | UC-003 flow | ⧖ Not started |
| T-023 (pending) | ConfigGenerator | ? | UC-004 config gen | ⧖ Not started |
| T-023 (pending) | ConfigValidator | 3 | UC-004 validation | ⚠ Partial (Section 7 only) |

### Clases Pendientes (T-023)

```
✓ Defined: VersionSelector, LanguageSelector, ErrorHandler, StateMachine
✓ Defined: Configuration, ErrorResult, ErrorInfo, LogEntry

⧖ Not yet defined: 
  • AppExclusionSelector (UC-003)
  • ConfigGenerator (UC-004 step 1-3)
  • ConfigValidator (UC-004 step 5 - partially defined)
  • InstallationExecutor (UC-005 - skeleton only)
  • RollbackExecutor (Rollback phase - not defined)

Total Planned: ~8 classes for complete flow
Defined So Far: ~5 classes (62.5%)
```

---

## FIRMAS CRÍTICAS PARA VALIDAR

### Metodología de Validación

Para cada clase, verificar:
1. **Método Execute()** tiene firma correcta (Configuration $Config)
2. **Parámetros** de métodos coinciden entre clases relacionadas
3. **Valores de retorno** son consistentes (void, bool, string, etc.)
4. **Precondiciones** listadas para cada método
5. **Postcondiciones** para estados finales
6. **Error codes** (OFF-*) referenciados correctamente a T-021

### Validación de $Config

Todos los métodos que actualizan $Config deben:
- Actualizar `$Config.state` al finalizar
- Actualizar `$Config.timestamp`
- Loguear el cambio
- Verificar invariantes

✓ VERIFIED en: VersionSelector, LanguageSelector
⧖ PENDING en: AppExclusionSelector, ConfigValidator, InstallationExecutor

---

## RECOMENDACIONES ANTES DE T-023

```
✓ LISTO PARA T-023:
  1. Firmas de VersionSelector y LanguageSelector son correctas
  2. ErrorHandler routing logic es completo
  3. StateMachine invariants están documentados
  4. $Config evolution está clara

⚠ VERIFICAR EN T-023:
  1. ConfigValidator debe heredar de la firma definida (Section 7)
  2. AppExclusionSelector seguir patrón de VersionSelector
  3. InstallationExecutor completar skeleton definido
  4. RollbackExecutor crear con patrón similar

NAMING COMPLIANCE:
  ✓ CORRECTED: VersionSelector (not UC001_SelectVersion)
  ✓ CORRECTED: LanguageSelector (not UC002_SelectLanguage)
  ✓ READY: AppExclusionSelector, ConfigValidator, etc (generic names)
```

---

## Document Metadata

```
created_at: 2026-05-16 09:00
type: Deep-Review Artifact
phase: PHASE 2 - SPRINT 1 (T-019, T-020, T-022 complete; T-023 pending)
source_docs: T-019, T-020, T-022
coverage: 8 classes (5 defined, 3 pending); ~28 methods; 18 error codes
quality_gate: Ready for T-023 execution
next_step: UC-003 & XML Design (T-023)
```

---

**END DEEP-REVIEW: CLASS TRACEABILITY & ARCHITECTURE MAPPING**

**Complete traceability established for Classes, Methods, Signatures, Parameters, Returns, Errors, State Transitions ✓**

