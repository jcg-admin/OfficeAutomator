```yml
created_at: 2026-04-22 09:35:00
project: OfficeAutomator
work_package: 2026-04-21-03-00-00-scope-definition
phase: Phase 4 — CONSTRAINTS
author: NestorMonroy
status: Borrador
```

# Business Constraints — OfficeAutomator Phase 4

## Overview

Restricciones de negocio que limitan el scope y viabilidad del OfficeAutomator.

---

## 1. Licensing Constraint

### Requirement
- **License model:** Volume Licensing Agreement (VLA) REQUIRED
- **No support:** Consumer/prosumer product keys
- **Activation:** Active Directory-based or KMS activation required
- **Type:** Office LTSC (License to Software Commitment) only

### Business Impact
- OfficeAutomator is for enterprise deployments only
- Home/small business users cannot use this tool
- Organizations must have VLA in place before deployment
- Pirated or unlicensed installations not supported

### Constraint Details
- Each Office LTSC version requires separate VLA
- VLA typically includes Software Assurance (SA) for upgrades
- Volume licensing eligibility: Minimum 5-50 licenses (varies by SKU)
- License expiration: After VLA expires, no updates/support from Microsoft

### Impact on Design
- UC-001 should document that VLA is prerequisite (not validated by tool)
- UC-005 does not include activation (assumes organization handles separately)
- README must clarify VLA requirement prominently
- Support scope: Only for VLA-licensed organizations

### Mitigation
- Add licensing documentation in README
- Provide FAQ for licensing questions (forward to Microsoft)
- Log activation status if detectable (informational only)

---

## 2. Support Window Constraint

### Mainstream Support Dates (as of 2026-04-22)
| Version | Mainstream End | Extended End |
|---------|------------------|-------------|
| Office 2024 | 2026-10-13 | 2031-10-13 |
| Office 2021 | 2026-10-13 | 2031-10-13 |
| Office 2019 | 2025-10-13 | 2030-10-13 |

### Business Impact
- Office 2019 approaches mainstream end-of-support (18 months from now)
- Organizations must plan upgrade path before Oct 2025
- After mainstream: Extended support period (5 years) with limited fixes
- OfficeAutomator v1.0.0 supports all 3 versions while in mainstream

### Sunset Planning
- **Phase 1 (v1.0.0 - Now to Oct 2025):** Support all 3 versions
- **Phase 2 (v1.1.0 - After Oct 2025):** Consider Office 2019 deprecation
- **Phase 3 (v2.0.0 - After Oct 2026):** Likely drop Office 2021, focus on 2024+

### Impact on Design
- OfficeAutomator v1.0.0 has sunset date of Oct 2025 (Office 2019 no longer mainstream)
- No design changes needed for support window, but document roadmap
- README should mention support timeline

### Mitigation
- Document support window prominently in README
- Provide upgrade guidance for organizations using Office 2019
- Plan v1.1.0 to address post-2025 landscape
- Archive v1.0.0 for legacy support after sunset

---

## 3. Target Organization Size Constraint

### Estimated Deployment Scale
- **Minimum:** 50+ employees (VLA minimum for cost-justification)
- **Typical:** 100-5,000 employees (mid-market to enterprise)
- **Maximum:** 50,000+ employees (no technical limit)

### Business Implication
- OfficeAutomator targets mid-market and enterprise only
- Small businesses (1-50 employees) unlikely to use tool
- Solo deployment (1 machine) not practical (manual install faster)
- Deployment automation ROI only justified at scale 50+

### Constraint Details
- Minimum 50-user organization to justify VLA licensing
- Average deployment: 200-2,000 concurrent users
- Largest realistic deployment: 50,000+ users (distributed across sites)
- Multi-site deployments not part of v1.0.0 scope (handled by infrastructure)

### Impact on Design
- UC-001 through UC-005 are single-machine focused
- Network-wide deployment orchestration out of v1.0.0 scope
- Configuration XML applies to one machine at a time
- v1.1+ may add SCCM/Intune/Cloud PC distribution (future)

### Mitigation
- Document target organization size in README
- Clarify v1.0.0 is single-machine deployment (not fleet-wide)
- Provide roadmap for multi-machine deployment in v1.1+

---

## 4. Cost Model Constraint

### Licensing Cost Baseline
- **Office 2024 VLA:** ~$60-120 per user annually (volume-dependent)
- **OfficeAutomator v1.0.0:** Free (open source / internal tool)
- **Development cost:** ~40 hours @ $150/hr = $6,000 (already sunk)
- **Maintenance cost:** ~10 hours/year for v1.0.0 sustainment

### ROI Calculation
- Organizations with 500+ users: ~$3,000/month on Office licensing
- Installation automation savings: 2-3 hours per machine = 1,000-1,500 hours/year for 500-machine fleet
- ROI break-even: ~2-3 weeks for large deployments
- Payback period for tool development: Positive ROI within first deployment cycle

### Business Implication
- Tool creates significant value for large deployments
- Cost avoidance (not new revenue) - internal tool mentality
- No licensing cost for users (open source / organizational asset)
- Maintenance cost << deployment savings

### Constraint Details
- OfficeAutomator requires no external licensing
- No support subscription model (v1.0.0 free, v1.1+ TBD)
- Internal tool assumption: Organization absorbs development cost
- Potential v2.0.0 commercial version if external sales model pursued

### Impact on Design
- No licensing validation in code
- No telemetry or analytics requirements
- No SLA commitments (as internal tool)
- v1.0 support model: GitHub issues + internal team support

### Mitigation
- Document that tool is internal/open source (free)
- Provide clear support contact in README
- Plan v1.1+ for any monetization model (if pursued)

---

## 5. Organizational Maturity Constraint

### Required Maturity Level
- **IT Infrastructure:** Active Directory, Windows domain, WSUS or similar update management
- **Change Management:** Formal change process, testing environments
- **Documentation:** Maintained runbooks and deployment guides
- **Personnel:** IT staff capable of reading PowerShell scripts

### Business Implication
- Immature organizations may lack prerequisites for successful deployment
- Manual installation might be simpler for some organizations
- Tool assumes professional IT infrastructure in place
- Support burden increases if organization lacks basic IT maturity

### Constraint Details
- Assumes Windows domain environment (not standalone machines)
- Assumes AD-based activation (KMS or user/device-based)
- Assumes organized deployment process (not ad-hoc)
- Assumes IT staff can troubleshoot PowerShell module issues

### Impact on Design
- No "wizard" mode for non-technical users (not in-scope)
- Configuration assumes technical audience
- Logging must be detailed for troubleshooting
- UC-004 validation error messages assume technical reading level

### Mitigation
- Document prerequisites clearly in README
- Provide detailed error messages (technical tone acceptable)
- Offer optional onboarding/training documentation
- Plan v1.1+ for less-mature organizations if demand exists

---

## 6. Change Management Window Constraint

### Deployment Timing
- **Ideal window:** Monthly patch Tuesday + 2-4 hours downtime
- **Typical:** 30-60 minute installation window per machine
- **Change freeze:** End of quarter, year-end, major events blocked
- **Maintenance:** After hours or weekend deployment common

### Business Implication
- Limited deployment windows per year
- Batch deployments required (not ad-hoc single-machine)
- OfficeAutomator must complete within 2-hour window for compliance
- Scheduling constraints may delay initial deployment

### Constraint Details
- Enterprise change management windows: 2-4 per month
- Each window supports 10-100 machine deployments
- Typical queue: 500-1000 machines waiting (implies months to complete fleet)
- Fast deployments (30-60 min) fit within approved windows

### Impact on Design
- UC-005 (Install-Office) must complete within 90 minutes maximum
- Progress logging essential (for change management approval)
- Timeout error if installation exceeds 2 hours (safety boundary)
- No capability to pause/resume in v1.0.0

### Mitigation
- Document installation duration requirements in README
- Provide performance optimization guidance
- Estimate realistic timeline for complete fleet deployment
- v1.1+ may support parallel deployments

---

## 7. Compliance and Audit Constraint

### Compliance Scope
- **Standards:** SOC 2, ISO 27001, HIPAA compliance relevant
- **Audit trail:** Installation logs must be retained 3+ years
- **Change control:** Installation recorded in ITIL/ServiceNow/Jira (manual, not automated)
- **Configuration tracking:** Installed versions, languages, exclusions documented

### Business Implication
- Organizations may require enhanced logging for compliance
- Installation must be traceable for audit purposes
- Configuration changes must be documented in formal change control system
- Non-compliance installations may violate enterprise policy

### Constraint Details
- Logs must record: User, Date/Time, Version, Languages, Exclusions, Status
- Audit retention: Typically 3-7 years per organization policy
- Change request: Formal ticket created in organization's ITSM system
- Approval: Manager/security sign-off required before deployment

### Impact on Design
- UC-005 must create detailed, parseable logs
- Logs must include all configuration details (languages, exclusions, version)
- Log format must support ingestion into SIEM/compliance tools
- Installation status must be exportable for audit

### Mitigation
- Design logs for machine-readability (not just human)
- Include all metadata (user, version, languages, exclusions, status)
- Document log format explicitly
- Provide guidance on log archival in README

---

## 8. Support Model Constraint

### v1.0.0 Support Model
- **Support tier:** Internal tool (no external SLA)
- **Support channel:** GitHub issues, internal team chat
- **Response time:** Best-effort (not SLA-backed)
- **Lifetime:** v1.0.0 supported until Oct 2025 (Office 2019 EOL)

### Business Implication
- Organizations cannot expect 24/7 support
- Complex troubleshooting may require vendor involvement
- Microsoft ODT issues escalate to Microsoft (not resolved by OfficeAutomator team)
- Support cost is organizational overhead (not external contract)

### Constraint Details
- v1.0.0 is "as-is" open source software
- No paid support contract option (v1.0.0)
- v1.1+ may introduce commercial support (TBD)
- Critical security issues may receive expedited response

### Impact on Design
- Error messages must be clear and actionable (self-service troubleshooting first)
- Logging must support troubleshooting without support team
- Documentation must be comprehensive (self-sufficient for most issues)
- UC-004 validation must catch 99% of issues before deployment

### Mitigation
- Excellent documentation (README, deployment guide, troubleshooting)
- Detailed logging for self-diagnosis
- Clear error messages with remediation steps
- Community support channel (GitHub discussions)

---

## 9. Project Timeline Constraint

### v1.0.0 Timeline (Current)
- **Phase 1: DISCOVER** - COMPLETED (5 UCs, requirements, constraints)
- **Phase 2-4:** ~1-2 weeks (design, planning)
- **Phase 5-7:** ~2-3 weeks (specification, detailed design)
- **Phase 8-10:** ~2-4 weeks (implementation, testing)
- **Phase 11-12:** ~1 week (finalization, release)
- **Target release:** May 2026 (3 weeks from now)

### Business Implication
- Compressed 3-week timeline requires focused scope
- No room for scope creep or gold-plating
- v1.0.0 is MVP (minimum viable product) by design
- v1.1+ addresses wishlist items after v1.0.0 release

### Constraint Details
- Release date: May 15, 2026 (hard deadline)
- Feature freeze: April 30, 2026 (no new features after this date)
- Testing period: May 1-12, 2026
- Release notes/documentation: Final week

### Impact on Design
- Only 5 UCs in v1.0.0 (no additional use cases)
- Single-machine deployment only (no SCCM/Intune in v1.0.0)
- English/Spanish languages only (no additional language support in v1.0.0)
- Basic error handling (no advanced recovery in v1.0.0)

### Mitigation
- Maintain strict scope control through gate reviews
- Document wishlist items for v1.1+ in technical debt log
- Prioritize MVP features only
- Plan v1.1+ immediately after v1.0.0 release

---

## 10. Stakeholder Alignment Constraint

### Key Stakeholders
1. **IT Operations:** Deployment/support team (primary user)
2. **Information Security:** Compliance, audit, change control
3. **Windows Infrastructure:** Active Directory, domain management
4. **Desktop/Endpoint:** Software distribution, SCCM/Intune
5. **Finance:** Licensing, procurement, cost tracking

### Business Implication
- Tool must satisfy multiple stakeholder concerns
- Security may require additional logging/audit
- Ops may require simplified deployment process
- Finance may want cost tracking

### Constraint Details
- IT Ops: Simple, automated, repeatable process
- InfoSec: Audit trail, change control, compliance logging
- Windows Infra: Works with domain-joined machines, AD-based activation
- Desktop/Endpoint: Single-machine deployments supported
- Finance: Cost-free tool (no licensing cost)

### Impact on Design
- UC-001: Must be simple for IT Ops (select version, language, exclusions)
- UC-004: Must provide audit trail for InfoSec (detailed validation logs)
- UC-005: Must work in enterprise environment (respects Windows policies)
- All UCs: Must provide clear status for tracking

### Mitigation
- Design for IT Ops simplicity (UC-001-UC-005 fit on 1-2 hours training)
- Provide audit-friendly logging (InfoSec requirements met)
- Work with domain infrastructure (no workarounds, use standard Windows)
- Plan stakeholder sign-off at Phase 6 scope gate

---

## Summary: Business Constraint Impact on Scope

| Constraint | Impact | Severity | Mitigation |
|-----------|--------|----------|-----------|
| VLA licensing only | Target: Enterprise only | MEDIUM | Document in README |
| Support window | Office 2019 sunset Oct 2025 | HIGH | Plan v1.1+ roadmap |
| 50+ user minimum | Not SMB-focused | MEDIUM | Document target market |
| Free tool model | No revenue | LOW | Plan v1.1+ monetization |
| Organizational maturity | Requires AD/IT staff | MEDIUM | Target enterprise market |
| Change management window | 2-hour max deployment | HIGH | Optimize installation speed |
| Compliance requirements | Audit trail needed | MEDIUM | Design detailed logging |
| Internal support model | Self-service focus | LOW | Excellent documentation |
| 3-week timeline | Strict scope control | HIGH | Maintain MVP focus |
| Multi-stakeholder alignment | Complex approval process | MEDIUM | Plan gate reviews |

---

**Phase 4 Artifact:** business-constraints.md  
**Status:** Borrador  
**Next Gate:** Phase 4 → 5 (Validar restricciones antes de diseñar estrategia)
