using System.Collections.Generic;

namespace EE.Doklad.Tests.Validation
{
    public sealed class EecalcValidationFixture
    {
        public string Id { get; init; } = string.Empty;

        public string Scenario { get; init; } = "Actual";

        public int ClimateZoneId { get; init; }

        public int FirstMonth { get; init; }

        public int LastMonth { get; init; }

        public int FirstDay { get; init; }

        public int LastDay { get; init; }

        public double HeatedArea { get; init; }

        public double HeatedVolume { get; init; }

        public double Infiltration { get; init; }

        public double HeatCapacity { get; init; }

        public double MetabolicHeat { get; init; }

        public double LatentMetabolicHeat { get; init; }

        public double ProjectTemperature { get; init; }

        public double NonProjectTemperature { get; init; }

        public double ProjectHumidity { get; init; }

        public double FlowTemperature { get; init; }

        public double FlowRelativeHumidity { get; init; }

        public double VentilationDebit { get; init; }

        public double LightsCoolingPower { get; init; }

        public double BalancedDevicesCoolingPower { get; init; }

        public double LightsCoolingWorkSchedule { get; init; }

        public double BalancedDevicesCoolingWorkSchedule { get; init; }

        public EecalcDailySchedule WorkdaySchedule { get; init; } = new();

        public EecalcDailySchedule SaturdaySchedule { get; init; } = new();

        public EecalcDailySchedule SundaySchedule { get; init; } = new();

        public EecalcDailySchedule OccupantsWorkdaySchedule { get; init; } = new();

        public EecalcDailySchedule OccupantsSaturdaySchedule { get; init; } = new();

        public EecalcDailySchedule OccupantsSundaySchedule { get; init; } = new();

        public EecalcDailySchedule VentilationWorkdaySchedule { get; init; } = new();

        public EecalcDailySchedule VentilationSaturdaySchedule { get; init; } = new();

        public EecalcDailySchedule VentilationSundaySchedule { get; init; } = new();

        public EecalcDailySchedule NightVentilationWorkdaySchedule { get; init; } = new();

        public EecalcDailySchedule NightVentilationSaturdaySchedule { get; init; } = new();

        public EecalcDailySchedule NightVentilationSundaySchedule { get; init; } = new();

        public IReadOnlyDictionary<int, int> HolidaysByMonth { get; init; } = new Dictionary<int, int>();

        public IReadOnlyDictionary<int, double> AverageOutdoorTemperatureByMonth { get; init; } = new Dictionary<int, double>();

        public IReadOnlyDictionary<int, EecalcSolarRadiationFixture> SolarRadiationByMonth { get; init; } =
            new Dictionary<int, EecalcSolarRadiationFixture>();

        public IReadOnlyDictionary<int, IReadOnlyList<EecalcHourlyWeatherFixture>> HourlyWeatherByMonth { get; init; } =
            new Dictionary<int, IReadOnlyList<EecalcHourlyWeatherFixture>>();
    }

    public sealed class EecalcDailySchedule
    {
        public int StartHour { get; init; }

        public int EndHour { get; init; }
    }

    public sealed class EecalcSolarRadiationFixture
    {
        public double N { get; init; }

        public double E { get; init; }

        public double S { get; init; }

        public double W { get; init; }

        public double H { get; init; }
    }

    public sealed class EecalcHourlyWeatherFixture
    {
        public double Temperature { get; init; }

        public double Humidity { get; init; }
    }
}
