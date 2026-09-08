using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TMS
{
    public class Form_Reg011_PublicComplaint : Form
    {
        private TextBox txtComplaintID;
        private TextBox txtComplainantName;
        private TextBox txtPNR;
        private ComboBox cmbCategory;
        private RichTextBox txtDescription;
        private ComboBox cmbStatus;
        private TextBox txtSubmittedBy;
        private DatabaseHelper db = new DatabaseHelper();
        

        public Form_Reg011_PublicComplaint()
        {
            this.Text = "Public Complaint (REG-011)";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.Size = new System.Drawing.Size(800, 700);
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.White;
            
            CreateControls();
            GenerateComplaintID();
        }

        private void CreateControls()
        {
            Panel headerPanel = new Panel();
            headerPanel.BackColor = System.Drawing.Color.FromArgb(0, 51, 102);
            headerPanel.Size = new System.Drawing.Size(800, 80);
            headerPanel.Dock = DockStyle.Top;
            this.Controls.Add(headerPanel);
            
            Label lblPath = new Label();
            lblPath.Text = "Home > Operational List > Public Complaint";
            lblPath.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Italic);
            lblPath.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);
            lblPath.Size = new System.Drawing.Size(750, 25);
            lblPath.Location = new System.Drawing.Point(25, 15);
            headerPanel.Controls.Add(lblPath);
            
            Label lblTitle = new Label();
            lblTitle.Text = "PUBLIC COMPLAINT REGISTER";
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.White;
            lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblTitle.Size = new System.Drawing.Size(750, 35);
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
            
            Label lblComplainantName = new Label();
            lblComplainantName.Text = "Complainant Name *";
            lblComplainantName.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblComplainantName.Location = new System.Drawing.Point(30, y);
            lblComplainantName.Size = new System.Drawing.Size(150, 30);
            this.Controls.Add(lblComplainantName);
            
            txtComplainantName = new TextBox();
            txtComplainantName.Location = new System.Drawing.Point(190, y);
            txtComplainantName.Size = new System.Drawing.Size(250, 30);
            this.Controls.Add(txtComplainantName);
            
            y += 50;
            
            Label lblPNR = new Label();
            lblPNR.Text = "PNR / Ticket No";
            lblPNR.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblPNR.Location = new System.Drawing.Point(30, y);
            lblPNR.Size = new System.Drawing.Size(150, 30);
            this.Controls.Add(lblPNR);
            
            txtPNR = new TextBox();
            txtPNR.Location = new System.Drawing.Point(190, y);
            txtPNR.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(txtPNR);
            
            y += 50;
            
            Label lblCategory = new Label();
            lblCategory.Text = "Category *";
            lblCategory.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblCategory.Location = new System.Drawing.Point(30, y);
            lblCategory.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblCategory);
            
            cmbCategory = new ComboBox();
            cmbCategory.Location = new System.Drawing.Point(160, y);
            cmbCategory.Size = new System.Drawing.Size(200, 30);
            cmbCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCategory.Items.AddRange(new string[] { "Cleanliness", "Staff Behavior", "Security", "Amenities", "Catering", "Others" });
            this.Controls.Add(cmbCategory);
            
            y += 80;
            
            Label lblDescription = new Label();
            lblDescription.Text = "Complaint Details *";
            lblDescription.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblDescription.Location = new System.Drawing.Point(30, y);
            lblDescription.Size = new System.Drawing.Size(150, 30);
            this.Controls.Add(lblDescription);
            
            txtDescription = new RichTextBox();
            txtDescription.Location = new System.Drawing.Point(30, y + 40);
            txtDescription.Size = new System.Drawing.Size(720, 120);
            txtDescription.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(txtDescription);
            
            y += 180;
            
            Label lblStatus = new Label();
            lblStatus.Text = "Status *";
            lblStatus.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblStatus.Location = new System.Drawing.Point(30, y);
            lblStatus.Size = new System.Drawing.Size(100, 30);
            this.Controls.Add(lblStatus);
            
            cmbStatus = new ComboBox();
            cmbStatus.Location = new System.Drawing.Point(140, y);
            cmbStatus.Size = new System.Drawing.Size(150, 30);
            cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStatus.Items.AddRange(new string[] { "Pending", "Under Review", "Resolved", "Closed", "Escalated" });
            this.Controls.Add(cmbStatus);
            
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
            btnView.Click += (s, e) => new ViewRecordsForm("Reg011_PublicComplaint", "Public Complaint Records").ShowDialog();
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
            string datePart = DateTime.Now.ToString("yyyyMMdd");
            string query = $"SELECT COUNT(*) FROM Reg011_PublicComplaint WHERE ComplaintID LIKE 'TMS-REG-011-{datePart}-%'";
            int count = Convert.ToInt32(db.ExecuteScalar(query));
            txtComplaintID.Text = $"TMS-REG-011-{datePart}-{(count + 1).ToString("D3")}";
        }
        
        
            private void BtnSave_Click(object sender, EventArgs e)
        {
        
        
            if (string.IsNullOrWhiteSpace(txtSubmittedBy.Text) || !int.TryParse(txtSubmittedBy.Text.Trim(), out _))
            {
                MessageBox.Show("Submitted By must be a valid numeric Staff ID.", "Validation Error");
                return;
            }
            if (!ValidationHelper.IsNotEmpty(txtComplainantName.Text, "Complainant Name")) return;
            if (!ValidationHelper.IsSelected(cmbCategory, "Category")) return;
            if (!ValidationHelper.IsNotEmpty(txtDescription.Text, "Complaint Details")) return;
            if (!ValidationHelper.IsSelected(cmbStatus, "Status")) return;
            
            string pnr = string.IsNullOrEmpty(txtPNR.Text) ? "NULL" : $"'{txtPNR.Text}'";
            
            string query = $@"
                INSERT INTO Reg011_PublicComplaint (ComplaintID, ComplainantName, PNR_TicketNo, Category, Description, Status, SubmittedBy)
                VALUES ('{txtComplaintID.Text}', '{txtComplainantName.Text}', {pnr}, 
                        '{cmbCategory.SelectedItem}', '{txtDescription.Text.Replace("'", "''")}', '{cmbStatus.SelectedItem}', {txtSubmittedBy.Text})";

            
            
            try
            {
                db.ExecuteNonQuery(query);
                MessageBox.Show($"Complaint Record Saved!\nComplaint ID: {txtComplaintID.Text}", "Success");
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
            txtComplainantName.Clear();
            txtPNR.Clear();
            cmbCategory.SelectedIndex = -1;
            txtDescription.Clear();
            cmbStatus.SelectedIndex = -1;
        }
    
        protected override void OnHandleCreated(System.EventArgs e) { base.OnHandleCreated(e); TMS.ThemeManager.ApplyTheme(this); }
    }
}
