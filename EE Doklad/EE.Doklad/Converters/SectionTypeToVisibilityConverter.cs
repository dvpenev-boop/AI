using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using EE.Doklad.Models;

namespace EE.Doklad.Converters
{
    public class SectionTypeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SectionType sectionType)
            {
                bool inverse = parameter?.ToString() == "Inverse";
                bool isCoverPage = sectionType == SectionType.CoverPage;

                if (inverse)
                {
                    return isCoverPage ? Visibility.Collapsed : Visibility.Visible;
                }
                else
                {
                    return isCoverPage ? Visibility.Visible : Visibility.Collapsed;
                }
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
