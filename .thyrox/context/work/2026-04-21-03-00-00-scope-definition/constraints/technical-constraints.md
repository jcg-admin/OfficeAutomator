```yml
created_at: 2026-04-22 09:30:00
project: OfficeAutomator
work_package: 2026-04-21-03-00-00-scope-definition
phase: Phase 4 — CONSTRAINTS
author: NestorMonroy
status: Borrador
```

# Technical Constraints — OfficeAutomator Phase 4

## Overview

Restricciones técnicas que limitan el diseño y la implementación del OfficeAutomator.

---

## 1. Operating System Constraints

### Requirement
- **OS:** Windows Server 2016+ or Windows 10/11
- **Architecture:** x64 only (no x86/32-bit support)
- **Installation context:** Local administrator privileges REQUIRED

### Impact on Design
- UC-005 (Install-Office) must check OS version before execution
- Fail-Fast validation in UC-004 must validate Windows edition
- PowerShell module cannot run on non-Windows platforms

### Mitigation
- Add OS version check in all UC implementations
- Document Windows version requirement in README
- Provide clear error messages for unsupported OS

---

## 2. PowerShell Runtime Constraints

### Requirement
- **Minimum:** PowerShell 5.1 (Windows PowerShell) or PowerShell 7.0+ (Core)
- **Preferred:** PowerShell 7.4+ for performance and features
- **Module platform:** Windows PowerShell 5.1 minimum for compatibility

### Constraint Details
- No advanced PS7 features (unless we branch for PS5.1 compatibility)
- Windows PowerShell 5.1 has slower module loading
- Script execution policy must allow local script execution
- RemoteSigned or Unrestricted execution policy required

### Impact on Design
- Keep code compatible with PS 5.1 unless performance-critical
- Use `#requires -PSEdition Desktop` or `#requires -PSEdition Core` if branching
- Error handling must account for PS 5.1 limitations
- UC-001 through UC-005 must work in both PS 5.1 and PS 7+

### Mitigation
- Add PS version check in UC-001 (Select-OfficeVersion)
- Test all UCs on both PS 5.1 and PS 7.4
- Document minimum PS version in installation guide

---

## 3. Office Deployment Tool (ODT) Constraints

### Requirement
- **Tool:** Microsoft Office Deployment Tool (Setup.exe)
- **Version support:** Consistent with Office versions 2024, 2021, 2019
- **Distribution:** Download from Microsoft's official servers
- **File format:** XML configuration (configuration.xml)
- **Signature:** SHA256 integrity verification mandatory

### Constraint Details
- ODT must be downloaded fresh for each Office version
- Network connectivity required for download phase
- ODT has built-in timeout (30-60 minutes per installation)
- setup.exe /configure must be called, not /install
- No direct SDK or API for Office installation (ODT is only supported method)

### Microsoft OCT Bug (Documented in UC-004)
- Silent installation failure: specific language+application combinations don't error—they just skip silently
- Example: `pt-BR` (Brazilian Portuguese) + Project 2024 installs silently without Project
- **Mitigation:** UC-004 validation point 6 prevents these combinations

### Impact on Design
- UC-005 (Install-Office) must run setup.exe with /configure flag
- UC-004 validation must include language-application matrix
- Download phase must include SHA256 verification
- Installation phase requires local admin

### Mitigation
- Pre-download and cache ODT with version information
- Validate ODT SHA256 with Microsoft-provided hashes (max 3 retries)
- Log all ODT operations for troubleshooting
- Provide detailed error messages if ODT fails

---

## 4. Microsoft Office Version Constraints

### Requirement
- **Supported versions:** Office 2024 LTSC, Office 2021 LTSC, Office 2019 LTSC
- **Unsupported:** Office 365 / Microsoft 365 (subscription model - different installation)
- **Support windows:** 
  - Office 2024: Until October 13, 2026 (mainstream)
  - Office 2021: Until October 13, 2026 (mainstream)
  - Office 2019: Until October 13, 2025 (mainstream)

### Constraint Details
- Each LTSC version has separate ODT binary
- Each version has separate language pack availability
- Application inclusion/exclusion varies by version
- Licensing model: Volume licensing required (not consumer SKU)

### Impact on Design
- UC-001 must enumerate only LTSC versions (exclude Office 365)
- UC-002 must filter languages by version (not all languages available for all versions)
- UC-003 must validate application availability per version
- UC-004 must include version compatibility checks

### Mitigation
- Hardcode supported versions in UC-001 (2024, 2021, 2019)
- Maintain language-version matrix in configuration data
- Provide clear messages if user selects unavailable language for version
- Plan for Office 2019 sunset (Oct 2025) with upgrade path

---

## 5. Language Support Constraints

### Current Limitation (v1.0.0)
- **Base languages:** 2 maximum (es-ES, en-US)
- **Future:** Extensible to 38+ languages in v1.1+

### Constraint Details
- Language availability varies by Office version
- Regional variants (es-ES vs es-MX) have different support levels
- Base-2 language limit prevents Windows UI language bloat
- Language pack downloads must match exact Office version

### Version × Language Matrix
```
Office 2024: es-ES, en-US, de-DE, fr-FR, it-IT, pt-BR, ja-JP, zh-CN, zh-TW, ko-KR, ru-RU, ... (34 total)
Office 2021: es-ES, en-US, de-DE, fr-FR, it-IT, pt-BR, ja-JP, zh-CN, zh-TW, ko-KR, ru-RU, ... (33 total - no new languages)
Office 2019: es-ES, en-US, de-DE, fr-FR, it-IT, pt-BR, ja-JP, zh-CN, zh-TW, ko-KR, ... (30 total - legacy languages dropped)
```

### Impact on Design
- UC-002 must validate language against version-specific list
- UC-004 validation point 3-4 checks language existence and version support
- Configuration XML must specify exact language code (e.g., `es-ES`, not `Spanish`)

### Mitigation
- Maintain explicit language-version compatibility matrix
- Provide helpful error messages if language unavailable for version
- Plan extensibility in codebase for v1.1+ multi-language support

---

## 6. Application/Component Constraints

### Current Supported Applications
1. **Access** - Database application
2. **Excel** - Spreadsheet application
3. **OneNote** - Note-taking application
4. **Outlook** - Email/calendar application
5. **PowerPoint** - Presentation application
6. **Project** - Project management (version-dependent)
7. **Publisher** - Desktop publishing (version-dependent)
8. **Visio** - Diagramming (version-dependent)
9. **Word** - Word processing application
10. **Teams** - Communication platform (v2024+, optional)
11. **OneDrive** - Cloud storage (can be excluded)
12. **Groove** - Sync client (legacy, can be excluded)
13. **Lync** - Communications (Office 2019 only, legacy)

### Version × Application Availability
- Office 2024: All 13 applications
- Office 2021: All except newest Teams feature set
- Office 2019: Lacks Teams, has Lync instead (legacy communications)
- Project/Visio/Publisher: Optional add-ons (not base install)

### Exclusion Constraints
- Cannot exclude core applications (Word, Excel, PowerPoint, Outlook, Access, OneNote)
- Can exclude Teams, OneDrive, Groove, Lync, Publisher, Project, Visio
- Excluding application saves ~100-500 MB per app
- Teams exclusion is most common (users may have separate MS Teams installation)

### Impact on Design
- UC-003 (Exclude-OfficeApplications) must validate against available exclusions
- UC-004 validation must ensure core apps are never excluded
- Configuration XML must list excluded apps properly
- v1.1+ may allow selective app inclusion (current model: install all, exclude some)

### Mitigation
- Maintain application-version compatibility matrix
- Fail-Fast in UC-003 if user tries to exclude core app
- Clear messaging in UC-003 explaining why core apps cannot be excluded

---

## 7. Configuration XML (ODT Config File) Constraints

### Format Requirement
- **Schema:** XML 1.0 (ODT configuration schema)
- **Encoding:** UTF-8 with BOM required
- **Root element:** `<Configuration>`
- **Child elements:** `<Add>`, `<Property>`, `<Updates>`, `<RemoveMSI>`, `<AppSettings>`, `<Logging>`

### ODT Schema Validation
- Must validate against Microsoft's XSD schema
- Missing required elements → Silent failure (Microsoft bug)
- Malformed XML → ODT error "cannot parse configuration file"
- Encoding issues → Installation hangs or fails silently

### Configuration Constraints
```xml
<Configuration>
  <Add OfficeClientEdition="64">                    <!-- Only 64-bit supported -->
    <Product ID="O365ProPlusRetail">               <!-- MUST match version -->
      <Language ID="es-ES" />
      <Language ID="en-US" />
      <ExcludedProduct ID="Teams" />               <!-- Can exclude -->
    </Product>
  </Add>
  <Property Name="SharedComputerLicensing" Value="0" /> <!-- 0=per-user, 1=shared -->
  <Updates Enabled="0" />                          <!-- Disable auto-update in config -->
  <Logging Level="Standard" Path="C:\Logs\" />     <!-- Logging location -->
</Configuration>
```

### Impact on Design
- UC-004 validation point 1 (XSD schema validation) is CRITICAL
- UC-004 validation point 8 (XML executability) must test configuration
- UC-005 (Install-Office) uses configuration.xml as input to setup.exe
- No manual editing of XML allowed (all generation via UC functions)

### Mitigation
- Generate XML programmatically (no manual editing)
- Validate XML schema before passing to ODT
- Maintain XML template with all required elements
- Log generated XML for debugging

---

## 8. Network and Connectivity Constraints

### Requirement
- **Download phase:** Requires internet connectivity to Microsoft CDN
- **Speed:** Minimum 5 Mbps recommended (Office 2024 ≈ 2.5 GB)
- **Retry logic:** Microsoft ODT has built-in 3-retry mechanism
- **Timeout:** 30-60 minutes per file group (ODT default)

### Download Size Estimates
- Office 2024 base: 2.5 GB
- Language pack (additional): 300-400 MB per language
- Exclusion savings: 100-500 MB per excluded application

### Constraint Details
- Installation requires downloading from:
  - Microsoft Office CDN (content-delivery.microsoft.com)
  - No offline installation supported (must download)
- Network interruption = automatic retry (up to 3 times, then failure)
- Proxy requirements: System proxy settings respected by ODT

### Impact on Design
- UC-005 must handle network errors gracefully
- Installation duration heavily dependent on network speed
- No offline "pre-cached" installation path in v1.0.0
- Progress monitoring limited to ODT's built-in logging

### Mitigation
- Provide estimated download times based on file size
- Clear error messages if network fails
- Suggest retry if temporary network failure detected
- Plan v1.1+ offline installation (pre-cache scenario)

---

## 9. Storage Constraints

### Disk Space Requirement
- **Temporary download cache:** 3-4 GB (during installation)
- **Final installed size:** 2.5-3.5 GB (after cleanup)
- **Log/temp files:** 100-200 MB
- **Total required free space:** 4-5 GB minimum

### Installation Paths
- **Default:** `C:\Program Files\Microsoft Office`
- **Alternative:** Any NTFS volume with 5 GB free space
- **System drive:** Must have 5 GB free for temp cache

### Constraint Details
- Office 2024 must install to local fixed disk (not network drives)
- Temp cache created in user's AppData (or specified location)
- No support for installation to network shares
- SSD recommended for faster installation

### Impact on Design
- UC-001 or UC-004 should validate free disk space
- UC-005 must check remaining disk space before download
- Fail-Fast if less than 5 GB free space available

### Mitigation
- Add disk space check in UC-004 validation
- Provide clear error message if disk full
- Suggest cleanup steps if insufficient space
- Monitor download progress and adjust timeout if slow

---

## 10. Administrator Privileges Constraint

### Requirement
- **Privilege level:** Local administrator account REQUIRED
- **UAC (User Access Control):** Must be enabled or disabled (both supported)
- **Service accounts:** Must be local administrator or domain admin with local elevation

### Constraint Details
- setup.exe requires admin context (no workaround)
- PowerShell module must detect and enforce admin privilege
- If non-admin: Fail-Fast immediately in UC-001
- Remote installations require PSRemoting + admin account

### Impact on Design
- UC-001 (Select-OfficeVersion) should check admin privilege upfront
- Provide clear error message if non-admin detected
- Cannot proceed if privilege elevation fails
- Error is non-recoverable (user must re-run as admin)

### Mitigation
- Add admin check at module load time (workflow-execute)
- Provide clear instructions for elevating to admin
- Log privilege level in installation start

---

## 11. Time and Scheduling Constraints

### Installation Duration
- **Typical:** 30-60 minutes (depends on:)
  - Network speed (5 Mbps = 7 min download + 5 min extract + 10 min install = ~22 min)
  - Number of languages (2 languages adds ~10 min)
  - Excluded apps (each exclusion saves ~1-2 min install time)
  - System performance (slow HDD adds 10-15 min)
- **Worst case:** 90+ minutes on slow network with full install

### Scheduling Implications
- Installation cannot be interrupted (high risk of corruption)
- Must complete within single user session
- Background installation not supported in v1.0.0
- Cannot run multiple Office installations in parallel

### Impact on Design
- UC-005 must run uninterrupted (no pause/resume)
- Progress monitoring essential for user confidence
- Timeout error if installation exceeds 2 hours
- Sleep/hibernation during installation not supported

### Mitigation
- Provide realistic time estimates in UC-001 (30-90 minutes based on config)
- Progress bar/logging in UC-005
- Document that system cannot sleep during installation
- v1.1+ may support pause/resume

---

## 12. Security and Signing Constraints

### SHA256 Integrity Requirement
- **Mandatory:** All downloaded files must be SHA256 verified
- **Frequency:** Verify EVERY download (no caching without verification)
- **Failure handling:** Max 3 retries then fail

### Signing Certificate
- **Office Deployment Tool:** Signed by Microsoft (certificate pinning NOT needed)
- **Configuration XML:** No signing requirement
- **PowerShell module:** Should be signed (organizational requirement, not functional)

### License Validation
- **Model:** Volume licensing only (LTSC requires organizational deployment)
- **Activation:** Key or Active Directory-based activation required
- **No:** Consumer product keys or trial installations

### Impact on Design
- UC-004 validation point 7 (SHA256) is CRITICAL
- UC-005 must verify all downloaded files
- Fail-Fast if signature verification fails
- UC-004 validation point 2 (version) must match Microsoft-signed ODT

### Mitigation
- Maintain Microsoft's official SHA256 hashes
- Implement retry logic (max 3) with exponential backoff
- Log all signature verification attempts
- Document license requirement (volume licensing only)

---

## Summary: Technical Constraint Impact on Design

| Constraint | Phase Impact | Severity | Mitigation |
|-----------|--------------|----------|-----------|
| Windows OS only | UC-001, UC-004 | HIGH | Fail-Fast OS check |
| PowerShell 5.1+ | Module load | HIGH | Test on both versions |
| ODT availability | UC-005 | HIGH | Download + verify SHA256 |
| Office LTSC only | UC-001 | MEDIUM | Enumerate LTSC versions |
| Language support | UC-002, UC-004 | MEDIUM | Maintain version matrix |
| Admin privilege | UC-001 | HIGH | Privilege check at start |
| Network required | UC-005 | HIGH | Handle download errors |
| 5 GB disk space | UC-004, UC-005 | MEDIUM | Validate free space |
| 30-90 min duration | UC-005 | MEDIUM | Provide time estimates |
| SHA256 verification | UC-004, UC-005 | CRITICAL | Max 3 retries |

---

**Phase 4 Artifact:** technical-constraints.md  
**Status:** Borrador  
**Next Gate:** Phase 4 → 5 (Validar restricciones antes de diseñar estrategia)
