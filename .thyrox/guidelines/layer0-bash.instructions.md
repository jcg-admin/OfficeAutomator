```yml
type: Guía de Desarrollo
language: Bash 4.0+
layer: Layer 0 — Bootstrap
version: 1.0.0
purpose: Define clean code standards for Bash in OfficeAutomator
applies_to: setup.sh, verify-environment.sh, .claude/scripts/
updated_at: 2026-04-22 16:30:00
```

# GUÍA DE DESARROLLO — Bash 4.0+

**Clean Code Principles for OfficeAutomator Bootstrap Layer**

---

## TABLA DE CONTENIDOS

1. [Filosofía General](#filosofía-general)
2. [Convenciones Bash Específicas](#convenciones-bash-específicas)
3. [Clean Code Principles](#clean-code-principles)
4. [Estructura de Scripts](#estructura-de-scripts)
5. [Funciones Bash](#funciones-bash)
6. [Nombres Revelan Intención](#nombres-revelan-intención)
7. [Manejo de Errores](#manejo-de-errores)
8. [Validación y Verificación](#validación-y-verificación)
9. [Pruebas (BATS)](#pruebas-bats)
10. [Anti-Patrones Bash](#anti-patrones-bash)
11. [Checklist de Calidad](#checklist-de-calidad)

---

## FILOSOFÍA GENERAL

### Core Principle

**Bootstrap system environment with reliable, transparent, defensive Bash code.**

Bash es la capa de bootstrap. Debe ser:

```
THREE PILLARS:

1. SAFETY
   - Validación temprana (fail-fast)
   - Exit codes explícitos
   - Manejo robusto de errores

2. TRANSPARENCY
   - Usuario siempre sabe qué está pasando
   - Output legible y accionable
   - Errores descriptivos y claros

3. PORTABILITY
   - Compatible Bash 4.0+ (no bashisms modernos)
   - Sin dependencias externas innecesarias
   - Verificación de prerequisitos
```

---

## CONVENCIONES BASH ESPECÍFICAS

### Indentación y Espaciado

**Obligatorio: 2 o 4 espacios consistentes (no tabs)**

Bash permite 2 espacios por nivel. Para consistencia con PowerShell y C#, usar 4 espacios.

```bash
#!/bin/bash

function validate_environment() {
    local script_dir
    script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
    
    if [[ ! -d "$script_dir" ]]; then
        echo "ERROR: Invalid script directory" >&2
        return 1
    fi
    
    return 0
}
```

### Convención de Nomenclatura

| Tipo | Convención | Ejemplo | Explicación |
|------|-----------|---------|------------|
| **Función** | lowercase_with_underscores | `validate_environment()` | Always lowercase |
| **Variable global** | UPPERCASE_WITH_UNDERSCORES | `SCRIPT_DIR` | Global constants uppercase |
| **Variable local** | lowercase_with_underscores | `local_var` | Local = lowercase |
| **Parámetro función** | `$1, $2, ...` | `$1` | Positional, or use local vars |
| **Constante** | UPPERCASE_WITH_UNDERSCORES | `MAX_RETRIES` | All caps for constants |

```bash
#!/bin/bash

# Global constants
readonly SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
readonly MAX_RETRIES=3
readonly TIMEOUT_SECONDS=300

function download_file() {
    local source_url="$1"
    local destination_path="$2"
    local retry_count=0
    
    while [[ $retry_count -lt $MAX_RETRIES ]]; do
        if curl --max-time "$TIMEOUT_SECONDS" -o "$destination_path" "$source_url"; then
            echo "Download successful"
            return 0
        fi
        
        ((retry_count++))
        sleep 2
    done
    
    echo "ERROR: Download failed after $MAX_RETRIES attempts" >&2
    return 1
}
```

### Shebang y Requisitos

**Siempre especificar shebang:**
```bash
#!/bin/bash
```

NO usar:
- `#!/bin/sh` (muy restrictivo)
- `#!/usr/bin/env bash` (no portable en algunos entornos)

**Requisitos al inicio del script:**

```bash
#!/bin/bash

set -euo pipefail

# Validar Bash version
if [[ "${BASH_VERSINFO[0]}" -lt 4 ]]; then
    echo "ERROR: Bash 4.0 or higher required" >&2
    exit 1
fi

# Validar dependencias
required_commands=("curl" "grep" "sed")
for cmd in "${required_commands[@]}"; do
    if ! command -v "$cmd" &> /dev/null; then
        echo "ERROR: Required command not found: $cmd" >&2
        exit 1
    fi
done
```

**Explicación de `set -euo pipefail`:**
- `-e`: Exit inmediatamente en error (fail-fast)
- `-u`: Error al usar variable undefined
- `-o pipefail`: Pipe fails si CUALQUIER comando falla

---

## CLEAN CODE PRINCIPLES

### 1. Single Responsibility Principle (SRP)

Una función = Una responsabilidad bien definida

```bash
# INCORRECTO - Múltiples responsabilidades
function setup() {
    validate_bash_version
    check_dependencies
    download_office_tool
    verify_download
    extract_archive
    configure_environment
    cleanup_temp_files
    generate_log
}

# CORRECTO - Cada función responsable de una cosa
function validate_bash_version() { ... }
function check_dependencies() { ... }
function download_office_tool() { ... }
function verify_download() { ... }
function extract_archive() { ... }
function configure_environment() { ... }
function cleanup_temp_files() { ... }
function generate_log() { ... }
```

### 2. Reveal Intent

```bash
# INCORRECTO - Nombres poco claros
function v1() {
    local d="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
    local f="$d/config.xml"
    [[ -f "$f" ]] && return 0 || return 1
}

# CORRECTO - Nombres descriptivos
function validate_config_file_exists() {
    local script_directory
    script_directory="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
    
    local config_file="$script_directory/config.xml"
    [[ -f "$config_file" ]]
}
```

### 3. Fail-Fast Principle

```bash
# CORRECTO - Validar ANTES
function install_office() {
    local config_path="$1"
    
    # Validar PRIMERO
    if [[ -z "$config_path" ]]; then
        echo "ERROR: Configuration path required" >&2
        return 1
    fi
    
    if [[ ! -f "$config_path" ]]; then
        echo "ERROR: Configuration file not found: $config_path" >&2
        return 1
    fi
    
    # LUEGO proceder
    setup.exe /configure "$config_path"
}
```

### 4. DRY (Don't Repeat Yourself)

```bash
# INCORRECTO - Repetición
function validate_file_1() {
    if [[ ! -f "$1" ]]; then
        echo "ERROR: File not found: $1" >&2
        return 1
    fi
}

function validate_file_2() {
    if [[ ! -f "$1" ]]; then
        echo "ERROR: File not found: $1" >&2
        return 1
    fi
}

# CORRECTO - Función reutilizable
function validate_file_exists() {
    local filepath="$1"
    if [[ ! -f "$filepath" ]]; then
        echo "ERROR: File not found: $filepath" >&2
        return 1
    fi
}
```

### 5. KISS (Keep It Simple, Stupid)

```bash
# INCORRECTO - Sobre-ingenierizado
function complex_check() {
    [[ -f "$1" ]] && [[ -r "$1" ]] && [[ -s "$1" ]] && { cat "$1" | grep -q "pattern" || return 1; } && return 0 || return 1
}

# CORRECTO - Simple y legible
function validate_config_file() {
    local config_file="$1"
    
    [[ -f "$config_file" ]] || return 1
    [[ -r "$config_file" ]] || return 1
    [[ -s "$config_file" ]] || return 1
}
```

---

## ESTRUCTURA DE SCRIPTS

### Layout Estándar

```bash
#!/bin/bash

set -euo pipefail

# ============================================================================
# SCRIPT METADATA
# ============================================================================

readonly SCRIPT_NAME="$(basename "${BASH_SOURCE[0]}")"
readonly SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
readonly VERSION="1.0.0"

# ============================================================================
# CONSTANTS
# ============================================================================

readonly MAX_RETRIES=3
readonly TIMEOUT_SECONDS=300
readonly LOG_FILE="$SCRIPT_DIR/setup.log"

# ============================================================================
# UTILITY FUNCTIONS
# ============================================================================

function log_info() {
    local message="$1"
    echo "[INFO] $message" | tee -a "$LOG_FILE"
}

function log_error() {
    local message="$1"
    echo "[ERROR] $message" >&2 | tee -a "$LOG_FILE"
}

# ============================================================================
# MAIN LOGIC
# ============================================================================

function main() {
    log_info "Starting environment setup"
    
    if ! validate_environment; then
        log_error "Environment validation failed"
        return 1
    fi
    
    log_info "Environment validation passed"
    return 0
}

# ============================================================================
# ENTRY POINT
# ============================================================================

if [[ "${BASH_SOURCE[0]}" == "${0}" ]]; then
    main "$@"
    exit $?
fi
```

---

## FUNCIONES BASH

### Template Estándar

```bash
function description_of_what_function_does() {
    # Usage: description_of_what_function_does <param1> <param2>
    # Returns: 0 on success, 1 on failure
    # Output: Writes to stdout/stderr as appropriate
    
    local param1="$1"
    local param2="$2"
    
    # Validación de parámetros
    if [[ -z "$param1" ]]; then
        echo "ERROR: param1 is required" >&2
        return 1
    fi
    
    # Lógica principal
    if process_something "$param1" "$param2"; then
        echo "Operation successful"
        return 0
    else
        echo "ERROR: Operation failed" >&2
        return 1
    fi
}
```

### Máximo de Líneas por Función

Bash: máximo 30 líneas. Si es más largo, dividir en funciones más pequeñas.

```bash
# CORRECTO - Funciones cortas, responsabilidades claras
function download_file() {
    local url="$1"
    local output="$2"
    
    if curl -o "$output" "$url"; then
        return 0
    else
        echo "ERROR: Download failed" >&2
        return 1
    fi
}

function verify_file_hash() {
    local filepath="$1"
    local expected_hash="$2"
    
    local actual_hash
    actual_hash="$(sha256sum "$filepath" | awk '{print $1}')"
    
    [[ "$actual_hash" == "$expected_hash" ]]
}

function setup_file() {
    local url="$1"
    local output="$2"
    local hash="$3"
    
    if ! download_file "$url" "$output"; then
        return 1
    fi
    
    if ! verify_file_hash "$output" "$hash"; then
        echo "ERROR: File verification failed" >&2
        return 1
    fi
    
    return 0
}
```

---

## NOMBRES REVELAN INTENCIÓN

```bash
# INCORRECTO
function v() {
    [[ -f "f.xml" ]] && return 0 || return 1
}

x=$(v)
if [[ $x -eq 0 ]]; then
    echo "OK"
fi

# CORRECTO
function validate_configuration_xml_exists() {
    local config_file="./config.xml"
    [[ -f "$config_file" ]]
}

config_is_valid=$(validate_configuration_xml_exists)
if [[ $config_is_valid -eq 0 ]]; then
    echo "Configuration file validation passed"
fi
```

---

## MANEJO DE ERRORES

### Try-Catch Pattern

```bash
function safe_operation() {
    local file="$1"
    
    # Pattern 1: if-then-else
    if ! rm -f "$file"; then
        echo "ERROR: Failed to remove file: $file" >&2
        return 1
    fi
    
    return 0
}

function operation_with_cleanup() {
    local temp_dir
    temp_dir="$(mktemp -d)" || {
        echo "ERROR: Failed to create temp directory" >&2
        return 1
    }
    
    # Cleanup en caso de error
    trap "rm -rf '$temp_dir'" RETURN
    
    # Operación principal
    if ! process_in_temp "$temp_dir"; then
        echo "ERROR: Processing failed" >&2
        return 1
    fi
    
    return 0
}
```

### Exit Codes Explícitos

```bash
function check_status() {
    local result="$1"
    
    case "$result" in
        0)
            echo "SUCCESS"
            return 0
            ;;
        1)
            echo "ERROR: General failure" >&2
            return 1
            ;;
        2)
            echo "ERROR: Validation failed" >&2
            return 1
            ;;
        *)
            echo "ERROR: Unknown status: $result" >&2
            return 1
            ;;
    esac
}
```

---

## VALIDACIÓN Y VERIFICACIÓN

### Validación de Entrada

```bash
function process_file() {
    local filepath="$1"
    
    # Validar parámetro proporcionado
    if [[ $# -lt 1 ]]; then
        echo "ERROR: filepath parameter required" >&2
        return 1
    fi
    
    # Validar que no está vacío
    if [[ -z "$filepath" ]]; then
        echo "ERROR: filepath cannot be empty" >&2
        return 1
    fi
    
    # Validar que el archivo existe
    if [[ ! -f "$filepath" ]]; then
        echo "ERROR: File not found: $filepath" >&2
        return 1
    fi
    
    # Validar que el archivo es legible
    if [[ ! -r "$filepath" ]]; then
        echo "ERROR: File not readable: $filepath" >&2
        return 1
    fi
    
    return 0
}
```

### Verificación de SHA256

```bash
function verify_file_integrity() {
    local filepath="$1"
    local expected_sha256="$2"
    
    if [[ ! -f "$filepath" ]]; then
        echo "ERROR: File not found: $filepath" >&2
        return 1
    fi
    
    local actual_sha256
    actual_sha256="$(sha256sum "$filepath" | awk '{print $1}')"
    
    if [[ "$actual_sha256" != "$expected_sha256" ]]; then
        echo "ERROR: SHA256 mismatch" >&2
        echo "  Expected: $expected_sha256" >&2
        echo "  Actual: $actual_sha256" >&2
        return 1
    fi
    
    echo "File integrity verified"
    return 0
}
```

---

## PRUEBAS (BATS)

### Framework BATS (Bash Automated Testing System)

```bash
#!/usr/bin/env bats

setup() {
    export TEST_DIR="$(mktemp -d)"
}

teardown() {
    rm -rf "$TEST_DIR"
}

@test "validate_file_exists returns 0 when file exists" {
    local test_file="$TEST_DIR/test.txt"
    echo "content" > "$test_file"
    
    run validate_file_exists "$test_file"
    [ $status -eq 0 ]
}

@test "validate_file_exists returns 1 when file missing" {
    run validate_file_exists "$TEST_DIR/nonexistent.txt"
    [ $status -eq 1 ]
    [[ "$output" =~ "File not found" ]]
}

@test "verify_file_integrity succeeds with matching hash" {
    local test_file="$TEST_DIR/test.txt"
    echo "content" > "$test_file"
    
    local expected_hash
    expected_hash="$(sha256sum "$test_file" | awk '{print $1}')"
    
    run verify_file_integrity "$test_file" "$expected_hash"
    [ $status -eq 0 ]
}

@test "verify_file_integrity fails with mismatched hash" {
    local test_file="$TEST_DIR/test.txt"
    echo "content" > "$test_file"
    
    local wrong_hash="0000000000000000000000000000000000000000000000000000000000000000"
    
    run verify_file_integrity "$test_file" "$wrong_hash"
    [ $status -eq 1 ]
}
```

---

## ANTI-PATRONES BASH

### 1. Magic Numbers

```bash
# INCORRECTO
if [[ $retry_count -gt 3 ]]; then
    break
fi

# CORRECTO
readonly MAX_RETRIES=3
if [[ $retry_count -gt $MAX_RETRIES ]]; then
    break
fi
```

### 2. Unquoted Variables

```bash
# INCORRECTO - Vulnerable a word splitting
rm -rf $temp_dir

# CORRECTO - Siempre citar
rm -rf "$temp_dir"
```

### 3. Silent Failures

```bash
# INCORRECTO - Error ignorado
download_file "$url" "$output"
echo "Downloaded successfully"

# CORRECTO - Validar resultado
if ! download_file "$url" "$output"; then
    echo "ERROR: Download failed" >&2
    return 1
fi
echo "Downloaded successfully"
```

### 4. Global Variables

```bash
# INCORRECTO - Global state
temp_dir="/tmp/office-install"

function cleanup() {
    rm -rf "$temp_dir"
}

# CORRECTO - Variables locales
function create_temp_and_process() {
    local temp_dir
    temp_dir="$(mktemp -d)"
    trap "rm -rf '$temp_dir'" RETURN
    
    process_in_temp "$temp_dir"
}
```

### 5. sed/awk One-Liners sin Comentarios

```bash
# INCORRECTO - Incomprehensible
result=$(echo "$data" | sed 's/^[^:]*://g' | awk '{print $NF}')

# CORRECTO - Explicar la intención
# Extract everything after first colon, then get last field
result=$(echo "$data" | sed 's/^[^:]*://g' | awk '{print $NF}')
```

---

## CHECKLIST DE CALIDAD

### Antes de Commit

- [ ] Script tiene shebang correcto: `#!/bin/bash`
- [ ] `set -euo pipefail` presente en scripts principales
- [ ] Todas las variables están quoted: `"$var"`
- [ ] Funciones tienen nombres descriptivos (lowercase_with_underscores)
- [ ] Máximo 30 líneas por función
- [ ] Constantes están en UPPERCASE_WITH_UNDERSCORES
- [ ] No hay números mágicos (usar constantes)
- [ ] Validación de entrada temprana (fail-fast)
- [ ] Manejo de errores robusto (exit codes)
- [ ] Logging en puntos clave
- [ ] No hay variables globales mutables
- [ ] Funciones retornan código de salida claro (0 = éxito, 1 = fallo)

### Antes de Pull Request

- [ ] Script es executable: `chmod +x script.sh`
- [ ] BATS tests escritos y pasando
- [ ] Bash 4.0+ compatible (sin bashisms modernos)
- [ ] Prueba manual en al menos 2 entornos
- [ ] No hay dependencias externas implícitas
- [ ] Documentación de función con Usage/Returns/Output
- [ ] Error messages son claros y accionables
- [ ] Exit codes documentados en código
- [ ] No hay código comentado (git history)

### Code Review

- [ ] Script es legible sin conocimiento previo
- [ ] Responsabilidad única por función
- [ ] Manejo de errores es robusto
- [ ] Validación es exhaustiva
- [ ] No introduce deuda técnica
- [ ] Exit codes son predecibles

---

## RESOURCES

### Bash Reference

- GNU Bash Manual: https://www.gnu.org/software/bash/manual/
- ShellCheck (linter): https://www.shellcheck.net/
- Google Shell Style Guide: https://google.github.io/styleguide/shellguide.html

### Testing

- BATS Documentation: https://bats-core.readthedocs.io/
- ShellSpec Alternative: https://shellspec.info/

---

**Versión:** 1.0.0
**Última actualización:** 2026-04-22
**Aplicable a:** OfficeAutomator v1.0.0+

**Principio fundamental:**

Bash está en la capa de bootstrap. Su responsabilidad es validar el entorno del sistema y prepararlo para las capas superiores (PowerShell, C#). El código debe ser defensivo, transparente y fail-fast.
