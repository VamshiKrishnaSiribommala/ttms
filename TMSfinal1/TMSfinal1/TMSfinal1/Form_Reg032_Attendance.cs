using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TMS
{
    public class Form_Reg032_Attendance : Form
    {
        private TextBox txtAttendanceID;
        private TextBox txtStaffID;
        private TextBox txtStaffName;
        private TextBox txtDesignation;
        private ComboBox cmbShiftAssigned;
        private DateTimePicker dtpShiftStartTime;
        private DateTimePicker dtpInTime;
        private DateTimePicker dtpOutTime;
        private NumericUpDown numTotalDutyHours;
        private TextBox txtLateRemark;
        private ComboBox cmbBiometricType;
        private TextBox txtDeviceID;
        private ComboBox cmbAttendanceStatus;
        private TextBox txtSubmittedBy;
        private DatabaseHelper db = new DatabaseHelper();
        

        public Form_Reg032_Attendance()
        {
            this.Text = "Attendance (REG-032)";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.Size = new System.Drawing.Size(850, 800);
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.White;
            
            CreateControls();
            GenerateAttendanceID();
        }

        private void CreateControls()
        {
            Panel headerPanel = new Panel();
            headerPanel.BackColor = System.Drawing.Color.FromArgb(0, 51, 102);
            headerPanel.Size = new System.Drawing.Size(850, 80);
            headerPanel.Dock = DockStyle.Top;
            this.Controls.Add(headerPanel);
            
            Label lblPath = new Label();
            lblPath.Text = "Home > Safety List > Attendance";
            lblPath.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Italic);
            lblPath.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);
            lblPath.Size = new System.Drawing.Size(800, 25);
            lblPath.Location = new System.Drawing.Point(25, 15);
            headerPanel.Controls.Add(lblPath);
            
            Label lblTitle = new Label();
            lblTitle.Text = "ATTENDANCE REGISTER";
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.White;
            lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblTitle.Size = new System.Drawing.Size(800, 35);
            lblTitle.Location = new System.Drawing.Point(25, 40);
            headerPanel.Controls.Add(lblTitle);
            
            int y = 110;
            
            Label lblAttendanceID = new Label();
            lblAttendanceID.Text = "Attendance ID (System Generated):";
            lblAttendanceID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblAttendanceID.Location = new System.Drawing.Point(30, y);
            lblAttendanceID.Size = new System.Drawing.Size(220, 30);
            this.Controls.Add(lblAttendanceID);
            
            txtAttendanceID = new TextBox();
            txtAttendanceID.Location = new System.Drawing.Point(260, y);
            txtAttendanceID.Size = new System.Drawing.Size(300, 30);
            txtAttendanceID.ReadOnly = true;
            txtAttendanceID.BackColor = System.Drawing.Color.LightGray;
            this.Controls.Add(txtAttendanceID);
            
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
            
            Label lblStaffName = new Label();
            lblStaffName.Text = "Staff Name *";
            lblStaffName.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblStaffName.Location = new System.Drawing.Point(30, y);
            lblStaffName.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblStaffName);
            
            txtStaffName = new TextBox();
            txtStaffName.Location = new System.Drawing.Point(160, y);
            txtStaffName.Size = new System.Drawing.Size(250, 30);
            this.Controls.Add(txtStaffName);
            
            y += 50;
            
            Label lblDesignation = new Label();
            lblDesignation.Text = "Designation *";
            lblDesignation.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblDesignation.Location = new System.Drawing.Point(30, y);
            lblDesignation.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblDesignation);
            
            txtDesignation = new TextBox();
            txtDesignation.Location = new System.Drawing.Point(160, y);
            txtDesignation.Size = new System.Drawing.Size(250, 30);
            this.Controls.Add(txtDesignation);
            
            y += 50;
            
            Label lblShiftAssigned = new Label();
            lblShiftAssigned.Text = "Shift Assigned *";
            lblShiftAssigned.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblShiftAssigned.Location = new System.Drawing.Point(30, y);
            lblShiftAssigned.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblShiftAssigned);
            
            cmbShiftAssigned = new ComboBox();
            cmbShiftAssigned.Location = new System.Drawing.Point(170, y);
            cmbShiftAssigned.Size = new System.Drawing.Size(150, 30);
            cmbShiftAssigned.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbShiftAssigned.Items.AddRange(new string[] { "Morning", "Evening", "Night", "Special Duty" });
            this.Controls.Add(cmbShiftAssigned);
            
            y += 50;
            
            Label lblShiftStartTime = new Label();
            lblShiftStartTime.Text = "Shift Start Time *";
            lblShiftStartTime.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblShiftStartTime.Location = new System.Drawing.Point(30, y);
            lblShiftStartTime.Size = new System.Drawing.Size(140, 30);
            this.Controls.Add(lblShiftStartTime);
            
            dtpShiftStartTime = new DateTimePicker();
            dtpShiftStartTime.Location = new System.Drawing.Point(180, y);
            dtpShiftStartTime.Size = new System.Drawing.Size(180, 30);
            dtpShiftStartTime.Format = DateTimePickerFormat.Custom;
            dtpShiftStartTime.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            this.Controls.Add(dtpShiftStartTime);
            
            y += 50;
            
            Label lblInTime = new Label();
            lblInTime.Text = "In Time *";
            lblInTime.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblInTime.Location = new System.Drawing.Point(30, y);
            lblInTime.Size = new System.Drawing.Size(100, 30);
            this.Controls.Add(lblInTime);
            
            dtpInTime = new DateTimePicker();
            dtpInTime.Location = new System.Drawing.Point(140, y);
            dtpInTime.Size = new System.Drawing.Size(180, 30);
            dtpInTime.Format = DateTimePickerFormat.Custom;
            dtpInTime.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            this.Controls.Add(dtpInTime);
            
            y += 50;
            
            Label lblOutTime = new Label();
            lblOutTime.Text = "Out Time";
            lblOutTime.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblOutTime.Location = new System.Drawing.Point(30, y);
            lblOutTime.Size = new System.Drawing.Size(100, 30);
            this.Controls.Add(lblOutTime);
            
            dtpOutTime = new DateTimePicker();
            dtpOutTime.Location = new System.Drawing.Point(140, y);
            dtpOutTime.Size = new System.Drawing.Size(180, 30);
            dtpOutTime.Format = DateTimePickerFormat.Custom;
            dtpOutTime.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            dtpOutTime.ShowCheckBox = true;
            dtpOutTime.Checked = false;
            this.Controls.Add(dtpOutTime);
            
            y += 50;
            
            Label lblTotalDutyHours = new Label();
            lblTotalDutyHours.Text = "Total Duty Hours";
            lblTotalDutyHours.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblTotalDutyHours.Location = new System.Drawing.Point(30, y);
            lblTotalDutyHours.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblTotalDutyHours);
            
            numTotalDutyHours = new NumericUpDown();
            numTotalDutyHours.Location = new System.Drawing.Point(170, y);
            numTotalDutyHours.Size = new System.Drawing.Size(100, 30);
            numTotalDutyHours.DecimalPlaces = 2;
            numTotalDutyHours.Minimum = 0;
            numTotalDutyHours.Maximum = 24;
            this.Controls.Add(numTotalDutyHours);
            
            y += 50;
            
            Label lblLateRemark = new Label();
            lblLateRemark.Text = "Late Remark";
            lblLateRemark.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblLateRemark.Location = new System.Drawing.Point(30, y);
            lblLateRemark.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblLateRemark);
            
            txtLateRemark = new TextBox();
            txtLateRemark.Location = new System.Drawing.Point(160, y);
            txtLateRemark.Size = new System.Drawing.Size(400, 30);
            this.Controls.Add(txtLateRemark);
            
            y += 50;
            
            Label lblBiometricType = new Label();
            lblBiometricType.Text = "Biometric Type *";
            lblBiometricType.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblBiometricType.Location = new System.Drawing.Point(30, y);
            lblBiometricType.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblBiometricType);
            
            cmbBiometricType = new ComboBox();
            cmbBiometricType.Location = new System.Drawing.Point(170, y);
            cmbBiometricType.Size = new System.Drawing.Size(150, 30);
            cmbBiometricType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbBiometricType.Items.AddRange(new string[] { "Fingerprint", "Face ID", "RFID", "Both" });
            this.Controls.Add(cmbBiometricType);
            
            y += 50;
            
            Label lblDeviceID = new Label();
            lblDeviceID.Text = "Device ID *";
            lblDeviceID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblDeviceID.Location = new System.Drawing.Point(30, y);
            lblDeviceID.Size = new System.Drawing.Size(100, 30);
            this.Controls.Add(lblDeviceID);
            
            txtDeviceID = new TextBox();
            txtDeviceID.Location = new System.Drawing.Point(140, y);
            txtDeviceID.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(txtDeviceID);
            
            y += 50;
            
            Label lblAttendanceStatus = new Label();
            lblAttendanceStatus.Text = "Attendance Status *";
            lblAttendanceStatus.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblAttendanceStatus.Location = new System.Drawing.Point(30, y);
            lblAttendanceStatus.Size = new System.Drawing.Size(140, 30);
            this.Controls.Add(lblAttendanceStatus);
            
            cmbAttendanceStatus = new ComboBox();
            cmbAttendanceStatus.Location = new System.Drawing.Point(180, y);
            cmbAttendanceStatus.Size = new System.Drawing.Size(150, 30);
            cmbAttendanceStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbAttendanceStatus.Items.AddRange(new string[] { "Present", "Late", "Absent", "Early Out" });
            this.Controls.Add(cmbAttendanceStatus);
            
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
            btnView.Click += (s, e) => new ViewRecordsForm("Reg032_Attendance", "Attendance Records").ShowDialog();
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
        
        private void GenerateAttendanceID()
        {
            string datePart = DateTime.Now.ToString("yyyyMMdd");
            string query = $"SELECT COUNT(*) FROM Reg032_Attendance WHERE AttendanceID LIKE 'TMS-REG-032-{datePart}-%'";
            int count = Convert.ToInt32(db.ExecuteScalar(query));
            txtAttendanceID.Text = $"TMS-REG-032-{datePart}-{(count + 1).ToString("D3")}";
        }
        
        
            private void BtnSave_Click(object sender, EventArgs e)
        {
        
        
            if (string.IsNullOrWhiteSpace(txtSubmittedBy.Text) || !int.TryParse(txtSubmittedBy.Text.Trim(), out _))
            {
                MessageBox.Show("Submitted By must be a valid numeric Staff ID.", "Validation Error");
                return;
            }
            if (!ValidationHelper.IsNotEmpty(txtStaffID.Text, "Staff ID")) return;
            if (!ValidationHelper.IsNotEmpty(txtStaffName.Text, "Staff Name")) return;
            if (!ValidationHelper.IsNotEmpty(txtDesignation.Text, "Designation")) return;
            if (!ValidationHelper.IsSelected(cmbShiftAssigned, "Shift Assigned")) return;
            if (!ValidationHelper.IsSelected(cmbBiometricType, "Biometric Type")) return;
            if (!ValidationHelper.IsNotEmpty(txtDeviceID.Text, "Device ID")) return;
            if (!ValidationHelper.IsSelected(cmbAttendanceStatus, "Attendance Status")) return;
            
            string outTime = dtpOutTime.Checked ? $"'{dtpOutTime.Value:yyyy-MM-dd HH:mm:ss.fff}'" : "NULL";
            
            string query = $@"
                INSERT INTO Reg032_Attendance (AttendanceID, StaffID, StaffName, Designation, ShiftAssigned, ShiftStartTime, InTime, OutTime, TotalDutyHours, LateRemark, BiometricType, DeviceID, AttendanceStatus, SubmittedBy)
                VALUES ('{txtAttendanceID.Text}', '{txtStaffID.Text}', '{txtStaffName.Text}', '{txtDesignation.Text}', 
                        '{cmbShiftAssigned.SelectedItem}', '{dtpShiftStartTime.Value:yyyy-MM-dd HH:mm:ss.fff}', 
                        '{dtpInTime.Value:yyyy-MM-dd HH:mm:ss.fff}', {outTime}, {numTotalDutyHours.Value}, 
                        '{txtLateRemark.Text}', '{cmbBiometricType.SelectedItem}', '{txtDeviceID.Text}', 
                        '{cmbAttendanceStatus.SelectedItem}', {txtSubmittedBy.Text})";

            
            
            try
            {
                db.ExecuteNonQuery(query);
                MessageBox.Show($"Attendance Record Saved!\nAttendance ID: {txtAttendanceID.Text}", "Success");
                ClearForm();
                GenerateAttendanceID();
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
            txtStaffName.Clear();
            txtDesignation.Clear();
            cmbShiftAssigned.SelectedIndex = -1;
            dtpShiftStartTime.Value = DateTime.Now;
            dtpInTime.Value = DateTime.Now;
            dtpOutTime.Checked = false;
            numTotalDutyHours.Value = 0;
            txtLateRemark.Clear();
            cmbBiometricType.SelectedIndex = -1;
            txtDeviceID.Clear();
            cmbAttendanceStatus.SelectedIndex = -1;
        }
    
        protected override void OnHandleCreated(System.EventArgs e) { base.OnHandleCreated(e); TMS.ThemeManager.ApplyTheme(this); }
    }
}
