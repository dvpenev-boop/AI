using System;
using System.Globalization;
using System.Windows.Data;

namespace EE.Doklad.Converters
{
    /// <summary>
    /// Конвертор за RadioButton – проверява дали binding стойността
    /// е равна на ConverterParameter (string с enum member name).
    /// Поддържа TwoWay binding.
    ///
    /// Употреба в XAML:
    ///   IsChecked="{Binding MyEnumProperty,
    ///       Converter={StaticResource EnumEqualityConverter},
    ///       ConverterParameter=MemberName}"
    /// </summary>
    public sealed class EnumEqualityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null || parameter is null) return false;
            return value.ToString() == parameter.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not true || parameter is null) return Binding.DoNothing;

            // targetType е типът на enum-а
            if (targetType.IsEnum)
                return Enum.Parse(targetType, parameter.ToString()!);

            // Ако targetType е nullable enum, получаваме underlying type
            var underlying = Nullable.GetUnderlyingType(targetType);
            if (underlying?.IsEnum == true)
                return Enum.Parse(underlying, parameter.ToString()!);

            return Binding.DoNothing;
        }
    }
}
