using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TMS
{
    public class Form_Reg039_NightInspection : Form
    {
        private TextBox txtInspectionID;
        private DateTimePicker dtpInspectionDate;
        private DateTimePicker dtpArrivalTime;
        private RichTextBox txtSignalIDs;
        private RichTextBox txtVisibilityStatus;
        private ComboBox cmbStaffPresence;
        private RichTextBox txtEquipmentStatus;
        private RichTextBox txtCorrectiveAction;
        private DateTimePicker dtpDepartureTime;
        private ComboBox cmbStatus;
        private TextBox txtSubmittedBy;
        private DatabaseHelper db = new DatabaseHelper();
        

        public Form_Reg039_NightInspection()
        {
            this.Text = "Night Inspection (REG-039)";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.Size = new System.Drawing.Size(850, 850);
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.White;
            
            CreateControls();
            GenerateInspectionID();
        }

        private void CreateControls()
        {
            Panel headerPanel = new Panel();
            headerPanel.BackColor = System.Drawing.Color.FromArgb(0, 51, 102);
            headerPanel.Size = new System.Drawing.Size(850, 80);
            headerPanel.Dock = DockStyle.Top;
            this.Controls.Add(headerPanel);
            
            Label lblPath = new Label();
            lblPath.Text = "Home > Safety List > Night Inspection";
            lblPath.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Italic);
            lblPath.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);
            lblPath.Size = new System.Drawing.Size(800, 25);
            lblPath.Location = new System.Drawing.Point(25, 15);
            headerPanel.Controls.Add(lblPath);
            
            Label lblTitle = new Label();
            lblTitle.Text = "NIGHT INSPECTION REGISTER";
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.White;
            lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblTitle.Size = new System.Drawing.Size(800, 35);
            lblTitle.Location = new System.Drawing.Point(25, 40);
            headerPanel.Controls.Add(lblTitle);
            
            int y = 110;
            
            Label lblInspectionID = new Label();
            lblInspectionID.Text = "Inspection ID (System Generated):";
            lblInspectionID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblInspectionID.Location = new System.Drawing.Point(30, y);
            lblInspectionID.Size = new System.Drawing.Size(220, 30);
            this.Controls.Add(lblInspectionID);
            
            txtInspectionID = new TextBox();
            txtInspectionID.Location = new System.Drawing.Point(260, y);
            txtInspectionID.Size = new System.Drawing.Size(300, 30);
            txtInspectionID.ReadOnly = true;
            txtInspectionID.BackColor = System.Drawing.Color.LightGray;
            this.Controls.Add(txtInspectionID);
            
            y += 50;
            
            Label lblInspectionDate = new Label();
            lblInspectionDate.Text = "Inspection Date *";
            lblInspectionDate.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblInspectionDate.Location = new System.Drawing.Point(30, y);
            lblInspectionDate.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblInspectionDate);
            
            dtpInspectionDate = new DateTimePicker();
            dtpInspectionDate.Location = new System.Drawing.Point(170, y);
            dtpInspectionDate.Size = new System.Drawing.Size(180, 30);
            dtpInspectionDate.Format = DateTimePickerFormat.Short;
            dtpInspectionDate.MaxDate = DateTime.Today;
            this.Controls.Add(dtpInspectionDate);
            
            y += 50;
            
            Label lblArrivalTime = new Label();
            lblArrivalTime.Text = "Arrival Time *";
            lblArrivalTime.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblArrivalTime.Location = new System.Drawing.Point(30, y);
            lblArrivalTime.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblArrivalTime);
            
            dtpArrivalTime = new DateTimePicker();
            dtpArrivalTime.Location = new System.Drawing.Point(160, y);
            dtpArrivalTime.Size = new System.Drawing.Size(180, 30);
            dtpArrivalTime.Format = DateTimePickerFormat.Custom;
            dtpArrivalTime.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            this.Controls.Add(dtpArrivalTime);
            
            y += 80;
            
            Label lblSignalIDs = new Label();
            lblSignalIDs.Text = "Signal ID(s) *";
            lblSignalIDs.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblSignalIDs.Location = new System.Drawing.Point(30, y);
            lblSignalIDs.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblSignalIDs);
            
            txtSignalIDs = new RichTextBox();
            txtSignalIDs.Location = new System.Drawing.Point(30, y + 40);
            txtSignalIDs.Size = new System.Drawing.Size(770, 80);
            txtSignalIDs.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(txtSignalIDs);
            
            y += 140;
            
            Label lblVisibilityStatus = new Label();
            lblVisibilityStatus.Text = "Visibility Status *";
            lblVisibilityStatus.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblVisibilityStatus.Location = new System.Drawing.Point(30, y);
            lblVisibilityStatus.Size = new System.Drawing.Size(140, 30);
            this.Controls.Add(lblVisibilityStatus);
            
            txtVisibilityStatus = new RichTextBox();
            txtVisibilityStatus.Location = new System.Drawing.Point(30, y + 40);
            txtVisibilityStatus.Size = new System.Drawing.Size(770, 80);
            txtVisibilityStatus.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(txtVisibilityStatus);
            
            y += 140;
            
            Label lblStaffPresence = new Label();
            lblStaffPresence.Text = "Staff Presence *";
            lblStaffPresence.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblStaffPresence.Location = new System.Drawing.Point(30, y);
            lblStaffPresence.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblStaffPresence);
            
            cmbStaffPresence = new ComboBox();
            cmbStaffPresence.Location = new System.Drawing.Point(170, y);
            cmbStaffPresence.Size = new System.Drawing.Size(200, 30);
            cmbStaffPresence.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStaffPresence.Items.AddRange(new string[] { "All Present", "Short", "Unauthorized Absent" });
            this.Controls.Add(cmbStaffPresence);
            
            y += 80;
            
            Label lblEquipmentStatus = new Label();
            lblEquipmentStatus.Text = "Equipment Status *";
            lblEquipmentStatus.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblEquipmentStatus.Location = new System.Drawing.Point(30, y);
            lblEquipmentStatus.Size = new System.Drawing.Size(140, 30);
            this.Controls.Add(lblEquipmentStatus);
            
            txtEquipmentStatus = new RichTextBox();
            txtEquipmentStatus.Location = new System.Drawing.Point(30, y + 40);
            txtEquipmentStatus.Size = new System.Drawing.Size(770, 80);
            txtEquipmentStatus.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(txtEquipmentStatus);
            
            y += 140;
            
            Label lblCorrectiveAction = new Label();
            lblCorrectiveAction.Text = "Corrective Action";
            lblCorrectiveAction.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblCorrectiveAction.Location = new System.Drawing.Point(30, y);
            lblCorrectiveAction.Size = new System.Drawing.Size(140, 30);
            this.Controls.Add(lblCorrectiveAction);
            
            txtCorrectiveAction = new RichTextBox();
            txtCorrectiveAction.Location = new System.Drawing.Point(30, y + 40);
            txtCorrectiveAction.Size = new System.Drawing.Size(770, 80);
            txtCorrectiveAction.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(txtCorrectiveAction);
            
            y += 140;
            
            Label lblDepartureTime = new Label();
            lblDepartureTime.Text = "Departure Time *";
            lblDepartureTime.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblDepartureTime.Location = new System.Drawing.Point(30, y);
            lblDepartureTime.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblDepartureTime);
            
            dtpDepartureTime = new DateTimePicker();
            dtpDepartureTime.Location = new System.Drawing.Point(170, y);
            dtpDepartureTime.Size = new System.Drawing.Size(180, 30);
            dtpDepartureTime.Format = DateTimePickerFormat.Custom;
            dtpDepartureTime.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            this.Controls.Add(dtpDepartureTime);
            
            y += 50;
            
            Label lblStatus = new Label();
            lblStatus.Text = "Status *";
            lblStatus.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblStatus.Location = new System.Drawing.Point(30, y);
            lblStatus.Size = new System.Drawing.Size(100, 30);
            this.Controls.Add(lblStatus);
            
            cmbStatus = new ComboBox();
            cmbStatus.Location = new System.Drawing.Point(140, y);
            cmbStatus.Size = new System.Drawing.Size(180, 30);
            cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStatus.Items.AddRange(new string[] { "Normal", "Irregularities", "Action Required" });
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
            btnView.Click += (s, e) => new ViewRecordsForm("Reg039_NightInspection", "Night Inspection Records").ShowDialog();
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
        
        private void GenerateInspectionID()
        {
            string datePart = DateTime.Now.ToString("yyyyMMdd");
            string query = $"SELECT COUNT(*) FROM Reg039_NightInspection WHERE InspectionID LIKE 'TMS-REG-039-{datePart}-%'";
            int count = Convert.ToInt32(db.ExecuteScalar(query));
            txtInspectionID.Text = $"TMS-REG-039-{datePart}-{(count + 1).ToString("D3")}";
        }
        
        
            private void BtnSave_Click(object sender, EventArgs e)
        {
        
        
            if (string.IsNullOrWhiteSpace(txtSubmittedBy.Text) || !int.TryParse(txtSubmittedBy.Text.Trim(), out _))
            {
                MessageBox.Show("Submitted By must be a valid numeric Staff ID.", "Validation Error");
                return;
            }
            if (!ValidationHelper.IsNotEmpty(txtSignalIDs.Text, "Signal ID(s)")) return;
            if (!ValidationHelper.IsNotEmpty(txtVisibilityStatus.Text, "Visibility Status")) return;
            if (!ValidationHelper.IsSelected(cmbStaffPresence, "Staff Presence")) return;
            if (!ValidationHelper.IsNotEmpty(txtEquipmentStatus.Text, "Equipment Status")) return;
            if (!ValidationHelper.IsSelected(cmbStatus, "Status")) return;
            if (!ValidationHelper.IsEndAfterStart(dtpArrivalTime.Value, dtpDepartureTime.Value, "Arrival Time", "Departure Time")) return;
            
            string correctiveAction = string.IsNullOrEmpty(txtCorrectiveAction.Text) ? "NULL" : $"'{txtCorrectiveAction.Text.Replace("'", "''")}'";
            
            string query = $@"
                INSERT INTO Reg039_NightInspection (InspectionID, InspectionDate, ArrivalTime, SignalIDs, VisibilityStatus, StaffPresence, EquipmentStatus, CorrectiveAction, DepartureTime, Status, SubmittedBy)
                VALUES ('{txtInspectionID.Text}', '{dtpInspectionDate.Value:yyyy-MM-dd}', '{dtpArrivalTime.Value:yyyy-MM-dd HH:mm:ss.fff}', 
                        '{txtSignalIDs.Text.Replace("'", "''")}', '{txtVisibilityStatus.Text.Replace("'", "''")}', 
                        '{cmbStaffPresence.SelectedItem}', '{txtEquipmentStatus.Text.Replace("'", "''")}', 
                        {correctiveAction}, '{dtpDepartureTime.Value:yyyy-MM-dd HH:mm:ss.fff}', '{cmbStatus.SelectedItem}', {txtSubmittedBy.Text})";

            
            
            try
            {
                db.ExecuteNonQuery(query);
                MessageBox.Show($"Night Inspection Record Saved!\nInspection ID: {txtInspectionID.Text}", "Success");
                ClearForm();
                GenerateInspectionID();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error");
            }
        }
        
        
            private void ClearForm()
        {
        
        
            if (txtSubmittedBy != null) txtSubmittedBy.Clear();
            dtpInspectionDate.Value = DateTime.Now;
            dtpArrivalTime.Value = DateTime.Now;
            txtSignalIDs.Clear();
            txtVisibilityStatus.Clear();
            cmbStaffPresence.SelectedIndex = -1;
            txtEquipmentStatus.Clear();
            txtCorrectiveAction.Clear();
            dtpDepartureTime.Value = DateTime.Now.AddHours(1);
            cmbStatus.SelectedIndex = -1;
        }
    
        protected override void OnHandleCreated(System.EventArgs e) { base.OnHandleCreated(e); TMS.ThemeManager.ApplyTheme(this); }
    }
}
