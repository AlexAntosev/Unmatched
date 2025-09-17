# ---------- Base runtime image ----------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8082       
# EXPOSE 443    # uncomment if you configure HTTPS inside container

# ---------- Build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy only project files first (layer caching for faster restores)
COPY ["Unmatched.Api/Unmatched.Api.csproj", "Unmatched.Api/"]
COPY ["Unmatched/Unmatched.csproj", "Unmatched/"]


# Restore NuGet packages (runs only if csproj files changed)
RUN dotnet restore "Unmatched.Api/Unmatched.Api.csproj"

# Copy the rest of the source code
COPY . .

# Build the app
WORKDIR "/src/Unmatched.Api"
RUN dotnet build -c Release -o /app/build

# ---------- Publish stage ----------
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# ---------- Final runtime image ----------
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Unmatched.Api.dll"]