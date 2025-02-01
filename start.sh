#!/bin/bash
set -e

dotnet tool install --global dotnet-ef --version 6.0.36 || true
export PATH="$PATH:/root/.dotnet/tools"

echo "Ejecutando migraciones de EF..."
dotnet ef database update \
  --project /app/AdaptadorPostgreSQL/AdaptadorPostgreSQL.csproj \
  --startup-project /app/API/API.csproj

echo "Iniciando la API..."
dotnet /app/out/API.dll