namespace Nha_Hang_Huit.Models
{
    /// <summary>
    /// Model nhom mon an - schema tblNhomMonAn
    /// </summary>
    public class NhomMonAn
    {
        public int MaNhom { get; set; }
        public string TenNhom { get; set; }
        public string MoTaNhom { get; set; }
        public int ThuTuHienThi { get; set; }
        public bool TrangThai { get; set; } = true;
    }
}
