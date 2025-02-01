#!/bin/bash
set -e


dotnet tool install --global dotnet-ef || true


export PATH="$PATH:$HOME/.dotnet/tools"

echo "Ejecutando migraciones de EF..."
dotnet ef database update --project ./AdaptadorPostgreSQL/AdaptadorPostgreSQL.csproj --startup-project ./out/API.dll

echo "Iniciando la API..."
dotnet ./out/API.dll