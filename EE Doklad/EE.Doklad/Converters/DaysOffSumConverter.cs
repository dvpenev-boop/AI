using System;
using System.Globalization;
using System.Windows.Data;

namespace EE.Doklad.Converters
{
    /// <summary>
    /// MultiValue converter that sums up monthly days-off string values and returns the total as string.
    /// Accepts null/empty and non-numeric values as zero.
    /// </summary>
    public class DaysOffSumConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int SumParse(object? o)
            {
                if (o == null) return 0;
                var s = o as string;
                if (string.IsNullOrWhiteSpace(s)) return 0;
                if (int.TryParse(s.Trim(), out var v)) return v;
                return 0;
            }

            int sum = 0;
            foreach (var v in values)
            {
                sum += SumParse(v);
            }

            // Return as string so TextBlock displays it directly
            return sum.ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
