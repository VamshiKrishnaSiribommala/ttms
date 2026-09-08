using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TMS
{
    public class Form_Reg010_NightInspection : Form
    {
        private TextBox txtInspectionID;
        private TextBox txtInspectingOfficer;
        private DateTimePicker dtpTimeOfVisit;
        private ComboBox cmbStaffAlertness;
        private RichTextBox txtObservations;
        private TextBox txtActionSuggested;
        private TextBox txtSubmittedBy;
        private DatabaseHelper db = new DatabaseHelper();
        

        public Form_Reg010_NightInspection()
        {
            this.Text = "Night Inspection (REG-010)";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.Size = new System.Drawing.Size(800, 700);
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.White;
            
            CreateControls();
            GenerateInspectionID();
        }

        private void CreateControls()
        {
            Panel headerPanel = new Panel();
            headerPanel.BackColor = System.Drawing.Color.FromArgb(0, 51, 102);
            headerPanel.Size = new System.Drawing.Size(800, 80);
            headerPanel.Dock = DockStyle.Top;
            this.Controls.Add(headerPanel);
            
            Label lblPath = new Label();
            lblPath.Text = "Home > Operational List > Night Inspection";
            lblPath.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Italic);
            lblPath.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);
            lblPath.Size = new System.Drawing.Size(750, 25);
            lblPath.Location = new System.Drawing.Point(25, 15);
            headerPanel.Controls.Add(lblPath);
            
            Label lblTitle = new Label();
            lblTitle.Text = "NIGHT INSPECTION REGISTER";
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.White;
            lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblTitle.Size = new System.Drawing.Size(750, 35);
            lblTitle.Location = new System.Drawing.Point(25, 40);
            headerPanel.Controls.Add(lblTitle);
            
            int y = 110;
            
            Label lblInspectionID = new Label();
            lblInspectionID.Text = "Inspection ID (System Generated):";
            lblInspectionID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblInspectionID.Location = new System.Drawing.Point(30, y);
            lblInspectionID.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(lblInspectionID);
            
            txtInspectionID = new TextBox();
            txtInspectionID.Location = new System.Drawing.Point(240, y);
            txtInspectionID.Size = new System.Drawing.Size(300, 30);
            txtInspectionID.ReadOnly = true;
            txtInspectionID.BackColor = System.Drawing.Color.LightGray;
            this.Controls.Add(txtInspectionID);
            
            y += 50;
            
            Label lblInspectingOfficer = new Label();
            lblInspectingOfficer.Text = "Inspecting Officer *";
            lblInspectingOfficer.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblInspectingOfficer.Location = new System.Drawing.Point(30, y);
            lblInspectingOfficer.Size = new System.Drawing.Size(150, 30);
            this.Controls.Add(lblInspectingOfficer);
            
            txtInspectingOfficer = new TextBox();
            txtInspectingOfficer.Location = new System.Drawing.Point(190, y);
            txtInspectingOfficer.Size = new System.Drawing.Size(350, 30);
            this.Controls.Add(txtInspectingOfficer);
            
            y += 50;
            
            Label lblTimeOfVisit = new Label();
            lblTimeOfVisit.Text = "Time of Visit *";
            lblTimeOfVisit.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblTimeOfVisit.Location = new System.Drawing.Point(30, y);
            lblTimeOfVisit.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblTimeOfVisit);
            
            dtpTimeOfVisit = new DateTimePicker();
            dtpTimeOfVisit.Location = new System.Drawing.Point(160, y);
            dtpTimeOfVisit.Size = new System.Drawing.Size(200, 30);
            dtpTimeOfVisit.Format = DateTimePickerFormat.Custom;
            dtpTimeOfVisit.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            this.Controls.Add(dtpTimeOfVisit);
            
            y += 50;
            
            Label lblStaffAlertness = new Label();
            lblStaffAlertness.Text = "Staff Alertness *";
            lblStaffAlertness.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblStaffAlertness.Location = new System.Drawing.Point(30, y);
            lblStaffAlertness.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblStaffAlertness);
            
            cmbStaffAlertness = new ComboBox();
            cmbStaffAlertness.Location = new System.Drawing.Point(160, y);
            cmbStaffAlertness.Size = new System.Drawing.Size(150, 30);
            cmbStaffAlertness.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStaffAlertness.Items.AddRange(new string[] { "Alert", "Not Alert", "Mixed" });
            this.Controls.Add(cmbStaffAlertness);
            
            y += 80;
            
            Label lblObservations = new Label();
            lblObservations.Text = "Observations *";
            lblObservations.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblObservations.Location = new System.Drawing.Point(30, y);
            lblObservations.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblObservations);
            
            txtObservations = new RichTextBox();
            txtObservations.Location = new System.Drawing.Point(30, y + 40);
            txtObservations.Size = new System.Drawing.Size(720, 100);
            txtObservations.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(txtObservations);
            
            y += 160;
            
            Label lblActionSuggested = new Label();
            lblActionSuggested.Text = "Action Suggested";
            lblActionSuggested.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblActionSuggested.Location = new System.Drawing.Point(30, y);
            lblActionSuggested.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblActionSuggested);
            
            txtActionSuggested = new TextBox();
            txtActionSuggested.Location = new System.Drawing.Point(160, y);
            txtActionSuggested.Size = new System.Drawing.Size(590, 30);
            this.Controls.Add(txtActionSuggested);
            
            y += 60;

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
            btnView.Click += (s, e) => new ViewRecordsForm("Reg010_NightInspection", "Night Inspection Records").ShowDialog();
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
            string datePart = DateTime.Now.ToString("yyyyMMdd");
            string query = $"SELECT COUNT(*) FROM Reg010_NightInspection WHERE InspectionID LIKE 'TMS-REG-010-{datePart}-%'";
            int count = Convert.ToInt32(db.ExecuteScalar(query));
            txtInspectionID.Text = $"TMS-REG-010-{datePart}-{(count + 1).ToString("D3")}";
        }
        
        
            private void BtnSave_Click(object sender, EventArgs e)
        {
        
        
            if (string.IsNullOrWhiteSpace(txtSubmittedBy.Text) || !int.TryParse(txtSubmittedBy.Text.Trim(), out _))
            {
                MessageBox.Show("Submitted By must be a valid numeric Staff ID.", "Validation Error");
                return;
            }
            if (!ValidationHelper.IsNotEmpty(txtInspectingOfficer.Text, "Inspecting Officer")) return;
            if (!ValidationHelper.IsSelected(cmbStaffAlertness, "Staff Alertness")) return;
            if (!ValidationHelper.IsNotEmpty(txtObservations.Text, "Observations")) return;
            
            string query = $@"
                INSERT INTO Reg010_NightInspection (InspectionID, InspectingOfficer, TimeOfVisit, StaffAlertness, Observations, ActionSuggested, SubmittedBy)
                VALUES ('{txtInspectionID.Text}', '{txtInspectingOfficer.Text}', '{dtpTimeOfVisit.Value:yyyy-MM-dd HH:mm:ss.fff}', 
                        '{cmbStaffAlertness.SelectedItem}', '{txtObservations.Text.Replace("'", "''")}', '{txtActionSuggested.Text.Replace("'", "''")}', {txtSubmittedBy.Text})";

            
            
            try
            {
                db.ExecuteNonQuery(query);
                MessageBox.Show($"Night Inspection Record Saved!\nInspection ID: {txtInspectionID.Text}", "Success");
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
            txtInspectingOfficer.Clear();
            dtpTimeOfVisit.Value = DateTime.Now;
            cmbStaffAlertness.SelectedIndex = -1;
            txtObservations.Clear();
            txtActionSuggested.Clear();
        }
    
        protected override void OnHandleCreated(System.EventArgs e) { base.OnHandleCreated(e); TMS.ThemeManager.ApplyTheme(this); }
    }
}
