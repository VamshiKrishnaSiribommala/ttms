using System;

namespace TrainWorkingApp
{
    public enum UserRole
    {
        Admin,
        User
    }

    public static class SessionManager
    {
        public static bool IsLoggedIn { get; private set; } = false;
        public static int CurrentUserId { get; private set; } = 0;
        public static string CurrentUsername { get; private set; } = string.Empty;
        public static string CurrentFullName { get; private set; } = string.Empty;
        public static string CurrentEmail { get; private set; } = string.Empty;
        public static string CurrentDepartment { get; private set; } = string.Empty;
        public static string CurrentCourse { get; private set; } = string.Empty;
        public static string CurrentPhone { get; private set; } = string.Empty;
        public static string CurrentAddress { get; private set; } = string.Empty;
        public static string CurrentCollegeOrOrg { get; private set; } = string.Empty;
        public static string CurrentStatus { get; private set; } = string.Empty;
        public static UserRole CurrentRole { get; private set; } = UserRole.User;
        public static DateTime LoginTime { get; private set; }

        public static void SetAdminSession(int adminId, string username, string fullName, string email)
        {
            IsLoggedIn = true;
            CurrentUserId = adminId;
            CurrentUsername = username;
            CurrentFullName = fullName;
            CurrentEmail = email;
            CurrentDepartment = "Administration";
            CurrentRole = UserRole.Admin;
            CurrentStatus = "Active";
            LoginTime = DateTime.Now;
        }

        public static void SetUserSession(int userId, string username, string fullName, string email, 
            string department, string course, string phone, string address, string collegeOrOrg, string status)
        {
            IsLoggedIn = true;
            CurrentUserId = userId;
            CurrentUsername = username;
            CurrentFullName = fullName;
            CurrentEmail = email;
            CurrentDepartment = department;
            CurrentCourse = course;
            CurrentPhone = phone;
            CurrentAddress = address;
            CurrentCollegeOrOrg = collegeOrOrg;
            CurrentStatus = status;
            CurrentRole = UserRole.User;
            LoginTime = DateTime.Now;
        }

        public static void Logout()
        {
            IsLoggedIn = false;
            CurrentUserId = 0;
            CurrentUsername = string.Empty;
            CurrentFullName = string.Empty;
            CurrentEmail = string.Empty;
            CurrentDepartment = string.Empty;
            CurrentCourse = string.Empty;
            CurrentPhone = string.Empty;
            CurrentAddress = string.Empty;
            CurrentCollegeOrOrg = string.Empty;
            CurrentStatus = string.Empty;
            CurrentRole = UserRole.User;
        }

        public static string GetDisplayBadge()
        {
            if (!IsLoggedIn) return "Guest";
            string roleStr = CurrentRole == UserRole.Admin ? "Admin" : CurrentDepartment;
            return $"{CurrentFullName} ({roleStr})";
        }
    }
}
