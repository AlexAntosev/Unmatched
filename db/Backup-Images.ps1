<#
.SYNOPSIS
    Mirrors the MinIO "images" bucket (hero/villain/minion/player pictures) into a
    timestamped folder in this directory (images-{stamp}/, no zero-padding on the stamp).

.DESCRIPTION
    Reads MINIO_ROOT_USER / MINIO_ROOT_PASSWORD from the repo's .env file (see .env.example)
    and requires "mc", the MinIO Client (https://min.io/docs/minio/linux/reference/minio-mc.html),
    to be available on PATH.

    To back up the SQL Server database as well, run Backup-Database.ps1.

.EXAMPLE
    ./db/Backup-Images.ps1
    ./db/Backup-Images.ps1 -MinioEndpoint "http://localhost:9000" -MinioBucket images
#>

param(
    [string]$MinioEndpoint = "http://localhost:9000",
    [string]$MinioBucket = "images"
)

$ErrorActionPreference = "Stop"

$repoRoot = Split-Path -Parent $PSScriptRoot
$envFile = Join-Path $repoRoot ".env"

if (-not (Test-Path $envFile)) {
    throw "Could not find .env at $envFile - copy .env.example to .env and fill in MINIO_ROOT_USER/MINIO_ROOT_PASSWORD first."
}

$envValues = @{}
Get-Content $envFile | Where-Object { $_ -match '^\s*[^#].*=' } | ForEach-Object {
    $key, $value = $_.Split('=', 2)
    $envValues[$key.Trim()] = $value.Trim()
}

$minioUser = $envValues["MINIO_ROOT_USER"]
$minioPass = $envValues["MINIO_ROOT_PASSWORD"]

if (-not $minioUser -or -not $minioPass) {
    throw "MINIO_ROOT_USER and/or MINIO_ROOT_PASSWORD not found in $envFile"
}

if (-not (Get-Command mc -ErrorAction SilentlyContinue)) {
    throw "mc (MinIO Client) not found on PATH. Install it from https://min.io/docs/minio/linux/reference/minio-mc.html"
}

$now = Get-Date
$stamp = "$($now.Year)-$($now.Month)-$($now.Day)-$($now.Hour)-$($now.Minute)"

$minioAlias = "unmatched-backup"
mc alias set $minioAlias $MinioEndpoint $minioUser $minioPass | Out-Null

$imagesTargetDir = Join-Path $PSScriptRoot "images-$stamp"
Write-Host "Mirroring MinIO '$MinioBucket' bucket to $imagesTargetDir ..."

mc mirror "$minioAlias/$MinioBucket" $imagesTargetDir

Write-Host "Done: $imagesTargetDir"
