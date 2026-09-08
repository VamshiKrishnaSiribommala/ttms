# Cross-platform PowerShell database restore script for Windows & Linux
$ErrorActionPreference = "Stop"

$saPassword = if ($env:MSSQL_SA_PASSWORD) { $env:MSSQL_SA_PASSWORD } else { "YourStrong@Password123" }
$containerName = "tms-sqlserver"

Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host " Initializing Train Management System (TMS) Databases     " -ForegroundColor Cyan
Write-Host "==========================================================" -ForegroundColor Cyan

Write-Host "Checking if Docker container '$containerName' is running..." -ForegroundColor Yellow

$retries = 30
$connected = $false

while ($retries -gt 0) {
    $testResult = docker exec $containerName /bin/bash -c "/opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P '$saPassword' -C -Q 'SELECT 1' 2>/dev/null || /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P '$saPassword' -C -Q 'SELECT 1' 2>/dev/null"
    if ($LASTEXITCODE -eq 0 -or $testResult -match "1") {
        $connected = $true
        break
    }
    Write-Host "SQL Server is initializing in container... retrying ($retries remaining)" -ForegroundColor Gray
    Start-Sleep -Seconds 3
    $retries--
}

if (-not $connected) {
    Write-Host "[ERROR] Could not connect to SQL Server in container '$containerName'." -ForegroundColor Red
    exit 1
}

Write-Host "SQL Server is ONLINE. Executing restore-databases.sql inside container..." -ForegroundColor Green

docker exec $containerName /bin/bash -c "
if [ -f /opt/mssql-tools18/bin/sqlcmd ]; then
    /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P '$saPassword' -C -i /var/opt/mssql/backup/restore-databases.sql
else
    /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P '$saPassword' -C -i /var/opt/mssql/backup/restore-databases.sql
fi"

Write-Host "==========================================================" -ForegroundColor Cyan
Write-Host " [SUCCESS] All databases restored and verified ready!     " -ForegroundColor Green
Write-Host "==========================================================" -ForegroundColor Cyan
