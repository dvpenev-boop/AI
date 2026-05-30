// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// EECalcCore, Version=1.0.0.1269, Culture=neutral, PublicKeyToken=null
// EECalcCore.Calculations.BuildingTypesManager
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using EECalcCore;

public static class BuildingTypesManager
{
	public static BuildingCategories Parameters { get; private set; }

	static BuildingTypesManager()
	{
		Assembly executingAssembly = Assembly.GetExecutingAssembly();
		Stream manifestResourceStream = executingAssembly.GetManifestResourceStream(executingAssembly.GetManifestResourceNames().FirstOrDefault());
		XmlDocument xmlDocument = new XmlDocument();
		if (manifestResourceStream != null)
		{
			xmlDocument.Load(manifestResourceStream);
		}
		Parameters = EntityBase<BuildingCategories>.Deserialize(xmlDocument.OuterXml);
	}

	public static Scale GetClimateZoneParams(InvestigationMethods investigationType)
	{
		return Parameters.ScaleType.Single((Scale z) => z.Type == (InvestigationType)investigationType);
	}
}
