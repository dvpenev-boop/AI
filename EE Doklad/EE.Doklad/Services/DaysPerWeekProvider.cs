using System;
using System.ComponentModel;
using EE.Doklad.Models;

namespace EE.Dokлад.Services
{
    /// <summary>
    /// Computes days-per-week for the building based on ObjectDataSectionData occupancy schedule fields.
    /// Logic: if OccupancyWorkdaysHours contains a numeric > 0, treat that as "workdays present" -> base 5 days.
    /// Then add 1 for Saturday if OccupancySaturdayHours > 0 and add 1 for Sunday if OccupancySundayHours > 0.
    /// If no workdays value is provided, fall back to defaultDays (5).
    /// Raises DaysPerWeekChanged when the computed value changes.
    /// </summary>
    public class DaysPerWeekProvider : IDisposable
    {
        private readonly ObjectDataSectionData _objectData;
        private int _daysPerWeek;

        public event Action<int>? DaysPerWeekChanged;

        public DaysPerWeekProvider(ObjectDataSectionData objectData)
        {
            _objectData = objectData ?? throw new ArgumentNullException(nameof(objectData));
            _objectData.PropertyChanged += ObjectData_PropertyChanged;
            _daysPerWeek = ComputeDaysPerWeek();
        }

        private void ObjectData_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // react only to occupancy-related fields
            if (e.PropertyName == nameof(ObjectDataSectionData.OccupancyWorkdaysHours) ||
                e.PropertyName == nameof(ObjectDataSectionData.OccupancySaturdayHours) ||
                e.PropertyName == nameof(ObjectDataSectionData.OccupancySundayHours) ||
                e.PropertyName == nameof(ObjectDataSectionData.DaysOffJanuary) ||
                e.PropertyName == nameof(ObjectDataSectionData.DaysOffFebruary) ||
                e.PropertyName == nameof(ObjectDataSectionData.DaysOffMarch) ||
                e.PropertyName == nameof(ObjectDataSectionData.DaysOffApril) ||
                e.PropertyName == nameof(ObjectDataSectionData.DaysOffMay) ||
                e.PropertyName == nameof(ObjectDataSectionData.DaysOffJune) ||
                e.PropertyName == nameof(ObjectDataSectionData.DaysOffJuly) ||
                e.PropertyName == nameof(ObjectDataSectionData.DaysOffAugust) ||
                e.PropertyName == nameof(ObjectDataSectionData.DaysOffSeptember) ||
                e.PropertyName == nameof(ObjectDataSectionData.DaysOffOctober) ||
                e.PropertyName == nameof(ObjectDataSectionData.DaysOffNovember) ||
                e.PropertyName == nameof(ObjectDataSectionData.DaysOffDecember))
            {
                var newDays = ComputeDaysPerWeek();
                if (newDays != _daysPerWeek)
                {
                    _daysPerWeek = newDays;
                    DaysPerWeekChanged?.Invoke(_daysPerWeek);
                }
            }
        }

        private static bool ParsePositive(string? s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;
            if (double.TryParse(s.Trim(), out double v)) return v > 0.0;
            return false;
        }

        private int ComputeDaysPerWeek()
        {
            const int defaultDays = 5;

            bool work = ParsePositive(_objectData.OccupancyWorkdaysHours);
            bool sat = ParsePositive(_objectData.OccupancySaturdayHours);
            bool sun = ParsePositive(_objectData.OccupancySundayHours);

            if (work)
            {
                int days = 5 + (sat ? 1 : 0) + (sun ? 1 : 0);
                return days;
            }

            // fallback to default
            return defaultDays;
        }

        public int GetDaysPerWeek() => _daysPerWeek;

        public void Dispose()
        {
            _objectData.PropertyChanged -= ObjectData_PropertyChanged;
        }
    }
}
