using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TMS
{
    public class Form_Reg009_FogSignalman : Form
    {
        private TextBox txtDeploymentID;
        private TextBox txtStaffID;
        private TextBox txtLocation;
        private DateTimePicker dtpStartTime;
        private DateTimePicker dtpEndTime;
        private NumericUpDown numDetonatorsUsed;
        private TextBox txtSubmittedBy;
        private DatabaseHelper db = new DatabaseHelper();
        

        public Form_Reg009_FogSignalman()
        {
            this.Text = "Fog Signalman (REG-009)";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.Size = new System.Drawing.Size(750, 600);
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.White;
            
            CreateControls();
            GenerateDeploymentID();
        }

        private void CreateControls()
        {
            Panel headerPanel = new Panel();
            headerPanel.BackColor = System.Drawing.Color.FromArgb(0, 51, 102);
            headerPanel.Size = new System.Drawing.Size(750, 80);
            headerPanel.Dock = DockStyle.Top;
            this.Controls.Add(headerPanel);
            
            Label lblPath = new Label();
            lblPath.Text = "Home > Operational List > Fog Signalman";
            lblPath.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Italic);
            lblPath.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);
            lblPath.Size = new System.Drawing.Size(700, 25);
            lblPath.Location = new System.Drawing.Point(25, 15);
            headerPanel.Controls.Add(lblPath);
            
            Label lblTitle = new Label();
            lblTitle.Text = "FOG SIGNALMAN REGISTER";
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.White;
            lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblTitle.Size = new System.Drawing.Size(700, 35);
            lblTitle.Location = new System.Drawing.Point(25, 40);
            headerPanel.Controls.Add(lblTitle);
            
            int y = 110;
            
            Label lblDeploymentID = new Label();
            lblDeploymentID.Text = "Deployment ID (System Generated):";
            lblDeploymentID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblDeploymentID.Location = new System.Drawing.Point(30, y);
            lblDeploymentID.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(lblDeploymentID);
            
            txtDeploymentID = new TextBox();
            txtDeploymentID.Location = new System.Drawing.Point(240, y);
            txtDeploymentID.Size = new System.Drawing.Size(300, 30);
            txtDeploymentID.ReadOnly = true;
            txtDeploymentID.BackColor = System.Drawing.Color.LightGray;
            this.Controls.Add(txtDeploymentID);
            
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
            
            Label lblLocation = new Label();
            lblLocation.Text = "Location *";
            lblLocation.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblLocation.Location = new System.Drawing.Point(30, y);
            lblLocation.Size = new System.Drawing.Size(100, 30);
            this.Controls.Add(lblLocation);
            
            txtLocation = new TextBox();
            txtLocation.Location = new System.Drawing.Point(140, y);
            txtLocation.Size = new System.Drawing.Size(350, 30);
            this.Controls.Add(txtLocation);
            
            y += 50;
            
            Label lblStartTime = new Label();
            lblStartTime.Text = "Start Time *";
            lblStartTime.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblStartTime.Location = new System.Drawing.Point(30, y);
            lblStartTime.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblStartTime);
            
            dtpStartTime = new DateTimePicker();
            dtpStartTime.Location = new System.Drawing.Point(160, y);
            dtpStartTime.Size = new System.Drawing.Size(200, 30);
            dtpStartTime.Format = DateTimePickerFormat.Custom;
            dtpStartTime.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            this.Controls.Add(dtpStartTime);
            
            y += 50;
            
            Label lblEndTime = new Label();
            lblEndTime.Text = "End Time";
            lblEndTime.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblEndTime.Location = new System.Drawing.Point(30, y);
            lblEndTime.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblEndTime);
            
            dtpEndTime = new DateTimePicker();
            dtpEndTime.Location = new System.Drawing.Point(160, y);
            dtpEndTime.Size = new System.Drawing.Size(200, 30);
            dtpEndTime.Format = DateTimePickerFormat.Custom;
            dtpEndTime.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            dtpEndTime.ShowCheckBox = true;
            dtpEndTime.Checked = false;
            this.Controls.Add(dtpEndTime);
            
            y += 50;
            
            Label lblDetonatorsUsed = new Label();
            lblDetonatorsUsed.Text = "Detonators Used *";
            lblDetonatorsUsed.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblDetonatorsUsed.Location = new System.Drawing.Point(30, y);
            lblDetonatorsUsed.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblDetonatorsUsed);
            
            numDetonatorsUsed = new NumericUpDown();
            numDetonatorsUsed.Location = new System.Drawing.Point(170, y);
            numDetonatorsUsed.Size = new System.Drawing.Size(100, 30);
            numDetonatorsUsed.Minimum = 0;
            numDetonatorsUsed.Maximum = 100;
            this.Controls.Add(numDetonatorsUsed);
            
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
            btnView.Click += (s, e) => new ViewRecordsForm("Reg009_FogSignalman", "Fog Signalman Records").ShowDialog();
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
        
        private void GenerateDeploymentID()
        {
            string datePart = DateTime.Now.ToString("yyyyMMdd");
            string query = $"SELECT COUNT(*) FROM Reg009_FogSignalman WHERE DeploymentID LIKE 'TMS-REG-009-{datePart}-%'";
            int count = Convert.ToInt32(db.ExecuteScalar(query));
            txtDeploymentID.Text = $"TMS-REG-009-{datePart}-{(count + 1).ToString("D3")}";
        }
        
        
            private void BtnSave_Click(object sender, EventArgs e)
        {
        
        
            if (string.IsNullOrWhiteSpace(txtSubmittedBy.Text) || !int.TryParse(txtSubmittedBy.Text.Trim(), out _))
            {
                MessageBox.Show("Submitted By must be a valid numeric Staff ID.", "Validation Error");
                return;
            }
            if (!ValidationHelper.IsNotEmpty(txtStaffID.Text, "Staff ID")) return;
            if (!ValidationHelper.IsNotEmpty(txtLocation.Text, "Location")) return;
            
            string endTime = dtpEndTime.Checked ? $"'{dtpEndTime.Value:yyyy-MM-dd HH:mm:ss.fff}'" : "NULL";
            
            string query = $@"
                INSERT INTO Reg009_FogSignalman (DeploymentID, StaffID, Location, StartTime, EndTime, DetonatorsUsed, SubmittedBy)
                VALUES ('{txtDeploymentID.Text}', '{txtStaffID.Text}', '{txtLocation.Text}', 
                        '{dtpStartTime.Value:yyyy-MM-dd HH:mm:ss.fff}', {endTime}, {numDetonatorsUsed.Value}, {txtSubmittedBy.Text})";

            
            
            try
            {
                db.ExecuteNonQuery(query);
                MessageBox.Show($"Fog Signalman Record Saved!\nDeployment ID: {txtDeploymentID.Text}", "Success");
                ClearForm();
                GenerateDeploymentID();
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
            txtLocation.Clear();
            dtpStartTime.Value = DateTime.Now;
            dtpEndTime.Checked = false;
            dtpEndTime.Value = DateTime.Now;
            numDetonatorsUsed.Value = 0;
        }
    
        protected override void OnHandleCreated(System.EventArgs e) { base.OnHandleCreated(e); TMS.ThemeManager.ApplyTheme(this); }
    }
}
