using System;
using System.Globalization;
using System.Windows.Data;
using EE.Doklad.Models;
using EE.Doklad.Services;

namespace EE.Doklad.Converters
{
    /// <summary>
    /// Конвертира Orientation enum към текстов етикет (И, СИ, С, ...)
    /// </summary>
    public class OrientationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Orientation orientation)
            {
                return WindowCalculator.GetOrientationLabel(orientation);
            }
            return value?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
