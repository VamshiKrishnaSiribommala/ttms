using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace TrainWorkingApp
{
    /// <summary>
    /// Indian Railways & IRCTC Inspired Professional Design System.
    /// Provides smooth anti-flicker rendering, top-right active operator badges on EVERY form,
    /// 1-click official dispatch action bar, and crisp layout styling across all authorities.
    /// </summary>
    public static class ThemeManager
    {
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        public static class IRCTCColors
        {
            public static readonly Color PrimaryNavy   = Color.FromArgb(33, 61, 119);   // #213D77 Indian Railways Blue
            public static readonly Color DarkNavy      = Color.FromArgb(14, 45, 95);    // #0E2D5F Deep Titlebar Navy
            public static readonly Color ActionOrange  = Color.FromArgb(251, 121, 43);  // #FB792B IRCTC Action Orange
            public static readonly Color OrangeHover   = Color.FromArgb(234, 88, 12);   // #EA580C
            public static readonly Color SuccessGreen  = Color.FromArgb(22, 163, 74);   // #16A34A Emerald
            public static readonly Color GreenHover    = Color.FromArgb(21, 128, 61);   // #15803D
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
            if (form is DynamicReportsForm || form is ViewRecordsForm || form is AdminDashboardForm || form is UserDashboardForm || form is LoginForm || form is AuthorityHubForm)
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

                int cardWidth = Math.Max(840, Math.Min(1020, form.ClientSize.Width - 60));
                int cardHeight = Math.Max(580, form.ClientSize.Height - 78 - 20);
                
                cardPanel.Size = new Size(cardWidth, cardHeight);
                cardPanel.Location = new Point((form.ClientSize.Width - cardWidth) / 2, 78);
                cardPanel.Anchor = AnchorStyles.None;
                EnableDoubleBuffering(cardPanel);

                cardPanel.Paint -= CardPanel_Paint;
                cardPanel.Paint += CardPanel_Paint;

                form.Resize -= Form_Resize;
                form.Resize += Form_Resize;

                OrganizeCardContents(form, cardPanel);

                Control tBar = form.Controls["CustomWebTitleBar"];
                if (tBar != null) tBar.BringToFront();
            }
            finally
            {
                form.ResumeLayout(true);
            }
        }

        private static void Form_Resize(object sender, EventArgs e)
        {
            Form form = sender as Form;
            if (form == null || form.Disposing || form.IsDisposed) return;
            if (form.WindowState == FormWindowState.Minimized) return;

            if (form.WindowState == FormWindowState.Maximized)
            {
                form.MaximumSize = Screen.FromControl(form).WorkingArea.Size;
            }

            Panel cardPanel = form.Controls["MainCardContainer"] as Panel;
            if (cardPanel != null)
            {
                OrganizeCardContents(form, cardPanel);
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
            int cardWidth = Math.Max(1020, Math.Min(1220, form.ClientSize.Width - 60));

            Panel headerPanel = cardPanel.Controls.OfType<Panel>().FirstOrDefault(p => p.Dock == DockStyle.Top || (p.Height <= 100 && p.Name != "CustomWebTitleBar" && p.Name != "BottomActionBar" && p.Name != "ContentScrollPanel"));
            if (headerPanel != null && headerPanel.Name != "CustomWebTitleBar")
            {
                headerPanel.Visible = true;
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
                        if (lbl.Font.Size >= 12 && !lbl.Text.Contains(">") && !lbl.Text.Contains("•") && !lbl.Text.StartsWith("Train Working") && !lbl.Text.StartsWith("Home"))
                        {
                            lblMainTitle = lbl;
                        }
                        else
                        {
                            lblBreadcrumb = lbl;
                        }
                    }
                }

                if (lblBreadcrumb != null && lblMainTitle == null)
                {
                    lblMainTitle = lblBreadcrumb;
                    lblBreadcrumb = null;
                }

                if (lblBreadcrumb != null)
                {
                    lblBreadcrumb.Font = new Font("Segoe UI", 9.5F, FontStyle.Italic);
                    lblBreadcrumb.ForeColor = Color.FromArgb(200, 225, 255);
                    lblBreadcrumb.Location = new Point(25, 10);
                    lblBreadcrumb.Size = new Size(cardWidth - 50, 22);
                    lblBreadcrumb.TextAlign = ContentAlignment.MiddleLeft;
                    lblBreadcrumb.BringToFront();
                }

                if (lblMainTitle != null)
                {
                    lblMainTitle.Font = new Font("Segoe UI", 14.5F, FontStyle.Bold);
                    lblMainTitle.ForeColor = Color.White;
                    lblMainTitle.Location = new Point(25, lblBreadcrumb != null ? 34 : 20);
                    lblMainTitle.Size = new Size(cardWidth - 50, 32);
                    lblMainTitle.TextAlign = ContentAlignment.MiddleCenter;
                    lblMainTitle.BringToFront();
                }
            }

            if (form is AuthoritySelectionForm)
            {
                ListBox lb = cardPanel.Controls.OfType<ListBox>().FirstOrDefault();
                if (lb != null)
                {
                    lb.Size = new Size(cardPanel.Width - 80, cardPanel.Height - 170);
                    lb.Location = new Point(40, 88);
                    lb.BackColor = Color.White;
                    lb.ForeColor = IRCTCColors.SlateText;
                    lb.Font = new Font("Segoe UI", 11.5F, FontStyle.Bold);
                    lb.BorderStyle = BorderStyle.FixedSingle;
                    lb.ItemHeight = 44;
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
                        Text = "💡 Tip: Double-click any authority register to open immediately  •  Or select and click Open Register",
                        Font = new Font("Segoe UI", 10F, FontStyle.Italic),
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

            LayoutRegisterInputForm(form, cardPanel);
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
            if (raw.Contains(" - ") || raw.Contains(": "))
            {
                int sepIdx = raw.IndexOf(" - ");
                if (sepIdx < 0) sepIdx = raw.IndexOf(": ");
                if (sepIdx > 0)
                {
                    badge = raw.Substring(0, sepIdx).Trim();
                    regName = raw.Substring(sepIdx + (raw.Contains(" - ") ? 3 : 2)).Trim();
                }
            }

            int x = e.Bounds.Left + 16;
            if (!string.IsNullOrEmpty(badge))
            {
                Rectangle badgeRect = new Rectangle(x, e.Bounds.Top + (e.Bounds.Height - 24) / 2, 72, 24);
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

                using (Font bFont = new Font("Segoe UI", 8.5F, FontStyle.Bold))
                using (SolidBrush bfg = new SolidBrush(badgeFg))
                {
                    StringFormat bsf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                    e.Graphics.DrawString(badge, bFont, bfg, badgeRect, bsf);
                }
                x += 80;
            }

            Rectangle textRect = new Rectangle(x, e.Bounds.Top, e.Bounds.Width - x - 40, e.Bounds.Height);
            using (SolidBrush fgBrush = new SolidBrush(fg))
            using (Font font = new Font("Segoe UI", 10F, isSelected ? FontStyle.Bold : FontStyle.Regular))
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

        private static void LayoutRegisterInputForm(Form form, Panel cardPanel)
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

            Panel bottomBar = cardPanel.Controls["BottomActionBar"] as Panel;
            if (bottomBar != null)
            {
                foreach (Control c in bottomBar.Controls.Cast<Control>().ToList())
                {
                    if (c is Button btn && !actionButtons.Contains(btn))
                    {
                        actionButtons.Add(btn);
                    }
                }
            }

            Panel scrollPanel = cardPanel.Controls["ContentScrollPanel"] as Panel;
            if (scrollPanel != null)
            {
                scrollPanel.AutoScrollPosition = new Point(0, 0);
                foreach (Control c in scrollPanel.Controls.Cast<Control>().ToList())
                {
                    if (c is Button btn)
                    {
                        if (!actionButtons.Contains(btn)) actionButtons.Add(btn);
                    }
                    else
                    {
                        formFields.Add(c);
                    }
                }
            }

            // 2. Create or find Bottom Action Bar (Dedicated container for buttons)
            if (bottomBar == null)
            {
                bottomBar = new Panel
                {
                    Name = "BottomActionBar",
                    Dock = DockStyle.Bottom,
                    Height = 72,
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

            // 3. Automatically ensure SEND button exists for all authority forms
            bool hasSave = actionButtons.Any(b => b.Text.ToUpper().Contains("SAVE") || b.Text.ToUpper().Contains("SUBMIT"));
            bool hasSend = actionButtons.Any(b => b.Text.ToUpper().Contains("SEND") || b.Text.ToUpper().Contains("DISPATCH"));
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

            // 4. Create or find Scrollable Content Panel for all inputs
            if (scrollPanel == null)
            {
                scrollPanel = new Panel
                {
                    Name = "ContentScrollPanel",
                    Dock = DockStyle.Fill,
                    BackColor = Color.White,
                    AutoScroll = true,
                    Padding = new Padding(24, 20, 24, 20)
                };
                cardPanel.Controls.Add(scrollPanel);
            }

            scrollPanel.Paint -= ScrollPanel_PaintBorders;
            scrollPanel.Paint += ScrollPanel_PaintBorders;
            scrollPanel.BringToFront();

            // 5. Structure Form Fields into Clean, Perfectly Aligned 2-Column Grid Layout
            int currentY = 25;
            int cardWidth = Math.Max(1020, Math.Min(1220, form.ClientSize.Width - 60));

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

                int gridWidth = Math.Min(1120, cardWidth - 60);
                int startX = Math.Max(25, (cardWidth - gridWidth) / 2);

                int lblW = 265;
                int labelToInputGap = 14;
                int colGap = 32;
                int inpW = (gridWidth - (2 * lblW + 2 * labelToInputGap + colGap)) / 2;
                if (inpW < 250) inpW = 250;

                int col1LblX = startX;
                int col1InpX = col1LblX + lblW + labelToInputGap;
                int col2LblX = col1InpX + inpW + colGap;
                int col2InpX = col2LblX + lblW + labelToInputGap;
                int fullInpW = (col2InpX + inpW) - col1InpX;

                var sortedLabels = formFields.OfType<Label>().OrderBy(l => l.Top).ThenBy(l => l.Left).ToList();
                var availableInputs = formFields.Where(c => !(c is Label)).OrderBy(c => c.Top).ThenBy(c => c.Left).ToList();

                int rowHeight = 56;
                int inputHeight = 34;
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
                    bool isNumeric = (input is NumericUpDown);
                    
                    // Only wide for multi-line / free-text fields (NOT for NumericUpDown, ComboBox, or simple text)
                    bool isWideField = false;
                    if (!isNumeric && !(input is ComboBox))
                    {
                        if (isRichText || (input != null && (input.Width > 380 || (input is TextBox tb && tb.Multiline))))
                        {
                            isWideField = true;
                        }
                        else if (lbl != null)
                        {
                            string lText = lbl.Text.ToLower();
                            if (lText.Contains("instruction") || lText.Contains("remark") || lText.Contains("precaution") ||
                                lText.Contains("caution order detail") || lText.Contains("safety condition") ||
                                lText.Contains("obstruction warning") || lText.Contains("stop short") ||
                                lText.Contains("description") || lText.Contains("reason for") || lText.Contains("action taken"))
                            {
                                isWideField = true;
                            }
                        }
                    }

                    if (isWideField && col == 1)
                    {
                        currentY += rowHeight;
                        col = 0;
                    }

                    int curLblX = (col == 0) ? col1LblX : col2LblX;
                    int curInpX = (col == 0) ? col1InpX : col2InpX;

                    if (lbl != null)
                    {
                        lbl.AutoSize = false;
                        lbl.Location = new Point(curLblX, currentY + 4);
                        lbl.Size = new Size(lblW, 28);
                        lbl.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
                        lbl.ForeColor = Color.FromArgb(30, 41, 59);
                        lbl.TextAlign = ContentAlignment.MiddleRight;
                        lbl.AutoEllipsis = true;
                    }

                    if (input != null)
                    {
                        availableInputs.Remove(input);
                        input.Location = new Point(curInpX, currentY);

                        if (isNumeric)
                        {
                            input.Size = new Size(160, inputHeight);
                        }
                        else if (isWideField)
                        {
                            input.Size = new Size(fullInpW, isRichText ? 90 : inputHeight);
                        }
                        else
                        {
                            input.Size = new Size(inpW, inputHeight);
                        }

                        StyleControl(input);
                        input.BringToFront();
                    }

                    if (isWideField)
                    {
                        currentY += isRichText ? 105 : rowHeight;
                        col = 0;
                    }
                    else
                    {
                        if (col == 0)
                        {
                            col = 1;
                        }
                        else
                        {
                            currentY += rowHeight;
                            col = 0;
                        }
                    }
                }

                // Place any leftover checkboxes or inputs
                if (col == 1) { currentY += rowHeight; col = 0; }
                foreach (Control remaining in availableInputs)
                {
                    if (remaining is CheckBox chk)
                    {
                        chk.Location = new Point(col1InpX, currentY + 4);
                        chk.Size = new Size(fullInpW, 30);
                        chk.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
                        chk.ForeColor = Color.FromArgb(30, 41, 59);
                        chk.Cursor = Cursors.Hand;
                        currentY += 44;
                    }
                    else
                    {
                        remaining.Location = new Point(col1InpX, currentY);
                        remaining.Size = new Size(fullInpW, inputHeight);
                        StyleControl(remaining);
                        remaining.BringToFront();
                        currentY += rowHeight;
                    }
                }
            }

            int maxBottom = 0;
            foreach (Control c in scrollPanel.Controls)
            {
                if (c.Visible && c.Bottom > maxBottom) maxBottom = c.Bottom;
            }
            int totalFieldsHeight = Math.Max(currentY, maxBottom) + 25;
            Panel headerPanel = cardPanel.Controls.OfType<Panel>().FirstOrDefault(p => p.Dock == DockStyle.Top || (p.Height <= 100 && p.Name != "CustomWebTitleBar" && p.Name != "BottomActionBar" && p.Name != "ContentScrollPanel"));
            int headerH = (headerPanel != null && headerPanel.Visible) ? headerPanel.Height : 0;
            int bottomBarHeight = (actionButtons.Count > 0) ? 72 : 0;
            int desiredCardHeight = headerH + totalFieldsHeight + bottomBarHeight + 50;

            int minCardHeight = 460;
            int maxCardHeight = Math.Max(minCardHeight, form.ClientSize.Height - 78 - 25);
            int cardHeight = Math.Max(minCardHeight, Math.Min(desiredCardHeight, maxCardHeight));

            int availableHeight = form.ClientSize.Height - 70;
            int cardY = 70 + Math.Max(15, (availableHeight - cardHeight) / 2);
            int cardX = Math.Max(20, (form.ClientSize.Width - cardWidth) / 2);

            cardPanel.Size = new Size(cardWidth, cardHeight);
            cardPanel.Location = new Point(cardX, cardY);

            // 7. Move and style action buttons in the bottom bar
            if (actionButtons.Count > 0)
            {
                Func<Button, int> getBtnPriority = b =>
                {
                    string txt = b.Text.ToUpper();
                    if (txt.Contains("SAVE") || txt.Contains("SUBMIT")) return 1;
                    if (txt.Contains("SEND") || txt.Contains("DISPATCH")) return 2;
                    if (txt.Contains("VIEW")) return 3;
                    if (txt.Contains("CLEAR") || txt.Contains("RESET")) return 4;
                    if (txt.Contains("BACK") || txt.Contains("CLOSE") || txt.Contains("EXIT")) return 5;
                    return 6;
                };

                actionButtons = actionButtons.OrderBy(getBtnPriority).ThenBy(b => b.Left).ToList();
                int btnW = actionButtons.Count >= 5 ? 135 : 155;
                int btnH = 46;
                int btnGap = 16;
                int totalBtnW = actionButtons.Count * btnW + (actionButtons.Count - 1) * btnGap;
                int startBtnX = Math.Max(20, (bottomBar.Width - totalBtnW) / 2);
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

            scrollPanel.Invalidate();
        }

        private static void ScrollPanel_PaintBorders(object sender, PaintEventArgs e)
        {
            Panel panel = sender as Panel;
            if (panel == null) return;

            using (Pen borderPen = new Pen(Color.FromArgb(148, 163, 184), 1.5f))
            {
                foreach (Control c in panel.Controls)
                {
                    if (c.Visible && c is ComboBox)
                    {
                        e.Graphics.DrawRectangle(borderPen, c.Left - 1, c.Top - 1, c.Width + 1, c.Height + 1);
                    }
                }
            }
        }

        private static void StyleControl(Control c)
        {
            if (c is TextBox tb)
            {
                tb.Font = new Font("Segoe UI", 10.5F, FontStyle.Regular);
                tb.BorderStyle = BorderStyle.FixedSingle;
                if (tb.ReadOnly)
                {
                    tb.BackColor = Color.FromArgb(241, 245, 249);
                    tb.ForeColor = Color.FromArgb(71, 85, 105);
                }
                else
                {
                    tb.BackColor = Color.White;
                    tb.ForeColor = Color.FromArgb(15, 23, 42);
                }
            }
            else if (c is ComboBox cb)
            {
                cb.Font = new Font("Segoe UI", 10.5F, FontStyle.Regular);
                cb.FlatStyle = FlatStyle.Standard;
                cb.BackColor = Color.White;
                cb.ForeColor = Color.FromArgb(15, 23, 42);
            }
            else if (c is DateTimePicker dtp)
            {
                dtp.Font = new Font("Segoe UI", 10.5F, FontStyle.Regular);
            }
            else if (c is NumericUpDown num)
            {
                num.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
                num.TextAlign = HorizontalAlignment.Center;
                num.BackColor = Color.White;
                num.ForeColor = Color.FromArgb(15, 23, 42);
            }
            else if (c is RichTextBox rtb)
            {
                rtb.Font = new Font("Segoe UI", 10.5F, FontStyle.Regular);
                rtb.BorderStyle = BorderStyle.FixedSingle;
            }
        }

        private static void StyleModernButton(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;

            string txt = btn.Text.ToUpper();
            if (txt.Contains("SAVE") || txt.Contains("SUBMIT"))
            {
                btn.BackColor = IRCTCColors.ActionOrange;
                btn.ForeColor = Color.White;
                btn.MouseEnter -= Orange_MouseEnter;
                btn.MouseLeave -= Orange_MouseLeave;
                btn.MouseEnter += Orange_MouseEnter;
                btn.MouseLeave += Orange_MouseLeave;
            }
            else if (txt.Contains("SEND") || txt.Contains("DISPATCH"))
            {
                btn.BackColor = IRCTCColors.SuccessGreen;
                btn.ForeColor = Color.White;
                btn.MouseEnter -= Green_MouseEnter;
                btn.MouseLeave -= Green_MouseLeave;
                btn.MouseEnter += Green_MouseEnter;
                btn.MouseLeave += Green_MouseLeave;
            }
            else if (txt.Contains("VIEW"))
            {
                btn.BackColor = IRCTCColors.PrimaryNavy;
                btn.ForeColor = Color.White;
                btn.MouseEnter -= Navy_MouseEnter;
                btn.MouseLeave -= Navy_MouseLeave;
                btn.MouseEnter += Navy_MouseEnter;
                btn.MouseLeave += Navy_MouseLeave;
            }
            else if (txt.Contains("CLEAR") || txt.Contains("RESET"))
            {
                btn.BackColor = Color.FromArgb(241, 245, 249);
                btn.ForeColor = IRCTCColors.SlateText;
                btn.FlatAppearance.BorderSize = 1;
                btn.FlatAppearance.BorderColor = IRCTCColors.BorderGray;
            }
            else if (txt.Contains("BACK") || txt.Contains("CLOSE") || txt.Contains("EXIT"))
            {
                btn.BackColor = Color.FromArgb(241, 245, 249);
                btn.ForeColor = IRCTCColors.DangerRed;
                btn.FlatAppearance.BorderSize = 1;
                btn.FlatAppearance.BorderColor = IRCTCColors.BorderGray;
            }
        }

        private static void Orange_MouseEnter(object sender, EventArgs e) => ((Button)sender).BackColor = IRCTCColors.OrangeHover;
        private static void Orange_MouseLeave(object sender, EventArgs e) => ((Button)sender).BackColor = IRCTCColors.ActionOrange;
        private static void Green_MouseEnter(object sender, EventArgs e) => ((Button)sender).BackColor = IRCTCColors.GreenHover;
        private static void Green_MouseLeave(object sender, EventArgs e) => ((Button)sender).BackColor = IRCTCColors.SuccessGreen;
        private static void Navy_MouseEnter(object sender, EventArgs e) => ((Button)sender).BackColor = IRCTCColors.DarkNavy;
        private static void Navy_MouseLeave(object sender, EventArgs e) => ((Button)sender).BackColor = IRCTCColors.PrimaryNavy;

        public static void InjectCustomTitleBar(Form form)
        {
            Panel existing = form.Controls["CustomWebTitleBar"] as Panel;
            if (existing != null) form.Controls.Remove(existing);

            Panel titleBar = new Panel
            {
                Name = "CustomWebTitleBar",
                Dock = DockStyle.Top,
                Height = 65,
                BackColor = IRCTCColors.DarkNavy
            };

            Label lblLogo = new Label
            {
                Text = "🚆",
                Font = new Font("Segoe UI Emoji", 20),
                ForeColor = Color.White,
                Location = new Point(16, 12),
                Size = new Size(42, 42),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };
            titleBar.Controls.Add(lblLogo);

            Label lblTitle = new Label
            {
                Text = form.Text.ToUpper(),
                Font = new Font("Segoe UI", 11.5f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(64, 12),
                AutoSize = true,
                UseMnemonic = false,
                BackColor = Color.Transparent
            };
            titleBar.Controls.Add(lblTitle);

            Label lblSub = new Label
            {
                Text = "Train Working Management System  •  Operational Authorities & Safety Register",
                Font = new Font("Segoe UI", 8.5f),
                ForeColor = Color.FromArgb(180, 205, 240),
                Location = new Point(66, 36),
                AutoSize = true,
                UseMnemonic = false,
                BackColor = Color.Transparent
            };
            titleBar.Controls.Add(lblSub);

            // Active User Profile Badge in the Top Right of EVERY authority form
            string deptName = string.IsNullOrWhiteSpace(SessionManager.CurrentDepartment) ? "Operating" : SessionManager.CurrentDepartment;
            string userName = string.IsNullOrWhiteSpace(SessionManager.CurrentFullName) ? "Station Operator" : SessionManager.CurrentFullName;

            Panel pnlUser = new Panel
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
            pnlUser.Controls.Add(lblAvatar);

            Label lblUser = new Label
            {
                Text = $"👤  {userName} ({deptName})",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(254, 240, 138), // Gold
                Location = new Point(38, 10),
                AutoSize = true,
                UseMnemonic = false,
                BackColor = Color.Transparent
            };
            pnlUser.Controls.Add(lblUser);
            titleBar.Controls.Add(pnlUser);

            Button btnClose = new Button
            {
                Text = "✕ CLOSE",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = IRCTCColors.DangerRed,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(100, 36),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => form.Close();
            titleBar.Controls.Add(btnClose);

            Action layoutTitleBar = () =>
            {
                int r = titleBar.ClientSize.Width - 18;
                btnClose.Location = new Point(r - 100, 14);
                r -= 115;

                int chipWidth = TextRenderer.MeasureText(lblUser.Text, lblUser.Font).Width + 55;
                pnlUser.Size = new Size(chipWidth, 40);
                pnlUser.Location = new Point(r - chipWidth, 12);
            };

            titleBar.Resize += (s, e) => layoutTitleBar();
            form.Shown += (s, e) => layoutTitleBar();

            form.Controls.Add(titleBar);
            titleBar.BringToFront();
        }

        private static string GetInitials(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName)) return "U";
            var parts = fullName.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1) return parts[0].Substring(0, Math.Min(2, parts[0].Length)).ToUpper();
            return (parts[0][0].ToString() + parts[parts.Length - 1][0].ToString()).ToUpper();
        }

        public static void EnableDoubleBuffering(Control c)
        {
            try
            {
                var prop = typeof(Control).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                prop?.SetValue(c, true, null);
            }
            catch { }
        }
    }
}
