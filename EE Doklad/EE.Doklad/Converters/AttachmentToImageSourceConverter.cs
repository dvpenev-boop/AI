using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using EE.Doklad.Models;

namespace EE.Doklad.Converters
{
    public class AttachmentToImageSourceConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is not AttachmentData attachment)
            {
                return null;
            }

            if (attachment.Bytes == null || attachment.Bytes.Length == 0)
            {
                return null;
            }

            if (attachment.ContentType == "application/pdf")
            {
                return null;
            }

            try
            {
                using var ms = new MemoryStream(attachment.Bytes);
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = ms;
                bitmap.EndInit();
                bitmap.Freeze();
                return bitmap;
            }
            catch
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class AttachmentToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var inverse = parameter?.ToString() == "Inverse";
            var visible = value is AttachmentData attachment
                && attachment.Bytes != null
                && attachment.Bytes.Length > 0
                && attachment.ContentType != "application/pdf";

            if (inverse)
            {
                visible = !visible;
            }

            return visible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
