```yml
created_at: 2026-04-22 09:40:00
project: OfficeAutomator
work_package: 2026-04-21-03-00-00-scope-definition
phase: Phase 4 — CONSTRAINTS
author: NestorMonroy
status: Borrador
```

# Platform Constraints — OfficeAutomator Phase 4

## Overview

Restricciones de plataforma que limitan la arquitectura, integración y despliegue del OfficeAutomator.

---

## 1. Windows-Only Platform Constraint

### Requirement
- **OS Family:** Windows only (no macOS, Linux, or hybrid)
- **Desktop OS:** Windows 10 (version 1909+), Windows 11
- **Server OS:** Windows Server 2016, 2019, 2022
- **No support:** macOS, Linux, WSL, Containers

### Business Implication
- Cross-platform portability not in-scope
- Tool is enterprise Windows-focused
- No WSL (Windows Subsystem for Linux) support (Office not supported in WSL)
- No Docker/containerized deployment

### Platform Constraint Details
- Office LTSC only runs on Windows (no Office 365 on other platforms)
- PowerShell native execution Windows-only (PowerShell Core can run on Linux, but Office cannot)
- Registry/WMI operations Windows-specific
- Installer (setup.exe) Windows binary only

### Impact on Design
- Entire codebase assumes Windows platform
- No platform abstraction layer needed
- Windows-specific APIs acceptable (registry, WMI, Windows API calls)
- Testing: Windows machines only (not CI/CD on Linux agents)

### Mitigation
- Document Windows-only requirement prominently
- Fail with clear error message if run on non-Windows platform
- No effort spent on cross-platform compatibility
- v1.0.0 scope: Windows only (future versions also Windows-only)

---

## 2. PowerShell Module Platform Constraint

### Requirement
- **Format:** PowerShell script module (.psm1 + manifest .psd1)
- **Execution context:** PowerShell 5.1+ (Windows PowerShell or PowerShell Core)
- **Distribution:** PowerShellGallery.com OR internal NuGet repository
- **Installation:** Manual copy OR `Install-Module` command

### Platform Implication
- Module format (not compiled DLL, not standalone executable)
- Requires PowerShell runtime on target machine
- Requires PowerShell execution policy to allow module loading
- Module-based distribution (not MSI/EXE installer)

### Platform Constraint Details
- PowerShell execution policy must allow scripts:
  - Minimum: `RemoteSigned` (scripts must be signed, local scripts OK unsigned)
  - Typical: `Unrestricted` or `RemoteSigned`
  - NOT compatible with `AllSigned` (module not signed in v1.0.0)
  - NOT compatible with `Restricted` (no scripts allowed)
- Module path: User or system-wide (`$PSModulePath`)
- Module scope: User or system (`Import-Module -Scope CurrentUser|AllUsers`)

### Impact on Design
- UC-001 must load PowerShell module (implies execution policy met)
- Fail-Fast if execution policy too restrictive
- Module manifest (.psd1) must declare dependencies (none for v1.0.0)
- Signed vs. unsigned: v1.0.0 unsigned (future may require signing)

### Mitigation
- Document execution policy requirement
- Check execution policy in module load
- Provide signing option for v1.1+ (if required by organization)
- Document module installation in README

---

## 3. Active Directory (AD) Integration Constraint

### Requirement
- **Domain model:** Windows domain (Active Directory) ASSUMED
- **Machine trust:** Domain-joined machine ASSUMED
- **User identity:** Domain user account (not local account)
- **Privileges:** Domain admin or local admin with AD integration

### Platform Implication
- Tool assumes enterprise domain environment
- Not compatible with workgroup (standalone machine) deployments
- AD-based activation required (KMS or user/device-based)
- Machine policy and group policy may affect execution

### Platform Constraint Details
- Domain-joined machine assumed: Fail if not domain-joined (warning in v1.0.0)
- Domain user assumed: Fail if local account (warning in v1.0.0)
- Group Policy Objects (GPOs) may restrict PowerShell execution
- Machine name must be resolvable in AD (not critical, but recommended)

### Impact on Design
- UC-001 can assume AD environment (no workaround for workgroup)
- UC-004 validation may check domain status (informational)
- UC-005 assumes AD activation available (no standalone KMS setup in tool)
- Error messages should clarify AD requirement

### Mitigation
- Document AD requirement clearly
- Warning message if non-domain-joined machine detected
- Roadmap v1.1+ for non-AD environments (if demand exists)
- Support article: "Deploying OfficeAutomator without Active Directory"

---

## 4. Windows Update (WSUS/WU) Integration Constraint

### Requirement
- **Update model:** Windows Update or WSUS (Windows Server Update Services)
- **Office updates:** Handled by Windows Update after installation (not by tool)
- **Security updates:** Organization responsible (not in OfficeAutomator scope)
- **Tool updates:** OfficeAutomator module updates via PowerShellGallery or internal repository

### Platform Implication
- Office Update management is post-installation responsibility
- Tool does not manage Office patches
- Organization's Windows Update policy applies to Office after installation
- OfficeAutomator itself must be updated separately (if v1.1+ released)

### Platform Constraint Details
- Office installed with updates disabled (UC-005 sets `<Updates Enabled="0" />`)
- Organization's Windows Update policy handles patches post-installation
- WSUS integration: Automatic (Office uses Windows Update path)
- Office deployment tool cannot configure update policy

### Impact on Design
- UC-005 explicitly disables in-place updates in configuration XML
- Post-installation update management is organizational responsibility (not in scope)
- Documentation must clarify update management responsibility
- No UC for "update installed Office" (out of v1.0.0 scope)

### Mitigation
- Document post-installation update management process
- Clarify that Office update responsibility is organizational
- Provide link to Microsoft's update management guides
- Plan v1.1+ for update management features (if demand exists)

---

## 5. Software Distribution Platform Integration Constraint

### v1.0.0 Scope: Direct PowerShell Execution
- **Deployment method:** Direct PowerShell module load (manual or script)
- **No integration:** SCCM (System Center Configuration Manager)
- **No integration:** Intune (Microsoft Endpoint Manager)
- **No integration:** Cloud PC or virtual desktop infrastructure
- **No integration:** Image deployment (sysprep, WIM)

### v1.1+ Roadmap: Future Integration
- **SCCM integration:** Execute OfficeAutomator via package
- **Intune integration:** Win32 app with intunewin packaging
- **Cloud PC:** PowerShell script deployment
- **VDI:** Image-based deployment (custom image creation)

### Platform Constraint Details
- v1.0.0 direct execution: `Import-Module OfficeAutomator; Invoke-OfficeAutomator`
- SCCM/Intune: Not supported (tool not designed for SCCM/Intune execution)
- Application packaging: Tool is PowerShell module, not application package
- Distribution: PowerShellGallery or internal repository (not MSI/AppX)

### Impact on Design
- Assume direct PowerShell execution environment
- No integration with enterprise application management
- Tool must work standalone (no SCCM agent required)
- v1.0.0 is "bring your own deployment mechanism"

### Mitigation
- Document v1.0.0 direct deployment method clearly
- Provide sample scripts for common deployment scenarios
- Publish roadmap for SCCM/Intune integration (v1.1+)
- Support article: "Deploying via SCCM/Intune (workarounds)"

---

## 6. Logging and Monitoring Platform Constraint

### Logging Destination
- **v1.0.0:** Local file system (`C:\Logs\OfficeAutomator\` or user-specified)
- **No integration:** Event Log, SIEM, splunk, ELK stack
- **No integration:** Cloud logging (Azure Monitor, CloudWatch)
- **No real-time monitoring:** Monitoring is post-installation review

### Platform Implication
- Logs are local files only
- No centralized log collection (organizational responsibility)
- No real-time alerting (operator must check logs)
- Log retention is organizational responsibility

### Logging Constraint Details
- Log path: Local filesystem (UNC paths not supported in v1.0.0)
- Log format: Structured text with timestamps
- Log rotation: Organization's responsibility (tool doesn't rotate logs)
- Log retention: Organization's responsibility (tool doesn't purge)

### Impact on Design
- UC-005 writes logs to local path
- Log format must be parseable (for SIEM integration later)
- Documentation should clarify log location and format
- v1.1+ may add Event Log or SIEM integration

### Mitigation
- Document log path and format clearly
- Provide sample SIEM integration script for v1.0.0 (post-deployment)
- Plan v1.1+ for Event Log/Azure Monitor/Splunk integration
- Support article: "Integrating OfficeAutomator logs with SIEM"

---

## 7. Antivirus and Security Software Constraint

### Requirement
- **Tool compatibility:** AV software may interfere with installation
- **No integration:** OfficeAutomator does not interact with AV
- **AV exclusions:** Organization's responsibility to configure
- **Security scanning:** May slow down installation

### Platform Implication
- Real-time AV scanning may impact installation performance
- Some AV products flag unsigned scripts (warning in v1.0.0, planned signing in v1.1+)
- Network AV scanning (web proxies) may delay ODT downloads
- AV may quarantine temporary files during installation

### Platform Constraint Details
- Antivirus exclusions recommended for:
  - PowerShell execution path (`C:\Windows\System32\WindowsPowerShell\`)
  - Office installation path (`C:\Program Files\Microsoft Office`)
  - Temporary download cache (default: `%APPDATA%\Microsoft\OfficeSetup\`)
- Real-time scanning adds 10-20% to installation time
- Unsigned PowerShell module may trigger warnings in strict AV policies

### Impact on Design
- UC-005 may be slow if AV is scanning downloads
- No AV integration in tool (AV is pre-configured by organization)
- Error handling must account for AV interference (as network error)
- Documentation should mention AV exclusion recommendations

### Mitigation
- Document recommended AV exclusions in README
- Add note about AV impact on installation performance
- Provide AV troubleshooting section in docs
- Plan signing for v1.1+ (reduces AV warnings)

---

## 8. Network Proxy and Firewall Constraint

### Network Requirement
- **Internet access:** Required (to download Office from Microsoft CDN)
- **Proxy support:** System proxy only (no proxy authentication in tool)
- **Firewall:** Outbound HTTPS (443) to content-delivery.microsoft.com
- **VPN:** May interfere with downloads (organization's responsibility)

### Platform Implication
- Air-gapped networks: Cannot deploy Office (offline installation future feature)
- Restrictive firewalls: May block Microsoft CDN
- Proxy servers: Tool uses system proxy configuration
- VPN: Performance varies (organization's responsibility)

### Platform Constraint Details
- Proxy support: Windows system proxy (via `WebClient` or `Net.Http`)
- Proxy authentication: Not supported in v1.0.0 (use system proxy with cached credentials)
- Firewall rules: Organization must allow outbound 443 to Microsoft CDN
- Content-delivery.microsoft.com: Primary download endpoint (no fallback CDNs)

### Impact on Design
- UC-005 uses system proxy (no explicit proxy configuration in tool)
- Network errors are handled as installation failures (with retry)
- No proxy authentication dialog in UC-005
- Documentation should list required firewall rules

### Mitigation
- Document firewall requirements (port 443, Microsoft CDN IPs)
- Provide network troubleshooting guide
- Plan v1.1+ for offline installation (pre-cached Office images)
- Support article: "OfficeAutomator behind corporate proxy/firewall"

---

## 9. Execution Environment Constraint

### Runtime Requirements
- **Process elevation:** Local administrator (UAC elevation required)
- **Execution context:** User session (not system service, not scheduled task service account)
- **Home directory:** Must have write access (for logs/temp files)
- **System resources:** Minimum 4 GB RAM, 5 GB disk space

### Platform Implication
- Cannot run as SYSTEM account (LocalSystem)
- Cannot run as NETWORK SERVICE (IIS application pool account)
- Cannot run headless/non-interactive (scheduled task limitations)
- Session must remain active during installation (no session timeout)

### Execution Environment Constraint Details
- User session: Login required (must have active user session)
- Admin privileges: `Run as Administrator` OR local admin account
- Session timeout: UAC may interfere if session is remote (RDP)
- Resource constraints: Minimum viable resources required

### Impact on Design
- Assume user is logged in with admin privileges
- UC-001 initial check: Admin privilege (non-negotiable)
- Session management: Not in scope (assume organization handles)
- Remote execution: Not supported in v1.0.0 (local only)

### Mitigation
- Document admin privilege requirement prominently
- Check privilege at module load time (UC-001)
- Provide clear instructions for elevation
- Plan v1.1+ for remote execution (PSRemoting)

---

## 10. Regulatory and Compliance Platform Constraints

### Regulatory Scope
- **HIPAA:** Health Insurance Portability and Accountability Act (healthcare)
- **PCI-DSS:** Payment Card Industry Data Security Standard (financial)
- **SOC 2:** Service Organization Control (service providers)
- **ISO 27001:** Information security management

### Platform Implication
- Regulated organizations may have enhanced audit/logging requirements
- Encryption and secure deletion may be mandated
- Data residency requirements may apply
- Regular security assessments mandatory

### Regulatory Constraint Details
- HIPAA: Audit trail required, minimum 3-year retention
- PCI-DSS: Change management required for any installation
- SOC 2: Configuration and access control documentation
- ISO 27001: Risk assessment and management plan

### Impact on Design
- Logging must be audit-friendly (machine-readable)
- Configuration must be documented and traceable
- Audit trail must be immutable (no log deletion)
- Documentation must include compliance guidance

### Mitigation
- Design detailed, audit-friendly logging
- Document compliance requirements in README
- Provide audit trail export mechanism
- Plan v1.1+ for enhanced compliance features (encryption, etc.)

---

## 11. Containers and Virtualization Platform Constraint

### v1.0.0 Scope
- **Hyper-V VMs:** Supported (virtual Windows machines)
- **VMware VMs:** Supported (virtual Windows machines)
- **VirtualBox VMs:** Supported (virtual Windows machines)
- **Cloud VMs:** Supported (Azure VMs, AWS EC2, Google Cloud VMs)
- **Docker containers:** Not supported (Office not supported in containers)
- **Cloud PC:** Not supported in v1.0.0 (planned for v1.1+)

### Platform Implication
- Virtualized environments (VMs) work identically to physical machines
- Cloud deployment supported (Azure/AWS/GCP)
- Container-based deployment not supported
- Cloud PC (Windows 365) not supported (future feature)

### Constraint Details
- Hyper-V/VMware: Office installation works normally
- Cloud VMs: Must meet Office requirements (Windows OS, admin privileges, disk space)
- Docker: Office not supported in containers (use VMs instead)
- Cloud PC: Requires special handling (future v1.1+ feature)

### Impact on Design
- No special handling for virtual machines (treat as physical)
- Cloud deployment supported (no changes needed)
- Container deployment out-of-scope (use VMs instead)
- Cloud PC roadmap item for v1.1+

### Mitigation
- Document supported environments (physical, VMs, cloud)
- Note that containers are not supported
- Provide roadmap for Cloud PC support (v1.1+)
- Support article: "OfficeAutomator in virtual environments"

---

## Summary: Platform Constraint Impact on Architecture

| Constraint | Impact | Severity | Mitigation |
|-----------|--------|----------|-----------|
| Windows-only | Cross-platform not in-scope | HIGH | Document Windows requirement |
| PowerShell module | Not compiled executable | MEDIUM | Distribution via PSGallery |
| AD integration | Assumes domain environment | MEDIUM | Warning if not domain-joined |
| WSUS/WU integration | Office updates organizational responsibility | LOW | Document in README |
| Software distribution | No SCCM/Intune v1.0.0 | MEDIUM | Plan v1.1+ integration |
| Local file logging | No SIEM integration v1.0.0 | MEDIUM | Plan v1.1+ integration |
| Antivirus interference | May slow installation | LOW | Document AV exclusions |
| Network/proxy | Requires internet access | HIGH | Document firewall rules |
| Admin execution | Must run as admin | HIGH | Check privilege upfront |
| Regulatory compliance | Audit trail required | MEDIUM | Design detailed logging |
| Containers unsupported | No Docker, use VMs | MEDIUM | Document in README |

---

**Phase 4 Artifact:** platform-constraints.md  
**Status:** Borrador  
**Next Gate:** Phase 4 → 5 (Validar restricciones antes de diseñar estrategia)
