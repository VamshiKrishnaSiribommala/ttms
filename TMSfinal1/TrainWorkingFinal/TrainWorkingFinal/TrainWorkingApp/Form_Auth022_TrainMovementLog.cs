using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace TrainWorkingApp
{
    public class Form_Auth022_TrainMovementLog : Form
    {
        private readonly DatabaseHelper db = new DatabaseHelper();
        private TextBox txtTrainNo;
        private TextBox txtTrainName;
        private ComboBox cmbDirection;
        private TextBox txtSection;
        private DateTimePicker dtpSchedArr;
        private DateTimePicker dtpActArr;
        private DateTimePicker dtpSchedDep;
        private DateTimePicker dtpActDep;
        private NumericUpDown numHalt;
        private ComboBox cmbRunningStatus;
        private TextBox txtRemarks;

        public Form_Auth022_TrainMovementLog()
        {
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Font = new Font("Segoe UI", 9.75f, FontStyle.Regular);
            InitializeComponent();
            ThemeManager.ApplyTheme(this);
        }

        private void InitializeComponent()
        {
            this.Text = "TRAIN MOVEMENT & STATION HALT LOG - AUTH-022";
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
                Text = "TRAIN MOVEMENT & STATION HALT LOG",
                Font = new Font("Segoe UI", 14, FontStyle.Bold), 
                ForeColor = Color.White, 
                Location = new Point(25, 36), 
                AutoSize = true 
            };
            header.Controls.Add(lblT);

            int y = 95;
            AddLabel("Train Number *:", 30, y);
            txtTrainNo = new TextBox { Location = new Point(290, y), Size = new Size(250, 26) };
            this.Controls.Add(txtTrainNo);
            y += 45;
            AddLabel("Train Name *:", 30, y);
            txtTrainName = new TextBox { Location = new Point(290, y), Size = new Size(350, 26) };
            this.Controls.Add(txtTrainName);
            y += 45;
            AddLabel("Direction *:", 30, y);
            cmbDirection = new ComboBox { Location = new Point(290, y), Size = new Size(350, 26), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbDirection.Items.AddRange(new string[] { "UP Main", "DOWN Main", "Loop 1", "Loop 2", "Goods Line", "Siding Line" });
            cmbDirection.SelectedIndex = -1;
            this.Controls.Add(cmbDirection);
            y += 45;
            AddLabel("Station / Platform *:", 30, y);
            txtSection = new TextBox { Location = new Point(290, y), Size = new Size(250, 26) };
            this.Controls.Add(txtSection);
            y += 45;
            AddLabel("Scheduled Arrival Time *:", 30, y);
            dtpSchedArr = new DateTimePicker { Location = new Point(290, y), Size = new Size(250, 26), Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy HH:mm:ss" };
            this.Controls.Add(dtpSchedArr);
            y += 45;
            AddLabel("Actual Arrival Time *:", 30, y);
            dtpActArr = new DateTimePicker { Location = new Point(290, y), Size = new Size(250, 26), Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy HH:mm:ss" };
            this.Controls.Add(dtpActArr);
            y += 45;
            AddLabel("Scheduled Departure Time *:", 30, y);
            dtpSchedDep = new DateTimePicker { Location = new Point(290, y), Size = new Size(250, 26), Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy HH:mm:ss" };
            this.Controls.Add(dtpSchedDep);
            y += 45;
            AddLabel("Actual Departure Time *:", 30, y);
            dtpActDep = new DateTimePicker { Location = new Point(290, y), Size = new Size(250, 26), Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy HH:mm:ss" };
            this.Controls.Add(dtpActDep);
            y += 45;
            AddLabel("Halt Duration (Minutes) *:", 30, y);
            numHalt = new NumericUpDown { Location = new Point(290, y), Size = new Size(120, 26), Minimum = 0, Maximum = 720, Value = 2 };
            this.Controls.Add(numHalt);
            y += 45;
            AddLabel("Running Status *:", 30, y);
            cmbRunningStatus = new ComboBox { Location = new Point(290, y), Size = new Size(350, 26), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbRunningStatus.Items.AddRange(new string[] { "Right Time (RT)", "Delayed / Running Late", "Pre-poned / Early", "Regulated in Section", "Diverted / Short Terminated" });
            cmbRunningStatus.SelectedIndex = -1;
            this.Controls.Add(cmbRunningStatus);
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
            btnView.Click += (s, e) => new ViewRecordsForm("Train_Movement_Log", "Train Movement & Station Halt Log").ShowDialog(this);
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
            txtTrainNo.Text = "";
            txtTrainName.Text = "";
            cmbDirection.SelectedIndex = -1;
            txtSection.Text = "";
            dtpSchedArr.Value = DateTime.Now;
            dtpActArr.Value = DateTime.Now;
            dtpSchedDep.Value = DateTime.Now;
            dtpActDep.Value = DateTime.Now;
            numHalt.Value = 2;
            cmbRunningStatus.SelectedIndex = -1;
            txtRemarks.Text = "";
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            var vals = new Dictionary<string, object>
            {
                { "Train_Number", txtTrainNo.Text.Trim() },
                { "Train_Name", txtTrainName.Text.Trim() },
                { "Direction", cmbDirection.SelectedItem?.ToString() ?? "" },
                { "Section_Station", txtSection.Text.Trim() },
                { "Halt_Duration", numHalt.Value.ToString() },
                { "Running_Status", cmbRunningStatus.SelectedItem?.ToString() ?? "" },
                { "Remarks", txtRemarks.Text.Trim() }
            };

            if (!ValidationHelper.ValidateForm("022", vals)) return;

            try
            {
                using (SqlCommand cmd = new SqlCommand(@"INSERT INTO Train_Movement_Log 
                    (Train_Number, Train_Name, Direction, Section_Station, Scheduled_Arrival_Time, Actual_Arrival_Time, Scheduled_Departure_Time, Actual_Departure_Time, Halt_Duration, Running_Status, Remarks)
                    VALUES (@train, @name, @dir, @sec, @sArr, @aArr, @sDep, @aDep, @halt, @stat, @rem)"))
                {
                    cmd.Parameters.AddWithValue("@train", txtTrainNo.Text.Trim());
                    cmd.Parameters.AddWithValue("@name", txtTrainName.Text.Trim());
                    cmd.Parameters.AddWithValue("@dir", (cmbDirection.SelectedItem?.ToString() ?? cmbDirection.Text?.Trim() ?? ""));
                    cmd.Parameters.AddWithValue("@sec", txtSection.Text.Trim());
                    cmd.Parameters.AddWithValue("@sArr", dtpSchedArr.Value);
                    cmd.Parameters.AddWithValue("@aArr", dtpActArr.Value);
                    cmd.Parameters.AddWithValue("@sDep", dtpSchedDep.Value);
                    cmd.Parameters.AddWithValue("@aDep", dtpActDep.Value);
                    cmd.Parameters.AddWithValue("@halt", (int)numHalt.Value);
                    cmd.Parameters.AddWithValue("@stat", (cmbRunningStatus.SelectedItem?.ToString() ?? cmbRunningStatus.Text?.Trim() ?? ""));
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
