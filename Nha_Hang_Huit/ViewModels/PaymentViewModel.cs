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
    /// ViewModel cho man hinh thanh toan
    /// </summary>
    public class PaymentViewModel : BaseViewModel
    {
        private readonly HoaDonService _hoaDonService = new HoaDonService();
        private readonly KhachHangService _khachHangService = new KhachHangService();
        private readonly CartService _cart = CartService.Instance;

        // ===== THONG TIN HOA DON =====
        private decimal _tongTien;
        public decimal TongTien
        {
            get => _tongTien;
            set => SetProperty(ref _tongTien, value);
        }
        public string TongTienText => $"{TongTien:N0} VND";

        private decimal _giamGia;
        public decimal GiamGia
        {
            get => _giamGia;
            set => SetProperty(ref _giamGia, value);
        }
        public string GiamGiaText => $"{GiamGia:N0} VND";

        private decimal _thanhToan;
        public decimal ThanhToan
        {
            get => _thanhToan;
            set => SetProperty(ref _thanhToan, value);
        }
        public string ThanhToanText => $"{ThanhToan:N0} VND";

        // ===== SO DIEN THOAI + TIM KHACH HANG =====
        private string _soDienThoaiInput;
        public string SoDienThoaiInput
        {
            get => _soDienThoaiInput;
            set
            {
                if (SetProperty(ref _soDienThoaiInput, value))
                {
                    TimGoiY();
                }
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
                {
                    ApDungKhachHang(value);
                }
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
                    OnPropertyChanged(nameof(TiLeGiamGiaText));
            }
        }
        public string TiLeGiamGiaText => $"Giam {TiLeGiamGia:P0}";

        // ===== TIEN KHACH DUA =====
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
            set => SetProperty(ref _tienThua, value);
        }
        public string TienThuaText => $"{TienThua:N0} VND";
        public bool DuTienTT => TienKhachDua >= ThanhToan;

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

        // ===== CHI TIET HOA DON =====
        public ObservableCollection<ChiTietHoaDon> ChiTietItems { get; } = new ObservableCollection<ChiTietHoaDon>();

        // ===== QR =====
        public string _phuongThucThanhToan = "TienMat";
        public string PhuongThucThanhToan
        {
            get => _phuongThucThanhToan;
            set => SetProperty(ref _phuongThucThanhToan, value);
        }

        private BitmapImage _qrImage;
        public BitmapImage QRImage
        {
            get => _qrImage;
            set => SetProperty(ref _qrImage, value);
        }

        private string _qrInfo = "Bam nut de tao ma QR";
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

        private string _qrCountdownText = "30s";
        public string QRCountdownText
        {
            get => _qrCountdownText;
            set => SetProperty(ref _qrCountdownText, value);
        }
        private bool _qrCountdownVisible;
        public bool QRCountdownVisible
        {
            get => _qrCountdownVisible;
            set => SetProperty(ref _qrCountdownVisible, value);
        }

        private string _qrResultText;
        public string QRResultText
        {
            get => _qrResultText;
            set => SetProperty(ref _qrResultText, value);
        }
        private bool _qrResultVisible;
        public bool QRResultVisible
        {
            get => _qrResultVisible;
            set => SetProperty(ref _qrResultVisible, value);
        }

        // ===== MA HOA DON =====
        private int? _maHoaDonHienTai;
        private CancellationTokenSource _ctsQR;
        private bool _daThanhToanQR;

        // ===== COMMANDS =====
        public ICommand XacNhanTTCommand { get; }
        public ICommand TaoQRCommand { get; }
        public ICommand GiaLapQRCommand { get; }
        public ICommand ChonKhachHangCommand { get; }

        public event EventHandler DongYeuCau;

        public PaymentViewModel()
        {
            XacNhanTTCommand = new RelayCommand(OnXacNhanTT);
            TaoQRCommand = new RelayCommand(OnTaoQR);
            GiaLapQRCommand = new RelayCommand(_ => GiaLapThanhCong(), _ => _maHoaDonHienTai.HasValue && !_daThanhToanQR);
            ChonKhachHangCommand = new RelayCommand(OnChonKhachHang);

            Load();
        }

        public void Load()
        {
            CapNhatHoaDonInfo();
            HienThiChiTiet();
            // Khoi phuc khach hang tu cart
            if (_cart.MaKhachHangHienTai.HasValue)
            {
                var kh = _khachHangService.GetById(_cart.MaKhachHangHienTai.Value);
                if (kh != null)
                {
                    _khachHangDuocChon = kh;
                    _soDienThoaiInput = kh.SoDienThoai;
                    OnPropertyChanged(nameof(SoDienThoaiInput));
                    HienThiThongTinKhachHang(kh);
                }
            }
        }

        private void CapNhatHoaDonInfo()
        {
            TongTien = _cart.TinhTongTien();
            GiamGia = _cart.TienGiamGiaHienTai;
            ThanhToan = _cart.TinhThanhTien();
            OnPropertyChanged(nameof(TongTienText));
            OnPropertyChanged(nameof(GiamGiaText));
            OnPropertyChanged(nameof(ThanhToanText));
        }

        private void HienThiChiTiet()
        {
            ChiTietItems.Clear();
            foreach (var item in _cart.GioHang)
                ChiTietItems.Add(item);
        }

        // ===== TIM KIEM SDT =====
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

        private void OnChonKhachHang(object obj)
        {
            var kh = obj as KhachHang;
            if (kh == null) return;
            KhachHangDuocChon = kh;
            SoDienThoaiInput = kh.SoDienThoai;
            CoGoiY = false;
        }

        private void ApDungKhachHang(KhachHang kh)
        {
            if (kh == null)
            {
                // Xoa khach hang
                _cart.MaKhachHangHienTai = null;
                _cart.TienGiamGiaHienTai = 0;
                TenKhachHangInfo = "";
                HangTheInfo = "";
                TiLeGiamGia = 0;
                CapNhatHoaDonInfo();
                return;
            }

            // Luu vao Cart
            _cart.MaKhachHangHienTai = kh.MaKhachHang;

            // Tinh giam gia theo hang
            decimal tiLe = (decimal)KhachHang.LayTiLeGiamGia(kh.HangThe);
            TiLeGiamGia = tiLe;
            _cart.TienGiamGiaHienTai = Math.Round(_cart.TinhTongTien() * tiLe, 0);
            _cart.TiLeGiamGiaHienTai = tiLe;

            HienThiThongTinKhachHang(kh);
            CapNhatHoaDonInfo();
        }

        private void HienThiThongTinKhachHang(KhachHang kh)
        {
            TenKhachHangInfo = kh.TenKhachHang;
            HangTheInfo = $"Hang: {kh.HangThe}";
        }

        // ===== TINH TIEN THUA =====
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
            OnPropertyChanged(nameof(DuTienTT));
            OnPropertyChanged(nameof(TienThuaText));
        }

        // ===== XAC NHAN TT =====
        private void OnXacNhanTT(object obj)
        {
            if (TienKhachDua < ThanhToan)
            {
                ThongBaoTT = "So tien khach dua khong hop le hoac chua du!";
                CoThongBaoTT = true;
                return;
            }

            int? maHD = _cart.TaoHoaDon(_hoaDonService);
            if (maHD == null) return;

            bool result = _cart.XacNhanThanhToan(maHD.Value, "TienMat", _hoaDonService, _khachHangService);
            if (result)
            {
                System.Windows.MessageBox.Show(
                    $"Thanh toan thanh cong!\nTien thua: {TienKhachDua - ThanhToan:N0} VND\nMa Hoa Don: #{maHD}",
                    "Thanh Cong",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Information);
                DongYeuCau?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                System.Windows.MessageBox.Show("Thanh toan that bai!", "Loi",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        // ===== QR =====
        private void OnTaoQR(object obj)
        {
            try
            {
                if (_maHoaDonHienTai == null)
                {
                    _maHoaDonHienTai = _cart.TaoHoaDon(_hoaDonService);
                    if (_maHoaDonHienTai == null) return;
                }

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
                QRCountdownVisible = true;
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
            _daThanhToanQR = false;

            try
            {
                for (int i = 30; i >= 0; i--)
                {
                    if (_ctsQR.Token.IsCancellationRequested || _daThanhToanQR)
                        return;

                    QRCountdownText = $"{i}s";
                    if (i > 0)
                        await Task.Delay(1000, _ctsQR.Token);
                }

                if (!_daThanhToanQR && !_ctsQR.Token.IsCancellationRequested)
                {
                    await System.Windows.Application.Current.Dispatcher.InvokeAsync(() =>
                        GiaLapThanhCong());
                }
            }
            catch (TaskCanceledException) { }
        }

        private void GiaLapThanhCong()
        {
            if (_daThanhToanQR || _maHoaDonHienTai == null) return;

            _daThanhToanQR = true;
            _ctsQR?.Cancel();

            bool result = _cart.XacNhanThanhToan(_maHoaDonHienTai.Value, "QRCode", _hoaDonService, _khachHangService);
            if (result)
            {
                QRResultText = $"✅ Thanh toan QR thanh cong!\nHoa Don #{_maHoaDonHienTai}";
                QRResultVisible = true;
                QRCountdownVisible = false;

                System.Windows.MessageBox.Show("Thanh toan QR thanh cong!", "Thanh Cong",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                DongYeuCau?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                QRResultText = "❌ Thanh toan that bai!";
                QRResultVisible = true;
            }
        }
    }
}
