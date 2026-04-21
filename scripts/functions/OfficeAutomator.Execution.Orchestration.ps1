<#
.SYNOPSIS
    Orchestrate the Office installation workflow

.DESCRIPTION
    Coordinates the overall installation workflow by calling validation and installation steps
    in sequence, with progress bar display. Implements UC-004 (Validation) and UC-005 (Installation).
    
    Part of OPTION B PowerShell wrapper layer.
    Responsible for: Orchestration of installation phases
    
    UC Mapping: UC-004 (Validation), UC-005 (Installation & Rollback)

.NOTES
    Author: Claude (AI Assistant)
    Date: 2026-04-21
    Version: 1.0
    
    RELATED SCRIPTS:
      - OfficeAutomator.Validation.Environment.ps1 (prerequisite check)
      - OfficeAutomator.Logging.Handler.ps1 (logging throughout)
      - OfficeAutomator.Execution.RollbackHandler.ps1 (called on failure)
      - OfficeAutomator.PowerShell.Script.ps1 (main script)

.LINK
    Phase 4 Design: §8.5 "Script 5: Execution.Orchestration"
    UC-004, UC-005
#>

function Invoke-OfficeInstallation {
    <#
    .SYNOPSIS
        Execute the complete Office installation workflow
    
    .PARAMETER Configuration
        Configuration object containing version, language, excluded apps
    
    .PARAMETER LogPath
        Full path to log file for operation logging
    
    .OUTPUTS
        [bool] $true if successful, $false if validation/installation failed
    #>
    
    param(
        [Parameter(Mandatory = $true)]
        $Configuration,
        
        [Parameter(Mandatory = $true)]
        [ValidateScript({ -not [string]::IsNullOrWhiteSpace($_) })]
        [string]$LogPath
    )
    
    try {
        # Phase 1: Validation
        if (-not (Invoke-ValidationStep -Configuration $Configuration)) {
            Write-LogEntry "Installation aborted: validation failed" -Level "WARNING" -LogPath $LogPath
            return $false
        }
        
        # Phase 2: Show progress
        Show-ProgressBar -Percent 30 "Downloading Office..."
        
        # Phase 3: Installation
        if (-not (Invoke-InstallationStep -Configuration $Configuration)) {
            Write-LogEntry "Installation failed" -Level "ERROR" -LogPath $LogPath
            return $false
        }
        
        # Phase 4: Complete
        Show-ProgressBar -Percent 100 "Completed!"
        Write-LogEntry "Installation completed successfully" -Level "SUCCESS" -LogPath $LogPath
        return $true
    }
    catch {
        Write-LogEntry "Installation error: $($_.Exception.Message)" -Level "ERROR" -LogPath $LogPath
        return $false
    }
}

function Invoke-ValidationStep {
    <#
    .SYNOPSIS
        Execute configuration validation (UC-004)
    
    .DESCRIPTION
        Validates the provided configuration using ConfigValidator C# class.
        Checks all required settings and returns validation result.
    
    .PARAMETER Configuration
        Configuration object containing version, language, excluded apps
    
    .PARAMETER LogPath
        Full path to log file for operation logging
    
    .OUTPUTS
        [bool] $true if validation passed, $false otherwise
    
    .NOTES
        Implements UC-004 (Configuration Validation)
        Creates and executes ConfigValidator.Execute() method
        All errors are logged and function returns false on failure
    
    .EXAMPLE
        PS> $result = Invoke-ValidationStep -Configuration $config -LogPath "$env:TEMP\install.log"
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
        Write-LogEntry "Starting validation..." -Level "INFO" -LogPath $LogPath
        
        # Create validator and execute
        $validator = New-Object OfficeAutomator.Core.Validation.ConfigValidator
        $result = $validator.Execute($Configuration)
        
        if ($result) {
            Write-LogEntry "Configuration validation passed" -Level "SUCCESS" -LogPath $LogPath
            return $true
        }
        else {
            Write-LogEntry "Configuration validation failed" -Level "ERROR" -LogPath $LogPath
            return $false
        }
    }
    catch {
        Write-LogEntry "Validation error: $($_.Exception.Message)" -Level "ERROR" -LogPath $LogPath
        return $false
    }
}

function Invoke-InstallationStep {
    <#
    .SYNOPSIS
        Execute Office installation (UC-005)
    
    .DESCRIPTION
        Executes the Office installation using InstallationExecutor C# class.
        Orchestrates the actual installation process and returns result.
    
    .PARAMETER Configuration
        Configuration object with installation settings
    
    .PARAMETER LogPath
        Full path to log file for operation logging
    
    .OUTPUTS
        [bool] $true if installation successful, $false otherwise
    
    .NOTES
        Implements UC-005 (Installation & Rollback)
        Creates and executes InstallationExecutor.Execute() method
        All errors are logged
        
        On failure, caller should invoke RollbackExecutor for cleanup
    
    .EXAMPLE
        PS> $result = Invoke-InstallationStep -Configuration $config -LogPath "$env:TEMP\install.log"
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
        Write-LogEntry "Starting installation..." -Level "INFO" -LogPath $LogPath
        
        # Create executor and execute
        $executor = New-Object OfficeAutomator.Core.Installation.InstallationExecutor
        $result = $executor.Execute($Configuration)
        
        if ($result) {
            Write-LogEntry "Installation completed" -Level "SUCCESS" -LogPath $LogPath
            return $true
        }
        else {
            Write-LogEntry "Installation failed" -Level "ERROR" -LogPath $LogPath
            return $false
        }
    }
    catch {
        Write-LogEntry "Installation error: $($_.Exception.Message)" -Level "ERROR" -LogPath $LogPath
        return $false
    }
}

function Show-ProgressBar {
    <#
    .SYNOPSIS
        Display progress bar with percentage and message
    
    .PARAMETER Percent
        Progress percentage (0-100)
    
    .PARAMETER Message
        Status message to display
    
    .EXAMPLE
        PS> Show-ProgressBar -Percent 50 "Downloading Office..."
        [█████░░░░░] 50% - Downloading Office...
    #>
    
    param(
        [Parameter(Mandatory = $true)]
        [ValidateRange(0, 100)]
        [int]$Percent,
        
        [Parameter(Mandatory = $true)]
        [string]$Message
    )
    
    try {
        # Build progress bar visualization
        $barLength = 10
        $filledLength = [math]::Round(($Percent / 100) * $barLength)
        $emptyLength = $barLength - $filledLength
        
        $filledBar = "█" * $filledLength
        $emptyBar = "░" * $emptyLength
        $progressBar = "[$filledBar$emptyBar]"
        
        # Build and display message
        $displayMessage = "$progressBar $Percent% - $Message"
        Write-Host $displayMessage -ForegroundColor Cyan
    }
    catch {
        Write-Host "ERROR: Progress bar display failed - $($_.Exception.Message)" -ForegroundColor Red
    }
}
