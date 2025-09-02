FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
EXPOSE 5000
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["Unmatched.UI.BlazorServer/Unmatched.UI.BlazorServer.csproj", "Unmatched.UI.BlazorServer/"]
COPY ["Unmatched.EntityFramework/Unmatched.EntityFramework.csproj", "Unmatched.EntityFramework/"]
COPY ["Unmatched/Unmatched.csproj", "Unmatched/"]
COPY ["Unmatched.Data/Unmatched.Data.csproj", "Unmatched.Data/"]
RUN dotnet restore "Unmatched.UI.BlazorServer/Unmatched.UI.BlazorServer.csproj"
COPY . .
WORKDIR "/src/Unmatched.UI.BlazorServer"
RUN dotnet build "Unmatched.UI.BlazorServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Unmatched.UI.BlazorServer.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Unmatched.UI.BlazorServer.dll"]
