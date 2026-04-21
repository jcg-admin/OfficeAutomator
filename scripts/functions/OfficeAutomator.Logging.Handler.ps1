<#
.SYNOPSIS
    Centralized logging handler for OfficeAutomator

.DESCRIPTION
    Writes log entries to file and console with automatic timestamp, level, and color formatting.
    Appends to existing log files instead of overwriting.
    
    Part of OPTION B PowerShell wrapper layer.
    Responsible for: Centralized logging (all phases)
    
    UC Mapping: None (infrastructure layer)

.PARAMETER Message
    Log message text to write

.PARAMETER Level
    Log level: INFO (cyan), SUCCESS (green), WARNING (yellow), ERROR (red)
    Default: INFO

.PARAMETER LogPath
    Full path to log file (created if doesn't exist)

.EXAMPLE
    PS> Write-LogEntry -Message "Starting OfficeAutomator" -Level "INFO" -LogPath "$env:TEMP\OfficeAutomator.log"
    [2026-04-21 10:15:30] [INFO] Starting OfficeAutomator

.EXAMPLE
    PS> Write-LogEntry -Message "Installation complete" -Level "SUCCESS" -LogPath "$env:TEMP\OfficeAutomator.log"
    [2026-04-21 10:15:45] [SUCCESS] Installation complete

.NOTES
    Author: Claude (AI Assistant)
    Date: 2026-04-21
    Version: 1.0
    
    DEPENDENCIES:
      - System.IO (built-in .NET)
      - Write-Host (built-in PowerShell)
    
    LOG LEVELS:
      - INFO (default): Informational messages (Cyan)
      - SUCCESS: Successful operations (Green)
      - WARNING: Caution/warning messages (Yellow)
      - ERROR: Error conditions (Red)
    
    FILE HANDLING:
      - Creates parent directory if doesn't exist
      - Appends to file (doesn't overwrite)
      - Creates file if doesn't exist
    
    RELATED SCRIPTS:
      - OfficeAutomator.Menu.Display.ps1 (uses for menu output)
      - OfficeAutomator.Execution.Orchestration.ps1 (uses throughout)
      - OfficeAutomator.PowerShell.Script.ps1 (centralizes all logging)

.LINK
    Phase 4 Design: §6 "Logging Architecture"
#>

function Write-LogEntry {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Message,
        
        [Parameter(Mandatory = $false)]
        [ValidateSet("INFO", "SUCCESS", "WARNING", "ERROR")]
        [string]$Level = "INFO",
        
        [Parameter(Mandatory = $true)]
        [ValidateScript({ -not [string]::IsNullOrWhiteSpace($_) })]
        [string]$LogPath
    )
    
    try {
        # Step 1: Get current timestamp in format [YYYY-MM-DD HH:MM:SS]
        $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
        
        # Step 2: Build log message string
        $logMessage = "[$timestamp] [$Level] $Message"
        
        # Step 3: Ensure parent directory exists
        $parentDir = Split-Path -Parent $LogPath
        if (-not (Test-Path $parentDir)) {
            New-Item -ItemType Directory -Path $parentDir -Force | Out-Null
        }
        
        # Step 4: Append to log file
        Add-Content -Path $LogPath -Value $logMessage -ErrorAction Stop
        
        # Step 5: Write to console with appropriate color
        $color = switch ($Level) {
            "INFO"    { "Cyan" }
            "SUCCESS" { "Green" }
            "WARNING" { "Yellow" }
            "ERROR"   { "Red" }
            default   { "White" }
        }
        
        Write-Host $logMessage -ForegroundColor $color
    }
    catch {
        Write-Host "ERROR: Failed to write log entry - $($_.Exception.Message)" -ForegroundColor Red
        throw
    }
}
