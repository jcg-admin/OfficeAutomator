# C# Build Practices — Compilation Cache Prevention

> Cargado automáticamente. Critical for all .NET projects using dotnet CLI.

## Core Rule: Always Clean Before Testing

Stale .dll files in build artifacts can cause test failures that don't reflect actual code defects.

```
OLD SOURCE → Compile → .dll in bin/
UPDATE SOURCE → Compile with --no-clean → Reuses OLD .dll
→ Tests fail with incorrect errors
```

**Solution:**
```bash
dotnet clean     # Delete bin/, obj/
dotnet build     # Recompile from scratch
dotnet test      # Run tests with fresh IL
```

## The Problem (Evidence)

**Scenario:** E2E test `E2E_013_State_Machine_Error_Recovery_Path` fails

```csharp
Assert.True(sm.TransitionTo("INSTALL_READY"));  // Assertion fails
```

**Investigation:**
- Source code: Transition CORRECTLY defined in state machine
- Build: Code correct, IL in .dll outdated
- Fix: `dotnet clean && dotnet build` → test passes

**Root Cause:**
Intermediate Language (IL) compiled into .dll from previous build doesn't match current source code.

**Symptoms:**
- Test fails but source code is correct
- Failure is reproducible
- Full rebuild fixes it
- Different test runner (IDE vs CLI) shows different results

## Build Workflow

### Local Development

**Makefile targets:**
```makefile
.PHONY: test clean build

clean:
	@echo "Cleaning build artifacts..."
	@dotnet clean
	@rm -rf src/*/bin src/*/obj

build: clean
	@echo "Building..."
	@dotnet build

test: clean
	@echo "Running tests..."
	@dotnet test
```

**Developer command:**
```bash
make test    # Safe: always cleans first
```

### CI/CD Pipeline

```yaml
test-job:
  steps:
    - name: Clean artifacts
      run: dotnet clean
    
    - name: Build solution
      run: dotnet build
    
    - name: Run tests
      run: dotnet test --logger trx --collect:"XPlat Code Coverage"
    
    - name: Report results
      run: |
        echo "Test Results:"
        dotnet test --no-build --no-restore
```

**Key Point:** `dotnet clean` is ALWAYS the first step.

### Testing Workflow

**Full test cycle (required before commit):**
```bash
dotnet clean       # 1. Clean ALL artifacts
dotnet build       # 2. Fresh compilation
dotnet test        # 3. Run with fresh IL
```

**Expected output:**
```
Passed:     220
Failed:     0
Skipped:    0
Total:      220
Duration:   ~150ms
```

## Troubleshooting

### If tests fail locally but pass in CI:

```bash
$ make clean && make test
# Or explicitly:
$ dotnet clean && dotnet build && dotnet test
```

### If you see "stale test failures":

These are false negatives. The code is correct, but cached IL is old:

1. Stop the dotnet process if running
2. Delete build artifacts:
   ```bash
   dotnet clean
   rm -rf src/*/bin src/*/obj
   ```
3. Rebuild:
   ```bash
   dotnet build
   dotnet test
   ```

### If IDE shows different results than CLI:

IDE may be using its own build cache. Run CLI tests to be sure:

```bash
dotnet test --no-build  # Forces CLI to use latest build
```

## Verification Checklist

Before claiming "tests pass":

```
[ ] Executed: dotnet clean
[ ] Executed: dotnet build
[ ] Executed: dotnet test
[ ] Result: Passed: 220, Failed: 0
[ ] Verified: No cached artifacts in bin/, obj/
[ ] Confirmed: Test count consistent across runs
```

## Prevention Practices

### 1. Makefile Discipline

Use `test: clean` target always:

```makefile
# ✓ CORRECT
test: clean
	@dotnet test

# ❌ INCORRECT
test:
	@dotnet test --no-build
```

### 2. CI/CD Discipline

First step of test job = clean:

```yaml
# ✓ CORRECT
steps:
  - run: dotnet clean
  - run: dotnet build
  - run: dotnet test

# ❌ INCORRECT
steps:
  - run: dotnet build
  - run: dotnet test
```

### 3. Developer Documentation

README.md should include troubleshooting:

```markdown
## Troubleshooting

### If tests fail locally but pass in CI:
Try: `make clean && make test`

### If you see unexpected test failures:
The build cache may be stale.
Solution: `dotnet clean && dotnet build && dotnet test`
```

### 4. Team Agreement

All developers follow: **Always clean before testing.**

This is NOT optional or "performance optimization." It's a correctness requirement.

## Performance Note

Clean build takes ~5 seconds. This is acceptable because:
- Test execution is ~150ms (fast once built)
- False negatives from stale cache are worse than 5s wait
- CI/CD runs overnight (time irrelevant)
- Developer productivity > 5s delay

**Never skip `dotnet clean` to save 5 seconds.**

## Root Cause Analysis

Why does this happen in .NET?

```
C# Source Code
    ↓ (compiler)
IL (Intermediate Language) bytecode
    ↓ (stored in)
assembly.dll (in bin/)
    ↓ (at runtime)
JIT compiled to native code
```

If IL in .dll is old, JIT compiles wrong native code → test fails.

.NET runtime doesn't automatically invalidate .dll when source changes.

**Solution:** Always delete bin/ before recompiling.

## Related Patterns

- See: `.claude/rules/compilation-cache-prevention.md` (system rule)
- See: `architecture-three-layer.instructions.md` (Layer 2 validation)

---

**Source:** Pattern from WP 2026-04-22-06-37-03-resolve-csharp-compilation-errors, Phase 11 TRACK investigation.  
**Confidence:** CRITICAL — affects all .NET projects
**Evidence:** E2E_013 test failure diagnosed and resolved via clean build
