#!/bin/bash
set -e

cd backend/src

export PATH="$PATH:$HOME/.dotnet/tools"
dotnet tool install --global dotnet-ef --version 6.0.36 || true

echo "=== Restaurando dependencias ==="
dotnet restore AdaptadorPostgreSQL/AdaptadorPostgreSQL.csproj

echo "=== Verificando migraciones ==="
MIGRATIONS=$(dotnet ef migrations list --project ./AdaptadorPostgreSQL/AdaptadorPostgreSQL.csproj --startup-project ./AdaptadorAPI/AdaptadorAPI.csproj)

if [ -z "$MIGRATIONS" ]; then
  echo "⚠️ No hay migraciones. Creando InitialCreate..."
  dotnet ef migrations add InitialCreate --ignore-changes \
    --project ./AdaptadorPostgreSQL/AdaptadorPostgreSQL.csproj \
    --startup-project ./AdaptadorAPI/AdaptadorAPI.csproj
fi

echo "=== Aplicando migraciones ==="
dotnet ef database update \
  --project ./AdaptadorPostgreSQL/AdaptadorPostgreSQL.csproj \
  --startup-project ./AdaptadorAPI/AdaptadorAPI.csproj

echo "=== Compilando API ==="
dotnet publish -c Release -o ./publish AdaptadorAPI/AdaptadorAPI.csproj

echo "=== Iniciando API ==="
dotnet ./publish/AdaptadorAPI.dll