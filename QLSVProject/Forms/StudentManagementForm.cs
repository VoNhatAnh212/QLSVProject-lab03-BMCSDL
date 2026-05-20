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

            public StudentManagementForm(
                string malop,
                string manv,
                string password,
                ClassManagementForm classManagementForm)
            {
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

                btnAdd = CreateButton("Thêm", new Point(20, 550), Color.FromArgb(0, 122, 204));
                btnUpdate = CreateButton("Sửa", new Point(180, 550), Color.FromArgb(0, 122, 204));
                btnDelete = CreateButton("Xóa", new Point(340, 550), Color.FromArgb(220, 53, 69));
                btnManageScores = CreateButton("Nhập điểm", new Point(500, 550), Color.FromArgb(40, 167, 69));
                btnRefresh = CreateButton("Tải lại", new Point(660, 550), Color.Gray);

                btnAdd.Size = new Size(140, 45);
                btnUpdate.Size = new Size(140, 45);
                btnDelete.Size = new Size(140, 45);
                btnManageScores.Size = new Size(140, 45);
                btnRefresh.Size = new Size(140, 45);

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
                return new Button()
                {
                    Text = text,
                    Size = new Size(130, 40),
                    Location = location,
                    BackColor = backColor,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold)
                };
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
                    MessageBox.Show(ex.Message);
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
            private void BtnAdd_Click(object sender, EventArgs e)
            {
                try
                {
                    string masv = Prompt("Thêm sinh viên", "Nhập mã SV:");
                    string hoten = Prompt("Thêm sinh viên", "Nhập họ tên:");
                    string ngaysinh = Prompt("Thêm sinh viên", "Nhập ngày sinh (yyyy-MM-dd):");
                    string diachi = Prompt("Thêm sinh viên", "Nhập địa chỉ:");
                    string tendn = Prompt("Tài khoản SV", "Tên đăng nhập:");
                    string mk = Prompt("Tài khoản SV", "Mật khẩu:");

                    if (string.IsNullOrWhiteSpace(masv) ||
                        string.IsNullOrWhiteSpace(hoten) ||
                        string.IsNullOrWhiteSpace(ngaysinh) ||
                        string.IsNullOrWhiteSpace(diachi) ||
                        string.IsNullOrWhiteSpace(tendn) ||
                        string.IsNullOrWhiteSpace(mk))
                    {
                        MessageBox.Show("Không được để trống!");
                        return;
                    }

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        SqlCommand cmd = new SqlCommand("SP_INS_PUBLIC_SINHVIEN", conn);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@MASV", masv);
                        cmd.Parameters.AddWithValue("@HOTEN", hoten);
                        cmd.Parameters.AddWithValue("@NGAYSINH", DateTime.Parse(ngaysinh));
                        cmd.Parameters.AddWithValue("@DIACHI", diachi);
                        cmd.Parameters.AddWithValue("@MALOP", malop);
                        cmd.Parameters.AddWithValue("@TENDN", tendn);
                        cmd.Parameters.AddWithValue("@MK", mk);
                        cmd.Parameters.AddWithValue("@MANV", manv); // ✔ FIX QUYỀN

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Thêm sinh viên thành công!");
                    LoadStudents();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("SQL Error: " + ex.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi: " + ex.Message);
                }
            }

            private void BtnUpdate_Click(object sender, EventArgs e)
            {
                if (dgvStudents.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn sinh viên!");
                    return;
                }

                // ===== lấy dữ liệu cũ =====
                var row = dgvStudents.SelectedRows[0];

                string masv = row.Cells["MASV"].Value.ToString();
                string hotenOld = row.Cells["HOTEN"].Value.ToString();
                string ngaysinhOld = row.Cells["NGAYSINH"].Value.ToString();
                string diachiOld = row.Cells["DIACHI"].Value.ToString();

                // ===== nhập lại nhưng có sẵn dữ liệu cũ =====
                string hoten = Prompt("Cập nhật sinh viên", "Họ tên:", hotenOld);
                string ngaysinh = Prompt("Cập nhật sinh viên", "Ngày sinh (yyyy-MM-dd):", ngaysinhOld);
                string diachi = Prompt("Cập nhật sinh viên", "Địa chỉ:", diachiOld);

                if (hoten == "" || ngaysinh == "" || diachi == "")
                {
                    MessageBox.Show("Không được để trống!");
                    return;
                }

                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();

                        SqlCommand cmd = new SqlCommand("SP_UPD_PUBLIC_SINHVIEN", conn);
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@MASV", masv);
                        cmd.Parameters.AddWithValue("@HOTEN", hoten);
                        cmd.Parameters.AddWithValue("@NGAYSINH", DateTime.Parse(ngaysinh));
                        cmd.Parameters.AddWithValue("@DIACHI", diachi);
                        cmd.Parameters.AddWithValue("@MALOP", malop);
                        cmd.Parameters.AddWithValue("@MANV", manv);

                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Cập nhật thành công!");
                    LoadStudents();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            // ================= DELETE =================
            private void BtnDelete_Click(object sender, EventArgs e)
            {
                if (dgvStudents.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Chọn sinh viên!");
                    return;
                }

                string masv = dgvStudents.SelectedRows[0].Cells["MASV"].Value.ToString();

                DialogResult confirm = MessageBox.Show(
                    "Bạn có chắc muốn xóa sinh viên này?",
                    "Xác nhận",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirm != DialogResult.Yes)
                    return;

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

                    MessageBox.Show("Xóa sinh viên thành công!");
                    LoadStudents();
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

            // ================= SCORE =================
            private void BtnManageScores_Click(object sender, EventArgs e)
            {
                if (dgvStudents.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Chọn sinh viên!");
                    return;
                }

                string masv = dgvStudents.SelectedRows[0].Cells["MASV"].Value.ToString();

                // ===== LẤY MALOP CỦA SINH VIÊN =====
                string malopSinhVien = dgvStudents
                    .SelectedRows[0]
                    .Cells["MALOP"]
                    .Value.ToString();

                // ===== SO SÁNH QUYỀN =====
                if (malopSinhVien.Trim() != this.malop.Trim())
                {
                    MessageBox.Show("Bạn không có quyền nhập điểm cho sinh viên lớp này!");
                    return;
                }

                ScoreManagementForm f = new ScoreManagementForm(masv, manv, password, this);

                this.Hide();
                f.ShowDialog();
                this.Show();
            }
        }
    }