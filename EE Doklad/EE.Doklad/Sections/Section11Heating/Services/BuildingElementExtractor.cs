using System.Collections.Generic;
using System.Linq;
using EE.Doklad.Models;
using EE.Doklad.Sections.Section11Heating.Models;

namespace EE.Doklad.Sections.Section11Heating.Services;

/// <summary>
/// Read-only projection from Sections 6/7/9 to neutral DTOs used by Section 11 calculations.
/// </summary>
public static class BuildingElementExtractor
{
    public static IEnumerable<WallData> ExtractWalls(ExternalWallsSectionData? wallsData)
    {
        if (wallsData == null)
        {
            return Enumerable.Empty<WallData>();
        }

        var result = new List<WallData>();
        foreach (var wall in wallsData.WallTypes)
        {
            AddWallIfPositive(result, wall.FacadeNorth, wall.Uw, wall.SurfaceProperties.AlphaDefault, wall.SurfaceProperties.EpsilonDefault, "N");
            AddWallIfPositive(result, wall.FacadeNorthEast, wall.Uw, wall.SurfaceProperties.GetAlpha(WallOrientation.NE), wall.SurfaceProperties.GetEpsilon(WallOrientation.NE), "NE");
            AddWallIfPositive(result, wall.FacadeEast, wall.Uw, wall.SurfaceProperties.GetAlpha(WallOrientation.E), wall.SurfaceProperties.GetEpsilon(WallOrientation.E), "E");
            AddWallIfPositive(result, wall.FacadeSouthEast, wall.Uw, wall.SurfaceProperties.GetAlpha(WallOrientation.SE), wall.SurfaceProperties.GetEpsilon(WallOrientation.SE), "SE");
            AddWallIfPositive(result, wall.FacadeSouth, wall.Uw, wall.SurfaceProperties.GetAlpha(WallOrientation.S), wall.SurfaceProperties.GetEpsilon(WallOrientation.S), "S");
            AddWallIfPositive(result, wall.FacadeSouthWest, wall.Uw, wall.SurfaceProperties.GetAlpha(WallOrientation.SW), wall.SurfaceProperties.GetEpsilon(WallOrientation.SW), "SW");
            AddWallIfPositive(result, wall.FacadeWest, wall.Uw, wall.SurfaceProperties.GetAlpha(WallOrientation.W), wall.SurfaceProperties.GetEpsilon(WallOrientation.W), "W");
            AddWallIfPositive(result, wall.FacadeNorthWest, wall.Uw, wall.SurfaceProperties.GetAlpha(WallOrientation.NW), wall.SurfaceProperties.GetEpsilon(WallOrientation.NW), "NW");
        }

        return result;
    }

    public static IEnumerable<WindowData> ExtractWindows(WindowsSectionData? windowsData)
    {
        if (windowsData == null)
        {
            return Enumerable.Empty<WindowData>();
        }

        return windowsData.WindowBatches
            .Where(batch => batch.Count > 0 && batch.AreaGross > 0.0)
            .Select(batch => new WindowData(
                Area: batch.Count * batch.AreaGross,
                U: batch.UValue,
                G: batch.GEffHeat > 0.0 ? batch.GEffHeat : batch.GEff,
                Epsilon: batch.GlassEmissivity > 0.0 ? batch.GlassEmissivity : 0.84,
                Orientation: MapOrientation(batch.Orientation),
                Fw: HeatingConstants.Fw_Default,
                Ffr: batch.FrameFraction > 0.0 ? batch.FrameFraction : HeatingConstants.Ffr_Default));
    }

    public static IEnumerable<RoofData> ExtractRoofs(RoofSectionData? roofData)
    {
        if (roofData == null)
        {
            return Enumerable.Empty<RoofData>();
        }

        return roofData.RoofTypes
            .Where(roof => roof.Area > 0.0 && roof.UValue > 0.0)
            .Select(roof => new RoofData(
                Area: roof.Area,
                U: roof.UValue,
                Alpha: roof.SurfaceProperties.AlphaDefault,
                Epsilon: roof.SurfaceProperties.EpsilonDefault));
    }

    private static void AddWallIfPositive(
        ICollection<WallData> target,
        double area,
        double u,
        double alpha,
        double epsilon,
        string orientation)
    {
        if (area <= 0.0 || u <= 0.0)
        {
            return;
        }

        target.Add(new WallData(area, u, alpha, epsilon, orientation));
    }

    private static string MapOrientation(Orientation orientation)
    {
        return orientation switch
        {
            Orientation.North => "N",
            Orientation.NorthEast => "NE",
            Orientation.East => "E",
            Orientation.SouthEast => "SE",
            Orientation.South => "S",
            Orientation.SouthWest => "SW",
            Orientation.West => "W",
            Orientation.NorthWest => "NW",
            _ => "S"
        };
    }
}
