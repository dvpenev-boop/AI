using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Media;
using EE.Doklad.Models;
using EE.Doklad.Sections.Section11Heating.Models;
using EE.Doklad.Sections.Section11Heating.Services;
using EE.Doklad.Sections.Section23InternalGains.Services;
using EE.Doklad.Sections.Section24SolarGains.Models;
using EE.Doklad.Services;

namespace EE.Doklad.ViewModels
{
    /// <summary>
    /// ViewModel пїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅ "10. пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ"
    /// пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅ, пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ
    /// пїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅ
    /// </summary>
    public class HeatingSectionViewModel : INotifyPropertyChanged
    {
        private readonly HeatingSectionData _data;
        private readonly ObjectDataSectionData? _objectData;
        private readonly Report? _report;
        private readonly ExternalWallsSectionData? _wallsData;
        private readonly RoofSectionData? _roofData;
        private readonly FloorSectionData? _floorData;
        private readonly WindowsSectionData? _windowsData;
        private readonly UnconditionedZoneSectionData? _ztuData;
        private readonly VentilationSectionData? _ventilationHeatingData;
        private readonly InternalGainsDebugInput? _section23Data;
        private readonly Section24SolarGainsData? _section24Data;
        private readonly HeatingCalculationService _calcService = new();
        private readonly InternalGainsService? _internalGainsService;
        private bool _isAdjustingShares = false; // guard to avoid recursive share updates
        private double _heatingInstallationHours;
        private HeatingAnnualResult? _lastAnnual;
        private List<HeatingMonthlyResult> _lastMonthly = new();

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// пїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅ пїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅ (пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ gating пїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ)
        /// </summary>
        public bool IsHeatingSeasonEnabled => _objectData?.HeatingSeasonEnabled ?? true;

        /// <summary>
        /// пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅ, пїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅ пїЅпїЅ пїЅ пїЅпїЅпїЅпїЅпїЅпїЅ
        /// </summary>
        public string HeatingSeasonWarning => IsHeatingSeasonEnabled
            ? string.Empty
            : "\u041D\u0435 \u0435 \u0438\u0437\u0431\u0440\u0430\u043D \u043E\u0442\u043E\u043F\u043B\u0438\u0442\u0435\u043B\u0435\u043D \u0441\u0435\u0437\u043E\u043D.";

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

        public bool IsMethodAuer
        {
            get => _data.CalculationMethod == HeatingCalculationMethod.AuerSoftware;
            set
            {
                if (value)
                {
                    SetMethod(HeatingCalculationMethod.AuerSoftware);
                }
            }
        }

        public bool IsMethodRd
        {
            get => _data.CalculationMethod == HeatingCalculationMethod.Rd0220_3;
            set
            {
                if (value)
                {
                    SetMethod(HeatingCalculationMethod.Rd0220_3);
                }
            }
        }

        public bool IsMethodAshrae
        {
            get => _data.CalculationMethod == HeatingCalculationMethod.Ashrae8760;
            set
            {
                if (value)
                {
                    SetMethod(HeatingCalculationMethod.Ashrae8760);
                }
            }
        }

        public string MethodStatusText => _data.CalculationMethod switch
        {
            HeatingCalculationMethod.AuerSoftware =>
                "\u041C\u0435\u0442\u043E\u0434 1: \u0410\u0423\u0415\u0420 - \u0441\u0440\u0430\u0432\u043D\u0438\u0442\u0435\u043B\u0435\u043D \u0440\u0435\u0436\u0438\u043C \u0441\u043F\u0440\u044F\u043C\u043E \u0440\u0435\u0444\u0435\u0440\u0435\u043D\u0442\u043D\u0438\u044F \u0441\u043E\u0444\u0442\u0443\u0435\u0440.",
            HeatingCalculationMethod.Rd0220_3 =>
                "\u041C\u0435\u0442\u043E\u0434 2: \u0420\u0414-02-20-3 / \u0411\u0414\u0421 EN ISO 52016-1.",
            HeatingCalculationMethod.Ashrae8760 =>
                "\u041C\u0435\u0442\u043E\u0434 3: ASHRAE 8760 - \u0432 \u0440\u0430\u0437\u0440\u0430\u0431\u043E\u0442\u043A\u0430, \u0440\u0435\u0437\u0443\u043B\u0442\u0430\u0442\u0438\u0442\u0435 \u043D\u0435 \u0441\u0430 \u043D\u0430\u043B\u0438\u0447\u043D\u0438.",
            _ => string.Empty
        };

        public Brush MethodStatusBrush => _data.CalculationMethod == HeatingCalculationMethod.Ashrae8760
            ? Brushes.DarkOrange
            : Brushes.DarkGreen;

        // ========== пїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅ ==========

        private string _infiltrationText;
        public string InfiltrationText
        {
            get => _infiltrationText;
            set
            {
                if (_infiltrationText != value)
                {
                    _infiltrationText = value;
                    OnPropertyChanged(nameof(InfiltrationText));
                    ValidateAndSetInfiltration(value);
                }
            }
        }

        private string _designTemperatureText;
        public string DesignTemperatureText
        {
            get => _designTemperatureText;
            set
            {
                if (_designTemperatureText != value)
                {
                    _designTemperatureText = value;
                    OnPropertyChanged(nameof(DesignTemperatureText));
                    ValidateAndSetDesignTemperature(value);
                }
            }
        }

        private string _reductionTemperatureText;
        public string ReductionTemperatureText
        {
            get => _reductionTemperatureText;
            set
            {
                if (_reductionTemperatureText != value)
                {
                    _reductionTemperatureText = value;
                    OnPropertyChanged(nameof(ReductionTemperatureText));
                    ValidateAndSetReductionTemperature(value);
                }
            }
        }

        public double EmissionEfficiency
        {
            get => _data.EmissionEfficiency;
            set
            {
                if (_data.EmissionEfficiency != value)
                {
                    var clamped = Math.Clamp(value, 0, 100);
                    _data.EmissionEfficiency = clamped;
                    OnPropertyChanged(nameof(EmissionEfficiency));
                }
            }
        }

        public double DistributionEfficiency
        {
            get => _data.DistributionEfficiency;
            set
            {
                if (_data.DistributionEfficiency != value)
                {
                    var clamped = Math.Clamp(value, 0, 100);
                    _data.DistributionEfficiency = clamped;
                    OnPropertyChanged(nameof(DistributionEfficiency));
                }
            }
        }

        public double AutomaticControl
        {
            get => _data.AutomaticControl;
            set
            {
                if (_data.AutomaticControl != value)
                {
                    var clamped = Math.Clamp(value, 0, 100);
                    _data.AutomaticControl = clamped;
                    OnPropertyChanged(nameof(AutomaticControl));
                }
            }
        }

        public double EnergyManagement
        {
            get => _data.EnergyManagement;
            set
            {
                if (_data.EnergyManagement != value)
                {
                    var clamped = Math.Clamp(value, 0, 100);
                    _data.EnergyManagement = clamped;
                    OnPropertyChanged(nameof(EnergyManagement));
                }
            }
        }

        public double HeatingEfficiency
        {
            get => _data.HeatingEfficiency;
            set
            {
                if (_data.HeatingEfficiency != value)
                {
                    // Allow heating efficiency values greater than 100 (some heat sources may be represented >100%),
                    // but do not allow negative values.
                    var bounded = Math.Max(value, 0);
                    _data.HeatingEfficiency = bounded;
                    OnPropertyChanged(nameof(HeatingEfficiency));
                }
            }
        }

        // ========== ENERGY SOURCE 1 PROPERTIES (for heating) ==========

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

                    // If second source is used, auto-adjust the other share so total is 100%
                    if (UseSecondEnergySource && _data.EnergySource2 != null && !_isAdjustingShares)
                    {
                        try
                        {
                            _isAdjustingShares = true;
                            _data.EnergySource2.Share = Math.Clamp(100.0 - clamped, 0.0, 100.0);
                            OnPropertyChanged(nameof(EnergySource2Share));
                        }
                        finally
                        {
                            _isAdjustingShares = false;
                        }
                    }

                    NotifyGrossHeatingEnergyChanged();
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
                    NotifyGrossHeatingEnergyChanged();
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
                    NotifyGrossHeatingEnergyChanged();
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
                    NotifyGrossHeatingEnergyChanged();
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
                    NotifyGrossHeatingEnergyChanged();
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
                    NotifyGrossHeatingEnergyChanged();
                }
            }
        }

        public Models.EnergyCarrierCode? EnergySource1Carrier
        {
            get => _data.EnergySource1.EnergyCarrier;
            set
            {
                if (_data.EnergySource1.EnergyCarrier != value)
                {
                    _data.EnergySource1.EnergyCarrier = value;
                    OnPropertyChanged(nameof(EnergySource1Carrier));
                }
            }
        }

        // ========== ENERGY SOURCE 2 PROPERTIES (optional) ==========

        public bool UseSecondEnergySource
        {
            get => _data.UseSecondEnergySource;
            set
            {
                if (_data.UseSecondEnergySource != value)
                {
                    _data.UseSecondEnergySource = value;
                    // If disabling second source, force shares to 100/0
                    if (!value)
                    {
                        if (_data.EnergySource1 != null)
                        {
                            _data.EnergySource1.Share = 100.0;
                            OnPropertyChanged(nameof(EnergySource1Share));
                        }
                        if (_data.EnergySource2 != null)
                        {
                            _data.EnergySource2.Share = 0.0;
                            OnPropertyChanged(nameof(EnergySource2Share));
                        }
                    }
                    else
                    {
                        // Ensure energySource2 exists
                        if (_data.EnergySource2 == null)
                        {
                            _data.EnergySource2 = new VentilationEnergySource
                            {
                                Type = EnergySourceType.Electricity,
                                Share = 0.0
                            };
                        }
                    }

                    OnPropertyChanged(nameof(UseSecondEnergySource));
                    NotifyGrossHeatingEnergyChanged();
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

                    // Auto-adjust source1 if needed
                    if (UseSecondEnergySource && _data.EnergySource1 != null && !_isAdjustingShares)
                    {
                        try
                        {
                            _isAdjustingShares = true;
                            _data.EnergySource1.Share = Math.Clamp(100.0 - clamped, 0.0, 100.0);
                            OnPropertyChanged(nameof(EnergySource1Share));
                        }
                        finally
                        {
                            _isAdjustingShares = false;
                        }
                    }

                    NotifyGrossHeatingEnergyChanged();
                }
            }
        }

        public double EnergySource2EmissionEfficiency
        {
            get => _data.EnergySource2?.EmissionEfficiency ?? 100;
            set
            {
                if (_data.EnergySource2 != null && _data.EnergySource2.EmissionEfficiency != value)
                {
                    var clamped = Math.Clamp(value, 0, 100);
                    _data.EnergySource2.EmissionEfficiency = clamped;
                    OnPropertyChanged(nameof(EnergySource2EmissionEfficiency));
                    NotifyGrossHeatingEnergyChanged();
                }
            }
        }

        public double EnergySource2DistributionEfficiency
        {
            get => _data.EnergySource2?.DistributionEfficiency ?? 100;
            set
            {
                if (_data.EnergySource2 != null && _data.EnergySource2.DistributionEfficiency != value)
                {
                    var clamped = Math.Clamp(value, 0, 100);
                    _data.EnergySource2.DistributionEfficiency = clamped;
                    OnPropertyChanged(nameof(EnergySource2DistributionEfficiency));
                    NotifyGrossHeatingEnergyChanged();
                }
            }
        }

        public double EnergySource2AutomaticControl
        {
            get => _data.EnergySource2?.AutomaticControl ?? 100;
            set
            {
                if (_data.EnergySource2 != null && _data.EnergySource2.AutomaticControl != value)
                {
                    var clamped = Math.Clamp(value, 0, 100);
                    _data.EnergySource2.AutomaticControl = clamped;
                    OnPropertyChanged(nameof(EnergySource2AutomaticControl));
                    NotifyGrossHeatingEnergyChanged();
                }
            }
        }

        public double EnergySource2EnergyManagement
        {
            get => _data.EnergySource2?.EnergyManagement ?? 100;
            set
            {
                if (_data.EnergySource2 != null && _data.EnergySource2.EnergyManagement != value)
                {
                    var clamped = Math.Clamp(value, 0, 100);
                    _data.EnergySource2.EnergyManagement = clamped;
                    OnPropertyChanged(nameof(EnergySource2EnergyManagement));
                    NotifyGrossHeatingEnergyChanged();
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
                    NotifyGrossHeatingEnergyChanged();
                }
            }
        }

        public Models.EnergyCarrierCode? EnergySource2Carrier
        {
            get => _data.EnergySource2?.EnergyCarrier;
            set
            {
                if (_data.EnergySource2 != null && _data.EnergySource2.EnergyCarrier != value)
                {
                    _data.EnergySource2.EnergyCarrier = value;
                    OnPropertyChanged(nameof(EnergySource2Carrier));
                }
            }
        }

        // ========== пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ ==========

        /// <summary>
        /// пїЅпїЅпїЅпїЅпїЅпїЅ пїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅ dropdown
        /// </summary>
        public List<ActivityLevelOption> ActivityLevelOptions { get; }

        public ActivityLevel SelectedActivityLevel
        {
            get => _data.SelectedActivityLevel;
            set
            {
                if (_data.SelectedActivityLevel != value)
                {
                    _data.SelectedActivityLevel = value;
                    OnPropertyChanged(nameof(SelectedActivityLevel));
                    RecalculateOccupantHeat();
                }
            }
        }

        /// <summary>
        /// пїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ (read-only, пїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅ 5)
        /// </summary>
        public int NumberOfOccupants
        {
            get
            {
                if (_objectData?.NumberOfOccupants != null &&
                    int.TryParse(_objectData.NumberOfOccupants, NumberStyles.Integer, CultureInfo.InvariantCulture, out int count))
                {
                    return count;
                }
                return 0;
            }
        }

        /// <summary>
        /// пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ (read-only, пїЅпїЅпїЅпїЅпїЅ пїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ)
        /// </summary>
        public string RoomTemperatureDisplay => $"{_data.DesignTemperature:F2} \u00B0C";

        public ObservableCollection<HeatingCharacteristicRow> ThermalCharacteristicsRows { get; } = new();
        public string HeatingInstallationHoursDisplay => $"{_heatingInstallationHours:F0}";
        public double NetHeatingEnergyNoGainsPerArea { get; private set; }
        public string NetHeatingEnergyNoGainsPerAreaDisplay => NetHeatingEnergyNoGainsPerArea.ToString("F2", CultureInfo.InvariantCulture);
        public double VentilationGainPerArea { get; private set; } = 0.0;
        public double LightingGainPerArea { get; private set; } = 0.0;
        public double AppliancesGainPerArea { get; private set; } = 0.0;
        public double TimeConstant_h => _lastAnnual?.Tau ?? 0.0;
        public double UtilizationFactor => _lastAnnual != null && _lastAnnual.Qht_total > 0.0
            ? 1.0 - (_lastAnnual.QH_total_kWh / _lastAnnual.Qht_total)
            : 0.0;
        public double SolarGain_total_kWh => _lastAnnual?.Qsol_total ?? 0.0;
        public double QH_per_m2 => _lastAnnual?.QH_per_m2 ?? 0.0;
        public bool IsAshraePlaceholder => _data.CalculationMethod == HeatingCalculationMethod.Ashrae8760;
        public double NetHeatingEnergyAfterGains =>
            _lastAnnual?.IsValid == true
                ? Math.Max(0.0, QH_per_m2 - VentilationGainPerArea)
                : Math.Max(0.0, NetHeatingEnergyNoGainsPerArea
                    - VentilationGainPerArea
                    - LightingGainPerArea
                    - AppliancesGainPerArea);
        public string VentilationGainPerAreaDisplay => VentilationGainPerArea.ToString("F2", CultureInfo.InvariantCulture);
        public string LightingGainPerAreaDisplay => LightingGainPerArea.ToString("F2", CultureInfo.InvariantCulture);
        public string AppliancesGainPerAreaDisplay => AppliancesGainPerArea.ToString("F2", CultureInfo.InvariantCulture);
        public string NetHeatingEnergyAfterGainsDisplay => NetHeatingEnergyAfterGains.ToString("F2", CultureInfo.InvariantCulture);
        public double VentilationGainKwh => VentilationGainPerArea * HeatedArea;
        public double LightingGainKwh => LightingGainPerArea * HeatedArea;
        public double AppliancesGainKwh => AppliancesGainPerArea * HeatedArea;
        public double NetHeatingEnergyAfterGainsKwh => NetHeatingEnergyAfterGains * HeatedArea;
        public string VentilationGainKwhDisplay => VentilationGainKwh.ToString("F2", CultureInfo.InvariantCulture);
        public string LightingGainKwhDisplay => LightingGainKwh.ToString("F2", CultureInfo.InvariantCulture);
        public string AppliancesGainKwhDisplay => AppliancesGainKwh.ToString("F2", CultureInfo.InvariantCulture);
        public string NetHeatingEnergyAfterGainsKwhDisplay => NetHeatingEnergyAfterGainsKwh.ToString("F2", CultureInfo.InvariantCulture);
        public double? EnergySource1RequiredEnergyPerArea => CalculateRequiredEnergyForSource(
            NetHeatingEnergyAfterGains,
            EnergySource1Share,
            EnergySource1EmissionEfficiency,
            EnergySource1DistributionEfficiency,
            EnergySource1AutomaticControl,
            EnergySource1EnergyManagement,
            EnergySource1GenerationEfficiency);
        public string EnergySource1RequiredEnergyPerAreaDisplay => FormatNullableDouble(EnergySource1RequiredEnergyPerArea);
        public double? EnergySource2RequiredEnergyPerArea => UseSecondEnergySource
            ? CalculateRequiredEnergyForSource(
                NetHeatingEnergyAfterGains,
                EnergySource2Share,
                EnergySource2EmissionEfficiency,
                EnergySource2DistributionEfficiency,
                EnergySource2AutomaticControl,
                EnergySource2EnergyManagement,
                EnergySource2GenerationEfficiency)
            : 0.0;
        public string EnergySource2RequiredEnergyPerAreaDisplay => FormatNullableDouble(EnergySource2RequiredEnergyPerArea);
        public double TotalGrossHeatingEnergyPerArea =>
            (EnergySource1RequiredEnergyPerArea ?? 0.0) +
            (EnergySource2RequiredEnergyPerArea ?? 0.0);
        public string TotalGrossHeatingEnergyPerAreaDisplay => TotalGrossHeatingEnergyPerArea.ToString("F2", CultureInfo.InvariantCulture);
        public double? OverallHeatGenerationEfficiencyPercent =>
            TotalGrossHeatingEnergyPerArea > 0.0
                ? NetHeatingEnergyAfterGains / TotalGrossHeatingEnergyPerArea * 100.0
                : null;
        public string OverallHeatGenerationEfficiencyDisplay => FormatNullableDouble(OverallHeatGenerationEfficiencyPercent);
        public bool HasEnergySourceShareWarning => Math.Abs(GetTotalEnergySourceShare() - 100.0) > 0.01;
        public string EnergySourceShareWarning => "\u0421\u0431\u043E\u0440\u044A\u0442 \u043D\u0430 \u0434\u044F\u043B\u043E\u0432\u0435\u0442\u0435 \u0442\u0440\u044F\u0431\u0432\u0430 \u0434\u0430 \u0435 100%";

        // ========== пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ ==========

        private double _sensibleHeatPerPerson;
        public double SensibleHeatPerPerson
        {
            get => _sensibleHeatPerPerson;
            private set
            {
                if (Math.Abs(_sensibleHeatPerPerson - value) > 0.001)
                {
                    _sensibleHeatPerPerson = value;
                    OnPropertyChanged(nameof(SensibleHeatPerPerson));
                    OnPropertyChanged(nameof(SensibleHeatPerPersonDisplay));
                }
            }
        }

        public string SensibleHeatPerPersonDisplay => $"{SensibleHeatPerPerson:F2} W";

        private double _latentHeatPerPerson;
        public double LatentHeatPerPerson
        {
            get => _latentHeatPerPerson;
            private set
            {
                if (Math.Abs(_latentHeatPerPerson - value) > 0.001)
                {
                    _latentHeatPerPerson = value;
                    OnPropertyChanged(nameof(LatentHeatPerPerson));
                    OnPropertyChanged(nameof(LatentHeatPerPersonDisplay));
                }
            }
        }

        public string LatentHeatPerPersonDisplay => $"{LatentHeatPerPerson:F2} W";

        private double _totalOccupantHeat;
        public double TotalOccupantHeat
        {
            get => _totalOccupantHeat;
            private set
            {
                if (Math.Abs(_totalOccupantHeat - value) > 0.001)
                {
                    _totalOccupantHeat = value;
                    OnPropertyChanged(nameof(TotalOccupantHeat));
                    OnPropertyChanged(nameof(TotalOccupantHeatDisplay));
                }
            }
        }

        public string TotalOccupantHeatDisplay => $"{TotalOccupantHeat:F2} W";

        private double _totalLatentHeat;
        public double TotalLatentHeat
        {
            get => _totalLatentHeat;
            private set
            {
                if (Math.Abs(_totalLatentHeat - value) > 0.001)
                {
                    _totalLatentHeat = value;
                    OnPropertyChanged(nameof(TotalLatentHeat));
                    OnPropertyChanged(nameof(TotalLatentHeatDisplay));
                }
            }
        }

        public string TotalLatentHeatDisplay => $"{TotalLatentHeat:F2} W";

        // ========== W/m? пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ ==========

        /// <summary>
        /// пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅ пїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅ 5
        /// </summary>
        public double HeatedArea
        {
            get
            {
                if (_objectData?.HeatedArea != null &&
                    double.TryParse(_objectData.HeatedArea.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out double area))
                {
                    return area > 0 ? area : 0;
                }
                return 0;
            }
        }

        /// <summary>
        /// пїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ [W/m?]
        /// </summary>
        public double TotalOccupantHeatPerArea => HeatedArea > 0 ? TotalOccupantHeat / HeatedArea : 0;

        public string TotalOccupantHeatPerAreaDisplay => HeatedArea > 0 
            ? $"{TotalOccupantHeatPerArea:F2} W/m\u00B2" 
            : "\u2014 (\u043D\u044F\u043C\u0430 \u0434\u0430\u043D\u043D\u0438)";

        /// <summary>
        /// пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ [W/m?]
        /// </summary>
        public double TotalLatentHeatPerArea => HeatedArea > 0 ? TotalLatentHeat / HeatedArea : 0;

        public string TotalLatentHeatPerAreaDisplay => HeatedArea > 0 
            ? $"{TotalLatentHeatPerArea:F2} W/m\u00B2" 
            : "\u2014 (\u043D\u044F\u043C\u0430 \u0434\u0430\u043D\u043D\u0438)";

        // ========== VALIDATION ERRORS ==========

        private string? _infiltrationError;
        public string? InfiltrationError
        {
            get => _infiltrationError;
            private set
            {
                if (_infiltrationError != value)
                {
                    _infiltrationError = value;
                    OnPropertyChanged(nameof(InfiltrationError));
                    OnPropertyChanged(nameof(HasInfiltrationError));
                }
            }
        }

        public bool HasInfiltrationError => !string.IsNullOrEmpty(InfiltrationError);

        private string? _designTemperatureError;
        public string? DesignTemperatureError
        {
            get => _designTemperatureError;
            private set
            {
                if (_designTemperatureError != value)
                {
                    _designTemperatureError = value;
                    OnPropertyChanged(nameof(DesignTemperatureError));
                    OnPropertyChanged(nameof(HasDesignTemperatureError));
                }
            }
        }

        public bool HasDesignTemperatureError => !string.IsNullOrEmpty(DesignTemperatureError);

        private string? _reductionTemperatureError;
        public string? ReductionTemperatureError
        {
            get => _reductionTemperatureError;
            private set
            {
                if (_reductionTemperatureError != value)
                {
                    _reductionTemperatureError = value;
                    OnPropertyChanged(nameof(ReductionTemperatureError));
                    OnPropertyChanged(nameof(HasReductionTemperatureError));
                }
            }
        }

        public bool HasReductionTemperatureError => !string.IsNullOrEmpty(ReductionTemperatureError);

        // ========== CONSTRUCTOR ==========

        public HeatingSectionViewModel(HeatingSectionData data, ObjectDataSectionData? objectData = null, Report? report = null)
        {
            _data = data;
            _objectData = objectData;
            _report = report;
            _wallsData = _report?.Sections?.FirstOrDefault(s => s.Type == SectionType.ExternalWalls)?.ExternalWallsSectionData;
            _roofData = _report?.Sections?.FirstOrDefault(s => s.Type == SectionType.Roof)?.RoofSectionData;
            _floorData = _report?.Sections?.FirstOrDefault(s => s.Type == SectionType.Floor)?.FloorSectionData;
            _windowsData = _report?.Sections?.FirstOrDefault(s => s.Type == SectionType.Windows)?.WindowsSectionData;
            _ztuData = _report?.Sections?.FirstOrDefault(s => s.Type == SectionType.UnconditionedZones)?.UnconditionedZoneSectionData;
            _section23Data = _report?.Sections?.FirstOrDefault(s => s.Type == SectionType.InternalGainsDebug)?.InternalGainsDebugInput;
            _section24Data = _report?.Sections?.FirstOrDefault(s => s.Type == SectionType.SolarGains)?.SolarGainsData;
            if (_section23Data != null)
            {
                _internalGainsService = new InternalGainsService(_section23Data, _objectData, _report);
            }
            _ventilationHeatingData = _report?.Sections?.FirstOrDefault(s =>
                s.Type == SectionType.Ventilation &&
                s.VentilationSectionData != null &&
                (s.Title?.Contains("\u0412\u0435\u043D\u0442\u0438\u043B\u0430\u0446\u0438\u044F \u041E\u0442\u043E\u043F\u043B\u0435\u043D\u0438\u0435", StringComparison.OrdinalIgnoreCase) ?? false))
                ?.VentilationSectionData;

            // пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅ activity options
            ActivityLevelOptions = ActivityDataService.GetAllActivities()
                .Select(a => new ActivityLevelOption
                {
                    Level = a.ActivityLevel,
                    DisplayName = a.DisplayName
                })
                .ToList();

            // пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅ
            _infiltrationText = _data.Infiltration.ToString("F2", CultureInfo.InvariantCulture);
            _designTemperatureText = _data.DesignTemperature.ToString("F2", CultureInfo.InvariantCulture);
            _reductionTemperatureText = _data.ReductionTemperature.ToString("F2", CultureInfo.InvariantCulture);

            // Subscribe пїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅ ObjectData пїЅпїЅ пїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ
            if (_objectData != null)
            {
                _objectData.PropertyChanged += ObjectData_PropertyChanged;
            }

            if (_ventilationHeatingData != null)
            {
                _ventilationHeatingData.PropertyChanged += VentilationHeatingData_PropertyChanged;
            }

            // пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ
            RecalculateOccupantHeat();
            AttachSectionListeners();
            RefreshThermalCharacteristics();
        }

        private void ObjectData_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ObjectDataSectionData.NumberOfOccupants))
            {
                OnPropertyChanged(nameof(NumberOfOccupants));
                RecalculateOccupantHeat();
                RefreshThermalCharacteristics();
            }
            else if (e.PropertyName == nameof(ObjectDataSectionData.HeatedArea))
            {
                OnPropertyChanged(nameof(HeatedArea));
                OnPropertyChanged(nameof(TotalOccupantHeatPerArea));
                OnPropertyChanged(nameof(TotalOccupantHeatPerAreaDisplay));
                OnPropertyChanged(nameof(TotalLatentHeatPerArea));
                OnPropertyChanged(nameof(TotalLatentHeatPerAreaDisplay));
                NotifyGrossHeatingEnergyChanged();
            }
            else if (e.PropertyName == nameof(ObjectDataSectionData.HeatingSeasonEnabled))
            {
                OnPropertyChanged(nameof(IsHeatingSeasonEnabled));
                OnPropertyChanged(nameof(HeatingSeasonWarning));
            }

            RefreshThermalCharacteristics();
        }

        private void VentilationHeatingData_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            RefreshThermalCharacteristics();
        }

        private void SetMethod(HeatingCalculationMethod method)
        {
            if (_data.CalculationMethod == method)
            {
                return;
            }

            _data.CalculationMethod = method;
            OnPropertyChanged(nameof(IsMethodAuer));
            OnPropertyChanged(nameof(IsMethodRd));
            OnPropertyChanged(nameof(IsMethodAshrae));
            OnPropertyChanged(nameof(MethodStatusText));
            OnPropertyChanged(nameof(MethodStatusBrush));
            RefreshThermalCharacteristics();
        }

        // ========== VALIDATION & PARSING ==========

        private void ValidateAndSetInfiltration(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                InfiltrationError = "\u041F\u043E\u043B\u0435\u0442\u043E \u0435 \u0437\u0430\u0434\u044A\u043B\u0436\u0438\u0442\u0435\u043B\u043D\u043E";
                return;
            }

            // Normalize: replace comma with dot
            var normalized = text.Replace(',', '.');

            if (double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
            {
                if (value < 0)
                {
                    InfiltrationError = "\u0421\u0442\u043E\u0439\u043D\u043E\u0441\u0442\u0442\u0430 \u0442\u0440\u044F\u0431\u0432\u0430 \u0434\u0430 \u0435 >= 0";
                    return;
                }

                // Round to 2 decimal places
                value = Math.Round(value, 2);
                _data.Infiltration = value;
                InfiltrationError = null;

                // Update text to normalized format
                if (_infiltrationText != value.ToString("F2", CultureInfo.InvariantCulture))
                {
                    _infiltrationText = value.ToString("F2", CultureInfo.InvariantCulture);
                    OnPropertyChanged(nameof(InfiltrationText));
                }
                RefreshThermalCharacteristics();
            }
            else
            {
                InfiltrationError = "\u041D\u0435\u0432\u0430\u043B\u0438\u0434\u043D\u043E \u0447\u0438\u0441\u043B\u043E";
            }
        }

        private void ValidateAndSetDesignTemperature(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                DesignTemperatureError = "\u041F\u043E\u043B\u0435\u0442\u043E \u0435 \u0437\u0430\u0434\u044A\u043B\u0436\u0438\u0442\u0435\u043B\u043D\u043E";
                return;
            }

            var normalized = text.Replace(',', '.');

            if (double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
            {
                value = Math.Round(value, 2);
                _data.DesignTemperature = value;
                DesignTemperatureError = null;

                if (_designTemperatureText != value.ToString("F2", CultureInfo.InvariantCulture))
                {
                    _designTemperatureText = value.ToString("F2", CultureInfo.InvariantCulture);
                    OnPropertyChanged(nameof(DesignTemperatureText));
                }

                // пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ (пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅ пїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ)
                OnPropertyChanged(nameof(RoomTemperatureDisplay));
                RecalculateOccupantHeat();
                RefreshThermalCharacteristics();
            }
            else
            {
                DesignTemperatureError = "\u041D\u0435\u0432\u0430\u043B\u0438\u0434\u043D\u043E \u0447\u0438\u0441\u043B\u043E";
            }
        }

        private void ValidateAndSetReductionTemperature(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                ReductionTemperatureError = "\u041F\u043E\u043B\u0435\u0442\u043E \u0435 \u0437\u0430\u0434\u044A\u043B\u0436\u0438\u0442\u0435\u043B\u043D\u043E";
                return;
            }

            var normalized = text.Replace(',', '.');

            if (double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
            {
                value = Math.Round(value, 2);
                _data.ReductionTemperature = value;
                ReductionTemperatureError = null;

                if (_reductionTemperatureText != value.ToString("F2", CultureInfo.InvariantCulture))
                {
                    _reductionTemperatureText = value.ToString("F2", CultureInfo.InvariantCulture);
                    OnPropertyChanged(nameof(ReductionTemperatureText));
                }
                RefreshThermalCharacteristics();
            }
            else
            {
                ReductionTemperatureError = "\u041D\u0435\u0432\u0430\u043B\u0438\u0434\u043D\u043E \u0447\u0438\u0441\u043B\u043E";
            }
        }

        // ========== CALCULATION ==========

        /// <summary>
        /// пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ
        /// </summary>
        private void RecalculateOccupantHeat()
        {
            var temperature = _data.DesignTemperature;
            var activityLevel = _data.SelectedActivityLevel;
            var occupantCount = NumberOfOccupants;

            // пїЅпїЅпїЅпїЅпїЅпїЅпїЅ sensible пїЅ latent heat per person пїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ
            var (sensible, latent) = ActivityDataService.CalculateHeatForTemperature(activityLevel, temperature);

            SensibleHeatPerPerson = sensible;
            LatentHeatPerPerson = latent;

            // пїЅпїЅпїЅпїЅпїЅпїЅпїЅ total heat
            TotalOccupantHeat = sensible * occupantCount;
            TotalLatentHeat = latent * occupantCount;

            // пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ W/m? пїЅпїЅпїЅпїЅпїЅпїЅ
            OnPropertyChanged(nameof(HeatedArea));
            OnPropertyChanged(nameof(TotalOccupantHeatPerArea));
            OnPropertyChanged(nameof(TotalOccupantHeatPerAreaDisplay));
            OnPropertyChanged(nameof(TotalLatentHeatPerArea));
            OnPropertyChanged(nameof(TotalLatentHeatPerAreaDisplay));
        }

        private void AttachSectionListeners()
        {
            AttachWallsListeners();
            AttachRoofListeners();
            AttachFloorListeners();
            AttachWindowsListeners();
            AttachZtuListeners();
            AttachSection23Listeners();
            AttachSection24Listeners();
        }

        private void AttachSection23Listeners()
        {
            if (_section23Data == null)
            {
                return;
            }

            _section23Data.HeatingMonths.CollectionChanged += Section23HeatingMonths_CollectionChanged;
        }

        private void AttachSection24Listeners()
        {
            if (_section24Data == null)
            {
                return;
            }

            _section24Data.MonthlyResults.CollectionChanged += Section24MonthlyResults_CollectionChanged;
        }

        private void Section23HeatingMonths_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            RefreshThermalCharacteristics();
        }

        private void Section24MonthlyResults_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            RefreshThermalCharacteristics();
        }

        private void AttachWallsListeners()
        {
            if (_wallsData == null)
            {
                return;
            }

            _wallsData.WallTypes.CollectionChanged += Walls_CollectionChanged;
            foreach (var wall in _wallsData.WallTypes)
            {
                AttachWall(wall);
            }
        }

        private void Walls_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (ExternalWallType wall in e.OldItems)
                {
                    wall.PropertyChanged -= SourceData_PropertyChanged;
                    if (wall.ThermalBridges != null)
                    {
                        wall.ThermalBridges.PropertyChanged -= SourceData_PropertyChanged;
                    }
                }
            }

            if (e.NewItems != null)
            {
                foreach (ExternalWallType wall in e.NewItems)
                {
                    AttachWall(wall);
                }
            }

            RefreshThermalCharacteristics();
        }

        private void AttachWall(ExternalWallType wall)
        {
            wall.PropertyChanged += SourceData_PropertyChanged;
            if (wall.ThermalBridges != null)
            {
                wall.ThermalBridges.PropertyChanged += SourceData_PropertyChanged;
            }
        }

        private void AttachRoofListeners()
        {
            if (_roofData == null)
            {
                return;
            }

            _roofData.RoofTypes.CollectionChanged += Roof_CollectionChanged;
            foreach (var roof in _roofData.RoofTypes)
            {
                AttachRoof(roof);
            }
        }

        private void Roof_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (RoofType roof in e.OldItems)
                {
                    roof.PropertyChanged -= SourceData_PropertyChanged;
                    if (roof.ThermalBridges != null)
                    {
                        roof.ThermalBridges.PropertyChanged -= SourceData_PropertyChanged;
                    }
                }
            }

            if (e.NewItems != null)
            {
                foreach (RoofType roof in e.NewItems)
                {
                    AttachRoof(roof);
                }
            }

            RefreshThermalCharacteristics();
        }

        private void AttachRoof(RoofType roof)
        {
            roof.PropertyChanged += SourceData_PropertyChanged;
            if (roof.ThermalBridges != null)
            {
                roof.ThermalBridges.PropertyChanged += SourceData_PropertyChanged;
            }
        }

        private void AttachFloorListeners()
        {
            if (_floorData == null)
            {
                return;
            }

            _floorData.FloorItems.CollectionChanged += Floor_CollectionChanged;
            foreach (var item in _floorData.FloorItems)
            {
                item.PropertyChanged += SourceData_PropertyChanged;
            }
        }

        private void Floor_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (FloorItem item in e.OldItems)
                {
                    item.PropertyChanged -= SourceData_PropertyChanged;
                }
            }

            if (e.NewItems != null)
            {
                foreach (FloorItem item in e.NewItems)
                {
                    item.PropertyChanged += SourceData_PropertyChanged;
                }
            }

            RefreshThermalCharacteristics();
        }

        private void AttachWindowsListeners()
        {
            if (_windowsData == null)
            {
                return;
            }

            _windowsData.WindowBatches.CollectionChanged += Windows_CollectionChanged;
            foreach (var batch in _windowsData.WindowBatches)
            {
                batch.PropertyChanged += SourceData_PropertyChanged;
            }
        }

        private void Windows_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (WindowBatch batch in e.OldItems)
                {
                    batch.PropertyChanged -= SourceData_PropertyChanged;
                }
            }

            if (e.NewItems != null)
            {
                foreach (WindowBatch batch in e.NewItems)
                {
                    batch.PropertyChanged += SourceData_PropertyChanged;
                }
            }

            RefreshThermalCharacteristics();
        }

        private void AttachZtuListeners()
        {
            if (_ztuData == null)
            {
                return;
            }

            _ztuData.Zones.CollectionChanged += ZtuZones_CollectionChanged;
            foreach (var zone in _ztuData.Zones)
            {
                AttachZone(zone);
            }
        }

        private void ZtuZones_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (ZtuZone zone in e.OldItems)
                {
                    DetachZone(zone);
                }
            }

            if (e.NewItems != null)
            {
                foreach (ZtuZone zone in e.NewItems)
                {
                    AttachZone(zone);
                }
            }

            RefreshThermalCharacteristics();
        }

        private void AttachZone(ZtuZone zone)
        {
            zone.PropertyChanged += SourceData_PropertyChanged;
            zone.ElementsToExternal.CollectionChanged += ZtuElements_CollectionChanged;
            zone.ElementsToBoundary.CollectionChanged += ZtuElements_CollectionChanged;

            foreach (var element in zone.ElementsToExternal)
            {
                AttachElement(element);
            }

            foreach (var element in zone.ElementsToBoundary)
            {
                AttachElement(element);
            }
        }

        private void DetachZone(ZtuZone zone)
        {
            zone.PropertyChanged -= SourceData_PropertyChanged;
            zone.ElementsToExternal.CollectionChanged -= ZtuElements_CollectionChanged;
            zone.ElementsToBoundary.CollectionChanged -= ZtuElements_CollectionChanged;

            foreach (var element in zone.ElementsToExternal)
            {
                DetachElement(element);
            }

            foreach (var element in zone.ElementsToBoundary)
            {
                DetachElement(element);
            }
        }

        private void ZtuElements_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (ZtuElement element in e.OldItems)
                {
                    DetachElement(element);
                }
            }

            if (e.NewItems != null)
            {
                foreach (ZtuElement element in e.NewItems)
                {
                    AttachElement(element);
                }
            }

            RefreshThermalCharacteristics();
        }

        private void AttachElement(ZtuElement element)
        {
            element.PropertyChanged += SourceData_PropertyChanged;
            element.Layers.CollectionChanged += ZtuLayers_CollectionChanged;
            foreach (var layer in element.Layers)
            {
                layer.PropertyChanged += SourceData_PropertyChanged;
            }
        }

        private void DetachElement(ZtuElement element)
        {
            element.PropertyChanged -= SourceData_PropertyChanged;
            element.Layers.CollectionChanged -= ZtuLayers_CollectionChanged;
            foreach (var layer in element.Layers)
            {
                layer.PropertyChanged -= SourceData_PropertyChanged;
            }
        }

        private void ZtuLayers_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (ZtuLayer layer in e.OldItems)
                {
                    layer.PropertyChanged -= SourceData_PropertyChanged;
                }
            }

            if (e.NewItems != null)
            {
                foreach (ZtuLayer layer in e.NewItems)
                {
                    layer.PropertyChanged += SourceData_PropertyChanged;
                }
            }

            RefreshThermalCharacteristics();
        }

        private void SourceData_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            RefreshThermalCharacteristics();
        }

        private void RefreshThermalCharacteristics()
        {
            var snapshot = HeatingCharacteristicsService.Build(_report, _objectData, _data);
            VentilationGainPerArea = CalculateVentilationHeatingContributionPerArea();
            RefreshSectionGainContributions();

            double hTrTotal = snapshot.Walls.H + snapshot.Roof.H + snapshot.Floor.H + snapshot.Windows.H + snapshot.ThermalBridgesH_WK + snapshot.ZtuH_WK;
            double hInf = 0.34 * snapshot.InfiltrationRate_AirChangesPerHour * snapshot.BuildingVolume_m3;
            double hVeTotal = hInf;
            double hTotal = hTrTotal + hVeTotal;
            double annualEquivalentHours = CalculateAnnualEquivalentHours(snapshot);
            double deltaT = 1.0;
            double energyHours = annualEquivalentHours;
            _heatingInstallationHours = snapshot.HeatingOperatingHours_h;
            RecalculateHeating(snapshot, hTrTotal, hVeTotal);
            NetHeatingEnergyNoGainsPerArea = _lastAnnual?.IsValid == true
                ? (_lastAnnual.Qht_total / Math.Max(snapshot.HeatedArea_m2, 1e-9))
                : snapshot.HeatedArea_m2 > 0.0
                    ? (hTotal * deltaT * energyHours / 1000.0) / snapshot.HeatedArea_m2
                    : 0.0;
            OnPropertyChanged(nameof(HeatingInstallationHoursDisplay));
            NotifyGrossHeatingEnergyChanged();

            ThermalCharacteristicsRows.Clear();
            ThermalCharacteristicsRows.Add(CreateMetricRow("\u0412\u044A\u043D\u0448\u043D\u0438 \u0441\u0442\u0435\u043D\u0438", snapshot.Walls.U, snapshot.Walls.Area, snapshot.Walls.H, snapshot.HeatedArea_m2, deltaT, energyHours));
            ThermalCharacteristicsRows.Add(CreateMetricRow("\u041F\u043E\u043A\u0440\u0438\u0432", snapshot.Roof.U, snapshot.Roof.Area, snapshot.Roof.H, snapshot.HeatedArea_m2, deltaT, energyHours));
            ThermalCharacteristicsRows.Add(CreateMetricRow("\u041F\u043E\u0434", snapshot.Floor.U, snapshot.Floor.Area, snapshot.Floor.H, snapshot.HeatedArea_m2, deltaT, energyHours));
            ThermalCharacteristicsRows.Add(CreateMetricRow("\u041F\u0440\u043E\u0437\u043E\u0440\u0446\u0438 \u0438 \u0432\u0440\u0430\u0442\u0438", snapshot.Windows.U, snapshot.Windows.Area, snapshot.Windows.H, snapshot.HeatedArea_m2, deltaT, energyHours));
            ThermalCharacteristicsRows.Add(CreateMetricRow("\u0422\u043E\u043F\u043B\u0438\u043D\u043D\u0438 \u043C\u043E\u0441\u0442\u043E\u0432\u0435", null, null, snapshot.ThermalBridgesH_WK, snapshot.HeatedArea_m2, deltaT, energyHours));
            ThermalCharacteristicsRows.Add(CreateMetricRow("ZTU (\u043D\u0435\u043A\u043B\u0438\u043C\u0430\u0442.)", null, null, snapshot.ZtuH_WK, snapshot.HeatedArea_m2, deltaT, energyHours));
            ThermalCharacteristicsRows.Add(HeatingCharacteristicRow.Separator());
            ThermalCharacteristicsRows.Add(CreateMetricRow("H_tr \u043E\u0431\u0449\u043E", null, null, hTrTotal, snapshot.HeatedArea_m2, deltaT, energyHours, isTotal: true));
            ThermalCharacteristicsRows.Add(CreateMetricRow("\u0418\u043D\u0444\u0438\u043B\u0442\u0440\u0430\u0446\u0438\u044F", snapshot.InfiltrationRate_AirChangesPerHour, snapshot.BuildingVolume_m3, hInf, snapshot.HeatedArea_m2, deltaT, energyHours, uUnit: "1/h", aUnit: "m3"));
            ThermalCharacteristicsRows.Add(HeatingCharacteristicRow.Separator());
            ThermalCharacteristicsRows.Add(CreateMetricRow("H_ve \u043E\u0431\u0449\u043E", null, null, hVeTotal, snapshot.HeatedArea_m2, deltaT, energyHours, isTotal: true));
            ThermalCharacteristicsRows.Add(CreateMetricRow("H_TOTAL", null, null, hTotal, snapshot.HeatedArea_m2, deltaT, energyHours, isTotal: true, isHighlighted: true));
        }

        private void RefreshSectionGainContributions()
        {
            double area = HeatedArea;
            if (area <= 0.0)
            {
                LightingGainPerArea = 0.0;
                AppliancesGainPerArea = 0.0;
                return;
            }

            double lightingGainKwh = 0.0;
            double appliancesGainKwh = 0.0;

            if (_internalGainsService != null)
            {
                var result = _internalGainsService.Recalculate(persist: false);
                if (result != null)
                {
                    lightingGainKwh = result.HeatingTable.Sum(m => m.L);
                    appliancesGainKwh = result.HeatingTable.Sum(m => m.A);
                }
            }
            else if (_section23Data != null)
            {
                lightingGainKwh = _section23Data.HeatingMonths.Sum(m => m.L_kWh);
                appliancesGainKwh = _section23Data.HeatingMonths.Sum(m => m.A_kWh);
            }

            LightingGainPerArea = lightingGainKwh / area;
            AppliancesGainPerArea = appliancesGainKwh / area;
        }

        private void RecalculateHeating(HeatingCharacteristicsSnapshot snapshot, double hTrTotal, double hVeTotal)
        {
            if (_objectData == null)
            {
                _lastAnnual = null;
                _lastMonthly = new List<HeatingMonthlyResult>();
                return;
            }

            if (IsAshraePlaceholder)
            {
                _lastAnnual = new HeatingAnnualResult
                {
                    IsValid = false,
                    ErrorMessage = "\u041C\u0435\u0442\u043E\u0434\u044A\u0442 ASHRAE 8760 \u0435 placeholder \u0438 \u043E\u0449\u0435 \u043D\u0435 \u0435 \u0438\u043C\u043F\u043B\u0435\u043C\u0435\u043D\u0442\u0438\u0440\u0430\u043D.",
                    Htr = hTrTotal,
                    Hve = hVeTotal,
                    Htotal = hTrTotal + hVeTotal,
                    Cm = _objectData.SpecificHeatCapacityWhPerM2K * snapshot.HeatedArea_m2
                };
                _lastMonthly = new List<HeatingMonthlyResult>();
                return;
            }

            double cm = Math.Max(0.0, _objectData.SpecificHeatCapacityWhPerM2K) * snapshot.HeatedArea_m2;
            double heatedArea = snapshot.HeatedArea_m2;
            var heatingMonths = GetHeatingMonthIndices(_objectData);
            var internalGainsResult = _internalGainsService?.Recalculate(persist: false);

            double GetQint(int monthIndex)
            {
                if (internalGainsResult == null || monthIndex < 0 || monthIndex >= internalGainsResult.HeatingTable.Length)
                {
                    return 0.0;
                }

                return internalGainsResult.HeatingTable[monthIndex].Total;
            }

            var (monthly, annual) = _calcService.Calculate(
                _data.CalculationMethod,
                BuildingElementExtractor.ExtractWalls(_wallsData),
                BuildingElementExtractor.ExtractWindows(_windowsData),
                BuildingElementExtractor.ExtractRoofs(_roofData),
                hTrTotal,
                hVeTotal,
                cm,
                _data.DesignTemperature,
                heatedArea,
                _objectData.ClimateZone,
                heatingMonths,
                GetQint);

            _lastAnnual = annual;
            _lastMonthly = monthly;
        }

        private static List<int> GetHeatingMonthIndices(ObjectDataSectionData objectData)
        {
            var climateData = new ClimateService(new JsonClimateRepository()).GetZone(objectData.ClimateZone);
            var months = new List<int>();
            for (int month = 1; month <= 12; month++)
            {
                if (ScheduleHelper.GetHeatingSeasonDaysInMonth(global::EE.Doklad.CalendarDefaults.ReferenceYear, month, climateData) > 0)
                {
                    months.Add(month - 1);
                }
            }

            return months;
        }

        private double CalculateVentilationHeatingContributionPerArea()
        {
            if (_ventilationHeatingData == null || _objectData == null)
            {
                return 0.0;
            }

            var climateService = new ClimateService(new JsonClimateRepository());
            if (!climateService.TryGetZone(_objectData.ClimateZone, out var climateData) || climateData == null)
            {
                return 0.0;
            }

            _ventilationHeatingData.HeatedArea_m2 = HeatedArea;

            int[] monthlyDaysOff =
            {
                ParseMonthlyDaysOff(_objectData.DaysOffJanuary),
                ParseMonthlyDaysOff(_objectData.DaysOffFebruary),
                ParseMonthlyDaysOff(_objectData.DaysOffMarch),
                ParseMonthlyDaysOff(_objectData.DaysOffApril),
                ParseMonthlyDaysOff(_objectData.DaysOffMay),
                ParseMonthlyDaysOff(_objectData.DaysOffJune),
                ParseMonthlyDaysOff(_objectData.DaysOffJuly),
                ParseMonthlyDaysOff(_objectData.DaysOffAugust),
                ParseMonthlyDaysOff(_objectData.DaysOffSeptember),
                ParseMonthlyDaysOff(_objectData.DaysOffOctober),
                ParseMonthlyDaysOff(_objectData.DaysOffNovember),
                ParseMonthlyDaysOff(_objectData.DaysOffDecember)
            };

            var calculator = new BgVentilationCalculator();
            var result = calculator.Calculate(_ventilationHeatingData, climateData, monthlyDaysOff);

            return result?.VentilationHeatingNetContribution_kWh_m2a ?? 0.0;
        }

        private static int ParseMonthlyDaysOff(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return 0;
            }

            return int.TryParse(value.Trim(), out int parsed)
                ? Math.Max(0, parsed)
                : 0;
        }

        private static HeatingCharacteristicRow CreateMetricRow(
            string parameter,
            double? u,
            double? area,
            double h,
            double heatedArea,
            double deltaT,
            double operatingHours,
            bool isTotal = false,
            bool isHighlighted = false,
            string uUnit = "W/m\u00B2K",
            string aUnit = "m\u00B2")
        {
            double kwh = h * deltaT * operatingHours / 1000.0;
            return new HeatingCharacteristicRow
            {
                Parameter = parameter,
                UDisplay = u.HasValue ? u.Value.ToString("F3", CultureInfo.InvariantCulture) : "-",
                ADisplay = area.HasValue ? area.Value.ToString("F2", CultureInfo.InvariantCulture) : "-",
                HDisplay = h.ToString("F2", CultureInfo.InvariantCulture),
                KwhPerAreaDisplay = heatedArea > 0 ? (kwh / heatedArea).ToString("F2", CultureInfo.InvariantCulture) : "0.00",
                KwhDisplay = kwh.ToString("F2", CultureInfo.InvariantCulture),
                IsTotal = isTotal,
                IsHighlighted = isHighlighted,
                UToolTip = u.HasValue ? uUnit : null,
                AToolTip = area.HasValue ? aUnit : null
            };
        }

        private static double CalculateAnnualEquivalentHours(HeatingCharacteristicsSnapshot snapshot)
        {
            double annualEquivalentHours = 0.0;
            int monthCount = new[]
            {
                snapshot.MonthlyOutdoorTemps_C.Length,
                snapshot.MonthlyOperatingHours_h.Length,
                snapshot.MonthlySetbackHours_h.Length
            }.Min();

            for (int month = 0; month < monthCount; month++)
            {
                double te = snapshot.MonthlyOutdoorTemps_C[month];
                double fullDelta = Math.Max(0.0, snapshot.DesignIndoorTemp_C - te);
                double setbackDelta = Math.Max(0.0, snapshot.SetbackIndoorTemp_C - te);
                double fullHours = Math.Max(0.0, snapshot.MonthlyOperatingHours_h[month]);
                double setbackHours = Math.Max(0.0, snapshot.MonthlySetbackHours_h[month]);

                annualEquivalentHours += fullDelta * fullHours + setbackDelta * setbackHours;
            }

            return annualEquivalentHours;
        }

        private void NotifyGrossHeatingEnergyChanged()
        {
            OnPropertyChanged(nameof(NetHeatingEnergyNoGainsPerArea));
            OnPropertyChanged(nameof(NetHeatingEnergyNoGainsPerAreaDisplay));
            OnPropertyChanged(nameof(TimeConstant_h));
            OnPropertyChanged(nameof(UtilizationFactor));
            OnPropertyChanged(nameof(SolarGain_total_kWh));
            OnPropertyChanged(nameof(QH_per_m2));
            OnPropertyChanged(nameof(IsAshraePlaceholder));
            OnPropertyChanged(nameof(NetHeatingEnergyAfterGains));
            OnPropertyChanged(nameof(NetHeatingEnergyAfterGainsDisplay));
            OnPropertyChanged(nameof(VentilationGainPerArea));
            OnPropertyChanged(nameof(NetHeatingEnergyAfterGainsKwh));
            OnPropertyChanged(nameof(NetHeatingEnergyAfterGainsKwhDisplay));
            OnPropertyChanged(nameof(VentilationGainKwh));
            OnPropertyChanged(nameof(VentilationGainPerAreaDisplay));
            OnPropertyChanged(nameof(VentilationGainKwhDisplay));
            OnPropertyChanged(nameof(LightingGainPerArea));
            OnPropertyChanged(nameof(LightingGainKwh));
            OnPropertyChanged(nameof(LightingGainPerAreaDisplay));
            OnPropertyChanged(nameof(LightingGainKwhDisplay));
            OnPropertyChanged(nameof(AppliancesGainPerArea));
            OnPropertyChanged(nameof(AppliancesGainKwh));
            OnPropertyChanged(nameof(AppliancesGainPerAreaDisplay));
            OnPropertyChanged(nameof(AppliancesGainKwhDisplay));
            OnPropertyChanged(nameof(EnergySource1RequiredEnergyPerArea));
            OnPropertyChanged(nameof(EnergySource1RequiredEnergyPerAreaDisplay));
            OnPropertyChanged(nameof(EnergySource2RequiredEnergyPerArea));
            OnPropertyChanged(nameof(EnergySource2RequiredEnergyPerAreaDisplay));
            OnPropertyChanged(nameof(TotalGrossHeatingEnergyPerArea));
            OnPropertyChanged(nameof(TotalGrossHeatingEnergyPerAreaDisplay));
            OnPropertyChanged(nameof(OverallHeatGenerationEfficiencyPercent));
            OnPropertyChanged(nameof(OverallHeatGenerationEfficiencyDisplay));
            OnPropertyChanged(nameof(HasEnergySourceShareWarning));
        }

        private double GetTotalEnergySourceShare()
        {
            return UseSecondEnergySource
                ? EnergySource1Share + EnergySource2Share
                : EnergySource1Share;
        }

        private static double? CalculateRequiredEnergyForSource(
            double netEnergyPerArea,
            double sharePercent,
            double emissionEfficiency,
            double distributionEfficiency,
            double automaticControl,
            double energyManagement,
            double generationEfficiency)
        {
            if (netEnergyPerArea <= 0.0 || sharePercent <= 0.0)
            {
                return 0.0;
            }

            double etaSys =
                (emissionEfficiency / 100.0) *
                (distributionEfficiency / 100.0) *
                (automaticControl / 100.0) *
                (energyManagement / 100.0) *
                (generationEfficiency / 100.0);

            if (etaSys <= 0.0)
            {
                return null;
            }

            return netEnergyPerArea * (sharePercent / 100.0) / etaSys;
        }

        private static string FormatNullableDouble(double? value)
        {
            return value.HasValue
                ? value.Value.ToString("F2", CultureInfo.InvariantCulture)
                : "-";
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// пїЅпїЅпїЅпїЅпїЅ пїЅпїЅ dropdown пїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ
    /// </summary>
    public class ActivityLevelOption
    {
        public ActivityLevel Level { get; set; }
        public string DisplayName { get; set; } = string.Empty;
    }

    public class HeatingCharacteristicRow
    {
        public string Parameter { get; set; } = string.Empty;
        public string UDisplay { get; set; } = "-";
        public string ADisplay { get; set; } = "-";
        public string HDisplay { get; set; } = "0.00";
        public string KwhPerAreaDisplay { get; set; } = "0.00";
        public string KwhDisplay { get; set; } = "0.00";
        public bool IsTotal { get; set; }
        public bool IsHighlighted { get; set; }
        public bool IsSeparator { get; set; }
        public string? UToolTip { get; set; }
        public string? AToolTip { get; set; }

        public static HeatingCharacteristicRow Separator()
        {
            return new HeatingCharacteristicRow
            {
                IsSeparator = true,
                Parameter = string.Empty,
                UDisplay = string.Empty,
                ADisplay = string.Empty,
                HDisplay = string.Empty,
                KwhPerAreaDisplay = string.Empty,
                KwhDisplay = string.Empty
            };
        }
    }
}
