using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TMS
{
    public class Form_Reg022_ControlInstruction : Form
    {
        private TextBox txtMessageID;
        private ComboBox cmbMessageType;
        private TextBox txtControllerID;
        private RichTextBox txtContent;
        private DateTimePicker dtpValidityStart;
        private DateTimePicker dtpValidityEnd;
        private CheckBox chkAckRequired;
        private TextBox txtAcknowledgedBy;
        private ComboBox cmbComplianceStatus;
        private TextBox txtSubmittedBy;
        private DatabaseHelper db = new DatabaseHelper();
        

        public Form_Reg022_ControlInstruction()
        {
            this.Text = "Control Instruction (REG-022)";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.Size = new System.Drawing.Size(800, 750);
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.White;
            
            CreateControls();
            GenerateMessageID();
        }

        private void CreateControls()
        {
            Panel headerPanel = new Panel();
            headerPanel.BackColor = System.Drawing.Color.FromArgb(0, 51, 102);
            headerPanel.Size = new System.Drawing.Size(800, 80);
            headerPanel.Dock = DockStyle.Top;
            this.Controls.Add(headerPanel);
            
            Label lblPath = new Label();
            lblPath.Text = "Home > Maintenance Sub > Control Instruction";
            lblPath.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Italic);
            lblPath.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);
            lblPath.Size = new System.Drawing.Size(750, 25);
            lblPath.Location = new System.Drawing.Point(25, 15);
            headerPanel.Controls.Add(lblPath);
            
            Label lblTitle = new Label();
            lblTitle.Text = "CONTROL OFFICE INSTRUCTION REGISTER";
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 14, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.White;
            lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblTitle.Size = new System.Drawing.Size(750, 35);
            lblTitle.Location = new System.Drawing.Point(25, 40);
            headerPanel.Controls.Add(lblTitle);
            
            int y = 110;
            
            Label lblMessageID = new Label();
            lblMessageID.Text = "Message ID (System Generated):";
            lblMessageID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblMessageID.Location = new System.Drawing.Point(30, y);
            lblMessageID.Size = new System.Drawing.Size(220, 30);
            this.Controls.Add(lblMessageID);
            
            txtMessageID = new TextBox();
            txtMessageID.Location = new System.Drawing.Point(260, y);
            txtMessageID.Size = new System.Drawing.Size(300, 30);
            txtMessageID.ReadOnly = true;
            txtMessageID.BackColor = System.Drawing.Color.LightGray;
            this.Controls.Add(txtMessageID);
            
            y += 50;
            
            Label lblMessageType = new Label();
            lblMessageType.Text = "Message Type *";
            lblMessageType.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblMessageType.Location = new System.Drawing.Point(30, y);
            lblMessageType.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblMessageType);
            
            cmbMessageType = new ComboBox();
            cmbMessageType.Location = new System.Drawing.Point(160, y);
            cmbMessageType.Size = new System.Drawing.Size(180, 30);
            cmbMessageType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbMessageType.Items.AddRange(new string[] { "Order", "Caution", "Advisory" });
            this.Controls.Add(cmbMessageType);
            
            y += 50;
            
            Label lblControllerID = new Label();
            lblControllerID.Text = "Controller ID";
            lblControllerID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblControllerID.Location = new System.Drawing.Point(30, y);
            lblControllerID.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblControllerID);
            
            txtControllerID = new TextBox();
            txtControllerID.Location = new System.Drawing.Point(160, y);
            txtControllerID.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(txtControllerID);
            
            y += 80;
            
            Label lblContent = new Label();
            lblContent.Text = "Content *";
            lblContent.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblContent.Location = new System.Drawing.Point(30, y);
            lblContent.Size = new System.Drawing.Size(100, 30);
            this.Controls.Add(lblContent);
            
            txtContent = new RichTextBox();
            txtContent.Location = new System.Drawing.Point(30, y + 40);
            txtContent.Size = new System.Drawing.Size(720, 100);
            txtContent.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(txtContent);
            
            y += 160;
            
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
            
            y += 50;
            
            Label lblValidityEnd = new Label();
            lblValidityEnd.Text = "Validity End";
            lblValidityEnd.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblValidityEnd.Location = new System.Drawing.Point(30, y);
            lblValidityEnd.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblValidityEnd);
            
            dtpValidityEnd = new DateTimePicker();
            dtpValidityEnd.Location = new System.Drawing.Point(160, y);
            dtpValidityEnd.Size = new System.Drawing.Size(200, 30);
            dtpValidityEnd.Format = DateTimePickerFormat.Custom;
            dtpValidityEnd.CustomFormat = "dd/MM/yyyy HH:mm";
            dtpValidityEnd.ShowCheckBox = true;
            dtpValidityEnd.Checked = false;
            this.Controls.Add(dtpValidityEnd);
            
            y += 50;
            
            Label lblAckRequired = new Label();
            lblAckRequired.Text = "Acknowledgment Required *";
            lblAckRequired.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblAckRequired.Location = new System.Drawing.Point(30, y);
            lblAckRequired.Size = new System.Drawing.Size(180, 30);
            this.Controls.Add(lblAckRequired);
            
            chkAckRequired = new CheckBox();
            chkAckRequired.Text = "Yes";
            chkAckRequired.Location = new System.Drawing.Point(220, y);
            chkAckRequired.Size = new System.Drawing.Size(80, 30);
            this.Controls.Add(chkAckRequired);
            
            y += 50;
            
            Label lblAcknowledgedBy = new Label();
            lblAcknowledgedBy.Text = "Acknowledged By";
            lblAcknowledgedBy.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblAcknowledgedBy.Location = new System.Drawing.Point(30, y);
            lblAcknowledgedBy.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblAcknowledgedBy);
            
            txtAcknowledgedBy = new TextBox();
            txtAcknowledgedBy.Location = new System.Drawing.Point(170, y);
            txtAcknowledgedBy.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(txtAcknowledgedBy);
            
            y += 50;
            
            Label lblComplianceStatus = new Label();
            lblComplianceStatus.Text = "Compliance Status *";
            lblComplianceStatus.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblComplianceStatus.Location = new System.Drawing.Point(30, y);
            lblComplianceStatus.Size = new System.Drawing.Size(140, 30);
            this.Controls.Add(lblComplianceStatus);
            
            cmbComplianceStatus = new ComboBox();
            cmbComplianceStatus.Location = new System.Drawing.Point(180, y);
            cmbComplianceStatus.Size = new System.Drawing.Size(150, 30);
            cmbComplianceStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbComplianceStatus.Items.AddRange(new string[] { "Pending", "In Progress", "Completed", "Complied" });
            this.Controls.Add(cmbComplianceStatus);
            
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
            btnView.Click += (s, e) => new ViewRecordsForm("Reg022_ControlInstruction", "Control Instruction Records").ShowDialog();
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
        
        private void GenerateMessageID()
        {
            string datePart = DateTime.Now.ToString("yyyyMMdd");
            string query = $"SELECT COUNT(*) FROM Reg022_ControlInstruction WHERE MessageID LIKE 'TMS-REG-022-{datePart}-%'";
            int count = Convert.ToInt32(db.ExecuteScalar(query));
            txtMessageID.Text = $"TMS-REG-022-{datePart}-{(count + 1).ToString("D3")}";
        }
        
        
            private void BtnSave_Click(object sender, EventArgs e)
        {
        
        
            if (string.IsNullOrWhiteSpace(txtSubmittedBy.Text) || !int.TryParse(txtSubmittedBy.Text.Trim(), out _))
            {
                MessageBox.Show("Submitted By must be a valid numeric Staff ID.", "Validation Error");
                return;
            }
            if (!ValidationHelper.IsSelected(cmbMessageType, "Message Type")) return;
            if (!ValidationHelper.IsNotEmpty(txtContent.Text, "Content")) return;
            if (!ValidationHelper.IsSelected(cmbComplianceStatus, "Compliance Status")) return;
            
            string validityEnd = dtpValidityEnd.Checked ? $"'{dtpValidityEnd.Value:yyyy-MM-dd HH:mm}'" : "NULL";
            string acknowledgedBy = string.IsNullOrEmpty(txtAcknowledgedBy.Text) ? "NULL" : $"'{txtAcknowledgedBy.Text}'";
            string controllerID = string.IsNullOrEmpty(txtControllerID.Text) ? "NULL" : $"'{txtControllerID.Text}'";
            
            string query = $@"
                INSERT INTO Reg022_ControlInstruction (MessageID, MessageType, ControllerID, Content, ValidityStart, ValidityEnd, AckRequired, AcknowledgedBy, ComplianceStatus, SubmittedBy)
                VALUES ('{txtMessageID.Text}', '{cmbMessageType.SelectedItem}', {controllerID}, 
                        '{txtContent.Text.Replace("'", "''")}', '{dtpValidityStart.Value:yyyy-MM-dd HH:mm:ss.fff}', {validityEnd}, 
                        {(chkAckRequired.Checked ? "1" : "0")}, {acknowledgedBy}, '{cmbComplianceStatus.SelectedItem}', {txtSubmittedBy.Text})";

            
            
            try
            {
                db.ExecuteNonQuery(query);
                MessageBox.Show($"Control Instruction Saved!\nMessage ID: {txtMessageID.Text}", "Success");
                ClearForm();
                GenerateMessageID();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error");
            }
        }
        
        
            private void ClearForm()
        {
        
        
            if (txtSubmittedBy != null) txtSubmittedBy.Clear();
            cmbMessageType.SelectedIndex = -1;
            txtControllerID.Clear();
            txtContent.Clear();
            dtpValidityStart.Value = DateTime.Now;
            dtpValidityEnd.Checked = false;
            chkAckRequired.Checked = false;
            txtAcknowledgedBy.Clear();
            cmbComplianceStatus.SelectedIndex = -1;
        }
    
        protected override void OnHandleCreated(System.EventArgs e) { base.OnHandleCreated(e); TMS.ThemeManager.ApplyTheme(this); }
    }
}
