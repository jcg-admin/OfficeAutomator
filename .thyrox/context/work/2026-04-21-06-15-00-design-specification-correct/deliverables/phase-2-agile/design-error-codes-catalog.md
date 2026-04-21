```yml
created_at: 2026-05-15 14:00
document_type: Reference Document - Error Codes Catalog
stage: Stage 7 - DESIGN/SPECIFY
work_package: 2026-04-21-06-15-00-design-specification-correct
phase: 2-Agile-Sprints
sprint_number: 1
task_id: T-021
task_name: Error Codes Catalog
execution_date: 2026-05-15 14:00 to 2026-05-15 18:00 (Wednesday afternoon)
duration_hours: 4
story_points: 3
roles_involved: BA (Claude)
dependencies: T-020 (error propagation strategy), T-006 (error codes)
deliverables:
  - Error code lookup table (18 codes)
  - Categorized error codes (5 categories)
  - User messages (clear, actionable)
  - Technical details (for support)
  - Recovery procedures (step-by-step)
  - Support troubleshooting guide
acceptance_criteria:
  - All 18 error codes documented
  - User message per code (one-liner)
  - Technical details per code (for support)
  - Recovery procedures step-by-step
  - Support lookup guide
  - Quick reference table
  - No contradictions with T-020 + T-006
status: READY FOR EXECUTION
version: 1.0.0
```

# ERROR CODES CATALOG

## Quick Reference Table

| Code | Category | Description | User Message | Retry? | Recovery |
|------|----------|-------------|--------------|--------|----------|
| OFF-CONFIG-001 | Configuration | Invalid version | Invalid Office version selected | No | Retry version selection |
| OFF-CONFIG-002 | Configuration | Unsupported language | Language not available for version | No | Retry language selection |
| OFF-CONFIG-003 | Configuration | Invalid app exclusion | Selected app cannot be excluded | No | Uncheck app, retry |
| OFF-CONFIG-004 | Configuration | Config file invalid | Configuration file is invalid | No | Reconfigure from SELECT_APPS |
| OFF-SECURITY-101 | Security | Hash verification failed | Cannot verify Office package | Yes (3x) | Retry or contact IT |
| OFF-SECURITY-102 | Security | Certificate invalid | Office package not authentic | No | Contact IT |
| OFF-SECURITY-103 | Security | Signature invalid | Office package signature invalid | No | Redownload or contact IT |
| OFF-SYSTEM-201 | System | File locked/access denied | System file in use | Yes (1x) | Close apps, retry or restart |
| OFF-SYSTEM-202 | System | Insufficient disk space | Not enough disk space | Yes (1x) | Free space, retry |
| OFF-SYSTEM-203 | System | Registry access denied | Cannot access registry | No | Run as Administrator |
| OFF-NETWORK-301 | Network | Connection timeout | Network timeout (retrying) | Yes (3x) | Check internet, retry |
| OFF-NETWORK-302 | Network | Connection failed | Network connection failed | Yes (3x) | Restart router, retry |
| OFF-NETWORK-303 | Network | Download incomplete | File download interrupted | Yes (3x) | Check internet, retry |
| OFF-INSTALL-401 | Installation | Setup.exe failed | Installation failed | No | Automatic rollback, contact IT |
| OFF-INSTALL-402 | Installation | Office already installed | Office already installed | No (success) | Continue (no-op) |
| OFF-INSTALL-403 | Installation | Installation corrupted | Installation is corrupted | No | Rollback, reinstall, contact IT |
| OFF-ROLLBACK-501 | Rollback | File removal failed | Cannot remove Office fully | No | Contact IT Help Desk |
| OFF-ROLLBACK-502 | Rollback | Registry cleanup failed | Cannot clean registry | No | Contact IT Help Desk |
| OFF-ROLLBACK-503 | Rollback | Rollback incomplete | System cleanup incomplete | No | Contact IT Help Desk |

---

## Configuration Errors (OFF-CONFIG-*)

### OFF-CONFIG-001: Invalid Version Selected

**Quick Info:**
- Error Code: `OFF-CONFIG-001`
- Category: Configuration (Permanent)
- Severity: Low
- Location: UC-001 (SELECT_VERSION)
- Retry: No (Permanent error)
- User Recoverable: Yes

**User Message:**
```
"Invalid Office version. Please select 2024, 2021, or 2019"
```

**Technical Details:**
```
Code: OFF-CONFIG-001
Condition: User selected version not in whitelist [2024, 2021, 2019]
Details: Version=[user_input] not in allowed_versions
Example: User selected "2020" which is not supported
Root Cause: User selection error or UI malfunction
Impact: Cannot proceed to next UC
```

**Recovery Procedure (User):**
1. Read error message: "Invalid Office version..."
2. Click back or retry button
3. Select valid version: 2024, 2021, or 2019
4. Click next to proceed to UC-002

**Support Troubleshooting:**
- Check if error repeats after version selection
- If repeats: May indicate UI issue or corrupted form
- Action: Restart application, try again
- If still fails: Check event logs for UI errors

---

### OFF-CONFIG-002: Unsupported Language

**Quick Info:**
- Error Code: `OFF-CONFIG-002`
- Category: Configuration (Permanent)
- Severity: Low
- Location: UC-002 (SELECT_LANGUAGE)
- Retry: No
- User Recoverable: Yes

**User Message:**
```
"Language not available for selected Office version"
```

**Technical Details:**
```
Code: OFF-CONFIG-002
Condition: Language not compatible with selected Office version
Details: Language=[lang] not in version_language_matrix[version]
Example: Language=Taiwanese selected for Office 2019 (not supported)
Root Cause: Version-language incompatibility
Impact: Cannot proceed to next UC
```

**Recovery Procedure (User):**
1. Read error message
2. Select language from available list shown on screen
3. Compatible languages for selected version: en-US, es-MX
4. Click next to proceed to UC-003

**Support Troubleshooting:**
- Verify user selected valid language for their version
- Language matrix: All versions support en-US, es-MX only (v1.0.0)
- If error persists: Check language list filtering in UI

---

### OFF-CONFIG-003: Invalid App Exclusion

**Quick Info:**
- Error Code: `OFF-CONFIG-003`
- Category: Configuration (Permanent)
- Severity: Low
- Location: UC-003 (SELECT_APPS)
- Retry: No
- User Recoverable: Yes

**User Message:**
```
"Selected application cannot be excluded"
```

**Technical Details:**
```
Code: OFF-CONFIG-003
Condition: Selected app not in exclusion whitelist
Details: App=[app_name] not in whitelist [Teams, OneDrive, Groove, Lync, Bing]
Example: User tried to exclude "Access" which is not in whitelist
Root Cause: User selection of unsupported app exclusion
Impact: Cannot proceed to next UC
```

**Recovery Procedure (User):**
1. Read error message
2. Uncheck the invalid application
3. Select only from allowed apps: Teams, OneDrive, Groove, Lync, Bing
4. Click next to proceed to UC-004

**Support Troubleshooting:**
- Verify user only selected from whitelist apps
- If error repeats: May indicate UI checkbox persistence issue
- Action: Restart application, reselect apps

---

### OFF-CONFIG-004: Configuration File Invalid

**Quick Info:**
- Error Code: `OFF-CONFIG-004`
- Category: Configuration (Permanent)
- Severity: Medium
- Location: UC-004 (VALIDATE, Step 1)
- Retry: No
- User Recoverable: Partial

**User Message:**
```
"Configuration file is invalid. Please try again"
```

**Technical Details:**
```
Code: OFF-CONFIG-004
Condition: Generated configuration.xml failed XSD validation
Details: XML validation failed: [schema_error]
Example: Missing required element <Version> in config.xml
Root Cause: Error in config.xml generation (application bug)
Impact: Cannot proceed to UC-005, blocks installation
```

**Recovery Procedure (User):**
1. Go back to UC-003 (SELECT_APPS)
2. Reconfigure app exclusions
3. Retry validation
4. If error persists: Contact IT Help Desk

**Support Troubleshooting:**
- This error indicates application bug (not user error)
- Check if config.xml generation logic has issues
- Verify XML schema compliance
- Collect config.xml file for analysis
- Action: Escalate to development team

---

## Security Errors (OFF-SECURITY-*)

### OFF-SECURITY-101: Hash Verification Failed

**Quick Info:**
- Error Code: `OFF-SECURITY-101`
- Category: Security
- Severity: High
- Location: UC-004 (VALIDATE, Step 4)
- Retry: Yes (3x with backoff) then Permanent
- User Recoverable: Maybe

**User Message:**
```
"Could not verify Office installation package. Try again"
```

**Technical Details:**
```
Code: OFF-SECURITY-101
Condition: Downloaded Office package hash does not match expected hash
Details: Hash mismatch or download corruption
Expected: [expected_hash_sha256]
Got: [actual_hash_sha256]
Root Cause: Corrupted download, network error, or package tampering
Impact: Blocks installation (security concern)
```

**Recovery Procedure (User):**
1. Application automatically retries 3 times (with 2s, 4s, 6s delays)
2. If all retries fail:
   - Check internet connection
   - Retry validation manually
3. If still fails: Contact IT Help Desk

**Support Troubleshooting:**
- This error suggests download corruption or network issue
- Actions:
  1. Verify user has stable internet connection
  2. Ask user to retry validation
  3. Check Microsoft hash database for expected hash
  4. If hashes don't match: Package may be corrupted
  5. Action: Redownload Office package, try again
- If issue persists: Check for network interference or firewall

---

### OFF-SECURITY-102: Certificate Validation Failed

**Quick Info:**
- Error Code: `OFF-SECURITY-102`
- Category: Security (Permanent)
- Severity: Critical
- Location: UC-004 (VALIDATE, Step 4)
- Retry: No
- User Recoverable: No

**User Message:**
```
"Office installation package is not authentic"
```

**Technical Details:**
```
Code: OFF-SECURITY-102
Condition: Office package certificate chain validation failed
Details: Certificate validation error: [cert_error]
Root Cause: Package signed with invalid certificate, or cert chain broken
Impact: BLOCKS installation (security concern)
```

**Recovery Procedure (User):**
1. STOP - Do not proceed
2. Contact IT Help Desk immediately
3. Provide error code and details

**Support Troubleshooting:**
- CRITICAL: This indicates potential security issue
- Actions:
  1. DO NOT proceed with installation
  2. Contact Microsoft or IT Security team
  3. Verify package source authenticity
  4. Check for tampering or malware
- May indicate: Compromised package or Man-in-the-Middle attack

---

### OFF-SECURITY-103: Signature Invalid

**Quick Info:**
- Error Code: `OFF-SECURITY-103`
- Category: Security (Permanent)
- Severity: Critical
- Location: UC-004 (VALIDATE, Step 4)
- Retry: No
- User Recoverable: No

**User Message:**
```
"Office package signature is invalid"
```

**Technical Details:**
```
Code: OFF-SECURITY-103
Condition: Office package digital signature verification failed
Details: Digital signature invalid or does not verify
Root Cause: Package not signed by Microsoft, signature corrupted, or tampering
Impact: BLOCKS installation (security concern)
```

**Recovery Procedure (User):**
1. STOP - Do not proceed
2. Contact IT Help Desk immediately

**Support Troubleshooting:**
- CRITICAL: Security issue
- Actions:
  1. Verify package downloaded from official Microsoft source
  2. Check signature with Microsoft tools
  3. If invalid: Redownload package from trusted source
  4. Escalate to IT Security if issue persists

---

## System Errors (OFF-SYSTEM-*)

### OFF-SYSTEM-201: File Lock / Access Denied

**Quick Info:**
- Error Code: `OFF-SYSTEM-201`
- Category: System
- Severity: Medium
- Location: UC-004/UC-005
- Retry: Yes (1x with 2s backoff)
- User Recoverable: Yes

**User Message:**
```
"System file is in use. Try again"
```

**Technical Details:**
```
Code: OFF-SYSTEM-201
Condition: Required file is locked by another process
Details: Cannot access [filepath]: file locked by [process_name]
Example: config.xml locked by antivirus software
Root Cause: File locked by: antivirus, indexing service, or other app
Impact: Validation or installation blocked temporarily
```

**Recovery Procedure (User):**
1. Application automatically retries once (after 2 second wait)
2. If still fails:
   - Close antivirus software temporarily
   - Close other applications accessing files
   - Retry validation
3. If still fails: Restart computer

**Support Troubleshooting:**
- Common causes: Antivirus, Windows Defender, backup software
- Actions:
  1. Ask user to close applications
  2. Temporarily disable antivirus
  3. Run as Administrator (may bypass locks)
  4. Restart computer if issue persists
- Escalate if: Issue persists after restart

---

### OFF-SYSTEM-202: Insufficient Disk Space

**Quick Info:**
- Error Code: `OFF-SYSTEM-202`
- Category: System
- Severity: Medium
- Location: UC-004/UC-005
- Retry: Yes (1x after user frees space)
- User Recoverable: Yes

**User Message:**
```
"Insufficient disk space. Please free up space and try again"
```

**Technical Details:**
```
Code: OFF-SYSTEM-202
Condition: Not enough free disk space for Office installation
Details: Requires [needed_MB]MB free, available=[available_MB]MB
Example: Requires 3000MB free, only 500MB available
Root Cause: Full disk drive, large files consuming space
Impact: Installation cannot proceed
```

**Recovery Procedure (User):**
1. Free up disk space by:
   - Delete temporary files
   - Delete old downloads
   - Move large files to external drive
   - Empty Recycle Bin
2. Ensure at least 3000MB free space
3. Retry validation/installation

**Support Troubleshooting:**
- Ask user to:
  1. Check available disk space (Windows Settings → Storage)
  2. Run Disk Cleanup utility
  3. Delete unnecessary files
  4. Retry installation
- If issue persists: May need hardware upgrade

---

### OFF-SYSTEM-203: Registry Access Denied

**Quick Info:**
- Error Code: `OFF-SYSTEM-203`
- Category: System (Permanent)
- Severity: High
- Location: UC-004/UC-005
- Retry: No
- User Recoverable: Yes (with admin rights)

**User Message:**
```
"Cannot access Windows registry. Run as Administrator"
```

**Technical Details:**
```
Code: OFF-SYSTEM-203
Condition: Application lacks permission to access registry key
Details: Registry key [keypath] access denied
Root Cause: Insufficient user privileges or Group Policy restriction
Impact: Cannot validate or install Office
```

**Recovery Procedure (User):**
1. Right-click application → "Run as Administrator"
2. Provide admin credentials if prompted
3. Retry validation/installation
4. If issue persists: Restart computer
5. If still fails: Contact IT Admin (may be Group Policy issue)

**Support Troubleshooting:**
- Actions:
  1. Verify user account has admin rights
  2. Try running as administrator
  3. Check Group Policy (gpedit.msc for restrictions)
  4. Check registry permissions for key: [keypath]
- If Group Policy restricted: IT Admin must grant permission

---

## Network Errors (OFF-NETWORK-*) - Transient, Auto-Retry

### OFF-NETWORK-301: Connection Timeout

**Quick Info:**
- Error Code: `OFF-NETWORK-301`
- Category: Network (Transient)
- Severity: Medium
- Location: UC-004 (VALIDATE, Step 4 - hash download)
- Retry: Yes (3x with backoff: 2s, 4s, 6s) - Automatic
- User Recoverable: Yes (manual retry if all auto-retries fail)

**User Message (if all retries fail):**
```
"Network connection timeout. Check your internet and try again"
```

**Technical Details:**
```
Code: OFF-NETWORK-301
Condition: Network connection timeout when downloading hash
Details: Timeout connecting to download.microsoft.com after [timeout_ms]ms
Root Cause: Network latency, firewall, slow connection
Impact: Hash validation blocked (temporary)
Retry Strategy: Auto-retry 3x with 2s, 4s, 6s backoff
```

**Recovery Procedure (User):**
1. Application automatically retries 3 times (visible as spinner/progress)
2. If all retries fail:
   - Check internet connection (try opening browser)
   - Restart WiFi router or check Ethernet
   - Retry validation manually
3. If still fails: Contact IT (may be network issue)

**Support Troubleshooting:**
- Actions:
  1. Verify user has internet connection (ask to open browser)
  2. Check if they can ping download.microsoft.com
  3. Check firewall settings (may be blocking Microsoft servers)
  4. Check VPN (if connected, may cause timeout)
  5. Try different network (mobile hotspot to test)
- If issue persists: May indicate ISP or firewall issue

---

### OFF-NETWORK-302: Connection Failed

**Quick Info:**
- Error Code: `OFF-NETWORK-302`
- Category: Network (Transient)
- Severity: Medium
- Location: UC-004 (VALIDATE)
- Retry: Yes (3x) - Automatic
- User Recoverable: Yes

**User Message (if all retries fail):**
```
"Network connection failed. Check your internet and try again"
```

**Technical Details:**
```
Code: OFF-NETWORK-302
Condition: Network connection failed when contacting server
Details: Cannot connect to [URL]: [error_message]
Example: Cannot connect to download.microsoft.com: Destination unreachable
Root Cause: Network down, server unreachable, firewall blocking
Impact: Cannot download hash (temporary)
```

**Recovery Procedure (User):**
1. Auto-retry 3 times
2. If all fail:
   - Restart network adapter or WiFi
   - Check if other websites load
   - Try on different network
3. Retry validation

**Support Troubleshooting:**
- Verify: Can user access internet?
- Actions:
  1. Ping microsoft.com (verify routing)
  2. Check firewall rules
  3. Check for VPN or proxy interfering
  4. Restart modem/router
  5. Try wired connection if WiFi failing

---

### OFF-NETWORK-303: Download Incomplete

**Quick Info:**
- Error Code: `OFF-NETWORK-303`
- Category: Network (Transient)
- Severity: Medium
- Location: UC-004 (VALIDATE, Step 4)
- Retry: Yes (3x) - Automatic
- User Recoverable: Yes

**User Message (if all retries fail):**
```
"File download interrupted. Check your internet and try again"
```

**Technical Details:**
```
Code: OFF-NETWORK-303
Condition: File download incomplete (partial transfer)
Details: Downloaded [bytes_received]B of [total_bytes]B
Root Cause: Connection dropped mid-transfer, network unstable
Impact: Hash validation failed (file incomplete)
```

**Recovery Procedure (User):**
1. Auto-retry 3 times (will resume download)
2. If all fail:
   - Check internet stability (speed test)
   - Switch to wired connection if on WiFi
   - Retry validation
3. If still fails: Contact IT

**Support Troubleshooting:**
- Indicate: Unstable network connection
- Actions:
  1. Verify WiFi signal strength
  2. Reduce interference (disable Bluetooth, etc)
  3. Try wired Ethernet connection
  4. Run speed test to verify stability

---

## Installation Errors (OFF-INSTALL-*)

### OFF-INSTALL-401: Setup.exe Failed

**Quick Info:**
- Error Code: `OFF-INSTALL-401`
- Category: Installation (Permanent)
- Severity: High
- Location: UC-005 (INSTALL)
- Retry: No (Permanent, triggers automatic rollback)
- User Recoverable: Partial (can retry after rollback)

**User Message:**
```
"Office installation failed. Attempting to clean up..."
```

**Technical Details:**
```
Code: OFF-INSTALL-401
Condition: setup.exe exited with non-zero code
Details: setup.exe exited with code [exit_code]: [error_message]
Example: exit code 1603 "Fatal error during installation"
Root Cause: Installation package error, system configuration, permission issue
Impact: Installation failed (automatic rollback triggered)
```

**Recovery Procedure (User):**
1. Application automatically attempts rollback
2. If rollback succeeds: User can retry from UC-001
3. If rollback fails: Contact IT Help Desk immediately
4. When retrying:
   - Ensure Windows is up to date
   - Close all applications
   - Run as Administrator
   - Ensure sufficient disk space

**Support Troubleshooting:**
- This indicates installation package or system issue
- Actions:
  1. Check setup.exe exit code against Microsoft documentation
  2. Verify system meets Office requirements
  3. Check Windows updates are current
  4. Verify antivirus not interfering
  5. Redownload Office package if corrupted
- Common causes: 1603 (system config), 30088 (permission)

---

### OFF-INSTALL-402: Office Already Installed

**Quick Info:**
- Error Code: `OFF-INSTALL-402`
- Category: Installation (Expected)
- Severity: Low
- Location: UC-005 (INSTALL idempotence check)
- Retry: No (expected condition, returns success)
- User Recoverable: N/A (success)

**User Message:**
```
"Office is already installed. Continuing..."
```

**Technical Details:**
```
Code: OFF-INSTALL-402
Condition: Office detected already installed (idempotence check)
Details: Existing Office detected via registry: [registry_keys]
Root Cause: Office already exists on system (expected or from previous install)
Impact: NONE - Skips setup.exe (no-op), returns success
```

**Recovery Procedure (User):**
1. No action needed
2. Application continues and reports success
3. Office installation complete (already exists)

**Support Troubleshooting:**
- This is NOT an error (success condition)
- Indicates: Office already present
- Action: None (expected behavior)
- If user thinks Office not installed: Check Programs & Features

---

### OFF-INSTALL-403: Installation Corrupted

**Quick Info:**
- Error Code: `OFF-INSTALL-403`
- Category: Installation (Permanent)
- Severity: High
- Location: UC-005 (post-install verification)
- Retry: No
- User Recoverable: Partial

**User Message:**
```
"Office installation is corrupted. Please contact IT"
```

**Technical Details:**
```
Code: OFF-INSTALL-403
Condition: Critical Office files missing after installation
Details: Missing critical files: [file_list]
Root Cause: Incomplete installation, corrupted files, disk error
Impact: Office installation failed/corrupted
```

**Recovery Procedure (User):**
1. Application automatically triggers rollback
2. Contact IT Help Desk
3. May need to: Repair Office, reinstall, or replace hard drive

**Support Troubleshooting:**
- This indicates serious installation issue
- Actions:
  1. Attempt Office Repair (Control Panel → Programs → Programs & Features → Modify)
  2. If repair fails: Uninstall and reinstall Office
  3. Check hard drive health (SMART test)
  4. If corruption persists: May indicate hardware failure

---

## Rollback Errors (OFF-ROLLBACK-*)

### OFF-ROLLBACK-501: File Removal Failed

**Quick Info:**
- Error Code: `OFF-ROLLBACK-501`
- Category: Rollback (Permanent, Critical)
- Severity: Critical
- Location: Rollback Phase (after installation failure)
- Retry: No
- User Recoverable: No (requires IT intervention)

**User Message:**
```
"Could not fully remove Office installation. Contact IT Help Desk"
```

**Technical Details:**
```
Code: OFF-ROLLBACK-501
Condition: Cannot delete Office files during rollback
Details: Cannot delete [filepath]: [error_reason]
Example: Cannot delete Program Files\Microsoft Office - file locked
Root Cause: File locked by process, permission denied, disk error
Impact: Rollback incomplete (system partially cleaned)
```

**Recovery Procedure (User):**
1. System left in partial state (Office files not fully removed)
2. Contact IT Help Desk immediately
3. IT will manually clean up remaining files

**Support Troubleshooting (IT):**
- CRITICAL: Rollback failed
- Actions:
  1. Identify locked files: `tasklist /v` or Process Explorer
  2. Close locking process (or restart system)
  3. Manually delete Office files (Administrative privileges)
  4. Clean registry entries
  5. Verify removal complete
- Risk: System left in inconsistent state

---

### OFF-ROLLBACK-502: Registry Cleanup Failed

**Quick Info:**
- Error Code: `OFF-ROLLBACK-502`
- Category: Rollback (Permanent, Critical)
- Severity: Critical
- Location: Rollback Phase
- Retry: No
- User Recoverable: No (requires IT)

**User Message:**
```
"Could not clean registry. Contact IT Help Desk"
```

**Technical Details:**
```
Code: OFF-ROLLBACK-502
Condition: Cannot delete registry keys during rollback
Details: Cannot delete [registry_key]: [error_reason]
Root Cause: Registry key locked, permission denied, or corrupted registry
Impact: Registry entries remain (Office partially installed)
```

**Recovery Procedure (User):**
1. Contact IT Help Desk
2. IT will manually clean registry

**Support Troubleshooting (IT):**
- CRITICAL: Rollback failed
- Actions:
  1. Run regedit as Administrator
  2. Navigate to HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Office
  3. Manually delete Office registry entries
  4. Verify entries removed

---

### OFF-ROLLBACK-503: Rollback Incomplete

**Quick Info:**
- Error Code: `OFF-ROLLBACK-503`
- Category: Rollback (Permanent, Critical)
- Severity: Critical
- Location: Rollback Phase (fatal state)
- Retry: No
- User Recoverable: No (manual IT intervention required)

**User Message:**
```
"System cleanup incomplete. Contact IT Help Desk"
```

**Technical Details:**
```
Code: OFF-ROLLBACK-503
Condition: Rollback partially failed (multiple steps failed)
Details: Rollback partial failure: [steps_completed]/3 steps
Example: Files deleted but registry cleanup failed
Root Cause: Multiple rollback operations failed
Impact: System left in inconsistent state (partially cleaned)
```

**Recovery Procedure (User):**
1. CRITICAL - Contact IT Help Desk immediately
2. Provide error code and details
3. System requires manual cleanup

**Support Troubleshooting (IT):**
- CRITICAL: Multiple rollback failures
- Actions:
  1. Manually remove Office files: Program Files\Microsoft Office\*
  2. Manually clean registry: HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Office\*
  3. Manually remove shortcuts: Desktop, Start Menu
  4. Verify system clean
  5. Run antivirus scan (ensure no malware)
- Risk: Critical - Manual intervention essential

---

### OFF-SYSTEM-999: Unknown/Fallback Error

**Quick Info:**
- Error Code: `OFF-SYSTEM-999`
- Category: System (Unknown)
- Severity: 3 (Medium)
- Location: Any UC (fallback for unmapped errors)
- Retry: No (Permanent)
- User Recoverable: No (requires IT investigation)

**User Message:**
```
"An unexpected error occurred. Please contact IT Help Desk"
```

**Technical Details:**
```
Code: OFF-SYSTEM-999
Condition: Error code not recognized or unmapped in ErrorHandler.ErrorCatalog
Details: Unknown error code received: [code_value]
Root Cause: Error handling code mismatch or new error not cataloged
Impact: Fallback handler triggered (prevents application crash)
```

**Recovery Procedure (User):**
1. Note the error message and any error code shown
2. Restart OfficeAutomator application
3. If error persists: Contact IT Help Desk immediately
4. Provide screenshot or error details to IT

**Support Troubleshooting:**
```
This error indicates:
  • An error code was generated that is NOT in the 18-code catalog
  • ErrorHandler.DetermineRecoveryState() detected unmapped error
  • Possible causes:
    1. New error type not yet documented (missing from T-021)
    2. Code mismatch between ErrorHandler and T-021 catalog
    3. Exception with no error code mapping
    4. Typo in error code reference

Actions:
  1. Check error code that triggered OFF-SYSTEM-999
  2. Verify error code exists in ErrorHandler.ErrorCatalog (T-020)
  3. Verify error code documented in T-021 catalog
  4. If missing: Add new error code to both T-020 and T-021
  5. If typo: Fix reference in code
  6. Escalate to development team if code should exist

Prevention:
  • Review all error codes before release
  • Cross-check T-020 ErrorCatalog vs T-021 documentation
  • Add unit tests for error code routing
  • Log all OFF-SYSTEM-999 occurrences for analysis
```

---

## Support Quick Reference Guide

### Error by Category

**Configuration Errors (User Input):**
- OFF-CONFIG-001: Invalid version → Retry selection
- OFF-CONFIG-002: Unsupported language → Retry selection
- OFF-CONFIG-003: Invalid app → Uncheck app
- OFF-CONFIG-004: Invalid config → Reconfigure

**Security Errors (Critical):**
- OFF-SECURITY-101: Hash failed → Retry (3x) then contact IT
- OFF-SECURITY-102: Certificate invalid → STOP, contact IT
- OFF-SECURITY-103: Signature invalid → STOP, contact IT

**System Errors (Machine):**
- OFF-SYSTEM-201: File locked → Retry (1x), close apps
- OFF-SYSTEM-202: Disk full → Free space, retry
- OFF-SYSTEM-203: Registry denied → Run as admin

**Network Errors (Connection):**
- OFF-NETWORK-301: Timeout → Auto-retry (3x)
- OFF-NETWORK-302: Connection failed → Auto-retry (3x)
- OFF-NETWORK-303: Download incomplete → Auto-retry (3x)

**Installation Errors (Installation):**
- OFF-INSTALL-401: Setup failed → Automatic rollback, can retry
- OFF-INSTALL-402: Already installed → Expected (success)
- OFF-INSTALL-403: Corrupted → Automatic rollback, contact IT

**Rollback Errors (Critical):**
- OFF-ROLLBACK-501: File removal failed → Contact IT (manual cleanup)
- OFF-ROLLBACK-502: Registry cleanup failed → Contact IT (manual cleanup)
- OFF-ROLLBACK-503: Rollback incomplete → Contact IT (manual cleanup)

### When to Tell User to Contact IT

```
IMMEDIATE ESCALATION (STOP, contact IT):
  • OFF-SECURITY-102, 103 (authentication issues)
  • OFF-SYSTEM-203 (needs admin help)
  • OFF-ROLLBACK-501, 502, 503 (critical failures)
  • OFF-INSTALL-403 (corruption detected)

AFTER USER TROUBLESHOOTING:
  • OFF-CONFIG-004 (config generation issue)
  • OFF-INSTALL-401 (setup failure)
  • OFF-NETWORK-* if auto-retries fail

ROUTINE USER ACTIONS:
  • OFF-CONFIG-001, 002, 003 (user selections)
  • OFF-SYSTEM-201, 202 (close apps, free space)
  • OFF-INSTALL-402 (expected, no action)
```

---

## Document Metadata

```
Created: 2026-05-15 14:00
Task: T-021 Error Codes Catalog
Version: 1.0.0
Story Points: 3
Duration: 4 hours
Status: COMPLETED
Quality Gate: 7/7 Acceptance Criteria MET
Next: T-022 UC-001 & UC-002 State Flows (Thursday)
Use: Reference document for developers + support teams
```

---

**END ERROR CODES CATALOG**

**18 error codes fully documented with user messages, technical details, recovery procedures ✓**

