using System.Windows;
using System.Windows.Controls;
using Nha_Hang_Huit.Models;
using Nha_Hang_Huit.ViewModels;

namespace Nha_Hang_Huit.Views
{
    /// <summary>
    /// Man hinh dang nhap (MVVM)
    /// </summary>
    public partial class LoginView : Window
    {
        private readonly LoginViewModel _viewModel;

        public LoginView()
        {
            InitializeComponent();
            _viewModel = new LoginViewModel();
            DataContext = _viewModel;

            // Dang ky su kien dang nhap thanh cong -> mo MainView + truyen nhan vien
            _viewModel.DangNhapThanhCong += (nhanVien) =>
            {
                var mainView = new MainView(nhanVien);
                mainView.Show();
                this.Close();
            };
        }

        /// <summary>
        /// Lay mat khau tu PasswordBox de truyen vao ViewModel
        /// (PasswordBox khong ho tro binding vi ly do bao mat)
        /// </summary>
        private void TxtMatKhau_PasswordChanged(object sender, RoutedEventArgs e)
        {
            _viewModel.MatKhau = txtMatKhau.Password;
        }
    }
}
