using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TrainWorkingApp
{
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
            this.Text = "Indian Railways – Train Working Management System (TMS) Authentication";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new Size(520, 640);
            this.BackColor = Color.FromArgb(244, 246, 249); // Clean IRCTC page background

            // Top decorative bar
            Panel topBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 6,
                BackColor = ThemeManager.IRCTCColors.PrimaryNavy
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
                Font = new Font("Segoe UI Emoji", 30),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(440, 50),
                Location = new Point(0, 12)
            };
            mainCard.Controls.Add(lblLogo);

            lblTitle = new Label
            {
                Text = "INDIAN RAILWAYS",
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                ForeColor = ThemeManager.IRCTCColors.PrimaryNavy,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(440, 26),
                Location = new Point(0, 65)
            };
            mainCard.Controls.Add(lblTitle);

            lblSubtitle = new Label
            {
                Text = "Train Working & Operational Authorities System",
                Font = new Font("Segoe UI", 9.5f),
                ForeColor = Color.FromArgb(100, 116, 139),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(440, 20),
                Location = new Point(0, 93)
            };
            mainCard.Controls.Add(lblSubtitle);

            // Tabs for Role Selection (User vs. Admin)
            Panel tabContainer = new Panel
            {
                Size = new Size(360, 40),
                Location = new Point(40, 125),
                BackColor = Color.FromArgb(241, 245, 249)
            };
            mainCard.Controls.Add(tabContainer);

            btnUserTab = new Button
            {
                Text = "👤 OPERATOR / USER",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Size = new Size(180, 40),
                Location = new Point(0, 0),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = ThemeManager.IRCTCColors.PrimaryNavy,
                Cursor = Cursors.Hand
            };
            btnUserTab.FlatAppearance.BorderSize = 0;
            btnUserTab.Click += (s, e) => SetMode(false);
            tabContainer.Controls.Add(btnUserTab);

            btnAdminTab = new Button
            {
                Text = "🛡️ ADMIN",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Size = new Size(180, 40),
                Location = new Point(180, 0),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.FromArgb(100, 116, 139),
                Cursor = Cursors.Hand
            };
            btnAdminTab.FlatAppearance.BorderSize = 0;
            btnAdminTab.Click += (s, e) => SetMode(true);
            tabContainer.Controls.Add(btnAdminTab);

            int startY = 180;

            // Username Field
            Label lblUser = new Label
            {
                Text = "Username or Employee ID",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                Location = new Point(40, startY),
                AutoSize = true
            };
            mainCard.Controls.Add(lblUser);

            txtUsername = new TextBox
            {
                Font = new Font("Segoe UI", 11f),
                Size = new Size(360, 32),
                Location = new Point(40, startY + 22),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 250, 252)
            };
            mainCard.Controls.Add(txtUsername);

            // Password Field
            Label lblPass = new Label
            {
                Text = "Password",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                Location = new Point(40, startY + 68),
                AutoSize = true
            };
            mainCard.Controls.Add(lblPass);

            txtPassword = new TextBox
            {
                Font = new Font("Segoe UI", 11f),
                Size = new Size(360, 32),
                Location = new Point(40, startY + 90),
                PasswordChar = '●',
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 250, 252)
            };
            mainCard.Controls.Add(txtPassword);

            // Show Password Checkbox
            chkShowPassword = new CheckBox
            {
                Text = "Show Password",
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(40, startY + 128),
                AutoSize = true,
                Cursor = Cursors.Hand
            };
            chkShowPassword.CheckedChanged += (s, e) =>
            {
                txtPassword.PasswordChar = chkShowPassword.Checked ? '\0' : '●';
            };
            mainCard.Controls.Add(chkShowPassword);

            // Forgot Password Link
            Label lblForgot = new Label
            {
                Text = "Forgot Password?",
                Font = new Font("Segoe UI", 8.5f, FontStyle.Underline),
                ForeColor = ThemeManager.IRCTCColors.PrimaryNavy,
                Location = new Point(290, startY + 128),
                AutoSize = true,
                Cursor = Cursors.Hand
            };
            lblForgot.Click += (s, e) =>
            {
                new ForgotPasswordForm().ShowDialog();
            };
            mainCard.Controls.Add(lblForgot);

            // Error Message
            lblError = new Label
            {
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 38, 38),
                Location = new Point(40, startY + 152),
                Size = new Size(360, 34),
                TextAlign = ContentAlignment.MiddleCenter,
                Text = ""
            };
            mainCard.Controls.Add(lblError);

            // Login Button
            btnLogin = new Button
            {
                Text = "SIGN IN AS OPERATOR",
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                Size = new Size(360, 42),
                Location = new Point(40, startY + 190),
                BackColor = ThemeManager.IRCTCColors.ActionOrange,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.MouseEnter += (s, e) => btnLogin.BackColor = ThemeManager.IRCTCColors.OrangeHover;
            btnLogin.MouseLeave += (s, e) => btnLogin.BackColor = ThemeManager.IRCTCColors.ActionOrange;
            btnLogin.Click += BtnLogin_Click;
            mainCard.Controls.Add(btnLogin);

            // Info / Hint Label
            lblAdminHint = new Label
            {
                Text = "Operator credentials are provided by Administrator only",
                Font = new Font("Segoe UI", 8.5f, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 116, 139),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(360, 20),
                Location = new Point(40, startY + 240),
                Cursor = Cursors.Default
            };
            mainCard.Controls.Add(lblAdminHint);

            this.AcceptButton = btnLogin;
        }

        private void SetMode(bool admin)
        {
            isAdminMode = admin;
            lblError.Text = "";
            txtUsername.Text = "";
            txtPassword.Text = "";

            if (admin)
            {
                btnAdminTab.BackColor = Color.White;
                btnAdminTab.ForeColor = ThemeManager.IRCTCColors.PrimaryNavy;
                btnUserTab.BackColor = Color.Transparent;
                btnUserTab.ForeColor = Color.FromArgb(100, 116, 139);

                btnLogin.Text = "SIGN IN AS ADMIN";
                lblAdminHint.Text = "Default credentials: admin / Admin@123";
                lblAdminHint.Cursor = Cursors.Default;
            }
            else
            {
                btnUserTab.BackColor = Color.White;
                btnUserTab.ForeColor = ThemeManager.IRCTCColors.PrimaryNavy;
                btnAdminTab.BackColor = Color.Transparent;
                btnAdminTab.ForeColor = Color.FromArgb(100, 116, 139);

                btnLogin.Text = "SIGN IN AS OPERATOR";
                lblAdminHint.Text = "Operator credentials are provided by Administrator only";
                lblAdminHint.Cursor = Cursors.Default;
            }
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string u = txtUsername.Text.Trim();
            string p = txtPassword.Text;

            if (isAdminMode)
            {
                if (authService.AuthenticateAdmin(u, p, out string err))
                {
                    this.Hide();
                    AdminDashboardForm adminForm = new AdminDashboardForm();
                    adminForm.FormClosed += (s, args) => { this.Show(); txtPassword.Text = ""; };
                    adminForm.Show();
                }
                else
                {
                    lblError.Text = "❌ " + err;
                }
            }
            else
            {
                if (authService.AuthenticateUser(u, p, out string err))
                {
                    this.Hide();
                    UserDashboardForm userForm = new UserDashboardForm();
                    userForm.FormClosed += (s, args) => { this.Show(); txtPassword.Text = ""; };
                    userForm.Show();
                }
                else
                {
                    lblError.Text = "❌ " + err;
                }
            }
        }

        private void MainCard_Paint(object sender, PaintEventArgs e)
        {
            using (Pen borderPen = new Pen(Color.FromArgb(226, 232, 240), 1))
            {
                e.Graphics.DrawRectangle(borderPen, 0, 0, mainCard.Width - 1, mainCard.Height - 1);
            }
        }
    }
}
