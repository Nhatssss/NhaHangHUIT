namespace Nha_Hang_Huit.Models
{
    /// <summary>
    /// Ban an trong nha hang
    /// </summary>
    public class BanAn
    {
        public int MaBan { get; set; }
        public string TenBan { get; set; }
        public int MaKhuVuc { get; set; }
        public string TenKhuVuc { get; set; }
        public int SoChoNgoi { get; set; }
        public string TrangThai { get; set; } = "Trong";  // Trong | DangDung | DaDat

        /// <summary>So hoa don dang mo cho ban nay (0 = khong co)</summary>
        public int SoHoaDonDangMo { get; set; }

        /// <summary>Tong tien tam tinh cho ban (neu co hoa don dang mo)</summary>
        public decimal TongTienTamTinh { get; set; }

        public int Cot { get; set; }
        public int Hang { get; set; }

        // Status helpers
        public bool IsTrong => TrangThai == "Trong";
        public bool IsDangDung => TrangThai == "DangDung";
        public bool IsDaDat => TrangThai == "DaDat";

        public string TrangThaiDisplay
        {
            get
            {
                switch (TrangThai)
                {
                    case "DangDung": return $"Đang dùng ({(SoHoaDonDangMo > 0 ? SoHoaDonDangMo + " hóa đơn" : "")})";
                    case "DaDat": return "Đã đặt";
                    default: return "Trống";
                }
            }
        }

        public string TongTienDisplay => TongTienTamTinh > 0 ? $"{TongTienTamTinh:N0}₫" : "";
    }
}
