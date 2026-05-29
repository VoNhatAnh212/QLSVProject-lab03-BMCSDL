using Microsoft.Data.SqlClient;
using QLSVProject;
using QLSVProject.Helpers;
using System;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace QLSVNhomApp
{
    public partial class LoginForm : Form
    {
        private string connectionString =
            ConfigurationManager.ConnectionStrings["QLSVNhomConnection"].ConnectionString;

        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnTogglePassword;

        private Label lblTitle;
        private Panel panelLogin;

        private bool isPasswordVisible = false;

        public LoginForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // ================= FORM =================
            this.Text = "Đăng nhập hệ thống";
            this.Size = new Size(520, 420);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(235, 240, 245);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // ================= TITLE =================
            lblTitle = new Label()
            {
                Text = "QUẢN LÝ SINH VIÊN",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 30, 30),
                Size = new Size(400, 40),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(60, 30)
            };

            // ================= PANEL =================
            panelLogin = new Panel()
            {
                Size = new Size(380, 250),
                Location = new Point(60, 90),
                BackColor = Color.White
            };

            // ================= USERNAME =================
            txtUsername = new TextBox()
            {
                Location = new Point(40, 40),
                Size = new Size(300, 35),
                Font = new Font("Segoe UI", 10),
                Text = "Tên đăng nhập",
                ForeColor = Color.Gray
            };

            txtUsername.Enter += (s, e) =>
            {
                if (txtUsername.Text == "Tên đăng nhập")
                {
                    txtUsername.Text = "";
                    txtUsername.ForeColor = Color.Black;
                }
            };

            txtUsername.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtUsername.Text))
                {
                    txtUsername.Text = "Tên đăng nhập";
                    txtUsername.ForeColor = Color.Gray;
                }
            };

            // ================= PASSWORD =================
            txtPassword = new TextBox()
            {
                Location = new Point(40, 100),
                Size = new Size(260, 35),
                Font = new Font("Segoe UI", 10),
                Text = "Mật khẩu",
                ForeColor = Color.Gray,
                UseSystemPasswordChar = false
            };

            txtPassword.Enter += (s, e) =>
            {
                if (txtPassword.Text == "Mật khẩu")
                {
                    txtPassword.Text = "";
                    txtPassword.ForeColor = Color.Black;
                    txtPassword.UseSystemPasswordChar = true;
                }
            };

            txtPassword.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    txtPassword.Text = "Mật khẩu";
                    txtPassword.ForeColor = Color.Gray;
                    txtPassword.UseSystemPasswordChar = false;
                }
            };

            // ================= TOGGLE PASSWORD =================
            btnTogglePassword = new Button()
            {
                Text = "👁",
                Size = new Size(40, 35),
                Location = new Point(305, 100),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                Cursor = Cursors.Hand
            };
            btnTogglePassword.FlatAppearance.BorderSize = 1;

            btnTogglePassword.Click += (s, e) =>
            {
                isPasswordVisible = !isPasswordVisible;

                if (txtPassword.Text != "Mật khẩu")
                    txtPassword.UseSystemPasswordChar = !isPasswordVisible;

                btnTogglePassword.Text = isPasswordVisible ? "🙈" : "👁";
            };

            // ================= LOGIN BUTTON =================
            btnLogin = new Button()
            {
                Text = "Đăng nhập",
                Size = new Size(300, 40),
                Location = new Point(40, 160),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            btnLogin.FlatAppearance.BorderSize = 0;

            btnLogin.MouseEnter += (s, e) =>
                btnLogin.BackColor = Color.FromArgb(30, 144, 255);

            btnLogin.MouseLeave += (s, e) =>
                btnLogin.BackColor = Color.FromArgb(0, 122, 204);

            btnLogin.Click += BtnLogin_Click;

            // Register link 
            Label lblRegisterText = new Label()
            {
                Text = "Bạn chưa có tài khoản?",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(90, 215)
            };

            LinkLabel lnkRegister = new LinkLabel()
            {
                Text = "Đăng ký tại đây",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(250, 215),
                LinkColor = Color.FromArgb(0, 122, 204),
                Cursor = Cursors.Hand
            };

            lnkRegister.LinkClicked += LinkLabelRegister_LinkClicked;

            panelLogin.Controls.Add(lblRegisterText);
            panelLogin.Controls.Add(lnkRegister);

            // ================= ADD CONTROL =================
            panelLogin.Controls.AddRange(new Control[]
            {
                txtUsername,
                txtPassword,
                btnTogglePassword,
                btnLogin
            });

            this.Controls.AddRange(new Control[]
            {
                lblTitle,
                panelLogin
            });
        }

        // ================= LOGIN =================
        private void BtnLogin_Click(object sender, EventArgs e)
        {
            if (txtUsername.Text == "Tên đăng nhập" || txtPassword.Text == "Mật khẩu")
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin.");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("SP_SEL_PUBLIC_ENCRYPT_NHANVIEN", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@TENDN", txtUsername.Text);

                        byte[] hashedMK;
                        using (SHA256 sha256 = SHA256.Create())
                        {
                            hashedMK = sha256.ComputeHash(Encoding.UTF8.GetBytes(txtPassword.Text.Trim()));
                        }
                        cmd.Parameters.AddWithValue("@MK",hashedMK);

                        string tendn = txtUsername.Text.Trim();
                        string mk = txtPassword.Text.Trim();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                Session.RsaKey = RsaHelper.CreateRSA(tendn + mk);

                                string manv = reader["MANV"].ToString();

                                Session.MaNV = reader["MANV"].ToString();
                                Session.HoTen = reader["HOTEN"].ToString();
                                Session.Email = reader["EMAIL"].ToString();
                                Session.Luong = BitConverter.ToInt32(RsaHelper.Decrypt(Session.RsaKey, (byte[])reader["LUONG"]), 0);

                                MessageBox.Show("Đăng nhập thành công!");

                                var mainForm = new MainForm(manv, txtUsername.Text, txtPassword.Text);

                                mainForm.FormClosed += (s, args) => Application.Exit();
                                mainForm.Show();
                                this.Hide();
                            }
                            else
                            {
                                MessageBox.Show("Sai tài khoản hoặc mật khẩu.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // ================ Register ================
        private void LinkLabelRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RegisterForm registerForm = new RegisterForm();

            this.Hide();
            registerForm.ShowDialog(); // modal

            this.Show();
        }
    }
}