using System.Windows;
using Nha_Hang_Huit.Models;
using Nha_Hang_Huit.ViewModels;

namespace Nha_Hang_Huit.Views
{
    /// <summary>
    /// Man hinh goi mon (MVVM)
    /// </summary>
    public partial class OrderView : Window
    {
        public OrderView() : this(null)
        {
        }

        public OrderView(BanAn ban = null)
        {
            InitializeComponent();
            var vm = new OrderViewModel(ban);
            DataContext = vm;
            vm.DongYeuCau += (s, e) => this.Close();

            if (ban != null)
                Title = $"Gọi Món - Bàn {ban.TenBan} ({ban.TenKhuVuc})";
        }
    }
}
