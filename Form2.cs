using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BT17TH10
{
    public partial class Form2 : Form
    {
        // Khai báo BindingList và BindingSource
        private BindingList<PhieuMuon> danhSachPhieu = new BindingList<PhieuMuon>();
        private BindingSource bindingSource = new BindingSource();

        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            KhoiTaoDuLieuBanDau();
            CauHinhDataGridView();

            // Khởi tạo ComboBox Thiết bị
            cboThietBi.Items.AddRange(new string[] { "Kính hiển vi", "Máy đo huyết áp", "Bộ dụng cụ phẫu thuật", "Máy chiếu", "Laptop thí nghiệm" });
            if (cboThietBi.Items.Count > 0) cboThietBi.SelectedIndex = 0;

            // Khởi tạo ComboBox Lọc
            cboLocTinhTrang.Items.AddRange(new string[] { "Tất cả", "Đang mượn", "Quá hạn", "Đã trả" });
            cboLocTinhTrang.SelectedIndex = 0;

            // Gán dữ liệu vào BindingSource và DataGridView
            bindingSource.DataSource = danhSachPhieu;
            dgvDanhSach.DataSource = bindingSource;

            // Đặt thời gian mặc định
            dtpNgayMuon.Value = DateTime.Now;
            dtpHanTra.Value = DateTime.Now.AddDays(7);
            dtpNgayTraThucTe.Enabled = false;

            // Đăng ký sự kiện
            dgvDanhSach.CellClick += dgvDanhSach_CellClick;
            chkDaTra.CheckedChanged += chkDaTra_CheckedChanged;
            txtTimKiem.TextChanged += (s, ev) => ApDungBoLoc();
            cboLocTinhTrang.SelectedIndexChanged += (s, ev) => ApDungBoLoc();

            CapNhatThongKe();
        }

        private void KhoiTaoDuLieuBanDau()
        {
            danhSachPhieu.Add(new PhieuMuon { MaPhieu = "PM01", MaSV = "SV001", HoTenSV = "Nguyễn Văn A", ThietBi = "Kính hiển vi", SoLuong = 1, NgayMuon = DateTime.Now.AddDays(-10), HanTra = DateTime.Now.AddDays(-3), NgayTraThucTe = null });
            danhSachPhieu.Add(new PhieuMuon { MaPhieu = "PM02", MaSV = "SV002", HoTenSV = "Trần Thị B", ThietBi = "Máy chiếu", SoLuong = 1, NgayMuon = DateTime.Now.AddDays(-2), HanTra = DateTime.Now.AddDays(5), NgayTraThucTe = null });
        }

        private void CauHinhDataGridView()
        {
            dgvDanhSach.AutoGenerateColumns = false;
            dgvDanhSach.Columns.Clear();

            // Tự động co dãn các cột lấp đầy DataGridView
            dgvDanhSach.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvDanhSach.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "MaPhieu", HeaderText = "Mã Phiếu", FillWeight = 80 });
            dgvDanhSach.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "MaSV", HeaderText = "Mã SV", FillWeight = 80 });
            dgvDanhSach.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "HoTenSV", HeaderText = "Họ Tên SV", FillWeight = 120 });
            dgvDanhSach.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "ThietBi", HeaderText = "Thiết Bị", FillWeight = 120 });
            dgvDanhSach.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "SoLuong", HeaderText = "Số Lượng", FillWeight = 60 });
            dgvDanhSach.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "NgayMuon", HeaderText = "Ngày Mượn", DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" }, FillWeight = 90 });
            dgvDanhSach.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "HanTra", HeaderText = "Hạn Trả", DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" }, FillWeight = 90 });
            dgvDanhSach.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "NgayTraThucTe", HeaderText = "Ngày Trả TT", DefaultCellStyle = new DataGridViewCellStyle { Format = "dd/MM/yyyy" }, FillWeight = 90 });
            dgvDanhSach.Columns.Add(new DataGridViewTextBoxColumn { DataPropertyName = "TinhTrang", HeaderText = "Tình Trạng", FillWeight = 90 });
        }

        // Bắt lỗi Validation
        private bool KiemTraDuLieu(bool isThemMoi = true)
        {
            errorProvider1.Clear();
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(txtMaPhieu.Text))
            {
                errorProvider1.SetError(txtMaPhieu, "Mã phiếu không được để trống!");
                isValid = false;
            }
            else if (isThemMoi && danhSachPhieu.Any(p => p.MaPhieu.Equals(txtMaPhieu.Text.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                errorProvider1.SetError(txtMaPhieu, "Mã phiếu đã tồn tại!");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(txtMaSV.Text))
            {
                errorProvider1.SetError(txtMaSV, "Mã sinh viên không được để trống!");
                isValid = false;
            }

            if (cboThietBi.SelectedIndex == -1)
            {
                errorProvider1.SetError(cboThietBi, "Vui lòng chọn thiết bị!");
                isValid = false;
            }

            if (nudSoLuong.Value <= 0)
            {
                errorProvider1.SetError(nudSoLuong, "Số lượng phải lớn hơn 0!");
                isValid = false;
            }

            if (dtpHanTra.Value.Date < dtpNgayMuon.Value.Date)
            {
                errorProvider1.SetError(dtpHanTra, "Hạn trả không được trước ngày mượn!");
                isValid = false;
            }

            if (chkDaTra.Checked && dtpNgayTraThucTe.Value.Date < dtpNgayMuon.Value.Date)
            {
                errorProvider1.SetError(dtpNgayTraThucTe, "Ngày trả thực tế không được trước ngày mượn!");
                isValid = false;
            }

            return isValid;
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            if (!KiemTraDuLieu(isThemMoi: true)) return;

            PhieuMuon pm = new PhieuMuon
            {
                MaPhieu = txtMaPhieu.Text.Trim(),
                MaSV = txtMaSV.Text.Trim(),
                HoTenSV = txtHoTenSV.Text.Trim(),
                ThietBi = cboThietBi.SelectedItem.ToString(),
                SoLuong = (int)nudSoLuong.Value,
                NgayMuon = dtpNgayMuon.Value.Date,
                HanTra = dtpHanTra.Value.Date,
                NgayTraThucTe = chkDaTra.Checked ? (DateTime?)dtpNgayTraThucTe.Value.Date : null
            };

            danhSachPhieu.Add(pm);
            CapNhatThongKe();
            MessageBox.Show("Thêm phiếu mượn thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ClearForm();
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            if (dgvDanhSach.CurrentRow == null) return;
            if (!KiemTraDuLieu(isThemMoi: false)) return;

            PhieuMuon pm = (PhieuMuon)dgvDanhSach.CurrentRow.DataBoundItem;
            pm.MaSV = txtMaSV.Text.Trim();
            pm.HoTenSV = txtHoTenSV.Text.Trim();
            pm.ThietBi = cboThietBi.SelectedItem.ToString();
            pm.SoLuong = (int)nudSoLuong.Value;
            pm.NgayMuon = dtpNgayMuon.Value.Date;
            pm.HanTra = dtpHanTra.Value.Date;
            pm.NgayTraThucTe = chkDaTra.Checked ? (DateTime?)dtpNgayTraThucTe.Value.Date : null;

            bindingSource.ResetBindings(false);
            CapNhatThongKe();
            MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dgvDanhSach.CurrentRow == null) return;

            if (MessageBox.Show("Bạn có chắc chắn muốn xóa phiếu mượn này?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                PhieuMuon pm = (PhieuMuon)dgvDanhSach.CurrentRow.DataBoundItem;
                danhSachPhieu.Remove(pm);
                CapNhatThongKe();
                ClearForm();
            }
        }

        private void chkDaTra_CheckedChanged(object sender, EventArgs e)
        {
            dtpNgayTraThucTe.Enabled = chkDaTra.Checked;
            if (chkDaTra.Checked)
            {
                dtpNgayTraThucTe.Value = DateTime.Now;
            }
        }

        private void dgvDanhSach_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvDanhSach.Rows[e.RowIndex].DataBoundItem is PhieuMuon pm)
            {
                txtMaPhieu.Text = pm.MaPhieu;
                txtMaPhieu.Enabled = false;
                txtMaSV.Text = pm.MaSV;
                txtHoTenSV.Text = pm.HoTenSV;
                cboThietBi.SelectedItem = pm.ThietBi;
                nudSoLuong.Value = pm.SoLuong;
                dtpNgayMuon.Value = pm.NgayMuon;
                dtpHanTra.Value = pm.HanTra;

                if (pm.NgayTraThucTe.HasValue)
                {
                    chkDaTra.Checked = true;
                    dtpNgayTraThucTe.Value = pm.NgayTraThucTe.Value;
                }
                else
                {
                    chkDaTra.Checked = false;
                }
            }
        }

        private void ApDungBoLoc()
        {
            string tuKhoa = txtTimKiem.Text.Trim().ToLower();
            string locTinhTrang = cboLocTinhTrang.SelectedItem?.ToString();

            var danhSachLoc = danhSachPhieu.Where(p =>
                (string.IsNullOrEmpty(tuKhoa) || p.MaSV.ToLower().Contains(tuKhoa) || p.ThietBi.ToLower().Contains(tuKhoa)) &&
                (locTinhTrang == "Tất cả" || p.TinhTrang == locTinhTrang)
            ).ToList();

            bindingSource.DataSource = new BindingList<PhieuMuon>(danhSachLoc);
        }

        private void CapNhatThongKe()
        {
            lblTongSoPhieu.Text = $"Tổng số phiếu: {danhSachPhieu.Count}";
            int soQuaHan = danhSachPhieu.Count(p => p.TinhTrang == "Quá hạn");
            lblSoPhieuQuaHan.Text = $" | Số phiếu quá hạn: {soQuaHan}";
        }

        private void btnXuatCSV_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = "CSV File (*.csv)|*.csv",
                FileName = "DanhSachPhieuMuon.csv"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("MaPhieu,MaSV,HoTenSV,ThietBi,SoLuong,NgayMuon,HanTra,NgayTraThucTe,TinhTrang");

                var listCurrent = (BindingList<PhieuMuon>)bindingSource.DataSource;
                foreach (var item in listCurrent)
                {
                    string ngayTra = item.NgayTraThucTe.HasValue ? item.NgayTraThucTe.Value.ToString("dd/MM/yyyy") : "";
                    sb.AppendLine($"{item.MaPhieu},{item.MaSV},{item.HoTenSV},{item.ThietBi},{item.SoLuong},{item.NgayMuon:dd/MM/yyyy},{item.HanTra:dd/MM/yyyy},{ngayTra},{item.TinhTrang}");
                }

                File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                MessageBox.Show("Xuất file CSV thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnLamMoi_Click(object sender, EventArgs e) => ClearForm();

        private void ClearForm()
        {
            txtMaPhieu.Enabled = true;
            txtMaPhieu.Clear();
            txtMaSV.Clear();
            txtHoTenSV.Clear();
            nudSoLuong.Value = 1;
            chkDaTra.Checked = false;
            dtpNgayMuon.Value = DateTime.Now;
            dtpHanTra.Value = DateTime.Now.AddDays(7);
            errorProvider1.Clear();
        }
    }
}