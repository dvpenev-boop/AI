using System;
using System.Globalization;
using System.Windows.Data;

namespace EE.Doklad.Converters
{
    /// <summary>
    /// Конвертира ShadingTypeId към текстов етикет
    /// </summary>
    public class ShadingTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string shadingTypeId && !string.IsNullOrEmpty(shadingTypeId))
            {
                // Simplified display - in production, lookup from catalog
                if (shadingTypeId.Contains("whiteBlind")) return "Бели венециански";
                if (shadingTypeId.Contains("whiteCurtain")) return "Бели завеси";
                if (shadingTypeId.Contains("coloredTextile")) return "Цветен текстил";
                if (shadingTypeId.Contains("aluminumTextile")) return "Ал. покритие";
                return "Щора";
            }
            return "Без";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Конвертира ObstacleProfileId към текстов етикет
    /// </summary>
    public class ObstacleProfileConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string profileId && !string.IsNullOrEmpty(profileId))
            {
                return profileId switch
                {
                    "none" => "Без",
                    "balcony" => "Балкон",
                    "adjacentBuilding" => "Съседна сграда",
                    "trees" => "Дървета",
                    "custom" => "Custom",
                    _ => profileId
                };
            }
            return "Без";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
