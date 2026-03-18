namespace EE.Doklad.Sections.Section11Heating.Models;

/// <summary>
/// Monthly heating-balance input assembled independently of the selected solar method.
/// </summary>
public class HeatingMonthlyInput
{
    public int MonthIndex { get; set; }
    public string MonthName { get; set; } = string.Empty;
    public double Te { get; set; }
    public int Dt { get; set; }
    public double Htr { get; set; }
    public double Hve { get; set; }
    public double Cm { get; set; }
    public double ThetaI { get; set; }
    public double Q_sol { get; set; }
    public double Q_int { get; set; }
}

public class HeatingMonthlyResult
{
    public int MonthIndex { get; set; }
    public string MonthName { get; set; } = string.Empty;
    public double Te { get; set; }
    public double Qht { get; set; }
    public double Qgn { get; set; }
    public double Qsol { get; set; }
    public double Qint { get; set; }
    public double Gamma { get; set; }
    public double Eta { get; set; }
    public double QH { get; set; }
    public bool IsHeating { get; set; }
}
