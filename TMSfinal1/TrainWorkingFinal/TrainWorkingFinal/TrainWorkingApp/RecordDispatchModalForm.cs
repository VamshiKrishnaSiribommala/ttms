using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrainWorkingApp
{
    /// <summary>
    /// Clean, Professional 1-Click Train Working Authority Dispatch Modal Form.
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
        private Button btnCopyMessage;
        private Button btnViewPdf;
        private Button btnClose;

        public RecordDispatchModalForm(string regCode, string regTitle, string logId, Dictionary<string, string> fieldData, string pdfPath)
        {
            this.regCode = regCode;
            this.regTitle = regTitle;
            this.logId = logId;
            this.fieldData = fieldData ?? new Dictionary<string, string>();
            this.pdfPath = pdfPath;

            InitializeForm();

            // Background task: Upload PDF to secure storage to get an instant clickable URL
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
            this.Text = "Official Authority Memo Dispatch";
            this.Size = new Size(820, 800);
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
                Text = "OFFICIAL AUTHORITY DISPATCH",
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

            // ── 2. STATUS PILL ────────────────────────────────────────────────────
            Panel pnlStatus = new Panel
            {
                Location = new Point(20, 72),
                Size = new Size(764, 48),
                BackColor = Color.FromArgb(240, 253, 244)
            };
            pnlStatus.Paint += (s, e) =>
            {
                using (Pen p = new Pen(Color.FromArgb(187, 247, 208), 1.2f))
                    e.Graphics.DrawRectangle(p, 0, 0, pnlStatus.Width - 1, pnlStatus.Height - 1);
            };
            Label lblStatusText = new Label
            {
                Text = $"Record saved successfully. Official PDF Memo generated ({Path.GetFileName(pdfPath)}).",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(22, 101, 52),
                Location = new Point(12, 5),
                AutoSize = true
            };

            lblPdfLinkStatus = new Label
            {
                Text = "Generating secure online PDF link for instant viewing...",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Italic),
                ForeColor = Color.FromArgb(2, 132, 199),
                Location = new Point(12, 25),
                AutoSize = true
            };

            btnCopyPdfLink = new Button
            {
                Text = "Copy Link",
                Font = new Font("Segoe UI", 8F, FontStyle.Bold),
                BackColor = Color.FromArgb(224, 242, 254),
                ForeColor = Color.FromArgb(3, 105, 161),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(650, 18),
                Size = new Size(100, 24),
                Visible = false,
                Cursor = Cursors.Hand
            };
            btnCopyPdfLink.FlatAppearance.BorderColor = Color.FromArgb(186, 230, 253);
            btnCopyPdfLink.Click += (s, e) =>
            {
                if (!string.IsNullOrEmpty(directPdfUrl))
                {
                    Clipboard.SetText(directPdfUrl);
                    MessageBox.Show("Direct PDF Link copied to clipboard!\n\n" + directPdfUrl, "Link Copied", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };

            pnlStatus.Controls.AddRange(new Control[] { lblStatusText, lblPdfLinkStatus, btnCopyPdfLink });
            this.Controls.Add(pnlStatus);

            // ── 3. AUTHORITY DETAILS PREVIEW CARD (NEW: Prominent Verification Box) ──
            GroupBox grpDetails = new GroupBox
            {
                Text = " Authority Information & Operational Parameters (Dispatched Content) ",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(20, 126),
                Size = new Size(764, 185),
                BackColor = Color.White
            };

            // Quick Badges / Header
            FlowLayoutPanel pnlBadges = new FlowLayoutPanel
            {
                Location = new Point(12, 20),
                Size = new Size(540, 28),
                BackColor = Color.White,
                WrapContents = false,
                AutoScroll = false
            };

            string trainVal = fieldData.FirstOrDefault(k => k.Key.ToLower().Contains("train")).Value;
            if (!string.IsNullOrWhiteSpace(trainVal) && trainVal != "-")
            {
                Label lblTrainBadge = new Label
                {
                    Text = $"Train: {trainVal}",
                    Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                    BackColor = Color.FromArgb(224, 231, 255),
                    ForeColor = Color.FromArgb(49, 46, 129),
                    Padding = new Padding(6, 3, 6, 3),
                    AutoSize = true,
                    Margin = new Padding(0, 0, 8, 0)
                };
                pnlBadges.Controls.Add(lblTrainBadge);
            }

            string sigVal = fieldData.FirstOrDefault(k => k.Key.ToLower().Contains("signal") && !k.Key.ToLower().Contains("location")).Value;
            if (!string.IsNullOrWhiteSpace(sigVal) && sigVal != "-")
            {
                Label lblSigBadge = new Label
                {
                    Text = $"Signal: {sigVal}",
                    Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                    BackColor = Color.FromArgb(254, 243, 199),
                    ForeColor = Color.FromArgb(146, 64, 14),
                    Padding = new Padding(6, 3, 6, 3),
                    AutoSize = true,
                    Margin = new Padding(0, 0, 8, 0)
                };
                pnlBadges.Controls.Add(lblSigBadge);
            }

            Label lblRefBadge = new Label
            {
                Text = $"Ref ID: {logId}",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                BackColor = Color.FromArgb(241, 245, 249),
                ForeColor = Color.FromArgb(71, 85, 105),
                Padding = new Padding(6, 3, 6, 3),
                AutoSize = true
            };
            pnlBadges.Controls.Add(lblRefBadge);
            grpDetails.Controls.Add(pnlBadges);

            // Direct Open PDF button in details box
            btnViewPdf = new Button
            {
                Text = "View PDF Document",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                BackColor = Color.FromArgb(238, 242, 255),
                ForeColor = Color.FromArgb(67, 56, 202),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(565, 18),
                Size = new Size(185, 26),
                Cursor = Cursors.Hand
            };
            btnViewPdf.FlatAppearance.BorderColor = Color.FromArgb(199, 210, 254);
            btnViewPdf.Click += (s, e) => RecordDispatchService.OpenPdfFile(pdfPath);
            grpDetails.Controls.Add(btnViewPdf);

            // Scrollable Grid of All Parameters
            Panel pnlDataScroll = new Panel
            {
                Location = new Point(12, 50),
                Size = new Size(740, 122),
                BackColor = Color.FromArgb(248, 250, 252),
                BorderStyle = BorderStyle.FixedSingle,
                AutoScroll = true
            };

            int curGridY = 4;
            int count = 0;
            foreach (var kvp in fieldData)
            {
                int xOffset = (count % 2 == 0) ? 8 : 375;
                int yOffset = curGridY + (count / 2) * 23;

                Label lblKey = new Label
                {
                    Text = kvp.Key + ":",
                    Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(51, 65, 85),
                    Location = new Point(xOffset, yOffset),
                    Size = new Size(170, 18),
                    AutoEllipsis = true
                };

                bool isVerified = kvp.Value.IndexOf("YES", StringComparison.OrdinalIgnoreCase) >= 0 || kvp.Value.IndexOf("Verified", StringComparison.OrdinalIgnoreCase) >= 0;
                bool isKeyParam = kvp.Key.IndexOf("Train", StringComparison.OrdinalIgnoreCase) >= 0 || kvp.Key.IndexOf("Signal", StringComparison.OrdinalIgnoreCase) >= 0 || kvp.Key.IndexOf("Speed", StringComparison.OrdinalIgnoreCase) >= 0;

                Label lblVal = new Label
                {
                    Text = kvp.Value,
                    Font = new Font("Segoe UI", 8.5F, (isVerified || isKeyParam) ? FontStyle.Bold : FontStyle.Regular),
                    ForeColor = isVerified ? Color.FromArgb(22, 101, 52) : (isKeyParam ? Color.FromArgb(30, 58, 138) : Color.FromArgb(15, 23, 42)),
                    Location = new Point(xOffset + 175, yOffset),
                    Size = new Size(185, 18),
                    AutoEllipsis = true
                };

                pnlDataScroll.Controls.Add(lblKey);
                pnlDataScroll.Controls.Add(lblVal);
                count++;
            }
            grpDetails.Controls.Add(pnlDataScroll);
            this.Controls.Add(grpDetails);

            // ── 4. RECIPIENT SELECTION LIST ───────────────────────────────────────
            GroupBox grpContacts = new GroupBox
            {
                Text = " Select Recipients to Send Memo ",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                Location = new Point(20, 318),
                Size = new Size(764, 195),
                BackColor = Color.White
            };

            chkListContacts = new CheckedListBox
            {
                Location = new Point(15, 25),
                Size = new Size(585, 155),
                Font = new Font("Segoe UI", 9F),
                CheckOnClick = true,
                BorderStyle = BorderStyle.FixedSingle
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

            btnSelectAll = new Button
            {
                Text = "Select All",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Location = new Point(612, 25),
                Size = new Size(138, 28),
                BackColor = Color.FromArgb(248, 250, 252),
                Cursor = Cursors.Hand
            };
            btnSelectAll.Click += (s, e) =>
            {
                for (int i = 0; i < chkListContacts.Items.Count; i++)
                    chkListContacts.SetItemChecked(i, true);
                UpdateSelectedSummaryAndButtons();
            };

            btnDeselectAll = new Button
            {
                Text = "Clear All",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Location = new Point(612, 58),
                Size = new Size(138, 28),
                BackColor = Color.FromArgb(248, 250, 252),
                Cursor = Cursors.Hand
            };
            btnDeselectAll.Click += (s, e) =>
            {
                for (int i = 0; i < chkListContacts.Items.Count; i++)
                    chkListContacts.SetItemChecked(i, false);
                UpdateSelectedSummaryAndButtons();
            };

            btnRemoveSelected = new Button
            {
                Text = "Remove Contact",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                Location = new Point(612, 91),
                Size = new Size(138, 28),
                BackColor = Color.FromArgb(254, 242, 242),
                ForeColor = Color.FromArgb(185, 28, 28),
                Cursor = Cursors.Hand
            };
            btnRemoveSelected.FlatAppearance.BorderColor = Color.FromArgb(254, 202, 202);
            btnRemoveSelected.Click += BtnRemoveSelected_Click;

            lblSelectedSummary = new Label
            {
                Text = "Selected: 0",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(37, 99, 235),
                Location = new Point(614, 130),
                Size = new Size(138, 45),
                TextAlign = ContentAlignment.TopLeft
            };

            grpContacts.Controls.AddRange(new Control[] { btnSelectAll, btnDeselectAll, btnRemoveSelected, lblSelectedSummary });
            this.Controls.Add(grpContacts);

            // ── 5. QUICK ADD NEW CONTACT ──────────────────────────────────────────
            GroupBox grpAdd = new GroupBox
            {
                Text = " Add New Contact ",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(71, 85, 105),
                Location = new Point(20, 520),
                Size = new Size(764, 68),
                BackColor = Color.White
            };

            Label lblP = new Label { Text = "Mobile Number:", Font = new Font("Segoe UI", 8F), Location = new Point(15, 16), AutoSize = true };
            txtNewPhone = new TextBox { Font = new Font("Segoe UI", 9F), Location = new Point(15, 34), Size = new Size(160, 24) };

            Label lblN = new Label { Text = "Name:", Font = new Font("Segoe UI", 8F), Location = new Point(190, 16), AutoSize = true };
            txtNewName = new TextBox { Font = new Font("Segoe UI", 9F), Location = new Point(190, 34), Size = new Size(185, 24) };

            Label lblD = new Label { Text = "Designation:", Font = new Font("Segoe UI", 8F), Location = new Point(390, 16), AutoSize = true };
            txtNewDesignation = new TextBox { Font = new Font("Segoe UI", 9F), Location = new Point(390, 34), Size = new Size(205, 24) };

            btnAddNewContact = new Button
            {
                Text = "+ Save Contact",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                BackColor = Color.FromArgb(241, 245, 249),
                ForeColor = Color.FromArgb(30, 58, 138),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(612, 30),
                Size = new Size(138, 28),
                Cursor = Cursors.Hand
            };
            btnAddNewContact.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnAddNewContact.Click += BtnAddNewContact_Click;

            grpAdd.Controls.AddRange(new Control[] { lblP, txtNewPhone, lblN, txtNewName, lblD, txtNewDesignation, btnAddNewContact });
            this.Controls.Add(grpAdd);

            // ── 6. 1-CLICK DISPATCH BUTTONS ──────────────────────────────────────
            Panel pnlActions = new Panel
            {
                Location = new Point(20, 595),
                Size = new Size(764, 155),
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
                Text = "Send via WhatsApp",
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                BackColor = Color.FromArgb(37, 211, 102),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(15, 12),
                Size = new Size(360, 42),
                Cursor = Cursors.Hand
            };
            btnSendWhatsApp.FlatAppearance.BorderSize = 0;
            btnSendWhatsApp.Click += BtnSendWhatsApp_Click;

            // Main Action 2: Telegram
            btnSendTelegram = new Button
            {
                Text = "Send via Telegram",
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                BackColor = Color.FromArgb(0, 136, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(388, 12),
                Size = new Size(362, 42),
                Cursor = Cursors.Hand
            };
            btnSendTelegram.FlatAppearance.BorderSize = 0;
            btnSendTelegram.Click += BtnSendTelegram_Click;

            // Main Action 3: Both
            btnSendBoth = new Button
            {
                Text = "Send to Both (WhatsApp + Telegram)",
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                BackColor = Color.FromArgb(109, 40, 217),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(15, 60),
                Size = new Size(735, 38),
                Cursor = Cursors.Hand
            };
            btnSendBoth.FlatAppearance.BorderSize = 0;
            btnSendBoth.Click += BtnSendBoth_Click;

            // Clipboard Action 1: Copy PDF File
            btnCopyPdf = new Button
            {
                Text = "Copy PDF File (Ctrl+V)",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                BackColor = Color.FromArgb(241, 245, 249),
                ForeColor = Color.FromArgb(51, 65, 85),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(15, 106),
                Size = new Size(240, 36),
                Cursor = Cursors.Hand
            };
            btnCopyPdf.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnCopyPdf.Click += (s, e) =>
            {
                RecordDispatchService.CopyPdfFileToClipboard(pdfPath);
                MessageBox.Show("Official PDF file copied to Clipboard!\n\nYou can now switch to WhatsApp/Telegram/Email and press Ctrl+V to paste the document directly.", "PDF File Copied", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            // Clipboard Action 2: Copy Full Text Message
            btnCopyMessage = new Button
            {
                Text = "Copy Text Message",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                BackColor = Color.FromArgb(241, 245, 249),
                ForeColor = Color.FromArgb(51, 65, 85),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(265, 106),
                Size = new Size(240, 36),
                Cursor = Cursors.Hand
            };
            btnCopyMessage.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnCopyMessage.Click += (s, e) =>
            {
                string text = RecordDispatchService.FormatDispatchMessage(regCode, regTitle, logId, fieldData, pdfPath, EnsureDirectPdfUrl());
                Clipboard.SetText(text);
                MessageBox.Show("Authority Text Message copied to Clipboard!\n\nYou can now paste it into any chat window.", "Text Copied", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            // Close
            btnClose = new Button
            {
                Text = "Done / Close",
                Font = new Font("Segoe UI", 8.5F),
                BackColor = Color.FromArgb(241, 245, 249),
                ForeColor = Color.FromArgb(71, 85, 105),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(515, 106),
                Size = new Size(235, 36),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnClose.Click += (s, e) => this.Close();

            pnlActions.Controls.AddRange(new Control[] { btnSendWhatsApp, btnSendTelegram, btnSendBoth, btnCopyPdf, btnCopyMessage, btnClose });
            this.Controls.Add(pnlActions);

            LoadContactsList();
        }

        private void LoadContactsList()
        {
            chkListContacts.Items.Clear();
            var contacts = RecordDispatchService.LoadSavedContacts();
            foreach (var c in contacts)
            {
                chkListContacts.Items.Add(c, false);
            }

            // Auto-check first recipient by default
            if (chkListContacts.Items.Count > 0)
            {
                chkListContacts.SetItemChecked(0, true);
            }

            UpdateSelectedSummaryAndButtons();
        }

        private void UpdateSelectedSummaryAndButtons()
        {
            var selected = chkListContacts.CheckedItems.Cast<RecordDispatchService.DispatchContact>().ToList();
            int count = selected.Count;

            lblSelectedSummary.Text = $"Selected: {count}\n" + (count == 0 ? "(None selected)" : (count == 1 ? selected[0].Name : $"{selected[0].Name} +{count - 1} more"));

            bool hasSelection = count > 0;
            btnSendWhatsApp.Enabled = hasSelection;
            btnSendTelegram.Enabled = hasSelection;
            btnSendBoth.Enabled = hasSelection;

            btnSendWhatsApp.Text = count > 1 ? $"Send to {count} Recipients (WhatsApp)" : "Send via WhatsApp";
            btnSendTelegram.Text = count > 1 ? $"Send to {count} Recipients (Telegram)" : "Send via Telegram";
            btnSendBoth.Text = count > 1 ? $"Send to Both ({count} Recipients)" : "Send to Both (WhatsApp + Telegram)";
        }

        private void BtnAddNewContact_Click(object sender, EventArgs e)
        {
            string phone = txtNewPhone.Text.Trim();
            string name = txtNewName.Text.Trim();
            string des = txtNewDesignation.Text.Trim();

            if (string.IsNullOrWhiteSpace(phone) || phone.Length < 8)
            {
                MessageBox.Show("Please enter a valid mobile number (with country code, e.g. +919876543210).", "Invalid Phone", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNewPhone.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                name = "Railway Official";
            }

            RecordDispatchService.AddContact(name, des, phone, "@" + name.Replace(" ", ""));
            txtNewPhone.Clear();
            txtNewName.Clear();
            txtNewDesignation.Clear();

            LoadContactsList();
            MessageBox.Show("✔ Contact saved to directory.", "Contact Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnRemoveSelected_Click(object sender, EventArgs e)
        {
            var selected = chkListContacts.CheckedItems.Cast<RecordDispatchService.DispatchContact>().ToList();
            if (selected.Count == 0)
            {
                MessageBox.Show("Please check the contacts in the list you wish to remove.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show($"Are you sure you want to remove {selected.Count} selected contact(s) from your saved directory?", "Confirm Remove", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                RecordDispatchService.RemoveContacts(selected.Select(c => c.PhoneNumber));
                LoadContactsList();
            }
        }

        private void BtnSendWhatsApp_Click(object sender, EventArgs e)
        {
            var selected = chkListContacts.CheckedItems.Cast<RecordDispatchService.DispatchContact>().ToList();
            if (selected.Count == 0)
            {
                MessageBox.Show("Please select at least one recipient.", "Recipient Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string onlinePdfUrl = EnsureDirectPdfUrl();
            string msg = RecordDispatchService.FormatDispatchMessage(regCode, regTitle, logId, fieldData, pdfPath, onlinePdfUrl);

            foreach (var contact in selected)
            {
                RecordDispatchService.DispatchToWhatsApp(contact.PhoneNumber, msg, pdfPath);
            }
        }

        private void BtnSendTelegram_Click(object sender, EventArgs e)
        {
            var selected = chkListContacts.CheckedItems.Cast<RecordDispatchService.DispatchContact>().ToList();
            if (selected.Count == 0)
            {
                MessageBox.Show("Please select at least one recipient.", "Recipient Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string onlinePdfUrl = EnsureDirectPdfUrl();
            string msg = RecordDispatchService.FormatDispatchMessage(regCode, regTitle, logId, fieldData, pdfPath, onlinePdfUrl);

            foreach (var contact in selected)
            {
                RecordDispatchService.DispatchToTelegram(contact.TelegramHandle, msg, pdfPath);
            }
        }

        private void BtnSendBoth_Click(object sender, EventArgs e)
        {
            var selected = chkListContacts.CheckedItems.Cast<RecordDispatchService.DispatchContact>().ToList();
            if (selected.Count == 0)
            {
                MessageBox.Show("Please select at least one recipient.", "Recipient Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string onlinePdfUrl = EnsureDirectPdfUrl();
            string msg = RecordDispatchService.FormatDispatchMessage(regCode, regTitle, logId, fieldData, pdfPath, onlinePdfUrl);

            foreach (var contact in selected)
            {
                RecordDispatchService.DispatchToWhatsApp(contact.PhoneNumber, msg, pdfPath);
                RecordDispatchService.DispatchToTelegram(contact.TelegramHandle, msg, pdfPath);
            }
        }
    }
}
