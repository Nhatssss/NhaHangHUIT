using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Nha_Hang_Huit.Models;
using Nha_Hang_Huit.Services;

namespace Nha_Hang_Huit.ViewModels
{
    /// <summary>
    /// ViewModel cho man hinh quan ly khach hang
    /// </summary>
    public class CustomerViewModel : BaseViewModel
    {
        private readonly KhachHangService _khachHangService = new KhachHangService();
        private readonly CartService _cart = CartService.Instance;

        // Danh sach khach hang
        public ObservableCollection<KhachHang> KhachHangList { get; } = new ObservableCollection<KhachHang>();

        private string _tongKHText = "Tong so: 0 khach hang";
        public string TongKHText
        {
            get => _tongKHText;
            set => SetProperty(ref _tongKHText, value);
        }

        private string _filterText = "";
        public string FilterText
        {
            get => _filterText;
            set
            {
                if (SetProperty(ref _filterText, value))
                    TaiDanhSachKH(value);
            }
        }

        // Tim kiem
        private string _soDienThoaiTim = "";
        public string SoDienThoaiTim
        {
            get => _soDienThoaiTim;
            set => SetProperty(ref _soDienThoaiTim, value);
        }

        private KhachHang _khTimDuoc;
        public KhachHang KhTimDuoc
        {
            get => _khTimDuoc;
            set
            {
                SetProperty(ref _khTimDuoc, value);
                OnPropertyChanged(nameof(CoThongTinKH));
                OnPropertyChanged(nameof(TenKH));
                OnPropertyChanged(nameof(SDTKH));
                OnPropertyChanged(nameof(DiemKH));
                OnPropertyChanged(nameof(HangTheKH));
            }
        }

        public bool CoThongTinKH => KhTimDuoc != null;
        public string TenKH => KhTimDuoc?.TenKhachHang ?? "";
        public string SDTKH => KhTimDuoc != null ? $"SDT: {KhTimDuoc.SoDienThoai}" : "";
        public string DiemKH => KhTimDuoc != null ? $"{KhTimDuoc.TongDiemTichLuy} diem" : "";
        public string HangTheKH => KhTimDuoc?.HangThe ?? "";

        // Them khach moi
        private string _tenKHMoi = "";
        public string TenKHMoi
        {
            get => _tenKHMoi;
            set => SetProperty(ref _tenKHMoi, value);
        }

        private string _sdtKHMoi = "";
        public string SdtKHMoi
        {
            get => _sdtKHMoi;
            set => SetProperty(ref _sdtKHMoi, value);
        }

        // Commands
        public ICommand TimCommand { get; }
        public ICommand ThemCommand { get; }
        public ICommand ChonKHCommand { get; }

        public event EventHandler DongYeuCau;

        public CustomerViewModel()
        {
            TimCommand = new RelayCommand(OnTim);
            ThemCommand = new RelayCommand(OnThem);
            ChonKHCommand = new RelayCommand(OnChonKH, _ => KhTimDuoc != null);

            Load();
        }

        public void Load()
        {
            TaiDanhSachKH();
        }

        private void TaiDanhSachKH(string filter = "")
        {
            var list = _khachHangService.GetAll();
            if (!string.IsNullOrWhiteSpace(filter))
            {
                list = list.Where(k =>
                    k.TenKhachHang.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    k.SoDienThoai.Contains(filter)
                ).ToList();
            }

            KhachHangList.Clear();
            foreach (var kh in list)
                KhachHangList.Add(kh);

            TongKHText = $"Tong so: {list.Count} khach hang";
        }

        private void OnTim(object obj)
        {
            string sdt = SoDienThoaiTim?.Trim() ?? "";
            if (string.IsNullOrWhiteSpace(sdt))
            {
                System.Windows.MessageBox.Show("Nhap so dien thoai can tim!", "Thong bao",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                return;
            }

            KhTimDuoc = _khachHangService.GetByPhone(sdt);
            if (KhTimDuoc == null)
            {
                System.Windows.MessageBox.Show("Khong tim thay khach hang voi so dien thoai nay!", "Thong bao",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
        }

        private void OnThem(object obj)
        {
            string ten = TenKHMoi?.Trim() ?? "";
            string sdt = SdtKHMoi?.Trim() ?? "";

            if (string.IsNullOrWhiteSpace(ten) || string.IsNullOrWhiteSpace(sdt))
            {
                System.Windows.MessageBox.Show("Vui long nhap ten va so dien thoai!", "Thong bao",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return;
            }

            var newKh = new KhachHang { TenKhachHang = ten, SoDienThoai = sdt };
            int maKH = _khachHangService.Insert(newKh);
            if (maKH > 0)
            {
                System.Windows.MessageBox.Show($"Them khach hang thanh cong! Ma KH: #{maKH}", "Thanh Cong",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                TenKHMoi = "";
                SdtKHMoi = "";
                TaiDanhSachKH();
            }
        }

        private void OnChonKH(object obj)
        {
            if (KhTimDuoc == null) return;

            _cart.MaKhachHangHienTai = KhTimDuoc.MaKhachHang;
            decimal tongTien = _cart.TinhTongTien();
            decimal tienGiam = _khachHangService.TinhTienGiamGia(KhTimDuoc, tongTien);
            _cart.TienGiamGiaHienTai = tienGiam;

            System.Windows.MessageBox.Show(
                $"Da chon khach hang: {KhTimDuoc.TenKhachHang}\n" +
                $"Hang the: {KhTimDuoc.HangThe}\n" +
                $"Giam gia: {tienGiam:N0} VND ({(KhTimDuoc.LayTiLeGiamGia() * 100):F0}%)",
                "Chon khach hang",
                System.Windows.MessageBoxButton.OK,
                System.Windows.MessageBoxImage.Information);

            DongYeuCau?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Set khach hang khi click tu danh sach
        /// </summary>
        public void SetKhachHang(KhachHang kh)
        {
            KhTimDuoc = kh;
            if (kh != null)
                SoDienThoaiTim = kh.SoDienThoai;
        }
    }
}
