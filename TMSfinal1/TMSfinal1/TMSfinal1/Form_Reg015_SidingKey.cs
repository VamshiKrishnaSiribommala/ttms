using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TMS
{
    public class Form_Reg015_SidingKey : Form
    {
        private TextBox txtTransactionID;
        private TextBox txtKeyNumber;
        private ComboBox cmbKeyType;
        private TextBox txtIssuedTo;
        private DateTimePicker dtpIssueTime;
        private DateTimePicker dtpReturnTime;
        private TextBox txtAuthID;
        private ComboBox cmbPurpose;
        private CheckBox chkChecklistStatus;
        private TextBox txtSubmittedBy;
        private DatabaseHelper db = new DatabaseHelper();
        

        public Form_Reg015_SidingKey()
        {
            this.Text = "Siding Key (REG-015)";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.Size = new System.Drawing.Size(800, 750);
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.White;
            
            CreateControls();
            GenerateTransactionID();
        }

        private void CreateControls()
        {
            Panel headerPanel = new Panel();
            headerPanel.BackColor = System.Drawing.Color.FromArgb(0, 51, 102);
            headerPanel.Size = new System.Drawing.Size(800, 80);
            headerPanel.Dock = DockStyle.Top;
            this.Controls.Add(headerPanel);
            
            Label lblPath = new Label();
            lblPath.Text = "Home > Maintenance Sub > Siding Key";
            lblPath.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Italic);
            lblPath.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);
            lblPath.Size = new System.Drawing.Size(750, 25);
            lblPath.Location = new System.Drawing.Point(25, 15);
            headerPanel.Controls.Add(lblPath);
            
            Label lblTitle = new Label();
            lblTitle.Text = "SIDING KEY REGISTER";
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.White;
            lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblTitle.Size = new System.Drawing.Size(750, 35);
            lblTitle.Location = new System.Drawing.Point(25, 40);
            headerPanel.Controls.Add(lblTitle);
            
            int y = 110;
            
            Label lblTransactionID = new Label();
            lblTransactionID.Text = "Transaction ID (System Generated):";
            lblTransactionID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblTransactionID.Location = new System.Drawing.Point(30, y);
            lblTransactionID.Size = new System.Drawing.Size(220, 30);
            this.Controls.Add(lblTransactionID);
            
            txtTransactionID = new TextBox();
            txtTransactionID.Location = new System.Drawing.Point(260, y);
            txtTransactionID.Size = new System.Drawing.Size(300, 30);
            txtTransactionID.ReadOnly = true;
            txtTransactionID.BackColor = System.Drawing.Color.LightGray;
            this.Controls.Add(txtTransactionID);
            
            y += 50;
            
            Label lblKeyNumber = new Label();
            lblKeyNumber.Text = "Key Number *";
            lblKeyNumber.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblKeyNumber.Location = new System.Drawing.Point(30, y);
            lblKeyNumber.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblKeyNumber);
            
            txtKeyNumber = new TextBox();
            txtKeyNumber.Location = new System.Drawing.Point(160, y);
            txtKeyNumber.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(txtKeyNumber);
            
            y += 50;
            
            Label lblKeyType = new Label();
            lblKeyType.Text = "Key Type *";
            lblKeyType.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblKeyType.Location = new System.Drawing.Point(30, y);
            lblKeyType.Size = new System.Drawing.Size(100, 30);
            this.Controls.Add(lblKeyType);
            
            cmbKeyType = new ComboBox();
            cmbKeyType.Location = new System.Drawing.Point(140, y);
            cmbKeyType.Size = new System.Drawing.Size(200, 30);
            cmbKeyType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbKeyType.Items.AddRange(new string[] { "Siding", "Relay Room", "Panel Room", "Others" });
            this.Controls.Add(cmbKeyType);
            
            y += 50;
            
            Label lblIssuedTo = new Label();
            lblIssuedTo.Text = "Issued To *";
            lblIssuedTo.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblIssuedTo.Location = new System.Drawing.Point(30, y);
            lblIssuedTo.Size = new System.Drawing.Size(100, 30);
            this.Controls.Add(lblIssuedTo);
            
            txtIssuedTo = new TextBox();
            txtIssuedTo.Location = new System.Drawing.Point(140, y);
            txtIssuedTo.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(txtIssuedTo);
            
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
            lblReturnTime.Text = "Return Time";
            lblReturnTime.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblReturnTime.Location = new System.Drawing.Point(30, y);
            lblReturnTime.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblReturnTime);
            
            dtpReturnTime = new DateTimePicker();
            dtpReturnTime.Location = new System.Drawing.Point(160, y);
            dtpReturnTime.Size = new System.Drawing.Size(200, 30);
            dtpReturnTime.Format = DateTimePickerFormat.Custom;
            dtpReturnTime.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            dtpReturnTime.ShowCheckBox = true;
            dtpReturnTime.Checked = false;
            this.Controls.Add(dtpReturnTime);
            
            y += 50;
            
            Label lblAuthID = new Label();
            lblAuthID.Text = "Authorization ID";
            lblAuthID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblAuthID.Location = new System.Drawing.Point(30, y);
            lblAuthID.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblAuthID);
            
            txtAuthID = new TextBox();
            txtAuthID.Location = new System.Drawing.Point(160, y);
            txtAuthID.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(txtAuthID);
            
            y += 50;
            
            Label lblPurpose = new Label();
            lblPurpose.Text = "Purpose *";
            lblPurpose.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblPurpose.Location = new System.Drawing.Point(30, y);
            lblPurpose.Size = new System.Drawing.Size(100, 30);
            this.Controls.Add(lblPurpose);
            
            cmbPurpose = new ComboBox();
            cmbPurpose.Location = new System.Drawing.Point(140, y);
            cmbPurpose.Size = new System.Drawing.Size(200, 30);
            cmbPurpose.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPurpose.Items.AddRange(new string[] { "Shunting", "Maintenance", "Inspection", "Emergency" });
            this.Controls.Add(cmbPurpose);
            
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
            btnView.Click += (s, e) => new ViewRecordsForm("Reg015_SidingKey", "Siding Key Records").ShowDialog();
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
        
        private void GenerateTransactionID()
        {
            string datePart = DateTime.Now.ToString("yyyyMMdd");
            string query = $"SELECT COUNT(*) FROM Reg015_SidingKey WHERE TransactionID LIKE 'TMS-REG-015-{datePart}-%'";
            int count = Convert.ToInt32(db.ExecuteScalar(query));
            txtTransactionID.Text = $"TMS-REG-015-{datePart}-{(count + 1).ToString("D3")}";
        }
        
        
            private void BtnSave_Click(object sender, EventArgs e)
        {
        
        
            if (string.IsNullOrWhiteSpace(txtSubmittedBy.Text) || !int.TryParse(txtSubmittedBy.Text.Trim(), out _))
            {
                MessageBox.Show("Submitted By must be a valid numeric Staff ID.", "Validation Error");
                return;
            }
            if (!ValidationHelper.IsNotEmpty(txtKeyNumber.Text, "Key Number")) return;
            if (!ValidationHelper.IsSelected(cmbKeyType, "Key Type")) return;
            if (!ValidationHelper.IsNotEmpty(txtIssuedTo.Text, "Issued To")) return;
            if (!ValidationHelper.IsSelected(cmbPurpose, "Purpose")) return;
            
            string returnTime = dtpReturnTime.Checked ? $"'{dtpReturnTime.Value:yyyy-MM-dd HH:mm:ss.fff}'" : "NULL";
            string authID = string.IsNullOrEmpty(txtAuthID.Text) ? "NULL" : $"'{txtAuthID.Text}'";
            
            string query = $@"
                INSERT INTO Reg015_SidingKey (TransactionID, KeyNumber, KeyType, IssuedTo, IssueTime, ReturnTime, AuthID, Purpose, ChecklistStatus, OverdueAlert, SubmittedBy)
                VALUES ('{txtTransactionID.Text}', '{txtKeyNumber.Text}', '{cmbKeyType.SelectedItem}', '{txtIssuedTo.Text}', 
                        '{dtpIssueTime.Value:yyyy-MM-dd HH:mm:ss.fff}', {returnTime}, {authID}, '{cmbPurpose.SelectedItem}', 
                        {(chkChecklistStatus.Checked ? "1" : "0")}, 0, {txtSubmittedBy.Text})";

            
            
            try
            {
                db.ExecuteNonQuery(query);
                MessageBox.Show($"Siding Key Record Saved!\nTransaction ID: {txtTransactionID.Text}", "Success");
                ClearForm();
                GenerateTransactionID();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error");
            }
        }
        
        
            private void ClearForm()
        {
        
        
            if (txtSubmittedBy != null) txtSubmittedBy.Clear();
            txtKeyNumber.Clear();
            cmbKeyType.SelectedIndex = -1;
            txtIssuedTo.Clear();
            dtpIssueTime.Value = DateTime.Now;
            dtpReturnTime.Checked = false;
            txtAuthID.Clear();
            cmbPurpose.SelectedIndex = -1;
            chkChecklistStatus.Checked = false;
        }
    
        protected override void OnHandleCreated(System.EventArgs e) { base.OnHandleCreated(e); TMS.ThemeManager.ApplyTheme(this); }
    }
}
