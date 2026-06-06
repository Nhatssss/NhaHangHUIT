using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Nha_Hang_Huit.Models;
using Nha_Hang_Huit.Services;
using QRCoder;

namespace Nha_Hang_Huit.ViewModels
{
    /// <summary>
    /// ViewModel gop ca goi mon va thanh toan trong 1 man hinh
    /// Khi click vao ban -> mo cua so nay -> order + payment cung luc
    /// </summary>
    public class OrderPaymentViewModel : BaseViewModel
    {
        private readonly MonAnService _monAnService = new MonAnService();
        private readonly HoaDonService _hoaDonService = new HoaDonService();
        private readonly KhachHangService _khachHangService = new KhachHangService();
        private readonly BanAnService _banAnService = new BanAnService();
        private readonly CartService _cart = CartService.Instance;

        // ============================
        // THONG TIN BAN
        // ============================
        private BanAn _banHienTai;
        public BanAn BanHienTai
        {
            get => _banHienTai;
            set
            {
                SetProperty(ref _banHienTai, value);
                OnPropertyChanged(nameof(BanTitle));
            }
        }
        public string BanTitle => BanHienTai != null
            ? $"Ban {BanHienTai.TenBan} - {BanHienTai.TenKhuVuc}"
            : "Goi mon & Thanh toan";

        // ============================
        // MENU - NHOM MON
        // ============================
        public ObservableCollection<string> NhomMonList { get; } = new ObservableCollection<string>();

        private string _nhomMonSelected;
        public string NhomMonSelected
        {
            get => _nhomMonSelected;
            set
            {
                if (SetProperty(ref _nhomMonSelected, value))
                    TaiMonAnTheoNhom();
            }
        }

        // ============================
        // MENU - MON AN
        // ============================
        public ObservableCollection<MonAn> MonAnList { get; } = new ObservableCollection<MonAn>();

        private MonAn _monDaChon;
        public MonAn MonDaChon
        {
            get => _monDaChon;
            set
            {
                SetProperty(ref _monDaChon, value);
                OnPropertyChanged(nameof(MonDaChonInfo));
                if (value != null)
                    SoLuongMon = 1;
            }
        }
        public string MonDaChonInfo => MonDaChon != null
            ? $"{MonDaChon.TenMonAn} - {MonDaChon.Gia:N0} VND"
            : "(Chua chon mon)";

        private int _soLuongMon = 1;
        public int SoLuongMon
        {
            get => _soLuongMon;
            set => SetProperty(ref _soLuongMon, value < 1 ? 1 : value > 99 ? 99 : value);
        }

        // ============================
        // GIO HANG
        // ============================
        public ObservableCollection<ChiTietHoaDon> GioHangItems { get; } = new ObservableCollection<ChiTietHoaDon>();

        private string _tongGioHangText = "0 VND";
        public string TongGioHangText
        {
            get => _tongGioHangText;
            set => SetProperty(ref _tongGioHangText, value);
        }

        // ============================
        // THANH TOAN - TONG TIEN
        // ============================
        private decimal _tongTien;
        public decimal TongTien
        {
            get => _tongTien;
            set => SetProperty(ref _tongTien, value);
        }
        public string TongTienDisplay => $"{TongTien:N0} VND";

        private decimal _giamGia;
        public decimal GiamGia
        {
            get => _giamGia;
            set => SetProperty(ref _giamGia, value);
        }
        public string GiamGiaDisplay => $"{GiamGia:N0} VND";

        private decimal _thanhToan;
        public decimal ThanhToan
        {
            get => _thanhToan;
            set => SetProperty(ref _thanhToan, value);
        }
        public string ThanhToanDisplay => $"{ThanhToan:N0} VND";

        // ============================
        // THANH TOAN - KHACH HANG
        // ============================
        private string _soDienThoaiInput;
        public string SoDienThoaiInput
        {
            get => _soDienThoaiInput;
            set
            {
                if (SetProperty(ref _soDienThoaiInput, value))
                    TimGoiY();
            }
        }

        public ObservableCollection<KhachHang> DanhSachGoiY { get; } = new ObservableCollection<KhachHang>();

        private bool _coGoiY;
        public bool CoGoiY
        {
            get => _coGoiY;
            set => SetProperty(ref _coGoiY, value);
        }

        private KhachHang _khachHangDuocChon;
        public KhachHang KhachHangDuocChon
        {
            get => _khachHangDuocChon;
            set
            {
                if (SetProperty(ref _khachHangDuocChon, value))
                    ApDungKhachHang(value);
            }
        }

        private string _tenKhachHangInfo;
        public string TenKhachHangInfo
        {
            get => _tenKhachHangInfo;
            set => SetProperty(ref _tenKhachHangInfo, value);
        }

        private string _hangTheInfo;
        public string HangTheInfo
        {
            get => _hangTheInfo;
            set => SetProperty(ref _hangTheInfo, value);
        }

        private decimal _tiLeGiamGia;
        public decimal TiLeGiamGia
        {
            get => _tiLeGiamGia;
            set
            {
                if (SetProperty(ref _tiLeGiamGia, value))
                    OnPropertyChanged(nameof(TiLeGiamGiaDisplay));
            }
        }
        public string TiLeGiamGiaDisplay => $"Giam {TiLeGiamGia:P0}";

        // ============================
        // THANH TOAN - Tien khach dua
        // ============================
        private decimal _tienKhachDua;
        public decimal TienKhachDua
        {
            get => _tienKhachDua;
            set
            {
                if (SetProperty(ref _tienKhachDua, value))
                    TinhTienThua();
            }
        }

        private decimal _tienThua;
        public decimal TienThua
        {
            get => _tienThua;
            set
            {
                SetProperty(ref _tienThua, value);
                OnPropertyChanged(nameof(TienThuaDisplay));
            }
        }
        public string TienThuaDisplay => $"{TienThua:N0} VND";

        private string _thongBaoTT;
        public string ThongBaoTT
        {
            get => _thongBaoTT;
            set => SetProperty(ref _thongBaoTT, value);
        }
        private bool _coThongBaoTT;
        public bool CoThongBaoTT
        {
            get => _coThongBaoTT;
            set => SetProperty(ref _coThongBaoTT, value);
        }

        // ============================
        // THANH TOAN - Phuong thuc
        // ============================
        private bool _thanhToanTienMat = true;
        public bool ThanhToanTienMat
        {
            get => _thanhToanTienMat;
            set
            {
                if (SetProperty(ref _thanhToanTienMat, value) && value)
                    PhuongThucThanhToan = "TienMat";
            }
        }

        private bool _thanhToanQR;
        public bool ThanhToanQR
        {
            get => _thanhToanQR;
            set
            {
                if (SetProperty(ref _thanhToanQR, value) && value)
                    PhuongThucThanhToan = "QRCode";
            }
        }

        private string _phuongThucThanhToan = "TienMat";
        public string PhuongThucThanhToan
        {
            get => _phuongThucThanhToan;
            set => SetProperty(ref _phuongThucThanhToan, value);
        }

        // ============================
        // THANH TOAN - QR
        // ============================
        private BitmapImage _qrImage;
        public BitmapImage QRImage
        {
            get => _qrImage;
            set => SetProperty(ref _qrImage, value);
        }

        private string _qrInfo = "Chon QR de tao ma";
        public string QRInfo
        {
            get => _qrInfo;
            set => SetProperty(ref _qrInfo, value);
        }

        private bool _qrVisible;
        public bool QRVisible
        {
            get => _qrVisible;
            set
            {
                SetProperty(ref _qrVisible, value);
                OnPropertyChanged(nameof(QRInfoVisible));
            }
        }
        public bool QRInfoVisible => !QRVisible;

        private bool _daThanhToanQR;
        private int? _maHoaDonHienTai;
        private CancellationTokenSource _ctsQR;

        // ============================
        // COMMANDS
        // ============================
        public ICommand TangSLCommand { get; }
        public ICommand GiamSLCommand { get; }
        public ICommand ThemVaoGioCommand { get; }
        public ICommand XoaItemGioCommand { get; }
        public ICommand XoaGioHangCommand { get; }
        public ICommand XacNhanTTCommand { get; }
        public ICommand TaoQRCommand { get; }
        public ICommand DongCommand { get; }

        public event EventHandler DongYeuCau;

        // ============================
        // CONSTRUCTOR
        // ============================
        public OrderPaymentViewModel(BanAn ban = null)
        {
            BanHienTai = ban;
            _cart.SoBanHienTai = ban?.MaBan;
            _cart.TenBanHienTai = ban?.TenBan;

            TangSLCommand = new RelayCommand(_ => SoLuongMon = Math.Min(SoLuongMon + 1, 99));
            GiamSLCommand = new RelayCommand(_ => SoLuongMon = Math.Max(SoLuongMon - 1, 1));
            ThemVaoGioCommand = new RelayCommand(OnThemVaoGio, _ => MonDaChon != null);
            XoaItemGioCommand = new RelayCommand(OnXoaItemGio);
            XoaGioHangCommand = new RelayCommand(_ =>
            {
                _cart.XoaGioHang();
                HienThiGioHang();
                TinhToan();
            });
            XacNhanTTCommand = new RelayCommand(OnXacNhanTT);
            TaoQRCommand = new RelayCommand(OnTaoQR);
            DongCommand = new RelayCommand(_ => DongYeuCau?.Invoke(this, EventArgs.Empty));

            _cart.GioHangChanged += (s, e) =>
            {
                HienThiGioHang();
                TinhToan();
            };

            LoadData();
        }

        // ============================
        // KHOI TAO
        // ============================
        private void LoadData()
        {
            TaiNhomMon();
            HienThiGioHang();
            CapNhatHoaDonInfo();

            // Khoi phuc khach hang tu cart (neu co)
            if (_cart.MaKhachHangHienTai.HasValue)
            {
                var kh = _khachHangService.GetById(_cart.MaKhachHangHienTai.Value);
                if (kh != null)
                {
                    _khachHangDuocChon = kh;
                    _soDienThoaiInput = kh.SoDienThoai;
                    OnPropertyChanged(nameof(SoDienThoaiInput));
                    ApDungKhachHang(kh);
                }
            }
        }

        // ============================
        // MENU
        // ============================
        private void TaiNhomMon()
        {
            NhomMonList.Clear();
            var nhoms = _monAnService.GetAllNhomMon();
            foreach (var n in nhoms)
                NhomMonList.Add(n);

            if (NhomMonList.Count > 0)
                _nhomMonSelected = NhomMonList[0];

            OnPropertyChanged(nameof(NhomMonSelected));
            TaiMonAnTheoNhom();
        }

        private void TaiMonAnTheoNhom()
        {
            MonAnList.Clear();
            if (string.IsNullOrEmpty(NhomMonSelected)) return;

            var mons = _monAnService.GetByNhom(NhomMonSelected);
            foreach (var m in mons)
                MonAnList.Add(m);

            MonDaChon = null;
        }

        private void OnThemVaoGio(object obj)
        {
            if (MonDaChon == null) return;
            _cart.ThemVaoGioHang(MonDaChon, SoLuongMon);
        }

        private void OnXoaItemGio(object obj)
        {
            if (obj is ChiTietHoaDon item)
                _cart.XoaKhoiGioHang(item.MaMonAn);
        }

        // ============================
        // HIEN THI GIO HANG
        // ============================
        private void HienThiGioHang()
        {
            GioHangItems.Clear();
            foreach (var item in _cart.GioHang)
                GioHangItems.Add(item);

            TongGioHangText = $"{_cart.TinhTongTien():N0} VND";
        }

        private void CapNhatHoaDonInfo()
        {
            OnPropertyChanged(nameof(TongTien));
            OnPropertyChanged(nameof(GiamGia));
            OnPropertyChanged(nameof(ThanhToan));
            OnPropertyChanged(nameof(TongTienDisplay));
            OnPropertyChanged(nameof(GiamGiaDisplay));
            OnPropertyChanged(nameof(ThanhToanDisplay));
        }

        private void TinhToan()
        {
            TongTien = _cart.TinhTongTien();
            GiamGia = _cart.TienGiamGiaHienTai;
            ThanhToan = _cart.TinhThanhTien();
            CapNhatHoaDonInfo();
            TinhTienThua();
        }

        // ============================
        // TIM KIEM KHACH HANG
        // ============================
        private void TimGoiY()
        {
            DanhSachGoiY.Clear();
            string sdt = _soDienThoaiInput?.Trim() ?? "";

            if (string.IsNullOrEmpty(sdt) || sdt.Length < 1)
            {
                CoGoiY = false;
                return;
            }

            var results = _khachHangService.SearchByPhone(sdt);
            foreach (var kh in results)
                DanhSachGoiY.Add(kh);

            CoGoiY = DanhSachGoiY.Count > 0;
        }

        public void ChonKhachHangTuGoiY(KhachHang kh)
        {
            if (kh == null) return;
            KhachHangDuocChon = kh;
            SoDienThoaiInput = kh.SoDienThoai;
            CoGoiY = false;
        }

        private void ApDungKhachHang(KhachHang kh)
        {
            if (kh == null)
            {
                _cart.MaKhachHangHienTai = null;
                _cart.TienGiamGiaHienTai = 0;
                _cart.TiLeGiamGiaHienTai = 0;
                TenKhachHangInfo = "";
                HangTheInfo = "";
                TiLeGiamGia = 0;
                TinhToan();
                return;
            }

            _cart.MaKhachHangHienTai = kh.MaKhachHang;

            decimal tiLe = (decimal)KhachHang.LayTiLeGiamGia(kh.HangThe);
            TiLeGiamGia = tiLe;
            _cart.TienGiamGiaHienTai = Math.Round(_cart.TinhTongTien() * tiLe, 0);
            _cart.TiLeGiamGiaHienTai = tiLe;

            TenKhachHangInfo = kh.TenKhachHang;
            HangTheInfo = $"Hang: {kh.HangThe}";
            TinhToan();
        }

        // ============================
        // TINH TIEN THUA
        // ============================
        private void TinhTienThua()
        {
            TienThua = TienKhachDua - ThanhToan;
            if (TienThua >= 0)
            {
                CoThongBaoTT = false;
            }
            else
            {
                ThongBaoTT = "Khach chua du tien!";
                CoThongBaoTT = true;
            }
        }

        // ============================
        // XAC NHAN THANH TOAN
        // ============================
        private void OnXacNhanTT(object obj)
        {
            if (_cart.GioHang.Count == 0)
            {
                SetAutoClearMessage(msg => ThongBaoTT = msg, vis => CoThongBaoTT = vis,
                    "Gio hang trong! Vui long them mon truoc.");
                return;
            }

            if (PhuongThucThanhToan == "TienMat")
            {
                // Kiem tra tien mat
                if (TienKhachDua < ThanhToan)
                {
                    SetAutoClearMessage(msg => ThongBaoTT = msg, vis => CoThongBaoTT = vis,
                        "So tien khach dua khong hop le hoac chua du!");
                    return;
                }
            }

            // Tao hoa don
            int? maHD = _cart.TaoHoaDon(_hoaDonService, _banAnService);
            if (maHD == null) return;

            // Xac nhan thanh toan
            bool result = _cart.XacNhanThanhToan(
                maHD.Value,
                PhuongThucThanhToan,
                TienKhachDua,
                _hoaDonService,
                _khachHangService,
                _banAnService);

            if (result)
            {
                string msg = PhuongThucThanhToan == "TienMat"
                    ? $"Thanh toan thanh cong!\nTien thua: {TienThua:N0} VND\nMa Hoa Don: #{maHD}"
                    : $"Thanh toan QR thanh cong!\nMa Hoa Don: #{maHD}";

                System.Windows.MessageBox.Show(msg, "Thanh Cong",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Information);

                DongYeuCau?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                SetAutoClearMessage(msg => ThongBaoTT = msg, vis => CoThongBaoTT = vis,
                    "Thanh toan that bai! Vui long thu lai.");
            }
        }

        // ============================
        // QR CODE
        // ============================
        private void OnTaoQR(object obj)
        {
            try
            {
                if (_cart.GioHang.Count == 0)
                {
                    SetAutoClearMessage(msg => ThongBaoTT = msg, vis => CoThongBaoTT = vis,
                        "Gio hang trong! Vui long them mon truoc.");
                    return;
                }

                // Tao hoa don truoc khi tao QR
                _maHoaDonHienTai = _cart.TaoHoaDon(_hoaDonService, _banAnService);
                if (_maHoaDonHienTai == null) return;

                string noiDungQR = $"THANH TOAN HOA DON #{_maHoaDonHienTai} - SO TIEN: {ThanhToan:N0} VND - NHA HANG HUIT";

                using (var qrGenerator = new QRCodeGenerator())
                using (var qrData = qrGenerator.CreateQrCode(noiDungQR, QRCodeGenerator.ECCLevel.Q))
                using (var qrCode = new QRCode(qrData))
                using (var qrBitmap = qrCode.GetGraphic(20))
                using (var ms = new MemoryStream())
                {
                    qrBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    ms.Position = 0;
                    var bi = new BitmapImage();
                    bi.BeginInit();
                    bi.StreamSource = ms;
                    bi.CacheOption = BitmapCacheOption.OnLoad;
                    bi.EndInit();
                    QRImage = bi;
                }

                QRInfo = $"QR Hoa Don #{_maHoaDonHienTai}";
                QRVisible = true;
                _daThanhToanQR = false;

                // Tu dong kiem tra sau 60s - thuc te can tich hop QR scan
                BatDauDemNguocQR();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Loi tao QR Code: " + ex.Message, "Loi",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        private async void BatDauDemNguocQR()
        {
            _ctsQR?.Cancel();
            _ctsQR = new CancellationTokenSource();

            try
            {
                for (int i = 60; i >= 0; i--)
                {
                    if (_ctsQR.Token.IsCancellationRequested || _daThanhToanQR)
                        return;

                    await Task.Delay(1000, _ctsQR.Token);
                }

                // Het thoi gian -> tu dong huy
                if (!_daThanhToanQR && !_ctsQR.Token.IsCancellationRequested)
                {
                    QRVisible = false;
                    QRInfo = "QR het han. Vui long tao lai.";
                }
            }
            catch (TaskCanceledException) { }
        }
    }
}
