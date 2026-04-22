```yml
created_at: 2026-04-22 05:21:10
project: OfficeAutomator
work_package: 2026-04-22-05-21-10-verify-test-execution-environment
phase: Phase 5 — STRATEGY
author: Claude
status: Borrador
```

# PHASE 5: STRATEGY — Solution Implementation Plan

## Problem Statement (Recap)

User identified: "There's a difference between 'tests CAN run' and 'tests ACTUALLY run'."

**Gap:** I claimed tests were ready without verification.

**Goal:** Create reproducible, idempotent infrastructure that enables ANY developer to:
1. Clone the repo
2. Run `make setup`
3. Run `make test`
4. Get guaranteed 220+ tests executed

---

## Solution Strategy: Makefile + Scripts + global.json

### Why This Approach

| Decision | Rationale | Alternative Rejected |
|----------|-----------|----------------------|
| **Makefile** | Single entry point for developers | Bash scripts directly (less discoverable) |
| **scripts/** | Modular, testable, reusable | Inline commands in Makefile (less maintainable) |
| **global.json** | Exact .NET version pinning | apt-get (system-wide, requires sudo) |
| **NO Docker** | Developers understand their environment | Docker (adds complexity, hides environment) |

---

## Architecture Overview

```
Developer runs: make setup
    ↓
Makefile target 'setup' → calls: bash scripts/verify-environment.sh
    ↓
verify-environment.sh:
  1. Check disk space (VP-001)
  2. Check network (VP-002)
  3. Check bash version (VP-003)
    ↓ (if all pass)
Makefile target 'setup' → calls: bash scripts/setup.sh
    ↓
setup.sh (idempotent):
  1. Check if .NET 8.0 installed
     - If yes → print "already installed", exit 0
     - If no → download + install
  2. Set DOTNET_ROOT + PATH
  3. Verify installation
    ↓ (on success)
Developer can now run: make test
    ↓
dotnet test (uses global.json for version guarantee)
```

---

## Deliverables — Phase 5 STRATEGY

This phase creates the STRATEGY (plan). Phase 8 PLAN EXECUTION will create actual files.

### S-001: Makefile Strategy

**Location:** `OfficeAutomator/Makefile` (root)

**Targets to implement:**

```makefile
.PHONY: setup
setup:          # Developer's entry point for environment setup
  verify        # Pre-flight checks
  install-sdk   # Install .NET 8.0 to ~/.dotnet
  verify-final  # Confirm installation successful

.PHONY: test
test:           # Run all tests
  build         # dotnet build (optional, test will do this)
  execute-tests # dotnet test

.PHONY: clean
clean:          # Remove artifacts (optional for developers)
  remove-bins   # rm -rf bin/ obj/

.PHONY: help
help:           # Print available targets (discoverable)
  list targets
```

**Design principle:** Each target is ONE logical step. Targets are composable.

---

### S-002: scripts/verify-environment.sh Strategy

**Location:** `OfficeAutomator/scripts/verify-environment.sh`

**Purpose:** Pre-flight checks before installation. Exit with error if ANY check fails.

**Checks to implement:**

```bash
#!/bin/bash
set -euo pipefail

# VP-001: Disk space (>1 GB free required)
available_kb=$(df ~ | awk 'NR==2 {print $4}')
if [ "$available_kb" -lt 1048576 ]; then
    echo "ERROR: <1 GB free disk space. Available: $((available_kb/1024)) MB"
    exit 1
fi

# VP-002: Network connectivity (can reach nuget.org)
if ! curl -s --connect-timeout 5 https://api.nuget.org/v3/index.json > /dev/null 2>&1; then
    echo "ERROR: Cannot reach api.nuget.org. Check internet connection."
    exit 1
fi

# VP-003: Bash version (4.0+ required)
if [ "${BASH_VERSINFO[0]}" -lt 4 ]; then
    echo "ERROR: Bash 4.0+ required. Current: ${BASH_VERSINFO[0]}.${BASH_VERSINFO[1]}"
    exit 1
fi

echo "✓ All pre-flight checks passed"
```

**Design principle:** Fail early. Tell developer WHAT failed, not generic "setup failed".

---

### S-003: scripts/setup.sh Strategy

**Location:** `OfficeAutomator/scripts/setup.sh`

**Purpose:** Idempotent .NET 8.0 installation to ~/.dotnet

**Idempotency pattern:**

```bash
#!/bin/bash
set -euo pipefail

echo "Checking .NET 8.0 installation..."

# IDEMPOTENT: Check if already installed
if command -v dotnet &> /dev/null; then
    current_version=$(dotnet --version 2>/dev/null || echo "")
    if [[ "$current_version" == 8.0.* ]]; then
        echo "✓ .NET 8.0 already installed: $current_version"
        exit 0
    fi
fi

# Not installed or wrong version → install
echo "Installing .NET SDK 8.0 to ~/.dotnet..."

export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=true
export DOTNET_CLI_TELEMETRY_OPTOUT=true

curl -fsSL https://dot.net/v1/dotnet-install.sh | \
    bash -s -- --channel 8.0 --install-dir "$HOME/.dotnet" --skip-user-profile

# Set environment
export DOTNET_ROOT="$HOME/.dotnet"
export PATH="$DOTNET_ROOT:$PATH"

# Verify
echo "Verifying installation..."
dotnet --version
echo "✓ .NET SDK 8.0 installed successfully"
```

**Design principle:** Run twice = same result. Scripts detect current state before acting.

---

### S-004: global.json Strategy

**Location:** `OfficeAutomator/global.json`

**Content:**

```json
{
  "sdk": {
    "version": "8.0.100",
    "rollForward": "patch"
  }
}
```

**Design principle:** Version pinning at project level. ANY developer gets same .NET version.

---

### S-005: CONTRIBUTING.md Strategy

**Location:** `OfficeAutomator/CONTRIBUTING.md`

**Sections to include:**

1. **Quick Start**
   ```
   make setup    # Install .NET 8.0
   make test     # Run tests
   ```

2. **Prerequisites**
   - Ubuntu 20.04+ (or macOS 10.14+)
   - Bash 4.0+
   - Internet connection
   - 1 GB free disk space

3. **What make setup Does**
   - Verifies disk space, network, bash version
   - Installs .NET SDK 8.0 to ~/.dotnet (non-root)
   - Sets DOTNET_ROOT and PATH
   - Verifies installation

4. **Troubleshooting**
   - T-001: "dotnet: command not found"
   - T-002: Wrong .NET version
   - T-003: NuGet restore fails
   - T-004: Disk space exhausted
   - T-005: Permission denied

5. **For macOS Users** (bonus)
   - Bash 3.2 vs 4.0+ difference
   - How to install Bash 4.0+

**Design principle:** Clear, actionable, discoverable.

---

## Replicability Guarantee

### Any Developer Can Execute

```bash
# Step 1: Clone repo
git clone https://github.com/jcg-admin/OfficeAutomator.git
cd OfficeAutomator

# Step 2: Setup environment (automatic)
make setup
# Output:
#   ✓ All pre-flight checks passed
#   Installing .NET SDK 8.0...
#   ✓ .NET SDK 8.0 installed successfully

# Step 3: Run tests (automatic)
make test
# Output:
#   Running tests...
#   Total tests: 220+
#   Passed: 220+
#   Failed: 0
```

**Guarantee:** Same result on any Ubuntu 20.04+ machine.

---

## Phase Flow

```
Phase 4 CONSTRAINTS (COMPLETED)
  ↓ Documented: Makefile vs Docker, .NET specs, idempotency patterns
  
Phase 5 STRATEGY (THIS PHASE)
  ↓ Define: What we'll build, how it works, why each choice
  
Phase 6 PLAN (NEXT)
  ↓ Scope: What goes in repo, what goes in docs
  
Phase 8 PLAN EXECUTION (FUTURE)
  ↓ Create: Actual Makefile, scripts/, global.json, CONTRIBUTING.md
  
Phase 10 IMPLEMENT (FUTURE)
  ↓ Execute: Run scripts, verify tests work, commit
  
Phase 11 TRACK (FUTURE)
  ↓ Evaluate: Did it work? Any issues?
```

---

## Exit Criteria — Phase 5 → Phase 6

✓ Strategy documented (this file)
✓ Makefile targets defined (S-001)
✓ verify-environment.sh design documented (S-002)
✓ setup.sh idempotency pattern documented (S-003)
✓ global.json strategy defined (S-004)
✓ CONTRIBUTING.md structure planned (S-005)
✓ Replicability guarantee stated
✓ All constraints from Phase 4 addressed in strategy

**Gate decision:** Proceed to Phase 6 PLAN?

---

## Risk Mapping (Phase 4 → Phase 5)

| Risk | Addressed By | How |
|------|--------------|-----|
| R-001: .NET install fails | S-003 + VP-001,VP-002,VP-003 | Pre-flight checks catch before attempt |
| R-002: NuGet unreachable | VP-002 | Network check in verify-environment.sh |
| R-003: Compilation errors | Not Phase 5 scope | Phase 8/10 will test |
| R-004: Test suite fails | Not Phase 5 scope | Phase 10/11 will execute and track |
| R-005: Timeout | Not Phase 5 scope | Timeout handled in Makefile target |
| R-006: Script vs direct mismatch | S-001 | Makefile provides consistent interface |

---

## Next Steps (Phase 6)

Phase 6 PLAN will define:
1. What files go WHERE in the repo
2. How scripts/ are organized
3. Where CONTRIBUTING.md lives
4. Version control: what gets committed vs generated

Phase 8 will CREATE actual files based on this strategy.

