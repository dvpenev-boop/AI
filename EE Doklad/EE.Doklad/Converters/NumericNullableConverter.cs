using System;
using System.Globalization;
using System.Windows.Data;

namespace EE.Doklad.Converters
{
    // Converts between string and double? allowing both comma and dot as decimal separator.
    public class NumericNullableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;
            if (value is double d)
            {
                // format with up to 3 decimal places (trim trailing zeros)
                return d.ToString("0.###", culture);
            }

            return value?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var sRaw = value as string ?? value?.ToString() ?? string.Empty;
            var s = sRaw.Trim();
            if (string.IsNullOrEmpty(s)) return null!;

            // Accept both comma and dot as decimal separator.
            s = s.Replace(',', '.');
            if (double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var d))
            {
                // normalize to max 3 decimal places
                var rounded = Math.Round(d, 3);
                return rounded;
            }

            return null!;
        }
    }
}
