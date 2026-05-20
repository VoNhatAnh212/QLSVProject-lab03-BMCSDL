using System;
using System.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;

namespace QLSVNhomApp
{
    public partial class ScoreManagementForm : Form
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["QLSVNhomConnection"].ConnectionString;

        private readonly string masv;
        private readonly string manv;
        private readonly string password;

        private StudentManagementForm studentManagementForm;

        // ================= UI =================
        private DataGridView dgvHocPhan;
        private TextBox txtMAHP;
        private TextBox txtDIEMTHI;

        private Button btnSave;
        private Button btnLoad;

        private Label lblTitle;
        private Label lblMAHP;
        private Label lblDIEMTHI;

        private Panel panelInput;

        public ScoreManagementForm(
            string masv,
            string manv,
            string password,
            StudentManagementForm studentManagementForm)
        {
            this.masv = masv;
            this.manv = manv;
            this.password = password;
            this.studentManagementForm = studentManagementForm;

            InitializeComponent();
            LoadHocPhan();

            this.FormClosed += (s, e) => studentManagementForm.Show();
        }

        // ================= UI DESIGN =================
        private void InitializeComponent()
        {
            this.Text = "Quản lý điểm sinh viên";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 247, 250);

            // ===== TITLE =====
            lblTitle = new Label()
            {
                Text = "QUẢN LÝ ĐIỂM SINH VIÊN",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 37, 41),
                Size = new Size(900, 40),
                Location = new Point(50, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // ===== LOAD BUTTON =====
            btnLoad = new Button()
            {
                Text = "Tải dữ liệu",
                Size = new Size(150, 40),
                Location = new Point(50, 80),
                BackColor = Color.FromArgb(0, 122, 204),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLoad.FlatAppearance.BorderSize = 0;
            btnLoad.Click += (s, e) => LoadHocPhan();

            // ===== GRID =====
            dgvHocPhan = new DataGridView()
            {
                Location = new Point(50, 140),
                Size = new Size(900, 360),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White
            };
            dgvHocPhan.SelectionChanged += DgvHocPhan_SelectionChanged;

            // ===== PANEL INPUT =====
            panelInput = new Panel()
            {
                Size = new Size(900, 120),
                Location = new Point(50, 520),
                BackColor = Color.White
            };

            lblMAHP = new Label()
            {
                Text = "Mã học phần",
                Location = new Point(40, 20),
                Size = new Size(150, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            txtMAHP = new TextBox()
            {
                Location = new Point(40, 50),
                Size = new Size(250, 30),
                ReadOnly = true,
                Font = new Font("Segoe UI", 10)
            };

            lblDIEMTHI = new Label()
            {
                Text = "Điểm thi",
                Location = new Point(350, 20),
                Size = new Size(150, 25),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            txtDIEMTHI = new TextBox()
            {
                Location = new Point(350, 50),
                Size = new Size(250, 30),
                Font = new Font("Segoe UI", 10)
            };

            // ===== SAVE BUTTON =====
            btnSave = new Button()
            {
                Text = "Lưu điểm",
                Size = new Size(180, 40),
                Location = new Point(650, 45),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            // add input panel controls
            panelInput.Controls.AddRange(new Control[]
            {
                lblMAHP, txtMAHP,
                lblDIEMTHI, txtDIEMTHI,
                btnSave
            });

            // add form controls
            this.Controls.AddRange(new Control[]
            {
                lblTitle,
                btnLoad,
                dgvHocPhan,
                panelInput
            });
        }

        // ================= LOAD =================
        private void LoadHocPhan()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd =
                        new SqlCommand("SP_SEL_PUBLIC_BANGDIEM", conn);

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@MASV", masv);
                    cmd.Parameters.AddWithValue("@MANV", manv);
                    cmd.Parameters.AddWithValue("@MK", password);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvHocPhan.DataSource = dt;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // ================= SELECT ROW =================
        private void DgvHocPhan_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvHocPhan.SelectedRows.Count == 0) return;

            txtMAHP.Text =
                dgvHocPhan.SelectedRows[0].Cells["MAHP"].Value?.ToString();

            txtDIEMTHI.Text =
                dgvHocPhan.SelectedRows[0].Cells["DIEMTHI"].Value?.ToString();
        }

        // ================= SAVE =================
        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInput(out int diemThi))
                return;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    SqlCommand cmd =
                        new SqlCommand("SP_UPD_PUBLIC_BANGDIEM", conn);

                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@MASV", masv);
                    cmd.Parameters.AddWithValue("@MAHP", txtMAHP.Text);
                    cmd.Parameters.AddWithValue("@DIEMTHI", diemThi);
                    cmd.Parameters.AddWithValue("@MANV", manv);
                    cmd.Parameters.AddWithValue("@MK", password);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Cập nhật điểm thành công!");
                LoadHocPhan();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // ================= VALIDATE =================
        private bool ValidateInput(out int diemThi)
        {
            diemThi = 0;

            if (string.IsNullOrWhiteSpace(txtMAHP.Text))
            {
                MessageBox.Show("Vui lòng chọn học phần.");
                return false;
            }

            if (!int.TryParse(txtDIEMTHI.Text.Trim(), out diemThi))
            {
                MessageBox.Show("Điểm phải là số.");
                return false;
            }

            if (diemThi < 0 || diemThi > 10)
            {
                MessageBox.Show("Điểm phải từ 0 - 10.");
                return false;
            }

            return true;
        }
    }
}