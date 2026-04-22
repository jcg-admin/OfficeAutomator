# Contributing to OfficeAutomator

## Quick Start

```bash
make setup    # Install .NET SDK 8.0
make test     # Run all tests
```

## Prerequisites

- **OS:** Ubuntu 20.04+ (or macOS 10.14+)
- **Bash:** 4.0 or higher
- **Disk:** 1 GB free space
- **Network:** Internet connection (for NuGet packages)

## What `make setup` Does

1. Verifies disk space (>1 GB)
2. Checks network connectivity
3. Checks Bash version (4.0+)
4. Downloads and installs .NET SDK 8.0 to ~/.dotnet
5. Sets DOTNET_ROOT and PATH environment variables
6. Verifies installation

**Result:** .NET 8.0 ready for development.

## Running Tests

```bash
make test
```

**Expected output:**
```
Total tests: 220+
Passed: 220+
Failed: 0
Duration: ~2-3 minutes
```

## Troubleshooting

### "dotnet: command not found"
```bash
export PATH=$HOME/.dotnet:$PATH
which dotnet
```

### "Wrong .NET version (7.0 instead of 8.0)"
```bash
rm -rf ~/.dotnet
make setup   # Re-install
```

### "Cannot reach nuget.org"
```bash
# Check network
ping api.nuget.org -c 1

# Try restore with cache reset
cd src/OfficeAutomator.Core
dotnet restore --no-cache
```

### "Disk space exhausted"
```bash
df -h ~
# Clean NuGet cache (safe)
rm -rf ~/.nuget/packages
```

## For macOS Users

macOS ships with Bash 3.2 (very old). Install Bash 4.0+:

```bash
brew install bash
chsh -s /usr/local/bin/bash   # Set as default shell
```

Then run `make setup`.

## Development Workflow

1. Clone repo: `git clone ...`
2. Setup: `make setup`
3. Develop: Edit `src/OfficeAutomator.Core/*.cs`
4. Test: `make test`
5. Commit: `git commit -m "..."`

## Additional Targets

```bash
make clean    # Remove build artifacts (bin/, obj/)
make help     # Show available targets
```
