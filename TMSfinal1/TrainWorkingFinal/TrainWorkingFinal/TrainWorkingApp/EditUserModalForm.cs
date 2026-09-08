using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace TrainWorkingApp
{
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

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // WS_EX_COMPOSITED - eliminate flicker
                return cp;
            }
        }

        public EditUserModalForm(int userId)
        {
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
            this.BackColor = Color.FromArgb(244, 246, 249);

            Panel topBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 6,
                BackColor = Color.FromArgb(33, 61, 119)
            };
            this.Controls.Add(topBar);

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

            card.Controls.Add(MakeFieldLabel("Full Name *", col1, y));
            txtFullName = MakeTextBox(col1, y + 20, colW);
            card.Controls.Add(txtFullName);

            card.Controls.Add(MakeFieldLabel("Username / Login ID (Read-only)", col2, y));
            txtUsername = MakeTextBox(col2, y + 20, colW);
            txtUsername.ReadOnly = true;
            txtUsername.BackColor = Color.FromArgb(241, 245, 249);
            card.Controls.Add(txtUsername);

            y += rowH;

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

            card.Controls.Add(MakeFieldLabel("Email Address *", col1, y));
            txtEmail = MakeTextBox(col1, y + 20, colW);
            card.Controls.Add(txtEmail);

            card.Controls.Add(MakeFieldLabel("Phone Number *", col2, y));
            txtPhone = MakeTextBox(col2, y + 20, colW);
            card.Controls.Add(txtPhone);

            y += rowH;

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

            card.Controls.Add(MakeFieldLabel("Year of Joining", col1, y));
            txtYear = MakeTextBox(col1, y + 20, colW);
            card.Controls.Add(txtYear);

            card.Controls.Add(MakeFieldLabel("Organization / Division", col2, y));
            txtOrg = MakeTextBox(col2, y + 20, colW);
            card.Controls.Add(txtOrg);

            y += rowH;

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

            lblMsg = new Label
            {
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 38, 38),
                Location = new Point(35, 615),
                Size = new Size(420, 35),
                AutoSize = false
            };
            this.Controls.Add(lblMsg);

            btnCancel = new Button
            {
                Text = "Cancel",
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                BackColor = Color.FromArgb(241, 245, 249),
                ForeColor = Color.FromArgb(51, 65, 85),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 42),
                Location = new Point(480, 612),
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnCancel.Click += (s, e) => this.Close();
            this.Controls.Add(btnCancel);

            btnSave = new Button
            {
                Text = "💾  SAVE CHANGES",
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                BackColor = Color.FromArgb(251, 121, 43), // IRCTC Orange
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(175, 42),
                Location = new Point(610, 612),
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);
        }

        private Label MakeFieldLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                Location = new Point(x, y),
                Size = new Size(330, 18),
                AutoSize = false
            };
        }

        private TextBox MakeTextBox(int x, int y, int width)
        {
            return new TextBox
            {
                Font = new Font("Segoe UI", 10.5f),
                Location = new Point(x, y),
                Size = new Size(width, 28),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(15, 23, 42),
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        private void LoadUserData()
        {
            DataRow row = authService.GetUserById(userId);
            if (row == null)
            {
                MessageBox.Show("User record not found in database.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            txtFullName.Text = row["FullName"] != DBNull.Value ? row["FullName"].ToString() : "";
            txtUsername.Text = row["Username"] != DBNull.Value ? row["Username"].ToString() : "";
            txtEmail.Text = row["Email"] != DBNull.Value ? row["Email"].ToString() : "";
            txtPhone.Text = row["Phone"] != DBNull.Value ? row["Phone"].ToString() : "";
            txtAddress.Text = row["Address"] != DBNull.Value ? row["Address"].ToString() : "";
            txtCourse.Text = row["Course"] != DBNull.Value ? row["Course"].ToString() : "";
            txtYear.Text = row["YearOfJoining"] != DBNull.Value ? row["YearOfJoining"].ToString() : "";
            txtOrg.Text = row["CollegeOrOrg"] != DBNull.Value ? row["CollegeOrOrg"].ToString() : "";

            if (row["DateOfBirth"] != DBNull.Value && DateTime.TryParse(row["DateOfBirth"].ToString(), out DateTime dob))
            {
                if (dob <= dtpDob.MaxDate && dob >= dtpDob.MinDate)
                    dtpDob.Value = dob;
            }

            string gender = row["Gender"] != DBNull.Value ? row["Gender"].ToString() : "Male";
            int gIdx = cmbGender.FindStringExact(gender);
            cmbGender.SelectedIndex = gIdx >= 0 ? gIdx : 0;

            string dept = row["Department"] != DBNull.Value ? row["Department"].ToString() : "";
            int dIdx = cmbDepartment.FindString(dept);
            cmbDepartment.SelectedIndex = dIdx >= 0 ? dIdx : 0;

            string status = row["Status"] != DBNull.Value ? row["Status"].ToString() : "Active";
            int sIdx = cmbStatus.FindStringExact(status);
            cmbStatus.SelectedIndex = sIdx >= 0 ? sIdx : 0;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            lblMsg.Text = "";
            bool ok = authService.UpdateUser(
                userId,
                txtFullName.Text.Trim(),
                dtpDob.Value,
                cmbGender.SelectedItem?.ToString(),
                txtEmail.Text.Trim(),
                txtPhone.Text.Trim(),
                txtAddress.Text.Trim(),
                cmbDepartment.SelectedItem?.ToString(),
                txtCourse.Text.Trim(),
                txtYear.Text.Trim(),
                txtOrg.Text.Trim(),
                txtNewPassword.Text,
                cmbStatus.SelectedItem?.ToString() ?? "Active",
                out string err);

            if (ok)
            {
                MessageBox.Show("User profile and credentials updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                lblMsg.ForeColor = Color.FromArgb(220, 38, 38);
                lblMsg.Text = "❌ " + err;
            }
        }
    }
}
