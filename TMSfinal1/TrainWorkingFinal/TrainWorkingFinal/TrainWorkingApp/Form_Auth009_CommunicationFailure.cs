using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace TrainWorkingApp
{
    public class Form_Auth009_CommunicationFailure : Form
    {
        private readonly DatabaseHelper db = new DatabaseHelper();
        private TextBox txtRefNo;
        private TextBox txtSection;
        private ComboBox cmbFailureType;
        private DateTimePicker dtpStart;
        private TextBox txtAuthorizedBy;
        private TextBox txtControlApproval;
        private ComboBox cmbCommMethod;
        private TextBox txtMsgRef;

        public Form_Auth009_CommunicationFailure()
        {
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.Font = new Font("Segoe UI", 9.75f, FontStyle.Regular);
            InitializeComponent();
            ThemeManager.ApplyTheme(this);
            GenerateRefNo();
        }

        private void InitializeComponent()
        {
            this.Text = "TOTAL INTERRUPTION OF COMMUNICATION (T/C 602) - AUTH-009";
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
                Text = "TOTAL INTERRUPTION OF COMMUNICATION (T/C 602)",
                Font = new Font("Segoe UI", 14, FontStyle.Bold), 
                ForeColor = Color.White, 
                Location = new Point(25, 36), 
                AutoSize = true 
            };
            header.Controls.Add(lblT);

            int y = 95;
            AddLabel("Failure Reference No (System):", 30, y);
            txtRefNo = new TextBox { Location = new Point(290, y), Size = new Size(250, 26), ReadOnly = true, BackColor = Color.LightGray };
            this.Controls.Add(txtRefNo);
            y += 45;
            AddLabel("Section Affected *:", 30, y);
            txtSection = new TextBox { Location = new Point(290, y), Size = new Size(350, 26) };
            this.Controls.Add(txtSection);
            y += 45;
            AddLabel("Type of Failure *:", 30, y);
            cmbFailureType = new ComboBox { Location = new Point(290, y), Size = new Size(350, 26), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbFailureType.Items.AddRange(new string[] { "Total Interruption of Communication", "Block Instrument Failure", "Control Phone Failure", "VHF Set Failure" });
            cmbFailureType.SelectedIndex = -1;
            this.Controls.Add(cmbFailureType);
            y += 45;
            AddLabel("Failure Start Time *:", 30, y);
            dtpStart = new DateTimePicker { Location = new Point(290, y), Size = new Size(250, 26), Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy HH:mm:ss" };
            this.Controls.Add(dtpStart);
            y += 45;
            AddLabel("Authorized By *:", 30, y);
            txtAuthorizedBy = new TextBox { Location = new Point(290, y), Size = new Size(250, 26) };
            this.Controls.Add(txtAuthorizedBy);
            y += 45;
            AddLabel("Control Approval *:", 30, y);
            txtControlApproval = new TextBox { Location = new Point(290, y), Size = new Size(250, 26) };
            this.Controls.Add(txtControlApproval);
            y += 45;
            AddLabel("Communication Method *:", 30, y);
            cmbCommMethod = new ComboBox { Location = new Point(290, y), Size = new Size(350, 26), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbCommMethod.Items.AddRange(new string[] { "VHF Handset", "Railway Magneto Telephone", "BSNL / Fixed Line", "Station Bell Code" });
            cmbCommMethod.SelectedIndex = -1;
            this.Controls.Add(cmbCommMethod);
            y += 45;
            AddLabel("Message Reference *:", 30, y);
            txtMsgRef = new TextBox { Location = new Point(290, y), Size = new Size(250, 26) };
            this.Controls.Add(txtMsgRef);
            y += 45;

            Button btnSave = new Button { Text = "SAVE AUTHORITY", Location = new Point(290, y), Size = new Size(160, 38) };
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            Button btnClear = new Button { Text = "CLEAR", Location = new Point(465, y), Size = new Size(100, 38) };
            btnClear.Click += (s, e) => ClearFields();
            this.Controls.Add(btnClear);

            Button btnView = new Button { Text = "VIEW RECORDS", Location = new Point(580, y), Size = new Size(140, 38) };
            btnView.Click += (s, e) => new ViewRecordsForm("Communication_Failure_Log", "Communication Failure Logs").ShowDialog(this);
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
            txtRefNo.Text = "FAIL-COMM-" + DateTime.Now.ToString("yyyyMMdd") + "-" + new Random().Next(100, 999);
        }

        private void ClearFields()
        {
            txtSection.Text = "";
            cmbFailureType.SelectedIndex = -1;
            dtpStart.Value = DateTime.Now;
            txtAuthorizedBy.Text = "";
            txtControlApproval.Text = "";
            cmbCommMethod.SelectedIndex = -1;
            txtMsgRef.Text = "";
            GenerateRefNo();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            var vals = new Dictionary<string, object>
            {
                { "Section_Affected", txtSection.Text.Trim() },
                { "Type_of_Failure", cmbFailureType.SelectedItem?.ToString() ?? "" },
                { "Authorized_By", txtAuthorizedBy.Text.Trim() },
                { "Control_Approval", txtControlApproval.Text.Trim() },
                { "Communication_Method", cmbCommMethod.SelectedItem?.ToString() ?? "" },
                { "Message_Reference", txtMsgRef.Text.Trim() }
            };

            if (!ValidationHelper.ValidateForm("009", vals)) return;

            try
            {
                using (SqlCommand cmd = new SqlCommand(@"INSERT INTO Communication_Failure_Log 
                    (Failure_Reference_No, Section_Affected, Type_of_Failure, Failure_Start_Time, Authorized_By, Control_Approval, Communication_Method, Message_Reference, Restoration_Status)
                    VALUES (@ref, @sec, @type, @start, @auth, @ctrl, @meth, @msg, 0)"))
                {
                    cmd.Parameters.AddWithValue("@ref", txtRefNo.Text);
                    cmd.Parameters.AddWithValue("@sec", txtSection.Text.Trim());
                    cmd.Parameters.AddWithValue("@type", (cmbFailureType.SelectedItem?.ToString() ?? cmbFailureType.Text?.Trim() ?? ""));
                    cmd.Parameters.AddWithValue("@start", dtpStart.Value);
                    cmd.Parameters.AddWithValue("@auth", txtAuthorizedBy.Text.Trim());
                    cmd.Parameters.AddWithValue("@ctrl", txtControlApproval.Text.Trim());
                    cmd.Parameters.AddWithValue("@meth", (cmbCommMethod.SelectedItem?.ToString() ?? cmbCommMethod.Text?.Trim() ?? ""));
                    cmd.Parameters.AddWithValue("@msg", txtMsgRef.Text.Trim());

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
