
set -e


cd backend/src


export PATH="$PATH:$HOME/.dotnet/tools"
dotnet tool install --global dotnet-ef --version 6.0.36 || true


echo "=== Directorio actual ==="
pwd

echo "=== Contenido de backend/src ==="
ls -la

echo "=== Contenido de adaptadorpostgrsql ==="
ls -la adaptadorpostgrsql

echo "=== Aplicando migraciones ==="
dotnet ef database update \
  --project ./adaptadorpostgresql/AdaptadorPostgreSQL.csproj \
  --startup-project .AdaptadorAPI/AdaptadorAPI.csproj \
  --msbuildprojectextensionspath ./obj

echo "=== Iniciando API ==="
dotnet ./AdaptadorAPI/bin/Release/net6.0/publish/API.dll