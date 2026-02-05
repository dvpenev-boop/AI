using System;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using EE.Doklad.Models;

namespace EE.Doklad.ViewModels
{
    /// <summary>
    /// ViewModel за секция 19 "Клас на енергопотребление"
    /// Автоматично синхронизира данни от секция 5 (тип сграда) и секция 18 (EP)
    /// </summary>
    public partial class EnergyClassViewModel : ObservableObject
    {
        private Report? _report;
        private ObjectDataSectionData? _objectDataSection;
        private ResultsSectionData? _resultsSection;

        [ObservableProperty]
        private EnergyClassSectionData? _data;

        public EnergyClassViewModel()
        {
        }

        /// <summary>
        /// Инициализира ViewModel с доклад и секция данни
        /// </summary>
        public void Initialize(Report report, EnergyClassSectionData data)
        {
            // Unsubscribe от стари събития
            Cleanup();

            _report = report;
            Data = data;

            // Намираме секция 5 "Данни за обекта" (предпочитаме SectionType, но пазим title-fallback)
            var objectSection = _report.Sections.FirstOrDefault(s =>
                s.Type == SectionType.ObjectData ||
                (s.Title != null && s.Title.Contains("Данни за обекта")));

            if (objectSection?.ObjectDataSectionData != null)
            {
                _objectDataSection = objectSection.ObjectDataSectionData;
                _objectDataSection.PropertyChanged += ObjectDataSection_PropertyChanged;
            }

            // Намираме секция "Резултати сграда" (предпочитаме SectionType, но пазим title-fallback)
            var resultsSection = _report.Sections.FirstOrDefault(s =>
                s.Type == SectionType.Results ||
                (s.Title != null && s.Title.Contains("Резултати сграда")));

            if (resultsSection?.ResultsSectionData != null)
            {
                _resultsSection = resultsSection.ResultsSectionData;
                _resultsSection.PropertyChanged += ResultsSection_PropertyChanged;
            }

            // Първоначално обновяване на данните
            RefreshData();
        }

        /// <summary>
        /// Освобождава ресурси и unsubscribe от събития
        /// </summary>
        public void Cleanup()
        {
            if (_objectDataSection != null)
            {
                _objectDataSection.PropertyChanged -= ObjectDataSection_PropertyChanged;
                _objectDataSection = null;
            }

            if (_resultsSection != null)
            {
                _resultsSection.PropertyChanged -= ResultsSection_PropertyChanged;
                _resultsSection = null;
            }
        }

        /// <summary>
        /// Обработва промени в ObjectDataSectionData (секция 5)
        /// </summary>
        private void ObjectDataSection_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ObjectDataSectionData.BuildingTypeCode))
            {
                RefreshBuildingType();
            }
        }

        /// <summary>
        /// Обработва промени в ResultsSectionData (секция 18)
        /// </summary>
        private void ResultsSection_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ResultsSectionData.TotalSpecificConsumption))
            {
                RefreshEnergyPerformance();
            }
        }

        /// <summary>
        /// Обновява цялата информация (BuildingType + EP)
        /// </summary>
        private void RefreshData()
        {
            RefreshBuildingType();
            RefreshEnergyPerformance();
        }

        /// <summary>
        /// Обновява тип сграда от секция 5
        /// </summary>
        private void RefreshBuildingType()
        {
            if (Data == null)
                return;

            Data.BuildingType = _objectDataSection?.BuildingTypeCode;
        }

        /// <summary>
        /// Обновява EP (годишна специфична енергия) от секция 18
        /// </summary>
        private void RefreshEnergyPerformance()
        {
            if (Data == null)
                return;

            Data.EnergyPerformance = _resultsSection?.TotalSpecificConsumption;
        }
    }
}
