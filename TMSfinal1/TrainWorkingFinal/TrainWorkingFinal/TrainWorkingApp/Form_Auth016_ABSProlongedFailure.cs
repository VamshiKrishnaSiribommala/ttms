using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace TrainWorkingApp
{
    public class Form_Auth016_ABSProlongedFailure : Form
    {
        private readonly DatabaseHelper db = new DatabaseHelper();
        private TextBox txtFailureId;
        private TextBox txtSignals;
        private DateTimePicker dtpStart;
        private ComboBox cmbAuthType;
        private NumericUpDown numSpeed;
        private TextBox txtCautionDetails;
        private ComboBox cmbBlockRule;
        private TextBox txtRemarks;
        private CheckBox chkLocoAck;

        public Form_Auth016_ABSProlongedFailure()
        {
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Font = new Font("Segoe UI", 9.75f, FontStyle.Regular);
            InitializeComponent();
            ThemeManager.ApplyTheme(this);
            GenerateRefNo();
        }

        private void InitializeComponent()
        {
            this.Text = "PROLONGED FAILURE OF ALL SIGNALS IN ABS (T/D 912) - AUTH-016";
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
                Text = "PROLONGED FAILURE OF ALL SIGNALS IN ABS (T/D 912)",
                Font = new Font("Segoe UI", 14, FontStyle.Bold), 
                ForeColor = Color.White, 
                Location = new Point(25, 36), 
                AutoSize = true 
            };
            header.Controls.Add(lblT);

            int y = 95;
            AddLabel("Failure ID (System):", 30, y);
            txtFailureId = new TextBox { Location = new Point(290, y), Size = new Size(250, 26), ReadOnly = true, BackColor = Color.LightGray };
            this.Controls.Add(txtFailureId);
            y += 45;
            AddLabel("Affected Signals *:", 30, y);
            txtSignals = new TextBox { Location = new Point(290, y), Size = new Size(250, 26) };
            this.Controls.Add(txtSignals);
            y += 45;
            AddLabel("Failure Start Time *:", 30, y);
            dtpStart = new DateTimePicker { Location = new Point(290, y), Size = new Size(250, 26), Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy HH:mm:ss" };
            this.Controls.Add(dtpStart);
            y += 45;
            AddLabel("Authority Type *:", 30, y);
            cmbAuthType = new ComboBox { Location = new Point(290, y), Size = new Size(350, 26), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbAuthType.Items.AddRange(new string[] { "T/D 912 Full Authority", "Pilot Guard Working", "Modified Absolute Block", "Pilot Engine Authority" });
            cmbAuthType.SelectedIndex = -1;
            this.Controls.Add(cmbAuthType);
            y += 45;
            AddLabel("Permitted Speed Limit (kmph) *:", 30, y);
            numSpeed = new NumericUpDown { Location = new Point(290, y), Size = new Size(120, 26), Minimum = 5, Maximum = 40, Value = 25 };
            this.Controls.Add(numSpeed);
            y += 45;
            AddLabel("Caution Order Details *:", 30, y);
            txtCautionDetails = new TextBox { Location = new Point(290, y), Size = new Size(250, 26) };
            this.Controls.Add(txtCautionDetails);
            y += 45;
            AddLabel("Block Clearance Rule *:", 30, y);
            cmbBlockRule = new ComboBox { Location = new Point(290, y), Size = new Size(350, 26), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbBlockRule.Items.AddRange(new string[] { "15 Minutes Time Interval", "Telephone Clearance Between Stations", "Continuous Pilot Guard Running", "Pilot Motor Trolley Ahead" });
            cmbBlockRule.SelectedIndex = -1;
            this.Controls.Add(cmbBlockRule);
            y += 45;
            AddLabel("Remarks *:", 30, y);
            txtRemarks = new TextBox { Location = new Point(290, y), Size = new Size(450, 26) };
            this.Controls.Add(txtRemarks);
            y += 45;
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
            btnView.Click += (s, e) => new ViewRecordsForm("ABS_Prolonged_Signal_Failure", "ABS Prolonged Signal Failure Records").ShowDialog(this);
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
            txtFailureId.Text = "FAIL-ABS-" + DateTime.Now.ToString("yyyyMMdd") + "-" + new Random().Next(100, 999);
        }

        private void ClearFields()
        {
            txtSignals.Text = "";
            dtpStart.Value = DateTime.Now;
            cmbAuthType.SelectedIndex = -1;
            numSpeed.Value = 25;
            txtCautionDetails.Text = "";
            cmbBlockRule.SelectedIndex = -1;
            txtRemarks.Text = "";
            chkLocoAck.Checked = false;
            GenerateRefNo();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            var vals = new Dictionary<string, object>
            {
                { "Affected_Signals", txtSignals.Text.Trim() },
                { "Authority_Type", cmbAuthType.SelectedItem?.ToString() ?? "" },
                { "Permitted_Speed_Limit", numSpeed.Value.ToString() },
                { "Caution_Order_Details", txtCautionDetails.Text.Trim() },
                { "Block_Clearance_Rule", cmbBlockRule.SelectedItem?.ToString() ?? "" },
                { "Loco_Pilot_Acknowledgement", chkLocoAck.Checked },
                { "Remarks", txtRemarks.Text.Trim() }
            };

            if (!ValidationHelper.ValidateForm("016", vals)) return;

            try
            {
                using (SqlCommand cmd = new SqlCommand(@"INSERT INTO ABS_Prolonged_Signal_Failure 
                    (Failure_ID, Affected_Signals, Failure_Start_Time, Authority_Type, Issued_By, Permitted_Speed_Limit, Caution_Order_Details, Block_Clearance_Rule, Loco_Pilot_Acknowledgement, Remarks)
                    VALUES (@fid, @sig, @start, @type, @issuedBy, @spd, @caut, @rule, @ack, @rem)"))
                {
                    cmd.Parameters.AddWithValue("@fid", txtFailureId.Text.Trim());
                    cmd.Parameters.AddWithValue("@sig", txtSignals.Text.Trim());
                    cmd.Parameters.AddWithValue("@start", dtpStart.Value);
                    cmd.Parameters.AddWithValue("@type", (cmbAuthType.SelectedItem?.ToString() ?? cmbAuthType.Text?.Trim() ?? ""));
                    cmd.Parameters.AddWithValue("@issuedBy", SessionManager.CurrentFullName ?? "Station Operator");
                    cmd.Parameters.AddWithValue("@spd", (int)numSpeed.Value);
                    cmd.Parameters.AddWithValue("@caut", txtCautionDetails.Text.Trim());
                    cmd.Parameters.AddWithValue("@rule", (cmbBlockRule.SelectedItem?.ToString() ?? cmbBlockRule.Text?.Trim() ?? ""));
                    cmd.Parameters.AddWithValue("@ack", chkLocoAck.Checked);
                    cmd.Parameters.AddWithValue("@rem", txtRemarks.Text.Trim());

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
