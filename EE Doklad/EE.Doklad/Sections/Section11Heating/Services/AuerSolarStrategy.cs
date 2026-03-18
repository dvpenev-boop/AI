using System;
using System.Collections.Generic;
using EE.Doklad.Sections.Section11Heating.Models;
using ClimateDb = EE.Doklad.Sections.Section11Heating.Models.ClimateDatabase;

namespace EE.Doklad.Sections.Section11Heating.Services;

public class AuerSolarStrategy : ISolarStrategy
{
    public double CalcQsol(
        IEnumerable<WallData> walls,
        IEnumerable<WindowData> windows,
        IEnumerable<RoofData> roofs,
        int climateZone,
        int monthIndex)
    {
        double qgn = 0.0;
        int dt = ClimateDb.DtHours[monthIndex];

        foreach (var wall in walls)
        {
            double isol = ClimateDb.GetIsol(climateZone, wall.Orientation, monthIndex);
            double asol = wall.Alpha * HeatingConstants.Rse * wall.U * wall.Area;
            double hr = CalcHr(wall.Epsilon);
            double phiR = HeatingConstants.Rse * wall.U * wall.Area * hr * HeatingConstants.DeltaTheta;
            double phiSol = asol * isol - HeatingConstants.Fsky_Vertical * phiR;
            qgn += phiSol * dt / 1000.0;
        }

        foreach (var roof in roofs)
        {
            double isol = ClimateDb.GetIsol(climateZone, "H", monthIndex);
            double asol = roof.Alpha * HeatingConstants.Rse * roof.U * roof.Area;
            double hr = CalcHr(roof.Epsilon);
            double phiR = HeatingConstants.Rse * roof.U * roof.Area * hr * HeatingConstants.DeltaTheta;
            double phiSol = asol * isol - HeatingConstants.Fsky_Horizontal * phiR;
            qgn += phiSol * dt / 1000.0;
        }

        foreach (var window in windows)
        {
            if (window.G <= 0.0 || window.Area <= 0.0)
            {
                continue;
            }

            double isol = ClimateDb.GetIsol(climateZone, window.Orientation, monthIndex);
            double asol = window.G * window.Area;
            double hr = CalcHr(window.Epsilon);
            double phiR = HeatingConstants.Rse * window.U * asol * hr * HeatingConstants.DeltaTheta;
            double phiSol = asol * isol - HeatingConstants.Fsky_Window_Auer * phiR;
            qgn += phiSol * dt / 1000.0;
        }

        return qgn;
    }

    private static double CalcHr(double epsilon)
        => 4 * epsilon * HeatingConstants.Sigma
           * Math.Pow(HeatingConstants.ThetaSs + 273.0, 3);
}
