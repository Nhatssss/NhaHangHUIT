using System;

namespace Nha_Hang_Huit.Models
{
    /// <summary>
    /// Model mon an - schema tblMonAn (service maps GiaBan→Gia, DuongDanHinh→HinhAnh)
    /// </summary>
    public class MonAn
    {
        public int MaMonAn { get; set; }
        public string TenMonAn { get; set; }
        /// <summary>Map tu GiaBan trong DB</summary>
        public decimal Gia { get; set; }
        /// <summary>Map tu TenNhom (JOIN tblNhomMonAn)</summary>
        public string NhomMon { get; set; }
        /// <summary>Map tu DuongDanHinh trong DB</summary>
        public string HinhAnh { get; set; }
        public string MoTa { get; set; }
        /// <summary>"Con" hoac "Het" (map tu BIT TrangThai)</summary>
        public string TrangThai { get; set; } = "Con";
        public DateTime NgayTao { get; set; }
    }
}
