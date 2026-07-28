<#
.SYNOPSIS
    Exports the local "Unmatched" SQL Server database to a .bacpac file, and mirrors the MinIO
    "images" bucket (hero/villain/minion/player pictures) into a matching timestamped folder, both
    in this directory (Unmatched-{stamp}.bacpac / images-{stamp}/, no zero-padding on the stamp).

.DESCRIPTION
    Reads DB_USER / DB_PASS and MINIO_ROOT_USER / MINIO_ROOT_PASSWORD from the repo's .env file
    (see .env.example) and requires both "sqlpackage" (install with:
    dotnet tool install -g microsoft.sqlpackage) and "mc", the MinIO Client
    (https://min.io/docs/minio/linux/reference/minio-mc.html), to be available on PATH.

.EXAMPLE
    ./db/Backup-Database.ps1
    ./db/Backup-Database.ps1 -ServerName "localhost,1433" -DatabaseName Unmatched -MinioEndpoint "http://localhost:9000"
#>

param(
    [string]$ServerName = "localhost,1433",
    [string]$DatabaseName = "Unmatched",
    [string]$MinioEndpoint = "http://localhost:9000",
    [string]$MinioBucket = "images"
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$envFile = Join-Path $repoRoot ".env"

if (-not (Test-Path $envFile)) {
    throw "Could not find .env at $envFile - copy .env.example to .env and fill in DB_USER/DB_PASS first."
}

$envValues = @{}
Get-Content $envFile | Where-Object { $_ -match '^\s*[^#].*=' } | ForEach-Object {
    $key, $value = $_.Split('=', 2)
    $envValues[$key.Trim()] = $value.Trim()
}

$dbUser = $envValues["DB_USER"]
$dbPass = $envValues["DB_PASS"]

if (-not $dbUser -or -not $dbPass) {
    throw "DB_USER and/or DB_PASS not found in $envFile"
}

$minioUser = $envValues["MINIO_ROOT_USER"]
$minioPass = $envValues["MINIO_ROOT_PASSWORD"]

if (-not $minioUser -or -not $minioPass) {
    throw "MINIO_ROOT_USER and/or MINIO_ROOT_PASSWORD not found in $envFile"
}

if (-not (Get-Command sqlpackage -ErrorAction SilentlyContinue)) {
    throw "sqlpackage not found on PATH. Install it with: dotnet tool install -g microsoft.sqlpackage"
}

if (-not (Get-Command mc -ErrorAction SilentlyContinue)) {
    throw "mc (MinIO Client) not found on PATH. Install it from https://min.io/docs/minio/linux/reference/minio-mc.html"
}

$now = Get-Date
$stamp = "$($now.Year)-$($now.Month)-$($now.Day)-$($now.Hour)-$($now.Minute)"
$targetFile = Join-Path $PSScriptRoot "$DatabaseName-$stamp.bacpac"

Write-Host "Exporting $DatabaseName from $ServerName to $targetFile ..."

sqlpackage `
    /Action:Export `
    /SourceServerName:$ServerName `
    /SourceDatabaseName:$DatabaseName `
    /SourceUser:$dbUser `
    /SourcePassword:$dbPass `
    /SourceTrustServerCertificate:True `
    /TargetFile:$targetFile

Write-Host "Done: $targetFile"

$minioAlias = "unmatched-backup"
mc alias set $minioAlias $MinioEndpoint $minioUser $minioPass | Out-Null

$imagesTargetDir = Join-Path $PSScriptRoot "images-$stamp"
Write-Host "Mirroring MinIO '$MinioBucket' bucket to $imagesTargetDir ..."

mc mirror "$minioAlias/$MinioBucket" $imagesTargetDir

Write-Host "Done: $imagesTargetDir"
