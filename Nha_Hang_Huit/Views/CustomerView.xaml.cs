using System.Windows;
using System.Windows.Input;
using Nha_Hang_Huit.ViewModels;

namespace Nha_Hang_Huit.Views
{
    /// <summary>
    /// Man hinh quan ly khach hang (MVVM)
    /// </summary>
    public partial class CustomerView : Window
    {
        private readonly CustomerViewModel _viewModel;

        public CustomerView()
        {
            InitializeComponent();

            _viewModel = new CustomerViewModel();
            DataContext = _viewModel;

            _viewModel.DongYeuCau += (s, e) => this.Close();
            Loaded += (s, e) => _viewModel.Load();
        }

        private void Dong_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
