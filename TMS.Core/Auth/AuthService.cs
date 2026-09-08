using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using TMS.Core.Database;

namespace TMS.Core.Auth
{
    public class UserSession
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string CollegeOrOrg { get; set; } = string.Empty;
        public string Status { get; set; } = "Active";
        public bool IsAdmin { get; set; }
    }

    public class AuthService
    {
        private readonly DatabaseHelper _db;

        public AuthService(DatabaseHelper db)
        {
            _db = db;
            _db.InitializeAuthSchema();
            SeedDefaultAdminIfEmpty();
        }

        #region Password Hashing & Cryptography (PBKDF2)

        public static void HashPassword(string password, out string hash, out string salt)
        {
            byte[] saltBytes = new byte[16];
            RandomNumberGenerator.Fill(saltBytes);
            salt = Convert.ToBase64String(saltBytes);

            byte[] hashBytes = Rfc2898DeriveBytes.Pbkdf2(password, saltBytes, 10000, HashAlgorithmName.SHA256, 32);
            hash = Convert.ToBase64String(hashBytes);
        }

        public static bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            try
            {
                byte[] saltBytes = Convert.FromBase64String(storedSalt);
                
                // Try SHA256 (modern)
                byte[] hashBytes = Rfc2898DeriveBytes.Pbkdf2(password, saltBytes, 10000, HashAlgorithmName.SHA256, 32);
                string computedHash = Convert.ToBase64String(hashBytes);
                if (computedHash == storedHash) return true;

                // Fallback to SHA1 (legacy .NET Framework PBKDF2 default)
                byte[] hashBytesLegacy = Rfc2898DeriveBytes.Pbkdf2(password, saltBytes, 10000, HashAlgorithmName.SHA1, 32);
                string computedHashLegacy = Convert.ToBase64String(hashBytesLegacy);
                if (computedHashLegacy == storedHash) return true;

                return false;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Seeding & Setup

        public void SeedDefaultAdminIfEmpty()
        {
            try
            {
                string checkAdmin = @"
                    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name IN ('TMS_Admin', 'TMS_Admins'))
                    BEGIN
                        CREATE TABLE TMS_Admin (
                            AdminId INT IDENTITY(1,1) PRIMARY KEY,
                            Username NVARCHAR(50) NOT NULL UNIQUE,
                            PasswordHash NVARCHAR(256) NOT NULL,
                            Salt NVARCHAR(100) NOT NULL,
                            FullName NVARCHAR(100) NOT NULL,
                            Email NVARCHAR(100) NOT NULL,
                            CreatedAt DATETIME DEFAULT GETDATE()
                        );
                    END
                ";
                _db.ExecuteNonQuery(checkAdmin);

                string tblName = "TMS_Admin";
                object? countObj = _db.ExecuteScalar("SELECT COUNT(*) FROM TMS_Admin");
                if (countObj == null && _db.ExecuteScalar("IF EXISTS (SELECT * FROM sys.tables WHERE name='TMS_Admins') SELECT 1 ELSE SELECT 0")?.ToString() == "1")
                {
                    tblName = "TMS_Admins";
                    countObj = _db.ExecuteScalar("SELECT COUNT(*) FROM TMS_Admins");
                }

                int count = countObj != null && int.TryParse(countObj.ToString(), out int c) ? c : 0;
                if (count == 0)
                {
                    HashPassword("Admin@123", out string hash, out string salt);
                    string insertSql = $@"
                        INSERT INTO {tblName} (Username, PasswordHash, Salt, FullName, Email)
                        VALUES (@u, @p, @s, @fn, @em)";
                    using var cmd = new SqlCommand(insertSql);
                    cmd.Parameters.AddWithValue("@u", "admin");
                    cmd.Parameters.AddWithValue("@p", hash);
                    cmd.Parameters.AddWithValue("@s", salt);
                    cmd.Parameters.AddWithValue("@fn", "System Administrator");
                    cmd.Parameters.AddWithValue("@em", "admin@railways.gov.in");
                    _db.ExecuteNonQuery(cmd);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error seeding default admin: " + ex.Message);
            }
        }

        #endregion

        #region Authentication

        public bool AuthenticateAdmin(string username, string password, out UserSession? session, out string errorMessage)
        {
            session = null;
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                errorMessage = "Please enter both Admin username and password.";
                return false;
            }

            try
            {
                string tblName = _db.ExecuteScalar("IF EXISTS (SELECT * FROM sys.tables WHERE name='TMS_Admin') SELECT 1 ELSE SELECT 0")?.ToString() == "1"
                    ? "TMS_Admin" : "TMS_Admins";

                using var cmd = new SqlCommand($"SELECT AdminId, Username, PasswordHash, Salt, FullName, Email FROM {tblName} WHERE Username = @u");
                cmd.Parameters.AddWithValue("@u", username.Trim());
                DataTable dt = _db.ExecuteQuery(cmd);

                if (dt.Rows.Count == 0)
                {
                    errorMessage = "Invalid Admin credentials.";
                    return false;
                }

                DataRow row = dt.Rows[0];
                string storedHash = row["PasswordHash"].ToString() ?? "";
                string storedSalt = row["Salt"].ToString() ?? "";

                if (!VerifyPassword(password, storedHash, storedSalt))
                {
                    errorMessage = "Invalid Admin credentials.";
                    return false;
                }

                session = new UserSession
                {
                    UserId = Convert.ToInt32(row["AdminId"]),
                    Username = row["Username"].ToString() ?? "",
                    FullName = row["FullName"].ToString() ?? "",
                    Email = row["Email"] != DBNull.Value ? row["Email"].ToString() ?? "" : "",
                    Department = "Administration",
                    IsAdmin = true,
                    Status = "Active"
                };

                return true;
            }
            catch (Exception ex)
            {
                errorMessage = "Authentication error: " + ex.Message;
                return false;
            }
        }

        public bool AuthenticateUser(string username, string password, out UserSession? session, out string errorMessage)
        {
            session = null;
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                errorMessage = "Please enter both username and password.";
                return false;
            }

            try
            {
                using var cmd = new SqlCommand(@"
                    SELECT UserId, FullName, Email, Phone, Address, Department, Course, 
                           CollegeOrOrg, Username, PasswordHash, Salt, Status 
                    FROM TMS_Users 
                    WHERE Username = @u");
                cmd.Parameters.AddWithValue("@u", username.Trim());
                DataTable dt = _db.ExecuteQuery(cmd);

                if (dt.Rows.Count == 0)
                {
                    errorMessage = "Invalid username or password.";
                    return false;
                }

                DataRow row = dt.Rows[0];
                string status = row["Status"]?.ToString() ?? "Active";
                if (!string.Equals(status, "Active", StringComparison.OrdinalIgnoreCase))
                {
                    errorMessage = "Your account is inactive. Please contact the Station Administrator.";
                    return false;
                }

                string storedHash = row["PasswordHash"].ToString() ?? "";
                string storedSalt = row["Salt"].ToString() ?? "";

                if (!VerifyPassword(password, storedHash, storedSalt))
                {
                    errorMessage = "Invalid username or password.";
                    return false;
                }

                session = new UserSession
                {
                    UserId = Convert.ToInt32(row["UserId"]),
                    Username = row["Username"].ToString() ?? "",
                    FullName = row["FullName"].ToString() ?? "",
                    Email = row["Email"]?.ToString() ?? "",
                    Department = row["Department"]?.ToString() ?? "",
                    Course = row["Course"] != DBNull.Value ? row["Course"].ToString() ?? "" : "",
                    Phone = row["Phone"] != DBNull.Value ? row["Phone"].ToString() ?? "" : "",
                    Address = row["Address"] != DBNull.Value ? row["Address"].ToString() ?? "" : "",
                    CollegeOrOrg = row["CollegeOrOrg"] != DBNull.Value ? row["CollegeOrOrg"].ToString() ?? "" : "",
                    Status = status,
                    IsAdmin = false
                };

                return true;
            }
            catch (Exception ex)
            {
                errorMessage = "Authentication error: " + ex.Message;
                return false;
            }
        }

        #endregion

        #region User Management (Admin & Self-Service)

        public bool CreateUser(
            string fullName,
            DateTime? dob,
            string gender,
            string email,
            string phone,
            string address,
            string department,
            string course,
            string yearOfJoining,
            string collegeOrOrg,
            string username,
            string password,
            string confirmPassword,
            string status,
            out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(fullName)) { errorMessage = "Full Name is required."; return false; }
            if (string.IsNullOrWhiteSpace(email)) { errorMessage = "Email is required."; return false; }
            if (string.IsNullOrWhiteSpace(phone)) { errorMessage = "Phone number is required."; return false; }
            if (string.IsNullOrWhiteSpace(department)) { errorMessage = "Department is required."; return false; }
            if (string.IsNullOrWhiteSpace(username)) { errorMessage = "Username is required."; return false; }
            if (string.IsNullOrWhiteSpace(password)) { errorMessage = "Password is required."; return false; }

            if (password != confirmPassword)
            {
                errorMessage = "Password and Confirm Password do not match.";
                return false;
            }

            if (password.Length < 6)
            {
                errorMessage = "Password must be at least 6 characters long.";
                return false;
            }

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                errorMessage = "Please enter a valid email address.";
                return false;
            }

            try
            {
                using var checkCmd = new SqlCommand("SELECT COUNT(*) FROM TMS_Users WHERE Username = @u OR Email = @e");
                checkCmd.Parameters.AddWithValue("@u", username.Trim());
                checkCmd.Parameters.AddWithValue("@e", email.Trim());
                int exists = Convert.ToInt32(_db.ExecuteScalar(checkCmd));
                if (exists > 0)
                {
                    errorMessage = "Username or Email already registered.";
                    return false;
                }

                HashPassword(password, out string hash, out string salt);

                using var insertCmd = new SqlCommand(@"
                    INSERT INTO TMS_Users 
                        (FullName, DateOfBirth, Gender, Email, Phone, Address, Department, Course, YearOfJoining, CollegeOrOrg, Username, PasswordHash, Salt, Status, CreatedAt, UpdatedAt)
                    VALUES 
                        (@fn, @dob, @gen, @em, @ph, @addr, @dept, @course, @yoj, @org, @u, @hash, @salt, @stat, GETDATE(), GETDATE())");

                insertCmd.Parameters.AddWithValue("@fn", fullName.Trim());
                insertCmd.Parameters.AddWithValue("@dob", (object?)dob ?? DBNull.Value);
                insertCmd.Parameters.AddWithValue("@gen", string.IsNullOrWhiteSpace(gender) ? DBNull.Value : gender);
                insertCmd.Parameters.AddWithValue("@em", email.Trim());
                insertCmd.Parameters.AddWithValue("@ph", phone.Trim());
                insertCmd.Parameters.AddWithValue("@addr", string.IsNullOrWhiteSpace(address) ? DBNull.Value : address.Trim());
                insertCmd.Parameters.AddWithValue("@dept", department.Trim());
                insertCmd.Parameters.AddWithValue("@course", string.IsNullOrWhiteSpace(course) ? DBNull.Value : course.Trim());
                insertCmd.Parameters.AddWithValue("@yoj", string.IsNullOrWhiteSpace(yearOfJoining) ? DBNull.Value : yearOfJoining.Trim());
                insertCmd.Parameters.AddWithValue("@org", string.IsNullOrWhiteSpace(collegeOrOrg) ? DBNull.Value : collegeOrOrg.Trim());
                insertCmd.Parameters.AddWithValue("@u", username.Trim());
                insertCmd.Parameters.AddWithValue("@hash", hash);
                insertCmd.Parameters.AddWithValue("@salt", salt);
                insertCmd.Parameters.AddWithValue("@stat", string.IsNullOrWhiteSpace(status) ? "Active" : status);

                _db.ExecuteNonQuery(insertCmd);
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = "Database error: " + ex.Message;
                return false;
            }
        }

        public DataTable GetAllUsers()
        {
            string query = @"
                SELECT UserId, FullName, Username, Email, Phone, Department, Course, CollegeOrOrg, Status, CreatedAt, UpdatedAt
                FROM TMS_Users
                ORDER BY UserId DESC";
            return _db.ExecuteQuery(query);
        }

        public bool UpdateUserStatus(int userId, string newStatus, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                using var cmd = new SqlCommand("UPDATE TMS_Users SET Status = @s, UpdatedAt = GETDATE() WHERE UserId = @id");
                cmd.Parameters.AddWithValue("@s", newStatus);
                cmd.Parameters.AddWithValue("@id", userId);
                _db.ExecuteNonQuery(cmd);
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public bool ResetPassword(string username, string newPassword, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(newPassword))
            {
                errorMessage = "Username and new password are required.";
                return false;
            }

            try
            {
                HashPassword(newPassword, out string hash, out string salt);
                using var cmd = new SqlCommand("UPDATE TMS_Users SET PasswordHash = @h, Salt = @s, UpdatedAt = GETDATE() WHERE Username = @u");
                cmd.Parameters.AddWithValue("@h", hash);
                cmd.Parameters.AddWithValue("@s", salt);
                cmd.Parameters.AddWithValue("@u", username.Trim());
                int rows = _db.ExecuteNonQuery(cmd);
                if (rows == 0)
                {
                    errorMessage = "User not found.";
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        #endregion
    }
}
