using System;
using System.Collections.Generic;

namespace EE.Doklad.Sections.Section11Heating.Services;

public class AshraeStrategy : ISolarStrategy
{
    public double CalcQsol(
        IEnumerable<WallData> walls,
        IEnumerable<WindowData> windows,
        IEnumerable<RoofData> roofs,
        int climateZone,
        int monthIndex)
    {
        throw new NotImplementedException(
            "\u041C\u0435\u0442\u043E\u0434 ASHRAE 8760 \u043D\u0435 \u0435 \u0438\u043C\u043F\u043B\u0435\u043C\u0435\u043D\u0442\u0438\u0440\u0430\u043D. \u0418\u0437\u0431\u0435\u0440\u0435\u0442\u0435 \u043C\u0435\u0442\u043E\u0434 1 (\u0410\u0423\u0415\u0420) \u0438\u043B\u0438 \u043C\u0435\u0442\u043E\u0434 2 (\u0420\u0414-02-20-3).");
    }
}
