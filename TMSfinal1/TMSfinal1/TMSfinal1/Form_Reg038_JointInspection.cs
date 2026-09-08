using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TMS
{
    public class Form_Reg038_JointInspection : Form
    {
        private TextBox txtInspectionID;
        private DateTimePicker dtpInspectionDate;
        private RichTextBox txtJointDepts;
        private TextBox txtAssetID;
        private ComboBox cmbParameterType;
        private NumericUpDown numMeasuredValue;
        private TextBox txtStandardRange;
        private ComboBox cmbResult;
        private RichTextBox txtDeptObservations;
        private ComboBox cmbConsensusResult;
        private CheckBox chkComplianceReq;
        private ComboBox cmbStatus;
        private TextBox txtSubmittedBy;
        private DatabaseHelper db = new DatabaseHelper();
        

        public Form_Reg038_JointInspection()
        {
            this.Text = "Joint Inspection (REG-038)";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.Size = new System.Drawing.Size(850, 850);
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.White;
            
            CreateControls();
            GenerateInspectionID();
        }

        private void CreateControls()
        {
            Panel headerPanel = new Panel();
            headerPanel.BackColor = System.Drawing.Color.FromArgb(0, 51, 102);
            headerPanel.Size = new System.Drawing.Size(850, 80);
            headerPanel.Dock = DockStyle.Top;
            this.Controls.Add(headerPanel);
            
            Label lblPath = new Label();
            lblPath.Text = "Home > Safety List > Joint Inspection";
            lblPath.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Italic);
            lblPath.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);
            lblPath.Size = new System.Drawing.Size(800, 25);
            lblPath.Location = new System.Drawing.Point(25, 15);
            headerPanel.Controls.Add(lblPath);
            
            Label lblTitle = new Label();
            lblTitle.Text = "JOINT INSPECTION REGISTER";
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.White;
            lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblTitle.Size = new System.Drawing.Size(800, 35);
            lblTitle.Location = new System.Drawing.Point(25, 40);
            headerPanel.Controls.Add(lblTitle);
            
            int y = 110;
            
            Label lblInspectionID = new Label();
            lblInspectionID.Text = "Inspection ID (System Generated):";
            lblInspectionID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblInspectionID.Location = new System.Drawing.Point(30, y);
            lblInspectionID.Size = new System.Drawing.Size(220, 30);
            this.Controls.Add(lblInspectionID);
            
            txtInspectionID = new TextBox();
            txtInspectionID.Location = new System.Drawing.Point(260, y);
            txtInspectionID.Size = new System.Drawing.Size(300, 30);
            txtInspectionID.ReadOnly = true;
            txtInspectionID.BackColor = System.Drawing.Color.LightGray;
            this.Controls.Add(txtInspectionID);
            
            y += 50;
            
            Label lblInspectionDate = new Label();
            lblInspectionDate.Text = "Inspection Date & Time *";
            lblInspectionDate.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblInspectionDate.Location = new System.Drawing.Point(30, y);
            lblInspectionDate.Size = new System.Drawing.Size(170, 30);
            this.Controls.Add(lblInspectionDate);
            
            dtpInspectionDate = new DateTimePicker();
            dtpInspectionDate.Location = new System.Drawing.Point(210, y);
            dtpInspectionDate.Size = new System.Drawing.Size(200, 30);
            dtpInspectionDate.Format = DateTimePickerFormat.Custom;
            dtpInspectionDate.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            dtpInspectionDate.MaxDate = DateTime.Now;
            this.Controls.Add(dtpInspectionDate);
            
            y += 80;
            
            Label lblJointDepts = new Label();
            lblJointDepts.Text = "Joint Departments *";
            lblJointDepts.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblJointDepts.Location = new System.Drawing.Point(30, y);
            lblJointDepts.Size = new System.Drawing.Size(150, 30);
            this.Controls.Add(lblJointDepts);
            
            txtJointDepts = new RichTextBox();
            txtJointDepts.Location = new System.Drawing.Point(30, y + 40);
            txtJointDepts.Size = new System.Drawing.Size(770, 60);
            txtJointDepts.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(txtJointDepts);
            
            y += 120;
            
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
            
            y += 50;
            
            Label lblParameterType = new Label();
            lblParameterType.Text = "Parameter Type *";
            lblParameterType.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblParameterType.Location = new System.Drawing.Point(30, y);
            lblParameterType.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblParameterType);
            
            cmbParameterType = new ComboBox();
            cmbParameterType.Location = new System.Drawing.Point(170, y);
            cmbParameterType.Size = new System.Drawing.Size(180, 30);
            cmbParameterType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbParameterType.Items.AddRange(new string[] { "Gauge", "Voltage", "Clearance", "Resistance", "Continuity", "Others" });
            this.Controls.Add(cmbParameterType);
            
            y += 50;
            
            Label lblMeasuredValue = new Label();
            lblMeasuredValue.Text = "Measured Value *";
            lblMeasuredValue.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblMeasuredValue.Location = new System.Drawing.Point(30, y);
            lblMeasuredValue.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblMeasuredValue);
            
            numMeasuredValue = new NumericUpDown();
            numMeasuredValue.Location = new System.Drawing.Point(170, y);
            numMeasuredValue.Size = new System.Drawing.Size(120, 30);
            numMeasuredValue.Minimum = 0;
            numMeasuredValue.Maximum = 999999;
            numMeasuredValue.DecimalPlaces = 2;
            this.Controls.Add(numMeasuredValue);
            
            y += 50;
            
            Label lblStandardRange = new Label();
            lblStandardRange.Text = "Standard Range *";
            lblStandardRange.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblStandardRange.Location = new System.Drawing.Point(30, y);
            lblStandardRange.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblStandardRange);
            
            txtStandardRange = new TextBox();
            txtStandardRange.Location = new System.Drawing.Point(170, y);
            txtStandardRange.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(txtStandardRange);
            
            y += 50;
            
            Label lblResult = new Label();
            lblResult.Text = "Result *";
            lblResult.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblResult.Location = new System.Drawing.Point(30, y);
            lblResult.Size = new System.Drawing.Size(100, 30);
            this.Controls.Add(lblResult);
            
            cmbResult = new ComboBox();
            cmbResult.Location = new System.Drawing.Point(140, y);
            cmbResult.Size = new System.Drawing.Size(150, 30);
            cmbResult.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbResult.Items.AddRange(new string[] { "Within Limit", "Out of Limit" });
            this.Controls.Add(cmbResult);
            
            y += 80;
            
            Label lblDeptObservations = new Label();
            lblDeptObservations.Text = "Department Observations *";
            lblDeptObservations.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblDeptObservations.Location = new System.Drawing.Point(30, y);
            lblDeptObservations.Size = new System.Drawing.Size(180, 30);
            this.Controls.Add(lblDeptObservations);
            
            txtDeptObservations = new RichTextBox();
            txtDeptObservations.Location = new System.Drawing.Point(30, y + 40);
            txtDeptObservations.Size = new System.Drawing.Size(770, 80);
            txtDeptObservations.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(txtDeptObservations);
            
            y += 140;
            
            Label lblConsensusResult = new Label();
            lblConsensusResult.Text = "Consensus Result *";
            lblConsensusResult.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblConsensusResult.Location = new System.Drawing.Point(30, y);
            lblConsensusResult.Size = new System.Drawing.Size(140, 30);
            this.Controls.Add(lblConsensusResult);
            
            cmbConsensusResult = new ComboBox();
            cmbConsensusResult.Location = new System.Drawing.Point(180, y);
            cmbConsensusResult.Size = new System.Drawing.Size(150, 30);
            cmbConsensusResult.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbConsensusResult.Items.AddRange(new string[] { "Fit", "Unfit", "Conditional" });
            this.Controls.Add(cmbConsensusResult);
            
            y += 50;
            
            Label lblComplianceReq = new Label();
            lblComplianceReq.Text = "Compliance Required *";
            lblComplianceReq.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblComplianceReq.Location = new System.Drawing.Point(30, y);
            lblComplianceReq.Size = new System.Drawing.Size(160, 30);
            this.Controls.Add(lblComplianceReq);
            
            chkComplianceReq = new CheckBox();
            chkComplianceReq.Text = "Yes";
            chkComplianceReq.Location = new System.Drawing.Point(200, y);
            chkComplianceReq.Size = new System.Drawing.Size(80, 30);
            this.Controls.Add(chkComplianceReq);
            
            y += 50;
            
            Label lblStatus = new Label();
            lblStatus.Text = "Status *";
            lblStatus.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblStatus.Location = new System.Drawing.Point(30, y);
            lblStatus.Size = new System.Drawing.Size(100, 30);
            this.Controls.Add(lblStatus);
            
            cmbStatus = new ComboBox();
            cmbStatus.Location = new System.Drawing.Point(140, y);
            cmbStatus.Size = new System.Drawing.Size(180, 30);
            cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStatus.Items.AddRange(new string[] { "Open", "Compliance Pending", "Closed" });
            this.Controls.Add(cmbStatus);
            
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
            btnView.Click += (s, e) => new ViewRecordsForm("Reg038_JointInspection", "Joint Inspection Records").ShowDialog();
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
        
        private void GenerateInspectionID()
        {
            string datePart = DateTime.Now.ToString("yyyyMM");
            string query = $"SELECT COUNT(*) FROM Reg038_JointInspection WHERE InspectionID LIKE 'TMS-REG-038-{datePart}-%'";
            int count = Convert.ToInt32(db.ExecuteScalar(query));
            txtInspectionID.Text = $"TMS-REG-038-{datePart}-{(count + 1).ToString("D3")}";
        }
        
        
            private void BtnSave_Click(object sender, EventArgs e)
        {
        
        
            if (string.IsNullOrWhiteSpace(txtSubmittedBy.Text) || !int.TryParse(txtSubmittedBy.Text.Trim(), out _))
            {
                MessageBox.Show("Submitted By must be a valid numeric Staff ID.", "Validation Error");
                return;
            }
            if (!ValidationHelper.IsNotEmpty(txtJointDepts.Text, "Joint Departments")) return;
            if (!ValidationHelper.IsNotEmpty(txtAssetID.Text, "Asset ID")) return;
            if (!ValidationHelper.IsSelected(cmbParameterType, "Parameter Type")) return;
            if (!ValidationHelper.IsSelected(cmbResult, "Result")) return;
            if (!ValidationHelper.IsNotEmpty(txtDeptObservations.Text, "Department Observations")) return;
            if (!ValidationHelper.IsSelected(cmbConsensusResult, "Consensus Result")) return;
            if (!ValidationHelper.IsSelected(cmbStatus, "Status")) return;
            
            string query = $@"
                INSERT INTO Reg038_JointInspection (InspectionID, InspectionDate, JointDepts, AssetID, ParameterType, MeasuredValue, StandardRange, Result, DeptObservations, ConsensusResult, ComplianceReq, Status, SubmittedBy)
                VALUES ('{txtInspectionID.Text}', '{dtpInspectionDate.Value:yyyy-MM-dd HH:mm:ss.fff}', 
                        '{txtJointDepts.Text.Replace("'", "''")}', '{txtAssetID.Text}', '{cmbParameterType.SelectedItem}', 
                        {numMeasuredValue.Value}, '{txtStandardRange.Text}', '{cmbResult.SelectedItem}', 
                        '{txtDeptObservations.Text.Replace("'", "''")}', '{cmbConsensusResult.SelectedItem}', 
                        {(chkComplianceReq.Checked ? "1" : "0")}', '{cmbStatus.SelectedItem}', {txtSubmittedBy.Text})";

            
            
            try
            {
                db.ExecuteNonQuery(query);
                MessageBox.Show($"Joint Inspection Record Saved!\nInspection ID: {txtInspectionID.Text}", "Success");
                ClearForm();
                GenerateInspectionID();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error");
            }
        }
        
        
            private void ClearForm()
        {
        
        
            if (txtSubmittedBy != null) txtSubmittedBy.Clear();
            dtpInspectionDate.Value = DateTime.Now;
            txtJointDepts.Clear();
            txtAssetID.Clear();
            cmbParameterType.SelectedIndex = -1;
            numMeasuredValue.Value = 0;
            txtStandardRange.Clear();
            cmbResult.SelectedIndex = -1;
            txtDeptObservations.Clear();
            cmbConsensusResult.SelectedIndex = -1;
            chkComplianceReq.Checked = false;
            cmbStatus.SelectedIndex = -1;
        }
    
        protected override void OnHandleCreated(System.EventArgs e) { base.OnHandleCreated(e); TMS.ThemeManager.ApplyTheme(this); }
    }
}
