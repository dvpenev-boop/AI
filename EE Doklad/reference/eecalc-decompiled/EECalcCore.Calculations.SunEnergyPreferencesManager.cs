// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// EECalcCore, Version=1.0.0.1269, Culture=neutral, PublicKeyToken=null
// EECalcCore.Calculations.SunEnergyPreferencesManager
using System.IO;
using System.Linq;
using System.Windows.Forms;
using EECalcCore;
using EECalcCore.SunPreferences;

public static class SunEnergyPreferencesManager
{
	public static SunParameters SunParameters { get; private set; }

	static SunEnergyPreferencesManager()
	{
		string fileName = Path.Combine(Application.StartupPath, "Xml/DefaultSunParams.xml");
		SunParameters = EECalcCore.SunPreferences.EntityBase<SunParameters>.LoadFromFile(fileName);
	}

	public static ClimateZone GetClimateZoneParams(ClimateZones zone)
	{
		return SunParameters.ClimateZones.Single((ClimateZone z) => z.Number == (int)zone);
	}
}
