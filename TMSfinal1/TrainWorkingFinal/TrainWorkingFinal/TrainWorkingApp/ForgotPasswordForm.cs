using System;
using System.Drawing;
using System.Windows.Forms;

namespace TrainWorkingApp
{
    public class ForgotPasswordForm : Form
    {
        private readonly AuthService authService = new AuthService();

        private Panel mainCard;
        private TextBox txtUsername;
        private TextBox txtPhone;
        private DateTimePicker dtpDob;
        private TextBox txtNewPassword;
        private TextBox txtConfirmPassword;
        private Button btnReset;
        private Button btnExit;
        private Label lblStatus;

        public ForgotPasswordForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Indian Railways – Operator Password Recovery";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new Size(500, 600);
            this.BackColor = Color.FromArgb(244, 246, 249);

            Panel topBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 6,
                BackColor = ThemeManager.IRCTCColors.PrimaryNavy
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
                Location = new Point(450, 12),
                Cursor = Cursors.Hand
            };
            btnExit.FlatAppearance.BorderSize = 0;
            btnExit.Click += (s, e) => this.Close();
            this.Controls.Add(btnExit);

            mainCard = new Panel
            {
                Size = new Size(420, 520),
                Location = new Point(40, 40),
                BackColor = Color.White
            };
            this.Controls.Add(mainCard);

            Label lblTitle = new Label
            {
                Text = "🔐 Password Recovery",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = ThemeManager.IRCTCColors.PrimaryNavy,
                Location = new Point(30, 20),
                AutoSize = true
            };
            mainCard.Controls.Add(lblTitle);

            Label lblSub = new Label
            {
                Text = "Verify your registered identity details to reset password.",
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(32, 48),
                AutoSize = true
            };
            mainCard.Controls.Add(lblSub);

            int y = 80;

            Label lblU = new Label { Text = "Username *", Font = new Font("Segoe UI", 9f, FontStyle.Bold), Location = new Point(30, y), AutoSize = true };
            txtUsername = new TextBox { Location = new Point(30, y + 20), Size = new Size(360, 26), Font = new Font("Segoe UI", 10f), BorderStyle = BorderStyle.FixedSingle };
            mainCard.Controls.Add(lblU);
            mainCard.Controls.Add(txtUsername);
            y += 55;

            Label lblP = new Label { Text = "Registered Mobile Number *", Font = new Font("Segoe UI", 9f, FontStyle.Bold), Location = new Point(30, y), AutoSize = true };
            txtPhone = new TextBox { Location = new Point(30, y + 20), Size = new Size(360, 26), Font = new Font("Segoe UI", 10f), BorderStyle = BorderStyle.FixedSingle };
            mainCard.Controls.Add(lblP);
            mainCard.Controls.Add(txtPhone);
            y += 55;

            Label lblDob = new Label { Text = "Date of Birth *", Font = new Font("Segoe UI", 9f, FontStyle.Bold), Location = new Point(30, y), AutoSize = true };
            dtpDob = new DateTimePicker { Location = new Point(30, y + 20), Size = new Size(360, 26), Font = new Font("Segoe UI", 10f), Format = DateTimePickerFormat.Short };
            mainCard.Controls.Add(lblDob);
            mainCard.Controls.Add(dtpDob);
            y += 55;

            Label lblNp = new Label { Text = "New Password *", Font = new Font("Segoe UI", 9f, FontStyle.Bold), Location = new Point(30, y), AutoSize = true };
            txtNewPassword = new TextBox { Location = new Point(30, y + 20), Size = new Size(360, 26), Font = new Font("Segoe UI", 10f), PasswordChar = '●', BorderStyle = BorderStyle.FixedSingle };
            mainCard.Controls.Add(lblNp);
            mainCard.Controls.Add(txtNewPassword);
            y += 55;

            Label lblCp = new Label { Text = "Confirm New Password *", Font = new Font("Segoe UI", 9f, FontStyle.Bold), Location = new Point(30, y), AutoSize = true };
            txtConfirmPassword = new TextBox { Location = new Point(30, y + 20), Size = new Size(360, 26), Font = new Font("Segoe UI", 10f), PasswordChar = '●', BorderStyle = BorderStyle.FixedSingle };
            mainCard.Controls.Add(lblCp);
            mainCard.Controls.Add(txtConfirmPassword);
            y += 60;

            lblStatus = new Label
            {
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 38, 38),
                Location = new Point(30, y),
                Size = new Size(360, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };
            mainCard.Controls.Add(lblStatus);
            y += 24;

            btnReset = new Button
            {
                Text = "CONFIRM & RESET PASSWORD",
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Size = new Size(360, 40),
                Location = new Point(30, y),
                BackColor = ThemeManager.IRCTCColors.ActionOrange,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnReset.FlatAppearance.BorderSize = 0;
            btnReset.Click += BtnReset_Click;
            mainCard.Controls.Add(btnReset);
        }

        private void BtnReset_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPhone.Text) || string.IsNullOrWhiteSpace(txtNewPassword.Text))
            {
                lblStatus.Text = "❌ Please fill all required fields.";
                return;
            }

            if (txtNewPassword.Text != txtConfirmPassword.Text)
            {
                lblStatus.Text = "❌ New passwords do not match.";
                return;
            }

            if (authService.ResetPassword(txtUsername.Text.Trim(), txtPhone.Text.Trim(), dtpDob.Value, txtNewPassword.Text, out string err))
            {
                MessageBox.Show("Password successfully reset! You may now sign in with your new credentials.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            else
            {
                lblStatus.Text = "❌ " + err;
            }
        }
    }
}
