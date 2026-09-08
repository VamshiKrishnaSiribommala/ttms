using System;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace TMS
{
    public class AuthService
    {
        private readonly DatabaseHelper db = new DatabaseHelper();

        public AuthService()
        {
            db.InitializeAuthSchema();
            SeedDefaultAdminIfEmpty();
        }

        #region Password Hashing & Cryptography (PBKDF2)

        public static void HashPassword(string password, out string hash, out string salt)
        {
            byte[] saltBytes = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }
            salt = Convert.ToBase64String(saltBytes);

            using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 10000))
            {
                byte[] hashBytes = pbkdf2.GetBytes(32);
                hash = Convert.ToBase64String(hashBytes);
            }
        }

        public static bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            try
            {
                byte[] saltBytes = Convert.FromBase64String(storedSalt);
                using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 10000))
                {
                    byte[] hashBytes = pbkdf2.GetBytes(32);
                    string computedHash = Convert.ToBase64String(hashBytes);
                    return computedHash == storedHash;
                }
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
                string checkQuery = "SELECT COUNT(*) FROM TMS_Admins";
                object result = db.ExecuteScalar(checkQuery);
                int count = result != null && int.TryParse(result.ToString(), out int c) ? c : 0;

                if (count == 0)
                {
                    HashPassword("Admin@123", out string hash, out string salt);
                    using (SqlCommand cmd = new SqlCommand(@"
                        INSERT INTO TMS_Admins (Username, PasswordHash, Salt, FullName, Email)
                        VALUES (@u, @p, @s, @fn, @em)"))
                    {
                        cmd.Parameters.AddWithValue("@u", "admin");
                        cmd.Parameters.AddWithValue("@p", hash);
                        cmd.Parameters.AddWithValue("@s", salt);
                        cmd.Parameters.AddWithValue("@fn", "System Administrator");
                        cmd.Parameters.AddWithValue("@em", "admin@railways.gov.in");
                        db.ExecuteNonQuery(cmd);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error seeding admin: " + ex.Message);
            }
        }

        #endregion

        #region Authentication

        public bool AuthenticateAdmin(string username, string password, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                errorMessage = "Please enter both Admin username and password.";
                return false;
            }

            try
            {
                using (SqlCommand cmd = new SqlCommand("SELECT AdminId, Username, PasswordHash, Salt, FullName, Email FROM TMS_Admins WHERE Username = @u"))
                {
                    cmd.Parameters.AddWithValue("@u", username.Trim());
                    DataTable dt = db.ExecuteQuery(cmd);

                    if (dt.Rows.Count == 0)
                    {
                        errorMessage = "Invalid Admin credentials.";
                        return false;
                    }

                    DataRow row = dt.Rows[0];
                    string storedHash = row["PasswordHash"].ToString();
                    string storedSalt = row["Salt"].ToString();

                    if (!VerifyPassword(password, storedHash, storedSalt))
                    {
                        errorMessage = "Invalid Admin credentials.";
                        return false;
                    }

                    int adminId = Convert.ToInt32(row["AdminId"]);
                    string adminUser = row["Username"].ToString();
                    string fullName = row["FullName"].ToString();
                    string email = row["Email"] != DBNull.Value ? row["Email"].ToString() : "";

                    SessionManager.SetAdminSession(adminId, adminUser, fullName, email);
                    return true;
                }
            }
            catch (Exception ex)
            {
                errorMessage = "Authentication error: " + ex.Message;
                return false;
            }
        }

        public bool AuthenticateUser(string username, string password, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                errorMessage = "Please enter both username and password.";
                return false;
            }

            try
            {
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT UserId, FullName, Email, Phone, Address, Department, Course, 
                           CollegeOrOrg, Username, PasswordHash, Salt, Status 
                    FROM TMS_Users 
                    WHERE Username = @u"))
                {
                    cmd.Parameters.AddWithValue("@u", username.Trim());
                    DataTable dt = db.ExecuteQuery(cmd);

                    if (dt.Rows.Count == 0)
                    {
                        errorMessage = "Invalid username or password.";
                        return false;
                    }

                    DataRow row = dt.Rows[0];
                    string status = row["Status"].ToString();
                    if (!string.Equals(status, "Active", StringComparison.OrdinalIgnoreCase))
                    {
                        errorMessage = "Your account is inactive. Please contact the Station Administrator.";
                        return false;
                    }

                    string storedHash = row["PasswordHash"].ToString();
                    string storedSalt = row["Salt"].ToString();

                    if (!VerifyPassword(password, storedHash, storedSalt))
                    {
                        errorMessage = "Invalid username or password.";
                        return false;
                    }

                    int userId = Convert.ToInt32(row["UserId"]);
                    string uName = row["Username"].ToString();
                    string fullName = row["FullName"].ToString();
                    string email = row["Email"].ToString();
                    string dept = row["Department"].ToString();
                    string course = row["Course"] != DBNull.Value ? row["Course"].ToString() : "";
                    string phone = row["Phone"] != DBNull.Value ? row["Phone"].ToString() : "";
                    string address = row["Address"] != DBNull.Value ? row["Address"].ToString() : "";
                    string org = row["CollegeOrOrg"] != DBNull.Value ? row["CollegeOrOrg"].ToString() : "";

                    SessionManager.SetUserSession(userId, uName, fullName, email, dept, course, phone, address, org, status);
                    return true;
                }
            }
            catch (Exception ex)
            {
                errorMessage = "Authentication error: " + ex.Message;
                return false;
            }
        }

        #endregion

        #region User Management (Admin functions)

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

            // Validations
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

            if (!Regex.IsMatch(phone, @"^[0-9+\-\s]{7,15}$"))
            {
                errorMessage = "Please enter a valid phone number (7-15 digits).";
                return false;
            }

            try
            {
                // Check unique username across Users & Admins
                using (SqlCommand chkUser = new SqlCommand("SELECT COUNT(*) FROM TMS_Users WHERE Username = @u"))
                {
                    chkUser.Parameters.AddWithValue("@u", username.Trim());
                    int userExists = Convert.ToInt32(db.ExecuteScalar(chkUser));
                    if (userExists > 0)
                    {
                        errorMessage = $"Username '{username}' is already taken. Please choose a different username.";
                        return false;
                    }
                }

                using (SqlCommand chkAdmin = new SqlCommand("SELECT COUNT(*) FROM TMS_Admins WHERE Username = @u"))
                {
                    chkAdmin.Parameters.AddWithValue("@u", username.Trim());
                    int adminExists = Convert.ToInt32(db.ExecuteScalar(chkAdmin));
                    if (adminExists > 0)
                    {
                        errorMessage = $"Username '{username}' is reserved for an Admin account.";
                        return false;
                    }
                }

                HashPassword(password, out string hash, out string salt);

                using (SqlCommand cmd = new SqlCommand(@"
                    INSERT INTO TMS_Users 
                    (FullName, DateOfBirth, Gender, Email, Phone, Address, Department, Course, YearOfJoining, CollegeOrOrg, Username, PasswordHash, Salt, Status, CreatedAt, UpdatedAt)
                    VALUES 
                    (@fn, @dob, @gen, @em, @ph, @addr, @dept, @course, @yr, @org, @u, @p, @s, @st, GETDATE(), GETDATE())"))
                {
                    cmd.Parameters.AddWithValue("@fn", fullName.Trim());
                    cmd.Parameters.AddWithValue("@dob", (object)dob ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@gen", string.IsNullOrWhiteSpace(gender) ? (object)DBNull.Value : gender.Trim());
                    cmd.Parameters.AddWithValue("@em", email.Trim());
                    cmd.Parameters.AddWithValue("@ph", phone.Trim());
                    cmd.Parameters.AddWithValue("@addr", string.IsNullOrWhiteSpace(address) ? (object)DBNull.Value : address.Trim());
                    cmd.Parameters.AddWithValue("@dept", department.Trim());
                    cmd.Parameters.AddWithValue("@course", string.IsNullOrWhiteSpace(course) ? (object)DBNull.Value : course.Trim());
                    cmd.Parameters.AddWithValue("@yr", string.IsNullOrWhiteSpace(yearOfJoining) ? (object)DBNull.Value : yearOfJoining.Trim());
                    cmd.Parameters.AddWithValue("@org", string.IsNullOrWhiteSpace(collegeOrOrg) ? (object)DBNull.Value : collegeOrOrg.Trim());
                    cmd.Parameters.AddWithValue("@u", username.Trim());
                    cmd.Parameters.AddWithValue("@p", hash);
                    cmd.Parameters.AddWithValue("@s", salt);
                    cmd.Parameters.AddWithValue("@st", string.IsNullOrWhiteSpace(status) ? "Active" : status);

                    int rows = db.ExecuteNonQuery(cmd);
                    return rows > 0;
                }
            }
            catch (Exception ex)
            {
                errorMessage = "Database error while creating user: " + ex.Message;
                return false;
            }
        }

        public bool UpdateUser(
            int userId,
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
            string newPassword,
            string status,
            out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(fullName)) { errorMessage = "Full Name is required."; return false; }
            if (string.IsNullOrWhiteSpace(email)) { errorMessage = "Email is required."; return false; }
            if (string.IsNullOrWhiteSpace(phone)) { errorMessage = "Phone number is required."; return false; }
            if (string.IsNullOrWhiteSpace(department)) { errorMessage = "Department is required."; return false; }

            try
            {
                bool updatePassword = !string.IsNullOrWhiteSpace(newPassword);
                string hash = null;
                string salt = null;

                if (updatePassword)
                {
                    if (newPassword.Length < 6)
                    {
                        errorMessage = "New password must be at least 6 characters long.";
                        return false;
                    }
                    HashPassword(newPassword, out hash, out salt);
                }

                string sql = @"
                    UPDATE TMS_Users 
                    SET FullName = @fn,
                        DateOfBirth = @dob,
                        Gender = @gen,
                        Email = @em,
                        Phone = @ph,
                        Address = @addr,
                        Department = @dept,
                        Course = @course,
                        YearOfJoining = @yr,
                        CollegeOrOrg = @org,
                        Status = @st,
                        UpdatedAt = GETDATE()" +
                        (updatePassword ? ", PasswordHash = @p, Salt = @s" : "") +
                        " WHERE UserId = @id";

                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    cmd.Parameters.AddWithValue("@id", userId);
                    cmd.Parameters.AddWithValue("@fn", fullName.Trim());
                    cmd.Parameters.AddWithValue("@dob", (object)dob ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@gen", string.IsNullOrWhiteSpace(gender) ? (object)DBNull.Value : gender.Trim());
                    cmd.Parameters.AddWithValue("@em", email.Trim());
                    cmd.Parameters.AddWithValue("@ph", phone.Trim());
                    cmd.Parameters.AddWithValue("@addr", string.IsNullOrWhiteSpace(address) ? (object)DBNull.Value : address.Trim());
                    cmd.Parameters.AddWithValue("@dept", department.Trim());
                    cmd.Parameters.AddWithValue("@course", string.IsNullOrWhiteSpace(course) ? (object)DBNull.Value : course.Trim());
                    cmd.Parameters.AddWithValue("@yr", string.IsNullOrWhiteSpace(yearOfJoining) ? (object)DBNull.Value : yearOfJoining.Trim());
                    cmd.Parameters.AddWithValue("@org", string.IsNullOrWhiteSpace(collegeOrOrg) ? (object)DBNull.Value : collegeOrOrg.Trim());
                    cmd.Parameters.AddWithValue("@st", string.IsNullOrWhiteSpace(status) ? "Active" : status);

                    if (updatePassword)
                    {
                        cmd.Parameters.AddWithValue("@p", hash);
                        cmd.Parameters.AddWithValue("@s", salt);
                    }

                    int rows = db.ExecuteNonQuery(cmd);
                    return rows > 0;
                }
            }
            catch (Exception ex)
            {
                errorMessage = "Database error while updating user: " + ex.Message;
                return false;
            }
        }

        public bool SetUserStatus(int userId, string status, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                using (SqlCommand cmd = new SqlCommand("UPDATE TMS_Users SET Status = @st, UpdatedAt = GETDATE() WHERE UserId = @id"))
                {
                    cmd.Parameters.AddWithValue("@st", status);
                    cmd.Parameters.AddWithValue("@id", userId);
                    int rows = db.ExecuteNonQuery(cmd);
                    return rows > 0;
                }
            }
            catch (Exception ex)
            {
                errorMessage = "Error updating status: " + ex.Message;
                return false;
            }
        }

        public DataTable GetAllUsers(string searchKeyword = "", string statusFilter = "")
        {
            try
            {
                string sql = @"
                    SELECT UserId, FullName, DateOfBirth, Gender, Email, Phone, 
                           Address, Department, Course, YearOfJoining, CollegeOrOrg, Username, Status, CreatedAt 
                    FROM TMS_Users WHERE 1=1 ";

                if (!string.IsNullOrWhiteSpace(statusFilter) && !string.Equals(statusFilter, "All", StringComparison.OrdinalIgnoreCase))
                {
                    sql += " AND Status = @status ";
                }

                if (!string.IsNullOrWhiteSpace(searchKeyword))
                {
                    sql += " AND (FullName LIKE @kw OR Username LIKE @kw OR Email LIKE @kw OR Phone LIKE @kw OR Department LIKE @kw OR Course LIKE @kw) ";
                }
                sql += " ORDER BY UserId DESC";

                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    if (!string.IsNullOrWhiteSpace(statusFilter) && !string.Equals(statusFilter, "All", StringComparison.OrdinalIgnoreCase))
                    {
                        cmd.Parameters.AddWithValue("@status", statusFilter.Trim());
                    }
                    if (!string.IsNullOrWhiteSpace(searchKeyword))
                    {
                        cmd.Parameters.AddWithValue("@kw", "%" + searchKeyword.Trim() + "%");
                    }
                    return db.ExecuteQuery(cmd);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error getting users: " + ex.Message);
                return new DataTable();
            }
        }

        public DataRow GetUserById(int userId)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM TMS_Users WHERE UserId = @id"))
                {
                    cmd.Parameters.AddWithValue("@id", userId);
                    DataTable dt = db.ExecuteQuery(cmd);
                    return dt.Rows.Count > 0 ? dt.Rows[0] : null;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error getting user by id: " + ex.Message);
                return null;
            }
        }

        #endregion

        #region User Password Recovery & Verification (User Self-Reset with Admin Audit)

        /// <summary>
        /// Allows a user to reset their forgotten password by verifying User ID (Username), Mobile Number, and Date of Birth.
        /// Logs the event to TMS_PasswordResetLogs so the Station Administrator is alerted.
        /// </summary>
        public bool ResetPasswordByVerification(
            string username, 
            string phone, 
            DateTime dob, 
            string newPassword, 
            string confirmPassword, 
            out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(username))
            {
                errorMessage = "Please enter your Username / User ID.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(phone))
            {
                errorMessage = "Please enter your Registered Mobile Number.";
                return false;
            }
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                errorMessage = "Please enter a New Password.";
                return false;
            }
            if (newPassword.Length < 6)
            {
                errorMessage = "New Password must be at least 6 characters long.";
                return false;
            }
            if (newPassword != confirmPassword)
            {
                errorMessage = "New Password and Confirm Password do not match.";
                return false;
            }

            try
            {
                // Query user by username
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT UserId, FullName, Phone, DateOfBirth, Status 
                    FROM TMS_Users 
                    WHERE Username = @u"))
                {
                    cmd.Parameters.AddWithValue("@u", username.Trim());
                    DataTable dt = db.ExecuteQuery(cmd);

                    if (dt.Rows.Count == 0)
                    {
                        errorMessage = "No registered user found with the provided Username / User ID.";
                        return false;
                    }

                    DataRow row = dt.Rows[0];
                    int userId = Convert.ToInt32(row["UserId"]);
                    string fullName = row["FullName"].ToString();
                    string status = row["Status"].ToString();
                    string storedPhone = row["Phone"] != DBNull.Value ? row["Phone"].ToString().Trim() : "";

                    if (!string.Equals(status, "Active", StringComparison.OrdinalIgnoreCase))
                    {
                        errorMessage = "Your account is inactive or locked. Please contact the Station Administrator.";
                        return false;
                    }

                    // Verify Date of Birth
                    if (row["DateOfBirth"] == DBNull.Value)
                    {
                        errorMessage = "No Date of Birth on file for this account. Please contact the Station Administrator.";
                        return false;
                    }

                    DateTime storedDob = Convert.ToDateTime(row["DateOfBirth"]);
                    if (storedDob.Date != dob.Date)
                    {
                        errorMessage = "Verification failed: Date of Birth does not match our records.";
                        return false;
                    }

                    // Verify Mobile Number (match digits ignoring spaces/hyphens)
                    string cleanInputPhone = Regex.Replace(phone, @"[^\d]", "");
                    string cleanStoredPhone = Regex.Replace(storedPhone, @"[^\d]", "");

                    bool phoneMatches = false;
                    if (cleanInputPhone.Length >= 10 && cleanStoredPhone.Length >= 10)
                    {
                        string subInput = cleanInputPhone.Substring(cleanInputPhone.Length - 10);
                        string subStored = cleanStoredPhone.Substring(cleanStoredPhone.Length - 10);
                        phoneMatches = subInput == subStored;
                    }
                    else
                    {
                        phoneMatches = cleanInputPhone == cleanStoredPhone;
                    }

                    if (!phoneMatches)
                    {
                        errorMessage = "Verification failed: Mobile number does not match the registered record.";
                        return false;
                    }

                    // Verification Succeeded -> Hash New Password & Update
                    HashPassword(newPassword, out string hash, out string salt);

                    using (SqlCommand updateCmd = new SqlCommand(@"
                        UPDATE TMS_Users 
                        SET PasswordHash = @p, Salt = @s, UpdatedAt = GETDATE() 
                        WHERE UserId = @id"))
                    {
                        updateCmd.Parameters.AddWithValue("@p", hash);
                        updateCmd.Parameters.AddWithValue("@s", salt);
                        updateCmd.Parameters.AddWithValue("@id", userId);
                        db.ExecuteNonQuery(updateCmd);
                    }

                    // Log audit event for the Station Administrator
                    try
                    {
                        string hostName = Environment.MachineName;
                        using (SqlCommand logCmd = new SqlCommand(@"
                            INSERT INTO TMS_PasswordResetLogs 
                            (UserId, Username, FullName, Phone, ResetTime, Status, IPAddressOrHost, AdminNotified)
                            VALUES 
                            (@uid, @u, @fn, @ph, GETDATE(), 'Password Reset by User Verification', @host, 0)"))
                        {
                            logCmd.Parameters.AddWithValue("@uid", userId);
                            logCmd.Parameters.AddWithValue("@u", username.Trim());
                            logCmd.Parameters.AddWithValue("@fn", fullName);
                            logCmd.Parameters.AddWithValue("@ph", storedPhone);
                            logCmd.Parameters.AddWithValue("@host", hostName);
                            db.ExecuteNonQuery(logCmd);
                        }
                    }
                    catch (Exception logEx)
                    {
                        System.Diagnostics.Debug.WriteLine("Error logging password reset: " + logEx.Message);
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                errorMessage = "Database error during password reset: " + ex.Message;
                return false;
            }
        }

        public DataTable GetPasswordResetLogs(string searchKeyword = "")
        {
            try
            {
                string sql = @"
                    SELECT LogId, UserId, Username, FullName, Phone, ResetTime, Status, IPAddressOrHost, AdminNotified 
                    FROM TMS_PasswordResetLogs ";

                if (!string.IsNullOrWhiteSpace(searchKeyword))
                {
                    sql += " WHERE FullName LIKE @kw OR Username LIKE @kw OR Phone LIKE @kw ";
                }
                sql += " ORDER BY ResetTime DESC";

                using (SqlCommand cmd = new SqlCommand(sql))
                {
                    if (!string.IsNullOrWhiteSpace(searchKeyword))
                    {
                        cmd.Parameters.AddWithValue("@kw", "%" + searchKeyword.Trim() + "%");
                    }
                    return db.ExecuteQuery(cmd);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error getting password reset logs: " + ex.Message);
                return new DataTable();
            }
        }

        public int GetUnreadPasswordResetCount()
        {
            try
            {
                string sql = "SELECT COUNT(*) FROM TMS_PasswordResetLogs WHERE AdminNotified = 0";
                object res = db.ExecuteScalar(sql);
                return res != null && int.TryParse(res.ToString(), out int count) ? count : 0;
            }
            catch
            {
                return 0;
            }
        }

        public void MarkPasswordResetsAsNotified()
        {
            try
            {
                string sql = "UPDATE TMS_PasswordResetLogs SET AdminNotified = 1 WHERE AdminNotified = 0";
                db.ExecuteNonQuery(sql);
            }
            catch { }
        }

        #endregion
    }
}
