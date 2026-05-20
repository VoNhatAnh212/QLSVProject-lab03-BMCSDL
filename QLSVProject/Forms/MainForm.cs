using System;
using System.Drawing;
using System.Windows.Forms;

namespace QLSVNhomApp
{
    public partial class MainForm : Form
    {
        private string manv;
        private string tendn;
        private string password;

        private Label lblTitle;
        private Label lblWelcome;
        private Panel pnlMenu;

        private Button btnClassManagement;
        private Button btnLogout;

        public MainForm(string manv, string tendn, string password)
        {
            this.manv = manv;
            this.tendn = tendn;
            this.password = password;

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // ================= FORM =================
            this.Text = "Hệ thống Quản lý Sinh viên";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(900, 650);
            this.MinimumSize = new Size(700, 500);
            this.BackColor = Color.FromArgb(236, 242, 248);
            this.FormClosed += (s, e) => Application.Exit();

            // ================= TITLE =================
            lblTitle = new Label()
            {
                Text = "HỆ THỐNG QUẢN LÝ SINH VIÊN",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 70, 140),
                AutoSize = false,
                Size = new Size(700, 45),
                TextAlign = ContentAlignment.MiddleCenter
            };

            lblWelcome = new Label()
            {
                Text = $"Xin chào: {tendn}",
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = Color.FromArgb(70, 70, 70),
                AutoSize = false,
                Size = new Size(500, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // ================= PANEL =================
            pnlMenu = new Panel()
            {
                Size = new Size(420, 260),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // ================= BUTTON QUẢN LÝ LỚP =================
            btnClassManagement = new Button()
            {
                Text = "Quản lý lớp học",
                Size = new Size(260, 50),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnClassManagement.FlatAppearance.BorderSize = 0;
            btnClassManagement.Click += BtnClassManagement_Click;

            // Hover effect
            btnClassManagement.MouseEnter += (s, e) =>
                btnClassManagement.BackColor = Color.FromArgb(30, 144, 255);

            btnClassManagement.MouseLeave += (s, e) =>
                btnClassManagement.BackColor = Color.FromArgb(0, 122, 204);

            // ================= BUTTON LOGOUT =================
            btnLogout = new Button()
            {
                Text = "Đăng xuất",
                Size = new Size(260, 50),
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.Click += BtnLogout_Click;

            btnLogout.MouseEnter += (s, e) =>
                btnLogout.BackColor = Color.FromArgb(240, 80, 95);

            btnLogout.MouseLeave += (s, e) =>
                btnLogout.BackColor = Color.FromArgb(220, 53, 69);

            // ================= ADD CONTROLS PANEL =================
            pnlMenu.Controls.Add(btnClassManagement);
            pnlMenu.Controls.Add(btnLogout);

            this.Controls.Add(lblTitle);
            this.Controls.Add(lblWelcome);
            this.Controls.Add(pnlMenu);

            // ================= LAYOUT =================
            this.Load += (s, e) => ArrangeControls();
            this.Resize += (s, e) => ArrangeControls();
        }

        private void ArrangeControls()
        {
            lblTitle.Location =
                new Point((this.ClientSize.Width - lblTitle.Width) / 2, 50);

            lblWelcome.Location =
                new Point((this.ClientSize.Width - lblWelcome.Width) / 2, 105);

            pnlMenu.Location =
                new Point((this.ClientSize.Width - pnlMenu.Width) / 2, 180);

            btnClassManagement.Location =
                new Point((pnlMenu.Width - btnClassManagement.Width) / 2, 55);

            btnLogout.Location =
                new Point((pnlMenu.Width - btnLogout.Width) / 2, 135);
        }

        private void BtnClassManagement_Click(object sender, EventArgs e)
        {
            ClassManagementForm f =
                new ClassManagementForm(manv, password, this);

            this.Hide();
            f.ShowDialog();
            this.Show();
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            LoginForm f = new LoginForm();

            f.Show();
            f.FormClosed += (s, args) => Application.Exit();

            this.Hide();
        }
    }
}