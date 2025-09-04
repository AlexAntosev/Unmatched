# ---------- Base runtime image ----------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8081       # HTTP
# EXPOSE 443    # uncomment if you configure HTTPS inside container

# ---------- Build stage ----------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy only project files first (layer caching for faster restores)
COPY ["Unmatched.UI.BlazorServer/Unmatched.UI.BlazorServer.csproj", "Unmatched.UI.BlazorServer/"]
COPY ["Unmatched.EntityFramework/Unmatched.EntityFramework.csproj", "Unmatched.EntityFramework/"]
COPY ["Unmatched/Unmatched.csproj", "Unmatched/"]
COPY ["Unmatched.Data/Unmatched.Data.csproj", "Unmatched.Data/"]

# Restore NuGet packages (runs only if csproj files changed)
RUN dotnet restore "Unmatched.UI.BlazorServer/Unmatched.UI.BlazorServer.csproj"

# Copy the rest of the source code
COPY . .

# Build the app
WORKDIR "/src/Unmatched.UI.BlazorServer"
RUN dotnet build -c Release -o /app/build

# ---------- Publish stage ----------
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# ---------- Final runtime image ----------
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Unmatched.UI.BlazorServer.dll"]