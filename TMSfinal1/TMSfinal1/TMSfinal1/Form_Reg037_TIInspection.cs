using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TMS
{
    public class Form_Reg037_TIInspection : Form
    {
        private TextBox txtInspectionID;
        private DateTimePicker dtpDateOfVisit;
        private TextBox txtTINameID;
        private ComboBox cmbStaffAlertness;
        private NumericUpDown numSWRKnowledge;
        private RichTextBox txtRegistersChecked;
        private RichTextBox txtOperationalFindings;
        private RichTextBox txtRuleViolations;
        private RichTextBox txtInstructions;
        private DateTimePicker dtpComplianceDue;
        private ComboBox cmbStatus;
        private TextBox txtSubmittedBy;
        private DatabaseHelper db = new DatabaseHelper();
        

        public Form_Reg037_TIInspection()
        {
            this.Text = "TI Inspection (REG-037)";
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
            lblPath.Text = "Home > Safety List > TI Inspection";
            lblPath.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Italic);
            lblPath.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);
            lblPath.Size = new System.Drawing.Size(800, 25);
            lblPath.Location = new System.Drawing.Point(25, 15);
            headerPanel.Controls.Add(lblPath);
            
            Label lblTitle = new Label();
            lblTitle.Text = "TI INSPECTION REGISTER";
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
            
            Label lblDateOfVisit = new Label();
            lblDateOfVisit.Text = "Date & Time of Visit *";
            lblDateOfVisit.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblDateOfVisit.Location = new System.Drawing.Point(30, y);
            lblDateOfVisit.Size = new System.Drawing.Size(160, 30);
            this.Controls.Add(lblDateOfVisit);
            
            dtpDateOfVisit = new DateTimePicker();
            dtpDateOfVisit.Location = new System.Drawing.Point(200, y);
            dtpDateOfVisit.Size = new System.Drawing.Size(200, 30);
            dtpDateOfVisit.Format = DateTimePickerFormat.Custom;
            dtpDateOfVisit.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            dtpDateOfVisit.MaxDate = DateTime.Now;
            this.Controls.Add(dtpDateOfVisit);
            
            y += 50;
            
            Label lblTINameID = new Label();
            lblTINameID.Text = "TI Name/ID *";
            lblTINameID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblTINameID.Location = new System.Drawing.Point(30, y);
            lblTINameID.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblTINameID);
            
            txtTINameID = new TextBox();
            txtTINameID.Location = new System.Drawing.Point(160, y);
            txtTINameID.Size = new System.Drawing.Size(250, 30);
            this.Controls.Add(txtTINameID);
            
            y += 50;
            
            Label lblStaffAlertness = new Label();
            lblStaffAlertness.Text = "Staff Alertness *";
            lblStaffAlertness.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblStaffAlertness.Location = new System.Drawing.Point(30, y);
            lblStaffAlertness.Size = new System.Drawing.Size(140, 30);
            this.Controls.Add(lblStaffAlertness);
            
            cmbStaffAlertness = new ComboBox();
            cmbStaffAlertness.Location = new System.Drawing.Point(180, y);
            cmbStaffAlertness.Size = new System.Drawing.Size(150, 30);
            cmbStaffAlertness.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStaffAlertness.Items.AddRange(new string[] { "Satisfactory", "Unsatisfactory" });
            this.Controls.Add(cmbStaffAlertness);
            
            y += 50;
            
            Label lblSWRKnowledge = new Label();
            lblSWRKnowledge.Text = "SWR Knowledge (1-10) *";
            lblSWRKnowledge.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblSWRKnowledge.Location = new System.Drawing.Point(30, y);
            lblSWRKnowledge.Size = new System.Drawing.Size(180, 30);
            this.Controls.Add(lblSWRKnowledge);
            
            numSWRKnowledge = new NumericUpDown();
            numSWRKnowledge.Location = new System.Drawing.Point(220, y);
            numSWRKnowledge.Size = new System.Drawing.Size(80, 30);
            numSWRKnowledge.Minimum = 1;
            numSWRKnowledge.Maximum = 10;
            this.Controls.Add(numSWRKnowledge);
            
            y += 80;
            
            Label lblRegistersChecked = new Label();
            lblRegistersChecked.Text = "Registers Checked *";
            lblRegistersChecked.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblRegistersChecked.Location = new System.Drawing.Point(30, y);
            lblRegistersChecked.Size = new System.Drawing.Size(150, 30);
            this.Controls.Add(lblRegistersChecked);
            
            txtRegistersChecked = new RichTextBox();
            txtRegistersChecked.Location = new System.Drawing.Point(30, y + 40);
            txtRegistersChecked.Size = new System.Drawing.Size(770, 80);
            txtRegistersChecked.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(txtRegistersChecked);
            
            y += 140;
            
            Label lblOperationalFindings = new Label();
            lblOperationalFindings.Text = "Operational Findings *";
            lblOperationalFindings.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblOperationalFindings.Location = new System.Drawing.Point(30, y);
            lblOperationalFindings.Size = new System.Drawing.Size(160, 30);
            this.Controls.Add(lblOperationalFindings);
            
            txtOperationalFindings = new RichTextBox();
            txtOperationalFindings.Location = new System.Drawing.Point(30, y + 40);
            txtOperationalFindings.Size = new System.Drawing.Size(770, 80);
            txtOperationalFindings.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(txtOperationalFindings);
            
            y += 140;
            
            Label lblRuleViolations = new Label();
            lblRuleViolations.Text = "Rule Violations";
            lblRuleViolations.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblRuleViolations.Location = new System.Drawing.Point(30, y);
            lblRuleViolations.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblRuleViolations);
            
            txtRuleViolations = new RichTextBox();
            txtRuleViolations.Location = new System.Drawing.Point(30, y + 40);
            txtRuleViolations.Size = new System.Drawing.Size(770, 80);
            txtRuleViolations.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(txtRuleViolations);
            
            y += 140;
            
            Label lblInstructions = new Label();
            lblInstructions.Text = "Instructions *";
            lblInstructions.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblInstructions.Location = new System.Drawing.Point(30, y);
            lblInstructions.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblInstructions);
            
            txtInstructions = new RichTextBox();
            txtInstructions.Location = new System.Drawing.Point(30, y + 40);
            txtInstructions.Size = new System.Drawing.Size(770, 80);
            txtInstructions.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(txtInstructions);
            
            y += 140;
            
            Label lblComplianceDue = new Label();
            lblComplianceDue.Text = "Compliance Due";
            lblComplianceDue.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblComplianceDue.Location = new System.Drawing.Point(30, y);
            lblComplianceDue.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblComplianceDue);
            
            dtpComplianceDue = new DateTimePicker();
            dtpComplianceDue.Location = new System.Drawing.Point(170, y);
            dtpComplianceDue.Size = new System.Drawing.Size(180, 30);
            dtpComplianceDue.Format = DateTimePickerFormat.Short;
            dtpComplianceDue.MinDate = DateTime.Today;
            dtpComplianceDue.ShowCheckBox = true;
            dtpComplianceDue.Checked = false;
            this.Controls.Add(dtpComplianceDue);
            
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
            cmbStatus.Items.AddRange(new string[] { "Open", "Compliance Pending", "Verified", "Closed" });
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
            btnView.Click += (s, e) => new ViewRecordsForm("Reg037_TIInspection", "TI Inspection Records").ShowDialog();
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
            string query = $"SELECT COUNT(*) FROM Reg037_TIInspection WHERE InspectionID LIKE 'TMS-REG-037-{datePart}-%'";
            int count = Convert.ToInt32(db.ExecuteScalar(query));
            txtInspectionID.Text = $"TMS-REG-037-{datePart}-{(count + 1).ToString("D3")}";
        }
        
        
            private void BtnSave_Click(object sender, EventArgs e)
        {
        
        
            if (string.IsNullOrWhiteSpace(txtSubmittedBy.Text) || !int.TryParse(txtSubmittedBy.Text.Trim(), out _))
            {
                MessageBox.Show("Submitted By must be a valid numeric Staff ID.", "Validation Error");
                return;
            }
            if (!ValidationHelper.IsNotEmpty(txtTINameID.Text, "TI Name/ID")) return;
            if (!ValidationHelper.IsSelected(cmbStaffAlertness, "Staff Alertness")) return;
            if (!ValidationHelper.IsNotEmpty(txtRegistersChecked.Text, "Registers Checked")) return;
            if (!ValidationHelper.IsNotEmpty(txtOperationalFindings.Text, "Operational Findings")) return;
            if (!ValidationHelper.IsNotEmpty(txtInstructions.Text, "Instructions")) return;
            if (!ValidationHelper.IsSelected(cmbStatus, "Status")) return;
            
            string complianceDue = dtpComplianceDue.Checked ? $"'{dtpComplianceDue.Value:yyyy-MM-dd}'" : "NULL";
            string ruleViolations = string.IsNullOrEmpty(txtRuleViolations.Text) ? "NULL" : $"'{txtRuleViolations.Text.Replace("'", "''")}'";
            
            string query = $@"
                INSERT INTO Reg037_TIInspection (InspectionID, DateOfVisit, TINameID, StaffAlertness, SWRKnowledge, RegistersChecked, OperationalFindings, RuleViolations, Instructions, ComplianceDue, Status, SubmittedBy)
                VALUES ('{txtInspectionID.Text}', '{dtpDateOfVisit.Value:yyyy-MM-dd HH:mm:ss.fff}', '{txtTINameID.Text}', 
                        '{cmbStaffAlertness.SelectedItem}', {numSWRKnowledge.Value}, 
                        '{txtRegistersChecked.Text.Replace("'", "''")}', '{txtOperationalFindings.Text.Replace("'", "''")}', 
                        {ruleViolations}, '{txtInstructions.Text.Replace("'", "''")}', {complianceDue}, 
                        '{cmbStatus.SelectedItem}', {txtSubmittedBy.Text})";

            
            
            try
            {
                db.ExecuteNonQuery(query);
                MessageBox.Show($"TI Inspection Record Saved!\nInspection ID: {txtInspectionID.Text}", "Success");
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
            dtpDateOfVisit.Value = DateTime.Now;
            txtTINameID.Clear();
            cmbStaffAlertness.SelectedIndex = -1;
            numSWRKnowledge.Value = 5;
            txtRegistersChecked.Clear();
            txtOperationalFindings.Clear();
            txtRuleViolations.Clear();
            txtInstructions.Clear();
            dtpComplianceDue.Checked = false;
            cmbStatus.SelectedIndex = -1;
        }
    
        protected override void OnHandleCreated(System.EventArgs e) { base.OnHandleCreated(e); TMS.ThemeManager.ApplyTheme(this); }
    }
}
