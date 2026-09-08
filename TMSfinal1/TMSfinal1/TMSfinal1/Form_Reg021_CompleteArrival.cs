using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TMS
{
    public class Form_Reg021_CompleteArrival : Form
    {
        private TextBox txtVerificationID;
        private TextBox txtTrainNumber;
        private DateTimePicker dtpArrivalTime;
        private NumericUpDown numWagonCount;
        private TextBox txtGuardID;
        private TextBox txtSMVerification;
        private TextBox txtSubmittedBy;
        private DatabaseHelper db = new DatabaseHelper();
        

        public Form_Reg021_CompleteArrival()
        {
            this.Text = "Complete Arrival (REG-021)";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.Size = new System.Drawing.Size(750, 650);
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.White;
            
            CreateControls();
            GenerateVerificationID();
        }

        private void CreateControls()
        {
            Panel headerPanel = new Panel();
            headerPanel.BackColor = System.Drawing.Color.FromArgb(0, 51, 102);
            headerPanel.Size = new System.Drawing.Size(750, 80);
            headerPanel.Dock = DockStyle.Top;
            this.Controls.Add(headerPanel);
            
            Label lblPath = new Label();
            lblPath.Text = "Home > Maintenance Sub > Complete Arrival";
            lblPath.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Italic);
            lblPath.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);
            lblPath.Size = new System.Drawing.Size(700, 25);
            lblPath.Location = new System.Drawing.Point(25, 15);
            headerPanel.Controls.Add(lblPath);
            
            Label lblTitle = new Label();
            lblTitle.Text = "COMPLETE ARRIVAL REGISTER";
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.White;
            lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblTitle.Size = new System.Drawing.Size(700, 35);
            lblTitle.Location = new System.Drawing.Point(25, 40);
            headerPanel.Controls.Add(lblTitle);
            
            int y = 110;
            
            Label lblVerificationID = new Label();
            lblVerificationID.Text = "Verification ID (System Generated):";
            lblVerificationID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblVerificationID.Location = new System.Drawing.Point(30, y);
            lblVerificationID.Size = new System.Drawing.Size(220, 30);
            this.Controls.Add(lblVerificationID);
            
            txtVerificationID = new TextBox();
            txtVerificationID.Location = new System.Drawing.Point(260, y);
            txtVerificationID.Size = new System.Drawing.Size(300, 30);
            txtVerificationID.ReadOnly = true;
            txtVerificationID.BackColor = System.Drawing.Color.LightGray;
            this.Controls.Add(txtVerificationID);
            
            y += 50;
            
            Label lblTrainNumber = new Label();
            lblTrainNumber.Text = "Train Number *";
            lblTrainNumber.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblTrainNumber.Location = new System.Drawing.Point(30, y);
            lblTrainNumber.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblTrainNumber);
            
            txtTrainNumber = new TextBox();
            txtTrainNumber.Location = new System.Drawing.Point(160, y);
            txtTrainNumber.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(txtTrainNumber);
            
            y += 50;
            
            Label lblArrivalTime = new Label();
            lblArrivalTime.Text = "Arrival Time *";
            lblArrivalTime.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblArrivalTime.Location = new System.Drawing.Point(30, y);
            lblArrivalTime.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblArrivalTime);
            
            dtpArrivalTime = new DateTimePicker();
            dtpArrivalTime.Location = new System.Drawing.Point(160, y);
            dtpArrivalTime.Size = new System.Drawing.Size(200, 30);
            dtpArrivalTime.Format = DateTimePickerFormat.Custom;
            dtpArrivalTime.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            this.Controls.Add(dtpArrivalTime);
            
            y += 50;
            
            Label lblWagonCount = new Label();
            lblWagonCount.Text = "Wagon Count *";
            lblWagonCount.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblWagonCount.Location = new System.Drawing.Point(30, y);
            lblWagonCount.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblWagonCount);
            
            numWagonCount = new NumericUpDown();
            numWagonCount.Location = new System.Drawing.Point(160, y);
            numWagonCount.Size = new System.Drawing.Size(100, 30);
            numWagonCount.Minimum = 0;
            numWagonCount.Maximum = 100;
            this.Controls.Add(numWagonCount);
            
            y += 50;
            
            Label lblGuardID = new Label();
            lblGuardID.Text = "Guard ID *";
            lblGuardID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblGuardID.Location = new System.Drawing.Point(30, y);
            lblGuardID.Size = new System.Drawing.Size(100, 30);
            this.Controls.Add(lblGuardID);
            
            txtGuardID = new TextBox();
            txtGuardID.Location = new System.Drawing.Point(140, y);
            txtGuardID.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(txtGuardID);
            
            y += 50;
            
            Label lblSMVerification = new Label();
            lblSMVerification.Text = "SM Verification *";
            lblSMVerification.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblSMVerification.Location = new System.Drawing.Point(30, y);
            lblSMVerification.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblSMVerification);
            
            txtSMVerification = new TextBox();
            txtSMVerification.Location = new System.Drawing.Point(170, y);
            txtSMVerification.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(txtSMVerification);
            
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
            btnView.Click += (s, e) => new ViewRecordsForm("Reg021_CompleteArrival", "Complete Arrival Records").ShowDialog();
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
        
        private void GenerateVerificationID()
        {
            string datePart = DateTime.Now.ToString("yyyyMMdd");
            string query = $"SELECT COUNT(*) FROM Reg021_CompleteArrival WHERE VerificationID LIKE 'TMS-REG-021-{datePart}-%'";
            int count = Convert.ToInt32(db.ExecuteScalar(query));
            txtVerificationID.Text = $"TMS-REG-021-{datePart}-{(count + 1).ToString("D3")}";
        }
        
        
            private void BtnSave_Click(object sender, EventArgs e)
        {
        
        
            if (string.IsNullOrWhiteSpace(txtSubmittedBy.Text) || !int.TryParse(txtSubmittedBy.Text.Trim(), out _))
            {
                MessageBox.Show("Submitted By must be a valid numeric Staff ID.", "Validation Error");
                return;
            }
            if (!ValidationHelper.IsNotEmpty(txtTrainNumber.Text, "Train Number")) return;
            if (!ValidationHelper.IsNotEmpty(txtGuardID.Text, "Guard ID")) return;
            if (!ValidationHelper.IsNotEmpty(txtSMVerification.Text, "SM Verification")) return;
            
            string query = $@"
                INSERT INTO Reg021_CompleteArrival (VerificationID, TrainNumber, ArrivalTime, WagonCount, GuardID, SMVerification, SubmittedBy)
                VALUES ('{txtVerificationID.Text}', '{txtTrainNumber.Text}', '{dtpArrivalTime.Value:yyyy-MM-dd HH:mm:ss.fff}', 
                        {numWagonCount.Value}, '{txtGuardID.Text}', '{txtSMVerification.Text}', {txtSubmittedBy.Text})";

            
            
            try
            {
                db.ExecuteNonQuery(query);
                MessageBox.Show($"Complete Arrival Record Saved!\nVerification ID: {txtVerificationID.Text}", "Success");
                ClearForm();
                GenerateVerificationID();
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
            dtpArrivalTime.Value = DateTime.Now;
            numWagonCount.Value = 0;
            txtGuardID.Clear();
            txtSMVerification.Clear();
        }
    
        protected override void OnHandleCreated(System.EventArgs e) { base.OnHandleCreated(e); TMS.ThemeManager.ApplyTheme(this); }
    }
}
