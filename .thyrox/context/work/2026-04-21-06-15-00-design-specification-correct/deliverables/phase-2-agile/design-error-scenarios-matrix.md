```yml
created_at: 2026-05-16 16:05
updated_at: 2026-05-16 16:05
document_type: Error Handling Reference - Scenario Matrix & Recovery
document_version: 1.0.0
version_notes: Complete documentation of all 19 error codes with context, recovery paths, user messages, IT runbook
stage: Stage 7 - DESIGN/SPECIFY
work_package: 2026-04-21-06-15-00-design-specification-correct
phase: 2-Agile-Sprints
sprint_number: 2
task_id: T-027
task_name: Error Scenario Matrix & Recovery Paths
execution_date: 2026-05-16 16:05 onwards
duration_hours: TBD
story_points: 4
roles_involved: ARCHITECT (Claude)
dependencies: T-020 (ErrorHandler), T-021 (Error Codes), T-026 (State Machine)
design_artifacts:
  - Error scenario matrix (all 19 codes)
  - Trigger conditions (when does each error occur)
  - Detection points (which method detects it)
  - Recovery actions (what happens after error)
  - User-facing messages (what user sees)
  - IT support runbook (help desk guide)
  - Escalation paths (when to contact IT)
  - Retry policy matrix (transient/system/permanent)
acceptance_criteria:
  - All 19 error codes documented with full context
  - Trigger conditions for each code
  - Detection points (UC and method)
  - Recovery actions documented
  - User-facing messages for each code
  - IT support runbook sections
  - Escalation criteria defined
  - Category distribution verified (6 categories)
status: IN PROGRESS
```

# DESIGN: ERROR SCENARIO MATRIX & RECOVERY PATHS

## Overview

This document provides comprehensive documentation of all 19 OfficeAutomator error codes. For each error, it documents: when it occurs (trigger), how it's detected, what happens after detection (recovery), what users see, and what IT support needs to know.

**Version:** 1.0.0  
**Scope:** All 19 error codes with full context (6 categories × ~3 codes each)  
**Source:** T-020 (ErrorHandler), T-021 (Error Catalog), T-026 (State Machine)  
**Purpose:** Reference for Stage 10 implementation, help desk, and user support

---

## 1. Error Code Matrix Overview

```
Total Error Codes: 19 (18 mapped + 1 fallback)

Distribution by Category:
  CONFIG (4): Version, language, exclusion, file errors
  SECURITY (3): Hash mismatch, certificate issues
  SYSTEM (4): Timeout, disk, admin, fallback
  NETWORK (3): Download, connection errors
  INSTALL (3): Setup failure, already installed, corrupted
  ROLLBACK (3): Partial rollback failures

Retry Policy Distribution:
  Transient (3x retry with backoff): 5 codes
  System (1x retry): 1 code
  Permanent (no retry): 13 codes

Severity Distribution:
  CRITICAL (escalate to IT): 3 codes (OFF-ROLLBACK-*)
  HIGH (user action required): 8 codes
  MEDIUM (user retry or workaround): 5 codes
  INFO (informational, no action): 3 codes
```

---

## 2. Error Codes by Category with Full Documentation

### CATEGORY: CONFIG (4 Codes)

#### **OFF-CONFIG-001: Invalid or Unavailable Office Version**

```
Category: CONFIG
Severity: HIGH
Retry Policy: PERMANENT (0x retry, fail immediately)
State: SELECT_VERSION

TRIGGER CONDITION:
  • User selects version not in whitelist (2024, 2021, 2019)
  • Selected version binaries not available on CDN
  • Selected version unsupported for target system

DETECTION POINT:
  UC-001: VersionSelector.IsValidVersion() returns false
  UC-004: ConfigValidator.ValidateVersionAvailability() step fails

WHEN IT OCCURS:
  Probability: LOW (UI validates against whitelist)
  Typical Cause: Version binaries removed from CDN (outdated), user bypassed UI validation

ERROR RESULT OBJECT:
  {
    code: "OFF-CONFIG-001",
    message: "Invalid or unavailable Office version",
    technicalDetails: "Version not in whitelist or binaries not found on CDN",
    suggestedAction: "Select from available versions: 2024, 2021, 2019"
  }

RECOVERY ACTION:
  1. ErrorHandler.HandleError() routes to PERMANENT error path
  2. Display error message to user
  3. Return to SELECT_VERSION state
  4. User must select different version
  5. Retry validation

USER-FACING MESSAGE:
  Title: "Invalid Office Version"
  Message: "The Office version you selected is not available or not supported.
            Please choose from: Office 2024, Office 2021, or Office 2019."
  Action: [Select Different Version]

IT SUPPORT RUNBOOK:
  Issue: User reports "Invalid Office version" error
  
  Troubleshooting:
    1. Verify user selected from UI dropdown (should enforce whitelist)
    2. Check CDN status: Are binaries available for that version?
    3. Confirm version is in OfficeAutomator whitelist
    
  If user insists on unavailable version:
    → Inform version is no longer supported
    → Suggest alternative (2024 is latest, 2021 is LTS)
    
  If all versions show as unavailable:
    → Check CDN connectivity
    → Verify Microsoft download URLs are accessible
    → Check Windows Firewall / proxy settings
    
  Resolution:
    → User selects available version
    → Restart OfficeAutomator and retry

ESCALATION CRITERIA:
  • All versions show unavailable (CDN/network issue)
  • Microsoft discontinues all supported versions (Microsoft end-of-life)
  → Escalate to: IT Infrastructure (network/CDN access)
```

---

#### **OFF-CONFIG-002: Unsupported Language Selection**

```
Category: CONFIG
Severity: MEDIUM
Retry Policy: PERMANENT (0x retry)
State: SELECT_LANGUAGE

TRIGGER CONDITION:
  • User selects language not supported by selected Office version
  • Language not in approved whitelist (en-US, es-MX)
  • Version-language compatibility not available

DETECTION POINT:
  UC-002: LanguageSelector.IsValidLanguageSelection() returns false
  UC-004: ConfigValidator.ValidateLanguageSupport() step fails

WHEN IT OCCURS:
  Probability: LOW (UI dropdown restricted by version selection)
  Typical Cause: Language pack not available for selected version, user bypassed UI

ERROR RESULT OBJECT:
  {
    code: "OFF-CONFIG-002",
    message: "Unsupported language for selected Office version",
    technicalDetails: "Language not supported or language pack unavailable",
    suggestedAction: "Select supported languages: English (en-US) or Spanish (es-MX)"
  }

RECOVERY ACTION:
  1. ErrorHandler.HandleError() routes to PERMANENT path
  2. Display error, highlight supported languages
  3. Return to SELECT_LANGUAGE state
  4. User selects supported language

USER-FACING MESSAGE:
  Title: "Unsupported Language"
  Message: "The language you selected is not available for Office {version}.
            Supported languages are: English (US), Spanish (Mexico)"
  Action: [Select Different Languages]

IT SUPPORT RUNBOOK:
  Issue: User reports "Unsupported language" error
  
  Troubleshooting:
    1. Verify language is in whitelist (en-US, es-MX only for v1.0.0)
    2. Check if language pack exists for selected Office version
    3. Note: v1.0.0 only supports 2 languages (v1.1 will add more)
    
  If user needs additional language:
    → Inform only en-US and es-MX supported in v1.0.0
    → Schedule upgrade to v1.1 for additional languages
    
  Resolution:
    → User selects en-US or es-MX
    → Retry installation

ESCALATION CRITERIA:
  • User requires language not in whitelist
  → Escalate to: Product Management (request new language support in v1.1)
```

---

#### **OFF-CONFIG-003: Invalid Application Exclusion Selection**

```
Category: CONFIG
Severity: MEDIUM
Retry Policy: PERMANENT (0x retry)
State: SELECT_APPS

TRIGGER CONDITION:
  • User selects application not in exclusion whitelist
  • Duplicate selections in exclusion list
  • Invalid app ID format

DETECTION POINT:
  UC-003: AppExclusionSelector.IsValidExclusionSet() returns false
  UC-004: ConfigValidator.ValidateExclusionList() step fails

WHEN IT OCCURS:
  Probability: LOW (UI checkboxes restrict to whitelist)
  Typical Cause: Corrupted configuration file, user manual JSON editing

ERROR RESULT OBJECT:
  {
    code: "OFF-CONFIG-003",
    message: "Invalid application exclusion selection",
    technicalDetails: "App not in whitelist or duplicate selection detected",
    suggestedAction: "Select only: Teams, OneDrive, Groove, Lync, or Bing"
  }

RECOVERY ACTION:
  1. ErrorHandler.HandleError() routes to PERMANENT path
  2. Clear invalid selections, show whitelist
  3. Return to SELECT_APPS state
  4. User reselects valid apps

USER-FACING MESSAGE:
  Title: "Invalid Application Selection"
  Message: "One or more of your selections are not valid or are duplicated.
            Valid applications to exclude are:
            • Microsoft Teams
            • OneDrive
            • Microsoft Groove
            • Skype for Business (Lync)
            • Bing Services"
  Action: [Review Selection]

IT SUPPORT RUNBOOK:
  Issue: User reports "Invalid application exclusion" error
  
  Troubleshooting:
    1. Check UI was used (should prevent invalid selections)
    2. If config file was edited manually:
       → Check for duplicate app names
       → Validate app names against whitelist
    3. Clarify which apps can be excluded:
       - Teams, OneDrive, Groove, Lync, Bing
       - Other apps cannot be excluded
    
  If user wants to exclude app not in whitelist:
    → Explain only 5 apps supported for exclusion in v1.0.0
    → Suggest v1.1 for expanded app exclusion list
    
  Resolution:
    → User corrects selections
    → Retry installation

ESCALATION CRITERIA:
  • Persistent "invalid app" errors despite correct selection
  → Check if configuration file is corrupted
  → Escalate to: IT Support (manual config file repair/deletion)
```

---

#### **OFF-CONFIG-004: Configuration File Invalid or Missing**

```
Category: CONFIG
Severity: HIGH
Retry Policy: PERMANENT (0x retry)
State: VALIDATE

TRIGGER CONDITION:
  • config.xml file not found at expected path
  • config.xml file size is 0 bytes (empty)
  • config.xml not readable by OfficeAutomator process
  • config.xml schema validation fails (malformed XML)

DETECTION POINT:
  UC-004: ConfigValidator.ValidateConfigFileExists() (Step 0) fails
  UC-004: ConfigValidator.ValidateXmlSchema() (Step 1) fails

WHEN IT OCCURS:
  Probability: MEDIUM (file system issues)
  Typical Cause: File deleted mid-process, disk corruption, permission denied

ERROR RESULT OBJECT:
  {
    code: "OFF-CONFIG-004",
    message: "Configuration file is invalid or inaccessible",
    technicalDetails: "config.xml not found, empty, or not readable",
    suggestedAction: "Run OfficeAutomator again to regenerate configuration"
  }

RECOVERY ACTION:
  1. ErrorHandler.HandleError() routes to PERMANENT path
  2. Delete corrupted/invalid config.xml
  3. Log error with timestamp and path
  4. Return to VALIDATE state or previous selection state
  5. Regenerate config.xml (automatic on next validation)

USER-FACING MESSAGE:
  Title: "Configuration File Error"
  Message: "The Office configuration file could not be created or accessed.
            This can happen if the file was deleted or the disk is full.
            
            Please try again. A new configuration file will be created."
  Action: [Retry] [Check Disk Space]

IT SUPPORT RUNBOOK:
  Issue: User reports "Configuration file error"
  
  Troubleshooting:
    1. Check if %APPDATA%\OfficeAutomator\ folder exists and is writable
    2. Verify disk space available (need at least 100MB for config + temp files)
    3. Check Windows Event Viewer for disk errors
    4. Verify user has write permissions to AppData folder
    5. Try running OfficeAutomator as Administrator (UAC bypass)
    
  Common Causes:
    • Disk full / insufficient free space
    • Permission denied (AppData access)
    • Antivirus blocking file creation
    • Corrupted file system
    
  Resolution:
    → Free up disk space if needed
    → Run as Administrator
    → Check antivirus exclusions
    → Retry OfficeAutomator
    
  If issue persists:
    → Delete entire %APPDATA%\OfficeAutomator\ folder
    → Restart system
    → Run OfficeAutomator again

ESCALATION CRITERIA:
  • Persistent "invalid config" after disk cleanup and reboot
  • Disk errors in Windows Event Viewer
  → Escalate to: IT Support (file system corruption, hardware issue)
```

---

### CATEGORY: SECURITY (3 Codes)

#### **OFF-SECURITY-101: Office Download Hash Mismatch (Transient)**

```
Category: SECURITY
Severity: HIGH
Retry Policy: TRANSIENT (3x retry with backoff: 2s, 4s, 6s)
State: VALIDATE (Step 4)

TRIGGER CONDITION:
  • Downloaded Office binaries hash != Microsoft official hash
  • Man-in-the-middle attack detected (MITM)
  • Corrupted download (network error during transmission)
  • Incomplete download (file truncated)

DETECTION POINT:
  UC-004: ConfigValidator.ValidateOfficeHash() (Step 4)
    Compares local hash with Microsoft's published hash

WHEN IT OCCURS:
  Probability: LOW (hash verification prevents tampering)
  Typical Cause: Network corruption during download, proxy interference

ERROR RESULT OBJECT:
  {
    code: "OFF-SECURITY-101",
    message: "Office download integrity check failed",
    technicalDetails: "Hash mismatch detected. Downloaded file hash != Microsoft hash",
    suggestedAction: "Retry automatically or check network security settings"
  }

RECOVERY ACTION:
  1. ErrorHandler.HandleError() routes to TRANSIENT path
  2. Delete corrupted download
  3. Retry download with backoff: 2s, 4s, 6s (3x total)
  4. If 3 retries exhausted: fail with OFF-INSTALL-401
  5. If retry succeeds: continue validation

AUTOMATIC RETRY:
  Attempt 1: Wait 2s, retry download
  Attempt 2: Wait 4s, retry download
  Attempt 3: Wait 6s, retry download
  Failure: OFF-INSTALL-401 (permanent failure)

USER-FACING MESSAGE:
  Title: "Office Download Verification"
  Message: "The downloaded Office files didn't match Microsoft's official version.
            This could indicate a network issue or security concern.
            
            OfficeAutomator is automatically retrying (attempt {X} of 3)...
            
            Please wait or check your network security settings."
  Progress: Shows retry count and estimated time

IT SUPPORT RUNBOOK:
  Issue: User reports "Office download hash mismatch" error (after 3 retries)
  
  Troubleshooting:
    1. Check network connectivity
    2. Test: Can user access Microsoft.com directly? (health check)
    3. Check for network proxy / firewall that might intercept downloads
    4. Check antivirus real-time protection (may interfere with hash verification)
    5. Try on different network (home WiFi vs. corporate LAN)
    
  Common Causes:
    • Proxy server modifying traffic
    • Antivirus intercepting downloads
    • Network corruption (rare)
    • Firewall blocking connection
    
  Resolution:
    → Disable antivirus real-time protection temporarily
    → Try on different network
    → Whitelist OfficeAutomator in firewall/proxy
    → Retry OfficeAutomator
    
  If issue persists:
    → Network team should investigate proxy/firewall rules
    → Escalate to: IT Network (proxy/firewall investigation)
```

---

#### **OFF-SECURITY-102: Certificate Chain Invalid (Permanent)**

```
Category: SECURITY
Severity: CRITICAL
Retry Policy: PERMANENT (0x retry)
State: VALIDATE (Step 4)

TRIGGER CONDITION:
  • Microsoft certificate chain invalid or expired
  • Certificate not trusted by system
  • Downloaded hash signed by untrusted certificate
  • Certificate revocation check failed

DETECTION POINT:
  UC-004: ConfigValidator.ValidateOfficeHash() (Step 4)
    During certificate validation of hash signature

WHEN IT OCCURS:
  Probability: VERY LOW (system certificates maintained by Windows Update)
  Typical Cause: System clock wrong, certificate revoked, malware infection

ERROR RESULT OBJECT:
  {
    code: "OFF-SECURITY-102",
    message: "Microsoft Office certificate validation failed",
    technicalDetails: "Certificate chain invalid or untrusted",
    suggestedAction: "Contact IT Support - potential security issue"
  }

RECOVERY ACTION:
  1. ErrorHandler.HandleError() routes to PERMANENT path
  2. CRITICAL: Do NOT proceed with installation
  3. Log error with full certificate details
  4. Alert user to contact IT immediately
  5. Fail to INSTALL_FAILED state

USER-FACING MESSAGE:
  Title: "SECURITY WARNING: Certificate Validation Failed"
  Message: "OfficeAutomator cannot verify that the Office download came from Microsoft.
            This is a serious security issue and could indicate:
            • Your system time is incorrect
            • Malware or network tampering
            • Microsoft certificate revocation
            
            DO NOT PROCEED. Contact IT Support immediately."
  Action: [Contact IT] [Exit]

IT SUPPORT RUNBOOK:
  Issue: User reports "Certificate validation failed" (CRITICAL)
  
  IMMEDIATE ACTIONS:
    1. ISOLATE system from network (malware risk)
    2. DO NOT install any software
    3. Alert Security team
    4. Check system time:
       → System → Date & Time
       → Should match current date/time precisely
       → If wrong, correct and retry
    5. Check Windows Update certificate status:
       → Settings → Update & Security → Troubleshoot
    
  Investigation:
    1. Has system been compromised? Run antivirus/malware scan
    2. Are Windows certificates up to date? Run Windows Update
    3. Check Event Viewer for certificate errors
    4. Verify network is not intercepting/modifying downloads (MITM attack)
    
  Common Causes:
    • System clock significantly off (months/years difference)
    • Windows certificates not updated
    • Man-in-the-middle attack on network
    • Malware certificate injection
    
  Resolution (in order):
    → Correct system clock if wrong
    → Run Windows Update to update certificates
    → Run malware scan
    → Test on different network
    → Contact Microsoft if certificate revoked
    
  ESCALATION:
    → Always escalate to: IT Security (potential compromise)

ESCALATION CRITERIA:
  • Certificate validation fails with correct system time
  • Certificate chain invalid from Microsoft
  → Escalate immediately to: IT Security
```

---

### CATEGORY: SYSTEM (4 Codes, Including Fallback)

#### **OFF-SYSTEM-201: System Resource Lock or Timeout (Transient)**

```
Category: SYSTEM
Severity: MEDIUM
Retry Policy: TRANSIENT (1x retry with 2s backoff)
State: VALIDATE or INSTALLING

TRIGGER CONDITION:
  • Registry key locked by another process (concurrent install running)
  • File system lock (another app reading/writing same file)
  • Validation timeout: >1000ms elapsed in UC-004
  • Installation timeout: >1200000ms elapsed in UC-005

DETECTION POINT:
  UC-004: ConfigValidator.Execute() timeout check (1000ms hard limit)
  UC-005: InstallationExecutor.Execute() timeout check (20min limit)

WHEN IT OCCURS:
  Probability: MEDIUM (other apps may lock registry)
  Typical Cause: Another Office install running, OneDrive sync, explorer.exe holding files

ERROR RESULT OBJECT:
  {
    code: "OFF-SYSTEM-201",
    message: "System resource temporarily unavailable",
    technicalDetails: "Registry/file locked by another process or timeout occurred",
    suggestedAction: "Close other applications and retry"
  }

RECOVERY ACTION:
  1. ErrorHandler.HandleError() routes to TRANSIENT path
  2. Wait 2 seconds (backoff)
  3. Retry operation 1 time
  4. If retry succeeds: continue
  5. If retry fails: escalate to OFF-INSTALL-401 or OFF-SYSTEM-203

AUTOMATIC RETRY:
  1. Detect lock/timeout
  2. Wait 2 seconds (give other process time to release lock)
  3. Retry the operation (Step X of validation/installation)
  4. If succeeds: continue normally
  5. If fails again: error

USER-FACING MESSAGE:
  Title: "System Resources Busy"
  Message: "OfficeAutomator detected that system resources are temporarily busy.
            This might mean another Office installation is running or a file is in use.
            
            Waiting 2 seconds... Retrying...
            
            If this continues, please close other Office applications and retry."
  Status: Shows "Retrying..." with progress

IT SUPPORT RUNBOOK:
  Issue: User reports "System resource lock" error
  
  Troubleshooting:
    1. Check if another Office installation is running:
       → Task Manager → Processes → search for "setup.exe"
       → If found: Wait for it to complete or kill it
    2. Check if OneDrive sync is active:
       → OneDrive → Menu → Pause sync for 2 hours
    3. Close all Office applications (Word, Excel, etc.)
    4. Close browser (may lock files)
    5. Restart explorer.exe:
       → Task Manager → Processes → explorer.exe → Restart
    6. Retry OfficeAutomator
    
  Common Causes:
    • Another Office install running (first finishes, blocks second)
    • OneDrive sync holds files
    • Windows Update installing at same time
    • Antivirus scanning files during install
    
  Resolution:
    → Close conflicting applications
    → Wait for other processes to complete
    → Pause antivirus real-time protection temporarily
    → Retry OfficeAutomator
    
  If timeout continues:
    → Timeout might indicate performance issue (slow disk, high CPU)
    → Check Task Manager for high CPU/Disk activity
    → Escalate to: IT Support (system performance)
```

---

#### **OFF-SYSTEM-202: Insufficient Disk Space (Permanent)**

```
Category: SYSTEM
Severity: HIGH
Retry Policy: PERMANENT (0x retry)
State: INSTALLING

TRIGGER CONDITION:
  • Available disk space < 10GB (Office installation needs ~10-15GB)
  • Disk fills up during installation (download + install ongoing)
  • Temp folder also on same disk and fills up

DETECTION POINT:
  UC-005: InstallationExecutor.VerifyPrerequisites() checks available space

WHEN IT OCCURS:
  Probability: MEDIUM (large installation, small SSD)
  Typical Cause: Bloated C: drive, large temp files, user files consuming space

ERROR RESULT OBJECT:
  {
    code: "OFF-SYSTEM-202",
    message: "Insufficient disk space",
    technicalDetails: "Available space < 10GB required for Office installation",
    suggestedAction: "Free up disk space and retry"
  }

RECOVERY ACTION:
  1. ErrorHandler.HandleError() routes to PERMANENT path
  2. Display space requirement and current available space
  3. Provide cleanup suggestions
  4. Return to INSTALLING state (user can retry after cleanup)

USER-FACING MESSAGE:
  Title: "Insufficient Disk Space"
  Message: "Office installation requires at least 10 GB of free disk space.
            You currently have {X} GB available.
            
            To free up space, you can:
            • Delete temporary files
            • Uninstall unused applications
            • Move large files to external storage
            
            After freeing space, please retry."
  Action: [Retry] [Open Disk Cleanup] [Exit]

IT SUPPORT RUNBOOK:
  Issue: User reports "Insufficient disk space" error
  
  Troubleshooting:
    1. Check current disk usage:
       → File Explorer → This PC → C: drive → right-click → Properties
       → Shows used space vs. total space
    2. Identify large files using Disk Usage Analyzer:
       → Windows: TreeSize or WinDirStat (free tools)
       → Look for files > 1GB
    3. Common space hogs:
       • Recycle Bin (empty it: Delete → Recycle Bin → Empty)
       • Windows.old folder (if after Windows upgrade)
       • Temp files: C:\Users\{user}\AppData\Local\Temp
       • Downloaded files: Downloads folder
       • Log files: %APPDATA%\OfficeAutomator\logs\
    
  Cleanup Steps:
    1. Empty Recycle Bin: Right-click → Empty Recycle Bin
    2. Run Disk Cleanup: Settings → System → Storage → Temporary files → Delete
    3. Delete Windows.old if present: Settings → System → Storage → Delete
    4. Clear Temp folder: %TEMP% → Select all → Delete
    5. Clear Downloads (after moving important files)
    6. Restart system
    7. Retry OfficeAutomator
    
  Target: Free up 15GB minimum (10GB for Office + 5GB buffer)
  
  Resolution:
    → User frees up disk space
    → Retry OfficeAutomator
    
  If disk space insufficient and cannot free up:
    → Suggest installing on different drive (D:, E:, etc.)
    → Or upgrade disk (add SSD)
    → Escalate to: IT Hardware (disk upgrade planning)
```

---

#### **OFF-SYSTEM-203: Administrator Rights Required (Permanent)**

```
Category: SYSTEM
Severity: HIGH
Retry Policy: PERMANENT (0x retry)
State: INSTALLING

TRIGGER CONDITION:
  • OfficeAutomator not running as Administrator
  • Process elevated privileges required for:
    • Writing to Program Files folder
    • Modifying HKLM registry
    • Creating Windows services
    • Installing COM objects

DETECTION POINT:
  UC-005: InstallationExecutor.VerifyPrerequisites() checks IsRunningAsAdmin()

WHEN IT OCCURS:
  Probability: HIGH (common user configuration issue)
  Typical Cause: User double-clicked OfficeAutomator.exe instead of "Run as Administrator"

ERROR RESULT OBJECT:
  {
    code: "OFF-SYSTEM-203",
    message: "Administrator rights required",
    technicalDetails: "OfficeAutomator must run with administrator privileges",
    suggestedAction: "Run OfficeAutomator as Administrator"
  }

RECOVERY ACTION:
  1. ErrorHandler.HandleError() routes to PERMANENT path
  2. Display instructions on how to run as Administrator
  3. Exit gracefully (do not proceed)
  4. User must restart with admin rights

USER-FACING MESSAGE:
  Title: "Administrator Rights Required"
  Message: "OfficeAutomator requires administrator rights to install Office.
            
            To run as Administrator:
            1. Close OfficeAutomator
            2. Right-click OfficeAutomator.exe
            3. Select 'Run as Administrator'
            4. Click 'Yes' if prompted by User Account Control
            5. Start the installation again
            
            Note: Your organization's IT policy may require approval before granting admin rights."
  Action: [Exit] [More Information]

IT SUPPORT RUNBOOK:
  Issue: User reports "Administrator rights required" error
  
  Troubleshooting:
    1. Confirm user account type:
       → Settings → Accounts → Your info
       → Should show "Administrator" account type
    2. Check if UAC (User Account Control) is preventing elevation:
       → Settings → Update & Security → Security & Maintenance
       → Check if UAC notifications are disabled
    3. Try running with explicit admin rights:
       → Right-click OfficeAutomator.exe
       → Select "Run as Administrator"
       → Click "Yes" when UAC prompts
    
  If user account is not Administrator:
    → User must contact their manager or IT Help Desk
    → Request admin rights or software install request
    → Standard users cannot install Office
    
  If admin account but still fails:
    → UAC may be preventing elevation
    → Try: Settings → User Account Control → Change settings (slider to lowest)
    → Or: Run batch file with "@echo off" and ShellExecute with admin rights
    
  Resolution:
    → User runs OfficeAutomator as Administrator
    → Retry installation
    
  ESCALATION:
    → If user needs admin rights but doesn't have them
    → Escalate to: IT Help Desk (access request)
```

---

#### **OFF-SYSTEM-999: Unknown or Fallback Error (Permanent)**

```
Category: SYSTEM (Fallback)
Severity: CRITICAL (unknown cause)
Retry Policy: PERMANENT (0x retry)
State: Any state (error occurred but not caught by specific handlers)

TRIGGER CONDITION:
  • Unhandled exception in any UC
  • Error code not recognized or not mapped
  • Unexpected system condition (not covered by other error codes)
  • Data corruption or memory corruption

DETECTION POINT:
  UC-004 or UC-005: ErrorHandler.HandleError() receives unrecognized error code
  OR: Unhandled exception in try/catch block

WHEN IT OCCURS:
  Probability: VERY LOW (all foreseeable errors mapped to specific codes)
  Typical Cause: Programming bug, corrupt memory, hardware failure

ERROR RESULT OBJECT:
  {
    code: "OFF-SYSTEM-999",
    message: "An unexpected error occurred",
    technicalDetails: "{Original exception message}",
    suggestedAction: "Contact IT Support with error details"
  }

RECOVERY ACTION:
  1. ErrorHandler.HandleError() routes to PERMANENT path
  2. Log full exception details (call stack, context)
  3. Display error code and "Contact IT" message
  4. Save diagnostic information to log file
  5. Exit gracefully

USER-FACING MESSAGE:
  Title: "Unexpected Error"
  Message: "OfficeAutomator encountered an unexpected error: OFF-SYSTEM-999
            
            Error Details: {Error message and code}
            
            Please contact IT Support and provide the error code and any details shown here.
            
            Log File Location: {Path to error log}
            
            This will help us diagnose and fix the issue."
  Action: [Copy Error Details] [Open Log File] [Exit]

IT SUPPORT RUNBOOK:
  Issue: User reports "Unknown error OFF-SYSTEM-999"
  
  CRITICAL: This indicates an unexpected condition, not a normal error path
  
  Information to Collect:
    1. Error message text (copy from user)
    2. Log file contents (Location: %APPDATA%\OfficeAutomator\logs\)
    3. OfficeAutomator version number
    4. Windows version and build number
    5. Office version being installed
    6. System specs (RAM, Disk, CPU)
    7. Steps taken before error
    8. Any recent system changes (Windows Update, driver update, etc.)
    
  Initial Troubleshooting:
    1. Restart system (may clear transient memory issues)
    2. Clear %APPDATA%\OfficeAutomator\ folder and retry
    3. Try on different user account
    4. Try on different system (if available)
    5. Check Windows Event Viewer for system errors
    
  Investigation:
    1. Read log file and exception details
    2. Identify step where error occurred
    3. Check for:
       • File system corruption (chkdsk)
       • Memory issues (Windows Memory Diagnostic)
       • Hardware issues (monitor temps, disk health)
       • Malware (run antivirus scan)
    4. If reproducible: Create case for OfficeAutomator development team
    
  Resolution:
    → Restart system, try again
    → Clear AppData and try again
    → If persists: Escalate with full log details
    
  ESCALATION:
    → Always escalate OFF-SYSTEM-999 with full diagnostics
    → Escalate to: OfficeAutomator Development Team + IT Support
```

---

### CATEGORY: NETWORK (3 Codes)

#### **OFF-NETWORK-301: Download Failed - Network Error (Transient)**

```
Category: NETWORK
Severity: MEDIUM
Retry Policy: TRANSIENT (3x retry with backoff: 2s, 4s, 6s)
State: INSTALLING (Step 2: DownloadOffice)

TRIGGER CONDITION:
  • Network connection lost during download
  • Connection timeout (no response from server for 30+ seconds)
  • Server returned 5xx error (Server error, not client error)
  • Download interrupted (partial file received)

DETECTION POINT:
  UC-005: InstallationExecutor.DownloadOffice() network call fails

WHEN IT OCCURS:
  Probability: MEDIUM (network reliability varies)
  Typical Cause: WiFi dropout, network latency, router reset, ISP issue

ERROR RESULT OBJECT:
  {
    code: "OFF-NETWORK-301",
    message: "Network error during Office download",
    technicalDetails: "Connection lost or timeout during download",
    suggestedAction: "Check network and retry (automatic retry in progress)"
  }

RECOVERY ACTION:
  1. ErrorHandler.HandleError() routes to TRANSIENT path
  2. Delete partial/corrupt download
  3. Retry with backoff: 2s, 4s, 6s
  4. If all retries exhausted: fail with OFF-INSTALL-401

AUTOMATIC RETRY:
  Attempt 1: Wait 2s, retry download
  Attempt 2: Wait 4s, retry download
  Attempt 3: Wait 6s, retry download
  Failure: OFF-INSTALL-401

USER-FACING MESSAGE:
  Title: "Network Connection Issue"
  Message: "OfficeAutomator encountered a network problem while downloading Office.
            
            Checking connection... Retrying (attempt {X} of 3)
            
            • Check if you're connected to network
            • Restart WiFi router if using wireless
            • Wired connection may be more reliable
            
            Retrying in {countdown}s..."
  Status: Shows countdown timer and attempt number

IT SUPPORT RUNBOOK:
  Issue: User reports "Network error during download" (after 3 retries)
  
  Troubleshooting:
    1. Test network connectivity:
       → Open browser, visit https://www.microsoft.com/
       → Should load successfully
    2. Check connection type:
       → WiFi: May have interference or weak signal
       → Wired: Check Ethernet cable connection
    3. Test speed:
       → Use speedtest.net to check connection speed
       → Office download typically needs >1 Mbps
    4. Check for network issues:
       → Ping google.com: Should get response
       → Check Windows Network Diagnostics
    5. Restart network device:
       → Restart WiFi router (5 minute power cycle)
       → Reconnect to network
    
  Common Causes:
    • Unstable WiFi (router interference, distance)
    • ISP issues (temporary outage)
    • Firewall/proxy blocking download
    • Network timeout (large file, slow connection)
    
  Resolution:
    → Ensure stable network connection
    → Use wired connection if WiFi unreliable
    → Disable VPN if using one (may slow download)
    → Pause antivirus real-time protection (may interfere)
    → Move closer to WiFi router
    → Retry OfficeAutomator
    
  If network is confirmed working:
    → Try on different network (home vs. work)
    → Contact IT if corporate network is slow
    → Escalate to: IT Network (if corporate network issue)
```

---

#### **OFF-NETWORK-302: Connection Timeout (Transient)**

```
Category: NETWORK
Severity: MEDIUM
Retry Policy: TRANSIENT (3x retry with backoff: 2s, 4s, 6s)
State: INSTALLING or VALIDATE (network calls)

TRIGGER CONDITION:
  • No response from Microsoft CDN for >30 seconds
  • DNS resolution timeout
  • Firewall blocks connection (connection silently dropped)
  • Network latency too high (packet loss)

DETECTION POINT:
  UC-004: ConfigValidator.ValidateOfficeHash() network timeout
  UC-005: InstallationExecutor.DownloadOffice() timeout

WHEN IT OCCURS:
  Probability: MEDIUM (network latency)
  Typical Cause: High latency network, firewall rules, DNS slowness, VPN

ERROR RESULT OBJECT:
  {
    code: "OFF-NETWORK-302",
    message: "Network connection timeout",
    technicalDetails: "No response from server within timeout period",
    suggestedAction: "Check network and retry"
  }

RECOVERY ACTION:
  1. ErrorHandler.HandleError() routes to TRANSIENT path
  2. Wait backoff interval
  3. Retry network operation
  4. If all retries exhausted: fail

AUTOMATIC RETRY:
  Attempt 1: Wait 2s, retry
  Attempt 2: Wait 4s, retry
  Attempt 3: Wait 6s, retry
  Failure: OFF-INSTALL-401 or OFF-SYSTEM-201

USER-FACING MESSAGE:
  Title: "Network Slow or Unresponsive"
  Message: "The network is responding slowly or the server is not responding.
            
            OfficeAutomator will retry automatically...
            
            Tips:
            • Check if other large downloads are running
            • Restart WiFi router
            • Move closer to router
            • Try using wired connection
            
            Retry attempt {X} of 3 in {countdown}s..."

IT SUPPORT RUNBOOK:
  Issue: User reports "Connection timeout" (after 3 retries)
  
  Troubleshooting:
    1. Check if Microsoft servers are accessible:
       → ping microsoft.com
       → nslookup download.microsoft.com (test DNS)
    2. Check network latency:
       → ping microsoft.com (look at "time" values)
       → Should be < 100ms; if > 200ms: high latency
    3. Check for network issues:
       → Run Windows Network Diagnostics
       → Check firewall rules (may block CDN)
    4. Test from different location:
       → Mobile hotspot
       → Different WiFi
       → Wired connection
    5. Check if VPN is in use:
       → VPN may add latency or be misconfigured
       → Try disabling VPN
    
  Common Causes:
    • VPN misconfiguration (high latency)
    • Firewall silently dropping packets
    • Corporate proxy slow or overloaded
    • ISP DNS slow (try 8.8.8.8)
    • High network latency (satellite, remote location)
    
  Resolution:
    → Check network latency (should be < 100ms)
    → Disable VPN if possible
    → Use different DNS (8.8.8.8 or 8.8.4.4)
    → Try wired connection
    → Contact network provider if ISP latency high
    → Escalate to: IT Network (if corporate network latency issue)
```

---

### CATEGORY: INSTALL (3 Codes)

#### **OFF-INSTALL-401: Office Setup Failure (Permanent)**

```
Category: INSTALL
Severity: HIGH
Retry Policy: PERMANENT (0x retry, triggers rollback)
State: INSTALLING → INSTALL_FAILED → Rollback

TRIGGER CONDITION:
  • setup.exe exited with non-zero code (typically exit code 1)
  • setup.exe crashed during execution
  • setup.exe failed to write critical registry keys
  • Office critical files not copied to Program Files
  • setup.exe timeout (exceeded 20 minute limit)

DETECTION POINT:
  UC-005: InstallationExecutor.Execute() checks setup.exe exit code
  UC-005: InstallationExecutor.ExecuteSetup() returns non-zero exit code

WHEN IT OCCURS:
  Probability: MEDIUM (Office setup is complex)
  Typical Cause: Corrupted Office binaries, registry permissions, conflicting app

ERROR RESULT OBJECT:
  {
    code: "OFF-INSTALL-401",
    message: "Office installation failed",
    technicalDetails: "setup.exe exited with code {N}. {Details}",
    suggestedAction: "System will be cleaned up. Please retry."
  }

RECOVERY ACTION:
  1. ErrorHandler.HandleError() routes to PERMANENT path
  2. Immediately trigger RollbackExecutor
  3. RollbackExecutor executes 3-part rollback:
     • Part 1: Remove Office files
     • Part 2: Clean registry
     • Part 3: Remove shortcuts
  4. Transition to INSTALL_FAILED → (rollback success) ROLLED_BACK
  5. User can retry from INIT

AUTOMATIC ROLLBACK:
  Triggered immediately on OFF-INSTALL-401
  Runs atomically (all 3 parts or abort)

USER-FACING MESSAGE (During Rollback):
  Title: "Office Installation Failed"
  Message: "The Office installation did not complete successfully.
            OfficeAutomator is cleaning up the system...
            
            Setup Exit Code: {N}
            
            After cleanup, you can retry installation."
  Status: Shows cleanup progress

IT SUPPORT RUNBOOK:
  Issue: User reports "Office installation failed"
  
  Investigation:
    1. Check Office installation log:
       → Location: %TEMP%\Office Installation Logs (created by setup.exe)
       → Contains detailed error messages
    2. Check setup exit code:
       → Common codes:
         • 1: General error
         • 30088: Admin rights issue (but OFF-SYSTEM-203 should catch this)
         • Others: Office-specific errors
    3. Check Windows Event Viewer:
       → System logs for critical errors during installation
       → Application logs for Office errors
    
  Common Causes:
    • Corrupted Office download (hash should have caught, but might not)
    • Registry write permission denied (even with admin)
    • Conflicting software (antivirus interfering)
    • Insufficient disk space (during installation, even though checked)
    • Previous incomplete Office installation
    
  Troubleshooting:
    1. Check if rollback completed successfully
       → Verify C:\Program Files\Microsoft Office is empty
       → Verify registry is clean (regedit: check if Office keys gone)
    2. Try again:
       → Run OfficeAutomator again
       → Select same versions/languages
       → System should have been cleaned
    3. If fails again:
       → Check Office log file for specific error
       → Disable antivirus real-time protection temporarily
       → Close other Office applications
       → Restart system
       → Try again
    4. If still fails:
       → May indicate download corruption
       → Clear cache: Delete %APPDATA%\OfficeAutomator\cache\{version}
       → Retry download and installation
    
  Advanced Troubleshooting:
    → Check if .NET framework corrupted (Office depends on it)
    → Check if Windows Installer service running
    → Check for pending Windows updates
    → Try installing on local drive (not network drive)
    
  Resolution:
    → System cleaned (rollback successful)
    → User retries installation
    → If repeatable: Escalate with Office log details
    
  ESCALATION:
    → If install fails 3+ times despite cleanup
    → Escalate to: IT Support + Microsoft Office support
```

---

#### **OFF-INSTALL-402: Office Already Installed (Info)**

```
Category: INSTALL
Severity: INFO
Retry Policy: INFORMATIONAL (not an error, no retry needed)
State: VALIDATE (Step 6: Idempotence check)

TRIGGER CONDITION:
  • Office already installed on system (registry key detected)
  • User attempts to run OfficeAutomator again
  • Office installation detected during validation

DETECTION POINT:
  UC-004: ConfigValidator.ValidateIdempotence() (Step 6)
    Checks HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Office\RegistrationDB

WHEN IT OCCURS:
  Probability: HIGH (user may retry or run multiple times)
  Typical Cause: Normal - user already has Office, runs OfficeAutomator again

ERROR RESULT OBJECT:
  {
    code: "OFF-INSTALL-402",
    message: "Office is already installed",
    technicalDetails: "Office installation detected in registry",
    suggestedAction: "No action needed. Office is ready to use."
  }

RECOVERY ACTION:
  1. ErrorHandler.HandleError() routes to INFORMATIONAL path
  2. This is NOT an error - Office is already installed
  3. Continue validation normally (do NOT skip remaining steps)
  4. If user clicks "Proceed" in confirmation:
     → setup.exe will detect existing installation
     → setup.exe may update/repair existing installation
     → OR setup.exe may skip and exit cleanly (0)

USER-FACING MESSAGE:
  Title: "Office Is Already Installed"
  Message: "Microsoft Office is already installed on this system.
            
            You can:
            • Click [Finish] to exit (use existing Office)
            • Click [Repair] to repair the installation
            • Click [Change] to modify installed features
            
            The validation will proceed and offer these options."
  Action: [Finish] [Repair] [Change]

IT SUPPORT RUNBOOK:
  Issue: User sees "Office is already installed" message
  
  Context:
    → This is informational, not an error
    → Office is working correctly
    → User can exit OfficeAutomator
    
  User Scenarios:
    1. User wants to exit: [Finish] button
       → OfficeAutomator closes
       → User has working Office
    
    2. User wants to repair Office: [Repair] button
       → Proceed with installation
       → setup.exe in repair mode
       → Validates/fixes existing Office
    
    3. User wants to change features: [Change] button
       → Proceed with installation
       → setup.exe in modify mode
       → User can add/remove Office apps
    
  Resolution:
    → User can exit (Office already working)
    → OR user proceeds to repair/change as needed
    
  No escalation needed (informational only)
```

---

#### **OFF-INSTALL-403: Installation Corrupted or Incomplete (Permanent)**

```
Category: INSTALL
Severity: HIGH
Retry Policy: PERMANENT (0x retry, triggers rollback)
State: INSTALLING → INSTALL_FAILED → Rollback

TRIGGER CONDITION:
  • Critical Office files missing after setup.exe exits 0
  • File sizes incorrect (Word.exe, Excel.exe, etc.)
  • Required registry keys not created
  • Office installation partially copied but incomplete

DETECTION POINT:
  UC-005: InstallationExecutor.ValidateInstallation()
    Checks for critical files:
      • Word.exe, Excel.exe, PowerPoint.exe in Program Files
      • Registry key: HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Office\RegistrationDB

WHEN IT OCCURS:
  Probability: LOW (setup.exe is reliable)
  Typical Cause: Disk corruption, sudden power loss during install, file system error

ERROR RESULT OBJECT:
  {
    code: "OFF-INSTALL-403",
    message: "Office installation is corrupted or incomplete",
    technicalDetails: "Critical files missing or invalid: {list}",
    suggestedAction: "System will be cleaned up. Please retry."
  }

RECOVERY ACTION:
  1. ErrorHandler.HandleError() routes to PERMANENT path
  2. Immediately trigger RollbackExecutor (same as OFF-INSTALL-401)
  3. Remove partial/corrupted Office installation
  4. Clean registry of incomplete entries
  5. User can retry from INIT

AUTOMATIC ROLLBACK:
  Triggered immediately, 3-part process

USER-FACING MESSAGE:
  Title: "Office Installation Corrupted"
  Message: "The Office installation appears to be corrupted or incomplete.
            Critical files are missing.
            
            OfficeAutomator is cleaning up the system...
            
            After cleanup, you can retry installation."
  Status: Shows cleanup progress

IT SUPPORT RUNBOOK:
  Issue: User reports "Installation corrupted" error
  
  Investigation:
    1. Check disk health:
       → Run chkdsk C: /F (requires reboot)
       → Check for bad sectors or corruption
    2. Check if power loss occurred:
       → Ask user: "Did system lose power during install?"
       → If yes: Disk corruption likely
    3. Check Event Viewer:
       → System logs for disk errors
       → Critical events during installation timeframe
    
  Common Causes:
    • Disk I/O error (bad sector, USB disconnect)
    • Power loss during installation
    • File system corruption
    • Antivirus quarantined critical files
    
  Troubleshooting:
    1. Verify rollback completed:
       → Check Program Files\Microsoft Office is empty
       → Regedit: Verify Office keys removed
    2. Check system reliability:
       → Run chkdsk C: /F (reboot required)
       → Run DISM /Online /Cleanup-Image /RestoreHealth (repair Windows)
    3. Retry installation:
       → After disk check/repair
       → Run OfficeAutomator again
    4. If fails again:
       → Likely persistent disk issue
       → Escalate to: IT Hardware (disk replacement planning)
    5. Workaround:
       → Install on different drive (D:, E:, etc.)
       → If available, try external SSD
    
  Resolution:
    → System cleaned
    → Disk repaired if corrupted
    → User retries installation
    → If persists: Hardware replacement needed
    
  ESCALATION:
    → If corruption persists after disk repair
    → Escalate to: IT Hardware (disk/system health)
```

---

### CATEGORY: ROLLBACK (3 Codes)

#### **OFF-ROLLBACK-501: File Removal Failed (Permanent)**

```
Category: ROLLBACK
Severity: CRITICAL
Retry Policy: PERMANENT (0x retry, no recovery)
State: INSTALL_FAILED (RollbackExecutor Part 1)

TRIGGER CONDITION:
  • Cannot delete Program Files\Microsoft Office folder
  • Cannot delete AppData\Microsoft\Office folder
  • Files locked by running process (can't force delete)
  • Permission denied on critical files
  • Disk write-protected

DETECTION POINT:
  UC-005: RollbackExecutor.RemoveOfficeFiles() returns failure

WHEN IT OCCURS:
  Probability: LOW (Robocopy with /PURGE usually works)
  Typical Cause: Office process still running, permission issue, system file

ERROR RESULT OBJECT:
  {
    code: "OFF-ROLLBACK-501",
    message: "Could not remove Office installation files",
    technicalDetails: "Failed to delete Office folder or critical files",
    suggestedAction: "Contact IT Support for manual cleanup"
  }

RECOVERY ACTION:
  1. RollbackExecutor detects Part 1 failure
  2. Continues with Part 2 and Part 3 anyway (complete what can be)
  3. Returns failure status (not all 3/3 parts succeeded)
  4. Transitions to INSTALL_FAILED (CRITICAL)
  5. User cannot retry, must contact IT

CRITICAL STATE:
  System stuck in INSTALL_FAILED
  Office partially installed, partially rolled back (inconsistent state)
  Requires manual IT intervention

USER-FACING MESSAGE:
  Title: "System Cleanup Error - CRITICAL"
  Message: "OfficeAutomator could not fully clean up the system after installation failure.
            
            This is a critical issue that requires IT Support.
            
            DO NOT restart system or run any Office utilities.
            
            Please contact IT Support immediately with this error code:
            OFF-ROLLBACK-501
            
            IT may need to manually clean Office files from your system."
  Action: [Contact IT] [Exit]

IT SUPPORT RUNBOOK:
  Issue: User reports "File removal failed" error (OFF-ROLLBACK-501) - CRITICAL
  
  CRITICAL: System in inconsistent state
  DO NOT attempt normal troubleshooting
  MUST perform manual cleanup
  
  Immediate Actions:
    1. Ask user to RESTART system (may release locked files)
    2. After restart, attempt manual cleanup:
       → Boot in Safe Mode (if necessary)
       → Manually delete C:\Program Files\Microsoft Office (if exists)
       → If access denied: Take ownership of folder
    3. Alternative: Reinstall OS on affected partition (if critical)
    
  Manual Cleanup Procedure:
    1. Restart system in Safe Mode (if files still locked):
       → Restart → Troubleshoot → Advanced Options → Startup Settings
       → Press 4 for Safe Mode
    2. Delete Office folders:
       → C:\Program Files\Microsoft Office
       → %APPDATA%\Microsoft\Office
    3. If access denied: Take ownership:
       → Right-click folder → Properties → Security → Advanced
       → Change Owner to Administrators
       → Apply recursively
       → Retry delete
    4. Clean registry:
       → Run regedit as Administrator
       → Delete: HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Office
       → Delete: HKEY_CURRENT_USER\SOFTWARE\Microsoft\Office
    5. Restart system (normal mode)
    
  Prevention:
    → Close all Office applications before running OfficeAutomator
    → Close OneDrive sync
    → Disable antivirus real-time protection temporarily
    
  Investigation:
    1. What files couldn't be deleted?
    2. Were Office applications running?
    3. Check Event Viewer for access denied errors
    4. Check antivirus logs (may have quarantined files)
    
  ESCALATION:
    → Always escalate OFF-ROLLBACK-501
    → Escalate to: IT Support (manual cleanup required)
```

---

#### **OFF-ROLLBACK-502: Registry Cleanup Failed (Permanent)**

```
Category: ROLLBACK
Severity: CRITICAL
Retry Policy: PERMANENT (0x retry)
State: INSTALL_FAILED (RollbackExecutor Part 2)

TRIGGER CONDITION:
  • Cannot delete HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Office
  • Cannot delete HKEY_CURRENT_USER\SOFTWARE\Microsoft\Office
  • Registry key owned by TrustedInstaller (cannot delete without special bypass)
  • Registry key locked by running process
  • Permission denied even with admin rights

DETECTION POINT:
  UC-005: RollbackExecutor.CleanRegistry() returns failure

WHEN IT OCCURS:
  Probability: LOW (most keys are deletable with admin rights)
  Typical Cause: TrustedInstaller ownership, system protecting critical keys, process locked

ERROR RESULT OBJECT:
  {
    code: "OFF-ROLLBACK-502",
    message: "Could not clean Office registry entries",
    technicalDetails: "Failed to delete Office registry keys",
    suggestedAction: "Contact IT Support for manual cleanup"
  }

RECOVERY ACTION:
  1. RollbackExecutor detects Part 2 failure
  2. Continues with Part 3 (shortcuts)
  3. Returns failure status (not all 3/3 parts succeeded)
  4. Transitions to INSTALL_FAILED (CRITICAL)
  5. User must contact IT for manual registry cleanup

CRITICAL STATE:
  System stuck in INSTALL_FAILED
  Office registry entries remain (may block reinstall)
  Requires manual IT intervention

USER-FACING MESSAGE:
  Title: "System Cleanup Error - Registry - CRITICAL"
  Message: "OfficeAutomator could not clean the Office registry entries after installation failure.
            
            This is a critical issue that requires IT Support.
            
            DO NOT attempt to repair or reinstall Office.
            
            Please contact IT Support immediately with this error code:
            OFF-ROLLBACK-502
            
            IT will need to manually clean the registry."
  Action: [Contact IT] [Exit]

IT SUPPORT RUNBOOK:
  Issue: User reports "Registry cleanup failed" (OFF-ROLLBACK-502) - CRITICAL
  
  CRITICAL: System in inconsistent state
  Registry entries prevent clean reinstall
  MUST perform manual registry cleanup
  
  Manual Cleanup Procedure:
    1. Open Registry Editor as Administrator:
       → Press Windows + R
       → Type: regedit
       → Right-click → Run as Administrator
    2. Navigate to and delete:
       → HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Office
       → Right-click → Delete
       → Confirm
    3. Delete user hive:
       → HKEY_CURRENT_USER\SOFTWARE\Microsoft\Office
       → Right-click → Delete
       → Confirm
    4. If access denied: Check ownership
       → Right-click key → Permissions
       → Check if owner is TrustedInstaller
       → Click "Advanced" → "Change" → "Administrators"
       → Enable "Full Control"
       → Apply → OK
       → Retry delete
    5. If still denied:
       → Close all applications
       → Restart system
       → Try again (process may no longer be locked)
    
  Alternative (More Aggressive):
    If TrustedInstaller ownership preventing delete:
    → Use psexec to run regedit under SYSTEM account (not recommended)
    → Or use Registry Editor in Windows PE (advanced recovery)
    
  Verification:
    After deletion, verify:
    → regedit → navigate to Microsoft\Office
    → Should not find "Office" folder anymore
    → Or find it but empty
    
  ESCALATION:
    → Always escalate OFF-ROLLBACK-502
    → Escalate to: IT Support (manual registry cleanup)
```

---

#### **OFF-ROLLBACK-503: Partial Rollback Failure (Permanent)**

```
Category: ROLLBACK
Severity: CRITICAL (partial recovery)
Retry Policy: PERMANENT (0x retry)
State: INSTALL_FAILED (after RollbackExecutor Parts 1/2 fail, Part 3 fails)

TRIGGER CONDITION:
  • One or more of 3 rollback parts failed
  • Successful: OFF-ROLLBACK-501 OR OFF-ROLLBACK-502
  • Failed: Part 3 (shortcuts removal) also failed
  • Result: System in inconsistent state with partial Office remnants

DETECTION POINT:
  UC-005: RollbackExecutor.Execute() returns false
    Because: successCount != totalParts (not all 3 succeeded)

WHEN IT OCCURS:
  Probability: MEDIUM (if Parts 1 or 2 fail, Part 3 might also)
  Typical Cause: Multiple failure points (files, registry, shortcuts all problematic)

ERROR RESULT OBJECT:
  {
    code: "OFF-ROLLBACK-503",
    message: "System cleanup incomplete",
    technicalDetails: "Rollback succeeded for {X}/3 parts. {Details}",
    suggestedAction: "Contact IT Support for manual cleanup"
  }

RECOVERY ACTION:
  1. RollbackExecutor completed with partial success
  2. Transition to INSTALL_FAILED (CRITICAL)
  3. System is partially cleaned but inconsistent
  4. User must contact IT for complete cleanup

CRITICAL STATE:
  System has:
    • Some Office files deleted
    • Some Office files still present
    • Some registry entries cleaned
    • Some registry entries still present
    • Shortcuts may or may not be removed
  Result: Inconsistent and unusable

USER-FACING MESSAGE:
  Title: "System Cleanup Incomplete - CRITICAL"
  Message: "OfficeAutomator could not fully clean up the system.
            The cleanup completed {X} out of 3 steps.
            
            Your system is in an inconsistent state and Office cannot be properly reinstalled.
            
            IMPORTANT: Do not attempt to manually fix this.
            
            Please contact IT Support immediately with this error code:
            OFF-ROLLBACK-503
            
            Error Details: {Which parts failed}
            
            IT Support will perform complete manual cleanup."
  Action: [Contact IT] [View Details]

IT SUPPORT RUNBOOK:
  Issue: User reports "Partial cleanup failed" (OFF-ROLLBACK-503) - CRITICAL
  
  CRITICAL: Highest priority for resolution
  System unusable, incomplete cleanup
  Requires complete manual intervention
  
  Assessment (Check error details):
    Which parts failed? (User should provide details)
    • Part 1 (files) failed?
    • Part 2 (registry) failed?
    • Part 3 (shortcuts) failed?
    
  Complete Manual Cleanup Procedure:
    ========== PART 1: FILES ==========
    1. Delete Office folders:
       → C:\Program Files\Microsoft Office
       → C:\Program Files (x86)\Microsoft Office (if exists)
       → %APPDATA%\Microsoft\Office
       → %LOCALAPPDATA%\Microsoft\Office
    2. If access denied:
       → Boot in Safe Mode
       → Retry delete
    3. If still locked:
       → Use Unlocker tool (freeware)
       → Or use Robocopy /PURGE in administrator command prompt
    
    ========== PART 2: REGISTRY ==========
    1. Open regedit as Administrator
    2. Delete (if exists):
       → HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Office
       → HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\Office (32-bit)
       → HKEY_CURRENT_USER\SOFTWARE\Microsoft\Office
       → HKEY_USERS\{SID}\SOFTWARE\Microsoft\Office (all user hives)
    3. If ownership issues:
       → Right-click → Advanced → Change Owner → Administrators
       → Enable Full Control → Apply → Retry delete
    
    ========== PART 3: SHORTCUTS ==========
    1. Delete from Start Menu:
       → %PROGRAMDATA%\Microsoft\Windows\Start Menu\Programs\Microsoft Office\
       → Delete entire "Microsoft Office" folder
    2. Delete from Desktop:
       → %USERPROFILE%\Desktop\
       → Delete any Office shortcuts
    3. Clear Taskbar pinned items:
       → Right-click any pinned Office app
       → "Unpin from taskbar"
    
    ========== CLEANUP VERIFICATION ==========
    1. Reboot system
    2. Search for "Office" or "Word", "Excel", etc.
       → Should NOT find any Office applications
    3. Check Programs & Features:
       → Settings → Apps → Apps & features
       → Should NOT list Microsoft Office
    4. Check registry:
       → regedit → Microsoft\Office
       → Should NOT exist or be empty
    
  After Complete Cleanup:
    → System is ready for fresh Office installation
    → User can run OfficeAutomator again
    → Installation should proceed normally
    
  Prevention:
    → Close all applications before running OfficeAutomator
    → Ensure admin rights
    → Check disk/registry health
    → Disable antivirus temporarily
    
  ESCALATION:
    → OFF-ROLLBACK-503 is highest priority
    → Must be resolved before user can reinstall Office
    → Escalate to: IT Support (immediate manual cleanup)
    → May require IT staff to visit physically if system critical
```

---

## 3. Error Scenario Summary Table

```
ERROR CODE      CATEGORY    SEVERITY   RETRY    STATE              ACTION
──────────────────────────────────────────────────────────────────────────
OFF-CONFIG-001  CONFIG      HIGH       0x       SELECT_VERSION     Retry selection
OFF-CONFIG-002  CONFIG      MEDIUM     0x       SELECT_LANGUAGE    Retry selection
OFF-CONFIG-003  CONFIG      MEDIUM     0x       SELECT_APPS        Retry selection
OFF-CONFIG-004  CONFIG      HIGH       0x       VALIDATE           Regenerate config

OFF-SECURITY-101 SECURITY   HIGH       3x       VALIDATE Step 4    Auto-retry, fail
OFF-SECURITY-102 SECURITY   CRITICAL   0x       VALIDATE Step 4    Escalate immediately

OFF-SYSTEM-201  SYSTEM      MEDIUM     1x       VALIDATE/INST      Auto-retry, fail
OFF-SYSTEM-202  SYSTEM      HIGH       0x       INSTALLING         User frees space
OFF-SYSTEM-203  SYSTEM      HIGH       0x       INSTALLING         User runs as Admin
OFF-SYSTEM-999  SYSTEM      CRITICAL   0x       Any                 Escalate critical

OFF-NETWORK-301 NETWORK     MEDIUM     3x       INSTALLING         Auto-retry, fail
OFF-NETWORK-302 NETWORK     MEDIUM     3x       VALIDATE/INST      Auto-retry, fail

OFF-INSTALL-401 INSTALL     HIGH       0x       INSTALLING         Rollback → retry
OFF-INSTALL-402 INSTALL     INFO       —        VALIDATE           Informational only
OFF-INSTALL-403 INSTALL     HIGH       0x       INSTALLING         Rollback → retry

OFF-ROLLBACK-501 ROLLBACK   CRITICAL   0x       INSTALL_FAILED     Manual IT cleanup
OFF-ROLLBACK-502 ROLLBACK   CRITICAL   0x       INSTALL_FAILED     Manual IT cleanup
OFF-ROLLBACK-503 ROLLBACK   CRITICAL   0x       INSTALL_FAILED     Manual IT cleanup
```

---

## Document Metadata

```
Created: 2026-05-16 16:05
Task: T-027 Error Scenario Matrix & Recovery Paths
Version: 1.0.0
Story Points: 4
Duration: Initial design
Status: IN PROGRESS
Dependencies: T-020, T-021, T-026
Next: T-028 (Configuration Object Lifecycle)
Use: Complete error reference for Stage 10 and IT Help Desk
Quality Gate: All 19 errors documented
```

---

**T-027 IN PROGRESS**

**All 19 error codes with full context, recovery paths, user messages, and IT runbook ✓**

