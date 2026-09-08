using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TMS
{
    public class Form_Reg025_SafetyMeeting : Form
    {
        private TextBox txtMeetingID;
        private DateTimePicker dtpMeetingDate;
        private ComboBox cmbMeetingType;
        private TextBox txtChairpersonID;
        private TextBox txtChairpersonName;
        private TextBox txtVenue;
        private RichTextBox txtAttendeeList;
        private RichTextBox txtAgenda;
        private RichTextBox txtMinutes;
        private RichTextBox txtActionItems;
        private ComboBox cmbFollowUpStatus;
        private ComboBox cmbMeetingStatus;
        private TextBox txtSubmittedBy;
        private DatabaseHelper db = new DatabaseHelper();
        

        public Form_Reg025_SafetyMeeting()
        {
            this.Text = "Safety Meeting (REG-025)";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.Size = new System.Drawing.Size(900, 850);
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.White;
            
            CreateControls();
            GenerateMeetingID();
        }

        private void CreateControls()
        {
            Panel headerPanel = new Panel();
            headerPanel.BackColor = System.Drawing.Color.FromArgb(0, 51, 102);
            headerPanel.Size = new System.Drawing.Size(900, 80);
            headerPanel.Dock = DockStyle.Top;
            this.Controls.Add(headerPanel);
            
            Label lblPath = new Label();
            lblPath.Text = "Home > Infrastructure Sub > Safety Meeting";
            lblPath.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Italic);
            lblPath.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);
            lblPath.Size = new System.Drawing.Size(850, 25);
            lblPath.Location = new System.Drawing.Point(25, 15);
            headerPanel.Controls.Add(lblPath);
            
            Label lblTitle = new Label();
            lblTitle.Text = "SAFETY MEETING REGISTER";
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.White;
            lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblTitle.Size = new System.Drawing.Size(850, 35);
            lblTitle.Location = new System.Drawing.Point(25, 40);
            headerPanel.Controls.Add(lblTitle);
            
            int y = 110;
            
            Label lblMeetingID = new Label();
            lblMeetingID.Text = "Meeting ID (System Generated):";
            lblMeetingID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblMeetingID.Location = new System.Drawing.Point(30, y);
            lblMeetingID.Size = new System.Drawing.Size(220, 30);
            this.Controls.Add(lblMeetingID);
            
            txtMeetingID = new TextBox();
            txtMeetingID.Location = new System.Drawing.Point(260, y);
            txtMeetingID.Size = new System.Drawing.Size(300, 30);
            txtMeetingID.ReadOnly = true;
            txtMeetingID.BackColor = System.Drawing.Color.LightGray;
            this.Controls.Add(txtMeetingID);
            
            y += 50;
            
            Label lblMeetingDate = new Label();
            lblMeetingDate.Text = "Meeting Date *";
            lblMeetingDate.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblMeetingDate.Location = new System.Drawing.Point(30, y);
            lblMeetingDate.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblMeetingDate);
            
            dtpMeetingDate = new DateTimePicker();
            dtpMeetingDate.Location = new System.Drawing.Point(160, y);
            dtpMeetingDate.Size = new System.Drawing.Size(180, 30);
            dtpMeetingDate.Format = DateTimePickerFormat.Short;
            this.Controls.Add(dtpMeetingDate);
            
            y += 50;
            
            Label lblMeetingType = new Label();
            lblMeetingType.Text = "Meeting Type *";
            lblMeetingType.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblMeetingType.Location = new System.Drawing.Point(30, y);
            lblMeetingType.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblMeetingType);
            
            cmbMeetingType = new ComboBox();
            cmbMeetingType.Location = new System.Drawing.Point(160, y);
            cmbMeetingType.Size = new System.Drawing.Size(180, 30);
            cmbMeetingType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbMeetingType.Items.AddRange(new string[] { "Monthly", "Emergency", "Counseling", "Special", "Quarterly" });
            this.Controls.Add(cmbMeetingType);
            
            y += 50;
            
            Label lblChairpersonID = new Label();
            lblChairpersonID.Text = "Chairperson ID *";
            lblChairpersonID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblChairpersonID.Location = new System.Drawing.Point(30, y);
            lblChairpersonID.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblChairpersonID);
            
            txtChairpersonID = new TextBox();
            txtChairpersonID.Location = new System.Drawing.Point(170, y);
            txtChairpersonID.Size = new System.Drawing.Size(180, 30);
            this.Controls.Add(txtChairpersonID);
            
            y += 50;
            
            Label lblChairpersonName = new Label();
            lblChairpersonName.Text = "Chairperson Name *";
            lblChairpersonName.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblChairpersonName.Location = new System.Drawing.Point(30, y);
            lblChairpersonName.Size = new System.Drawing.Size(140, 30);
            this.Controls.Add(lblChairpersonName);
            
            txtChairpersonName = new TextBox();
            txtChairpersonName.Location = new System.Drawing.Point(180, y);
            txtChairpersonName.Size = new System.Drawing.Size(250, 30);
            this.Controls.Add(txtChairpersonName);
            
            y += 50;
            
            Label lblVenue = new Label();
            lblVenue.Text = "Venue *";
            lblVenue.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblVenue.Location = new System.Drawing.Point(30, y);
            lblVenue.Size = new System.Drawing.Size(100, 30);
            this.Controls.Add(lblVenue);
            
            txtVenue = new TextBox();
            txtVenue.Location = new System.Drawing.Point(140, y);
            txtVenue.Size = new System.Drawing.Size(300, 30);
            this.Controls.Add(txtVenue);
            
            y += 80;
            
            Label lblAttendeeList = new Label();
            lblAttendeeList.Text = "Attendee List *";
            lblAttendeeList.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblAttendeeList.Location = new System.Drawing.Point(30, y);
            lblAttendeeList.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblAttendeeList);
            
            txtAttendeeList = new RichTextBox();
            txtAttendeeList.Location = new System.Drawing.Point(30, y + 40);
            txtAttendeeList.Size = new System.Drawing.Size(820, 80);
            txtAttendeeList.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(txtAttendeeList);
            
            y += 140;
            
            Label lblAgenda = new Label();
            lblAgenda.Text = "Agenda/Topic *";
            lblAgenda.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblAgenda.Location = new System.Drawing.Point(30, y);
            lblAgenda.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblAgenda);
            
            txtAgenda = new RichTextBox();
            txtAgenda.Location = new System.Drawing.Point(30, y + 40);
            txtAgenda.Size = new System.Drawing.Size(820, 80);
            txtAgenda.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(txtAgenda);
            
            y += 140;
            
            Label lblMinutes = new Label();
            lblMinutes.Text = "Minutes (MoM) *";
            lblMinutes.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblMinutes.Location = new System.Drawing.Point(30, y);
            lblMinutes.Size = new System.Drawing.Size(140, 30);
            this.Controls.Add(lblMinutes);
            
            txtMinutes = new RichTextBox();
            txtMinutes.Location = new System.Drawing.Point(30, y + 40);
            txtMinutes.Size = new System.Drawing.Size(820, 80);
            txtMinutes.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(txtMinutes);
            
            y += 140;
            
            Label lblActionItems = new Label();
            lblActionItems.Text = "Action Items *";
            lblActionItems.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblActionItems.Location = new System.Drawing.Point(30, y);
            lblActionItems.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblActionItems);
            
            txtActionItems = new RichTextBox();
            txtActionItems.Location = new System.Drawing.Point(30, y + 40);
            txtActionItems.Size = new System.Drawing.Size(820, 80);
            txtActionItems.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(txtActionItems);
            
            y += 140;
            
            Label lblFollowUpStatus = new Label();
            lblFollowUpStatus.Text = "Follow-Up Status *";
            lblFollowUpStatus.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblFollowUpStatus.Location = new System.Drawing.Point(30, y);
            lblFollowUpStatus.Size = new System.Drawing.Size(140, 30);
            this.Controls.Add(lblFollowUpStatus);
            
            cmbFollowUpStatus = new ComboBox();
            cmbFollowUpStatus.Location = new System.Drawing.Point(180, y);
            cmbFollowUpStatus.Size = new System.Drawing.Size(180, 30);
            cmbFollowUpStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFollowUpStatus.Items.AddRange(new string[] { "Pending", "In Progress", "Completed" });
            this.Controls.Add(cmbFollowUpStatus);
            
            y += 50;
            
            Label lblMeetingStatus = new Label();
            lblMeetingStatus.Text = "Meeting Status *";
            lblMeetingStatus.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblMeetingStatus.Location = new System.Drawing.Point(30, y);
            lblMeetingStatus.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblMeetingStatus);
            
            cmbMeetingStatus = new ComboBox();
            cmbMeetingStatus.Location = new System.Drawing.Point(170, y);
            cmbMeetingStatus.Size = new System.Drawing.Size(180, 30);
            cmbMeetingStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbMeetingStatus.Items.AddRange(new string[] { "Open", "Action Pending", "Closed" });
            this.Controls.Add(cmbMeetingStatus);
            
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
            btnView.Click += (s, e) => new ViewRecordsForm("Reg025_SafetyMeeting", "Safety Meeting Records").ShowDialog();
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
        
        private void GenerateMeetingID()
        {
            string datePart = DateTime.Now.ToString("yyyyMM");
            string query = $"SELECT COUNT(*) FROM Reg025_SafetyMeeting WHERE MeetingID LIKE 'TMS-REG-025-{datePart}-%'";
            int count = Convert.ToInt32(db.ExecuteScalar(query));
            txtMeetingID.Text = $"TMS-REG-025-{datePart}-{(count + 1).ToString("D3")}";
        }
        
        
            private void BtnSave_Click(object sender, EventArgs e)
        {
        
        
            if (string.IsNullOrWhiteSpace(txtSubmittedBy.Text) || !int.TryParse(txtSubmittedBy.Text.Trim(), out _))
            {
                MessageBox.Show("Submitted By must be a valid numeric Staff ID.", "Validation Error");
                return;
            }
            if (!ValidationHelper.IsSelected(cmbMeetingType, "Meeting Type")) return;
            if (!ValidationHelper.IsNotEmpty(txtChairpersonID.Text, "Chairperson ID")) return;
            if (!ValidationHelper.IsNotEmpty(txtChairpersonName.Text, "Chairperson Name")) return;
            if (!ValidationHelper.IsNotEmpty(txtVenue.Text, "Venue")) return;
            if (!ValidationHelper.IsNotEmpty(txtAttendeeList.Text, "Attendee List")) return;
            if (!ValidationHelper.IsNotEmpty(txtAgenda.Text, "Agenda")) return;
            if (!ValidationHelper.IsNotEmpty(txtMinutes.Text, "Minutes")) return;
            if (!ValidationHelper.IsNotEmpty(txtActionItems.Text, "Action Items")) return;
            if (!ValidationHelper.IsSelected(cmbFollowUpStatus, "Follow-Up Status")) return;
            if (!ValidationHelper.IsSelected(cmbMeetingStatus, "Meeting Status")) return;
            
            string query = $@"
                INSERT INTO Reg025_SafetyMeeting (MeetingID, MeetingDate, MeetingType, ChairpersonID, ChairpersonName, Venue, AttendeeList, Agenda, Minutes, ActionItems, FollowUpStatus, MeetingStatus, SubmittedBy)
                VALUES ('{txtMeetingID.Text}', '{dtpMeetingDate.Value:yyyy-MM-dd}', '{cmbMeetingType.SelectedItem}', 
                        '{txtChairpersonID.Text}', '{txtChairpersonName.Text}', '{txtVenue.Text}', 
                        '{txtAttendeeList.Text.Replace("'", "''")}', '{txtAgenda.Text.Replace("'", "''")}', 
                        '{txtMinutes.Text.Replace("'", "''")}', '{txtActionItems.Text.Replace("'", "''")}', 
                        '{cmbFollowUpStatus.SelectedItem}', '{cmbMeetingStatus.SelectedItem}', {txtSubmittedBy.Text})";

            
            
            try
            {
                db.ExecuteNonQuery(query);
                MessageBox.Show($"Safety Meeting Record Saved!\nMeeting ID: {txtMeetingID.Text}", "Success");
                ClearForm();
                GenerateMeetingID();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error");
            }
        }
        
        
            private void ClearForm()
        {
        
        
            if (txtSubmittedBy != null) txtSubmittedBy.Clear();
            dtpMeetingDate.Value = DateTime.Now;
            cmbMeetingType.SelectedIndex = -1;
            txtChairpersonID.Clear();
            txtChairpersonName.Clear();
            txtVenue.Clear();
            txtAttendeeList.Clear();
            txtAgenda.Clear();
            txtMinutes.Clear();
            txtActionItems.Clear();
            cmbFollowUpStatus.SelectedIndex = -1;
            cmbMeetingStatus.SelectedIndex = -1;
        }
    
        protected override void OnHandleCreated(System.EventArgs e) { base.OnHandleCreated(e); TMS.ThemeManager.ApplyTheme(this); }
    }
}
