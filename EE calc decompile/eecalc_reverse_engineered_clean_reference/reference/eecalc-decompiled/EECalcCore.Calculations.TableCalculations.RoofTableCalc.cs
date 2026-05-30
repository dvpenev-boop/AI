// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// EECalcCore, Version=1.0.0.1269, Culture=neutral, PublicKeyToken=null
// EECalcCore.Calculations.TableCalculations.RoofTableCalc
using System.Collections.Generic;
using EECalcCore;
using EECalcCore.Calculations;

public static class RoofTableCalc
{
	private static List<double> areaValuesList;

	private static List<double> trasparentAreaValuesList;

	private static List<double> ceilingAreaValuesList;

	public static void CalculateArea(this Roof roof)
	{
		roof.AccumulateNonTransparentA = Calculator.SumFields(areaValuesList = new List<double> { roof.NonTransparentA1, roof.NonTransparentA2, roof.NonTransparentA3, roof.NonTransparentA4, roof.NonTransparentA5, roof.NonTransparentА6, roof.NonTransparentA7, roof.NonTransparentA8, roof.NonTransparentA9 });
	}

	public static void CalculateNonTranspU(this Roof roof)
	{
		List<double> valuesList = new List<double> { roof.NonTransparentU1, roof.NonTransparentU2, roof.NonTransparentU3, roof.NonTransparentU4, roof.NonTransparentU5, roof.NonTransparentU6, roof.NonTransparentU7, roof.NonTransparentU8, roof.NonTransparentU9 };
		roof.AccumulateNonTransparentU = Calculator.AcumulateWeight(valuesList, areaValuesList, roof.AccumulateNonTransparentA);
	}

	public static void SumL(this Roof roof)
	{
		List<double> valuesList = new List<double>
		{
			roof.NonTransparent1.SumL,
			roof.NonTransparent2.SumL,
			roof.NonTransparent3.SumL,
			roof.NonTransparent4.SumL,
			roof.NonTransparent5.SumL,
			roof.NonTransparent6.SumL,
			roof.NonTransparent7.SumL,
			roof.NonTransparent8.SumL,
			roof.NonTransparent9.SumL
		};
		roof.AccumulateNonTransparentL = Calculator.SumFields(valuesList);
	}

	public static void SumX(this Roof roof)
	{
		List<double> valuesList = new List<double>
		{
			roof.NonTransparent1.SumX,
			roof.NonTransparent2.SumX,
			roof.NonTransparent3.SumX,
			roof.NonTransparent4.SumX,
			roof.NonTransparent5.SumX,
			roof.NonTransparent6.SumX,
			roof.NonTransparent7.SumX,
			roof.NonTransparent8.SumX,
			roof.NonTransparent9.SumX
		};
		roof.AccumulateNonTransparentX = Calculator.SumFields(valuesList);
	}

	public static void CalculateEpsilon(this Roof roof)
	{
		List<double> valuesList = new List<double> { roof.NonTransparentE1, roof.NonTransparentE2, roof.NonTransparentE3, roof.NonTransparentE4, roof.NonTransparentE5, roof.NonTransparentE6, roof.NonTransparentE7, roof.NonTransparentE8, roof.NonTransparentE9 };
		roof.AccumulateNonTransparentE = Calculator.AcumulateWeight(valuesList, areaValuesList, roof.AccumulateNonTransparentA);
	}

	public static void CalculateAlfa(this Roof roof)
	{
		List<double> valuesList = new List<double> { roof.NonTransparentAlfa1, roof.NonTransparentAlfa2, roof.NonTransparentAlfa3, roof.NonTransparentAlfa4, roof.NonTransparentAlfa5, roof.NonTransparentAlfa6, roof.NonTransparentAlfa7, roof.NonTransparentAlfa8, roof.NonTransparentAlfa9 };
		roof.AccumulateNonTransparentAlfa = Calculator.AcumulateWeight(valuesList, areaValuesList, roof.AccumulateNonTransparentA);
	}

	public static void SumTrasparentArea(this Roof roof)
	{
		roof.AccumulateTransparentA = Calculator.SumFields(trasparentAreaValuesList = new List<double> { roof.TransparentA1, roof.TransparentA2, roof.TransparentA3, roof.TransparentA4, roof.TransparentA5, roof.TransparentА6, roof.TransparentA7, roof.TransparentA8, roof.TransparentA9 });
	}

	public static void CalculateTrasparentU(this Roof roof)
	{
		List<double> valuesList = new List<double> { roof.TransparentU1, roof.TransparentU2, roof.TransparentU3, roof.TransparentU4, roof.TransparentU5, roof.TransparentU6, roof.TransparentU7, roof.TransparentU8, roof.TransparentU9 };
		roof.AccumulateTransparentU = Calculator.AcumulateWeight(valuesList, trasparentAreaValuesList, roof.AccumulateTransparentA);
	}

	public static void CalculateTrasparentG(this Roof roof)
	{
		List<double> valuesList = new List<double> { roof.TransparentG1, roof.TransparentG2, roof.TransparentG3, roof.TransparentG4, roof.TransparentG5, roof.TransparentG6, roof.TransparentG7, roof.TransparentG8, roof.TransparentG9 };
		roof.AccumulateTransparentG = Calculator.AcumulateWeight(valuesList, trasparentAreaValuesList, roof.AccumulateTransparentA);
	}

	public static void SumCeilingArea(this Roof roof)
	{
		roof.AccumulateCeilingA = Calculator.SumFields(ceilingAreaValuesList = new List<double> { roof.CeilingA1, roof.CeilingA2, roof.CeilingA3, roof.CeilingA4, roof.CeilingA5, roof.CeilingA6, roof.CeilingA7, roof.CeilingA8, roof.CeilingA9 });
	}

	public static void CalculateCeilingU(this Roof roof)
	{
		List<double> valuesList = new List<double> { roof.CeilingU1, roof.CeilingU2, roof.CeilingU3, roof.CeilingU4, roof.CeilingU5, roof.CeilingU6, roof.CeilingU7, roof.CeilingU8, roof.CeilingU9 };
		roof.AccumulateCeilingU = Calculator.AcumulateWeight(valuesList, ceilingAreaValuesList, roof.AccumulateCeilingA);
	}
}
