# Rule: FSM Minimal Path Testing

> Cargado automáticamente. Test strategy for Finite State Machines.

## Principle

Test coverage for FSMs = **number of states, not combinations**.

```
FSM: 11 states, infinite transitions
Coverage needed: 11 paths (one per state)
NOT: 11 × 10 = 110 possible combinations
```

## The Rule

**Test Requirement:**
- ✓ Each state must be reachable from initial state
- ✓ Minimum 1 valid path per state
- ✓ Each transition in path verified

**Test Coverage Formula:**
```
FSM Coverage = Number of States Reachable / Total States
For OfficeAutomator: 11/11 = 100%
```

## Implementation Pattern

```csharp
[Fact]
public void FSM_All_States_Reachable() {
    var fsm = new OfficeAutomatorStateMachine();
    var allStates = new[] { "INIT", "STATE2", /* ... 9 more */ };
    
    foreach (var target in allStates) {
        var path = GetMinimalPathToState(target);
        
        foreach (var nextState in path) {
            Assert.True(fsm.TransitionTo(nextState));
        }
        
        fsm.Reset();
    }
}
```

**Count:** 11 tests (one per state)  
**NOT:** 110 tests (all combinations)

## Why This Works

```
FSM Invariant: Transitions are deterministic
(no branching, no random behavior)

If path INIT→A→B→C works:
  Then every valid transition is deterministic
  Therefore: All paths of form INIT→...→X work
  Therefore: Testing one path per state is SUFFICIENT
```

## Coverage Checklist

```
[ ] All N states have ≥1 valid path from initial state
[ ] Each path tested explicitly
[ ] Invalid transitions rejected (negative tests)
[ ] State invariants maintained
[ ] No infinite loops
[ ] Full test suite passes (220/220)
```

## Anti-Pattern

❌ **WRONG: Testing all combinations**
```csharp
[Theory]
[InlineData("INIT", "STATE2")]
[InlineData("INIT", "STATE3")]
// ... 110 test cases ...
public void FSM_AllTransitions(string from, string to) { }
```

**Problem:** Combinatorial explosion, slow to run, hard to maintain

✓ **CORRECT: Minimal path set**
```csharp
[Fact]
public void FSM_All_States_Reachable() {
    // 11 states = 11 paths tested
}
```

## Metrics

For FSM with N states:

| Metric | Acceptable | Example |
|--------|-----------|---------|
| **State Coverage** | 100% | 11/11 states reachable |
| **Path Coverage** | 100% | One path per state |
| **Transition Coverage** | ≥80% | Not all combos needed |
| **Test Count** | N tests | 11 tests for 11 states |

## Related Documentation

- See: `.thyrox/guidelines/csharp-state-machine-patterns.instructions.md` (full guide)
- See: `adr-fsm-test-coverage-strategy.md` (architectural decision)

---

**Source:** Pattern from WP 2026-04-22-06-37-03-resolve-csharp-compilation-errors  
**Confidence:** INFERRED (architectural pattern, proven by 220/220 test pass)
