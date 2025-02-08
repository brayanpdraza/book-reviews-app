#!/bin/bash
set -e

cd backend/src

export PATH="$PATH:$HOME/.dotnet/tools"
dotnet tool install --global dotnet-ef --version 6.0.36 || true

echo "=== Restaurando dependencias ==="
dotnet restore AdaptadorPostgreSQL/AdaptadorPostgreSQL.csproj

echo "=== Verificando migraciones pendientes ==="
if [[ $(dotnet ef migrations list --project ./AdaptadorPostgreSQL/AdaptadorPostgreSQL.csproj | wc -l) -gt 0 ]]; then
  echo "=== Aplicando migraciones ==="
  dotnet ef database update \
    --project ./AdaptadorPostgreSQL/AdaptadorPostgreSQL.csproj \
    --startup-project ./AdaptadorAPI/AdaptadorAPI.csproj
else
  echo "No hay migraciones pendientes, omitiendo actualización de la base de datos."
fi

echo "=== Compilando API ==="
dotnet publish -c Release -o ./publish AdaptadorAPI/AdaptadorAPI.csproj

echo "=== Iniciando API ==="
dotnet ./publish/AdaptadorAPI.dll
