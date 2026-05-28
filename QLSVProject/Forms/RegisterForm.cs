using QLSVNhomApp;
using QLSVProject.Helpers;
using System;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace QLSVProject
{
    internal class RegisterForm : Form
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["QLSVNhomConnection"].ConnectionString;

        // ================= CONTROLS =================
        private TextBox txtMaNV;
        private TextBox txtHoTen;
        private TextBox txtEmail;
        private TextBox txtLuong;
        private TextBox txtUsername;
        private TextBox txtPassword;

        private Button btnRegister;

        public RegisterForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // ================= FORM =================
            this.Text = "Đăng ký tài khoản";
            this.Size = new Size(500, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            // ================= TABLE =================
            TableLayoutPanel table = new TableLayoutPanel()
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 9,
                Padding = new Padding(40),
            };

            // ================= ROW STYLE =================
            for (int i = 0; i < 7; i++)
            {
                table.RowStyles.Add(
                    new RowStyle(SizeType.Absolute, 55)
                );
            }

            table.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 70)
            );

            // ================= TITLE =================
            Label lblTitle = new Label()
            {
                Text = "ĐĂNG KÝ NHÂN VIÊN",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };

            // ================= INPUT =================
            txtMaNV = CreateTextBox("Mã nhân viên");
            txtHoTen = CreateTextBox("Họ tên");
            txtEmail = CreateTextBox("Email");
            txtLuong = CreateTextBox("Lương");
            txtUsername = CreateTextBox("Tên đăng nhập");
            txtPassword = CreateTextBox("Mật khẩu");
            txtPassword.UseSystemPasswordChar = true;

            // ================= BUTTON =================
            btnRegister = new Button()
            {
                Text = "Đăng ký",
                Dock = DockStyle.Fill,
                Height = 45,
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            btnRegister.FlatAppearance.BorderSize = 0;

            btnRegister.Click += BtnRegister_Click;

            // ================= ADD CONTROLS =================
            table.Controls.Add(lblTitle, 0, 0);
            table.Controls.Add(txtMaNV, 0, 1);
            table.Controls.Add(txtHoTen, 0, 2);
            table.Controls.Add(txtEmail, 0, 3);
            table.Controls.Add(txtLuong, 0, 4);
            table.Controls.Add(txtUsername, 0, 5);
            table.Controls.Add(txtPassword, 0, 6);
            table.Controls.Add(btnRegister, 0, 7);

            this.Controls.Add(table);
        }

        // ================= CREATE TEXTBOX =================
        private TextBox CreateTextBox(string placeholder)
        {
            TextBox txt = new TextBox()
            {
                Text = placeholder,
                ForeColor = Color.Gray,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10),
                Margin = new Padding(0, 5, 0, 5)
            };

            txt.Enter += (s, e) =>
            {
                if (txt.Text == placeholder)
                {
                    txt.Text = "";
                    txt.ForeColor = Color.Black;
                }
            };

            txt.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txt.Text))
                {
                    txt.Text = placeholder;
                    txt.ForeColor = Color.Gray;
                }
            };

            return txt;
        }

        // ================= REGISTER =================
        private void BtnRegister_Click(object sender, EventArgs e)
        {
            string manv = txtMaNV.Text.Trim();
            string hoten = txtHoTen.Text.Trim();
            string email = txtEmail.Text.Trim();
            string luongtxt = txtLuong.Text.Trim();
            int luong = int.Parse(luongtxt);
            string tendn = txtUsername.Text.Trim();
            string matkhau = txtPassword.Text.Trim();

            // ================= VALIDATE =================
            if (
                string.IsNullOrWhiteSpace(manv) ||
                string.IsNullOrWhiteSpace(hoten) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(tendn) ||
                string.IsNullOrWhiteSpace(matkhau)
            )
            {
                MessageBox.Show(
                    "Vui lòng nhập đầy đủ thông tin!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            try
            {
                // ================= RSA =================
                var rsa = RsaHelper.CreateRSA(tendn + matkhau);
                var publicKey = Encoding.UTF8.GetBytes(rsa.ToXmlString(false));

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("SP_INS_PUBLIC_NHANVIEN", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@MANV", manv);
                        cmd.Parameters.AddWithValue("@HOTEN", hoten);
                        cmd.Parameters.AddWithValue("@EMAIL", email);

                        byte[] encryptedLuoong = RsaHelper.Encrypt(rsa, BitConverter.GetBytes(luong)); 
                        cmd.Parameters.AddWithValue("@LUONGCB", encryptedLuoong);
                        cmd.Parameters.AddWithValue("@TENDN", tendn);

                        byte[] hashedMK;
                        using (SHA256 sha256 = SHA256.Create())
                        {
                            hashedMK = sha256.ComputeHash(Encoding.UTF8.GetBytes(matkhau));
                        }

                        cmd.Parameters.AddWithValue("@MK", hashedMK);
                        cmd.Parameters.AddWithValue("@PUBKEY", publicKey);

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Đăng ký thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.Close();
                //LoginForm login = new LoginForm();

                //this.Hide();
                //login.ShowDialog();
                //this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}