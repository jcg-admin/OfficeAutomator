#!/usr/bin/env bash
# validate-commit-message.sh — PreToolUse hook para Bash(git commit *)
# Valida que git commit demuestre los cambios realizados
# Input: JSON via stdin con tool_name y tool_input.command
# Output: JSON via stdout con permissionDecision

set -euo pipefail

INPUT=$(cat)
TOOL_NAME=$(echo "$INPUT" | python3 -c "import sys,json; d=json.load(sys.stdin); print(d.get('tool_name',''))" 2>/dev/null || echo "")

if [ "$TOOL_NAME" != "Bash" ]; then
  echo '{"hookSpecificOutput":{"hookEventName":"PreToolUse","permissionDecision":"allow"}}'
  exit 0
fi

COMMAND=$(echo "$INPUT" | python3 -c "import sys,json; d=json.load(sys.stdin); print(d.get('tool_input',{}).get('command',''))" 2>/dev/null || echo "")

# Solo interceptar git commit
if ! echo "$COMMAND" | grep -qE "git commit"; then
  echo '{"hookSpecificOutput":{"hookEventName":"PreToolUse","permissionDecision":"allow"}}'
  exit 0
fi

# Extraer mensaje del commit (dobles comillas o simples)
MSG=$(echo "$COMMAND" | grep -oP '(?<=-m ")[^"]+' 2>/dev/null || true)
if [ -z "$MSG" ]; then
  MSG=$(echo "$COMMAND" | grep -oP "(?<=-m ')[^']+" 2>/dev/null || true)
fi

if [ -z "$MSG" ]; then
  echo '{"hookSpecificOutput":{"hookEventName":"PreToolUse","permissionDecision":"allow"}}'
  exit 0
fi

# Validar que el mensaje sea descriptivo (minimo 10 caracteres)
# No exigir prefijo type(scope) estricto - lo importante es que demuestre los cambios
FIRST_LINE=$(echo "$MSG" | head -1)
MSG_LENGTH=${#FIRST_LINE}

if [ "$MSG_LENGTH" -lt 10 ]; then
  REASON="Commit message muy corto. Debe tener al menos 10 caracteres y describir los cambios realizados. Mensaje recibido: \"$FIRST_LINE\""
  echo "{\"hookSpecificOutput\":{\"hookEventName\":\"PreToolUse\",\"permissionDecision\":\"deny\",\"reason\":\"$REASON\"}}"
  exit 0
fi

# Validar que no este vacio o sea solo whitespace
if ! echo "$FIRST_LINE" | grep -qE '[a-zA-Z0-9]'; then
  REASON="Commit message debe contener caracteres alfanumericos descriptivos"
  echo "{\"hookSpecificOutput\":{\"hookEventName\":\"PreToolUse\",\"permissionDecision\":\"deny\",\"reason\":\"$REASON\"}}"
  exit 0
fi

echo '{"hookSpecificOutput":{"hookEventName":"PreToolUse","permissionDecision":"allow"}}'
