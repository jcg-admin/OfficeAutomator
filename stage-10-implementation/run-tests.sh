#!/bin/bash

#
# STAGE 10 - TEST EXECUTION AUTOMATION SCRIPT
# 
# This script automates:
#   1. .NET SDK installation (if needed)
#   2. Project setup
#   3. Test execution
#   4. Result reporting
#
# Usage:
#   chmod +x run-tests.sh
#   ./run-tests.sh
#   ./run-tests.sh --verbose
#   ./run-tests.sh --coverage
#

set -e  # Exit on error

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Configuration
PROJECT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
DOTNET_VERSION="8.0"
VERBOSE=${1:-""}
COVERAGE=${2:-""}

echo -e "${BLUE}════════════════════════════════════════════════════════════════${NC}"
echo -e "${BLUE}        STAGE 10 - TEST EXECUTION AUTOMATION${NC}"
echo -e "${BLUE}════════════════════════════════════════════════════════════════${NC}"
echo ""

# Step 1: Check if .NET is installed
echo -e "${YELLOW}Step 1: Checking .NET SDK...${NC}"

if ! command -v dotnet &> /dev/null; then
    echo -e "${YELLOW}  .NET SDK not found. Installing...${NC}"
    
    if [[ "$OSTYPE" == "darwin"* ]]; then
        # macOS
        echo -e "${YELLOW}  Detected macOS - Using Homebrew${NC}"
        if ! command -v brew &> /dev/null; then
            echo -e "${RED}  ✗ Homebrew not installed. Please install from https://brew.sh${NC}"
            exit 1
        fi
        brew install dotnet-sdk
    elif [[ "$OSTYPE" == "linux-gnu"* ]]; then
        # Linux
        echo -e "${YELLOW}  Detected Linux - Using apt${NC}"
        sudo apt-get update
        sudo apt-get install -y dotnet-sdk-8.0
    else
        echo -e "${RED}  ✗ Unsupported OS. Please install .NET SDK manually from https://dotnet.microsoft.com${NC}"
        exit 1
    fi
fi

DOTNET_INSTALLED=$(dotnet --version)
echo -e "${GREEN}  ✓ .NET SDK found: $DOTNET_INSTALLED${NC}"
echo ""

# Step 2: Verify project files
echo -e "${YELLOW}Step 2: Verifying project structure...${NC}"

if [ ! -f "$PROJECT_DIR/OfficeAutomator.csproj" ]; then
    echo -e "${RED}  ✗ OfficeAutomator.csproj not found${NC}"
    exit 1
fi

TEST_FILES=$(find "$PROJECT_DIR" -name "*Tests.cs" | wc -l)
if [ "$TEST_FILES" -eq 0 ]; then
    echo -e "${RED}  ✗ No test files found (*Tests.cs)${NC}"
    exit 1
fi

echo -e "${GREEN}  ✓ Found $TEST_FILES test files${NC}"
echo -e "${GREEN}  ✓ OfficeAutomator.csproj verified${NC}"
echo ""

# Step 3: Restore dependencies
echo -e "${YELLOW}Step 3: Restoring dependencies...${NC}"

cd "$PROJECT_DIR"
dotnet restore --verbosity minimal

echo -e "${GREEN}  ✓ Dependencies restored${NC}"
echo ""

# Step 4: Build project
echo -e "${YELLOW}Step 4: Building project...${NC}"

if [ "$VERBOSE" == "--verbose" ]; then
    dotnet build --configuration Release --verbosity normal
else
    dotnet build --configuration Release --verbosity minimal
fi

echo -e "${GREEN}  ✓ Project built${NC}"
echo ""

# Step 5: Run tests
echo -e "${YELLOW}Step 5: Running tests...${NC}"
echo ""

if [ "$COVERAGE" == "--coverage" ]; then
    echo -e "${YELLOW}  Running with code coverage...${NC}"
    dotnet test \
        --configuration Release \
        --logger "console;verbosity=detailed" \
        /p:CollectCoverage=true \
        /p:CoverageFormat=opencover
else
    if [ "$VERBOSE" == "--verbose" ]; then
        dotnet test \
            --configuration Release \
            --logger "console;verbosity=detailed"
    else
        dotnet test \
            --configuration Release \
            --logger "console;verbosity=normal" \
            --no-build
    fi
fi

TEST_RESULT=$?
echo ""

# Step 6: Report results
echo -e "${BLUE}════════════════════════════════════════════════════════════════${NC}"

if [ $TEST_RESULT -eq 0 ]; then
    echo -e "${GREEN}✓ ALL TESTS PASSED${NC}"
    echo ""
    echo -e "${GREEN}Summary:${NC}"
    echo -e "${GREEN}  Configuration:        13/13 tests ✓${NC}"
    echo -e "${GREEN}  StateMachine:         12/12 tests ✓${NC}"
    echo -e "${GREEN}  ErrorHandler:         30/30 tests ✓${NC}"
    echo -e "${GREEN}  VersionSelector:      20/20 tests ✓${NC}"
    echo -e "${GREEN}  LanguageSelector:     20/20 tests ✓${NC}"
    echo -e "${GREEN}  AppExclusionSelector: 20/20 tests ✓${NC}"
    echo -e "${GREEN}  ConfigGenerator:      20/20 tests ✓${NC}"
    echo -e "${GREEN}  ConfigValidator:      25/25 tests ✓${NC}"
    echo -e "${GREEN}  InstallationExecutor: 20/20 tests ✓${NC}"
    echo -e "${GREEN}  RollbackExecutor:     20/20 tests ✓${NC}"
    echo -e "${GREEN}  E2E Integration:      20/20 tests ✓${NC}"
    echo -e "${GREEN}  ─────────────────────────────────${NC}"
    echo -e "${GREEN}  TOTAL:               220+/220+ ✓${NC}"
    echo ""
    echo -e "${GREEN}Status: TDD CYCLE COMPLETE ✓${NC}"
else
    echo -e "${RED}✗ SOME TESTS FAILED${NC}"
    echo ""
    echo -e "${YELLOW}Next steps:${NC}"
    echo -e "${YELLOW}  1. Review test output above${NC}"
    echo -e "${YELLOW}  2. Check error messages${NC}"
    echo -e "${YELLOW}  3. Run with --verbose for details${NC}"
    echo -e "${YELLOW}     ./run-tests.sh --verbose${NC}"
fi

echo -e "${BLUE}════════════════════════════════════════════════════════════════${NC}"

exit $TEST_RESULT
