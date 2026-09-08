using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TMS
{
    public class Form_Reg001_StationDiary : Form
    {
        private TextBox txtLogID;
        private DateTimePicker dtpEventTime;
        private ComboBox cmbCategory;
        private RichTextBox txtDescription;
        private TextBox txtReportedBy;
        private TextBox txtRemarks;
        private TextBox txtSubmittedBy;
        private DatabaseHelper db = new DatabaseHelper();
        

        public Form_Reg001_StationDiary()
        {
            this.Text = "Station Master's Diary (REG-001)";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.Size = new System.Drawing.Size(850, 700);
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.White;
            
            CreateControls();
            GenerateLogID();
        }

        private void CreateControls()
        {
            Panel headerPanel = new Panel();
            headerPanel.BackColor = System.Drawing.Color.FromArgb(0, 51, 102);
            headerPanel.Size = new System.Drawing.Size(850, 80);
            headerPanel.Dock = DockStyle.Top;
            this.Controls.Add(headerPanel);
            
            Label lblPath = new Label();
            lblPath.Text = "Home > Operational List > Station Master's Diary";
            lblPath.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Italic);
            lblPath.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);
            lblPath.Size = new System.Drawing.Size(800, 25);
            lblPath.Location = new System.Drawing.Point(25, 15);
            headerPanel.Controls.Add(lblPath);
            
            Label lblTitle = new Label();
            lblTitle.Text = "STATION MASTER'S DIARY";
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.White;
            lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblTitle.Size = new System.Drawing.Size(800, 35);
            lblTitle.Location = new System.Drawing.Point(25, 40);
            headerPanel.Controls.Add(lblTitle);
            
            int y = 110;
            
            Label lblLogID = new Label();
            lblLogID.Text = "Log ID (System Generated):";
            lblLogID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblLogID.Location = new System.Drawing.Point(30, y);
            lblLogID.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(lblLogID);
            
            txtLogID = new TextBox();
            txtLogID.Location = new System.Drawing.Point(240, y);
            txtLogID.Size = new System.Drawing.Size(300, 30);
            txtLogID.ReadOnly = true;
            txtLogID.BackColor = System.Drawing.Color.LightGray;
            this.Controls.Add(txtLogID);
            
            y += 50;
            
            Label lblEventTime = new Label();
            lblEventTime.Text = "Event Time *";
            lblEventTime.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblEventTime.Location = new System.Drawing.Point(30, y);
            lblEventTime.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblEventTime);
            
            dtpEventTime = new DateTimePicker();
            dtpEventTime.Location = new System.Drawing.Point(160, y);
            dtpEventTime.Size = new System.Drawing.Size(220, 30);
            dtpEventTime.Format = DateTimePickerFormat.Custom;
            dtpEventTime.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            this.Controls.Add(dtpEventTime);
            
            y += 50;
            
            Label lblCategory = new Label();
            lblCategory.Text = "Category *";
            lblCategory.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblCategory.Location = new System.Drawing.Point(30, y);
            lblCategory.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblCategory);
            
            cmbCategory = new ComboBox();
            cmbCategory.Location = new System.Drawing.Point(160, y);
            cmbCategory.Size = new System.Drawing.Size(220, 30);
            cmbCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCategory.Items.AddRange(new string[] { "Administrative", "Operational", "Commercial", "Safety", "Maintenance", "Emergency", "Others" });
            this.Controls.Add(cmbCategory);
            
            y += 80;
            
            Label lblDescription = new Label();
            lblDescription.Text = "Description *";
            lblDescription.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblDescription.Location = new System.Drawing.Point(30, y);
            lblDescription.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblDescription);
            
            txtDescription = new RichTextBox();
            txtDescription.Location = new System.Drawing.Point(160, y);
            txtDescription.Size = new System.Drawing.Size(630, 120);
            this.Controls.Add(txtDescription);
            
            y += 140;
            
            Label lblReportedBy = new Label();
            lblReportedBy.Text = "Reported By *";
            lblReportedBy.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblReportedBy.Location = new System.Drawing.Point(30, y);
            lblReportedBy.Size = new System.Drawing.Size(220, 30);
            this.Controls.Add(lblReportedBy);
            
            txtReportedBy = new TextBox();
            txtReportedBy.Location = new System.Drawing.Point(260, y);
            txtReportedBy.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(txtReportedBy);
            
            y += 50;
            
            Label lblRemarks = new Label();
            lblRemarks.Text = "Remarks";
            lblRemarks.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblRemarks.Location = new System.Drawing.Point(30, y);
            lblRemarks.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblRemarks);
            
            txtRemarks = new TextBox();
            txtRemarks.Location = new System.Drawing.Point(160, y);
            txtRemarks.Size = new System.Drawing.Size(630, 30);
            this.Controls.Add(txtRemarks);
            
            y += 60;

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
            btnView.Click += (s, e) => new ViewRecordsForm("Reg001_StationDiary", "Station Master's Diary").ShowDialog();
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
        
        private void GenerateLogID()
        {
            string datePart = DateTime.Now.ToString("yyyyMMdd");
            string query = $"SELECT COUNT(*) FROM Reg001_StationDiary WHERE LogID LIKE 'TMS-REG-001-{datePart}-%'";
            int count = Convert.ToInt32(db.ExecuteScalar(query));
            txtLogID.Text = $"TMS-REG-001-{datePart}-{(count + 1).ToString("D3")}";
        }
        
        
            private void BtnSave_Click(object sender, EventArgs e)
        {
        
        
            if (string.IsNullOrWhiteSpace(txtSubmittedBy.Text) || !int.TryParse(txtSubmittedBy.Text.Trim(), out _))
            {
                MessageBox.Show("Submitted By must be a valid numeric Staff ID.", "Validation Error");
                return;
            }
            if (!ValidationHelper.IsSelected(cmbCategory, "Category")) return;
            if (!ValidationHelper.IsNotEmpty(txtDescription.Text, "Description")) return;
            if (!ValidationHelper.IsNotEmpty(txtReportedBy.Text, "Reported By")) return;
            
            string query = $@"
                INSERT INTO Reg001_StationDiary (LogID, EventTime, Category, Description, ReportedBy, Remarks, SubmittedBy)
                VALUES ('{txtLogID.Text}', '{dtpEventTime.Value:yyyy-MM-dd HH:mm:ss.fff}', '{cmbCategory.SelectedItem}', 
                        '{txtDescription.Text.Replace("'", "''")}', '{txtReportedBy.Text}', '{txtRemarks.Text.Replace("'", "''")}', {txtSubmittedBy.Text})";

            
            
            try
            {
                db.ExecuteNonQuery(query);
                MessageBox.Show($"Record Saved!\nLog ID: {txtLogID.Text}", "Success");
                ClearForm();
                GenerateLogID();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error");
            }
        }
        
        
            private void ClearForm()
        {
        
        
            if (txtSubmittedBy != null) txtSubmittedBy.Clear();
            dtpEventTime.Value = DateTime.Now;
            cmbCategory.SelectedIndex = -1;
            txtDescription.Clear();
            txtReportedBy.Clear();
            txtRemarks.Clear();
        }
    
        protected override void OnHandleCreated(System.EventArgs e) { base.OnHandleCreated(e); TMS.ThemeManager.ApplyTheme(this); }
    }
}
