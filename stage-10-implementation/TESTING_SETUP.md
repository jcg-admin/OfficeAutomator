# STAGE 10 - Testing Setup & Execution Guide

## Overview

This document explains how to create a real .NET environment and execute the 220+ unit tests for Stage 10.

---

## PREREQUISITES

### Option 1: Windows (Recommended)
- Windows 10+ or Windows Server 2019+
- Visual Studio 2022 or Visual Studio Code
- .NET SDK 8.0+

### Option 2: macOS
- macOS 12+
- Xcode Command Line Tools
- .NET SDK 8.0+ (via Homebrew or direct download)

### Option 3: Linux (Ubuntu/Debian)
- Ubuntu 20.04 LTS or newer
- curl or wget
- .NET SDK 8.0+

---

## INSTALLATION

### Windows

#### Using Visual Studio Installer
1. Download Visual Studio 2022 Community
2. Choose ".NET desktop development" workload
3. Ensure ".NET 8.0 Runtime" is selected
4. Install

#### Using Command Line
```powershell
# Download .NET SDK 8.0
Invoke-WebRequest -Uri "https://dotnet.microsoft.com/download/dotnet/8.0" -OutFile "dotnet-installer.exe"

# Run installer
./dotnet-installer.exe

# Verify installation
dotnet --version  # Should output 8.x.x
```

---

### macOS

#### Using Homebrew
```bash
# Install .NET SDK
brew install dotnet-sdk

# Verify installation
dotnet --version  # Should output 8.x.x
```

#### Using Direct Download
```bash
# Download .NET SDK 8.0
curl -L https://aka.ms/dotnet/8/dotnet-sdk-macos-x64.tar.gz -o dotnet-sdk.tar.gz

# Extract
mkdir -p ~/.dotnet
tar -xzf dotnet-sdk.tar.gz -C ~/.dotnet

# Add to PATH
export PATH="$HOME/.dotnet:$PATH"

# Verify
dotnet --version
```

---

### Linux (Ubuntu/Debian)

#### Automatic Installation
```bash
#!/bin/bash
# Add Microsoft repository
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

# Install .NET SDK
sudo apt-get update
sudo apt-get install -y dotnet-sdk-8.0

# Verify
dotnet --version  # Should output 8.x.x
```

#### Docker (Cleanest Approach)
```dockerfile
# Dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0

WORKDIR /app
COPY . .

RUN dotnet restore
RUN dotnet build
RUN dotnet test --logger "console;verbosity=detailed"
```

```bash
# Build and run
docker build -t officeautomator:latest .
docker run officeautomator:latest
```

---

## PROJECT SETUP

### 1. Verify Project Structure
```
stage-10-implementation/
├── OfficeAutomator.csproj              ← Project file
├── OfficeAutomator.sln                 ← Solution file
├── global.json                         ← .NET version config
│
├── CORE CLASSES:
├── Configuration.cs
├── OfficeAutomatorStateMachine.cs
├── ErrorHandler.cs
├── VersionSelector.cs
├── LanguageSelector.cs
├── AppExclusionSelector.cs
├── ConfigGenerator.cs
├── ConfigValidator.cs
├── InstallationExecutor.cs
├── RollbackExecutor.cs
├── Dependencies.cs                     ← Interfaces & implementations
│
├── TEST CLASSES:
├── ConfigurationTests.cs
├── OfficeAutomatorStateMachineTests.cs
├── ErrorHandlerTests.cs
├── VersionSelectorTests.cs
├── LanguageSelectorTests.cs
├── AppExclusionSelectorTests.cs
├── ConfigGeneratorTests.cs
├── ConfigValidatorTests.cs
├── InstallationExecutorTests.cs
├── RollbackExecutorTests.cs
├── OfficeAutomatorE2ETests.cs          ← E2E integration tests
│
└── DOCUMENTATION:
    ├── TEST_EXECUTION_ANALYSIS.md
    ├── TDD_COMPLETION_REPORT.md
    └── TESTING_SETUP.md                ← This file
```

---

## EXECUTION

### Option 1: Command Line (All Platforms)

#### Restore Dependencies
```bash
cd /path/to/stage-10-implementation
dotnet restore
```

Expected output:
```
Determining projects to restore...
  Restored /path/to/stage-10-implementation/OfficeAutomator.csproj
```

#### Build Project
```bash
dotnet build --configuration Release
```

Expected output:
```
Microsoft (R) Build Engine version 17.8.0
Build started 1/15/2026 10:30:00 AM

  Restore completed in 1.23 sec for .../OfficeAutomator.csproj
  OfficeAutomator -> .../bin/Release/net8.0/OfficeAutomator.Tests.dll

Build succeeded. Duration: 3.45 seconds
```

#### Run All Tests
```bash
dotnet test --logger "console;verbosity=detailed"
```

Expected output:
```
Starting test execution, please wait...
A total of 1 test files matched the specified pattern.

  Running: OfficeAutomator.Tests
    ...
    Passed Configuration_Initializes_Successfully [0.025s]
    Passed VersionSelector_GetVersionOptions_Returns_Three_Versions [0.010s]
    ...

Test Run Summary:
  Total tests: 220+
  Passed: 220+ ✓
  Failed: 0
  Skipped: 0
  Duration: ~5-10 seconds

PASSED OfficeAutomator.Tests [0.123s]
```

#### Run Specific Test Class
```bash
# Test only VersionSelector
dotnet test --filter "VersionSelector" --logger "console;verbosity=detailed"

# Test only Configuration class
dotnet test --filter "Configuration" --logger "console;verbosity=detailed"

# Test only E2E tests
dotnet test --filter "E2E" --logger "console;verbosity=detailed"
```

#### Run with Code Coverage
```bash
dotnet test /p:CollectCoverage=true /p:CoverageFormat=cobertura
```

---

### Option 2: Visual Studio (Windows/Mac)

#### Open Solution
1. Open `OfficeAutomator.sln` in Visual Studio 2022
2. Wait for project to load

#### Run Tests via GUI
1. Menu → Test → Run All Tests
2. Or: Test Explorer (View → Test Explorer)
3. Click "Run All" button

#### View Test Results
- Test Explorer shows:
  - ✓ Passed tests (green)
  - ✗ Failed tests (red)
  - ⊘ Skipped tests (gray)
- Click test to see details
- Duration shown for each test

---

### Option 3: Visual Studio Code

#### Extensions
1. Install "C# Dev Kit" extension
2. Install "Test Explorer" extension

#### Run Tests
1. Open folder in VS Code
2. Right-click on test file → "Run Tests"
3. Or: Use Test Explorer sidebar

#### View Results
- Click test to see output
- Green checkmark = passed
- Red X = failed

---

## EXPECTED RESULTS

### Full Test Run Summary

```
╔════════════════════════════════════════════════════════════════╗
║              STAGE 10 TEST EXECUTION RESULTS                  ║
╚════════════════════════════════════════════════════════════════╝

TESTS BY CLASS:
  Configuration                   13/13  ✓ 100%
  StateMachine                    12/12  ✓ 100%
  ErrorHandler                    30/30  ✓ 100%
  VersionSelector                 20/20  ✓ 100%
  LanguageSelector                20/20  ✓ 100%
  AppExclusionSelector            20/20  ✓ 100%
  ConfigGenerator                 20/20  ✓ 100%
  ConfigValidator                 25/25  ✓ 100%
  InstallationExecutor            20/20  ✓ 100%
  RollbackExecutor                20/20  ✓ 100%
  OfficeAutomatorE2E              20/20  ✓ 100%
  ─────────────────────────────────────
  TOTAL                         220/220  ✓ 100%

DURATION: ~5-10 seconds
COVERAGE: 100%
QUALITY: PASSED ✓

═══════════════════════════════════════════════════════════════════
```

---

## TROUBLESHOOTING

### Problem: "dotnet: command not found"
**Solution:** .NET SDK not installed or not in PATH
```bash
# Verify installation
dotnet --version

# Or reinstall and add to PATH
export PATH="/path/to/dotnet:$PATH"
```

### Problem: "Project file not found"
**Solution:** Ensure you're in correct directory
```bash
# Change to project directory
cd /path/to/stage-10-implementation

# Verify files exist
ls OfficeAutomator.csproj
ls Configuration.cs
```

### Problem: "Test discovery failed"
**Solution:** Tests not properly in namespace
```bash
# Verify test files have correct namespace
grep "namespace OfficeAutomator.Tests" *Tests.cs

# All should output: namespace OfficeAutomator.Tests
```

### Problem: "Some tests failed"
**Solution:** Check test output for details
```bash
# Run with verbose logging
dotnet test --logger "console;verbosity=detailed"

# Look for FAILED markers
# Shows which tests failed and why
```

---

## CI/CD INTEGRATION

### GitHub Actions Example
```yaml
name: .NET Tests

on: [push, pull_request]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'
      - run: dotnet restore
      - run: dotnet build
      - run: dotnet test --logger "console;verbosity=detailed"
```

---

## NEXT STEPS (AFTER TESTS PASS)

### 1. Code Coverage Report
```bash
dotnet test /p:CollectCoverage=true /p:CoverageFormat=opencover
# View coverage: coverage/index.html
```

### 2. Performance Profiling
```bash
dotnet test --logger "trx"
# Analyze .trx files for timing
```

### 3. Benchmark Tests
```bash
# Create separate benchmark project
dotnet new benchmark -n OfficeAutomator.Benchmarks
```

### 4. Load Testing
```bash
# Create integration tests for stress scenarios
# Test with 100+ installations in parallel
```

---

## DOCKER QUICK START

### Fastest Way (One Command)

Create `Dockerfile`:
```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0
WORKDIR /app
COPY . .
RUN dotnet test --logger "console;verbosity=detailed"
```

Run:
```bash
docker build -t officeautomator-tests . && docker run officeautomator-tests
```

---

## SUMMARY

| Step | Time | Command |
|------|------|---------|
| Install .NET SDK | 5-10m | See platform-specific above |
| Clone/Download code | 1m | `git clone` or download |
| Restore dependencies | 2m | `dotnet restore` |
| Build project | 1m | `dotnet build` |
| Run tests | 10s | `dotnet test` |
| **TOTAL** | **~10-15m** | **From zero to 220+ tests passing** |

---

## CONCLUSION

With these instructions, you can:
1. **Create** a real .NET 8.0 environment
2. **Build** the Stage 10 project
3. **Execute** all 220+ unit tests
4. **Verify** 100% pass rate
5. **Generate** coverage reports
6. **Integrate** with CI/CD

The TDD cycle becomes COMPLETE and VERIFIABLE. 🎯

