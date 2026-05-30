using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EE.Doklad.Services.EecalcClimate;
using EE.Doklad.Tests.Validation;
using EE.Doklad.Tests.Validation.FullOracle;
using EecalcTest;

const double metabolicHeat = 3.16;
const int zoneId = 7;
const string xmlPath = "reference/eecalc-config/DefaultParams.xml";
const string sunXmlPath = "reference/eecalc-config/DefaultSunParams.xml";

var provider = new LegacyEecalcXmlClimateDataProvider(
    ClimateProviderMode.LegacyEECalcStrict,
    xmlPath);

var sunProvider = new LegacyEecalcXmlSunEnergyDataProvider(sunXmlPath);

var averageOutdoorTemperatureByMonth = new Dictionary<int, double>();
var solarRadiationByMonth = new Dictionary<int, EecalcSolarRadiationFixture>();
var hourlyWeatherByMonth = new Dictionary<int, IReadOnlyList<EecalcHourlyWeatherFixture>>();

for (var monthNumber = 1; monthNumber <= 12; monthNumber++)
{
    var month = (Month)(monthNumber - 1);
    var solar = provider.GetSolarRadiation(zoneId, month);
    var hourly = provider.GetHourlyClimateData(zoneId, month);

    averageOutdoorTemperatureByMonth[monthNumber] = provider.GetMonthlyAvgTemp(zoneId, month);
    solarRadiationByMonth[monthNumber] = new EecalcSolarRadiationFixture
    {
        N = solar.N,
        E = solar.E,
        S = solar.S,
        W = solar.W,
        H = solar.H
    };
    hourlyWeatherByMonth[monthNumber] = hourly
        .Select(hour => new EecalcHourlyWeatherFixture
        {
            Temperature = hour.Temperature,
            Humidity = hour.Humidity
        })
        .ToList();
}

var baseFixture = TestFixture.Build();
var calculation = CopyCalculationWithClimate(
    baseFixture.Calculation,
    zoneId,
    averageOutdoorTemperatureByMonth,
    solarRadiationByMonth,
    hourlyWeatherByMonth,
    metabolicHeat);
var coolingCalculation = CopyCalculationWithClimate(
    baseFixture.Calculation,
    zoneId,
    averageOutdoorTemperatureByMonth,
    solarRadiationByMonth,
    hourlyWeatherByMonth,
    metabolicHeat,
    firstMonth: 6,
    lastMonth: 8,
    firstDay: 20,
    lastDay: 31,
    projectTemperature: 26.0,
    nonProjectTemperature: 30.0,
    projectHumidity: 60.0,
    flowTemperature: 22.0,
    flowRelativeHumidity: 40.0,
    ventilationDebit: 0.500,
    workdaySchedule: new EecalcDailySchedule { StartHour = 0, EndHour = 24 },
    saturdaySchedule: new EecalcDailySchedule { StartHour = 0, EndHour = 24 },
    sundaySchedule: new EecalcDailySchedule { StartHour = 0, EndHour = 24 },
    occupantsWorkdaySchedule: new EecalcDailySchedule { StartHour = 0, EndHour = 12 },
    occupantsSaturdaySchedule: new EecalcDailySchedule { StartHour = 0, EndHour = 12 },
    occupantsSundaySchedule: new EecalcDailySchedule { StartHour = 0, EndHour = 12 },
    ventilationWorkdaySchedule: new EecalcDailySchedule { StartHour = 0, EndHour = 20 },
    ventilationSaturdaySchedule: new EecalcDailySchedule { StartHour = 0, EndHour = 20 },
    ventilationSundaySchedule: new EecalcDailySchedule { StartHour = 0, EndHour = 20 },
    nightVentilationWorkdaySchedule: new EecalcDailySchedule { StartHour = 0, EndHour = 0 },
    nightVentilationSaturdaySchedule: new EecalcDailySchedule { StartHour = 0, EndHour = 0 },
    nightVentilationSundaySchedule: new EecalcDailySchedule { StartHour = 0, EndHour = 0 });
var dhwCalculation = CopyCalculationWithClimate(
    baseFixture.Calculation,
    zoneId,
    averageOutdoorTemperatureByMonth,
    solarRadiationByMonth,
    hourlyWeatherByMonth,
    metabolicHeat,
    firstMonth: 1,
    lastMonth: 12,
    firstDay: 1,
    lastDay: 31);

var fixture = new EecalcEnvelopeFixture
{
    Id = baseFixture.Id,
    Calculation = calculation,
    NorthWalls = baseFixture.NorthWalls,
    NorthEastWalls = baseFixture.NorthEastWalls,
    EastWalls = baseFixture.EastWalls,
    SouthEastWalls = baseFixture.SouthEastWalls,
    SouthWalls = baseFixture.SouthWalls,
    SouthWestWalls = baseFixture.SouthWestWalls,
    WestWalls = baseFixture.WestWalls,
    NorthWestWalls = baseFixture.NorthWestWalls,
    Roof = baseFixture.Roof,
    Floor = baseFixture.Floor
};
var coolingFixture = new EecalcEnvelopeFixture
{
    Id = baseFixture.Id,
    Calculation = coolingCalculation,
    NorthWalls = baseFixture.NorthWalls,
    NorthEastWalls = baseFixture.NorthEastWalls,
    EastWalls = baseFixture.EastWalls,
    SouthEastWalls = baseFixture.SouthEastWalls,
    SouthWalls = baseFixture.SouthWalls,
    SouthWestWalls = baseFixture.SouthWestWalls,
    WestWalls = baseFixture.WestWalls,
    NorthWestWalls = baseFixture.NorthWestWalls,
    Roof = baseFixture.Roof,
    Floor = baseFixture.Floor
};

var oracle = new EecalcMonthlyHeatingOracle();
var rows = oracle.Calculate(fixture);
var sumQnd = rows.Sum(row => row.FinalQnd);
var ventInput = TestFixture.BuildVentilation();
var ventilation = new EECalcVentilationOracle().Calculate(calculation, ventInput);
var coolingVentInput = TestFixture.BuildCoolingVentilation();
var coolingVentilation = new EECalcVentilationOracle().Calculate(coolingCalculation, coolingVentInput);
var coolingVentilationInputs = coolingVentilation.Rows.Sum(row => row.CoolingInputs);
var coolingVentilationPowHeating = coolingVentilation.Rows.Sum(row => row.PowHeating);
var coolingVentilationPowCooling = coolingVentilation.Rows.Sum(row => row.PowCooling);
var coolingVentilationWithering = coolingVentilation.Rows.Sum(row => row.WitheringEnergy);
var coolingVentilationNeeded = coolingVentilationPowCooling / (0.96 * 0.97);
var cooling = new EecalcMonthlyCoolingOracle().Calculate(coolingFixture, coolingVentilationInputs);
var coolingFansAndPumpsInput = TestFixture.BuildCoolingFansAndPumps();
var coolingFansAndPumps = new EECalcCoolingFansAndPumpsOracle()
    .Calculate(coolingCalculation, coolingFansAndPumpsInput, coolingVentInput);
var dhwOracle = new EECalcDhwBgvOracle();
var dhwWithoutSolarInput = TestFixture.BuildDhwBgvWithoutSolar();
var dhwWithoutSolar = dhwOracle.Calculate(dhwCalculation, dhwWithoutSolarInput);
var dhwSolarCalculationInput = TestFixture.BuildDhwBgvWithSolarCollectors();
var dhwSolarCalculation = dhwOracle.Calculate(dhwCalculation, dhwSolarCalculationInput);
var dhwWithSolarInput = TestFixture.BuildDhwBgvWithSolarCollectors(dhwSolarCalculation.TotalUsedSunEnergy);
var dhwWithSolar = dhwOracle.Calculate(dhwCalculation, dhwWithSolarInput);
var dhwMonths = new EecalcMonthlyDaysOracle().Calculate(dhwCalculation)
    .ToDictionary(month => month.Month);
var sumMonthlyHeat = ventilation.Rows.Sum(row => row.MonthlyHeat + row.ThermoPumpEnergy);
var sumHeatingInputs = ventilation.Rows.Sum(row => row.HeatingInputs);
var lightingInput = TestFixture.BuildLightingAndDevices();
var lightingOracle = new EECalcLightingDevicesOracle();
var lightingResult = lightingOracle.Calculate(calculation, lightingInput, rows);
var lightingHeatingContribution = lightingResult.ResulLightInputs;
var lightingAnnualEnergy = lightingResult.LightsGeneralNeededEnergy;
var balancedDevicesContribution = lightingResult.ResulAppliancesInputs;
var nonBalancedDevicesContribution = lightingResult.GroupRows
    .Where(row => row.Group == "NonBalancedDevices" && row.Period == "Heating")
    .Sum(row => row.DevicesNeededEnergy);
var totalLightingDevicesContributions =
    lightingHeatingContribution + balancedDevicesContribution;
var combinedNetEnergy = sumQnd - sumHeatingInputs - totalLightingDevicesContributions;
var heatingMonths = new EecalcMonthlyDaysOracle().Calculate(calculation);
const double fansPower = 0.60;
const double ventilationPumpsPower = 0.0;
const double heatingPumpsPower = 0.30;
const double pumpsFansEnergyManagement = 96.0;
var heatingWeeks = heatingMonths.Sum(month => month.Weeks);
var weeklyVentilationHours = WeeklyHours(
    ventInput.WorkdaySchedule,
    ventInput.SaturdaySchedule,
    ventInput.SundaySchedule);
var pumpsFansNeededEnergy =
    (fansPower + ventilationPumpsPower) * weeklyVentilationHours * heatingWeeks / 1000.0
    / (pumpsFansEnergyManagement / 100.0)
    + heatingPumpsPower * 24.0 * 7.0 * heatingWeeks / 1000.0
    / (pumpsFansEnergyManagement / 100.0);

Console.WriteLine($"ZoneId: {zoneId}");
Console.WriteLine("Month | AvgTemp | Qtr | Qve | Qht | Qgn | Qnd");
Console.WriteLine("------|---------|-----|-----|-----|-----|-----");

foreach (var row in rows)
{
    Console.WriteLine(string.Join(
        " | ",
        row.Month.ToString(CultureInfo.InvariantCulture),
        Format(row.AverageOutdoorTemperature),
        Format(row.Qtr),
        Format(row.Qve),
        Format(row.Qht),
        Format(row.Qgn),
        Format(row.FinalQnd)));
}

Console.WriteLine();
Console.WriteLine($"Σ MonthlyHeat: {Format(sumMonthlyHeat)}");
Console.WriteLine($"Σ HeatingInputs: {Format(sumHeatingInputs)}");
Console.WriteLine($"Σ Qnd: {Format(sumQnd)}");

Console.WriteLine();
Console.WriteLine("=== ОСВЕТЛЕНИЕ И УРЕДИ ===");
Console.WriteLine($"Lighting: {Format(lightingHeatingContribution)}");
Console.WriteLine($"BalancedDevices: {Format(balancedDevicesContribution)}");
Console.WriteLine($"NonBalancedDevices: {Format(nonBalancedDevicesContribution)}");
Console.WriteLine($"Σ balance contributions: {Format(totalLightingDevicesContributions)}");
Console.WriteLine($"Lighting annual energy: {Format(lightingAnnualEnergy)}");
Console.WriteLine($"Нетна енергия: {Format(combinedNetEnergy)}");

Console.WriteLine();
Console.WriteLine("=== ПОМПИ И ВЕНТИЛАТОРИ - ОТОПЛЕНИЕ ===");
Console.WriteLine($"Fans: {Format(fansPower)} W/m2");
Console.WriteLine($"Ventilation pumps: {Format(ventilationPumpsPower)} W/m2");
Console.WriteLine($"Heating pumps: {Format(heatingPumpsPower)} W/m2");
Console.WriteLine($"EM and maintenance: {Format(pumpsFansEnergyManagement)} %");
Console.WriteLine($"Heating weeks: {Format(heatingWeeks)}");
Console.WriteLine($"Weekly ventilation hours: {Format(weeklyVentilationHours)}");
Console.WriteLine("Heating pump weekly hours: 168.000");
Console.WriteLine($"Потребна енергия: {Format(pumpsFansNeededEnergy)} kWh/m2");

Console.WriteLine();
Console.WriteLine("Vent Month | Hours | AvgVentTemp | PostRecTemp | MonthlyHeat | HeatingInputs");
Console.WriteLine("-----------|-------|-------------|-------------|-------------|--------------");
foreach (var row in ventilation.Rows)
{
    Console.WriteLine(string.Join(
        " | ",
        row.Month.ToString(CultureInfo.InvariantCulture),
        Format(row.MonthHours),
        Format(row.AverageVentHeatTemp),
        Format(row.PostRecoveryTemp),
        Format(row.MonthlyHeat + row.ThermoPumpEnergy),
        Format(row.HeatingInputs)));
}

Console.WriteLine();
Console.WriteLine("=== COOLING / R6 ===");
Console.WriteLine($"Cooling season: {coolingCalculation.FirstDay} {coolingCalculation.FirstMonth} - {coolingCalculation.LastDay} {coolingCalculation.LastMonth}");
Console.WriteLine($"No inputs net energy: {Format(cooling.ResultNoInputsNetEnergy)}");
Console.WriteLine($"Cooling inputs (free cooling): {Format(cooling.ResultCoolingInputs)}");
Console.WriteLine($"Ventilation inputs: {Format(cooling.ResultVentilationInputs)}");
Console.WriteLine($"Net energy cooling: {Format(cooling.ResultNetEnergy)}");
Console.WriteLine("Cool Month | AvgTemp | Qgain | Qloss | Ac | Eta | QcoolRaw | Qfree | Qve");
Console.WriteLine("-----------|---------|-------|-------|----|-----|----------|-------|----");
foreach (var row in cooling.Rows)
{
    Console.WriteLine(string.Join(
        " | ",
        row.Month.ToString(CultureInfo.InvariantCulture),
        Format(averageOutdoorTemperatureByMonth[row.Month]),
        Format(row.Qgain),
        Format(row.Qloss),
        Format(row.Ac),
        Format(row.Eta),
        Format(row.QcoolRaw),
        Format(row.QfreeCooling),
        Format(row.QveCooling)));
}

Console.WriteLine();
Console.WriteLine("=== VENTILATION - COOLING / R7 ===");
Console.WriteLine($"ResultEnergyForCooling: {Format(coolingVentilationPowCooling)}");
Console.WriteLine($"Cooling-season ResultEnergyForHeating: {Format(coolingVentilationPowHeating)}");
Console.WriteLine($"ResultEnergyForWithering: {Format(coolingVentilationWithering)}");
Console.WriteLine($"ResulCoolingInputs: {Format(coolingVentilationInputs)}");
Console.WriteLine($"ResultNeededEnergy cooling-only: {Format(coolingVentilationNeeded)}");
Console.WriteLine("Vent Cool Month | PowHeating | PowCooling | Withering | CoolingInputs");
Console.WriteLine("----------------|------------|------------|-----------|--------------");
foreach (var row in coolingVentilation.Rows)
{
    Console.WriteLine(string.Join(
        " | ",
        row.Month.ToString(CultureInfo.InvariantCulture),
        Format(row.PowHeating),
        Format(row.PowCooling),
        Format(row.WitheringEnergy),
        Format(row.CoolingInputs)));
}

Console.WriteLine();
Console.WriteLine("=== PUMPS AND FANS - COOLING ===");
Console.WriteLine($"Ventilators ventilation: {Format(coolingFansAndPumpsInput.VentilatorsCool)} W/m2");
Console.WriteLine($"Ventilators outdoor air no treatment: {Format(coolingFansAndPumpsInput.VentilatorsOutdoorAirCool)} W/m2");
Console.WriteLine($"Ventilation pumps: {Format(coolingFansAndPumpsInput.PumpVentilationCool)} W/m2");
Console.WriteLine($"Cooling pumps: {Format(coolingFansAndPumpsInput.CoolingPump)} W/m2");
Console.WriteLine($"EM and maintenance: {Format(coolingFansAndPumpsInput.EnergyManagement)} %");
Console.WriteLine($"Cooling weeks: {Format(coolingFansAndPumps.CoolingWeeks)}");
Console.WriteLine($"Weekly cooling ventilation hours: {Format(coolingFansAndPumps.WeeklyCoolingVentilationHours)}");
Console.WriteLine($"Weekly cooling season hours: {Format(coolingFansAndPumps.WeeklyCoolingSeasonHours)}");
Console.WriteLine("Cooling pump weekly hours: 168.000");
Console.WriteLine($"Needed energy: {Format(coolingFansAndPumps.NeededEnergy)} kWh/m2");

Console.WriteLine();
Console.WriteLine("=== OTHER CONSUMERS - COOLING ===");
Console.WriteLine($"Other ventilation: {Format(coolingFansAndPumpsInput.OtherCoolingVentilation)} W/m2");
Console.WriteLine($"Other cooling: {Format(coolingFansAndPumpsInput.OtherCooling)} W/m2");
Console.WriteLine($"Needed energy other: {Format(coolingFansAndPumps.OtherNeededEnergy)} kWh/m2");

Console.WriteLine();
Console.WriteLine("=== DHW / BGV WITHOUT SOLAR ===");
Console.WriteLine($"Annual consumption: {Format(dhwWithoutSolarInput.Consumption)} l/m2year");
Console.WriteLine($"Temperature difference: {Format(dhwWithoutSolarInput.TempDifference)} C");
Console.WriteLine($"Mixed water annually: {Format(dhwWithoutSolar.MixedWater)} m3");
Console.WriteLine($"Net energy: {Format(dhwWithoutSolar.ResulNetEnergy)} kWh/m2");
Console.WriteLine($"Solar energy for BGV: {Format(dhwWithoutSolarInput.SunEnergy)} kWh/m2");
Console.WriteLine($"Needed energy before sources: {Format(dhwWithoutSolar.ResultEnergyForHeating)} kWh/m2");
Console.WriteLine($"Source 1 needed energy: {Format(dhwWithoutSolar.ResultSourceEnergy)} kWh/m2");
Console.WriteLine($"Source 2 needed energy: {Format(dhwWithoutSolar.ResultSourceEnergy2)} kWh/m2");
Console.WriteLine($"Heat generation efficiency: {Format(dhwWithoutSolar.HeatEfficiencyGenerating)} %");
Console.WriteLine($"Total needed energy: {Format(dhwWithoutSolar.ResultNeededEnergy)} kWh/m2");

Console.WriteLine();
Console.WriteLine("=== DHW / BGV SOLAR COLLECTORS ===");
Console.WriteLine($"Solar usage: {Format(dhwSolarCalculationInput.SolarWaterUsage)} l/day");
Console.WriteLine($"Days in week: {Format(dhwSolarCalculationInput.SolarDaysInWeek)}");
Console.WriteLine($"Collector area: {Format(dhwSolarCalculationInput.AbsorbingSurface * dhwSolarCalculationInput.CollectorsCount)} m2");
Console.WriteLine($"Pitch: {Format(dhwSolarCalculationInput.Pitch)} deg");
Console.WriteLine($"Environment reflection: {Format(dhwSolarCalculationInput.ImpactEnvironment)}");
Console.WriteLine($"Accumulator volume: {Format(dhwSolarCalculationInput.AcumulatorVolume)} l");
Console.WriteLine($"Serpentine efficiency: {Format(dhwSolarCalculationInput.SerpentineEfficiency)} %");
Console.WriteLine($"Solar pump power: {Format(dhwSolarCalculationInput.PumpsVolume)} W/m2");
Console.WriteLine("Month | Days | H | Ht | Tm | QbgvNeed | Fm | QbgvSolar");
Console.WriteLine("------|------|---|----|----|----------|----|----------");
foreach (var row in dhwSolarCalculation.SolarRows)
{
    var month = (Month)(row.Month - 1);
    Console.WriteLine(string.Join(
        " | ",
        row.Month.ToString(CultureInfo.InvariantCulture),
        Format(dhwSolarCalculationInput.SolarDaysInWeek * dhwMonths[row.Month].Weeks),
        Format(sunProvider.GetMonthlyRadiation(zoneId, month)),
        Format(row.Ht),
        Format(sunProvider.GetMonthlyAvgTemp(zoneId, month)),
        Format(row.QhotWater),
        Format(row.Fm),
        Format(row.UsedSunEnergy)));
}

Console.WriteLine($"Solar share: {Format(dhwSolarCalculation.BGVSunEnergy == 0.0 ? 0.0 : dhwSolarCalculation.BGVSunEnergy / dhwSolarCalculation.SolarRows.Sum(row => row.QhotWater) * 100.0)} %");
Console.WriteLine($"Absorbed solar energy for BGV: {Format(dhwSolarCalculation.BGVSunEnergy)} kWh");
Console.WriteLine($"Received solar heat: {Format(dhwSolarCalculation.BGVSunEnergy)} kWh");
Console.WriteLine($"Pump needed energy: {Format(dhwSolarCalculation.BGVPumpsTotal)} kWh");
Console.WriteLine($"Solar energy per heated area: {Format(dhwSolarCalculation.TotalUsedSunEnergy)} kWh/m2");
Console.WriteLine($"BGV table net energy: {Format(dhwWithSolar.ResulNetEnergy)} kWh/m2");
Console.WriteLine($"BGV table solar energy: {Format(dhwWithSolarInput.SunEnergy)} kWh/m2");
Console.WriteLine($"BGV table needed energy: {Format(dhwWithSolar.ResultNeededEnergy)} kWh/m2");

static EecalcValidationFixture CopyCalculationWithClimate(
    EecalcValidationFixture source,
    int zoneId,
    IReadOnlyDictionary<int, double> averageOutdoorTemperatureByMonth,
    IReadOnlyDictionary<int, EecalcSolarRadiationFixture> solarRadiationByMonth,
    IReadOnlyDictionary<int, IReadOnlyList<EecalcHourlyWeatherFixture>> hourlyWeatherByMonth,
    double metabolicHeat,
    int? firstMonth = null,
    int? lastMonth = null,
    int? firstDay = null,
    int? lastDay = null,
    double? projectTemperature = null,
    double? nonProjectTemperature = null,
    double? projectHumidity = null,
    double? flowTemperature = null,
    double? flowRelativeHumidity = null,
    double? ventilationDebit = null,
    EecalcDailySchedule? workdaySchedule = null,
    EecalcDailySchedule? saturdaySchedule = null,
    EecalcDailySchedule? sundaySchedule = null,
    EecalcDailySchedule? occupantsWorkdaySchedule = null,
    EecalcDailySchedule? occupantsSaturdaySchedule = null,
    EecalcDailySchedule? occupantsSundaySchedule = null,
    EecalcDailySchedule? ventilationWorkdaySchedule = null,
    EecalcDailySchedule? ventilationSaturdaySchedule = null,
    EecalcDailySchedule? ventilationSundaySchedule = null,
    EecalcDailySchedule? nightVentilationWorkdaySchedule = null,
    EecalcDailySchedule? nightVentilationSaturdaySchedule = null,
    EecalcDailySchedule? nightVentilationSundaySchedule = null)
{
    return new EecalcValidationFixture
    {
        Id = source.Id,
        Scenario = source.Scenario,
        ClimateZoneId = zoneId,
        FirstMonth = firstMonth ?? source.FirstMonth,
        LastMonth = lastMonth ?? source.LastMonth,
        FirstDay = firstDay ?? source.FirstDay,
        LastDay = lastDay ?? source.LastDay,
        HeatedArea = source.HeatedArea,
        HeatedVolume = source.HeatedVolume,
        Infiltration = source.Infiltration,
        HeatCapacity = source.HeatCapacity,
        MetabolicHeat = metabolicHeat,
        LatentMetabolicHeat = source.LatentMetabolicHeat,
        ProjectTemperature = projectTemperature ?? source.ProjectTemperature,
        NonProjectTemperature = nonProjectTemperature ?? source.NonProjectTemperature,
        ProjectHumidity = projectHumidity ?? source.ProjectHumidity,
        FlowTemperature = flowTemperature ?? source.FlowTemperature,
        FlowRelativeHumidity = flowRelativeHumidity ?? source.FlowRelativeHumidity,
        VentilationDebit = ventilationDebit ?? source.VentilationDebit,
        LightsCoolingPower = source.LightsCoolingPower,
        BalancedDevicesCoolingPower = source.BalancedDevicesCoolingPower,
        LightsCoolingWorkSchedule = source.LightsCoolingWorkSchedule,
        BalancedDevicesCoolingWorkSchedule = source.BalancedDevicesCoolingWorkSchedule,
        WorkdaySchedule = workdaySchedule ?? source.WorkdaySchedule,
        SaturdaySchedule = saturdaySchedule ?? source.SaturdaySchedule,
        SundaySchedule = sundaySchedule ?? source.SundaySchedule,
        OccupantsWorkdaySchedule = occupantsWorkdaySchedule ?? source.OccupantsWorkdaySchedule,
        OccupantsSaturdaySchedule = occupantsSaturdaySchedule ?? source.OccupantsSaturdaySchedule,
        OccupantsSundaySchedule = occupantsSundaySchedule ?? source.OccupantsSundaySchedule,
        VentilationWorkdaySchedule = ventilationWorkdaySchedule ?? source.VentilationWorkdaySchedule,
        VentilationSaturdaySchedule = ventilationSaturdaySchedule ?? source.VentilationSaturdaySchedule,
        VentilationSundaySchedule = ventilationSundaySchedule ?? source.VentilationSundaySchedule,
        NightVentilationWorkdaySchedule = nightVentilationWorkdaySchedule ?? source.NightVentilationWorkdaySchedule,
        NightVentilationSaturdaySchedule = nightVentilationSaturdaySchedule ?? source.NightVentilationSaturdaySchedule,
        NightVentilationSundaySchedule = nightVentilationSundaySchedule ?? source.NightVentilationSundaySchedule,
        HolidaysByMonth = source.HolidaysByMonth,
        AverageOutdoorTemperatureByMonth = averageOutdoorTemperatureByMonth,
        SolarRadiationByMonth = solarRadiationByMonth,
        HourlyWeatherByMonth = hourlyWeatherByMonth
    };
}

static string Format(double value)
{
    return Math.Round(value, 3, MidpointRounding.AwayFromZero).ToString("0.000", CultureInfo.InvariantCulture);
}

static double Duration(EecalcDailySchedule schedule)
{
    return schedule.EndHour > schedule.StartHour
        ? schedule.EndHour - schedule.StartHour
        : 0.0;
}

static double WeeklyHours(
    EecalcDailySchedule workday,
    EecalcDailySchedule saturday,
    EecalcDailySchedule sunday)
{
    return 5.0 * Duration(workday) + Duration(sunday) + Duration(saturday);
}
