# Rule: TDD Cycle Completeness

> Cargado automáticamente. Discipline for Red-Green-Refactor methodology.

## Principle

Test-Driven Development requires **three complete phases in sequence:**

```
RED     → Test fails with specific error
GREEN   → Minimal fix, test passes
REFACTOR → Improve clarity, behavior unchanged
```

## Non-Negotiable Rules

### RED Phase

**MUST:**
- ✓ Write test FIRST (before implementation)
- ✓ Test FAILS with specific assertion error
- ✓ Error message is clear and actionable
- ✓ Failure is reproducible

**MUST NOT:**
- ❌ Write vague tests
- ❌ Write test after implementation
- ❌ Have passing test in RED phase

### GREEN Phase

**MUST:**
- ✓ Implement MINIMAL code (1-5 lines typically)
- ✓ Change ONLY what's needed for test to pass
- ✓ Test PASSES
- ✓ All existing tests still pass

**MUST NOT:**
- ❌ Add unrelated features
- ❌ "Improve" unrelated code
- ❌ Refactor (refactor is Phase 3)
- ❌ Have failing tests after GREEN

### REFACTOR Phase

**MUST:**
- ✓ Improve code clarity (better names, extract methods)
- ✓ All tests still pass
- ✓ Behavior unchanged
- ✓ Run full test suite

**MUST NOT:**
- ❌ Add new features
- ❌ Modify test code
- ❌ Change behavior
- ❌ Have failing tests after REFACTOR

## Validation Checklist

```
RED:
  [ ] Test written first
  [ ] Test fails with specific error
  [ ] Error points to exact assertion
  [ ] Reproducible on any machine

GREEN:
  [ ] ≤5 lines added
  [ ] Only fix, no extras
  [ ] Test passes
  [ ] No other test breaks

REFACTOR:
  [ ] Clarity improved
  [ ] No new features
  [ ] No test changes
  [ ] Full suite passes
  [ ] Behavior identical
```

## Commit Pattern

Each TDD cycle is ONE commit:

```
feat(feature-name): add xyz via TDD [RED-GREEN-REFACTOR]

- RED: Added test that validates feature (line X)
- GREEN: Minimal implementation (Y lines)
- REFACTOR: Improved clarity (description)

Resolves: T-NNN (task ID)
```

## Examples of Violations

❌ **Incomplete REFACTOR:**
```csharp
// GREEN: Added feature
public string Generate(config) { return "<?xml...>" + content; }

// REFACTOR: Added ANOTHER feature (❌ NOT allowed)
public string Generate(config, validate=true) {
    if (validate) ValidateSchema();  // NEW FEATURE
    return "<?xml...>" + content;
}
```

❌ **Skipped RED Phase:**
```csharp
// Wrote code without test first
public string Generate(config) { ... }

// Then wrote test (might pass immediately, defeats TDD purpose)
[Fact] public void Test() { Assert.NotNull(Generate(...)); }
```

❌ **Too Much in GREEN:**
```csharp
// WRONG: GREEN should be minimal
public string Generate(config) {
    var decl = GetXmlDeclaration();
    var content = GenerateContent();      // OK
    var validated = ValidateSchema();     // ❌ NEW FEATURE
    var optimized = OptimizeXml();        // ❌ NEW FEATURE
    return decl + content + validated + optimized;
}
```

## Related Documentation

- See: `.thyrox/guidelines/csharp-tdd-guide.instructions.md` (full guide with examples)

---

**Source:** Pattern from WP 2026-04-22-06-37-03-resolve-csharp-compilation-errors  
**Confidence:** PROVEN — 3 complete cycles executed, 220/220 tests passing
