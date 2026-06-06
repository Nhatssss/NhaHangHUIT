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
    /// ViewModel cho man hinh thong ke top mon ban chay (admin)
    /// Doc lap khoi bao cao dong ca, co the xem bat ky luc nao
    /// </summary>
    public class TopMonViewModel : BaseViewModel
    {
        private readonly BaoCaoService _baoCaoService = new BaoCaoService();
        private readonly HoaDonService _hoaDonService = new HoaDonService();
        private readonly int _maCa;
        private DataTable _topMonData;

        public string MaCaText { get; private set; }

        // Top mon
        public ObservableCollection<TopMonItem> TopMonList { get; } = new ObservableCollection<TopMonItem>();

        private string _tongText;
        public string TongText
        {
            get => _tongText;
            set => SetProperty(ref _tongText, value);
        }

        // Commands
        public ICommand XuatBaoCaoCommand { get; }
        public ICommand DongCommand { get; }

        public event EventHandler DongYeuCau;

        public TopMonViewModel(int maCa)
        {
            _maCa = maCa;
            MaCaText = $"Ca #{maCa}";
            XuatBaoCaoCommand = new RelayCommand(OnXuatBaoCao);
            DongCommand = new RelayCommand(_ => DongYeuCau?.Invoke(this, EventArgs.Empty));
        }

        public void Load()
        {
            TaiTopMonBanChay();
        }

        private void TaiTopMonBanChay()
        {
            _topMonData = _baoCaoService.GetTopMonBanChay(_maCa);
            TopMonList.Clear();
            int stt = 1;
            decimal tongDoanhThu = 0;
            int tongSoLuong = 0;

            foreach (DataRow row in _topMonData.Rows)
            {
                var sl = Convert.ToInt32(row["TongSoLuong"]);
                var dt = Convert.ToDecimal(row["TongDoanhThu"]);
                TopMonList.Add(new TopMonItem
                {
                    STT = stt++,
                    TenMonAn = row["TenMonAn"].ToString(),
                    SoLuong = sl,
                    DoanhThu = dt
                });
                tongSoLuong += sl;
                tongDoanhThu += dt;
            }

            TongText = $"Tong: {TopMonList.Count} mon | {tongSoLuong} phan | {tongDoanhThu:N0} VND";
        }

        private void OnXuatBaoCao(object obj)
        {
            var caInfo = _baoCaoService.GetCaInfo(_maCa);
            if (caInfo == null)
            {
                System.Windows.MessageBox.Show("Khong the xuat bao cao vi thong tin ca khong hop le!", "Loi",
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return;
            }

            var dialog = new SaveFileDialog
            {
                Title = "Luu bao cao top mon ban chay",
                Filter = "Text files (*.txt)|*.txt",
                FileName = $"TopMonBanChay_Ca_{_maCa}_{DateTime.Now:yyyyMMdd_HHmmss}.txt"
            };

            if (dialog.ShowDialog() == true)
            {
                bool result = _baoCaoService.XuatBaoCaoTxt(caInfo, _topMonData, dialog.FileName);
                if (result)
                {
                    System.Windows.MessageBox.Show($"Xuat bao cao thanh cong!\nFile: {dialog.FileName}", "Thanh Cong",
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                }
            }
        }

    }
}
