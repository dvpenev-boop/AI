using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace EE.Doklad.Services.EecalcClimate
{
    public sealed class LegacyEecalcXmlClimateDataProvider : IClimateDataProvider
    {
        private readonly Dictionary<int, ClimateZoneXmlData> _zonesByNumber;
        private readonly ClimateProviderMode _mode;

        public LegacyEecalcXmlClimateDataProvider(
            ClimateProviderMode mode = ClimateProviderMode.LegacyEECalcStrict,
            string? xmlPath = null)
        {
            if (mode == ClimateProviderMode.CurrentOrdinance)
            {
                throw new ArgumentException(
                    $"{nameof(LegacyEecalcXmlClimateDataProvider)} cannot be used for {mode}.",
                    nameof(mode));
            }

            _mode = mode;
            xmlPath ??= EecalcDataPathResolver.FindRequiredFile(
                "reference", "eecalc-config", "DefaultParams.xml");
            _zonesByNumber = Load(xmlPath);
        }

        public double GetMonthlyAvgTemp(int zoneId, Month month)
        {
            if (_mode == ClimateProviderMode.LegacyEECalcCorrectedData && month == Month.January)
            {
                return zoneId switch
                {
                    1 => 1.9,
                    2 => 0.5,
                    3 => 0.1,
                    _ => GetMonth(zoneId, month).AvgTemp
                };
            }

            return GetMonth(zoneId, month).AvgTemp;
        }

        public SolarRadiationData GetSolarRadiation(int zoneId, Month month)
        {
            var monthData = GetMonth(zoneId, month);
            return new SolarRadiationData(
                monthData.N,
                monthData.E,
                monthData.S,
                monthData.W,
                monthData.H);
        }

        public IReadOnlyList<HourlyClimateData> GetHourlyClimateData(int zoneId, Month month)
        {
            var zone = GetZone(zoneId);
            var monthIndex = ToMonthIndex(month);
            if (!zone.HourlyByMonth.TryGetValue(monthIndex, out var hourly))
            {
                throw new InvalidOperationException(
                    $"DefaultParams.xml does not contain hourly TempHumidity data for zone {zoneId}, month {monthIndex + 1}.");
            }

            return hourly;
        }

        public double GetPb(int zoneId)
        {
            return GetZone(zoneId).Pb;
        }

        private ClimateMonthXmlData GetMonth(int zoneId, Month month)
        {
            var zone = GetZone(zoneId);
            var monthIndex = ToMonthIndex(month);
            if (!zone.MonthsByIndex.TryGetValue(monthIndex, out var monthData))
            {
                throw new InvalidOperationException(
                    $"DefaultParams.xml does not contain SolarRadiation data for zone {zoneId}, month {monthIndex + 1}.");
            }

            return monthData;
        }

        private ClimateZoneXmlData GetZone(int zoneId)
        {
            var xmlNumber = ToXmlNumber(zoneId);
            if (!_zonesByNumber.TryGetValue(xmlNumber, out var zone))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(zoneId),
                    zoneId,
                    $"No DefaultParams.xml ClimateZone with Number={xmlNumber}.");
            }

            return zone;
        }

        private static Dictionary<int, ClimateZoneXmlData> Load(string xmlPath)
        {
            var document = XDocument.Load(xmlPath);
            var zones = document.Root?
                .Element("ClimateZones")?
                .Elements("ClimateZone")
                ?? Enumerable.Empty<XElement>();

            return zones.Select(ParseZone).ToDictionary(zone => zone.Number);
        }

        private static ClimateZoneXmlData ParseZone(XElement zoneElement)
        {
            var number = ReadInt(zoneElement, "Number");
            var pb = ReadDouble(zoneElement, "Pb");

            var months = zoneElement
                .Element("SolarRadiation")?
                .Element("Months")?
                .Elements("Month")
                .Select((monthElement, index) => new ClimateMonthXmlData(
                    index,
                    ReadDouble(monthElement, "AvgTemp"),
                    ReadDouble(monthElement, "N"),
                    ReadDouble(monthElement, "E"),
                    ReadDouble(monthElement, "S"),
                    ReadDouble(monthElement, "W"),
                    ReadDouble(monthElement, "H")))
                .ToDictionary(month => month.MonthIndex)
                ?? new Dictionary<int, ClimateMonthXmlData>();

            var hourlyByMonth = zoneElement
                .Element("TempHumidity")?
                .Element("Months")?
                .Elements("Month")
                .Select((monthElement, index) => new
                {
                    MonthIndex = index,
                    Hours = monthElement
                        .Element("Hours")?
                        .Elements()
                        .Select((hourElement, hourIndex) => new HourlyClimateData(
                            hourIndex,
                            ReadDouble(hourElement, "Temp"),
                            ReadDouble(hourElement, "Humidity")))
                        .ToArray() ?? Array.Empty<HourlyClimateData>()
                })
                .ToDictionary(item => item.MonthIndex, item => (IReadOnlyList<HourlyClimateData>)item.Hours)
                ?? new Dictionary<int, IReadOnlyList<HourlyClimateData>>();

            return new ClimateZoneXmlData(number, pb, months, hourlyByMonth);
        }

        private static int ToXmlNumber(int zoneId)
        {
            if (zoneId < 1 || zoneId > 9)
            {
                throw new ArgumentOutOfRangeException(nameof(zoneId), zoneId, "ZoneId must be 1..9.");
            }

            return zoneId - 1;
        }

        private static int ToMonthIndex(Month month)
        {
            var value = (int)month;
            if (value < 0 || value > 11)
            {
                throw new ArgumentOutOfRangeException(nameof(month), month, "Month must be January..December.");
            }

            return value;
        }

        private static int ReadInt(XElement parent, string name)
        {
            return int.Parse(ReadRequired(parent, name), CultureInfo.InvariantCulture);
        }

        private static double ReadDouble(XElement parent, string name)
        {
            return double.Parse(ReadRequired(parent, name), CultureInfo.InvariantCulture);
        }

        private static string ReadRequired(XElement parent, string name)
        {
            return parent.Element(name)?.Value
                ?? throw new InvalidOperationException($"Missing XML element '{name}'.");
        }

        private sealed record ClimateZoneXmlData(
            int Number,
            double Pb,
            IReadOnlyDictionary<int, ClimateMonthXmlData> MonthsByIndex,
            IReadOnlyDictionary<int, IReadOnlyList<HourlyClimateData>> HourlyByMonth);

        private sealed record ClimateMonthXmlData(
            int MonthIndex,
            double AvgTemp,
            double N,
            double E,
            double S,
            double W,
            double H);
    }
}
