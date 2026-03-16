# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar archivos de solución y proyectos
COPY *.sln .
COPY NPS.WebAPI/*.csproj NPS.WebAPI/
COPY NPS.Application/*.csproj NPS.Application/
COPY NPS.Domain/*.csproj NPS.Domain/
COPY NPS.Infrastructure/*.csproj NPS.Infrastructure/

# Restaurar dependencias
RUN dotnet restore

# Copiar todo el código y compilar
COPY . .
WORKDIR /src/NPS.WebAPI
RUN dotnet publish -c Release -o /app/publish

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 80
EXPOSE 443

COPY --from=build /app/publish .

# Script de espera para SQL Server
COPY wait-for-it.sh /wait-for-it.sh
RUN chmod +x /wait-for-it.sh

ENTRYPOINT ["/wait-for-it.sh", "db:1433", "--", "dotnet", "NPS.WebAPI.dll"]