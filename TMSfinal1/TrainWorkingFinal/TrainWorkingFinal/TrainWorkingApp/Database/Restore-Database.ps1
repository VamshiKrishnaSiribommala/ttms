# ==============================================================================
# Automated Database Restore Script for Train Working Management System (TMS)
# ==============================================================================

Write-Host "========================================================" -ForegroundColor Cyan
Write-Host "    Restoring Database 'TrainManagementDB' with Stored Data" -ForegroundColor Cyan
Write-Host "========================================================" -ForegroundColor Cyan

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$bakSource = Join-Path $scriptDir "TrainManagementDB.bak"
$publicBak = "C:\Users\Public\TrainManagementDB.bak"

if (-not (Test-Path $bakSource)) {
    Write-Host "[ERROR] Backup file not found at: $bakSource" -ForegroundColor Red
    exit 1
}

# Copy to public directory to ensure SQL Server service has read permissions
try {
    Copy-Item -Path $bakSource -Destination $publicBak -Force
    Write-Host "[INFO] Copied backup to $publicBak for SQL Server service access." -ForegroundColor Gray
} catch {
    Write-Host "[WARNING] Could not copy to C:\Users\Public, using direct path." -ForegroundColor Yellow
    $publicBak = $bakSource
}

$sqlScript = @"
USE [master];
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'TrainManagementDB')
BEGIN
    ALTER DATABASE [TrainManagementDB] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
END

DECLARE @dataPath NVARCHAR(512);
DECLARE @logPath NVARCHAR(512);

SELECT @dataPath = CAST(SERVERPROPERTY('InstanceDefaultDataPath') AS NVARCHAR(512));
SELECT @logPath = CAST(SERVERPROPERTY('InstanceDefaultLogPath') AS NVARCHAR(512));

IF @dataPath IS NULL OR @dataPath = ''
    SELECT @dataPath = SUBSTRING(physical_name, 1, CHARINDEX('master.mdf', LOWER(physical_name)) - 1)
    FROM master.sys.master_files WHERE database_id = 1 AND file_id = 1;

IF @logPath IS NULL OR @logPath = ''
    SET @logPath = @dataPath;

DECLARE @mdf NVARCHAR(512) = @dataPath + 'TrainManagementDB.mdf';
DECLARE @ldf NVARCHAR(512) = @logPath + 'TrainManagementDB_log.ldf';

RESTORE DATABASE [TrainManagementDB]
FROM DISK = '$publicBak'
WITH REPLACE,
MOVE 'TrainManagementDB' TO @mdf,
MOVE 'TrainManagementDB_log' TO @ldf;

ALTER DATABASE [TrainManagementDB] SET MULTI_USER;
"@

$serversToTry = @("localhost\SQLEXPRESS", ".\SQLEXPRESS", "(local)\SQLEXPRESS", ".", "localhost", "(local)", "(localdb)\MSSQLLocalDB")
$restored = $false

Write-Host "`nAttempting automated restore via .NET SQL Client..." -ForegroundColor Yellow

foreach ($srv in $serversToTry) {
    try {
        $connStr = "Server=$srv;Database=master;Integrated Security=True;TrustServerCertificate=True;Connect Timeout=3"
        $conn = New-Object System.Data.SqlClient.SqlConnection($connStr)
        $conn.Open()
        
        $cmd = $conn.CreateCommand()
        $cmd.CommandTimeout = 120
        $cmd.CommandText = $sqlScript
        $null = $cmd.ExecuteNonQuery()
        $conn.Close()
        
        Write-Host "`n[SUCCESS] Database 'TrainManagementDB' has been successfully restored on instance '$srv'!" -ForegroundColor Green
        Write-Host "All authority tables, accounts, and historical register data are loaded and ready." -ForegroundColor Green
        $restored = $true
        break
    } catch {
        # Try next instance silently
    }
}

if (-not $restored) {
    Write-Host "Trying fallback with sqlcmd CLI..." -ForegroundColor Yellow
    $sqlFile = Join-Path $env:TEMP "restore_trainworking.sql"
    Set-Content -Path $sqlFile -Value $sqlScript -Encoding UTF8
    
    foreach ($srv in $serversToTry) {
        if (Get-Command sqlcmd -ErrorAction SilentlyContinue) {
            $res = & sqlcmd -S $srv -E -C -i $sqlFile 2>&1
            if ($LASTEXITCODE -eq 0) {
                Write-Host "`n[SUCCESS] Database 'TrainManagementDB' restored via sqlcmd on '$srv'!" -ForegroundColor Green
                $restored = $true
                break
            }
        }
    }
    Remove-Item -Path $sqlFile -Force -ErrorAction SilentlyContinue
}

if (-not $restored) {
    Write-Host "`n[NOTICE] Automated restore could not find an active SQL Server instance." -ForegroundColor Red
    Write-Host "Please ensure Microsoft SQL Server Express is installed and running:" -ForegroundColor Yellow
    Write-Host "  1. Open Services (services.msc) and ensure 'SQL Server (SQLEXPRESS)' is Running." -ForegroundColor Gray
    Write-Host "  2. Or in SSMS: Right-click 'Databases' -> 'Restore Database...' -> Device -> Select 'Database\TrainManagementDB.bak'." -ForegroundColor Gray
    exit 1
}

exit 0
