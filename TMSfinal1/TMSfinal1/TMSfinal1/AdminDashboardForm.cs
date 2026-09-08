using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TMS
{
    public class AdminDashboardForm : Form
    {
        private readonly AuthService authService = new AuthService();

        // Layout panels
        private Panel headerPanel;
        private Panel navPanel;
        private Panel contentPanel;

        // Sub-view panels
        private Panel pnlOverview;
        private Panel pnlAddUser;
        private Panel pnlManageUsers;
        private Panel pnlResetLogs;

        // Nav Buttons
        private Button btnNavOverview;
        private Button btnNavAddUser;
        private Button btnNavManageUsers;
        private Button btnNavResetLogs;

        // Add User Controls
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
        private TextBox txtPassword;
        private TextBox txtConfirmPassword;
        private ComboBox cmbStatus;
        private Label lblAddUserMsg;

        // Manage Users Controls
        private TextBox txtSearch;
        private DataGridView dgvUsers;
        private Label lblSelectedUser;
        private int selectedUserId = 0;
        private string selectedUserStatus = "";

        // Stats labels
        private Label lblTotalUsers;
        private Label lblActiveUsers;
        private Label lblInactiveUsers;
        private Label lblResetLogsCount;

        // Reset Logs Controls
        private TextBox txtSearchResetLogs;
        private DataGridView dgvResetLogs;

        public AdminDashboardForm()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.DoubleBuffered = true;
            InitializeComponent();
            LoadUserData();
            ShowPanel(pnlOverview, btnNavOverview);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // WS_EX_COMPOSITED: Paints all descendants off-screen in one pass, eliminating flicker/blinking
                return cp;
            }
        }

        private void InitializeComponent()
        {
            this.Text = "TMS – Admin Dashboard";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MinimumSize = new Size(1100, 720);
            this.BackColor = Color.FromArgb(241, 245, 249);

            // WinForms docking order: Fill first, Left second, Top last
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(241, 245, 249),
                AutoScroll = true
            };
            this.Controls.Add(contentPanel);

            navPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 220,
                BackColor = Color.FromArgb(30, 41, 59)
            };
            this.Controls.Add(navPanel);

            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.FromArgb(33, 61, 119) // #213D77 Indian Railways Navy
            };
            this.Controls.Add(headerPanel);

            // ── Header contents ──────────────────────────────────────────────
            Label lblAppTitle = new Label
            {
                Text = "TRAIN MANAGEMENT SYSTEM – ADMIN PORTAL",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 12),
                AutoSize = true
            };
            headerPanel.Controls.Add(lblAppTitle);

            Label lblSub = new Label
            {
                Text = "Indian Railways  •  Central User Administration & Authorization Portal",
                Font = new Font("Segoe UI", 9f, FontStyle.Italic),
                ForeColor = Color.FromArgb(210, 230, 255),
                Location = new Point(20, 40),
                AutoSize = true
            };
            headerPanel.Controls.Add(lblSub);

            Button btnLogout = new Button
            {
                Text = "🚪 Logout",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                BackColor = Color.FromArgb(220, 38, 38), // Crimson Red
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(115, 38),
                Cursor = Cursors.Hand
            };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.Click += BtnLogout_Click;
            headerPanel.Controls.Add(btnLogout);

            Button btnOpenTMS = new Button
            {
                Text = "🚆 Open TMS",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                BackColor = Color.FromArgb(251, 121, 43), // #FB792B IRCTC Orange
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(130, 38),
                Cursor = Cursors.Hand
            };
            btnOpenTMS.FlatAppearance.BorderSize = 0;
            btnOpenTMS.Click += (s, e) => NavigateToMain();
            headerPanel.Controls.Add(btnOpenTMS);

            Label lblAdminName = new Label
            {
                Text = $"🛡️ {SessionManager.CurrentFullName} (Admin)",
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 215, 0),
                AutoSize = true
            };
            headerPanel.Controls.Add(lblAdminName);

            Action layoutHeader = () =>
            {
                int r = headerPanel.ClientSize.Width - 15;
                btnLogout.Location = new Point(r - btnLogout.Width, 16);
                btnOpenTMS.Location = new Point(r - btnLogout.Width - btnOpenTMS.Width - 10, 16);
                lblAdminName.Location = new Point(r - btnLogout.Width - btnOpenTMS.Width - 10 - lblAdminName.Width - 18, 25);
            };
            headerPanel.Resize += (s, e) => layoutHeader();
            this.Shown += (s, e) => layoutHeader();

            // ── Nav Sidebar ──────────────────────────────────────────────────
            Label lblNavTitle = new Label
            {
                Text = "ADMIN CONTROLS",
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(18, 22),
                AutoSize = true
            };
            navPanel.Controls.Add(lblNavTitle);

            btnNavOverview = CreateNavBtn("📊  Dashboard", 55);
            btnNavOverview.Click += (s, e) => ShowPanel(pnlOverview, btnNavOverview);
            navPanel.Controls.Add(btnNavOverview);

            btnNavAddUser = CreateNavBtn("➕  Create New User", 110);
            btnNavAddUser.Click += (s, e) => ShowPanel(pnlAddUser, btnNavAddUser);
            navPanel.Controls.Add(btnNavAddUser);

            btnNavManageUsers = CreateNavBtn("👥  Manage Users", 165);
            btnNavManageUsers.Click += (s, e) => { LoadUserData(); ShowPanel(pnlManageUsers, btnNavManageUsers); };
            navPanel.Controls.Add(btnNavManageUsers);

            btnNavResetLogs = CreateNavBtn("🔑  Password Logs", 220);
            btnNavResetLogs.Click += (s, e) => { LoadResetLogs(); ShowPanel(pnlResetLogs, btnNavResetLogs); };
            navPanel.Controls.Add(btnNavResetLogs);

            // ── Build Panels ─────────────────────────────────────────────────
            BuildOverviewPanel();
            BuildAddUserPanel();
            BuildManageUsersPanel();
            BuildResetLogsPanel();
        }

        private Button currentActiveBtn = null;

        private Button CreateNavBtn(string text, int top)
        {
            var btn = new Button
            {
                Text = text,
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(16, 0, 0, 0),
                ForeColor = Color.FromArgb(203, 213, 225),
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(220, 48),
                Location = new Point(0, top),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(45, 60, 85);
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(30, 58, 138);

            btn.MouseEnter += (s, e) =>
            {
                if (btn != currentActiveBtn)
                {
                    btn.BackColor = Color.FromArgb(45, 60, 85);
                    btn.ForeColor = Color.White;
                }
            };
            btn.MouseLeave += (s, e) =>
            {
                if (btn != currentActiveBtn)
                {
                    btn.BackColor = Color.Transparent;
                    btn.ForeColor = Color.FromArgb(203, 213, 225);
                }
            };
            return btn;
        }

        private void ShowPanel(Panel target, Button activeBtn)
        {
            currentActiveBtn = activeBtn;

            pnlOverview.Visible = false;
            pnlAddUser.Visible = false;
            pnlManageUsers.Visible = false;
            pnlResetLogs.Visible = false;

            Button[] allNav = { btnNavOverview, btnNavAddUser, btnNavManageUsers, btnNavResetLogs };
            foreach (var b in allNav)
            {
                if (b != null)
                {
                    if (b == activeBtn)
                    {
                        b.BackColor = Color.FromArgb(30, 58, 138); // Modern vibrant active navy #1E3A8A
                        b.ForeColor = Color.White;
                        b.FlatAppearance.MouseOverBackColor = Color.FromArgb(37, 99, 235);
                    }
                    else
                    {
                        b.BackColor = Color.Transparent;
                        b.ForeColor = Color.FromArgb(203, 213, 225);
                        b.FlatAppearance.MouseOverBackColor = Color.FromArgb(45, 60, 85);
                    }
                }
            }

            target.Visible = true;
        }

        private Label MakeLabel(string text, Font font, Color fore, Point loc, Size size)
            => new Label { Text = text, Font = font, ForeColor = fore, Location = loc, Size = size, AutoSize = false };

        #region Overview Panel

        private void BuildOverviewPanel()
        {
            pnlOverview = new Panel { Dock = DockStyle.Fill, AutoScroll = true, BackColor = Color.FromArgb(241, 245, 249) };
            contentPanel.Controls.Add(pnlOverview);

            pnlOverview.Controls.Add(MakeLabel("Dashboard Overview", new Font("Segoe UI", 18, FontStyle.Bold),
                Color.FromArgb(15, 23, 42), new Point(30, 20), new Size(500, 38)));

            int cardY = 72;
            int cardW = 200; int cardH = 100; int cardGap = 20; int startX = 30;

            lblTotalUsers = new Label();
            pnlOverview.Controls.Add(MakeStatCard("TOTAL USERS", lblTotalUsers, Color.FromArgb(59, 130, 246), startX, cardY, cardW, cardH));

            lblActiveUsers = new Label();
            pnlOverview.Controls.Add(MakeStatCard("ACTIVE ACCOUNTS", lblActiveUsers, Color.FromArgb(16, 185, 129), startX + cardW + cardGap, cardY, cardW, cardH));

            lblInactiveUsers = new Label();
            pnlOverview.Controls.Add(MakeStatCard("INACTIVE / LOCKED", lblInactiveUsers, Color.FromArgb(239, 68, 68), startX + (cardW + cardGap) * 2, cardY, cardW, cardH));

            lblResetLogsCount = new Label();
            pnlOverview.Controls.Add(MakeStatCard("PASSWORD RESETS", lblResetLogsCount, Color.FromArgb(245, 158, 11), startX + (cardW + cardGap) * 3, cardY, cardW, cardH));

            int qaY = cardY + cardH + 30;
            var grp = new GroupBox
            {
                Text = "  Quick Actions  ",
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                Location = new Point(30, qaY),
                Size = new Size(860, 140),
                BackColor = Color.White
            };
            pnlOverview.Controls.Add(grp);

            Button qBtn(string t, Color c, int x, int w = 190) => new Button
            {
                Text = t, Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                BackColor = c, ForeColor = Color.White, FlatStyle = FlatStyle.Flat,
                Size = new Size(w, 55), Location = new Point(x, 48), Cursor = Cursors.Hand
            };

            var b1 = qBtn("➕ Register User", Color.FromArgb(16, 185, 129), 20);
            b1.FlatAppearance.BorderSize = 0;
            b1.Click += (s, e) => ShowPanel(pnlAddUser, btnNavAddUser);
            grp.Controls.Add(b1);

            var b2 = qBtn("👥 View Accounts", Color.FromArgb(59, 130, 246), 225);
            b2.FlatAppearance.BorderSize = 0;
            b2.Click += (s, e) => { LoadUserData(); ShowPanel(pnlManageUsers, btnNavManageUsers); };
            grp.Controls.Add(b2);

            var b3 = qBtn("🔑 Password Logs", Color.FromArgb(245, 158, 11), 430);
            b3.FlatAppearance.BorderSize = 0;
            b3.Click += (s, e) => { LoadResetLogs(); ShowPanel(pnlResetLogs, btnNavResetLogs); };
            grp.Controls.Add(b3);

            var b4 = qBtn("🚆 Open Registers", Color.FromArgb(139, 92, 246), 635);
            b4.FlatAppearance.BorderSize = 0;
            b4.Click += (s, e) => NavigateToMain();
            grp.Controls.Add(b4);

            int noteY = qaY + 160;
            var noteBox = new Panel
            {
                Location = new Point(30, noteY),
                Size = new Size(780, 190),
                BackColor = Color.FromArgb(238, 242, 255)
            };
            pnlOverview.Controls.Add(noteBox);

            noteBox.Controls.Add(MakeLabel("📌 Administrative Security Policy", new Font("Segoe UI", 11, FontStyle.Bold),
                Color.FromArgb(30, 27, 75), new Point(15, 15), new Size(740, 25)));

            noteBox.Controls.Add(MakeLabel(
                "• Only the Station Administrator can create, update, and activate user accounts.\n" +
                "• Passwords are encrypted using salted PBKDF2 cryptography.\n" +
                "• Users cannot register their own accounts; credentials must match Admin database records.\n" +
                "• Inactive accounts are blocked immediately from signing in to the TMS registers.\n" +
                "• All operations in station registers will record the authenticated username.",
                new Font("Segoe UI", 10), Color.FromArgb(51, 65, 85),
                new Point(15, 48), new Size(740, 125)));
        }

        private Panel MakeStatCard(string title, Label valueLabel, Color accent, int x, int y, int w, int h)
        {
            var card = new Panel { Location = new Point(x, y), Size = new Size(w, h), BackColor = Color.White };
            card.Controls.Add(new Panel { Dock = DockStyle.Left, Width = 5, BackColor = accent });

            card.Controls.Add(MakeLabel(title, new Font("Segoe UI", 8.5f, FontStyle.Bold),
                Color.FromArgb(100, 116, 139), new Point(16, 14), new Size(w - 20, 18)));

            valueLabel.Text = "0";
            valueLabel.Font = new Font("Segoe UI", 24, FontStyle.Bold);
            valueLabel.ForeColor = Color.FromArgb(15, 23, 42);
            valueLabel.Location = new Point(16, 38);
            valueLabel.Size = new Size(w - 20, 48);
            card.Controls.Add(valueLabel);
            return card;
        }

        #endregion

        #region Add User Panel (Fixed pixel layout for crisp, clear visibility)

        private void BuildAddUserPanel()
        {
            pnlAddUser = new Panel { Dock = DockStyle.Fill, AutoScroll = true, BackColor = Color.FromArgb(241, 245, 249) };
            contentPanel.Controls.Add(pnlAddUser);

            pnlAddUser.Controls.Add(MakeLabel("Create New User Account", new Font("Segoe UI", 18, FontStyle.Bold),
                Color.FromArgb(15, 23, 42), new Point(30, 18), new Size(500, 35)));

            pnlAddUser.Controls.Add(MakeLabel(
                "Enter employee details and create login credentials. User can log in once authorized.",
                new Font("Segoe UI", 9.5f), Color.FromArgb(100, 116, 139), new Point(30, 55), new Size(800, 22)));

            // Form container card
            var formCard = new Panel
            {
                Location = new Point(30, 85),
                Size = new Size(880, 640),
                BackColor = Color.White,
                BorderStyle = BorderStyle.None
            };
            pnlAddUser.Controls.Add(formCard);

            int col1 = 30;
            int col2 = 460;
            int colW = 390;
            int y = 20;
            int gap = 62;

            // Row 1: Full Name & DOB
            formCard.Controls.Add(MakeFieldLabel("Full Name *", col1, y));
            txtFullName = MakeInputBox(col1, y + 22, colW);
            formCard.Controls.Add(txtFullName);

            formCard.Controls.Add(MakeFieldLabel("Date of Birth", col2, y));
            dtpDob = new DateTimePicker
            {
                Location = new Point(col2, y + 22),
                Size = new Size(colW, 30),
                Font = new Font("Segoe UI", 11),
                Format = DateTimePickerFormat.Short
            };
            formCard.Controls.Add(dtpDob);

            // Row 2: Gender & Email
            y += gap;
            formCard.Controls.Add(MakeFieldLabel("Gender", col1, y));
            cmbGender = new ComboBox
            {
                Location = new Point(col1, y + 22),
                Size = new Size(colW, 30),
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbGender.Items.AddRange(new object[] { "Male", "Female", "Other" });
            cmbGender.SelectedIndex = 0;
            formCard.Controls.Add(cmbGender);

            formCard.Controls.Add(MakeFieldLabel("Email Address *", col2, y));
            txtEmail = MakeInputBox(col2, y + 22, colW);
            formCard.Controls.Add(txtEmail);

            // Row 3: Phone & Address
            y += gap;
            formCard.Controls.Add(MakeFieldLabel("Phone Number *", col1, y));
            txtPhone = MakeInputBox(col1, y + 22, colW);
            formCard.Controls.Add(txtPhone);

            formCard.Controls.Add(MakeFieldLabel("Address / Station Quarter", col2, y));
            txtAddress = MakeInputBox(col2, y + 22, colW);
            formCard.Controls.Add(txtAddress);

            // Row 4: Department & Course
            y += gap;
            formCard.Controls.Add(MakeFieldLabel("Department *", col1, y));
            cmbDepartment = new ComboBox
            {
                Location = new Point(col1, y + 22),
                Size = new Size(colW, 30),
                Font = new Font("Segoe UI", 10.5f),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbDepartment.Items.AddRange(new object[] {
                "Operating (Station Master / Traffic)",
                "Signalling & Telecom (S&T)",
                "Safety & Inspection",
                "Civil Engineering",
                "Mechanical & Rolling Stock",
                "Electrical & Power Supply",
                "Commercial"
            });
            cmbDepartment.SelectedIndex = 0;
            formCard.Controls.Add(cmbDepartment);

            formCard.Controls.Add(MakeFieldLabel("Course / Designation", col2, y));
            txtCourse = MakeInputBox(col2, y + 22, colW);
            txtCourse.Text = "Station Master";
            formCard.Controls.Add(txtCourse);

            // Row 5: Year & Org
            y += gap;
            formCard.Controls.Add(MakeFieldLabel("Year of Joining", col1, y));
            txtYear = MakeInputBox(col1, y + 22, colW);
            txtYear.Text = DateTime.Now.Year.ToString();
            formCard.Controls.Add(txtYear);

            formCard.Controls.Add(MakeFieldLabel("Organization / Division", col2, y));
            txtOrg = MakeInputBox(col2, y + 22, colW);
            txtOrg.Text = "Indian Railways – South Central Division";
            formCard.Controls.Add(txtOrg);

            // Section separator: Login Credentials
            y += gap;
            var sepLine = new Label
            {
                Text = "─── Login Credentials & Account Status ───",
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 51, 102),
                Location = new Point(col1, y),
                Size = new Size(820, 24),
                TextAlign = ContentAlignment.MiddleCenter
            };
            formCard.Controls.Add(sepLine);

            // Row 6: Username & Status
            y += gap - 15;
            formCard.Controls.Add(MakeFieldLabel("Username *", col1, y));
            txtUsername = MakeInputBox(col1, y + 22, colW);
            formCard.Controls.Add(txtUsername);

            formCard.Controls.Add(MakeFieldLabel("Account Status", col2, y));
            cmbStatus = new ComboBox
            {
                Location = new Point(col2, y + 22),
                Size = new Size(colW, 30),
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbStatus.Items.AddRange(new object[] { "Active", "Inactive" });
            cmbStatus.SelectedIndex = 0;
            formCard.Controls.Add(cmbStatus);

            // Row 7: Password & Confirm Password
            y += gap;
            formCard.Controls.Add(MakeFieldLabel("Password (min 6 characters) *", col1, y));
            txtPassword = MakeInputBox(col1, y + 22, colW);
            txtPassword.UseSystemPasswordChar = true;
            formCard.Controls.Add(txtPassword);

            formCard.Controls.Add(MakeFieldLabel("Confirm Password *", col2, y));
            txtConfirmPassword = MakeInputBox(col2, y + 22, colW);
            txtConfirmPassword.UseSystemPasswordChar = true;
            formCard.Controls.Add(txtConfirmPassword);

            // Row 8: Error label & Action button
            y += gap + 5;
            lblAddUserMsg = new Label
            {
                Location = new Point(col1, y),
                Size = new Size(450, 45),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 38, 38)
            };
            formCard.Controls.Add(lblAddUserMsg);

            Button btnCreate = new Button
            {
                Text = "💾  CREATE & AUTHORIZE USER",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(251, 121, 43), // #FB792B IRCTC Orange
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(330, 46),
                Location = new Point(col2 + 60, y),
                Cursor = Cursors.Hand
            };
            btnCreate.FlatAppearance.BorderSize = 0;
            btnCreate.Click += BtnCreateUser_Click;
            formCard.Controls.Add(btnCreate);
        }

        private Label MakeFieldLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                Location = new Point(x, y),
                Size = new Size(380, 20),
                AutoSize = false
            };
        }

        private TextBox MakeInputBox(int x, int y, int width)
        {
            return new TextBox
            {
                Font = new Font("Segoe UI", 11),
                Location = new Point(x, y),
                Size = new Size(width, 30),
                BackColor = Color.White,
                ForeColor = Color.FromArgb(15, 23, 42),
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        private void BtnCreateUser_Click(object sender, EventArgs e)
        {
            lblAddUserMsg.Text = "";
            bool ok = authService.CreateUser(
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
                txtUsername.Text.Trim(),
                txtPassword.Text,
                txtConfirmPassword.Text,
                cmbStatus.SelectedItem?.ToString() ?? "Active",
                out string err);

            if (ok)
            {
                MessageBox.Show($"User '{txtUsername.Text.Trim()}' ({txtFullName.Text.Trim()}) created and authorized successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearAddUserForm();
                LoadUserData();
                ShowPanel(pnlManageUsers, btnNavManageUsers);
            }
            else
            {
                lblAddUserMsg.ForeColor = Color.FromArgb(220, 38, 38);
                lblAddUserMsg.Text = "❌ " + err;
            }
        }

        private void ClearAddUserForm()
        {
            txtFullName.Text = txtEmail.Text = txtPhone.Text = txtAddress.Text =
            txtUsername.Text = txtPassword.Text = txtConfirmPassword.Text = "";
            lblAddUserMsg.Text = "";
        }

        #endregion

        #region Manage Users Panel

        private string currentStatusFilter = "All";
        private Button btnFilterAll;
        private Button btnFilterActive;
        private Button btnFilterInactive;
        private Button btnEditUser;
        private Button btnToggle;

        private void BuildManageUsersPanel()
        {
            pnlManageUsers = new Panel { Dock = DockStyle.Fill, AutoScroll = false, BackColor = Color.FromArgb(241, 245, 249) };
            contentPanel.Controls.Add(pnlManageUsers);

            pnlManageUsers.Controls.Add(MakeLabel("Registered Users Directory", new Font("Segoe UI", 17, FontStyle.Bold),
                Color.FromArgb(15, 23, 42), new Point(30, 18), new Size(600, 36)));

            // ── 1. Status Filter Tabs Bar (All / Active / Inactive) ──────────
            Panel tabFilterBar = new Panel
            {
                Location = new Point(30, 68),
                Size = new Size(880, 40),
                BackColor = Color.Transparent
            };
            pnlManageUsers.Controls.Add(tabFilterBar);

            btnFilterAll = CreateFilterTabBtn("📋 All Accounts (0)", 0, 175, "All", Color.FromArgb(33, 61, 119));
            tabFilterBar.Controls.Add(btnFilterAll);

            btnFilterActive = CreateFilterTabBtn("🟢 Active Accounts (0)", 185, 200, "Active", Color.FromArgb(22, 163, 74));
            tabFilterBar.Controls.Add(btnFilterActive);

            btnFilterInactive = CreateFilterTabBtn("🔴 Inactive / Disabled (0)", 395, 230, "Inactive", Color.FromArgb(220, 38, 38));
            tabFilterBar.Controls.Add(btnFilterInactive);

            // ── 2. Action & Search Toolbar ────────────────────────────────────
            var toolbar = new Panel { Location = new Point(30, 120), Height = 48, BackColor = Color.White };
            toolbar.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(226, 232, 240), 1))
                    e.Graphics.DrawRectangle(pen, 0, 0, toolbar.Width - 1, toolbar.Height - 1);
            };
            pnlManageUsers.Controls.Add(toolbar);

            toolbar.Controls.Add(MakeLabel("🔍 Search:", new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Color.FromArgb(51, 65, 85), new Point(12, 13), new Size(75, 22)));

            txtSearch = new TextBox
            {
                Location = new Point(90, 10),
                Size = new Size(220, 28),
                Font = new Font("Segoe UI", 10.5f),
                BorderStyle = BorderStyle.FixedSingle
            };
            txtSearch.TextChanged += (s, e) => LoadUserData(txtSearch.Text.Trim());
            toolbar.Controls.Add(txtSearch);

            var btnRefresh = new Button
            {
                Text = "🔄 Refresh",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                BackColor = Color.FromArgb(241, 245, 249),
                ForeColor = Color.FromArgb(51, 65, 85),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(95, 32),
                Location = new Point(318, 8),
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnRefresh.Click += (s, e) => { txtSearch.Text = ""; LoadUserData(); };
            toolbar.Controls.Add(btnRefresh);

            // Edit User Details Button
            btnEditUser = new Button
            {
                Text = "✏️ Edit User Details",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                BackColor = Color.FromArgb(37, 99, 235), // Royal Blue #2563EB
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(165, 32),
                Location = new Point(422, 8),
                Cursor = Cursors.Hand
            };
            btnEditUser.FlatAppearance.BorderSize = 0;
            btnEditUser.MouseEnter += (s, e) => btnEditUser.BackColor = Color.FromArgb(29, 78, 216);
            btnEditUser.MouseLeave += (s, e) => btnEditUser.BackColor = Color.FromArgb(37, 99, 235);
            btnEditUser.Click += (s, e) => OpenEditUserModal();
            toolbar.Controls.Add(btnEditUser);

            // Enable / Disable Toggle Button
            btnToggle = new Button
            {
                Text = "⚡ Toggle User Status",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                BackColor = Color.FromArgb(245, 158, 11),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(230, 32),
                Location = new Point(595, 8),
                Cursor = Cursors.Hand
            };
            btnToggle.FlatAppearance.BorderSize = 0;
            btnToggle.Click += BtnToggleStatus_Click;
            toolbar.Controls.Add(btnToggle);

            // ── 3. DataGridView ───────────────────────────────────────────────
            dgvUsers = new DataGridView
            {
                Location = new Point(30, 180),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                EnableHeadersVisualStyles = false,
                GridColor = Color.FromArgb(226, 232, 240),
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
            };
            // Enable native double-buffering on the grid to kill row-selection flicker
            typeof(DataGridView).GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(dgvUsers, true, null);

            dgvUsers.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvUsers.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 61, 119);
            dgvUsers.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvUsers.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvUsers.ColumnHeadersHeight = 46;
            dgvUsers.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;

            dgvUsers.DefaultCellStyle.Font = new Font("Segoe UI", 9.5f);
            dgvUsers.DefaultCellStyle.ForeColor = Color.FromArgb(15, 23, 42);
            dgvUsers.DefaultCellStyle.BackColor = Color.White;
            dgvUsers.DefaultCellStyle.SelectionBackColor = Color.FromArgb(37, 99, 235);
            dgvUsers.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvUsers.DefaultCellStyle.Padding = new Padding(6, 0, 6, 0);

            dgvUsers.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(241, 245, 249);
            dgvUsers.AlternatingRowsDefaultCellStyle.ForeColor = Color.FromArgb(15, 23, 42);
            dgvUsers.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(37, 99, 235);
            dgvUsers.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.White;
            dgvUsers.AlternatingRowsDefaultCellStyle.Padding = new Padding(6, 0, 6, 0);

            dgvUsers.RowTemplate.Height = 36;
            dgvUsers.SelectionChanged += DgvUsers_SelectionChanged;
            dgvUsers.CellDoubleClick += (s, e) => { if (e.RowIndex >= 0) OpenEditUserModal(); };
            dgvUsers.CellFormatting += DgvUsers_CellFormatting;
            pnlManageUsers.Controls.Add(dgvUsers);

            lblSelectedUser = new Label
            {
                Location = new Point(30, 640),
                Size = new Size(880, 25),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 116, 139),
                Text = "Select a user row to edit details or toggle account access (Double-click to edit)."
            };
            pnlManageUsers.Controls.Add(lblSelectedUser);

            Action layoutManage = () =>
            {
                int w = Math.Max(780, pnlManageUsers.ClientSize.Width - 60);
                int h = Math.Max(300, pnlManageUsers.ClientSize.Height - 225);
                tabFilterBar.Width = w;
                toolbar.Width = w;
                dgvUsers.Size = new Size(w, h);
                lblSelectedUser.Location = new Point(30, pnlManageUsers.ClientSize.Height - 32);
                lblSelectedUser.Width = w;
            };
            pnlManageUsers.Resize += (s, e) => layoutManage();
            this.Shown += (s, e) => layoutManage();
        }

        private Button CreateFilterTabBtn(string text, int left, int width, string filterMode, Color activeColor)
        {
            var btn = new Button
            {
                Text = text,
                Tag = filterMode,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Location = new Point(left, 2),
                Size = new Size(width, 36),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += (s, e) => SetStatusFilter(filterMode);
            return btn;
        }

        private void SetStatusFilter(string filter)
        {
            currentStatusFilter = filter;

            // Update Tab active styling
            UpdateFilterTabStyles();
            LoadUserData(txtSearch?.Text.Trim() ?? "");
        }

        private void UpdateFilterTabStyles()
        {
            if (btnFilterAll == null || btnFilterActive == null || btnFilterInactive == null) return;

            bool isAll = string.Equals(currentStatusFilter, "All", StringComparison.OrdinalIgnoreCase);
            bool isActive = string.Equals(currentStatusFilter, "Active", StringComparison.OrdinalIgnoreCase);
            bool isInactive = string.Equals(currentStatusFilter, "Inactive", StringComparison.OrdinalIgnoreCase);

            btnFilterAll.BackColor = isAll ? Color.FromArgb(33, 61, 119) : Color.FromArgb(226, 232, 240);
            btnFilterAll.ForeColor = isAll ? Color.White : Color.FromArgb(51, 65, 85);

            btnFilterActive.BackColor = isActive ? Color.FromArgb(22, 163, 74) : Color.FromArgb(226, 232, 240);
            btnFilterActive.ForeColor = isActive ? Color.White : Color.FromArgb(51, 65, 85);

            btnFilterInactive.BackColor = isInactive ? Color.FromArgb(220, 38, 38) : Color.FromArgb(226, 232, 240);
            btnFilterInactive.ForeColor = isInactive ? Color.White : Color.FromArgb(51, 65, 85);
        }

        private void DgvUsers_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvUsers.Columns[e.ColumnIndex].Name == "Status" && e.Value != null)
            {
                string status = e.Value.ToString();
                if (string.Equals(status, "Active", StringComparison.OrdinalIgnoreCase))
                {
                    e.CellStyle.ForeColor = Color.FromArgb(22, 101, 52); // Dark Green
                    e.CellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
                }
                else
                {
                    e.CellStyle.ForeColor = Color.FromArgb(185, 28, 28); // Crimson Red
                    e.CellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
                }
            }
        }

        private void DgvUsers_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvUsers.SelectedRows.Count > 0)
            {
                var row = dgvUsers.SelectedRows[0];
                selectedUserId = Convert.ToInt32(row.Cells["UserId"].Value);
                selectedUserStatus = row.Cells["Status"].Value.ToString();
                lblSelectedUser.Text =
                    $"Selected User: {row.Cells["FullName"].Value} (@{row.Cells["Username"].Value})  |  Department: {row.Cells["Department"].Value}  |  Status: {selectedUserStatus}";

                if (btnToggle != null)
                {
                    if (string.Equals(selectedUserStatus, "Active", StringComparison.OrdinalIgnoreCase))
                    {
                        btnToggle.Text = "🚫 Disable / Deactivate User";
                        btnToggle.BackColor = Color.FromArgb(220, 38, 38); // Red
                    }
                    else
                    {
                        btnToggle.Text = "✅ Enable / Activate User";
                        btnToggle.BackColor = Color.FromArgb(22, 163, 74); // Green
                    }
                }
            }
            else
            {
                selectedUserId = 0;
                lblSelectedUser.Text = "Select a user row to edit details or toggle account access (Double-click to edit).";
                if (btnToggle != null)
                {
                    btnToggle.Text = "⚡ Toggle User Status";
                    btnToggle.BackColor = Color.FromArgb(245, 158, 11);
                }
            }
        }

        private void OpenEditUserModal()
        {
            if (selectedUserId == 0)
            {
                MessageBox.Show("Please select a user from the list to edit.", "No User Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (var editForm = new EditUserModalForm(selectedUserId))
            {
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    LoadUserData(txtSearch?.Text.Trim() ?? "");
                }
            }
        }

        private void BtnToggleStatus_Click(object sender, EventArgs e)
        {
            if (selectedUserId == 0)
            {
                MessageBox.Show("Please select a user from the list first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool isCurrentlyActive = string.Equals(selectedUserStatus, "Active", StringComparison.OrdinalIgnoreCase);
            string newStatus = isCurrentlyActive ? "Inactive" : "Active";
            string actionWord = isCurrentlyActive ? "DISABLE (Deactivate)" : "ENABLE (Activate)";

            if (MessageBox.Show(
                $"Are you sure you want to {actionWord} user #{selectedUserId} ({lblSelectedUser.Text})?\n\n" +
                (isCurrentlyActive ? "The user will be immediately blocked from signing into the system." : "The user will be granted active portal access."),
                "Confirm Account Status Change",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (authService.SetUserStatus(selectedUserId, newStatus, out string err))
                {
                    MessageBox.Show($"User account status changed to '{newStatus}' successfully.", "Status Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadUserData(txtSearch?.Text.Trim() ?? "");
                }
                else
                {
                    MessageBox.Show("Error updating status: " + err, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void LoadUserData(string search = "")
        {
            try
            {
                DataTable dt = authService.GetAllUsers(search, currentStatusFilter);
                dgvUsers.DataSource = dt;

                // Configure friendly column headers + explicit widths so text is never clipped
                void SetCol(string name, string header, int width) {
                    if (dgvUsers.Columns[name] != null) {
                        dgvUsers.Columns[name].HeaderText = header;
                        dgvUsers.Columns[name].Width = width;
                        dgvUsers.Columns[name].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                        dgvUsers.Columns[name].MinimumWidth = width;
                        dgvUsers.Columns[name].DefaultCellStyle.WrapMode = DataGridViewTriState.False;
                        dgvUsers.Columns[name].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                    }
                }
                SetCol("UserId",       "User ID",        65);
                SetCol("FullName",     "Full Name",      140);
                SetCol("DateOfBirth",  "Date of Birth",  110);
                SetCol("Gender",       "Gender",          75);
                SetCol("Email",        "Email",          165);
                SetCol("Phone",        "Phone",          115);
                SetCol("Address",      "Station Address",155);
                SetCol("Department",   "Department",     145);
                SetCol("Course",       "Designation",    130);
                SetCol("YearOfJoining","Joined",          70);
                SetCol("CollegeOrOrg", "Division / Org", 145);
                SetCol("Username",     "Username",       110);
                SetCol("Status",       "Status",          80);
                SetCol("CreatedAt",    "Registered On",  130);

                // Calculate global counts
                DataTable allDt = authService.GetAllUsers();
                int total = allDt.Rows.Count, active = 0, inactive = 0;
                foreach (DataRow r in allDt.Rows)
                {
                    if (string.Equals(r["Status"].ToString(), "Active", StringComparison.OrdinalIgnoreCase))
                        active++;
                    else
                        inactive++;
                }

                if (lblTotalUsers != null) lblTotalUsers.Text = total.ToString();
                if (lblActiveUsers != null) lblActiveUsers.Text = active.ToString();
                if (lblInactiveUsers != null) lblInactiveUsers.Text = inactive.ToString();
                if (lblResetLogsCount != null) lblResetLogsCount.Text = authService.GetPasswordResetLogs().Rows.Count.ToString();

                // Update filter tab button labels with live counts
                if (btnFilterAll != null) btnFilterAll.Text = $"📋 All Accounts ({total})";
                if (btnFilterActive != null) btnFilterActive.Text = $"🟢 Active Accounts ({active})";
                if (btnFilterInactive != null) btnFilterInactive.Text = $"🔴 Inactive / Disabled ({inactive})";

                UpdateFilterTabStyles();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LoadUserData: " + ex.Message);
            }
        }

        #endregion

        #region Password Reset Audit Logs Panel

        private void BuildResetLogsPanel()
        {
            pnlResetLogs = new Panel { Dock = DockStyle.Fill, AutoScroll = true, BackColor = Color.FromArgb(241, 245, 249) };
            contentPanel.Controls.Add(pnlResetLogs);

            pnlResetLogs.Controls.Add(MakeLabel("User Password Reset Audit Logs", new Font("Segoe UI", 18, FontStyle.Bold),
                Color.FromArgb(15, 23, 42), new Point(30, 20), new Size(600, 35)));

            pnlResetLogs.Controls.Add(MakeLabel("Real-time audit tracking of user self-service password resets verified by Mobile & DOB.",
                new Font("Segoe UI", 9.5f), Color.FromArgb(100, 116, 139), new Point(30, 56), new Size(650, 22)));

            // Top Search & Refresh Bar
            Panel bar = new Panel { Location = new Point(30, 85), Size = new Size(880, 48), BackColor = Color.White };
            pnlResetLogs.Controls.Add(bar);

            bar.Controls.Add(MakeLabel("🔍 Search Logs:", new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Color.FromArgb(51, 65, 85), new Point(14, 14), new Size(110, 22)));

            txtSearchResetLogs = new TextBox
            {
                Font = new Font("Segoe UI", 10.5f),
                Location = new Point(130, 10),
                Size = new Size(320, 28),
                BorderStyle = BorderStyle.FixedSingle
            };
            txtSearchResetLogs.TextChanged += (s, e) => LoadResetLogs(txtSearchResetLogs.Text.Trim());
            bar.Controls.Add(txtSearchResetLogs);

            Button btnRefreshLogs = new Button
            {
                Text = "🔄 Refresh Logs",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                BackColor = Color.FromArgb(59, 130, 246),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(130, 32),
                Location = new Point(470, 8),
                Cursor = Cursors.Hand
            };
            btnRefreshLogs.FlatAppearance.BorderSize = 0;
            btnRefreshLogs.Click += (s, e) => { txtSearchResetLogs.Clear(); LoadResetLogs(); };
            bar.Controls.Add(btnRefreshLogs);

            // DataGridView for Reset Logs
            dgvResetLogs = new DataGridView
            {
                Location = new Point(30, 145),
                Size = new Size(880, 480),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false,
                Font = new Font("Segoe UI", 9.5f)
            };
            dgvResetLogs.EnableHeadersVisualStyles = false;
            dgvResetLogs.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(33, 61, 119);
            dgvResetLogs.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvResetLogs.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            dgvResetLogs.ColumnHeadersHeight = 36;
            dgvResetLogs.RowTemplate.Height = 32;
            dgvResetLogs.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            pnlResetLogs.Controls.Add(dgvResetLogs);

            pnlResetLogs.Resize += (s, e) =>
            {
                bar.Width = Math.Max(800, pnlResetLogs.ClientSize.Width - 60);
                dgvResetLogs.Width = Math.Max(800, pnlResetLogs.ClientSize.Width - 60);
                dgvResetLogs.Height = Math.Max(300, pnlResetLogs.ClientSize.Height - 170);
            };
        }

        private void LoadResetLogs(string search = "")
        {
            try
            {
                DataTable dt = authService.GetPasswordResetLogs(search);
                if (dgvResetLogs != null)
                {
                    dgvResetLogs.DataSource = dt;
                    if (dgvResetLogs.Columns["LogId"] != null) dgvResetLogs.Columns["LogId"].HeaderText = "Log ID";
                    if (dgvResetLogs.Columns["UserId"] != null) dgvResetLogs.Columns["UserId"].HeaderText = "User ID";
                    if (dgvResetLogs.Columns["Username"] != null) dgvResetLogs.Columns["Username"].HeaderText = "Username";
                    if (dgvResetLogs.Columns["FullName"] != null) dgvResetLogs.Columns["FullName"].HeaderText = "Employee Full Name";
                    if (dgvResetLogs.Columns["Phone"] != null) dgvResetLogs.Columns["Phone"].HeaderText = "Registered Mobile";
                    if (dgvResetLogs.Columns["ResetTime"] != null) dgvResetLogs.Columns["ResetTime"].HeaderText = "Reset Timestamp";
                    if (dgvResetLogs.Columns["Status"] != null) dgvResetLogs.Columns["Status"].HeaderText = "Action Status";
                    if (dgvResetLogs.Columns["IPAddressOrHost"] != null) dgvResetLogs.Columns["IPAddressOrHost"].HeaderText = "Host Terminal";
                    if (dgvResetLogs.Columns["AdminNotified"] != null) dgvResetLogs.Columns["AdminNotified"].Visible = false;
                }
                if (lblResetLogsCount != null) lblResetLogsCount.Text = dt.Rows.Count.ToString();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("LoadResetLogs: " + ex.Message);
            }
        }

        #endregion

        private void NavigateToMain()
        {
            var main = Application.OpenForms.Cast<Form>().OfType<MainClassesForm>().FirstOrDefault();
            if (main != null && !main.IsDisposed)
            {
                main.SuspendLayout();
                main.Show();
                main.BringToFront();
                main.ResumeLayout(true);
                main.Update();
            }
            else
            {
                new MainClassesForm().Show();
            }
            this.BeginInvoke(new Action(() => this.Close()));
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Log out from Admin portal?", "Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                SessionManager.Logout();
                new LoginForm().Show();
                this.Close();
            }
        }
    }
}
