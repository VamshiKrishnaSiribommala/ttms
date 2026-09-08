using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TMS
{
    public class Form_Reg034_EmployeeComplaint : Form
    {
        private TextBox txtReferenceID;
        private DateTimePicker dtpComplaintDateTime;
        private TextBox txtEmployeeID;
        private TextBox txtEmployeeName;
        private TextBox txtDepartment;
        private ComboBox cmbIssueCategory;
        private RichTextBox txtIssueDescription;
        private ComboBox cmbUrgencyLevel;
        private ComboBox cmbAssignedOfficer;
        private RichTextBox txtResolutionNote;
        private DateTimePicker dtpResolutionTime;
        private ComboBox cmbStatus;
        private TextBox txtSubmittedBy;
        private DatabaseHelper db = new DatabaseHelper();
        

        public Form_Reg034_EmployeeComplaint()
        {
            this.Text = "Employee Complaint (REG-034)";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.Size = new System.Drawing.Size(850, 850);
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.White;
            
            CreateControls();
            GenerateReferenceID();
        }

        private void CreateControls()
        {
            Panel headerPanel = new Panel();
            headerPanel.BackColor = System.Drawing.Color.FromArgb(0, 51, 102);
            headerPanel.Size = new System.Drawing.Size(850, 80);
            headerPanel.Dock = DockStyle.Top;
            this.Controls.Add(headerPanel);
            
            Label lblPath = new Label();
            lblPath.Text = "Home > Safety List > Employee Complaint";
            lblPath.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Italic);
            lblPath.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);
            lblPath.Size = new System.Drawing.Size(800, 25);
            lblPath.Location = new System.Drawing.Point(25, 15);
            headerPanel.Controls.Add(lblPath);
            
            Label lblTitle = new Label();
            lblTitle.Text = "EMPLOYEE COMPLAINT REGISTER";
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.White;
            lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblTitle.Size = new System.Drawing.Size(800, 35);
            lblTitle.Location = new System.Drawing.Point(25, 40);
            headerPanel.Controls.Add(lblTitle);
            
            int y = 110;
            
            Label lblReferenceID = new Label();
            lblReferenceID.Text = "Reference ID (System Generated):";
            lblReferenceID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblReferenceID.Location = new System.Drawing.Point(30, y);
            lblReferenceID.Size = new System.Drawing.Size(220, 30);
            this.Controls.Add(lblReferenceID);
            
            txtReferenceID = new TextBox();
            txtReferenceID.Location = new System.Drawing.Point(260, y);
            txtReferenceID.Size = new System.Drawing.Size(300, 30);
            txtReferenceID.ReadOnly = true;
            txtReferenceID.BackColor = System.Drawing.Color.LightGray;
            this.Controls.Add(txtReferenceID);
            
            y += 50;
            
            Label lblComplaintDateTime = new Label();
            lblComplaintDateTime.Text = "Complaint Date & Time *";
            lblComplaintDateTime.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblComplaintDateTime.Location = new System.Drawing.Point(30, y);
            lblComplaintDateTime.Size = new System.Drawing.Size(170, 30);
            this.Controls.Add(lblComplaintDateTime);
            
            dtpComplaintDateTime = new DateTimePicker();
            dtpComplaintDateTime.Location = new System.Drawing.Point(210, y);
            dtpComplaintDateTime.Size = new System.Drawing.Size(200, 30);
            dtpComplaintDateTime.Format = DateTimePickerFormat.Custom;
            dtpComplaintDateTime.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            this.Controls.Add(dtpComplaintDateTime);
            
            y += 50;
            
            Label lblEmployeeID = new Label();
            lblEmployeeID.Text = "Employee ID *";
            lblEmployeeID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblEmployeeID.Location = new System.Drawing.Point(30, y);
            lblEmployeeID.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblEmployeeID);
            
            txtEmployeeID = new TextBox();
            txtEmployeeID.Location = new System.Drawing.Point(160, y);
            txtEmployeeID.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(txtEmployeeID);
            
            y += 50;
            
            Label lblEmployeeName = new Label();
            lblEmployeeName.Text = "Employee Name *";
            lblEmployeeName.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblEmployeeName.Location = new System.Drawing.Point(30, y);
            lblEmployeeName.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblEmployeeName);
            
            txtEmployeeName = new TextBox();
            txtEmployeeName.Location = new System.Drawing.Point(170, y);
            txtEmployeeName.Size = new System.Drawing.Size(250, 30);
            this.Controls.Add(txtEmployeeName);
            
            y += 50;
            
            Label lblDepartment = new Label();
            lblDepartment.Text = "Department *";
            lblDepartment.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblDepartment.Location = new System.Drawing.Point(30, y);
            lblDepartment.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblDepartment);
            
            txtDepartment = new TextBox();
            txtDepartment.Location = new System.Drawing.Point(160, y);
            txtDepartment.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(txtDepartment);
            
            y += 50;
            
            Label lblIssueCategory = new Label();
            lblIssueCategory.Text = "Issue Category *";
            lblIssueCategory.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblIssueCategory.Location = new System.Drawing.Point(30, y);
            lblIssueCategory.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblIssueCategory);
            
            cmbIssueCategory = new ComboBox();
            cmbIssueCategory.Location = new System.Drawing.Point(170, y);
            cmbIssueCategory.Size = new System.Drawing.Size(200, 30);
            cmbIssueCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbIssueCategory.Items.AddRange(new string[] { "Admin", "Health", "Infrastructure", "Welfare", "Salary", "Leave", "Harassment", "Others" });
            this.Controls.Add(cmbIssueCategory);
            
            y += 80;
            
            Label lblIssueDescription = new Label();
            lblIssueDescription.Text = "Issue Description *";
            lblIssueDescription.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblIssueDescription.Location = new System.Drawing.Point(30, y);
            lblIssueDescription.Size = new System.Drawing.Size(140, 30);
            this.Controls.Add(lblIssueDescription);
            
            txtIssueDescription = new RichTextBox();
            txtIssueDescription.Location = new System.Drawing.Point(30, y + 40);
            txtIssueDescription.Size = new System.Drawing.Size(770, 100);
            txtIssueDescription.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(txtIssueDescription);
            
            y += 160;
            
            Label lblUrgencyLevel = new Label();
            lblUrgencyLevel.Text = "Urgency Level *";
            lblUrgencyLevel.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblUrgencyLevel.Location = new System.Drawing.Point(30, y);
            lblUrgencyLevel.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblUrgencyLevel);
            
            cmbUrgencyLevel = new ComboBox();
            cmbUrgencyLevel.Location = new System.Drawing.Point(170, y);
            cmbUrgencyLevel.Size = new System.Drawing.Size(150, 30);
            cmbUrgencyLevel.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbUrgencyLevel.Items.AddRange(new string[] { "Low", "Medium", "High", "Critical" });
            this.Controls.Add(cmbUrgencyLevel);
            
            y += 50;
            
            Label lblAssignedOfficer = new Label();
            lblAssignedOfficer.Text = "Assigned Officer *";
            lblAssignedOfficer.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblAssignedOfficer.Location = new System.Drawing.Point(30, y);
            lblAssignedOfficer.Size = new System.Drawing.Size(140, 30);
            this.Controls.Add(lblAssignedOfficer);
            
            cmbAssignedOfficer = new ComboBox();
            cmbAssignedOfficer.Location = new System.Drawing.Point(180, y);
            cmbAssignedOfficer.Size = new System.Drawing.Size(200, 30);
            cmbAssignedOfficer.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbAssignedOfficer.Items.AddRange(new string[] { "DRM", "ADRM", "Sr.DSO", "Divisional Officer", "HR Manager", "Station Manager" });
            this.Controls.Add(cmbAssignedOfficer);
            
            y += 80;
            
            Label lblResolutionNote = new Label();
            lblResolutionNote.Text = "Resolution Note *";
            lblResolutionNote.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblResolutionNote.Location = new System.Drawing.Point(30, y);
            lblResolutionNote.Size = new System.Drawing.Size(140, 30);
            this.Controls.Add(lblResolutionNote);
            
            txtResolutionNote = new RichTextBox();
            txtResolutionNote.Location = new System.Drawing.Point(30, y + 40);
            txtResolutionNote.Size = new System.Drawing.Size(770, 80);
            txtResolutionNote.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(txtResolutionNote);
            
            y += 140;
            
            Label lblResolutionTime = new Label();
            lblResolutionTime.Text = "Resolution Time";
            lblResolutionTime.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblResolutionTime.Location = new System.Drawing.Point(30, y);
            lblResolutionTime.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblResolutionTime);
            
            dtpResolutionTime = new DateTimePicker();
            dtpResolutionTime.Location = new System.Drawing.Point(170, y);
            dtpResolutionTime.Size = new System.Drawing.Size(200, 30);
            dtpResolutionTime.Format = DateTimePickerFormat.Custom;
            dtpResolutionTime.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            dtpResolutionTime.ShowCheckBox = true;
            dtpResolutionTime.Checked = false;
            this.Controls.Add(dtpResolutionTime);
            
            y += 50;
            
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
            cmbStatus.Items.AddRange(new string[] { "Submitted", "Under Review", "Resolved", "Closed" });
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
            btnView.Click += (s, e) => new ViewRecordsForm("Reg034_EmployeeComplaint", "Employee Complaint Records").ShowDialog();
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
        
        private void GenerateReferenceID()
        {
            string datePart = DateTime.Now.ToString("yyyyMM");
            string query = $"SELECT COUNT(*) FROM Reg034_EmployeeComplaint WHERE ReferenceID LIKE 'TMS-REG-034-{datePart}-%'";
            int count = Convert.ToInt32(db.ExecuteScalar(query));
            txtReferenceID.Text = $"TMS-REG-034-{datePart}-{(count + 1).ToString("D3")}";
        }
        
        
            private void BtnSave_Click(object sender, EventArgs e)
        {
        
        
            if (string.IsNullOrWhiteSpace(txtSubmittedBy.Text) || !int.TryParse(txtSubmittedBy.Text.Trim(), out _))
            {
                MessageBox.Show("Submitted By must be a valid numeric Staff ID.", "Validation Error");
                return;
            }
            if (!ValidationHelper.IsNotEmpty(txtEmployeeID.Text, "Employee ID")) return;
            if (!ValidationHelper.IsNotEmpty(txtEmployeeName.Text, "Employee Name")) return;
            if (!ValidationHelper.IsNotEmpty(txtDepartment.Text, "Department")) return;
            if (!ValidationHelper.IsSelected(cmbIssueCategory, "Issue Category")) return;
            if (!ValidationHelper.IsNotEmpty(txtIssueDescription.Text, "Issue Description")) return;
            if (!ValidationHelper.IsSelected(cmbUrgencyLevel, "Urgency Level")) return;
            if (!ValidationHelper.IsSelected(cmbAssignedOfficer, "Assigned Officer")) return;
            if (!ValidationHelper.IsNotEmpty(txtResolutionNote.Text, "Resolution Note")) return;
            if (!ValidationHelper.IsSelected(cmbStatus, "Status")) return;
            
            string resolutionTime = dtpResolutionTime.Checked ? $"'{dtpResolutionTime.Value:yyyy-MM-dd HH:mm:ss.fff}'" : "NULL";
            
            string query = $@"
                INSERT INTO Reg034_EmployeeComplaint (ReferenceID, ComplaintDateTime, EmployeeID, EmployeeName, Department, IssueCategory, IssueDescription, UrgencyLevel, AssignedOfficer, ResolutionNote, ResolutionTime, Status, SubmittedBy)
                VALUES ('{txtReferenceID.Text}', '{dtpComplaintDateTime.Value:yyyy-MM-dd HH:mm:ss.fff}', '{txtEmployeeID.Text}', 
                        '{txtEmployeeName.Text}', '{txtDepartment.Text}', '{cmbIssueCategory.SelectedItem}', 
                        '{txtIssueDescription.Text.Replace("'", "''")}', '{cmbUrgencyLevel.SelectedItem}', 
                        '{cmbAssignedOfficer.SelectedItem}', '{txtResolutionNote.Text.Replace("'", "''")}', 
                        {resolutionTime}, '{cmbStatus.SelectedItem}', {txtSubmittedBy.Text})";

            
            
            try
            {
                db.ExecuteNonQuery(query);
                MessageBox.Show($"Employee Complaint Saved!\nReference ID: {txtReferenceID.Text}", "Success");
                ClearForm();
                GenerateReferenceID();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error");
            }
        }
        
        
            private void ClearForm()
        {
        
        
            if (txtSubmittedBy != null) txtSubmittedBy.Clear();
            dtpComplaintDateTime.Value = DateTime.Now;
            txtEmployeeID.Clear();
            txtEmployeeName.Clear();
            txtDepartment.Clear();
            cmbIssueCategory.SelectedIndex = -1;
            txtIssueDescription.Clear();
            cmbUrgencyLevel.SelectedIndex = -1;
            cmbAssignedOfficer.SelectedIndex = -1;
            txtResolutionNote.Clear();
            dtpResolutionTime.Checked = false;
            cmbStatus.SelectedIndex = -1;
        }
    
        protected override void OnHandleCreated(System.EventArgs e) { base.OnHandleCreated(e); TMS.ThemeManager.ApplyTheme(this); }
    }
}
