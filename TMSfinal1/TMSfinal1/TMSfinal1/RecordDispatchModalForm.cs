using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TMS
{
    /// <summary>
    /// Clean, Professional 1-Click Record Dispatch Modal Form.
    /// Simplified, spacious, and extremely easy to understand.
    /// </summary>
    public class RecordDispatchModalForm : Form
    {
        private readonly string regCode;
        private readonly string regTitle;
        private readonly string logId;
        private readonly Dictionary<string, string> fieldData;
        private readonly string pdfPath;
        private string directPdfUrl = null;
        private Task<string> uploadTask = null;

        // UI Controls
        private Label lblPdfLinkStatus;
        private Button btnCopyPdfLink;
        private CheckedListBox chkListContacts;
        private TextBox txtNewPhone;
        private TextBox txtNewName;
        private TextBox txtNewDesignation;
        private Button btnAddNewContact;
        private Button btnRemoveSelected;
        private Button btnSelectAll;
        private Button btnDeselectAll;

        private Label lblSelectedSummary;
        private Button btnSendWhatsApp;
        private Button btnSendTelegram;
        private Button btnSendBoth;
        private Button btnCopyPdf;
        private Button btnViewPdf;
        private Button btnClose;

        public RecordDispatchModalForm(string regCode, string regTitle, string logId, Dictionary<string, string> fieldData, string pdfPath)
        {
            this.regCode = regCode;
            this.regTitle = regTitle;
            this.logId = logId;
            this.fieldData = fieldData;
            this.pdfPath = pdfPath;

            InitializeForm();
            StartAsyncPdfUpload();
        }

        private void StartAsyncPdfUpload()
        {
            uploadTask = Task.Run(() => RecordDispatchService.UploadPdfForDirectLink(pdfPath));
            uploadTask.ContinueWith(t =>
            {
                try
                {
                    if (t.IsCompleted && !string.IsNullOrEmpty(t.Result))
                    {
                        directPdfUrl = t.Result;
                        if (this.IsHandleCreated)
                        {
                            this.BeginInvoke(new Action(() =>
                            {
                                if (lblPdfLinkStatus != null)
                                {
                                    lblPdfLinkStatus.Text = "🔗 Direct PDF Link Ready: " + (directPdfUrl.Length > 48 ? directPdfUrl.Substring(0, 45) + "..." : directPdfUrl);
                                    lblPdfLinkStatus.ForeColor = Color.FromArgb(5, 150, 105);
                                }
                                if (btnCopyPdfLink != null)
                                {
                                    btnCopyPdfLink.Visible = true;
                                }
                            }));
                        }
                    }
                    else
                    {
                        if (this.IsHandleCreated)
                        {
                            this.BeginInvoke(new Action(() =>
                            {
                                if (lblPdfLinkStatus != null)
                                {
                                    lblPdfLinkStatus.Text = "📁 Local PDF Document Attached (Ready to paste via Ctrl+V)";
                                    lblPdfLinkStatus.ForeColor = Color.FromArgb(71, 85, 105);
                                }
                            }));
                        }
                    }
                }
                catch { }
            });
        }

        private string EnsureDirectPdfUrl()
        {
            if (!string.IsNullOrEmpty(directPdfUrl))
                return directPdfUrl;

            if (uploadTask != null)
            {
                try
                {
                    if (!uploadTask.IsCompleted)
                    {
                        Cursor.Current = Cursors.WaitCursor;
                        uploadTask.Wait(4000);
                    }
                    if (uploadTask.IsCompleted && !string.IsNullOrEmpty(uploadTask.Result))
                    {
                        directPdfUrl = uploadTask.Result;
                    }
                }
                catch { }
                finally
                {
                    Cursor.Current = Cursors.Default;
                }
            }

            if (string.IsNullOrEmpty(directPdfUrl))
            {
                try
                {
                    directPdfUrl = RecordDispatchService.UploadPdfForDirectLink(pdfPath);
                }
                catch { }
            }

            return directPdfUrl;
        }

        private void InitializeForm()
        {
            this.Text = "Send Record Memo";
            this.Size = new Size(680, 680);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.Font = new Font("Segoe UI", 9.5F);

            // ── 1. HEADER BANNER (Navy #1E3A8A) ───────────────────────────────────
            Panel pnlBanner = new Panel
            {
                Dock = DockStyle.Top,
                Height = 65,
                BackColor = Color.FromArgb(30, 58, 138)
            };
            Label lblBannerTitle = new Label
            {
                Text = "🚆 OFFICIAL RECORD DISPATCH",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 10),
                AutoSize = true
            };
            Label lblBannerSub = new Label
            {
                Text = $"{regTitle}  •  Ref: {logId}",
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(219, 234, 254),
                Location = new Point(20, 36),
                AutoSize = true
            };
            pnlBanner.Controls.AddRange(new Control[] { lblBannerTitle, lblBannerSub });
            this.Controls.Add(pnlBanner);

            // ── 2. CLEAN SUCCESS STATUS PILL ──────────────────────────────────────
            Panel pnlStatus = new Panel
            {
                Location = new Point(20, 74),
                Size = new Size(625, 54),
                BackColor = Color.FromArgb(240, 253, 244)
            };
            pnlStatus.Paint += (s, e) =>
            {
                using (Pen p = new Pen(Color.FromArgb(187, 247, 208), 1.2f))
                    e.Graphics.DrawRectangle(p, 0, 0, pnlStatus.Width - 1, pnlStatus.Height - 1);
            };
            Label lblStatusText = new Label
            {
                Text = $"✔ Record saved successfully. PDF Memo generated ({Path.GetFileName(pdfPath)}).",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(22, 101, 52),
                Location = new Point(12, 6),
                AutoSize = true
            };

            lblPdfLinkStatus = new Label
            {
                Text = "⏳ Generating secure online PDF link for instant viewing...",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Italic),
                ForeColor = Color.FromArgb(2, 132, 199),
                Location = new Point(12, 28),
                AutoSize = true
            };

            btnCopyPdfLink = new Button
            {
                Text = "📋 Copy Link",
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                BackColor = Color.FromArgb(224, 242, 254),
                ForeColor = Color.FromArgb(3, 105, 161),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(515, 23),
                Size = new Size(95, 24),
                Visible = false,
                Cursor = Cursors.Hand
            };
            btnCopyPdfLink.FlatAppearance.BorderColor = Color.FromArgb(186, 230, 253);
            btnCopyPdfLink.Click += (s, e) =>
            {
                if (!string.IsNullOrEmpty(directPdfUrl))
                {
                    Clipboard.SetText(directPdfUrl);
                    MessageBox.Show("✔ Direct PDF Link copied to clipboard!\n\n" + directPdfUrl, "Link Copied", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };

            pnlStatus.Controls.AddRange(new Control[] { lblStatusText, lblPdfLinkStatus, btnCopyPdfLink });
            this.Controls.Add(pnlStatus);

            // ── 3. RECIPIENT CONTACTS DIRECTORY ───────────────────────────────────
            GroupBox grpContacts = new GroupBox
            {
                Text = " Select Recipients to Send Memo ",
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(20, 136),
                Size = new Size(625, 215),
                BackColor = Color.White
            };

            chkListContacts = new CheckedListBox
            {
                Location = new Point(15, 28),
                Size = new Size(460, 175),
                Font = new Font("Segoe UI", 9.5F),
                CheckOnClick = true,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 250, 252)
            };
            chkListContacts.ItemCheck += (s, e) =>
            {
                if (this.IsHandleCreated)
                {
                    try { this.BeginInvoke(new Action(UpdateSelectedSummaryAndButtons)); } catch { }
                }
                else
                {
                    UpdateSelectedSummaryAndButtons();
                }
            };
            grpContacts.Controls.Add(chkListContacts);

            // Contact Tool Buttons (Select All, Clear, Remove)
            btnSelectAll = new Button
            {
                Text = "✔ Select All",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                BackColor = Color.FromArgb(241, 245, 249),
                ForeColor = Color.FromArgb(30, 41, 59),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(485, 28),
                Size = new Size(125, 30),
                Cursor = Cursors.Hand
            };
            btnSelectAll.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnSelectAll.Click += (s, e) =>
            {
                for (int i = 0; i < chkListContacts.Items.Count; i++)
                    chkListContacts.SetItemChecked(i, true);
                UpdateSelectedSummaryAndButtons();
            };

            btnDeselectAll = new Button
            {
                Text = "✖ Clear All",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                BackColor = Color.FromArgb(241, 245, 249),
                ForeColor = Color.FromArgb(30, 41, 59),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(485, 64),
                Size = new Size(125, 30),
                Cursor = Cursors.Hand
            };
            btnDeselectAll.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnDeselectAll.Click += (s, e) =>
            {
                for (int i = 0; i < chkListContacts.Items.Count; i++)
                    chkListContacts.SetItemChecked(i, false);
                UpdateSelectedSummaryAndButtons();
            };

            btnRemoveSelected = new Button
            {
                Text = "🗑 Remove",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                BackColor = Color.FromArgb(254, 242, 242),
                ForeColor = Color.FromArgb(220, 38, 38),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(485, 100),
                Size = new Size(125, 30),
                Cursor = Cursors.Hand
            };
            btnRemoveSelected.FlatAppearance.BorderColor = Color.FromArgb(254, 202, 202);
            btnRemoveSelected.Click += BtnRemoveSelected_Click;

            lblSelectedSummary = new Label
            {
                Text = "Selected: 0",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(37, 99, 235),
                Location = new Point(485, 140),
                Size = new Size(125, 45),
                TextAlign = ContentAlignment.TopLeft
            };

            grpContacts.Controls.AddRange(new Control[] { btnSelectAll, btnDeselectAll, btnRemoveSelected, lblSelectedSummary });
            this.Controls.Add(grpContacts);

            // ── 4. QUICK ADD NEW CONTACT (Clean Row) ──────────────────────────────
            GroupBox grpAdd = new GroupBox
            {
                Text = " Add New Contact ",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(71, 85, 105),
                Location = new Point(20, 358),
                Size = new Size(625, 75),
                BackColor = Color.White
            };

            Label lblP = new Label { Text = "Mobile Number:", Font = new Font("Segoe UI", 8F), Location = new Point(15, 18), AutoSize = true };
            txtNewPhone = new TextBox { Font = new Font("Segoe UI", 9F), Location = new Point(15, 38), Size = new Size(140, 24) };

            Label lblN = new Label { Text = "Name:", Font = new Font("Segoe UI", 8F), Location = new Point(165, 18), AutoSize = true };
            txtNewName = new TextBox { Font = new Font("Segoe UI", 9F), Location = new Point(165, 38), Size = new Size(150, 24) };

            Label lblD = new Label { Text = "Designation:", Font = new Font("Segoe UI", 8F), Location = new Point(325, 18), AutoSize = true };
            txtNewDesignation = new TextBox { Font = new Font("Segoe UI", 9F), Location = new Point(325, 38), Size = new Size(150, 24) };

            btnAddNewContact = new Button
            {
                Text = "➕ Save",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                BackColor = Color.FromArgb(241, 245, 249),
                ForeColor = Color.FromArgb(30, 58, 138),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(485, 34),
                Size = new Size(125, 30),
                Cursor = Cursors.Hand
            };
            btnAddNewContact.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnAddNewContact.Click += BtnAddNewContact_Click;

            grpAdd.Controls.AddRange(new Control[] { lblP, txtNewPhone, lblN, txtNewName, lblD, txtNewDesignation, btnAddNewContact });
            this.Controls.Add(grpAdd);

            // ── 5. PROMINENT 1-CLICK DISPATCH BUTTONS ──────────────────────────────
            Panel pnlActions = new Panel
            {
                Location = new Point(20, 440),
                Size = new Size(625, 160),
                BackColor = Color.White
            };
            pnlActions.Paint += (s, e) =>
            {
                using (Pen p = new Pen(Color.FromArgb(226, 232, 240), 1.2f))
                    e.Graphics.DrawRectangle(p, 0, 0, pnlActions.Width - 1, pnlActions.Height - 1);
            };

            // Main Action 1: WhatsApp
            btnSendWhatsApp = new Button
            {
                Text = "🟢  Send via WhatsApp",
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                BackColor = Color.FromArgb(37, 211, 102),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(15, 14),
                Size = new Size(290, 42),
                Cursor = Cursors.Hand
            };
            btnSendWhatsApp.FlatAppearance.BorderSize = 0;
            btnSendWhatsApp.Click += BtnSendWhatsApp_Click;

            // Main Action 2: Telegram
            btnSendTelegram = new Button
            {
                Text = "🔵  Send via Telegram",
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 136, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(315, 14),
                Size = new Size(295, 42),
                Cursor = Cursors.Hand
            };
            btnSendTelegram.FlatAppearance.BorderSize = 0;
            btnSendTelegram.Click += BtnSendTelegram_Click;

            // Main Action 3: Both
            btnSendBoth = new Button
            {
                Text = "🚀  Send to Both (WhatsApp + Telegram)",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(109, 40, 217),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(15, 62),
                Size = new Size(595, 38),
                Cursor = Cursors.Hand
            };
            btnSendBoth.FlatAppearance.BorderSize = 0;
            btnSendBoth.Click += BtnSendBoth_Click;

            // Utility Row: Copy PDF, View PDF, Close
            btnCopyPdf = new Button
            {
                Text = "📎 Copy PDF File (Ctrl+V)",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                BackColor = Color.FromArgb(238, 242, 255),
                ForeColor = Color.FromArgb(67, 56, 202),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(15, 108),
                Size = new Size(220, 34),
                Cursor = Cursors.Hand
            };
            btnCopyPdf.FlatAppearance.BorderColor = Color.FromArgb(199, 210, 254);
            btnCopyPdf.Click += (s, e) =>
            {
                RecordDispatchService.CopyPdfFileToClipboard(pdfPath);
                MessageBox.Show("✔ PDF Document copied to Clipboard!\nPress Ctrl + V in any chat window to paste the PDF.", "Copied", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            btnViewPdf = new Button
            {
                Text = "📄 View PDF",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                BackColor = Color.FromArgb(241, 245, 249),
                ForeColor = Color.FromArgb(51, 65, 85),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(245, 108),
                Size = new Size(160, 34),
                Cursor = Cursors.Hand
            };
            btnViewPdf.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnViewPdf.Click += (s, e) => RecordDispatchService.OpenPdfFile(pdfPath);

            btnClose = new Button
            {
                Text = "✖ Close",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                BackColor = Color.FromArgb(241, 245, 249),
                ForeColor = Color.FromArgb(100, 116, 139),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(415, 108),
                Size = new Size(195, 34),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
            btnClose.Click += (s, e) => this.Close();

            pnlActions.Controls.AddRange(new Control[] {
                btnSendWhatsApp, btnSendTelegram, btnSendBoth,
                btnCopyPdf, btnViewPdf, btnClose
            });
            this.Controls.Add(pnlActions);

            this.Load += (s, e) => LoadContactsList();
        }

        private void LoadContactsList()
        {
            var contacts = RecordDispatchService.LoadSavedContacts();
            chkListContacts.Items.Clear();

            foreach (var c in contacts)
            {
                chkListContacts.Items.Add(c, false);
            }

            if (chkListContacts.Items.Count > 0)
            {
                chkListContacts.SetItemChecked(0, true);
            }

            UpdateSelectedSummaryAndButtons();
        }

        private void UpdateSelectedSummaryAndButtons()
        {
            var selected = GetSelectedContacts();
            int count = selected.Count;

            lblSelectedSummary.Text = count == 1
                ? "Selected: 1"
                : $"Selected: {count}";

            lblSelectedSummary.ForeColor = count > 0 ? Color.FromArgb(5, 150, 105) : Color.FromArgb(220, 38, 38);

            if (count > 1)
            {
                btnSendWhatsApp.Text = $"🟢  Send to All ({count}) via WhatsApp";
                btnSendTelegram.Text = $"🔵  Send to All ({count}) via Telegram";
                btnSendBoth.Text = $"🚀  Send to All ({count}) (WhatsApp + Telegram)";
            }
            else if (count == 1)
            {
                btnSendWhatsApp.Text = "🟢  Send via WhatsApp";
                btnSendTelegram.Text = "🔵  Send via Telegram";
                btnSendBoth.Text = "🚀  Send via Both (WhatsApp + Telegram)";
            }
            else
            {
                btnSendWhatsApp.Text = "🟢  Send via WhatsApp";
                btnSendTelegram.Text = "🔵  Send via Telegram";
                btnSendBoth.Text = "🚀  Send via Both";
            }
        }

        private List<RecordDispatchService.DispatchContact> GetSelectedContacts()
        {
            List<RecordDispatchService.DispatchContact> list = new List<RecordDispatchService.DispatchContact>();
            foreach (var item in chkListContacts.CheckedItems)
            {
                if (item is RecordDispatchService.DispatchContact c)
                {
                    list.Add(c);
                }
            }
            return list;
        }

        private void BtnAddNewContact_Click(object sender, EventArgs e)
        {
            string phone = txtNewPhone.Text.Trim();
            string name = txtNewName.Text.Trim();
            string des = txtNewDesignation.Text.Trim();

            if (string.IsNullOrWhiteSpace(phone))
            {
                MessageBox.Show("Please enter a Mobile Number.", "Mobile Number Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNewPhone.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                name = "Railway Staff (" + phone + ")";
            }

            RecordDispatchService.AddContact(name, des, phone, "");
            txtNewPhone.Clear();
            txtNewName.Clear();
            txtNewDesignation.Clear();

            LoadContactsList();
            MessageBox.Show($"Contact '{name}' saved successfully!", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnRemoveSelected_Click(object sender, EventArgs e)
        {
            var checkedItems = GetSelectedContacts();
            if (checkedItems.Count == 0)
            {
                var selected = chkListContacts.SelectedItem as RecordDispatchService.DispatchContact;
                if (selected != null)
                {
                    checkedItems.Add(selected);
                }
            }

            if (checkedItems.Count == 0)
            {
                MessageBox.Show("Please check the contact(s) you want to remove.", "Select Contact", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string prompt = checkedItems.Count == 1
                ? $"Remove contact:\n\n{checkedItems[0].Name} ({checkedItems[0].PhoneNumber})?"
                : $"Remove all {checkedItems.Count} selected contacts from saved list?";

            DialogResult dr = MessageBox.Show(
                prompt,
                "Confirm Remove",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (dr == DialogResult.Yes)
            {
                RecordDispatchService.RemoveContacts(checkedItems.Select(c => c.PhoneNumber));
                LoadContactsList();
            }
        }

        private void BtnSendWhatsApp_Click(object sender, EventArgs e)
        {
            var selected = GetSelectedContacts();
            if (selected.Count == 0)
            {
                MessageBox.Show("Please check at least one contact from the list.", "Select Recipient", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string pdfUrl = EnsureDirectPdfUrl();
            string msg = RecordDispatchService.FormatDispatchMessage(regCode, regTitle, logId, fieldData, pdfPath, pdfUrl);
            RecordDispatchService.CopyPdfFileToClipboard(pdfPath);

            foreach (var c in selected)
            {
                if (!string.IsNullOrWhiteSpace(c.PhoneNumber))
                {
                    RecordDispatchService.DispatchToWhatsApp(c.PhoneNumber, msg, pdfPath);
                }
            }
        }

        private void BtnSendTelegram_Click(object sender, EventArgs e)
        {
            var selected = GetSelectedContacts();
            if (selected.Count == 0)
            {
                MessageBox.Show("Please check at least one contact from the list.", "Select Recipient", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string pdfUrl = EnsureDirectPdfUrl();
            string msg = RecordDispatchService.FormatDispatchMessage(regCode, regTitle, logId, fieldData, pdfPath, pdfUrl);
            RecordDispatchService.CopyPdfFileToClipboard(pdfPath);

            foreach (var c in selected)
            {
                string target = !string.IsNullOrWhiteSpace(c.TelegramHandle) ? c.TelegramHandle : c.PhoneNumber;
                if (!string.IsNullOrWhiteSpace(target))
                {
                    RecordDispatchService.DispatchToTelegram(target, msg, pdfPath);
                }
            }
        }

        private void BtnSendBoth_Click(object sender, EventArgs e)
        {
            var selected = GetSelectedContacts();
            if (selected.Count == 0)
            {
                MessageBox.Show("Please check at least one contact from the list.", "Select Recipient", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string pdfUrl = EnsureDirectPdfUrl();
            string msg = RecordDispatchService.FormatDispatchMessage(regCode, regTitle, logId, fieldData, pdfPath, pdfUrl);
            RecordDispatchService.CopyPdfFileToClipboard(pdfPath);

            foreach (var c in selected)
            {
                if (!string.IsNullOrWhiteSpace(c.PhoneNumber))
                    RecordDispatchService.DispatchToWhatsApp(c.PhoneNumber, msg, pdfPath);

                string teleTarget = !string.IsNullOrWhiteSpace(c.TelegramHandle) ? c.TelegramHandle : c.PhoneNumber;
                if (!string.IsNullOrWhiteSpace(teleTarget))
                    RecordDispatchService.DispatchToTelegram(teleTarget, msg, pdfPath);
            }
        }
    }
}
