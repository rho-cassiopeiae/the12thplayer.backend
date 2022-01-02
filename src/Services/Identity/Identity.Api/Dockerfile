FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

COPY "The12thPlayer.sln" "The12thPlayer.sln"

COPY ["BuildingBlocks/MessageBus/MessageBus.Components/MessageBus.Components.csproj", "BuildingBlocks/MessageBus/MessageBus.Components/"]
COPY ["BuildingBlocks/MessageBus/MessageBus.Contracts/MessageBus.Contracts.csproj", "BuildingBlocks/MessageBus/MessageBus.Contracts/"]

COPY ["Services/Admin/Admin.Api/Admin.Api.csproj", "Services/Admin/Admin.Api/"]
COPY ["Services/Admin/Admin.Application/Admin.Application.csproj", "Services/Admin/Admin.Application/"]
COPY ["Services/Admin/Admin.Infrastructure/Admin.Infrastructure.csproj", "Services/Admin/Admin.Infrastructure/"]

COPY ["Services/Identity/Identity.Api/Identity.Api.csproj", "Services/Identity/Identity.Api/"]
COPY ["Services/Identity/Identity.Application/Identity.Application.csproj", "Services/Identity/Identity.Application/"]
COPY ["Services/Identity/Identity.Domain/Identity.Domain.csproj", "Services/Identity/Identity.Domain/"]
COPY ["Services/Identity/Identity.Infrastructure/Identity.Infrastructure.csproj", "Services/Identity/Identity.Infrastructure/"]

COPY ["Services/Profile/Profile.Api/Profile.Api.csproj", "Services/Profile/Profile.Api/"]
COPY ["Services/Profile/Profile.Application/Profile.Application.csproj", "Services/Profile/Profile.Application/"]
COPY ["Services/Profile/Profile.Domain/Profile.Domain.csproj", "Services/Profile/Profile.Domain/"]
COPY ["Services/Profile/Profile.Infrastructure/Profile.Infrastructure.csproj", "Services/Profile/Profile.Infrastructure/"]

COPY ["Services/Livescore/Livescore.Api/Livescore.Api.csproj", "Services/Livescore/Livescore.Api/"]
COPY ["Services/Livescore/Livescore.Application/Livescore.Application.csproj", "Services/Livescore/Livescore.Application/"]
COPY ["Services/Livescore/Livescore.Domain/Livescore.Domain.csproj", "Services/Livescore/Livescore.Domain/"]
COPY ["Services/Livescore/Livescore.Infrastructure/Livescore.Infrastructure.csproj", "Services/Livescore/Livescore.Infrastructure/"]

COPY ["Services/FileHostingGateway/FileHostingGateway.Host/FileHostingGateway.Host.csproj", "Services/FileHostingGateway/FileHostingGateway.Host/"]
COPY ["Services/FileHostingGateway/FileHostingGateway.Application/FileHostingGateway.Application.csproj", "Services/FileHostingGateway/FileHostingGateway.Application/"]
COPY ["Services/FileHostingGateway/FileHostingGateway.Infrastructure/FileHostingGateway.Infrastructure.csproj", "Services/FileHostingGateway/FileHostingGateway.Infrastructure/"]

COPY ["Services/Feed/Feed.Api/Feed.Api.csproj", "Services/Feed/Feed.Api/"]
COPY ["Services/Feed/Feed.Application/Feed.Application.csproj", "Services/Feed/Feed.Application/"]
COPY ["Services/Feed/Feed.Domain/Feed.Domain.csproj", "Services/Feed/Feed.Domain/"]
COPY ["Services/Feed/Feed.Infrastructure/Feed.Infrastructure.csproj", "Services/Feed/Feed.Infrastructure/"]

COPY ["Services/Notification/Notification.Host/Notification.Host.csproj", "Services/Notification/Notification.Host/"]
COPY ["Services/Notification/Notification.Application/Notification.Application.csproj", "Services/Notification/Notification.Application/"]

COPY ["Services/Worker/Worker.Host/Worker.Host.csproj", "Services/Worker/Worker.Host/"]
COPY ["Services/Worker/Worker.Application/Worker.Application.csproj", "Services/Worker/Worker.Application/"]
COPY ["Services/Worker/Worker.Infrastructure/Worker.Infrastructure.csproj", "Services/Worker/Worker.Infrastructure/"]

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