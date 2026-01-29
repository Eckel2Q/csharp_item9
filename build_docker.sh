#\!/bin/bash
NAMESPACE="${1:-codebase_b1021_app}"
docker build -t "$NAMESPACE" .
