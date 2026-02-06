using System;
using System.Globalization;
using System.Windows.Data;

namespace EE.Doklad.Converters
{
    // Extracts the 'Начало' day (numeric) from a HeatingSeasonInfo string like "Начало: 21 октомври; Край: 20 април"
    public class HeatingSeasonStartDayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var s = value as string;
            if (string.IsNullOrWhiteSpace(s)) return string.Empty;
            var parts = s.Split(';');
            if (parts.Length == 0) return string.Empty;
            var startPart = parts[0]; // expected "Начало: 21 октомври"
            var idx = startPart.IndexOf(':');
            if (idx >= 0 && idx + 1 < startPart.Length)
            {
                var rest = startPart.Substring(idx + 1).Trim();
                var tokens = rest.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length >= 1) return tokens[0];
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Extracts the 'Начало' month name from HeatingSeasonInfo
    public class HeatingSeasonStartMonthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var s = value as string;
            if (string.IsNullOrWhiteSpace(s)) return string.Empty;
            var parts = s.Split(';');
            if (parts.Length == 0) return string.Empty;
            var startPart = parts[0];
            var idx = startPart.IndexOf(':');
            if (idx >= 0 && idx + 1 < startPart.Length)
            {
                var rest = startPart.Substring(idx + 1).Trim();
                var tokens = rest.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length >= 2)
                {
                    // month is the remainder after the first token
                    return string.Join(' ', tokens, 1, tokens.Length - 1);
                }
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Extracts the 'Край' day (numeric) from HeatingSeasonInfo
    public class HeatingSeasonEndDayConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var s = value as string;
            if (string.IsNullOrWhiteSpace(s)) return string.Empty;
            var parts = s.Split(';');
            if (parts.Length < 2) return string.Empty;
            var endPart = parts[1]; // expected " Край: 20 април"
            var idx = endPart.IndexOf(':');
            if (idx >= 0 && idx + 1 < endPart.Length)
            {
                var rest = endPart.Substring(idx + 1).Trim();
                var tokens = rest.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length >= 1) return tokens[0];
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Extracts the 'Край' month name from HeatingSeasonInfo
    public class HeatingSeasonEndMonthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var s = value as string;
            if (string.IsNullOrWhiteSpace(s)) return string.Empty;
            var parts = s.Split(';');
            if (parts.Length < 2) return string.Empty;
            var endPart = parts[1];
            var idx = endPart.IndexOf(':');
            if (idx >= 0 && idx + 1 < endPart.Length)
            {
                var rest = endPart.Substring(idx + 1).Trim();
                var tokens = rest.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length >= 2)
                {
                    return string.Join(' ', tokens, 1, tokens.Length - 1);
                }
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
