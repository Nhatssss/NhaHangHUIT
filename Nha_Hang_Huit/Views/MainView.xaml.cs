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
            _viewModel.MoKhachHangTriggered += (s, e) =>
            {
                var customerView = new CustomerView();
                customerView.ShowDialog();
                _viewModel.HienThiGioHang();
            };

            _viewModel.MoBaoCaoTriggered += (s, e) =>
            {
                int maCa = e;
                MessageBox.Show($"Da dong ca #{maCa} thanh cong!", "Ket Ca", MessageBoxButton.OK, MessageBoxImage.Information);
            };

            _viewModel.MoTopMonTriggered += (s, e) =>
            {
                int maCa = e;
                var topMonView = new TopMonView(maCa);
                topMonView.ShowDialog();
            };

            // Table event: mo combined order+payment cho 1 ban
            _viewModel.MoBanOrderPaymentTriggered += (s, ban) =>
            {
                if (ban == null) return;
                var combinedView = new OrderPaymentView(ban);
                combinedView.ShowDialog();
                _viewModel.HienThiGioHang();
                _viewModel.KhoiTao();
            };

            // Bao cao tong hop danh cho quan ly (khong can mo ca)
            _viewModel.MoBaoCaoAllTriggered += (s, e) =>
            {
                MessageBox.Show("Tinh nang bao cao tong hop dang phat trien.", "Thong bao", MessageBoxButton.OK, MessageBoxImage.Information);
            };

            _viewModel.MoTopMonAllTriggered += (s, e) =>
            {
                MessageBox.Show("Tinh nang top mon tong hop dang phat trien.", "Thong bao", MessageBoxButton.OK, MessageBoxImage.Information);
            };

            // Quan ly nhan vien
            _viewModel.MoStaffManagementTriggered += (s, e) =>
            {
                var staffView = new StaffManagementView();
                staffView.ShowDialog();
            };

            // Lich su hoa don
            _viewModel.MoInvoiceHistoryTriggered += (s, e) =>
            {
                var invoiceView = new InvoiceHistoryView();
                invoiceView.ShowDialog();
            };

            // Phieu nhap hang
            _viewModel.MoPhieuNhapTriggered += (s, e) =>
            {
                var phieuNhapView = new PhieuNhapView();
                phieuNhapView.ShowDialog();
            };

            // Dang xuat: quay ve man hinh dang nhap
            _viewModel.DangXuatTriggered += (s, e) =>
            {
                var result = MessageBox.Show("Ban co chac muon dang xuat?", "Xac nhan",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes) return;

                var loginView = new LoginView();
                loginView.Show();
                this.Close();
            };

            Loaded += (s, e) => _viewModel.KhoiTao();
        }

    }
}
