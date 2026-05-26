using QLSVProject.Helpers;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace QLSVNhomApp
{
    /// <summary>
    /// Hiển thị thông tin cá nhân của nhân viên đang đăng nhập.
    /// Dữ liệu lấy trực tiếp từ Session (không cần query DB lại).
    /// </summary>
    public class EmployeeInfoForm : Form
    {
        // ─── Controls ─────────────────────────────────────────────────
        private Panel pnlHeader;
        private Label lblHeaderTitle;
        private Label lblHeaderIcon;

        private Panel pnlCard;

        private Panel pnlAvatar;
        private Label lblAvatarInitial;

        private Panel pnlRowManv;
        private Panel pnlRowHoTen;
        private Panel pnlRowEmail;
        private Panel pnlRowLuong;

        private Button btnClose;

        // ─── Constructor ──────────────────────────────────────────────
        public EmployeeInfoForm()
        {
            InitializeComponent();
            LoadFromSession();
        }

        // ─── InitializeComponent ─────────────────────────────────────
        private void InitializeComponent()
        {
            // ---------- FORM ----------
            this.Text = "Thông tin cá nhân";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(520, 660);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(245, 247, 250);

            // =========================================================
            // HEADER
            // =========================================================
            pnlHeader = new Panel()
            {
                Dock = DockStyle.Top,
                Height = 90,
                BackColor = Color.FromArgb(15, 52, 96)
            };
            pnlHeader.Paint += (s, e) =>
            {
                using (LinearGradientBrush br = new LinearGradientBrush(
                    pnlHeader.ClientRectangle,
                    Color.FromArgb(15, 52, 96),
                    Color.FromArgb(25, 80, 145),
                    LinearGradientMode.Horizontal))
                {
                    e.Graphics.FillRectangle(br, pnlHeader.ClientRectangle);
                }
                // gạch accent phía dưới header
                using (Pen pen = new Pen(Color.FromArgb(0, 180, 255), 3))
                    e.Graphics.DrawLine(pen,
                        0, pnlHeader.Height - 3,
                        pnlHeader.Width, pnlHeader.Height - 3);
            };

            lblHeaderIcon = new Label()
            {
                Text = "🪪",
                Font = new Font("Segoe UI Emoji", 22),
                AutoSize = false,
                Size = new Size(50, 50),
                Location = new Point(25, 20),
                BackColor = Color.Transparent,
                ForeColor = Color.White
            };

            lblHeaderTitle = new Label()
            {
                Text = "THÔNG TIN CÁ NHÂN",
                Font = new Font("Segoe UI", 17, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                Size = new Size(380, 40),
                Location = new Point(80, 25),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };

            pnlHeader.Controls.Add(lblHeaderIcon);
            pnlHeader.Controls.Add(lblHeaderTitle);

            // =========================================================
            // CARD
            // =========================================================
            pnlCard = new Panel()
            {
                Location = new Point(30, 108),
                Size = new Size(460, 400),
                BackColor = Color.Transparent
            };
            pnlCard.Paint += PnlCard_Paint;

            // ─── AVATAR ───────────────────────────────────────────────
            pnlAvatar = new Panel()
            {
                Location = new Point((460 - 80) / 2, 22),
                Size = new Size(80, 80),
                BackColor = Color.Transparent
            };
            pnlAvatar.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (SolidBrush br = new SolidBrush(Color.FromArgb(120, 80, 200)))
                    e.Graphics.FillEllipse(br, 0, 0, 78, 78);
                using (Pen pen = new Pen(Color.FromArgb(200, 180, 255), 3))
                    e.Graphics.DrawEllipse(pen, 1, 1, 76, 76);
            };

            lblAvatarInitial = new Label()
            {
                Text = "?",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                AutoSize = false,
                Size = new Size(80, 80),
                Location = new Point(0, 0),
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlAvatar.Controls.Add(lblAvatarInitial);

            // ─── INFO ROWS ────────────────────────────────────────────
            int rowStartY = 118;
            int rowGap = 65;

            pnlRowManv = CreateInfoRow("Mã nhân viên", "🆔", rowStartY + rowGap * 0);
            pnlRowHoTen = CreateInfoRow("Họ và tên", "👤", rowStartY + rowGap * 1);
            pnlRowEmail = CreateInfoRow("Email", "✉️", rowStartY + rowGap * 2);
            pnlRowLuong = CreateInfoRow("Lương (VNĐ)", "💰", rowStartY + rowGap * 3);

            pnlCard.Controls.Add(pnlAvatar);
            pnlCard.Controls.Add(pnlRowManv);
            pnlCard.Controls.Add(pnlRowHoTen);
            pnlCard.Controls.Add(pnlRowEmail);
            pnlCard.Controls.Add(pnlRowLuong);

            // ─── BUTTON ĐÓNG ─────────────────────────────────────────
            btnClose = new Button()
            {
                Text = "Đóng",
                Size = new Size(150, 44),
                Location = new Point((520 - 150) / 2, 550),
                BackColor = Color.FromArgb(210, 50, 65),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleCenter
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();
            btnClose.MouseEnter += (s, e) => btnClose.BackColor = Color.FromArgb(235, 75, 90);
            btnClose.MouseLeave += (s, e) => btnClose.BackColor = Color.FromArgb(210, 50, 65);

            // ─── ADD TO FORM ──────────────────────────────────────────
            this.Controls.Add(pnlHeader);
            this.Controls.Add(pnlCard);
            this.Controls.Add(btnClose);
        }

        // ─── Load dữ liệu từ Session (không query DB) ─────────────────
        private void LoadFromSession()
        {
            // Lấy thẳng từ Session
            string sManv = Session.MaNV ?? "---";
            string sHoten = Session.HoTen ?? "---";
            string sEmail = Session.Email ?? "---";
            string sLuong = Session.Luong > 0
                ? Session.Luong.ToString("N0") + " ₫"
                : "---";

            // Gán vào các row
            SetRowValue(pnlRowManv, sManv);
            SetRowValue(pnlRowHoTen, sHoten);
            SetRowValue(pnlRowEmail, sEmail);
            SetRowValue(pnlRowLuong, sLuong, Color.FromArgb(34, 139, 87));

            // Avatar: chữ cái cuối của HoTen (tên riêng người Việt)
            if (!string.IsNullOrWhiteSpace(sHoten))
            {
                string[] parts = sHoten.Trim().Split(
                    new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                lblAvatarInitial.Text =
                    parts[parts.Length - 1].Substring(0, 1).ToUpper();
            }
        }

        // ─── Vẽ card bo góc + shadow ──────────────────────────────────
        private void PnlCard_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            for (int i = 5; i >= 1; i--)
            {
                Rectangle shadow = new Rectangle(i, i,
                    pnlCard.Width - 10, pnlCard.Height - 10);
                using (SolidBrush sb = new SolidBrush(Color.FromArgb(15 * i, 0, 0, 0)))
                    g.FillRectangle(sb, shadow);
            }

            Rectangle rect = new Rectangle(0, 0,
                pnlCard.Width - 8, pnlCard.Height - 8);
            using (GraphicsPath path = RoundedRect(rect, 16))
            using (SolidBrush white = new SolidBrush(Color.White))
                g.FillPath(white, path);
        }

        // ─── Tạo một hàng thông tin ───────────────────────────────────
        private Panel CreateInfoRow(string title, string icon, int top)
        {
            Panel row = new Panel()
            {
                Location = new Point(18, top),
                Size = new Size(424, 56),
                BackColor = Color.Transparent
            };

            // đường kẻ phân cách
            row.Paint += (s, e) =>
            {
                using (Pen pen = new Pen(Color.FromArgb(230, 235, 245), 1))
                    e.Graphics.DrawLine(pen, 0, 0, row.Width, 0);
            };

            Label lblIcon = new Label()
            {
                Text = icon,
                Font = new Font("Segoe UI Emoji", 13),
                AutoSize = false,
                Size = new Size(34, 28),
                Location = new Point(16, 6),
                BackColor = Color.Transparent,
                TextAlign = ContentAlignment.MiddleCenter
            };

            Label lblTitle = new Label()
            {
                Text = title,
                Font = new Font("Segoe UI", 8, FontStyle.Regular),
                ForeColor = Color.FromArgb(140, 150, 170),
                AutoSize = false,
                Size = new Size(340, 18),
                Location = new Point(55, 4),
                BackColor = Color.Transparent
            };

            Label lblValue = new Label()
            {
                Text = "---",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 40, 60),
                AutoSize = false,
                Size = new Size(340, 26),
                Location = new Point(55, 24),
                BackColor = Color.Transparent,
                Tag = "value"
            };

            row.Controls.Add(lblIcon);
            row.Controls.Add(lblTitle);
            row.Controls.Add(lblValue);
            return row;
        }

        // ─── Helpers ──────────────────────────────────────────────────
        private Label GetValueLabel(Panel row)
        {
            foreach (Control c in row.Controls)
                if (c is Label lbl && (string)lbl.Tag == "value")
                    return lbl;
            return null;
        }

        private void SetRowValue(Panel row, string text, Color? foreColor = null)
        {
            Label lbl = GetValueLabel(row);
            if (lbl == null) return;
            lbl.Text = text;
            lbl.ForeColor = foreColor ?? Color.FromArgb(30, 40, 60);
        }

        private GraphicsPath RoundedRect(Rectangle r, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(r.X, r.Y, radius, radius, 180, 90);
            path.AddArc(r.Right - radius, r.Y, radius, radius, 270, 90);
            path.AddArc(r.Right - radius, r.Bottom - radius, radius, radius, 0, 90);
            path.AddArc(r.X, r.Bottom - radius, radius, radius, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}