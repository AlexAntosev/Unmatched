# Stage 1: build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# copy csproj files and restore as distinct layers
COPY ["Services/Match/Unmatched.MatchService.Api/Unmatched.MatchService.Api.csproj", "Unmatched.MatchService.Api/"]
COPY ["Services/Match/Unmatched.MatchService.EntityFramework/Unmatched.MatchService.EntityFramework.csproj", "Unmatched.MatchService.EntityFramework/"]
COPY ["Services/Match/Unmatched.MatchService.Domain/Unmatched.MatchService.Domain.csproj", "Unmatched.MatchService.Domain/"]

RUN dotnet restore "Unmatched.MatchService.Api/Unmatched.MatchService.Api.csproj"

# copy everything else and build
COPY Services/Match/ .
WORKDIR "/src/Unmatched.MatchService.Api"
RUN dotnet publish -c Release -o /app/publish

# Stage 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Unmatched.MatchService.Api.dll"]