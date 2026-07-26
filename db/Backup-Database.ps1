<#
.SYNOPSIS
    Exports the local "Unmatched" SQL Server database to a .bacpac file in this folder,
    named to match the existing backups here (Unmatched-{year}-{month}-{day}-{hour}-{minute}.bacpac,
    no zero-padding).

.DESCRIPTION
    Reads DB_USER / DB_PASS from the repo's .env file (see .env.example) and requires the
    "sqlpackage" tool to be available (install with: dotnet tool install -g microsoft.sqlpackage).

.EXAMPLE
    ./db/Backup-Database.ps1
    ./db/Backup-Database.ps1 -ServerName "localhost,1433" -DatabaseName Unmatched
#>

param(
    [string]$ServerName = "localhost,1433",
    [string]$DatabaseName = "Unmatched"
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

if (-not (Get-Command sqlpackage -ErrorAction SilentlyContinue)) {
    throw "sqlpackage not found on PATH. Install it with: dotnet tool install -g microsoft.sqlpackage"
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
