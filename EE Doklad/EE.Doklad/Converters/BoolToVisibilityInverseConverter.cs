using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace EE.Doklad.Converters
{
    /// <summary>
    /// Converter който преобразува bool в Visibility (inverse: true = Collapsed, false = Visible)
    /// </summary>
    public class BoolToVisibilityInverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? Visibility.Collapsed : Visibility.Visible;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility != Visibility.Visible;
            }
            return false;
        }
    }
}
