using System;
using Microsoft.Extensions.DependencyInjection;

namespace EE.Doklad.Services.EecalcClimate
{
    public static class EecalcClimateServiceCollectionExtensions
    {
        public static IServiceCollection AddEecalcClimateProviders(
            this IServiceCollection services,
            ClimateProviderMode mode)
        {
            ArgumentNullException.ThrowIfNull(services);

            services.AddSingleton<IClimateDataProvider>(_ => mode switch
            {
                ClimateProviderMode.LegacyEECalcStrict =>
                    new LegacyEecalcXmlClimateDataProvider(ClimateProviderMode.LegacyEECalcStrict),
                ClimateProviderMode.LegacyEECalcCorrectedData =>
                    new LegacyEecalcXmlClimateDataProvider(ClimateProviderMode.LegacyEECalcCorrectedData),
                ClimateProviderMode.CurrentOrdinance =>
                    new CorrectedJsonClimateDataProvider(),
                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
            });

            services.AddSingleton<ISunEnergyDataProvider>(_ => new LegacyEecalcXmlSunEnergyDataProvider());

            return services;
        }
    }
}
