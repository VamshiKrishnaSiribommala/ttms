using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TMS
{
    public class Form_Reg019_SignalFailure : Form
    {
        private TextBox txtFailureID;
        private TextBox txtSignalNumber;
        private DateTimePicker dtpFailureTime;
        private ComboBox cmbFailureType;
        private TextBox txtReportingStaff;
        private DateTimePicker dtpRestorationTime;
        private NumericUpDown numMTTR;
        private ComboBox cmbRootCause;
        private RichTextBox txtActionTaken;
        private TextBox txtSubmittedBy;
        private DatabaseHelper db = new DatabaseHelper();
        

        public Form_Reg019_SignalFailure()
        {
            this.Text = "Signal Failure (REG-019)";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.Size = new System.Drawing.Size(800, 750);
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.White;
            
            CreateControls();
            GenerateFailureID();
        }

        private void CreateControls()
        {
            Panel headerPanel = new Panel();
            headerPanel.BackColor = System.Drawing.Color.FromArgb(0, 51, 102);
            headerPanel.Size = new System.Drawing.Size(800, 80);
            headerPanel.Dock = DockStyle.Top;
            this.Controls.Add(headerPanel);
            
            Label lblPath = new Label();
            lblPath.Text = "Home > Maintenance Sub > Signal Failure";
            lblPath.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Italic);
            lblPath.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);
            lblPath.Size = new System.Drawing.Size(750, 25);
            lblPath.Location = new System.Drawing.Point(25, 15);
            headerPanel.Controls.Add(lblPath);
            
            Label lblTitle = new Label();
            lblTitle.Text = "SIGNAL FAILURE REGISTER";
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.White;
            lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblTitle.Size = new System.Drawing.Size(750, 35);
            lblTitle.Location = new System.Drawing.Point(25, 40);
            headerPanel.Controls.Add(lblTitle);
            
            int y = 110;
            
            Label lblFailureID = new Label();
            lblFailureID.Text = "Failure ID (System Generated):";
            lblFailureID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblFailureID.Location = new System.Drawing.Point(30, y);
            lblFailureID.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(lblFailureID);
            
            txtFailureID = new TextBox();
            txtFailureID.Location = new System.Drawing.Point(240, y);
            txtFailureID.Size = new System.Drawing.Size(300, 30);
            txtFailureID.ReadOnly = true;
            txtFailureID.BackColor = System.Drawing.Color.LightGray;
            this.Controls.Add(txtFailureID);
            
            y += 50;
            
            Label lblSignalNumber = new Label();
            lblSignalNumber.Text = "Signal Number *";
            lblSignalNumber.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblSignalNumber.Location = new System.Drawing.Point(30, y);
            lblSignalNumber.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblSignalNumber);
            
            txtSignalNumber = new TextBox();
            txtSignalNumber.Location = new System.Drawing.Point(160, y);
            txtSignalNumber.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(txtSignalNumber);
            
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
            this.Controls.Add(dtpFailureTime);
            
            y += 50;
            
            Label lblFailureType = new Label();
            lblFailureType.Text = "Failure Type *";
            lblFailureType.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblFailureType.Location = new System.Drawing.Point(30, y);
            lblFailureType.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblFailureType);
            
            cmbFailureType = new ComboBox();
            cmbFailureType.Location = new System.Drawing.Point(160, y);
            cmbFailureType.Size = new System.Drawing.Size(200, 30);
            cmbFailureType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFailureType.Items.AddRange(new string[] { "Bulb", "Cable", "Power", "Relay", "Others" });
            this.Controls.Add(cmbFailureType);
            
            y += 50;
            
            Label lblReportingStaff = new Label();
            lblReportingStaff.Text = "Reporting Staff *";
            lblReportingStaff.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblReportingStaff.Location = new System.Drawing.Point(30, y);
            lblReportingStaff.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblReportingStaff);
            
            txtReportingStaff = new TextBox();
            txtReportingStaff.Location = new System.Drawing.Point(170, y);
            txtReportingStaff.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(txtReportingStaff);
            
            y += 50;
            
            Label lblRestorationTime = new Label();
            lblRestorationTime.Text = "Restoration Time";
            lblRestorationTime.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblRestorationTime.Location = new System.Drawing.Point(30, y);
            lblRestorationTime.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblRestorationTime);
            
            dtpRestorationTime = new DateTimePicker();
            dtpRestorationTime.Location = new System.Drawing.Point(170, y);
            dtpRestorationTime.Size = new System.Drawing.Size(200, 30);
            dtpRestorationTime.Format = DateTimePickerFormat.Custom;
            dtpRestorationTime.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            dtpRestorationTime.ShowCheckBox = true;
            dtpRestorationTime.Checked = false;
            this.Controls.Add(dtpRestorationTime);
            
            y += 50;
            
            Label lblMTTR = new Label();
            lblMTTR.Text = "MTTR (Minutes";
            lblMTTR.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblMTTR.Location = new System.Drawing.Point(30, y);
            lblMTTR.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblMTTR);
            
            numMTTR = new NumericUpDown();
            numMTTR.Location = new System.Drawing.Point(170, y);
            numMTTR.Size = new System.Drawing.Size(100, 30);
            numMTTR.Minimum = 0;
            numMTTR.Maximum = 9999;
            this.Controls.Add(numMTTR);
            
            y += 50;
            
            Label lblRootCause = new Label();
            lblRootCause.Text = "Root Cause *";
            lblRootCause.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblRootCause.Location = new System.Drawing.Point(30, y);
            lblRootCause.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblRootCause);
            
            cmbRootCause = new ComboBox();
            cmbRootCause.Location = new System.Drawing.Point(160, y);
            cmbRootCause.Size = new System.Drawing.Size(200, 30);
            cmbRootCause.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRootCause.Items.AddRange(new string[] { "Power Surge", "Bulb Fuse", "Cable Cut", "Relay Failure", "Water Ingress", "Others" });
            this.Controls.Add(cmbRootCause);
            
            y += 80;
            
            Label lblActionTaken = new Label();
            lblActionTaken.Text = "Action Taken *";
            lblActionTaken.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblActionTaken.Location = new System.Drawing.Point(30, y);
            lblActionTaken.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblActionTaken);
            
            txtActionTaken = new RichTextBox();
            txtActionTaken.Location = new System.Drawing.Point(30, y + 40);
            txtActionTaken.Size = new System.Drawing.Size(720, 100);
            txtActionTaken.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(txtActionTaken);
            
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
            btnView.Click += (s, e) => new ViewRecordsForm("Reg019_SignalFailure", "Signal Failure Records").ShowDialog();
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
            string query = $"SELECT COUNT(*) FROM Reg019_SignalFailure WHERE FailureID LIKE 'TMS-REG-019-{datePart}-%'";
            int count = Convert.ToInt32(db.ExecuteScalar(query));
            txtFailureID.Text = $"TMS-REG-019-{datePart}-{(count + 1).ToString("D3")}";
        }
        
        
            private void BtnSave_Click(object sender, EventArgs e)
        {
        
        
            if (string.IsNullOrWhiteSpace(txtSubmittedBy.Text) || !int.TryParse(txtSubmittedBy.Text.Trim(), out _))
            {
                MessageBox.Show("Submitted By must be a valid numeric Staff ID.", "Validation Error");
                return;
            }
            if (!ValidationHelper.IsNotEmpty(txtSignalNumber.Text, "Signal Number")) return;
            if (!ValidationHelper.IsSelected(cmbFailureType, "Failure Type")) return;
            if (!ValidationHelper.IsNotEmpty(txtReportingStaff.Text, "Reporting Staff")) return;
            if (!ValidationHelper.IsSelected(cmbRootCause, "Root Cause")) return;
            if (!ValidationHelper.IsNotEmpty(txtActionTaken.Text, "Action Taken")) return;
            
            string restorationTime = dtpRestorationTime.Checked ? $"'{dtpRestorationTime.Value:yyyy-MM-dd HH:mm:ss.fff}'" : "NULL";
            
            string query = $@"
                INSERT INTO Reg019_SignalFailure (FailureID, SignalNumber, FailureTime, FailureType, ReportingStaff, RestorationTime, MTTR, RootCause, ActionTaken, SubmittedBy)
                VALUES ('{txtFailureID.Text}', '{txtSignalNumber.Text}', '{dtpFailureTime.Value:yyyy-MM-dd HH:mm:ss.fff}', 
                        '{cmbFailureType.SelectedItem}', '{txtReportingStaff.Text}', {restorationTime}, 
                        {numMTTR.Value}, '{cmbRootCause.SelectedItem}', '{txtActionTaken.Text.Replace("'", "''")}', {txtSubmittedBy.Text})";

            
            
            try
            {
                db.ExecuteNonQuery(query);
                MessageBox.Show($"Signal Failure Record Saved!\nFailure ID: {txtFailureID.Text}", "Success");
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
            txtSignalNumber.Clear();
            dtpFailureTime.Value = DateTime.Now;
            cmbFailureType.SelectedIndex = -1;
            txtReportingStaff.Clear();
            dtpRestorationTime.Checked = false;
            numMTTR.Value = 0;
            cmbRootCause.SelectedIndex = -1;
            txtActionTaken.Clear();
        }
    
        protected override void OnHandleCreated(System.EventArgs e) { base.OnHandleCreated(e); TMS.ThemeManager.ApplyTheme(this); }
    }
}
