using System.Windows;
using Nha_Hang_Huit.Models;
using Nha_Hang_Huit.ViewModels;

namespace Nha_Hang_Huit.Views
{
    /// <summary>
    /// Man hinh chinh - Dashboard (MVVM)
    /// </summary>
    public partial class MainView : Window
    {
        private readonly MainViewModel _viewModel;

        public MainView(NhanVien nhanVien)
        {
            InitializeComponent();

            _viewModel = new MainViewModel();
            _viewModel.NhanVienHienTai = nhanVien;
            DataContext = _viewModel;

            // Dang ky su kien mo dialog
            _viewModel.MoMenuTriggered += (s, e) =>
            {
                var orderView = new OrderView();
                orderView.ShowDialog();
                _viewModel.HienThiGioHang();
            };

            _viewModel.MoThanhToanTriggered += (s, e) =>
            {
                var paymentView = new PaymentView();
                paymentView.ShowDialog();
                _viewModel.HienThiGioHang();
            };

            _viewModel.MoKhachHangTriggered += (s, e) =>
            {
                var customerView = new CustomerView();
                customerView.ShowDialog();
                _viewModel.HienThiGioHang();
            };

            _viewModel.MoBaoCaoTriggered += (s, e) =>
            {
                int maCa = e;
                var reportView = new ShiftReportView(maCa);
                reportView.ShowDialog();
            };

            _viewModel.MoTopMonTriggered += (s, e) =>
            {
                int maCa = e;
                var topMonView = new TopMonView(maCa);
                topMonView.ShowDialog();
            };

            Loaded += (s, e) => _viewModel.KhoiTao();
        }
    }
}
