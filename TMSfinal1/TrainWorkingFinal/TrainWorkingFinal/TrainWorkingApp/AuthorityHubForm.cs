using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TrainWorkingApp
{
    /// <summary>
    /// Grand Operational Authorities Portal Hub.
    /// Features clean executive category modules, real-time authority counts,
    /// top navigation with active operator session badge, My Profile navigation, and Logout.
    /// </summary>
    public class AuthorityHubForm : Form
    {
        public AuthorityHubForm()
        {
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Font = new Font("Segoe UI", 9.75f, FontStyle.Regular);
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Indian Railways – Train Working Operational Authorities Hub";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MinimumSize = new Size(1150, 750);
            this.BackColor = Color.FromArgb(241, 245, 249); // Clean slate background

            // ── 1. Top Header Bar ─────────────────────────────────────────────
            Panel topBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 72,
                BackColor = Color.FromArgb(20, 48, 105) // Deep Navy
            };
            this.Controls.Add(topBar);

            Label lblLogo = new Label
            {
                Text = "🚆",
                Font = new Font("Segoe UI Emoji", 20),
                ForeColor = Color.White,
                Location = new Point(18, 14),
                Size = new Size(42, 42),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            topBar.Controls.Add(lblLogo);

            Label lblTitle = new Label
            {
                Text = "TRAIN WORKING OPERATIONAL AUTHORITIES HUB",
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(66, 12),
                AutoSize = true,
                UseMnemonic = false,
                BackColor = Color.Transparent
            };
            topBar.Controls.Add(lblTitle);

            Label lblSub = new Label
            {
                Text = "Central Register Hub Covering All 22 Operating Forms, Digital Authorities & Dynamic Audit Reports",
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(190, 215, 250),
                Location = new Point(68, 38),
                AutoSize = true,
                UseMnemonic = false,
                BackColor = Color.Transparent
            };
            topBar.Controls.Add(lblSub);

            // Active Operator Chip on Header
            string deptName = string.IsNullOrWhiteSpace(SessionManager.CurrentDepartment) ? "Operating" : SessionManager.CurrentDepartment;
            string userName = string.IsNullOrWhiteSpace(SessionManager.CurrentFullName) ? "Station Operator" : SessionManager.CurrentFullName;

            Panel userChip = new Panel
            {
                BackColor = Color.FromArgb(15, 35, 80),
                Height = 40,
                Cursor = Cursors.Default
            };

            Label lblAvatar = new Label
            {
                Text = GetInitials(userName),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 48, 105),
                BackColor = Color.FromArgb(254, 240, 138),
                Size = new Size(28, 28),
                Location = new Point(6, 6),
                TextAlign = ContentAlignment.MiddleCenter
            };
            userChip.Controls.Add(lblAvatar);

            Label lblUserName = new Label
            {
                Text = $"👤  {userName} ({deptName})",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(254, 240, 138), // Gold
                Location = new Point(38, 10),
                AutoSize = true,
                UseMnemonic = false,
                BackColor = Color.Transparent
            };
            userChip.Controls.Add(lblUserName);
            topBar.Controls.Add(userChip);

            // My Profile / Admin Panel Navigation Button
            Button btnProfile = new Button
            {
                Text = SessionManager.CurrentRole == UserRole.Admin ? "🛡️ Admin Panel" : "🏠 My Profile",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(251, 121, 43), // #FB792B IRCTC Orange
                FlatStyle = FlatStyle.Flat,
                Size = new Size(140, 36),
                Cursor = Cursors.Hand
            };
            btnProfile.FlatAppearance.BorderSize = 0;
            btnProfile.Click += (s, e) =>
            {
                if (SessionManager.CurrentRole == UserRole.Admin)
                {
                    new AdminDashboardForm().Show();
                }
                else
                {
                    new UserDashboardForm().Show();
                }
                this.Close();
            };
            topBar.Controls.Add(btnProfile);

            // Logout Button
            Button btnLogout = new Button
            {
                Text = "🚪 Logout",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(220, 38, 38), // Crimson Red
                FlatStyle = FlatStyle.Flat,
                Size = new Size(105, 36),
                Cursor = Cursors.Hand
            };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.Click += (s, e) =>
            {
                if (MessageBox.Show("Are you sure you want to end your active duty session and logout?",
                    "Confirm Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    SessionManager.Logout();
                    new LoginForm().Show();
                    this.Close();
                }
            };
            topBar.Controls.Add(btnLogout);

            Action layoutHeader = () =>
            {
                int r = topBar.ClientSize.Width - 18;
                btnLogout.Location = new Point(r - 105, 16);
                r -= 120;

                btnProfile.Location = new Point(r - 140, 16);
                r -= 155;

                int chipWidth = TextRenderer.MeasureText(lblUserName.Text, lblUserName.Font).Width + 55;
                userChip.Size = new Size(chipWidth, 40);
                userChip.Location = new Point(r - chipWidth, 14);
            };
            topBar.Resize += (s, e) => layoutHeader();
            this.Shown += (s, e) => layoutHeader();

            // ── 2. Content Body Panel ─────────────────────────────────────────
            Panel content = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(241, 245, 249),
                Padding = new Padding(30, 25, 30, 25)
            };
            this.Controls.Add(content);
            content.BringToFront();

            // Banner Notice
            Panel banner = new Panel
            {
                Location = new Point(30, 20),
                Size = new Size(1080, 60),
                BackColor = Color.White
            };
            banner.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(226, 232, 240), 1))
                    e.Graphics.DrawRectangle(pen, 0, 0, banner.Width - 1, banner.Height - 1);
                using (var brush = new SolidBrush(Color.FromArgb(33, 61, 119)))
                    e.Graphics.FillRectangle(brush, 0, 0, 5, banner.Height);
            };
            content.Controls.Add(banner);

            Label lblBannerTitle = new Label
            {
                Text = "⚡ OPERATIONAL AUTHORITIES CLASSIFICATION & AUDIT CONSOLE",
                Font = new Font("Segoe UI", 11.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(20, 10),
                AutoSize = true,
                UseMnemonic = false
            };
            banner.Controls.Add(lblBannerTitle);

            Label lblBannerSub = new Label
            {
                Text = "Select an operational category below to issue authorities, record track permits, and inspect official logs.",
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(22, 32),
                AutoSize = true,
                UseMnemonic = false
            };
            banner.Controls.Add(lblBannerSub);

            // 4 Clean Grand Category Cards (Paragraph descriptions removed)
            int y = 95;
            CreateGrandCategoryCard(content, "1. Signal & Defective Authorities", 
                "6 Operational Authorities", 
                "🚦", Color.FromArgb(30, 58, 138), Color.FromArgb(239, 246, 255), 30, y, 1);

            CreateGrandCategoryCard(content, "2. Line Clear & Block Working", 
                "8 Operating Registers", 
                "🎫", Color.FromArgb(22, 163, 74), Color.FromArgb(240, 253, 244), 580, y, 2);

            y += 180;

            CreateGrandCategoryCard(content, "3. Maintenance, S&T & Trolley", 
                "4 Notice Modules", 
                "🔧", Color.FromArgb(234, 88, 12), Color.FromArgb(255, 247, 237), 30, y, 3);

            CreateGrandCategoryCard(content, "4. Emergency, Relief & Movements", 
                "4 Emergency Logs", 
                "🚨", Color.FromArgb(220, 38, 38), Color.FromArgb(254, 242, 242), 580, y, 4);

            y += 180;

            // 5th Grand Card: 📊 Dynamic Operational Reports & Audit Console
            CreateReportsCard(content, 30, y);

            content.Resize += (s, e) =>
            {
                int w = Math.Max(1050, content.ClientSize.Width - 60);
                banner.Width = w;
            };
        }

        private void CreateGrandCategoryCard(Panel parent, string title, string badgeCount, 
            string icon, Color primaryColor, Color tintBg, int x, int y, int catId)
        {
            Panel card = new Panel
            {
                Size = new Size(520, 160),
                Location = new Point(x, y),
                BackColor = Color.White,
                Cursor = Cursors.Hand
            };
            card.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(226, 232, 240), 1.5f))
                    e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
            };

            // Top Color Accent Bar
            Panel topAccent = new Panel
            {
                Dock = DockStyle.Top,
                Height = 6,
                BackColor = primaryColor
            };
            card.Controls.Add(topAccent);

            // Icon circle badge
            Panel pnlIcon = new Panel
            {
                Location = new Point(20, 22),
                Size = new Size(52, 52),
                BackColor = tintBg
            };
            pnlIcon.Paint += (s, e) =>
            {
                using (var pen = new Pen(primaryColor, 1))
                    e.Graphics.DrawRectangle(pen, 0, 0, pnlIcon.Width - 1, pnlIcon.Height - 1);
            };
            Label lblIco = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI Emoji", 22),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent,
                UseMnemonic = false
            };
            pnlIcon.Controls.Add(lblIco);
            card.Controls.Add(pnlIcon);

            // Title
            Label lblT = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(84, 24),
                AutoSize = true,
                UseMnemonic = false
            };
            card.Controls.Add(lblT);

            // Count badge
            Label lblCount = new Label
            {
                Text = badgeCount,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = primaryColor,
                BackColor = tintBg,
                Padding = new Padding(8, 3, 8, 3),
                Location = new Point(86, 52),
                AutoSize = true,
                UseMnemonic = false
            };
            card.Controls.Add(lblCount);

            // Open Action Button
            Button btnOpen = new Button
            {
                Text = "OPEN CATEGORY ➔",
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = primaryColor,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(480, 42),
                Location = new Point(20, 98),
                Cursor = Cursors.Hand
            };
            btnOpen.FlatAppearance.BorderSize = 0;
            btnOpen.Click += (s, e) => OpenCategory(catId);
            card.Controls.Add(btnOpen);

            card.Click += (s, e) => OpenCategory(catId);
            lblT.Click += (s, e) => OpenCategory(catId);
            pnlIcon.Click += (s, e) => OpenCategory(catId);
            lblIco.Click += (s, e) => OpenCategory(catId);

            parent.Controls.Add(card);
        }

        private void CreateReportsCard(Panel parent, int x, int y)
        {
            Panel card = new Panel
            {
                Size = new Size(1070, 95),
                Location = new Point(x, y),
                BackColor = Color.White,
                Cursor = Cursors.Hand
            };
            card.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(226, 232, 240), 1.5f))
                    e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
            };

            // Left Emerald Stripe
            Panel leftAccent = new Panel
            {
                Dock = DockStyle.Left,
                Width = 6,
                BackColor = Color.FromArgb(22, 163, 74)
            };
            card.Controls.Add(leftAccent);

            // Icon
            Panel pnlIcon = new Panel
            {
                Location = new Point(22, 20),
                Size = new Size(52, 52),
                BackColor = Color.FromArgb(240, 253, 244)
            };
            pnlIcon.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(22, 163, 74), 1))
                    e.Graphics.DrawRectangle(pen, 0, 0, pnlIcon.Width - 1, pnlIcon.Height - 1);
            };
            Label lblIco = new Label
            {
                Text = "📊",
                Font = new Font("Segoe UI Emoji", 22),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent,
                UseMnemonic = false
            };
            pnlIcon.Controls.Add(lblIco);
            card.Controls.Add(pnlIcon);

            // Title
            Label lblT = new Label
            {
                Text = "5. Dynamic Operational Reports & Railway Audit Console",
                Font = new Font("Segoe UI", 13f, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(88, 24),
                AutoSize = true,
                UseMnemonic = false
            };
            card.Controls.Add(lblT);

            Label lblSub = new Label
            {
                Text = "Cross-register queries, date-range filtration, and export official records to Excel, CSV, or PDF.",
                Font = new Font("Segoe UI", 9.5f),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(90, 52),
                AutoSize = true,
                UseMnemonic = false
            };
            card.Controls.Add(lblSub);

            // Action Button
            Button btnLaunch = new Button
            {
                Text = "📊 LAUNCH REPORTS ➔",
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(22, 163, 74),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(240, 44),
                Location = new Point(805, 25),
                Cursor = Cursors.Hand
            };
            btnLaunch.FlatAppearance.BorderSize = 0;
            btnLaunch.Click += (s, e) => new DynamicReportsForm().ShowDialog(this);
            card.Controls.Add(btnLaunch);

            card.Click += (s, e) => new DynamicReportsForm().ShowDialog(this);
            lblT.Click += (s, e) => new DynamicReportsForm().ShowDialog(this);
            lblSub.Click += (s, e) => new DynamicReportsForm().ShowDialog(this);
            pnlIcon.Click += (s, e) => new DynamicReportsForm().ShowDialog(this);
            lblIco.Click += (s, e) => new DynamicReportsForm().ShowDialog(this);

            parent.Controls.Add(card);
        }

        private string GetInitials(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName)) return "U";
            var parts = fullName.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1) return parts[0].Substring(0, Math.Min(2, parts[0].Length)).ToUpper();
            return (parts[0][0].ToString() + parts[parts.Length - 1][0].ToString()).ToUpper();
        }

        private void OpenCategory(int catId)
        {
            AuthoritySelectionForm selectionForm = new AuthoritySelectionForm(catId);
            selectionForm.ShowDialog(this);
        }
    }
}
