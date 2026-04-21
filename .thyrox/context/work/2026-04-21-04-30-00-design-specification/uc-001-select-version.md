```yml
type: Use Case Design
stage: Stage 7 - DESIGN/SPECIFY
uc_id: UC-001
name: Select Office Version
created_at: 2026-04-21 04:35:00
version: 1.0.0
```

# UC-001: SELECT OFFICE VERSION

---

## Overview

User selects the Office LTSC version to install (2024, 2021, or 2019). This is the first decision point in the installation flow. Selection determines compatibility with subsequent language and application choices.

**Complexity:** Low
**Criticality:** Critical (affects all downstream decisions)
**Failure Mode:** Fail-Fast (invalid selection → error + exit)

---

## Actors and Preconditions

### Primary Actor
- IT Administrator (human user)

### Preconditions
1. PowerShell session is open
2. User has Administrator privileges
3. User has executed `Invoke-OfficeAutomator.ps1`

### Postconditions (Success)
- Version selected and validated
- Version stored in configuration object
- Flow proceeds to UC-002

### Postconditions (Failure)
- Error message displayed
- Version not set
- UC-002 not invoked

---

## Main Flow

```
1. System displays version options:
   - Office 2024 LTSC (Principal, Support until 2026-10-13)
   - Office 2021 LTSC (Secondary, Support until 2026-10-13)
   - Office 2019 LTSC (Tertiary, Support until 2025-10-13)

2. User selects version via menu

3. System validates selection:
   [Is selection valid?]
   - If NO: Display error, return to step 1
   - If YES: Proceed to step 4

4. System confirms selection:
   [Display: "Selected Office {version} LTSC"]

5. Store version in $Config.Version

6. Proceed to UC-002
```

---

## Technical Design

### Function Signature

```powershell
function Select-OfficeVersion {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $false)]
        [ValidateSet('2024', '2021', '2019')]
        [string]$Version
    )
    
    # If $Version provided, skip menu and validate
    # If not provided, display interactive menu
    # Return selected version or $null on error
}
```

### Parameters

| Parameter | Type | Required | Validation | Description |
|-----------|------|----------|-----------|-------------|
| `$Version` | string | No | 2024, 2021, 2019 | Allows scripted input (bypass menu) |

### Return Value

```powershell
# Success:
[PSCustomObject]@{
    Version = "2024"
    DisplayName = "Office 2024 LTSC"
    SupportEndDate = "2026-10-13"
    Success = $true
}

# Failure:
[PSCustomObject]@{
    Version = $null
    Success = $false
    Error = "Invalid selection. Please choose 2024, 2021, or 2019."
}
```

---

## Error Scenarios

### Scenario A: Invalid Input

**User Action:** Selects invalid version (e.g., "2020")

**System Response:**
- Display: `[ERROR] Invalid selection. Options: 2024, 2021, 2019`
- Retry count: Unlimited (user can keep trying)
- Max timeout: None (user controls exit)

**Recovery:** Return to step 1 of main flow

---

### Scenario B: Version Not Available (Future)

**User Action:** Selects version no longer supported

**System Response:**
- Display: `[WARN] Office 2019 support ends 2025-10-13. Consider upgrading to 2024.`
- Allow continuation (just warning, not blocking)

**Recovery:** Proceed to UC-002 with warning logged

---

## Validation Rules

### Rule 1: Version Must Exist

```powershell
$ValidVersions = @('2024', '2021', '2019')
if ($Version -notin $ValidVersions) {
    Write-Error "Version not supported"
    return $null
}
```

### Rule 2: Version Must Have Soporte Vigente

All three versions have active support until at least 2025-10-13.

### Rule 3: Version Determines Language Options

| Version | Compatible Languages |
|---------|----------------------|
| 2024 | es-ES, en-US (+ roadmap) |
| 2021 | es-ES, en-US (+ roadmap) |
| 2019 | es-ES, en-US (+ roadmap) |

---

## Integration Points

### Depends On
- `Get-SupportedVersions.ps1` - Retrieve valid versions

### Used By
- `Select-OfficeLanguage.ps1` (UC-002) - Receives version as context
- `Exclude-OfficeApplications.ps1` (UC-003) - Receives version as context

### Logs
- Entry: `[INFO] UC-001 started: Select Office Version`
- Selection: `[INFO] User selected Office {version} LTSC`
- Exit: `[INFO] UC-001 completed successfully`
- Error: `[ERROR] UC-001 failed: {error message}`

---

## User Experience

### Interactive Menu Example

```
╔═══════════════════════════════════════════════════════════╗
║         SELECT OFFICE VERSION                             ║
╚═══════════════════════════════════════════════════════════╝

Available versions:

  1) Office 2024 LTSC
     Support until: 2026-10-13
     Status: RECOMMENDED

  2) Office 2021 LTSC
     Support until: 2026-10-13
     Status: SECONDARY

  3) Office 2019 LTSC
     Support until: 2025-10-13
     Status: LEGACY

Select version (1-3): _
```

### Success Output

```
[SUCCESS] Office 2024 LTSC selected
Proceeding to language selection...
```

### Error Output

```
[ERROR] Invalid selection. Please select 1, 2, or 3.
[INFO] Please try again.

Select version (1-3): _
```

---

## Exit Criteria for UC-001

- [x] Version selection menu displays correctly
- [x] User input is validated
- [x] Valid selection proceeds to UC-002
- [x] Invalid selection shows error + retry
- [x] Timeout/abort returns error
- [x] Logging captures all actions
- [x] Version is stored in $Config object

---

## Testing Strategy

### Unit Tests: `VersionSelection.Tests.ps1`

```powershell
# Test: Valid input accepted
Invoke-Expression -Command 'Select-OfficeVersion -Version "2024"' | Should -Be $true

# Test: Invalid input rejected
Invoke-Expression -Command 'Select-OfficeVersion -Version "2020"' | Should -Be $false

# Test: Menu displays all options
# (Interactive test - manual verification)

# Test: Default behavior (no parameter)
# (Interactive test - manual verification)
```

### Integration Tests: `Installation.Tests.ps1`

```powershell
# Test: UC-001 → UC-002 flow
$version = Select-OfficeVersion -Version "2024"
Select-OfficeLanguage -Version $version.Version | Should -Not -BeNullOrEmpty
```

---

## Performance Targets

- Menu display: < 500 ms
- Input validation: < 100 ms
- Total UC-001 execution: < 1 second

---

**Version:** 1.0.0
**Design Status:** Complete
**Implementation Status:** Pending (Stage 10)
**Next UC:** UC-002 - Select Office Language

