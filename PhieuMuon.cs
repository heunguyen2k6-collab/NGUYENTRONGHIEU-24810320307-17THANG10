using System;

namespace BT17TH10
{
    public class PhieuMuon
    {
        public string MaPhieu { get; set; }
        public string MaSV { get; set; }
        public string HoTenSV { get; set; }
        public string ThietBi { get; set; }
        public int SoLuong { get; set; }
        public DateTime NgayMuon { get; set; }
        public DateTime HanTra { get; set; }
        public DateTime? NgayTraThucTe { get; set; } // Có thể null khi chưa trả

        // Tự động tính toán hoặc trả về Tình trạng
        public string TinhTrang
        {
            get
            {
                if (NgayTraThucTe.HasValue)
                {
                    return "Đã trả";
                }
                else if (DateTime.Now.Date > HanTra.Date)
                {
                    return "Quá hạn";
                }
                else
                {
                    return "Đang mượn";
                }
            }
        }
    }
}