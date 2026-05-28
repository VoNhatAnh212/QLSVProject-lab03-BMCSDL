using QLSVProject.Forms;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace QLSVNhomApp
{
    public partial class MainForm : Form
    {
        // ─── Thông tin đăng nhập ───────────────────────────────────────
        private string manv;
        private string tendn;
        private string password;

        // ─── Controls ─────────────────────────────────────────────────
        private Panel pnlHeader;
        private Label lblTitle;
        private Label lblSubtitle;
        private Label lblWelcome;

        private Panel pnlMenu;
        private Button btnClassManagement;
        private Button btnEmployeeManagement;
        private Button btnPersonalInfo;
        private Button btnLogout;

        // ─── Constructor ──────────────────────────────────────────────
        public MainForm(string manv, string tendn, string password)
        {
            this.manv = manv;
            this.tendn = tendn;
            this.password = password;

            InitializeComponent();
        }

        // ─── InitializeComponent ──────────────────────────────────────
        private void InitializeComponent()
        {
            // ---------- FORM ----------
            this.Text = "Hệ thống Quản lý Sinh viên";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(960, 680);
            this.MinimumSize = new Size(760, 560);
            this.BackColor = Color.FromArgb(245, 247, 250);
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.FormClosed += (s, e) => Application.Exit();

            // ---------- HEADER PANEL ----------
            pnlHeader = new Panel()
            {
                Dock = DockStyle.Top,
                Height = 130,
                BackColor = Color.FromArgb(15, 52, 96)    // xanh navy đậm
            };
            pnlHeader.Paint += PnlHeader_Paint;

            lblTitle = new Label()
            {
                Text = "HỆ THỐNG QUẢN LÝ SINH VIÊN",
                Font = new Font("Segoe UI", 22, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                Height = 50,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            lblSubtitle = new Label()
            {
                Text = "Student Management System",
                Font = new Font("Segoe UI", 10, FontStyle.Italic),
                ForeColor = Color.FromArgb(180, 210, 255),
                AutoSize = false,
                Height = 25,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            lblWelcome = new Label()
            {
                Text = $"👤  Xin chào:  {tendn}   |   Mã NV: {manv}",
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                ForeColor = Color.FromArgb(200, 230, 255),
                AutoSize = false,
                Height = 25,
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblSubtitle);
            pnlHeader.Controls.Add(lblWelcome);

            // ---------- MENU CARD ----------
            pnlMenu = new Panel()
            {
                Size = new Size(460, 380),
                BackColor = Color.White,
                BorderStyle = BorderStyle.None
            };
            pnlMenu.Paint += PnlMenu_Paint;   // vẽ shadow + bo góc

            // ---------- BUTTONS ----------
            btnClassManagement = CreateMenuButton(
                text: "📚   Quản lý lớp học",
                backColor: Color.FromArgb(0, 122, 204),
                hoverColor: Color.FromArgb(30, 144, 255));
            btnClassManagement.Click += BtnClassManagement_Click;

            btnEmployeeManagement = CreateMenuButton(
                text: "👥   Quản lý nhân viên",
                backColor: Color.FromArgb(34, 139, 87),
                hoverColor: Color.FromArgb(50, 168, 110));
            btnEmployeeManagement.Click += BtnEmployeeManagement_Click;

            btnPersonalInfo = CreateMenuButton(
                text: "🪪   Thông tin cá nhân",
                backColor: Color.FromArgb(120, 80, 200),
                hoverColor: Color.FromArgb(145, 100, 230));
            btnPersonalInfo.Click += BtnPersonalInfo_Click;

            btnLogout = CreateMenuButton(
                text: "🚪   Đăng xuất",
                backColor: Color.FromArgb(210, 50, 65),
                hoverColor: Color.FromArgb(235, 75, 90));
            btnLogout.Click += BtnLogout_Click;

            // ---------- ADD CONTROLS ----------
            pnlMenu.Controls.Add(btnClassManagement);
            pnlMenu.Controls.Add(btnEmployeeManagement);
            pnlMenu.Controls.Add(btnPersonalInfo);
            pnlMenu.Controls.Add(btnLogout);

            this.Controls.Add(pnlHeader);
            this.Controls.Add(pnlMenu);

            // ---------- LAYOUT ----------
            this.Load += (s, e) => ArrangeControls();
            this.Resize += (s, e) => ArrangeControls();
        }

        // ─── Paint: header gradient ────────────────────────────────────
        private void PnlHeader_Paint(object sender, PaintEventArgs e)
        {
            using (LinearGradientBrush br = new LinearGradientBrush(
                pnlHeader.ClientRectangle,
                Color.FromArgb(15, 52, 96),
                Color.FromArgb(25, 80, 145),
                LinearGradientMode.Horizontal))
            {
                e.Graphics.FillRectangle(br, pnlHeader.ClientRectangle);
            }

            // gạch kẻ đáy header
            using (Pen pen = new Pen(Color.FromArgb(0, 180, 255), 3))
                e.Graphics.DrawLine(pen, 0, pnlHeader.Height - 3,
                                         pnlHeader.Width, pnlHeader.Height - 3);
        }

        // ─── Paint: card shadow + bo góc ──────────────────────────────
        private void PnlMenu_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = new Rectangle(4, 4,
                pnlMenu.Width - 10, pnlMenu.Height - 10);

            // vẽ shadow giả bằng nhiều rect mờ dần
            for (int i = 4; i >= 1; i--)
            {
                Rectangle shadow = new Rectangle(
                    rect.X + i, rect.Y + i,
                    rect.Width, rect.Height);
                using (SolidBrush sb = new SolidBrush(
                    Color.FromArgb(20 * i, 0, 0, 0)))
                {
                    g.FillRectangle(sb, shadow);
                }
            }

            // card trắng bo góc
            using (GraphicsPath path = RoundedRect(rect, 14))
            using (SolidBrush white = new SolidBrush(Color.White))
            {
                g.FillPath(white, path);
            }
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

        // ─── Factory: tạo button menu ─────────────────────────────────
        private Button CreateMenuButton(string text, Color backColor, Color hoverColor)
        {
            Button btn = new Button()
            {
                Text = text,
                Size = new Size(300, 54),
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0)
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.BorderColor = backColor;

            Color originalColor = backColor;
            btn.MouseEnter += (s, e) => btn.BackColor = hoverColor;
            btn.MouseLeave += (s, e) => btn.BackColor = originalColor;

            return btn;
        }

        // ─── Layout ──────────────────────────────────────────────────
        private void ArrangeControls()
        {
            int w = this.ClientSize.Width;
            int h = this.ClientSize.Height;

            // Header labels
            lblTitle.Width = w - 40;
            lblSubtitle.Width = w - 40;
            lblWelcome.Width = w - 40;

            lblTitle.Location = new Point(20, 18);
            lblSubtitle.Location = new Point(20, 65);
            lblWelcome.Location = new Point(20, 95);

            // Menu card
            int cardTop = pnlHeader.Height + 40;
            pnlMenu.Location = new Point((w - pnlMenu.Width) / 2, cardTop);

            // Buttons trong card (căn giữa theo card)
            int bx = (pnlMenu.Width - btnClassManagement.Width) / 2;

            btnClassManagement.Location = new Point(bx, 36);
            btnEmployeeManagement.Location = new Point(bx, 36 + 70);
            btnPersonalInfo.Location = new Point(bx, 36 + 140);
            btnLogout.Location = new Point(bx, 36 + 210);
        }

        // ─── Event handlers ──────────────────────────────────────────
        private void BtnClassManagement_Click(object sender, EventArgs e)
        {
            ClassManagementForm f = new ClassManagementForm(manv, password, this);
            this.Hide();
            f.ShowDialog();
            this.Show();
        }

        private void BtnEmployeeManagement_Click(object sender, EventArgs e)
        {
            EmployeeManagementForm f = new EmployeeManagementForm();
            this.Hide();
            f.ShowDialog();
            this.Show();
        }

        private void BtnPersonalInfo_Click(object sender, EventArgs e)
        {
            // Mở form thông tin cá nhân, truyền manv + password để query DB
            EmployeeInfoForm f = new EmployeeInfoForm();
            f.ShowDialog();
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Bạn có chắc muốn đăng xuất không?",
                "Xác nhận đăng xuất",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                LoginForm f = new LoginForm();
                f.Show();
                f.FormClosed += (s, args) => Application.Exit();
                this.Hide();
            }
        }
    }
}