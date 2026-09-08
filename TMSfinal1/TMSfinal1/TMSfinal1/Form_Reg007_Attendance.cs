using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TMS
{
    public class Form_Reg007_Attendance : Form
    {
        private TextBox txtAttendanceID;
        private TextBox txtStaffID;
        private ComboBox cmbShiftType;
        private DateTimePicker dtpPunchIn;
        private DateTimePicker dtpPunchOut;
        private ComboBox cmbStatus;
        private TextBox txtSubmittedBy;
        private DatabaseHelper db = new DatabaseHelper();
        

        public Form_Reg007_Attendance()
        {
            this.Text = "Bio-Metric Attendance (REG-007)";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.Size = new System.Drawing.Size(750, 600);
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.White;
            
            CreateControls();
            GenerateAttendanceID();
        }

        private void CreateControls()
        {
            Panel headerPanel = new Panel();
            headerPanel.BackColor = System.Drawing.Color.FromArgb(0, 51, 102);
            headerPanel.Size = new System.Drawing.Size(750, 80);
            headerPanel.Dock = DockStyle.Top;
            this.Controls.Add(headerPanel);
            
            Label lblPath = new Label();
            lblPath.Text = "Home > Operational List > Bio-Metric Attendance";
            lblPath.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Italic);
            lblPath.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);
            lblPath.Size = new System.Drawing.Size(700, 25);
            lblPath.Location = new System.Drawing.Point(25, 15);
            headerPanel.Controls.Add(lblPath);
            
            Label lblTitle = new Label();
            lblTitle.Text = "BIO-METRIC ATTENDANCE REGISTER";
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.White;
            lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblTitle.Size = new System.Drawing.Size(700, 35);
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
            
            Label lblShiftType = new Label();
            lblShiftType.Text = "Shift Type *";
            lblShiftType.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblShiftType.Location = new System.Drawing.Point(310, y);
            lblShiftType.Size = new System.Drawing.Size(100, 30);
            this.Controls.Add(lblShiftType);
            
            cmbShiftType = new ComboBox();
            cmbShiftType.Location = new System.Drawing.Point(420, y);
            cmbShiftType.Size = new System.Drawing.Size(120, 30);
            cmbShiftType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbShiftType.Items.AddRange(new string[] { "Day", "Evening", "Night" });
            this.Controls.Add(cmbShiftType);
            
            y += 50;
            
            Label lblPunchIn = new Label();
            lblPunchIn.Text = "Punch In Time *";
            lblPunchIn.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblPunchIn.Location = new System.Drawing.Point(30, y);
            lblPunchIn.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblPunchIn);
            
            dtpPunchIn = new DateTimePicker();
            dtpPunchIn.Location = new System.Drawing.Point(160, y);
            dtpPunchIn.Size = new System.Drawing.Size(200, 30);
            dtpPunchIn.Format = DateTimePickerFormat.Custom;
            dtpPunchIn.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            this.Controls.Add(dtpPunchIn);
            
            y += 50;
            
            Label lblPunchOut = new Label();
            lblPunchOut.Text = "Punch Out Time";
            lblPunchOut.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblPunchOut.Location = new System.Drawing.Point(30, y);
            lblPunchOut.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblPunchOut);
            
            dtpPunchOut = new DateTimePicker();
            dtpPunchOut.Location = new System.Drawing.Point(160, y);
            dtpPunchOut.Size = new System.Drawing.Size(200, 30);
            dtpPunchOut.Format = DateTimePickerFormat.Custom;
            dtpPunchOut.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            dtpPunchOut.ShowCheckBox = true;
            dtpPunchOut.Checked = false;
            this.Controls.Add(dtpPunchOut);
            
            y += 50;
            
            Label lblStatus = new Label();
            lblStatus.Text = "Status *";
            lblStatus.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblStatus.Location = new System.Drawing.Point(30, y);
            lblStatus.Size = new System.Drawing.Size(100, 30);
            this.Controls.Add(lblStatus);
            
            cmbStatus = new ComboBox();
            cmbStatus.Location = new System.Drawing.Point(140, y);
            cmbStatus.Size = new System.Drawing.Size(150, 30);
            cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStatus.Items.AddRange(new string[] { "Present", "Late", "Absent", "Early Out" });
            this.Controls.Add(cmbStatus);
            
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
            btnView.Click += (s, e) => new ViewRecordsForm("Reg007_Attendance", "Attendance Records").ShowDialog();
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
            string query = $"SELECT COUNT(*) FROM Reg007_Attendance WHERE AttendanceID LIKE 'TMS-REG-007-{datePart}-%'";
            int count = Convert.ToInt32(db.ExecuteScalar(query));
            txtAttendanceID.Text = $"TMS-REG-007-{datePart}-{(count + 1).ToString("D3")}";
        }
        
        
            private void BtnSave_Click(object sender, EventArgs e)
        {
        
        
            if (string.IsNullOrWhiteSpace(txtSubmittedBy.Text) || !int.TryParse(txtSubmittedBy.Text.Trim(), out _))
            {
                MessageBox.Show("Submitted By must be a valid numeric Staff ID.", "Validation Error");
                return;
            }
            if (!ValidationHelper.IsNotEmpty(txtStaffID.Text, "Staff ID")) return;
            if (!ValidationHelper.IsSelected(cmbShiftType, "Shift Type")) return;
            if (!ValidationHelper.IsSelected(cmbStatus, "Status")) return;
            
            string punchOut = dtpPunchOut.Checked ? $"'{dtpPunchOut.Value:yyyy-MM-dd HH:mm:ss.fff}'" : "NULL";
            
            string query = $@"
                INSERT INTO Reg007_Attendance (AttendanceID, StaffID, ShiftType, PunchInTime, PunchOutTime, Status, SubmittedBy)
                VALUES ('{txtAttendanceID.Text}', '{txtStaffID.Text}', '{cmbShiftType.SelectedItem}', 
                        '{dtpPunchIn.Value:yyyy-MM-dd HH:mm:ss.fff}', {punchOut}, '{cmbStatus.SelectedItem}', {txtSubmittedBy.Text})";

            
            
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
            cmbShiftType.SelectedIndex = -1;
            dtpPunchIn.Value = DateTime.Now;
            dtpPunchOut.Checked = false;
            dtpPunchOut.Value = DateTime.Now;
            cmbStatus.SelectedIndex = -1;
        }
    
        protected override void OnHandleCreated(System.EventArgs e) { base.OnHandleCreated(e); TMS.ThemeManager.ApplyTheme(this); }
    }
}
