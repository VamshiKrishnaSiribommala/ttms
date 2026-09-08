using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TMS
{
    /// <summary>
    /// Professional Indian Railways & IRCTC Inspired User Password Recovery Form.
    /// Allows users to securely reset their password by verifying User ID, Mobile Number, and Date of Birth.
    /// All reset events are recorded in TMS_PasswordResetLogs for Administrative Audit.
    /// </summary>
    public class ForgotPasswordForm : Form
    {
        private readonly AuthService authService = new AuthService();

        private Panel mainCard;
        private TextBox txtUsername;
        private TextBox txtPhone;
        private DateTimePicker dtpDob;
        private TextBox txtNewPassword;
        private TextBox txtConfirmPassword;
        private CheckBox chkShowPassword;
        private Button btnReset;
        private Button btnBack;
        private Label lblStatus;
        private Button btnExit;

        public ForgotPasswordForm()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.DoubleBuffered = true;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Indian Railways – User Password Recovery";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new Size(540, 720);
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
                Location = new Point(490, 14),
                Cursor = Cursors.Hand
            };
            btnExit.FlatAppearance.BorderSize = 0;
            btnExit.Click += (s, e) => this.Close();
            btnExit.MouseEnter += (s, e) => btnExit.ForeColor = Color.FromArgb(220, 38, 38);
            btnExit.MouseLeave += (s, e) => btnExit.ForeColor = Color.FromArgb(100, 116, 139);
            this.Controls.Add(btnExit);

            // Main Card Container
            mainCard = new Panel
            {
                Size = new Size(460, 640),
                Location = new Point(40, 45),
                BackColor = Color.White
            };
            mainCard.Paint += MainCard_Paint;
            this.Controls.Add(mainCard);

            // Logo & Titles
            Label lblLogo = new Label
            {
                Text = "🚆",
                Font = new Font("Segoe UI Emoji", 26),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(460, 40),
                Location = new Point(0, 12)
            };
            mainCard.Controls.Add(lblLogo);

            Label lblTitle = new Label
            {
                Text = "RECOVER ACCOUNT PASSWORD",
                Font = new Font("Segoe UI", 13.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 61, 119),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(460, 24),
                Location = new Point(0, 54)
            };
            mainCard.Controls.Add(lblTitle);

            Label lblSub = new Label
            {
                Text = "Verify your registered identity to reset login credentials",
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(100, 116, 139),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(460, 20),
                Location = new Point(0, 78)
            };
            mainCard.Controls.Add(lblSub);

            int startY = 106;
            int gap = 58;

            // 1. Username / User ID
            mainCard.Controls.Add(MakeFieldLabel("User ID / Username *", 30, startY));
            txtUsername = MakeTextBox(30, startY + 20, false);
            mainCard.Controls.Add(txtUsername);

            // 2. Registered Mobile Number
            mainCard.Controls.Add(MakeFieldLabel("Registered Mobile Number *", 30, startY + gap));
            txtPhone = MakeTextBox(30, startY + gap + 20, false);
            mainCard.Controls.Add(txtPhone);

            // 3. Date of Birth
            mainCard.Controls.Add(MakeFieldLabel("Date of Birth (DOB) *", 30, startY + gap * 2));
            dtpDob = new DateTimePicker
            {
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd-MMM-yyyy",
                Font = new Font("Segoe UI", 10.5f),
                Location = new Point(30, startY + gap * 2 + 20),
                Size = new Size(400, 28),
                MaxDate = DateTime.Today,
                Value = new DateTime(1995, 1, 1)
            };
            mainCard.Controls.Add(dtpDob);

            // 4. New Password
            mainCard.Controls.Add(MakeFieldLabel("New Password (min 6 characters) *", 30, startY + gap * 3));
            txtNewPassword = MakeTextBox(30, startY + gap * 3 + 20, true);
            mainCard.Controls.Add(txtNewPassword);

            // 5. Confirm Password
            mainCard.Controls.Add(MakeFieldLabel("Confirm New Password *", 30, startY + gap * 4));
            txtConfirmPassword = MakeTextBox(30, startY + gap * 4 + 20, true);
            mainCard.Controls.Add(txtConfirmPassword);

            // Show Password checkbox
            chkShowPassword = new CheckBox
            {
                Text = "Show Passwords",
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(30, startY + gap * 5 + 4),
                Size = new Size(160, 22),
                Cursor = Cursors.Hand
            };
            chkShowPassword.CheckedChanged += (s, e) =>
            {
                txtNewPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
                txtConfirmPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
            };
            mainCard.Controls.Add(chkShowPassword);

            // Status message label
            lblStatus = new Label
            {
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 38, 38),
                Location = new Point(30, startY + gap * 5 + 28),
                Size = new Size(400, 38),
                TextAlign = ContentAlignment.MiddleLeft,
                Text = ""
            };
            mainCard.Controls.Add(lblStatus);

            // Reset Password Button (Vibrant IRCTC Orange)
            btnReset = new Button
            {
                Text = "🔒 VERIFY & RESET PASSWORD",
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                Size = new Size(400, 44),
                Location = new Point(30, startY + gap * 5 + 68),
                BackColor = Color.FromArgb(251, 121, 43), // #FB792B IRCTC Orange
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnReset.FlatAppearance.BorderSize = 0;
            btnReset.MouseEnter += (s, e) => btnReset.BackColor = Color.FromArgb(234, 88, 12);
            btnReset.MouseLeave += (s, e) => btnReset.BackColor = Color.FromArgb(251, 121, 43);
            btnReset.Click += (s, e) => PerformPasswordReset();
            mainCard.Controls.Add(btnReset);

            // Back to Login Button
            btnBack = new Button
            {
                Text = "➔ Return to Login",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Size = new Size(400, 30),
                Location = new Point(30, startY + gap * 5 + 118),
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(33, 61, 119),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnBack.FlatAppearance.BorderSize = 0;
            btnBack.MouseEnter += (s, e) => btnBack.ForeColor = Color.FromArgb(234, 88, 12);
            btnBack.MouseLeave += (s, e) => btnBack.ForeColor = Color.FromArgb(33, 61, 119);
            btnBack.Click += (s, e) => this.Close();
            mainCard.Controls.Add(btnBack);

            // Admin Notice Info Box
            Panel noticeBox = new Panel
            {
                Location = new Point(30, startY + gap * 5 + 154),
                Size = new Size(400, 52),
                BackColor = Color.FromArgb(238, 242, 255)
            };
            mainCard.Controls.Add(noticeBox);

            Label lblNotice = new Label
            {
                Text = "🛡️ Security Compliance: Password reset events are automatically logged and notified to the Station Administrator.",
                Font = new Font("Segoe UI", 8f),
                ForeColor = Color.FromArgb(30, 58, 138),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Padding = new Padding(6)
            };
            noticeBox.Controls.Add(lblNotice);

            // Window Dragging
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
            Size = new Size(400, 18)
        };

        private TextBox MakeTextBox(int x, int y, bool isPassword) => new TextBox
        {
            Font = new Font("Segoe UI", 10.5f),
            Location = new Point(x, y),
            Size = new Size(400, 28),
            BackColor = Color.White,
            ForeColor = Color.FromArgb(30, 41, 59),
            UseSystemPasswordChar = isPassword,
            BorderStyle = BorderStyle.FixedSingle
        };

        private void MainCard_Paint(object sender, PaintEventArgs e)
        {
            var p = sender as Panel;
            if (p == null) return;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            using (var pen = new Pen(Color.FromArgb(218, 225, 233), 1.5f))
            {
                var rect = new Rectangle(0, 0, p.Width - 1, p.Height - 1);
                int r = 10;
                using (var path = new GraphicsPath())
                {
                    path.AddArc(rect.X, rect.Y, r * 2, r * 2, 180, 90);
                    path.AddArc(rect.Right - r * 2, rect.Y, r * 2, r * 2, 270, 90);
                    path.AddArc(rect.Right - r * 2, rect.Bottom - r * 2, r * 2, r * 2, 0, 90);
                    path.AddArc(rect.X, rect.Bottom - r * 2, r * 2, r * 2, 90, 90);
                    path.CloseFigure();
                    e.Graphics.DrawPath(pen, path);
                }
            }
        }

        private void PerformPasswordReset()
        {
            lblStatus.Text = "";
            string username = txtUsername.Text.Trim();
            string phone = txtPhone.Text.Trim();
            DateTime dob = dtpDob.Value;
            string newPassword = txtNewPassword.Text;
            string confirmPassword = txtConfirmPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(newPassword))
            {
                lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
                lblStatus.Text = "⚠️ Please fill in all required fields.";
                return;
            }

            btnReset.Enabled = false;
            btnReset.Text = "Verifying Credentials...";

            bool success = authService.ResetPasswordByVerification(username, phone, dob, newPassword, confirmPassword, out string errMsg);

            btnReset.Enabled = true;
            btnReset.Text = "🔒 VERIFY & RESET PASSWORD";

            if (success)
            {
                lblStatus.ForeColor = Color.FromArgb(22, 163, 74); // Emerald Green
                lblStatus.Text = "✅ Password successfully updated! An audit event has been notified to the Administrator.";
                MessageBox.Show(
                    "Your password has been successfully reset!\n\nYou can now log in with your new password.\nAn audit notice has been logged for the Station Administrator.",
                    "Password Reset Successful",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                lblStatus.ForeColor = Color.FromArgb(220, 38, 38);
                lblStatus.Text = "❌ " + errMsg;
            }
        }
    }
}
