```yml
type: TDD + Documentation Phase
work_package: 2026-04-21-11-15-00-project-structure-reorganization
project: OfficeAutomator
phase: 4.3 - 4.5 (Namespace updates + Documentation)
status: PENDING_EXECUTION
requires: Manual execution in PowerShell
```

# Fase TDD + Documentación - Actualización de Namespaces

## 📋 Objetivo

Aplicar TDD (Test-Driven Development) mediante:
1. Actualización de namespaces en código
2. Actualización de using statements en tests
3. Documentación técnica de la nueva estructura

---

## 🔴 RED Phase - Actuales Namespaces (Genéricos)

### Estado Actual (Incorrecto para estructura semántica)

```csharp
// Models/Configuration.cs
namespace OfficeAutomator.Core
{
    public class Configuration { }
}

// Services/VersionSelector.cs
namespace OfficeAutomator.Core
{
    public class VersionSelector { }
}

// State/OfficeAutomatorStateMachine.cs
namespace OfficeAutomator.Core
{
    public class OfficeAutomatorStateMachine { }
}
```

**Problema:** Todos en mismo namespace, no refleja estructura semántica

---

## 🟢 GREEN Phase - Namespaces Semánticos (Correctos)

### Objetivo: Actualizar a

```csharp
// Models/Configuration.cs
namespace OfficeAutomator.Core.Models
{
    public class Configuration { }
}

// Services/VersionSelector.cs
namespace OfficeAutomator.Core.Services
{
    public class VersionSelector { }
}

// State/OfficeAutomatorStateMachine.cs
namespace OfficeAutomator.Core.State
{
    public class OfficeAutomatorStateMachine { }
}
```

**Beneficio:** Namespace refleja ubicación física + responsabilidad

---

## Instrucciones Paso a Paso

### Paso 1: Actualizar Namespaces en Código (src/)

#### Opción A: Script PowerShell Automático (RECOMENDADO)

```powershell
# En tu máquina, en PowerShell 7+

cd /path/to/OfficeAutomator/src/OfficeAutomator.Core

# Copiar y ejecutar el script UPDATE_NAMESPACES.ps1
. /path/to/UPDATE_NAMESPACES.ps1

# Expected output:
# === ACTUALIZACIÓN DE NAMESPACES ===
#
# Procesando: Models/
#   ✓ Configuration.cs → OfficeAutomator.Core.Models
#
# Procesando: State/
#   ✓ OfficeAutomatorStateMachine.cs → OfficeAutomator.Core.State
# ... etc

# === VERIFICACIÓN ===
# Models/:
#   Configuration.cs: namespace OfficeAutomator.Core.Models
# ... etc
#
# ✓ Namespaces actualizados exitosamente
```

#### Opción B: Manual (para entendimiento)

Para cada carpeta semántica:

**Models/Configuration.cs:**
```csharp
// ANTES
namespace OfficeAutomator.Core

// DESPUÉS
namespace OfficeAutomator.Core.Models
```

**State/OfficeAutomatorStateMachine.cs:**
```csharp
// ANTES
namespace OfficeAutomator.Core

// DESPUÉS
namespace OfficeAutomator.Core.State
```

**Error/ErrorHandler.cs:**
```csharp
// ANTES
namespace OfficeAutomator.Core

// DESPUÉS
namespace OfficeAutomator.Core.Error
```

**Services/VersionSelector.cs, LanguageSelector.cs, AppExclusionSelector.cs:**
```csharp
// ANTES
namespace OfficeAutomator.Core

// DESPUÉS
namespace OfficeAutomator.Core.Services
```

**Validation/ConfigGenerator.cs, ConfigValidator.cs:**
```csharp
// ANTES
namespace OfficeAutomator.Core

// DESPUÉS
namespace OfficeAutomator.Core.Validation
```

**Installation/InstallationExecutor.cs, RollbackExecutor.cs:**
```csharp
// ANTES
namespace OfficeAutomator.Core

// DESPUÉS
namespace OfficeAutomator.Core.Installation
```

**Infrastructure/Dependencies.cs:**
```csharp
// ANTES
namespace OfficeAutomator.Core

// DESPUÉS
namespace OfficeAutomator.Core.Infrastructure
```

---

### Paso 2: Actualizar Using Statements en Tests

#### Opción A: Script PowerShell Automático (RECOMENDADO)

```powershell
# En tu máquina, en PowerShell 7+

cd /path/to/OfficeAutomator/tests/OfficeAutomator.Core.Tests

# Copiar y ejecutar el script UPDATE_TEST_USING_STATEMENTS.ps1
. /path/to/UPDATE_TEST_USING_STATEMENTS.ps1

# Expected output:
# === ACTUALIZACIÓN DE USING STATEMENTS EN TESTS ===
#
# Procesando: Models/ConfigurationTests.cs
#   ✓ Using statements actualizados
#
# Procesando: State/OfficeAutomatorStateMachineTests.cs
#   ✓ Using statements actualizados
# ... etc
```

#### Opción B: Manual (para entendimiento)

Para cada test file:

**Models/ConfigurationTests.cs:**
```csharp
// ANTES
using OfficeAutomator.Core;
using Xunit;

// DESPUÉS
using OfficeAutomator.Core.Models;
using Xunit;
```

**State/OfficeAutomatorStateMachineTests.cs:**
```csharp
// ANTES
using OfficeAutomator.Core;
using Xunit;

// DESPUÉS
using OfficeAutomator.Core.State;
using Xunit;
```

**Error/ErrorHandlerTests.cs:**
```csharp
// ANTES
using OfficeAutomator.Core;
using Xunit;

// DESPUÉS
using OfficeAutomator.Core.Error;
using Xunit;
```

**Services/\*Tests.cs:**
```csharp
// ANTES
using OfficeAutomator.Core;
using Xunit;

// DESPUÉS
using OfficeAutomator.Core.Services;
using Xunit;
```

**Validation/\*Tests.cs:**
```csharp
// ANTES
using OfficeAutomator.Core;
using Xunit;

// DESPUÉS
using OfficeAutomator.Core.Validation;
using Xunit;
```

**Installation/\*Tests.cs:**
```csharp
// ANTES
using OfficeAutomator.Core;
using Xunit;

// DESPUÉS
using OfficeAutomator.Core.Installation;
using Xunit;
```

**Integration/OfficeAutomatorE2ETests.cs:**
```csharp
// ANTES
using OfficeAutomator.Core.Tests;
using Xunit;

// DESPUÉS
using OfficeAutomator.Core.Tests.EndToEnd;
using Xunit;
```

---

### Paso 3: Hacer Commit de Cambios

```powershell
# En raíz del proyecto
git add -A
git commit -m "TDD: Update namespaces and using statements for semantic folders"

# Expected:
# [feature/project-structure-reorganization XX] TDD: Update namespaces and using statements
# 22 files changed, 110 insertions(+), 110 deletions(-)
```

---

## 📚 Documentación Técnica a Crear

### 1. ARCHITECTURE.md

```markdown
# OfficeAutomator Architecture

## Overview
OfficeAutomator is a production-ready C# library for Office automation with semantic folder organization, comprehensive testing, and enterprise-grade error handling.

## Semantic Folder Structure

### Models/
**Purpose:** Data classes representing OfficeAutomator configuration and state

**Classes:**
- Configuration: Represents the application configuration with properties, validation, and ToString()

**Namespace:** OfficeAutomator.Core.Models

### State/
**Purpose:** State machine orchestration and transition management

**Classes:**
- OfficeAutomatorStateMachine: Manages application state transitions, validates states, handles terminal and error states

**Namespace:** OfficeAutomator.Core.State

**States:** 11 states defined
- Initial, ConfigSelecting, LanguageSelecting, AppSelecting, ConfigGenerating, ConfigValidating, Installing, Success, RollingBack, RolledBack, Error

### Error/
**Purpose:** Error handling, classification, and retry logic

**Classes:**
- ErrorHandler: Classifies errors, determines retry policies, calculates backoff intervals

**Namespace:** OfficeAutomator.Core.Error

**Error Codes:** 19 error codes
- PERMANENT (not retryable)
- TRANSIENT (retryable)
- CRITICAL (require immediate attention)

### Services/
**Purpose:** User-facing services implementing business logic

**Classes:**
- VersionSelector: UC-001 - Version selection
- LanguageSelector: UC-002 - Language selection
- AppExclusionSelector: UC-003 - App selection

**Namespace:** OfficeAutomator.Core.Services

**Pattern:** Fluent API design

### Validation/
**Purpose:** Configuration validation and XML generation

**Classes:**
- ConfigGenerator: Generates configuration XML from user inputs
- ConfigValidator: 8-step validation process ensuring configuration integrity

**Namespace:** OfficeAutomator.Core.Validation

### Installation/
**Purpose:** Installation execution and atomic rollback

**Classes:**
- InstallationExecutor: Executes Office installation with retry policies
- RollbackExecutor: Atomic rollback of installation changes

**Namespace:** OfficeAutomator.Core.Installation

**Guarantee:** Atomic rollback - all changes reverted or none

### Infrastructure/
**Purpose:** Cross-cutting concerns and dependency injection

**Classes:**
- Dependencies: Contains DI interfaces (IFileSystem, IRegistry, ISecurityContext, IProcessRunner)

**Namespace:** OfficeAutomator.Core.Infrastructure

**Pattern:** Dependency Injection for testability

## Dependency Graph

```
Models/
    Configuration (no dependencies)

State/
    OfficeAutomatorStateMachine (depends on: Error)

Error/
    ErrorHandler (no dependencies)

Services/
    VersionSelector (depends on: Models)
    LanguageSelector (depends on: Models)
    AppExclusionSelector (depends on: Models)

Validation/
    ConfigGenerator (depends on: Models, Error)
    ConfigValidator (depends on: Models, Error)

Installation/
    InstallationExecutor (depends on: Infrastructure, Error)
    RollbackExecutor (depends on: Infrastructure, Error)

Infrastructure/
    Dependencies (no dependencies)
```

## Design Principles

1. **Single Responsibility:** Each class has one reason to change
2. **Dependency Injection:** All external dependencies injected
3. **Error Handling:** Explicit error codes and retry policies
4. **Testability:** 220+ unit tests covering all classes
5. **Atomicity:** All-or-nothing operations for rollback

## Testing Strategy

- **Unit Tests:** 220+ tests covering all public APIs
- **Mocking:** Moq used for external dependencies
- **TDD:** Red-Green-Refactor methodology
- **E2E Tests:** Integration tests validating complete workflows

## Future Extensions

Current structure supports:
- Additional error codes
- New selectors (EmailSelector, etc.)
- Multiple language support
- Regional configurations
```

### 2. NAMESPACING_GUIDE.md

```markdown
# Namespacing Guide - OfficeAutomator.Core

## Namespace Convention

```
OfficeAutomator.Core.<SemanticFolder>
```

## Namespace Mapping

| Folder | Namespace | Purpose |
|--------|-----------|---------|
| Models/ | OfficeAutomator.Core.Models | Data classes |
| State/ | OfficeAutomator.Core.State | State orchestration |
| Error/ | OfficeAutomator.Core.Error | Error handling |
| Services/ | OfficeAutomator.Core.Services | User-facing services |
| Validation/ | OfficeAutomator.Core.Validation | Configuration validation |
| Installation/ | OfficeAutomator.Core.Installation | Installation execution |
| Infrastructure/ | OfficeAutomator.Core.Infrastructure | DI and utilities |

## Using Statements in Tests

```csharp
// Test for Models/
using OfficeAutomator.Core.Models;
using Xunit;

// Test for Services/
using OfficeAutomator.Core.Services;
using Xunit;
using Moq;

// Test for Installation/
using OfficeAutomator.Core.Installation;
using OfficeAutomator.Core.Infrastructure;
using Xunit;
using Moq;
```

## Public vs Internal APIs

### Public (exported in NuGet)
- OfficeAutomator.Core.Models.Configuration
- OfficeAutomator.Core.Services.VersionSelector
- OfficeAutomator.Core.Services.LanguageSelector
- OfficeAutomator.Core.Services.AppExclusionSelector
- OfficeAutomator.Core.Validation.ConfigValidator

### Internal (used within library)
- OfficeAutomator.Core.State.OfficeAutomatorStateMachine
- OfficeAutomator.Core.Error.ErrorHandler
- OfficeAutomator.Core.Installation.*
- OfficeAutomator.Core.Infrastructure.Dependencies
```

### 3. PROJECT_STRUCTURE_REFERENCE.md

```markdown
# Project Structure Reference

## Directory Layout

```
OfficeAutomator/                          ← Repository Root
├── src/
│   └── OfficeAutomator.Core/            ← Core Library Project
│       ├── Models/                       ← Data classes (1 file)
│       ├── State/                        ← State management (1 file)
│       ├── Error/                        ← Error handling (1 file)
│       ├── Services/                     ← Business logic (3 files)
│       ├── Validation/                   ← Validation (2 files)
│       ├── Installation/                 ← Installation (2 files)
│       ├── Infrastructure/               ← DI interfaces (1 file)
│       └── OfficeAutomator.Core.csproj
│
├── tests/
│   └── OfficeAutomator.Core.Tests/      ← Test Project
│       ├── Models/                       ← Model tests
│       ├── State/                        ← State tests
│       ├── Error/                        ← Error tests
│       ├── Services/                     ← Services tests
│       ├── Validation/                   ← Validation tests
│       ├── Installation/                 ← Installation tests
│       ├── Integration/                  ← E2E tests
│       ├── Fixtures/                     ← Shared test setup
│       ├── SampleData/                   ← Test resources
│       └── OfficeAutomator.Core.Tests.csproj
│
├── docs/                                 ← Documentation
│   ├── ARCHITECTURE.md
│   ├── NAMESPACING_GUIDE.md
│   ├── PROJECT_STRUCTURE_REFERENCE.md
│   ├── TESTING_SETUP.md
│   ├── UC_COMPLETION_VERIFICATION.md
│   ├── TDD_COMPLETION_REPORT.md
│   └── TEST_EXECUTION_ANALYSIS.md
│
├── README.md                             ← Main Entry Point
├── OfficeAutomator.sln                   ← Solution with 2 projects
├── global.json                           ← .NET version config
├── Directory.Build.props                 ← Shared build settings
└── .editorconfig                         ← Code style
```

## Files by Semantic Folder

### Models/ (1 file)
- Configuration.cs - Application configuration

### State/ (1 file)
- OfficeAutomatorStateMachine.cs - State transitions

### Error/ (1 file)
- ErrorHandler.cs - Error classification and retry logic

### Services/ (3 files)
- VersionSelector.cs - Office version selection
- LanguageSelector.cs - Installation language selection
- AppExclusionSelector.cs - Application exclusion selection

### Validation/ (2 files)
- ConfigGenerator.cs - XML config generation
- ConfigValidator.cs - Configuration validation

### Installation/ (2 files)
- InstallationExecutor.cs - Office installation execution
- RollbackExecutor.cs - Rollback functionality

### Infrastructure/ (1 file)
- Dependencies.cs - DI interface definitions

## Test Organization

Tests mirror implementation structure:

- Models/ - ConfigurationTests.cs
- State/ - OfficeAutomatorStateMachineTests.cs
- Error/ - ErrorHandlerTests.cs
- Services/ - VersionSelectorTests.cs, LanguageSelectorTests.cs, AppExclusionSelectorTests.cs
- Validation/ - ConfigGeneratorTests.cs, ConfigValidatorTests.cs
- Installation/ - InstallationExecutorTests.cs, RollbackExecutorTests.cs
- Integration/ - OfficeAutomatorE2ETests.cs

## File Counts

- Implementation files: 11
- Test files: 11
- Total C# files: 22
- Documentation files: 7
- Configuration files: 3

## Total LOC

- Implementation: ~5,000 LOC
- Tests: ~2,500 LOC
- Ratio: 1:0.5 (good test coverage)
```

---

## 🔄 Refactor Phase - Validación

Después de actualizar namespaces:

```powershell
# 1. Build para verificar no hay errores de namespace
dotnet build OfficeAutomator.sln -c Debug

# Expected:
# Build succeeded.
# 0 warning(s)

# 2. Run tests para verificar using statements correctos
dotnet test tests/OfficeAutomator.Core.Tests/

# Expected:
# Test Run Successful
# Total tests: 220+
# Passed: 220+
```

---

## ✅ Checklist - TDD + Documentación

### TDD Phase
- [ ] Ejecutar UPDATE_NAMESPACES.ps1 en src/OfficeAutomator.Core/
- [ ] Ejecutar UPDATE_TEST_USING_STATEMENTS.ps1 en tests/OfficeAutomator.Core.Tests/
- [ ] Verificar con `dotnet build`
- [ ] Verificar con `dotnet test`
- [ ] Commit: "TDD: Update namespaces and using statements"

### Documentación Phase
- [ ] Crear ARCHITECTURE.md en docs/
- [ ] Crear NAMESPACING_GUIDE.md en docs/
- [ ] Crear PROJECT_STRUCTURE_REFERENCE.md en docs/
- [ ] Commit: "DOCS: Add technical architecture documentation"

### Verificación Final
- [ ] Todos los namespaces semánticos ✓
- [ ] Todos los tests pasando ✓
- [ ] Documentación completa ✓
- [ ] Ready para merge ✓

---

**Status:** PLAN COMPLETO PARA TDD + DOCS  
**Próximo:** Ejecutar en tu máquina con .NET SDK

