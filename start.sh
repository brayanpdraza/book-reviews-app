#!/bin/sh
dotnet ef database update --project ./AdaptadorPostgreSQL/AdaptadorPostgreSQL.csproj --startup-project ./out/API.dll
dotnet ./out/API.dll