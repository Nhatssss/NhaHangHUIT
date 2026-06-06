using System;
using Nha_Hang_Huit.Models;

namespace Nha_Hang_Huit.ViewModels
{
    /// <summary>
    /// ViewModel cho form in phieu nhap hang
    /// </summary>
    public class PhieuNhapReportViewModel : BaseViewModel
    {
        public PhieuNhap PhieuNhap { get; }
        public string TenQuan => "Nhà Hàng HUIT";
        public string DiaChiQuan => "01 Võ Văn Ngân, P. Linh Chiểu, TP. Thủ Đức";
        public string DienThoaiQuan => "0901 234 567 - 0909 888 777";
        public string SoPhieu => $"PN-{PhieuNhap.MaPhieuNhap:D5}";
        public string NgayIn => DateTime.Now.ToString("dd/MM/yyyy HH:mm");

        public PhieuNhapReportViewModel(PhieuNhap phieuNhap)
        {
            PhieuNhap = phieuNhap;
        }
    }
}
