```yml
created_at: 2026-04-22 06:37:03
project: OfficeAutomator
work_package: 2026-04-22-06-37-03-resolve-csharp-compilation-errors
updated_at: 2026-04-22 06:39:09
author: Claude
status: Aprobado
```

# Risk Register — Resolve C# Compilation Errors

---

## Active Risks

### Risk 1: Namespace Fix Introduces Unintended Side Effects

| Attribute | Value |
|-----------|-------|
| **Risk ID** | R-001 |
| **Description** | Changing RootNamespace from `Apps72.OfficeAutomator.Core.Tests` to `OfficeAutomator.Core.Tests` may affect tooling, CI/CD pipelines, or external references to the test assembly |
| **Probability** | Medium (40%) |
| **Impact** | High — Could break test discovery, CI/CD integration, or build pipelines |
| **Priority** | HIGH |

**Mitigation Strategy:**
1. Change only RootNamespace, keep AssemblyName unchanged (`Apps72.OfficeAutomator.Core.Tests`)
2. Verify with `dotnet build --configuration Debug` immediately after change
3. Run `dotnet test --list-tests` to confirm test discovery still works
4. Check CI pipeline configuration for hardcoded assembly references

**Contingency:**
- If build fails: Revert RootNamespace change immediately
- If test discovery fails: Revert and use Solution B (change all file namespaces instead)
- Document which approach failed and why

---

### Risk 2: File Namespace Inconsistency Across Test Project

| Attribute | Value |
|-----------|-------|
| **Risk ID** | R-002 |
| **Description** | If Solution B is chosen (update file namespaces), 10+ test files must be updated consistently. Risk of missing some files or partial updates |
| **Probability** | Medium (50%) if Solution B chosen, Low (5%) if Solution A chosen |
| **Impact** | Medium — Could leave some tests in wrong namespace, causing partial compilation failures |
| **Priority** | MEDIUM (if Solution B chosen), LOW (if Solution A chosen) |

**Mitigation Strategy:**
1. Prefer Solution A (RootNamespace change) to avoid file-by-file updates
2. If Solution B is necessary, use automated find/replace: `sed -i 's/namespace OfficeAutomator.Tests/namespace OfficeAutomator.Core.Tests/g' tests/**/*.cs`
3. Verify with grep: `grep "namespace OfficeAutomator" tests/**/*.cs | sort | uniq -c`
4. Expected output: All files with `OfficeAutomator.Core.Tests`

---

### Risk 3: Build Cache Hiding Deeper Issues

| Attribute | Value |
|-----------|-------|
| **Risk ID** | R-003 |
| **Description** | Namespace fix resolves immediate CS0246 errors, but deeper issues with project structure could remain hidden until test execution |
| **Probability** | Low (20%) |
| **Impact** | High — Tests might compile but fail to execute or discover |
| **Priority** | MEDIUM |

**Mitigation Strategy:**
1. After namespace fix, clean all build artifacts: `dotnet clean`
2. Full rebuild: `dotnet build --configuration Debug`
3. List all tests: `dotnet test --list-tests`
4. Execute subset of tests: `dotnet test --filter "ClassName=ErrorHandlerTests"`
5. Full test run: `dotnet test` (verify all 220+ tests pass)

**Contingency:**
- If tests compile but fail to discover: Project reference or NuGet issue
- If tests discover but fail to execute: Runtime dependency issue
- Escalate to Phase 3 DIAGNOSE for deeper analysis

---

### Risk 4: External Tools or Documentation Reference Old Namespace

| Attribute | Value |
|-----------|-------|
| **Risk ID** | R-004 |
| **Description** | If any external tools, CI/CD scripts, or documentation hardcode the old namespace `Apps72.OfficeAutomator.Core.Tests`, they will break after namespace change |
| **Probability** | Low (15%) |
| **Impact** | Medium — CI/CD pipelines or deployment scripts might fail |
| **Priority** | LOW |

**Mitigation Strategy:**
1. Search for hardcoded references: `grep -r "Apps72.OfficeAutomator.Core.Tests" . --include="*.yml" --include="*.yaml" --include="*.sh"`
2. Search for hardcoded references in docs: `grep -r "Apps72.OfficeAutomator.Core.Tests" docs/`
3. Update all references found during Phase 8 PLAN EXECUTION
4. Document in CONTRIBUTING.md: "Test project namespace is OfficeAutomator.Core.Tests"

**Contingency:**
- If CI/CD breaks: Revert namespace change and roll back pipeline updates

---

## Closed Risks

### Risk: Missing Source Files

| Status | CLOSED |
|--------|--------|
| **Resolution** | Phase 1 DISCOVER confirmed all source files exist: ErrorHandler.cs, Configuration.cs, OfficeAutomatorStateMachine.cs, etc. Files are present and properly implemented. Root cause is NOT missing files. |

### Risk: Project Reference Not Declared

| Status | CLOSED |
|--------|--------|
| **Resolution** | Phase 1 DISCOVER confirmed ProjectReference is correctly declared in OfficeAutomator.Core.Tests.csproj with valid relative path (../../src/OfficeAutomator.Core/OfficeAutomator.Core.csproj). Path verified to resolve correctly from project root. |

### Risk: .NET SDK Not Installed

| Status | CLOSED |
|--------|--------|
| **Resolution** | Previous WP (verify-test-execution-environment) successfully installed .NET 8.0.110. Verified with `dotnet --version`. Compilation proceeds to semantic analysis stage, confirming SDK is operational. |

---

## Risk Monitoring & Escalation

### Escalation Trigger 1: Namespace Fix Fails to Resolve Errors

**Condition:** After applying namespace fix, `dotnet build` still produces CS0246 errors

**Action:**
1. Investigate deeper: `dotnet build --verbosity detailed 2>&1 | grep -C 5 "CS0246"`
2. Escalate to Phase 3 DIAGNOSE for root cause re-analysis
3. Document findings in error register (`.thyrox/context/errors/`)

### Escalation Trigger 2: Build Succeeds but Tests Don't Discover

**Condition:** `dotnet build` returns 0 exit code, but `dotnet test --list-tests` returns 0 tests

**Action:**
1. Check project structure: Confirm test classes inherit from `TestClass` (xUnit requirement)
2. Check file naming: Confirm files end with `Tests.cs`
3. Escalate to Phase 3 DIAGNOSE for test discovery analysis

### Escalation Trigger 3: Tests Execute but Fail at Runtime

**Condition:** Tests compile and discover, but execution fails with runtime errors

**Action:**
1. Not a namespace issue — escalate to Phase 3 DIAGNOSE for dependency/runtime analysis
2. Could indicate: Missing xUnit, Moq version mismatch, missing IDisposable patterns, etc.

---

## Summary

**Total Risks:** 4 active, 3 closed = 7 total identified

**High Priority:** 1 (namespace fix side effects)
**Medium Priority:** 2 (file consistency, build cache)
**Low Priority:** 1 (external tools)

**Next Review:** After Phase 8 PLAN EXECUTION decides which solution to implement

**Owner:** Claude (Phase 1 DISCOVER)
**Status:** Risk register complete, all risks documented

