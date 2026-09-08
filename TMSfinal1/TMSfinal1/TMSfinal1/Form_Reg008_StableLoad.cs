using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TMS
{
    public class Form_Reg008_StableLoad : Form
    {
        private TextBox txtStablingID;
        private TextBox txtTrainLoadID;
        private ComboBox cmbLineNumber;
        private DateTimePicker dtpStabledTime;
        private ComboBox cmbHandBrakeStatus;
        private DateTimePicker dtpClearedTime;
        private TextBox txtSubmittedBy;
        private DatabaseHelper db = new DatabaseHelper();
        

        public Form_Reg008_StableLoad()
        {
            this.Text = "Stable Load (REG-008)";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.Size = new System.Drawing.Size(750, 600);
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.White;
            
            CreateControls();
            GenerateStablingID();
        }

        private void CreateControls()
        {
            Panel headerPanel = new Panel();
            headerPanel.BackColor = System.Drawing.Color.FromArgb(0, 51, 102);
            headerPanel.Size = new System.Drawing.Size(750, 80);
            headerPanel.Dock = DockStyle.Top;
            this.Controls.Add(headerPanel);
            
            Label lblPath = new Label();
            lblPath.Text = "Home > Operational List > Stable Load";
            lblPath.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Italic);
            lblPath.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);
            lblPath.Size = new System.Drawing.Size(700, 25);
            lblPath.Location = new System.Drawing.Point(25, 15);
            headerPanel.Controls.Add(lblPath);
            
            Label lblTitle = new Label();
            lblTitle.Text = "STABLE LOAD REGISTER";
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.White;
            lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblTitle.Size = new System.Drawing.Size(700, 35);
            lblTitle.Location = new System.Drawing.Point(25, 40);
            headerPanel.Controls.Add(lblTitle);
            
            int y = 110;
            
            Label lblStablingID = new Label();
            lblStablingID.Text = "Stabling ID (System Generated):";
            lblStablingID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblStablingID.Location = new System.Drawing.Point(30, y);
            lblStablingID.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(lblStablingID);
            
            txtStablingID = new TextBox();
            txtStablingID.Location = new System.Drawing.Point(240, y);
            txtStablingID.Size = new System.Drawing.Size(300, 30);
            txtStablingID.ReadOnly = true;
            txtStablingID.BackColor = System.Drawing.Color.LightGray;
            this.Controls.Add(txtStablingID);
            
            y += 50;
            
            Label lblTrainLoadID = new Label();
            lblTrainLoadID.Text = "Train/Load ID *";
            lblTrainLoadID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblTrainLoadID.Location = new System.Drawing.Point(30, y);
            lblTrainLoadID.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblTrainLoadID);
            
            txtTrainLoadID = new TextBox();
            txtTrainLoadID.Location = new System.Drawing.Point(160, y);
            txtTrainLoadID.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(txtTrainLoadID);
            
            y += 50;
            
            Label lblLineNumber = new Label();
            lblLineNumber.Text = "Line Number *";
            lblLineNumber.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblLineNumber.Location = new System.Drawing.Point(30, y);
            lblLineNumber.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblLineNumber);
            
            cmbLineNumber = new ComboBox();
            cmbLineNumber.Location = new System.Drawing.Point(160, y);
            cmbLineNumber.Size = new System.Drawing.Size(200, 30);
            cmbLineNumber.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbLineNumber.Items.AddRange(new string[] { "Platform 1", "Platform 2", "Platform 3", "Loop Line 1", "Loop Line 2", "Stabling Line" });
            this.Controls.Add(cmbLineNumber);
            
            y += 50;
            
            Label lblStabledTime = new Label();
            lblStabledTime.Text = "Stabled Time *";
            lblStabledTime.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblStabledTime.Location = new System.Drawing.Point(30, y);
            lblStabledTime.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblStabledTime);
            
            dtpStabledTime = new DateTimePicker();
            dtpStabledTime.Location = new System.Drawing.Point(160, y);
            dtpStabledTime.Size = new System.Drawing.Size(200, 30);
            dtpStabledTime.Format = DateTimePickerFormat.Custom;
            dtpStabledTime.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            this.Controls.Add(dtpStabledTime);
            
            y += 50;
            
            Label lblHandBrakeStatus = new Label();
            lblHandBrakeStatus.Text = "Hand Brake Status *";
            lblHandBrakeStatus.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblHandBrakeStatus.Location = new System.Drawing.Point(30, y);
            lblHandBrakeStatus.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblHandBrakeStatus);
            
            cmbHandBrakeStatus = new ComboBox();
            cmbHandBrakeStatus.Location = new System.Drawing.Point(170, y);
            cmbHandBrakeStatus.Size = new System.Drawing.Size(150, 30);
            cmbHandBrakeStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbHandBrakeStatus.Items.AddRange(new string[] { "Applied", "Not Applied" });
            this.Controls.Add(cmbHandBrakeStatus);
            
            y += 50;
            
            Label lblClearedTime = new Label();
            lblClearedTime.Text = "Cleared Time";
            lblClearedTime.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblClearedTime.Location = new System.Drawing.Point(30, y);
            lblClearedTime.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblClearedTime);
            
            dtpClearedTime = new DateTimePicker();
            dtpClearedTime.Location = new System.Drawing.Point(160, y);
            dtpClearedTime.Size = new System.Drawing.Size(200, 30);
            dtpClearedTime.Format = DateTimePickerFormat.Custom;
            dtpClearedTime.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            dtpClearedTime.ShowCheckBox = true;
            dtpClearedTime.Checked = false;
            this.Controls.Add(dtpClearedTime);
            
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
            btnView.Click += (s, e) => new ViewRecordsForm("Reg008_StableLoad", "Stable Load Records").ShowDialog();
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
        
        private void GenerateStablingID()
        {
            string datePart = DateTime.Now.ToString("yyyyMMdd");
            string query = $"SELECT COUNT(*) FROM Reg008_StableLoad WHERE StablingID LIKE 'TMS-REG-008-{datePart}-%'";
            int count = Convert.ToInt32(db.ExecuteScalar(query));
            txtStablingID.Text = $"TMS-REG-008-{datePart}-{(count + 1).ToString("D3")}";
        }
        
        
            private void BtnSave_Click(object sender, EventArgs e)
        {
        
        
            if (string.IsNullOrWhiteSpace(txtSubmittedBy.Text) || !int.TryParse(txtSubmittedBy.Text.Trim(), out _))
            {
                MessageBox.Show("Submitted By must be a valid numeric Staff ID.", "Validation Error");
                return;
            }
            if (!ValidationHelper.IsNotEmpty(txtTrainLoadID.Text, "Train/Load ID")) return;
            if (!ValidationHelper.IsSelected(cmbLineNumber, "Line Number")) return;
            if (!ValidationHelper.IsSelected(cmbHandBrakeStatus, "Hand Brake Status")) return;
            
            string clearedTime = dtpClearedTime.Checked ? $"'{dtpClearedTime.Value:yyyy-MM-dd HH:mm:ss.fff}'" : "NULL";
            
            string query = $@"
                INSERT INTO Reg008_StableLoad (StablingID, TrainLoadID, LineNumber, StabledTime, HandBrakeStatus, ClearedTime, SubmittedBy)
                VALUES ('{txtStablingID.Text}', '{txtTrainLoadID.Text}', '{cmbLineNumber.SelectedItem}', 
                        '{dtpStabledTime.Value:yyyy-MM-dd HH:mm:ss.fff}', '{cmbHandBrakeStatus.SelectedItem}', {clearedTime}, {txtSubmittedBy.Text})";

            
            
            try
            {
                db.ExecuteNonQuery(query);
                MessageBox.Show($"Stable Load Record Saved!\nStabling ID: {txtStablingID.Text}", "Success");
                ClearForm();
                GenerateStablingID();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error");
            }
        }
        
        
            private void ClearForm()
        {
        
        
            if (txtSubmittedBy != null) txtSubmittedBy.Clear();
            txtTrainLoadID.Clear();
            cmbLineNumber.SelectedIndex = -1;
            dtpStabledTime.Value = DateTime.Now;
            cmbHandBrakeStatus.SelectedIndex = -1;
            dtpClearedTime.Checked = false;
            dtpClearedTime.Value = DateTime.Now;
        }
    
        protected override void OnHandleCreated(System.EventArgs e) { base.OnHandleCreated(e); TMS.ThemeManager.ApplyTheme(this); }
    }
}
