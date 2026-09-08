using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace TMS
{
    /// <summary>
    /// Custom interactive module card control with smooth hover gradient elevations,
    /// rounded corners, illuminated borders, and right-side interactive pill badges.
    /// </summary>
    public class ModuleCardButton : Control
    {
        public string CategoryIcon { get; set; } = "🚆";
        public string CategoryTitle { get; set; } = "OPERATIONAL LIST";
        public string BadgeText { get; set; } = "14 Registers";
        public Color BaseColorStart { get; set; } = Color.FromArgb(30, 58, 138);
        public Color BaseColorEnd { get; set; } = Color.FromArgb(37, 99, 235);
        public Color HoverColorStart { get; set; } = Color.FromArgb(37, 99, 235);
        public Color HoverColorEnd { get; set; } = Color.FromArgb(59, 130, 246);
        public Color BorderGlowColor { get; set; } = Color.FromArgb(191, 219, 254);

        private bool isHovered = false;
        private bool isPressed = false;

        public ModuleCardButton()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
            this.DoubleBuffered = true;
            this.BackColor = Color.Transparent;
            this.Cursor = Cursors.Hand;
            this.Size = new Size(640, 78);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            isHovered = true;
            this.Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            isHovered = false;
            isPressed = false;
            this.Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                isPressed = true;
                this.Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            isPressed = false;
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            Rectangle rect = this.ClientRectangle;
            rect.Width -= 1;
            rect.Height -= 1;

            if (isPressed)
            {
                rect.Offset(1, 1);
                rect.Width -= 1;
                rect.Height -= 1;
            }

            int radius = 14;
            using (GraphicsPath path = CreateRoundedRectanglePath(rect, radius))
            {
                // Background Gradient Fill
                Color start = isHovered ? HoverColorStart : BaseColorStart;
                Color end = isHovered ? HoverColorEnd : BaseColorEnd;

                using (LinearGradientBrush brush = new LinearGradientBrush(rect, start, end, LinearGradientMode.Horizontal))
                {
                    g.FillPath(brush, path);
                }

                // Outer Illuminated Glow Border on Hover
                Color borderColor = isHovered ? BorderGlowColor : Color.FromArgb(75, 255, 255, 255);
                float borderWidth = isHovered ? 2.4f : 1.2f;
                using (Pen pen = new Pen(borderColor, borderWidth))
                {
                    g.DrawPath(pen, path);
                }
            }

            // 1. Icon Container Box (Centered Vertically)
            int iconBoxSize = 50;
            int iconBoxX = rect.X + 16;
            int iconBoxY = rect.Y + (rect.Height - iconBoxSize) / 2;
            Rectangle iconBoxRect = new Rectangle(iconBoxX, iconBoxY, iconBoxSize, iconBoxSize);

            using (GraphicsPath iconBoxPath = CreateRoundedRectanglePath(iconBoxRect, 12))
            {
                Color boxFill = isHovered ? Color.FromArgb(60, 255, 255, 255) : Color.FromArgb(30, 255, 255, 255);
                using (SolidBrush boxBrush = new SolidBrush(boxFill))
                {
                    g.FillPath(boxBrush, iconBoxPath);
                }
                using (Pen boxPen = new Pen(Color.FromArgb(100, 255, 255, 255), 1f))
                {
                    g.DrawPath(boxPen, iconBoxPath);
                }
            }

            // 2. Center the Symbol inside the Icon Box
            using (Font iconFont = new Font("Segoe UI Emoji", 20f, FontStyle.Regular))
            {
                TextRenderer.DrawText(g, CategoryIcon, iconFont, iconBoxRect, Color.White, 
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            }

            // 3. Category Title
            int titleX = iconBoxX + iconBoxSize + 16;
            using (Font titleFont = new Font("Segoe UI", 13.5f, FontStyle.Bold))
            {
                Rectangle titleRect = new Rectangle(titleX, rect.Y, rect.Width - titleX - 190, rect.Height);
                TextRenderer.DrawText(g, CategoryTitle, titleFont, titleRect, Color.White, TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            }

            // 4. Right Pill Badge
            using (Font badgeFont = new Font("Segoe UI", 10.5f, FontStyle.Bold))
            {
                string pillText = isHovered ? $"{BadgeText}  ➔" : $"{BadgeText}  ›";
                Size textSize = TextRenderer.MeasureText(pillText, badgeFont);
                int badgeW = textSize.Width + 26;
                int badgeH = 38;
                int badgeX = rect.Right - badgeW - 16;
                int badgeY = rect.Y + (rect.Height - badgeH) / 2;

                Rectangle badgeRect = new Rectangle(badgeX, badgeY, badgeW, badgeH);
                int badgeRadius = 19; // Rounded pill

                using (GraphicsPath badgePath = CreateRoundedRectanglePath(badgeRect, badgeRadius))
                {
                    Color badgeBg = isHovered ? Color.FromArgb(80, 255, 255, 255) : Color.FromArgb(40, 255, 255, 255);
                    using (SolidBrush badgeBrush = new SolidBrush(badgeBg))
                    {
                        g.FillPath(badgeBrush, badgePath);
                    }

                    Color badgeBorder = isHovered ? Color.FromArgb(240, 255, 255, 255) : Color.FromArgb(120, 255, 255, 255);
                    using (Pen badgePen = new Pen(badgeBorder, 1.2f))
                    {
                        g.DrawPath(badgePen, badgePath);
                    }
                }

                TextRenderer.DrawText(g, pillText, badgeFont, badgeRect, Color.White, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            }
        }

        private static GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            if (rect.Width <= 0 || rect.Height <= 0) return path;

            int d = radius * 2;
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }

    /// <summary>
    /// Indian Railways, IRCTC & ConfirmTkt Inspired Portal.
    /// Features modern Frosted Glass / Translucent Glassmorphism styling
    /// on both the left selection card and the top navigation bar with zero flicker.
    /// </summary>
    public class MainClassesForm : Form
    {
        private Label lblUserBadge;
        private Label lblDeptBadge;
        private Panel topNavBar;
        private Panel subNavBar;
        private Panel card;
        private Image bgImage;
        private ModuleCardButton[] moduleButtons;

        public MainClassesForm()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.DoubleBuffered = true;
            LoadBg();
            InitializeComponent();
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

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (bgImage != null)
            {
                e.Graphics.InterpolationMode = InterpolationMode.Low;
                e.Graphics.DrawImage(bgImage, this.ClientRectangle);
            }
            else
            {
                base.OnPaintBackground(e);
            }
        }

        private void LoadBg()
        {
            try
            {
                string[] paths = {
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Resources\dashboard_bg.jpg"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\dashboard_bg.jpg"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Resources\vande_bharat_bg.png"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\vande_bharat_bg.png"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Resources\train_bg.png"),
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources\\train_bg.png")
                };
                foreach (var p in paths)
                {
                    if (File.Exists(p))
                    {
                        byte[] bytes = File.ReadAllBytes(p);
                        using (MemoryStream ms = new MemoryStream(bytes))
                        {
                            bgImage = new Bitmap(ms);
                        }
                        break;
                    }
                }
            }
            catch { }
        }

        private void InitializeComponent()
        {
            this.Text = "Indian Railways – Train Management System (TMS)";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MinimumSize = new Size(1080, 760);
            this.BackColor = Color.FromArgb(244, 246, 249);

            // ── 1. Top Navbar Header (Frosted Glass Translucent IRCTC Style) ──
            topNavBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 84,
                BackColor = Color.FromArgb(248, 255, 255, 255)
            };
            topNavBar.Paint += (s, e) => {
                using (var pen = new Pen(Color.FromArgb(200, 215, 230), 1.2f))
                    e.Graphics.DrawLine(pen, 0, topNavBar.Height - 1, topNavBar.Width, topNavBar.Height - 1);
            };

            // Left Logo & Title
            Label lblLogo = new Label
            {
                Text = "🚆",
                Font = new Font("Segoe UI Emoji", 26),
                Location = new Point(22, 16),
                Size = new Size(52, 52),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            topNavBar.Controls.Add(lblLogo);

            Label lblAppTitle = new Label
            {
                Text = "INDIAN RAILWAYS",
                Font = new Font("Segoe UI", 16.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 58, 138), // #1E3A8A Royal Navy
                Location = new Point(80, 16),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            topNavBar.Controls.Add(lblAppTitle);

            // Subtitle with high-contrast, crystal-clear non-overlapping typography
            Label lblMotto = new Label
            {
                Text = "Safety  •  Security  •  Punctuality  •  Train Management System",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(71, 85, 105), // Slate-600: clean and sharp
                Location = new Point(82, 48),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            topNavBar.Controls.Add(lblMotto);

            // Right Action Controls
            Button btnLogout = new Button
            {
                Text = "🚪 Logout",
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                BackColor = Color.FromArgb(220, 38, 38), // Crimson Red
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(118, 40),
                Cursor = Cursors.Hand
            };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.MouseEnter += (s, e) => btnLogout.BackColor = Color.FromArgb(185, 28, 28);
            btnLogout.MouseLeave += (s, e) => btnLogout.BackColor = Color.FromArgb(220, 38, 38);
            btnLogout.Click += BtnLogout_Click;
            topNavBar.Controls.Add(btnLogout);

            Button btnPanel = new Button
            {
                Text = SessionManager.CurrentRole == UserRole.Admin ? "🛡️ Admin Panel" : "🏠 My Profile",
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                BackColor = Color.FromArgb(251, 121, 43), // #FB792B IRCTC Orange
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(150, 40),
                Cursor = Cursors.Hand
            };
            btnPanel.FlatAppearance.BorderSize = 0;
            btnPanel.MouseEnter += (s, e) => btnPanel.BackColor = Color.FromArgb(234, 88, 12);
            btnPanel.MouseLeave += (s, e) => btnPanel.BackColor = Color.FromArgb(251, 121, 43);
            btnPanel.Click += BtnDashboard_Click;
            topNavBar.Controls.Add(btnPanel);

            // Modern Unified User Capsule Pill - Clear, generous spacing, NO avatar overlap
            Panel pnlUserCapsule = new Panel
            {
                Name = "pnlUserCapsule",
                Height = 44,
                BackColor = Color.FromArgb(238, 242, 255),
                Cursor = Cursors.Default
            };
            pnlUserCapsule.Paint += (s, e) =>
            {
                using (Pen p = new Pen(Color.FromArgb(199, 210, 254), 1.2f))
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.DrawRectangle(p, 0, 0, pnlUserCapsule.Width - 1, pnlUserCapsule.Height - 1);
                }
            };

            Label lblAvatar = new Label
            {
                Text = "👤",
                Font = new Font("Segoe UI Emoji", 12f),
                ForeColor = Color.FromArgb(30, 58, 138),
                Location = new Point(12, 10),
                Size = new Size(24, 24),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            pnlUserCapsule.Controls.Add(lblAvatar);

            lblUserBadge = new Label
            {
                Text = SessionManager.CurrentFullName,
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 58, 138),
                Location = new Point(40, 11),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            pnlUserCapsule.Controls.Add(lblUserBadge);

            string roleDisplay = SessionManager.CurrentRole == UserRole.Admin ? "Administrator" : SessionManager.CurrentDepartment;
            lblDeptBadge = new Label
            {
                Text = $"|   {roleDisplay}",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular),
                ForeColor = Color.FromArgb(71, 85, 105),
                Location = new Point(lblUserBadge.Right + 8, 12),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            pnlUserCapsule.Controls.Add(lblDeptBadge);
            topNavBar.Controls.Add(pnlUserCapsule);

            // ── 2. Navy Sub-Header Bar (Clean Professional Design, 2nd Image Matter Removed) ──
            subNavBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 50,
                BackColor = Color.FromArgb(240, 20, 42, 88)
            };
            subNavBar.Paint += (s, e) => {
                using (var pen = new Pen(Color.FromArgb(96, 165, 250), 2f))
                    e.Graphics.DrawLine(pen, 0, subNavBar.Height - 1, subNavBar.Width, subNavBar.Height - 1);
            };

            Label lblSubTitle = new Label
            {
                UseMnemonic = false, // Prevents & from being treated as accelerator mnemonic
                Text = "STATION REGISTERS PORTAL   •   CENTRALIZED RAILWAY OPERATIONS & AUDIT SYSTEM",
                Font = new Font("Segoe UI", 11.5f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(24, 15),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            subNavBar.Controls.Add(lblSubTitle);

            // Docking order: subNavBar added FIRST, topNavBar added SECOND so topNavBar docks at top
            this.Controls.Add(subNavBar);
            this.Controls.Add(topNavBar);

            // ── 3. Translucent Frosted Glass Left-Aligned Card Container ───────
            card = new Panel
            {
                Name = "MainCard",
                BackColor = Color.FromArgb(205, 255, 255, 255), // Translucent Frosted Glass
                Size = new Size(700, 580)
            };
            card.Paint += Card_Paint;
            this.Controls.Add(card);

            // Card Header
            Label cardTitle = new Label
            {
                Text = "SELECT REGISTER MODULE",
                Font = new Font("Segoe UI", 17.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 58, 138),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(20, 18),
                Size = new Size(660, 32),
                BackColor = Color.Transparent
            };
            card.Controls.Add(cardTitle);

            Label cardSub = new Label
            {
                Text = "Click on any category to access station registers",
                Font = new Font("Segoe UI", 10.5f),
                ForeColor = Color.FromArgb(71, 85, 105),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(20, 52),
                Size = new Size(660, 24),
                BackColor = Color.Transparent
            };
            card.Controls.Add(cardSub);

            Panel sep = new Panel
            {
                Location = new Point(30, 84),
                Size = new Size(640, 1),
                BackColor = Color.FromArgb(180, 205, 225)
            };
            card.Controls.Add(sep);

            // 5 Modern Enterprise Module Categories with Counts & Rich Gradients
            string[] icons = { "🚆", "🛠️", "⚡", "🚨", "📊" };
            string[] titles = {
                "OPERATIONAL LIST",
                "MAINTENANCE SUB",
                "INFRASTRUCTURE SUB",
                "SAFETY LIST",
                "ADMIN & REPORTS"
            };
            string[] badges = {
                "14 Registers",
                "13 Registers",
                "6 Registers",
                "7 Registers",
                "Reports & Audit"
            };

            // Base Gradient Colors
            Color[] baseStarts = {
                Color.FromArgb(30, 58, 138),   // Royal Navy (#1E3A8A)
                Color.FromArgb(6, 95, 70),     // Deep Emerald (#065F46)
                Color.FromArgb(146, 64, 14),   // Deep Bronze Amber (#92400E)
                Color.FromArgb(153, 27, 27),   // Deep Ruby Red (#991B1B)
                Color.FromArgb(91, 33, 182)    // Deep Amethyst (#5B21B6)
            };
            Color[] baseEnds = {
                Color.FromArgb(37, 99, 235),   // Bright Sapphire (#2563EB)
                Color.FromArgb(5, 150, 105),   // Bright Emerald (#059669)
                Color.FromArgb(217, 119, 6),   // Bright Warm Amber (#D97706)
                Color.FromArgb(220, 38, 38),   // Bright Crimson (#DC2626)
                Color.FromArgb(124, 58, 237)   // Bright Royal Violet (#7C3AED)
            };

            // Vibrant Hover Colors
            Color[] hoverStarts = {
                Color.FromArgb(37, 99, 235),   // Bright Sapphire Blue
                Color.FromArgb(5, 150, 105),   // Bright Emerald
                Color.FromArgb(217, 119, 6),   // Bright Amber
                Color.FromArgb(220, 38, 38),   // Bright Red
                Color.FromArgb(124, 58, 237)   // Bright Violet
            };
            Color[] hoverEnds = {
                Color.FromArgb(59, 130, 246),  // Sky Blue
                Color.FromArgb(16, 185, 129),  // Mint Emerald
                Color.FromArgb(245, 158, 11),  // Amber Glow
                Color.FromArgb(239, 68, 68),   // Light Crimson
                Color.FromArgb(139, 92, 246)   // Purple Glow
            };

            Color[] glowColors = {
                Color.FromArgb(191, 219, 254),
                Color.FromArgb(167, 243, 208),
                Color.FromArgb(253, 230, 138),
                Color.FromArgb(254, 202, 202),
                Color.FromArgb(221, 214, 254)
            };

            int[] modIds = { 1, 2, 3, 4, 5 };
            string[] modNames = { "Operational List", "Maintenance Sub", "Infrastructure Sub", "Safety List", "Admin List" };

            moduleButtons = new ModuleCardButton[titles.Length];
            int btnY = 98;
            int btnHeight = 78;
            int btnGap = 14;
            int btnWidth = 640;
            int btnX = (card.Width - btnWidth) / 2;

            for (int i = 0; i < titles.Length; i++)
            {
                int idx = i;
                var cardBtn = new ModuleCardButton
                {
                    CategoryIcon = icons[i],
                    CategoryTitle = titles[i],
                    BadgeText = badges[i],
                    BaseColorStart = baseStarts[i],
                    BaseColorEnd = baseEnds[i],
                    HoverColorStart = hoverStarts[i],
                    HoverColorEnd = hoverEnds[i],
                    BorderGlowColor = glowColors[i],
                    Location = new Point(btnX, btnY + idx * (btnHeight + btnGap)),
                    Size = new Size(btnWidth, btnHeight)
                };

                cardBtn.Click += (s, e) =>
                {
                    if (idx == 4) // "ADMIN & REPORTS" -> directly open Form_Reg041_DynamicReports
                    {
                        var dynReports = new Form_Reg041_DynamicReports();
                        this.SuspendLayout();
                        this.Hide();
                        this.ResumeLayout(false);
                        dynReports.FormClosed += (s2, e2) =>
                        {
                            if (!this.IsDisposed)
                            {
                                this.SuspendLayout();
                                this.Show();
                                this.BringToFront();
                                this.ResumeLayout(true);
                                this.Update();
                            }
                        };
                        dynReports.Show();
                    }
                    else
                    {
                        var f = new RegisterSelectionForm(modIds[idx], modNames[idx], this);
                        // Show child first so it's ready, then hide self — avoids black flash
                        this.SuspendLayout();
                        this.Hide();
                        this.ResumeLayout(false);
                        f.Show();
                    }
                };
                card.Controls.Add(cardBtn);
                moduleButtons[i] = cardBtn;
            }

            // Navbar layout positioning
            Action layoutNavbar = () =>
            {
                int r = topNavBar.ClientSize.Width - 22;
                int mid = (topNavBar.ClientSize.Height - 40) / 2;
                btnLogout.Location = new Point(r - btnLogout.Width, mid);
                r -= (btnLogout.Width + 12);

                btnPanel.Location = new Point(r - btnPanel.Width, mid);
                r -= (btnPanel.Width + 18);

                var capsule = topNavBar.Controls["pnlUserCapsule"];
                if (capsule != null)
                {
                    int totalContentW = 40 + (lblUserBadge?.PreferredWidth ?? 0) + 12 + (lblDeptBadge?.PreferredWidth ?? 0) + 20;
                    capsule.Size = new Size(totalContentW, 44);
                    
                    int capsuleMid = (topNavBar.ClientSize.Height - 44) / 2;
                    int targetX = r - totalContentW;
                    
                    int minLeft = (lblMotto != null) ? lblMotto.Right + 24 : 450;
                    if (targetX < minLeft)
                    {
                        targetX = Math.Max(minLeft, targetX);
                    }

                    capsule.Location = new Point(targetX, capsuleMid);
                    if (lblDeptBadge != null && lblUserBadge != null)
                    {
                        lblDeptBadge.Location = new Point(lblUserBadge.Right + 8, 12);
                    }
                }
            };
            topNavBar.Resize += (s, e) => layoutNavbar();

            // Left-aligned card positioning
            Action alignCardLeft = () =>
            {
                int topOffset = (topNavBar?.Height ?? 84) + (subNavBar?.Height ?? 50);
                int availW = this.ClientSize.Width;
                int availH = this.ClientSize.Height - topOffset;

                int cw = 700;
                int ch = 580;

                int leftMargin = Math.Min(90, Math.Max(40, (availW - cw) / 6));
                int topMargin = topOffset + Math.Max(16, (availH - ch) / 2);

                card.Size = new Size(cw, ch);
                card.Location = new Point(leftMargin, topMargin);
                
                int bX = (card.Width - 640) / 2;
                for (int i = 0; i < moduleButtons.Length; i++)
                {
                    if (moduleButtons[i] != null)
                    {
                        moduleButtons[i].Location = new Point(bX, 98 + i * (78 + 14));
                        moduleButtons[i].Size = new Size(640, 78);
                    }
                }
            };

            this.Resize += (s, e) => alignCardLeft();
            this.VisibleChanged += (s, e) =>
            {
                if (this.Visible)
                {
                    layoutNavbar();
                    alignCardLeft();
                }
            };
            this.Shown += (s, e) =>
            {
                layoutNavbar();
                alignCardLeft();
            };
        }

        private void Card_Paint(object sender, PaintEventArgs e)
        {
            var p = sender as Panel;
            if (p == null) return;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // Semi-Transparent Frosted Glass Fill (80% Alpha)
            using (var b = new SolidBrush(Color.FromArgb(210, 255, 255, 255)))
            {
                e.Graphics.FillRectangle(b, p.ClientRectangle);
            }

            // Elegant Glass Edge Highlight
            using (var pen = new Pen(Color.FromArgb(235, 255, 255, 255), 1.6f))
            {
                var rect = new Rectangle(0, 0, p.Width - 1, p.Height - 1);
                int r = 18;
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

        private void BtnDashboard_Click(object sender, EventArgs e)
        {
            Form dash = (SessionManager.CurrentRole == UserRole.Admin) ? (Form)new AdminDashboardForm() : (Form)new UserDashboardForm();
            dash.Show();
            this.BeginInvoke(new Action(() => this.Hide()));
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to log out from the TMS session?", "Logout Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                SessionManager.Logout();
                new LoginForm().Show();
                this.Close();
            }
        }
    }
}