using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using EE.Doklad.Models;

namespace EE.Doklad.ViewModels
{
    /// <summary>
    /// ViewModel за Секция 15: Помпи и вентилатори
    /// Изчислява реална годишна електрическа консумация [kWh] и специфична мощност [W/m²]
    /// на база автоматично изчислени часове на работа от графиците в т.5
    /// </summary>
    public class PumpsAndFansSectionViewModel : INotifyPropertyChanged
    {
        private readonly PumpsAndFansSectionData _data;
        private readonly ObjectDataSectionData? _objectData;

        public event PropertyChangedEventHandler? PropertyChanged;

        // Public access to data for view interaction
        public PumpsAndFansSectionData Data => _data;

        // ========== CONSTRUCTOR ==========

        public PumpsAndFansSectionViewModel(PumpsAndFansSectionData data, ObjectDataSectionData? objectData = null)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data));
            _objectData = objectData;

            // Subscribe to data changes
            if (_objectData != null)
            {
                _objectData.PropertyChanged += OnObjectDataChanged;
            }

            // Subscribe to collection changes — 4 separate collections
            _data.HeatingPumpRows.CollectionChanged += (s, e) => { RecalculateHeating(); MaintainHeatingRowSubscriptions(); };
            _data.HeatingFanRows.CollectionChanged  += (s, e) => { RecalculateHeating(); MaintainHeatingRowSubscriptions(); };
            _data.CoolingPumpRows.CollectionChanged += (s, e) => { RecalculateCooling(); MaintainCoolingRowSubscriptions(); };
            _data.CoolingFanRows.CollectionChanged  += (s, e) => { RecalculateCooling(); MaintainCoolingRowSubscriptions(); };

            MaintainHeatingRowSubscriptions();
            MaintainCoolingRowSubscriptions();

            // Initial calculation
            RecalculateAll();
        }

        private void MaintainHeatingRowSubscriptions()
        {
            foreach (var row in _data.HeatingPumpRows)
            {
                row.PropertyChanged -= HeatingRow_PropertyChanged;
                row.PropertyChanged += HeatingRow_PropertyChanged;
            }
            foreach (var row in _data.HeatingFanRows)
            {
                row.PropertyChanged -= HeatingRow_PropertyChanged;
                row.PropertyChanged += HeatingRow_PropertyChanged;
            }
        }

        private void HeatingRow_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            RecalculateHeating();
        }

        private void MaintainCoolingRowSubscriptions()
        {
            foreach (var row in _data.CoolingPumpRows)
            {
                row.PropertyChanged -= CoolingRow_PropertyChanged;
                row.PropertyChanged += CoolingRow_PropertyChanged;
            }
            foreach (var row in _data.CoolingFanRows)
            {
                row.PropertyChanged -= CoolingRow_PropertyChanged;
                row.PropertyChanged += CoolingRow_PropertyChanged;
            }
        }

        private void CoolingRow_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            RecalculateCooling();
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

        // ========== 15.1 ОТОПЛЕНИЕ ==========

        public ObservableCollection<PumpFanHeatingRow> HeatingPumpRows => _data.HeatingPumpRows;
        public ObservableCollection<PumpFanHeatingRow> HeatingFanRows => _data.HeatingFanRows;

        public string HeatingEM
        {
            get => _data.HeatingEM ?? "0.96";
            set
            {
                if (_data.HeatingEM != value)
                {
                    _data.HeatingEM = value;
                    OnPropertyChanged(nameof(HeatingEM));
                    RecalculateHeating();
                }
            }
        }

        public double HeatingTotalAnnualConsumption => _data.HeatingTotalAnnualConsumption;
        public double HeatingSpecificPower => _data.HeatingSpecificPower;
        public double HeatingAnnualHours => _data.HeatingAnnualHours;

        // 15.1.1 / 15.1.2 subgroup totals
        public double HeatingPumpsTotalAnnualConsumption => _data.HeatingPumpsTotalAnnualConsumption;
        public double HeatingPumpsSpecificPower => _data.HeatingPumpsSpecificPower;
        public double HeatingFansTotalAnnualConsumption => _data.HeatingFansTotalAnnualConsumption;
        public double HeatingFansSpecificPower => _data.HeatingFansSpecificPower;

        // ========== 15.2 ОХЛАЖДАНЕ ==========

        public ObservableCollection<PumpFanCoolingRow> CoolingPumpRows => _data.CoolingPumpRows;
        public ObservableCollection<PumpFanCoolingRow> CoolingFanRows => _data.CoolingFanRows;

        public string CoolingEM
        {
            get => _data.CoolingEM ?? "0.96";
            set
            {
                if (_data.CoolingEM != value)
                {
                    _data.CoolingEM = value;
                    OnPropertyChanged(nameof(CoolingEM));
                    RecalculateCooling();
                }
            }
        }

        public double CoolingTotalAnnualConsumption => _data.CoolingTotalAnnualConsumption;
        public double CoolingSpecificPower => _data.CoolingSpecificPower;
        public double CoolingAnnualHours => _data.CoolingAnnualHours;

        // 15.2.1 / 15.2.2 subgroup totals
        public double CoolingPumpsTotalAnnualConsumption => _data.CoolingPumpsTotalAnnualConsumption;
        public double CoolingPumpsSpecificPower => _data.CoolingPumpsSpecificPower;
        public double CoolingFansTotalAnnualConsumption => _data.CoolingFansTotalAnnualConsumption;
        public double CoolingFansSpecificPower => _data.CoolingFansSpecificPower;

        // ========== 15.3 БГВ ==========

        public string DhwPumpNominalPower
        {
            get => _data.DhwPumpNominalPower ?? string.Empty;
            set
            {
                if (_data.DhwPumpNominalPower != value)
                {
                    _data.DhwPumpNominalPower = value;
                    OnPropertyChanged(nameof(DhwPumpNominalPower));
                    RecalculateDhw();
                }
            }
        }

        public string DhwPumpQuantity
        {
            get => _data.DhwPumpQuantity ?? "1";
            set
            {
                if (_data.DhwPumpQuantity != value)
                {
                    _data.DhwPumpQuantity = value;
                    OnPropertyChanged(nameof(DhwPumpQuantity));
                    RecalculateDhw();
                }
            }
        }

        public string DhwPumpHoursPerDay
        {
            get => _data.DhwPumpHoursPerDay ?? string.Empty;
            set
            {
                if (_data.DhwPumpHoursPerDay != value)
                {
                    _data.DhwPumpHoursPerDay = value;
                    OnPropertyChanged(nameof(DhwPumpHoursPerDay));
                    RecalculateDhw();
                }
            }
        }

        public string DhwPumpMode
        {
            get => _data.DhwPumpMode ?? string.Empty;
            set
            {
                if (_data.DhwPumpMode != value)
                {
                    _data.DhwPumpMode = value;
                    OnPropertyChanged(nameof(DhwPumpMode));
                }
            }
        }

        public string DhwEM
        {
            get => _data.DhwEM ?? "0.96";
            set
            {
                if (_data.DhwEM != value)
                {
                    _data.DhwEM = value;
                    OnPropertyChanged(nameof(DhwEM));
                    RecalculateDhw();
                }
            }
        }

        public double DhwTotalAnnualConsumption => _data.DhwTotalAnnualConsumption;
        public double DhwSpecificPower => _data.DhwSpecificPower;
        public double DhwAnnualHours => _data.DhwAnnualHours;

        // ========== ОБЩО ==========

        public double TotalAnnualConsumption => _data.TotalAnnualConsumption;
        public double TotalSpecificPower => _data.TotalSpecificPower;

        public string GeneratedReportText => _data.GeneratedReportText;

        // ========== HELPER METHODS ==========

        private void OnObjectDataChanged(object? sender, PropertyChangedEventArgs e)
        {
            // Recalculate when relevant object data changes
            var relevantProps = new[]
            {
                nameof(ObjectDataSectionData.ClimateZone),
                nameof(ObjectDataSectionData.HeatingWorkdaysHours),
                nameof(ObjectDataSectionData.HeatingSaturdayHours),
                nameof(ObjectDataSectionData.HeatingSundayHours),
                nameof(ObjectDataSectionData.CoolingWorkdaysHours),
                nameof(ObjectDataSectionData.CoolingSaturdayHours),
                nameof(ObjectDataSectionData.CoolingSundayHours),
                nameof(ObjectDataSectionData.HeatedArea),
                nameof(ObjectDataSectionData.DaysOffJanuary),
                nameof(ObjectDataSectionData.DaysOffFebruary),
                nameof(ObjectDataSectionData.DaysOffMarch),
                nameof(ObjectDataSectionData.DaysOffApril),
                nameof(ObjectDataSectionData.DaysOffMay),
                nameof(ObjectDataSectionData.DaysOffJune),
                nameof(ObjectDataSectionData.DaysOffJuly),
                nameof(ObjectDataSectionData.DaysOffAugust),
                nameof(ObjectDataSectionData.DaysOffSeptember),
                nameof(ObjectDataSectionData.DaysOffOctober),
                nameof(ObjectDataSectionData.DaysOffNovember),
                nameof(ObjectDataSectionData.DaysOffDecember)
            };

            if (relevantProps.Contains(e.PropertyName))
            {
                RecalculateAll();
            }
        }

        private void RecalculateAll()
        {
            RecalculateHeating();
            RecalculateCooling();
            RecalculateDhw();
            RecalculateTotal();
        }

        // ========== 15.1 ОТОПЛЕНИЕ CALCULATIONS ==========

        private void RecalculateHeating()
        {
            if (_objectData == null)
            {
                _data.HeatingAnnualHours = 0;
                _data.HeatingTotalAnnualConsumption = 0;
                _data.HeatingSpecificPower = 0;
                NotifyHeatingPropertiesChanged();
                return;
            }

            var heatingHours = CalculateHeatingSeasonHours();
            _data.HeatingAnnualHours = heatingHours;

            var ventilationHours = CalculateVentilationAnnualHours();

            double pumpsConsumption = 0;
            double fansConsumption = 0;

            // Помпи — работят по отоплителния сезон
            foreach (var row in _data.HeatingPumpRows)
            {
                row.AnnualHours = heatingHours;
                if (TryParseDouble(row.NominalPower, out double power) &&
                    TryParseDouble(row.Quantity, out double qty))
                {
                    row.AnnualConsumption = (power * qty * row.AnnualHours) / 1000.0;
                    pumpsConsumption += row.AnnualConsumption;
                }
                else
                {
                    row.AnnualConsumption = 0;
                }
            }

            // Вентилатори — работят по вентилационния график
            foreach (var row in _data.HeatingFanRows)
            {
                row.AnnualHours = ventilationHours;
                if (TryParseDouble(row.NominalPower, out double power) &&
                    TryParseDouble(row.Quantity, out double qty))
                {
                    row.AnnualConsumption = (power * qty * row.AnnualHours) / 1000.0;
                    fansConsumption += row.AnnualConsumption;
                }
                else
                {
                    row.AnnualConsumption = 0;
                }
            }

            // Store totals
            _data.HeatingPumpsTotalAnnualConsumption = pumpsConsumption;
            _data.HeatingFansTotalAnnualConsumption = fansConsumption;

            _data.HeatingTotalAnnualConsumption = pumpsConsumption + fansConsumption;

            // Calculate specific power [W/m²] per subgroup and overall
            _data.HeatingPumpsSpecificPower = CalculateSpecificPower(pumpsConsumption, HeatingEM);
            _data.HeatingFansSpecificPower = CalculateSpecificPower(fansConsumption, HeatingEM);
            _data.HeatingSpecificPower = CalculateSpecificPower(_data.HeatingTotalAnnualConsumption, HeatingEM);

            NotifyHeatingPropertiesChanged();
        }

        private double CalculateHeatingSeasonHours()
        {
            if (_objectData == null) return 0;

            // Get heating season start and end dates
            var (startMonth, startDay, endMonth, endDay) = GetHeatingSeasonDates(_objectData.ClimateZone);

            // Get schedules
            var workdaysHours = ParseDouble(_objectData.HeatingWorkdaysHours);
            var saturdayHours = ParseDouble(_objectData.HeatingSaturdayHours);
            var sundayHours = ParseDouble(_objectData.HeatingSundayHours);

            // Calculate hours for each month in heating season
            double totalHours = 0;

            for (int month = 1; month <= 12; month++)
            {
                if (!IsMonthInHeatingSeason(month, startMonth, endMonth))
                    continue;

                var daysInMonth = DateTime.DaysInMonth(2024, month); // Using 2024 (leap year)
                var daysOff = GetDaysOffForMonth(month);

                // Calculate active days for heating this month
                int activeDays = daysInMonth - daysOff;

                // Calculate heating hours based on day types
                var monthHours = CalculateMonthlyHours(
                    month, startDay, endDay, startMonth, endMonth,
                    activeDays, workdaysHours, saturdayHours, sundayHours);

                totalHours += monthHours;
            }

            return totalHours;
        }

        private (int startMonth, int startDay, int endMonth, int endDay) GetHeatingSeasonDates(int climateZone)
        {
            return climateZone switch
            {
                1 => (10, 21, 4, 20),  // 21 окт - 20 апр
                2 => (10, 21, 4, 25),  // 21 окт - 25 апр
                3 => (10, 23, 4, 15),  // 23 окт - 15 апр
                4 => (10, 16, 4, 23),  // 16 окт - 23 апр
                5 => (10, 25, 4, 19),  // 25 окт - 19 апр
                6 => (10, 24, 4, 6),   // 24 окт - 6 апр
                7 => (10, 15, 4, 23),  // 15 окт - 23 апр
                8 => (10, 28, 4, 6),   // 28 окт - 6 апр
                9 => (10, 28, 4, 5),   // 28 окт - 5 апр
                _ => (10, 15, 4, 15)   // Default
            };
        }

        private bool IsMonthInHeatingSeason(int month, int startMonth, int endMonth)
        {
            // Heating season wraps around year end
            if (startMonth > endMonth)
            {
                return month >= startMonth || month <= endMonth;
            }
            return month >= startMonth && month <= endMonth;
        }

        private double CalculateMonthlyHours(
            int month, int startDay, int endDay, int startMonth, int endMonth,
            int activeDays, double workdaysHours, double saturdayHours, double sundayHours)
        {
            // Simplified calculation: distribute hours evenly across active days
            // In reality, we should consider weekday distribution

            double avgHoursPerDay = 0;
            int scheduleCount = 0;

            if (workdaysHours > 0)
            {
                avgHoursPerDay += workdaysHours * 5; // 5 workdays per week
                scheduleCount += 5;
            }
            if (saturdayHours > 0)
            {
                avgHoursPerDay += saturdayHours;
                scheduleCount += 1;
            }
            if (sundayHours > 0)
            {
                avgHoursPerDay += sundayHours;
                scheduleCount += 1;
            }

            if (scheduleCount == 0) return 0;

            avgHoursPerDay /= 7.0; // Average per day in week

            // Adjust for partial months
            double daysInMonth = DateTime.DaysInMonth(2024, month);
            double effectiveDays = activeDays;

            if (month == startMonth && startMonth == endMonth)
            {
                // Both start and end in same month
                effectiveDays = Math.Max(0, endDay - startDay + 1 - GetDaysOffForMonth(month));
            }
            else if (month == startMonth)
            {
                effectiveDays = Math.Max(0, daysInMonth - startDay + 1 - GetDaysOffForMonth(month));
            }
            else if (month == endMonth)
            {
                effectiveDays = Math.Max(0, endDay - GetDaysOffForMonth(month));
            }

            return effectiveDays * avgHoursPerDay;
        }

        private int GetDaysOffForMonth(int month)
        {
            if (_objectData == null) return 0;

            return month switch
            {
                1 => ParseInt(_objectData.DaysOffJanuary),
                2 => ParseInt(_objectData.DaysOffFebruary),
                3 => ParseInt(_objectData.DaysOffMarch),
                4 => ParseInt(_objectData.DaysOffApril),
                5 => ParseInt(_objectData.DaysOffMay),
                6 => ParseInt(_objectData.DaysOffJune),
                7 => ParseInt(_objectData.DaysOffJuly),
                8 => ParseInt(_objectData.DaysOffAugust),
                9 => ParseInt(_objectData.DaysOffSeptember),
                10 => ParseInt(_objectData.DaysOffOctober),
                11 => ParseInt(_objectData.DaysOffNovember),
                12 => ParseInt(_objectData.DaysOffDecember),
                _ => 0
            };
        }

        // ========== 15.2 ОХЛАЖДАНЕ CALCULATIONS ==========

        private void RecalculateCooling()
        {
            if (_objectData == null)
            {
                _data.CoolingAnnualHours = 0;
                _data.CoolingTotalAnnualConsumption = 0;
                _data.CoolingSpecificPower = 0;
                NotifyCoolingPropertiesChanged();
                return;
            }

            var coolingHours = CalculateCoolingSeasonHours();
            _data.CoolingAnnualHours = coolingHours;

            var ventilationHours = CalculateVentilationAnnualHours();

            double pumpsConsumption = 0;
            double fansConsumption = 0;

            // Помпи — работят по охладителния сезон
            foreach (var row in _data.CoolingPumpRows)
            {
                row.AnnualHours = coolingHours;
                if (TryParseDouble(row.NominalPower, out double power) &&
                    TryParseDouble(row.Quantity, out double qty))
                {
                    row.AnnualConsumption = (power * qty * row.AnnualHours) / 1000.0;
                    pumpsConsumption += row.AnnualConsumption;
                }
                else
                {
                    row.AnnualConsumption = 0;
                }
            }

            // Вентилатори — работят по вентилационния график
            foreach (var row in _data.CoolingFanRows)
            {
                row.AnnualHours = ventilationHours;
                if (TryParseDouble(row.NominalPower, out double power) &&
                    TryParseDouble(row.Quantity, out double qty))
                {
                    row.AnnualConsumption = (power * qty * row.AnnualHours) / 1000.0;
                    fansConsumption += row.AnnualConsumption;
                }
                else
                {
                    row.AnnualConsumption = 0;
                }
            }

            // Store totals
            _data.CoolingPumpsTotalAnnualConsumption = pumpsConsumption;
            _data.CoolingFansTotalAnnualConsumption = fansConsumption;

            _data.CoolingTotalAnnualConsumption = pumpsConsumption + fansConsumption;

            // Calculate specific power [W/m²] per subgroup and overall
            _data.CoolingPumpsSpecificPower = CalculateSpecificPower(pumpsConsumption, CoolingEM);
            _data.CoolingFansSpecificPower = CalculateSpecificPower(fansConsumption, CoolingEM);
            _data.CoolingSpecificPower = CalculateSpecificPower(_data.CoolingTotalAnnualConsumption, CoolingEM);

            NotifyCoolingPropertiesChanged();
        }

        private double CalculateCoolingSeasonHours()
        {
            if (_objectData == null) return 0;

            // Fixed cooling period: 1 May - 30 September
            var workdaysHours = ParseDouble(_objectData.CoolingWorkdaysHours);
            var saturdayHours = ParseDouble(_objectData.CoolingSaturdayHours);
            var sundayHours = ParseDouble(_objectData.CoolingSundayHours);

            // If no cooling schedule, return 0
            if (workdaysHours == 0 && saturdayHours == 0 && sundayHours == 0)
                return 0;

            double totalHours = 0;

            // Months: May (5), June (6), July (7), August (8), September (9)
            for (int month = 5; month <= 9; month++)
            {
                var daysInMonth = DateTime.DaysInMonth(2024, month);
                var daysOff = GetDaysOffForMonth(month);
                int activeDays = daysInMonth - daysOff;

                // Calculate average hours per day
                double avgHoursPerDay = 0;
                if (workdaysHours > 0) avgHoursPerDay += workdaysHours * 5;
                if (saturdayHours > 0) avgHoursPerDay += saturdayHours;
                if (sundayHours > 0) avgHoursPerDay += sundayHours;
                avgHoursPerDay /= 7.0;

                totalHours += activeDays * avgHoursPerDay;
            }

            return totalHours;
        }

        private double CalculateVentilationAnnualHours()
        {
            if (_objectData == null) return 0;

            var workdaysHours = ParseDouble(_objectData.VentilationWorkdaysHours);
            var saturdayHours = ParseDouble(_objectData.VentilationSaturdayHours);
            var sundayHours = ParseDouble(_objectData.VentilationSundayHours);

            if (workdaysHours == 0 && saturdayHours == 0 && sundayHours == 0)
                return 0;

            double totalHours = 0;

            for (int month = 1; month <= 12; month++)
            {
                var daysInMonth = DateTime.DaysInMonth(2024, month);
                var daysOff = GetDaysOffForMonth(month);
                int activeDays = daysInMonth - daysOff;

                // Calculate average hours per day
                double avgHoursPerDay = 0;
                if (workdaysHours > 0) avgHoursPerDay += workdaysHours * 5;
                if (saturdayHours > 0) avgHoursPerDay += saturdayHours;
                if (sundayHours > 0) avgHoursPerDay += sundayHours;
                avgHoursPerDay /= 7.0;

                totalHours += activeDays * avgHoursPerDay;
            }

            return totalHours;
        }

        // ========== 15.3 БГВ CALCULATIONS ==========

        private void RecalculateDhw()
        {
            if (_objectData == null)
            {
                _data.DhwAnnualHours = 0;
                _data.DhwTotalAnnualConsumption = 0;
                _data.DhwSpecificPower = 0;
                NotifyDhwPropertiesChanged();
                return;
            }

            // Calculate annual hours based on manual hours/day input
            var hoursPerDay = ParseDouble(_data.DhwPumpHoursPerDay);
            var annualDays = CalculateDhwAnnualDays();
            var annualHours = hoursPerDay * annualDays;

            _data.DhwAnnualHours = annualHours;

            // Calculate consumption
            if (TryParseDouble(_data.DhwPumpNominalPower, out double power) &&
                TryParseDouble(_data.DhwPumpQuantity, out double qty))
            {
                // kWh = (W × Брой × Годишни часове) / 1000
                _data.DhwTotalAnnualConsumption = (power * qty * annualHours) / 1000.0;
            }
            else
            {
                _data.DhwTotalAnnualConsumption = 0;
            }

            // Calculate specific power [W/m²]
            _data.DhwSpecificPower = CalculateSpecificPower(_data.DhwTotalAnnualConsumption, DhwEM);

            NotifyDhwPropertiesChanged();
        }

        private double CalculateDhwAnnualDays()
        {
            if (_objectData == null) return 365;

            // Use schedule from ObjectData to determine active days
            // DHW pump runs when there's ANY schedule (heating, cooling, or occupancy)
            // For simplicity, use heating schedule as baseline

            var workdaysHours = ParseDouble(_objectData.HeatingWorkdaysHours);
            var saturdayHours = ParseDouble(_objectData.HeatingSaturdayHours);
            var sundayHours = ParseDouble(_objectData.HeatingSundayHours);

            // Count active day types
            int activeDayTypes = 0;
            double daysPerWeek = 0;

            if (workdaysHours > 0)
            {
                activeDayTypes++;
                daysPerWeek += 5; // Workdays
            }
            if (saturdayHours > 0)
            {
                activeDayTypes++;
                daysPerWeek += 1; // Saturday
            }
            if (sundayHours > 0)
            {
                activeDayTypes++;
                daysPerWeek += 1; // Sunday
            }

            if (activeDayTypes == 0) return 0; // No active schedule

            // Calculate annual days minus days off
            var totalDaysOff = _objectData.MonthlyDaysOffSum;
            var annualDays = 365.0 - totalDaysOff;

            // Adjust based on active day types ratio
            annualDays = annualDays * (daysPerWeek / 7.0);

            return Math.Max(0, annualDays);
        }

        // ========== TOTAL CALCULATIONS ==========

        private void RecalculateTotal()
        {
            _data.TotalAnnualConsumption =
                _data.HeatingTotalAnnualConsumption +
                _data.CoolingTotalAnnualConsumption +
                _data.DhwTotalAnnualConsumption;

            _data.TotalSpecificPower =
                _data.HeatingSpecificPower +
                _data.CoolingSpecificPower +
                _data.DhwSpecificPower;

            OnPropertyChanged(nameof(TotalAnnualConsumption));
            OnPropertyChanged(nameof(TotalSpecificPower));
            OnPropertyChanged(nameof(GeneratedReportText));
        }

        // ========== SPECIFIC POWER CALCULATION ==========

        private double CalculateSpecificPower(double annualConsumptionKwh, string emString)
        {
            if (_objectData == null) return 0;

            // Parse heated area
            if (!TryParseDouble(_objectData.HeatedArea, out double heatedArea) || heatedArea <= 0)
                return 0;

            // Parse EM coefficient
            var em = ParseDouble(emString);
            if (em <= 0) em = 0.96; // Default

            // Calculate working days per year
            var workingDays = CalculateWorkingDaysPerYear();
            if (workingDays <= 0) return 0;

            // W/m² = (kWh × 1000 × EM) / (Working_days × 24 × Heated_area)
            var specificPower = (annualConsumptionKwh * 1000.0 * em) / (workingDays * 24.0 * heatedArea);

            return specificPower;
        }

        private double CalculateWorkingDaysPerYear()
        {
            if (_objectData == null) return 365;

            // Determine working days from schedules
            var heatingWorkdays = ParseDouble(_objectData.HeatingWorkdaysHours);
            var heatingSaturday = ParseDouble(_objectData.HeatingSaturdayHours);
            var heatingSunday = ParseDouble(_objectData.HeatingSundayHours);

            var coolingWorkdays = ParseDouble(_objectData.CoolingWorkdaysHours);
            var coolingSaturday = ParseDouble(_objectData.CoolingSaturdayHours);
            var coolingSunday = ParseDouble(_objectData.CoolingSundayHours);

            // Count active day types (any schedule active = working day)
            double daysPerWeek = 0;

            if (heatingWorkdays > 0 || coolingWorkdays > 0)
                daysPerWeek += 5; // Workdays

            if (heatingSaturday > 0 || coolingSaturday > 0)
                daysPerWeek += 1; // Saturday

            if (heatingSunday > 0 || coolingSunday > 0)
                daysPerWeek += 1; // Sunday

            if (daysPerWeek == 0) return 0;

            // Annual working days = (weeks per year) × (days per week) - days off
            var weeksPerYear = 365.0 / 7.0;
            var totalDaysOff = _objectData.MonthlyDaysOffSum;
            var workingDays = (weeksPerYear * daysPerWeek) - totalDaysOff;

            return Math.Max(0, workingDays);
        }

        // ========== HELPER METHODS ==========

        private double ParseDouble(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return 0;
            if (double.TryParse(value.Trim().Replace(',', '.'),
                NumberStyles.Any, CultureInfo.InvariantCulture, out double result))
            {
                return result;
            }
            return 0;
        }

        private bool TryParseDouble(string? value, out double result)
        {
            result = 0;
            if (string.IsNullOrWhiteSpace(value)) return false;
            return double.TryParse(value.Trim().Replace(',', '.'),
                NumberStyles.Any, CultureInfo.InvariantCulture, out result);
        }

        private int ParseInt(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return 0;
            if (int.TryParse(value.Trim(), out int result))
            {
                return result;
            }
            return 0;
        }

        // ========== PROPERTY CHANGE NOTIFICATIONS ==========

        private void NotifyHeatingPropertiesChanged()
        {
            OnPropertyChanged(nameof(HeatingTotalAnnualConsumption));
            OnPropertyChanged(nameof(HeatingSpecificPower));
            OnPropertyChanged(nameof(HeatingAnnualHours));
            OnPropertyChanged(nameof(HeatingPumpsTotalAnnualConsumption));
            OnPropertyChanged(nameof(HeatingPumpsSpecificPower));
            OnPropertyChanged(nameof(HeatingFansTotalAnnualConsumption));
            OnPropertyChanged(nameof(HeatingFansSpecificPower));
            RecalculateTotal();
        }

        private void NotifyCoolingPropertiesChanged()
        {
            OnPropertyChanged(nameof(CoolingTotalAnnualConsumption));
            OnPropertyChanged(nameof(CoolingSpecificPower));
            OnPropertyChanged(nameof(CoolingAnnualHours));
            OnPropertyChanged(nameof(CoolingPumpsTotalAnnualConsumption));
            OnPropertyChanged(nameof(CoolingPumpsSpecificPower));
            OnPropertyChanged(nameof(CoolingFansTotalAnnualConsumption));
            OnPropertyChanged(nameof(CoolingFansSpecificPower));
            RecalculateTotal();
        }

        private void NotifyDhwPropertiesChanged()
        {
            OnPropertyChanged(nameof(DhwTotalAnnualConsumption));
            OnPropertyChanged(nameof(DhwSpecificPower));
            OnPropertyChanged(nameof(DhwAnnualHours));
            RecalculateTotal();
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
