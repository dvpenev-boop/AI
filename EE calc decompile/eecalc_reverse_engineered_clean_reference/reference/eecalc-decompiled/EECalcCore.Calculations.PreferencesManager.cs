// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// EECalcCore, Version=1.0.0.1269, Culture=neutral, PublicKeyToken=null
// EECalcCore.Calculations.PreferencesManager
using System.IO;
using System.Linq;
using System.Windows.Forms;
using EECalcCore;
using EECalcCore.Preferences;

public static class PreferencesManager
{
	public static Parameters Parameters { get; private set; }

	static PreferencesManager()
	{
		string fileName = Path.Combine(Application.StartupPath, "Xml/DefaultParams.xml");
		Parameters = EECalcCore.Preferences.EntityBase<Parameters>.LoadFromFile(fileName);
	}

	public static ClimateZone GetClimateZoneParams(ClimateZones zone)
	{
		return Parameters.ClimateZones.Single((ClimateZone z) => z.Number == (int)zone);
	}
}
