using System;
using System.ComponentModel;
using System.Globalization;
using EE.Doklad.Models;
using EE.Doklad.Services;

namespace EE.Doklad.ViewModels
{
    /// <summary>
    /// ViewModel за секция "12. Вентилация"
    /// Управлява входните данни и изчисленията по Наредба RD-02-20-3
    /// </summary>
    public class VentilationSectionViewModel : INotifyPropertyChanged
    {
        private readonly VentilationSectionData _data;
        private readonly ObjectDataSectionData? _objectData;
        private readonly ClimateZoneData? _climateData;
        private readonly BgVentilationCalculator _calculator;

        public event PropertyChangedEventHandler? PropertyChanged;

        // Calculation result
        private VentilationCalculationResult? _calculationResult;

        public VentilationSectionViewModel(
            VentilationSectionData data,
            ObjectDataSectionData? objectData = null,
            ClimateZoneData? climateData = null)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));
            _objectData = objectData;
            _climateData = climateData;
            _calculator = new BgVentilationCalculator();

            // Subscribe to data changes
            _data.PropertyChanged += OnDataPropertyChanged;

            // Initialize
            Recalculate();
        }

        // ========== PROPERTIES ==========

        public string Description
        {
            get => _data.Description ?? string.Empty;
            set
            {
                if (_data.Description != value)
                {
                    _data.Description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        // ========== INPUT PROPERTIES ==========

        public double OperatingHoursPerWeek
        {
            get => _data.OperatingHoursPerWeek;
            set
            {
                if (_data.OperatingHoursPerWeek != value)
                {
                    var clamped = Math.Clamp(value, 0, 168);
                    _data.OperatingHoursPerWeek = clamped;
                    OnPropertyChanged(nameof(OperatingHoursPerWeek));
                    Recalculate();
                }
            }
        }

        public double AirflowRatePerM2
        {
            get => _data.AirflowRatePerM2;
            set
            {
                if (_data.AirflowRatePerM2 != value)
                {
                    var clamped = Math.Max(value, 0);
                    _data.AirflowRatePerM2 = clamped;
                    OnPropertyChanged(nameof(AirflowRatePerM2));
                    Recalculate();
                }
            }
        }

        public double SupplyTemperature
        {
            get => _data.SupplyTemperature;
            set
            {
                if (_data.SupplyTemperature != value)
                {
                    _data.SupplyTemperature = value;
                    OnPropertyChanged(nameof(SupplyTemperature));
                    Recalculate();
                }
            }
        }

        public double RelativeHumidity
        {
            get => _data.RelativeHumidity;
            set
            {
                if (_data.RelativeHumidity != value)
                {
                    var clamped = Math.Clamp(value, 0, 100);
                    _data.RelativeHumidity = clamped;
                    OnPropertyChanged(nameof(RelativeHumidity));
                    Recalculate();
                }
            }
        }

        public double FirstStageRecuperationEfficiency
        {
            get => _data.FirstStageRecuperationEfficiency;
            set
            {
                if (_data.FirstStageRecuperationEfficiency != value)
                {
                    var clamped = Math.Clamp(value, 0, 100);
                    _data.FirstStageRecuperationEfficiency = clamped;
                    OnPropertyChanged(nameof(FirstStageRecuperationEfficiency));
                    Recalculate();
                }
            }
        }

        public double SecondStageRecuperationEfficiency
        {
            get => _data.SecondStageRecuperationEfficiency;
            set
            {
                if (_data.SecondStageRecuperationEfficiency != value)
                {
                    var clamped = Math.Clamp(value, 0, 100);
                    _data.SecondStageRecuperationEfficiency = clamped;
                    OnPropertyChanged(nameof(SecondStageRecuperationEfficiency));
                    Recalculate();
                }
            }
        }

        public double MaxTemperatureDifferenceSecondStage
        {
            get => _data.MaxTemperatureDifferenceSecondStage;
            set
            {
                if (_data.MaxTemperatureDifferenceSecondStage != value)
                {
                    var clamped = Math.Clamp(value, 4, 8);
                    _data.MaxTemperatureDifferenceSecondStage = clamped;
                    OnPropertyChanged(nameof(MaxTemperatureDifferenceSecondStage));
                    Recalculate();
                }
            }
        }

        public double MinExhaustAirTemperature
        {
            get => _data.MinExhaustAirTemperature;
            set
            {
                if (_data.MinExhaustAirTemperature != value)
                {
                    var clamped = Math.Clamp(value, 3, 5);
                    _data.MinExhaustAirTemperature = clamped;
                    OnPropertyChanged(nameof(MinExhaustAirTemperature));
                    Recalculate();
                }
            }
        }

        // ========== ENERGY SOURCE 1 PROPERTIES ==========

        public EnergySourceType EnergySource1Type
        {
            get => _data.EnergySource1.Type;
            set
            {
                if (_data.EnergySource1.Type != value)
                {
                    _data.EnergySource1.Type = value;
                    OnPropertyChanged(nameof(EnergySource1Type));
                    Recalculate();
                }
            }
        }

        public double EnergySource1Share
        {
            get => _data.EnergySource1.Share;
            set
            {
                if (_data.EnergySource1.Share != value)
                {
                    var clamped = Math.Clamp(value, 0, 100);
                    _data.EnergySource1.Share = clamped;
                    OnPropertyChanged(nameof(EnergySource1Share));
                    Recalculate();
                }
            }
        }

        public double EnergySource1EmissionEfficiency
        {
            get => _data.EnergySource1.EmissionEfficiency;
            set
            {
                if (_data.EnergySource1.EmissionEfficiency != value)
                {
                    var clamped = Math.Clamp(value, 0, 100);
                    _data.EnergySource1.EmissionEfficiency = clamped;
                    OnPropertyChanged(nameof(EnergySource1EmissionEfficiency));
                    Recalculate();
                }
            }
        }

        public double EnergySource1DistributionEfficiency
        {
            get => _data.EnergySource1.DistributionEfficiency;
            set
            {
                if (_data.EnergySource1.DistributionEfficiency != value)
                {
                    var clamped = Math.Clamp(value, 0, 100);
                    _data.EnergySource1.DistributionEfficiency = clamped;
                    OnPropertyChanged(nameof(EnergySource1DistributionEfficiency));
                    Recalculate();
                }
            }
        }

        public double EnergySource1AutomaticControl
        {
            get => _data.EnergySource1.AutomaticControl;
            set
            {
                if (_data.EnergySource1.AutomaticControl != value)
                {
                    var clamped = Math.Clamp(value, 0, 100);
                    _data.EnergySource1.AutomaticControl = clamped;
                    OnPropertyChanged(nameof(EnergySource1AutomaticControl));
                    Recalculate();
                }
            }
        }

        public double EnergySource1EnergyManagement
        {
            get => _data.EnergySource1.EnergyManagement;
            set
            {
                if (_data.EnergySource1.EnergyManagement != value)
                {
                    var clamped = Math.Clamp(value, 0, 100);
                    _data.EnergySource1.EnergyManagement = clamped;
                    OnPropertyChanged(nameof(EnergySource1EnergyManagement));
                    Recalculate();
                }
            }
        }

        public double EnergySource1GenerationEfficiency
        {
            get => _data.EnergySource1.GenerationEfficiency;
            set
            {
                if (_data.EnergySource1.GenerationEfficiency != value)
                {
                    var clamped = Math.Max(value, 0); // Can be > 100 for heat pumps
                    _data.EnergySource1.GenerationEfficiency = clamped;
                    OnPropertyChanged(nameof(EnergySource1GenerationEfficiency));
                    Recalculate();
                }
            }
        }

        // ========== ENERGY SOURCE 2 (OPTIONAL) ==========

        public bool UseSecondEnergySource
        {
            get => _data.UseSecondEnergySource;
            set
            {
                if (_data.UseSecondEnergySource != value)
                {
                    _data.UseSecondEnergySource = value;
                    if (value && _data.EnergySource2 == null)
                    {
                        _data.EnergySource2 = new VentilationEnergySource
                        {
                            Type = EnergySourceType.Electricity,
                            Share = 0
                        };
                    }
                    OnPropertyChanged(nameof(UseSecondEnergySource));
                    Recalculate();
                }
            }
        }

        public EnergySourceType EnergySource2Type
        {
            get => _data.EnergySource2?.Type ?? EnergySourceType.Electricity;
            set
            {
                if (_data.EnergySource2 != null && _data.EnergySource2.Type != value)
                {
                    _data.EnergySource2.Type = value;
                    OnPropertyChanged(nameof(EnergySource2Type));
                    Recalculate();
                }
            }
        }

        public double EnergySource2Share
        {
            get => _data.EnergySource2?.Share ?? 0;
            set
            {
                if (_data.EnergySource2 != null && _data.EnergySource2.Share != value)
                {
                    var clamped = Math.Clamp(value, 0, 100);
                    _data.EnergySource2.Share = clamped;
                    OnPropertyChanged(nameof(EnergySource2Share));
                    Recalculate();
                }
            }
        }

        public double EnergySource2GenerationEfficiency
        {
            get => _data.EnergySource2?.GenerationEfficiency ?? 100;
            set
            {
                if (_data.EnergySource2 != null && _data.EnergySource2.GenerationEfficiency != value)
                {
                    var clamped = Math.Max(value, 0);
                    _data.EnergySource2.GenerationEfficiency = clamped;
                    OnPropertyChanged(nameof(EnergySource2GenerationEfficiency));
                    Recalculate();
                }
            }
        }

        // ========== READ-ONLY PROPERTIES ==========

        public double HeatedArea_m2
        {
            get
            {
                if (_objectData?.HeatedArea != null &&
                    double.TryParse(_objectData.HeatedArea, NumberStyles.Float, CultureInfo.InvariantCulture, out double area))
                {
                    return area;
                }
                return 0;
            }
        }

        public double IndoorTemperature_C => _data.IndoorTemperature_C;

        // ========== CALCULATED OUTPUT PROPERTIES ==========

        public double VentilationLossCoefficient_WK =>
            _calculationResult?.VentilationLossCoefficient_WK ?? 0;

        public double AnnualVentilationHeatingEnergy_kWh_a =>
            _calculationResult?.AnnualVentilationHeatingEnergy_kWh_a ?? 0;

        public double SpecificVentilationHeatingEnergy_kWh_m2a =>
            _calculationResult?.SpecificVentilationHeatingEnergy_kWh_m2a ?? 0;

        public double TotalFinalEnergy_kWh_a =>
            _calculationResult?.TotalFinalEnergy_kWh_a ?? 0;

        public double SpecificFinalEnergy_kWh_m2a =>
            _calculationResult?.SpecificFinalEnergy_kWh_m2a ?? 0;

        public string ErrorMessage =>
            _calculationResult?.IsValid == false ? _calculationResult.ErrorMessage ?? string.Empty : string.Empty;

        // ========== CALCULATION ==========

        private void Recalculate()
        {
            // Update heated area in data model
            _data.HeatedArea_m2 = HeatedArea_m2;

            // Perform calculation
            _calculationResult = _calculator.Calculate(_data, _climateData);

            // Notify all output properties
            OnPropertyChanged(nameof(VentilationLossCoefficient_WK));
            OnPropertyChanged(nameof(AnnualVentilationHeatingEnergy_kWh_a));
            OnPropertyChanged(nameof(SpecificVentilationHeatingEnergy_kWh_m2a));
            OnPropertyChanged(nameof(TotalFinalEnergy_kWh_a));
            OnPropertyChanged(nameof(SpecificFinalEnergy_kWh_m2a));
            OnPropertyChanged(nameof(ErrorMessage));
        }

        private void OnDataPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // Recalculate when data changes
            Recalculate();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
