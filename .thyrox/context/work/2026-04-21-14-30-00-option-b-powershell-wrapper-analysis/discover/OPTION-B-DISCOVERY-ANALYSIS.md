```yaml
phase: Phase 1 DISCOVER
wp: 2026-04-21-14-30-00-option-b-powershell-wrapper-analysis
methodology: workflow-discover
date_created: 2026-04-21
status: IN_PROGRESS
```

# Phase 1: DISCOVER - OPTION B PowerShell Wrapper Analysis

## 1. Objetivo / Por qué (WHY)

### Business Need
Crear una capa PowerShell profesional que envuelva los 5 Use Cases existentes de OfficeAutomator.Core.dll, proporcionando una interfaz familiar y accesible para administradores de sistemas (sysadmins) sin requerir conocimientos de C#.

### Strategic Goal
Reemplazar MSOI.ps1 (script amateur con 5+ bugs críticos, 0 tests) con una solución híbrida que mantenga la experiencia PowerShell pero leverage la arquitectura robusta de C# (220+ tests, 0 bugs).

### Value Proposition
- **Para sysadmins:** Interfaz PowerShell familiar, menús interactivos, sin curva de aprendizaje
- **Para IT:** Solución profesional con 220+ tests, validación 8 pasos, rollback automático
- **Para organizaciones:** Reducción de riesgo (0 bugs vs 5+), mejor soporte, escalabilidad a múltiples clientes
- **Para developers:** Base sólida para futuras extensiones (CLI tool, módulo PS, REST API)

---

## 2. Stakeholders (WHO)

### Primary Stakeholders

| Stakeholder | Role | Needs | Pain Points |
|-------------|------|-------|-------------|
| **IT Sysadmin** | End User | Interfaz fácil, sin complejidad | MSOI.ps1 tiene bugs, sin documentación clara |
| **IT Manager** | Decision Maker | Solución confiable, bajo soporte | MSOI frágil, deploy riesgoso |
| **Developer (PowerShell)** | Builder | Arquitectura clara, patrones profesionales | No existen specs claras para OPCIÓN B |
| **Developer (C#)** | Builder | Integración seamless con Core.dll | No hay documentación de cómo PS llama C# |
| **QA Tester** | Validator | Tests reproducibles, casos claros | MSOI sin tests |

### Secondary Stakeholders
- Tech lead: Arquitectura y decisiones de diseño
- Project sponsor: Presupuesto, timeline, ROI
- Security team: Validación de no crear vulnerabilidades

---

## 3. Uso Operacional (HOW)

### Current State (As-Is)
```
Sysadmin
  ↓
MSOI.ps1 (script amateur, 166 líneas)
  ├─ 5+ bugs críticos
  ├─ 0 tests
  ├─ 0 validación estructurada
  └─ Timeout 60s (insuficiente)
  ↓
Manual troubleshooting si falla
```

### Desired State (To-Be)
```
Sysadmin ejecuta: .\OfficeAutomator.PowerShell.Script.ps1
  ↓
Menú 1: UC-001 (VersionSelector)
  Entrada: Seleccionar versión
  
Menú 2: UC-002 (LanguageSelector)
  Entrada: Seleccionar idioma
  
Menú 3: UC-003 (AppExclusionSelector)
  Entrada: Seleccionar apps a excluir
  
Validación: UC-004 (ConfigValidator - 8 pasos)
  Automático: Sin entrada usuario
  
Instalación: UC-005 (InstallationExecutor)
  Automático con progreso en tiempo real
  Si falla: Rollback automático
  
Resultado: Log detallado + éxito/fallo claro
```

### User Journey
**Happy Path:** 15-30 minutos desde inicio a Office instalado
**Error Path:** Rollback automático + log claro de qué falló

### Usage Frequency
- Daily: IT teams instalando en nuevas máquinas
- Weekly: Batch deployments (20-50 máquinas)
- Monthly: Versión updates (cambio a nuevo Office LTSC)

---

## 4. Atributos de Calidad (QUALITY)

| Atributo | Importancia | Target | Rationale |
|----------|-----------|--------|-----------|
| **Confiabilidad** | CRÍTICA | 99.5% successful installations | MSOI tiene 5+ bugs, 0 tolerance for failures |
| **Usabilidad** | CRÍTICA | Sysadmin usa sin entrenamiento | PowerShell familiar, menús intuitivos |
| **Performance** | ALTA | < 30 minutos per installation | Acceptable range for IT workflows |
| **Maintainability** | ALTA | < 4 semanas para feature nuevo | Código limpio, bien documentado |
| **Testability** | ALTA | > 90% code coverage | All paths covered by tests |
| **Compatibility** | MEDIA | PowerShell 5.1+, .NET 8.0+ | Windows 10+ machines |
| **Security** | ALTA | No security vulnerabilities | No credential exposure, hash validation |
| **Supportability** | MEDIA | Log files + error codes | Clear diagnosis for IT support |

---

## 5. Restricciones (CONSTRAINTS)

### Technical Constraints
| Constraint | Impact | Mitigation |
|-----------|--------|-----------|
| PowerShell 5.1 minimum | Older Windows versions not supported | Document requirement clearly |
| .NET 8.0 runtime | Must be installed on target machine | Include in pre-requisites checklist |
| Cannot modify C# Core.dll | Must work with existing UC-001 to UC-005 | Wrapper pattern (no changes to Core) |
| System.Diagnostics.Process only | Limited to Windows | Design is Windows-only (acceptable) |

### Organizational Constraints
| Constraint | Impact | Mitigation |
|-----------|--------|-----------|
| Budget: Fixed | 56 hours max for implementation | Timeline: 2 weeks full-time developer |
| Timeline: 2 weeks | Must ship OPTION B v2.0.0-beta1 by end of week 2 | Prioritize critical path first |
| Team: 1 PS developer | No parallel work on multiple features | Sequential implementation (Lunes-Viernes plan) |
| Support: Limited | No 24/7 support guarantee | Clear documentation + troubleshooting guide |

### Regulatory Constraints
- None identified (internal tool, no compliance requirements)

---

## 6. Contexto / Sistemas Vecinos (CONTEXT)

### Adjacent Systems
```
OfficeAutomator.PowerShell.Script.ps1
  ├─ integrates with → OfficeAutomator.Core.dll (5 UC)
  ├─ reads from → OfficeDeploymentTool.exe (Microsoft official)
  ├─ executes → setup.exe (Office installation)
  ├─ writes to → %TEMP%\OfficeAutomator_*.log
  ├─ modifies → Windows Registry (Office installation)
  └─ may integrate with → Intune / SCCM (future)
```

### Environmental Assumptions
- Windows 10 or later (with PowerShell 5.1+)
- Internet connectivity (to download Office)
- Local admin privileges on target machine
- No Office already installed (or user confirms overwrite)
- 10 GB free disk space available

---

## 7. Fuera de Alcance (OUT OF SCOPE)

| Item | Why Out | For Future |
|------|---------|-----------|
| GUI (WPF Form) | Complexity vs benefit, PowerShell is enough | PHASE 2: OfficeAutomator.Cli |
| REST API | Requires server infrastructure | PHASE 3: OfficeAutomator.WebApi |
| Uninstallation | Scope limited to installation | Future: UC-006 (Uninstall) |
| Multi-language UI | English + Spanish sufficient for MVP | Future: Localization |
| SCCM/Intune integration | Can be added post-MVP | Future: PHASE 2 |
| Linux support | Windows-only solution acceptable | Future consideration |

---

## 8. Criterios de Éxito (SUCCESS CRITERIA)

### Functional Success
- [ ] Script executes UC-001 (Version Selection) successfully
- [ ] Script executes UC-002 (Language Selection) successfully
- [ ] Script executes UC-003 (App Exclusion) successfully
- [ ] Script executes UC-004 (Configuration Validation) successfully
- [ ] Script executes UC-005 (Installation & Rollback) successfully
- [ ] Automatic rollback works on failure
- [ ] Log file created with all actions recorded

### Quality Success
- [ ] OfficeAutomator.PowerShell.Integration.Tests.ps1 passes 100%
- [ ] OfficeAutomator.PowerShell.EndToEnd.Tests.ps1 passes 100%
- [ ] No PowerShell errors or exceptions unhandled
- [ ] DLL loads correctly in PowerShell context
- [ ] C# objects created and methods called successfully

### User Experience Success
- [ ] Sysadmin can complete installation without documentation
- [ ] Menús are clear and self-explanatory
- [ ] Progress bars show actual progress
- [ ] Error messages are actionable
- [ ] Log file is easy to troubleshoot

### Adoption Success
- [ ] Beta testing on 5 machines: 100% successful
- [ ] Production testing on 20 machines: >= 95% successful
- [ ] Zero critical bugs found in production
- [ ] Better than MSOI in all aspects

---

## 9. Decisiones Clave (KEY DECISIONS)

### Decision 1: Architecture - WRAPPER PATTERN
**Chosen:** PowerShell wrapper over C# Core.dll (OPTION B)
**Rejected:** Pure C# DLL (OPTION A) - would require retraining IT teams
**Rationale:** Leverage existing C# quality while maintaining familiar PowerShell experience

### Decision 2: Naming Convention
**Chosen:** OfficeAutomator.PowerShell.Script.ps1 (professional, auto-descriptive)
**Rejected:** MSOI-Mejorado.ps1 (unprofessional, not auto-descriptive)
**Rationale:** Professional names communicate quality and clarity

### Decision 3: No New Use Cases
**Chosen:** Wrap existing 5 UC (no new UC creation)
**Rejected:** Create 7 new UC (would duplicate existing logic)
**Rationale:** Reuse validated architecture, reduce risk

---

## Next Steps (TOLLGATE)

### Tollgate Review Required
Before proceeding to Phase 2 (ANALYZE), this DISCOVER phase must receive:

1. **Business Approval**
   - [ ] Confirm objective/why is correct
   - [ ] Approve stakeholder list
   - [ ] Confirm success criteria
   - [ ] OK key decisions

2. **Technical Feasibility**
   - [ ] Verify C# Core.dll can be called from PowerShell
   - [ ] Confirm UC-001 to UC-005 are complete + tested
   - [ ] Validate naming convention aligns with project standards

3. **Resource Commitment**
   - [ ] 1 PowerShell Developer assigned
   - [ ] 56 hours budget approved
   - [ ] 2-week timeline confirmed

---

**Status:** Phase 1 DISCOVER Complete - Awaiting Tollgate Review  
**Ready for:** Phase 2 ANALYZE (ba-requirements-analysis)
**Prepared by:** Claude  
**Date:** 2026-04-21

