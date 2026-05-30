// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// EECalcCore, Version=1.0.0.1269, Culture=neutral, PublicKeyToken=null
// EECalcCore.Calculations.TableCalculations.TempBridgeCalc
using EECalcCore;

public static class TempBridgeCalc
{
	public static void CalculateSums(this TempBridge tempBridge)
	{
		tempBridge.Type1Sum = tempBridge.Type1L * tempBridge.Type1Fi;
		tempBridge.Type2Sum = tempBridge.Type2L * tempBridge.Type2Fi;
		tempBridge.Type3Sum = tempBridge.Type3L * tempBridge.Type3Fi;
		tempBridge.Type4Sum = tempBridge.Type4L * tempBridge.Type4Fi;
		tempBridge.SumL = tempBridge.Type1Sum + tempBridge.Type2Sum + tempBridge.Type3Sum + tempBridge.Type4Sum;
	}
}
