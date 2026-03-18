using System.Collections.Generic;

namespace EE.Doklad.Sections.Section11Heating.Services;

public interface ISolarStrategy
{
    double CalcQsol(
        IEnumerable<WallData> walls,
        IEnumerable<WindowData> windows,
        IEnumerable<RoofData> roofs,
        int climateZone,
        int monthIndex);
}

public record WallData(
    double Area,
    double U,
    double Alpha,
    double Epsilon,
    string Orientation);

public record WindowData(
    double Area,
    double U,
    double G,
    double Epsilon,
    string Orientation,
    double Fw = 0.90,
    double Ffr = 0.30);

public record RoofData(
    double Area,
    double U,
    double Alpha,
    double Epsilon);
