// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// EECalcCore, Version=1.0.0.1269, Culture=neutral, PublicKeyToken=null
// EECalcCore.Calculations.TableCalculations.FloorTableCalc
using System.Collections.Generic;
using EECalcCore;
using EECalcCore.Calculations;

public static class FloorTableCalc
{
	private static List<double> floorAreaValuesList;

	private static List<double> otherFloorAreaValuesList;

	public static void CalculateFloorArea(this Floor floor)
	{
		floor.AccumulateFloorA = Calculator.SumFields(floorAreaValuesList = new List<double> { floor.FloorA1, floor.FloorA2, floor.FloorA3, floor.FloorA4, floor.FloorA5, floor.FloorA6 });
	}

	public static void CalculateFloorU(this Floor floor)
	{
		List<double> valuesList = new List<double> { floor.FloorU1, floor.FloorU2, floor.FloorU3, floor.FloorU4, floor.FloorU5, floor.FloorU6 };
		floor.AccumulateFloorU = Calculator.AcumulateWeight(valuesList, floorAreaValuesList, floor.AccumulateFloorA);
	}

	public static void CalculateOtherFloorArea(this Floor floor)
	{
		floor.AccumulateOtherFloorA = Calculator.SumFields(otherFloorAreaValuesList = new List<double> { floor.OtherFloorA1, floor.OtherFloorA2, floor.OtherFloorA3, floor.OtherFloorA4, floor.OtherFloorA5, floor.OtherFloorA6 });
	}

	public static void CalculateOtherFloorU(this Floor floor)
	{
		List<double> valuesList = new List<double> { floor.OtherFloorU1, floor.OtherFloorU2, floor.OtherFloorU3, floor.OtherFloorU4, floor.OtherFloorU5, floor.OtherFloorU6 };
		floor.AccumulateOtherFloorU = Calculator.AcumulateWeight(valuesList, otherFloorAreaValuesList, floor.AccumulateFloorA);
	}

	public static void SumX(this Floor floor)
	{
		List<double> valuesList = new List<double>
		{
			floor.Floor1.SumX,
			floor.Floor2.SumX,
			floor.Floor3.SumX,
			floor.Floor4.SumX,
			floor.Floor5.SumX,
			floor.Floor6.SumX
		};
		floor.AccumulateFloorX = Calculator.SumFields(valuesList);
	}

	public static void SumL(this Floor floor)
	{
		List<double> valuesList = new List<double>
		{
			floor.Floor1.SumL,
			floor.Floor2.SumL,
			floor.Floor3.SumL,
			floor.Floor4.SumL,
			floor.Floor5.SumL,
			floor.Floor6.SumL
		};
		floor.AccumulateFloorL = Calculator.SumFields(valuesList);
	}
}
