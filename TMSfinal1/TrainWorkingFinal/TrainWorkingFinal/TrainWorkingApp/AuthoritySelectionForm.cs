using System;
using System.Drawing;
using System.Windows.Forms;

namespace TrainWorkingApp
{
    /// <summary>
    /// Dedicated Operational Category Workspace Console.
    /// Features breadcrumb navigation, live authority search,
    /// form badge indicators (T/369-3b, T/511, etc.), and dual action buttons (Fill Form & View Records).
    /// </summary>
    public class AuthoritySelectionForm : Form
    {
        private readonly int categoryId;
        private TextBox txtSearch;
        private Panel registerCardsContainer;

        public AuthoritySelectionForm(int categoryId)
        {
            this.categoryId = categoryId;
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Font = new Font("Segoe UI", 9.75f, FontStyle.Regular);
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            string catTitle = GetCategoryTitle(categoryId);
            this.Text = catTitle + " – Operational Registers Console";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MinimumSize = new Size(1150, 750);
            this.BackColor = Color.FromArgb(241, 245, 249);

            // ── 1. Top Navigation Bar ─────────────────────────────────────────
            Panel topBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 72,
                BackColor = ThemeManager.IRCTCColors.PrimaryNavy
            };
            this.Controls.Add(topBar);

            Label lblLogo = new Label
            {
                Text = "📋",
                Font = new Font("Segoe UI Emoji", 20),
                ForeColor = Color.White,
                Location = new Point(16, 14),
                Size = new Size(42, 42),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            topBar.Controls.Add(lblLogo);

            Label lblTitle = new Label
            {
                Text = catTitle.ToUpper(),
                Font = new Font("Segoe UI", 12.5f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(64, 12),
                AutoSize = true,
                UseMnemonic = false,
                BackColor = Color.Transparent
            };
            topBar.Controls.Add(lblTitle);

            Label lblSub = new Label
            {
                Text = "Select an operational authority to issue form, verify parameters, or view historical audit records",
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(190, 215, 250),
                Location = new Point(66, 38),
                AutoSize = true,
                UseMnemonic = false,
                BackColor = Color.Transparent
            };
            topBar.Controls.Add(lblSub);

            // Active User Profile Badge
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
                ForeColor = Color.FromArgb(254, 240, 138),
                Location = new Point(38, 10),
                AutoSize = true,
                UseMnemonic = false,
                BackColor = Color.Transparent
            };
            userChip.Controls.Add(lblUserName);
            topBar.Controls.Add(userChip);

            Button btnProfile = new Button
            {
                Text = SessionManager.CurrentRole == UserRole.Admin ? "🛡️ Admin Panel" : "🏠 My Profile",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(251, 121, 43), // IRCTC Orange
                FlatStyle = FlatStyle.Flat,
                Size = new Size(130, 36),
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

            Button btnBack = new Button
            {
                Text = "← BACK TO HUB",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(51, 65, 85),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(135, 36),
                Cursor = Cursors.Hand
            };
            btnBack.FlatAppearance.BorderSize = 0;
            btnBack.Click += (s, e) => this.Close();
            topBar.Controls.Add(btnBack);

            Action layoutHeader = () =>
            {
                int r = topBar.ClientSize.Width - 18;
                btnBack.Location = new Point(r - 135, 16);
                r -= 145;

                btnProfile.Location = new Point(r - 130, 16);
                r -= 145;

                int chipWidth = TextRenderer.MeasureText(lblUserName.Text, lblUserName.Font).Width + 55;
                userChip.Size = new Size(chipWidth, 40);
                userChip.Location = new Point(r - chipWidth, 14);
            };
            topBar.Resize += (s, e) => layoutHeader();
            this.Shown += (s, e) => layoutHeader();

            // ── 2. Breadcrumb & Search Action Bar ──────────────────────────────
            Panel searchBarPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 56,
                BackColor = Color.White
            };
            searchBarPanel.Paint += (s, e) =>
            {
                using (var pen = new Pen(Color.FromArgb(226, 232, 240), 1))
                    e.Graphics.DrawLine(pen, 0, searchBarPanel.Height - 1, searchBarPanel.Width, searchBarPanel.Height - 1);
            };
            this.Controls.Add(searchBarPanel);
            searchBarPanel.BringToFront();

            Label lblBreadcrumb = new Label
            {
                Text = $"🏠 Authorities Hub  ➔  {catTitle}",
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                ForeColor = ThemeManager.IRCTCColors.PrimaryNavy,
                Location = new Point(25, 16),
                AutoSize = true,
                UseMnemonic = false
            };
            searchBarPanel.Controls.Add(lblBreadcrumb);

            Label lblSearch = new Label
            {
                Text = "🔍 Quick Filter:",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                Location = new Point(580, 17),
                AutoSize = true
            };
            searchBarPanel.Controls.Add(lblSearch);

            txtSearch = new TextBox
            {
                Location = new Point(690, 13),
                Size = new Size(300, 30),
                Font = new Font("Segoe UI", 10.5f),
                BorderStyle = BorderStyle.FixedSingle
            };
            txtSearch.TextChanged += (s, e) => PopulateRegisters(registerCardsContainer, txtSearch.Text.Trim());
            searchBarPanel.Controls.Add(txtSearch);

            // ── 3. Registers Container ─────────────────────────────────────────
            registerCardsContainer = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(241, 245, 249),
                Padding = new Padding(25, 20, 25, 20)
            };
            this.Controls.Add(registerCardsContainer);
            registerCardsContainer.BringToFront();

            PopulateRegisters(registerCardsContainer, "");
        }

        private string GetInitials(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName)) return "U";
            var parts = fullName.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1) return parts[0].Substring(0, Math.Min(2, parts[0].Length)).ToUpper();
            return (parts[0][0].ToString() + parts[parts.Length - 1][0].ToString()).ToUpper();
        }

        private string GetCategoryTitle(int cat)
        {
            switch (cat)
            {
                case 1: return "1. Signal & Defective Authorities";
                case 2: return "2. Line Clear & Block Working";
                case 3: return "3. Maintenance, S&T & Trolley";
                case 4: return "4. Emergency, Relief & Movements";
                default: return "Operational Registers";
            }
        }

        private void PopulateRegisters(Panel container, string filter)
        {
            container.SuspendLayout();
            container.Controls.Clear();

            var items = GetRegisterList(categoryId);
            int x = 25, y = 20;
            int cardW = 540, cardH = 145;

            for (int i = 0; i < items.Length; i++)
            {
                var item = items[i];

                if (!string.IsNullOrEmpty(filter))
                {
                    if (item.Name.IndexOf(filter, StringComparison.OrdinalIgnoreCase) < 0 &&
                        item.Code.IndexOf(filter, StringComparison.OrdinalIgnoreCase) < 0 &&
                        item.Description.IndexOf(filter, StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        continue;
                    }
                }

                Panel card = new Panel
                {
                    Size = new Size(cardW, cardH),
                    Location = new Point(x, y),
                    BackColor = Color.White
                };
                card.Paint += (s, e) =>
                {
                    using (var pen = new Pen(Color.FromArgb(226, 232, 240), 1))
                        e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
                    using (var brush = new SolidBrush(ThemeManager.IRCTCColors.PrimaryNavy))
                        e.Graphics.FillRectangle(brush, 0, 0, 4, card.Height);
                };

                // Code Badge
                Label lblCode = new Label
                {
                    Text = item.Code,
                    Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                    ForeColor = Color.FromArgb(30, 58, 138),
                    BackColor = Color.FromArgb(239, 246, 255),
                    Padding = new Padding(6, 2, 6, 2),
                    Location = new Point(14, 12),
                    AutoSize = true,
                    UseMnemonic = false
                };
                card.Controls.Add(lblCode);

                // Name
                Label lblName = new Label
                {
                    Text = item.Name,
                    Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                    ForeColor = Color.FromArgb(15, 23, 42),
                    Location = new Point(14, 38),
                    AutoSize = true,
                    UseMnemonic = false
                };
                card.Controls.Add(lblName);

                // Description
                Label lblDesc = new Label
                {
                    Text = item.Description,
                    Font = new Font("Segoe UI", 8.5f),
                    ForeColor = Color.FromArgb(100, 116, 139),
                    Location = new Point(15, 62),
                    Size = new Size(510, 32),
                    UseMnemonic = false
                };
                card.Controls.Add(lblDesc);

                // Action 1: Fill Form
                Button btnFill = new Button
                {
                    Text = "📝  ISSUE / FILL FORM",
                    Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                    ForeColor = Color.White,
                    BackColor = ThemeManager.IRCTCColors.ActionOrange,
                    FlatStyle = FlatStyle.Flat,
                    Size = new Size(185, 32),
                    Location = new Point(14, 100),
                    Cursor = Cursors.Hand
                };
                btnFill.FlatAppearance.BorderSize = 0;
                Action openAction = item.OpenAction;
                btnFill.Click += (s, e) => openAction();
                card.Controls.Add(btnFill);

                // Action 2: View Records
                Button btnView = new Button
                {
                    Text = "📋  VIEW RECORDS",
                    Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                    ForeColor = Color.White,
                    BackColor = ThemeManager.IRCTCColors.PrimaryNavy,
                    FlatStyle = FlatStyle.Flat,
                    Size = new Size(165, 32),
                    Location = new Point(210, 100),
                    Cursor = Cursors.Hand
                };
                btnView.FlatAppearance.BorderSize = 0;
                string tableName = item.DbTable;
                string formTitle = item.Name;
                btnView.Click += (s, e) => new ViewRecordsForm(tableName, formTitle).ShowDialog(this);
                card.Controls.Add(btnView);

                container.Controls.Add(card);

                if (x == 25)
                {
                    x = 590;
                }
                else
                {
                    x = 25;
                    y += 160;
                }
            }

            container.ResumeLayout(true);
        }

        private class RegisterItem
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string DbTable { get; set; }
            public Action OpenAction { get; set; }
        }

        private RegisterItem[] GetRegisterList(int cat)
        {
            switch (cat)
            {
                case 1:
                    return new RegisterItem[]
                    {
                        new RegisterItem { Code = "AUTH-001 • T/369-3b", Name = "Advance Authority Defective Signal", Description = "Authority to pass defective signals at ON aspect with fixed 15 kmph speed limit.", DbTable = "Advance_Authority_Defective_Signal", OpenAction = () => new Form_Auth001_AdvanceDefectiveSignal().ShowDialog(this) },
                        new RegisterItem { Code = "AUTH-002 • SIGNAL ON", Name = "Authority to Pass Signal at ON", Description = "Passing fixed signals at Danger with verified route clearance & block verification.", DbTable = "Authority_Pass_Signal_ON", OpenAction = () => new Form_Auth002_PassSignalON().ShowDialog(this) },
                        new RegisterItem { Code = "AUTH-003 • T/409", Name = "Caution Order Entry Record", Description = "Imposition and notification of temporary speed restrictions and track cautions.", DbTable = "Caution_Order_Entry", OpenAction = () => new Form_Auth003_CautionOrder().ShowDialog(this) },
                        new RegisterItem { Code = "AUTH-007 • T/512", Name = "Authority for Common Starter", Description = "Authority to start train from common starter signalled lines.", DbTable = "Authority_Common_Starter", OpenAction = () => new Form_Auth007_CommonStarter().ShowDialog(this) },
                        new RegisterItem { Code = "AUTH-013 • PASSING", Name = "Signal Passing Authority", Description = "Special signal passing record, authority validation, and territory authorization.", DbTable = "Signal_Passing_Authority", OpenAction = () => new Form_Auth013_SignalPassingAuthority().ShowDialog(this) },
                        new RegisterItem { Code = "AUTH-016 • T/D 912", Name = "ABS Prolonged Signal Failure", Description = "Authority during prolonged failure of all signals on Automatic Block section.", DbTable = "ABS_Prolonged_Signal_Failure", OpenAction = () => new Form_Auth016_ABSProlongedFailure().ShowDialog(this) }
                    };

                case 2:
                    return new RegisterItem[]
                    {
                        new RegisterItem { Code = "AUTH-004 • T/509", Name = "Authority to Receive on Obstructed Line", Description = "Authorizing train reception into obstructed platform/loop line with pilot hand signals.", DbTable = "Authority_Receive_Obstructed_Line", OpenAction = () => new Form_Auth004_ReceiveObstructedLine().ShowDialog(this) },
                        new RegisterItem { Code = "AUTH-005 • NON-SIG", Name = "Authority to Receive on Non-Signalled Line", Description = "Reception of trains on non-signalled lines with station pilot escort.", DbTable = "Authority_Receive_Non_Signalled", OpenAction = () => new Form_Auth005_ReceiveNonSignalled().ShowDialog(this) },
                        new RegisterItem { Code = "AUTH-006 • T/511", Name = "Authority to Start from Non-Signalled Line", Description = "Dispatching trains from non-signalled siding or station yard lines.", DbTable = "Authority_Start_Non_Signalled", OpenAction = () => new Form_Auth006_StartNonSignalled().ShowDialog(this) },
                        new RegisterItem { Code = "AUTH-010 • TFC-1/2", Name = "Line Clear Inquiry & Permission", Description = "Train inquiry and permission request via telecommunication between stations.", DbTable = "Line_Clear_Inquiry_TFC", OpenAction = () => new Form_Auth010_LineClearInquiry().ShowDialog(this) },
                        new RegisterItem { Code = "AUTH-011 • T/D 602", Name = "Temporary Single Line Working", Description = "Single line working on double line during total track obstruction.", DbTable = "Temporary_Single_Line_Working", OpenAction = () => new Form_Auth011_TemporarySingleLine().ShowDialog(this) },
                        new RegisterItem { Code = "AUTH-014 • T/C 912", Name = "ABS Proceed Without Line Clear", Description = "Authority for light engine/relief train into obstructed ABS block section.", DbTable = "ABS_Proceed_Without_Line_Clear", OpenAction = () => new Form_Auth014_ABSProceedWithoutLineClear().ShowDialog(this) },
                        new RegisterItem { Code = "AUTH-017 • LC-REPLY", Name = "Line Clear Inquiry & Reply Terminal", Description = "Line clear inquiry transmission, verification, and digital reply record.", DbTable = "Line_Clear_Inquiry_TFC", OpenAction = () => new Form_Auth017_LineClearInquiryReply().ShowDialog(this) },
                        new RegisterItem { Code = "AUTH-018 • T/A 1425", Name = "Paper Line Clear Tickets", Description = "Paper Line Clear Ticket record for UP/DN line train dispatches.", DbTable = "Line_Clear_Tickets", OpenAction = () => new Form_Auth018_LineClearTickets().ShowDialog(this) }
                    };

                case 3:
                    return new RegisterItem[]
                    {
                        new RegisterItem { Code = "AUTH-012 • T/806", Name = "Shunting Order Management", Description = "Yard and station shunting authority with engine and limitation instructions.", DbTable = "Shunting_Order_Management", OpenAction = () => new Form_Auth012_ShuntingOrder().ShowDialog(this) },
                        new RegisterItem { Code = "AUTH-019 • TROLLEY", Name = "Maintenance Trolley Notice", Description = "Notice of working for push trolleys and maintenance track units.", DbTable = "Maintenance_Trolley_Notice", OpenAction = () => new Form_Auth019_MaintenanceTrolleyNotice().ShowDialog(this) },
                        new RegisterItem { Code = "AUTH-020 • T/1518", Name = "Motor Trolley Permit", Description = "Official permit for motor trolley working in block sections.", DbTable = "Motor_Trolley_Permit", OpenAction = () => new Form_Auth020_MotorTrolleyPermit().ShowDialog(this) },
                        new RegisterItem { Code = "AUTH-021 • S&T T/351", Name = "S&T Disconnection / Reconnection", Description = "Signal, point, and track circuit maintenance notice and joint testing log.", DbTable = "ST_Disconnection_Notice", OpenAction = () => new Form_Auth021_STDisconnectionNotice().ShowDialog(this) }
                    };

                case 4:
                    return new RegisterItem[]
                    {
                        new RegisterItem { Code = "AUTH-008 • T/A 602", Name = "Relief Train Authorization", Description = "Authorization for relief engine/train to enter obstructed block section.", DbTable = "Relief_Train_Authorization", OpenAction = () => new Form_Auth008_ReliefTrain().ShowDialog(this) },
                        new RegisterItem { Code = "AUTH-009 • T/C 602", Name = "Communication Failure Working Log", Description = "Working of trains during total failure of all telecommunication means.", DbTable = "Communication_Failure_Log", OpenAction = () => new Form_Auth009_CommunicationFailure().ShowDialog(this) },
                        new RegisterItem { Code = "AUTH-015 • T/A 912", Name = "ABS Relief Engine / Train Authority", Description = "Relief engine movement authority in Automatic Block territory.", DbTable = "ABS_Relief_Engine_Authority", OpenAction = () => new Form_Auth015_ABSReliefEngine().ShowDialog(this) },
                        new RegisterItem { Code = "AUTH-022 • MOVEMENT", Name = "Train Movement & Halt Log", Description = "Station train movement log capturing scheduled vs actual timings and delays.", DbTable = "Train_Movement_Log", OpenAction = () => new Form_Auth022_TrainMovementLog().ShowDialog(this) }
                    };

                default:
                    return new RegisterItem[0];
            }
        }
    }
}
