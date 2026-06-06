using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Nha_Hang_Huit.Models;
using Nha_Hang_Huit.Services;

namespace Nha_Hang_Huit.ViewModels
{
    public class InvoiceDisplay
    {
        public int MaHoaDon { get; set; }
        public string NgayTao { get; set; }
        public string KhachHang { get; set; }
        public string SoDienThoai { get; set; }
        public string TongTienDisplay => $"{TongTien:N0} VND";
        public decimal TongTien { get; set; }
        public string GiamGiaDisplay => $"{GiamGia:N0} VND";
        public decimal GiamGia { get; set; }
        public string ThanhToanDisplay => $"{ThanhToan:N0} VND";
        public decimal ThanhToan { get; set; }
        public string PhuongThucTT { get; set; }
        public int? MaCa { get; set; }
    }

    public class InvoiceHistoryViewModel : BaseViewModel
    {
        private readonly HoaDonService _hoaDonService = new HoaDonService();

        public ObservableCollection<InvoiceDisplay> InvoiceList { get; } = new ObservableCollection<InvoiceDisplay>();

        private int _tongHoaDon;
        public int TongHoaDon { get => _tongHoaDon; set => SetProperty(ref _tongHoaDon, value); }
        public string TongHoaDonText => $"Tong: {TongHoaDon} hoa don";

        private decimal _tongDoanhThu;
        public decimal TongDoanhThu { get => _tongDoanhThu; set => SetProperty(ref _tongDoanhThu, value); }
        public string TongDoanhThuText => $"Doanh thu: {TongDoanhThu:N0} VND";

        private decimal _tongGiamGia;
        public decimal TongGiamGia { get => _tongGiamGia; set => SetProperty(ref _tongGiamGia, value); }
        public string TongGiamGiaText => $"Giam gia: {TongGiamGia:N0} VND";

        private decimal _tongThucNhan;
        public decimal TongThucNhan { get => _tongThucNhan; set => SetProperty(ref _tongThucNhan, value); }
        public string TongThucNhanText => $"Thuc nhan: {TongThucNhan:N0} VND";

        // Loc theo ngay
        private DateTime? _tuNgay;
        public DateTime? TuNgay { get => _tuNgay; set => SetProperty(ref _tuNgay, value); }

        private DateTime? _denNgay;
        public DateTime? DenNgay { get => _denNgay; set => SetProperty(ref _denNgay, value); }

        public ICommand DongCommand { get; }
        public ICommand LocCommand { get; }

        public event EventHandler DongYeuCau;

        public InvoiceHistoryViewModel()
        {
            DongCommand = new RelayCommand(_ => DongYeuCau?.Invoke(this, EventArgs.Empty));
            LocCommand = new RelayCommand(_ => Load());

            // Mac dinh: hom nay
            TuNgay = DateTime.Today;
            DenNgay = DateTime.Today.AddDays(1);

            Load();
        }

        public void Load()
        {
            var all = _hoaDonService.GetAllDaThanhToan();

            // Loc theo ngay neu co
            if (TuNgay.HasValue)
                all = all.Where(h => h.NgayTao >= TuNgay.Value).ToList();
            if (DenNgay.HasValue)
                all = all.Where(h => h.NgayTao < DenNgay.Value).ToList();

            InvoiceList.Clear();
            foreach (var hd in all)
            {
                InvoiceList.Add(new InvoiceDisplay
                {
                    MaHoaDon = hd.MaHoaDon,
                    NgayTao = hd.NgayTao.ToString("dd/MM/yyyy HH:mm"),
                    KhachHang = hd.MaKhachHang.HasValue ? "KH#" + hd.MaKhachHang.Value.ToString() : "(Le)",
                    SoDienThoai = "",
                    TongTien = hd.TongTien,
                    GiamGia = hd.TienGiamGia,
                    ThanhToan = hd.ThanhTien,
                    PhuongThucTT = hd.PhuongThucThanhToan,
                    MaCa = hd.MaCa
                });
            }

            TongHoaDon = InvoiceList.Count;
            TongDoanhThu = all.Sum(h => h.TongTien);
            TongGiamGia = all.Sum(h => h.TienGiamGia);
            TongThucNhan = all.Sum(h => h.ThanhTien);
        }
    }
}
