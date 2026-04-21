---
type: Error Documentation
error_id: ERROR-004
severity: CRÍTICA
fecha: 2026-04-21 06:05:00
fase: Stage 7 DESIGN/SPECIFY v2 (PLAN creado sin esto)
---

# ERROR 4: No busqué los 10 PATRONES CLAVE del proyecto

## Descripción del error

Creé un PLAN para Stage 7 sin verificar los 10 patrones clave de OfficeAutomator que van MÁS ALLÁ de las 9 convenciones leídas.

**Lo que asumí:**
- Las 9 convenciones en `.claude/rules/` eran TODO

**Lo que ignoré:**
- ROADMAP.md (épicas estruturadas)
- GitHub Actions workflows (validate.yml)
- README en directorios
- now.md (estado persistente)
- UC Matrix (mapeo de responsables)
- ADR Structure específica
- Stages simplificados (6 vs 12)

## 10 Patrones Clave encontrados

1. **YAML Frontmatter**
   - Metadatos consistentes en TODOS los docs
   - NO usar `---` frontmatter YAML (usar bloque yml)

2. **Work Packages**
   - Nomenclatura: `YYYY-MM-DD-HH-MM-SS-nombre`
   - Tracking automático de timeline

3. **Conventional Commits**
   - Validación automática en PRs
   - `feat/fix/docs/test/chore(scope): message`

4. **ROADMAP.md** ⭐ MISSING
   - Épicas estructuradas
   - WP asociados
   - Stages mapeados
   - Timeline claro

5. **GitHub Actions** ⭐ MISSING
   - validate.yml para checks automáticos
   - Linter de convenciones
   - CI/CD pipeline

6. **ADR Structure** ⭐ PARCIALMENTE
   - Decisiones arquitectónicas documentadas
   - `.thyrox/context/decisions/adr-*.md`
   - Formato específico de decisiones

7. **UC Matrix** ⭐ MISSING
   - Mapeo: Use Cases → Stages → Responsables
   - Tracking de dependencias
   - Coverage matrix

8. **now.md** ⭐ MISSING
   - Estado persistente del proyecto
   - Current work package
   - Current phase
   - Next steps

9. **README en directorios** ⭐ MISSING
   - Documentación local en cada nivel
   - Explicación de contenido
   - Convenciones por directorio

10. **Stages simplificados** ⭐ CONFLICTO
    - Proyecto usa 6 stages (no 12 como THYROX base)
    - Stages reales: DISCOVER, SCOPE, DESIGN, IMPLEMENT, TRACK, RELEASE
    - Plan que creé asume 12 stages THYROX

## Dónde debería haber encontrado esto

```bash
# Pattern 1: ROADMAP.md
ls -la /tmp/projects/OfficeAutomator/ROADMAP.md
# Debería buscar AQUÍ

# Pattern 4: now.md
find /tmp/projects/OfficeAutomator -name "now.md"
# Debería buscar AQUÍ

# Pattern 5: GitHub Actions
find /tmp/projects/OfficeAutomator/.github -name "*.yml"
# Debería buscar AQUÍ

# Pattern 7: UC Matrix
find /tmp/projects/OfficeAutomator -name "*uc-matrix*" -o -name "*matrix*"
# Debería buscar AQUÍ

# Pattern 8: README en directorios
find /tmp/projects/OfficeAutomator -name "README.md"
# Debería buscar AQUÍ

# Pattern 9: ADR Structure
ls -la /tmp/projects/OfficeAutomator/.thyrox/context/decisions/adr-*.md
# Debería leer TODOS los ADRs para entender estructura
```

## Impacto en el PLAN creado

**El PLAN que acabo de crear es INCOMPLETO porque:**

1. ✗ NO menciona ROADMAP.md (debería estar como referencia)
2. ✗ NO menciona GitHub Actions (debería estar como prerequisito)
3. ✗ NO menciona UC Matrix (debería estar como dependency)
4. ✗ NO menciona now.md (debería actualizar después de cada fase)
5. ✗ NO menciona README.md para Stage 7 (debería crear uno)
6. ✗ Asume 12 stages THYROX cuando el proyecto tiene 6 stages simplificados
7. ✗ NO especifica ADR structure format (debería tener template)

## Acción correctiva requerida

**ANTES de continuar con Stage 7, DEBO:**

1. [ ] Leer ROADMAP.md (si existe)
2. [ ] Leer now.md para entender estado actual
3. [ ] Revisar GitHub Actions workflows
4. [ ] Analizar UC Matrix
5. [ ] Revisar README en directorios
6. [ ] Entender los 6 stages REALES del proyecto
7. [ ] Leer 3-5 ADRs existentes para entender estructura
8. [ ] ACTUALIZAR el PLAN para incluir estos 10 patrones

## Clasificación

- **Tipo:** Falta de diligencia exhaustiva (GRAVE)
- **Categoría:** Búsqueda incompleta de patrones
- **Raíz:** Asumí que las convenciones eran suficientes
- **Impacto:** PLAN incompleto, puede necesitar rehacer

## Próximos pasos

1. Documentar este ERROR-004
2. Buscar TODOS los 10 patrones en el proyecto
3. RECHAZAR el PLAN actual
4. CREAR nuevo PLAN que incluya los 10 patrones

---

**Archivo de error creado:** 2026-04-21 06:05:00
**Estado:** Documentado, PLAN debe ser rechazado
**Acción:** Búsqueda exhaustiva de 10 patrones ANTES de re-crear PLAN

