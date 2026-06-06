using System.Windows;
using Nha_Hang_Huit.ViewModels;

namespace Nha_Hang_Huit.Views
{
    public partial class StaffManagementView : Window
    {
        private readonly StaffManagementViewModel _viewModel;

        public StaffManagementView()
        {
            InitializeComponent();
            _viewModel = new StaffManagementViewModel();
            DataContext = _viewModel;

            _viewModel.DongYeuCau += (s, e) => this.Close();
        }
    }
}
