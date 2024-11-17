# Etap bazowy
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Etap budowy projektu
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SofomoWeatherForecastAPI.csproj", "."]
RUN dotnet restore "./SofomoWeatherForecastAPI.csproj"
COPY . . 
WORKDIR "/src/."
RUN dotnet build "./SofomoWeatherForecastAPI.csproj" -c %BUILD_CONFIGURATION% -o /app/build

# Instalacja dotnet-ef
RUN dotnet tool install --global dotnet-ef
RUN apt-get update && apt-get install -y netcat-openbsd

# Ustawienie ścieżki dla narzędzi .NET
ENV PATH="$PATH:/root/.dotnet/tools"

# Skopiowanie pliku .csproj oraz zależności do kontenera, aby użyć migracji
COPY SofomoWeatherForecastAPI.csproj /app/

# Etap publikacji projektu
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SofomoWeatherForecastAPI.csproj" -c %BUILD_CONFIGURATION% -o /app/publish /p:UseAppHost=false

# Kopiowanie skryptu entrypoint
COPY entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh

# Ustawienie skryptu entrypoint jako punkt wejścia
ENTRYPOINT ["/entrypoint.sh"]
