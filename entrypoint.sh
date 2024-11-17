#!/bin/bash
echo "Waiting for SQL Server to be available..."
until nc -z -v -w30 db 1433
do
  echo "Waiting for SQL Server..."
  sleep 5
done

# Uruchom migracjê
echo "Running migrations..."
dotnet ef database update --project /app/SofomoWeatherForecastAPI.csproj

echo "Starting application..."
dotnet /app/publish/SofomoWeatherForecastAPI.dll