using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TMS
{
    public class Form_Reg018_CrossoverTest : Form
    {
        private TextBox txtTestID;
        private TextBox txtCrossoverID;
        private CheckBox chkLockingVerified;
        private CheckBox chkDetectionVerified;
        private TextBox txtMaintainerID;
        private TextBox txtSubmittedBy;
        private DatabaseHelper db = new DatabaseHelper();
        

        public Form_Reg018_CrossoverTest()
        {
            this.Text = "Cross-Over Testing (REG-018)";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.Size = new System.Drawing.Size(700, 550);
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.White;
            
            CreateControls();
            GenerateTestID();
        }

        private void CreateControls()
        {
            Panel headerPanel = new Panel();
            headerPanel.BackColor = System.Drawing.Color.FromArgb(0, 51, 102);
            headerPanel.Size = new System.Drawing.Size(700, 80);
            headerPanel.Dock = DockStyle.Top;
            this.Controls.Add(headerPanel);
            
            Label lblPath = new Label();
            lblPath.Text = "Home > Maintenance Sub > Cross-Over Testing";
            lblPath.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Italic);
            lblPath.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);
            lblPath.Size = new System.Drawing.Size(650, 25);
            lblPath.Location = new System.Drawing.Point(25, 15);
            headerPanel.Controls.Add(lblPath);
            
            Label lblTitle = new Label();
            lblTitle.Text = "CROSS-OVER TESTING REGISTER";
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.White;
            lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblTitle.Size = new System.Drawing.Size(650, 35);
            lblTitle.Location = new System.Drawing.Point(25, 40);
            headerPanel.Controls.Add(lblTitle);
            
            int y = 110;
            
            Label lblTestID = new Label();
            lblTestID.Text = "Test ID (System Generated):";
            lblTestID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblTestID.Location = new System.Drawing.Point(30, y);
            lblTestID.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(lblTestID);
            
            txtTestID = new TextBox();
            txtTestID.Location = new System.Drawing.Point(240, y);
            txtTestID.Size = new System.Drawing.Size(300, 30);
            txtTestID.ReadOnly = true;
            txtTestID.BackColor = System.Drawing.Color.LightGray;
            this.Controls.Add(txtTestID);
            
            y += 50;
            
            Label lblCrossoverID = new Label();
            lblCrossoverID.Text = "Crossover ID *";
            lblCrossoverID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblCrossoverID.Location = new System.Drawing.Point(30, y);
            lblCrossoverID.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblCrossoverID);
            
            txtCrossoverID = new TextBox();
            txtCrossoverID.Location = new System.Drawing.Point(160, y);
            txtCrossoverID.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(txtCrossoverID);
            
            y += 50;
            
            Label lblLockingVerified = new Label();
            lblLockingVerified.Text = "Locking Verified *";
            lblLockingVerified.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblLockingVerified.Location = new System.Drawing.Point(30, y);
            lblLockingVerified.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblLockingVerified);
            
            chkLockingVerified = new CheckBox();
            chkLockingVerified.Text = "Yes";
            chkLockingVerified.Location = new System.Drawing.Point(170, y);
            chkLockingVerified.Size = new System.Drawing.Size(80, 30);
            this.Controls.Add(chkLockingVerified);
            
            y += 50;
            
            Label lblDetectionVerified = new Label();
            lblDetectionVerified.Text = "Detection Verified *";
            lblDetectionVerified.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblDetectionVerified.Location = new System.Drawing.Point(30, y);
            lblDetectionVerified.Size = new System.Drawing.Size(140, 30);
            this.Controls.Add(lblDetectionVerified);
            
            chkDetectionVerified = new CheckBox();
            chkDetectionVerified.Text = "Yes";
            chkDetectionVerified.Location = new System.Drawing.Point(180, y);
            chkDetectionVerified.Size = new System.Drawing.Size(80, 30);
            this.Controls.Add(chkDetectionVerified);
            
            y += 50;
            
            Label lblMaintainerID = new Label();
            lblMaintainerID.Text = "Maintainer ID *";
            lblMaintainerID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblMaintainerID.Location = new System.Drawing.Point(30, y);
            lblMaintainerID.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblMaintainerID);
            
            txtMaintainerID = new TextBox();
            txtMaintainerID.Location = new System.Drawing.Point(160, y);
            txtMaintainerID.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(txtMaintainerID);
            
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
            btnView.Click += (s, e) => new ViewRecordsForm("Reg018_CrossoverTest", "Cross-Over Test Records").ShowDialog();
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
        
        private void GenerateTestID()
        {
            string datePart = DateTime.Now.ToString("yyyyMMdd");
            string query = $"SELECT COUNT(*) FROM Reg018_CrossoverTest WHERE TestID LIKE 'TMS-REG-018-{datePart}-%'";
            int count = Convert.ToInt32(db.ExecuteScalar(query));
            txtTestID.Text = $"TMS-REG-018-{datePart}-{(count + 1).ToString("D3")}";
        }
        
        
            private void BtnSave_Click(object sender, EventArgs e)
        {
        
        
            if (string.IsNullOrWhiteSpace(txtSubmittedBy.Text) || !int.TryParse(txtSubmittedBy.Text.Trim(), out _))
            {
                MessageBox.Show("Submitted By must be a valid numeric Staff ID.", "Validation Error");
                return;
            }
            if (!ValidationHelper.IsNotEmpty(txtCrossoverID.Text, "Crossover ID")) return;
            if (!ValidationHelper.IsNotEmpty(txtMaintainerID.Text, "Maintainer ID")) return;
            
            string query = $@"
                INSERT INTO Reg018_CrossoverTest (TestID, CrossoverID, LockingVerified, DetectionVerified, MaintainerID, SubmittedBy)
                VALUES ('{txtTestID.Text}', '{txtCrossoverID.Text}', {(chkLockingVerified.Checked ? "1" : "0")}, 
                        {(chkDetectionVerified.Checked ? "1" : "0")}', '{txtMaintainerID.Text}', {txtSubmittedBy.Text})";

            
            
            try
            {
                db.ExecuteNonQuery(query);
                MessageBox.Show($"Cross-Over Test Record Saved!\nTest ID: {txtTestID.Text}", "Success");
                ClearForm();
                GenerateTestID();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error");
            }
        }
        
        
            private void ClearForm()
        {
        
        
            if (txtSubmittedBy != null) txtSubmittedBy.Clear();
            txtCrossoverID.Clear();
            chkLockingVerified.Checked = false;
            chkDetectionVerified.Checked = false;
            txtMaintainerID.Clear();
        }
    
        protected override void OnHandleCreated(System.EventArgs e) { base.OnHandleCreated(e); TMS.ThemeManager.ApplyTheme(this); }
    }
}
