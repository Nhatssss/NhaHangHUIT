using System.Windows;
using Nha_Hang_Huit.ViewModels;

namespace Nha_Hang_Huit.Views
{
    public partial class InvoiceHistoryView : Window
    {
        private readonly InvoiceHistoryViewModel _viewModel;

        public InvoiceHistoryView()
        {
            InitializeComponent();
            _viewModel = new InvoiceHistoryViewModel();
            DataContext = _viewModel;

            _viewModel.DongYeuCau += (s, e) => this.Close();
        }
    }
}
