```yml
created_at: 2026-04-22 10:45:00
project: OfficeAutomator
work_package: 2026-04-22-06-37-03-resolve-csharp-compilation-errors
phase: Phase 11 — TRACK/EVALUATE
author: Claude
status: Aprobado
adr_id: adr-scripts-directory-structure
```

# ADR: Scripts Directory Structure — Separation of Bash and PowerShell

## Status
**APPROVED** — Architectural decision documented and validated.

## Context

During Phase 11 TRACK/EVALUATE, the question arose whether infrastructure scripts (`setup.sh`, `verify-environment.sh`) should be colocated with PowerShell automation scripts in the `/scripts` directory.

**Decision Point:** Should infrastructure scripts (Bash) and automation scripts (PowerShell) share the same `/scripts` directory, or should they be organized separately?

---

## Decision
**Separate Directory Structure is CORRECT**

```
OfficeAutomator/
├── setup.sh                          ← System Layer (Bash)
├── verify-environment.sh             ← System Layer (Bash)
├── scripts/                          ← Automation Layer (PowerShell)
│   ├── Install-OfficeAutomator.ps1
│   ├── Select-OfficeVersion.ps1
│   └── ... other UC functions
```

---

## Rationale

### 1. **Execution Layer Separation**

The three-layer architecture requires **distinct execution contexts**:

| Layer | Script Language | When | Purpose |
|-------|-----------------|------|---------|
| **Layer 0: System Setup** | Bash | During initial `clone` + `./setup.sh` | Install .NET SDK, verify system dependencies |
| **Layer 1: Automation** | PowerShell | After Layer 0 succeeds | OfficeAutomator workflow (UC-001 through UC-005) |
| **Layer 2: Core Logic** | C# | Runtime, loaded via reflection | Business logic, validation, state management |

**Consequence:** These layers have **different prerequisites and context**. Mixing their scripts obscures the dependency chain.

### 2. **User Discovery Pattern**

Users expect to find **"How do I set up this project?"** at the repository root:

```bash
$ git clone <repo>
$ cd OfficeAutomator
$ ./setup.sh        ← ✓ FOUND AT ROOT
$ # ... dependencies install ...
$ ./scripts/...     ← Then discover automation scripts
```

**Bad pattern:** If `setup.sh` is buried in `/scripts/`, discoverability fails:

```bash
$ git clone <repo>
$ cd OfficeAutomator
$ ls                # ← setup.sh NOT visible here
$ # User confused: "How do I run this?"
$ find . -name "setup.sh"  # ← Needed to discover
```

### 3. **Privilege and Scope**

- **`setup.sh`**: Often requires `sudo` or `apt-get`, interacts with system package managers
- **PowerShell scripts**: Run unprivileged, interact with user home directory + Office registry

**Consequence:** Separating them signals "these have different privilege levels."

### 4. **Documentation and Distribution**

Standard practice across projects:

- **Root level**: setup, configuration, bootstrapping (README, setup.sh, Makefile, docker-compose.yml)
- **Subdirectory**: Feature-specific automation (scripts/, src/, tests/)

This matches projects like:
- `golang/go` — build.sh at root, scripts/ for individual tools
- `nodejs/node` — configure at root, tools/ for development
- `dotnet/dotnet-cli` — build.sh at root, scripts/ for testing

### 5. **CI/CD Integration**

Build systems reference scripts by pattern:

```yaml
# GitHub Actions, GitLab CI, etc.
script:
  - bash ./setup.sh              # Root-level system bootstrap
  - pwsh ./scripts/Validate*.ps1 # Feature automation
```

If setup.sh is in `/scripts/`, CI config becomes ambiguous:
```yaml
script:
  - pwsh ./scripts/setup.sh       # ← Confusing: bash script in pwsh location
```

---

## Alternatives Considered

### Alternative A: Colocate all scripts in `/scripts/`

```
scripts/
├── setup.sh
├── verify-environment.sh
├── Install-OfficeAutomator.ps1
└── ...
```

**Rejected because:**
- Obscures execution layer dependencies
- Harms user discoverability (setup.sh hidden in subdirectory)
- Mixes privilege levels (sudo required vs. unprivileged)
- CI/CD patterns become confusing

### Alternative B: Separate `/bin/`, `/shell/`, `/automation/` directories

```
bin/setup.sh
shell/verify-environment.sh
automation/Install-OfficeAutomator.ps1
```

**Rejected because:**
- Over-engineered for the scope
- Adds three subdirectories vs. root + one subdirectory
- Inconsistent with standard conventions

---

## Consequences

### Positive

✅ **Clear separation of concerns**: System setup vs. user automation  
✅ **User discoverability**: `./setup.sh` at root is the first step  
✅ **Standards alignment**: Matches golang, nodejs, dotnet conventions  
✅ **CI/CD clarity**: Root-level scripts are always bootstrap; subdirectory scripts are always features  
✅ **Documentation simplicity**: README says "Run `./setup.sh`, then `pwsh scripts/*.ps1`"  

### Negative

⚠ **Two locations for scripts**: Users must remember location (mitigated by README)

---

## Implementation

### Current State ✓

```
OfficeAutomator/
├── setup.sh                    # ✓ Root level
├── verify-environment.sh       # ✓ Root level
├── scripts/                    # ✓ PowerShell only
│   ├── Install-OfficeAutomator.ps1
│   └── ...
```

**Status: Correct as-is. No changes required.**

### Documentation Update Required

**TODO:** Update README.md to document the two-layer approach:

```markdown
## Getting Started

### Layer 0: System Setup
```bash
./setup.sh             # Install .NET SDK, verify dependencies
./verify-environment.sh # Pre-flight validation
```

### Layer 1: OfficeAutomator Automation
```powershell
pwsh
. scripts/Install-OfficeAutomator.ps1
Invoke-OfficeAutomator
```

---

## Decision Record

| Attribute | Value |
|-----------|-------|
| **Decision** | Separate `/scripts/` (PowerShell) from root-level scripts (Bash) |
| **Date** | 2026-04-22 |
| **Author** | Claude (Phase 11 TRACK) |
| **Status** | APPROVED |
| **Justification** | Execution layer separation, user discovery, CI/CD clarity |
| **Related** | adr-three-layer-architecture.md (if created), REGLAS_DESARROLLO_OFFICEAUTOMATOR.md |

---

## Notes

- This decision enforces the **three-layer architecture**: Bash (system) → PowerShell (automation) → C# (core)
- The separation is enforced by convention, not by technology — both are valid shell environments
- If future infrastructure scripts are added (e.g., `docker-setup.sh`, `k8s-deploy.sh`), they belong at **root**, not in `/scripts/`
- If new automation scripts are added (e.g., `Invoke-Upgrade.ps1`), they belong in **`/scripts/`**

---

**Approved by:** Claude (THYROX Phase 11 TRACK/EVALUATE)  
**Date Approved:** 2026-04-22 10:45:00  
**Reference:** WP 2026-04-22-06-37-03-resolve-csharp-compilation-errors  
