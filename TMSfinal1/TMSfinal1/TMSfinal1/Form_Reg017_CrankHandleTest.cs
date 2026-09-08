using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TMS
{
    public class Form_Reg017_CrankHandleTest : Form
    {
        private TextBox txtTestID;
        private TextBox txtHandleID;
        private DateTimePicker dtpTestDate;
        private ComboBox cmbTestType;
        private TextBox txtTesterID;
        private ComboBox cmbTestResult;
        private DateTimePicker dtpNextDueDate;
        private TextBox txtSubmittedBy;
        private DatabaseHelper db = new DatabaseHelper();
        

        public Form_Reg017_CrankHandleTest()
        {
            this.Text = "Crank Handle Testing (REG-017)";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.Size = new System.Drawing.Size(750, 650);
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.White;
            
            CreateControls();
            GenerateTestID();
        }

        private void CreateControls()
        {
            Panel headerPanel = new Panel();
            headerPanel.BackColor = System.Drawing.Color.FromArgb(0, 51, 102);
            headerPanel.Size = new System.Drawing.Size(750, 80);
            headerPanel.Dock = DockStyle.Top;
            this.Controls.Add(headerPanel);
            
            Label lblPath = new Label();
            lblPath.Text = "Home > Maintenance Sub > Crank Handle Testing";
            lblPath.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Italic);
            lblPath.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);
            lblPath.Size = new System.Drawing.Size(700, 25);
            lblPath.Location = new System.Drawing.Point(25, 15);
            headerPanel.Controls.Add(lblPath);
            
            Label lblTitle = new Label();
            lblTitle.Text = "CRANK HANDLE TESTING REGISTER";
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.White;
            lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblTitle.Size = new System.Drawing.Size(700, 35);
            lblTitle.Location = new System.Drawing.Point(25, 40);
            headerPanel.Controls.Add(lblTitle);
            
            int y = 110;
            
            Label lblTestID = new Label();
            lblTestID.Text = "Test ID (System Generated):";
            lblTestID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblTestID.Location = new System.Drawing.Point(30, y);
            lblTestID.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(lblTestID);
            
            txtTestID = new TextBox();
            txtTestID.Location = new System.Drawing.Point(240, y);
            txtTestID.Size = new System.Drawing.Size(300, 30);
            txtTestID.ReadOnly = true;
            txtTestID.BackColor = System.Drawing.Color.LightGray;
            this.Controls.Add(txtTestID);
            
            y += 50;
            
            Label lblHandleID = new Label();
            lblHandleID.Text = "Handle ID *";
            lblHandleID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblHandleID.Location = new System.Drawing.Point(30, y);
            lblHandleID.Size = new System.Drawing.Size(100, 30);
            this.Controls.Add(lblHandleID);
            
            txtHandleID = new TextBox();
            txtHandleID.Location = new System.Drawing.Point(140, y);
            txtHandleID.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(txtHandleID);
            
            y += 50;
            
            Label lblTestDate = new Label();
            lblTestDate.Text = "Test Date *";
            lblTestDate.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblTestDate.Location = new System.Drawing.Point(30, y);
            lblTestDate.Size = new System.Drawing.Size(100, 30);
            this.Controls.Add(lblTestDate);
            
            dtpTestDate = new DateTimePicker();
            dtpTestDate.Location = new System.Drawing.Point(140, y);
            dtpTestDate.Size = new System.Drawing.Size(180, 30);
            dtpTestDate.Format = DateTimePickerFormat.Short;
            this.Controls.Add(dtpTestDate);
            
            y += 50;
            
            Label lblTestType = new Label();
            lblTestType.Text = "Test Type *";
            lblTestType.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblTestType.Location = new System.Drawing.Point(30, y);
            lblTestType.Size = new System.Drawing.Size(100, 30);
            this.Controls.Add(lblTestType);
            
            cmbTestType = new ComboBox();
            cmbTestType.Location = new System.Drawing.Point(140, y);
            cmbTestType.Size = new System.Drawing.Size(180, 30);
            cmbTestType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTestType.Items.AddRange(new string[] { "Functional", "Safety", "Both" });
            this.Controls.Add(cmbTestType);
            
            y += 50;
            
            Label lblTesterID = new Label();
            lblTesterID.Text = "Tester ID *";
            lblTesterID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblTesterID.Location = new System.Drawing.Point(30, y);
            lblTesterID.Size = new System.Drawing.Size(100, 30);
            this.Controls.Add(lblTesterID);
            
            txtTesterID = new TextBox();
            txtTesterID.Location = new System.Drawing.Point(140, y);
            txtTesterID.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(txtTesterID);
            
            y += 50;
            
            Label lblTestResult = new Label();
            lblTestResult.Text = "Test Result *";
            lblTestResult.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblTestResult.Location = new System.Drawing.Point(30, y);
            lblTestResult.Size = new System.Drawing.Size(100, 30);
            this.Controls.Add(lblTestResult);
            
            cmbTestResult = new ComboBox();
            cmbTestResult.Location = new System.Drawing.Point(140, y);
            cmbTestResult.Size = new System.Drawing.Size(150, 30);
            cmbTestResult.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbTestResult.Items.AddRange(new string[] { "Pass", "Fail", "Partial" });
            this.Controls.Add(cmbTestResult);
            
            y += 50;
            
            Label lblNextDueDate = new Label();
            lblNextDueDate.Text = "Next Due Date *";
            lblNextDueDate.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblNextDueDate.Location = new System.Drawing.Point(30, y);
            lblNextDueDate.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblNextDueDate);
            
            dtpNextDueDate = new DateTimePicker();
            dtpNextDueDate.Location = new System.Drawing.Point(160, y);
            dtpNextDueDate.Size = new System.Drawing.Size(180, 30);
            dtpNextDueDate.Format = DateTimePickerFormat.Short;
            dtpNextDueDate.MinDate = DateTime.Today;
            this.Controls.Add(dtpNextDueDate);
            
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
            btnView.Click += (s, e) => new ViewRecordsForm("Reg017_CrankHandleTest", "Crank Handle Test Records").ShowDialog();
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
        
        private void GenerateTestID()
        {
            string datePart = DateTime.Now.ToString("yyyyMMdd");
            string query = $"SELECT COUNT(*) FROM Reg017_CrankHandleTest WHERE TestID LIKE 'TMS-REG-017-{datePart}-%'";
            int count = Convert.ToInt32(db.ExecuteScalar(query));
            txtTestID.Text = $"TMS-REG-017-{datePart}-{(count + 1).ToString("D3")}";
        }
        
        
            private void BtnSave_Click(object sender, EventArgs e)
        {
        
        
            if (string.IsNullOrWhiteSpace(txtSubmittedBy.Text) || !int.TryParse(txtSubmittedBy.Text.Trim(), out _))
            {
                MessageBox.Show("Submitted By must be a valid numeric Staff ID.", "Validation Error");
                return;
            }
            if (!ValidationHelper.IsNotEmpty(txtHandleID.Text, "Handle ID")) return;
            if (!ValidationHelper.IsSelected(cmbTestType, "Test Type")) return;
            if (!ValidationHelper.IsNotEmpty(txtTesterID.Text, "Tester ID")) return;
            if (!ValidationHelper.IsSelected(cmbTestResult, "Test Result")) return;
            
            string query = $@"
                INSERT INTO Reg017_CrankHandleTest (TestID, HandleID, TestDate, TestType, TesterID, TestResult, NextDueDate, SubmittedBy)
                VALUES ('{txtTestID.Text}', '{txtHandleID.Text}', '{dtpTestDate.Value:yyyy-MM-dd}', 
                        '{cmbTestType.SelectedItem}', '{txtTesterID.Text}', '{(cmbTestResult.SelectedItem.ToString() == "Pass" ? "1" : "0")}', 
                        '{dtpNextDueDate.Value:yyyy-MM-dd}', {txtSubmittedBy.Text})";

            
            
            try
            {
                db.ExecuteNonQuery(query);
                MessageBox.Show($"Test Record Saved!\nTest ID: {txtTestID.Text}", "Success");
                ClearForm();
                GenerateTestID();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error");
            }
        }
        
        
            private void ClearForm()
        {
        
        
            if (txtSubmittedBy != null) txtSubmittedBy.Clear();
            txtHandleID.Clear();
            dtpTestDate.Value = DateTime.Now;
            cmbTestType.SelectedIndex = -1;
            txtTesterID.Clear();
            cmbTestResult.SelectedIndex = -1;
            dtpNextDueDate.Value = DateTime.Now.AddMonths(6);
        }
    
        protected override void OnHandleCreated(System.EventArgs e) { base.OnHandleCreated(e); TMS.ThemeManager.ApplyTheme(this); }
    }
}
