// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// EECalcCore, Version=1.0.0.1269, Culture=neutral, PublicKeyToken=null
// EECalcCore.Calculations.Calculator
using System;
using System.Collections.Generic;
using System.Linq;
using EECalcCore;

public static class Calculator
{
	public static void Calculate(CalculationInput input)
	{
	}

	public static double AcumulateWeight(List<double> valuesList, List<double> areaValues, double acumulatedAreaValue)
	{
		double num = 0.0;
		try
		{
			for (int i = 0; i < valuesList.Count(); i++)
			{
				double num2 = valuesList[i];
				if (Math.Abs(num2) < 0.0001)
				{
					num2 = 0.0;
				}
				num += num2 * areaValues[i];
			}
			num /= acumulatedAreaValue;
			if (double.IsInfinity(num) || double.IsNaN(num))
			{
				num = 0.0;
			}
		}
		catch
		{
			num = 0.0;
		}
		return num;
	}

	public static double SumFields(IEnumerable<double> valuesList)
	{
		double num = 0.0;
		foreach (double values in valuesList)
		{
			try
			{
				num = ((!(Math.Abs(values) < 0.0001)) ? (num + values) : (num + 0.0));
			}
			catch
			{
				num += 0.0;
			}
		}
		return num;
	}
}
