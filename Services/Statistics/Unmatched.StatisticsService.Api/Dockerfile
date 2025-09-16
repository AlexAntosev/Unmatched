# Stage 1: build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# copy csproj files and restore as distinct layers
COPY ["Services/Statistics/Unmatched.StatisticsService.Api/Unmatched.StatisticsService.Api.csproj", "Unmatched.StatisticsService.Api/"]
COPY ["Services/Statistics/Unmatched.StatisticsService.EntityFramework/Unmatched.StatisticsService.EntityFramework.csproj", "Unmatched.StatisticsService.EntityFramework/"]
COPY ["Services/Statistics/Unmatched.StatisticsService.Domain/Unmatched.StatisticsService.Domain.csproj", "Unmatched.StatisticsService.Domain/"]

RUN dotnet restore "Unmatched.StatisticsService.Api/Unmatched.StatisticsService.Api.csproj"

# copy everything else and build
COPY Services/Statistics/ .
WORKDIR "/src/Unmatched.StatisticsService.Api"
RUN dotnet publish -c Release -o /app/publish

# Stage 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Unmatched.StatisticsService.Api.dll"]