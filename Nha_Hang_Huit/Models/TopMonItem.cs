namespace Nha_Hang_Huit.Models
{
    /// <summary>
    /// Item thong ke top mon ban chay (dung chung giua ShiftReportViewModel va TopMonViewModel)
    /// </summary>
    public class TopMonItem
    {
        public int STT { get; set; }
        public string TenMonAn { get; set; }
        public int SoLuong { get; set; }
        public decimal DoanhThu { get; set; }
        public string DoanhThuText => $"{DoanhThu:N0} VND";
    }
}
