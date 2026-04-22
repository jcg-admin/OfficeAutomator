```yml
created_at: 2026-04-22 05:21:10
project: OfficeAutomator
work_package: 2026-04-22-05-21-10-verify-test-execution-environment
phase: Phase 4 — CONSTRAINTS
author: Claude
status: Borrador
```

# PHASE 4: CONSTRAINTS — Setup Infrastructure Design

## Architectural Decision: Makefile + Scripts + global.json

We reject Docker. We adopt:
- **Makefile** — orchestration (targets: setup, test, clean)
- **scripts/** — shell scripts for idempotent operations
- **global.json** — .NET version pinning (exact version)
- **CONTRIBUTING.md** — developer onboarding

This enables reproducibility WITHOUT containerization.

---

## Technical Constraints

### C-001: Operating System Boundary

| Constraint | Detail | Impact |
|-----------|--------|--------|
| Primary OS | Ubuntu (Linux) | Scripts must be Bash 4+ compatible |
| Secondary OS | macOS | Bash differences (BSD vs GNU) — handled separately |
| Not Supported | Windows Local | Use WSL2 for Windows developers |
| Shell Requirement | Bash 4.0+ | macOS ships with Bash 3.2 (old) |

**Implication:** Scripts MUST work on Ubuntu. macOS support is bonus. Windows = WSL2 only.

---

### C-002: .NET SDK Installation

| Constraint | Detail | Evidence |
|-----------|--------|----------|
| .NET Version | 8.0.xxx (exact) | `global.json` pins exact patch |
| Download Size | ~500 MB | Must verify disk space before download |
| Installation Time | 2-5 minutes | Depends on network, disk speed |
| Permissions | Non-root install OK | Can install to `~/.dotnet/` |
| Network | Requires internet | Check connectivity before attempt |
| Disk Space | 1+ GB free (safe) | SDK itself ~500MB, projects ~100-200MB |

**Idempotency requirement:** Running `make setup` twice must result in same state.

---

### C-003: NuGet Package Resolution

| Constraint | Detail | Impact |
|-----------|--------|--------|
| Package Source | Default NuGet.org | Must be accessible from network |
| Cache Location | ~/.nuget/packages | Persistent across runs (good for idempotency) |
| No Corporate Proxy | Assumed | If behind proxy, CONTRIBUTING.md must document |
| Package Integrity | Checksums verified | NuGet handles automatically |

**Risk:** If NuGet.org is unreachable, restore fails. Mitigation: document fallback.

---

### C-004: Test Execution Environment

| Constraint | Detail |
|-----------|--------|
| RAM Required | 500 MB minimum (xUnit runs in-process) |
| CPU | Single core OK (tests are I/O-bound, not CPU-bound) |
| Test Timeout | 5 minutes max (safety limit in Makefile) |
| Parallelization | xUnit runs tests in parallel by default |
| Log Output | Captured to test results, not stdout streaming |

**Implication:** This Ubuntu system has sufficient resources.

---

### C-005: Makefile Portability

| Constraint | Detail | Solution |
|-----------|--------|----------|
| Tab Indentation | Makefile requires REAL tabs, not spaces | Editor config: `indent_style = tab` in `.editorconfig` |
| POSIX Compliance | Must work on Linux/macOS | Use `$(SHELL)` not hardcoded `/bin/bash` |
| Comments | Lines starting with `#` | Clear documentation |
| .PHONY Targets | Distinguish file vs task targets | All our targets are .PHONY (no files created) |

**Prevention:** `.editorconfig` will enforce tabs.

---

### C-006: Script Idempotency Pattern

```bash
# CORRECT IDEMPOTENT PATTERN:
if [ -d ~/.dotnet ]; then
    echo ".NET already installed"
else
    echo "Installing .NET SDK 8.0..."
    # install
fi

# WRONG (not idempotent):
echo "Installing .NET SDK 8.0..."
# install (always happens, even if installed)
```

**Requirement:** Every step in `scripts/setup.sh` must check state BEFORE action.

---

### C-007: global.json Pinning

```json
{
  "sdk": {
    "version": "8.0.100",
    "rollForward": "patch"
  }
}
```

- Pins to exact version
- `rollForward: patch` allows security patches (8.0.101, etc.) but blocks 8.0.200 or 9.0.0
- **Reproducibility guarantee:** Any developer gets SAME .NET version

---

## Design Decisions

### D-001: No Docker

**Decision:** Makefile + Scripts, NO containerization.

**Rationale:**
- Docker adds complexity (~200MB image, Docker daemon required)
- .NET SDK installs cleanly on Ubuntu without Docker
- Developers should understand their environment
- Easier to debug if something fails

**Trade-off:** CI/CD must also install .NET (or use Docker for CI only).

---

### D-002: Structure

```
OfficeAutomator/
├── Makefile                    ← orchestration (make setup, make test)
├── global.json                 ← .NET 8.0.xxx pinning
├── scripts/
│   ├── setup.sh                ← idempotent installation
│   ├── verify-environment.sh   ← pre-flight checks
│   └── cleanup.sh              ← optional: remove artifacts
├── CONTRIBUTING.md             ← developer guide
└── ...rest of project
```

**NOT in `.infra/` or similar:** Scripts go in root `scripts/` because they're essential for ANY developer.

---

### D-003: Makefile Targets

```makefile
.PHONY: setup
setup:
	@echo "Setting up environment..."
	@bash scripts/verify-environment.sh
	@bash scripts/setup.sh
	@echo "Setup complete"

.PHONY: test
test:
	@echo "Running tests..."
	dotnet test --configuration Release --verbosity minimal

.PHONY: clean
clean:
	@echo "Cleaning artifacts..."
	rm -rf bin/ obj/ .vs/

.PHONY: help
help:
	@echo "Available targets:"
	@echo "  make setup    - Install .NET SDK 8.0 and verify environment"
	@echo "  make test     - Run all tests"
	@echo "  make clean    - Remove build artifacts"
```

**Benefit:** Single entry point for developers: `make setup`, `make test`.

---

### D-004: CONTRIBUTING.md Structure

```markdown
# Contributing to OfficeAutomator

## Quick Start
1. Clone the repository
2. Run: make setup
3. Run: make test

## Prerequisites
- Ubuntu 20.04+ (or macOS 10.14+)
- Bash 4.0+
- Internet connection (for NuGet package download)
- 1 GB free disk space

## Troubleshooting
[common issues and solutions]
```

---

## Validation Checkpoints (Pre-Flight)

### VP-001: Disk Space Check
```bash
available=$(df ~ | awk 'NR==2 {print $4}')
if [ $available -lt 1048576 ]; then  # 1 GB in KB
    echo "ERROR: <1 GB free disk space"
    exit 1
fi
```

### VP-002: Network Connectivity
```bash
if ! curl -s --connect-timeout 5 https://api.nuget.org > /dev/null; then
    echo "ERROR: Cannot reach NuGet.org"
    exit 1
fi
```

### VP-003: Bash Version
```bash
if [ "${BASH_VERSINFO[0]}" -lt 4 ]; then
    echo "ERROR: Bash 4.0+ required"
    exit 1
fi
```

---

## Risks Mapped to Constraints

| Risk | Constraint | Mitigation |
|------|-----------|-----------|
| .NET install fails | C-002 | VP-001 (disk check), VP-002 (network check) before attempt |
| NuGet unreachable | C-003 | VP-002 catches, document fallback URLs in CONTRIBUTING.md |
| Wrong .NET version | C-007 | global.json + VP check for version mismatch |
| Idempotency breaks | C-006 | Code review: scripts MUST check state first |
| Makefile tab errors | C-005 | .editorconfig enforces tabs |
| macOS Bash breaks | C-001 | Scripts use `$(SHELL)`, document macOS setup separately |

---

## Success Criteria (Phase 4 → 5)

✓ Constraints documented and agreed
✓ Architecture decision made: Makefile + scripts + global.json
✓ No Docker — clean Ubuntu install
✓ Idempotency pattern understood
✓ Validation checkpoints defined
✓ Risks mapped to mitigations

**Gate Approval Required:** Proceed to Phase 5 STRATEGY (design plan)?

---

## Deliverables from Phase 4 This WP

- [x] C-001 through C-007 constraints documented
- [x] D-001 through D-004 design decisions explicit
- [x] VP-001 through VP-003 validation checkpoints defined
- [ ] Ready for Phase 5: detailed implementation strategy

