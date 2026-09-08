using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace TrainWorkingApp
{
    public class Form_Auth018_LineClearTickets : Form
    {
        private readonly DatabaseHelper db = new DatabaseHelper();
        private TextBox txtTicketNo;
        private TextBox txtFromStation;
        private TextBox txtToStation;
        private TextBox txtTrainNo;
        private ComboBox cmbDirection;
        private DateTimePicker dtpInquiryTime;
        private ComboBox cmbReply;
        private DateTimePicker dtpReplyTime;
        private TextBox txtReason;
        private DateTimePicker dtpIssuedTime;

        public Form_Auth018_LineClearTickets()
        {
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Font = new Font("Segoe UI", 9.75f, FontStyle.Regular);
            InitializeComponent();
            ThemeManager.ApplyTheme(this);
            GenerateRefNo();
        }

        private void InitializeComponent()
        {
            this.Text = "PAPER LINE CLEAR TICKETS (T/A 1425 UP & T/B 1425 DOWN) - AUTH-018";
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
                Text = "PAPER LINE CLEAR TICKETS (T/A 1425 UP & T/B 1425 DOWN)",
                Font = new Font("Segoe UI", 14, FontStyle.Bold), 
                ForeColor = Color.White, 
                Location = new Point(25, 36), 
                AutoSize = true 
            };
            header.Controls.Add(lblT);

            int y = 95;
            AddLabel("Ticket Number (System):", 30, y);
            txtTicketNo = new TextBox { Location = new Point(290, y), Size = new Size(250, 26), ReadOnly = true, BackColor = Color.LightGray };
            this.Controls.Add(txtTicketNo);
            y += 45;
            AddLabel("From Station *:", 30, y);
            txtFromStation = new TextBox { Location = new Point(290, y), Size = new Size(250, 26) };
            this.Controls.Add(txtFromStation);
            y += 45;
            AddLabel("To Station *:", 30, y);
            txtToStation = new TextBox { Location = new Point(290, y), Size = new Size(250, 26) };
            this.Controls.Add(txtToStation);
            y += 45;
            AddLabel("Train Number *:", 30, y);
            txtTrainNo = new TextBox { Location = new Point(290, y), Size = new Size(250, 26) };
            this.Controls.Add(txtTrainNo);
            y += 45;
            AddLabel("Direction *:", 30, y);
            cmbDirection = new ComboBox { Location = new Point(290, y), Size = new Size(350, 26), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbDirection.Items.AddRange(new string[] { "UP Line (T/A 1425)", "DOWN Line (T/B 1425)", "Single Line UP", "Single Line DOWN" });
            cmbDirection.SelectedIndex = -1;
            this.Controls.Add(cmbDirection);
            y += 45;
            AddLabel("Inquiry Time *:", 30, y);
            dtpInquiryTime = new DateTimePicker { Location = new Point(290, y), Size = new Size(250, 26), Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy HH:mm:ss" };
            this.Controls.Add(dtpInquiryTime);
            y += 45;
            AddLabel("Reply Status *:", 30, y);
            cmbReply = new ComboBox { Location = new Point(290, y), Size = new Size(350, 26), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbReply.Items.AddRange(new string[] { "Line Clear Received", "Conditional Ticket", "Special Line Clear Order" });
            cmbReply.SelectedIndex = -1;
            this.Controls.Add(cmbReply);
            y += 45;
            AddLabel("Reply Time *:", 30, y);
            dtpReplyTime = new DateTimePicker { Location = new Point(290, y), Size = new Size(250, 26), Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy HH:mm:ss" };
            this.Controls.Add(dtpReplyTime);
            y += 45;
            AddLabel("Reason If Not Clear *:", 30, y);
            txtReason = new TextBox { Location = new Point(290, y), Size = new Size(350, 26) };
            this.Controls.Add(txtReason);
            y += 45;
            AddLabel("Issued Time *:", 30, y);
            dtpIssuedTime = new DateTimePicker { Location = new Point(290, y), Size = new Size(250, 26), Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy HH:mm:ss" };
            this.Controls.Add(dtpIssuedTime);
            y += 45;

            Button btnSave = new Button { Text = "SAVE AUTHORITY", Location = new Point(290, y), Size = new Size(160, 38) };
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            Button btnClear = new Button { Text = "CLEAR", Location = new Point(465, y), Size = new Size(100, 38) };
            btnClear.Click += (s, e) => ClearFields();
            this.Controls.Add(btnClear);

            Button btnView = new Button { Text = "VIEW RECORDS", Location = new Point(580, y), Size = new Size(140, 38) };
            btnView.Click += (s, e) => new ViewRecordsForm("Line_Clear_Tickets", "Paper Line Clear Tickets").ShowDialog(this);
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
            txtTicketNo.Text = "PLCT-" + DateTime.Now.ToString("yyyyMMdd") + "-" + new Random().Next(100, 999);
        }

        private void ClearFields()
        {
            txtFromStation.Text = "";
            txtToStation.Text = "";
            txtTrainNo.Text = "";
            cmbDirection.SelectedIndex = -1;
            dtpInquiryTime.Value = DateTime.Now;
            cmbReply.SelectedIndex = -1;
            dtpReplyTime.Value = DateTime.Now;
            txtReason.Text = "";
            dtpIssuedTime.Value = DateTime.Now;
            GenerateRefNo();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            var vals = new Dictionary<string, object>
            {
                { "From_Station", txtFromStation.Text.Trim() },
                { "To_Station", txtToStation.Text.Trim() },
                { "Train_Number", txtTrainNo.Text.Trim() },
                { "Direction", cmbDirection.SelectedItem?.ToString() ?? "" },
                { "Reply_Status", cmbReply.SelectedItem?.ToString() ?? "" },
                { "Reason_If_Not_Clear", txtReason.Text.Trim() }
            };

            if (!ValidationHelper.ValidateForm("018", vals)) return;

            try
            {
                using (SqlCommand cmd = new SqlCommand(@"INSERT INTO Line_Clear_Tickets 
                    (Inquiry_ID, From_Station, To_Station, Train_Number, Direction, Inquiry_Time, Reply_Status, Reply_Time, Reason_If_Not_Clear, Ticket_Number, Issued_Time, Issued_By)
                    VALUES (@inqId, @from, @to, @train, @dir, @inq, @rep, @repTime, @reason, @tkt, @iss, @issuedBy)"))
                {
                    cmd.Parameters.AddWithValue("@inqId", "INQ-" + DateTime.Now.ToString("yyyyMMddHHmmss"));
                    cmd.Parameters.AddWithValue("@from", txtFromStation.Text.Trim());
                    cmd.Parameters.AddWithValue("@to", txtToStation.Text.Trim());
                    cmd.Parameters.AddWithValue("@train", txtTrainNo.Text.Trim());
                    cmd.Parameters.AddWithValue("@dir", (cmbDirection.SelectedItem?.ToString() ?? cmbDirection.Text?.Trim() ?? ""));
                    cmd.Parameters.AddWithValue("@inq", dtpInquiryTime.Value);
                    cmd.Parameters.AddWithValue("@rep", (cmbReply.SelectedItem?.ToString() ?? cmbReply.Text?.Trim() ?? ""));
                    cmd.Parameters.AddWithValue("@repTime", dtpReplyTime.Value);
                    cmd.Parameters.AddWithValue("@reason", txtReason.Text.Trim());
                    cmd.Parameters.AddWithValue("@tkt", txtTicketNo.Text);
                    cmd.Parameters.AddWithValue("@iss", dtpIssuedTime.Value);
                    cmd.Parameters.AddWithValue("@issuedBy", SessionManager.CurrentFullName ?? "Station Operator");

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
