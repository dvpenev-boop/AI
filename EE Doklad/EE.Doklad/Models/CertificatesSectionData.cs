using CommunityToolkit.Mvvm.ComponentModel;

namespace EE.Doklad.Models
{
    /// <summary>
    /// Данни за прикачен файл (PDF или изображение)
    /// </summary>
    public partial class AttachmentData : ObservableObject
    {
        [ObservableProperty]
        private string? _fileName;

        [ObservableProperty]
        private string? _contentType;

        [ObservableProperty]
        private byte[]? _bytes;

        [ObservableProperty]
        private int _sourcePageCount;

        /// <summary>
        /// Има ли прикачен файл
        /// </summary>
        public bool HasAttachment => Bytes != null && Bytes.Length > 0;

        /// <summary>
        /// Предупреждение ако PDF има повече от една страница
        /// </summary>
        public string? MultiPageWarning => SourcePageCount > 1
            ? $"PDF има {SourcePageCount} страници. Ще се използва само първата страница."
            : null;
    }

    /// <summary>
    /// Данни за секция "Удостоверения"
    /// </summary>
    public partial class CertificatesSectionData : ObservableObject
    {
        [ObservableProperty]
        private string _title = "Удостоверения";

        [ObservableProperty]
        private AttachmentData? _certificateAttachment;

        [ObservableProperty]
        private AttachmentData? _insuranceAttachment;
    }
}
