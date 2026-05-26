//using System;
//using System.Text;
//using System.Configuration;
//using System.Windows.Forms;
//using System.Drawing;
//using System.Security.Cryptography;
//using System.Data.SqlClient;
//using System.Data;
//using System.IO;
//using QLSVNhomApp;
//using System.Security.Cryptography.X509Certificates;
//namespace QLSVProject.Forms
//{
//    internal class RegisterForm : Form // Inherit from Form to access Form properties like Size
//    {
//        private string connectionString = ConfigurationManager.ConnectionStrings["QLSVNhomConnection"].ConnectionString;
//        private Label lblMaNV;
//        private Label lblHoTen;
//        private Label lblEmail;
//        private Label lblLuong;
//        private Label lblUsername;
//        private Label lblPassword;
//        private TextBox txtMaNV;
//        private TextBox txtHoTen;
//        private TextBox txtEmail;
//        private TextBox txtLuong;
//        private TextBox txtUsername;
//        private TextBox txtPassword;
//        private Button btnRegister;
//        private TextBox txtMaTrgPhong;
//        private Label lblMaTrgPhong;

//        public RegisterForm()
//        {
//            InitializeComponent();
//        }

//        private void InitializeComponent()
//        {
//            // Form settings
//            this.Text = "Đăng ký tài khoản";
//            this.Size = new Size(400, 500); // Size is a property of Form
//            this.StartPosition = FormStartPosition.CenterScreen;
//            this.BackColor = Color.FromArgb(240, 244, 248);

//            // Label Mã nhân viên
//            lblMaNV = new Label
//            {
//                Text = "Mã nhân viên:",
//                Font = new Font("Segoe UI", 10),
//                Location = new Point(50, 50),
//                Size = new Size(120, 25)
//            };

//            // TextBox Mã nhân viên
//            txtMaNV = new TextBox
//            {
//                Location = new Point(200, 50),
//                Size = new Size(150, 25)
//            };

//            // Label Họ tên
//            lblHoTen = new Label
//            {
//                Text = "Họ tên:",
//                Font = new Font("Segoe UI", 10),
//                Location = new Point(50, 100),
//                Size = new Size(120, 25)
//            };

//            // TextBox Họ tên
//            txtHoTen = new TextBox
//            {
//                Location = new Point(200, 100),
//                Size = new Size(150, 25)
//            };

//            // Label Email
//            lblEmail = new Label
//            {
//                Text = "Email:",
//                Font = new Font("Segoe UI", 10),
//                Location = new Point(50, 150),
//                Size = new Size(120, 25)
//            };

//            // TextBox Email
//            txtEmail = new TextBox
//            {
//                Location = new Point(200, 150),
//                Size = new Size(150, 25)
//            };

//            // Label Lương
//            lblLuong = new Label
//            {
//                Text = "Lương:",
//                Font = new Font("Segoe UI", 10),
//                Location = new Point(50, 200),
//                Size = new Size(120, 25)
//            };

//            // TextBox Lương
//            txtLuong = new TextBox
//            {
//                Location = new Point(200, 200),
//                Size = new Size(150, 25)
//            };
//            lblMaTrgPhong = new Label
//            {
//                Text = "Mã Trưởng Phòng:",
//                Font = new Font("Segoe UI", 10),
//                Location = new Point(50, 250),
//                Size = new Size(120, 25)
//            };
//            // TextBox Mã Trưởng Phòng
//            txtMaTrgPhong = new TextBox
//            {
//                Location = new Point(200, 250),
//                Size = new Size(150, 25)
//            };

//            // Label Tên đăng nhập
//            lblUsername = new Label
//            {
//                Text = "Tên đăng nhập:",
//                Font = new Font("Segoe UI", 10),
//                Location = new Point(50, 300),
//                Size = new Size(120, 25)
//            };
//            // TextBox Tên đăng nhập
//            txtUsername = new TextBox
//            {
//                Location = new Point(200, 300),
//                Size = new Size(150, 25)
//            };
//            // Label Mật khẩu
//            lblPassword = new Label
//            {
//                Text = "Mật khẩu:",
//                Font = new Font("Segoe UI", 10),
//                Location = new Point(50, 350),
//                Size = new Size(120, 25)
//            };
//            // TextBox Mật khẩu
//            txtPassword = new TextBox
//            {
//                Location = new Point(200, 350),
//                Size = new Size(150, 25),
//                PasswordChar = '*'
//            };
//            // Button Đăng ký
//            btnRegister = new Button
//            {
//                Text = "Đăng ký",
//                Location = new Point(150, 400),
//                Size = new Size(100, 30),
//                BackColor = Color.FromArgb(0, 123, 255),
//                ForeColor = Color.White,
//                FlatStyle = FlatStyle.Flat
//            };
//            btnRegister.FlatAppearance.BorderSize = 0;
//            btnRegister.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 105, 217);
//            btnRegister.Click += new EventHandler(btnRegister_Click);
//            // Add controls to the form
//            this.Controls.Add(lblMaNV);
//            this.Controls.Add(txtMaNV);
//            this.Controls.Add(lblHoTen);
//            this.Controls.Add(txtHoTen);
//            this.Controls.Add(lblEmail);
//            this.Controls.Add(txtEmail);
//            this.Controls.Add(lblLuong);
//            this.Controls.Add(txtLuong);
//            this.Controls.Add(lblMaTrgPhong);
//            this.Controls.Add(txtMaTrgPhong);
//            this.Controls.Add(lblUsername);
//            this.Controls.Add(txtUsername);
//            this.Controls.Add(lblPassword);
//            this.Controls.Add(txtPassword);
//            this.Controls.Add(btnRegister);

//        }

//        private (string publicKeyXml, string privateKeyXml) GenerateRSAKeyPair()
//        {
//            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(2048))
//            {
//                string publicKey = rsa.ToXmlString(false); // public key
//                string privateKey = rsa.ToXmlString(true); // private key
//                return (publicKey, privateKey);
//            }
//        }
//        private void btnRegister_Click(object sender, EventArgs e)
//        {
//            string manv = txtMaNV.Text;
//            string hoten = txtHoTen.Text;
//            string email = txtEmail.Text;
//            string luong = txtLuong.Text;


//            string tendn = txtUsername.Text;
//            string matkhau = txtPassword.Text;
//            string matTrgPhong = txtMaTrgPhong.Text;

//                // Fix for CS0019: Operator '==' cannot be applied to operands of type 'string' and 'char'
//                if (matTrgPhong.Length == 0) // Compare the length as an integer
//                {
//                    matTrgPhong = null;
//                }
//            //MessageBox.Show(matTrgPhong);

//            //// Băm mật khẩu
//            //string hashedPassword = Helpers.SecurityHelper.HashPasswordSHA1(matkhau);
//            //// Tạo cặp khóa RSA
//            //var (publicKeyXml, privateKeyXml) = GenerateRSAKeyPair();
//            //privateKeyXml = Helpers.SecurityHelper.encryptPrivateWithPassword(privateKeyXml, matkhau); // Mã hóa private key bằng mật khẩu (mã nhân viên)
//            //// Lưu private key vào file

//            //Helpers.SecurityHelper.SavePrivateKeyToFile(privateKeyXml, manv);

//            //string encryptedLuong = Helpers.SecurityHelper.EncryptWithPublicKey(luong, publicKeyXml);
//            // Gửi dữ liệu lên SQL Server
//            try
//            {
//                using (SqlConnection conn = new SqlConnection(connectionString))
//                {
//                    conn.Open();
//                    using (SqlCommand cmd = new SqlCommand("SP_INS_PUBLIC_NHANVIEN", conn))
//                    {
//                        cmd.CommandType = CommandType.StoredProcedure;
//                        cmd.Parameters.AddWithValue("@MANV", manv);
//                        cmd.Parameters.AddWithValue("@HOTEN", hoten);
//                        cmd.Parameters.AddWithValue("@EMAIL", email);
//                        cmd.Parameters.AddWithValue("@LUONGCB", luong);
//                        cmd.Parameters.AddWithValue("@TENDN", tendn);
//                        cmd.Parameters.AddWithValue("@MK", matkhau);
//                        //cmd.Parameters.AddWithValue("@PUB", publicKeyXml);
//                        //if (string.IsNullOrEmpty(matTrgPhong))
//                        //{
//                        //    cmd.Parameters.AddWithValue("@TRGPHONG", DBNull.Value); // Thêm tham số mã trưởng phòng nếu không có
//                        //}
//                        //else
//                        //    cmd.Parameters.AddWithValue("@TRGPHONG", matTrgPhong); // Thêm tham số mã trưởng phòng

//                        cmd.ExecuteNonQuery();
//                        MessageBox.Show("Đăng ký thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
//                        var login = new LoginForm();
//                        this.Hide();
//                        login.ShowDialog();


//                    }

//                }
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Lỗi khi đăng ký: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
//            }
//        }





//    }
//}


//using System;
//using System.Drawing;
//using System.Windows.Forms;

//public class RegisterForm : Form
//{
//    TextBox txtFullName, txtEmail, txtUsername, txtPassword;
//    Button btnRegister;

//    public RegisterForm()
//    {
//        InitializeComponent();
//    }

//    private void InitializeComponent()
//    {
//        this.Text = "Đăng ký";
//        this.Size = new Size(420, 420);
//        this.StartPosition = FormStartPosition.CenterScreen;
//        this.BackColor = Color.White;

//        // ================= TABLE =================
//        TableLayoutPanel table = new TableLayoutPanel()
//        {
//            RowCount = 6,
//            ColumnCount = 1,
//            Dock = DockStyle.Fill,
//            Padding = new Padding(40),
//        };

//        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 50)); // title
//        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
//        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
//        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
//        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 50));
//        table.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));

//        // ================= TITLE =================
//        Label lblTitle = new Label()
//        {
//            Text = "ĐĂNG KÝ",
//            Font = new Font("Segoe UI", 14, FontStyle.Bold),
//            TextAlign = ContentAlignment.MiddleCenter,
//            Dock = DockStyle.Fill
//        };

//        // ================= INPUTS =================
//        txtFullName = CreateBox("Họ tên");
//        txtEmail = CreateBox("Email");
//        txtUsername = CreateBox("Tên đăng nhập");
//        txtPassword = CreateBox("Mật khẩu");
//        txtPassword.UseSystemPasswordChar = true;

//        // ================= BUTTON =================
//        btnRegister = new Button()
//        {
//            Text = "Đăng ký",
//            Dock = DockStyle.Fill,
//            BackColor = Color.FromArgb(0, 122, 204),
//            ForeColor = Color.White,
//            FlatStyle = FlatStyle.Flat
//        };

//        btnRegister.Click += BtnRegister_Click;

//        // ================= ADD =================
//        table.Controls.Add(lblTitle, 0, 0);
//        table.Controls.Add(txtFullName, 0, 1);
//        table.Controls.Add(txtEmail, 0, 2);
//        table.Controls.Add(txtUsername, 0, 3);
//        table.Controls.Add(txtPassword, 0, 4);
//        table.Controls.Add(btnRegister, 0, 5);

//        this.Controls.Add(table);
//    }

//    // ================= TEXTBOX FACTORY =================
//    private TextBox CreateBox(string placeholder)
//    {
//        TextBox txt = new TextBox()
//        {
//            Text = placeholder,
//            ForeColor = Color.Gray,
//            Dock = DockStyle.Fill
//        };

//        txt.Enter += (s, e) =>
//        {
//            if (txt.Text == placeholder)
//            {
//                txt.Text = "";
//                txt.ForeColor = Color.Black;
//            }
//        };

//        txt.Leave += (s, e) =>
//        {
//            if (string.IsNullOrWhiteSpace(txt.Text))
//            {
//                txt.Text = placeholder;
//                txt.ForeColor = Color.Gray;
//            }
//        };

//        return txt;
//    }

//    // ================= REGISTER =================
//    private void BtnRegister_Click(object sender, EventArgs e)
//    {
//        MessageBox.Show("Đăng ký thành công!");
//    }
//}

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