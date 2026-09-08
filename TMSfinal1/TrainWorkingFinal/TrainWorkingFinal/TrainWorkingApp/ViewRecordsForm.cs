using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TrainWorkingApp
{
    public class ViewRecordsForm : Form
    {
        private DataGridView dgv;
        private Label lblCount;
        private TextBox txtSearch;
        private DataTable fullData;
        private readonly string tableName;
        private readonly string formTitle;

        public ViewRecordsForm(string tableName, string title)
        {
            this.tableName = tableName;
            this.formTitle = title;
            BuildUI();
            LoadData();
        }

        private void BuildUI()
        {
            this.Text = formTitle + " – Saved Records";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = new Size(1000, 650);
            this.BackColor = Color.FromArgb(241, 245, 249);
            this.Font = new Font("Segoe UI", 10F);

            Panel topHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 102,
                BackColor = Color.White
            };

            Panel titleBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 48,
                BackColor = ThemeManager.IRCTCColors.PrimaryNavy
            };
            var lblT = new Label
            {
                Text = formTitle + "  —  View Records",
                Font = new Font("Segoe UI", 12.5F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(16, 12),
                AutoSize = true,
                UseMnemonic = false
            };
            titleBar.Controls.Add(lblT);

            string deptName = string.IsNullOrWhiteSpace(SessionManager.CurrentDepartment) ? "Operating" : SessionManager.CurrentDepartment;
            string userName = string.IsNullOrWhiteSpace(SessionManager.CurrentFullName) ? "Station Operator" : SessionManager.CurrentFullName;

            var lblUserBadge = new Label
            {
                Text = $"👤  {userName} ({deptName})",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(254, 240, 138), // Gold
                AutoSize = true,
                UseMnemonic = false,
                BackColor = Color.Transparent
            };
            titleBar.Controls.Add(lblUserBadge);

            Action layoutTitle = () =>
            {
                lblUserBadge.Location = new Point(titleBar.ClientSize.Width - lblUserBadge.Width - 18, 14);
            };
            titleBar.Resize += (s, e) => layoutTitle();
            this.Shown += (s, e) => layoutTitle();

            topHeader.Controls.Add(titleBar);

            Panel toolbar = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };
            toolbar.Paint += (s, e) => e.Graphics.DrawLine(
                new Pen(Color.FromArgb(203, 213, 225)), 0, toolbar.Height - 1, toolbar.Width, toolbar.Height - 1);

            var lblS = new Label
            {
                Text = "Search Records:",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                AutoSize = true,
                Location = new Point(16, 14)
            };
            toolbar.Controls.Add(lblS);

            txtSearch = new TextBox
            {
                Font = new Font("Segoe UI", 10.5F),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 250, 252),
                ForeColor = Color.FromArgb(15, 23, 42),
                Size = new Size(320, 28),
                Location = new Point(135, 11)
            };
            txtSearch.TextChanged += OnSearch;
            toolbar.Controls.Add(txtSearch);

            lblCount = new Label
            {
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(5, 150, 105),
                AutoSize = true,
                Location = new Point(475, 14)
            };
            toolbar.Controls.Add(lblCount);

            var tip = new Label
            {
                Text = "💡 Tip: Double-click any row to view complete record details",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = true,
                Location = new Point(720, 15)
            };
            toolbar.Controls.Add(tip);
            topHeader.Controls.Add(toolbar);
            toolbar.BringToFront();

            Panel footer = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 56,
                BackColor = Color.White
            };
            footer.Paint += (s, e) => e.Graphics.DrawLine(
                new Pen(Color.FromArgb(203, 213, 225)), 0, 0, footer.Width, 0);

            var btnRefresh = new Button
            {
                Text = "🔄  Refresh",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                BackColor = Color.FromArgb(241, 245, 249),
                ForeColor = Color.FromArgb(30, 41, 59),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(110, 36),
                Location = new Point(16, 10),
                Cursor = Cursors.Hand
            };
            btnRefresh.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnRefresh.Click += (s, e) => LoadData();
            footer.Controls.Add(btnRefresh);

            var btnClose = new Button
            {
                Text = "✕  Close",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                BackColor = Color.FromArgb(241, 245, 249),
                ForeColor = Color.FromArgb(220, 38, 38),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(95, 36),
                Location = new Point(136, 10),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnClose.Click += (s, e) => this.Close();
            footer.Controls.Add(btnClose);

            Panel gridCard = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(16, 14, 16, 14),
                BackColor = Color.FromArgb(241, 245, 249)
            };

            dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = Color.FromArgb(241, 245, 249),
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells,
                ScrollBars = ScrollBars.Both,
                EnableHeadersVisualStyles = false
            };

            dgv.ColumnHeadersDefaultCellStyle.BackColor = ThemeManager.IRCTCColors.PrimaryNavy;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(8, 0, 8, 0);
            dgv.ColumnHeadersHeight = 44;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 9.75F);
            dgv.DefaultCellStyle.ForeColor = Color.FromArgb(15, 23, 42);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(219, 234, 254);
            dgv.DefaultCellStyle.SelectionForeColor = Color.FromArgb(15, 23, 42);
            dgv.DefaultCellStyle.Padding = new Padding(6, 0, 6, 0);
            dgv.RowTemplate.Height = 36;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);

            dgv.CellDoubleClick += OnRowDoubleClick;
            gridCard.Controls.Add(dgv);

            this.Controls.Add(gridCard);
            this.Controls.Add(footer);
            this.Controls.Add(topHeader);
        }

        private static readonly Dictionary<string, string> TableMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Advance_Authority_Defective_Signal", "Advance_Authority_Defective_Signal" },
            { "Train_Advance_Authority_Defective_Signal", "Advance_Authority_Defective_Signal" },
            { "Authority_Pass_Signal_ON", "Authority_Pass_Signal_ON" },
            { "Train_Pass_Signal_At_ON", "Authority_Pass_Signal_ON" },
            { "Train_Authority_Pass_Signal_ON", "Authority_Pass_Signal_ON" },
            { "Pass_Signal_ON", "Authority_Pass_Signal_ON" },
            { "Caution_Order_Entry", "Caution_Order_Entry" },
            { "Train_Caution_Order", "Caution_Order_Entry" },
            { "Train_Caution_Order_Entry", "Caution_Order_Entry" },
            { "Authority_Receive_Obstructed_Line", "Authority_Receive_Obstructed_Line" },
            { "Train_Receive_Obstructed_Line", "Authority_Receive_Obstructed_Line" },
            { "Authority_Receive_Non_Signalled", "Authority_Receive_Non_Signalled" },
            { "Train_Receive_Non_Signalled", "Authority_Receive_Non_Signalled" },
            { "Authority_Start_Non_Signalled", "Authority_Start_Non_Signalled" },
            { "Train_Start_Non_Signalled", "Authority_Start_Non_Signalled" },
            { "Authority_Common_Starter", "Authority_Common_Starter" },
            { "Train_Common_Starter_Authority", "Authority_Common_Starter" },
            { "Relief_Train_Authorization", "Relief_Train_Authorization" },
            { "Train_Relief_Train_Auth", "Relief_Train_Authorization" },
            { "Train_Relief_Train_Authorization", "Relief_Train_Authorization" },
            { "Communication_Failure_Log", "Communication_Failure_Log" },
            { "Train_Communication_Failure", "Communication_Failure_Log" },
            { "Line_Clear_Inquiry_TFC", "Line_Clear_Inquiry_TFC" },
            { "Train_Line_Clear_Inquiry", "Line_Clear_Inquiry_TFC" },
            { "Temporary_Single_Line_Working", "Temporary_Single_Line_Working" },
            { "Train_Temporary_Single_Line", "Temporary_Single_Line_Working" },
            { "Shunting_Order_Management", "Shunting_Order_Management" },
            { "Train_Shunting_Order", "Shunting_Order_Management" },
            { "Signal_Passing_Authority", "Signal_Passing_Authority" },
            { "Train_Signal_Passing_Authority", "Signal_Passing_Authority" },
            { "ABS_Proceed_Without_Line_Clear", "ABS_Proceed_Without_Line_Clear" },
            { "Train_ABS_Proceed_Without_Line_Clear", "ABS_Proceed_Without_Line_Clear" },
            { "ABS_Relief_Engine_Authority", "ABS_Relief_Engine_Authority" },
            { "Train_ABS_Relief_Engine", "ABS_Relief_Engine_Authority" },
            { "ABS_Prolonged_Signal_Failure", "ABS_Prolonged_Signal_Failure" },
            { "Train_ABS_Prolonged_Failure", "ABS_Prolonged_Signal_Failure" },
            { "Line_Clear_Tickets", "Line_Clear_Tickets" },
            { "Train_Line_Clear_Tickets", "Line_Clear_Tickets" },
            { "Maintenance_Trolley_Notice", "Maintenance_Trolley_Notice" },
            { "Train_Maintenance_Trolley_Notice", "Maintenance_Trolley_Notice" },
            { "Motor_Trolley_Permit", "Motor_Trolley_Permit" },
            { "Train_Motor_Trolley_Permit", "Motor_Trolley_Permit" },
            { "ST_Disconnection_Notice", "ST_Disconnection_Notice" },
            { "Train_ST_Disconnection_Notice", "ST_Disconnection_Notice" },
            { "Train_Movement_Log", "Train_Movement_Log" }
        };

        private void LoadData()
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                DatabaseHelper db = new DatabaseHelper();
                
                string targetTable = tableName;
                if (TableMap.ContainsKey(tableName))
                {
                    targetTable = TableMap[tableName];
                }

                fullData = db.ExecuteQuery($"SELECT * FROM [{targetTable}]");

                foreach (DataColumn col in fullData.Columns)
                {
                    if (col.DataType == typeof(byte[]))
                    {
                        col.ColumnName = col.ColumnName; // keep
                    }
                }

                dgv.DataSource = fullData;
                foreach (DataGridViewColumn col in dgv.Columns)
                {
                    col.MinimumWidth = 130;
                }
                UpdateCount(fullData.Rows.Count, fullData.Rows.Count);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load records:\n\n" + ex.Message,
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void OnSearch(object sender, EventArgs e)
        {
            if (fullData == null) return;
            string term = txtSearch.Text.Trim().Replace("'", "''");

            if (string.IsNullOrEmpty(term))
            {
                fullData.DefaultView.RowFilter = string.Empty;
                UpdateCount(fullData.Rows.Count, fullData.Rows.Count);
                return;
            }

            var sb = new StringBuilder();
            bool first = true;
            foreach (DataColumn col in fullData.Columns)
            {
                if (col.DataType == typeof(string))
                {
                    if (!first) sb.Append(" OR ");
                    sb.Append($"[{col.ColumnName}] LIKE '%{term}%'");
                    first = false;
                }
            }

            try
            {
                fullData.DefaultView.RowFilter = sb.ToString();
                UpdateCount(fullData.DefaultView.Count, fullData.Rows.Count);
            }
            catch
            {
                fullData.DefaultView.RowFilter = string.Empty;
            }
        }

        private void UpdateCount(int shown, int total)
        {
            lblCount.Text = shown == total
                ? $"Showing {total:N0} records"
                : $"Showing {shown:N0} of {total:N0} records";
        }

        private void OnRowDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = ((DataRowView)dgv.Rows[e.RowIndex].DataBoundItem).Row;

            Form modal = new Form
            {
                Text = $"{formTitle} — Record Details",
                Size = new Size(620, 520),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.White
            };

            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(20),
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };

            foreach (DataColumn col in fullData.Columns)
            {
                if (col.DataType == typeof(byte[])) continue;

                var itemPanel = new Panel { Width = 560, Height = 48, Margin = new Padding(0, 0, 0, 4) };
                var lblCol = new Label
                {
                    Text = col.ColumnName.Replace("_", " "),
                    Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(100, 116, 139),
                    Location = new Point(0, 2),
                    AutoSize = true
                };
                var lblVal = new Label
                {
                    Text = row[col] == DBNull.Value ? "—" : row[col].ToString(),
                    Font = new Font("Segoe UI", 10F),
                    ForeColor = Color.FromArgb(15, 23, 42),
                    Location = new Point(0, 20),
                    Size = new Size(550, 22),
                    AutoEllipsis = true
                };
                itemPanel.Controls.Add(lblCol);
                itemPanel.Controls.Add(lblVal);
                panel.Controls.Add(itemPanel);
            }

            var btnOk = new Button
            {
                Text = "Close",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                BackColor = ThemeManager.IRCTCColors.PrimaryNavy,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(100, 36),
                Dock = DockStyle.Bottom,
                Cursor = Cursors.Hand
            };
            btnOk.Click += (s, ev) => modal.Close();

            modal.Controls.Add(panel);
            modal.Controls.Add(btnOk);
            modal.ShowDialog(this);
        }
    }
}
