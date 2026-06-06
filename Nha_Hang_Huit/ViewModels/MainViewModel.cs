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
        // Services (private readonly — ViewModel duy nhat quan ly services)
        private readonly MonAnService _monAnService = new MonAnService();
        private readonly KhachHangService _khachHangService = new KhachHangService();
        private readonly HoaDonService _hoaDonService = new HoaDonService();
        private readonly BaoCaoService _baoCaoService = new BaoCaoService();
        private readonly BanAnService _banAnService = new BanAnService();
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
                OnPropertyChanged(nameof(MoCaVisibility));
                OnPropertyChanged(nameof(DongCaVisibility));
                OnPropertyChanged(nameof(ChucNangSectionVisibility));
            }
        }

        public string TrangThaiCaText => _maCaHienTai.HasValue
            ? $"CA #{_maCaHienTai} - DANG MO"
            : "CA: CHUA MO";

        public bool CoTheMoCa => !_maCaHienTai.HasValue;
        public bool CoTheDongCa => _maCaHienTai.HasValue;
        public bool CoTheThaoTac => _maCaHienTai.HasValue && (IsNhanVien || IsAdmin);

        // Visibility for merged sidebar sections
        public System.Windows.Visibility MoCaVisibility =>
            !_maCaHienTai.HasValue ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        public System.Windows.Visibility DongCaVisibility =>
            _maCaHienTai.HasValue ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        public System.Windows.Visibility ChucNangSectionVisibility =>
            _maCaHienTai.HasValue ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        public System.Windows.Visibility AdminSectionVisibility =>
            IsAdmin ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        public System.Windows.Visibility NoCaBlockedVisibility =>
            (IsNhanVien && !_maCaHienTai.HasValue) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        public System.Windows.Visibility CaDaMoVisibility =>
            (_maCaHienTai.HasValue) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

        // ==================== QUAN LY BAN ====================
        private readonly ObservableCollection<KhuVuc> _khuVucList = new ObservableCollection<KhuVuc>();
        public ObservableCollection<KhuVuc> KhuVucList => _khuVucList;

        private KhuVuc _khuVucSelected;
        public KhuVuc KhuVucSelected
        {
            get => _khuVucSelected;
            set
            {
                if (SetProperty(ref _khuVucSelected, value) && value != null)
                    TaiBanAn(value.MaKhuVuc);
            }
        }

        private readonly ObservableCollection<BanAn> _banAnList = new ObservableCollection<BanAn>();
        public ObservableCollection<BanAn> BanAnList => _banAnList;

        // Ban dang lam viec
        private BanAn _banHienTai;
        public BanAn BanHienTai
        {
            get => _banHienTai;
            set
            {
                SetProperty(ref _banHienTai, value);
                OnPropertyChanged(nameof(BanHienTaiText));
                OnPropertyChanged(nameof(BanHienTaiInfo));
                OnPropertyChanged(nameof(CoChonBan));
            }
        }

        public bool CoChonBan => BanHienTai != null;
        public string BanHienTaiText => BanHienTai != null ? $"Ban {BanHienTai.TenBan} ({BanHienTai.TenKhuVuc})" : "(Chua chon ban)";
        public string BanHienTaiInfo => BanHienTai != null
            ? $"{BanHienTai.TenBan} | {BanHienTai.SoChoNgoi} cho | {BanHienTai.TrangThaiDisplay}"
            : "";

        // For counting tables by status
        private int _soBanTrong;
        private int _soBanDangDung;
        private int _soBanDaDat;
        public int SoBanTrong { get => _soBanTrong; set => SetProperty(ref _soBanTrong, value); }
        public int SoBanDangDung { get => _soBanDangDung; set => SetProperty(ref _soBanDangDung, value); }
        public int SoBanDaDat { get => _soBanDaDat; set => SetProperty(ref _soBanDaDat, value); }

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

        // Visibility toggles
        private bool _showTableLayout = true;
        public bool ShowTableLayout
        {
            get => _showTableLayout;
            set
            {
                SetProperty(ref _showTableLayout, value);
                OnPropertyChanged(nameof(TableLayoutVisibility));
                OnPropertyChanged(nameof(OrderPanelVisibility));
            }
        }
        public System.Windows.Visibility TableLayoutVisibility => _showTableLayout ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        public System.Windows.Visibility OrderPanelVisibility => !_showTableLayout ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;

        // Commands
        public ICommand MoCaCommand { get; }
        public ICommand DongCaCommand { get; }
        public ICommand MoMenuCommand { get; }
        public ICommand MoThanhToanCommand { get; }
        public ICommand MoKhachHangCommand { get; }
        public ICommand MoBaoCaoCommand { get; }
        public ICommand MoBaoCaoAllCommand { get; }
        public ICommand MoStaffManagementCommand { get; }
        public ICommand MoInvoiceHistoryCommand { get; }
        public ICommand MoTopMonCommand { get; }
        public ICommand MoTopMonAllCommand { get; }
        public ICommand MoPhieuNhapCommand { get; }
        public ICommand XoaGioHangCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand DangXuatCommand { get; }

        // Table commands
        public ICommand ChonBanCommand { get; }
        public ICommand HuyChonBanCommand { get; }
        public ICommand GoiMonChoBanCommand { get; }
        public ICommand ThanhToanBanCommand { get; }

        // Events de View xu ly mo dialog
        public event EventHandler MoKhachHangTriggered;
        public event EventHandler<int> MoBaoCaoTriggered;
        public event EventHandler<int> MoTopMonTriggered;

        // Events cho admin xem bao cao tong hop (khong can mo ca)
        public event EventHandler MoBaoCaoAllTriggered;
        public event EventHandler MoTopMonAllTriggered;
        public event EventHandler MoPhieuNhapTriggered;

        // Quan ly
        public event EventHandler MoStaffManagementTriggered;
        public event EventHandler MoInvoiceHistoryTriggered;

        // Events
        public event EventHandler<BanAn> MoBanOrderPaymentTriggered;
        public event EventHandler DangXuatTriggered;

        public MainViewModel()
        {
            MoCaCommand = new RelayCommand(OnMoCa, _ => CoTheMoCa);
            DongCaCommand = new RelayCommand(OnDongCa, _ => CoTheDongCa);

            // Table selection first, then order/payment
            MoMenuCommand = new RelayCommand(OnMoMenu, _ => CoTheThaoTac);
            MoThanhToanCommand = new RelayCommand(OnMoThanhToan, _ => CoTheThaoTac && CoChonBan);
            MoKhachHangCommand = new RelayCommand(_ => MoKhachHangTriggered?.Invoke(this, EventArgs.Empty), _ => CoTheThaoTac);
            MoBaoCaoCommand = new RelayCommand(_ =>
            {
                if (MaCaHienTai.HasValue)
                    MoBaoCaoTriggered?.Invoke(this, MaCaHienTai.Value);
                else
                    SetAutoClearMessage(msg => ThongBaoHienTai = msg, "Chua co ca nao mo! Vui long mo ca truoc.");
            });
            MoBaoCaoAllCommand = new RelayCommand(_ => MoBaoCaoAllTriggered?.Invoke(this, EventArgs.Empty));
            MoStaffManagementCommand = new RelayCommand(_ => MoStaffManagementTriggered?.Invoke(this, EventArgs.Empty));
            MoInvoiceHistoryCommand = new RelayCommand(_ => MoInvoiceHistoryTriggered?.Invoke(this, EventArgs.Empty));
            MoTopMonCommand = new RelayCommand(_ =>
            {
                if (MaCaHienTai.HasValue)
                    MoTopMonTriggered?.Invoke(this, MaCaHienTai.Value);
                else
                    SetAutoClearMessage(msg => ThongBaoHienTai = msg, "Chua co ca nao mo! Vui long mo ca truoc.");
            });
            MoTopMonAllCommand = new RelayCommand(_ => MoTopMonAllTriggered?.Invoke(this, EventArgs.Empty));
            MoPhieuNhapCommand = new RelayCommand(_ => MoPhieuNhapTriggered?.Invoke(this, EventArgs.Empty));
            DangXuatCommand = new RelayCommand(_ => DangXuatTriggered?.Invoke(this, EventArgs.Empty));
            XoaGioHangCommand = new RelayCommand(_ =>
            {
                Cart.XoaGioHang();
                HienThiGioHang();
            }, _ => CoTheThaoTac);
            RefreshCommand = new RelayCommand(_ =>
            {
                CapNhatThongTin();
                HienThiGioHang();
                LoadTableData();
                SetAutoClearMessage(msg => ThongBaoHienTai = msg, "Da cap nhat thong tin!");
            });

            // Table commands
            ChonBanCommand = new RelayCommand<BanAn>(OnChonBan);
            HuyChonBanCommand = new RelayCommand(_ =>
            {
                BanHienTai = null;
                Cart.SoBanHienTai = null;
                Cart.TenBanHienTai = null;
            });
            GoiMonChoBanCommand = new RelayCommand(_ => MoBanOrderPaymentTriggered?.Invoke(this, BanHienTai), _ => CoChonBan && CoTheThaoTac);
            ThanhToanBanCommand = new RelayCommand(_ => MoBanOrderPaymentTriggered?.Invoke(this, BanHienTai), _ => CoChonBan && CoTheThaoTac);

            Cart.GioHangChanged += (s, e) => HienThiGioHang();
        }

        public void KhoiTao()
        {
            MaCaHienTai = _hoaDonService.GetCurrentCa();
            CapNhatThongTin();
            HienThiGioHang();
            if (IsAdmin)
            {
                ThongBaoHienTai = "Chao mung Quan Ly! Xem bao cao va doanh so ben duoi.";
            }
            else if (!MaCaHienTai.HasValue)
            {
                ThongBaoHienTai = "Vui long mo ca de bat dau thao tac!";
            }
            else
            {
                LoadTableData();
                ThongBaoHienTai = "Da mo ca - Chon ban de bat dau goi mon.";
            }
        }

        /// <summary>
        /// Load danh sach khu vuc + ban an
        /// </summary>
        public void LoadTableData()
        {
            if (!MaCaHienTai.HasValue) return;

            // Save selected area
            string selectedTenKv = _khuVucSelected?.TenKhuVuc;

            _khuVucList.Clear();
            var kvs = _banAnService.GetAllKhuVuc();
            foreach (var kv in kvs)
                _khuVucList.Add(kv);

            // Restore selection or pick first
            if (!string.IsNullOrEmpty(selectedTenKv))
            {
                var found = _khuVucList.FirstOrDefault(k => k.TenKhuVuc == selectedTenKv);
                if (found != null) KhuVucSelected = found;
                else if (_khuVucList.Count > 0) KhuVucSelected = _khuVucList[0];
            }
            else if (_khuVucList.Count > 0)
            {
                KhuVucSelected = _khuVucList[0];
            }
        }

        private void TaiBanAn(int maKhuVuc)
        {
            _banAnList.Clear();
            var bans = _banAnService.GetBanAnByKhuVuc(maKhuVuc);
            // Sort by hang, cot for grid layout
            bans = bans.OrderBy(b => b.Hang).ThenBy(b => b.Cot).ToList();
            foreach (var b in bans)
                _banAnList.Add(b);

            SoBanTrong = bans.Count(b => b.IsTrong);
            SoBanDangDung = bans.Count(b => b.IsDangDung);
            SoBanDaDat = bans.Count(b => b.IsDaDat);
        }

        public void CapNhatThongTin()
        {
            if (!MaCaHienTai.HasValue)
            {
                TongHoaDonHomNay = 0;
                DoanhThuHomNay = 0;
                return;
            }

            var hoaDons = _hoaDonService.GetDaThanhToanByCa(MaCaHienTai.Value);
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
                var kh = _khachHangService.GetById(Cart.MaKhachHangHienTai.Value);
                KhachHangInfo = kh != null ? $"{kh.TenKhachHang} ({kh.HangThe})" : "(Chua chon)";
            }
            else
                KhachHangInfo = "(Chua chon)";

            OnPropertyChanged(nameof(TongTienGioHangText));
            OnPropertyChanged(nameof(GiamGiaText));
            OnPropertyChanged(nameof(DoanhThuText));
            OnPropertyChanged(nameof(TongHoaDonText));
        }

        private void OnChonBan(BanAn ban)
        {
            if (ban == null) return;

            BanHienTai = ban;
            Cart.SoBanHienTai = ban.MaBan;
            Cart.TenBanHienTai = ban.TenBan;

            if (ban.IsTrong)
            {
                // Empty table → start new order + payment combined
                Cart.XoaGioHang();
            }
            
            // Mo combined view: order + payment cung luc
            MoBanOrderPaymentTriggered?.Invoke(this, ban);
        }

        private void OnMoMenu(object obj)
        {
            if (!CoTheThaoTac) return;

            if (!CoChonBan)
            {
                SetAutoClearMessage(msg => ThongBaoHienTai = msg, "Vui long chon ban truoc khi goi mon!");
                return;
            }
            MoBanOrderPaymentTriggered?.Invoke(this, BanHienTai);
        }

        private void OnMoThanhToan(object obj)
        {
            if (!CoTheThaoTac || !CoChonBan) return;
            MoBanOrderPaymentTriggered?.Invoke(this, BanHienTai);
        }

        private void OnMoCa(object obj)
        {
            int maNv = _nhanVienHienTai?.MaNhanVien ?? 1;
            int maCa = _baoCaoService.MoCa(maNv);
            if (maCa > 0)
            {
                MaCaHienTai = maCa;
                CapNhatThongTin();
                LoadTableData();
                SetAutoClearMessage(msg => ThongBaoHienTai = msg, $"Da mo ca #{maCa} thanh cong! Chon ban de bat dau.");
                ShowTableLayout = true;
                CommandManager.InvalidateRequerySuggested();
            }
        }

        private void OnDongCa(object obj)
        {
            if (!MaCaHienTai.HasValue) return;

            var ca = _baoCaoService.DongCa(MaCaHienTai.Value);
            if (ca != null)
            {
                MoBaoCaoTriggered?.Invoke(this, MaCaHienTai.Value);
                MaCaHienTai = null;
                CapNhatThongTin();
                Cart.XoaGioHang();
                HienThiGioHang();
                BanHienTai = null;
                _khuVucList.Clear();
                _banAnList.Clear();
                SetAutoClearMessage(msg => ThongBaoHienTai = msg, "Da dong ca thanh cong!");
                CommandManager.InvalidateRequerySuggested();
            }
        }


    }
}
