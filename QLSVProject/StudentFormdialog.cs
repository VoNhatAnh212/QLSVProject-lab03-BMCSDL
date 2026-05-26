using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace QLSVNhomApp
{
    /// <summary>
    /// Form nhập liệu dùng chung cho Thêm và Cập nhật sinh viên.
    /// - Thêm mới : new StudentFormDialog()              → các ô trống
    /// - Cập nhật : new StudentFormDialog(masv, hoten…)  → các ô có sẵn dữ liệu
    /// Kết quả lấy qua các property sau khi ShowDialog() == OK.
    /// </summary>
    public class StudentFormDialog : Form
    {
        // ─── Chế độ ───────────────────────────────────────────────────
        private readonly bool _isEditMode;

        // ─── Output properties ────────────────────────────────────────
        public string MaSV => txtMaSV.Text.Trim();
        public string HoTen => txtHoTen.Text.Trim();
        public string NgaySinh => txtNgaySinh.Text.Trim();
        public string DiaChi => txtDiaChi.Text.Trim();
        public string TenDN => txtTenDN.Text.Trim();
        public string MatKhau => txtMatKhau.Text.Trim();

        // ─── TextBox controls ─────────────────────────────────────────
        private TextBox txtMaSV;
        private TextBox txtHoTen;
        private TextBox txtNgaySinh;
        private TextBox txtDiaChi;
        private TextBox txtTenDN;
        private TextBox txtMatKhau;

        private Button btnOK;
        private Button btnCancel;
        private Panel pnlHeader;
        private Label lblHeaderTitle;

        // ─── Constructor: THÊM MỚI ────────────────────────────────────
        public StudentFormDialog()
        {
            _isEditMode = false;
            InitializeComponent();
        }

        // ─── Constructor: CẬP NHẬT ────────────────────────────────────
        public StudentFormDialog(string masv, string hoten,
                                 string ngaysinh, string diachi)
        {
            _isEditMode = true;
            InitializeComponent();

            // Điền dữ liệu cũ vào các ô
            Color normalColor = Color.FromArgb(30, 40, 60);

            txtMaSV.Text = masv; txtMaSV.ForeColor = normalColor;
            txtHoTen.Text = hoten; txtHoTen.ForeColor = normalColor;
            txtNgaySinh.Text = ngaysinh; txtNgaySinh.ForeColor = normalColor;
            txtDiaChi.Text = diachi; txtDiaChi.ForeColor = normalColor;

            // Khi cập nhật: ẩn phần tài khoản (không đổi TenDN / MK)
            SetAccountFieldsVisible(false);

            // Không cho sửa MaSV
            txtMaSV.ReadOnly = true;
            txtMaSV.BackColor = Color.FromArgb(240, 242, 245);
        }

        // ─── InitializeComponent ─────────────────────────────────────
        private void InitializeComponent()
        {
            // ---------- FORM ----------
            this.Text = _isEditMode ? "Cập nhật sinh viên" : "Thêm sinh viên mới";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(480, _isEditMode ? 460 : 570);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(245, 247, 250);

            // ---------- HEADER ----------
            pnlHeader = new Panel()
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.FromArgb(15, 52, 96)
            };
            pnlHeader.Paint += (s, e) =>
            {
                using (LinearGradientBrush br = new LinearGradientBrush(
                    pnlHeader.ClientRectangle,
                    Color.FromArgb(15, 52, 96),
                    Color.FromArgb(25, 80, 145),
                    LinearGradientMode.Horizontal))
                    e.Graphics.FillRectangle(br, pnlHeader.ClientRectangle);

                using (Pen pen = new Pen(Color.FromArgb(0, 180, 255), 3))
                    e.Graphics.DrawLine(pen,
                        0, pnlHeader.Height - 3,
                        pnlHeader.Width, pnlHeader.Height - 3);
            };

            string icon = _isEditMode ? "✏️" : "➕";
            Label lblIcon = new Label()
            {
                Text = icon,
                Font = new Font("Segoe UI Emoji", 18),
                AutoSize = false,
                Size = new Size(44, 44),
                Location = new Point(20, 13),
                BackColor = Color.Transparent,
                ForeColor = Color.White
            };

            lblHeaderTitle = new Label()
            {
                Text = _isEditMode ? "CẬP NHẬT SINH VIÊN" : "THÊM SINH VIÊN MỚI",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                Size = new Size(360, 40),
                Location = new Point(68, 15),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };

            pnlHeader.Controls.Add(lblIcon);
            pnlHeader.Controls.Add(lblHeaderTitle);

            // ---------- TABLE LAYOUT ----------
            // Mỗi hàng: Label bên trái + TextBox bên phải
            int startY = 90;
            int rowH = 66;
            int labelX = 30;
            int inputX = 170;
            int inputW = 270;

            // ─── Mã sinh viên ────────────────────────────────────────
            MakeRow("Mã sinh viên *", startY + rowH * 0,
                    labelX, inputX, inputW,
                    "VD: SV001", false,
                    out txtMaSV);

            // ─── Họ và tên ───────────────────────────────────────────
            MakeRow("Họ và tên *", startY + rowH * 1,
                    labelX, inputX, inputW,
                    "VD: Nguyễn Văn A", false,
                    out txtHoTen);

            // ─── Ngày sinh ───────────────────────────────────────────
            MakeRow("Ngày sinh *", startY + rowH * 2,
                    labelX, inputX, inputW,
                    "yyyy-MM-dd", false,
                    out txtNgaySinh);

            // ─── Địa chỉ ─────────────────────────────────────────────
            MakeRow("Địa chỉ *", startY + rowH * 3,
                    labelX, inputX, inputW,
                    "VD: Hà Nội", false,
                    out txtDiaChi);

            // ─── Tên đăng nhập (chỉ hiện khi thêm mới) ──────────────
            MakeRow("Tên đăng nhập *", startY + rowH * 4,
                    labelX, inputX, inputW,
                    "VD: nguyenvana", false,
                    out txtTenDN);

            // ─── Mật khẩu (chỉ hiện khi thêm mới) ───────────────────
            MakeRow("Mật khẩu *", startY + rowH * 5,
                    labelX, inputX, inputW,
                    "Nhập mật khẩu", true,
                    out txtMatKhau);

            // ---------- BUTTONS ----------
            int btnY = _isEditMode
                ? startY + rowH * 4 + 10
                : startY + rowH * 6 + 10;

            btnOK = new Button()
            {
                Text = _isEditMode ? "💾  Lưu thay đổi" : "✔  Thêm mới",
                Size = new Size(160, 44),
                Location = new Point(80, btnY),
                BackColor = _isEditMode
                    ? Color.FromArgb(34, 139, 87)
                    : Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand,
                DialogResult = DialogResult.OK
            };
            btnOK.FlatAppearance.BorderSize = 0;
            btnOK.Click += BtnOK_Click;

            Color okColor = btnOK.BackColor;
            btnOK.MouseEnter += (s, e) => btnOK.BackColor = ControlPaint.Light(okColor, 0.2f);
            btnOK.MouseLeave += (s, e) => btnOK.BackColor = okColor;

            btnCancel = new Button()
            {
                Text = "✖  Hủy",
                Size = new Size(130, 44),
                Location = new Point(260, btnY),
                BackColor = Color.FromArgb(108, 117, 125),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand,
                DialogResult = DialogResult.Cancel
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.MouseEnter += (s, e) => btnCancel.BackColor = Color.FromArgb(140, 148, 155);
            btnCancel.MouseLeave += (s, e) => btnCancel.BackColor = Color.FromArgb(108, 117, 125);

            // ---------- ADD TO FORM ----------
            this.Controls.Add(pnlHeader);
            this.Controls.Add(btnOK);
            this.Controls.Add(btnCancel);
            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
        }

        // ─── Tạo một hàng Label + TextBox ────────────────────────────
        private void MakeRow(string labelText, int top,
                             int labelX, int inputX, int inputW,
                             string placeholder, bool isPassword,
                             out TextBox outTxt)
        {
            Label lbl = new Label()
            {
                Text = labelText,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(80, 90, 110),
                AutoSize = false,
                Size = new Size(130, 20),
                Location = new Point(labelX, top + 18),
            };

            TextBox txt = new TextBox()
            {
                Size = new Size(inputW, 34),
                Location = new Point(inputX, top + 10),
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(30, 40, 60),
                PasswordChar = isPassword ? '●' : '\0'
            };

            // Placeholder effect
            if (!string.IsNullOrEmpty(placeholder))
            {
                txt.ForeColor = Color.FromArgb(180, 185, 195);
                txt.Text = placeholder;

                txt.GotFocus += (s, e) =>
                {
                    if (txt.Text == placeholder && txt.ForeColor == Color.FromArgb(180, 185, 195))
                    {
                        txt.Text = "";
                        txt.ForeColor = Color.FromArgb(30, 40, 60);
                    }
                };
                txt.LostFocus += (s, e) =>
                {
                    if (string.IsNullOrWhiteSpace(txt.Text))
                    {
                        txt.Text = placeholder;
                        txt.ForeColor = Color.FromArgb(180, 185, 195);
                    }
                };
            }

            // Đường kẻ phân cách nhẹ
            Panel separator = new Panel()
            {
                Location = new Point(labelX, top + 58),
                Size = new Size(inputX + inputW - labelX, 1),
                BackColor = Color.FromArgb(225, 230, 240)
            };

            this.Controls.Add(lbl);
            this.Controls.Add(txt);
            this.Controls.Add(separator);

            outTxt = txt;
        }

        // ─── Ẩn/hiện nhóm tài khoản ──────────────────────────────────
        private void SetAccountFieldsVisible(bool visible)
        {
            txtTenDN.Visible = visible;
            txtMatKhau.Visible = visible;

            // Ẩn cả label và separator tương ứng
            // (tìm theo vị trí Y vì MakeRow không lưu lại label)
            foreach (Control c in this.Controls)
            {
                if (c is Label lbl &&
                    (lbl.Text == "Tên đăng nhập *" || lbl.Text == "Mật khẩu *"))
                    lbl.Visible = visible;

                // ẩn separator của 2 row cuối
                if (c is Panel sep)
                {
                    int sepY = sep.Location.Y;
                    // row 4 và row 5 (startY=90, rowH=66 → top=354,420)
                    if (sepY >= 90 + 66 * 4 + 58)
                        sep.Visible = visible;
                }
            }
        }

        // ─── Validate khi bấm OK ─────────────────────────────────────
        private void BtnOK_Click(object sender, EventArgs e)
        {
            // Danh sách placeholder để bỏ qua khi validate
            string[] placeholders = {
                "VD: SV001", "VD: Nguyễn Văn A",
                "yyyy-MM-dd", "VD: Hà Nội",
                "VD: nguyenvana", "Nhập mật khẩu"
            };

            bool IsEmpty(TextBox t) =>
                string.IsNullOrWhiteSpace(t.Text) ||
                Array.IndexOf(placeholders, t.Text) >= 0;

            // Validate các trường bắt buộc
            if (IsEmpty(txtMaSV) || IsEmpty(txtHoTen) ||
                IsEmpty(txtNgaySinh) || IsEmpty(txtDiaChi))
            {
                MessageBox.Show(
                    "Vui lòng điền đầy đủ thông tin sinh viên!",
                    "Thiếu thông tin",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            // Validate ngày sinh
            if (!DateTime.TryParse(txtNgaySinh.Text.Trim(), out _))
            {
                MessageBox.Show(
                    "Ngày sinh không hợp lệ!\nĐịnh dạng: yyyy-MM-dd",
                    "Lỗi định dạng",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                txtNgaySinh.Focus();
                this.DialogResult = DialogResult.None;
                return;
            }

            // Validate tài khoản (chỉ khi thêm mới)
            if (!_isEditMode && (IsEmpty(txtTenDN) || IsEmpty(txtMatKhau)))
            {
                MessageBox.Show(
                    "Vui lòng điền tên đăng nhập và mật khẩu!",
                    "Thiếu thông tin",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            this.DialogResult = DialogResult.OK;
        }
    }
}