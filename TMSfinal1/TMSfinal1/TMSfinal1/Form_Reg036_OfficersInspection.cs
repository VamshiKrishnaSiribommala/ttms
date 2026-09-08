using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TMS
{
    public class Form_Reg036_OfficersInspection : Form
    {
        private TextBox txtInspectionID;
        private DateTimePicker dtpDateOfVisit;
        private TextBox txtOfficerName;
        private TextBox txtOfficerID;
        private TextBox txtDesignation;
        private ComboBox cmbStationInspected;
        private RichTextBox txtItemsInspected;
        private RichTextBox txtIrregularitiesFound;
        private ComboBox cmbPriorityLevel;
        private DateTimePicker dtpComplianceDue;
        private RichTextBox txtSMComplianceRem;
        private ComboBox cmbStatus;
        private TextBox txtSubmittedBy;
        private DatabaseHelper db = new DatabaseHelper();
        

        public Form_Reg036_OfficersInspection()
        {
            this.Text = "Officers Inspection (REG-036)";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.Size = new System.Drawing.Size(850, 850);
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.White;
            
            CreateControls();
            GenerateInspectionID();
        }

        private void CreateControls()
        {
            Panel headerPanel = new Panel();
            headerPanel.BackColor = System.Drawing.Color.FromArgb(0, 51, 102);
            headerPanel.Size = new System.Drawing.Size(850, 80);
            headerPanel.Dock = DockStyle.Top;
            this.Controls.Add(headerPanel);
            
            Label lblPath = new Label();
            lblPath.Text = "Home > Safety List > Officers Inspection";
            lblPath.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Italic);
            lblPath.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);
            lblPath.Size = new System.Drawing.Size(800, 25);
            lblPath.Location = new System.Drawing.Point(25, 15);
            headerPanel.Controls.Add(lblPath);
            
            Label lblTitle = new Label();
            lblTitle.Text = "OFFICERS INSPECTION REGISTER";
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.White;
            lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblTitle.Size = new System.Drawing.Size(800, 35);
            lblTitle.Location = new System.Drawing.Point(25, 40);
            headerPanel.Controls.Add(lblTitle);
            
            int y = 110;
            
            Label lblInspectionID = new Label();
            lblInspectionID.Text = "Inspection ID (System Generated):";
            lblInspectionID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblInspectionID.Location = new System.Drawing.Point(30, y);
            lblInspectionID.Size = new System.Drawing.Size(220, 30);
            this.Controls.Add(lblInspectionID);
            
            txtInspectionID = new TextBox();
            txtInspectionID.Location = new System.Drawing.Point(260, y);
            txtInspectionID.Size = new System.Drawing.Size(300, 30);
            txtInspectionID.ReadOnly = true;
            txtInspectionID.BackColor = System.Drawing.Color.LightGray;
            this.Controls.Add(txtInspectionID);
            
            y += 50;
            
            Label lblDateOfVisit = new Label();
            lblDateOfVisit.Text = "Date & Time of Visit *";
            lblDateOfVisit.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblDateOfVisit.Location = new System.Drawing.Point(30, y);
            lblDateOfVisit.Size = new System.Drawing.Size(160, 30);
            this.Controls.Add(lblDateOfVisit);
            
            dtpDateOfVisit = new DateTimePicker();
            dtpDateOfVisit.Location = new System.Drawing.Point(200, y);
            dtpDateOfVisit.Size = new System.Drawing.Size(200, 30);
            dtpDateOfVisit.Format = DateTimePickerFormat.Custom;
            dtpDateOfVisit.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            dtpDateOfVisit.MaxDate = DateTime.Now;
            this.Controls.Add(dtpDateOfVisit);
            
            y += 50;
            
            Label lblOfficerName = new Label();
            lblOfficerName.Text = "Officer Name *";
            lblOfficerName.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblOfficerName.Location = new System.Drawing.Point(30, y);
            lblOfficerName.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblOfficerName);
            
            txtOfficerName = new TextBox();
            txtOfficerName.Location = new System.Drawing.Point(160, y);
            txtOfficerName.Size = new System.Drawing.Size(250, 30);
            this.Controls.Add(txtOfficerName);
            
            y += 50;
            
            Label lblOfficerID = new Label();
            lblOfficerID.Text = "Officer ID *";
            lblOfficerID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblOfficerID.Location = new System.Drawing.Point(30, y);
            lblOfficerID.Size = new System.Drawing.Size(100, 30);
            this.Controls.Add(lblOfficerID);
            
            txtOfficerID = new TextBox();
            txtOfficerID.Location = new System.Drawing.Point(140, y);
            txtOfficerID.Size = new System.Drawing.Size(180, 30);
            this.Controls.Add(txtOfficerID);
            
            y += 50;
            
            Label lblDesignation = new Label();
            lblDesignation.Text = "Designation *";
            lblDesignation.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblDesignation.Location = new System.Drawing.Point(30, y);
            lblDesignation.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblDesignation);
            
            txtDesignation = new TextBox();
            txtDesignation.Location = new System.Drawing.Point(160, y);
            txtDesignation.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(txtDesignation);
            
            y += 50;
            
            Label lblStationInspected = new Label();
            lblStationInspected.Text = "Station Inspected *";
            lblStationInspected.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblStationInspected.Location = new System.Drawing.Point(30, y);
            lblStationInspected.Size = new System.Drawing.Size(140, 30);
            this.Controls.Add(lblStationInspected);
            
            cmbStationInspected = new ComboBox();
            cmbStationInspected.Location = new System.Drawing.Point(180, y);
            cmbStationInspected.Size = new System.Drawing.Size(200, 30);
            cmbStationInspected.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStationInspected.Items.AddRange(new string[] { "Station A", "Station B", "Station C", "Station D", "Station E" });
            this.Controls.Add(cmbStationInspected);
            
            y += 80;
            
            Label lblItemsInspected = new Label();
            lblItemsInspected.Text = "Items Inspected *";
            lblItemsInspected.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblItemsInspected.Location = new System.Drawing.Point(30, y);
            lblItemsInspected.Size = new System.Drawing.Size(140, 30);
            this.Controls.Add(lblItemsInspected);
            
            txtItemsInspected = new RichTextBox();
            txtItemsInspected.Location = new System.Drawing.Point(30, y + 40);
            txtItemsInspected.Size = new System.Drawing.Size(770, 80);
            txtItemsInspected.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(txtItemsInspected);
            
            y += 140;
            
            Label lblIrregularitiesFound = new Label();
            lblIrregularitiesFound.Text = "Irregularities Found *";
            lblIrregularitiesFound.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblIrregularitiesFound.Location = new System.Drawing.Point(30, y);
            lblIrregularitiesFound.Size = new System.Drawing.Size(150, 30);
            this.Controls.Add(lblIrregularitiesFound);
            
            txtIrregularitiesFound = new RichTextBox();
            txtIrregularitiesFound.Location = new System.Drawing.Point(30, y + 40);
            txtIrregularitiesFound.Size = new System.Drawing.Size(770, 80);
            txtIrregularitiesFound.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(txtIrregularitiesFound);
            
            y += 140;
            
            Label lblPriorityLevel = new Label();
            lblPriorityLevel.Text = "Priority Level *";
            lblPriorityLevel.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblPriorityLevel.Location = new System.Drawing.Point(30, y);
            lblPriorityLevel.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblPriorityLevel);
            
            cmbPriorityLevel = new ComboBox();
            cmbPriorityLevel.Location = new System.Drawing.Point(170, y);
            cmbPriorityLevel.Size = new System.Drawing.Size(150, 30);
            cmbPriorityLevel.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPriorityLevel.Items.AddRange(new string[] { "Critical", "High", "Medium", "Low" });
            this.Controls.Add(cmbPriorityLevel);
            
            y += 50;
            
            Label lblComplianceDue = new Label();
            lblComplianceDue.Text = "Compliance Due *";
            lblComplianceDue.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblComplianceDue.Location = new System.Drawing.Point(30, y);
            lblComplianceDue.Size = new System.Drawing.Size(140, 30);
            this.Controls.Add(lblComplianceDue);
            
            dtpComplianceDue = new DateTimePicker();
            dtpComplianceDue.Location = new System.Drawing.Point(180, y);
            dtpComplianceDue.Size = new System.Drawing.Size(180, 30);
            dtpComplianceDue.Format = DateTimePickerFormat.Short;
            dtpComplianceDue.MinDate = DateTime.Today;
            this.Controls.Add(dtpComplianceDue);
            
            y += 80;
            
            Label lblSMComplianceRem = new Label();
            lblSMComplianceRem.Text = "SM Compliance Remarks";
            lblSMComplianceRem.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblSMComplianceRem.Location = new System.Drawing.Point(30, y);
            lblSMComplianceRem.Size = new System.Drawing.Size(180, 30);
            this.Controls.Add(lblSMComplianceRem);
            
            txtSMComplianceRem = new RichTextBox();
            txtSMComplianceRem.Location = new System.Drawing.Point(30, y + 40);
            txtSMComplianceRem.Size = new System.Drawing.Size(770, 80);
            txtSMComplianceRem.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(txtSMComplianceRem);
            
            y += 140;
            
            Label lblStatus = new Label();
            lblStatus.Text = "Status *";
            lblStatus.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblStatus.Location = new System.Drawing.Point(30, y);
            lblStatus.Size = new System.Drawing.Size(100, 30);
            this.Controls.Add(lblStatus);
            
            cmbStatus = new ComboBox();
            cmbStatus.Location = new System.Drawing.Point(140, y);
            cmbStatus.Size = new System.Drawing.Size(180, 30);
            cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStatus.Items.AddRange(new string[] { "Open", "Compliance Pending", "Complied", "Closed" });
            this.Controls.Add(cmbStatus);
            
            y += 80;

            Label lblSubmittedBy = new Label();
            lblSubmittedBy.Text = "Staff ID (Submitted By) *";
            lblSubmittedBy.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblSubmittedBy.Location = new System.Drawing.Point(30, y);
            lblSubmittedBy.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblSubmittedBy);
            
            txtSubmittedBy = new TextBox();
            txtSubmittedBy.Location = new System.Drawing.Point(160, y);
            txtSubmittedBy.Size = new System.Drawing.Size(630, 30);
            this.Controls.Add(txtSubmittedBy);
            
            y += 60;

            Button btnSave = new Button();

            btnSave.Text = "SAVE";
            btnSave.Size = new System.Drawing.Size(150, 45);
            btnSave.Location = new System.Drawing.Point(180, y);
            btnSave.BackColor = System.Drawing.Color.FromArgb(46, 204, 113);
            btnSave.ForeColor = System.Drawing.Color.White;
            btnSave.Font = new System.Drawing.Font("Segoe UI", 11, System.Drawing.FontStyle.Bold);
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);
            
            Button btnView = new Button();
            btnView.Text = "VIEW RECORDS";
            btnView.Size = new System.Drawing.Size(150, 45);
            btnView.Location = new System.Drawing.Point(350, y);
            btnView.BackColor = System.Drawing.Color.FromArgb(52, 152, 219);
            btnView.ForeColor = System.Drawing.Color.White;
            btnView.Font = new System.Drawing.Font("Segoe UI", 11, System.Drawing.FontStyle.Bold);
            btnView.FlatStyle = FlatStyle.Flat;
            btnView.Click += (s, e) => new ViewRecordsForm("Reg036_OfficersInspection", "Officers Inspection Records").ShowDialog();
            this.Controls.Add(btnView);
            
            Button btnClear = new Button();
            btnClear.Text = "CLEAR";
            btnClear.Size = new System.Drawing.Size(120, 45);
            btnClear.Location = new System.Drawing.Point(520, y);
            btnClear.BackColor = System.Drawing.Color.FromArgb(241, 196, 15);
            btnClear.ForeColor = System.Drawing.Color.Black;
            btnClear.Font = new System.Drawing.Font("Segoe UI", 11, System.Drawing.FontStyle.Bold);
            btnClear.FlatStyle = FlatStyle.Flat;
            btnClear.Click += (s, e) => ClearForm();
            this.Controls.Add(btnClear);
            
            Button btnBack = new Button();
            btnBack.Text = "BACK";
            btnBack.Size = new System.Drawing.Size(100, 45);
            btnBack.Location = new System.Drawing.Point(660, y);
            btnBack.BackColor = System.Drawing.Color.FromArgb(231, 76, 60);
            btnBack.ForeColor = System.Drawing.Color.White;
            btnBack.Font = new System.Drawing.Font("Segoe UI", 11, System.Drawing.FontStyle.Bold);
            btnBack.FlatStyle = FlatStyle.Flat;
            btnBack.DialogResult = DialogResult.Cancel;
            btnBack.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            this.Controls.Add(btnBack);
        }
        
        private void GenerateInspectionID()
        {
            string datePart = DateTime.Now.ToString("yyyyMM");
            string query = $"SELECT COUNT(*) FROM Reg036_OfficersInspection WHERE InspectionID LIKE 'TMS-REG-036-{datePart}-%'";
            int count = Convert.ToInt32(db.ExecuteScalar(query));
            txtInspectionID.Text = $"TMS-REG-036-{datePart}-{(count + 1).ToString("D3")}";
        }
        
        
            private void BtnSave_Click(object sender, EventArgs e)
        {
        
        
            if (string.IsNullOrWhiteSpace(txtSubmittedBy.Text) || !int.TryParse(txtSubmittedBy.Text.Trim(), out _))
            {
                MessageBox.Show("Submitted By must be a valid numeric Staff ID.", "Validation Error");
                return;
            }
            if (!ValidationHelper.IsNotEmpty(txtOfficerName.Text, "Officer Name")) return;
            if (!ValidationHelper.IsNotEmpty(txtOfficerID.Text, "Officer ID")) return;
            if (!ValidationHelper.IsNotEmpty(txtDesignation.Text, "Designation")) return;
            if (!ValidationHelper.IsSelected(cmbStationInspected, "Station Inspected")) return;
            if (!ValidationHelper.IsNotEmpty(txtItemsInspected.Text, "Items Inspected")) return;
            if (!ValidationHelper.IsNotEmpty(txtIrregularitiesFound.Text, "Irregularities Found")) return;
            if (!ValidationHelper.IsSelected(cmbPriorityLevel, "Priority Level")) return;
            if (!ValidationHelper.IsSelected(cmbStatus, "Status")) return;
            
            string smComplianceRem = string.IsNullOrEmpty(txtSMComplianceRem.Text) ? "NULL" : $"'{txtSMComplianceRem.Text.Replace("'", "''")}'";
            
            string query = $@"
                INSERT INTO Reg036_OfficersInspection (InspectionID, DateOfVisit, OfficerName, OfficerID, Designation, StationInspected, ItemsInspected, IrregularitiesFound, PriorityLevel, ComplianceDue, SMComplianceRem, Status, SubmittedBy)
                VALUES ('{txtInspectionID.Text}', '{dtpDateOfVisit.Value:yyyy-MM-dd HH:mm:ss.fff}', '{txtOfficerName.Text}', 
                        '{txtOfficerID.Text}', '{txtDesignation.Text}', '{cmbStationInspected.SelectedItem}', 
                        '{txtItemsInspected.Text.Replace("'", "''")}', '{txtIrregularitiesFound.Text.Replace("'", "''")}', 
                        '{cmbPriorityLevel.SelectedItem}', '{dtpComplianceDue.Value:yyyy-MM-dd}', {smComplianceRem}, 
                        '{cmbStatus.SelectedItem}', {txtSubmittedBy.Text})";

            
            
            try
            {
                db.ExecuteNonQuery(query);
                MessageBox.Show($"Officers Inspection Record Saved!\nInspection ID: {txtInspectionID.Text}", "Success");
                ClearForm();
                GenerateInspectionID();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error");
            }
        }
        
        
            private void ClearForm()
        {
        
        
            if (txtSubmittedBy != null) txtSubmittedBy.Clear();
            dtpDateOfVisit.Value = DateTime.Now;
            txtOfficerName.Clear();
            txtOfficerID.Clear();
            txtDesignation.Clear();
            cmbStationInspected.SelectedIndex = -1;
            txtItemsInspected.Clear();
            txtIrregularitiesFound.Clear();
            cmbPriorityLevel.SelectedIndex = -1;
            dtpComplianceDue.Value = DateTime.Now.AddDays(7);
            txtSMComplianceRem.Clear();
            cmbStatus.SelectedIndex = -1;
        }
    
        protected override void OnHandleCreated(System.EventArgs e) { base.OnHandleCreated(e); TMS.ThemeManager.ApplyTheme(this); }
    }
}
