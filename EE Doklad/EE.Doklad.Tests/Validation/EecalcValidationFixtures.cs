using System.Collections.Generic;

namespace EE.Doklad.Tests.Validation
{
    public static class EecalcValidationFixtures
    {
        public const string Zone9MinimalHeating2260626 = "zone9_minimal_heating_22_60626";

        public static EecalcValidationFixture CreateZone9MinimalHeating2260626()
        {
            const double averageOutdoorTemperature = 6.148125;

            return new EecalcValidationFixture
            {
                Id = Zone9MinimalHeating2260626,
                Scenario = "Actual",
                ClimateZoneId = 9,
                FirstMonth = 10,
                LastMonth = 4,
                FirstDay = 28,
                LastDay = 5,
                HeatedArea = 1000.0,
                HeatedVolume = 1250.0,
                Infiltration = 1.0,
                ProjectTemperature = 20.0,
                NonProjectTemperature = 20.0,
                WorkdaySchedule = new EecalcDailySchedule { StartHour = 0, EndHour = 24 },
                SaturdaySchedule = new EecalcDailySchedule { StartHour = 0, EndHour = 24 },
                SundaySchedule = new EecalcDailySchedule { StartHour = 0, EndHour = 24 },
                HolidaysByMonth = new Dictionary<int, int>
                {
                    [1] = 0,
                    [2] = 0,
                    [3] = 0,
                    [4] = 0,
                    [10] = 0,
                    [11] = 0,
                    [12] = 0
                },
                AverageOutdoorTemperatureByMonth = new Dictionary<int, double>
                {
                    [1] = averageOutdoorTemperature,
                    [2] = averageOutdoorTemperature,
                    [3] = averageOutdoorTemperature,
                    [4] = averageOutdoorTemperature,
                    [10] = averageOutdoorTemperature,
                    [11] = averageOutdoorTemperature,
                    [12] = averageOutdoorTemperature
                }
            };
        }
    }
}
