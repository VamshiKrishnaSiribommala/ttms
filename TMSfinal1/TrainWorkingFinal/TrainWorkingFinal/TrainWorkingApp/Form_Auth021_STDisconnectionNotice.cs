using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace TrainWorkingApp
{
    public class Form_Auth021_STDisconnectionNotice : Form
    {
        private readonly DatabaseHelper db = new DatabaseHelper();
        private TextBox txtMemoNo;
        private TextBox txtEquipmentId;
        private DateTimePicker dtpDisconnection;
        private ComboBox cmbReason;
        private DateTimePicker dtpReconnection;
        private CheckBox chkJointTesting;

        public Form_Auth021_STDisconnectionNotice()
        {
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Font = new Font("Segoe UI", 9.75f, FontStyle.Regular);
            InitializeComponent();
            ThemeManager.ApplyTheme(this);
            GenerateRefNo();
        }

        private void InitializeComponent()
        {
            this.Text = "S&T DISCONNECTION NOTICE (S&T T/351) - AUTH-021";
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
                Text = "S&T DISCONNECTION NOTICE (S&T T/351)",
                Font = new Font("Segoe UI", 14, FontStyle.Bold), 
                ForeColor = Color.White, 
                Location = new Point(25, 36), 
                AutoSize = true 
            };
            header.Controls.Add(lblT);

            int y = 95;
            AddLabel("Disconnection Memo No (System):", 30, y);
            txtMemoNo = new TextBox { Location = new Point(290, y), Size = new Size(250, 26), ReadOnly = true, BackColor = Color.LightGray };
            this.Controls.Add(txtMemoNo);
            y += 45;
            AddLabel("Equipment ID / Gear *:", 30, y);
            txtEquipmentId = new TextBox { Location = new Point(290, y), Size = new Size(350, 26) };
            this.Controls.Add(txtEquipmentId);
            y += 45;
            AddLabel("Disconnection Time *:", 30, y);
            dtpDisconnection = new DateTimePicker { Location = new Point(290, y), Size = new Size(250, 26), Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy HH:mm:ss" };
            this.Controls.Add(dtpDisconnection);
            y += 45;
            AddLabel("Reason *:", 30, y);
            cmbReason = new ComboBox { Location = new Point(290, y), Size = new Size(350, 26), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbReason.Items.AddRange(new string[] { "Track Circuit Maintenance", "Point Machine Replacement", "Signal Aspect Alignment", "Interlocking Testing", "Cable Jointing / Splicing", "Emergency Defect Rectification" });
            cmbReason.SelectedIndex = -1;
            this.Controls.Add(cmbReason);
            y += 45;
            AddLabel("Estimated Reconnection Time *:", 30, y);
            dtpReconnection = new DateTimePicker { Location = new Point(290, y), Size = new Size(250, 26), Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy HH:mm:ss" };
            this.Controls.Add(dtpReconnection);
            y += 45;
            chkJointTesting = new CheckBox { Text = "Joint testing successfully completed by Station Master & S&T Maintainer *", Location = new Point(290, y), Size = new Size(560, 24), Checked = false };
            this.Controls.Add(chkJointTesting);
            y += 30;
            y += 15;

            Button btnSave = new Button { Text = "SAVE AUTHORITY", Location = new Point(290, y), Size = new Size(160, 38) };
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            Button btnClear = new Button { Text = "CLEAR", Location = new Point(465, y), Size = new Size(100, 38) };
            btnClear.Click += (s, e) => ClearFields();
            this.Controls.Add(btnClear);

            Button btnView = new Button { Text = "VIEW RECORDS", Location = new Point(580, y), Size = new Size(140, 38) };
            btnView.Click += (s, e) => new ViewRecordsForm("ST_Disconnection_Notice", "S&T Disconnection Notices").ShowDialog(this);
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
            txtMemoNo.Text = "ST-MEMO-" + DateTime.Now.ToString("yyyyMMdd") + "-" + new Random().Next(100, 999);
        }

        private void ClearFields()
        {
            txtEquipmentId.Text = "";
            dtpDisconnection.Value = DateTime.Now;
            cmbReason.SelectedIndex = -1;
            dtpReconnection.Value = DateTime.Now;
            chkJointTesting.Checked = false;
            GenerateRefNo();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            var vals = new Dictionary<string, object>
            {
                { "Equipment_ID", txtEquipmentId.Text.Trim() },
                { "Reason", cmbReason.SelectedItem?.ToString() ?? "" },
                { "Joint_Testing_Result", chkJointTesting.Checked }
            };

            if (!ValidationHelper.ValidateForm("021", vals)) return;

            try
            {
                using (SqlCommand cmd = new SqlCommand(@"INSERT INTO ST_Disconnection_Notice 
                    (Memo_Number, Equipment_ID, Disconnection_Time, Reason, Reconnection_Time, Joint_Testing_Result)
                    VALUES (@ref, @eq, @dt, @reason, @reconn, @test)"))
                {
                    cmd.Parameters.AddWithValue("@ref", txtMemoNo.Text);
                    cmd.Parameters.AddWithValue("@eq", txtEquipmentId.Text.Trim());
                    cmd.Parameters.AddWithValue("@dt", dtpDisconnection.Value);
                    cmd.Parameters.AddWithValue("@reason", (cmbReason.SelectedItem?.ToString() ?? cmbReason.Text?.Trim() ?? ""));
                    cmd.Parameters.AddWithValue("@reconn", dtpReconnection.Value);
                    cmd.Parameters.AddWithValue("@test", chkJointTesting.Checked);

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
