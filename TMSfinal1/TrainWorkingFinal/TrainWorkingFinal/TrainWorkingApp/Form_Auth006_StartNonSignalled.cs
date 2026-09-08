using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace TrainWorkingApp
{
    public class Form_Auth006_StartNonSignalled : Form
    {
        private readonly DatabaseHelper db = new DatabaseHelper();
        private TextBox txtRefNo;
        private TextBox txtTrainNo;
        private TextBox txtLineNo;
        private TextBox txtStation;
        private DateTimePicker dtpIssue;
        private TextBox txtOfficer;
        private CheckBox chkRouteClearance;
        private CheckBox chkBlockSection;
        private CheckBox chkLocoAck;

        public Form_Auth006_StartNonSignalled()
        {
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Font = new Font("Segoe UI", 9.75f, FontStyle.Regular);
            InitializeComponent();
            ThemeManager.ApplyTheme(this);
            GenerateRefNo();
        }

        private void InitializeComponent()
        {
            this.Text = "AUTHORITY TO START FROM NON-SIGNALLED LINE (T/511) - AUTH-006";
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
                Text = "AUTHORITY TO START FROM NON-SIGNALLED LINE (T/511)",
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
            AddLabel("Starting Station *:", 30, y);
            txtStation = new TextBox { Location = new Point(290, y), Size = new Size(250, 26) };
            this.Controls.Add(txtStation);
            y += 45;
            AddLabel("Date & Time of Issue *:", 30, y);
            dtpIssue = new DateTimePicker { Location = new Point(290, y), Size = new Size(250, 26), Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy HH:mm:ss" };
            this.Controls.Add(dtpIssue);
            y += 45;
            AddLabel("Issuing Officer *:", 30, y);
            txtOfficer = new TextBox { Location = new Point(290, y), Size = new Size(250, 26) };
            this.Controls.Add(txtOfficer);
            y += 45;
            chkRouteClearance = new CheckBox { Text = "Facing points correctly set, clamped and route clear *", Location = new Point(290, y), Size = new Size(560, 24), Checked = false };
            this.Controls.Add(chkRouteClearance);
            y += 30;
            chkBlockSection = new CheckBox { Text = "Line Clear obtained from station ahead *", Location = new Point(290, y), Size = new Size(560, 24), Checked = false };
            this.Controls.Add(chkBlockSection);
            y += 30;
            chkLocoAck = new CheckBox { Text = "Loco Pilot acknowledgement received *", Location = new Point(290, y), Size = new Size(560, 24), Checked = false };
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
            btnView.Click += (s, e) => new ViewRecordsForm("Authority_Start_Non_Signalled", "Start Non-Signalled Line Authorities").ShowDialog(this);
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
            txtRefNo.Text = "AUTH-SNS-" + DateTime.Now.ToString("yyyyMMdd") + "-" + new Random().Next(100, 999);
        }

        private void ClearFields()
        {
            txtTrainNo.Text = "";
            txtLineNo.Text = "";
            txtStation.Text = "";
            dtpIssue.Value = DateTime.Now;
            txtOfficer.Text = "";
            chkRouteClearance.Checked = false;
            chkBlockSection.Checked = false;
            chkLocoAck.Checked = false;
            GenerateRefNo();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            var vals = new Dictionary<string, object>
            {
                { "Train_No", txtTrainNo.Text.Trim() },
                { "Line_No", txtLineNo.Text.Trim() },
                { "Starting_Station", txtStation.Text.Trim() },
                { "Issuing_Officer", txtOfficer.Text.Trim() },
                { "Route_Clearance", chkRouteClearance.Checked },
                { "Block_Section_Status", chkBlockSection.Checked },
                { "Loco_Pilot_Ack", chkLocoAck.Checked }
            };

            if (!ValidationHelper.ValidateForm("006", vals)) return;

            try
            {
                using (SqlCommand cmd = new SqlCommand(@"INSERT INTO Authority_Start_Non_Signalled 
                    (Authority_Reference_No, Train_No, Line_No, Starting_Station, Date_Time_of_Issue, Issuing_Officer, Route_Clearance, Block_Section_Status, Loco_Pilot_Ack)
                    VALUES (@ref, @train, @line, @stn, @dt, @off, @route, @blk, @ack)"))
                {
                    cmd.Parameters.AddWithValue("@ref", txtRefNo.Text);
                    cmd.Parameters.AddWithValue("@train", txtTrainNo.Text.Trim());
                    cmd.Parameters.AddWithValue("@line", txtLineNo.Text.Trim());
                    cmd.Parameters.AddWithValue("@stn", txtStation.Text.Trim());
                    cmd.Parameters.AddWithValue("@dt", dtpIssue.Value);
                    cmd.Parameters.AddWithValue("@off", txtOfficer.Text.Trim());
                    cmd.Parameters.AddWithValue("@route", chkRouteClearance.Checked);
                    cmd.Parameters.AddWithValue("@blk", chkBlockSection.Checked);
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
