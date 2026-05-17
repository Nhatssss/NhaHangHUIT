using System.Windows;
using Nha_Hang_Huit.ViewModels;

namespace Nha_Hang_Huit.Views
{
    /// <summary>
    /// Man hinh bao cao dong ca (MVVM)
    /// </summary>
    public partial class ShiftReportView : Window
    {
        public ShiftReportView(int maCa)
        {
            InitializeComponent();

            var vm = new ShiftReportViewModel(maCa);
            DataContext = vm;

            vm.DongYeuCau += (s, e) => this.Close();
            Loaded += (s, e) => vm.Load();
        }

        private void Dong_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
