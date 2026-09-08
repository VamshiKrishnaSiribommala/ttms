using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using TMS.Core.Database;
using TMS.Core.Models;
using TMS.Core.Services;

namespace TMS.Web.Controllers
{
    [Authorize]
    public class AuthoritiesController : Controller
    {
        private readonly DatabaseHelper _db;

        public AuthoritiesController([FromKeyedServices("TrainWorkingDb")] DatabaseHelper db)
        {
            _db = db;
        }

        public IActionResult Index(string? search = null, string? category = null)
        {
            var fullList = new List<RegisterDefinition>();
            for (int i = 1; i <= 22; i++)
            {
                var auth = RegistryMetadata.GetAuthorityByNumber(i.ToString());
                if (auth != null) fullList.Add(auth);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                fullList = fullList.Where(a => 
                    a.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    a.Code.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    a.Description.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(category) && category != "All")
            {
                fullList = fullList.Where(a => string.Equals(a.Category, category, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            ViewBag.Search = search;
            ViewBag.SelectedCategory = category ?? "All";
            ViewBag.Categories = new[] { "All", "Signaling Authority", "Track Safety", "Block Working", "Authority Form" };

            return View(fullList);
        }

        [HttpGet]
        public IActionResult Form(string id)
        {
            var auth = RegistryMetadata.GetAuthorityByNumber(id);
            if (auth == null) return NotFound();

            string datePart = DateTime.Now.ToString("yyyyMMdd");
            int count = 0;
            try
            {
                string checkQuery = $"IF EXISTS (SELECT * FROM sys.tables WHERE name = '{auth.TableName}') SELECT COUNT(*) FROM {auth.TableName} WHERE LogID LIKE '{auth.LogIdPrefix}-{datePart}-%' ELSE SELECT 0";
                object? res = _db.ExecuteScalar(checkQuery);
                if (res != null && int.TryParse(res.ToString(), out int c)) count = c;
            }
            catch { }

            string generatedLogId = $"{auth.LogIdPrefix}-{datePart}-{(count + 1).ToString("D3")}";
            ViewBag.LogId = generatedLogId;
            ViewBag.StationMaster = User.FindFirst("FullName")?.Value ?? "Station Master (SC)";

            return View(auth);
        }

        [HttpPost]
        public IActionResult Submit(string id, IFormCollection form)
        {
            var auth = RegistryMetadata.GetAuthorityByNumber(id);
            if (auth == null) return NotFound();

            string logId = form["LogID"].ToString();
            if (string.IsNullOrWhiteSpace(logId))
            {
                string datePart = DateTime.Now.ToString("yyyyMMdd");
                logId = $"{auth.LogIdPrefix}-{datePart}-001";
            }

            try
            {
                EnsureTableExists(auth);

                var columns = new List<string> { "LogID" };
                var paramNames = new List<string> { "@LogID" };
                var parameters = new List<SqlParameter> { new SqlParameter("@LogID", logId) };
                var fieldDict = new Dictionary<string, string>();

                foreach (var field in auth.Fields)
                {
                    string val = form[field.Name].ToString();
                    if (field.Required && string.IsNullOrWhiteSpace(val))
                    {
                        ViewBag.Error = $"Field '{field.Label}' is required.";
                        ViewBag.LogId = logId;
                        return View("Form", auth);
                    }

                    columns.Add(field.Name);
                    string pName = "@" + field.Name;
                    paramNames.Add(pName);
                    fieldDict[field.Label] = val;

                    if (field.Type == "number" && int.TryParse(val, out int numVal))
                    {
                        parameters.Add(new SqlParameter(pName, numVal));
                    }
                    else if ((field.Type == "datetime" || field.Type == "date") && DateTime.TryParse(val, out DateTime dtVal))
                    {
                        parameters.Add(new SqlParameter(pName, dtVal));
                    }
                    else
                    {
                        parameters.Add(new SqlParameter(pName, string.IsNullOrWhiteSpace(val) ? (object)DBNull.Value : val));
                    }
                }

                string insertSql = $"INSERT INTO {auth.TableName} ({string.Join(", ", columns)}) VALUES ({string.Join(", ", paramNames)})";
                using var cmd = new SqlCommand(insertSql);
                cmd.Parameters.AddRange(parameters.ToArray());

                _db.ExecuteNonQuery(cmd);

                TempData["SuccessMessage"] = $"Authority issued successfully! Log ID: {logId}";
                return RedirectToAction("ViewRecords", new { id = auth.Number });
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Database error issuing authority: {ex.Message}";
                ViewBag.LogId = logId;
                return View("Form", auth);
            }
        }

        [HttpGet]
        public IActionResult ViewRecords(string id)
        {
            var auth = RegistryMetadata.GetAuthorityByNumber(id);
            if (auth == null) return NotFound();

            DataTable records = new DataTable();
            try
            {
                string query = $"IF EXISTS (SELECT * FROM sys.tables WHERE name = '{auth.TableName}') SELECT TOP 100 * FROM {auth.TableName} ORDER BY 1 DESC ELSE SELECT 'No Records Found' AS Status";
                records = _db.ExecuteQuery(query);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            ViewBag.Authority = auth;
            return View(records);
        }

        private void EnsureTableExists(RegisterDefinition auth)
        {
            try
            {
                string check = $"SELECT COUNT(*) FROM sys.tables WHERE name = '{auth.TableName}'";
                int exists = Convert.ToInt32(_db.ExecuteScalar(check) ?? 0);
                if (exists == 0)
                {
                    var cols = new List<string> { "LogID VARCHAR(100) PRIMARY KEY" };
                    foreach (var f in auth.Fields)
                    {
                        if (f.Type == "number") cols.Add($"{f.Name} INT NULL");
                        else if (f.Type == "datetime") cols.Add($"{f.Name} DATETIME NULL");
                        else if (f.Type == "date") cols.Add($"{f.Name} DATE NULL");
                        else if (f.Type == "textarea") cols.Add($"{f.Name} NVARCHAR(MAX) NULL");
                        else cols.Add($"{f.Name} NVARCHAR(255) NULL");
                    }
                    cols.Add("IssuedAt DATETIME DEFAULT GETDATE()");

                    string createSql = $"CREATE TABLE {auth.TableName} ({string.Join(", ", cols)})";
                    _db.ExecuteNonQuery(createSql);
                }
            }
            catch { }
        }
    }
}
