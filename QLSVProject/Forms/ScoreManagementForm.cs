using Microsoft.Data.SqlClient;
using QLSVProject.Helpers;
using System;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace QLSVNhomApp
{
    public partial class ScoreManagementForm : Form
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["QLSVNhomConnection"].ConnectionString;

        private readonly string masv;
        private readonly string manv;
        private readonly string password;

        private StudentManagementForm studentManagementForm;

        // ================= UI =================
        private DataGridView dgvHocPhan;
        private TextBox txtMAHP;
        private TextBox txtDIEMTHI;

        private Button btnSave;
        private Button btnLoad;
        private Button btnAddHocPhan;       // MỚI: tạo học phần
        private Button btnLinkHocPhan;      // MỚI: liên kết SV - HP

        private Label lblTitle;
        private Label lblMAHP;
        private Label lblDIEMTHI;

        private Panel panelInput;

        public ScoreManagementForm(
            string masv,
            string manv,
            string password,
            StudentManagementForm studentManagementForm)
        {
            this.masv = masv;
            this.manv = manv;
            this.password = password;
            this.studentManagementForm = studentManagementForm;

            InitializeComponent();
            LoadHocPhan();

            this.FormClosed += (s, e) => studentManagementForm.Show();
        }

        // ================= UI DESIGN =================
        private void InitializeComponent()
        {
            this.Text = "Quản lý điểm sinh viên";
            this.Size = new Size(1000, 760);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 247, 250);

            // ===== TITLE =====
            lblTitle = new Label()
            {
                Text = "QUẢN LÝ ĐIỂM SINH VIÊN",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41),
                Size = new Size(900, 40),
                Location = new Point(50, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // ===== TOOLBAR BUTTONS =====
            btnLoad = CreateToolbarButton(
                "🔄 Tải dữ liệu",
                new Point(50, 78),
                Color.FromArgb(0, 122, 204));
            btnLoad.Click += (s, e) => LoadHocPhan();

            btnAddHocPhan = CreateToolbarButton(
                "📚 Tạo học phần",
                new Point(220, 78),
                Color.FromArgb(120, 80, 200));
            btnAddHocPhan.Click += BtnAddHocPhan_Click;

            btnLinkHocPhan = CreateToolbarButton(
                "🔗 Liên kết HP",
                new Point(390, 78),
                Color.FromArgb(34, 139, 87));
            btnLinkHocPhan.Click += BtnLinkHocPhan_Click;

            // ===== GRID =====
            dgvHocPhan = new DataGridView()
            {
                Location = new Point(50, 140),
                Size = new Size(900, 360),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White
            };
            dgvHocPhan.SelectionChanged += DgvHocPhan_SelectionChanged;

            // ===== PANEL INPUT =====
            panelInput = new Panel()
            {
                Size = new Size(900, 120),
                Location = new Point(50, 520),
                BackColor = Color.White
            };

            lblMAHP = new Label()
            {
                Text = "Mã học phần",
                Location = new Point(40, 20),
                Size = new Size(150, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            txtMAHP = new TextBox()
            {
                Location = new Point(40, 50),
                Size = new Size(250, 30),
                ReadOnly = true,
                Font = new Font("Segoe UI", 10)
            };

            lblDIEMTHI = new Label()
            {
                Text = "Điểm thi",
                Location = new Point(350, 20),
                Size = new Size(150, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            txtDIEMTHI = new TextBox()
            {
                Location = new Point(350, 50),
                Size = new Size(250, 30),
                Font = new Font("Segoe UI", 10)
            };

            // ===== SAVE BUTTON =====
            btnSave = new Button()
            {
                Text = "💾 Lưu điểm",
                Size = new Size(180, 40),
                Location = new Point(650, 45),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            panelInput.Controls.AddRange(new Control[]
            {
                lblMAHP, txtMAHP,
                lblDIEMTHI, txtDIEMTHI,
                btnSave
            });

            this.Controls.AddRange(new Control[]
            {
                lblTitle,
                btnLoad, btnAddHocPhan, btnLinkHocPhan,
                dgvHocPhan,
                panelInput
            });
        }

        // ─── Factory button toolbar ───────────────────────────────────
        private Button CreateToolbarButton(string text, Point location, Color backColor)
        {
            Button btn = new Button()
            {
                Text = text,
                Size = new Size(155, 40),
                Location = location,
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;

            Color orig = backColor;
            btn.MouseEnter += (s, e) => btn.BackColor = ControlPaint.Light(orig, 0.15f);
            btn.MouseLeave += (s, e) => btn.BackColor = orig;
            return btn;
        }

        // ================= LOAD =================
        private void LoadHocPhan()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SP_SEL_PUBLIC_BANGDIEM", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    byte[] hashedMK;
                    using (SHA256 sha256 = SHA256.Create())
                        hashedMK = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                    cmd.Parameters.AddWithValue("@MASV", masv);
                    cmd.Parameters.AddWithValue("@MANV", manv);
                    cmd.Parameters.AddWithValue("@MK", hashedMK);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    DataTable newDt = dt.Clone();
                    newDt.Columns["DIEMTHI"].DataType = typeof(string);

                    foreach (DataRow row in dt.Rows)
                    {
                        DataRow newRow = newDt.NewRow();
                        foreach (DataColumn col in dt.Columns)
                        {
                            if (col.ColumnName == "DIEMTHI")
                            {
                                if (row["DIEMTHI"] != DBNull.Value)
                                {
                                    byte[] encrypted = (byte[])row["DIEMTHI"];
                                    byte[] decrypted = RsaHelper.Decrypt(Session.RsaKey, encrypted);
                                    int diem = BitConverter.ToInt32(decrypted, 0);
                                    newRow["DIEMTHI"] = diem.ToString();
                                }
                            }
                            else
                            {
                                newRow[col.ColumnName] = row[col.ColumnName];
                            }
                        }
                        newDt.Rows.Add(newRow);
                    }

                    dgvHocPhan.DataSource = newDt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu:\n" + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ================= SELECT ROW =================
        private void DgvHocPhan_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvHocPhan.SelectedRows.Count == 0) return;

            txtMAHP.Text = dgvHocPhan.SelectedRows[0].Cells["MAHP"].Value?.ToString();
            txtDIEMTHI.Text = dgvHocPhan.SelectedRows[0].Cells["DIEMTHI"].Value?.ToString();
        }

        // ================= SAVE SCORE =================
        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInput(out int diemThi)) return;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SP_UPD_PUBLIC_BANGDIEM", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@MASV", masv);
                    cmd.Parameters.AddWithValue("@MAHP", txtMAHP.Text);

                    byte[] encryptedDiem = RsaHelper.Encrypt(
                        Session.RsaKey, BitConverter.GetBytes(diemThi));
                    cmd.Parameters.AddWithValue("@DIEMTHI", encryptedDiem);
                    cmd.Parameters.AddWithValue("@MANV", manv);

                    byte[] hashedMK;
                    using (SHA256 sha256 = SHA256.Create())
                        hashedMK = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                    cmd.Parameters.AddWithValue("@MK", hashedMK);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Cập nhật điểm thành công!",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadHocPhan();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi:\n" + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ================= TẠO HỌC PHẦN (SP_INS_HOCPHAN) =================
        private void BtnAddHocPhan_Click(object sender, EventArgs e)
        {
            using (HocPhanFormDialog dlg = new HocPhanFormDialog())
            {
                if (dlg.ShowDialog() != DialogResult.OK) return;

                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        SqlCommand cmd = new SqlCommand("SP_INS_HOCPHAN", conn);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@MAHP", dlg.MaHP);
                        cmd.Parameters.AddWithValue("@TENHP", dlg.TenHP);
                        cmd.Parameters.AddWithValue("@SOTC", dlg.SoTC);

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show($"Tạo học phần [{dlg.MaHP}] thành công!",
                        "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadHocPhan();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Lỗi SQL:\n" + ex.Message,
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi:\n" + ex.Message,
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ================= LIÊN KẾT SV - HP (SP_INS_BANGDIEM) =================
        private void BtnLinkHocPhan_Click(object sender, EventArgs e)
        {
            using (LinkHocPhanFormDialog dlg = new LinkHocPhanFormDialog(masv))
            {
                if (dlg.ShowDialog() != DialogResult.OK) return;

                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        SqlCommand cmd = new SqlCommand("SP_INS_BANGDIEM", conn);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@MASV", masv);
                        cmd.Parameters.AddWithValue("@MAHP", dlg.MaHP);
                        // Điểm mặc định NULL khi mới liên kết
                        SqlParameter paramDiem = new SqlParameter("@DIEMTHI", SqlDbType.VarBinary);
                        paramDiem.Value = DBNull.Value;
                        cmd.Parameters.Add(paramDiem);

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show(
                        $"Đã liên kết sinh viên [{masv}]\nvới học phần [{dlg.MaHP}]!",
                        "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadHocPhan();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Lỗi SQL:\n" + ex.Message,
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi:\n" + ex.Message,
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ================= VALIDATE =================
        private bool ValidateInput(out int diemThi)
        {
            diemThi = 0;

            if (string.IsNullOrWhiteSpace(txtMAHP.Text))
            {
                MessageBox.Show("Vui lòng chọn học phần.",
                    "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!int.TryParse(txtDIEMTHI.Text.Trim(), out diemThi))
            {
                MessageBox.Show("Điểm phải là số nguyên.",
                    "Sai định dạng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (diemThi < 0 || diemThi > 10)
            {
                MessageBox.Show("Điểm phải từ 0 đến 10.",
                    "Ngoài phạm vi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }
    }


    // =================================================================
    // DIALOG: TẠO HỌC PHẦN
    // =================================================================
    public class HocPhanFormDialog : Form
    {
        public string MaHP => txtMaHP.Text.Trim();
        public string TenHP => txtTenHP.Text.Trim();
        public int SoTC => int.TryParse(txtSoTC.Text.Trim(), out int v) ? v : 0;

        private TextBox txtMaHP;
        private TextBox txtTenHP;
        private TextBox txtSoTC;
        private Button btnOK;
        private Button btnCancel;

        public HocPhanFormDialog()
        {
            this.Text = "Tạo học phần mới";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(440, 380);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(245, 247, 250);

            // Header
            Panel pnlHeader = new Panel()
            {
                Dock = DockStyle.Top,
                Height = 65,
                BackColor = Color.FromArgb(120, 80, 200)
            };
            pnlHeader.Paint += (s, e) =>
            {
                using (LinearGradientBrush br = new LinearGradientBrush(
                    pnlHeader.ClientRectangle,
                    Color.FromArgb(120, 80, 200),
                    Color.FromArgb(90, 55, 170),
                    LinearGradientMode.Horizontal))
                    e.Graphics.FillRectangle(br, pnlHeader.ClientRectangle);
            };

            Label lblHeader = new Label()
            {
                Text = "📚  TẠO HỌC PHẦN MỚI",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                Size = new Size(400, 40),
                Location = new Point(20, 13),
                BackColor = Color.Transparent
            };
            pnlHeader.Controls.Add(lblHeader);

            // Rows
            int startY = 85;
            int rowH = 70;

            MakeRow("Mã học phần *", startY + rowH * 0, "VD: HP001", out txtMaHP);
            MakeRow("Tên học phần *", startY + rowH * 1, "VD: Lập trình C#", out txtTenHP);
            MakeRow("Số tín chỉ *", startY + rowH * 2, "VD: 3", out txtSoTC);

            // Buttons
            int btnY = startY + rowH * 3 + 5;

            btnOK = MakeButton("✔  Tạo", new Point(70, btnY),
                Color.FromArgb(120, 80, 200), DialogResult.OK);
            btnOK.Click += BtnOK_Click;

            btnCancel = MakeButton("✖  Hủy", new Point(240, btnY),
                Color.FromArgb(108, 117, 125), DialogResult.Cancel);

            this.Controls.Add(pnlHeader);
            this.Controls.Add(btnOK);
            this.Controls.Add(btnCancel);
            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }

        private void MakeRow(string label, int top, string placeholder, out TextBox outTxt)
        {
            Label lbl = new Label()
            {
                Text = label,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(80, 90, 110),
                AutoSize = false,
                Size = new Size(360, 18),
                Location = new Point(40, top)
            };

            TextBox txt = new TextBox()
            {
                Size = new Size(360, 30),
                Location = new Point(40, top + 22),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(180, 185, 195),
                Text = placeholder
            };

            txt.GotFocus += (s, e) =>
            {
                if (txt.ForeColor == Color.FromArgb(180, 185, 195))
                { txt.Text = ""; txt.ForeColor = Color.FromArgb(30, 40, 60); }
            };
            txt.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txt.Text))
                { txt.Text = placeholder; txt.ForeColor = Color.FromArgb(180, 185, 195); }
            };

            this.Controls.Add(lbl);
            this.Controls.Add(txt);
            outTxt = txt;
        }

        private Button MakeButton(string text, Point loc, Color color, DialogResult dr)
        {
            Button btn = new Button()
            {
                Text = text,
                Size = new Size(130, 42),
                Location = loc,
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand,
                DialogResult = dr
            };
            btn.FlatAppearance.BorderSize = 0;
            Color orig = color;
            btn.MouseEnter += (s, e) => btn.BackColor = ControlPaint.Light(orig, 0.15f);
            btn.MouseLeave += (s, e) => btn.BackColor = orig;
            return btn;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            string[] ph = { "VD: HP001", "VD: Lập trình C#", "VD: 3" };

            bool IsEmpty(TextBox t) =>
                string.IsNullOrWhiteSpace(t.Text) || Array.IndexOf(ph, t.Text) >= 0;

            if (IsEmpty(txtMaHP) || IsEmpty(txtTenHP) || IsEmpty(txtSoTC))
            {
                MessageBox.Show("Vui lòng điền đầy đủ thông tin học phần!",
                    "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            if (!int.TryParse(txtSoTC.Text.Trim(), out int sotc) || sotc <= 0)
            {
                MessageBox.Show("Số tín chỉ phải là số nguyên lớn hơn 0!",
                    "Sai định dạng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSoTC.Focus();
                this.DialogResult = DialogResult.None;
                return;
            }

            this.DialogResult = DialogResult.OK;
        }
    }


    // =================================================================
    // DIALOG: LIÊN KẾT SINH VIÊN - HỌC PHẦN
    // =================================================================
    public class LinkHocPhanFormDialog : Form
    {
        public string MaHP => txtMaHP.Text.Trim();

        private readonly string masv;
        private TextBox txtMaSV;
        private TextBox txtMaHP;
        private Button btnOK;
        private Button btnCancel;

        public LinkHocPhanFormDialog(string masv)
        {
            this.masv = masv;

            this.Text = "Liên kết sinh viên - học phần";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(440, 320);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(245, 247, 250);

            // Header
            Panel pnlHeader = new Panel()
            {
                Dock = DockStyle.Top,
                Height = 65,
                BackColor = Color.FromArgb(34, 139, 87)
            };
            pnlHeader.Paint += (s, e) =>
            {
                using (LinearGradientBrush br = new LinearGradientBrush(
                    pnlHeader.ClientRectangle,
                    Color.FromArgb(34, 139, 87),
                    Color.FromArgb(22, 105, 65),
                    LinearGradientMode.Horizontal))
                    e.Graphics.FillRectangle(br, pnlHeader.ClientRectangle);
            };

            Label lblHeader = new Label()
            {
                Text = "🔗  LIÊN KẾT HỌC PHẦN",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                Size = new Size(400, 40),
                Location = new Point(20, 13),
                BackColor = Color.Transparent
            };
            pnlHeader.Controls.Add(lblHeader);

            int startY = 85;
            int rowH = 70;

            // MaSV: readonly, hiển thị sẵn
            Label lblMaSV = new Label()
            {
                Text = "Mã sinh viên",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(80, 90, 110),
                AutoSize = false,
                Size = new Size(360, 18),
                Location = new Point(40, startY)
            };

            txtMaSV = new TextBox()
            {
                Size = new Size(360, 30),
                Location = new Point(40, startY + 22),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(240, 242, 245),
                ForeColor = Color.FromArgb(30, 40, 60),
                ReadOnly = true,
                Text = masv
            };

            // MaHP: nhập tay
            Label lblMaHP = new Label()
            {
                Text = "Mã học phần *",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(80, 90, 110),
                AutoSize = false,
                Size = new Size(360, 18),
                Location = new Point(40, startY + rowH)
            };

            txtMaHP = new TextBox()
            {
                Size = new Size(360, 30),
                Location = new Point(40, startY + rowH + 22),
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(180, 185, 195),
                Text = "VD: HP001"
            };
            txtMaHP.GotFocus += (s, e) =>
            {
                if (txtMaHP.ForeColor == Color.FromArgb(180, 185, 195))
                { txtMaHP.Text = ""; txtMaHP.ForeColor = Color.FromArgb(30, 40, 60); }
            };
            txtMaHP.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtMaHP.Text))
                { txtMaHP.Text = "VD: HP001"; txtMaHP.ForeColor = Color.FromArgb(180, 185, 195); }
            };

            // Buttons
            int btnY = startY + rowH * 2 + 10;

            btnOK = new Button()
            {
                Text = "🔗  Liên kết",
                Size = new Size(140, 42),
                Location = new Point(60, btnY),
                BackColor = Color.FromArgb(34, 139, 87),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand,
                DialogResult = DialogResult.OK
            };
            btnOK.FlatAppearance.BorderSize = 0;
            btnOK.Click += BtnOK_Click;

            btnCancel = new Button()
            {
                Text = "✖  Hủy",
                Size = new Size(130, 42),
                Location = new Point(225, btnY),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand,
                DialogResult = DialogResult.Cancel
            };
            btnCancel.FlatAppearance.BorderSize = 0;

            this.Controls.Add(pnlHeader);
            this.Controls.Add(lblMaSV);
            this.Controls.Add(txtMaSV);
            this.Controls.Add(lblMaHP);
            this.Controls.Add(txtMaHP);
            this.Controls.Add(btnOK);
            this.Controls.Add(btnCancel);
            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaHP.Text) ||
                txtMaHP.Text == "VD: HP001")
            {
                MessageBox.Show("Vui lòng nhập mã học phần!",
                    "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                txtMaHP.Focus();
                return;
            }
            this.DialogResult = DialogResult.OK;
        }
    }
}