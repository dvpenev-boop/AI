// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// EECalcCore, Version=1.0.0.1269, Culture=neutral, PublicKeyToken=null
// EECalcCore.Calculations.TableCalculations.BaseLineData
using EECalcCore;
using EECalcCore.Calculations;

public class BaseLineData
{
	public DataRow WorkingSchedule { get; set; }

	public DataRow UouterWalls { get; set; }

	public DataRow Uwindows { get; set; }

	public DataRow Unontransparent { get; set; }

	public DataRow Ufloor { get; set; }

	public DataRow G { get; set; }

	public DataRow UinnerWalls { get; set; }

	public DataRow Uceiling { get; set; }

	public DataRow UfloorOther { get; set; }

	public DataRow Infiltracion { get; set; }

	public DataRow ProjectTemperature { get; set; }

	public DataRow NonProjectTemperature { get; set; }

	public DataRow ResulNoInputsNetEnergy { get; set; }

	public DataRow ResulVentilationInputs { get; set; }

	public DataRow ResulLightInputs { get; set; }

	public DataRow ResulAppliancesInputs { get; set; }

	public DataRow ResulNetEnergy { get; set; }

	public Fuel Fuel1 { get; set; }

	public DataRow Part1 { get; set; }

	public DataRow TransmitTempEfficiency1 { get; set; }

	public DataRow SupplyNetEfficiency1 { get; set; }

	public DataRow Automatic1 { get; set; }

	public DataRow EnergyManagement1 { get; set; }

	public DataRow GeneratorHeatEfficiency1 { get; set; }

	public DataRow ResultSourceEnergy { get; set; }

	public Fuel Fuel2 { get; set; }

	public DataRow Part2 { get; set; }

	public DataRow TransmitTempEfficiency2 { get; set; }

	public DataRow SupplyNetEfficiency2 { get; set; }

	public DataRow Automatic2 { get; set; }

	public DataRow EnergyManagement2 { get; set; }

	public DataRow ResultSourceEnergy2 { get; set; }

	public DataRow GeneratorHeatEfficiency2 { get; set; }

	public DataRow HeatEfficiencyGenerating { get; set; }

	public DataRow ResultNeededEnergy { get; set; }
}
