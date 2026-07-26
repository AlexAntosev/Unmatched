<#
.SYNOPSIS
    Restores the MinIO "images" bucket from a snapshot created by Backup-Database.ps1
    (db/images-{stamp}/), mirroring it back onto the running MinIO server.

.DESCRIPTION
    Reads MINIO_ROOT_USER / MINIO_ROOT_PASSWORD from the repo's .env file (see .env.example) and
    requires "mc", the MinIO Client (https://min.io/docs/minio/linux/reference/minio-mc.html), on
    PATH. By default restores from the most recently created db/images-* backup folder; pass
    -BackupFolder to pick a specific one.

.EXAMPLE
    ./db/Restore-Images.ps1
    ./db/Restore-Images.ps1 -BackupFolder "db/images-2026-7-26-17-2"
    ./db/Restore-Images.ps1 -MinioEndpoint "http://localhost:9000" -MinioBucket images
#>

param(
    [string]$MinioEndpoint = "http://localhost:9000",
    [string]$MinioBucket = "images",
    [string]$BackupFolder
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

if (-not $BackupFolder) {
    $latest = Get-ChildItem -Path $PSScriptRoot -Directory -Filter "images-*" |
        Sort-Object LastWriteTime -Descending |
        Select-Object -First 1

    if (-not $latest) {
        throw "No db/images-* backup folder found. Run Backup-Database.ps1 first, or pass -BackupFolder explicitly."
    }

    $BackupFolder = $latest.FullName
    Write-Host "No -BackupFolder given, using most recent snapshot: $BackupFolder"
}

if (-not (Test-Path $BackupFolder)) {
    throw "Backup folder not found: $BackupFolder"
}

$minioAlias = "unmatched-restore"
mc alias set $minioAlias $MinioEndpoint $minioUser $minioPass | Out-Null

Write-Host "Restoring '$BackupFolder' into MinIO '$MinioBucket' bucket ..."

mc mirror --overwrite $BackupFolder "$minioAlias/$MinioBucket"

Write-Host "Done. Bucket '$MinioBucket' restored from $BackupFolder."
