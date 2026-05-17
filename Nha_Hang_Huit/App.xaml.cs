using System;
using System.Windows;

namespace Nha_Hang_Huit
{
    /// <summary>
    /// Lop App chinh: khoi tao ung dung va xu ly loi khong mong muon
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Xu ly loi khong bat duoc o thread UI
            this.DispatcherUnhandledException += (s, args) =>
            {
                MessageBox.Show("Loi ung dung: " + args.Exception.Message,
                    "Loi khong mong muon", MessageBoxButton.OK, MessageBoxImage.Error);
                args.Handled = true;
            };
        }
    }
}
