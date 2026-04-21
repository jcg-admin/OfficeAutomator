```yml
type: Use Case Design
stage: Stage 7 - DESIGN/SPECIFY
uc_id: UC-005
name: Install Office
created_at: 2026-04-21 04:55:00
version: 1.0.0
```

# UC-005: INSTALL OFFICE

---

## Overview

Executes Office installation using validated configuration from UC-004. Implements idempotence: if Office is already installed, verifies compatibility instead of re-installing.

**Complexity:** High (execution, monitoring, idempotence)
**Criticality:** Critical (final installation step)
**Failure Mode:** Fail-Fast with clear error, rollback if partial install

---

## Main Flow

```
1. Verify UC-004 success:
   [Is Config.ValidationPassed == $true?]
   - If NO: Display error, exit
   - If YES: Proceed

2. Check for existing Office installation:
   [Is Office already installed?]
   - If YES: Go to step 3 (Verify mode)
   - If NO: Go to step 5 (Install mode)

3. VERIFY MODE: Check installed version matches config
   [Version == Config.Version?]
   [Languages == Config.Languages?]
   [Excluded apps == Config.ExcludedApps?]
   - If all match: Display success, exit
   - If mismatch: Repair/Update (step 4)

4. REPAIR/UPDATE: Run setup.exe with repair flag
   [setup.exe /configure configuration.xml /repair]
   - Monitor progress
   - Capture output
   - Handle errors

5. INSTALL MODE: Run setup.exe with fresh config
   [setup.exe /configure configuration.xml]
   - Monitor progress
   - Capture output
   - Handle errors

6. Verify post-installation:
   [Is Office installed and running correctly?]
   - If YES: Display success
   - If NO: Display error, capture logs

7. Cleanup:
   - Delete temporary configuration.xml
   - Delete temporary setup.exe
   - Cleanup log files (if requested)

8. Return result to user
```

---

## Technical Design

### Function Signature

```powershell
function Install-Office {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [PSObject]$Config,
        
        [Parameter(Mandatory = $false)]
        [switch]$Force,
        
        [Parameter(Mandatory = $false)]
        [switch]$NoCleanup
    )
    
    # Returns: PSObject with installation result
}
```

### Parameters

| Parameter | Type | Validation | Description |
|-----------|------|-----------|-------------|
| `$Config` | PSObject | Must have ValidationPassed=$true | From UC-004 |
| `$Force` | switch | N/A | Skip existing install check, reinstall anyway |
| `$NoCleanup` | switch | N/A | Keep temp files for debugging |

### Return Value

```powershell
# Success - Fresh Install:
[PSCustomObject]@{
    Mode = "Install"
    Version = "2024"
    Status = "Success"
    InstallDuration = "00:15:30"
    Message = "Office 2024 LTSC installed successfully"
    Success = $true
}

# Success - Already Installed (Idempotent):
[PSCustomObject]@{
    Mode = "Verify"
    Status = "Already Installed"
    Version = "2024"
    Languages = @("es-ES")
    Message = "Office 2024 LTSC already installed with matching configuration"
    Success = $true
}

# Failure:
[PSCustomObject]@{
    Status = "Failed"
    FailedStep = "Download"
    ErrorCode = "0x80070002"
    Message = "Setup.exe execution failed: File not found"
    Success = $false
    LogPath = "C:\Temp\OfficeAutomator-error.log"
}
```

---

## Installation Modes

### Mode A: Fresh Installation

**Condition:** Office not detected on system

**Steps:**
```powershell
1. Verify prerequisites:
   [Is admin?] [Disk space available?] [Network OK?]

2. Execute setup:
   $SetupArgs = '/configure', $Config.ConfigPath
   & $Config.ODTPath $SetupArgs

3. Monitor execution:
   - Track progress (every 5 seconds)
   - Capture stdout/stderr
   - Handle timeout (30 minutes max)

4. Verify installation:
   - Check registry entries
   - Verify installed files
   - Test basic functionality (Open Word, etc.)

5. Return success or error
```

---

### Mode B: Verify (Already Installed)

**Condition:** Office already installed

**Steps:**
```powershell
1. Read installed configuration:
   - Version from registry
   - Languages from registry
   - Installed components list

2. Compare with Config:
   if ($InstalledVersion -eq $Config.Version -and
       $InstalledLanguages -eq $Config.Languages -and
       $InstalledApps -eq $Config.IncludedApps) {
       # Configuration matches → Idempotent success
       return Success
   } else {
       # Configuration mismatch → Repair
       goto Mode C
   }
```

---

### Mode C: Repair/Update

**Condition:** Office installed but configuration mismatch

**Steps:**
```powershell
1. Log mismatch details:
   [INFO] "Installed: 2021, Expected: 2024"
   [INFO] "Repairing installation..."

2. Execute repair:
   $SetupArgs = '/configure', $Config.ConfigPath, '/repair'
   & $Config.ODTPath $SetupArgs

3. Monitor and verify (same as Mode A)

4. Return success or error
```

---

## Idempotence Implementation

### Idempotence Check Function

```powershell
function Test-OfficeInstallation {
    [CmdletBinding()]
    param([PSObject]$Config)
    
    # Check registry for Office installation
    $OfficeKey = "HKLM:\SOFTWARE\Microsoft\Office\ClickToRun\Configuration"
    if (Test-Path $OfficeKey) {
        $Installed = @{
            Version = (Get-ItemProperty $OfficeKey).Version
            # Parse version from registry
        }
        
        # Compare with Config
        if ($Installed.Version -eq $Config.Version) {
            return $true  # Already installed, match
        } else {
            return $false # Installed but mismatch
        }
    }
    return $false  # Not installed
}
```

### Idempotence Verification

```
CALL 1: Install-Office -Config $Config
  └─ Office not installed → Fresh install → Success

CALL 2: Install-Office -Config $Config (same config)
  └─ Office already installed
     └─ Version matches, languages match
        └─ Return success (already done)

CALL 3: Install-Office -Config $Config (different version)
  └─ Office already installed
     └─ Version MISMATCH
        └─ Repair with new version
           └─ Success

Result: Idempotent - running twice = running once
```

---

## Error Handling

### Error Categories

| Category | Examples | Action |
|----------|----------|--------|
| [BLOQUEADOR] | UC-004 failed, Admin check failed, Disk full | Fail-Fast, no installation |
| [CRITICO] | setup.exe crash, Registry corruption | Fail-Fast, rollback attempt |
| [RECUPERABLE] | Specific component installation failed | Continue with warning |

### Specific Error Codes

| Code | Meaning | Action |
|------|---------|--------|
| 0x80070002 | File not found | Check file integrity |
| 0x80070005 | Access denied | Check admin privileges |
| 0x800F0922 | System does not meet requirements | Check OS version |
| 0x80070643 | Installation failed | Check logs for details |

---

## Execution Flow

### Pre-Installation Checks

```powershell
# 1. Validate UC-004 Success
if (-not $Config.ValidationPassed) {
    Write-Error "UC-004 validation not passed"
    return $false
}

# 2. Check Administrator Privileges
if (-not (Test-Administrator)) {
    Write-Error "Administrator privileges required"
    return $false
}

# 3. Check Disk Space
$RequiredSpace = 3GB
$AvailableSpace = (Get-Volume C).SizeRemaining
if ($AvailableSpace -lt $RequiredSpace) {
    Write-Error "Insufficient disk space"
    return $false
}

# 4. Verify ODT and Config Files Exist
if (-not (Test-Path $Config.ODTPath)) {
    Write-Error "ODT file not found"
    return $false
}

if (-not (Test-Path $Config.ConfigPath)) {
    Write-Error "Configuration.xml not found"
    return $false
}
```

### Installation Execution

```powershell
# Start timer
$StartTime = Get-Date

# Check for existing installation
if (Test-OfficeInstallation $Config) {
    Write-OfficeLog "Office already installed, verifying..."
    # Mode B: Verify
} else {
    Write-OfficeLog "Starting Office installation..."
    # Mode A: Fresh Install
}

# Execute setup
$SetupProcess = Start-Process -FilePath $Config.ODTPath `
                             -ArgumentList '/configure', $Config.ConfigPath `
                             -RedirectStandardOutput $LogFile `
                             -PassThru `
                             -NoNewWindow

# Monitor Progress
while (-not $SetupProcess.HasExited) {
    Start-Sleep -Seconds 5
    Write-OfficeLog "Installation in progress..."
}

# Capture Exit Code
$ExitCode = $SetupProcess.ExitCode
$Duration = (Get-Date) - $StartTime

if ($ExitCode -eq 0) {
    Write-OfficeLog "Installation completed successfully"
    return Success
} else {
    Write-OfficeLog "Installation failed with exit code: $ExitCode"
    return Error
}
```

### Post-Installation Verification

```powershell
# Verify files are present
$WordPath = "C:\Program Files\Microsoft Office\Office16\WINWORD.EXE"
if (Test-Path $WordPath) {
    Write-OfficeLog "Word executable found ✓"
}

# Verify registry entries
$OfficeKey = "HKLM:\SOFTWARE\Microsoft\Office\ClickToRun\Configuration"
if (Test-Path $OfficeKey) {
    Write-OfficeLog "Office registry entries found ✓"
}

# Test basic functionality
try {
    $Word = New-Object -ComObject Word.Application
    $Word.Quit()
    Write-OfficeLog "Word instantiation test passed ✓"
} catch {
    Write-OfficeLog "Word instantiation test failed: $($_.Message)"
}
```

---

## Cleanup Strategy

```powershell
if (-not $NoCleanup) {
    # Remove temporary files
    Remove-Item $Config.ConfigPath -Force -ErrorAction SilentlyContinue
    Remove-Item $Config.ODTPath -Force -ErrorAction SilentlyContinue
    
    # Archive main log (keep for support)
    Copy-Item $LogFile -Destination "$LogArchive\OfficeInstall-$(Get-Date -Format 'yyyyMMdd-HHmmss').log"
    
    # Remove temp logs
    Remove-Item $LogFile -Force -ErrorAction SilentlyContinue
}
```

---

## Integration with Configuration.xml

### Sample Configuration.xml Structure

```xml
<Configuration>
  <Add OfficeClientEdition="64" Version="2024">
    <Product ID="ProPlus2024Volume">
      <Language ID="es-es" />
      <Language ID="en-us" />
      <ExcludeApp ID="Teams" />
      <ExcludeApp ID="OneDrive" />
    </Product>
  </Add>
  <Updates Enabled="True" UpdatePath="..." />
  <Display Level="None" AcceptEULA="True" />
  <Logging Path="%TEMP%" />
</Configuration>
```

---

## Monitoring and Logging

### Installation Log Format

```
[2026-04-21 14:30:45] [INFO] UC-005 started: Install Office
[2026-04-21 14:30:46] [INFO] Admin check passed ✓
[2026-04-21 14:30:47] [INFO] Disk space check passed (150 GB available) ✓
[2026-04-21 14:30:48] [INFO] Office not currently installed
[2026-04-21 14:30:49] [INFO] Starting setup.exe /configure configuration.xml
[2026-04-21 14:30:50] [INFO] Installation progress: 10%
[2026-04-21 14:30:55] [INFO] Installation progress: 20%
...
[2026-04-21 14:45:20] [INFO] Installation completed with exit code 0
[2026-04-21 14:45:21] [INFO] Post-installation verification: Word test passed ✓
[2026-04-21 14:45:22] [SUCCESS] Office 2024 LTSC installation successful
[2026-04-21 14:45:23] [INFO] UC-005 completed successfully
```

---

## Testing Strategy

### Unit Tests: `Installation.Tests.ps1`

```powershell
# Test: Pre-installation checks
Install-Office -Config $TestConfig | Should -Detect "Admin check failed" # If not admin

# Test: Idempotence
Install-Office -Config $TestConfig  # First run
$Result1 = Install-Office -Config $TestConfig  # Second run
$Result1.Status | Should -Be "Already Installed"

# Test: Error handling
Install-Office -Config $BadConfig | Should -HaveProperty "Success" -Value $false
```

---

**Version:** 1.0.0
**Design Status:** Complete
**Implementation Ready:** YES
**Next Phase:** Stage 10 IMPLEMENT

