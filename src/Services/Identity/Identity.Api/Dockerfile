FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

COPY "The12thPlayer.sln" "The12thPlayer.sln"

COPY ["BuildingBlocks/MessageBus/MessageBus.Components/MessageBus.Components.csproj", "BuildingBlocks/MessageBus/MessageBus.Components/"]
COPY ["BuildingBlocks/MessageBus/MessageBus.Contracts/MessageBus.Contracts.csproj", "BuildingBlocks/MessageBus/MessageBus.Contracts/"]

COPY ["Services/Identity/Identity.Api/Identity.Api.csproj", "Services/Identity/Identity.Api/"]
COPY ["Services/Identity/Identity.Application/Identity.Application.csproj", "Services/Identity/Identity.Application/"]
COPY ["Services/Identity/Identity.Domain/Identity.Domain.csproj", "Services/Identity/Identity.Domain/"]
COPY ["Services/Identity/Identity.Infrastructure/Identity.Infrastructure.csproj", "Services/Identity/Identity.Infrastructure/"]

COPY ["Services/Profile/Profile.Api/Profile.Api.csproj", "Services/Profile/Profile.Api/"]
COPY ["Services/Profile/Profile.Application/Profile.Application.csproj", "Services/Profile/Profile.Application/"]
COPY ["Services/Profile/Profile.Domain/Profile.Domain.csproj", "Services/Profile/Profile.Domain/"]
COPY ["Services/Profile/Profile.Infrastructure/Profile.Infrastructure.csproj", "Services/Profile/Profile.Infrastructure/"]

COPY ["Services/Notification/Notification.Host/Notification.Host.csproj", "Services/Notification/Notification.Host/"]
COPY ["Services/Notification/Notification.Application/Notification.Application.csproj", "Services/Notification/Notification.Application/"]

COPY ["Deploy/Migrator/Migrator.csproj", "Deploy/Migrator/"]

COPY "docker-compose.dcproj" "docker-compose.dcproj"

RUN dotnet restore "The12thPlayer.sln"

COPY . .

WORKDIR /src/Services/Identity/Identity.Api
RUN dotnet publish --no-restore -c Release -o /app

FROM build AS publish

FROM base AS final
WORKDIR /app
EXPOSE 80
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Identity.Api.dll"]