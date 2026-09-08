using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace TrainWorkingApp
{
    public class Form_Auth011_TemporarySingleLine : Form
    {
        private readonly DatabaseHelper db = new DatabaseHelper();
        private TextBox txtRefNo;
        private TextBox txtSection;
        private ComboBox cmbBlockedLine;
        private ComboBox cmbWorkingDir;
        private DateTimePicker dtpStart;
        private DateTimePicker dtpEnd;
        private TextBox txtControlApproval;

        public Form_Auth011_TemporarySingleLine()
        {
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Font = new Font("Segoe UI", 9.75f, FontStyle.Regular);
            InitializeComponent();
            ThemeManager.ApplyTheme(this);
            GenerateRefNo();
        }

        private void InitializeComponent()
        {
            this.Text = "TEMPORARY SINGLE LINE WORKING ON DOUBLE LINE (T/D 602) - AUTH-011";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.MinimumSize = new Size(1000, 700);
            this.BackColor = Color.FromArgb(241, 245, 249);

            Panel header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 75,
                BackColor = ThemeManager.IRCTCColors.PrimaryNavy
            };
            this.Controls.Add(header);

            Label lblPath = new Label 
            { 
                Text = "Train Working Management System • Operational Authorities & Safety Register",
                Font = new Font("Segoe UI", 9, FontStyle.Italic), 
                ForeColor = Color.FromArgb(200, 200, 200), 
                Location = new Point(25, 12), 
                Size = new Size(700, 20) 
            };
            header.Controls.Add(lblPath);

            Label lblT = new Label 
            { 
                Text = "TEMPORARY SINGLE LINE WORKING ON DOUBLE LINE (T/D 602)",
                Font = new Font("Segoe UI", 14, FontStyle.Bold), 
                ForeColor = Color.White, 
                Location = new Point(25, 36), 
                AutoSize = true 
            };
            header.Controls.Add(lblT);

            int y = 95;
            AddLabel("Working Ref No (System):", 30, y);
            txtRefNo = new TextBox { Location = new Point(290, y), Size = new Size(250, 26), ReadOnly = true, BackColor = Color.LightGray };
            this.Controls.Add(txtRefNo);
            y += 45;
            AddLabel("Affected Section *:", 30, y);
            txtSection = new TextBox { Location = new Point(290, y), Size = new Size(350, 26) };
            this.Controls.Add(txtSection);
            y += 45;
            AddLabel("Blocked Line *:", 30, y);
            cmbBlockedLine = new ComboBox { Location = new Point(290, y), Size = new Size(350, 26), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbBlockedLine.Items.AddRange(new string[] { "UP Main Line", "DOWN Main Line", "Both Lines Defective", "Goods Bypass Line" });
            cmbBlockedLine.SelectedIndex = -1;
            this.Controls.Add(cmbBlockedLine);
            y += 45;
            AddLabel("Working Direction *:", 30, y);
            cmbWorkingDir = new ComboBox { Location = new Point(290, y), Size = new Size(350, 26), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbWorkingDir.Items.AddRange(new string[] { "Right Line Direction", "Wrong Line / Opposite Direction", "Bi-directional Single Line" });
            cmbWorkingDir.SelectedIndex = -1;
            this.Controls.Add(cmbWorkingDir);
            y += 45;
            AddLabel("Start Time *:", 30, y);
            dtpStart = new DateTimePicker { Location = new Point(290, y), Size = new Size(250, 26), Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy HH:mm:ss" };
            this.Controls.Add(dtpStart);
            y += 45;
            AddLabel("Estimated End Time *:", 30, y);
            dtpEnd = new DateTimePicker { Location = new Point(290, y), Size = new Size(250, 26), Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy HH:mm:ss" };
            this.Controls.Add(dtpEnd);
            y += 45;
            AddLabel("Control Approval Ref *:", 30, y);
            txtControlApproval = new TextBox { Location = new Point(290, y), Size = new Size(250, 26) };
            this.Controls.Add(txtControlApproval);
            y += 45;

            Button btnSave = new Button { Text = "SAVE AUTHORITY", Location = new Point(290, y), Size = new Size(160, 38) };
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            Button btnClear = new Button { Text = "CLEAR", Location = new Point(465, y), Size = new Size(100, 38) };
            btnClear.Click += (s, e) => ClearFields();
            this.Controls.Add(btnClear);

            Button btnView = new Button { Text = "VIEW RECORDS", Location = new Point(580, y), Size = new Size(140, 38) };
            btnView.Click += (s, e) => new ViewRecordsForm("Temporary_Single_Line_Working", "Temporary Single Line Working").ShowDialog(this);
            this.Controls.Add(btnView);

            Button btnClose = new Button { Text = "CLOSE", Location = new Point(735, y), Size = new Size(100, 38) };
            btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(btnClose);
        }

        private void AddLabel(string text, int x, int y)
        {
            Label lbl = new Label { Text = text, Location = new Point(x, y + 4), AutoSize = true, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold) };
            this.Controls.Add(lbl);
        }

        private void GenerateRefNo()
        {
            txtRefNo.Text = "TSL-" + DateTime.Now.ToString("yyyyMMdd") + "-" + new Random().Next(100, 999);
        }

        private void ClearFields()
        {
            txtSection.Text = "";
            cmbBlockedLine.SelectedIndex = -1;
            cmbWorkingDir.SelectedIndex = -1;
            dtpStart.Value = DateTime.Now;
            dtpEnd.Value = DateTime.Now;
            txtControlApproval.Text = "";
            GenerateRefNo();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            var vals = new Dictionary<string, object>
            {
                { "Affected_Section", txtSection.Text.Trim() },
                { "Blocked_Line", cmbBlockedLine.SelectedItem?.ToString() ?? "" },
                { "Working_Direction", cmbWorkingDir.SelectedItem?.ToString() ?? "" },
                { "Control_Approval", txtControlApproval.Text.Trim() }
            };

            if (!ValidationHelper.ValidateForm("011", vals)) return;

            try
            {
                using (SqlCommand cmd = new SqlCommand(@"INSERT INTO Temporary_Single_Line_Working 
                    (Working_Reference_No, Affected_Section, Blocked_Line, Working_Direction, Start_Time, End_Time, Control_Approval)
                    VALUES (@ref, @sec, @line, @dir, @start, @end, @ctrl)"))
                {
                    cmd.Parameters.AddWithValue("@ref", txtRefNo.Text);
                    cmd.Parameters.AddWithValue("@sec", txtSection.Text.Trim());
                    cmd.Parameters.AddWithValue("@line", (cmbBlockedLine.SelectedItem?.ToString() ?? cmbBlockedLine.Text?.Trim() ?? ""));
                    cmd.Parameters.AddWithValue("@dir", (cmbWorkingDir.SelectedItem?.ToString() ?? cmbWorkingDir.Text?.Trim() ?? ""));
                    cmd.Parameters.AddWithValue("@start", dtpStart.Value);
                    cmd.Parameters.AddWithValue("@end", dtpEnd.Value);
                    cmd.Parameters.AddWithValue("@ctrl", txtControlApproval.Text.Trim());

                    db.ExecuteNonQuery(cmd);
                    MessageBox.Show("Authority saved and registered successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearFields();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
