using System.Windows;
using System.Windows.Controls;
using Nha_Hang_Huit.Models;
using Nha_Hang_Huit.ViewModels;

namespace Nha_Hang_Huit.Views
{
    /// <summary>
    /// Man hinh goi mon + thanh toan (gop chung)
    /// Mo ra khi click vao ban -> order va thanh toan cung luc
    /// </summary>
    public partial class OrderPaymentView : Window
    {
        private readonly OrderPaymentViewModel _viewModel;

        public OrderPaymentView(BanAn ban = null)
        {
            InitializeComponent();

            _viewModel = new OrderPaymentViewModel(ban);
            DataContext = _viewModel;

            _viewModel.DongYeuCau += (s, e) => this.Close();

            if (ban != null)
                Title = $"Goi mon & Thanh toan - Ban {ban.TenBan} ({ban.TenKhuVuc})";

            // Xu ly su kien cho goi y khach hang (code-behind vi ListBox
            // can tuong tac voi Mouse events)
            lbGoiY.PreviewMouseDown += LbGoiY_PreviewMouseDown;
        }

        /// <summary>
        /// Xu ly click chon khach hang tu goi y
        /// </summary>
        private void LbGoiY_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is ListBox lb && lb.SelectedItem is KhachHang kh)
            {
                _viewModel.ChonKhachHangTuGoiY(kh);
                e.Handled = true;
            }
        }
    }
}
