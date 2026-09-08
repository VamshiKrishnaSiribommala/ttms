using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TMS
{
    public class Form_Reg020_EmergencyKey : Form
    {
        private TextBox txtLogID;
        private TextBox txtEmergencyKeyID;
        private TextBox txtAssetID;
        private DateTimePicker dtpIssueTime;
        private DateTimePicker dtpReturnTime;
        private TextBox txtIssuingSMID;
        private TextBox txtControllerAuth;
        private CheckBox chkChecklistStatus;
        private TextBox txtSubmittedBy;
        private DatabaseHelper db = new DatabaseHelper();
        

        public Form_Reg020_EmergencyKey()
        {
            this.Text = "Emergency Key (REG-020)";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.Size = new System.Drawing.Size(750, 700);
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.White;
            
            CreateControls();
            GenerateLogID();
        }

        private void CreateControls()
        {
            Panel headerPanel = new Panel();
            headerPanel.BackColor = System.Drawing.Color.FromArgb(0, 51, 102);
            headerPanel.Size = new System.Drawing.Size(750, 80);
            headerPanel.Dock = DockStyle.Top;
            this.Controls.Add(headerPanel);
            
            Label lblPath = new Label();
            lblPath.Text = "Home > Maintenance Sub > Emergency Key";
            lblPath.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Italic);
            lblPath.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);
            lblPath.Size = new System.Drawing.Size(700, 25);
            lblPath.Location = new System.Drawing.Point(25, 15);
            headerPanel.Controls.Add(lblPath);
            
            Label lblTitle = new Label();
            lblTitle.Text = "EMERGENCY KEY REGISTER";
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.White;
            lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblTitle.Size = new System.Drawing.Size(700, 35);
            lblTitle.Location = new System.Drawing.Point(25, 40);
            headerPanel.Controls.Add(lblTitle);
            
            int y = 110;
            
            Label lblLogID = new Label();
            lblLogID.Text = "Log ID (System Generated):";
            lblLogID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblLogID.Location = new System.Drawing.Point(30, y);
            lblLogID.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(lblLogID);
            
            txtLogID = new TextBox();
            txtLogID.Location = new System.Drawing.Point(240, y);
            txtLogID.Size = new System.Drawing.Size(300, 30);
            txtLogID.ReadOnly = true;
            txtLogID.BackColor = System.Drawing.Color.LightGray;
            this.Controls.Add(txtLogID);
            
            y += 50;
            
            Label lblEmergencyKeyID = new Label();
            lblEmergencyKeyID.Text = "Emergency Key ID *";
            lblEmergencyKeyID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblEmergencyKeyID.Location = new System.Drawing.Point(30, y);
            lblEmergencyKeyID.Size = new System.Drawing.Size(140, 30);
            this.Controls.Add(lblEmergencyKeyID);
            
            txtEmergencyKeyID = new TextBox();
            txtEmergencyKeyID.Location = new System.Drawing.Point(180, y);
            txtEmergencyKeyID.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(txtEmergencyKeyID);
            
            y += 50;
            
            Label lblAssetID = new Label();
            lblAssetID.Text = "Asset ID *";
            lblAssetID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblAssetID.Location = new System.Drawing.Point(30, y);
            lblAssetID.Size = new System.Drawing.Size(100, 30);
            this.Controls.Add(lblAssetID);
            
            txtAssetID = new TextBox();
            txtAssetID.Location = new System.Drawing.Point(140, y);
            txtAssetID.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(txtAssetID);
            
            y += 50;
            
            Label lblIssueTime = new Label();
            lblIssueTime.Text = "Issue Time *";
            lblIssueTime.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblIssueTime.Location = new System.Drawing.Point(30, y);
            lblIssueTime.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblIssueTime);
            
            dtpIssueTime = new DateTimePicker();
            dtpIssueTime.Location = new System.Drawing.Point(160, y);
            dtpIssueTime.Size = new System.Drawing.Size(200, 30);
            dtpIssueTime.Format = DateTimePickerFormat.Custom;
            dtpIssueTime.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            this.Controls.Add(dtpIssueTime);
            
            y += 50;
            
            Label lblReturnTime = new Label();
            lblReturnTime.Text = "Return Time *";
            lblReturnTime.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblReturnTime.Location = new System.Drawing.Point(30, y);
            lblReturnTime.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblReturnTime);
            
            dtpReturnTime = new DateTimePicker();
            dtpReturnTime.Location = new System.Drawing.Point(160, y);
            dtpReturnTime.Size = new System.Drawing.Size(200, 30);
            dtpReturnTime.Format = DateTimePickerFormat.Custom;
            dtpReturnTime.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            this.Controls.Add(dtpReturnTime);
            
            y += 50;
            
            Label lblIssuingSMID = new Label();
            lblIssuingSMID.Text = "Issuing SM ID *";
            lblIssuingSMID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblIssuingSMID.Location = new System.Drawing.Point(30, y);
            lblIssuingSMID.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblIssuingSMID);
            
            txtIssuingSMID = new TextBox();
            txtIssuingSMID.Location = new System.Drawing.Point(160, y);
            txtIssuingSMID.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(txtIssuingSMID);
            
            y += 50;
            
            Label lblControllerAuth = new Label();
            lblControllerAuth.Text = "Controller Auth *";
            lblControllerAuth.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblControllerAuth.Location = new System.Drawing.Point(30, y);
            lblControllerAuth.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblControllerAuth);
            
            txtControllerAuth = new TextBox();
            txtControllerAuth.Location = new System.Drawing.Point(170, y);
            txtControllerAuth.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(txtControllerAuth);
            
            y += 50;
            
            Label lblChecklistStatus = new Label();
            lblChecklistStatus.Text = "Checklist Status *";
            lblChecklistStatus.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblChecklistStatus.Location = new System.Drawing.Point(30, y);
            lblChecklistStatus.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblChecklistStatus);
            
            chkChecklistStatus = new CheckBox();
            chkChecklistStatus.Text = "Completed";
            chkChecklistStatus.Location = new System.Drawing.Point(170, y);
            chkChecklistStatus.Size = new System.Drawing.Size(100, 30);
            this.Controls.Add(chkChecklistStatus);
            
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
            btnView.Click += (s, e) => new ViewRecordsForm("Reg020_EmergencyKey", "Emergency Key Records").ShowDialog();
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
        
        private void GenerateLogID()
        {
            string datePart = DateTime.Now.ToString("yyyyMMdd");
            string query = $"SELECT COUNT(*) FROM Reg020_EmergencyKey WHERE LogID LIKE 'TMS-REG-020-{datePart}-%'";
            int count = Convert.ToInt32(db.ExecuteScalar(query));
            txtLogID.Text = $"TMS-REG-020-{datePart}-{(count + 1).ToString("D3")}";
        }
        
        
            private void BtnSave_Click(object sender, EventArgs e)
        {
        
        
            if (string.IsNullOrWhiteSpace(txtSubmittedBy.Text) || !int.TryParse(txtSubmittedBy.Text.Trim(), out _))
            {
                MessageBox.Show("Submitted By must be a valid numeric Staff ID.", "Validation Error");
                return;
            }
            if (!ValidationHelper.IsNotEmpty(txtEmergencyKeyID.Text, "Emergency Key ID")) return;
            if (!ValidationHelper.IsNotEmpty(txtAssetID.Text, "Asset ID")) return;
            if (!ValidationHelper.IsNotEmpty(txtIssuingSMID.Text, "Issuing SM ID")) return;
            if (!ValidationHelper.IsNotEmpty(txtControllerAuth.Text, "Controller Auth")) return;
            if (!ValidationHelper.IsEndAfterStart(dtpIssueTime.Value, dtpReturnTime.Value, "Issue Time", "Return Time")) return;
            
            string query = $@"
                INSERT INTO Reg020_EmergencyKey (LogID, EmergencyKeyID, AssetID, IssueTime, ReturnTime, IssuingSMID, ControllerAuth, ChecklistStatus, SubmittedBy)
                VALUES ('{txtLogID.Text}', '{txtEmergencyKeyID.Text}', '{txtAssetID.Text}', 
                        '{dtpIssueTime.Value:yyyy-MM-dd HH:mm:ss.fff}', '{dtpReturnTime.Value:yyyy-MM-dd HH:mm:ss.fff}', 
                        '{txtIssuingSMID.Text}', '{txtControllerAuth.Text}', {(chkChecklistStatus.Checked ? "1" : "0")}, {txtSubmittedBy.Text})";

            
            
            try
            {
                db.ExecuteNonQuery(query);
                MessageBox.Show($"Emergency Key Record Saved!\nLog ID: {txtLogID.Text}", "Success");
                ClearForm();
                GenerateLogID();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error");
            }
        }
        
        
            private void ClearForm()
        {
        
        
            if (txtSubmittedBy != null) txtSubmittedBy.Clear();
            txtEmergencyKeyID.Clear();
            txtAssetID.Clear();
            dtpIssueTime.Value = DateTime.Now;
            dtpReturnTime.Value = DateTime.Now.AddHours(2);
            txtIssuingSMID.Clear();
            txtControllerAuth.Clear();
            chkChecklistStatus.Checked = false;
        }
    
        protected override void OnHandleCreated(System.EventArgs e) { base.OnHandleCreated(e); TMS.ThemeManager.ApplyTheme(this); }
    }
}
