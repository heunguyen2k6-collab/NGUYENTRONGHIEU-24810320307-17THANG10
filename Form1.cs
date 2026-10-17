using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace BT17TH10
{
    public partial class Form1 : Form
    {
        private BindingList<LichHen> danhSachGoc = new BindingList<LichHen>();
        private BindingSource bindingSource = new BindingSource();

        public Form1()
        {
            InitializeComponent();
            SetupDataGridView();
            InitData();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cboBacSi.Items.Clear();
            cboBacSi.Items.AddRange(new string[] { "BS. Nguyễn Văn A", "BS. Trần Thị B", "BS. Lê Văn C" });
            if (cboBacSi.Items.Count > 0) cboBacSi.SelectedIndex = 0;

            cboTrangThai.Items.Clear();
            cboTrangThai.Items.AddRange(new string[] { "Chờ khám", "Đang khám", "Hoàn tất", "Đã hủy" });
            cboTrangThai.SelectedIndex = 0;

            cboLocTrangThai.Items.Clear();
            cboLocTrangThai.Items.AddRange(new string[] { "Tất cả", "Chờ khám", "Đang khám", "Hoàn tất", "Đã hủy" });
            cboLocTrangThai.SelectedIndex = 0;

            bindingSource.DataSource = danhSachGoc;
            dgvLichHen.DataSource = bindingSource;

            CapNhatStatusStrip();
        }

        private void SetupDataGridView()
        {
            dgvLichHen.AutoGenerateColumns = false;
            dgvLichHen.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvLichHen.MultiSelect = false;
            dgvLichHen.ReadOnly = true;

            if (dgvLichHen.Columns["colMa"] != null) dgvLichHen.Columns["colMa"].DataPropertyName = "MaLichHen";
            if (dgvLichHen.Columns["colHoTen"] != null) dgvLichHen.Columns["colHoTen"].DataPropertyName = "HoTen";
            if (dgvLichHen.Columns["colSDT"] != null) dgvLichHen.Columns["colSDT"].DataPropertyName = "SoDienThoai";
            if (dgvLichHen.Columns["colNgayGio"] != null) dgvLichHen.Columns["colNgayGio"].DataPropertyName = "NgayGio";
            if (dgvLichHen.Columns["colBacSi"] != null) dgvLichHen.Columns["colBacSi"].DataPropertyName = "BacSi";
            if (dgvLichHen.Columns["colLyDo"] != null) dgvLichHen.Columns["colLyDo"].DataPropertyName = "LyDo";
            if (dgvLichHen.Columns["colTrangThai"] != null) dgvLichHen.Columns["colTrangThai"].DataPropertyName = "TrangThai";
        }

        private void InitData()
        {
            danhSachGoc.Add(new LichHen
            {
                MaLichHen = "LH01",
                HoTen = "Nguyễn Văn Nam",
                SoDienThoai = "0912345678",
                NgayGio = DateTime.Now.AddDays(1),
                BacSi = "BS. Nguyễn Văn A",
                LyDo = "Khám định kỳ",
                TrangThai = "Chờ khám"
            });
        }

        private bool ValidateInput(bool isThem = true)
        {
            errorProvider1.Clear();
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(txtMaLichHen.Text))
            {
                errorProvider1.SetError(txtMaLichHen, "Mã lịch hẹn không được để trống!");
                isValid = false;
            }
            else if (isThem && danhSachGoc.Any(x => x.MaLichHen.Equals(txtMaLichHen.Text.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                errorProvider1.SetError(txtMaLichHen, "Mã lịch hẹn đã tồn tại!");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(txtHoTen.Text))
            {
                errorProvider1.SetError(txtHoTen, "Họ tên không được để trống!");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(txtSDT.Text) || !Regex.IsMatch(txtSDT.Text.Trim(), @"^\d{10}$"))
            {
                errorProvider1.SetError(txtSDT, "Số điện thoại phải đúng 10 chữ số!");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(txtLyDo.Text))
            {
                errorProvider1.SetError(txtLyDo, "Lý do khám không được để trống!");
                isValid = false;
            }

            if (dtpNgayGio.Value < DateTime.Now)
            {
                errorProvider1.SetError(dtpNgayGio, "Thời gian hẹn không được ở quá khứ!");
                isValid = false;
            }

            return isValid;
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (!ValidateInput(isThem: true)) return;

            LichHen lh = new LichHen
            {
                MaLichHen = txtMaLichHen.Text.Trim(),
                HoTen = txtHoTen.Text.Trim(),
                SoDienThoai = txtSDT.Text.Trim(),
                NgayGio = dtpNgayGio.Value,
                BacSi = cboBacSi.SelectedItem?.ToString(),
                LyDo = txtLyDo.Text.Trim(),
                TrangThai = cboTrangThai.SelectedItem?.ToString()
            };

            danhSachGoc.Add(lh);
            ThucHienLocVaTimKiem();
            XoaTrangInput();
            MessageBox.Show("Thêm lịch hẹn thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnCapNhat_Click(object sender, EventArgs e)
        {
            if (dgvLichHen.CurrentRow == null) return;

            string maLH = txtMaLichHen.Text.Trim();
            var item = danhSachGoc.FirstOrDefault(x => x.MaLichHen.Equals(maLH, StringComparison.OrdinalIgnoreCase));

            if (item == null)
            {
                MessageBox.Show("Không tìm thấy lịch hẹn!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!ValidateInput(isThem: false)) return;

            item.HoTen = txtHoTen.Text.Trim();
            item.SoDienThoai = txtSDT.Text.Trim();
            item.NgayGio = dtpNgayGio.Value;
            item.BacSi = cboBacSi.SelectedItem?.ToString();
            item.LyDo = txtLyDo.Text.Trim();
            item.TrangThai = cboTrangThai.SelectedItem?.ToString();

            bindingSource.ResetBindings(false);
            ThucHienLocVaTimKiem();
            MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvLichHen.CurrentRow == null) return;

            LichHen selected = dgvLichHen.CurrentRow.DataBoundItem as LichHen;
            if (selected == null) return;

            DialogResult dr = MessageBox.Show($"Xác nhận xóa lịch hẹn của {selected.HoTen}?",
                                              "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (dr == DialogResult.Yes)
            {
                danhSachGoc.Remove(selected);
                ThucHienLocVaTimKiem();
                XoaTrangInput();
                MessageBox.Show("Xóa thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void dgvLichHen_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvLichHen.CurrentRow != null && dgvLichHen.CurrentRow.DataBoundItem is LichHen lh)
            {
                txtMaLichHen.Text = lh.MaLichHen;
                txtHoTen.Text = lh.HoTen;
                txtSDT.Text = lh.SoDienThoai;
                dtpNgayGio.Value = lh.NgayGio > DateTime.Now ? lh.NgayGio : DateTime.Now;
                cboBacSi.SelectedItem = lh.BacSi;
                txtLyDo.Text = lh.LyDo;
                cboTrangThai.SelectedItem = lh.TrangThai;
                txtMaLichHen.ReadOnly = true;
            }
        }

        private void txtTimKiem_TextChanged(object sender, EventArgs e) => ThucHienLocVaTimKiem();
        private void dtpLocNgay_ValueChanged(object sender, EventArgs e) => ThucHienLocVaTimKiem();
        private void cboLocTrangThai_SelectedIndexChanged(object sender, EventArgs e) => ThucHienLocVaTimKiem();

        private void ThucHienLocVaTimKiem()
        {
            string tuKhoa = txtTimKiem.Text.Trim().ToLower();
            string trangThaiLoc = cboLocTrangThai.SelectedItem?.ToString();
            DateTime ngayLoc = dtpLocNgay.Value.Date;

            var result = danhSachGoc.Where(lh =>
                (string.IsNullOrEmpty(tuKhoa) || lh.HoTen.ToLower().Contains(tuKhoa) || lh.SoDienThoai.Contains(tuKhoa)) &&
                (trangThaiLoc == "Tất cả" || string.IsNullOrEmpty(trangThaiLoc) || lh.TrangThai == trangThaiLoc) &&
                (lh.NgayGio.Date == ngayLoc)
            ).ToList();

            bindingSource.DataSource = new BindingList<LichHen>(result);
            CapNhatStatusStrip();
        }

        private void btnXuatCSV_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv",
                FileName = "DanhSachLichHen.csv"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("MaLichHen,HoTen,SoDienThoai,NgayGio,BacSi,LyDo,TrangThai");

                var list = bindingSource.DataSource as BindingList<LichHen>;
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        sb.AppendLine($"\"{item.MaLichHen}\",\"{item.HoTen}\",\"{item.SoDienThoai}\",\"{item.NgayGio:dd/MM/yyyy HH:mm}\",\"{item.BacSi}\",\"{item.LyDo}\",\"{item.TrangThai}\"");
                    }
                }

                File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                MessageBox.Show("Xuất file CSV thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void CapNhatStatusStrip()
        {
            int count = bindingSource.Count;
            lblTongSo.Text = $"Tổng số lịch hẹn hiển thị: {count}";
        }

        private void XoaTrangInput()
        {
            txtMaLichHen.ReadOnly = false;
            txtMaLichHen.Clear();
            txtHoTen.Clear();
            txtSDT.Clear();
            txtLyDo.Clear();
            dtpNgayGio.Value = DateTime.Now.AddHours(1);
            if (cboBacSi.Items.Count > 0) cboBacSi.SelectedIndex = 0;
            if (cboTrangThai.Items.Count > 0) cboTrangThai.SelectedIndex = 0;
            errorProvider1.Clear();
        }

        private void btnLamMoi_Click(object sender, EventArgs e)
        {
            XoaTrangInput();
            bindingSource.DataSource = danhSachGoc;
            CapNhatStatusStrip();
        }

        private void dgvLichHen_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}