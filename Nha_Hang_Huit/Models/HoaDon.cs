using System;

namespace Nha_Hang_Huit.Models
{
    /// <summary>
    /// Model hoa don - schema tblHoaDon (service maps column names)
    /// </summary>
    public class HoaDon
    {
        public int MaHoaDon { get; set; }
        public int? MaKhachHang { get; set; }
        /// <summary>Map tu ThoiGianTao</summary>
        public DateTime NgayTao { get; set; }
        /// <summary>Map tu TongTienGoc</summary>
        public decimal TongTien { get; set; }
        /// <summary>Map tu SoTienGiamGia</summary>
        public decimal TienGiamGia { get; set; }
        /// <summary>Map tu TongThanhToan</summary>
        public decimal ThanhTien { get; set; }
        /// <summary>Map tu HinhThucTT</summary>
        public string PhuongThucThanhToan { get; set; }
        public string TrangThai { get; set; }
        public int? MaCa { get; set; }
        public string GhiChu { get; set; }
    }
}
