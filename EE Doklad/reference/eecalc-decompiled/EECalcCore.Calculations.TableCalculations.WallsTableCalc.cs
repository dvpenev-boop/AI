// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// EECalcCore, Version=1.0.0.1269, Culture=neutral, PublicKeyToken=null
// EECalcCore.Calculations.TableCalculations.WallsTableCalc
using System.Collections.Generic;
using EECalcCore;
using EECalcCore.Calculations;

public static class WallsTableCalc
{
	private static List<double> outerAreaValuesList;

	private static List<double> innerAreaValuesList;

	private static List<double> windowAreaValuesList;

	public static void SumColumnOuterArea(this Walls walls)
	{
		walls.AccumulateOuterA = Calculator.SumFields(outerAreaValuesList = new List<double> { walls.OuterA1, walls.OuterA2, walls.OuterA3, walls.OuterA4, walls.OuterA5, walls.OuterA6 });
	}

	public static void AccumulateOuterU(this Walls walls)
	{
		List<double> valuesList = new List<double> { walls.OuterU1, walls.OuterU2, walls.OuterU3, walls.OuterU4, walls.OuterU5, walls.OuterU6 };
		walls.AccumulateOuterU = Calculator.AcumulateWeight(valuesList, outerAreaValuesList, walls.AccumulateOuterA);
	}

	public static void SumColumnOuterL(this Walls walls)
	{
		List<double> valuesList = new List<double>
		{
			walls.Outer1.SumL,
			walls.Outer2.SumL,
			walls.Outer3.SumL,
			walls.Outer4.SumL,
			walls.Outer5.SumL,
			walls.Outer6.SumL
		};
		walls.AccumulateOuterL = Calculator.SumFields(valuesList);
	}

	public static void SumColumnOuterX(this Walls walls)
	{
		List<double> valuesList = new List<double>
		{
			walls.Outer1.SumX,
			walls.Outer2.SumX,
			walls.Outer3.SumX,
			walls.Outer4.SumX,
			walls.Outer5.SumX,
			walls.Outer6.SumX
		};
		walls.AccumulateOuterX = Calculator.SumFields(valuesList);
	}

	public static void AcumulateOuterEpsilon(this Walls walls)
	{
		List<double> valuesList = new List<double> { walls.OuterE1, walls.OuterE2, walls.OuterE3, walls.OuterE4, walls.OuterE5, walls.OuterE6 };
		walls.AccumulateOuterE = Calculator.AcumulateWeight(valuesList, outerAreaValuesList, walls.AccumulateOuterA);
	}

	public static void AcumulateOuterAlfa(this Walls walls)
	{
		List<double> valuesList = new List<double> { walls.OuterAlfa1, walls.OuterAlfa2, walls.OuterAlfa3, walls.OuterAlfa4, walls.OuterAlfa5, walls.OuterAlfa6 };
		walls.AccumulateOuterAlfa = Calculator.AcumulateWeight(valuesList, outerAreaValuesList, walls.AccumulateOuterA);
	}

	public static void SumColumnInnerArea(this Walls walls)
	{
		walls.AccumulateInnerA = Calculator.SumFields(innerAreaValuesList = new List<double> { walls.InnerA1, walls.InnerA2, walls.InnerA3, walls.InnerA4, walls.IneerA5, walls.InnerA6 });
	}

	public static void CalculateInnerU(this Walls walls)
	{
		List<double> valuesList = new List<double> { walls.InnerU1, walls.InnerU2, walls.InnerU3, walls.InnerU4, walls.InnerU5, walls.InnerU6 };
		walls.AccumulateInnerU = Calculator.AcumulateWeight(valuesList, innerAreaValuesList, walls.AccumulateInnerA);
	}

	public static void SumWindowArea(this Walls walls)
	{
		walls.AccumulateWindowA = Calculator.SumFields(windowAreaValuesList = new List<double> { walls.WindowA1, walls.WindowA2, walls.WindowA3, walls.WindowA4, walls.WindowA5, walls.WindowA6 });
	}

	public static void CalculateWindowU(this Walls walls)
	{
		List<double> valuesList = new List<double> { walls.WindowU1, walls.WindowU2, walls.WindowU3, walls.WindowU4, walls.WindowU5, walls.WindowU6 };
		walls.AccumulateWindowU = Calculator.AcumulateWeight(valuesList, windowAreaValuesList, walls.AccumulateWindowA);
	}

	public static void CalculateWindowG(this Walls walls)
	{
		List<double> valuesList = new List<double> { walls.WindowG1, walls.WindowG2, walls.WindowG3, walls.WindowG4, walls.WindowG5, walls.WindowG6 };
		walls.AccumulateWindowG = Calculator.AcumulateWeight(valuesList, windowAreaValuesList, walls.AccumulateWindowA);
	}

	public static void CalculateWindowE(this Walls walls)
	{
		List<double> valuesList = new List<double> { walls.WindowE1, walls.WindowE2, walls.WindowE3, walls.WindowE4, walls.WindowE5, walls.WindowE6 };
		walls.AccumulateWindowE = Calculator.AcumulateWeight(valuesList, windowAreaValuesList, walls.AccumulateWindowA);
	}
}
