using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace TrainWorkingApp
{
    public class Form_Auth010_LineClearInquiry : Form
    {
        private readonly DatabaseHelper db = new DatabaseHelper();
        private TextBox txtInquiryNo;
        private TextBox txtSendingStation;
        private TextBox txtReceivingStation;
        private DateTimePicker dtpSent;
        private ComboBox cmbMethod;
        private ComboBox cmbResponse;
        private DateTimePicker dtpResponse;

        public Form_Auth010_LineClearInquiry()
        {
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Font = new Font("Segoe UI", 9.75f, FontStyle.Regular);
            InitializeComponent();
            ThemeManager.ApplyTheme(this);
            GenerateRefNo();
        }

        private void InitializeComponent()
        {
            this.Text = "LINE CLEAR INQUIRY & PERMISSION (TFC) - AUTH-010";
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
                Text = "LINE CLEAR INQUIRY & PERMISSION (TFC)",
                Font = new Font("Segoe UI", 14, FontStyle.Bold), 
                ForeColor = Color.White, 
                Location = new Point(25, 36), 
                AutoSize = true 
            };
            header.Controls.Add(lblT);

            int y = 95;
            AddLabel("Inquiry Ref No (System):", 30, y);
            txtInquiryNo = new TextBox { Location = new Point(290, y), Size = new Size(250, 26), ReadOnly = true, BackColor = Color.LightGray };
            this.Controls.Add(txtInquiryNo);
            y += 45;
            AddLabel("Sending Station *:", 30, y);
            txtSendingStation = new TextBox { Location = new Point(290, y), Size = new Size(250, 26) };
            this.Controls.Add(txtSendingStation);
            y += 45;
            AddLabel("Receiving Station *:", 30, y);
            txtReceivingStation = new TextBox { Location = new Point(290, y), Size = new Size(250, 26) };
            this.Controls.Add(txtReceivingStation);
            y += 45;
            AddLabel("Date & Time Sent *:", 30, y);
            dtpSent = new DateTimePicker { Location = new Point(290, y), Size = new Size(250, 26), Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy HH:mm:ss" };
            this.Controls.Add(dtpSent);
            y += 45;
            AddLabel("Transmission Method *:", 30, y);
            cmbMethod = new ComboBox { Location = new Point(290, y), Size = new Size(350, 26), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbMethod.Items.AddRange(new string[] { "Block Instrument Line Clear", "Station-to-Station Telephone", "Control Phone (TC)", "VHF Communication" });
            cmbMethod.SelectedIndex = -1;
            this.Controls.Add(cmbMethod);
            y += 45;
            AddLabel("Line Clear Response *:", 30, y);
            cmbResponse = new ComboBox { Location = new Point(290, y), Size = new Size(350, 26), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbResponse.Items.AddRange(new string[] { "Line Clear Granted", "Line Clear Refused", "Conditional Permission", "Inquiry Acknowledged" });
            cmbResponse.SelectedIndex = -1;
            this.Controls.Add(cmbResponse);
            y += 45;
            AddLabel("Response Time *:", 30, y);
            dtpResponse = new DateTimePicker { Location = new Point(290, y), Size = new Size(250, 26), Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy HH:mm:ss" };
            this.Controls.Add(dtpResponse);
            y += 45;

            Button btnSave = new Button { Text = "SAVE AUTHORITY", Location = new Point(290, y), Size = new Size(160, 38) };
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            Button btnClear = new Button { Text = "CLEAR", Location = new Point(465, y), Size = new Size(100, 38) };
            btnClear.Click += (s, e) => ClearFields();
            this.Controls.Add(btnClear);

            Button btnView = new Button { Text = "VIEW RECORDS", Location = new Point(580, y), Size = new Size(140, 38) };
            btnView.Click += (s, e) => new ViewRecordsForm("Line_Clear_Inquiry_TFC", "Line Clear Inquiries").ShowDialog(this);
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
            txtInquiryNo.Text = "TFC-INQ-" + DateTime.Now.ToString("yyyyMMdd") + "-" + new Random().Next(100, 999);
        }

        private void ClearFields()
        {
            txtSendingStation.Text = "";
            txtReceivingStation.Text = "";
            dtpSent.Value = DateTime.Now;
            cmbMethod.SelectedIndex = -1;
            cmbResponse.SelectedIndex = -1;
            dtpResponse.Value = DateTime.Now;
            GenerateRefNo();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            var vals = new Dictionary<string, object>
            {
                { "Sending_Station", txtSendingStation.Text.Trim() },
                { "Receiving_Station", txtReceivingStation.Text.Trim() },
                { "Transmission_Method", cmbMethod.SelectedItem?.ToString() ?? "" },
                { "Line_Clear_Response", cmbResponse.SelectedItem?.ToString() ?? "" }
            };

            if (!ValidationHelper.ValidateForm("010", vals)) return;

            try
            {
                using (SqlCommand cmd = new SqlCommand(@"INSERT INTO Line_Clear_Inquiry_TFC 
                    (Inquiry_Reference_No, Sending_Station, Receiving_Station, Date_Time_Sent, Transmission_Method, Line_Clear_Response, Response_Time)
                    VALUES (@ref, @send, @recv, @sentDt, @meth, @resp, @respDt)"))
                {
                    cmd.Parameters.AddWithValue("@ref", txtInquiryNo.Text);
                    cmd.Parameters.AddWithValue("@send", txtSendingStation.Text.Trim());
                    cmd.Parameters.AddWithValue("@recv", txtReceivingStation.Text.Trim());
                    cmd.Parameters.AddWithValue("@sentDt", dtpSent.Value);
                    cmd.Parameters.AddWithValue("@meth", (cmbMethod.SelectedItem?.ToString() ?? cmbMethod.Text?.Trim() ?? ""));
                    cmd.Parameters.AddWithValue("@resp", (cmbResponse.SelectedItem?.ToString() ?? cmbResponse.Text?.Trim() ?? ""));
                    cmd.Parameters.AddWithValue("@respDt", dtpResponse.Value);

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
