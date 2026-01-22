using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace EE.Doklad.Models
{
    public partial class AttachmentData
    {
    public string? AttachmentFileName { get; set; }
        public byte[]? Data { get; set; }
        public string? MimeType { get; set; }

        public BitmapImage? ToBitmapImage()
        {
            if (Data == null) return null;
            var image = new BitmapImage();
            using (var ms = new MemoryStream(Data))
            {
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();
                image.Freeze();
            }
            return image;
        }
    }
}
