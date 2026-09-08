using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TMS
{
    public class RegisterSelectionForm : Form
    {
        private int moduleId;
        private string moduleName;
        private ListBox listBox;
        private Form parentForm;
        private bool isNavigatingBack = false;
        
        public RegisterSelectionForm(int moduleId, string moduleName, Form parent = null)
        {
            this.moduleId = moduleId;
            this.moduleName = moduleName;
            this.parentForm = parent;
            this.Text = moduleName + " - Select Register";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.Size = new System.Drawing.Size(750, 700);
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.White;
            
            // Enable double buffering
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            this.DoubleBuffered = true;

            CreateControls();
            LoadRegisters();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // WS_EX_COMPOSITED: Off-screen composite rendering to eliminate all form transition flicker
                return cp;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
        }

        private void NavigateBackToMain()
        {
            if (isNavigatingBack) return;
            isNavigatingBack = true;

            if (parentForm != null && !parentForm.IsDisposed)
            {
                parentForm.SuspendLayout();
                parentForm.Show();
                parentForm.BringToFront();
                parentForm.ResumeLayout(true);
                parentForm.Update();
            }
            else
            {
                var main = Application.OpenForms.Cast<Form>().OfType<MainClassesForm>().FirstOrDefault();
                if (main != null && !main.IsDisposed)
                {
                    main.SuspendLayout();
                    main.Show();
                    main.BringToFront();
                    main.ResumeLayout(true);
                    main.Update();
                }
                else
                {
                    new MainClassesForm().Show();
                }
            }

            this.BeginInvoke(new Action(() => this.Close()));
        }
        
        private void CreateControls()
        {
            Panel headerPanel = new Panel();
            headerPanel.BackColor = System.Drawing.Color.FromArgb(33, 61, 119);
            headerPanel.Size = new System.Drawing.Size(750, 80);
            headerPanel.Dock = DockStyle.Top;
            this.Controls.Add(headerPanel);
            
            Label lblPath = new Label();
            lblPath.Text = "Home > " + moduleName;
            lblPath.Font = new System.Drawing.Font("Segoe UI", 10, System.Drawing.FontStyle.Italic);
            lblPath.ForeColor = System.Drawing.Color.FromArgb(200, 225, 255);
            lblPath.Size = new System.Drawing.Size(700, 25);
            lblPath.Location = new System.Drawing.Point(25, 15);
            headerPanel.Controls.Add(lblPath);
            
            Label title = new Label();
            title.Text = moduleName + " Registers";
            title.Font = new System.Drawing.Font("Segoe UI", 16, System.Drawing.FontStyle.Bold);
            title.ForeColor = System.Drawing.Color.White;
            title.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            title.Size = new System.Drawing.Size(700, 35);
            title.Location = new System.Drawing.Point(25, 40);
            headerPanel.Controls.Add(title);
            
            listBox = new ListBox();
            listBox.Size = new System.Drawing.Size(650, 450);
            listBox.Location = new System.Drawing.Point(50, 110);
            listBox.Font = new System.Drawing.Font("Segoe UI", 11);
            listBox.BackColor = System.Drawing.Color.White;
            listBox.Cursor = Cursors.Hand;
            listBox.MouseDoubleClick += (s, e) => {
                if (listBox.IndexFromPoint(e.Location) != ListBox.NoMatches)
                {
                    BtnOpen_Click(s, e);
                }
            };
            listBox.KeyDown += (s, e) => {
                if (e.KeyCode == Keys.Enter)
                {
                    e.Handled = true;
                    BtnOpen_Click(s, e);
                }
            };
            this.Controls.Add(listBox);
            
            Button btnOpen = new Button();
            btnOpen.Text = "OPEN REGISTER";
            btnOpen.Size = new System.Drawing.Size(200, 50);
            btnOpen.Location = new System.Drawing.Point(275, 580);
            btnOpen.BackColor = System.Drawing.Color.FromArgb(251, 121, 43); // IRCTC Orange
            btnOpen.ForeColor = System.Drawing.Color.White;
            btnOpen.Font = new System.Drawing.Font("Segoe UI", 11, System.Drawing.FontStyle.Bold);
            btnOpen.FlatStyle = FlatStyle.Flat;
            btnOpen.Cursor = Cursors.Hand;
            btnOpen.Click += BtnOpen_Click;
            this.Controls.Add(btnOpen);
            
            Button btnBack = new Button();
            btnBack.Text = "BACK TO MAIN";
            btnBack.Size = new System.Drawing.Size(200, 50);
            btnBack.Location = new System.Drawing.Point(275, 640);
            btnBack.BackColor = System.Drawing.Color.FromArgb(220, 38, 38);
            btnBack.ForeColor = System.Drawing.Color.White;
            btnBack.Font = new System.Drawing.Font("Segoe UI", 11, System.Drawing.FontStyle.Bold);
            btnBack.FlatStyle = FlatStyle.Flat;
            btnBack.Cursor = Cursors.Hand;
            btnBack.Click += (s, e) => NavigateBackToMain();
            this.Controls.Add(btnBack);
        }
        
        private void LoadRegisters()
        {
            listBox.Items.Clear();
            
            if (moduleId == 1)
            {
                for (int i = 1; i <= 14; i++)
                {
                    string name = GetRegisterName(i);
                    listBox.Items.Add($"{i:D3} - {name}");
                }
            }
            else if (moduleId == 2)
            {
                int[] ids = { 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 31, 35, 40 };
                foreach (int id in ids)
                {
                    string name = GetRegisterName(id);
                    listBox.Items.Add($"{id:D3} - {name}");
                }
            }
            else if (moduleId == 3)
            {
                int[] ids = { 25, 26, 27, 28, 29, 30 };
                foreach (int id in ids)
                {
                    string name = GetRegisterName(id);
                    listBox.Items.Add($"{id:D3} - {name}");
                }
            }
            else if (moduleId == 4)
            {
                int[] ids = { 32, 33, 34, 36, 37, 38, 39 };
                foreach (int id in ids)
                {
                    string name = GetRegisterName(id);
                    listBox.Items.Add($"{id:D3} - {name}");
                }
            }
            else if (moduleId == 5)
            {
                listBox.Items.Add("041 - Dynamic Reports");
            }
        }
        
        private string GetRegisterName(int id)
        {
            switch (id)
            {
                case 1: return "Station Master's Diary";
                case 2: return "Train Signal Register";
                case 3: return "SWR Acknowledgment";
                case 4: return "Caution Order";
                case 5: return "Signal/Point/Block Failure";
                case 6: return "S&T Disconnection/Reconnection";
                case 7: return "Bio-Metric Attendance";
                case 8: return "Stable Load";
                case 9: return "Fog Signalman";
                case 10: return "Night Inspection";
                case 11: return "Public Complaint";
                case 12: return "Staff Grievance";
                case 13: return "Inspection & Observation";
                case 14: return "Miscellaneous Counter";
                case 15: return "Siding Key";
                case 16: return "Crank Handle";
                case 17: return "Crank Handle Testing";
                case 18: return "Cross-Over Testing";
                case 19: return "Signal Failure";
                case 20: return "Emergency Key";
                case 21: return "Complete Arrival";
                case 22: return "Control Instruction";
                case 23: return "SM Relief Diary";
                case 24: return "Traffic/Power Block";
                case 25: return "Safety Meeting";
                case 26: return "HQ Safety Circular";
                case 27: return "Safety Meeting (Part 2)";
                case 28: return "Staff Biodata";
                case 29: return "Assurance";
                case 30: return "Private Number Sheet";
                case 31: return "Petty Repair";
                case 32: return "Attendance";
                case 33: return "Passenger Complaint";
                case 34: return "Employee Complaint";
                case 35: return "Power Supply";
                case 36: return "Officers Inspection";
                case 37: return "TI Inspection";
                case 38: return "Joint Inspection";
                case 39: return "Night Inspection";
                case 40: return "Failure Inspection";
                case 41: return "Dynamic Reports";
                default: return "Unknown";
            }
        }
        
        private bool isOpening = false;
        
        private void BtnOpen_Click(object sender, EventArgs e)
        {
            if (isOpening) return;
            if (listBox.SelectedItem == null)
            {
                MessageBox.Show("Please select a register to open", "Selection Required");
                return;
            }
            
            try
            {
                isOpening = true;
                string selected = listBox.SelectedItem.ToString();
                string regId = selected.Substring(0, 3);
                Form registerForm = null;
                
                switch (regId)
                {
                    case "001": registerForm = new Form_Reg001_StationDiary(); break;
                    case "002": registerForm = new Form_Reg002_TrainSignal(); break;
                    case "003": registerForm = new Form_Reg003_SWR(); break;
                    case "004": registerForm = new Form_Reg004_CautionOrder(); break;
                    case "005": registerForm = new Form_Reg005_Failure(); break;
                    case "006": registerForm = new Form_Reg006_DisconRecon(); break;
                    case "007": registerForm = new Form_Reg007_Attendance(); break;
                    case "008": registerForm = new Form_Reg008_StableLoad(); break;
                    case "009": registerForm = new Form_Reg009_FogSignalman(); break;
                    case "010": registerForm = new Form_Reg010_NightInspection(); break;
                    case "011": registerForm = new Form_Reg011_PublicComplaint(); break;
                    case "012": registerForm = new Form_Reg012_StaffGrievance(); break;
                    case "013": registerForm = new Form_Reg013_Inspection(); break;
                    case "014": registerForm = new Form_Reg014_MiscCounter(); break;
                    case "015": registerForm = new Form_Reg015_SidingKey(); break;
                    case "016": registerForm = new Form_Reg016_CrankHandle(); break;
                    case "017": registerForm = new Form_Reg017_CrankHandleTest(); break;
                    case "018": registerForm = new Form_Reg018_CrossoverTest(); break;
                    case "019": registerForm = new Form_Reg019_SignalFailure(); break;
                    case "020": registerForm = new Form_Reg020_EmergencyKey(); break;
                    case "021": registerForm = new Form_Reg021_CompleteArrival(); break;
                    case "022": registerForm = new Form_Reg022_ControlInstruction(); break;
                    case "023": registerForm = new Form_Reg023_SMRelief(); break;
                    case "024": registerForm = new Form_Reg024_TrafficBlock(); break;
                    case "025": registerForm = new Form_Reg025_SafetyMeeting(); break;
                    case "026": registerForm = new Form_Reg026_SafetyCircular(); break;
                    case "027": registerForm = new Form_Reg027_SafetyMeeting2(); break;
                    case "028": registerForm = new Form_Reg028_StaffBiodata(); break;
                    case "029": registerForm = new Form_Reg029_Assurance(); break;
                    case "030": registerForm = new Form_Reg030_PNSheet(); break;
                    case "031": registerForm = new Form_Reg031_PettyRepair(); break;
                    case "032": registerForm = new Form_Reg032_Attendance(); break;
                    case "033": registerForm = new Form_Reg033_PassengerComplaint(); break;
                    case "034": registerForm = new Form_Reg034_EmployeeComplaint(); break;
                    case "035": registerForm = new Form_Reg035_PowerSupply(); break;
                    case "036": registerForm = new Form_Reg036_OfficersInspection(); break;
                    case "037": registerForm = new Form_Reg037_TIInspection(); break;
                    case "038": registerForm = new Form_Reg038_JointInspection(); break;
                    case "039": registerForm = new Form_Reg039_NightInspection(); break;
                    case "040": registerForm = new Form_Reg040_FailureInspection(); break;
                    case "041": registerForm = new Form_Reg041_DynamicReports(); break;
                    default: MessageBox.Show($"Form for {selected} is coming soon!"); return;
                }
                
                if (registerForm != null) registerForm.ShowDialog();
            }
            finally
            {
                isOpening = false;
            }
        }
    
        protected override void OnHandleCreated(System.EventArgs e) { base.OnHandleCreated(e); TMS.ThemeManager.ApplyTheme(this); }
    }
}