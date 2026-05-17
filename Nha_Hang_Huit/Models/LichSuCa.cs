using System;

namespace Nha_Hang_Huit.Models
{
    /// <summary>
    /// Model lich su ca lam viec / bao cao dong ca - schema tblCa
    /// </summary>
    public class LichSuCa
    {
        public int MaCa { get; set; }
        /// <summary>Map tu ThoiGianMoCa</summary>
        public DateTime GioBatDau { get; set; }
        /// <summary>Map tu ThoiGianDongCa (nullable)</summary>
        public DateTime? GioKetThuc { get; set; }
        /// <summary>Map tu HoTen (JOIN tblNhanVien)</summary>
        public string NhanVien { get; set; }
        /// <summary>Map tu TongHoaDon</summary>
        public int TongSoHoaDon { get; set; }
        /// <summary>Map tu TongDoanhThu</summary>
        public decimal TongDoanhThuTruocGiamGia { get; set; }
        /// <summary>Map tu TongGiamGia</summary>
        public decimal TongTienGiamGia { get; set; }
        /// <summary>Map tu DoanhThuThucNhan</summary>
        public decimal TongDoanhThuThucNhan { get; set; }
        /// <summary>Map tu KhachMoiTrongNgay (tu spDongCa)</summary>
        public int SoKhachMoi { get; set; }
        public string GhiChu { get; set; }
    }
}
