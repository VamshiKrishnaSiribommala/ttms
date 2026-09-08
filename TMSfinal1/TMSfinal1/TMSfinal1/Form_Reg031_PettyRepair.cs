using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TMS
{
    public class Form_Reg031_PettyRepair : Form
    {
        private TextBox txtComplaintID;
        private ComboBox cmbAssetCategory;
        private TextBox txtAssetID;
        private RichTextBox txtDefectDescription;
        private ComboBox cmbAssignedDept;
        private DateTimePicker dtpComplaintTime;
        private TextBox txtMaterialsUsed;
        private DateTimePicker dtpCompletionTime;
        private ComboBox cmbCompletionStatus;
        private TextBox txtSubmittedBy;
        private DatabaseHelper db = new DatabaseHelper();
        

        public Form_Reg031_PettyRepair()
        {
            this.Text = "Petty Repair (REG-031)";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.Size = new System.Drawing.Size(850, 750);
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.White;
            
            CreateControls();
            GenerateComplaintID();
        }

        private void CreateControls()
        {
            Panel headerPanel = new Panel();
            headerPanel.BackColor = System.Drawing.Color.FromArgb(0, 51, 102);
            headerPanel.Size = new System.Drawing.Size(850, 80);
            headerPanel.Dock = DockStyle.Top;
            this.Controls.Add(headerPanel);
            
            Label lblPath = new Label();
            lblPath.Text = "Home > Maintenance Sub > Petty Repair";
            lblPath.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Italic);
            lblPath.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);
            lblPath.Size = new System.Drawing.Size(800, 25);
            lblPath.Location = new System.Drawing.Point(25, 15);
            headerPanel.Controls.Add(lblPath);
            
            Label lblTitle = new Label();
            lblTitle.Text = "MINOR / PETTY REPAIR REGISTER";
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.White;
            lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblTitle.Size = new System.Drawing.Size(800, 35);
            lblTitle.Location = new System.Drawing.Point(25, 40);
            headerPanel.Controls.Add(lblTitle);
            
            int y = 110;
            
            Label lblComplaintID = new Label();
            lblComplaintID.Text = "Complaint ID (System Generated):";
            lblComplaintID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblComplaintID.Location = new System.Drawing.Point(30, y);
            lblComplaintID.Size = new System.Drawing.Size(220, 30);
            this.Controls.Add(lblComplaintID);
            
            txtComplaintID = new TextBox();
            txtComplaintID.Location = new System.Drawing.Point(260, y);
            txtComplaintID.Size = new System.Drawing.Size(300, 30);
            txtComplaintID.ReadOnly = true;
            txtComplaintID.BackColor = System.Drawing.Color.LightGray;
            this.Controls.Add(txtComplaintID);
            
            y += 50;
            
            Label lblAssetCategory = new Label();
            lblAssetCategory.Text = "Asset Category *";
            lblAssetCategory.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblAssetCategory.Location = new System.Drawing.Point(30, y);
            lblAssetCategory.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblAssetCategory);
            
            cmbAssetCategory = new ComboBox();
            cmbAssetCategory.Location = new System.Drawing.Point(170, y);
            cmbAssetCategory.Size = new System.Drawing.Size(200, 30);
            cmbAssetCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbAssetCategory.Items.AddRange(new string[] { "Furniture", "Lighting", "Plumbing", "Civil", "Electrical", "S&T", "Mechanical", "Others" });
            this.Controls.Add(cmbAssetCategory);
            
            y += 50;
            
            Label lblAssetID = new Label();
            lblAssetID.Text = "Asset ID *";
            lblAssetID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblAssetID.Location = new System.Drawing.Point(30, y);
            lblAssetID.Size = new System.Drawing.Size(100, 30);
            this.Controls.Add(lblAssetID);
            
            txtAssetID = new TextBox();
            txtAssetID.Location = new System.Drawing.Point(140, y);
            txtAssetID.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(txtAssetID);
            
            y += 80;
            
            Label lblDefectDescription = new Label();
            lblDefectDescription.Text = "Defect Description *";
            lblDefectDescription.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblDefectDescription.Location = new System.Drawing.Point(30, y);
            lblDefectDescription.Size = new System.Drawing.Size(150, 30);
            this.Controls.Add(lblDefectDescription);
            
            txtDefectDescription = new RichTextBox();
            txtDefectDescription.Location = new System.Drawing.Point(30, y + 40);
            txtDefectDescription.Size = new System.Drawing.Size(770, 100);
            txtDefectDescription.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(txtDefectDescription);
            
            y += 160;
            
            Label lblAssignedDept = new Label();
            lblAssignedDept.Text = "Assigned Dept *";
            lblAssignedDept.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblAssignedDept.Location = new System.Drawing.Point(30, y);
            lblAssignedDept.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblAssignedDept);
            
            cmbAssignedDept = new ComboBox();
            cmbAssignedDept.Location = new System.Drawing.Point(170, y);
            cmbAssignedDept.Size = new System.Drawing.Size(200, 30);
            cmbAssignedDept.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbAssignedDept.Items.AddRange(new string[] { "Works", "Electrical", "S&T", "Mechanical", "Civil", "Others" });
            this.Controls.Add(cmbAssignedDept);
            
            y += 50;
            
            Label lblComplaintTime = new Label();
            lblComplaintTime.Text = "Complaint Time *";
            lblComplaintTime.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblComplaintTime.Location = new System.Drawing.Point(30, y);
            lblComplaintTime.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblComplaintTime);
            
            dtpComplaintTime = new DateTimePicker();
            dtpComplaintTime.Location = new System.Drawing.Point(170, y);
            dtpComplaintTime.Size = new System.Drawing.Size(200, 30);
            dtpComplaintTime.Format = DateTimePickerFormat.Custom;
            dtpComplaintTime.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            this.Controls.Add(dtpComplaintTime);
            
            y += 50;
            
            Label lblMaterialsUsed = new Label();
            lblMaterialsUsed.Text = "Materials Used";
            lblMaterialsUsed.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblMaterialsUsed.Location = new System.Drawing.Point(30, y);
            lblMaterialsUsed.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblMaterialsUsed);
            
            txtMaterialsUsed = new TextBox();
            txtMaterialsUsed.Location = new System.Drawing.Point(170, y);
            txtMaterialsUsed.Size = new System.Drawing.Size(400, 30);
            this.Controls.Add(txtMaterialsUsed);
            
            y += 50;
            
            Label lblCompletionTime = new Label();
            lblCompletionTime.Text = "Completion Time";
            lblCompletionTime.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblCompletionTime.Location = new System.Drawing.Point(30, y);
            lblCompletionTime.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblCompletionTime);
            
            dtpCompletionTime = new DateTimePicker();
            dtpCompletionTime.Location = new System.Drawing.Point(170, y);
            dtpCompletionTime.Size = new System.Drawing.Size(200, 30);
            dtpCompletionTime.Format = DateTimePickerFormat.Custom;
            dtpCompletionTime.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            dtpCompletionTime.ShowCheckBox = true;
            dtpCompletionTime.Checked = false;
            this.Controls.Add(dtpCompletionTime);
            
            y += 50;
            
            Label lblCompletionStatus = new Label();
            lblCompletionStatus.Text = "Completion Status *";
            lblCompletionStatus.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblCompletionStatus.Location = new System.Drawing.Point(30, y);
            lblCompletionStatus.Size = new System.Drawing.Size(140, 30);
            this.Controls.Add(lblCompletionStatus);
            
            cmbCompletionStatus = new ComboBox();
            cmbCompletionStatus.Location = new System.Drawing.Point(180, y);
            cmbCompletionStatus.Size = new System.Drawing.Size(180, 30);
            cmbCompletionStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCompletionStatus.Items.AddRange(new string[] { "Open", "Assigned", "In Progress", "Resolved", "Closed" });
            this.Controls.Add(cmbCompletionStatus);
            
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
            btnView.Click += (s, e) => new ViewRecordsForm("Reg031_PettyRepair", "Petty Repair Records").ShowDialog();
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
        
        private void GenerateComplaintID()
        {
            string datePart = DateTime.Now.ToString("yyyyMM");
            string query = $"SELECT COUNT(*) FROM Reg031_PettyRepair WHERE ComplaintID LIKE 'TMS-REG-031-{datePart}-%'";
            int count = Convert.ToInt32(db.ExecuteScalar(query));
            txtComplaintID.Text = $"TMS-REG-031-{datePart}-{(count + 1).ToString("D3")}";
        }
        
        
            private void BtnSave_Click(object sender, EventArgs e)
        {
        
        
            if (string.IsNullOrWhiteSpace(txtSubmittedBy.Text) || !int.TryParse(txtSubmittedBy.Text.Trim(), out _))
            {
                MessageBox.Show("Submitted By must be a valid numeric Staff ID.", "Validation Error");
                return;
            }
            if (!ValidationHelper.IsSelected(cmbAssetCategory, "Asset Category")) return;
            if (!ValidationHelper.IsNotEmpty(txtAssetID.Text, "Asset ID")) return;
            if (!ValidationHelper.IsNotEmpty(txtDefectDescription.Text, "Defect Description")) return;
            if (!ValidationHelper.IsSelected(cmbAssignedDept, "Assigned Dept")) return;
            if (!ValidationHelper.IsSelected(cmbCompletionStatus, "Completion Status")) return;
            
            string completionTime = dtpCompletionTime.Checked ? $"'{dtpCompletionTime.Value:yyyy-MM-dd HH:mm:ss.fff}'" : "NULL";
            
            string query = $@"
                INSERT INTO Reg031_PettyRepair (ComplaintID, AssetCategory, AssetID, DefectDescription, AssignedDept, ComplaintTime, MaterialsUsed, CompletionTime, CompletionStatus, SubmittedBy)
                VALUES ('{txtComplaintID.Text}', '{cmbAssetCategory.SelectedItem}', '{txtAssetID.Text}', 
                        '{txtDefectDescription.Text.Replace("'", "''")}', '{cmbAssignedDept.SelectedItem}', 
                        '{dtpComplaintTime.Value:yyyy-MM-dd HH:mm:ss.fff}', '{txtMaterialsUsed.Text}', {completionTime}, 
                        '{cmbCompletionStatus.SelectedItem}', {txtSubmittedBy.Text})";

            
            
            try
            {
                db.ExecuteNonQuery(query);
                MessageBox.Show($"Petty Repair Record Saved!\nComplaint ID: {txtComplaintID.Text}", "Success");
                ClearForm();
                GenerateComplaintID();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error");
            }
        }
        
        
            private void ClearForm()
        {
        
        
            if (txtSubmittedBy != null) txtSubmittedBy.Clear();
            cmbAssetCategory.SelectedIndex = -1;
            txtAssetID.Clear();
            txtDefectDescription.Clear();
            cmbAssignedDept.SelectedIndex = -1;
            dtpComplaintTime.Value = DateTime.Now;
            txtMaterialsUsed.Clear();
            dtpCompletionTime.Checked = false;
            cmbCompletionStatus.SelectedIndex = -1;
        }
    
        protected override void OnHandleCreated(System.EventArgs e) { base.OnHandleCreated(e); TMS.ThemeManager.ApplyTheme(this); }
    }
}
