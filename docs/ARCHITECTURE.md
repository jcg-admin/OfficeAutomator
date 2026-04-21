# OfficeAutomator Architecture

## Overview

OfficeAutomator is a production-ready C# library for Office automation with enterprise-grade architecture, semantic folder organization, comprehensive testing (220+ unit tests), and explicit error handling with retry policies.

**Key Characteristics:**
- Semantic folder structure reflecting responsibility
- Dependency Injection for testability
- Explicit state machine with 11 states
- 19 error codes with retry classification
- 5 Use Cases (UC-001 to UC-005)

---

## Semantic Folder Structure

### Models/
**Purpose:** Core data classes representing configuration and application state

**Responsibility:** Single - represent data structures

**Namespace:** `OfficeAutomator.Core.Models`

**Classes:**
- `Configuration` - Application configuration with properties, validation, ToString()
  - Properties: 9 settings (version, language, excluded apps, etc.)
  - Methods: ToString(), implicit validation

**Dependencies:** None

**Example Usage:**
```csharp
using OfficeAutomator.Core.Models;

var config = new Configuration 
{ 
    Version = "2024", 
    Language = "es-MX" 
};
```

---

### State/
**Purpose:** Application state machine orchestration and transition management

**Responsibility:** Manage state transitions, validate states, handle terminal/error conditions

**Namespace:** `OfficeAutomator.Core.State`

**Classes:**
- `OfficeAutomatorStateMachine` - Manages application workflow
  - Methods: TransitionTo(), GetCurrentState(), IsTerminalState(), IsErrorState(), IsValidTransition()
  - States: 11 total

**States Machine:**
```
Initial
  ↓
ConfigSelecting → LanguageSelecting → AppSelecting
  ↓
ConfigGenerating → ConfigValidating
  ↓
Installing
  ↓
[Success] OR [Error] → RollingBack → RolledBack
```

**Dependencies:** Error (for error state management)

**Example Usage:**
```csharp
using OfficeAutomator.Core.State;

var sm = new OfficeAutomatorStateMachine();
sm.TransitionTo("ConfigSelecting");

if (sm.IsValidTransition("Error")) 
{
    sm.TransitionTo("Error");
}
```

---

### Error/
**Purpose:** Error handling, classification, and retry logic

**Responsibility:** Classify errors, determine retry policies, calculate backoff

**Namespace:** `OfficeAutomator.Core.Error`

**Classes:**
- `ErrorHandler` - Error classification and retry management
  - Methods: CreateError(), GetRetryPolicy(), ShouldRetry(), GetBackoffMs(), IsRetryableError(), IsCriticalError()
  - Error Codes: 19 total

**Error Classification:**
- **PERMANENT** (not retryable): Invalid configuration, incompatible version
- **TRANSIENT** (retryable): Network timeout, temporary lock
- **CRITICAL** (requires attention): Security failure, system error

**Retry Policy:**
```
Attempt 1: Immediate
Attempt 2: 500ms exponential backoff
Attempt 3: 2000ms exponential backoff
Attempt 4: 8000ms exponential backoff
Max Retries: 4
```

**Dependencies:** None

**Example Usage:**
```csharp
using OfficeAutomator.Core.Error;

var errorHandler = new ErrorHandler();
var error = errorHandler.CreateError("NETWORK_TIMEOUT");

if (errorHandler.ShouldRetry(error)) 
{
    int backoffMs = errorHandler.GetBackoffMs(attemptNumber);
    Thread.Sleep(backoffMs);
    // Retry operation
}
```

---

### Services/
**Purpose:** User-facing services implementing business logic

**Responsibility:** Execute UC (Use Cases), interact with user selections

**Namespace:** `OfficeAutomator.Core.Services`

**Classes:**
- `VersionSelector` - UC-001: Office version selection
  - Methods: GetVersionOptions(), IsValidVersion(), Execute(), GetVersionDescription()
  - Options: Multiple Office versions

- `LanguageSelector` - UC-002: Installation language selection
  - Methods: GetLanguageOptions(), IsValidLanguageSelection(), Execute(), GetLanguageDescription()
  - Supported: 50+ languages

- `AppExclusionSelector` - UC-003: Application exclusion
  - Methods: GetExcludableApps(), IsValidExclusionSet(), Execute(), GetAppDescription(), IsAppExcluded()
  - Supports: Selective app exclusion

**Pattern:** Fluent API design
```csharp
new VersionSelector().Execute("2024");
new LanguageSelector().Execute("es-MX");
new AppExclusionSelector().Execute(new[] { "Access", "Publisher" });
```

**Dependencies:** Models

---

### Validation/
**Purpose:** Configuration validation and XML generation

**Responsibility:** Generate valid XML, validate configuration, ensure integrity

**Namespace:** `OfficeAutomator.Core.Validation`

**Classes:**
- `ConfigGenerator` - XML configuration generation
  - Methods: GenerateConfigXml(), GetConfigFilePath(), ValidateStructure()
  - Output: Valid Office configuration XML

- `ConfigValidator` - 8-step validation process
  - Methods: Execute() - performs all 8 validation steps
  - Steps:
    1. Null check
    2. Version validation
    3. Language validation
    4. App set validation
    5. XML structure validation
    6. File permissions check
    7. Disk space check
    8. Integrity verification

**Dependencies:** Models, Error

**Validation Guarantee:** Configuration is correct before installation proceeds

---

### Installation/
**Purpose:** Installation execution and atomic rollback

**Responsibility:** Execute Office installation, ensure atomic rollback

**Namespace:** `OfficeAutomator.Core.Installation`

**Classes:**
- `InstallationExecutor` - Office installation execution
  - Methods: Execute(), VerifyPrerequisites(), CanDownloadOffice(), GetTimeoutMs(), GetCurrentProgress(), GetSetupExecutablePath()
  - Retry Policy: Integrated with ErrorHandler
  - Timeout: 1800 seconds default

- `RollbackExecutor` - Atomic rollback functionality
  - Methods: Execute(), RemoveOfficeFiles(), CleanRegistry(), RemoveShortcuts(), GetFileRemovalLocations(), GetRegistryKeysToRemove(), GetShortcutsToRemove()
  - Guarantee: All-or-nothing operation

**Atomic Guarantee:**
- All changes reverted if any step fails
- Registry keys cleaned
- Files removed
- Shortcuts deleted
- System returned to pre-installation state

**Dependencies:** Infrastructure (DI), Error

**Example Usage:**
```csharp
using OfficeAutomator.Core.Installation;

var executor = new InstallationExecutor(
    fileSystem: new FileSystemImpl(),
    registry: new RegistryImpl(),
    securityContext: new SecurityContextImpl(),
    processRunner: new ProcessRunnerImpl()
);

bool success = executor.Execute(configuration);

if (!success) 
{
    var rollback = new RollbackExecutor(...);
    rollback.Execute();
}
```

---

### Infrastructure/
**Purpose:** Cross-cutting concerns and dependency injection

**Responsibility:** Define interfaces for external dependencies, enable testability

**Namespace:** `OfficeAutomator.Core.Infrastructure`

**Classes:**
- `Dependencies` - DI interface definitions and implementations

**Interfaces:**
1. `IFileSystem` - File operations
   - `FileSystemImpl` - Production implementation

2. `IRegistry` - Windows Registry operations
   - `RegistryImpl` - Production implementation

3. `ISecurityContext` - Security/privilege operations
   - `SecurityContextImpl` - Production implementation

4. `IProcessRunner` - Process execution
   - `ProcessRunnerImpl` - Production implementation

**Benefits:**
- Testability: Easy mocking in tests
- Extensibility: Swap implementations
- Separation: Business logic independent of infrastructure

---

## Dependency Graph

```
┌─────────────────────────────────────────────────────────────┐
│                    OfficeAutomator.Core                     │
└─────────────────────────────────────────────────────────────┘

┌──────────────┐
│   Models/    │  (Configuration)
└──────┬───────┘
       │
       ├─→ Services/ (VersionSelector, LanguageSelector, AppExclusionSelector)
       │
       ├─→ Validation/ (ConfigGenerator, ConfigValidator)
       │
       └─→ State/ (OfficeAutomatorStateMachine)
               │
               └─→ Error/ (ErrorHandler)
                       │
                       └─→ Infrastructure/ (DI interfaces)
                               │
                               └─→ Installation/ (Executors with DI)
```

---

## Design Principles

### 1. Single Responsibility Principle
Each class has one reason to change:
- `Configuration` - only changes if config structure changes
- `ErrorHandler` - only changes if error policy changes
- `InstallationExecutor` - only changes if installation process changes

### 2. Dependency Injection
All external dependencies injected:
```csharp
public class InstallationExecutor
{
    private readonly IFileSystem _fileSystem;
    private readonly IRegistry _registry;
    // ... constructor injection
}
```

### 3. Explicit Error Handling
19 error codes, classified by retry behavior:
- Permanent errors fail immediately
- Transient errors retry with exponential backoff
- Critical errors trigger rollback

### 4. Atomic Operations
Installation and rollback are all-or-nothing:
- All changes succeed or all revert
- No partial installations
- System state always consistent

### 5. Testability
220+ unit tests covering all public APIs:
- Mocking via Moq
- TDD methodology
- Red-Green-Refactor

---

## Testing Strategy

### Unit Tests (220+)
- **Models/**: Configuration validation, ToString()
- **State/**: State transitions, validity checks
- **Error/**: Error classification, retry logic
- **Services/**: Selection logic, validation
- **Validation/**: XML generation, 8-step validation
- **Installation/**: Execution, rollback

### Test Organization
Tests mirror implementation structure:
```
src/OfficeAutomator.Core/Models/Configuration.cs
tests/OfficeAutomator.Core.Tests/Models/ConfigurationTests.cs
```

### Fixtures & Sample Data
- `Fixtures/` - Reusable test setup
- `SampleData/` - Test resources
- Moq mocks for interfaces

---

## Future Extensions

Current architecture supports:
- Additional error codes (extensible enum pattern)
- New selectors (EmailSelector, RegionSelector, etc.)
- Multiple language support (already designed)
- Regional configurations
- Custom validation rules
- Plugin architecture via DI

---

## Production Readiness Checklist

- ✓ 220+ unit tests (95%+ coverage)
- ✓ Explicit error handling (19 error codes)
- ✓ Retry policies (exponential backoff)
- ✓ Atomic rollback guarantee
- ✓ Semantic folder structure
- ✓ Dependency injection
- ✓ Professional naming
- ✓ Comprehensive documentation
- ✓ Industry-standard patterns

---

**Version:** 1.0.0  
**Last Updated:** 2026-04-21  
**Status:** Production Ready

