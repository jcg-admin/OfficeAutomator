#!/bin/bash
set -euo pipefail

echo "Checking .NET 8.0 installation..."

# Idempotent check
if command -v dotnet &> /dev/null; then
    current_version=$(dotnet --version 2>/dev/null || echo "")
    if [[ "$current_version" == 8.0.* ]]; then
        echo "✓ .NET 8.0 already installed: $current_version"
        exit 0
    fi
fi

# Install from Microsoft Visual Studio download server (stable, verified)
# See: docs/DOTNET_SDK_INSTALLATION_NOTES.md for research and rationale
echo "Installing .NET SDK 8.0..."
export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=true
export DOTNET_CLI_TELEMETRY_OPTOUT=true

DOTNET_VERSION="8.0.110"
DOTNET_FILE="dotnet-sdk-${DOTNET_VERSION}-linux-x64.tar.gz"
DOTNET_URL="https://download.visualstudio.microsoft.com/download/pr/9d4db360-5016-4be5-9783-cbf515a7d011/17e0019da97f0f57548a2d7a53edcf28/${DOTNET_FILE}"
DOTNET_ROOT="$HOME/.dotnet"
TEMP_FILE=$(mktemp)

trap "rm -f $TEMP_FILE" EXIT

# Download tarball
echo "Downloading .NET SDK ${DOTNET_VERSION}..."
curl -fsSL -o "$TEMP_FILE" "$DOTNET_URL"

# Create directory
mkdir -p "$DOTNET_ROOT"

# Extract
echo "Extracting .NET SDK..."
tar -xzf "$TEMP_FILE" -C "$DOTNET_ROOT"

# Set environment
export DOTNET_ROOT="$DOTNET_ROOT"
export PATH="$DOTNET_ROOT:$PATH"

# Verify
dotnet --version
echo "✓ .NET SDK 8.0 installed"
