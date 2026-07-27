# Runs Unmatched.UI.BlazorServer on the host against the backend services that docker-compose
# already exposes (catalog 5001, match 5003, player 5004, statistics 5007, MinIO 9000). Handy for
# UI work: the front end reloads in seconds without rebuilding any container.
#
# The MinIO credentials come from .env, the same file docker-compose reads.
#
# CAVEAT: without the .NET 9 runtime installed, the roll-forward below runs the app on .NET 10,
# where MapBlazorHub does not serve /_framework/blazor.server.js - the page prerenders correctly
# but nothing is interactive. Server-rendered markup, styling and data can be checked here;
# clicks, filters and forms have to be checked against the container (docker compose up blazor-ui).
$ErrorActionPreference = 'Stop'
$root = $PSScriptRoot

foreach ($line in Get-Content (Join-Path $root '.env')) {
    if ($line -match '^\s*([^#=]+?)\s*=\s*(.*?)\s*$') {
        Set-Item -Path "Env:$($Matches[1])" -Value $Matches[2]
    }
}

# The repo targets net9.0 but this machine only has the .NET 10 runtime installed, so let the
# host roll forward. Containers ship their own net9.0 runtime and are unaffected.
$env:DOTNET_ROLL_FORWARD = 'LatestMajor'
$env:ASPNETCORE_ENVIRONMENT = 'Development'
$env:ASPNETCORE_URLS = 'http://localhost:5133'
$env:Services__CatalogService__BaseUrl = 'http://localhost:5001'
$env:Services__MatchService__BaseUrl = 'http://localhost:5003'
$env:Services__PlayerService__BaseUrl = 'http://localhost:5004'
$env:Services__StatisticsService__BaseUrl = 'http://localhost:5007'
$env:Minio__Endpoint = 'localhost:9000'
$env:Minio__AccessKey = $env:MINIO_ROOT_USER
$env:Minio__SecretKey = $env:MINIO_ROOT_PASSWORD

dotnet run --project (Join-Path $root 'Unmatched.UI.BlazorServer\Unmatched.UI.BlazorServer.csproj') --no-launch-profile
