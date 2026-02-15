using System;
using System.Globalization;
using System.Windows.Data;
using EE.Doklad.Models;

namespace EE.Doklad.Converters
{
    /// <summary>
    /// Converter за ClimateDatabase enum към дисплей текст.
    /// </summary>
    public class ClimateDatabaseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ClimateDatabase db)
            {
                return db switch
                {
                    ClimateDatabase.BG => "БГ (средни стойности)",
                    ClimateDatabase.ASHRAE => "ASHRAE (международен стандарт)",
                    _ => value.ToString() ?? string.Empty
                };
            }
            return value?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
