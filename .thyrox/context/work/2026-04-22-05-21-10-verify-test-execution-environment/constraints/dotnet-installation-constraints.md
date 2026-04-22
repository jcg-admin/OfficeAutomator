```yml
created_at: 2026-04-22 05:21:10
project: OfficeAutomator
work_package: 2026-04-22-05-21-10-verify-test-execution-environment
phase: Phase 4 — CONSTRAINTS
author: Claude
status: Borrador
```

# PHASE 4: CONSTRAINTS — .NET 8.0 Installation Technical Details

## Scope

This analysis documents TECHNICAL CONSTRAINTS for installing .NET SDK 8.0 on Ubuntu.
Complements `setup-infrastructure-constraints.md` (architectural decisions).

---

## .NET 8.0 SDK Specifications

### DNC-001: Official Release Information

| Property | Value | Evidence |
|----------|-------|----------|
| Latest LTS Release | .NET 8.0 | Microsoft official release (Nov 2023) |
| Current Patch | 8.0.100+ | Updates every month |
| Support Timeline | Until Nov 2026 | LTS = 3 years support |
| Installation Method | dotnet-install script OR apt package | Both idempotent-capable |

**Decision:** Use official `dotnet-install.sh` script (more portable than apt).

---

### DNC-002: System Requirements

| Requirement | OfficeAutomator Need | Ubuntu Support | Status |
|------------|---------------------|-----------------|--------|
| CPU Architecture | x64 (Intel/AMD) | ✓ Standard | OK |
| Linux Kernel | 5.10+ | ✓ Ubuntu 20.04+ | OK |
| glibc Version | 2.31+ | ✓ Ubuntu 20.04 has 2.31 | OK |
| OpenSSL | 1.1 or 3.0 | ✓ Ubuntu 20.04+ has both | OK |
| RAM | 512 MB minimum | ✓ Available | OK |
| Disk Space | 1 GB for SDK + projects | ✓ Verified in VP-001 | OK |

**Implication:** No special tweaking needed. Standard Ubuntu 20.04+ works.

---

### DNC-003: Installation Location Options

```bash
Option 1: System-wide (/usr/share/dotnet)
  ✓ Shared by all users
  ✗ Requires sudo
  ✗ Hard to update without permission

Option 2: User-local (~/.dotnet)
  ✓ No sudo needed
  ✓ Easy to update/remove
  ✓ No conflicts with system .NET
  ✗ Not shared (each user has own copy)

Option 3: Project-local (./dotnet)
  ✓ Complete isolation
  ✗ Wastes disk space
  ✗ Complex scripts
```

**Decision:** Option 2 (~/.dotnet) — non-root, idempotent, developer-friendly.

---

### DNC-004: Installation Methods Compared

| Method | Pros | Cons | Idempotent? |
|--------|------|------|-------------|
| `dotnet-install.sh` | Official, portable, flexible | Needs bash, downloads on each run | If wrapped correctly: YES |
| `apt install dotnet-sdk-8.0` | System package, managed | Requires sudo, system-wide | Partially (skips if installed) |
| `snap install dotnet-sdk` | Easy, isolated | Slower, different paths | Partially |
| Pre-built tarball | Complete control | Manual management, updates hard | NO |

**Decision:** `dotnet-install.sh` wrapped in idempotent shell check:
```bash
if dotnet --version | grep -q "8.0."; then
    echo ".NET 8.0 already installed"
else
    bash dotnet-install.sh --channel 8.0 --install-dir ~/.dotnet
fi
```

---

### DNC-005: global.json Version Pinning

```json
{
  "sdk": {
    "version": "8.0.100",
    "rollForward": "patch"
  }
}
```

**Semantics:**
- `version: 8.0.100` — exact version you want
- `rollForward: patch` — allows 8.0.101, 8.0.102 (security patches) but blocks 8.1.0 or 9.0.0

**Reproducibility guarantee:**
- Developer A installs: gets exactly 8.0.100+ (same patch level as CI)
- Developer B installs: gets exactly 8.0.100+ (same as A)
- Ensures: "works on my machine" == "works on their machine"

**Alternative rollForward modes:**
| Mode | Behavior | Use Case |
|------|----------|----------|
| `patch` | 8.0.x only | OfficeAutomator (stable) |
| `minor` | 8.x.y | Bleeding edge (risky) |
| `feature` | 8.0.0 - 8.y.z | Latest features in current major |
| `latestPatch` | Latest 8.0.z always | Automatic security updates |
| `disable` | EXACT version only | Production hardening |

**Decision:** `patch` — balance between stability and security updates.

---

### DNC-006: Environment Variables

```bash
# DOTNET_ROOT — tells system where .NET lives
export DOTNET_ROOT=$HOME/.dotnet

# PATH — ensures `dotnet` command is found
export PATH=$DOTNET_ROOT:$PATH

# DOTNET_SKIP_FIRST_TIME_EXPERIENCE — no telemetry prompt
export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=true

# DOTNET_CLI_TELEMETRY_OPTOUT — disable telemetry
export DOTNET_CLI_TELEMETRY_OPTOUT=true
```

**Where to set:**
- In `scripts/setup.sh` — export during install
- In `~/.bashrc` or `~/.zshrc` — persist across sessions (optional)
- In `CONTRIBUTING.md` — document for developers

---

### DNC-007: NuGet Package Restoration

```bash
dotnet restore
```

This downloads all NuGet packages declared in `.csproj` files.

| Constraint | Detail |
|-----------|--------|
| **Network Required** | Must access nuget.org (or configured feeds) |
| **Cache Location** | ~/.nuget/packages (persistent, shared across projects) |
| **Disk Space** | Typically 100-200 MB for OfficeAutomator packages |
| **Time** | First restore: 1-2 minutes. Subsequent: seconds (cached) |
| **Idempotency** | Safe to run multiple times (skips up-to-date) |

**Failure scenarios:**
- No internet → fail with "cannot reach nuget.org"
- Network timeout → fail with timeout error
- Package unavailable → fail with "package not found"

**Mitigation:** VP-002 (network check) catches before attempt.

---

### DNC-008: Compilation and Test Execution

```bash
# Compilation
dotnet build

# Test execution
dotnet test
```

| Phase | Time | Resources | Parallelization |
|-------|------|-----------|-----------------|
| Restore | 1-2 min (first) | 50-100 MB | N/A |
| Build | 30-60 sec | 100-200 MB | Sequential |
| Test | 2-5 min | 200-500 MB | Parallel (xUnit default) |

**Test Parallelization:** xUnit runs tests in parallel by default.
- Can be configured in `.runsettings` if needed
- For OfficeAutomator (220+ tests): ~2-3 minutes typical

---

### DNC-009: Verification Post-Installation

```bash
# Check .NET is in PATH
dotnet --version
# Expected output: 8.0.100 (or later patch)

# Check .NET can find projects
dotnet --list-sdks
# Should show: 8.0.100 [~/.dotnet]

# Check NuGet is accessible
dotnet nuget list source
# Should show: nuget.org (https://api.nuget.org/v3/index.json)
```

---

## Idempotency Guarantees

### Pattern for Idempotent Installation

```bash
#!/bin/bash
set -euo pipefail

echo "Checking .NET 8.0 installation..."

# IDEMPOTENT: Check if already installed and correct version
if command -v dotnet &> /dev/null; then
    current_version=$(dotnet --version)
    if [[ $current_version == 8.0.* ]]; then
        echo "✓ .NET 8.0 already installed: $current_version"
        exit 0
    fi
fi

# If not installed, install
echo "Installing .NET SDK 8.0..."
bash <(curl -fsSL https://dot.net/v1/dotnet-install.sh) \
    --channel 8.0 \
    --install-dir ~/.dotnet \
    --skip-user-profile

# Verify installation
export DOTNET_ROOT=$HOME/.dotnet
export PATH=$DOTNET_ROOT:$PATH

dotnet --version
echo "✓ Installation complete"
```

**Idempotency properties:**
- ✓ Run twice = same result
- ✓ Detects already-installed version
- ✓ Skips download if already installed
- ✓ Sets environment variables correctly
- ✓ Exits cleanly on success

---

## Troubleshooting Constraints

### T-001: "dotnet: command not found"

**Cause:** PATH not set or installation failed

**Fix:**
```bash
export PATH=$HOME/.dotnet:$PATH
which dotnet
```

**Prevention:** VP-003 checks PATH after installation.

---

### T-002: Wrong .NET Version (e.g., 7.0.x instead of 8.0.x)

**Cause:** System .NET installation conflict, or PATH priority

**Fix:**
```bash
rm -rf ~/.dotnet  # Remove local installation
# Re-run setup.sh
```

**Prevention:** global.json enforces 8.0 at project level.

---

### T-003: "Unable to reach nuget.org" (NuGet Restore Fails)

**Cause:** No internet, network timeout, or proxy issue

**Fix:**
```bash
# Check network
ping api.nuget.org -c 1

# Try with explicit timeout
dotnet restore --no-cache

# Check NuGet config
cat ~/.nuget/NuGet/NuGet.Config
```

**Prevention:** VP-002 checks network before attempting restore.

---

### T-004: "Disk space exhausted"

**Cause:** Not enough disk space for SDK + packages + projects

**Fix:**
```bash
# Clean NuGet cache (safe, will re-download as needed)
rm -rf ~/.nuget/packages

# Check disk
df -h ~
```

**Prevention:** VP-001 checks 1+ GB available.

---

### T-005: Permission Denied (when installing system-wide)

**Cause:** Attempted `dotnet-install.sh` to /usr/share/dotnet without sudo

**Fix:** Use `~/.dotnet` (non-root) instead

**Prevention:** Our setup.sh uses ~/.dotnet (no sudo needed).

---

## Success Criteria (Phase 4 → 5)

✓ .NET 8.0 installation method decided: `dotnet-install.sh` to ~/.dotnet
✓ Idempotency pattern documented
✓ global.json version pinning strategy defined
✓ Environment variables documented
✓ Troubleshooting guide provided
✓ NuGet restoration constraints understood
✓ Verification post-installation steps defined

---

## Deliverables for Phase 5 STRATEGY

These constraints feed into Phase 5 plan:
1. **Makefile target:** `make setup` → runs scripts/setup.sh
2. **scripts/setup.sh:** Implements idempotent pattern from DNC-009
3. **CONTRIBUTING.md:** Documents environment setup from DNC-006
4. **Troubleshooting section:** References T-001 through T-005

