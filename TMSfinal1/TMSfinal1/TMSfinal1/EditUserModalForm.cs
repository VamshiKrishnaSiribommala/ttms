using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TMS
{
    /// <summary>
    /// Professional Indian Railways & IRCTC Inspired Edit User Modal Dialog.
    /// Allows Administrator to modify all employee profile details, toggle status (Active/Inactive),
    /// and optionally reset the user's password.
    /// </summary>
    public class EditUserModalForm : Form
    {
        private readonly AuthService authService = new AuthService();
        private readonly int userId;

        private TextBox txtFullName;
        private DateTimePicker dtpDob;
        private ComboBox cmbGender;
        private TextBox txtEmail;
        private TextBox txtPhone;
        private TextBox txtAddress;
        private ComboBox cmbDepartment;
        private TextBox txtCourse;
        private TextBox txtYear;
        private TextBox txtOrg;
        private TextBox txtUsername;
        private ComboBox cmbStatus;
        private TextBox txtNewPassword;
        private CheckBox chkShowPassword;
        private Label lblMsg;
        private Button btnSave;
        private Button btnCancel;
        private Button btnExit;

        public EditUserModalForm(int userId)
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.DoubleBuffered = true;
            this.userId = userId;
            InitializeComponent();
            LoadUserData();
        }

        private void InitializeComponent()
        {
            this.Text = "TMS – Edit Registered User";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new Size(820, 680);
            this.BackColor = Color.FromArgb(244, 246, 249); // Clean IRCTC background

            // Top decorative bar
            Panel topBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 6,
                BackColor = Color.FromArgb(33, 61, 119) // Indian Railways Navy
            };
            this.Controls.Add(topBar);

            // Exit Button
            btnExit = new Button
            {
                Text = "✕",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139),
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(36, 36),
                Location = new Point(770, 12),
                Cursor = Cursors.Hand
            };
            btnExit.FlatAppearance.BorderSize = 0;
            btnExit.Click += (s, e) => this.Close();
            btnExit.MouseEnter += (s, e) => btnExit.ForeColor = Color.FromArgb(220, 38, 38);
            btnExit.MouseLeave += (s, e) => btnExit.ForeColor = Color.FromArgb(100, 116, 139);
            this.Controls.Add(btnExit);

            // Header Title
            Label lblTitle = new Label
            {
                Text = "✏️  EDIT REGISTERED USER ACCOUNT",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 61, 119),
                Location = new Point(35, 18),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            Label lblSub = new Label
            {
                Text = $"Update employee operational credentials, department assignment, and access authorization status (User ID: {userId}).",
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(37, 46),
                AutoSize = true
            };
            this.Controls.Add(lblSub);

            // Main Card
            Panel card = new Panel
            {
                Location = new Point(35, 75),
                Size = new Size(750, 525),
                BackColor = Color.White
            };
            card.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(226, 232, 240), 1))
                    e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
            };
            this.Controls.Add(card);

            int col1 = 30;
            int col2 = 390;
            int colW = 330;
            int rowH = 60;
            int y = 20;

            // Row 1: Full Name & Username
            card.Controls.Add(MakeFieldLabel("Full Name *", col1, y));
            txtFullName = MakeTextBox(col1, y + 20, colW);
            card.Controls.Add(txtFullName);

            card.Controls.Add(MakeFieldLabel("Username / Login ID (Read-only)", col2, y));
            txtUsername = MakeTextBox(col2, y + 20, colW);
            txtUsername.ReadOnly = true;
            txtUsername.BackColor = Color.FromArgb(241, 245, 249);
            card.Controls.Add(txtUsername);

            y += rowH;

            // Row 2: Date of Birth & Gender
            card.Controls.Add(MakeFieldLabel("Date of Birth *", col1, y));
            dtpDob = new DateTimePicker
            {
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd-MMM-yyyy",
                Font = new Font("Segoe UI", 10f),
                Location = new Point(col1, y + 20),
                Size = new Size(colW, 28),
                MaxDate = DateTime.Today
            };
            card.Controls.Add(dtpDob);

            card.Controls.Add(MakeFieldLabel("Gender", col2, y));
            cmbGender = new ComboBox
            {
                Font = new Font("Segoe UI", 10f),
                Location = new Point(col2, y + 20),
                Size = new Size(colW, 28),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbGender.Items.AddRange(new object[] { "Male", "Female", "Other" });
            card.Controls.Add(cmbGender);

            y += rowH;

            // Row 3: Email & Phone
            card.Controls.Add(MakeFieldLabel("Email Address *", col1, y));
            txtEmail = MakeTextBox(col1, y + 20, colW);
            card.Controls.Add(txtEmail);

            card.Controls.Add(MakeFieldLabel("Phone Number *", col2, y));
            txtPhone = MakeTextBox(col2, y + 20, colW);
            card.Controls.Add(txtPhone);

            y += rowH;

            // Row 4: Department & Designation / Course
            card.Controls.Add(MakeFieldLabel("Department *", col1, y));
            cmbDepartment = new ComboBox
            {
                Font = new Font("Segoe UI", 10f),
                Location = new Point(col1, y + 20),
                Size = new Size(colW, 28),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbDepartment.Items.AddRange(new object[] {
                "Operating (Station Master / Traffic)",
                "Signal & Telecommunication (S&T)",
                "Commercial (Booking & Goods)",
                "Safety & Inspection",
                "Civil Engineering & Track",
                "Electrical & Power Supply",
                "Mechanical & Rolling Stock",
                "Security (RPF / Station Staff)",
                "Administration & HR"
            });
            card.Controls.Add(cmbDepartment);

            card.Controls.Add(MakeFieldLabel("Course / Designation", col2, y));
            txtCourse = MakeTextBox(col2, y + 20, colW);
            card.Controls.Add(txtCourse);

            y += rowH;

            // Row 5: Year of Joining & Organization/Division
            card.Controls.Add(MakeFieldLabel("Year of Joining", col1, y));
            txtYear = MakeTextBox(col1, y + 20, colW);
            card.Controls.Add(txtYear);

            card.Controls.Add(MakeFieldLabel("Organization / Division", col2, y));
            txtOrg = MakeTextBox(col2, y + 20, colW);
            card.Controls.Add(txtOrg);

            y += rowH;

            // Row 6: Station Address & Account Status
            card.Controls.Add(MakeFieldLabel("Station / Office Address", col1, y));
            txtAddress = MakeTextBox(col1, y + 20, colW);
            card.Controls.Add(txtAddress);

            card.Controls.Add(MakeFieldLabel("Account Status (Enable / Disable) *", col2, y));
            cmbStatus = new ComboBox
            {
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Location = new Point(col2, y + 20),
                Size = new Size(colW, 28),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbStatus.Items.AddRange(new object[] { "Active", "Inactive" });
            card.Controls.Add(cmbStatus);

            y += rowH;

            // Row 7: Optional Reset Password
            card.Controls.Add(MakeFieldLabel("Reset Password (leave blank to keep current)", col1, y));
            txtNewPassword = MakeTextBox(col1, y + 20, colW);
            txtNewPassword.UseSystemPasswordChar = true;
            card.Controls.Add(txtNewPassword);

            chkShowPassword = new CheckBox
            {
                Text = "Show Password",
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(col2, y + 22),
                Size = new Size(160, 24),
                Cursor = Cursors.Hand
            };
            chkShowPassword.CheckedChanged += (s, e) => txtNewPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
            card.Controls.Add(chkShowPassword);

            // Message label
            lblMsg = new Label
            {
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 38, 38),
                Location = new Point(35, 608),
                Size = new Size(450, 32),
                TextAlign = ContentAlignment.MiddleLeft,
                Text = ""
            };
            this.Controls.Add(lblMsg);

            // Action Buttons
            btnSave = new Button
            {
                Text = "💾  SAVE CHANGES",
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                Size = new Size(160, 42),
                Location = new Point(490, 612),
                BackColor = Color.FromArgb(251, 121, 43), // IRCTC Orange #FB792B
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.MouseEnter += (s, e) => btnSave.BackColor = Color.FromArgb(234, 88, 12);
            btnSave.MouseLeave += (s, e) => btnSave.BackColor = Color.FromArgb(251, 121, 43);
            btnSave.Click += (s, e) => SaveChanges();
            this.Controls.Add(btnSave);

            btnCancel = new Button
            {
                Text = "Cancel",
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Size = new Size(110, 42),
                Location = new Point(665, 612),
                BackColor = Color.FromArgb(226, 232, 240),
                ForeColor = Color.FromArgb(51, 65, 85),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => this.Close();
            this.Controls.Add(btnCancel);

            // Window Drag
            this.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    ThemeManager.ReleaseCapture();
                    ThemeManager.SendMessage(this.Handle, ThemeManager.WM_NCLBUTTONDOWN, ThemeManager.HT_CAPTION, 0);
                }
            };
        }

        private Label MakeFieldLabel(string text, int x, int y) => new Label
        {
            Text = text,
            Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
            ForeColor = Color.FromArgb(33, 61, 119),
            Location = new Point(x, y),
            Size = new Size(330, 18)
        };

        private TextBox MakeTextBox(int x, int y, int w) => new TextBox
        {
            Font = new Font("Segoe UI", 10f),
            Location = new Point(x, y),
            Size = new Size(w, 28),
            BackColor = Color.White,
            ForeColor = Color.FromArgb(30, 41, 59),
            BorderStyle = BorderStyle.FixedSingle
        };

        private void LoadUserData()
        {
            DataRow row = authService.GetUserById(userId);
            if (row == null)
            {
                MessageBox.Show("Could not load user data.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            txtFullName.Text = row["FullName"].ToString();
            txtUsername.Text = row["Username"].ToString();
            txtEmail.Text = row["Email"].ToString();
            txtPhone.Text = row["Phone"] != DBNull.Value ? row["Phone"].ToString() : "";
            txtAddress.Text = row["Address"] != DBNull.Value ? row["Address"].ToString() : "";
            txtCourse.Text = row["Course"] != DBNull.Value ? row["Course"].ToString() : "";
            txtYear.Text = row["YearOfJoining"] != DBNull.Value ? row["YearOfJoining"].ToString() : "";
            txtOrg.Text = row["CollegeOrOrg"] != DBNull.Value ? row["CollegeOrOrg"].ToString() : "";

            if (row["DateOfBirth"] != DBNull.Value)
            {
                dtpDob.Value = Convert.ToDateTime(row["DateOfBirth"]);
            }

            string gender = row["Gender"] != DBNull.Value ? row["Gender"].ToString() : "Male";
            cmbGender.SelectedItem = gender;

            string dept = row["Department"].ToString();
            if (cmbDepartment.Items.Contains(dept))
                cmbDepartment.SelectedItem = dept;
            else
                cmbDepartment.Text = dept;

            string status = row["Status"].ToString();
            cmbStatus.SelectedItem = string.Equals(status, "Active", StringComparison.OrdinalIgnoreCase) ? "Active" : "Inactive";
        }

        private void SaveChanges()
        {
            lblMsg.Text = "";
            string fullName = txtFullName.Text.Trim();
            DateTime dob = dtpDob.Value;
            string gender = cmbGender.SelectedItem?.ToString();
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string address = txtAddress.Text.Trim();
            string dept = cmbDepartment.SelectedItem?.ToString() ?? cmbDepartment.Text.Trim();
            string course = txtCourse.Text.Trim();
            string year = txtYear.Text.Trim();
            string org = txtOrg.Text.Trim();
            string status = cmbStatus.SelectedItem?.ToString() ?? "Active";
            string newPass = txtNewPassword.Text.Trim();

            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(phone) || string.IsNullOrWhiteSpace(dept))
            {
                lblMsg.ForeColor = Color.FromArgb(220, 38, 38);
                lblMsg.Text = "⚠️ Please fill in all required fields marked with *.";
                return;
            }

            btnSave.Enabled = false;
            btnSave.Text = "Saving...";

            bool success = authService.UpdateUser(
                userId,
                fullName,
                dob,
                gender,
                email,
                phone,
                address,
                dept,
                course,
                year,
                org,
                string.IsNullOrWhiteSpace(newPass) ? null : newPass,
                status,
                out string errMsg);

            btnSave.Enabled = true;
            btnSave.Text = "💾  SAVE CHANGES";

            if (success)
            {
                MessageBox.Show($"User '{txtUsername.Text}' updated successfully!", "Update Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                lblMsg.ForeColor = Color.FromArgb(220, 38, 38);
                lblMsg.Text = "❌ " + errMsg;
            }
        }
    }
}
