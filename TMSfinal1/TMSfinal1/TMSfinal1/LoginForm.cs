using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TMS
{
    /// <summary>
    /// Professional Indian Railways & IRCTC Inspired White/Light Theme Login Portal.
    /// </summary>
    public class LoginForm : Form
    {
        private readonly AuthService authService = new AuthService();
        private bool isAdminMode = false;

        private Panel mainCard;
        private Label lblTitle;
        private Label lblSubtitle;
        private Button btnUserTab;
        private Button btnAdminTab;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private CheckBox chkShowPassword;
        private Button btnLogin;
        private Label lblError;
        private Label lblAdminHint;
        private Button btnExit;

        public LoginForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Indian Railways – Train Management System (TMS) Authentication";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new Size(520, 640);
            this.BackColor = Color.FromArgb(244, 246, 249); // Clean IRCTC page background

            // Top decorative bar (Indian Railways Navy + IRCTC Orange)
            Panel topBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 6,
                BackColor = Color.FromArgb(33, 61, 119) // #213D77
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
                Location = new Point(470, 15),
                Cursor = Cursors.Hand
            };
            btnExit.FlatAppearance.BorderSize = 0;
            btnExit.Click += (s, e) => Application.Exit();
            btnExit.MouseEnter += (s, e) => btnExit.ForeColor = Color.FromArgb(220, 38, 38);
            btnExit.MouseLeave += (s, e) => btnExit.ForeColor = Color.FromArgb(100, 116, 139);
            this.Controls.Add(btnExit);

            // Main White Card Panel
            mainCard = new Panel
            {
                Size = new Size(440, 560),
                Location = new Point(40, 45),
                BackColor = Color.White
            };
            mainCard.Paint += MainCard_Paint;
            this.Controls.Add(mainCard);

            // Logo & Emblem
            Label lblLogo = new Label
            {
                Text = "🚆",
                Font = new Font("Segoe UI Emoji", 26),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(440, 38),
                Location = new Point(0, 16)
            };
            mainCard.Controls.Add(lblLogo);

            lblTitle = new Label
            {
                Text = "INDIAN RAILWAYS",
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 61, 119), // #213D77
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(440, 26),
                Location = new Point(0, 58)
            };
            mainCard.Controls.Add(lblTitle);

            lblSubtitle = new Label
            {
                Text = "Train Management System (TMS) Portal",
                Font = new Font("Segoe UI", 9.5f),
                ForeColor = Color.FromArgb(100, 116, 139),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(440, 22),
                Location = new Point(0, 88)
            };
            mainCard.Controls.Add(lblSubtitle);

            // Tab Segmented Container (User vs Admin)
            Panel tabContainer = new Panel
            {
                Size = new Size(380, 44),
                Location = new Point(30, 126),
                BackColor = Color.FromArgb(241, 245, 249)
            };
            mainCard.Controls.Add(tabContainer);

            btnUserTab = new Button
            {
                Text = "👤 User Login",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(187, 40),
                Location = new Point(2, 2),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(33, 61, 119),
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnUserTab.FlatAppearance.BorderSize = 0;
            btnUserTab.Click += (s, e) => SwitchMode(false);
            tabContainer.Controls.Add(btnUserTab);

            btnAdminTab = new Button
            {
                Text = "🛡️ Admin Login",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(187, 40),
                Location = new Point(191, 2),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(70, 80, 95),
                Cursor = Cursors.Hand
            };
            btnAdminTab.FlatAppearance.BorderSize = 0;
            btnAdminTab.Click += (s, e) => SwitchMode(true);
            tabContainer.Controls.Add(btnAdminTab);

            // Username Label & Input
            Label lblUser = new Label
            {
                Text = "Username",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 61, 119),
                Location = new Point(30, 188),
                Size = new Size(380, 20)
            };
            mainCard.Controls.Add(lblUser);

            txtUsername = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                Location = new Point(30, 210),
                Size = new Size(380, 30),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(30, 41, 59),
                BorderStyle = BorderStyle.FixedSingle
            };
            mainCard.Controls.Add(txtUsername);

            // Password Label & Input
            Label lblPass = new Label
            {
                Text = "Password",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 61, 119),
                Location = new Point(30, 252),
                Size = new Size(380, 20)
            };
            mainCard.Controls.Add(lblPass);

            txtPassword = new TextBox
            {
                Font = new Font("Segoe UI", 11),
                Location = new Point(30, 274),
                Size = new Size(380, 30),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(30, 41, 59),
                UseSystemPasswordChar = true,
                BorderStyle = BorderStyle.FixedSingle
            };
            txtPassword.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) PerformLogin(); };
            txtUsername.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) txtPassword.Focus(); };
            mainCard.Controls.Add(txtPassword);

            // Show Password Checkbox
            chkShowPassword = new CheckBox
            {
                Text = "Show Password",
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(30, 308),
                Size = new Size(160, 22),
                Cursor = Cursors.Hand
            };
            chkShowPassword.CheckedChanged += (s, e) => txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
            mainCard.Controls.Add(chkShowPassword);

            // Forgot Password Link
            Label lnkForgotPassword = new Label
            {
                Name = "lnkForgotPassword",
                Text = "Forgot Password?",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(37, 99, 235), // Royal Blue
                Location = new Point(270, 308),
                Size = new Size(140, 22),
                TextAlign = ContentAlignment.MiddleRight,
                Cursor = Cursors.Hand
            };
            lnkForgotPassword.MouseEnter += (s, e) => lnkForgotPassword.ForeColor = Color.FromArgb(234, 88, 12);
            lnkForgotPassword.MouseLeave += (s, e) => lnkForgotPassword.ForeColor = Color.FromArgb(37, 99, 235);
            lnkForgotPassword.Click += (s, e) => { new ForgotPasswordForm().ShowDialog(); };
            mainCard.Controls.Add(lnkForgotPassword);

            // Error Message
            lblError = new Label
            {
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 38, 38),
                Location = new Point(30, 335),
                Size = new Size(380, 36),
                TextAlign = ContentAlignment.MiddleLeft,
                Text = ""
            };
            mainCard.Controls.Add(lblError);

            // Login Button (IRCTC Vibrant Orange)
            btnLogin = new Button
            {
                Text = "SIGN IN AS USER",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Size = new Size(380, 46),
                Location = new Point(30, 380),
                BackColor = Color.FromArgb(251, 121, 43), // #FB792B IRCTC Orange
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Click += (s, e) => PerformLogin();
            mainCard.Controls.Add(btnLogin);

            // Info Note Box
            Panel notePanel = new Panel
            {
                Location = new Point(30, 442),
                Size = new Size(380, 70),
                BackColor = Color.FromArgb(238, 242, 255)
            };
            mainCard.Controls.Add(notePanel);

            lblAdminHint = new Label
            {
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(51, 65, 85),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Padding = new Padding(10),
                Text = "ℹ️ User accounts are authorized by the Station Administrator. Please use your assigned credentials."
            };
            notePanel.Controls.Add(lblAdminHint);

            // Window drag
            this.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    ThemeManager.ReleaseCapture();
                    ThemeManager.SendMessage(this.Handle, ThemeManager.WM_NCLBUTTONDOWN, ThemeManager.HT_CAPTION, 0);
                }
            };
        }

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

        private void SwitchMode(bool admin)
        {
            isAdminMode = admin;
            lblError.Text = "";
            txtPassword.Text = "";

            Control lnk = mainCard.Controls["lnkForgotPassword"];
            if (lnk != null) lnk.Visible = !admin;

            if (isAdminMode)
            {
                btnAdminTab.BackColor = Color.FromArgb(33, 61, 119); // Indian Railways Navy
                btnAdminTab.ForeColor = Color.White;
                btnUserTab.BackColor = Color.Transparent;
                btnUserTab.ForeColor = Color.FromArgb(70, 80, 95);

                btnLogin.Text = "SIGN IN AS ADMIN";
                btnLogin.BackColor = Color.FromArgb(33, 61, 119);
                lblAdminHint.Text = "🛡️ Initial Setup Admin Credentials:\nUsername: admin  |  Password: Admin@123";
            }
            else
            {
                btnUserTab.BackColor = Color.FromArgb(33, 61, 119);
                btnUserTab.ForeColor = Color.White;
                btnAdminTab.BackColor = Color.Transparent;
                btnAdminTab.ForeColor = Color.FromArgb(70, 80, 95);

                btnLogin.Text = "SIGN IN AS USER";
                btnLogin.BackColor = Color.FromArgb(251, 121, 43); // IRCTC Orange
                lblAdminHint.Text = "ℹ️ User accounts are authorized by the Station Administrator. Please use your assigned credentials.";
            }
        }

        private void PerformLogin()
        {
            lblError.Text = "";
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                lblError.Text = "⚠️ Please enter both username and password.";
                return;
            }

            btnLogin.Enabled = false;
            btnLogin.Text = "Authenticating...";

            try
            {
                if (isAdminMode)
                {
                    if (authService.AuthenticateAdmin(username, password, out string err))
                    {
                        AdminDashboardForm adminDashboard = new AdminDashboardForm();
                        adminDashboard.Show();
                        this.Hide();
                    }
                    else
                    {
                        lblError.Text = "❌ " + err;
                    }
                }
                else
                {
                    if (authService.AuthenticateUser(username, password, out string err))
                    {
                        UserDashboardForm userDashboard = new UserDashboardForm();
                        userDashboard.Show();
                        this.Hide();
                    }
                    else
                    {
                        lblError.Text = "❌ " + err;
                    }
                }
            }
            catch (Exception ex)
            {
                lblError.Text = "⚠️ System error: " + ex.Message;
            }
            finally
            {
                btnLogin.Enabled = true;
                btnLogin.Text = isAdminMode ? "SIGN IN AS ADMIN" : "SIGN IN AS USER";
            }
        }
    }
}
