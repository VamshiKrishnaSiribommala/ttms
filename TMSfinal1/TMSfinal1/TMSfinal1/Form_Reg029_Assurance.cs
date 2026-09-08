using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TMS
{
    public class Form_Reg029_Assurance : Form
    {
        private TextBox txtAssuranceID;
        private ComboBox cmbDocumentType;
        private TextBox txtDocumentID;
        private TextBox txtDocumentVersion;
        private TextBox txtRegisterPart;
        private DateTimePicker dtpEffectiveDate;
        private ComboBox cmbLanguageSelection;
        private CheckBox chkReadConfirmation;
        private TextBox txtSubmittedBy;
        private DatabaseHelper db = new DatabaseHelper();

        public Form_Reg029_Assurance()
        {
            this.Text = "Assurance (REG-029)";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.Size = new System.Drawing.Size(750, 700);
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.White;

            CreateControls();
            GenerateAssuranceID();
        }

        private void CreateControls()
        {
            Panel headerPanel = new Panel();
            headerPanel.BackColor = System.Drawing.Color.FromArgb(0, 51, 102);
            headerPanel.Size = new System.Drawing.Size(750, 80);
            headerPanel.Dock = DockStyle.Top;
            this.Controls.Add(headerPanel);

            Label lblPath = new Label();
            lblPath.Text = "Home > Infrastructure Sub > Assurance";
            lblPath.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Italic);
            lblPath.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);
            lblPath.Size = new System.Drawing.Size(700, 25);
            lblPath.Location = new System.Drawing.Point(25, 15);
            headerPanel.Controls.Add(lblPath);

            Label lblTitle = new Label();
            lblTitle.Text = "ASSURANCE REGISTER";
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.White;
            lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblTitle.Size = new System.Drawing.Size(700, 35);
            lblTitle.Location = new System.Drawing.Point(25, 40);
            headerPanel.Controls.Add(lblTitle);

            int y = 110;

            Label lblAssuranceID = new Label();
            lblAssuranceID.Text = "Assurance ID (System Generated):";
            lblAssuranceID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblAssuranceID.Location = new System.Drawing.Point(30, y);
            lblAssuranceID.Size = new System.Drawing.Size(220, 30);
            this.Controls.Add(lblAssuranceID);

            txtAssuranceID = new TextBox();
            txtAssuranceID.Location = new System.Drawing.Point(260, y);
            txtAssuranceID.Size = new System.Drawing.Size(300, 30);
            txtAssuranceID.ReadOnly = true;
            txtAssuranceID.BackColor = System.Drawing.Color.LightGray;
            this.Controls.Add(txtAssuranceID);

            y += 50;

            Label lblDocumentType = new Label();
            lblDocumentType.Text = "Document Type *";
            lblDocumentType.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblDocumentType.Location = new System.Drawing.Point(30, y);
            lblDocumentType.Size = new System.Drawing.Size(130, 30);
            this.Controls.Add(lblDocumentType);

            cmbDocumentType = new ComboBox();
            cmbDocumentType.Location = new System.Drawing.Point(170, y);
            cmbDocumentType.Size = new System.Drawing.Size(200, 30);
            cmbDocumentType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDocumentType.Items.AddRange(new string[] { "SWR", "Safety Circular", "Correction Slip", "GR", "SR", "SOP" });
            this.Controls.Add(cmbDocumentType);

            y += 50;

            Label lblDocumentID = new Label();
            lblDocumentID.Text = "Document ID *";
            lblDocumentID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblDocumentID.Location = new System.Drawing.Point(30, y);
            lblDocumentID.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblDocumentID);

            txtDocumentID = new TextBox();
            txtDocumentID.Location = new System.Drawing.Point(160, y);
            txtDocumentID.Size = new System.Drawing.Size(250, 30);
            this.Controls.Add(txtDocumentID);

            y += 50;

            Label lblDocumentVersion = new Label();
            lblDocumentVersion.Text = "Document Version *";
            lblDocumentVersion.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblDocumentVersion.Location = new System.Drawing.Point(30, y);
            lblDocumentVersion.Size = new System.Drawing.Size(140, 30);
            this.Controls.Add(lblDocumentVersion);

            txtDocumentVersion = new TextBox();
            txtDocumentVersion.Location = new System.Drawing.Point(180, y);
            txtDocumentVersion.Size = new System.Drawing.Size(150, 30);
            this.Controls.Add(txtDocumentVersion);

            y += 50;

            Label lblRegisterPart = new Label();
            lblRegisterPart.Text = "Register Part (1-6) *";
            lblRegisterPart.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblRegisterPart.Location = new System.Drawing.Point(30, y);
            lblRegisterPart.Size = new System.Drawing.Size(150, 30);
            this.Controls.Add(lblRegisterPart);

            txtRegisterPart = new TextBox();
            txtRegisterPart.Location = new System.Drawing.Point(190, y);
            txtRegisterPart.Size = new System.Drawing.Size(100, 30);
            this.Controls.Add(txtRegisterPart);

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
            dtpEffectiveDate.MaxDate = DateTime.Today;
            this.Controls.Add(dtpEffectiveDate);

            y += 50;

            Label lblLanguage = new Label();
            lblLanguage.Text = "Language Selection *";
            lblLanguage.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblLanguage.Location = new System.Drawing.Point(30, y);
            lblLanguage.Size = new System.Drawing.Size(150, 30);
            this.Controls.Add(lblLanguage);

            cmbLanguageSelection = new ComboBox();
            cmbLanguageSelection.Location = new System.Drawing.Point(190, y);
            cmbLanguageSelection.Size = new System.Drawing.Size(150, 30);
            cmbLanguageSelection.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbLanguageSelection.Items.AddRange(new string[] { "English", "Hindi" });
            this.Controls.Add(cmbLanguageSelection);

            y += 50;

            Label lblReadConfirmation = new Label();
            lblReadConfirmation.Text = "Read Confirmation *";
            lblReadConfirmation.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblReadConfirmation.Location = new System.Drawing.Point(30, y);
            lblReadConfirmation.Size = new System.Drawing.Size(150, 30);
            this.Controls.Add(lblReadConfirmation);

            chkReadConfirmation = new CheckBox();
            chkReadConfirmation.Text = "I have read and understood the above document";
            chkReadConfirmation.Location = new System.Drawing.Point(190, y);
            chkReadConfirmation.Size = new System.Drawing.Size(350, 30);
            this.Controls.Add(chkReadConfirmation);

            y += 60;

            Label lblSubmittedBy = new Label();
            lblSubmittedBy.Text = "Submitted By *";
            lblSubmittedBy.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblSubmittedBy.Location = new System.Drawing.Point(30, y);
            lblSubmittedBy.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblSubmittedBy);

            txtSubmittedBy = new TextBox();
            txtSubmittedBy.Location = new System.Drawing.Point(160, y);
            txtSubmittedBy.Size = new System.Drawing.Size(530, 30);
            this.Controls.Add(txtSubmittedBy);

            y += 60;

            Button btnSave = new Button();
            btnSave.Text = "SAVE";
            btnSave.Size = new System.Drawing.Size(150, 45);
            btnSave.Location = new System.Drawing.Point(100, y);
            btnSave.BackColor = System.Drawing.Color.FromArgb(46, 204, 113);
            btnSave.ForeColor = System.Drawing.Color.White;
            btnSave.Font = new System.Drawing.Font("Segoe UI", 11, System.Drawing.FontStyle.Bold);
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            Button btnView = new Button();
            btnView.Text = "VIEW RECORDS";
            btnView.Size = new System.Drawing.Size(150, 45);
            btnView.Location = new System.Drawing.Point(265, y);
            btnView.BackColor = System.Drawing.Color.FromArgb(52, 152, 219);
            btnView.ForeColor = System.Drawing.Color.White;
            btnView.Font = new System.Drawing.Font("Segoe UI", 11, System.Drawing.FontStyle.Bold);
            btnView.FlatStyle = FlatStyle.Flat;
            btnView.Click += (s, e) => new ViewRecordsForm("Reg029_Assurance", "Assurance Records").ShowDialog();
            this.Controls.Add(btnView);

            Button btnClear = new Button();
            btnClear.Text = "CLEAR";
            btnClear.Size = new System.Drawing.Size(120, 45);
            btnClear.Location = new System.Drawing.Point(430, y);
            btnClear.BackColor = System.Drawing.Color.FromArgb(241, 196, 15);
            btnClear.ForeColor = System.Drawing.Color.Black;
            btnClear.Font = new System.Drawing.Font("Segoe UI", 11, System.Drawing.FontStyle.Bold);
            btnClear.FlatStyle = FlatStyle.Flat;
            btnClear.Click += (s, e) => ClearForm();
            this.Controls.Add(btnClear);

            Button btnBack = new Button();
            btnBack.Text = "BACK";
            btnBack.Size = new System.Drawing.Size(100, 45);
            btnBack.Location = new System.Drawing.Point(565, y);
            btnBack.BackColor = System.Drawing.Color.FromArgb(231, 76, 60);
            btnBack.ForeColor = System.Drawing.Color.White;
            btnBack.Font = new System.Drawing.Font("Segoe UI", 11, System.Drawing.FontStyle.Bold);
            btnBack.FlatStyle = FlatStyle.Flat;
            btnBack.DialogResult = DialogResult.Cancel;
            btnBack.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            this.Controls.Add(btnBack);
        }

        private void GenerateAssuranceID()
        {
            string datePart = DateTime.Now.ToString("yyyyMMdd");
            string query = $"SELECT COUNT(*) FROM Reg029_Assurance WHERE AssuranceID LIKE 'TMS-REG-029-{datePart}-%'";
            int count = Convert.ToInt32(db.ExecuteScalar(query));
            txtAssuranceID.Text = $"TMS-REG-029-{datePart}-{(count + 1).ToString("D3")}";
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSubmittedBy.Text) || !int.TryParse(txtSubmittedBy.Text.Trim(), out _))
            {
                MessageBox.Show("Submitted By must be a valid numeric Staff ID.", "Validation Error");
                return;
            }
            if (!ValidationHelper.IsSelected(cmbDocumentType, "Document Type")) return;
            if (!ValidationHelper.IsNotEmpty(txtDocumentID.Text, "Document ID")) return;
            if (!ValidationHelper.IsNotEmpty(txtDocumentVersion.Text, "Document Version")) return;
            if (!ValidationHelper.IsNotEmpty(txtRegisterPart.Text, "Register Part")) return;
            if (!ValidationHelper.IsSelected(cmbLanguageSelection, "Language Selection")) return;
            if (!chkReadConfirmation.Checked)
            {
                MessageBox.Show("Please confirm that you have read and understood the document!", "Validation Error");
                return;
            }

            string query = $@"
                INSERT INTO Reg029_Assurance (AssuranceID, DocumentType, DocumentID, DocumentVersion, RegisterPart, EffectiveDate, LanguageSelection, ReadConfirmation, SubmittedBy)
                VALUES ('{txtAssuranceID.Text}', '{cmbDocumentType.SelectedItem}', '{txtDocumentID.Text}',
                        '{txtDocumentVersion.Text}', '{txtRegisterPart.Text}', '{dtpEffectiveDate.Value:yyyy-MM-dd}',
                        '{cmbLanguageSelection.SelectedItem}', {(chkReadConfirmation.Checked ? "1" : "0")}, {txtSubmittedBy.Text})";

            try
            {
                db.ExecuteNonQuery(query);
                MessageBox.Show($"Assurance Record Saved!\nAssurance ID: {txtAssuranceID.Text}", "Success");
                ClearForm();
                GenerateAssuranceID();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error");
            }
        }

        private void ClearForm()
        {
            if (txtSubmittedBy != null) txtSubmittedBy.Clear();
            cmbDocumentType.SelectedIndex = -1;
            txtDocumentID.Clear();
            txtDocumentVersion.Clear();
            txtRegisterPart.Clear();
            dtpEffectiveDate.Value = DateTime.Now;
            cmbLanguageSelection.SelectedIndex = -1;
            chkReadConfirmation.Checked = false;
        }

        protected override void OnLoad(System.EventArgs e) { base.OnLoad(e); TMS.ThemeManager.ApplyTheme(this); }
    }
}