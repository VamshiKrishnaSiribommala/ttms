using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;

namespace TMS
{
    /// <summary>
    /// Professional Indian Railways & IRCTC Inspired Enterprise User Dashboard.
    /// Features modern operational hero cards, live digital railway clock,
    /// responsive profile data tiles, quick register access, interactive hover effects, and anti-flicker rendering.
    /// </summary>
    public class UserDashboardForm : Form
    {
        private Timer liveClockTimer;
        private Label lblLiveClock;
        private Panel body;
        private Panel header;

        public UserDashboardForm()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.DoubleBuffered = true;
            InitializeComponent();
            StartLiveClock();
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
            this.Text = "TMS – Station Operations User Portal";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MinimumSize = new Size(1100, 750);
            this.BackColor = Color.FromArgb(241, 245, 249); // Modern slate page background

            // ── 1. Body Panel (Fill – added FIRST in WinForms docking order) ──────────
            body = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(241, 245, 249)
            };
            this.Controls.Add(body);

            // ── 2. Top Header (Added LAST so it docks on top) ─────────────────────────
            header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 74,
                BackColor = Color.FromArgb(33, 61, 119) // #213D77 Indian Railways Navy
            };
            this.Controls.Add(header);

            BuildHeader(header);
            BuildBody(body);
        }

        private void BuildHeader(Panel headerPanel)
        {
            // Left Logo & System Titles
            Label lblTrainIcon = new Label
            {
                Text = "🚆",
                Font = new Font("Segoe UI Emoji", 20),
                ForeColor = Color.White,
                Location = new Point(20, 15),
                Size = new Size(42, 42),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(lblTrainIcon);

            Label lblAppTitle = new Label
            {
                Text = "TRAIN MANAGEMENT SYSTEM",
                Font = new Font("Segoe UI", 13.5f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(68, 13),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(lblAppTitle);

            Label lblAppSub = new Label
            {
                Text = "Indian Railways  •  Station Operations & Digital Registers Portal",
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(205, 225, 255),
                Location = new Point(70, 40),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(lblAppSub);

            // Live Digital Railway Clock
            lblLiveClock = new Label
            {
                Text = $"📅 {DateTime.Now:ddd, dd-MMM-yyyy}   ⏰ {DateTime.Now:HH:mm:ss} IST",
                Font = new Font("Segoe UI Semibold", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(255, 235, 160), // Warm golden amber
                BackColor = Color.FromArgb(22, 45, 92),
                Padding = new Padding(12, 6, 12, 6),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter
            };
            headerPanel.Controls.Add(lblLiveClock);

            // Right Action Controls with Hover Effects
            Button btnHeaderRegisters = new Button
            {
                Text = "🚆 Open Registers",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                BackColor = Color.FromArgb(251, 121, 43), // #FB792B IRCTC Orange
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(180, 40),
                Cursor = Cursors.Hand
            };
            btnHeaderRegisters.FlatAppearance.BorderSize = 0;
            btnHeaderRegisters.MouseEnter += (s, e) => btnHeaderRegisters.BackColor = Color.FromArgb(234, 88, 12); // Deep orange on hover
            btnHeaderRegisters.MouseLeave += (s, e) => btnHeaderRegisters.BackColor = Color.FromArgb(251, 121, 43);
            btnHeaderRegisters.Click += (s, e) => NavigateToMain();
            headerPanel.Controls.Add(btnHeaderRegisters);

            Button btnHeaderLogout = new Button
            {
                Text = "🚪 Logout",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                BackColor = Color.FromArgb(220, 38, 38), // Crimson Red
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 40),
                Cursor = Cursors.Hand
            };
            btnHeaderLogout.FlatAppearance.BorderSize = 0;
            btnHeaderLogout.MouseEnter += (s, e) => btnHeaderLogout.BackColor = Color.FromArgb(185, 28, 28); // Darker red on hover
            btnHeaderLogout.MouseLeave += (s, e) => btnHeaderLogout.BackColor = Color.FromArgb(220, 38, 38);
            btnHeaderLogout.Click += (s, e) => DoLogout();
            headerPanel.Controls.Add(btnHeaderLogout);

            // User Identity Badge in Header (Auto-sized to prevent any text clipping)
            string deptName = string.IsNullOrWhiteSpace(SessionManager.CurrentDepartment) ? "Operating" : SessionManager.CurrentDepartment;
            string userName = string.IsNullOrWhiteSpace(SessionManager.CurrentFullName) ? "Station Operator" : SessionManager.CurrentFullName;

            Panel userChip = new Panel
            {
                BackColor = Color.FromArgb(24, 48, 98),
                Height = 44,
                Cursor = Cursors.Default
            };
            userChip.MouseEnter += (s, e) => userChip.BackColor = Color.FromArgb(32, 62, 124);
            userChip.MouseLeave += (s, e) => userChip.BackColor = Color.FromArgb(24, 48, 98);

            Label lblAvatar = new Label
            {
                Text = GetInitials(userName),
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 61, 119),
                BackColor = Color.FromArgb(254, 240, 138), // Soft yellow pill
                Size = new Size(32, 32),
                Location = new Point(7, 6),
                TextAlign = ContentAlignment.MiddleCenter
            };
            userChip.Controls.Add(lblAvatar);

            Label lblUserName = new Label
            {
                Text = userName,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(46, 4),
                AutoSize = true
            };
            userChip.Controls.Add(lblUserName);

            Label lblUserRole = new Label
            {
                Text = deptName,
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(190, 215, 250),
                Location = new Point(46, 23),
                AutoSize = true
            };
            userChip.Controls.Add(lblUserRole);

            headerPanel.Controls.Add(userChip);

            // Dynamic header layout adjustment with full auto-width measurement
            Action layoutHeader = () =>
            {
                int r = headerPanel.ClientSize.Width - 20;
                btnHeaderLogout.Location = new Point(r - btnHeaderLogout.Width, 17);
                r -= (btnHeaderLogout.Width + 12);

                btnHeaderRegisters.Location = new Point(r - btnHeaderRegisters.Width, 17);
                r -= (btnHeaderRegisters.Width + 16);

                int textW = Math.Max(TextRenderer.MeasureText(userName, lblUserName.Font).Width,
                                     TextRenderer.MeasureText(deptName, lblUserRole.Font).Width);
                userChip.Width = Math.Max(240, textW + 65);
                userChip.Location = new Point(r - userChip.Width, 15);
                r -= (userChip.Width + 20);

                lblLiveClock.Location = new Point(Math.Max(380, r - lblLiveClock.Width), 21);
            };

            headerPanel.Resize += (s, e) => layoutHeader();
            this.Shown += (s, e) => layoutHeader();
        }

        private void StartLiveClock()
        {
            liveClockTimer = new Timer { Interval = 1000 };
            liveClockTimer.Tick += (s, e) =>
            {
                if (lblLiveClock != null && !lblLiveClock.IsDisposed)
                {
                    lblLiveClock.Text = $"📅 {DateTime.Now:ddd, dd-MMM-yyyy}   ⏰ {DateTime.Now:HH:mm:ss} IST";
                }
            };
            liveClockTimer.Start();
        }

        private void BuildBody(Panel content)
        {
            content.SuspendLayout();
            content.Controls.Clear();

            // Main container that adapts to screen width with nice breathing room
            Panel container = new Panel
            {
                Location = new Point(25, 20),
                BackColor = Color.Transparent,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };
            content.Controls.Add(container);

            content.Resize += (s, e) =>
            {
                int targetWidth = Math.Max(1000, content.ClientSize.Width - 50);
                container.Width = targetWidth;
            };
            container.Width = Math.Max(1000, content.ClientSize.Width - 50);

            int currentY = 0;

            // ═════════════════════════════════════════════════════════════════════
            // 1. WELCOME & ACTIVE SHIFT BANNER (Clean & Uncluttered)
            // ═════════════════════════════════════════════════════════════════════
            Panel welcomeBanner = CreateCardPanel(0, currentY, container.Width, 75);
            container.Controls.Add(welcomeBanner);

            // Left Emerald Accent Stripe
            Panel accentBar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 6,
                BackColor = Color.FromArgb(22, 163, 74) // Emerald Green
            };
            welcomeBanner.Controls.Add(accentBar);

            // Greeting (Clean bold title only)
            Label lblWelcomeTitle = new Label
            {
                Text = $"Welcome, {SessionManager.CurrentFullName}",
                Font = new Font("Segoe UI", 16.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42), // Slate 900
                Location = new Point(24, 22),
                AutoSize = true
            };
            welcomeBanner.Controls.Add(lblWelcomeTitle);

            // Right Status Badges
            Panel pnlStatusRight = new Panel
            {
                Size = new Size(320, 50),
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            welcomeBanner.Controls.Add(pnlStatusRight);

            Label badgeStatus = new Label
            {
                Text = "● ACTIVE DUTY SESSION",
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(22, 101, 52), // Dark Green
                BackColor = Color.FromArgb(220, 252, 231), // Mint green
                Padding = new Padding(10, 4, 10, 4),
                Size = new Size(170, 26),
                Location = new Point(140, 12),
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlStatusRight.Controls.Add(badgeStatus);

            welcomeBanner.Resize += (s, e) =>
            {
                pnlStatusRight.Location = new Point(welcomeBanner.ClientSize.Width - 340, 12);
            };
            pnlStatusRight.Location = new Point(welcomeBanner.ClientSize.Width - 340, 12);

            currentY += 90;

            // ═════════════════════════════════════════════════════════════════════
            // 2. STATION REGISTERS MANAGEMENT (Light Pink & Light Violet Gradient Banner)
            // ═════════════════════════════════════════════════════════════════════
            Panel heroRegisters = new Panel
            {
                Location = new Point(0, currentY),
                Size = new Size(container.Width, 80),
                BackColor = Color.FromArgb(253, 232, 244)
            };
            heroRegisters.Paint += (s, e) =>
            {
                using (var brush = new LinearGradientBrush(heroRegisters.ClientRectangle,
                    Color.FromArgb(253, 232, 244), // Soft Pink
                    Color.FromArgb(237, 233, 254), // Soft Violet / Lavender
                    LinearGradientMode.Horizontal))
                {
                    e.Graphics.FillRectangle(brush, heroRegisters.ClientRectangle);
                }
                DrawCardBorder(e.Graphics, heroRegisters.ClientRectangle, Color.FromArgb(233, 213, 255), 1);
            };
            container.Controls.Add(heroRegisters);

            Panel pinkVioletAccent = new Panel
            {
                Dock = DockStyle.Left,
                Width = 5,
                BackColor = Color.FromArgb(147, 51, 234)
            };
            heroRegisters.Controls.Add(pinkVioletAccent);

            Label lblHeroIcon = new Label
            {
                Text = "🚆",
                Font = new Font("Segoe UI Emoji", 20),
                Location = new Point(20, 18),
                Size = new Size(42, 42),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            heroRegisters.Controls.Add(lblHeroIcon);

            Label lblHeroTitle = new Label
            {
                Text = "Station Registers Management – All 41 Modules",
                Font = new Font("Segoe UI", 13.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(88, 28, 135), // Deep Rich Purple (#581C87)
                Location = new Point(68, 26),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            heroRegisters.Controls.Add(lblHeroTitle);

            Button btnOpenRegisters = new Button
            {
                Text = "🚆  Open Station Registers ➔",
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                BackColor = Color.FromArgb(124, 58, 237), // Royal Violet (#7C3AED)
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(260, 44),
                Cursor = Cursors.Hand
            };
            btnOpenRegisters.FlatAppearance.BorderSize = 0;
            btnOpenRegisters.MouseEnter += (s, e) => btnOpenRegisters.BackColor = Color.FromArgb(109, 40, 217);
            btnOpenRegisters.MouseLeave += (s, e) => btnOpenRegisters.BackColor = Color.FromArgb(124, 58, 237);
            btnOpenRegisters.Click += (s, e) => NavigateToMain();
            heroRegisters.Controls.Add(btnOpenRegisters);

            Action layoutHero = () =>
            {
                btnOpenRegisters.Location = new Point(heroRegisters.ClientSize.Width - 280, 18);
            };
            heroRegisters.Resize += (s, e) => layoutHero();
            layoutHero();

            currentY += 95;

            // ═════════════════════════════════════════════════════════════════════
            // 3. REGISTERED EMPLOYEE PROFILE & AUTHORIZATION (Executive Grade)
            // ═════════════════════════════════════════════════════════════════════
            Panel profileCard = CreateCardPanel(0, currentY, container.Width, 475);
            container.Controls.Add(profileCard);

            // Left Navy Accent Stripe
            Panel navyAccent = new Panel
            {
                Dock = DockStyle.Left,
                Width = 6,
                BackColor = Color.FromArgb(33, 61, 119) // Indian Railways Navy
            };
            profileCard.Controls.Add(navyAccent);

            // Top Profile Header with Large Avatar & Badges
            Panel profHeaderBar = new Panel
            {
                Location = new Point(20, 16),
                Size = new Size(profileCard.Width - 40, 122),
                BackColor = Color.FromArgb(248, 250, 252)
            };
            profHeaderBar.Paint += (s, e) => {
                using (Pen p = new Pen(Color.FromArgb(226, 232, 240), 1))
                    e.Graphics.DrawRectangle(p, 0, 0, profHeaderBar.Width - 1, profHeaderBar.Height - 1);
            };
            profileCard.Controls.Add(profHeaderBar);

            // Avatar Box
            Label lblBigAvatar = new Label
            {
                Text = GetInitials(SessionManager.CurrentFullName),
                Font = new Font("Segoe UI", 18f, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 61, 119),
                BackColor = Color.FromArgb(254, 240, 138), // Gold
                Size = new Size(68, 68),
                Location = new Point(14, 27),
                TextAlign = ContentAlignment.MiddleCenter
            };
            lblBigAvatar.Paint += (s, e) => {
                using (Pen p = new Pen(Color.FromArgb(202, 138, 4), 1.5f))
                    e.Graphics.DrawRectangle(p, 0, 0, lblBigAvatar.Width - 1, lblBigAvatar.Height - 1);
            };
            profHeaderBar.Controls.Add(lblBigAvatar);

            Label lblProfName = new Label
            {
                Text = SessionManager.CurrentFullName,
                Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(94, 12),
                AutoSize = true
            };
            profHeaderBar.Controls.Add(lblProfName);

            string roleSubtitle = $"{SessionManager.CurrentDepartment}   •   {SessionManager.CurrentCourse}";
            Label lblProfRole = new Label
            {
                Text = roleSubtitle,
                Font = new Font("Segoe UI", 10f, FontStyle.Regular),
                ForeColor = Color.FromArgb(71, 85, 105),
                Location = new Point(96, 45),
                AutoSize = true
            };
            profHeaderBar.Controls.Add(lblProfRole);

            // Badges FlowLayoutPanel (Auto-arranges without overlapping)
            FlowLayoutPanel pnlBadges = new FlowLayoutPanel
            {
                Location = new Point(94, 76),
                Size = new Size(profHeaderBar.Width - 105, 36),
                BackColor = Color.Transparent,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = false
            };
            profHeaderBar.Controls.Add(pnlBadges);

            Label badgeAuth = CreatePillBadge("🟢 Active Authorized Duty", Color.FromArgb(220, 252, 231), Color.FromArgb(22, 101, 52));
            Label badgeEmpId = CreatePillBadge($"🏷️ Emp ID: {SessionManager.CurrentUsername}", Color.FromArgb(238, 242, 255), Color.FromArgb(30, 64, 175));
            Label badgeOrg = CreatePillBadge($"🏛️ {SessionManager.CurrentCollegeOrOrg}", Color.FromArgb(254, 243, 199), Color.FromArgb(146, 64, 14));
            pnlBadges.Controls.AddRange(new Control[] { badgeAuth, badgeEmpId, badgeOrg });

            // 6 Executive Information Attribute Tiles (2 Columns x 3 Rows)
            TableLayoutPanel tblProfile = new TableLayoutPanel
            {
                Location = new Point(20, 150),
                ColumnCount = 2,
                RowCount = 3,
                BackColor = Color.Transparent,
                Size = new Size(profileCard.Width - 40, 305)
            };
            tblProfile.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            tblProfile.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));

            for (int r = 0; r < 3; r++)
                tblProfile.RowStyles.Add(new RowStyle(SizeType.Absolute, 96f));

            profileCard.Controls.Add(tblProfile);

            // Tile 1: Full Name & Account
            AddSpaciousProfileTile(tblProfile, 0, 0, "👤", "FULL NAME & USER IDENTITY", SessionManager.CurrentFullName, $"Account Login ID: @{SessionManager.CurrentUsername}", Color.FromArgb(37, 99, 235));
            // Tile 2: Department & Railway Org
            AddSpaciousProfileTile(tblProfile, 1, 0, "🏢", "DEPARTMENT & RAILWAY DIVISION", SessionManager.CurrentDepartment, $"Organization: {SessionManager.CurrentCollegeOrOrg}", Color.FromArgb(124, 58, 237));

            // Tile 3: Designation / Role
            AddSpaciousProfileTile(tblProfile, 0, 1, "🎓", "DESIGNATION & OPERATING ROLE", SessionManager.CurrentCourse, "Authorization Level: Station Master / Traffic Controller", Color.FromArgb(217, 119, 6));
            // Tile 4: Official Email & Contact
            AddSpaciousProfileTile(tblProfile, 1, 1, "📧", "OFFICIAL COMMUNICATIONS", SessionManager.CurrentEmail, $"Official Contact: {SessionManager.CurrentPhone}", Color.FromArgb(22, 163, 74));

            // Tile 5: Station / Office Address
            string addr = string.IsNullOrWhiteSpace(SessionManager.CurrentAddress) ? "Headquarters Station Operations Hub" : SessionManager.CurrentAddress;
            AddSpaciousProfileTile(tblProfile, 0, 2, "📍", "POSTING & STATION ADDRESS", addr, "Assigned Section: South Central Division", Color.FromArgb(2, 132, 199));
            // Tile 6: Security & Session Log
            AddSpaciousProfileTile(tblProfile, 1, 2, "🛡️", "SECURITY & SESSION AUDIT", $"{SessionManager.CurrentStatus} (Digitally Authenticated)", $"Logged In: {SessionManager.LoginTime:dd-MMM-yyyy  hh:mm:ss tt}", Color.FromArgb(71, 85, 105));

            currentY += 490;

            // ═════════════════════════════════════════════════════════════════════
            // 4. GENERAL SAFETY & SECURE OPERATIONS NOTICE
            // ═════════════════════════════════════════════════════════════════════
            Panel safetyPanel = new Panel
            {
                Location = new Point(0, currentY),
                Size = new Size(container.Width, 110),
                BackColor = Color.FromArgb(240, 253, 244) // Light Mint / Safe Green (#F0FDF4)
            };
            safetyPanel.Paint += (s, e) => DrawCardBorder(e.Graphics, safetyPanel.ClientRectangle, Color.FromArgb(187, 247, 208), 1);
            container.Controls.Add(safetyPanel);

            // Left Emerald Accent
            Panel greenAccent = new Panel
            {
                Dock = DockStyle.Left,
                Width = 5,
                BackColor = Color.FromArgb(22, 163, 74) // Emerald Green
            };
            safetyPanel.Controls.Add(greenAccent);

            Label lblSafetyTitle = new Label
            {
                Text = "🛡️  Safe & Secure Station Operations",
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 83, 45), // Deep Forest Green (#14532D)
                Location = new Point(20, 14),
                AutoSize = true
            };
            safetyPanel.Controls.Add(lblSafetyTitle);

            Label lblSafetyText = new Label
            {
                Text = "• Safety First: Ensure safe, accurate, and authorized station operations during duty hours.\n" +
                       "• Register Security: All station register entries are digitally authenticated and securely audited.\n" +
                       "• Shift Handover: Verify that all entries are up-to-date and complete relief acknowledgment before logoff.",
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(22, 101, 52), // Dark Green (#166534)
                Location = new Point(20, 40),
                Size = new Size(container.Width - 50, 60)
            };
            safetyPanel.Controls.Add(lblSafetyText);

            currentY += 125;

            // ═════════════════════════════════════════════════════════════════════
            // 5. FOOTER
            // ═════════════════════════════════════════════════════════════════════
            Label lblFooter = new Label
            {
                Text = "Indian Railways Train Management System (TMS)  •  Station Operations Portal  •  Authorized Session",
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(0, currentY),
                Size = new Size(container.Width, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };
            container.Controls.Add(lblFooter);

            currentY += 40;

            // Responsive dynamic resize handler
            container.Resize += (s, e) =>
            {
                int w = container.Width;
                welcomeBanner.Width = w;
                heroRegisters.Width = w;
                safetyPanel.Width = w;
                lblSafetyText.Width = w - 50;
                profileCard.Width = w;
                profHeaderBar.Width = w - 40;
                pnlBadges.Width = profHeaderBar.Width - 105;
                tblProfile.Width = w - 40;
                lblFooter.Width = w;
            };

            content.ResumeLayout(true);
        }

        private Label CreatePillBadge(string text, Color bg, Color fg)
        {
            Label lbl = new Label
            {
                Text = text,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = fg,
                BackColor = bg,
                AutoSize = true,
                Margin = new Padding(0, 0, 10, 0),
                Padding = new Padding(10, 4, 10, 4)
            };
            lbl.Paint += (s, e) => {
                using (Pen p = new Pen(Color.FromArgb(60, fg.R, fg.G, fg.B), 1))
                    e.Graphics.DrawRectangle(p, 0, 0, lbl.Width - 1, lbl.Height - 1);
            };
            return lbl;
        }

        private void AddSpaciousProfileTile(TableLayoutPanel tbl, int col, int row, string icon, string title, string val, string subVal, Color accentColor)
        {
            Panel tile = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 250, 252),
                Margin = new Padding(6, 4, 6, 4),
                Padding = new Padding(12, 10, 12, 10)
            };
            tile.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(226, 232, 240), 1))
                    e.Graphics.DrawRectangle(pen, 0, 0, tile.Width - 1, tile.Height - 1);

                using (var brush = new SolidBrush(accentColor))
                    e.Graphics.FillRectangle(brush, 0, 0, 4, tile.Height);
            };

            // Left Icon Badge
            Label lblIcon = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI Emoji", 14f),
                Location = new Point(14, 14),
                Size = new Size(32, 32),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            tile.Controls.Add(lblIcon);

            // Title Label
            Label lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 8f, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(52, 8),
                AutoSize = true
            };
            tile.Controls.Add(lblTitle);

            // Main Value
            string displayVal = string.IsNullOrWhiteSpace(val) ? "—" : val;
            Label lblVal = new Label
            {
                Text = displayVal,
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(52, 26),
                AutoSize = true
            };
            tile.Controls.Add(lblVal);

            // Sub Value
            if (!string.IsNullOrWhiteSpace(subVal))
            {
                Label lblSubVal = new Label
                {
                    Text = subVal,
                    Font = new Font("Segoe UI", 8.5f, FontStyle.Regular),
                    ForeColor = Color.FromArgb(100, 116, 139),
                    Location = new Point(52, 48),
                    AutoSize = true
                };
                tile.Controls.Add(lblSubVal);
            }

            tile.MouseEnter += (s, e) => { tile.BackColor = Color.FromArgb(238, 246, 255); tile.Invalidate(); };
            tile.MouseLeave += (s, e) => { tile.BackColor = Color.FromArgb(248, 250, 252); tile.Invalidate(); };

            tbl.Controls.Add(tile, col, row);
        }

        private Panel CreateCardPanel(int x, int y, int width, int height)
        {
            Panel p = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(width, height),
                BackColor = Color.White
            };
            p.Paint += (s, e) => DrawCardBorder(e.Graphics, p.ClientRectangle, Color.FromArgb(226, 232, 240), 1);
            return p;
        }

        private void DrawCardBorder(Graphics g, Rectangle rect, Color borderColor, int borderWidth)
        {
            using (var pen = new Pen(borderColor, borderWidth))
            {
                g.DrawRectangle(pen, 0, 0, rect.Width - 1, rect.Height - 1);
            }
        }

        private string GetInitials(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName)) return "U";
            var parts = fullName.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1) return parts[0].Substring(0, Math.Min(2, parts[0].Length)).ToUpper();
            return (parts[0][0].ToString() + parts[parts.Length - 1][0].ToString()).ToUpper();
        }

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

        private void DoLogout()
        {
            if (MessageBox.Show("Are you sure you want to log out from the Train Management System?", 
                "Confirm Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (liveClockTimer != null)
                {
                    liveClockTimer.Stop();
                    liveClockTimer.Dispose();
                }
                SessionManager.Logout();
                new LoginForm().Show();
                this.Close();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (liveClockTimer != null)
            {
                liveClockTimer.Stop();
                liveClockTimer.Dispose();
            }
            base.OnFormClosing(e);
        }
    }
}
