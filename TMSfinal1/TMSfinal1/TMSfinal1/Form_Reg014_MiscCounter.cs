using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TMS
{
    public class Form_Reg014_MiscCounter : Form
    {
        private TextBox txtCounterLogID;
        private ComboBox cmbCounterType;
        private TextBox txtAssetID;
        private NumericUpDown numOldCounterValue;
        private NumericUpDown numNewCounterValue;
        private DateTimePicker dtpOperationTime;
        private TextBox txtInitiatorID;
        private TextBox txtAuthorizationRef;
        private ComboBox cmbReasonCode;
        private TextBox txtSubmittedBy;
        private DatabaseHelper db = new DatabaseHelper();
        

        public Form_Reg014_MiscCounter()
        {
            this.Text = "Miscellaneous Counter (REG-014)";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.Size = new System.Drawing.Size(800, 750);
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.White;
            
            CreateControls();
            GenerateCounterLogID();
        }

        private void CreateControls()
        {
            Panel headerPanel = new Panel();
            headerPanel.BackColor = System.Drawing.Color.FromArgb(0, 51, 102);
            headerPanel.Size = new System.Drawing.Size(800, 80);
            headerPanel.Dock = DockStyle.Top;
            this.Controls.Add(headerPanel);
            
            Label lblPath = new Label();
            lblPath.Text = "Home > Operational List > Miscellaneous Counter";
            lblPath.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Italic);
            lblPath.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);
            lblPath.Size = new System.Drawing.Size(750, 25);
            lblPath.Location = new System.Drawing.Point(25, 15);
            headerPanel.Controls.Add(lblPath);
            
            Label lblTitle = new Label();
            lblTitle.Text = "MISCELLANEOUS COUNTER REGISTER";
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.White;
            lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblTitle.Size = new System.Drawing.Size(750, 35);
            lblTitle.Location = new System.Drawing.Point(25, 40);
            headerPanel.Controls.Add(lblTitle);
            
            int y = 110;
            
            Label lblCounterLogID = new Label();
            lblCounterLogID.Text = "Counter Log ID (System Generated):";
            lblCounterLogID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblCounterLogID.Location = new System.Drawing.Point(30, y);
            lblCounterLogID.Size = new System.Drawing.Size(220, 30);
            this.Controls.Add(lblCounterLogID);
            
            txtCounterLogID = new TextBox();
            txtCounterLogID.Location = new System.Drawing.Point(260, y);
            txtCounterLogID.Size = new System.Drawing.Size(300, 30);
            txtCounterLogID.ReadOnly = true;
            txtCounterLogID.BackColor = System.Drawing.Color.LightGray;
            this.Controls.Add(txtCounterLogID);
            
            y += 50;
            
            Label lblCounterType = new Label();
            lblCounterType.Text = "Counter Type *";
            lblCounterType.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblCounterType.Location = new System.Drawing.Point(30, y);
            lblCounterType.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblCounterType);
            
            cmbCounterType = new ComboBox();
            cmbCounterType.Location = new System.Drawing.Point(160, y);
            cmbCounterType.Size = new System.Drawing.Size(200, 30);
            cmbCounterType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCounterType.Items.AddRange(new string[] { "Point", "Route", "Axle Counter", "EUUYN", "Others" });
            this.Controls.Add(cmbCounterType);
            
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
            
            Label lblOldCounterValue = new Label();
            lblOldCounterValue.Text = "Old Counter Value *";
            lblOldCounterValue.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblOldCounterValue.Location = new System.Drawing.Point(30, y);
            lblOldCounterValue.Size = new System.Drawing.Size(150, 30);
            this.Controls.Add(lblOldCounterValue);
            
            numOldCounterValue = new NumericUpDown();
            numOldCounterValue.Location = new System.Drawing.Point(190, y);
            numOldCounterValue.Size = new System.Drawing.Size(120, 30);
            numOldCounterValue.Minimum = 0;
            numOldCounterValue.Maximum = 999999;
            this.Controls.Add(numOldCounterValue);
            
            y += 50;
            
            Label lblNewCounterValue = new Label();
            lblNewCounterValue.Text = "New Counter Value *";
            lblNewCounterValue.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblNewCounterValue.Location = new System.Drawing.Point(30, y);
            lblNewCounterValue.Size = new System.Drawing.Size(150, 30);
            this.Controls.Add(lblNewCounterValue);
            
            numNewCounterValue = new NumericUpDown();
            numNewCounterValue.Location = new System.Drawing.Point(190, y);
            numNewCounterValue.Size = new System.Drawing.Size(120, 30);
            numNewCounterValue.Minimum = 0;
            numNewCounterValue.Maximum = 999999;
            this.Controls.Add(numNewCounterValue);
            
            y += 50;
            
            Label lblOperationTime = new Label();
            lblOperationTime.Text = "Operation Time *";
            lblOperationTime.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblOperationTime.Location = new System.Drawing.Point(30, y);
            lblOperationTime.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblOperationTime);
            
            dtpOperationTime = new DateTimePicker();
            dtpOperationTime.Location = new System.Drawing.Point(160, y);
            dtpOperationTime.Size = new System.Drawing.Size(200, 30);
            dtpOperationTime.Format = DateTimePickerFormat.Custom;
            dtpOperationTime.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            this.Controls.Add(dtpOperationTime);
            
            y += 50;
            
            Label lblInitiatorID = new Label();
            lblInitiatorID.Text = "Initiator ID *";
            lblInitiatorID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblInitiatorID.Location = new System.Drawing.Point(30, y);
            lblInitiatorID.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblInitiatorID);
            
            txtInitiatorID = new TextBox();
            txtInitiatorID.Location = new System.Drawing.Point(160, y);
            txtInitiatorID.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(txtInitiatorID);
            
            y += 50;
            
            Label lblAuthorizationRef = new Label();
            lblAuthorizationRef.Text = "Authorization Ref *";
            lblAuthorizationRef.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblAuthorizationRef.Location = new System.Drawing.Point(30, y);
            lblAuthorizationRef.Size = new System.Drawing.Size(150, 30);
            this.Controls.Add(lblAuthorizationRef);
            
            txtAuthorizationRef = new TextBox();
            txtAuthorizationRef.Location = new System.Drawing.Point(190, y);
            txtAuthorizationRef.Size = new System.Drawing.Size(300, 30);
            this.Controls.Add(txtAuthorizationRef);
            
            y += 50;
            
            Label lblReasonCode = new Label();
            lblReasonCode.Text = "Reason Code *";
            lblReasonCode.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblReasonCode.Location = new System.Drawing.Point(30, y);
            lblReasonCode.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblReasonCode);
            
            cmbReasonCode = new ComboBox();
            cmbReasonCode.Location = new System.Drawing.Point(160, y);
            cmbReasonCode.Size = new System.Drawing.Size(200, 30);
            cmbReasonCode.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbReasonCode.Items.AddRange(new string[] { "Maintenance", "IR Failure", "Emergency Override", "Calibration", "Testing" });
            this.Controls.Add(cmbReasonCode);
            
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
            btnView.Click += (s, e) => new ViewRecordsForm("Reg014_MiscCounter", "Counter Records").ShowDialog();
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
        
        private void GenerateCounterLogID()
        {
            string datePart = DateTime.Now.ToString("yyyyMMdd");
            string query = $"SELECT COUNT(*) FROM Reg014_MiscCounter WHERE CounterLogID LIKE 'TMS-REG-014-{datePart}-%'";
            int count = Convert.ToInt32(db.ExecuteScalar(query));
            txtCounterLogID.Text = $"TMS-REG-014-{datePart}-{(count + 1).ToString("D3")}";
        }
        
        
            private void BtnSave_Click(object sender, EventArgs e)
        {
        
        
            if (string.IsNullOrWhiteSpace(txtSubmittedBy.Text) || !int.TryParse(txtSubmittedBy.Text.Trim(), out _))
            {
                MessageBox.Show("Submitted By must be a valid numeric Staff ID.", "Validation Error");
                return;
            }
            if (!ValidationHelper.IsSelected(cmbCounterType, "Counter Type")) return;
            if (!ValidationHelper.IsNotEmpty(txtAssetID.Text, "Asset ID")) return;
            if (numNewCounterValue.Value <= numOldCounterValue.Value)
            {
                MessageBox.Show("New Counter Value must be greater than Old Counter Value!", "Validation Error");
                return;
            }
            if (!ValidationHelper.IsNotEmpty(txtInitiatorID.Text, "Initiator ID")) return;
            if (!ValidationHelper.IsNotEmpty(txtAuthorizationRef.Text, "Authorization Ref")) return;
            if (!ValidationHelper.IsSelected(cmbReasonCode, "Reason Code")) return;
            
            string query = $@"
                INSERT INTO Reg014_MiscCounter (CounterLogID, CounterType, AssetID, OldCounterValue, NewCounterValue, OperationTime, InitiatorID, AuthorizationRef, ReasonCode, AuditFlag, SubmittedBy)
                VALUES ('{txtCounterLogID.Text}', '{cmbCounterType.SelectedItem}', '{txtAssetID.Text}', 
                        {numOldCounterValue.Value}, {numNewCounterValue.Value}, '{dtpOperationTime.Value:yyyy-MM-dd HH:mm:ss.fff}', 
                        '{txtInitiatorID.Text}', '{txtAuthorizationRef.Text}', '{cmbReasonCode.SelectedItem}', 1, {txtSubmittedBy.Text})";

            
            
            try
            {
                db.ExecuteNonQuery(query);
                MessageBox.Show($"Counter Record Saved!\nCounter Log ID: {txtCounterLogID.Text}", "Success");
                ClearForm();
                GenerateCounterLogID();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error");
            }
        }
        
        
            private void ClearForm()
        {
        
        
            if (txtSubmittedBy != null) txtSubmittedBy.Clear();
            cmbCounterType.SelectedIndex = -1;
            txtAssetID.Clear();
            numOldCounterValue.Value = 0;
            numNewCounterValue.Value = 0;
            dtpOperationTime.Value = DateTime.Now;
            txtInitiatorID.Clear();
            txtAuthorizationRef.Clear();
            cmbReasonCode.SelectedIndex = -1;
        }
    
        protected override void OnHandleCreated(System.EventArgs e) { base.OnHandleCreated(e); TMS.ThemeManager.ApplyTheme(this); }
    }
}
