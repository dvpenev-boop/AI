using System;
using EE.Doklad.Services;

namespace EE.Doklad.Sections.Section11Heating.Models;

public static class ClimateDatabase
{
    private static readonly ClimateService ClimateService = new(new JsonClimateRepository());

    public static readonly int[] DtHours =
        { 744, 672, 744, 720, 744, 720, 744, 744, 720, 744, 720, 744 };

    public static readonly string[] MonthNames =
    {
        "\u042F\u043D\u0443",
        "\u0424\u0435\u0432",
        "\u041C\u0430\u0440",
        "\u0410\u043F\u0440",
        "\u041C\u0430\u0439",
        "\u042E\u043D\u0438",
        "\u042E\u043B\u0438",
        "\u0410\u0432\u0433",
        "\u0421\u0435\u043F",
        "\u041E\u043A\u0442",
        "\u041D\u043E\u0432",
        "\u0414\u0435\u043A"
    };

    public static double GetTe(int zone, int monthIndex)
    {
        var climateZone = ClimateService.GetZone(Math.Clamp(zone, 1, 9));
        if (climateZone?.Monthly?.AvgMonthlyTempC == null || monthIndex < 0 || monthIndex >= 12)
        {
            return 0.0;
        }

        return climateZone.Monthly.AvgMonthlyTempC[monthIndex];
    }

    public static double GetIsol(int zone, string orientation, int monthIndex)
    {
        var climateZone = ClimateService.GetZone(Math.Clamp(zone, 1, 9));
        if (climateZone?.Monthly?.AvgFullSolarVerticalWm2 == null || monthIndex < 0 || monthIndex >= 12)
        {
            return 0.0;
        }

        string key = NormalizeOrientation(orientation);
        return climateZone.Monthly.AvgFullSolarVerticalWm2.TryGetValue(key, out var values) && monthIndex < values.Length
            ? values[monthIndex]
            : 0.0;
    }

    private static string NormalizeOrientation(string orientation)
    {
        return orientation?.Trim().ToUpperInvariant() switch
        {
            "N" => "N",
            "NE" => "NE",
            "E" => "E",
            "SE" => "SE",
            "S" => "S",
            "SW" => "SW",
            "W" => "W",
            "NW" => "NW",
            "H" => "H",
            _ => "S"
        };
    }
}
