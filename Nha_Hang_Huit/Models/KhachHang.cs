using System;

namespace Nha_Hang_Huit.Models
{
    /// <summary>
    /// Model khach hang - schema tblKhachHang + JOIN tblHangThe
    /// </summary>
    public class KhachHang
    {
        public int MaKhachHang { get; set; }
        public string TenKhachHang { get; set; }
        public string SoDienThoai { get; set; }
        public DateTime NgayDangKy { get; set; }
        public int TongDiemTichLuy { get; set; }
        public int? MaHang { get; set; }
        /// <summary>Map tu TenHang (JOIN tblHangThe)</summary>
        public string HangThe { get; set; } = "Thuong";

        /// <summary>Tinh ti le giam gia theo hang the (instance method)</summary>
        public double LayTiLeGiamGia()
        {
            return LayTiLeGiamGia(HangThe);
        }

        /// <summary>Tinh ti le giam gia theo hang the (static)</summary>
        public static double LayTiLeGiamGia(string hangThe)
        {
            switch (hangThe?.Trim().ToLowerInvariant())
            {
                case "kim cuong": return 0.15;
                case "vang": return 0.10;
                case "bac": return 0.05;
                case "thuong":
                default: return 0.0;
            }
        }
    }
}
