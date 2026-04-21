```yml
type: Use Case Design
stage: Stage 7 - DESIGN/SPECIFY
uc_id: UC-003
name: Exclude Applications
created_at: 2026-04-21 04:45:00
version: 1.0.0
```

# UC-003: EXCLUDE APPLICATIONS

---

## Overview

User selects which Office applications to exclude from installation. Each version has different available applications. Defaults vary by version (e.g., Teams excluded by default in 2024).

**Complexity:** Medium
**Criticality:** High (affects installation footprint)
**Failure Mode:** Fail-Fast for version mismatches, tolerant for individual exclusions

---

## Main Flow

```
1. Display available applications for selected version:
   - Word, Excel, PowerPoint, Outlook, Access, Publisher, OneNote
   - Teams (2024, 2021), Groove (2021, 2019), Lync (2019)
   - OneDrive, Bing

2. Display defaults for selected version:
   [Teams: EXCLUDED by default]
   [OneDrive: EXCLUDED by default]
   [Other apps: INCLUDED by default]

3. User selects applications to exclude:
   Option A: Accept defaults
   Option B: Customize (checkbox UI for each app)
   Option C: Select all / Deselect all

4. System validates app-version compatibility:
   [Can {version} exclude {app}?]
   - If NO: Display error, retry
   - If YES: Proceed

5. Store exclusions in $Config.ExcludedApps (array)

6. Proceed to UC-004 (Validation)
```

---

## Technical Design

### Function Signature

```powershell
function Exclude-OfficeApplications {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [ValidateSet('2024', '2021', '2019')]
        [string]$Version,
        
        [Parameter(Mandatory = $false)]
        [ValidateSet('Teams', 'OneDrive', 'Groove', 'Lync', 'Bing', 'Word', 'Excel', 'PowerPoint', 'Outlook', 'Access', 'Publisher', 'OneNote')]
        [string[]]$ExcludeApps,
        
        [Parameter(Mandatory = $false)]
        [switch]$UseDefaults
    )
}
```

### Parameters

| Parameter | Type | Validation | Description |
|-----------|------|-----------|-------------|
| `$Version` | string | 2024, 2021, 2019 | From UC-001 (required) |
| `$ExcludeApps` | string[] | App whitelist | Applications to exclude |
| `$UseDefaults` | switch | N/A | Skip UI, use version defaults |

### Return Value

```powershell
# Success:
[PSCustomObject]@{
    ExcludedApps = @("Teams", "OneDrive")
    IncludedApps = @("Word", "Excel", "PowerPoint", "Outlook", "Access", "Publisher", "OneNote")
    Success = $true
}

# Failure:
[PSCustomObject]@{
    ExcludedApps = $null
    Success = $false
    Error = "Application not available in version 2024"
}
```

---

## Application Support Matrix

### Office 2024 LTSC

| Application | Status | Default | Excludable |
|-------------|--------|---------|-----------|
| Word | Available | Include | ✓ |
| Excel | Available | Include | ✓ |
| PowerPoint | Available | Include | ✓ |
| Outlook | Available | Include | ✓ |
| Access | Available | Include | ✓ |
| Publisher | Available | Include | ✓ |
| OneNote | Available | Include | ✓ |
| Teams | Available | **EXCLUDE** | ✓ |
| OneDrive | Available | **EXCLUDE** | ✓ |
| Bing | Available | Include | ✓ |
| Groove | N/A | N/A | ✗ |
| Lync | N/A | N/A | ✗ |

### Office 2021 LTSC

| Application | Status | Default | Excludable |
|-------------|--------|---------|-----------|
| Word | Available | Include | ✓ |
| Excel | Available | Include | ✓ |
| PowerPoint | Available | Include | ✓ |
| Outlook | Available | Include | ✓ |
| Access | Available | Include | ✓ |
| Publisher | Available | Include | ✓ |
| OneNote | Available | Include | ✓ |
| Teams | Available | **EXCLUDE** | ✓ |
| OneDrive | Available | **EXCLUDE** | ✓ |
| Bing | Available | Include | ✓ |
| Groove | Available | Include | ✓ |
| Lync | N/A | N/A | ✗ |

### Office 2019 LTSC

| Application | Status | Default | Excludable |
|-------------|--------|---------|-----------|
| Word | Available | Include | ✓ |
| Excel | Available | Include | ✓ |
| PowerPoint | Available | Include | ✓ |
| Outlook | Available | Include | ✓ |
| Access | Available | Include | ✓ |
| Publisher | Available | Include | ✓ |
| OneNote | Available | Include | ✓ |
| Teams | N/A | N/A | ✗ |
| OneDrive | Available | **EXCLUDE** | ✓ |
| Bing | N/A | N/A | ✗ |
| Groove | Available | Include | ✓ |
| Lync | Available | Include | ✓ |

---

## Defaults by Version

```powershell
# Office 2024
$Defaults_2024 = @{
    ExcludeApps = @("Teams", "OneDrive")
    IncludeApps = @("Word", "Excel", "PowerPoint", "Outlook", "Access", "Publisher", "OneNote", "Bing", "Groove")
}

# Office 2021
$Defaults_2021 = @{
    ExcludeApps = @("Teams", "OneDrive")
    IncludeApps = @("Word", "Excel", "PowerPoint", "Outlook", "Access", "Publisher", "OneNote", "Bing", "Groove", "Lync")
}

# Office 2019
$Defaults_2019 = @{
    ExcludeApps = @("OneDrive")
    IncludeApps = @("Word", "Excel", "PowerPoint", "Outlook", "Access", "Publisher", "OneNote", "Groove", "Lync")
}
```

---

## Error Scenarios

### Scenario A: App Not Available in Version

**Example:** User tries to exclude "Lync" from 2024

**System Response:**
```
[ERROR] Lync is not available in Office 2024 LTSC
[INFO] Available apps: Word, Excel, PowerPoint, Outlook, Access, Publisher, OneNote, Teams, OneDrive, Bing, Groove
```

**Recovery:** Show available apps for version, allow retry

---

### Scenario B: Core Application Exclusion

**Example:** User excludes Word (not recommended)

**System Response:**
```
[WARN] Word is a core application. Excluding it may impact functionality.
[INFO] Continue? (Y/N): _
```

**Recovery:** Allow continuation with warning, proceed to UC-004

---

## Validation Rules

### Rule 1: App Must Exist in Version

```powershell
$AvailableApps = Get-AvailableAppsForVersion -Version $Version
foreach ($app in $ExcludeApps) {
    if ($app -notin $AvailableApps) {
        Write-Error "App not available in version"
    }
}
```

### Rule 2: Core Applications (Word, Excel, PowerPoint, Outlook) - Warning Only

```powershell
$CoreApps = @("Word", "Excel", "PowerPoint", "Outlook")
if ($ExcludeApps | Where-Object { $_ -in $CoreApps }) {
    Write-Warning "Core application excluded"
}
```

### Rule 3: Cannot Exclude Non-Existent Apps

```powershell
if ("Teams" -in $ExcludeApps -and "2019" -eq $Version) {
    Write-Error "Teams not available in 2019"
}
```

---

## Integration Points

### Depends On
- UC-001 result (Version)
- UC-002 result (Languages)
- `Get-AvailableAppsForVersion.ps1` (version-aware app list)

### Used By
- UC-004: Validates app-language compatibility
- UC-005: Builds configuration.xml with exclusions

### Logs
- `[INFO] UC-003 started: Exclude Applications`
- `[INFO] Excluded apps: {list}`
- `[WARN] Core application excluded: {app}`
- `[INFO] UC-003 completed successfully`

---

## User Experience

### Interactive Checkbox UI

```
╔═══════════════════════════════════════════════════════════╗
║    SELECT APPLICATIONS TO EXCLUDE - Office 2024 LTSC       ║
╚═══════════════════════════════════════════════════════════╝

[✓] Teams            (Recommended to exclude)
[✓] OneDrive         (Recommended to exclude)
[ ] Word
[ ] Excel
[ ] PowerPoint
[ ] Outlook
[ ] Access
[ ] Publisher
[ ] OneNote
[ ] Bing
[ ] Groove

Apply Defaults? [Y] Customize? [N]: Y
```

### Success Output

```
[SUCCESS] Applications configured
Excluded: Teams, OneDrive
Included: Word, Excel, PowerPoint, Outlook, Access, Publisher, OneNote, Bing
Proceeding to validation...
```

---

## Configuration Object Update

```powershell
$Config.ExcludedApps = @("Teams", "OneDrive")

# Will be used in UC-005:
# setup.exe /configure configuration.xml
# Where configuration.xml contains:
# <ExcludeApp ID="Teams" />
# <ExcludeApp ID="OneDrive" />
```

---

## Testing Strategy

### Unit Tests: `ApplicationSelection.Tests.ps1`

```powershell
# Test: Valid exclusion accepted
Exclude-OfficeApplications -Version "2024" -ExcludeApps "Teams" | Should -Be $true

# Test: Invalid app rejected
Exclude-OfficeApplications -Version "2024" -ExcludeApps "Lync" | Should -Be $false

# Test: Defaults applied correctly
Exclude-OfficeApplications -Version "2024" -UseDefaults | Should -HaveProperty ExcludedApps

# Test: Multiple exclusions
Exclude-OfficeApplications -Version "2024" -ExcludeApps @("Teams", "OneDrive") | Should -Be $true
```

---

**Version:** 1.0.0
**Design Status:** Complete
**Next UC:** UC-004 - Validate Configuration (CRITICAL)

