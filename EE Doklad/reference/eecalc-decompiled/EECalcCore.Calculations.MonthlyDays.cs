// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// EECalcCore, Version=1.0.0.1269, Culture=neutral, PublicKeyToken=null
// EECalcCore.Calculations.MonthlyDays
using EECalcCore;

public class MonthlyDays
{
	public Month Month { get; set; }

	public int WorkDays { get; set; }

	public int Saturdays { get; set; }

	public int Sundays { get; set; }

	public int Holydays { get; set; }

	public int TotalDays { get; set; }

	public double Weeks { get; set; }

	public MonthlyDays(Month monthValue, int workdayValue, int saturdayValue, int sundayValue, int totalDays)
	{
		Month = monthValue;
		WorkDays = workdayValue;
		Saturdays = saturdayValue;
		Sundays = sundayValue;
		TotalDays = totalDays;
	}

	public MonthlyDays()
	{
	}
}
