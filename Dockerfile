FROM mcr.microsoft.com/dotnet/runtime:8.0-alpine AS base
# FROM alpine AS base
WORKDIR /app
RUN apk update && apk add --no-cache ffmpeg icu
ENV SRC_DIR=/usr/lib
ENV DEST_DIR=/app
RUN ln -s $(ls -v $SRC_DIR/libopus.so* | tail -n 1) $DEST_DIR/libopus.so && \
    ln -s $(ls -v $SRC_DIR/libsodium.so* | tail -n 1) $DEST_DIR/libsodium.so
ARG UID=10001
RUN adduser \
    --disabled-password \
    --gecos "" \
    --home "/nonexistent" \
    --shell "/sbin/nologin" \
    --no-create-home \
    --uid "${UID}" \
    appuser

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
# RUN apk update && apk add --no-cache clang build-base zlib-dev
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DiscordMusicBot.csproj", "./"]
RUN dotnet restore "DiscordMusicBot.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "DiscordMusicBot.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "DiscordMusicBot.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=true --self-contained

FROM base AS final
COPY --chown=appuser --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DiscordMusicBot.dll"]
# ENTRYPOINT ["./DiscordMusicBot"]
