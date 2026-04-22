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

# Install
echo "Installing .NET SDK 8.0..."
export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=true
export DOTNET_CLI_TELEMETRY_OPTOUT=true

curl -fsSL https://dot.net/v1/dotnet-install.sh | \
    bash -s -- --channel 8.0 --install-dir "$HOME/.dotnet" --skip-user-profile

# Set environment
export DOTNET_ROOT="$HOME/.dotnet"
export PATH="$DOTNET_ROOT:$PATH"

# Verify
dotnet --version
echo "✓ .NET SDK 8.0 installed"
