FROM mcr.microsoft.com/dotnet/runtime:8.0-alpine AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DiscordMusicBot.csproj", "./"]
RUN dotnet restore "DiscordMusicBot.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "DiscordMusicBot.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "DiscordMusicBot.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
RUN apk update -y && apk add --no-cache opus libsodium ffmpeg
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DiscordMusicBot.dll"]
