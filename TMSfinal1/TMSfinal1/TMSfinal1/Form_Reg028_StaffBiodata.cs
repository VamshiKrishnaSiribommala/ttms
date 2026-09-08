using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TMS
{
    public class Form_Reg028_StaffBiodata : Form
    {
        private TextBox txtEmployeeID;
        private TextBox txtStaffName;
        private TextBox txtDesignation;
        private TextBox txtDepartment;
        private DateTimePicker dtpPMEDate;
        private DateTimePicker dtpPMEDueDate;
        private DateTimePicker dtpSafetyCampDate;
        private ComboBox cmbCompetencyType;
        private DateTimePicker dtpCompExpiryDate;
        private ComboBox cmbVisibilityTest;
        private TextBox txtA91RecordNo;
        private TextBox txtSubmittedBy;
        private DatabaseHelper db = new DatabaseHelper();
        

        public Form_Reg028_StaffBiodata()
        {
            this.Text = "Staff Biodata (REG-028)";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.Size = new System.Drawing.Size(850, 800);
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.White;
            
            CreateControls();
            GenerateEmployeeID();
        }

        private void CreateControls()
        {
            Panel headerPanel = new Panel();
            headerPanel.BackColor = System.Drawing.Color.FromArgb(0, 51, 102);
            headerPanel.Size = new System.Drawing.Size(850, 80);
            headerPanel.Dock = DockStyle.Top;
            this.Controls.Add(headerPanel);
            
            Label lblPath = new Label();
            lblPath.Text = "Home > Infrastructure Sub > Staff Biodata";
            lblPath.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Italic);
            lblPath.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);
            lblPath.Size = new System.Drawing.Size(800, 25);
            lblPath.Location = new System.Drawing.Point(25, 15);
            headerPanel.Controls.Add(lblPath);
            
            Label lblTitle = new Label();
            lblTitle.Text = "STAFF BIODATA (JEEVAN VRITTA";
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.White;
            lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblTitle.Size = new System.Drawing.Size(800, 35);
            lblTitle.Location = new System.Drawing.Point(25, 40);
            headerPanel.Controls.Add(lblTitle);
            
            int y = 110;
            
            Label lblEmployeeID = new Label();
            lblEmployeeID.Text = "Employee ID (System Generated):";
            lblEmployeeID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblEmployeeID.Location = new System.Drawing.Point(30, y);
            lblEmployeeID.Size = new System.Drawing.Size(220, 30);
            this.Controls.Add(lblEmployeeID);
            
            txtEmployeeID = new TextBox();
            txtEmployeeID.Location = new System.Drawing.Point(260, y);
            txtEmployeeID.Size = new System.Drawing.Size(250, 30);
            txtEmployeeID.ReadOnly = true;
            txtEmployeeID.BackColor = System.Drawing.Color.LightGray;
            this.Controls.Add(txtEmployeeID);
            
            y += 50;
            
            Label lblStaffName = new Label();
            lblStaffName.Text = "Staff Name *";
            lblStaffName.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblStaffName.Location = new System.Drawing.Point(30, y);
            lblStaffName.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblStaffName);
            
            txtStaffName = new TextBox();
            txtStaffName.Location = new System.Drawing.Point(160, y);
            txtStaffName.Size = new System.Drawing.Size(300, 30);
            this.Controls.Add(txtStaffName);
            
            y += 50;
            
            Label lblDesignation = new Label();
            lblDesignation.Text = "Designation *";
            lblDesignation.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblDesignation.Location = new System.Drawing.Point(30, y);
            lblDesignation.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblDesignation);
            
            txtDesignation = new TextBox();
            txtDesignation.Location = new System.Drawing.Point(160, y);
            txtDesignation.Size = new System.Drawing.Size(250, 30);
            this.Controls.Add(txtDesignation);
            
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
            
            Label lblPMEDate = new Label();
            lblPMEDate.Text = "PME Date *";
            lblPMEDate.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblPMEDate.Location = new System.Drawing.Point(30, y);
            lblPMEDate.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblPMEDate);
            
            dtpPMEDate = new DateTimePicker();
            dtpPMEDate.Location = new System.Drawing.Point(160, y);
            dtpPMEDate.Size = new System.Drawing.Size(180, 30);
            dtpPMEDate.Format = DateTimePickerFormat.Short;
            dtpPMEDate.MaxDate = DateTime.Today;
            this.Controls.Add(dtpPMEDate);
            
            y += 50;
            
            Label lblPMEDueDate = new Label();
            lblPMEDueDate.Text = "PME Due Date *";
            lblPMEDueDate.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblPMEDueDate.Location = new System.Drawing.Point(30, y);
            lblPMEDueDate.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblPMEDueDate);
            
            dtpPMEDueDate = new DateTimePicker();
            dtpPMEDueDate.Location = new System.Drawing.Point(170, y);
            dtpPMEDueDate.Size = new System.Drawing.Size(180, 30);
            dtpPMEDueDate.Format = DateTimePickerFormat.Short;
            dtpPMEDueDate.MinDate = DateTime.Today;
            this.Controls.Add(dtpPMEDueDate);
            
            y += 50;
            
            Label lblSafetyCampDate = new Label();
            lblSafetyCampDate.Text = "Safety Camp Date";
            lblSafetyCampDate.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblSafetyCampDate.Location = new System.Drawing.Point(30, y);
            lblSafetyCampDate.Size = new System.Drawing.Size(140, 30);
            this.Controls.Add(lblSafetyCampDate);
            
            dtpSafetyCampDate = new DateTimePicker();
            dtpSafetyCampDate.Location = new System.Drawing.Point(180, y);
            dtpSafetyCampDate.Size = new System.Drawing.Size(180, 30);
            dtpSafetyCampDate.Format = DateTimePickerFormat.Short;
            dtpSafetyCampDate.ShowCheckBox = true;
            dtpSafetyCampDate.Checked = false;
            this.Controls.Add(dtpSafetyCampDate);
            
            y += 50;
            
            Label lblCompetencyType = new Label();
            lblCompetencyType.Text = "Competency Type *";
            lblCompetencyType.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblCompetencyType.Location = new System.Drawing.Point(30, y);
            lblCompetencyType.Size = new System.Drawing.Size(140, 30);
            this.Controls.Add(lblCompetencyType);
            
            cmbCompetencyType = new ComboBox();
            cmbCompetencyType.Location = new System.Drawing.Point(180, y);
            cmbCompetencyType.Size = new System.Drawing.Size(200, 30);
            cmbCompetencyType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCompetencyType.Items.AddRange(new string[] { "Rules", "Points", "Block Operation", "Signal", "Train Working", "Others" });
            this.Controls.Add(cmbCompetencyType);
            
            y += 50;
            
            Label lblCompExpiryDate = new Label();
            lblCompExpiryDate.Text = "Competency Expiry Date *";
            lblCompExpiryDate.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblCompExpiryDate.Location = new System.Drawing.Point(30, y);
            lblCompExpiryDate.Size = new System.Drawing.Size(180, 30);
            this.Controls.Add(lblCompExpiryDate);
            
            dtpCompExpiryDate = new DateTimePicker();
            dtpCompExpiryDate.Location = new System.Drawing.Point(220, y);
            dtpCompExpiryDate.Size = new System.Drawing.Size(180, 30);
            dtpCompExpiryDate.Format = DateTimePickerFormat.Short;
            dtpCompExpiryDate.MinDate = DateTime.Today;
            this.Controls.Add(dtpCompExpiryDate);
            
            y += 50;
            
            Label lblVisibilityTest = new Label();
            lblVisibilityTest.Text = "Visibility Test *";
            lblVisibilityTest.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblVisibilityTest.Location = new System.Drawing.Point(30, y);
            lblVisibilityTest.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblVisibilityTest);
            
            cmbVisibilityTest = new ComboBox();
            cmbVisibilityTest.Location = new System.Drawing.Point(170, y);
            cmbVisibilityTest.Size = new System.Drawing.Size(120, 30);
            cmbVisibilityTest.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbVisibilityTest.Items.AddRange(new string[] { "Pass", "Fail" });
            this.Controls.Add(cmbVisibilityTest);
            
            y += 50;
            
            Label lblA91RecordNo = new Label();
            lblA91RecordNo.Text = "A-91 Record No *";
            lblA91RecordNo.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblA91RecordNo.Location = new System.Drawing.Point(30, y);
            lblA91RecordNo.Size = new System.Drawing.Size(140, 30);
            this.Controls.Add(lblA91RecordNo);
            
            txtA91RecordNo = new TextBox();
            txtA91RecordNo.Location = new System.Drawing.Point(180, y);
            txtA91RecordNo.Size = new System.Drawing.Size(250, 30);
            this.Controls.Add(txtA91RecordNo);
            
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
            btnView.Click += (s, e) => new ViewRecordsForm("Reg028_StaffBiodata", "Staff Biodata Records").ShowDialog();
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
        
        private void GenerateEmployeeID()
        {
            string year = DateTime.Now.ToString("yyyy");
            string query = $"SELECT COUNT(*) FROM Reg028_StaffBiodata WHERE EmployeeID LIKE 'STAFF-{year}-%'";
            int count = Convert.ToInt32(db.ExecuteScalar(query));
            txtEmployeeID.Text = $"STAFF-{year}-{(count + 1).ToString("D4")}";
        }
        
        
            private void BtnSave_Click(object sender, EventArgs e)
        {
        
        
            if (string.IsNullOrWhiteSpace(txtSubmittedBy.Text) || !int.TryParse(txtSubmittedBy.Text.Trim(), out _))
            {
                MessageBox.Show("Submitted By must be a valid numeric Staff ID.", "Validation Error");
                return;
            }
            if (!ValidationHelper.IsNotEmpty(txtStaffName.Text, "Staff Name")) return;
            if (!ValidationHelper.IsNotEmpty(txtDesignation.Text, "Designation")) return;
            if (!ValidationHelper.IsNotEmpty(txtDepartment.Text, "Department")) return;
            if (!ValidationHelper.IsSelected(cmbCompetencyType, "Competency Type")) return;
            if (!ValidationHelper.IsSelected(cmbVisibilityTest, "Visibility Test")) return;
            if (!ValidationHelper.IsNotEmpty(txtA91RecordNo.Text, "A-91 Record No")) return;
            
            string safetyCampDate = dtpSafetyCampDate.Checked ? $"'{dtpSafetyCampDate.Value:yyyy-MM-dd}'" : "NULL";
            
            string query = $@"
                INSERT INTO Reg028_StaffBiodata (EmployeeID, StaffName, Designation, Department, PMEDate, PMEDueDate, SafetyCampDate, CompetencyType, CompExpiryDate, VisibilityTest, A91RecordNo, RenewalAlert, SubmittedBy)
                VALUES ('{txtEmployeeID.Text}', '{txtStaffName.Text}', '{txtDesignation.Text}', '{txtDepartment.Text}', 
                        '{dtpPMEDate.Value:yyyy-MM-dd}', '{dtpPMEDueDate.Value:yyyy-MM-dd}', {safetyCampDate}, 
                        '{cmbCompetencyType.SelectedItem}', '{dtpCompExpiryDate.Value:yyyy-MM-dd}', 
                        '{cmbVisibilityTest.SelectedItem}', '{txtA91RecordNo.Text}', 0, {txtSubmittedBy.Text})";

            
            
            try
            {
                db.ExecuteNonQuery(query);
                MessageBox.Show($"Staff Biodata Saved!\nEmployee ID: {txtEmployeeID.Text}", "Success");
                ClearForm();
                GenerateEmployeeID();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error");
            }
        }
        
        
            private void ClearForm()
        {
        
        
            if (txtSubmittedBy != null) txtSubmittedBy.Clear();
            txtStaffName.Clear();
            txtDesignation.Clear();
            txtDepartment.Clear();
            dtpPMEDate.Value = DateTime.Now;
            dtpPMEDueDate.Value = DateTime.Now.AddYears(2);
            dtpSafetyCampDate.Checked = false;
            cmbCompetencyType.SelectedIndex = -1;
            dtpCompExpiryDate.Value = DateTime.Now.AddYears(1);
            cmbVisibilityTest.SelectedIndex = -1;
            txtA91RecordNo.Clear();
        }
    
        protected override void OnHandleCreated(System.EventArgs e) { base.OnHandleCreated(e); TMS.ThemeManager.ApplyTheme(this); }
    }
}
