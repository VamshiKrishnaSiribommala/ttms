using System;
using System.Data;
using System.Data.SqlClient;

namespace TMS
{
    public class DatabaseHelper
    {
        public static bool LastExecutionSuccessful { get; set; } = true;
        private static bool _schemaInitialized = false;
        private static readonly object _schemaLock = new object();
        private string connectionString = @"Data Source=localhost\SQLEXPRESS;Initial Catalog=TMS_2024_New;Integrated Security=True;TrustServerCertificate=True";

        public DatabaseHelper()
        {
            EnsureSchemaReady();
        }

        private void EnsureSchemaReady()
        {
            if (!_schemaInitialized)
            {
                lock (_schemaLock)
                {
                    if (!_schemaInitialized)
                    {
                        InitializeAuthSchema();
                        EnsureDatabaseSchemaUpgraded();
                        _schemaInitialized = true;
                    }
                }
            }
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }

        public DataTable ExecuteQuery(string query)
        {
            using (SqlConnection conn = GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        public int ExecuteNonQuery(string query)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        conn.Open();
                        int res = cmd.ExecuteNonQuery();
                        LastExecutionSuccessful = true;
                        return res;
                    }
                }
            }
            catch
            {
                LastExecutionSuccessful = false;
                throw;
            }
        }

        public object ExecuteScalar(string query)
        {
            using (SqlConnection conn = GetConnection())
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    return cmd.ExecuteScalar();
                }
            }
        }

        public DataTable ExecuteQuery(SqlCommand cmd)
        {
            using (SqlConnection conn = GetConnection())
            {
                cmd.Connection = conn;
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }

        public int ExecuteNonQuery(SqlCommand cmd)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    cmd.Connection = conn;
                    conn.Open();
                    int res = cmd.ExecuteNonQuery();
                    LastExecutionSuccessful = true;
                    return res;
                }
            }
            catch
            {
                LastExecutionSuccessful = false;
                throw;
            }
        }

        public object ExecuteScalar(SqlCommand cmd)
        {
            using (SqlConnection conn = GetConnection())
            {
                cmd.Connection = conn;
                conn.Open();
                return cmd.ExecuteScalar();
            }
        }

        public void InitializeAuthSchema()
        {
            try
            {
                string script = @"
                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TMS_Admins')
                    BEGIN
                        CREATE TABLE TMS_Admins (
                            AdminId INT IDENTITY(1,1) PRIMARY KEY,
                            Username NVARCHAR(50) NOT NULL UNIQUE,
                            PasswordHash NVARCHAR(256) NOT NULL,
                            Salt NVARCHAR(100) NOT NULL,
                            FullName NVARCHAR(100) NOT NULL,
                            Email NVARCHAR(100) NULL,
                            CreatedAt DATETIME DEFAULT GETDATE()
                        );
                    END

                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TMS_Users')
                    BEGIN
                        CREATE TABLE TMS_Users (
                            UserId INT IDENTITY(1,1) PRIMARY KEY,
                            FullName NVARCHAR(100) NOT NULL,
                            DateOfBirth DATE NULL,
                            Gender NVARCHAR(20) NULL,
                            Email NVARCHAR(100) NOT NULL,
                            Phone NVARCHAR(20) NOT NULL,
                            Address NVARCHAR(255) NULL,
                            Department NVARCHAR(100) NOT NULL,
                            Course NVARCHAR(100) NULL,
                            YearOfJoining NVARCHAR(50) NULL,
                            CollegeOrOrg NVARCHAR(150) NULL,
                            Username NVARCHAR(50) NOT NULL UNIQUE,
                            PasswordHash NVARCHAR(256) NOT NULL,
                            Salt NVARCHAR(100) NOT NULL,
                            Status NVARCHAR(20) NOT NULL DEFAULT 'Active',
                            CreatedAt DATETIME DEFAULT GETDATE(),
                            UpdatedAt DATETIME DEFAULT GETDATE()
                        );
                    END

                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'TMS_PasswordResetLogs')
                    BEGIN
                        CREATE TABLE TMS_PasswordResetLogs (
                            LogId INT IDENTITY(1,1) PRIMARY KEY,
                            UserId INT NOT NULL,
                            Username NVARCHAR(50) NOT NULL,
                            FullName NVARCHAR(100) NOT NULL,
                            Phone NVARCHAR(20) NOT NULL,
                            ResetTime DATETIME DEFAULT GETDATE(),
                            Status NVARCHAR(100) DEFAULT 'Password Reset Successful',
                            IPAddressOrHost NVARCHAR(100) NULL,
                            AdminNotified BIT DEFAULT 0
                        );
                    END
                ";
                ExecuteNonQuery(script);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Database auth schema initialization error: " + ex.Message);
            }
        }

        public void EnsureDatabaseSchemaUpgraded()
        {
            string[] upgradeStatements = new string[]
            {
                // Reg026 Circular Document column
                "IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Reg026_SafetyCircular') ALTER TABLE Reg026_SafetyCircular ALTER COLUMN CircularDocument VARBINARY(MAX) NULL;",
                
                // Reg027 Table Creation
                @"IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Reg027_SafetyMeeting2')
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
                  END;",

                // Reg028 Staff Biodata
                @"IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Reg028_StaffBiodata') 
                  BEGIN 
                      IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Reg028_StaffBiodata') AND name = 'CompExpiryDate') 
                          ALTER TABLE Reg028_StaffBiodata ADD CompExpiryDate DATE NULL; 
                      ALTER TABLE Reg028_StaffBiodata ALTER COLUMN CompetencyExpiryDate DATE NULL; 
                  END;",

                // Reg029 Assurance
                @"IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Reg029_Assurance') 
                  BEGIN 
                      IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Reg029_Assurance') AND name = 'LanguageSelection') 
                          ALTER TABLE Reg029_Assurance ADD LanguageSelection VARCHAR(50) NULL; 
                      ALTER TABLE Reg029_Assurance ALTER COLUMN [Language] VARCHAR(50) NULL; 
                      ALTER TABLE Reg029_Assurance ALTER COLUMN DocViewStatus BIT NULL; 
                      ALTER TABLE Reg029_Assurance ALTER COLUMN AcknowledgementTime DATETIME NULL; 
                  END;",

                // Reg030 PN Sheet
                @"IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Reg030_PNSheet') 
                  BEGIN 
                      IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Reg030_PNSheet') AND name = 'AssocTrainNo') 
                          ALTER TABLE Reg030_PNSheet ADD AssocTrainNo VARCHAR(50) NULL; 
                      ALTER TABLE Reg030_PNSheet ALTER COLUMN AssociatedTrainNo VARCHAR(50) NULL; 
                  END;",

                // Reg031 Petty Repair
                @"IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Reg031_PettyRepair') 
                  BEGIN 
                      IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Reg031_PettyRepair') AND name = 'ComplaintTime') 
                          ALTER TABLE Reg031_PettyRepair ADD ComplaintTime DATETIME NULL; 
                      IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Reg031_PettyRepair') AND name = 'CompletionTime') 
                          ALTER TABLE Reg031_PettyRepair ADD CompletionTime DATETIME NULL; 
                  END;",

                // Reg036 Officers Inspection
                @"IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Reg036_OfficersInspection') 
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
                  END;",

                // Reg037 TI Inspection
                @"IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Reg037_TIInspection') 
                  BEGIN 
                      IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Reg037_TIInspection') AND name = 'StaffAlertness') 
                          ALTER TABLE Reg037_TIInspection ADD StaffAlertness BIT NULL; 
                      IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Reg037_TIInspection') AND name = 'OperationalFindings') 
                          ALTER TABLE Reg037_TIInspection ADD OperationalFindings TEXT NULL; 
                      IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Reg037_TIInspection') AND name = 'RuleViolations') 
                          ALTER TABLE Reg037_TIInspection ADD RuleViolations TEXT NULL; 
                      ALTER TABLE Reg037_TIInspection ALTER COLUMN Alertness BIT NULL; 
                      ALTER TABLE Reg037_TIInspection ALTER COLUMN Findings TEXT NULL; 
                  END;"
            };

            foreach (string sql in upgradeStatements)
            {
                try
                {
                    ExecuteNonQuery(sql);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Schema upgrade statement error: " + ex.Message);
                }
            }
        }
    }
}

