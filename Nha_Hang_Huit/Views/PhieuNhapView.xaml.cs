using System.Windows;
using Nha_Hang_Huit.ViewModels;

namespace Nha_Hang_Huit.Views
{
    /// <summary>
    /// Man hinh phieu nhap hang
    /// </summary>
    public partial class PhieuNhapView : Window
    {
        public PhieuNhapView()
        {
            InitializeComponent();

            var vm = new PhieuNhapViewModel();
            DataContext = vm;

            vm.DongYeuCau += (s, e) => this.Close();
            vm.InPhieuYeuCau += (s, phieuNhap) =>
            {
                var report = new PhieuNhapReportView(phieuNhap);
                report.ShowDialog();
            };
            Loaded += (s, e) => vm.LoadData();
        }
    }
}
