using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using TMS.Core.Database;
using TMS.Core.Models;
using TMS.Core.Services;

namespace TMS.Web.Controllers
{
    [Authorize]
    public class ReportsController : Controller
    {
        private readonly DatabaseHelper _tmsDb;
        private readonly DatabaseHelper _twDb;

        public ReportsController(
            [FromKeyedServices("TMSDb")] DatabaseHelper tmsDb,
            [FromKeyedServices("TrainWorkingDb")] DatabaseHelper twDb)
        {
            _tmsDb = tmsDb;
            _twDb = twDb;
        }

        public IActionResult Index(string? reportType = "Registers", string? target = "001", DateTime? fromDate = null, DateTime? toDate = null)
        {
            ViewBag.ReportType = reportType;
            ViewBag.Target = target ?? "001";
            ViewBag.FromDate = fromDate?.ToString("yyyy-MM-dd") ?? DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd");
            ViewBag.ToDate = toDate?.ToString("yyyy-MM-dd") ?? DateTime.Today.ToString("yyyy-MM-dd");

            DataTable results = new DataTable();
            string selectedTitle = "";

            try
            {
                if (reportType == "Registers")
                {
                    var reg = RegistryMetadata.GetRegisterByNumber(target ?? "001");
                    selectedTitle = reg?.Title ?? "Register";
                    string q = $"IF EXISTS (SELECT * FROM sys.tables WHERE name = '{reg?.TableName}') SELECT TOP 200 * FROM {reg?.TableName} ORDER BY 1 DESC ELSE SELECT 'No Data' AS Status";
                    results = _tmsDb.ExecuteQuery(q);
                }
                else
                {
                    var auth = RegistryMetadata.GetAuthorityByNumber(target ?? "001");
                    selectedTitle = auth?.Title ?? "Authority";
                    string q = $"IF EXISTS (SELECT * FROM sys.tables WHERE name = '{auth?.TableName}') SELECT TOP 200 * FROM {auth?.TableName} ORDER BY 1 DESC ELSE SELECT 'No Data' AS Status";
                    results = _twDb.ExecuteQuery(q);
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }

            ViewBag.SelectedTitle = selectedTitle;
            ViewBag.Contacts = RecordDispatchService.LoadContacts();

            return View(results);
        }

        [HttpGet]
        public IActionResult DispatchMemo(string type, string id, string logId)
        {
            RegisterDefinition? def = (type == "Registers")
                ? RegistryMetadata.GetRegisterByNumber(id)
                : RegistryMetadata.GetAuthorityByNumber(id);

            if (def == null) return NotFound();

            var db = (type == "Registers") ? _tmsDb : _twDb;
            var fields = new Dictionary<string, string>();

            try
            {
                string q = $"SELECT * FROM {def.TableName} WHERE LogID = '{logId.Replace("'", "''")}'";
                var dt = db.ExecuteQuery(q);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataColumn col in dt.Columns)
                    {
                        fields[col.ColumnName] = dt.Rows[0][col]?.ToString() ?? "";
                    }
                }
            }
            catch { }

            string memoText = RecordDispatchService.FormatDispatchMessage(def.Code, def.Title, logId, fields);
            var contacts = RecordDispatchService.LoadContacts();

            ViewBag.Definition = def;
            ViewBag.LogId = logId;
            ViewBag.Fields = fields;
            ViewBag.MemoText = memoText;
            ViewBag.Contacts = contacts;

            return View();
        }
    }
}
