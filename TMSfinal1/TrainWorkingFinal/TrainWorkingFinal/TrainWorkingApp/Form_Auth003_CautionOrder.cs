using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace TrainWorkingApp
{
    public class Form_Auth003_CautionOrder : Form
    {
        private readonly DatabaseHelper db = new DatabaseHelper();
        private TextBox txtOrderNo;
        private TextBox txtTrainNo;
        private TextBox txtLocation;
        private NumericUpDown numSpeed;
        private ComboBox cmbReason;
        private ComboBox cmbType;
        private DateTimePicker dtpIssue;
        private TextBox txtOfficer;
        private CheckBox chkValidity;
        private CheckBox chkLocoAck;

        public Form_Auth003_CautionOrder()
        {
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Font = new Font("Segoe UI", 9.75f, FontStyle.Regular);
            InitializeComponent();
            ThemeManager.ApplyTheme(this);
            GenerateRefNo();
        }

        private void InitializeComponent()
        {
            this.Text = "CAUTION ORDER ENTRY & SPEED RESTRICTIONS (T/409) - AUTH-003";
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
                Text = "CAUTION ORDER ENTRY & SPEED RESTRICTIONS (T/409)",
                Font = new Font("Segoe UI", 14, FontStyle.Bold), 
                ForeColor = Color.White, 
                Location = new Point(25, 36), 
                AutoSize = true 
            };
            header.Controls.Add(lblT);

            int y = 95;
            AddLabel("Caution Order No (System):", 30, y);
            txtOrderNo = new TextBox { Location = new Point(290, y), Size = new Size(250, 26), ReadOnly = true, BackColor = Color.LightGray };
            this.Controls.Add(txtOrderNo);
            y += 45;
            AddLabel("Train Number *:", 30, y);
            txtTrainNo = new TextBox { Location = new Point(290, y), Size = new Size(250, 26) };
            this.Controls.Add(txtTrainNo);
            y += 45;
            AddLabel("Section / Location *:", 30, y);
            txtLocation = new TextBox { Location = new Point(290, y), Size = new Size(350, 26) };
            this.Controls.Add(txtLocation);
            y += 45;
            AddLabel("Speed Restriction (kmph) *:", 30, y);
            numSpeed = new NumericUpDown { Location = new Point(290, y), Size = new Size(120, 26), Minimum = 5, Maximum = 120, Value = 30 };
            this.Controls.Add(numSpeed);
            y += 45;
            AddLabel("Reason *:", 30, y);
            cmbReason = new ComboBox { Location = new Point(290, y), Size = new Size(350, 26), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbReason.Items.AddRange(new string[] { "Track Maintenance / P-Way Work", "Bridge Repair / Structural Work", "Overhead Equipment (OHE) Defect", "Signal / Interlocking Failure", "Monsoon / Weather Precaution", "Cattle Run Over / Track Obstruction" });
            cmbReason.SelectedIndex = -1;
            this.Controls.Add(cmbReason);
            y += 45;
            AddLabel("Caution Type *:", 30, y);
            cmbType = new ComboBox { Location = new Point(290, y), Size = new Size(350, 26), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbType.Items.AddRange(new string[] { "Divisional Caution Order", "Special Caution Order", "Emergency Caution Order", "Nil Caution Order" });
            cmbType.SelectedIndex = -1;
            this.Controls.Add(cmbType);
            y += 45;
            AddLabel("Date & Time of Issue *:", 30, y);
            dtpIssue = new DateTimePicker { Location = new Point(290, y), Size = new Size(250, 26), Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy HH:mm:ss" };
            this.Controls.Add(dtpIssue);
            y += 45;
            AddLabel("Issued By *:", 30, y);
            txtOfficer = new TextBox { Location = new Point(290, y), Size = new Size(250, 26) };
            this.Controls.Add(txtOfficer);
            y += 45;
            chkValidity = new CheckBox { Text = "Restriction validity confirmed active *", Location = new Point(290, y), Size = new Size(560, 24), Checked = false };
            this.Controls.Add(chkValidity);
            y += 30;
            chkLocoAck = new CheckBox { Text = "Loco Pilot signature / acknowledgement recorded *", Location = new Point(290, y), Size = new Size(560, 24), Checked = false };
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
            btnView.Click += (s, e) => new ViewRecordsForm("Caution_Order_Entry", "Caution Order Entries").ShowDialog(this);
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
            txtOrderNo.Text = "CO-" + DateTime.Now.ToString("yyyyMMdd") + "-" + new Random().Next(100, 999);
        }

        private void ClearFields()
        {
            txtTrainNo.Text = "";
            txtLocation.Text = "";
            numSpeed.Value = 30;
            cmbReason.SelectedIndex = -1;
            cmbType.SelectedIndex = -1;
            dtpIssue.Value = DateTime.Now;
            txtOfficer.Text = "";
            chkValidity.Checked = false;
            chkLocoAck.Checked = false;
            GenerateRefNo();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            var vals = new Dictionary<string, object>
            {
                { "Train_No", txtTrainNo.Text.Trim() },
                { "Section_Location", txtLocation.Text.Trim() },
                { "Speed_Restriction", numSpeed.Value.ToString() },
                { "Reason", cmbReason.SelectedItem?.ToString() ?? "" },
                { "Caution_Type", cmbType.SelectedItem?.ToString() ?? "" },
                { "Issued_By", txtOfficer.Text.Trim() },
                { "Restriction_Validity", chkValidity.Checked },
                { "Loco_Pilot_Ack", chkLocoAck.Checked }
            };

            if (!ValidationHelper.ValidateForm("003", vals)) return;

            try
            {
                using (SqlCommand cmd = new SqlCommand(@"INSERT INTO Caution_Order_Entry 
                    (Caution_Order_No, Train_No, Section_Location, Speed_Restriction, Reason, Caution_Type, Date_Time_of_Issue, Issued_By, Restriction_Validity, Loco_Pilot_Ack)
                    VALUES (@ref, @train, @loc, @spd, @reason, @type, @dt, @off, @val, @ack)"))
                {
                    cmd.Parameters.AddWithValue("@ref", txtOrderNo.Text);
                    cmd.Parameters.AddWithValue("@train", txtTrainNo.Text.Trim());
                    cmd.Parameters.AddWithValue("@loc", txtLocation.Text.Trim());
                    cmd.Parameters.AddWithValue("@spd", (int)numSpeed.Value);
                    cmd.Parameters.AddWithValue("@reason", (cmbReason.SelectedItem?.ToString() ?? cmbReason.Text?.Trim() ?? ""));
                    cmd.Parameters.AddWithValue("@type", (cmbType.SelectedItem?.ToString() ?? cmbType.Text?.Trim() ?? ""));
                    cmd.Parameters.AddWithValue("@dt", dtpIssue.Value);
                    cmd.Parameters.AddWithValue("@off", txtOfficer.Text.Trim());
                    cmd.Parameters.AddWithValue("@val", chkValidity.Checked);
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
