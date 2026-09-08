using System;
using System.Data;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using TMS.Core.Auth;
using TMS.Core.Database;

namespace TMS.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly DatabaseHelper _tmsDb;
        private readonly DatabaseHelper _twDb;
        private readonly AuthService _authService;

        public DashboardController(
            [FromKeyedServices("TMSDb")] DatabaseHelper tmsDb,
            [FromKeyedServices("TrainWorkingDb")] DatabaseHelper twDb,
            AuthService authService)
        {
            _tmsDb = tmsDb;
            _twDb = twDb;
            _authService = authService;
        }

        public IActionResult Index()
        {
            ViewBag.Username = User.Identity?.Name ?? "User";
            ViewBag.FullName = User.FindFirst("FullName")?.Value ?? ViewBag.Username;
            ViewBag.Role = User.IsInRole("Admin") ? "Admin" : "User";
            ViewBag.Department = User.FindFirst("Department")?.Value ?? "Operations";
            ViewBag.OS = RuntimeInformation.OSDescription;

            // Health and stats
            int tmsTables = 0;
            int twTables = 0;
            bool dbConnected = false;

            try
            {
                var dt1 = _tmsDb.ExecuteQuery("SELECT COUNT(*) FROM sys.tables");
                if (dt1.Rows.Count > 0) tmsTables = Convert.ToInt32(dt1.Rows[0][0]);

                var dt2 = _twDb.ExecuteQuery("SELECT COUNT(*) FROM sys.tables");
                if (dt2.Rows.Count > 0) twTables = Convert.ToInt32(dt2.Rows[0][0]);

                dbConnected = true;
            }
            catch (Exception ex)
            {
                ViewBag.DbError = ex.Message;
            }

            ViewBag.DbConnected = dbConnected;
            ViewBag.TmsTables = tmsTables;
            ViewBag.TwTables = twTables;
            ViewBag.RegistersCount = 41;
            ViewBag.AuthoritiesCount = 22;

            if (User.IsInRole("Admin"))
            {
                try
                {
                    DataTable users = _authService.GetAllUsers();
                    ViewBag.TotalUsers = users.Rows.Count;
                    ViewBag.UserList = users;
                }
                catch { }
            }

            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult ToggleUserStatus(int userId, string status)
        {
            string newStatus = string.Equals(status, "Active", StringComparison.OrdinalIgnoreCase) ? "Inactive" : "Active";
            _authService.UpdateUserStatus(userId, newStatus, out _);
            return RedirectToAction("Index");
        }
    }
}
