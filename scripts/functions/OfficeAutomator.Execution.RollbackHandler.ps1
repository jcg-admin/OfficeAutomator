<#
.SYNOPSIS
    Handle rollback operations for failed Office installations

.DESCRIPTION
    Executes rollback procedures when installation fails, using RollbackExecutor C# class.
    Ensures clean recovery by removing partially installed files and configurations.
    
    Part of OPTION B PowerShell wrapper layer.
    Responsible for: Rollback orchestration (UC-005 part 2)
    
    UC Mapping: UC-005 (Installation & Rollback)

.NOTES
    Author: Claude (AI Assistant)
    Date: 2026-04-21
    Version: 1.0
    
    DEPENDENCIES:
      - OfficeAutomator.Core.Installation.RollbackExecutor (C# class)
      - Write-LogEntry function (from Logging.Handler)
    
    ERROR CODES:
      - 1006: Rollback failed (limpieza incompleta)
    
    RELATED SCRIPTS:
      - OfficeAutomator.Execution.Orchestration.ps1 (calls this on failure)
      - OfficeAutomator.Logging.Handler.ps1 (for logging)
      - OfficeAutomator.PowerShell.Script.ps1 (main script)

.LINK
    Phase 4 Design: §8.6 "Script 6: Execution.RollbackHandler"
    UC-005
#>

function Invoke-RollbackOnFailure {
    <#
    .SYNOPSIS
        Execute rollback procedures after installation failure
    
    .DESCRIPTION
        Uses RollbackExecutor to clean up and remove partially installed files.
        Ensures the system is returned to a known good state.
    
    .PARAMETER Configuration
        Configuration object used during failed installation
    
    .PARAMETER LogPath
        Full path to log file for rollback operation logging
    
    .OUTPUTS
        [bool] $true if rollback completed, $false if rollback failed
    
    .NOTES
        Implements UC-005 (Rollback procedure)
        Called when Invoke-InstallationStep fails
        All operations are logged
    
    .EXAMPLE
        PS> $result = Invoke-RollbackOnFailure -Configuration $config -LogPath "$env:TEMP\install.log"
        PS> $result
        True
    #>
    
    param(
        [Parameter(Mandatory = $true)]
        $Configuration,
        
        [Parameter(Mandatory = $true)]
        [ValidateScript({ -not [string]::IsNullOrWhiteSpace($_) })]
        [string]$LogPath
    )
    
    try {
        Write-LogEntry "Starting rollback..." -Level "WARNING" -LogPath $LogPath
        
        # Create RollbackExecutor and execute with Configuration
        $rollback = [OfficeAutomator.Core.Installation.RollbackExecutor]::new()
        $result = $rollback.Execute($Configuration)
        
        if ($result) {
            Write-LogEntry "Rollback completed successfully" -Level "SUCCESS" -LogPath $LogPath
            return $true
        }
        else {
            Write-LogEntry "Rollback failed: cleanup incomplete (Error Code: 1006)" -Level "ERROR" -LogPath $LogPath
            return $false
        }
    }
    catch {
        Write-LogEntry "Rollback error: $($_.Exception.Message) (Error Code: 1006)" -Level "ERROR" -LogPath $LogPath
        return $false
    }
}
