using System;
using System.Windows;
using System.Windows.Data;

namespace Nha_Hang_Huit.Converters
{
    /// <summary>
    /// Boolean -> Visibility converter cho WPF binding.
    /// true = Visible, false = Collapsed.
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool b = value is bool bv && bv;
            return b ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value is Visibility v && v == Visibility.Visible;
        }
    }
}
