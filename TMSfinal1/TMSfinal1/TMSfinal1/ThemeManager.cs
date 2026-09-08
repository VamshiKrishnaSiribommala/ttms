using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TMS
{
    /// <summary>
    /// Indian Railways, IRCTC & ConfirmTkt Inspired Professional Design System.
    /// Provides smooth anti-flicker rendering, top-right active user badges on EVERY form,
    /// and crisp translucent glassmorphism layouts across all 41 registers.
    /// </summary>
    public static class ThemeManager
    {
        [DllImport("dwmapi.dll")]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_COMPOSITED = 0x02000000;
        public const int WM_SETREDRAW = 11;

        public struct MARGINS
        {
            public int leftWidth;
            public int rightWidth;
            public int topHeight;
            public int bottomHeight;
        }

        // Official Indian Railways & Modern Travel App Color Tokens
        public static class IRCTCColors
        {
            public static readonly Color PrimaryNavy   = Color.FromArgb(33, 61, 119);   // #213D77 Indian Railways Blue
            public static readonly Color DarkNavy      = Color.FromArgb(14, 45, 95);    // #0E2D5F Deep Titlebar Navy
            public static readonly Color ActionOrange  = Color.FromArgb(251, 121, 43);  // #FB792B IRCTC Action Orange
            public static readonly Color OrangeHover   = Color.FromArgb(234, 88, 12);   // #EA580C
            public static readonly Color SuccessGreen  = Color.FromArgb(22, 163, 74);   // #16A34A Emerald
            public static readonly Color DangerRed     = Color.FromArgb(220, 38, 38);   // #DC2626 Crimson
            public static readonly Color SlateText     = Color.FromArgb(30, 41, 59);    // #1E293B Primary Text
            public static readonly Color SubtitleText  = Color.FromArgb(100, 116, 139); // #64748B Secondary Text
            public static readonly Color BorderGray    = Color.FromArgb(203, 213, 225); // #CBD5E1 Input Border
            public static readonly Color CardBg        = Color.White;
            public static readonly Color PageBg        = Color.FromArgb(244, 246, 249); // #F4F6F9 Background
            public static readonly Color AltRowBg      = Color.FromArgb(248, 250, 252);
            public static readonly Color ItemHoverBg   = Color.FromArgb(239, 246, 255); // #EFF6FF
        }

        public static void ApplyTheme(Form form)
        {
            if (form is Form_Reg041_DynamicReports || form is ViewRecordsForm || form is AdminDashboardForm || form is UserDashboardForm || form is LoginForm || form is MainClassesForm)
                return;

            form.FormBorderStyle = FormBorderStyle.None;
            form.SuspendLayout();

            try
            {
                EnableDoubleBuffering(form);
                form.MaximumSize = Screen.FromControl(form).WorkingArea.Size;
                form.WindowState = FormWindowState.Maximized;
                form.BackColor = IRCTCColors.PageBg;

                InjectCustomTitleBar(form);

                Panel cardPanel = form.Controls["MainCardContainer"] as Panel;
                if (cardPanel == null)
                {
                    cardPanel = new Panel();
                    cardPanel.Name = "MainCardContainer";
                    cardPanel.BackColor = IRCTCColors.CardBg;
                    
                    List<Control> toMove = new List<Control>();
                    foreach (Control c in form.Controls)
                    {
                        if (c.Name != "CustomWebTitleBar" && c != cardPanel)
                        {
                            toMove.Add(c);
                        }
                    }
                    
                    foreach (Control c in toMove)
                    {
                        form.Controls.Remove(c);
                        cardPanel.Controls.Add(c);
                    }
                    
                    form.Controls.Add(cardPanel);
                }

                int cardWidth = Math.Max(780, Math.Min(960, form.ClientSize.Width - 80));
                int cardHeight = Math.Max(640, form.ClientSize.Height - 50);
                
                cardPanel.Size = new Size(cardWidth, cardHeight);
                cardPanel.Location = new Point((form.ClientSize.Width - cardWidth) / 2, 40);
                cardPanel.Anchor = AnchorStyles.None;

                EnableDoubleBuffering(cardPanel);

                cardPanel.Paint -= CardPanel_Paint;
                cardPanel.Paint += CardPanel_Paint;

                form.Resize += (s, e) => {
                    if (form.WindowState == FormWindowState.Maximized)
                    {
                        form.MaximumSize = Screen.FromControl(form).WorkingArea.Size;
                    }
                    int cw = Math.Max(780, Math.Min(960, form.ClientSize.Width - 80));
                    int ch = Math.Max(640, form.ClientSize.Height - 50);
                    cardPanel.Size = new Size(cw, ch);
                    cardPanel.Location = new Point((form.ClientSize.Width - cw) / 2, 40);
                };

                OrganizeCardContents(form, cardPanel);

                Control tBar = form.Controls["CustomWebTitleBar"];
                if (tBar != null) tBar.BringToFront();
            }
            finally
            {
                form.ResumeLayout(true);
            }
        }

        private static void CardPanel_Paint(object sender, PaintEventArgs e)
        {
            Panel panel = sender as Panel;
            if (panel == null) return;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle rect = new Rectangle(0, 0, panel.Width - 1, panel.Height - 1);
            int radius = 10;
            
            using (GraphicsPath path = GetRoundedPath(rect, radius))
            {
                using (Pen borderPen = new Pen(Color.FromArgb(218, 225, 233), 1.5f))
                {
                    e.Graphics.DrawPath(borderPen, path);
                }
            }
        }

        private static GraphicsPath GetRoundedPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }

        private static void OrganizeCardContents(Form form, Panel cardPanel)
        {
            Panel headerPanel = cardPanel.Controls.OfType<Panel>().FirstOrDefault(p => p.Dock == DockStyle.Top || (p.Height <= 100 && p.Name != "CustomWebTitleBar" && p.Name != "BottomActionBar" && p.Name != "ContentScrollPanel"));
            if (headerPanel != null && headerPanel.Name != "CustomWebTitleBar")
            {
                headerPanel.Dock = DockStyle.Top;
                headerPanel.Height = 74;
                headerPanel.BackColor = IRCTCColors.PrimaryNavy;

                Label lblBreadcrumb = null;
                Label lblMainTitle = null;

                foreach (Control hc in headerPanel.Controls)
                {
                    if (hc is Label lbl)
                    {
                        lbl.BackColor = Color.Transparent;
                        if (lbl.Font.Size >= 13 || lbl.Text.ToUpper().Contains("REGISTER"))
                        {
                            lblMainTitle = lbl;
                        }
                        else
                        {
                            lblBreadcrumb = lbl;
                        }
                    }
                }

                if (lblBreadcrumb != null)
                {
                    lblBreadcrumb.Font = new Font("Segoe UI", 9.5F, FontStyle.Italic);
                    lblBreadcrumb.ForeColor = Color.FromArgb(200, 225, 255);
                    lblBreadcrumb.Location = new Point(25, 10);
                    lblBreadcrumb.Size = new Size(cardPanel.Width - 50, 20);
                    lblBreadcrumb.TextAlign = ContentAlignment.MiddleLeft;
                    lblBreadcrumb.BringToFront();
                }

                if (lblMainTitle != null)
                {
                    lblMainTitle.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
                    lblMainTitle.ForeColor = Color.White;
                    lblMainTitle.Location = new Point(25, 34);
                    lblMainTitle.Size = new Size(cardPanel.Width - 50, 32);
                    lblMainTitle.TextAlign = ContentAlignment.MiddleCenter;
                    lblMainTitle.BringToFront();
                }
            }

            if (form is RegisterSelectionForm)
            {
                ListBox lb = cardPanel.Controls.OfType<ListBox>().FirstOrDefault();
                if (lb != null)
                {
                    lb.Size = new Size(cardPanel.Width - 80, cardPanel.Height - 170);
                    lb.Location = new Point(40, 88);
                    lb.BackColor = Color.White;
                    lb.ForeColor = IRCTCColors.SlateText;
                    lb.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
                    lb.BorderStyle = BorderStyle.FixedSingle;
                    lb.ItemHeight = 46;
                    lb.DrawMode = DrawMode.OwnerDrawFixed;
                    lb.DrawItem -= ListBox_DrawItem;
                    lb.DrawItem += ListBox_DrawItem;
                }

                Label lblTip = cardPanel.Controls["lblRegSelectTip"] as Label;
                if (lblTip == null)
                {
                    lblTip = new Label
                    {
                        Name = "lblRegSelectTip",
                        Text = "💡 Tip: Double-click any register to open immediately  •  Or select and click Open Register",
                        Font = new Font("Segoe UI", 10.5F, FontStyle.Italic),
                        ForeColor = Color.FromArgb(71, 85, 105),
                        TextAlign = ContentAlignment.MiddleCenter,
                        BackColor = Color.Transparent,
                        Size = new Size(cardPanel.Width - 80, 24),
                        Location = new Point(40, cardPanel.Height - 88)
                    };
                    cardPanel.Controls.Add(lblTip);
                }
                else
                {
                    lblTip.Size = new Size(cardPanel.Width - 80, 24);
                    lblTip.Location = new Point(40, cardPanel.Height - 88);
                }

                List<Button> actionBtns = cardPanel.Controls.OfType<Button>().ToList();
                int bWidth = 210;
                int bHeight = 46;
                int bGap = 18;
                int totalW = actionBtns.Count * bWidth + (actionBtns.Count - 1) * bGap;
                int startX = (cardPanel.Width - totalW) / 2;
                int btnY = cardPanel.Height - 60;

                for (int i = 0; i < actionBtns.Count; i++)
                {
                    Button btn = actionBtns[i];
                    btn.Size = new Size(bWidth, bHeight);
                    btn.Location = new Point(startX + i * (bWidth + bGap), btnY);
                    StyleModernButton(btn);
                }
                return;
            }

            LayoutRegisterInputForm(cardPanel);
        }

        private static void ListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            ListBox lb = sender as ListBox;
            if (lb == null || e.Index < 0 || e.Index >= lb.Items.Count) return;

            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
            Color bg = isSelected ? IRCTCColors.PrimaryNavy : (e.Index % 2 == 0 ? Color.White : IRCTCColors.AltRowBg);
            Color fg = isSelected ? Color.White : IRCTCColors.SlateText;

            using (SolidBrush bgBrush = new SolidBrush(bg))
            {
                e.Graphics.FillRectangle(bgBrush, e.Bounds);
            }

            using (Pen linePen = new Pen(Color.FromArgb(235, 240, 245)))
            {
                e.Graphics.DrawLine(linePen, e.Bounds.Left, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);
            }

            string raw = lb.Items[e.Index].ToString();
            string badge = "";
            string regName = raw;
            if (raw.Contains(" - "))
            {
                int dashIdx = raw.IndexOf(" - ");
                badge = raw.Substring(0, dashIdx).Trim();
                regName = raw.Substring(dashIdx + 3).Trim();
            }

            int x = e.Bounds.Left + 16;
            if (!string.IsNullOrEmpty(badge))
            {
                Rectangle badgeRect = new Rectangle(x, e.Bounds.Top + (e.Bounds.Height - 24) / 2, 48, 24);
                Color badgeBg = isSelected ? Color.FromArgb(251, 121, 43) : Color.FromArgb(238, 242, 255);
                Color badgeFg = isSelected ? Color.White : Color.FromArgb(33, 61, 119);

                using (SolidBrush bb = new SolidBrush(badgeBg))
                {
                    e.Graphics.FillRectangle(bb, badgeRect);
                }
                using (Pen bp = new Pen(isSelected ? Color.FromArgb(234, 88, 12) : Color.FromArgb(199, 210, 254)))
                {
                    e.Graphics.DrawRectangle(bp, badgeRect);
                }

                using (Font bFont = new Font("Segoe UI", 9F, FontStyle.Bold))
                using (SolidBrush bfg = new SolidBrush(badgeFg))
                {
                    StringFormat bsf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    e.Graphics.DrawString(badge, bFont, bfg, badgeRect, bsf);
                }
                x += 58;
            }

            Rectangle textRect = new Rectangle(x, e.Bounds.Top, e.Bounds.Width - x - 40, e.Bounds.Height);
            using (SolidBrush fgBrush = new SolidBrush(fg))
            using (Font font = new Font("Segoe UI", 10.5F, isSelected ? FontStyle.Bold : FontStyle.Regular))
            {
                StringFormat sf = new StringFormat
                {
                    LineAlignment = StringAlignment.Center,
                    Alignment = StringAlignment.Near,
                    Trimming = StringTrimming.EllipsisCharacter
                };
                e.Graphics.DrawString(regName, font, fgBrush, textRect, sf);
            }

            string arrow = isSelected ? "➔" : "›";
            Color arrowCol = isSelected ? Color.FromArgb(255, 215, 0) : Color.FromArgb(148, 163, 184);
            Rectangle arrowRect = new Rectangle(e.Bounds.Right - 36, e.Bounds.Top, 26, e.Bounds.Height);
            using (Font arrowFont = new Font("Segoe UI", isSelected ? 11F : 13F, FontStyle.Bold))
            using (SolidBrush arrowBrush = new SolidBrush(arrowCol))
            {
                StringFormat asf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                e.Graphics.DrawString(arrow, arrowFont, arrowBrush, arrowRect, asf);
            }

            if (isSelected)
            {
                e.DrawFocusRectangle();
            }
        }

        private static void LayoutRegisterInputForm(Panel cardPanel)
        {
            // 1. Separate action buttons from input controls
            List<Button> actionButtons = new List<Button>();
            List<Control> formFields = new List<Control>();

            foreach (Control c in cardPanel.Controls.Cast<Control>().ToList())
            {
                if (c is Panel && (c.Dock == DockStyle.Top || c.Name == "BottomActionBar" || c.Name == "ContentScrollPanel"))
                    continue;

                if (c is Button btn)
                {
                    actionButtons.Add(btn);
                }
                else
                {
                    formFields.Add(c);
                }
            }

            // 2. Create or find Bottom Action Bar (Dedicated container for buttons)
            Panel bottomBar = cardPanel.Controls["BottomActionBar"] as Panel;
            if (bottomBar == null)
            {
                bottomBar = new Panel
                {
                    Name = "BottomActionBar",
                    Dock = DockStyle.Bottom,
                    Height = 64,
                    BackColor = Color.White
                };
                bottomBar.Paint += (s, e) =>
                {
                    using (Pen p = new Pen(Color.FromArgb(226, 232, 240), 1.5f))
                    {
                        e.Graphics.DrawLine(p, 0, 0, bottomBar.Width, 0);
                    }
                };
                cardPanel.Controls.Add(bottomBar);
            }

            // 3. Automatically ensure SEND button exists for all register forms
            bool hasSave = actionButtons.Any(b => b.Text.ToUpper().Contains("SAVE") || b.Text.ToUpper().Contains("SUBMIT"));
            bool hasSend = actionButtons.Any(b => b.Text.ToUpper().Contains("SEND"));
            if (hasSave && !hasSend)
            {
                Button btnSend = new Button
                {
                    Name = "btnDynamicSend",
                    Text = "SEND",
                    Cursor = Cursors.Hand
                };
                btnSend.Click += (s, e) =>
                {
                    Form f = (s as Control)?.FindForm();
                    if (f != null)
                    {
                        RecordDispatchService.HandleSendButtonClick(f);
                    }
                };
                actionButtons.Add(btnSend);
            }

            // 4. Move and style action buttons in the bottom bar
            if (actionButtons.Count > 0)
            {
                Func<Button, int> getBtnPriority = b =>
                {
                    string txt = b.Text.ToUpper();
                    if (txt.Contains("SAVE") || txt.Contains("SUBMIT")) return 1;
                    if (txt.Contains("SEND")) return 2;
                    if (txt.Contains("VIEW")) return 3;
                    if (txt.Contains("CLEAR") || txt.Contains("RESET")) return 4;
                    if (txt.Contains("BACK") || txt.Contains("CLOSE")) return 5;
                    return 6;
                };

                actionButtons = actionButtons.OrderBy(getBtnPriority).ThenBy(b => b.Left).ToList();
                int btnW = actionButtons.Count >= 5 ? 120 : 145;
                int btnH = 42;
                int btnGap = 12;
                int totalBtnW = actionButtons.Count * btnW + (actionButtons.Count - 1) * btnGap;
                int startBtnX = Math.Max(20, (cardPanel.Width - totalBtnW) / 2);
                int btnY = (bottomBar.Height - btnH) / 2;

                for (int i = 0; i < actionButtons.Count; i++)
                {
                    Button btn = actionButtons[i];
                    if (btn.Parent != bottomBar)
                    {
                        btn.Parent?.Controls.Remove(btn);
                        bottomBar.Controls.Add(btn);
                    }
                    btn.Size = new Size(btnW, btnH);
                    btn.Location = new Point(startBtnX + i * (btnW + btnGap), btnY);
                    StyleModernButton(btn);
                }
            }

            // 4. Create or find Scrollable Content Panel for all inputs
            Panel scrollPanel = cardPanel.Controls["ContentScrollPanel"] as Panel;
            if (scrollPanel == null)
            {
                scrollPanel = new Panel
                {
                    Name = "ContentScrollPanel",
                    Dock = DockStyle.Fill,
                    BackColor = Color.White,
                    AutoScroll = true,
                    Padding = new Padding(25, 20, 25, 20)
                };
                cardPanel.Controls.Add(scrollPanel);
            }

            scrollPanel.BringToFront();

            // 5. Structure Form Fields into Professional 2-Column Grid Layout (Matching Image 3)
            if (formFields.Count > 0)
            {
                foreach (Control c in formFields)
                {
                    if (c.Parent != scrollPanel)
                    {
                        c.Parent?.Controls.Remove(c);
                        scrollPanel.Controls.Add(c);
                    }
                }

                int gridWidth = Math.Min(860, cardPanel.ClientSize.Width - 60);
                int startX = Math.Max(25, (cardPanel.ClientSize.Width - gridWidth) / 2);

                int lblW = 160;
                int colGap = 30;
                int inpW = (gridWidth - (2 * lblW + 20 + colGap)) / 2;
                if (inpW < 220) inpW = 220;

                int col1LblX = startX;
                int col1InpX = col1LblX + lblW + 10;
                int col2LblX = col1InpX + inpW + colGap;
                int col2InpX = col2LblX + lblW + 10;
                int fullInpW = (col2InpX + inpW) - col1InpX;

                var sortedLabels = formFields.OfType<Label>().OrderBy(l => l.Top).ThenBy(l => l.Left).ToList();
                var availableInputs = formFields.Where(c => !(c is Label)).OrderBy(c => c.Top).ThenBy(c => c.Left).ToList();

                int currentY = 22;
                int col = 0;

                for (int i = 0; i < sortedLabels.Count; i++)
                {
                    Label lbl = sortedLabels[i];

                    // Find companion input
                    Control input = availableInputs.FirstOrDefault(inp => Math.Abs(inp.Top - lbl.Top) < 35 && inp.Left >= lbl.Left);
                    if (input == null)
                    {
                        input = availableInputs.FirstOrDefault(inp => Math.Abs(inp.Top - lbl.Top) < 60);
                    }
                    if (input == null && availableInputs.Count > 0)
                    {
                        input = availableInputs[0];
                    }

                    bool isRichText = (input is RichTextBox);
                    bool isWideField = isRichText || (input != null && (input.Width > 380 || (input is TextBox tb && tb.Multiline)));

                    if (lbl != null && !isWideField)
                    {
                        string lText = lbl.Text.ToLower();
                        if (lText.Contains("description") || lText.Contains("remark") || lText.Contains("observation") ||
                            lText.Contains("action suggested") || lText.Contains("action taken") || lText.Contains("corrective action") ||
                            lText.Contains("details") || lText.Contains("reason") || lText.Contains("grievance") ||
                            lText.Contains("complaint") || lText.Contains("subject") || lText.Contains("findings") ||
                            lText.Contains("brief") || lText.Contains("address") || lText.Contains("instructions") ||
                            lText.Contains("cause of") || lText.Contains("submitted by"))
                        {
                            isWideField = true;
                        }
                    }

                    if (isWideField)
                    {
                        if (col == 1)
                        {
                            currentY += 46;
                            col = 0;
                        }

                        lbl.Location = new Point(col1LblX, currentY + 6);
                        lbl.Size = new Size(lblW, 24);
                        lbl.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                        lbl.ForeColor = IRCTCColors.SlateText;
                        lbl.TextAlign = ContentAlignment.MiddleLeft;

                        if (input != null)
                        {
                            availableInputs.Remove(input);
                            int inputHeight = isRichText ? 76 : 32;
                            input.Location = new Point(col1InpX, currentY);
                            input.Size = new Size(fullInpW, inputHeight);
                            ApplyStyleToControl(input);
                            currentY += (inputHeight + 14);
                        }
                        else
                        {
                            currentY += 46;
                        }
                        col = 0;
                    }
                    else
                    {
                        int curLblX = (col == 0) ? col1LblX : col2LblX;
                        int curInpX = (col == 0) ? col1InpX : col2InpX;

                        lbl.Location = new Point(curLblX, currentY + 6);
                        lbl.Size = new Size(lblW, 24);
                        lbl.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                        lbl.ForeColor = IRCTCColors.SlateText;
                        lbl.TextAlign = ContentAlignment.MiddleLeft;

                        if (input != null)
                        {
                            availableInputs.Remove(input);
                            if (input is CheckBox chk)
                            {
                                chk.Location = new Point(curInpX, currentY + 6);
                                chk.AutoSize = true;
                                chk.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                                chk.ForeColor = IRCTCColors.SlateText;
                            }
                            else
                            {
                                input.Location = new Point(curInpX, currentY);
                                input.Size = new Size(inpW, 32);
                                ApplyStyleToControl(input);
                            }
                        }

                        if (col == 0)
                        {
                            col = 1;
                        }
                        else
                        {
                            col = 0;
                            currentY += 46;
                        }
                    }
                }

                if (col == 1)
                {
                    currentY += 46;
                }

                // Place any remaining inputs
                foreach (Control rem in availableInputs)
                {
                    rem.Location = new Point(col1InpX, currentY);
                    rem.Size = new Size(inpW, 32);
                    ApplyStyleToControl(rem);
                    currentY += 46;
                }
            }
        }

        public static void StyleDataGridView(DataGridView dgv)
        {
            dgv.BackgroundColor = Color.White;
            dgv.ForeColor = IRCTCColors.SlateText;
            dgv.GridColor = Color.FromArgb(226, 232, 240);
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.DefaultCellStyle.ForeColor = IRCTCColors.SlateText;
            dgv.DefaultCellStyle.SelectionBackColor = IRCTCColors.PrimaryNavy;
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            dgv.DefaultCellStyle.Padding = new Padding(4);

            dgv.AlternatingRowsDefaultCellStyle.BackColor = IRCTCColors.AltRowBg;
            dgv.AlternatingRowsDefaultCellStyle.ForeColor = IRCTCColors.SlateText;
            dgv.AlternatingRowsDefaultCellStyle.SelectionBackColor = IRCTCColors.PrimaryNavy;
            dgv.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.White;

            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersHeight = 42;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = IRCTCColors.PrimaryNavy;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgv.RowTemplate.Height = 36;
        }

        private static void ApplyStyleToControl(Control control)
        {
            if (!string.IsNullOrEmpty(control.Text))
            {
                while (control.Text.StartsWith("?") || control.Text.StartsWith(" "))
                {
                    control.Text = control.Text.TrimStart('?', ' ');
                }
            }

            EnableDoubleBuffering(control);

            if (control is Label label)
            {
                label.BackColor = Color.Transparent;
                label.ForeColor = IRCTCColors.SlateText;
                label.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                label.AutoSize = true;
            }
            else if (control is Button button)
            {
                StyleModernButton(button);
            }
            else if (control is TextBox textBox)
            {
                textBox.BackColor = Color.White;
                textBox.ForeColor = IRCTCColors.SlateText;
                textBox.BorderStyle = BorderStyle.FixedSingle;
                textBox.Font = new Font("Segoe UI", 10.5F);
            }
            else if (control is RichTextBox richTextBox)
            {
                richTextBox.BackColor = Color.White;
                richTextBox.ForeColor = IRCTCColors.SlateText;
                richTextBox.BorderStyle = BorderStyle.FixedSingle;
                richTextBox.Font = new Font("Segoe UI", 10.5F);
            }
            else if (control is ComboBox comboBox)
            {
                comboBox.BackColor = Color.White;
                comboBox.ForeColor = IRCTCColors.SlateText;
                comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                comboBox.Font = new Font("Segoe UI", 10.5F);
            }
            else if (control is DateTimePicker dtp)
            {
                dtp.Font = new Font("Segoe UI", 10F);
                dtp.Format = DateTimePickerFormat.Custom;
                dtp.CustomFormat = "dd-MMM-yyyy  HH:mm:ss";
                dtp.Width = Math.Max(dtp.Width, 215);
                try
                {
                    if (dtp.MaxDate < DateTime.MaxValue.AddDays(-10))
                    {
                        dtp.MaxDate = DateTime.MaxValue.AddDays(-5);
                    }
                    if (dtp.MinDate > DateTime.MinValue.AddDays(10))
                    {
                        dtp.MinDate = new DateTime(2000, 1, 1);
                    }
                }
                catch { }
            }
            else if (control is NumericUpDown num)
            {
                num.BackColor = Color.White;
                num.ForeColor = IRCTCColors.SlateText;
                num.BorderStyle = BorderStyle.FixedSingle;
                num.Font = new Font("Segoe UI", 10.5F);
            }
            else if (control is CheckBox chk)
            {
                chk.BackColor = Color.Transparent;
                chk.ForeColor = IRCTCColors.SlateText;
                chk.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
                chk.AutoSize = true;
            }
        }

        private static void EnableDoubleBuffering(Control control)
        {
            try
            {
                typeof(Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(control, true, null);
            }
            catch { }
        }

        public static void StyleModernButton(Button button)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Cursor = Cursors.Hand;
            button.Font = new Font("Segoe UI", 11.5F, FontStyle.Bold);

            string btnText = button.Text.ToUpper();
            Color baseColor;
            Color hoverColor;

            if (btnText.Contains("SAVE") || btnText.Contains("SUBMIT") || btnText.Contains("OPEN") || btnText.Contains("CREATE"))
            {
                baseColor = IRCTCColors.ActionOrange;
                hoverColor = Color.FromArgb(234, 88, 12);
            }
            else if (btnText.Contains("SEND") || btnText.Contains("SHARE") || btnText.Contains("DISPATCH"))
            {
                baseColor = Color.FromArgb(16, 185, 129); // #10B981 Emerald
                hoverColor = Color.FromArgb(5, 150, 105); // #059669
            }
            else if (btnText.Contains("VIEW") || btnText.Contains("SEARCH") || btnText.Contains("REFRESH") || btnText.Contains("RECORDS"))
            {
                baseColor = IRCTCColors.PrimaryNavy;
                hoverColor = Color.FromArgb(43, 80, 155);
            }
            else if (btnText.Contains("BACK") || btnText.Contains("CANCEL") || btnText.Contains("CLOSE") || btnText.Contains("LOGOUT"))
            {
                baseColor = IRCTCColors.DangerRed;
                hoverColor = Color.FromArgb(239, 68, 68);

                // Ensure single-click modal dialog close
                button.DialogResult = DialogResult.Cancel;
                button.Click -= BtnBack_DirectClose;
                button.Click += BtnBack_DirectClose;
            }
            else if (btnText.Contains("CLEAR") || btnText.Contains("RESET"))
            {
                baseColor = Color.FromArgb(100, 116, 139);
                hoverColor = Color.FromArgb(71, 85, 105);
            }
            else
            {
                baseColor = IRCTCColors.PrimaryNavy;
                hoverColor = Color.FromArgb(43, 80, 155);
            }

            button.BackColor = baseColor;
            button.ForeColor = Color.White;

            button.MouseEnter += (s, e) => button.BackColor = hoverColor;
            button.MouseLeave += (s, e) => button.BackColor = baseColor;
        }

        private static void BtnBack_DirectClose(object sender, EventArgs e)
        {
            Control c = sender as Control;
            Form child = c?.FindForm();
            if (child == null || child.IsDisposed) return;

            // Find the parent RegisterSelectionForm that owns this register child
            // Walk OpenForms to find a RegisterSelectionForm that is hidden (parent was hidden when this opened)
            Form parentToRestore = null;
            foreach (Form f in Application.OpenForms)
            {
                if (f is RegisterSelectionForm && !f.IsDisposed)
                {
                    parentToRestore = f;
                    break;
                }
                if (f is MainClassesForm && !f.IsDisposed)
                {
                    parentToRestore = f;
                    // Don't break - prefer RegisterSelectionForm if both exist
                }
            }

            // Show parent BEFORE closing child to avoid black flash
            if (parentToRestore != null)
            {
                parentToRestore.SuspendLayout();
                parentToRestore.Show();
                parentToRestore.BringToFront();
                parentToRestore.ResumeLayout(false);
            }

            // Defer close to next message pump cycle so parent paints first
            child.BeginInvoke(new Action(() =>
            {
                child.DialogResult = DialogResult.Cancel;
                child.Close();
            }));
        }

        private static void InjectCustomTitleBar(Form form)
        {
            Control existing = form.Controls["CustomWebTitleBar"];
            if (existing != null) form.Controls.Remove(existing);

            Panel titleBar = new Panel();
            titleBar.Name = "CustomWebTitleBar";
            titleBar.Dock = DockStyle.Top;
            titleBar.Height = 38;
            titleBar.BackColor = IRCTCColors.DarkNavy;

            // Form Title (Left)
            Label titleLabel = new Label();
            titleLabel.Text = "🚆  " + form.Text;
            titleLabel.ForeColor = Color.White;
            titleLabel.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            titleLabel.Location = new Point(14, 9);
            titleLabel.AutoSize = true;
            titleBar.Controls.Add(titleLabel);

            // Close Button (Right)
            Button closeBtn = new Button();
            closeBtn.Text = "✕";
            closeBtn.Size = new Size(42, 38);
            closeBtn.Dock = DockStyle.Right;
            closeBtn.FlatStyle = FlatStyle.Flat;
            closeBtn.FlatAppearance.BorderSize = 0;
            closeBtn.ForeColor = Color.White;
            closeBtn.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            closeBtn.Cursor = Cursors.Hand;
            closeBtn.Click += (s, e) => form.Close();
            closeBtn.MouseEnter += (s, e) => closeBtn.BackColor = IRCTCColors.DangerRed;
            closeBtn.MouseLeave += (s, e) => closeBtn.BackColor = Color.Transparent;
            titleBar.Controls.Add(closeBtn);

            // Active User Session Badge on the Top Right of EVERY form
            string role = SessionManager.CurrentRole == UserRole.Admin ? "Admin" : SessionManager.CurrentDepartment;
            if (role != null && role.Length > 25) role = role.Substring(0, 22) + "...";
            string userDisplayText = $"👤 {SessionManager.CurrentFullName} ({role})";

            Label userBadge = new Label
            {
                Text = userDisplayText,
                ForeColor = Color.FromArgb(255, 220, 80), // Gold
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            titleBar.Controls.Add(userBadge);

            Action layoutTitleBar = () =>
            {
                int r = titleBar.ClientSize.Width - closeBtn.Width - 15;
                userBadge.Location = new Point(r - userBadge.Width, 9);
            };

            titleBar.Resize += (s, e) => layoutTitleBar();
            form.Shown += (s, e) => layoutTitleBar();

            titleBar.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    ReleaseCapture();
                    SendMessage(form.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
                }
            };

            form.Controls.Add(titleBar);
            titleBar.BringToFront();
        }
    }
}
