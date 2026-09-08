using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TMS
{
    public class Form_Reg040_FailureInspection : Form
    {
        private TextBox txtFailureID;
        private TextBox txtFailureLinkID;
        private TextBox txtAssetID;
        private DateTimePicker dtpFailureTime;
        private ComboBox cmbFailureClass;
        private RichTextBox txtObsPreRepair;
        private RichTextBox txtRootCause;
        private RichTextBox txtRepairAction;
        private RichTextBox txtObsPostRepair;
        private ComboBox cmbTestResult;
        private DateTimePicker dtpRestorationTime;
        private TextBox txtVerifiedBy;
        private ComboBox cmbRecordStatus;
        private TextBox txtSubmittedBy;
        private DatabaseHelper db = new DatabaseHelper();
        

        public Form_Reg040_FailureInspection()
        {
            this.Text = "Failure Inspection (REG-040)";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.Size = new System.Drawing.Size(850, 900);
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.White;
            
            CreateControls();
            GenerateFailureID();
        }

        private void CreateControls()
        {
            Panel headerPanel = new Panel();
            headerPanel.BackColor = System.Drawing.Color.FromArgb(0, 51, 102);
            headerPanel.Size = new System.Drawing.Size(850, 80);
            headerPanel.Dock = DockStyle.Top;
            this.Controls.Add(headerPanel);
            
            Label lblPath = new Label();
            lblPath.Text = "Home > Maintenance Sub > Failure Inspection";
            lblPath.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Italic);
            lblPath.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);
            lblPath.Size = new System.Drawing.Size(800, 25);
            lblPath.Location = new System.Drawing.Point(25, 15);
            headerPanel.Controls.Add(lblPath);
            
            Label lblTitle = new Label();
            lblTitle.Text = "SIGNAL & BLOCK FAILURE INSPECTION BOOK";
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 14, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.White;
            lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblTitle.Size = new System.Drawing.Size(800, 35);
            lblTitle.Location = new System.Drawing.Point(25, 40);
            headerPanel.Controls.Add(lblTitle);
            
            int y = 110;
            
            Label lblFailureID = new Label();
            lblFailureID.Text = "Failure ID (System Generated):";
            lblFailureID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblFailureID.Location = new System.Drawing.Point(30, y);
            lblFailureID.Size = new System.Drawing.Size(220, 30);
            this.Controls.Add(lblFailureID);
            
            txtFailureID = new TextBox();
            txtFailureID.Location = new System.Drawing.Point(260, y);
            txtFailureID.Size = new System.Drawing.Size(300, 30);
            txtFailureID.ReadOnly = true;
            txtFailureID.BackColor = System.Drawing.Color.LightGray;
            this.Controls.Add(txtFailureID);
            
            y += 50;
            
            Label lblFailureLinkID = new Label();
            lblFailureLinkID.Text = "Failure Link ID *";
            lblFailureLinkID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblFailureLinkID.Location = new System.Drawing.Point(30, y);
            lblFailureLinkID.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblFailureLinkID);
            
            txtFailureLinkID = new TextBox();
            txtFailureLinkID.Location = new System.Drawing.Point(170, y);
            txtFailureLinkID.Size = new System.Drawing.Size(250, 30);
            this.Controls.Add(txtFailureLinkID);
            
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
            
            Label lblFailureTime = new Label();
            lblFailureTime.Text = "Failure Time *";
            lblFailureTime.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblFailureTime.Location = new System.Drawing.Point(30, y);
            lblFailureTime.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblFailureTime);
            
            dtpFailureTime = new DateTimePicker();
            dtpFailureTime.Location = new System.Drawing.Point(160, y);
            dtpFailureTime.Size = new System.Drawing.Size(200, 30);
            dtpFailureTime.Format = DateTimePickerFormat.Custom;
            dtpFailureTime.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            dtpFailureTime.MaxDate = DateTime.Now;
            this.Controls.Add(dtpFailureTime);
            
            y += 50;
            
            Label lblFailureClass = new Label();
            lblFailureClass.Text = "Failure Class *";
            lblFailureClass.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblFailureClass.Location = new System.Drawing.Point(30, y);
            lblFailureClass.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblFailureClass);
            
            cmbFailureClass = new ComboBox();
            cmbFailureClass.Location = new System.Drawing.Point(160, y);
            cmbFailureClass.Size = new System.Drawing.Size(200, 30);
            cmbFailureClass.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFailureClass.Items.AddRange(new string[] { "Bulb Fuse", "Point Failure", "Power Failure", "Cable Cut", "Relay Failure", "Software", "Others" });
            this.Controls.Add(cmbFailureClass);
            
            y += 80;
            
            Label lblObsPreRepair = new Label();
            lblObsPreRepair.Text = "Observations (Pre-Repair) *";
            lblObsPreRepair.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblObsPreRepair.Location = new System.Drawing.Point(30, y);
            lblObsPreRepair.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(lblObsPreRepair);
            
            txtObsPreRepair = new RichTextBox();
            txtObsPreRepair.Location = new System.Drawing.Point(30, y + 40);
            txtObsPreRepair.Size = new System.Drawing.Size(770, 80);
            txtObsPreRepair.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(txtObsPreRepair);
            
            y += 140;
            
            Label lblRootCause = new Label();
            lblRootCause.Text = "Root Cause *";
            lblRootCause.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblRootCause.Location = new System.Drawing.Point(30, y);
            lblRootCause.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblRootCause);
            
            txtRootCause = new RichTextBox();
            txtRootCause.Location = new System.Drawing.Point(30, y + 40);
            txtRootCause.Size = new System.Drawing.Size(770, 80);
            txtRootCause.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(txtRootCause);
            
            y += 140;
            
            Label lblRepairAction = new Label();
            lblRepairAction.Text = "Repair Action *";
            lblRepairAction.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblRepairAction.Location = new System.Drawing.Point(30, y);
            lblRepairAction.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblRepairAction);
            
            txtRepairAction = new RichTextBox();
            txtRepairAction.Location = new System.Drawing.Point(30, y + 40);
            txtRepairAction.Size = new System.Drawing.Size(770, 80);
            txtRepairAction.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(txtRepairAction);
            
            y += 140;
            
            Label lblObsPostRepair = new Label();
            lblObsPostRepair.Text = "Observations (Post-Repair) *";
            lblObsPostRepair.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblObsPostRepair.Location = new System.Drawing.Point(30, y);
            lblObsPostRepair.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(lblObsPostRepair);
            
            txtObsPostRepair = new RichTextBox();
            txtObsPostRepair.Location = new System.Drawing.Point(30, y + 40);
            txtObsPostRepair.Size = new System.Drawing.Size(770, 80);
            txtObsPostRepair.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(txtObsPostRepair);
            
            y += 140;
            
            Label lblTestResult = new Label();
            lblTestResult.Text = "Test Result *";
            lblTestResult.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblTestResult.Location = new System.Drawing.Point(30, y);
            lblTestResult.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblTestResult);
            
            cmbTestResult = new ComboBox();
            cmbTestResult.Location = new System.Drawing.Point(160, y);
            cmbTestResult.Size = new System.Drawing.Size(120, 30);
            cmbTestResult.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTestResult.Items.AddRange(new string[] { "Pass", "Fail" });
            this.Controls.Add(cmbTestResult);
            
            y += 50;
            
            Label lblRestorationTime = new Label();
            lblRestorationTime.Text = "Restoration Time *";
            lblRestorationTime.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblRestorationTime.Location = new System.Drawing.Point(30, y);
            lblRestorationTime.Size = new System.Drawing.Size(140, 30);
            this.Controls.Add(lblRestorationTime);
            
            dtpRestorationTime = new DateTimePicker();
            dtpRestorationTime.Location = new System.Drawing.Point(180, y);
            dtpRestorationTime.Size = new System.Drawing.Size(200, 30);
            dtpRestorationTime.Format = DateTimePickerFormat.Custom;
            dtpRestorationTime.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            this.Controls.Add(dtpRestorationTime);
            
            y += 50;
            
            Label lblVerifiedBy = new Label();
            lblVerifiedBy.Text = "Verified By";
            lblVerifiedBy.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblVerifiedBy.Location = new System.Drawing.Point(30, y);
            lblVerifiedBy.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblVerifiedBy);
            
            txtVerifiedBy = new TextBox();
            txtVerifiedBy.Location = new System.Drawing.Point(160, y);
            txtVerifiedBy.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(txtVerifiedBy);
            
            y += 50;
            
            Label lblRecordStatus = new Label();
            lblRecordStatus.Text = "Record Status *";
            lblRecordStatus.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblRecordStatus.Location = new System.Drawing.Point(30, y);
            lblRecordStatus.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblRecordStatus);
            
            cmbRecordStatus = new ComboBox();
            cmbRecordStatus.Location = new System.Drawing.Point(160, y);
            cmbRecordStatus.Size = new System.Drawing.Size(180, 30);
            cmbRecordStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRecordStatus.Items.AddRange(new string[] { "Open", "Under Repair", "Restored", "Closed" });
            this.Controls.Add(cmbRecordStatus);
            
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
            btnView.Click += (s, e) => new ViewRecordsForm("Reg040_FailureInspection", "Failure Inspection Records").ShowDialog();
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
        
        private void GenerateFailureID()
        {
            string datePart = DateTime.Now.ToString("yyyyMMdd");
            string query = $"SELECT COUNT(*) FROM Reg040_FailureInspection WHERE FailureID LIKE 'TMS-REG-040-{datePart}-%'";
            int count = Convert.ToInt32(db.ExecuteScalar(query));
            txtFailureID.Text = $"TMS-REG-040-{datePart}-{(count + 1).ToString("D3")}";
        }
        
        
            private void BtnSave_Click(object sender, EventArgs e)
        {
        
        
            if (string.IsNullOrWhiteSpace(txtSubmittedBy.Text) || !int.TryParse(txtSubmittedBy.Text.Trim(), out _))
            {
                MessageBox.Show("Submitted By must be a valid numeric Staff ID.", "Validation Error");
                return;
            }
            if (!ValidationHelper.IsNotEmpty(txtFailureLinkID.Text, "Failure Link ID")) return;
            if (!ValidationHelper.IsNotEmpty(txtAssetID.Text, "Asset ID")) return;
            if (!ValidationHelper.IsSelected(cmbFailureClass, "Failure Class")) return;
            if (!ValidationHelper.IsNotEmpty(txtObsPreRepair.Text, "Pre-Repair Observations")) return;
            if (!ValidationHelper.IsNotEmpty(txtRootCause.Text, "Root Cause")) return;
            if (!ValidationHelper.IsNotEmpty(txtRepairAction.Text, "Repair Action")) return;
            if (!ValidationHelper.IsNotEmpty(txtObsPostRepair.Text, "Post-Repair Observations")) return;
            if (!ValidationHelper.IsSelected(cmbTestResult, "Test Result")) return;
            if (!ValidationHelper.IsEndAfterStart(dtpFailureTime.Value, dtpRestorationTime.Value, "Failure Time", "Restoration Time")) return;
            if (!ValidationHelper.IsSelected(cmbRecordStatus, "Record Status")) return;
            
            string verifiedBy = string.IsNullOrEmpty(txtVerifiedBy.Text) ? "NULL" : $"'{txtVerifiedBy.Text}'";
            
            string query = $@"
                INSERT INTO Reg040_FailureInspection (FailureID, FailureLinkID, AssetID, FailureTime, FailureClass, ObsPreRepair, RootCause, RepairAction, ObsPostRepair, TestResult, RestorationTime, VerifiedBy, RecordStatus, SubmittedBy)
                VALUES ('{txtFailureID.Text}', '{txtFailureLinkID.Text}', '{txtAssetID.Text}', 
                        '{dtpFailureTime.Value:yyyy-MM-dd HH:mm:ss.fff}', '{cmbFailureClass.SelectedItem}', 
                        '{txtObsPreRepair.Text.Replace("'", "''")}', '{txtRootCause.Text.Replace("'", "''")}', 
                        '{txtRepairAction.Text.Replace("'", "''")}', '{txtObsPostRepair.Text.Replace("'", "''")}', 
                        '{(cmbTestResult.SelectedItem.ToString() == "Pass" ? "1" : "0")}', 
                        '{dtpRestorationTime.Value:yyyy-MM-dd HH:mm:ss.fff}', {verifiedBy}, '{cmbRecordStatus.SelectedItem}', {txtSubmittedBy.Text})";

            
            
            try
            {
                db.ExecuteNonQuery(query);
                MessageBox.Show($"Failure Inspection Record Saved!\nFailure ID: {txtFailureID.Text}", "Success");
                ClearForm();
                GenerateFailureID();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error");
            }
        }
        
        
            private void ClearForm()
        {
        
        
            if (txtSubmittedBy != null) txtSubmittedBy.Clear();
            txtFailureLinkID.Clear();
            txtAssetID.Clear();
            dtpFailureTime.Value = DateTime.Now;
            cmbFailureClass.SelectedIndex = -1;
            txtObsPreRepair.Clear();
            txtRootCause.Clear();
            txtRepairAction.Clear();
            txtObsPostRepair.Clear();
            cmbTestResult.SelectedIndex = -1;
            dtpRestorationTime.Value = DateTime.Now.AddHours(2);
            txtVerifiedBy.Clear();
            cmbRecordStatus.SelectedIndex = -1;
        }
    
        protected override void OnHandleCreated(System.EventArgs e) { base.OnHandleCreated(e); TMS.ThemeManager.ApplyTheme(this); }
    }
}
