using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace TrainWorkingApp
{
    public class Form_Auth008_ReliefTrain : Form
    {
        private readonly DatabaseHelper db = new DatabaseHelper();
        private TextBox txtAuthNo;
        private TextBox txtReliefTrain;
        private TextBox txtFailedTrain;
        private TextBox txtSection;
        private TextBox txtLocation;
        private DateTimePicker dtpIssue;
        private TextBox txtStation;
        private TextBox txtOfficer;
        private ComboBox cmbPurpose;
        private NumericUpDown numSpeed;
        private TextBox txtObstruction;
        private TextBox txtControlRef;
        private CheckBox chkProtection;

        public Form_Auth008_ReliefTrain()
        {
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Font = new Font("Segoe UI", 9.75f, FontStyle.Regular);
            InitializeComponent();
            ThemeManager.ApplyTheme(this);
            GenerateRefNo();
        }

        private void InitializeComponent()
        {
            this.Text = "AUTHORITY FOR RELIEF TRAIN INTO OCCUPIED BLOCK (T/A 602) - AUTH-008";
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
                Text = "AUTHORITY FOR RELIEF TRAIN INTO OCCUPIED BLOCK (T/A 602)",
                Font = new Font("Segoe UI", 14, FontStyle.Bold), 
                ForeColor = Color.White, 
                Location = new Point(25, 36), 
                AutoSize = true 
            };
            header.Controls.Add(lblT);

            int y = 95;
            AddLabel("Authorization Ref No (System):", 30, y);
            txtAuthNo = new TextBox { Location = new Point(290, y), Size = new Size(250, 26), ReadOnly = true, BackColor = Color.LightGray };
            this.Controls.Add(txtAuthNo);
            y += 45;
            AddLabel("Relief Train / Engine No *:", 30, y);
            txtReliefTrain = new TextBox { Location = new Point(290, y), Size = new Size(250, 26) };
            this.Controls.Add(txtReliefTrain);
            y += 45;
            AddLabel("Disabled / Failed Train No *:", 30, y);
            txtFailedTrain = new TextBox { Location = new Point(290, y), Size = new Size(250, 26) };
            this.Controls.Add(txtFailedTrain);
            y += 45;
            AddLabel("Block Section *:", 30, y);
            txtSection = new TextBox { Location = new Point(290, y), Size = new Size(350, 26) };
            this.Controls.Add(txtSection);
            y += 45;
            AddLabel("Location of Failure (KM) *:", 30, y);
            txtLocation = new TextBox { Location = new Point(290, y), Size = new Size(350, 26) };
            this.Controls.Add(txtLocation);
            y += 45;
            AddLabel("Date & Time of Issue *:", 30, y);
            dtpIssue = new DateTimePicker { Location = new Point(290, y), Size = new Size(250, 26), Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy HH:mm:ss" };
            this.Controls.Add(dtpIssue);
            y += 45;
            AddLabel("Issuing Station *:", 30, y);
            txtStation = new TextBox { Location = new Point(290, y), Size = new Size(250, 26) };
            this.Controls.Add(txtStation);
            y += 45;
            AddLabel("Issuing Officer *:", 30, y);
            txtOfficer = new TextBox { Location = new Point(290, y), Size = new Size(250, 26) };
            this.Controls.Add(txtOfficer);
            y += 45;
            AddLabel("Purpose of Relief *:", 30, y);
            cmbPurpose = new ComboBox { Location = new Point(290, y), Size = new Size(350, 26), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbPurpose.Items.AddRange(new string[] { "Clear Disabled Train", "Assist Stalled Freight", "Medical Relief Train", "Accident Relief Train" });
            cmbPurpose.SelectedIndex = -1;
            this.Controls.Add(cmbPurpose);
            y += 45;
            AddLabel("Permitted Speed (kmph) *:", 30, y);
            numSpeed = new NumericUpDown { Location = new Point(290, y), Size = new Size(120, 26), Minimum = 1, Maximum = 60, Value = 15 };
            this.Controls.Add(numSpeed);
            y += 45;
            AddLabel("Obstruction Warning *:", 30, y);
            txtObstruction = new TextBox { Location = new Point(290, y), Size = new Size(250, 26) };
            this.Controls.Add(txtObstruction);
            y += 45;
            AddLabel("Control Approval Ref *:", 30, y);
            txtControlRef = new TextBox { Location = new Point(290, y), Size = new Size(250, 26) };
            this.Controls.Add(txtControlRef);
            y += 45;
            chkProtection = new CheckBox { Text = "Protection of failed train confirmed by Guard & Station Master *", Location = new Point(290, y), Size = new Size(560, 24), Checked = false };
            this.Controls.Add(chkProtection);
            y += 30;
            y += 15;

            Button btnSave = new Button { Text = "SAVE AUTHORITY", Location = new Point(290, y), Size = new Size(160, 38) };
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            Button btnClear = new Button { Text = "CLEAR", Location = new Point(465, y), Size = new Size(100, 38) };
            btnClear.Click += (s, e) => ClearFields();
            this.Controls.Add(btnClear);

            Button btnView = new Button { Text = "VIEW RECORDS", Location = new Point(580, y), Size = new Size(140, 38) };
            btnView.Click += (s, e) => new ViewRecordsForm("Relief_Train_Authorization", "Relief Train Authorizations").ShowDialog(this);
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
            txtAuthNo.Text = "REL-AUTH-" + DateTime.Now.ToString("yyyyMMdd") + "-" + new Random().Next(100, 999);
        }

        private void ClearFields()
        {
            txtReliefTrain.Text = "";
            txtFailedTrain.Text = "";
            txtSection.Text = "";
            txtLocation.Text = "";
            dtpIssue.Value = DateTime.Now;
            txtStation.Text = "";
            txtOfficer.Text = "";
            cmbPurpose.SelectedIndex = -1;
            numSpeed.Value = 15;
            txtObstruction.Text = "";
            txtControlRef.Text = "";
            chkProtection.Checked = false;
            GenerateRefNo();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            var vals = new Dictionary<string, object>
            {
                { "Relief_Train_No", txtReliefTrain.Text.Trim() },
                { "Failed_Train_No", txtFailedTrain.Text.Trim() },
                { "Block_Section", txtSection.Text.Trim() },
                { "Location_of_Failure", txtLocation.Text.Trim() },
                { "Issuing_Station", txtStation.Text.Trim() },
                { "Issuing_Officer", txtOfficer.Text.Trim() },
                { "Purpose_of_Relief", cmbPurpose.SelectedItem?.ToString() ?? "" },
                { "Permitted_Speed", numSpeed.Value.ToString() },
                { "Obstruction_Warning", txtObstruction.Text.Trim() },
                { "Control_Approval_Ref", txtControlRef.Text.Trim() },
                { "Protection_Confirmed", chkProtection.Checked }
            };

            if (!ValidationHelper.ValidateForm("008", vals)) return;

            try
            {
                using (SqlCommand cmd = new SqlCommand(@"INSERT INTO Relief_Train_Authorization 
                    (Authorization_Reference_No, Relief_Train_No, Failed_Train_No, Block_Section, Location_of_Failure, Date_Time_of_Issue, Issuing_Station, Issuing_Officer, Purpose_of_Relief, Permitted_Speed, Obstruction_Warning, Protection_Confirmed, Control_Approval_Ref)
                    VALUES (@ref, @rt, @ft, @sec, @loc, @dt, @stn, @off, @purp, @spd, @obs, @prot, @ctrl)"))
                {
                    cmd.Parameters.AddWithValue("@ref", txtAuthNo.Text);
                    cmd.Parameters.AddWithValue("@rt", txtReliefTrain.Text.Trim());
                    cmd.Parameters.AddWithValue("@ft", txtFailedTrain.Text.Trim());
                    cmd.Parameters.AddWithValue("@sec", txtSection.Text.Trim());
                    cmd.Parameters.AddWithValue("@loc", txtLocation.Text.Trim());
                    cmd.Parameters.AddWithValue("@dt", dtpIssue.Value);
                    cmd.Parameters.AddWithValue("@stn", txtStation.Text.Trim());
                    cmd.Parameters.AddWithValue("@off", txtOfficer.Text.Trim());
                    cmd.Parameters.AddWithValue("@purp", (cmbPurpose.SelectedItem?.ToString() ?? cmbPurpose.Text?.Trim() ?? ""));
                    cmd.Parameters.AddWithValue("@spd", (int)numSpeed.Value);
                    cmd.Parameters.AddWithValue("@obs", txtObstruction.Text.Trim());
                    cmd.Parameters.AddWithValue("@prot", chkProtection.Checked);
                    cmd.Parameters.AddWithValue("@ctrl", txtControlRef.Text.Trim());

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
