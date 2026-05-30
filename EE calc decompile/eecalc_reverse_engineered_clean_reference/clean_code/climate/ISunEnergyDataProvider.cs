namespace EE.Doklad.Services.EecalcClimate
{
    public interface ISunEnergyDataProvider
    {
        double GetMonthlyAvgTemp(int zoneId, Month month);

        double GetMonthlyRadiation(int zoneId, Month month);

        double GetMonthlyCloudiness(int zoneId, Month month);
    }
}
