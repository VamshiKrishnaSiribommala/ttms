USE [master];
GO

PRINT '=======================================================';
PRINT '  Idempotent SQL Server Database Restore & Init Script ';
PRINT '=======================================================';

-- 1. Restore TMS_2024_New if not already present
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'TMS_2024_New')
BEGIN
    PRINT '[INFO] Restoring database TMS_2024_New from backup...';

    DECLARE @dataPath NVARCHAR(512);
    DECLARE @logPath NVARCHAR(512);

    SELECT @dataPath = CAST(SERVERPROPERTY('InstanceDefaultDataPath') AS NVARCHAR(512));
    SELECT @logPath = CAST(SERVERPROPERTY('InstanceDefaultLogPath') AS NVARCHAR(512));

    IF @dataPath IS NULL OR @dataPath = ''
        SET @dataPath = '/var/opt/mssql/data/';
    IF @logPath IS NULL OR @logPath = ''
        SET @logPath = '/var/opt/mssql/data/';

    DECLARE @mdf NVARCHAR(512) = @dataPath + 'TMS_2024_New.mdf';
    DECLARE @ldf NVARCHAR(512) = @logPath + 'TMS_2024_New_log.ldf';

    RESTORE DATABASE [TMS_2024_New]
    FROM DISK = '/var/opt/mssql/backup/TMS_2024_New.bak'
    WITH REPLACE,
    MOVE 'TMS_2024_New' TO @mdf,
    MOVE 'TMS_2024_New_log' TO @ldf;

    ALTER DATABASE [TMS_2024_New] SET MULTI_USER;
    PRINT '[SUCCESS] Database TMS_2024_New restored successfully.';
END
ELSE
BEGIN
    PRINT '[INFO] Database TMS_2024_New already exists. Skipping restore.';
END
GO

-- 2. Restore TrainManagementDB if not already present
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'TrainManagementDB')
BEGIN
    PRINT '[INFO] Restoring database TrainManagementDB from backup...';

    DECLARE @dataPath NVARCHAR(512);
    DECLARE @logPath NVARCHAR(512);

    SELECT @dataPath = CAST(SERVERPROPERTY('InstanceDefaultDataPath') AS NVARCHAR(512));
    SELECT @logPath = CAST(SERVERPROPERTY('InstanceDefaultLogPath') AS NVARCHAR(512));

    IF @dataPath IS NULL OR @dataPath = ''
        SET @dataPath = '/var/opt/mssql/data/';
    IF @logPath IS NULL OR @logPath = ''
        SET @logPath = '/var/opt/mssql/data/';

    DECLARE @mdf NVARCHAR(512) = @dataPath + 'TrainManagementDB.mdf';
    DECLARE @ldf NVARCHAR(512) = @logPath + 'TrainManagementDB_log.ldf';

    RESTORE DATABASE [TrainManagementDB]
    FROM DISK = '/var/opt/mssql/backup/TrainManagementDB.bak'
    WITH REPLACE,
    MOVE 'TrainManagementDB' TO @mdf,
    MOVE 'TrainManagementDB_log' TO @ldf;

    ALTER DATABASE [TrainManagementDB] SET MULTI_USER;
    PRINT '[SUCCESS] Database TrainManagementDB restored successfully.';
END
ELSE
BEGIN
    PRINT '[INFO] Database TrainManagementDB already exists. Skipping restore.';
END
GO

-- 3. Ensure schema upgrades in TMS_2024_New
USE [TMS_2024_New];
GO

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
    PRINT '[INFO] Created Reg027_SafetyMeeting2 table.';
END
GO

PRINT '[SUCCESS] All databases and schemas verified and ready.';
GO
