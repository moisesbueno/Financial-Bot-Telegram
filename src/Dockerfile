FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy only the csproj first so the restore layer is cached until dependencies change
COPY Financial.Bot.csproj ./
RUN dotnet restore Financial.Bot.csproj

# Copy the rest and publish (this also builds)
COPY . .
RUN dotnet publish Financial.Bot.csproj \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM base AS final
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Financial.Bot.dll"]