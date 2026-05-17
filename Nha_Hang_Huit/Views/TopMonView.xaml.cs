using System.Windows;
using Nha_Hang_Huit.ViewModels;

namespace Nha_Hang_Huit.Views
{
    /// <summary>
    /// View doc lap cho admin xem thong ke top mon ban chay
    /// </summary>
    public partial class TopMonView : Window
    {
        private readonly TopMonViewModel _viewModel;

        public TopMonView(int maCa)
        {
            InitializeComponent();
            _viewModel = new TopMonViewModel(maCa);
            DataContext = _viewModel;

            _viewModel.DongYeuCau += (s, e) => this.Close();

            Loaded += (s, e) => _viewModel.Load();
        }
    }
}
