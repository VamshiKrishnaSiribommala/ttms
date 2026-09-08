using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace TrainWorkingApp
{
    public class Form_Auth012_ShuntingOrder : Form
    {
        private readonly DatabaseHelper db = new DatabaseHelper();
        private TextBox txtOrderNo;
        private TextBox txtStation;
        private TextBox txtEngineNo;
        private TextBox txtYardLimits;
        private DateTimePicker dtpDateTime;
        private TextBox txtIssuedBy;
        private TextBox txtInstructions;

        public Form_Auth012_ShuntingOrder()
        {
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Font = new Font("Segoe UI", 9.75f, FontStyle.Regular);
            InitializeComponent();
            ThemeManager.ApplyTheme(this);
            GenerateRefNo();
        }

        private void InitializeComponent()
        {
            this.Text = "SHUNTING ORDER MANAGEMENT (T/806) - AUTH-012";
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
                Text = "SHUNTING ORDER MANAGEMENT (T/806)",
                Font = new Font("Segoe UI", 14, FontStyle.Bold), 
                ForeColor = Color.White, 
                Location = new Point(25, 36), 
                AutoSize = true 
            };
            header.Controls.Add(lblT);

            int y = 95;
            AddLabel("Shunting Order No (System):", 30, y);
            txtOrderNo = new TextBox { Location = new Point(290, y), Size = new Size(250, 26), ReadOnly = true, BackColor = Color.LightGray };
            this.Controls.Add(txtOrderNo);
            y += 45;
            AddLabel("Station *:", 30, y);
            txtStation = new TextBox { Location = new Point(290, y), Size = new Size(250, 26) };
            this.Controls.Add(txtStation);
            y += 45;
            AddLabel("Engine No / Shunting Train *:", 30, y);
            txtEngineNo = new TextBox { Location = new Point(290, y), Size = new Size(250, 26) };
            this.Controls.Add(txtEngineNo);
            y += 45;
            AddLabel("Yard Limits / Lines Authorized *:", 30, y);
            txtYardLimits = new TextBox { Location = new Point(290, y), Size = new Size(350, 26) };
            this.Controls.Add(txtYardLimits);
            y += 45;
            AddLabel("Date & Time *:", 30, y);
            dtpDateTime = new DateTimePicker { Location = new Point(290, y), Size = new Size(250, 26), Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy HH:mm:ss" };
            this.Controls.Add(dtpDateTime);
            y += 45;
            AddLabel("Issued By *:", 30, y);
            txtIssuedBy = new TextBox { Location = new Point(290, y), Size = new Size(250, 26) };
            this.Controls.Add(txtIssuedBy);
            y += 45;
            AddLabel("Instructions *:", 30, y);
            txtInstructions = new TextBox { Location = new Point(290, y), Size = new Size(450, 26) };
            this.Controls.Add(txtInstructions);
            y += 45;

            Button btnSave = new Button { Text = "SAVE AUTHORITY", Location = new Point(290, y), Size = new Size(160, 38) };
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            Button btnClear = new Button { Text = "CLEAR", Location = new Point(465, y), Size = new Size(100, 38) };
            btnClear.Click += (s, e) => ClearFields();
            this.Controls.Add(btnClear);

            Button btnView = new Button { Text = "VIEW RECORDS", Location = new Point(580, y), Size = new Size(140, 38) };
            btnView.Click += (s, e) => new ViewRecordsForm("Shunting_Order_Management", "Shunting Orders").ShowDialog(this);
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
            txtOrderNo.Text = "SO-" + DateTime.Now.ToString("yyyyMMdd") + "-" + new Random().Next(100, 999);
        }

        private void ClearFields()
        {
            txtStation.Text = "";
            txtEngineNo.Text = "";
            txtYardLimits.Text = "";
            dtpDateTime.Value = DateTime.Now;
            txtIssuedBy.Text = "";
            txtInstructions.Text = "";
            GenerateRefNo();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            var vals = new Dictionary<string, object>
            {
                { "Station", txtStation.Text.Trim() },
                { "Engine_No", txtEngineNo.Text.Trim() },
                { "Yard_Limits", txtYardLimits.Text.Trim() },
                { "Issued_By", txtIssuedBy.Text.Trim() },
                { "Instructions", txtInstructions.Text.Trim() }
            };

            if (!ValidationHelper.ValidateForm("012", vals)) return;

            try
            {
                using (SqlCommand cmd = new SqlCommand(@"INSERT INTO Shunting_Order_Management 
                    (Shunting_Order_No, Station, Engine_No, Yard_Limits, Date_Time, Issued_By, Instructions)
                    VALUES (@ref, @stn, @eng, @yard, @dt, @by, @inst)"))
                {
                    cmd.Parameters.AddWithValue("@ref", txtOrderNo.Text);
                    cmd.Parameters.AddWithValue("@stn", txtStation.Text.Trim());
                    cmd.Parameters.AddWithValue("@eng", txtEngineNo.Text.Trim());
                    cmd.Parameters.AddWithValue("@yard", txtYardLimits.Text.Trim());
                    cmd.Parameters.AddWithValue("@dt", dtpDateTime.Value);
                    cmd.Parameters.AddWithValue("@by", txtIssuedBy.Text.Trim());
                    cmd.Parameters.AddWithValue("@inst", txtInstructions.Text.Trim());

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
