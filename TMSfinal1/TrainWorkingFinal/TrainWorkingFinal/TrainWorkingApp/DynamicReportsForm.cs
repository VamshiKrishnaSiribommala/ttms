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

namespace TrainWorkingApp
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
    /// Interactive Multi-Select Register Checkbox Dropdown Selector for Train Working Authorities.
    /// Provides Checkboxes for all 22 Registers with Quick Presets, Search filtering,
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
            public int Number => int.TryParse(Code.Replace("AUTH-", "").Trim(), out int n) ? n : 0;
            public bool IsChecked { get; set; } = true;
            public override string ToString() => $"{Code}: {Name}";
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
            this.Size = new Size(350, 30);
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
                Text = "☑ ALL AUTHORITIES (001 - 022 CONSOLIDATED)",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 61, 119),
                Location = new Point(8, 5),
                Size = new Size(this.Width - 36, 20),
                AutoEllipsis = true,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            lblSummary.Click += (s, e) => ToggleDropDown();
            this.Controls.Add(lblSummary);

            lblDropChevron = new Label
            {
                Text = "▼",
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                ForeColor = Color.FromArgb(71, 85, 105),
                Location = new Point(this.Width - 26, 6),
                Size = new Size(20, 18),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            lblDropChevron.Click += (s, e) => ToggleDropDown();
            this.Controls.Add(lblDropChevron);

            this.Click += (s, e) => ToggleDropDown();

            BuildDropDown();
        }

        private void BuildDropDown()
        {
            Panel popupPanel = new Panel
            {
                Size = new Size(440, 500),
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
                Height = 38,
                BackColor = Color.FromArgb(33, 61, 119)
            };
            Label lblPopupTitle = new Label
            {
                Text = "📋  SELECT REGISTERS TO AUDIT & PRINT",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(12, 9),
                AutoSize = true
            };
            pnlHeader.Controls.Add(lblPopupTitle);
            popupPanel.Controls.Add(pnlHeader);

            // 2. Search Box
            Panel pnlSearch = new Panel
            {
                Dock = DockStyle.Top,
                Height = 42,
                BackColor = Color.FromArgb(248, 250, 252),
                Padding = new Padding(10, 6, 10, 6)
            };
            txtSearch = new TextBox
            {
                Font = new Font("Segoe UI", 9.5F),
                BorderStyle = BorderStyle.FixedSingle,
                Dock = DockStyle.Fill
            };
            txtSearch.TextChanged += (s, e) => FilterRegisterList(txtSearch.Text);
            txtSearch.HandleCreated += (s, e) => SendMessage(txtSearch.Handle, 0x1501, 1, "🔍 Search authority register name or code...");
            pnlSearch.Controls.Add(txtSearch);
            popupPanel.Controls.Add(pnlSearch);

            // 3. Quick Presets
            Panel pnlPresets = new Panel
            {
                Dock = DockStyle.Top,
                Height = 64,
                BackColor = Color.FromArgb(241, 245, 249),
                Padding = new Padding(6, 4, 6, 4)
            };

            Button btnAll = CreatePillButton("✔ Select All", Color.FromArgb(22, 163, 74), 8, 4, 95, 26, () => SetAllChecked(true));
            Button btnNone = CreatePillButton("✖ Clear All", Color.FromArgb(220, 38, 38), 108, 4, 90, 26, () => SetAllChecked(false));
            Button btnSig = CreatePillButton("🚆 Signalling (001-007)", Color.FromArgb(30, 58, 138), 203, 4, 215, 26, () => CheckRange(1, 7));

            Button btnFail = CreatePillButton("⚠️ Failures (008-013)", Color.FromArgb(146, 64, 14), 8, 33, 145, 26, () => CheckRange(8, 13));
            Button btnAbs = CreatePillButton("🛡️ ABS & Block (014-018)", Color.FromArgb(153, 27, 27), 158, 33, 150, 26, () => CheckRange(14, 18));
            Button btnMaint = CreatePillButton("⚡ Movement (019-022)", Color.FromArgb(6, 95, 70), 313, 33, 115, 26, () => CheckRange(19, 22));

            pnlPresets.Controls.AddRange(new Control[] { btnAll, btnNone, btnSig, btnFail, btnAbs, btnMaint });
            popupPanel.Controls.Add(pnlPresets);

            // 4. CheckedListBox
            checkedListBox = new CheckedListBox
            {
                Dock = DockStyle.Fill,
                CheckOnClick = true,
                Font = new Font("Segoe UI", 9.5F),
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
                Text = "22 of 22 Selected",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(5, 150, 105),
                Location = new Point(10, 12),
                AutoSize = true
            };
            pnlFooter.Controls.Add(lblPopupCount);

            Button btnApply = new Button
            {
                Text = "✓ Apply & Close",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                BackColor = Color.FromArgb(251, 121, 43), // IRCTC Orange
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(130, 32),
                Location = new Point(295, 6),
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
                Padding = Padding.Empty,
                Margin = Padding.Empty,
                AutoClose = true
            };
            dropDown.Items.Add(host);
            dropDown.Closed += (s, e) =>
            {
                UpdateSummaryLabel();
                SelectionChanged?.Invoke(this, EventArgs.Empty);
            };
        }

        private Button CreatePillButton(string text, Color col, int x, int y, int w, int h, Action onClick)
        {
            Button btn = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(w, h),
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                BackColor = Color.White,
                ForeColor = col,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderColor = Color.FromArgb(180, col);
            btn.FlatAppearance.BorderSize = 1;
            btn.Click += (s, e) => onClick();
            return btn;
        }

        public void InitializeItems(IEnumerable<DynamicReportsForm.RegMeta> registers)
        {
            allItems.Clear();
            foreach (var r in registers)
            {
                allItems.Add(new RegisterItem
                {
                    Code = r.Code,
                    Name = r.Name,
                    IsChecked = true
                });
            }
            FilterRegisterList("");
            UpdateSummaryLabel();
        }

        private void FilterRegisterList(string query)
        {
            isUpdatingChecks = true;
            checkedListBox.Items.Clear();
            filteredItems.Clear();

            string q = (query ?? "").Trim().ToLower();
            foreach (var item in allItems)
            {
                if (string.IsNullOrEmpty(q) || item.Code.ToLower().Contains(q) || item.Name.ToLower().Contains(q))
                {
                    filteredItems.Add(item);
                    checkedListBox.Items.Add(item, item.IsChecked);
                }
            }
            isUpdatingChecks = false;
            UpdatePopupCountLabel();
        }

        private void CheckedListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (isUpdatingChecks) return;
            if (e.Index >= 0 && e.Index < filteredItems.Count)
            {
                filteredItems[e.Index].IsChecked = (e.NewValue == CheckState.Checked);
                this.BeginInvoke(new Action(() =>
                {
                    UpdatePopupCountLabel();
                    UpdateSummaryLabel();
                }));
            }
        }

        private void SetAllChecked(bool check)
        {
            isUpdatingChecks = true;
            foreach (var item in allItems)
                item.IsChecked = check;

            for (int i = 0; i < checkedListBox.Items.Count; i++)
                checkedListBox.SetItemChecked(i, check);

            isUpdatingChecks = false;
            UpdatePopupCountLabel();
            UpdateSummaryLabel();
        }

        private void CheckRange(int minNum, int maxNum)
        {
            isUpdatingChecks = true;
            foreach (var item in allItems)
            {
                item.IsChecked = (item.Number >= minNum && item.Number <= maxNum);
            }
            for (int i = 0; i < checkedListBox.Items.Count; i++)
            {
                RegisterItem item = checkedListBox.Items[i] as RegisterItem;
                if (item != null)
                {
                    checkedListBox.SetItemChecked(i, item.IsChecked);
                }
            }
            isUpdatingChecks = false;
            UpdatePopupCountLabel();
            UpdateSummaryLabel();
        }

        public void ToggleDropDown()
        {
            if (dropDown.Visible)
            {
                dropDown.Close();
            }
            else
            {
                FilterRegisterList(txtSearch.Text);
                Point pt = this.PointToScreen(new Point(0, this.Height));
                dropDown.Show(pt);
                txtSearch.Focus();
            }
        }

        private void UpdatePopupCountLabel()
        {
            int total = allItems.Count;
            int checkedCount = allItems.Count(x => x.IsChecked);
            lblPopupCount.Text = $"{checkedCount} of {total} Selected";
            lblPopupCount.ForeColor = checkedCount == 0 ? Color.FromArgb(220, 38, 38) : Color.FromArgb(5, 150, 105);
        }

        private void UpdateSummaryLabel()
        {
            int total = allItems.Count;
            int checkedCount = allItems.Count(x => x.IsChecked);

            if (checkedCount == total)
            {
                lblSummary.Text = "☑ ALL AUTHORITIES (001 - 022 CONSOLIDATED)";
                lblSummary.ForeColor = Color.FromArgb(33, 61, 119);
            }
            else if (checkedCount == 0)
            {
                lblSummary.Text = "⚠ NO REGISTERS SELECTED (PLEASE SELECT)";
                lblSummary.ForeColor = Color.FromArgb(220, 38, 38);
            }
            else if (checkedCount == 1)
            {
                var single = allItems.First(x => x.IsChecked);
                lblSummary.Text = $"📄 {single.Code}: {single.Name}";
                lblSummary.ForeColor = Color.FromArgb(30, 58, 138);
            }
            else
            {
                lblSummary.Text = $"☑ {checkedCount} OF {total} REGISTERS SELECTED";
                lblSummary.ForeColor = Color.FromArgb(251, 121, 43); // Orange
            }
        }

        public List<string> GetSelectedCodes()
        {
            return allItems.Where(x => x.IsChecked).Select(x => x.Code).ToList();
        }

        public bool IsAllSelected => allItems.Count > 0 && allItems.All(x => x.IsChecked);
        public bool IsNoneSelected => allItems.All(x => !x.IsChecked);
    }

    /// <summary>
    /// Consolidated Multi-Register Dynamic Reports & Cross-Register Audit Console.
    /// Features:
    /// - Multi-Select Checkbox Dropdown for all 22 registers with search, quick presets, and consolidation.
    /// - Direct "Save as PDF" pure generator.
    /// - Professional Excel (.xls XML Spreadsheet) export with auto-sized columns and navy headers.
    /// - Professional CSV export.
    /// - Max 3-day date range enforcement (Last 3 Days default, Today, Yesterday & Today presets).
    /// </summary>
    public class DynamicReportsForm : Form
    {
        private CheckedRegisterSelector regSelector;
        private DateTimePicker dtpFrom;
        private DateTimePicker dtpTo;
        private Button btnPresetToday;
        private Button btnPresetYesterday;
        private Button btnPreset3Days;
        private TextBox txtSearch;
        private TouchpadScrollableDataGridView dgvReport;
        private Label lblTotalRecords;
        private DataTable currentData;
        private readonly DatabaseHelper db = new DatabaseHelper();
        private readonly List<RegMeta> registerRegistry = new List<RegMeta>();
        private bool isInitializing = true;

        public class RegMeta
        {
            public string Code { get; set; }
            public string Name { get; set; }
            public string TableName { get; set; }
            public string DateColumn { get; set; }
            public string SelectQuery { get; set; }
            public override string ToString() => $"{Code}: {Name}";
        }

        public DynamicReportsForm()
        {
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Font = new Font("Segoe UI", 9.75f, FontStyle.Regular);
            InitializeRegisterMetadata();
            InitializeComponent();
            regSelector.InitializeItems(registerRegistry);
            isInitializing = false;
            ApplyPreset(3); // Default to Last 3 Days Consolidated
        }

        private void InitializeRegisterMetadata()
        {
            registerRegistry.Add(new RegMeta { 
                Code = "AUTH-001", 
                Name = "Advance Authority Defective Signal (T/369-3b)", 
                TableName = "Advance_Authority_Defective_Signal", 
                DateColumn = "Date_Time_of_Issue", 
                SelectQuery = "SELECT CAST(Date_Time_of_Issue AS DATETIME) AS [Record_Timestamp], 'AUTH-001' AS [Authority_Code], 'Advance Authority Defective Signal' AS [Register_Name], CAST(Authority_Reference_No AS NVARCHAR(100)) AS [Authority_Ref_No], CAST(Train_No AS NVARCHAR(100)) AS [Train_No], CAST(Signal_Location AS NVARCHAR(100)) AS [Section_Location], CAST(Reason AS NVARCHAR(255)) AS [Operational_Reason], CAST(Permitted_Speed AS NVARCHAR(100)) AS [Speed_Status], CAST(Issuing_Station AS NVARCHAR(100)) AS [Issuing_Station], CAST(Issuing_Officer AS NVARCHAR(100)) AS [Issuing_Officer] FROM [Advance_Authority_Defective_Signal]"
            });
            registerRegistry.Add(new RegMeta { 
                Code = "AUTH-002", 
                Name = "Authority to Pass Signal at ON (T/369-3b)", 
                TableName = "Authority_Pass_Signal_ON", 
                DateColumn = "Date_Time_of_Issue", 
                SelectQuery = "SELECT CAST(Date_Time_of_Issue AS DATETIME) AS [Record_Timestamp], 'AUTH-002' AS [Authority_Code], 'Authority to Pass Signal at ON' AS [Register_Name], CAST(Authority_Reference_No AS NVARCHAR(100)) AS [Authority_Ref_No], CAST(Train_No AS NVARCHAR(100)) AS [Train_No], CAST(Signal_Location AS NVARCHAR(100)) AS [Section_Location], CAST(Reason AS NVARCHAR(255)) AS [Operational_Reason], CAST(Permitted_Speed AS NVARCHAR(100)) AS [Speed_Status], 'Station' AS [Issuing_Station], CAST(Issuing_Officer AS NVARCHAR(100)) AS [Issuing_Officer] FROM [Authority_Pass_Signal_ON]"
            });
            registerRegistry.Add(new RegMeta { 
                Code = "AUTH-003", 
                Name = "Caution Order Entry (T/409)", 
                TableName = "Caution_Order_Entry", 
                DateColumn = "Date_Time_of_Issue", 
                SelectQuery = "SELECT CAST(Date_Time_of_Issue AS DATETIME) AS [Record_Timestamp], 'AUTH-003' AS [Authority_Code], 'Caution Order Entry (T/409)' AS [Register_Name], CAST(Caution_Order_No AS NVARCHAR(100)) AS [Authority_Ref_No], CAST(Train_No AS NVARCHAR(100)) AS [Train_No], CAST(Section_Location AS NVARCHAR(100)) AS [Section_Location], CAST(Reason AS NVARCHAR(255)) AS [Operational_Reason], CAST(Speed_Restriction AS NVARCHAR(100)) AS [Speed_Status], 'Station' AS [Issuing_Station], CAST(Issued_By AS NVARCHAR(100)) AS [Issuing_Officer] FROM [Caution_Order_Entry]"
            });
            registerRegistry.Add(new RegMeta { 
                Code = "AUTH-004", 
                Name = "Receive on Obstructed Line (T/509)", 
                TableName = "Authority_Receive_Obstructed_Line", 
                DateColumn = "Date_Time_of_Issue", 
                SelectQuery = "SELECT CAST(Date_Time_of_Issue AS DATETIME) AS [Record_Timestamp], 'AUTH-004' AS [Authority_Code], 'Receive on Obstructed Line' AS [Register_Name], CAST(Authority_Reference_No AS NVARCHAR(100)) AS [Authority_Ref_No], CAST(Train_No AS NVARCHAR(100)) AS [Train_No], CAST(Line_No AS NVARCHAR(100)) AS [Section_Location], CAST(Nature_of_Obstruction AS NVARCHAR(255)) AS [Operational_Reason], CAST(Permitted_Speed AS NVARCHAR(100)) AS [Speed_Status], 'Station' AS [Issuing_Station], CAST(Issuing_Officer AS NVARCHAR(100)) AS [Issuing_Officer] FROM [Authority_Receive_Obstructed_Line]"
            });
            registerRegistry.Add(new RegMeta { 
                Code = "AUTH-005", 
                Name = "Receive on Non-Signalled Line", 
                TableName = "Authority_Receive_Non_Signalled", 
                DateColumn = "Date_Time_of_Issue", 
                SelectQuery = "SELECT CAST(Date_Time_of_Issue AS DATETIME) AS [Record_Timestamp], 'AUTH-005' AS [Authority_Code], 'Receive on Non-Signalled Line' AS [Register_Name], CAST(Authority_Reference_No AS NVARCHAR(100)) AS [Authority_Ref_No], CAST(Train_No AS NVARCHAR(100)) AS [Train_No], CAST(Line_No AS NVARCHAR(100)) AS [Section_Location], 'Receive Non-Signalled' AS [Operational_Reason], '-' AS [Speed_Status], CAST(Receiving_Station AS NVARCHAR(100)) AS [Issuing_Station], CAST(Issuing_Officer AS NVARCHAR(100)) AS [Issuing_Officer] FROM [Authority_Receive_Non_Signalled]"
            });
            registerRegistry.Add(new RegMeta { 
                Code = "AUTH-006", 
                Name = "Start from Non-Signalled Line (T/511)", 
                TableName = "Authority_Start_Non_Signalled", 
                DateColumn = "Date_Time_of_Issue", 
                SelectQuery = "SELECT CAST(Date_Time_of_Issue AS DATETIME) AS [Record_Timestamp], 'AUTH-006' AS [Authority_Code], 'Start Non-Signalled Line' AS [Register_Name], CAST(Authority_Reference_No AS NVARCHAR(100)) AS [Authority_Ref_No], CAST(Train_No AS NVARCHAR(100)) AS [Train_No], CAST(Line_No AS NVARCHAR(100)) AS [Section_Location], 'Start Non-Signalled' AS [Operational_Reason], '-' AS [Speed_Status], CAST(Starting_Station AS NVARCHAR(100)) AS [Issuing_Station], CAST(Issuing_Officer AS NVARCHAR(100)) AS [Issuing_Officer] FROM [Authority_Start_Non_Signalled]"
            });
            registerRegistry.Add(new RegMeta { 
                Code = "AUTH-007", 
                Name = "Authority for Common Starter (T/512)", 
                TableName = "Authority_Common_Starter", 
                DateColumn = "Date_Time_of_Issue", 
                SelectQuery = "SELECT CAST(Date_Time_of_Issue AS DATETIME) AS [Record_Timestamp], 'AUTH-007' AS [Authority_Code], 'Authority Common Starter' AS [Register_Name], CAST(Authority_Reference_No AS NVARCHAR(100)) AS [Authority_Ref_No], CAST(Train_No AS NVARCHAR(100)) AS [Train_No], CAST(Starter_Signal_ID AS NVARCHAR(100)) AS [Section_Location], 'Common Starter Authority' AS [Operational_Reason], '-' AS [Speed_Status], CAST(Starting_Station AS NVARCHAR(100)) AS [Issuing_Station], CAST(Issuing_Officer AS NVARCHAR(100)) AS [Issuing_Officer] FROM [Authority_Common_Starter]"
            });
            registerRegistry.Add(new RegMeta { 
                Code = "AUTH-008", 
                Name = "Relief Train into Occupied Block (T/A 602)", 
                TableName = "Relief_Train_Authorization", 
                DateColumn = "Date_Time_of_Issue", 
                SelectQuery = "SELECT CAST(Date_Time_of_Issue AS DATETIME) AS [Record_Timestamp], 'AUTH-008' AS [Authority_Code], 'Relief Train Authorization' AS [Register_Name], CAST(Authorization_Reference_No AS NVARCHAR(100)) AS [Authority_Ref_No], CAST(Relief_Train_No AS NVARCHAR(100)) AS [Train_No], CAST(Block_Section AS NVARCHAR(100)) AS [Section_Location], CAST(Purpose_of_Relief AS NVARCHAR(255)) AS [Operational_Reason], CAST(Permitted_Speed AS NVARCHAR(100)) AS [Speed_Status], CAST(Issuing_Station AS NVARCHAR(100)) AS [Issuing_Station], CAST(Issuing_Officer AS NVARCHAR(100)) AS [Issuing_Officer] FROM [Relief_Train_Authorization]"
            });
            registerRegistry.Add(new RegMeta { 
                Code = "AUTH-009", 
                Name = "Total Interruption of Comm (T/C 602)", 
                TableName = "Communication_Failure_Log", 
                DateColumn = "Failure_Start_Time", 
                SelectQuery = "SELECT CAST(Failure_Start_Time AS DATETIME) AS [Record_Timestamp], 'AUTH-009' AS [Authority_Code], 'Communication Failure Log' AS [Register_Name], CAST(Failure_Reference_No AS NVARCHAR(100)) AS [Authority_Ref_No], CAST(Communication_Method AS NVARCHAR(100)) AS [Train_No], CAST(Section_Affected AS NVARCHAR(100)) AS [Section_Location], CAST(Type_of_Failure AS NVARCHAR(255)) AS [Operational_Reason], CAST(Message_Reference AS NVARCHAR(100)) AS [Speed_Status], 'Section' AS [Issuing_Station], CAST(Authorized_By AS NVARCHAR(100)) AS [Issuing_Officer] FROM [Communication_Failure_Log]"
            });
            registerRegistry.Add(new RegMeta { 
                Code = "AUTH-010", 
                Name = "Line Clear Inquiry & Permission (TFC)", 
                TableName = "Line_Clear_Inquiry_TFC", 
                DateColumn = "Date_Time_Sent", 
                SelectQuery = "SELECT CAST(Date_Time_Sent AS DATETIME) AS [Record_Timestamp], 'AUTH-010' AS [Authority_Code], 'Line Clear Inquiry & Permission' AS [Register_Name], CAST(Inquiry_Reference_No AS NVARCHAR(100)) AS [Authority_Ref_No], CAST(Transmission_Method AS NVARCHAR(100)) AS [Train_No], CAST(Receiving_Station AS NVARCHAR(100)) AS [Section_Location], CAST(Transmission_Method AS NVARCHAR(255)) AS [Operational_Reason], CAST(Line_Clear_Response AS NVARCHAR(100)) AS [Speed_Status], CAST(Sending_Station AS NVARCHAR(100)) AS [Issuing_Station], CAST(Line_Clear_Response AS NVARCHAR(100)) AS [Issuing_Officer] FROM [Line_Clear_Inquiry_TFC]"
            });
            registerRegistry.Add(new RegMeta { 
                Code = "AUTH-011", 
                Name = "Temporary Single Line Working (T/D 602)", 
                TableName = "Temporary_Single_Line_Working", 
                DateColumn = "Start_Time", 
                SelectQuery = "SELECT CAST(Start_Time AS DATETIME) AS [Record_Timestamp], 'AUTH-011' AS [Authority_Code], 'Temporary Single Line Working' AS [Register_Name], CAST(Working_Reference_No AS NVARCHAR(100)) AS [Authority_Ref_No], CAST(Blocked_Line AS NVARCHAR(100)) AS [Train_No], CAST(Affected_Section AS NVARCHAR(100)) AS [Section_Location], CAST(Working_Direction AS NVARCHAR(255)) AS [Operational_Reason], CAST(Working_Direction AS NVARCHAR(100)) AS [Speed_Status], 'Section' AS [Issuing_Station], CAST(Control_Approval AS NVARCHAR(100)) AS [Issuing_Officer] FROM [Temporary_Single_Line_Working]"
            });
            registerRegistry.Add(new RegMeta { 
                Code = "AUTH-012", 
                Name = "Shunting Order Management (T/806)", 
                TableName = "Shunting_Order_Management", 
                DateColumn = "Date_Time", 
                SelectQuery = "SELECT CAST(Date_Time AS DATETIME) AS [Record_Timestamp], 'AUTH-012' AS [Authority_Code], 'Shunting Order Management' AS [Register_Name], CAST(Shunting_Order_No AS NVARCHAR(100)) AS [Authority_Ref_No], CAST(Engine_No AS NVARCHAR(100)) AS [Train_No], CAST(Yard_Limits AS NVARCHAR(100)) AS [Section_Location], CAST(Instructions AS NVARCHAR(255)) AS [Operational_Reason], '-' AS [Speed_Status], CAST(Station AS NVARCHAR(100)) AS [Issuing_Station], CAST(Issued_By AS NVARCHAR(100)) AS [Issuing_Officer] FROM [Shunting_Order_Management]"
            });
            registerRegistry.Add(new RegMeta { 
                Code = "AUTH-013", 
                Name = "Signal Passing Authority (Memo)", 
                TableName = "Signal_Passing_Authority", 
                DateColumn = null, 
                SelectQuery = "SELECT CAST(GETDATE() AS DATETIME) AS [Record_Timestamp], 'AUTH-013' AS [Authority_Code], 'Signal Passing Authority' AS [Register_Name], CAST(Authority_Reference_No AS NVARCHAR(100)) AS [Authority_Ref_No], CAST(Train_No AS NVARCHAR(100)) AS [Train_No], CAST(Signal_No AS NVARCHAR(100)) AS [Section_Location], CAST(Reason AS NVARCHAR(255)) AS [Operational_Reason], CAST(Permitted_Speed AS NVARCHAR(100)) AS [Speed_Status], 'Station' AS [Issuing_Station], CAST(Issuing_Officer AS NVARCHAR(100)) AS [Issuing_Officer] FROM [Signal_Passing_Authority]"
            });
            registerRegistry.Add(new RegMeta { 
                Code = "AUTH-014", 
                Name = "ABS Proceed Without Line Clear (T/C 912)", 
                TableName = "ABS_Proceed_Without_Line_Clear", 
                DateColumn = "Date_Time", 
                SelectQuery = "SELECT CAST(Date_Time AS DATETIME) AS [Record_Timestamp], 'AUTH-014' AS [Authority_Code], 'ABS Proceed Without Line Clear' AS [Register_Name], CAST(Authority_Reference_No AS NVARCHAR(100)) AS [Authority_Ref_No], CAST(Train_No AS NVARCHAR(100)) AS [Train_No], CAST(ABS_Section AS NVARCHAR(100)) AS [Section_Location], CAST(Safety_Conditions AS NVARCHAR(255)) AS [Operational_Reason], '-' AS [Speed_Status], 'ABS Section' AS [Issuing_Station], CAST(Issuing_Authority AS NVARCHAR(100)) AS [Issuing_Officer] FROM [ABS_Proceed_Without_Line_Clear]"
            });
            registerRegistry.Add(new RegMeta { 
                Code = "AUTH-015", 
                Name = "ABS Relief Engine Authority (T/A 912)", 
                TableName = "ABS_Relief_Engine_Authority", 
                DateColumn = "Issue_Time", 
                SelectQuery = "SELECT CAST(Issue_Time AS DATETIME) AS [Record_Timestamp], 'AUTH-015' AS [Authority_Code], 'ABS Relief Engine Authority' AS [Register_Name], CAST(Authority_Number AS NVARCHAR(100)) AS [Authority_Ref_No], CAST(Relief_Train_Engine_No AS NVARCHAR(100)) AS [Train_No], CAST(Block_Section AS NVARCHAR(100)) AS [Section_Location], CAST(Reason AS NVARCHAR(255)) AS [Operational_Reason], CAST(Validity_Duration AS NVARCHAR(100)) AS [Speed_Status], 'ABS Section' AS [Issuing_Station], CAST(Issued_By AS NVARCHAR(100)) AS [Issuing_Officer] FROM [ABS_Relief_Engine_Authority]"
            });
            registerRegistry.Add(new RegMeta { 
                Code = "AUTH-016", 
                Name = "ABS Prolonged Signal Failure (T/D 912)", 
                TableName = "ABS_Prolonged_Signal_Failure", 
                DateColumn = "Failure_Start_Time", 
                SelectQuery = "SELECT CAST(Failure_Start_Time AS DATETIME) AS [Record_Timestamp], 'AUTH-016' AS [Authority_Code], 'ABS Prolonged Signal Failure' AS [Register_Name], CAST(Failure_ID AS NVARCHAR(100)) AS [Authority_Ref_No], CAST(Affected_Signals AS NVARCHAR(100)) AS [Train_No], CAST(Affected_Signals AS NVARCHAR(100)) AS [Section_Location], CAST(Caution_Order_Details AS NVARCHAR(255)) AS [Operational_Reason], CAST(Permitted_Speed_Limit AS NVARCHAR(100)) AS [Speed_Status], 'ABS Territory' AS [Issuing_Station], 'Section Controller' AS [Issuing_Officer] FROM [ABS_Prolonged_Signal_Failure]"
            });
            registerRegistry.Add(new RegMeta { 
                Code = "AUTH-017", 
                Name = "Line Clear Inquiry & Reply (T/B 1425)", 
                TableName = "Line_Clear_Inquiry_TFC", 
                DateColumn = "Date_Time_Sent", 
                SelectQuery = "SELECT CAST(Date_Time_Sent AS DATETIME) AS [Record_Timestamp], 'AUTH-017' AS [Authority_Code], 'Line Clear Inquiry Reply' AS [Register_Name], CAST(Inquiry_Reference_No AS NVARCHAR(100)) AS [Authority_Ref_No], CAST(Transmission_Method AS NVARCHAR(100)) AS [Train_No], CAST(Receiving_Station AS NVARCHAR(100)) AS [Section_Location], CAST(Transmission_Method AS NVARCHAR(255)) AS [Operational_Reason], CAST(Line_Clear_Response AS NVARCHAR(100)) AS [Speed_Status], CAST(Sending_Station AS NVARCHAR(100)) AS [Issuing_Station], CAST(Line_Clear_Response AS NVARCHAR(100)) AS [Issuing_Officer] FROM [Line_Clear_Inquiry_TFC]"
            });
            registerRegistry.Add(new RegMeta { 
                Code = "AUTH-018", 
                Name = "Paper Line Clear Tickets (T/A 1425)", 
                TableName = "Line_Clear_Tickets", 
                DateColumn = "Issued_Time", 
                SelectQuery = "SELECT CAST(Issued_Time AS DATETIME) AS [Record_Timestamp], 'AUTH-018' AS [Authority_Code], 'Paper Line Clear Tickets' AS [Register_Name], CAST(Ticket_Number AS NVARCHAR(100)) AS [Authority_Ref_No], CAST(Train_Number AS NVARCHAR(100)) AS [Train_No], CAST(To_Station AS NVARCHAR(100)) AS [Section_Location], CAST(Reply_Status AS NVARCHAR(255)) AS [Operational_Reason], CAST(Direction AS NVARCHAR(100)) AS [Speed_Status], CAST(From_Station AS NVARCHAR(100)) AS [Issuing_Station], 'Station Master' AS [Issuing_Officer] FROM [Line_Clear_Tickets]"
            });
            registerRegistry.Add(new RegMeta { 
                Code = "AUTH-019", 
                Name = "Maintenance Trolley Notice", 
                TableName = "Maintenance_Trolley_Notice", 
                DateColumn = null, 
                SelectQuery = "SELECT CAST(GETDATE() AS DATETIME) AS [Record_Timestamp], 'AUTH-019' AS [Authority_Code], 'Maintenance Trolley Notice' AS [Register_Name], CAST(Notice_ID AS NVARCHAR(100)) AS [Authority_Ref_No], CAST(Type AS NVARCHAR(100)) AS [Train_No], CAST(Section AS NVARCHAR(100)) AS [Section_Location], CAST(Work_Duration AS NVARCHAR(255)) AS [Operational_Reason], CAST(Safety_Precautions AS NVARCHAR(100)) AS [Speed_Status], 'Station' AS [Issuing_Station], CAST(Staff_In_Charge AS NVARCHAR(100)) AS [Issuing_Officer] FROM [Maintenance_Trolley_Notice]"
            });
            registerRegistry.Add(new RegMeta { 
                Code = "AUTH-020", 
                Name = "Motor Trolley Permit (T/1518)", 
                TableName = "Motor_Trolley_Permit", 
                DateColumn = null, 
                SelectQuery = "SELECT CAST(GETDATE() AS DATETIME) AS [Record_Timestamp], 'AUTH-020' AS [Authority_Code], 'Motor Trolley Permit' AS [Register_Name], CAST(Notice_ID AS NVARCHAR(100)) AS [Authority_Ref_No], CAST(Type AS NVARCHAR(100)) AS [Train_No], CAST(Section AS NVARCHAR(100)) AS [Section_Location], CAST(Safety_Precautions AS NVARCHAR(255)) AS [Operational_Reason], '-' AS [Speed_Status], 'Station' AS [Issuing_Station], CAST(Staff_In_Charge AS NVARCHAR(100)) AS [Issuing_Officer] FROM [Motor_Trolley_Permit]"
            });
            registerRegistry.Add(new RegMeta { 
                Code = "AUTH-021", 
                Name = "S&T Disconnection Notice (S&T T/351)", 
                TableName = "ST_Disconnection_Notice", 
                DateColumn = "Disconnection_Time", 
                SelectQuery = "SELECT CAST(Disconnection_Time AS DATETIME) AS [Record_Timestamp], 'AUTH-021' AS [Authority_Code], 'S&T Disconnection Notice' AS [Register_Name], CAST(Memo_Number AS NVARCHAR(100)) AS [Authority_Ref_No], CAST(Equipment_ID AS NVARCHAR(100)) AS [Train_No], CAST(Equipment_ID AS NVARCHAR(100)) AS [Section_Location], CAST(Reason AS NVARCHAR(255)) AS [Operational_Reason], '-' AS [Speed_Status], 'S&T' AS [Issuing_Station], 'ESM / Maintainer' AS [Issuing_Officer] FROM [ST_Disconnection_Notice]"
            });
            registerRegistry.Add(new RegMeta { 
                Code = "AUTH-022", 
                Name = "Train Movement & Station Halt Log", 
                TableName = "Train_Movement_Log", 
                DateColumn = "Scheduled_Arrival_Time", 
                SelectQuery = "SELECT CAST(Scheduled_Arrival_Time AS DATETIME) AS [Record_Timestamp], 'AUTH-022' AS [Authority_Code], 'Train Movement & Station Halt Log' AS [Register_Name], CAST(Train_Number AS NVARCHAR(100)) AS [Authority_Ref_No], CAST(Train_Number AS NVARCHAR(100)) AS [Train_No], CAST(Section_Station AS NVARCHAR(100)) AS [Section_Location], CAST(Train_Name AS NVARCHAR(255)) AS [Operational_Reason], CAST(Running_Status AS NVARCHAR(100)) AS [Speed_Status], CAST(Section_Station AS NVARCHAR(100)) AS [Issuing_Station], 'Duty SM' AS [Issuing_Officer] FROM [Train_Movement_Log]"
            });
        }

        private void InitializeComponent()
        {
            this.Text = "Train Working Management System – Dynamic Operational Reports & Audits";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MinimumSize = new Size(1150, 720);
            this.BackColor = Color.FromArgb(241, 245, 249);

            // ── 1. Top Bar ────────────────────────────────────────────────────
            Panel topBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = ThemeManager.IRCTCColors.PrimaryNavy
            };
            this.Controls.Add(topBar);

            Label lblLogo = new Label
            {
                Text = "📊",
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
                Text = "DYNAMIC OPERATIONAL REPORTS & AUDIT CONSOLE",
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
                Text = "Query, Filter, and Export Railway Operational Authorities Across All 22 Registers (Max 3 Days Window)",
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
            Label lblUser = new Label
            {
                Text = $"👤  {userName} ({deptName})",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(254, 240, 138),
                Location = new Point(12, 10),
                AutoSize = true,
                UseMnemonic = false,
                BackColor = Color.Transparent
            };
            userChip.Controls.Add(lblUser);
            topBar.Controls.Add(userChip);

            Button btnBack = new Button
            {
                Text = "🚪 Exit to Hub",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                BackColor = ThemeManager.IRCTCColors.DangerRed,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(130, 38),
                Cursor = Cursors.Hand
            };
            btnBack.FlatAppearance.BorderSize = 0;
            btnBack.Click += (s, e) => this.Close();
            topBar.Controls.Add(btnBack);

            Action layoutTopBar = () =>
            {
                int r = topBar.ClientSize.Width - 18;
                btnBack.Location = new Point(r - 130, 16);
                r -= 145;

                int chipWidth = TextRenderer.MeasureText(lblUser.Text, lblUser.Font).Width + 24;
                userChip.Size = new Size(chipWidth, 40);
                userChip.Location = new Point(r - chipWidth, 15);
            };
            topBar.Resize += (s, e) => layoutTopBar();
            this.Shown += (s, e) => layoutTopBar();

            // ── 2. Filter Ribbon Control Bar ─────────────────────────────────
            Panel filterBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 84,
                BackColor = Color.White,
                Padding = new Padding(16, 10, 16, 10)
            };
            filterBar.Paint += (s, e) =>
            {
                using (Pen p = new Pen(Color.FromArgb(226, 232, 240), 1.5f))
                    e.Graphics.DrawLine(p, 0, filterBar.Height - 1, filterBar.Width, filterBar.Height - 1);
            };
            this.Controls.Add(filterBar);

            // Register Dropdown Selector
            Label lblRegSelect = new Label
            {
                Text = "Select Authority Registers:",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(16, 12),
                AutoSize = true
            };
            filterBar.Controls.Add(lblRegSelect);

            regSelector = new CheckedRegisterSelector
            {
                Location = new Point(16, 36),
                Size = new Size(360, 32)
            };
            regSelector.SelectionChanged += (s, e) => { if (!isInitializing) ExecuteQuery(); };
            filterBar.Controls.Add(regSelector);

            // Date Range Presets (Max 3 Days)
            Label lblPresets = new Label
            {
                Text = "Quick Presets:",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(390, 12),
                AutoSize = true
            };
            filterBar.Controls.Add(lblPresets);

            btnPresetToday = CreateFilterButton("Today", 390, 36, 75, () => ApplyPreset(1));
            btnPresetYesterday = CreateFilterButton("Yesterday & Today", 470, 36, 140, () => ApplyPreset(2));
            btnPreset3Days = CreateFilterButton("Last 3 Days (Max)", 615, 36, 135, () => ApplyPreset(3));
            filterBar.Controls.AddRange(new Control[] { btnPresetToday, btnPresetYesterday, btnPreset3Days });

            // Date Range Inputs (Max 3 Days Window)
            Label lblDateRange = new Label
            {
                Text = "Custom Date Range (Max 3 Days Window):",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(765, 12),
                AutoSize = true
            };
            filterBar.Controls.Add(lblDateRange);

            dtpFrom = new DateTimePicker
            {
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd-MMM-yyyy",
                Font = new Font("Segoe UI", 9.5f),
                Location = new Point(765, 36),
                Size = new Size(125, 32)
            };
            dtpFrom.ValueChanged += (s, e) => { if (!isInitializing) ValidateAndRunDateChange(true); };
            filterBar.Controls.Add(dtpFrom);

            Label lblTo = new Label
            {
                Text = "to",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(895, 41),
                AutoSize = true
            };
            filterBar.Controls.Add(lblTo);

            dtpTo = new DateTimePicker
            {
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd-MMM-yyyy",
                Font = new Font("Segoe UI", 9.5f),
                Location = new Point(920, 36),
                Size = new Size(125, 32)
            };
            dtpTo.ValueChanged += (s, e) => { if (!isInitializing) ValidateAndRunDateChange(false); };
            filterBar.Controls.Add(dtpTo);

            // ── 3. Bottom Action / Export Bar ────────────────────────────────
            Panel bottomBar = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = Color.White,
                Padding = new Padding(16, 10, 16, 10)
            };
            bottomBar.Paint += (s, e) =>
            {
                using (Pen p = new Pen(Color.FromArgb(226, 232, 240), 1.5f))
                    e.Graphics.DrawLine(p, 0, 0, bottomBar.Width, 0);
            };
            this.Controls.Add(bottomBar);

            lblTotalRecords = new Label
            {
                Text = "Records Loaded: 0",
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = ThemeManager.IRCTCColors.PrimaryNavy,
                Location = new Point(16, 18),
                AutoSize = true
            };
            bottomBar.Controls.Add(lblTotalRecords);

            Button btnExportPdf = new Button
            {
                Text = "📄 Export to PDF",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                BackColor = Color.FromArgb(220, 38, 38), // Crimson Red
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(170, 38),
                Cursor = Cursors.Hand
            };
            btnExportPdf.FlatAppearance.BorderSize = 0;
            btnExportPdf.Click += (s, e) => ExportToPdf();
            bottomBar.Controls.Add(btnExportPdf);

            Button btnExportExcel = new Button
            {
                Text = "📊 Export to Excel",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                BackColor = Color.FromArgb(22, 163, 74), // Emerald Green
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(170, 38),
                Cursor = Cursors.Hand
            };
            btnExportExcel.FlatAppearance.BorderSize = 0;
            btnExportExcel.Click += (s, e) => ExportToExcel();
            bottomBar.Controls.Add(btnExportExcel);

            Button btnExportCsv = new Button
            {
                Text = "📋 Export to CSV",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                BackColor = ThemeManager.IRCTCColors.PrimaryNavy,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(170, 38),
                Cursor = Cursors.Hand
            };
            btnExportCsv.FlatAppearance.BorderSize = 0;
            btnExportCsv.Click += (s, e) => ExportToCsv();
            bottomBar.Controls.Add(btnExportCsv);

            Action layoutBottomBar = () =>
            {
                int r = bottomBar.ClientSize.Width - 18;
                btnExportCsv.Size = new Size(170, 38);
                btnExportCsv.Location = new Point(r - 170, 11);
                r -= 182;
                btnExportExcel.Size = new Size(170, 38);
                btnExportExcel.Location = new Point(r - 170, 11);
                r -= 182;
                btnExportPdf.Size = new Size(170, 38);
                btnExportPdf.Location = new Point(r - 170, 11);
            };
            bottomBar.Resize += (s, e) => layoutBottomBar();
            this.Shown += (s, e) => layoutBottomBar();

            // ── 4. Main Body & Data Table ─────────────────────────────────────
            Panel bodyPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 250, 252),
                Padding = new Padding(16, 12, 16, 8)
            };
            this.Controls.Add(bodyPanel);

            // Search Bar Sub-Panel
            Panel searchPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 44,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 0, 0, 8)
            };
            bodyPanel.Controls.Add(searchPanel);

            Label lblSearch = new Label
            {
                Text = "🔍 Quick Grid Search / Filter:",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = ThemeManager.IRCTCColors.PrimaryNavy,
                Location = new Point(0, 8),
                AutoSize = true
            };
            searchPanel.Controls.Add(lblSearch);

            txtSearch = new TextBox
            {
                Font = new Font("Segoe UI", 10f),
                BorderStyle = BorderStyle.FixedSingle,
                Location = new Point(220, 4),
                Size = new Size(380, 28)
            };
            txtSearch.TextChanged += (s, e) => FilterCurrentGrid(txtSearch.Text);
            searchPanel.Controls.Add(txtSearch);

            // DataGridView Setup
            dgvReport = new TouchpadScrollableDataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                EnableHeadersVisualStyles = false,
                Font = new Font("Segoe UI", 9.5f),
                RowTemplate = { Height = 34 }
            };

            dgvReport.ColumnHeadersDefaultCellStyle.BackColor = ThemeManager.IRCTCColors.PrimaryNavy;
            dgvReport.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvReport.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            dgvReport.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvReport.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 8, 0);
            dgvReport.ColumnHeadersHeight = 48;
            dgvReport.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            dgvReport.DefaultCellStyle.BackColor = Color.White;
            dgvReport.DefaultCellStyle.ForeColor = Color.FromArgb(15, 23, 42);
            dgvReport.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            dgvReport.DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 58, 138);
            dgvReport.DefaultCellStyle.Padding = new Padding(8, 0, 8, 0);

            dgvReport.AlternatingRowsDefaultCellStyle.BackColor = ThemeManager.IRCTCColors.AltRowBg;
            dgvReport.GridColor = Color.FromArgb(226, 232, 240);

            bodyPanel.Controls.Add(dgvReport);
            dgvReport.BringToFront();

            // Docking order
            bottomBar.SendToBack();
            filterBar.SendToBack();
            topBar.SendToBack();
        }

        private Button CreateFilterButton(string text, int x, int y, int w, Action onClick)
        {
            Button btn = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(w, 32),
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                BackColor = Color.FromArgb(241, 245, 249),
                ForeColor = ThemeManager.IRCTCColors.SlateText,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderColor = ThemeManager.IRCTCColors.BorderGray;
            btn.Click += (s, e) => onClick();
            return btn;
        }

        private void ApplyPreset(int days)
        {
            isInitializing = true;
            DateTime end = DateTime.Today;
            DateTime start = end.AddDays(-(days - 1));

            dtpFrom.Value = start;
            dtpTo.Value = end;

            UpdatePresetButtonStyles(days);

            isInitializing = false;
            ExecuteQuery();
        }

        private void UpdatePresetButtonStyles(int activeDays = 0)
        {
            btnPresetToday.BackColor = (activeDays == 1) ? ThemeManager.IRCTCColors.PrimaryNavy : Color.FromArgb(241, 245, 249);
            btnPresetToday.ForeColor = (activeDays == 1) ? Color.White : ThemeManager.IRCTCColors.SlateText;

            btnPresetYesterday.BackColor = (activeDays == 2) ? ThemeManager.IRCTCColors.PrimaryNavy : Color.FromArgb(241, 245, 249);
            btnPresetYesterday.ForeColor = (activeDays == 2) ? Color.White : ThemeManager.IRCTCColors.SlateText;

            btnPreset3Days.BackColor = (activeDays == 3) ? ThemeManager.IRCTCColors.PrimaryNavy : Color.FromArgb(241, 245, 249);
            btnPreset3Days.ForeColor = (activeDays == 3) ? Color.White : ThemeManager.IRCTCColors.SlateText;
        }

        private void ValidateAndRunDateChange(bool fromChanged = true)
        {
            if (dtpFrom.Value.Date > dtpTo.Value.Date)
            {
                isInitializing = true;
                if (fromChanged)
                    dtpTo.Value = dtpFrom.Value;
                else
                    dtpFrom.Value = dtpTo.Value;
                isInitializing = false;
            }

            TimeSpan diff = dtpTo.Value.Date - dtpFrom.Value.Date;
            if (diff.TotalDays > 2)
            {
                isInitializing = true;
                if (fromChanged)
                    dtpTo.Value = dtpFrom.Value.Date.AddDays(2);
                else
                    dtpFrom.Value = dtpTo.Value.Date.AddDays(-2);
                isInitializing = false;
            }

            int activeDays = 0;
            if (dtpTo.Value.Date == DateTime.Today)
            {
                int days = (int)(dtpTo.Value.Date - dtpFrom.Value.Date).TotalDays + 1;
                if (days == 1 || days == 2 || days == 3)
                    activeDays = days;
            }
            UpdatePresetButtonStyles(activeDays);

            ExecuteQuery();
        }

        private void ExecuteQuery()
        {
            if (isInitializing) return;

            DateTime fromDate = dtpFrom.Value.Date;
            DateTime toDate = dtpTo.Value.Date.AddDays(1).AddSeconds(-1);

            List<string> selectedCodes = regSelector.GetSelectedCodes();

            if (selectedCodes.Count == 0)
            {
                currentData = new DataTable();
                dgvReport.DataSource = currentData;
                lblTotalRecords.Text = "Records Loaded: 0 (No Registers Selected)";
                return;
            }

            try
            {
                Cursor.Current = Cursors.WaitCursor;

                // Case 1: Single Register Selected -> Show its specific columns
                if (selectedCodes.Count == 1)
                {
                    RegMeta meta = registerRegistry.FirstOrDefault(x => x.Code == selectedCodes[0]);
                    if (meta != null)
                    {
                        string query;
                        if (!string.IsNullOrEmpty(meta.DateColumn))
                        {
                            query = $@"
                                SELECT * FROM [{meta.TableName}]
                                WHERE [{meta.DateColumn}] >= @from AND [{meta.DateColumn}] <= @to
                                ORDER BY [{meta.DateColumn}] DESC";
                        }
                        else
                        {
                            query = $"SELECT * FROM [{meta.TableName}]";
                        }

                        using (SqlCommand cmd = new SqlCommand(query))
                        {
                            if (!string.IsNullOrEmpty(meta.DateColumn))
                            {
                                cmd.Parameters.AddWithValue("@from", fromDate);
                                cmd.Parameters.AddWithValue("@to", toDate);
                            }
                            currentData = db.ExecuteQuery(cmd);
                        }

                        dgvReport.DataSource = currentData;
                        FormatSpecificGrid();
                        lblTotalRecords.Text = $"Records Loaded: {currentData.Rows.Count}  •  {meta.Code}: {meta.Name} ({fromDate:dd-MMM} to {dtpTo.Value:dd-MMM-yyyy})";
                        return;
                    }
                }

                // Case 2: Multi-Register Consolidated View
                var selectedMetas = registerRegistry.Where(r => selectedCodes.Contains(r.Code)).ToList();
                List<string> unionParts = new List<string>();

                foreach (var meta in selectedMetas)
                {
                    string part;
                    if (!string.IsNullOrEmpty(meta.DateColumn))
                    {
                        part = meta.SelectQuery + $" WHERE [{meta.DateColumn}] >= @from AND [{meta.DateColumn}] <= @to";
                    }
                    else
                    {
                        part = meta.SelectQuery;
                    }
                    unionParts.Add(part);
                }

                string fullQuery = string.Join("\nUNION ALL\n", unionParts) + "\nORDER BY [Record_Timestamp] DESC";

                using (SqlCommand cmd = new SqlCommand(fullQuery))
                {
                    cmd.Parameters.AddWithValue("@from", fromDate);
                    cmd.Parameters.AddWithValue("@to", toDate);
                    currentData = db.ExecuteQuery(cmd);
                }

                dgvReport.DataSource = currentData;
                FormatConsolidatedGrid();

                string filterScope = selectedCodes.Count == registerRegistry.Count ? "ALL 22 REGISTERS" : $"{selectedCodes.Count} REGISTERS";
                lblTotalRecords.Text = $"Total Operational Records: {currentData.Rows.Count}  •  Scope: {filterScope} ({fromDate:dd-MMM} to {dtpTo.Value:dd-MMM-yyyy})";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading report data: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void FormatConsolidatedGrid()
        {
            if (dgvReport.Columns["Record_Timestamp"] != null)
            {
                dgvReport.Columns["Record_Timestamp"].HeaderText = "Timestamp";
                dgvReport.Columns["Record_Timestamp"].DefaultCellStyle.Format = "dd-MMM-yyyy HH:mm:ss";
                dgvReport.Columns["Record_Timestamp"].Width = 160;
            }
            if (dgvReport.Columns["Authority_Code"] != null)
            {
                dgvReport.Columns["Authority_Code"].HeaderText = "Code";
                dgvReport.Columns["Authority_Code"].Width = 95;
            }
            if (dgvReport.Columns["Register_Name"] != null)
            {
                dgvReport.Columns["Register_Name"].HeaderText = "Authority Register Name";
                dgvReport.Columns["Register_Name"].Width = 260;
            }
            if (dgvReport.Columns["Authority_Ref_No"] != null)
            {
                dgvReport.Columns["Authority_Ref_No"].HeaderText = "Ref / Order No";
                dgvReport.Columns["Authority_Ref_No"].Width = 175;
            }
            if (dgvReport.Columns["Train_No"] != null)
            {
                dgvReport.Columns["Train_No"].HeaderText = "Train / Gear";
                dgvReport.Columns["Train_No"].Width = 120;
            }
            if (dgvReport.Columns["Section_Location"] != null)
            {
                dgvReport.Columns["Section_Location"].HeaderText = "Location / Line";
                dgvReport.Columns["Section_Location"].Width = 160;
            }
            if (dgvReport.Columns["Operational_Reason"] != null)
            {
                dgvReport.Columns["Operational_Reason"].HeaderText = "Operational Context / Reason";
                dgvReport.Columns["Operational_Reason"].Width = 240;
            }
            if (dgvReport.Columns["Speed_Status"] != null)
            {
                dgvReport.Columns["Speed_Status"].HeaderText = "Speed / PN / Status";
                dgvReport.Columns["Speed_Status"].Width = 150;
            }
            if (dgvReport.Columns["Issuing_Station"] != null)
            {
                dgvReport.Columns["Issuing_Station"].HeaderText = "Station";
                dgvReport.Columns["Issuing_Station"].Width = 120;
            }
            if (dgvReport.Columns["Issuing_Officer"] != null)
            {
                dgvReport.Columns["Issuing_Officer"].HeaderText = "Duty Staff";
                dgvReport.Columns["Issuing_Officer"].Width = 140;
            }
        }

        private void FormatSpecificGrid()
        {
            foreach (DataGridViewColumn col in dgvReport.Columns)
            {
                col.HeaderText = col.HeaderText.Replace("_", " ");
                if (col.ValueType == typeof(DateTime))
                {
                    col.DefaultCellStyle.Format = "dd-MMM-yyyy HH:mm:ss";
                    col.Width = 150;
                }
                else
                {
                    col.Width = 140;
                }
            }
        }

        private void FilterCurrentGrid(string filterText)
        {
            if (currentData == null) return;
            try
            {
                if (string.IsNullOrWhiteSpace(filterText))
                {
                    currentData.DefaultView.RowFilter = string.Empty;
                }
                else
                {
                    List<string> subQueries = new List<string>();
                    string clean = filterText.Trim().Replace("'", "''");
                    foreach (DataColumn col in currentData.Columns)
                    {
                        if (col.DataType == typeof(string))
                        {
                            subQueries.Add($"[{col.ColumnName}] LIKE '%{clean}%'");
                        }
                    }
                    currentData.DefaultView.RowFilter = string.Join(" OR ", subQueries);
                }
                lblTotalRecords.Text = $"Showing {currentData.DefaultView.Count} of {currentData.Rows.Count} matching records";
            }
            catch { }
        }

        #region Exports (PDF, Excel, CSV)

        private class ExportColDef
        {
            public string Header { get; set; }
            public string ColumnName { get; set; }
            public int Width { get; set; }
        }

        private void ExportToPdf()
        {
            if (currentData == null || currentData.DefaultView.Count == 0)
            {
                MessageBox.Show("No records available to export in the current query view.", "Export PDF", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "PDF Document (*.pdf)|*.pdf",
                FileName = $"TrainWorking_Audit_{DateTime.Now:yyyyMMdd_HHmmss}.pdf"
            };

            if (sfd.ShowDialog() != DialogResult.OK) return;

            try
            {
                Cursor.Current = Cursors.WaitCursor;
                string pdfPath = sfd.FileName;
                DataView view = currentData.DefaultView;
                int rowCount = view.Count;

                int pageWidth = 842;  // A4 Landscape
                int pageHeight = 595;
                int usableWidth = pageWidth - 40; // 802pt

                // Determine Columns to Export
                List<ExportColDef> exportCols = new List<ExportColDef>();
                List<string> selectedCodes = regSelector.GetSelectedCodes();

                if (selectedCodes.Count == 1 && currentData.Columns.Count > 0)
                {
                    // Single Register View: Dynamic columns from specific table
                    int colCount = currentData.Columns.Count;
                    int baseWidth = usableWidth / colCount;
                    int rem = usableWidth % colCount;

                    for (int c = 0; c < colCount; c++)
                    {
                        var col = currentData.Columns[c];
                        int w = baseWidth + (c == colCount - 1 ? rem : 0);
                        exportCols.Add(new ExportColDef
                        {
                            Header = col.ColumnName.Replace("_", " ").ToUpper(),
                            ColumnName = col.ColumnName,
                            Width = w
                        });
                    }
                }
                else
                {
                    // Consolidated View: Full 10 Operational Attributes
                    exportCols = new List<ExportColDef>
                    {
                        new ExportColDef { Header = "TIME", ColumnName = "Record_Timestamp", Width = 75 },
                        new ExportColDef { Header = "CODE", ColumnName = "Authority_Code", Width = 50 },
                        new ExportColDef { Header = "AUTHORITY REGISTER", ColumnName = "Register_Name", Width = 130 },
                        new ExportColDef { Header = "REF / ORDER NO", ColumnName = "Authority_Ref_No", Width = 95 },
                        new ExportColDef { Header = "TRAIN/GEAR", ColumnName = "Train_No", Width = 65 },
                        new ExportColDef { Header = "LOCATION/LINE", ColumnName = "Section_Location", Width = 85 },
                        new ExportColDef { Header = "OPERATIONAL REASON", ColumnName = "Operational_Reason", Width = 132 },
                        new ExportColDef { Header = "SPEED/STATUS", ColumnName = "Speed_Status", Width = 60 },
                        new ExportColDef { Header = "STATION", ColumnName = "Issuing_Station", Width = 55 },
                        new ExportColDef { Header = "DUTY STAFF", ColumnName = "Issuing_Officer", Width = 55 }
                    };
                }

                int rowsPerPage = 18;
                int totalPages = (int)Math.Ceiling((double)rowCount / rowsPerPage);
                if (totalPages < 1) totalPages = 1;

                List<string> pageContents = new List<string>();

                for (int pageIdx = 0; pageIdx < totalPages; pageIdx++)
                {
                    StringBuilder sb = new StringBuilder();

                    // Header Navy Banner
                    sb.AppendLine("0.129 0.239 0.467 rg");
                    sb.AppendLine($"20 520 {usableWidth} 60 re f");

                    // Orange accent
                    sb.AppendLine("0.984 0.475 0.169 rg");
                    sb.AppendLine($"20 515 {usableWidth} 5 re f");

                    // Title Text
                    sb.AppendLine("BT");
                    sb.AppendLine("/F1 13 Tf");
                    sb.AppendLine("1 1 1 rg");
                    sb.AppendLine("1 0 0 1 35 555 Tm");
                    sb.AppendLine("(INDIAN RAILWAYS - TRAIN WORKING OPERATIONAL AUDIT REPORT) Tj");
                    sb.AppendLine("1 0 0 1 35 538 Tm");
                    sb.AppendLine("/F2 9 Tf");
                    sb.AppendLine($"({EscapePdf($"Generated: {DateTime.Now:dd-MMM-yyyy HH:mm:ss} | Query Window: {dtpFrom.Value:dd-MMM-yyyy} to {dtpTo.Value:dd-MMM-yyyy} | Records: {rowCount} | Page {pageIdx + 1} of {totalPages}")}) Tj");
                    sb.AppendLine("ET");

                    // Table Header
                    int tableTop = 485;
                    sb.AppendLine("0.129 0.239 0.467 rg");
                    sb.AppendLine($"20 {tableTop} {usableWidth} 22 re f");

                    sb.AppendLine("BT");
                    sb.AppendLine("/F1 7.5 Tf");
                    sb.AppendLine("1 1 1 rg");

                    int xCursor = 24;
                    foreach (var col in exportCols)
                    {
                        int maxHChars = Math.Max(4, (col.Width - 6) / 5);
                        string hText = Trunc(col.Header, maxHChars);
                        sb.AppendLine($"1 0 0 1 {xCursor} {tableTop + 7} Tm");
                        sb.AppendLine($"({EscapePdf(hText)}) Tj");
                        xCursor += col.Width;
                    }
                    sb.AppendLine("ET");

                    // Rows for this page
                    int curY = tableTop - 20;
                    int rowH = 20;
                    int startRow = pageIdx * rowsPerPage;
                    int endRow = Math.Min(rowCount, (pageIdx + 1) * rowsPerPage);

                    for (int r = startRow; r < endRow; r++)
                    {
                        DataRowView rowView = view[r];

                        if (r % 2 == 1)
                        {
                            sb.AppendLine("0.96 0.97 0.98 rg");
                            sb.AppendLine($"20 {curY} {usableWidth} {rowH} re f");
                        }

                        sb.AppendLine("0.88 0.91 0.94 RG");
                        sb.AppendLine("0.5 w");
                        sb.AppendLine($"20 {curY} m {pageWidth - 20} {curY} l S");

                        sb.AppendLine("BT");
                        sb.AppendLine("/F2 7.5 Tf");
                        sb.AppendLine("0.1 0.15 0.2 rg");

                        xCursor = 24;
                        foreach (var col in exportCols)
                        {
                            string rawVal = "-";
                            if (rowView.Row.Table.Columns.Contains(col.ColumnName))
                            {
                                object o = rowView[col.ColumnName];
                                if (o != null && o != DBNull.Value)
                                {
                                    if (o is DateTime dt)
                                        rawVal = dt.ToString("dd-MMM HH:mm");
                                    else
                                        rawVal = o.ToString();
                                }
                            }

                            int maxVChars = Math.Max(4, (col.Width - 6) / 5);
                            string cellText = Trunc(rawVal, maxVChars);

                            sb.AppendLine($"1 0 0 1 {xCursor} {curY + 6} Tm");
                            sb.AppendLine($"({EscapePdf(cellText)}) Tj");
                            xCursor += col.Width;
                        }
                        sb.AppendLine("ET");

                        curY -= rowH;
                    }

                    // Footer
                    sb.AppendLine("BT");
                    sb.AppendLine("/F2 8 Tf");
                    sb.AppendLine("0.4 0.45 0.5 rg");
                    sb.AppendLine("1 0 0 1 25 25 Tm");
                    sb.AppendLine($"({EscapePdf($"Certified Official Railway Audit Document | Total Matches: {rowCount} | Page {pageIdx + 1} of {totalPages} | Printed by: {SessionManager.CurrentFullName}")}) Tj");
                    sb.AppendLine("ET");

                    pageContents.Add(sb.ToString());
                }

                // Assemble Multi-Page PDF 1.4 Binary
                using (FileStream fs = new FileStream(pdfPath, FileMode.Create, FileAccess.Write))
                using (StreamWriter sw = new StreamWriter(fs, Encoding.ASCII))
                {
                    int font1ObjId = 3 + 2 * totalPages;
                    int font2ObjId = 4 + 2 * totalPages;

                    List<string> kids = new List<string>();
                    for (int i = 0; i < totalPages; i++) kids.Add($"{3 + 2 * i} 0 R");

                    List<string> pdfObjects = new List<string>();
                    pdfObjects.Add("<< /Type /Catalog /Pages 2 0 R >>");
                    pdfObjects.Add($"<< /Type /Pages /Kids [{string.Join(" ", kids)}] /Count {totalPages} >>");

                    for (int i = 0; i < totalPages; i++)
                    {
                        string contentStr = pageContents[i];
                        byte[] contentBytes = Encoding.ASCII.GetBytes(contentStr);
                        int streamObjId = 4 + 2 * i;

                        pdfObjects.Add($"<< /Type /Page /Parent 2 0 R /MediaBox [0 0 {pageWidth} {pageHeight}] /Contents {streamObjId} 0 R /Resources << /Font << /F1 {font1ObjId} 0 R /F2 {font2ObjId} 0 R >> >> >>");
                        pdfObjects.Add($"<< /Length {contentBytes.Length} >>\nstream\n{contentStr}\nendstream");
                    }

                    pdfObjects.Add("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold >>");
                    pdfObjects.Add("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>");

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

                if (MessageBox.Show("Audit PDF report generated successfully.\nWould you like to open it now?", "Export Success", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo { FileName = pdfPath, UseShellExecute = true });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("PDF export error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void ExportToExcel()
        {
            if (currentData == null || currentData.DefaultView.Count == 0)
            {
                MessageBox.Show("No records available to export.", "Export Excel", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "Excel Spreadsheet (*.xls)|*.xls",
                FileName = $"TrainWorking_Audit_{DateTime.Now:yyyyMMdd_HHmmss}.xls"
            };

            if (sfd.ShowDialog() != DialogResult.OK) return;

            try
            {
                Cursor.Current = Cursors.WaitCursor;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("<?xml version=\"1.0\"?>");
                sb.AppendLine("<?mso-application progid=\"Excel.Sheet\"?>");
                sb.AppendLine("<Workbook xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\"");
                sb.AppendLine(" xmlns:o=\"urn:schemas-microsoft-com:office:office\"");
                sb.AppendLine(" xmlns:x=\"urn:schemas-microsoft-com:office:excel\"");
                sb.AppendLine(" xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\">");
                sb.AppendLine(" <Styles>");
                sb.AppendLine("  <Style ss:ID=\"Default\" ss:Name=\"Normal\"><Font ss:FontName=\"Segoe UI\" ss:Size=\"10\"/></Style>");
                sb.AppendLine("  <Style ss:ID=\"HeaderStyle\"><Font ss:FontName=\"Segoe UI\" ss:Size=\"10\" ss:Color=\"#FFFFFF\" ss:Bold=\"1\"/><Interior ss:Color=\"#213D77\" ss:Pattern=\"Solid\"/><Alignment ss:Horizontal=\"Center\" ss:Vertical=\"Center\"/></Style>");
                sb.AppendLine(" </Styles>");
                sb.AppendLine(" <Worksheet ss:Name=\"Operational Audit\">");
                sb.AppendLine("  <Table>");

                DataView view = currentData.DefaultView;

                // Header Row
                sb.AppendLine("   <Row ss:Height=\"24\">");
                foreach (DataColumn col in currentData.Columns)
                {
                    sb.AppendLine($"    <Cell ss:StyleID=\"HeaderStyle\"><Data ss:Type=\"String\">{EscapeXml(col.ColumnName.Replace("_", " "))}</Data></Cell>");
                }
                sb.AppendLine("   </Row>");

                // Data Rows
                foreach (DataRowView r in view)
                {
                    sb.AppendLine("   <Row ss:Height=\"20\">");
                    foreach (DataColumn col in currentData.Columns)
                    {
                        object val = r[col.ColumnName];
                        string valStr = val == DBNull.Value ? "" : (val is DateTime dt ? dt.ToString("dd-MMM-yyyy HH:mm:ss") : val.ToString());
                        sb.AppendLine($"    <Cell><Data ss:Type=\"String\">{EscapeXml(valStr)}</Data></Cell>");
                    }
                    sb.AppendLine("   </Row>");
                }

                sb.AppendLine("  </Table>");
                sb.AppendLine(" </Worksheet>");
                sb.AppendLine("</Workbook>");

                File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);

                if (MessageBox.Show("Excel report generated successfully.\nWould you like to open it now?", "Export Success", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo { FileName = sfd.FileName, UseShellExecute = true });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Excel export error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void ExportToCsv()
        {
            if (currentData == null || currentData.DefaultView.Count == 0)
            {
                MessageBox.Show("No records available to export.", "Export CSV", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "CSV File (*.csv)|*.csv",
                FileName = $"TrainWorking_Audit_{DateTime.Now:yyyyMMdd_HHmmss}.csv"
            };

            if (sfd.ShowDialog() != DialogResult.OK) return;

            try
            {
                Cursor.Current = Cursors.WaitCursor;
                StringBuilder sb = new StringBuilder();
                DataView view = currentData.DefaultView;

                // Header
                List<string> headers = new List<string>();
                foreach (DataColumn col in currentData.Columns)
                    headers.Add($"\"{col.ColumnName.Replace("_", " ")}\"");
                sb.AppendLine(string.Join(",", headers));

                // Rows
                foreach (DataRowView r in view)
                {
                    List<string> fields = new List<string>();
                    foreach (DataColumn col in currentData.Columns)
                    {
                        object val = r[col.ColumnName];
                        string valStr = val == DBNull.Value ? "" : (val is DateTime dt ? dt.ToString("dd-MMM-yyyy HH:mm:ss") : val.ToString());
                        fields.Add($"\"{valStr.Replace("\"", "\"\"")}\"");
                    }
                    sb.AppendLine(string.Join(",", fields));
                }

                File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);

                if (MessageBox.Show("CSV audit file generated successfully.\nWould you like to open it now?", "Export Success", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo { FileName = sfd.FileName, UseShellExecute = true });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("CSV export error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private static string EscapePdf(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            return input.Replace("\\", "\\\\").Replace("(", "\\(").Replace(")", "\\)");
        }

        private static string EscapeXml(string input)
        {
            if (string.IsNullOrEmpty(input)) return "";
            return input.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");
        }

        private static string Trunc(string input, int maxLen)
        {
            if (string.IsNullOrEmpty(input)) return "-";
            return input.Length <= maxLen ? input : input.Substring(0, maxLen - 2) + "..";
        }

        #endregion
    }
}
