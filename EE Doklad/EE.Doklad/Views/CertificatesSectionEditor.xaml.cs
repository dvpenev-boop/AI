using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using EE.Doklad.Models;
using Microsoft.Win32;

namespace EE.Doklad.Views
{
    public partial class CertificatesSectionEditor : UserControl
    {
        public CertificatesSectionEditor()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext is CertificatesSectionData data)
            {
                if (data.CertificateAttachment != null)
                {
                    data.CertificateAttachment.PropertyChanged += (s, args) =>
                    {
                        if (args.PropertyName == nameof(AttachmentData.Bytes))
                            UpdateCertificatePreview();
                    };
                }

                if (data.InsuranceAttachment != null)
                {
                    data.InsuranceAttachment.PropertyChanged += (s, args) =>
                    {
                        if (args.PropertyName == nameof(AttachmentData.Bytes))
                            UpdateInsurancePreview();
                    };
                }

                UpdateCertificatePreview();
                UpdateInsurancePreview();
            }
        }

        private void UploadCertificate_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not CertificatesSectionData data) return;

            var filePath = SelectFile();
            if (filePath != null)
            {
                data.CertificateAttachment ??= new AttachmentData();
                LoadFile(data.CertificateAttachment, filePath);
                UpdateCertificatePreview();
            }
        }

        private void RemoveCertificate_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is CertificatesSectionData data && data.CertificateAttachment != null)
            {
                data.CertificateAttachment.FileName = null;
                data.CertificateAttachment.Bytes = null;
                data.CertificateAttachment.SourcePageCount = 0;
                UpdateCertificatePreview();
            }
        }

        private void UploadInsurance_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not CertificatesSectionData data) return;

            var filePath = SelectFile();
            if (filePath != null)
            {
                data.InsuranceAttachment ??= new AttachmentData();
                LoadFile(data.InsuranceAttachment, filePath);
                UpdateInsurancePreview();
            }
        }

        private void RemoveInsurance_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is CertificatesSectionData data && data.InsuranceAttachment != null)
            {
                data.InsuranceAttachment.FileName = null;
                data.InsuranceAttachment.Bytes = null;
                data.InsuranceAttachment.SourcePageCount = 0;
                UpdateInsurancePreview();
            }
        }

        private string? SelectFile()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Документи|*.pdf;*.png;*.jpg;*.jpeg|PDF|*.pdf|Изображения|*.png;*.jpg;*.jpeg",
                Title = "Изберете файл"
            };

            return dialog.ShowDialog() == true ? dialog.FileName : null;
        }

        private void LoadFile(AttachmentData attachment, string filePath)
        {
            try
            {
                attachment.FileName = Path.GetFileName(filePath);
                attachment.Bytes = File.ReadAllBytes(filePath);

                var ext = Path.GetExtension(filePath).ToLowerInvariant();
                attachment.ContentType = ext switch
                {
                    ".pdf" => "application/pdf",
                    ".png" => "image/png",
                    ".jpg" or ".jpeg" => "image/jpeg",
                    _ => "application/octet-stream"
                };

                // За PDF - поставяме 1 (реално извличане на брой страници е TODO)
                attachment.SourcePageCount = ext == ".pdf" ? 1 : 1;

                MessageBox.Show($"Файлът \"{attachment.FileName}\" е качен успешно!",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Грешка при зареждане: {ex.Message}",
                    "Грешка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateCertificatePreview()
        {
            var data = DataContext as CertificatesSectionData;
            UpdatePreview(CertificateImage, CertificatePlaceholder, data?.CertificateAttachment);
        }

        private void UpdateInsurancePreview()
        {
            var data = DataContext as CertificatesSectionData;
            UpdatePreview(InsuranceImage, InsurancePlaceholder, data?.InsuranceAttachment);
        }

        private void UpdatePreview(Image imageControl, UIElement placeholder, AttachmentData? attachment)
        {
            if (attachment?.Bytes == null || attachment.Bytes.Length == 0)
            {
                imageControl.Source = null;
                imageControl.Visibility = Visibility.Collapsed;
                placeholder.Visibility = Visibility.Visible;
                return;
            }

            // За PDF - показваме placeholder (TODO: рендериране на първа страница)
            if (attachment.ContentType == "application/pdf")
            {
                imageControl.Source = null;
                imageControl.Visibility = Visibility.Collapsed;
                placeholder.Visibility = Visibility.Visible;
                return;
            }

            // За изображения - показваме директно
            try
            {
                using var ms = new MemoryStream(attachment.Bytes);
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = ms;
                bitmap.EndInit();
                bitmap.Freeze();

                imageControl.Source = bitmap;
                imageControl.Visibility = Visibility.Visible;
                placeholder.Visibility = Visibility.Collapsed;
            }
            catch
            {
                imageControl.Source = null;
                imageControl.Visibility = Visibility.Collapsed;
                placeholder.Visibility = Visibility.Visible;
            }
        }
    }
}
