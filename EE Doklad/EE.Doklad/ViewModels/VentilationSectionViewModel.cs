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
        private readonly HeatingSectionData? _heatingData;
        private readonly ClimateZoneData? _climateData;
        private readonly BgVentilationCalculator _calculator;

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Указва дали отоплителният сезон е включен (контролира gating на секцията)
        /// </summary>
        public bool IsHeatingSeasonEnabled => _objectData?.HeatingSeasonEnabled ?? true;

        /// <summary>
        /// Информационен текст, когато отоплителният сезон не е избран
        /// </summary>
        public string HeatingSeasonWarning => IsHeatingSeasonEnabled ? string.Empty : "Не е избран отоплителен сезон.";

        // Calculation result
        private VentilationCalculationResult? _calculationResult;
    private bool _showDebug = false;
    private string _debugText = string.Empty;
    private bool _isAdjustingShares = false; // guard to avoid recursive share updates

        public VentilationSectionViewModel(
            VentilationSectionData data,
            ObjectDataSectionData? objectData = null,
            HeatingSectionData? heatingData = null,
            ClimateZoneData? climateData = null)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));
            _objectData = objectData;
            _heatingData = heatingData;
            _climateData = climateData;
            _calculator = new BgVentilationCalculator();

            // Subscribe to data changes
            _data.PropertyChanged += OnDataPropertyChanged;
            if (_objectData != null)
            {
                _objectData.PropertyChanged += OnObjectDataPropertyChanged;
            }
            if (_heatingData != null)
            {
                _heatingData.PropertyChanged += OnHeatingDataPropertyChanged;
            }

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

        /// <summary>
        /// Покажи временен debug панел (не се съхранява в модел)
        /// </summary>
        public bool ShowDebug
        {
            get => _showDebug;
            set
            {
                if (_showDebug != value)
                {
                    _showDebug = value;
                    OnPropertyChanged(nameof(ShowDebug));
                }
            }
        }

        /// <summary>
        /// Текст за debug изход (много редове)
        /// </summary>
        public string DebugText
        {
            get => _debugText;
            private set
            {
                if (_debugText != value)
                {
                    _debugText = value;
                    OnPropertyChanged(nameof(DebugText));
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
                    // Mark that the user explicitly provided a supply temperature so calculator uses it
                    _data.SupplyTemperatureIsUserDefined = true;
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

        // --- Energy carrier selector (uses section 18 carriers list)
        public System.Collections.Generic.IEnumerable<Models.EnergyCarrierInfo> EnergyCarriers => Models.EnergyCarrierInfo.All;

        public Models.EnergyCarrierCode? EnergySource1Carrier
        {
            get => _data.EnergySource1.EnergyCarrier;
            set
            {
                if (_data.EnergySource1.EnergyCarrier != value)
                {
                    _data.EnergySource1.EnergyCarrier = value;
                    OnPropertyChanged(nameof(EnergySource1Carrier));
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

        // Expose the missing EI2 fields (to mirror EI1)
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
                    Recalculate();
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
                    Recalculate();
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
                    Recalculate();
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
                    Recalculate();
                }
            }
        }

        // Energy carrier for source 2
        public Models.EnergyCarrierCode? EnergySource2Carrier
        {
            get => _data.EnergySource2?.EnergyCarrier;
            set
            {
                if (_data.EnergySource2 != null && _data.EnergySource2.EnergyCarrier != value)
                {
                    _data.EnergySource2.EnergyCarrier = value;
                    OnPropertyChanged(nameof(EnergySource2Carrier));
                    Recalculate();
                }
            }
        }

        // --- Read-only outputs for results table (per-m2 and absolute)
        public double AnnualVentilationHeatingEnergy_kWh => _calculationResult?.AnnualVentilationHeatingEnergy_kWh_a ?? 0;
        public double FinalEnergySource1_kWh => _calculationResult?.FinalEnergySource1_kWh_a ?? 0;
        public double FinalEnergySource2_kWh => _calculationResult?.FinalEnergySource2_kWh_a ?? 0;
        public double FinalEnergySource1_kWh_per_m2 => (HeatedArea_m2 > 0) ? (FinalEnergySource1_kWh / HeatedArea_m2) : 0;
        public double FinalEnergySource2_kWh_per_m2 => (HeatedArea_m2 > 0) ? (FinalEnergySource2_kWh / HeatedArea_m2) : 0;
        public double OverallHeatGenerationEfficiencyPercent
        {
            get
            {
                var finalTotal = _calculationResult?.TotalFinalEnergy_kWh_a ?? 0;
                var heating = _calculationResult?.AnnualVentilationHeatingEnergy_kWh_a ?? 0;
                if (finalTotal <= 0) return 0;
                return (heating / finalTotal) * 100.0;
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

        public double VentilationHeatingNetContribution_kWh =>
            _calculationResult?.VentilationHeatingNetContribution_kWh ?? 0;

        public double VentilationHeatingNetContribution_kWh_m2a =>
            _calculationResult?.VentilationHeatingNetContribution_kWh_m2a ?? 0;

        public double TotalFinalEnergy_kWh_a =>
            _calculationResult?.TotalFinalEnergy_kWh_a ?? 0;

        public double SpecificFinalEnergy_kWh_m2a =>
            _calculationResult?.SpecificFinalEnergy_kWh_m2a ?? 0;

        public string ErrorMessage =>
            _calculationResult?.IsValid == false ? _calculationResult.ErrorMessage ?? string.Empty : string.Empty;

        // ========== CALCULATION ==========

        private void Recalculate()
        {
            // If object data contains a ventilation schedule, compute weekly operating hours automatically
            if (_objectData != null)
            {
                double ParseDay(string? s)
                {
                    if (string.IsNullOrWhiteSpace(s)) return 0.0;
                    if (double.TryParse(s.Trim(), System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double v)) return v;
                    if (double.TryParse(s.Trim(), out v)) return v;
                    return 0.0;
                }

                var workdayHours = ParseDay(_objectData.VentilationWorkdaysHours);
                var sat = ParseDay(_objectData.VentilationSaturdayHours);
                var sun = ParseDay(_objectData.VentilationSundayHours);

                // weekly = workdayHours * 5 + saturday + sunday
                var weekly = workdayHours * 5.0 + sat + sun;
                weekly = Math.Clamp(weekly, 0.0, 168.0);
                if (_data.OperatingHoursPerWeek != weekly)
                {
                    _data.OperatingHoursPerWeek = weekly;
                    OnPropertyChanged(nameof(OperatingHoursPerWeek));
                }
            }

            // Update heated area in data model
            _data.HeatedArea_m2 = HeatedArea_m2;

            // Prepare monthly days-off array (from section 5). If object data is not
            // available, pass null. Values are counts per calendar month (Jan..Dec).
            int[]? monthlyDaysOff = null;
            if (_objectData != null)
            {
                monthlyDaysOff = new int[12];
                int ParseInt(string? s)
                {
                    if (string.IsNullOrWhiteSpace(s)) return 0;
                    if (int.TryParse(s.Trim(), out int v)) return Math.Max(0, v);
                    return 0;
                }

                monthlyDaysOff[0] = ParseInt(_objectData.DaysOffJanuary);
                monthlyDaysOff[1] = ParseInt(_objectData.DaysOffFebruary);
                monthlyDaysOff[2] = ParseInt(_objectData.DaysOffMarch);
                monthlyDaysOff[3] = ParseInt(_objectData.DaysOffApril);
                monthlyDaysOff[4] = ParseInt(_objectData.DaysOffMay);
                monthlyDaysOff[5] = ParseInt(_objectData.DaysOffJune);
                monthlyDaysOff[6] = ParseInt(_objectData.DaysOffJuly);
                monthlyDaysOff[7] = ParseInt(_objectData.DaysOffAugust);
                monthlyDaysOff[8] = ParseInt(_objectData.DaysOffSeptember);
                monthlyDaysOff[9] = ParseInt(_objectData.DaysOffOctober);
                monthlyDaysOff[10] = ParseInt(_objectData.DaysOffNovember);
                monthlyDaysOff[11] = ParseInt(_objectData.DaysOffDecember);
            }

            // Perform calculation (pass monthly days-off so holidays are excluded from heating-season hours)
            _calculationResult = _calculator.Calculate(_data, _climateData, monthlyDaysOff);

            // Populate debug text for UI if needed
            DebugText = _calculationResult != null ? BuildDebugText(_calculationResult) : string.Empty;

            // Notify all output properties
            OnPropertyChanged(nameof(VentilationLossCoefficient_WK));
            OnPropertyChanged(nameof(AnnualVentilationHeatingEnergy_kWh_a));
            OnPropertyChanged(nameof(SpecificVentilationHeatingEnergy_kWh_m2a));
            OnPropertyChanged(nameof(VentilationHeatingNetContribution_kWh));
            OnPropertyChanged(nameof(VentilationHeatingNetContribution_kWh_m2a));
            OnPropertyChanged(nameof(TotalFinalEnergy_kWh_a));
            OnPropertyChanged(nameof(SpecificFinalEnergy_kWh_m2a));
            OnPropertyChanged(nameof(ErrorMessage));

            // New outputs
            OnPropertyChanged(nameof(AnnualVentilationHeatingEnergy_kWh));
            OnPropertyChanged(nameof(FinalEnergySource1_kWh));
            OnPropertyChanged(nameof(FinalEnergySource2_kWh));
            OnPropertyChanged(nameof(FinalEnergySource1_kWh_per_m2));
            OnPropertyChanged(nameof(FinalEnergySource2_kWh_per_m2));
            OnPropertyChanged(nameof(OverallHeatGenerationEfficiencyPercent));

            // Notify carrier bindings
            OnPropertyChanged(nameof(EnergySource1Carrier));
            OnPropertyChanged(nameof(EnergySource2Carrier));
        }

        private void OnDataPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // Recalculate when data changes
            Recalculate();
        }

        private string BuildDebugText(VentilationCalculationResult result)
        {
            if (result == null) return string.Empty;

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("--- Debug: секция 12 - Вентилация (временно) ---");
            sb.AppendLine($"Климатична зона: {_climateData?.Name ?? "(не е избрана)"}");
            sb.AppendLine($"Отопляема площ (m2): {result.HeatedArea_m2:F2}");
            sb.AppendLine($"Дебит (m3/h на m2): {result.AirflowRatePerM2:F3}");
            double totalAirflow = result.AirflowRatePerM2 * result.HeatedArea_m2;
            sb.AppendLine($"Общ въздуховод V̇ (m3/h): {totalAirflow:F3}");
            sb.AppendLine($"Часове на седмица: {result.OperatingHoursPerWeek:F2}");
            sb.AppendLine(" ");
            sb.AppendLine("Месец | Te [°C] | Ti [°C] | Tsup [°C] | Hve [W/K] | bVe [-] | t_m [h] | Q_airheat_m [kWh] | Q_contrib_m [kWh]");

            // Alternative rho·c (Wh/(m3·K)) to compare with other software (e.g. 0.34)
            const double AlternativeRhoCp_Wh_per_m3K = 0.34;
            double totalQcontrib = 0.0;
            double totalQcontribAlt = 0.0;

            foreach (var m in result.MonthlyResults)
            {
                double qVe = m.VentilationHeatingEnergy_kWh;
                double hours_m = m.MonthlyOperatingTime_h;
                double tsup = m.SupplyTemperature_C;
                double ti = m.IndoorTemperature_C;
                double te = m.OutdoorTemperature_C;
                double hve = m.VentilationLossCoefficient_WK;
                double bve = m.TemperatureControlCoefficient;

                double qContrib = 0.0;
                double qContribAlt = 0.0;
                if (hours_m > 0)
                {
                    // use Hve [W/K] × (Tsup - Ti) × t_m / 1000 to obtain kWh
                    qContrib = hve * (tsup - ti) * hours_m / 1000.0;

                    // Alternative: compute hVe from alternative rho*c (Wh/(m3·K))
                    double hVeAlt = AlternativeRhoCp_Wh_per_m3K * totalAirflow;
                    qContribAlt = hVeAlt * (tsup - ti) * hours_m / 1000.0;
                }
                totalQcontrib += qContrib;
                totalQcontribAlt += qContribAlt;

                sb.AppendLine($"{m.MonthName.PadRight(7)} | {te,6:0.0} | {ti,5:0.0} | {tsup,6:0.0} | {hve,8:0.0} | {bve,5:0.00} | {hours_m,6:0.0} | {qVe,10:0.00} | {qContrib,10:0.00} | {qContribAlt,10:0.00}");
            }

            sb.AppendLine(" ");
            sb.AppendLine($"Сумиран нетен принос (изчислен по месечни) = {totalQcontrib:F2} kWh");
            sb.AppendLine($"Сумиран нетен принос (алтернативен rho·c={AlternativeRhoCp_Wh_per_m3K:F2}) = {totalQcontribAlt:F2} kWh");
            sb.AppendLine($"Резултат: VentilationHeatingNetContribution_kWh = {result.VentilationHeatingNetContribution_kWh:F2} kWh");
            sb.AppendLine("--- Край на debug ---");

            return sb.ToString();
        }

        private void OnObjectDataPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // Recalculate when relevant object-data schedule fields change
            if (e.PropertyName == nameof(ObjectDataSectionData.VentilationWorkdaysHours) ||
                e.PropertyName == nameof(ObjectDataSectionData.VentilationSaturdayHours) ||
                e.PropertyName == nameof(ObjectDataSectionData.VentilationSundayHours))
            {
                Recalculate();
            }
            else if (e.PropertyName == nameof(ObjectDataSectionData.HeatingSeasonEnabled))
            {
                OnPropertyChanged(nameof(IsHeatingSeasonEnabled));
                OnPropertyChanged(nameof(HeatingSeasonWarning));
            }
        }

        private void OnHeatingDataPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(HeatingSectionData.DesignTemperature))
            {
                _data.IndoorTemperature_C = _heatingData?.DesignTemperature ?? _data.IndoorTemperature_C;
                OnPropertyChanged(nameof(IndoorTemperature_C));
                Recalculate();
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
