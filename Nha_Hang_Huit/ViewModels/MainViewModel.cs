using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Nha_Hang_Huit.Models;
using Nha_Hang_Huit.Services;

namespace Nha_Hang_Huit.ViewModels
{
    /// <summary>
    /// ViewModel cho form chinh / Dashboard
    /// </summary>
    public class MainViewModel : BaseViewModel
    {
        // Services
        public MonAnService MonAnService { get; } = new MonAnService();
        public KhachHangService KhachHangService { get; } = new KhachHangService();
        public HoaDonService HoaDonService { get; } = new HoaDonService();
        public BaoCaoService BaoCaoService { get; } = new BaoCaoService();
        public CartService Cart => CartService.Instance;

        // Thong tin nhan vien dang nhap
        private NhanVien _nhanVienHienTai;
        public NhanVien NhanVienHienTai
        {
            get => _nhanVienHienTai;
            set
            {
                SetProperty(ref _nhanVienHienTai, value);
                OnPropertyChanged(nameof(IsAdmin));
                OnPropertyChanged(nameof(IsNhanVien));
                OnPropertyChanged(nameof(NhanVienVisibility));
                OnPropertyChanged(nameof(AdminPanelVisibility));
                OnPropertyChanged(nameof(TenNhanVienText));
            }
        }

        public bool IsAdmin => _nhanVienHienTai?.IsAdmin ?? false;
        public bool IsNhanVien => !IsAdmin;
        public System.Windows.Visibility NhanVienVisibility => IsNhanVien ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        public System.Windows.Visibility AdminPanelVisibility => IsAdmin ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        public string TenNhanVienText => _nhanVienHienTai != null ? $"{_nhanVienHienTai.HoTen} ({_nhanVienHienTai.ChucVu})" : "";

        // Trang thai ca
        private int? _maCaHienTai;
        public int? MaCaHienTai
        {
            get => _maCaHienTai;
            set
            {
                SetProperty(ref _maCaHienTai, value);
                OnPropertyChanged(nameof(TrangThaiCaText));
                OnPropertyChanged(nameof(CoTheMoCa));
                OnPropertyChanged(nameof(CoTheDongCa));
                OnPropertyChanged(nameof(CoTheThaoTac));
                OnPropertyChanged(nameof(NoCaBlockedVisibility));
                OnPropertyChanged(nameof(CaDaMoVisibility));
            }
        }

        public string TrangThaiCaText => _maCaHienTai.HasValue
            ? $"CA #{_maCaHienTai} - DANG MO"
            : "CA: CHUA MO";

        public bool CoTheMoCa => !_maCaHienTai.HasValue;
        public bool CoTheDongCa => _maCaHienTai.HasValue;
        /// <summary>Nhan vien co the thao tac (da co ca dang mo)</summary>
        public bool CoTheThaoTac => _maCaHienTai.HasValue && IsNhanVien;
        /// <summary>Hien thi overlay can mo ca (nhan vien + chua mo ca)</summary>
        public System.Windows.Visibility NoCaBlockedVisibility =>
            (IsNhanVien && !_maCaHienTai.HasValue) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        /// <summary>Hien thi panel thao tac khi da mo ca</summary>
        public System.Windows.Visibility CaDaMoVisibility =>
            (IsNhanVien && _maCaHienTai.HasValue) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

        // Thong tin nhanh
        private int _tongHoaDonHomNay;
        public int TongHoaDonHomNay
        {
            get => _tongHoaDonHomNay;
            set => SetProperty(ref _tongHoaDonHomNay, value);
        }

        private decimal _doanhThuHomNay;
        public decimal DoanhThuHomNay
        {
            get => _doanhThuHomNay;
            set => SetProperty(ref _doanhThuHomNay, value);
        }

        public string DoanhThuText => $"{DoanhThuHomNay:N0} VND";
        public string TongHoaDonText => $"Hoa don: {TongHoaDonHomNay}";

        // Gio hang binding
        public ObservableCollection<ChiTietHoaDon> GioHangItems { get; } = new ObservableCollection<ChiTietHoaDon>();

        private decimal _tongTienGioHang;
        public decimal TongTienGioHang
        {
            get => _tongTienGioHang;
            set => SetProperty(ref _tongTienGioHang, value);
        }

        public string TongTienGioHangText => $"{TongTienGioHang:N0} VND";

        private decimal _giamGiaHienTai;
        public decimal GiamGiaHienTai
        {
            get => _giamGiaHienTai;
            set => SetProperty(ref _giamGiaHienTai, value);
        }

        public string GiamGiaText => $"{GiamGiaHienTai:N0} VND";

        private string _khachHangInfo = "(Chua chon)";
        public string KhachHangInfo
        {
            get => _khachHangInfo;
            set => SetProperty(ref _khachHangInfo, value);
        }

        private string _thongBaoHienTai = "Chao mung den voi Nha Hang HUIT!";
        public string ThongBaoHienTai
        {
            get => _thongBaoHienTai;
            set => SetProperty(ref _thongBaoHienTai, value);
        }

        // Commands
        public ICommand MoCaCommand { get; }
        public ICommand DongCaCommand { get; }
        public ICommand MoMenuCommand { get; }
        public ICommand MoThanhToanCommand { get; }
        public ICommand MoKhachHangCommand { get; }
        public ICommand MoBaoCaoCommand { get; }
        public ICommand MoTopMonCommand { get; }
        public ICommand XoaGioHangCommand { get; }
        public ICommand RefreshCommand { get; }

        // Events de View xu ly mo dialog
        public event EventHandler MoMenuTriggered;
        public event EventHandler MoThanhToanTriggered;
        public event EventHandler MoKhachHangTriggered;
        public event EventHandler<int> MoBaoCaoTriggered;
        public event EventHandler<int> MoTopMonTriggered;

        public MainViewModel()
        {
            MoCaCommand = new RelayCommand(OnMoCa, _ => CoTheMoCa);
            DongCaCommand = new RelayCommand(OnDongCa, _ => CoTheDongCa);
            MoMenuCommand = new RelayCommand(_ => MoMenuTriggered?.Invoke(this, EventArgs.Empty), _ => CoTheThaoTac);
            MoThanhToanCommand = new RelayCommand(_ => MoThanhToanTriggered?.Invoke(this, EventArgs.Empty), _ => CoTheThaoTac);
            MoKhachHangCommand = new RelayCommand(_ => MoKhachHangTriggered?.Invoke(this, EventArgs.Empty), _ => CoTheThaoTac);
            MoBaoCaoCommand = new RelayCommand(_ =>
            {
                if (MaCaHienTai.HasValue)
                    MoBaoCaoTriggered?.Invoke(this, MaCaHienTai.Value);
                else
                    MoBaoCaoTriggered?.Invoke(this, 0);
            });
            MoTopMonCommand = new RelayCommand(_ =>
            {
                if (MaCaHienTai.HasValue)
                    MoTopMonTriggered?.Invoke(this, MaCaHienTai.Value);
                else
                    MoTopMonTriggered?.Invoke(this, 0);
            });
            XoaGioHangCommand = new RelayCommand(_ =>
            {
                Cart.XoaGioHang();
                HienThiGioHang();
            }, _ => CoTheThaoTac);
            RefreshCommand = new RelayCommand(_ =>
            {
                CapNhatThongTin();
                HienThiGioHang();
                ThongBaoHienTai = "Da cap nhat thong tin!";
            });

            Cart.GioHangChanged += (s, e) => HienThiGioHang();
        }

        public void KhoiTao()
        {
            MaCaHienTai = HoaDonService.GetCurrentCa();
            CapNhatThongTin();
            HienThiGioHang();
            if (IsAdmin)
                ThongBaoHienTai = "Chao mung Quan Ly! Xem bao cao va doanh so ben duoi.";
            else if (!MaCaHienTai.HasValue)
                ThongBaoHienTai = "Vui long mo ca de bat dau thao tac!";
            else
                ThongBaoHienTai = "Da mo ca - Chon chuc nang de thao tac.";
        }

        public void CapNhatThongTin()
        {
            if (!MaCaHienTai.HasValue)
            {
                TongHoaDonHomNay = 0;
                DoanhThuHomNay = 0;
                return;
            }

            var hoaDons = HoaDonService.GetDaThanhToanByCa(MaCaHienTai.Value);
            TongHoaDonHomNay = hoaDons.Count;
            DoanhThuHomNay = hoaDons.Sum(h => h.ThanhTien);
        }

        public void HienThiGioHang()
        {
            GioHangItems.Clear();
            foreach (var item in Cart.GioHang)
                GioHangItems.Add(item);

            TongTienGioHang = Cart.TinhTongTien();
            GiamGiaHienTai = Cart.TienGiamGiaHienTai;

            if (Cart.MaKhachHangHienTai.HasValue)
            {
                var kh = KhachHangService.GetById(Cart.MaKhachHangHienTai.Value);
                KhachHangInfo = kh != null ? $"{kh.TenKhachHang} ({kh.HangThe})" : "(Chua chon)";
            }
            else
                KhachHangInfo = "(Chua chon)";

            OnPropertyChanged(nameof(TongTienGioHangText));
            OnPropertyChanged(nameof(GiamGiaText));
            OnPropertyChanged(nameof(DoanhThuText));
            OnPropertyChanged(nameof(TongHoaDonText));
        }

        private void OnMoCa(object obj)
        {
            int maNv = _nhanVienHienTai?.MaNhanVien ?? 1;
            int maCa = BaoCaoService.MoCa(maNv);
            if (maCa > 0)
            {
                MaCaHienTai = maCa;
                CapNhatThongTin();
                ThongBaoHienTai = $"Da mo ca #{maCa} thanh cong!";
                // Refresh command can-execute for all operations
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private void OnDongCa(object obj)
        {
            if (!MaCaHienTai.HasValue) return;

            var ca = BaoCaoService.DongCa(MaCaHienTai.Value);
            if (ca != null)
            {
                MoBaoCaoTriggered?.Invoke(this, MaCaHienTai.Value);
                MaCaHienTai = null;
                CapNhatThongTin();
                Cart.XoaGioHang();
                HienThiGioHang();
                ThongBaoHienTai = "Da dong ca thanh cong!";
                CommandManager.InvalidateRequerySuggested();
            }
        }
    }
}
