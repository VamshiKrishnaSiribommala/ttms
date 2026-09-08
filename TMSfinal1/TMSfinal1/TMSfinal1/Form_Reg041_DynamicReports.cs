using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace TMS
{
    /// <summary>
    /// Custom DataGridView that natively handles Touchpad 2-finger horizontal swipe/scroll, 
    /// Shift+Wheel horizontal scrolling, and mouse wheel tilt (WM_MOUSEHWHEEL).
    /// </summary>
    public class TouchpadScrollableDataGridView : DataGridView
    {
        private const int WM_MOUSEHWHEEL = 0x020E;
        private const int WM_MOUSEWHEEL = 0x020A;

        public TouchpadScrollableDataGridView()
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
        }

        protected override void WndProc(ref Message m)
        {
            // Touchpad Horizontal Swipe / Tilt Wheel
            if (m.Msg == WM_MOUSEHWHEEL)
            {
                short delta = (short)((m.WParam.ToInt64() >> 16) & 0xffff);
                if (delta != 0)
                {
                    try
                    {
                        int step = (delta > 0) ? 60 : -60;
                        int newOffset = this.HorizontalScrollingOffset + step;
                        if (newOffset < 0) newOffset = 0;
                        this.HorizontalScrollingOffset = newOffset;
                        m.Result = IntPtr.Zero;
                        return;
                    }
                    catch { }
                }
            }
            // Shift + Mouse Wheel = Horizontal Scroll
            else if (m.Msg == WM_MOUSEWHEEL)
            {
                if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                {
                    short delta = (short)((m.WParam.ToInt64() >> 16) & 0xffff);
                    if (delta != 0)
                    {
                        try
                        {
                            int step = (delta > 0) ? -60 : 60;
                            int newOffset = this.HorizontalScrollingOffset + step;
                            if (newOffset < 0) newOffset = 0;
                            this.HorizontalScrollingOffset = newOffset;
                            m.Result = IntPtr.Zero;
                            return;
                        }
                        catch { }
                    }
                }
            }

            base.WndProc(ref m);
        }
    }

    /// <summary>
    /// Interactive Multi-Select Register Checkbox Dropdown Selector.
    /// Provides Checkboxes for all 40 Registers with Quick Presets, Search filtering,
    /// and dynamic status summary display.
    /// </summary>
    public class CheckedRegisterSelector : Panel
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, string lParam);

        public class RegisterItem
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public int Number => int.TryParse(Code, out int n) ? n : 0;
            public bool IsChecked { get; set; } = true;
            public override string ToString() => $"{Code} - {Name}";
        }

        private readonly List<RegisterItem> allItems = new List<RegisterItem>();
        private readonly List<RegisterItem> filteredItems = new List<RegisterItem>();
        private ToolStripDropDown dropDown;
        private CheckedListBox checkedListBox;
        private TextBox txtSearch;
        private Label lblSummary;
        private Label lblDropChevron;
        private Label lblPopupCount;
        private bool isUpdatingChecks = false;

        public event EventHandler SelectionChanged;

        public CheckedRegisterSelector()
        {
            this.Size = new Size(360, 36);
            this.BackColor = Color.White;
            this.Cursor = Cursors.Hand;
            this.DoubleBuffered = true;

            // Paint Border
            this.Paint += (s, e) =>
            {
                using (Pen p = new Pen(Color.FromArgb(203, 213, 225), 1.2f))
                {
                    e.Graphics.DrawRectangle(p, 0, 0, this.Width - 1, this.Height - 1);
                }
            };

            lblSummary = new Label
            {
                Text = "☑ ALL REGISTERS (001 - 040 CONSOLIDATED)",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 61, 119),
                Location = new Point(8, 7),
                Size = new Size(this.Width - 36, 20),
                AutoEllipsis = false,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            lblSummary.Click += (s, e) => ToggleDropDown();
            this.Controls.Add(lblSummary);

            lblDropChevron = new Label
            {
                Text = "▼",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(71, 85, 105),
                Location = new Point(this.Width - 26, 7),
                Size = new Size(20, 20),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            lblDropChevron.Click += (s, e) => ToggleDropDown();
            this.Controls.Add(lblDropChevron);

            this.Resize += (s, e) =>
            {
                lblSummary.Size = new Size(this.Width - 36, 20);
                lblDropChevron.Location = new Point(this.Width - 26, (this.Height - lblDropChevron.Height) / 2);
            };

            this.Click += (s, e) => ToggleDropDown();

            BuildDropDown();
        }

        private void BuildDropDown()
        {
            Panel popupPanel = new Panel
            {
                Size = new Size(460, 520),
                BackColor = Color.White,
                Padding = new Padding(0)
            };
            popupPanel.Paint += (s, e) =>
            {
                using (Pen p = new Pen(Color.FromArgb(33, 61, 119), 2f))
                {
                    e.Graphics.DrawRectangle(p, 0, 0, popupPanel.Width - 1, popupPanel.Height - 1);
                }
            };

            // 1. Popup Title Header
            Panel pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 42,
                BackColor = Color.FromArgb(33, 61, 119)
            };
            Label lblPopupTitle = new Label
            {
                Text = "📋  SELECT REGISTERS TO AUDIT & PRINT",
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(12, 10),
                AutoSize = true
            };
            pnlHeader.Controls.Add(lblPopupTitle);
            popupPanel.Controls.Add(pnlHeader);

            // 2. Search Box
            Panel pnlSearch = new Panel
            {
                Dock = DockStyle.Top,
                Height = 44,
                BackColor = Color.FromArgb(248, 250, 252),
                Padding = new Padding(10, 6, 10, 6)
            };
            txtSearch = new TextBox
            {
                Font = new Font("Segoe UI", 10.5F),
                BorderStyle = BorderStyle.FixedSingle,
                Dock = DockStyle.Fill
            };
            txtSearch.TextChanged += (s, e) => FilterRegisterList(txtSearch.Text);
            txtSearch.HandleCreated += (s, e) => SendMessage(txtSearch.Handle, 0x1501, 1, "🔍 Search register name or code...");
            pnlSearch.Controls.Add(txtSearch);
            popupPanel.Controls.Add(pnlSearch);

            // 3. Quick Presets (Select All, Clear, Categories)
            Panel pnlPresets = new Panel
            {
                Dock = DockStyle.Top,
                Height = 68,
                BackColor = Color.FromArgb(241, 245, 249),
                Padding = new Padding(6, 4, 6, 4)
            };

            Button btnAll = CreatePillButton("✔ Select All", Color.FromArgb(22, 163, 74), 8, 4, 100, 28, () => SetAllChecked(true));
            Button btnNone = CreatePillButton("✖ Clear All", Color.FromArgb(220, 38, 38), 114, 4, 95, 28, () => SetAllChecked(false));
            Button btnOp = CreatePillButton("🚆 Operational (001-014)", Color.FromArgb(30, 58, 138), 215, 4, 230, 28, () => CheckRange(1, 14));

            Button btnMaint = CreatePillButton("🛠 Maintenance", Color.FromArgb(6, 95, 70), 8, 35, 135, 28, () => CheckSpecific(new[] { 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 31, 35, 40 }));
            Button btnInfra = CreatePillButton("⚡ Infrastructure", Color.FromArgb(146, 64, 14), 149, 35, 140, 28, () => CheckRange(25, 30));
            Button btnSafety = CreatePillButton("🚨 Safety", Color.FromArgb(153, 27, 27), 295, 35, 150, 28, () => CheckSpecific(new[] { 32, 33, 34, 36, 37, 38, 39 }));

            pnlPresets.Controls.AddRange(new Control[] { btnAll, btnNone, btnOp, btnMaint, btnInfra, btnSafety });
            popupPanel.Controls.Add(pnlPresets);

            // 4. CheckedListBox
            checkedListBox = new CheckedListBox
            {
                Dock = DockStyle.Fill,
                CheckOnClick = true,
                Font = new Font("Segoe UI", 10.5F),
                BorderStyle = BorderStyle.None,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(15, 23, 42),
                Padding = new Padding(6)
            };
            checkedListBox.ItemCheck += CheckedListBox_ItemCheck;
            popupPanel.Controls.Add(checkedListBox);

            // 5. Popup Footer
            Panel pnlFooter = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 44,
                BackColor = Color.FromArgb(248, 250, 252),
                Padding = new Padding(10, 6, 10, 6)
            };
            pnlFooter.Paint += (s, e) =>
            {
                using (Pen p = new Pen(Color.FromArgb(226, 232, 240)))
                    e.Graphics.DrawLine(p, 0, 0, pnlFooter.Width, 0);
            };

            lblPopupCount = new Label
            {
                Text = "40 of 40 Selected",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(5, 150, 105),
                Location = new Point(10, 12),
                AutoSize = true
            };
            pnlFooter.Controls.Add(lblPopupCount);

            Button btnApply = new Button
            {
                Text = "✓ Apply & Close",
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                BackColor = Color.FromArgb(251, 121, 43), // IRCTC Orange
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(140, 32),
                Location = new Point(300, 6),
                Cursor = Cursors.Hand
            };
            btnApply.FlatAppearance.BorderSize = 0;
            btnApply.Click += (s, e) => dropDown.Close();
            pnlFooter.Controls.Add(btnApply);

            popupPanel.Controls.Add(pnlFooter);

            // Docking order
            pnlFooter.BringToFront();
            pnlPresets.SendToBack();
            pnlSearch.SendToBack();
            pnlHeader.SendToBack();
            checkedListBox.BringToFront();

            ToolStripControlHost host = new ToolStripControlHost(popupPanel)
            {
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                AutoSize = false,
                Size = popupPanel.Size
            };

            dropDown = new ToolStripDropDown
            {
                Margin = Padding.Empty,
                Padding = Padding.Empty,
                AutoClose = true
            };
            dropDown.Items.Add(host);
            dropDown.Closed += (s, e) =>
            {
                UpdateSummaryDisplay();
                SelectionChanged?.Invoke(this, EventArgs.Empty);
            };
        }

        private Button CreatePillButton(string text, Color bg, int x, int y, int w, int h, Action onClick)
        {
            Button btn = new Button
            {
                Text = text,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                BackColor = bg,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(w, h),
                Location = new Point(x, y),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Click += (s, e) => onClick();
            return btn;
        }

        public void InitRegisters(Dictionary<string, Form_Reg041_DynamicReports.RegisterMeta> registry)
        {
            allItems.Clear();
            foreach (var kvp in registry)
            {
                allItems.Add(new RegisterItem
                {
                    Code = kvp.Key,
                    Name = kvp.Value.Name,
                    IsChecked = true
                });
            }
            FilterRegisterList("");
            UpdateSummaryDisplay();
        }

        private void FilterRegisterList(string query)
        {
            isUpdatingChecks = true;
            try
            {
                checkedListBox.Items.Clear();
                filteredItems.Clear();

                string q = query?.Trim().ToLower() ?? "";
                foreach (var item in allItems)
                {
                    if (string.IsNullOrEmpty(q) || item.Code.ToLower().Contains(q) || item.Name.ToLower().Contains(q))
                    {
                        filteredItems.Add(item);
                        checkedListBox.Items.Add(item, item.IsChecked);
                    }
                }
                UpdatePopupCount();
            }
            finally
            {
                isUpdatingChecks = false;
            }
        }

        private void CheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (isUpdatingChecks) return;
            if (e.Index >= 0 && e.Index < filteredItems.Count)
            {
                filteredItems[e.Index].IsChecked = (e.NewValue == CheckState.Checked);
                this.BeginInvoke(new Action(() =>
                {
                    UpdatePopupCount();
                    UpdateSummaryDisplay();
                    SelectionChanged?.Invoke(this, EventArgs.Empty);
                }));
            }
        }

        private void SetAllChecked(bool isChecked)
        {
            isUpdatingChecks = true;
            try
            {
                foreach (var itm in allItems) itm.IsChecked = isChecked;
                for (int i = 0; i < checkedListBox.Items.Count; i++)
                {
                    checkedListBox.SetItemChecked(i, isChecked);
                }
                UpdatePopupCount();
                UpdateSummaryDisplay();
                SelectionChanged?.Invoke(this, EventArgs.Empty);
            }
            finally
            {
                isUpdatingChecks = false;
            }
        }

        private void CheckRange(int startNum, int endNum)
        {
            isUpdatingChecks = true;
            try
            {
                foreach (var itm in allItems)
                {
                    itm.IsChecked = (itm.Number >= startNum && itm.Number <= endNum);
                }
                for (int i = 0; i < filteredItems.Count; i++)
                {
                    checkedListBox.SetItemChecked(i, filteredItems[i].IsChecked);
                }
                UpdatePopupCount();
                UpdateSummaryDisplay();
                SelectionChanged?.Invoke(this, EventArgs.Empty);
            }
            finally
            {
                isUpdatingChecks = false;
            }
        }

        private void CheckSpecific(int[] nums)
        {
            isUpdatingChecks = true;
            try
            {
                HashSet<int> set = new HashSet<int>(nums);
                foreach (var itm in allItems)
                {
                    itm.IsChecked = set.Contains(itm.Number);
                }
                for (int i = 0; i < filteredItems.Count; i++)
                {
                    checkedListBox.SetItemChecked(i, filteredItems[i].IsChecked);
                }
                UpdatePopupCount();
                UpdateSummaryDisplay();
                SelectionChanged?.Invoke(this, EventArgs.Empty);
            }
            finally
            {
                isUpdatingChecks = false;
            }
        }

        private void UpdatePopupCount()
        {
            int checkedCount = allItems.Count(x => x.IsChecked);
            lblPopupCount.Text = $"{checkedCount} of {allItems.Count} Selected";
            lblPopupCount.ForeColor = checkedCount > 0 ? Color.FromArgb(5, 150, 105) : Color.FromArgb(220, 38, 38);
        }

        private void UpdateSummaryDisplay()
        {
            int checkedCount = allItems.Count(x => x.IsChecked);
            if (checkedCount == allItems.Count && allItems.Count > 0)
            {
                lblSummary.Text = "☑ ALL REGISTERS (001 - 040 CONSOLIDATED)";
                lblSummary.ForeColor = Color.FromArgb(33, 61, 119);
            }
            else if (checkedCount == 0)
            {
                lblSummary.Text = "☐ NONE SELECTED (Click to select)";
                lblSummary.ForeColor = Color.FromArgb(220, 38, 38);
            }
            else if (checkedCount == 1)
            {
                var single = allItems.First(x => x.IsChecked);
                lblSummary.Text = $"☑ {single.Code} - {single.Name}";
                lblSummary.ForeColor = Color.FromArgb(33, 61, 119);
            }
            else if (checkedCount <= 3)
            {
                string codes = string.Join(", ", allItems.Where(x => x.IsChecked).Select(x => x.Code));
                lblSummary.Text = $"☑ {checkedCount} Registers: ({codes})";
                lblSummary.ForeColor = Color.FromArgb(33, 61, 119);
            }
            else
            {
                lblSummary.Text = $"☑ {checkedCount} Registers Selected";
                lblSummary.ForeColor = Color.FromArgb(33, 61, 119);
            }
        }

        public HashSet<string> GetSelectedCodes()
        {
            return new HashSet<string>(allItems.Where(x => x.IsChecked).Select(x => x.Code));
        }

        public string GetScopeSummaryText()
        {
            int checkedCount = allItems.Count(x => x.IsChecked);
            if (checkedCount == allItems.Count && allItems.Count > 0)
                return "ALL REGISTERS (001 - 040 CONSOLIDATED)";
            if (checkedCount == 0)
                return "NONE SELECTED";
            if (checkedCount == 1)
            {
                var s = allItems.First(x => x.IsChecked);
                return $"REG-{s.Code} - {s.Name}";
            }
            if (checkedCount <= 4)
            {
                return $"{checkedCount} Registers: " + string.Join(", ", allItems.Where(x => x.IsChecked).Select(x => $"REG-{x.Code}"));
            }
            return $"{checkedCount} Selected Registers ({allItems.First(x => x.IsChecked).Code} ... {allItems.Last(x => x.IsChecked).Code})";
        }

        public void ToggleDropDown()
        {
            if (dropDown.Visible)
            {
                dropDown.Close();
            }
            else
            {
                txtSearch.Clear();
                FilterRegisterList("");
                Point pt = this.PointToScreen(new Point(0, this.Height));
                dropDown.Show(pt);
                txtSearch.Focus();
            }
        }
    }

    /// <summary>
    /// Premium Glassmorphism / "Class" Action Button with smooth hover elevation,
    /// rounded corners, specular glass gloss reflection highlight, and high-contrast styling.
    /// </summary>
    public class ClassGlassButton : Button
    {
        private bool isHovered = false;
        private bool isPressed = false;
        private readonly Color baseColor;

        public ClassGlassButton(string text, Color color, int x, int y, int width, int height, Action onClick)
        {
            this.Text = text;
            this.baseColor = color;
            this.Location = new Point(x, y);
            this.Size = new Size(width, height);
            this.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
            this.ForeColor = Color.White;
            this.FlatStyle = FlatStyle.Flat;
            this.FlatAppearance.BorderSize = 0;
            this.Cursor = Cursors.Hand;
            this.DoubleBuffered = true;

            this.MouseEnter += (s, e) => { isHovered = true; this.Invalidate(); };
            this.MouseLeave += (s, e) => { isHovered = false; isPressed = false; this.Invalidate(); };
            this.MouseDown  += (s, e) => { isPressed = true; this.Invalidate(); };
            this.MouseUp    += (s, e) => { isPressed = false; this.Invalidate(); };

            if (onClick != null)
            {
                this.Click += (s, e) => onClick();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            Rectangle rect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);
            int radius = 8;

            // Determine gradient tones based on hover / pressed state
            Color topCol, botCol;
            if (isPressed)
            {
                topCol = ControlPaint.Dark(baseColor, 0.15f);
                botCol = ControlPaint.Dark(baseColor, 0.30f);
            }
            else if (isHovered)
            {
                topCol = ControlPaint.Light(baseColor, 0.28f);
                botCol = ControlPaint.Light(baseColor, 0.08f);
            }
            else
            {
                topCol = ControlPaint.Light(baseColor, 0.12f);
                botCol = baseColor;
            }

            using (GraphicsPath path = CreateRoundedRectanglePath(rect, radius))
            {
                // 1. Base vibrant gradient fill
                using (LinearGradientBrush lgb = new LinearGradientBrush(rect, topCol, botCol, LinearGradientMode.Vertical))
                {
                    g.FillPath(lgb, path);
                }

                // 2. Translucent specular "Glass" reflection on top half
                Rectangle glossRect = new Rectangle(1, 1, this.Width - 3, (this.Height / 2) - 1);
                using (GraphicsPath glossPath = CreateTopRoundedPath(glossRect, radius))
                {
                    int glossAlpha = isHovered ? 90 : 55;
                    using (LinearGradientBrush glossBrush = new LinearGradientBrush(glossRect, 
                        Color.FromArgb(glossAlpha, 255, 255, 255), 
                        Color.FromArgb(5, 255, 255, 255), 
                        LinearGradientMode.Vertical))
                    {
                        g.FillPath(glossBrush, glossPath);
                    }
                }

                // 3. Crisp Glass Border highlight
                Color borderColor = isHovered ? Color.FromArgb(245, 255, 255, 255) : Color.FromArgb(130, 255, 255, 255);
                using (Pen borderPen = new Pen(borderColor, isHovered ? 1.6f : 1.2f))
                {
                    g.DrawPath(borderPen, path);
                }
            }

            // 4. Centered Icon + Text rendering
            TextFormatFlags flags = TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix;
            TextRenderer.DrawText(g, this.Text, this.Font, rect, Color.White, flags);
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

        private static GraphicsPath CreateTopRoundedPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            if (rect.Width <= 0 || rect.Height <= 0) return path;
            int d = radius * 2;
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddLine(rect.Right, rect.Bottom, rect.Left, rect.Bottom);
            path.CloseFigure();
            return path;
        }
    }

    /// <summary>
    /// Consolidated Multi-Register Dynamic Reports & Cross-Register Audit (REG-041).
    /// Features:
    /// - Auto-fetch data on date or register selection change (Strict Max 3 Days).
    /// - Touchpad horizontal 2-finger scroll and drag support.
    /// - Window bounds respect the Windows Taskbar (taskbar stays fully visible).
    /// - 11 structured columns with clean '-' dash for unavailable register fields.
    /// - Multi-page PDF generator without banner overlap and complete columns.
    /// </summary>
    public class Form_Reg041_DynamicReports : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, string lParam);

        // UI Panels
        private Panel titleBar;
        private Panel filterPanel;
        private Panel actionBar;
        private Panel gridPanel;

        // UI Controls
        private CheckedRegisterSelector chkRegisterSelector;
        private DateTimePicker dtpFromDate;
        private DateTimePicker dtpToDate;
        private Button btnPresetToday;
        private Button btnPresetYesterdayToday;
        private Button btnPreset3Days;
        private TextBox txtSearch;
        private Label lblTotalRecords;
        private Label lblActiveRegisters;
        private Label lblDateRangeInfo;
        private TouchpadScrollableDataGridView dgvResults;
        private ClassGlassButton btnExportPDF;
        private ClassGlassButton btnExportExcel;
        private ClassGlassButton btnExportCSV;
        private ClassGlassButton btnPrint;

        // Data Storage
        private DataTable dtConsolidated;
        private readonly DatabaseHelper db = new DatabaseHelper();
        private readonly Dictionary<string, RegisterMeta> registerRegistry = new Dictionary<string, RegisterMeta>();
        private bool isInitializing = true;

        // Print Support
        private PrintDocument printDoc;
        private int printRowIndex = 0;
        private int printPageNumber = 1;

        public class RegisterMeta
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public string TableName { get; set; }
            public string IdColumn { get; set; }
            public string DateColumn { get; set; }
            public string StaffColumn { get; set; }
            public string CategoryColumn { get; set; }
            public string DescriptionColumn { get; set; }
            public string AssetColumn { get; set; }
            public string StatusColumn { get; set; }
            public string ReportedByColumn { get; set; }
        }


        public Form_Reg041_DynamicReports()
        {
            this.Text = "Dynamic Reports & Consolidated Audit (REG-041)";
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.Manual;

            // Fit working area so the Windows Taskbar is NEVER hidden
            Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
            this.Location = workingArea.Location;
            this.Size = workingArea.Size;
            this.MinimumSize = new Size(1000, 600);
            this.BackColor = Color.FromArgb(241, 245, 249);
            this.Font = new Font("Segoe UI", 10F);

            // Double Buffering
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.DoubleBuffered = true;

            this.Load += (s, e) =>
            {
                // Ensure taskbar is visible on current screen
                this.Bounds = Screen.FromControl(this).WorkingArea;
            };

            InitializeRegisterRegistry();
            BuildUI();

            isInitializing = false;

            // Auto-fetch data (Strict Maximum 3 Days)
            // First check if Today has records; if not, check latest entered date to immediately show user's data
            DateTime latestDate = GetLatestRecordDate();
            if (latestDate != DateTime.MinValue && latestDate.Date < DateTime.Today)
            {
                DateTime start = latestDate.AddDays(-2);
                dtpFromDate.Value = start;
                dtpToDate.Value = latestDate;
                FetchConsolidatedRecords();
            }
            else
            {
                ApplyPreset(1);
            }

            // Auto-refresh data when user navigates back to this window
            this.Activated += (s, e) =>
            {
                if (!isInitializing)
                {
                    FetchConsolidatedRecords();
                }
            };
        }

        // ─────────────────────────────────────────────────────────────────────
        // 1. SYSTEMATIC REGISTER METADATA REGISTRY (REG-001 TO REG-040)
        // ─────────────────────────────────────────────────────────────────────
        private void InitializeRegisterRegistry()
        {
            AddReg("001", "Station Master's Diary", "Reg001_StationDiary", "LogID", "EventTime", "SubmittedBy", "Category", "Description", "", "Remarks", "ReportedBy");
            AddReg("002", "Train Signal Register", "Reg002_TrainSignal", "EntryID", "TrainArrival", "SubmittedBy", "Direction", "LineNumber", "TrainNumber", "SMonDuty", "SMonDuty");
            AddReg("003", "SWR Acknowledgment", "Reg003_SWR", "AckID", "DateOfReading", "SubmittedBy", "SWRVersion", "DateOfReading", "", "VerifiedBy", "StaffID");
            AddReg("004", "Caution Order", "Reg004_CautionOrder", "CautionID", "ValidityStart", "SubmittedBy", "Section", "Reason", "SpeedLimit", "IssuedTo", "IssuedTo");
            AddReg("005", "Signal/Point/Block Failure", "Reg005_Failure", "FailureID", "FailureTime", "SubmittedBy", "AssetType", "FailureTime", "AssetID", "RectificationTime", "ReportedTo");
            AddReg("006", "S&T Disconnection/Reconnection", "Reg006_DisconRecon", "MemoID", "DisconnectionTime", "SubmittedBy", "GearID", "DisconnectionTime", "GearID", "ReconnectionTime", "MaintainerID");
            AddReg("007", "Bio-Metric Attendance", "Reg007_Attendance", "AttendanceID", "PunchInTime", "SubmittedBy", "ShiftType", "PunchInTime", "", "Status", "StaffID");
            AddReg("008", "Stable Load", "Reg008_StableLoad", "StablingID", "StabledTime", "SubmittedBy", "LineNumber", "TrainLoadID", "LineNumber", "HandBrakeStatus", "SubmittedBy");
            AddReg("009", "Fog Signalman", "Reg009_FogSignalman", "DeploymentID", "StartTime", "SubmittedBy", "Location", "DetonatorsUsed", "Location", "StartTime", "StaffID");
            AddReg("010", "Night Inspection", "Reg010_NightInspection", "InspectionID", "TimeOfVisit", "SubmittedBy", "StaffAlertness", "Observations", "", "ActionSuggested", "InspectingOfficer");
            AddReg("011", "Public Complaint", "Reg011_PublicComplaint", "ComplaintID", "SubmittedAt", "SubmittedBy", "Category", "Description", "PNR_TicketNo", "Status", "ComplainantName");
            AddReg("012", "Staff Grievance", "Reg012_StaffGrievance", "GrievanceID", "SubmittedAt", "SubmittedBy", "IssueType", "Subject", "", "Status", "StaffID");
            AddReg("013", "Inspection & Observation", "Reg013_Inspection", "VisitID", "InspectionDate", "SubmittedBy", "Scope", "Observations", "", "ComplianceDate", "OfficerID");
            AddReg("014", "Miscellaneous Counter", "Reg014_MiscCounter", "CounterLogID", "OperationTime", "SubmittedBy", "CounterType", "ReasonCode", "AssetID", "AuditFlag", "InitiatorID");
            AddReg("015", "Siding Key", "Reg015_SidingKey", "TransactionID", "IssueTime", "SubmittedBy", "KeyType", "Purpose", "KeyNumber", "ChecklistStatus", "IssuedTo");
            AddReg("016", "Crank Handle", "Reg016_CrankHandle", "LogID", "IssueTime", "SubmittedBy", "PointNumber", "AuthorizationPN", "CrankHandleID", "SafetyOverride", "OperatorID");
            AddReg("017", "Crank Handle Testing", "Reg017_CrankHandleTest", "TestID", "TestDate", "SubmittedBy", "TestType", "TestResult", "HandleID", "NextDueDate", "TesterID");
            AddReg("018", "Cross-Over Testing", "Reg018_CrossoverTest", "TestID", "SubmittedAt", "SubmittedBy", "CrossoverID", "LockingVerified", "CrossoverID", "DetectionVerified", "MaintainerID");
            AddReg("019", "Signal Failure", "Reg019_SignalFailure", "FailureID", "FailureTime", "SubmittedBy", "FailureType", "RootCause", "SignalNumber", "ActionTaken", "ReportingStaff");
            AddReg("020", "Emergency Key", "Reg020_EmergencyKey", "LogID", "IssueTime", "SubmittedBy", "EmergencyKeyID", "IssueTime", "AssetID", "ChecklistStatus", "IssuingSMID");
            AddReg("021", "Complete Arrival", "Reg021_CompleteArrival", "VerificationID", "ArrivalTime", "SubmittedBy", "TrainNumber", "WagonCount", "TrainNumber", "SMVerification", "GuardID");
            AddReg("022", "Control Instruction", "Reg022_ControlInstruction", "MessageID", "ValidityStart", "SubmittedBy", "MessageType", "Content", "", "ComplianceStatus", "ControllerID");
            AddReg("023", "SM Relief Diary", "Reg023_SMRelief", "EntryID", "HandoverTime", "SubmittedBy", "WeatherStatus", "PendingIssues", "", "HandoverTime", "RelievingSMID");
            AddReg("024", "Traffic/Power Block", "Reg024_TrafficBlock", "BlockID", "ActualStart", "SubmittedBy", "BlockType", "AffectedSection", "AffectedSection", "ApprovedBy", "RequestDept");
            AddReg("025", "Safety Meeting", "Reg025_SafetyMeeting", "MeetingID", "MeetingDate", "SubmittedBy", "MeetingType", "Agenda", "Venue", "MeetingStatus", "ChairpersonName");
            AddReg("026", "HQ Safety Circular", "Reg026_SafetyCircular", "CircularID", "EffectiveDate", "SubmittedBy", "CircularNumber", "Subject", "", "ImplementationStatus", "AckBySM");
            AddReg("027", "Safety Meeting (Part 2)", "Reg027_SafetyMeeting2", "MeetingID", "MeetingDate", "SubmittedBy", "MeetingType", "Minutes", "Venue", "ActionItems", "PresidedBy");
            AddReg("028", "Staff Biodata", "Reg028_StaffBiodata", "EmployeeID", "PMEDate", "SubmittedBy", "Department", "Designation", "", "RenewalAlert", "StaffName");
            AddReg("029", "Assurance", "Reg029_Assurance", "AssuranceID", "EffectiveDate", "SubmittedBy", "DocumentType", "DocumentID", "", "ReadConfirmation", "SubmittedBy");
            AddReg("030", "Private Number Sheet", "Reg030_PNSheet", "PNNumber", "ExchangeTime", "SubmittedBy", "Purpose", "AssocTrainNo", "FromStation", "PNStatus", "RecipientName");
            AddReg("031", "Petty Repair", "Reg031_PettyRepair", "ComplaintID", "ComplaintTime", "SubmittedBy", "AssetCategory", "DefectDescription", "AssetID", "CompletionStatus", "AssignedDept");
            AddReg("032", "Attendance", "Reg032_Attendance", "AttendanceID", "ShiftStartTime", "SubmittedBy", "ShiftAssigned", "Designation", "", "AttendanceStatus", "StaffName");
            AddReg("033", "Passenger Complaint", "Reg033_PassengerComplaint", "EntryID", "ComplaintDateTime", "SubmittedBy", "Category", "ComplaintDetails", "PNR_TicketNo", "Status", "PassengerName");
            AddReg("034", "Employee Complaint", "Reg034_EmployeeComplaint", "ReferenceID", "ComplaintDateTime", "SubmittedBy", "IssueCategory", "IssueDescription", "Department", "Status", "EmployeeName");
            AddReg("035", "Power Supply", "Reg035_PowerSupply", "EntryID", "FailureTime", "SubmittedBy", "PrimarySource", "SecondarySource", "", "RestorationTime", "RecordedBy");
            AddReg("036", "Officers Inspection", "Reg036_OfficersInspection", "InspectionID", "DateOfVisit", "SubmittedBy", "Designation", "ItemsInspected", "StationInspected", "Status", "OfficerName");
            AddReg("037", "TI Inspection", "Reg037_TIInspection", "InspectionID", "DateOfVisit", "SubmittedBy", "StaffAlertness", "OperationalFindings", "", "Status", "TINameID");
            AddReg("038", "Joint Inspection", "Reg038_JointInspection", "InspectionID", "InspectionDate", "SubmittedBy", "ParameterType", "DeptObservations", "AssetID", "ConsensusResult", "JointDepts");
            AddReg("039", "Night Inspection", "Reg039_NightInspection", "InspectionID", "InspectionDate", "SubmittedBy", "SignalIDs", "EquipmentStatus", "SignalIDs", "CorrectiveAction", "StaffPresence");
            AddReg("040", "Failure Inspection", "Reg040_FailureInspection", "FailureID", "FailureTime", "SubmittedBy", "FailureClass", "RootCause", "AssetID", "RecordStatus", "VerifiedBy");
        }

        private void AddReg(string code, string name, string tableName, string idCol, string dateCol, string staffCol, string catCol, string descCol, string assetCol, string statusCol, string repCol)
        {
            registerRegistry[code] = new RegisterMeta
            {
                Code = code,
                Name = name,
                TableName = tableName,
                IdColumn = idCol,
                DateColumn = dateCol,
                StaffColumn = staffCol,
                CategoryColumn = catCol,
                DescriptionColumn = descCol,
                AssetColumn = assetCol,
                StatusColumn = statusCol,
                ReportedByColumn = repCol
            };
        }

        // ─────────────────────────────────────────────────────────────────────
        // 2. UI CONSTRUCTION
        // ─────────────────────────────────────────────────────────────────────
        private void BuildUI()
        {
            // ── 1. TITLE BAR (Navy #213D77) ──────────────────────────────────
            titleBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 58,
                BackColor = Color.FromArgb(33, 61, 119)
            };

            Label lblBreadcrumb = new Label
            {
                Text = "Home > Admin Reports > Dynamic Reports (REG-041)",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Italic),
                ForeColor = Color.FromArgb(191, 219, 254),
                Location = new Point(20, 7),
                AutoSize = true
            };
            titleBar.Controls.Add(lblBreadcrumb);

            Label lblTitle = new Label
            {
                Text = "CONSOLIDATED MULTI-REGISTER DYNAMIC REPORTS & AUDIT",
                Font = new Font("Segoe UI", 13.5F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 24),
                AutoSize = true,
                UseMnemonic = false
            };
            titleBar.Controls.Add(lblTitle);

            Button btnClose = new Button
            {
                Text         = "✕ CLOSE",
                Font         = new Font("Segoe UI", 10F, FontStyle.Bold),
                Size         = new Size(110, 36),
                BackColor    = Color.FromArgb(220, 38, 38),
                ForeColor    = Color.White,
                FlatStyle    = FlatStyle.Flat,
                Cursor       = Cursors.Hand,
                DialogResult = DialogResult.Cancel
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            titleBar.Controls.Add(btnClose);
            titleBar.Resize += (s, e) =>
                btnClose.Location = new Point(titleBar.Width - btnClose.Width - 20, (titleBar.Height - btnClose.Height) / 2);


            filterPanel = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 84,
                BackColor = Color.White
            };
            filterPanel.Paint += (s, e) =>
                e.Graphics.DrawLine(new Pen(Color.FromArgb(226, 232, 240), 1),
                    0, filterPanel.Height - 1, filterPanel.Width, filterPanel.Height - 1);

            // ── RIGHT: Live badge — always docked right ─────────────────
            Panel pnlLiveStatus = new Panel
            {
                Dock      = DockStyle.Right,
                Width     = 195,
                BackColor = Color.FromArgb(240, 253, 244)
            };
            pnlLiveStatus.Paint += (s, e) =>
            {
                using (Pen p = new Pen(Color.FromArgb(187, 247, 208), 1))
                    e.Graphics.DrawRectangle(p, 4, 24, pnlLiveStatus.Width - 10, 34);
            };
            Label lblLiveIcon = new Label
            {
                Text      = "⚡ Live Auto-Synced",
                Font      = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(22, 101, 52),
                Location  = new Point(4, 24),
                Size      = new Size(185, 34),
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlLiveStatus.Controls.Add(lblLiveIcon);
            filterPanel.Controls.Add(pnlLiveStatus);  // add RIGHT panel FIRST

            // ── LEFT+FILL: All filter controls ──────────────────────────
            Panel filterLeft = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            // Row 1 labels (y=8)
            Label lblPresets = new Label { Text = "Quick Presets (Max 3 Days):", Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), ForeColor = Color.FromArgb(51, 65, 85), Location = new Point(18, 8), AutoSize = true };
            Label lblFrom    = new Label { Text = "From Date:",                  Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), ForeColor = Color.FromArgb(51, 65, 85), Location = new Point(495, 8), AutoSize = true };
            Label lblTo      = new Label { Text = "To Date (Max 3 Days):",       Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), ForeColor = Color.FromArgb(51, 65, 85), Location = new Point(645, 8), AutoSize = true };
            Label lblScope   = new Label { Text = "Register Scope:",             Font = new Font("Segoe UI", 9.5F, FontStyle.Bold), ForeColor = Color.FromArgb(51, 65, 85), Location = new Point(795, 8), AutoSize = true };
            filterLeft.Controls.AddRange(new Control[] { lblPresets, lblFrom, lblTo, lblScope });

            // Row 2 controls (y=32)
            btnPresetToday          = CreatePresetButton("📅 Today",             18,  32, 110, () => ApplyPreset(1));
            btnPresetYesterdayToday = CreatePresetButton("📅 Yesterday & Today", 136, 32, 185, () => ApplyPreset(2));
            btnPreset3Days          = CreatePresetButton("📅 Last 3 Days",       329, 32, 145, () => ApplyPreset(3));
            filterLeft.Controls.AddRange(new Control[] { btnPresetToday, btnPresetYesterdayToday, btnPreset3Days });

            dtpFromDate = new DateTimePicker { Format = DateTimePickerFormat.Short, Font = new Font("Segoe UI", 10F, FontStyle.Bold), Size = new Size(135, 32), Location = new Point(495, 33), Value = DateTime.Today };
            dtpFromDate.ValueChanged += DtpDate_ValueChanged;
            filterLeft.Controls.Add(dtpFromDate);

            dtpToDate = new DateTimePicker { Format = DateTimePickerFormat.Short, Font = new Font("Segoe UI", 10F, FontStyle.Bold), Size = new Size(135, 32), Location = new Point(645, 33), Value = DateTime.Today };
            dtpToDate.ValueChanged += DtpDate_ValueChanged;
            filterLeft.Controls.Add(dtpToDate);

            chkRegisterSelector = new CheckedRegisterSelector
            {
                Location = new Point(795, 32),
                Size     = new Size(360, 34)
            };
            chkRegisterSelector.InitRegisters(registerRegistry);
            chkRegisterSelector.SelectionChanged += (s, e) => { if (!isInitializing) FetchConsolidatedRecords(); };
            filterLeft.Controls.Add(chkRegisterSelector);
            filterPanel.Controls.Add(filterLeft);   // FILL panel MUST be added AFTER RIGHT panel


            // ── 3. ACTION BAR (Stats Badges | Search | Export Buttons) ─────────
            actionBar = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 74,
                BackColor = Color.FromArgb(248, 250, 252)
            };
            actionBar.Paint += (s, e) =>
            {
                e.Graphics.DrawLine(new Pen(Color.FromArgb(203, 213, 225)), 0, actionBar.Height - 1, actionBar.Width, actionBar.Height - 1);
                e.Graphics.DrawLine(new Pen(Color.FromArgb(226, 232, 240)), 0, 0, actionBar.Width, 0);
            };

            // ── RIGHT PANEL: Export buttons, anchored to the right ────────
            Panel rightPanel = new Panel
            {
                Dock      = DockStyle.Right,
                Width     = 550,
                BackColor = Color.Transparent
            };

            btnExportPDF   = new ClassGlassButton("📥  PDF",   Color.FromArgb(220, 38, 38), 8,   15, 122, 44, () => ExportToPDF());
            btnExportExcel = new ClassGlassButton("📊  Excel", Color.FromArgb(22, 163, 74), 142, 15, 126, 44, () => ExportToExcel());
            btnExportCSV   = new ClassGlassButton("📄  CSV",   Color.FromArgb(2, 132, 199), 280, 15, 122, 44, () => ExportToCSV());
            btnPrint       = new ClassGlassButton("🖨  Print", Color.FromArgb(71, 85, 105), 414, 15, 122, 44, () => PrintReport());

            rightPanel.Controls.AddRange(new Control[] { btnExportPDF, btnExportExcel, btnExportCSV, btnPrint });
            actionBar.Controls.Add(rightPanel);

            // ── LEFT PANEL: Badges + Search, fills remaining left space ───
            Panel leftPanel = new Panel
            {
                Dock      = DockStyle.Fill,
                BackColor = Color.Transparent
            };

            // Badges (generous widths & padding: words are never cramped / clammy)
            Panel bRecords   = CreateBadge("Records:",     "0",              Color.FromArgb(37, 99, 235),  16,  17, 130, out lblTotalRecords);
            Panel bRegisters = CreateBadge("Active Regs:", "0 / 40",         Color.FromArgb(16, 185, 129), 158, 17, 175, out lblActiveRegisters);
            Panel bSpan      = CreateBadge("Date Span:",   "Today (1 Day)",  Color.FromArgb(109, 40, 217), 345, 17, 310, out lblDateRangeInfo);
            leftPanel.Controls.AddRange(new Control[] { bRecords, bRegisters, bSpan });

            // Search label
            Label lblSearch = new Label
            {
                Text      = "🔍 Search:",
                Font      = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 61, 119),
                Location  = new Point(675, 26),
                AutoSize  = true
            };
            leftPanel.Controls.Add(lblSearch);

            // Search box
            txtSearch = new TextBox
            {
                Font        = new Font("Segoe UI", 10.5F),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor   = Color.White,
                Size        = new Size(200, 30),
                Location    = new Point(765, 22)
            };
            txtSearch.TextChanged += TxtSearch_TextChanged;
            txtSearch.HandleCreated += (s, e) => SendMessage(txtSearch.Handle, 0x1501, 1, "Filter results...");
            leftPanel.Controls.Add(txtSearch);

            actionBar.Controls.Add(leftPanel);

            // ── 4. MAIN DATA GRID PANEL (Touchpad Horizontal Scrolling Supported) ─
            gridPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(241, 245, 249),
                Padding = new Padding(20, 12, 20, 14)
            };

            dgvResults = new TouchpadScrollableDataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = Color.FromArgb(226, 232, 240),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false,
                ScrollBars = ScrollBars.Both, // Enable horizontal & vertical scrolling
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                RowTemplate = { Height = 44 }
            };

            StyleGrid();
            gridPanel.Controls.Add(dgvResults);

            // ── 5. DOCKING ASSEMBLY IN EXACT WINFORMS ORDER ───────────────────
            this.Controls.Clear();
            this.Controls.Add(gridPanel);     // Fill space below toolbars
            this.Controls.Add(actionBar);     // Top position 3
            this.Controls.Add(filterPanel);   // Top position 2
            this.Controls.Add(titleBar);      // Top position 1 (very top)
        }

        private Button CreatePresetButton(string text, int x, int y, int width, Action action)
        {
            Button btn = new Button
            {
                Text = text,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                Size = new Size(width, 34),
                Location = new Point(x, y),
                BackColor = Color.FromArgb(238, 242, 255),
                ForeColor = Color.FromArgb(33, 61, 119),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderColor = Color.FromArgb(199, 210, 254);
            btn.FlatAppearance.BorderSize = 1;
            btn.Click += (s, e) => action();
            return btn;
        }

        private void HighlightPresetButton(Button activeBtn)
        {
            Button[] allPresets = { btnPresetToday, btnPresetYesterdayToday, btnPreset3Days };
            foreach (var b in allPresets)
            {
                if (b == null) continue;
                if (b == activeBtn)
                {
                    b.BackColor = Color.FromArgb(37, 99, 235);
                    b.ForeColor = Color.White;
                    b.FlatAppearance.BorderColor = Color.FromArgb(29, 78, 216);
                }
                else
                {
                    b.BackColor = Color.FromArgb(238, 242, 255);
                    b.ForeColor = Color.FromArgb(33, 61, 119);
                    b.FlatAppearance.BorderColor = Color.FromArgb(199, 210, 254);
                }
            }
        }

        private Panel CreateBadge(string labelText, string initialVal, Color accentColor, int x, int y, int width, out Label valLabel)
        {
            Panel p = new Panel
            {
                Size = new Size(width, 40),
                Location = new Point(x, y),
                BackColor = Color.FromArgb(248, 250, 252)
            };
            p.Paint += (s, e) =>
            {
                using (SolidBrush b = new SolidBrush(accentColor))
                    e.Graphics.FillRectangle(b, 0, 0, 4, p.Height);
                using (Pen pen = new Pen(Color.FromArgb(203, 213, 225)))
                    e.Graphics.DrawRectangle(pen, 0, 0, p.Width - 1, p.Height - 1);
            };

            Label lblTitle = new Label
            {
                Text = labelText,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(10, 10),
                AutoSize = true
            };
            p.Controls.Add(lblTitle);

            Label targetVal = new Label
            {
                Text = initialVal,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(lblTitle.PreferredWidth + 14, 9),
                AutoSize = true
            };
            valLabel = targetVal;
            p.Controls.Add(targetVal);

            targetVal.TextChanged += (s, e) =>
            {
                int neededW = targetVal.Right + 14;
                if (neededW > p.Width) p.Width = neededW;
            };

            return p;
        }

        private void StyleGrid()
        {
            dgvResults.EnableHeadersVisualStyles = false;
            dgvResults.ColumnHeadersVisible = true;
            dgvResults.ColumnHeadersHeight = 48;
            dgvResults.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvResults.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(33, 61, 119),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleLeft,
                Padding = new Padding(8, 0, 8, 0)
            };

            dgvResults.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.White,
                ForeColor = Color.FromArgb(15, 23, 42),
                SelectionBackColor = Color.FromArgb(199, 220, 255),
                SelectionForeColor = Color.FromArgb(15, 23, 42),
                Font = new Font("Segoe UI", 11F),
                Padding = new Padding(6, 4, 6, 4)
            };

            dgvResults.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(248, 250, 252),
                ForeColor = Color.FromArgb(15, 23, 42),
                SelectionBackColor = Color.FromArgb(199, 220, 255),
                SelectionForeColor = Color.FromArgb(15, 23, 42),
                Font = new Font("Segoe UI", 11F),
                Padding = new Padding(6, 4, 6, 4)
            };

            // Custom header painting with NoPrefix flag to prevent '&' being drawn as underscore
            dgvResults.CellPainting += (s, e) =>
            {
                // 1. Column Header (e.RowIndex == -1)
                if (e.RowIndex == -1 && e.ColumnIndex >= 0)
                {
                    e.PaintBackground(e.CellBounds, true);
                    using (Brush b = new SolidBrush(Color.FromArgb(33, 61, 119)))
                        e.Graphics.FillRectangle(b, e.CellBounds);
                    using (Pen p = new Pen(Color.FromArgb(51, 65, 85)))
                        e.Graphics.DrawRectangle(p, e.CellBounds.X, e.CellBounds.Y, e.CellBounds.Width - 1, e.CellBounds.Height - 1);

                    string header = dgvResults.Columns[e.ColumnIndex].HeaderText;
                    TextFormatFlags flags = TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.NoPrefix;
                    if (e.ColumnIndex == 0) flags = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.NoPrefix;
                    Rectangle rect = new Rectangle(e.CellBounds.X + 8, e.CellBounds.Y, e.CellBounds.Width - 14, e.CellBounds.Height);
                    using (Font headerFont = new Font("Segoe UI", 11F, FontStyle.Bold))
                    {
                        TextRenderer.DrawText(e.Graphics, header, headerFont, rect, Color.White, flags);
                    }
                    e.Handled = true;
                }
                // 2. S.No Cell (e.ColumnIndex == 0)
                else if (e.RowIndex >= 0 && e.ColumnIndex == 0)
                {
                    e.PaintBackground(e.CellBounds, true);
                    Color bgColor = (dgvResults.Rows[e.RowIndex].Selected) ? Color.FromArgb(37, 99, 235) : Color.FromArgb(241, 245, 249);
                    Color textColor = (dgvResults.Rows[e.RowIndex].Selected) ? Color.White : Color.FromArgb(33, 61, 119);

                    using (Brush backBrush = new SolidBrush(bgColor))
                    {
                        e.Graphics.FillRectangle(backBrush, e.CellBounds);
                    }
                    using (Pen borderPen = new Pen(Color.FromArgb(226, 232, 240)))
                    {
                        e.Graphics.DrawLine(borderPen, e.CellBounds.Left, e.CellBounds.Bottom - 1, e.CellBounds.Right, e.CellBounds.Bottom - 1);
                    }
                    string val = e.Value?.ToString() ?? "";
                    TextFormatFlags flags = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.NoPrefix;
                    TextRenderer.DrawText(e.Graphics, val, new Font("Segoe UI", 10.5F, FontStyle.Bold), e.CellBounds, textColor, flags);
                    e.Handled = true;
                }
            };

            // Double Click to open full details
            dgvResults.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex < 0) return;
                ShowRowDetails(e.RowIndex);
            };
        }

        // ─────────────────────────────────────────────────────────────────────
        // 3. DATE RESTRICTION & PRESET MANAGEMENT (STRICT MAX 3 DAYS & AUTO-FETCH)
        // ─────────────────────────────────────────────────────────────────────
        private void ApplyPreset(int days)
        {
            if (days > 3) days = 3;
            if (days < 1) days = 1;

            if (days == 1) HighlightPresetButton(btnPresetToday);
            else if (days == 2) HighlightPresetButton(btnPresetYesterdayToday);
            else if (days == 3) HighlightPresetButton(btnPreset3Days);

            DateTime end = DateTime.Today;
            DateTime start = DateTime.Today.AddDays(-(days - 1));

            isInitializing = true;
            dtpFromDate.Value = start;
            dtpToDate.Value = end;
            isInitializing = false;

            FetchConsolidatedRecords();
        }

        private void DtpDate_ValueChanged(object sender, EventArgs e)
        {
            if (isInitializing) return;

            DateTime from = dtpFromDate.Value.Date;
            DateTime to = dtpToDate.Value.Date;

            isInitializing = true;
            try
            {
                if (to < from)
                {
                    to = from;
                    dtpToDate.Value = to;
                }

                // Strictly enforce maximum of 3 days
                int diffDays = (int)(to - from).TotalDays + 1;
                if (diffDays > 3)
                {
                    to = from.AddDays(2);
                    dtpToDate.Value = to;
                }
            }
            finally
            {
                isInitializing = false;
            }

            HighlightPresetButton(null);

            // Auto-fetch data immediately
            FetchConsolidatedRecords();
        }

        // ─────────────────────────────────────────────────────────────────────
        // 4. CROSS-REGISTER SYSTEMATIC CONSOLIDATED DATA QUERY ENGINE
        // ─────────────────────────────────────────────────────────────────────
        private void FetchConsolidatedRecords()
        {
            DateTime fromDate = dtpFromDate.Value.Date;
            DateTime toDate = dtpToDate.Value.Date;

            // Ensure strict maximum of 3 days (Day 1, Day 2, Day 3)
            if ((toDate - fromDate).TotalDays > 2)
            {
                toDate = fromDate.AddDays(2);
            }
            DateTime toDateEndOfDay = toDate.AddDays(1).AddTicks(-1); // End of day 23:59:59.999

            dtConsolidated = CreateConsolidatedTableStructure();
            HashSet<string> activeRegs = new HashSet<string>();

            HashSet<string> selectedCodes = chkRegisterSelector?.GetSelectedCodes() ?? new HashSet<string>();

            if (selectedCodes.Count == 0)
            {
                dtConsolidated.Clear();
                dgvResults.DataSource = dtConsolidated;
                lblTotalRecords.Text = "0";
                lblActiveRegisters.Text = "0 / 0 Selected";
                lblDateRangeInfo.Text = "No Registers Selected";
                return;
            }

            int serialNo = 1;

            foreach (var kvp in registerRegistry)
            {
                string code = kvp.Key;
                RegisterMeta meta = kvp.Value;

                if (!selectedCodes.Contains(code))
                    continue;

                try
                {
                    DataTable rawTable = QueryRegisterRecords(meta, fromDate, toDateEndOfDay);
                    if (rawTable != null && rawTable.Rows.Count > 0)
                    {
                        activeRegs.Add(code);
                        foreach (DataRow r in rawTable.Rows)
                        {
                            DataRow consRow = dtConsolidated.NewRow();
                            consRow["S.No"] = serialNo++;
                            consRow["RegisterCode"] = $"REG-{meta.Code}";
                            consRow["RegisterName"] = meta.Name;

                            string recordId = CleanValue(ExtractColumnValue(r, meta.IdColumn), $"REC-{serialNo}");
                            consRow["RecordID"] = recordId;

                            // Category
                            string cat = CleanValue(ExtractColumnValue(r, meta.CategoryColumn), meta.Name);
                            consRow["Category"] = cat;

                            // Description & Remarks
                            string desc = CleanValue(ExtractColumnValue(r, meta.DescriptionColumn), "-");
                            consRow["Description"] = desc;

                            // Asset / Train / Point / Line Number
                            string asset = CleanValue(ExtractColumnValue(r, meta.AssetColumn), "-");
                            consRow["AssetInfo"] = asset;

                            // Status / Action
                            string status = CleanValue(ExtractColumnValue(r, meta.StatusColumn), "-");
                            consRow["Status"] = status;

                            // Reported By / Staff Name
                            string reportedBy = CleanValue(ExtractColumnValue(r, meta.ReportedByColumn), "-");
                            consRow["ReportedBy"] = reportedBy;

                            // Submitted By (Staff ID)
                            string staff = CleanValue(ExtractColumnValue(r, meta.StaffColumn) ?? ExtractColumnValue(r, "SubmittedBy"), "-");
                            consRow["SubmittedBy"] = staff;

                            // Submission Date & Time
                            DateTime recordTime = ExtractDateTime(r, meta.DateColumn);
                            consRow["SubmittedTime"] = recordTime != DateTime.MinValue ? (object)recordTime : (r.Table.Columns.Contains("SubmittedAt") && r["SubmittedAt"] != DBNull.Value ? r["SubmittedAt"] : DateTime.Now);

                            // Store raw metadata JSON/key-values for complete detail inspection
                            consRow["RawDetails"] = BuildFullDetailDump(r);
                            consRow["TableName"] = meta.TableName;

                            dtConsolidated.Rows.Add(consRow);
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error querying {meta.TableName}: {ex.Message}");
                }
            }

            BindData(dtConsolidated);

            // Update Metrics
            lblTotalRecords.Text = dtConsolidated.Rows.Count.ToString("N0");
            lblActiveRegisters.Text = $"{activeRegs.Count} / {selectedCodes.Count} Selected";

            int spanDays = (int)(toDate.Date - fromDate.Date).TotalDays + 1;
            if (fromDate.Date == toDate.Date)
            {
                lblDateRangeInfo.Text = $"{fromDate:dd-MMM-yyyy} (1 Day)";
            }
            else
            {
                lblDateRangeInfo.Text = $"{fromDate:dd-MMM} to {toDate:dd-MMM} ({spanDays} Days)";
            }
        }

        private DateTime GetLatestRecordDate()
        {
            try
            {
                string q = @"
                    SELECT MAX(LatestDate) FROM (
                        SELECT MAX(SubmittedAt) AS LatestDate FROM Reg001_StationDiary
                        UNION ALL SELECT MAX(EventTime) FROM Reg001_StationDiary
                        UNION ALL SELECT MAX(SubmittedAt) FROM Reg002_TrainSignal
                        UNION ALL SELECT MAX(SubmittedAt) FROM Reg005_Failure
                        UNION ALL SELECT MAX(SubmittedAt) FROM Reg007_Attendance
                        UNION ALL SELECT MAX(SubmittedAt) FROM Reg011_PublicComplaint
                    ) t WHERE LatestDate IS NOT NULL";
                object res = db.ExecuteScalar(q);
                if (res != null && res != DBNull.Value && DateTime.TryParse(res.ToString(), out DateTime dt))
                {
                    return dt.Date;
                }
            }
            catch { }
            return DateTime.Today;
        }

        private string CleanValue(string val, string fallback)
        {
            if (string.IsNullOrWhiteSpace(val) || val.Trim().Equals("N/A", StringComparison.OrdinalIgnoreCase) || val.Trim().Equals("(None)", StringComparison.OrdinalIgnoreCase))
                return fallback;
            return val.Trim();
        }

        private DataTable CreateConsolidatedTableStructure()
        {
            DataTable dt = new DataTable("ConsolidatedReports");
            dt.Columns.Add("S.No", typeof(int));
            dt.Columns.Add("RegisterCode", typeof(string));
            dt.Columns.Add("RegisterName", typeof(string));
            dt.Columns.Add("RecordID", typeof(string));
            dt.Columns.Add("Category", typeof(string));
            dt.Columns.Add("Description", typeof(string));
            dt.Columns.Add("AssetInfo", typeof(string));
            dt.Columns.Add("Status", typeof(string));
            dt.Columns.Add("ReportedBy", typeof(string));
            dt.Columns.Add("SubmittedBy", typeof(string));
            dt.Columns.Add("SubmittedTime", typeof(DateTime));
            dt.Columns.Add("RawDetails", typeof(string));
            dt.Columns.Add("TableName", typeof(string));
            return dt;
        }

        private DataTable QueryRegisterRecords(RegisterMeta meta, DateTime fromDate, DateTime toDate)
        {
            string fromStr = fromDate.ToString("yyyy-MM-dd");
            string toStr = toDate.ToString("yyyy-MM-dd");

            // Query using TRY_CONVERT(date, ...) to eliminate timestamp formatting mismatch
            string query = $@"
                IF OBJECT_ID('{meta.TableName}', 'U') IS NOT NULL
                BEGIN
                    IF COL_LENGTH('{meta.TableName}', 'SubmittedAt') IS NOT NULL AND COL_LENGTH('{meta.TableName}', '{meta.DateColumn}') IS NOT NULL
                    BEGIN
                        SELECT * FROM {meta.TableName} 
                        WHERE (TRY_CONVERT(date, SubmittedAt) BETWEEN '{fromStr}' AND '{toStr}')
                           OR (TRY_CONVERT(date, {meta.DateColumn}) BETWEEN '{fromStr}' AND '{toStr}')
                        ORDER BY SubmittedAt DESC
                    END
                    ELSE IF COL_LENGTH('{meta.TableName}', 'SubmittedAt') IS NOT NULL
                    BEGIN
                        SELECT * FROM {meta.TableName} 
                        WHERE TRY_CONVERT(date, SubmittedAt) BETWEEN '{fromStr}' AND '{toStr}'
                        ORDER BY SubmittedAt DESC
                    END
                    ELSE IF COL_LENGTH('{meta.TableName}', '{meta.DateColumn}') IS NOT NULL
                    BEGIN
                        SELECT * FROM {meta.TableName} 
                        WHERE TRY_CONVERT(date, {meta.DateColumn}) BETWEEN '{fromStr}' AND '{toStr}'
                        ORDER BY {meta.DateColumn} DESC
                    END
                    ELSE
                    BEGIN
                        SELECT TOP 0 * FROM {meta.TableName}
                    END
                END
                ELSE
                BEGIN
                    SELECT 1 WHERE 1=0
                END";

            return db.ExecuteQuery(query);
        }

        private string ExtractColumnValue(DataRow row, string colName)
        {
            if (string.IsNullOrEmpty(colName) || !row.Table.Columns.Contains(colName) || row[colName] == DBNull.Value)
                return null;
            return row[colName].ToString();
        }

        private DateTime ExtractDateTime(DataRow row, string colName)
        {
            if (string.IsNullOrEmpty(colName) || !row.Table.Columns.Contains(colName) || row[colName] == DBNull.Value)
                return DateTime.MinValue;

            if (row[colName] is DateTime dt) return dt;
            if (DateTime.TryParse(row[colName].ToString(), out DateTime parsed)) return parsed;
            return DateTime.MinValue;
        }

        private string BuildFullDetailDump(DataRow row)
        {
            StringBuilder sb = new StringBuilder();
            foreach (DataColumn col in row.Table.Columns)
            {
                string val = (row[col] == DBNull.Value || string.IsNullOrWhiteSpace(row[col].ToString())) ? "-" : row[col].ToString();
                sb.AppendLine($"{col.ColumnName,-22}: {val}");
            }
            return sb.ToString();
        }

        private void BindData(DataTable dt)
        {
            dgvResults.DataSource = null;
            dgvResults.DataSource = dt;

            if (dgvResults.Columns.Count > 0)
            {
                // Fixed generous column widths enable smooth left/right horizontal scrolling with zero truncation!
                dgvResults.Columns["S.No"].Width = 65;
                dgvResults.Columns["S.No"].HeaderText = "S.No";
                dgvResults.Columns["S.No"].Frozen = true;

                dgvResults.Columns["RegisterCode"].Width = 115;
                dgvResults.Columns["RegisterCode"].HeaderText = "Reg Code";

                dgvResults.Columns["RegisterName"].Width = 240;
                dgvResults.Columns["RegisterName"].HeaderText = "Register Name";

                dgvResults.Columns["RecordID"].Width = 270;
                dgvResults.Columns["RecordID"].HeaderText = "Record ID";

                dgvResults.Columns["Category"].Width = 190;
                dgvResults.Columns["Category"].HeaderText = "Category / Type";

                dgvResults.Columns["Description"].Width = 320;
                dgvResults.Columns["Description"].HeaderText = "Description & Details";

                dgvResults.Columns["AssetInfo"].Width = 210;
                dgvResults.Columns["AssetInfo"].HeaderText = "Asset / Train / Point";

                dgvResults.Columns["Status"].Width = 195;
                dgvResults.Columns["Status"].HeaderText = "Status / Remarks";

                dgvResults.Columns["ReportedBy"].Width = 175;
                dgvResults.Columns["ReportedBy"].HeaderText = "Reported By";

                dgvResults.Columns["SubmittedBy"].Width = 125;
                dgvResults.Columns["SubmittedBy"].HeaderText = "Staff ID";

                dgvResults.Columns["SubmittedTime"].Width = 240;
                dgvResults.Columns["SubmittedTime"].HeaderText = "Submission Date & Time";
                dgvResults.Columns["SubmittedTime"].DefaultCellStyle.Format = "dd-MMM-yyyy hh:mm:ss tt";

                // Hide internal payload columns
                if (dgvResults.Columns.Contains("RawDetails")) dgvResults.Columns["RawDetails"].Visible = false;
                if (dgvResults.Columns.Contains("TableName")) dgvResults.Columns["TableName"].Visible = false;
            }
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            if (dtConsolidated == null) return;
            string filter = txtSearch.Text.Trim().Replace("'", "''");
            if (string.IsNullOrEmpty(filter))
            {
                dtConsolidated.DefaultView.RowFilter = "";
            }
            else
            {
                dtConsolidated.DefaultView.RowFilter =
                    $"RegisterCode LIKE '%{filter}%' OR RegisterName LIKE '%{filter}%' OR RecordID LIKE '%{filter}%' OR Category LIKE '%{filter}%' OR Description LIKE '%{filter}%' OR AssetInfo LIKE '%{filter}%' OR Status LIKE '%{filter}%' OR ReportedBy LIKE '%{filter}%' OR SubmittedBy LIKE '%{filter}%'";
            }
            lblTotalRecords.Text = dtConsolidated.DefaultView.Count.ToString("N0");
        }

        private void ShowRowDetails(int rowIndex)
        {
            DataGridViewRow row = dgvResults.Rows[rowIndex];
            string regName = row.Cells["RegisterName"].Value?.ToString() ?? "Register";
            string recId = row.Cells["RecordID"].Value?.ToString() ?? "";
            string rawDump = row.Cells["RawDetails"].Value?.ToString() ?? "";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("=================================================");
            sb.AppendLine($"  {regName.ToUpper()} - COMPLETE RECORD DETAILS");
            sb.AppendLine($"  Record ID: {recId}");
            sb.AppendLine("=================================================\n");
            sb.AppendLine(rawDump);

            MessageBox.Show(sb.ToString(), $"Record Detail - {recId}", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ─────────────────────────────────────────────────────────────────────
        // 5. EXPORT ENGINE: MULTI-PAGE PDF GENERATOR (.PDF) (ALL 11 COLUMNS & NO OVERLAP)
        // ─────────────────────────────────────────────────────────────────────
        private void ExportToPDF()
        {
            if (dgvResults.Rows.Count == 0)
            {
                MessageBox.Show("No records available to export.", "Export PDF", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "PDF Document (*.pdf)|*.pdf",
                FileName = $"TMS_DynamicReports_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    GenerateMultiPagePdf(sfd.FileName);
                    MessageBox.Show($"Complete Report with ALL {dgvResults.Rows.Count} records successfully exported to PDF!\n\nLocation: {sfd.FileName}", "PDF Export Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error generating PDF: {ex.Message}", "PDF Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void GenerateMultiPagePdf(string filePath)
        {
            // Landscape A4 Canvas: Width=842, Height=595
            int pageWidth = 842;
            int pageHeight = 595;
            int rowsPerPage = 20; // 20 clean rows per page
            int totalRows = dgvResults.Rows.Count;
            int totalPages = (int)Math.Ceiling((double)totalRows / rowsPerPage);
            if (totalPages < 1) totalPages = 1;

            string dateRangeStr = $"{dtpFromDate.Value:dd-MMM-yyyy} to {dtpToDate.Value:dd-MMM-yyyy}";
            string genDate = DateTime.Now.ToString("dd-MMM-yyyy hh:mm tt");
            string scopeStr = chkRegisterSelector?.GetScopeSummaryText() ?? "ALL REGISTERS (001 - 040 CONSOLIDATED)";

            using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs, Encoding.ASCII))
            {
                List<string> pageContents = new List<string>();

                for (int page = 1; page <= totalPages; page++)
                {
                    StringBuilder sb = new StringBuilder();

                    // 1. Top Header Banner (Navy Background #213D77)
                    sb.AppendLine("0.129 0.239 0.467 rg"); // #213D77 Fill
                    sb.AppendLine($"15 525 {pageWidth - 30} 55 re f");

                    // Title Text - Left Side
                    sb.AppendLine("BT");
                    sb.AppendLine("/F1 12.5 Tf");
                    sb.AppendLine("1 1 1 rg"); // White text
                    sb.AppendLine("28 560 Td");
                    sb.AppendLine("(INDIAN RAILWAYS - TRAIN MANAGEMENT SYSTEM (TMS)) Tj");
                    sb.AppendLine("0 -15 Td");
                    sb.AppendLine("/F2 8.5 Tf");
                    sb.AppendLine($"({EscapePdfText($"CONSOLIDATED DYNAMIC REPORT (REG-041)  |  Date Range: {dateRangeStr}  |  Scope: {Truncate(scopeStr, 40)}")}) Tj");
                    sb.AppendLine("ET");

                    // Meta Info - Right Side (Completely separated, no overlap!)
                    sb.AppendLine("BT");
                    sb.AppendLine("/F2 8.5 Tf");
                    sb.AppendLine("1 1 1 rg");
                    sb.AppendLine($"{pageWidth - 190} 560 Td");
                    sb.AppendLine($"({EscapePdfText($"Page {page} of {totalPages} | Total: {totalRows} Records")}) Tj");
                    sb.AppendLine("0 -15 Td");
                    sb.AppendLine($"({EscapePdfText($"Generated: {genDate}")}) Tj");
                    sb.AppendLine("ET");

                    // 2. Table Column Headers
                    int tableTop = 505;
                    sb.AppendLine("0.2 0.25 0.35 rg");
                    sb.AppendLine($"15 {tableTop} {pageWidth - 30} 18 re f");

                    // 11 Clean Columns across 812 pt table width:
                    // S.No(24), Code(42), RegName(115), RecordID(120), Cat(74), Desc(110), Asset(68), Status(58), Reported(66), Staff(42), Date(75)
                    int[] colX = { 18, 44, 88, 205, 328, 404, 516, 586, 646, 714, 758 };
                    string[] colHead = { "S.No", "Code", "Register Name", "Record ID", "Category", "Description", "Asset/Point", "Status", "Reported By", "Staff", "Date & Time" };

                    sb.AppendLine("BT");
                    sb.AppendLine("/F1 7.5 Tf");
                    sb.AppendLine("1 1 1 rg");
                    for (int c = 0; c < colHead.Length; c++)
                    {
                        sb.AppendLine($"1 0 0 1 {colX[c]} {tableTop + 5} Tm");
                        sb.AppendLine($"({EscapePdfText(colHead[c])}) Tj");
                    }
                    sb.AppendLine("ET");

                    // 3. Table Rows
                    int startIdx = (page - 1) * rowsPerPage;
                    int endIdx = Math.Min(startIdx + rowsPerPage, totalRows);
                    int rowY = tableTop - 19;

                    for (int r = startIdx; r < endIdx; r++)
                    {
                        DataGridViewRow dRow = dgvResults.Rows[r];

                        // Alternating Row Fill
                        if ((r - startIdx) % 2 == 1)
                        {
                            sb.AppendLine("0.96 0.97 0.98 rg");
                            sb.AppendLine($"15 {rowY - 1} {pageWidth - 30} 18 re f");
                        }

                        // Bottom border
                        sb.AppendLine("0.88 0.91 0.94 RG");
                        sb.AppendLine("0.5 w");
                        sb.AppendLine($"15 {rowY - 1} m {pageWidth - 15} {rowY - 1} l S");

                        string sno = (r + 1).ToString();
                        string code = CleanPdf(dRow.Cells["RegisterCode"].Value);
                        string name = Truncate(CleanPdf(dRow.Cells["RegisterName"].Value), 22);
                        string recId = Truncate(CleanPdf(dRow.Cells["RecordID"].Value), 23);
                        string cat = Truncate(CleanPdf(dRow.Cells["Category"].Value), 14);
                        string desc = Truncate(CleanPdf(dRow.Cells["Description"].Value), 20);
                        string asset = Truncate(CleanPdf(dRow.Cells["AssetInfo"].Value), 12);
                        string status = Truncate(CleanPdf(dRow.Cells["Status"].Value), 11);
                        string repBy = Truncate(CleanPdf(dRow.Cells["ReportedBy"].Value), 12);
                        string staff = CleanPdf(dRow.Cells["SubmittedBy"].Value);
                        string time = dRow.Cells["SubmittedTime"].Value is DateTime dt ? dt.ToString("dd-MM-yy HH:mm") : "-";

                        string[] rowVals = { sno, code, name, recId, cat, desc, asset, status, repBy, staff, time };

                        sb.AppendLine("BT");
                        sb.AppendLine("/F2 7 Tf");
                        sb.AppendLine("0.05 0.1 0.15 rg");
                        for (int c = 0; c < rowVals.Length; c++)
                        {
                            sb.AppendLine($"1 0 0 1 {colX[c]} {rowY + 4} Tm");
                            sb.AppendLine($"({EscapePdfText(rowVals[c])}) Tj");
                        }
                        sb.AppendLine("ET");

                        rowY -= 19;
                    }

                    // 4. Footer Note
                    sb.AppendLine("BT");
                    sb.AppendLine("/F2 7 Tf");
                    sb.AppendLine("0.4 0.45 0.5 rg");
                    sb.AppendLine($"15 18 Td");
                    sb.AppendLine("(Official Train Management System Report - Indian Railways. All register parameters verified.) Tj");
                    sb.AppendLine("ET");

                    pageContents.Add(sb.ToString());
                }

                // Assemble complete PDF structure
                List<string> pdfObjects = new List<string>();
                int pageCount = pageContents.Count;

                // Obj 1: Catalog
                pdfObjects.Add("<< /Type /Catalog /Pages 2 0 R >>");

                // Obj 2: Pages Collection
                StringBuilder kidsSb = new StringBuilder();
                for (int i = 0; i < pageCount; i++)
                {
                    int pageObjNum = 3 + i * 2;
                    kidsSb.Append($"{pageObjNum} 0 R ");
                }
                pdfObjects.Add($"<< /Type /Pages /Kids [{kidsSb}] /Count {pageCount} >>");

                // Page Objects and Content Streams
                int fontBoldObj = 3 + pageCount * 2;
                int fontNormObj = 4 + pageCount * 2;

                for (int i = 0; i < pageCount; i++)
                {
                    int contentObjNum = 4 + i * 2;
                    pdfObjects.Add($"<< /Type /Page /Parent 2 0 R /MediaBox [0 0 {pageWidth} {pageHeight}] /Contents {contentObjNum} 0 R /Resources << /Font << /F1 {fontBoldObj} 0 R /F2 {fontNormObj} 0 R >> >> >>");

                    string contentStr = pageContents[i];
                    byte[] contentBytes = Encoding.ASCII.GetBytes(contentStr);
                    pdfObjects.Add($"<< /Length {contentBytes.Length} >>\nstream\n{contentStr}\nendstream");
                }

                // Font Objects
                pdfObjects.Add("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold >>");
                pdfObjects.Add("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>");

                // Write PDF Header
                sw.WriteLine("%PDF-1.4");
                List<long> offsets = new List<long>();

                for (int i = 0; i < pdfObjects.Count; i++)
                {
                    sw.Flush();
                    offsets.Add(fs.Position);
                    sw.WriteLine($"{i + 1} 0 obj");
                    sw.WriteLine(pdfObjects[i]);
                    sw.WriteLine("endobj");
                }

                sw.Flush();
                long xrefPos = fs.Position;
                sw.WriteLine("xref");
                sw.WriteLine($"0 {pdfObjects.Count + 1}");
                sw.WriteLine("0000000000 65535 f ");
                foreach (long off in offsets)
                {
                    sw.WriteLine($"{off:D10} 00000 n ");
                }
                sw.WriteLine("trailer");
                sw.WriteLine($"<< /Size {pdfObjects.Count + 1} /Root 1 0 R >>");
                sw.WriteLine("startxref");
                sw.WriteLine(xrefPos);
                sw.WriteLine("%%EOF");
            }
        }

        private string CleanPdf(object val)
        {
            if (val == null || val == DBNull.Value || string.IsNullOrWhiteSpace(val.ToString()) || val.ToString().Trim().Equals("N/A", StringComparison.OrdinalIgnoreCase))
                return "-";
            return val.ToString().Trim();
        }

        private string EscapePdfText(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            return s.Replace("\\", "\\\\").Replace("(", "\\(").Replace(")", "\\)");
        }

        private string Truncate(string s, int maxLen)
        {
            if (string.IsNullOrEmpty(s)) return "-";
            return s.Length <= maxLen ? s : s.Substring(0, maxLen - 1) + ".";
        }

        // ─────────────────────────────────────────────────────────────────────
        // 6. EXPORT ENGINE: EXCEL FORMAT (.XLS / XML SPREADSHEET)
        // ─────────────────────────────────────────────────────────────────────
        private void ExportToExcel()
        {
            if (dgvResults.Rows.Count == 0)
            {
                MessageBox.Show("No records available to export.", "Export Excel", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "Excel Spreadsheet (*.xls)|*.xls",
                FileName = $"TMS_DynamicReports_{DateTime.Now:yyyyMMdd_HHmmss}.xls"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("<?xml version=\"1.0\"?>");
                    sb.AppendLine("<?mso-application progid=\"Excel.Sheet\"?>");
                    sb.AppendLine("<Workbook xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\"");
                    sb.AppendLine(" xmlns:o=\"urn:schemas-microsoft-com:office:office\"");
                    sb.AppendLine(" xmlns:x=\"urn:schemas-microsoft-com:office:excel\"");
                    sb.AppendLine(" xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\">");
                    sb.AppendLine(" <Styles>");
                    sb.AppendLine("  <Style ss:ID=\"Header\">");
                    sb.AppendLine("   <Font ss:Bold=\"1\" ss:Color=\"#FFFFFF\" ss:FontName=\"Segoe UI\" ss:Size=\"11\"/>");
                    sb.AppendLine("   <Interior ss:Color=\"#213D77\" ss:Pattern=\"Solid\"/>");
                    sb.AppendLine("   <Alignment ss:Vertical=\"Center\" ss:Horizontal=\"Left\"/>");
                    sb.AppendLine("   <Borders><Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" ss:Color=\"#000000\"/></Borders>");
                    sb.AppendLine("  </Style>");
                    sb.AppendLine("  <Style ss:ID=\"Title\">");
                    sb.AppendLine("   <Font ss:Bold=\"1\" ss:Color=\"#213D77\" ss:FontName=\"Segoe UI\" ss:Size=\"14\"/>");
                    sb.AppendLine("  </Style>");
                    sb.AppendLine("  <Style ss:ID=\"Data\">");
                    sb.AppendLine("   <Font ss:FontName=\"Segoe UI\" ss:Size=\"10\"/>");
                    sb.AppendLine("   <Borders><Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" ss:Color=\"#E2E8F0\"/></Borders>");
                    sb.AppendLine("  </Style>");
                    sb.AppendLine("  <Style ss:ID=\"DataDate\">");
                    sb.AppendLine("   <Font ss:FontName=\"Segoe UI\" ss:Size=\"10\"/>");
                    sb.AppendLine("   <NumberFormat ss:Format=\"yyyy-mm-dd hh:mm:ss\"/>");
                    sb.AppendLine("   <Borders><Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\" ss:Color=\"#E2E8F0\"/></Borders>");
                    sb.AppendLine("  </Style>");
                    sb.AppendLine(" </Styles>");
                    sb.AppendLine(" <Worksheet ss:Name=\"DynamicReports\">");
                    sb.AppendLine("  <Table>");
                    sb.AppendLine("   <Column ss:Width=\"45\"/>");
                    sb.AppendLine("   <Column ss:Width=\"85\"/>");
                    sb.AppendLine("   <Column ss:Width=\"180\"/>");
                    sb.AppendLine("   <Column ss:Width=\"170\"/>");
                    sb.AppendLine("   <Column ss:Width=\"140\"/>");
                    sb.AppendLine("   <Column ss:Width=\"260\"/>");
                    sb.AppendLine("   <Column ss:Width=\"140\"/>");
                    sb.AppendLine("   <Column ss:Width=\"120\"/>");
                    sb.AppendLine("   <Column ss:Width=\"130\"/>");
                    sb.AppendLine("   <Column ss:Width=\"90\"/>");
                    sb.AppendLine("   <Column ss:Width=\"150\"/>");

                    // Title Row
                    string scopeStr = chkRegisterSelector?.GetScopeSummaryText() ?? "ALL REGISTERS";
                    sb.AppendLine("   <Row ss:Height=\"26\">");
                    sb.AppendLine($"    <Cell ss:MergeAcross=\"10\" ss:StyleID=\"Title\"><Data ss:Type=\"String\">INDIAN RAILWAYS TMS - DYNAMIC REPORTS ({dtpFromDate.Value:dd-MMM-yyyy} to {dtpToDate.Value:dd-MMM-yyyy}) - Scope: {EscapeXml(scopeStr)}</Data></Cell>");
                    sb.AppendLine("   </Row>");

                    // Column Headers
                    sb.AppendLine("   <Row ss:Height=\"22\">");
                    sb.AppendLine("    <Cell ss:StyleID=\"Header\"><Data ss:Type=\"String\">S.No</Data></Cell>");
                    sb.AppendLine("    <Cell ss:StyleID=\"Header\"><Data ss:Type=\"String\">Reg Code</Data></Cell>");
                    sb.AppendLine("    <Cell ss:StyleID=\"Header\"><Data ss:Type=\"String\">Register Name</Data></Cell>");
                    sb.AppendLine("    <Cell ss:StyleID=\"Header\"><Data ss:Type=\"String\">Record ID</Data></Cell>");
                    sb.AppendLine("    <Cell ss:StyleID=\"Header\"><Data ss:Type=\"String\">Category / Type</Data></Cell>");
                    sb.AppendLine("    <Cell ss:StyleID=\"Header\"><Data ss:Type=\"String\">Description &amp; Details</Data></Cell>");
                    sb.AppendLine("    <Cell ss:StyleID=\"Header\"><Data ss:Type=\"String\">Asset / Train / Point</Data></Cell>");
                    sb.AppendLine("    <Cell ss:StyleID=\"Header\"><Data ss:Type=\"String\">Status / Remarks</Data></Cell>");
                    sb.AppendLine("    <Cell ss:StyleID=\"Header\"><Data ss:Type=\"String\">Reported By</Data></Cell>");
                    sb.AppendLine("    <Cell ss:StyleID=\"Header\"><Data ss:Type=\"String\">Staff ID</Data></Cell>");
                    sb.AppendLine("    <Cell ss:StyleID=\"Header\"><Data ss:Type=\"String\">Submission Date &amp; Time</Data></Cell>");
                    sb.AppendLine("   </Row>");

                    // Data Rows
                    int sNo = 1;
                    foreach (DataGridViewRow row in dgvResults.Rows)
                    {
                        string code = EscapeXml(CleanPdf(row.Cells["RegisterCode"].Value));
                        string name = EscapeXml(CleanPdf(row.Cells["RegisterName"].Value));
                        string recId = EscapeXml(CleanPdf(row.Cells["RecordID"].Value));
                        string cat = EscapeXml(CleanPdf(row.Cells["Category"].Value));
                        string desc = EscapeXml(CleanPdf(row.Cells["Description"].Value));
                        string asset = EscapeXml(CleanPdf(row.Cells["AssetInfo"].Value));
                        string status = EscapeXml(CleanPdf(row.Cells["Status"].Value));
                        string repBy = EscapeXml(CleanPdf(row.Cells["ReportedBy"].Value));
                        string staff = EscapeXml(CleanPdf(row.Cells["SubmittedBy"].Value));
                        string timeVal = row.Cells["SubmittedTime"].Value is DateTime dt ? dt.ToString("yyyy-MM-ddTHH:mm:ss.fff") : "";

                        sb.AppendLine("   <Row ss:Height=\"18\">");
                        sb.AppendLine($"    <Cell ss:StyleID=\"Data\"><Data ss:Type=\"Number\">{sNo++}</Data></Cell>");
                        sb.AppendLine($"    <Cell ss:StyleID=\"Data\"><Data ss:Type=\"String\">{code}</Data></Cell>");
                        sb.AppendLine($"    <Cell ss:StyleID=\"Data\"><Data ss:Type=\"String\">{name}</Data></Cell>");
                        sb.AppendLine($"    <Cell ss:StyleID=\"Data\"><Data ss:Type=\"String\">{recId}</Data></Cell>");
                        sb.AppendLine($"    <Cell ss:StyleID=\"Data\"><Data ss:Type=\"String\">{cat}</Data></Cell>");
                        sb.AppendLine($"    <Cell ss:StyleID=\"Data\"><Data ss:Type=\"String\">{desc}</Data></Cell>");
                        sb.AppendLine($"    <Cell ss:StyleID=\"Data\"><Data ss:Type=\"String\">{asset}</Data></Cell>");
                        sb.AppendLine($"    <Cell ss:StyleID=\"Data\"><Data ss:Type=\"String\">{status}</Data></Cell>");
                        sb.AppendLine($"    <Cell ss:StyleID=\"Data\"><Data ss:Type=\"String\">{repBy}</Data></Cell>");
                        sb.AppendLine($"    <Cell ss:StyleID=\"Data\"><Data ss:Type=\"String\">{staff}</Data></Cell>");
                        if (!string.IsNullOrEmpty(timeVal))
                            sb.AppendLine($"    <Cell ss:StyleID=\"DataDate\"><Data ss:Type=\"DateTime\">{timeVal}</Data></Cell>");
                        else
                            sb.AppendLine("    <Cell ss:StyleID=\"Data\"><Data ss:Type=\"String\">-</Data></Cell>");
                        sb.AppendLine("   </Row>");
                    }

                    sb.AppendLine("  </Table>");
                    sb.AppendLine(" </Worksheet>");
                    sb.AppendLine("</Workbook>");

                    File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                    MessageBox.Show($"Report successfully exported to Excel!\n\nLocation: {sfd.FileName}", "Excel Export Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error generating Excel spreadsheet: {ex.Message}", "Excel Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private string EscapeXml(string s)
        {
            if (string.IsNullOrEmpty(s)) return "-";
            return s.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace("'", "&apos;");
        }

        // ─────────────────────────────────────────────────────────────────────
        // 7. EXPORT ENGINE: CSV FORMAT (.CSV)
        // ─────────────────────────────────────────────────────────────────────
        private void ExportToCSV()
        {
            if (dgvResults.Rows.Count == 0)
            {
                MessageBox.Show("No records available to export.", "Export CSV", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "CSV (Comma delimited) (*.csv)|*.csv",
                FileName = $"TMS_DynamicReports_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    StringBuilder sb = new StringBuilder();
                    // Systematic Header Row with all parameters
                    sb.AppendLine("S.No,Register Code,Register Name,Record ID,Category / Type,Description & Details,Asset / Train / Point,Status / Remarks,Reported By,Staff ID,Submission Date & Time");

                    int sNo = 1;
                    foreach (DataGridViewRow row in dgvResults.Rows)
                    {
                        string code = EscapeCsv(CleanPdf(row.Cells["RegisterCode"].Value));
                        string name = EscapeCsv(CleanPdf(row.Cells["RegisterName"].Value));
                        string recId = EscapeCsv(CleanPdf(row.Cells["RecordID"].Value));
                        string cat = EscapeCsv(CleanPdf(row.Cells["Category"].Value));
                        string desc = EscapeCsv(CleanPdf(row.Cells["Description"].Value));
                        string asset = EscapeCsv(CleanPdf(row.Cells["AssetInfo"].Value));
                        string status = EscapeCsv(CleanPdf(row.Cells["Status"].Value));
                        string repBy = EscapeCsv(CleanPdf(row.Cells["ReportedBy"].Value));
                        string staff = EscapeCsv(CleanPdf(row.Cells["SubmittedBy"].Value));
                        string time = row.Cells["SubmittedTime"].Value is DateTime dt ? dt.ToString("yyyy-MM-dd HH:mm:ss") : "-";

                        sb.AppendLine($"{sNo++},{code},{name},{recId},{cat},{desc},{asset},{status},{repBy},{staff},{EscapeCsv(time)}");
                    }

                    File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                    MessageBox.Show($"Report successfully exported to CSV!\n\nLocation: {sfd.FileName}", "CSV Export Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error generating CSV: {ex.Message}", "CSV Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private string EscapeCsv(string s)
        {
            if (string.IsNullOrEmpty(s)) return "\"-\"";
            return $"\"{s.Replace("\"", "\"\"")}\"";
        }

        // ─────────────────────────────────────────────────────────────────────
        // 8. PRINT & PRINT PREVIEW ENGINE
        // ─────────────────────────────────────────────────────────────────────
        private void PrintReport()
        {
            if (dgvResults.Rows.Count == 0)
            {
                MessageBox.Show("No records available to print.", "Print Report", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                printRowIndex = 0;
                printPageNumber = 1;

                printDoc = new PrintDocument();
                printDoc.DefaultPageSettings.Landscape = true;
                printDoc.PrintPage += PrintDoc_PrintPage;

                PrintPreviewDialog prevDlg = new PrintPreviewDialog
                {
                    Document = printDoc,
                    WindowState = FormWindowState.Maximized,
                    Text = "Dynamic Reports - Print Preview"
                };
                prevDlg.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Print Error: {ex.Message}", "Print Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PrintDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            int startX = 25;
            int startY = 30;
            int currentY = startY;

            // Page Header
            using (Font titleFont = new Font("Segoe UI", 13F, FontStyle.Bold))
            using (Font subFont = new Font("Segoe UI", 9F))
            using (Brush navyBrush = new SolidBrush(Color.FromArgb(33, 61, 119)))
            using (Brush darkBrush = new SolidBrush(Color.FromArgb(15, 23, 42)))
            {
                g.DrawString("INDIAN RAILWAYS - TRAIN MANAGEMENT SYSTEM (TMS)", titleFont, navyBrush, startX, currentY);
                currentY += 24;
                string scopeStr = chkRegisterSelector?.GetScopeSummaryText() ?? "ALL REGISTERS (001 - 040 CONSOLIDATED)";
                g.DrawString($"CONSOLIDATED DYNAMIC REPORT (REG-041)  |  Scope: {Truncate(scopeStr, 45)}  |  Date: {dtpFromDate.Value:dd-MMM-yyyy} to {dtpToDate.Value:dd-MMM-yyyy}", subFont, darkBrush, startX, currentY);
                currentY += 18;
                g.DrawString($"Generated: {DateTime.Now:dd-MMM-yyyy hh:mm tt}  |  Total Records: {dgvResults.Rows.Count}  |  Page {printPageNumber}", subFont, darkBrush, startX, currentY);
                currentY += 22;
            }

            // Table Header Bar (11 Columns)
            int[] colWidths = { 45, 60, 135, 135, 105, 150, 95, 80, 95, 60, 125 };
            string[] headers = { "S.No", "Code", "Register Name", "Record ID", "Category", "Description", "Asset/Point", "Status", "Reported By", "Staff", "Date & Time" };

            int tableWidth = 0;
            foreach (int w in colWidths) tableWidth += w;

            g.FillRectangle(new SolidBrush(Color.FromArgb(33, 61, 119)), startX, currentY, tableWidth, 26);
            int curX = startX;

            using (Font headFont = new Font("Segoe UI", 8.5F, FontStyle.Bold))
            {
                for (int i = 0; i < headers.Length; i++)
                {
                    g.DrawString(headers[i], headFont, Brushes.White, curX + 4, currentY + 5);
                    curX += colWidths[i];
                }
            }

            currentY += 28;

            // Table Rows
            using (Font rowFont = new Font("Segoe UI", 7.5F))
            using (Pen linePen = new Pen(Color.FromArgb(226, 232, 240)))
            {
                while (printRowIndex < dgvResults.Rows.Count)
                {
                    if (currentY + 22 > e.MarginBounds.Bottom + 50)
                    {
                        e.HasMorePages = true;
                        printPageNumber++;
                        return;
                    }

                    DataGridViewRow row = dgvResults.Rows[printRowIndex];
                    curX = startX;

                    string[] vals = {
                        (printRowIndex + 1).ToString(),
                        CleanPdf(row.Cells["RegisterCode"].Value),
                        Truncate(CleanPdf(row.Cells["RegisterName"].Value), 20),
                        Truncate(CleanPdf(row.Cells["RecordID"].Value), 20),
                        Truncate(CleanPdf(row.Cells["Category"].Value), 14),
                        Truncate(CleanPdf(row.Cells["Description"].Value), 22),
                        Truncate(CleanPdf(row.Cells["AssetInfo"].Value), 12),
                        Truncate(CleanPdf(row.Cells["Status"].Value), 10),
                        Truncate(CleanPdf(row.Cells["ReportedBy"].Value), 12),
                        CleanPdf(row.Cells["SubmittedBy"].Value),
                        row.Cells["SubmittedTime"].Value is DateTime dt ? dt.ToString("dd-MMM-yy HH:mm") : "-"
                    };

                    if (printRowIndex % 2 == 1)
                    {
                        g.FillRectangle(new SolidBrush(Color.FromArgb(248, 250, 252)), startX, currentY, tableWidth, 20);
                    }

                    for (int i = 0; i < vals.Length; i++)
                    {
                        g.DrawString(vals[i], rowFont, Brushes.Black, curX + 4, currentY + 3);
                        curX += colWidths[i];
                    }

                    g.DrawLine(linePen, startX, currentY + 20, startX + tableWidth, currentY + 20);
                    currentY += 21;
                    printRowIndex++;
                }
            }

            e.HasMorePages = false;
        }
    }
}
