# Stage 1: build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# copy csproj files and restore as distinct layers
COPY ["Services/Catalog/Unmatched.CatalogService.Api/Unmatched.CatalogService.Api.csproj", "Unmatched.CatalogService.Api/"]
COPY ["Services/Catalog/Unmatched.CatalogService.EntityFramework/Unmatched.CatalogService.EntityFramework.csproj", "Unmatched.CatalogService.EntityFramework/"]
COPY ["Services/Catalog/Unmatched.CatalogService.Domain/Unmatched.CatalogService.Domain.csproj", "Unmatched.CatalogService.Domain/"]

RUN dotnet restore "Unmatched.CatalogService.Api/Unmatched.CatalogService.Api.csproj"

# copy everything else and build
COPY Services/Catalog/ .
WORKDIR "/src/Unmatched.CatalogService.Api"
RUN dotnet publish -c Release -o /app/publish

# Stage 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Unmatched.CatalogService.Api.dll"]