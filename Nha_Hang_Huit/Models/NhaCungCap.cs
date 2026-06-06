using System;

namespace Nha_Hang_Huit.Models
{
    /// <summary>
    /// Model nha cung cap - schema tblNhaCungCap
    /// </summary>
    public class NhaCungCap
    {
        public int MaNhaCungCap { get; set; }
        public string TenNhaCungCap { get; set; }
        public string DiaChi { get; set; }
        public string SoDienThoai { get; set; }
        public string Email { get; set; }
        public string GhiChu { get; set; }
    }
}
