using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TMS
{
    public class Form_Reg003_SWR : Form
    {
        private TextBox txtAckID;
        private TextBox txtStaffID;
        private TextBox txtSWRVersion;
        private DateTimePicker dtpReadingDate;
        private TextBox txtVerifiedBy;
        private TextBox txtSubmittedBy;
        private DatabaseHelper db = new DatabaseHelper();
        

        public Form_Reg003_SWR()
        {
            this.Text = "SWR Acknowledgment (REG-003)";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.Size = new System.Drawing.Size(700, 550);
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.White;
            
            CreateControls();
            GenerateAckID();
        }

        private void CreateControls()
        {
            Panel headerPanel = new Panel();
            headerPanel.BackColor = System.Drawing.Color.FromArgb(0, 51, 102);
            headerPanel.Size = new System.Drawing.Size(700, 80);
            headerPanel.Dock = DockStyle.Top;
            this.Controls.Add(headerPanel);
            
            Label lblPath = new Label();
            lblPath.Text = "Home > Operational List > SWR Acknowledgment";
            lblPath.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Italic);
            lblPath.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);
            lblPath.Size = new System.Drawing.Size(650, 25);
            lblPath.Location = new System.Drawing.Point(25, 15);
            headerPanel.Controls.Add(lblPath);
            
            Label lblTitle = new Label();
            lblTitle.Text = "SWR ACKNOWLEDGMENT REGISTER";
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.White;
            lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblTitle.Size = new System.Drawing.Size(650, 35);
            lblTitle.Location = new System.Drawing.Point(25, 40);
            headerPanel.Controls.Add(lblTitle);
            
            int y = 110;
            
            Label lblAckID = new Label();
            lblAckID.Text = "Ack ID (System Generated):";
            lblAckID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblAckID.Location = new System.Drawing.Point(30, y);
            lblAckID.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(lblAckID);
            
            txtAckID = new TextBox();
            txtAckID.Location = new System.Drawing.Point(240, y);
            txtAckID.Size = new System.Drawing.Size(300, 30);
            txtAckID.ReadOnly = true;
            txtAckID.BackColor = System.Drawing.Color.LightGray;
            this.Controls.Add(txtAckID);
            
            y += 50;
            
            Label lblStaffID = new Label();
            lblStaffID.Text = "Staff ID *";
            lblStaffID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblStaffID.Location = new System.Drawing.Point(30, y);
            lblStaffID.Size = new System.Drawing.Size(100, 30);
            this.Controls.Add(lblStaffID);
            
            txtStaffID = new TextBox();
            txtStaffID.Location = new System.Drawing.Point(140, y);
            txtStaffID.Size = new System.Drawing.Size(150, 30);
            this.Controls.Add(txtStaffID);
            
            Label lblSWRVersion = new Label();
            lblSWRVersion.Text = "SWR Version *";
            lblSWRVersion.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblSWRVersion.Location = new System.Drawing.Point(310, y);
            lblSWRVersion.Size = new System.Drawing.Size(100, 30);
            this.Controls.Add(lblSWRVersion);
            
            txtSWRVersion = new TextBox();
            txtSWRVersion.Location = new System.Drawing.Point(420, y);
            txtSWRVersion.Size = new System.Drawing.Size(150, 30);
            this.Controls.Add(txtSWRVersion);
            
            y += 50;
            
            Label lblReadingDate = new Label();
            lblReadingDate.Text = "Date of Reading *";
            lblReadingDate.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblReadingDate.Location = new System.Drawing.Point(30, y);
            lblReadingDate.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblReadingDate);
            
            dtpReadingDate = new DateTimePicker();
            dtpReadingDate.Location = new System.Drawing.Point(160, y);
            dtpReadingDate.Size = new System.Drawing.Size(180, 30);
            dtpReadingDate.Format = DateTimePickerFormat.Short;
            this.Controls.Add(dtpReadingDate);
            
            y += 50;
            
            Label lblVerifiedBy = new Label();
            lblVerifiedBy.Text = "Verified By *";
            lblVerifiedBy.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblVerifiedBy.Location = new System.Drawing.Point(30, y);
            lblVerifiedBy.Size = new System.Drawing.Size(100, 30);
            this.Controls.Add(lblVerifiedBy);
            
            txtVerifiedBy = new TextBox();
            txtVerifiedBy.Location = new System.Drawing.Point(140, y);
            txtVerifiedBy.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(txtVerifiedBy);
            
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
            btnView.Click += (s, e) => new ViewRecordsForm("Reg003_SWR", "SWR Records").ShowDialog();
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
        
        private void GenerateAckID()
        {
            string datePart = DateTime.Now.ToString("yyyyMMdd");
            string query = $"SELECT COUNT(*) FROM Reg003_SWR WHERE AckID LIKE 'TMS-REG-003-{datePart}-%'";
            int count = Convert.ToInt32(db.ExecuteScalar(query));
            txtAckID.Text = $"TMS-REG-003-{datePart}-{(count + 1).ToString("D3")}";
        }
        
        
            private void BtnSave_Click(object sender, EventArgs e)
        {
        
        
            if (string.IsNullOrWhiteSpace(txtSubmittedBy.Text) || !int.TryParse(txtSubmittedBy.Text.Trim(), out _))
            {
                MessageBox.Show("Submitted By must be a valid numeric Staff ID.", "Validation Error");
                return;
            }
            if (!ValidationHelper.IsNotEmpty(txtStaffID.Text, "Staff ID")) return;
            if (!ValidationHelper.IsNotEmpty(txtSWRVersion.Text, "SWR Version")) return;
            if (!ValidationHelper.IsNotEmpty(txtVerifiedBy.Text, "Verified By")) return;
            
            string query = $@"
                INSERT INTO Reg003_SWR (AckID, StaffID, SWRVersion, DateOfReading, VerifiedBy, SubmittedBy)
                VALUES ('{txtAckID.Text}', '{txtStaffID.Text}', '{txtSWRVersion.Text}', '{dtpReadingDate.Value:yyyy-MM-dd}', '{txtVerifiedBy.Text}', {txtSubmittedBy.Text})";

            
            
            try
            {
                db.ExecuteNonQuery(query);
                MessageBox.Show($"Record Saved!\nAck ID: {txtAckID.Text}", "Success");
                ClearForm();
                GenerateAckID();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error");
            }
        }
        
        
            private void ClearForm()
        {
        
        
            if (txtSubmittedBy != null) txtSubmittedBy.Clear();
            txtStaffID.Clear();
            txtSWRVersion.Clear();
            dtpReadingDate.Value = DateTime.Now;
            txtVerifiedBy.Clear();
        }
    
        protected override void OnHandleCreated(System.EventArgs e) { base.OnHandleCreated(e); TMS.ThemeManager.ApplyTheme(this); }
    }
}
