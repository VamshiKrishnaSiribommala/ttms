using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace TrainWorkingApp
{
    public class Form_Auth015_ABSReliefEngine : Form
    {
        private readonly DatabaseHelper db = new DatabaseHelper();
        private TextBox txtAuthNo;
        private TextBox txtReliefTrain;
        private TextBox txtBlockSection;
        private TextBox txtReason;
        private DateTimePicker dtpIssueTime;
        private TextBox txtIssuedBy;
        private NumericUpDown numValidity;
        private DateTimePicker dtpEntry;
        private DateTimePicker dtpExit;
        private TextBox txtRemarks;

        public Form_Auth015_ABSReliefEngine()
        {
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Font = new Font("Segoe UI", 9.75f, FontStyle.Regular);
            InitializeComponent();
            ThemeManager.ApplyTheme(this);
            GenerateRefNo();
        }

        private void InitializeComponent()
        {
            this.Text = "AUTHORITY FOR RELIEF ENGINE TO ENTER OCCUPIED ABS (T/A 912) - AUTH-015";
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
                Text = "AUTHORITY FOR RELIEF ENGINE TO ENTER OCCUPIED ABS (T/A 912)",
                Font = new Font("Segoe UI", 14, FontStyle.Bold), 
                ForeColor = Color.White, 
                Location = new Point(25, 36), 
                AutoSize = true 
            };
            header.Controls.Add(lblT);

            int y = 95;
            AddLabel("Authority Number (System):", 30, y);
            txtAuthNo = new TextBox { Location = new Point(290, y), Size = new Size(250, 26), ReadOnly = true, BackColor = Color.LightGray };
            this.Controls.Add(txtAuthNo);
            y += 45;
            AddLabel("Relief Train / Engine No *:", 30, y);
            txtReliefTrain = new TextBox { Location = new Point(290, y), Size = new Size(250, 26) };
            this.Controls.Add(txtReliefTrain);
            y += 45;
            AddLabel("Block Section *:", 30, y);
            txtBlockSection = new TextBox { Location = new Point(290, y), Size = new Size(350, 26) };
            this.Controls.Add(txtBlockSection);
            y += 45;
            AddLabel("Reason *:", 30, y);
            txtReason = new TextBox { Location = new Point(290, y), Size = new Size(350, 26) };
            this.Controls.Add(txtReason);
            y += 45;
            AddLabel("Issue Time *:", 30, y);
            dtpIssueTime = new DateTimePicker { Location = new Point(290, y), Size = new Size(250, 26), Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy HH:mm:ss" };
            this.Controls.Add(dtpIssueTime);
            y += 45;
            AddLabel("Issued By *:", 30, y);
            txtIssuedBy = new TextBox { Location = new Point(290, y), Size = new Size(250, 26) };
            this.Controls.Add(txtIssuedBy);
            y += 45;
            AddLabel("Validity Duration (Mins) *:", 30, y);
            numValidity = new NumericUpDown { Location = new Point(290, y), Size = new Size(120, 26), Minimum = 10, Maximum = 240, Value = 60 };
            this.Controls.Add(numValidity);
            y += 45;
            AddLabel("Entry Time *:", 30, y);
            dtpEntry = new DateTimePicker { Location = new Point(290, y), Size = new Size(250, 26), Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy HH:mm:ss" };
            this.Controls.Add(dtpEntry);
            y += 45;
            AddLabel("Exit Time *:", 30, y);
            dtpExit = new DateTimePicker { Location = new Point(290, y), Size = new Size(250, 26), Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy HH:mm:ss" };
            this.Controls.Add(dtpExit);
            y += 45;
            AddLabel("Remarks *:", 30, y);
            txtRemarks = new TextBox { Location = new Point(290, y), Size = new Size(450, 26) };
            this.Controls.Add(txtRemarks);
            y += 45;

            Button btnSave = new Button { Text = "SAVE AUTHORITY", Location = new Point(290, y), Size = new Size(160, 38) };
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            Button btnClear = new Button { Text = "CLEAR", Location = new Point(465, y), Size = new Size(100, 38) };
            btnClear.Click += (s, e) => ClearFields();
            this.Controls.Add(btnClear);

            Button btnView = new Button { Text = "VIEW RECORDS", Location = new Point(580, y), Size = new Size(140, 38) };
            btnView.Click += (s, e) => new ViewRecordsForm("ABS_Relief_Engine_Authority", "ABS Relief Engine Authorities").ShowDialog(this);
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
            txtAuthNo.Text = "ABS-RT-" + DateTime.Now.ToString("yyyyMMdd") + "-" + new Random().Next(100, 999);
        }

        private void ClearFields()
        {
            txtReliefTrain.Text = "";
            txtBlockSection.Text = "";
            txtReason.Text = "";
            dtpIssueTime.Value = DateTime.Now;
            txtIssuedBy.Text = "";
            numValidity.Value = 60;
            dtpEntry.Value = DateTime.Now;
            dtpExit.Value = DateTime.Now;
            txtRemarks.Text = "";
            GenerateRefNo();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            var vals = new Dictionary<string, object>
            {
                { "Relief_Train_Engine_No", txtReliefTrain.Text.Trim() },
                { "Block_Section", txtBlockSection.Text.Trim() },
                { "Reason", txtReason.Text.Trim() },
                { "Issued_By", txtIssuedBy.Text.Trim() },
                { "Validity_Duration", numValidity.Value.ToString() },
                { "Remarks", txtRemarks.Text.Trim() }
            };

            if (!ValidationHelper.ValidateForm("015", vals)) return;

            try
            {
                using (SqlCommand cmd = new SqlCommand(@"INSERT INTO ABS_Relief_Engine_Authority 
                    (Authority_Number, Relief_Train_Engine_No, Block_Section, Reason, Issue_Time, Issued_By, Validity_Duration, Entry_Time, Exit_Time, Authority_Closure_Status, Remarks)
                    VALUES (@ref, @rt, @sec, @reason, @dt, @by, @val, @entry, @exit, 0, @rem)"))
                {
                    cmd.Parameters.AddWithValue("@ref", txtAuthNo.Text);
                    cmd.Parameters.AddWithValue("@rt", txtReliefTrain.Text.Trim());
                    cmd.Parameters.AddWithValue("@sec", txtBlockSection.Text.Trim());
                    cmd.Parameters.AddWithValue("@reason", txtReason.Text.Trim());
                    cmd.Parameters.AddWithValue("@dt", dtpIssueTime.Value);
                    cmd.Parameters.AddWithValue("@by", txtIssuedBy.Text.Trim());
                    cmd.Parameters.AddWithValue("@val", (int)numValidity.Value);
                    cmd.Parameters.AddWithValue("@entry", dtpEntry.Value);
                    cmd.Parameters.AddWithValue("@exit", dtpExit.Value);
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
