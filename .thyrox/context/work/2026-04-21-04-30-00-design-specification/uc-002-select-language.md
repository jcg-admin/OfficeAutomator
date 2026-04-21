```yml
type: Use Case Design
stage: Stage 7 - DESIGN/SPECIFY
uc_id: UC-002
name: Select Office Language
created_at: 2026-04-21 04:40:00
version: 1.0.0
```

# UC-002: SELECT OFFICE LANGUAGE

---

## Overview

User selects language(s) for Office installation. Supports single language or multiple languages (up to 2 in v1.0.0). Can auto-detect OS language or allow manual selection.

**Complexity:** Medium
**Criticality:** Critical (affects UC-004 validation)
**Failure Mode:** Fail-Fast for invalid language-version combinations

---

## Main Flow

```
1. Display available languages for selected version:
   - es-ES (Español España)
   - en-US (English USA)

2. Display auto-detection option:
   [Detect from Windows language: {current OS language}]

3. User chooses:
   Option A: Auto-detect (use current OS language)
   Option B: Select manually (choose from list)
   Option C: Multiple languages (es-ES + en-US)

4. System validates language selection:
   [Is language compatible with version?]
   - Check against compatibility matrix
   - If NO: Error + retry
   - If YES: Proceed

5. Store languages in $Config.Languages (array)

6. Proceed to UC-003
```

---

## Technical Design

### Function Signature

```powershell
function Select-OfficeLanguage {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [ValidateSet('2024', '2021', '2019')]
        [string]$Version,
        
        [Parameter(Mandatory = $false)]
        [ValidateSet('es-ES', 'en-US')]
        [string[]]$Language,
        
        [Parameter(Mandatory = $false)]
        [switch]$UseOSLanguage
    )
}
```

### Parameters

| Parameter | Type | Validation | Description |
|-----------|------|-----------|-------------|
| `$Version` | string | 2024, 2021, 2019 | From UC-001 (required) |
| `$Language` | string[] | es-ES, en-US | Allow multiple (max 2 in v1.0.0) |
| `$UseOSLanguage` | switch | N/A | Auto-detect from Windows |

### Return Value

```powershell
# Success:
[PSCustomObject]@{
    Languages = @("es-ES", "en-US")
    IsAutoDetected = $false
    Success = $true
}

# Failure:
[PSCustomObject]@{
    Languages = $null
    Success = $false
    Error = "Language not compatible with version"
}
```

---

## Error Scenarios

### Scenario A: Language Not Compatible with Version

**Example:** User selects language not in compatibility matrix

**System Response:**
- `[ERROR] Language en-GB not supported in v1.0.0`
- Display available languages for selected version
- Retry from step 3

---

### Scenario B: Too Many Languages (> 2)

**Example:** User selects 3 or more languages

**System Response:**
- `[ERROR] Maximum 2 languages allowed in v1.0.0`
- Allow user to deselect one language
- Retry from step 3

---

## Validation Rules

### Rule 1: Language Must Be in Whitelist

```powershell
$ValidLanguages = @('es-ES', 'en-US')
if ($Language -notcontains $ValidLanguages) { error }
```

### Rule 2: Language Count ≤ 2

```powershell
if ($Language.Count -gt 2) { error }
```

### Rule 3: Version-Language Compatibility

Check language-compatibility-matrix.md for valid combinations.

---

## Integration Points

### Depends On
- UC-001 result (Version)
- `Get-SupportedLanguages.ps1` (version-aware)
- `Test-LanguageValidity.ps1` (validation)

### Used By
- UC-003: Receives language context
- UC-004: Validates language against apps

---

## User Experience

### Interactive Menu Example

```
╔═══════════════════════════════════════════════════════════╗
║         SELECT LANGUAGE - Office 2024 LTSC                ║
╚═══════════════════════════════════════════════════════════╝

Options:

  1) Detect from Windows (Current: es-ES)
  2) Select manually
  3) Multiple languages (es-ES + en-US)

Select option (1-3): _
```

### Multiple Language Selection

```
╔═══════════════════════════════════════════════════════════╗
║         SELECT LANGUAGES (max 2)                          ║
╚═══════════════════════════════════════════════════════════╝

Available:
  ☐ es-ES (Español España)
  ☐ en-US (English USA)

Select languages (comma-separated): es-ES, en-US
```

---

## Testing Strategy

### Unit Tests: `LanguageSelection.Tests.ps1`

```powershell
# Test: Valid language accepted
Select-OfficeLanguage -Version "2024" -Language "es-ES" | Should -Be $true

# Test: Multiple languages accepted (up to 2)
Select-OfficeLanguage -Version "2024" -Language @("es-ES", "en-US") | Should -Be $true

# Test: 3+ languages rejected
Select-OfficeLanguage -Version "2024" -Language @("es-ES", "en-US", "fr-FR") | Should -Be $false

# Test: Auto-detect works
Select-OfficeLanguage -Version "2024" -UseOSLanguage | Should -Not -BeNullOrEmpty
```

---

**Version:** 1.0.0
**Design Status:** Complete
**Next UC:** UC-003 - Exclude Applications

