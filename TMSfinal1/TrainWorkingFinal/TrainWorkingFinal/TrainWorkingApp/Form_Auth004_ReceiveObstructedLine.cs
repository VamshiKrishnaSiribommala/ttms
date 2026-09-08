using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace TrainWorkingApp
{
    public class Form_Auth004_ReceiveObstructedLine : Form
    {
        private readonly DatabaseHelper db = new DatabaseHelper();
        private TextBox txtRefNo;
        private TextBox txtTrainNo;
        private TextBox txtLineNo;
        private ComboBox cmbNature;
        private DateTimePicker dtpIssue;
        private TextBox txtOfficer;
        private NumericUpDown numSpeed;
        private TextBox txtStopShort;
        private CheckBox chkLineProtection;
        private CheckBox chkLocoAck;

        public Form_Auth004_ReceiveObstructedLine()
        {
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Font = new Font("Segoe UI", 9.75f, FontStyle.Regular);
            InitializeComponent();
            ThemeManager.ApplyTheme(this);
            GenerateRefNo();
        }

        private void InitializeComponent()
        {
            this.Text = "AUTHORITY TO RECEIVE TRAIN ON OBSTRUCTED LINE (T/509) - AUTH-004";
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
                Text = "AUTHORITY TO RECEIVE TRAIN ON OBSTRUCTED LINE (T/509)",
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
            AddLabel("Line Number *:", 30, y);
            txtLineNo = new TextBox { Location = new Point(290, y), Size = new Size(250, 26) };
            this.Controls.Add(txtLineNo);
            y += 45;
            AddLabel("Nature of Obstruction *:", 30, y);
            cmbNature = new ComboBox { Location = new Point(290, y), Size = new Size(350, 26), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbNature.Items.AddRange(new string[] { "Disabled Train on Line", "Stalled Wagons / Shunting Rake", "Derailment / Breakdown Crane in Section", "Track Settlement / Washout", "Fallen Tree / OHE Wire Hanging" });
            cmbNature.SelectedIndex = -1;
            this.Controls.Add(cmbNature);
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
            numSpeed = new NumericUpDown { Location = new Point(290, y), Size = new Size(120, 26), Minimum = 1, Maximum = 25, Value = 10 };
            this.Controls.Add(numSpeed);
            y += 45;
            AddLabel("Stop Short Instruction *:", 30, y);
            txtStopShort = new TextBox { Location = new Point(290, y), Size = new Size(450, 26) };
            this.Controls.Add(txtStopShort);
            y += 45;
            chkLineProtection = new CheckBox { Text = "Line protection flags/detonators placed *", Location = new Point(290, y), Size = new Size(560, 24), Checked = false };
            this.Controls.Add(chkLineProtection);
            y += 30;
            chkLocoAck = new CheckBox { Text = "Loco Pilot verbal & digital acknowledgement recorded *", Location = new Point(290, y), Size = new Size(560, 24), Checked = false };
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
            btnView.Click += (s, e) => new ViewRecordsForm("Authority_Receive_Obstructed_Line", "Obstructed Line Authority").ShowDialog(this);
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
            txtRefNo.Text = "AUTH-OBS-" + DateTime.Now.ToString("yyyyMMdd") + "-" + new Random().Next(100, 999);
        }

        private void ClearFields()
        {
            txtTrainNo.Text = "";
            txtLineNo.Text = "";
            cmbNature.SelectedIndex = -1;
            dtpIssue.Value = DateTime.Now;
            txtOfficer.Text = "";
            numSpeed.Value = 10;
            txtStopShort.Text = "";
            chkLineProtection.Checked = false;
            chkLocoAck.Checked = false;
            GenerateRefNo();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            var vals = new Dictionary<string, object>
            {
                { "Train_No", txtTrainNo.Text.Trim() },
                { "Line_No", txtLineNo.Text.Trim() },
                { "Nature_of_Obstruction", cmbNature.SelectedItem?.ToString() ?? "" },
                { "Issuing_Officer", txtOfficer.Text.Trim() },
                { "Permitted_Speed", numSpeed.Value.ToString() },
                { "Line_Protection", chkLineProtection.Checked },
                { "Stop_Short_Instruction", txtStopShort.Text.Trim() },
                { "Loco_Pilot_Ack", chkLocoAck.Checked }
            };

            if (!ValidationHelper.ValidateForm("004", vals)) return;

            try
            {
                using (SqlCommand cmd = new SqlCommand(@"INSERT INTO Authority_Receive_Obstructed_Line 
                    (Authority_Reference_No, Train_No, Line_No, Nature_of_Obstruction, Date_Time_of_Issue, Issuing_Officer, Permitted_Speed, Line_Protection, Stop_Short_Instruction, Loco_Pilot_Ack)
                    VALUES (@ref, @train, @line, @nat, @dt, @off, @spd, @prot, @stop, @ack)"))
                {
                    cmd.Parameters.AddWithValue("@ref", txtRefNo.Text);
                    cmd.Parameters.AddWithValue("@train", txtTrainNo.Text.Trim());
                    cmd.Parameters.AddWithValue("@line", txtLineNo.Text.Trim());
                    cmd.Parameters.AddWithValue("@nat", (cmbNature.SelectedItem?.ToString() ?? cmbNature.Text?.Trim() ?? ""));
                    cmd.Parameters.AddWithValue("@dt", dtpIssue.Value);
                    cmd.Parameters.AddWithValue("@off", txtOfficer.Text.Trim());
                    cmd.Parameters.AddWithValue("@spd", (int)numSpeed.Value);
                    cmd.Parameters.AddWithValue("@prot", chkLineProtection.Checked);
                    cmd.Parameters.AddWithValue("@stop", txtStopShort.Text.Trim());
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
