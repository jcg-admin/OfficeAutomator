```yml
type: Implementation Plan
project: OfficeAutomator
stage: PROJECT_STRUCTURE_REORGANIZATION
focus: Transformación de estructura plana a estructura profesional
estimated_duration: 8-10 hours
difficulty: MEDIUM
risk_level: LOW
```

# Plan de Implementación - Reorganización de Estructura

## Fase 0: Preparación (30 minutos)

### 0.1 Crear Rama de Trabajo

```powershell
cd /tmp/projects/OfficeAutomator

# Crear backup
git branch backup-before-restructure

# Crear rama de trabajo
git checkout -b feature/project-structure-reorganization

# Verificar
git branch -v
```

**Expected Output:**
```
backup-before-restructure    dd20439 RESTRUCTURE COMPLETE
* feature/project-structure-reorganization dd20439 RESTRUCTURE COMPLETE
master                       dd20439 RESTRUCTURE COMPLETE
```

### 0.2 Crear Estructura de Carpetas Base

```powershell
# En raíz del proyecto
mkdir -p src
mkdir -p tests
mkdir -p docs

# Verificar
ls -d src tests docs
```

**Expected Output:**
```
docs  src  tests
```

---

## Fase 1: Mover Código Fuente a src/ (1 hora)

### 1.1 Mover stage-10-implementation a src/OfficeAutomator.Core

```powershell
# Opción 1: Git move (preserva historia)
git mv stage-10-implementation src/OfficeAutomator.Core
git commit -m "STRUCTURE: Move implementation to src/OfficeAutomator.Core"

# Opción 2: Bash move (si git falla)
mv stage-10-implementation src/OfficeAutomator.Core
git add -A
git commit -m "STRUCTURE: Move implementation to src/OfficeAutomator.Core"
```

**Verification:**
```powershell
ls -la src/OfficeAutomator.Core/

# Expected: OfficeAutomator.Core.csproj y otros archivos
```

### 1.2 Verificar Archivos de Código

```powershell
ls src/OfficeAutomator.Core/*.cs | wc -l
# Expected: 10 archivos (implementation files)

# Listar:
ls src/OfficeAutomator.Core/*.cs | grep -v Tests
# Configuration.cs
# OfficeAutomatorStateMachine.cs
# ErrorHandler.cs
# VersionSelector.cs
# LanguageSelector.cs
# AppExclusionSelector.cs
# ConfigGenerator.cs
# ConfigValidator.cs
# InstallationExecutor.cs
# RollbackExecutor.cs
# Dependencies.cs
```

### 1.3 Commit

```powershell
git add -A
git commit -m "STRUCTURE: Code files moved to src/OfficeAutomator.Core"
```

---

## Fase 2: Crear Proyectos de Test (1.5 horas)

### 2.1 Crear Estructura de tests/

```powershell
# Crear proyecto principal de tests
mkdir -p tests/OfficeAutomator.Core.Tests

# Crear subcarpetas por semantic folder
mkdir -p tests/OfficeAutomator.Core.Tests/Models
mkdir -p tests/OfficeAutomator.Core.Tests/State
mkdir -p tests/OfficeAutomator.Core.Tests/Error
mkdir -p tests/OfficeAutomator.Core.Tests/Services
mkdir -p tests/OfficeAutomator.Core.Tests/Validation
mkdir -p tests/OfficeAutomator.Core.Tests/Installation
mkdir -p tests/OfficeAutomator.Core.Tests/Fixtures
mkdir -p tests/OfficeAutomator.Core.Tests/SampleData

# Crear carpeta para E2E (opcional)
mkdir -p tests/OfficeAutomator.Core.IntegrationTests
```

### 2.2 Crear OfficeAutomator.Core.Tests.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <IsTestProject>true</IsTestProject>
    <AssemblyName>Apps72.OfficeAutomator.Core.Tests</AssemblyName>
    <RootNamespace>Apps72.OfficeAutomator.Core.Tests</RootNamespace>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="../../src/OfficeAutomator.Core/OfficeAutomator.Core.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="xunit" Version="2.6.0" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.5.0" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
    <PackageReference Include="Moq" Version="4.20.0" />
  </ItemGroup>

</Project>
```

**Guardar como:** `tests/OfficeAutomator.Core.Tests/OfficeAutomator.Core.Tests.csproj`

### 2.3 Mover Archivos de Test

```powershell
# Cd a tests/OfficeAutomator.Core.Tests

# Mover tests por semantic folder
mv /path/ConfigurationTests.cs Models/
mv /path/OfficeAutomatorStateMachineTests.cs State/
mv /path/ErrorHandlerTests.cs Error/
mv /path/VersionSelectorTests.cs Services/
mv /path/LanguageSelectorTests.cs Services/
mv /path/AppExclusionSelectorTests.cs Services/
mv /path/ConfigGeneratorTests.cs Validation/
mv /path/ConfigValidatorTests.cs Validation/
mv /path/InstallationExecutorTests.cs Installation/
mv /path/RollbackExecutorTests.cs Installation/
mv /path/OfficeAutomatorE2ETests.cs Integration/  # Crear Integration/ si no existe

# Crear Integration/ si necesario
mkdir -p Integration
```

### 2.4 Actualizar Namespaces en Tests

Cada archivo test debe tener namespace actualizado:

**Ejemplo:**

```csharp
// Configuration tests
namespace Apps72.OfficeAutomator.Core.Tests.Models
{
    public class ConfigurationTests
    {
        // tests
    }
}
```

### 2.5 Crear OfficeAutomator.Core.IntegrationTests.csproj (Opcional)

```xml
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <IsTestProject>true</IsTestProject>
    <AssemblyName>Apps72.OfficeAutomator.Core.IntegrationTests</AssemblyName>
    <RootNamespace>Apps72.OfficeAutomator.Core.IntegrationTests</RootNamespace>
  </PropertyGroup>

  <ItemGroup>
    <ProjectReference Include="../../src/OfficeAutomator.Core/OfficeAutomator.Core.csproj" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="xunit" Version="2.6.0" />
    <PackageReference Include="xunit.runner.visualstudio" Version="2.5.0" />
    <PackageReference Include="Microsoft.NET.Test.Sdk" Version="17.8.0" />
  </ItemGroup>

</Project>
```

**Guardar como:** `tests/OfficeAutomator.Core.IntegrationTests/OfficeAutomator.Core.IntegrationTests.csproj`

### 2.6 Commit

```powershell
git add -A
git commit -m "STRUCTURE: Create test projects structure"
```

---

## Fase 3: Mover Documentación a docs/ (45 minutos)

### 3.1 Crear Estructura docs/

```powershell
mkdir -p docs
```

### 3.2 Mover Documentación Existente

```powershell
# Mover archivos markdown existentes
mv src/OfficeAutomator.Core/TESTING_SETUP.md docs/
mv src/OfficeAutomator.Core/UC_COMPLETION_VERIFICATION.md docs/
mv src/OfficeAutomator.Core/TDD_COMPLETION_REPORT.md docs/
mv src/OfficeAutomator.Core/TEST_EXECUTION_ANALYSIS.md docs/
```

### 3.3 Crear Nuevos Documentos

**docs/ARCHITECTURE.md**
```markdown
# OfficeAutomator Architecture

## Overview
[Brief description]

## Components
- **Models:** Data structures
- **State:** State machine
- **Error:** Error handling
- **Services:** Selectors
- **Validation:** Configuration validation
- **Installation:** Installation execution
- **Infrastructure:** Dependency injection

## Design Decisions
[Document key decisions]
```

**docs/DESIGN_DECISIONS.md**
```markdown
# Design Decisions

## ADR-001: Semantic Folder Organization
- **Decision:** Use semantic folders (Models, State, Error, etc.)
- **Rationale:** Clear separation of concerns
- **Status:** Accepted

[More ADRs...]
```

**docs/API_REFERENCE.md**
```markdown
# API Reference

## Public Classes

### Configuration
[API documentation]

### OfficeAutomatorStateMachine
[API documentation]

[More classes...]
```

**docs/USER_GUIDE.md**
```markdown
# User Guide

## Installation
[Installation instructions]

## Quick Start
[Getting started]

## Examples
[Code examples]
```

### 3.4 Commit

```powershell
git add -A
git commit -m "STRUCTURE: Move documentation to docs/"
```

---

## Fase 4: Organizar Código en Semantic Folders (1.5 horas)

### 4.1 Crear Carpetas Semánticas en src/OfficeAutomator.Core/

```powershell
cd src/OfficeAutomator.Core

mkdir -p Models
mkdir -p State
mkdir -p Error
mkdir -p Services
mkdir -p Validation
mkdir -p Installation
mkdir -p Infrastructure
```

### 4.2 Mover Archivos de Implementación

```powershell
# Models
mv Configuration.cs Models/

# State
mv OfficeAutomatorStateMachine.cs State/

# Error
mv ErrorHandler.cs Error/

# Services
mv VersionSelector.cs Services/
mv LanguageSelector.cs Services/
mv AppExclusionSelector.cs Services/

# Validation
mv ConfigGenerator.cs Validation/
mv ConfigValidator.cs Validation/

# Installation
mv InstallationExecutor.cs Installation/
mv RollbackExecutor.cs Installation/

# Infrastructure
mv Dependencies.cs Infrastructure/
```

### 4.3 Actualizar Namespaces en Código Implementación

Cada archivo debe tener namespace actualizado:

```csharp
// Models/Configuration.cs
namespace Apps72.OfficeAutomator.Models
{
    public class Configuration
    {
        // implementation
    }
}

// State/OfficeAutomatorStateMachine.cs
namespace Apps72.OfficeAutomator.State
{
    public class OfficeAutomatorStateMachine
    {
        // implementation
    }
}

// Services/VersionSelector.cs
namespace Apps72.OfficeAutomator.Services
{
    public class VersionSelector
    {
        // implementation
    }
}

// [etc. para todos los archivos]
```

### 4.4 Actualizar Using Statements en Tests

Cada test file debe actualizar using statements:

```csharp
// Models/ConfigurationTests.cs
using Apps72.OfficeAutomator.Models;
using Xunit;

public class ConfigurationTests
{
    // tests
}

// State/OfficeAutomatorStateMachineTests.cs
using Apps72.OfficeAutomator.State;
using Xunit;

public class OfficeAutomatorStateMachineTests
{
    // tests
}

// [etc. para todos los tests]
```

### 4.5 Commit

```powershell
git add -A
git commit -m "STRUCTURE: Organize code in semantic folders with updated namespaces"
```

---

## Fase 5: Actualizar Solución (.sln) (30 minutos)

### 5.1 Editar OfficeAutomator.sln

```xml
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio Version 17.0

Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "OfficeAutomator.Core", 
  "src\OfficeAutomator.Core\OfficeAutomator.Core.csproj", "{GUID1}"
EndProject

Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "OfficeAutomator.Core.Tests", 
  "tests\OfficeAutomator.Core.Tests\OfficeAutomator.Core.Tests.csproj", "{GUID2}"
EndProject

Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "OfficeAutomator.Core.IntegrationTests", 
  "tests\OfficeAutomator.Core.IntegrationTests\OfficeAutomator.Core.IntegrationTests.csproj", "{GUID3}"
EndProject

Global
  GlobalSection(SolutionConfigurationPlatforms) = preSolution
    Debug|Any CPU = Debug|Any CPU
    Release|Any CPU = Release|Any CPU
  EndGlobalSection

  GlobalSection(ProjectConfigurationPlatforms) = postSolution
    {GUID1}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
    {GUID1}.Debug|Any CPU.Build.0 = Debug|Any CPU
    {GUID1}.Release|Any CPU.ActiveCfg = Release|Any CPU
    {GUID1}.Release|Any CPU.Build.0 = Release|Any CPU

    {GUID2}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
    {GUID2}.Debug|Any CPU.Build.0 = Debug|Any CPU
    {GUID2}.Release|Any CPU.ActiveCfg = Release|Any CPU
    {GUID2}.Release|Any CPU.Build.0 = Release|Any CPU

    {GUID3}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
    {GUID3}.Debug|Any CPU.Build.0 = Debug|Any CPU
    {GUID3}.Release|Any CPU.ActiveCfg = Release|Any CPU
    {GUID3}.Release|Any CPU.Build.0 = Release|Any CPU
  EndGlobalSection
EndGlobal
```

**Nota:** Los GUIDs deben ser únicos. Puedes generar con:
```powershell
[guid]::NewGuid()
```

### 5.2 Commit

```powershell
git add OfficeAutomator.sln
git commit -m "STRUCTURE: Update solution file with both projects"
```

---

## Fase 6: Verificación y Testing (1 hora)

### 6.1 Limpiar Build Cache

```powershell
dotnet clean OfficeAutomator.sln
```

### 6.2 Build Completo

```powershell
dotnet build OfficeAutomator.sln -c Debug

# Expected output:
# Build succeeded after X.XXs
# - No warnings
# - Both projects compiled
```

**Si hay errores:**
1. Verificar namespaces (deben match folder structure)
2. Verificar using statements (deben referenciar nuevos namespaces)
3. Verificar .csproj references

### 6.3 Run Tests

```powershell
dotnet test tests/OfficeAutomator.Core.Tests/ --no-build

# Expected output:
# Test Run Successful
# Total tests: 220+
# Passed: 220+
# Failed: 0
```

**Si hay test failures:**
1. Revisar namespace updates
2. Revisar using statement updates
3. Revisar paths en test files

### 6.4 Commit

```powershell
git add -A
git commit -m "STRUCTURE: Verify build and tests pass"
```

---

## Fase 7: Final Cleanup (30 minutos)

### 7.1 Verificar No Hay Archivos Huérfanos

```powershell
# En raíz, verificar que NO hay:
# - stage-10-implementation/ (debe estar en src/)
# - .cs files at root (deben estar en src/)
# - Documentation/ (debe estar en docs/)
# - Tests/ (debe estar en tests/)

ls -la | grep -E "stage-10|\.cs$|Documentation|^Tests"
# Expected: ningún resultado
```

### 7.2 Verificar Estructura Final

```powershell
tree -L 3 -I 'bin|obj'

# Expected structure:
# OfficeAutomator/
# ├── src/OfficeAutomator.Core/
# │   ├── Models/
# │   ├── State/
# │   ├── Error/
# │   ├── Services/
# │   ├── Validation/
# │   ├── Installation/
# │   └── Infrastructure/
# ├── tests/OfficeAutomator.Core.Tests/
# │   ├── Models/
# │   ├── State/
# │   ├── Error/
# │   ├── Services/
# │   ├── Validation/
# │   ├── Installation/
# │   ├── Fixtures/
# │   └── SampleData/
# └── docs/
```

### 7.3 Verificar .gitignore

```
# .gitignore debe incluir:
bin/
obj/
.vs/
.vscode/
*.user
*.suo
*.sln.docstates
```

### 7.4 Commit Final

```powershell
git add -A
git commit -m "STRUCTURE: Final cleanup and verification"
```

---

## Fase 8: Merge a Master (15 minutos)

### 8.1 Merge Feature Branch

```powershell
git checkout master
git merge feature/project-structure-reorganization

# Expected: Fast-forward merge
```

### 8.2 Verify on Master

```powershell
# Verificar estructura
ls -d src tests docs

# Verificar build
dotnet build OfficeAutomator.sln

# Verificar tests
dotnet test tests/OfficeAutomator.Core.Tests/
```

### 8.3 Delete Feature Branch

```powershell
git branch -d feature/project-structure-reorganization
```

### 8.4 Delete Backup Branch (After Verification)

```powershell
git branch -d backup-before-restructure
```

---

## Rollback Plan

Si algo sale mal en cualquier momento:

```powershell
# Opción 1: Reset a antes del feature branch
git reset --hard master^

# Opción 2: Revert último commit
git revert HEAD

# Opción 3: Usar backup branch
git checkout backup-before-restructure
git reset --hard
```

---

## Checklist Final

- [ ] Rama de trabajo creada
- [ ] Carpetas src/, tests/, docs/ creadas
- [ ] Código movido a src/OfficeAutomator.Core/
- [ ] Proyectos de test creados
- [ ] Archivos de test movidos
- [ ] Documentación movida a docs/
- [ ] Carpetas semánticas creadas en src/
- [ ] Namespaces actualizados en código
- [ ] Using statements actualizados en tests
- [ ] OfficeAutomator.sln actualizado
- [ ] dotnet build exitoso (sin warnings)
- [ ] dotnet test 220+ tests passed
- [ ] Estructura verificada
- [ ] Feature branch mergeado a master
- [ ] Backup branch deletedo

---

## Timeline Estimado

| Fase | Duración | Total Acumulado |
|------|----------|-----------------|
| 0 - Preparación | 30 min | 30 min |
| 1 - Mover código | 1 hr | 1.5 hrs |
| 2 - Tests | 1.5 hrs | 3 hrs |
| 3 - Docs | 45 min | 3.75 hrs |
| 4 - Semantic folders | 1.5 hrs | 5.25 hrs |
| 5 - .sln | 30 min | 5.75 hrs |
| 6 - Verificación | 1 hr | 6.75 hrs |
| 7 - Cleanup | 30 min | 7.25 hrs |
| 8 - Merge | 15 min | 7.5 hrs |
| **TOTAL** | | **7.5 - 8.5 horas** |

---

**Status:** PLAN COMPLETO ✓  
**Dificultad:** MEDIA  
**Riesgo:** BAJO  
**Rollback:** Posible en cualquier momento  

