using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;

namespace EE.Doklad.Services.EecalcClimate
{
    public sealed class LegacyEecalcXmlSunEnergyDataProvider : ISunEnergyDataProvider
    {
        private readonly Dictionary<int, SunZoneXmlData> _zonesByNumber;

        public LegacyEecalcXmlSunEnergyDataProvider(string? xmlPath = null)
        {
            xmlPath ??= EecalcDataPathResolver.FindRequiredFile(
                "reference", "eecalc-config", "DefaultSunParams.xml");
            _zonesByNumber = Load(xmlPath);
        }

        public double GetMonthlyAvgTemp(int zoneId, Month month)
        {
            return GetMonth(zoneId, month).AvgTemp;
        }

        public double GetMonthlyRadiation(int zoneId, Month month)
        {
            return GetMonth(zoneId, month).Radiation;
        }

        public double GetMonthlyCloudiness(int zoneId, Month month)
        {
            return GetMonth(zoneId, month).Cloudiness;
        }

        private SunMonthXmlData GetMonth(int zoneId, Month month)
        {
            var xmlNumber = ToXmlNumber(zoneId);
            if (!_zonesByNumber.TryGetValue(xmlNumber, out var zone))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(zoneId),
                    zoneId,
                    $"No DefaultSunParams.xml ClimateZone with Number={xmlNumber}.");
            }

            var monthIndex = ToMonthIndex(month);
            if (!zone.MonthsByIndex.TryGetValue(monthIndex, out var monthData))
            {
                throw new InvalidOperationException(
                    $"DefaultSunParams.xml does not contain SolarRadiation data for zone {zoneId}, month {monthIndex + 1}.");
            }

            return monthData;
        }

        private static Dictionary<int, SunZoneXmlData> Load(string xmlPath)
        {
            var document = XDocument.Load(xmlPath);
            var zones = document.Root?
                .Element("ClimateZones")?
                .Elements("ClimateZone")
                ?? Enumerable.Empty<XElement>();

            return zones.Select(ParseZone).ToDictionary(zone => zone.Number);
        }

        private static SunZoneXmlData ParseZone(XElement zoneElement)
        {
            var number = ReadInt(zoneElement, "Number");
            var months = zoneElement
                .Element("SolarRadiation")?
                .Element("Months")?
                .Elements("Month")
                .Select((monthElement, index) => new SunMonthXmlData(
                    index,
                    ReadDouble(monthElement, "AvgTemp"),
                    ReadDouble(monthElement, "Radiation"),
                    ReadDouble(monthElement, "Cloudiness")))
                .ToDictionary(month => month.MonthIndex)
                ?? new Dictionary<int, SunMonthXmlData>();

            return new SunZoneXmlData(number, months);
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

        private sealed record SunZoneXmlData(
            int Number,
            IReadOnlyDictionary<int, SunMonthXmlData> MonthsByIndex);

        private sealed record SunMonthXmlData(
            int MonthIndex,
            double AvgTemp,
            double Radiation,
            double Cloudiness);
    }
}
