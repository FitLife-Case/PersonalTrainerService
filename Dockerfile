# ── Stage 1: Build ────────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["PersonalTrainerService.csproj", "PersonalTrainerService/"]
RUN dotnet restore "PersonalTrainerService/PersonalTrainerService.csproj"

COPY . ./PersonalTrainerService/
WORKDIR "/src/PersonalTrainerService"
RUN dotnet build "PersonalTrainerService.csproj" -c Release -o /app/build

# ── Stage 2: Publish ──────────────────────────────────────────────────────────
FROM build AS publish
RUN dotnet publish "PersonalTrainerService.csproj" -c Release -o /app/publish --no-restore

# ── Stage 3: Runtime ──────────────────────────────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080

COPY --from=publish /app/publish .
COPY --from=build /src/PersonalTrainerService/NLog.config .

ENTRYPOINT ["dotnet", "PersonalTrainerService.dll"]
