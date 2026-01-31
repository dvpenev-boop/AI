using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace EE.Doklad.Converters
{
    /// <summary>
    /// Converter който показва елемент само ако стойността съвпада с параметъра
    /// </summary>
    public class StringMatchToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return Visibility.Collapsed;

            string valueStr = value.ToString() ?? "";
            string paramStr = parameter.ToString() ?? "";

            return valueStr.Equals(paramStr, StringComparison.OrdinalIgnoreCase) 
                ? Visibility.Visible 
                : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
