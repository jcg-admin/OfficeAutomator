#!/bin/bash
# compare-branches.sh - Compara dos ramas sin errores
# Uso: bash compare-branches.sh [rama1] [rama2]
# Ejemplos:
#   bash compare-branches.sh main HEAD
#   bash compare-branches.sh main feature/project-structure-reorganization
#   bash compare-branches.sh develop stage-10-implementation

set -e

BRANCH1="${1:-main}"
BRANCH2="${2:-HEAD}"

echo "========================================"
echo "Comparador de Ramas Git"
echo "========================================"
echo ""

# Asegurar que las ramas existan localmente
for branch in "$BRANCH1" "$BRANCH2"; do
    if ! git rev-parse "$branch" > /dev/null 2>&1; then
        if git rev-parse "origin/$branch" > /dev/null 2>&1; then
            echo "[INFO] Creando rama local $branch desde origin/$branch..."
            git branch "$branch" "origin/$branch"
        else
            echo "[ERROR] Rama $branch no existe (ni local ni remota)"
            exit 1
        fi
    fi
done

echo ""
echo "Rama base:    $BRANCH1"
echo "Rama target:  $BRANCH2"
echo ""
echo "========================================"
echo "COMMITS ADELANTE"
echo "========================================"

COMMIT_COUNT=$(git log --oneline "$BRANCH1..$BRANCH2" 2>/dev/null | wc -l)
echo "Total: $COMMIT_COUNT commits"
echo ""

echo "========================================"
echo "CAMBIOS EN ARCHIVOS (resumen estadistico)"
echo "========================================"
git diff --stat "$BRANCH1..$BRANCH2" 2>/dev/null || echo "[VACIO]"
echo ""

echo "========================================"
echo "ARCHIVOS MODIFICADOS (nombres)"
echo "========================================"
FILE_COUNT=$(git diff --name-only "$BRANCH1..$BRANCH2" 2>/dev/null | wc -l)
echo "Total: $FILE_COUNT archivos"
echo ""
git diff --name-only "$BRANCH1..$BRANCH2" 2>/dev/null | sort || echo "[VACIO]"
echo ""

echo "========================================"
echo "HISTORIAL DE COMMITS"
echo "========================================"
git log --oneline --graph "$BRANCH1..$BRANCH2" 2>/dev/null | head -30 || echo "[VACIO]"

if [ $COMMIT_COUNT -gt 30 ]; then
    echo ""
    echo "... y $(($COMMIT_COUNT - 30)) commits mas"
fi

echo ""
echo "========================================"
echo "RESUMEN"
echo "========================================"
echo "Rama base: $BRANCH1"
echo "Rama target: $BRANCH2"
echo "Commits adelante: $COMMIT_COUNT"
echo "Archivos modificados: $FILE_COUNT"
echo "========================================"
