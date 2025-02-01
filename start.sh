#!/bin/bash
set -e

cd backend/src

export PATH="$PATH:$HOME/.dotnet/tools"
dotnet tool install --global dotnet-ef --version 6.0.36 || true


echo "=== Restaurando dependencias ==="
dotnet restore AdaptadorPostgreSQL/AdaptadorPostgreSQL.csproj

echo "=== Directorio actual ==="
pwd

echo "=== Contenido de backend/src ==="
ls -la

echo "=== Aplicando migraciones ==="
dotnet ef migrations add InitialCreate \
  --project ./AdaptadorPostgreSQL/AdaptadorPostgreSQL.csproj \
  --startup-project ./AdaptadorAPI/AdaptadorAPI.csproj


echo "=== Compilando API ==="
dotnet publish -c Release -o ./publish AdaptadorAPI/AdaptadorAPI.csproj

echo "=== Iniciando API ==="
dotnet ./publish/AdaptadorAPI.dll