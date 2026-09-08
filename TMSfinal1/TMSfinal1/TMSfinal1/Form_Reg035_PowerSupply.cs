using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TMS
{
    public class Form_Reg035_PowerSupply : Form
    {
        private TextBox txtEntryID;
        private ComboBox cmbPrimarySource;
        private ComboBox cmbSecondarySource;
        private DateTimePicker dtpFailureTime;
        private DateTimePicker dtpRestorationTime;
        private DateTimePicker dtpDGStartTime;
        private DateTimePicker dtpDGStopTime;
        private NumericUpDown numDGRunTime;
        private NumericUpDown numFuelLevel;
        private NumericUpDown numConsumption;
        private TextBox txtRecordedBy;
        private TextBox txtSubmittedBy;
        private DatabaseHelper db = new DatabaseHelper();
        

        public Form_Reg035_PowerSupply()
        {
            this.Text = "Power Supply (REG-035)";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.Size = new System.Drawing.Size(750, 750);
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.White;
            
            CreateControls();
            GenerateEntryID();
        }

        private void CreateControls()
        {
            Panel headerPanel = new Panel();
            headerPanel.BackColor = System.Drawing.Color.FromArgb(0, 51, 102);
            headerPanel.Size = new System.Drawing.Size(750, 80);
            headerPanel.Dock = DockStyle.Top;
            this.Controls.Add(headerPanel);
            
            Label lblPath = new Label();
            lblPath.Text = "Home > Maintenance Sub > Power Supply";
            lblPath.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Italic);
            lblPath.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);
            lblPath.Size = new System.Drawing.Size(700, 25);
            lblPath.Location = new System.Drawing.Point(25, 15);
            headerPanel.Controls.Add(lblPath);
            
            Label lblTitle = new Label();
            lblTitle.Text = "STATION POWER SUPPLY REGISTER";
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.White;
            lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblTitle.Size = new System.Drawing.Size(700, 35);
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
            
            Label lblPrimarySource = new Label();
            lblPrimarySource.Text = "Primary Source *";
            lblPrimarySource.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblPrimarySource.Location = new System.Drawing.Point(30, y);
            lblPrimarySource.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblPrimarySource);
            
            cmbPrimarySource = new ComboBox();
            cmbPrimarySource.Location = new System.Drawing.Point(170, y);
            cmbPrimarySource.Size = new System.Drawing.Size(150, 30);
            cmbPrimarySource.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPrimarySource.Items.AddRange(new string[] { "EB", "AT", "Grid" });
            this.Controls.Add(cmbPrimarySource);
            
            y += 50;
            
            Label lblSecondarySource = new Label();
            lblSecondarySource.Text = "Secondary Source *";
            lblSecondarySource.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblSecondarySource.Location = new System.Drawing.Point(30, y);
            lblSecondarySource.Size = new System.Drawing.Size(140, 30);
            this.Controls.Add(lblSecondarySource);
            
            cmbSecondarySource = new ComboBox();
            cmbSecondarySource.Location = new System.Drawing.Point(180, y);
            cmbSecondarySource.Size = new System.Drawing.Size(150, 30);
            cmbSecondarySource.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSecondarySource.Items.AddRange(new string[] { "DG", "Solar", "UPS", "Not Used" });
            this.Controls.Add(cmbSecondarySource);
            
            y += 50;
            
            Label lblFailureTime = new Label();
            lblFailureTime.Text = "Failure Time *";
            lblFailureTime.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblFailureTime.Location = new System.Drawing.Point(30, y);
            lblFailureTime.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblFailureTime);
            
            dtpFailureTime = new DateTimePicker();
            dtpFailureTime.Location = new System.Drawing.Point(160, y);
            dtpFailureTime.Size = new System.Drawing.Size(200, 30);
            dtpFailureTime.Format = DateTimePickerFormat.Custom;
            dtpFailureTime.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            this.Controls.Add(dtpFailureTime);
            
            y += 50;
            
            Label lblRestorationTime = new Label();
            lblRestorationTime.Text = "Restoration Time";
            lblRestorationTime.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblRestorationTime.Location = new System.Drawing.Point(30, y);
            lblRestorationTime.Size = new System.Drawing.Size(140, 30);
            this.Controls.Add(lblRestorationTime);
            
            dtpRestorationTime = new DateTimePicker();
            dtpRestorationTime.Location = new System.Drawing.Point(180, y);
            dtpRestorationTime.Size = new System.Drawing.Size(200, 30);
            dtpRestorationTime.Format = DateTimePickerFormat.Custom;
            dtpRestorationTime.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            dtpRestorationTime.ShowCheckBox = true;
            dtpRestorationTime.Checked = false;
            this.Controls.Add(dtpRestorationTime);
            
            y += 50;
            
            Label lblDGStartTime = new Label();
            lblDGStartTime.Text = "DG Start Time";
            lblDGStartTime.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblDGStartTime.Location = new System.Drawing.Point(30, y);
            lblDGStartTime.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblDGStartTime);
            
            dtpDGStartTime = new DateTimePicker();
            dtpDGStartTime.Location = new System.Drawing.Point(170, y);
            dtpDGStartTime.Size = new System.Drawing.Size(200, 30);
            dtpDGStartTime.Format = DateTimePickerFormat.Custom;
            dtpDGStartTime.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            dtpDGStartTime.ShowCheckBox = true;
            dtpDGStartTime.Checked = false;
            this.Controls.Add(dtpDGStartTime);
            
            y += 50;
            
            Label lblDGStopTime = new Label();
            lblDGStopTime.Text = "DG Stop Time";
            lblDGStopTime.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblDGStopTime.Location = new System.Drawing.Point(30, y);
            lblDGStopTime.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblDGStopTime);
            
            dtpDGStopTime = new DateTimePicker();
            dtpDGStopTime.Location = new System.Drawing.Point(170, y);
            dtpDGStopTime.Size = new System.Drawing.Size(200, 30);
            dtpDGStopTime.Format = DateTimePickerFormat.Custom;
            dtpDGStopTime.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            dtpDGStopTime.ShowCheckBox = true;
            dtpDGStopTime.Checked = false;
            this.Controls.Add(dtpDGStopTime);
            
            y += 50;
            
            Label lblDGRunTime = new Label();
            lblDGRunTime.Text = "Run Time (minutes) *";
            lblDGRunTime.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblDGRunTime.Location = new System.Drawing.Point(30, y);
            lblDGRunTime.Size = new System.Drawing.Size(160, 30);
            this.Controls.Add(lblDGRunTime);
            
            numDGRunTime = new NumericUpDown();
            numDGRunTime.Location = new System.Drawing.Point(200, y);
            numDGRunTime.Size = new System.Drawing.Size(100, 30);
            numDGRunTime.Minimum = 0;
            numDGRunTime.Maximum = 9999;
            this.Controls.Add(numDGRunTime);
            
            y += 50;
            
            Label lblFuelLevel = new Label();
            lblFuelLevel.Text = "DG Fuel (Liters";
            lblFuelLevel.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblFuelLevel.Location = new System.Drawing.Point(30, y);
            lblFuelLevel.Size = new System.Drawing.Size(150, 30);
            this.Controls.Add(lblFuelLevel);
            
            numFuelLevel = new NumericUpDown();
            numFuelLevel.Location = new System.Drawing.Point(190, y);
            numFuelLevel.Size = new System.Drawing.Size(100, 30);
            numFuelLevel.Minimum = 0;
            numFuelLevel.Maximum = 10000;
            numFuelLevel.DecimalPlaces = 2;
            this.Controls.Add(numFuelLevel);
            
            y += 50;
            
            Label lblConsumption = new Label();
            lblConsumption.Text = "Consumption (Liters) *";
            lblConsumption.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblConsumption.Location = new System.Drawing.Point(30, y);
            lblConsumption.Size = new System.Drawing.Size(170, 30);
            this.Controls.Add(lblConsumption);
            
            numConsumption = new NumericUpDown();
            numConsumption.Location = new System.Drawing.Point(210, y);
            numConsumption.Size = new System.Drawing.Size(100, 30);
            numConsumption.Minimum = 0;
            numConsumption.Maximum = 10000;
            numConsumption.DecimalPlaces = 2;
            this.Controls.Add(numConsumption);
            
            y += 50;
            
            Label lblRecordedBy = new Label();
            lblRecordedBy.Text = "Recorded By *";
            lblRecordedBy.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblRecordedBy.Location = new System.Drawing.Point(30, y);
            lblRecordedBy.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblRecordedBy);
            
            txtRecordedBy = new TextBox();
            txtRecordedBy.Location = new System.Drawing.Point(160, y);
            txtRecordedBy.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(txtRecordedBy);
            
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
            btnView.Click += (s, e) => new ViewRecordsForm("Reg035_PowerSupply", "Power Supply Records").ShowDialog();
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
            string datePart = DateTime.Now.ToString("yyyyMM");
            string query = $"SELECT COUNT(*) FROM Reg035_PowerSupply WHERE EntryID LIKE 'TMS-REG-035-{datePart}-%'";
            int count = Convert.ToInt32(db.ExecuteScalar(query));
            txtEntryID.Text = $"TMS-REG-035-{datePart}-{(count + 1).ToString("D3")}";
        }
        
        
            private void BtnSave_Click(object sender, EventArgs e)
        {
        
        
            if (string.IsNullOrWhiteSpace(txtSubmittedBy.Text) || !int.TryParse(txtSubmittedBy.Text.Trim(), out _))
            {
                MessageBox.Show("Submitted By must be a valid numeric Staff ID.", "Validation Error");
                return;
            }
            if (!ValidationHelper.IsSelected(cmbPrimarySource, "Primary Source")) return;
            if (!ValidationHelper.IsSelected(cmbSecondarySource, "Secondary Source")) return;
            if (!ValidationHelper.IsNotEmpty(txtRecordedBy.Text, "Recorded By")) return;
            
            string restorationTime = dtpRestorationTime.Checked ? $"'{dtpRestorationTime.Value:yyyy-MM-dd HH:mm:ss.fff}'" : "NULL";
            string dgStartTime = dtpDGStartTime.Checked ? $"'{dtpDGStartTime.Value:yyyy-MM-dd HH:mm:ss.fff}'" : "NULL";
            string dgStopTime = dtpDGStopTime.Checked ? $"'{dtpDGStopTime.Value:yyyy-MM-dd HH:mm:ss.fff}'" : "NULL";
            string fuelLevel = numFuelLevel.Value > 0 ? numFuelLevel.Value.ToString() : "NULL";
            
            string query = $@"
                INSERT INTO Reg035_PowerSupply (EntryID, PrimarySource, SecondarySource, FailureTime, RestorationTime, DGStartTime, DGStopTime, DGRunTime, FuelLevel, Consumption, RecordedBy, SubmittedBy)
                VALUES ('{txtEntryID.Text}', '{cmbPrimarySource.SelectedItem}', '{cmbSecondarySource.SelectedItem}', 
                        '{dtpFailureTime.Value:yyyy-MM-dd HH:mm:ss.fff}', {restorationTime}, {dgStartTime}, {dgStopTime}, 
                        {numDGRunTime.Value}, {fuelLevel}, {numConsumption.Value}, '{txtRecordedBy.Text}', {txtSubmittedBy.Text})";

            
            
            try
            {
                db.ExecuteNonQuery(query);
                MessageBox.Show($"Power Supply Record Saved!\nEntry ID: {txtEntryID.Text}", "Success");
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
            cmbPrimarySource.SelectedIndex = -1;
            cmbSecondarySource.SelectedIndex = -1;
            dtpFailureTime.Value = DateTime.Now;
            dtpRestorationTime.Checked = false;
            dtpDGStartTime.Checked = false;
            dtpDGStopTime.Checked = false;
            numDGRunTime.Value = 0;
            numFuelLevel.Value = 0;
            numConsumption.Value = 0;
            txtRecordedBy.Clear();
        }
    
        protected override void OnHandleCreated(System.EventArgs e) { base.OnHandleCreated(e); TMS.ThemeManager.ApplyTheme(this); }
    }
}
