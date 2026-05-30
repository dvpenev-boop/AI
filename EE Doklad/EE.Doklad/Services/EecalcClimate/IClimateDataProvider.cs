using System.Collections.Generic;

namespace EE.Doklad.Services.EecalcClimate
{
    public interface IClimateDataProvider
    {
        double GetMonthlyAvgTemp(int zoneId, Month month);

        SolarRadiationData GetSolarRadiation(int zoneId, Month month);

        IReadOnlyList<HourlyClimateData> GetHourlyClimateData(int zoneId, Month month);

        double GetPb(int zoneId);
    }
}
