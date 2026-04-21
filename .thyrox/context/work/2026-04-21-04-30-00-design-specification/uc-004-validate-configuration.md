```yml
type: Use Case Design
stage: Stage 7 - DESIGN/SPECIFY
uc_id: UC-004
name: Validate Configuration
criticality: BLOQUEADOR
created_at: 2026-04-21 04:50:00
version: 1.0.0
```

# UC-004: VALIDATE CONFIGURATION

---

## Overview

**CRITICAL USE CASE.** Validates all configuration decisions (version, languages, exclusions) before installation. Implements Fail-Fast principle: if validation fails, installation does not proceed.

**Complexity:** High (8 validation points across 3 phases)
**Criticality:** BLOQUEADOR (blocks UC-005 if failure)
**Failure Mode:** Fail-Fast with clear error messages

**Core Principle:** Never attempt installation with invalid configuration.

---

## Validation Structure

UC-004 has **8 validation points** organized in **3 phases**:

### Phase 1: Parallel Validation (Steps 1, 2, 5)
These can execute in parallel - no dependencies

### Phase 2: Sequential Validation (Steps 3→4→6)
These must execute in sequence - later steps depend on earlier results

### Phase 3: Retry with Fallback (Step 7 + 8)
SHA256 verification with 3 retry attempts

---

## Detailed Validation Flow

```
                    ┌─────────────────────────────────┐
                    │ START: Configuration Object     │
                    │ Version, Languages, ExcludedApps│
                    └────────────┬────────────────────┘
                                 │
            ┌────────────────────┼────────────────────┐
            │                    │                    │
      ┌─────▼────┐         ┌─────▼────┐        ┌─────▼────┐
      │ STEP 1   │         │ STEP 2   │        │ STEP 5   │
      │ Validate │         │ Validate │        │ Validate │
      │ Version  │         │ XML      │        │ App-     │
      │ Exists   │         │ Schema   │        │ Version  │
      │          │         │          │        │ Combo    │
      └─────┬────┘         └─────┬────┘        └─────┬────┘
            │                    │                    │
            └────────────────────┼────────────────────┘
                                 │
                    ┌────────────▼─────────────┐
                    │ All Parallel OK?         │
                    └────────────┬─────────────┘
                                 │ YES
                    ┌────────────▼─────────────┐
                    │ STEP 3: Validate Language│
                    │ Exists in version?       │
                    └────────────┬─────────────┘
                                 │ YES
                    ┌────────────▼─────────────┐
                    │ STEP 4: Validate Language│
                    │ Supported in v1.0.0?     │
                    │ (Only es-ES, en-US)      │
                    └────────────┬─────────────┘
                                 │ YES
                    ┌────────────▼─────────────┐
                    │ STEP 6: Anti-Microsoft-  │
                    │ OCT Bug: Validate        │
                    │ Language-App Compat      │
                    │ (e.g., Project+en-GB OK)│
                    └────────────┬─────────────┘
                                 │ YES
                    ┌────────────▼─────────────┐
                    │ STEP 7: Download ODT    │
                    │ & Verify SHA256         │
                    │ (Retry 3x if fail)      │
                    └────────────┬─────────────┘
                                 │ SUCCESS or 3x FAIL
                    ┌────────────▼─────────────┐
                    │ STEP 8: Generate config.│
                    │ xml (executable)        │
                    │ Verify syntax           │
                    └────────────┬─────────────┘
                                 │ SUCCESS
                    ┌────────────▼─────────────┐
                    │ VALIDATION COMPLETE     │
                    │ Mark UC-004 Success     │
                    │ Release to UC-005       │
                    └─────────────────────────┘
```

---

## Step-by-Step Specifications

### STEP 1: Validate Version Exists

**Condition:** Version in $Config.Version exists in supported list

**Validation:**
```powershell
if ($Config.Version -notin @("2024", "2021", "2019")) {
    return [PSCustomObject]@{
        Step = 1
        Category = "BLOQUEADOR"
        Error = "Version not supported"
        Success = $false
    }
}
```

**Error Category:** [BLOQUEADOR] - No recovery
**Exit on Failure:** YES - Stop immediately

---

### STEP 2: Validate XML Schema

**Condition:** Generated configuration.xml matches ODT XML schema

**Validation:**
```powershell
$schema = [xml](Get-Content "schema.xsd")
$config = [xml](Get-Content $Config.ConfigPath)
if (-not (Test-XmlSchema -Xml $config -Schema $schema)) {
    return [PSCustomObject]@{
        Step = 2
        Category = "BLOQUEADOR"
        Error = "Configuration.xml is malformed"
        Success = $false
    }
}
```

**Error Category:** [BLOQUEADOR] - No recovery
**Exit on Failure:** YES - Stop immediately

---

### STEP 3: Validate Language Exists in Version

**Condition:** Language list is compatible with Office version

**Validation:**
```powershell
$SupportedLangs = Get-SupportedLanguages -Version $Config.Version
foreach ($lang in $Config.Languages) {
    if ($lang -notin $SupportedLangs) {
        return [PSCustomObject]@{
            Step = 3
            Category = "BLOQUEADOR"
            Error = "Language $lang not available in version $($Config.Version)"
            Success = $false
        }
    }
}
```

**Error Category:** [BLOQUEADOR] - No recovery
**Exit on Failure:** YES - Stop immediately

---

### STEP 4: Validate Language Supported in v1.0.0

**Condition:** Language is in approved list (es-ES, en-US only in v1.0.0)

**Validation:**
```powershell
$ApprovedLangs = @("es-ES", "en-US")
foreach ($lang in $Config.Languages) {
    if ($lang -notin $ApprovedLangs) {
        return [PSCustomObject]@{
            Step = 4
            Category = "BLOQUEADOR"
            Error = "Language $lang not available until v1.1"
            Success = $false
        }
    }
}
```

**Error Category:** [BLOQUEADOR] - No recovery
**Exit on Failure:** YES - Stop immediately

---

### STEP 5: Validate App-Version Compatibility

**Condition:** All excluded apps are valid for the selected version

**Validation:**
```powershell
$AvailableApps = Get-AvailableApps -Version $Config.Version
foreach ($app in $Config.ExcludedApps) {
    if ($app -notin $AvailableApps) {
        return [PSCustomObject]@{
            Step = 5
            Category = "BLOQUEADOR"
            Error = "App $app not available in version $($Config.Version)"
            Success = $false
        }
    }
}
```

**Error Category:** [BLOQUEADOR] - No recovery
**Exit on Failure:** YES - Stop immediately

---

### STEP 6: Anti-Microsoft-OCT Bug - Language-App Compatibility

**Condition:** Language and application combination is valid

**Background:** Microsoft OCT allows selecting incompatible language-app combinations (e.g., English UK + Project) which cause silent installation failures.

**Validation:**
```powershell
$CompatMatrix = Get-LanguageCompatibilityMatrix
foreach ($lang in $Config.Languages) {
    foreach ($app in $Config.ExcludedApps) {
        if (-not $CompatMatrix["$lang`_$app"].IsCompatible) {
            return [PSCustomObject]@{
                Step = 6
                Category = "BLOQUEADOR"
                Error = "Language $lang incompatible with app $app"
                Success = $false
            }
        }
    }
}
```

**Error Category:** [BLOQUEADOR] - No recovery
**Exit on Failure:** YES - Stop immediately
**Special Note:** This is the core workaround for Microsoft OCT bug identified in Stage 1

---

### STEP 7: Download ODT & Verify SHA256 (With Retry)

**Condition:** Office Deployment Tool is downloaded and SHA256 matches official value

**Validation with Retry:**
```powershell
$MaxRetries = 3
$Attempt = 0
while ($Attempt -lt $MaxRetries) {
    $Attempt++
    try {
        $DownloadedFile = Download-ODT
        $ActualSHA256 = Get-FileHash $DownloadedFile -Algorithm SHA256
        $ExpectedSHA256 = Get-OfficialODTSHA256 -Version $Config.Version
        
        if ($ActualSHA256.Hash -eq $ExpectedSHA256) {
            # SUCCESS
            break
        }
    } catch {
        Write-Warning "Attempt $Attempt failed: $($_.Message)"
    }
    
    if ($Attempt -lt $MaxRetries) {
        Start-Sleep -Seconds (2 * $Attempt)  # Exponential backoff
    }
}

if ($ActualSHA256.Hash -ne $ExpectedSHA256) {
    return [PSCustomObject]@{
        Step = 7
        Category = "CRITICO"
        Error = "ODT verification failed after 3 attempts"
        Success = $false
    }
}
```

**Error Category:** [CRITICO] - Automatic retry (3x)
**Retry Strategy:** Exponential backoff (2s, 4s, 6s)
**Exit on Failure:** YES after 3 attempts

---

### STEP 8: Generate config.xml (Executable)

**Condition:** configuration.xml is valid and ready for setup.exe

**Validation:**
```powershell
$ConfigXml = Convert-ToConfigurationXml @{
    Version = $Config.Version
    Languages = $Config.Languages
    ExcludedApps = $Config.ExcludedApps
}

if (-not (Test-Path $ConfigXml)) {
    return [PSCustomObject]@{
        Step = 8
        Category = "BLOQUEADOR"
        Error = "configuration.xml generation failed"
        Success = $false
    }
}

# Write to disk for UC-005 to use
$ConfigXml | Out-File -FilePath $Config.ConfigPath -Encoding UTF8
```

**Error Category:** [BLOQUEADOR] - No recovery
**Exit on Failure:** YES - Stop immediately

---

## Function Signature

```powershell
function Validate-OfficeConfiguration {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory = $true)]
        [PSObject]$Config
    )
    
    # Returns: PSObject with properties:
    # - AllStepsPassed: $true/$false
    # - FailedStep: 1-8 or $null
    # - FailureCategory: "BLOQUEADOR", "CRITICO", or $null
    # - ErrorMessage: string or $null
    # - Logs: array of validation log entries
}
```

---

## Error Categories and Handling

### [BLOQUEADOR] - Critical Blocking Error
- Steps: 1, 2, 3, 4, 5, 6, 8
- **Action:** Fail-Fast. Stop immediately. Do NOT proceed to UC-005.
- **Recovery:** User must fix input and restart from UC-001
- **Example:** "Version 2022 not supported"

### [CRITICO] - Critical but Recoverable
- Steps: 7 (SHA256 verification)
- **Action:** Automatic retry up to 3 times with exponential backoff
- **Recovery:** If 3 retries fail, then Fail-Fast
- **Example:** "Network timeout downloading ODT"

### [RECUPERABLE] - Recoverable Warning
- None in UC-004 (all are blocking or retry)
- Reserved for future use (non-essential components)

---

## Phase Execution Logic

```
[PHASE 1: Parallel]
  Execute in parallel (no dependencies):
  - Step 1: Validate Version
  - Step 2: Validate XML Schema
  - Step 5: Validate App-Version Combo

  If ANY fails → Return error [BLOQUEADOR]

[PHASE 2: Sequential]
  Execute in sequence (dependencies):
  - Step 3: Language exists in version
    → If fail → Return error [BLOQUEADOR]
  
  - Step 4: Language in approved list
    → If fail → Return error [BLOQUEADOR]
  
  - Step 6: Anti-Microsoft-OCT bug
    → If fail → Return error [BLOQUEADOR]

[PHASE 3: Retry Logic]
  - Step 7: Download + SHA256 verify
    → If fail → Retry 3x with backoff
    → If still fail → Return error [CRITICO]

[Phase 4: Generation]
  - Step 8: Generate config.xml
    → If fail → Return error [BLOQUEADOR]

[SUCCESS]
All 8 steps passed → Release to UC-005
```

---

## State Management

### Before UC-004
```powershell
$Config = @{
    Version           = "2024"
    Languages         = @("es-ES")
    ExcludedApps      = @("Teams", "OneDrive")
    ConfigPath        = $null          # Not yet generated
    ODTPath           = $null          # Not yet downloaded
    ValidationPassed  = $false
}
```

### After UC-004 Success
```powershell
$Config = @{
    Version           = "2024"
    Languages         = @("es-ES")
    ExcludedApps      = @("Teams", "OneDrive")
    ConfigPath        = "C:\Temp\configuration.xml"  # Generated
    ODTPath           = "C:\Temp\setup.exe"          # Downloaded
    ValidationPassed  = $true          # CRITICAL: Set to $true
    ValidationLog     = @(...)         # Array of step results
}
```

### If UC-004 Fails
```powershell
$Config.ValidationPassed = $false
# UC-005 checks this flag before proceeding
# If $false, UC-005 is not executed
```

---

## Logging Strategy

Every step logs entry, validation, and result:

```powershell
Write-OfficeLog -Level INFO -Message "UC-004 STEP 1: Validating version..."
Write-OfficeLog -Level DEBUG -Message "Version: $($Config.Version)"
Write-OfficeLog -Level INFO -Message "UC-004 STEP 1: Version valid ✓"

Write-OfficeLog -Level WARN -Message "UC-004 STEP 7: SHA256 mismatch, retry 1/3"
Write-OfficeLog -Level INFO -Message "UC-004 STEP 7: SHA256 verified ✓"

Write-OfficeLog -Level ERROR -Message "UC-004 STEP 3 [BLOQUEADOR]: Language not supported"
Write-OfficeLog -Level INFO -Message "UC-004 FAILED: Validation did not complete"
```

---

## Success Criteria

- [x] All 8 steps specified with exact validation logic
- [x] Phase structure clear (parallel, sequential, retry)
- [x] Error categories defined (BLOQUEADOR, CRITICO)
- [x] Retry logic specified (3x with exponential backoff)
- [x] Anti-Microsoft-OCT-bug validation at Step 6
- [x] State management clear (ValidationPassed flag)
- [x] Logging at every validation point

---

## Critical Decision Points

**Question:** Why 8 steps and not 4?
**Answer:** Fail-Fast requires multiple independent checks. Combining steps would mask individual failures.

**Question:** Why retry only on Step 7?
**Answer:** Only Step 7 (network) is transient. Steps 1-6 and 8 are configuration issues (permanent).

**Question:** Why exponential backoff instead of immediate retry?
**Answer:** Gives transient network issues time to resolve, prevents overwhelming the network.

---

**Version:** 1.0.0
**Design Status:** Complete
**Criticality:** BLOQUEADOR
**Next UC:** UC-005 - Install Office (proceeds only if UC-004 succeeds)

