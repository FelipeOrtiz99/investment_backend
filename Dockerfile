FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/InvestmentBackend.WebApi/InvestmentBackend.WebApi.csproj", "src/InvestmentBackend.WebApi/"]
COPY ["src/InvestmentBackend.Application/InvestmentBackend.Application.csproj", "src/InvestmentBackend.Application/"]
COPY ["src/InvestmentBackend.Domain/InvestmentBackend.Domain.csproj", "src/InvestmentBackend.Domain/"]
COPY ["src/InvestmentBackend.Infrastructure/InvestmentBackend.Infrastructure.csproj", "src/InvestmentBackend.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "src/InvestmentBackend.WebApi/InvestmentBackend.WebApi.csproj"

# Copy all source code
COPY . .

# Build application
WORKDIR "/src/src/InvestmentBackend.WebApi"
RUN dotnet build "InvestmentBackend.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "InvestmentBackend.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Add health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost/health || exit 1

ENTRYPOINT ["dotnet", "InvestmentBackend.WebApi.dll"]
