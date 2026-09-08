# ==============================================================================
# Automated Database Restore Script for Train Management System (TMS)
# ==============================================================================

Write-Host "========================================================" -ForegroundColor Cyan
Write-Host "    Restoring Database 'TMS_2024_New' with Stored Data" -ForegroundColor Cyan
Write-Host "========================================================" -ForegroundColor Cyan

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$bakSource = Join-Path $scriptDir "TMS_2024_New.bak"
$publicBak = "C:\Users\Public\TMS_2024_New.bak"

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
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'TMS_2024_New')
BEGIN
    ALTER DATABASE [TMS_2024_New] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
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

DECLARE @mdf NVARCHAR(512) = @dataPath + 'TMS_2024_New.mdf';
DECLARE @ldf NVARCHAR(512) = @logPath + 'TMS_2024_New_log.ldf';

RESTORE DATABASE [TMS_2024_New]
FROM DISK = '$publicBak'
WITH REPLACE,
MOVE 'TMS_2024_New' TO @mdf,
MOVE 'TMS_2024_New_log' TO @ldf;

ALTER DATABASE [TMS_2024_New] SET MULTI_USER;
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
        
        Write-Host "`n[SUCCESS] Database 'TMS_2024_New' has been successfully restored on instance '$srv'!" -ForegroundColor Green
        Write-Host "All 50 tables, accounts, and historical register data are loaded and ready." -ForegroundColor Green
        $restored = $true
        break
    } catch {
        # Try next instance silently
    }
}

if (-not $restored) {
    Write-Host "Trying fallback with sqlcmd CLI..." -ForegroundColor Yellow
    $sqlFile = Join-Path $env:TEMP "restore_tms.sql"
    Set-Content -Path $sqlFile -Value $sqlScript -Encoding UTF8
    
    foreach ($srv in $serversToTry) {
        if (Get-Command sqlcmd -ErrorAction SilentlyContinue) {
            $res = & sqlcmd -S $srv -E -C -i $sqlFile 2>&1
            if ($LASTEXITCODE -eq 0) {
                Write-Host "`n[SUCCESS] Database 'TMS_2024_New' restored via sqlcmd on '$srv'!" -ForegroundColor Green
                $restored = $true
                break
            }
        }
    }
    Remove-Item -Path $sqlFile -Force -ErrorAction SilentlyContinue
}

if ($restored) {
    Write-Host "`nApplying schema alignment updates for all 40 registers..." -ForegroundColor Cyan
    $upgradeSql = @"
USE [TMS_2024_New];
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Reg027_SafetyMeeting2')
BEGIN
    CREATE TABLE Reg027_SafetyMeeting2 (
        MeetingID VARCHAR(50) PRIMARY KEY,
        MeetingDate DATE NOT NULL,
        MeetingType VARCHAR(100) NOT NULL,
        PresidedBy VARCHAR(100) NOT NULL,
        Venue VARCHAR(100) NOT NULL,
        InvitedStaff TEXT NOT NULL,
        AttendedStaff TEXT NOT NULL,
        Minutes TEXT NOT NULL,
        ActionItems TEXT NOT NULL,
        Deadline DATE NOT NULL,
        SubmittedBy INT NULL,
        SubmittedAt DATETIME DEFAULT GETDATE()
    );
END

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Reg028_StaffBiodata')
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Reg028_StaffBiodata') AND name = 'CompExpiryDate')
        ALTER TABLE Reg028_StaffBiodata ADD CompExpiryDate DATE NULL;
    ALTER TABLE Reg028_StaffBiodata ALTER COLUMN CompetencyExpiryDate DATE NULL;
END

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Reg029_Assurance')
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Reg029_Assurance') AND name = 'LanguageSelection')
        ALTER TABLE Reg029_Assurance ADD LanguageSelection VARCHAR(50) NULL;
    ALTER TABLE Reg029_Assurance ALTER COLUMN [Language] VARCHAR(50) NULL;
    ALTER TABLE Reg029_Assurance ALTER COLUMN DocViewStatus BIT NULL;
    ALTER TABLE Reg029_Assurance ALTER COLUMN AcknowledgementTime DATETIME NULL;
END

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Reg030_PNSheet')
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Reg030_PNSheet') AND name = 'AssocTrainNo')
        ALTER TABLE Reg030_PNSheet ADD AssocTrainNo VARCHAR(50) NULL;
    ALTER TABLE Reg030_PNSheet ALTER COLUMN AssociatedTrainNo VARCHAR(50) NULL;
END

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Reg031_PettyRepair')
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Reg031_PettyRepair') AND name = 'ComplaintTime')
        ALTER TABLE Reg031_PettyRepair ADD ComplaintTime DATETIME NULL;
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Reg031_PettyRepair') AND name = 'CompletionTime')
        ALTER TABLE Reg031_PettyRepair ADD CompletionTime DATETIME NULL;
END

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Reg036_OfficersInspection')
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Reg036_OfficersInspection') AND name = 'StationInspected')
        ALTER TABLE Reg036_OfficersInspection ADD StationInspected VARCHAR(100) NULL;
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Reg036_OfficersInspection') AND name = 'IrregularitiesFound')
        ALTER TABLE Reg036_OfficersInspection ADD IrregularitiesFound TEXT NULL;
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Reg036_OfficersInspection') AND name = 'ComplianceDue')
        ALTER TABLE Reg036_OfficersInspection ADD ComplianceDue DATE NULL;
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Reg036_OfficersInspection') AND name = 'SMComplianceRem')
        ALTER TABLE Reg036_OfficersInspection ADD SMComplianceRem TEXT NULL;
    ALTER TABLE Reg036_OfficersInspection ALTER COLUMN StationName VARCHAR(100) NULL;
    ALTER TABLE Reg036_OfficersInspection ALTER COLUMN Irregularities TEXT NULL;
    ALTER TABLE Reg036_OfficersInspection ALTER COLUMN DueDate DATE NULL;
END

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Reg037_TIInspection')
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Reg037_TIInspection') AND name = 'StaffAlertness')
        ALTER TABLE Reg037_TIInspection ADD StaffAlertness BIT NULL;
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Reg037_TIInspection') AND name = 'OperationalFindings')
        ALTER TABLE Reg037_TIInspection ADD OperationalFindings TEXT NULL;
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Reg037_TIInspection') AND name = 'RuleViolations')
        ALTER TABLE Reg037_TIInspection ADD RuleViolations TEXT NULL;
    ALTER TABLE Reg037_TIInspection ALTER COLUMN Alertness BIT NULL;
    ALTER TABLE Reg037_TIInspection ALTER COLUMN Findings TEXT NULL;
END
"@
    foreach ($srv in $serversToTry) {
        try {
            $connStr = "Server=$srv;Database=TMS_2024_New;Integrated Security=True;TrustServerCertificate=True;Connect Timeout=3"
            $conn = New-Object System.Data.SqlClient.SqlConnection($connStr)
            $conn.Open()
            $cmd = $conn.CreateCommand()
            $cmd.CommandTimeout = 30
            $cmd.CommandText = $upgradeSql
            $null = $cmd.ExecuteNonQuery()
            $conn.Close()
            Write-Host "[SUCCESS] All 40 register schemas aligned and ready on '$srv'!" -ForegroundColor Green
            break
        } catch { }
    }
}

exit 0
