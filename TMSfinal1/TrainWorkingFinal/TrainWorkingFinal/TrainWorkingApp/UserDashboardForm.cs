using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TrainWorkingApp
{
    /// <summary>
    /// Professional Indian Railways & IRCTC Inspired Enterprise User Dashboard.
    /// Clean layout, unclipped text, full ampersand rendering (UseMnemonic = false),
    /// live railway digital clock, and complete operator authorization profile.
    /// </summary>
    public class UserDashboardForm : Form
    {
        private Timer liveClockTimer;
        private Label lblLiveClock;
        private Panel body;
        private Panel header;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // WS_EX_COMPOSITED - eliminate flicker
                return cp;
            }
        }

        public UserDashboardForm()
        {
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Font = new Font("Segoe UI", 9.75f, FontStyle.Regular);
            InitializeComponent();
            StartLiveClock();
        }

        private void InitializeComponent()
        {
            this.Text = "TMS – Station Operations & Operator Profile Portal";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MinimumSize = new Size(1150, 750);
            this.BackColor = Color.FromArgb(241, 245, 249);

            // WinForms docking order: Fill first, Top last
            body = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(241, 245, 249)
            };
            this.Controls.Add(body);

            header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 72,
                BackColor = Color.FromArgb(20, 48, 105) // #143069 Indian Railways Navy
            };
            this.Controls.Add(header);

            BuildHeader(header);
            BuildBody(body);
        }

        private void BuildHeader(Panel headerPanel)
        {
            Label lblAppTitle = new Label
            {
                Text = "TRAIN WORKING MANAGEMENT SYSTEM",
                Font = new Font("Segoe UI", 13.5f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(22, 13),
                AutoSize = true,
                UseMnemonic = false,
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(lblAppTitle);

            Label lblAppSub = new Label
            {
                Text = "Indian Railways • Central Station Operations & Operational Authorities Console",
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(200, 225, 255),
                Location = new Point(24, 40),
                AutoSize = true,
                UseMnemonic = false,
                BackColor = Color.Transparent
            };
            headerPanel.Controls.Add(lblAppSub);

            // Live Digital Railway Clock
            lblLiveClock = new Label
            {
                Text = $"{DateTime.Now:ddd, dd-MMM-yyyy}   {DateTime.Now:HH:mm:ss} IST",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(254, 240, 138), // Golden Amber
                BackColor = Color.FromArgb(15, 35, 80),
                Padding = new Padding(12, 6, 12, 6),
                AutoSize = true,
                UseMnemonic = false,
                TextAlign = ContentAlignment.MiddleCenter
            };
            headerPanel.Controls.Add(lblLiveClock);

            // Right Action Controls
            Button btnHeaderRegisters = new Button
            {
                Text = "🚆 Open Authorities",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                BackColor = Color.FromArgb(249, 115, 22), // IRCTC Orange
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(165, 38),
                Cursor = Cursors.Hand
            };
            btnHeaderRegisters.FlatAppearance.BorderSize = 0;
            btnHeaderRegisters.Click += (s, e) => { new AuthorityHubForm().Show(); this.Hide(); };
            headerPanel.Controls.Add(btnHeaderRegisters);

            Button btnHeaderLogout = new Button
            {
                Text = "🚪 Logout",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                BackColor = Color.FromArgb(220, 38, 38), // Crimson Red
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(100, 38),
                Cursor = Cursors.Hand
            };
            btnHeaderLogout.FlatAppearance.BorderSize = 0;
            btnHeaderLogout.Click += (s, e) => DoLogout();
            headerPanel.Controls.Add(btnHeaderLogout);

            // User Identity Badge
            string deptName = string.IsNullOrWhiteSpace(SessionManager.CurrentDepartment) ? "Operating" : SessionManager.CurrentDepartment;
            string userName = string.IsNullOrWhiteSpace(SessionManager.CurrentFullName) ? "Station Operator" : SessionManager.CurrentFullName;

            Panel userChip = new Panel
            {
                BackColor = Color.FromArgb(15, 35, 80),
                Height = 42,
                Cursor = Cursors.Default
            };

            Label lblAvatar = new Label
            {
                Text = GetInitials(userName),
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 48, 105),
                BackColor = Color.FromArgb(254, 240, 138),
                Size = new Size(30, 30),
                Location = new Point(6, 6),
                TextAlign = ContentAlignment.MiddleCenter
            };
            userChip.Controls.Add(lblAvatar);

            Label lblUserName = new Label
            {
                Text = userName,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(42, 3),
                AutoSize = true,
                UseMnemonic = false
            };
            userChip.Controls.Add(lblUserName);

            Label lblUserRole = new Label
            {
                Text = deptName,
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(190, 215, 250),
                Location = new Point(42, 22),
                AutoSize = true,
                UseMnemonic = false
            };
            userChip.Controls.Add(lblUserRole);
            headerPanel.Controls.Add(userChip);

            Action layoutHeader = () =>
            {
                int r = headerPanel.ClientSize.Width - 18;
                btnHeaderLogout.Location = new Point(r - btnHeaderLogout.Width, 17);
                r -= (btnHeaderLogout.Width + 10);

                btnHeaderRegisters.Location = new Point(r - btnHeaderRegisters.Width, 17);
                r -= (btnHeaderRegisters.Width + 14);

                int textW = Math.Max(TextRenderer.MeasureText(userName, lblUserName.Font).Width,
                                     TextRenderer.MeasureText(deptName, lblUserRole.Font).Width);
                userChip.Width = Math.Max(220, textW + 55);
                userChip.Location = new Point(r - userChip.Width, 15);
                r -= (userChip.Width + 16);

                lblLiveClock.Location = new Point(Math.Max(500, r - lblLiveClock.Width), 21);
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
                    lblLiveClock.Text = $"{DateTime.Now:ddd, dd-MMM-yyyy}   {DateTime.Now:HH:mm:ss} IST";
                }
            };
            liveClockTimer.Start();
        }

        private void BuildBody(Panel content)
        {
            content.SuspendLayout();
            content.Controls.Clear();

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
                int targetWidth = Math.Max(1050, content.ClientSize.Width - 50);
                container.Width = targetWidth;
            };
            container.Width = Math.Max(1050, content.ClientSize.Width - 50);

            int currentY = 0;

            // ═════════════════════════════════════════════════════════════════════
            // 1. WELCOME & ACTIVE SHIFT BANNER
            // ═════════════════════════════════════════════════════════════════════
            Panel welcomeBanner = CreateCardPanel(0, currentY, container.Width, 75);
            container.Controls.Add(welcomeBanner);

            Panel accentBar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 6,
                BackColor = Color.FromArgb(22, 163, 74) // Emerald Green
            };
            welcomeBanner.Controls.Add(accentBar);

            Label lblWelcomeTitle = new Label
            {
                Text = $"Welcome, {SessionManager.CurrentFullName}",
                Font = new Font("Segoe UI", 16.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(24, 22),
                AutoSize = true,
                UseMnemonic = false
            };
            welcomeBanner.Controls.Add(lblWelcomeTitle);

            Panel pnlStatusRight = new Panel
            {
                Size = new Size(240, 50),
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            welcomeBanner.Controls.Add(pnlStatusRight);

            Label badgeStatus = new Label
            {
                Text = "● ACTIVE DUTY SESSION",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(22, 101, 52), // Dark Green
                BackColor = Color.FromArgb(220, 252, 231), // Mint green
                Padding = new Padding(12, 5, 12, 5),
                Size = new Size(210, 30),
                Location = new Point(15, 12),
                TextAlign = ContentAlignment.MiddleCenter,
                UseMnemonic = false
            };
            pnlStatusRight.Controls.Add(badgeStatus);

            welcomeBanner.Resize += (s, e) =>
            {
                pnlStatusRight.Location = new Point(welcomeBanner.ClientSize.Width - 250, 12);
            };
            pnlStatusRight.Location = new Point(welcomeBanner.ClientSize.Width - 250, 12);

            currentY += 92;

            // ═════════════════════════════════════════════════════════════════════
            // 2. OPERATIONAL AUTHORITIES ACCESS BANNER
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
                    Color.FromArgb(253, 232, 244),
                    Color.FromArgb(237, 233, 254),
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

            Label lblHeroTitle = new Label
            {
                Text = "Train Working Authorities & Registers Management – All Modules",
                Font = new Font("Segoe UI", 13.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(88, 28, 135),
                Location = new Point(24, 26),
                AutoSize = true,
                UseMnemonic = false,
                BackColor = Color.Transparent
            };
            heroRegisters.Controls.Add(lblHeroTitle);

            Button btnOpenRegisters = new Button
            {
                Text = "🚆 Open Authorities Hub ➔",
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                BackColor = Color.FromArgb(124, 58, 237),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(260, 44),
                Cursor = Cursors.Hand
            };
            btnOpenRegisters.FlatAppearance.BorderSize = 0;
            btnOpenRegisters.Click += (s, e) => { new AuthorityHubForm().Show(); this.Hide(); };
            heroRegisters.Controls.Add(btnOpenRegisters);

            Action layoutHero = () =>
            {
                btnOpenRegisters.Location = new Point(heroRegisters.ClientSize.Width - 280, 18);
            };
            heroRegisters.Resize += (s, e) => layoutHero();
            layoutHero();

            currentY += 95;

            // ═════════════════════════════════════════════════════════════════════
            // 3. EMPLOYEE PROFILE & AUTHORIZATION CARD (10 TILES)
            // ═════════════════════════════════════════════════════════════════════
            Panel profileCard = CreateCardPanel(0, currentY, container.Width, 340);
            container.Controls.Add(profileCard);

            Label lblProfHeader = new Label
            {
                Text = "Registered Employee Profile & Station Authorization Details",
                Font = new Font("Segoe UI", 13.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 61, 119),
                Location = new Point(24, 18),
                AutoSize = true,
                UseMnemonic = false
            };
            profileCard.Controls.Add(lblProfHeader);

            Label lblProfSub = new Label
            {
                Text = "Official records and operating credentials authorized by the Railway Station Administrator.",
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(26, 46),
                AutoSize = true,
                UseMnemonic = false
            };
            profileCard.Controls.Add(lblProfSub);

            TableLayoutPanel tblProfile = new TableLayoutPanel
            {
                Location = new Point(24, 76),
                ColumnCount = 2,
                RowCount = 5,
                BackColor = Color.Transparent,
                Dock = DockStyle.None,
                Size = new Size(profileCard.Width - 48, 240)
            };
            tblProfile.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
            tblProfile.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));

            for (int r = 0; r < 5; r++)
                tblProfile.RowStyles.Add(new RowStyle(SizeType.Absolute, 46f));

            profileCard.Controls.Add(tblProfile);

            AddProfileTile(tblProfile, 0, 0, "Full Name", SessionManager.CurrentFullName);
            AddProfileTile(tblProfile, 1, 0, "Username / Employee ID", SessionManager.CurrentUsername);

            AddProfileTile(tblProfile, 0, 1, "Department", SessionManager.CurrentDepartment);
            AddProfileTile(tblProfile, 1, 1, "Designation / Role", SessionManager.CurrentCourse);

            AddProfileTile(tblProfile, 0, 2, "Official Email Address", SessionManager.CurrentEmail);
            AddProfileTile(tblProfile, 1, 2, "Contact Phone Number", SessionManager.CurrentPhone);

            AddProfileTile(tblProfile, 0, 3, "Railway Division / Organization", SessionManager.CurrentCollegeOrOrg);
            AddProfileTile(tblProfile, 1, 3, "Station / Office Quarter Address", SessionManager.CurrentAddress);

            AddProfileTile(tblProfile, 0, 4, "Account Authorization Status", $"{SessionManager.CurrentStatus} (Authorized)");
            AddProfileTile(tblProfile, 1, 4, "Active Session Established", SessionManager.LoginTime.ToString("dd-MMM-yyyy  hh:mm:ss tt"));

            currentY += 355;

            // ═════════════════════════════════════════════════════════════════════
            // 4. SAFE & SECURE STATION OPERATIONS POLICY
            // ═════════════════════════════════════════════════════════════════════
            Panel safetyPanel = new Panel
            {
                Location = new Point(0, currentY),
                Size = new Size(container.Width, 110),
                BackColor = Color.FromArgb(240, 253, 244)
            };
            safetyPanel.Paint += (s, e) => DrawCardBorder(e.Graphics, safetyPanel.ClientRectangle, Color.FromArgb(187, 247, 208), 1);
            container.Controls.Add(safetyPanel);

            Panel greenAccent = new Panel
            {
                Dock = DockStyle.Left,
                Width = 5,
                BackColor = Color.FromArgb(22, 163, 74)
            };
            safetyPanel.Controls.Add(greenAccent);

            Label lblSafetyTitle = new Label
            {
                Text = "Safe & Secure Station Operations",
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                ForeColor = Color.FromArgb(20, 83, 45),
                Location = new Point(20, 14),
                AutoSize = true,
                UseMnemonic = false
            };
            safetyPanel.Controls.Add(lblSafetyTitle);

            Label lblSafetyText = new Label
            {
                Text = "• Safety First: Ensure safe, accurate, and authorized station operations during duty hours.\n" +
                       "• Register Security: All operational authority form entries are digitally authenticated and securely audited.\n" +
                       "• Shift Handover: Verify that all entries are up-to-date and complete relief acknowledgment before logoff.",
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(22, 101, 52),
                Location = new Point(20, 40),
                Size = new Size(container.Width - 50, 60),
                UseMnemonic = false
            };
            safetyPanel.Controls.Add(lblSafetyText);

            currentY += 125;

            // ═════════════════════════════════════════════════════════════════════
            // 5. FOOTER
            // ═════════════════════════════════════════════════════════════════════
            Label lblFooter = new Label
            {
                Text = "Indian Railways Train Working Management System (TMS)  •  Station Operations Portal  •  Authorized Session",
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(0, currentY),
                Size = new Size(container.Width, 30),
                TextAlign = ContentAlignment.MiddleCenter,
                UseMnemonic = false
            };
            container.Controls.Add(lblFooter);

            currentY += 40;

            container.Resize += (s, e) =>
            {
                int w = container.Width;
                welcomeBanner.Width = w;
                heroRegisters.Width = w;
                safetyPanel.Width = w;
                lblSafetyText.Width = w - 50;
                profileCard.Width = w;
                tblProfile.Width = w - 48;
                lblFooter.Width = w;
            };

            content.ResumeLayout(true);
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

        private void AddProfileTile(TableLayoutPanel tbl, int col, int row, string labelText, string valText)
        {
            Panel tile = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 250, 252),
                Margin = new Padding(4, 3, 4, 3),
                Padding = new Padding(10, 5, 10, 5)
            };
            tile.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(226, 232, 240), 1))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, tile.Width - 1, tile.Height - 1);
                }
            };

            Label lblTitle = new Label
            {
                Text = labelText,
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(10, 4),
                AutoSize = true,
                UseMnemonic = false
            };
            tile.Controls.Add(lblTitle);

            string displayValue = string.IsNullOrWhiteSpace(valText) ? "—" : valText;
            Label lblVal = new Label
            {
                Text = displayValue,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(10, 22),
                AutoSize = true,
                AutoEllipsis = true,
                UseMnemonic = false
            };

            ToolTip tt = new ToolTip();
            tt.SetToolTip(lblVal, displayValue);

            tile.Controls.Add(lblVal);

            tile.MouseEnter += (s, e) =>
            {
                tile.BackColor = Color.FromArgb(238, 242, 255);
                tile.Invalidate();
            };
            tile.MouseLeave += (s, e) =>
            {
                tile.BackColor = Color.FromArgb(248, 250, 252);
                tile.Invalidate();
            };

            tbl.Controls.Add(tile, col, row);
        }

        private string GetInitials(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName)) return "U";
            var parts = fullName.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1) return parts[0].Substring(0, Math.Min(2, parts[0].Length)).ToUpper();
            return (parts[0][0].ToString() + parts[parts.Length - 1][0].ToString()).ToUpper();
        }

        private void DoLogout()
        {
            if (MessageBox.Show("Are you sure you want to log out from the Train Working Management System?", 
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
