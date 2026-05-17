using System.Windows;
using Nha_Hang_Huit.ViewModels;

namespace Nha_Hang_Huit.Views
{
    /// <summary>
    /// Man hinh goi mon (MVVM)
    /// </summary>
    public partial class OrderView : Window
    {
        public OrderView()
        {
            InitializeComponent();
            var vm = new OrderViewModel();
            DataContext = vm;
            vm.DongYeuCau += (s, e) => this.Close();
        }
    }
}
