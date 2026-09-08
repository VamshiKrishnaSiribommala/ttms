using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TMS
{
    public class Form_Reg023_SMRelief : Form
    {
        private TextBox txtEntryID;
        private TextBox txtRelievingSMID;
        private TextBox txtRelievedSMID;
        private DateTimePicker dtpHandoverTime;
        private RichTextBox txtPendingIssues;
        private ComboBox cmbWeatherStatus;
        private TextBox txtSubmittedBy;
        private DatabaseHelper db = new DatabaseHelper();
        

        public Form_Reg023_SMRelief()
        {
            this.Text = "SM Relief Diary (REG-023)";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.Size = new System.Drawing.Size(800, 700);
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.White;
            
            CreateControls();
            GenerateEntryID();
        }

        private void CreateControls()
        {
            Panel headerPanel = new Panel();
            headerPanel.BackColor = System.Drawing.Color.FromArgb(0, 51, 102);
            headerPanel.Size = new System.Drawing.Size(800, 80);
            headerPanel.Dock = DockStyle.Top;
            this.Controls.Add(headerPanel);
            
            Label lblPath = new Label();
            lblPath.Text = "Home > Maintenance Sub > SM Relief Diary";
            lblPath.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Italic);
            lblPath.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);
            lblPath.Size = new System.Drawing.Size(750, 25);
            lblPath.Location = new System.Drawing.Point(25, 15);
            headerPanel.Controls.Add(lblPath);
            
            Label lblTitle = new Label();
            lblTitle.Text = "STATION MASTER RELIEF DIARY";
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.White;
            lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblTitle.Size = new System.Drawing.Size(750, 35);
            lblTitle.Location = new System.Drawing.Point(25, 40);
            headerPanel.Controls.Add(lblTitle);
            
            int y = 110;
            
            Label lblEntryID = new Label();
            lblEntryID.Text = "Entry ID (System Generated):";
            lblEntryID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblEntryID.Location = new System.Drawing.Point(30, y);
            lblEntryID.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(lblEntryID);
            
            txtEntryID = new TextBox();
            txtEntryID.Location = new System.Drawing.Point(240, y);
            txtEntryID.Size = new System.Drawing.Size(300, 30);
            txtEntryID.ReadOnly = true;
            txtEntryID.BackColor = System.Drawing.Color.LightGray;
            this.Controls.Add(txtEntryID);
            
            y += 50;
            
            Label lblRelievingSMID = new Label();
            lblRelievingSMID.Text = "Relieving SM ID *";
            lblRelievingSMID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblRelievingSMID.Location = new System.Drawing.Point(30, y);
            lblRelievingSMID.Size = new System.Drawing.Size(140, 30);
            this.Controls.Add(lblRelievingSMID);
            
            txtRelievingSMID = new TextBox();
            txtRelievingSMID.Location = new System.Drawing.Point(180, y);
            txtRelievingSMID.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(txtRelievingSMID);
            
            y += 50;
            
            Label lblRelievedSMID = new Label();
            lblRelievedSMID.Text = "Relieved SM ID *";
            lblRelievedSMID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblRelievedSMID.Location = new System.Drawing.Point(30, y);
            lblRelievedSMID.Size = new System.Drawing.Size(140, 30);
            this.Controls.Add(lblRelievedSMID);
            
            txtRelievedSMID = new TextBox();
            txtRelievedSMID.Location = new System.Drawing.Point(180, y);
            txtRelievedSMID.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(txtRelievedSMID);
            
            y += 50;
            
            Label lblHandoverTime = new Label();
            lblHandoverTime.Text = "Handover Time";
            lblHandoverTime.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblHandoverTime.Location = new System.Drawing.Point(30, y);
            lblHandoverTime.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblHandoverTime);
            
            dtpHandoverTime = new DateTimePicker();
            dtpHandoverTime.Location = new System.Drawing.Point(170, y);
            dtpHandoverTime.Size = new System.Drawing.Size(200, 30);
            dtpHandoverTime.Format = DateTimePickerFormat.Custom;
            dtpHandoverTime.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            dtpHandoverTime.ShowCheckBox = true;
            dtpHandoverTime.Checked = false;
            this.Controls.Add(dtpHandoverTime);
            
            y += 80;
            
            Label lblPendingIssues = new Label();
            lblPendingIssues.Text = "Pending Issues *";
            lblPendingIssues.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblPendingIssues.Location = new System.Drawing.Point(30, y);
            lblPendingIssues.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblPendingIssues);
            
            txtPendingIssues = new RichTextBox();
            txtPendingIssues.Location = new System.Drawing.Point(30, y + 40);
            txtPendingIssues.Size = new System.Drawing.Size(720, 120);
            txtPendingIssues.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(txtPendingIssues);
            
            y += 180;
            
            Label lblWeatherStatus = new Label();
            lblWeatherStatus.Text = "Weather Status *";
            lblWeatherStatus.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblWeatherStatus.Location = new System.Drawing.Point(30, y);
            lblWeatherStatus.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblWeatherStatus);
            
            cmbWeatherStatus = new ComboBox();
            cmbWeatherStatus.Location = new System.Drawing.Point(170, y);
            cmbWeatherStatus.Size = new System.Drawing.Size(180, 30);
            cmbWeatherStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbWeatherStatus.Items.AddRange(new string[] { "Clear", "Foggy", "Rainy", "Stormy", "Cloudy" });
            this.Controls.Add(cmbWeatherStatus);
            
            y += 80;

            Label lblSubmittedBy = new Label();
            lblSubmittedBy.Text = "Staff ID (Submitted By) *";
            lblSubmittedBy.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblSubmittedBy.Location = new System.Drawing.Point(30, y);
            lblSubmittedBy.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblSubmittedBy);
            
            txtSubmittedBy = new TextBox();
            txtSubmittedBy.Location = new System.Drawing.Point(160, y);
            txtSubmittedBy.Size = new System.Drawing.Size(630, 30);
            this.Controls.Add(txtSubmittedBy);
            
            y += 60;

            Button btnSave = new Button();

            btnSave.Text = "SAVE";
            btnSave.Size = new System.Drawing.Size(150, 45);
            btnSave.Location = new System.Drawing.Point(180, y);
            btnSave.BackColor = System.Drawing.Color.FromArgb(46, 204, 113);
            btnSave.ForeColor = System.Drawing.Color.White;
            btnSave.Font = new System.Drawing.Font("Segoe UI", 11, System.Drawing.FontStyle.Bold);
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);
            
            Button btnView = new Button();
            btnView.Text = "VIEW RECORDS";
            btnView.Size = new System.Drawing.Size(150, 45);
            btnView.Location = new System.Drawing.Point(350, y);
            btnView.BackColor = System.Drawing.Color.FromArgb(52, 152, 219);
            btnView.ForeColor = System.Drawing.Color.White;
            btnView.Font = new System.Drawing.Font("Segoe UI", 11, System.Drawing.FontStyle.Bold);
            btnView.FlatStyle = FlatStyle.Flat;
            btnView.Click += (s, e) => new ViewRecordsForm("Reg023_SMRelief", "SM Relief Records").ShowDialog();
            this.Controls.Add(btnView);
            
            Button btnClear = new Button();
            btnClear.Text = "CLEAR";
            btnClear.Size = new System.Drawing.Size(120, 45);
            btnClear.Location = new System.Drawing.Point(520, y);
            btnClear.BackColor = System.Drawing.Color.FromArgb(241, 196, 15);
            btnClear.ForeColor = System.Drawing.Color.Black;
            btnClear.Font = new System.Drawing.Font("Segoe UI", 11, System.Drawing.FontStyle.Bold);
            btnClear.FlatStyle = FlatStyle.Flat;
            btnClear.Click += (s, e) => ClearForm();
            this.Controls.Add(btnClear);
            
            Button btnBack = new Button();
            btnBack.Text = "BACK";
            btnBack.Size = new System.Drawing.Size(100, 45);
            btnBack.Location = new System.Drawing.Point(660, y);
            btnBack.BackColor = System.Drawing.Color.FromArgb(231, 76, 60);
            btnBack.ForeColor = System.Drawing.Color.White;
            btnBack.Font = new System.Drawing.Font("Segoe UI", 11, System.Drawing.FontStyle.Bold);
            btnBack.FlatStyle = FlatStyle.Flat;
            btnBack.DialogResult = DialogResult.Cancel;
            btnBack.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            this.Controls.Add(btnBack);
        }
        
        private void GenerateEntryID()
        {
            string datePart = DateTime.Now.ToString("yyyyMMdd");
            string query = $"SELECT COUNT(*) FROM Reg023_SMRelief WHERE EntryID LIKE 'TMS-REG-023-{datePart}-%'";
            int count = Convert.ToInt32(db.ExecuteScalar(query));
            txtEntryID.Text = $"TMS-REG-023-{datePart}-{(count + 1).ToString("D3")}";
        }
        
        
            private void BtnSave_Click(object sender, EventArgs e)
        {
        
        
            if (string.IsNullOrWhiteSpace(txtSubmittedBy.Text) || !int.TryParse(txtSubmittedBy.Text.Trim(), out _))
            {
                MessageBox.Show("Submitted By must be a valid numeric Staff ID.", "Validation Error");
                return;
            }
            if (!ValidationHelper.IsNotEmpty(txtRelievingSMID.Text, "Relieving SM ID")) return;
            if (!ValidationHelper.IsNotEmpty(txtRelievedSMID.Text, "Relieved SM ID")) return;
            if (!ValidationHelper.IsNotEmpty(txtPendingIssues.Text, "Pending Issues")) return;
            if (!ValidationHelper.IsSelected(cmbWeatherStatus, "Weather Status")) return;
            
            string handoverTime = dtpHandoverTime.Checked ? $"'{dtpHandoverTime.Value:yyyy-MM-dd HH:mm:ss.fff}'" : "NULL";
            
            string query = $@"
                INSERT INTO Reg023_SMRelief (EntryID, RelievingSMID, RelievedSMID, HandoverTime, PendingIssues, WeatherStatus, SubmittedBy)
                VALUES ('{txtEntryID.Text}', '{txtRelievingSMID.Text}', '{txtRelievedSMID.Text}', 
                        {handoverTime}, '{txtPendingIssues.Text.Replace("'", "''")}', '{cmbWeatherStatus.SelectedItem}', {txtSubmittedBy.Text})";

            
            
            try
            {
                db.ExecuteNonQuery(query);
                MessageBox.Show($"SM Relief Record Saved!\nEntry ID: {txtEntryID.Text}", "Success");
                ClearForm();
                GenerateEntryID();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error");
            }
        }
        
        
            private void ClearForm()
        {
        
        
            if (txtSubmittedBy != null) txtSubmittedBy.Clear();
            txtRelievingSMID.Clear();
            txtRelievedSMID.Clear();
            dtpHandoverTime.Checked = false;
            txtPendingIssues.Clear();
            cmbWeatherStatus.SelectedIndex = -1;
        }
    
        protected override void OnHandleCreated(System.EventArgs e) { base.OnHandleCreated(e); TMS.ThemeManager.ApplyTheme(this); }
    }
}
