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

            // Support both property names used in the codebase: Data / Bytes and MimeType / ContentType
            var bytes = attachment.Data;
            var contentType = attachment.MimeType;

            // Try reflection fallback for older naming
            if ((bytes == null || bytes.Length == 0))
            {
                var prop = attachment.GetType().GetProperty("Bytes");
                if (prop != null)
                    bytes = prop.GetValue(attachment) as byte[];
            }
            if (string.IsNullOrEmpty(contentType))
            {
                var prop2 = attachment.GetType().GetProperty("ContentType");
                if (prop2 != null)
                    contentType = prop2.GetValue(attachment) as string;
            }

            if (bytes == null || bytes.Length == 0) return null;
            if (contentType == "application/pdf") return null;

            try
            {
                using var ms = new MemoryStream(bytes);
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
            var visible = false;
            if (value is AttachmentData attachment)
            {
                var bytes = attachment.Data;
                var contentType = attachment.MimeType;
                if ((bytes == null || bytes.Length == 0))
                {
                    var prop = attachment.GetType().GetProperty("Bytes");
                    if (prop != null)
                        bytes = prop.GetValue(attachment) as byte[];
                }
                if (string.IsNullOrEmpty(contentType))
                {
                    var prop2 = attachment.GetType().GetProperty("ContentType");
                    if (prop2 != null)
                        contentType = prop2.GetValue(attachment) as string;
                }
                visible = bytes != null && bytes.Length > 0 && contentType != "application/pdf";
            }

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
