# C# Test-Driven Development (TDD) Guide

> Cargado automáticamente. Methodology for C# feature development.

## Three Phases: Red-Green-Refactor

Test-Driven Development has three distinct, sequential phases:

```
RED Phase       → Write failing test, see specific error
GREEN Phase     → Implement minimal fix, test passes
REFACTOR Phase  → Improve code clarity, behavior unchanged
```

## Phase 1: RED — Write Failing Test

**Goal:** Write a test that FAILS with a specific, clear error.

**Requirements:**
- Test is SPECIFIC (not vague)
- Assertion points to exact expectation
- Error message is actionable
- Test is reproducible

### Example

```csharp
[Fact]
public void ConfigGenerator_Should_Prepend_XmlDeclaration() {
    // Arrange
    var generator = new ConfigGenerator();
    var config = new OfficeConfiguration {
        Version = "2024",
        Languages = new[] { "es-ES" }
    };
    
    // Act
    var xml = generator.Generate(config);
    
    // Assert — This assertion FAILS (RED phase)
    Assert.StartsWith("<?xml version=\"1.0\"?>", xml);
}
```

**Run test:**
```bash
dotnet test --filter ConfigGenerator_Should_Prepend_XmlDeclaration
```

**Expected output:**
```
FAILED: Assert.StartsWith failed
Actual value does not start with expected value
Expected: <?xml version="1.0"?>
Actual: <Office xmlns="http://schemas.microsoft.com/...">
```

**Checkpoint:**
- ✅ Test fails with specific error
- ✅ Error points to exact assertion
- ✅ Message is clear

## Phase 2: GREEN — Implement Minimal Fix

**Goal:** Write MINIMAL code to make the test pass.

**Rules:**
- Change ONLY what's necessary
- Don't add extra features
- Don't "improve" unrelated code
- Single responsibility for the fix

### Example

```csharp
public string Generate(OfficeConfiguration config) {
    // MINIMAL FIX: Just add the declaration
    return "<?xml version=\"1.0\"?>" + GenerateXmlContent(config);
}

private string GenerateXmlContent(OfficeConfiguration config) {
    // Existing logic unchanged
    return "<Office xmlns=\"...\">...</Office>";
}
```

**Run test:**
```bash
dotnet test --filter ConfigGenerator_Should_Prepend_XmlDeclaration
```

**Expected output:**
```
PASSED
```

**Checkpoint:**
- ✅ Test passes immediately
- ✅ No unrelated changes
- ✅ Minimal code added (2 lines)
- ✅ All OTHER tests still pass

## Phase 3: REFACTOR — Improve Clarity

**Goal:** Improve code clarity WITHOUT changing behavior.

**Rules:**
- NO new features
- NO test changes
- NO logic changes
- Improve readability (variable names, extract methods)
- Run full test suite after refactoring

### Example

```csharp
private const string XmlDeclaration = "<?xml version=\"1.0\"?>";

public string Generate(OfficeConfiguration config) {
    var xmlContent = GenerateXmlContent(config);
    return XmlDeclaration + xmlContent;
}

private string GenerateXmlContent(OfficeConfiguration config) {
    // Same as before
    return "<Office xmlns=\"...\">...</Office>";
}
```

**Run full test suite:**
```bash
dotnet test
```

**Expected output:**
```
Passed:     220
Failed:     0
Total:      220
```

**Checkpoint:**
- ✅ All tests pass (no regressions)
- ✅ Code is more readable
- ✅ Behavior unchanged

## TDD Cycle Checklist

For each feature implementation:

```
RED Phase:
  [ ] Write test first (test FAILS)
  [ ] Specific assertion (not generic)
  [ ] Error message is clear
  [ ] Reproducible failure

GREEN Phase:
  [ ] Minimal implementation
  [ ] Only fix, no extras
  [ ] Test PASSES
  [ ] All other tests still pass

REFACTOR Phase:
  [ ] Extract constants/methods for clarity
  [ ] Improve variable names
  [ ] NO new features
  [ ] NO test changes
  [ ] Run full test suite
  [ ] All tests pass

Commit:
  [ ] Include all 3 phases
  [ ] Message: feat(feature-name): add xyz via TDD
  [ ] Reference test that validates feature
```

## Anti-Patterns to Avoid

### ❌ "Big Refactor" in Phase 3

```csharp
// WRONG: Refactoring adds new features
public string Generate(OfficeConfiguration config) {
    var declaration = GetXmlDeclaration();  // OK
    var content = GenerateOptimizedContent();  // NEW — not allowed
    var validated = ValidateAgainstSchema();   // NEW — not allowed
    return declaration + content + validated;
}
```

**Fix:** REFACTOR only improves clarity, not functionality.

### ❌ Skipping RED Phase

```csharp
// WRONG: Write code without test first
public string Generate(OfficeConfiguration config) {
    // Just code it
}

// THEN write test (test might pass immediately)
```

**Fix:** Always write failing test FIRST.

### ❌ Incomplete GREEN Phase

```csharp
// WRONG: Implement too much
public string Generate(OfficeConfiguration config) {
    var declaration = "<?xml version=\"1.0\"?>";
    var content = GenerateContent(config);
    var validated = ValidateSchema();       // Not needed for test
    var optimized = OptimizeOutput();       // Not needed for test
    return declaration + content + validated + optimized;
}
```

**Fix:** Minimal change that makes test pass. Nothing more.

## Real TDD Cycles from OfficeAutomator

### Cycle 1: XML Declaration

| Phase | What | Code |
|-------|------|------|
| RED | Test expects `<?xml` prefix | `Assert.StartsWith("<?xml", xml)` → FAILS |
| GREEN | Add prepend to ConfigGenerator | `return "<?xml..." + content` |
| REFACTOR | Extract constant for clarity | `const string XmlDeclaration = "<?xml..."` |

### Cycle 2: 11-State FSM Reachability

| Phase | What | Code |
|-------|------|------|
| RED | Test verifies all 11 states reachable | Loop through states, verify paths → Some FAIL |
| GREEN | Add path logic in GetPathToState() | Implement minimal branching to reach all states |
| REFACTOR | Simplify path calculation | Extract helper methods, improve readability |

### Cycle 3: E2E Error Recovery

| Phase | What | Code |
|-------|------|------|
| RED | Test exercises invalid state sequence | Full path INIT → INSTALL_READY → ERROR → RECOVERY → Fails |
| GREEN | Add state transitions for recovery | Add T-026 through T-030 transitions |
| REFACTOR | Clarify test comments | Add "// Verify error recovery path" comments |

## Test Categories in TDD

Use different test types for different concerns:

| Test Type | RED Phase Example | GREEN Phase Impl |
|-----------|------------------|-----------------|
| Unit | Method returns correct type | Simple return statement |
| Integration | DLL loads and method callable | Assembly.LoadFrom() + reflection |
| E2E | Full workflow succeeds | Complete state transition sequence |

## Prevention Checklist

Before committing TDD cycles:

```
RED Phase:
  [ ] Test written FIRST (before implementation)
  [ ] Test fails with specific error
  [ ] Error message is clear
  [ ] Test is reproducible

GREEN Phase:
  [ ] Minimal change (1-5 lines typically)
  [ ] Only code needed for test to pass
  [ ] No unrelated improvements
  [ ] All other tests still pass

REFACTOR Phase:
  [ ] Clarity improved (better names, extract methods)
  [ ] NO new features
  [ ] NO test modifications
  [ ] NO behavior changes
  [ ] Full test suite passes

Documentation:
  [ ] Commit message: feat(scope): description
  [ ] Reference test file and assertion
  [ ] Link to Phase 10 task plan if in execution phase
```

## Integration with Phase 10 EXECUTE

When executing Phase 10 IMPLEMENT using TDD:

1. **Task plan (Phase 8)** defines "what" to build
2. **TDD cycle (Phase 10)** defines "how" to validate it:
   - Test from task plan becomes RED phase
   - Implementation satisfies GREEN phase
   - Code quality becomes REFACTOR phase

3. **Commit** after each complete cycle:
   ```
   feat(task-name): add feature [RED-GREEN-REFACTOR]
   
   - RED: Added test that validates feature
   - GREEN: Minimal implementation to pass test
   - REFACTOR: Improved code clarity
   
   Resolves: T-NNN (Phase 8 task ID)
   ```

## Related Patterns

- See: `csharp-build-practices.instructions.md` (always clean before testing)
- See: `csharp-state-machine-patterns.instructions.md` (FSM-specific TDD)
- See: `.claude/rules/tdd-cycle-completeness.md` (system rule)

---

**Source:** Pattern from WP 2026-04-22-06-37-03-resolve-csharp-compilation-errors  
**Confidence:** PROVEN — 3 complete cycles executed, 220/220 tests passing  
**Evidence:** All TDD cycles documented in track/resolve-csharp-compilation-errors-tdd-analysis.md
