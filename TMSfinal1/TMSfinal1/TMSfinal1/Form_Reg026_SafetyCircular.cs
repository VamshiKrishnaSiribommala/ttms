using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TMS
{
    public class Form_Reg026_SafetyCircular : Form
    {
        private TextBox txtCircularID;
        private TextBox txtCircularNumber;
        private TextBox txtSubject;
        private TextBox txtVersion;
        private DateTimePicker dtpEffectiveDate;
        private ComboBox cmbAckStatus;
        private TextBox txtAckBySM;
        private ComboBox cmbImplementationStatus;
        private TextBox txtSubmittedBy;
        private DatabaseHelper db = new DatabaseHelper();
        

        public Form_Reg026_SafetyCircular()
        {
            this.Text = "HQ Safety Circular (REG-026)";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.Size = new System.Drawing.Size(800, 700);
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.White;
            
            CreateControls();
            GenerateCircularID();
        }

        private void CreateControls()
        {
            Panel headerPanel = new Panel();
            headerPanel.BackColor = System.Drawing.Color.FromArgb(0, 51, 102);
            headerPanel.Size = new System.Drawing.Size(800, 80);
            headerPanel.Dock = DockStyle.Top;
            this.Controls.Add(headerPanel);
            
            Label lblPath = new Label();
            lblPath.Text = "Home > Infrastructure Sub > HQ Safety Circular";
            lblPath.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Italic);
            lblPath.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);
            lblPath.Size = new System.Drawing.Size(750, 25);
            lblPath.Location = new System.Drawing.Point(25, 15);
            headerPanel.Controls.Add(lblPath);
            
            Label lblTitle = new Label();
            lblTitle.Text = "HQ SAFETY CIRCULAR REGISTER";
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.White;
            lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblTitle.Size = new System.Drawing.Size(750, 35);
            lblTitle.Location = new System.Drawing.Point(25, 40);
            headerPanel.Controls.Add(lblTitle);
            
            int y = 110;
            
            Label lblCircularID = new Label();
            lblCircularID.Text = "Circular ID (System Generated):";
            lblCircularID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblCircularID.Location = new System.Drawing.Point(30, y);
            lblCircularID.Size = new System.Drawing.Size(220, 30);
            this.Controls.Add(lblCircularID);
            
            txtCircularID = new TextBox();
            txtCircularID.Location = new System.Drawing.Point(260, y);
            txtCircularID.Size = new System.Drawing.Size(300, 30);
            txtCircularID.ReadOnly = true;
            txtCircularID.BackColor = System.Drawing.Color.LightGray;
            this.Controls.Add(txtCircularID);
            
            y += 50;
            
            Label lblCircularNumber = new Label();
            lblCircularNumber.Text = "Circular Number *";
            lblCircularNumber.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblCircularNumber.Location = new System.Drawing.Point(30, y);
            lblCircularNumber.Size = new System.Drawing.Size(140, 30);
            this.Controls.Add(lblCircularNumber);
            
            txtCircularNumber = new TextBox();
            txtCircularNumber.Location = new System.Drawing.Point(180, y);
            txtCircularNumber.Size = new System.Drawing.Size(250, 30);
            this.Controls.Add(txtCircularNumber);
            
            y += 50;
            
            Label lblSubject = new Label();
            lblSubject.Text = "Subject *";
            lblSubject.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblSubject.Location = new System.Drawing.Point(30, y);
            lblSubject.Size = new System.Drawing.Size(100, 30);
            this.Controls.Add(lblSubject);
            
            txtSubject = new TextBox();
            txtSubject.Location = new System.Drawing.Point(140, y);
            txtSubject.Size = new System.Drawing.Size(400, 30);
            this.Controls.Add(txtSubject);
            
            y += 50;
            
            Label lblVersion = new Label();
            lblVersion.Text = "Version *";
            lblVersion.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblVersion.Location = new System.Drawing.Point(30, y);
            lblVersion.Size = new System.Drawing.Size(100, 30);
            this.Controls.Add(lblVersion);
            
            txtVersion = new TextBox();
            txtVersion.Location = new System.Drawing.Point(140, y);
            txtVersion.Size = new System.Drawing.Size(150, 30);
            this.Controls.Add(txtVersion);
            
            y += 50;
            
            Label lblEffectiveDate = new Label();
            lblEffectiveDate.Text = "Effective Date *";
            lblEffectiveDate.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblEffectiveDate.Location = new System.Drawing.Point(30, y);
            lblEffectiveDate.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblEffectiveDate);
            
            dtpEffectiveDate = new DateTimePicker();
            dtpEffectiveDate.Location = new System.Drawing.Point(170, y);
            dtpEffectiveDate.Size = new System.Drawing.Size(180, 30);
            dtpEffectiveDate.Format = DateTimePickerFormat.Short;
            this.Controls.Add(dtpEffectiveDate);
            
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
            cmbAckStatus.Items.AddRange(new string[] { "Pending", "Acknowledged", "Overdue" });
            this.Controls.Add(cmbAckStatus);
            
            y += 50;
            
            Label lblAckBySM = new Label();
            lblAckBySM.Text = "Acknowledged By SM *";
            lblAckBySM.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblAckBySM.Location = new System.Drawing.Point(30, y);
            lblAckBySM.Size = new System.Drawing.Size(170, 30);
            this.Controls.Add(lblAckBySM);
            
            txtAckBySM = new TextBox();
            txtAckBySM.Location = new System.Drawing.Point(210, y);
            txtAckBySM.Size = new System.Drawing.Size(180, 30);
            this.Controls.Add(txtAckBySM);
            
            y += 50;
            
            Label lblImplementationStatus = new Label();
            lblImplementationStatus.Text = "Implementation Status *";
            lblImplementationStatus.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblImplementationStatus.Location = new System.Drawing.Point(30, y);
            lblImplementationStatus.Size = new System.Drawing.Size(170, 30);
            this.Controls.Add(lblImplementationStatus);
            
            cmbImplementationStatus = new ComboBox();
            cmbImplementationStatus.Location = new System.Drawing.Point(210, y);
            cmbImplementationStatus.Size = new System.Drawing.Size(180, 30);
            cmbImplementationStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbImplementationStatus.Items.AddRange(new string[] { "Not Started", "In Progress", "Completed", "Complied" });
            this.Controls.Add(cmbImplementationStatus);
            
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
            btnView.Click += (s, e) => new ViewRecordsForm("Reg026_SafetyCircular", "Safety Circular Records").ShowDialog();
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
        
        private void GenerateCircularID()
        {
            string datePart = DateTime.Now.ToString("yyyyMM");
            string query = $"SELECT COUNT(*) FROM Reg026_SafetyCircular WHERE CircularID LIKE 'TMS-REG-026-{datePart}-%'";
            int count = Convert.ToInt32(db.ExecuteScalar(query));
            txtCircularID.Text = $"TMS-REG-026-{datePart}-{(count + 1).ToString("D3")}";
        }
        
        
            private void BtnSave_Click(object sender, EventArgs e)
        {
        
        
            if (string.IsNullOrWhiteSpace(txtSubmittedBy.Text) || !int.TryParse(txtSubmittedBy.Text.Trim(), out _))
            {
                MessageBox.Show("Submitted By must be a valid numeric Staff ID.", "Validation Error");
                return;
            }
            if (!ValidationHelper.IsNotEmpty(txtCircularNumber.Text, "Circular Number")) return;
            if (!ValidationHelper.IsNotEmpty(txtSubject.Text, "Subject")) return;
            if (!ValidationHelper.IsNotEmpty(txtVersion.Text, "Version")) return;
            if (!ValidationHelper.IsSelected(cmbAckStatus, "Acknowledgment Status")) return;
            if (!ValidationHelper.IsNotEmpty(txtAckBySM.Text, "Acknowledged By SM")) return;
            if (!ValidationHelper.IsSelected(cmbImplementationStatus, "Implementation Status")) return;
            
            string query = $@"
                INSERT INTO Reg026_SafetyCircular (CircularID, CircularNumber, Subject, Version, EffectiveDate, AckStatus, AckBySM, ImplementationStatus, SubmittedBy)
                VALUES ('{txtCircularID.Text}', '{txtCircularNumber.Text}', '{txtSubject.Text}', 
                        '{txtVersion.Text}', '{dtpEffectiveDate.Value:yyyy-MM-dd}', '{cmbAckStatus.SelectedItem}', 
                        '{txtAckBySM.Text}', '{cmbImplementationStatus.SelectedItem}', {txtSubmittedBy.Text})";

            
            
            try
            {
                db.ExecuteNonQuery(query);
                MessageBox.Show($"Safety Circular Record Saved!\nCircular ID: {txtCircularID.Text}", "Success");
                ClearForm();
                GenerateCircularID();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error");
            }
        }
        
        
            private void ClearForm()
        {
        
        
            if (txtSubmittedBy != null) txtSubmittedBy.Clear();
            txtCircularNumber.Clear();
            txtSubject.Clear();
            txtVersion.Clear();
            dtpEffectiveDate.Value = DateTime.Now;
            cmbAckStatus.SelectedIndex = -1;
            txtAckBySM.Clear();
            cmbImplementationStatus.SelectedIndex = -1;
        }
    
        protected override void OnHandleCreated(System.EventArgs e) { base.OnHandleCreated(e); TMS.ThemeManager.ApplyTheme(this); }
    }
}
