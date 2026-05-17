namespace Nha_Hang_Huit.Models
{
    /// <summary>
    /// Model hang the khach hang - schema tblHangThe
    /// </summary>
    public class HangThe
    {
        public int MaHang { get; set; }
        public string TenHang { get; set; }
        public double TiLeGiamGia { get; set; }
        public int? DiemToiThieu { get; set; }
        public string MoTa { get; set; }
        public bool TrangThai { get; set; } = true;
    }
}
