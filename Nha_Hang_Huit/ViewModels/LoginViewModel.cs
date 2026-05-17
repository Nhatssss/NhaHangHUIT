using System;
using System.Windows.Input;
using Nha_Hang_Huit.Models;
using Nha_Hang_Huit.Services;

namespace Nha_Hang_Huit.ViewModels
{
    /// <summary>
    /// ViewModel cho man hinh dang nhap
    /// </summary>
    public class LoginViewModel : BaseViewModel
    {
        private string _tenDangNhap = "admin";
        private string _matKhau;
        private string _thongBao;
        private bool _coThongBao;

        private readonly NhanVienService _nhanVienService = new NhanVienService();

        public string TenDangNhap
        {
            get => _tenDangNhap;
            set => SetProperty(ref _tenDangNhap, value);
        }

        public string MatKhau
        {
            get => _matKhau;
            set => SetProperty(ref _matKhau, value);
        }

        public string ThongBao
        {
            get => _thongBao;
            set => SetProperty(ref _thongBao, value);
        }

        public bool CoThongBao
        {
            get => _coThongBao;
            set => SetProperty(ref _coThongBao, value);
        }

        public ICommand DangNhapCommand { get; }

        /// <summary>
        /// Event gui thong tin nhan vien khi dang nhap thanh cong
        /// </summary>
        public event Action<NhanVien> DangNhapThanhCong;

        public LoginViewModel()
        {
            DangNhapCommand = new RelayCommand(OnDangNhap);
        }

        private void OnDangNhap(object obj)
        {
            string ten = TenDangNhap?.Trim() ?? "";
            string mk = MatKhau ?? "";

            if (string.IsNullOrEmpty(ten) || string.IsNullOrEmpty(mk))
            {
                ThongBao = "Vui long nhap ten dang nhap va mat khau!";
                CoThongBao = true;
                return;
            }

            var nhanVien = _nhanVienService.DangNhap(ten, mk);
            if (nhanVien != null)
            {
                // Tra ve thong tin nhan vien cho View xu ly
                DangNhapThanhCong?.Invoke(nhanVien);
            }
            else
            {
                ThongBao = "Ten dang nhap hoac mat khau khong dung!";
                CoThongBao = true;
            }
        }
    }
}
