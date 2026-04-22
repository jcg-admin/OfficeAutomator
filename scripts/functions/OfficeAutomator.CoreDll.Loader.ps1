<#
.SYNOPSIS
    Load OfficeAutomator.Core.dll and validate required classes

.DESCRIPTION
    Loads the OfficeAutomator.Core assembly into the current PowerShell process
    using .NET reflection. Validates that all required classes exist before returning.
    
    Part of OPTION B PowerShell wrapper layer.
    Responsible for: DLL loading and initialization
    
    UC Mapping: None (infrastructure layer)

.PARAMETER DllPath
    Full path to OfficeAutomator.Core.dll file
    Must be a valid .NET assembly file

.EXAMPLE
    PS> Load-OfficeAutomatorCoreDll -DllPath ".\OfficeAutomator.Core.dll"
    ✓ DLL loaded successfully
    
    True

.EXAMPLE
    PS> Load-OfficeAutomatorCoreDll -DllPath "invalid.dll"
    ERROR: Failed to load DLL - File not found
    
    (Exception thrown)

.INPUTS
    [string] DLL file path

.OUTPUTS
    [bool] $true if successful, throws exception on error

.NOTES
    Author: Claude (AI Assistant)
    Date: 2026-04-21
    Version: 1.0
    
    DEPENDENCIES:
      - System.Reflection.Assembly (built-in .NET)
      - OfficeAutomator.Core.dll must be valid .NET 8.0 assembly
    
    ERROR CODES:
      - 2001: DLL not found
      - 2002: DLL loading failed (invalid .NET assembly)
      - 2003: Required classes not found in DLL
    
    RELATED SCRIPTS:
      - OfficeAutomator.Validation.Environment.ps1 (runs first to validate .NET runtime)
      - OfficeAutomator.PowerShell.Script.ps1 (calls this during Phase 1)
      - OfficeAutomator.Logging.Handler.ps1 (for error logging)

.LINK
    https://github.com/your-repo/OfficeAutomator
    Phase 4 Design: §4.1 "DLL Loading Pattern"
#>

function Load-OfficeAutomatorCoreDll {
    param(
        [Parameter(Mandatory = $false, HelpMessage = "Path to OfficeAutomator.Core.dll")]
        [string]$DllPath
    )
    
    try {
        # Step 0: If DllPath not provided, construct it from script location
        if ([string]::IsNullOrEmpty($DllPath)) {
            $projectRoot = Split-Path -Parent (Split-Path -Parent (Split-Path -Parent $PSScriptRoot))
            $DllPath = Join-Path $projectRoot "src\OfficeAutomator.Core\bin\Release\net8.0\OfficeAutomator.Core.dll"
        }
        
        # Verify DLL exists
        if (-not (Test-Path $DllPath -PathType Leaf)) {
            throw [System.IO.FileNotFoundException]::new("DLL not found: $DllPath (Error Code: 2001)")
        }
        
        # Step 1: Load DLL using .NET reflection
        # This makes all public types available in current PowerShell session
        [System.Reflection.Assembly]::LoadFrom($DllPath) | Out-Null
        
        # Step 2: Verify key classes exist (required for functionality)
        # If these don't exist, something is wrong with the DLL
        $null = [OfficeAutomator.Core.Models.Configuration]
        $null = [OfficeAutomator.Core.Validation.ConfigValidator]
        $null = [OfficeAutomator.Core.Installation.InstallationExecutor]
        $null = [OfficeAutomator.Core.Installation.RollbackExecutor]
        
        # Step 3: All checks passed
        Write-Host "✓ DLL loaded successfully" -ForegroundColor Green
        return $true
    }
    catch [System.IO.FileNotFoundException] {
        # DLL file doesn't exist at specified path
        $errorMsg = "DLL not found: $DllPath (Error Code: 2001)"
        Write-Host "ERROR: $errorMsg" -ForegroundColor Red
        throw [System.IO.FileNotFoundException]::new($errorMsg, $_.Exception)
    }
    catch [System.BadImageFormatException] {
        # File exists but is not a valid .NET assembly
        $errorMsg = "DLL is not a valid .NET assembly: $DllPath (Error Code: 2002)"
        Write-Host "ERROR: $errorMsg" -ForegroundColor Red
        throw [System.BadImageFormatException]::new($errorMsg, $_.Exception)
    }
    catch [System.TypeLoadException] {
        # DLL loaded but required classes not found
        $errorMsg = "Required classes not found in DLL: $DllPath (Error Code: 2003)"
        Write-Host "ERROR: $errorMsg" -ForegroundColor Red
        throw [System.TypeLoadException]::new($errorMsg, $_.Exception)
    }
    catch [System.Exception] {
        # Generic/unexpected error during DLL loading
        $errorMsg = "Unexpected error loading DLL: $($_.Exception.Message)"
        Write-Host "ERROR: $errorMsg" -ForegroundColor Red
        throw
    }
}
