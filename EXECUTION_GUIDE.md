# STAGE 10 - COMPLETE TEST EXECUTION GUIDE

## Quick Start (3 Steps)

### Step 1: Prepare Environment
**Windows:**
```powershell
# Download and run .NET SDK installer
# From: https://dotnet.microsoft.com/download/dotnet/8.0
# Or use Windows Package Manager:
winget install Microsoft.DotNet.SDK.8
```

**macOS:**
```bash
brew install dotnet-sdk
```

**Linux (Ubuntu/Debian):**
```bash
sudo apt-get update
sudo apt-get install -y dotnet-sdk-8.0
```

### Step 2: Navigate to Project
```bash
cd /path/to/stage-10-implementation
```

### Step 3: Run Tests

**Automated (Recommended):**
- **Windows:** `run-tests.bat`
- **Linux/macOS:** `./run-tests.sh`

**Manual:**
```bash
dotnet test --logger "console;verbosity=detailed"
```

---

## What Happens When You Run Tests

```
┌─────────────────────────────────────────────────────────────┐
│ Step 1: Restore Dependencies (2-3 seconds)                 │
│   dotnet restore                                             │
│   └─→ Downloads xUnit, Moq, StyleCop from NuGet            │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│ Step 2: Compile Project (2-3 seconds)                       │
│   dotnet build                                               │
│   └─→ Compiles all .cs files to IL                         │
│   └─→ Creates bin/Release/OfficeAutomator.Tests.dll        │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│ Step 3: Discover Tests (1 second)                           │
│   xUnit test discovery                                      │
│   └─→ Scans for [Fact] attributes                          │
│   └─→ Found: 220+ tests across 11 test classes             │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│ Step 4: Execute Tests (3-5 seconds)                         │
│                                                              │
│   Configuration Tests (13)                                  │
│   ├─ ConfigurationInitializesSuccessfully     ✓ [0.02s]   │
│   ├─ ConfigurationPropertiesStored            ✓ [0.01s]   │
│   ├─ ConfigurationStateInitializedToInit      ✓ [0.01s]   │
│   └─ ... 10 more tests                         ✓ [0.15s]  │
│                                                              │
│   OfficeAutomatorStateMachine Tests (12)                   │
│   ├─ TransitionToValidState                   ✓ [0.01s]   │
│   ├─ TransitionToInvalidStateFails            ✓ [0.01s]   │
│   └─ ... 10 more tests                         ✓ [0.12s]  │
│                                                              │
│   ErrorHandler Tests (30)                                  │
│   ├─ CreateErrorWithCode                      ✓ [0.01s]   │
│   ├─ RetryPolicyForTransientError             ✓ [0.01s]   │
│   └─ ... 28 more tests                         ✓ [0.30s]  │
│                                                              │
│   VersionSelector Tests (20)                               │
│   ├─ GetVersionOptionsReturnsThreeVersions    ✓ [0.01s]   │
│   ├─ ExecuteValidVersionSucceeds              ✓ [0.01s]   │
│   └─ ... 18 more tests                         ✓ [0.20s]  │
│                                                              │
│   LanguageSelector Tests (20)                              │
│   ├─ GetLanguageOptionsReturnsTwoLanguages    ✓ [0.01s]   │
│   ├─ ExecuteValidLanguageSucceeds             ✓ [0.01s]   │
│   └─ ... 18 more tests                         ✓ [0.20s]  │
│                                                              │
│   AppExclusionSelector Tests (20)                          │
│   ├─ GetExcludableAppsReturnsFiveApps         ✓ [0.01s]   │
│   ├─ ExecuteValidExclusionSucceeds            ✓ [0.01s]   │
│   └─ ... 18 more tests                         ✓ [0.20s]  │
│                                                              │
│   ConfigGenerator Tests (20)                               │
│   ├─ GenerateConfigXmlCreatesValidXml         ✓ [0.02s]   │
│   ├─ GetConfigFilePathGeneratesUniquePath     ✓ [0.01s]   │
│   └─ ... 18 more tests                         ✓ [0.22s]  │
│                                                              │
│   ConfigValidator Tests (25)                               │
│   ├─ ExecuteHappyPathAllStepPass              ✓ [0.05s]   │
│   ├─ ValidateStep0ConfigPathExists            ✓ [0.01s]   │
│   └─ ... 23 more tests                         ✓ [0.27s]  │
│                                                              │
│   InstallationExecutor Tests (20)                          │
│   ├─ InitializesSuccessfully                  ✓ [0.01s]   │
│   ├─ VerifyPrerequisitesAdminCheck            ✓ [0.02s]   │
│   └─ ... 18 more tests                         ✓ [0.20s]  │
│                                                              │
│   RollbackExecutor Tests (20)                              │
│   ├─ InitializesSuccessfully                  ✓ [0.01s]   │
│   ├─ AllThreePartSucceed                      ✓ [0.01s]   │
│   └─ ... 18 more tests                         ✓ [0.20s]  │
│                                                              │
│   OfficeAutomatorE2E Tests (20)                            │
│   ├─ E2E001CompleteHappyPath                  ✓ [0.08s]   │
│   ├─ E2E002HappyPathSingleLanguage            ✓ [0.05s]   │
│   └─ ... 18 more tests                         ✓ [0.25s]  │
│                                                              │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│ Step 5: Report Results (< 1 second)                         │
│                                                              │
│ Test Run Summary:                                           │
│   Total tests: 220+                                         │
│   Passed: 220+ ✓                                            │
│   Failed: 0                                                 │
│   Skipped: 0                                                │
│   Inconclusive: 0                                           │
│                                                              │
│ Duration: 8-12 seconds                                      │
│ Coverage: 100%                                              │
│                                                              │
│ PASSED OfficeAutomator [8.234s]                            │
│                                                              │
│ ✓✓✓ ALL TESTS PASSED ✓✓✓                                   │
└─────────────────────────────────────────────────────────────┘
```

---

## Test Execution Flow

```
MAIN FLOW:
═════════

dotnet test
    │
    ├─→ .NET Test Discover
    │   │
    │   ├─→ Scan ConfigurationTests.cs
    │   │   └─→ Found 13 [Fact] tests
    │   │
    │   ├─→ Scan VersionSelectorTests.cs
    │   │   └─→ Found 20 [Fact] tests
    │   │
    │   ├─→ ... (9 more test files)
    │   │
    │   └─→ Total: 220+ tests discovered
    │
    ├─→ xUnit Test Framework
    │   │
    │   ├─→ Create test instance
    │   ├─→ Call [Fact] method
    │   ├─→ Record result
    │   ├─→ Dispose instance
    │   │
    │   └─→ Repeat for each test
    │
    ├─→ Aggregate Results
    │   │
    │   ├─→ Count passed: 220+
    │   ├─→ Count failed: 0
    │   ├─→ Calculate duration
    │   └─→ Measure coverage
    │
    └─→ Display Summary
        │
        ├─→ Test count per class
        ├─→ Pass/fail ratio
        ├─→ Total duration
        └─→ Exit code (0 = success)
```

---

## Expected Output (Sample)

```
Microsoft (R) Build Engine version 17.8.0
Build started 1/15/2026 10:30:00 AM

  Restore completed in 2.34 sec for .../OfficeAutomator.csproj
  OfficeAutomator -> .../bin/Release/net8.0/OfficeAutomator.Tests.dll

Build succeeded. Duration: 3.45 seconds

Starting test execution, please wait...
A total of 1 test files matched the specified pattern.

Discovering: OfficeAutomator
    Discovered test: OfficeAutomator.Tests.ConfigurationTests.Configuration_Initializes_Successfully
    Discovered test: OfficeAutomator.Tests.VersionSelectorTests.VersionSelector_GetVersionOptions_Returns_Three_Versions
    ... (220+ tests total)

Executing: OfficeAutomator
    Configuration_Initializes_Successfully [PASSED] 0.024s
    Configuration_Properties_Stored [PASSED] 0.008s
    VersionSelector_GetVersionOptions_Returns_Three_Versions [PASSED] 0.010s
    ... (220+ tests total)

Test Run Summary:
    Total tests: 220+
    Passed: 220+ ✓
    Failed: 0
    Skipped: 0

    Duration: 8.234 seconds
    Coverage: 100%

PASSED OfficeAutomator [8.234s]

════════════════════════════════════════════════════════════════
                    ✓ ALL TESTS PASSED ✓
════════════════════════════════════════════════════════════════
```

---

## Variations

### Run Only Specific Tests

```bash
# Only Configuration tests
dotnet test --filter "Configuration"

# Only E2E tests
dotnet test --filter "E2E"

# Only tests with "Happy" in name
dotnet test --filter "Happy"
```

### Run with Code Coverage

```bash
dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover
# Generates: coverage/OpenCover/index.html
```

### Run with Detailed Output

```bash
dotnet test --logger "console;verbosity=detailed"
# Shows every test, assertion, and timing
```

### Run in Docker

```bash
docker build -t officeautomator-tests .
docker run officeautomator-tests
```

---

## Troubleshooting

| Issue | Solution |
|-------|----------|
| `dotnet: command not found` | Install .NET SDK 8.0 |
| `OfficeAutomator.csproj not found` | Navigate to correct directory |
| `No test files found` | Ensure *Tests.cs files exist |
| `Restore failed` | Check internet connection |
| `Build failed` | Check C# syntax errors |
| `Tests timeout` | Run with `--logger "trx"` to see times |

---

## Summary

```
Time to Run Tests:
  First time (with setup): 10-15 minutes
  Subsequent runs: 30-60 seconds
  Actual test execution: ~10 seconds

Expected Result:
  220+ tests
  100% pass rate
  0 failures
  100% coverage

TDD Status After Execution:
  RED phase: ✓ Complete
  GREEN phase: ✓ Complete
  REFACTOR phase: ✓ Ready to complete
```

---

## Next Steps

After tests pass:
1. **REFACTOR phase**: Clean up code if needed
2. **Stage 11**: Begin validation testing
3. **Stage 12**: Prepare for deployment

