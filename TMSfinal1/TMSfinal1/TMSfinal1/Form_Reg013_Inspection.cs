using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TMS
{
    public class Form_Reg013_Inspection : Form
    {
        private TextBox txtVisitID;
        private TextBox txtOfficerID;
        private DateTimePicker dtpInspectionDate;
        private ComboBox cmbScope;
        private RichTextBox txtObservations;
        private DateTimePicker dtpComplianceDate;
        private TextBox txtSubmittedBy;
        private DatabaseHelper db = new DatabaseHelper();
        

        public Form_Reg013_Inspection()
        {
            this.Text = "Inspection & Observation (REG-013)";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.Size = new System.Drawing.Size(800, 700);
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.White;
            
            CreateControls();
            GenerateVisitID();
        }

        private void CreateControls()
        {
            Panel headerPanel = new Panel();
            headerPanel.BackColor = System.Drawing.Color.FromArgb(0, 51, 102);
            headerPanel.Size = new System.Drawing.Size(800, 80);
            headerPanel.Dock = DockStyle.Top;
            this.Controls.Add(headerPanel);
            
            Label lblPath = new Label();
            lblPath.Text = "Home > Operational List > Inspection & Observation";
            lblPath.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Italic);
            lblPath.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);
            lblPath.Size = new System.Drawing.Size(750, 25);
            lblPath.Location = new System.Drawing.Point(25, 15);
            headerPanel.Controls.Add(lblPath);
            
            Label lblTitle = new Label();
            lblTitle.Text = "INSPECTION & OBSERVATION REGISTER";
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.White;
            lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblTitle.Size = new System.Drawing.Size(750, 35);
            lblTitle.Location = new System.Drawing.Point(25, 40);
            headerPanel.Controls.Add(lblTitle);
            
            int y = 110;
            
            Label lblVisitID = new Label();
            lblVisitID.Text = "Visit ID (System Generated):";
            lblVisitID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblVisitID.Location = new System.Drawing.Point(30, y);
            lblVisitID.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(lblVisitID);
            
            txtVisitID = new TextBox();
            txtVisitID.Location = new System.Drawing.Point(240, y);
            txtVisitID.Size = new System.Drawing.Size(300, 30);
            txtVisitID.ReadOnly = true;
            txtVisitID.BackColor = System.Drawing.Color.LightGray;
            this.Controls.Add(txtVisitID);
            
            y += 50;
            
            Label lblOfficerID = new Label();
            lblOfficerID.Text = "Officer ID *";
            lblOfficerID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblOfficerID.Location = new System.Drawing.Point(30, y);
            lblOfficerID.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblOfficerID);
            
            txtOfficerID = new TextBox();
            txtOfficerID.Location = new System.Drawing.Point(160, y);
            txtOfficerID.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(txtOfficerID);
            
            y += 50;
            
            Label lblInspectionDate = new Label();
            lblInspectionDate.Text = "Inspection Date *";
            lblInspectionDate.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblInspectionDate.Location = new System.Drawing.Point(30, y);
            lblInspectionDate.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblInspectionDate);
            
            dtpInspectionDate = new DateTimePicker();
            dtpInspectionDate.Location = new System.Drawing.Point(170, y);
            dtpInspectionDate.Size = new System.Drawing.Size(180, 30);
            dtpInspectionDate.Format = DateTimePickerFormat.Short;
            this.Controls.Add(dtpInspectionDate);
            
            y += 50;
            
            Label lblScope = new Label();
            lblScope.Text = "Scope *";
            lblScope.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblScope.Location = new System.Drawing.Point(30, y);
            lblScope.Size = new System.Drawing.Size(100, 30);
            this.Controls.Add(lblScope);
            
            cmbScope = new ComboBox();
            cmbScope.Location = new System.Drawing.Point(140, y);
            cmbScope.Size = new System.Drawing.Size(200, 30);
            cmbScope.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbScope.Items.AddRange(new string[] { "Full Station", "S&T", "Commercial", "Operations", "Safety" });
            this.Controls.Add(cmbScope);
            
            y += 80;
            
            Label lblObservations = new Label();
            lblObservations.Text = "Observations *";
            lblObservations.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblObservations.Location = new System.Drawing.Point(30, y);
            lblObservations.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblObservations);
            
            txtObservations = new RichTextBox();
            txtObservations.Location = new System.Drawing.Point(30, y + 40);
            txtObservations.Size = new System.Drawing.Size(720, 120);
            txtObservations.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(txtObservations);
            
            y += 180;
            
            Label lblComplianceDate = new Label();
            lblComplianceDate.Text = "Compliance Date *";
            lblComplianceDate.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblComplianceDate.Location = new System.Drawing.Point(30, y);
            lblComplianceDate.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblComplianceDate);
            
            dtpComplianceDate = new DateTimePicker();
            dtpComplianceDate.Location = new System.Drawing.Point(170, y);
            dtpComplianceDate.Size = new System.Drawing.Size(180, 30);
            dtpComplianceDate.Format = DateTimePickerFormat.Short;
            dtpComplianceDate.MinDate = DateTime.Today;
            this.Controls.Add(dtpComplianceDate);
            
            y += 70;

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
            btnView.Click += (s, e) => new ViewRecordsForm("Reg013_Inspection", "Inspection Records").ShowDialog();
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
        
        private void GenerateVisitID()
        {
            string datePart = DateTime.Now.ToString("yyyyMMdd");
            string query = $"SELECT COUNT(*) FROM Reg013_Inspection WHERE VisitID LIKE 'TMS-REG-013-{datePart}-%'";
            int count = Convert.ToInt32(db.ExecuteScalar(query));
            txtVisitID.Text = $"TMS-REG-013-{datePart}-{(count + 1).ToString("D3")}";
        }
        
        
            private void BtnSave_Click(object sender, EventArgs e)
        {
        
        
            if (string.IsNullOrWhiteSpace(txtSubmittedBy.Text) || !int.TryParse(txtSubmittedBy.Text.Trim(), out _))
            {
                MessageBox.Show("Submitted By must be a valid numeric Staff ID.", "Validation Error");
                return;
            }
            if (!ValidationHelper.IsNotEmpty(txtOfficerID.Text, "Officer ID")) return;
            if (!ValidationHelper.IsSelected(cmbScope, "Scope")) return;
            if (!ValidationHelper.IsNotEmpty(txtObservations.Text, "Observations")) return;
            
            string query = $@"
                INSERT INTO Reg013_Inspection (VisitID, OfficerID, InspectionDate, Scope, Observations, ComplianceDate, SubmittedBy)
                VALUES ('{txtVisitID.Text}', '{txtOfficerID.Text}', '{dtpInspectionDate.Value:yyyy-MM-dd}', 
                        '{cmbScope.SelectedItem}', '{txtObservations.Text.Replace("'", "''")}', '{dtpComplianceDate.Value:yyyy-MM-dd}', {txtSubmittedBy.Text})";

            
            
            try
            {
                db.ExecuteNonQuery(query);
                MessageBox.Show($"Inspection Record Saved!\nVisit ID: {txtVisitID.Text}", "Success");
                ClearForm();
                GenerateVisitID();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error");
            }
        }
        
        
            private void ClearForm()
        {
        
        
            if (txtSubmittedBy != null) txtSubmittedBy.Clear();
            txtOfficerID.Clear();
            dtpInspectionDate.Value = DateTime.Now;
            cmbScope.SelectedIndex = -1;
            txtObservations.Clear();
            dtpComplianceDate.Value = DateTime.Now.AddDays(7);
        }
    
        protected override void OnHandleCreated(System.EventArgs e) { base.OnHandleCreated(e); TMS.ThemeManager.ApplyTheme(this); }
    }
}
