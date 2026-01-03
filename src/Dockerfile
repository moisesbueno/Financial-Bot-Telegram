FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base

USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["/Financial.Bot.csproj", "src/"]
RUN dotnet restore "src/Financial.Bot.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "./Financial.Bot.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Financial.Bot.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Financial.Bot.dll"]
