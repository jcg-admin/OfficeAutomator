# Rule: Compilation Cache Prevention

> Cargado automáticamente. Critical for all .NET projects.

## Principle

**ALWAYS clean build artifacts before running tests.**

Stale .dll files cause false test failures.

```bash
dotnet clean && dotnet build && dotnet test
```

## The Rule

**BEFORE any `dotnet test` command, MUST run:**

```bash
dotnet clean    # Delete bin/, obj/ directories
```

## Non-Negotiable Pattern

### Local Development

```bash
make test    # Equivalent to: dotnet clean && dotnet build && dotnet test
```

### CI/CD

```yaml
test:
  steps:
    - run: dotnet clean      # FIRST
    - run: dotnet build      # SECOND
    - run: dotnet test       # THIRD
```

### IDE

If testing via IDE, afterward verify with CLI:

```bash
dotnet clean && dotnet test
```

## Why This Matters

```
Old source → Compile → .dll in bin/
New source → Edit files → dotnet test WITHOUT clean
→ Reuses OLD .dll from previous build
→ Tests fail with misleading errors
→ Code is correct, IL is wrong
```

## Evidence

**Test Case:** E2E_013_State_Machine_Error_Recovery_Path

```
Before clean:  FAILED (incorrect error message)
After clean:   PASSED (correct IL from fresh compile)
```

Same code. Different build state. Different test result.

## Verification

Every test report MUST include:

```
dotnet clean ✓
dotnet build ✓
dotnet test  ✓
Result: Passed: 220, Failed: 0
```

## Prevention

Add to Makefile:

```makefile
test: clean
	@dotnet test

clean:
	@dotnet clean
	@rm -rf src/*/bin src/*/obj
```

Add to README:

```markdown
## If tests fail locally but pass in CI:
Try: make clean && make test
```

## Exceptions

**NONE.** This rule has no exceptions.

Even for "quick local test," clean first. The 5-second penalty is worth avoiding false negatives.

## Related Documentation

- See: `.thyrox/guidelines/csharp-build-practices.instructions.md` (full guide)

---

**Source:** Pattern from WP 2026-04-22-06-37-03-resolve-csharp-compilation-errors  
**Confidence:** CRITICAL — affects all .NET projects
