using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace TrainWorkingApp
{
    public class Form_Auth019_MaintenanceTrolleyNotice : Form
    {
        private readonly DatabaseHelper db = new DatabaseHelper();
        private ComboBox cmbType;
        private TextBox txtSection;
        private TextBox txtWorkDuration;
        private TextBox txtStaff;
        private TextBox txtPrecautions;

        public Form_Auth019_MaintenanceTrolleyNotice()
        {
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Font = new Font("Segoe UI", 9.75f, FontStyle.Regular);
            InitializeComponent();
            ThemeManager.ApplyTheme(this);
        }

        private void InitializeComponent()
        {
            this.Text = "MAINTENANCE TROLLEY NOTICE (TRACK & OHE) - AUTH-019";
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
                Text = "MAINTENANCE TROLLEY NOTICE (TRACK & OHE)",
                Font = new Font("Segoe UI", 14, FontStyle.Bold), 
                ForeColor = Color.White, 
                Location = new Point(25, 36), 
                AutoSize = true 
            };
            header.Controls.Add(lblT);

            int y = 95;
            AddLabel("Trolley Type *:", 30, y);
            cmbType = new ComboBox { Location = new Point(290, y), Size = new Size(350, 26), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbType.Items.AddRange(new string[] { "Push Trolley", "Cycle Trolley", "Moped Trolley", "Tower Wagon (Inspection)" });
            cmbType.SelectedIndex = -1;
            this.Controls.Add(cmbType);
            y += 45;
            AddLabel("Track Section *:", 30, y);
            txtSection = new TextBox { Location = new Point(290, y), Size = new Size(350, 26) };
            this.Controls.Add(txtSection);
            y += 45;
            AddLabel("Work Duration (e.g. 2 Hours) *:", 30, y);
            txtWorkDuration = new TextBox { Location = new Point(290, y), Size = new Size(250, 26) };
            this.Controls.Add(txtWorkDuration);
            y += 45;
            AddLabel("Staff In Charge *:", 30, y);
            txtStaff = new TextBox { Location = new Point(290, y), Size = new Size(250, 26) };
            this.Controls.Add(txtStaff);
            y += 45;
            AddLabel("Safety Precautions *:", 30, y);
            txtPrecautions = new TextBox { Location = new Point(290, y), Size = new Size(450, 26) };
            this.Controls.Add(txtPrecautions);
            y += 45;

            Button btnSave = new Button { Text = "SAVE AUTHORITY", Location = new Point(290, y), Size = new Size(160, 38) };
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            Button btnClear = new Button { Text = "CLEAR", Location = new Point(465, y), Size = new Size(100, 38) };
            btnClear.Click += (s, e) => ClearFields();
            this.Controls.Add(btnClear);

            Button btnView = new Button { Text = "VIEW RECORDS", Location = new Point(580, y), Size = new Size(140, 38) };
            btnView.Click += (s, e) => new ViewRecordsForm("Maintenance_Trolley_Notice", "Maintenance Trolley Notices").ShowDialog(this);
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



        private void ClearFields()
        {
            cmbType.SelectedIndex = -1;
            txtSection.Text = "";
            txtWorkDuration.Text = "";
            txtStaff.Text = "";
            txtPrecautions.Text = "";
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            var vals = new Dictionary<string, object>
            {
                { "Type", cmbType.SelectedItem?.ToString() ?? "" },
                { "Section", txtSection.Text.Trim() },
                { "Work_Duration", txtWorkDuration.Text.Trim() },
                { "Staff_In_Charge", txtStaff.Text.Trim() },
                { "Safety_Precautions", txtPrecautions.Text.Trim() }
            };

            if (!ValidationHelper.ValidateForm("019", vals)) return;

            try
            {
                using (SqlCommand cmd = new SqlCommand(@"INSERT INTO Maintenance_Trolley_Notice 
                    (Notice_ID, Type, Section, Work_Duration, Staff_In_Charge, Safety_Precautions)
                    VALUES (@nid, @type, @sec, @dur, @staff, @prec)"))
                {
                    cmd.Parameters.AddWithValue("@nid", "MTN-" + DateTime.Now.ToString("yyyyMMddHHmmss") + "-" + new Random().Next(100, 999));
                    cmd.Parameters.AddWithValue("@type", (cmbType.SelectedItem?.ToString() ?? cmbType.Text?.Trim() ?? ""));
                    cmd.Parameters.AddWithValue("@sec", txtSection.Text.Trim());
                    cmd.Parameters.AddWithValue("@dur", txtWorkDuration.Text.Trim());
                    cmd.Parameters.AddWithValue("@staff", txtStaff.Text.Trim());
                    cmd.Parameters.AddWithValue("@prec", txtPrecautions.Text.Trim());

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
