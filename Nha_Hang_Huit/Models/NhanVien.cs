namespace Nha_Hang_Huit.Models
{
    /// <summary>
    /// Model nhan vien - schema tblNhanVien
    /// </summary>
    public class NhanVien
    {
        public int MaNhanVien { get; set; }
        public string HoTen { get; set; }
        public string TaiKhoan { get; set; }
        public string MatKhau { get; set; }
        public string ChucVu { get; set; }
        public bool TrangThai { get; set; } = true;

        /// <summary>Kiem tra co phai admin hay khong dua tren ChucVu</summary>
        public bool IsAdmin => ChucVu == "Quan Ly";

        /// <summary>Hien thi tren UI</summary>
        public string HienThi => $"{HoTen} - {ChucVu}";
    }
}
