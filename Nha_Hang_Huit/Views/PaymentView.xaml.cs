using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Nha_Hang_Huit.Models;
using Nha_Hang_Huit.ViewModels;

namespace Nha_Hang_Huit.Views
{
    /// <summary>
    /// Man hinh thanh toan (MVVM)
    /// </summary>
    public partial class PaymentView : Window
    {
        private readonly PaymentViewModel _viewModel;

        public PaymentView()
        {
            InitializeComponent();

            _viewModel = new PaymentViewModel();
            DataContext = _viewModel;

            _viewModel.DongYeuCau += (s, e) => this.Close();
            Loaded += (s, e) => _viewModel.Load();
        }

        /// <summary>
        /// Khi go phim trong o SDT, cap nhat viewmodel va kich hoat tim kiem
        /// </summary>
        private void txtSoDienThoai_TextChanged(object sender, TextChangedEventArgs e)
        {
            // ViewModel da tu dong xu ly qua PropertyChanged binding
            // Can cap nhat visibility cho listbox goi y
            var vm = DataContext as PaymentViewModel;
            if (vm == null) return;

            lbGoiY.Visibility = vm.CoGoiY ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Chon khach hang tu danh sach goi y
        /// </summary>
        private void lbGoiY_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var listBox = sender as ListBox;
            if (listBox == null) return;

            var item = FindVisualParent<ListBoxItem>(e.OriginalSource as DependencyObject);
            if (item == null) return;

            var kh = item.DataContext as KhachHang;
            if (kh == null) return;

            var vm = DataContext as PaymentViewModel;
            if (vm == null) return;

            vm.ChonKhachHangCommand.Execute(kh);
            lbGoiY.Visibility = Visibility.Collapsed;
            e.Handled = true;
        }

        private static T FindVisualParent<T>(DependencyObject child) where T : DependencyObject
        {
            while (child != null)
            {
                if (child is T parent)
                    return parent;
                child = System.Windows.Media.VisualTreeHelper.GetParent(child);
            }
            return null;
        }

        private void Dong_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
