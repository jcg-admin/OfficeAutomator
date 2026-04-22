#!/bin/bash
set -euo pipefail

echo "Verifying environment..."

# VP-001: Disk space
available_kb=$(df ~ | awk 'NR==2 {print $4}')
if [ "$available_kb" -lt 1048576 ]; then
    echo "ERROR: <1 GB free disk space. Available: $((available_kb/1024)) MB"
    exit 1
fi
echo "✓ Disk space OK"

# VP-002: Network
if ! curl -s --connect-timeout 5 https://api.nuget.org/v3/index.json > /dev/null 2>&1; then
    echo "ERROR: Cannot reach api.nuget.org"
    exit 1
fi
echo "✓ Network connectivity OK"

# VP-003: Bash version
if [ "${BASH_VERSINFO[0]}" -lt 4 ]; then
    echo "ERROR: Bash 4.0+ required. Current: ${BASH_VERSINFO[0]}.${BASH_VERSINFO[1]}"
    exit 1
fi
echo "✓ Bash version OK"

echo "✓ All pre-flight checks passed"
