using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TMS
{
    public class Form_Reg027_SafetyMeeting2 : Form
    {
        private TextBox txtMeetingID;
        private DateTimePicker dtpMeetingDate;
        private ComboBox cmbMeetingType;
        private TextBox txtPresidedBy;
        private TextBox txtVenue;
        private RichTextBox txtInvitedStaff;
        private RichTextBox txtAttendedStaff;
        private RichTextBox txtMinutes;
        private RichTextBox txtActionItems;
        private DateTimePicker dtpDeadline;
        private TextBox txtSubmittedBy;
        private DatabaseHelper db = new DatabaseHelper();
        

        public Form_Reg027_SafetyMeeting2()
        {
            this.Text = "Safety Meeting (REG-027)";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.Size = new System.Drawing.Size(850, 800);
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.White;
            
            CreateControls();
            GenerateMeetingID();
        }

        private void CreateControls()
        {
            Panel headerPanel = new Panel();
            headerPanel.BackColor = System.Drawing.Color.FromArgb(0, 51, 102);
            headerPanel.Size = new System.Drawing.Size(850, 80);
            headerPanel.Dock = DockStyle.Top;
            this.Controls.Add(headerPanel);
            
            Label lblPath = new Label();
            lblPath.Text = "Home > Infrastructure Sub > Safety Meeting (Part 2";
            lblPath.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Italic);
            lblPath.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);
            lblPath.Size = new System.Drawing.Size(800, 25);
            lblPath.Location = new System.Drawing.Point(25, 15);
            headerPanel.Controls.Add(lblPath);
            
            Label lblTitle = new Label();
            lblTitle.Text = "SAFETY MEETING REGISTER (PART 2";
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.White;
            lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblTitle.Size = new System.Drawing.Size(800, 35);
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
            
            Label lblPresidedBy = new Label();
            lblPresidedBy.Text = "Presided By *";
            lblPresidedBy.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblPresidedBy.Location = new System.Drawing.Point(30, y);
            lblPresidedBy.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblPresidedBy);
            
            txtPresidedBy = new TextBox();
            txtPresidedBy.Location = new System.Drawing.Point(160, y);
            txtPresidedBy.Size = new System.Drawing.Size(250, 30);
            this.Controls.Add(txtPresidedBy);
            
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
            
            Label lblInvitedStaff = new Label();
            lblInvitedStaff.Text = "Invited Staff *";
            lblInvitedStaff.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblInvitedStaff.Location = new System.Drawing.Point(30, y);
            lblInvitedStaff.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblInvitedStaff);
            
            txtInvitedStaff = new RichTextBox();
            txtInvitedStaff.Location = new System.Drawing.Point(30, y + 40);
            txtInvitedStaff.Size = new System.Drawing.Size(770, 80);
            txtInvitedStaff.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(txtInvitedStaff);
            
            y += 140;
            
            Label lblAttendedStaff = new Label();
            lblAttendedStaff.Text = "Attended Staff *";
            lblAttendedStaff.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblAttendedStaff.Location = new System.Drawing.Point(30, y);
            lblAttendedStaff.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblAttendedStaff);
            
            txtAttendedStaff = new RichTextBox();
            txtAttendedStaff.Location = new System.Drawing.Point(30, y + 40);
            txtAttendedStaff.Size = new System.Drawing.Size(770, 80);
            txtAttendedStaff.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(txtAttendedStaff);
            
            y += 140;
            
            Label lblMinutes = new Label();
            lblMinutes.Text = "Minutes (MoM) *";
            lblMinutes.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblMinutes.Location = new System.Drawing.Point(30, y);
            lblMinutes.Size = new System.Drawing.Size(140, 30);
            this.Controls.Add(lblMinutes);
            
            txtMinutes = new RichTextBox();
            txtMinutes.Location = new System.Drawing.Point(30, y + 40);
            txtMinutes.Size = new System.Drawing.Size(770, 80);
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
            txtActionItems.Size = new System.Drawing.Size(770, 80);
            txtActionItems.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(txtActionItems);
            
            y += 140;
            
            Label lblDeadline = new Label();
            lblDeadline.Text = "Deadline *";
            lblDeadline.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblDeadline.Location = new System.Drawing.Point(30, y);
            lblDeadline.Size = new System.Drawing.Size(100, 30);
            this.Controls.Add(lblDeadline);
            
            dtpDeadline = new DateTimePicker();
            dtpDeadline.Location = new System.Drawing.Point(140, y);
            dtpDeadline.Size = new System.Drawing.Size(180, 30);
            dtpDeadline.Format = DateTimePickerFormat.Short;
            dtpDeadline.MinDate = DateTime.Today;
            this.Controls.Add(dtpDeadline);
            
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
            btnView.Click += (s, e) => new ViewRecordsForm("Reg027_SafetyMeeting2", "Safety Meeting Records").ShowDialog();
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
            string query = $"SELECT COUNT(*) FROM Reg027_SafetyMeeting2 WHERE MeetingID LIKE 'TMS-REG-027-{datePart}-%'";
            int count = Convert.ToInt32(db.ExecuteScalar(query));
            txtMeetingID.Text = $"TMS-REG-027-{datePart}-{(count + 1).ToString("D3")}";
        }
        
        
            private void BtnSave_Click(object sender, EventArgs e)
        {
        
        
            if (string.IsNullOrWhiteSpace(txtSubmittedBy.Text) || !int.TryParse(txtSubmittedBy.Text.Trim(), out _))
            {
                MessageBox.Show("Submitted By must be a valid numeric Staff ID.", "Validation Error");
                return;
            }
            if (!ValidationHelper.IsSelected(cmbMeetingType, "Meeting Type")) return;
            if (!ValidationHelper.IsNotEmpty(txtPresidedBy.Text, "Presided By")) return;
            if (!ValidationHelper.IsNotEmpty(txtVenue.Text, "Venue")) return;
            if (!ValidationHelper.IsNotEmpty(txtInvitedStaff.Text, "Invited Staff")) return;
            if (!ValidationHelper.IsNotEmpty(txtAttendedStaff.Text, "Attended Staff")) return;
            if (!ValidationHelper.IsNotEmpty(txtMinutes.Text, "Minutes")) return;
            if (!ValidationHelper.IsNotEmpty(txtActionItems.Text, "Action Items")) return;
            
            string query = $@"
                INSERT INTO Reg027_SafetyMeeting2 (MeetingID, MeetingDate, MeetingType, PresidedBy, Venue, InvitedStaff, AttendedStaff, Minutes, ActionItems, Deadline, SubmittedBy)
                VALUES ('{txtMeetingID.Text}', '{dtpMeetingDate.Value:yyyy-MM-dd}', '{cmbMeetingType.SelectedItem}', 
                        '{txtPresidedBy.Text}', '{txtVenue.Text}', '{txtInvitedStaff.Text.Replace("'", "''")}', 
                        '{txtAttendedStaff.Text.Replace("'", "''")}', '{txtMinutes.Text.Replace("'", "''")}', 
                        '{txtActionItems.Text.Replace("'", "''")}', '{dtpDeadline.Value:yyyy-MM-dd}', {txtSubmittedBy.Text})";

            
            
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
            txtPresidedBy.Clear();
            txtVenue.Clear();
            txtInvitedStaff.Clear();
            txtAttendedStaff.Clear();
            txtMinutes.Clear();
            txtActionItems.Clear();
            dtpDeadline.Value = DateTime.Now.AddDays(7);
        }
    
        protected override void OnHandleCreated(System.EventArgs e) { base.OnHandleCreated(e); TMS.ThemeManager.ApplyTheme(this); }
    }
}
