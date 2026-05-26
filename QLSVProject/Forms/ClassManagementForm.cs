using System;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace QLSVNhomApp
{
    public partial class ClassManagementForm : Form
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["QLSVNhomConnection"].ConnectionString;
        private string manv;
        private string password;
        private MainForm mainForm;

        private Label lblTitle;
        private Label lblSearch;

        private TextBox txtSearch;
        private DataGridView dgvClasses;

        private Button btnAdd;
        private Button btnEdit;
        private Button btnDelete;
        private Button btnManageStudents;
        private Button btnRefresh;

        public ClassManagementForm(string manv, string password, MainForm mainForm)
        {
            this.manv = manv;
            this.password = password;
            this.mainForm = mainForm;

            InitializeComponent();
            LoadClasses();

            this.FormClosed += (s, e) => mainForm.Show();
        }

        // ================= UI =================
        private void InitializeComponent()
        {
            this.Text = "Quản lý lớp học";
            this.Size = new Size(1050, 670);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 244, 248);

            lblTitle = new Label()
            {
                Text = "QUẢN LÝ LỚP HỌC",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 70, 140),
                AutoSize = false,
                Size = new Size(500, 40),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(260, 20)
            };

            lblSearch = new Label()
            {
                Text = "Tìm kiếm:",
                Font = new Font("Segoe UI", 10),
                Size = new Size(80, 30),
                Location = new Point(30, 80)
            };

            txtSearch = new TextBox()
            {
                Size = new Size(260, 30),
                Location = new Point(110, 78),
                Font = new Font("Segoe UI", 10)
            };
            txtSearch.TextChanged += (s, e) => SearchClasses();

            btnRefresh = CreateBlueButton("Tải lại", 390, 75, 110, 35);
            btnRefresh.Click += (s, e) => LoadClasses();

            dgvClasses = new DataGridView()
            {
                Location = new Point(30, 130),
                Size = new Size(970, 390),
                ReadOnly = true,
                AllowUserToAddRows = false,
                MultiSelect = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None
            };

            StyleGrid();

            btnAdd = CreateBlueButton("Thêm lớp", 70, 560, 150, 45);
            btnEdit = CreateBlueButton("Sửa lớp", 250, 560, 150, 45);
            btnDelete = CreateRedButton("Xóa lớp", 430, 560, 150, 45);
            btnManageStudents = CreateBlueButton("Quản lý sinh viên", 610, 560, 220, 45);

            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
            btnManageStudents.Click += BtnManageStudents_Click;

            this.Controls.AddRange(new Control[]
            {
                lblTitle, lblSearch, txtSearch, btnRefresh,
                dgvClasses,
                btnAdd, btnEdit, btnDelete, btnManageStudents
            });
        }

        // ================= STYLE =================
        private void StyleGrid()
        {
            dgvClasses.EnableHeadersVisualStyles = false;

            dgvClasses.ColumnHeadersDefaultCellStyle.BackColor =
                Color.FromArgb(0, 122, 204);

            dgvClasses.ColumnHeadersDefaultCellStyle.ForeColor =
                Color.White;

            dgvClasses.ColumnHeadersDefaultCellStyle.Font =
                new Font("Segoe UI", 10, FontStyle.Bold);

            dgvClasses.DefaultCellStyle.Font =
                new Font("Segoe UI", 10);

            dgvClasses.DefaultCellStyle.SelectionBackColor =
                Color.FromArgb(220, 235, 252);

            dgvClasses.DefaultCellStyle.SelectionForeColor =
                Color.Black;

            dgvClasses.AlternatingRowsDefaultCellStyle.BackColor =
                Color.FromArgb(248, 250, 252);
        }

        private Button CreateBlueButton(
            string text, int x, int y, int w, int h)
        {
            Button btn = new Button();

            btn.Text = text;
            btn.Size = new Size(w, h);
            btn.Location = new Point(x, y);
            btn.BackColor = Color.FromArgb(0, 122, 204);
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;

            btn.MouseEnter += (s, e) =>
                btn.BackColor = Color.FromArgb(30, 144, 255);

            btn.MouseLeave += (s, e) =>
                btn.BackColor = Color.FromArgb(0, 122, 204);

            return btn;
        }

        private Button CreateRedButton(
            string text, int x, int y, int w, int h)
        {
            Button btn = new Button();

            btn.Text = text;
            btn.Size = new Size(w, h);
            btn.Location = new Point(x, y);
            btn.BackColor = Color.FromArgb(220, 53, 69);
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;

            btn.MouseEnter += (s, e) =>
                btn.BackColor = Color.FromArgb(235, 75, 90);

            btn.MouseLeave += (s, e) =>
                btn.BackColor = Color.FromArgb(220, 53, 69);

            return btn;
        }

        // ================= LOAD =================
        private void LoadClasses()
        {
            try
            {
                using (SqlConnection conn =
                    new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd =
                        new SqlCommand("SP_SEL_PUBLIC_LOP", conn);

                    cmd.CommandType =
                        CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@MANV", manv);

                    SqlDataAdapter da =
                        new SqlDataAdapter(cmd);

                    DataTable dt =
                        new DataTable();

                    da.Fill(dt);

                    dgvClasses.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải dữ liệu.\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ================= SEARCH =================
        private void SearchClasses()
        {
            string keyword = txtSearch.Text.Trim();

            if (keyword == "")
            {
                LoadClasses();
                return;
            }

            try
            {
                using (SqlConnection conn =
                    new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd =
                        new SqlCommand(
                            @"SELECT *
                              FROM LOP
                              WHERE MALOP LIKE @k
                                 OR TENLOP LIKE @k", conn);

                    cmd.Parameters.AddWithValue(
                        "@k", "%" + keyword + "%");

                    SqlDataAdapter da =
                        new SqlDataAdapter(cmd);

                    DataTable dt =
                        new DataTable();

                    da.Fill(dt);

                    dgvClasses.DataSource = dt;
                }
            }
            catch
            {
                MessageBox.Show(
                    "Không thể tìm kiếm dữ liệu.",
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        // ================= THÊM =================
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            string malop =
                Prompt.ShowDialog("Nhập mã lớp:", "Thêm lớp");

            string tenlop =
                Prompt.ShowDialog("Nhập tên lớp:", "Thêm lớp");

            if (malop == "" || tenlop == "") return;

            try
            {
                using (SqlConnection conn =
                    new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd =
                        new SqlCommand("SP_INS_LOP", conn);

                    cmd.CommandType =
                        CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@MALOP", malop);
                    cmd.Parameters.AddWithValue("@TENLOP", tenlop);
                    cmd.Parameters.AddWithValue("@MANV", manv);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show(
                    "Thêm lớp thành công!",
                    "Thông báo");

                LoadClasses();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ================= SỬA =================
        //private void BtnEdit_Click(object sender, EventArgs e)
        //{
        //    if (dgvClasses.SelectedRows.Count == 0)
        //    {
        //        MessageBox.Show("Vui lòng chọn lớp cần sửa.");
        //        return;
        //    }

        //    string malop =
        //        dgvClasses.SelectedRows[0]
        //        .Cells["MALOP"].Value.ToString();

        //    string oldTen =
        //        dgvClasses.SelectedRows[0]
        //        .Cells["TENLOP"].Value.ToString();

        //    string tenMoi =
        //        Prompt.ShowDialog(
        //            "Nhập tên mới:",
        //            "Sửa lớp",
        //            oldTen);

        //    if (tenMoi == "") return;

        //    try
        //    {
        //        using (SqlConnection conn =
        //            new SqlConnection(connectionString))
        //        {
        //            conn.Open();

        //            SqlCommand cmd =
        //                new SqlCommand("SP_UPD_LOP", conn);

        //            cmd.CommandType =
        //                CommandType.StoredProcedure;

        //            cmd.Parameters.AddWithValue("@MALOP", malop);
        //            cmd.Parameters.AddWithValue("@TENLOP", tenMoi);

        //            cmd.ExecuteNonQuery();
        //        }

        //        MessageBox.Show(
        //            "Cập nhật thành công!");

        //        LoadClasses();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}

        // ================= XÓA =================
        //private void BtnDelete_Click(object sender, EventArgs e)
        //{
        //    if (dgvClasses.SelectedRows.Count == 0)
        //    {
        //        MessageBox.Show("Vui lòng chọn lớp cần xóa.");
        //        return;
        //    }

        //    string malop =
        //        dgvClasses.SelectedRows[0]
        //        .Cells["MALOP"].Value.ToString();

        //    DialogResult rs =
        //        MessageBox.Show(
        //            "Bạn chắc chắn muốn xóa lớp này?",
        //            "Xác nhận",
        //            MessageBoxButtons.YesNo,
        //            MessageBoxIcon.Question);

        //    if (rs == DialogResult.No) return;

        //    try
        //    {
        //        using (SqlConnection conn =
        //            new SqlConnection(connectionString))
        //        {
        //            conn.Open();

        //            SqlCommand cmd =
        //                new SqlCommand("SP_DEL_LOP", conn);

        //            cmd.CommandType =
        //                CommandType.StoredProcedure;

        //            cmd.Parameters.AddWithValue("@MALOP", malop);

        //            cmd.ExecuteNonQuery();
        //        }

        //        MessageBox.Show(
        //            "Xóa lớp thành công!");

        //        LoadClasses();
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //}

        // ================= QUẢN LÝ SV =================

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvClasses.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn lớp cần sửa.");
                return;
            }

            string malop = dgvClasses.SelectedRows[0].Cells["MALOP"].Value.ToString();
            string oldTen = dgvClasses.SelectedRows[0].Cells["TENLOP"].Value.ToString();

            string tenMoi = Prompt.ShowDialog(
                "Nhập tên mới:",
                "Sửa lớp",
                oldTen);

            if (string.IsNullOrWhiteSpace(tenMoi)) return;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("SP_UPD_LOP", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@MALOP", malop);
                        cmd.Parameters.AddWithValue("@TENLOP", tenMoi);
                        cmd.Parameters.AddWithValue("@MANV", manv); // 🔥 QUAN TRỌNG

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Cập nhật thành công!", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadClasses();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message, "Không thể cập nhật",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hệ thống: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvClasses.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn lớp cần xóa.");
                return;
            }

            string malop = dgvClasses.SelectedRows[0].Cells["MALOP"].Value.ToString();
            string tenlop = dgvClasses.SelectedRows[0].Cells["TENLOP"].Value.ToString();

            DialogResult rs = MessageBox.Show(
                $"Bạn có chắc muốn xóa lớp:\n{malop} - {tenlop} ?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (rs != DialogResult.Yes)
                return;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand("SP_DEL_LOP", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@MALOP", SqlDbType.VarChar, 20).Value = malop;
                    cmd.Parameters.Add("@MANV", SqlDbType.VarChar, 20).Value = manv;

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Xóa lớp thành công!",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                LoadClasses();
            }
            catch (SqlException ex)
            {
                MessageBox.Show(ex.Message,
                    "Không thể xóa",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi hệ thống: " + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        private void BtnManageStudents_Click(
            object sender,
            EventArgs e)
        {
            if (dgvClasses.SelectedRows.Count == 0)
            {
                MessageBox.Show("Vui lòng chọn lớp.");
                return;
            }

            string malop = dgvClasses.SelectedRows[0].Cells["MALOP"].Value.ToString();

            string classManv = dgvClasses.SelectedRows[0].Cells["MANV"].Value.ToString();

            StudentManagementForm f =
                new StudentManagementForm(
                    classManv,
                    malop,
                    manv,
                    password,
                    this);

            this.Hide();
            f.ShowDialog();
            this.Show();

            LoadClasses();
        }
    }

    // ================= FORM NHẬP =================
    public static class Prompt
    {
        public static string ShowDialog(
            string text,
            string caption,
            string defaultValue = "")
        {
            Form form = new Form();
            Label lbl = new Label();
            TextBox txt = new TextBox();
            Button btnOk = new Button();

            form.Text = caption;
            form.Size = new Size(400, 220);
            form.StartPosition =
                FormStartPosition.CenterScreen;
            form.BackColor = Color.WhiteSmoke;
            form.FormBorderStyle =
                FormBorderStyle.FixedDialog;
            form.MaximizeBox = false;
            form.MinimizeBox = false;

            lbl.Text = text;
            lbl.Location = new Point(25, 25);
            lbl.Size = new Size(320, 25);
            lbl.Font = new Font("Segoe UI", 10);

            txt.Location = new Point(25, 60);
            txt.Size = new Size(330, 30);
            txt.Text = defaultValue;
            txt.Font = new Font("Segoe UI", 10);

            btnOk.Text = "OK";
            btnOk.Size = new Size(100, 35);
            btnOk.Location = new Point(135, 115);
            btnOk.BackColor = Color.FromArgb(0, 122, 204);
            btnOk.ForeColor = Color.White;
            btnOk.FlatStyle = FlatStyle.Flat;
            btnOk.FlatAppearance.BorderSize = 0;
            btnOk.DialogResult = DialogResult.OK;

            form.Controls.Add(lbl);
            form.Controls.Add(txt);
            form.Controls.Add(btnOk);

            form.AcceptButton = btnOk;

            return form.ShowDialog() == DialogResult.OK
                ? txt.Text.Trim()
                : "";
        }
    }
}