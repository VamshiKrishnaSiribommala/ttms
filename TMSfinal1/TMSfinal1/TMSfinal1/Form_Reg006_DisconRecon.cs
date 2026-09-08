using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace TMS
{
    public class Form_Reg006_DisconRecon : Form
    {
        private TextBox txtMemoID;
        private DateTimePicker dtpDisconnectionTime;
        private TextBox txtGearID;
        private TextBox txtMaintainerID;
        private DateTimePicker dtpReconnectionTime;
        private TextBox txtSubmittedBy;
        private DatabaseHelper db = new DatabaseHelper();
        

        public Form_Reg006_DisconRecon()
        {
            this.Text = "S&T Disconnection/Reconnection (REG-006)";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.Size = new System.Drawing.Size(750, 600);
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.White;
            
            CreateControls();
            GenerateMemoID();
        }

        private void CreateControls()
        {
            Panel headerPanel = new Panel();
            headerPanel.BackColor = System.Drawing.Color.FromArgb(0, 51, 102);
            headerPanel.Size = new System.Drawing.Size(750, 80);
            headerPanel.Dock = DockStyle.Top;
            this.Controls.Add(headerPanel);
            
            Label lblPath = new Label();
            lblPath.Text = "Home > Operational List > S&T Disconnection/Reconnection";
            lblPath.Font = new System.Drawing.Font("Segoe UI", 9, System.Drawing.FontStyle.Italic);
            lblPath.ForeColor = System.Drawing.Color.FromArgb(200, 200, 200);
            lblPath.Size = new System.Drawing.Size(700, 25);
            lblPath.Location = new System.Drawing.Point(25, 15);
            headerPanel.Controls.Add(lblPath);
            
            Label lblTitle = new Label();
            lblTitle.Text = "S&T DISCONNECTION/RECONNECTION REGISTER";
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 14, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.White;
            lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            lblTitle.Size = new System.Drawing.Size(700, 35);
            lblTitle.Location = new System.Drawing.Point(25, 40);
            headerPanel.Controls.Add(lblTitle);
            
            int y = 110;
            
            Label lblMemoID = new Label();
            lblMemoID.Text = "Memo ID (System Generated):";
            lblMemoID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblMemoID.Location = new System.Drawing.Point(30, y);
            lblMemoID.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(lblMemoID);
            
            txtMemoID = new TextBox();
            txtMemoID.Location = new System.Drawing.Point(240, y);
            txtMemoID.Size = new System.Drawing.Size(300, 30);
            txtMemoID.ReadOnly = true;
            txtMemoID.BackColor = System.Drawing.Color.LightGray;
            this.Controls.Add(txtMemoID);
            
            y += 50;
            
            Label lblDisconnectionTime = new Label();
            lblDisconnectionTime.Text = "Disconnection Time *";
            lblDisconnectionTime.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblDisconnectionTime.Location = new System.Drawing.Point(30, y);
            lblDisconnectionTime.Size = new System.Drawing.Size(150, 30);
            this.Controls.Add(lblDisconnectionTime);
            
            dtpDisconnectionTime = new DateTimePicker();
            dtpDisconnectionTime.Location = new System.Drawing.Point(190, y);
            dtpDisconnectionTime.Size = new System.Drawing.Size(200, 30);
            dtpDisconnectionTime.Format = DateTimePickerFormat.Custom;
            dtpDisconnectionTime.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            this.Controls.Add(dtpDisconnectionTime);
            
            y += 50;
            
            Label lblGearID = new Label();
            lblGearID.Text = "Gear ID *";
            lblGearID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblGearID.Location = new System.Drawing.Point(30, y);
            lblGearID.Size = new System.Drawing.Size(100, 30);
            this.Controls.Add(lblGearID);
            
            txtGearID = new TextBox();
            txtGearID.Location = new System.Drawing.Point(140, y);
            txtGearID.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(txtGearID);
            
            y += 50;
            
            Label lblMaintainerID = new Label();
            lblMaintainerID.Text = "Maintainer ID *";
            lblMaintainerID.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblMaintainerID.Location = new System.Drawing.Point(30, y);
            lblMaintainerID.Size = new System.Drawing.Size(120, 30);
            this.Controls.Add(lblMaintainerID);
            
            txtMaintainerID = new TextBox();
            txtMaintainerID.Location = new System.Drawing.Point(160, y);
            txtMaintainerID.Size = new System.Drawing.Size(200, 30);
            this.Controls.Add(txtMaintainerID);
            
            y += 50;
            
            Label lblReconnectionTime = new Label();
            lblReconnectionTime.Text = "Reconnection Time *";
            lblReconnectionTime.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Bold);
            lblReconnectionTime.Location = new System.Drawing.Point(30, y);
            lblReconnectionTime.Size = new System.Drawing.Size(150, 30);
            this.Controls.Add(lblReconnectionTime);
            
            dtpReconnectionTime = new DateTimePicker();
            dtpReconnectionTime.Location = new System.Drawing.Point(190, y);
            dtpReconnectionTime.Size = new System.Drawing.Size(200, 30);
            dtpReconnectionTime.Format = DateTimePickerFormat.Custom;
            dtpReconnectionTime.CustomFormat = "dd/MM/yyyy HH:mm:ss";
            this.Controls.Add(dtpReconnectionTime);
            
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
            btnView.Click += (s, e) => new ViewRecordsForm("Reg006_DisconRecon", "Disconnection Records").ShowDialog();
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
        
        private void GenerateMemoID()
        {
            string datePart = DateTime.Now.ToString("yyyyMMdd");
            string query = $"SELECT COUNT(*) FROM Reg006_DisconRecon WHERE MemoID LIKE 'TMS-REG-006-{datePart}-%'";
            int count = Convert.ToInt32(db.ExecuteScalar(query));
            txtMemoID.Text = $"TMS-REG-006-{datePart}-{(count + 1).ToString("D3")}";
        }
        
        
            private void BtnSave_Click(object sender, EventArgs e)
        {
        
        
            if (string.IsNullOrWhiteSpace(txtSubmittedBy.Text) || !int.TryParse(txtSubmittedBy.Text.Trim(), out _))
            {
                MessageBox.Show("Submitted By must be a valid numeric Staff ID.", "Validation Error");
                return;
            }
            if (!ValidationHelper.IsNotEmpty(txtGearID.Text, "Gear ID")) return;
            if (!ValidationHelper.IsNotEmpty(txtMaintainerID.Text, "Maintainer ID")) return;
            if (!ValidationHelper.IsEndAfterStart(dtpDisconnectionTime.Value, dtpReconnectionTime.Value, "Disconnection Time", "Reconnection Time")) return;
            
            string query = $@"
                INSERT INTO Reg006_DisconRecon (MemoID, DisconnectionTime, GearID, MaintainerID, ReconnectionTime, SubmittedBy)
                VALUES ('{txtMemoID.Text}', '{dtpDisconnectionTime.Value:yyyy-MM-dd HH:mm:ss.fff}', '{txtGearID.Text}', 
                        '{txtMaintainerID.Text}', '{dtpReconnectionTime.Value:yyyy-MM-dd HH:mm:ss.fff}', {txtSubmittedBy.Text})";

            
            
            try
            {
                db.ExecuteNonQuery(query);
                MessageBox.Show($"Disconnection Record Saved!\nMemo ID: {txtMemoID.Text}", "Success");
                ClearForm();
                GenerateMemoID();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Database Error");
            }
        }
        
        
            private void ClearForm()
        {
        
        
            if (txtSubmittedBy != null) txtSubmittedBy.Clear();
            dtpDisconnectionTime.Value = DateTime.Now;
            txtGearID.Clear();
            txtMaintainerID.Clear();
            dtpReconnectionTime.Value = DateTime.Now.AddHours(2);
        }
    
        protected override void OnHandleCreated(System.EventArgs e) { base.OnHandleCreated(e); TMS.ThemeManager.ApplyTheme(this); }
    }
}
