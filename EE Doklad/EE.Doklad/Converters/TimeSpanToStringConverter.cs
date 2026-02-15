using System;
using System.Globalization;
using System.Windows.Data;

namespace EE.Doklad.Converters
{
    /// <summary>
    /// Converter за TimeSpan към string формат "HH:mm" (двупосочен).
    /// Валидира входа: ако невалиден, връща "00:00".
    /// </summary>
    public class TimeSpanToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeSpan ts)
            {
                // Format as HH:mm
                int hours = (int)ts.TotalHours; // може да е >24 ако е overnight накъсо
                int minutes = ts.Minutes;
                return $"{hours:D2}:{minutes:D2}";
            }
            return "00:00";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str && !string.IsNullOrWhiteSpace(str))
            {
                // Parse HH:mm
                var parts = str.Trim().Split(':');
                if (parts.Length == 2 && int.TryParse(parts[0], out int h) && int.TryParse(parts[1], out int m))
                {
                    if (h >= 0 && h < 24 && m >= 0 && m < 60)
                    {
                        return new TimeSpan(h, m, 0);
                    }
                }
            }
            // Invalid => fallback 00:00
            return TimeSpan.Zero;
        }
    }
}
