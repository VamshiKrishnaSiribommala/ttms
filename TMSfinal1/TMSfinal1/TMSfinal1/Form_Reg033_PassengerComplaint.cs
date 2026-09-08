using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TMS
{
    public class Form_Reg033_PassengerComplaint : Form
    {
        private TextBox txtEntryID;
        private DateTimePicker dtpComplaintDateTime;
        private TextBox txtPassengerName;
        private TextBox txtMobileNumber;
        private TextBox txtPNR;
        private ComboBox cmbCategory;
        private RichTextBox txtComplaintDetails;
        private TextBox txtStationLocation;
        private TextBox txtSMRemark;
        private ComboBox cmbAssignedDept;
        private RichTextBox txtFinalAction;
        private DateTimePicker dtpResolutionTime;
        private ComboBox cmbStatus;
        private TextBox txtSubmittedBy;
        private DatabaseHelper db = new DatabaseHelper();
        

        public Form_Reg033_PassengerComplaint()
        {
            this.Text = "Passenger Complaint (REG-033)";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.Size = new System.Drawing.Size(850, 850);
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.White;
            
            CreateControls();
            GenerateEntryID();
        }

        private void CreateControls()
        {
            Panel headerPanel = new Panel();
            headerPanel.BackColor = System.Drawing.Color.FromArgb(0, 51, 102);
            headerPanel.Size = new System.Drawing.Size(850, 80);
            headerPanel.Dock = DockStyle.Top;
            this.Controls.Add(headerPanel);
            
            Label lblPath = new Label();
            lblPath.Text = "Home > Safety List > Passenger Complaint";
            lblPath.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Italic);
            lblPath.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);
            lblPath.Size = new System.Drawing.Size(800, 25);
            lblPath.Location = new System.Drawing.Point(25, 15);
            headerPanel.Controls.Add(lblPath);
            
            Label lblTitle = new Label();
            lblTitle.Text = "PASSENGER COMPLAINT REGISTER";
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.White;
            lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblTitle.Size = new System.Drawing.Size(800, 35);
            lblTitle.Location = new System.Drawing.Point(25, 40);
            headerPanel.Controls.Add(lblTitle);
            
            int y = 110;
            
            Label lblEntryID = new Label();
            lblEntryID.Text = "Entry ID (System Generated):";
            lblEntryID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblEntryID.Location = new System.Drawing.Point(30, y);
            lblEntryID.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(lblEntryID);
            
            txtEntryID = new TextBox();
            txtEntryID.Location = new System.Drawing.Point(240, y);
            txtEntryID.Size = new System.Drawing.Size(300, 30);
            txtEntryID.ReadOnly = true;
            txtEntryID.BackColor = System.Drawing.Color.LightGray;
            this.Controls.Add(txtEntryID);
            
            y += 50;
            
            Label lblComplaintDateTime = new Label();
            lblComplaintDateTime.Text = "Complaint Date & Time *";
            lblComplaintDateTime.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblComplaintDateTime.Location = new System.Drawing.Point(30, y);
            lblComplaintDateTime.Size = new System.Drawing.Size(170, 30);
            this.Controls.Add(lblComplaintDateTime);
            
            dtpComplaintDateTime = new DateTimePicker();
            dtpComplaintDateTime.Location = new System.Drawing.Point(210, y);
            dtpComplaintDateTime.Size = new System.Drawing.Size(200, 30);
            dtpComplaintDateTime.Format = DateTimePickerFormat.Custom;
            dtpComplaintDateTime.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            this.Controls.Add(dtpComplaintDateTime);
            
            y += 50;
            
            Label lblPassengerName = new Label();
            lblPassengerName.Text = "Passenger Name *";
            lblPassengerName.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblPassengerName.Location = new System.Drawing.Point(30, y);
            lblPassengerName.Size = new System.Drawing.Size(140, 30);
            this.Controls.Add(lblPassengerName);
            
            txtPassengerName = new TextBox();
            txtPassengerName.Location = new System.Drawing.Point(180, y);
            txtPassengerName.Size = new System.Drawing.Size(250, 30);
            this.Controls.Add(txtPassengerName);
            
            y += 50;
            
            Label lblMobileNumber = new Label();
            lblMobileNumber.Text = "Mobile Number *";
            lblMobileNumber.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblMobileNumber.Location = new System.Drawing.Point(30, y);
            lblMobileNumber.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblMobileNumber);
            
            txtMobileNumber = new TextBox();
            txtMobileNumber.Location = new System.Drawing.Point(170, y);
            txtMobileNumber.Size = new System.Drawing.Size(150, 30);
            this.Controls.Add(txtMobileNumber);
            
            y += 50;
            
            Label lblPNR = new Label();
            lblPNR.Text = "PNR / Ticket No";
            lblPNR.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblPNR.Location = new System.Drawing.Point(30, y);
            lblPNR.Size = new System.Drawing.Size(140, 30);
            this.Controls.Add(lblPNR);
            
            txtPNR = new TextBox();
            txtPNR.Location = new System.Drawing.Point(180, y);
            txtPNR.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(txtPNR);
            
            y += 50;
            
            Label lblCategory = new Label();
            lblCategory.Text = "Category *";
            lblCategory.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblCategory.Location = new System.Drawing.Point(30, y);
            lblCategory.Size = new System.Drawing.Size(100, 30);
            this.Controls.Add(lblCategory);
            
            cmbCategory = new ComboBox();
            cmbCategory.Location = new System.Drawing.Point(140, y);
            cmbCategory.Size = new System.Drawing.Size(200, 30);
            cmbCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCategory.Items.AddRange(new string[] { "Cleanliness", "Staff Behavior", "Security", "Amenities", "Catering", "Ticketing", "Others" });
            this.Controls.Add(cmbCategory);
            
            y += 80;
            
            Label lblComplaintDetails = new Label();
            lblComplaintDetails.Text = "Complaint Details *";
            lblComplaintDetails.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblComplaintDetails.Location = new System.Drawing.Point(30, y);
            lblComplaintDetails.Size = new System.Drawing.Size(140, 30);
            this.Controls.Add(lblComplaintDetails);
            
            txtComplaintDetails = new RichTextBox();
            txtComplaintDetails.Location = new System.Drawing.Point(30, y + 40);
            txtComplaintDetails.Size = new System.Drawing.Size(770, 100);
            txtComplaintDetails.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(txtComplaintDetails);
            
            y += 160;
            
            Label lblStationLocation = new Label();
            lblStationLocation.Text = "Station Location *";
            lblStationLocation.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblStationLocation.Location = new System.Drawing.Point(30, y);
            lblStationLocation.Size = new System.Drawing.Size(140, 30);
            this.Controls.Add(lblStationLocation);
            
            txtStationLocation = new TextBox();
            txtStationLocation.Location = new System.Drawing.Point(180, y);
            txtStationLocation.Size = new System.Drawing.Size(250, 30);
            this.Controls.Add(txtStationLocation);
            
            y += 50;
            
            Label lblSMRemark = new Label();
            lblSMRemark.Text = "SM Remark *";
            lblSMRemark.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblSMRemark.Location = new System.Drawing.Point(30, y);
            lblSMRemark.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblSMRemark);
            
            txtSMRemark = new TextBox();
            txtSMRemark.Location = new System.Drawing.Point(160, y);
            txtSMRemark.Size = new System.Drawing.Size(400, 30);
            this.Controls.Add(txtSMRemark);
            
            y += 50;
            
            Label lblAssignedDept = new Label();
            lblAssignedDept.Text = "Assigned Dept *";
            lblAssignedDept.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblAssignedDept.Location = new System.Drawing.Point(30, y);
            lblAssignedDept.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblAssignedDept);
            
            cmbAssignedDept = new ComboBox();
            cmbAssignedDept.Location = new System.Drawing.Point(170, y);
            cmbAssignedDept.Size = new System.Drawing.Size(200, 30);
            cmbAssignedDept.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbAssignedDept.Items.AddRange(new string[] { "Commercial", "Works", "Electrical", "S&T", "Security", "Catering", "Others" });
            this.Controls.Add(cmbAssignedDept);
            
            y += 80;
            
            Label lblFinalAction = new Label();
            lblFinalAction.Text = "Final Action *";
            lblFinalAction.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblFinalAction.Location = new System.Drawing.Point(30, y);
            lblFinalAction.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblFinalAction);
            
            txtFinalAction = new RichTextBox();
            txtFinalAction.Location = new System.Drawing.Point(30, y + 40);
            txtFinalAction.Size = new System.Drawing.Size(770, 80);
            txtFinalAction.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(txtFinalAction);
            
            y += 140;
            
            Label lblResolutionTime = new Label();
            lblResolutionTime.Text = "Resolution Time";
            lblResolutionTime.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblResolutionTime.Location = new System.Drawing.Point(30, y);
            lblResolutionTime.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblResolutionTime);
            
            dtpResolutionTime = new DateTimePicker();
            dtpResolutionTime.Location = new System.Drawing.Point(170, y);
            dtpResolutionTime.Size = new System.Drawing.Size(200, 30);
            dtpResolutionTime.Format = DateTimePickerFormat.Custom;
            dtpResolutionTime.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            dtpResolutionTime.ShowCheckBox = true;
            dtpResolutionTime.Checked = false;
            this.Controls.Add(dtpResolutionTime);
            
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
            cmbStatus.Items.AddRange(new string[] { "Pending", "Under Review", "Resolved", "Closed" });
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
            btnView.Click += (s, e) => new ViewRecordsForm("Reg033_PassengerComplaint", "Passenger Complaint Records").ShowDialog();
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
        
        private void GenerateEntryID()
        {
            string datePart = DateTime.Now.ToString("yyyyMM");
            string query = $"SELECT COUNT(*) FROM Reg033_PassengerComplaint WHERE EntryID LIKE 'TMS-REG-033-{datePart}-%'";
            int count = Convert.ToInt32(db.ExecuteScalar(query));
            txtEntryID.Text = $"TMS-REG-033-{datePart}-{(count + 1).ToString("D3")}";
        }
        
        
            private void BtnSave_Click(object sender, EventArgs e)
        {
        
        
            if (string.IsNullOrWhiteSpace(txtSubmittedBy.Text) || !int.TryParse(txtSubmittedBy.Text.Trim(), out _))
            {
                MessageBox.Show("Submitted By must be a valid numeric Staff ID.", "Validation Error");
                return;
            }
            if (!ValidationHelper.IsNotEmpty(txtPassengerName.Text, "Passenger Name")) return;
            if (!ValidationHelper.IsNotEmpty(txtMobileNumber.Text, "Mobile Number")) return;
            if (!ValidationHelper.IsSelected(cmbCategory, "Category")) return;
            if (!ValidationHelper.IsNotEmpty(txtComplaintDetails.Text, "Complaint Details")) return;
            if (!ValidationHelper.IsNotEmpty(txtStationLocation.Text, "Station Location")) return;
            if (!ValidationHelper.IsNotEmpty(txtSMRemark.Text, "SM Remark")) return;
            if (!ValidationHelper.IsSelected(cmbAssignedDept, "Assigned Dept")) return;
            if (!ValidationHelper.IsNotEmpty(txtFinalAction.Text, "Final Action")) return;
            if (!ValidationHelper.IsSelected(cmbStatus, "Status")) return;
            
            string pnr = string.IsNullOrEmpty(txtPNR.Text) ? "NULL" : $"'{txtPNR.Text}'";
            string resolutionTime = dtpResolutionTime.Checked ? $"'{dtpResolutionTime.Value:yyyy-MM-dd HH:mm:ss.fff}'" : "NULL";
            
            string query = $@"
                INSERT INTO Reg033_PassengerComplaint (EntryID, ComplaintDateTime, PassengerName, MobileNumber, PNR_TicketNo, Category, ComplaintDetails, StationLocation, SMRemark, AssignedDept, FinalAction, ResolutionTime, Status, SubmittedBy)
                VALUES ('{txtEntryID.Text}', '{dtpComplaintDateTime.Value:yyyy-MM-dd HH:mm:ss.fff}', '{txtPassengerName.Text}', 
                        '{txtMobileNumber.Text}', {pnr}, '{cmbCategory.SelectedItem}', 
                        '{txtComplaintDetails.Text.Replace("'", "''")}', '{txtStationLocation.Text}', 
                        '{txtSMRemark.Text}', '{cmbAssignedDept.SelectedItem}', 
                        '{txtFinalAction.Text.Replace("'", "''")}', {resolutionTime}, '{cmbStatus.SelectedItem}', {txtSubmittedBy.Text})";

            
            
            try
            {
                db.ExecuteNonQuery(query);
                MessageBox.Show($"Passenger Complaint Saved!\nEntry ID: {txtEntryID.Text}", "Success");
                ClearForm();
                GenerateEntryID();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error");
            }
        }
        
        
            private void ClearForm()
        {
        
        
            if (txtSubmittedBy != null) txtSubmittedBy.Clear();
            dtpComplaintDateTime.Value = DateTime.Now;
            txtPassengerName.Clear();
            txtMobileNumber.Clear();
            txtPNR.Clear();
            cmbCategory.SelectedIndex = -1;
            txtComplaintDetails.Clear();
            txtStationLocation.Clear();
            txtSMRemark.Clear();
            cmbAssignedDept.SelectedIndex = -1;
            txtFinalAction.Clear();
            dtpResolutionTime.Checked = false;
            cmbStatus.SelectedIndex = -1;
        }
    
        protected override void OnHandleCreated(System.EventArgs e) { base.OnHandleCreated(e); TMS.ThemeManager.ApplyTheme(this); }
    }
}
