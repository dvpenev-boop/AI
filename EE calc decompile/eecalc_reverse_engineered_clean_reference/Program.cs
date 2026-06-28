using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using EE.Doklad.Services.EecalcClimate;
using EE.Doklad.Tests.Validation;
using EE.Doklad.Tests.Validation.FullOracle;
using EecalcTest;

const double metabolicHeat = 3.16;
var zoneId = GetClimateZoneId(args);
var coolingFlowTemperature = GetCoolingFlowTemperature(args);
var coolingVentilationDebit = GetCoolingVentilationDebit(args);
var coolingEsmFlowTemperature = GetCoolingEsmFlowTemperature(args, coolingFlowTemperature);
var coolingCoreFlowTemperature = GetOptionDouble(args, "--cooling-core-flow-temp", 22.0, allowNegative: true);
var coolingCoreFlowHumidity = GetOptionDouble(args, "--cooling-core-flow-rh", 40.0, allowNegative: false);
var coolingFreeDebit = GetOptionDouble(args, "--cooling-free-debit", 0.0, allowNegative: false);
var coolingEsmFreeDebit = GetOptionDouble(args, "--cooling-esm-free-debit", coolingFreeDebit, allowNegative: false);
var coolingFreeWorkSchedule = GetSchedule(args, "--cooling-free-work", 0, 0);
var coolingFreeSaturdaySchedule = GetSchedule(args, "--cooling-free-sat", 0, 0);
var coolingFreeSundaySchedule = GetSchedule(args, "--cooling-free-sun", 0, 0);
var coolingFirstMonth = GetOptionInt(args, "--cooling-first-month", 6);
var coolingLastMonth = GetOptionInt(args, "--cooling-last-month", 8);
var coolingFirstDay = GetOptionInt(args, "--cooling-first-day", 20);
var coolingLastDay = GetOptionInt(args, "--cooling-last-day", 31);
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
var holidaysByMonth = ApplyHolidayOverrides(baseFixture.Calculation.HolidaysByMonth, args);
var calculation = CopyCalculationWithClimate(
    baseFixture.Calculation,
    zoneId,
    averageOutdoorTemperatureByMonth,
    solarRadiationByMonth,
    hourlyWeatherByMonth,
    metabolicHeat,
    holidaysByMonth: holidaysByMonth);
var coolingCalculation = CopyCalculationWithClimate(
    baseFixture.Calculation,
    zoneId,
    averageOutdoorTemperatureByMonth,
    solarRadiationByMonth,
    hourlyWeatherByMonth,
    metabolicHeat,
    firstMonth: coolingFirstMonth,
    lastMonth: coolingLastMonth,
    firstDay: coolingFirstDay,
    lastDay: coolingLastDay,
    projectTemperature: 26.0,
    nonProjectTemperature: 30.0,
    projectHumidity: 60.0,
    flowTemperature: coolingCoreFlowTemperature,
    flowRelativeHumidity: coolingCoreFlowHumidity,
    ventilationDebit: coolingFreeDebit,
    workdaySchedule: new EecalcDailySchedule { StartHour = 0, EndHour = 24 },
    saturdaySchedule: new EecalcDailySchedule { StartHour = 0, EndHour = 24 },
    sundaySchedule: new EecalcDailySchedule { StartHour = 0, EndHour = 24 },
    occupantsWorkdaySchedule: new EecalcDailySchedule { StartHour = 0, EndHour = 12 },
    occupantsSaturdaySchedule: new EecalcDailySchedule { StartHour = 0, EndHour = 12 },
    occupantsSundaySchedule: new EecalcDailySchedule { StartHour = 0, EndHour = 12 },
    ventilationWorkdaySchedule: new EecalcDailySchedule { StartHour = 0, EndHour = 20 },
    ventilationSaturdaySchedule: new EecalcDailySchedule { StartHour = 0, EndHour = 20 },
    ventilationSundaySchedule: new EecalcDailySchedule { StartHour = 0, EndHour = 20 },
    nightVentilationWorkdaySchedule: coolingFreeWorkSchedule,
    nightVentilationSaturdaySchedule: coolingFreeSaturdaySchedule,
    nightVentilationSundaySchedule: coolingFreeSundaySchedule,
    holidaysByMonth: holidaysByMonth);
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
    lastDay: 31,
    holidaysByMonth: holidaysByMonth);

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

static int GetClimateZoneId(string[] args)
{
    const int defaultZoneId = 7;
    var optionIndex = Array.IndexOf(args, "--climate-zone");
    if (optionIndex < 0)
    {
        return defaultZoneId;
    }

    if (optionIndex == args.Length - 1
        || !int.TryParse(args[optionIndex + 1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var zoneId)
        || zoneId < 1
        || zoneId > 9)
    {
        throw new ArgumentException("--climate-zone expects an integer value from 1 to 9.");
    }

    return zoneId;
}

static double GetCoolingFlowTemperature(string[] args)
{
    const double defaultFlowTemperature = 22.0;
    var optionIndex = Array.IndexOf(args, "--cooling-flow-temp");
    if (optionIndex < 0)
    {
        return defaultFlowTemperature;
    }

    if (optionIndex == args.Length - 1
        || !double.TryParse(args[optionIndex + 1], NumberStyles.Float, CultureInfo.InvariantCulture, out var flowTemperature))
    {
        throw new ArgumentException("--cooling-flow-temp expects a numeric value.");
    }

    return flowTemperature;
}

static double GetCoolingVentilationDebit(string[] args)
{
    return GetOptionDouble(args, "--cooling-vent-debit", 0.500, allowNegative: false);
}

static double GetCoolingEsmFlowTemperature(string[] args, double defaultFlowTemperature)
{
    var optionIndex = Array.IndexOf(args, "--cooling-esm-flow-temp");
    if (optionIndex < 0)
    {
        return defaultFlowTemperature;
    }

    if (optionIndex == args.Length - 1
        || !double.TryParse(args[optionIndex + 1], NumberStyles.Float, CultureInfo.InvariantCulture, out var flowTemperature))
    {
        throw new ArgumentException("--cooling-esm-flow-temp expects a numeric value.");
    }

    return flowTemperature;
}

static double GetOptionDouble(
    string[] args,
    string optionName,
    double defaultValue,
    bool allowNegative)
{
    var optionIndex = Array.IndexOf(args, optionName);
    if (optionIndex < 0)
    {
        return defaultValue;
    }

    if (optionIndex == args.Length - 1
        || !double.TryParse(args[optionIndex + 1], NumberStyles.Float, CultureInfo.InvariantCulture, out var value)
        || (!allowNegative && value < 0.0))
    {
        var qualifier = allowNegative ? "numeric" : "non-negative numeric";
        throw new ArgumentException($"{optionName} expects a {qualifier} value.");
    }

    return value;
}

static int GetOptionInt(
    string[] args,
    string optionName,
    int defaultValue)
{
    var optionIndex = Array.IndexOf(args, optionName);
    if (optionIndex < 0)
    {
        return defaultValue;
    }

    if (optionIndex == args.Length - 1
        || !int.TryParse(args[optionIndex + 1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var value))
    {
        throw new ArgumentException($"{optionName} expects an integer value.");
    }

    return value;
}

static IReadOnlyDictionary<int, int> ApplyHolidayOverrides(
    IReadOnlyDictionary<int, int> source,
    string[] args)
{
    var result = new Dictionary<int, int>(source);
    for (var i = 0; i < args.Length; i++)
    {
        if (!string.Equals(args[i], "--holiday", StringComparison.Ordinal))
        {
            continue;
        }

        if (i > args.Length - 3
            || !int.TryParse(args[i + 1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var month)
            || !int.TryParse(args[i + 2], NumberStyles.Integer, CultureInfo.InvariantCulture, out var holidays)
            || month < 1
            || month > 12
            || holidays < 0)
        {
            throw new ArgumentException("--holiday expects month and non-negative day count: --holiday 8 10.");
        }

        result[month] = holidays;
        i += 2;
    }

    return result;
}

static EecalcDailySchedule GetSchedule(
    string[] args,
    string optionName,
    int defaultStart,
    int defaultEnd)
{
    var optionIndex = Array.IndexOf(args, optionName);
    if (optionIndex < 0)
    {
        return new EecalcDailySchedule { StartHour = defaultStart, EndHour = defaultEnd };
    }

    if (optionIndex > args.Length - 3
        || !int.TryParse(args[optionIndex + 1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var start)
        || !int.TryParse(args[optionIndex + 2], NumberStyles.Integer, CultureInfo.InvariantCulture, out var end)
        || start < 0
        || start > 24
        || end < 0
        || end > 24)
    {
        throw new ArgumentException($"{optionName} expects two hour values from 0 to 24: start end.");
    }

    return new EecalcDailySchedule { StartHour = start, EndHour = end };
}

if (args.Contains("--verify-envelope-savings"))
{
    RunEnvelopeSavingsVerification(fixture);
    return;
}

if (args.Contains("--verify-ventilation-savings"))
{
    RunVentilationSavingsVerification(calculation, coolingCalculation);
    return;
}

if (args.Contains("--verify-component-savings"))
{
    RunComponentSavingsVerification(fixture, coolingFixture, calculation, coolingCalculation, coolingEsmFlowTemperature);
    return;
}

if (args.Contains("--verify-full-project"))
{
    RunFullProjectVerification(fixture, coolingFixture, calculation, coolingCalculation, dhwCalculation, coolingFlowTemperature, coolingEsmFlowTemperature, coolingVentilationDebit, coolingEsmFreeDebit);
    return;
}

if (args.Contains("--verify-cooling-details"))
{
    RunCoolingDetailsVerification(coolingFixture, coolingCalculation, coolingFlowTemperature, coolingEsmFlowTemperature, coolingVentilationDebit, coolingEsmFreeDebit);
    return;
}

var oracle = new EecalcMonthlyHeatingOracle();
var rows = oracle.Calculate(fixture);
var sumQnd = rows.Sum(row => row.FinalQnd);
var ventInput = TestFixture.BuildVentilation();
var ventilation = new EECalcVentilationOracle().Calculate(calculation, ventInput);
var coolingVentInput = BuildCoolingVentilationFor(coolingCalculation, coolingFlowTemperature, coolingVentilationDebit);
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
    double? lightsCoolingPower = null,
    double? balancedDevicesCoolingPower = null,
    double? lightsCoolingWorkSchedule = null,
    double? balancedDevicesCoolingWorkSchedule = null,
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
    EecalcDailySchedule? nightVentilationSundaySchedule = null,
    IReadOnlyDictionary<int, int>? holidaysByMonth = null)
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
        LightsCoolingPower = lightsCoolingPower ?? source.LightsCoolingPower,
        BalancedDevicesCoolingPower = balancedDevicesCoolingPower ?? source.BalancedDevicesCoolingPower,
        LightsCoolingWorkSchedule = lightsCoolingWorkSchedule ?? source.LightsCoolingWorkSchedule,
        BalancedDevicesCoolingWorkSchedule = balancedDevicesCoolingWorkSchedule ?? source.BalancedDevicesCoolingWorkSchedule,
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
        HolidaysByMonth = holidaysByMonth ?? source.HolidaysByMonth,
        AverageOutdoorTemperatureByMonth = averageOutdoorTemperatureByMonth,
        SolarRadiationByMonth = solarRadiationByMonth,
        HourlyWeatherByMonth = hourlyWeatherByMonth
    };
}

static string Format(double value)
{
    return Math.Round(value, 3, MidpointRounding.AwayFromZero).ToString("0.000", CultureInfo.InvariantCulture);
}

static string Format6(double value)
{
    return Math.Round(value, 6, MidpointRounding.AwayFromZero).ToString("0.000000", CultureInfo.InvariantCulture);
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

static void RunFullProjectVerification(
    EecalcEnvelopeFixture heatingSource,
    EecalcEnvelopeFixture coolingSource,
    EecalcValidationFixture heatingCalculation,
    EecalcValidationFixture coolingCalculation,
    EecalcValidationFixture dhwCalculation,
    double coolingFlowTemperature,
    double coolingEsmFlowTemperature,
    double coolingVentilationDebit,
    double coolingEsmFreeDebit)
{
    var baseLineUValues = new EecalcEnvelopeUValues
    {
        OuterWallsU = 3.000,
        WindowsU = 2.000,
        NonTransparentRoofU = 2.000,
        FloorU = 1.000
    };
    var esmUValues = new EecalcEnvelopeUValues
    {
        OuterWallsU = 0.250,
        WindowsU = 1.100,
        NonTransparentRoofU = 0.250,
        FloorU = 0.500
    };

    var currentHeatingFixture = EecalcEnvelopeEsmCalculator.CreateEnvelopeState(heatingSource, baseLineUValues);
    var currentCoolingFixture = EecalcEnvelopeEsmCalculator.CreateEnvelopeState(coolingSource, baseLineUValues);
    var esmHeatingFixture = EecalcEnvelopeEsmCalculator.CreateEnvelopeState(currentHeatingFixture, esmUValues);

    var lightingBaseLine = TestFixture.BuildLightingAndDevices();
    var lightingEsm = EecalcLightingDevicesEsmCalculator.Clone(
        lightingBaseLine,
        lights: EecalcLightingDevicesEsmCalculator.Clone(
            lightingBaseLine.Lights,
            heatingPower: 0.40,
            heatingWorkSchedule: 40.0,
            coolingPower: 0.40,
            coolingWorkSchedule: 40.0,
            generalPower: 0.40,
            generalWorkSchedule: 40.0));

    var esmCoolingCalculation = CloneCalculationWithCoolingLoads(
        coolingCalculation,
        lightsCoolingPower: 0.40,
        lightsCoolingWorkSchedule: 40.0,
        ventilationDebit: coolingEsmFreeDebit);
    var esmCoolingEnvelope = EecalcEnvelopeEsmCalculator.CreateEnvelopeState(coolingSource, esmUValues);
    var esmCoolingFixture = WithCalculation(esmCoolingEnvelope, esmCoolingCalculation);

    var heatingVentilationBaseLine = TestFixture.BuildVentilation();
    var heatingVentilationEsm = EecalcVentilationEsmCalculator.Clone(
        heatingVentilationBaseLine,
        firstRecEfficiency: 80.0);
    var coolingVentilationSource = BuildCoolingVentilationFor(coolingCalculation, coolingFlowTemperature, coolingVentilationDebit);
    var coolingVentilationEsmSource = BuildCoolingVentilationFor(esmCoolingCalculation, coolingEsmFlowTemperature, coolingVentilationDebit);
    var coolingVentilation = EecalcVentilationEsmCalculator.Clone(
        coolingVentilationSource,
        coolingEfficiency1: new EECalcEfficiencyChain
        {
            TransmitTempEfficiency = coolingVentilationSource.CoolingEfficiency1.TransmitTempEfficiency,
            SupplyNetEfficiency = coolingVentilationSource.CoolingEfficiency1.SupplyNetEfficiency,
            Automatic = 97.0,
            EnergyManagement = coolingVentilationSource.CoolingEfficiency1.EnergyManagement,
            GeneratorEfficiency = 280.0
        });
    var coolingVentilationEsm = EecalcVentilationEsmCalculator.Clone(
        coolingVentilationEsmSource,
        coolingEfficiency1: new EECalcEfficiencyChain
        {
            TransmitTempEfficiency = coolingVentilationEsmSource.CoolingEfficiency1.TransmitTempEfficiency,
            SupplyNetEfficiency = coolingVentilationEsmSource.CoolingEfficiency1.SupplyNetEfficiency,
            Automatic = 97.0,
            EnergyManagement = coolingVentilationEsmSource.CoolingEfficiency1.EnergyManagement,
            GeneratorEfficiency = 280.0
        });

    var dhwOracle = new EECalcDhwBgvOracle();
    var dhwWithoutSolarInput = TestFixture.BuildDhwBgvWithoutSolar();
    var dhwWithoutSolar = dhwOracle.Calculate(dhwCalculation, dhwWithoutSolarInput);
    var dhwSolarCalculationInput = TestFixture.BuildDhwBgvWithSolarCollectors();
    var dhwSolarCalculation = dhwOracle.Calculate(dhwCalculation, dhwSolarCalculationInput);
    var dhwWithSolarInput = TestFixture.BuildDhwBgvWithSolarCollectors(dhwSolarCalculation.TotalUsedSunEnergy);
    var dhwWithSolar = dhwOracle.Calculate(dhwCalculation, dhwWithSolarInput);
    var esmDhwPumps = dhwWithSolar.BGVPumpsTotal / dhwCalculation.HeatedArea;

    var heatingFansBaseLine = new EECalcHeatingFansAndPumpsInput
    {
        VentilatorsHeat = 0.60,
        PumpVentilationHeat = 0.00,
        HeatingPump = 0.30,
        EnergyManagement = 96.0
    };
    var heatingFansEsm = EecalcFansPumpsEsmCalculator.Clone(
        heatingFansBaseLine,
        ventilatorsHeat: 0.30,
        heatingPump: 0.15);
    var coolingFansBaseLine = TestFixture.BuildCoolingFansAndPumps();
    var coolingFansEsm = EecalcFansPumpsEsmCalculator.Clone(
        coolingFansBaseLine,
        ventilatorsCool: 0.40,
        pumpVentilationCool: 0.10,
        coolingPump: 0.50);

    var coolingCoreInput = BuildCoolingVentilationFor(coolingCalculation, coolingFlowTemperature, coolingVentilationDebit);
    var esmCoolingCoreInput = BuildCoolingVentilationFor(esmCoolingCalculation, coolingEsmFlowTemperature, coolingVentilationDebit);
    var currentHeating = CalculateHeatingNeeded(currentHeatingFixture, heatingVentilationBaseLine, lightingBaseLine);
    var esmHeating = CalculateHeatingNeeded(esmHeatingFixture, heatingVentilationEsm, lightingEsm);
    var currentCooling = CalculateCoolingNeededSource(currentCoolingFixture, coolingCoreInput);
    var esmCooling = CalculateCoolingNeededSource(esmCoolingFixture, esmCoolingCoreInput);

    var currentHeatingVentilation = new EECalcVentilationOracle()
        .Calculate(heatingCalculation, heatingVentilationBaseLine)
        .HeatingNeededEnergy;
    var esmHeatingVentilation = new EECalcVentilationOracle()
        .Calculate(heatingCalculation, heatingVentilationEsm)
        .HeatingNeededEnergy;
    var currentCoolingVentilation = new EECalcVentilationOracle()
        .Calculate(coolingCalculation, coolingVentilation)
        .CoolingNeededEnergy;
    var esmCoolingVentilation = new EECalcVentilationOracle()
        .Calculate(esmCoolingCalculation, coolingVentilationEsm)
        .CoolingNeededEnergy;

    var currentHeatingFans = new EECalcHeatingFansAndPumpsOracle()
        .Calculate(heatingCalculation, heatingFansBaseLine, heatingVentilationBaseLine);
    var esmHeatingFans = new EECalcHeatingFansAndPumpsOracle()
        .Calculate(heatingCalculation, heatingFansEsm, heatingVentilationBaseLine);
    var currentCoolingFans = new EECalcCoolingFansAndPumpsOracle()
        .Calculate(coolingCalculation, coolingFansBaseLine, coolingVentilation);
    var esmCoolingFans = new EECalcCoolingFansAndPumpsOracle()
        .Calculate(coolingCalculation, coolingFansEsm, coolingVentilation);

    var currentLighting = new EECalcLightingDevicesOracle()
        .Calculate(heatingCalculation, lightingBaseLine, heatingRows: null);
    var esmLighting = new EECalcLightingDevicesOracle()
        .Calculate(heatingCalculation, lightingEsm, heatingRows: null);

    var rows = new[]
    {
        FullProjectRow("Heating", currentHeating, currentHeating, esmHeating),
        FullProjectRow("Cooling", currentCooling, currentCooling, esmCooling),
        FullProjectRow("Ventilation heating", currentHeatingVentilation, currentHeatingVentilation, esmHeatingVentilation),
        FullProjectRow("Ventilation cooling", currentCoolingVentilation, currentCoolingVentilation, esmCoolingVentilation),
        FullProjectRow("DHW", dhwWithoutSolar.ResultNeededEnergy, dhwWithoutSolar.ResultNeededEnergy, dhwWithSolar.ResultNeededEnergy),
        FullProjectRow("DHW pumps", 0.0, 0.0, esmDhwPumps),
        FullProjectRow(
            "Fans and pumps",
            currentHeatingFans.NeededEnergy + currentCoolingFans.NeededEnergy,
            currentHeatingFans.NeededEnergy + currentCoolingFans.NeededEnergy,
            esmHeatingFans.NeededEnergy + esmCoolingFans.NeededEnergy),
        FullProjectRow("Lighting", currentLighting.LightsGeneralNeededEnergy, currentLighting.LightsGeneralNeededEnergy, esmLighting.LightsGeneralNeededEnergy),
        FullProjectRow(
            "Devices affecting heat balance",
            currentLighting.BalancedDevicesGeneralNeededEnergy,
            currentLighting.BalancedDevicesGeneralNeededEnergy,
            currentLighting.BalancedDevicesGeneralNeededEnergy),
        FullProjectRow(
            "Devices not affecting heat balance",
            currentLighting.NonBalancedDevicesGeneralNeededEnergy,
            currentLighting.NonBalancedDevicesGeneralNeededEnergy,
            currentLighting.NonBalancedDevicesGeneralNeededEnergy),
        FullProjectRow(
            "Other",
            currentCoolingFans.OtherNeededEnergy,
            currentCoolingFans.OtherNeededEnergy,
            currentCoolingFans.OtherNeededEnergy)
    };

    Console.WriteLine($"Full project energy verification. Climate zone {heatingCalculation.ClimateZoneId}.");
    Console.WriteLine("Needed energy by component, kWh/m2");
    Console.WriteLine("Component | Current | Normalized | ESM | Saving");
    Console.WriteLine("----------|---------|------------|-----|-------");
    foreach (var row in rows)
    {
        Console.WriteLine(string.Join(
            " | ",
            row.Name,
            Format(row.Current),
            Format(row.Normalized),
            Format(row.Esm),
            Format(row.Current - row.Esm)));
    }

    Console.WriteLine("----------|---------|------------|-----|-------");
    Console.WriteLine(string.Join(
        " | ",
        "Total",
        Format(rows.Sum(row => row.Current)),
        Format(rows.Sum(row => row.Normalized)),
        Format(rows.Sum(row => row.Esm)),
        Format(rows.Sum(row => row.Current) - rows.Sum(row => row.Esm))));

    Console.WriteLine();
    Console.WriteLine("Needed energy by component, software table layout");
    Console.WriteLine("Component | Current kWh/m2 | Current kWh/year | Normalized kWh/m2 | Normalized kWh/year | ESM kWh/m2 | ESM kWh/year");
    Console.WriteLine("----------|----------------|------------------|-------------------|---------------------|------------|--------------");
    foreach (var row in rows)
    {
        PrintFullProjectSoftwareRow(row, heatingCalculation.HeatedArea);
    }

    var totalRow = FullProjectRow(
        "Total",
        rows.Sum(row => row.Current),
        rows.Sum(row => row.Normalized),
        rows.Sum(row => row.Esm));
    Console.WriteLine("----------|----------------|------------------|-------------------|---------------------|------------|--------------");
    PrintFullProjectSoftwareRow(totalRow, heatingCalculation.HeatedArea);

    Console.WriteLine();
    Console.WriteLine("ECM aggregators used for measure rows:");
    Console.WriteLine("- EecalcEnvelopeEsmCalculator: heating envelope U measures");
    Console.WriteLine("- EecalcCoolingEsmCalculator: cooling envelope U measures");
    Console.WriteLine("- EecalcVentilationEsmCalculator: heating/cooling ventilation");
    Console.WriteLine("- EecalcLightingDevicesEsmCalculator: lighting/devices");
    Console.WriteLine("- EecalcFansPumpsEsmCalculator: heating/cooling fans and pumps");
    Console.WriteLine("- EECalcDhwBgvOracle: DHW and solar pump final-energy rows");
}

static void RunCoolingDetailsVerification(
    EecalcEnvelopeFixture coolingSource,
    EecalcValidationFixture coolingCalculation,
    double coolingFlowTemperature,
    double coolingEsmFlowTemperature,
    double coolingVentilationDebit,
    double coolingEsmFreeDebit)
{
    var baseLineUValues = new EecalcEnvelopeUValues
    {
        OuterWallsU = 3.000,
        WindowsU = 2.000,
        NonTransparentRoofU = 2.000,
        FloorU = 1.000
    };
    var esmUValues = new EecalcEnvelopeUValues
    {
        OuterWallsU = 0.250,
        WindowsU = 1.100,
        NonTransparentRoofU = 0.250,
        FloorU = 0.500
    };
    var coolingEfficiency = new EECalcEfficiencyChain
    {
        TransmitTempEfficiency = 100.0,
        SupplyNetEfficiency = 100.0,
        Automatic = 97.0,
        EnergyManagement = 96.0,
        GeneratorEfficiency = 100.0
    };
    var coolingVentilation = BuildCoolingVentilationFor(coolingCalculation, coolingFlowTemperature, coolingVentilationDebit);
    var baseLine = EecalcEnvelopeEsmCalculator.CreateEnvelopeState(coolingSource, baseLineUValues);
    var esmEnvelopeOnly = EecalcEnvelopeEsmCalculator.CreateEnvelopeState(baseLine, esmUValues);
    var esmCoolingCalculation = CloneCalculationWithCoolingLoads(
        coolingCalculation,
        lightsCoolingPower: 0.40,
        lightsCoolingWorkSchedule: 40.0,
        ventilationDebit: coolingEsmFreeDebit);
    var esm = WithCalculation(
        EecalcEnvelopeEsmCalculator.CreateEnvelopeState(coolingSource, esmUValues),
        esmCoolingCalculation);

    var esmCoolingVentilation = BuildCoolingVentilationFor(esm.Calculation, coolingEsmFlowTemperature, coolingVentilationDebit);
    var baseState = CalculateCoolingState(baseLine, coolingVentilation, coolingEfficiency);
    var esmEnvelopeOnlyState = CalculateCoolingState(esmEnvelopeOnly, coolingVentilation, coolingEfficiency);
    var esmState = CalculateCoolingState(esm, esmCoolingVentilation, coolingEfficiency);
    var savings = new EecalcEnvelopeSavingsOracle().Calculate(
        baseLine,
        esm,
        fixture => CalculateCoolingState(fixture, coolingVentilation, coolingEfficiency).NeededEnergy);

    Console.WriteLine($"Cooling diagnostics. Climate zone {coolingCalculation.ClimateZoneId}.");
    Console.WriteLine($"Core cooling flow temperature: {Format(coolingCalculation.FlowTemperature)}.");
    Console.WriteLine($"Core no-treatment cooling debit: {Format(coolingCalculation.VentilationDebit)}.");
    Console.WriteLine($"Core no-treatment cooling debit ESM: {Format(coolingEsmFreeDebit)}.");
    Console.WriteLine($"Ventilation cooling flow temperature: current {Format(coolingFlowTemperature)}, ESM {Format(coolingEsmFlowTemperature)}.");
    Console.WriteLine($"Ventilation cooling debit: {Format(coolingVentilationDebit)}.");
    Console.WriteLine("Energy rows, kWh/m2");
    Console.WriteLine("Row | Current | Normalized | ESM envelope only | ESM final");
    Console.WriteLine("----|---------|------------|-------------------|----------");
    PrintCoolingStateRow("Net energy without inputs", baseState.NoInputsNetEnergy, baseState.NoInputsNetEnergy, esmEnvelopeOnlyState.NoInputsNetEnergy, esmState.NoInputsNetEnergy);
    PrintCoolingStateRow("Cooling inputs", baseState.CoolingInputs, baseState.CoolingInputs, esmEnvelopeOnlyState.CoolingInputs, esmState.CoolingInputs);
    PrintCoolingStateRow("Ventilation inputs", baseState.VentilationInputs, baseState.VentilationInputs, esmEnvelopeOnlyState.VentilationInputs, esmState.VentilationInputs);
    PrintCoolingStateRow("Net energy", baseState.NetEnergy, baseState.NetEnergy, esmEnvelopeOnlyState.NetEnergy, esmState.NetEnergy);
    PrintCoolingStateRow("Needed/source energy EI1", baseState.NeededEnergy, baseState.NeededEnergy, esmEnvelopeOnlyState.NeededEnergy, esmState.NeededEnergy);

    PrintCoolingTargetInference(baseState, coolingEfficiency, softwareNeeded: 4138.296 / 1000.0);
    PrintCoolingRawDetails(baseLine);
    PrintFreeCoolingDetails(baseLine, esm);
    PrintCoolingExactReconciliation(baseLine, esm, coolingVentilation, esmCoolingVentilation, coolingEfficiency);
    PrintCoolingVentilationEnergyDetails(coolingCalculation, esm.Calculation, coolingVentilation, esmCoolingVentilation);

    Console.WriteLine();
    Console.WriteLine("Cooling U-measure distribution, source/needed kWh/m2");
    Console.WriteLine("Measure | Current U | ESM U | Virtual saving | Actual saving");
    Console.WriteLine("--------|-----------|-------|----------------|--------------");
    foreach (var item in savings.Items)
    {
        Console.WriteLine(string.Join(
            " | ",
            CoolingMeasureName(item.Tag),
            Format(item.OldValue),
            Format(item.NewValue),
            Format(item.VirtualSaving),
            Format(item.ActualSaving)));
    }

    Console.WriteLine("--------|-----------|-------|----------------|--------------");
    Console.WriteLine(string.Join(
        " | ",
        "Total",
        string.Empty,
        string.Empty,
        Format(savings.Items.Sum(item => item.VirtualSaving)),
        Format(savings.TotalSaving)));
}

static void PrintCoolingStateRow(
    string row,
    double current,
    double normalized,
    double esmEnvelopeOnly,
    double esm)
{
    Console.WriteLine(string.Join(
        " | ",
        row,
        Format(current),
        Format(normalized),
        Format(esmEnvelopeOnly),
        Format(esm)));
}

static void PrintCoolingExactReconciliation(
    EecalcEnvelopeFixture baseLine,
    EecalcEnvelopeFixture esm,
    EECalcVentilationInput coolingVentilationInput,
    EECalcVentilationInput esmCoolingVentilationInput,
    EECalcEfficiencyChain coolingEfficiency)
{
    var baseCleanVentilation = new EECalcVentilationOracle().Calculate(baseLine.Calculation, coolingVentilationInput);
    var esmCleanVentilation = new EECalcVentilationOracle().Calculate(esm.Calculation, esmCoolingVentilationInput);
    var baseDecompiledVentilationRows = CalculateCoolingVentilationInputsDecompiledStyle(baseLine.Calculation, coolingVentilationInput);
    var esmDecompiledVentilationRows = CalculateCoolingVentilationInputsDecompiledStyle(esm.Calculation, esmCoolingVentilationInput);

    var baseCleanVentilationInputs = baseCleanVentilation.Rows.Sum(row => row.CoolingInputs);
    var esmCleanVentilationInputs = esmCleanVentilation.Rows.Sum(row => row.CoolingInputs);
    var baseDecompiledVentilationInputs = baseDecompiledVentilationRows.Sum(row => row.CoolingInputs);
    var esmDecompiledVentilationInputs = esmDecompiledVentilationRows.Sum(row => row.CoolingInputs);

    var baseCoolingClean = new EecalcMonthlyCoolingOracle().Calculate(baseLine, baseCleanVentilationInputs);
    var esmCoolingClean = new EecalcMonthlyCoolingOracle().Calculate(esm, esmCleanVentilationInputs);
    var baseCoolingDecompiledVent = new EecalcMonthlyCoolingOracle().Calculate(baseLine, baseDecompiledVentilationInputs);
    var esmCoolingDecompiledVent = new EecalcMonthlyCoolingOracle().Calculate(esm, esmDecompiledVentilationInputs);
    var efficiency = EECalcMath.EfficiencyProduct(coolingEfficiency);

    Console.WriteLine();
    Console.WriteLine("Cooling exact reconciliation, kWh/m2");
    Console.WriteLine("State | NoInputs | FreeCooling | Vent clean | Vent decompiled-loop | Net clean | Needed clean | Needed decompiled-loop | Needed diff");
    Console.WriteLine("------|----------|-------------|------------|----------------------|-----------|--------------|------------------------|------------");
    PrintCoolingExactRow("Current", baseCoolingClean, baseCoolingDecompiledVent, efficiency);
    PrintCoolingExactRow("ESM", esmCoolingClean, esmCoolingDecompiledVent, efficiency);

    Console.WriteLine();
    Console.WriteLine("Cooling ventilation inputs by month, clean oracle vs decompiled-loop");
    Console.WriteLine("Month | Work | Sat | Sun | Hol | Clean input | Decompiled-loop input | Diff");
    Console.WriteLine("------|------|-----|-----|-----|-------------|-----------------------|-----");
    foreach (var cleanRow in baseCleanVentilation.Rows)
    {
        var decompiledRow = baseDecompiledVentilationRows.First(row => row.Month == cleanRow.Month);
        Console.WriteLine(string.Join(
            " | ",
            cleanRow.Month.ToString(CultureInfo.InvariantCulture),
            decompiledRow.WorkDays.ToString(CultureInfo.InvariantCulture),
            decompiledRow.Saturdays.ToString(CultureInfo.InvariantCulture),
            decompiledRow.Sundays.ToString(CultureInfo.InvariantCulture),
            decompiledRow.Holidays.ToString(CultureInfo.InvariantCulture),
            Format6(cleanRow.CoolingInputs),
            Format6(decompiledRow.CoolingInputs),
            Format6(cleanRow.CoolingInputs - decompiledRow.CoolingInputs)));
    }

    Console.WriteLine("------|------|-----|-----|-----|-------------|-----------------------|-----");
    Console.WriteLine(string.Join(
        " | ",
        "Total",
        baseDecompiledVentilationRows.Sum(row => row.WorkDays).ToString(CultureInfo.InvariantCulture),
        baseDecompiledVentilationRows.Sum(row => row.Saturdays).ToString(CultureInfo.InvariantCulture),
        baseDecompiledVentilationRows.Sum(row => row.Sundays).ToString(CultureInfo.InvariantCulture),
        baseDecompiledVentilationRows.Sum(row => row.Holidays).ToString(CultureInfo.InvariantCulture),
        Format6(baseCleanVentilationInputs),
        Format6(baseDecompiledVentilationInputs),
        Format6(baseCleanVentilationInputs - baseDecompiledVentilationInputs)));

}

static void PrintFreeCoolingDetails(
    EecalcEnvelopeFixture current,
    EecalcEnvelopeFixture esm)
{
    var currentCooling = new EecalcMonthlyCoolingOracle().Calculate(current);
    var esmCooling = new EecalcMonthlyCoolingOracle().Calculate(esm);

    Console.WriteLine();
    Console.WriteLine("No-treatment/free-cooling inputs by month, kWh/m2");
    Console.WriteLine("State | Month | Work | Sat | Sun | Hol | Qfree");
    Console.WriteLine("------|-------|------|-----|-----|-----|------");
    foreach (var row in currentCooling.Rows)
    {
        PrintFreeCoolingDetailsRow("Current", row);
    }

    foreach (var row in esmCooling.Rows)
    {
        PrintFreeCoolingDetailsRow("ESM", row);
    }

    Console.WriteLine("------|-------|------|-----|-----|-----|------");
    Console.WriteLine(string.Join(
        " | ",
        "Total",
        string.Empty,
        currentCooling.Rows.Sum(row => row.WorkDays).ToString(CultureInfo.InvariantCulture),
        currentCooling.Rows.Sum(row => row.Saturdays).ToString(CultureInfo.InvariantCulture),
        currentCooling.Rows.Sum(row => row.Sundays).ToString(CultureInfo.InvariantCulture),
        currentCooling.Rows.Sum(row => row.Holidays).ToString(CultureInfo.InvariantCulture),
        Format6(currentCooling.ResultCoolingInputs)));
}

static void PrintCoolingRawDetails(EecalcEnvelopeFixture current)
{
    var currentCooling = new EecalcMonthlyCoolingOracle().Calculate(current);

    Console.WriteLine();
    Console.WriteLine("Cooling raw details by month, kWh before area normalization");
    Console.WriteLine("Month | Qgain | Qloss | Eta | LatOcc | LatInf | LatVent | QcoolRaw | Qfree");
    Console.WriteLine("------|-------|-------|-----|--------|--------|---------|----------|------");
    foreach (var row in currentCooling.Rows)
    {
        Console.WriteLine(string.Join(
            " | ",
            row.Month.ToString(CultureInfo.InvariantCulture),
            Format6(row.Qgain),
            Format6(row.Qloss),
            Format6(row.Eta),
            Format6(row.QLatentOccupants),
            Format6(row.QLatentInf),
            Format6(row.QLatentVent),
            Format6(row.QcoolRaw),
            Format6(row.QfreeCooling)));
    }

    Console.WriteLine("------|-------|-------|-----|--------|--------|---------|----------|------");
    Console.WriteLine(string.Join(
        " | ",
        "Total/m2",
        string.Empty,
        string.Empty,
        string.Empty,
        Format6(currentCooling.Rows.Sum(row => row.QLatentOccupants) / current.Calculation.HeatedArea),
        Format6(currentCooling.Rows.Sum(row => row.QLatentInf) / current.Calculation.HeatedArea),
        Format6(currentCooling.Rows.Sum(row => row.QLatentVent) / current.Calculation.HeatedArea),
        Format6(currentCooling.ResultNoInputsNetEnergy),
        Format6(currentCooling.ResultCoolingInputs)));
}

static void PrintCoolingTargetInference(
    (double NoInputsNetEnergy, double CoolingInputs, double VentilationInputs, double NetEnergy, double NeededEnergy) state,
    EECalcEfficiencyChain coolingEfficiency,
    double softwareNeeded)
{
    var efficiency = EECalcMath.EfficiencyProduct(coolingEfficiency);
    var softwareNet = softwareNeeded * efficiency;
    var inferredFreeWithOurNoInputs =
        state.NoInputsNetEnergy - state.VentilationInputs - softwareNet;
    var inferredNoInputsWithOurFree =
        softwareNet + state.CoolingInputs + state.VentilationInputs;

    Console.WriteLine();
    Console.WriteLine("Cooling target inference from supplied software result");
    Console.WriteLine("Metric | Value");
    Console.WriteLine("-------|------");
    Console.WriteLine($"Software needed/source EI1 | {Format6(softwareNeeded)}");
    Console.WriteLine($"Software implied net energy | {Format6(softwareNet)}");
    Console.WriteLine($"Our no-inputs | {Format6(state.NoInputsNetEnergy)}");
    Console.WriteLine($"Our free-cooling input | {Format6(state.CoolingInputs)}");
    Console.WriteLine($"Our ventilation input | {Format6(state.VentilationInputs)}");
    Console.WriteLine($"Free-cooling required if our no-inputs is right | {Format6(inferredFreeWithOurNoInputs)}");
    Console.WriteLine($"No-inputs required if our free-cooling is right | {Format6(inferredNoInputsWithOurFree)}");
    Console.WriteLine($"Current missing net reduction | {Format6(state.NetEnergy - softwareNet)}");
}

static void PrintFreeCoolingDetailsRow(
    string state,
    EecalcMonthlyCoolingOracleRow row)
{
    Console.WriteLine(string.Join(
        " | ",
        state,
        row.Month.ToString(CultureInfo.InvariantCulture),
        row.WorkDays.ToString(CultureInfo.InvariantCulture),
        row.Saturdays.ToString(CultureInfo.InvariantCulture),
        row.Sundays.ToString(CultureInfo.InvariantCulture),
        row.Holidays.ToString(CultureInfo.InvariantCulture),
        Format6(row.QfreeCooling)));
}

static void PrintCoolingExactRow(
    string state,
    EecalcMonthlyCoolingOracleResult clean,
    EecalcMonthlyCoolingOracleResult decompiledVent,
    double efficiency)
{
    var cleanNeeded = EECalcMath.DivideByEfficiency(clean.ResultNetEnergy, efficiency);
    var decompiledNeeded = EECalcMath.DivideByEfficiency(decompiledVent.ResultNetEnergy, efficiency);

    Console.WriteLine(string.Join(
        " | ",
        state,
        Format6(clean.ResultNoInputsNetEnergy),
        Format6(clean.ResultCoolingInputs),
        Format6(clean.ResultVentilationInputs),
        Format6(decompiledVent.ResultVentilationInputs),
        Format6(clean.ResultNetEnergy),
        Format6(cleanNeeded),
        Format6(decompiledNeeded),
        Format6(cleanNeeded - decompiledNeeded)));
}

static IReadOnlyList<(int Month, int WorkDays, int Saturdays, int Sundays, int Holidays, double CoolingInputs)>
    CalculateCoolingVentilationInputsDecompiledStyle(
        EecalcValidationFixture fixture,
        EECalcVentilationInput coolingVentilationInput)
{
    var months = new EecalcMonthlyDaysOracle().Calculate(fixture);
    return months
        .Select(month => (
            month.Month,
            month.WorkDays,
            month.Saturdays,
            month.Sundays,
            month.Holidays,
            CoolingInputDayDecompiledStyle(
                coolingVentilationInput.WorkdaySchedule,
                fixture.WorkdaySchedule,
                month.WorkDays,
                inclusiveCoolingEnd: false,
                fixture,
                coolingVentilationInput)
            + CoolingInputDayDecompiledStyle(
                coolingVentilationInput.SaturdaySchedule,
                fixture.SaturdaySchedule,
                month.Saturdays,
                inclusiveCoolingEnd: true,
                fixture,
                coolingVentilationInput)
            + CoolingInputDayDecompiledStyle(
                coolingVentilationInput.SundaySchedule,
                fixture.SundaySchedule,
                month.Sundays,
                inclusiveCoolingEnd: true,
                fixture,
                coolingVentilationInput)))
        .ToList();
}

static void PrintCoolingVentilationEnergyDetails(
    EecalcValidationFixture currentCalculation,
    EecalcValidationFixture esmCalculation,
    EECalcVentilationInput currentCoolingVentilation,
    EECalcVentilationInput esmCoolingVentilation)
{
    var current = new EECalcVentilationOracle().Calculate(
        currentCalculation,
        WithCoolingVentilationGenerator280(currentCoolingVentilation));
    var esm = new EECalcVentilationOracle().Calculate(
        esmCalculation,
        WithCoolingVentilationGenerator280(esmCoolingVentilation));

    Console.WriteLine();
    Console.WriteLine("Ventilation cooling details, kWh/m2");
    Console.WriteLine("Row | Current | Normalized | ESM");
    Console.WriteLine("----|---------|------------|----");
    PrintVentCoolingDetailsRow("Air cooling energy", current.ResultEnergyForCooling, current.ResultEnergyForCooling, esm.ResultEnergyForCooling);
    PrintVentCoolingDetailsRow("Air heating energy", current.Rows.Sum(row => row.PowHeating), current.Rows.Sum(row => row.PowHeating), esm.Rows.Sum(row => row.PowHeating));
    PrintVentCoolingDetailsRow("Air withering energy", current.ResultEnergyForWithering, current.ResultEnergyForWithering, esm.ResultEnergyForWithering);
    PrintVentCoolingDetailsRow("Cooling contribution", current.Rows.Sum(row => row.CoolingInputs), current.Rows.Sum(row => row.CoolingInputs), esm.Rows.Sum(row => row.CoolingInputs));
    PrintVentCoolingDetailsRow("Total needed energy", current.CoolingNeededEnergy, current.CoolingNeededEnergy, esm.CoolingNeededEnergy);
}

static void PrintVentCoolingDetailsRow(
    string row,
    double current,
    double normalized,
    double esm)
{
    Console.WriteLine(string.Join(
        " | ",
        row,
        Format(current),
        Format(normalized),
        Format(esm)));
}

static EECalcVentilationInput WithCoolingVentilationGenerator280(EECalcVentilationInput source)
{
    return EecalcVentilationEsmCalculator.Clone(
        source,
        coolingEfficiency1: new EECalcEfficiencyChain
        {
            TransmitTempEfficiency = source.CoolingEfficiency1.TransmitTempEfficiency,
            SupplyNetEfficiency = source.CoolingEfficiency1.SupplyNetEfficiency,
            Automatic = 97.0,
            EnergyManagement = source.CoolingEfficiency1.EnergyManagement,
            GeneratorEfficiency = 280.0
        });
}

static double CoolingInputDayDecompiledStyle(
    EecalcDailySchedule ventilationSchedule,
    EecalcDailySchedule coolingSchedule,
    int dayCount,
    bool inclusiveCoolingEnd,
    EecalcValidationFixture fixture,
    EECalcVentilationInput coolingVentilationInput)
{
    var result = 0.0;
    for (var hour = ventilationSchedule.StartHour; hour < ventilationSchedule.EndHour; hour++)
    {
        var isProjectHour = hour >= coolingSchedule.StartHour
            && (inclusiveCoolingEnd ? hour <= coolingSchedule.EndHour : hour < coolingSchedule.EndHour);
        var selectedTemperature = isProjectHour
            ? fixture.ProjectTemperature
            : fixture.NonProjectTemperature;
        result += coolingVentilationInput.Debit
            * 0.34
            * (selectedTemperature - coolingVentilationInput.FlowTemperature)
            / 1000.0;
    }

    return result * dayCount;
}

static (double NoInputsNetEnergy, double CoolingInputs, double VentilationInputs, double NetEnergy, double NeededEnergy)
    CalculateCoolingState(
        EecalcEnvelopeFixture fixture,
        EECalcVentilationInput coolingVentilationInput,
        EECalcEfficiencyChain coolingEfficiency)
{
    var ventilation = new EECalcVentilationOracle().Calculate(fixture.Calculation, coolingVentilationInput);
    var ventilationInputs = ventilation.Rows.Sum(row => row.CoolingInputs);
    var cooling = new EecalcMonthlyCoolingOracle().Calculate(fixture, ventilationInputs);
    var needed = EECalcMath.DivideByEfficiency(
        cooling.ResultNetEnergy,
        EECalcMath.EfficiencyProduct(coolingEfficiency));
    return (
        cooling.ResultNoInputsNetEnergy,
        cooling.ResultCoolingInputs,
        cooling.ResultVentilationInputs,
        cooling.ResultNetEnergy,
        needed);
}

static string CoolingMeasureName(string tag)
{
    return tag switch
    {
        "UouterWalls" => "Outer walls",
        "Uwindows" => "Windows",
        "Unontransparent" => "Non-transparent roof",
        "Ufloor" => "Floor",
        _ => tag
    };
}

static (string Name, double Current, double Normalized, double Esm) FullProjectRow(
    string name,
    double current,
    double normalized,
    double esm)
{
    return (name, current, normalized, esm);
}

static EECalcVentilationInput BuildCoolingVentilationFor(
    EecalcValidationFixture coolingCalculation,
    double flowTemperature,
    double debit = 0.500)
{
    return EecalcVentilationEsmCalculator.Clone(
        TestFixture.BuildCoolingVentilation(),
        debit: debit,
        flowTemperature: flowTemperature);
}

static void PrintFullProjectSoftwareRow(
    (string Name, double Current, double Normalized, double Esm) row,
    double area)
{
    Console.WriteLine(string.Join(
        " | ",
        row.Name,
        Format(row.Current),
        Format(row.Current * area),
        Format(row.Normalized),
        Format(row.Normalized * area),
        Format(row.Esm),
        Format(row.Esm * area)));
}

static double CalculateHeatingNeeded(
    EecalcEnvelopeFixture fixture,
    EECalcVentilationInput ventilationInput,
    EECalcLightingDevicesInput lightingInput)
{
    var heatingRows = new EecalcMonthlyHeatingOracle().Calculate(fixture);
    var ventilation = new EECalcVentilationOracle().Calculate(fixture.Calculation, ventilationInput);
    var lighting = new EECalcLightingDevicesOracle().Calculate(fixture.Calculation, lightingInput, heatingRows);

    return heatingRows.Sum(row => row.FinalQnd)
        - ventilation.Rows.Sum(row => row.HeatingInputs)
        - lighting.ResulLightInputs
        - lighting.ResulAppliancesInputs;
}

static double CalculateCoolingNeeded(
    EecalcEnvelopeFixture fixture,
    EECalcVentilationInput coolingVentilationInput)
{
    var ventilation = new EECalcVentilationOracle().Calculate(fixture.Calculation, coolingVentilationInput);
    var ventilationInputs = ventilation.Rows.Sum(row => row.CoolingInputs);
    return new EecalcMonthlyCoolingOracle().Calculate(fixture, ventilationInputs).ResultNetEnergy;
}

static double CalculateCoolingNeededSource(
    EecalcEnvelopeFixture fixture,
    EECalcVentilationInput coolingVentilationInput)
{
    var netEnergy = CalculateCoolingNeeded(fixture, coolingVentilationInput);
    return EECalcMath.DivideByEfficiency(
        netEnergy,
        EECalcMath.EfficiencyProduct(coolingVentilationInput.CoolingEfficiency1));
}

static EecalcEnvelopeFixture WithCalculation(
    EecalcEnvelopeFixture source,
    EecalcValidationFixture calculation)
{
    return new EecalcEnvelopeFixture
    {
        Id = source.Id,
        Calculation = calculation,
        NorthWalls = source.NorthWalls,
        NorthEastWalls = source.NorthEastWalls,
        EastWalls = source.EastWalls,
        SouthEastWalls = source.SouthEastWalls,
        SouthWalls = source.SouthWalls,
        SouthWestWalls = source.SouthWestWalls,
        WestWalls = source.WestWalls,
        NorthWestWalls = source.NorthWestWalls,
        Roof = source.Roof,
        Floor = source.Floor
    };
}

static EecalcValidationFixture CloneCalculationWithCoolingLoads(
    EecalcValidationFixture source,
    double? lightsCoolingPower = null,
    double? balancedDevicesCoolingPower = null,
    double? lightsCoolingWorkSchedule = null,
    double? balancedDevicesCoolingWorkSchedule = null,
    double? flowTemperature = null,
    double? ventilationDebit = null)
{
    return new EecalcValidationFixture
    {
        Id = source.Id,
        Scenario = source.Scenario,
        ClimateZoneId = source.ClimateZoneId,
        FirstMonth = source.FirstMonth,
        LastMonth = source.LastMonth,
        FirstDay = source.FirstDay,
        LastDay = source.LastDay,
        HeatedArea = source.HeatedArea,
        HeatedVolume = source.HeatedVolume,
        Infiltration = source.Infiltration,
        HeatCapacity = source.HeatCapacity,
        MetabolicHeat = source.MetabolicHeat,
        LatentMetabolicHeat = source.LatentMetabolicHeat,
        ProjectTemperature = source.ProjectTemperature,
        NonProjectTemperature = source.NonProjectTemperature,
        ProjectHumidity = source.ProjectHumidity,
        FlowTemperature = flowTemperature ?? source.FlowTemperature,
        FlowRelativeHumidity = source.FlowRelativeHumidity,
        VentilationDebit = ventilationDebit ?? source.VentilationDebit,
        LightsCoolingPower = lightsCoolingPower ?? source.LightsCoolingPower,
        BalancedDevicesCoolingPower = balancedDevicesCoolingPower ?? source.BalancedDevicesCoolingPower,
        LightsCoolingWorkSchedule = lightsCoolingWorkSchedule ?? source.LightsCoolingWorkSchedule,
        BalancedDevicesCoolingWorkSchedule = balancedDevicesCoolingWorkSchedule ?? source.BalancedDevicesCoolingWorkSchedule,
        WorkdaySchedule = source.WorkdaySchedule,
        SaturdaySchedule = source.SaturdaySchedule,
        SundaySchedule = source.SundaySchedule,
        OccupantsWorkdaySchedule = source.OccupantsWorkdaySchedule,
        OccupantsSaturdaySchedule = source.OccupantsSaturdaySchedule,
        OccupantsSundaySchedule = source.OccupantsSundaySchedule,
        VentilationWorkdaySchedule = source.VentilationWorkdaySchedule,
        VentilationSaturdaySchedule = source.VentilationSaturdaySchedule,
        VentilationSundaySchedule = source.VentilationSundaySchedule,
        NightVentilationWorkdaySchedule = source.NightVentilationWorkdaySchedule,
        NightVentilationSaturdaySchedule = source.NightVentilationSaturdaySchedule,
        NightVentilationSundaySchedule = source.NightVentilationSundaySchedule,
        HolidaysByMonth = source.HolidaysByMonth,
        AverageOutdoorTemperatureByMonth = source.AverageOutdoorTemperatureByMonth,
        SolarRadiationByMonth = source.SolarRadiationByMonth,
        HourlyWeatherByMonth = source.HourlyWeatherByMonth
    };
}

static void RunEnvelopeSavingsVerification(EecalcEnvelopeFixture baseLine)
{
    var calculator = new EecalcEnvelopeEsmCalculator();
    var ventilationInput = TestFixture.BuildVentilation();
    var lightingInput = TestFixture.BuildLightingAndDevices();
    var baseLineUValues = new EecalcEnvelopeUValues
    {
        OuterWallsU = 3.000,
        WindowsU = 2.000,
        NonTransparentRoofU = 2.000,
        FloorU = 1.000
    };
    var esmUValues = new EecalcEnvelopeUValues
    {
        OuterWallsU = 0.250,
        WindowsU = 1.100,
        NonTransparentRoofU = 0.250,
        FloorU = 0.500
    };

    var combined = calculator.CalculateSavings(
        baseLine,
        baseLineUValues,
        esmUValues,
        ventilationInput,
        lightingInput);
    AssertClose(
        combined.TotalSaving,
        combined.Items.Sum(item => item.ActualSaving),
        "combined actual savings must sum to total baseline-to-ESM saving");
    AssertClose(
        1.0,
        combined.Items.Sum(item => item.Part),
        "combined parts must sum to 100%");

    foreach (var item in combined.Items)
    {
        if (double.IsNaN(item.Part) || double.IsInfinity(item.Part))
        {
            throw new InvalidOperationException($"{item.Tag} produced an invalid part.");
        }
    }

    var savingsBaseLine = EecalcEnvelopeEsmCalculator.CreateEnvelopeState(baseLine, baseLineUValues);
    var singleWallEsm = EecalcEnvelopeEsmCalculator.CreateEnvelopeState(
        savingsBaseLine,
        new EecalcEnvelopeUValues
        {
            OuterWallsU = 0.250,
            WindowsU = baseLineUValues.WindowsU,
            NonTransparentRoofU = baseLineUValues.NonTransparentRoofU,
            FloorU = baseLineUValues.FloorU
        });
    var singleWall = new EecalcEnvelopeSavingsOracle().Calculate(
        savingsBaseLine,
        singleWallEsm,
        fixture => calculator.CalculateNetHeatingEnergyAfterInputs(fixture, ventilationInput, lightingInput));
    if (singleWall.Items.Count != 1 || singleWall.Items[0].Tag != "UouterWalls")
    {
        throw new InvalidOperationException("single wall ECM must produce exactly one UouterWalls saving row.");
    }

    AssertClose(1.0, singleWall.Items[0].Part, "single ECM part must be 100%");
    AssertClose(singleWall.TotalSaving, singleWall.Items[0].ActualSaving, "single ECM actual saving must equal total saving");

    Console.WriteLine("Envelope ECM savings verification passed.");
    Console.WriteLine($"Baseline energy: {Format(combined.BaseLineEnergy)} kWh/m2");
    Console.WriteLine($"ESM energy: {Format(combined.EsmEnergy)} kWh/m2");
    Console.WriteLine($"Total saving: {Format(combined.TotalSaving)} kWh/m2");
    Console.WriteLine("Tag | Virtual saving | Part % | Actual saving");
    Console.WriteLine("----|----------------|--------|--------------");
    foreach (var item in combined.Items)
    {
        Console.WriteLine(string.Join(
            " | ",
            item.Tag,
            Format(item.VirtualSaving),
            Format(item.Percent),
            Format(item.ActualSaving)));
    }
}

static void RunVentilationSavingsVerification(
    EecalcValidationFixture heatingCalculation,
    EecalcValidationFixture coolingCalculation)
{
    var calculator = new EecalcVentilationEsmCalculator();
    var heatingBaseLine = TestFixture.BuildVentilation();
    var heatingEsm = EecalcVentilationEsmCalculator.Clone(
        heatingBaseLine,
        debit: 0.350,
        firstRecEfficiency: 80.0,
        workdaySchedule: new EecalcDailySchedule { StartHour = 0, EndHour = 16 },
        saturdaySchedule: new EecalcDailySchedule { StartHour = 0, EndHour = 16 },
        sundaySchedule: new EecalcDailySchedule { StartHour = 0, EndHour = 0 });
    var heatingSavings = calculator.CalculateHeatingSavings(
        heatingCalculation,
        heatingBaseLine,
        heatingEsm);

    var coolingBaseLine = BuildCoolingVentilationFor(coolingCalculation, coolingCalculation.FlowTemperature);
    var coolingEsm = EecalcVentilationEsmCalculator.Clone(
        coolingBaseLine,
        debit: 0.350,
        flowTemperature: 24.0,
        coolingEfficiency1: new EECalcEfficiencyChain
        {
            TransmitTempEfficiency = coolingBaseLine.CoolingEfficiency1.TransmitTempEfficiency,
            SupplyNetEfficiency = coolingBaseLine.CoolingEfficiency1.SupplyNetEfficiency,
            Automatic = coolingBaseLine.CoolingEfficiency1.Automatic,
            EnergyManagement = 98.0,
            GeneratorEfficiency = coolingBaseLine.CoolingEfficiency1.GeneratorEfficiency
        },
        workdaySchedule: new EecalcDailySchedule { StartHour = 0, EndHour = 16 },
        saturdaySchedule: new EecalcDailySchedule { StartHour = 0, EndHour = 16 },
        sundaySchedule: new EecalcDailySchedule { StartHour = 0, EndHour = 16 });
    var coolingSavings = calculator.CalculateCoolingSavings(
        coolingCalculation,
        coolingBaseLine,
        coolingEsm);
    var singleHeatingDebit = calculator.CalculateHeatingSavings(
        heatingCalculation,
        heatingBaseLine,
        EecalcVentilationEsmCalculator.Clone(heatingBaseLine, debit: 0.350));
    var singleCoolingFlowTemperature = calculator.CalculateCoolingSavings(
        coolingCalculation,
        coolingBaseLine,
        EecalcVentilationEsmCalculator.Clone(coolingBaseLine, flowTemperature: 24.0));

    AssertSavingsAreConsistent(heatingSavings);
    AssertSavingsAreConsistent(coolingSavings);
    AssertSingleVentilationMeasure(singleHeatingDebit, "Debit");
    AssertSingleVentilationMeasure(singleCoolingFlowTemperature, "FlowTemperature");

    Console.WriteLine("Ventilation ECM savings verification passed.");
    PrintVentilationSavings("Heating ventilation", heatingSavings);
    PrintVentilationSavings("Cooling ventilation", coolingSavings);
}

static void RunComponentSavingsVerification(
    EecalcEnvelopeFixture heatingFixture,
    EecalcEnvelopeFixture coolingFixture,
    EecalcValidationFixture heatingCalculation,
    EecalcValidationFixture coolingCalculation,
    double coolingEsmFlowTemperature)
{
    var coolingCalculator = new EecalcCoolingEsmCalculator();
    var esmCoolingCalculation = CloneCalculationWithCoolingLoads(
        coolingCalculation,
        lightsCoolingPower: 0.40,
        lightsCoolingWorkSchedule: 40.0);
    var coolingSavings = coolingCalculator.CalculateNeededSavings(
        coolingFixture,
        new EecalcEnvelopeUValues
        {
            OuterWallsU = 3.000,
            WindowsU = 2.000,
            NonTransparentRoofU = 2.000,
            FloorU = 1.000
        },
        new EecalcEnvelopeUValues
        {
            OuterWallsU = 0.250,
            WindowsU = 1.100,
            NonTransparentRoofU = 0.250,
            FloorU = 0.500
        },
        BuildCoolingVentilationFor(coolingCalculation, coolingCalculation.FlowTemperature),
        esmCoolingCalculation);
    AssertClose(
        coolingSavings.TotalSaving,
        coolingSavings.Items.Sum(item => item.ActualSaving),
        "cooling core actual savings must sum to total baseline-to-ESM saving");

    var lightingCalculator = new EecalcLightingDevicesEsmCalculator();
    var lightingBaseLine = TestFixture.BuildLightingAndDevices();
    var lightingEsm = EecalcLightingDevicesEsmCalculator.Clone(
        lightingBaseLine,
        lights: EecalcLightingDevicesEsmCalculator.Clone(
            lightingBaseLine.Lights,
            generalPower: 0.40,
            generalWorkSchedule: 40.0));
    var lightingSavings = lightingCalculator.CalculateLightingSavings(
        heatingCalculation,
        lightingBaseLine,
        lightingEsm);
    AssertComponentSavingsAreConsistent(lightingSavings);

    var fansPumpsCalculator = new EecalcFansPumpsEsmCalculator();
    var heatingFansBaseLine = new EECalcHeatingFansAndPumpsInput
    {
        VentilatorsHeat = 0.60,
        PumpVentilationHeat = 0.00,
        HeatingPump = 0.30,
        EnergyManagement = 96.0
    };
    var heatingFansEsm = EecalcFansPumpsEsmCalculator.Clone(
        heatingFansBaseLine,
        ventilatorsHeat: 0.30,
        heatingPump: 0.15,
        energyManagement: 98.0);
    var heatingFansSavings = fansPumpsCalculator.CalculateHeatingSavings(
        heatingCalculation,
        TestFixture.BuildVentilation(),
        heatingFansBaseLine,
        heatingFansEsm);
    AssertComponentSavingsAreConsistent(heatingFansSavings);

    var coolingFansBaseLine = TestFixture.BuildCoolingFansAndPumps();
    var coolingFansEsm = EecalcFansPumpsEsmCalculator.Clone(
        coolingFansBaseLine,
        ventilatorsCool: 0.40,
        pumpVentilationCool: 0.10,
        coolingPump: 0.50,
        energyManagement: 98.0);
    var coolingFansSavings = fansPumpsCalculator.CalculateCoolingSavings(
        coolingCalculation,
        BuildCoolingVentilationFor(coolingCalculation, coolingCalculation.FlowTemperature),
        coolingFansBaseLine,
        coolingFansEsm);
    AssertComponentSavingsAreConsistent(coolingFansSavings);

    Console.WriteLine("Component ECM savings verification passed.");
    PrintEnvelopeSavings("Cooling core", coolingSavings);
    PrintComponentSavings(lightingSavings);
    PrintComponentSavings(heatingFansSavings);
    PrintComponentSavings(coolingFansSavings);
}

static void AssertComponentSavingsAreConsistent(EecalcComponentSavingsResult result)
{
    AssertClose(
        result.TotalSaving,
        result.Items.Sum(item => item.ActualSaving),
        $"{result.Technology} actual savings must sum to total baseline-to-ESM saving");

    if (result.Items.Count > 0)
    {
        AssertClose(1.0, result.Items.Sum(item => item.Part), $"{result.Technology} parts must sum to 100%");
    }

    foreach (var item in result.Items)
    {
        if (double.IsNaN(item.Part) || double.IsInfinity(item.Part))
        {
            throw new InvalidOperationException($"{result.Technology} {item.Tag} produced an invalid part.");
        }
    }
}

static void PrintEnvelopeSavings(string title, EecalcEnvelopeSavingsResult result)
{
    Console.WriteLine();
    Console.WriteLine(title);
    Console.WriteLine($"Baseline energy: {Format(result.BaseLineEnergy)} kWh/m2");
    Console.WriteLine($"ESM energy: {Format(result.EsmEnergy)} kWh/m2");
    Console.WriteLine($"Total saving: {Format(result.TotalSaving)} kWh/m2");
    Console.WriteLine("Tag | Virtual saving | Part % | Actual saving");
    Console.WriteLine("----|----------------|--------|--------------");
    foreach (var item in result.Items)
    {
        Console.WriteLine(string.Join(
            " | ",
            item.Tag,
            Format(item.VirtualSaving),
            Format(item.Percent),
            Format(item.ActualSaving)));
    }
}

static void PrintComponentSavings(EecalcComponentSavingsResult result)
{
    Console.WriteLine();
    Console.WriteLine(result.Technology);
    Console.WriteLine($"Baseline energy: {Format(result.BaseLineEnergy)} kWh/m2");
    Console.WriteLine($"ESM energy: {Format(result.EsmEnergy)} kWh/m2");
    Console.WriteLine($"Total saving: {Format(result.TotalSaving)} kWh/m2");
    Console.WriteLine("Tag | Virtual saving | Part % | Actual saving");
    Console.WriteLine("----|----------------|--------|--------------");
    foreach (var item in result.Items)
    {
        Console.WriteLine(string.Join(
            " | ",
            item.Tag,
            Format(item.VirtualSaving),
            Format(item.Percent),
            Format(item.ActualSaving)));
    }
}

static void AssertSingleVentilationMeasure(EecalcVentilationSavingsResult result, string expectedTag)
{
    if (result.Items.Count != 1 || result.Items[0].Tag != expectedTag)
    {
        throw new InvalidOperationException(
            $"{result.Mode} ventilation single ECM must produce exactly one {expectedTag} saving row.");
    }

    AssertClose(1.0, result.Items[0].Part, $"{result.Mode} ventilation single ECM part must be 100%");
    AssertClose(
        result.TotalSaving,
        result.Items[0].ActualSaving,
        $"{result.Mode} ventilation single ECM actual saving must equal total saving");
}

static void AssertSavingsAreConsistent(EecalcVentilationSavingsResult result)
{
    AssertClose(
        result.TotalSaving,
        result.Items.Sum(item => item.ActualSaving),
        $"{result.Mode} ventilation actual savings must sum to total baseline-to-ESM saving");

    if (result.Items.Count > 0)
    {
        AssertClose(
            1.0,
            result.Items.Sum(item => item.Part),
            $"{result.Mode} ventilation parts must sum to 100%");
    }

    foreach (var item in result.Items)
    {
        if (double.IsNaN(item.Part) || double.IsInfinity(item.Part))
        {
            throw new InvalidOperationException($"{result.Mode} ventilation {item.Tag} produced an invalid part.");
        }
    }
}

static void PrintVentilationSavings(string title, EecalcVentilationSavingsResult result)
{
    Console.WriteLine();
    Console.WriteLine(title);
    Console.WriteLine($"Baseline energy: {Format(result.BaseLineEnergy)} kWh/m2");
    Console.WriteLine($"ESM energy: {Format(result.EsmEnergy)} kWh/m2");
    Console.WriteLine($"Total saving: {Format(result.TotalSaving)} kWh/m2");
    Console.WriteLine("Tag | Virtual saving | Part % | Actual saving");
    Console.WriteLine("----|----------------|--------|--------------");
    foreach (var item in result.Items)
    {
        Console.WriteLine(string.Join(
            " | ",
            item.Tag,
            Format(item.VirtualSaving),
            Format(item.Percent),
            Format(item.ActualSaving)));
    }
}

static void AssertClose(double expected, double actual, string message)
{
    if (Math.Abs(expected - actual) > 0.000001)
    {
        throw new InvalidOperationException(
            $"{message}. Expected {expected.ToString(CultureInfo.InvariantCulture)}, actual {actual.ToString(CultureInfo.InvariantCulture)}.");
    }
}
