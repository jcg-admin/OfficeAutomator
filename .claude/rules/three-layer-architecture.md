# Rule: Three-Layer Architecture

> Cargado automáticamente. Constraint on project structure.

## Principle

OfficeAutomator organizes code into 3 distinct layers by responsibility and execution context.

```
Layer 0 (Root)       Bash    System bootstrap (SDK, validation)
Layer 1 (/scripts)   PowerShell  Automation (UCs)
Layer 2 (src/)       C#      Core logic (DLLs, tests)
```

## Directory Structure Rule

**MUST:**
- ✓ System bootstrap scripts at **root level** (`setup.sh`, `verify-environment.sh`)
- ✓ PowerShell automation in **/scripts/** directory
- ✓ C# core logic in **src/** directory
- ✓ Tests in **Tests/** directory (Layer 2 validation)

**MUST NOT:**
- ❌ Put Bash scripts in `/scripts/` (confuses Layer 0 vs 1)
- ❌ Put PowerShell scripts at root (bootstrap only at root)
- ❌ Put C# logic in PowerShell files (belongs in Layer 2)

## File Placement Checklist

Before creating a new file, ask:

```
Is this a system bootstrap script?
  YES → Root level (setup.sh, verify-environment.sh)
  NO  → Next question

Is this a PowerShell automation function?
  YES → /scripts/ directory
  NO  → Next question

Is this C# business logic?
  YES → src/OfficeAutomator.Core/
  NO  → Next question

Is this a test?
  YES → Tests/ directory
```

## Consequences

Violating this rule causes:
1. User confusion about execution order
2. Mixed concerns (hard to maintain)
3. Dependency tangles (PowerShell → C# → PowerShell)
4. Unclear prerequisites

## Related Documentation

- See: `.thyrox/guidelines/architecture-three-layer.instructions.md` (full guide)
- See: `adr-scripts-directory-structure.md` (decision)

---

**Source:** Pattern from WP 2026-04-22-06-37-03-resolve-csharp-compilation-errors (Phase 12)
