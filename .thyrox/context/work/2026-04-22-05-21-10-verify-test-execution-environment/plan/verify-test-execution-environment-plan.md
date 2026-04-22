```yml
created_at: 2026-04-22 05:21:10
project: OfficeAutomator
work_package: 2026-04-22-05-21-10-verify-test-execution-environment
phase: Phase 6 — PLAN
author: Claude
status: Borrador
```

# PHASE 6: PLAN — Project Structure and Scope

## Scope Statement

**Goal:** Define EXACTLY what files go WHERE in the repo to enable reproducible test execution.

**In Scope:**
- Makefile (root level)
- scripts/ directory with 3 shell scripts
- global.json (already exists, may need update)
- CONTRIBUTING.md (new, documentation)
- .editorconfig (new, enforce tabs in Makefile)

**Out of Scope:**
- Modifying existing OfficeAutomator.Core C# code
- CI/CD pipeline (GitHub Actions)
- Docker setup
- Windows batch scripts (run-tests.bat)

---

## Project Structure — Current vs Post-Implementation

### Current Structure

```
OfficeAutomator/
├── .git/
├── .github/
├── .claude/
├── .thyrox/
├── src/
│   └── OfficeAutomator.Core/
│       ├── *.cs (classes)
│       ├── *Tests.cs (tests)
│       ├── run-tests.sh (exists)
│       ├── run-tests.bat (exists)
│       └── OfficeAutomator.Core.csproj
├── docs/
│   ├── EXECUTION_GUIDE.md (moved here Phase 8)
│   └── TEST_EXECUTION_REPORT.md (moved here Phase 8)
├── global.json (exists)
├── README.md (exists, will update)
└── OfficeAutomator.sln
```

### Post-Implementation Structure (Phase 8+)

```
OfficeAutomator/
├── .git/
├── .github/
├── .claude/
├── .thyrox/
├── src/
│   └── OfficeAutomator.Core/
│       ├── [same as above]
│       └── run-tests.sh (keep, scripts/setup.sh will set env)
│
├── scripts/                           ← NEW DIRECTORY
│   ├── setup.sh                       ← NEW: Idempotent .NET install
│   ├── verify-environment.sh          ← NEW: Pre-flight checks
│   └── cleanup.sh                     ← NEW: Optional artifact cleanup
│
├── docs/
│   ├── EXECUTION_GUIDE.md             ← (moved Phase 8)
│   ├── TEST_EXECUTION_REPORT.md       ← (moved Phase 8)
│   └── CONTRIBUTING.md                ← NEW: Developer guide
│
├── Makefile                           ← NEW: Orchestration
├── .editorconfig                      ← NEW: Enforce tabs
├── global.json                        ← EXISTING (may verify/update)
├── README.md                          ← UPDATED: reference make setup
└── OfficeAutomator.sln
```

---

## Files to Create — Phase 8 PLAN EXECUTION

### P-001: Makefile (Root Level)

**Path:** `OfficeAutomator/Makefile`
**Size:** ~100 lines
**Status:** Created in Phase 8
**Content:**
```makefile
.PHONY: help setup test clean

help:
	@echo "OfficeAutomator - Available targets:"
	@echo "  make setup    - Install .NET SDK 8.0 and verify environment"
	@echo "  make test     - Run all tests"
	@echo "  make clean    - Remove build artifacts"

setup: verify-env install-sdk
	@echo "✓ Setup complete"

verify-env:
	@bash scripts/verify-environment.sh

install-sdk:
	@bash scripts/setup.sh

test:
	@echo "Running tests..."
	@cd src/OfficeAutomator.Core && dotnet test

clean:
	@echo "Cleaning artifacts..."
	@rm -rf src/OfficeAutomator.Core/bin
	@rm -rf src/OfficeAutomator.Core/obj
	@echo "✓ Cleanup complete"
```

**Git:** Commit in Phase 8

---

### P-002: scripts/setup.sh

**Path:** `OfficeAutomator/scripts/setup.sh`
**Size:** ~50 lines
**Executable:** `chmod +x`
**Status:** Created in Phase 8
**Content:** (from Phase 5 S-003)
```bash
#!/bin/bash
set -euo pipefail

echo "Checking .NET 8.0 installation..."

# Idempotent check
if command -v dotnet &> /dev/null; then
    current_version=$(dotnet --version 2>/dev/null || echo "")
    if [[ "$current_version" == 8.0.* ]]; then
        echo "✓ .NET 8.0 already installed: $current_version"
        exit 0
    fi
fi

# Install
echo "Installing .NET SDK 8.0..."
export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=true
export DOTNET_CLI_TELEMETRY_OPTOUT=true

curl -fsSL https://dot.net/v1/dotnet-install.sh | \
    bash -s -- --channel 8.0 --install-dir "$HOME/.dotnet" --skip-user-profile

# Set environment
export DOTNET_ROOT="$HOME/.dotnet"
export PATH="$DOTNET_ROOT:$PATH"

# Verify
dotnet --version
echo "✓ .NET SDK 8.0 installed"
```

**Git:** Commit in Phase 8 (with `chmod +x`)

---

### P-003: scripts/verify-environment.sh

**Path:** `OfficeAutomator/scripts/verify-environment.sh`
**Size:** ~40 lines
**Executable:** `chmod +x`
**Status:** Created in Phase 8
**Content:** (from Phase 5 S-002)
```bash
#!/bin/bash
set -euo pipefail

echo "Verifying environment..."

# VP-001: Disk space
available_kb=$(df ~ | awk 'NR==2 {print $4}')
if [ "$available_kb" -lt 1048576 ]; then
    echo "ERROR: <1 GB free disk space. Available: $((available_kb/1024)) MB"
    exit 1
fi
echo "✓ Disk space OK"

# VP-002: Network
if ! curl -s --connect-timeout 5 https://api.nuget.org/v3/index.json > /dev/null 2>&1; then
    echo "ERROR: Cannot reach api.nuget.org"
    exit 1
fi
echo "✓ Network connectivity OK"

# VP-003: Bash version
if [ "${BASH_VERSINFO[0]}" -lt 4 ]; then
    echo "ERROR: Bash 4.0+ required. Current: ${BASH_VERSINFO[0]}.${BASH_VERSINFO[1]}"
    exit 1
fi
echo "✓ Bash version OK"

echo "✓ All pre-flight checks passed"
```

**Git:** Commit in Phase 8 (with `chmod +x`)

---

### P-004: scripts/cleanup.sh (Optional)

**Path:** `OfficeAutomator/scripts/cleanup.sh`
**Size:** ~20 lines
**Status:** Created in Phase 8 (OPTIONAL)
**Purpose:** Remove NuGet cache, build artifacts (for developers)

**Note:** This is OPTIONAL. Only if needed by developers.

---

### P-005: .editorconfig (New)

**Path:** `OfficeAutomator/.editorconfig`
**Size:** ~30 lines
**Status:** Created in Phase 8
**Content:**
```ini
# EditorConfig is awesome: https://EditorConfig.org

# top-most EditorConfig file
root = true

# Makefile: require REAL tabs (not spaces)
[Makefile]
indent_style = tab
tab_width = 4

# Shell scripts: spaces OK, but 2-space indent preferred
[*.sh]
indent_style = space
indent_size = 2

# Markdown: spaces, 2-space
[*.md]
indent_style = space
indent_size = 2

# JSON: spaces, 2-space
[*.json]
indent_style = space
indent_size = 2
charset = utf-8

# C#: spaces, 4-space (default in project)
[*.cs]
indent_style = space
indent_size = 4
```

**Git:** Commit in Phase 8

---

### P-006: docs/CONTRIBUTING.md (New)

**Path:** `OfficeAutomator/docs/CONTRIBUTING.md`
**Size:** ~150 lines
**Status:** Created in Phase 8
**Content:** (from Phase 5 S-005)

```markdown
# Contributing to OfficeAutomator

## Quick Start

```bash
make setup    # Install .NET SDK 8.0
make test     # Run all tests
```

## Prerequisites

- **OS:** Ubuntu 20.04+ (or macOS 10.14+)
- **Bash:** 4.0 or higher
- **Disk:** 1 GB free space
- **Network:** Internet connection (for NuGet packages)

## What `make setup` Does

1. Verifies disk space (>1 GB)
2. Checks network connectivity
3. Checks Bash version (4.0+)
4. Downloads and installs .NET SDK 8.0 to ~/.dotnet
5. Sets DOTNET_ROOT and PATH environment variables
6. Verifies installation

**Result:** .NET 8.0 ready for development.

## Running Tests

```bash
make test
```

**Expected output:**
```
Total tests: 220+
Passed: 220+
Failed: 0
Duration: ~2-3 minutes
```

## Troubleshooting

### "dotnet: command not found"
```bash
export PATH=$HOME/.dotnet:$PATH
which dotnet
```

### "Wrong .NET version (7.0 instead of 8.0)"
```bash
rm -rf ~/.dotnet
make setup   # Re-install
```

### "Cannot reach nuget.org"
```bash
# Check network
ping api.nuget.org -c 1

# Try restore with cache reset
cd src/OfficeAutomator.Core
dotnet restore --no-cache
```

### "Disk space exhausted"
```bash
df -h ~
# Clean NuGet cache (safe)
rm -rf ~/.nuget/packages
```

## For macOS Users

macOS ships with Bash 3.2 (very old). Install Bash 4.0+:

```bash
brew install bash
chsh -s /usr/local/bin/bash   # Set as default shell
```

Then run `make setup`.

## Development Workflow

1. Clone repo: `git clone ...`
2. Setup: `make setup`
3. Develop: Edit `src/OfficeAutomator.Core/*.cs`
4. Test: `make test`
5. Commit: `git commit -m "..."`

## Additional Targets

```bash
make clean    # Remove build artifacts (bin/, obj/)
make help     # Show available targets
```
```

**Git:** Commit in Phase 8

---

### P-007: global.json (Verify/Update)

**Path:** `OfficeAutomator/global.json`
**Status:** Exists, may need verification in Phase 8
**Current content:**
```json
{
  "sdk": {
    "version": "8.0.100"
  }
}
```

**Phase 8 action:** Verify that `rollForward: patch` is set:
```json
{
  "sdk": {
    "version": "8.0.100",
    "rollForward": "patch"
  }
}
```

**Git:** Update if needed in Phase 8

---

## Files to Update — Phase 8+

### U-001: README.md

**Path:** `OfficeAutomator/README.md`
**Change:** Add quick-start reference to `make setup`
**Location:** Early section, after title
**Content to add:**
```markdown
## Quick Start

```bash
make setup    # Install dependencies
make test     # Run all tests
```

For detailed setup instructions, see [CONTRIBUTING.md](docs/CONTRIBUTING.md).
```

**Git:** Commit in Phase 8

---

## Files NOT to Create

**Out of Scope (explicitly NOT creating):**

- ❌ `Dockerfile` (rejected in Phase 4)
- ❌ `.github/workflows/*.yml` (CI/CD, separate WP)
- ❌ `run-tests.bat` modifications (Windows not supported)
- ❌ `setup.ps1` PowerShell version (Bash only)
- ❌ Docker Compose files

---

## Dependency Graph (Phase 8 Execution Order)

```
1. Create .editorconfig          (standalone, no deps)
   ↓
2. Create scripts/verify-environment.sh    (no deps)
3. Create scripts/setup.sh                 (no deps, but depends on #2 running first)
4. Create scripts/cleanup.sh               (optional, no deps)
   ↓ (all scripts ready)
5. Create Makefile               (depends on scripts/ existing)
   ↓ (Makefile ready)
6. Update global.json            (standalone, but used by Makefile)
7. Update README.md              (references Makefile)
   ↓ (all files ready)
8. Create docs/CONTRIBUTING.md   (documents the setup)
```

**Critical path:** .editorconfig → scripts/ → Makefile → README, CONTRIBUTING

---

## Testing the Implementation (Phase 8+)

### T-001: Makefile targets discoverable

```bash
make help
# Should output: setup, test, clean
```

### T-002: Setup is idempotent

```bash
make setup
# Output: ✓ Setup complete

make setup
# Output: ✓ .NET 8.0 already installed, ✓ Setup complete
```

### T-003: Tests execute

```bash
make test
# Should run 220+ tests and report results
```

### T-004: CONTRIBUTING.md is accurate

```bash
# New developer clones repo
git clone ...
cd OfficeAutomator

# Follows CONTRIBUTING.md steps
make setup
make test

# Outcome: Same result as me
```

---

## Exit Criteria — Phase 6 → Phase 8

✓ Scope defined: 7 files to create, 2 files to update
✓ Structure clear: where each file goes
✓ Dependency graph documented
✓ Testing strategy defined
✓ Out-of-scope explicitly listed
✓ Ready for Phase 8 PLAN EXECUTION (create files)

**Gate decision:** Proceed to Phase 8 PLAN EXECUTION?

---

## Timeline Estimate

| Phase | Task | Estimated Time |
|-------|------|-----------------|
| Phase 6 PLAN | Define scope (this file) | Complete ✓ |
| Phase 8 PLAN EXECUTION | Create 7 files + update 2 | 30-45 minutes |
| Phase 10 IMPLEMENT | Execute + verify | 15-30 minutes |
| Phase 11 TRACK | Document results | 10 minutes |

**Total:** ~1-2 hours from Phase 8 to completion

---

## Notes

- **Git:** All scripts must be committed with `chmod +x` executable bit preserved
- **Idempotency:** Critical constraint — scripts MUST be safe to run multiple times
- **Portability:** Bash scripts must work on Ubuntu AND macOS (documented in CONTRIBUTING.md)
- **Documentation:** CONTRIBUTING.md is the primary developer reference

