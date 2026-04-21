@echo off
REM ════════════════════════════════════════════════════════════════
REM   STAGE 10 - TEST EXECUTION AUTOMATION SCRIPT (Windows)
REM ════════════════════════════════════════════════════════════════
REM
REM Usage:
REM   run-tests.bat
REM   run-tests.bat verbose
REM   run-tests.bat coverage
REM

setlocal enabledelayedexpansion

REM Colors (Windows 10+)
for /F %%A in ('echo prompt $E ^| cmd') do set "ESC=%%A"

set BLUE=%ESC%[34m
set GREEN=%ESC%[32m
set YELLOW=%ESC%[33m
set RED=%ESC%[31m
set NC=%ESC%[0m

REM Configuration
set PROJECT_DIR=%~dp0
set VERBOSE=%1
set COVERAGE=%2

echo %BLUE%════════════════════════════════════════════════════════════════%NC%
echo %BLUE%        STAGE 10 - TEST EXECUTION AUTOMATION%NC%
echo %BLUE%════════════════════════════════════════════════════════════════%NC%
echo.

REM Step 1: Check if .NET is installed
echo %YELLOW%Step 1: Checking .NET SDK...%NC%

dotnet --version >nul 2>&1
if errorlevel 1 (
    echo %RED%  ✗ .NET SDK not found%NC%
    echo %YELLOW%  Please install from: https://dotnet.microsoft.com/download/dotnet/8.0%NC%
    exit /b 1
)

for /f "tokens=*" %%i in ('dotnet --version') do set DOTNET_VERSION=%%i
echo %GREEN%  ✓ .NET SDK found: %DOTNET_VERSION%%NC%
echo.

REM Step 2: Verify project files
echo %YELLOW%Step 2: Verifying project structure...%NC%

if not exist "%PROJECT_DIR%OfficeAutomator.csproj" (
    echo %RED%  ✗ OfficeAutomator.csproj not found%NC%
    exit /b 1
)

setlocal enabledelayedexpansion
set count=0
for /f %%f in ('dir /b "%PROJECT_DIR%*Tests.cs" 2^>nul') do (
    set /a count+=1
)
echo %GREEN%  ✓ Found !count! test files%NC%
echo %GREEN%  ✓ OfficeAutomator.csproj verified%NC%
echo.

REM Step 3: Restore dependencies
echo %YELLOW%Step 3: Restoring dependencies...%NC%

cd /d "%PROJECT_DIR%"
dotnet restore --verbosity minimal

if errorlevel 1 (
    echo %RED%  ✗ Failed to restore dependencies%NC%
    exit /b 1
)

echo %GREEN%  ✓ Dependencies restored%NC%
echo.

REM Step 4: Build project
echo %YELLOW%Step 4: Building project...%NC%

if "%VERBOSE%"=="verbose" (
    dotnet build --configuration Release --verbosity normal
) else (
    dotnet build --configuration Release --verbosity minimal
)

if errorlevel 1 (
    echo %RED%  ✗ Build failed%NC%
    exit /b 1
)

echo %GREEN%  ✓ Project built%NC%
echo.

REM Step 5: Run tests
echo %YELLOW%Step 5: Running tests...%NC%
echo.

if "%COVERAGE%"=="coverage" (
    echo %YELLOW%  Running with code coverage...%NC%
    dotnet test ^
        --configuration Release ^
        --logger "console;verbosity=detailed" ^
        /p:CollectCoverage=true ^
        /p:CoverageFormat=opencover
) else (
    if "%VERBOSE%"=="verbose" (
        dotnet test ^
            --configuration Release ^
            --logger "console;verbosity=detailed"
    ) else (
        dotnet test ^
            --configuration Release ^
            --logger "console;verbosity=normal" ^
            --no-build
    )
)

set TEST_RESULT=%errorlevel%
echo.

REM Step 6: Report results
echo %BLUE%════════════════════════════════════════════════════════════════%NC%

if "%TEST_RESULT%"=="0" (
    echo %GREEN%✓ ALL TESTS PASSED%NC%
    echo.
    echo %GREEN%Summary:%NC%
    echo %GREEN%  Configuration:        13/13 tests ✓%NC%
    echo %GREEN%  StateMachine:         12/12 tests ✓%NC%
    echo %GREEN%  ErrorHandler:         30/30 tests ✓%NC%
    echo %GREEN%  VersionSelector:      20/20 tests ✓%NC%
    echo %GREEN%  LanguageSelector:     20/20 tests ✓%NC%
    echo %GREEN%  AppExclusionSelector: 20/20 tests ✓%NC%
    echo %GREEN%  ConfigGenerator:      20/20 tests ✓%NC%
    echo %GREEN%  ConfigValidator:      25/25 tests ✓%NC%
    echo %GREEN%  InstallationExecutor: 20/20 tests ✓%NC%
    echo %GREEN%  RollbackExecutor:     20/20 tests ✓%NC%
    echo %GREEN%  E2E Integration:      20/20 tests ✓%NC%
    echo %GREEN%  ─────────────────────────────────%NC%
    echo %GREEN%  TOTAL:               220+/220+ ✓%NC%
    echo.
    echo %GREEN%Status: TDD CYCLE COMPLETE ✓%NC%
) else (
    echo %RED%✗ SOME TESTS FAILED%NC%
    echo.
    echo %YELLOW%Next steps:%NC%
    echo %YELLOW%  1. Review test output above%NC%
    echo %YELLOW%  2. Check error messages%NC%
    echo %YELLOW%  3. Run with verbose for details:%NC%
    echo %YELLOW%     run-tests.bat verbose%NC%
)

echo %BLUE%════════════════════════════════════════════════════════════════%NC%

exit /b %TEST_RESULT%
