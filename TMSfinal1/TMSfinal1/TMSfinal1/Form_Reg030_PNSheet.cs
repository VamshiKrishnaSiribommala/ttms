using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TMS
{
    public class Form_Reg030_PNSheet : Form
    {
        private TextBox txtPNNumber;
        private ComboBox cmbPurpose;
        private TextBox txtAssociatedTrainNo;
        private TextBox txtFromStation;
        private ComboBox cmbNextStation;
        private TextBox txtRecipientName;
        private DateTimePicker dtpExchangeTime;
        private ComboBox cmbAckStatus;
        private ComboBox cmbPNStatus;
        private TextBox txtSubmittedBy;
        private DatabaseHelper db = new DatabaseHelper();
        

        public Form_Reg030_PNSheet()
        {
            this.Text = "Private Number Sheet (REG-030)";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.Size = new System.Drawing.Size(750, 700);
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.White;
            
            CreateControls();
            GeneratePNNumber();
        }

        private void CreateControls()
        {
            Panel headerPanel = new Panel();
            headerPanel.BackColor = System.Drawing.Color.FromArgb(0, 51, 102);
            headerPanel.Size = new System.Drawing.Size(750, 80);
            headerPanel.Dock = DockStyle.Top;
            this.Controls.Add(headerPanel);
            
            Label lblPath = new Label();
            lblPath.Text = "Home > Infrastructure Sub > Private Number Sheet";
            lblPath.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Italic);
            lblPath.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);
            lblPath.Size = new System.Drawing.Size(700, 25);
            lblPath.Location = new System.Drawing.Point(25, 15);
            headerPanel.Controls.Add(lblPath);
            
            Label lblTitle = new Label();
            lblTitle.Text = "PRIVATE NUMBER (PN) SHEET REGISTER";
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.White;
            lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblTitle.Size = new System.Drawing.Size(700, 35);
            lblTitle.Location = new System.Drawing.Point(25, 40);
            headerPanel.Controls.Add(lblTitle);
            
            int y = 110;
            
            Label lblPNNumber = new Label();
            lblPNNumber.Text = "PN Number (System Generated):";
            lblPNNumber.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblPNNumber.Location = new System.Drawing.Point(30, y);
            lblPNNumber.Size = new System.Drawing.Size(220, 30);
            this.Controls.Add(lblPNNumber);
            
            txtPNNumber = new TextBox();
            txtPNNumber.Location = new System.Drawing.Point(260, y);
            txtPNNumber.Size = new System.Drawing.Size(300, 30);
            txtPNNumber.ReadOnly = true;
            txtPNNumber.BackColor = System.Drawing.Color.LightGray;
            this.Controls.Add(txtPNNumber);
            
            y += 50;
            
            Label lblPurpose = new Label();
            lblPurpose.Text = "Purpose *";
            lblPurpose.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblPurpose.Location = new System.Drawing.Point(30, y);
            lblPurpose.Size = new System.Drawing.Size(100, 30);
            this.Controls.Add(lblPurpose);
            
            cmbPurpose = new ComboBox();
            cmbPurpose.Location = new System.Drawing.Point(140, y);
            cmbPurpose.Size = new System.Drawing.Size(200, 30);
            cmbPurpose.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPurpose.Items.AddRange(new string[] { "Line Clear", "Block Forward", "Block Back", "Reset", "Emergency", "Others" });
            this.Controls.Add(cmbPurpose);
            
            y += 50;
            
            Label lblAssociatedTrainNo = new Label();
            lblAssociatedTrainNo.Text = "Associated Train No";
            lblAssociatedTrainNo.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblAssociatedTrainNo.Location = new System.Drawing.Point(30, y);
            lblAssociatedTrainNo.Size = new System.Drawing.Size(150, 30);
            this.Controls.Add(lblAssociatedTrainNo);
            
            txtAssociatedTrainNo = new TextBox();
            txtAssociatedTrainNo.Location = new System.Drawing.Point(190, y);
            txtAssociatedTrainNo.Size = new System.Drawing.Size(180, 30);
            this.Controls.Add(txtAssociatedTrainNo);
            
            y += 50;
            
            Label lblFromStation = new Label();
            lblFromStation.Text = "From Station *";
            lblFromStation.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblFromStation.Location = new System.Drawing.Point(30, y);
            lblFromStation.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblFromStation);
            
            txtFromStation = new TextBox();
            txtFromStation.Location = new System.Drawing.Point(160, y);
            txtFromStation.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(txtFromStation);
            
            y += 50;
            
            Label lblNextStation = new Label();
            lblNextStation.Text = "Next Station *";
            lblNextStation.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblNextStation.Location = new System.Drawing.Point(30, y);
            lblNextStation.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblNextStation);
            
            cmbNextStation = new ComboBox();
            cmbNextStation.Location = new System.Drawing.Point(160, y);
            cmbNextStation.Size = new System.Drawing.Size(200, 30);
            cmbNextStation.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbNextStation.Items.AddRange(new string[] { "Station A", "Station B", "Station C", "Station D", "Station E" });
            this.Controls.Add(cmbNextStation);
            
            y += 50;
            
            Label lblRecipientName = new Label();
            lblRecipientName.Text = "Recipient Name/ID *";
            lblRecipientName.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblRecipientName.Location = new System.Drawing.Point(30, y);
            lblRecipientName.Size = new System.Drawing.Size(150, 30);
            this.Controls.Add(lblRecipientName);
            
            txtRecipientName = new TextBox();
            txtRecipientName.Location = new System.Drawing.Point(190, y);
            txtRecipientName.Size = new System.Drawing.Size(250, 30);
            this.Controls.Add(txtRecipientName);
            
            y += 50;
            
            Label lblExchangeTime = new Label();
            lblExchangeTime.Text = "Exchange Time *";
            lblExchangeTime.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblExchangeTime.Location = new System.Drawing.Point(30, y);
            lblExchangeTime.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblExchangeTime);
            
            dtpExchangeTime = new DateTimePicker();
            dtpExchangeTime.Location = new System.Drawing.Point(170, y);
            dtpExchangeTime.Size = new System.Drawing.Size(200, 30);
            dtpExchangeTime.Format = DateTimePickerFormat.Custom;
            dtpExchangeTime.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            this.Controls.Add(dtpExchangeTime);
            
            y += 50;
            
            Label lblAckStatus = new Label();
            lblAckStatus.Text = "Acknowledgment Status *";
            lblAckStatus.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblAckStatus.Location = new System.Drawing.Point(30, y);
            lblAckStatus.Size = new System.Drawing.Size(170, 30);
            this.Controls.Add(lblAckStatus);
            
            cmbAckStatus = new ComboBox();
            cmbAckStatus.Location = new System.Drawing.Point(210, y);
            cmbAckStatus.Size = new System.Drawing.Size(150, 30);
            cmbAckStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbAckStatus.Items.AddRange(new string[] { "Pending", "Confirmed" });
            this.Controls.Add(cmbAckStatus);
            
            y += 50;
            
            Label lblPNStatus = new Label();
            lblPNStatus.Text = "PN Status *";
            lblPNStatus.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblPNStatus.Location = new System.Drawing.Point(30, y);
            lblPNStatus.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblPNStatus);
            
            cmbPNStatus = new ComboBox();
            cmbPNStatus.Location = new System.Drawing.Point(160, y);
            cmbPNStatus.Size = new System.Drawing.Size(150, 30);
            cmbPNStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPNStatus.Items.AddRange(new string[] { "Open", "Used", "Cancelled" });
            this.Controls.Add(cmbPNStatus);
            
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
            btnView.Click += (s, e) => new ViewRecordsForm("Reg030_PNSheet", "PN Sheet Records").ShowDialog();
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
        
        private void GeneratePNNumber()
        {
            string datePart = DateTime.Now.ToString("yyyyMMdd");
            string query = $"SELECT COUNT(*) FROM Reg030_PNSheet WHERE PNNumber LIKE 'TMS-PN-{datePart}-%'";
            int count = Convert.ToInt32(db.ExecuteScalar(query));
            txtPNNumber.Text = $"TMS-PN-{datePart}-{(count + 1).ToString("D4")}";
        }
        
        
            private void BtnSave_Click(object sender, EventArgs e)
        {
        
        
            if (string.IsNullOrWhiteSpace(txtSubmittedBy.Text) || !int.TryParse(txtSubmittedBy.Text.Trim(), out _))
            {
                MessageBox.Show("Submitted By must be a valid numeric Staff ID.", "Validation Error");
                return;
            }
            if (!ValidationHelper.IsSelected(cmbPurpose, "Purpose")) return;
            if (!ValidationHelper.IsNotEmpty(txtFromStation.Text, "From Station")) return;
            if (!ValidationHelper.IsSelected(cmbNextStation, "Next Station")) return;
            if (!ValidationHelper.IsNotEmpty(txtRecipientName.Text, "Recipient Name")) return;
            if (!ValidationHelper.IsSelected(cmbAckStatus, "Acknowledgment Status")) return;
            if (!ValidationHelper.IsSelected(cmbPNStatus, "PN Status")) return;
            
            string associatedTrain = string.IsNullOrEmpty(txtAssociatedTrainNo.Text) ? "NULL" : $"'{txtAssociatedTrainNo.Text}'";
            
            string query = $@"
                INSERT INTO Reg030_PNSheet (PNNumber, Purpose, AssocTrainNo, FromStation, NextStation, RecipientName, ExchangeTime, AckStatus, PNStatus, SubmittedBy)
                VALUES ('{txtPNNumber.Text}', '{cmbPurpose.SelectedItem}', {associatedTrain}, 
                        '{txtFromStation.Text}', '{cmbNextStation.SelectedItem}', '{txtRecipientName.Text}', 
                        '{dtpExchangeTime.Value:yyyy-MM-dd HH:mm:ss.fff}', '{cmbAckStatus.SelectedItem}', '{cmbPNStatus.SelectedItem}', {txtSubmittedBy.Text})";

            
            
            try
            {
                db.ExecuteNonQuery(query);
                MessageBox.Show($"PN Sheet Record Saved!\nPN Number: {txtPNNumber.Text}", "Success");
                ClearForm();
                GeneratePNNumber();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error");
            }
        }
        
        
            private void ClearForm()
        {
        
        
            if (txtSubmittedBy != null) txtSubmittedBy.Clear();
            cmbPurpose.SelectedIndex = -1;
            txtAssociatedTrainNo.Clear();
            txtFromStation.Clear();
            cmbNextStation.SelectedIndex = -1;
            txtRecipientName.Clear();
            dtpExchangeTime.Value = DateTime.Now;
            cmbAckStatus.SelectedIndex = -1;
            cmbPNStatus.SelectedIndex = -1;
        }
    
        protected override void OnHandleCreated(System.EventArgs e) { base.OnHandleCreated(e); TMS.ThemeManager.ApplyTheme(this); }
    }
}
