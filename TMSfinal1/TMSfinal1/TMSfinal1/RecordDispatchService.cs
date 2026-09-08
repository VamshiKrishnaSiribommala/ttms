using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace TMS
{
    /// <summary>
    /// Core Dispatch Service for Generating Official PDF Memos and Dispatching
    /// Railway Register Records via WhatsApp, Telegram, or Both in 1-Click.
    /// </summary>
    public static class RecordDispatchService
    {
        public static string GetAppDirectory()
        {
            try
            {
                string loc = typeof(RecordDispatchService).Assembly.Location;
                if (!string.IsNullOrEmpty(loc))
                {
                    string dir = Path.GetDirectoryName(loc);
                    if (!string.IsNullOrEmpty(dir) && !dir.Contains("System32"))
                        return dir;
                }
            }
            catch { }

            string startup = Application.StartupPath;
            if (!string.IsNullOrEmpty(startup) && !startup.Contains("System32"))
                return startup;

            return AppDomain.CurrentDomain.BaseDirectory;
        }

        public static string DispatchesFolder => Path.Combine(GetAppDirectory(), "Dispatches");
        public static string ContactsFilePath => Path.Combine(GetAppDirectory(), "dispatch_contacts.txt");

        static RecordDispatchService()
        {
            try
            {
                if (!Directory.Exists(DispatchesFolder))
                    Directory.CreateDirectory(DispatchesFolder);
            }
            catch { }
        }

        #region Contact Management (Saved Railway Mobile Numbers)

        public class DispatchContact
        {
            public string Name { get; set; }
            public string Designation { get; set; }
            public string PhoneNumber { get; set; }
            public string TelegramHandle { get; set; }

            public override string ToString()
            {
                string des = string.IsNullOrWhiteSpace(Designation) ? "" : $" ({Designation})";
                return $"{Name}{des} - {PhoneNumber}";
            }
        }

        public static List<DispatchContact> LoadSavedContacts()
        {
            List<DispatchContact> contacts = new List<DispatchContact>();
            try
            {
                if (File.Exists(ContactsFilePath))
                {
                    string[] lines = File.ReadAllLines(ContactsFilePath);
                    foreach (string line in lines)
                    {
                        if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;
                        string[] parts = line.Split('|');
                        if (parts.Length >= 3)
                        {
                            contacts.Add(new DispatchContact
                            {
                                Name = parts[0].Trim(),
                                Designation = parts[1].Trim(),
                                PhoneNumber = parts[2].Trim(),
                                TelegramHandle = parts.Length >= 4 ? parts[3].Trim() : ""
                            });
                        }
                    }
                    return contacts; // If file exists, return the loaded list even if empty
                }
            }
            catch { }

            // Default Railway Operational Contacts ONLY on very first run if file does not exist
            contacts.Add(new DispatchContact { Name = "Chief Section Controller", Designation = "Control Office", PhoneNumber = "+919876543210", TelegramHandle = "@SectionController" });
            contacts.Add(new DispatchContact { Name = "Traffic Inspector (TI)", Designation = "Division HQ", PhoneNumber = "+919811223344", TelegramHandle = "@TrafficInspector" });
            contacts.Add(new DispatchContact { Name = "Station Superintendent", Designation = "Station Master Office", PhoneNumber = "+919822334455", TelegramHandle = "@StationSuperintendent" });
            contacts.Add(new DispatchContact { Name = "S&T Maintainer (ESM)", Designation = "Signal & Telecom", PhoneNumber = "+919833445566", TelegramHandle = "@ST_Maintainer" });
            contacts.Add(new DispatchContact { Name = "Safety Counselor / Officer", Designation = "Safety Dept", PhoneNumber = "+919844556677", TelegramHandle = "@SafetyOfficer" });
            SaveContacts(contacts);

            return contacts;
        }

        public static void SaveContacts(List<DispatchContact> contacts)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("# Railway Dispatch Contacts (Format: Name|Designation|PhoneNumber|TelegramHandle)");
                foreach (var c in contacts)
                {
                    sb.AppendLine($"{c.Name}|{c.Designation}|{c.PhoneNumber}|{c.TelegramHandle}");
                }
                File.WriteAllText(ContactsFilePath, sb.ToString(), Encoding.UTF8);
            }
            catch { }
        }

        public static void AddContact(string name, string designation, string phone, string telegram)
        {
            var list = LoadSavedContacts();
            list.RemoveAll(x => x.PhoneNumber == phone.Trim());
            list.Insert(0, new DispatchContact
            {
                Name = name.Trim(),
                Designation = designation.Trim(),
                PhoneNumber = phone.Trim(),
                TelegramHandle = telegram.Trim()
            });
            SaveContacts(list);
        }

        public static void RemoveContact(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return;
            var list = LoadSavedContacts();
            list.RemoveAll(x => x.PhoneNumber.Trim() == phone.Trim());
            SaveContacts(list);
        }

        public static void RemoveContacts(IEnumerable<string> phones)
        {
            if (phones == null) return;
            var list = LoadSavedContacts();
            var phoneSet = new HashSet<string>(phones.Select(p => p.Trim()));
            list.RemoveAll(x => phoneSet.Contains(x.PhoneNumber.Trim()));
            SaveContacts(list);
        }

        #endregion

        #region Form Handler (Invoked when SEND button is clicked on any register form)

        public static void HandleSendButtonClick(Form form)
        {
            if (form == null || form.IsDisposed) return;

            // 1. Find the SAVE button on the form
            Button btnSave = form.Controls.Find("btnSave", true).FirstOrDefault() as Button;
            if (btnSave == null)
            {
                btnSave = FindButtonByText(form, "SAVE");
            }

            if (btnSave == null)
            {
                MessageBox.Show("Could not find the Save action for this form.", "Dispatch Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 2. Register details
            string regTitle = form.Text;
            string regCode = "REG";
            Match m = Regex.Match(regTitle, @"REG[-_ ]?([0-9]{3})", RegexOptions.IgnoreCase);
            if (m.Success)
            {
                regCode = "REG-" + m.Groups[1].Value;
            }
            else
            {
                Match m2 = Regex.Match(form.GetType().Name, @"Reg([0-9]{3})", RegexOptions.IgnoreCase);
                if (m2.Success) regCode = "REG-" + m2.Groups[1].Value;
            }

            // 3. Extract all key-value field data before save
            Dictionary<string, string> fieldData = ExtractFormFields(form);

            // 4. Validate mandatory fields before sending - STRICT SEND GATE
            if (!ValidateFormBeforeDispatch(form, regCode, fieldData, out string valError))
            {
                MessageBox.Show(valError, "Mandatory Fields Required - Cannot Send", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 5. Extract or generate Log ID
            string logId = "TMS-REC-" + DateTime.Now.ToString("yyyyMMdd-HHmmss");
            foreach (var kvp in fieldData)
            {
                if (kvp.Key.ToLower().Contains("log") || kvp.Key.ToLower().Contains("id") || kvp.Key.ToLower().Contains("number"))
                {
                    if (!string.IsNullOrWhiteSpace(kvp.Value) && kvp.Value.StartsWith("TMS-"))
                    {
                        logId = kvp.Value;
                        break;
                    }
                }
            }

            // 6. Reset validation and execution tracking flags, then trigger save
            ValidationHelper.LastValidationPassed = false;
            DatabaseHelper.LastExecutionSuccessful = false;

            btnSave.PerformClick();

            // Strictly verify that save succeeded before proceeding to dispatch
            if (!ValidationHelper.LastValidationPassed || !DatabaseHelper.LastExecutionSuccessful)
            {
                return;
            }

            // 7. Generate official PDF Memo
            string pdfPath = GenerateRecordPdfMemo(regCode, regTitle, logId, fieldData);

            // 8. Open the 1-Click Dispatch Dialog
            RecordDispatchModalForm modal = new RecordDispatchModalForm(regCode, regTitle, logId, fieldData, pdfPath);
            modal.ShowDialog(form);
        }

        private static bool ValidateFormBeforeDispatch(Form form, string regCode, Dictionary<string, string> fieldData, out string errorMessage)
        {
            errorMessage = "";
            List<string> missing = new List<string>();

            List<Label> allLabels = new List<Label>();
            List<Control> allInputs = new List<Control>();
            CollectControls(form, allLabels, allInputs);

            // Check 1: Mandatory labels with asterisk '*'
            foreach (var lbl in allLabels)
            {
                if (lbl.Text.Contains("*"))
                {
                    string fieldName = lbl.Text.Trim().TrimEnd('*', ':', ' ');
                    if (string.IsNullOrWhiteSpace(fieldName)) continue;

                    Point lblPt = lbl.PointToScreen(Point.Empty);
                    Control matchedInput = null;
                    int minDistance = int.MaxValue;

                    foreach (var inp in allInputs)
                    {
                        Point inpPt = inp.PointToScreen(Point.Empty);
                        int dy = Math.Abs(inpPt.Y - lblPt.Y);
                        int dx = inpPt.X - lblPt.X;
                        if (dy < 45 && dx >= -10 && dx < 450)
                        {
                            int dist = dy * 10 + dx;
                            if (dist < minDistance)
                            {
                                minDistance = dist;
                                matchedInput = inp;
                            }
                        }
                    }

                    if (matchedInput != null)
                    {
                        if (matchedInput is TextBox tb && string.IsNullOrWhiteSpace(tb.Text))
                        {
                            if (!missing.Contains(fieldName)) missing.Add(fieldName);
                        }
                        else if (matchedInput is RichTextBox rtb && string.IsNullOrWhiteSpace(rtb.Text))
                        {
                            if (!missing.Contains(fieldName)) missing.Add(fieldName);
                        }
                        else if (matchedInput is ComboBox cb && (cb.SelectedIndex < 0 || string.IsNullOrWhiteSpace(cb.Text) || cb.Text.Trim().StartsWith("--")))
                        {
                            if (!missing.Contains(fieldName)) missing.Add(fieldName);
                        }
                        else if (matchedInput is CheckBox chk && !chk.Checked)
                        {
                            if (!missing.Contains(fieldName)) missing.Add(fieldName + " (Verification Required)");
                        }
                    }
                }
            }

            // Check 2: Checkboxes with asterisk '*' in their text (safety verifications)
            foreach (var inp in allInputs)
            {
                if (inp is CheckBox chk && chk.Text.Contains("*") && !chk.Checked)
                {
                    string chkName = chk.Text.Trim().TrimEnd('*', ':', ' ');
                    if (!missing.Contains(chkName)) missing.Add(chkName + " (Verification Required)");
                }
            }

            // Check 3: Explicit inspection of reflection controls for empty mandatory inputs
            var formFields = form.GetType().GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            foreach (var f in formFields)
            {
                string name = f.Name.ToLower();
                if (name.Contains("trainno") || name.Contains("train_no"))
                {
                    if (f.GetValue(form) is TextBox tb && string.IsNullOrWhiteSpace(tb.Text) && !missing.Any(m => m.ToLower().Contains("train")))
                        missing.Add("Train Number");
                }
                else if (name.Contains("staffid") || name.Contains("staff_id") || name.Contains("submittedby"))
                {
                    if (f.GetValue(form) is TextBox tb && string.IsNullOrWhiteSpace(tb.Text) && !missing.Any(m => m.ToLower().Contains("staff")))
                        missing.Add("Staff ID / Submitted By");
                }
            }

            // Check 4: RegisterValidationEngine evaluation
            Dictionary<string, object> engineValues = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            if (fieldData != null)
            {
                foreach (var kvp in fieldData)
                {
                    if (kvp.Value != "-" && !string.IsNullOrWhiteSpace(kvp.Value))
                    {
                        engineValues[kvp.Key] = kvp.Value;
                    }
                }
            }

            foreach (var f in formFields)
            {
                if (typeof(Control).IsAssignableFrom(f.FieldType))
                {
                    Control c = f.GetValue(form) as Control;
                    if (c != null)
                    {
                        string raw = f.Name.TrimStart('_');
                        string baseName = raw;
                        if (baseName.StartsWith("txt") || baseName.StartsWith("cmb") || baseName.StartsWith("dtp") || baseName.StartsWith("chk") || baseName.StartsWith("rtb") || baseName.StartsWith("num"))
                            baseName = baseName.Substring(3);

                        object val = null;
                        if (c is TextBox tb) val = tb.Text.Trim();
                        else if (c is RichTextBox rtb) val = rtb.Text.Trim();
                        else if (c is ComboBox cb) val = cb.SelectedItem?.ToString() ?? cb.Text.Trim();
                        else if (c is DateTimePicker dtp) val = dtp.Value;
                        else if (c is CheckBox chk) val = chk.Checked;
                        else if (c is NumericUpDown num) val = num.Value.ToString();

                        if (val != null)
                        {
                            engineValues[baseName] = val;
                            engineValues[raw] = val;
                        }
                    }
                }
            }

            ValidationResult valResult = RegisterValidationEngine.Validate(regCode, engineValues);
            if (!valResult.IsValid)
            {
                if (!string.IsNullOrWhiteSpace(valResult.FieldName) && !missing.Contains(valResult.FieldName))
                {
                    missing.Add(valResult.FieldName + $" ({valResult.ErrorReason})");
                }
            }

            if (missing.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("❌ Cannot Send Register Memo: Form is Incomplete!\n");
                sb.AppendLine("Please fill in all mandatory fields marked with (*) and confirm required verifications before sending:\n");
                foreach (var m in missing.Distinct())
                {
                    sb.AppendLine($"  • {m}");
                }
                errorMessage = sb.ToString();
                return false;
            }

            return true;
        }

        private static Button FindButtonByText(Control root, string searchText)
        {
            foreach (Control c in root.Controls)
            {
                if (c is Button b && b.Text.ToUpper().Contains(searchText.ToUpper()))
                    return b;
                Button found = FindButtonByText(c, searchText);
                if (found != null) return found;
            }
            return null;
        }

        public static Dictionary<string, string> ExtractFormFields(Form form)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();

            try
            {
                // 1. Scan Form Fields via Reflection for 100% reliable control access
                var formFields = form.GetType().GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                
                Dictionary<string, string> labelMap = new Dictionary<string, string>();
                List<Label> allLabels = new List<Label>();
                List<Control> allInputs = new List<Control>();

                CollectControls(form, allLabels, allInputs);

                // Map labels by name and position
                foreach (var lbl in allLabels)
                {
                    string cleanText = lbl.Text.Trim().TrimEnd('*', ':', ' ');
                    if (!string.IsNullOrWhiteSpace(cleanText) && cleanText.Length >= 2 && !cleanText.ToUpper().Contains("REGISTER") && !cleanText.ToUpper().Contains("HOME >") && !cleanText.ToUpper().Contains("TRAIN MANAGEMENT"))
                    {
                        if (!string.IsNullOrEmpty(lbl.Name))
                            labelMap[lbl.Name.ToLower()] = cleanText;
                    }
                }

                // Extract values from all input controls
                foreach (var field in formFields)
                {
                    if (typeof(Control).IsAssignableFrom(field.FieldType))
                    {
                        Control c = field.GetValue(form) as Control;
                        if (c != null && (c is TextBox || c is ComboBox || c is DateTimePicker || c is CheckBox || c is RichTextBox || c is NumericUpDown))
                        {
                            string rawName = field.Name.TrimStart('_');
                            string displayName = null;

                            // Check matching label by name e.g. lblLineNumber -> cmbLineNumber
                            string baseName = rawName;
                            if (baseName.StartsWith("txt") || baseName.StartsWith("cmb") || baseName.StartsWith("dtp") || baseName.StartsWith("chk") || baseName.StartsWith("rtb") || baseName.StartsWith("num"))
                                baseName = baseName.Substring(3);

                            string labelKey = "lbl" + baseName.ToLower();
                            if (labelMap.ContainsKey(labelKey))
                            {
                                displayName = labelMap[labelKey];
                            }
                            else
                            {
                                // Format from control variable name cleanly (e.g. StablingID -> Stabling ID)
                                displayName = Regex.Replace(baseName, @"([a-z])([A-Z])", "$1 $2").Trim();
                            }

                            string val = GetControlValue(c);
                            if (!string.IsNullOrWhiteSpace(displayName))
                            {
                                AddOrUpdateField(data, displayName, val);
                            }
                        }
                    }
                }

                // 2. Spatial proximity fallback for any dynamic/unnamed controls
                var sortedLabels = allLabels.OrderBy(l => l.PointToScreen(Point.Empty).Y).ThenBy(l => l.PointToScreen(Point.Empty).X).ToList();

                foreach (var lbl in sortedLabels)
                {
                    string fieldName = lbl.Text.Trim().TrimEnd('*', ':', ' ');
                    if (string.IsNullOrWhiteSpace(fieldName) || fieldName.Length < 2) continue;
                    if (fieldName.ToUpper().Contains("REGISTER") || fieldName.ToUpper().Contains("HOME >") || fieldName.ToUpper().Contains("TRAIN MANAGEMENT")) continue;

                    Point lblPt = lbl.PointToScreen(Point.Empty);
                    Control closest = null;
                    int minDistance = int.MaxValue;

                    foreach (var inp in allInputs)
                    {
                        Point inpPt = inp.PointToScreen(Point.Empty);
                        int dy = Math.Abs(inpPt.Y - lblPt.Y);
                        int dx = inpPt.X - lblPt.X;

                        if (dy < 50 && dx >= -10 && dx < 450)
                        {
                            int dist = dy * 10 + dx;
                            if (dist < minDistance)
                            {
                                minDistance = dist;
                                closest = inp;
                            }
                        }
                    }

                    if (closest != null)
                    {
                        string val = GetControlValue(closest);
                        AddOrUpdateField(data, fieldName, val);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Field extraction error: " + ex.Message);
            }

            // Always ensure timestamp and submitted by are recorded
            if (!data.ContainsKey("Logged At"))
            {
                data["Logged At"] = DateTime.Now.ToString("dd-MMM-yyyy hh:mm:ss tt");
            }
            if (!data.ContainsKey("Operator / Staff"))
            {
                data["Operator / Staff"] = $"{SessionManager.CurrentFullName} ({SessionManager.CurrentDepartment})";
            }

            return data;
        }

        private static void AddOrUpdateField(Dictionary<string, string> data, string name, string val)
        {
            string cleanVal = string.IsNullOrWhiteSpace(val) ? "-" : val.Trim();
            string norm = NormalizeFieldName(name);
            string existingKey = data.Keys.FirstOrDefault(k => NormalizeFieldName(k) == norm);

            if (existingKey != null)
            {
                if (name.Length > existingKey.Length && !existingKey.Contains("("))
                {
                    string oldVal = data[existingKey];
                    data.Remove(existingKey);
                    data[name] = (cleanVal == "-" && oldVal != "-") ? oldVal : cleanVal;
                }
                else if (cleanVal != "-" && data[existingKey] == "-")
                {
                    data[existingKey] = cleanVal;
                }
            }
            else
            {
                data[name] = cleanVal;
            }
        }

        private static string NormalizeFieldName(string name)
        {
            if (string.IsNullOrEmpty(name)) return "";
            return Regex.Replace(name, @"\s*\(.*?\)", "").Replace(" ", "").Replace("/", "").Replace("-", "").ToLower();
        }

        private static void CollectControls(Control parent, List<Label> labels, List<Control> inputs)
        {
            foreach (Control c in parent.Controls)
            {
                if (c is Label lbl && !lbl.Name.StartsWith("lblLogo") && !lbl.Name.StartsWith("lblPath"))
                {
                    labels.Add(lbl);
                }
                else if (c is TextBox || c is RichTextBox || c is ComboBox || c is DateTimePicker || c is CheckBox || c is NumericUpDown)
                {
                    inputs.Add(c);
                }
                
                if (c.HasChildren)
                {
                    CollectControls(c, labels, inputs);
                }
            }
        }

        private static string GetControlValue(Control c)
        {
            if (c is TextBox tb) return tb.Text.Trim();
            if (c is RichTextBox rtb) return rtb.Text.Trim();
            if (c is ComboBox cb) return cb.SelectedItem?.ToString() ?? cb.Text.Trim();
            if (c is DateTimePicker dtp) return dtp.Value.ToString("dd-MMM-yyyy HH:mm:ss");
            if (c is CheckBox chk) return chk.Checked ? "YES (Verified)" : "NO (Pending)";
            if (c is NumericUpDown num) return num.Value.ToString();
            return c.Text?.Trim() ?? "";
        }

        #endregion

        #region PDF Memo Generator

        public static string GenerateRecordPdfMemo(string regCode, string regTitle, string logId, Dictionary<string, string> fields)
        {
            try
            {
                if (!Directory.Exists(DispatchesFolder))
                    Directory.CreateDirectory(DispatchesFolder);

                string safeCode = Regex.Replace(regCode, @"[^\w\-]", "_");
                string safeLogId = Regex.Replace(logId, @"[^\w\-]", "_");
                string fileName = $"TMS_Dispatch_{safeCode}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                string fullPath = Path.Combine(DispatchesFolder, fileName);

                int pageWidth = 595;
                int pageHeight = 842;

                using (FileStream fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
                using (StreamWriter sw = new StreamWriter(fs, Encoding.ASCII))
                {
                    StringBuilder sb = new StringBuilder();

                    // 1. Header Banner (Navy #213D77)
                    sb.AppendLine("0.129 0.239 0.467 rg"); // #213D77
                    sb.AppendLine($"20 740 {pageWidth - 40} 80 re f");

                    // Orange accent stripe
                    sb.AppendLine("0.984 0.475 0.169 rg"); // #FB792B
                    sb.AppendLine($"20 735 {pageWidth - 40} 5 re f");

                    // Title Text
                    sb.AppendLine("BT");
                    sb.AppendLine("/F1 15 Tf");
                    sb.AppendLine("1 1 1 rg");
                    sb.AppendLine("1 0 0 1 36 788 Tm");
                    sb.AppendLine("(INDIAN RAILWAYS - TRAIN MANAGEMENT SYSTEM) Tj");
                    sb.AppendLine("1 0 0 1 36 770 Tm");
                    sb.AppendLine("/F1 12 Tf");
                    sb.AppendLine($"({EscapePdfText($"OFFICIAL DISPATCH MEMO  |  {regTitle.ToUpper()}")}) Tj");
                    sb.AppendLine("1 0 0 1 36 755 Tm");
                    sb.AppendLine("/F2 9 Tf");
                    sb.AppendLine($"({EscapePdfText($"Station Operations & Safety Record  |  Ref: {logId}")}) Tj");
                    sb.AppendLine("ET");

                    // 2. Metadata Summary Box
                    int metaY = 660;
                    sb.AppendLine("0.95 0.97 1.0 rg");
                    sb.AppendLine($"20 {metaY} {pageWidth - 40} 60 re f");
                    sb.AppendLine("0.8 0.85 0.92 RG");
                    sb.AppendLine("1 w");
                    sb.AppendLine($"20 {metaY} {pageWidth - 40} 60 re S");

                    sb.AppendLine("BT");
                    sb.AppendLine("/F1 9 Tf");
                    sb.AppendLine("0.1 0.2 0.4 rg");
                    sb.AppendLine($"1 0 0 1 32 {metaY + 38} Tm");
                    sb.AppendLine($"({EscapePdfText($"Record Log ID: {logId}")}) Tj");
                    sb.AppendLine($"1 0 0 1 310 {metaY + 38} Tm");
                    sb.AppendLine($"({EscapePdfText($"Dispatch Date: {DateTime.Now:dd-MMM-yyyy hh:mm tt}")}) Tj");
                    sb.AppendLine($"1 0 0 1 32 {metaY + 18} Tm");
                    sb.AppendLine($"({EscapePdfText($"Register Code: {regCode}")}) Tj");
                    string staffName = string.IsNullOrWhiteSpace(SessionManager.CurrentFullName) ? "Duty Station Master" : SessionManager.CurrentFullName;
                    string deptName = string.IsNullOrWhiteSpace(SessionManager.CurrentDepartment) ? "Operating / Traffic" : SessionManager.CurrentDepartment;
                    sb.AppendLine($"1 0 0 1 310 {metaY + 18} Tm");
                    sb.AppendLine($"({EscapePdfText($"Dispatched By: {staffName} ({deptName})")}) Tj");
                    sb.AppendLine("ET");

                    // 3. Field Key-Value Table
                    int tableTop = metaY - 24;
                    sb.AppendLine("0.129 0.239 0.467 rg");
                    sb.AppendLine($"20 {tableTop} {pageWidth - 40} 24 re f");

                    sb.AppendLine("BT");
                    sb.AppendLine("/F1 9.5 Tf");
                    sb.AppendLine("1 1 1 rg");
                    sb.AppendLine($"1 0 0 1 35 {tableTop + 7} Tm");
                    sb.AppendLine("(OPERATIONAL PARAMETER / ATTRIBUTE) Tj");
                    sb.AppendLine($"1 0 0 1 230 {tableTop + 7} Tm");
                    sb.AppendLine("(RECORDED VALUE / OBSERVATION DETAILS) Tj");
                    sb.AppendLine("ET");

                    int curY = tableTop - 22;
                    int rowH = 22;
                    int rowIdx = 0;

                    foreach (var kvp in fields)
                    {
                        if (curY < 120) break; // Keep within page margins

                        // Alternating row background
                        if (rowIdx % 2 == 1)
                        {
                            sb.AppendLine("0.96 0.97 0.98 rg");
                            sb.AppendLine($"20 {curY} {pageWidth - 40} {rowH} re f");
                        }

                        // Border line
                        sb.AppendLine("0.88 0.91 0.94 RG");
                        sb.AppendLine("0.5 w");
                        sb.AppendLine($"20 {curY} m {pageWidth - 20} {curY} l S");

                        string key = Truncate(kvp.Key, 32);
                        string val = Truncate(kvp.Value, 56);

                        sb.AppendLine("BT");
                        sb.AppendLine("/F1 8.5 Tf");
                        sb.AppendLine("0.12 0.18 0.3 rg");
                        sb.AppendLine($"1 0 0 1 32 {curY + 6} Tm");
                        sb.AppendLine($"({EscapePdfText(key)}) Tj");

                        sb.AppendLine("/F2 8.5 Tf");
                        sb.AppendLine("0.05 0.1 0.15 rg");
                        sb.AppendLine($"1 0 0 1 230 {curY + 6} Tm");
                        sb.AppendLine($"({EscapePdfText(val)}) Tj");
                        sb.AppendLine("ET");

                        curY -= rowH;
                        rowIdx++;
                    }

                    // 4. Official Verification Stamp Box
                    int stampY = Math.Max(50, curY - 30);
                    sb.AppendLine("0.94 0.98 0.95 rg");
                    sb.AppendLine($"20 {stampY} {pageWidth - 40} 45 re f");
                    sb.AppendLine("0.1 0.6 0.3 RG");
                    sb.AppendLine("1 w");
                    sb.AppendLine($"20 {stampY} {pageWidth - 40} 45 re S");

                    sb.AppendLine("BT");
                    sb.AppendLine("/F1 8.5 Tf");
                    sb.AppendLine("0.05 0.45 0.2 rg");
                    sb.AppendLine($"1 0 0 1 32 {stampY + 26} Tm");
                    sb.AppendLine("(✔ OFFICIAL RAILWAY DIGITAL RECORD - VERIFIED & DISPATCHED VIA TMS) Tj");
                    sb.AppendLine("/F2 7.5 Tf");
                    sb.AppendLine("0.3 0.35 0.4 rg");
                    sb.AppendLine($"1 0 0 1 32 {stampY + 10} Tm");
                    sb.AppendLine($"({EscapePdfText($"Permanent SQL Server Record ID: {logId} | Station Authority Stamp: {Environment.MachineName}")}) Tj");
                    sb.AppendLine("ET");

                    // Assemble single page PDF
                    string contentStr = sb.ToString();
                    byte[] contentBytes = Encoding.ASCII.GetBytes(contentStr);

                    List<string> pdfObjects = new List<string>();
                    pdfObjects.Add("<< /Type /Catalog /Pages 2 0 R >>");
                    pdfObjects.Add("<< /Type /Pages /Kids [3 0 R] /Count 1 >>");
                    pdfObjects.Add($"<< /Type /Page /Parent 2 0 R /MediaBox [0 0 {pageWidth} {pageHeight}] /Contents 4 0 R /Resources << /Font << /F1 5 0 R /F2 6 0 R >> >> >>");
                    pdfObjects.Add($"<< /Length {contentBytes.Length} >>\nstream\n{contentStr}\nendstream");
                    pdfObjects.Add("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold >>");
                    pdfObjects.Add("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>");

                    sw.WriteLine("%PDF-1.4");
                    List<long> offsets = new List<long>();
                    for (int i = 0; i < pdfObjects.Count; i++)
                    {
                        sw.Flush();
                        offsets.Add(fs.Position);
                        sw.WriteLine($"{i + 1} 0 obj");
                        sw.WriteLine(pdfObjects[i]);
                        sw.WriteLine("endobj");
                    }

                    sw.Flush();
                    long xrefPos = fs.Position;
                    sw.WriteLine("xref");
                    sw.WriteLine($"0 {pdfObjects.Count + 1}");
                    sw.WriteLine("0000000000 65535 f ");
                    foreach (long off in offsets)
                    {
                        sw.WriteLine($"{off:D10} 00000 n ");
                    }

                    sw.WriteLine("trailer");
                    sw.WriteLine($"<< /Size {pdfObjects.Count + 1} /Root 1 0 R >>");
                    sw.WriteLine("startxref");
                    sw.WriteLine(xrefPos);
                    sw.WriteLine("%%EOF");
                }

                return fullPath;
            }
            catch (Exception ex)
            {
                Console.WriteLine("PDF Memo Gen Error: " + ex.ToString());
                System.Diagnostics.Debug.WriteLine("PDF Memo Gen Error: " + ex.Message);
                return string.Empty;
            }
        }

        private static string EscapePdfText(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            return input.Replace("\\", "\\\\").Replace("(", "\\(").Replace(")", "\\)");
        }

        private static string Truncate(string input, int maxLen)
        {
            if (string.IsNullOrEmpty(input)) return "-";
            return input.Length <= maxLen ? input : input.Substring(0, maxLen - 2) + "..";
        }

        #endregion

        #region WhatsApp & Telegram 1-Click Dispatch Actions

        public static string UploadPdfForDirectLink(string pdfPath)
        {
            if (!File.Exists(pdfPath)) return null;

            try
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            }
            catch { }

            // Provider 1: Direct Raw PDF Stream (No HTML/disclaimers, opens directly in browser PDF viewer)
            try
            {
                using (var client = new System.Net.Http.HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(6);
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) TMS/1.0");
                    using (var fs = File.OpenRead(pdfPath))
                    {
                        var content = new System.Net.Http.StreamContent(fs);
                        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
                        var resp = client.PostAsync("https://paste.c-net.org/", content).Result;
                        if (resp.IsSuccessStatusCode)
                        {
                            string url = resp.Content.ReadAsStringAsync().Result.Trim();
                            if (url.StartsWith("http://") || url.StartsWith("https://"))
                            {
                                return url;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Provider 1 (direct) failed: " + ex.Message);
            }

            // Provider 2: GoFile.io (Clean, professional file viewer)
            try
            {
                using (var client = new System.Net.Http.HttpClient())
                {
                    client.Timeout = TimeSpan.FromSeconds(6);
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) TMS/1.0");
                    string srvResp = client.GetStringAsync("https://api.gofile.io/servers").Result;
                    Match m = Regex.Match(srvResp, "\"name\"\\s*:\\s*\"([^\"]+)\"");
                    string srv = m.Success ? m.Groups[1].Value : "store1";

                    using (var form = new System.Net.Http.MultipartFormDataContent())
                    using (var fs = File.OpenRead(pdfPath))
                    {
                        var fileContent = new System.Net.Http.StreamContent(fs);
                        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
                        form.Add(fileContent, "file", Path.GetFileName(pdfPath));

                        var resp = client.PostAsync("https://" + srv + ".gofile.io/contents/uploadfile", form).Result;
                        if (resp.IsSuccessStatusCode)
                        {
                            string res = resp.Content.ReadAsStringAsync().Result;
                            Match downloadPage = Regex.Match(res, "\"downloadPage\"\\s*:\\s*\"([^\"]+)\"");
                            if (downloadPage.Success)
                            {
                                return downloadPage.Groups[1].Value;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Provider 2 (gofile) failed: " + ex.Message);
            }

            return null;
        }

        public static string FormatDispatchMessage(string regCode, string regTitle, string logId, Dictionary<string, string> fields, string pdfPath, string directPdfUrl = null)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("🚆 *INDIAN RAILWAYS — STATION DISPATCH MEMO*");
            sb.AppendLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            sb.AppendLine($"📋 *Register:* {regTitle} ({regCode})");
            sb.AppendLine($"🆔 *Log ID:* `{logId}`");
            sb.AppendLine($"⏰ *Date & Time:* {DateTime.Now:dd-MMM-yyyy hh:mm:ss tt}");
            sb.AppendLine($"👤 *Dispatched By:* {SessionManager.CurrentFullName} ({SessionManager.CurrentDepartment})");
            sb.AppendLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━");

            sb.AppendLine("📝 *RECORD DETAILS:*");

            foreach (var kvp in fields)
            {
                if (kvp.Key == "Logged At" || kvp.Key == "Operator / Staff") continue;
                sb.AppendLine($"• *{kvp.Key}:* {kvp.Value}");
            }

            sb.AppendLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            sb.AppendLine("📄 *OFFICIAL PDF MEMO DOCUMENT:*");
            if (!string.IsNullOrWhiteSpace(directPdfUrl))
            {
                sb.AppendLine($"📥 *Download / View PDF:* {directPdfUrl}");
            }
            sb.AppendLine($"📁 *File Reference:* `{Path.GetFileName(pdfPath)}`");
            sb.AppendLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
            sb.AppendLine("✅ _Sent via Train Management System (TMS) 1-Click Operational Dispatch._");

            return sb.ToString();
        }

        public static bool DispatchToWhatsApp(string phoneNumber, string messageText, string pdfPath)
        {
            try
            {
                // Clean phone number (strip non-digits, keep leading + if present)
                string cleanPhone = Regex.Replace(phoneNumber, @"[^\d]", "");
                if (cleanPhone.Length == 10) cleanPhone = "91" + cleanPhone; // Default to India country code if 10 digits

                // Copy PDF file and message to Clipboard for instant paste
                CopyPdfFileToClipboard(pdfPath);

                string encodedMsg = Uri.EscapeDataString(messageText);
                string url = $"https://api.whatsapp.com/send?phone={cleanPhone}&text={encodedMsg}";

                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("WhatsApp Dispatch Error: " + ex.Message, "Dispatch Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public static bool DispatchToTelegram(string telegramHandleOrPhone, string messageText, string pdfPath)
        {
            try
            {
                // Copy PDF file and message to Clipboard for instant paste
                CopyPdfFileToClipboard(pdfPath);

                string encodedMsg = Uri.EscapeDataString(messageText);
                string handle = (telegramHandleOrPhone ?? "").Trim().TrimStart('@');
                
                string url;
                if (!string.IsNullOrEmpty(handle) && !Regex.IsMatch(handle, @"^\d+$"))
                {
                    // Telegram direct username link
                    url = $"https://t.me/{handle}?text={encodedMsg}";
                }
                else
                {
                    // Telegram web share dialog
                    url = $"https://t.me/share/url?url={Uri.EscapeDataString(Path.GetFileName(pdfPath))}&text={encodedMsg}";
                }

                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Telegram Dispatch Error: " + ex.Message, "Dispatch Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public static void CopyPdfFileToClipboard(string pdfPath)
        {
            try
            {
                if (File.Exists(pdfPath))
                {
                    var fileList = new System.Collections.Specialized.StringCollection();
                    fileList.Add(pdfPath);
                    DataObject data = new DataObject();
                    data.SetFileDropList(fileList);
                    Clipboard.SetDataObject(data, true);
                }
            }
            catch { }
        }

        public static void OpenPdfFile(string pdfPath)
        {
            try
            {
                if (File.Exists(pdfPath))
                {
                    Process.Start(new ProcessStartInfo { FileName = pdfPath, UseShellExecute = true });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not open PDF: " + ex.Message);
            }
        }

        public static void OpenPdfFolder(string pdfPath)
        {
            try
            {
                if (File.Exists(pdfPath))
                {
                    Process.Start("explorer.exe", $"/select,\"{pdfPath}\"");
                }
                else if (Directory.Exists(DispatchesFolder))
                {
                    Process.Start("explorer.exe", DispatchesFolder);
                }
            }
            catch { }
        }

        #endregion
    }
}
