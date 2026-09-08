using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TMS
{
    public class Form_Reg004_CautionOrder : Form
    {
        private TextBox txtCautionID;
        private TextBox txtSection;
        private NumericUpDown numSpeedLimit;
        private ComboBox cmbReason;
        private DateTimePicker dtpValidityStart;
        private DateTimePicker dtpValidityEnd;
        private TextBox txtIssuedTo;
        private TextBox txtSubmittedBy;
        private DatabaseHelper db = new DatabaseHelper();
        

        public Form_Reg004_CautionOrder()
        {
            this.Text = "Caution Order (REG-004)";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.Size = new System.Drawing.Size(800, 650);
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.White;
            
            CreateControls();
            GenerateCautionID();
        }

        private void CreateControls()
        {
            Panel headerPanel = new Panel();
            headerPanel.BackColor = System.Drawing.Color.FromArgb(0, 51, 102);
            headerPanel.Size = new System.Drawing.Size(800, 80);
            headerPanel.Dock = DockStyle.Top;
            this.Controls.Add(headerPanel);
            
            Label lblPath = new Label();
            lblPath.Text = "Home > Operational List > Caution Order";
            lblPath.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Italic);
            lblPath.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);
            lblPath.Size = new System.Drawing.Size(750, 25);
            lblPath.Location = new System.Drawing.Point(25, 15);
            headerPanel.Controls.Add(lblPath);
            
            Label lblTitle = new Label();
            lblTitle.Text = "CAUTION ORDER REGISTER";
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.White;
            lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblTitle.Size = new System.Drawing.Size(750, 35);
            lblTitle.Location = new System.Drawing.Point(25, 40);
            headerPanel.Controls.Add(lblTitle);
            
            int y = 110;
            
            Label lblCautionID = new Label();
            lblCautionID.Text = "Caution ID (System Generated):";
            lblCautionID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblCautionID.Location = new System.Drawing.Point(30, y);
            lblCautionID.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(lblCautionID);
            
            txtCautionID = new TextBox();
            txtCautionID.Location = new System.Drawing.Point(240, y);
            txtCautionID.Size = new System.Drawing.Size(300, 30);
            txtCautionID.ReadOnly = true;
            txtCautionID.BackColor = System.Drawing.Color.LightGray;
            this.Controls.Add(txtCautionID);
            
            y += 50;
            
            Label lblSection = new Label();
            lblSection.Text = "Section (Kilometer Range) *";
            lblSection.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblSection.Location = new System.Drawing.Point(30, y);
            lblSection.Size = new System.Drawing.Size(180, 30);
            this.Controls.Add(lblSection);
            
            txtSection = new TextBox();
            txtSection.Location = new System.Drawing.Point(220, y);
            txtSection.Size = new System.Drawing.Size(350, 30);
            this.Controls.Add(txtSection);
            
            y += 50;
            
            Label lblSpeedLimit = new Label();
            lblSpeedLimit.Text = "Speed Limit (kmph) *";
            lblSpeedLimit.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblSpeedLimit.Location = new System.Drawing.Point(30, y);
            lblSpeedLimit.Size = new System.Drawing.Size(150, 30);
            this.Controls.Add(lblSpeedLimit);
            
            numSpeedLimit = new NumericUpDown();
            numSpeedLimit.Location = new System.Drawing.Point(190, y);
            numSpeedLimit.Size = new System.Drawing.Size(100, 30);
            numSpeedLimit.Minimum = 0;
            numSpeedLimit.Maximum = 200;
            numSpeedLimit.Value = 20;
            this.Controls.Add(numSpeedLimit);
            
            y += 50;
            
            Label lblReason = new Label();
            lblReason.Text = "Reason *";
            lblReason.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblReason.Location = new System.Drawing.Point(30, y);
            lblReason.Size = new System.Drawing.Size(100, 30);
            this.Controls.Add(lblReason);
            
            cmbReason = new ComboBox();
            cmbReason.Location = new System.Drawing.Point(140, y);
            cmbReason.Size = new System.Drawing.Size(200, 30);
            cmbReason.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbReason.Items.AddRange(new string[] { "Track Maintenance", "Signal Failure", "Bridge Work", "Level Crossing Work", "P-Way Work", "OHE Work", "Others" });
            this.Controls.Add(cmbReason);
            
            y += 50;
            
            Label lblValidityStart = new Label();
            lblValidityStart.Text = "Validity Start *";
            lblValidityStart.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblValidityStart.Location = new System.Drawing.Point(30, y);
            lblValidityStart.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblValidityStart);
            
            dtpValidityStart = new DateTimePicker();
            dtpValidityStart.Location = new System.Drawing.Point(160, y);
            dtpValidityStart.Size = new System.Drawing.Size(200, 30);
            dtpValidityStart.Format = DateTimePickerFormat.Custom;
            dtpValidityStart.CustomFormat = "dd/MM/yyyy HH:mm";
            this.Controls.Add(dtpValidityStart);
            
            Label lblValidityEnd = new Label();
            lblValidityEnd.Text = "Validity End *";
            lblValidityEnd.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblValidityEnd.Location = new System.Drawing.Point(390, y);
            lblValidityEnd.Size = new System.Drawing.Size(100, 30);
            this.Controls.Add(lblValidityEnd);
            
            dtpValidityEnd = new DateTimePicker();
            dtpValidityEnd.Location = new System.Drawing.Point(500, y);
            dtpValidityEnd.Size = new System.Drawing.Size(200, 30);
            dtpValidityEnd.Format = DateTimePickerFormat.Custom;
            dtpValidityEnd.CustomFormat = "dd/MM/yyyy HH:mm";
            dtpValidityEnd.Value = DateTime.Now.AddDays(1);
            this.Controls.Add(dtpValidityEnd);
            
            y += 50;
            
            Label lblIssuedTo = new Label();
            lblIssuedTo.Text = "Issued To *";
            lblIssuedTo.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblIssuedTo.Location = new System.Drawing.Point(30, y);
            lblIssuedTo.Size = new System.Drawing.Size(100, 30);
            this.Controls.Add(lblIssuedTo);
            
            txtIssuedTo = new TextBox();
            txtIssuedTo.Location = new System.Drawing.Point(140, y);
            txtIssuedTo.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(txtIssuedTo);
            
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
            btnView.Click += (s, e) => new ViewRecordsForm("Reg004_CautionOrder", "Caution Order Records").ShowDialog();
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
        
        private void GenerateCautionID()
        {
            string datePart = DateTime.Now.ToString("yyyyMMdd");
            string query = $"SELECT COUNT(*) FROM Reg004_CautionOrder WHERE CautionID LIKE 'TMS-REG-004-{datePart}-%'";
            int count = Convert.ToInt32(db.ExecuteScalar(query));
            txtCautionID.Text = $"TMS-REG-004-{datePart}-{(count + 1).ToString("D3")}";
        }
        
        
            private void BtnSave_Click(object sender, EventArgs e)
        {
        
        
            if (string.IsNullOrWhiteSpace(txtSubmittedBy.Text) || !int.TryParse(txtSubmittedBy.Text.Trim(), out _))
            {
                MessageBox.Show("Submitted By must be a valid numeric Staff ID.", "Validation Error");
                return;
            }
            if (!ValidationHelper.IsNotEmpty(txtSection.Text, "Section")) return;
            if (!ValidationHelper.IsSelected(cmbReason, "Reason")) return;
            if (!ValidationHelper.IsNotEmpty(txtIssuedTo.Text, "Issued To")) return;
            if (!ValidationHelper.IsEndAfterStart(dtpValidityStart.Value, dtpValidityEnd.Value, "Validity Start", "Validity End")) return;
            
            string query = $@"
                INSERT INTO Reg004_CautionOrder (CautionID, Section, SpeedLimit, Reason, ValidityStart, ValidityEnd, IssuedTo, SubmittedBy)
                VALUES ('{txtCautionID.Text}', '{txtSection.Text}', {numSpeedLimit.Value}, '{cmbReason.SelectedItem}', 
                        '{dtpValidityStart.Value:yyyy-MM-dd HH:mm:ss.fff}', '{dtpValidityEnd.Value:yyyy-MM-dd HH:mm:ss.fff}', '{txtIssuedTo.Text}', {txtSubmittedBy.Text})";

            
            
            try
            {
                db.ExecuteNonQuery(query);
                MessageBox.Show($"Caution Order Issued!\nCaution ID: {txtCautionID.Text}", "Success");
                ClearForm();
                GenerateCautionID();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error");
            }
        }
        
        
            private void ClearForm()
        {
        
        
            if (txtSubmittedBy != null) txtSubmittedBy.Clear();
            txtSection.Clear();
            numSpeedLimit.Value = 20;
            cmbReason.SelectedIndex = -1;
            dtpValidityStart.Value = DateTime.Now;
            dtpValidityEnd.Value = DateTime.Now.AddDays(1);
            txtIssuedTo.Clear();
        }
    
        protected override void OnHandleCreated(System.EventArgs e) { base.OnHandleCreated(e); TMS.ThemeManager.ApplyTheme(this); }
    }
}
