using System;
using System.Globalization;
using System.Windows.Data;

namespace EE.Doklad.Converters
{
    public class FlexibleDoubleConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return string.Empty;
            }

            if (value is double doubleValue)
            {
                return doubleValue;
            }

            if (value is float floatValue)
            {
                return (double)floatValue;
            }

            if (value is decimal decimalValue)
            {
                return (double)decimalValue;
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var text = value?.ToString() ?? string.Empty;
            text = text.Trim();

            if (string.IsNullOrEmpty(text))
            {
                return 0d;
            }

            if (text.EndsWith(".", StringComparison.Ordinal) || text.EndsWith(",", StringComparison.Ordinal))
            {
                return Binding.DoNothing;
            }

            var normalized = text.Replace(" ", string.Empty).Replace('.', ',');
            var parsingCulture = (CultureInfo)culture.Clone();
            parsingCulture.NumberFormat.NumberDecimalSeparator = ",";

            if (double.TryParse(normalized, NumberStyles.Float, parsingCulture, out var parsed))
            {
                return parsed;
            }

            if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out parsed))
            {
                return parsed;
            }

            return Binding.DoNothing;
        }
    }
}
