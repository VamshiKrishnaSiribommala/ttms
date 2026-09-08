using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TMS
{
    public class Form_Reg005_Failure : Form
    {
        private TextBox txtFailureID;
        private ComboBox cmbAssetType;
        private TextBox txtAssetID;
        private DateTimePicker dtpFailureTime;
        private TextBox txtReportedTo;
        private DateTimePicker dtpRectificationTime;
        private TextBox txtSubmittedBy;
        private DatabaseHelper db = new DatabaseHelper();
        

        public Form_Reg005_Failure()
        {
            this.Text = "Signal/Point/Block Failure (REG-005)";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.Size = new System.Drawing.Size(750, 600);
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.White;
            
            CreateControls();
            GenerateFailureID();
        }

        private void CreateControls()
        {
            Panel headerPanel = new Panel();
            headerPanel.BackColor = System.Drawing.Color.FromArgb(0, 51, 102);
            headerPanel.Size = new System.Drawing.Size(750, 80);
            headerPanel.Dock = DockStyle.Top;
            this.Controls.Add(headerPanel);
            
            Label lblPath = new Label();
            lblPath.Text = "Home > Operational List > Signal/Point/Block Failure";
            lblPath.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Italic);
            lblPath.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);
            lblPath.Size = new System.Drawing.Size(700, 25);
            lblPath.Location = new System.Drawing.Point(25, 15);
            headerPanel.Controls.Add(lblPath);
            
            Label lblTitle = new Label();
            lblTitle.Text = "SIGNAL/POINT/BLOCK FAILURE REGISTER";
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 14, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.White;
            lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblTitle.Size = new System.Drawing.Size(700, 35);
            lblTitle.Location = new System.Drawing.Point(25, 40);
            headerPanel.Controls.Add(lblTitle);
            
            int y = 110;
            
            Label lblFailureID = new Label();
            lblFailureID.Text = "Failure ID (System Generated):";
            lblFailureID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblFailureID.Location = new System.Drawing.Point(30, y);
            lblFailureID.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(lblFailureID);
            
            txtFailureID = new TextBox();
            txtFailureID.Location = new System.Drawing.Point(240, y);
            txtFailureID.Size = new System.Drawing.Size(300, 30);
            txtFailureID.ReadOnly = true;
            txtFailureID.BackColor = System.Drawing.Color.LightGray;
            this.Controls.Add(txtFailureID);
            
            y += 50;
            
            Label lblAssetType = new Label();
            lblAssetType.Text = "Asset Type *";
            lblAssetType.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblAssetType.Location = new System.Drawing.Point(30, y);
            lblAssetType.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblAssetType);
            
            cmbAssetType = new ComboBox();
            cmbAssetType.Location = new System.Drawing.Point(160, y);
            cmbAssetType.Size = new System.Drawing.Size(180, 30);
            cmbAssetType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbAssetType.Items.AddRange(new string[] { "Signal", "Point", "Block Instrument", "Track Circuit" });
            this.Controls.Add(cmbAssetType);
            
            Label lblAssetID = new Label();
            lblAssetID.Text = "Asset ID *";
            lblAssetID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblAssetID.Location = new System.Drawing.Point(370, y);
            lblAssetID.Size = new System.Drawing.Size(80, 30);
            this.Controls.Add(lblAssetID);
            
            txtAssetID = new TextBox();
            txtAssetID.Location = new System.Drawing.Point(460, y);
            txtAssetID.Size = new System.Drawing.Size(150, 30);
            this.Controls.Add(txtAssetID);
            
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
            
            Label lblReportedTo = new Label();
            lblReportedTo.Text = "Reported To *";
            lblReportedTo.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblReportedTo.Location = new System.Drawing.Point(30, y);
            lblReportedTo.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblReportedTo);
            
            txtReportedTo = new TextBox();
            txtReportedTo.Location = new System.Drawing.Point(160, y);
            txtReportedTo.Size = new System.Drawing.Size(250, 30);
            this.Controls.Add(txtReportedTo);
            
            y += 50;
            
            Label lblRectificationTime = new Label();
            lblRectificationTime.Text = "Rectification Time";
            lblRectificationTime.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblRectificationTime.Location = new System.Drawing.Point(30, y);
            lblRectificationTime.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblRectificationTime);
            
            dtpRectificationTime = new DateTimePicker();
            dtpRectificationTime.Location = new System.Drawing.Point(170, y);
            dtpRectificationTime.Size = new System.Drawing.Size(200, 30);
            dtpRectificationTime.Format = DateTimePickerFormat.Custom;
            dtpRectificationTime.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            dtpRectificationTime.ShowCheckBox = true;
            dtpRectificationTime.Checked = false;
            this.Controls.Add(dtpRectificationTime);
            
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
            btnView.Click += (s, e) => new ViewRecordsForm("Reg005_Failure", "Failure Records").ShowDialog();
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
        
        private void GenerateFailureID()
        {
            string datePart = DateTime.Now.ToString("yyyyMMdd");
            string query = $"SELECT COUNT(*) FROM Reg005_Failure WHERE FailureID LIKE 'TMS-REG-005-{datePart}-%'";
            int count = Convert.ToInt32(db.ExecuteScalar(query));
            txtFailureID.Text = $"TMS-REG-005-{datePart}-{(count + 1).ToString("D3")}";
        }
        
        
            private void BtnSave_Click(object sender, EventArgs e)
        {
        
        
            if (string.IsNullOrWhiteSpace(txtSubmittedBy.Text) || !int.TryParse(txtSubmittedBy.Text.Trim(), out _))
            {
                MessageBox.Show("Submitted By must be a valid numeric Staff ID.", "Validation Error");
                return;
            }
            if (!ValidationHelper.IsSelected(cmbAssetType, "Asset Type")) return;
            if (!ValidationHelper.IsNotEmpty(txtAssetID.Text, "Asset ID")) return;
            if (!ValidationHelper.IsNotEmpty(txtReportedTo.Text, "Reported To")) return;
            
            string rectTime = dtpRectificationTime.Checked ? $"'{dtpRectificationTime.Value:yyyy-MM-dd HH:mm:ss.fff}'" : "NULL";
            
            string query = $@"
                INSERT INTO Reg005_Failure (FailureID, AssetType, AssetID, FailureTime, ReportedTo, RectificationTime, SubmittedBy)
                VALUES ('{txtFailureID.Text}', '{cmbAssetType.SelectedItem}', '{txtAssetID.Text}', 
                        '{dtpFailureTime.Value:yyyy-MM-dd HH:mm:ss.fff}', '{txtReportedTo.Text}', {rectTime}, {txtSubmittedBy.Text})";

            
            
            try
            {
                db.ExecuteNonQuery(query);
                MessageBox.Show($"Failure Record Saved!\nFailure ID: {txtFailureID.Text}", "Success");
                ClearForm();
                GenerateFailureID();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error");
            }
        }
        
        
            private void ClearForm()
        {
        
        
            if (txtSubmittedBy != null) txtSubmittedBy.Clear();
            cmbAssetType.SelectedIndex = -1;
            txtAssetID.Clear();
            dtpFailureTime.Value = DateTime.Now;
            txtReportedTo.Clear();
            dtpRectificationTime.Checked = false;
            dtpRectificationTime.Value = DateTime.Now;
        }
    
        protected override void OnHandleCreated(System.EventArgs e) { base.OnHandleCreated(e); TMS.ThemeManager.ApplyTheme(this); }
    }
}
