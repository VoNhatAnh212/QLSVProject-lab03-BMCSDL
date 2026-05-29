using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using QLSVProject.Helpers;

namespace QLSVProject.Forms
{
    public partial class EmployeeManagementForm : Form
    {
        private string connectionString =
            ConfigurationManager.ConnectionStrings["QLSVNhomConnection"].ConnectionString;

        private DataGridView dgvEmployees;
        private Button btnSearch;
        private Button btnUpdate;
        private Button btnDelete;
        private Button btnRefresh;
        private Button btnNhapLuong;   // chỉ hiện với admin
        private TextBox txtSearch;
        private Label lblSearch;

        public EmployeeManagementForm()
        {
            InitializeComponent();
            ApplyPermissions();
        }

        // ================= UI =================
        private void InitializeComponent()
        {
            this.Text = "Quản lý nhân viên";
            this.Size = new Size(1200, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 247, 250);

            Label title = new Label()
            {
                Text = "QUẢN LÝ NHÂN VIÊN",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(1150, 50),
                Location = new Point(20, 10)
            };

            lblSearch = new Label()
            {
                Text = "Mã nhân viên:",
                Font = new Font("Segoe UI", 10),
                Location = new Point(20, 70),
                AutoSize = true
            };

            txtSearch = new TextBox()
            {
                Location = new Point(130, 67),
                Size = new Size(200, 30),
                Font = new Font("Segoe UI", 10)
            };

            btnSearch = CreateButton("Tìm kiếm", new Point(340, 65), Color.FromArgb(0, 122, 204));
            btnSearch.Click += BtnSearch_Click;

            dgvEmployees = new DataGridView()
            {
                Location = new Point(20, 110),
                Size = new Size(1150, 450),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D
            };

            btnUpdate = CreateButton("Cập nhật", new Point(20, 580), Color.FromArgb(0, 122, 204));
            btnDelete = CreateButton("Xóa", new Point(180, 580), Color.FromArgb(220, 53, 69));
            btnRefresh = CreateButton("Tải lại", new Point(340, 580), Color.Gray);
            btnNhapLuong = CreateButton("Nhập lương", new Point(500, 580), Color.FromArgb(40, 167, 69));
            btnNhapLuong.Visible = false; // ẩn mặc định — ApplyPermissions() sẽ mở khi là admin
            btnUpdate.Visible = false;
            btnDelete.Visible = false;

            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;
            btnRefresh.Click += (s, e) => LoadEmployees();
            btnNhapLuong.Click += BtnNhapLuong_Click;

            this.Controls.AddRange(new Control[]
            {
                title,
                lblSearch, txtSearch, btnSearch,
                dgvEmployees,
                btnUpdate, btnDelete, btnRefresh, btnNhapLuong
            });

            LoadEmployees();
        }

        public void ApplyPermissions()
        {
            bool isAdmin = (Session.MaNV == "admin");
            btnNhapLuong.Visible = isAdmin;

            btnUpdate.Visible = isAdmin;
            btnDelete.Visible = isAdmin;
            btnNhapLuong.Visible = isAdmin;
        }

        private Button CreateButton(string text, Point location, Color backColor)
        {
            return new Button()
            {
                Text = text,
                Size = new Size(140, 40),
                Location = location,
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
        }

        // ================= LOAD =================
        private void LoadEmployees()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SP_SEL_PUBLIC_INFOR_NHANVIEN", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MANV", DBNull.Value);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvEmployees.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
            }
        }

        // ================= TÌM KIẾM =================
        private void BtnSearch_Click(object sender, EventArgs e)
        {
            string maNV = txtSearch.Text.Trim();

            if (string.IsNullOrWhiteSpace(maNV))
            {
                LoadEmployees();
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SP_SEL_PUBLIC_INFOR_NHANVIEN", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MANV", maNV);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("Không tìm thấy nhân viên!", "Thông báo",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }

                    dgvEmployees.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tìm kiếm: " + ex.Message);
            }
        }

        // ================= INPUT BOX CUSTOM =================
        private string Prompt(string title, string text, string defaultValue = "")
        {
            Form f = new Form();
            f.Width = 350;
            f.Height = 180;
            f.Text = title;
            f.StartPosition = FormStartPosition.CenterScreen;

            Label lbl = new Label() { Left = 20, Top = 20, Text = text, Width = 280 };
            TextBox txt = new TextBox() { Left = 20, Top = 50, Width = 280 };
            txt.Text = defaultValue;

            Button ok = new Button() { Text = "OK", Left = 120, Width = 80, Top = 90 };
            ok.DialogResult = DialogResult.OK;

            f.Controls.Add(lbl);
            f.Controls.Add(txt);
            f.Controls.Add(ok);
            f.AcceptButton = ok;

            return f.ShowDialog() == DialogResult.OK ? txt.Text : "";
        }

        // ================= CẬP NHẬT =================
        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvEmployees.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn nhân viên cần cập nhật!");
                return;
            }

            var row = dgvEmployees.SelectedRows[0];

            string maNV = row.Cells["MANV"].Value.ToString();
            string hotenOld = row.Cells["HOTEN"].Value.ToString();
            string emailOld = row.Cells["EMAIL"].Value.ToString();

            string hoten = Prompt("Cập nhật nhân viên", "Họ tên:", hotenOld);
            string email = Prompt("Cập nhật nhân viên", "Email:", emailOld);

            if (string.IsNullOrWhiteSpace(hoten) || string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("Không được để trống!");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SP_UPD_PUBLIC_NHANVIEN", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@MANV", maNV);
                    cmd.Parameters.AddWithValue("@HOTEN", hoten);
                    cmd.Parameters.AddWithValue("@EMAIL", email);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Cập nhật thành công!");
                LoadEmployees();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật: " + ex.Message);
            }
        }

        // ================= XÓA =================
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvEmployees.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn nhân viên cần xóa!");
                return;
            }

            string maNV = dgvEmployees.SelectedRows[0].Cells["MANV"].Value.ToString();
            string hoTen = dgvEmployees.SelectedRows[0].Cells["HOTEN"].Value.ToString();

            DialogResult confirm = MessageBox.Show(
                $"Bạn có chắc muốn xóa nhân viên '{hoTen}'?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SP_DEL_PUBLIC_NHANVIEN", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MANV", maNV);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Xóa nhân viên thành công!");
                LoadEmployees();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Không thể xóa:\n" + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hệ thống:\n" + ex.Message);
            }
        }

        // ================= NHẬP LƯƠNG =================
        private void BtnNhapLuong_Click(object sender, EventArgs e)
        {
            // 1. Kiểm tra quyền admin (bảo vệ kép: nút đã ẩn, nhưng vẫn guard ở đây)
            if (Session.MaNV != "admin")
            {
                MessageBox.Show(
                    "Chỉ tài khoản 'admin' mới được phép nhập lương!",
                    "Từ chối quyền truy cập",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Stop);
                return;
            }

            // 2. Phải chọn một hàng trong grid
            if (dgvEmployees.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn nhân viên cần nhập lương!");
                return;
            }

            var row = dgvEmployees.SelectedRows[0];
            string maNV = row.Cells["MANV"].Value.ToString();
            string hoTen = row.Cells["HOTEN"].Value.ToString();

            // 3. Nhập giá trị lương qua hộp thoại Prompt (giống phong cách cũ)
            string luongStr = Prompt("Nhập lương", $"Lương mới cho '{hoTen}':");

            if (string.IsNullOrWhiteSpace(luongStr))
                return; // người dùng bấm Cancel hoặc để trống

            if (!int.TryParse(luongStr.Trim(), out int luong) || luong < 0)
            {
                MessageBox.Show("Lương không hợp lệ! Vui lòng nhập số nguyên dương.",
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 4. Lấy public key của nhân viên qua SP_GET_PUBLICKEY
            string pubKeyXml = GetPublicKey(maNV);
            if (string.IsNullOrEmpty(pubKeyXml))
            {
                MessageBox.Show(
                    $"Không tìm thấy public key của nhân viên '{hoTen}'.\n" +
                    "Không thể mã hóa lương — hãy đảm bảo nhân viên đã có PUBKEY.",
                    "Thiếu public key",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            // 5. Mã hóa lương phía client bằng RSA-OAEP (SHA-256)
            byte[] encLuong = RsaEncrypt(pubKeyXml, luong);
            if (encLuong == null)
            {
                MessageBox.Show("Mã hóa thất bại! Public key có thể không hợp lệ.",
                    "Lỗi mã hóa", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            // 6. Gọi SP_UPD_LUONG với bản mã
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SP_UPD_LUONG", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@MANV_CALLER", Session.MaNV); // "admin"
                    cmd.Parameters.AddWithValue("@MANV_TARGET", maNV);

                    // Truyền VARBINARY — dùng SqlDbType.VarBinary tránh lỗi type inference
                    SqlParameter pLuong = cmd.Parameters.Add("@LUONG_ENC", SqlDbType.VarBinary, -1);
                    pLuong.Value = encLuong;

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show($"Đã cập nhật lương (đã mã hóa) cho nhân viên '{hoTen}'!",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadEmployees(); // làm mới grid (lương không hiện ra vì SP không trả cột LUONG)
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Lỗi SQL: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hệ thống: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GetPublicKey(string maNV) {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SP_GET_PUBLICKEY", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MANV", maNV);

                    object result = cmd.ExecuteScalar();

                    if (result == null || result == DBNull.Value)
                        return null;

                    byte[] publicKeyBytes = (byte[])result;

                    return Encoding.UTF8.GetString(publicKeyBytes);
                }
            }
            catch
            {
                return null;
            }
        }

        private static byte[] RsaEncrypt(string rsaPublicKeyXml, int value) {
            try
            {
                using (RSA rsa = new RSACng())
                {
                    rsa.FromXmlString(rsaPublicKeyXml);

                    byte[] data = BitConverter.GetBytes(value);

                    return rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA256);
                }
            }
            catch
            {
                return null;
            }
        }
    }
}