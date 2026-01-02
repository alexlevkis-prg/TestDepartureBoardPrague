FROM mcr.microsoft.com/dotnet/runtime:9.0-nanoserver-ltsc2022 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0-nanoserver-ltsc2022 AS build
ARG configuration=Release
WORKDIR /src
COPY ["PIDTelegramBot.csproj", "./"]
RUN dotnet restore "PIDTelegramBot.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "PIDTelegramBot.csproj" -c $configuration -o /app/build

FROM build AS publish
ARG configuration=Release
RUN dotnet publish "PIDTelegramBot.csproj" -c $configuration -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PIDTelegramBot.dll"]
