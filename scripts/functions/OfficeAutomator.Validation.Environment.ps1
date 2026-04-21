<#
.SYNOPSIS
    Validate PowerShell environment prerequisites for OfficeAutomator

.DESCRIPTION
    Validates that the current environment meets all requirements to run OfficeAutomator.
    Checks for administrator privileges, .NET 8.0+ runtime, and Core DLL availability.
    
    Part of OPTION B PowerShell wrapper layer.
    Responsible for: Environment validation (Phase 1 prerequisites)
    
    UC Mapping: None (infrastructure layer)

.EXAMPLE
    PS> Test-AdminRights
    True
    
    PS> Test-CoreDllExists -DllPath ".\OfficeAutomator.Core.dll"
    True
    
    PS> Test-DotNetRuntime
    True
    
    PS> Test-PrerequisitesMet -DllPath ".\OfficeAutomator.Core.dll"
    True

.NOTES
    Author: Claude (AI Assistant)
    Date: 2026-04-21
    Version: 1.0
    
    DEPENDENCIES:
      - System.Security.Principal (built-in .NET)
      - System.Runtime.InteropServices (built-in .NET)
    
    ERROR CODES:
      - 2001: Administrator privileges required
      - 2001: PowerShell 5.1+ required (caught by validation)
      - 2001: .NET 8.0+ required
      - 2001: Core DLL not found
    
    RELATED SCRIPTS:
      - OfficeAutomator.CoreDll.Loader.ps1 (called after validation succeeds)
      - OfficeAutomator.PowerShell.Script.ps1 (calls this during Phase 1)

.LINK
    Phase 4 Design: §4.2 "C# Object Creation Pattern"
#>

function Test-AdminRights {
    <#
    .SYNOPSIS
        Check if current PowerShell session has administrator privileges
    
    .OUTPUTS
        [bool] $true if admin, $false otherwise
    #>
    
    try {
        $identity = [System.Security.Principal.WindowsIdentity]::GetCurrent()
        $principal = New-Object System.Security.Principal.WindowsPrincipal($identity)
        
        # Check if user is in Administrators group
        $adminRole = [System.Security.Principal.WindowsBuiltInRole]::Administrator
        $result = $principal.IsInRole($adminRole)
        
        return $result
    }
    catch {
        Write-Host "ERROR: Failed to check admin rights - $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

function Test-CoreDllExists {
    <#
    .SYNOPSIS
        Check if OfficeAutomator.Core.dll exists at specified path
    
    .PARAMETER DllPath
        Full path to OfficeAutomator.Core.dll
        Must be non-empty string
    
    .OUTPUTS
        [bool] $true if file exists, $false otherwise
    #>
    
    param(
        [Parameter(Mandatory = $true)]
        [ValidateScript({ -not [string]::IsNullOrWhiteSpace($_) })]
        [string]$DllPath
    )
    
    try {
        $exists = Test-Path $DllPath -PathType Leaf
        return $exists
    }
    catch {
        Write-Host "ERROR: Failed to verify DLL path - $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

function Test-DotNetRuntime {
    <#
    .SYNOPSIS
        Check if .NET 8.0+ runtime is installed
    
    .OUTPUTS
        [bool] $true if .NET 8.0+, $false otherwise
    #>
    
    try {
        $frameworkDesc = [System.Runtime.InteropServices.RuntimeInformation]::FrameworkDescription
        
        # More robust pattern handles ".NET 8", ".NET 9", ".NET Core 8.0", etc.
        if ($frameworkDesc -match "\.NET(?:\s+Core)?\s+(\d+)") {
            $version = [int]$matches[1]
            return $version -ge 8
        }
        
        return $false
    }
    catch {
        Write-Host "ERROR: Failed to detect .NET runtime - $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

function Test-PrerequisitesMet {
    <#
    .SYNOPSIS
        Validate all prerequisites are met for OfficeAutomator
    
    .DESCRIPTION
        Validates that the current environment meets all requirements:
        1. Running as administrator
        2. .NET 8.0+ runtime installed
        3. Core DLL exists at specified path
        
        Reports which prerequisites failed for debugging.
    
    .PARAMETER DllPath
        Full path to OfficeAutomator.Core.dll
        Must be non-empty string
    
    .EXAMPLE
        PS> Test-PrerequisitesMet -DllPath ".\OfficeAutomator.Core.dll"
        ✓ All prerequisites validated
        True
    
    .EXAMPLE
        PS> Test-PrerequisitesMet -DllPath ".\fake.dll"
        ERROR: Core DLL not found at: .\fake.dll (Error Code: 2001)
        False
    
    .INPUTS
        [string] DLL file path
    
    .OUTPUTS
        [bool] $true if all prerequisites met, $false otherwise
    
    .NOTES
        Author: Claude (AI Assistant)
        Date: 2026-04-21
        Version: 1.0
        
        ERROR CODES:
          - 2001: Prerequisites validation failed (admin, .NET, or DLL issue)
        
        RELATED SCRIPTS:
          - OfficeAutomator.CoreDll.Loader.ps1 (called after this succeeds)
          - OfficeAutomator.PowerShell.Script.ps1 (calls this during Phase 1)
    #>
    <#
    .SYNOPSIS
        Validate all prerequisites are met for OfficeAutomator
    
    .PARAMETER DllPath
        Full path to OfficeAutomator.Core.dll
    
    .OUTPUTS
        [bool] $true if all prerequisites met, $false if any prerequisite missing
    
    .DESCRIPTION
        Validates:
        1. Running as administrator
        2. .NET 8.0+ runtime installed
        3. Core DLL exists at specified path
    #>
    
    param(
        [Parameter(Mandatory = $true)]
        [string]$DllPath
    )
    
    try {
        # Check all prerequisites
        $adminRights = Test-AdminRights
        $dllExists = Test-CoreDllExists -DllPath $DllPath
        $dotNetOk = Test-DotNetRuntime
        
        # All must pass
        if ($adminRights -and $dllExists -and $dotNetOk) {
            Write-Host "✓ All prerequisites validated" -ForegroundColor Green
            return $true
        }
        
        # Report which ones failed
        if (-not $adminRights) {
            Write-Host "ERROR: Administrator privileges required (Error Code: 2001)" -ForegroundColor Red
        }
        if (-not $dllExists) {
            Write-Host "ERROR: Core DLL not found at: $DllPath (Error Code: 2001)" -ForegroundColor Red
        }
        if (-not $dotNetOk) {
            Write-Host "ERROR: .NET 8.0+ required (Error Code: 2001)" -ForegroundColor Red
        }
        
        return $false
    }
    catch {
        Write-Host "ERROR: Failed to validate prerequisites - $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}
