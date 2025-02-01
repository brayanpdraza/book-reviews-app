#!/bin/bash
set -e

cd backend/src

export PATH="$PATH:$HOME/.dotnet/tools"
dotnet tool install --global dotnet-ef --version 6.0.36 || true

echo "=== Directorio actual ==="
pwd

echo "=== Contenido de backend/src ==="
ls -la

echo "=== Aplicando migraciones ==="
dotnet ef database update \
  --project ./AdaptadorPostgreSQL/AdaptadorPostgreSQL.csproj \
  --startup-project ./AdaptadorAPI/AdaptadorAPI.csproj \
  --msbuildprojectextensionspath ./obj

echo "=== Compilando API ==="
dotnet publish -c Release -o ./publish AdaptadorAPI/AdaptadorAPI.csproj

echo "=== Iniciando API ==="
dotnet ./publish/AdaptadorAPI.dll