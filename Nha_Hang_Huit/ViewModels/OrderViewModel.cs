using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Nha_Hang_Huit.Models;
using Nha_Hang_Huit.Services;

namespace Nha_Hang_Huit.ViewModels
{
    /// <summary>
    /// ViewModel cho man hinh goi mon
    /// </summary>
    public class OrderViewModel : BaseViewModel
    {
        private readonly MonAnService _monAnService = new MonAnService();
        private readonly CartService _cart = CartService.Instance;

        // Thong tin ban hien tai
        private BanAn _banHienTai;
        public BanAn BanHienTai
        {
            get => _banHienTai;
            set
            {
                SetProperty(ref _banHienTai, value);
                OnPropertyChanged(nameof(BanTitleText));
            }
        }

        public string BanTitleText => BanHienTai != null
            ? $"Ban {BanHienTai.TenBan} - {BanHienTai.TenKhuVuc}"
            : "Goi mon";

        // Danh sach nhom mon
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

        // Danh sach mon an
        public ObservableCollection<MonAn> MonAnList { get; } = new ObservableCollection<MonAn>();

        private MonAn _monDaChon;
        public MonAn MonDaChon
        {
            get => _monDaChon;
            set
            {
                SetProperty(ref _monDaChon, value);
                OnPropertyChanged(nameof(CoChonMon));
                OnPropertyChanged(nameof(MonDaChonDisplay));
                OnPropertyChanged(nameof(GiaDaChonDisplay));
                if (value != null)
                    SoLuong = 1;
            }
        }

        public bool CoChonMon => MonDaChon != null;
        public string MonDaChonDisplay => MonDaChon?.TenMonAn ?? "(chua chon)";
        public string GiaDaChonDisplay => MonDaChon != null ? $"{MonDaChon.Gia:N0} VND" : "0 VND";

        // So luong
        private int _soLuong = 1;
        public int SoLuong
        {
            get => _soLuong;
            set => SetProperty(ref _soLuong, value < 1 ? 1 : value > 99 ? 99 : value);
        }

        // Gio hang tam
        public ObservableCollection<ChiTietHoaDon> GioHangItems { get; } = new ObservableCollection<ChiTietHoaDon>();

        private string _tongGioHangText = "0 VND";
        public string TongGioHangText
        {
            get => _tongGioHangText;
            set => SetProperty(ref _tongGioHangText, value);
        }

        // Commands
        public ICommand TangSLCommand { get; }
        public ICommand GiamSLCommand { get; }
        public ICommand ThemVaoGioCommand { get; }
        public ICommand XoaChonCommand { get; }
        public ICommand DongCommand { get; }

        public event EventHandler DongYeuCau;

        public OrderViewModel(BanAn ban = null)
        {
            BanHienTai = ban;

            TangSLCommand = new RelayCommand(_ => SoLuong = Math.Min(SoLuong + 1, 99));
            GiamSLCommand = new RelayCommand(_ => SoLuong = Math.Max(SoLuong - 1, 1));
            ThemVaoGioCommand = new RelayCommand(OnThemVaoGio, _ => MonDaChon != null);
            XoaChonCommand = new RelayCommand(OnXoaChon);
            DongCommand = new RelayCommand(_ => DongYeuCau?.Invoke(this, EventArgs.Empty));

            _cart.GioHangChanged += (s, e) => HienThiGioHangTam();
            Load();
        }

        public void Load()
        {
            TaiNhomMon();
            HienThiGioHangTam();
        }

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
            _cart.ThemVaoGioHang(MonDaChon, SoLuong);
        }

        private void OnXoaChon(object obj)
        {
            if (obj is ChiTietHoaDon item)
                _cart.XoaKhoiGioHang(item.MaMonAn);
        }

        private void HienThiGioHangTam()
        {
            GioHangItems.Clear();
            foreach (var item in _cart.GioHang)
                GioHangItems.Add(item);

            TongGioHangText = $"{_cart.TinhTongTien():N0} VND";
        }
    }
}
