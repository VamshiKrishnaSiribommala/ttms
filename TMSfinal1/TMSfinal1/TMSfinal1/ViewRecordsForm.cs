using System;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace TMS
{
    // ── Native WM_MOUSEHWHEEL hook so two-finger left/right swipe scrolls the grid ──
    internal sealed class HScrollHook : NativeWindow, IDisposable
    {
        private const int WM_MOUSEHWHEEL = 0x020E;
        private readonly DataGridView _dgv;

        public HScrollHook(DataGridView dgv)
        {
            _dgv = dgv;
            dgv.HandleCreated  += (s, e) => AssignHandle(dgv.Handle);
            dgv.HandleDestroyed += (s, e) => ReleaseHandle();
            if (dgv.IsHandleCreated) AssignHandle(dgv.Handle);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_MOUSEHWHEEL)
            {
                int delta  = (short)(m.WParam.ToInt64() >> 16);     // positive = right
                int scroll = (delta > 0 ? 1 : -1) * SystemInformation.MouseWheelScrollLines * 30;
                int newOff = Math.Max(0, _dgv.HorizontalScrollingOffset + scroll);
                _dgv.HorizontalScrollingOffset = newOff;
                m.Result = (IntPtr)1;
                return;
            }
            base.WndProc(ref m);
        }

        public void Dispose() => ReleaseHandle();
    }

    public class ViewRecordsForm : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, string lParam);

        private DataGridView dgv;
        private Label        lblCount;
        private TextBox      txtSearch;
        private DataTable    fullData;
        private HScrollHook  _hScrollHook;
        private readonly string tableName;
        private readonly string formTitle;

        public ViewRecordsForm(string tableName, string title)
        {
            this.tableName = tableName;
            this.formTitle = title;
            BuildUI();
            LoadData();
        }

        // ─── BUILD UI ─────────────────────────────────────────────────────────
        private void BuildUI()
        {
            this.Text             = formTitle + " – Records";
            this.StartPosition    = FormStartPosition.CenterScreen;
            this.WindowState      = FormWindowState.Maximized;
            this.MinimumSize      = new Size(1000, 650);
            this.BackColor        = Color.FromArgb(241, 245, 249);
            this.Font             = new Font("Segoe UI", 10F);

            // ── 1. TOP HEADER CONTAINER (Contains Title + Toolbar) ──────
            Panel topHeader = new Panel {
                Dock      = DockStyle.Top,
                Height    = 102,
                BackColor = Color.White
            };

            // 1a. PAGE TITLE BAR (Top part of topHeader)
            Panel titleBar = new Panel {
                Dock      = DockStyle.Top,
                Height    = 48,
                BackColor = Color.FromArgb(33, 61, 119)   // #213D77 Indian Railways Navy
            };
            var lblT = new Label {
                Text      = formTitle + "  —  View Records",
                Font      = new Font("Segoe UI", 13.5F, FontStyle.Bold),
                ForeColor = Color.White,
                Dock      = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding   = new Padding(16, 0, 0, 0)
            };
            titleBar.Controls.Add(lblT);
            topHeader.Controls.Add(titleBar);

            // 1b. SEARCH TOOLBAR (Bottom part of topHeader)
            Panel toolbar = new Panel {
                Dock      = DockStyle.Fill,
                BackColor = Color.White
            };
            toolbar.Paint += (s, e) => e.Graphics.DrawLine(
                new Pen(Color.FromArgb(203, 213, 225)), 0, toolbar.Height - 1, toolbar.Width, toolbar.Height - 1);

            var lblS = new Label {
                Text = "🔍 Search Records:", Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 61, 119), AutoSize = true
            };
            toolbar.Controls.Add(lblS);

            txtSearch = new TextBox {
                Font = new Font("Segoe UI", 11.5F), BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White, ForeColor = Color.FromArgb(15, 23, 42),
                Size = new Size(320, 30)
            };
            txtSearch.TextChanged += OnSearch;
            txtSearch.HandleCreated += (s, e) => SendMessage(txtSearch.Handle, 0x1501, 1, "Type to filter any column...");
            toolbar.Controls.Add(txtSearch);

            lblCount = new Label {
                Font = new Font("Segoe UI", 11F, FontStyle.Bold), ForeColor = Color.FromArgb(5, 150, 105),
                AutoSize = true
            };
            toolbar.Controls.Add(lblCount);

            var tip = new Label {
                Text = "💡 Tip: Double-click any row to view complete record details",
                Font = new Font("Segoe UI", 10F, FontStyle.Italic), ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = true
            };
            toolbar.Controls.Add(tip);

            Action layoutToolbar = () =>
            {
                lblS.Location = new Point(16, 14);
                txtSearch.Location = new Point(lblS.Right + 12, 10);
                lblCount.Location = new Point(txtSearch.Right + 18, 14);
                tip.Location = new Point(lblCount.Right + 25, 15);
            };
            toolbar.Resize += (s, e) => layoutToolbar();
            this.Shown += (s, e) => layoutToolbar();

            topHeader.Controls.Add(toolbar);
            toolbar.BringToFront();

            // ── 2. FOOTER (Bottom of screen) ─────────────────────────────
            Panel footer = new Panel {
                Dock      = DockStyle.Bottom,
                Height    = 58,
                BackColor = Color.White
            };
            footer.Paint += (s, e) => e.Graphics.DrawLine(
                new Pen(Color.FromArgb(203, 213, 225)), 0, 0, footer.Width, 0);

            var btnClose = new Button {
                Text = "✕   CLOSE", Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                Size = new Size(160, 40), BackColor = Color.FromArgb(220, 38, 38),
                ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand,
                DialogResult = DialogResult.Cancel
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click      += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            btnClose.MouseEnter += (s, e) => btnClose.BackColor = Color.FromArgb(185, 28, 28);
            btnClose.MouseLeave += (s, e) => btnClose.BackColor = Color.FromArgb(220, 38, 38);
            footer.Controls.Add(btnClose);
            footer.Resize += (s, e) =>
                btnClose.Location = new Point((footer.Width - btnClose.Width) / 2,
                                              (footer.Height - btnClose.Height) / 2);

            // ── 3. MAIN TABLE WRAPPER (Fills remaining space below topHeader) ─
            Panel wrap = new Panel {
                Dock      = DockStyle.Fill,
                BackColor = Color.FromArgb(241, 245, 249),
                Padding   = new Padding(14, 10, 14, 8)
            };

            dgv = new DataGridView { Dock = DockStyle.Fill };
            StyleGrid();
            _hScrollHook = new HScrollHook(dgv);          // enables two-finger horizontal swipe
            this.FormClosed += (s, e) => _hScrollHook?.Dispose();
            wrap.Controls.Add(dgv);

            // ── 4. ADD TO FORM IN EXACT WINFORMS DOCKING ORDER ───────────
            this.Controls.Clear();
            this.Controls.Add(wrap);       // Added FIRST so DockStyle.Fill occupies remaining space
            this.Controls.Add(footer);     // Added SECOND for DockStyle.Bottom
            this.Controls.Add(topHeader);  // Added LAST so DockStyle.Top sits cleanly at top without overlapping wrap
        }

        // ─── GRID STYLING ─────────────────────────────────────────────────────
        private void StyleGrid()
        {
            dgv.BackgroundColor        = Color.White;
            dgv.BorderStyle            = BorderStyle.FixedSingle;
            dgv.CellBorderStyle        = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor              = Color.FromArgb(226, 232, 240);
            dgv.ReadOnly               = true;
            dgv.AllowUserToAddRows     = false;
            dgv.AllowUserToDeleteRows  = false;
            dgv.AllowUserToResizeRows  = false;
            dgv.SelectionMode          = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect            = false;
            dgv.RowHeadersVisible      = false;
            dgv.ScrollBars             = ScrollBars.Both;
            dgv.AutoSizeColumnsMode    = DataGridViewAutoSizeColumnsMode.DisplayedCells;

            // Handle DataError to suppress default error popups
            dgv.DataError += (s, e) => {
                e.Cancel = true;
                e.ThrowException = false;
            };

            // Data Cell Default Style
            dgv.DefaultCellStyle = new DataGridViewCellStyle {
                BackColor          = Color.White,
                ForeColor          = Color.FromArgb(15, 23, 42),
                SelectionBackColor = Color.FromArgb(199, 220, 255),
                SelectionForeColor = Color.FromArgb(15, 23, 42),
                Font               = new Font("Segoe UI", 11F),
                Padding            = new Padding(8, 6, 8, 6),
                WrapMode           = DataGridViewTriState.False
            };

            // Alternating Row Style
            dgv.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle {
                BackColor          = Color.FromArgb(248, 250, 252),
                ForeColor          = Color.FromArgb(15, 23, 42),
                SelectionBackColor = Color.FromArgb(199, 220, 255),
                SelectionForeColor = Color.FromArgb(15, 23, 42),
                Font               = new Font("Segoe UI", 11F),
                Padding            = new Padding(8, 6, 8, 6),
                WrapMode           = DataGridViewTriState.False
            };

            dgv.RowTemplate.Height = 44;

            // Column Header Style Configuration
            dgv.EnableHeadersVisualStyles      = false;
            dgv.ColumnHeadersVisible           = true;
            dgv.ColumnHeadersHeight            = 48;
            dgv.ColumnHeadersHeightSizeMode    = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgv.ColumnHeadersDefaultCellStyle  = new DataGridViewCellStyle {
                BackColor          = Color.FromArgb(33, 61, 119),   // #213D77 Indian Railways Navy
                ForeColor          = Color.White,
                Font               = new Font("Segoe UI", 11.5F, FontStyle.Bold),
                Alignment          = DataGridViewContentAlignment.MiddleLeft,
                Padding            = new Padding(8, 0, 8, 0),
                SelectionBackColor = Color.FromArgb(33, 61, 119),
                SelectionForeColor = Color.White
            };

            // ── CUSTOM CELL PAINTING: Guarantees 100% visible Column Headers & S.No text ──
            dgv.CellPainting += (s, e) => {
                // 1. Paint Column Headers (e.RowIndex == -1)
                if (e.RowIndex == -1 && e.ColumnIndex >= 0)
                {
                    e.PaintBackground(e.CellBounds, true);
                    using (Brush backBrush = new SolidBrush(Color.FromArgb(30, 41, 59)))
                    {
                        e.Graphics.FillRectangle(backBrush, e.CellBounds);
                    }
                    using (Pen borderPen = new Pen(Color.FromArgb(71, 85, 105)))
                    {
                        e.Graphics.DrawRectangle(borderPen, e.CellBounds.X, e.CellBounds.Y, e.CellBounds.Width - 1, e.CellBounds.Height - 1);
                    }
                    string headerText = dgv.Columns[e.ColumnIndex].HeaderText;
                    TextFormatFlags flags = TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;
                    if (e.ColumnIndex == 0)
                        flags |= TextFormatFlags.HorizontalCenter;
                    else
                        flags |= TextFormatFlags.Left;

                    Rectangle textBounds = new Rectangle(e.CellBounds.X + 6, e.CellBounds.Y, e.CellBounds.Width - 12, e.CellBounds.Height);
                    TextRenderer.DrawText(e.Graphics, headerText, new Font("Segoe UI", 10.5F, FontStyle.Bold), textBounds, Color.White, flags);
                    e.Handled = true;
                }
                // 2. Paint S.No Column Cells (e.ColumnIndex == 0)
                else if (e.RowIndex >= 0 && e.ColumnIndex == 0)
                {
                    e.PaintBackground(e.CellBounds, true);
                    Color bgColor = (dgv.Rows[e.RowIndex].Selected) ? Color.FromArgb(37, 99, 235) : Color.FromArgb(30, 41, 59);
                    using (Brush backBrush = new SolidBrush(bgColor))
                    {
                        e.Graphics.FillRectangle(backBrush, e.CellBounds);
                    }
                    using (Pen borderPen = new Pen(Color.FromArgb(51, 65, 85)))
                    {
                        e.Graphics.DrawLine(borderPen, e.CellBounds.Left, e.CellBounds.Bottom - 1, e.CellBounds.Right, e.CellBounds.Bottom - 1);
                    }
                    string val = e.Value?.ToString() ?? "";
                    TextFormatFlags flags = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter;
                    TextRenderer.DrawText(e.Graphics, val, new Font("Segoe UI", 10F, FontStyle.Bold), e.CellBounds, Color.White, flags);
                    e.Handled = true;
                }
            };

            // Row Hover Highlight
            dgv.CellMouseEnter += (s, e) => {
                if (e.RowIndex < 0) return;
                foreach (DataGridViewCell cell in dgv.Rows[e.RowIndex].Cells)
                    if (cell.ColumnIndex != 0)
                        cell.Style.BackColor = Color.FromArgb(224, 237, 255);
            };
            dgv.CellMouseLeave += (s, e) => {
                if (e.RowIndex < 0) return;
                foreach (DataGridViewCell cell in dgv.Rows[e.RowIndex].Cells)
                    if (cell.ColumnIndex != 0)
                        cell.Style.BackColor = Color.Empty;
            };

            // Format DateTime cells (including SubmittedAt) to include time with milliseconds
            dgv.CellFormatting += (s, e) => {
                if (e.Value != null && e.Value != DBNull.Value && e.RowIndex >= 0)
                {
                    string colName = dgv.Columns[e.ColumnIndex].Name;
                    if (colName.EndsWith("By", StringComparison.OrdinalIgnoreCase) ||
                        colName.EndsWith("ID", StringComparison.OrdinalIgnoreCase) ||
                        colName.Equals("SubmittedBy", StringComparison.OrdinalIgnoreCase))
                    {
                        return; // Do NOT format Staff ID / user ID as dates
                    }

                    if (e.Value is DateTime dtVal)
                    {
                        e.Value = dtVal.ToString("dd-MMM-yy hh:mm:ss.fff tt");
                        e.FormattingApplied = true;
                    }
                    else if ((colName.EndsWith("At", StringComparison.OrdinalIgnoreCase) ||
                              colName.IndexOf("Date", StringComparison.OrdinalIgnoreCase) >= 0 ||
                              colName.IndexOf("Time", StringComparison.OrdinalIgnoreCase) >= 0) &&
                             !colName.EndsWith("By", StringComparison.OrdinalIgnoreCase))
                    {
                        if (DateTime.TryParse(e.Value.ToString(), out DateTime parsedDt))
                        {
                            e.Value = parsedDt.ToString("dd-MMM-yy hh:mm:ss.fff tt");
                            e.FormattingApplied = true;
                        }
                    }
                }
            };

            // ── HORIZONTAL TOUCHPAD / MOUSE SCROLL SUPPORT ──────────────────
            // Intercept Shift+Scroll and two-finger horizontal swipe for left/right scrolling
            dgv.MouseWheel += (s, e) =>
            {
                // Shift+ScrollWheel = horizontal scroll
                if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
                {
                    int delta = e.Delta > 0 ? -3 : 3;
                    int newVal = Math.Max(dgv.HorizontalScrollingOffset,
                                 Math.Min(dgv.HorizontalScrollingOffset + delta * 30,
                                          dgv.Columns.GetColumnsWidth(DataGridViewElementStates.Visible) - dgv.ClientSize.Width));
                    if (newVal >= 0) dgv.HorizontalScrollingOffset = newVal;
                    ((HandledMouseEventArgs)e).Handled = true;
                }
            };

            // WM_MOUSEHWHEEL (0x020E) for native two-finger horizontal swipe on touchpad
            dgv.HandleCreated += (s, e) => dgv.GetType()
                .GetProperty("HScroll", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            // Double-Click → Styled Record Detail Modal
            dgv.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex < 0) return;

                // Collect field data
                var fields = new System.Collections.Generic.List<(string Label, string Value)>();
                foreach (DataGridViewCell cell in dgv.Rows[e.RowIndex].Cells)
                {
                    string header  = dgv.Columns[cell.ColumnIndex].HeaderText;
                    string colName = dgv.Columns[cell.ColumnIndex].Name;
                    string val;

                    bool isIdOrBy = colName.EndsWith("By",  StringComparison.OrdinalIgnoreCase) ||
                                    colName.EndsWith("ID",  StringComparison.OrdinalIgnoreCase) ||
                                    colName.Equals("SubmittedBy", StringComparison.OrdinalIgnoreCase);

                    if (!isIdOrBy && cell.Value is DateTime dtVal)
                        val = dtVal.ToString("dd-MMM-yyyy  hh:mm:ss tt");
                    else if (!isIdOrBy &&
                             (colName.EndsWith("At",   StringComparison.OrdinalIgnoreCase) ||
                              colName.IndexOf("Date",  StringComparison.OrdinalIgnoreCase) >= 0 ||
                              colName.IndexOf("Time",  StringComparison.OrdinalIgnoreCase) >= 0) &&
                             cell.Value != null && cell.Value != DBNull.Value &&
                             DateTime.TryParse(cell.Value.ToString(), out DateTime parsedDt))
                        val = parsedDt.ToString("dd-MMM-yyyy  hh:mm:ss tt");
                    else
                        val = cell.Value?.ToString() ?? "—";

                    if (string.IsNullOrWhiteSpace(val)) val = "—";
                    fields.Add((header, val));
                }

                // Build the detail dialog
                Form detailForm = new Form
                {
                    Text            = formTitle + " — Record Detail",
                    StartPosition   = FormStartPosition.CenterParent,
                    Size            = new Size(700, Math.Min(820, fields.Count * 52 + 160)),
                    MinimumSize     = new Size(600, 400),
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    MaximizeBox     = false,
                    MinimizeBox     = false,
                    BackColor       = Color.FromArgb(245, 247, 250),
                    Font            = new Font("Segoe UI", 10.5F)
                };

                // Title bar
                Panel titlePnl = new Panel
                {
                    Dock      = DockStyle.Top,
                    Height    = 56,
                    BackColor = Color.FromArgb(33, 61, 119)
                };
                Label titleLbl = new Label
                {
                    Text      = "📋  " + formTitle.ToUpper() + "  —  RECORD DETAIL",
                    Font      = new Font("Segoe UI", 11.5F, FontStyle.Bold),
                    ForeColor = Color.White,
                    Dock      = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Padding   = new Padding(18, 0, 0, 0)
                };
                titlePnl.Controls.Add(titleLbl);

                // Scrollable content panel
                Panel scroll = new Panel
                {
                    Dock        = DockStyle.Fill,
                    AutoScroll  = true,
                    Padding     = new Padding(24, 14, 24, 14),
                    BackColor   = Color.FromArgb(245, 247, 250)
                };

                int rowY = 0;
                bool alternate = false;
                foreach (var (label, value) in fields)
                {
                    Panel row = new Panel
                    {
                        Location  = new Point(0, rowY),
                        Size      = new Size(620, 46),
                        BackColor = alternate ? Color.FromArgb(237, 242, 255) : Color.White,
                        Anchor    = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
                    };

                    // Left: field label
                    Label lblField = new Label
                    {
                        Text      = label,
                        Font      = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                        ForeColor = Color.FromArgb(33, 61, 119),
                        Location  = new Point(14, 0),
                        Size      = new Size(200, 46),
                        TextAlign = ContentAlignment.MiddleLeft
                    };

                    // Separator bar
                    Label sep = new Label
                    {
                        Text      = "",
                        BackColor = Color.FromArgb(203, 213, 225),
                        Location  = new Point(214, 13),
                        Size      = new Size(1, 20)
                    };

                    // Right: value
                    Label lblValue = new Label
                    {
                        Text      = value,
                        Font      = new Font("Segoe UI", 10.5F),
                        ForeColor = Color.FromArgb(15, 23, 42),
                        Location  = new Point(226, 0),
                        Size      = new Size(380, 46),
                        TextAlign = ContentAlignment.MiddleLeft
                    };

                    row.Controls.Add(lblField);
                    row.Controls.Add(sep);
                    row.Controls.Add(lblValue);
                    scroll.Controls.Add(row);

                    rowY += 47;
                    alternate = !alternate;
                }

                // Footer close button
                Panel footerPnl = new Panel
                {
                    Dock      = DockStyle.Bottom,
                    Height    = 58,
                    BackColor = Color.White
                };
                footerPnl.Paint += (fp, fe) =>
                    fe.Graphics.DrawLine(new Pen(Color.FromArgb(203, 213, 225)), 0, 0, footerPnl.Width, 0);

                Button btnOk = new Button
                {
                    Text         = "✔  OK",
                    Size         = new Size(140, 38),
                    Font         = new Font("Segoe UI", 11F, FontStyle.Bold),
                    BackColor    = Color.FromArgb(33, 61, 119),
                    ForeColor    = Color.White,
                    FlatStyle    = FlatStyle.Flat,
                    DialogResult = DialogResult.OK,
                    Cursor       = Cursors.Hand
                };
                btnOk.FlatAppearance.BorderSize = 0;
                btnOk.Click += (bs, be) => detailForm.Close();
                footerPnl.Controls.Add(btnOk);
                footerPnl.Resize += (fp, fe) =>
                    btnOk.Location = new Point((footerPnl.Width - btnOk.Width) / 2, (footerPnl.Height - btnOk.Height) / 2);

                detailForm.Controls.Add(scroll);
                detailForm.Controls.Add(footerPnl);
                detailForm.Controls.Add(titlePnl);
                detailForm.AcceptButton = btnOk;
                detailForm.ShowDialog(this);
            };
        }

        // ─── DATA LOADING ─────────────────────────────────────────────────────
        private void LoadData()
        {
            try
            {
                DataTable raw = null;
                // Safe query execution with fallbacks
                try
                {
                    string query = $"SELECT * FROM {tableName} ORDER BY SubmittedAt DESC";
                    raw = new DatabaseHelper().ExecuteQuery(query);
                }
                catch
                {
                    try
                    {
                        string query = $"SELECT * FROM {tableName} ORDER BY 1 DESC";
                        raw = new DatabaseHelper().ExecuteQuery(query);
                    }
                    catch
                    {
                        string query = $"SELECT * FROM {tableName}";
                        raw = new DatabaseHelper().ExecuteQuery(query);
                    }
                }

                if (raw != null)
                {
                    fullData = AddSerialNumbers(raw);
                    BindData(fullData);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading records from database:\n" + ex.Message, "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Adds "S.No" as column 0
        private DataTable AddSerialNumbers(DataTable raw)
        {
            DataTable display = raw.Clone();

            if (display.Columns.Contains("S.No"))
                display.Columns.Remove("S.No");

            DataColumn snCol = new DataColumn("S.No", typeof(string));
            display.Columns.Add(snCol);
            display.Columns["S.No"].SetOrdinal(0);

            int n = 1;
            foreach (DataRow r in raw.Rows)
            {
                DataRow nr = display.NewRow();
                nr["S.No"] = n++.ToString();
                foreach (DataColumn col in raw.Columns)
                {
                    if (display.Columns.Contains(col.ColumnName))
                        nr[col.ColumnName] = r[col];
                }
                display.Rows.Add(nr);
            }
            return display;
        }

        private void BindData(DataTable dt)
        {
            dgv.DataSource = null;
            dgv.DataSource = dt;

            if (dgv.Columns.Count > 0)
            {
                // Column 0: S.No formatting
                dgv.Columns[0].AutoSizeMode  = DataGridViewAutoSizeColumnMode.None;
                dgv.Columns[0].Width         = 65;
                dgv.Columns[0].Frozen        = true;
                dgv.Columns[0].HeaderText    = "S.No";

                // Format all remaining column headers cleanly
                for (int i = 1; i < dgv.Columns.Count; i++)
                {
                    DataGridViewColumn col = dgv.Columns[i];
                    col.HeaderText = FormatHeaderText(col.Name);
                    col.MinimumWidth = 120;

                    string nameLower = col.Name.ToLower();
                    if (!nameLower.EndsWith("by") && !nameLower.EndsWith("id") && (col.ValueType == typeof(DateTime) || nameLower.EndsWith("at") || nameLower.Contains("date") || nameLower.Contains("time")))
                    {
                        col.DefaultCellStyle.Format = "dd-MMM-yy hh:mm:ss.fff tt";
                    }

                    if (nameLower.Contains("description") || nameLower.Contains("remark") || 
                        nameLower.Contains("item") || nameLower.Contains("irregularity") || 
                        nameLower.Contains("action") || nameLower.Contains("reason") ||
                        nameLower.Contains("detail"))
                    {
                        col.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                        col.MinimumWidth = 220;
                    }
                }
            }

            if (dgv.Rows.Count > 0)
            {
                dgv.FirstDisplayedScrollingRowIndex = 0;
            }

            UpdateCount(dt.Rows.Count);
        }

        private string FormatHeaderText(string name)
        {
            if (string.IsNullOrEmpty(name)) return "";
            if (name.Equals("S.No", StringComparison.OrdinalIgnoreCase)) return "S.No";
            if (name.Equals("ID", StringComparison.OrdinalIgnoreCase)) return "ID";
            if (name.Equals("LogID", StringComparison.OrdinalIgnoreCase)) return "Log ID";
            if (name.Equals("OfficerID", StringComparison.OrdinalIgnoreCase)) return "Officer ID";
            if (name.Equals("StaffID", StringComparison.OrdinalIgnoreCase)) return "Staff ID";
            if (name.Equals("SWRID", StringComparison.OrdinalIgnoreCase)) return "SWR ID";
            if (name.Equals("PNNo", StringComparison.OrdinalIgnoreCase)) return "PN Number";

            // Insert space before capital letters in camelCase/PascalCase string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < name.Length; i++)
            {
                if (i > 0 && char.IsUpper(name[i]) && (!char.IsUpper(name[i - 1]) || (i + 1 < name.Length && char.IsLower(name[i + 1]))))
                {
                    sb.Append(" ");
                }
                sb.Append(name[i]);
            }
            return sb.ToString().Trim();
        }

        private void UpdateCount(int n)
        {
            lblCount.Text      = $"Total Records: {n}";
            lblCount.ForeColor = n > 0 ? Color.FromArgb(5, 150, 105) : Color.FromArgb(220, 38, 38);
        }

        // ─── SEARCH ───────────────────────────────────────────────────────────
        private void OnSearch(object sender, EventArgs e)
        {
            if (fullData == null) return;
            string q = txtSearch.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(q)) { BindData(fullData); return; }

            DataTable filtered = fullData.Clone();
            foreach (DataRow row in fullData.Rows)
            {
                foreach (var item in row.ItemArray)
                {
                    if (item?.ToString().ToLower().Contains(q) == true)
                    {
                        filtered.ImportRow(row);
                        break;
                    }
                }
            }

            // Re-sequence S.No after search filtering
            int n = 1;
            foreach (DataRow row in filtered.Rows)
                row["S.No"] = n++.ToString();

            BindData(filtered);
        }
    }
}