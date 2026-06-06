using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Nha_Hang_Huit.Services;

namespace Nha_Hang_Huit.ViewModels
{
    public class StaffListDisplay
    {
        public int MaNhanVien { get; set; }
        public string HoTen { get; set; }
        public string TaiKhoan { get; set; }
        public string ChucVu { get; set; }
        public string TrangThaiText => TrangThai ? "Dang lam" : "Da nghi";
        public bool TrangThai { get; set; }
    }

    public class StaffManagementViewModel : BaseViewModel
    {
        private readonly NhanVienService _nhanVienService = new NhanVienService();

        public ObservableCollection<StaffListDisplay> StaffList { get; } = new ObservableCollection<StaffListDisplay>();

        private int _tongNhanVien;
        public int TongNhanVien { get => _tongNhanVien; set => SetProperty(ref _tongNhanVien, value); }
        public string TongNhanVienText => $"Tong: {TongNhanVien} nhan vien";

        private int _dangLam;
        public int DangLam { get => _dangLam; set => SetProperty(ref _dangLam, value); }
        public string DangLamText => $"Dang lam: {DangLam}";

        public ICommand DongCommand { get; }

        public event System.EventHandler DongYeuCau;

        public StaffManagementViewModel()
        {
            DongCommand = new RelayCommand(_ => DongYeuCau?.Invoke(this, System.EventArgs.Empty));
            Load();
        }

        public void Load()
        {
            StaffList.Clear();
            var staff = _nhanVienService.GetAll();
            foreach (var s in staff)
            {
                StaffList.Add(new StaffListDisplay
                {
                    MaNhanVien = s.MaNhanVien,
                    HoTen = s.HoTen,
                    TaiKhoan = s.TaiKhoan,
                    ChucVu = s.ChucVu,
                    TrangThai = s.TrangThai
                });
            }

            TongNhanVien = staff.Count;
            DangLam = staff.Count(s => s.TrangThai);
        }
    }
}
