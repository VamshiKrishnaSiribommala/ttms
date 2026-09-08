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

namespace TMS.Web.Controllers
{
    [Authorize]
    public class RegistersController : Controller
    {
        private readonly DatabaseHelper _db;

        public RegistersController([FromKeyedServices("TMSDb")] DatabaseHelper db)
        {
            _db = db;
        }

        public IActionResult Index(string? search = null, string? category = null)
        {
            var registers = RegistryMetadata.AllRegisters;

            // Ensure all 41 registers are available
            var fullList = new List<RegisterDefinition>();
            for (int i = 1; i <= 41; i++)
            {
                var reg = RegistryMetadata.GetRegisterByNumber(i.ToString());
                if (reg != null) fullList.Add(reg);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                fullList = fullList.Where(r => 
                    r.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    r.Code.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    r.Description.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(category) && category != "All")
            {
                fullList = fullList.Where(r => string.Equals(r.Category, category, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            ViewBag.Search = search;
            ViewBag.SelectedCategory = category ?? "All";
            ViewBag.Categories = new[] { "All", "Operational", "Safety", "Maintenance", "Inspection", "General Register" };

            return View(fullList);
        }

        [HttpGet]
        public IActionResult Form(string id)
        {
            var reg = RegistryMetadata.GetRegisterByNumber(id);
            if (reg == null) return NotFound();

            string datePart = DateTime.Now.ToString("yyyyMMdd");
            int count = 0;
            try
            {
                string checkQuery = $"IF EXISTS (SELECT * FROM sys.tables WHERE name = '{reg.TableName}') SELECT COUNT(*) FROM {reg.TableName} WHERE LogID LIKE '{reg.LogIdPrefix}-{datePart}-%' ELSE SELECT 0";
                object? res = _db.ExecuteScalar(checkQuery);
                if (res != null && int.TryParse(res.ToString(), out int c)) count = c;
            }
            catch { }

            string generatedLogId = $"{reg.LogIdPrefix}-{datePart}-{(count + 1).ToString("D3")}";
            ViewBag.LogId = generatedLogId;
            ViewBag.StaffId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "101";

            return View(reg);
        }

        [HttpPost]
        public IActionResult Submit(string id, IFormCollection form)
        {
            var reg = RegistryMetadata.GetRegisterByNumber(id);
            if (reg == null) return NotFound();

            string logId = form["LogID"].ToString();
            if (string.IsNullOrWhiteSpace(logId))
            {
                string datePart = DateTime.Now.ToString("yyyyMMdd");
                logId = $"{reg.LogIdPrefix}-{datePart}-001";
            }

            try
            {
                // Verify table exists or create basic schema
                EnsureTableExists(reg);

                var columns = new List<string> { "LogID" };
                var paramNames = new List<string> { "@LogID" };
                var parameters = new List<SqlParameter> { new SqlParameter("@LogID", logId) };

                foreach (var field in reg.Fields)
                {
                    string val = form[field.Name].ToString();
                    if (field.Required && string.IsNullOrWhiteSpace(val))
                    {
                        ViewBag.Error = $"Field '{field.Label}' is required.";
                        ViewBag.LogId = logId;
                        return View("Form", reg);
                    }

                    columns.Add(field.Name);
                    string pName = "@" + field.Name;
                    paramNames.Add(pName);

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

                // Add CreatedAt if exists
                string insertSql = $"INSERT INTO {reg.TableName} ({string.Join(", ", columns)}) VALUES ({string.Join(", ", paramNames)})";
                using var cmd = new SqlCommand(insertSql);
                cmd.Parameters.AddRange(parameters.ToArray());

                _db.ExecuteNonQuery(cmd);

                TempData["SuccessMessage"] = $"Record successfully saved with Log ID: {logId}";
                return RedirectToAction("ViewRecords", new { id = reg.Number });
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Database error saving record: {ex.Message}";
                ViewBag.LogId = logId;
                return View("Form", reg);
            }
        }

        [HttpGet]
        public IActionResult ViewRecords(string id, string? search = null)
        {
            var reg = RegistryMetadata.GetRegisterByNumber(id);
            if (reg == null) return NotFound();

            DataTable records = new DataTable();
            try
            {
                string query = $"IF EXISTS (SELECT * FROM sys.tables WHERE name = '{reg.TableName}') SELECT TOP 100 * FROM {reg.TableName} ORDER BY 1 DESC ELSE SELECT 'No Table' AS Status";
                records = _db.ExecuteQuery(query);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            ViewBag.Register = reg;
            ViewBag.Search = search;
            return View(records);
        }

        [HttpGet]
        public IActionResult ExportCsv(string id)
        {
            var reg = RegistryMetadata.GetRegisterByNumber(id);
            if (reg == null) return NotFound();

            DataTable dt = _db.ExecuteQuery($"SELECT * FROM {reg.TableName}");
            var sb = new StringBuilder();

            var colNames = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName);
            sb.AppendLine(string.Join(",", colNames.Select(c => $"\"{c}\"")));

            foreach (DataRow row in dt.Rows)
            {
                var fields = row.ItemArray.Select(field => $"\"{field?.ToString()?.Replace("\"", "\"\"")}\"");
                sb.AppendLine(string.Join(",", fields));
            }

            byte[] bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/csv", $"{reg.TableName}_{DateTime.Now:yyyyMMdd_HHmmss}.csv");
        }

        private void EnsureTableExists(RegisterDefinition reg)
        {
            try
            {
                string check = $"SELECT COUNT(*) FROM sys.tables WHERE name = '{reg.TableName}'";
                int exists = Convert.ToInt32(_db.ExecuteScalar(check) ?? 0);
                if (exists == 0)
                {
                    var cols = new List<string> { "LogID VARCHAR(100) PRIMARY KEY" };
                    foreach (var f in reg.Fields)
                    {
                        if (f.Type == "number") cols.Add($"{f.Name} INT NULL");
                        else if (f.Type == "datetime") cols.Add($"{f.Name} DATETIME NULL");
                        else if (f.Type == "date") cols.Add($"{f.Name} DATE NULL");
                        else if (f.Type == "textarea") cols.Add($"{f.Name} NVARCHAR(MAX) NULL");
                        else cols.Add($"{f.Name} NVARCHAR(255) NULL");
                    }
                    cols.Add("CreatedAt DATETIME DEFAULT GETDATE()");

                    string createSql = $"CREATE TABLE {reg.TableName} ({string.Join(", ", cols)})";
                    _db.ExecuteNonQuery(createSql);
                }
            }
            catch { }
        }
    }
}
