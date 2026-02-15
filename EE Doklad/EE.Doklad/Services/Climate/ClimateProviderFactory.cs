using System;
using EE.Doklad.Models;
using EE.Doklad.Models.Climate;
using EE.Doklad.Services;

namespace EE.Doklad.Services.Climate
{
    /// <summary>
    /// Factory за създаване на IClimateDataProvider в зависимост от избора на потребителя.
    /// </summary>
    public class ClimateProviderFactory
    {
        private readonly ClimateService _climateService;

        public ClimateProviderFactory()
        {
            _climateService = new ClimateService(new JsonClimateRepository());
        }

        /// <summary>
        /// Създава climate provider в зависимост от настройките.
        /// </summary>
        /// <param name="database">Избрана база данни (BG или ASHRAE)</param>
        /// <param name="climateZone">Номер на климатична зона (за BG)</param>
        /// <param name="report">Report инстанция (за ASHRAE EPW данни)</param>
        /// <returns>IClimateDataProvider или null ако липсват данни</returns>
        public IClimateDataProvider? CreateProvider(
            ClimateDatabase database,
            int climateZone,
            Report? report)
        {
            switch (database)
            {
                case ClimateDatabase.BG:
                    return CreateBgProvider(climateZone);

                case ClimateDatabase.ASHRAE:
                    return CreateEpwProvider(report);

                default:
                    throw new ArgumentException($"Неподдържана климатична база данни: {database}");
            }
        }

        /// <summary>
        /// Създава BG типични дни провайдър.
        /// </summary>
        private IClimateDataProvider? CreateBgProvider(int climateZone)
        {
            if (climateZone < 1 || climateZone > 9)
                return null;

            try
            {
                return new BgTypicalDayClimateProvider(climateZone);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Създава EPW провайдър от вградени данни в Report.
        /// </summary>
        private IClimateDataProvider? CreateEpwProvider(Report? report)
        {
            if (report?.EmbeddedEpwData == null)
                return null;

            try
            {
                return report.EmbeddedEpwData.ToClimateProvider();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Проверява дали има валидни климатични данни за дадените настройки.
        /// </summary>
        public bool HasValidClimateData(
            ClimateDatabase database,
            int climateZone,
            Report? report)
        {
            return CreateProvider(database, climateZone, report) != null;
        }

        /// <summary>
        /// Извлича display име за климатичните данни.
        /// </summary>
        public string GetClimateDisplayName(
            ClimateDatabase database,
            int climateZone,
            Report? report)
        {
            switch (database)
            {
                case ClimateDatabase.BG:
                    var zoneData = _climateService.GetZone(climateZone);
                    return zoneData != null ? $"BG: {zoneData.Name}" : $"BG: Зона {climateZone}";

                case ClimateDatabase.ASHRAE:
                    if (report?.EmbeddedEpwData != null)
                        return $"ASHRAE: {report.EmbeddedEpwData.GetDisplayName()}";
                    return "ASHRAE: (няма зареден EPW)";

                default:
                    return "Непознат източник";
            }
        }
    }
}
