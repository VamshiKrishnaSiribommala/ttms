using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace TrainWorkingApp
{
    public class DatabaseHelper
    {
        public static bool LastExecutionSuccessful { get; set; } = true;
        private string connectionString = @"Data Source=localhost\SQLEXPRESS;Initial Catalog=TrainManagementDB;Integrated Security=True;TrustServerCertificate=True";

        public DatabaseHelper()
        {
            try
            {
                if (ConfigurationManager.ConnectionStrings["TrainWorkingConnectionString"] != null)
                {
                    string configured = ConfigurationManager.ConnectionStrings["TrainWorkingConnectionString"].ConnectionString;
                    if (!string.IsNullOrWhiteSpace(configured))
                    {
                        connectionString = configured;
                    }
                }
            }
            catch { }
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

                EnsureDatabaseSchemaUpgraded();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Database schema initialization error: " + ex.Message);
            }
        }

        public void EnsureDatabaseSchemaUpgraded()
        {
            string[] upgradeStatements = new string[]
            {
                "IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Communication_Failure_Log') ALTER TABLE Communication_Failure_Log ALTER COLUMN Restoration_Time DATETIME NULL;",
                "IF EXISTS (SELECT * FROM sys.tables WHERE name = 'ST_Disconnection_Notice') ALTER TABLE ST_Disconnection_Notice ALTER COLUMN Reconnection_Time DATETIME NULL;",
                "IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Advance_Authority_Defective_Signal') BEGIN ALTER TABLE Advance_Authority_Defective_Signal ALTER COLUMN Issuing_Station VARCHAR(255) NULL; ALTER TABLE Advance_Authority_Defective_Signal ALTER COLUMN Issuing_Officer VARCHAR(255) NULL; END",
                "IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Authority_Pass_Signal_ON') ALTER TABLE Authority_Pass_Signal_ON ALTER COLUMN Issuing_Officer VARCHAR(255) NULL;",
                "IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Caution_Order_Entry') ALTER TABLE Caution_Order_Entry ALTER COLUMN Issued_By VARCHAR(255) NULL;",
                "IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Authority_Receive_Obstructed_Line') ALTER TABLE Authority_Receive_Obstructed_Line ALTER COLUMN Issuing_Officer VARCHAR(255) NULL;",
                "IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Authority_Receive_Non_Signalled') BEGIN ALTER TABLE Authority_Receive_Non_Signalled ALTER COLUMN Receiving_Station VARCHAR(255) NULL; ALTER TABLE Authority_Receive_Non_Signalled ALTER COLUMN Issuing_Officer VARCHAR(255) NULL; END",
                "IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Authority_Start_Non_Signalled') BEGIN ALTER TABLE Authority_Start_Non_Signalled ALTER COLUMN Starting_Station VARCHAR(255) NULL; ALTER TABLE Authority_Start_Non_Signalled ALTER COLUMN Issuing_Officer VARCHAR(255) NULL; END",
                "IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Authority_Common_Starter') BEGIN ALTER TABLE Authority_Common_Starter ALTER COLUMN Starting_Station VARCHAR(255) NULL; ALTER TABLE Authority_Common_Starter ALTER COLUMN Issuing_Officer VARCHAR(255) NULL; END",
                "IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Relief_Train_Authorization') BEGIN ALTER TABLE Relief_Train_Authorization ALTER COLUMN Issuing_Station VARCHAR(255) NULL; ALTER TABLE Relief_Train_Authorization ALTER COLUMN Issuing_Officer VARCHAR(255) NULL; END",
                "IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Shunting_Order_Management') BEGIN ALTER TABLE Shunting_Order_Management ALTER COLUMN Station VARCHAR(255) NULL; ALTER TABLE Shunting_Order_Management ALTER COLUMN Issued_By VARCHAR(255) NULL; END",
                "IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Signal_Passing_Authority') ALTER TABLE Signal_Passing_Authority ALTER COLUMN Issuing_Officer VARCHAR(255) NULL;",
                "IF EXISTS (SELECT * FROM sys.tables WHERE name = 'ABS_Proceed_Without_Line_Clear') ALTER TABLE ABS_Proceed_Without_Line_Clear ALTER COLUMN Issuing_Authority VARCHAR(255) NULL;",
                "IF EXISTS (SELECT * FROM sys.tables WHERE name = 'ABS_Relief_Engine_Authority') BEGIN ALTER TABLE ABS_Relief_Engine_Authority ALTER COLUMN Issued_By VARCHAR(255) NULL; IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('ABS_Relief_Engine_Authority') AND name = 'Remarks') ALTER TABLE ABS_Relief_Engine_Authority ADD Remarks VARCHAR(MAX) NULL; END",
                "IF EXISTS (SELECT * FROM sys.tables WHERE name = 'ABS_Prolonged_Signal_Failure') BEGIN ALTER TABLE ABS_Prolonged_Signal_Failure ALTER COLUMN Issued_By VARCHAR(255) NULL; END",
                "IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Line_Clear_Tickets') BEGIN ALTER TABLE Line_Clear_Tickets ALTER COLUMN Issued_By VARCHAR(255) NULL; ALTER TABLE Line_Clear_Tickets ALTER COLUMN Digital_Signature VARBINARY(MAX) NULL; END",
                "IF EXISTS (SELECT * FROM sys.tables WHERE name = 'ST_Disconnection_Notice') BEGIN ALTER TABLE ST_Disconnection_Notice ALTER COLUMN Signatures VARBINARY(MAX) NULL; END"
            };

            foreach (string sql in upgradeStatements)
            {
                try
                {
                    ExecuteNonQuery(sql);
                }
                catch { }
            }
        }
    }
}
