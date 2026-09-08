using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TMS
{
    public class Form_Reg016_CrankHandle : Form
    {
        private TextBox txtLogID;
        private TextBox txtCrankHandleID;
        private TextBox txtPointNumber;
        private DateTimePicker dtpIssueTime;
        private DateTimePicker dtpReturnTime;
        private TextBox txtOperatorID;
        private TextBox txtAuthorizationPN;
        private CheckBox chkSafetyOverride;
        private TextBox txtSubmittedBy;
        private DatabaseHelper db = new DatabaseHelper();
        

        public Form_Reg016_CrankHandle()
        {
            this.Text = "Crank Handle (REG-016)";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.Size = new System.Drawing.Size(800, 700);
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.White;
            
            CreateControls();
            GenerateLogID();
        }

        private void CreateControls()
        {
            Panel headerPanel = new Panel();
            headerPanel.BackColor = System.Drawing.Color.FromArgb(0, 51, 102);
            headerPanel.Size = new System.Drawing.Size(800, 80);
            headerPanel.Dock = DockStyle.Top;
            this.Controls.Add(headerPanel);
            
            Label lblPath = new Label();
            lblPath.Text = "Home > Maintenance Sub > Crank Handle";
            lblPath.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Italic);
            lblPath.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);
            lblPath.Size = new System.Drawing.Size(750, 25);
            lblPath.Location = new System.Drawing.Point(25, 15);
            headerPanel.Controls.Add(lblPath);
            
            Label lblTitle = new Label();
            lblTitle.Text = "CRANK HANDLE REGISTER";
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.White;
            lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblTitle.Size = new System.Drawing.Size(750, 35);
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
            
            Label lblCrankHandleID = new Label();
            lblCrankHandleID.Text = "Crank Handle ID *";
            lblCrankHandleID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblCrankHandleID.Location = new System.Drawing.Point(30, y);
            lblCrankHandleID.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblCrankHandleID);
            
            txtCrankHandleID = new TextBox();
            txtCrankHandleID.Location = new System.Drawing.Point(170, y);
            txtCrankHandleID.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(txtCrankHandleID);
            
            y += 50;
            
            Label lblPointNumber = new Label();
            lblPointNumber.Text = "Point Number *";
            lblPointNumber.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblPointNumber.Location = new System.Drawing.Point(30, y);
            lblPointNumber.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblPointNumber);
            
            txtPointNumber = new TextBox();
            txtPointNumber.Location = new System.Drawing.Point(160, y);
            txtPointNumber.Size = new System.Drawing.Size(150, 30);
            this.Controls.Add(txtPointNumber);
            
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
            
            Label lblOperatorID = new Label();
            lblOperatorID.Text = "Operator ID *";
            lblOperatorID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblOperatorID.Location = new System.Drawing.Point(30, y);
            lblOperatorID.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblOperatorID);
            
            txtOperatorID = new TextBox();
            txtOperatorID.Location = new System.Drawing.Point(160, y);
            txtOperatorID.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(txtOperatorID);
            
            y += 50;
            
            Label lblAuthorizationPN = new Label();
            lblAuthorizationPN.Text = "Authorization PN *";
            lblAuthorizationPN.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblAuthorizationPN.Location = new System.Drawing.Point(30, y);
            lblAuthorizationPN.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblAuthorizationPN);
            
            txtAuthorizationPN = new TextBox();
            txtAuthorizationPN.Location = new System.Drawing.Point(170, y);
            txtAuthorizationPN.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(txtAuthorizationPN);
            
            y += 50;
            
            Label lblSafetyOverride = new Label();
            lblSafetyOverride.Text = "Safety Override Status *";
            lblSafetyOverride.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblSafetyOverride.Location = new System.Drawing.Point(30, y);
            lblSafetyOverride.Size = new System.Drawing.Size(170, 30);
            this.Controls.Add(lblSafetyOverride);
            
            chkSafetyOverride = new CheckBox();
            chkSafetyOverride.Text = "Override Active";
            chkSafetyOverride.Location = new System.Drawing.Point(210, y);
            chkSafetyOverride.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(chkSafetyOverride);
            
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
            btnView.Click += (s, e) => new ViewRecordsForm("Reg016_CrankHandle", "Crank Handle Records").ShowDialog();
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
            string query = $"SELECT COUNT(*) FROM Reg016_CrankHandle WHERE LogID LIKE 'TMS-REG-016-{datePart}-%'";
            int count = Convert.ToInt32(db.ExecuteScalar(query));
            txtLogID.Text = $"TMS-REG-016-{datePart}-{(count + 1).ToString("D3")}";
        }
        
        
            private void BtnSave_Click(object sender, EventArgs e)
        {
        
        
            if (string.IsNullOrWhiteSpace(txtSubmittedBy.Text) || !int.TryParse(txtSubmittedBy.Text.Trim(), out _))
            {
                MessageBox.Show("Submitted By must be a valid numeric Staff ID.", "Validation Error");
                return;
            }
            if (!ValidationHelper.IsNotEmpty(txtCrankHandleID.Text, "Crank Handle ID")) return;
            if (!ValidationHelper.IsNotEmpty(txtPointNumber.Text, "Point Number")) return;
            if (!ValidationHelper.IsNotEmpty(txtOperatorID.Text, "Operator ID")) return;
            if (!ValidationHelper.IsNotEmpty(txtAuthorizationPN.Text, "Authorization PN")) return;
            if (!ValidationHelper.IsEndAfterStart(dtpIssueTime.Value, dtpReturnTime.Value, "Issue Time", "Return Time")) return;
            
            string query = $@"
                INSERT INTO Reg016_CrankHandle (LogID, CrankHandleID, PointNumber, IssueTime, ReturnTime, OperatorID, AuthorizationPN, SafetyOverride, MaintenanceFlag, SubmittedBy)
                VALUES ('{txtLogID.Text}', '{txtCrankHandleID.Text}', '{txtPointNumber.Text}', 
                        '{dtpIssueTime.Value:yyyy-MM-dd HH:mm:ss.fff}', '{dtpReturnTime.Value:yyyy-MM-dd HH:mm:ss.fff}', 
                        '{txtOperatorID.Text}', '{txtAuthorizationPN.Text}', {(chkSafetyOverride.Checked ? "1" : "0")}, 0, {txtSubmittedBy.Text})";

            
            
            try
            {
                db.ExecuteNonQuery(query);
                MessageBox.Show($"Crank Handle Record Saved!\nLog ID: {txtLogID.Text}", "Success");
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
            txtCrankHandleID.Clear();
            txtPointNumber.Clear();
            dtpIssueTime.Value = DateTime.Now;
            dtpReturnTime.Value = DateTime.Now.AddHours(2);
            txtOperatorID.Clear();
            txtAuthorizationPN.Clear();
            chkSafetyOverride.Checked = false;
        }
    
        protected override void OnHandleCreated(System.EventArgs e) { base.OnHandleCreated(e); TMS.ThemeManager.ApplyTheme(this); }
    }
}
