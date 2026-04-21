```yml
type: Convención de Documentación
stage: Post Stage 6
version: 1.0.0
purpose: Establecer estándar profesional para toda la documentación
updated_at: 2026-04-21 03:40:00
```

# CONVENTION: PROFESSIONAL DOCUMENTATION

---

## Standard Language Requirement

**All documentation must use professional, technical language.**

No colloquial phrases, no "golden rules", no decorative references.

---

## Prohibited Phrases

| Phrase | Why Unprofessional | Alternative |
|--------|-------------------|------------|
| "Regla de Oro" | Spanish colloquialism | "Core Principle", "Critical Standard" |
| "Principio Unix" | Named after external philosophy | "Core Principle", "Architectural Principle" |
| "Los 5 Pilares" | Decorative Spanish | "Five Core Principles", "Five Standards" |
| "Si tienes que..." | Conversational Spanish | "If code requires explanation, it lacks clarity" |
| "Recuerda que..." | Informal | "Note that...", "Important:" |
| "Para recordar:" | Informal | Structure as a list or section heading |

---

## Required Structure

### Heading Styles

**Correct:**
```
# TITLE (all caps)
## SECTION (Title Case)
### SUBSECTION (Title Case)
```

**Incorrect:**
```
# Título de Sección
## Sección pequeña
### subsección
```

---

### Introduction/Opening Statements

**Correct:**
```
## Core Principle

Software must be clear and maintainable above all else.
```

**Incorrect:**
```
## Regla de Oro

La regla de oro es que el software debe ser claro y mantenible.
```

---

### Lists and Bullets

**Correct:**
```
Key standards:
- Clarity: Code explains itself
- Reliability: Fail-fast on errors
- Testability: Functions are independent
```

**Incorrect:**
```
Recuerda estos 3 pilares:
1. La claridad es todo
2. Los errores deben fallar rápido
3. Las funciones deben ser independientes
```

---

### Code Examples

**Correct:**
```powershell
# CORRECT - No magic numbers
$MAX_RETRIES = 3
if ($retryCount -gt $MAX_RETRIES) { break }
```

**Incorrect:**
```powershell
# MALO - Números mágicos
if ($retryCount -gt 3) { break }
```

---

## Examples of Professional Statements

### Instead of "Regla de Oro"

| Instead Of | Use |
|-----------|-----|
| "La regla de oro es..." | "The fundamental principle is..." |
| "La regla de oro que seguimos es..." | "Core standard: ..." |
| "Nunca olvides la regla de oro..." | "Critical standard to maintain: ..." |

### Instead of "Principio Unix"

| Instead Of | Use |
|-----------|-----|
| "Siguiendo el principio Unix..." | "Following software design principles: ..." |
| "El principio Unix dice que..." | "Core principle: ..." |
| "Recordando el principio Unix..." | "Core design pattern: ..." |

### Instead of "Los 5 Pilares"

| Instead Of | Use |
|-----------|-----|
| "Los 5 pilares son..." | "Five core principles:" |
| "Estos pilares fundamentales..." | "These fundamental standards:" |
| "Los pilares del proyecto..." | "Project foundations:" |

---

## Tone Guidelines

### Acceptable Tone

- Technical and precise
- Direct and clear
- Formal but not rigid
- English preferred (if document is in English)
- Consistent terminology

### Unacceptable Tone

- Colloquial or conversational
- Decorative or metaphorical phrases
- Mixing languages (Spanish/English without reason)
- Informal "rules" or "laws"
- Cute or humorous references

---

## Examples: Before vs After

### Before (Unprofessional)

```
## La Regla de Oro

Si tienes que explicar qué hace tu código, 
probablemente el código no es lo suficientemente claro.

Este es el principio Unix que seguimos: 
cada función debe hacer UNA cosa bien.
```

### After (Professional)

```
## Critical Standard

Code that requires explanation likely lacks clarity.

Core principle: each function should have a single, 
well-defined responsibility.
```

---

## Documentation Sections

Use these professional section names:

```
# TITLE

## Overview / Introduction
- What is this?
- Why does it matter?

## Core Principles
- Fundamental standards

## Requirements / Prerequisites
- What's needed?

## Implementation / Process
- How does it work?

## Examples
- Code samples
- Usage patterns

## Standards / Best Practices
- How to do it correctly
- What to avoid

## References
- External documentation
- Related documents

## Next Steps
- What comes after?
```

**Avoid:**
- "Regla de Oro", "Principio Unix", "Los 5 Pilares"
- Decorative headers like "La Magia de X", "Los Secretos de X"
- Informal sub-headings like "Recuerda:", "No Olvides:"

---

## Enforcement

**Starting now (post Stage 6):**
- All new documents must follow this standard
- Existing documents with unprofessional phrases have been updated
- Code review includes language check
- Commits requiring documentation review

---

## Files Already Updated

- ✓ REGLAS_DESARROLLO_OFFICEAUTOMATOR.md
- ✓ convention-mermaid-diagrams.md

**Future documents:**
- All Stage 7+ artifacts
- All new conventions
- All project documentation

---

**Version:** 1.0.0
**Effective Date:** 2026-04-21 (immediate)
**Status:** ACTIVE

