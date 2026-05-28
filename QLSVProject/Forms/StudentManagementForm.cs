using System;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace QLSVNhomApp
{
    public partial class StudentManagementForm : Form
    {
        private string connectionString =
            ConfigurationManager.ConnectionStrings["QLSVNhomConnection"].ConnectionString;

        private string classManv;
        private string malop;
        private string manv;
        private string password;

        private ClassManagementForm classManagementForm;

        private DataGridView dgvStudents;

        private Button btnAdd;
        private Button btnUpdate;
        private Button btnDelete;
        private Button btnManageScores;
        private Button btnRefresh;

        public StudentManagementForm(string classManv, string malop,
            string manv, string password,
            ClassManagementForm classManagementForm)
        {
            this.classManv = classManv;
            this.malop = malop;
            this.manv = manv;
            this.password = password;
            this.classManagementForm = classManagementForm;

            InitializeComponent();
            this.FormClosed += (s, e) => classManagementForm.Show();
        }

        // ================= UI =================
        private void InitializeComponent()
        {
            this.Text = "Quản lý sinh viên";
            this.Size = new Size(1200, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 247, 250);

            Label title = new Label()
            {
                Text = "QUẢN LÝ SINH VIÊN",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(1150, 50),
                Location = new Point(20, 10)
            };

            dgvStudents = new DataGridView()
            {
                Location = new Point(20, 70),
                Size = new Size(1150, 450),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.Fixed3D
            };

            btnAdd = CreateButton("➕ Thêm", new Point(20, 550), Color.FromArgb(0, 122, 204));
            btnUpdate = CreateButton("✏️ Sửa", new Point(180, 550), Color.FromArgb(0, 122, 204));
            btnDelete = CreateButton("🗑️ Xóa", new Point(340, 550), Color.FromArgb(220, 53, 69));
            btnManageScores = CreateButton("📊 Nhập điểm", new Point(500, 550), Color.FromArgb(40, 167, 69));
            btnRefresh = CreateButton("🔄 Tải lại", new Point(660, 550), Color.Gray);

            btnAdd.Click += BtnAdd_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;
            btnManageScores.Click += BtnManageScores_Click;
            btnRefresh.Click += (s, e) => LoadStudents();

            this.Controls.AddRange(new Control[]
            {
                title,
                dgvStudents,
                btnAdd, btnUpdate, btnDelete, btnManageScores, btnRefresh
            });

            LoadStudents();
        }

        private Button CreateButton(string text, Point location, Color backColor)
        {
            Button btn = new Button()
            {
                Text = text,
                Size = new Size(140, 45),
                Location = location,
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        // ================= LOAD =================
        private void LoadStudents()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SP_SEL_PUBLIC_SINHVIEN", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@MALOP", malop);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvStudents.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu:\n" + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ================= THÊM =================
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            // Mở form thêm mới — các ô trống, có placeholder
            using (StudentFormDialog dlg = new StudentFormDialog())
            {
                if (dlg.ShowDialog() != DialogResult.OK) return;

                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        SqlCommand cmd = new SqlCommand("SP_INS_PUBLIC_SINHVIEN", conn);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@MASV", dlg.MaSV);
                        cmd.Parameters.AddWithValue("@HOTEN", dlg.HoTen);
                        cmd.Parameters.AddWithValue("@NGAYSINH", DateTime.Parse(dlg.NgaySinh));
                        cmd.Parameters.AddWithValue("@DIACHI", dlg.DiaChi);
                        cmd.Parameters.AddWithValue("@MALOP", malop);
                        cmd.Parameters.AddWithValue("@TENDN", dlg.TenDN);
                        cmd.Parameters.AddWithValue("@MK", dlg.MatKhau);
                        cmd.Parameters.AddWithValue("@MANV", manv);

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Thêm sinh viên thành công!",
                        "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadStudents();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("SQL Error:\n" + ex.Message,
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi:\n" + ex.Message,
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ================= CẬP NHẬT =================
        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (dgvStudents.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn sinh viên cần sửa!",
                    "Chưa chọn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var row = dgvStudents.SelectedRows[0];

            string masv = row.Cells["MASV"].Value?.ToString() ?? "";
            string hoten = row.Cells["HOTEN"].Value?.ToString() ?? "";
            string ngaysinh = row.Cells["NGAYSINH"].Value?.ToString() ?? "";
            string diachi = row.Cells["DIACHI"].Value?.ToString() ?? "";

            // Format ngày cho đúng yyyy-MM-dd nếu cần
            if (DateTime.TryParse(ngaysinh, out DateTime dt))
                ngaysinh = dt.ToString("yyyy-MM-dd");

            // Mở form cập nhật — các ô đã điền sẵn dữ liệu cũ
            using (StudentFormDialog dlg = new StudentFormDialog(masv, hoten, ngaysinh, diachi))
            {
                if (dlg.ShowDialog() != DialogResult.OK) return;

                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        SqlCommand cmd = new SqlCommand("SP_UPD_PUBLIC_SINHVIEN", conn);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@MASV", dlg.MaSV);
                        cmd.Parameters.AddWithValue("@HOTEN", dlg.HoTen);
                        cmd.Parameters.AddWithValue("@NGAYSINH", DateTime.Parse(dlg.NgaySinh));
                        cmd.Parameters.AddWithValue("@DIACHI", dlg.DiaChi);
                        cmd.Parameters.AddWithValue("@MALOP", malop);
                        cmd.Parameters.AddWithValue("@MANV", manv);

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Cập nhật sinh viên thành công!",
                        "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadStudents();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi:\n" + ex.Message,
                        "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // ================= XÓA =================
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvStudents.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn sinh viên cần xóa!",
                    "Chưa chọn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string masv = dgvStudents.SelectedRows[0].Cells["MASV"].Value?.ToString() ?? "";
            string hoten = dgvStudents.SelectedRows[0].Cells["HOTEN"].Value?.ToString() ?? "";

            DialogResult confirm = MessageBox.Show(
                $"Bạn có chắc muốn xóa sinh viên:\n\n  {masv} — {hoten}?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SP_DEL_PUBLIC_SINHVIEN", conn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@MASV", masv);
                    cmd.Parameters.AddWithValue("@MANV", manv);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Xóa sinh viên thành công!",
                    "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadStudents();
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Không thể xóa:\n" + ex.Message,
                    "Lỗi SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hệ thống:\n" + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ================= NHẬP ĐIỂM =================
        private void BtnManageScores_Click(object sender, EventArgs e)
        {
            if (dgvStudents.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn sinh viên!",
                    "Chưa chọn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (classManv.Trim().ToUpper() != manv.Trim().ToUpper())
            {
                MessageBox.Show("Bạn không có quyền xem điểm!",
                    "Từ chối", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            string masv = dgvStudents.SelectedRows[0].Cells["MASV"].Value?.ToString() ?? "";
            string malopSinhVien = dgvStudents.SelectedRows[0].Cells["MALOP"].Value?.ToString() ?? "";

            if (malopSinhVien.Trim() != this.malop.Trim())
            {
                MessageBox.Show("Bạn không có quyền nhập điểm cho sinh viên lớp này!",
                    "Từ chối", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            ScoreManagementForm f = new ScoreManagementForm(masv, manv, password, this);
            this.Hide();
            f.ShowDialog();
            this.Show();
        }
    }
}