using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TMS
{
    public class Form_Reg002_TrainSignal : Form
    {
        private TextBox txtEntryID;
        private TextBox txtTrainNumber;
        private ComboBox cmbDirection;
        private ComboBox cmbLineNumber;
        private DateTimePicker dtpArrival;
        private DateTimePicker dtpDeparture;
        private DateTimePicker dtpPassed;
        private TextBox txtSMonDuty;
        private TextBox txtSubmittedBy;
        private DatabaseHelper db = new DatabaseHelper();
        

        public Form_Reg002_TrainSignal()
        {
            this.Text = "Train Signal Register (REG-002)";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.Size = new System.Drawing.Size(850, 700);
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.White;
            
            CreateControls();
            GenerateEntryID();
        }

        private void CreateControls()
        {
            Panel headerPanel = new Panel();
            headerPanel.BackColor = System.Drawing.Color.FromArgb(0, 51, 102);
            headerPanel.Size = new System.Drawing.Size(850, 80);
            headerPanel.Dock = DockStyle.Top;
            this.Controls.Add(headerPanel);
            
            Label lblPath = new Label();
            lblPath.Text = "Home > Operational List > Train Signal Register";
            lblPath.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Italic);
            lblPath.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);
            lblPath.Size = new System.Drawing.Size(800, 25);
            lblPath.Location = new System.Drawing.Point(25, 15);
            headerPanel.Controls.Add(lblPath);
            
            Label lblTitle = new Label();
            lblTitle.Text = "TRAIN SIGNAL REGISTER (TSR";
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.White;
            lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblTitle.Size = new System.Drawing.Size(800, 35);
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
            
            Label lblTrainNumber = new Label();
            lblTrainNumber.Text = "Train Number *";
            lblTrainNumber.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblTrainNumber.Location = new System.Drawing.Point(30, y);
            lblTrainNumber.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblTrainNumber);
            
            txtTrainNumber = new TextBox();
            txtTrainNumber.Location = new System.Drawing.Point(160, y);
            txtTrainNumber.Size = new System.Drawing.Size(150, 30);
            this.Controls.Add(txtTrainNumber);
            
            y += 50;
            
            Label lblDirection = new Label();
            lblDirection.Text = "Direction *";
            lblDirection.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblDirection.Location = new System.Drawing.Point(30, y);
            lblDirection.Size = new System.Drawing.Size(100, 30);
            this.Controls.Add(lblDirection);
            
            cmbDirection = new ComboBox();
            cmbDirection.Location = new System.Drawing.Point(140, y);
            cmbDirection.Size = new System.Drawing.Size(100, 30);
            cmbDirection.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDirection.Items.AddRange(new string[] { "Up", "Down" });
            this.Controls.Add(cmbDirection);
            
            Label lblLineNumber = new Label();
            lblLineNumber.Text = "Line Number *";
            lblLineNumber.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblLineNumber.Location = new System.Drawing.Point(280, y);
            lblLineNumber.Size = new System.Drawing.Size(100, 30);
            this.Controls.Add(lblLineNumber);
            
            cmbLineNumber = new ComboBox();
            cmbLineNumber.Location = new System.Drawing.Point(390, y);
            cmbLineNumber.Size = new System.Drawing.Size(120, 30);
            cmbLineNumber.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbLineNumber.Items.AddRange(new string[] { "Platform 1", "Platform 2", "Platform 3", "Through Line", "Loop Line", "Stabling Line" });
            this.Controls.Add(cmbLineNumber);
            
            y += 60;
            
            Label lblArrival = new Label();
            lblArrival.Text = "Train Arrival *";
            lblArrival.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblArrival.Location = new System.Drawing.Point(30, y);
            lblArrival.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblArrival);
            
            dtpArrival = new DateTimePicker();
            dtpArrival.Location = new System.Drawing.Point(160, y);
            dtpArrival.Size = new System.Drawing.Size(180, 30);
            dtpArrival.Format = DateTimePickerFormat.Custom;
            dtpArrival.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            this.Controls.Add(dtpArrival);
            
            Label lblDeparture = new Label();
            lblDeparture.Text = "Train Departure *";
            lblDeparture.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblDeparture.Location = new System.Drawing.Point(370, y);
            lblDeparture.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblDeparture);
            
            dtpDeparture = new DateTimePicker();
            dtpDeparture.Location = new System.Drawing.Point(500, y);
            dtpDeparture.Size = new System.Drawing.Size(180, 30);
            dtpDeparture.Format = DateTimePickerFormat.Custom;
            dtpDeparture.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            this.Controls.Add(dtpDeparture);
            
            y += 50;
            
            Label lblPassed = new Label();
            lblPassed.Text = "Train Passed *";
            lblPassed.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblPassed.Location = new System.Drawing.Point(30, y);
            lblPassed.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblPassed);
            
            dtpPassed = new DateTimePicker();
            dtpPassed.Location = new System.Drawing.Point(160, y);
            dtpPassed.Size = new System.Drawing.Size(180, 30);
            dtpPassed.Format = DateTimePickerFormat.Custom;
            dtpPassed.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            this.Controls.Add(dtpPassed);
            
            y += 60;
            
            Label lblSMonDuty = new Label();
            lblSMonDuty.Text = "SM on Duty *";
            lblSMonDuty.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblSMonDuty.Location = new System.Drawing.Point(30, y);
            lblSMonDuty.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblSMonDuty);
            
            txtSMonDuty = new TextBox();
            txtSMonDuty.Location = new System.Drawing.Point(160, y);
            txtSMonDuty.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(txtSMonDuty);
            
            y += 70;

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
            btnView.Click += (s, e) => new ViewRecordsForm("Reg002_TrainSignal", "Train Signal Records").ShowDialog();
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
            string query = $"SELECT COUNT(*) FROM Reg002_TrainSignal WHERE EntryID LIKE 'TMS-REG-002-{datePart}-%'";
            int count = Convert.ToInt32(db.ExecuteScalar(query));
            txtEntryID.Text = $"TMS-REG-002-{datePart}-{(count + 1).ToString("D3")}";
        }
        
        
            private void BtnSave_Click(object sender, EventArgs e)
        {
        
        
            if (string.IsNullOrWhiteSpace(txtSubmittedBy.Text) || !int.TryParse(txtSubmittedBy.Text.Trim(), out _))
            {
                MessageBox.Show("Submitted By must be a valid numeric Staff ID.", "Validation Error");
                return;
            }
            if (!ValidationHelper.IsNotEmpty(txtTrainNumber.Text, "Train Number")) return;
            if (!ValidationHelper.IsValidTrainNumber(txtTrainNumber.Text)) return;
            if (!ValidationHelper.IsSelected(cmbDirection, "Direction")) return;
            if (!ValidationHelper.IsSelected(cmbLineNumber, "Line Number")) return;
            if (!ValidationHelper.IsEndAfterStart(dtpArrival.Value, dtpDeparture.Value, "Arrival", "Departure")) return;
            if (!ValidationHelper.IsNotEmpty(txtSMonDuty.Text, "SM on Duty")) return;
            
            string query = $@"
                INSERT INTO Reg002_TrainSignal (EntryID, TrainNumber, Direction, LineNumber, TrainArrival, TrainDeparture, TrainPassed, SMonDuty, SubmittedBy)
                VALUES ('{txtEntryID.Text}', '{txtTrainNumber.Text}', '{cmbDirection.SelectedItem}', '{cmbLineNumber.SelectedItem}', 
                        '{dtpArrival.Value:yyyy-MM-dd HH:mm:ss.fff}', '{dtpDeparture.Value:yyyy-MM-dd HH:mm:ss.fff}', 
                        '{dtpPassed.Value:yyyy-MM-dd HH:mm:ss.fff}', '{txtSMonDuty.Text}', {txtSubmittedBy.Text})";

            
            
            try
            {
                db.ExecuteNonQuery(query);
                MessageBox.Show($"Record Saved!\nEntry ID: {txtEntryID.Text}", "Success");
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
            txtTrainNumber.Clear();
            cmbDirection.SelectedIndex = -1;
            cmbLineNumber.SelectedIndex = -1;
            dtpArrival.Value = DateTime.Now;
            dtpDeparture.Value = DateTime.Now.AddMinutes(5);
            dtpPassed.Value = DateTime.Now.AddMinutes(6);
            txtSMonDuty.Clear();
        }
    
        protected override void OnHandleCreated(System.EventArgs e) { base.OnHandleCreated(e); TMS.ThemeManager.ApplyTheme(this); }
    }
}
