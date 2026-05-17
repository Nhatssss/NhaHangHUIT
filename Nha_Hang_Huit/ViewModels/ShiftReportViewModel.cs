using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows.Input;
using Nha_Hang_Huit.Models;
using Nha_Hang_Huit.Services;
using Microsoft.Win32;

namespace Nha_Hang_Huit.ViewModels
{
    /// <summary>
    /// ViewModel cho man hinh bao cao dong ca
    /// </summary>
    public class ShiftReportViewModel : BaseViewModel
    {
        private readonly BaoCaoService _baoCaoService = new BaoCaoService();
        private readonly int _maCa;
        private LichSuCa _caInfo;
        private DataTable _topMonData;

        // Thong tin ca
        public string MaCaText => $"Ca #{_maCa}";
        public string GioBatDauText { get; private set; } = "Bat dau: --";
        public string GioKetThucText { get; private set; } = "Ket thuc: --";
        public string NhanVienText { get; private set; } = "--";
        public string TongHoaDonText { get; private set; } = "Tong hoa don: 0";
        public string DoanhThuTruocGiamText { get; private set; } = "Truoc giam gia: 0 VND";
        public string TongGiamGiaText { get; private set; } = "Giam gia: 0 VND";
        public string DoanhThuThucNhanText { get; private set; } = "Thuc nhan: 0 VND";
        public string SoKhachMoiText { get; private set; } = "Khach moi: 0";

        // Top mon
        public ObservableCollection<TopMonItem> TopMonList { get; } = new ObservableCollection<TopMonItem>();

        // Commands
        public ICommand XuatBaoCaoCommand { get; }
        public ICommand DongCommand { get; }

        public event EventHandler DongYeuCau;

        public ShiftReportViewModel(int maCa)
        {
            _maCa = maCa;
            XuatBaoCaoCommand = new RelayCommand(OnXuatBaoCao);
            DongCommand = new RelayCommand(_ => DongYeuCau?.Invoke(this, EventArgs.Empty));

            Load();
        }

        public void Load()
        {
            TaiThongTinCa();
            TaiTopMonBanChay();
        }

        private void TaiThongTinCa()
        {
            _caInfo = _baoCaoService.GetCaInfo(_maCa);
            if (_caInfo == null) return;

            var ci = _caInfo;
            GioBatDauText = $"Bat dau: {ci.GioBatDau:dd/MM/yyyy HH:mm:ss}";
            GioKetThucText = $"Ket thuc: {ci.GioKetThuc:dd/MM/yyyy HH:mm:ss}";
            NhanVienText = $"Nhan vien: {ci.NhanVien}";
            TongHoaDonText = $"Tong hoa don: {ci.TongSoHoaDon}";
            DoanhThuTruocGiamText = $"Truoc giam gia: {ci.TongDoanhThuTruocGiamGia:N0} VND";
            TongGiamGiaText = $"Giam gia: {ci.TongTienGiamGia:N0} VND";
            DoanhThuThucNhanText = $"Thuc nhan: {ci.TongDoanhThuThucNhan:N0} VND";
            SoKhachMoiText = $"Khach moi: {ci.SoKhachMoi}";

            OnPropertyChanged(nameof(GioBatDauText));
            OnPropertyChanged(nameof(GioKetThucText));
            OnPropertyChanged(nameof(NhanVienText));
            OnPropertyChanged(nameof(TongHoaDonText));
            OnPropertyChanged(nameof(DoanhThuTruocGiamText));
            OnPropertyChanged(nameof(TongGiamGiaText));
            OnPropertyChanged(nameof(DoanhThuThucNhanText));
            OnPropertyChanged(nameof(SoKhachMoiText));
        }

        private void TaiTopMonBanChay()
        {
            _topMonData = _baoCaoService.GetTopMonBanChay(_maCa);
            TopMonList.Clear();
            int stt = 1;

            foreach (DataRow row in _topMonData.Rows)
            {
                TopMonList.Add(new TopMonItem
                {
                    STT = stt++,
                    TenMonAn = row["TenMonAn"].ToString(),
                    SoLuong = Convert.ToInt32(row["TongSoLuong"]),
                    DoanhThu = Convert.ToDecimal(row["TongDoanhThu"])
                });
            }
        }

        private void OnXuatBaoCao(object obj)
        {
            var dialog = new SaveFileDialog
            {
                Title = "Luu bao cao",
                Filter = "Text files (*.txt)|*.txt",
                FileName = $"BaoCao_Ca_{_maCa}_{DateTime.Now:yyyyMMdd_HHmmss}.txt"
            };

            if (dialog.ShowDialog() == true)
            {
                bool result = _baoCaoService.XuatBaoCaoTxt(_caInfo, _topMonData, dialog.FileName);
                if (result)
                {
                    System.Windows.MessageBox.Show($"Xuat bao cao thanh cong!\nFile: {dialog.FileName}", "Thanh Cong",
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                }
            }
        }

        public class TopMonItem
        {
            public int STT { get; set; }
            public string TenMonAn { get; set; }
            public int SoLuong { get; set; }
            public decimal DoanhThu { get; set; }
        }
    }
}
