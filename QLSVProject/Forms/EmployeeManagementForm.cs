using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Drawing;
using System.Text;
using System.Security.Cryptography;
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
        private TextBox txtSearch;
        private Label lblSearch;

        public EmployeeManagementForm()
        {
            InitializeComponent();
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

            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;
            btnRefresh.Click += (s, e) => LoadEmployees();

            this.Controls.AddRange(new Control[]
            {
                title,
                lblSearch, txtSearch, btnSearch,
                dgvEmployees,
                btnUpdate, btnDelete, btnRefresh
            });

            LoadEmployees();
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
                    cmd.Parameters.AddWithValue("@MANV", DBNull.Value); // null = lấy tất cả

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
                LoadEmployees(); // load tất cả nếu không nhập
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
    }
}