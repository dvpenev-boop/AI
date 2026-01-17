using System;
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace EE.Doklad.Converters
{
    public class CollectionToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ICollection collection)
            {
                var result = collection.Count > 0 ? Visibility.Visible : Visibility.Collapsed;
                System.Diagnostics.Debug.WriteLine($"CollectionToVisibilityConverter: Count={collection.Count}, Result={result}");
                return result;
            }

            System.Diagnostics.Debug.WriteLine($"CollectionToVisibilityConverter: value is not ICollection, returning Collapsed");
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
