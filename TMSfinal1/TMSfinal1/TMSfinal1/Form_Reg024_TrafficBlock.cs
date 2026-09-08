using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TMS
{
    public class Form_Reg024_TrafficBlock : Form
    {
        private TextBox txtBlockID;
        private ComboBox cmbBlockType;
        private ComboBox cmbRequestDept;
        private TextBox txtApprovedBy;
        private DateTimePicker dtpActualStart;
        private DateTimePicker dtpActualEnd;
        private ComboBox cmbAffectedSection;
        private TextBox txtSubmittedBy;
        private DatabaseHelper db = new DatabaseHelper();
        

        public Form_Reg024_TrafficBlock()
        {
            this.Text = "Traffic/Power Block (REG-024)";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.Size = new System.Drawing.Size(750, 700);
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.White;
            
            CreateControls();
            GenerateBlockID();
        }

        private void CreateControls()
        {
            Panel headerPanel = new Panel();
            headerPanel.BackColor = System.Drawing.Color.FromArgb(0, 51, 102);
            headerPanel.Size = new System.Drawing.Size(750, 80);
            headerPanel.Dock = DockStyle.Top;
            this.Controls.Add(headerPanel);
            
            Label lblPath = new Label();
            lblPath.Text = "Home > Maintenance Sub > Traffic/Power Block";
            lblPath.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Italic);
            lblPath.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);
            lblPath.Size = new System.Drawing.Size(700, 25);
            lblPath.Location = new System.Drawing.Point(25, 15);
            headerPanel.Controls.Add(lblPath);
            
            Label lblTitle = new Label();
            lblTitle.Text = "TRAFFIC/POWER BLOCK REGISTER";
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.White;
            lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblTitle.Size = new System.Drawing.Size(700, 35);
            lblTitle.Location = new System.Drawing.Point(25, 40);
            headerPanel.Controls.Add(lblTitle);
            
            int y = 110;
            
            Label lblBlockID = new Label();
            lblBlockID.Text = "Block ID (System Generated):";
            lblBlockID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblBlockID.Location = new System.Drawing.Point(30, y);
            lblBlockID.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(lblBlockID);
            
            txtBlockID = new TextBox();
            txtBlockID.Location = new System.Drawing.Point(240, y);
            txtBlockID.Size = new System.Drawing.Size(300, 30);
            txtBlockID.ReadOnly = true;
            txtBlockID.BackColor = System.Drawing.Color.LightGray;
            this.Controls.Add(txtBlockID);
            
            y += 50;
            
            Label lblBlockType = new Label();
            lblBlockType.Text = "Block Type *";
            lblBlockType.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblBlockType.Location = new System.Drawing.Point(30, y);
            lblBlockType.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblBlockType);
            
            cmbBlockType = new ComboBox();
            cmbBlockType.Location = new System.Drawing.Point(160, y);
            cmbBlockType.Size = new System.Drawing.Size(180, 30);
            cmbBlockType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbBlockType.Items.AddRange(new string[] { "Traffic", "Power", "Both" });
            this.Controls.Add(cmbBlockType);
            
            y += 50;
            
            Label lblRequestDept = new Label();
            lblRequestDept.Text = "Request Dept *";
            lblRequestDept.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblRequestDept.Location = new System.Drawing.Point(30, y);
            lblRequestDept.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblRequestDept);
            
            cmbRequestDept = new ComboBox();
            cmbRequestDept.Location = new System.Drawing.Point(160, y);
            cmbRequestDept.Size = new System.Drawing.Size(180, 30);
            cmbRequestDept.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRequestDept.Items.AddRange(new string[] { "P-Way", "OHE", "S&T", "Works", "Others" });
            this.Controls.Add(cmbRequestDept);
            
            y += 50;
            
            Label lblApprovedBy = new Label();
            lblApprovedBy.Text = "Approved By";
            lblApprovedBy.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblApprovedBy.Location = new System.Drawing.Point(30, y);
            lblApprovedBy.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblApprovedBy);
            
            txtApprovedBy = new TextBox();
            txtApprovedBy.Location = new System.Drawing.Point(160, y);
            txtApprovedBy.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(txtApprovedBy);
            
            y += 50;
            
            Label lblActualStart = new Label();
            lblActualStart.Text = "Actual Start *";
            lblActualStart.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblActualStart.Location = new System.Drawing.Point(30, y);
            lblActualStart.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblActualStart);
            
            dtpActualStart = new DateTimePicker();
            dtpActualStart.Location = new System.Drawing.Point(160, y);
            dtpActualStart.Size = new System.Drawing.Size(200, 30);
            dtpActualStart.Format = DateTimePickerFormat.Custom;
            dtpActualStart.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            this.Controls.Add(dtpActualStart);
            
            y += 50;
            
            Label lblActualEnd = new Label();
            lblActualEnd.Text = "Actual End *";
            lblActualEnd.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblActualEnd.Location = new System.Drawing.Point(30, y);
            lblActualEnd.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblActualEnd);
            
            dtpActualEnd = new DateTimePicker();
            dtpActualEnd.Location = new System.Drawing.Point(160, y);
            dtpActualEnd.Size = new System.Drawing.Size(200, 30);
            dtpActualEnd.Format = DateTimePickerFormat.Custom;
            dtpActualEnd.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            this.Controls.Add(dtpActualEnd);
            
            y += 50;
            
            Label lblAffectedSection = new Label();
            lblAffectedSection.Text = "Affected Section *";
            lblAffectedSection.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblAffectedSection.Location = new System.Drawing.Point(30, y);
            lblAffectedSection.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblAffectedSection);
            
            cmbAffectedSection = new ComboBox();
            cmbAffectedSection.Location = new System.Drawing.Point(170, y);
            cmbAffectedSection.Size = new System.Drawing.Size(200, 30);
            cmbAffectedSection.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbAffectedSection.Items.AddRange(new string[] { "Up Line", "Down Line", "Both Lines", "Yard", "Platform Area" });
            this.Controls.Add(cmbAffectedSection);
            
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
            btnView.Click += (s, e) => new ViewRecordsForm("Reg024_TrafficBlock", "Block Records").ShowDialog();
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
        
        private void GenerateBlockID()
        {
            string datePart = DateTime.Now.ToString("yyyyMMdd");
            string query = $"SELECT COUNT(*) FROM Reg024_TrafficBlock WHERE BlockID LIKE 'TMS-REG-024-{datePart}-%'";
            int count = Convert.ToInt32(db.ExecuteScalar(query));
            txtBlockID.Text = $"TMS-REG-024-{datePart}-{(count + 1).ToString("D3")}";
        }
        
        
            private void BtnSave_Click(object sender, EventArgs e)
        {
        
        
            if (string.IsNullOrWhiteSpace(txtSubmittedBy.Text) || !int.TryParse(txtSubmittedBy.Text.Trim(), out _))
            {
                MessageBox.Show("Submitted By must be a valid numeric Staff ID.", "Validation Error");
                return;
            }
            if (!ValidationHelper.IsSelected(cmbBlockType, "Block Type")) return;
            if (!ValidationHelper.IsSelected(cmbRequestDept, "Request Dept")) return;
            if (!ValidationHelper.IsSelected(cmbAffectedSection, "Affected Section")) return;
            if (!ValidationHelper.IsEndAfterStart(dtpActualStart.Value, dtpActualEnd.Value, "Actual Start", "Actual End")) return;
            
            string approvedBy = string.IsNullOrEmpty(txtApprovedBy.Text) ? "NULL" : $"'{txtApprovedBy.Text}'";
            
            string query = $@"
                INSERT INTO Reg024_TrafficBlock (BlockID, BlockType, RequestDept, ApprovedBy, ActualStart, ActualEnd, AffectedSection, SubmittedBy)
                VALUES ('{txtBlockID.Text}', '{cmbBlockType.SelectedItem}', '{cmbRequestDept.SelectedItem}', 
                        {approvedBy}, '{dtpActualStart.Value:yyyy-MM-dd HH:mm:ss.fff}', '{dtpActualEnd.Value:yyyy-MM-dd HH:mm:ss.fff}', 
                        '{cmbAffectedSection.SelectedItem}', {txtSubmittedBy.Text})";

            
            
            try
            {
                db.ExecuteNonQuery(query);
                MessageBox.Show($"Block Record Saved!\nBlock ID: {txtBlockID.Text}", "Success");
                ClearForm();
                GenerateBlockID();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error");
            }
        }
        
        
            private void ClearForm()
        {
        
        
            if (txtSubmittedBy != null) txtSubmittedBy.Clear();
            cmbBlockType.SelectedIndex = -1;
            cmbRequestDept.SelectedIndex = -1;
            txtApprovedBy.Clear();
            dtpActualStart.Value = DateTime.Now;
            dtpActualEnd.Value = DateTime.Now.AddHours(2);
            cmbAffectedSection.SelectedIndex = -1;
        }
    
        protected override void OnHandleCreated(System.EventArgs e) { base.OnHandleCreated(e); TMS.ThemeManager.ApplyTheme(this); }
    }
}
