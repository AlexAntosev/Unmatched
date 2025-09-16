# Stage 1: build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# copy csproj files and restore as distinct layers
COPY ["Services/Player/Unmatched.PlayerService.Api/Unmatched.PlayerService.Api.csproj", "Unmatched.PlayerService.Api/"]
COPY ["Services/Player/Unmatched.PlayerService.EntityFramework/Unmatched.PlayerService.EntityFramework.csproj", "Unmatched.PlayerService.EntityFramework/"]
COPY ["Services/Player/Unmatched.PlayerService.Domain/Unmatched.PlayerService.Domain.csproj", "Unmatched.PlayerService.Domain/"]

RUN dotnet restore "Unmatched.PlayerService.Api/Unmatched.PlayerService.Api.csproj"

# copy everything else and build
COPY Services/Player/ .
WORKDIR "/src/Unmatched.PlayerService.Api"
RUN dotnet publish -c Release -o /app/publish

# Stage 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Unmatched.PlayerService.Api.dll"]