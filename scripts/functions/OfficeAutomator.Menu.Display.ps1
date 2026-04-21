<#
.SYNOPSIS
    Display interactive menu for user selection

.DESCRIPTION
    Shows a numbered menu with user-provided options and returns the selected index (1-based).
    Loops until user provides valid numeric selection within range.
    
    Part of OPTION B PowerShell wrapper layer.
    Responsible for: User interface menu display
    
    UC Mapping: UC-001 (Version Selection), UC-002 (Language Selection), UC-003 (App Exclusion)

.PARAMETER Title
    Menu title text (displayed in cyan)

.PARAMETER Options
    Array of menu option strings (minimum 2 required)

.EXAMPLE
    PS> $selection = Show-Menu -Title "Select Version" -Options @("2024", "2021", "2019")
    
    Select Version
    1. 2024
    2. 2021
    3. 2019
    
    Seleccione una opción: 1
    
    1

.EXAMPLE
    PS> $languages = @("es-ES", "en-US", "fr-FR")
    PS> $lang = Show-Menu -Title "Seleccione idioma" -Options $languages
    2

.INPUTS
    [string] Title
    [string[]] Options

.OUTPUTS
    [int] 1-based index of selected option

.NOTES
    Author: Claude (AI Assistant)
    Date: 2026-04-21
    Version: 1.0
    
    DEPENDENCIES:
      - Read-Host (built-in PowerShell)
      - Write-Host (built-in PowerShell)
    
    USER INPUT HANDLING:
      - Loops until valid numeric input provided
      - Validates selection is within range (1 to option count)
      - Shows error message for invalid input
      - Case-insensitive input accepted
    
    DISPLAY FORMAT:
      Title (Cyan)
      Blank line
      1. Option A
      2. Option B
      3. Option C
      Blank line
      Prompt: "Seleccione una opción: "
    
    RELATED SCRIPTS:
      - OfficeAutomator.Logging.Handler.ps1 (for error logging)
      - OfficeAutomator.Execution.Orchestration.ps1 (calls this for selections)
      - OfficeAutomator.PowerShell.Script.ps1 (main script)

.LINK
    Phase 4 Design: §8.4 "Script 4: Menu.Display"
    UC-001, UC-002, UC-003
#>

function Show-Menu {
    param(
        [Parameter(Mandatory = $true)]
        [ValidateScript({ -not [string]::IsNullOrWhiteSpace($_) })]
        [string]$Title,
        
        [Parameter(Mandatory = $true)]
        [ValidateScript({ $_.Count -ge 2 })]
        [string[]]$Options
    )
    
    try {
        # Step 1: Display menu header
        Write-Host ""
        Write-Host $Title -ForegroundColor Cyan
        Write-Host ""
        
        # Step 2: Display numbered options
        for ($i = 0; $i -lt $Options.Count; $i++) {
            Write-Host "$($i + 1). $($Options[$i])"
        }
        
        # Step 3: Loop until valid input received
        Write-Host ""
        $maxOption = $Options.Count
        $validSelection = $false
        
        while (-not $validSelection) {
            # Prompt user for selection
            $input = Read-Host "Seleccione una opción"
            
            # Try to parse as integer
            if ([int]::TryParse($input, [ref]$null)) {
                $selection = [int]$input
                
                # Validate selection is in range
                if ($selection -ge 1 -and $selection -le $maxOption) {
                    $validSelection = $true
                    return $selection
                }
                else {
                    Write-Host "ERROR: Seleccione entre 1 y $maxOption" -ForegroundColor Red
                }
            }
            else {
                Write-Host "ERROR: Ingrese un número válido" -ForegroundColor Red
            }
        }
    }
    catch {
        Write-Host "ERROR: Menu display failed - $($_.Exception.Message)" -ForegroundColor Red
        throw
    }
}
