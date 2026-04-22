# Three-Layer Architecture Guidelines

> Cargado automáticamente. Define la estructura de componentes del proyecto.

## Definición

OfficeAutomator usa arquitectura de 3 capas separadas por responsabilidad y lenguaje de programación:

```
Layer 0 (System Bootstrap)    ← Bash at ROOT
├── setup.sh                   (Install .NET SDK)
└── verify-environment.sh      (Pre-flight validation)

Layer 1 (Automation)           ← PowerShell in /scripts/
├── Install-OfficeAutomator.ps1
├── functions/                 (UC helpers)
└── Invoke-OfficeAutomator     (Main orchestrator)

Layer 2 (Core Logic)           ← C# in src/
└── OfficeAutomator.Core.dll   (Loaded via reflection)
```

## Principio Core

**Estructura de directorios comunica intención de ejecución:**

- **Root level files** = "Run this first" (bootstrapping, prerequisites)
- **Subdirectories** = "Run this after Layer 0 succeeds" (features)

## Responsabilidades por Capa

### Layer 0: System Bootstrap (Bash)

**Files:** `setup.sh`, `verify-environment.sh`, `Makefile`

**Responsibility:**
- Install .NET SDK from official sources
- Validate system prerequisites
- Pre-flight checks (disk space, network, bash version)

**User Discovery:** Root-level — users expect setup to be at root

**Success Criteria:**
- SDK installed in standard location
- Environment validated before Layer 1
- Idempotent (run 2x = run 1x)
- Clear error messages if validation fails

### Layer 1: Automation (PowerShell)

**Files:** `/scripts/*.ps1`

**Responsibility:**
- Orchestrate Use Cases (UC-001 through UC-005)
- Load OfficeAutomator.Core.dll via reflection
- Provide user-friendly interface

**User Discovery:** Subdirectory — users navigate here after Layer 0 succeeds

**Success Criteria:**
- All UCs functional
- Proper error handling
- Clear console output
- Logging to file

### Layer 2: Core Logic (C#)

**Files:** `src/OfficeAutomator.Core/` (.dll)

**Responsibility:**
- Business logic (validation, configuration generation, state machines)
- Test suite (220+ tests)
- No PowerShell/Bash dependencies

**User Discovery:** Transparent — loaded automatically by Layer 1

**Success Criteria:**
- All tests passing
- No external dependencies
- Clean code (SOLID principles)
- Full type safety

## Implementation Checklist

When creating a new feature or module:

```
[ ] Layer 0 Bootstrap
    [ ] System prerequisites documented
    [ ] Setup script handles them
    [ ] Validation before proceeding

[ ] Layer 1 Automation
    [ ] PowerShell functions created
    [ ] UC orchestration implemented
    [ ] Error handling added

[ ] Layer 2 Core Logic
    [ ] C# classes implemented
    [ ] Unit tests written (TDD)
    [ ] Integration tests for UC
    [ ] All tests passing
```

## File Structure Reference

```
OfficeAutomator/
├── setup.sh                    ← Layer 0 (executable, bootstraps system)
├── verify-environment.sh       ← Layer 0 (pre-flight checks)
├── Makefile                    ← Layer 0 (targets for bootstrap)
│
├── scripts/                    ← Layer 1 (automation)
│   ├── Install-OfficeAutomator.ps1
│   └── functions/
│
├── src/                        ← Layer 2 (core logic)
│   └── OfficeAutomator.Core/
│       ├── Models/
│       ├── Error/
│       ├── StateMachine/
│       └── ...
│
└── Tests/                      ← Layer 2 (validation)
    ├── UC-*.Tests.cs
    └── Integration.Tests.cs
```

## Documentation Pattern

README.md should document layers explicitly:

```markdown
## Getting Started

### Layer 0: System Setup
$ ./setup.sh              # Install .NET SDK
$ ./verify-environment.sh # Validate prerequisites

### Layer 1: OfficeAutomator Automation
$ pwsh
PS> . scripts/Install-OfficeAutomator.ps1
PS> Invoke-OfficeAutomator

### Layer 2: Core Logic
(Loaded automatically by Layer 1)
```

## Prevention

**Common Mistakes:**
- ❌ Bash scripts in `/scripts/` (confuses Layer 0 vs 1)
- ❌ C# logic in PowerShell files (should be in Layer 2)
- ❌ PowerShell logic at root (should be in `/scripts/`)

**Correct Pattern:**
- ✅ Bash bootstrap at root
- ✅ PowerShell automation in `/scripts/`
- ✅ C# business logic in `src/`
- ✅ Tests in `Tests/` (Layer 2 validation)

---

**Source:** Pattern discovered in WP 2026-04-22-06-37-03-resolve-csharp-compilation-errors (Phase 12 STANDARDIZE)
