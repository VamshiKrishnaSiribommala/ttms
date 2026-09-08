using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace TrainWorkingApp
{
    public class Form_Auth002_PassSignalON : Form
    {
        private readonly DatabaseHelper db = new DatabaseHelper();
        private TextBox txtRefNo;
        private TextBox txtTrainNo;
        private TextBox txtSignalNo;
        private TextBox txtLocation;
        private ComboBox cmbReason;
        private DateTimePicker dtpIssue;
        private TextBox txtOfficer;
        private NumericUpDown numSpeed;
        private CheckBox chkBlockSection;
        private CheckBox chkRouteClearance;
        private CheckBox chkLocoAck;

        public Form_Auth002_PassSignalON()
        {
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Font = new Font("Segoe UI", 9.75f, FontStyle.Regular);
            InitializeComponent();
            ThemeManager.ApplyTheme(this);
            GenerateRefNo();
        }

        private void InitializeComponent()
        {
            this.Text = "AUTHORITY TO PASS SIGNAL AT ON (T/369-3b) - AUTH-002";
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
                Text = "AUTHORITY TO PASS SIGNAL AT ON (T/369-3b)",
                Font = new Font("Segoe UI", 14, FontStyle.Bold), 
                ForeColor = Color.White, 
                Location = new Point(25, 36), 
                AutoSize = true 
            };
            header.Controls.Add(lblT);

            int y = 95;
            AddLabel("Authority Ref No (System):", 30, y);
            txtRefNo = new TextBox { Location = new Point(290, y), Size = new Size(250, 26), ReadOnly = true, BackColor = Color.LightGray };
            this.Controls.Add(txtRefNo);
            y += 45;
            AddLabel("Train Number *:", 30, y);
            txtTrainNo = new TextBox { Location = new Point(290, y), Size = new Size(250, 26) };
            this.Controls.Add(txtTrainNo);
            y += 45;
            AddLabel("Signal Number *:", 30, y);
            txtSignalNo = new TextBox { Location = new Point(290, y), Size = new Size(250, 26) };
            this.Controls.Add(txtSignalNo);
            y += 45;
            AddLabel("Signal Location *:", 30, y);
            txtLocation = new TextBox { Location = new Point(290, y), Size = new Size(350, 26) };
            this.Controls.Add(txtLocation);
            y += 45;
            AddLabel("Reason *:", 30, y);
            cmbReason = new ComboBox { Location = new Point(290, y), Size = new Size(350, 26), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbReason.Items.AddRange(new string[] { "Signal ON due to Track Circuit Failure", "Point Failure - Hand Signalled", "Power Supply Failure", "Interlocking Defective" });
            cmbReason.SelectedIndex = -1;
            this.Controls.Add(cmbReason);
            y += 45;
            AddLabel("Date & Time of Issue *:", 30, y);
            dtpIssue = new DateTimePicker { Location = new Point(290, y), Size = new Size(250, 26), Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy HH:mm:ss" };
            this.Controls.Add(dtpIssue);
            y += 45;
            AddLabel("Issuing Officer *:", 30, y);
            txtOfficer = new TextBox { Location = new Point(290, y), Size = new Size(250, 26) };
            this.Controls.Add(txtOfficer);
            y += 45;
            AddLabel("Permitted Speed (kmph) *:", 30, y);
            numSpeed = new NumericUpDown { Location = new Point(290, y), Size = new Size(120, 26), Minimum = 1, Maximum = 160, Value = 15 };
            this.Controls.Add(numSpeed);
            y += 45;
            chkBlockSection = new CheckBox { Text = "Block section occupied / verified clear as per operating rule *", Location = new Point(290, y), Size = new Size(560, 24), Checked = false };
            this.Controls.Add(chkBlockSection);
            y += 30;
            chkRouteClearance = new CheckBox { Text = "Route set, clamped and padlocked / verified clear *", Location = new Point(290, y), Size = new Size(560, 24), Checked = false };
            this.Controls.Add(chkRouteClearance);
            y += 30;
            chkLocoAck = new CheckBox { Text = "Loco Pilot verbal / digital acknowledgement received *", Location = new Point(290, y), Size = new Size(560, 24), Checked = false };
            this.Controls.Add(chkLocoAck);
            y += 30;
            y += 15;

            Button btnSave = new Button { Text = "SAVE AUTHORITY", Location = new Point(290, y), Size = new Size(160, 38) };
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            Button btnClear = new Button { Text = "CLEAR", Location = new Point(465, y), Size = new Size(100, 38) };
            btnClear.Click += (s, e) => ClearFields();
            this.Controls.Add(btnClear);

            Button btnView = new Button { Text = "VIEW RECORDS", Location = new Point(580, y), Size = new Size(140, 38) };
            btnView.Click += (s, e) => new ViewRecordsForm("Authority_Pass_Signal_ON", "Authority Pass Signal at ON").ShowDialog(this);
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
            txtRefNo.Text = "AUTH-ON-" + DateTime.Now.ToString("yyyyMMdd") + "-" + new Random().Next(100, 999);
        }

        private void ClearFields()
        {
            txtTrainNo.Text = "";
            txtSignalNo.Text = "";
            txtLocation.Text = "";
            cmbReason.SelectedIndex = -1;
            dtpIssue.Value = DateTime.Now;
            txtOfficer.Text = "";
            numSpeed.Value = 15;
            chkBlockSection.Checked = false;
            chkRouteClearance.Checked = false;
            chkLocoAck.Checked = false;
            GenerateRefNo();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            var vals = new Dictionary<string, object>
            {
                { "Train_No", txtTrainNo.Text.Trim() },
                { "Signal_No", txtSignalNo.Text.Trim() },
                { "Signal_Location", txtLocation.Text.Trim() },
                { "Reason", cmbReason.SelectedItem?.ToString() ?? "" },
                { "Issuing_Officer", txtOfficer.Text.Trim() },
                { "Permitted_Speed", numSpeed.Value.ToString() },
                { "Block_Section_Status", chkBlockSection.Checked },
                { "Route_Clearance", chkRouteClearance.Checked },
                { "Loco_Pilot_Ack", chkLocoAck.Checked }
            };

            if (!ValidationHelper.ValidateForm("002", vals)) return;

            try
            {
                using (SqlCommand cmd = new SqlCommand(@"INSERT INTO Authority_Pass_Signal_ON 
                    (Authority_Reference_No, Train_No, Signal_No, Signal_Location, Reason, Date_Time_of_Issue, Issuing_Officer, Block_Section_Status, Route_Clearance, Permitted_Speed, Loco_Pilot_Ack)
                    VALUES (@ref, @train, @sig, @loc, @reason, @dt, @off, @blk, @route, @spd, @ack)"))
                {
                    cmd.Parameters.AddWithValue("@ref", txtRefNo.Text);
                    cmd.Parameters.AddWithValue("@train", txtTrainNo.Text.Trim());
                    cmd.Parameters.AddWithValue("@sig", txtSignalNo.Text.Trim());
                    cmd.Parameters.AddWithValue("@loc", txtLocation.Text.Trim());
                    cmd.Parameters.AddWithValue("@reason", (cmbReason.SelectedItem?.ToString() ?? cmbReason.Text?.Trim() ?? ""));
                    cmd.Parameters.AddWithValue("@dt", dtpIssue.Value);
                    cmd.Parameters.AddWithValue("@off", txtOfficer.Text.Trim());
                    cmd.Parameters.AddWithValue("@blk", chkBlockSection.Checked);
                    cmd.Parameters.AddWithValue("@route", chkRouteClearance.Checked);
                    cmd.Parameters.AddWithValue("@spd", (int)numSpeed.Value);
                    cmd.Parameters.AddWithValue("@ack", chkLocoAck.Checked);

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
