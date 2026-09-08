using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TMS
{
    public class Form_Reg012_StaffGrievance : Form
    {
        private TextBox txtGrievanceID;
        private TextBox txtStaffID;
        private ComboBox cmbIssueType;
        private TextBox txtSubject;
        private ComboBox cmbStatus;
        private RichTextBox txtResolutionNote;
        private TextBox txtSubmittedBy;
        private DatabaseHelper db = new DatabaseHelper();
        

        public Form_Reg012_StaffGrievance()
        {
            this.Text = "Staff Grievance (REG-012)";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.Size = new System.Drawing.Size(800, 700);
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.White;
            
            CreateControls();
            GenerateGrievanceID();
        }

        private void CreateControls()
        {
            Panel headerPanel = new Panel();
            headerPanel.BackColor = System.Drawing.Color.FromArgb(0, 51, 102);
            headerPanel.Size = new System.Drawing.Size(800, 80);
            headerPanel.Dock = DockStyle.Top;
            this.Controls.Add(headerPanel);
            
            Label lblPath = new Label();
            lblPath.Text = "Home > Operational List > Staff Grievance";
            lblPath.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Italic);
            lblPath.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);
            lblPath.Size = new System.Drawing.Size(750, 25);
            lblPath.Location = new System.Drawing.Point(25, 15);
            headerPanel.Controls.Add(lblPath);
            
            Label lblTitle = new Label();
            lblTitle.Text = "STAFF GRIEVANCE REGISTER";
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.White;
            lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblTitle.Size = new System.Drawing.Size(750, 35);
            lblTitle.Location = new System.Drawing.Point(25, 40);
            headerPanel.Controls.Add(lblTitle);
            
            int y = 110;
            
            Label lblGrievanceID = new Label();
            lblGrievanceID.Text = "Grievance ID (System Generated):";
            lblGrievanceID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblGrievanceID.Location = new System.Drawing.Point(30, y);
            lblGrievanceID.Size = new System.Drawing.Size(220, 30);
            this.Controls.Add(lblGrievanceID);
            
            txtGrievanceID = new TextBox();
            txtGrievanceID.Location = new System.Drawing.Point(260, y);
            txtGrievanceID.Size = new System.Drawing.Size(300, 30);
            txtGrievanceID.ReadOnly = true;
            txtGrievanceID.BackColor = System.Drawing.Color.LightGray;
            this.Controls.Add(txtGrievanceID);
            
            y += 50;
            
            Label lblStaffID = new Label();
            lblStaffID.Text = "Staff ID *";
            lblStaffID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblStaffID.Location = new System.Drawing.Point(30, y);
            lblStaffID.Size = new System.Drawing.Size(100, 30);
            this.Controls.Add(lblStaffID);
            
            txtStaffID = new TextBox();
            txtStaffID.Location = new System.Drawing.Point(140, y);
            txtStaffID.Size = new System.Drawing.Size(150, 30);
            this.Controls.Add(txtStaffID);
            
            y += 50;
            
            Label lblIssueType = new Label();
            lblIssueType.Text = "Issue Type *";
            lblIssueType.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblIssueType.Location = new System.Drawing.Point(30, y);
            lblIssueType.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblIssueType);
            
            cmbIssueType = new ComboBox();
            cmbIssueType.Location = new System.Drawing.Point(160, y);
            cmbIssueType.Size = new System.Drawing.Size(200, 30);
            cmbIssueType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbIssueType.Items.AddRange(new string[] { "Rostering", "Leave", "Facilities", "Salary", "Promotion", "Transfer", "Others" });
            this.Controls.Add(cmbIssueType);
            
            y += 50;
            
            Label lblSubject = new Label();
            lblSubject.Text = "Subject *";
            lblSubject.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblSubject.Location = new System.Drawing.Point(30, y);
            lblSubject.Size = new System.Drawing.Size(100, 30);
            this.Controls.Add(lblSubject);
            
            txtSubject = new TextBox();
            txtSubject.Location = new System.Drawing.Point(140, y);
            txtSubject.Size = new System.Drawing.Size(400, 30);
            this.Controls.Add(txtSubject);
            
            y += 50;
            
            Label lblStatus = new Label();
            lblStatus.Text = "Status *";
            lblStatus.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblStatus.Location = new System.Drawing.Point(30, y);
            lblStatus.Size = new System.Drawing.Size(100, 30);
            this.Controls.Add(lblStatus);
            
            cmbStatus = new ComboBox();
            cmbStatus.Location = new System.Drawing.Point(140, y);
            cmbStatus.Size = new System.Drawing.Size(150, 30);
            cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStatus.Items.AddRange(new string[] { "Open", "Under Review", "Resolved", "Closed" });
            this.Controls.Add(cmbStatus);
            
            y += 80;
            
            Label lblResolutionNote = new Label();
            lblResolutionNote.Text = "Resolution Note";
            lblResolutionNote.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblResolutionNote.Location = new System.Drawing.Point(30, y);
            lblResolutionNote.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblResolutionNote);
            
            txtResolutionNote = new RichTextBox();
            txtResolutionNote.Location = new System.Drawing.Point(30, y + 40);
            txtResolutionNote.Size = new System.Drawing.Size(720, 100);
            txtResolutionNote.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(txtResolutionNote);
            
            y += 170;

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
            btnView.Click += (s, e) => new ViewRecordsForm("Reg012_StaffGrievance", "Staff Grievance Records").ShowDialog();
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
        
        private void GenerateGrievanceID()
        {
            string datePart = DateTime.Now.ToString("yyyyMMdd");
            string query = $"SELECT COUNT(*) FROM Reg012_StaffGrievance WHERE GrievanceID LIKE 'TMS-REG-012-{datePart}-%'";
            int count = Convert.ToInt32(db.ExecuteScalar(query));
            txtGrievanceID.Text = $"TMS-REG-012-{datePart}-{(count + 1).ToString("D3")}";
        }
        
        
            private void BtnSave_Click(object sender, EventArgs e)
        {
        
        
            if (string.IsNullOrWhiteSpace(txtSubmittedBy.Text) || !int.TryParse(txtSubmittedBy.Text.Trim(), out _))
            {
                MessageBox.Show("Submitted By must be a valid numeric Staff ID.", "Validation Error");
                return;
            }
            if (!ValidationHelper.IsNotEmpty(txtStaffID.Text, "Staff ID")) return;
            if (!ValidationHelper.IsSelected(cmbIssueType, "Issue Type")) return;
            if (!ValidationHelper.IsNotEmpty(txtSubject.Text, "Subject")) return;
            if (!ValidationHelper.IsSelected(cmbStatus, "Status")) return;
            
            string query = $@"
                INSERT INTO Reg012_StaffGrievance (GrievanceID, StaffID, IssueType, Subject, Status, ResolutionNote, SubmittedBy)
                VALUES ('{txtGrievanceID.Text}', '{txtStaffID.Text}', '{cmbIssueType.SelectedItem}', 
                        '{txtSubject.Text.Replace("'", "''")}', '{cmbStatus.SelectedItem}', '{txtResolutionNote.Text.Replace("'", "''")}', {txtSubmittedBy.Text})";

            
            
            try
            {
                db.ExecuteNonQuery(query);
                MessageBox.Show($"Grievance Record Saved!\nGrievance ID: {txtGrievanceID.Text}", "Success");
                ClearForm();
                GenerateGrievanceID();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error");
            }
        }
        
        
            private void ClearForm()
        {
        
        
            if (txtSubmittedBy != null) txtSubmittedBy.Clear();
            txtStaffID.Clear();
            cmbIssueType.SelectedIndex = -1;
            txtSubject.Clear();
            cmbStatus.SelectedIndex = -1;
            txtResolutionNote.Clear();
        }
    
        protected override void OnHandleCreated(System.EventArgs e) { base.OnHandleCreated(e); TMS.ThemeManager.ApplyTheme(this); }
    }
}
