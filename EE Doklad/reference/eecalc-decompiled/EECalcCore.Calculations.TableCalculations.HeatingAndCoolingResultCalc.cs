// Warning: Some assembly references could not be resolved automatically. This might lead to incorrect decompilation of some parts,
// for ex. property getter/setter access. To get optimal decompilation results, please manually add the missing references to the list of loaded assemblies.
// EECalcCore, Version=1.0.0.1269, Culture=neutral, PublicKeyToken=null
// EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using EECalcCore;
using EECalcCore.Calculations;
using EECalcCore.Calculations.TableCalculations;
using EECalcCore.Preferences;
using Telerik.Windows.Diagrams.Core;

public static class HeatingAndCoolingResultCalc
{
	private static BuildingZone buildingZone;

	private static double north;

	private static double northEast;

	private static double east;

	private static double southEast;

	private static double south;

	private static double southWest;

	private static double west;

	private static double northWest;

	private static double northU;

	private static double northEastU;

	private static double eastU;

	private static double southEastU;

	private static double southU;

	private static double southWestU;

	private static double westU;

	private static double northWestU;

	private static double areaNorth;

	private static double areaNorthEast;

	private static double areaEast;

	private static double areaSouthEast;

	private static double areaSouth;

	private static double areaSouthWest;

	private static double areaWest;

	private static double areaNorthWest;

	private static double parameterNiRef1;

	private static double parameterNiRef2;

	private static double parameterNiBaseLine;

	private static double parameterNiESM;

	private static BuildingZone currentZone;

	private static readonly List<double> LigthsListRef1 = new List<double>();

	private static readonly List<double> LigthsListRef2 = new List<double>();

	private static readonly List<double> LigthsList = new List<double>();

	private static readonly List<double> LigthsListBaseLine = new List<double>();

	private static readonly List<double> LigthsListESM = new List<double>();

	private static readonly List<double> DevicesRef1 = new List<double>();

	private static readonly List<double> DevicesRef2 = new List<double>();

	private static readonly List<double> DevicesList = new List<double>();

	private static readonly List<double> DevicesListBaseLine = new List<double>();

	private static readonly List<double> DevicesListESM = new List<double>();

	private static double weekRegime;

	private static CalculationData publicCalculationData;

	private static double virtualBaseLineNetEnergy;

	private static double virtualESMNetEnergy;

	private static CalculationData fansAndPumnps;

	private const double Corr = Math.PI / 180.0;

	private static SunEnergyCalculationData sunEnergyCalcdata;

	private static double innerTemp;

	private static double monthHours;

	public static List<MonthDataCooling> MonthDataCoolingList { get; set; }

	public static List<MonthData> MonthDataList { get; set; }

	public static object MessageBoxButton { get; private set; }

	public static void CoolingCalculations(this CalculationData calcData, Section section, CalculationInput calcInput, BuildingZone zone, CalculationData lightsAndDevicesCalculationData, CalculationData ventCool)
	{
		buildingZone = zone;
		List<MonthlyDays> monthslist = section.CalcPeriod((int)section.CoolingSeason.FirstMonthCool, (int)section.CoolingSeason.LastMonthCool, section.CoolingSeason.FirstDayCool, section.CoolingSeason.LastDayCool);
		if (buildingZone.HasRefenceValues)
		{
			CalculateCoolingEnergyRef1(monthslist, calcData, section, calcInput, lightsAndDevicesCalculationData, ventCool);
			CalculateCoolingEnergyRef2(monthslist, calcData, section, calcInput, lightsAndDevicesCalculationData, ventCool);
		}
		CalculateCoolingEnergyActual(monthslist, calcData, section, calcInput, lightsAndDevicesCalculationData, ventCool);
		CalculateCoolingEnergyBaseLine(monthslist, calcData, section, calcInput, lightsAndDevicesCalculationData, ventCool);
		CalculateCoolingEnergyESM(monthslist, calcData, section, calcInput, lightsAndDevicesCalculationData, ventCool);
	}

	public static void GetWeekHoursCoolingResultActual(this CalculationData coolingCalc, Section section)
	{
		double num = 5.0 * section.CalcHours(section.CoolingSeasons.Cooling.WorkCurrentStart, section.CoolingSeasons.Cooling.WorkCurrentEnd);
		num += section.CalcHours(section.CoolingSeasons.Cooling.SunCurrentStart, section.CoolingSeasons.Cooling.SunCurrentEnd);
		num += section.CalcHours(section.CoolingSeasons.Cooling.SatCurrentStart, section.CoolingSeasons.Cooling.SatCurrentEnd);
		coolingCalc.WorkingScheduleActual = num;
	}

	public static void GetWeekHoursCoolingResultBaseLine(this CalculationData coolingCalc, Section section)
	{
		double num = 5.0 * section.CalcHours(section.CoolingSeasons.Cooling.WorkBaseStart, section.CoolingSeasons.Cooling.WorkBaseEnd);
		num += section.CalcHours(section.CoolingSeasons.Cooling.SunBaseStart, section.CoolingSeasons.Cooling.SunBaseEnd);
		num += section.CalcHours(section.CoolingSeasons.Cooling.SatBaseStart, section.CoolingSeasons.Cooling.SatBaseEnd);
		coolingCalc.WorkingScheduleBaseLine = num;
	}

	public static void GetWeekHoursCoolingResultEsm(this CalculationData coolingCalc, Section section)
	{
		double num = 5.0 * section.CalcHours(section.CoolingSeasons.Cooling.WorkEsmStart, section.CoolingSeasons.Cooling.WorkEsmEnd);
		num += section.CalcHours(section.CoolingSeasons.Cooling.SunEsmStart, section.CoolingSeasons.Cooling.SunEsmEnd);
		num += section.CalcHours(section.CoolingSeasons.Cooling.SatEsmStart, section.CoolingSeasons.Cooling.SatEsmEnd);
		coolingCalc.WorkingScheduleESM = num;
	}

	public static void GetWeekHoursCoolingResultReferences(this CalculationData coolingCalc, Section section)
	{
		double num = 5.0 * section.CalcHours(section.CoolingSeasons.Cooling.WorkBaseStart, section.CoolingSeasons.Cooling.WorkBaseEnd);
		num += section.CalcHours(section.CoolingSeasons.Cooling.SunBaseStart, section.CoolingSeasons.Cooling.SunBaseEnd);
		num = (coolingCalc.WorkingScheduleRef = num + section.CalcHours(section.CoolingSeasons.Cooling.SatBaseStart, section.CoolingSeasons.Cooling.SatBaseEnd));
		coolingCalc.WorkingScheduleRef2 = num;
	}

	private static void CalculateCoolingEnergyRef1(List<MonthlyDays> monthslist, CalculationData calcData, Section section, CalculationInput calcInput, CalculationData lightsAndDevicesCalculationData, CalculationData ventCool)
	{
		List<MonthDataCooling> list = new List<MonthDataCooling>();
		List<double> list2 = new List<double>();
		List<double> list3 = new List<double>();
		List<double> list4 = new List<double>();
		Section tempSection = EECalcCore.EntityBase<Section>.Deserialize(section.Serialize());
		ApplyValuesToTempSectionRef1(tempSection, calcData);
		foreach (MonthlyDays item2 in monthslist)
		{
			MonthDataCooling monthDataCooling = new MonthDataCooling();
			double num = CalculateQgainRef1(section, calcInput.General.ClimateZone, item2, lightsAndDevicesCalculationData);
			double num2 = CalculateCoolingQtrRef1(calcData, section, calcInput.General.ClimateZone, item2) + CalculateQinfRef1(section, calcInput.General.ClimateZone, calcData, item2);
			double parameterAc = CalculateAcRef1(calcData, section, calcInput.General.ClimateZone, item2);
			double num3 = CalculateETA(parameterAc, num2, num, section);
			double num4 = num - num3 * num2 + CalculateQLatentOccupantsRef1(section, item2);
			num4 = num4 + CalculateLatentHeatsInfRef1(section, calcData, item2, calcInput.General.ClimateZone) + CalculateLatentHeatsVentRef1(section, calcData, item2, calcInput.General.ClimateZone, calcData);
			list3.Add(num4);
			double num5 = ClaculateQfreecoolingRef1(section, calcData, item2, calcInput.General.ClimateZone);
			list2.Add(num5);
			double item = num4 + num5 + CalculateQveRef1(section, calcData, ventCool, item2);
			list4.Add(item);
			monthDataCooling.AvgTemp = PreferencesManager.GetClimateZoneParams(calcInput.General.ClimateZone).SolarRadiation.Months[(int)item2.Month].AvgTemp;
			monthDataCooling.Month = item2;
			monthDataCooling.ParameterQtr = num2;
			monthDataCooling.ParameterNi = num3;
			monthDataCooling.ParameterHtr = section.Test.ParameterHtr;
			monthDataCooling.ParamHd = section.Test.ParameterHd;
			monthDataCooling.ParamHg = section.Test.ParameterHg;
			monthDataCooling.ParamHu = section.Test.ParameterHu;
			monthDataCooling.NetEnergyQnd = num4;
			monthDataCooling.ParameterGama = section.Test.ParameterGamma;
			list.Add(monthDataCooling);
		}
		MonthDataCoolingList = list;
		double num6 = list3.Aggregate(0.0, (double num9, double num10) => num9 + num10);
		calcData.ResulNoInputsNetEnergyRef1 = num6 / section.Area.HeatedArea;
		double num7 = (calcData.ResulCoolingInputsRef1 = list2.Aggregate(0.0, (double num9, double num10) => num9 + num10));
		calcData.ResulNetEnergyRef1 = num6 / section.Area.HeatedArea - num7 - calcData.ResulVentilationInputsRef1;
	}

	private static void CalculateCoolingEnergyRef2(List<MonthlyDays> monthslist, CalculationData calcData, Section section, CalculationInput calcInput, CalculationData lightsAndDevicesCalculationData, CalculationData ventCool)
	{
		List<MonthDataCooling> list = new List<MonthDataCooling>();
		List<double> list2 = new List<double>();
		List<double> list3 = new List<double>();
		List<double> list4 = new List<double>();
		Section tempSection = EECalcCore.EntityBase<Section>.Deserialize(section.Serialize());
		ApplyValuesToTempSectionRef2(tempSection, calcData);
		foreach (MonthlyDays item2 in monthslist)
		{
			MonthDataCooling monthDataCooling = new MonthDataCooling();
			double num = CalculateQgainRef2(section, calcInput.General.ClimateZone, item2, lightsAndDevicesCalculationData);
			double num2 = CalculateCoolingQtrRef2(calcData, section, calcInput.General.ClimateZone, item2) + CalculateQinfRef2(section, calcInput.General.ClimateZone, calcData, item2);
			double parameterAc = CalculateAcRef2(calcData, section, calcInput.General.ClimateZone, item2);
			double num3 = CalculateETA(parameterAc, num2, num, section);
			double num4 = num - num3 * num2 + CalculateQLatentOccupantsRef2(section, item2);
			num4 = num4 + CalculateLatentHeatsInfRef2(section, calcData, item2, calcInput.General.ClimateZone) + CalculateLatentHeatsVentRef2(section, calcData, item2, calcInput.General.ClimateZone, calcData);
			list3.Add(num4);
			double num5 = ClaculateQfreecoolingRef2(section, calcData, item2, calcInput.General.ClimateZone);
			list2.Add(num5);
			double item = num4 + num5 + CalculateQveRef2(section, calcData, ventCool, item2);
			list4.Add(item);
			monthDataCooling.AvgTemp = PreferencesManager.GetClimateZoneParams(calcInput.General.ClimateZone).SolarRadiation.Months[(int)item2.Month].AvgTemp;
			monthDataCooling.Month = item2;
			monthDataCooling.ParameterQtr = num2;
			monthDataCooling.ParameterNi = num3;
			monthDataCooling.ParameterHtr = section.Test.ParameterHtr;
			monthDataCooling.ParamHd = section.Test.ParameterHd;
			monthDataCooling.ParamHg = section.Test.ParameterHg;
			monthDataCooling.ParamHu = section.Test.ParameterHu;
			monthDataCooling.NetEnergyQnd = num4;
			monthDataCooling.ParameterGama = section.Test.ParameterGamma;
			list.Add(monthDataCooling);
		}
		MonthDataCoolingList = list;
		double num6 = list3.Aggregate(0.0, (double num9, double num10) => num9 + num10);
		calcData.ResulNoInputsNetEnergyRef2 = num6 / section.Area.HeatedArea;
		double num7 = (calcData.ResulCoolingInputsRef2 = list2.Aggregate(0.0, (double num9, double num10) => num9 + num10));
		calcData.ResulNetEnergyRef2 = num6 / section.Area.HeatedArea - num7 - calcData.ResulVentilationInputsRef2;
	}

	private static void CalculateCoolingEnergyActual(List<MonthlyDays> monthslist, CalculationData calcData, Section section, CalculationInput calcInput, CalculationData lightsAndDevicesCalculationData, CalculationData ventCool)
	{
		List<MonthDataCooling> list = new List<MonthDataCooling>();
		List<double> list2 = new List<double>();
		List<double> list3 = new List<double>();
		List<double> list4 = new List<double>();
		foreach (MonthlyDays item2 in monthslist)
		{
			MonthDataCooling monthDataCooling = new MonthDataCooling();
			double num = CalculateQgain(section, calcInput.General.ClimateZone, item2, lightsAndDevicesCalculationData);
			double num2 = CalculateCoolingQtr(calcData, section, calcInput.General.ClimateZone, item2) + CalculateQinf(section, calcInput.General.ClimateZone, calcData, item2);
			double parameterAc = CalculateAc(calcData, section, calcInput.General.ClimateZone, item2);
			double num3 = CalculateETA(parameterAc, num2, num, section);
			double num4 = num - num3 * num2 + CalculateQLatentOccupants(section, item2);
			num4 = num4 + CalculateLatentHeatsInf(section, calcData, item2, calcInput.General.ClimateZone) + CalculateLatentHeatsVent(section, calcData, item2, calcInput.General.ClimateZone, calcData);
			list2.Add(num4);
			double num5 = ClaculateQfreecooling(section, calcData, item2, calcInput.General.ClimateZone);
			list4.Add(num5);
			double item = num4 + num5 + CalculateQve(section, calcData, ventCool, item2);
			list3.Add(item);
			monthDataCooling.AvgTemp = PreferencesManager.GetClimateZoneParams(calcInput.General.ClimateZone).SolarRadiation.Months[(int)item2.Month].AvgTemp;
			monthDataCooling.Month = item2;
			monthDataCooling.ParameterQtr = num2;
			monthDataCooling.ParameterNi = num3;
			monthDataCooling.ParameterHtr = section.Test.ParameterHtr;
			monthDataCooling.ParamHd = section.Test.ParameterHd;
			monthDataCooling.ParamHg = section.Test.ParameterHg;
			monthDataCooling.ParamHu = section.Test.ParameterHu;
			monthDataCooling.NetEnergyQnd = num4;
			monthDataCooling.ParameterGama = section.Test.ParameterGamma;
			list.Add(monthDataCooling);
		}
		MonthDataCoolingList = list;
		double num6 = list2.Aggregate(0.0, (double num9, double num10) => num9 + num10);
		calcData.ResulNoInputsNetEnergyActual = num6 / section.Area.HeatedArea;
		double num7 = (calcData.ResulCoolingInputsActual = list4.Aggregate(0.0, (double num9, double num10) => num9 + num10));
		calcData.ResulNetEnergyActual = num6 / section.Area.HeatedArea - num7 - calcData.ResulVentilationInputsActual;
	}

	private static void CalculateCoolingEnergyBaseLine(List<MonthlyDays> monthslist, CalculationData calcData, Section section, CalculationInput calcInput, CalculationData lightsAndDevicesCalculationData, CalculationData ventCool)
	{
		List<double> list = new List<double>();
		List<double> list2 = new List<double>();
		List<double> list3 = new List<double>();
		foreach (MonthlyDays item2 in monthslist)
		{
			double num = CalculateQgainBaseLine(section, calcInput.General.ClimateZone, item2, lightsAndDevicesCalculationData);
			double num2 = CalculateCoolingQtrBaseLine(calcData, section, calcInput.General.ClimateZone, item2) + CalculateQinfBaseLine(section, calcInput.General.ClimateZone, calcData, item2);
			double parameterAc = CalculateAcBaseLine(calcData, section, calcInput.General.ClimateZone, item2);
			double num3 = CalculateETA(parameterAc, num2, num, section);
			double num4 = num - num3 * num2 + CalculateQLatentOccupantsBaseLine(section, item2);
			num4 = num4 + CalculateLatentHeatsInfBaseLine(section, calcData, item2, calcInput.General.ClimateZone) + CalculateLatentHeatsVentBaseLine(section, calcData, item2, calcInput.General.ClimateZone, calcData);
			list.Add(num4);
			double num5 = ClaculateQfreecoolingBaseLine(section, calcData, item2, calcInput.General.ClimateZone);
			list3.Add(num5);
			double item = num4 + num5 + CalculateQveBaseLine(section, calcData, ventCool, item2);
			list2.Add(item);
		}
		double num6 = list.Aggregate(0.0, (double num9, double num10) => num9 + num10);
		calcData.ResulNoInputsNetEnergyBaseLine = num6 / section.Area.HeatedArea;
		double num7 = (calcData.ResulCoolingInputsBaseLine = list3.Aggregate(0.0, (double num9, double num10) => num9 + num10));
		calcData.ResulNetEnergyBaseLine = num6 / section.Area.HeatedArea - num7 - calcData.ResulVentilationInputsBaseLine;
	}

	private static void CalculateCoolingEnergyESM(List<MonthlyDays> monthslist, CalculationData calcData, Section section, CalculationInput calcInput, CalculationData lightsAndDevicesCalculationData, CalculationData ventCool)
	{
		List<double> list = new List<double>();
		List<double> list2 = new List<double>();
		List<double> list3 = new List<double>();
		foreach (MonthlyDays item2 in monthslist)
		{
			double num = CalculateQgainESM(section, calcInput.General.ClimateZone, item2, lightsAndDevicesCalculationData);
			double num2 = CalculateCoolingQtrESM(calcData, section, calcInput.General.ClimateZone, item2) + CalculateQinfESM(section, calcInput.General.ClimateZone, calcData, item2);
			double parameterAc = CalculateAcESM(calcData, section, calcInput.General.ClimateZone, item2);
			double num3 = CalculateETA(parameterAc, num2, num, section);
			double num4 = num - num3 * num2 + CalculateQLatentOccupantsESM(section, item2);
			num4 = num4 + CalculateLatentHeatsInfESM(section, calcData, item2, calcInput.General.ClimateZone) + CalculateLatentHeatsVentESM(section, calcData, item2, calcInput.General.ClimateZone, calcData);
			list.Add(num4);
			double num5 = ClaculateQfreecoolingEsm(section, calcData, item2, calcInput.General.ClimateZone);
			list3.Add(num5);
			double item = num4 + num5 + CalculateQveESM(section, calcData, ventCool, item2);
			list2.Add(item);
		}
		double num6 = list.Aggregate(0.0, (double num9, double num10) => num9 + num10);
		calcData.ResulNoInputsNetEnergyESM = num6 / section.Area.HeatedArea;
		double num7 = (calcData.ResulCoolingInputsESM = list3.Aggregate(0.0, (double num9, double num10) => num9 + num10));
		calcData.ResulNetEnergyESM = num6 / section.Area.HeatedArea - num7 - calcData.ResulVentilationInputsESM;
	}

	private static double CalculateLatentHeatsInfRef1(Section section, CalculationData calcData, MonthlyDays month, ClimateZones climateZone)
	{
		List<TempHumidityPerDay> daysHours = GetDaysHours(climateZone, (int)month.Month);
		List<double> list = new List<double>();
		double num = 0.0;
		double num2 = 0.0;
		for (int i = 0; i < section.CoolingSeasons.Occupants.WorkBaseStart; i++)
		{
			num += CalcRo(daysHours[i].Temp, daysHours[i].Humidity) * CalcAirX(daysHours[i].Temp, daysHours[i].Humidity) - CalcRo(calcData.NonProjectTemperatureRef1, calcData.ProjectHumidityRef1) * CalcAirX(calcData.NonProjectTemperatureRef1, calcData.ProjectHumidityRef1);
			list.Add(CalcRo(daysHours[i].Temp, daysHours[i].Humidity) * CalcAirX(daysHours[i].Temp, daysHours[i].Humidity) - CalcRo(calcData.NonProjectTemperatureRef1, calcData.ProjectHumidityRef1) * CalcAirX(calcData.NonProjectTemperatureRef1, calcData.ProjectHumidityRef1));
		}
		list.Clear();
		for (int j = section.CoolingSeasons.Occupants.WorkBaseStart; j < section.CoolingSeasons.Occupants.WorkBaseEnd; j++)
		{
			int index = ((j < daysHours.Count) ? j : 0);
			list.Add(CalcRo(daysHours[index].Temp, daysHours[index].Humidity) * CalcAirX(daysHours[index].Temp, daysHours[index].Humidity) - CalcRo(calcData.ProjectTemperatureRef1, calcData.ProjectHumidityRef1) * CalcAirX(calcData.ProjectTemperatureRef1, calcData.ProjectHumidityRef1));
			num2 += CalcRo(daysHours[index].Temp, daysHours[index].Humidity) * CalcAirX(daysHours[index].Temp, daysHours[index].Humidity) - CalcRo(calcData.ProjectTemperatureRef1, calcData.ProjectHumidityRef1) * CalcAirX(calcData.ProjectTemperatureRef1, calcData.ProjectHumidityRef1);
		}
		for (int k = section.CoolingSeasons.Occupants.WorkBaseEnd; k < 24; k++)
		{
			num += CalcRo(daysHours[k].Temp, daysHours[k].Humidity) * CalcAirX(daysHours[k].Temp, daysHours[k].Humidity) - CalcRo(calcData.NonProjectTemperatureRef1, calcData.ProjectHumidityRef1) * CalcAirX(calcData.NonProjectTemperatureRef1, calcData.ProjectHumidityRef1);
			list.Add(CalcRo(daysHours[k].Temp, daysHours[k].Humidity) * CalcAirX(daysHours[k].Temp, daysHours[k].Humidity) - CalcRo(calcData.NonProjectTemperatureRef1, calcData.ProjectHumidityRef1) * CalcAirX(calcData.NonProjectTemperatureRef1, calcData.ProjectHumidityRef1));
		}
		double num3 = (num + num2) * (double)month.WorkDays;
		double num4 = 0.0;
		double num5 = 0.0;
		for (int l = 0; l < section.CoolingSeasons.Occupants.SatBaseStart; l++)
		{
			num5 += CalcRo(daysHours[l].Temp, daysHours[l].Humidity) * CalcAirX(daysHours[l].Temp, daysHours[l].Humidity) - CalcRo(calcData.NonProjectTemperatureRef1, calcData.ProjectHumidityRef1) * CalcAirX(calcData.NonProjectTemperatureRef1, calcData.ProjectHumidityRef1);
		}
		for (int m = section.CoolingSeasons.Occupants.SatBaseStart; m < section.CoolingSeasons.Occupants.SatBaseEnd; m++)
		{
			int index2 = ((m < daysHours.Count) ? m : 0);
			num4 += CalcRo(daysHours[index2].Temp, daysHours[index2].Humidity) * CalcAirX(daysHours[index2].Temp, daysHours[index2].Humidity) - CalcRo(calcData.ProjectTemperatureRef1, calcData.ProjectHumidityRef1) * CalcAirX(calcData.ProjectTemperatureRef1, calcData.ProjectHumidityRef1);
		}
		for (int n = section.CoolingSeasons.Occupants.SatBaseEnd; n < 24; n++)
		{
			num5 += CalcRo(daysHours[n].Temp, daysHours[n].Humidity) * CalcAirX(daysHours[n].Temp, daysHours[n].Humidity) - CalcRo(calcData.NonProjectTemperatureRef1, calcData.ProjectHumidityRef1) * CalcAirX(calcData.NonProjectTemperatureRef1, calcData.ProjectHumidityRef1);
		}
		double num6 = (num4 + num5) * (double)month.Saturdays;
		double num7 = 0.0;
		double num8 = 0.0;
		for (int num9 = 0; num9 < section.CoolingSeasons.Occupants.SunBaseStart; num9++)
		{
			num8 += CalcRo(daysHours[num9].Temp, daysHours[num9].Humidity) * CalcAirX(daysHours[num9].Temp, daysHours[num9].Humidity) - CalcRo(calcData.NonProjectTemperatureRef1, calcData.ProjectHumidityRef1) * CalcAirX(calcData.NonProjectTemperatureRef1, calcData.ProjectHumidityRef1);
		}
		for (int num10 = section.CoolingSeasons.Occupants.SunBaseStart; num10 < section.CoolingSeasons.Occupants.SunBaseEnd; num10++)
		{
			int index3 = ((num10 < daysHours.Count) ? num10 : 0);
			num7 += CalcRo(daysHours[index3].Temp, daysHours[index3].Humidity) * CalcAirX(daysHours[index3].Temp, daysHours[index3].Humidity) - CalcRo(calcData.ProjectTemperatureRef1, calcData.ProjectHumidityRef1) * CalcAirX(calcData.ProjectTemperatureRef1, calcData.ProjectHumidityRef1);
		}
		for (int num11 = section.CoolingSeasons.Occupants.SunBaseEnd; num11 < 24; num11++)
		{
			num8 += CalcRo(daysHours[num11].Temp, daysHours[num11].Humidity) * CalcAirX(daysHours[num11].Temp, daysHours[num11].Humidity) - CalcRo(calcData.NonProjectTemperatureRef1, calcData.ProjectHumidityRef1) * CalcAirX(calcData.NonProjectTemperatureRef1, calcData.ProjectHumidityRef1);
		}
		double num12 = (num7 + num8) * (double)month.Sundays;
		double num13 = section.Area.HeatedVolume * calcData.InfiltracionRef1 * (1.0 / section.Area.HeatedArea) * (num3 + num6 + num12) * 0.6947222222222222;
		if (double.IsNaN(num13) || double.IsInfinity(num13))
		{
			num13 = 0.0;
		}
		return num13;
	}

	private static double CalculateLatentHeatsInfRef2(Section section, CalculationData calcData, MonthlyDays month, ClimateZones climateZone)
	{
		List<TempHumidityPerDay> daysHours = GetDaysHours(climateZone, (int)month.Month);
		List<double> list = new List<double>();
		double num = 0.0;
		double num2 = 0.0;
		for (int i = 0; i < section.CoolingSeasons.Occupants.WorkBaseStart; i++)
		{
			num += CalcRo(daysHours[i].Temp, daysHours[i].Humidity) * CalcAirX(daysHours[i].Temp, daysHours[i].Humidity) - CalcRo(calcData.NonProjectTemperatureRef2, calcData.ProjectHumidityRef2) * CalcAirX(calcData.NonProjectTemperatureRef2, calcData.ProjectHumidityRef2);
			list.Add(CalcRo(daysHours[i].Temp, daysHours[i].Humidity) * CalcAirX(daysHours[i].Temp, daysHours[i].Humidity) - CalcRo(calcData.NonProjectTemperatureRef2, calcData.ProjectHumidityRef2) * CalcAirX(calcData.NonProjectTemperatureRef2, calcData.ProjectHumidityRef2));
		}
		list.Clear();
		for (int j = section.CoolingSeasons.Occupants.WorkBaseStart; j < section.CoolingSeasons.Occupants.WorkBaseEnd; j++)
		{
			int index = ((j < daysHours.Count) ? j : 0);
			list.Add(CalcRo(daysHours[index].Temp, daysHours[index].Humidity) * CalcAirX(daysHours[index].Temp, daysHours[index].Humidity) - CalcRo(calcData.ProjectTemperatureRef2, calcData.ProjectHumidityRef2) * CalcAirX(calcData.ProjectTemperatureRef2, calcData.ProjectHumidityRef2));
			num2 += CalcRo(daysHours[index].Temp, daysHours[index].Humidity) * CalcAirX(daysHours[index].Temp, daysHours[index].Humidity) - CalcRo(calcData.ProjectTemperatureRef2, calcData.ProjectHumidityRef2) * CalcAirX(calcData.ProjectTemperatureRef2, calcData.ProjectHumidityRef2);
		}
		for (int k = section.CoolingSeasons.Occupants.WorkBaseEnd; k < 24; k++)
		{
			num += CalcRo(daysHours[k].Temp, daysHours[k].Humidity) * CalcAirX(daysHours[k].Temp, daysHours[k].Humidity) - CalcRo(calcData.NonProjectTemperatureRef2, calcData.ProjectHumidityRef2) * CalcAirX(calcData.NonProjectTemperatureRef2, calcData.ProjectHumidityRef2);
			list.Add(CalcRo(daysHours[k].Temp, daysHours[k].Humidity) * CalcAirX(daysHours[k].Temp, daysHours[k].Humidity) - CalcRo(calcData.NonProjectTemperatureRef2, calcData.ProjectHumidityRef2) * CalcAirX(calcData.NonProjectTemperatureRef2, calcData.ProjectHumidityRef2));
		}
		double num3 = (num + num2) * (double)month.WorkDays;
		double num4 = 0.0;
		double num5 = 0.0;
		for (int l = 0; l < section.CoolingSeasons.Occupants.SatBaseStart; l++)
		{
			num5 += CalcRo(daysHours[l].Temp, daysHours[l].Humidity) * CalcAirX(daysHours[l].Temp, daysHours[l].Humidity) - CalcRo(calcData.NonProjectTemperatureRef2, calcData.ProjectHumidityRef2) * CalcAirX(calcData.NonProjectTemperatureRef2, calcData.ProjectHumidityRef2);
		}
		for (int m = section.CoolingSeasons.Occupants.SatBaseStart; m < section.CoolingSeasons.Occupants.SatBaseEnd; m++)
		{
			int index2 = ((m < daysHours.Count) ? m : 0);
			num4 += CalcRo(daysHours[index2].Temp, daysHours[index2].Humidity) * CalcAirX(daysHours[index2].Temp, daysHours[index2].Humidity) - CalcRo(calcData.ProjectTemperatureRef2, calcData.ProjectHumidityRef2) * CalcAirX(calcData.ProjectTemperatureRef2, calcData.ProjectHumidityRef2);
		}
		for (int n = section.CoolingSeasons.Occupants.SatBaseEnd; n < 24; n++)
		{
			num5 += CalcRo(daysHours[n].Temp, daysHours[n].Humidity) * CalcAirX(daysHours[n].Temp, daysHours[n].Humidity) - CalcRo(calcData.NonProjectTemperatureRef2, calcData.ProjectHumidityRef2) * CalcAirX(calcData.NonProjectTemperatureRef2, calcData.ProjectHumidityRef2);
		}
		double num6 = (num4 + num5) * (double)month.Saturdays;
		double num7 = 0.0;
		double num8 = 0.0;
		for (int num9 = 0; num9 < section.CoolingSeasons.Occupants.SunBaseStart; num9++)
		{
			num8 += CalcRo(daysHours[num9].Temp, daysHours[num9].Humidity) * CalcAirX(daysHours[num9].Temp, daysHours[num9].Humidity) - CalcRo(calcData.NonProjectTemperatureRef2, calcData.ProjectHumidityRef2) * CalcAirX(calcData.NonProjectTemperatureRef2, calcData.ProjectHumidityRef2);
		}
		for (int num10 = section.CoolingSeasons.Occupants.SunBaseStart; num10 < section.CoolingSeasons.Occupants.SunBaseEnd; num10++)
		{
			int index3 = ((num10 < daysHours.Count) ? num10 : 0);
			num7 += CalcRo(daysHours[index3].Temp, daysHours[index3].Humidity) * CalcAirX(daysHours[index3].Temp, daysHours[index3].Humidity) - CalcRo(calcData.ProjectTemperatureRef2, calcData.ProjectHumidityRef2) * CalcAirX(calcData.ProjectTemperatureRef2, calcData.ProjectHumidityRef2);
		}
		for (int num11 = section.CoolingSeasons.Occupants.SunBaseEnd; num11 < 24; num11++)
		{
			num8 += CalcRo(daysHours[num11].Temp, daysHours[num11].Humidity) * CalcAirX(daysHours[num11].Temp, daysHours[num11].Humidity) - CalcRo(calcData.NonProjectTemperatureRef2, calcData.ProjectHumidityRef2) * CalcAirX(calcData.NonProjectTemperatureRef2, calcData.ProjectHumidityRef2);
		}
		double num12 = (num7 + num8) * (double)month.Sundays;
		double num13 = section.Area.HeatedVolume * calcData.InfiltracionRef2 * (1.0 / section.Area.HeatedArea) * (num3 + num6 + num12) * 0.6947222222222222;
		if (double.IsNaN(num13) || double.IsInfinity(num13))
		{
			num13 = 0.0;
		}
		return num13;
	}

	private static double CalculateLatentHeatsInf(Section section, CalculationData calcData, MonthlyDays month, ClimateZones climateZone)
	{
		List<TempHumidityPerDay> daysHours = GetDaysHours(climateZone, (int)month.Month);
		List<double> list = new List<double>();
		double num = 0.0;
		double num2 = 0.0;
		for (int i = 0; i < section.CoolingSeasons.Occupants.WorkCurrentStart; i++)
		{
			num += CalcRo(daysHours[i].Temp, daysHours[i].Humidity) * CalcAirX(daysHours[i].Temp, daysHours[i].Humidity) - CalcRo(calcData.NonProjectTemperatureActual, calcData.ProjectHumidityActual) * CalcAirX(calcData.NonProjectTemperatureActual, calcData.ProjectHumidityActual);
			list.Add(CalcRo(daysHours[i].Temp, daysHours[i].Humidity) * CalcAirX(daysHours[i].Temp, daysHours[i].Humidity) - CalcRo(calcData.NonProjectTemperatureActual, calcData.ProjectHumidityActual) * CalcAirX(calcData.NonProjectTemperatureActual, calcData.ProjectHumidityActual));
		}
		list.Clear();
		for (int j = section.CoolingSeasons.Occupants.WorkCurrentStart; j < section.CoolingSeasons.Occupants.WorkCurrentEnd; j++)
		{
			int index = ((j < daysHours.Count) ? j : 0);
			list.Add(CalcRo(daysHours[index].Temp, daysHours[index].Humidity) * CalcAirX(daysHours[index].Temp, daysHours[index].Humidity) - CalcRo(calcData.ProjectTemperatureActual, calcData.ProjectHumidityActual) * CalcAirX(calcData.ProjectTemperatureActual, calcData.ProjectHumidityActual));
			num2 += CalcRo(daysHours[index].Temp, daysHours[index].Humidity) * CalcAirX(daysHours[index].Temp, daysHours[index].Humidity) - CalcRo(calcData.ProjectTemperatureActual, calcData.ProjectHumidityActual) * CalcAirX(calcData.ProjectTemperatureActual, calcData.ProjectHumidityActual);
		}
		for (int k = section.CoolingSeasons.Occupants.WorkCurrentEnd; k < 24; k++)
		{
			num += CalcRo(daysHours[k].Temp, daysHours[k].Humidity) * CalcAirX(daysHours[k].Temp, daysHours[k].Humidity) - CalcRo(calcData.NonProjectTemperatureActual, calcData.ProjectHumidityActual) * CalcAirX(calcData.NonProjectTemperatureActual, calcData.ProjectHumidityActual);
			list.Add(CalcRo(daysHours[k].Temp, daysHours[k].Humidity) * CalcAirX(daysHours[k].Temp, daysHours[k].Humidity) - CalcRo(calcData.NonProjectTemperatureActual, calcData.ProjectHumidityActual) * CalcAirX(calcData.NonProjectTemperatureActual, calcData.ProjectHumidityActual));
		}
		double num3 = (num + num2) * (double)month.WorkDays;
		double num4 = 0.0;
		double num5 = 0.0;
		for (int l = 0; l < section.CoolingSeasons.Occupants.SatCurrentStart; l++)
		{
			num5 += CalcRo(daysHours[l].Temp, daysHours[l].Humidity) * CalcAirX(daysHours[l].Temp, daysHours[l].Humidity) - CalcRo(calcData.NonProjectTemperatureActual, calcData.ProjectHumidityActual) * CalcAirX(calcData.NonProjectTemperatureActual, calcData.ProjectHumidityActual);
		}
		for (int m = section.CoolingSeasons.Occupants.SatCurrentStart; m < section.CoolingSeasons.Occupants.SatCurrentEnd; m++)
		{
			int index2 = ((m < daysHours.Count) ? m : 0);
			num4 += CalcRo(daysHours[index2].Temp, daysHours[index2].Humidity) * CalcAirX(daysHours[index2].Temp, daysHours[index2].Humidity) - CalcRo(calcData.ProjectTemperatureActual, calcData.ProjectHumidityActual) * CalcAirX(calcData.ProjectTemperatureActual, calcData.ProjectHumidityActual);
		}
		for (int n = section.CoolingSeasons.Occupants.SatCurrentEnd; n < 24; n++)
		{
			num5 += CalcRo(daysHours[n].Temp, daysHours[n].Humidity) * CalcAirX(daysHours[n].Temp, daysHours[n].Humidity) - CalcRo(calcData.NonProjectTemperatureActual, calcData.ProjectHumidityActual) * CalcAirX(calcData.NonProjectTemperatureActual, calcData.ProjectHumidityActual);
		}
		double num6 = (num4 + num5) * (double)month.Saturdays;
		double num7 = 0.0;
		double num8 = 0.0;
		for (int num9 = 0; num9 < section.CoolingSeasons.Occupants.SunCurrentStart; num9++)
		{
			num8 += CalcRo(daysHours[num9].Temp, daysHours[num9].Humidity) * CalcAirX(daysHours[num9].Temp, daysHours[num9].Humidity) - CalcRo(calcData.NonProjectTemperatureActual, calcData.ProjectHumidityActual) * CalcAirX(calcData.NonProjectTemperatureActual, calcData.ProjectHumidityActual);
		}
		for (int num10 = section.CoolingSeasons.Occupants.SunCurrentStart; num10 < section.CoolingSeasons.Occupants.SunCurrentEnd; num10++)
		{
			int index3 = ((num10 < daysHours.Count) ? num10 : 0);
			num7 += CalcRo(daysHours[index3].Temp, daysHours[index3].Humidity) * CalcAirX(daysHours[index3].Temp, daysHours[index3].Humidity) - CalcRo(calcData.ProjectTemperatureActual, calcData.ProjectHumidityActual) * CalcAirX(calcData.ProjectTemperatureActual, calcData.ProjectHumidityActual);
		}
		for (int num11 = section.CoolingSeasons.Occupants.SunCurrentEnd; num11 < 24; num11++)
		{
			num8 += CalcRo(daysHours[num11].Temp, daysHours[num11].Humidity) * CalcAirX(daysHours[num11].Temp, daysHours[num11].Humidity) - CalcRo(calcData.NonProjectTemperatureActual, calcData.ProjectHumidityActual) * CalcAirX(calcData.NonProjectTemperatureActual, calcData.ProjectHumidityActual);
		}
		double num12 = (num7 + num8) * (double)month.Sundays;
		double num13 = section.Area.HeatedVolume * calcData.InfiltracionActual * (num3 + num6 + num12) * 0.6947222222222222 / section.Area.HeatedArea;
		if (double.IsNaN(num13) || double.IsInfinity(num13))
		{
			num13 = 0.0;
		}
		return num13;
	}

	private static double CalculateLatentHeatsInfBaseLine(Section section, CalculationData calcData, MonthlyDays month, ClimateZones climateZone)
	{
		List<TempHumidityPerDay> daysHours = GetDaysHours(climateZone, (int)month.Month);
		List<double> list = new List<double>();
		double num = 0.0;
		double num2 = 0.0;
		for (int i = 0; i < section.CoolingSeasons.Occupants.WorkBaseStart; i++)
		{
			num += CalcRo(daysHours[i].Temp, daysHours[i].Humidity) * CalcAirX(daysHours[i].Temp, daysHours[i].Humidity) - CalcRo(calcData.NonProjectTemperatureBaseLine, calcData.ProjectHumidityBaseLine) * CalcAirX(calcData.NonProjectTemperatureBaseLine, calcData.ProjectHumidityBaseLine);
			list.Add(CalcRo(daysHours[i].Temp, daysHours[i].Humidity) * CalcAirX(daysHours[i].Temp, daysHours[i].Humidity) - CalcRo(calcData.NonProjectTemperatureBaseLine, calcData.ProjectHumidityBaseLine) * CalcAirX(calcData.NonProjectTemperatureBaseLine, calcData.ProjectHumidityBaseLine));
		}
		list.Clear();
		for (int j = section.CoolingSeasons.Occupants.WorkBaseStart; j < section.CoolingSeasons.Occupants.WorkBaseEnd; j++)
		{
			int index = ((j < daysHours.Count) ? j : 0);
			list.Add(CalcRo(daysHours[index].Temp, daysHours[index].Humidity) * CalcAirX(daysHours[index].Temp, daysHours[index].Humidity) - CalcRo(calcData.ProjectTemperatureBaseLine, calcData.ProjectHumidityBaseLine) * CalcAirX(calcData.ProjectTemperatureBaseLine, calcData.ProjectHumidityBaseLine));
			num2 += CalcRo(daysHours[index].Temp, daysHours[index].Humidity) * CalcAirX(daysHours[index].Temp, daysHours[index].Humidity) - CalcRo(calcData.ProjectTemperatureBaseLine, calcData.ProjectHumidityBaseLine) * CalcAirX(calcData.ProjectTemperatureBaseLine, calcData.ProjectHumidityBaseLine);
		}
		for (int k = section.CoolingSeasons.Occupants.WorkBaseEnd; k < 24; k++)
		{
			num += CalcRo(daysHours[k].Temp, daysHours[k].Humidity) * CalcAirX(daysHours[k].Temp, daysHours[k].Humidity) - CalcRo(calcData.NonProjectTemperatureBaseLine, calcData.ProjectHumidityBaseLine) * CalcAirX(calcData.NonProjectTemperatureBaseLine, calcData.ProjectHumidityBaseLine);
			list.Add(CalcRo(daysHours[k].Temp, daysHours[k].Humidity) * CalcAirX(daysHours[k].Temp, daysHours[k].Humidity) - CalcRo(calcData.NonProjectTemperatureBaseLine, calcData.ProjectHumidityBaseLine) * CalcAirX(calcData.NonProjectTemperatureBaseLine, calcData.ProjectHumidityBaseLine));
		}
		double num3 = (num + num2) * (double)month.WorkDays;
		double num4 = 0.0;
		double num5 = 0.0;
		for (int l = 0; l < section.CoolingSeasons.Occupants.SatBaseStart; l++)
		{
			num5 += CalcRo(daysHours[l].Temp, daysHours[l].Humidity) * CalcAirX(daysHours[l].Temp, daysHours[l].Humidity) - CalcRo(calcData.NonProjectTemperatureBaseLine, calcData.ProjectHumidityBaseLine) * CalcAirX(calcData.NonProjectTemperatureBaseLine, calcData.ProjectHumidityBaseLine);
		}
		for (int m = section.CoolingSeasons.Occupants.SatBaseStart; m < section.CoolingSeasons.Occupants.SatBaseEnd; m++)
		{
			int index2 = ((m < daysHours.Count) ? m : 0);
			num4 += CalcRo(daysHours[index2].Temp, daysHours[index2].Humidity) * CalcAirX(daysHours[index2].Temp, daysHours[index2].Humidity) - CalcRo(calcData.ProjectTemperatureBaseLine, calcData.ProjectHumidityBaseLine) * CalcAirX(calcData.ProjectTemperatureBaseLine, calcData.ProjectHumidityBaseLine);
		}
		for (int n = section.CoolingSeasons.Occupants.SatBaseEnd; n < 24; n++)
		{
			num5 += CalcRo(daysHours[n].Temp, daysHours[n].Humidity) * CalcAirX(daysHours[n].Temp, daysHours[n].Humidity) - CalcRo(calcData.NonProjectTemperatureBaseLine, calcData.ProjectHumidityBaseLine) * CalcAirX(calcData.NonProjectTemperatureBaseLine, calcData.ProjectHumidityBaseLine);
		}
		double num6 = (num4 + num5) * (double)month.Saturdays;
		double num7 = 0.0;
		double num8 = 0.0;
		for (int num9 = 0; num9 < section.CoolingSeasons.Occupants.SunBaseStart; num9++)
		{
			num8 += CalcRo(daysHours[num9].Temp, daysHours[num9].Humidity) * CalcAirX(daysHours[num9].Temp, daysHours[num9].Humidity) - CalcRo(calcData.NonProjectTemperatureBaseLine, calcData.ProjectHumidityBaseLine) * CalcAirX(calcData.NonProjectTemperatureBaseLine, calcData.ProjectHumidityBaseLine);
		}
		for (int num10 = section.CoolingSeasons.Occupants.SunBaseStart; num10 < section.CoolingSeasons.Occupants.SunBaseEnd; num10++)
		{
			int index3 = ((num10 < daysHours.Count) ? num10 : 0);
			num7 += CalcRo(daysHours[index3].Temp, daysHours[index3].Humidity) * CalcAirX(daysHours[index3].Temp, daysHours[index3].Humidity) - CalcRo(calcData.ProjectTemperatureBaseLine, calcData.ProjectHumidityBaseLine) * CalcAirX(calcData.ProjectTemperatureBaseLine, calcData.ProjectHumidityBaseLine);
		}
		for (int num11 = section.CoolingSeasons.Occupants.SunBaseEnd; num11 < 24; num11++)
		{
			num8 += CalcRo(daysHours[num11].Temp, daysHours[num11].Humidity) * CalcAirX(daysHours[num11].Temp, daysHours[num11].Humidity) - CalcRo(calcData.NonProjectTemperatureBaseLine, calcData.ProjectHumidityBaseLine) * CalcAirX(calcData.NonProjectTemperatureBaseLine, calcData.ProjectHumidityBaseLine);
		}
		double num12 = (num7 + num8) * (double)month.Sundays;
		double num13 = section.Area.HeatedVolume * calcData.InfiltracionBaseLine * (1.0 / section.Area.HeatedArea) * (num3 + num6 + num12) * 0.6947222222222222;
		if (double.IsNaN(num13) || double.IsInfinity(num13))
		{
			num13 = 0.0;
		}
		return num13;
	}

	private static double CalculateLatentHeatsInfESM(Section section, CalculationData calcData, MonthlyDays month, ClimateZones climateZone)
	{
		List<TempHumidityPerDay> daysHours = GetDaysHours(climateZone, (int)month.Month);
		List<double> list = new List<double>();
		double num = 0.0;
		double num2 = 0.0;
		for (int i = 0; i < section.CoolingSeasons.Occupants.WorkEsmStart; i++)
		{
			num += CalcRo(daysHours[i].Temp, daysHours[i].Humidity) * CalcAirX(daysHours[i].Temp, daysHours[i].Humidity) - CalcRo(calcData.NonProjectTemperatureESM, calcData.ProjectHumidityESM) * CalcAirX(calcData.NonProjectTemperatureESM, calcData.ProjectHumidityESM);
			list.Add(CalcRo(daysHours[i].Temp, daysHours[i].Humidity) * CalcAirX(daysHours[i].Temp, daysHours[i].Humidity) - CalcRo(calcData.NonProjectTemperatureESM, calcData.ProjectHumidityESM) * CalcAirX(calcData.NonProjectTemperatureESM, calcData.ProjectHumidityESM));
		}
		list.Clear();
		for (int j = section.CoolingSeasons.Occupants.WorkEsmStart; j < section.CoolingSeasons.Occupants.WorkEsmEnd; j++)
		{
			int index = ((j < daysHours.Count) ? j : 0);
			list.Add(CalcRo(daysHours[index].Temp, daysHours[index].Humidity) * CalcAirX(daysHours[index].Temp, daysHours[index].Humidity) - CalcRo(calcData.ProjectTemperatureESM, calcData.ProjectHumidityESM) * CalcAirX(calcData.ProjectTemperatureESM, calcData.ProjectHumidityESM));
			num2 += CalcRo(daysHours[index].Temp, daysHours[index].Humidity) * CalcAirX(daysHours[index].Temp, daysHours[index].Humidity) - CalcRo(calcData.ProjectTemperatureESM, calcData.ProjectHumidityESM) * CalcAirX(calcData.ProjectTemperatureESM, calcData.ProjectHumidityESM);
		}
		for (int k = section.CoolingSeasons.Occupants.WorkEsmEnd; k < 24; k++)
		{
			num += CalcRo(daysHours[k].Temp, daysHours[k].Humidity) * CalcAirX(daysHours[k].Temp, daysHours[k].Humidity) - CalcRo(calcData.NonProjectTemperatureESM, calcData.ProjectHumidityESM) * CalcAirX(calcData.NonProjectTemperatureESM, calcData.ProjectHumidityESM);
			list.Add(CalcRo(daysHours[k].Temp, daysHours[k].Humidity) * CalcAirX(daysHours[k].Temp, daysHours[k].Humidity) - CalcRo(calcData.NonProjectTemperatureESM, calcData.ProjectHumidityESM) * CalcAirX(calcData.NonProjectTemperatureESM, calcData.ProjectHumidityESM));
		}
		double num3 = (num + num2) * (double)month.WorkDays;
		double num4 = 0.0;
		double num5 = 0.0;
		for (int l = 0; l < section.CoolingSeasons.Occupants.SatEsmStart; l++)
		{
			num5 += CalcRo(daysHours[l].Temp, daysHours[l].Humidity) * CalcAirX(daysHours[l].Temp, daysHours[l].Humidity) - CalcRo(calcData.NonProjectTemperatureESM, calcData.ProjectHumidityESM) * CalcAirX(calcData.NonProjectTemperatureESM, calcData.ProjectHumidityESM);
		}
		for (int m = section.CoolingSeasons.Occupants.SatEsmStart; m < section.CoolingSeasons.Occupants.SatEsmEnd; m++)
		{
			int index2 = ((m < daysHours.Count) ? m : 0);
			num4 += CalcRo(daysHours[index2].Temp, daysHours[index2].Humidity) * CalcAirX(daysHours[index2].Temp, daysHours[index2].Humidity) - CalcRo(calcData.ProjectTemperatureESM, calcData.ProjectHumidityESM) * CalcAirX(calcData.ProjectTemperatureESM, calcData.ProjectHumidityESM);
		}
		for (int n = section.CoolingSeasons.Occupants.SatEsmEnd; n < 24; n++)
		{
			num5 += CalcRo(daysHours[n].Temp, daysHours[n].Humidity) * CalcAirX(daysHours[n].Temp, daysHours[n].Humidity) - CalcRo(calcData.NonProjectTemperatureESM, calcData.ProjectHumidityESM) * CalcAirX(calcData.NonProjectTemperatureESM, calcData.ProjectHumidityESM);
		}
		double num6 = (num4 + num5) * (double)month.Saturdays;
		double num7 = 0.0;
		double num8 = 0.0;
		for (int num9 = 0; num9 < section.CoolingSeasons.Occupants.SunEsmStart; num9++)
		{
			num8 += CalcRo(daysHours[num9].Temp, daysHours[num9].Humidity) * CalcAirX(daysHours[num9].Temp, daysHours[num9].Humidity) - CalcRo(calcData.NonProjectTemperatureESM, calcData.ProjectHumidityESM) * CalcAirX(calcData.NonProjectTemperatureESM, calcData.ProjectHumidityESM);
		}
		for (int num10 = section.CoolingSeasons.Occupants.SunEsmStart; num10 < section.CoolingSeasons.Occupants.SunEsmEnd; num10++)
		{
			int index3 = ((num10 < daysHours.Count) ? num10 : 0);
			num7 += CalcRo(daysHours[index3].Temp, daysHours[index3].Humidity) * CalcAirX(daysHours[index3].Temp, daysHours[index3].Humidity) - CalcRo(calcData.ProjectTemperatureESM, calcData.ProjectHumidityESM) * CalcAirX(calcData.ProjectTemperatureESM, calcData.ProjectHumidityESM);
		}
		for (int num11 = section.CoolingSeasons.Occupants.SunEsmEnd; num11 < 24; num11++)
		{
			num8 += CalcRo(daysHours[num11].Temp, daysHours[num11].Humidity) * CalcAirX(daysHours[num11].Temp, daysHours[num11].Humidity) - CalcRo(calcData.NonProjectTemperatureESM, calcData.ProjectHumidityESM) * CalcAirX(calcData.NonProjectTemperatureESM, calcData.ProjectHumidityESM);
		}
		double num12 = (num7 + num8) * (double)month.Sundays;
		double num13 = section.Area.HeatedVolume * calcData.InfiltracionESM * (1.0 / section.Area.HeatedArea) * (num3 + num6 + num12) * 0.6947222222222222;
		if (double.IsNaN(num13) || double.IsInfinity(num13))
		{
			num13 = 0.0;
		}
		return num13;
	}

	private static double CalculateLatentHeatsVentRef1(Section section, CalculationData calcData, MonthlyDays month, ClimateZones climateZone, CalculationData ventCool)
	{
		List<TempHumidityPerDay> daysHours = GetDaysHours(climateZone, (int)month.Month);
		double num = 0.0;
		double num2 = 0.0;
		for (int i = 0; i < section.CoolingSeasons.Ventilation.WorkBaseStart; i++)
		{
			num2 += ventCool.DebitRef1 * (CalcRoW(ventCool.FlowTemperatureRef1) * CalcAirX(ventCool.FlowTemperatureRef1, ventCool.RelativeHumidityRef1) - CalcRoW(daysHours[i].Temp) * CalcAirX(daysHours[i].Temp, daysHours[i].Humidity)) * 0.6947222222222222;
		}
		for (int j = section.CoolingSeasons.Ventilation.WorkBaseStart; j < section.CoolingSeasons.Ventilation.WorkBaseEnd; j++)
		{
			num += ventCool.DebitRef1 * (CalcRoW(ventCool.FlowTemperatureRef1) * CalcAirX(ventCool.FlowTemperatureRef1, ventCool.RelativeHumidityRef1) - CalcRoW(daysHours[j].Temp) * CalcAirX(daysHours[j].Temp, daysHours[j].Humidity)) * 0.6947222222222222;
		}
		for (int k = section.CoolingSeasons.Ventilation.WorkBaseEnd; k < 24; k++)
		{
			num2 += ventCool.DebitRef1 * (CalcRoW(ventCool.FlowTemperatureRef1) * CalcAirX(ventCool.FlowTemperatureRef1, ventCool.RelativeHumidityRef1) - CalcRoW(daysHours[k].Temp) * CalcAirX(daysHours[k].Temp, daysHours[k].Humidity)) * 0.6947222222222222;
		}
		double num3 = (num + num2) * (double)month.WorkDays;
		num3 = ((double.IsNaN(num3) || double.IsInfinity(num3)) ? 0.0 : num3);
		double num4 = 0.0;
		double num5 = 0.0;
		for (int l = 0; l < section.CoolingSeasons.Ventilation.SatBaseStart; l++)
		{
			num5 += ventCool.DebitRef1 * (CalcRoW(ventCool.FlowTemperatureRef1) * CalcAirX(ventCool.FlowTemperatureRef1, ventCool.RelativeHumidityRef1) - CalcRoW(daysHours[l].Temp) * CalcAirX(daysHours[l].Temp, daysHours[l].Humidity)) * 0.6947222222222222;
		}
		for (int m = section.CoolingSeasons.Ventilation.SatBaseStart; m < section.CoolingSeasons.Ventilation.SatBaseEnd; m++)
		{
			num4 += ventCool.DebitRef1 * (CalcRoW(ventCool.FlowTemperatureRef1) * CalcAirX(ventCool.FlowTemperatureRef1, ventCool.RelativeHumidityRef1) - CalcRoW(daysHours[m].Temp) * CalcAirX(daysHours[m].Temp, daysHours[m].Humidity)) * 0.6947222222222222;
		}
		for (int n = section.CoolingSeasons.Ventilation.SatBaseEnd; n < 24; n++)
		{
			num5 += ventCool.DebitRef1 * (ventCool.DebitRef1 * (CalcRoW(ventCool.FlowTemperatureRef1) * CalcAirX(ventCool.FlowTemperatureRef1, ventCool.RelativeHumidityRef1) - CalcRoW(daysHours[n].Temp) * CalcAirX(daysHours[n].Temp, daysHours[n].Humidity))) * 0.6947222222222222;
		}
		double num6 = (num4 + num5) * (double)month.Saturdays;
		num6 = ((double.IsNaN(num6) || double.IsInfinity(num6)) ? 0.0 : num6);
		double num7 = 0.0;
		double num8 = 0.0;
		for (int num9 = 0; num9 < section.CoolingSeasons.Ventilation.SunBaseStart; num9++)
		{
			num8 += ventCool.DebitRef1 * (CalcRoW(ventCool.FlowTemperatureRef1) * CalcAirX(ventCool.FlowTemperatureRef1, ventCool.RelativeHumidityRef1) - CalcRoW(daysHours[num9].Temp) * CalcAirX(daysHours[num9].Temp, daysHours[num9].Humidity)) * 0.6947222222222222;
		}
		for (int num10 = section.CoolingSeasons.Ventilation.SunBaseStart; num10 < section.CoolingSeasons.Ventilation.SunBaseEnd; num10++)
		{
			num7 += ventCool.DebitRef1 * (CalcRoW(ventCool.FlowTemperatureRef1) * CalcAirX(ventCool.FlowTemperatureRef1, ventCool.RelativeHumidityRef1) - CalcRoW(daysHours[num10].Temp) * CalcAirX(daysHours[num10].Temp, daysHours[num10].Humidity)) * 0.6947222222222222;
		}
		for (int num11 = section.CoolingSeasons.Ventilation.SunBaseEnd; num11 < 24; num11++)
		{
			num8 += ventCool.DebitRef1 * (CalcRoW(ventCool.FlowTemperatureRef1) * CalcAirX(ventCool.FlowTemperatureRef1, ventCool.RelativeHumidityRef1) - CalcRoW(daysHours[num11].Temp) * CalcAirX(daysHours[num11].Temp, daysHours[num11].Humidity)) * 0.6947222222222222;
		}
		double num12 = (num7 + num8) * (double)month.Sundays;
		num12 = ((double.IsNaN(num12) || double.IsInfinity(num12)) ? 0.0 : num12);
		double num13 = 0.0;
		for (int num14 = 0; num14 < 24; num14++)
		{
			num13 += ventCool.DebitRef1 * (CalcRoW(ventCool.FlowTemperatureRef1) * CalcAirX(ventCool.FlowTemperatureRef1, ventCool.RelativeHumidityRef1) - CalcRoW(calcData.NonProjectTemperatureRef1) * CalcAirX(calcData.NonProjectTemperatureRef1, calcData.ProjectHumidityRef1)) * 0.6947222222222222;
		}
		double num15 = num13 * (double)month.Holydays;
		num15 = ((double.IsNaN(num15) || double.IsInfinity(num15)) ? 0.0 : num15);
		return num3 + num6 + num12 + num15;
	}

	private static double CalculateLatentHeatsVentRef2(Section section, CalculationData calcData, MonthlyDays month, ClimateZones climateZone, CalculationData ventCool)
	{
		List<TempHumidityPerDay> daysHours = GetDaysHours(climateZone, (int)month.Month);
		double num = 0.0;
		double num2 = 0.0;
		for (int i = 0; i < section.CoolingSeasons.Ventilation.WorkBaseStart; i++)
		{
			num2 += ventCool.DebitRef2 * (CalcRoW(ventCool.FlowTemperatureRef2) * CalcAirX(ventCool.FlowTemperatureRef2, ventCool.RelativeHumidityRef2) - CalcRoW(daysHours[i].Temp) * CalcAirX(daysHours[i].Temp, daysHours[i].Humidity)) * 0.6947222222222222;
		}
		for (int j = section.CoolingSeasons.Ventilation.WorkBaseStart; j < section.CoolingSeasons.Ventilation.WorkBaseEnd; j++)
		{
			num += ventCool.DebitRef2 * (CalcRoW(ventCool.FlowTemperatureRef2) * CalcAirX(ventCool.FlowTemperatureRef2, ventCool.RelativeHumidityRef2) - CalcRoW(daysHours[j].Temp) * CalcAirX(daysHours[j].Temp, daysHours[j].Humidity)) * 0.6947222222222222;
		}
		for (int k = section.CoolingSeasons.Ventilation.WorkBaseEnd; k < 24; k++)
		{
			num2 += ventCool.DebitRef2 * (CalcRoW(ventCool.FlowTemperatureRef2) * CalcAirX(ventCool.FlowTemperatureRef2, ventCool.RelativeHumidityRef2) - CalcRoW(daysHours[k].Temp) * CalcAirX(daysHours[k].Temp, daysHours[k].Humidity)) * 0.6947222222222222;
		}
		double num3 = (num + num2) * (double)month.WorkDays;
		num3 = ((double.IsNaN(num3) || double.IsInfinity(num3)) ? 0.0 : num3);
		double num4 = 0.0;
		double num5 = 0.0;
		for (int l = 0; l < section.CoolingSeasons.Ventilation.SatBaseStart; l++)
		{
			num5 += ventCool.DebitRef2 * (CalcRoW(ventCool.FlowTemperatureRef2) * CalcAirX(ventCool.FlowTemperatureRef2, ventCool.RelativeHumidityRef2) - CalcRoW(daysHours[l].Temp) * CalcAirX(daysHours[l].Temp, daysHours[l].Humidity)) * 0.6947222222222222;
		}
		for (int m = section.CoolingSeasons.Ventilation.SatBaseStart; m < section.CoolingSeasons.Ventilation.SatBaseEnd; m++)
		{
			num4 += ventCool.DebitRef2 * (CalcRoW(ventCool.FlowTemperatureRef2) * CalcAirX(ventCool.FlowTemperatureRef2, ventCool.RelativeHumidityRef2) - CalcRoW(daysHours[m].Temp) * CalcAirX(daysHours[m].Temp, daysHours[m].Humidity)) * 0.6947222222222222;
		}
		for (int n = section.CoolingSeasons.Ventilation.SatBaseEnd; n < 24; n++)
		{
			num5 += ventCool.DebitRef2 * (ventCool.DebitRef2 * (CalcRoW(ventCool.FlowTemperatureRef2) * CalcAirX(ventCool.FlowTemperatureRef2, ventCool.RelativeHumidityRef2) - CalcRoW(daysHours[n].Temp) * CalcAirX(daysHours[n].Temp, daysHours[n].Humidity))) * 0.6947222222222222;
		}
		double num6 = (num4 + num5) * (double)month.Saturdays;
		num6 = ((double.IsNaN(num6) || double.IsInfinity(num6)) ? 0.0 : num6);
		double num7 = 0.0;
		double num8 = 0.0;
		for (int num9 = 0; num9 < section.CoolingSeasons.Ventilation.SunBaseStart; num9++)
		{
			num8 += ventCool.DebitRef2 * (CalcRoW(ventCool.FlowTemperatureRef2) * CalcAirX(ventCool.FlowTemperatureRef2, ventCool.RelativeHumidityRef2) - CalcRoW(daysHours[num9].Temp) * CalcAirX(daysHours[num9].Temp, daysHours[num9].Humidity)) * 0.6947222222222222;
		}
		for (int num10 = section.CoolingSeasons.Ventilation.SunBaseStart; num10 < section.CoolingSeasons.Ventilation.SunBaseEnd; num10++)
		{
			num7 += ventCool.DebitRef2 * (CalcRoW(ventCool.FlowTemperatureRef2) * CalcAirX(ventCool.FlowTemperatureRef2, ventCool.RelativeHumidityRef2) - CalcRoW(daysHours[num10].Temp) * CalcAirX(daysHours[num10].Temp, daysHours[num10].Humidity)) * 0.6947222222222222;
		}
		for (int num11 = section.CoolingSeasons.Ventilation.SunBaseEnd; num11 < 24; num11++)
		{
			num8 += ventCool.DebitRef2 * (CalcRoW(ventCool.FlowTemperatureRef2) * CalcAirX(ventCool.FlowTemperatureRef2, ventCool.RelativeHumidityRef2) - CalcRoW(daysHours[num11].Temp) * CalcAirX(daysHours[num11].Temp, daysHours[num11].Humidity)) * 0.6947222222222222;
		}
		double num12 = (num7 + num8) * (double)month.Sundays;
		num12 = ((double.IsNaN(num12) || double.IsInfinity(num12)) ? 0.0 : num12);
		double num13 = 0.0;
		for (int num14 = 0; num14 < 24; num14++)
		{
			num13 += ventCool.DebitRef2 * (CalcRoW(ventCool.FlowTemperatureRef2) * CalcAirX(ventCool.FlowTemperatureRef2, ventCool.RelativeHumidityRef2) - CalcRoW(calcData.NonProjectTemperatureRef2) * CalcAirX(calcData.NonProjectTemperatureRef2, calcData.ProjectHumidityRef2)) * 0.6947222222222222;
		}
		double num15 = num13 * (double)month.Holydays;
		num15 = ((double.IsNaN(num15) || double.IsInfinity(num15)) ? 0.0 : num15);
		return num3 + num6 + num12 + num15;
	}

	private static double CalculateLatentHeatsVent(Section section, CalculationData calcData, MonthlyDays month, ClimateZones climateZone, CalculationData ventCool)
	{
		List<TempHumidityPerDay> daysHours = GetDaysHours(climateZone, (int)month.Month);
		double num = 0.0;
		double num2 = 0.0;
		for (int i = 0; i < section.CoolingSeasons.Ventilation.WorkCurrentStart; i++)
		{
			num2 += ventCool.DebitActual * (CalcRoW(ventCool.FlowTemperatureActual) * CalcAirX(ventCool.FlowTemperatureActual, ventCool.RelativeHumidityActual) - CalcRoW(daysHours[i].Temp) * CalcAirX(daysHours[i].Temp, daysHours[i].Humidity)) * 0.6947222222222222;
		}
		for (int j = section.CoolingSeasons.Ventilation.WorkCurrentStart; j < section.CoolingSeasons.Ventilation.WorkCurrentEnd; j++)
		{
			num += ventCool.DebitActual * (CalcRoW(ventCool.FlowTemperatureActual) * CalcAirX(ventCool.FlowTemperatureActual, ventCool.RelativeHumidityActual) - CalcRoW(daysHours[j].Temp) * CalcAirX(daysHours[j].Temp, daysHours[j].Humidity)) * 0.6947222222222222;
		}
		for (int k = section.CoolingSeasons.Ventilation.WorkCurrentEnd; k < 24; k++)
		{
			num2 += ventCool.DebitActual * (CalcRoW(ventCool.FlowTemperatureActual) * CalcAirX(ventCool.FlowTemperatureActual, ventCool.RelativeHumidityActual) - CalcRoW(daysHours[k].Temp) * CalcAirX(daysHours[k].Temp, daysHours[k].Humidity)) * 0.6947222222222222;
		}
		double num3 = (num + num2) * (double)month.WorkDays;
		num3 = ((double.IsNaN(num3) || double.IsInfinity(num3)) ? 0.0 : num3);
		double num4 = 0.0;
		double num5 = 0.0;
		for (int l = 0; l < section.CoolingSeasons.Ventilation.SatCurrentStart; l++)
		{
			num5 += ventCool.DebitActual * (CalcRoW(ventCool.FlowTemperatureActual) * CalcAirX(ventCool.FlowTemperatureActual, ventCool.RelativeHumidityActual) - CalcRoW(daysHours[l].Temp) * CalcAirX(daysHours[l].Temp, daysHours[l].Humidity)) * 0.6947222222222222;
		}
		for (int m = section.CoolingSeasons.Ventilation.SatCurrentStart; m < section.CoolingSeasons.Ventilation.SatCurrentEnd; m++)
		{
			num4 += ventCool.DebitActual * (CalcRoW(ventCool.FlowTemperatureActual) * CalcAirX(ventCool.FlowTemperatureActual, ventCool.RelativeHumidityActual) - CalcRoW(daysHours[m].Temp) * CalcAirX(daysHours[m].Temp, daysHours[m].Humidity)) * 0.6947222222222222;
		}
		for (int n = section.CoolingSeasons.Ventilation.SatCurrentEnd; n < 24; n++)
		{
			num5 += ventCool.DebitActual * (ventCool.DebitActual * (CalcRoW(ventCool.FlowTemperatureActual) * CalcAirX(ventCool.FlowTemperatureActual, ventCool.RelativeHumidityActual) - CalcRoW(daysHours[n].Temp) * CalcAirX(daysHours[n].Temp, daysHours[n].Humidity))) * 0.6947222222222222;
		}
		double num6 = (num4 + num5) * (double)month.Saturdays;
		num6 = ((double.IsNaN(num6) || double.IsInfinity(num6)) ? 0.0 : num6);
		double num7 = 0.0;
		double num8 = 0.0;
		for (int num9 = 0; num9 < section.CoolingSeasons.Ventilation.SunCurrentStart; num9++)
		{
			num8 += ventCool.DebitActual * (CalcRoW(ventCool.FlowTemperatureActual) * CalcAirX(ventCool.FlowTemperatureActual, ventCool.RelativeHumidityActual) - CalcRoW(daysHours[num9].Temp) * CalcAirX(daysHours[num9].Temp, daysHours[num9].Humidity)) * 0.6947222222222222;
		}
		for (int num10 = section.CoolingSeasons.Ventilation.SunCurrentStart; num10 < section.CoolingSeasons.Ventilation.SunCurrentEnd; num10++)
		{
			num7 += ventCool.DebitActual * (CalcRoW(ventCool.FlowTemperatureActual) * CalcAirX(ventCool.FlowTemperatureActual, ventCool.RelativeHumidityActual) - CalcRoW(daysHours[num10].Temp) * CalcAirX(daysHours[num10].Temp, daysHours[num10].Humidity)) * 0.6947222222222222;
		}
		for (int num11 = section.CoolingSeasons.Ventilation.SunCurrentEnd; num11 < 24; num11++)
		{
			num8 += ventCool.DebitActual * (CalcRoW(ventCool.FlowTemperatureActual) * CalcAirX(ventCool.FlowTemperatureActual, ventCool.RelativeHumidityActual) - CalcRoW(daysHours[num11].Temp) * CalcAirX(daysHours[num11].Temp, daysHours[num11].Humidity)) * 0.6947222222222222;
		}
		double num12 = (num7 + num8) * (double)month.Sundays;
		num12 = ((double.IsNaN(num12) || double.IsInfinity(num12)) ? 0.0 : num12);
		double num13 = 0.0;
		for (int num14 = 0; num14 < 24; num14++)
		{
			num13 += ventCool.DebitActual * (CalcRoW(ventCool.FlowTemperatureActual) * CalcAirX(ventCool.FlowTemperatureActual, ventCool.RelativeHumidityActual) - CalcRoW(calcData.NonProjectTemperatureActual) * CalcAirX(calcData.NonProjectTemperatureActual, calcData.ProjectHumidityActual)) * 0.6947222222222222;
		}
		double num15 = num13 * (double)month.Holydays;
		num15 = ((double.IsNaN(num15) || double.IsInfinity(num15)) ? 0.0 : num15);
		return num3 + num6 + num12 + num15;
	}

	private static double CalculateLatentHeatsVentBaseLine(Section section, CalculationData calcData, MonthlyDays month, ClimateZones climateZone, CalculationData ventCool)
	{
		List<TempHumidityPerDay> daysHours = GetDaysHours(climateZone, (int)month.Month);
		double num = 0.0;
		double num2 = 0.0;
		for (int i = 0; i < section.CoolingSeasons.Ventilation.WorkBaseStart; i++)
		{
			num2 += ventCool.DebitBaseLine * (CalcRoW(ventCool.FlowTemperatureBaseLine) * CalcAirX(ventCool.FlowTemperatureBaseLine, ventCool.RelativeHumidityBaseLine) - CalcRoW(daysHours[i].Temp) * CalcAirX(daysHours[i].Temp, daysHours[i].Humidity)) * 0.6947222222222222;
		}
		for (int j = section.CoolingSeasons.Ventilation.WorkBaseStart; j < section.CoolingSeasons.Ventilation.WorkBaseEnd; j++)
		{
			num += ventCool.DebitBaseLine * (CalcRoW(ventCool.FlowTemperatureBaseLine) * CalcAirX(ventCool.FlowTemperatureBaseLine, ventCool.RelativeHumidityBaseLine) - CalcRoW(daysHours[j].Temp) * CalcAirX(daysHours[j].Temp, daysHours[j].Humidity)) * 0.6947222222222222;
		}
		for (int k = section.CoolingSeasons.Ventilation.WorkBaseEnd; k < 24; k++)
		{
			num2 += ventCool.DebitBaseLine * (CalcRoW(ventCool.FlowTemperatureBaseLine) * CalcAirX(ventCool.FlowTemperatureBaseLine, ventCool.RelativeHumidityBaseLine) - CalcRoW(daysHours[k].Temp) * CalcAirX(daysHours[k].Temp, daysHours[k].Humidity)) * 0.6947222222222222;
		}
		double num3 = (num + num2) * (double)month.WorkDays;
		num3 = ((double.IsNaN(num3) || double.IsInfinity(num3)) ? 0.0 : num3);
		double num4 = 0.0;
		double num5 = 0.0;
		for (int l = 0; l < section.CoolingSeasons.Ventilation.SatBaseStart; l++)
		{
			num5 += ventCool.DebitBaseLine * (CalcRoW(ventCool.FlowTemperatureBaseLine) * CalcAirX(ventCool.FlowTemperatureBaseLine, ventCool.RelativeHumidityBaseLine) - CalcRoW(daysHours[l].Temp) * CalcAirX(daysHours[l].Temp, daysHours[l].Humidity)) * 0.6947222222222222;
		}
		for (int m = section.CoolingSeasons.Ventilation.SatBaseStart; m < section.CoolingSeasons.Ventilation.SatBaseEnd; m++)
		{
			num4 += ventCool.DebitBaseLine * (CalcRoW(ventCool.FlowTemperatureBaseLine) * CalcAirX(ventCool.FlowTemperatureBaseLine, ventCool.RelativeHumidityBaseLine) - CalcRoW(daysHours[m].Temp) * CalcAirX(daysHours[m].Temp, daysHours[m].Humidity)) * 0.6947222222222222;
		}
		for (int n = section.CoolingSeasons.Ventilation.SatBaseEnd; n < 24; n++)
		{
			num5 += ventCool.DebitBaseLine * (ventCool.DebitBaseLine * (CalcRoW(ventCool.FlowTemperatureBaseLine) * CalcAirX(ventCool.FlowTemperatureBaseLine, ventCool.RelativeHumidityBaseLine) - CalcRoW(daysHours[n].Temp) * CalcAirX(daysHours[n].Temp, daysHours[n].Humidity))) * 0.6947222222222222;
		}
		double num6 = (num4 + num5) * (double)month.Saturdays;
		num6 = ((double.IsNaN(num6) || double.IsInfinity(num6)) ? 0.0 : num6);
		double num7 = 0.0;
		double num8 = 0.0;
		for (int num9 = 0; num9 < section.CoolingSeasons.Ventilation.SunBaseStart; num9++)
		{
			num8 += ventCool.DebitBaseLine * (CalcRoW(ventCool.FlowTemperatureBaseLine) * CalcAirX(ventCool.FlowTemperatureBaseLine, ventCool.RelativeHumidityBaseLine) - CalcRoW(daysHours[num9].Temp) * CalcAirX(daysHours[num9].Temp, daysHours[num9].Humidity)) * 0.6947222222222222;
		}
		for (int num10 = section.CoolingSeasons.Ventilation.SunBaseStart; num10 < section.CoolingSeasons.Ventilation.SunBaseEnd; num10++)
		{
			num7 += ventCool.DebitBaseLine * (CalcRoW(ventCool.FlowTemperatureBaseLine) * CalcAirX(ventCool.FlowTemperatureBaseLine, ventCool.RelativeHumidityBaseLine) - CalcRoW(daysHours[num10].Temp) * CalcAirX(daysHours[num10].Temp, daysHours[num10].Humidity)) * 0.6947222222222222;
		}
		for (int num11 = section.CoolingSeasons.Ventilation.SunBaseEnd; num11 < 24; num11++)
		{
			num8 += ventCool.DebitBaseLine * (CalcRoW(ventCool.FlowTemperatureBaseLine) * CalcAirX(ventCool.FlowTemperatureBaseLine, ventCool.RelativeHumidityBaseLine) - CalcRoW(daysHours[num11].Temp) * CalcAirX(daysHours[num11].Temp, daysHours[num11].Humidity)) * 0.6947222222222222;
		}
		double num12 = (num7 + num8) * (double)month.Sundays;
		num12 = ((double.IsNaN(num12) || double.IsInfinity(num12)) ? 0.0 : num12);
		double num13 = 0.0;
		for (int num14 = 0; num14 < 24; num14++)
		{
			num13 += ventCool.DebitBaseLine * (CalcRoW(ventCool.FlowTemperatureBaseLine) * CalcAirX(ventCool.FlowTemperatureBaseLine, ventCool.RelativeHumidityBaseLine) - CalcRoW(calcData.NonProjectTemperatureBaseLine) * CalcAirX(calcData.NonProjectTemperatureBaseLine, calcData.ProjectHumidityBaseLine)) * 0.6947222222222222;
		}
		double num15 = num13 * (double)month.Holydays;
		num15 = ((double.IsNaN(num15) || double.IsInfinity(num15)) ? 0.0 : num15);
		return num3 + num6 + num12 + num15;
	}

	private static double CalculateLatentHeatsVentESM(Section section, CalculationData calcData, MonthlyDays month, ClimateZones climateZone, CalculationData ventCool)
	{
		List<TempHumidityPerDay> daysHours = GetDaysHours(climateZone, (int)month.Month);
		double num = 0.0;
		double num2 = 0.0;
		for (int i = 0; i < section.CoolingSeasons.Ventilation.WorkEsmStart; i++)
		{
			num2 += ventCool.DebitESM * (CalcRoW(ventCool.FlowTemperatureESM) * CalcAirX(ventCool.FlowTemperatureESM, ventCool.RelativeHumidityESM) - CalcRoW(daysHours[i].Temp) * CalcAirX(daysHours[i].Temp, daysHours[i].Humidity)) * 0.6947222222222222;
		}
		for (int j = section.CoolingSeasons.Ventilation.WorkEsmStart; j < section.CoolingSeasons.Ventilation.WorkEsmEnd; j++)
		{
			num += ventCool.DebitESM * (CalcRoW(ventCool.FlowTemperatureESM) * CalcAirX(ventCool.FlowTemperatureESM, ventCool.RelativeHumidityESM) - CalcRoW(daysHours[j].Temp) * CalcAirX(daysHours[j].Temp, daysHours[j].Humidity)) * 0.6947222222222222;
		}
		for (int k = section.CoolingSeasons.Ventilation.WorkEsmEnd; k < 24; k++)
		{
			num2 += ventCool.DebitESM * (CalcRoW(ventCool.FlowTemperatureESM) * CalcAirX(ventCool.FlowTemperatureESM, ventCool.RelativeHumidityESM) - CalcRoW(daysHours[k].Temp) * CalcAirX(daysHours[k].Temp, daysHours[k].Humidity)) * 0.6947222222222222;
		}
		double num3 = (num + num2) * (double)month.WorkDays;
		num3 = ((double.IsNaN(num3) || double.IsInfinity(num3)) ? 0.0 : num3);
		double num4 = 0.0;
		double num5 = 0.0;
		for (int l = 0; l < section.CoolingSeasons.Ventilation.SatEsmStart; l++)
		{
			num5 += ventCool.DebitESM * (CalcRoW(ventCool.FlowTemperatureESM) * CalcAirX(ventCool.FlowTemperatureESM, ventCool.RelativeHumidityESM) - CalcRoW(daysHours[l].Temp) * CalcAirX(daysHours[l].Temp, daysHours[l].Humidity)) * 0.6947222222222222;
		}
		for (int m = section.CoolingSeasons.Ventilation.SatEsmStart; m < section.CoolingSeasons.Ventilation.SatEsmEnd; m++)
		{
			num4 += ventCool.DebitESM * (CalcRoW(ventCool.FlowTemperatureESM) * CalcAirX(ventCool.FlowTemperatureESM, ventCool.RelativeHumidityESM) - CalcRoW(daysHours[m].Temp) * CalcAirX(daysHours[m].Temp, daysHours[m].Humidity)) * 0.6947222222222222;
		}
		for (int n = section.CoolingSeasons.Ventilation.SatEsmEnd; n < 24; n++)
		{
			num5 += ventCool.DebitESM * (ventCool.DebitESM * (CalcRoW(ventCool.FlowTemperatureESM) * CalcAirX(ventCool.FlowTemperatureESM, ventCool.RelativeHumidityESM) - CalcRoW(daysHours[n].Temp) * CalcAirX(daysHours[n].Temp, daysHours[n].Humidity))) * 0.6947222222222222;
		}
		double num6 = (num4 + num5) * (double)month.Saturdays;
		num6 = ((double.IsNaN(num6) || double.IsInfinity(num6)) ? 0.0 : num6);
		double num7 = 0.0;
		double num8 = 0.0;
		for (int num9 = 0; num9 < section.CoolingSeasons.Ventilation.SunEsmStart; num9++)
		{
			num8 += ventCool.DebitESM * (CalcRoW(ventCool.FlowTemperatureESM) * CalcAirX(ventCool.FlowTemperatureESM, ventCool.RelativeHumidityESM) - CalcRoW(daysHours[num9].Temp) * CalcAirX(daysHours[num9].Temp, daysHours[num9].Humidity)) * 0.6947222222222222;
		}
		for (int num10 = section.CoolingSeasons.Ventilation.SunEsmStart; num10 < section.CoolingSeasons.Ventilation.SunEsmEnd; num10++)
		{
			num7 += ventCool.DebitESM * (CalcRoW(ventCool.FlowTemperatureESM) * CalcAirX(ventCool.FlowTemperatureESM, ventCool.RelativeHumidityESM) - CalcRoW(daysHours[num10].Temp) * CalcAirX(daysHours[num10].Temp, daysHours[num10].Humidity)) * 0.6947222222222222;
		}
		for (int num11 = section.CoolingSeasons.Ventilation.SunEsmEnd; num11 < 24; num11++)
		{
			num8 += ventCool.DebitESM * (CalcRoW(ventCool.FlowTemperatureESM) * CalcAirX(ventCool.FlowTemperatureESM, ventCool.RelativeHumidityESM) - CalcRoW(daysHours[num11].Temp) * CalcAirX(daysHours[num11].Temp, daysHours[num11].Humidity)) * 0.6947222222222222;
		}
		double num12 = (num7 + num8) * (double)month.Sundays;
		num12 = ((double.IsNaN(num12) || double.IsInfinity(num12)) ? 0.0 : num12);
		double num13 = 0.0;
		for (int num14 = 0; num14 < 24; num14++)
		{
			num13 += ventCool.DebitESM * (CalcRoW(ventCool.FlowTemperatureESM) * CalcAirX(ventCool.FlowTemperatureESM, ventCool.RelativeHumidityESM) - CalcRoW(calcData.NonProjectTemperatureESM) * CalcAirX(calcData.NonProjectTemperatureESM, calcData.ProjectHumidityESM)) * 0.6947222222222222;
		}
		double num15 = num13 * (double)month.Holydays;
		num15 = ((double.IsNaN(num15) || double.IsInfinity(num15)) ? 0.0 : num15);
		return num3 + num6 + num12 + num15;
	}

	private static double CalcAirX(double temp, double humidity)
	{
		double num = 273.15 + temp;
		double num2 = Math.Pow(2.718281828459, 77.345 + 0.0057 * num - 7235.0 / num) / Math.Pow(num, 8.2);
		double num3 = humidity * num2 / 100.0;
		return 0.62198 * (num3 / (101325.0 - num3));
	}

	private static double CalcRoW(double temp)
	{
		double num = temp + 273.15;
		return 101325.0 / (286.9 * num);
	}

	private static double CalcRo(double temp, double humidity)
	{
		double num = CalcAirX(temp, humidity);
		return CalcRoW(temp) * (1.0 + num) / (1.0 + 1.609 * num);
	}

	private static double CalculateETA(double parameterAc, double loses, double gainings, Section section)
	{
		double num = gainings / loses;
		section.Test.ParameterGamma = num;
		if (num > 0.0 && Math.Abs(num - 1.0) > 0.01)
		{
			return (1.0 - Math.Pow(num, 0.0 - parameterAc)) / (1.0 - Math.Pow(num, 0.0 - (parameterAc + 1.0)));
		}
		if (Math.Abs(num - 1.0) < 0.01)
		{
			return parameterAc / (parameterAc + 1.0);
		}
		if (num < 0.0)
		{
			return 1.0;
		}
		return 0.0;
	}

	private static double CalculateAcRef1(CalculationData calculationdata, Section section, ClimateZones climateZone, MonthlyDays month)
	{
		double avgTemp = PreferencesManager.GetClimateZoneParams(climateZone).SolarRadiation.Months[(int)month.Month].AvgTemp;
		double averageInnerCoolTemp = CalculateAverageCoolingTempRef1(section, calculationdata, month);
		double num = CalculateCoolingHtr(section, avgTemp, averageInnerCoolTemp);
		double num2 = CalculateHinfRef1(section, calculationdata);
		double num3 = section.Area.HeatedArea * section.Area.HeatCapacity / (num + num2);
		return 1.0 + num3 / 15.0;
	}

	private static double CalculateAcRef2(CalculationData calculationdata, Section section, ClimateZones climateZone, MonthlyDays month)
	{
		double avgTemp = PreferencesManager.GetClimateZoneParams(climateZone).SolarRadiation.Months[(int)month.Month].AvgTemp;
		double averageInnerCoolTemp = CalculateAverageCoolingTempRef2(section, calculationdata, month);
		double num = CalculateCoolingHtr(section, avgTemp, averageInnerCoolTemp);
		double num2 = CalculateHinfRef2(section, calculationdata);
		double num3 = section.Area.HeatedArea * section.Area.HeatCapacity / (num + num2);
		return 1.0 + num3 / 15.0;
	}

	private static double CalculateAc(CalculationData calculationdata, Section section, ClimateZones climateZone, MonthlyDays month)
	{
		double avgTemp = PreferencesManager.GetClimateZoneParams(climateZone).SolarRadiation.Months[(int)month.Month].AvgTemp;
		double averageInnerCoolTemp = CalculateAverageCoolingTempCurrent(section, calculationdata, month);
		double num = CalculateCoolingHtr(section, avgTemp, averageInnerCoolTemp);
		double num2 = CalculateHinf(section, calculationdata);
		double num3 = section.Area.HeatedArea * section.Area.HeatCapacity / (num + num2);
		return 1.0 + num3 / 15.0;
	}

	private static double CalculateAcBaseLine(CalculationData calculationdata, Section section, ClimateZones climateZone, MonthlyDays month)
	{
		double avgTemp = PreferencesManager.GetClimateZoneParams(climateZone).SolarRadiation.Months[(int)month.Month].AvgTemp;
		double averageInnerCoolTemp = CalculateAverageCoolingTempBaseLine(section, calculationdata, month);
		double num = CalculateCoolingHtr(section, avgTemp, averageInnerCoolTemp);
		double num2 = CalculateHinfBaseLine(section, calculationdata);
		double num3 = section.Area.HeatedArea * section.Area.HeatCapacity / (num + num2);
		return 1.0 + num3 / 15.0;
	}

	private static double CalculateAcESM(CalculationData calculationdata, Section section, ClimateZones climateZone, MonthlyDays month)
	{
		double avgTemp = PreferencesManager.GetClimateZoneParams(climateZone).SolarRadiation.Months[(int)month.Month].AvgTemp;
		double averageInnerCoolTemp = CalculateAverageCoolingTempESM(section, calculationdata, month);
		double num = CalculateCoolingHtrESM(section, avgTemp, averageInnerCoolTemp);
		double num2 = CalculateHinfESM(section, calculationdata);
		double num3 = section.Area.HeatedArea * section.Area.HeatCapacity / (num + num2);
		return 1.0 + num3 / 15.0;
	}

	private static IEnumerable<int> GetNightWorkingHours(int strat, int end)
	{
		List<int> list = new List<int>();
		if (strat == end)
		{
			return list;
		}
		if (strat > end)
		{
			for (int i = 0; i < end; i++)
			{
				list.Add(i);
			}
			for (int j = strat; j < 24; j++)
			{
				list.Add(j);
			}
		}
		else
		{
			for (int k = strat; k < end; k++)
			{
				list.Add(k);
			}
		}
		return list;
	}

	private static double ClaculateQfreecoolingRef1(Section section, CalculationData calcData, MonthlyDays month, ClimateZones climateZone)
	{
		ClimateZoneTempHumidityMonth climateZoneTempHumidityMonth = PreferencesManager.GetClimateZoneParams(climateZone).TempHumidity.Months[(int)month.Month];
		double num = calcData.DebitRef1 * 0.34;
		List<double> list = new List<double>();
		foreach (int nightWorkingHour in GetNightWorkingHours(section.CoolingSeasons.NightVentilation.WorkBaseStart, section.CoolingSeasons.NightVentilation.WorkBaseEnd))
		{
			int index = ((nightWorkingHour < climateZoneTempHumidityMonth.Hours.Count) ? nightWorkingHour : 0);
			double item = num * (calcData.ProjectTemperatureRef1 - climateZoneTempHumidityMonth.Hours[index].Temp) / 1000.0;
			list.Add(item);
		}
		double num2 = list.Aggregate(0.0, (double num6, double num7) => num6 + num7);
		List<double> list2 = new List<double>();
		foreach (int nightWorkingHour2 in GetNightWorkingHours(section.CoolingSeasons.NightVentilation.SatBaseStart, section.CoolingSeasons.NightVentilation.SatBaseEnd))
		{
			int index2 = ((nightWorkingHour2 < climateZoneTempHumidityMonth.Hours.Count) ? nightWorkingHour2 : 0);
			double item2 = num * (calcData.ProjectTemperatureRef1 - climateZoneTempHumidityMonth.Hours[index2].Temp) / 1000.0;
			list2.Add(item2);
		}
		double num3 = list2.Aggregate(0.0, (double num6, double num7) => num6 + num7);
		List<double> list3 = new List<double>();
		foreach (int nightWorkingHour3 in GetNightWorkingHours(section.CoolingSeasons.NightVentilation.SunBaseStart, section.CoolingSeasons.NightVentilation.SunBaseEnd))
		{
			int index3 = ((nightWorkingHour3 < climateZoneTempHumidityMonth.Hours.Count) ? nightWorkingHour3 : 0);
			double item3 = num * (calcData.ProjectTemperatureRef1 - climateZoneTempHumidityMonth.Hours[index3].Temp) / 1000.0;
			list3.Add(item3);
		}
		double num4 = list3.Aggregate(0.0, (double num6, double num7) => num6 + num7);
		List<double> list4 = new List<double>();
		foreach (int nightWorkingHour4 in GetNightWorkingHours(section.CoolingSeasons.NightVentilation.SunBaseStart, section.CoolingSeasons.NightVentilation.SunBaseEnd))
		{
			int index4 = ((nightWorkingHour4 < climateZoneTempHumidityMonth.Hours.Count) ? nightWorkingHour4 : 0);
			double item4 = num * (calcData.NonProjectTemperatureRef1 - climateZoneTempHumidityMonth.Hours[index4].Temp) / 1000.0;
			list4.Add(item4);
		}
		double num5 = list4.Aggregate(0.0, (double num6, double num7) => num6 + num7);
		return num2 * (double)month.WorkDays + num3 * (double)month.Saturdays + num4 * (double)month.Sundays + num5 * (double)month.Holydays;
	}

	private static double ClaculateQfreecoolingRef2(Section section, CalculationData calcData, MonthlyDays month, ClimateZones climateZone)
	{
		ClimateZoneTempHumidityMonth climateZoneTempHumidityMonth = PreferencesManager.GetClimateZoneParams(climateZone).TempHumidity.Months[(int)month.Month];
		double num = calcData.DebitRef2 * 0.34;
		List<double> list = new List<double>();
		foreach (int nightWorkingHour in GetNightWorkingHours(section.CoolingSeasons.NightVentilation.WorkBaseStart, section.CoolingSeasons.NightVentilation.WorkBaseEnd))
		{
			int index = ((nightWorkingHour < climateZoneTempHumidityMonth.Hours.Count) ? nightWorkingHour : 0);
			double item = num * (calcData.ProjectTemperatureRef2 - climateZoneTempHumidityMonth.Hours[index].Temp) / 1000.0;
			list.Add(item);
		}
		double num2 = list.Aggregate(0.0, (double num6, double num7) => num6 + num7);
		List<double> list2 = new List<double>();
		foreach (int nightWorkingHour2 in GetNightWorkingHours(section.CoolingSeasons.NightVentilation.SatBaseStart, section.CoolingSeasons.NightVentilation.SatBaseEnd))
		{
			int index2 = ((nightWorkingHour2 < climateZoneTempHumidityMonth.Hours.Count) ? nightWorkingHour2 : 0);
			double item2 = num * (calcData.ProjectTemperatureRef2 - climateZoneTempHumidityMonth.Hours[index2].Temp) / 1000.0;
			list2.Add(item2);
		}
		double num3 = list2.Aggregate(0.0, (double num6, double num7) => num6 + num7);
		List<double> list3 = new List<double>();
		foreach (int nightWorkingHour3 in GetNightWorkingHours(section.CoolingSeasons.NightVentilation.SunBaseStart, section.CoolingSeasons.NightVentilation.SunBaseEnd))
		{
			int index3 = ((nightWorkingHour3 < climateZoneTempHumidityMonth.Hours.Count) ? nightWorkingHour3 : 0);
			double item3 = num * (calcData.ProjectTemperatureRef2 - climateZoneTempHumidityMonth.Hours[index3].Temp) / 1000.0;
			list3.Add(item3);
		}
		double num4 = list3.Aggregate(0.0, (double num6, double num7) => num6 + num7);
		List<double> list4 = new List<double>();
		foreach (int nightWorkingHour4 in GetNightWorkingHours(section.CoolingSeasons.NightVentilation.SunBaseStart, section.CoolingSeasons.NightVentilation.SunBaseEnd))
		{
			int index4 = ((nightWorkingHour4 < climateZoneTempHumidityMonth.Hours.Count) ? nightWorkingHour4 : 0);
			double item4 = num * (calcData.NonProjectTemperatureRef2 - climateZoneTempHumidityMonth.Hours[index4].Temp) / 1000.0;
			list4.Add(item4);
		}
		double num5 = list4.Aggregate(0.0, (double num6, double num7) => num6 + num7);
		return num2 * (double)month.WorkDays + num3 * (double)month.Saturdays + num4 * (double)month.Sundays + num5 * (double)month.Holydays;
	}

	private static double ClaculateQfreecooling(Section section, CalculationData calcData, MonthlyDays month, ClimateZones climateZone)
	{
		ClimateZoneTempHumidityMonth climateZoneTempHumidityMonth = PreferencesManager.GetClimateZoneParams(climateZone).TempHumidity.Months[(int)month.Month];
		double num = calcData.DebitActual * 0.34;
		List<double> list = new List<double>();
		foreach (int nightWorkingHour in GetNightWorkingHours(section.CoolingSeasons.NightVentilation.WorkCurrentStart, section.CoolingSeasons.NightVentilation.WorkCurrentEnd))
		{
			int index = ((nightWorkingHour < climateZoneTempHumidityMonth.Hours.Count) ? nightWorkingHour : 0);
			double item = num * (calcData.ProjectTemperatureActual - climateZoneTempHumidityMonth.Hours[index].Temp) / 1000.0;
			list.Add(item);
		}
		double num2 = list.Aggregate(0.0, (double num6, double num7) => num6 + num7);
		List<double> list2 = new List<double>();
		foreach (int nightWorkingHour2 in GetNightWorkingHours(section.CoolingSeasons.NightVentilation.SatCurrentStart, section.CoolingSeasons.NightVentilation.SatCurrentEnd))
		{
			int index2 = ((nightWorkingHour2 < climateZoneTempHumidityMonth.Hours.Count) ? nightWorkingHour2 : 0);
			double item2 = num * (calcData.ProjectTemperatureActual - climateZoneTempHumidityMonth.Hours[index2].Temp) / 1000.0;
			list2.Add(item2);
		}
		double num3 = list2.Aggregate(0.0, (double num6, double num7) => num6 + num7);
		List<double> list3 = new List<double>();
		foreach (int nightWorkingHour3 in GetNightWorkingHours(section.CoolingSeasons.NightVentilation.SunCurrentStart, section.CoolingSeasons.NightVentilation.SunCurrentEnd))
		{
			int index3 = ((nightWorkingHour3 < climateZoneTempHumidityMonth.Hours.Count) ? nightWorkingHour3 : 0);
			double item3 = num * (calcData.ProjectTemperatureActual - climateZoneTempHumidityMonth.Hours[index3].Temp) / 1000.0;
			list3.Add(item3);
		}
		double num4 = list3.Aggregate(0.0, (double num6, double num7) => num6 + num7);
		List<double> list4 = new List<double>();
		foreach (int nightWorkingHour4 in GetNightWorkingHours(section.CoolingSeasons.NightVentilation.SunCurrentStart, section.CoolingSeasons.NightVentilation.SunCurrentEnd))
		{
			int index4 = ((nightWorkingHour4 < climateZoneTempHumidityMonth.Hours.Count) ? nightWorkingHour4 : 0);
			double item4 = num * (calcData.NonProjectTemperatureActual - climateZoneTempHumidityMonth.Hours[index4].Temp) / 1000.0;
			list4.Add(item4);
		}
		double num5 = list4.Aggregate(0.0, (double num6, double num7) => num6 + num7);
		return num2 * (double)month.WorkDays + num3 * (double)month.Saturdays + num4 * (double)month.Sundays + num5 * (double)month.Holydays;
	}

	private static double ClaculateQfreecoolingBaseLine(Section section, CalculationData calcData, MonthlyDays month, ClimateZones climateZone)
	{
		ClimateZoneTempHumidityMonth climateZoneTempHumidityMonth = PreferencesManager.GetClimateZoneParams(climateZone).TempHumidity.Months[(int)month.Month];
		double num = calcData.DebitBaseLine * 0.34;
		List<double> list = new List<double>();
		foreach (int nightWorkingHour in GetNightWorkingHours(section.CoolingSeasons.NightVentilation.WorkBaseStart, section.CoolingSeasons.NightVentilation.WorkBaseEnd))
		{
			int index = ((nightWorkingHour < climateZoneTempHumidityMonth.Hours.Count) ? nightWorkingHour : 0);
			double item = num * (calcData.ProjectTemperatureBaseLine - climateZoneTempHumidityMonth.Hours[index].Temp) / 1000.0;
			list.Add(item);
		}
		double num2 = list.Aggregate(0.0, (double num6, double num7) => num6 + num7);
		List<double> list2 = new List<double>();
		foreach (int nightWorkingHour2 in GetNightWorkingHours(section.CoolingSeasons.NightVentilation.SatBaseStart, section.CoolingSeasons.NightVentilation.SatBaseEnd))
		{
			int index2 = ((nightWorkingHour2 < climateZoneTempHumidityMonth.Hours.Count) ? nightWorkingHour2 : 0);
			double item2 = num * (calcData.ProjectTemperatureBaseLine - climateZoneTempHumidityMonth.Hours[index2].Temp) / 1000.0;
			list2.Add(item2);
		}
		double num3 = list2.Aggregate(0.0, (double num6, double num7) => num6 + num7);
		List<double> list3 = new List<double>();
		foreach (int nightWorkingHour3 in GetNightWorkingHours(section.CoolingSeasons.NightVentilation.SunBaseStart, section.CoolingSeasons.NightVentilation.SunBaseEnd))
		{
			int index3 = ((nightWorkingHour3 < climateZoneTempHumidityMonth.Hours.Count) ? nightWorkingHour3 : 0);
			double item3 = num * (calcData.ProjectTemperatureBaseLine - climateZoneTempHumidityMonth.Hours[index3].Temp) / 1000.0;
			list3.Add(item3);
		}
		double num4 = list3.Aggregate(0.0, (double num6, double num7) => num6 + num7);
		List<double> list4 = new List<double>();
		foreach (int nightWorkingHour4 in GetNightWorkingHours(section.CoolingSeasons.NightVentilation.SunBaseStart, section.CoolingSeasons.NightVentilation.SunBaseEnd))
		{
			int index4 = ((nightWorkingHour4 < climateZoneTempHumidityMonth.Hours.Count) ? nightWorkingHour4 : 0);
			double item4 = num * (calcData.NonProjectTemperatureBaseLine - climateZoneTempHumidityMonth.Hours[index4].Temp) / 1000.0;
			list4.Add(item4);
		}
		double num5 = list4.Aggregate(0.0, (double num6, double num7) => num6 + num7);
		return num2 * (double)month.WorkDays + num3 * (double)month.Saturdays + num4 * (double)month.Sundays + num5 * (double)month.Holydays;
	}

	private static double ClaculateQfreecoolingEsm(Section section, CalculationData calcData, MonthlyDays month, ClimateZones climateZone)
	{
		ClimateZoneTempHumidityMonth climateZoneTempHumidityMonth = PreferencesManager.GetClimateZoneParams(climateZone).TempHumidity.Months[(int)month.Month];
		double num = calcData.DebitESM * 0.34;
		List<double> list = new List<double>();
		foreach (int nightWorkingHour in GetNightWorkingHours(section.CoolingSeasons.NightVentilation.WorkEsmStart, section.CoolingSeasons.NightVentilation.WorkEsmEnd))
		{
			int index = ((nightWorkingHour < climateZoneTempHumidityMonth.Hours.Count) ? nightWorkingHour : 0);
			double item = num * (calcData.ProjectTemperatureESM - climateZoneTempHumidityMonth.Hours[index].Temp) / 1000.0;
			list.Add(item);
		}
		double num2 = list.Aggregate(0.0, (double num6, double num7) => num6 + num7);
		List<double> list2 = new List<double>();
		foreach (int nightWorkingHour2 in GetNightWorkingHours(section.CoolingSeasons.NightVentilation.SatEsmStart, section.CoolingSeasons.NightVentilation.SatEsmEnd))
		{
			int index2 = ((nightWorkingHour2 < climateZoneTempHumidityMonth.Hours.Count) ? nightWorkingHour2 : 0);
			double item2 = num * (calcData.ProjectTemperatureESM - climateZoneTempHumidityMonth.Hours[index2].Temp) / 1000.0;
			list2.Add(item2);
		}
		double num3 = list2.Aggregate(0.0, (double num6, double num7) => num6 + num7);
		List<double> list3 = new List<double>();
		foreach (int nightWorkingHour3 in GetNightWorkingHours(section.CoolingSeasons.NightVentilation.SunEsmStart, section.CoolingSeasons.NightVentilation.SunEsmEnd))
		{
			int index3 = ((nightWorkingHour3 < climateZoneTempHumidityMonth.Hours.Count) ? nightWorkingHour3 : 0);
			double item3 = num * (calcData.ProjectTemperatureESM - climateZoneTempHumidityMonth.Hours[index3].Temp) / 1000.0;
			list3.Add(item3);
		}
		double num4 = list3.Aggregate(0.0, (double num6, double num7) => num6 + num7);
		List<double> list4 = new List<double>();
		foreach (int nightWorkingHour4 in GetNightWorkingHours(section.CoolingSeasons.NightVentilation.SunEsmStart, section.CoolingSeasons.NightVentilation.SunEsmEnd))
		{
			int index4 = ((nightWorkingHour4 < climateZoneTempHumidityMonth.Hours.Count) ? nightWorkingHour4 : 0);
			double item4 = num * (calcData.NonProjectTemperatureESM - climateZoneTempHumidityMonth.Hours[index4].Temp) / 1000.0;
			list4.Add(item4);
		}
		double num5 = list4.Aggregate(0.0, (double num6, double num7) => num6 + num7);
		return num2 * (double)month.WorkDays + num3 * (double)month.Saturdays + num4 * (double)month.Sundays + num5 * (double)month.Holydays;
	}

	private static double CalculateQgainRef1(Section section, ClimateZones climateZone, MonthlyDays month, CalculationData lightsAndDevicesCalculationData)
	{
		double num = CalculateQsolRef1(section, climateZone, month);
		double num2 = CalculateQintRef1(lightsAndDevicesCalculationData, month, section.Area.HeatedArea);
		double num3 = CalculateQoccupantsBaseLine(section, month);
		return num + num2 + num3;
	}

	private static double CalculateQgainRef2(Section section, ClimateZones climateZone, MonthlyDays month, CalculationData lightsAndDevicesCalculationData)
	{
		double num = CalculateQsolRef2(section, climateZone, month);
		double num2 = CalculateQintRef2(lightsAndDevicesCalculationData, month, section.Area.HeatedArea);
		double num3 = CalculateQoccupantsBaseLine(section, month);
		return num + num2 + num3;
	}

	private static double CalculateQgain(Section section, ClimateZones climateZone, MonthlyDays month, CalculationData lightsAndDevicesCalculationData)
	{
		double num = CalculateQsol(section, climateZone, month);
		double num2 = CalculateQint(lightsAndDevicesCalculationData, month, section.Area.HeatedArea);
		double num3 = CalculateQoccupants(section, month);
		return num + num2 + num3;
	}

	private static double CalculateQgainBaseLine(Section section, ClimateZones climateZone, MonthlyDays month, CalculationData lightsAndDevicesCalculationData)
	{
		double num = CalculateQsolBaseLine(section, climateZone, month);
		double num2 = CalculateQintBaseLine(lightsAndDevicesCalculationData, month, section.Area.HeatedArea);
		double num3 = CalculateQoccupantsBaseLine(section, month);
		return num + num2 + num3;
	}

	private static double CalculateQgainESM(Section section, ClimateZones climateZone, MonthlyDays month, CalculationData lightsAndDevicesCalculationData)
	{
		double num = CalculateQsolESM(section, climateZone, month);
		double num2 = CalculateQintESM(lightsAndDevicesCalculationData, month, section.Area.HeatedArea);
		double num3 = CalculateQoccupantsESM(section, month);
		return num + num2 + num3;
	}

	private static double CalculateQintRef1(CalculationData lightsAndDevicesCalculationData, MonthlyDays month, double area)
	{
		double num = lightsAndDevicesCalculationData.Lights.Cooling.PowerRef1 * (lightsAndDevicesCalculationData.Lights.Cooling.WorkScheduleRef1 * month.Weeks) / 1000.0;
		double num2 = lightsAndDevicesCalculationData.BalancedDevices.Cooling.PowerRef1 * (lightsAndDevicesCalculationData.BalancedDevices.Cooling.WorkScheduleRef1 * month.Weeks) / 1000.0;
		return num * area + num2 * area;
	}

	private static double CalculateQintRef2(CalculationData lightsAndDevicesCalculationData, MonthlyDays month, double area)
	{
		double num = lightsAndDevicesCalculationData.Lights.Cooling.PowerRef2 * (lightsAndDevicesCalculationData.Lights.Cooling.WorkScheduleRef2 * month.Weeks) / 1000.0;
		double num2 = lightsAndDevicesCalculationData.BalancedDevices.Cooling.PowerRef2 * (lightsAndDevicesCalculationData.BalancedDevices.Cooling.WorkScheduleRef2 * month.Weeks) / 1000.0;
		return num * area + num2 * area;
	}

	private static double CalculateQint(CalculationData lightsAndDevicesCalculationData, MonthlyDays month, double area)
	{
		double num = (lightsAndDevicesCalculationData.Lights.ByMonths ? (CalcAvgMonthPower(lightsAndDevicesCalculationData.Lights.Actual, month) * (weekRegime * month.Weeks) / 1000.0) : (lightsAndDevicesCalculationData.Lights.Cooling.PowerActual * (lightsAndDevicesCalculationData.Lights.Cooling.WorkScheduleActual * month.Weeks) / 1000.0));
		double num2 = (lightsAndDevicesCalculationData.BalancedDevices.ByMonths ? (CalcAvgMonthPower(lightsAndDevicesCalculationData.BalancedDevices.Actual, month) * (weekRegime * month.Weeks) / 1000.0) : (lightsAndDevicesCalculationData.BalancedDevices.Cooling.PowerActual * (lightsAndDevicesCalculationData.BalancedDevices.Cooling.WorkScheduleActual * month.Weeks) / 1000.0));
		return num * area + num2 * area;
	}

	private static double CalculateQintBaseLine(CalculationData lightsAndDevicesCalculationData, MonthlyDays month, double area)
	{
		double num = (lightsAndDevicesCalculationData.Lights.ByMonths ? (CalcAvgMonthPower(lightsAndDevicesCalculationData.Lights.BaseLine, month) * (weekRegime * month.Weeks) / 1000.0) : (lightsAndDevicesCalculationData.Lights.Cooling.PowerBaseLine * (lightsAndDevicesCalculationData.Lights.Cooling.WorkScheduleBaseLine * month.Weeks) / 1000.0));
		double num2 = (lightsAndDevicesCalculationData.BalancedDevices.ByMonths ? (CalcAvgMonthPower(lightsAndDevicesCalculationData.BalancedDevices.BaseLine, month) * (weekRegime * month.Weeks) / 1000.0) : (lightsAndDevicesCalculationData.BalancedDevices.Cooling.PowerBaseLine * (lightsAndDevicesCalculationData.BalancedDevices.Cooling.WorkScheduleBaseLine * month.Weeks) / 1000.0));
		return num * area + num2 * area;
	}

	private static double CalculateQintESM(CalculationData lightsAndDevicesCalculationData, MonthlyDays month, double area)
	{
		double num = (lightsAndDevicesCalculationData.Lights.ByMonths ? (CalcAvgMonthPower(lightsAndDevicesCalculationData.Lights.Esm, month) * (weekRegime * month.Weeks) / 1000.0) : (lightsAndDevicesCalculationData.Lights.Cooling.PowerESM * (lightsAndDevicesCalculationData.Lights.Cooling.WorkScheduleESM * month.Weeks) / 1000.0));
		double num2 = (lightsAndDevicesCalculationData.BalancedDevices.ByMonths ? (CalcAvgMonthPower(lightsAndDevicesCalculationData.BalancedDevices.Esm, month) * (weekRegime * month.Weeks) / 1000.0) : (lightsAndDevicesCalculationData.BalancedDevices.Cooling.PowerESM * (lightsAndDevicesCalculationData.BalancedDevices.Cooling.WorkScheduleESM * month.Weeks) / 1000.0));
		return num * area + num2 * area;
	}

	private static double CalculateQoccupants(Section section, MonthlyDays month)
	{
		double num = CalculateOccupantshours(section, month);
		double num2 = section.Area.MetabolicHeat * num / 1000.0;
		return num2 * section.Area.HeatedArea;
	}

	private static double CalculateQoccupantsBaseLine(Section section, MonthlyDays month)
	{
		double num = CalculateOccupantshoursBaseLine(section, month);
		double num2 = section.Area.MetabolicHeat * num / 1000.0;
		return num2 * section.Area.HeatedArea;
	}

	private static double CalculateQoccupantsESM(Section section, MonthlyDays month)
	{
		double num = CalculateOccupantshoursESM(section, month);
		double num2 = section.Area.MetabolicHeat * num / 1000.0;
		return num2 * section.Area.HeatedArea;
	}

	private static double CalculateQLatentOccupantsRef1(Section section, MonthlyDays month)
	{
		double num = CalculateOccupantshours(section, month);
		double num2 = section.Area.LatentMetabolicHeat * num / 1000.0;
		return num2 * section.Area.HeatedArea;
	}

	private static double CalculateQLatentOccupantsRef2(Section section, MonthlyDays month)
	{
		double num = CalculateOccupantshours(section, month);
		double num2 = section.Area.LatentMetabolicHeat * num / 1000.0;
		return num2 * section.Area.HeatedArea;
	}

	private static double CalculateQLatentOccupants(Section section, MonthlyDays month)
	{
		double num = CalculateOccupantshours(section, month);
		double num2 = section.Area.LatentMetabolicHeat * num / 1000.0;
		return num2 * section.Area.HeatedArea;
	}

	private static double CalculateQLatentOccupantsBaseLine(Section section, MonthlyDays month)
	{
		double num = CalculateOccupantshoursBaseLine(section, month);
		double num2 = section.Area.LatentMetabolicHeat * num / 1000.0;
		return num2 * section.Area.HeatedArea;
	}

	private static double CalculateQLatentOccupantsESM(Section section, MonthlyDays month)
	{
		double num = CalculateOccupantshoursESM(section, month);
		double num2 = section.Area.LatentMetabolicHeat * num / 1000.0;
		return num2 * section.Area.HeatedArea;
	}

	private static double CalculateOccupantshours(Section section, MonthlyDays month)
	{
		int num = month.WorkDays * (section.CoolingSeasons.Occupants.WorkCurrentEnd - section.CoolingSeasons.Occupants.WorkCurrentStart);
		num += month.Saturdays * (section.CoolingSeasons.Occupants.SatCurrentEnd - section.CoolingSeasons.Occupants.SatCurrentStart);
		num += month.Sundays * (section.CoolingSeasons.Occupants.SunCurrentEnd - section.CoolingSeasons.Occupants.SunCurrentStart);
		return num;
	}

	private static double CalculateOccupantshoursBaseLine(Section section, MonthlyDays month)
	{
		int num = month.WorkDays * (section.CoolingSeasons.Occupants.WorkBaseEnd - section.CoolingSeasons.Occupants.WorkBaseStart);
		num += month.Saturdays * (section.CoolingSeasons.Occupants.SatBaseEnd - section.CoolingSeasons.Occupants.SatBaseStart);
		num += month.Sundays * (section.CoolingSeasons.Occupants.SunBaseEnd - section.CoolingSeasons.Occupants.SunBaseStart);
		return num;
	}

	private static double CalculateOccupantshoursESM(Section section, MonthlyDays month)
	{
		int num = month.WorkDays * (section.CoolingSeasons.Occupants.WorkEsmEnd - section.CoolingSeasons.Occupants.WorkEsmStart);
		num += month.Saturdays * (section.CoolingSeasons.Occupants.SatEsmEnd - section.CoolingSeasons.Occupants.SatEsmStart);
		num += month.Sundays * (section.CoolingSeasons.Occupants.SunEsmEnd - section.CoolingSeasons.Occupants.SunEsmStart);
		return num;
	}

	private static double CalculateQveRef1(Section section, CalculationData calcData, CalculationData ventCool, MonthlyDays month)
	{
		double num = 0.0;
		double num2 = 0.0;
		for (int i = 0; i < section.CoolingSeasons.Ventilation.WorkBaseStart; i++)
		{
			double num3 = ((i < section.CoolingSeasons.Occupants.WorkCurrentStart) ? calcData.NonProjectTemperatureRef1 : calcData.ProjectTemperatureRef1);
			num += CalculateHveRef1(ventCool) * (num3 - ventCool.FlowTemperatureRef1) / 1000.0;
		}
		for (int j = section.CoolingSeasons.Ventilation.WorkBaseStart; j < section.CoolingSeasons.Ventilation.WorkBaseEnd; j++)
		{
			double num4 = ((j >= section.CoolingSeasons.Occupants.WorkCurrentStart && j < section.CoolingSeasons.Occupants.WorkCurrentEnd) ? calcData.ProjectTemperatureRef1 : calcData.NonProjectTemperatureRef1);
			num2 += CalculateHveRef1(ventCool) * (num4 - ventCool.FlowTemperatureRef1) / 1000.0;
		}
		for (int k = section.CoolingSeasons.Ventilation.WorkBaseEnd; k < 24; k++)
		{
			double num5 = ((k < section.CoolingSeasons.Occupants.WorkCurrentEnd) ? calcData.ProjectTemperatureRef1 : calcData.NonProjectTemperatureRef1);
			num += CalculateHveRef1(ventCool) * (num5 - ventCool.FlowTemperatureRef1) / 1000.0;
		}
		double num6 = (num2 + num) * (double)month.WorkDays;
		double num7 = 0.0;
		double num8 = 0.0;
		for (int l = 0; l < section.CoolingSeasons.Ventilation.SatBaseStart; l++)
		{
			double num9 = ((l < section.CoolingSeasons.Occupants.SatCurrentStart) ? calcData.NonProjectTemperatureRef1 : calcData.ProjectTemperatureRef1);
			num7 += CalculateHveRef1(ventCool) * (num9 - ventCool.FlowTemperatureRef1) / 1000.0;
		}
		for (int m = section.CoolingSeasons.Ventilation.SatBaseStart; m < section.CoolingSeasons.Ventilation.SatBaseEnd; m++)
		{
			double num10 = ((m >= section.CoolingSeasons.Occupants.SatCurrentStart && m < section.CoolingSeasons.Occupants.SatCurrentEnd) ? calcData.ProjectTemperatureRef1 : calcData.NonProjectTemperatureRef1);
			num8 += CalculateHveRef1(ventCool) * (num10 - ventCool.FlowTemperatureRef1) / 1000.0;
		}
		for (int n = section.CoolingSeasons.Ventilation.SatBaseEnd; n < 24; n++)
		{
			double num11 = ((n < section.CoolingSeasons.Occupants.SatCurrentEnd) ? calcData.ProjectTemperatureRef1 : calcData.NonProjectTemperatureRef1);
			num7 += CalculateHveRef1(ventCool) * (num11 - ventCool.FlowTemperatureRef1) / 1000.0;
		}
		double num12 = (num7 + num8) * (double)month.Saturdays;
		double num13 = 0.0;
		double num14 = 0.0;
		for (int num15 = 0; num15 < section.CoolingSeasons.Ventilation.SunBaseStart; num15++)
		{
			double num16 = ((num15 < section.CoolingSeasons.Occupants.SunCurrentStart) ? calcData.NonProjectTemperatureRef1 : calcData.ProjectTemperatureRef1);
			num13 += CalculateHveRef1(ventCool) * (num16 - ventCool.FlowTemperatureRef1) / 1000.0;
		}
		for (int num17 = section.CoolingSeasons.Ventilation.SunBaseStart; num17 < section.CoolingSeasons.Ventilation.SunBaseEnd; num17++)
		{
			double num18 = ((num17 >= section.CoolingSeasons.Occupants.SunCurrentStart && num17 < section.CoolingSeasons.Occupants.SunCurrentEnd) ? calcData.ProjectTemperatureRef1 : calcData.NonProjectTemperatureRef1);
			num14 += CalculateHveRef1(ventCool) * (num18 - ventCool.FlowTemperatureRef1) / 1000.0;
		}
		for (int num19 = section.CoolingSeasons.Ventilation.SunBaseEnd; num19 < 24; num19++)
		{
			double num20 = ((num19 < section.CoolingSeasons.Occupants.SunCurrentEnd) ? calcData.ProjectTemperatureRef1 : calcData.NonProjectTemperatureRef1);
			num13 += CalculateHveRef1(ventCool) * (num20 - ventCool.FlowTemperatureRef1) / 1000.0;
		}
		double num21 = (num13 + num14) * (double)month.Sundays;
		return num6 + num12 + num21;
	}

	private static double CalculateQveRef2(Section section, CalculationData calcData, CalculationData ventCool, MonthlyDays month)
	{
		double num = 0.0;
		double num2 = 0.0;
		for (int i = 0; i < section.CoolingSeasons.Ventilation.WorkBaseStart; i++)
		{
			double num3 = ((i < section.CoolingSeasons.Occupants.WorkCurrentStart) ? calcData.NonProjectTemperatureRef2 : calcData.ProjectTemperatureRef2);
			num += CalculateHveRef2(ventCool) * (num3 - ventCool.FlowTemperatureRef2) / 1000.0;
		}
		for (int j = section.CoolingSeasons.Ventilation.WorkBaseStart; j < section.CoolingSeasons.Ventilation.WorkBaseEnd; j++)
		{
			double num4 = ((j >= section.CoolingSeasons.Occupants.WorkCurrentStart && j < section.CoolingSeasons.Occupants.WorkCurrentEnd) ? calcData.ProjectTemperatureRef2 : calcData.NonProjectTemperatureRef2);
			num2 += CalculateHveRef2(ventCool) * (num4 - ventCool.FlowTemperatureRef2) / 1000.0;
		}
		for (int k = section.CoolingSeasons.Ventilation.WorkBaseEnd; k < 24; k++)
		{
			double num5 = ((k < section.CoolingSeasons.Occupants.WorkCurrentEnd) ? calcData.ProjectTemperatureRef2 : calcData.NonProjectTemperatureRef2);
			num += CalculateHveRef2(ventCool) * (num5 - ventCool.FlowTemperatureRef2) / 1000.0;
		}
		double num6 = (num2 + num) * (double)month.WorkDays;
		double num7 = 0.0;
		double num8 = 0.0;
		for (int l = 0; l < section.CoolingSeasons.Ventilation.SatBaseStart; l++)
		{
			double num9 = ((l < section.CoolingSeasons.Occupants.SatCurrentStart) ? calcData.NonProjectTemperatureRef2 : calcData.ProjectTemperatureRef2);
			num7 += CalculateHveRef2(ventCool) * (num9 - ventCool.FlowTemperatureRef2) / 1000.0;
		}
		for (int m = section.CoolingSeasons.Ventilation.SatBaseStart; m < section.CoolingSeasons.Ventilation.SatBaseEnd; m++)
		{
			double num10 = ((m >= section.CoolingSeasons.Occupants.SatCurrentStart && m < section.CoolingSeasons.Occupants.SatCurrentEnd) ? calcData.ProjectTemperatureRef2 : calcData.NonProjectTemperatureRef2);
			num8 += CalculateHveRef2(ventCool) * (num10 - ventCool.FlowTemperatureRef2) / 1000.0;
		}
		for (int n = section.CoolingSeasons.Ventilation.SatBaseEnd; n < 24; n++)
		{
			double num11 = ((n < section.CoolingSeasons.Occupants.SatCurrentEnd) ? calcData.ProjectTemperatureRef2 : calcData.NonProjectTemperatureRef2);
			num7 += CalculateHveRef2(ventCool) * (num11 - ventCool.FlowTemperatureRef2) / 1000.0;
		}
		double num12 = (num7 + num8) * (double)month.Saturdays;
		double num13 = 0.0;
		double num14 = 0.0;
		for (int num15 = 0; num15 < section.CoolingSeasons.Ventilation.SunBaseStart; num15++)
		{
			double num16 = ((num15 < section.CoolingSeasons.Occupants.SunCurrentStart) ? calcData.NonProjectTemperatureRef2 : calcData.ProjectTemperatureRef2);
			num13 += CalculateHveRef2(ventCool) * (num16 - ventCool.FlowTemperatureRef2) / 1000.0;
		}
		for (int num17 = section.CoolingSeasons.Ventilation.SunBaseStart; num17 < section.CoolingSeasons.Ventilation.SunBaseEnd; num17++)
		{
			double num18 = ((num17 >= section.CoolingSeasons.Occupants.SunCurrentStart && num17 < section.CoolingSeasons.Occupants.SunCurrentEnd) ? calcData.ProjectTemperatureRef2 : calcData.NonProjectTemperatureRef2);
			num14 += CalculateHveRef2(ventCool) * (num18 - ventCool.FlowTemperatureRef2) / 1000.0;
		}
		for (int num19 = section.CoolingSeasons.Ventilation.SunBaseEnd; num19 < 24; num19++)
		{
			double num20 = ((num19 < section.CoolingSeasons.Occupants.SunCurrentEnd) ? calcData.ProjectTemperatureRef2 : calcData.NonProjectTemperatureRef2);
			num13 += CalculateHveRef2(ventCool) * (num20 - ventCool.FlowTemperatureRef2) / 1000.0;
		}
		double num21 = (num13 + num14) * (double)month.Sundays;
		double num22 = 0.0;
		for (int num23 = 0; num23 < 24; num23++)
		{
			num22 += CalculateHveRef2(ventCool) * (calcData.NonProjectTemperatureRef2 - ventCool.FlowTemperatureRef2) / 1000.0;
		}
		double num24 = num22 * (double)month.Holydays;
		return num6 + num12 + num21 + num24;
	}

	private static double CalculateQve(Section section, CalculationData calcData, CalculationData ventCool, MonthlyDays month)
	{
		double num = 0.0;
		double num2 = 0.0;
		for (int i = 0; i < section.CoolingSeasons.Ventilation.WorkCurrentStart; i++)
		{
			double num3 = ((i < section.CoolingSeasons.Occupants.WorkCurrentStart) ? calcData.NonProjectTemperatureActual : calcData.ProjectTemperatureActual);
			num += CalculateHve(ventCool) * (num3 - ventCool.FlowTemperatureActual) / 1000.0;
		}
		for (int j = section.CoolingSeasons.Ventilation.WorkCurrentStart; j < section.CoolingSeasons.Ventilation.WorkCurrentEnd; j++)
		{
			double num4 = ((j >= section.CoolingSeasons.Occupants.WorkCurrentStart && j < section.CoolingSeasons.Occupants.WorkCurrentEnd) ? calcData.ProjectTemperatureActual : calcData.NonProjectTemperatureActual);
			num2 += CalculateHve(ventCool) * (num4 - ventCool.FlowTemperatureActual) / 1000.0;
		}
		for (int k = section.CoolingSeasons.Ventilation.WorkCurrentEnd; k < 24; k++)
		{
			double num5 = ((k < section.CoolingSeasons.Occupants.WorkCurrentEnd) ? calcData.ProjectTemperatureActual : calcData.NonProjectTemperatureActual);
			num += CalculateHve(ventCool) * (num5 - ventCool.FlowTemperatureActual) / 1000.0;
		}
		double num6 = (num2 + num) * (double)month.WorkDays;
		double num7 = 0.0;
		double num8 = 0.0;
		for (int l = 0; l < section.CoolingSeasons.Ventilation.SatCurrentStart; l++)
		{
			double num9 = ((l < section.CoolingSeasons.Occupants.SatCurrentStart) ? calcData.NonProjectTemperatureActual : calcData.ProjectTemperatureActual);
			num7 += CalculateHve(ventCool) * (num9 - ventCool.FlowTemperatureActual) / 1000.0;
		}
		for (int m = section.CoolingSeasons.Ventilation.SatCurrentStart; m < section.CoolingSeasons.Ventilation.SatCurrentEnd; m++)
		{
			double num10 = ((m >= section.CoolingSeasons.Occupants.SatCurrentStart && m < section.CoolingSeasons.Occupants.SatCurrentEnd) ? calcData.ProjectTemperatureActual : calcData.NonProjectTemperatureActual);
			num8 += CalculateHve(ventCool) * (num10 - ventCool.FlowTemperatureActual) / 1000.0;
		}
		for (int n = section.CoolingSeasons.Ventilation.SatCurrentEnd; n < 24; n++)
		{
			double num11 = ((n < section.CoolingSeasons.Occupants.SatCurrentEnd) ? calcData.ProjectTemperatureActual : calcData.NonProjectTemperatureActual);
			num7 += CalculateHve(ventCool) * (num11 - ventCool.FlowTemperatureActual) / 1000.0;
		}
		double num12 = (num7 + num8) * (double)month.Saturdays;
		double num13 = 0.0;
		double num14 = 0.0;
		for (int num15 = 0; num15 < section.CoolingSeasons.Ventilation.SunCurrentStart; num15++)
		{
			double num16 = ((num15 < section.CoolingSeasons.Occupants.SunCurrentStart) ? calcData.NonProjectTemperatureActual : calcData.ProjectTemperatureActual);
			num13 += CalculateHve(ventCool) * (num16 - ventCool.FlowTemperatureActual) / 1000.0;
		}
		for (int num17 = section.CoolingSeasons.Ventilation.SunCurrentStart; num17 < section.CoolingSeasons.Ventilation.SunCurrentEnd; num17++)
		{
			double num18 = ((num17 >= section.CoolingSeasons.Occupants.SunCurrentStart && num17 < section.CoolingSeasons.Occupants.SunCurrentEnd) ? calcData.ProjectTemperatureActual : calcData.NonProjectTemperatureActual);
			num14 += CalculateHve(ventCool) * (num18 - ventCool.FlowTemperatureActual) / 1000.0;
		}
		for (int num19 = section.CoolingSeasons.Ventilation.SunCurrentEnd; num19 < 24; num19++)
		{
			double num20 = ((num19 < section.CoolingSeasons.Occupants.SunCurrentEnd) ? calcData.ProjectTemperatureActual : calcData.NonProjectTemperatureActual);
			num13 += CalculateHve(ventCool) * (num20 - ventCool.FlowTemperatureActual) / 1000.0;
		}
		double num21 = (num13 + num14) * (double)month.Sundays;
		double num22 = 0.0;
		for (int num23 = 0; num23 < 24; num23++)
		{
			num22 += CalculateHve(ventCool) * (calcData.NonProjectTemperatureActual - ventCool.FlowTemperatureActual) / 1000.0;
		}
		double num24 = num22 * (double)month.Holydays;
		return num6 + num12 + num21 + num24;
	}

	private static double CalculateQveBaseLine(Section section, CalculationData calcData, CalculationData ventCool, MonthlyDays month)
	{
		double num = 0.0;
		double num2 = 0.0;
		for (int i = 0; i < section.CoolingSeasons.Ventilation.WorkBaseStart; i++)
		{
			double num3 = ((i < section.CoolingSeasons.Occupants.WorkBaseStart) ? calcData.NonProjectTemperatureBaseLine : calcData.ProjectTemperatureBaseLine);
			num += CalculateHveBaseLine(ventCool) * (num3 - ventCool.FlowTemperatureBaseLine) / 1000.0;
		}
		for (int j = section.CoolingSeasons.Ventilation.WorkBaseStart; j < section.CoolingSeasons.Ventilation.WorkBaseEnd; j++)
		{
			double num4 = ((j >= section.CoolingSeasons.Occupants.WorkBaseStart && j < section.CoolingSeasons.Occupants.WorkBaseEnd) ? calcData.ProjectTemperatureBaseLine : calcData.NonProjectTemperatureBaseLine);
			num2 += CalculateHveBaseLine(ventCool) * (num4 - ventCool.FlowTemperatureBaseLine) / 1000.0;
		}
		for (int k = section.CoolingSeasons.Ventilation.WorkBaseEnd; k < 24; k++)
		{
			double num5 = ((k < section.CoolingSeasons.Occupants.WorkBaseEnd) ? calcData.ProjectTemperatureBaseLine : calcData.NonProjectTemperatureBaseLine);
			num += CalculateHveBaseLine(ventCool) * (num5 - ventCool.FlowTemperatureBaseLine) / 1000.0;
		}
		double num6 = (num2 + num) * (double)month.WorkDays;
		double num7 = 0.0;
		double num8 = 0.0;
		for (int l = 0; l < section.CoolingSeasons.Ventilation.SatBaseStart; l++)
		{
			double num9 = ((l < section.CoolingSeasons.Occupants.SatBaseStart) ? calcData.NonProjectTemperatureBaseLine : calcData.ProjectTemperatureBaseLine);
			num7 += CalculateHveBaseLine(ventCool) * (num9 - ventCool.FlowTemperatureBaseLine) / 1000.0;
		}
		for (int m = section.CoolingSeasons.Ventilation.SatBaseStart; m < section.CoolingSeasons.Ventilation.SatBaseEnd; m++)
		{
			double num10 = ((m >= section.CoolingSeasons.Occupants.SatBaseStart && m < section.CoolingSeasons.Occupants.SatBaseEnd) ? calcData.ProjectTemperatureBaseLine : calcData.NonProjectTemperatureBaseLine);
			num8 += CalculateHveBaseLine(ventCool) * (num10 - ventCool.FlowTemperatureBaseLine) / 1000.0;
		}
		for (int n = section.CoolingSeasons.Ventilation.SatBaseEnd; n < 24; n++)
		{
			double num11 = ((n < section.CoolingSeasons.Occupants.SatBaseEnd) ? calcData.ProjectTemperatureBaseLine : calcData.NonProjectTemperatureBaseLine);
			num7 += CalculateHveBaseLine(ventCool) * (num11 - ventCool.FlowTemperatureBaseLine) / 1000.0;
		}
		double num12 = (num7 + num8) * (double)month.Saturdays;
		double num13 = 0.0;
		double num14 = 0.0;
		for (int num15 = 0; num15 < section.CoolingSeasons.Ventilation.SunBaseStart; num15++)
		{
			double num16 = ((num15 < section.CoolingSeasons.Occupants.SunBaseStart) ? calcData.NonProjectTemperatureBaseLine : calcData.ProjectTemperatureBaseLine);
			num13 += CalculateHveBaseLine(ventCool) * (num16 - ventCool.FlowTemperatureBaseLine) / 1000.0;
		}
		for (int num17 = section.CoolingSeasons.Ventilation.SunBaseStart; num17 < section.CoolingSeasons.Ventilation.SunBaseEnd; num17++)
		{
			double num18 = ((num17 >= section.CoolingSeasons.Occupants.SunBaseStart && num17 < section.CoolingSeasons.Occupants.SunBaseEnd) ? calcData.ProjectTemperatureBaseLine : calcData.NonProjectTemperatureBaseLine);
			num14 += CalculateHveBaseLine(ventCool) * (num18 - ventCool.FlowTemperatureBaseLine) / 1000.0;
		}
		for (int num19 = section.CoolingSeasons.Ventilation.SunBaseEnd; num19 < 24; num19++)
		{
			double num20 = ((num19 < section.CoolingSeasons.Occupants.SunBaseEnd) ? calcData.ProjectTemperatureBaseLine : calcData.NonProjectTemperatureBaseLine);
			num13 += CalculateHveBaseLine(ventCool) * (num20 - ventCool.FlowTemperatureBaseLine) / 1000.0;
		}
		double num21 = (num13 + num14) * (double)month.Sundays;
		double num22 = 0.0;
		for (int num23 = 0; num23 < 24; num23++)
		{
			num22 += CalculateHveBaseLine(ventCool) * (calcData.NonProjectTemperatureBaseLine - ventCool.FlowTemperatureBaseLine) / 1000.0;
		}
		double num24 = num22 * (double)month.Holydays;
		return num6 + num12 + num21 + num24;
	}

	private static double CalculateQveESM(Section section, CalculationData calcData, CalculationData ventCool, MonthlyDays month)
	{
		double num = 0.0;
		double num2 = 0.0;
		for (int i = 0; i < section.CoolingSeasons.Ventilation.WorkEsmStart; i++)
		{
			double num3 = ((i < section.CoolingSeasons.Occupants.WorkEsmStart) ? calcData.NonProjectTemperatureESM : calcData.ProjectTemperatureESM);
			num += CalculateHveESM(ventCool) * (num3 - ventCool.FlowTemperatureESM) / 1000.0;
		}
		for (int j = section.CoolingSeasons.Ventilation.WorkEsmStart; j < section.CoolingSeasons.Ventilation.WorkEsmEnd; j++)
		{
			double num4 = ((j >= section.CoolingSeasons.Occupants.WorkEsmStart && j < section.CoolingSeasons.Occupants.WorkEsmEnd) ? calcData.ProjectTemperatureESM : calcData.NonProjectTemperatureESM);
			num2 += CalculateHveESM(ventCool) * (num4 - ventCool.FlowTemperatureESM) / 1000.0;
		}
		for (int k = section.CoolingSeasons.Ventilation.WorkEsmEnd; k < 24; k++)
		{
			double num5 = ((k < section.CoolingSeasons.Occupants.WorkEsmEnd) ? calcData.ProjectTemperatureESM : calcData.NonProjectTemperatureESM);
			num += CalculateHveESM(ventCool) * (num5 - ventCool.FlowTemperatureESM) / 1000.0;
		}
		double num6 = (num2 + num) * (double)month.WorkDays;
		double num7 = 0.0;
		double num8 = 0.0;
		for (int l = 0; l < section.CoolingSeasons.Ventilation.SatEsmStart; l++)
		{
			double num9 = ((l < section.CoolingSeasons.Occupants.SatEsmStart) ? calcData.NonProjectTemperatureESM : calcData.ProjectTemperatureESM);
			num7 += CalculateHveESM(ventCool) * (num9 - ventCool.FlowTemperatureESM) / 1000.0;
		}
		for (int m = section.CoolingSeasons.Ventilation.SatEsmStart; m < section.CoolingSeasons.Ventilation.SatEsmEnd; m++)
		{
			double num10 = ((m >= section.CoolingSeasons.Occupants.SatEsmStart && m < section.CoolingSeasons.Occupants.SatEsmEnd) ? calcData.ProjectTemperatureESM : calcData.NonProjectTemperatureESM);
			num8 += CalculateHveESM(ventCool) * (num10 - ventCool.FlowTemperatureESM) / 1000.0;
		}
		for (int n = section.CoolingSeasons.Ventilation.SatEsmEnd; n < 24; n++)
		{
			double num11 = ((n < section.CoolingSeasons.Occupants.SatEsmEnd) ? calcData.ProjectTemperatureESM : calcData.NonProjectTemperatureESM);
			num7 += CalculateHveESM(ventCool) * (num11 - ventCool.FlowTemperatureESM) / 1000.0;
		}
		double num12 = (num7 + num8) * (double)month.Saturdays;
		double num13 = 0.0;
		double num14 = 0.0;
		for (int num15 = 0; num15 < section.CoolingSeasons.Ventilation.SunEsmStart; num15++)
		{
			double num16 = ((num15 < section.CoolingSeasons.Occupants.SunEsmStart) ? calcData.NonProjectTemperatureESM : calcData.ProjectTemperatureESM);
			num13 += CalculateHveESM(ventCool) * (num16 - ventCool.FlowTemperatureESM) / 1000.0;
		}
		for (int num17 = section.CoolingSeasons.Ventilation.SunEsmStart; num17 < section.CoolingSeasons.Ventilation.SunEsmEnd; num17++)
		{
			double num18 = ((num17 >= section.CoolingSeasons.Occupants.SunEsmStart && num17 < section.CoolingSeasons.Occupants.SunEsmEnd) ? calcData.ProjectTemperatureESM : calcData.NonProjectTemperatureESM);
			num14 += CalculateHveESM(ventCool) * (num18 - ventCool.FlowTemperatureESM) / 1000.0;
		}
		for (int num19 = section.CoolingSeasons.Ventilation.SunEsmEnd; num19 < 24; num19++)
		{
			double num20 = ((num19 < section.CoolingSeasons.Occupants.SunEsmEnd) ? calcData.ProjectTemperatureESM : calcData.NonProjectTemperatureESM);
			num13 += CalculateHveESM(ventCool) * (num20 - ventCool.FlowTemperatureESM) / 1000.0;
		}
		double num21 = (num13 + num14) * (double)month.Sundays;
		double num22 = 0.0;
		for (int num23 = 0; num23 < 24; num23++)
		{
			num22 += CalculateHveESM(ventCool) * (calcData.NonProjectTemperatureESM - ventCool.FlowTemperatureESM) / 1000.0;
		}
		double num24 = num22 * (double)month.Holydays;
		return num6 + num12 + num21 + num24;
	}

	private static double CalculateHveRef1(CalculationData ventCool)
	{
		return ventCool.DebitRef1 * 0.34;
	}

	private static double CalculateHveRef2(CalculationData ventCool)
	{
		return ventCool.DebitRef2 * 0.34;
	}

	private static double CalculateHve(CalculationData ventCool)
	{
		return ventCool.DebitActual * 0.34;
	}

	private static double CalculateHveBaseLine(CalculationData ventCool)
	{
		return ventCool.DebitBaseLine * 0.34;
	}

	private static double CalculateHveESM(CalculationData ventCool)
	{
		return ventCool.DebitESM * 0.34;
	}

	private static double CalculateQsolRef1(Section section, ClimateZones climateZone, MonthlyDays month)
	{
		int num = month.WorkDays * (section.CoolingSeasons.Cooling.WorkBaseEnd - section.CoolingSeasons.Cooling.WorkBaseStart);
		num = ((month.Sundays > 0) ? (month.Sundays * (section.CoolingSeasons.Cooling.SunBaseEnd - section.CoolingSeasons.Cooling.SunBaseStart) + num) : num);
		num = ((month.Saturdays > 0) ? (month.Saturdays * (section.CoolingSeasons.Cooling.SatBaseEnd - section.CoolingSeasons.Cooling.SatBaseStart) + num) : num);
		int num2 = month.WorkDays * (24 - (section.CoolingSeasons.Cooling.WorkBaseEnd - section.CoolingSeasons.Cooling.WorkBaseStart));
		num2 = month.Saturdays * (24 - (section.CoolingSeasons.Cooling.SatBaseEnd - section.CoolingSeasons.Cooling.SatBaseStart)) + num2;
		num2 = month.Sundays * (24 - (section.CoolingSeasons.Cooling.SunBaseEnd - section.CoolingSeasons.Cooling.SunBaseStart)) + num2;
		num2 = month.Holydays * 24 + num2;
		double num3 = CalculateNonTrasparentFsol(section, climateZone, month);
		double num4 = CalculateTrasparentFsol(section, climateZone, month);
		return (num4 + num3) * (double)(num + num2) / 1000.0;
	}

	private static double CalculateQsolRef2(Section section, ClimateZones climateZone, MonthlyDays month)
	{
		int num = month.WorkDays * (section.CoolingSeasons.Cooling.WorkBaseEnd - section.CoolingSeasons.Cooling.WorkBaseStart);
		num = ((month.Sundays > 0) ? (month.Sundays * (section.CoolingSeasons.Cooling.SunBaseEnd - section.CoolingSeasons.Cooling.SunBaseStart) + num) : num);
		num = ((month.Saturdays > 0) ? (month.Saturdays * (section.CoolingSeasons.Cooling.SatBaseEnd - section.CoolingSeasons.Cooling.SatBaseStart) + num) : num);
		int num2 = month.WorkDays * (24 - (section.CoolingSeasons.Cooling.WorkBaseEnd - section.CoolingSeasons.Cooling.WorkBaseStart));
		num2 = month.Saturdays * (24 - (section.CoolingSeasons.Cooling.SatBaseEnd - section.CoolingSeasons.Cooling.SatBaseStart)) + num2;
		num2 = month.Sundays * (24 - (section.CoolingSeasons.Cooling.SunBaseEnd - section.CoolingSeasons.Cooling.SunBaseStart)) + num2;
		num2 = month.Holydays * 24 + num2;
		double num3 = CalculateNonTrasparentFsol(section, climateZone, month);
		double num4 = CalculateTrasparentFsol(section, climateZone, month);
		return (num4 + num3) * (double)(num + num2) / 1000.0;
	}

	private static double CalculateQsol(Section section, ClimateZones climateZone, MonthlyDays month)
	{
		int num = month.WorkDays * (section.CoolingSeasons.Cooling.WorkCurrentEnd - section.CoolingSeasons.Cooling.WorkCurrentStart);
		num = ((month.Sundays > 0) ? (month.Sundays * (section.CoolingSeasons.Cooling.SunCurrentEnd - section.CoolingSeasons.Cooling.SunCurrentStart) + num) : num);
		num = ((month.Saturdays > 0) ? (month.Saturdays * (section.CoolingSeasons.Cooling.SatCurrentEnd - section.CoolingSeasons.Cooling.SatCurrentStart) + num) : num);
		int num2 = month.WorkDays * (24 - (section.CoolingSeasons.Cooling.WorkCurrentEnd - section.CoolingSeasons.Cooling.WorkCurrentStart));
		num2 = month.Saturdays * (24 - (section.CoolingSeasons.Cooling.SatCurrentEnd - section.CoolingSeasons.Cooling.SatCurrentStart)) + num2;
		num2 = month.Sundays * (24 - (section.CoolingSeasons.Cooling.SunCurrentEnd - section.CoolingSeasons.Cooling.SunCurrentStart)) + num2;
		num2 = month.Holydays * 24 + num2;
		double num3 = CalculateNonTrasparentFsol(section, climateZone, month);
		double num4 = CalculateTrasparentFsol(section, climateZone, month);
		return (num4 + num3) * (double)(num + num2) / 1000.0;
	}

	private static double CalculateQsolBaseLine(Section section, ClimateZones climateZone, MonthlyDays month)
	{
		int num = month.WorkDays * (section.CoolingSeasons.Cooling.WorkBaseEnd - section.CoolingSeasons.Cooling.WorkBaseStart);
		num = ((month.Sundays > 0) ? (month.Sundays * (section.CoolingSeasons.Cooling.SunBaseEnd - section.CoolingSeasons.Cooling.SunBaseStart) + num) : num);
		num = ((month.Saturdays > 0) ? (month.Saturdays * (section.CoolingSeasons.Cooling.SatBaseEnd - section.CoolingSeasons.Cooling.SatBaseStart) + num) : num);
		int num2 = month.WorkDays * (24 - (section.CoolingSeasons.Cooling.WorkBaseEnd - section.CoolingSeasons.Cooling.WorkBaseStart));
		num2 = month.Saturdays * (24 - (section.CoolingSeasons.Cooling.SatBaseEnd - section.CoolingSeasons.Cooling.SatBaseStart)) + num2;
		num2 = month.Sundays * (24 - (section.CoolingSeasons.Cooling.SunBaseEnd - section.CoolingSeasons.Cooling.SunBaseStart)) + num2;
		num2 = month.Holydays * 24 + num2;
		double num3 = CalculateNonTrasparentFsol(section, climateZone, month);
		double num4 = CalculateTrasparentFsol(section, climateZone, month);
		return (num4 + num3) * (double)(num + num2) / 1000.0;
	}

	private static double CalculateQsolESM(Section section, ClimateZones climateZone, MonthlyDays month)
	{
		int num = month.WorkDays * (section.CoolingSeasons.Cooling.WorkEsmEnd - section.CoolingSeasons.Cooling.WorkEsmStart);
		num = ((month.Sundays > 0) ? (month.Sundays * (section.CoolingSeasons.Cooling.SunEsmEnd - section.CoolingSeasons.Cooling.SunEsmStart) + num) : num);
		num = ((month.Saturdays > 0) ? (month.Saturdays * (section.CoolingSeasons.Cooling.SatEsmEnd - section.CoolingSeasons.Cooling.SatEsmStart) + num) : num);
		int num2 = month.WorkDays * (24 - (section.CoolingSeasons.Cooling.WorkEsmEnd - section.CoolingSeasons.Cooling.WorkEsmStart));
		num2 = month.Saturdays * (24 - (section.CoolingSeasons.Cooling.SatEsmEnd - section.CoolingSeasons.Cooling.SatEsmStart)) + num2;
		num2 = month.Sundays * (24 - (section.CoolingSeasons.Cooling.SunEsmEnd - section.CoolingSeasons.Cooling.SunEsmStart)) + num2;
		num2 = month.Holydays * 24 + num2;
		double num3 = CalculateNonTrasparentFsolEsm(section, climateZone, month);
		double num4 = CalculateTrasparentFsolEsm(section, climateZone, month);
		return (num4 + num3) * (double)(num + num2) / 1000.0;
	}

	private static double CalculateQinfRef1(Section section, ClimateZones climateZone, CalculationData calcData, MonthlyDays month)
	{
		double avgTemp = PreferencesManager.GetClimateZoneParams(climateZone).SolarRadiation.Months[(int)month.Month].AvgTemp;
		return CalculateHinfRef1(section, calcData) * (CalcAvgProjectTempCoolingRef1(section, avgTemp, calcData, month) + CalcAvgNonProjectTempCoolingRef1(section, avgTemp, calcData, month)) / 1000.0;
	}

	private static double CalculateQinfRef2(Section section, ClimateZones climateZone, CalculationData calcData, MonthlyDays month)
	{
		double avgTemp = PreferencesManager.GetClimateZoneParams(climateZone).SolarRadiation.Months[(int)month.Month].AvgTemp;
		return CalculateHinfRef2(section, calcData) * (CalcAvgProjectTempCoolingRef2(section, avgTemp, calcData, month) + CalcAvgNonProjectTempCoolingRef2(section, avgTemp, calcData, month)) / 1000.0;
	}

	private static double CalculateQinf(Section section, ClimateZones climateZone, CalculationData calcData, MonthlyDays month)
	{
		double avgTemp = PreferencesManager.GetClimateZoneParams(climateZone).SolarRadiation.Months[(int)month.Month].AvgTemp;
		return CalculateHinf(section, calcData) * (CalcAvgProjectTempCooling(section, avgTemp, calcData, month) + CalcAvgNonProjectTempCooling(section, avgTemp, calcData, month)) / 1000.0;
	}

	private static double CalculateQinfBaseLine(Section section, ClimateZones climateZone, CalculationData calcData, MonthlyDays month)
	{
		double avgTemp = PreferencesManager.GetClimateZoneParams(climateZone).SolarRadiation.Months[(int)month.Month].AvgTemp;
		return CalculateHinfBaseLine(section, calcData) * (CalcAvgProjectTempCoolingBaseLine(section, avgTemp, calcData, month) + CalcAvgNonProjectTempCoolingBaseLine(section, avgTemp, calcData, month)) / 1000.0;
	}

	private static double CalculateQinfESM(Section section, ClimateZones climateZone, CalculationData calcData, MonthlyDays month)
	{
		double avgTemp = PreferencesManager.GetClimateZoneParams(climateZone).SolarRadiation.Months[(int)month.Month].AvgTemp;
		return CalculateHinfESM(section, calcData) * (CalcAvgProjectTempCoolingESM(section, avgTemp, calcData, month) + CalcAvgNonProjectTempCoolingESM(section, avgTemp, calcData, month)) / 1000.0;
	}

	private static double CalculateHinfRef1(Section section, CalculationData calcData)
	{
		return section.Area.HeatedVolume * calcData.InfiltracionRef1 * 0.34;
	}

	private static double CalculateHinfRef2(Section section, CalculationData calcData)
	{
		return section.Area.HeatedVolume * calcData.InfiltracionRef2 * 0.34;
	}

	private static double CalculateHinf(Section section, CalculationData calcData)
	{
		return section.Area.HeatedVolume * calcData.InfiltracionActual * 0.34;
	}

	private static double CalculateHinfBaseLine(Section section, CalculationData calcData)
	{
		return section.Area.HeatedVolume * calcData.InfiltracionBaseLine * 0.34;
	}

	private static double CalculateHinfESM(Section section, CalculationData calcData)
	{
		return section.Area.HeatedVolume * calcData.InfiltracionESM * 0.34;
	}

	private static double CalculateCoolingQtrRef1(CalculationData calcData, Section section, ClimateZones climateZone, MonthlyDays month)
	{
		double avgTemp = PreferencesManager.GetClimateZoneParams(climateZone).SolarRadiation.Months[(int)month.Month].AvgTemp;
		double averageInnerCoolTemp = CalculateAverageCoolingTempRef1(section, calcData, month);
		double num = CalculateCoolingHtr(section, avgTemp, averageInnerCoolTemp);
		section.Test.ParameterHtr = num;
		return num * (CalcAvgProjectTempCoolingRef1(section, avgTemp, calcData, month) + CalcAvgNonProjectTempCoolingRef1(section, avgTemp, calcData, month)) / 1000.0;
	}

	private static double CalculateCoolingQtrRef2(CalculationData calcData, Section section, ClimateZones climateZone, MonthlyDays month)
	{
		double avgTemp = PreferencesManager.GetClimateZoneParams(climateZone).SolarRadiation.Months[(int)month.Month].AvgTemp;
		double averageInnerCoolTemp = CalculateAverageCoolingTempRef2(section, calcData, month);
		double num = CalculateCoolingHtr(section, avgTemp, averageInnerCoolTemp);
		section.Test.ParameterHtr = num;
		return num * (CalcAvgProjectTempCoolingRef2(section, avgTemp, calcData, month) + CalcAvgNonProjectTempCoolingRef2(section, avgTemp, calcData, month)) / 1000.0;
	}

	private static double CalculateCoolingQtr(CalculationData calcData, Section section, ClimateZones climateZone, MonthlyDays month)
	{
		double avgTemp = PreferencesManager.GetClimateZoneParams(climateZone).SolarRadiation.Months[(int)month.Month].AvgTemp;
		double averageInnerCoolTemp = CalculateAverageCoolingTempCurrent(section, calcData, month);
		double num = CalculateCoolingHtr(section, avgTemp, averageInnerCoolTemp);
		section.Test.ParameterHtr = num;
		return num * (CalcAvgProjectTempCooling(section, avgTemp, calcData, month) + CalcAvgNonProjectTempCooling(section, avgTemp, calcData, month)) / 1000.0;
	}

	private static double CalculateCoolingQtrBaseLine(CalculationData calculationData, Section section, ClimateZones climateZone, MonthlyDays month)
	{
		double avgTemp = PreferencesManager.GetClimateZoneParams(climateZone).SolarRadiation.Months[(int)month.Month].AvgTemp;
		double averageInnerCoolTemp = CalculateAverageCoolingTempBaseLine(section, calculationData, month);
		double num = CalculateCoolingHtr(section, avgTemp, averageInnerCoolTemp);
		return num * (CalcAvgProjectTempCoolingBaseLine(section, avgTemp, calculationData, month) + CalcAvgNonProjectTempCoolingBaseLine(section, avgTemp, calculationData, month)) / 1000.0;
	}

	private static double CalculateCoolingQtrESM(CalculationData calculationData, Section section, ClimateZones climateZone, MonthlyDays month)
	{
		double avgTemp = PreferencesManager.GetClimateZoneParams(climateZone).SolarRadiation.Months[(int)month.Month].AvgTemp;
		double averageInnerCoolTemp = CalculateAverageCoolingTempESM(section, calculationData, month);
		double num = CalculateCoolingHtrESM(section, avgTemp, averageInnerCoolTemp);
		return num * (CalcAvgProjectTempCoolingESM(section, avgTemp, calculationData, month) + CalcAvgNonProjectTempCoolingESM(section, avgTemp, calculationData, month)) / 1000.0;
	}

	private static double CalculateCoolingHtr(Section section, double averageMontlyTemp, double averageInnerCoolTemp)
	{
		double num = SumWallDirecrionsHu1Cooling(section, averageMontlyTemp, averageInnerCoolTemp);
		double num2 = CalcCeilingsParameterHu2Cooling(section.Roof.Current, averageMontlyTemp, averageInnerCoolTemp);
		double num3 = CalcFloorsParameterHu3Cooling(section.Floor.Current, averageMontlyTemp, averageInnerCoolTemp);
		double num4 = num + num2 + num3;
		double num5 = CalculateParameterHdCurrent(section);
		double num6 = CalculateParameterHgCurrent(section);
		section.Test.ParameterHu = num + num2 + num3;
		section.Test.ParameterHd = CalculateParameterHdCurrent(section);
		section.Test.ParameterHg = CalculateParameterHgCurrent(section);
		return num5 + num6 + num4;
	}

	private static double CalculateCoolingHtrESM(Section section, double averageMontlyTemp, double averageInnerCoolTemp)
	{
		double num = SumWallDirecrionsHu1CoolingESM(section, averageMontlyTemp, averageInnerCoolTemp);
		double num2 = CalcCeilingsParameterHu2Cooling(section.Roof.Esm, averageMontlyTemp, averageInnerCoolTemp);
		double num3 = CalcFloorsParameterHu3Cooling(section.Floor.Esm, averageMontlyTemp, averageInnerCoolTemp);
		double num4 = num + num2 + num3;
		double num5 = CalculateParameterHdEsm(section);
		double num6 = CalculateParameterHgEsm(section);
		section.Test.ParameterHu = num + num2 + num3;
		section.Test.ParameterHd = CalculateParameterHdEsm(section);
		section.Test.ParameterHg = CalculateParameterHgEsm(section);
		return num5 + num6 + num4;
	}

	private static double CalculateAverageCoolingTempRef1(Section section, CalculationData calculationData, MonthlyDays month)
	{
		int num = month.WorkDays * (section.CoolingSeasons.Cooling.WorkBaseEnd - section.CoolingSeasons.Cooling.WorkBaseStart);
		num = ((month.Sundays > 0) ? (month.Sundays * (section.CoolingSeasons.Cooling.SunBaseEnd - section.CoolingSeasons.Cooling.SunBaseStart) + num) : num);
		num = ((month.Saturdays > 0) ? (month.Saturdays * (section.CoolingSeasons.Cooling.SatBaseEnd - section.CoolingSeasons.Cooling.SatBaseStart) + num) : num);
		double projectTemperatureRef = calculationData.ProjectTemperatureRef1;
		int num2 = month.WorkDays * (24 - (section.CoolingSeasons.Cooling.WorkBaseEnd - section.CoolingSeasons.Cooling.WorkBaseStart));
		num2 = month.Saturdays * (24 - (section.CoolingSeasons.Cooling.SatBaseEnd - section.CoolingSeasons.Cooling.SatBaseStart)) + num2;
		num2 = month.Sundays * (24 - (section.CoolingSeasons.Cooling.SunBaseEnd - section.CoolingSeasons.Cooling.SunBaseStart)) + num2;
		num2 = month.Holydays * 24 + num2;
		double nonProjectTemperatureRef = calculationData.NonProjectTemperatureRef1;
		return ((double)num * projectTemperatureRef + (double)num2 * nonProjectTemperatureRef) / (double)(num + num2);
	}

	private static double CalculateAverageCoolingTempRef2(Section section, CalculationData calculationData, MonthlyDays month)
	{
		int num = month.WorkDays * (section.CoolingSeasons.Cooling.WorkBaseEnd - section.CoolingSeasons.Cooling.WorkBaseStart);
		num = ((month.Sundays > 0) ? (month.Sundays * (section.CoolingSeasons.Cooling.SunBaseEnd - section.CoolingSeasons.Cooling.SunBaseStart) + num) : num);
		num = ((month.Saturdays > 0) ? (month.Saturdays * (section.CoolingSeasons.Cooling.SatBaseEnd - section.CoolingSeasons.Cooling.SatBaseStart) + num) : num);
		double projectTemperatureRef = calculationData.ProjectTemperatureRef2;
		int num2 = month.WorkDays * (24 - (section.CoolingSeasons.Cooling.WorkBaseEnd - section.CoolingSeasons.Cooling.WorkBaseStart));
		num2 = month.Saturdays * (24 - (section.CoolingSeasons.Cooling.SatBaseEnd - section.CoolingSeasons.Cooling.SatBaseStart)) + num2;
		num2 = month.Sundays * (24 - (section.CoolingSeasons.Cooling.SunBaseEnd - section.CoolingSeasons.Cooling.SunBaseStart)) + num2;
		num2 = month.Holydays * 24 + num2;
		double nonProjectTemperatureRef = calculationData.NonProjectTemperatureRef2;
		return ((double)num * projectTemperatureRef + (double)num2 * nonProjectTemperatureRef) / (double)(num + num2);
	}

	private static double CalculateAverageCoolingTempCurrent(Section section, CalculationData calculationData, MonthlyDays month)
	{
		int num = month.WorkDays * (section.CoolingSeasons.Cooling.WorkCurrentEnd - section.CoolingSeasons.Cooling.WorkCurrentStart);
		num = ((month.Sundays > 0) ? (month.Sundays * (section.CoolingSeasons.Cooling.SunCurrentEnd - section.CoolingSeasons.Cooling.SunCurrentStart) + num) : num);
		num = ((month.Saturdays > 0) ? (month.Saturdays * (section.CoolingSeasons.Cooling.SatCurrentEnd - section.CoolingSeasons.Cooling.SatCurrentStart) + num) : num);
		double projectTemperatureActual = calculationData.ProjectTemperatureActual;
		int num2 = month.WorkDays * (24 - (section.CoolingSeasons.Cooling.WorkCurrentEnd - section.CoolingSeasons.Cooling.WorkCurrentStart));
		num2 = month.Saturdays * (24 - (section.CoolingSeasons.Cooling.SatCurrentEnd - section.CoolingSeasons.Cooling.SatCurrentStart)) + num2;
		num2 = month.Sundays * (24 - (section.CoolingSeasons.Cooling.SunCurrentEnd - section.CoolingSeasons.Cooling.SunCurrentStart)) + num2;
		num2 = month.Holydays * 24 + num2;
		double nonProjectTemperatureActual = calculationData.NonProjectTemperatureActual;
		return ((double)num * projectTemperatureActual + (double)num2 * nonProjectTemperatureActual) / (double)(num + num2);
	}

	private static double CalculateAverageCoolingTempBaseLine(Section section, CalculationData calculationData, MonthlyDays month)
	{
		int num = month.WorkDays * (section.CoolingSeasons.Cooling.WorkBaseEnd - section.CoolingSeasons.Cooling.WorkBaseStart);
		num = ((month.Sundays > 0) ? (month.Sundays * (section.CoolingSeasons.Cooling.SunBaseEnd - section.CoolingSeasons.Cooling.SunBaseStart) + num) : num);
		num = ((month.Saturdays > 0) ? (month.Saturdays * (section.CoolingSeasons.Cooling.SatBaseEnd - section.CoolingSeasons.Cooling.SatBaseStart) + num) : num);
		double projectTemperatureBaseLine = calculationData.ProjectTemperatureBaseLine;
		int num2 = month.WorkDays * (24 - (section.CoolingSeasons.Cooling.WorkBaseEnd - section.CoolingSeasons.Cooling.WorkBaseStart));
		num2 = month.Saturdays * (24 - (section.CoolingSeasons.Cooling.SatBaseEnd - section.CoolingSeasons.Cooling.SatBaseStart)) + num2;
		num2 = month.Sundays * (24 - (section.CoolingSeasons.Cooling.SunBaseEnd - section.CoolingSeasons.Cooling.SunBaseStart)) + num2;
		num2 = month.Holydays * 24 + num2;
		double nonProjectTemperatureBaseLine = calculationData.NonProjectTemperatureBaseLine;
		return ((double)num * projectTemperatureBaseLine + (double)num2 * nonProjectTemperatureBaseLine) / (double)(num + num2);
	}

	private static double CalculateAverageCoolingTempESM(Section section, CalculationData calculationData, MonthlyDays month)
	{
		int num = month.WorkDays * (section.CoolingSeasons.Cooling.WorkEsmEnd - section.CoolingSeasons.Cooling.WorkEsmStart);
		num = ((month.Sundays > 0) ? (month.Sundays * (section.CoolingSeasons.Cooling.SunEsmEnd - section.CoolingSeasons.Cooling.SunEsmStart) + num) : num);
		num = ((month.Saturdays > 0) ? (month.Saturdays * (section.CoolingSeasons.Cooling.SatEsmEnd - section.CoolingSeasons.Cooling.SatEsmStart) + num) : num);
		double projectTemperatureESM = calculationData.ProjectTemperatureESM;
		int num2 = month.WorkDays * (24 - (section.CoolingSeasons.Cooling.WorkEsmEnd - section.CoolingSeasons.Cooling.WorkEsmStart));
		num2 = month.Saturdays * (24 - (section.CoolingSeasons.Cooling.SatEsmEnd - section.CoolingSeasons.Cooling.SatEsmStart)) + num2;
		num2 = month.Sundays * (24 - (section.CoolingSeasons.Cooling.SunEsmEnd - section.CoolingSeasons.Cooling.SunEsmStart)) + num2;
		num2 = month.Holydays * 24 + num2;
		double nonProjectTemperatureESM = calculationData.NonProjectTemperatureESM;
		return ((double)num * projectTemperatureESM + (double)num2 * nonProjectTemperatureESM) / (double)(num + num2);
	}

	private static double CalcAvgProjectTempCoolingRef1(Section section, double averageMontlyTemp, CalculationData calcData, MonthlyDays month)
	{
		int num = month.WorkDays * (section.CoolingSeasons.Cooling.WorkBaseEnd - section.CoolingSeasons.Cooling.WorkBaseStart);
		int num2 = month.Sundays * (section.CoolingSeasons.Cooling.SunBaseEnd - section.CoolingSeasons.Cooling.SunBaseStart);
		int num3 = month.Saturdays * (section.CoolingSeasons.Cooling.SatBaseEnd - section.CoolingSeasons.Cooling.SatBaseStart);
		return (calcData.ProjectTemperatureRef1 - averageMontlyTemp) * (double)(num + num3 + num2);
	}

	private static double CalcAvgProjectTempCoolingRef2(Section section, double averageMontlyTemp, CalculationData calcData, MonthlyDays month)
	{
		int num = month.WorkDays * (section.CoolingSeasons.Cooling.WorkBaseEnd - section.CoolingSeasons.Cooling.WorkBaseStart);
		int num2 = month.Sundays * (section.CoolingSeasons.Cooling.SunBaseEnd - section.CoolingSeasons.Cooling.SunBaseStart);
		int num3 = month.Saturdays * (section.CoolingSeasons.Cooling.SatBaseEnd - section.CoolingSeasons.Cooling.SatBaseStart);
		return (calcData.ProjectTemperatureRef2 - averageMontlyTemp) * (double)(num + num3 + num2);
	}

	private static double CalcAvgProjectTempCooling(Section section, double averageMontlyTemp, CalculationData calcData, MonthlyDays month)
	{
		int num = month.WorkDays * (section.CoolingSeasons.Cooling.WorkCurrentEnd - section.CoolingSeasons.Cooling.WorkCurrentStart);
		int num2 = month.Sundays * (section.CoolingSeasons.Cooling.SunCurrentEnd - section.CoolingSeasons.Cooling.SunCurrentStart);
		int num3 = month.Saturdays * (section.CoolingSeasons.Cooling.SatCurrentEnd - section.CoolingSeasons.Cooling.SatCurrentStart);
		return (calcData.ProjectTemperatureActual - averageMontlyTemp) * (double)(num + num3 + num2);
	}

	private static double CalcAvgProjectTempCoolingBaseLine(Section section, double averageMontlyTemp, CalculationData calcData, MonthlyDays month)
	{
		int num = month.WorkDays * (section.CoolingSeasons.Cooling.WorkBaseEnd - section.CoolingSeasons.Cooling.WorkBaseStart);
		int num2 = month.Sundays * (section.CoolingSeasons.Cooling.SunBaseEnd - section.CoolingSeasons.Cooling.SunBaseStart);
		int num3 = month.Saturdays * (section.CoolingSeasons.Cooling.SatBaseEnd - section.CoolingSeasons.Cooling.SatBaseStart);
		return (calcData.ProjectTemperatureBaseLine - averageMontlyTemp) * (double)(num + num3 + num2);
	}

	private static double CalcAvgProjectTempCoolingESM(Section section, double averageMontlyTemp, CalculationData calcData, MonthlyDays month)
	{
		int num = month.WorkDays * (section.CoolingSeasons.Cooling.WorkEsmEnd - section.CoolingSeasons.Cooling.WorkEsmStart);
		int num2 = month.Sundays * (section.CoolingSeasons.Cooling.SunEsmEnd - section.CoolingSeasons.Cooling.SunEsmStart);
		int num3 = month.Saturdays * (section.CoolingSeasons.Cooling.SatEsmEnd - section.CoolingSeasons.Cooling.SatEsmStart);
		return (calcData.ProjectTemperatureESM - averageMontlyTemp) * (double)(num + num3 + num2);
	}

	private static double CalcAvgNonProjectTempCoolingRef1(Section section, double averageMontlyTemp, CalculationData calculationData, MonthlyDays month)
	{
		int num = month.WorkDays * (24 - (section.CoolingSeasons.Cooling.WorkBaseEnd - section.CoolingSeasons.Cooling.WorkBaseStart));
		int num2 = month.Saturdays * (24 - (section.CoolingSeasons.Cooling.SatBaseEnd - section.CoolingSeasons.Cooling.SatBaseStart));
		int num3 = month.Sundays * (24 - (section.CoolingSeasons.Cooling.SunBaseEnd - section.CoolingSeasons.Cooling.SunBaseStart));
		int num4 = month.Holydays * 24;
		return (calculationData.NonProjectTemperatureRef1 - averageMontlyTemp) * (double)(num + num2 + num3 + num4);
	}

	private static double CalcAvgNonProjectTempCoolingRef2(Section section, double averageMontlyTemp, CalculationData calculationData, MonthlyDays month)
	{
		int num = month.WorkDays * (24 - (section.CoolingSeasons.Cooling.WorkBaseEnd - section.CoolingSeasons.Cooling.WorkBaseStart));
		int num2 = month.Saturdays * (24 - (section.CoolingSeasons.Cooling.SatBaseEnd - section.CoolingSeasons.Cooling.SatBaseStart));
		int num3 = month.Sundays * (24 - (section.CoolingSeasons.Cooling.SunBaseEnd - section.CoolingSeasons.Cooling.SunBaseStart));
		int num4 = month.Holydays * 24;
		return (calculationData.NonProjectTemperatureRef2 - averageMontlyTemp) * (double)(num + num2 + num3 + num4);
	}

	private static double CalcAvgNonProjectTempCooling(Section section, double averageMontlyTemp, CalculationData calculationData, MonthlyDays month)
	{
		int num = month.WorkDays * (24 - (section.CoolingSeasons.Cooling.WorkCurrentEnd - section.CoolingSeasons.Cooling.WorkCurrentStart));
		int num2 = month.Saturdays * (24 - (section.CoolingSeasons.Cooling.SatCurrentEnd - section.CoolingSeasons.Cooling.SatCurrentStart));
		int num3 = month.Sundays * (24 - (section.CoolingSeasons.Cooling.SunCurrentEnd - section.CoolingSeasons.Cooling.SunCurrentStart));
		int num4 = month.Holydays * 24;
		return (calculationData.NonProjectTemperatureActual - averageMontlyTemp) * (double)(num + num2 + num3 + num4);
	}

	private static double CalcAvgNonProjectTempCoolingBaseLine(Section section, double averageMontlyTemp, CalculationData calculationData, MonthlyDays month)
	{
		int num = month.WorkDays * (24 - (section.CoolingSeasons.Cooling.WorkBaseEnd - section.CoolingSeasons.Cooling.WorkBaseStart));
		int num2 = month.Saturdays * (24 - (section.CoolingSeasons.Cooling.SatBaseEnd - section.CoolingSeasons.Cooling.SatBaseStart));
		int num3 = month.Sundays * (24 - (section.CoolingSeasons.Cooling.SunBaseEnd - section.CoolingSeasons.Cooling.SunBaseStart));
		int num4 = month.Holydays * 24;
		return (calculationData.NonProjectTemperatureBaseLine - averageMontlyTemp) * (double)(num + num2 + num3 + num4);
	}

	private static double CalcAvgNonProjectTempCoolingESM(Section section, double averageMontlyTemp, CalculationData calculationData, MonthlyDays month)
	{
		int num = month.WorkDays * (24 - (section.CoolingSeasons.Cooling.WorkEsmEnd - section.CoolingSeasons.Cooling.WorkEsmStart));
		int num2 = month.Saturdays * (24 - (section.CoolingSeasons.Cooling.SatEsmEnd - section.CoolingSeasons.Cooling.SatEsmStart));
		int num3 = month.Sundays * (24 - (section.CoolingSeasons.Cooling.SunEsmEnd - section.CoolingSeasons.Cooling.SunEsmStart));
		int num4 = month.Holydays * 24;
		return (calculationData.NonProjectTemperatureESM - averageMontlyTemp) * (double)(num + num2 + num3 + num4);
	}

	private static double SumWallDirecrionsHu1Cooling(Section section, double averageMontlyTemp, double averageInnerCoolTemp)
	{
		double num = CalcWallDirectionParameterHu1Cooling(section.NorthWalls.Current, averageMontlyTemp, averageInnerCoolTemp);
		double num2 = CalcWallDirectionParameterHu1Cooling(section.NorthWalls.Current, averageMontlyTemp, averageInnerCoolTemp);
		double num3 = CalcWallDirectionParameterHu1Cooling(section.NorthWalls.Current, averageMontlyTemp, averageInnerCoolTemp);
		double num4 = CalcWallDirectionParameterHu1Cooling(section.NorthWalls.Current, averageMontlyTemp, averageInnerCoolTemp);
		double num5 = CalcWallDirectionParameterHu1Cooling(section.NorthWalls.Current, averageMontlyTemp, averageInnerCoolTemp);
		double num6 = CalcWallDirectionParameterHu1Cooling(section.NorthWalls.Current, averageMontlyTemp, averageInnerCoolTemp);
		double num7 = CalcWallDirectionParameterHu1Cooling(section.NorthWalls.Current, averageMontlyTemp, averageInnerCoolTemp);
		double num8 = CalcWallDirectionParameterHu1Cooling(section.NorthWalls.Current, averageMontlyTemp, averageInnerCoolTemp);
		return num + num2 + num3 + num4 + num5 + num6 + num7 + num8;
	}

	private static double SumWallDirecrionsHu1CoolingESM(Section section, double averageMontlyTemp, double averageInnerCoolTemp)
	{
		double num = CalcWallDirectionParameterHu1Cooling(section.NorthWalls.Esm, averageMontlyTemp, averageInnerCoolTemp);
		double num2 = CalcWallDirectionParameterHu1Cooling(section.NorthWalls.Esm, averageMontlyTemp, averageInnerCoolTemp);
		double num3 = CalcWallDirectionParameterHu1Cooling(section.NorthWalls.Esm, averageMontlyTemp, averageInnerCoolTemp);
		double num4 = CalcWallDirectionParameterHu1Cooling(section.NorthWalls.Esm, averageMontlyTemp, averageInnerCoolTemp);
		double num5 = CalcWallDirectionParameterHu1Cooling(section.NorthWalls.Esm, averageMontlyTemp, averageInnerCoolTemp);
		double num6 = CalcWallDirectionParameterHu1Cooling(section.NorthWalls.Esm, averageMontlyTemp, averageInnerCoolTemp);
		double num7 = CalcWallDirectionParameterHu1Cooling(section.NorthWalls.Esm, averageMontlyTemp, averageInnerCoolTemp);
		double num8 = CalcWallDirectionParameterHu1Cooling(section.NorthWalls.Esm, averageMontlyTemp, averageInnerCoolTemp);
		return num + num2 + num3 + num4 + num5 + num6 + num7 + num8;
	}

	private static double CalcWallDirectionParameterHu1Cooling(Walls wall, double averageMontlyTemp, double averageInnerCoolTemp)
	{
		double num = averageInnerCoolTemp - averageMontlyTemp;
		if (object.Equals(num, 0.0))
		{
			return 0.0;
		}
		double innerA = wall.InnerA1;
		double innerU = wall.InnerU1;
		double num2 = averageInnerCoolTemp - (double)wall.InnerS1;
		double num3 = innerA * innerU * num2 / num;
		innerA = wall.InnerA2;
		innerU = wall.InnerU2;
		num2 = averageInnerCoolTemp - (double)wall.InnerS2;
		double num4 = innerA * innerU * num2 / num;
		innerA = wall.InnerA3;
		innerU = wall.InnerU3;
		num2 = averageInnerCoolTemp - (double)wall.InnerS3;
		double num5 = innerA * innerU * num2 / num;
		innerA = wall.InnerA4;
		innerU = wall.InnerU4;
		num2 = averageInnerCoolTemp - (double)wall.InnerS4;
		double num6 = innerA * innerU * num2 / num;
		innerA = wall.IneerA5;
		innerU = wall.IneerA5;
		num2 = averageInnerCoolTemp - (double)wall.InnerS5;
		double num7 = innerA * innerU * num2 / num;
		innerA = wall.InnerA6;
		innerU = wall.InnerU6;
		num2 = averageInnerCoolTemp - (double)wall.InnerS6;
		double num8 = innerA * innerU * num2 / num;
		return num3 + num4 + num5 + num6 + num7 + num8;
	}

	private static double CalcCeilingsParameterHu2Cooling(Roof roof, double averageMontlyTemp, double averageInnerCoolTemp)
	{
		double num = averageInnerCoolTemp - averageMontlyTemp;
		if (object.Equals(num, 0.0))
		{
			return 0.0;
		}
		double ceilingA = roof.CeilingA1;
		double ceilingU = roof.CeilingU1;
		double num2 = averageInnerCoolTemp - (double)roof.CeilingS1;
		double num3 = ceilingA * ceilingU * num2 / num;
		ceilingA = roof.CeilingA2;
		ceilingU = roof.CeilingU2;
		num2 = averageInnerCoolTemp - (double)roof.CeilingS2;
		double num4 = ceilingA * ceilingU * num2 / num;
		ceilingA = roof.CeilingA3;
		ceilingU = roof.CeilingU3;
		num2 = averageInnerCoolTemp - (double)roof.CeilingS3;
		double num5 = ceilingA * ceilingU * num2 / num;
		ceilingA = roof.CeilingA4;
		ceilingU = roof.CeilingU4;
		num2 = averageInnerCoolTemp - (double)roof.CeilingS4;
		double num6 = ceilingA * ceilingU * num2 / num;
		ceilingA = roof.CeilingA5;
		ceilingU = roof.CeilingA5;
		num2 = averageInnerCoolTemp - (double)roof.CeilingS5;
		double num7 = ceilingA * ceilingU * num2 / num;
		ceilingA = roof.CeilingA6;
		ceilingU = roof.CeilingU6;
		num2 = averageInnerCoolTemp - (double)roof.CeilingS6;
		double num8 = ceilingA * ceilingU * num2 / num;
		return num3 + num4 + num5 + num6 + num7 + num8;
	}

	private static double CalcFloorsParameterHu3Cooling(Floor floor, double averageMontlyTemp, double averageInnerCoolTemp)
	{
		double num = averageInnerCoolTemp - averageMontlyTemp;
		if (object.Equals(num, 0.0))
		{
			return 0.0;
		}
		double otherFloorA = floor.OtherFloorA1;
		double otherFloorU = floor.OtherFloorU1;
		double num2 = averageInnerCoolTemp - (double)floor.OtherFloorS1;
		double num3 = otherFloorA * otherFloorU * num2 / num;
		otherFloorA = floor.OtherFloorA2;
		otherFloorU = floor.OtherFloorU2;
		num2 = averageInnerCoolTemp - (double)floor.OtherFloorS2;
		double num4 = otherFloorA * otherFloorU * num2 / num;
		otherFloorA = floor.OtherFloorA3;
		otherFloorU = floor.OtherFloorU3;
		num2 = averageInnerCoolTemp - (double)floor.OtherFloorS3;
		double num5 = otherFloorA * otherFloorU * num2 / num;
		otherFloorA = floor.OtherFloorA4;
		otherFloorU = floor.OtherFloorU4;
		num2 = averageInnerCoolTemp - (double)floor.OtherFloorS4;
		double num6 = otherFloorA * otherFloorU * num2 / num;
		otherFloorA = floor.OtherFloorA5;
		otherFloorU = floor.OtherFloorU5;
		num2 = averageInnerCoolTemp - (double)floor.OtherFloorS5;
		double num7 = otherFloorA * otherFloorU * num2 / num;
		otherFloorA = floor.OtherFloorA6;
		otherFloorU = floor.OtherFloorU6;
		num2 = averageInnerCoolTemp - (double)floor.OtherFloorS4;
		double num8 = otherFloorA * otherFloorU * num2 / num;
		return num3 + num4 + num5 + num6 + num7 + num8;
	}

	public static void CalculateFansAndPumpsHeatingRef1(this HeatingCalculations calc, Section section)
	{
		List<MonthlyDays> source = section.CalcPeriod((int)section.HeatingSeason.FirstMonthHeat, (int)section.HeatingSeason.LastMonthHeat, section.HeatingSeason.FirstDayHeat, section.HeatingSeason.LastDayHeat);
		double num = source.Sum((MonthlyDays month) => month.Weeks);
		double weekHeatingVentilationHoursBaseLine = GetWeekHeatingVentilationHoursBaseLine(section);
		double num2 = calc.FansAndPumps.VentilatorsHeatRef1 * weekHeatingVentilationHoursBaseLine * num / 1000.0;
		num2 += calc.FansAndPumps.PumpVentilationRef1 * weekHeatingVentilationHoursBaseLine * num / 1000.0;
		num2 += calc.FansAndPumps.PumpHeatingRef1 * 24.0 * 7.0 * num / 1000.0;
		num2 = num2 / calc.FansAndPumps.EnergyManagementRef1 * 100.0;
		if (double.IsInfinity(num2) || double.IsNaN(num2))
		{
			num2 = 0.0;
		}
		calc.FansAndPumps.PumpNeededEnergyRef1 = num2;
	}

	public static void CalculateFansAndPumpsHeatingRef2(this HeatingCalculations calc, Section section)
	{
		List<MonthlyDays> source = section.CalcPeriod((int)section.HeatingSeason.FirstMonthHeat, (int)section.HeatingSeason.LastMonthHeat, section.HeatingSeason.FirstDayHeat, section.HeatingSeason.LastDayHeat);
		double num = source.Sum((MonthlyDays month) => month.Weeks);
		double weekHeatingVentilationHoursBaseLine = GetWeekHeatingVentilationHoursBaseLine(section);
		double num2 = calc.FansAndPumps.VentilatorsHeatRef2 * weekHeatingVentilationHoursBaseLine * num / 1000.0;
		num2 += calc.FansAndPumps.PumpVentilationRef2 * weekHeatingVentilationHoursBaseLine * num / 1000.0;
		num2 += calc.FansAndPumps.PumpHeatingRef2 * 24.0 * 7.0 * num / 1000.0;
		num2 = num2 / calc.FansAndPumps.EnergyManagementRef2 * 100.0;
		if (double.IsInfinity(num2) || double.IsNaN(num2))
		{
			num2 = 0.0;
		}
		calc.FansAndPumps.PumpNeededEnergyRef2 = num2;
	}

	public static void CalculateFansAndPumpsHeatingActual(this HeatingCalculations calc, Section section)
	{
		List<MonthlyDays> source = section.CalcPeriod((int)section.HeatingSeason.FirstMonthHeat, (int)section.HeatingSeason.LastMonthHeat, section.HeatingSeason.FirstDayHeat, section.HeatingSeason.LastDayHeat);
		double num = source.Sum((MonthlyDays month) => month.Weeks);
		double weekHeatingVentilationHoursActual = GetWeekHeatingVentilationHoursActual(section);
		double num2 = calc.FansAndPumps.VentilatorsHeatActual * weekHeatingVentilationHoursActual * num / 1000.0;
		num2 += calc.FansAndPumps.PumpVentilationActual * weekHeatingVentilationHoursActual * num / 1000.0;
		num2 += calc.FansAndPumps.PumpHeatingActual * 24.0 * 7.0 * num / 1000.0;
		num2 = num2 / calc.FansAndPumps.EnergyManagementActual * 100.0;
		if (double.IsInfinity(num2) || double.IsNaN(num2))
		{
			num2 = 0.0;
		}
		calc.FansAndPumps.PumpNeededEnergyActual = num2;
	}

	public static void CalculateFansAndPumpsHeatingBaseLine(this HeatingCalculations calc, Section section)
	{
		List<MonthlyDays> source = section.CalcPeriod((int)section.HeatingSeason.FirstMonthHeat, (int)section.HeatingSeason.LastMonthHeat, section.HeatingSeason.FirstDayHeat, section.HeatingSeason.LastDayHeat);
		double num = source.Sum((MonthlyDays month) => month.Weeks);
		double weekHeatingVentilationHoursBaseLine = GetWeekHeatingVentilationHoursBaseLine(section);
		double num2 = calc.FansAndPumps.VentilatorsHeatBaseLine * weekHeatingVentilationHoursBaseLine * num / 1000.0;
		num2 += calc.FansAndPumps.PumpVentilationBaseLine * weekHeatingVentilationHoursBaseLine * num / 1000.0;
		weekHeatingVentilationHoursBaseLine = GetWeekHeatingSeasonHoursBaseLine(section);
		num2 += calc.FansAndPumps.PumpHeatingBaseLine * 24.0 * 7.0 * num / 1000.0;
		num2 = num2 / calc.FansAndPumps.EnergyManagementBaseLine * 100.0;
		if (double.IsInfinity(num2) || double.IsNaN(num2))
		{
			num2 = 0.0;
		}
		calc.FansAndPumps.PumpNeededEnergyBaseLine = num2;
	}

	public static void CalculateFansAndPumpsHeatingEsm(this HeatingCalculations calc, Section section)
	{
		List<MonthlyDays> source = section.CalcPeriod((int)section.HeatingSeason.FirstMonthHeat, (int)section.HeatingSeason.LastMonthHeat, section.HeatingSeason.FirstDayHeat, section.HeatingSeason.LastDayHeat);
		double num = source.Sum((MonthlyDays month) => month.Weeks);
		double weekHeatingVentilationHoursEsm = GetWeekHeatingVentilationHoursEsm(section);
		double num2 = calc.FansAndPumps.VentilatorsHeatESM * weekHeatingVentilationHoursEsm * num / 1000.0;
		num2 += calc.FansAndPumps.PumpVentilationESM * weekHeatingVentilationHoursEsm * num / 1000.0;
		weekHeatingVentilationHoursEsm = GetWeekHeatingSeasonHoursEsm(section);
		num2 += calc.FansAndPumps.PumpHeatingESM * 24.0 * 7.0 * num / 1000.0;
		num2 = num2 / calc.FansAndPumps.EnergyManagementESM * 100.0;
		if (double.IsInfinity(num2) || double.IsNaN(num2))
		{
			num2 = 0.0;
		}
		calc.FansAndPumps.PumpNeededEnergyESM = num2;
		calc.FansAndPumps.PumpNeededEnergySavings = (calc.FansAndPumps.PumpNeededEnergyBaseLine - calc.FansAndPumps.PumpNeededEnergyESM).ToString("F3");
	}

	public static void CalculateFansAndPumpsCoolingRef1(this HeatingCalculations calc, Section section)
	{
		List<MonthlyDays> source = section.CalcPeriod((int)section.CoolingSeason.FirstMonthCool, (int)section.CoolingSeason.LastMonthCool, section.CoolingSeason.FirstDayCool, section.CoolingSeason.LastDayCool);
		double num = source.Sum((MonthlyDays month) => month.Weeks);
		double weekCoolingVentilationHoursBaseLine = GetWeekCoolingVentilationHoursBaseLine(section);
		double num2 = calc.FansAndPumps.VentilatorsCoolRef1 * weekCoolingVentilationHoursBaseLine * num / 1000.0;
		num2 += calc.FansAndPumps.PumpVentilationCoolRef1 * weekCoolingVentilationHoursBaseLine * num / 1000.0;
		num2 += calc.FansAndPumps.VentilatorsOutdoorAirCoolRef1 * weekCoolingVentilationHoursBaseLine * num / 1000.0;
		weekCoolingVentilationHoursBaseLine = GetWeekCoolingSeasonHoursBaseLine(section);
		num2 += calc.FansAndPumps.CoolingPumpRef1 * 24.0 * 7.0 * num / 1000.0;
		num2 = num2 / calc.FansAndPumps.EnergyManagement2Ref2 * 100.0;
		if (double.IsInfinity(num2) || double.IsNaN(num2))
		{
			num2 = 0.0;
		}
		calc.FansAndPumps.CoolNeededEnergyRef1 = num2;
		weekCoolingVentilationHoursBaseLine = GetWeekCoolingVentilationHoursBaseLine(section);
		double num3 = calc.FansAndPumps.OtherCoolingVentilationRef1 * weekCoolingVentilationHoursBaseLine * num / 1000.0;
		weekCoolingVentilationHoursBaseLine = GetWeekCoolingSeasonHoursBaseLine(section);
		num3 += calc.FansAndPumps.OtherCoolingRef1 * weekCoolingVentilationHoursBaseLine * num / 1000.0;
		if (double.IsInfinity(num3) || double.IsNaN(num3))
		{
			num3 = 0.0;
		}
		calc.FansAndPumps.OtherResultCoolingRef1 = num3;
	}

	public static void CalculateFansAndPumpsCoolingRef2(this HeatingCalculations calc, Section section)
	{
		List<MonthlyDays> source = section.CalcPeriod((int)section.CoolingSeason.FirstMonthCool, (int)section.CoolingSeason.LastMonthCool, section.CoolingSeason.FirstDayCool, section.CoolingSeason.LastDayCool);
		double num = source.Sum((MonthlyDays month) => month.Weeks);
		double weekCoolingVentilationHoursBaseLine = GetWeekCoolingVentilationHoursBaseLine(section);
		double num2 = calc.FansAndPumps.VentilatorsCoolRef2 * weekCoolingVentilationHoursBaseLine * num / 1000.0;
		num2 += calc.FansAndPumps.PumpVentilationCoolRef2 * weekCoolingVentilationHoursBaseLine * num / 1000.0;
		num2 += calc.FansAndPumps.VentilatorsOutdoorAirCoolRef2 * weekCoolingVentilationHoursBaseLine * num / 1000.0;
		num2 += calc.FansAndPumps.CoolingPumpRef2 * 24.0 * 7.0 * num / 1000.0;
		num2 = num2 / calc.FansAndPumps.EnergyManagement2Ref2 * 100.0;
		if (double.IsInfinity(num2) || double.IsNaN(num2))
		{
			num2 = 0.0;
		}
		calc.FansAndPumps.CoolNeededEnergyRef2 = num2;
		weekCoolingVentilationHoursBaseLine = GetWeekCoolingVentilationHoursBaseLine(section);
		double num3 = calc.FansAndPumps.OtherCoolingVentilationRef2 * weekCoolingVentilationHoursBaseLine * num / 1000.0;
		weekCoolingVentilationHoursBaseLine = GetWeekCoolingSeasonHoursBaseLine(section);
		num3 += calc.FansAndPumps.OtherCoolingRef2 * weekCoolingVentilationHoursBaseLine * num / 1000.0;
		if (double.IsInfinity(num3) || double.IsNaN(num3))
		{
			num3 = 0.0;
		}
		calc.FansAndPumps.OtherResultCoolingRef2 = num3;
	}

	public static void CalculateFansAndPumpsCoolingActual(this HeatingCalculations calc, Section section)
	{
		List<MonthlyDays> source = section.CalcPeriod((int)section.CoolingSeason.FirstMonthCool, (int)section.CoolingSeason.LastMonthCool, section.CoolingSeason.FirstDayCool, section.CoolingSeason.LastDayCool);
		double num = source.Sum((MonthlyDays month) => month.Weeks);
		double weekCoolingVentilationHoursActual = GetWeekCoolingVentilationHoursActual(section);
		double num2 = calc.FansAndPumps.VentilatorsCoolActual * weekCoolingVentilationHoursActual * num / 1000.0;
		num2 += calc.FansAndPumps.PumpVentilationCoolActual * weekCoolingVentilationHoursActual * num / 1000.0;
		num2 += calc.FansAndPumps.VentilatorsOutdoorAirCoolActual * weekCoolingVentilationHoursActual * num / 1000.0;
		num2 += calc.FansAndPumps.CoolingPumpActual * 24.0 * 7.0 * num / 1000.0;
		num2 = num2 / calc.FansAndPumps.EnergyManagement2Actual * 100.0;
		if (double.IsInfinity(num2) || double.IsNaN(num2))
		{
			num2 = 0.0;
		}
		calc.FansAndPumps.CoolNeededEnergyActual = num2;
		weekCoolingVentilationHoursActual = GetWeekCoolingVentilationHoursActual(section);
		double num3 = calc.FansAndPumps.OtherCoolingVentilationActual * weekCoolingVentilationHoursActual * num / 1000.0;
		weekCoolingVentilationHoursActual = GetWeekCoolingSeasonHoursActual(section);
		num3 += calc.FansAndPumps.OtherCoolingActual * weekCoolingVentilationHoursActual * num / 1000.0;
		if (double.IsInfinity(num3) || double.IsNaN(num3))
		{
			num3 = 0.0;
		}
		calc.FansAndPumps.OtherResultCoolingActual = num3;
	}

	public static void CalculateFansAndPumpsCoolingBaseLine(this HeatingCalculations calc, Section section)
	{
		List<MonthlyDays> source = section.CalcPeriod((int)section.CoolingSeason.FirstMonthCool, (int)section.CoolingSeason.LastMonthCool, section.CoolingSeason.FirstDayCool, section.CoolingSeason.LastDayCool);
		double num = source.Sum((MonthlyDays month) => month.Weeks);
		double weekCoolingVentilationHoursBaseLine = GetWeekCoolingVentilationHoursBaseLine(section);
		double num2 = calc.FansAndPumps.VentilatorsCoolBaseLine * weekCoolingVentilationHoursBaseLine * num / 1000.0;
		num2 += calc.FansAndPumps.PumpVentilationCoolBaseLine * weekCoolingVentilationHoursBaseLine * num / 1000.0;
		num2 += calc.FansAndPumps.VentilatorsOutdoorAirCoolBaseLine * weekCoolingVentilationHoursBaseLine * num / 1000.0;
		num2 += calc.FansAndPumps.CoolingPumpBaseLine * 24.0 * 7.0 * num / 1000.0;
		num2 = num2 / calc.FansAndPumps.EnergyManagement2BaseLine * 100.0;
		if (double.IsInfinity(num2) || double.IsNaN(num2))
		{
			num2 = 0.0;
		}
		calc.FansAndPumps.CoolNeededEnergyBaseLine = num2;
		weekCoolingVentilationHoursBaseLine = GetWeekCoolingVentilationHoursBaseLine(section);
		double num3 = calc.FansAndPumps.OtherCoolingVentilationBaseLine * weekCoolingVentilationHoursBaseLine * num / 1000.0;
		weekCoolingVentilationHoursBaseLine = GetWeekCoolingSeasonHoursBaseLine(section);
		num3 += calc.FansAndPumps.OtherCoolingBaseLine * weekCoolingVentilationHoursBaseLine * num / 1000.0;
		if (double.IsInfinity(num3) || double.IsNaN(num3))
		{
			num3 = 0.0;
		}
		calc.FansAndPumps.OtherResultCoolingBaseLine = num3;
	}

	public static void CalculateFansAndPumpsCoolingEsm(this HeatingCalculations calc, Section section)
	{
		List<MonthlyDays> source = section.CalcPeriod((int)section.CoolingSeason.FirstMonthCool, (int)section.CoolingSeason.LastMonthCool, section.CoolingSeason.FirstDayCool, section.CoolingSeason.LastDayCool);
		double num = source.Sum((MonthlyDays month) => month.Weeks);
		double weekCoolingVentilationHoursEsm = GetWeekCoolingVentilationHoursEsm(section);
		double num2 = calc.FansAndPumps.VentilatorsCoolESM * weekCoolingVentilationHoursEsm * num / 1000.0;
		num2 += calc.FansAndPumps.PumpVentilationCoolESM * weekCoolingVentilationHoursEsm * num / 1000.0;
		num2 += calc.FansAndPumps.VentilatorsOutdoorAirCoolESM * weekCoolingVentilationHoursEsm * num / 1000.0;
		num2 += calc.FansAndPumps.CoolingPumpESM * 24.0 * 7.0 * num / 1000.0;
		num2 = num2 / calc.FansAndPumps.EnergyManagement2ESM * 100.0;
		if (double.IsInfinity(num2) || double.IsNaN(num2))
		{
			num2 = 0.0;
		}
		calc.FansAndPumps.CoolNeededEnergyESM = num2;
		weekCoolingVentilationHoursEsm = GetWeekCoolingVentilationHoursEsm(section);
		double num3 = calc.FansAndPumps.OtherCoolingVentilationESM * weekCoolingVentilationHoursEsm * num / 1000.0;
		weekCoolingVentilationHoursEsm = GetWeekCoolingSeasonHoursEsm(section);
		num3 += calc.FansAndPumps.OtherCoolingESM * weekCoolingVentilationHoursEsm * num / 1000.0;
		if (double.IsInfinity(num3) || double.IsNaN(num3))
		{
			num3 = 0.0;
		}
		calc.FansAndPumps.OtherResultCoolingESM = num3;
		calc.FansAndPumps.OtherResultCoolingSavings = (calc.FansAndPumps.OtherResultCoolingBaseLine - calc.FansAndPumps.OtherResultCoolingESM).ToString("F3");
	}

	private static double GetWeekHeatingVentilationHoursActual(Section section)
	{
		double num = 5.0 * section.CalcHours(section.HeatingSeasons.Ventilation.WorkCurrentStart, section.HeatingSeasons.Ventilation.WorkCurrentEnd);
		num += section.CalcHours(section.HeatingSeasons.Ventilation.SunCurrentStart, section.HeatingSeasons.Ventilation.SunCurrentEnd);
		return num + section.CalcHours(section.HeatingSeasons.Ventilation.SatCurrentStart, section.HeatingSeasons.Ventilation.SatCurrentEnd);
	}

	private static double GetWeekHeatingVentilationHoursBaseLine(Section section)
	{
		double num = 5.0 * section.CalcHours(section.HeatingSeasons.Ventilation.WorkBaseStart, section.HeatingSeasons.Ventilation.WorkBaseEnd);
		num += section.CalcHours(section.HeatingSeasons.Ventilation.SunBaseStart, section.HeatingSeasons.Ventilation.SunBaseEnd);
		return num + section.CalcHours(section.HeatingSeasons.Ventilation.SatBaseStart, section.HeatingSeasons.Ventilation.SatBaseEnd);
	}

	private static double GetWeekHeatingVentilationHoursEsm(Section section)
	{
		double num = 5.0 * section.CalcHours(section.HeatingSeasons.Ventilation.WorkEsmStart, section.HeatingSeasons.Ventilation.WorkEsmEnd);
		num += section.CalcHours(section.HeatingSeasons.Ventilation.SunEsmStart, section.HeatingSeasons.Ventilation.SunEsmEnd);
		return num + section.CalcHours(section.HeatingSeasons.Ventilation.SatEsmStart, section.HeatingSeasons.Ventilation.SatEsmEnd);
	}

	private static double GetWeekHeatingSeasonHoursActual(Section section)
	{
		double num = 5.0 * section.CalcHours(section.HeatingSeasons.Heating.WorkCurrentStart, section.HeatingSeasons.Heating.WorkCurrentEnd);
		num += section.CalcHours(section.HeatingSeasons.Heating.SunCurrentStart, section.HeatingSeasons.Heating.SunCurrentEnd);
		return num + section.CalcHours(section.HeatingSeasons.Heating.SatCurrentStart, section.HeatingSeasons.Heating.SatCurrentEnd);
	}

	private static double GetWeekHeatingSeasonHoursBaseLine(Section section)
	{
		double num = 5.0 * section.CalcHours(section.HeatingSeasons.Heating.WorkBaseStart, section.HeatingSeasons.Heating.WorkBaseEnd);
		num += section.CalcHours(section.HeatingSeasons.Heating.SunBaseStart, section.HeatingSeasons.Heating.SunBaseEnd);
		return num + section.CalcHours(section.HeatingSeasons.Heating.SatBaseStart, section.HeatingSeasons.Heating.SatBaseEnd);
	}

	private static double GetWeekHeatingSeasonHoursEsm(Section section)
	{
		double num = 5.0 * section.CalcHours(section.HeatingSeasons.Heating.WorkEsmStart, section.HeatingSeasons.Heating.WorkEsmEnd);
		num += section.CalcHours(section.HeatingSeasons.Heating.SunEsmStart, section.HeatingSeasons.Heating.SunEsmEnd);
		return num + section.CalcHours(section.HeatingSeasons.Heating.SatEsmStart, section.HeatingSeasons.Heating.SatEsmEnd);
	}

	private static double GetWeekCoolingSeasonHoursActual(Section section)
	{
		double num = 5.0 * section.CalcHours(section.CoolingSeasons.Cooling.WorkCurrentStart, section.CoolingSeasons.Cooling.WorkCurrentEnd);
		num += section.CalcHours(section.CoolingSeasons.Cooling.SunCurrentStart, section.CoolingSeasons.Cooling.SunCurrentEnd);
		return num + section.CalcHours(section.CoolingSeasons.Cooling.SatCurrentStart, section.CoolingSeasons.Cooling.SatCurrentEnd);
	}

	private static double GetWeekCoolingSeasonHoursBaseLine(Section section)
	{
		double num = 5.0 * section.CalcHours(section.CoolingSeasons.Cooling.WorkBaseStart, section.CoolingSeasons.Cooling.WorkBaseEnd);
		num += section.CalcHours(section.CoolingSeasons.Cooling.SunBaseStart, section.CoolingSeasons.Cooling.SunBaseEnd);
		return num + section.CalcHours(section.CoolingSeasons.Cooling.SatBaseStart, section.CoolingSeasons.Cooling.SatBaseEnd);
	}

	private static double GetWeekCoolingSeasonHoursEsm(Section section)
	{
		double num = 5.0 * section.CalcHours(section.CoolingSeasons.Cooling.WorkEsmStart, section.CoolingSeasons.Cooling.WorkEsmEnd);
		num += section.CalcHours(section.CoolingSeasons.Cooling.SunEsmStart, section.CoolingSeasons.Cooling.SunEsmEnd);
		return num + section.CalcHours(section.CoolingSeasons.Cooling.SatEsmStart, section.CoolingSeasons.Cooling.SatEsmEnd);
	}

	private static double GetWeekCoolingVentilationHoursActual(Section section)
	{
		double num = 5.0 * section.CalcHours(section.CoolingSeasons.Ventilation.WorkCurrentStart, section.CoolingSeasons.Ventilation.WorkCurrentEnd);
		num += section.CalcHours(section.CoolingSeasons.Ventilation.SunCurrentStart, section.CoolingSeasons.Ventilation.SunCurrentEnd);
		return num + section.CalcHours(section.CoolingSeasons.Ventilation.SatCurrentStart, section.CoolingSeasons.Ventilation.SatCurrentEnd);
	}

	private static double GetWeekCoolingVentilationHoursBaseLine(Section section)
	{
		double num = 5.0 * section.CalcHours(section.CoolingSeasons.Ventilation.WorkBaseStart, section.CoolingSeasons.Ventilation.WorkBaseEnd);
		num += section.CalcHours(section.CoolingSeasons.Ventilation.SunBaseStart, section.CoolingSeasons.Ventilation.SunBaseEnd);
		return num + section.CalcHours(section.CoolingSeasons.Ventilation.SatBaseStart, section.CoolingSeasons.Ventilation.SatBaseEnd);
	}

	private static double GetWeekCoolingVentilationHoursEsm(Section section)
	{
		double num = 5.0 * section.CalcHours(section.CoolingSeasons.Ventilation.WorkEsmStart, section.CoolingSeasons.Ventilation.WorkEsmEnd);
		num += section.CalcHours(section.CoolingSeasons.Ventilation.SunEsmStart, section.CoolingSeasons.Ventilation.SunEsmEnd);
		return num + section.CalcHours(section.CoolingSeasons.Ventilation.SatEsmStart, section.CoolingSeasons.Ventilation.SatEsmEnd);
	}

	public static void CalculateUouterWallsCurrent(this CalculationData heatingAndCoolingCalculations, Section section)
	{
		northU = section.NorthWalls.Current.AccumulateOuterU;
		northEastU = section.NorthEastWalls.Current.AccumulateOuterU;
		eastU = section.EastWalls.Current.AccumulateOuterU;
		southEastU = section.SouthEastWalls.Current.AccumulateOuterU;
		southU = section.SouthWalls.Current.AccumulateOuterU;
		southWestU = section.SouthWestWalls.Current.AccumulateOuterU;
		westU = section.WestWalls.Current.AccumulateOuterU;
		northWestU = section.NorthWestWalls.Current.AccumulateOuterU;
		areaNorth = section.NorthWalls.Current.AccumulateOuterA;
		areaNorthEast = section.NorthEastWalls.Current.AccumulateOuterA;
		areaEast = section.EastWalls.Current.AccumulateOuterA;
		areaSouthEast = section.SouthEastWalls.Current.AccumulateOuterA;
		areaSouth = section.SouthWalls.Current.AccumulateOuterA;
		areaSouthWest = section.SouthWestWalls.Current.AccumulateOuterA;
		areaWest = section.WestWalls.Current.AccumulateOuterA;
		areaNorthWest = section.NorthWestWalls.Current.AccumulateOuterA;
		double num = northU * areaNorth + northEastU * areaNorthEast + eastU * areaEast + southEastU * areaSouthEast + southU * areaSouth + southWestU * areaSouthWest + westU * areaWest + northWestU * areaNorthWest + section.NorthWalls.Current.AccumulateOuterL + section.NorthWalls.Current.AccumulateOuterX + section.NorthEastWalls.Current.AccumulateOuterL + section.NorthEastWalls.Current.AccumulateOuterX + section.EastWalls.Current.AccumulateOuterL + section.EastWalls.Current.AccumulateOuterX + section.SouthEastWalls.Current.AccumulateOuterL + section.SouthEastWalls.Current.AccumulateOuterX + section.SouthWalls.Current.AccumulateOuterL + section.SouthWalls.Current.AccumulateOuterX + section.SouthWestWalls.Current.AccumulateOuterL + section.SouthWestWalls.Current.AccumulateOuterX + section.WestWalls.Current.AccumulateOuterL + section.WestWalls.Current.AccumulateOuterX + section.NorthWestWalls.Current.AccumulateOuterL + section.NorthWestWalls.Current.AccumulateOuterX;
		double num2 = areaNorth + areaNorthEast + areaEast + areaSouthEast + areaSouth + areaSouthWest + areaWest + areaNorthWest;
		double num3 = num / num2;
		if (double.IsNaN(num3) || double.IsInfinity(num3))
		{
			num3 = 0.0;
		}
		heatingAndCoolingCalculations.UouterWallsActual = num3;
		heatingAndCoolingCalculations.UouterWallsBaseLine = num3;
	}

	public static void CalculateUouterWallsEsm(this CalculationData heatingAndCoolingCalculations, Section section)
	{
		northU = section.NorthWalls.Esm.AccumulateOuterU;
		northEastU = section.NorthEastWalls.Esm.AccumulateOuterU;
		eastU = section.EastWalls.Esm.AccumulateOuterU;
		southEastU = section.SouthEastWalls.Esm.AccumulateOuterU;
		southU = section.SouthWalls.Esm.AccumulateOuterU;
		southWestU = section.SouthWestWalls.Esm.AccumulateOuterU;
		westU = section.WestWalls.Esm.AccumulateOuterU;
		northWestU = section.NorthWestWalls.Esm.AccumulateOuterU;
		areaNorth = section.NorthWalls.Esm.AccumulateOuterA;
		areaNorthEast = section.NorthEastWalls.Esm.AccumulateOuterA;
		areaEast = section.EastWalls.Esm.AccumulateOuterA;
		areaSouthEast = section.SouthEastWalls.Esm.AccumulateOuterA;
		areaSouth = section.SouthWalls.Esm.AccumulateOuterA;
		areaSouthWest = section.SouthWestWalls.Esm.AccumulateOuterA;
		areaWest = section.WestWalls.Esm.AccumulateOuterA;
		areaNorthWest = section.NorthWestWalls.Esm.AccumulateOuterA;
		double num = northU * areaNorth + northEastU * areaNorthEast + eastU * areaEast + southEastU * areaSouthEast + southU * areaSouth + southWestU * areaSouthWest + westU * areaWest + northWestU * areaNorthWest + section.NorthWalls.Esm.AccumulateOuterL + section.NorthWalls.Esm.AccumulateOuterX + section.NorthEastWalls.Esm.AccumulateOuterL + section.NorthEastWalls.Esm.AccumulateOuterX + section.EastWalls.Esm.AccumulateOuterL + section.EastWalls.Esm.AccumulateOuterX + section.SouthEastWalls.Esm.AccumulateOuterL + section.SouthEastWalls.Esm.AccumulateOuterX + section.SouthWalls.Esm.AccumulateOuterL + section.SouthWalls.Esm.AccumulateOuterX + section.SouthWestWalls.Esm.AccumulateOuterL + section.SouthWestWalls.Esm.AccumulateOuterX + section.WestWalls.Esm.AccumulateOuterL + section.WestWalls.Esm.AccumulateOuterX + section.NorthWestWalls.Esm.AccumulateOuterL + section.NorthWestWalls.Esm.AccumulateOuterX;
		double num2 = areaNorth + areaNorthEast + areaEast + areaSouthEast + areaSouth + areaSouthWest + areaWest + areaNorthWest;
		double num3 = num / num2;
		if (double.IsNaN(num3) || double.IsInfinity(num3))
		{
			num3 = 0.0;
		}
		heatingAndCoolingCalculations.UouterWallsESM = num3;
	}

	public static void CalculateUinnerWallsCurrent(this CalculationData heatingAndCoolingCalculations, Section section)
	{
		northU = section.NorthWalls.Current.AccumulateInnerU;
		northEastU = section.NorthEastWalls.Current.AccumulateInnerU;
		eastU = section.EastWalls.Current.AccumulateInnerU;
		southEastU = section.SouthEastWalls.Current.AccumulateInnerU;
		southU = section.SouthWalls.Current.AccumulateInnerU;
		southWestU = section.SouthWestWalls.Current.AccumulateInnerU;
		westU = section.WestWalls.Current.AccumulateInnerU;
		northWestU = section.NorthWestWalls.Current.AccumulateInnerU;
		areaNorth = section.NorthWalls.Current.AccumulateInnerA;
		areaNorthEast = section.NorthEastWalls.Current.AccumulateInnerA;
		areaEast = section.EastWalls.Current.AccumulateInnerA;
		areaSouthEast = section.SouthEastWalls.Current.AccumulateInnerA;
		areaSouth = section.SouthWalls.Current.AccumulateInnerA;
		areaSouthWest = section.SouthWestWalls.Current.AccumulateInnerA;
		areaWest = section.WestWalls.Current.AccumulateInnerA;
		areaNorthWest = section.NorthWestWalls.Current.AccumulateInnerA;
		double num = northU * areaNorth + northEastU * areaNorthEast + eastU * areaEast + southEastU * areaSouthEast + southU * areaSouth + southWestU * areaSouthWest + westU * areaWest + northWestU * areaNorthWest;
		double num2 = areaNorth + areaNorthEast + areaEast + areaSouthEast + areaSouth + areaSouthWest + areaWest + areaNorthWest;
		double num3 = num / num2;
		if (double.IsNaN(num3) || double.IsInfinity(num3))
		{
			num3 = 0.0;
		}
		heatingAndCoolingCalculations.UinnerWallsActual = num3;
		heatingAndCoolingCalculations.UinnerWallsBaseLine = num3;
	}

	public static void CalculateUinnerWallsEsm(this CalculationData heatingAndCoolingCalculations, Section section)
	{
		northU = section.NorthWalls.Esm.AccumulateInnerU;
		northEastU = section.NorthEastWalls.Esm.AccumulateInnerU;
		eastU = section.EastWalls.Esm.AccumulateInnerU;
		southEastU = section.SouthEastWalls.Esm.AccumulateInnerU;
		southU = section.SouthWalls.Esm.AccumulateInnerU;
		southWestU = section.SouthWestWalls.Esm.AccumulateInnerU;
		westU = section.WestWalls.Esm.AccumulateInnerU;
		northWestU = section.NorthWestWalls.Esm.AccumulateInnerU;
		areaNorth = section.NorthWalls.Esm.AccumulateInnerA;
		areaNorthEast = section.NorthEastWalls.Esm.AccumulateInnerA;
		areaEast = section.EastWalls.Esm.AccumulateInnerA;
		areaSouthEast = section.SouthEastWalls.Esm.AccumulateInnerA;
		areaSouth = section.SouthWalls.Esm.AccumulateInnerA;
		areaSouthWest = section.SouthWestWalls.Esm.AccumulateInnerA;
		areaWest = section.WestWalls.Esm.AccumulateInnerA;
		areaNorthWest = section.NorthWestWalls.Esm.AccumulateInnerA;
		double num = northU * areaNorth + northEastU * areaNorthEast + eastU * areaEast + southEastU * areaSouthEast + southU * areaSouth + southWestU * areaSouthWest + westU * areaWest + northWestU * areaNorthWest;
		double num2 = areaNorth + areaNorthEast + areaEast + areaSouthEast + areaSouth + areaSouthWest + areaWest + areaNorthWest;
		double num3 = num / num2;
		if (double.IsNaN(num3) || double.IsInfinity(num3))
		{
			num3 = 0.0;
		}
		heatingAndCoolingCalculations.UinnerWallsESM = num3;
	}

	public static void CalculateUwindowsCurrent(this CalculationData heatingAndCoolingCalculations, Section section)
	{
		northU = section.NorthWalls.Current.AccumulateWindowU;
		northEastU = section.NorthEastWalls.Current.AccumulateWindowU;
		eastU = section.EastWalls.Current.AccumulateWindowU;
		southEastU = section.SouthEastWalls.Current.AccumulateWindowU;
		southU = section.SouthWalls.Current.AccumulateWindowU;
		southWestU = section.SouthWestWalls.Current.AccumulateWindowU;
		westU = section.WestWalls.Current.AccumulateWindowU;
		northWestU = section.NorthWestWalls.Current.AccumulateWindowU;
		double accumulateTransparentU = section.Roof.Current.AccumulateTransparentU;
		double accumulateTransparentA = section.Roof.Current.AccumulateTransparentA;
		areaNorth = section.NorthWalls.Current.AccumulateWindowA;
		areaNorthEast = section.NorthEastWalls.Current.AccumulateWindowA;
		areaEast = section.EastWalls.Current.AccumulateWindowA;
		areaSouthEast = section.SouthEastWalls.Current.AccumulateWindowA;
		areaSouth = section.SouthWalls.Current.AccumulateWindowA;
		areaSouthWest = section.SouthWestWalls.Current.AccumulateWindowA;
		areaWest = section.WestWalls.Current.AccumulateWindowA;
		areaNorthWest = section.NorthWestWalls.Current.AccumulateWindowA;
		double accumulateTransparentA2 = section.Roof.Current.AccumulateTransparentA;
		double num = northU * areaNorth + northEastU * areaNorthEast + eastU * areaEast + southEastU * areaSouthEast + southU * areaSouth + southWestU * areaSouthWest + westU * areaWest + northWestU * areaNorthWest + accumulateTransparentU * accumulateTransparentA;
		double num2 = areaNorth + areaNorthEast + areaEast + areaSouthEast + areaSouth + areaSouthWest + areaWest + areaNorthWest + accumulateTransparentA2;
		double num3 = num / num2;
		if (double.IsNaN(num3) || double.IsInfinity(num3))
		{
			num3 = 0.0;
		}
		heatingAndCoolingCalculations.UwindowsActual = num3;
		heatingAndCoolingCalculations.UwindowsBaseLine = num3;
	}

	public static void CalculateUwindowsEsm(this CalculationData heatingAndCoolingCalculations, Section section)
	{
		northU = section.NorthWalls.Esm.AccumulateWindowU;
		northEastU = section.NorthEastWalls.Esm.AccumulateWindowU;
		eastU = section.EastWalls.Esm.AccumulateWindowU;
		southEastU = section.SouthEastWalls.Esm.AccumulateWindowU;
		southU = section.SouthWalls.Esm.AccumulateWindowU;
		southWestU = section.SouthWestWalls.Esm.AccumulateWindowU;
		westU = section.WestWalls.Esm.AccumulateWindowU;
		northWestU = section.NorthWestWalls.Esm.AccumulateWindowU;
		double accumulateTransparentU = section.Roof.Esm.AccumulateTransparentU;
		double accumulateTransparentA = section.Roof.Esm.AccumulateTransparentA;
		areaNorth = section.NorthWalls.Esm.AccumulateWindowA;
		areaNorthEast = section.NorthEastWalls.Esm.AccumulateWindowA;
		areaEast = section.EastWalls.Esm.AccumulateWindowA;
		areaSouthEast = section.SouthEastWalls.Esm.AccumulateWindowA;
		areaSouth = section.SouthWalls.Esm.AccumulateWindowA;
		areaSouthWest = section.SouthWestWalls.Esm.AccumulateWindowA;
		areaWest = section.WestWalls.Esm.AccumulateWindowA;
		areaNorthWest = section.NorthWestWalls.Esm.AccumulateWindowA;
		double accumulateTransparentA2 = section.Roof.Esm.AccumulateTransparentA;
		double num = northU * areaNorth + northEastU * areaNorthEast + eastU * areaEast + southEastU * areaSouthEast + southU * areaSouth + southWestU * areaSouthWest + westU * areaWest + northWestU * areaNorthWest + accumulateTransparentU * accumulateTransparentA;
		double num2 = areaNorth + areaNorthEast + areaEast + areaSouthEast + areaSouth + areaSouthWest + areaWest + areaNorthWest + accumulateTransparentA2;
		double num3 = num / num2;
		if (double.IsNaN(num3) || double.IsInfinity(num3))
		{
			num3 = 0.0;
		}
		heatingAndCoolingCalculations.UwindowsESM = num3;
	}

	public static void CalculateGcurrent(this CalculationData heatingAndCoolingCalculations, Section section)
	{
		northU = section.NorthWalls.Current.AccumulateWindowG;
		northEastU = section.NorthEastWalls.Current.AccumulateWindowG;
		eastU = section.EastWalls.Current.AccumulateWindowG;
		southEastU = section.SouthEastWalls.Current.AccumulateWindowG;
		southU = section.SouthWalls.Current.AccumulateWindowG;
		southWestU = section.SouthWestWalls.Current.AccumulateWindowG;
		westU = section.WestWalls.Current.AccumulateWindowG;
		northWestU = section.NorthWestWalls.Current.AccumulateWindowG;
		double accumulateTransparentA = section.Roof.Current.AccumulateTransparentA;
		double accumulateTransparentG = section.Roof.Current.AccumulateTransparentG;
		areaNorth = section.NorthWalls.Current.AccumulateWindowA;
		areaNorthEast = section.NorthEastWalls.Current.AccumulateWindowA;
		areaEast = section.EastWalls.Current.AccumulateWindowA;
		areaSouthEast = section.SouthEastWalls.Current.AccumulateWindowA;
		areaSouth = section.SouthWalls.Current.AccumulateWindowA;
		areaSouthWest = section.SouthWestWalls.Current.AccumulateWindowA;
		areaWest = section.WestWalls.Current.AccumulateWindowA;
		areaNorthWest = section.NorthWestWalls.Current.AccumulateWindowA;
		double accumulateTransparentA2 = section.Roof.Current.AccumulateTransparentA;
		double num = northU * areaNorth + northEastU * areaNorthEast + eastU * areaEast + southEastU * areaSouthEast + southU * areaSouth + southWestU * areaSouthWest + westU * areaWest + northWestU * areaNorthWest + accumulateTransparentA * accumulateTransparentG;
		double num2 = areaNorth + areaNorthEast + areaEast + areaSouthEast + areaSouth + areaSouthWest + areaWest + areaNorthWest + accumulateTransparentA2;
		double num3 = num / num2;
		if (double.IsNaN(num3) || double.IsInfinity(num3))
		{
			num3 = 0.0;
		}
		heatingAndCoolingCalculations.gActual = num3;
		heatingAndCoolingCalculations.gBaseLine = num3;
	}

	public static void CalculateGesm(this CalculationData heatingAndCoolingCalculations, Section section)
	{
		northU = section.NorthWalls.Esm.AccumulateWindowG;
		northEastU = section.NorthEastWalls.Esm.AccumulateWindowG;
		eastU = section.EastWalls.Esm.AccumulateWindowG;
		southEastU = section.SouthEastWalls.Esm.AccumulateWindowG;
		southU = section.SouthWalls.Esm.AccumulateWindowG;
		southWestU = section.SouthWestWalls.Esm.AccumulateWindowG;
		westU = section.WestWalls.Esm.AccumulateWindowG;
		northWestU = section.NorthWestWalls.Esm.AccumulateWindowG;
		double accumulateTransparentA = section.Roof.Esm.AccumulateTransparentA;
		double accumulateTransparentG = section.Roof.Esm.AccumulateTransparentG;
		areaNorth = section.NorthWalls.Esm.AccumulateWindowA;
		areaNorthEast = section.NorthEastWalls.Esm.AccumulateWindowA;
		areaEast = section.EastWalls.Esm.AccumulateWindowA;
		areaSouthEast = section.SouthEastWalls.Esm.AccumulateWindowA;
		areaSouth = section.SouthWalls.Esm.AccumulateWindowA;
		areaSouthWest = section.SouthWestWalls.Esm.AccumulateWindowA;
		areaWest = section.WestWalls.Esm.AccumulateWindowA;
		areaNorthWest = section.NorthWestWalls.Esm.AccumulateWindowA;
		double accumulateTransparentA2 = section.Roof.Esm.AccumulateTransparentA;
		double num = northU * areaNorth + northEastU * areaNorthEast + eastU * areaEast + southEastU * areaSouthEast + southU * areaSouth + southWestU * areaSouthWest + westU * areaWest + northWestU * areaNorthWest + accumulateTransparentA * accumulateTransparentG;
		double num2 = areaNorth + areaNorthEast + areaEast + areaSouthEast + areaSouth + areaSouthWest + areaWest + areaNorthWest + accumulateTransparentA2;
		double num3 = num / num2;
		if (double.IsNaN(num3) || double.IsInfinity(num3))
		{
			num3 = 0.0;
		}
		heatingAndCoolingCalculations.gESM = num3;
	}

	public static void GetUnonTrasparentRoof(this CalculationData heatingAndCoolingCalculations, Section section)
	{
		heatingAndCoolingCalculations.UnontransparentActual = section.Roof.Current.AccumulateNonTransparentU;
		heatingAndCoolingCalculations.UnontransparentBaseLine = section.Roof.Current.AccumulateNonTransparentU;
		heatingAndCoolingCalculations.UnontransparentESM = section.Roof.Esm.AccumulateNonTransparentU;
	}

	public static void GetUceiling(this CalculationData heatingAndCoolingCalculations, Section section)
	{
		heatingAndCoolingCalculations.UceilingActual = section.Roof.Current.AccumulateCeilingU;
		heatingAndCoolingCalculations.UceilingBaseLine = section.Roof.Current.AccumulateCeilingU;
		heatingAndCoolingCalculations.UceilingESM = section.Roof.Esm.AccumulateCeilingU;
	}

	public static void GetUfloor(this CalculationData heatingAndCoolingCalculations, Section section)
	{
		heatingAndCoolingCalculations.UfloorActual = section.Floor.Current.AccumulateFloorU;
		heatingAndCoolingCalculations.UfloorBaseLine = section.Floor.Current.AccumulateFloorU;
		heatingAndCoolingCalculations.UfloorESM = section.Floor.Esm.AccumulateFloorU;
	}

	public static void GetUotherFloor(this CalculationData heatingAndCoolingCalculations, Section section)
	{
		heatingAndCoolingCalculations.UfloorOtherActual = section.Floor.Current.AccumulateOtherFloorU;
		heatingAndCoolingCalculations.UfloorOtherBaseLine = section.Floor.Current.AccumulateOtherFloorU;
		heatingAndCoolingCalculations.UfloorOtherESM = section.Floor.Esm.AccumulateOtherFloorU;
	}

	public static void CalculateNetEnergy(this CalculationData heatingCalculations)
	{
		heatingCalculations.ResulNetEnergyRef1 = heatingCalculations.ResulNoInputsNetEnergyRef1 - (heatingCalculations.ResulVentilationInputsRef1 + heatingCalculations.ResulLightInputsRef1 + heatingCalculations.ResulAppliancesInputsRef1);
		heatingCalculations.ResulNetEnergyRef2 = heatingCalculations.ResulNoInputsNetEnergyRef2 - (heatingCalculations.ResulVentilationInputsRef2 + heatingCalculations.ResulLightInputsref2 + heatingCalculations.ResulAppliancesInputsRef2);
		heatingCalculations.ResulNetEnergyActual = heatingCalculations.ResulNoInputsNetEnergyActual - (heatingCalculations.ResulVentilationInputsActual + heatingCalculations.ResulLightInputsActual + heatingCalculations.ResulAppliancesInputsActual);
		heatingCalculations.ResulNetEnergyBaseLine = heatingCalculations.ResulNoInputsNetEnergyBaseLine - (heatingCalculations.ResulVentilationInputsBaseLine + heatingCalculations.ResulLightInputsBaseLine + heatingCalculations.ResulAppliancesInputsBaseLine);
		heatingCalculations.ResulNetEnergyESM = heatingCalculations.ResulNoInputsNetEnergyESM - (heatingCalculations.ResulVentilationInputsESM + heatingCalculations.ResulLightInputsESM + heatingCalculations.ResulAppliancesInputsESM);
	}

	public static void CalculateNeededEnergyRef1(this CalculationData heatgCalc)
	{
		double num = heatgCalc.ResulNetEnergyRef1 * heatgCalc.Part1Ref1 / 100.0;
		heatgCalc.ResultSourceEnergyRef1 = num / (heatgCalc.TransmitTempEfficiencyRef1 / 100.0 * (heatgCalc.SupplyNetEfficiencyRef1 / 100.0) * (heatgCalc.AutomaticRef1 / 100.0) * (heatgCalc.EnergyManagementRef1 / 100.0) * (heatgCalc.GeneratorHeatEfficiency1Ref1 / 100.0));
		if (double.IsInfinity(heatgCalc.ResultSourceEnergyRef1) || double.IsNaN(heatgCalc.ResultSourceEnergyRef1))
		{
			heatgCalc.ResultSourceEnergyRef1 = 0.0;
		}
		double num2 = heatgCalc.ResulNetEnergyRef1 * heatgCalc.Part2Ref1 / 100.0;
		heatgCalc.ResultSourceEnergy2Ref1 = num2 / (heatgCalc.TransmitTempEfficiency2Ref1 / 100.0 * (heatgCalc.SupplyNetEfficiency2Ref1 / 100.0) * (heatgCalc.Automatic2Ref1 / 100.0) * (heatgCalc.EnergyManagement2Ref1 / 100.0) * (heatgCalc.GeneratorHeatEfficiency2Ref1 / 100.0));
		if (double.IsInfinity(heatgCalc.ResultSourceEnergy2Ref1) || double.IsNaN(heatgCalc.ResultSourceEnergy2Ref1))
		{
			heatgCalc.ResultSourceEnergy2Ref1 = 0.0;
		}
		heatgCalc.ResultNeededEnergyRef1 = heatgCalc.ResultSourceEnergyRef1 + heatgCalc.ResultSourceEnergy2Ref1;
	}

	public static void CalculateNeededEnergyRef2(this CalculationData heatgCalc)
	{
		double num = heatgCalc.ResulNetEnergyRef2 * heatgCalc.Part1Ref2 / 100.0;
		heatgCalc.ResultSourceEnergyRef2 = num / (heatgCalc.TransmitTempEfficiencyRef2 / 100.0 * (heatgCalc.SupplyNetEfficiencyRef2 / 100.0) * (heatgCalc.AutomaticRef2 / 100.0) * (heatgCalc.EnergyManagementRef2 / 100.0) * (heatgCalc.GeneratorHeatEfficiency1Ref2 / 100.0));
		if (double.IsInfinity(heatgCalc.ResultSourceEnergyRef2) || double.IsNaN(heatgCalc.ResultSourceEnergyRef2))
		{
			heatgCalc.ResultSourceEnergyRef2 = 0.0;
		}
		double num2 = heatgCalc.ResulNetEnergyRef2 * heatgCalc.Part2Ref2 / 100.0;
		heatgCalc.ResultSourceEnergy2Ref2 = num2 / (heatgCalc.TransmitTempEfficiency2Ref2 / 100.0 * (heatgCalc.SupplyNetEfficiency2Ref2 / 100.0) * (heatgCalc.Automatic2Ref2 / 100.0) * (heatgCalc.EnergyManagement2Ref2 / 100.0) * (heatgCalc.GeneratorHeatEfficiency2Ref2 / 100.0));
		if (double.IsInfinity(heatgCalc.ResultSourceEnergy2Ref2) || double.IsNaN(heatgCalc.ResultSourceEnergy2Ref2))
		{
			heatgCalc.ResultSourceEnergy2Ref2 = 0.0;
		}
		heatgCalc.ResultNeededEnergyRef2 = heatgCalc.ResultSourceEnergyRef2 + heatgCalc.ResultSourceEnergy2Ref2;
	}

	public static void CalculateNeededEnergyActual(this CalculationData heatgCalc)
	{
		double num = heatgCalc.ResulNetEnergyActual * heatgCalc.Part1Actual / 100.0;
		heatgCalc.ResultSourceEnergyActual = num / (heatgCalc.TransmitTempEfficiencyActual / 100.0 * (heatgCalc.SupplyNetEfficiencyActual / 100.0) * (heatgCalc.AutomaticActual / 100.0) * (heatgCalc.EnergyManagementActual / 100.0) * (heatgCalc.GeneratorHeatEfficiency1Actual / 100.0));
		if (double.IsInfinity(heatgCalc.ResultSourceEnergyActual) || double.IsNaN(heatgCalc.ResultSourceEnergyActual))
		{
			heatgCalc.ResultSourceEnergyActual = 0.0;
		}
		double num2 = heatgCalc.ResulNetEnergyActual * heatgCalc.Part2Actual / 100.0;
		heatgCalc.ResultSourceEnergy2Actual = num2 / (heatgCalc.TransmitTempEfficiency2Actual / 100.0 * (heatgCalc.SupplyNetEfficiency2Actual / 100.0) * (heatgCalc.Automatic2Actual / 100.0) * (heatgCalc.EnergyManagement2Actual / 100.0) * (heatgCalc.GeneratorHeatEfficiency2Actual / 100.0));
		if (double.IsInfinity(heatgCalc.ResultSourceEnergy2Actual) || double.IsNaN(heatgCalc.ResultSourceEnergy2Actual))
		{
			heatgCalc.ResultSourceEnergy2Actual = 0.0;
		}
		heatgCalc.ResultNeededEnergyActual = heatgCalc.ResultSourceEnergyActual + heatgCalc.ResultSourceEnergy2Actual;
	}

	public static void CalculateNeededEnergyBaseLine(this CalculationData heatgCalc)
	{
		double num = heatgCalc.ResulNetEnergyBaseLine * heatgCalc.Part1BaseLine / 100.0;
		heatgCalc.ResultSourceEnergyBaseLine = num / (heatgCalc.TransmitTempEfficiencyBaseLine / 100.0 * (heatgCalc.SupplyNetEfficiencyBaseLine / 100.0) * (heatgCalc.AutomaticBaseLine / 100.0) * (heatgCalc.EnergyManagementBaseLine / 100.0) * (heatgCalc.GeneratorHeatEfficiency1BaseLine / 100.0));
		if (double.IsInfinity(heatgCalc.ResultSourceEnergyBaseLine) || double.IsNaN(heatgCalc.ResultSourceEnergyBaseLine))
		{
			heatgCalc.ResultSourceEnergyBaseLine = 0.0;
		}
		double num2 = heatgCalc.ResulNetEnergyBaseLine * heatgCalc.Part2BaseLine / 100.0;
		heatgCalc.ResultSourceEnergy2BaseLine = num2 / (heatgCalc.TransmitTempEfficiency2BaseLine / 100.0 * (heatgCalc.SupplyNetEfficiency2BaseLine / 100.0) * (heatgCalc.Automatic2BaseLine / 100.0) * (heatgCalc.EnergyManagement2BaseLine / 100.0) * (heatgCalc.GeneratorHeatEfficiency2BaseLine / 100.0));
		if (double.IsInfinity(heatgCalc.ResultSourceEnergy2BaseLine) || double.IsNaN(heatgCalc.ResultSourceEnergy2BaseLine))
		{
			heatgCalc.ResultSourceEnergy2BaseLine = 0.0;
		}
		heatgCalc.ResultNeededEnergyBaseLine = heatgCalc.ResultSourceEnergyBaseLine + heatgCalc.ResultSourceEnergy2BaseLine;
	}

	public static void CalculateNeededEnergyEsm(this CalculationData heatgCalc)
	{
		double num = heatgCalc.ResulNetEnergyESM * heatgCalc.Part1ESM / 100.0;
		heatgCalc.ResultSourceEnergyESM = num / (heatgCalc.TransmitTempEfficiencyESM / 100.0 * (heatgCalc.SupplyNetEfficiencyESM / 100.0) * (heatgCalc.AutomaticESM / 100.0) * (heatgCalc.EnergyManagementESM / 100.0) * (heatgCalc.GeneratorHeatEfficiency1ESM / 100.0));
		if (double.IsInfinity(heatgCalc.ResultSourceEnergyESM) || double.IsNaN(heatgCalc.ResultSourceEnergyESM))
		{
			heatgCalc.ResultSourceEnergyESM = 0.0;
		}
		double num2 = heatgCalc.ResulNetEnergyESM * heatgCalc.Part2ESM / 100.0;
		heatgCalc.ResultSourceEnergy2ESM = num2 / (heatgCalc.TransmitTempEfficiency2ESM / 100.0 * (heatgCalc.SupplyNetEfficiency2ESM / 100.0) * (heatgCalc.Automatic2ESM / 100.0) * (heatgCalc.EnergyManagement2ESM / 100.0) * (heatgCalc.GeneratorHeatEfficiency2ESM / 100.0));
		if (double.IsInfinity(heatgCalc.ResultSourceEnergy2ESM) || double.IsNaN(heatgCalc.ResultSourceEnergy2ESM))
		{
			heatgCalc.ResultSourceEnergy2ESM = 0.0;
		}
		heatgCalc.ResultNeededEnergyESM = heatgCalc.ResultSourceEnergyESM + heatgCalc.ResultSourceEnergy2ESM;
		heatgCalc.ResultNeededEnergySavings = (heatgCalc.ResultNeededEnergyBaseLine - heatgCalc.ResultNeededEnergyESM).ToString("F3");
	}

	public static void CalculateGeneratorHeatEfficiencyRef1(this CalculationData heatgCalc)
	{
		try
		{
			heatgCalc.HeatEfficiencyGeneratingRef1 = (heatgCalc.ResultSourceEnergyRef1 * heatgCalc.GeneratorHeatEfficiency1Ref1 + heatgCalc.ResultSourceEnergy2Ref1 * heatgCalc.GeneratorHeatEfficiency2Ref1) / (heatgCalc.ResultSourceEnergyRef1 + heatgCalc.ResultSourceEnergy2Ref1);
			if (double.IsInfinity(heatgCalc.HeatEfficiencyGeneratingRef1) || double.IsNaN(heatgCalc.HeatEfficiencyGeneratingRef1))
			{
				heatgCalc.HeatEfficiencyGeneratingRef1 = 0.0;
			}
		}
		catch
		{
			heatgCalc.HeatEfficiencyGeneratingRef1 = 0.0;
		}
	}

	public static void CalculateGeneratorHeatEfficiencyRef2(this CalculationData heatgCalc)
	{
		try
		{
			heatgCalc.HeatEfficiencyGeneratingRef2 = (heatgCalc.ResultSourceEnergyRef2 * heatgCalc.GeneratorHeatEfficiency1Ref2 + heatgCalc.ResultSourceEnergy2Ref2 * heatgCalc.GeneratorHeatEfficiency2Ref2) / (heatgCalc.ResultSourceEnergyRef2 + heatgCalc.ResultSourceEnergy2Ref2);
			if (double.IsInfinity(heatgCalc.HeatEfficiencyGeneratingRef2) || double.IsNaN(heatgCalc.HeatEfficiencyGeneratingRef2))
			{
				heatgCalc.HeatEfficiencyGeneratingRef2 = 0.0;
			}
		}
		catch
		{
			heatgCalc.HeatEfficiencyGeneratingRef2 = 0.0;
		}
	}

	public static void CalculateGeneratorHeatEfficiencyActual(this CalculationData heatgCalc)
	{
		try
		{
			heatgCalc.HeatEfficiencyGeneratingActual = (heatgCalc.ResultSourceEnergyActual * heatgCalc.GeneratorHeatEfficiency1Actual + heatgCalc.ResultSourceEnergy2Actual * heatgCalc.GeneratorHeatEfficiency2Actual) / (heatgCalc.ResultSourceEnergyActual + heatgCalc.ResultSourceEnergy2Actual);
			if (double.IsInfinity(heatgCalc.HeatEfficiencyGeneratingActual) || double.IsNaN(heatgCalc.HeatEfficiencyGeneratingActual))
			{
				heatgCalc.HeatEfficiencyGeneratingActual = 0.0;
			}
		}
		catch
		{
			heatgCalc.HeatEfficiencyGeneratingActual = 0.0;
		}
	}

	public static void CalculateGeneratorHeatEfficiencyBaseLine(this CalculationData heatgCalc)
	{
		try
		{
			heatgCalc.HeatEfficiencyGeneratingBaseLine = (heatgCalc.ResultSourceEnergyBaseLine * heatgCalc.GeneratorHeatEfficiency1BaseLine + heatgCalc.ResultSourceEnergy2BaseLine * heatgCalc.GeneratorHeatEfficiency2BaseLine) / (heatgCalc.ResultSourceEnergyBaseLine + heatgCalc.ResultSourceEnergy2BaseLine);
			if (double.IsInfinity(heatgCalc.HeatEfficiencyGeneratingBaseLine) || double.IsNaN(heatgCalc.HeatEfficiencyGeneratingBaseLine))
			{
				heatgCalc.HeatEfficiencyGeneratingBaseLine = 0.0;
			}
		}
		catch
		{
			heatgCalc.HeatEfficiencyGeneratingBaseLine = 0.0;
		}
	}

	public static void CalculateGeneratorHeatEfficiencyEsm(this CalculationData heatgCalc)
	{
		try
		{
			heatgCalc.HeatEfficiencyGeneratingESM = (heatgCalc.ResultSourceEnergyESM * heatgCalc.GeneratorHeatEfficiency1ESM + heatgCalc.ResultSourceEnergy2ESM * heatgCalc.GeneratorHeatEfficiency2ESM) / (heatgCalc.ResultSourceEnergyESM + heatgCalc.ResultSourceEnergy2ESM);
			if (double.IsInfinity(heatgCalc.HeatEfficiencyGeneratingESM) || double.IsNaN(heatgCalc.HeatEfficiencyGeneratingESM))
			{
				heatgCalc.HeatEfficiencyGeneratingESM = 0.0;
			}
		}
		catch
		{
			heatgCalc.HeatEfficiencyGeneratingESM = 0.0;
		}
	}

	public static void CalculateGeneratorVentilationCoolEfficiencyRef1(this CalculationData coolCalc)
	{
		try
		{
			coolCalc.ColdEfficiencyGeneratingRef1 = (coolCalc.ResultSourceEnergyRef1 * coolCalc.GeneratorColdEfficiency1Ref1 + coolCalc.ResultSourceEnergy2Ref1 * coolCalc.GeneratorColdEfficiency2Ref1) / (coolCalc.ResultSourceEnergyRef1 + coolCalc.ResultSourceEnergy2Ref1);
			if (double.IsInfinity(coolCalc.ColdEfficiencyGeneratingRef1) || double.IsNaN(coolCalc.ColdEfficiencyGeneratingRef1))
			{
				coolCalc.ColdEfficiencyGeneratingRef1 = 0.0;
			}
		}
		catch
		{
			coolCalc.ColdEfficiencyGeneratingRef1 = 0.0;
		}
	}

	public static void CalculateGeneratorVentilationCoolEfficiencyRef2(this CalculationData coolCalc)
	{
		try
		{
			coolCalc.ColdEfficiencyGeneratingRef2 = (coolCalc.ResultSourceEnergyRef2 * coolCalc.GeneratorColdEfficiency1Ref2 + coolCalc.ResultSourceEnergy2Ref2 * coolCalc.GeneratorColdEfficiency2Ref2) / (coolCalc.ResultSourceEnergyRef2 + coolCalc.ResultSourceEnergy2Ref2);
			if (double.IsInfinity(coolCalc.ColdEfficiencyGeneratingRef2) || double.IsNaN(coolCalc.ColdEfficiencyGeneratingRef2))
			{
				coolCalc.ColdEfficiencyGeneratingRef2 = 0.0;
			}
		}
		catch
		{
			coolCalc.ColdEfficiencyGeneratingRef2 = 0.0;
		}
	}

	public static void CalculateGeneratorVentilationCoolEfficiencyActual(this CalculationData coolCalc)
	{
		try
		{
			coolCalc.ColdEfficiencyGeneratingActual = (coolCalc.ResultSourceEnergyActual * coolCalc.GeneratorColdEfficiency1Actual + coolCalc.ResultSourceEnergy2Actual * coolCalc.GeneratorColdEfficiency2Actual) / (coolCalc.ResultSourceEnergyActual + coolCalc.ResultSourceEnergy2Actual);
			if (double.IsInfinity(coolCalc.ColdEfficiencyGeneratingActual) || double.IsNaN(coolCalc.ColdEfficiencyGeneratingActual))
			{
				coolCalc.ColdEfficiencyGeneratingActual = 0.0;
			}
		}
		catch
		{
			coolCalc.ColdEfficiencyGeneratingActual = 0.0;
		}
	}

	public static void CalculateGeneratorVentilationCoolEfficiencyBaseLine(this CalculationData coolCalc)
	{
		try
		{
			coolCalc.ColdEfficiencyGeneratingBaseLine = (coolCalc.ResultSourceEnergyBaseLine * coolCalc.GeneratorColdEfficiency1BaseLine + coolCalc.ResultSourceEnergy2BaseLine * coolCalc.GeneratorColdEfficiency2BaseLine) / (coolCalc.ResultSourceEnergyBaseLine + coolCalc.ResultSourceEnergy2BaseLine);
			if (double.IsInfinity(coolCalc.ColdEfficiencyGeneratingBaseLine) || double.IsNaN(coolCalc.ColdEfficiencyGeneratingBaseLine))
			{
				coolCalc.ColdEfficiencyGeneratingBaseLine = 0.0;
			}
		}
		catch
		{
			coolCalc.ColdEfficiencyGeneratingBaseLine = 0.0;
		}
	}

	public static void CalculateGeneratorVentilationCoolEfficiencyESM(this CalculationData coolCalc)
	{
		try
		{
			coolCalc.ColdEfficiencyGeneratingESM = (coolCalc.ResultSourceEnergyESM * coolCalc.GeneratorColdEfficiency1ESM + coolCalc.ResultSourceEnergy2ESM * coolCalc.GeneratorColdEfficiency2ESM) / (coolCalc.ResultSourceEnergyESM + coolCalc.ResultSourceEnergy2ESM);
			if (double.IsInfinity(coolCalc.ColdEfficiencyGeneratingESM) || double.IsNaN(coolCalc.ColdEfficiencyGeneratingESM))
			{
				coolCalc.ColdEfficiencyGeneratingESM = 0.0;
			}
		}
		catch
		{
			coolCalc.ColdEfficiencyGeneratingESM = 0.0;
		}
	}

	public static void CalculateGeneratorCoolEfficiencyRef1(this CalculationData coolCalc)
	{
		try
		{
			coolCalc.HeatEfficiencyGeneratingRef1 = (coolCalc.ResultSourceEnergyRef1 * coolCalc.GeneratorColdEfficiency1Ref1 + coolCalc.ResultSourceEnergy2Ref1 * coolCalc.GeneratorColdEfficiency2Ref1) / (coolCalc.ResultSourceEnergyRef1 + coolCalc.ResultSourceEnergy2Ref1);
			if (double.IsInfinity(coolCalc.HeatEfficiencyGeneratingRef1) || double.IsNaN(coolCalc.HeatEfficiencyGeneratingRef1))
			{
				coolCalc.HeatEfficiencyGeneratingRef1 = 0.0;
			}
		}
		catch
		{
			coolCalc.HeatEfficiencyGeneratingRef1 = 0.0;
		}
	}

	public static void CalculateGeneratorCoolEfficiencyRef2(this CalculationData coolCalc)
	{
		try
		{
			coolCalc.HeatEfficiencyGeneratingRef2 = (coolCalc.ResultSourceEnergyRef2 * coolCalc.GeneratorColdEfficiency1Ref2 + coolCalc.ResultSourceEnergy2Ref2 * coolCalc.GeneratorColdEfficiency2Ref2) / (coolCalc.ResultSourceEnergyRef2 + coolCalc.ResultSourceEnergy2Ref2);
			if (double.IsInfinity(coolCalc.HeatEfficiencyGeneratingRef2) || double.IsNaN(coolCalc.HeatEfficiencyGeneratingRef2))
			{
				coolCalc.HeatEfficiencyGeneratingRef2 = 0.0;
			}
		}
		catch
		{
			coolCalc.HeatEfficiencyGeneratingRef2 = 0.0;
		}
	}

	public static void CalculateGeneratorCoolEfficiencyActual(this CalculationData coolCalc)
	{
		try
		{
			coolCalc.HeatEfficiencyGeneratingActual = (coolCalc.ResultSourceEnergyActual * coolCalc.GeneratorColdEfficiency1Actual + coolCalc.ResultSourceEnergy2Actual * coolCalc.GeneratorColdEfficiency2Actual) / (coolCalc.ResultSourceEnergyActual + coolCalc.ResultSourceEnergy2Actual);
			if (double.IsInfinity(coolCalc.HeatEfficiencyGeneratingActual) || double.IsNaN(coolCalc.HeatEfficiencyGeneratingActual))
			{
				coolCalc.HeatEfficiencyGeneratingActual = 0.0;
			}
		}
		catch
		{
			coolCalc.HeatEfficiencyGeneratingActual = 0.0;
		}
	}

	public static void CalculateGeneratorCoolEfficiencyBaseLine(this CalculationData coolCalc)
	{
		try
		{
			coolCalc.HeatEfficiencyGeneratingBaseLine = (coolCalc.ResultSourceEnergyBaseLine * coolCalc.GeneratorColdEfficiency1BaseLine + coolCalc.ResultSourceEnergy2BaseLine * coolCalc.GeneratorColdEfficiency2BaseLine) / (coolCalc.ResultSourceEnergyBaseLine + coolCalc.ResultSourceEnergy2BaseLine);
			if (double.IsInfinity(coolCalc.HeatEfficiencyGeneratingBaseLine) || double.IsNaN(coolCalc.HeatEfficiencyGeneratingBaseLine))
			{
				coolCalc.HeatEfficiencyGeneratingBaseLine = 0.0;
			}
		}
		catch
		{
			coolCalc.HeatEfficiencyGeneratingBaseLine = 0.0;
		}
	}

	public static void CalculateGeneratorCoolEfficiencyESM(this CalculationData coolCalc)
	{
		try
		{
			coolCalc.HeatEfficiencyGeneratingESM = (coolCalc.ResultSourceEnergyESM * coolCalc.GeneratorColdEfficiency1ESM + coolCalc.ResultSourceEnergy2ESM * coolCalc.GeneratorColdEfficiency2ESM) / (coolCalc.ResultSourceEnergyESM + coolCalc.ResultSourceEnergy2ESM);
			if (double.IsInfinity(coolCalc.HeatEfficiencyGeneratingESM) || double.IsNaN(coolCalc.HeatEfficiencyGeneratingESM))
			{
				coolCalc.HeatEfficiencyGeneratingESM = 0.0;
			}
		}
		catch
		{
			coolCalc.HeatEfficiencyGeneratingESM = 0.0;
		}
	}

	public static void CalculateNeededEnergyCoolingRef1(this CalculationData coolCalc)
	{
		double num = coolCalc.ResulNetEnergyRef1 * coolCalc.Part1Ref1 / 100.0;
		coolCalc.ResultSourceEnergyRef1 = num / (coolCalc.TransmitTempEfficiencyRef1 / 100.0 * (coolCalc.SupplyNetEfficiencyRef1 / 100.0) * (coolCalc.AutomaticRef1 / 100.0) * (coolCalc.EnergyManagementRef1 / 100.0) * (coolCalc.GeneratorColdEfficiency1Ref1 / 100.0));
		if (double.IsInfinity(coolCalc.ResultSourceEnergyRef1) || double.IsNaN(coolCalc.ResultSourceEnergyRef1))
		{
			coolCalc.ResultSourceEnergyRef1 = 0.0;
		}
		double num2 = coolCalc.ResulNetEnergyRef1 * coolCalc.Part2Ref1 / 100.0;
		coolCalc.ResultSourceEnergy2Ref1 = num2 / (coolCalc.TransmitTempEfficiency2Ref1 / 100.0 * (coolCalc.SupplyNetEfficiency2Ref1 / 100.0) * (coolCalc.Automatic2Ref1 / 100.0) * (coolCalc.EnergyManagement2Ref1 / 100.0) * (coolCalc.GeneratorColdEfficiency2Ref1 / 100.0));
		if (double.IsInfinity(coolCalc.ResultSourceEnergy2Ref1) || double.IsNaN(coolCalc.ResultSourceEnergy2Ref1))
		{
			coolCalc.ResultSourceEnergy2Ref1 = 0.0;
		}
		coolCalc.ResultNeededEnergyRef1 = coolCalc.ResultSourceEnergyRef1 + coolCalc.ResultSourceEnergy2Ref1;
	}

	public static void CalculateNeededEnergyCoolingRef2(this CalculationData coolCalc)
	{
		double num = coolCalc.ResulNetEnergyRef2 * coolCalc.Part1Ref2 / 100.0;
		coolCalc.ResultSourceEnergyRef2 = num / (coolCalc.TransmitTempEfficiencyRef2 / 100.0 * (coolCalc.SupplyNetEfficiencyRef2 / 100.0) * (coolCalc.AutomaticRef2 / 100.0) * (coolCalc.EnergyManagementRef2 / 100.0) * (coolCalc.GeneratorColdEfficiency1Ref2 / 100.0));
		if (double.IsInfinity(coolCalc.ResultSourceEnergyRef2) || double.IsNaN(coolCalc.ResultSourceEnergyRef2))
		{
			coolCalc.ResultSourceEnergyRef2 = 0.0;
		}
		double num2 = coolCalc.ResulNetEnergyRef2 * coolCalc.Part2Ref2 / 100.0;
		coolCalc.ResultSourceEnergy2Ref2 = num2 / (coolCalc.TransmitTempEfficiency2Ref2 / 100.0 * (coolCalc.SupplyNetEfficiency2Ref2 / 100.0) * (coolCalc.Automatic2Ref2 / 100.0) * (coolCalc.EnergyManagement2Ref2 / 100.0) * (coolCalc.GeneratorColdEfficiency2Ref2 / 100.0));
		if (double.IsInfinity(coolCalc.ResultSourceEnergy2Ref2) || double.IsNaN(coolCalc.ResultSourceEnergy2Ref2))
		{
			coolCalc.ResultSourceEnergy2Ref2 = 0.0;
		}
		coolCalc.ResultNeededEnergyRef2 = coolCalc.ResultSourceEnergyRef2 + coolCalc.ResultSourceEnergy2Ref2;
	}

	public static void CalculateNeededEnergyCoolingActual(this CalculationData coolCalc)
	{
		double num = coolCalc.ResulNetEnergyActual * coolCalc.Part1Actual / 100.0;
		coolCalc.ResultSourceEnergyActual = num / (coolCalc.TransmitTempEfficiencyActual / 100.0 * (coolCalc.SupplyNetEfficiencyActual / 100.0) * (coolCalc.AutomaticActual / 100.0) * (coolCalc.EnergyManagementActual / 100.0) * (coolCalc.GeneratorColdEfficiency1Actual / 100.0));
		if (double.IsInfinity(coolCalc.ResultSourceEnergyActual) || double.IsNaN(coolCalc.ResultSourceEnergyActual))
		{
			coolCalc.ResultSourceEnergyActual = 0.0;
		}
		double num2 = coolCalc.ResulNetEnergyActual * coolCalc.Part2Actual / 100.0;
		coolCalc.ResultSourceEnergy2Actual = num2 / (coolCalc.TransmitTempEfficiency2Actual / 100.0 * (coolCalc.SupplyNetEfficiency2Actual / 100.0) * (coolCalc.Automatic2Actual / 100.0) * (coolCalc.EnergyManagement2Actual / 100.0) * (coolCalc.GeneratorColdEfficiency2Actual / 100.0));
		if (double.IsInfinity(coolCalc.ResultSourceEnergy2Actual) || double.IsNaN(coolCalc.ResultSourceEnergy2Actual))
		{
			coolCalc.ResultSourceEnergy2Actual = 0.0;
		}
		coolCalc.ResultNeededEnergyActual = coolCalc.ResultSourceEnergyActual + coolCalc.ResultSourceEnergy2Actual;
	}

	public static void CalculateNeededEnergyCoolingBaseLine(this CalculationData coolCalc)
	{
		double num = coolCalc.ResulNetEnergyBaseLine * coolCalc.Part1BaseLine / 100.0;
		coolCalc.ResultSourceEnergyBaseLine = num / (coolCalc.TransmitTempEfficiencyBaseLine / 100.0 * (coolCalc.SupplyNetEfficiencyBaseLine / 100.0) * (coolCalc.AutomaticBaseLine / 100.0) * (coolCalc.EnergyManagementBaseLine / 100.0) * (coolCalc.GeneratorColdEfficiency1BaseLine / 100.0));
		if (double.IsInfinity(coolCalc.ResultSourceEnergyBaseLine) || double.IsNaN(coolCalc.ResultSourceEnergyBaseLine))
		{
			coolCalc.ResultSourceEnergyBaseLine = 0.0;
		}
		double num2 = coolCalc.ResulNetEnergyBaseLine * coolCalc.Part2BaseLine / 100.0;
		coolCalc.ResultSourceEnergy2BaseLine = num2 / (coolCalc.TransmitTempEfficiency2BaseLine / 100.0 * (coolCalc.SupplyNetEfficiency2BaseLine / 100.0) * (coolCalc.Automatic2BaseLine / 100.0) * (coolCalc.EnergyManagement2BaseLine / 100.0) * (coolCalc.GeneratorColdEfficiency2BaseLine / 100.0));
		if (double.IsInfinity(coolCalc.ResultSourceEnergy2BaseLine) || double.IsNaN(coolCalc.ResultSourceEnergy2BaseLine))
		{
			coolCalc.ResultSourceEnergy2BaseLine = 0.0;
		}
		coolCalc.ResultNeededEnergyBaseLine = coolCalc.ResultSourceEnergyBaseLine + coolCalc.ResultSourceEnergy2BaseLine;
	}

	public static void CalculateNeededEnergyCoolingESM(this CalculationData coolCalc)
	{
		double num = coolCalc.ResulNetEnergyESM * coolCalc.Part1ESM / 100.0;
		coolCalc.ResultSourceEnergyESM = num / (coolCalc.TransmitTempEfficiencyESM / 100.0 * (coolCalc.SupplyNetEfficiencyESM / 100.0) * (coolCalc.AutomaticESM / 100.0) * (coolCalc.EnergyManagementESM / 100.0) * (coolCalc.GeneratorColdEfficiency1ESM / 100.0));
		if (double.IsInfinity(coolCalc.ResultSourceEnergyESM) || double.IsNaN(coolCalc.ResultSourceEnergyESM))
		{
			coolCalc.ResultSourceEnergyESM = 0.0;
		}
		double num2 = coolCalc.ResulNetEnergyESM * coolCalc.Part2ESM / 100.0;
		coolCalc.ResultSourceEnergy2ESM = num2 / (coolCalc.TransmitTempEfficiency2ESM / 100.0 * (coolCalc.SupplyNetEfficiency2ESM / 100.0) * (coolCalc.Automatic2ESM / 100.0) * (coolCalc.EnergyManagement2ESM / 100.0) * (coolCalc.GeneratorColdEfficiency2ESM / 100.0));
		if (double.IsInfinity(coolCalc.ResultSourceEnergy2ESM) || double.IsNaN(coolCalc.ResultSourceEnergy2ESM))
		{
			coolCalc.ResultSourceEnergy2ESM = 0.0;
		}
		coolCalc.ResultNeededEnergyESM = coolCalc.ResultSourceEnergyESM + coolCalc.ResultSourceEnergy2ESM;
		coolCalc.ResultNeededEnergySavings = (coolCalc.ResultNeededEnergyBaseLine - coolCalc.ResultNeededEnergyESM).ToString("F3");
	}

	public static void Calculations(this CalculationData calcData, Section section, CalculationInput calcInput, BuildingZone zone, CalculationData lightsAndDevicesCalculationData)
	{
		currentZone = zone;
		List<MonthData> list = new List<MonthData>();
		List<double> list2 = new List<double>();
		List<double> list3 = new List<double>();
		List<double> list4 = new List<double>();
		List<double> list5 = new List<double>();
		List<double> list6 = new List<double>();
		List<double> list7 = new List<double>();
		List<double> list8 = new List<double>();
		List<double> list9 = new List<double>();
		List<double> list10 = new List<double>();
		List<double> list11 = new List<double>();
		Section section2 = new Section();
		Section section3 = new Section();
		if (zone.HasRefenceValues)
		{
			section2 = EECalcCore.EntityBase<Section>.Deserialize(section.Serialize());
			ApplyValuesToTempSectionRef1(section2, calcData);
			section3 = EECalcCore.EntityBase<Section>.Deserialize(section.Serialize());
			ApplyValuesToTempSectionRef2(section3, calcData);
		}
		List<MonthlyDays> list12 = section.CalcPeriod((int)section.HeatingSeason.FirstMonthHeat, (int)section.HeatingSeason.LastMonthHeat, section.HeatingSeason.FirstDayHeat, section.HeatingSeason.LastDayHeat);
		foreach (MonthlyDays item in list12)
		{
			if (zone.HasRefenceValues)
			{
				CalculateRef1(calcData, section, calcInput, list2, section2, item, list7);
				CalculateRef2(calcData, section, calcInput, list3, section3, item, list8);
			}
			MonthData monthData = new MonthData();
			int num = OccupantHours(section, item);
			double num2 = section.Area.MetabolicHeat * (double)num / 1000.0;
			CalculateActual(calcData, section, calcInput, monthData, item, num2);
			list9.Add(monthData.ParameterNi * num2);
			list4.Add(monthData.NetEnergyQnd);
			int num3 = OccupantsHoursBaseLine(section, item);
			double num4 = section.Area.MetabolicHeat * (double)num3 / 1000.0;
			double num5 = calcData.CalculateBaseLine(section, calcInput, item, num4);
			list5.Add(num5);
			list10.Add(parameterNiBaseLine * num4);
			int num6 = OccupantsHoursEsm(section, item);
			double num7 = section.Area.MetabolicHeat * (double)num6 / 1000.0;
			double num8 = calcData.CalculateEsm(section, calcInput, item, num7);
			list6.Add(num8);
			list11.Add(parameterNiESM * num7);
			CalculateLightsAndDevicesInputs(lightsAndDevicesCalculationData, item, parameterNiRef1, parameterNiRef2, monthData.ParameterNi, parameterNiBaseLine, parameterNiESM);
			list.Add(monthData);
			if (item.Month == Month.January)
			{
				double num9 = monthData.NetEnergyQnd - monthData.ParameterNi * num2 * section.Area.HeatedArea;
				double num10 = num9 * calcData.Part1Actual / 100.0 / (calcData.TransmitTempEfficiencyActual / 100.0 * (calcData.SupplyNetEfficiencyActual / 100.0) * (calcData.AutomaticActual / 100.0) * (calcData.EnergyManagementActual / 100.0) * (calcData.GeneratorHeatEfficiency1Actual / 100.0));
				if (double.IsInfinity(num10) || double.IsNaN(num10))
				{
					num10 = 0.0;
				}
				double num11 = num9 * calcData.Part2Actual / 100.0 / (calcData.TransmitTempEfficiency2Actual / 100.0 * (calcData.SupplyNetEfficiency2Actual / 100.0) * (calcData.Automatic2Actual / 100.0) * (calcData.EnergyManagement2Actual / 100.0) * (calcData.GeneratorHeatEfficiency2Actual / 100.0));
				if (double.IsInfinity(num11) || double.IsNaN(num11))
				{
					num11 = 0.0;
				}
				section.Area.ETlineData.MonthJanuaryHeatingEnergy.Actual = num10 + num11;
				section.Area.ETlineData.MonthJanuaryOuterTemp.Actual = PreferencesManager.GetClimateZoneParams(calcInput.General.ClimateZone).SolarRadiation.Months[0].AvgTemp;
				num9 = num5 - parameterNiBaseLine * num4 * section.Area.HeatedArea;
				num10 = num9 * calcData.Part1BaseLine / 100.0 / (calcData.TransmitTempEfficiencyBaseLine / 100.0 * (calcData.SupplyNetEfficiencyBaseLine / 100.0) * (calcData.AutomaticBaseLine / 100.0) * (calcData.EnergyManagementBaseLine / 100.0) * (calcData.GeneratorHeatEfficiency1BaseLine / 100.0));
				if (double.IsInfinity(num10) || double.IsNaN(num10))
				{
					num10 = 0.0;
				}
				num11 = num9 * calcData.Part2BaseLine / 100.0 / (calcData.TransmitTempEfficiency2BaseLine / 100.0 * (calcData.SupplyNetEfficiency2BaseLine / 100.0) * (calcData.Automatic2BaseLine / 100.0) * (calcData.EnergyManagement2BaseLine / 100.0) * (calcData.GeneratorHeatEfficiency2BaseLine / 100.0));
				if (double.IsInfinity(num11) || double.IsNaN(num11))
				{
					num11 = 0.0;
				}
				section.Area.ETlineData.MonthJanuaryHeatingEnergy.BaseLine = num10 + num11;
				section.Area.ETlineData.MonthJanuaryOuterTemp.BaseLine = PreferencesManager.GetClimateZoneParams(calcInput.General.ClimateZone).SolarRadiation.Months[0].AvgTemp;
				num9 = num8 - parameterNiESM * num7 * section.Area.HeatedArea;
				num10 = num9 * calcData.Part1ESM / 100.0 / (calcData.TransmitTempEfficiencyESM / 100.0 * (calcData.SupplyNetEfficiencyESM / 100.0) * (calcData.AutomaticESM / 100.0) * (calcData.EnergyManagementESM / 100.0) * (calcData.GeneratorHeatEfficiency1ESM / 100.0));
				if (double.IsInfinity(num10) || double.IsNaN(num10))
				{
					num10 = 0.0;
				}
				num11 = num9 * calcData.Part2ESM / 100.0 / (calcData.TransmitTempEfficiency2ESM / 100.0 * (calcData.SupplyNetEfficiency2ESM / 100.0) * (calcData.Automatic2ESM / 100.0) * (calcData.EnergyManagement2ESM / 100.0) * (calcData.GeneratorHeatEfficiency2ESM / 100.0));
				if (double.IsInfinity(num11) || double.IsNaN(num11))
				{
					num11 = 0.0;
				}
				section.Area.ETlineData.MonthJanuaryHeatingEnergy.ESM = num10 + num11;
				section.Area.ETlineData.MonthJanuaryOuterTemp.ESM = PreferencesManager.GetClimateZoneParams(calcInput.General.ClimateZone).SolarRadiation.Months[0].AvgTemp;
			}
			if (item.Month == Month.March)
			{
				double num12 = monthData.NetEnergyQnd - monthData.ParameterNi * num2 * section.Area.HeatedArea;
				double num13 = num12 * calcData.Part1Actual / 100.0 / (calcData.TransmitTempEfficiencyActual / 100.0 * (calcData.SupplyNetEfficiencyActual / 100.0) * (calcData.AutomaticActual / 100.0) * (calcData.EnergyManagementActual / 100.0) * (calcData.GeneratorHeatEfficiency1Actual / 100.0));
				if (double.IsInfinity(num13) || double.IsNaN(num13))
				{
					num13 = 0.0;
				}
				double num14 = num12 * calcData.Part2Actual / 100.0 / (calcData.TransmitTempEfficiency2Actual / 100.0 * (calcData.SupplyNetEfficiency2Actual / 100.0) * (calcData.Automatic2Actual / 100.0) * (calcData.EnergyManagement2Actual / 100.0) * (calcData.GeneratorHeatEfficiency2Actual / 100.0));
				if (double.IsInfinity(num14) || double.IsNaN(num14))
				{
					num14 = 0.0;
				}
				section.Area.ETlineData.MonthMarchHeatingEnergy.Actual = num13 + num14;
				section.Area.ETlineData.MonthMarchOuterTemp.Actual = PreferencesManager.GetClimateZoneParams(calcInput.General.ClimateZone).SolarRadiation.Months[2].AvgTemp;
				num12 = num5 - parameterNiBaseLine * num4 * section.Area.HeatedArea;
				num13 = num12 * calcData.Part1BaseLine / 100.0 / (calcData.TransmitTempEfficiencyBaseLine / 100.0 * (calcData.SupplyNetEfficiencyBaseLine / 100.0) * (calcData.AutomaticBaseLine / 100.0) * (calcData.EnergyManagementBaseLine / 100.0) * (calcData.GeneratorHeatEfficiency1BaseLine / 100.0));
				if (double.IsInfinity(num13) || double.IsNaN(num13))
				{
					num13 = 0.0;
				}
				num14 = num12 * calcData.Part2BaseLine / 100.0 / (calcData.TransmitTempEfficiency2BaseLine / 100.0 * (calcData.SupplyNetEfficiency2BaseLine / 100.0) * (calcData.Automatic2BaseLine / 100.0) * (calcData.EnergyManagement2BaseLine / 100.0) * (calcData.GeneratorHeatEfficiency2BaseLine / 100.0));
				section.Area.ETlineData.MonthMarchHeatingEnergy.BaseLine = num13 + num14;
				if (double.IsInfinity(num14) || double.IsNaN(num14))
				{
					num14 = 0.0;
				}
				section.Area.ETlineData.MonthMarchHeatingEnergy.BaseLine = num13 + num14;
				section.Area.ETlineData.MonthMarchOuterTemp.BaseLine = PreferencesManager.GetClimateZoneParams(calcInput.General.ClimateZone).SolarRadiation.Months[2].AvgTemp;
				num12 = num8 - parameterNiESM * num7 * section.Area.HeatedArea;
				num13 = num12 * calcData.Part1ESM / 100.0 / (calcData.TransmitTempEfficiencyESM / 100.0 * (calcData.SupplyNetEfficiencyESM / 100.0) * (calcData.AutomaticESM / 100.0) * (calcData.EnergyManagementESM / 100.0) * (calcData.GeneratorHeatEfficiency1ESM / 100.0));
				if (double.IsInfinity(num13) || double.IsNaN(num13))
				{
					num13 = 0.0;
				}
				num14 = num12 * calcData.Part2ESM / 100.0 / (calcData.TransmitTempEfficiency2ESM / 100.0 * (calcData.SupplyNetEfficiency2ESM / 100.0) * (calcData.Automatic2ESM / 100.0) * (calcData.EnergyManagement2ESM / 100.0) * (calcData.GeneratorHeatEfficiency2ESM / 100.0));
				if (double.IsInfinity(num14) || double.IsNaN(num14))
				{
					num14 = 0.0;
				}
				section.Area.ETlineData.MonthMarchHeatingEnergy.ESM = num13 + num14;
				section.Area.ETlineData.MonthMarchOuterTemp.ESM = PreferencesManager.GetClimateZoneParams(calcInput.General.ClimateZone).SolarRadiation.Months[2].AvgTemp;
			}
			monthData.NetEnergyQnd = 0.0;
			monthData.NetEnergyQnd = (monthData.ParameterQht - monthData.ParameterNi * monthData.ParameterQgn) / section.Area.HeatedArea - monthData.ParameterNi * num2;
		}
		GetLightsAndDevicesInputs(calcData);
		MonthDataList = list;
		if (zone.HasRefenceValues)
		{
			double num15 = list7.Aggregate(0.0, (double num25, double item) => num25 + item);
			double num16 = list2.Aggregate(0.0, (double num25, double item) => num25 + item);
			calcData.ResulNoInputsNetEnergyRef1 = CheckForNaN(num16 / section.Area.HeatedArea - num15);
			double num17 = list8.Aggregate(0.0, (double num25, double item) => num25 + item);
			double num18 = list3.Aggregate(0.0, (double num25, double item) => num25 + item);
			calcData.ResulNoInputsNetEnergyRef2 = CheckForNaN(num18 / section.Area.HeatedArea - num17);
		}
		double num19 = list9.Aggregate(0.0, (double num25, double item) => num25 + item);
		double num20 = list4.Aggregate(0.0, (double num25, double item) => num25 + item);
		calcData.ResulNoInputsNetEnergyActual = CheckForNaN(num20 / section.Area.HeatedArea - num19);
		double num21 = list10.Aggregate(0.0, (double num25, double item) => num25 + item);
		double num22 = list5.Aggregate(0.0, (double num25, double item) => num25 + item);
		calcData.ResulNoInputsNetEnergyBaseLine = CheckForNaN(num22 / section.Area.HeatedArea - num21);
		double num23 = list11.Aggregate(0.0, (double num25, double item) => num25 + item);
		double num24 = list6.Aggregate(0.0, (double num25, double item) => num25 + item);
		calcData.ResulNoInputsNetEnergyESM = CheckForNaN(num24 / section.Area.HeatedArea - num23);
	}

	public static void GetWeekHoursResultReferences(this CalculationData heatgCalc, Section section)
	{
		double num = 5.0 * section.CalcHours(section.HeatingSeasons.Heating.WorkBaseStart, section.HeatingSeasons.Heating.WorkBaseEnd);
		double num2 = section.CalcHours(section.HeatingSeasons.Heating.SunBaseStart, section.HeatingSeasons.Heating.SunBaseEnd);
		if (num2 > 0.0)
		{
			num = num2 + num;
		}
		double num3 = section.CalcHours(section.HeatingSeasons.Heating.SatBaseStart, section.HeatingSeasons.Heating.SatBaseEnd);
		if (num3 > 0.0)
		{
			num = num3 + num;
		}
		heatgCalc.WorkingScheduleRef = num;
		heatgCalc.WorkingScheduleRef2 = num;
	}

	public static void GetWeekHoursResultActual(this CalculationData heatgCalc, Section section)
	{
		double num = 5.0 * section.CalcHours(section.HeatingSeasons.Heating.WorkCurrentStart, section.HeatingSeasons.Heating.WorkCurrentEnd);
		double num2 = section.CalcHours(section.HeatingSeasons.Heating.SunCurrentStart, section.HeatingSeasons.Heating.SunCurrentEnd);
		if (num2 > 0.0)
		{
			num = num2 + num;
		}
		double num3 = section.CalcHours(section.HeatingSeasons.Heating.SatCurrentStart, section.HeatingSeasons.Heating.SatCurrentEnd);
		if (num3 > 0.0)
		{
			num = num3 + num;
		}
		heatgCalc.WorkingScheduleActual = num;
	}

	public static void GetWeekHoursResultBaseLine(this CalculationData heatgCalc, Section section)
	{
		double num = 5.0 * section.CalcHours(section.HeatingSeasons.Heating.WorkBaseStart, section.HeatingSeasons.Heating.WorkBaseEnd);
		double num2 = section.CalcHours(section.HeatingSeasons.Heating.SunBaseStart, section.HeatingSeasons.Heating.SunBaseEnd);
		if (num2 > 0.0)
		{
			num = num2 + num;
		}
		double num3 = section.CalcHours(section.HeatingSeasons.Heating.SatBaseStart, section.HeatingSeasons.Heating.SatBaseEnd);
		if (num3 > 0.0)
		{
			num = num3 + num;
		}
		heatgCalc.WorkingScheduleBaseLine = num;
	}

	public static void GetWeekHoursResultEsm(this CalculationData heatgCalc, Section section)
	{
		double num = 5.0 * section.CalcHours(section.HeatingSeasons.Heating.WorkEsmStart, section.HeatingSeasons.Heating.WorkEsmEnd);
		double num2 = section.CalcHours(section.HeatingSeasons.Heating.SunEsmStart, section.HeatingSeasons.Heating.SunEsmEnd);
		if (num2 > 0.0)
		{
			num = num2 + num;
		}
		double num3 = section.CalcHours(section.HeatingSeasons.Heating.SatEsmStart, section.HeatingSeasons.Heating.SatEsmEnd);
		if (num3 > 0.0)
		{
			num = num3 + num;
		}
		heatgCalc.WorkingScheduleESM = num;
	}

	private static int OccupantHours(Section section, MonthlyDays month)
	{
		return month.WorkDays * (section.HeatingSeasons.Occupants.WorkCurrentEnd - section.HeatingSeasons.Occupants.WorkCurrentStart) + month.Sundays * (section.HeatingSeasons.Occupants.SunCurrentEnd - section.HeatingSeasons.Occupants.SunCurrentStart) + month.Saturdays * (section.HeatingSeasons.Occupants.SatCurrentEnd - section.HeatingSeasons.Occupants.SatCurrentStart);
	}

	private static void GetTestValue(Section section, CalculationInput calcInput, MonthData monthData, MonthlyDays month)
	{
		monthData.Month = month;
		monthData.AvgTemp = PreferencesManager.GetClimateZoneParams(calcInput.General.ClimateZone).SolarRadiation.Months[(int)month.Month].AvgTemp;
		monthData.ParameterHtr = section.Test.ParameterHtr;
		monthData.ParameterHve = section.Test.ParameterHve;
		monthData.ParamHd = section.Test.ParameterHd;
		monthData.ParamHg = section.Test.ParameterHg;
		monthData.ParamHu = section.Test.ParameterHu;
	}

	private static void CalculateActual(CalculationData calcData, Section section, CalculationInput calcInput, MonthData monthData, MonthlyDays month, double latentHeatPerMonth)
	{
		monthData.ParameterQtr = CalculateParameterQtr(calcData, section, calcInput.General.ClimateZone, month, out var _);
		monthData.ParameterQve = CalculateParameterQve(section, calcInput.General.ClimateZone, calcData, month);
		monthData.ParameterQht = monthData.ParameterQtr + monthData.ParameterQve;
		monthData.ParameterQgn = CalculateParameterQgn(section, calcInput.General.ClimateZone, month) / 1000.0;
		monthData.ParameterGama = (monthData.ParameterQgn + latentHeatPerMonth * section.Area.HeatedArea) / monthData.ParameterQht;
		monthData.ParameterNi = CalculateParameterNign(calcData, calcInput.General.ClimateZone, month, monthData.ParameterGama, section);
		monthData.NetEnergyQnd = monthData.ParameterQht - monthData.ParameterNi * monthData.ParameterQgn;
	}

	private static void CalculateLightsAndDevicesInputs(CalculationData lightsAndDevicesCalculationData, MonthlyDays month, double parameterEtaRef1, double parameterEtaRef2, double parameterEta, double parameterEtaBaseLine, double parameterEtaESM)
	{
		if (lightsAndDevicesCalculationData.Lights.ByMonths)
		{
			if (currentZone.HasRefenceValues)
			{
				double num = lightsAndDevicesCalculationData.Lights.Heating.PowerRef1 * (lightsAndDevicesCalculationData.Lights.Heating.WorkScheduleRef1 * month.Weeks) / 1000.0;
				LigthsListRef1.Add(num * parameterEtaRef1);
				double num2 = lightsAndDevicesCalculationData.Lights.Heating.PowerRef2 * (lightsAndDevicesCalculationData.Lights.Heating.WorkScheduleRef2 * month.Weeks) / 1000.0;
				LigthsListRef2.Add(num2 * parameterEtaRef2);
			}
			double num3 = CalcAvgMonthPower(lightsAndDevicesCalculationData.Lights.Actual, month) * (weekRegime * month.Weeks) / 1000.0;
			LigthsList.Add(num3 * parameterEta);
			double num4 = CalcAvgMonthPower(lightsAndDevicesCalculationData.Lights.BaseLine, month) * (weekRegime * month.Weeks) / 1000.0;
			LigthsListBaseLine.Add(num4 * parameterEtaBaseLine);
			double num5 = CalcAvgMonthPower(lightsAndDevicesCalculationData.Lights.Esm, month) * (weekRegime * month.Weeks) / 1000.0;
			LigthsListESM.Add(num5 * parameterEtaESM);
		}
		else
		{
			if (currentZone.HasRefenceValues)
			{
				double num6 = lightsAndDevicesCalculationData.Lights.Heating.PowerRef1 * (lightsAndDevicesCalculationData.Lights.Heating.WorkScheduleRef1 * month.Weeks) / 1000.0;
				LigthsListRef1.Add(num6 * parameterEtaRef1);
				double num7 = lightsAndDevicesCalculationData.Lights.Heating.PowerRef2 * (lightsAndDevicesCalculationData.Lights.Heating.WorkScheduleRef2 * month.Weeks) / 1000.0;
				LigthsListRef2.Add(num7 * parameterEtaRef2);
			}
			double num8 = lightsAndDevicesCalculationData.Lights.Heating.PowerActual * (lightsAndDevicesCalculationData.Lights.Heating.WorkScheduleActual * month.Weeks) / 1000.0;
			LigthsList.Add(num8 * parameterEta);
			double num9 = lightsAndDevicesCalculationData.Lights.Heating.PowerBaseLine * (lightsAndDevicesCalculationData.Lights.Heating.WorkScheduleBaseLine * month.Weeks) / 1000.0;
			LigthsListBaseLine.Add(num9 * parameterEtaBaseLine);
			double num10 = lightsAndDevicesCalculationData.Lights.Heating.PowerESM * (lightsAndDevicesCalculationData.Lights.Heating.WorkScheduleESM * month.Weeks) / 1000.0;
			LigthsListESM.Add(num10 * parameterEtaESM);
		}
		if (lightsAndDevicesCalculationData.BalancedDevices.ByMonths)
		{
			if (currentZone.HasRefenceValues)
			{
				double num11 = lightsAndDevicesCalculationData.BalancedDevices.Heating.PowerRef1 * (lightsAndDevicesCalculationData.BalancedDevices.Heating.WorkScheduleRef1 * month.Weeks) / 1000.0;
				DevicesRef1.Add(num11 * parameterEtaRef1);
				double num12 = lightsAndDevicesCalculationData.BalancedDevices.Heating.PowerRef2 * (lightsAndDevicesCalculationData.BalancedDevices.Heating.WorkScheduleRef2 * month.Weeks) / 1000.0;
				DevicesRef2.Add(num12 * parameterEtaRef2);
			}
			double num13 = CalcAvgMonthPower(lightsAndDevicesCalculationData.BalancedDevices.Actual, month) * (weekRegime * month.Weeks) / 1000.0;
			DevicesList.Add(num13 * parameterEta);
			double num14 = CalcAvgMonthPower(lightsAndDevicesCalculationData.BalancedDevices.BaseLine, month) * (weekRegime * month.Weeks) / 1000.0;
			DevicesListBaseLine.Add(num14 * parameterEtaBaseLine);
			double num15 = CalcAvgMonthPower(lightsAndDevicesCalculationData.BalancedDevices.Esm, month) * (weekRegime * month.Weeks) / 1000.0;
			DevicesListESM.Add(num15 * parameterEtaESM);
		}
		else
		{
			if (currentZone.HasRefenceValues)
			{
				double num16 = lightsAndDevicesCalculationData.BalancedDevices.Heating.PowerRef1 * (lightsAndDevicesCalculationData.BalancedDevices.Heating.WorkScheduleRef1 * month.Weeks) / 1000.0;
				DevicesRef1.Add(num16 * parameterEtaRef1);
				double num17 = lightsAndDevicesCalculationData.BalancedDevices.Heating.PowerRef2 * (lightsAndDevicesCalculationData.BalancedDevices.Heating.WorkScheduleRef2 * month.Weeks) / 1000.0;
				DevicesRef2.Add(num17 * parameterEtaRef2);
			}
			double num18 = lightsAndDevicesCalculationData.BalancedDevices.Heating.PowerActual * (lightsAndDevicesCalculationData.BalancedDevices.Heating.WorkScheduleActual * month.Weeks) / 1000.0;
			DevicesList.Add(num18 * parameterEta);
			double num19 = lightsAndDevicesCalculationData.BalancedDevices.Heating.PowerBaseLine * (lightsAndDevicesCalculationData.BalancedDevices.Heating.WorkScheduleBaseLine * month.Weeks) / 1000.0;
			DevicesListBaseLine.Add(num19 * parameterEtaBaseLine);
			double num20 = lightsAndDevicesCalculationData.BalancedDevices.Heating.PowerESM * (lightsAndDevicesCalculationData.BalancedDevices.Heating.WorkScheduleESM * month.Weeks) / 1000.0;
			DevicesListESM.Add(num20 * parameterEtaESM);
		}
	}

	private static void GetLightsAndDevicesInputs(CalculationData calcData)
	{
		if (currentZone.HasRefenceValues)
		{
			double resulLightInputsRef = SumItemsList(LigthsListRef1);
			calcData.ResulLightInputsRef1 = resulLightInputsRef;
			LigthsListRef1.Clear();
			double resulLightInputsref = SumItemsList(LigthsListRef2);
			calcData.ResulLightInputsref2 = resulLightInputsref;
			LigthsListRef2.Clear();
		}
		double resulLightInputsActual = SumItemsList(LigthsList);
		calcData.ResulLightInputsActual = resulLightInputsActual;
		LigthsList.Clear();
		double resulLightInputsBaseLine = SumItemsList(LigthsListBaseLine);
		calcData.ResulLightInputsBaseLine = resulLightInputsBaseLine;
		LigthsListBaseLine.Clear();
		double resulLightInputsESM = SumItemsList(LigthsListESM);
		calcData.ResulLightInputsESM = resulLightInputsESM;
		LigthsListESM.Clear();
		if (currentZone.HasRefenceValues)
		{
			double num = DevicesRef1.Aggregate(0.0, (double current, double item) => current + item);
			if (double.IsNaN(num) || double.IsInfinity(num))
			{
				num = 0.0;
			}
			calcData.ResulAppliancesInputsRef1 = num;
			DevicesRef1.Clear();
			double num2 = DevicesRef2.Aggregate(0.0, (double current, double item) => current + item);
			if (double.IsNaN(num2) || double.IsInfinity(num2))
			{
				num2 = 0.0;
			}
			calcData.ResulAppliancesInputsRef2 = num2;
			DevicesRef2.Clear();
		}
		double num3 = DevicesList.Aggregate(0.0, (double current, double item) => current + item);
		if (double.IsNaN(num3) || double.IsInfinity(num3))
		{
			num3 = 0.0;
		}
		calcData.ResulAppliancesInputsActual = num3;
		DevicesList.Clear();
		double num4 = DevicesListBaseLine.Aggregate(0.0, (double current, double item) => current + item);
		if (double.IsNaN(num4) || double.IsInfinity(num4))
		{
			num4 = 0.0;
		}
		calcData.ResulAppliancesInputsBaseLine = num4;
		DevicesListBaseLine.Clear();
		double num5 = DevicesListESM.Aggregate(0.0, (double current, double item) => current + item);
		if (double.IsNaN(num5) || double.IsInfinity(num5))
		{
			num5 = 0.0;
		}
		calcData.ResulAppliancesInputsESM = num5;
		DevicesListESM.Clear();
	}

	private static double SumItemsList(List<double> itemsList)
	{
		double num = itemsList.Aggregate(0.0, (double current, double item) => current + item);
		if (double.IsNaN(num) || double.IsInfinity(num))
		{
			num = 0.0;
		}
		return num;
	}

	private static double CalculateParameterNign(CalculationData calculationdata, ClimateZones climateZone, MonthlyDays month, double gamma, Section section)
	{
		double num = CalculateaH(calculationdata, section, climateZone, month);
		if (gamma > 0.0 && Math.Abs(gamma - 1.0) > 0.01)
		{
			return (1.0 - Math.Pow(gamma, num)) / (1.0 - Math.Pow(gamma, num + 1.0));
		}
		if (gamma < 0.0)
		{
			return 1.0;
		}
		if (Math.Abs(gamma - 1.0) < 0.01)
		{
			return num / (num + 1.0);
		}
		return 0.0;
	}

	private static double CalculateaH(CalculationData calculationdata, Section section, ClimateZones climateZone, MonthlyDays month)
	{
		double avgTemp = PreferencesManager.GetClimateZoneParams(climateZone).SolarRadiation.Months[(int)month.Month].AvgTemp;
		double averageInnerHeatTemp = CalculateAverageHeatTempCurrent(section, calculationdata, month);
		double num = SumWallDirecrionsHu1(section, avgTemp, averageInnerHeatTemp);
		double num2 = CalcCeilingsParameterHu2(section.Roof.Current, avgTemp, averageInnerHeatTemp);
		double num3 = CalcFloorsParameterHu3(section.Floor.Current, avgTemp, averageInnerHeatTemp);
		double num4 = CalculateParameterHdCurrent(section) + CalculateParameterHgCurrent(section) + (num + num2 + num3);
		double num5 = CalcParameterHve(section, calculationdata);
		double num6 = section.Area.HeatedArea * section.Area.HeatCapacity / (num4 + num5);
		return 1.0 + num6 / 15.0;
	}

	private static double CalculateParameterQve(Section section, ClimateZones climateZone, CalculationData calculationData, MonthlyDays month)
	{
		return CalcParameterHve(section, calculationData) * (CalcAvgProjectTemp(section, climateZone, calculationData, month) + CalcAvgNonProjectTemp(section, climateZone, calculationData, month)) / 1000.0;
	}

	private static double CalcAvgProjectTemp(Section section, ClimateZones climateZone, CalculationData calculationData, MonthlyDays month)
	{
		double avgTemp = PreferencesManager.GetClimateZoneParams(climateZone).SolarRadiation.Months[(int)month.Month].AvgTemp;
		int num = month.WorkDays * (section.HeatingSeasons.Heating.WorkCurrentEnd - section.HeatingSeasons.Heating.WorkCurrentStart);
		int num2 = month.Sundays * (section.HeatingSeasons.Heating.SunCurrentEnd - section.HeatingSeasons.Heating.SunCurrentStart);
		int num3 = month.Saturdays * (section.HeatingSeasons.Heating.SatCurrentEnd - section.HeatingSeasons.Heating.SatCurrentStart);
		return (calculationData.ProjectTemperatureActual - avgTemp) * (double)(num + num3 + num2);
	}

	private static double CalcAvgNonProjectTemp(Section section, ClimateZones climateZone, CalculationData calculationData, MonthlyDays month)
	{
		double avgTemp = PreferencesManager.GetClimateZoneParams(climateZone).SolarRadiation.Months[(int)month.Month].AvgTemp;
		int num = month.WorkDays * (24 - (section.HeatingSeasons.Heating.WorkCurrentEnd - section.HeatingSeasons.Heating.WorkCurrentStart));
		int num2 = month.Saturdays * (24 - (section.HeatingSeasons.Heating.SatCurrentEnd - section.HeatingSeasons.Heating.SatCurrentStart));
		int num3 = month.Sundays * (24 - (section.HeatingSeasons.Heating.SunCurrentEnd - section.HeatingSeasons.Heating.SunCurrentStart));
		int num4 = month.Holydays * 24;
		return (calculationData.NonProjectTemperatureActual - avgTemp) * (double)(num + num2 + num3 + num4);
	}

	private static double CalcParameterHve(Section section, CalculationData calculationData)
	{
		section.Test.ParameterHve = section.Area.HeatedVolume * calculationData.InfiltracionActual * 0.34;
		return section.Test.ParameterHve;
	}

	private static double CalculateParameterQtr(CalculationData calculationData, Section section, ClimateZones climateZone, MonthlyDays month, out double parameterHtr)
	{
		double avgTemp = PreferencesManager.GetClimateZoneParams(climateZone).SolarRadiation.Months[(int)month.Month].AvgTemp;
		double averageInnerHeatTemp = CalculateAverageHeatTempCurrent(section, calculationData, month);
		parameterHtr = CalculateParameterHtr(section, avgTemp, averageInnerHeatTemp);
		section.Test.ParameterHtr = parameterHtr;
		return section.Test.ParameterHtr * (CalcAvgProjectTemp(section, climateZone, calculationData, month) + CalcAvgNonProjectTemp(section, climateZone, calculationData, month)) / 1000.0;
	}

	private static double CalculateParameterHtr(Section section, double averageMontlyTemp, double averageInnerHeatTemp)
	{
		double num = SumWallDirecrionsHu1(section, averageMontlyTemp, averageInnerHeatTemp);
		double num2 = CalcCeilingsParameterHu2(section.Roof.Current, averageMontlyTemp, averageInnerHeatTemp);
		double num3 = CalcFloorsParameterHu3(section.Floor.Current, averageMontlyTemp, averageInnerHeatTemp);
		section.Test.ParameterHu = num + num2 + num3;
		section.Test.ParameterHd = CalculateParameterHdCurrent(section);
		section.Test.ParameterHg = CalculateParameterHgCurrent(section);
		return section.Test.ParameterHd + section.Test.ParameterHg + section.Test.ParameterHu;
	}

	private static double CalculateParameterHdCurrent(Section section)
	{
		return SumAllDirectionsWallsCurrent(section) + SumAllDirectionWindowsCurrent(section) + SumNonTrasparentRoof(section.Roof.Current) + SumTrasparentRoof(section.Roof.Current);
	}

	private static double CalculateParameterHgCurrent(Section section)
	{
		return section.Floor.Current.AccumulateFloorA * section.Floor.Current.AccumulateFloorU;
	}

	private static double SumAllDirectionsWallsCurrent(Section section)
	{
		north = CalculateItemsWalls(section.NorthWalls.Current);
		northEast = CalculateItemsWalls(section.NorthEastWalls.Current);
		east = CalculateItemsWalls(section.EastWalls.Current);
		southEast = CalculateItemsWalls(section.SouthEastWalls.Current);
		south = CalculateItemsWalls(section.SouthWalls.Current);
		southWest = CalculateItemsWalls(section.SouthWestWalls.Current);
		west = CalculateItemsWalls(section.WestWalls.Current);
		northWest = CalculateItemsWalls(section.NorthWestWalls.Current);
		return north + northEast + east + southEast + south + southWest + west + northWest;
	}

	private static double CalculateItemsWalls(Walls walls)
	{
		double num = walls.OuterA1 * walls.OuterU1;
		double num2 = walls.OuterA2 * walls.OuterU2;
		double num3 = walls.OuterA3 * walls.OuterU3;
		double num4 = walls.OuterA4 * walls.OuterU4;
		double num5 = walls.OuterA5 * walls.OuterU5;
		double num6 = walls.OuterA6 * walls.OuterU6;
		double num7 = num + num2 + num3 + num4 + num5 + num6;
		num = walls.Outer1.SumL;
		num2 = walls.Outer2.SumL;
		num3 = walls.Outer3.SumL;
		num4 = walls.Outer4.SumL;
		num5 = walls.Outer5.SumL;
		num6 = walls.Outer6.SumL;
		double num8 = num + num2 + num3 + num4 + num5 + num6;
		num = walls.Outer1.SumX;
		num2 = walls.Outer2.SumX;
		num3 = walls.Outer3.SumX;
		num4 = walls.Outer4.SumX;
		num5 = walls.Outer5.SumX;
		num6 = walls.Outer6.SumX;
		double num9 = num + num2 + num3 + num4 + num5 + num6;
		return num7 + num8 + num9;
	}

	private static double SumAllDirectionWindowsCurrent(Section section)
	{
		north = section.NorthWalls.Current.AccumulateWindowU * section.NorthWalls.Current.AccumulateWindowA;
		northEast = section.NorthEastWalls.Current.AccumulateWindowU * section.NorthEastWalls.Current.AccumulateWindowA;
		east = section.EastWalls.Current.AccumulateWindowU * section.EastWalls.Current.AccumulateWindowA;
		southEast = section.SouthEastWalls.Current.AccumulateWindowU * section.SouthEastWalls.Current.AccumulateWindowA;
		south = section.SouthWalls.Current.AccumulateWindowU * section.SouthWalls.Current.AccumulateWindowA;
		southWest = section.SouthWestWalls.Current.AccumulateWindowU * section.SouthWestWalls.Current.AccumulateWindowA;
		west = section.WestWalls.Current.AccumulateWindowU * section.WestWalls.Current.AccumulateWindowA;
		northWest = section.NorthWestWalls.Current.AccumulateWindowU * section.NorthWestWalls.Current.AccumulateWindowA;
		return north + northEast + east + southEast + south + southWest + west + northWest;
	}

	private static double SumNonTrasparentRoof(Roof roof)
	{
		double num = roof.NonTransparentA1 * roof.NonTransparentU1;
		double num2 = roof.NonTransparentA2 * roof.NonTransparentU2;
		double num3 = roof.NonTransparentA3 * roof.NonTransparentU3;
		double num4 = roof.NonTransparentA4 * roof.NonTransparentU4;
		double num5 = roof.NonTransparentA5 * roof.NonTransparentU5;
		double num6 = roof.NonTransparentА6 * roof.NonTransparentU6;
		double num7 = roof.NonTransparentA7 * roof.NonTransparentU7;
		double num8 = roof.NonTransparentA8 * roof.NonTransparentU8;
		double num9 = roof.NonTransparentA9 * roof.NonTransparentU9;
		double num10 = num + num2 + num3 + num4 + num5 + num6 + num7 + num8 + num9;
		num = roof.NonTransparent1.SumL;
		num2 = roof.NonTransparent2.SumL;
		num3 = roof.NonTransparent3.SumL;
		num4 = roof.NonTransparent4.SumL;
		num5 = roof.NonTransparent5.SumL;
		num6 = roof.NonTransparent6.SumL;
		num7 = roof.NonTransparent7.SumL;
		num8 = roof.NonTransparent8.SumL;
		num9 = roof.NonTransparent9.SumL;
		double num11 = num + num2 + num3 + num4 + num5 + num6 + num7 + num8 + num9;
		num = roof.NonTransparent1.SumX;
		num2 = roof.NonTransparent2.SumX;
		num3 = roof.NonTransparent3.SumX;
		num4 = roof.NonTransparent4.SumX;
		num5 = roof.NonTransparent5.SumX;
		num6 = roof.NonTransparent6.SumX;
		num7 = roof.NonTransparent7.SumX;
		num8 = roof.NonTransparent8.SumX;
		num9 = roof.NonTransparent9.SumX;
		double num12 = num + num2 + num3 + num4 + num5 + num6 + num7 + num8 + num9;
		return num10 + num11 + num12;
	}

	private static double SumTrasparentRoof(Roof roof)
	{
		double num = roof.TransparentA1 * roof.TransparentU1;
		double num2 = roof.TransparentA2 * roof.TransparentU2;
		double num3 = roof.TransparentA3 * roof.TransparentU3;
		double num4 = roof.TransparentA4 * roof.TransparentU4;
		double num5 = roof.TransparentA5 * roof.TransparentU5;
		double num6 = roof.TransparentА6 * roof.TransparentU6;
		double num7 = roof.TransparentA7 * roof.TransparentU7;
		double num8 = roof.TransparentA8 * roof.TransparentU8;
		double num9 = roof.TransparentA9 * roof.TransparentU9;
		return num + num2 + num3 + num4 + num5 + num6 + num7 + num8 + num9;
	}

	private static double CalculateAverageHeatTempCurrent(Section section, CalculationData calculationData, MonthlyDays month)
	{
		int num = month.WorkDays * (section.HeatingSeasons.Heating.WorkCurrentEnd - section.HeatingSeasons.Heating.WorkCurrentStart);
		num = ((month.Sundays > 0) ? (month.Sundays * (section.HeatingSeasons.Heating.SunCurrentEnd - section.HeatingSeasons.Heating.SunCurrentStart) + num) : num);
		num = ((month.Saturdays > 0) ? (month.Saturdays * (section.HeatingSeasons.Heating.SatCurrentEnd - section.HeatingSeasons.Heating.SatCurrentStart) + num) : num);
		double projectTemperatureActual = calculationData.ProjectTemperatureActual;
		int num2 = month.WorkDays * (24 - (section.HeatingSeasons.Heating.WorkCurrentEnd - section.HeatingSeasons.Heating.WorkCurrentStart));
		num2 = month.Saturdays * (24 - (section.HeatingSeasons.Heating.SatCurrentEnd - section.HeatingSeasons.Heating.SatCurrentStart)) + num2;
		num2 = month.Sundays * (24 - (section.HeatingSeasons.Heating.SunCurrentEnd - section.HeatingSeasons.Heating.SunCurrentStart)) + num2;
		num2 = month.Holydays * 24 + num2;
		double nonProjectTemperatureActual = calculationData.NonProjectTemperatureActual;
		return ((double)num * projectTemperatureActual + (double)num2 * nonProjectTemperatureActual) / (double)(num + num2);
	}

	private static double SumWallDirecrionsHu1(Section section, double averageMontlyTemp, double averageInnerHeatTemp)
	{
		double num = CalcWallDirectionParameterHu1(section.NorthWalls.Current, averageMontlyTemp, averageInnerHeatTemp);
		double num2 = CalcWallDirectionParameterHu1(section.NorthWalls.Current, averageMontlyTemp, averageInnerHeatTemp);
		double num3 = CalcWallDirectionParameterHu1(section.NorthWalls.Current, averageMontlyTemp, averageInnerHeatTemp);
		double num4 = CalcWallDirectionParameterHu1(section.NorthWalls.Current, averageMontlyTemp, averageInnerHeatTemp);
		double num5 = CalcWallDirectionParameterHu1(section.NorthWalls.Current, averageMontlyTemp, averageInnerHeatTemp);
		double num6 = CalcWallDirectionParameterHu1(section.NorthWalls.Current, averageMontlyTemp, averageInnerHeatTemp);
		double num7 = CalcWallDirectionParameterHu1(section.NorthWalls.Current, averageMontlyTemp, averageInnerHeatTemp);
		double num8 = CalcWallDirectionParameterHu1(section.NorthWalls.Current, averageMontlyTemp, averageInnerHeatTemp);
		return num + num2 + num3 + num4 + num5 + num6 + num7 + num8;
	}

	private static double CalcWallDirectionParameterHu1(Walls wall, double averageMontlyTemp, double averageInnerHeatTemp)
	{
		double num = averageInnerHeatTemp - averageMontlyTemp;
		double innerA = wall.InnerA1;
		double innerU = wall.InnerU1;
		double num2 = averageInnerHeatTemp - (double)wall.InnerW1;
		double num3 = innerA * innerU * num2 / num;
		innerA = wall.InnerA2;
		innerU = wall.InnerU2;
		num2 = averageInnerHeatTemp - (double)wall.InnerW2;
		double num4 = innerA * innerU * num2 / num;
		innerA = wall.InnerA3;
		innerU = wall.InnerU3;
		num2 = averageInnerHeatTemp - (double)wall.InnerW3;
		double num5 = innerA * innerU * num2 / num;
		innerA = wall.InnerA4;
		innerU = wall.InnerU4;
		num2 = averageInnerHeatTemp - (double)wall.InnerW4;
		double num6 = innerA * innerU * num2 / num;
		innerA = wall.IneerA5;
		innerU = wall.IneerA5;
		num2 = averageInnerHeatTemp - (double)wall.InnerW5;
		double num7 = innerA * innerU * num2 / num;
		innerA = wall.InnerA6;
		innerU = wall.InnerU6;
		num2 = averageInnerHeatTemp - (double)wall.InnerW6;
		double num8 = innerA * innerU * num2 / num;
		return num3 + num4 + num5 + num6 + num7 + num8;
	}

	private static double CalcCeilingsParameterHu2(Roof roof, double averageMontlyTemp, double averageInnerHeatTemp)
	{
		double num = averageInnerHeatTemp - averageMontlyTemp;
		double ceilingA = roof.CeilingA1;
		double ceilingU = roof.CeilingU1;
		double num2 = averageInnerHeatTemp - (double)roof.CeilingW1;
		double num3 = ceilingA * ceilingU * num2 / num;
		ceilingA = roof.CeilingA2;
		ceilingU = roof.CeilingU2;
		num2 = averageInnerHeatTemp - (double)roof.CeilingW2;
		double num4 = ceilingA * ceilingU * num2 / num;
		ceilingA = roof.CeilingA3;
		ceilingU = roof.CeilingU3;
		num2 = averageInnerHeatTemp - (double)roof.CeilingW3;
		double num5 = ceilingA * ceilingU * num2 / num;
		ceilingA = roof.CeilingA4;
		ceilingU = roof.CeilingU4;
		num2 = averageInnerHeatTemp - (double)roof.CeilingW4;
		double num6 = ceilingA * ceilingU * num2 / num;
		ceilingA = roof.CeilingA5;
		ceilingU = roof.CeilingA5;
		num2 = averageInnerHeatTemp - (double)roof.CeilingW5;
		double num7 = ceilingA * ceilingU * num2 / num;
		ceilingA = roof.CeilingA6;
		ceilingU = roof.CeilingU6;
		num2 = averageInnerHeatTemp - (double)roof.CeilingW6;
		double num8 = ceilingA * ceilingU * num2 / num;
		return num3 + num4 + num5 + num6 + num7 + num8;
	}

	private static double CalcFloorsParameterHu3(Floor floor, double averageMontlyTemp, double averageInnerHeatTemp)
	{
		double num = averageInnerHeatTemp - averageMontlyTemp;
		double otherFloorA = floor.OtherFloorA1;
		double otherFloorU = floor.OtherFloorU1;
		double num2 = averageInnerHeatTemp - (double)floor.OtherFloorW1;
		double num3 = otherFloorA * otherFloorU * num2 / num;
		otherFloorA = floor.OtherFloorA2;
		otherFloorU = floor.OtherFloorU2;
		num2 = averageInnerHeatTemp - (double)floor.OtherFloorW2;
		double num4 = otherFloorA * otherFloorU * num2 / num;
		otherFloorA = floor.OtherFloorA3;
		otherFloorU = floor.OtherFloorU3;
		num2 = averageInnerHeatTemp - (double)floor.OtherFloorW3;
		double num5 = otherFloorA * otherFloorU * num2 / num;
		otherFloorA = floor.OtherFloorA4;
		otherFloorU = floor.OtherFloorU4;
		num2 = averageInnerHeatTemp - (double)floor.OtherFloorW4;
		double num6 = otherFloorA * otherFloorU * num2 / num;
		otherFloorA = floor.OtherFloorA5;
		otherFloorU = floor.OtherFloorU5;
		num2 = averageInnerHeatTemp - (double)floor.OtherFloorW5;
		double num7 = otherFloorA * otherFloorU * num2 / num;
		otherFloorA = floor.OtherFloorA6;
		otherFloorU = floor.OtherFloorU6;
		num2 = averageInnerHeatTemp - (double)floor.OtherFloorW6;
		double num8 = otherFloorA * otherFloorU * num2 / num;
		return num3 + num4 + num5 + num6 + num7 + num8;
	}

	private static double CalculateParameterQgn(Section section, ClimateZones climateZone, MonthlyDays month)
	{
		int num = month.WorkDays * (section.HeatingSeasons.Heating.WorkCurrentEnd - section.HeatingSeasons.Heating.WorkCurrentStart);
		num = ((month.Sundays > 0) ? (month.Sundays * (section.HeatingSeasons.Heating.SunCurrentEnd - section.HeatingSeasons.Heating.SunCurrentStart) + num) : num);
		num = ((month.Saturdays > 0) ? (month.Saturdays * (section.HeatingSeasons.Heating.SatCurrentEnd - section.HeatingSeasons.Heating.SatCurrentStart) + num) : num);
		int num2 = month.WorkDays * (24 - (section.HeatingSeasons.Heating.WorkCurrentEnd - section.HeatingSeasons.Heating.WorkCurrentStart));
		num2 = month.Saturdays * (24 - (section.HeatingSeasons.Heating.SatCurrentEnd - section.HeatingSeasons.Heating.SatCurrentStart)) + num2;
		num2 = month.Sundays * (24 - (section.HeatingSeasons.Heating.SunCurrentEnd - section.HeatingSeasons.Heating.SunCurrentStart)) + num2;
		num2 = month.Holydays * 24 + num2;
		return (CalculateNonTrasparentFsol(section, climateZone, month) + CalculateTrasparentFsol(section, climateZone, month)) * (double)(num + num2);
	}

	private static double CalculateTransparentFsol(double windowA, double windowG, double windowE, double sunShiningIntensity, bool horizontal = false)
	{
		decimal num = 4m * (decimal)windowE * 0.0000000567m * (decimal)Math.Pow(283.0, 3.0);
		double num2 = 0.04 * windowG * windowA * 11.0 * (double)num;
		double num3 = 0.5;
		if (horizontal)
		{
			num3 = 1.0;
		}
		return windowA * windowG * sunShiningIntensity - num3 * num2;
	}

	private static double CalculateTrasparentFsol(Section section, ClimateZones climateZone, MonthlyDays month)
	{
		SolarRadiationPerMonth solarRadiationPerMonth = PreferencesManager.GetClimateZoneParams(climateZone).SolarRadiation.Months[(int)month.Month];
		Walls current = section.NorthWalls.Current;
		double num = CalculateTransparentFsol(current.AccumulateWindowA, current.AccumulateWindowG, current.AccumulateWindowE, solarRadiationPerMonth.N);
		current = section.NorthEastWalls.Current;
		num += CalculateTransparentFsol(current.AccumulateWindowA, current.AccumulateWindowG, current.AccumulateWindowE, (solarRadiationPerMonth.N + solarRadiationPerMonth.E) / 2.0);
		current = section.EastWalls.Current;
		num += CalculateTransparentFsol(current.AccumulateWindowA, current.AccumulateWindowG, current.AccumulateWindowE, solarRadiationPerMonth.E);
		current = section.SouthEastWalls.Current;
		num += CalculateTransparentFsol(current.AccumulateWindowA, current.AccumulateWindowG, current.AccumulateWindowE, (solarRadiationPerMonth.S + solarRadiationPerMonth.E) / 2.0);
		current = section.SouthWalls.Current;
		num += CalculateTransparentFsol(current.AccumulateWindowA, current.AccumulateWindowG, current.AccumulateWindowE, solarRadiationPerMonth.S);
		current = section.SouthWestWalls.Current;
		num += CalculateTransparentFsol(current.AccumulateWindowA, current.AccumulateWindowG, current.AccumulateWindowE, (solarRadiationPerMonth.S + solarRadiationPerMonth.W) / 2.0);
		current = section.WestWalls.Current;
		num += CalculateTransparentFsol(current.AccumulateWindowA, current.AccumulateWindowG, current.AccumulateWindowE, solarRadiationPerMonth.W);
		current = section.NorthWestWalls.Current;
		num += CalculateTransparentFsol(current.AccumulateWindowA, current.AccumulateWindowG, current.AccumulateWindowE, (solarRadiationPerMonth.N + solarRadiationPerMonth.W) / 2.0);
		Roof current2 = section.Roof.Current;
		double num2 = CalculateTransparentFsol(current2.TransparentA1, current2.TransparentG1, current2.TransparentE1, solarRadiationPerMonth.N);
		num2 += CalculateTransparentFsol(current2.TransparentA2, current2.TransparentG2, current2.TransparentE2, (solarRadiationPerMonth.N + solarRadiationPerMonth.E) / 2.0);
		num2 += CalculateTransparentFsol(current2.TransparentA3, current2.TransparentG3, current2.TransparentE3, solarRadiationPerMonth.E);
		num2 += CalculateTransparentFsol(current2.TransparentA4, current2.TransparentG4, current2.TransparentE4, (solarRadiationPerMonth.S + solarRadiationPerMonth.E) / 2.0);
		num2 += CalculateTransparentFsol(current2.TransparentA5, current2.TransparentG5, current2.TransparentE5, solarRadiationPerMonth.S);
		num2 += CalculateTransparentFsol(current2.TransparentА6, current2.TransparentG6, current2.TransparentE6, (solarRadiationPerMonth.S + solarRadiationPerMonth.W) / 2.0);
		num2 += CalculateTransparentFsol(current2.TransparentA7, current2.TransparentG7, current2.TransparentE7, solarRadiationPerMonth.W);
		num2 += CalculateTransparentFsol(current2.TransparentA8, current2.TransparentG8, current2.TransparentE8, (solarRadiationPerMonth.N + solarRadiationPerMonth.W) / 2.0);
		num2 += CalculateTransparentFsol(current2.TransparentA9, current2.TransparentG9, current2.TransparentE9, solarRadiationPerMonth.H, horizontal: true);
		return num + num2;
	}

	private static double CalculateNonTransparentFsol(double outerWallAlfa, double outerWallU, double outerWallEpsi, double outerWallArea, double sunShiningIntensity, bool horizontal = false)
	{
		double num = outerWallAlfa * 0.04 * outerWallU * outerWallArea;
		decimal num2 = 4m * (decimal)outerWallEpsi * 0.0000000567m * (decimal)Math.Pow(283.0, 3.0);
		double num3 = 0.04 * outerWallU * outerWallArea * 11.0 * (double)num2;
		double num4 = 0.5;
		if (horizontal)
		{
			num4 = 1.0;
		}
		return num * sunShiningIntensity - num4 * num3;
	}

	private static double CalculateNonTrasparentFsol(Section section, ClimateZones climateZone, MonthlyDays month)
	{
		SolarRadiationPerMonth solarRadiationPerMonth = PreferencesManager.GetClimateZoneParams(climateZone).SolarRadiation.Months[(int)month.Month];
		Walls current = section.NorthWalls.Current;
		double num = CalculateNonTransparentFsol(current.AccumulateOuterAlfa, current.AccumulateOuterU, current.AccumulateOuterE, current.AccumulateOuterA, solarRadiationPerMonth.N);
		current = section.NorthEastWalls.Current;
		num += CalculateNonTransparentFsol(current.AccumulateOuterAlfa, current.AccumulateOuterU, current.AccumulateOuterE, current.AccumulateOuterA, (solarRadiationPerMonth.N + solarRadiationPerMonth.E) / 2.0);
		current = section.EastWalls.Current;
		num += CalculateNonTransparentFsol(current.AccumulateOuterAlfa, current.AccumulateOuterU, current.AccumulateOuterE, current.AccumulateOuterA, solarRadiationPerMonth.E);
		current = section.SouthEastWalls.Current;
		num += CalculateNonTransparentFsol(current.AccumulateOuterAlfa, current.AccumulateOuterU, current.AccumulateOuterE, current.AccumulateOuterA, (solarRadiationPerMonth.S + solarRadiationPerMonth.E) / 2.0);
		current = section.SouthWalls.Current;
		num += CalculateNonTransparentFsol(current.AccumulateOuterAlfa, current.AccumulateOuterU, current.AccumulateOuterE, current.AccumulateOuterA, solarRadiationPerMonth.S);
		current = section.SouthWestWalls.Current;
		num += CalculateNonTransparentFsol(current.AccumulateOuterAlfa, current.AccumulateOuterU, current.AccumulateOuterE, current.AccumulateOuterA, (solarRadiationPerMonth.S + solarRadiationPerMonth.W) / 2.0);
		current = section.WestWalls.Current;
		num += CalculateNonTransparentFsol(current.AccumulateOuterAlfa, current.AccumulateOuterU, current.AccumulateOuterE, current.AccumulateOuterA, solarRadiationPerMonth.W);
		current = section.NorthWestWalls.Current;
		num += CalculateNonTransparentFsol(current.AccumulateOuterAlfa, current.AccumulateOuterU, current.AccumulateOuterE, current.AccumulateOuterA, (solarRadiationPerMonth.N + solarRadiationPerMonth.W) / 2.0);
		Roof current2 = section.Roof.Current;
		double num2 = CalculateNonTransparentFsol(current2.AccumulateNonTransparentAlfa, current2.AccumulateNonTransparentU, current2.AccumulateNonTransparentE, current2.AccumulateNonTransparentA, solarRadiationPerMonth.H, horizontal: true);
		return num + num2;
	}

	private static int OccupantsHoursEsm(Section section, MonthlyDays month)
	{
		return month.WorkDays * (section.HeatingSeasons.Occupants.WorkEsmEnd - section.HeatingSeasons.Occupants.WorkEsmStart) + month.Sundays * (section.HeatingSeasons.Occupants.SunEsmEnd - section.HeatingSeasons.Occupants.SunEsmStart) + month.Saturdays * (section.HeatingSeasons.Occupants.SatEsmEnd - section.HeatingSeasons.Occupants.SatEsmStart);
	}

	private static double CalculateEsm(this CalculationData heatingAndCoolingCalculations, Section section, CalculationInput calcInput, MonthlyDays month, double latentHeatPerMonth)
	{
		double num = CalculateParameterQtrEsm(heatingAndCoolingCalculations, section, calcInput.General.ClimateZone, month);
		double num2 = CalculateParameterQveEsm(section, calcInput.General.ClimateZone, heatingAndCoolingCalculations, month);
		double num3 = num + num2;
		double num4 = CalculateParameterQgnEsm(section, calcInput.General.ClimateZone, month) / 1000.0;
		double gamma = (num4 + latentHeatPerMonth * section.Area.HeatedArea) / num3;
		parameterNiESM = CalculateParameterNiEsm(heatingAndCoolingCalculations, calcInput.General.ClimateZone, month, gamma, section);
		return num3 - parameterNiESM * num4;
	}

	private static double CalculateParameterNiEsm(CalculationData calculationdata, ClimateZones climateZone, MonthlyDays month, double gamma, Section section)
	{
		double num = CalculateaHesm(calculationdata, section, climateZone, month);
		if (gamma > 0.0 && Math.Abs(gamma - 1.0) > 0.01)
		{
			return (1.0 - Math.Pow(gamma, num)) / (1.0 - Math.Pow(gamma, num + 1.0));
		}
		if (gamma < 0.0)
		{
			return 1.0;
		}
		if (Math.Abs(gamma - 1.0) < 0.01)
		{
			return num / (num + 1.0);
		}
		return 0.0;
	}

	private static double CalculateaHesm(CalculationData calculationdata, Section section, ClimateZones climateZone, MonthlyDays month)
	{
		double avgTemp = PreferencesManager.GetClimateZoneParams(climateZone).SolarRadiation.Months[(int)month.Month].AvgTemp;
		double averageInnerHeatTemp = CalculateAverageHeatTempEsm(section, calculationdata, month);
		double num = SumWallDirecrionsHu1Esm(section, avgTemp, averageInnerHeatTemp);
		double num2 = CalcCeilingsParameterHu2(section.Roof.Esm, avgTemp, averageInnerHeatTemp);
		double num3 = CalcFloorsParameterHu3(section.Floor.Esm, avgTemp, averageInnerHeatTemp);
		double num4 = CalculateParameterHdEsm(section) + CalculateParameterHgEsm(section) + (num + num2 + num3);
		double num5 = CalcParameterHveEsm(section, calculationdata);
		double num6 = section.Area.HeatedArea * section.Area.HeatCapacity / (num4 + num5);
		return 1.0 + num6 / 15.0;
	}

	private static double CalculateParameterQveEsm(Section section, ClimateZones climateZone, CalculationData calculationData, MonthlyDays month)
	{
		double avgTemp = PreferencesManager.GetClimateZoneParams(climateZone).SolarRadiation.Months[(int)month.Month].AvgTemp;
		return CalcParameterHveEsm(section, calculationData) * (CalcAvgProjectTempEsm(section, avgTemp, calculationData, month) + CalcAvgNonProjectTempEsm(section, avgTemp, calculationData, month)) / 1000.0;
	}

	private static double CalculateParameterHtrEsm(Section section, double averageMontlyTemp, double averageInnerHeatTemp)
	{
		double num = SumWallDirecrionsHu1Esm(section, averageMontlyTemp, averageInnerHeatTemp);
		double num2 = CalcCeilingsParameterHu2(section.Roof.Current, averageMontlyTemp, averageInnerHeatTemp);
		double num3 = CalcFloorsParameterHu3(section.Floor.Current, averageMontlyTemp, averageInnerHeatTemp);
		section.Test.ParameterHu = num + num2 + num3;
		section.Test.ParameterHd = CalculateParameterHdEsm(section);
		section.Test.ParameterHg = CalculateParameterHgEsm(section);
		return section.Test.ParameterHd + section.Test.ParameterHg + section.Test.ParameterHu;
	}

	private static double CalcParameterHveEsm(Section section, CalculationData heatingAndCoolingCalculations)
	{
		return section.Area.HeatedVolume * heatingAndCoolingCalculations.InfiltracionESM * 0.34;
	}

	private static double CalculateParameterQtrEsm(CalculationData claculationdata, Section section, ClimateZones climateZone, MonthlyDays month)
	{
		double avgTemp = PreferencesManager.GetClimateZoneParams(climateZone).SolarRadiation.Months[(int)month.Month].AvgTemp;
		double averageInnerHeatTemp = CalculateAverageHeatTempEsm(section, claculationdata, month);
		double num = SumWallDirecrionsHu1Esm(section, avgTemp, averageInnerHeatTemp);
		double num2 = CalcCeilingsParameterHu2(section.Roof.Esm, avgTemp, averageInnerHeatTemp);
		double num3 = CalcFloorsParameterHu3(section.Floor.Esm, avgTemp, averageInnerHeatTemp);
		double num4 = CalculateParameterHdEsm(section) + CalculateParameterHgEsm(section) + (num + num2 + num3);
		return num4 * (CalcAvgProjectTempEsm(section, avgTemp, claculationdata, month) + CalcAvgNonProjectTempEsm(section, avgTemp, claculationdata, month)) / 1000.0;
	}

	private static double CalculateAverageHeatTempEsm(Section section, CalculationData calculationData, MonthlyDays month)
	{
		int num = month.WorkDays * (section.HeatingSeasons.Heating.WorkEsmEnd - section.HeatingSeasons.Heating.WorkEsmStart);
		num = ((month.Sundays > 0) ? (month.Sundays * (section.HeatingSeasons.Heating.SunEsmEnd - section.HeatingSeasons.Heating.SunEsmStart) + num) : num);
		num = ((month.Saturdays > 0) ? (month.Saturdays * (section.HeatingSeasons.Heating.SatEsmEnd - section.HeatingSeasons.Heating.SatEsmStart) + num) : num);
		double projectTemperatureESM = calculationData.ProjectTemperatureESM;
		int num2 = month.WorkDays * (24 - (section.HeatingSeasons.Heating.WorkEsmEnd - section.HeatingSeasons.Heating.WorkEsmStart));
		num2 = month.Saturdays * (24 - (section.HeatingSeasons.Heating.SatEsmEnd - section.HeatingSeasons.Heating.SatEsmStart)) + num2;
		num2 = month.Sundays * (24 - (section.HeatingSeasons.Heating.SunEsmEnd - section.HeatingSeasons.Heating.SunEsmStart)) + num2;
		num2 = month.Holydays * 24 + num2;
		double nonProjectTemperatureESM = calculationData.NonProjectTemperatureESM;
		return ((double)num * projectTemperatureESM + (double)num2 * nonProjectTemperatureESM) / (double)(num + num2);
	}

	private static double CalcAvgNonProjectTempEsm(Section section, double averageMontlyTemp, CalculationData calculationData, MonthlyDays month)
	{
		int num = month.WorkDays * (24 - (section.HeatingSeasons.Heating.WorkEsmEnd - section.HeatingSeasons.Heating.WorkEsmStart));
		int num2 = month.Saturdays * (24 - (section.HeatingSeasons.Heating.SatEsmEnd - section.HeatingSeasons.Heating.SatEsmStart));
		int num3 = month.Sundays * (24 - (section.HeatingSeasons.Heating.SunEsmEnd - section.HeatingSeasons.Heating.SunEsmStart));
		int num4 = month.Holydays * 24;
		return (calculationData.NonProjectTemperatureESM - averageMontlyTemp) * (double)(num + num2 + num3 + num4);
	}

	private static double CalcAvgProjectTempEsm(Section section, double averageMontlyTemp, CalculationData calculationData, MonthlyDays month)
	{
		int num = month.WorkDays * (section.HeatingSeasons.Heating.WorkEsmEnd - section.HeatingSeasons.Heating.WorkEsmStart);
		int num2 = month.Sundays * (section.HeatingSeasons.Heating.SunEsmEnd - section.HeatingSeasons.Heating.SunEsmStart);
		int num3 = month.Saturdays * (section.HeatingSeasons.Heating.SatEsmEnd - section.HeatingSeasons.Heating.SatEsmStart);
		return (calculationData.ProjectTemperatureESM - averageMontlyTemp) * (double)(num + num3 + num2);
	}

	private static double CalculateParameterHdEsm(Section section)
	{
		return SumAllDirectionsWallsEsm(section) + SumAllDirectionWindowsEsm(section) + SumNonTrasparentRoof(section.Roof.Esm) + SumTrasparentRoof(section.Roof.Esm);
	}

	private static double SumAllDirectionsWallsEsm(Section section)
	{
		north = CalculateItemsWalls(section.NorthWalls.Esm);
		northEast = CalculateItemsWalls(section.NorthEastWalls.Esm);
		east = CalculateItemsWalls(section.EastWalls.Esm);
		southEast = CalculateItemsWalls(section.SouthEastWalls.Esm);
		south = CalculateItemsWalls(section.SouthWalls.Esm);
		southWest = CalculateItemsWalls(section.SouthWestWalls.Esm);
		west = CalculateItemsWalls(section.WestWalls.Esm);
		northWest = CalculateItemsWalls(section.NorthWestWalls.Esm);
		return north + northEast + east + southEast + south + southWest + west + northWest;
	}

	private static double SumAllDirectionWindowsEsm(Section section)
	{
		north = section.NorthWalls.Esm.AccumulateWindowU * section.NorthWalls.Esm.AccumulateWindowA;
		northEast = section.NorthEastWalls.Esm.AccumulateWindowU * section.NorthEastWalls.Esm.AccumulateWindowA;
		east = section.EastWalls.Esm.AccumulateWindowU * section.EastWalls.Esm.AccumulateWindowA;
		southEast = section.SouthEastWalls.Esm.AccumulateWindowU * section.SouthEastWalls.Esm.AccumulateWindowA;
		south = section.SouthWalls.Esm.AccumulateWindowU * section.SouthWalls.Esm.AccumulateWindowA;
		southWest = section.SouthWestWalls.Esm.AccumulateWindowU * section.SouthWestWalls.Esm.AccumulateWindowA;
		west = section.WestWalls.Esm.AccumulateWindowU * section.WestWalls.Esm.AccumulateWindowA;
		northWest = section.NorthWestWalls.Esm.AccumulateWindowU * section.NorthWestWalls.Esm.AccumulateWindowA;
		return north + northEast + east + southEast + south + southWest + west + northWest;
	}

	private static double CalculateParameterHgEsm(Section section)
	{
		return section.Floor.Esm.AccumulateFloorA * section.Floor.Esm.AccumulateFloorU;
	}

	private static double SumWallDirecrionsHu1Esm(Section section, double averageMontlyTemp, double averageInnerHeatTemp)
	{
		double num = CalcWallDirectionParameterHu1(section.NorthWalls.Esm, averageMontlyTemp, averageInnerHeatTemp);
		double num2 = CalcWallDirectionParameterHu1(section.NorthWalls.Esm, averageMontlyTemp, averageInnerHeatTemp);
		double num3 = CalcWallDirectionParameterHu1(section.NorthWalls.Esm, averageMontlyTemp, averageInnerHeatTemp);
		double num4 = CalcWallDirectionParameterHu1(section.NorthWalls.Esm, averageMontlyTemp, averageInnerHeatTemp);
		double num5 = CalcWallDirectionParameterHu1(section.NorthWalls.Esm, averageMontlyTemp, averageInnerHeatTemp);
		double num6 = CalcWallDirectionParameterHu1(section.NorthWalls.Esm, averageMontlyTemp, averageInnerHeatTemp);
		double num7 = CalcWallDirectionParameterHu1(section.NorthWalls.Esm, averageMontlyTemp, averageInnerHeatTemp);
		double num8 = CalcWallDirectionParameterHu1(section.NorthWalls.Esm, averageMontlyTemp, averageInnerHeatTemp);
		return num + num2 + num3 + num4 + num5 + num6 + num7 + num8;
	}

	private static double CalculateParameterQgnEsm(Section section, ClimateZones climateZone, MonthlyDays month)
	{
		int num = month.WorkDays * (section.HeatingSeasons.Heating.WorkEsmEnd - section.HeatingSeasons.Heating.WorkEsmStart);
		num = ((month.Sundays > 0) ? (month.Sundays * (section.HeatingSeasons.Heating.SunEsmEnd - section.HeatingSeasons.Heating.SunEsmStart) + num) : num);
		num = ((month.Saturdays > 0) ? (month.Saturdays * (section.HeatingSeasons.Heating.SatEsmEnd - section.HeatingSeasons.Heating.SatEsmStart) + num) : num);
		int num2 = month.WorkDays * (24 - (section.HeatingSeasons.Heating.WorkEsmEnd - section.HeatingSeasons.Heating.WorkEsmStart));
		num2 = month.Saturdays * (24 - (section.HeatingSeasons.Heating.SatEsmEnd - section.HeatingSeasons.Heating.SatEsmStart)) + num2;
		num2 = month.Sundays * (24 - (section.HeatingSeasons.Heating.SunEsmEnd - section.HeatingSeasons.Heating.SunEsmStart)) + num2;
		num2 = month.Holydays * 24 + num2;
		return (CalculateNonTrasparentFsolEsm(section, climateZone, month) + CalculateTrasparentFsolEsm(section, climateZone, month)) * (double)(num + num2);
	}

	private static double CalculateTrasparentFsolEsm(Section section, ClimateZones climateZone, MonthlyDays month)
	{
		SolarRadiationPerMonth solarRadiationPerMonth = PreferencesManager.GetClimateZoneParams(climateZone).SolarRadiation.Months[(int)month.Month];
		Walls esm = section.NorthWalls.Esm;
		double num = CalculateTransparentFsol(esm.AccumulateWindowA, esm.AccumulateWindowG, esm.AccumulateWindowE, solarRadiationPerMonth.N);
		esm = section.NorthEastWalls.Esm;
		num += CalculateTransparentFsol(esm.AccumulateWindowA, esm.AccumulateWindowG, esm.AccumulateWindowE, (solarRadiationPerMonth.N + solarRadiationPerMonth.E) / 2.0);
		esm = section.EastWalls.Esm;
		num += CalculateTransparentFsol(esm.AccumulateWindowA, esm.AccumulateWindowG, esm.AccumulateWindowE, solarRadiationPerMonth.E);
		esm = section.SouthEastWalls.Esm;
		num += CalculateTransparentFsol(esm.AccumulateWindowA, esm.AccumulateWindowG, esm.AccumulateWindowE, (solarRadiationPerMonth.S + solarRadiationPerMonth.E) / 2.0);
		esm = section.SouthWalls.Esm;
		num += CalculateTransparentFsol(esm.AccumulateWindowA, esm.AccumulateWindowG, esm.AccumulateWindowE, solarRadiationPerMonth.S);
		esm = section.SouthWestWalls.Esm;
		num += CalculateTransparentFsol(esm.AccumulateWindowA, esm.AccumulateWindowG, esm.AccumulateWindowE, (solarRadiationPerMonth.S + solarRadiationPerMonth.W) / 2.0);
		esm = section.WestWalls.Esm;
		num += CalculateTransparentFsol(esm.AccumulateWindowA, esm.AccumulateWindowG, esm.AccumulateWindowE, solarRadiationPerMonth.W);
		esm = section.NorthWestWalls.Esm;
		num += CalculateTransparentFsol(esm.AccumulateWindowA, esm.AccumulateWindowG, esm.AccumulateWindowE, (solarRadiationPerMonth.N + solarRadiationPerMonth.W) / 2.0);
		Roof esm2 = section.Roof.Esm;
		double num2 = CalculateTransparentFsol(esm2.TransparentA1, esm2.TransparentG1, esm2.TransparentE1, solarRadiationPerMonth.N);
		esm2 = section.Roof.Esm;
		num2 += CalculateTransparentFsol(esm2.TransparentA2, esm2.TransparentG2, esm2.TransparentE2, (solarRadiationPerMonth.N + solarRadiationPerMonth.E) / 2.0);
		esm2 = section.Roof.Esm;
		num2 += CalculateTransparentFsol(esm2.TransparentA3, esm2.TransparentG3, esm2.TransparentE3, solarRadiationPerMonth.E);
		esm2 = section.Roof.Esm;
		num2 += CalculateTransparentFsol(esm2.TransparentA4, esm2.TransparentG4, esm2.TransparentE4, (solarRadiationPerMonth.S + solarRadiationPerMonth.E) / 2.0);
		esm2 = section.Roof.Esm;
		num2 += CalculateTransparentFsol(esm2.TransparentA5, esm2.TransparentG5, esm2.TransparentE5, solarRadiationPerMonth.S);
		esm2 = section.Roof.Esm;
		num2 += CalculateTransparentFsol(esm2.TransparentА6, esm2.TransparentG6, esm2.TransparentE6, (solarRadiationPerMonth.S + solarRadiationPerMonth.W) / 2.0);
		esm2 = section.Roof.Esm;
		num2 += CalculateTransparentFsol(esm2.TransparentA7, esm2.TransparentG7, esm2.TransparentE7, solarRadiationPerMonth.W);
		esm2 = section.Roof.Esm;
		num2 += CalculateTransparentFsol(esm2.TransparentA8, esm2.TransparentG8, esm2.TransparentE8, (solarRadiationPerMonth.N + solarRadiationPerMonth.W) / 2.0);
		esm2 = section.Roof.Esm;
		num2 += CalculateTransparentFsol(esm2.TransparentA9, esm2.TransparentG9, esm2.TransparentE9, solarRadiationPerMonth.H, horizontal: true);
		return num + num2;
	}

	private static double CalculateNonTrasparentFsolEsm(Section section, ClimateZones climateZone, MonthlyDays month)
	{
		SolarRadiationPerMonth solarRadiationPerMonth = PreferencesManager.GetClimateZoneParams(climateZone).SolarRadiation.Months[(int)month.Month];
		Walls esm = section.NorthWalls.Esm;
		double num = CalculateNonTransparentFsol(esm.AccumulateOuterAlfa, esm.AccumulateOuterU, esm.AccumulateOuterE, esm.AccumulateOuterA, solarRadiationPerMonth.N);
		esm = section.NorthEastWalls.Esm;
		num += CalculateNonTransparentFsol(esm.AccumulateOuterAlfa, esm.AccumulateOuterU, esm.AccumulateOuterE, esm.AccumulateOuterA, (solarRadiationPerMonth.N + solarRadiationPerMonth.E) / 2.0);
		esm = section.EastWalls.Esm;
		num += CalculateNonTransparentFsol(esm.AccumulateOuterAlfa, esm.AccumulateOuterU, esm.AccumulateOuterE, esm.AccumulateOuterA, solarRadiationPerMonth.E);
		esm = section.SouthEastWalls.Esm;
		num += CalculateNonTransparentFsol(esm.AccumulateOuterAlfa, esm.AccumulateOuterU, esm.AccumulateOuterE, esm.AccumulateOuterA, (solarRadiationPerMonth.S + solarRadiationPerMonth.E) / 2.0);
		esm = section.SouthWalls.Esm;
		num += CalculateNonTransparentFsol(esm.AccumulateOuterAlfa, esm.AccumulateOuterU, esm.AccumulateOuterE, esm.AccumulateOuterA, solarRadiationPerMonth.S);
		esm = section.SouthWestWalls.Esm;
		num += CalculateNonTransparentFsol(esm.AccumulateOuterAlfa, esm.AccumulateOuterU, esm.AccumulateOuterE, esm.AccumulateOuterA, (solarRadiationPerMonth.S + solarRadiationPerMonth.W) / 2.0);
		esm = section.WestWalls.Esm;
		num += CalculateNonTransparentFsol(esm.AccumulateOuterAlfa, esm.AccumulateOuterU, esm.AccumulateOuterE, esm.AccumulateOuterA, solarRadiationPerMonth.W);
		esm = section.NorthWestWalls.Esm;
		num += CalculateNonTransparentFsol(esm.AccumulateOuterAlfa, esm.AccumulateOuterU, esm.AccumulateOuterE, esm.AccumulateOuterA, (solarRadiationPerMonth.N + solarRadiationPerMonth.W) / 2.0);
		Roof esm2 = section.Roof.Esm;
		double num2 = CalculateNonTransparentFsol(esm2.AccumulateNonTransparentAlfa, esm2.AccumulateNonTransparentU, esm2.AccumulateNonTransparentE, esm2.AccumulateNonTransparentA, solarRadiationPerMonth.H, horizontal: true);
		return num + num2;
	}

	private static int OccupantsHoursBaseLine(Section section, MonthlyDays month)
	{
		return month.WorkDays * (section.HeatingSeasons.Occupants.WorkBaseEnd - section.HeatingSeasons.Occupants.WorkBaseStart) + month.Sundays * (section.HeatingSeasons.Occupants.SunBaseEnd - section.HeatingSeasons.Occupants.SunBaseStart) + month.Saturdays * (section.HeatingSeasons.Occupants.SatBaseEnd - section.HeatingSeasons.Occupants.SatBaseStart);
	}

	private static double CalculateBaseLine(this CalculationData heatingAndCoolingCalculations, Section section, CalculationInput calcInput, MonthlyDays month, double latentHeatPerMonth)
	{
		double num = CalculateParameterQtrBaseLine(heatingAndCoolingCalculations, section, calcInput.General.ClimateZone, month);
		double num2 = CalculateParameterQveBaseLIne(section, heatingAndCoolingCalculations, calcInput.General.ClimateZone, month);
		double num3 = num + num2;
		double num4 = CalculateParameterQgnBaseLine(section, calcInput.General.ClimateZone, month) / 1000.0;
		double gamma = (num4 + latentHeatPerMonth * section.Area.HeatedArea) / num3;
		parameterNiBaseLine = CalculateParameterNignBaseLine(heatingAndCoolingCalculations, calcInput.General.ClimateZone, month, gamma, section);
		return num3 - parameterNiBaseLine * num4;
	}

	private static double CalculateaHbaseLine(CalculationData calculationdata, Section section, ClimateZones climateZone, MonthlyDays month)
	{
		double avgTemp = PreferencesManager.GetClimateZoneParams(climateZone).SolarRadiation.Months[(int)month.Month].AvgTemp;
		double averageInnerHeatTemp = CalculateAverageHeatTempBaseLine(section, calculationdata, month);
		double num = SumWallDirecrionsHu1(section, avgTemp, averageInnerHeatTemp);
		double num2 = CalcCeilingsParameterHu2(section.Roof.Current, avgTemp, averageInnerHeatTemp);
		double num3 = CalcFloorsParameterHu3(section.Floor.Current, avgTemp, averageInnerHeatTemp);
		double num4 = CalculateParameterHdCurrent(section) + CalculateParameterHgCurrent(section) + (num + num2 + num3);
		double num5 = CalcParameterHveBaseLine(section, calculationdata);
		double num6 = section.Area.HeatedArea * section.Area.HeatCapacity / (num4 + num5);
		return 1.0 + num6 / 15.0;
	}

	private static double CalculateParameterNignBaseLine(CalculationData calculationdata, ClimateZones climateZone, MonthlyDays month, double gamma, Section section)
	{
		double num = CalculateaHbaseLine(calculationdata, section, climateZone, month);
		if (gamma > 0.0 && Math.Abs(gamma - 1.0) > 0.01)
		{
			return (1.0 - Math.Pow(gamma, num)) / (1.0 - Math.Pow(gamma, num + 1.0));
		}
		if (gamma < 0.0)
		{
			return 1.0;
		}
		if (Math.Abs(gamma - 1.0) < 0.01)
		{
			return num / (num + 1.0);
		}
		return 0.0;
	}

	private static double CalculateParameterQtrBaseLine(CalculationData claculationdata, Section section, ClimateZones climateZone, MonthlyDays month)
	{
		double avgTemp = PreferencesManager.GetClimateZoneParams(climateZone).SolarRadiation.Months[(int)month.Month].AvgTemp;
		double averageInnerHeatTemp = CalculateAverageHeatTempBaseLine(section, claculationdata, month);
		double num = SumWallDirecrionsHu1(section, avgTemp, averageInnerHeatTemp);
		double num2 = CalcCeilingsParameterHu2(section.Roof.Current, avgTemp, averageInnerHeatTemp);
		double num3 = CalcFloorsParameterHu3(section.Floor.Current, avgTemp, averageInnerHeatTemp);
		double num4 = CalculateParameterHdCurrent(section) + CalculateParameterHgCurrent(section) + (num + num2 + num3);
		return num4 * (CalcAvgProjectTempBaseLine(section, avgTemp, claculationdata, month) + CalcAvgNonProjectTempBaseLine(section, avgTemp, claculationdata, month)) / 1000.0;
	}

	private static double CalcAvgNonProjectTempBaseLine(Section section, double averageMontlyTemp, CalculationData calculationData, MonthlyDays month)
	{
		int num = month.WorkDays * (24 - (section.HeatingSeasons.Heating.WorkBaseEnd - section.HeatingSeasons.Heating.WorkBaseStart));
		int num2 = month.Saturdays * (24 - (section.HeatingSeasons.Heating.SatBaseEnd - section.HeatingSeasons.Heating.SatBaseStart));
		int num3 = month.Sundays * (24 - (section.HeatingSeasons.Heating.SunBaseEnd - section.HeatingSeasons.Heating.SunBaseStart));
		int num4 = month.Holydays * 24;
		return (calculationData.NonProjectTemperatureBaseLine - averageMontlyTemp) * (double)(num + num2 + num3 + num4);
	}

	private static double CalcAvgProjectTempBaseLine(Section section, double averageMontlyTemp, CalculationData calculationData, MonthlyDays month)
	{
		int num = month.WorkDays * (section.HeatingSeasons.Heating.WorkBaseEnd - section.HeatingSeasons.Heating.WorkBaseStart);
		int num2 = month.Sundays * (section.HeatingSeasons.Heating.SunBaseEnd - section.HeatingSeasons.Heating.SunBaseStart);
		int num3 = month.Saturdays * (section.HeatingSeasons.Heating.SatBaseEnd - section.HeatingSeasons.Heating.SatBaseStart);
		return (calculationData.ProjectTemperatureBaseLine - averageMontlyTemp) * (double)(num + num3 + num2);
	}

	private static double CalculateAverageHeatTempBaseLine(Section section, CalculationData calculationData, MonthlyDays month)
	{
		int num = month.WorkDays * (section.HeatingSeasons.Heating.WorkBaseEnd - section.HeatingSeasons.Heating.WorkBaseStart);
		num = ((month.Sundays > 0) ? (month.Sundays * (section.HeatingSeasons.Heating.SunBaseEnd - section.HeatingSeasons.Heating.SunBaseStart) + num) : num);
		num = ((month.Saturdays > 0) ? (month.Saturdays * (section.HeatingSeasons.Heating.SatBaseEnd - section.HeatingSeasons.Heating.SatBaseStart) + num) : num);
		double projectTemperatureBaseLine = calculationData.ProjectTemperatureBaseLine;
		int num2 = month.WorkDays * (24 - (section.HeatingSeasons.Heating.WorkBaseEnd - section.HeatingSeasons.Heating.WorkBaseStart));
		num2 = month.Saturdays * (24 - (section.HeatingSeasons.Heating.SatBaseEnd - section.HeatingSeasons.Heating.SatBaseStart)) + num2;
		num2 = month.Sundays * (24 - (section.HeatingSeasons.Heating.SunBaseEnd - section.HeatingSeasons.Heating.SunBaseStart)) + num2;
		num2 = month.Holydays * 24 + num2;
		double nonProjectTemperatureBaseLine = calculationData.NonProjectTemperatureBaseLine;
		return ((double)num * projectTemperatureBaseLine + (double)num2 * nonProjectTemperatureBaseLine) / (double)(num + num2);
	}

	private static double CalculateParameterQveBaseLIne(Section section, CalculationData calculationData, ClimateZones climateZone, MonthlyDays month)
	{
		double avgTemp = PreferencesManager.GetClimateZoneParams(climateZone).SolarRadiation.Months[(int)month.Month].AvgTemp;
		return CalcParameterHveBaseLine(section, calculationData) * (CalcAvgProjectTempBaseLine(section, avgTemp, calculationData, month) + CalcAvgNonProjectTempBaseLine(section, avgTemp, calculationData, month)) / 1000.0;
	}

	private static double CalculateParameterHtrBaseLine(Section section, double averageMontlyTemp, double averageInnerHeatTemp)
	{
		double num = SumWallDirecrionsHu1(section, averageMontlyTemp, averageInnerHeatTemp);
		double num2 = CalcCeilingsParameterHu2(section.Roof.Current, averageMontlyTemp, averageInnerHeatTemp);
		double num3 = CalcFloorsParameterHu3(section.Floor.Current, averageMontlyTemp, averageInnerHeatTemp);
		section.Test.ParameterHu = num + num2 + num3;
		section.Test.ParameterHd = CalculateParameterHdCurrent(section);
		section.Test.ParameterHg = CalculateParameterHgCurrent(section);
		return section.Test.ParameterHd + section.Test.ParameterHg + section.Test.ParameterHu;
	}

	private static double CalcParameterHveBaseLine(Section section, CalculationData heatingAndCoolingCalculations)
	{
		section.Test.ParameterHve = section.Area.HeatedVolume * heatingAndCoolingCalculations.InfiltracionBaseLine * 0.34;
		return section.Test.ParameterHve;
	}

	private static double CalculateParameterQgnBaseLine(Section section, ClimateZones climateZone, MonthlyDays month)
	{
		int num = month.WorkDays * (section.HeatingSeasons.Heating.WorkBaseEnd - section.HeatingSeasons.Heating.WorkBaseStart);
		num = ((month.Sundays > 0) ? (month.Sundays * (section.HeatingSeasons.Heating.SunBaseEnd - section.HeatingSeasons.Heating.SunBaseStart) + num) : num);
		num = ((month.Saturdays > 0) ? (month.Saturdays * (section.HeatingSeasons.Heating.SatBaseEnd - section.HeatingSeasons.Heating.SatBaseStart) + num) : num);
		int num2 = month.WorkDays * (24 - (section.HeatingSeasons.Heating.WorkBaseEnd - section.HeatingSeasons.Heating.WorkBaseStart));
		num2 = month.Saturdays * (24 - (section.HeatingSeasons.Heating.SatBaseEnd - section.HeatingSeasons.Heating.SatBaseStart)) + num2;
		num2 = month.Sundays * (24 - (section.HeatingSeasons.Heating.SunBaseEnd - section.HeatingSeasons.Heating.SunBaseStart)) + num2;
		num2 = month.Holydays * 24 + num2;
		return (CalculateNonTrasparentFsol(section, climateZone, month) + CalculateTrasparentFsol(section, climateZone, month)) * (double)(num + num2);
	}

	private static int OccupantsHoursRef1(MonthlyDays month, Section tempSectionRef1)
	{
		return month.WorkDays * (tempSectionRef1.HeatingSeasons.Occupants.WorkCurrentEnd - tempSectionRef1.HeatingSeasons.Occupants.WorkCurrentStart) + month.Sundays * (tempSectionRef1.HeatingSeasons.Occupants.SunCurrentEnd - tempSectionRef1.HeatingSeasons.Occupants.SunCurrentStart) + month.Saturdays * (tempSectionRef1.HeatingSeasons.Occupants.SatCurrentEnd - tempSectionRef1.HeatingSeasons.Occupants.SatCurrentStart);
	}

	private static double CalculateRef1(CalculationData calcData, Section section, CalculationInput calcInput, MonthlyDays month)
	{
		int num = OccupantHours(section, month);
		double num2 = section.Area.MetabolicHeat * (double)num / 1000.0;
		double parameterHtr;
		double num3 = CalculateParameterQtrRef1(calcData, section, calcInput.General.ClimateZone, month, out parameterHtr);
		double num4 = CalculateParameterQveRef1(section, calcInput.General.ClimateZone, calcData, month);
		double num5 = num3 + num4;
		double num6 = CalculateParameterQgnBaseLine(section, calcInput.General.ClimateZone, month) / 1000.0;
		double gamma = (num6 + num2 * section.Area.HeatedArea) / num5;
		parameterNiRef1 = CalculateParameterNignRef1(calcData, calcInput.General.ClimateZone, month, gamma, section);
		return num5 - parameterNiRef1 * num6;
	}

	private static void CalculateRef1(CalculationData calcData, Section section, CalculationInput calcInput, List<double> energyByMonthsListRef1, Section tempSectionRef1, MonthlyDays month, List<double> latentHeatListRef1)
	{
		energyByMonthsListRef1.Add(CalculateRef1(calcData, tempSectionRef1, calcInput, month));
		int num = OccupantsHoursRef1(month, tempSectionRef1);
		double item = parameterNiRef1 * section.Area.MetabolicHeat * (double)num / 1000.0;
		latentHeatListRef1.Add(item);
	}

	private static double CalculateParameterQveRef1(Section section, ClimateZones climateZone, CalculationData calculationData, MonthlyDays month)
	{
		return CalcParameterHveRef1(section, calculationData) * (CalcAvgProjectTempRef1(section, climateZone, calculationData, month) + CalcAvgNonProjectTempRef1(section, climateZone, calculationData, month)) / 1000.0;
	}

	private static double CalcParameterHveRef1(Section section, CalculationData heatingAndCoolingCalculations)
	{
		return section.Area.HeatedVolume * heatingAndCoolingCalculations.InfiltracionRef1 * 0.34;
	}

	private static double CalculateParameterQtrRef1(CalculationData calculationdata, Section tempSection, ClimateZones climateZone, MonthlyDays month, out double parameterHtr)
	{
		double avgTemp = PreferencesManager.GetClimateZoneParams(climateZone).SolarRadiation.Months[(int)month.Month].AvgTemp;
		double averageInnerHeatTemp = CalculateAverageHeatTempRef1(tempSection, calculationdata, month);
		parameterHtr = CalculateParameterHtr(tempSection, avgTemp, averageInnerHeatTemp);
		tempSection.Test.ParameterHtr = parameterHtr;
		return tempSection.Test.ParameterHtr * (CalcAvgProjectTempRef1(tempSection, climateZone, calculationdata, month) + CalcAvgNonProjectTempRef1(tempSection, climateZone, calculationdata, month)) / 1000.0;
	}

	private static double CalculateParameterHtrRef(Section tempSection, double averageMontlyTemp, double averageInnerHeatTemp)
	{
		double num = SumWallDirecrionsHu1(tempSection, averageMontlyTemp, averageInnerHeatTemp);
		double num2 = CalcCeilingsParameterHu2(tempSection.Roof.Current, averageMontlyTemp, averageInnerHeatTemp);
		double num3 = CalcFloorsParameterHu3(tempSection.Floor.Current, averageMontlyTemp, averageInnerHeatTemp);
		tempSection.Test.ParameterHu = num + num2 + num3;
		tempSection.Test.ParameterHd = CalculateParameterHdCurrent(tempSection);
		tempSection.Test.ParameterHg = CalculateParameterHgCurrent(tempSection);
		return tempSection.Test.ParameterHd + tempSection.Test.ParameterHg + tempSection.Test.ParameterHu;
	}

	private static double CalculateAverageHeatTempRef1(Section section, CalculationData calculationdata, MonthlyDays month)
	{
		int num = month.WorkDays * (section.HeatingSeasons.Heating.WorkCurrentEnd - section.HeatingSeasons.Heating.WorkCurrentStart);
		num = ((month.Sundays > 0) ? (month.Sundays * (section.HeatingSeasons.Heating.SunBaseEnd - section.HeatingSeasons.Heating.SunCurrentStart) + num) : num);
		num = ((month.Saturdays > 0) ? (month.Saturdays * (section.HeatingSeasons.Heating.SatBaseEnd - section.HeatingSeasons.Heating.SunCurrentStart) + num) : num);
		double projectTemperatureRef = calculationdata.ProjectTemperatureRef1;
		int num2 = month.WorkDays * (24 - (section.HeatingSeasons.Heating.WorkCurrentEnd - section.HeatingSeasons.Heating.WorkCurrentStart));
		num2 = month.Saturdays * (24 - (section.HeatingSeasons.Heating.SatCurrentEnd - section.HeatingSeasons.Heating.SatCurrentStart)) + num2;
		num2 = month.Sundays * (24 - (section.HeatingSeasons.Heating.SunCurrentEnd - section.HeatingSeasons.Heating.SunCurrentStart)) + num2;
		num2 = month.Holydays * 24 + num2;
		double nonProjectTemperatureRef = calculationdata.NonProjectTemperatureRef1;
		return ((double)num * projectTemperatureRef + (double)num2 * nonProjectTemperatureRef) / (double)(num + num2);
	}

	private static double CalcAvgProjectTempRef1(Section section, ClimateZones climateZone, CalculationData calculationData, MonthlyDays month)
	{
		double avgTemp = PreferencesManager.GetClimateZoneParams(climateZone).SolarRadiation.Months[(int)month.Month].AvgTemp;
		int num = month.WorkDays * (section.HeatingSeasons.Heating.WorkCurrentEnd - section.HeatingSeasons.Heating.WorkCurrentStart);
		int num2 = month.Sundays * (section.HeatingSeasons.Heating.SunCurrentEnd - section.HeatingSeasons.Heating.SunCurrentStart);
		int num3 = month.Saturdays * (section.HeatingSeasons.Heating.SatCurrentEnd - section.HeatingSeasons.Heating.SatCurrentStart);
		return (calculationData.ProjectTemperatureRef1 - avgTemp) * (double)(num + num3 + num2);
	}

	private static double CalcAvgNonProjectTempRef1(Section section, ClimateZones climateZone, CalculationData calculationData, MonthlyDays month)
	{
		double avgTemp = PreferencesManager.GetClimateZoneParams(climateZone).SolarRadiation.Months[(int)month.Month].AvgTemp;
		int num = month.WorkDays * (24 - (section.HeatingSeasons.Heating.WorkBaseEnd - section.HeatingSeasons.Heating.WorkBaseStart));
		int num2 = month.Saturdays * (24 - (section.HeatingSeasons.Heating.SatBaseEnd - section.HeatingSeasons.Heating.SatBaseStart));
		int num3 = month.Sundays * (24 - (section.HeatingSeasons.Heating.SunBaseEnd - section.HeatingSeasons.Heating.SunBaseStart));
		int num4 = month.Holydays * 24;
		return (calculationData.NonProjectTemperatureRef1 - avgTemp) * (double)(num + num2 + num3 + num4);
	}

	private static double CalculateaHref1(CalculationData calculationdata, Section tempSection, ClimateZones climateZone, MonthlyDays month)
	{
		double avgTemp = PreferencesManager.GetClimateZoneParams(climateZone).SolarRadiation.Months[(int)month.Month].AvgTemp;
		double averageInnerHeatTemp = CalculateAverageHeatTempRef1(tempSection, calculationdata, month);
		double num = CalculateParameterHtr(tempSection, avgTemp, averageInnerHeatTemp);
		double num2 = CalcParameterHveRef1(tempSection, calculationdata);
		double num3 = tempSection.Area.HeatedArea * tempSection.Area.HeatCapacity / (num + num2);
		return 1.0 + num3 / 15.0;
	}

	private static double CalculateParameterNignRef1(CalculationData calculationdata, ClimateZones climateZone, MonthlyDays month, double gamma, Section section)
	{
		double num = CalculateaHref1(calculationdata, section, climateZone, month);
		if (gamma > 0.0 && Math.Abs(gamma - 1.0) > 0.01)
		{
			return (1.0 - Math.Pow(gamma, num)) / (1.0 - Math.Pow(gamma, num + 1.0));
		}
		if (gamma < 0.0)
		{
			return 1.0;
		}
		if (Math.Abs(gamma - 1.0) < 0.01)
		{
			return num / (num + 1.0);
		}
		return 0.0;
	}

	private static void ApplyValuesToTempSectionRef1(Section tempSection, CalculationData calcData)
	{
		ApplyUdirectionWalls(tempSection, calcData.UouterWallsRef1, calcData.UinnerWallsRef1, calcData.UwindowsRef1);
		ApplyUroofsAndCeilings(tempSection, calcData.UnontransparentRef1, calcData.UfloorRef1, calcData.UceilingRef1, calcData.UfloorOtherRef1);
		ApplyCoefficientG(tempSection, calcData.gRef1);
	}

	private static void ApplyCoefficientG(Section tempSection, double gRef)
	{
		CopyGbyOrientation(tempSection.NorthWalls, gRef);
		CopyGbyOrientation(tempSection.NorthEastWalls, gRef);
		CopyGbyOrientation(tempSection.EastWalls, gRef);
		CopyGbyOrientation(tempSection.SouthEastWalls, gRef);
		CopyGbyOrientation(tempSection.SouthWalls, gRef);
		CopyGbyOrientation(tempSection.SouthWestWalls, gRef);
		CopyGbyOrientation(tempSection.WestWalls, gRef);
		CopyGbyOrientation(tempSection.NorthWestWalls, gRef);
		tempSection.Roof.Current.TransparentG1 = ((tempSection.Roof.Current.TransparentG1 > 0.0) ? gRef : 0.0);
		tempSection.Roof.Current.TransparentG2 = ((tempSection.Roof.Current.TransparentG2 > 0.0) ? gRef : 0.0);
		tempSection.Roof.Current.TransparentG3 = ((tempSection.Roof.Current.TransparentG3 > 0.0) ? gRef : 0.0);
		tempSection.Roof.Current.TransparentG4 = ((tempSection.Roof.Current.TransparentG4 > 0.0) ? gRef : 0.0);
		tempSection.Roof.Current.TransparentG5 = ((tempSection.Roof.Current.TransparentG5 > 0.0) ? gRef : 0.0);
		tempSection.Roof.Current.TransparentG6 = ((tempSection.Roof.Current.TransparentG6 > 0.0) ? gRef : 0.0);
		tempSection.Roof.Current.TransparentG7 = ((tempSection.Roof.Current.TransparentG7 > 0.0) ? gRef : 0.0);
		tempSection.Roof.Current.TransparentG8 = ((tempSection.Roof.Current.TransparentG8 > 0.0) ? gRef : 0.0);
		tempSection.Roof.Current.TransparentG9 = ((tempSection.Roof.Current.TransparentG9 > 0.0) ? gRef : 0.0);
		tempSection.Roof.Current.SumTrasparentArea();
		tempSection.Roof.Current.CalculateTrasparentG();
	}

	private static void ApplyUroofsAndCeilings(Section tempSection, double unontransparentRef, double ufloorRef, double uceilingRef, double ufloorOtherRef)
	{
		tempSection.Roof.Current.NonTransparentU1 = ((tempSection.Roof.Current.NonTransparentU1 > 0.0) ? unontransparentRef : 0.0);
		tempSection.Roof.Current.NonTransparentU2 = ((tempSection.Roof.Current.NonTransparentU2 > 0.0) ? unontransparentRef : 0.0);
		tempSection.Roof.Current.NonTransparentU3 = ((tempSection.Roof.Current.NonTransparentU3 > 0.0) ? unontransparentRef : 0.0);
		tempSection.Roof.Current.NonTransparentU4 = ((tempSection.Roof.Current.NonTransparentU4 > 0.0) ? unontransparentRef : 0.0);
		tempSection.Roof.Current.NonTransparentU5 = ((tempSection.Roof.Current.NonTransparentU5 > 0.0) ? unontransparentRef : 0.0);
		tempSection.Roof.Current.NonTransparentU6 = ((tempSection.Roof.Current.NonTransparentU6 > 0.0) ? unontransparentRef : 0.0);
		tempSection.Roof.Current.NonTransparentU7 = ((tempSection.Roof.Current.NonTransparentU7 > 0.0) ? unontransparentRef : 0.0);
		tempSection.Roof.Current.NonTransparentU8 = ((tempSection.Roof.Current.NonTransparentU8 > 0.0) ? unontransparentRef : 0.0);
		tempSection.Roof.Current.NonTransparentU9 = ((tempSection.Roof.Current.NonTransparentU9 > 0.0) ? unontransparentRef : 0.0);
		tempSection.Roof.Current.CalculateArea();
		tempSection.Roof.Current.CalculateNonTranspU();
		tempSection.Roof.Current.CeilingU1 = ((tempSection.Roof.Current.CeilingU1 > 0.0) ? uceilingRef : 0.0);
		tempSection.Roof.Current.CeilingU2 = ((tempSection.Roof.Current.CeilingU2 > 0.0) ? uceilingRef : 0.0);
		tempSection.Roof.Current.CeilingU3 = ((tempSection.Roof.Current.CeilingU3 > 0.0) ? uceilingRef : 0.0);
		tempSection.Roof.Current.CeilingU4 = ((tempSection.Roof.Current.CeilingU4 > 0.0) ? uceilingRef : 0.0);
		tempSection.Roof.Current.CeilingU5 = ((tempSection.Roof.Current.CeilingU5 > 0.0) ? uceilingRef : 0.0);
		tempSection.Roof.Current.CeilingU6 = ((tempSection.Roof.Current.CeilingU6 > 0.0) ? uceilingRef : 0.0);
		tempSection.Roof.Current.CeilingU7 = ((tempSection.Roof.Current.CeilingU7 > 0.0) ? uceilingRef : 0.0);
		tempSection.Roof.Current.CeilingU8 = ((tempSection.Roof.Current.CeilingU8 > 0.0) ? uceilingRef : 0.0);
		tempSection.Roof.Current.CeilingU9 = ((tempSection.Roof.Current.CeilingU9 > 0.0) ? uceilingRef : 0.0);
		tempSection.Roof.Current.SumCeilingArea();
		tempSection.Roof.Current.CalculateCeilingU();
		tempSection.Floor.Current.FloorU1 = ((tempSection.Floor.Current.FloorU1 > 0.0) ? ufloorRef : 0.0);
		tempSection.Floor.Current.FloorU2 = ((tempSection.Floor.Current.FloorU2 > 0.0) ? ufloorRef : 0.0);
		tempSection.Floor.Current.FloorU3 = ((tempSection.Floor.Current.FloorU3 > 0.0) ? ufloorRef : 0.0);
		tempSection.Floor.Current.FloorU4 = ((tempSection.Floor.Current.FloorU4 > 0.0) ? ufloorRef : 0.0);
		tempSection.Floor.Current.FloorU5 = ((tempSection.Floor.Current.FloorU5 > 0.0) ? ufloorRef : 0.0);
		tempSection.Floor.Current.FloorU6 = ((tempSection.Floor.Current.FloorU6 > 0.0) ? ufloorRef : 0.0);
		tempSection.Floor.Current.CalculateFloorArea();
		tempSection.Floor.Current.CalculateFloorU();
		tempSection.Floor.Current.OtherFloorU1 = ((tempSection.Floor.Current.OtherFloorU1 > 0.0) ? ufloorOtherRef : 0.0);
		tempSection.Floor.Current.OtherFloorU2 = ((tempSection.Floor.Current.OtherFloorU2 > 0.0) ? ufloorOtherRef : 0.0);
		tempSection.Floor.Current.OtherFloorU3 = ((tempSection.Floor.Current.OtherFloorU3 > 0.0) ? ufloorOtherRef : 0.0);
		tempSection.Floor.Current.OtherFloorU4 = ((tempSection.Floor.Current.OtherFloorU4 > 0.0) ? ufloorOtherRef : 0.0);
		tempSection.Floor.Current.OtherFloorU5 = ((tempSection.Floor.Current.OtherFloorU5 > 0.0) ? ufloorOtherRef : 0.0);
		tempSection.Floor.Current.OtherFloorU6 = ((tempSection.Floor.Current.OtherFloorU6 > 0.0) ? ufloorOtherRef : 0.0);
		tempSection.Floor.Current.CalculateOtherFloorArea();
		tempSection.Floor.Current.CalculateOtherFloorU();
	}

	private static void ApplyUdirectionWalls(Section tempSection, double uOuterWallsRef, double uInnerWallsRef, double windowsRef)
	{
		CopyByOrientation(tempSection.NorthWalls, uOuterWallsRef, uInnerWallsRef, windowsRef);
		CopyByOrientation(tempSection.NorthEastWalls, uOuterWallsRef, uInnerWallsRef, windowsRef);
		CopyByOrientation(tempSection.EastWalls, uOuterWallsRef, uInnerWallsRef, windowsRef);
		CopyByOrientation(tempSection.SouthEastWalls, uOuterWallsRef, uInnerWallsRef, windowsRef);
		CopyByOrientation(tempSection.SouthWalls, uOuterWallsRef, uInnerWallsRef, windowsRef);
		CopyByOrientation(tempSection.SouthWestWalls, uOuterWallsRef, uInnerWallsRef, windowsRef);
		CopyByOrientation(tempSection.WestWalls, uOuterWallsRef, uInnerWallsRef, windowsRef);
		CopyByOrientation(tempSection.NorthWestWalls, uOuterWallsRef, uInnerWallsRef, windowsRef);
		ApplyToTrasparentRoofs(tempSection.Roof, windowsRef);
	}

	private static void ApplyToTrasparentRoofs(RoofStates roof, double windowsRef)
	{
		roof.Current.TransparentU1 = ((roof.Current.TransparentU1 > 0.0) ? windowsRef : 0.0);
		roof.Current.TransparentU2 = ((roof.Current.TransparentU2 > 0.0) ? windowsRef : 0.0);
		roof.Current.TransparentU3 = ((roof.Current.TransparentU3 > 0.0) ? windowsRef : 0.0);
		roof.Current.TransparentU4 = ((roof.Current.TransparentU4 > 0.0) ? windowsRef : 0.0);
		roof.Current.TransparentU5 = ((roof.Current.TransparentU5 > 0.0) ? windowsRef : 0.0);
		roof.Current.TransparentU6 = ((roof.Current.TransparentU6 > 0.0) ? windowsRef : 0.0);
		roof.Current.TransparentU7 = ((roof.Current.TransparentU7 > 0.0) ? windowsRef : 0.0);
		roof.Current.TransparentU8 = ((roof.Current.TransparentU8 > 0.0) ? windowsRef : 0.0);
		roof.Current.TransparentU9 = ((roof.Current.TransparentU9 > 0.0) ? windowsRef : 0.0);
		roof.Current.SumTrasparentArea();
		roof.Current.CalculateTrasparentU();
	}

	private static void CopyByOrientation(WallsStates wall, double uouterWallsRef, double uInnerWallsRef, double windowsRef)
	{
		wall.Current.OuterU1 = ((wall.Current.OuterU1 > 0.0) ? uouterWallsRef : 0.0);
		wall.Current.OuterU2 = ((wall.Current.OuterU2 > 0.0) ? uouterWallsRef : 0.0);
		wall.Current.OuterU3 = ((wall.Current.OuterU3 > 0.0) ? uouterWallsRef : 0.0);
		wall.Current.OuterU4 = ((wall.Current.OuterU4 > 0.0) ? uouterWallsRef : 0.0);
		wall.Current.OuterU5 = ((wall.Current.OuterU5 > 0.0) ? uouterWallsRef : 0.0);
		wall.Current.OuterU6 = ((wall.Current.OuterU6 > 0.0) ? uouterWallsRef : 0.0);
		wall.Current.SumColumnOuterArea();
		wall.Current.AccumulateOuterU();
		wall.Current.WindowU1 = ((wall.Current.WindowU1 > 0.0) ? windowsRef : 0.0);
		wall.Current.WindowU2 = ((wall.Current.WindowU2 > 0.0) ? windowsRef : 0.0);
		wall.Current.WindowU3 = ((wall.Current.WindowU3 > 0.0) ? windowsRef : 0.0);
		wall.Current.WindowU4 = ((wall.Current.WindowU4 > 0.0) ? windowsRef : 0.0);
		wall.Current.WindowU5 = ((wall.Current.WindowU5 > 0.0) ? windowsRef : 0.0);
		wall.Current.WindowU6 = ((wall.Current.WindowU6 > 0.0) ? windowsRef : 0.0);
		wall.Current.SumWindowArea();
		wall.Current.CalculateWindowU();
		wall.Current.InnerU1 = ((wall.Current.InnerU1 > 0.0) ? uInnerWallsRef : 0.0);
		wall.Current.InnerU2 = ((wall.Current.InnerU2 > 0.0) ? uInnerWallsRef : 0.0);
		wall.Current.InnerU3 = ((wall.Current.InnerU3 > 0.0) ? uInnerWallsRef : 0.0);
		wall.Current.InnerU4 = ((wall.Current.InnerU4 > 0.0) ? uInnerWallsRef : 0.0);
		wall.Current.InnerU5 = ((wall.Current.InnerU5 > 0.0) ? uInnerWallsRef : 0.0);
		wall.Current.InnerU6 = ((wall.Current.InnerU6 > 0.0) ? uInnerWallsRef : 0.0);
		wall.Current.SumColumnInnerArea();
		wall.Current.CalculateInnerU();
	}

	private static void CopyGbyOrientation(WallsStates wall, double gRef)
	{
		wall.Current.WindowG1 = ((wall.Current.WindowG1 > 0.0) ? gRef : 0.0);
		wall.Current.WindowG2 = ((wall.Current.WindowG2 > 0.0) ? gRef : 0.0);
		wall.Current.WindowG3 = ((wall.Current.WindowG3 > 0.0) ? gRef : 0.0);
		wall.Current.WindowG4 = ((wall.Current.WindowG4 > 0.0) ? gRef : 0.0);
		wall.Current.WindowG5 = ((wall.Current.WindowG5 > 0.0) ? gRef : 0.0);
		wall.Current.WindowG6 = ((wall.Current.WindowG6 > 0.0) ? gRef : 0.0);
		wall.Current.SumWindowArea();
		wall.Current.CalculateWindowG();
	}

	private static int OccupantsHoursRef2(MonthlyDays month, Section tempSectionRef2)
	{
		return month.WorkDays * (tempSectionRef2.HeatingSeasons.Occupants.WorkCurrentEnd - tempSectionRef2.HeatingSeasons.Occupants.WorkCurrentStart) + month.Sundays * (tempSectionRef2.HeatingSeasons.Occupants.SunCurrentEnd - tempSectionRef2.HeatingSeasons.Occupants.SunCurrentStart) + month.Saturdays * (tempSectionRef2.HeatingSeasons.Occupants.SatCurrentEnd - tempSectionRef2.HeatingSeasons.Occupants.SatCurrentStart);
	}

	private static double CalculateRef2(CalculationData heatingAndCoolingCalculations, Section section, CalculationInput calcInput, MonthlyDays month)
	{
		int num = OccupantHours(section, month);
		double num2 = section.Area.MetabolicHeat * (double)num / 1000.0;
		double parameterHtr;
		double num3 = CalculateParameterQtrRef2(heatingAndCoolingCalculations, section, calcInput.General.ClimateZone, month, out parameterHtr);
		double num4 = CalculateParameterQveRef2(section, heatingAndCoolingCalculations, calcInput.General.ClimateZone, month);
		double num5 = num3 + num4;
		double num6 = CalculateParameterQgnBaseLine(section, calcInput.General.ClimateZone, month) / 1000.0;
		double gamma = (num6 + num2 * section.Area.HeatedArea) / num5;
		parameterNiRef2 = CalculateParameterNignRef2(heatingAndCoolingCalculations, calcInput.General.ClimateZone, month, gamma, section);
		return num5 - parameterNiRef2 * num6;
	}

	private static void CalculateRef2(CalculationData calcData, Section section, CalculationInput calcInput, List<double> energyByMonthsListRef2, Section tempSectionRef2, MonthlyDays month, List<double> latentHeatListRef2)
	{
		energyByMonthsListRef2.Add(CalculateRef2(calcData, tempSectionRef2, calcInput, month));
		int num = OccupantsHoursRef2(month, tempSectionRef2);
		double item = parameterNiRef2 * section.Area.MetabolicHeat * (double)num / 1000.0;
		latentHeatListRef2.Add(item);
	}

	private static double CalculateParameterQveRef2(Section section, CalculationData calculationData, ClimateZones climateZone, MonthlyDays month)
	{
		double avgTemp = PreferencesManager.GetClimateZoneParams(climateZone).SolarRadiation.Months[(int)month.Month].AvgTemp;
		return CalcParameterHveRef2(section, calculationData) * (CalcAvgProjectTempRef2(section, climateZone, calculationData, month) + CalcAvgNonProjectTempRef2(section, climateZone, calculationData, month)) / 1000.0;
	}

	private static double CalcParameterHveRef2(Section section, CalculationData heatingAndCoolingCalculations)
	{
		return section.Area.HeatedVolume * heatingAndCoolingCalculations.InfiltracionRef2 * 0.34;
	}

	private static double CalculateParameterQtrRef2(CalculationData calculationdata, Section tempSection, ClimateZones climateZone, MonthlyDays month, out double parameterHtr)
	{
		double avgTemp = PreferencesManager.GetClimateZoneParams(climateZone).SolarRadiation.Months[(int)month.Month].AvgTemp;
		double averageInnerHeatTemp = CalculateAverageHeatTempRef2(tempSection, calculationdata, month);
		parameterHtr = CalculateParameterHtr(tempSection, avgTemp, averageInnerHeatTemp);
		tempSection.Test.ParameterHtr = parameterHtr;
		return tempSection.Test.ParameterHtr * (CalcAvgProjectTempRef2(tempSection, climateZone, calculationdata, month) + CalcAvgNonProjectTempRef2(tempSection, climateZone, calculationdata, month)) / 1000.0;
	}

	private static double CalculateAverageHeatTempRef2(Section section, CalculationData calculationdata, MonthlyDays month)
	{
		int num = month.WorkDays * (section.HeatingSeasons.Heating.WorkBaseEnd - section.HeatingSeasons.Heating.WorkBaseStart);
		num = ((month.Sundays > 0) ? (month.Sundays * (section.HeatingSeasons.Heating.SunBaseEnd - section.HeatingSeasons.Heating.SunBaseStart) + num) : num);
		num = ((month.Saturdays > 0) ? (month.Saturdays * (section.HeatingSeasons.Heating.SatBaseEnd - section.HeatingSeasons.Heating.SatBaseStart) + num) : num);
		double projectTemperatureRef = calculationdata.ProjectTemperatureRef2;
		int num2 = month.WorkDays * (24 - (section.HeatingSeasons.Heating.WorkBaseEnd - section.HeatingSeasons.Heating.WorkBaseStart));
		num2 = month.Saturdays * (24 - (section.HeatingSeasons.Heating.SatBaseEnd - section.HeatingSeasons.Heating.SatBaseStart)) + num2;
		num2 = month.Sundays * (24 - (section.HeatingSeasons.Heating.SunBaseEnd - section.HeatingSeasons.Heating.SunBaseStart)) + num2;
		num2 = month.Holydays * 24 + num2;
		double nonProjectTemperatureRef = calculationdata.NonProjectTemperatureRef2;
		return ((double)num * projectTemperatureRef + (double)num2 * nonProjectTemperatureRef) / (double)(num + num2);
	}

	private static double CalcAvgProjectTempRef2(Section section, ClimateZones climateZone, CalculationData calculationData, MonthlyDays month)
	{
		double avgTemp = PreferencesManager.GetClimateZoneParams(climateZone).SolarRadiation.Months[(int)month.Month].AvgTemp;
		int num = month.WorkDays * (section.HeatingSeasons.Heating.WorkCurrentEnd - section.HeatingSeasons.Heating.WorkCurrentStart);
		int num2 = month.Sundays * (section.HeatingSeasons.Heating.SunCurrentEnd - section.HeatingSeasons.Heating.SunCurrentStart);
		int num3 = month.Saturdays * (section.HeatingSeasons.Heating.SatCurrentEnd - section.HeatingSeasons.Heating.SatCurrentStart);
		return (calculationData.ProjectTemperatureRef2 - avgTemp) * (double)(num + num3 + num2);
	}

	private static double CalcAvgNonProjectTempRef2(Section section, ClimateZones climateZone, CalculationData calculationData, MonthlyDays month)
	{
		double avgTemp = PreferencesManager.GetClimateZoneParams(climateZone).SolarRadiation.Months[(int)month.Month].AvgTemp;
		int num = month.WorkDays * (24 - (section.HeatingSeasons.Heating.WorkBaseEnd - section.HeatingSeasons.Heating.WorkBaseStart));
		int num2 = month.Saturdays * (24 - (section.HeatingSeasons.Heating.SatBaseEnd - section.HeatingSeasons.Heating.SatBaseStart));
		int num3 = month.Sundays * (24 - (section.HeatingSeasons.Heating.SunBaseEnd - section.HeatingSeasons.Heating.SunBaseStart));
		int num4 = month.Holydays * 24;
		return (calculationData.NonProjectTemperatureRef2 - avgTemp) * (double)(num + num2 + num3 + num4);
	}

	private static double CalculateaHref2(CalculationData calculationdata, Section tempSection, ClimateZones climateZone, MonthlyDays month)
	{
		double avgTemp = PreferencesManager.GetClimateZoneParams(climateZone).SolarRadiation.Months[(int)month.Month].AvgTemp;
		double averageInnerHeatTemp = CalculateAverageHeatTempRef2(tempSection, calculationdata, month);
		double num = CalculateParameterHtrRef(tempSection, avgTemp, averageInnerHeatTemp);
		double num2 = CalcParameterHveRef2(tempSection, calculationdata);
		double num3 = tempSection.Area.HeatedArea * tempSection.Area.HeatCapacity / (num + num2);
		return 1.0 + num3 / 15.0;
	}

	private static double CalculateParameterNignRef2(CalculationData calculationdata, ClimateZones climateZone, MonthlyDays month, double gamma, Section section)
	{
		double num = CalculateaHref2(calculationdata, section, climateZone, month);
		if (gamma > 0.0 && Math.Abs(gamma - 1.0) > 0.01)
		{
			return (1.0 - Math.Pow(gamma, num)) / (1.0 - Math.Pow(gamma, num + 1.0));
		}
		if (gamma < 0.0)
		{
			return 1.0;
		}
		if (Math.Abs(gamma - 1.0) < 0.01)
		{
			return num / (num + 1.0);
		}
		return 0.0;
	}

	private static void ApplyValuesToTempSectionRef2(Section tempSection, CalculationData calcData)
	{
		ApplyUdirectionWalls(tempSection, calcData.UouterWallsRef2, calcData.UinnerWallsRef2, calcData.UwindowsRef2);
		ApplyUroofsAndCeilings(tempSection, calcData.UnontransparentRef2, calcData.UfloorRef2, calcData.UceilingRef2, calcData.UfloorOtherRef2);
		ApplyCoefficientG(tempSection, calcData.gRef2);
	}

	public static void HotWaterCalculationReferences(this CalculationData calc, Section section, CalculationInput calcInput)
	{
		double num = calcInput.BuildingZones.Sum((BuildingZone zone) => zone.Heating.Area.HeatedArea);
		calc.MixedWaterRef1 = calc.ConsumptionRef1 * num / 1000.0;
		calc.MixedWaterRef2 = calc.ConsumptionRef2 * num / 1000.0;
		calc.ResulNetEnergyRef1 = 1.161 * calc.TempDifferenceRef1 * 0.98 * calc.ConsumptionRef1 / 1000.0;
		calc.ResulNetEnergyRef2 = 1.161 * calc.TempDifferenceRef2 * 0.98 * calc.ConsumptionRef2 / 1000.0;
		calc.ResultEnergyForHeatingRef1 = Math.Max(0.0, calc.ResulNetEnergyRef1 - calc.SunEnergyRef1);
		calc.ResultEnergyForHeatingRef2 = Math.Max(0.0, calc.ResulNetEnergyRef2 - calc.SunEnergyRef2);
	}

	public static void HotWaterCalculationActual(this CalculationData calc, Section section, CalculationInput calcInput)
	{
		double num = calcInput.BuildingZones.Sum((BuildingZone zone) => zone.Heating.Area.HeatedArea);
		calc.MixedWaterActual = calc.ConsumptionActual * num / 1000.0;
		calc.ResulNetEnergyActual = 1.161 * calc.TempDifferenceActual * 0.98 * calc.ConsumptionActual / 1000.0;
		calc.ResultEnergyForHeatingActual = Math.Max(0.0, calc.ResulNetEnergyActual - calc.SunEnergyActual);
	}

	public static void HotWaterCalculationBaseLine(this CalculationData calc, Section section, CalculationInput calcInput)
	{
		double num = calcInput.BuildingZones.Sum((BuildingZone zone) => zone.Heating.Area.HeatedArea);
		calc.MixedWaterBaseLine = calc.ConsumptionBaseLine * num / 1000.0;
		calc.ResulNetEnergyBaseLine = 1.161 * calc.TempDifferenceBaseLine * 0.98 * calc.ConsumptionBaseLine / 1000.0;
		calc.ResultEnergyForHeatingBaseLine = Math.Max(0.0, calc.ResulNetEnergyBaseLine - calc.SunEnergyBaseLine);
	}

	public static void HotWaterCalculationESM(this CalculationData calc, Section section, CalculationInput calcInput)
	{
		double num = calcInput.BuildingZones.Sum((BuildingZone zone) => zone.Heating.Area.HeatedArea);
		calc.MixedWaterESM = calc.ConsumptionESM * num / 1000.0;
		calc.ResulNetEnergyESM = 1.161 * calc.TempDifferenceESM * 0.98 * calc.ConsumptionESM / 1000.0;
		calc.ResultEnergyForHeatingESM = Math.Max(0.0, calc.ResulNetEnergyESM - calc.SunEnergyESM);
	}

	public static void CalculateGeneratorHotWaterEfficiencyRef1(this CalculationData calc)
	{
		try
		{
			calc.HeatEfficiencyGeneratingRef1 = (calc.ResultSourceEnergyRef1 * calc.GeneratorHeatEfficiency1Ref1 + calc.ResultSourceEnergy2Ref1 * calc.GeneratorHeatEfficiency2Ref1) / (calc.ResultSourceEnergyRef1 + calc.ResultSourceEnergy2Ref1);
			if (double.IsInfinity(calc.HeatEfficiencyGeneratingRef1) || double.IsNaN(calc.HeatEfficiencyGeneratingRef1))
			{
				calc.HeatEfficiencyGeneratingRef1 = 0.0;
			}
		}
		catch
		{
			calc.HeatEfficiencyGeneratingRef1 = 0.0;
		}
	}

	public static void CalculateGeneratorHotWaterEfficiencyRef2(this CalculationData calc)
	{
		try
		{
			calc.HeatEfficiencyGeneratingRef2 = (calc.ResultSourceEnergyRef2 * calc.GeneratorHeatEfficiency1Ref2 + calc.ResultSourceEnergy2Ref2 * calc.GeneratorHeatEfficiency2Ref2) / (calc.ResultSourceEnergyRef2 + calc.ResultSourceEnergy2Ref2);
			if (double.IsInfinity(calc.HeatEfficiencyGeneratingRef2) || double.IsNaN(calc.HeatEfficiencyGeneratingRef2))
			{
				calc.HeatEfficiencyGeneratingRef2 = 0.0;
			}
		}
		catch
		{
			calc.HeatEfficiencyGeneratingRef2 = 0.0;
		}
	}

	public static void CalculateGeneratorHotWaterEfficiencyActual(this CalculationData calc)
	{
		try
		{
			calc.HeatEfficiencyGeneratingActual = (calc.ResultSourceEnergyActual * calc.GeneratorHeatEfficiency1Actual + calc.ResultSourceEnergy2Actual * calc.GeneratorHeatEfficiency2Actual) / (calc.ResultSourceEnergyActual + calc.ResultSourceEnergy2Actual);
			if (double.IsInfinity(calc.HeatEfficiencyGeneratingActual) || double.IsNaN(calc.HeatEfficiencyGeneratingActual))
			{
				calc.HeatEfficiencyGeneratingActual = 0.0;
			}
		}
		catch
		{
			calc.HeatEfficiencyGeneratingActual = 0.0;
		}
	}

	public static void CalculateGeneratorHotWaterEfficiencyBaseLine(this CalculationData calc)
	{
		try
		{
			calc.HeatEfficiencyGeneratingBaseLine = (calc.ResultSourceEnergyBaseLine * calc.GeneratorHeatEfficiency1BaseLine + calc.ResultSourceEnergy2BaseLine * calc.GeneratorHeatEfficiency2BaseLine) / (calc.ResultSourceEnergyBaseLine + calc.ResultSourceEnergy2BaseLine);
			if (double.IsInfinity(calc.HeatEfficiencyGeneratingBaseLine) || double.IsNaN(calc.HeatEfficiencyGeneratingBaseLine))
			{
				calc.HeatEfficiencyGeneratingBaseLine = 0.0;
			}
		}
		catch
		{
			calc.HeatEfficiencyGeneratingBaseLine = 0.0;
		}
	}

	public static void CalculateGeneratorHotWaterEfficiencyEsm(this CalculationData calc)
	{
		try
		{
			calc.HeatEfficiencyGeneratingESM = (calc.ResultSourceEnergyESM * calc.GeneratorHeatEfficiency1ESM + calc.ResultSourceEnergy2ESM * calc.GeneratorHeatEfficiency2ESM) / (calc.ResultSourceEnergyESM + calc.ResultSourceEnergy2ESM);
			if (double.IsInfinity(calc.HeatEfficiencyGeneratingESM) || double.IsNaN(calc.HeatEfficiencyGeneratingESM))
			{
				calc.HeatEfficiencyGeneratingESM = 0.0;
			}
		}
		catch
		{
			calc.HeatEfficiencyGeneratingESM = 0.0;
		}
	}

	public static void CalculateHotWaterNeededEnergyRef1(this CalculationData calc)
	{
		double num = calc.ResultEnergyForHeatingRef1 * calc.Part1Ref1 / 100.0;
		calc.ResultSourceEnergyRef1 = num / (calc.SupplyNetEfficiencyRef1 / 100.0 * (calc.AutomaticRef1 / 100.0) * (calc.EnergyManagementRef1 / 100.0) * (calc.GeneratorHeatEfficiency1Ref1 / 100.0));
		if (double.IsInfinity(calc.ResultSourceEnergyRef1) || double.IsNaN(calc.ResultSourceEnergyRef1))
		{
			calc.ResultSourceEnergyRef1 = 0.0;
		}
		double num2 = calc.ResultEnergyForHeatingRef1 * calc.Part2Ref1 / 100.0;
		calc.ResultSourceEnergy2Ref1 = num2 / (calc.SupplyNetEfficiency2Ref1 / 100.0 * (calc.Automatic2Ref1 / 100.0) * (calc.EnergyManagement2Ref1 / 100.0) * (calc.GeneratorHeatEfficiency2Ref1 / 100.0));
		if (double.IsInfinity(calc.ResultSourceEnergy2Ref1) || double.IsNaN(calc.ResultSourceEnergy2Ref1))
		{
			calc.ResultSourceEnergy2Ref1 = 0.0;
		}
		calc.ResultNeededEnergyRef1 = calc.ResultSourceEnergyRef1 + calc.ResultSourceEnergy2Ref1;
	}

	public static void CalculateHotWaterNeededEnergyRef2(this CalculationData calc)
	{
		double num = calc.ResultEnergyForHeatingRef2 * calc.Part1Ref2 / 100.0;
		calc.ResultSourceEnergyRef2 = num / (calc.SupplyNetEfficiencyRef2 / 100.0 * (calc.AutomaticRef2 / 100.0) * (calc.EnergyManagementRef2 / 100.0) * (calc.GeneratorHeatEfficiency1Ref2 / 100.0));
		if (double.IsInfinity(calc.ResultSourceEnergyRef2) || double.IsNaN(calc.ResultSourceEnergyRef2))
		{
			calc.ResultSourceEnergyRef2 = 0.0;
		}
		double num2 = calc.ResultEnergyForHeatingRef2 * calc.Part2Ref2 / 100.0;
		calc.ResultSourceEnergy2Ref2 = num2 / (calc.SupplyNetEfficiency2Ref2 / 100.0 * (calc.Automatic2Ref2 / 100.0) * (calc.EnergyManagement2Ref2 / 100.0) * (calc.GeneratorHeatEfficiency2Ref2 / 100.0));
		if (double.IsInfinity(calc.ResultSourceEnergy2Ref2) || double.IsNaN(calc.ResultSourceEnergy2Ref2))
		{
			calc.ResultSourceEnergy2Ref2 = 0.0;
		}
		calc.ResultNeededEnergyRef2 = calc.ResultSourceEnergyRef2 + calc.ResultSourceEnergy2Ref2;
	}

	public static void CalculateHotWaterNeededEnergyActual(this CalculationData calc)
	{
		double num = calc.ResultEnergyForHeatingActual * calc.Part1Actual / 100.0;
		calc.ResultSourceEnergyActual = num / (calc.SupplyNetEfficiencyActual / 100.0 * (calc.AutomaticActual / 100.0) * (calc.EnergyManagementActual / 100.0) * (calc.GeneratorHeatEfficiency1Actual / 100.0));
		if (double.IsInfinity(calc.ResultSourceEnergyActual) || double.IsNaN(calc.ResultSourceEnergyActual))
		{
			calc.ResultSourceEnergyActual = 0.0;
		}
		double num2 = calc.ResultEnergyForHeatingActual * calc.Part2Actual / 100.0;
		calc.ResultSourceEnergy2Actual = num2 / (calc.SupplyNetEfficiency2Actual / 100.0 * (calc.Automatic2Actual / 100.0) * (calc.EnergyManagement2Actual / 100.0) * (calc.GeneratorHeatEfficiency2Actual / 100.0));
		if (double.IsInfinity(calc.ResultSourceEnergy2Actual) || double.IsNaN(calc.ResultSourceEnergy2Actual))
		{
			calc.ResultSourceEnergy2Actual = 0.0;
		}
		calc.ResultNeededEnergyActual = calc.ResultSourceEnergyActual + calc.ResultSourceEnergy2Actual;
	}

	public static void CalculateHotWaterNeededEnergyBaseLine(this CalculationData calc)
	{
		double num = calc.ResultEnergyForHeatingBaseLine * calc.Part1BaseLine / 100.0;
		calc.ResultSourceEnergyBaseLine = num / (calc.SupplyNetEfficiencyBaseLine / 100.0 * (calc.AutomaticBaseLine / 100.0) * (calc.EnergyManagementBaseLine / 100.0) * (calc.GeneratorHeatEfficiency1BaseLine / 100.0));
		if (double.IsInfinity(calc.ResultSourceEnergyBaseLine) || double.IsNaN(calc.ResultSourceEnergyBaseLine))
		{
			calc.ResultSourceEnergyBaseLine = 0.0;
		}
		double num2 = calc.ResultEnergyForHeatingBaseLine * calc.Part2BaseLine / 100.0;
		calc.ResultSourceEnergy2BaseLine = num2 / (calc.SupplyNetEfficiency2BaseLine / 100.0 * (calc.Automatic2BaseLine / 100.0) * (calc.EnergyManagement2BaseLine / 100.0) * (calc.GeneratorHeatEfficiency2BaseLine / 100.0));
		if (double.IsInfinity(calc.ResultSourceEnergy2BaseLine) || double.IsNaN(calc.ResultSourceEnergy2BaseLine))
		{
			calc.ResultSourceEnergy2BaseLine = 0.0;
		}
		calc.ResultNeededEnergyBaseLine = calc.ResultSourceEnergyBaseLine + calc.ResultSourceEnergy2BaseLine;
	}

	public static void CalculateHotWaterNeededEnergyEsm(this CalculationData calc)
	{
		double num = calc.ResultEnergyForHeatingESM * calc.Part1ESM / 100.0;
		calc.ResultSourceEnergyESM = num / (calc.SupplyNetEfficiencyESM / 100.0 * (calc.AutomaticESM / 100.0) * (calc.EnergyManagementESM / 100.0) * (calc.GeneratorHeatEfficiency1ESM / 100.0));
		if (double.IsInfinity(calc.ResultSourceEnergyESM) || double.IsNaN(calc.ResultSourceEnergyESM))
		{
			calc.ResultSourceEnergyESM = 0.0;
		}
		double num2 = calc.ResultEnergyForHeatingESM * calc.Part2ESM / 100.0;
		calc.ResultSourceEnergy2ESM = num2 / (calc.SupplyNetEfficiency2ESM / 100.0 * (calc.Automatic2ESM / 100.0) * (calc.EnergyManagement2ESM / 100.0) * (calc.GeneratorHeatEfficiency2ESM / 100.0));
		if (double.IsInfinity(calc.ResultSourceEnergy2ESM) || double.IsNaN(calc.ResultSourceEnergy2ESM))
		{
			calc.ResultSourceEnergy2ESM = 0.0;
		}
		calc.ResultNeededEnergyESM = calc.ResultSourceEnergyESM + calc.ResultSourceEnergy2ESM;
		calc.ResultNeededEnergySavings = (calc.ResultNeededEnergyBaseLine - calc.ResultNeededEnergyESM).ToString("F3");
	}

	public static void CalculatePeriodsReference(this CalculationData calcData, Section section)
	{
		calcData.CalculateHeatingPeriodRef1(section);
		calcData.CalculateHeatingPeriodRef2(section);
		calcData.CalculateCoolingPeriodRef1(section);
		calcData.CalculateCoolingPeriodRef2(section);
		calcData.CalculateAnnualPeriodRef1(section);
		calcData.CalculateAnnualPeriodRef2(section);
	}

	public static void CalculatePeriodsActual(this CalculationData calcData, Section section)
	{
		calcData.CalculateAnnualPeriodActual(section);
		calcData.CalculateHeatingPeriodActual(section);
		calcData.CalculateCoolingPeriodActual(section);
	}

	public static void CalculatePeriodsBaseLine(this CalculationData calcData, Section section)
	{
		calcData.CalculateAnnualPeriodBaseLine(section);
		calcData.CalculateHeatingPeriodBaseLine(section);
		calcData.CalculateCoolingPeriodBaseLine(section);
	}

	public static void CalculatePeriodsESM(this CalculationData calcData, Section section)
	{
		calcData.CalculateAnnualPeriodESM(section);
		calcData.CalculateHeatingPeriodESM(section);
		calcData.CalculateCoolingPeriodESM(section);
	}

	public static void CalculatePeriodsReferenceBalanced(this CalculationData calcData, Section section)
	{
		calcData.CalculateHeatingPeriodRef1Balanced(section);
		calcData.CalculateHeatingPeriodRef2Balanced(section);
		calcData.CalculateCoolingPeriodRef1Balanced(section);
		calcData.CalculateCoolingPeriodRef2Balanced(section);
		calcData.CalculateAnnualPeriodRef1Balanced(section);
		calcData.CalculateAnnualPeriodRef2Balanced(section);
	}

	public static void CalculatePeriodsActualBalanced(this CalculationData calcData, Section section)
	{
		calcData.CalculateAnnualPeriodActualBalanced(section);
		calcData.CalculateHeatingPeriodActualBalanced(section);
		calcData.CalculateCoolingPeriodActualBalanced(section);
	}

	public static void CalculatePeriodsBaseLineBalanced(this CalculationData calcData, Section section)
	{
		calcData.CalculateAnnualPeriodBaseLineBalanced(section);
		calcData.CalculateHeatingPeriodBaseLineBalanced(section);
		calcData.CalculateCoolingPeriodBaseLineBalanced(section);
	}

	public static void CalculatePeriodsESMBalanced(this CalculationData calcData, Section section)
	{
		calcData.CalculateAnnualPeriodESMBalanced(section);
		calcData.CalculateHeatingPeriodESMBalanced(section);
		calcData.CalculateCoolingPeriodESMBalanced(section);
	}

	public static void CalculatePeriodsReferenceNonBalanced(this CalculationData calcData, Section section)
	{
		calcData.CalculateHeatingPeriodRef1NonBalanced(section);
		calcData.CalculateHeatingPeriodRef2NonBalanced(section);
		calcData.CalculateCoolingPeriodRef1NonBalanced(section);
		calcData.CalculateCoolingPeriodRef2NonBalanced(section);
		calcData.CalculateAnnualPeriodRef1NonBalanced(section);
		calcData.CalculateAnnualPeriodRef2NonBalanced(section);
	}

	public static void CalculatePeriodsActualNonBalanced(this CalculationData calcData, Section section)
	{
		calcData.CalculateAnnualPeriodActualNonBalanced(section);
		calcData.CalculateHeatingPeriodActualNonBalanced(section);
		calcData.CalculateCoolingPeriodActualNonBalanced(section);
	}

	public static void CalculatePeriodsBaseLineNonBalanced(this CalculationData calcData, Section section)
	{
		calcData.CalculateAnnualPeriodBaseLineNonBalanced(section);
		calcData.CalculateHeatingPeriodBaseLineNonBalanced(section);
		calcData.CalculateCoolingPeriodBaseLineNonBalanced(section);
	}

	public static void CalculatePeriodsESMNonBalanced(this CalculationData calcData, Section section)
	{
		calcData.CalculateAnnualPeriodESMNonBalanced(section);
		calcData.CalculateHeatingPeriodESMNonBalanced(section);
		calcData.CalculateCoolingPeriodESMNonBalanced(section);
	}

	public static void CalculatePeriodsReferenceHotWaterPumps(this CalculationData calcData, Section section)
	{
		calcData.CalculateHeatingPeriodRef1HotWaterPumps(section);
		calcData.CalculateHeatingPeriodRef2HotWaterPumps(section);
		calcData.CalculateCoolingPeriodRef1HotWaterPumps(section);
		calcData.CalculateCoolingPeriodRef2HotWaterPumps(section);
		calcData.CalculateAnnualPeriodRef1HotWaterPumps(section);
		calcData.CalculateAnnualPeriodRef2HotWaterPumps(section);
	}

	public static void CalculatePeriodsActualHotWaterPumps(this CalculationData calcData, Section section)
	{
		calcData.CalculateAnnualPeriodActualHotWaterPumps(section);
		calcData.CalculateHeatingPeriodActualHotWaterPumps(section);
		calcData.CalculateCoolingPeriodActualHotWaterPumps(section);
	}

	public static void CalculatePeriodsBaseLineHotWaterPumps(this CalculationData calcData, Section section)
	{
		calcData.CalculateHeatingPeriodBaseLineHotWaterPumps(section);
		calcData.CalculateCoolingPeriodBaseLineHotWaterPumps(section);
		calcData.CalculateAnnualPeriodBaseLineHotWaterPumps(section);
	}

	public static void CalculatePeriodsESMHotWaterPumps(this CalculationData calcData, Section section)
	{
		calcData.CalculateHeatingPeriodESMHotWaterPumps(section);
		calcData.CalculateCoolingPeriodESMHotWaterPumps(section);
		calcData.CalculateAnnualPeriodESMHotWaterPumps(section);
	}

	private static void CalculateHeatingPeriodRef1(this CalculationData calcData, Section section)
	{
		List<MonthlyDays> source = section.CalcPeriod((int)section.HeatingSeason.FirstMonthHeat, (int)section.HeatingSeason.LastMonthHeat, section.HeatingSeason.FirstDayHeat, section.HeatingSeason.LastDayHeat);
		double num = source.Sum((MonthlyDays month) => month.Weeks);
		calcData.Lights.Heating.DevicesNeededEnergyRef1 = calcData.Lights.Heating.WorkScheduleRef1 * calcData.Lights.Heating.PowerRef1 * num / 1000.0;
	}

	private static void CalculateHeatingPeriodRef2(this CalculationData calcData, Section section)
	{
		List<MonthlyDays> source = section.CalcPeriod((int)section.HeatingSeason.FirstMonthHeat, (int)section.HeatingSeason.LastMonthHeat, section.HeatingSeason.FirstDayHeat, section.HeatingSeason.LastDayHeat);
		double num = source.Sum((MonthlyDays month) => month.Weeks);
		calcData.Lights.Heating.DevicesNeededEnergyRef2 = calcData.Lights.Heating.WorkScheduleRef2 * calcData.Lights.Heating.PowerRef2 * num / 1000.0;
	}

	private static void CalculateCoolingPeriodRef1(this CalculationData calcData, Section section)
	{
		List<MonthlyDays> source = section.CalcPeriod((int)section.CoolingSeason.FirstMonthCool, (int)section.CoolingSeason.LastMonthCool, section.CoolingSeason.FirstDayCool, section.CoolingSeason.LastDayCool);
		double num = source.Sum((MonthlyDays month) => month.Weeks);
		calcData.Lights.Cooling.DevicesNeededEnergyRef1 = calcData.Lights.Cooling.WorkScheduleRef1 * calcData.Lights.Cooling.PowerRef1 * num / 1000.0;
	}

	private static void CalculateCoolingPeriodRef2(this CalculationData calcData, Section section)
	{
		List<MonthlyDays> source = section.CalcPeriod((int)section.CoolingSeason.FirstMonthCool, (int)section.CoolingSeason.LastMonthCool, section.CoolingSeason.FirstDayCool, section.CoolingSeason.LastDayCool);
		double num = source.Sum((MonthlyDays month) => month.Weeks);
		calcData.Lights.Cooling.DevicesNeededEnergyRef2 = calcData.Lights.Cooling.WorkScheduleRef2 * calcData.Lights.Cooling.PowerRef2 * num / 1000.0;
	}

	private static void CalculateAnnualPeriodRef1(this CalculationData calcData, Section section)
	{
		List<MonthlyDays> source = section.CalcPeriod(0, 11, 1, 31);
		double num = source.Sum((MonthlyDays month) => month.Weeks);
		calcData.Lights.General.DevicesNeededEnergyRef1 = calcData.Lights.General.WorkScheduleRef1 * calcData.Lights.General.PowerRef1 * num / 1000.0;
	}

	private static void CalculateAnnualPeriodRef2(this CalculationData calcData, Section section)
	{
		List<MonthlyDays> source = section.CalcPeriod(0, 11, 1, 31);
		double num = source.Sum((MonthlyDays month) => month.Weeks);
		calcData.Lights.General.DevicesNeededEnergyRef2 = calcData.Lights.General.WorkScheduleRef2 * calcData.Lights.General.PowerRef2 * num / 1000.0;
	}

	private static void CalculateHeatingPeriodActual(this CalculationData calcData, Section section)
	{
		List<double> list = new List<double>();
		List<double> list2 = new List<double>();
		double num = 0.0;
		List<MonthlyDays> list3 = section.CalcPeriod((int)section.HeatingSeason.FirstMonthHeat, (int)section.HeatingSeason.LastMonthHeat, section.HeatingSeason.FirstDayHeat, section.HeatingSeason.LastDayHeat);
		foreach (MonthlyDays item in list3)
		{
			double weeks = item.Weeks;
			list.Add(CalcAvgMonthPower(calcData.Lights.Actual, item));
			list2.Add(weekRegime * weeks);
			num += weeks;
		}
		if (calcData.Lights.ByMonths)
		{
			double num2 = 0.0;
			double num3 = 0.0;
			for (int i = 0; i < list.Count; i++)
			{
				num2 += list[i] * list2[i];
				num3 += list2[i];
			}
			double num4 = num2 / num3;
			double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);
			calcData.Lights.Heating.DevicesNeededEnergyActual = num4 * num5 / 1000.0;
			if (Math.Abs(num4) > 0.01)
			{
				calcData.Lights.Heating.PowerActual = num4;
			}
			if (Math.Abs(num5) > 0.01)
			{
				calcData.Lights.Heating.WorkScheduleActual = num5 / num;
			}
			if (double.IsNaN(calcData.Lights.Heating.DevicesNeededEnergyActual) || double.IsInfinity(calcData.Lights.Heating.DevicesNeededEnergyActual))
			{
				calcData.Lights.Heating.DevicesNeededEnergyActual = 0.0;
			}
		}
		else
		{
			calcData.Lights.Heating.DevicesNeededEnergyActual = calcData.Lights.Heating.WorkScheduleActual * calcData.Lights.Heating.PowerActual * num / 1000.0;
		}
	}

	private static void CalculateCoolingPeriodActual(this CalculationData calcData, Section section)
	{
		List<double> list = new List<double>();
		List<double> list2 = new List<double>();
		double num = 0.0;
		List<MonthlyDays> list3 = section.CalcPeriod((int)section.CoolingSeason.FirstMonthCool, (int)section.CoolingSeason.LastMonthCool, section.CoolingSeason.FirstDayCool, section.CoolingSeason.LastDayCool);
		foreach (MonthlyDays item in list3)
		{
			double weeks = item.Weeks;
			list.Add(CalcAvgMonthPower(calcData.Lights.Actual, item));
			list2.Add(weekRegime * weeks);
			num += weeks;
		}
		if (calcData.Lights.ByMonths)
		{
			double num2 = 0.0;
			double num3 = 0.0;
			for (int i = 0; i < list.Count; i++)
			{
				num2 += list[i] * list2[i];
				num3 += list2[i];
			}
			double num4 = num2 / num3;
			double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);
			calcData.Lights.Cooling.DevicesNeededEnergyActual = num4 * num5 / 1000.0;
			if (Math.Abs(num4) > 0.01)
			{
				calcData.Lights.Cooling.PowerActual = num4;
			}
			if (Math.Abs(num5) > 0.01)
			{
				calcData.Lights.Cooling.WorkScheduleActual = num5 / num;
			}
			if (double.IsNaN(calcData.Lights.Cooling.DevicesNeededEnergyActual) || double.IsInfinity(calcData.Lights.Cooling.DevicesNeededEnergyActual))
			{
				calcData.Lights.Cooling.DevicesNeededEnergyActual = 0.0;
			}
		}
		else
		{
			calcData.Lights.Cooling.DevicesNeededEnergyActual = calcData.Lights.Cooling.WorkScheduleActual * calcData.Lights.Cooling.PowerActual * num / 1000.0;
		}
	}

	private static void CalculateAnnualPeriodActual(this CalculationData calcData, Section section)
	{
		List<double> list = new List<double>();
		List<double> list2 = new List<double>();
		double num = 0.0;
		List<MonthlyDays> list3 = section.CalcPeriod(0, 11, 1, 31);
		foreach (MonthlyDays item in list3)
		{
			double weeks = item.Weeks;
			list.Add(CalcAvgMonthPower(calcData.Lights.Actual, item));
			list2.Add(weekRegime * weeks);
			num += weeks;
		}
		if (calcData.Lights.ByMonths)
		{
			double num2 = 0.0;
			double num3 = 0.0;
			for (int i = 0; i < list.Count; i++)
			{
				num2 += list[i] * list2[i];
				num3 += list2[i];
			}
			double num4 = num2 / num3;
			double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);
			calcData.Lights.General.DevicesNeededEnergyActual = num4 * num5 / 1000.0;
			if (Math.Abs(num5) > 0.01)
			{
				calcData.Lights.General.PowerActual = num4;
			}
			if (Math.Abs(num5) > 0.01)
			{
				calcData.Lights.General.WorkScheduleActual = num5 / num;
			}
			if (double.IsNaN(calcData.Lights.General.DevicesNeededEnergyActual) || double.IsInfinity(calcData.Lights.General.DevicesNeededEnergyActual))
			{
				calcData.Lights.General.DevicesNeededEnergyActual = 0.0;
			}
		}
		else
		{
			calcData.Lights.General.DevicesNeededEnergyActual = calcData.Lights.General.WorkScheduleActual * calcData.Lights.General.PowerActual * num / 1000.0;
		}
	}

	private static void CalculateHeatingPeriodBaseLine(this CalculationData calcData, Section section)
	{
		List<double> list = new List<double>();
		List<double> list2 = new List<double>();
		double num = 0.0;
		List<MonthlyDays> list3 = section.CalcPeriod((int)section.HeatingSeason.FirstMonthHeat, (int)section.HeatingSeason.LastMonthHeat, section.HeatingSeason.FirstDayHeat, section.HeatingSeason.LastDayHeat);
		foreach (MonthlyDays item in list3)
		{
			double weeks = item.Weeks;
			list.Add(CalcAvgMonthPower(calcData.Lights.BaseLine, item));
			list2.Add(weekRegime * weeks);
			num += weeks;
		}
		if (calcData.Lights.ByMonths)
		{
			double num2 = 0.0;
			double num3 = 0.0;
			for (int i = 0; i < list.Count; i++)
			{
				num2 += list[i] * list2[i];
				num3 += list2[i];
			}
			double num4 = num2 / num3;
			double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);
			calcData.Lights.Heating.DevicesNeededEnergyBaseLine = num4 * num5 / 1000.0;
			if (Math.Abs(num4) > 0.01)
			{
				calcData.Lights.Heating.PowerBaseLine = num4;
			}
			if (Math.Abs(num5) > 0.01)
			{
				calcData.Lights.Heating.WorkScheduleBaseLine = num5 / num;
			}
			if (double.IsNaN(calcData.Lights.Heating.DevicesNeededEnergyBaseLine) || double.IsInfinity(calcData.Lights.Heating.DevicesNeededEnergyBaseLine))
			{
				calcData.Lights.Heating.DevicesNeededEnergyBaseLine = 0.0;
			}
		}
		else
		{
			calcData.Lights.Heating.DevicesNeededEnergyBaseLine = calcData.Lights.Heating.WorkScheduleBaseLine * calcData.Lights.Heating.PowerBaseLine * num / 1000.0;
		}
	}

	private static void CalculateCoolingPeriodBaseLine(this CalculationData calcData, Section section)
	{
		List<double> list = new List<double>();
		List<double> list2 = new List<double>();
		double num = 0.0;
		List<MonthlyDays> list3 = section.CalcPeriod((int)section.CoolingSeason.FirstMonthCool, (int)section.CoolingSeason.LastMonthCool, section.CoolingSeason.FirstDayCool, section.CoolingSeason.LastDayCool);
		foreach (MonthlyDays item in list3)
		{
			double weeks = item.Weeks;
			list.Add(CalcAvgMonthPower(calcData.Lights.BaseLine, item));
			list2.Add(weekRegime * weeks);
			num += weeks;
		}
		if (calcData.Lights.ByMonths)
		{
			double num2 = 0.0;
			double num3 = 0.0;
			for (int i = 0; i < list.Count; i++)
			{
				num2 += list[i] * list2[i];
				num3 += list2[i];
			}
			double num4 = num2 / num3;
			double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);
			calcData.Lights.Cooling.DevicesNeededEnergyBaseLine = num4 * num5 / 1000.0;
			if (Math.Abs(num4) > 0.01)
			{
				calcData.Lights.Cooling.PowerBaseLine = num4;
			}
			if (Math.Abs(num5) > 0.01)
			{
				calcData.Lights.Cooling.WorkScheduleBaseLine = num5 / num;
			}
			if (double.IsNaN(calcData.Lights.Cooling.DevicesNeededEnergyBaseLine) || double.IsInfinity(calcData.Lights.Cooling.DevicesNeededEnergyBaseLine))
			{
				calcData.Lights.Cooling.DevicesNeededEnergyBaseLine = 0.0;
			}
		}
		else
		{
			calcData.Lights.Cooling.DevicesNeededEnergyBaseLine = calcData.Lights.Cooling.WorkScheduleBaseLine * calcData.Lights.Cooling.PowerBaseLine * num / 1000.0;
		}
	}

	private static void CalculateAnnualPeriodBaseLine(this CalculationData calcData, Section section)
	{
		List<double> list = new List<double>();
		List<double> list2 = new List<double>();
		double num = 0.0;
		List<MonthlyDays> list3 = section.CalcPeriod(0, 11, 1, 31);
		foreach (MonthlyDays item in list3)
		{
			double weeks = item.Weeks;
			list.Add(CalcAvgMonthPower(calcData.Lights.BaseLine, item));
			list2.Add(weekRegime * weeks);
			num += weeks;
		}
		if (calcData.Lights.ByMonths)
		{
			double num2 = 0.0;
			double num3 = 0.0;
			for (int i = 0; i < list.Count; i++)
			{
				num2 += list[i] * list2[i];
				num3 += list2[i];
			}
			double num4 = num2 / num3;
			double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);
			calcData.Lights.General.DevicesNeededEnergyBaseLine = num4 * num5 / 1000.0;
			if (Math.Abs(num5) > 0.01)
			{
				calcData.Lights.General.PowerBaseLine = num4;
			}
			if (Math.Abs(num5) > 0.01)
			{
				calcData.Lights.General.WorkScheduleBaseLine = num5 / num;
			}
			if (double.IsNaN(calcData.Lights.General.DevicesNeededEnergyBaseLine) || double.IsInfinity(calcData.Lights.General.DevicesNeededEnergyBaseLine))
			{
				calcData.Lights.General.DevicesNeededEnergyBaseLine = 0.0;
			}
		}
		else
		{
			calcData.Lights.General.DevicesNeededEnergyBaseLine = calcData.Lights.General.WorkScheduleBaseLine * calcData.Lights.General.PowerBaseLine * num / 1000.0;
		}
	}

	private static void CalculateHeatingPeriodESM(this CalculationData calcData, Section section)
	{
		List<double> list = new List<double>();
		List<double> list2 = new List<double>();
		double num = 0.0;
		List<MonthlyDays> list3 = section.CalcPeriod((int)section.HeatingSeason.FirstMonthHeat, (int)section.HeatingSeason.LastMonthHeat, section.HeatingSeason.FirstDayHeat, section.HeatingSeason.LastDayHeat);
		foreach (MonthlyDays item in list3)
		{
			double weeks = item.Weeks;
			list.Add(CalcAvgMonthPower(calcData.Lights.Esm, item));
			list2.Add(weekRegime * weeks);
			num += weeks;
		}
		if (calcData.Lights.ByMonths)
		{
			double num2 = 0.0;
			double num3 = 0.0;
			for (int i = 0; i < list.Count; i++)
			{
				num2 += list[i] * list2[i];
				num3 += list2[i];
			}
			double num4 = num2 / num3;
			double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);
			calcData.Lights.Heating.DevicesNeededEnergyESM = num4 * num5 / 1000.0;
			if (Math.Abs(num5) > 0.01)
			{
				calcData.Lights.Heating.PowerESM = num4;
			}
			if (Math.Abs(num5) > 0.01)
			{
				calcData.Lights.Heating.WorkScheduleESM = num5 / num;
			}
			if (double.IsNaN(calcData.Lights.Heating.DevicesNeededEnergyESM) || double.IsInfinity(calcData.Lights.Heating.DevicesNeededEnergyESM))
			{
				calcData.Lights.Heating.DevicesNeededEnergyESM = 0.0;
			}
		}
		else
		{
			calcData.Lights.Heating.DevicesNeededEnergyESM = calcData.Lights.Heating.WorkScheduleESM * calcData.Lights.Heating.PowerESM * num / 1000.0;
		}
	}

	private static void CalculateCoolingPeriodESM(this CalculationData calcData, Section section)
	{
		List<double> list = new List<double>();
		List<double> list2 = new List<double>();
		double num = 0.0;
		List<MonthlyDays> list3 = section.CalcPeriod((int)section.CoolingSeason.FirstMonthCool, (int)section.CoolingSeason.LastMonthCool, section.CoolingSeason.FirstDayCool, section.CoolingSeason.LastDayCool);
		foreach (MonthlyDays item in list3)
		{
			double weeks = item.Weeks;
			list.Add(CalcAvgMonthPower(calcData.Lights.Esm, item));
			list2.Add(weekRegime * weeks);
			num += weeks;
		}
		if (calcData.Lights.ByMonths)
		{
			double num2 = 0.0;
			double num3 = 0.0;
			for (int i = 0; i < list.Count; i++)
			{
				num2 += list[i] * list2[i];
				num3 += list2[i];
			}
			double num4 = num2 / num3;
			double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);
			calcData.Lights.Cooling.DevicesNeededEnergyESM = num4 * num5 / 1000.0;
			if (Math.Abs(num5) > 0.01)
			{
				calcData.Lights.Cooling.PowerESM = num4;
			}
			if (Math.Abs(num5) > 0.01)
			{
				calcData.Lights.Cooling.WorkScheduleESM = num5 / num;
			}
			if (double.IsNaN(calcData.Lights.Cooling.DevicesNeededEnergyESM) || double.IsInfinity(calcData.Lights.Cooling.DevicesNeededEnergyESM))
			{
				calcData.Lights.Cooling.DevicesNeededEnergyESM = 0.0;
			}
		}
		else
		{
			calcData.Lights.Cooling.DevicesNeededEnergyESM = calcData.Lights.Cooling.WorkScheduleESM * calcData.Lights.Cooling.PowerESM * num / 1000.0;
		}
	}

	private static void CalculateAnnualPeriodESM(this CalculationData calcData, Section section)
	{
		List<double> list = new List<double>();
		List<double> list2 = new List<double>();
		double num = 0.0;
		List<MonthlyDays> list3 = section.CalcPeriod(0, 11, 1, 31);
		foreach (MonthlyDays item in list3)
		{
			double weeks = item.Weeks;
			list.Add(CalcAvgMonthPower(calcData.Lights.Esm, item));
			list2.Add(weekRegime * weeks);
			num += weeks;
		}
		if (calcData.Lights.ByMonths)
		{
			double num2 = 0.0;
			double num3 = 0.0;
			for (int i = 0; i < list.Count; i++)
			{
				num2 += list[i] * list2[i];
				num3 += list2[i];
			}
			double num4 = num2 / num3;
			double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);
			calcData.Lights.General.DevicesNeededEnergyESM = num4 * num5 / 1000.0;
			if (Math.Abs(num5) > 0.01)
			{
				calcData.Lights.General.PowerESM = num4;
			}
			if (Math.Abs(num5) > 0.01)
			{
				calcData.Lights.General.WorkScheduleESM = num5 / num;
			}
			if (double.IsNaN(calcData.Lights.General.DevicesNeededEnergyESM) || double.IsInfinity(calcData.Lights.General.DevicesNeededEnergyESM))
			{
				calcData.Lights.General.DevicesNeededEnergyESM = 0.0;
			}
		}
		else
		{
			calcData.Lights.General.DevicesNeededEnergyESM = calcData.Lights.General.WorkScheduleESM * calcData.Lights.General.PowerESM * num / 1000.0;
		}
	}

	private static void CalculateHeatingPeriodRef1Balanced(this CalculationData calcData, Section section)
	{
		List<MonthlyDays> source = section.CalcPeriod((int)section.HeatingSeason.FirstMonthHeat, (int)section.HeatingSeason.LastMonthHeat, section.HeatingSeason.FirstDayHeat, section.HeatingSeason.LastDayHeat);
		double num = source.Sum((MonthlyDays month) => month.Weeks);
		calcData.BalancedDevices.Heating.DevicesNeededEnergyRef1 = calcData.BalancedDevices.Heating.WorkScheduleRef1 * calcData.BalancedDevices.Heating.PowerRef1 * num / 1000.0;
	}

	private static void CalculateCoolingPeriodRef1Balanced(this CalculationData calcData, Section section)
	{
		List<MonthlyDays> source = section.CalcPeriod((int)section.CoolingSeason.FirstMonthCool, (int)section.CoolingSeason.LastMonthCool, section.CoolingSeason.FirstDayCool, section.CoolingSeason.LastDayCool);
		double num = source.Sum((MonthlyDays month) => month.Weeks);
		calcData.BalancedDevices.Cooling.DevicesNeededEnergyRef1 = calcData.BalancedDevices.Cooling.WorkScheduleRef1 * calcData.BalancedDevices.Cooling.PowerRef1 * num / 1000.0;
	}

	private static void CalculateAnnualPeriodRef1Balanced(this CalculationData calcData, Section section)
	{
		List<MonthlyDays> source = section.CalcPeriod(0, 11, 1, 31);
		double num = source.Sum((MonthlyDays month) => month.Weeks);
		calcData.BalancedDevices.General.DevicesNeededEnergyRef1 = calcData.BalancedDevices.General.WorkScheduleRef1 * calcData.BalancedDevices.General.PowerRef1 * num / 1000.0;
	}

	private static void CalculateHeatingPeriodRef2Balanced(this CalculationData calcData, Section section)
	{
		List<MonthlyDays> source = section.CalcPeriod((int)section.HeatingSeason.FirstMonthHeat, (int)section.HeatingSeason.LastMonthHeat, section.HeatingSeason.FirstDayHeat, section.HeatingSeason.LastDayHeat);
		double num = source.Sum((MonthlyDays month) => month.Weeks);
		calcData.BalancedDevices.Heating.DevicesNeededEnergyRef2 = calcData.BalancedDevices.Heating.WorkScheduleRef2 * calcData.BalancedDevices.Heating.PowerRef2 * num / 1000.0;
	}

	private static void CalculateCoolingPeriodRef2Balanced(this CalculationData calcData, Section section)
	{
		List<MonthlyDays> source = section.CalcPeriod((int)section.CoolingSeason.FirstMonthCool, (int)section.CoolingSeason.LastMonthCool, section.CoolingSeason.FirstDayCool, section.CoolingSeason.LastDayCool);
		double num = source.Sum((MonthlyDays month) => month.Weeks);
		calcData.BalancedDevices.Cooling.DevicesNeededEnergyRef2 = calcData.BalancedDevices.Cooling.WorkScheduleRef2 * calcData.BalancedDevices.Cooling.PowerRef2 * num / 1000.0;
	}

	private static void CalculateAnnualPeriodRef2Balanced(this CalculationData calcData, Section section)
	{
		List<MonthlyDays> source = section.CalcPeriod(0, 11, 1, 31);
		double num = source.Sum((MonthlyDays month) => month.Weeks);
		calcData.BalancedDevices.General.DevicesNeededEnergyRef2 = calcData.BalancedDevices.General.WorkScheduleRef2 * calcData.BalancedDevices.General.PowerRef2 * num / 1000.0;
	}

	private static void CalculateHeatingPeriodActualBalanced(this CalculationData calcData, Section section)
	{
		List<double> list = new List<double>();
		List<double> list2 = new List<double>();
		double num = 0.0;
		List<MonthlyDays> list3 = section.CalcPeriod((int)section.HeatingSeason.FirstMonthHeat, (int)section.HeatingSeason.LastMonthHeat, section.HeatingSeason.FirstDayHeat, section.HeatingSeason.LastDayHeat);
		foreach (MonthlyDays item in list3)
		{
			double weeks = item.Weeks;
			list.Add(CalcAvgMonthPower(calcData.BalancedDevices.Actual, item));
			list2.Add(weekRegime * weeks);
			num += weeks;
		}
		if (calcData.BalancedDevices.ByMonths)
		{
			double num2 = 0.0;
			double num3 = 0.0;
			for (int i = 0; i < list.Count; i++)
			{
				num2 += list[i] * list2[i];
				num3 += list2[i];
			}
			double num4 = num2 / num3;
			double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);
			calcData.BalancedDevices.Heating.DevicesNeededEnergyActual = num4 * num5 / 1000.0;
			if (Math.Abs(num4) > 0.01)
			{
				calcData.BalancedDevices.Heating.PowerActual = num4;
			}
			if (Math.Abs(num5) > 0.01)
			{
				calcData.BalancedDevices.Heating.WorkScheduleActual = num5 / num;
			}
			if (double.IsNaN(calcData.BalancedDevices.Heating.DevicesNeededEnergyActual) || double.IsInfinity(calcData.BalancedDevices.Heating.DevicesNeededEnergyActual))
			{
				calcData.BalancedDevices.Heating.DevicesNeededEnergyActual = 0.0;
			}
		}
		else
		{
			calcData.BalancedDevices.Heating.DevicesNeededEnergyActual = calcData.BalancedDevices.Heating.WorkScheduleActual * calcData.BalancedDevices.Heating.PowerActual * num / 1000.0;
		}
	}

	private static void CalculateCoolingPeriodActualBalanced(this CalculationData calcData, Section section)
	{
		List<double> list = new List<double>();
		List<double> list2 = new List<double>();
		double num = 0.0;
		List<MonthlyDays> list3 = section.CalcPeriod((int)section.CoolingSeason.FirstMonthCool, (int)section.CoolingSeason.LastMonthCool, section.CoolingSeason.FirstDayCool, section.CoolingSeason.LastDayCool);
		foreach (MonthlyDays item in list3)
		{
			double weeks = item.Weeks;
			list.Add(CalcAvgMonthPower(calcData.BalancedDevices.Actual, item));
			list2.Add(weekRegime * weeks);
			num += weeks;
		}
		if (calcData.BalancedDevices.ByMonths)
		{
			double num2 = 0.0;
			double num3 = 0.0;
			for (int i = 0; i < list.Count; i++)
			{
				num2 += list[i] * list2[i];
				num3 += list2[i];
			}
			double num4 = num2 / num3;
			double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);
			calcData.BalancedDevices.Cooling.DevicesNeededEnergyActual = num4 * num5 / 1000.0;
			if (Math.Abs(num4) > 0.01)
			{
				calcData.BalancedDevices.Cooling.PowerActual = num4;
			}
			if (Math.Abs(num5) > 0.01)
			{
				calcData.BalancedDevices.Cooling.WorkScheduleActual = num5 / num;
			}
			if (double.IsNaN(calcData.BalancedDevices.Cooling.DevicesNeededEnergyActual) || double.IsInfinity(calcData.BalancedDevices.Cooling.DevicesNeededEnergyActual))
			{
				calcData.BalancedDevices.Cooling.DevicesNeededEnergyActual = 0.0;
			}
		}
		else
		{
			calcData.BalancedDevices.Cooling.DevicesNeededEnergyActual = calcData.BalancedDevices.Cooling.WorkScheduleActual * calcData.BalancedDevices.Cooling.PowerActual * num / 1000.0;
		}
	}

	private static void CalculateAnnualPeriodActualBalanced(this CalculationData calcData, Section section)
	{
		List<double> list = new List<double>();
		List<double> list2 = new List<double>();
		double num = 0.0;
		List<MonthlyDays> list3 = section.CalcPeriod(0, 11, 1, 31);
		foreach (MonthlyDays item in list3)
		{
			double weeks = item.Weeks;
			list.Add(CalcAvgMonthPower(calcData.BalancedDevices.Actual, item));
			list2.Add(weekRegime * weeks);
			num += weeks;
		}
		if (calcData.BalancedDevices.ByMonths)
		{
			double num2 = 0.0;
			double num3 = 0.0;
			for (int i = 0; i < list.Count; i++)
			{
				num2 += list[i] * list2[i];
				num3 += list2[i];
			}
			double num4 = num2 / num3;
			double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);
			calcData.BalancedDevices.General.DevicesNeededEnergyActual = num4 * num5 / 1000.0;
			if (Math.Abs(num4) > 0.01)
			{
				calcData.BalancedDevices.General.PowerActual = num4;
			}
			if (Math.Abs(num5) > 0.01)
			{
				calcData.BalancedDevices.General.WorkScheduleActual = num5 / num;
			}
			if (double.IsNaN(calcData.BalancedDevices.General.DevicesNeededEnergyActual) || double.IsInfinity(calcData.BalancedDevices.General.DevicesNeededEnergyActual))
			{
				calcData.BalancedDevices.General.DevicesNeededEnergyActual = 0.0;
			}
		}
		else
		{
			calcData.BalancedDevices.General.DevicesNeededEnergyActual = calcData.BalancedDevices.General.WorkScheduleActual * calcData.BalancedDevices.General.PowerActual * num / 1000.0;
		}
	}

	private static void CalculateHeatingPeriodBaseLineBalanced(this CalculationData calcData, Section section)
	{
		List<double> list = new List<double>();
		List<double> list2 = new List<double>();
		double num = 0.0;
		List<MonthlyDays> list3 = section.CalcPeriod((int)section.HeatingSeason.FirstMonthHeat, (int)section.HeatingSeason.LastMonthHeat, section.HeatingSeason.FirstDayHeat, section.HeatingSeason.LastDayHeat);
		foreach (MonthlyDays item in list3)
		{
			double weeks = item.Weeks;
			list.Add(CalcAvgMonthPower(calcData.BalancedDevices.BaseLine, item));
			list2.Add(weekRegime * weeks);
			num += weeks;
		}
		if (calcData.BalancedDevices.ByMonths)
		{
			double num2 = 0.0;
			double num3 = 0.0;
			for (int i = 0; i < list.Count; i++)
			{
				num2 += list[i] * list2[i];
				num3 += list2[i];
			}
			double num4 = num2 / num3;
			double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);
			calcData.BalancedDevices.Heating.DevicesNeededEnergyBaseLine = num4 * num5 / 1000.0;
			if (Math.Abs(num4) > 0.01)
			{
				calcData.BalancedDevices.Heating.PowerBaseLine = num4;
			}
			if (Math.Abs(num5) > 0.01)
			{
				calcData.BalancedDevices.Heating.WorkScheduleBaseLine = num5 / num;
			}
			if (double.IsNaN(calcData.BalancedDevices.Heating.DevicesNeededEnergyBaseLine) || double.IsInfinity(calcData.BalancedDevices.Heating.DevicesNeededEnergyBaseLine))
			{
				calcData.BalancedDevices.Heating.DevicesNeededEnergyBaseLine = 0.0;
			}
		}
		else
		{
			calcData.BalancedDevices.Heating.DevicesNeededEnergyBaseLine = calcData.BalancedDevices.Heating.WorkScheduleBaseLine * calcData.BalancedDevices.Heating.PowerBaseLine * num / 1000.0;
		}
	}

	private static void CalculateCoolingPeriodBaseLineBalanced(this CalculationData calcData, Section section)
	{
		List<double> list = new List<double>();
		List<double> list2 = new List<double>();
		double num = 0.0;
		List<MonthlyDays> list3 = section.CalcPeriod((int)section.CoolingSeason.FirstMonthCool, (int)section.CoolingSeason.LastMonthCool, section.CoolingSeason.FirstDayCool, section.CoolingSeason.LastDayCool);
		foreach (MonthlyDays item in list3)
		{
			double weeks = item.Weeks;
			list.Add(CalcAvgMonthPower(calcData.BalancedDevices.BaseLine, item));
			list2.Add(weekRegime * weeks);
			num += weeks;
		}
		if (calcData.BalancedDevices.ByMonths)
		{
			double num2 = 0.0;
			double num3 = 0.0;
			for (int i = 0; i < list.Count; i++)
			{
				num2 += list[i] * list2[i];
				num3 += list2[i];
			}
			double num4 = num2 / num3;
			double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);
			calcData.BalancedDevices.Cooling.DevicesNeededEnergyBaseLine = num4 * num5 / 1000.0;
			if (Math.Abs(num4) > 0.01)
			{
				calcData.BalancedDevices.Cooling.PowerBaseLine = num4;
			}
			if (Math.Abs(num5) > 0.01)
			{
				calcData.BalancedDevices.Cooling.WorkScheduleBaseLine = num5 / num;
			}
			if (double.IsNaN(calcData.BalancedDevices.Cooling.DevicesNeededEnergyBaseLine) || double.IsInfinity(calcData.BalancedDevices.Cooling.DevicesNeededEnergyBaseLine))
			{
				calcData.BalancedDevices.Cooling.DevicesNeededEnergyBaseLine = 0.0;
			}
		}
		else
		{
			calcData.BalancedDevices.Cooling.DevicesNeededEnergyBaseLine = calcData.BalancedDevices.Cooling.WorkScheduleBaseLine * calcData.BalancedDevices.Cooling.PowerBaseLine * num / 1000.0;
		}
	}

	private static void CalculateAnnualPeriodBaseLineBalanced(this CalculationData calcData, Section section)
	{
		List<double> list = new List<double>();
		List<double> list2 = new List<double>();
		double num = 0.0;
		List<MonthlyDays> list3 = section.CalcPeriod(0, 11, 1, 31);
		foreach (MonthlyDays item in list3)
		{
			double weeks = item.Weeks;
			list.Add(CalcAvgMonthPower(calcData.BalancedDevices.BaseLine, item));
			list2.Add(weekRegime * weeks);
			num += weeks;
		}
		if (calcData.BalancedDevices.ByMonths)
		{
			double num2 = 0.0;
			double num3 = 0.0;
			for (int i = 0; i < list.Count; i++)
			{
				num2 += list[i] * list2[i];
				num3 += list2[i];
			}
			double num4 = num2 / num3;
			double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);
			calcData.BalancedDevices.General.DevicesNeededEnergyBaseLine = num4 * num5 / 1000.0;
			if (Math.Abs(num4) > 0.01)
			{
				calcData.BalancedDevices.General.PowerBaseLine = num4;
			}
			if (Math.Abs(num5) > 0.01)
			{
				calcData.BalancedDevices.General.WorkScheduleBaseLine = num5 / num;
			}
			if (double.IsNaN(calcData.BalancedDevices.General.DevicesNeededEnergyBaseLine) || double.IsInfinity(calcData.BalancedDevices.General.DevicesNeededEnergyBaseLine))
			{
				calcData.BalancedDevices.General.DevicesNeededEnergyBaseLine = 0.0;
			}
		}
		else
		{
			calcData.BalancedDevices.General.DevicesNeededEnergyBaseLine = calcData.BalancedDevices.General.WorkScheduleBaseLine * calcData.BalancedDevices.General.PowerBaseLine * num / 1000.0;
		}
	}

	private static void CalculateHeatingPeriodESMBalanced(this CalculationData calcData, Section section)
	{
		List<double> list = new List<double>();
		List<double> list2 = new List<double>();
		double num = 0.0;
		List<MonthlyDays> list3 = section.CalcPeriod((int)section.HeatingSeason.FirstMonthHeat, (int)section.HeatingSeason.LastMonthHeat, section.HeatingSeason.FirstDayHeat, section.HeatingSeason.LastDayHeat);
		foreach (MonthlyDays item in list3)
		{
			double weeks = item.Weeks;
			list.Add(CalcAvgMonthPower(calcData.BalancedDevices.Esm, item));
			list2.Add(weekRegime * weeks);
			num += weeks;
		}
		if (calcData.BalancedDevices.ByMonths)
		{
			double num2 = 0.0;
			double num3 = 0.0;
			for (int i = 0; i < list.Count; i++)
			{
				num2 += list[i] * list2[i];
				num3 += list2[i];
			}
			double num4 = num2 / num3;
			double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);
			calcData.BalancedDevices.Heating.DevicesNeededEnergyESM = num4 * num5 / 1000.0;
			if (Math.Abs(num5) > 0.01)
			{
				calcData.BalancedDevices.Heating.PowerESM = num4;
			}
			if (Math.Abs(num5) > 0.01)
			{
				calcData.BalancedDevices.Heating.WorkScheduleESM = num5 / num;
			}
			if (double.IsNaN(calcData.BalancedDevices.Heating.DevicesNeededEnergyESM) || double.IsInfinity(calcData.BalancedDevices.Heating.DevicesNeededEnergyESM))
			{
				calcData.BalancedDevices.Heating.DevicesNeededEnergyESM = 0.0;
			}
		}
		else
		{
			calcData.BalancedDevices.Heating.DevicesNeededEnergyESM = calcData.BalancedDevices.Heating.WorkScheduleESM * calcData.BalancedDevices.Heating.PowerESM * num / 1000.0;
		}
	}

	private static void CalculateCoolingPeriodESMBalanced(this CalculationData calcData, Section section)
	{
		List<double> list = new List<double>();
		List<double> list2 = new List<double>();
		double num = 0.0;
		List<MonthlyDays> list3 = section.CalcPeriod((int)section.CoolingSeason.FirstMonthCool, (int)section.CoolingSeason.LastMonthCool, section.CoolingSeason.FirstDayCool, section.CoolingSeason.LastDayCool);
		foreach (MonthlyDays item in list3)
		{
			double weeks = item.Weeks;
			list.Add(CalcAvgMonthPower(calcData.BalancedDevices.Esm, item));
			list2.Add(weekRegime * weeks);
			num += weeks;
		}
		if (calcData.BalancedDevices.ByMonths)
		{
			double num2 = 0.0;
			double num3 = 0.0;
			for (int i = 0; i < list.Count; i++)
			{
				num2 += list[i] * list2[i];
				num3 += list2[i];
			}
			double num4 = num2 / num3;
			double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);
			calcData.BalancedDevices.Cooling.DevicesNeededEnergyESM = num4 * num5 / 1000.0;
			if (Math.Abs(num5) > 0.01)
			{
				calcData.BalancedDevices.Cooling.PowerESM = num4;
			}
			if (Math.Abs(num5) > 0.01)
			{
				calcData.BalancedDevices.Cooling.WorkScheduleESM = num5 / num;
			}
			if (double.IsNaN(calcData.BalancedDevices.Cooling.DevicesNeededEnergyESM) || double.IsInfinity(calcData.BalancedDevices.Cooling.DevicesNeededEnergyESM))
			{
				calcData.BalancedDevices.Cooling.DevicesNeededEnergyESM = 0.0;
			}
		}
		else
		{
			calcData.BalancedDevices.Cooling.DevicesNeededEnergyESM = calcData.BalancedDevices.Cooling.WorkScheduleESM * calcData.BalancedDevices.Cooling.PowerESM * num / 1000.0;
		}
	}

	private static void CalculateAnnualPeriodESMBalanced(this CalculationData calcData, Section section)
	{
		List<double> list = new List<double>();
		List<double> list2 = new List<double>();
		double num = 0.0;
		List<MonthlyDays> list3 = section.CalcPeriod(0, 11, 1, 31);
		foreach (MonthlyDays item in list3)
		{
			double weeks = item.Weeks;
			list.Add(CalcAvgMonthPower(calcData.BalancedDevices.Esm, item));
			list2.Add(weekRegime * weeks);
			num += weeks;
		}
		if (calcData.BalancedDevices.ByMonths)
		{
			double num2 = 0.0;
			double num3 = 0.0;
			for (int i = 0; i < list.Count; i++)
			{
				num2 += list[i] * list2[i];
				num3 += list2[i];
			}
			double num4 = num2 / num3;
			double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);
			calcData.BalancedDevices.General.DevicesNeededEnergyESM = num4 * num5 / 1000.0;
			if (Math.Abs(num5) > 0.01)
			{
				calcData.BalancedDevices.General.PowerESM = num4;
			}
			if (Math.Abs(num5) > 0.01)
			{
				calcData.BalancedDevices.General.WorkScheduleESM = num5 / num;
			}
			if (double.IsNaN(calcData.BalancedDevices.General.DevicesNeededEnergyESM) || double.IsInfinity(calcData.BalancedDevices.General.DevicesNeededEnergyESM))
			{
				calcData.BalancedDevices.General.DevicesNeededEnergyESM = 0.0;
			}
		}
		else
		{
			calcData.BalancedDevices.General.DevicesNeededEnergyESM = calcData.BalancedDevices.General.WorkScheduleESM * calcData.BalancedDevices.General.PowerESM * num / 1000.0;
		}
	}

	private static void CalculateHeatingPeriodRef1NonBalanced(this CalculationData calcData, Section section)
	{
		List<MonthlyDays> source = section.CalcPeriod((int)section.HeatingSeason.FirstMonthHeat, (int)section.HeatingSeason.LastMonthHeat, section.HeatingSeason.FirstDayHeat, section.HeatingSeason.LastDayHeat);
		double num = source.Sum((MonthlyDays month) => month.Weeks);
		calcData.NonBalancedDevices.Heating.DevicesNeededEnergyRef1 = calcData.NonBalancedDevices.Heating.WorkScheduleRef1 * calcData.NonBalancedDevices.Heating.PowerRef1 * num / 1000.0;
	}

	private static void CalculateHeatingPeriodRef2NonBalanced(this CalculationData calcData, Section section)
	{
		List<MonthlyDays> source = section.CalcPeriod((int)section.HeatingSeason.FirstMonthHeat, (int)section.HeatingSeason.LastMonthHeat, section.HeatingSeason.FirstDayHeat, section.HeatingSeason.LastDayHeat);
		double num = source.Sum((MonthlyDays month) => month.Weeks);
		calcData.NonBalancedDevices.Heating.DevicesNeededEnergyRef2 = calcData.NonBalancedDevices.Heating.WorkScheduleRef2 * calcData.NonBalancedDevices.Heating.PowerRef2 * num / 1000.0;
	}

	private static void CalculateCoolingPeriodRef1NonBalanced(this CalculationData calcData, Section section)
	{
		List<MonthlyDays> source = section.CalcPeriod((int)section.CoolingSeason.FirstMonthCool, (int)section.CoolingSeason.LastMonthCool, section.CoolingSeason.FirstDayCool, section.CoolingSeason.LastDayCool);
		double num = source.Sum((MonthlyDays month) => month.Weeks);
		calcData.NonBalancedDevices.Cooling.DevicesNeededEnergyRef1 = calcData.NonBalancedDevices.Cooling.WorkScheduleRef1 * calcData.NonBalancedDevices.Cooling.PowerRef1 * num / 1000.0;
	}

	private static void CalculateCoolingPeriodRef2NonBalanced(this CalculationData calcData, Section section)
	{
		List<MonthlyDays> source = section.CalcPeriod((int)section.CoolingSeason.FirstMonthCool, (int)section.CoolingSeason.LastMonthCool, section.CoolingSeason.FirstDayCool, section.CoolingSeason.LastDayCool);
		double num = source.Sum((MonthlyDays month) => month.Weeks);
		calcData.NonBalancedDevices.Cooling.DevicesNeededEnergyRef2 = calcData.NonBalancedDevices.Cooling.WorkScheduleRef2 * calcData.NonBalancedDevices.Cooling.PowerRef2 * num / 1000.0;
	}

	private static void CalculateAnnualPeriodRef1NonBalanced(this CalculationData calcData, Section section)
	{
		List<MonthlyDays> source = section.CalcPeriod(0, 11, 1, 31);
		double num = source.Sum((MonthlyDays month) => month.Weeks);
		calcData.NonBalancedDevices.General.DevicesNeededEnergyRef1 = calcData.NonBalancedDevices.General.WorkScheduleRef1 * calcData.NonBalancedDevices.General.PowerRef1 * num / 1000.0;
	}

	private static void CalculateAnnualPeriodRef2NonBalanced(this CalculationData calcData, Section section)
	{
		List<MonthlyDays> source = section.CalcPeriod(0, 11, 1, 31);
		double num = source.Sum((MonthlyDays month) => month.Weeks);
		calcData.NonBalancedDevices.General.DevicesNeededEnergyRef2 = calcData.NonBalancedDevices.General.WorkScheduleRef2 * calcData.NonBalancedDevices.General.PowerRef2 * num / 1000.0;
	}

	private static void CalculateHeatingPeriodActualNonBalanced(this CalculationData calcData, Section section)
	{
		List<double> list = new List<double>();
		List<double> list2 = new List<double>();
		double num = 0.0;
		List<MonthlyDays> list3 = section.CalcPeriod((int)section.HeatingSeason.FirstMonthHeat, (int)section.HeatingSeason.LastMonthHeat, section.HeatingSeason.FirstDayHeat, section.HeatingSeason.LastDayHeat);
		foreach (MonthlyDays item in list3)
		{
			double weeks = item.Weeks;
			list.Add(CalcAvgMonthPower(calcData.NonBalancedDevices.Actual, item));
			list2.Add(weekRegime * weeks);
			num += weeks;
		}
		if (calcData.NonBalancedDevices.ByMonths)
		{
			double num2 = 0.0;
			double num3 = 0.0;
			for (int i = 0; i < list.Count; i++)
			{
				num2 += list[i] * list2[i];
				num3 += list2[i];
			}
			double num4 = num2 / num3;
			double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);
			calcData.NonBalancedDevices.Heating.DevicesNeededEnergyActual = num4 * num5 / 1000.0;
			if (double.IsNaN(calcData.NonBalancedDevices.Heating.DevicesNeededEnergyActual) || double.IsInfinity(calcData.NonBalancedDevices.Heating.DevicesNeededEnergyActual))
			{
				calcData.NonBalancedDevices.Heating.DevicesNeededEnergyActual = 0.0;
			}
		}
		else
		{
			calcData.NonBalancedDevices.Heating.DevicesNeededEnergyActual = calcData.NonBalancedDevices.Heating.WorkScheduleActual * calcData.NonBalancedDevices.Heating.PowerActual * num / 1000.0;
		}
	}

	private static void CalculateCoolingPeriodActualNonBalanced(this CalculationData calcData, Section section)
	{
		List<double> list = new List<double>();
		List<double> list2 = new List<double>();
		double num = 0.0;
		List<MonthlyDays> list3 = section.CalcPeriod((int)section.CoolingSeason.FirstMonthCool, (int)section.CoolingSeason.LastMonthCool, section.CoolingSeason.FirstDayCool, section.CoolingSeason.LastDayCool);
		foreach (MonthlyDays item in list3)
		{
			double weeks = item.Weeks;
			list.Add(CalcAvgMonthPower(calcData.NonBalancedDevices.Actual, item));
			list2.Add(weekRegime * weeks);
			num += weeks;
		}
		if (calcData.NonBalancedDevices.ByMonths)
		{
			double num2 = 0.0;
			double num3 = 0.0;
			for (int i = 0; i < list.Count; i++)
			{
				num2 += list[i] * list2[i];
				num3 += list2[i];
			}
			double num4 = num2 / num3;
			double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);
			calcData.NonBalancedDevices.Cooling.DevicesNeededEnergyActual = num4 * num5 / 1000.0;
			if (double.IsNaN(calcData.NonBalancedDevices.Cooling.DevicesNeededEnergyActual) || double.IsInfinity(calcData.NonBalancedDevices.Cooling.DevicesNeededEnergyActual))
			{
				calcData.NonBalancedDevices.Cooling.DevicesNeededEnergyActual = 0.0;
			}
		}
		else
		{
			calcData.NonBalancedDevices.Cooling.DevicesNeededEnergyActual = calcData.NonBalancedDevices.Cooling.WorkScheduleActual * calcData.NonBalancedDevices.Cooling.PowerActual * num / 1000.0;
		}
	}

	private static void CalculateAnnualPeriodActualNonBalanced(this CalculationData calcData, Section section)
	{
		List<double> list = new List<double>();
		List<double> list2 = new List<double>();
		double num = 0.0;
		List<MonthlyDays> list3 = section.CalcPeriod(0, 11, 1, 31);
		foreach (MonthlyDays item in list3)
		{
			double weeks = item.Weeks;
			list.Add(CalcAvgMonthPower(calcData.NonBalancedDevices.Actual, item));
			list2.Add(weekRegime * weeks);
			num += weeks;
		}
		if (calcData.NonBalancedDevices.ByMonths)
		{
			double num2 = 0.0;
			double num3 = 0.0;
			for (int i = 0; i < list.Count; i++)
			{
				num2 += list[i] * list2[i];
				num3 += list2[i];
			}
			double num4 = num2 / num3;
			double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);
			calcData.NonBalancedDevices.General.DevicesNeededEnergyActual = num4 * num5 / 1000.0;
			if (double.IsNaN(calcData.NonBalancedDevices.General.DevicesNeededEnergyActual) || double.IsInfinity(calcData.NonBalancedDevices.General.DevicesNeededEnergyActual))
			{
				calcData.NonBalancedDevices.General.DevicesNeededEnergyActual = 0.0;
			}
		}
		else
		{
			calcData.NonBalancedDevices.General.DevicesNeededEnergyActual = calcData.NonBalancedDevices.General.WorkScheduleActual * calcData.NonBalancedDevices.General.PowerActual * num / 1000.0;
		}
	}

	private static void CalculateHeatingPeriodBaseLineNonBalanced(this CalculationData calcData, Section section)
	{
		List<double> list = new List<double>();
		List<double> list2 = new List<double>();
		double num = 0.0;
		List<MonthlyDays> list3 = section.CalcPeriod((int)section.HeatingSeason.FirstMonthHeat, (int)section.HeatingSeason.LastMonthHeat, section.HeatingSeason.FirstDayHeat, section.HeatingSeason.LastDayHeat);
		foreach (MonthlyDays item in list3)
		{
			double weeks = item.Weeks;
			list.Add(CalcAvgMonthPower(calcData.NonBalancedDevices.BaseLine, item));
			list2.Add(weekRegime * weeks);
			num += weeks;
		}
		if (calcData.NonBalancedDevices.ByMonths)
		{
			double num2 = 0.0;
			double num3 = 0.0;
			for (int i = 0; i < list.Count; i++)
			{
				num2 += list[i] * list2[i];
				num3 += list2[i];
			}
			double num4 = num2 / num3;
			double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);
			calcData.NonBalancedDevices.Heating.DevicesNeededEnergyBaseLine = num4 * num5 / 1000.0;
			if (Math.Abs(num4) > 0.01)
			{
				calcData.NonBalancedDevices.Heating.PowerBaseLine = num4;
			}
			if (Math.Abs(num5) > 0.01)
			{
				calcData.NonBalancedDevices.Heating.WorkScheduleBaseLine = num5 / num;
			}
			if (double.IsNaN(calcData.NonBalancedDevices.Heating.DevicesNeededEnergyBaseLine) || double.IsInfinity(calcData.NonBalancedDevices.Heating.DevicesNeededEnergyBaseLine))
			{
				calcData.NonBalancedDevices.Heating.DevicesNeededEnergyBaseLine = 0.0;
			}
		}
		else
		{
			calcData.NonBalancedDevices.Heating.DevicesNeededEnergyBaseLine = calcData.NonBalancedDevices.Heating.WorkScheduleBaseLine * calcData.NonBalancedDevices.Heating.PowerBaseLine * num / 1000.0;
		}
	}

	private static void CalculateCoolingPeriodBaseLineNonBalanced(this CalculationData calcData, Section section)
	{
		List<double> list = new List<double>();
		List<double> list2 = new List<double>();
		double num = 0.0;
		List<MonthlyDays> list3 = section.CalcPeriod((int)section.CoolingSeason.FirstMonthCool, (int)section.CoolingSeason.LastMonthCool, section.CoolingSeason.FirstDayCool, section.CoolingSeason.LastDayCool);
		foreach (MonthlyDays item in list3)
		{
			double weeks = item.Weeks;
			list.Add(CalcAvgMonthPower(calcData.NonBalancedDevices.BaseLine, item));
			list2.Add(weekRegime * weeks);
			num += weeks;
		}
		if (calcData.NonBalancedDevices.ByMonths)
		{
			double num2 = 0.0;
			double num3 = 0.0;
			for (int i = 0; i < list.Count; i++)
			{
				num2 += list[i] * list2[i];
				num3 += list2[i];
			}
			double num4 = num2 / num3;
			double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);
			calcData.NonBalancedDevices.Cooling.DevicesNeededEnergyBaseLine = num4 * num5 / 1000.0;
			if (Math.Abs(num4) > 0.01)
			{
				calcData.NonBalancedDevices.Cooling.PowerBaseLine = num4;
			}
			if (Math.Abs(num5) > 0.01)
			{
				calcData.NonBalancedDevices.Cooling.WorkScheduleBaseLine = num5 / num;
			}
			if (double.IsNaN(calcData.NonBalancedDevices.Cooling.DevicesNeededEnergyBaseLine) || double.IsInfinity(calcData.NonBalancedDevices.Cooling.DevicesNeededEnergyBaseLine))
			{
				calcData.NonBalancedDevices.Cooling.DevicesNeededEnergyBaseLine = 0.0;
			}
		}
		else
		{
			calcData.NonBalancedDevices.Cooling.DevicesNeededEnergyBaseLine = calcData.NonBalancedDevices.Cooling.WorkScheduleBaseLine * calcData.NonBalancedDevices.Cooling.PowerBaseLine * num / 1000.0;
		}
	}

	private static void CalculateAnnualPeriodBaseLineNonBalanced(this CalculationData calcData, Section section)
	{
		List<double> list = new List<double>();
		List<double> list2 = new List<double>();
		double num = 0.0;
		List<MonthlyDays> list3 = section.CalcPeriod(0, 11, 1, 31);
		foreach (MonthlyDays item in list3)
		{
			double weeks = item.Weeks;
			list.Add(CalcAvgMonthPower(calcData.NonBalancedDevices.BaseLine, item));
			list2.Add(weekRegime * weeks);
			num += weeks;
		}
		if (calcData.NonBalancedDevices.ByMonths)
		{
			double num2 = 0.0;
			double num3 = 0.0;
			for (int i = 0; i < list.Count; i++)
			{
				num2 += list[i] * list2[i];
				num3 += list2[i];
			}
			double num4 = num2 / num3;
			double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);
			calcData.NonBalancedDevices.General.DevicesNeededEnergyBaseLine = num4 * num5 / 1000.0;
			if (Math.Abs(num4) > 0.01)
			{
				calcData.NonBalancedDevices.General.PowerBaseLine = num4;
			}
			if (Math.Abs(num5) > 0.01)
			{
				calcData.NonBalancedDevices.General.WorkScheduleBaseLine = num5 / num;
			}
			if (double.IsNaN(calcData.NonBalancedDevices.General.DevicesNeededEnergyBaseLine) || double.IsInfinity(calcData.NonBalancedDevices.General.DevicesNeededEnergyBaseLine))
			{
				calcData.NonBalancedDevices.General.DevicesNeededEnergyBaseLine = 0.0;
			}
		}
		else
		{
			calcData.NonBalancedDevices.General.DevicesNeededEnergyBaseLine = calcData.NonBalancedDevices.General.WorkScheduleBaseLine * calcData.NonBalancedDevices.General.PowerBaseLine * num / 1000.0;
		}
	}

	private static void CalculateHeatingPeriodESMNonBalanced(this CalculationData calcData, Section section)
	{
		List<double> list = new List<double>();
		List<double> list2 = new List<double>();
		double num = 0.0;
		List<MonthlyDays> list3 = section.CalcPeriod((int)section.HeatingSeason.FirstMonthHeat, (int)section.HeatingSeason.LastMonthHeat, section.HeatingSeason.FirstDayHeat, section.HeatingSeason.LastDayHeat);
		foreach (MonthlyDays item in list3)
		{
			double weeks = item.Weeks;
			list.Add(CalcAvgMonthPower(calcData.NonBalancedDevices.Esm, item));
			list2.Add(weekRegime * weeks);
			num += weeks;
		}
		if (calcData.NonBalancedDevices.ByMonths)
		{
			double num2 = 0.0;
			double num3 = 0.0;
			for (int i = 0; i < list.Count; i++)
			{
				num2 += list[i] * list2[i];
				num3 += list2[i];
			}
			double num4 = num2 / num3;
			double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);
			calcData.NonBalancedDevices.Heating.DevicesNeededEnergyESM = num4 * num5 / 1000.0;
			if (Math.Abs(num5) > 0.01)
			{
				calcData.NonBalancedDevices.Heating.PowerESM = num4;
			}
			if (Math.Abs(num5) > 0.01)
			{
				calcData.NonBalancedDevices.Heating.WorkScheduleESM = num5 / num;
			}
			if (double.IsNaN(calcData.NonBalancedDevices.Heating.DevicesNeededEnergyESM) || double.IsInfinity(calcData.NonBalancedDevices.Heating.DevicesNeededEnergyESM))
			{
				calcData.NonBalancedDevices.Heating.DevicesNeededEnergyESM = 0.0;
			}
		}
		else
		{
			calcData.NonBalancedDevices.Heating.DevicesNeededEnergyESM = calcData.NonBalancedDevices.Heating.WorkScheduleESM * calcData.NonBalancedDevices.Heating.PowerESM * num / 1000.0;
		}
	}

	private static void CalculateCoolingPeriodESMNonBalanced(this CalculationData calcData, Section section)
	{
		List<double> list = new List<double>();
		List<double> list2 = new List<double>();
		double num = 0.0;
		List<MonthlyDays> list3 = section.CalcPeriod((int)section.CoolingSeason.FirstMonthCool, (int)section.CoolingSeason.LastMonthCool, section.CoolingSeason.FirstDayCool, section.CoolingSeason.LastDayCool);
		foreach (MonthlyDays item in list3)
		{
			double weeks = item.Weeks;
			list.Add(CalcAvgMonthPower(calcData.NonBalancedDevices.Esm, item));
			list2.Add(weekRegime * weeks);
			num += weeks;
		}
		if (calcData.NonBalancedDevices.ByMonths)
		{
			double num2 = 0.0;
			double num3 = 0.0;
			for (int i = 0; i < list.Count; i++)
			{
				num2 += list[i] * list2[i];
				num3 += list2[i];
			}
			double num4 = num2 / num3;
			double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);
			calcData.NonBalancedDevices.Cooling.DevicesNeededEnergyESM = num4 * num5 / 1000.0;
			if (Math.Abs(num5) > 0.01)
			{
				calcData.NonBalancedDevices.Cooling.PowerESM = num4;
			}
			if (Math.Abs(num5) > 0.01)
			{
				calcData.NonBalancedDevices.Cooling.WorkScheduleESM = num5 / num;
			}
			if (double.IsNaN(calcData.NonBalancedDevices.Cooling.DevicesNeededEnergyESM) || double.IsInfinity(calcData.NonBalancedDevices.Cooling.DevicesNeededEnergyESM))
			{
				calcData.NonBalancedDevices.Cooling.DevicesNeededEnergyESM = 0.0;
			}
		}
		else
		{
			calcData.NonBalancedDevices.Cooling.DevicesNeededEnergyESM = calcData.NonBalancedDevices.Cooling.WorkScheduleESM * calcData.NonBalancedDevices.Cooling.PowerESM * num / 1000.0;
		}
	}

	private static void CalculateAnnualPeriodESMNonBalanced(this CalculationData calcData, Section section)
	{
		List<double> list = new List<double>();
		List<double> list2 = new List<double>();
		double num = 0.0;
		List<MonthlyDays> list3 = section.CalcPeriod(0, 11, 1, 31);
		foreach (MonthlyDays item in list3)
		{
			double weeks = item.Weeks;
			list.Add(CalcAvgMonthPower(calcData.NonBalancedDevices.Esm, item));
			list2.Add(weekRegime * weeks);
			num += weeks;
		}
		if (calcData.NonBalancedDevices.ByMonths)
		{
			double num2 = 0.0;
			double num3 = 0.0;
			for (int i = 0; i < list.Count; i++)
			{
				num2 += list[i] * list2[i];
				num3 += list2[i];
			}
			double num4 = num2 / num3;
			double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);
			calcData.NonBalancedDevices.General.DevicesNeededEnergyESM = num4 * num5 / 1000.0;
			if (Math.Abs(num5) > 0.01)
			{
				calcData.NonBalancedDevices.General.PowerESM = num4;
			}
			if (Math.Abs(num5) > 0.01)
			{
				calcData.NonBalancedDevices.General.WorkScheduleESM = num5 / num;
			}
			if (double.IsNaN(calcData.NonBalancedDevices.General.DevicesNeededEnergyESM) || double.IsInfinity(calcData.NonBalancedDevices.General.DevicesNeededEnergyESM))
			{
				calcData.NonBalancedDevices.General.DevicesNeededEnergyESM = 0.0;
			}
		}
		else
		{
			calcData.NonBalancedDevices.General.DevicesNeededEnergyESM = calcData.NonBalancedDevices.General.WorkScheduleESM * calcData.NonBalancedDevices.General.PowerESM * num / 1000.0;
		}
	}

	private static void CalculateHeatingPeriodRef1HotWaterPumps(this CalculationData calcData, Section section)
	{
		List<MonthlyDays> source = section.CalcPeriod((int)section.HeatingSeason.FirstMonthHeat, (int)section.HeatingSeason.LastMonthHeat, section.HeatingSeason.FirstDayHeat, section.HeatingSeason.LastDayHeat);
		double num = source.Sum((MonthlyDays month) => month.Weeks);
		calcData.HotWaterPumps.Heating.DevicesNeededEnergyRef1 = calcData.HotWaterPumps.Heating.WorkScheduleRef1 * calcData.HotWaterPumps.Heating.PowerRef1 * num / 1000.0;
	}

	private static void CalculateCoolingPeriodRef1HotWaterPumps(this CalculationData calcData, Section section)
	{
		List<MonthlyDays> source = section.CalcPeriod((int)section.CoolingSeason.FirstMonthCool, (int)section.CoolingSeason.LastMonthCool, section.CoolingSeason.FirstDayCool, section.CoolingSeason.LastDayCool);
		double num = source.Sum((MonthlyDays month) => month.Weeks);
		calcData.HotWaterPumps.Cooling.DevicesNeededEnergyRef1 = calcData.HotWaterPumps.Cooling.WorkScheduleRef1 * calcData.HotWaterPumps.Cooling.PowerRef1 * num / 1000.0;
	}

	private static void CalculateAnnualPeriodRef1HotWaterPumps(this CalculationData calcData, Section section)
	{
		List<MonthlyDays> source = section.CalcPeriod(0, 11, 1, 31);
		double num = source.Sum((MonthlyDays month) => month.Weeks);
		calcData.HotWaterPumps.General.DevicesNeededEnergyRef1 = calcData.HotWaterPumps.General.WorkScheduleRef1 * calcData.HotWaterPumps.General.PowerRef1 * num / 1000.0;
	}

	private static void CalculateHeatingPeriodRef2HotWaterPumps(this CalculationData calcData, Section section)
	{
		List<MonthlyDays> source = section.CalcPeriod((int)section.HeatingSeason.FirstMonthHeat, (int)section.HeatingSeason.LastMonthHeat, section.HeatingSeason.FirstDayHeat, section.HeatingSeason.LastDayHeat);
		double num = source.Sum((MonthlyDays month) => month.Weeks);
		calcData.HotWaterPumps.Heating.DevicesNeededEnergyRef2 = calcData.HotWaterPumps.Heating.WorkScheduleRef2 * calcData.HotWaterPumps.Heating.PowerRef2 * num / 1000.0;
	}

	private static void CalculateCoolingPeriodRef2HotWaterPumps(this CalculationData calcData, Section section)
	{
		List<MonthlyDays> source = section.CalcPeriod((int)section.CoolingSeason.FirstMonthCool, (int)section.CoolingSeason.LastMonthCool, section.CoolingSeason.FirstDayCool, section.CoolingSeason.LastDayCool);
		double num = source.Sum((MonthlyDays month) => month.Weeks);
		calcData.HotWaterPumps.Cooling.DevicesNeededEnergyRef2 = calcData.HotWaterPumps.Cooling.WorkScheduleRef2 * calcData.HotWaterPumps.Cooling.PowerRef2 * num / 1000.0;
	}

	private static void CalculateAnnualPeriodRef2HotWaterPumps(this CalculationData calcData, Section section)
	{
		List<MonthlyDays> source = section.CalcPeriod(0, 11, 1, 31);
		double num = source.Sum((MonthlyDays month) => month.Weeks);
		calcData.HotWaterPumps.General.DevicesNeededEnergyRef2 = calcData.HotWaterPumps.General.WorkScheduleRef2 * calcData.HotWaterPumps.General.PowerRef2 * num / 1000.0;
	}

	private static void CalculateHeatingPeriodActualHotWaterPumps(this CalculationData calcData, Section section)
	{
		List<double> list = new List<double>();
		List<double> list2 = new List<double>();
		double num = 0.0;
		List<MonthlyDays> list3 = section.CalcPeriod((int)section.HeatingSeason.FirstMonthHeat, (int)section.HeatingSeason.LastMonthHeat, section.HeatingSeason.FirstDayHeat, section.HeatingSeason.LastDayHeat);
		foreach (MonthlyDays item in list3)
		{
			double weeks = item.Weeks;
			list.Add(CalcAvgMonthPower(calcData.HotWaterPumps.Actual, item));
			list2.Add(weekRegime * weeks);
			num += weeks;
		}
		if (calcData.HotWaterPumps.ByMonths)
		{
			double num2 = 0.0;
			double num3 = 0.0;
			for (int i = 0; i < list.Count; i++)
			{
				num2 += list[i] * list2[i];
				num3 += list2[i];
			}
			double num4 = num2 / num3;
			double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);
			calcData.HotWaterPumps.Heating.DevicesNeededEnergyActual = num4 * num5 / 1000.0;
			if (double.IsNaN(calcData.HotWaterPumps.Heating.DevicesNeededEnergyActual) || double.IsInfinity(calcData.HotWaterPumps.Heating.DevicesNeededEnergyActual))
			{
				calcData.HotWaterPumps.Heating.DevicesNeededEnergyActual = 0.0;
			}
		}
		else
		{
			calcData.HotWaterPumps.Heating.DevicesNeededEnergyActual = calcData.HotWaterPumps.Heating.WorkScheduleActual * calcData.HotWaterPumps.Heating.PowerActual * num / 1000.0;
		}
	}

	private static void CalculateCoolingPeriodActualHotWaterPumps(this CalculationData calcData, Section section)
	{
		List<double> list = new List<double>();
		List<double> list2 = new List<double>();
		double num = 0.0;
		List<MonthlyDays> list3 = section.CalcPeriod((int)section.CoolingSeason.FirstMonthCool, (int)section.CoolingSeason.LastMonthCool, section.CoolingSeason.FirstDayCool, section.CoolingSeason.LastDayCool);
		foreach (MonthlyDays item in list3)
		{
			double weeks = item.Weeks;
			list.Add(CalcAvgMonthPower(calcData.HotWaterPumps.Actual, item));
			list2.Add(weekRegime * weeks);
			num += weeks;
		}
		if (calcData.HotWaterPumps.ByMonths)
		{
			double num2 = 0.0;
			double num3 = 0.0;
			for (int i = 0; i < list.Count; i++)
			{
				num2 += list[i] * list2[i];
				num3 += list2[i];
			}
			double num4 = num2 / num3;
			double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);
			calcData.HotWaterPumps.Cooling.DevicesNeededEnergyActual = num4 * num5 / 1000.0;
			if (double.IsNaN(calcData.HotWaterPumps.Cooling.DevicesNeededEnergyActual) || double.IsInfinity(calcData.HotWaterPumps.Cooling.DevicesNeededEnergyActual))
			{
				calcData.HotWaterPumps.Cooling.DevicesNeededEnergyActual = 0.0;
			}
		}
		else
		{
			calcData.HotWaterPumps.Cooling.DevicesNeededEnergyActual = calcData.HotWaterPumps.Cooling.WorkScheduleActual * calcData.HotWaterPumps.Cooling.PowerActual * num / 1000.0;
		}
	}

	private static void CalculateAnnualPeriodActualHotWaterPumps(this CalculationData calcData, Section section)
	{
		List<double> list = new List<double>();
		List<double> list2 = new List<double>();
		double num = 0.0;
		List<MonthlyDays> list3 = section.CalcPeriod(0, 11, 1, 31);
		foreach (MonthlyDays item in list3)
		{
			double weeks = item.Weeks;
			list.Add(CalcAvgMonthPower(calcData.HotWaterPumps.Actual, item));
			list2.Add(weekRegime * weeks);
			num += weeks;
		}
		if (calcData.HotWaterPumps.ByMonths)
		{
			double num2 = 0.0;
			double num3 = 0.0;
			for (int i = 0; i < list.Count; i++)
			{
				num2 += list[i] * list2[i];
				num3 += list2[i];
			}
			double num4 = num2 / num3;
			double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);
			calcData.HotWaterPumps.General.DevicesNeededEnergyActual = num4 * num5 / 1000.0;
			if (double.IsNaN(calcData.HotWaterPumps.General.DevicesNeededEnergyActual) || double.IsInfinity(calcData.HotWaterPumps.General.DevicesNeededEnergyActual))
			{
				calcData.HotWaterPumps.General.DevicesNeededEnergyActual = 0.0;
			}
		}
		else
		{
			calcData.HotWaterPumps.General.DevicesNeededEnergyActual = calcData.HotWaterPumps.General.WorkScheduleActual * calcData.HotWaterPumps.General.PowerActual * num / 1000.0;
		}
	}

	private static void CalculateHeatingPeriodBaseLineHotWaterPumps(this CalculationData calcData, Section section)
	{
		List<double> list = new List<double>();
		List<double> list2 = new List<double>();
		double num = 0.0;
		List<MonthlyDays> list3 = section.CalcPeriod((int)section.HeatingSeason.FirstMonthHeat, (int)section.HeatingSeason.LastMonthHeat, section.HeatingSeason.FirstDayHeat, section.HeatingSeason.LastDayHeat);
		foreach (MonthlyDays item in list3)
		{
			double weeks = item.Weeks;
			list.Add(CalcAvgMonthPower(calcData.HotWaterPumps.BaseLine, item));
			list2.Add(weekRegime * weeks);
			num += weeks;
		}
		if (calcData.HotWaterPumps.ByMonths)
		{
			double num2 = 0.0;
			double num3 = 0.0;
			for (int i = 0; i < list.Count; i++)
			{
				num2 += list[i] * list2[i];
				num3 += list2[i];
			}
			double num4 = num2 / num3;
			double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);
			calcData.HotWaterPumps.Heating.DevicesNeededEnergyBaseLine = num4 * num5 / 1000.0;
			if (Math.Abs(num4) > 0.01)
			{
				calcData.HotWaterPumps.Heating.PowerBaseLine = num4;
			}
			if (Math.Abs(num5) > 0.01)
			{
				calcData.HotWaterPumps.Heating.WorkScheduleBaseLine = num5 / num;
			}
			if (double.IsNaN(calcData.HotWaterPumps.Heating.DevicesNeededEnergyBaseLine) || double.IsInfinity(calcData.HotWaterPumps.Heating.DevicesNeededEnergyBaseLine))
			{
				calcData.HotWaterPumps.Heating.DevicesNeededEnergyBaseLine = 0.0;
			}
		}
		else
		{
			calcData.HotWaterPumps.Heating.DevicesNeededEnergyBaseLine = calcData.HotWaterPumps.Heating.WorkScheduleBaseLine * calcData.HotWaterPumps.Heating.PowerBaseLine * num / 1000.0;
		}
	}

	private static void CalculateCoolingPeriodBaseLineHotWaterPumps(this CalculationData calcData, Section section)
	{
		List<double> list = new List<double>();
		List<double> list2 = new List<double>();
		double num = 0.0;
		List<MonthlyDays> list3 = section.CalcPeriod((int)section.CoolingSeason.FirstMonthCool, (int)section.CoolingSeason.LastMonthCool, section.CoolingSeason.FirstDayCool, section.CoolingSeason.LastDayCool);
		foreach (MonthlyDays item in list3)
		{
			double weeks = item.Weeks;
			list.Add(CalcAvgMonthPower(calcData.HotWaterPumps.BaseLine, item));
			list2.Add(weekRegime * weeks);
			num += weeks;
		}
		if (calcData.HotWaterPumps.ByMonths)
		{
			double num2 = 0.0;
			double num3 = 0.0;
			for (int i = 0; i < list.Count; i++)
			{
				num2 += list[i] * list2[i];
				num3 += list2[i];
			}
			double num4 = num2 / num3;
			double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);
			calcData.HotWaterPumps.Cooling.DevicesNeededEnergyBaseLine = num4 * num5 / 1000.0;
			if (Math.Abs(num4) > 0.01)
			{
				calcData.HotWaterPumps.Cooling.PowerBaseLine = num4;
			}
			if (Math.Abs(num5) > 0.01)
			{
				calcData.HotWaterPumps.Cooling.WorkScheduleBaseLine = num5 / num;
			}
			if (double.IsNaN(calcData.HotWaterPumps.Cooling.DevicesNeededEnergyBaseLine) || double.IsInfinity(calcData.HotWaterPumps.Cooling.DevicesNeededEnergyBaseLine))
			{
				calcData.HotWaterPumps.Cooling.DevicesNeededEnergyBaseLine = 0.0;
			}
		}
		else
		{
			calcData.HotWaterPumps.Cooling.DevicesNeededEnergyBaseLine = calcData.HotWaterPumps.Cooling.WorkScheduleBaseLine * calcData.HotWaterPumps.Cooling.PowerBaseLine * num / 1000.0;
		}
	}

	private static void CalculateAnnualPeriodBaseLineHotWaterPumps(this CalculationData calcData, Section section)
	{
		List<double> list = new List<double>();
		List<double> list2 = new List<double>();
		double num = 0.0;
		List<MonthlyDays> list3 = section.CalcPeriod(0, 11, 1, 31);
		foreach (MonthlyDays item in list3)
		{
			double weeks = item.Weeks;
			list.Add(CalcAvgMonthPower(calcData.HotWaterPumps.BaseLine, item));
			list2.Add(weekRegime * weeks);
			num += weeks;
		}
		if (calcData.HotWaterPumps.ByMonths)
		{
			double num2 = 0.0;
			double num3 = 0.0;
			for (int i = 0; i < list.Count; i++)
			{
				num2 += list[i] * list2[i];
				num3 += list2[i];
			}
			double num4 = num2 / num3;
			double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);
			calcData.HotWaterPumps.General.DevicesNeededEnergyBaseLine = num4 * num5 / 1000.0;
			if (Math.Abs(num4) > 0.01)
			{
				calcData.HotWaterPumps.General.PowerBaseLine = num4;
			}
			if (Math.Abs(num5) > 0.01)
			{
				calcData.HotWaterPumps.General.WorkScheduleBaseLine = num5 / num;
			}
			if (double.IsNaN(calcData.HotWaterPumps.General.DevicesNeededEnergyBaseLine) || double.IsInfinity(calcData.HotWaterPumps.General.DevicesNeededEnergyBaseLine))
			{
				calcData.HotWaterPumps.General.DevicesNeededEnergyBaseLine = 0.0;
			}
		}
		else
		{
			calcData.HotWaterPumps.General.DevicesNeededEnergyBaseLine = calcData.HotWaterPumps.General.WorkScheduleBaseLine * calcData.HotWaterPumps.General.PowerBaseLine * num / 1000.0;
		}
	}

	private static void CalculateHeatingPeriodESMHotWaterPumps(this CalculationData calcData, Section section)
	{
		List<double> list = new List<double>();
		List<double> list2 = new List<double>();
		double num = 0.0;
		List<MonthlyDays> list3 = section.CalcPeriod((int)section.HeatingSeason.FirstMonthHeat, (int)section.HeatingSeason.LastMonthHeat, section.HeatingSeason.FirstDayHeat, section.HeatingSeason.LastDayHeat);
		foreach (MonthlyDays item in list3)
		{
			double weeks = item.Weeks;
			list.Add(CalcAvgMonthPower(calcData.HotWaterPumps.Esm, item));
			list2.Add(weekRegime * weeks);
			num += weeks;
		}
		if (calcData.HotWaterPumps.ByMonths)
		{
			double num2 = 0.0;
			double num3 = 0.0;
			for (int i = 0; i < list.Count; i++)
			{
				num2 += list[i] * list2[i];
				num3 += list2[i];
			}
			double num4 = num2 / num3;
			double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);
			calcData.HotWaterPumps.Heating.DevicesNeededEnergyESM = num4 * num5 / 1000.0;
			if (Math.Abs(num5) > 0.01)
			{
				calcData.HotWaterPumps.Heating.PowerESM = num4;
			}
			if (Math.Abs(num5) > 0.01)
			{
				calcData.HotWaterPumps.Heating.WorkScheduleESM = num5 / num;
			}
			if (double.IsNaN(calcData.HotWaterPumps.Heating.DevicesNeededEnergyESM) || double.IsInfinity(calcData.HotWaterPumps.Heating.DevicesNeededEnergyESM))
			{
				calcData.HotWaterPumps.Heating.DevicesNeededEnergyESM = 0.0;
			}
		}
		else
		{
			calcData.HotWaterPumps.Heating.DevicesNeededEnergyESM = calcData.HotWaterPumps.Heating.WorkScheduleESM * calcData.HotWaterPumps.Heating.PowerESM * num / 1000.0;
			calcData.HotWaterPumps.Heating.DevicesNeededEnergySavings = (calcData.HotWaterPumps.Heating.DevicesNeededEnergyBaseLine - calcData.HotWaterPumps.Heating.DevicesNeededEnergyESM).ToString("F3");
		}
	}

	private static void CalculateCoolingPeriodESMHotWaterPumps(this CalculationData calcData, Section section)
	{
		List<double> list = new List<double>();
		List<double> list2 = new List<double>();
		double num = 0.0;
		List<MonthlyDays> list3 = section.CalcPeriod((int)section.CoolingSeason.FirstMonthCool, (int)section.CoolingSeason.LastMonthCool, section.CoolingSeason.FirstDayCool, section.CoolingSeason.LastDayCool);
		foreach (MonthlyDays item in list3)
		{
			double weeks = item.Weeks;
			list.Add(CalcAvgMonthPower(calcData.HotWaterPumps.Esm, item));
			list2.Add(weekRegime * weeks);
			num += weeks;
		}
		if (calcData.HotWaterPumps.ByMonths)
		{
			double num2 = 0.0;
			double num3 = 0.0;
			for (int i = 0; i < list.Count; i++)
			{
				num2 += list[i] * list2[i];
				num3 += list2[i];
			}
			double num4 = num2 / num3;
			double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);
			calcData.HotWaterPumps.Cooling.DevicesNeededEnergyESM = num4 * num5 / 1000.0;
			if (Math.Abs(num5) > 0.01)
			{
				calcData.HotWaterPumps.Cooling.PowerESM = num4;
			}
			if (Math.Abs(num5) > 0.01)
			{
				calcData.HotWaterPumps.Cooling.WorkScheduleESM = num5 / num;
			}
			if (double.IsNaN(calcData.HotWaterPumps.Cooling.DevicesNeededEnergyESM) || double.IsInfinity(calcData.HotWaterPumps.Cooling.DevicesNeededEnergyESM))
			{
				calcData.HotWaterPumps.Cooling.DevicesNeededEnergyESM = 0.0;
			}
		}
		else
		{
			calcData.HotWaterPumps.Cooling.DevicesNeededEnergyESM = calcData.HotWaterPumps.Cooling.WorkScheduleESM * calcData.HotWaterPumps.Cooling.PowerESM * num / 1000.0;
			calcData.HotWaterPumps.Cooling.DevicesNeededEnergySavings = (calcData.HotWaterPumps.Cooling.DevicesNeededEnergyBaseLine - calcData.HotWaterPumps.Cooling.DevicesNeededEnergyESM).ToString("F3");
		}
	}

	private static void CalculateAnnualPeriodESMHotWaterPumps(this CalculationData calcData, Section section)
	{
		List<double> list = new List<double>();
		List<double> list2 = new List<double>();
		double num = 0.0;
		List<MonthlyDays> list3 = section.CalcPeriod(0, 11, 1, 31);
		foreach (MonthlyDays item in list3)
		{
			double weeks = item.Weeks;
			list.Add(CalcAvgMonthPower(calcData.HotWaterPumps.Esm, item));
			list2.Add(weekRegime * weeks);
			num += weeks;
		}
		if (calcData.HotWaterPumps.ByMonths)
		{
			double num2 = 0.0;
			double num3 = 0.0;
			for (int i = 0; i < list.Count; i++)
			{
				num2 += list[i] * list2[i];
				num3 += list2[i];
			}
			double num4 = num2 / num3;
			double num5 = list2.Aggregate(0.0, (double num6, double item) => num6 + item);
			calcData.HotWaterPumps.General.DevicesNeededEnergyESM = num4 * num5 / 1000.0;
			if (Math.Abs(num5) > 0.01)
			{
				calcData.HotWaterPumps.General.PowerESM = num4;
			}
			if (Math.Abs(num5) > 0.01)
			{
				calcData.HotWaterPumps.General.WorkScheduleESM = num5 / num;
			}
			if (double.IsNaN(calcData.HotWaterPumps.General.DevicesNeededEnergyESM) || double.IsInfinity(calcData.HotWaterPumps.General.DevicesNeededEnergyESM))
			{
				calcData.HotWaterPumps.General.DevicesNeededEnergyESM = 0.0;
			}
		}
		else
		{
			calcData.HotWaterPumps.General.DevicesNeededEnergyESM = calcData.HotWaterPumps.General.WorkScheduleESM * calcData.HotWaterPumps.General.PowerESM * num / 1000.0;
			calcData.HotWaterPumps.General.DevicesNeededEnergySavings = (calcData.HotWaterPumps.General.DevicesNeededEnergyBaseLine - calcData.HotWaterPumps.General.DevicesNeededEnergyESM).ToString("F3");
		}
	}

	private static double CalcAvgMonthPower(ScheduleMonth schedule, MonthlyDays month)
	{
		return month.Month switch
		{
			Month.January => CalcWeekPower(schedule.January), 
			Month.February => CalcWeekPower(schedule.February), 
			Month.March => CalcWeekPower(schedule.March), 
			Month.April => CalcWeekPower(schedule.April), 
			Month.May => CalcWeekPower(schedule.May), 
			Month.June => CalcWeekPower(schedule.June), 
			Month.July => CalcWeekPower(schedule.July), 
			Month.August => CalcWeekPower(schedule.August), 
			Month.September => CalcWeekPower(schedule.September), 
			Month.October => CalcWeekPower(schedule.October), 
			Month.November => CalcWeekPower(schedule.November), 
			Month.December => CalcWeekPower(schedule.December), 
			_ => double.NaN, 
		};
	}

	private static double CalcWeekPower(MonthState month)
	{
		double workDays = month.WorkDays;
		double workDaysUsedEnergy = month.WorkDaysUsedEnergy;
		double saturdays = month.Saturdays;
		double saturdaysUsedEnergy = month.SaturdaysUsedEnergy;
		double sundays = month.Sundays;
		double sundaysUsedEnergy = month.SundaysUsedEnergy;
		weekRegime = workDays * 5.0 + saturdays + sundays;
		double num = (workDays * workDaysUsedEnergy * 5.0 + saturdays * saturdaysUsedEnergy + sundays * sundaysUsedEnergy) / (workDays * 5.0 + saturdays + sundays);
		if (double.IsNaN(num) || double.IsInfinity(num))
		{
			num = 0.0;
		}
		return num;
	}

	public static void CalculateZonePowerEnergy(this Results zoneBalanceResult, Section section, BuildingZone zone, Results results)
	{
		ClearFuelCellsPowerTable(results);
		zone.HeatingCalculations.HeatingResult.CalculateHeatingPower(section, zone, results);
		CalculateEnergySourcePowerFuel1(zone, results);
		CalculateEnergySourcePowerFuel2(zone, results);
		CalculateEnergySourcePowerFuel1BaseLine(section, zone, results);
		CalculateEnergySourcePowerFuel2BaseLine(section, zone, results);
		CalculateEnergySourcePowerFuel1Esm(section, zone, results);
		CalculateEnergySourcePowerFuel2Esm(section, zone, results);
		CalculateEnergySourcePowerFuel1Area(section, zone, results);
		CalculateEnergySourcePowerFuel2Area(section, zone, results);
		CalculateEnergySourcePowerFuel1BaseLineArea(section, zone, results);
		CalculateEnergySourcePowerFuel2BaseLineArea(section, zone, results);
		CalculateEnergySourcePowerFuel1EsmArea(section, zone, results);
		CalculateEnergySourcePowerFuel2EsmArea(section, zone, results);
	}

	public static void CalculateBuildingPowerEnergy(CalculationInput calcInput, Results buildingBalanceResult)
	{
		foreach (BuildingZone buildingZone in calcInput.BuildingZones)
		{
			buildingBalanceResult.CalculateZonePowerEnergy(buildingZone.Heating, buildingZone, buildingZone.ZoneResults);
		}
		CalculateBuildingHeatingPower(calcInput, buildingBalanceResult);
		CalculateBuildingSourcePower(calcInput, buildingBalanceResult);
	}

	public static void CalculateBuildingHeatingPower(CalculationInput calcInput, Results buildingBalanceResult)
	{
		if (calcInput.BuildingZones.Any((BuildingZone zone) => zone.HasHeating))
		{
			double num = calcInput.BuildingZones.Sum((BuildingZone buildingZone) => buildingZone.Heating.Area.HeatedArea);
			buildingBalanceResult.PowerBudgetTable.Heating.Actual = calcInput.BuildingZones.Sum((BuildingZone zone) => zone.ZoneResults.PowerBudgetTable.Heating.Actual);
			buildingBalanceResult.PowerBudgetTable.Heating.ActualArea = buildingBalanceResult.PowerBudgetTable.Heating.Actual * 1000.0 / num;
			buildingBalanceResult.PowerBudgetTable.Heating.BaseLine = calcInput.BuildingZones.Sum((BuildingZone zone) => zone.ZoneResults.PowerBudgetTable.Heating.BaseLine);
			buildingBalanceResult.PowerBudgetTable.Heating.BaseLineArea = buildingBalanceResult.PowerBudgetTable.Heating.BaseLine * 1000.0 / num;
			buildingBalanceResult.PowerBudgetTable.Heating.ESM = calcInput.BuildingZones.Sum((BuildingZone zone) => zone.ZoneResults.PowerBudgetTable.Heating.ESM);
			buildingBalanceResult.PowerBudgetTable.Heating.ESMArea = buildingBalanceResult.PowerBudgetTable.Heating.ESM * 1000.0 / num;
		}
	}

	public static void CalculateBuildingSourcePower(CalculationInput calcInput, Results buildingBalanceResult)
	{
		buildingBalanceResult.PowerBudgetTable.Fuel1.ActualArea = calcInput.BuildingZones.Sum((BuildingZone zone) => zone.ZoneResults.PowerBudgetTable.Fuel1.Actual);
		buildingBalanceResult.PowerBudgetTable.Fuel1.BaseLineArea = calcInput.BuildingZones.Sum((BuildingZone zone) => zone.ZoneResults.PowerBudgetTable.Fuel1.BaseLine);
		buildingBalanceResult.PowerBudgetTable.Fuel1.ESMArea = calcInput.BuildingZones.Sum((BuildingZone zone) => zone.ZoneResults.PowerBudgetTable.Fuel1.ESM);
		buildingBalanceResult.PowerBudgetTable.Fuel2.ActualArea = calcInput.BuildingZones.Sum((BuildingZone zone) => zone.ZoneResults.PowerBudgetTable.Fuel2.Actual);
		buildingBalanceResult.PowerBudgetTable.Fuel2.BaseLineArea = calcInput.BuildingZones.Sum((BuildingZone zone) => zone.ZoneResults.PowerBudgetTable.Fuel2.BaseLine);
		buildingBalanceResult.PowerBudgetTable.Fuel2.ESMArea = calcInput.BuildingZones.Sum((BuildingZone zone) => zone.ZoneResults.PowerBudgetTable.Fuel2.ESM);
		buildingBalanceResult.PowerBudgetTable.Fuel3.ActualArea = calcInput.BuildingZones.Sum((BuildingZone zone) => zone.ZoneResults.PowerBudgetTable.Fuel3.Actual);
		buildingBalanceResult.PowerBudgetTable.Fuel3.BaseLineArea = calcInput.BuildingZones.Sum((BuildingZone zone) => zone.ZoneResults.PowerBudgetTable.Fuel3.BaseLine);
		buildingBalanceResult.PowerBudgetTable.Fuel3.ESMArea = calcInput.BuildingZones.Sum((BuildingZone zone) => zone.ZoneResults.PowerBudgetTable.Fuel3.ESM);
		buildingBalanceResult.PowerBudgetTable.Fuel4.ActualArea = calcInput.BuildingZones.Sum((BuildingZone zone) => zone.ZoneResults.PowerBudgetTable.Fuel4.Actual);
		buildingBalanceResult.PowerBudgetTable.Fuel4.BaseLineArea = calcInput.BuildingZones.Sum((BuildingZone zone) => zone.ZoneResults.PowerBudgetTable.Fuel4.BaseLine);
		buildingBalanceResult.PowerBudgetTable.Fuel4.ESMArea = calcInput.BuildingZones.Sum((BuildingZone zone) => zone.ZoneResults.PowerBudgetTable.Fuel4.ESM);
		buildingBalanceResult.PowerBudgetTable.Fuel5.ActualArea = calcInput.BuildingZones.Sum((BuildingZone zone) => zone.ZoneResults.PowerBudgetTable.Fuel5.Actual);
		buildingBalanceResult.PowerBudgetTable.Fuel5.BaseLineArea = calcInput.BuildingZones.Sum((BuildingZone zone) => zone.ZoneResults.PowerBudgetTable.Fuel5.BaseLine);
		buildingBalanceResult.PowerBudgetTable.Fuel5.ESMArea = calcInput.BuildingZones.Sum((BuildingZone zone) => zone.ZoneResults.PowerBudgetTable.Fuel5.ESM);
		buildingBalanceResult.PowerBudgetTable.Fuel6.ActualArea = calcInput.BuildingZones.Sum((BuildingZone zone) => zone.ZoneResults.PowerBudgetTable.Fuel6.Actual);
		buildingBalanceResult.PowerBudgetTable.Fuel6.BaseLineArea = calcInput.BuildingZones.Sum((BuildingZone zone) => zone.ZoneResults.PowerBudgetTable.Fuel6.BaseLine);
		buildingBalanceResult.PowerBudgetTable.Fuel6.ESMArea = calcInput.BuildingZones.Sum((BuildingZone zone) => zone.ZoneResults.PowerBudgetTable.Fuel6.ESM);
		buildingBalanceResult.PowerBudgetTable.Fuel7.ActualArea = calcInput.BuildingZones.Sum((BuildingZone zone) => zone.ZoneResults.PowerBudgetTable.Fuel7.Actual);
		buildingBalanceResult.PowerBudgetTable.Fuel7.BaseLineArea = calcInput.BuildingZones.Sum((BuildingZone zone) => zone.ZoneResults.PowerBudgetTable.Fuel7.BaseLine);
		buildingBalanceResult.PowerBudgetTable.Fuel7.ESMArea = calcInput.BuildingZones.Sum((BuildingZone zone) => zone.ZoneResults.PowerBudgetTable.Fuel7.ESM);
		buildingBalanceResult.PowerBudgetTable.Fuel8.ActualArea = calcInput.BuildingZones.Sum((BuildingZone zone) => zone.ZoneResults.PowerBudgetTable.Fuel8.Actual);
		buildingBalanceResult.PowerBudgetTable.Fuel8.BaseLineArea = calcInput.BuildingZones.Sum((BuildingZone zone) => zone.ZoneResults.PowerBudgetTable.Fuel8.BaseLine);
		buildingBalanceResult.PowerBudgetTable.Fuel8.ESMArea = calcInput.BuildingZones.Sum((BuildingZone zone) => zone.ZoneResults.PowerBudgetTable.Fuel8.ESM);
		buildingBalanceResult.PowerBudgetTable.Fuel9.ActualArea = calcInput.BuildingZones.Sum((BuildingZone zone) => zone.ZoneResults.PowerBudgetTable.Fuel9.Actual);
		buildingBalanceResult.PowerBudgetTable.Fuel9.BaseLineArea = calcInput.BuildingZones.Sum((BuildingZone zone) => zone.ZoneResults.PowerBudgetTable.Fuel9.BaseLine);
		buildingBalanceResult.PowerBudgetTable.Fuel9.ESMArea = calcInput.BuildingZones.Sum((BuildingZone zone) => zone.ZoneResults.PowerBudgetTable.Fuel9.ESM);
		buildingBalanceResult.PowerBudgetTable.Fuel10.ActualArea = calcInput.BuildingZones.Sum((BuildingZone zone) => zone.ZoneResults.PowerBudgetTable.Fuel10.Actual);
		buildingBalanceResult.PowerBudgetTable.Fuel10.BaseLineArea = calcInput.BuildingZones.Sum((BuildingZone zone) => zone.ZoneResults.PowerBudgetTable.Fuel10.BaseLine);
		buildingBalanceResult.PowerBudgetTable.Fuel10.ESMArea = calcInput.BuildingZones.Sum((BuildingZone zone) => zone.ZoneResults.PowerBudgetTable.Fuel10.ESM);
		buildingBalanceResult.PowerBudgetTable.Fuel11.ActualArea = calcInput.BuildingZones.Sum((BuildingZone zone) => zone.ZoneResults.PowerBudgetTable.Fuel11.Actual);
		buildingBalanceResult.PowerBudgetTable.Fuel11.BaseLineArea = calcInput.BuildingZones.Sum((BuildingZone zone) => zone.ZoneResults.PowerBudgetTable.Fuel11.BaseLine);
		buildingBalanceResult.PowerBudgetTable.Fuel11.ESMArea = calcInput.BuildingZones.Sum((BuildingZone zone) => zone.ZoneResults.PowerBudgetTable.Fuel11.ESM);
	}

	public static void CalculateHeatingPower(this CalculationData calcData, Section section, BuildingZone zone, Results results)
	{
		if (!zone.HasHeating)
		{
			results.PowerBudgetTable.Heating.Actual = 0.0;
			results.PowerBudgetTable.Heating.ActualArea = 0.0;
			results.PowerBudgetTable.Heating.BaseLine = 0.0;
			results.PowerBudgetTable.Heating.BaseLineArea = 0.0;
			results.PowerBudgetTable.Heating.ESM = 0.0;
			results.PowerBudgetTable.Heating.ESMArea = 0.0;
			return;
		}
		double num = CalculateParameterHtr(section, results.PowerBudgetTable.Heating.CalculateTemperature, calcData.ProjectTemperatureActual);
		double num2 = CalcParameterHve(section, calcData);
		results.PowerBudgetTable.Heating.Actual = (num + num2) * (calcData.ProjectTemperatureActual - results.PowerBudgetTable.Heating.CalculateTemperature) / 1000.0;
		results.PowerBudgetTable.Heating.ActualArea = results.PowerBudgetTable.Heating.Actual * 1000.0 / section.Area.HeatedArea;
		double num3 = CalculateParameterHtrBaseLine(section, results.PowerBudgetTable.Heating.CalculateTemperature, calcData.ProjectTemperatureActual);
		double num4 = CalcParameterHveBaseLine(section, calcData);
		results.PowerBudgetTable.Heating.BaseLine = (num3 + num4) * (calcData.ProjectTemperatureBaseLine - results.PowerBudgetTable.Heating.CalculateTemperature) / 1000.0;
		results.PowerBudgetTable.Heating.BaseLineArea = results.PowerBudgetTable.Heating.BaseLine * 1000.0 / section.Area.HeatedArea;
		double num5 = CalculateParameterHtrEsm(section, results.PowerBudgetTable.Heating.CalculateTemperature, calcData.ProjectTemperatureActual);
		double num6 = CalcParameterHveEsm(section, calcData);
		results.PowerBudgetTable.Heating.ESM = (num5 + num6) * (calcData.ProjectTemperatureESM - results.PowerBudgetTable.Heating.CalculateTemperature) / 1000.0;
		results.PowerBudgetTable.Heating.ESMArea = results.PowerBudgetTable.Heating.ESM * 1000.0 / section.Area.HeatedArea;
	}

	public static void CalculateEnergySourcePowerFuel1(BuildingZone zone, Results results)
	{
		switch (zone.HeatingCalculations.HeatingResult.Fuel1Actual)
		{
		case Fuel.Fuel1:
			results.PowerBudgetTable.Fuel1.Actual += CalculateFuelValue(zone, results);
			break;
		case Fuel.Fuel2:
			results.PowerBudgetTable.Fuel2.Actual += CalculateFuelValue(zone, results);
			break;
		case Fuel.Fuel3:
			results.PowerBudgetTable.Fuel3.Actual += CalculateFuelValue(zone, results);
			break;
		case Fuel.Fuel4:
			results.PowerBudgetTable.Fuel4.Actual += CalculateFuelValue(zone, results);
			break;
		case Fuel.Fuel5:
			results.PowerBudgetTable.Fuel5.Actual += CalculateFuelValue(zone, results);
			break;
		case Fuel.Fuel6:
			results.PowerBudgetTable.Fuel6.Actual += CalculateFuelValue(zone, results);
			break;
		case Fuel.Fuel7:
			results.PowerBudgetTable.Fuel7.Actual += CalculateFuelValue(zone, results);
			break;
		case Fuel.Fuel8:
			results.PowerBudgetTable.Fuel8.Actual += CalculateFuelValue(zone, results);
			break;
		case Fuel.Fuel9:
			results.PowerBudgetTable.Fuel9.Actual += CalculateFuelValue(zone, results);
			break;
		case Fuel.Fuel10:
			results.PowerBudgetTable.Fuel10.Actual += CalculateFuelValue(zone, results);
			break;
		case Fuel.Fuel11:
			results.PowerBudgetTable.Fuel11.Actual += CalculateFuelValue(zone, results);
			break;
		}
	}

	public static void CalculateEnergySourcePowerFuel1Area(Section section, BuildingZone zone, Results results)
	{
		switch (zone.HeatingCalculations.HeatingResult.Fuel1Actual)
		{
		case Fuel.Fuel1:
			results.PowerBudgetTable.Fuel1.ActualArea += CalculateFuelAreaValue(zone, results, section);
			break;
		case Fuel.Fuel2:
			results.PowerBudgetTable.Fuel2.ActualArea += CalculateFuelAreaValue(zone, results, section);
			break;
		case Fuel.Fuel3:
			results.PowerBudgetTable.Fuel3.ActualArea += CalculateFuelAreaValue(zone, results, section);
			break;
		case Fuel.Fuel4:
			results.PowerBudgetTable.Fuel4.ActualArea += CalculateFuelAreaValue(zone, results, section);
			break;
		case Fuel.Fuel5:
			results.PowerBudgetTable.Fuel5.ActualArea += CalculateFuelAreaValue(zone, results, section);
			break;
		case Fuel.Fuel6:
			results.PowerBudgetTable.Fuel6.ActualArea += CalculateFuelAreaValue(zone, results, section);
			break;
		case Fuel.Fuel7:
			results.PowerBudgetTable.Fuel7.ActualArea += CalculateFuelAreaValue(zone, results, section);
			break;
		case Fuel.Fuel8:
			results.PowerBudgetTable.Fuel8.ActualArea += CalculateFuelAreaValue(zone, results, section);
			break;
		case Fuel.Fuel9:
			results.PowerBudgetTable.Fuel9.ActualArea += CalculateFuelAreaValue(zone, results, section);
			break;
		case Fuel.Fuel10:
			results.PowerBudgetTable.Fuel10.ActualArea += CalculateFuelAreaValue(zone, results, section);
			break;
		case Fuel.Fuel11:
			results.PowerBudgetTable.Fuel11.ActualArea += CalculateFuelAreaValue(zone, results, section);
			break;
		}
	}

	public static void CalculateEnergySourcePowerFuel2(BuildingZone zone, Results results)
	{
		switch (zone.HeatingCalculations.HeatingResult.Fuel2Actual)
		{
		case Fuel.Fuel1:
			results.PowerBudgetTable.Fuel1.Actual += CalculateFuel2Value(zone, results);
			break;
		case Fuel.Fuel2:
			results.PowerBudgetTable.Fuel2.Actual += CalculateFuel2Value(zone, results);
			break;
		case Fuel.Fuel3:
			results.PowerBudgetTable.Fuel3.Actual += CalculateFuel2Value(zone, results);
			break;
		case Fuel.Fuel4:
			results.PowerBudgetTable.Fuel4.Actual += CalculateFuel2Value(zone, results);
			break;
		case Fuel.Fuel5:
			results.PowerBudgetTable.Fuel5.Actual += CalculateFuel2Value(zone, results);
			break;
		case Fuel.Fuel6:
			results.PowerBudgetTable.Fuel6.Actual += CalculateFuel2Value(zone, results);
			break;
		case Fuel.Fuel7:
			results.PowerBudgetTable.Fuel7.Actual += CalculateFuel2Value(zone, results);
			break;
		case Fuel.Fuel8:
			results.PowerBudgetTable.Fuel8.Actual += CalculateFuel2Value(zone, results);
			break;
		case Fuel.Fuel9:
			results.PowerBudgetTable.Fuel9.Actual += CalculateFuel2Value(zone, results);
			break;
		case Fuel.Fuel10:
			results.PowerBudgetTable.Fuel10.Actual += CalculateFuel2Value(zone, results);
			break;
		case Fuel.Fuel11:
			results.PowerBudgetTable.Fuel11.Actual += CalculateFuel2Value(zone, results);
			break;
		}
	}

	public static void CalculateEnergySourcePowerFuel2Area(Section section, BuildingZone zone, Results results)
	{
		switch (zone.HeatingCalculations.HeatingResult.Fuel2Actual)
		{
		case Fuel.Fuel1:
			results.PowerBudgetTable.Fuel1.ActualArea += CalculateFuel2AreaValue(zone, results, section);
			break;
		case Fuel.Fuel2:
			results.PowerBudgetTable.Fuel2.ActualArea += CalculateFuel2AreaValue(zone, results, section);
			break;
		case Fuel.Fuel3:
			results.PowerBudgetTable.Fuel3.ActualArea += CalculateFuel2AreaValue(zone, results, section);
			break;
		case Fuel.Fuel4:
			results.PowerBudgetTable.Fuel4.ActualArea += CalculateFuel2AreaValue(zone, results, section);
			break;
		case Fuel.Fuel5:
			results.PowerBudgetTable.Fuel5.ActualArea += CalculateFuel2AreaValue(zone, results, section);
			break;
		case Fuel.Fuel6:
			results.PowerBudgetTable.Fuel6.ActualArea += CalculateFuel2AreaValue(zone, results, section);
			break;
		case Fuel.Fuel7:
			results.PowerBudgetTable.Fuel7.ActualArea += CalculateFuel2AreaValue(zone, results, section);
			break;
		case Fuel.Fuel8:
			results.PowerBudgetTable.Fuel8.ActualArea += CalculateFuel2AreaValue(zone, results, section);
			break;
		case Fuel.Fuel9:
			results.PowerBudgetTable.Fuel9.ActualArea += CalculateFuel2AreaValue(zone, results, section);
			break;
		case Fuel.Fuel10:
			results.PowerBudgetTable.Fuel10.ActualArea += CalculateFuel2AreaValue(zone, results, section);
			break;
		case Fuel.Fuel11:
			results.PowerBudgetTable.Fuel11.ActualArea += CalculateFuel2AreaValue(zone, results, section);
			break;
		}
	}

	public static void CalculateEnergySourcePowerFuel1BaseLine(Section section, BuildingZone zone, Results results)
	{
		switch (zone.HeatingCalculations.HeatingResult.Fuel1BaseLine)
		{
		case Fuel.Fuel1:
			results.PowerBudgetTable.Fuel1.BaseLine += CalculateFuelValueBaseLine(zone, results);
			break;
		case Fuel.Fuel2:
			results.PowerBudgetTable.Fuel2.BaseLine += CalculateFuelValueBaseLine(zone, results);
			break;
		case Fuel.Fuel3:
			results.PowerBudgetTable.Fuel3.BaseLine += CalculateFuelValueBaseLine(zone, results);
			break;
		case Fuel.Fuel4:
			results.PowerBudgetTable.Fuel4.BaseLine += CalculateFuelValueBaseLine(zone, results);
			break;
		case Fuel.Fuel5:
			results.PowerBudgetTable.Fuel5.BaseLine += CalculateFuelValueBaseLine(zone, results);
			break;
		case Fuel.Fuel6:
			results.PowerBudgetTable.Fuel6.BaseLine += CalculateFuelValueBaseLine(zone, results);
			break;
		case Fuel.Fuel7:
			results.PowerBudgetTable.Fuel7.BaseLine += CalculateFuelValueBaseLine(zone, results);
			break;
		case Fuel.Fuel8:
			results.PowerBudgetTable.Fuel8.BaseLine += CalculateFuelValueBaseLine(zone, results);
			break;
		case Fuel.Fuel9:
			results.PowerBudgetTable.Fuel9.BaseLine += CalculateFuelValueBaseLine(zone, results);
			break;
		case Fuel.Fuel10:
			results.PowerBudgetTable.Fuel10.BaseLine += CalculateFuelValueBaseLine(zone, results);
			break;
		case Fuel.Fuel11:
			results.PowerBudgetTable.Fuel11.BaseLine += CalculateFuelValueBaseLine(zone, results);
			break;
		}
	}

	public static void CalculateEnergySourcePowerFuel1BaseLineArea(Section section, BuildingZone zone, Results results)
	{
		switch (zone.HeatingCalculations.HeatingResult.Fuel1BaseLine)
		{
		case Fuel.Fuel1:
			results.PowerBudgetTable.Fuel1.BaseLineArea += CalculateFuelAreaValueBaseLine(zone, results, section);
			break;
		case Fuel.Fuel2:
			results.PowerBudgetTable.Fuel2.BaseLineArea += CalculateFuelAreaValueBaseLine(zone, results, section);
			break;
		case Fuel.Fuel3:
			results.PowerBudgetTable.Fuel3.BaseLineArea += CalculateFuelAreaValueBaseLine(zone, results, section);
			break;
		case Fuel.Fuel4:
			results.PowerBudgetTable.Fuel4.BaseLineArea += CalculateFuelAreaValueBaseLine(zone, results, section);
			break;
		case Fuel.Fuel5:
			results.PowerBudgetTable.Fuel5.BaseLineArea += CalculateFuelAreaValueBaseLine(zone, results, section);
			break;
		case Fuel.Fuel6:
			results.PowerBudgetTable.Fuel6.BaseLineArea += CalculateFuelAreaValueBaseLine(zone, results, section);
			break;
		case Fuel.Fuel7:
			results.PowerBudgetTable.Fuel7.BaseLineArea += CalculateFuelAreaValueBaseLine(zone, results, section);
			break;
		case Fuel.Fuel8:
			results.PowerBudgetTable.Fuel8.BaseLineArea += CalculateFuelAreaValueBaseLine(zone, results, section);
			break;
		case Fuel.Fuel9:
			results.PowerBudgetTable.Fuel9.BaseLineArea += CalculateFuelAreaValueBaseLine(zone, results, section);
			break;
		case Fuel.Fuel10:
			results.PowerBudgetTable.Fuel10.BaseLineArea += CalculateFuelAreaValueBaseLine(zone, results, section);
			break;
		case Fuel.Fuel11:
			results.PowerBudgetTable.Fuel11.BaseLineArea += CalculateFuelAreaValueBaseLine(zone, results, section);
			break;
		}
	}

	public static void CalculateEnergySourcePowerFuel2BaseLine(Section section, BuildingZone zone, Results results)
	{
		switch (zone.HeatingCalculations.HeatingResult.Fuel2BaseLine)
		{
		case Fuel.Fuel1:
			results.PowerBudgetTable.Fuel1.BaseLine += CalculateFuel2ValueBaseLine(zone, results);
			break;
		case Fuel.Fuel2:
			results.PowerBudgetTable.Fuel2.BaseLine += CalculateFuel2ValueBaseLine(zone, results);
			break;
		case Fuel.Fuel3:
			results.PowerBudgetTable.Fuel3.BaseLine += CalculateFuel2ValueBaseLine(zone, results);
			break;
		case Fuel.Fuel4:
			results.PowerBudgetTable.Fuel4.BaseLine += CalculateFuel2ValueBaseLine(zone, results);
			break;
		case Fuel.Fuel5:
			results.PowerBudgetTable.Fuel5.BaseLine += CalculateFuel2ValueBaseLine(zone, results);
			break;
		case Fuel.Fuel6:
			results.PowerBudgetTable.Fuel6.BaseLine += CalculateFuel2ValueBaseLine(zone, results);
			break;
		case Fuel.Fuel7:
			results.PowerBudgetTable.Fuel7.BaseLine += CalculateFuel2ValueBaseLine(zone, results);
			break;
		case Fuel.Fuel8:
			results.PowerBudgetTable.Fuel8.BaseLine += CalculateFuel2ValueBaseLine(zone, results);
			break;
		case Fuel.Fuel9:
			results.PowerBudgetTable.Fuel9.BaseLine += CalculateFuel2ValueBaseLine(zone, results);
			break;
		case Fuel.Fuel10:
			results.PowerBudgetTable.Fuel10.BaseLine += CalculateFuel2ValueBaseLine(zone, results);
			break;
		case Fuel.Fuel11:
			results.PowerBudgetTable.Fuel11.BaseLine += CalculateFuel2ValueBaseLine(zone, results);
			break;
		}
	}

	public static void CalculateEnergySourcePowerFuel2BaseLineArea(Section section, BuildingZone zone, Results results)
	{
		switch (zone.HeatingCalculations.HeatingResult.Fuel2BaseLine)
		{
		case Fuel.Fuel1:
			results.PowerBudgetTable.Fuel1.BaseLineArea += CalculateFuel2AreaValueBaseLine(zone, results, section);
			break;
		case Fuel.Fuel2:
			results.PowerBudgetTable.Fuel2.BaseLineArea += CalculateFuel2AreaValueBaseLine(zone, results, section);
			break;
		case Fuel.Fuel3:
			results.PowerBudgetTable.Fuel3.BaseLineArea += CalculateFuel2AreaValueBaseLine(zone, results, section);
			break;
		case Fuel.Fuel4:
			results.PowerBudgetTable.Fuel4.BaseLineArea += CalculateFuel2AreaValueBaseLine(zone, results, section);
			break;
		case Fuel.Fuel5:
			results.PowerBudgetTable.Fuel5.BaseLineArea += CalculateFuel2AreaValueBaseLine(zone, results, section);
			break;
		case Fuel.Fuel6:
			results.PowerBudgetTable.Fuel6.BaseLineArea += CalculateFuel2AreaValueBaseLine(zone, results, section);
			break;
		case Fuel.Fuel7:
			results.PowerBudgetTable.Fuel7.BaseLineArea += CalculateFuel2AreaValueBaseLine(zone, results, section);
			break;
		case Fuel.Fuel8:
			results.PowerBudgetTable.Fuel8.BaseLineArea += CalculateFuel2AreaValueBaseLine(zone, results, section);
			break;
		case Fuel.Fuel9:
			results.PowerBudgetTable.Fuel9.BaseLineArea += CalculateFuel2AreaValueBaseLine(zone, results, section);
			break;
		case Fuel.Fuel10:
			results.PowerBudgetTable.Fuel10.BaseLineArea += CalculateFuel2AreaValueBaseLine(zone, results, section);
			break;
		case Fuel.Fuel11:
			results.PowerBudgetTable.Fuel11.BaseLineArea += CalculateFuel2AreaValueBaseLine(zone, results, section);
			break;
		}
	}

	public static void CalculateEnergySourcePowerFuel1Esm(Section section, BuildingZone zone, Results results)
	{
		switch (zone.HeatingCalculations.HeatingResult.Fuel1ESM)
		{
		case Fuel.Fuel1:
			results.PowerBudgetTable.Fuel1.ESM += CalculateFuelValueEsm(zone, results);
			break;
		case Fuel.Fuel2:
			results.PowerBudgetTable.Fuel2.ESM += CalculateFuelValueEsm(zone, results);
			break;
		case Fuel.Fuel3:
			results.PowerBudgetTable.Fuel3.ESM += CalculateFuelValueEsm(zone, results);
			break;
		case Fuel.Fuel4:
			results.PowerBudgetTable.Fuel4.ESM += CalculateFuelValueEsm(zone, results);
			break;
		case Fuel.Fuel5:
			results.PowerBudgetTable.Fuel5.ESM += CalculateFuelValueEsm(zone, results);
			break;
		case Fuel.Fuel6:
			results.PowerBudgetTable.Fuel6.ESM += CalculateFuelValueEsm(zone, results);
			break;
		case Fuel.Fuel7:
			results.PowerBudgetTable.Fuel7.ESM += CalculateFuelValueEsm(zone, results);
			break;
		case Fuel.Fuel8:
			results.PowerBudgetTable.Fuel8.ESM += CalculateFuelValueEsm(zone, results);
			break;
		case Fuel.Fuel9:
			results.PowerBudgetTable.Fuel9.ESM += CalculateFuelValueEsm(zone, results);
			break;
		case Fuel.Fuel10:
			results.PowerBudgetTable.Fuel10.ESM += CalculateFuelValueEsm(zone, results);
			break;
		case Fuel.Fuel11:
			results.PowerBudgetTable.Fuel11.ESM += CalculateFuelValueEsm(zone, results);
			break;
		}
	}

	public static void CalculateEnergySourcePowerFuel1EsmArea(Section section, BuildingZone zone, Results results)
	{
		switch (zone.HeatingCalculations.HeatingResult.Fuel1ESM)
		{
		case Fuel.Fuel1:
			results.PowerBudgetTable.Fuel1.ESMArea += CalculateFuelAreaValueEsm(zone, results, section);
			break;
		case Fuel.Fuel2:
			results.PowerBudgetTable.Fuel2.ESMArea += CalculateFuelAreaValueEsm(zone, results, section);
			break;
		case Fuel.Fuel3:
			results.PowerBudgetTable.Fuel3.ESMArea += CalculateFuelAreaValueEsm(zone, results, section);
			break;
		case Fuel.Fuel4:
			results.PowerBudgetTable.Fuel4.ESMArea += CalculateFuelAreaValueEsm(zone, results, section);
			break;
		case Fuel.Fuel5:
			results.PowerBudgetTable.Fuel5.ESMArea += CalculateFuelAreaValueEsm(zone, results, section);
			break;
		case Fuel.Fuel6:
			results.PowerBudgetTable.Fuel6.ESMArea += CalculateFuelAreaValueEsm(zone, results, section);
			break;
		case Fuel.Fuel7:
			results.PowerBudgetTable.Fuel7.ESMArea += CalculateFuelAreaValueEsm(zone, results, section);
			break;
		case Fuel.Fuel8:
			results.PowerBudgetTable.Fuel8.ESMArea += CalculateFuelAreaValueEsm(zone, results, section);
			break;
		case Fuel.Fuel9:
			results.PowerBudgetTable.Fuel9.ESMArea += CalculateFuelAreaValueEsm(zone, results, section);
			break;
		case Fuel.Fuel10:
			results.PowerBudgetTable.Fuel10.ESMArea += CalculateFuelAreaValueEsm(zone, results, section);
			break;
		case Fuel.Fuel11:
			results.PowerBudgetTable.Fuel11.ESMArea += CalculateFuelAreaValueEsm(zone, results, section);
			break;
		}
	}

	public static void CalculateEnergySourcePowerFuel2Esm(Section section, BuildingZone zone, Results results)
	{
		switch (zone.HeatingCalculations.HeatingResult.Fuel2ESM)
		{
		case Fuel.Fuel1:
			results.PowerBudgetTable.Fuel1.ESM += CalculateFuel2ValueEsm(zone, results);
			break;
		case Fuel.Fuel2:
			results.PowerBudgetTable.Fuel2.ESM += CalculateFuel2ValueEsm(zone, results);
			break;
		case Fuel.Fuel3:
			results.PowerBudgetTable.Fuel3.ESM += CalculateFuel2ValueEsm(zone, results);
			break;
		case Fuel.Fuel4:
			results.PowerBudgetTable.Fuel4.ESM += CalculateFuel2ValueEsm(zone, results);
			break;
		case Fuel.Fuel5:
			results.PowerBudgetTable.Fuel5.ESM += CalculateFuel2ValueEsm(zone, results);
			break;
		case Fuel.Fuel6:
			results.PowerBudgetTable.Fuel6.ESM += CalculateFuel2ValueEsm(zone, results);
			break;
		case Fuel.Fuel7:
			results.PowerBudgetTable.Fuel7.ESM += CalculateFuel2ValueEsm(zone, results);
			break;
		case Fuel.Fuel8:
			results.PowerBudgetTable.Fuel8.ESM += CalculateFuel2ValueEsm(zone, results);
			break;
		case Fuel.Fuel9:
			results.PowerBudgetTable.Fuel9.ESM += CalculateFuel2ValueEsm(zone, results);
			break;
		case Fuel.Fuel10:
			results.PowerBudgetTable.Fuel10.ESM += CalculateFuel2ValueEsm(zone, results);
			break;
		case Fuel.Fuel11:
			results.PowerBudgetTable.Fuel11.ESM += CalculateFuel2ValueEsm(zone, results);
			break;
		}
	}

	public static void CalculateEnergySourcePowerFuel2EsmArea(Section section, BuildingZone zone, Results results)
	{
		switch (zone.HeatingCalculations.HeatingResult.Fuel2ESM)
		{
		case Fuel.Fuel1:
			results.PowerBudgetTable.Fuel1.ESMArea += CalculateFuel2AreaValueEsm(zone, results, section);
			break;
		case Fuel.Fuel2:
			results.PowerBudgetTable.Fuel2.ESMArea += CalculateFuel2AreaValueEsm(zone, results, section);
			break;
		case Fuel.Fuel3:
			results.PowerBudgetTable.Fuel3.ESMArea += CalculateFuel2AreaValueEsm(zone, results, section);
			break;
		case Fuel.Fuel4:
			results.PowerBudgetTable.Fuel4.ESMArea += CalculateFuel2AreaValueEsm(zone, results, section);
			break;
		case Fuel.Fuel5:
			results.PowerBudgetTable.Fuel5.ESMArea += CalculateFuel2AreaValueEsm(zone, results, section);
			break;
		case Fuel.Fuel6:
			results.PowerBudgetTable.Fuel6.ESMArea += CalculateFuel2AreaValueEsm(zone, results, section);
			break;
		case Fuel.Fuel7:
			results.PowerBudgetTable.Fuel7.ESMArea += CalculateFuel2AreaValueEsm(zone, results, section);
			break;
		case Fuel.Fuel8:
			results.PowerBudgetTable.Fuel8.ESMArea += CalculateFuel2AreaValueEsm(zone, results, section);
			break;
		case Fuel.Fuel9:
			results.PowerBudgetTable.Fuel9.ESMArea += CalculateFuel2AreaValueEsm(zone, results, section);
			break;
		case Fuel.Fuel10:
			results.PowerBudgetTable.Fuel10.ESMArea += CalculateFuel2AreaValueEsm(zone, results, section);
			break;
		case Fuel.Fuel11:
			results.PowerBudgetTable.Fuel11.ESMArea += CalculateFuel2AreaValueEsm(zone, results, section);
			break;
		}
	}

	public static void ClearFuelCellsPowerTable(Results zoneBalanceResult)
	{
		zoneBalanceResult.PowerBudgetTable.Fuel1.Actual = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel2.Actual = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel3.Actual = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel4.Actual = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel5.Actual = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel6.Actual = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel7.Actual = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel8.Actual = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel9.Actual = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel10.Actual = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel11.Actual = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel1.BaseLine = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel2.BaseLine = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel3.BaseLine = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel4.BaseLine = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel5.BaseLine = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel6.BaseLine = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel7.BaseLine = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel8.BaseLine = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel9.BaseLine = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel10.BaseLine = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel11.BaseLine = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel1.ESM = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel2.ESM = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel3.ESM = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel4.ESM = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel5.ESM = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel6.ESM = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel7.ESM = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel8.ESM = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel9.ESM = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel10.ESM = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel11.ESM = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel1.ActualArea = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel2.ActualArea = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel3.ActualArea = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel4.ActualArea = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel5.ActualArea = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel6.ActualArea = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel7.ActualArea = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel8.ActualArea = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel9.ActualArea = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel10.ActualArea = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel11.ActualArea = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel1.BaseLineArea = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel2.BaseLineArea = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel3.BaseLineArea = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel4.BaseLineArea = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel5.BaseLineArea = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel6.BaseLineArea = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel7.BaseLineArea = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel8.BaseLineArea = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel9.BaseLineArea = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel10.BaseLineArea = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel11.BaseLineArea = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel1.ESMArea = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel2.ESMArea = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel3.ESMArea = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel4.ESMArea = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel5.ESMArea = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel6.ESMArea = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel7.ESMArea = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel8.ESMArea = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel9.ESMArea = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel10.ESMArea = 0.0;
		zoneBalanceResult.PowerBudgetTable.Fuel11.ESMArea = 0.0;
	}

	public static void ClearFuelCellsPowerTableBuilding(this CalculationData calcData, Results buildingBalanceResult)
	{
		buildingBalanceResult.PowerBudgetTable.Fuel1.Actual = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel2.Actual = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel3.Actual = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel4.Actual = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel5.Actual = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel6.Actual = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel7.Actual = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel8.Actual = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel9.Actual = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel10.Actual = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel11.Actual = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel1.BaseLine = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel2.BaseLine = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel3.BaseLine = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel4.BaseLine = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel5.BaseLine = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel6.BaseLine = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel7.BaseLine = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel8.BaseLine = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel9.BaseLine = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel10.BaseLine = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel11.BaseLine = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel1.ESM = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel2.ESM = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel3.ESM = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel4.ESM = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel5.ESM = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel6.ESM = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel7.ESM = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel8.ESM = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel9.ESM = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel10.ESM = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel11.ESM = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel1.ActualArea = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel2.ActualArea = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel3.ActualArea = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel4.ActualArea = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel5.ActualArea = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel6.ActualArea = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel7.ActualArea = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel8.ActualArea = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel9.ActualArea = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel10.ActualArea = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel11.ActualArea = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel1.BaseLineArea = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel2.BaseLineArea = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel3.BaseLineArea = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel4.BaseLineArea = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel5.BaseLineArea = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel6.BaseLineArea = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel7.BaseLineArea = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel8.BaseLineArea = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel9.BaseLineArea = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel10.BaseLineArea = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel11.BaseLineArea = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel1.ESMArea = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel2.ESMArea = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel3.ESMArea = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel4.ESMArea = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel5.ESMArea = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel6.ESMArea = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel7.ESMArea = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel8.ESMArea = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel9.ESMArea = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel10.ESMArea = 0.0;
		buildingBalanceResult.PowerBudgetTable.Fuel11.ESMArea = 0.0;
	}

	private static double CalculateFuelValue(BuildingZone zone, Results results)
	{
		double num = results.PowerBudgetTable.Heating.Actual * zone.HeatingCalculations.HeatingResult.Part1Actual / 100.0 / (zone.HeatingCalculations.HeatingResult.TransmitTempEfficiencyActual / 100.0 * (zone.HeatingCalculations.HeatingResult.SupplyNetEfficiencyActual / 100.0) * (zone.HeatingCalculations.HeatingResult.AutomaticActual / 100.0) * (zone.HeatingCalculations.HeatingResult.EnergyManagementActual / 100.0) * (zone.HeatingCalculations.HeatingResult.GeneratorHeatEfficiency1Actual / 100.0));
		return (double.IsNaN(num) || double.IsInfinity(num)) ? 0.0 : num;
	}

	private static double CalculateFuelValueBaseLine(BuildingZone zone, Results results)
	{
		double num = results.PowerBudgetTable.Heating.BaseLine * zone.HeatingCalculations.HeatingResult.Part1BaseLine / 100.0 / (zone.HeatingCalculations.HeatingResult.TransmitTempEfficiencyBaseLine / 100.0 * (zone.HeatingCalculations.HeatingResult.SupplyNetEfficiencyBaseLine / 100.0) * (zone.HeatingCalculations.HeatingResult.AutomaticBaseLine / 100.0) * (zone.HeatingCalculations.HeatingResult.EnergyManagementBaseLine / 100.0) * (zone.HeatingCalculations.HeatingResult.GeneratorHeatEfficiency1BaseLine / 100.0));
		return (double.IsNaN(num) || double.IsInfinity(num)) ? 0.0 : num;
	}

	private static double CalculateFuelValueEsm(BuildingZone zone, Results results)
	{
		double num = results.PowerBudgetTable.Heating.ESM * zone.HeatingCalculations.HeatingResult.Part1ESM / 100.0 / (zone.HeatingCalculations.HeatingResult.TransmitTempEfficiencyESM / 100.0 * (zone.HeatingCalculations.HeatingResult.SupplyNetEfficiencyESM / 100.0) * (zone.HeatingCalculations.HeatingResult.AutomaticESM / 100.0) * (zone.HeatingCalculations.HeatingResult.EnergyManagementESM / 100.0) * (zone.HeatingCalculations.HeatingResult.GeneratorHeatEfficiency1ESM / 100.0));
		return (double.IsNaN(num) || double.IsInfinity(num)) ? 0.0 : num;
	}

	private static double CalculateFuel2Value(BuildingZone zone, Results results)
	{
		double num = results.PowerBudgetTable.Heating.Actual * zone.HeatingCalculations.HeatingResult.Part2Actual / 100.0 / (zone.HeatingCalculations.HeatingResult.TransmitTempEfficiency2Actual / 100.0 * (zone.HeatingCalculations.HeatingResult.SupplyNetEfficiency2Actual / 100.0) * (zone.HeatingCalculations.HeatingResult.Automatic2Actual / 100.0) * (zone.HeatingCalculations.HeatingResult.EnergyManagement2Actual / 100.0) * (zone.HeatingCalculations.HeatingResult.GeneratorHeatEfficiency2Actual / 100.0));
		return (double.IsNaN(num) || double.IsInfinity(num)) ? 0.0 : num;
	}

	private static double CalculateFuel2ValueBaseLine(BuildingZone zone, Results results)
	{
		double num = results.PowerBudgetTable.Heating.BaseLine * zone.HeatingCalculations.HeatingResult.Part2BaseLine / 100.0 / (zone.HeatingCalculations.HeatingResult.TransmitTempEfficiency2BaseLine / 100.0 * (zone.HeatingCalculations.HeatingResult.SupplyNetEfficiency2BaseLine / 100.0) * (zone.HeatingCalculations.HeatingResult.Automatic2BaseLine / 100.0) * (zone.HeatingCalculations.HeatingResult.EnergyManagement2BaseLine / 100.0) * (zone.HeatingCalculations.HeatingResult.GeneratorHeatEfficiency2BaseLine / 100.0));
		return (double.IsNaN(num) || double.IsInfinity(num)) ? 0.0 : num;
	}

	private static double CalculateFuel2ValueEsm(BuildingZone zone, Results results)
	{
		double num = results.PowerBudgetTable.Heating.ESM * zone.HeatingCalculations.HeatingResult.Part2ESM / 100.0 / (zone.HeatingCalculations.HeatingResult.TransmitTempEfficiency2ESM / 100.0 * (zone.HeatingCalculations.HeatingResult.SupplyNetEfficiency2ESM / 100.0) * (zone.HeatingCalculations.HeatingResult.Automatic2ESM / 100.0) * (zone.HeatingCalculations.HeatingResult.EnergyManagement2ESM / 100.0) * (zone.HeatingCalculations.HeatingResult.GeneratorHeatEfficiency2ESM / 100.0));
		return (double.IsNaN(num) || double.IsInfinity(num)) ? 0.0 : num;
	}

	private static double CalculateFuelAreaValue(BuildingZone zone, Results results, Section section)
	{
		return CalculateFuelValue(zone, results) * 1000.0 / section.Area.HeatedArea;
	}

	private static double CalculateFuelAreaValueBaseLine(BuildingZone zone, Results results, Section section)
	{
		return CalculateFuelValueBaseLine(zone, results) * 1000.0 / section.Area.HeatedArea;
	}

	private static double CalculateFuelAreaValueEsm(BuildingZone zone, Results results, Section section)
	{
		return CalculateFuelValueEsm(zone, results) * 1000.0 / section.Area.HeatedArea;
	}

	private static double CalculateFuel2AreaValue(BuildingZone zone, Results results, Section section)
	{
		return CalculateFuel2Value(zone, results) * 1000.0 / section.Area.HeatedArea;
	}

	private static double CalculateFuel2AreaValueBaseLine(BuildingZone zone, Results results, Section section)
	{
		return CalculateFuel2ValueBaseLine(zone, results) * 1000.0 / section.Area.HeatedArea;
	}

	private static double CalculateFuel2AreaValueEsm(BuildingZone zone, Results results, Section section)
	{
		return CalculateFuel2ValueEsm(zone, results) * 1000.0 / section.Area.HeatedArea;
	}

	private static void ClearPrimaryEnergy(Results zoneBalanceResult)
	{
		zoneBalanceResult.PrimaryEnergyTable.Heating.Ref1 = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.Heating.Ref2 = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.Heating.Actual = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.Heating.BaseLine = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.Heating.ESM = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.Cooling.Ref1 = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.Cooling.Ref2 = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.Cooling.Actual = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.Cooling.BaseLine = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.Cooling.ESM = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.HeatingVentilation.Ref1 = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.HeatingVentilation.Ref2 = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.HeatingVentilation.Actual = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.HeatingVentilation.BaseLine = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.HeatingVentilation.ESM = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.CoolingVentilation.Ref1 = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.CoolingVentilation.Ref2 = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.CoolingVentilation.Actual = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.CoolingVentilation.BaseLine = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.CoolingVentilation.ESM = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.FansAndPumps.Ref1 = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.FansAndPumps.Ref2 = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.FansAndPumps.Actual = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.FansAndPumps.BaseLine = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.FansAndPumps.ESM = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.BGV.Ref1 = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.BGV.Ref2 = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.BGV.Actual = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.BGV.BaseLine = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.BGV.ESM = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.BGVPumps.Ref1 = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.BGVPumps.Ref2 = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.BGVPumps.Actual = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.BGVPumps.BaseLine = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.BGVPumps.ESM = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.Lights.Ref1 = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.Lights.Ref2 = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.Lights.Actual = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.Lights.BaseLine = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.Lights.ESM = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.HeatAffectingDevices.Ref1 = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.HeatAffectingDevices.Ref2 = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.HeatAffectingDevices.Actual = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.HeatAffectingDevices.BaseLine = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.HeatAffectingDevices.ESM = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.NonHeatAffectingDevices.Ref1 = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.NonHeatAffectingDevices.Ref2 = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.NonHeatAffectingDevices.Actual = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.NonHeatAffectingDevices.BaseLine = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.NonHeatAffectingDevices.ESM = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.Other.Ref1 = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.Other.Ref2 = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.Other.Actual = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.Other.BaseLine = 0.0;
		zoneBalanceResult.PrimaryEnergyTable.Other.ESM = 0.0;
	}

	private static void CalculatePrimaryEnergyByTechnologies(Results zoneBalanceResult, BuildingZone zone, bool isBGVused, double totalArea = 1.0, bool isFirstBuildingZone = true)
	{
		double heatedArea = zone.Heating.Area.HeatedArea;
		double num;
		double num2;
		double num3;
		double num4;
		double num5;
		if (zone.HasHeating)
		{
			double primaryEnergyCoeficient = GetPrimaryEnergyCoeficient(zone.HeatingCalculations.HeatingResult.Fuel1Ref1, zone.HeatingCalculations.HeatingResult.ResultSourceEnergyRef1);
			double primaryEnergyCoeficient2 = GetPrimaryEnergyCoeficient(zone.HeatingCalculations.HeatingResult.Fuel2Ref1, zone.HeatingCalculations.HeatingResult.ResultSourceEnergy2Ref1);
			num = primaryEnergyCoeficient * heatedArea + primaryEnergyCoeficient2 * heatedArea;
			zoneBalanceResult.PrimaryEnergyTable.Heating.Ref1 += num;
			primaryEnergyCoeficient = GetPrimaryEnergyCoeficient(zone.HeatingCalculations.HeatingResult.Fuel1Ref2, zone.HeatingCalculations.HeatingResult.ResultSourceEnergyRef2);
			primaryEnergyCoeficient2 = GetPrimaryEnergyCoeficient(zone.HeatingCalculations.HeatingResult.Fuel2Ref2, zone.HeatingCalculations.HeatingResult.ResultSourceEnergy2Ref2);
			num2 = primaryEnergyCoeficient * heatedArea + primaryEnergyCoeficient2 * heatedArea;
			zoneBalanceResult.PrimaryEnergyTable.Heating.Ref2 += num2;
			primaryEnergyCoeficient = GetPrimaryEnergyCoeficient(zone.HeatingCalculations.HeatingResult.Fuel1Actual, zone.HeatingCalculations.HeatingResult.ResultSourceEnergyActual);
			primaryEnergyCoeficient2 = GetPrimaryEnergyCoeficient(zone.HeatingCalculations.HeatingResult.Fuel2Actual, zone.HeatingCalculations.HeatingResult.ResultSourceEnergy2Actual);
			num3 = primaryEnergyCoeficient * heatedArea + primaryEnergyCoeficient2 * heatedArea;
			zoneBalanceResult.PrimaryEnergyTable.Heating.Actual += num3;
			primaryEnergyCoeficient = GetPrimaryEnergyCoeficient(zone.HeatingCalculations.HeatingResult.Fuel1BaseLine, zone.HeatingCalculations.HeatingResult.ResultSourceEnergyBaseLine);
			primaryEnergyCoeficient2 = GetPrimaryEnergyCoeficient(zone.HeatingCalculations.HeatingResult.Fuel2BaseLine, zone.HeatingCalculations.HeatingResult.ResultSourceEnergy2BaseLine);
			num4 = primaryEnergyCoeficient * heatedArea + primaryEnergyCoeficient2 * heatedArea;
			zoneBalanceResult.PrimaryEnergyTable.Heating.BaseLine += num4;
			primaryEnergyCoeficient = GetPrimaryEnergyCoeficient(zone.HeatingCalculations.HeatingResult.Fuel1ESM, zone.HeatingCalculations.HeatingResult.ResultSourceEnergyESM);
			primaryEnergyCoeficient2 = GetPrimaryEnergyCoeficient(zone.HeatingCalculations.HeatingResult.Fuel2ESM, zone.HeatingCalculations.HeatingResult.ResultSourceEnergy2ESM);
			num5 = primaryEnergyCoeficient * heatedArea + primaryEnergyCoeficient2 * heatedArea;
			zoneBalanceResult.PrimaryEnergyTable.Heating.ESM += num5;
			zoneBalanceResult.PrimaryEnergyTable.Heating.Savings = zoneBalanceResult.PrimaryEnergyTable.Heating.BaseLine - zoneBalanceResult.PrimaryEnergyTable.Heating.ESM;
		}
		if (zone.HasCooling)
		{
			double primaryEnergyCoeficient3 = GetPrimaryEnergyCoeficient(zone.CoolingCalculations.CoolingResult.Fuel1Ref1, zone.CoolingCalculations.CoolingResult.ResultSourceEnergyRef1);
			double primaryEnergyCoeficient4 = GetPrimaryEnergyCoeficient(zone.CoolingCalculations.CoolingResult.Fuel2Ref1, zone.CoolingCalculations.CoolingResult.ResultSourceEnergy2Ref1);
			num = primaryEnergyCoeficient3 * heatedArea + primaryEnergyCoeficient4 * heatedArea;
			zoneBalanceResult.PrimaryEnergyTable.Cooling.Ref1 += num;
			primaryEnergyCoeficient3 = GetPrimaryEnergyCoeficient(zone.CoolingCalculations.CoolingResult.Fuel1Ref2, zone.CoolingCalculations.CoolingResult.ResultSourceEnergyRef2);
			primaryEnergyCoeficient4 = GetPrimaryEnergyCoeficient(zone.CoolingCalculations.CoolingResult.Fuel2Ref2, zone.CoolingCalculations.CoolingResult.ResultSourceEnergy2Ref2);
			num2 = primaryEnergyCoeficient3 * heatedArea + primaryEnergyCoeficient4 * heatedArea;
			zoneBalanceResult.PrimaryEnergyTable.Cooling.Ref2 += num2;
			primaryEnergyCoeficient3 = GetPrimaryEnergyCoeficient(zone.CoolingCalculations.CoolingResult.Fuel1Actual, zone.CoolingCalculations.CoolingResult.ResultSourceEnergyActual);
			primaryEnergyCoeficient4 = GetPrimaryEnergyCoeficient(zone.CoolingCalculations.CoolingResult.Fuel2Actual, zone.CoolingCalculations.CoolingResult.ResultSourceEnergy2Actual);
			num3 = primaryEnergyCoeficient3 * heatedArea + primaryEnergyCoeficient4 * heatedArea;
			zoneBalanceResult.PrimaryEnergyTable.Cooling.Actual += num3;
			primaryEnergyCoeficient3 = GetPrimaryEnergyCoeficient(zone.CoolingCalculations.CoolingResult.Fuel1BaseLine, zone.CoolingCalculations.CoolingResult.ResultSourceEnergyBaseLine);
			primaryEnergyCoeficient4 = GetPrimaryEnergyCoeficient(zone.CoolingCalculations.CoolingResult.Fuel2BaseLine, zone.CoolingCalculations.CoolingResult.ResultSourceEnergy2BaseLine);
			num4 = primaryEnergyCoeficient3 * heatedArea + primaryEnergyCoeficient4 * heatedArea;
			zoneBalanceResult.PrimaryEnergyTable.Cooling.BaseLine += num4;
			primaryEnergyCoeficient3 = GetPrimaryEnergyCoeficient(zone.CoolingCalculations.CoolingResult.Fuel1ESM, zone.CoolingCalculations.CoolingResult.ResultSourceEnergyESM);
			primaryEnergyCoeficient4 = GetPrimaryEnergyCoeficient(zone.CoolingCalculations.CoolingResult.Fuel2ESM, zone.CoolingCalculations.CoolingResult.ResultSourceEnergy2ESM);
			num5 = primaryEnergyCoeficient3 * heatedArea + primaryEnergyCoeficient4 * heatedArea;
			zoneBalanceResult.PrimaryEnergyTable.Cooling.ESM += num5;
			zoneBalanceResult.PrimaryEnergyTable.Cooling.Savings = zoneBalanceResult.PrimaryEnergyTable.Cooling.BaseLine - zoneBalanceResult.PrimaryEnergyTable.Cooling.ESM;
		}
		if (isBGVused)
		{
			if (isFirstBuildingZone)
			{
				double primaryEnergyCoeficient5 = GetPrimaryEnergyCoeficient(zone.HeatingCalculations.HotWaterCalculations.Fuel1Ref1, zone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergyRef1);
				double primaryEnergyCoeficient6 = GetPrimaryEnergyCoeficient(zone.HeatingCalculations.HotWaterCalculations.Fuel2Ref1, zone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergy2Ref1);
				num = primaryEnergyCoeficient5 * totalArea + primaryEnergyCoeficient6 * totalArea;
				zoneBalanceResult.PrimaryEnergyTable.BGV.Ref1 += num;
				primaryEnergyCoeficient5 = GetPrimaryEnergyCoeficient(zone.HeatingCalculations.HotWaterCalculations.Fuel1Ref2, zone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergyRef2);
				primaryEnergyCoeficient6 = GetPrimaryEnergyCoeficient(zone.HeatingCalculations.HotWaterCalculations.Fuel2Ref2, zone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergy2Ref2);
				num2 = primaryEnergyCoeficient5 * totalArea + primaryEnergyCoeficient6 * totalArea;
				zoneBalanceResult.PrimaryEnergyTable.BGV.Ref2 += num2;
				primaryEnergyCoeficient5 = GetPrimaryEnergyCoeficient(zone.HeatingCalculations.HotWaterCalculations.Fuel1Actual, zone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergyActual);
				primaryEnergyCoeficient6 = GetPrimaryEnergyCoeficient(zone.HeatingCalculations.HotWaterCalculations.Fuel2Actual, zone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergy2Actual);
				num3 = primaryEnergyCoeficient5 * totalArea + primaryEnergyCoeficient6 * totalArea;
				zoneBalanceResult.PrimaryEnergyTable.BGV.Actual += num3;
				primaryEnergyCoeficient5 = GetPrimaryEnergyCoeficient(zone.HeatingCalculations.HotWaterCalculations.Fuel1BaseLine, zone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergyBaseLine);
				primaryEnergyCoeficient6 = GetPrimaryEnergyCoeficient(zone.HeatingCalculations.HotWaterCalculations.Fuel2BaseLine, zone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergy2BaseLine);
				num4 = primaryEnergyCoeficient5 * totalArea + primaryEnergyCoeficient6 * totalArea;
				zoneBalanceResult.PrimaryEnergyTable.BGV.BaseLine += num4;
				primaryEnergyCoeficient5 = GetPrimaryEnergyCoeficient(zone.HeatingCalculations.HotWaterCalculations.Fuel1ESM, zone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergyESM);
				primaryEnergyCoeficient6 = GetPrimaryEnergyCoeficient(zone.HeatingCalculations.HotWaterCalculations.Fuel2ESM, zone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergy2ESM);
				num5 = primaryEnergyCoeficient5 * totalArea + primaryEnergyCoeficient6 * totalArea;
				zoneBalanceResult.PrimaryEnergyTable.BGV.ESM += num5;
				zoneBalanceResult.PrimaryEnergyTable.BGVPumps.Ref1 = zoneBalanceResult.NeededEnergyTable.BGVPumps.Ref1 * 3.0;
				zoneBalanceResult.PrimaryEnergyTable.BGVPumps.Ref2 = zoneBalanceResult.NeededEnergyTable.BGVPumps.Ref2 * 3.0;
				zoneBalanceResult.PrimaryEnergyTable.BGVPumps.Actual = zoneBalanceResult.NeededEnergyTable.BGVPumps.Actual * 3.0;
				zoneBalanceResult.PrimaryEnergyTable.BGVPumps.BaseLine = zoneBalanceResult.NeededEnergyTable.BGVPumps.BaseLine * 3.0;
				zoneBalanceResult.PrimaryEnergyTable.BGVPumps.ESM = zoneBalanceResult.NeededEnergyTable.BGVPumps.ESM * 3.0;
			}
		}
		else
		{
			zoneBalanceResult.PrimaryEnergyTable.BGV.Ref1 = 0.0;
			zoneBalanceResult.PrimaryEnergyTable.BGV.Ref2 = 0.0;
			zoneBalanceResult.PrimaryEnergyTable.BGV.Actual = 0.0;
			zoneBalanceResult.PrimaryEnergyTable.BGV.BaseLine = 0.0;
			zoneBalanceResult.PrimaryEnergyTable.BGV.ESM = 0.0;
			zoneBalanceResult.PrimaryEnergyTable.BGVPumps.Ref1 = 0.0;
			zoneBalanceResult.PrimaryEnergyTable.BGVPumps.Ref2 = 0.0;
			zoneBalanceResult.PrimaryEnergyTable.BGVPumps.Actual = 0.0;
			zoneBalanceResult.PrimaryEnergyTable.BGVPumps.BaseLine = 0.0;
			zoneBalanceResult.PrimaryEnergyTable.BGVPumps.ESM = 0.0;
		}
		if (zone.HasHeating)
		{
			double primaryEnergyCoeficient7 = GetPrimaryEnergyCoeficient(zone.HeatingCalculations.VentilationHeating.Fuel1Ref1, zone.HeatingCalculations.VentilationHeating.ResultSourceEnergyRef1);
			double primaryEnergyCoeficient8 = GetPrimaryEnergyCoeficient(zone.HeatingCalculations.VentilationHeating.Fuel2Ref1, zone.HeatingCalculations.VentilationHeating.ResultSourceEnergy2Ref1);
			num = primaryEnergyCoeficient7 * heatedArea + primaryEnergyCoeficient8 * heatedArea;
			zoneBalanceResult.PrimaryEnergyTable.HeatingVentilation.Ref1 += num;
			primaryEnergyCoeficient7 = GetPrimaryEnergyCoeficient(zone.HeatingCalculations.VentilationHeating.Fuel1Ref2, zone.HeatingCalculations.VentilationHeating.ResultSourceEnergyRef2);
			primaryEnergyCoeficient8 = GetPrimaryEnergyCoeficient(zone.HeatingCalculations.VentilationHeating.Fuel2Ref2, zone.HeatingCalculations.VentilationHeating.ResultSourceEnergy2Ref2);
			num2 = primaryEnergyCoeficient7 * heatedArea + primaryEnergyCoeficient8 * heatedArea;
			zoneBalanceResult.PrimaryEnergyTable.HeatingVentilation.Ref2 += num2;
			primaryEnergyCoeficient7 = GetPrimaryEnergyCoeficient(zone.HeatingCalculations.VentilationHeating.Fuel1Actual, zone.HeatingCalculations.VentilationHeating.ResultSourceEnergyActual);
			primaryEnergyCoeficient8 = GetPrimaryEnergyCoeficient(zone.HeatingCalculations.VentilationHeating.Fuel2Actual, zone.HeatingCalculations.VentilationHeating.ResultSourceEnergy2Actual);
			num3 = primaryEnergyCoeficient7 * heatedArea + primaryEnergyCoeficient8 * heatedArea;
			zoneBalanceResult.PrimaryEnergyTable.HeatingVentilation.Actual += num3;
			primaryEnergyCoeficient7 = GetPrimaryEnergyCoeficient(zone.HeatingCalculations.VentilationHeating.Fuel1BaseLine, zone.HeatingCalculations.VentilationHeating.ResultSourceEnergyBaseLine);
			primaryEnergyCoeficient8 = GetPrimaryEnergyCoeficient(zone.HeatingCalculations.VentilationHeating.Fuel2BaseLine, zone.HeatingCalculations.VentilationHeating.ResultSourceEnergy2BaseLine);
			num4 = primaryEnergyCoeficient7 * heatedArea + primaryEnergyCoeficient8 * heatedArea;
			zoneBalanceResult.PrimaryEnergyTable.HeatingVentilation.BaseLine += num4;
			primaryEnergyCoeficient7 = GetPrimaryEnergyCoeficient(zone.HeatingCalculations.VentilationHeating.Fuel1ESM, zone.HeatingCalculations.VentilationHeating.ResultSourceEnergyESM);
			primaryEnergyCoeficient8 = GetPrimaryEnergyCoeficient(zone.HeatingCalculations.VentilationHeating.Fuel2ESM, zone.HeatingCalculations.VentilationHeating.ResultSourceEnergy2ESM);
			num5 = primaryEnergyCoeficient7 * heatedArea + primaryEnergyCoeficient8 * heatedArea;
			zoneBalanceResult.PrimaryEnergyTable.HeatingVentilation.ESM += num5;
			zoneBalanceResult.PrimaryEnergyTable.HeatingVentilation.Savings = zoneBalanceResult.PrimaryEnergyTable.HeatingVentilation.BaseLine - zoneBalanceResult.PrimaryEnergyTable.HeatingVentilation.ESM;
		}
		if (zone.HasCooling)
		{
			double primaryEnergyCoeficient9 = GetPrimaryEnergyCoeficient(zone.CoolingCalculations.VentilationCooling.Fuel1Ref1, zone.CoolingCalculations.VentilationCooling.ResultSourceEnergyRef1);
			double primaryEnergyCoeficient10 = GetPrimaryEnergyCoeficient(zone.CoolingCalculations.VentilationCooling.Fuel2Ref1, zone.CoolingCalculations.VentilationCooling.ResultSourceEnergy2Ref1);
			num = primaryEnergyCoeficient9 * heatedArea + primaryEnergyCoeficient10 * heatedArea;
			zoneBalanceResult.PrimaryEnergyTable.CoolingVentilation.Ref1 += num;
			primaryEnergyCoeficient9 = GetPrimaryEnergyCoeficient(zone.CoolingCalculations.VentilationCooling.Fuel1Ref2, zone.CoolingCalculations.VentilationCooling.ResultSourceEnergyRef2);
			primaryEnergyCoeficient10 = GetPrimaryEnergyCoeficient(zone.CoolingCalculations.VentilationCooling.Fuel2Ref2, zone.CoolingCalculations.VentilationCooling.ResultSourceEnergy2Ref2);
			num2 = primaryEnergyCoeficient9 * heatedArea + primaryEnergyCoeficient10 * heatedArea;
			zoneBalanceResult.PrimaryEnergyTable.CoolingVentilation.Ref2 += num2;
			primaryEnergyCoeficient9 = GetPrimaryEnergyCoeficient(zone.CoolingCalculations.VentilationCooling.Fuel1Actual, zone.CoolingCalculations.VentilationCooling.ResultSourceEnergyActual);
			primaryEnergyCoeficient10 = GetPrimaryEnergyCoeficient(zone.CoolingCalculations.VentilationCooling.Fuel2Actual, zone.CoolingCalculations.VentilationCooling.ResultSourceEnergy2Actual);
			num3 = primaryEnergyCoeficient9 * heatedArea + primaryEnergyCoeficient10 * heatedArea;
			zoneBalanceResult.PrimaryEnergyTable.CoolingVentilation.Actual += num3;
			primaryEnergyCoeficient9 = GetPrimaryEnergyCoeficient(zone.CoolingCalculations.VentilationCooling.Fuel1BaseLine, zone.CoolingCalculations.VentilationCooling.ResultSourceEnergyBaseLine);
			primaryEnergyCoeficient10 = GetPrimaryEnergyCoeficient(zone.CoolingCalculations.VentilationCooling.Fuel2BaseLine, zone.CoolingCalculations.VentilationCooling.ResultSourceEnergy2BaseLine);
			num4 = primaryEnergyCoeficient9 * heatedArea + primaryEnergyCoeficient10 * heatedArea;
			zoneBalanceResult.PrimaryEnergyTable.CoolingVentilation.BaseLine += num4;
			primaryEnergyCoeficient9 = GetPrimaryEnergyCoeficient(zone.CoolingCalculations.VentilationCooling.Fuel1ESM, zone.CoolingCalculations.VentilationCooling.ResultSourceEnergyESM);
			primaryEnergyCoeficient10 = GetPrimaryEnergyCoeficient(zone.CoolingCalculations.VentilationCooling.Fuel2ESM, zone.CoolingCalculations.VentilationCooling.ResultSourceEnergy2ESM);
			num5 = primaryEnergyCoeficient9 * heatedArea + primaryEnergyCoeficient10 * heatedArea;
			zoneBalanceResult.PrimaryEnergyTable.CoolingVentilation.ESM += num5;
			zoneBalanceResult.PrimaryEnergyTable.CoolingVentilation.Savings = zoneBalanceResult.PrimaryEnergyTable.CoolingVentilation.BaseLine - zoneBalanceResult.PrimaryEnergyTable.CoolingVentilation.ESM;
		}
		double primaryEnergyCoeficient11 = GetPrimaryEnergyCoeficient(Fuel.Fuel1, zone.HeatingCalculations.FansAndPumps.CoolNeededEnergyRef1 + zone.HeatingCalculations.FansAndPumps.PumpNeededEnergyRef1);
		num = primaryEnergyCoeficient11 * heatedArea;
		zoneBalanceResult.PrimaryEnergyTable.FansAndPumps.Ref1 += num;
		primaryEnergyCoeficient11 = GetPrimaryEnergyCoeficient(Fuel.Fuel1, zone.HeatingCalculations.FansAndPumps.CoolNeededEnergyRef2 + zone.HeatingCalculations.FansAndPumps.PumpNeededEnergyRef2);
		num2 = primaryEnergyCoeficient11 * heatedArea;
		zoneBalanceResult.PrimaryEnergyTable.FansAndPumps.Ref2 += num2;
		primaryEnergyCoeficient11 = GetPrimaryEnergyCoeficient(Fuel.Fuel1, zone.HeatingCalculations.FansAndPumps.CoolNeededEnergyActual + zone.HeatingCalculations.FansAndPumps.PumpNeededEnergyActual);
		num3 = primaryEnergyCoeficient11 * heatedArea;
		zoneBalanceResult.PrimaryEnergyTable.FansAndPumps.Actual += num3;
		primaryEnergyCoeficient11 = GetPrimaryEnergyCoeficient(Fuel.Fuel1, zone.HeatingCalculations.FansAndPumps.CoolNeededEnergyBaseLine + zone.HeatingCalculations.FansAndPumps.PumpNeededEnergyBaseLine);
		num4 = primaryEnergyCoeficient11 * heatedArea;
		zoneBalanceResult.PrimaryEnergyTable.FansAndPumps.BaseLine += num4;
		primaryEnergyCoeficient11 = GetPrimaryEnergyCoeficient(Fuel.Fuel1, zone.HeatingCalculations.FansAndPumps.CoolNeededEnergyESM + zone.HeatingCalculations.FansAndPumps.PumpNeededEnergyESM);
		num5 = primaryEnergyCoeficient11 * heatedArea;
		zoneBalanceResult.PrimaryEnergyTable.FansAndPumps.ESM += num5;
		zoneBalanceResult.PrimaryEnergyTable.FansAndPumps.Savings = zoneBalanceResult.PrimaryEnergyTable.FansAndPumps.BaseLine - zoneBalanceResult.PrimaryEnergyTable.FansAndPumps.ESM;
		primaryEnergyCoeficient11 = GetPrimaryEnergyCoeficient(Fuel.Fuel1, zone.HeatingCalculations.LightAndDevices.Lights.General.DevicesNeededEnergyRef1);
		num = primaryEnergyCoeficient11 * heatedArea;
		zoneBalanceResult.PrimaryEnergyTable.Lights.Ref1 += num;
		primaryEnergyCoeficient11 = GetPrimaryEnergyCoeficient(Fuel.Fuel1, zone.HeatingCalculations.LightAndDevices.Lights.General.DevicesNeededEnergyRef2);
		num2 = primaryEnergyCoeficient11 * heatedArea;
		zoneBalanceResult.PrimaryEnergyTable.Lights.Ref2 += num2;
		primaryEnergyCoeficient11 = GetPrimaryEnergyCoeficient(Fuel.Fuel1, zone.HeatingCalculations.LightAndDevices.Lights.General.DevicesNeededEnergyActual);
		num3 = primaryEnergyCoeficient11 * heatedArea;
		zoneBalanceResult.PrimaryEnergyTable.Lights.Actual += num3;
		primaryEnergyCoeficient11 = GetPrimaryEnergyCoeficient(Fuel.Fuel1, zone.HeatingCalculations.LightAndDevices.Lights.General.DevicesNeededEnergyBaseLine);
		num4 = primaryEnergyCoeficient11 * heatedArea;
		zoneBalanceResult.PrimaryEnergyTable.Lights.BaseLine += num4;
		primaryEnergyCoeficient11 = GetPrimaryEnergyCoeficient(Fuel.Fuel1, zone.HeatingCalculations.LightAndDevices.Lights.General.DevicesNeededEnergyESM);
		num5 = primaryEnergyCoeficient11 * heatedArea;
		zoneBalanceResult.PrimaryEnergyTable.Lights.ESM += num5;
		zoneBalanceResult.PrimaryEnergyTable.Lights.Savings = zoneBalanceResult.PrimaryEnergyTable.Lights.BaseLine - zoneBalanceResult.PrimaryEnergyTable.Lights.ESM;
		primaryEnergyCoeficient11 = GetPrimaryEnergyCoeficient(Fuel.Fuel1, zone.HeatingCalculations.LightAndDevices.BalancedDevices.General.DevicesNeededEnergyRef1);
		num = primaryEnergyCoeficient11 * heatedArea;
		zoneBalanceResult.PrimaryEnergyTable.HeatAffectingDevices.Ref1 += num;
		primaryEnergyCoeficient11 = GetPrimaryEnergyCoeficient(Fuel.Fuel1, zone.HeatingCalculations.LightAndDevices.BalancedDevices.General.DevicesNeededEnergyRef2);
		num2 = primaryEnergyCoeficient11 * heatedArea;
		zoneBalanceResult.PrimaryEnergyTable.HeatAffectingDevices.Ref2 += num2;
		primaryEnergyCoeficient11 = GetPrimaryEnergyCoeficient(Fuel.Fuel1, zone.HeatingCalculations.LightAndDevices.BalancedDevices.General.DevicesNeededEnergyActual);
		num3 = primaryEnergyCoeficient11 * heatedArea;
		zoneBalanceResult.PrimaryEnergyTable.HeatAffectingDevices.Actual += num3;
		primaryEnergyCoeficient11 = GetPrimaryEnergyCoeficient(Fuel.Fuel1, zone.HeatingCalculations.LightAndDevices.BalancedDevices.General.DevicesNeededEnergyBaseLine);
		num4 = primaryEnergyCoeficient11 * heatedArea;
		zoneBalanceResult.PrimaryEnergyTable.HeatAffectingDevices.BaseLine += num4;
		primaryEnergyCoeficient11 = GetPrimaryEnergyCoeficient(Fuel.Fuel1, zone.HeatingCalculations.LightAndDevices.BalancedDevices.General.DevicesNeededEnergyESM);
		num5 = primaryEnergyCoeficient11 * heatedArea;
		zoneBalanceResult.PrimaryEnergyTable.HeatAffectingDevices.ESM += num5;
		zoneBalanceResult.PrimaryEnergyTable.HeatAffectingDevices.Savings = zoneBalanceResult.PrimaryEnergyTable.HeatAffectingDevices.BaseLine - zoneBalanceResult.PrimaryEnergyTable.HeatAffectingDevices.ESM;
		primaryEnergyCoeficient11 = GetPrimaryEnergyCoeficient(Fuel.Fuel1, zone.HeatingCalculations.LightAndDevices.NonBalancedDevices.General.DevicesNeededEnergyRef1);
		num = primaryEnergyCoeficient11 * heatedArea;
		zoneBalanceResult.PrimaryEnergyTable.NonHeatAffectingDevices.Ref1 += num;
		primaryEnergyCoeficient11 = GetPrimaryEnergyCoeficient(Fuel.Fuel1, zone.HeatingCalculations.LightAndDevices.NonBalancedDevices.General.DevicesNeededEnergyRef2);
		num2 = primaryEnergyCoeficient11 * heatedArea;
		zoneBalanceResult.PrimaryEnergyTable.NonHeatAffectingDevices.Ref2 += num2;
		primaryEnergyCoeficient11 = GetPrimaryEnergyCoeficient(Fuel.Fuel1, zone.HeatingCalculations.LightAndDevices.NonBalancedDevices.General.DevicesNeededEnergyActual);
		num3 = primaryEnergyCoeficient11 * heatedArea;
		zoneBalanceResult.PrimaryEnergyTable.NonHeatAffectingDevices.Actual += num3;
		primaryEnergyCoeficient11 = GetPrimaryEnergyCoeficient(Fuel.Fuel1, zone.HeatingCalculations.LightAndDevices.NonBalancedDevices.General.DevicesNeededEnergyBaseLine);
		num4 = primaryEnergyCoeficient11 * heatedArea;
		zoneBalanceResult.PrimaryEnergyTable.NonHeatAffectingDevices.BaseLine += num4;
		primaryEnergyCoeficient11 = GetPrimaryEnergyCoeficient(Fuel.Fuel1, zone.HeatingCalculations.LightAndDevices.NonBalancedDevices.General.DevicesNeededEnergyESM);
		num5 = primaryEnergyCoeficient11 * heatedArea;
		zoneBalanceResult.PrimaryEnergyTable.NonHeatAffectingDevices.ESM += num5;
		zoneBalanceResult.PrimaryEnergyTable.NonHeatAffectingDevices.Savings = zoneBalanceResult.PrimaryEnergyTable.NonHeatAffectingDevices.BaseLine - zoneBalanceResult.PrimaryEnergyTable.NonHeatAffectingDevices.ESM;
		primaryEnergyCoeficient11 = GetPrimaryEnergyCoeficient(Fuel.Fuel1, zone.HeatingCalculations.FansAndPumps.OtherResultCoolingRef1);
		num = primaryEnergyCoeficient11 * heatedArea;
		zoneBalanceResult.PrimaryEnergyTable.Other.Ref1 += num;
		primaryEnergyCoeficient11 = GetPrimaryEnergyCoeficient(Fuel.Fuel1, zone.HeatingCalculations.FansAndPumps.OtherResultCoolingRef2);
		num2 = primaryEnergyCoeficient11 * heatedArea;
		zoneBalanceResult.PrimaryEnergyTable.Other.Ref2 += num2;
		primaryEnergyCoeficient11 = GetPrimaryEnergyCoeficient(Fuel.Fuel1, zone.HeatingCalculations.FansAndPumps.OtherResultCoolingActual);
		num3 = primaryEnergyCoeficient11 * heatedArea;
		zoneBalanceResult.PrimaryEnergyTable.Other.Actual += num3;
		primaryEnergyCoeficient11 = GetPrimaryEnergyCoeficient(Fuel.Fuel1, zone.HeatingCalculations.FansAndPumps.OtherResultCoolingBaseLine);
		num4 = primaryEnergyCoeficient11 * heatedArea;
		zoneBalanceResult.PrimaryEnergyTable.Other.BaseLine += num4;
		primaryEnergyCoeficient11 = GetPrimaryEnergyCoeficient(Fuel.Fuel1, zone.HeatingCalculations.FansAndPumps.OtherResultCoolingESM);
		num5 = primaryEnergyCoeficient11 * heatedArea;
		zoneBalanceResult.PrimaryEnergyTable.Other.ESM += num5;
		zoneBalanceResult.PrimaryEnergyTable.Other.Savings = zoneBalanceResult.PrimaryEnergyTable.Other.BaseLine - zoneBalanceResult.PrimaryEnergyTable.Other.ESM;
	}

	private static void CalculatePrimaryEnergyPerArea(Results buildingBalanceResult, double area, bool isBGVused)
	{
		buildingBalanceResult.PrimaryEnergyTable.Heating.Ref1 = buildingBalanceResult.PrimaryEnergyTable.Heating.Ref1 / area;
		buildingBalanceResult.PrimaryEnergyTable.Heating.Ref2 = buildingBalanceResult.PrimaryEnergyTable.Heating.Ref2 / area;
		buildingBalanceResult.PrimaryEnergyTable.Heating.Actual = buildingBalanceResult.PrimaryEnergyTable.Heating.Actual / area;
		buildingBalanceResult.PrimaryEnergyTable.Heating.BaseLine = buildingBalanceResult.PrimaryEnergyTable.Heating.BaseLine / area;
		buildingBalanceResult.PrimaryEnergyTable.Heating.ESM = buildingBalanceResult.PrimaryEnergyTable.Heating.ESM / area;
		buildingBalanceResult.PrimaryEnergyTable.Heating.Savings = buildingBalanceResult.PrimaryEnergyTable.Heating.BaseLine - buildingBalanceResult.PrimaryEnergyTable.Heating.ESM;
		buildingBalanceResult.PrimaryEnergyTable.Cooling.Ref1 = buildingBalanceResult.PrimaryEnergyTable.Cooling.Ref1 / area;
		buildingBalanceResult.PrimaryEnergyTable.Cooling.Ref2 = buildingBalanceResult.PrimaryEnergyTable.Cooling.Ref2 / area;
		buildingBalanceResult.PrimaryEnergyTable.Cooling.Actual = buildingBalanceResult.PrimaryEnergyTable.Cooling.Actual / area;
		buildingBalanceResult.PrimaryEnergyTable.Cooling.BaseLine = buildingBalanceResult.PrimaryEnergyTable.Cooling.BaseLine / area;
		buildingBalanceResult.PrimaryEnergyTable.Cooling.ESM = buildingBalanceResult.PrimaryEnergyTable.Cooling.ESM / area;
		buildingBalanceResult.PrimaryEnergyTable.Cooling.Savings = buildingBalanceResult.PrimaryEnergyTable.Cooling.BaseLine - buildingBalanceResult.PrimaryEnergyTable.Cooling.ESM;
		buildingBalanceResult.PrimaryEnergyTable.HeatingVentilation.Ref1 = buildingBalanceResult.PrimaryEnergyTable.HeatingVentilation.Ref1 / area;
		buildingBalanceResult.PrimaryEnergyTable.HeatingVentilation.Ref2 = buildingBalanceResult.PrimaryEnergyTable.HeatingVentilation.Ref2 / area;
		buildingBalanceResult.PrimaryEnergyTable.HeatingVentilation.Actual = buildingBalanceResult.PrimaryEnergyTable.HeatingVentilation.Actual / area;
		buildingBalanceResult.PrimaryEnergyTable.HeatingVentilation.BaseLine = buildingBalanceResult.PrimaryEnergyTable.HeatingVentilation.BaseLine / area;
		buildingBalanceResult.PrimaryEnergyTable.HeatingVentilation.ESM = buildingBalanceResult.PrimaryEnergyTable.HeatingVentilation.ESM / area;
		buildingBalanceResult.PrimaryEnergyTable.HeatingVentilation.Savings = buildingBalanceResult.PrimaryEnergyTable.HeatingVentilation.BaseLine - buildingBalanceResult.PrimaryEnergyTable.HeatingVentilation.ESM;
		buildingBalanceResult.PrimaryEnergyTable.CoolingVentilation.Ref1 = buildingBalanceResult.PrimaryEnergyTable.CoolingVentilation.Ref1 / area;
		buildingBalanceResult.PrimaryEnergyTable.CoolingVentilation.Ref2 = buildingBalanceResult.PrimaryEnergyTable.CoolingVentilation.Ref2 / area;
		buildingBalanceResult.PrimaryEnergyTable.CoolingVentilation.Actual = buildingBalanceResult.PrimaryEnergyTable.CoolingVentilation.Actual / area;
		buildingBalanceResult.PrimaryEnergyTable.CoolingVentilation.BaseLine = buildingBalanceResult.PrimaryEnergyTable.CoolingVentilation.BaseLine / area;
		buildingBalanceResult.PrimaryEnergyTable.CoolingVentilation.ESM = buildingBalanceResult.PrimaryEnergyTable.CoolingVentilation.ESM / area;
		buildingBalanceResult.PrimaryEnergyTable.CoolingVentilation.Savings = buildingBalanceResult.PrimaryEnergyTable.CoolingVentilation.BaseLine - buildingBalanceResult.PrimaryEnergyTable.CoolingVentilation.ESM;
		if (isBGVused)
		{
			buildingBalanceResult.PrimaryEnergyTable.BGV.Ref1 = buildingBalanceResult.PrimaryEnergyTable.BGV.Ref1 / area;
			buildingBalanceResult.PrimaryEnergyTable.BGV.Ref2 = buildingBalanceResult.PrimaryEnergyTable.BGV.Ref2 / area;
			buildingBalanceResult.PrimaryEnergyTable.BGV.Actual = buildingBalanceResult.PrimaryEnergyTable.BGV.Actual / area;
			buildingBalanceResult.PrimaryEnergyTable.BGV.BaseLine = buildingBalanceResult.PrimaryEnergyTable.BGV.BaseLine / area;
			buildingBalanceResult.PrimaryEnergyTable.BGV.ESM = buildingBalanceResult.PrimaryEnergyTable.BGV.ESM / area;
			buildingBalanceResult.PrimaryEnergyTable.BGV.Savings = buildingBalanceResult.PrimaryEnergyTable.BGV.BaseLine - buildingBalanceResult.PrimaryEnergyTable.BGV.ESM;
			buildingBalanceResult.PrimaryEnergyTable.BGVPumps.Ref1 = buildingBalanceResult.PrimaryEnergyTable.BGVPumps.Ref1 / area;
			buildingBalanceResult.PrimaryEnergyTable.BGVPumps.Ref2 = buildingBalanceResult.PrimaryEnergyTable.BGVPumps.Ref2 / area;
			buildingBalanceResult.PrimaryEnergyTable.BGVPumps.Actual = buildingBalanceResult.PrimaryEnergyTable.BGVPumps.Actual / area;
			buildingBalanceResult.PrimaryEnergyTable.BGVPumps.BaseLine = buildingBalanceResult.PrimaryEnergyTable.BGVPumps.BaseLine / area;
			buildingBalanceResult.PrimaryEnergyTable.BGVPumps.ESM = buildingBalanceResult.PrimaryEnergyTable.BGVPumps.ESM / area;
			buildingBalanceResult.PrimaryEnergyTable.BGVPumps.Savings = buildingBalanceResult.PrimaryEnergyTable.BGVPumps.BaseLine - buildingBalanceResult.PrimaryEnergyTable.BGVPumps.ESM;
		}
		else
		{
			buildingBalanceResult.PrimaryEnergyTable.BGV.Ref1 = 0.0;
			buildingBalanceResult.PrimaryEnergyTable.BGV.Ref2 = 0.0;
			buildingBalanceResult.PrimaryEnergyTable.BGV.Actual = 0.0;
			buildingBalanceResult.PrimaryEnergyTable.BGV.BaseLine = 0.0;
			buildingBalanceResult.PrimaryEnergyTable.BGV.ESM = 0.0;
			buildingBalanceResult.PrimaryEnergyTable.BGVPumps.Ref1 = 0.0;
			buildingBalanceResult.PrimaryEnergyTable.BGVPumps.Ref2 = 0.0;
			buildingBalanceResult.PrimaryEnergyTable.BGVPumps.Actual = 0.0;
			buildingBalanceResult.PrimaryEnergyTable.BGVPumps.BaseLine = 0.0;
			buildingBalanceResult.PrimaryEnergyTable.BGVPumps.ESM = 0.0;
		}
		buildingBalanceResult.PrimaryEnergyTable.FansAndPumps.Ref1 = buildingBalanceResult.PrimaryEnergyTable.FansAndPumps.Ref1 / area;
		buildingBalanceResult.PrimaryEnergyTable.FansAndPumps.Ref2 = buildingBalanceResult.PrimaryEnergyTable.FansAndPumps.Ref2 / area;
		buildingBalanceResult.PrimaryEnergyTable.FansAndPumps.Actual = buildingBalanceResult.PrimaryEnergyTable.FansAndPumps.Actual / area;
		buildingBalanceResult.PrimaryEnergyTable.FansAndPumps.BaseLine = buildingBalanceResult.PrimaryEnergyTable.FansAndPumps.BaseLine / area;
		buildingBalanceResult.PrimaryEnergyTable.FansAndPumps.ESM = buildingBalanceResult.PrimaryEnergyTable.FansAndPumps.ESM / area;
		buildingBalanceResult.PrimaryEnergyTable.FansAndPumps.Savings = buildingBalanceResult.PrimaryEnergyTable.FansAndPumps.BaseLine - buildingBalanceResult.PrimaryEnergyTable.FansAndPumps.ESM;
		buildingBalanceResult.PrimaryEnergyTable.Lights.Ref1 = buildingBalanceResult.PrimaryEnergyTable.Lights.Ref1 / area;
		buildingBalanceResult.PrimaryEnergyTable.Lights.Ref2 = buildingBalanceResult.PrimaryEnergyTable.Lights.Ref2 / area;
		buildingBalanceResult.PrimaryEnergyTable.Lights.Actual = buildingBalanceResult.PrimaryEnergyTable.Lights.Actual / area;
		buildingBalanceResult.PrimaryEnergyTable.Lights.BaseLine = buildingBalanceResult.PrimaryEnergyTable.Lights.BaseLine / area;
		buildingBalanceResult.PrimaryEnergyTable.Lights.ESM = buildingBalanceResult.PrimaryEnergyTable.Lights.ESM / area;
		buildingBalanceResult.PrimaryEnergyTable.Lights.Savings = buildingBalanceResult.PrimaryEnergyTable.Lights.BaseLine - buildingBalanceResult.PrimaryEnergyTable.Lights.ESM;
		buildingBalanceResult.PrimaryEnergyTable.HeatAffectingDevices.Ref1 = buildingBalanceResult.PrimaryEnergyTable.HeatAffectingDevices.Ref1 / area;
		buildingBalanceResult.PrimaryEnergyTable.HeatAffectingDevices.Ref2 = buildingBalanceResult.PrimaryEnergyTable.HeatAffectingDevices.Ref2 / area;
		buildingBalanceResult.PrimaryEnergyTable.HeatAffectingDevices.Actual = buildingBalanceResult.PrimaryEnergyTable.HeatAffectingDevices.Actual / area;
		buildingBalanceResult.PrimaryEnergyTable.HeatAffectingDevices.BaseLine = buildingBalanceResult.PrimaryEnergyTable.HeatAffectingDevices.BaseLine / area;
		buildingBalanceResult.PrimaryEnergyTable.HeatAffectingDevices.ESM = buildingBalanceResult.PrimaryEnergyTable.HeatAffectingDevices.ESM / area;
		buildingBalanceResult.PrimaryEnergyTable.HeatAffectingDevices.Savings = buildingBalanceResult.PrimaryEnergyTable.HeatAffectingDevices.BaseLine - buildingBalanceResult.PrimaryEnergyTable.HeatAffectingDevices.ESM;
		buildingBalanceResult.PrimaryEnergyTable.NonHeatAffectingDevices.Ref1 = buildingBalanceResult.PrimaryEnergyTable.NonHeatAffectingDevices.Ref1 / area;
		buildingBalanceResult.PrimaryEnergyTable.NonHeatAffectingDevices.Ref2 = buildingBalanceResult.PrimaryEnergyTable.NonHeatAffectingDevices.Ref2 / area;
		buildingBalanceResult.PrimaryEnergyTable.NonHeatAffectingDevices.Actual = buildingBalanceResult.PrimaryEnergyTable.NonHeatAffectingDevices.Actual / area;
		buildingBalanceResult.PrimaryEnergyTable.NonHeatAffectingDevices.BaseLine = buildingBalanceResult.PrimaryEnergyTable.NonHeatAffectingDevices.BaseLine / area;
		buildingBalanceResult.PrimaryEnergyTable.NonHeatAffectingDevices.ESM = buildingBalanceResult.PrimaryEnergyTable.NonHeatAffectingDevices.ESM / area;
		buildingBalanceResult.PrimaryEnergyTable.NonHeatAffectingDevices.Savings = buildingBalanceResult.PrimaryEnergyTable.NonHeatAffectingDevices.BaseLine - buildingBalanceResult.PrimaryEnergyTable.NonHeatAffectingDevices.ESM;
		buildingBalanceResult.PrimaryEnergyTable.Other.Ref1 = buildingBalanceResult.PrimaryEnergyTable.Other.Ref1 / area;
		buildingBalanceResult.PrimaryEnergyTable.Other.Ref2 = buildingBalanceResult.PrimaryEnergyTable.Other.Ref2 / area;
		buildingBalanceResult.PrimaryEnergyTable.Other.Actual = buildingBalanceResult.PrimaryEnergyTable.Other.Actual / area;
		buildingBalanceResult.PrimaryEnergyTable.Other.BaseLine = buildingBalanceResult.PrimaryEnergyTable.Other.BaseLine / area;
		buildingBalanceResult.PrimaryEnergyTable.Other.ESM = buildingBalanceResult.PrimaryEnergyTable.Other.ESM / area;
		buildingBalanceResult.PrimaryEnergyTable.Other.Savings = buildingBalanceResult.PrimaryEnergyTable.Other.BaseLine - buildingBalanceResult.PrimaryEnergyTable.Other.ESM;
		buildingBalanceResult.PrimaryEnergyTable.Devices.Ref1 = buildingBalanceResult.PrimaryEnergyTable.Devices.Ref1 / area;
		buildingBalanceResult.PrimaryEnergyTable.Devices.Ref2 = buildingBalanceResult.PrimaryEnergyTable.Devices.Ref2 / area;
		buildingBalanceResult.PrimaryEnergyTable.Devices.Actual = buildingBalanceResult.PrimaryEnergyTable.Devices.Actual / area;
		buildingBalanceResult.PrimaryEnergyTable.Devices.BaseLine = buildingBalanceResult.PrimaryEnergyTable.Devices.BaseLine / area;
		buildingBalanceResult.PrimaryEnergyTable.Devices.ESM = buildingBalanceResult.PrimaryEnergyTable.Devices.ESM / area;
		buildingBalanceResult.PrimaryEnergyTable.Devices.Savings = buildingBalanceResult.PrimaryEnergyTable.Devices.BaseLine - buildingBalanceResult.PrimaryEnergyTable.Devices.ESM;
		buildingBalanceResult.PrimaryEnergyTable.Total.Ref1 = buildingBalanceResult.PrimaryEnergyTable.Heating.Ref1 + buildingBalanceResult.PrimaryEnergyTable.HeatingVentilation.Ref1 + buildingBalanceResult.PrimaryEnergyTable.Cooling.Ref1 + buildingBalanceResult.PrimaryEnergyTable.CoolingVentilation.Ref1 + buildingBalanceResult.PrimaryEnergyTable.BGV.Ref1 + buildingBalanceResult.PrimaryEnergyTable.BGVPumps.Ref1 + buildingBalanceResult.PrimaryEnergyTable.FansAndPumps.Ref1 + buildingBalanceResult.PrimaryEnergyTable.Lights.Ref1 + buildingBalanceResult.PrimaryEnergyTable.HeatAffectingDevices.Ref1 + buildingBalanceResult.PrimaryEnergyTable.NonHeatAffectingDevices.Ref1 + buildingBalanceResult.PrimaryEnergyTable.Devices.Ref1;
		buildingBalanceResult.PrimaryEnergyTable.Total.Ref2 = buildingBalanceResult.PrimaryEnergyTable.Heating.Ref2 + buildingBalanceResult.PrimaryEnergyTable.HeatingVentilation.Ref2 + buildingBalanceResult.PrimaryEnergyTable.Cooling.Ref2 + buildingBalanceResult.PrimaryEnergyTable.CoolingVentilation.Ref2 + buildingBalanceResult.PrimaryEnergyTable.BGV.Ref2 + buildingBalanceResult.PrimaryEnergyTable.BGVPumps.Ref2 + buildingBalanceResult.PrimaryEnergyTable.FansAndPumps.Ref2 + buildingBalanceResult.PrimaryEnergyTable.Lights.Ref2 + buildingBalanceResult.PrimaryEnergyTable.HeatAffectingDevices.Ref2 + buildingBalanceResult.PrimaryEnergyTable.NonHeatAffectingDevices.Ref2 + buildingBalanceResult.PrimaryEnergyTable.Devices.Ref2;
		buildingBalanceResult.PrimaryEnergyTable.Total.Actual = buildingBalanceResult.PrimaryEnergyTable.Heating.Actual + buildingBalanceResult.PrimaryEnergyTable.HeatingVentilation.Actual + buildingBalanceResult.PrimaryEnergyTable.Cooling.Actual + buildingBalanceResult.PrimaryEnergyTable.CoolingVentilation.Actual + buildingBalanceResult.PrimaryEnergyTable.BGV.Actual + buildingBalanceResult.PrimaryEnergyTable.BGVPumps.Actual + buildingBalanceResult.PrimaryEnergyTable.FansAndPumps.Actual + buildingBalanceResult.PrimaryEnergyTable.Lights.Actual + buildingBalanceResult.PrimaryEnergyTable.HeatAffectingDevices.Actual + buildingBalanceResult.PrimaryEnergyTable.NonHeatAffectingDevices.Actual + buildingBalanceResult.PrimaryEnergyTable.Devices.Actual;
		buildingBalanceResult.PrimaryEnergyTable.Total.BaseLine = buildingBalanceResult.PrimaryEnergyTable.Heating.BaseLine + buildingBalanceResult.PrimaryEnergyTable.HeatingVentilation.BaseLine + buildingBalanceResult.PrimaryEnergyTable.Cooling.BaseLine + buildingBalanceResult.PrimaryEnergyTable.CoolingVentilation.BaseLine + buildingBalanceResult.PrimaryEnergyTable.BGV.BaseLine + buildingBalanceResult.PrimaryEnergyTable.BGVPumps.BaseLine + buildingBalanceResult.PrimaryEnergyTable.FansAndPumps.BaseLine + buildingBalanceResult.PrimaryEnergyTable.Lights.BaseLine + buildingBalanceResult.PrimaryEnergyTable.HeatAffectingDevices.BaseLine + buildingBalanceResult.PrimaryEnergyTable.NonHeatAffectingDevices.BaseLine + buildingBalanceResult.PrimaryEnergyTable.Devices.BaseLine;
		buildingBalanceResult.PrimaryEnergyTable.Total.ESM = buildingBalanceResult.PrimaryEnergyTable.Heating.ESM + buildingBalanceResult.PrimaryEnergyTable.HeatingVentilation.ESM + buildingBalanceResult.PrimaryEnergyTable.Cooling.ESM + buildingBalanceResult.PrimaryEnergyTable.CoolingVentilation.ESM + buildingBalanceResult.PrimaryEnergyTable.BGV.ESM + buildingBalanceResult.PrimaryEnergyTable.BGVPumps.ESM + buildingBalanceResult.PrimaryEnergyTable.FansAndPumps.ESM + buildingBalanceResult.PrimaryEnergyTable.Lights.ESM + buildingBalanceResult.PrimaryEnergyTable.HeatAffectingDevices.ESM + buildingBalanceResult.PrimaryEnergyTable.NonHeatAffectingDevices.ESM + buildingBalanceResult.PrimaryEnergyTable.Devices.ESM;
		buildingBalanceResult.PrimaryEnergyTable.Total.Savings = buildingBalanceResult.PrimaryEnergyTable.Heating.Savings + buildingBalanceResult.PrimaryEnergyTable.HeatingVentilation.Savings + buildingBalanceResult.PrimaryEnergyTable.Cooling.Savings + buildingBalanceResult.PrimaryEnergyTable.CoolingVentilation.Savings + buildingBalanceResult.PrimaryEnergyTable.BGV.Savings + buildingBalanceResult.PrimaryEnergyTable.BGVPumps.Savings + buildingBalanceResult.PrimaryEnergyTable.FansAndPumps.Savings + buildingBalanceResult.PrimaryEnergyTable.Lights.Savings + buildingBalanceResult.PrimaryEnergyTable.HeatAffectingDevices.Savings + buildingBalanceResult.PrimaryEnergyTable.NonHeatAffectingDevices.Savings + buildingBalanceResult.PrimaryEnergyTable.Devices.Savings;
	}

	private static void CalculatePrimaryFuelTypeAndValuesPerArea(Results buildingBalanceResult, double area)
	{
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel1.Ref1 = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel1.Ref1 / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel1.Ref2 = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel1.Ref2 / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel1.Actual = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel1.Actual / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel1.BaseLine = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel1.BaseLine / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel1.ESM = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel1.ESM / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel1.Savings = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel1.BaseLine - buildingBalanceResult.PrimaryEnergyFuelTable.Fuel1.ESM;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel2.Ref1 = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel2.Ref1 / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel2.Ref2 = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel2.Ref2 / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel2.Actual = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel2.Actual / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel2.BaseLine = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel2.BaseLine / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel2.ESM = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel2.ESM / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel2.Savings = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel2.BaseLine - buildingBalanceResult.PrimaryEnergyFuelTable.Fuel2.ESM;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel3.Ref1 = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel3.Ref1 / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel3.Ref2 = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel3.Ref2 / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel3.Actual = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel3.Actual / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel3.BaseLine = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel3.BaseLine / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel3.ESM = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel3.ESM / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel3.Savings = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel3.BaseLine - buildingBalanceResult.PrimaryEnergyFuelTable.Fuel3.ESM;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel4.Ref1 = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel4.Ref1 / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel4.Ref2 = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel4.Ref2 / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel4.Actual = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel4.Actual / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel4.BaseLine = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel4.BaseLine / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel4.ESM = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel4.ESM / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel4.Savings = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel4.BaseLine - buildingBalanceResult.PrimaryEnergyFuelTable.Fuel4.ESM;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel5.Ref1 = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel5.Ref1 / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel5.Ref2 = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel5.Ref2 / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel5.Actual = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel5.Actual / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel5.BaseLine = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel5.BaseLine / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel5.ESM = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel5.ESM / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel5.Savings = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel5.BaseLine - buildingBalanceResult.PrimaryEnergyFuelTable.Fuel5.ESM;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel6.Ref1 = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel6.Ref1 / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel6.Ref2 = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel6.Ref2 / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel6.Actual = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel6.Actual / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel6.BaseLine = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel6.BaseLine / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel6.ESM = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel6.ESM / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel6.Savings = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel6.BaseLine - buildingBalanceResult.PrimaryEnergyFuelTable.Fuel6.ESM;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel7.Ref1 = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel7.Ref1 / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel7.Ref2 = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel7.Ref2 / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel7.Actual = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel7.Actual / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel7.BaseLine = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel7.BaseLine / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel7.ESM = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel7.ESM / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel7.Savings = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel7.BaseLine - buildingBalanceResult.PrimaryEnergyFuelTable.Fuel7.ESM;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel8.Ref1 = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel8.Ref1 / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel8.Ref2 = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel8.Ref2 / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel8.Actual = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel8.Actual / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel8.BaseLine = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel8.BaseLine / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel8.ESM = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel8.ESM / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel8.Savings = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel8.BaseLine - buildingBalanceResult.PrimaryEnergyFuelTable.Fuel8.ESM;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel9.Ref1 = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel9.Ref1 / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel9.Ref2 = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel9.Ref2 / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel9.Actual = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel9.Actual / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel9.BaseLine = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel9.BaseLine / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel9.ESM = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel9.ESM / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel9.Savings = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel9.BaseLine - buildingBalanceResult.PrimaryEnergyFuelTable.Fuel9.ESM;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel10.Ref1 = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel10.Ref1 / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel10.Ref2 = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel10.Ref2 / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel10.Actual = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel10.Actual / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel10.BaseLine = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel10.BaseLine / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel10.ESM = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel10.ESM / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel10.Savings = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel10.BaseLine - buildingBalanceResult.PrimaryEnergyFuelTable.Fuel10.ESM;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel11.Ref1 = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel11.Ref1 / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel11.Ref2 = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel11.Ref2 / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel11.Actual = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel11.Actual / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel11.BaseLine = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel11.BaseLine / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel11.ESM = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel11.ESM / area;
		buildingBalanceResult.PrimaryEnergyFuelTable.Fuel11.Savings = buildingBalanceResult.PrimaryEnergyFuelTable.Fuel11.BaseLine - buildingBalanceResult.PrimaryEnergyFuelTable.Fuel11.ESM;
	}

	private static void CalculatePrimaryTotalEnergy(Results buildingBalanceResult)
	{
		CalculateTotalPrimaryRef1(buildingBalanceResult);
		CalculateTotalPrimaryRef2(buildingBalanceResult);
		CalculateTotalPrimaryActual(buildingBalanceResult);
		CalculateTotalPrimaryBaseLine(buildingBalanceResult);
		CalculateTotalPrimaryEsm(buildingBalanceResult);
	}

	private static void CalculateTotalPrimaryRef1(Results buildingBalanceResult)
	{
		buildingBalanceResult.PrimaryEnergyTable.Total.Ref1 = buildingBalanceResult.PrimaryEnergyTable.Heating.Ref1 + buildingBalanceResult.PrimaryEnergyTable.Cooling.Ref1 + buildingBalanceResult.PrimaryEnergyTable.HeatingVentilation.Ref1 + buildingBalanceResult.PrimaryEnergyTable.CoolingVentilation.Ref1 + buildingBalanceResult.PrimaryEnergyTable.BGV.Ref1 + buildingBalanceResult.PrimaryEnergyTable.BGVPumps.Ref1 + buildingBalanceResult.PrimaryEnergyTable.FansAndPumps.Ref1 + buildingBalanceResult.PrimaryEnergyTable.Lights.Ref1 + buildingBalanceResult.PrimaryEnergyTable.HeatAffectingDevices.Ref1 + buildingBalanceResult.PrimaryEnergyTable.NonHeatAffectingDevices.Ref1 + buildingBalanceResult.PrimaryEnergyTable.Other.Ref1;
	}

	private static void CalculateTotalPrimaryRef2(Results buildingBalanceResult)
	{
		buildingBalanceResult.PrimaryEnergyTable.Total.Ref2 = buildingBalanceResult.PrimaryEnergyTable.Heating.Ref2 + buildingBalanceResult.PrimaryEnergyTable.Cooling.Ref2 + buildingBalanceResult.PrimaryEnergyTable.HeatingVentilation.Ref2 + buildingBalanceResult.PrimaryEnergyTable.CoolingVentilation.Ref2 + buildingBalanceResult.PrimaryEnergyTable.BGV.Ref2 + buildingBalanceResult.PrimaryEnergyTable.BGVPumps.Ref2 + buildingBalanceResult.PrimaryEnergyTable.FansAndPumps.Ref2 + buildingBalanceResult.PrimaryEnergyTable.Lights.Ref2 + buildingBalanceResult.PrimaryEnergyTable.HeatAffectingDevices.Ref2 + buildingBalanceResult.PrimaryEnergyTable.NonHeatAffectingDevices.Ref2 + buildingBalanceResult.PrimaryEnergyTable.Other.Ref2;
	}

	private static void CalculateTotalPrimaryEsm(Results buildingBalanceResult)
	{
		buildingBalanceResult.PrimaryEnergyTable.Total.ESM = buildingBalanceResult.PrimaryEnergyTable.Heating.ESM + buildingBalanceResult.PrimaryEnergyTable.Cooling.ESM + buildingBalanceResult.PrimaryEnergyTable.HeatingVentilation.ESM + buildingBalanceResult.PrimaryEnergyTable.CoolingVentilation.ESM + buildingBalanceResult.PrimaryEnergyTable.BGV.ESM + buildingBalanceResult.PrimaryEnergyTable.BGVPumps.ESM + buildingBalanceResult.PrimaryEnergyTable.FansAndPumps.ESM + buildingBalanceResult.PrimaryEnergyTable.Lights.ESM + buildingBalanceResult.PrimaryEnergyTable.HeatAffectingDevices.ESM + buildingBalanceResult.PrimaryEnergyTable.NonHeatAffectingDevices.ESM + buildingBalanceResult.PrimaryEnergyTable.Other.ESM;
	}

	private static void CalculateTotalPrimaryBaseLine(Results buildingBalanceResult)
	{
		buildingBalanceResult.PrimaryEnergyTable.Total.BaseLine = buildingBalanceResult.PrimaryEnergyTable.Heating.BaseLine + buildingBalanceResult.PrimaryEnergyTable.Cooling.BaseLine + buildingBalanceResult.PrimaryEnergyTable.HeatingVentilation.BaseLine + buildingBalanceResult.PrimaryEnergyTable.CoolingVentilation.BaseLine + buildingBalanceResult.PrimaryEnergyTable.BGV.BaseLine + buildingBalanceResult.PrimaryEnergyTable.BGVPumps.BaseLine + buildingBalanceResult.PrimaryEnergyTable.FansAndPumps.BaseLine + buildingBalanceResult.PrimaryEnergyTable.Lights.BaseLine + buildingBalanceResult.PrimaryEnergyTable.HeatAffectingDevices.BaseLine + buildingBalanceResult.PrimaryEnergyTable.NonHeatAffectingDevices.BaseLine + buildingBalanceResult.PrimaryEnergyTable.Other.BaseLine;
	}

	private static void CalculateTotalPrimaryActual(Results buildingBalanceResult)
	{
		buildingBalanceResult.PrimaryEnergyTable.Total.Actual = buildingBalanceResult.PrimaryEnergyTable.Heating.Actual + buildingBalanceResult.PrimaryEnergyTable.Cooling.Actual + buildingBalanceResult.PrimaryEnergyTable.HeatingVentilation.Actual + buildingBalanceResult.PrimaryEnergyTable.CoolingVentilation.Actual + buildingBalanceResult.PrimaryEnergyTable.BGV.Actual + buildingBalanceResult.PrimaryEnergyTable.BGVPumps.Actual + buildingBalanceResult.PrimaryEnergyTable.FansAndPumps.Actual + buildingBalanceResult.PrimaryEnergyTable.Lights.Actual + buildingBalanceResult.PrimaryEnergyTable.HeatAffectingDevices.Actual + buildingBalanceResult.PrimaryEnergyTable.NonHeatAffectingDevices.Actual + buildingBalanceResult.PrimaryEnergyTable.Other.Actual;
	}

	private static void CalculateFuelSavings(Results zoneBalanceResult)
	{
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel1.Savings = zoneBalanceResult.PrimaryEnergyFuelTable.Fuel1.BaseLine - zoneBalanceResult.PrimaryEnergyFuelTable.Fuel1.ESM;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel2.Savings = zoneBalanceResult.PrimaryEnergyFuelTable.Fuel2.BaseLine - zoneBalanceResult.PrimaryEnergyFuelTable.Fuel2.ESM;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel3.Savings = zoneBalanceResult.PrimaryEnergyFuelTable.Fuel3.BaseLine - zoneBalanceResult.PrimaryEnergyFuelTable.Fuel3.ESM;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel4.Savings = zoneBalanceResult.PrimaryEnergyFuelTable.Fuel4.BaseLine - zoneBalanceResult.PrimaryEnergyFuelTable.Fuel4.ESM;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel5.Savings = zoneBalanceResult.PrimaryEnergyFuelTable.Fuel5.BaseLine - zoneBalanceResult.PrimaryEnergyFuelTable.Fuel5.ESM;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel6.Savings = zoneBalanceResult.PrimaryEnergyFuelTable.Fuel6.BaseLine - zoneBalanceResult.PrimaryEnergyFuelTable.Fuel6.ESM;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel7.Savings = zoneBalanceResult.PrimaryEnergyFuelTable.Fuel7.BaseLine - zoneBalanceResult.PrimaryEnergyFuelTable.Fuel7.ESM;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel8.Savings = zoneBalanceResult.PrimaryEnergyFuelTable.Fuel8.BaseLine - zoneBalanceResult.PrimaryEnergyFuelTable.Fuel8.ESM;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel9.Savings = zoneBalanceResult.PrimaryEnergyFuelTable.Fuel9.BaseLine - zoneBalanceResult.PrimaryEnergyFuelTable.Fuel9.ESM;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel10.Savings = zoneBalanceResult.PrimaryEnergyFuelTable.Fuel10.BaseLine - zoneBalanceResult.PrimaryEnergyFuelTable.Fuel10.ESM;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel11.Savings = zoneBalanceResult.PrimaryEnergyFuelTable.Fuel11.BaseLine - zoneBalanceResult.PrimaryEnergyFuelTable.Fuel11.ESM;
	}

	private static void GetPrimaryFuelTypeAndValues(Results zoneBalanceResult, BuildingZone zone, bool isBGVused, double totalArea, bool isFirstBuildingZone = true)
	{
		double heatedArea = zone.Heating.Area.HeatedArea;
		if (zone.HasHeating)
		{
			GetPrimaryFuelTypeRef1(zoneBalanceResult, zone.HeatingCalculations.HeatingResult.Fuel1Ref1, zone.HeatingCalculations.HeatingResult.ResultSourceEnergyRef1, heatedArea);
			GetPrimaryFuelTypeRef1(zoneBalanceResult, zone.HeatingCalculations.HeatingResult.Fuel2Ref1, zone.HeatingCalculations.HeatingResult.ResultSourceEnergy2Ref1, heatedArea);
			GetPrimaryFuelTypeRef2(zoneBalanceResult, zone.HeatingCalculations.HeatingResult.Fuel1Ref2, zone.HeatingCalculations.HeatingResult.ResultSourceEnergyRef2, heatedArea);
			GetPrimaryFuelTypeRef2(zoneBalanceResult, zone.HeatingCalculations.HeatingResult.Fuel2Ref2, zone.HeatingCalculations.HeatingResult.ResultSourceEnergy2Ref2, heatedArea);
			GetPrimaryFuelType(zoneBalanceResult, zone.HeatingCalculations.HeatingResult.Fuel1Actual, zone.HeatingCalculations.HeatingResult.ResultSourceEnergyActual, heatedArea);
			GetPrimaryFuelType(zoneBalanceResult, zone.HeatingCalculations.HeatingResult.Fuel2Actual, zone.HeatingCalculations.HeatingResult.ResultSourceEnergy2Actual, heatedArea);
			GetPrimaryFuelTypeBaseLine(zoneBalanceResult, zone.HeatingCalculations.HeatingResult.Fuel1BaseLine, zone.HeatingCalculations.HeatingResult.ResultSourceEnergyBaseLine, heatedArea);
			GetPrimaryFuelTypeBaseLine(zoneBalanceResult, zone.HeatingCalculations.HeatingResult.Fuel2BaseLine, zone.HeatingCalculations.HeatingResult.ResultSourceEnergy2BaseLine, heatedArea);
			GetPrimaryFuelTypeEsm(zoneBalanceResult, zone.HeatingCalculations.HeatingResult.Fuel1ESM, zone.HeatingCalculations.HeatingResult.ResultSourceEnergyESM, heatedArea);
			GetPrimaryFuelTypeEsm(zoneBalanceResult, zone.HeatingCalculations.HeatingResult.Fuel2ESM, zone.HeatingCalculations.HeatingResult.ResultSourceEnergy2ESM, heatedArea);
		}
		if (zone.HasCooling)
		{
			GetPrimaryFuelTypeRef1(zoneBalanceResult, zone.CoolingCalculations.CoolingResult.Fuel1Ref1, zone.CoolingCalculations.CoolingResult.ResultSourceEnergyRef1, heatedArea);
			GetPrimaryFuelTypeRef1(zoneBalanceResult, zone.CoolingCalculations.CoolingResult.Fuel2Ref1, zone.CoolingCalculations.CoolingResult.ResultSourceEnergy2Ref1, heatedArea);
			GetPrimaryFuelTypeRef2(zoneBalanceResult, zone.CoolingCalculations.CoolingResult.Fuel1Ref2, zone.CoolingCalculations.CoolingResult.ResultSourceEnergyRef2, heatedArea);
			GetPrimaryFuelTypeRef2(zoneBalanceResult, zone.CoolingCalculations.CoolingResult.Fuel2Ref2, zone.CoolingCalculations.CoolingResult.ResultSourceEnergy2Ref2, heatedArea);
			GetPrimaryFuelType(zoneBalanceResult, zone.CoolingCalculations.CoolingResult.Fuel1Actual, zone.CoolingCalculations.CoolingResult.ResultSourceEnergyActual, heatedArea);
			GetPrimaryFuelType(zoneBalanceResult, zone.CoolingCalculations.CoolingResult.Fuel2Actual, zone.CoolingCalculations.CoolingResult.ResultSourceEnergy2Actual, heatedArea);
			GetPrimaryFuelTypeBaseLine(zoneBalanceResult, zone.CoolingCalculations.CoolingResult.Fuel1BaseLine, zone.CoolingCalculations.CoolingResult.ResultSourceEnergyBaseLine, heatedArea);
			GetPrimaryFuelTypeBaseLine(zoneBalanceResult, zone.CoolingCalculations.CoolingResult.Fuel2BaseLine, zone.CoolingCalculations.CoolingResult.ResultSourceEnergy2BaseLine, heatedArea);
			GetPrimaryFuelTypeEsm(zoneBalanceResult, zone.CoolingCalculations.CoolingResult.Fuel1ESM, zone.CoolingCalculations.CoolingResult.ResultSourceEnergyESM, heatedArea);
			GetPrimaryFuelTypeEsm(zoneBalanceResult, zone.CoolingCalculations.CoolingResult.Fuel2ESM, zone.CoolingCalculations.CoolingResult.ResultSourceEnergy2ESM, heatedArea);
		}
		if (zone.HasHeating)
		{
			GetPrimaryFuelTypeRef1(zoneBalanceResult, zone.HeatingCalculations.VentilationHeating.Fuel1Ref1, zone.HeatingCalculations.VentilationHeating.ResultSourceEnergyRef1, heatedArea);
			GetPrimaryFuelTypeRef1(zoneBalanceResult, zone.HeatingCalculations.VentilationHeating.Fuel2Ref1, zone.HeatingCalculations.VentilationHeating.ResultSourceEnergy2Ref1, heatedArea);
			GetPrimaryFuelTypeRef2(zoneBalanceResult, zone.HeatingCalculations.VentilationHeating.Fuel1Ref2, zone.HeatingCalculations.VentilationHeating.ResultSourceEnergyRef2, heatedArea);
			GetPrimaryFuelTypeRef2(zoneBalanceResult, zone.HeatingCalculations.VentilationHeating.Fuel2Ref2, zone.HeatingCalculations.VentilationHeating.ResultSourceEnergy2Ref2, heatedArea);
			GetPrimaryFuelType(zoneBalanceResult, zone.HeatingCalculations.VentilationHeating.Fuel1Actual, zone.HeatingCalculations.VentilationHeating.ResultSourceEnergyActual, heatedArea);
			GetPrimaryFuelType(zoneBalanceResult, zone.HeatingCalculations.VentilationHeating.Fuel2Actual, zone.HeatingCalculations.VentilationHeating.ResultSourceEnergy2Actual, heatedArea);
			GetPrimaryFuelTypeBaseLine(zoneBalanceResult, zone.HeatingCalculations.VentilationHeating.Fuel1BaseLine, zone.HeatingCalculations.VentilationHeating.ResultSourceEnergyBaseLine, heatedArea);
			GetPrimaryFuelTypeBaseLine(zoneBalanceResult, zone.HeatingCalculations.VentilationHeating.Fuel2BaseLine, zone.HeatingCalculations.VentilationHeating.ResultSourceEnergy2BaseLine, heatedArea);
			GetPrimaryFuelTypeEsm(zoneBalanceResult, zone.HeatingCalculations.VentilationHeating.Fuel1ESM, zone.HeatingCalculations.VentilationHeating.ResultSourceEnergyESM, heatedArea);
			GetPrimaryFuelTypeEsm(zoneBalanceResult, zone.HeatingCalculations.VentilationHeating.Fuel2ESM, zone.HeatingCalculations.VentilationHeating.ResultSourceEnergy2ESM, heatedArea);
		}
		if (zone.HasCooling)
		{
			GetPrimaryFuelTypeRef1(zoneBalanceResult, zone.CoolingCalculations.VentilationCooling.Fuel1Ref1, zone.CoolingCalculations.VentilationCooling.ResultSourceEnergyRef1, heatedArea);
			GetPrimaryFuelTypeRef1(zoneBalanceResult, zone.CoolingCalculations.VentilationCooling.Fuel2Ref1, zone.CoolingCalculations.VentilationCooling.ResultSourceEnergy2Ref1, heatedArea);
			GetPrimaryFuelTypeRef2(zoneBalanceResult, zone.CoolingCalculations.VentilationCooling.Fuel1Ref2, zone.CoolingCalculations.VentilationCooling.ResultSourceEnergyRef2, heatedArea);
			GetPrimaryFuelTypeRef2(zoneBalanceResult, zone.CoolingCalculations.VentilationCooling.Fuel2Ref2, zone.CoolingCalculations.VentilationCooling.ResultSourceEnergy2Ref2, heatedArea);
			GetPrimaryFuelType(zoneBalanceResult, zone.CoolingCalculations.VentilationCooling.Fuel1Actual, zone.CoolingCalculations.VentilationCooling.ResultSourceEnergyActual, heatedArea);
			GetPrimaryFuelType(zoneBalanceResult, zone.CoolingCalculations.VentilationCooling.Fuel2Actual, zone.CoolingCalculations.VentilationCooling.ResultSourceEnergy2Actual, heatedArea);
			GetPrimaryFuelTypeBaseLine(zoneBalanceResult, zone.CoolingCalculations.VentilationCooling.Fuel1BaseLine, zone.CoolingCalculations.VentilationCooling.ResultSourceEnergyBaseLine, heatedArea);
			GetPrimaryFuelTypeBaseLine(zoneBalanceResult, zone.CoolingCalculations.VentilationCooling.Fuel2BaseLine, zone.CoolingCalculations.VentilationCooling.ResultSourceEnergy2BaseLine, heatedArea);
			GetPrimaryFuelTypeEsm(zoneBalanceResult, zone.CoolingCalculations.VentilationCooling.Fuel1ESM, zone.CoolingCalculations.VentilationCooling.ResultSourceEnergyESM, heatedArea);
			GetPrimaryFuelTypeEsm(zoneBalanceResult, zone.CoolingCalculations.VentilationCooling.Fuel2ESM, zone.CoolingCalculations.VentilationCooling.ResultSourceEnergy2ESM, heatedArea);
		}
		GetPrimaryFuelTypeRef1(zoneBalanceResult, Fuel.Fuel1, zone.HeatingCalculations.FansAndPumps.CoolNeededEnergyRef1 + zone.HeatingCalculations.FansAndPumps.PumpNeededEnergyRef1, heatedArea);
		GetPrimaryFuelTypeRef2(zoneBalanceResult, Fuel.Fuel1, zone.HeatingCalculations.FansAndPumps.CoolNeededEnergyRef2 + zone.HeatingCalculations.FansAndPumps.PumpNeededEnergyRef2, heatedArea);
		GetPrimaryFuelType(zoneBalanceResult, Fuel.Fuel1, zone.HeatingCalculations.FansAndPumps.CoolNeededEnergyActual + zone.HeatingCalculations.FansAndPumps.PumpNeededEnergyActual, heatedArea);
		GetPrimaryFuelTypeBaseLine(zoneBalanceResult, Fuel.Fuel1, zone.HeatingCalculations.FansAndPumps.CoolNeededEnergyBaseLine + zone.HeatingCalculations.FansAndPumps.PumpNeededEnergyBaseLine, heatedArea);
		GetPrimaryFuelTypeEsm(zoneBalanceResult, Fuel.Fuel1, zone.HeatingCalculations.FansAndPumps.CoolNeededEnergyESM + zone.HeatingCalculations.FansAndPumps.PumpNeededEnergyESM, heatedArea);
		GetPrimaryFuelTypeRef1(zoneBalanceResult, Fuel.Fuel1, zone.HeatingCalculations.LightAndDevices.Lights.General.DevicesNeededEnergyRef1, heatedArea);
		GetPrimaryFuelTypeRef2(zoneBalanceResult, Fuel.Fuel1, zone.HeatingCalculations.LightAndDevices.Lights.General.DevicesNeededEnergyRef2, heatedArea);
		GetPrimaryFuelType(zoneBalanceResult, Fuel.Fuel1, zone.HeatingCalculations.LightAndDevices.Lights.General.DevicesNeededEnergyActual, heatedArea);
		GetPrimaryFuelTypeBaseLine(zoneBalanceResult, Fuel.Fuel1, zone.HeatingCalculations.LightAndDevices.Lights.General.DevicesNeededEnergyBaseLine, heatedArea);
		GetPrimaryFuelTypeEsm(zoneBalanceResult, Fuel.Fuel1, zone.HeatingCalculations.LightAndDevices.Lights.General.DevicesNeededEnergyESM, heatedArea);
		GetPrimaryFuelTypeRef1(zoneBalanceResult, Fuel.Fuel1, zone.HeatingCalculations.LightAndDevices.BalancedDevices.General.DevicesNeededEnergyRef1, heatedArea);
		GetPrimaryFuelTypeRef2(zoneBalanceResult, Fuel.Fuel1, zone.HeatingCalculations.LightAndDevices.BalancedDevices.General.DevicesNeededEnergyRef2, heatedArea);
		GetPrimaryFuelType(zoneBalanceResult, Fuel.Fuel1, zone.HeatingCalculations.LightAndDevices.BalancedDevices.General.DevicesNeededEnergyActual, heatedArea);
		GetPrimaryFuelTypeBaseLine(zoneBalanceResult, Fuel.Fuel1, zone.HeatingCalculations.LightAndDevices.BalancedDevices.General.DevicesNeededEnergyBaseLine, heatedArea);
		GetPrimaryFuelTypeEsm(zoneBalanceResult, Fuel.Fuel1, zone.HeatingCalculations.LightAndDevices.BalancedDevices.General.DevicesNeededEnergyESM, heatedArea);
		GetPrimaryFuelTypeRef1(zoneBalanceResult, Fuel.Fuel1, zone.HeatingCalculations.LightAndDevices.NonBalancedDevices.General.DevicesNeededEnergyRef1, heatedArea);
		GetPrimaryFuelTypeRef2(zoneBalanceResult, Fuel.Fuel1, zone.HeatingCalculations.LightAndDevices.NonBalancedDevices.General.DevicesNeededEnergyRef2, heatedArea);
		GetPrimaryFuelType(zoneBalanceResult, Fuel.Fuel1, zone.HeatingCalculations.LightAndDevices.NonBalancedDevices.General.DevicesNeededEnergyActual, heatedArea);
		GetPrimaryFuelTypeBaseLine(zoneBalanceResult, Fuel.Fuel1, zone.HeatingCalculations.LightAndDevices.NonBalancedDevices.General.DevicesNeededEnergyBaseLine, heatedArea);
		GetPrimaryFuelTypeEsm(zoneBalanceResult, Fuel.Fuel1, zone.HeatingCalculations.LightAndDevices.NonBalancedDevices.General.DevicesNeededEnergyESM, heatedArea);
		GetPrimaryFuelTypeRef1(zoneBalanceResult, Fuel.Fuel1, zone.HeatingCalculations.FansAndPumps.OtherResultCoolingRef1, heatedArea);
		GetPrimaryFuelTypeRef2(zoneBalanceResult, Fuel.Fuel1, zone.HeatingCalculations.FansAndPumps.OtherResultCoolingRef2, heatedArea);
		GetPrimaryFuelType(zoneBalanceResult, Fuel.Fuel1, zone.HeatingCalculations.FansAndPumps.OtherResultCoolingActual, heatedArea);
		GetPrimaryFuelTypeBaseLine(zoneBalanceResult, Fuel.Fuel1, zone.HeatingCalculations.FansAndPumps.OtherResultCoolingBaseLine, heatedArea);
		GetPrimaryFuelTypeEsm(zoneBalanceResult, Fuel.Fuel1, zone.HeatingCalculations.FansAndPumps.OtherResultCoolingESM, heatedArea);
		if (isBGVused)
		{
			if (isFirstBuildingZone)
			{
				GetPrimaryFuelTypeRef1(zoneBalanceResult, zone.HeatingCalculations.HotWaterCalculations.Fuel1Ref1, zone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergyRef1, totalArea);
				GetPrimaryFuelTypeRef1(zoneBalanceResult, zone.HeatingCalculations.HotWaterCalculations.Fuel2Ref1, zone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergy2Ref1, totalArea);
				GetPrimaryFuelTypeRef2(zoneBalanceResult, zone.HeatingCalculations.HotWaterCalculations.Fuel1Ref2, zone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergyRef2, totalArea);
				GetPrimaryFuelTypeRef2(zoneBalanceResult, zone.HeatingCalculations.HotWaterCalculations.Fuel2Ref2, zone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergy2Ref2, totalArea);
				GetPrimaryFuelType(zoneBalanceResult, zone.HeatingCalculations.HotWaterCalculations.Fuel1Actual, zone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergyActual, totalArea);
				GetPrimaryFuelType(zoneBalanceResult, zone.HeatingCalculations.HotWaterCalculations.Fuel2Actual, zone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergy2Actual, totalArea);
				GetPrimaryFuelTypeBaseLine(zoneBalanceResult, zone.HeatingCalculations.HotWaterCalculations.Fuel1BaseLine, zone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergyBaseLine, totalArea);
				GetPrimaryFuelTypeBaseLine(zoneBalanceResult, zone.HeatingCalculations.HotWaterCalculations.Fuel2BaseLine, zone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergy2BaseLine, totalArea);
				GetPrimaryFuelTypeEsm(zoneBalanceResult, zone.HeatingCalculations.HotWaterCalculations.Fuel1ESM, zone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergyESM, totalArea);
				GetPrimaryFuelTypeEsm(zoneBalanceResult, zone.HeatingCalculations.HotWaterCalculations.Fuel2ESM, zone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergy2ESM, totalArea);
				GetPrimaryFuelTypeRef1(zoneBalanceResult, Fuel.Fuel1, zoneBalanceResult.NeededEnergyTable.BGVPumps.Ref1Area, totalArea);
				GetPrimaryFuelTypeRef2(zoneBalanceResult, Fuel.Fuel1, zoneBalanceResult.NeededEnergyTable.BGVPumps.Ref2Area, totalArea);
				GetPrimaryFuelType(zoneBalanceResult, Fuel.Fuel1, zoneBalanceResult.NeededEnergyTable.BGVPumps.ActualArea, totalArea);
				GetPrimaryFuelTypeBaseLine(zoneBalanceResult, Fuel.Fuel1, zoneBalanceResult.NeededEnergyTable.BGVPumps.BaseLineArea, totalArea);
				GetPrimaryFuelTypeEsm(zoneBalanceResult, Fuel.Fuel1, zoneBalanceResult.NeededEnergyTable.BGVPumps.ESMArea, totalArea);
			}
		}
		else
		{
			zoneBalanceResult.PrimaryEnergyTable.BGV.Ref1 = 0.0;
			zoneBalanceResult.PrimaryEnergyTable.BGV.Ref2 = 0.0;
			zoneBalanceResult.PrimaryEnergyTable.BGV.Actual = 0.0;
			zoneBalanceResult.PrimaryEnergyTable.BGV.BaseLine = 0.0;
			zoneBalanceResult.PrimaryEnergyTable.BGV.ESM = 0.0;
		}
		CalculateFuelSavings(zoneBalanceResult);
	}

	private static void GetPrimaryFuelTypeRef1(Results zoneBalanceResult, Fuel fuel, double quantity, double area)
	{
		if (double.IsInfinity(quantity) || double.IsNaN(quantity))
		{
			quantity = 0.0;
		}
		switch (fuel)
		{
		case Fuel.Fuel1:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel8.Ref1 += area * quantity * 3.0;
			break;
		case Fuel.Fuel2:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel2.Ref1 += area * quantity * 1.1;
			break;
		case Fuel.Fuel3:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel3.Ref1 += area * quantity * 1.1;
			break;
		case Fuel.Fuel4:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel4.Ref1 += area * quantity * 1.2;
			break;
		case Fuel.Fuel5:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel5.Ref1 += area * quantity * 1.2;
			break;
		case Fuel.Fuel6:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel6.Ref1 += area * quantity * 1.05;
			break;
		case Fuel.Fuel7:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel7.Ref1 += area * quantity * 1.25;
			break;
		case Fuel.Fuel8:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel1.Ref1 += area * quantity * 1.1;
			break;
		case Fuel.Fuel9:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel9.Ref1 += area * quantity * 1.3;
			break;
		case Fuel.Fuel10:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel10.Ref1 += area * quantity * 1.1;
			break;
		case Fuel.Fuel11:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel11.Ref1 += area * quantity * 1.2;
			break;
		}
	}

	private static void GetPrimaryFuelTypeRef2(Results zoneBalanceResult, Fuel fuel, double quantity, double area)
	{
		switch (fuel)
		{
		case Fuel.Fuel1:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel8.Ref2 += area * quantity * 3.0;
			break;
		case Fuel.Fuel2:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel2.Ref2 += area * quantity * 1.1;
			break;
		case Fuel.Fuel3:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel3.Ref2 += area * quantity * 1.1;
			break;
		case Fuel.Fuel4:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel4.Ref2 += area * quantity * 1.2;
			break;
		case Fuel.Fuel5:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel5.Ref2 += area * quantity * 1.2;
			break;
		case Fuel.Fuel6:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel6.Ref2 += area * quantity * 1.05;
			break;
		case Fuel.Fuel7:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel7.Ref2 += area * quantity * 1.25;
			break;
		case Fuel.Fuel8:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel1.Ref2 += area * quantity * 1.1;
			break;
		case Fuel.Fuel9:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel9.Ref2 += area * quantity * 1.3;
			break;
		case Fuel.Fuel10:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel10.Ref2 += area * quantity * 1.1;
			break;
		case Fuel.Fuel11:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel11.Ref2 += area * quantity * 1.2;
			break;
		}
	}

	private static void GetPrimaryFuelType(Results zoneBalanceResult, Fuel fuel, double quantity, double area)
	{
		if (double.IsInfinity(quantity) || double.IsNaN(quantity))
		{
			quantity = 0.0;
		}
		switch (fuel)
		{
		case Fuel.Fuel1:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel8.Actual += area * quantity * 3.0;
			break;
		case Fuel.Fuel2:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel2.Actual += area * quantity * 1.1;
			break;
		case Fuel.Fuel3:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel3.Actual += area * quantity * 1.1;
			break;
		case Fuel.Fuel4:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel4.Actual += area * quantity * 1.2;
			break;
		case Fuel.Fuel5:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel5.Actual += area * quantity * 1.2;
			break;
		case Fuel.Fuel6:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel6.Actual += area * quantity * 1.05;
			break;
		case Fuel.Fuel7:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel7.Actual += area * quantity * 1.25;
			break;
		case Fuel.Fuel8:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel1.Actual += area * quantity * 1.1;
			break;
		case Fuel.Fuel9:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel9.Actual += area * quantity * 1.3;
			break;
		case Fuel.Fuel10:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel10.Actual += area * quantity * 1.1;
			break;
		case Fuel.Fuel11:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel11.Actual += area * quantity * 1.2;
			break;
		}
	}

	private static void GetPrimaryFuelTypeBaseLine(Results zoneBalanceResult, Fuel fuel, double quantity, double area)
	{
		if (double.IsInfinity(quantity) || double.IsNaN(quantity))
		{
			quantity = 0.0;
		}
		switch (fuel)
		{
		case Fuel.Fuel1:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel8.BaseLine += area * quantity * 3.0;
			break;
		case Fuel.Fuel2:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel2.BaseLine += area * quantity * 1.1;
			break;
		case Fuel.Fuel3:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel3.BaseLine += area * quantity * 1.1;
			break;
		case Fuel.Fuel4:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel4.BaseLine += area * quantity * 1.2;
			break;
		case Fuel.Fuel5:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel5.BaseLine += area * quantity * 1.2;
			break;
		case Fuel.Fuel6:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel6.BaseLine += area * quantity * 1.05;
			break;
		case Fuel.Fuel7:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel7.BaseLine += area * quantity * 1.25;
			break;
		case Fuel.Fuel8:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel1.BaseLine += area * quantity * 1.1;
			break;
		case Fuel.Fuel9:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel9.BaseLine += area * quantity * 1.3;
			break;
		case Fuel.Fuel10:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel10.BaseLine += area * quantity * 1.1;
			break;
		case Fuel.Fuel11:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel11.BaseLine += area * quantity * 1.2;
			break;
		}
	}

	private static void GetPrimaryFuelTypeEsm(Results zoneBalanceResult, Fuel fuel, double quantity, double area)
	{
		if (double.IsInfinity(quantity) || double.IsNaN(quantity))
		{
			quantity = 0.0;
		}
		switch (fuel)
		{
		case Fuel.Fuel1:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel8.ESM += area * quantity * 3.0;
			break;
		case Fuel.Fuel2:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel2.ESM += area * quantity * 1.1;
			break;
		case Fuel.Fuel3:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel3.ESM += area * quantity * 1.1;
			break;
		case Fuel.Fuel4:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel4.ESM += area * quantity * 1.2;
			break;
		case Fuel.Fuel5:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel5.ESM += area * quantity * 1.2;
			break;
		case Fuel.Fuel6:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel6.ESM += area * quantity * 1.05;
			break;
		case Fuel.Fuel7:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel7.ESM += area * quantity * 1.25;
			break;
		case Fuel.Fuel8:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel1.ESM += area * quantity * 1.1;
			break;
		case Fuel.Fuel9:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel9.ESM += area * quantity * 1.3;
			break;
		case Fuel.Fuel10:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel10.ESM += area * quantity * 1.1;
			break;
		case Fuel.Fuel11:
			zoneBalanceResult.PrimaryEnergyFuelTable.Fuel11.ESM += area * quantity * 1.2;
			break;
		}
	}

	private static void CalculatePrimaryEnergyFuelTotal(Results zoneBalanceResult)
	{
		CalculateTotalPrimaryFuelRef1(zoneBalanceResult);
		CalculateTotalPrimaryFuelRef2(zoneBalanceResult);
		CalculateTotalPrimaryFuelActual(zoneBalanceResult);
		CalculateTotalPrimaryFuelBaseLine(zoneBalanceResult);
		CalculateTotalPrimaryFuelESM(zoneBalanceResult);
		CalculateTotalPrimaryFuelSavings(zoneBalanceResult);
	}

	private static void CalculateTotalPrimaryFuelRef1(Results zoneBalanceResult)
	{
		zoneBalanceResult.PrimaryEnergyFuelTable.Total.Ref1 = zoneBalanceResult.PrimaryEnergyFuelTable.Fuel1.Ref1 + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel2.Ref1 + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel3.Ref1 + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel4.Ref1 + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel5.Ref1 + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel6.Ref1 + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel7.Ref1 + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel8.Ref1 + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel9.Ref1 + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel10.Ref1 + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel11.Ref1;
	}

	private static void CalculateTotalPrimaryFuelRef2(Results zoneBalanceResult)
	{
		zoneBalanceResult.PrimaryEnergyFuelTable.Total.Ref2 = zoneBalanceResult.PrimaryEnergyFuelTable.Fuel1.Ref2 + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel2.Ref2 + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel3.Ref2 + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel4.Ref2 + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel5.Ref2 + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel6.Ref2 + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel7.Ref2 + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel8.Ref2 + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel9.Ref2 + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel10.Ref2 + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel11.Ref2;
	}

	private static void CalculateTotalPrimaryFuelActual(Results zoneBalanceResult)
	{
		zoneBalanceResult.PrimaryEnergyFuelTable.Total.Actual = zoneBalanceResult.PrimaryEnergyFuelTable.Fuel1.Actual + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel2.Actual + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel3.Actual + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel4.Actual + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel5.Actual + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel6.Actual + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel7.Actual + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel8.Actual + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel9.Actual + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel10.Actual + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel11.Actual;
	}

	private static void CalculateTotalPrimaryFuelBaseLine(Results zoneBalanceResult)
	{
		zoneBalanceResult.PrimaryEnergyFuelTable.Total.BaseLine = zoneBalanceResult.PrimaryEnergyFuelTable.Fuel1.BaseLine + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel2.BaseLine + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel3.BaseLine + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel4.BaseLine + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel5.BaseLine + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel6.BaseLine + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel7.BaseLine + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel8.BaseLine + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel9.BaseLine + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel10.BaseLine + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel11.BaseLine;
	}

	private static void CalculateTotalPrimaryFuelESM(Results zoneBalanceResult)
	{
		zoneBalanceResult.PrimaryEnergyFuelTable.Total.ESM = zoneBalanceResult.PrimaryEnergyFuelTable.Fuel1.ESM + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel2.ESM + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel3.ESM + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel4.ESM + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel5.ESM + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel6.ESM + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel7.ESM + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel8.ESM + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel9.ESM + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel10.ESM + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel11.ESM;
	}

	private static void CalculateTotalPrimaryFuelSavings(Results zoneBalanceResult)
	{
		zoneBalanceResult.PrimaryEnergyFuelTable.Total.Savings = zoneBalanceResult.PrimaryEnergyFuelTable.Fuel1.Savings + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel2.Savings + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel3.Savings + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel4.Savings + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel5.Savings + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel6.Savings + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel7.Savings + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel8.Savings + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel9.Savings + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel10.Savings + zoneBalanceResult.PrimaryEnergyFuelTable.Fuel11.Savings;
	}

	private static double GetPrimaryEnergyCoeficient(Fuel fuel, double quantity)
	{
		if (double.IsNaN(quantity) || double.IsInfinity(quantity))
		{
			quantity = 0.0;
		}
		return fuel switch
		{
			Fuel.Fuel1 => quantity * 3.0, 
			Fuel.Fuel2 => quantity * 1.1, 
			Fuel.Fuel3 => quantity * 1.1, 
			Fuel.Fuel4 => quantity * 1.2, 
			Fuel.Fuel5 => quantity * 1.2, 
			Fuel.Fuel6 => quantity * 1.05, 
			Fuel.Fuel7 => quantity * 1.25, 
			Fuel.Fuel8 => quantity * 1.1, 
			Fuel.Fuel9 => quantity * 1.3, 
			Fuel.Fuel10 => quantity * 1.1, 
			Fuel.Fuel11 => quantity * 1.2, 
			_ => 0.0, 
		};
	}

	public static void BuildingCalculations(this Results buildingBalanceResult, CalculationInput calcInput, Results zoneBalanceResult)
	{
		GetBuildingData(buildingBalanceResult, calcInput);
		GetConditionedArea(calcInput, buildingBalanceResult);
		ClearNeededVEIenergy(buildingBalanceResult);
		UpdateRefsState(buildingBalanceResult, calcInput);
		UpdateActualState(buildingBalanceResult, calcInput);
		UpdateBaseLineState(buildingBalanceResult, calcInput);
		UpdateEsmState(buildingBalanceResult, calcInput);
		CalculateTotalsNeededEnergyTable(buildingBalanceResult, isBGVused: true);
		ClearFuelCells(buildingBalanceResult);
		ClearNetEnergy(buildingBalanceResult);
		ClearNetEnergyWithoutInputs(buildingBalanceResult);
		ClearPrimaryEnergy(buildingBalanceResult);
		ClearPrimaryEnergyFuelTableValues(buildingBalanceResult);
		double totalHeatedArea = buildingBalanceResult.TotalAreaElements.TotalHeatedArea;
		bool isFirstBuildingZone = true;
		foreach (BuildingZone buildingZone in calcInput.BuildingZones)
		{
			CalculateNetEnergyByTechnologiesBuilding(buildingBalanceResult, buildingZone);
			CalculateNetWithoutInputsEnergyByTechnologies(buildingBalanceResult, buildingZone);
			CalculatePrimaryEnergyByTechnologies(buildingBalanceResult, buildingZone, isBGVused: true, totalHeatedArea, isFirstBuildingZone);
			GetPrimaryFuelTypeAndValues(buildingBalanceResult, buildingZone, isBGVused: true, totalHeatedArea, isFirstBuildingZone);
			GetFuelTypeAndValues(buildingBalanceResult, buildingZone, isBGVused: true, buildingZone.Heating.Area.HeatedArea, totalHeatedArea, isFirstBuildingZone);
			isFirstBuildingZone = false;
		}
		buildingBalanceResult.FuelEnergyTable.Fuel8.ActualArea += calcInput.BuildingZones.First().HeatingCalculations.HotWaterCalculations.SunEnergyCalculations.Actual.SunEnergyResTable.BGVPumpsTotal;
		buildingBalanceResult.FuelEnergyTable.Fuel8.BaseLineArea += calcInput.BuildingZones.First().HeatingCalculations.HotWaterCalculations.SunEnergyCalculations.BaseLine.SunEnergyResTable.BGVPumpsTotal;
		buildingBalanceResult.FuelEnergyTable.Fuel8.ESMArea += calcInput.BuildingZones.First().HeatingCalculations.HotWaterCalculations.SunEnergyCalculations.ESM.SunEnergyResTable.BGVPumpsTotal;
		SetFuelValue(buildingBalanceResult, totalHeatedArea);
		buildingBalanceResult.CalculateNetEnergyPerArea();
		buildingBalanceResult.CalculateNetWithoutInputsEnergyByTechnologiesPerArea();
		CalculatePrimaryEnergyPerArea(buildingBalanceResult, totalHeatedArea, isBGVused: true);
		CalculatePrimaryFuelTypeAndValuesPerArea(buildingBalanceResult, totalHeatedArea);
		buildingBalanceResult.BuildingCO2Calculations(calcInput);
		CalculateTotalFuelEnergy(buildingBalanceResult);
		CalculatePrimaryEnergyFuelTotal(buildingBalanceResult);
		CalculatePrimaryTotalEnergy(buildingBalanceResult);
		CalculateBuildingPowerEnergy(calcInput, buildingBalanceResult);
		CalculateTotalVei(buildingBalanceResult, isBGVused: true);
		SetScaleValues(calcInput);
	}

	public static void ZoneCalculations(this Results zoneBalanceResult, CalculationInput calcInput, BuildingZone zone)
	{
		zoneBalanceResult.CalculateZonePowerEnergy(zone.Heating, zone, zoneBalanceResult);
		zoneBalanceResult.ZoneCO2Calculations(zone, isBGVused: false);
		ClearFuelCells(zoneBalanceResult);
		ClearNeededVEIenergy(zoneBalanceResult);
		GetFuelTypeAndValues(zoneBalanceResult, zone, isBGVused: false, 1.0, 1.0);
		CalculateTotalFuelEnergy(zoneBalanceResult);
		zoneBalanceResult.NeededEnergyTable.ConditionedArea = zone.Heating.Area.HeatedArea;
		zoneBalanceResult.NetEnergyTable.ConditionedArea = zone.Heating.Area.HeatedArea;
		CalculateTotalsNeededEnergyTable(zoneBalanceResult, isBGVused: false);
		ClearNetEnergy(zoneBalanceResult);
		ClearNetEnergyWithoutInputs(zoneBalanceResult);
		CalculateNetEnergyByTechnologies(zoneBalanceResult, zone);
		CalculateNetWithoutInputsEnergyByTechnologies(zoneBalanceResult, zone);
		ClearPrimaryEnergy(zoneBalanceResult);
		CalculatePrimaryEnergyByTechnologies(zoneBalanceResult, zone, isBGVused: false);
		double heatedArea = zone.Heating.Area.HeatedArea;
		CalculatePrimaryEnergyPerArea(zoneBalanceResult, heatedArea, isBGVused: false);
		ClearPrimaryEnergyFuelTableValues(zoneBalanceResult);
		GetPrimaryFuelTypeAndValues(zoneBalanceResult, zone, isBGVused: false, 1.0);
		CalculatePrimaryFuelTypeAndValuesPerArea(zoneBalanceResult, heatedArea);
		CalculatePrimaryEnergyFuelTotal(zoneBalanceResult);
		CalculatePrimaryTotalEnergy(zoneBalanceResult);
		CalculateTotalVei(zoneBalanceResult, isBGVused: false);
	}

	private static void SetScaleValues(CalculationInput calcInput)
	{
		Scale climateZoneParams = BuildingTypesManager.GetClimateZoneParams(calcInput.General.InvestigationMethod);
		SetScaleType(climateZoneParams, calcInput.General.BuildingResults);
	}

	private static void BuildingCO2Calculations(this Results buildingBalanceResult, CalculationInput calcInput)
	{
		ClearValuesCO2(buildingBalanceResult);
		ClearFuelCellsCO2(buildingBalanceResult);
		foreach (BuildingZone buildingZone in calcInput.BuildingZones)
		{
			CalculateCO2Emissions(buildingBalanceResult, buildingZone, isBGVused: true);
			Co2GetFuelTypesBuilding(buildingBalanceResult, buildingZone);
		}
		buildingBalanceResult.EmissionNeededEnergyTable.BGVPumps.Ref1 += calcInput.BuildingZones.First().HeatingCalculations.HotWaterCalculations.SunEnergyCalculations.Actual.SunEnergyResTable.BGVPumpsTotal * 819.0 / 1000000.0;
		buildingBalanceResult.EmissionNeededEnergyTable.BGVPumps.Ref2 += calcInput.BuildingZones.First().HeatingCalculations.HotWaterCalculations.SunEnergyCalculations.Actual.SunEnergyResTable.BGVPumpsTotal * 819.0 / 1000000.0;
		buildingBalanceResult.EmissionNeededEnergyTable.BGVPumps.Actual += calcInput.BuildingZones.First().HeatingCalculations.HotWaterCalculations.SunEnergyCalculations.Actual.SunEnergyResTable.BGVPumpsTotal * 819.0 / 1000000.0;
		buildingBalanceResult.EmissionNeededEnergyTable.BGVPumps.BaseLine += calcInput.BuildingZones.First().HeatingCalculations.HotWaterCalculations.SunEnergyCalculations.BaseLine.SunEnergyResTable.BGVPumpsTotal * 819.0 / 1000000.0;
		buildingBalanceResult.EmissionNeededEnergyTable.BGVPumps.ESM += calcInput.BuildingZones.First().HeatingCalculations.HotWaterCalculations.SunEnergyCalculations.ESM.SunEnergyResTable.BGVPumpsTotal * 819.0 / 1000000.0;
		Co2CalculateEmissionEnergySupplyBuilding(buildingBalanceResult);
		Co2EnergyCalculateTotal(buildingBalanceResult);
		CalculateSavings(buildingBalanceResult.EmissionNeededEnergyTable);
		CalculateFuelSavings(buildingBalanceResult.EmissionEnergySupplyTable);
	}

	private static void ZoneCO2Calculations(this Results zoneBalanceResult, BuildingZone zone, bool isBGVused)
	{
		ClearFuelCellsCO2(zoneBalanceResult);
		CO2EnergyZoneCalculations(zoneBalanceResult, zone, isBGVused);
		Co2EnergyCalculateTotal(zoneBalanceResult);
		ClearValuesCO2(zoneBalanceResult);
		CalculateCO2Emissions(zoneBalanceResult, zone, isBGVused: false);
		CalculateSavings(zoneBalanceResult.EmissionNeededEnergyTable);
		CalculateFuelSavings(zoneBalanceResult.EmissionEnergySupplyTable);
	}

	private static void GetBuildingData(Results buildingBalanceResult, CalculationInput calcInput)
	{
		buildingBalanceResult.TotalAreaElements.TotalHeatedArea = calcInput.BuildingZones.Sum((BuildingZone o) => o.Heating.Area.HeatedArea) - calcInput.BuildingZones.Sum((BuildingZone o) => o.Heating.Area.OtherArea);
		buildingBalanceResult.TotalAreaElements.TotalVolume = calcInput.BuildingZones.Sum((BuildingZone o) => o.Heating.Area.HeatedVolume) - calcInput.BuildingZones.Sum((BuildingZone o) => o.Heating.Area.OtherVolume);
		buildingBalanceResult.TotalAreaElements.TotalFloorElements.Actual = calcInput.BuildingZones.Sum((BuildingZone o) => o.Heating.Area.ZoneAreaElements.TotalFloorElements.Actual);
		buildingBalanceResult.TotalAreaElements.TotalFloorElements.Esm = calcInput.BuildingZones.Sum((BuildingZone o) => o.Heating.Area.ZoneAreaElements.TotalFloorElements.Esm);
		buildingBalanceResult.TotalAreaElements.TotalOuterWalls.Actual = calcInput.BuildingZones.Sum((BuildingZone o) => o.Heating.Area.ZoneAreaElements.TotalOuterWalls.Actual);
		buildingBalanceResult.TotalAreaElements.TotalOuterWalls.Esm = calcInput.BuildingZones.Sum((BuildingZone o) => o.Heating.Area.ZoneAreaElements.TotalOuterWalls.Esm);
		buildingBalanceResult.TotalAreaElements.TotalRoofElements.Actual = calcInput.BuildingZones.Sum((BuildingZone o) => o.Heating.Area.ZoneAreaElements.TotalRoofElements.Actual);
		buildingBalanceResult.TotalAreaElements.TotalRoofElements.Esm = calcInput.BuildingZones.Sum((BuildingZone o) => o.Heating.Area.ZoneAreaElements.TotalRoofElements.Esm);
		buildingBalanceResult.TotalAreaElements.TotalWindows.Actual = calcInput.BuildingZones.Sum((BuildingZone o) => o.Heating.Area.ZoneAreaElements.TotalWindows.Actual);
		buildingBalanceResult.TotalAreaElements.TotalWindows.Esm = calcInput.BuildingZones.Sum((BuildingZone o) => o.Heating.Area.ZoneAreaElements.TotalWindows.Esm);
	}

	private static void ClearPrimaryEnergyFuelTableValues(Results zoneBalanceResult)
	{
		ClearValuesFuelRef1(zoneBalanceResult);
		ClearValuesFuelRef2(zoneBalanceResult);
		ClearValuesFuelActual(zoneBalanceResult);
		ClearValuesFuelBaseLine(zoneBalanceResult);
		ClearValuesFuelESM(zoneBalanceResult);
	}

	private static void ClearValuesFuelRef1(Results zoneBalanceResult)
	{
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel1.Ref1 = 0.0;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel2.Ref1 = 0.0;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel3.Ref1 = 0.0;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel4.Ref1 = 0.0;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel5.Ref1 = 0.0;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel6.Ref1 = 0.0;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel7.Ref1 = 0.0;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel8.Ref1 = 0.0;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel9.Ref1 = 0.0;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel10.Ref1 = 0.0;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel11.Ref1 = 0.0;
	}

	private static void ClearValuesFuelRef2(Results zoneBalanceResult)
	{
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel1.Ref2 = 0.0;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel2.Ref2 = 0.0;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel3.Ref2 = 0.0;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel4.Ref2 = 0.0;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel5.Ref2 = 0.0;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel6.Ref2 = 0.0;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel7.Ref2 = 0.0;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel8.Ref2 = 0.0;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel9.Ref2 = 0.0;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel10.Ref2 = 0.0;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel11.Ref2 = 0.0;
	}

	private static void ClearValuesFuelActual(Results zoneBalanceResult)
	{
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel1.Actual = 0.0;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel2.Actual = 0.0;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel3.Actual = 0.0;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel4.Actual = 0.0;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel5.Actual = 0.0;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel6.Actual = 0.0;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel7.Actual = 0.0;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel8.Actual = 0.0;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel9.Actual = 0.0;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel10.Actual = 0.0;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel11.Actual = 0.0;
	}

	private static void ClearValuesFuelBaseLine(Results zoneBalanceResult)
	{
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel1.BaseLine = 0.0;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel2.BaseLine = 0.0;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel3.BaseLine = 0.0;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel4.BaseLine = 0.0;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel5.BaseLine = 0.0;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel6.BaseLine = 0.0;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel7.BaseLine = 0.0;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel8.BaseLine = 0.0;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel9.BaseLine = 0.0;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel10.BaseLine = 0.0;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel11.BaseLine = 0.0;
	}

	private static void ClearValuesFuelESM(Results zoneBalanceResult)
	{
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel1.ESM = 0.0;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel2.ESM = 0.0;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel3.ESM = 0.0;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel4.ESM = 0.0;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel5.ESM = 0.0;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel6.ESM = 0.0;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel7.ESM = 0.0;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel8.ESM = 0.0;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel9.ESM = 0.0;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel10.ESM = 0.0;
		zoneBalanceResult.PrimaryEnergyFuelTable.Fuel11.ESM = 0.0;
	}

	private static void UpdateRefsState(Results buildingBalanceResult, CalculationInput calcInput)
	{
		double num = CalcTotalArea(calcInput);
		if (calcInput.BuildingZones.Count > 0)
		{
			buildingBalanceResult.NeededEnergyTable.Heating.Ref1Area = calcInput.BuildingZones.Where((BuildingZone z) => z.HasHeating).Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.Heating.Ref1) / num;
			buildingBalanceResult.NeededEnergyTable.Heating.Ref1 = calcInput.BuildingZones.Where((BuildingZone z) => z.HasHeating).Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.Heating.Ref1);
			buildingBalanceResult.NeededEnergyTable.HeatingVentilation.Ref1Area = calcInput.BuildingZones.Where((BuildingZone z) => z.HasHeating).Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.HeatingVentilation.Ref1) / num;
			buildingBalanceResult.NeededEnergyTable.HeatingVentilation.Ref1 = calcInput.BuildingZones.Where((BuildingZone z) => z.HasHeating).Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.HeatingVentilation.Ref1);
			buildingBalanceResult.NeededEnergyTable.Cooling.Ref1Area = calcInput.BuildingZones.Where((BuildingZone z) => z.HasCooling).Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.Cooling.Ref1) / num;
			buildingBalanceResult.NeededEnergyTable.Cooling.Ref1 = calcInput.BuildingZones.Where((BuildingZone z) => z.HasCooling).Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.Cooling.Ref1);
			buildingBalanceResult.NeededEnergyTable.CoolingVentilation.Ref1Area = calcInput.BuildingZones.Where((BuildingZone z) => z.HasCooling).Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.CoolingVentilation.Ref1) / num;
			buildingBalanceResult.NeededEnergyTable.CoolingVentilation.Ref1 = calcInput.BuildingZones.Where((BuildingZone z) => z.HasCooling).Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.CoolingVentilation.Ref1);
			buildingBalanceResult.NeededEnergyTable.BGV.Ref1Area = calcInput.BuildingZones.First().ZoneResults.NeededEnergyTable.BGV.Ref1 / num;
			buildingBalanceResult.NeededEnergyTable.BGV.Ref1 = calcInput.BuildingZones.First().ZoneResults.NeededEnergyTable.BGV.Ref1;
			buildingBalanceResult.NeededEnergyTable.BGVPumps.Ref1Area = (calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.BGVPumps.Ref1) + calcInput.BuildingZones.First().HeatingCalculations.HotWaterCalculations.SunEnergyCalculations.Actual.SunEnergyResTable.BGVPumpsTotal) / num;
			buildingBalanceResult.NeededEnergyTable.BGVPumps.Ref1 = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.BGVPumps.Ref1) + calcInput.BuildingZones.First().HeatingCalculations.HotWaterCalculations.SunEnergyCalculations.Actual.SunEnergyResTable.BGVPumpsTotal;
			buildingBalanceResult.NeededEnergyTable.FansAndPumps.Ref1Area = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.FansAndPumps.Ref1) / num;
			buildingBalanceResult.NeededEnergyTable.FansAndPumps.Ref1 = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.FansAndPumps.Ref1);
			buildingBalanceResult.NeededEnergyTable.Lights.Ref1Area = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.Lights.Ref1) / num;
			buildingBalanceResult.NeededEnergyTable.Lights.Ref1 = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.Lights.Ref1);
			buildingBalanceResult.NeededEnergyTable.HeatAffectingDevices.Ref1Area = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.HeatAffectingDevices.Ref1) / num;
			buildingBalanceResult.NeededEnergyTable.HeatAffectingDevices.Ref1 = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.HeatAffectingDevices.Ref1);
			buildingBalanceResult.NeededEnergyTable.NonHeatAffectingDevices.Ref1Area = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.NonHeatAffectingDevices.Ref1) / num;
			buildingBalanceResult.NeededEnergyTable.NonHeatAffectingDevices.Ref1 = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.NonHeatAffectingDevices.Ref1);
			buildingBalanceResult.NeededEnergyTable.Other.Ref1Area = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.Other.Ref1) / num;
			buildingBalanceResult.NeededEnergyTable.Other.Ref1 = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.Other.Ref1);
			buildingBalanceResult.NeededEnergyTable.Heating.Ref2Area = calcInput.BuildingZones.Where((BuildingZone z) => z.HasHeating).Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.Heating.Ref2) / num;
			buildingBalanceResult.NeededEnergyTable.Heating.Ref2 = calcInput.BuildingZones.Where((BuildingZone z) => z.HasHeating).Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.Heating.Ref2);
			buildingBalanceResult.NeededEnergyTable.HeatingVentilation.Ref2Area = calcInput.BuildingZones.Where((BuildingZone z) => z.HasHeating).Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.HeatingVentilation.Ref2) / num;
			buildingBalanceResult.NeededEnergyTable.HeatingVentilation.Ref2 = calcInput.BuildingZones.Where((BuildingZone z) => z.HasHeating).Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.HeatingVentilation.Ref2);
			buildingBalanceResult.NeededEnergyTable.Cooling.Ref2Area = calcInput.BuildingZones.Where((BuildingZone z) => z.HasCooling).Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.Cooling.Ref2) / num;
			buildingBalanceResult.NeededEnergyTable.Cooling.Ref2 = calcInput.BuildingZones.Where((BuildingZone z) => z.HasCooling).Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.Cooling.Ref2);
			buildingBalanceResult.NeededEnergyTable.CoolingVentilation.Ref2Area = calcInput.BuildingZones.Where((BuildingZone z) => z.HasCooling).Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.CoolingVentilation.Ref2) / num;
			buildingBalanceResult.NeededEnergyTable.CoolingVentilation.Ref2 = calcInput.BuildingZones.Where((BuildingZone z) => z.HasCooling).Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.CoolingVentilation.Ref2);
			buildingBalanceResult.NeededEnergyTable.BGV.Ref2Area = calcInput.BuildingZones.First().ZoneResults.NeededEnergyTable.BGV.Ref2 / num;
			buildingBalanceResult.NeededEnergyTable.BGV.Ref2 = calcInput.BuildingZones.First().ZoneResults.NeededEnergyTable.BGV.Ref2;
			buildingBalanceResult.NeededEnergyTable.BGVPumps.Ref2Area = (calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.BGVPumps.Ref2) + calcInput.BuildingZones.First().HeatingCalculations.HotWaterCalculations.SunEnergyCalculations.Actual.SunEnergyResTable.BGVPumpsTotal) / num;
			buildingBalanceResult.NeededEnergyTable.BGVPumps.Ref2 = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.BGVPumps.Ref2) + calcInput.BuildingZones.First().HeatingCalculations.HotWaterCalculations.SunEnergyCalculations.Actual.SunEnergyResTable.BGVPumpsTotal;
			buildingBalanceResult.NeededEnergyTable.FansAndPumps.Ref2Area = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.FansAndPumps.Ref2) / num;
			buildingBalanceResult.NeededEnergyTable.FansAndPumps.Ref2 = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.FansAndPumps.Ref2);
			buildingBalanceResult.NeededEnergyTable.Lights.Ref2Area = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.Lights.Ref2) / num;
			buildingBalanceResult.NeededEnergyTable.Lights.Ref2 = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.Lights.Ref2);
			buildingBalanceResult.NeededEnergyTable.HeatAffectingDevices.Ref2Area = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.HeatAffectingDevices.Ref2) / num;
			buildingBalanceResult.NeededEnergyTable.HeatAffectingDevices.Ref2 = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.HeatAffectingDevices.Ref2);
			buildingBalanceResult.NeededEnergyTable.NonHeatAffectingDevices.Ref2Area = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.NonHeatAffectingDevices.Ref2) / num;
			buildingBalanceResult.NeededEnergyTable.NonHeatAffectingDevices.Ref2 = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.NonHeatAffectingDevices.Ref2);
			buildingBalanceResult.NeededEnergyTable.Other.Ref2Area = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.Other.Ref2) / num;
			buildingBalanceResult.NeededEnergyTable.Other.Ref2 = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.Other.Ref2);
		}
	}

	private static void UpdateActualState(Results buildingBalanceResult, CalculationInput calcInput)
	{
		double num = CalcTotalArea(calcInput);
		if (calcInput.BuildingZones.Count > 0)
		{
			buildingBalanceResult.NeededEnergyTable.Heating.ActualArea = calcInput.BuildingZones.Where((BuildingZone z) => z.HasHeating).Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.Heating.Actual) / num;
			buildingBalanceResult.NeededEnergyTable.Heating.Actual = calcInput.BuildingZones.Where((BuildingZone z) => z.HasHeating).Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.Heating.Actual);
			buildingBalanceResult.NeededEnergyTable.HeatingVentilation.ActualArea = calcInput.BuildingZones.Where((BuildingZone z) => z.HasHeating).Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.HeatingVentilation.Actual) / num;
			buildingBalanceResult.NeededEnergyTable.HeatingVentilation.Actual = calcInput.BuildingZones.Where((BuildingZone z) => z.HasHeating).Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.HeatingVentilation.Actual);
			buildingBalanceResult.NeededEnergyTable.Cooling.ActualArea = calcInput.BuildingZones.Where((BuildingZone z) => z.HasCooling).Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.Cooling.Actual) / num;
			buildingBalanceResult.NeededEnergyTable.Cooling.Actual = calcInput.BuildingZones.Where((BuildingZone z) => z.HasCooling).Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.Cooling.Actual);
			buildingBalanceResult.NeededEnergyTable.CoolingVentilation.ActualArea = calcInput.BuildingZones.Where((BuildingZone z) => z.HasCooling).Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.CoolingVentilation.Actual) / num;
			buildingBalanceResult.NeededEnergyTable.CoolingVentilation.Actual = calcInput.BuildingZones.Where((BuildingZone z) => z.HasCooling).Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.CoolingVentilation.Actual);
			buildingBalanceResult.NeededEnergyTable.BGV.ActualArea = calcInput.BuildingZones.First().ZoneResults.NeededEnergyTable.BGV.Actual / num;
			buildingBalanceResult.NeededEnergyTable.BGV.Actual = calcInput.BuildingZones.First().ZoneResults.NeededEnergyTable.BGV.Actual;
			buildingBalanceResult.NeededEnergyTable.BGVPumps.ActualArea = calcInput.BuildingZones.First().ZoneResults.NeededEnergyTable.BGVPumps.ActualArea + calcInput.BuildingZones.First().HeatingCalculations.HotWaterCalculations.SunEnergyCalculations.Actual.SunEnergyResTable.BGVPumpsTotal / num;
			buildingBalanceResult.NeededEnergyTable.BGVPumps.Actual = calcInput.BuildingZones.First().ZoneResults.NeededEnergyTable.BGVPumps.ActualArea * num + calcInput.BuildingZones.First().HeatingCalculations.HotWaterCalculations.SunEnergyCalculations.Actual.SunEnergyResTable.BGVPumpsTotal;
			buildingBalanceResult.NeededEnergyTable.FansAndPumps.ActualArea = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.FansAndPumps.Actual) / num;
			buildingBalanceResult.NeededEnergyTable.FansAndPumps.Actual = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.FansAndPumps.Actual);
			buildingBalanceResult.NeededEnergyTable.Lights.ActualArea = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.Lights.Actual) / num;
			buildingBalanceResult.NeededEnergyTable.Lights.Actual = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.Lights.Actual);
			buildingBalanceResult.NeededEnergyTable.HeatAffectingDevices.ActualArea = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.HeatAffectingDevices.Actual) / num;
			buildingBalanceResult.NeededEnergyTable.HeatAffectingDevices.Actual = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.HeatAffectingDevices.Actual);
			buildingBalanceResult.NeededEnergyTable.NonHeatAffectingDevices.ActualArea = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.NonHeatAffectingDevices.Actual) / num;
			buildingBalanceResult.NeededEnergyTable.NonHeatAffectingDevices.Actual = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.NonHeatAffectingDevices.Actual);
			buildingBalanceResult.NeededEnergyTable.Other.ActualArea = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.Other.Actual) / num;
			buildingBalanceResult.NeededEnergyTable.Other.Actual = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.Other.Actual);
		}
	}

	private static void UpdateBaseLineState(Results buildingBalanceResult, CalculationInput calcInput)
	{
		double num = CalcTotalArea(calcInput);
		if (calcInput.BuildingZones.Count > 0)
		{
			buildingBalanceResult.NeededEnergyTable.Heating.BaseLineArea = calcInput.BuildingZones.Where((BuildingZone z) => z.HasHeating).Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.Heating.BaseLine) / num;
			buildingBalanceResult.NeededEnergyTable.Heating.BaseLine = calcInput.BuildingZones.Where((BuildingZone z) => z.HasHeating).Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.Heating.BaseLine);
			buildingBalanceResult.NeededEnergyTable.HeatingVentilation.BaseLineArea = calcInput.BuildingZones.Where((BuildingZone z) => z.HasHeating).Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.HeatingVentilation.BaseLine) / num;
			buildingBalanceResult.NeededEnergyTable.HeatingVentilation.BaseLine = calcInput.BuildingZones.Where((BuildingZone z) => z.HasHeating).Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.HeatingVentilation.BaseLine);
			buildingBalanceResult.NeededEnergyTable.Cooling.BaseLineArea = calcInput.BuildingZones.Where((BuildingZone z) => z.HasCooling).Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.Cooling.BaseLine) / num;
			buildingBalanceResult.NeededEnergyTable.Cooling.BaseLine = calcInput.BuildingZones.Where((BuildingZone z) => z.HasCooling).Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.Cooling.BaseLine);
			buildingBalanceResult.NeededEnergyTable.CoolingVentilation.BaseLineArea = calcInput.BuildingZones.Where((BuildingZone z) => z.HasCooling).Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.CoolingVentilation.BaseLine) / num;
			buildingBalanceResult.NeededEnergyTable.CoolingVentilation.BaseLine = calcInput.BuildingZones.Where((BuildingZone z) => z.HasCooling).Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.CoolingVentilation.BaseLine);
			buildingBalanceResult.NeededEnergyTable.BGV.BaseLineArea = calcInput.BuildingZones.First().ZoneResults.NeededEnergyTable.BGV.BaseLine / num;
			buildingBalanceResult.NeededEnergyTable.BGV.BaseLine = calcInput.BuildingZones.First().ZoneResults.NeededEnergyTable.BGV.BaseLine;
			buildingBalanceResult.NeededEnergyTable.BGVPumps.BaseLineArea = calcInput.BuildingZones.First().ZoneResults.NeededEnergyTable.BGVPumps.BaseLineArea + calcInput.BuildingZones.First().HeatingCalculations.HotWaterCalculations.SunEnergyCalculations.BaseLine.SunEnergyResTable.BGVPumpsTotal / num;
			buildingBalanceResult.NeededEnergyTable.BGVPumps.BaseLine = calcInput.BuildingZones.First().ZoneResults.NeededEnergyTable.BGVPumps.BaseLineArea * num + calcInput.BuildingZones.First().HeatingCalculations.HotWaterCalculations.SunEnergyCalculations.BaseLine.SunEnergyResTable.BGVPumpsTotal;
			buildingBalanceResult.NeededEnergyTable.FansAndPumps.BaseLineArea = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.FansAndPumps.BaseLine) / num;
			buildingBalanceResult.NeededEnergyTable.FansAndPumps.BaseLine = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.FansAndPumps.BaseLine);
			buildingBalanceResult.NeededEnergyTable.Lights.BaseLineArea = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.Lights.BaseLine) / num;
			buildingBalanceResult.NeededEnergyTable.Lights.BaseLine = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.Lights.BaseLine);
			buildingBalanceResult.NeededEnergyTable.HeatAffectingDevices.BaseLineArea = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.HeatAffectingDevices.BaseLine) / num;
			buildingBalanceResult.NeededEnergyTable.HeatAffectingDevices.BaseLine = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.HeatAffectingDevices.BaseLine);
			buildingBalanceResult.NeededEnergyTable.NonHeatAffectingDevices.BaseLineArea = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.NonHeatAffectingDevices.BaseLine) / num;
			buildingBalanceResult.NeededEnergyTable.NonHeatAffectingDevices.BaseLine = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.NonHeatAffectingDevices.BaseLine);
			buildingBalanceResult.NeededEnergyTable.Other.BaseLineArea = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.Other.BaseLine) / num;
			buildingBalanceResult.NeededEnergyTable.Other.BaseLine = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.Other.BaseLine);
		}
	}

	private static void UpdateEsmState(Results buildingBalanceResult, CalculationInput calcInput)
	{
		double num = CalcTotalArea(calcInput);
		if (calcInput.BuildingZones.Count > 0)
		{
			buildingBalanceResult.NeededEnergyTable.Heating.ESMArea = calcInput.BuildingZones.Where((BuildingZone z) => z.HasHeating).Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.Heating.ESM) / num;
			buildingBalanceResult.NeededEnergyTable.Heating.ESM = calcInput.BuildingZones.Where((BuildingZone z) => z.HasHeating).Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.Heating.ESM);
			buildingBalanceResult.NeededEnergyTable.HeatingVentilation.ESMArea = calcInput.BuildingZones.Where((BuildingZone z) => z.HasHeating).Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.HeatingVentilation.ESM) / num;
			buildingBalanceResult.NeededEnergyTable.HeatingVentilation.ESM = calcInput.BuildingZones.Where((BuildingZone z) => z.HasHeating).Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.HeatingVentilation.ESM);
			buildingBalanceResult.NeededEnergyTable.Cooling.ESMArea = calcInput.BuildingZones.Where((BuildingZone z) => z.HasCooling).Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.Cooling.ESM) / num;
			buildingBalanceResult.NeededEnergyTable.Cooling.ESM = calcInput.BuildingZones.Where((BuildingZone z) => z.HasCooling).Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.Cooling.ESM);
			buildingBalanceResult.NeededEnergyTable.CoolingVentilation.ESMArea = calcInput.BuildingZones.Where((BuildingZone z) => z.HasCooling).Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.CoolingVentilation.ESM) / num;
			buildingBalanceResult.NeededEnergyTable.CoolingVentilation.ESM = calcInput.BuildingZones.Where((BuildingZone z) => z.HasCooling).Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.CoolingVentilation.ESM);
			buildingBalanceResult.NeededEnergyTable.BGV.ESMArea = calcInput.BuildingZones.First().ZoneResults.NeededEnergyTable.BGV.ESM / num;
			buildingBalanceResult.NeededEnergyTable.BGV.ESM = calcInput.BuildingZones.First().ZoneResults.NeededEnergyTable.BGV.ESM;
			buildingBalanceResult.NeededEnergyTable.BGVPumps.ESMArea = calcInput.BuildingZones.First().ZoneResults.NeededEnergyTable.BGVPumps.ESMArea + calcInput.BuildingZones.First().HeatingCalculations.HotWaterCalculations.SunEnergyCalculations.ESM.SunEnergyResTable.BGVPumpsTotal / num;
			buildingBalanceResult.NeededEnergyTable.BGVPumps.ESM = calcInput.BuildingZones.First().ZoneResults.NeededEnergyTable.BGVPumps.ESMArea * num + calcInput.BuildingZones.First().HeatingCalculations.HotWaterCalculations.SunEnergyCalculations.ESM.SunEnergyResTable.BGVPumpsTotal;
			buildingBalanceResult.NeededEnergyTable.FansAndPumps.ESMArea = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.FansAndPumps.ESM) / num;
			buildingBalanceResult.NeededEnergyTable.FansAndPumps.ESM = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.FansAndPumps.ESM);
			buildingBalanceResult.NeededEnergyTable.Lights.ESMArea = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.Lights.ESM) / num;
			buildingBalanceResult.NeededEnergyTable.Lights.ESM = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.Lights.ESM);
			buildingBalanceResult.NeededEnergyTable.HeatAffectingDevices.ESMArea = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.HeatAffectingDevices.ESM) / num;
			buildingBalanceResult.NeededEnergyTable.HeatAffectingDevices.ESM = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.HeatAffectingDevices.ESM);
			buildingBalanceResult.NeededEnergyTable.NonHeatAffectingDevices.ESMArea = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.NonHeatAffectingDevices.ESM) / num;
			buildingBalanceResult.NeededEnergyTable.NonHeatAffectingDevices.ESM = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.NonHeatAffectingDevices.ESM);
			buildingBalanceResult.NeededEnergyTable.Other.ESMArea = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.Other.ESM) / num;
			buildingBalanceResult.NeededEnergyTable.Other.ESM = calcInput.BuildingZones.Sum((BuildingZone buildZone) => buildZone.ZoneResults.NeededEnergyTable.Other.ESM);
		}
	}

	private static void GetVeiHeating(Results zoneBalanceResult, Fuel fuel, double efficiency, double quantity, double area)
	{
		switch (fuel)
		{
		case Fuel.Fuel1:
			zoneBalanceResult.NeededEnergyTable.Heating.GeneralVEI += CalculateElectricityVEI(efficiency, quantity) * area;
			break;
		case Fuel.Fuel6:
			zoneBalanceResult.NeededEnergyTable.Heating.VEI += quantity * area;
			zoneBalanceResult.NeededEnergyTable.Heating.GeneralVEI += quantity * area;
			break;
		case Fuel.Fuel7:
			zoneBalanceResult.NeededEnergyTable.Heating.VEI += quantity * area;
			zoneBalanceResult.NeededEnergyTable.Heating.GeneralVEI += quantity * area;
			break;
		}
	}

	private static void GetVeiHeatVentilation(Results zoneBalanceResult, Fuel fuel, double efficiency, double quantity, double heatedArea)
	{
		switch (fuel)
		{
		case Fuel.Fuel1:
			zoneBalanceResult.NeededEnergyTable.HeatingVentilation.GeneralVEI += CalculateElectricityVEI(efficiency, quantity) * heatedArea;
			break;
		case Fuel.Fuel6:
			zoneBalanceResult.NeededEnergyTable.HeatingVentilation.VEI += quantity * heatedArea;
			zoneBalanceResult.NeededEnergyTable.HeatingVentilation.GeneralVEI += quantity * heatedArea;
			break;
		case Fuel.Fuel7:
			zoneBalanceResult.NeededEnergyTable.HeatingVentilation.VEI += quantity * heatedArea;
			zoneBalanceResult.NeededEnergyTable.HeatingVentilation.GeneralVEI += quantity * heatedArea;
			break;
		}
	}

	private static void GetVeiBGV(Results zoneBalanceResult, Fuel fuel, double efficiency, double quantity, double heatedArea)
	{
		switch (fuel)
		{
		case Fuel.Fuel1:
			zoneBalanceResult.NeededEnergyTable.BGV.GeneralVEI += CalculateElectricityVEI(efficiency, quantity) * heatedArea;
			break;
		case Fuel.Fuel6:
			zoneBalanceResult.NeededEnergyTable.BGV.VEI += quantity * heatedArea;
			zoneBalanceResult.NeededEnergyTable.BGV.GeneralVEI += quantity * heatedArea;
			break;
		case Fuel.Fuel7:
			zoneBalanceResult.NeededEnergyTable.BGV.VEI += quantity * heatedArea;
			zoneBalanceResult.NeededEnergyTable.BGV.GeneralVEI += quantity * heatedArea;
			break;
		}
	}

	private static double CalculateElectricityVEI(double efficiency, double quantity)
	{
		return (efficiency > 100.0) ? (quantity * ((efficiency - 100.0) / 100.0)) : 0.0;
	}

	private static void CalculateTotalsNeededEnergyTable(Results buildingBalanceResult, bool isBGVused)
	{
		CalculateTotalActual(buildingBalanceResult, isBGVused);
		CalculateTotalActualYearly(buildingBalanceResult, isBGVused);
		CalculateTotalBaseLine(buildingBalanceResult, isBGVused);
		CalculateTotalBaseLineYearly(buildingBalanceResult, isBGVused);
		CalculateTotalEsm(buildingBalanceResult, isBGVused);
		CalculateTotalEsmYearly(buildingBalanceResult, isBGVused);
		CalculateTotalRefs(buildingBalanceResult, isBGVused);
		CalculateTotalRefsYearly(buildingBalanceResult, isBGVused);
	}

	private static void CalculateTotalRefsYearly(Results buildingBalanceResult, bool isBGVused)
	{
		if (!isBGVused)
		{
			buildingBalanceResult.NeededEnergyTable.BGV.Ref1Area = 0.0;
			buildingBalanceResult.NeededEnergyTable.BGVPumps.Ref1Area = 0.0;
			buildingBalanceResult.NeededEnergyTable.BGV.Ref2Area = 0.0;
			buildingBalanceResult.NeededEnergyTable.BGVPumps.Ref2Area = 0.0;
		}
		buildingBalanceResult.NeededEnergyTable.Total.Ref1Area = CheckForNaN(buildingBalanceResult.NeededEnergyTable.Heating.Ref1Area) + CheckForNaN(buildingBalanceResult.NeededEnergyTable.Cooling.Ref1Area) + CheckForNaN(buildingBalanceResult.NeededEnergyTable.HeatingVentilation.Ref1Area) + CheckForNaN(buildingBalanceResult.NeededEnergyTable.CoolingVentilation.Ref1Area) + CheckForNaN(buildingBalanceResult.NeededEnergyTable.BGV.Ref1Area) + CheckForNaN(buildingBalanceResult.NeededEnergyTable.BGVPumps.Ref1Area) + CheckForNaN(buildingBalanceResult.NeededEnergyTable.FansAndPumps.Ref1Area) + CheckForNaN(buildingBalanceResult.NeededEnergyTable.Lights.Ref1Area) + CheckForNaN(buildingBalanceResult.NeededEnergyTable.HeatAffectingDevices.Ref1Area) + CheckForNaN(buildingBalanceResult.NeededEnergyTable.NonHeatAffectingDevices.Ref1Area) + CheckForNaN(buildingBalanceResult.NeededEnergyTable.Other.Ref1Area);
		buildingBalanceResult.NeededEnergyTable.Total.Ref2Area = CheckForNaN(buildingBalanceResult.NeededEnergyTable.Heating.Ref2Area) + CheckForNaN(buildingBalanceResult.NeededEnergyTable.Cooling.Ref2Area) + CheckForNaN(buildingBalanceResult.NeededEnergyTable.HeatingVentilation.Ref2Area) + CheckForNaN(buildingBalanceResult.NeededEnergyTable.CoolingVentilation.Ref2Area) + CheckForNaN(buildingBalanceResult.NeededEnergyTable.BGV.Ref2Area) + CheckForNaN(buildingBalanceResult.NeededEnergyTable.BGVPumps.Ref2Area) + CheckForNaN(buildingBalanceResult.NeededEnergyTable.FansAndPumps.Ref2Area) + CheckForNaN(buildingBalanceResult.NeededEnergyTable.Lights.Ref2Area) + CheckForNaN(buildingBalanceResult.NeededEnergyTable.HeatAffectingDevices.Ref2Area) + CheckForNaN(buildingBalanceResult.NeededEnergyTable.NonHeatAffectingDevices.Ref2Area) + CheckForNaN(buildingBalanceResult.NeededEnergyTable.Other.Ref2Area);
	}

	private static void CalculateTotalRefs(Results buildingBalanceResult, bool isBGVused)
	{
		if (!isBGVused)
		{
			buildingBalanceResult.NeededEnergyTable.BGV.Ref1 = 0.0;
			buildingBalanceResult.NeededEnergyTable.BGVPumps.Ref1 = 0.0;
			buildingBalanceResult.NeededEnergyTable.BGV.Ref2 = 0.0;
			buildingBalanceResult.NeededEnergyTable.BGVPumps.Ref2 = 0.0;
		}
		buildingBalanceResult.NeededEnergyTable.Total.Ref1 = buildingBalanceResult.NeededEnergyTable.Heating.Ref1 + buildingBalanceResult.NeededEnergyTable.Cooling.Ref1 + buildingBalanceResult.NeededEnergyTable.HeatingVentilation.Ref1 + buildingBalanceResult.NeededEnergyTable.CoolingVentilation.Ref1 + buildingBalanceResult.NeededEnergyTable.BGV.Ref1 + buildingBalanceResult.NeededEnergyTable.BGVPumps.Ref1 + buildingBalanceResult.NeededEnergyTable.FansAndPumps.Ref1 + buildingBalanceResult.NeededEnergyTable.Lights.Ref1 + buildingBalanceResult.NeededEnergyTable.HeatAffectingDevices.Ref1 + buildingBalanceResult.NeededEnergyTable.NonHeatAffectingDevices.Ref1 + buildingBalanceResult.NeededEnergyTable.Other.Ref1;
		buildingBalanceResult.NeededEnergyTable.Total.Ref2 = buildingBalanceResult.NeededEnergyTable.Heating.Ref2 + buildingBalanceResult.NeededEnergyTable.Cooling.Ref2 + buildingBalanceResult.NeededEnergyTable.HeatingVentilation.Ref2 + buildingBalanceResult.NeededEnergyTable.CoolingVentilation.Ref2 + buildingBalanceResult.NeededEnergyTable.BGV.Ref2 + buildingBalanceResult.NeededEnergyTable.BGVPumps.Ref2 + buildingBalanceResult.NeededEnergyTable.FansAndPumps.Ref2 + buildingBalanceResult.NeededEnergyTable.Lights.Ref2 + buildingBalanceResult.NeededEnergyTable.HeatAffectingDevices.Ref2 + buildingBalanceResult.NeededEnergyTable.NonHeatAffectingDevices.Ref2 + buildingBalanceResult.NeededEnergyTable.Other.Ref2;
	}

	private static void CalculateTotalEsmYearly(Results buildingBalanceResult, bool isBGVused)
	{
		if (!isBGVused)
		{
			buildingBalanceResult.NeededEnergyTable.BGV.ESMArea = 0.0;
			buildingBalanceResult.NeededEnergyTable.BGVPumps.ESMArea = 0.0;
		}
		buildingBalanceResult.NeededEnergyTable.Total.ESMArea = buildingBalanceResult.NeededEnergyTable.Heating.ESMArea + buildingBalanceResult.NeededEnergyTable.Cooling.ESMArea + buildingBalanceResult.NeededEnergyTable.HeatingVentilation.ESMArea + buildingBalanceResult.NeededEnergyTable.CoolingVentilation.ESMArea + buildingBalanceResult.NeededEnergyTable.BGV.ESMArea + buildingBalanceResult.NeededEnergyTable.BGVPumps.ESMArea + buildingBalanceResult.NeededEnergyTable.FansAndPumps.ESMArea + buildingBalanceResult.NeededEnergyTable.Lights.ESMArea + buildingBalanceResult.NeededEnergyTable.HeatAffectingDevices.ESMArea + buildingBalanceResult.NeededEnergyTable.NonHeatAffectingDevices.ESMArea + buildingBalanceResult.NeededEnergyTable.Other.ESMArea;
	}

	private static void CalculateTotalVei(Results buildingBalanceResult, bool isBGVused)
	{
		if (!isBGVused)
		{
			buildingBalanceResult.NeededEnergyTable.BGV.VEI = 0.0;
			buildingBalanceResult.NeededEnergyTable.BGVPumps.VEI = 0.0;
			buildingBalanceResult.NeededEnergyTable.BGV.GeneralVEI = 0.0;
			buildingBalanceResult.NeededEnergyTable.BGVPumps.GeneralVEI = 0.0;
		}
		buildingBalanceResult.NeededEnergyTable.Total.VEI = buildingBalanceResult.NeededEnergyTable.Heating.VEI + buildingBalanceResult.NeededEnergyTable.Cooling.VEI + buildingBalanceResult.NeededEnergyTable.HeatingVentilation.VEI + buildingBalanceResult.NeededEnergyTable.CoolingVentilation.VEI + buildingBalanceResult.NeededEnergyTable.BGV.VEI + buildingBalanceResult.NeededEnergyTable.BGVPumps.VEI + buildingBalanceResult.NeededEnergyTable.FansAndPumps.VEI + buildingBalanceResult.NeededEnergyTable.Lights.VEI + buildingBalanceResult.NeededEnergyTable.HeatAffectingDevices.VEI + buildingBalanceResult.NeededEnergyTable.NonHeatAffectingDevices.VEI + buildingBalanceResult.NeededEnergyTable.Other.VEI;
		buildingBalanceResult.NeededEnergyTable.Total.GeneralVEI = buildingBalanceResult.NeededEnergyTable.Heating.GeneralVEI + buildingBalanceResult.NeededEnergyTable.Cooling.GeneralVEI + buildingBalanceResult.NeededEnergyTable.HeatingVentilation.GeneralVEI + buildingBalanceResult.NeededEnergyTable.CoolingVentilation.GeneralVEI + buildingBalanceResult.NeededEnergyTable.BGV.GeneralVEI + buildingBalanceResult.NeededEnergyTable.BGVPumps.GeneralVEI + buildingBalanceResult.NeededEnergyTable.FansAndPumps.GeneralVEI + buildingBalanceResult.NeededEnergyTable.Lights.GeneralVEI + buildingBalanceResult.NeededEnergyTable.HeatAffectingDevices.GeneralVEI + buildingBalanceResult.NeededEnergyTable.NonHeatAffectingDevices.GeneralVEI + buildingBalanceResult.NeededEnergyTable.Other.GeneralVEI;
	}

	private static void CalculateTotalEsm(Results buildingBalanceResult, bool isBGVused)
	{
		if (!isBGVused)
		{
			buildingBalanceResult.NeededEnergyTable.BGV.ESM = 0.0;
			buildingBalanceResult.NeededEnergyTable.BGVPumps.ESM = 0.0;
		}
		buildingBalanceResult.NeededEnergyTable.Total.ESM = buildingBalanceResult.NeededEnergyTable.Heating.ESM + buildingBalanceResult.NeededEnergyTable.Cooling.ESM + buildingBalanceResult.NeededEnergyTable.HeatingVentilation.ESM + buildingBalanceResult.NeededEnergyTable.CoolingVentilation.ESM + buildingBalanceResult.NeededEnergyTable.BGV.ESM + buildingBalanceResult.NeededEnergyTable.BGVPumps.ESM + buildingBalanceResult.NeededEnergyTable.FansAndPumps.ESM + buildingBalanceResult.NeededEnergyTable.Lights.ESM + buildingBalanceResult.NeededEnergyTable.HeatAffectingDevices.ESM + buildingBalanceResult.NeededEnergyTable.NonHeatAffectingDevices.ESM + buildingBalanceResult.NeededEnergyTable.Other.ESM;
	}

	private static void CalculateTotalBaseLineYearly(Results buildingBalanceResult, bool isBGVused)
	{
		if (!isBGVused)
		{
			buildingBalanceResult.NeededEnergyTable.BGV.BaseLineArea = 0.0;
			buildingBalanceResult.NeededEnergyTable.BGVPumps.BaseLineArea = 0.0;
		}
		buildingBalanceResult.NeededEnergyTable.Total.BaseLineArea = buildingBalanceResult.NeededEnergyTable.Heating.BaseLineArea + buildingBalanceResult.NeededEnergyTable.Cooling.BaseLineArea + buildingBalanceResult.NeededEnergyTable.HeatingVentilation.BaseLineArea + buildingBalanceResult.NeededEnergyTable.CoolingVentilation.BaseLineArea + buildingBalanceResult.NeededEnergyTable.BGV.BaseLineArea + buildingBalanceResult.NeededEnergyTable.BGVPumps.BaseLineArea + buildingBalanceResult.NeededEnergyTable.FansAndPumps.BaseLineArea + buildingBalanceResult.NeededEnergyTable.Lights.BaseLineArea + buildingBalanceResult.NeededEnergyTable.HeatAffectingDevices.BaseLineArea + buildingBalanceResult.NeededEnergyTable.NonHeatAffectingDevices.BaseLineArea + buildingBalanceResult.NeededEnergyTable.Other.BaseLineArea;
	}

	private static void CalculateTotalBaseLine(Results buildingBalanceResult, bool isBGVused)
	{
		if (!isBGVused)
		{
			buildingBalanceResult.NeededEnergyTable.BGV.BaseLine = 0.0;
			buildingBalanceResult.NeededEnergyTable.BGVPumps.BaseLine = 0.0;
		}
		buildingBalanceResult.NeededEnergyTable.Total.BaseLine = buildingBalanceResult.NeededEnergyTable.Heating.BaseLine + buildingBalanceResult.NeededEnergyTable.Cooling.BaseLine + buildingBalanceResult.NeededEnergyTable.HeatingVentilation.BaseLine + buildingBalanceResult.NeededEnergyTable.CoolingVentilation.BaseLine + buildingBalanceResult.NeededEnergyTable.BGV.BaseLine + buildingBalanceResult.NeededEnergyTable.BGVPumps.BaseLine + buildingBalanceResult.NeededEnergyTable.FansAndPumps.BaseLine + buildingBalanceResult.NeededEnergyTable.Lights.BaseLine + buildingBalanceResult.NeededEnergyTable.HeatAffectingDevices.BaseLine + buildingBalanceResult.NeededEnergyTable.NonHeatAffectingDevices.BaseLine + buildingBalanceResult.NeededEnergyTable.Other.BaseLine;
	}

	private static void CalculateTotalActualYearly(Results buildingBalanceResult, bool isBGVused)
	{
		if (!isBGVused)
		{
			buildingBalanceResult.NeededEnergyTable.BGV.ActualArea = 0.0;
			buildingBalanceResult.NeededEnergyTable.BGVPumps.ActualArea = 0.0;
		}
		buildingBalanceResult.NeededEnergyTable.Total.ActualArea = CheckForNaN(buildingBalanceResult.NeededEnergyTable.Heating.ActualArea) + CheckForNaN(buildingBalanceResult.NeededEnergyTable.Cooling.ActualArea) + CheckForNaN(buildingBalanceResult.NeededEnergyTable.HeatingVentilation.ActualArea) + CheckForNaN(buildingBalanceResult.NeededEnergyTable.CoolingVentilation.ActualArea) + CheckForNaN(buildingBalanceResult.NeededEnergyTable.BGV.ActualArea) + CheckForNaN(buildingBalanceResult.NeededEnergyTable.BGVPumps.ActualArea) + CheckForNaN(buildingBalanceResult.NeededEnergyTable.FansAndPumps.ActualArea) + CheckForNaN(buildingBalanceResult.NeededEnergyTable.Lights.ActualArea) + CheckForNaN(buildingBalanceResult.NeededEnergyTable.HeatAffectingDevices.ActualArea) + CheckForNaN(buildingBalanceResult.NeededEnergyTable.NonHeatAffectingDevices.ActualArea) + CheckForNaN(buildingBalanceResult.NeededEnergyTable.Other.ActualArea);
	}

	private static void CalculateTotalActual(Results buildingBalanceResult, bool isBGVused)
	{
		if (!isBGVused)
		{
			buildingBalanceResult.NeededEnergyTable.BGV.Actual = 0.0;
			buildingBalanceResult.NeededEnergyTable.BGVPumps.Actual = 0.0;
		}
		buildingBalanceResult.NeededEnergyTable.Total.Actual = CheckForNaN(buildingBalanceResult.NeededEnergyTable.Heating.Actual) + CheckForNaN(buildingBalanceResult.NeededEnergyTable.Cooling.Actual) + CheckForNaN(buildingBalanceResult.NeededEnergyTable.HeatingVentilation.Actual) + CheckForNaN(buildingBalanceResult.NeededEnergyTable.CoolingVentilation.Actual) + CheckForNaN(buildingBalanceResult.NeededEnergyTable.BGV.Actual) + CheckForNaN(buildingBalanceResult.NeededEnergyTable.BGVPumps.Actual) + CheckForNaN(buildingBalanceResult.NeededEnergyTable.FansAndPumps.Actual) + CheckForNaN(buildingBalanceResult.NeededEnergyTable.Lights.Actual) + CheckForNaN(buildingBalanceResult.NeededEnergyTable.HeatAffectingDevices.Actual) + CheckForNaN(buildingBalanceResult.NeededEnergyTable.NonHeatAffectingDevices.Actual) + CheckForNaN(buildingBalanceResult.NeededEnergyTable.Other.Actual);
	}

	private static void GetFuelTypeAndValues(Results zoneBalanceResult, BuildingZone buildZone, bool isBGVused, double area, double totalArea, bool isFirstBuildingZone = true)
	{
		if (buildZone.HasHeating)
		{
			GetFuelType(zoneBalanceResult, buildZone.HeatingCalculations.HeatingResult.Fuel1Actual, buildZone.HeatingCalculations.HeatingResult.ResultSourceEnergyActual, area);
			GetFuelType(zoneBalanceResult, buildZone.HeatingCalculations.HeatingResult.Fuel2Actual, buildZone.HeatingCalculations.HeatingResult.ResultSourceEnergy2Actual, area);
			GetVeiHeating(zoneBalanceResult, buildZone.HeatingCalculations.HeatingResult.Fuel1ESM, buildZone.HeatingCalculations.HeatingResult.GeneratorHeatEfficiency1ESM, buildZone.HeatingCalculations.HeatingResult.ResultSourceEnergyESM, buildZone.Heating.Area.HeatedArea);
			GetVeiHeating(zoneBalanceResult, buildZone.HeatingCalculations.HeatingResult.Fuel2ESM, buildZone.HeatingCalculations.HeatingResult.GeneratorHeatEfficiency2ESM, buildZone.HeatingCalculations.HeatingResult.ResultSourceEnergy2ESM, buildZone.Heating.Area.HeatedArea);
			GetFuelType(zoneBalanceResult, buildZone.HeatingCalculations.VentilationHeating.Fuel1Actual, buildZone.HeatingCalculations.VentilationHeating.ResultSourceEnergyActual, area);
			GetFuelType(zoneBalanceResult, buildZone.HeatingCalculations.VentilationHeating.Fuel2Actual, buildZone.HeatingCalculations.VentilationHeating.ResultSourceEnergy2Actual, area);
			GetVeiHeatVentilation(zoneBalanceResult, buildZone.HeatingCalculations.VentilationHeating.Fuel1ESM, buildZone.HeatingCalculations.VentilationHeating.GeneratorHeatEfficiency1ESM, buildZone.HeatingCalculations.VentilationHeating.ResultSourceEnergyESM, buildZone.Heating.Area.HeatedArea);
			GetVeiHeatVentilation(zoneBalanceResult, buildZone.HeatingCalculations.VentilationHeating.Fuel2ESM, buildZone.HeatingCalculations.VentilationHeating.GeneratorHeatEfficiency2ESM, buildZone.HeatingCalculations.VentilationHeating.ResultSourceEnergy2ESM, buildZone.Heating.Area.HeatedArea);
		}
		if (buildZone.HasCooling)
		{
			GetFuelType(zoneBalanceResult, buildZone.CoolingCalculations.CoolingResult.Fuel1Actual, buildZone.CoolingCalculations.CoolingResult.ResultSourceEnergyActual, area);
			GetFuelType(zoneBalanceResult, buildZone.CoolingCalculations.CoolingResult.Fuel2Actual, buildZone.CoolingCalculations.CoolingResult.ResultSourceEnergy2Actual, area);
			GetFuelType(zoneBalanceResult, buildZone.CoolingCalculations.VentilationCooling.Fuel1Actual, buildZone.CoolingCalculations.VentilationCooling.ResultSourceEnergyActual, area);
			GetFuelType(zoneBalanceResult, buildZone.CoolingCalculations.VentilationCooling.Fuel2Actual, buildZone.CoolingCalculations.VentilationCooling.ResultSourceEnergy2Actual, area);
		}
		GetFuelType(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.FansAndPumps.CoolNeededEnergyActual + buildZone.HeatingCalculations.FansAndPumps.PumpNeededEnergyActual, area);
		GetFuelType(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.Lights.General.DevicesNeededEnergyActual, area);
		GetFuelType(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.BalancedDevices.General.DevicesNeededEnergyActual, area);
		GetFuelType(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.NonBalancedDevices.General.DevicesNeededEnergyActual, area);
		GetFuelType(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.FansAndPumps.OtherResultCoolingActual, area);
		if (isBGVused)
		{
			zoneBalanceResult.FuelEnergyTable.Fuel8.ActualArea += buildZone.ZoneResults.NeededEnergyTable.BGVPumps.ActualArea * totalArea;
		}
		if (buildZone.HasHeating)
		{
			GetFuelTypeBaseLine(zoneBalanceResult, buildZone.HeatingCalculations.HeatingResult.Fuel1BaseLine, buildZone.HeatingCalculations.HeatingResult.ResultSourceEnergyBaseLine, area);
			GetFuelTypeBaseLine(zoneBalanceResult, buildZone.HeatingCalculations.HeatingResult.Fuel2BaseLine, buildZone.HeatingCalculations.HeatingResult.ResultSourceEnergy2BaseLine, area);
			GetFuelTypeBaseLine(zoneBalanceResult, buildZone.HeatingCalculations.VentilationHeating.Fuel1BaseLine, buildZone.HeatingCalculations.VentilationHeating.ResultSourceEnergyBaseLine, area);
			GetFuelTypeBaseLine(zoneBalanceResult, buildZone.HeatingCalculations.VentilationHeating.Fuel2BaseLine, buildZone.HeatingCalculations.VentilationHeating.ResultSourceEnergy2BaseLine, area);
		}
		if (buildZone.HasCooling)
		{
			GetFuelTypeBaseLine(zoneBalanceResult, buildZone.CoolingCalculations.CoolingResult.Fuel1BaseLine, buildZone.CoolingCalculations.CoolingResult.ResultSourceEnergyBaseLine, area);
			GetFuelTypeBaseLine(zoneBalanceResult, buildZone.CoolingCalculations.CoolingResult.Fuel2BaseLine, buildZone.CoolingCalculations.CoolingResult.ResultSourceEnergy2BaseLine, area);
			GetFuelTypeBaseLine(zoneBalanceResult, buildZone.CoolingCalculations.VentilationCooling.Fuel1BaseLine, buildZone.CoolingCalculations.VentilationCooling.ResultSourceEnergyBaseLine, area);
			GetFuelTypeBaseLine(zoneBalanceResult, buildZone.CoolingCalculations.VentilationCooling.Fuel2BaseLine, buildZone.CoolingCalculations.VentilationCooling.ResultSourceEnergy2BaseLine, area);
		}
		GetFuelTypeBaseLine(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.FansAndPumps.CoolNeededEnergyBaseLine + buildZone.HeatingCalculations.FansAndPumps.PumpNeededEnergyBaseLine, area);
		GetFuelTypeBaseLine(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.Lights.General.DevicesNeededEnergyBaseLine, area);
		GetFuelTypeBaseLine(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.BalancedDevices.General.DevicesNeededEnergyBaseLine, area);
		GetFuelTypeBaseLine(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.NonBalancedDevices.General.DevicesNeededEnergyBaseLine, area);
		GetFuelTypeBaseLine(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.FansAndPumps.OtherResultCoolingBaseLine, area);
		if (isBGVused)
		{
			zoneBalanceResult.FuelEnergyTable.Fuel8.BaseLineArea += buildZone.ZoneResults.NeededEnergyTable.BGVPumps.BaseLineArea * totalArea;
		}
		if (buildZone.HasHeating)
		{
			GetFuelTypeEsm(zoneBalanceResult, buildZone.HeatingCalculations.HeatingResult.Fuel1ESM, buildZone.HeatingCalculations.HeatingResult.ResultSourceEnergyESM, area);
			GetFuelTypeEsm(zoneBalanceResult, buildZone.HeatingCalculations.HeatingResult.Fuel2ESM, buildZone.HeatingCalculations.HeatingResult.ResultSourceEnergy2ESM, area);
			GetFuelTypeEsm(zoneBalanceResult, buildZone.HeatingCalculations.VentilationHeating.Fuel1ESM, buildZone.HeatingCalculations.VentilationHeating.ResultSourceEnergyESM, area);
			GetFuelTypeEsm(zoneBalanceResult, buildZone.HeatingCalculations.VentilationHeating.Fuel2ESM, buildZone.HeatingCalculations.VentilationHeating.ResultSourceEnergy2ESM, area);
		}
		if (buildZone.HasCooling)
		{
			GetFuelTypeEsm(zoneBalanceResult, buildZone.CoolingCalculations.CoolingResult.Fuel1ESM, buildZone.CoolingCalculations.CoolingResult.ResultSourceEnergyESM, area);
			GetFuelTypeEsm(zoneBalanceResult, buildZone.CoolingCalculations.CoolingResult.Fuel2ESM, buildZone.CoolingCalculations.CoolingResult.ResultSourceEnergy2ESM, area);
			GetFuelTypeEsm(zoneBalanceResult, buildZone.CoolingCalculations.VentilationCooling.Fuel1ESM, buildZone.CoolingCalculations.VentilationCooling.ResultSourceEnergyESM, area);
			GetFuelTypeEsm(zoneBalanceResult, buildZone.CoolingCalculations.VentilationCooling.Fuel2ESM, buildZone.CoolingCalculations.VentilationCooling.ResultSourceEnergy2ESM, area);
		}
		GetFuelTypeEsm(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.FansAndPumps.CoolNeededEnergyESM + buildZone.HeatingCalculations.FansAndPumps.PumpNeededEnergyESM, area);
		GetFuelTypeEsm(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.Lights.General.DevicesNeededEnergyESM, area);
		GetFuelTypeEsm(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.BalancedDevices.General.DevicesNeededEnergyESM, area);
		GetFuelTypeEsm(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.NonBalancedDevices.General.DevicesNeededEnergyESM, area);
		GetFuelTypeEsm(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.FansAndPumps.OtherResultCoolingESM, area);
		if (isBGVused)
		{
			zoneBalanceResult.FuelEnergyTable.Fuel8.ESMArea += buildZone.ZoneResults.NeededEnergyTable.BGVPumps.ESMArea * totalArea;
		}
		if (buildZone.HasHeating)
		{
			GetFuelTypeRef1(zoneBalanceResult, buildZone.HeatingCalculations.HeatingResult.Fuel1Ref1, buildZone.HeatingCalculations.HeatingResult.ResultSourceEnergyRef1, area);
			GetFuelTypeRef1(zoneBalanceResult, buildZone.HeatingCalculations.HeatingResult.Fuel2Ref1, buildZone.HeatingCalculations.HeatingResult.ResultSourceEnergy2Ref1, area);
			GetFuelTypeRef1(zoneBalanceResult, buildZone.HeatingCalculations.VentilationHeating.Fuel1Ref1, buildZone.HeatingCalculations.VentilationHeating.ResultSourceEnergyRef1, area);
			GetFuelTypeRef1(zoneBalanceResult, buildZone.HeatingCalculations.VentilationHeating.Fuel2Ref1, buildZone.HeatingCalculations.VentilationHeating.ResultSourceEnergy2Ref1, area);
		}
		if (buildZone.HasCooling)
		{
			GetFuelTypeRef1(zoneBalanceResult, buildZone.CoolingCalculations.CoolingResult.Fuel1Ref1, buildZone.CoolingCalculations.CoolingResult.ResultSourceEnergyRef1, area);
			GetFuelTypeRef1(zoneBalanceResult, buildZone.CoolingCalculations.CoolingResult.Fuel2Ref1, buildZone.CoolingCalculations.CoolingResult.ResultSourceEnergy2Ref1, area);
			GetFuelTypeRef1(zoneBalanceResult, buildZone.CoolingCalculations.VentilationCooling.Fuel1ESM, buildZone.CoolingCalculations.VentilationCooling.ResultSourceEnergyRef1, area);
			GetFuelTypeRef1(zoneBalanceResult, buildZone.CoolingCalculations.VentilationCooling.Fuel2ESM, buildZone.CoolingCalculations.VentilationCooling.ResultSourceEnergy2Ref1, area);
		}
		GetFuelTypeRef1(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.FansAndPumps.CoolNeededEnergyRef1 + buildZone.HeatingCalculations.FansAndPumps.PumpNeededEnergyRef1, area);
		GetFuelTypeRef1(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.Lights.General.DevicesNeededEnergyRef1, area);
		GetFuelTypeRef1(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.BalancedDevices.General.DevicesNeededEnergyRef1, area);
		GetFuelTypeRef1(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.NonBalancedDevices.General.DevicesNeededEnergyRef1, area);
		GetFuelTypeRef1(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.FansAndPumps.OtherResultCoolingRef1, area);
		if (isBGVused)
		{
			zoneBalanceResult.FuelEnergyTable.Fuel8.Ref1Area += buildZone.ZoneResults.NeededEnergyTable.BGVPumps.Ref1Area * totalArea;
		}
		if (buildZone.HasHeating)
		{
			GetFuelTypeRef2(zoneBalanceResult, buildZone.HeatingCalculations.HeatingResult.Fuel1Ref2, buildZone.HeatingCalculations.HeatingResult.ResultSourceEnergyRef2, area);
			GetFuelTypeRef2(zoneBalanceResult, buildZone.HeatingCalculations.HeatingResult.Fuel2Ref2, buildZone.HeatingCalculations.HeatingResult.ResultSourceEnergy2Ref2, area);
			GetFuelTypeRef2(zoneBalanceResult, buildZone.HeatingCalculations.VentilationHeating.Fuel1Ref2, buildZone.HeatingCalculations.VentilationHeating.ResultSourceEnergyRef2, area);
			GetFuelTypeRef2(zoneBalanceResult, buildZone.HeatingCalculations.VentilationHeating.Fuel2Ref2, buildZone.HeatingCalculations.VentilationHeating.ResultSourceEnergy2Ref2, area);
		}
		if (buildZone.HasCooling)
		{
			GetFuelTypeRef2(zoneBalanceResult, buildZone.CoolingCalculations.CoolingResult.Fuel1Ref2, buildZone.CoolingCalculations.CoolingResult.ResultSourceEnergyRef2, area);
			GetFuelTypeRef2(zoneBalanceResult, buildZone.CoolingCalculations.CoolingResult.Fuel2Ref2, buildZone.CoolingCalculations.CoolingResult.ResultSourceEnergy2Ref2, area);
			GetFuelTypeRef2(zoneBalanceResult, buildZone.CoolingCalculations.VentilationCooling.Fuel1ESM, buildZone.CoolingCalculations.VentilationCooling.ResultSourceEnergyRef2, area);
			GetFuelTypeRef2(zoneBalanceResult, buildZone.CoolingCalculations.VentilationCooling.Fuel2ESM, buildZone.CoolingCalculations.VentilationCooling.ResultSourceEnergy2Ref2, area);
		}
		GetFuelTypeRef2(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.FansAndPumps.CoolNeededEnergyRef2 + buildZone.HeatingCalculations.FansAndPumps.PumpNeededEnergyRef2, area);
		GetFuelTypeRef2(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.Lights.General.DevicesNeededEnergyRef2, area);
		GetFuelTypeRef2(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.BalancedDevices.General.DevicesNeededEnergyRef2, area);
		GetFuelTypeRef2(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.NonBalancedDevices.General.DevicesNeededEnergyRef2, area);
		GetFuelTypeRef2(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.FansAndPumps.OtherResultCoolingRef2, area);
		if (isBGVused)
		{
			zoneBalanceResult.FuelEnergyTable.Fuel8.Ref2Area += buildZone.ZoneResults.NeededEnergyTable.BGVPumps.Ref2Area * totalArea;
		}
		if (isBGVused)
		{
			if (isFirstBuildingZone)
			{
				GetFuelTypeRef1(zoneBalanceResult, buildZone.HeatingCalculations.HotWaterCalculations.Fuel1Ref1, buildZone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergyRef1, totalArea);
				GetFuelTypeRef1(zoneBalanceResult, buildZone.HeatingCalculations.HotWaterCalculations.Fuel2Ref1, buildZone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergy2Ref1, totalArea);
				GetFuelTypeRef2(zoneBalanceResult, buildZone.HeatingCalculations.HotWaterCalculations.Fuel1Ref2, buildZone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergyRef2, totalArea);
				GetFuelTypeRef2(zoneBalanceResult, buildZone.HeatingCalculations.HotWaterCalculations.Fuel2Ref2, buildZone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergy2Ref2, totalArea);
				GetFuelType(zoneBalanceResult, buildZone.HeatingCalculations.HotWaterCalculations.Fuel1Actual, buildZone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergyActual, totalArea);
				GetFuelType(zoneBalanceResult, buildZone.HeatingCalculations.HotWaterCalculations.Fuel2Actual, buildZone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergy2Actual, totalArea);
				GetFuelTypeBaseLine(zoneBalanceResult, buildZone.HeatingCalculations.HotWaterCalculations.Fuel1BaseLine, buildZone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergyBaseLine, totalArea);
				GetFuelTypeBaseLine(zoneBalanceResult, buildZone.HeatingCalculations.HotWaterCalculations.Fuel2BaseLine, buildZone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergy2BaseLine, totalArea);
				GetFuelTypeEsm(zoneBalanceResult, buildZone.HeatingCalculations.HotWaterCalculations.Fuel1ESM, buildZone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergyESM, totalArea);
				GetFuelTypeEsm(zoneBalanceResult, buildZone.HeatingCalculations.HotWaterCalculations.Fuel2ESM, buildZone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergy2ESM, totalArea);
				GetVeiBGV(zoneBalanceResult, buildZone.HeatingCalculations.HotWaterCalculations.Fuel1ESM, buildZone.HeatingCalculations.HotWaterCalculations.GeneratorHeatEfficiency1ESM, buildZone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergyESM, totalArea);
				GetVeiBGV(zoneBalanceResult, buildZone.HeatingCalculations.HotWaterCalculations.Fuel2ESM, buildZone.HeatingCalculations.HotWaterCalculations.GeneratorHeatEfficiency2ESM, buildZone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergy2ESM, totalArea);
				zoneBalanceResult.NeededEnergyTable.BGV.GeneralVEI += Math.Min(buildZone.HeatingCalculations.HotWaterCalculations.SunEnergyESM, buildZone.HeatingCalculations.HotWaterCalculations.ResulNetEnergyESM) * totalArea;
			}
		}
		else
		{
			zoneBalanceResult.NeededEnergyTable.BGV.Ref1 = 0.0;
			zoneBalanceResult.NeededEnergyTable.BGV.Ref2 = 0.0;
			zoneBalanceResult.NeededEnergyTable.BGV.Actual = 0.0;
			zoneBalanceResult.NeededEnergyTable.BGV.BaseLine = 0.0;
			zoneBalanceResult.NeededEnergyTable.BGV.ESM = 0.0;
		}
	}

	private static void GetFuelTypeRef1(Results zoneBalanceResult, Fuel fuel, double quantity, double area)
	{
		if (double.IsInfinity(quantity) || double.IsNaN(quantity))
		{
			quantity = 0.0;
		}
		switch (fuel)
		{
		case Fuel.Fuel1:
			zoneBalanceResult.FuelEnergyTable.Fuel8.Ref1Area += quantity * area;
			break;
		case Fuel.Fuel2:
			zoneBalanceResult.FuelEnergyTable.Fuel2.Ref1Area += quantity * area;
			break;
		case Fuel.Fuel3:
			zoneBalanceResult.FuelEnergyTable.Fuel3.Ref1Area += quantity * area;
			break;
		case Fuel.Fuel4:
			zoneBalanceResult.FuelEnergyTable.Fuel4.Ref1Area += quantity * area;
			break;
		case Fuel.Fuel5:
			zoneBalanceResult.FuelEnergyTable.Fuel5.Ref1Area += quantity * area;
			break;
		case Fuel.Fuel6:
			zoneBalanceResult.FuelEnergyTable.Fuel6.Ref1Area += quantity * area;
			break;
		case Fuel.Fuel7:
			zoneBalanceResult.FuelEnergyTable.Fuel7.Ref1Area += quantity * area;
			break;
		case Fuel.Fuel8:
			zoneBalanceResult.FuelEnergyTable.Fuel1.Ref1Area += quantity * area;
			break;
		case Fuel.Fuel9:
			zoneBalanceResult.FuelEnergyTable.Fuel9.Ref1Area += quantity * area;
			break;
		case Fuel.Fuel10:
			zoneBalanceResult.FuelEnergyTable.Fuel10.Ref1Area += quantity * area;
			break;
		case Fuel.Fuel11:
			zoneBalanceResult.FuelEnergyTable.Fuel11.Ref1Area += quantity * area;
			break;
		}
	}

	private static void GetFuelTypeRef2(Results zoneBalanceResult, Fuel fuel, double quantity, double area)
	{
		if (double.IsInfinity(quantity) || double.IsNaN(quantity))
		{
			quantity = 0.0;
		}
		switch (fuel)
		{
		case Fuel.Fuel1:
			zoneBalanceResult.FuelEnergyTable.Fuel8.Ref2Area += quantity * area;
			break;
		case Fuel.Fuel2:
			zoneBalanceResult.FuelEnergyTable.Fuel2.Ref2Area += quantity * area;
			break;
		case Fuel.Fuel3:
			zoneBalanceResult.FuelEnergyTable.Fuel3.Ref2Area += quantity * area;
			break;
		case Fuel.Fuel4:
			zoneBalanceResult.FuelEnergyTable.Fuel4.Ref2Area += quantity * area;
			break;
		case Fuel.Fuel5:
			zoneBalanceResult.FuelEnergyTable.Fuel5.Ref2Area += quantity * area;
			break;
		case Fuel.Fuel6:
			zoneBalanceResult.FuelEnergyTable.Fuel6.Ref2Area += quantity * area;
			break;
		case Fuel.Fuel7:
			zoneBalanceResult.FuelEnergyTable.Fuel7.Ref2Area += quantity * area;
			break;
		case Fuel.Fuel8:
			zoneBalanceResult.FuelEnergyTable.Fuel1.Ref2Area += quantity * area;
			break;
		case Fuel.Fuel9:
			zoneBalanceResult.FuelEnergyTable.Fuel9.Ref2Area += quantity * area;
			break;
		case Fuel.Fuel10:
			zoneBalanceResult.FuelEnergyTable.Fuel10.Ref2Area += quantity * area;
			break;
		case Fuel.Fuel11:
			zoneBalanceResult.FuelEnergyTable.Fuel11.Ref2Area += quantity * area;
			break;
		}
	}

	private static void GetFuelType(Results zoneBalanceResult, Fuel fuel, double quantity, double area)
	{
		if (double.IsInfinity(quantity) || double.IsNaN(quantity))
		{
			quantity = 0.0;
		}
		switch (fuel)
		{
		case Fuel.Fuel1:
			zoneBalanceResult.FuelEnergyTable.Fuel8.ActualArea += quantity * area;
			break;
		case Fuel.Fuel2:
			zoneBalanceResult.FuelEnergyTable.Fuel2.ActualArea += quantity * area;
			break;
		case Fuel.Fuel3:
			zoneBalanceResult.FuelEnergyTable.Fuel3.ActualArea += quantity * area;
			break;
		case Fuel.Fuel4:
			zoneBalanceResult.FuelEnergyTable.Fuel4.ActualArea += quantity * area;
			break;
		case Fuel.Fuel5:
			zoneBalanceResult.FuelEnergyTable.Fuel5.ActualArea += quantity * area;
			break;
		case Fuel.Fuel6:
			zoneBalanceResult.FuelEnergyTable.Fuel6.ActualArea += quantity * area;
			break;
		case Fuel.Fuel7:
			zoneBalanceResult.FuelEnergyTable.Fuel7.ActualArea += quantity * area;
			break;
		case Fuel.Fuel8:
			zoneBalanceResult.FuelEnergyTable.Fuel1.ActualArea += quantity * area;
			break;
		case Fuel.Fuel9:
			zoneBalanceResult.FuelEnergyTable.Fuel9.ActualArea += quantity * area;
			break;
		case Fuel.Fuel10:
			zoneBalanceResult.FuelEnergyTable.Fuel10.ActualArea += quantity * area;
			break;
		case Fuel.Fuel11:
			zoneBalanceResult.FuelEnergyTable.Fuel11.ActualArea += quantity * area;
			break;
		}
	}

	private static void GetFuelTypeBaseLine(Results zoneBalanceResult, Fuel fuel, double quantity, double area)
	{
		if (double.IsInfinity(quantity) || double.IsNaN(quantity))
		{
			quantity = 0.0;
		}
		switch (fuel)
		{
		case Fuel.Fuel1:
			zoneBalanceResult.FuelEnergyTable.Fuel8.BaseLineArea += quantity * area;
			break;
		case Fuel.Fuel2:
			zoneBalanceResult.FuelEnergyTable.Fuel2.BaseLineArea += quantity * area;
			break;
		case Fuel.Fuel3:
			zoneBalanceResult.FuelEnergyTable.Fuel3.BaseLineArea += quantity * area;
			break;
		case Fuel.Fuel4:
			zoneBalanceResult.FuelEnergyTable.Fuel4.BaseLineArea += quantity * area;
			break;
		case Fuel.Fuel5:
			zoneBalanceResult.FuelEnergyTable.Fuel5.BaseLineArea += quantity * area;
			break;
		case Fuel.Fuel6:
			zoneBalanceResult.FuelEnergyTable.Fuel6.BaseLineArea += quantity * area;
			break;
		case Fuel.Fuel7:
			zoneBalanceResult.FuelEnergyTable.Fuel7.BaseLineArea += quantity * area;
			break;
		case Fuel.Fuel8:
			zoneBalanceResult.FuelEnergyTable.Fuel1.BaseLineArea += quantity * area;
			break;
		case Fuel.Fuel9:
			zoneBalanceResult.FuelEnergyTable.Fuel9.BaseLineArea += quantity * area;
			break;
		case Fuel.Fuel10:
			zoneBalanceResult.FuelEnergyTable.Fuel10.BaseLineArea += quantity * area;
			break;
		case Fuel.Fuel11:
			zoneBalanceResult.FuelEnergyTable.Fuel11.BaseLineArea += quantity * area;
			break;
		}
	}

	private static void GetFuelTypeEsm(Results zoneBalanceResult, Fuel fuel, double quantity, double area)
	{
		if (double.IsInfinity(quantity) || double.IsNaN(quantity))
		{
			quantity = 0.0;
		}
		switch (fuel)
		{
		case Fuel.Fuel1:
			zoneBalanceResult.FuelEnergyTable.Fuel8.ESMArea += quantity * area;
			break;
		case Fuel.Fuel2:
			zoneBalanceResult.FuelEnergyTable.Fuel2.ESMArea += quantity * area;
			break;
		case Fuel.Fuel3:
			zoneBalanceResult.FuelEnergyTable.Fuel3.ESMArea += quantity * area;
			break;
		case Fuel.Fuel4:
			zoneBalanceResult.FuelEnergyTable.Fuel4.ESMArea += quantity * area;
			break;
		case Fuel.Fuel5:
			zoneBalanceResult.FuelEnergyTable.Fuel5.ESMArea += quantity * area;
			break;
		case Fuel.Fuel6:
			zoneBalanceResult.FuelEnergyTable.Fuel6.ESMArea += quantity * area;
			break;
		case Fuel.Fuel7:
			zoneBalanceResult.FuelEnergyTable.Fuel7.ESMArea += quantity * area;
			break;
		case Fuel.Fuel8:
			zoneBalanceResult.FuelEnergyTable.Fuel1.ESMArea += quantity * area;
			break;
		case Fuel.Fuel9:
			zoneBalanceResult.FuelEnergyTable.Fuel9.ESMArea += quantity * area;
			break;
		case Fuel.Fuel10:
			zoneBalanceResult.FuelEnergyTable.Fuel10.ESMArea += quantity * area;
			break;
		case Fuel.Fuel11:
			zoneBalanceResult.FuelEnergyTable.Fuel11.ESMArea += quantity * area;
			break;
		}
	}

	private static void CalculateTotalFuelEnergy(Results zoneBalanceResult)
	{
		CalculateTotalFuelRef1(zoneBalanceResult);
		CalculateTotalFuelRef2(zoneBalanceResult);
		CalculateTotalFuelActual(zoneBalanceResult);
		CalculateTotalFuelBaseLine(zoneBalanceResult);
		CalculateTotalFuelESM(zoneBalanceResult);
	}

	private static void CalculateTotalFuelRef1(Results zoneBalanceResult)
	{
		zoneBalanceResult.FuelEnergyTable.Total.Ref1Area = zoneBalanceResult.FuelEnergyTable.Fuel1.Ref1Area + zoneBalanceResult.FuelEnergyTable.Fuel1.Ref1Area + zoneBalanceResult.FuelEnergyTable.Fuel2.Ref1Area + zoneBalanceResult.FuelEnergyTable.Fuel3.Ref1Area + zoneBalanceResult.FuelEnergyTable.Fuel4.Ref1Area + zoneBalanceResult.FuelEnergyTable.Fuel5.Ref1Area + zoneBalanceResult.FuelEnergyTable.Fuel6.Ref1Area + zoneBalanceResult.FuelEnergyTable.Fuel7.Ref1Area + zoneBalanceResult.FuelEnergyTable.Fuel8.Ref1Area + zoneBalanceResult.FuelEnergyTable.Fuel9.Ref1Area + zoneBalanceResult.FuelEnergyTable.Fuel10.Ref1Area + zoneBalanceResult.FuelEnergyTable.Fuel11.Ref1Area;
	}

	private static void CalculateTotalFuelRef2(Results zoneBalanceResult)
	{
		zoneBalanceResult.FuelEnergyTable.Total.Ref2Area = zoneBalanceResult.FuelEnergyTable.Fuel1.Ref2Area + zoneBalanceResult.FuelEnergyTable.Fuel1.Ref2Area + zoneBalanceResult.FuelEnergyTable.Fuel2.Ref2Area + zoneBalanceResult.FuelEnergyTable.Fuel3.Ref2Area + zoneBalanceResult.FuelEnergyTable.Fuel4.Ref2Area + zoneBalanceResult.FuelEnergyTable.Fuel5.Ref2Area + zoneBalanceResult.FuelEnergyTable.Fuel6.Ref2Area + zoneBalanceResult.FuelEnergyTable.Fuel7.Ref2Area + zoneBalanceResult.FuelEnergyTable.Fuel8.Ref2Area + zoneBalanceResult.FuelEnergyTable.Fuel9.Ref2Area + zoneBalanceResult.FuelEnergyTable.Fuel10.Ref2Area + zoneBalanceResult.FuelEnergyTable.Fuel11.Ref2Area;
	}

	private static void CalculateTotalFuelActual(Results zoneBalanceResult)
	{
		zoneBalanceResult.FuelEnergyTable.Total.ActualArea = zoneBalanceResult.FuelEnergyTable.Fuel1.ActualArea + zoneBalanceResult.FuelEnergyTable.Fuel1.ActualArea + zoneBalanceResult.FuelEnergyTable.Fuel2.ActualArea + zoneBalanceResult.FuelEnergyTable.Fuel3.ActualArea + zoneBalanceResult.FuelEnergyTable.Fuel4.ActualArea + zoneBalanceResult.FuelEnergyTable.Fuel5.ActualArea + zoneBalanceResult.FuelEnergyTable.Fuel6.ActualArea + zoneBalanceResult.FuelEnergyTable.Fuel7.ActualArea + zoneBalanceResult.FuelEnergyTable.Fuel8.ActualArea + zoneBalanceResult.FuelEnergyTable.Fuel9.ActualArea + zoneBalanceResult.FuelEnergyTable.Fuel10.ActualArea + zoneBalanceResult.FuelEnergyTable.Fuel11.ActualArea;
	}

	private static void CalculateTotalFuelBaseLine(Results zoneBalanceResult)
	{
		zoneBalanceResult.FuelEnergyTable.Total.BaseLineArea = zoneBalanceResult.FuelEnergyTable.Fuel1.BaseLineArea + zoneBalanceResult.FuelEnergyTable.Fuel1.BaseLineArea + zoneBalanceResult.FuelEnergyTable.Fuel2.BaseLineArea + zoneBalanceResult.FuelEnergyTable.Fuel3.BaseLineArea + zoneBalanceResult.FuelEnergyTable.Fuel4.BaseLineArea + zoneBalanceResult.FuelEnergyTable.Fuel5.BaseLineArea + zoneBalanceResult.FuelEnergyTable.Fuel6.BaseLineArea + zoneBalanceResult.FuelEnergyTable.Fuel7.BaseLineArea + zoneBalanceResult.FuelEnergyTable.Fuel8.BaseLineArea + zoneBalanceResult.FuelEnergyTable.Fuel9.BaseLineArea + zoneBalanceResult.FuelEnergyTable.Fuel10.BaseLineArea + zoneBalanceResult.FuelEnergyTable.Fuel11.BaseLineArea;
	}

	private static void CalculateTotalFuelESM(Results zoneBalanceResult)
	{
		zoneBalanceResult.FuelEnergyTable.Total.ESMArea = zoneBalanceResult.FuelEnergyTable.Fuel1.ESMArea + zoneBalanceResult.FuelEnergyTable.Fuel1.ESMArea + zoneBalanceResult.FuelEnergyTable.Fuel2.ESMArea + zoneBalanceResult.FuelEnergyTable.Fuel3.ESMArea + zoneBalanceResult.FuelEnergyTable.Fuel4.ESMArea + zoneBalanceResult.FuelEnergyTable.Fuel5.ESMArea + zoneBalanceResult.FuelEnergyTable.Fuel6.ESMArea + zoneBalanceResult.FuelEnergyTable.Fuel7.ESMArea + zoneBalanceResult.FuelEnergyTable.Fuel8.ESMArea + zoneBalanceResult.FuelEnergyTable.Fuel9.ESMArea + zoneBalanceResult.FuelEnergyTable.Fuel10.ESMArea + zoneBalanceResult.FuelEnergyTable.Fuel11.ESMArea;
	}

	private static void ClearFuelCells(Results zoneBalanceResult)
	{
		zoneBalanceResult.FuelEnergyTable.Fuel1.Ref1 = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel2.Ref1 = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel3.Ref1 = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel4.Ref1 = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel5.Ref1 = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel6.Ref1 = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel7.Ref1 = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel8.Ref1 = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel9.Ref1 = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel10.Ref1 = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel11.Ref1 = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel1.Ref2 = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel2.Ref2 = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel3.Ref2 = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel4.Ref2 = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel5.Ref2 = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel6.Ref2 = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel7.Ref2 = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel8.Ref2 = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel9.Ref2 = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel10.Ref2 = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel11.Ref2 = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel1.Ref1Area = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel2.Ref1Area = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel3.Ref1Area = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel4.Ref1Area = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel5.Ref1Area = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel6.Ref1Area = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel7.Ref1Area = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel8.Ref1Area = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel9.Ref1Area = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel10.Ref1Area = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel11.Ref1Area = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel1.Ref2Area = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel2.Ref2Area = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel3.Ref2Area = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel4.Ref2Area = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel5.Ref2Area = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel6.Ref2Area = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel7.Ref2Area = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel8.Ref2Area = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel9.Ref2Area = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel10.Ref2Area = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel11.Ref2Area = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel1.ActualArea = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel2.ActualArea = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel3.ActualArea = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel4.ActualArea = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel5.ActualArea = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel6.ActualArea = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel7.ActualArea = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel8.ActualArea = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel9.ActualArea = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel10.ActualArea = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel11.ActualArea = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel1.BaseLineArea = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel2.BaseLineArea = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel3.BaseLineArea = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel4.BaseLineArea = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel5.BaseLineArea = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel6.BaseLineArea = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel7.BaseLineArea = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel8.BaseLineArea = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel9.BaseLineArea = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel10.BaseLineArea = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel11.BaseLineArea = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel1.ESMArea = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel2.ESMArea = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel3.ESMArea = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel4.ESMArea = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel5.ESMArea = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel6.ESMArea = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel7.ESMArea = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel8.ESMArea = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel9.ESMArea = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel10.ESMArea = 0.0;
		zoneBalanceResult.FuelEnergyTable.Fuel11.ESMArea = 0.0;
	}

	private static void ClearFuelCellsCO2(Results zoneBalanceResult)
	{
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel1.Ref1 = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel2.Ref1 = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel3.Ref1 = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel4.Ref1 = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel5.Ref1 = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel6.Ref1 = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel7.Ref1 = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel8.Ref1 = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel9.Ref1 = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel10.Ref1 = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel11.Ref1 = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel1.Ref2 = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel2.Ref2 = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel3.Ref2 = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel4.Ref2 = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel5.Ref2 = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel6.Ref2 = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel7.Ref2 = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel8.Ref2 = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel9.Ref2 = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel10.Ref2 = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel11.Ref2 = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel1.Actual = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel2.Actual = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel3.Actual = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel4.Actual = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel5.Actual = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel6.Actual = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel7.Actual = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel8.Actual = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel9.Actual = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel10.Actual = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel11.Actual = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel1.BaseLine = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel2.BaseLine = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel3.BaseLine = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel4.BaseLine = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel5.BaseLine = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel6.BaseLine = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel7.BaseLine = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel8.BaseLine = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel9.BaseLine = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel10.BaseLine = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel11.BaseLine = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel1.ESM = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel2.ESM = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel3.ESM = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel4.ESM = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel5.ESM = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel6.ESM = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel7.ESM = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel8.ESM = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel9.ESM = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel10.ESM = 0.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel11.ESM = 0.0;
	}

	private static double CheckForNaN(double value)
	{
		if (double.IsNaN(value) || double.IsInfinity(value))
		{
			return 0.0;
		}
		return value;
	}

	private static void CalculateNetEnergyByTechnologies(Results zoneResults, BuildingZone buldingZone)
	{
		double heatedArea = buldingZone.Heating.Area.HeatedArea;
		if (buldingZone.HasHeating)
		{
			zoneResults.NetEnergyTable.Heating.Ref1Area = CheckForNaN(buldingZone.HeatingCalculations.HeatingResult.ResulNetEnergyRef1);
			zoneResults.NetEnergyTable.Heating.Ref1 = CheckForNaN(buldingZone.HeatingCalculations.HeatingResult.ResulNetEnergyRef1) * heatedArea;
			zoneResults.NetEnergyTable.Heating.Ref2Area = CheckForNaN(buldingZone.HeatingCalculations.HeatingResult.ResulNetEnergyRef2);
			zoneResults.NetEnergyTable.Heating.Ref2 = CheckForNaN(buldingZone.HeatingCalculations.HeatingResult.ResulNetEnergyRef2) * heatedArea;
			zoneResults.NetEnergyTable.Heating.ActualArea = CheckForNaN(buldingZone.HeatingCalculations.HeatingResult.ResulNetEnergyActual);
			zoneResults.NetEnergyTable.Heating.Actual = CheckForNaN(buldingZone.HeatingCalculations.HeatingResult.ResulNetEnergyActual) * heatedArea;
			zoneResults.NetEnergyTable.Heating.BaseLineArea = CheckForNaN(buldingZone.HeatingCalculations.HeatingResult.ResulNetEnergyBaseLine);
			zoneResults.NetEnergyTable.Heating.BaseLine = CheckForNaN(buldingZone.HeatingCalculations.HeatingResult.ResulNetEnergyBaseLine) * heatedArea;
			zoneResults.NetEnergyTable.Heating.ESMArea = CheckForNaN(buldingZone.HeatingCalculations.HeatingResult.ResulNetEnergyESM);
			zoneResults.NetEnergyTable.Heating.ESM = CheckForNaN(buldingZone.HeatingCalculations.HeatingResult.ResulNetEnergyESM) * heatedArea;
			zoneResults.NetEnergyTable.HeatingVentilation.Ref1Area = CheckForNaN(buldingZone.HeatingCalculations.VentilationHeating.ResultEnergyForHeatingRef1);
			zoneResults.NetEnergyTable.HeatingVentilation.Ref1 = CheckForNaN(buldingZone.HeatingCalculations.VentilationHeating.ResultEnergyForHeatingRef1) * heatedArea;
			zoneResults.NetEnergyTable.HeatingVentilation.Ref2Area = CheckForNaN(buldingZone.HeatingCalculations.VentilationHeating.ResultEnergyForHeatingRef2);
			zoneResults.NetEnergyTable.HeatingVentilation.Ref2 = CheckForNaN(buldingZone.HeatingCalculations.VentilationHeating.ResultEnergyForHeatingRef1) * heatedArea;
			zoneResults.NetEnergyTable.HeatingVentilation.ActualArea = CheckForNaN(buldingZone.HeatingCalculations.VentilationHeating.ResultEnergyForHeatingActual);
			zoneResults.NetEnergyTable.HeatingVentilation.Actual = CheckForNaN(buldingZone.HeatingCalculations.VentilationHeating.ResultEnergyForHeatingActual) * heatedArea;
			zoneResults.NetEnergyTable.HeatingVentilation.BaseLineArea = CheckForNaN(buldingZone.HeatingCalculations.VentilationHeating.ResultEnergyForHeatingBaseLine);
			zoneResults.NetEnergyTable.HeatingVentilation.BaseLine = CheckForNaN(buldingZone.HeatingCalculations.VentilationHeating.ResultEnergyForHeatingBaseLine) * heatedArea;
			zoneResults.NetEnergyTable.HeatingVentilation.ESMArea = CheckForNaN(buldingZone.HeatingCalculations.VentilationHeating.ResultEnergyForHeatingESM);
			zoneResults.NetEnergyTable.HeatingVentilation.ESM = CheckForNaN(buldingZone.HeatingCalculations.VentilationHeating.ResultEnergyForHeatingESM) * heatedArea;
		}
		if (buldingZone.HasCooling)
		{
			zoneResults.NetEnergyTable.Cooling.Ref1Area = CheckForNaN(buldingZone.CoolingCalculations.CoolingResult.ResulNetEnergyRef1);
			zoneResults.NetEnergyTable.Cooling.Ref1 = CheckForNaN(buldingZone.CoolingCalculations.CoolingResult.ResulNetEnergyRef1) * heatedArea;
			zoneResults.NetEnergyTable.Cooling.Ref2Area = CheckForNaN(buldingZone.CoolingCalculations.CoolingResult.ResulNetEnergyRef2);
			zoneResults.NetEnergyTable.Cooling.Ref2 = CheckForNaN(buldingZone.CoolingCalculations.CoolingResult.ResulNetEnergyRef2) * heatedArea;
			zoneResults.NetEnergyTable.Cooling.ActualArea = CheckForNaN(buldingZone.CoolingCalculations.CoolingResult.ResulNetEnergyActual);
			zoneResults.NetEnergyTable.Cooling.Actual = CheckForNaN(buldingZone.CoolingCalculations.CoolingResult.ResulNetEnergyActual) * heatedArea;
			zoneResults.NetEnergyTable.Cooling.BaseLineArea = CheckForNaN(buldingZone.CoolingCalculations.CoolingResult.ResulNetEnergyBaseLine);
			zoneResults.NetEnergyTable.Cooling.BaseLine = CheckForNaN(buldingZone.CoolingCalculations.CoolingResult.ResulNetEnergyBaseLine) * heatedArea;
			zoneResults.NetEnergyTable.Cooling.ESMArea = CheckForNaN(buldingZone.CoolingCalculations.CoolingResult.ResulNetEnergyESM);
			zoneResults.NetEnergyTable.Cooling.ESM = CheckForNaN(buldingZone.CoolingCalculations.CoolingResult.ResulNetEnergyESM) * heatedArea;
			zoneResults.NetEnergyTable.CoolingVentilation.Ref1Area = CheckForNaN(buldingZone.CoolingCalculations.VentilationCooling.ResultEnergyForCoolingRef1);
			zoneResults.NetEnergyTable.CoolingVentilation.Ref1 = CheckForNaN(buldingZone.CoolingCalculations.VentilationCooling.ResultEnergyForCoolingRef1) * heatedArea;
			zoneResults.NetEnergyTable.CoolingVentilation.Ref2Area = CheckForNaN(buldingZone.CoolingCalculations.VentilationCooling.ResultEnergyForCoolingRef2);
			zoneResults.NetEnergyTable.CoolingVentilation.Ref2 = CheckForNaN(buldingZone.CoolingCalculations.VentilationCooling.ResultEnergyForCoolingRef2) * heatedArea;
			zoneResults.NetEnergyTable.CoolingVentilation.ActualArea = CheckForNaN(buldingZone.CoolingCalculations.VentilationCooling.ResultEnergyForCoolingActual);
			zoneResults.NetEnergyTable.CoolingVentilation.Actual = CheckForNaN(buldingZone.CoolingCalculations.VentilationCooling.ResultEnergyForCoolingActual) * heatedArea;
			zoneResults.NetEnergyTable.CoolingVentilation.BaseLineArea = CheckForNaN(buldingZone.CoolingCalculations.VentilationCooling.ResultEnergyForCoolingBaseLine);
			zoneResults.NetEnergyTable.CoolingVentilation.BaseLine = CheckForNaN(buldingZone.CoolingCalculations.VentilationCooling.ResultEnergyForCoolingBaseLine) * heatedArea;
			zoneResults.NetEnergyTable.CoolingVentilation.ESMArea = CheckForNaN(buldingZone.CoolingCalculations.VentilationCooling.ResultEnergyForCoolingESM);
			zoneResults.NetEnergyTable.CoolingVentilation.ESM = CheckForNaN(buldingZone.CoolingCalculations.VentilationCooling.ResultEnergyForCoolingESM) * heatedArea;
		}
		zoneResults.NetEnergyTable.Total.Ref1Area = zoneResults.NetEnergyTable.Heating.Ref1Area + zoneResults.NetEnergyTable.HeatingVentilation.Ref1Area + zoneResults.NetEnergyTable.Cooling.Ref1Area + zoneResults.NetEnergyTable.CoolingVentilation.Ref1Area;
		zoneResults.NetEnergyTable.Total.Ref1 = zoneResults.NetEnergyTable.Heating.Ref1 + zoneResults.NetEnergyTable.HeatingVentilation.Ref1 + zoneResults.NetEnergyTable.Cooling.Ref1 + zoneResults.NetEnergyTable.CoolingVentilation.Ref1;
		zoneResults.NetEnergyTable.Total.Ref2Area = zoneResults.NetEnergyTable.Heating.Ref2Area + zoneResults.NetEnergyTable.HeatingVentilation.Ref2Area + zoneResults.NetEnergyTable.Cooling.Ref2Area + zoneResults.NetEnergyTable.CoolingVentilation.Ref2Area;
		zoneResults.NetEnergyTable.Total.Ref2 = zoneResults.NetEnergyTable.Heating.Ref2 + zoneResults.NetEnergyTable.HeatingVentilation.Ref2 + zoneResults.NetEnergyTable.Cooling.Ref2 + zoneResults.NetEnergyTable.CoolingVentilation.Ref2;
		zoneResults.NetEnergyTable.Total.ActualArea = zoneResults.NetEnergyTable.Heating.ActualArea + zoneResults.NetEnergyTable.HeatingVentilation.ActualArea + zoneResults.NetEnergyTable.Cooling.ActualArea + zoneResults.NetEnergyTable.CoolingVentilation.ActualArea;
		zoneResults.NetEnergyTable.Total.Actual = zoneResults.NetEnergyTable.Heating.Actual + zoneResults.NetEnergyTable.HeatingVentilation.Actual + zoneResults.NetEnergyTable.Cooling.Actual + zoneResults.NetEnergyTable.CoolingVentilation.Actual;
		zoneResults.NetEnergyTable.Total.BaseLine = zoneResults.NetEnergyTable.Heating.BaseLine + zoneResults.NetEnergyTable.HeatingVentilation.BaseLine + zoneResults.NetEnergyTable.Cooling.BaseLine + zoneResults.NetEnergyTable.CoolingVentilation.BaseLine;
		zoneResults.NetEnergyTable.Total.BaseLineArea = zoneResults.NetEnergyTable.Heating.BaseLineArea + zoneResults.NetEnergyTable.HeatingVentilation.BaseLineArea + zoneResults.NetEnergyTable.Cooling.BaseLineArea + zoneResults.NetEnergyTable.CoolingVentilation.BaseLineArea;
		zoneResults.NetEnergyTable.Total.ESM = zoneResults.NetEnergyTable.Heating.ESM + zoneResults.NetEnergyTable.HeatingVentilation.ESM + zoneResults.NetEnergyTable.Cooling.ESM + zoneResults.NetEnergyTable.CoolingVentilation.ESM;
		zoneResults.NetEnergyTable.Total.ESMArea = zoneResults.NetEnergyTable.Heating.ESMArea + zoneResults.NetEnergyTable.HeatingVentilation.ESMArea + zoneResults.NetEnergyTable.Cooling.ESMArea + zoneResults.NetEnergyTable.CoolingVentilation.ESMArea;
	}

	private static void CalculateNetEnergyPerArea(this Results buildingBalanceResult)
	{
		double totalHeatedArea = buildingBalanceResult.TotalAreaElements.TotalHeatedArea;
		buildingBalanceResult.NetEnergyTable.Heating.Ref1Area = CheckForNaN(buildingBalanceResult.NetEnergyTable.Heating.Ref1Area / totalHeatedArea);
		buildingBalanceResult.NetEnergyTable.Heating.Ref2Area = CheckForNaN(buildingBalanceResult.NetEnergyTable.Heating.Ref2Area / totalHeatedArea);
		buildingBalanceResult.NetEnergyTable.Heating.ActualArea = CheckForNaN(buildingBalanceResult.NetEnergyTable.Heating.ActualArea / totalHeatedArea);
		buildingBalanceResult.NetEnergyTable.Heating.BaseLineArea = CheckForNaN(buildingBalanceResult.NetEnergyTable.Heating.BaseLineArea / totalHeatedArea);
		buildingBalanceResult.NetEnergyTable.Heating.ESMArea = CheckForNaN(buildingBalanceResult.NetEnergyTable.Heating.ESMArea / totalHeatedArea);
		buildingBalanceResult.NetEnergyTable.Cooling.Ref1 = CheckForNaN(buildingBalanceResult.NetEnergyTable.Cooling.Ref1 / totalHeatedArea);
		buildingBalanceResult.NetEnergyTable.Cooling.Ref2 = CheckForNaN(buildingBalanceResult.NetEnergyTable.Cooling.Ref2 / totalHeatedArea);
		buildingBalanceResult.NetEnergyTable.Cooling.Ref1Area = CheckForNaN(buildingBalanceResult.NetEnergyTable.Cooling.Ref1Area / totalHeatedArea);
		buildingBalanceResult.NetEnergyTable.Cooling.Ref2Area = CheckForNaN(buildingBalanceResult.NetEnergyTable.Cooling.Ref2Area / totalHeatedArea);
		buildingBalanceResult.NetEnergyTable.Cooling.ActualArea = CheckForNaN(buildingBalanceResult.NetEnergyTable.Cooling.ActualArea / totalHeatedArea);
		buildingBalanceResult.NetEnergyTable.Cooling.BaseLineArea = CheckForNaN(buildingBalanceResult.NetEnergyTable.Cooling.BaseLineArea / totalHeatedArea);
		buildingBalanceResult.NetEnergyTable.Cooling.ESMArea = CheckForNaN(buildingBalanceResult.NetEnergyTable.Cooling.ESMArea / totalHeatedArea);
		buildingBalanceResult.NetEnergyTable.HeatingVentilation.Ref1Area = CheckForNaN(buildingBalanceResult.NetEnergyTable.HeatingVentilation.Ref1Area / totalHeatedArea);
		buildingBalanceResult.NetEnergyTable.HeatingVentilation.Ref2Area = CheckForNaN(buildingBalanceResult.NetEnergyTable.HeatingVentilation.Ref2Area / totalHeatedArea);
		buildingBalanceResult.NetEnergyTable.HeatingVentilation.ActualArea = CheckForNaN(buildingBalanceResult.NetEnergyTable.HeatingVentilation.ActualArea / totalHeatedArea);
		buildingBalanceResult.NetEnergyTable.HeatingVentilation.BaseLineArea = CheckForNaN(buildingBalanceResult.NetEnergyTable.HeatingVentilation.BaseLineArea / totalHeatedArea);
		buildingBalanceResult.NetEnergyTable.HeatingVentilation.ESMArea = CheckForNaN(buildingBalanceResult.NetEnergyTable.HeatingVentilation.ESMArea / totalHeatedArea);
		buildingBalanceResult.NetEnergyTable.CoolingVentilation.Ref1Area = CheckForNaN(buildingBalanceResult.NetEnergyTable.CoolingVentilation.Ref1Area / buildingBalanceResult.TotalAreaElements.TotalHeatedArea);
		buildingBalanceResult.NetEnergyTable.CoolingVentilation.Ref2Area = CheckForNaN(buildingBalanceResult.NetEnergyTable.CoolingVentilation.Ref2Area / buildingBalanceResult.TotalAreaElements.TotalHeatedArea);
		buildingBalanceResult.NetEnergyTable.CoolingVentilation.ActualArea = CheckForNaN(buildingBalanceResult.NetEnergyTable.CoolingVentilation.ActualArea / buildingBalanceResult.TotalAreaElements.TotalHeatedArea);
		buildingBalanceResult.NetEnergyTable.CoolingVentilation.BaseLineArea = CheckForNaN(buildingBalanceResult.NetEnergyTable.CoolingVentilation.BaseLineArea / buildingBalanceResult.TotalAreaElements.TotalHeatedArea);
		buildingBalanceResult.NetEnergyTable.CoolingVentilation.ESMArea = CheckForNaN(buildingBalanceResult.NetEnergyTable.CoolingVentilation.ESMArea / buildingBalanceResult.TotalAreaElements.TotalHeatedArea);
		buildingBalanceResult.NetEnergyTable.Total.Ref1Area = buildingBalanceResult.NetEnergyTable.Heating.Ref1Area + buildingBalanceResult.NetEnergyTable.HeatingVentilation.Ref1Area + buildingBalanceResult.NetEnergyTable.Cooling.Ref1Area + buildingBalanceResult.NetEnergyTable.CoolingVentilation.Ref1Area;
		buildingBalanceResult.NetEnergyTable.Total.Ref2Area = buildingBalanceResult.NetEnergyTable.Heating.Ref2Area + buildingBalanceResult.NetEnergyTable.HeatingVentilation.Ref2Area + buildingBalanceResult.NetEnergyTable.Cooling.Ref2Area + buildingBalanceResult.NetEnergyTable.CoolingVentilation.Ref2Area;
		buildingBalanceResult.NetEnergyTable.Total.ActualArea = buildingBalanceResult.NetEnergyTable.Heating.ActualArea + buildingBalanceResult.NetEnergyTable.HeatingVentilation.ActualArea + buildingBalanceResult.NetEnergyTable.Cooling.ActualArea + buildingBalanceResult.NetEnergyTable.CoolingVentilation.ActualArea;
		buildingBalanceResult.NetEnergyTable.Total.BaseLineArea = buildingBalanceResult.NetEnergyTable.Heating.BaseLineArea + buildingBalanceResult.NetEnergyTable.HeatingVentilation.BaseLineArea + buildingBalanceResult.NetEnergyTable.Cooling.BaseLineArea + buildingBalanceResult.NetEnergyTable.CoolingVentilation.BaseLineArea;
		buildingBalanceResult.NetEnergyTable.Total.ESMArea = buildingBalanceResult.NetEnergyTable.Heating.ESMArea + buildingBalanceResult.NetEnergyTable.HeatingVentilation.ESMArea + buildingBalanceResult.NetEnergyTable.Cooling.ESMArea + buildingBalanceResult.NetEnergyTable.CoolingVentilation.ESMArea;
	}

	private static void CalculateNetWithoutInputsEnergyByTechnologiesPerArea(this Results buildingBalanceResult)
	{
		double totalHeatedArea = buildingBalanceResult.TotalAreaElements.TotalHeatedArea;
		buildingBalanceResult.NoInputsNetEnergyTable.Heating.Ref1Area = buildingBalanceResult.NoInputsNetEnergyTable.Heating.Ref1 / totalHeatedArea;
		buildingBalanceResult.NoInputsNetEnergyTable.Heating.Ref2Area = buildingBalanceResult.NoInputsNetEnergyTable.Heating.Ref2 / totalHeatedArea;
		buildingBalanceResult.NoInputsNetEnergyTable.Heating.ActualArea = buildingBalanceResult.NoInputsNetEnergyTable.Heating.Actual / totalHeatedArea;
		buildingBalanceResult.NoInputsNetEnergyTable.Heating.BaseLineArea = buildingBalanceResult.NoInputsNetEnergyTable.Heating.BaseLine / totalHeatedArea;
		buildingBalanceResult.NoInputsNetEnergyTable.Heating.ESMArea = buildingBalanceResult.NoInputsNetEnergyTable.Heating.ESM / totalHeatedArea;
		buildingBalanceResult.NoInputsNetEnergyTable.Cooling.Ref1Area = buildingBalanceResult.NoInputsNetEnergyTable.Cooling.Ref1 / totalHeatedArea;
		buildingBalanceResult.NoInputsNetEnergyTable.Cooling.Ref2Area = buildingBalanceResult.NoInputsNetEnergyTable.Cooling.Ref2 / totalHeatedArea;
		buildingBalanceResult.NoInputsNetEnergyTable.Cooling.ActualArea = buildingBalanceResult.NoInputsNetEnergyTable.Cooling.Actual / totalHeatedArea;
		buildingBalanceResult.NoInputsNetEnergyTable.Cooling.BaseLineArea = buildingBalanceResult.NoInputsNetEnergyTable.Cooling.BaseLine / totalHeatedArea;
		buildingBalanceResult.NoInputsNetEnergyTable.Cooling.ESMArea = buildingBalanceResult.NoInputsNetEnergyTable.Cooling.ESM / totalHeatedArea;
		buildingBalanceResult.NoInputsNetEnergyTable.HeatingVentilation.ActualArea = buildingBalanceResult.NoInputsNetEnergyTable.HeatingVentilation.Actual / totalHeatedArea;
		buildingBalanceResult.NoInputsNetEnergyTable.HeatingVentilation.BaseLineArea = buildingBalanceResult.NoInputsNetEnergyTable.HeatingVentilation.BaseLine / totalHeatedArea;
		buildingBalanceResult.NoInputsNetEnergyTable.HeatingVentilation.ESMArea = buildingBalanceResult.NoInputsNetEnergyTable.HeatingVentilation.ESM / totalHeatedArea;
		buildingBalanceResult.NoInputsNetEnergyTable.CoolingVentilation.Ref1Area = buildingBalanceResult.NoInputsNetEnergyTable.CoolingVentilation.Ref1 / totalHeatedArea;
		buildingBalanceResult.NoInputsNetEnergyTable.CoolingVentilation.Ref2Area = buildingBalanceResult.NoInputsNetEnergyTable.CoolingVentilation.Ref2 / totalHeatedArea;
		buildingBalanceResult.NoInputsNetEnergyTable.CoolingVentilation.ActualArea = buildingBalanceResult.NoInputsNetEnergyTable.CoolingVentilation.Actual / totalHeatedArea;
		buildingBalanceResult.NoInputsNetEnergyTable.CoolingVentilation.BaseLineArea = buildingBalanceResult.NoInputsNetEnergyTable.CoolingVentilation.BaseLine / totalHeatedArea;
		buildingBalanceResult.NoInputsNetEnergyTable.CoolingVentilation.ESMArea = buildingBalanceResult.NoInputsNetEnergyTable.CoolingVentilation.ESM / totalHeatedArea;
		buildingBalanceResult.NetEnergyTable.Total.Ref1Area = buildingBalanceResult.NetEnergyTable.Heating.Ref1Area + buildingBalanceResult.NetEnergyTable.HeatingVentilation.Ref1Area + buildingBalanceResult.NetEnergyTable.Cooling.Ref1Area + buildingBalanceResult.NetEnergyTable.CoolingVentilation.Ref1Area;
		buildingBalanceResult.NetEnergyTable.Total.Ref2Area = buildingBalanceResult.NetEnergyTable.Heating.Ref2Area + buildingBalanceResult.NetEnergyTable.HeatingVentilation.Ref2Area + buildingBalanceResult.NetEnergyTable.Cooling.Ref2Area + buildingBalanceResult.NetEnergyTable.CoolingVentilation.Ref2Area;
		buildingBalanceResult.NetEnergyTable.Total.ActualArea = buildingBalanceResult.NetEnergyTable.Heating.ActualArea + buildingBalanceResult.NetEnergyTable.HeatingVentilation.ActualArea + buildingBalanceResult.NetEnergyTable.Cooling.ActualArea + buildingBalanceResult.NetEnergyTable.CoolingVentilation.ActualArea;
		buildingBalanceResult.NetEnergyTable.Total.BaseLineArea = buildingBalanceResult.NetEnergyTable.Heating.BaseLineArea + buildingBalanceResult.NetEnergyTable.HeatingVentilation.BaseLineArea + buildingBalanceResult.NetEnergyTable.Cooling.BaseLineArea + buildingBalanceResult.NetEnergyTable.CoolingVentilation.BaseLineArea;
		buildingBalanceResult.NetEnergyTable.Total.ESMArea = buildingBalanceResult.NetEnergyTable.Heating.ESMArea + buildingBalanceResult.NetEnergyTable.HeatingVentilation.ESMArea + buildingBalanceResult.NetEnergyTable.Cooling.ESMArea + buildingBalanceResult.NetEnergyTable.CoolingVentilation.ESMArea;
	}

	private static void CalculateNetEnergyByTechnologiesBuilding(Results zoneResults, BuildingZone buldingZone)
	{
		double heatedArea = buldingZone.Heating.Area.HeatedArea;
		if (buldingZone.HasHeating)
		{
			double num = buldingZone.HeatingCalculations.HeatingResult.ResulNetEnergyRef1 * heatedArea;
			zoneResults.NetEnergyTable.Heating.Ref1Area += num;
			zoneResults.NetEnergyTable.Heating.Ref1 += buldingZone.HeatingCalculations.HeatingResult.ResulNetEnergyRef1 * heatedArea;
			double num2 = buldingZone.HeatingCalculations.HeatingResult.ResulNetEnergyRef2 * heatedArea;
			zoneResults.NetEnergyTable.Heating.Ref2Area += num2;
			zoneResults.NetEnergyTable.Heating.Ref2 += buldingZone.HeatingCalculations.HeatingResult.ResulNetEnergyRef2 * heatedArea;
			double num3 = buldingZone.HeatingCalculations.HeatingResult.ResulNetEnergyActual * heatedArea;
			zoneResults.NetEnergyTable.Heating.ActualArea += num3;
			zoneResults.NetEnergyTable.Heating.Actual += buldingZone.HeatingCalculations.HeatingResult.ResulNetEnergyActual * heatedArea;
			double num4 = buldingZone.HeatingCalculations.HeatingResult.ResulNetEnergyBaseLine * heatedArea;
			zoneResults.NetEnergyTable.Heating.BaseLineArea += num4;
			zoneResults.NetEnergyTable.Heating.BaseLine += buldingZone.HeatingCalculations.HeatingResult.ResulNetEnergyBaseLine * heatedArea;
			double num5 = buldingZone.HeatingCalculations.HeatingResult.ResulNetEnergyESM * heatedArea;
			zoneResults.NetEnergyTable.Heating.ESMArea += num5;
			zoneResults.NetEnergyTable.Heating.ESM += buldingZone.HeatingCalculations.HeatingResult.ResulNetEnergyESM * heatedArea;
			num = buldingZone.HeatingCalculations.VentilationHeating.ResultEnergyForHeatingRef1 * heatedArea;
			zoneResults.NetEnergyTable.HeatingVentilation.Ref1Area += num;
			zoneResults.NetEnergyTable.HeatingVentilation.Ref1 += buldingZone.HeatingCalculations.VentilationHeating.ResultEnergyForHeatingRef1 * heatedArea;
			num2 = buldingZone.HeatingCalculations.VentilationHeating.ResultEnergyForHeatingRef2 * heatedArea;
			zoneResults.NetEnergyTable.HeatingVentilation.Ref2Area += num2;
			zoneResults.NetEnergyTable.HeatingVentilation.Ref2 += buldingZone.HeatingCalculations.VentilationHeating.ResultEnergyForHeatingRef2 * heatedArea;
			num3 = buldingZone.HeatingCalculations.VentilationHeating.ResultEnergyForHeatingActual * heatedArea;
			zoneResults.NetEnergyTable.HeatingVentilation.ActualArea += num3;
			zoneResults.NetEnergyTable.HeatingVentilation.Actual += buldingZone.HeatingCalculations.VentilationHeating.ResultEnergyForHeatingActual * heatedArea;
			num4 = buldingZone.HeatingCalculations.VentilationHeating.ResultEnergyForHeatingBaseLine * heatedArea;
			zoneResults.NetEnergyTable.HeatingVentilation.BaseLineArea += num4;
			zoneResults.NetEnergyTable.HeatingVentilation.BaseLine += buldingZone.HeatingCalculations.VentilationHeating.ResultEnergyForHeatingBaseLine * heatedArea;
			num5 = buldingZone.HeatingCalculations.VentilationHeating.ResultEnergyForHeatingESM * heatedArea;
			zoneResults.NetEnergyTable.HeatingVentilation.ESMArea += num5;
			zoneResults.NetEnergyTable.HeatingVentilation.ESM += buldingZone.HeatingCalculations.VentilationHeating.ResultEnergyForHeatingESM * heatedArea;
		}
		if (buldingZone.HasCooling)
		{
			double num6 = buldingZone.CoolingCalculations.CoolingResult.ResulNetEnergyRef1 * heatedArea;
			zoneResults.NetEnergyTable.Cooling.Ref1Area += num6;
			zoneResults.NetEnergyTable.Cooling.Ref1 += buldingZone.CoolingCalculations.CoolingResult.ResulNetEnergyRef1 * heatedArea;
			double num7 = buldingZone.CoolingCalculations.CoolingResult.ResulNetEnergyRef2 * heatedArea;
			zoneResults.NetEnergyTable.Cooling.Ref2Area += num7;
			zoneResults.NetEnergyTable.Cooling.Ref2 += buldingZone.CoolingCalculations.CoolingResult.ResulNetEnergyRef2 * heatedArea;
			double num8 = buldingZone.CoolingCalculations.CoolingResult.ResulNetEnergyActual * heatedArea;
			zoneResults.NetEnergyTable.Cooling.ActualArea += num8;
			zoneResults.NetEnergyTable.Cooling.Actual += buldingZone.CoolingCalculations.CoolingResult.ResulNetEnergyActual * heatedArea;
			double num9 = buldingZone.CoolingCalculations.CoolingResult.ResulNetEnergyBaseLine * heatedArea;
			zoneResults.NetEnergyTable.Cooling.BaseLineArea += num9;
			zoneResults.NetEnergyTable.Cooling.BaseLine += buldingZone.CoolingCalculations.CoolingResult.ResulNetEnergyBaseLine * heatedArea;
			double num10 = buldingZone.CoolingCalculations.CoolingResult.ResulNetEnergyESM * heatedArea;
			zoneResults.NetEnergyTable.Cooling.ESMArea += num10;
			zoneResults.NetEnergyTable.Cooling.ESM += buldingZone.CoolingCalculations.CoolingResult.ResulNetEnergyESM * heatedArea;
			num6 = buldingZone.CoolingCalculations.VentilationCooling.ResultEnergyForCoolingRef1 * heatedArea;
			zoneResults.NetEnergyTable.CoolingVentilation.Ref1Area += num6;
			zoneResults.NetEnergyTable.CoolingVentilation.Ref1 += buldingZone.CoolingCalculations.VentilationCooling.ResultEnergyForCoolingRef1 * heatedArea;
			num7 = buldingZone.CoolingCalculations.VentilationCooling.ResultEnergyForCoolingRef2 * heatedArea;
			zoneResults.NetEnergyTable.CoolingVentilation.Ref2Area += num7;
			zoneResults.NetEnergyTable.CoolingVentilation.Ref2 += buldingZone.CoolingCalculations.VentilationCooling.ResultEnergyForCoolingRef2 * heatedArea;
			num8 = buldingZone.CoolingCalculations.VentilationCooling.ResultEnergyForCoolingActual * heatedArea;
			zoneResults.NetEnergyTable.CoolingVentilation.ActualArea += num8;
			zoneResults.NetEnergyTable.CoolingVentilation.Actual += buldingZone.CoolingCalculations.VentilationCooling.ResultEnergyForCoolingActual * heatedArea;
			num9 = buldingZone.CoolingCalculations.VentilationCooling.ResultEnergyForCoolingBaseLine * heatedArea;
			zoneResults.NetEnergyTable.CoolingVentilation.BaseLineArea += num9;
			zoneResults.NetEnergyTable.CoolingVentilation.BaseLine += buldingZone.CoolingCalculations.VentilationCooling.ResultEnergyForCoolingBaseLine * heatedArea;
			num10 = buldingZone.CoolingCalculations.VentilationCooling.ResultEnergyForCoolingESM * heatedArea;
			zoneResults.NetEnergyTable.CoolingVentilation.ESMArea += num10;
			zoneResults.NetEnergyTable.CoolingVentilation.ESM += buldingZone.CoolingCalculations.VentilationCooling.ResultEnergyForCoolingESM * heatedArea;
		}
		zoneResults.NetEnergyTable.Total.Ref1Area = (zoneResults.NetEnergyTable.Heating.Ref1Area + zoneResults.NetEnergyTable.HeatingVentilation.Ref1Area + zoneResults.NetEnergyTable.Cooling.Ref1Area + zoneResults.NetEnergyTable.CoolingVentilation.Ref1Area) / heatedArea;
		zoneResults.NetEnergyTable.Total.Ref1 = zoneResults.NetEnergyTable.Heating.Ref1 + zoneResults.NetEnergyTable.HeatingVentilation.Ref1 + zoneResults.NetEnergyTable.Cooling.Ref1 + zoneResults.NetEnergyTable.CoolingVentilation.Ref1;
		zoneResults.NetEnergyTable.Total.Ref2Area = (zoneResults.NetEnergyTable.Heating.Ref2Area + zoneResults.NetEnergyTable.HeatingVentilation.Ref2Area + zoneResults.NetEnergyTable.Cooling.Ref2Area + zoneResults.NetEnergyTable.CoolingVentilation.Ref2Area) / heatedArea;
		zoneResults.NetEnergyTable.Total.Ref2 = zoneResults.NetEnergyTable.Heating.Ref2 + zoneResults.NetEnergyTable.HeatingVentilation.Ref2 + zoneResults.NetEnergyTable.Cooling.Ref2 + zoneResults.NetEnergyTable.CoolingVentilation.Ref2;
		zoneResults.NetEnergyTable.Total.ActualArea = (zoneResults.NetEnergyTable.Heating.ActualArea + zoneResults.NetEnergyTable.HeatingVentilation.ActualArea + zoneResults.NetEnergyTable.Cooling.ActualArea + zoneResults.NetEnergyTable.CoolingVentilation.ActualArea) / heatedArea;
		zoneResults.NetEnergyTable.Total.Actual = zoneResults.NetEnergyTable.Heating.Actual + zoneResults.NetEnergyTable.HeatingVentilation.Actual + zoneResults.NetEnergyTable.Cooling.Actual + zoneResults.NetEnergyTable.CoolingVentilation.Actual;
		zoneResults.NetEnergyTable.Total.BaseLine = zoneResults.NetEnergyTable.Heating.BaseLine + zoneResults.NetEnergyTable.HeatingVentilation.BaseLine + zoneResults.NetEnergyTable.Cooling.BaseLine + zoneResults.NetEnergyTable.CoolingVentilation.BaseLine;
		zoneResults.NetEnergyTable.Total.BaseLineArea = (zoneResults.NetEnergyTable.Heating.BaseLineArea + zoneResults.NetEnergyTable.HeatingVentilation.BaseLineArea + zoneResults.NetEnergyTable.Cooling.BaseLineArea + zoneResults.NetEnergyTable.CoolingVentilation.BaseLineArea) / heatedArea;
		zoneResults.NetEnergyTable.Total.ESM = zoneResults.NetEnergyTable.Heating.ESM + zoneResults.NetEnergyTable.HeatingVentilation.ESM + zoneResults.NetEnergyTable.Cooling.ESM + zoneResults.NetEnergyTable.CoolingVentilation.ESM;
		zoneResults.NetEnergyTable.Total.ESMArea = (zoneResults.NetEnergyTable.Heating.ESMArea + zoneResults.NetEnergyTable.HeatingVentilation.ESMArea + zoneResults.NetEnergyTable.Cooling.ESMArea + zoneResults.NetEnergyTable.CoolingVentilation.ESMArea) / heatedArea;
	}

	private static void ClearNeededVEIenergy(Results zoneResults)
	{
		zoneResults.NeededEnergyTable.Heating.VEI = 0.0;
		zoneResults.NeededEnergyTable.Heating.GeneralVEI = 0.0;
		zoneResults.NeededEnergyTable.HeatingVentilation.VEI = 0.0;
		zoneResults.NeededEnergyTable.HeatingVentilation.GeneralVEI = 0.0;
		zoneResults.NeededEnergyTable.BGV.VEI = 0.0;
		zoneResults.NeededEnergyTable.BGV.GeneralVEI = 0.0;
		zoneResults.NeededEnergyTable.Total.VEI = 0.0;
		zoneResults.NeededEnergyTable.Total.GeneralVEI = 0.0;
	}

	private static void ClearNetEnergy(Results zoneResults)
	{
		zoneResults.NetEnergyTable.Heating.Ref1Area = 0.0;
		zoneResults.NetEnergyTable.Heating.Ref1 = 0.0;
		zoneResults.NetEnergyTable.Heating.Ref2Area = 0.0;
		zoneResults.NetEnergyTable.Heating.Ref2 = 0.0;
		zoneResults.NetEnergyTable.Heating.ActualArea = 0.0;
		zoneResults.NetEnergyTable.Heating.Actual = 0.0;
		zoneResults.NetEnergyTable.Heating.BaseLineArea = 0.0;
		zoneResults.NetEnergyTable.Heating.BaseLine = 0.0;
		zoneResults.NetEnergyTable.Heating.ESMArea = 0.0;
		zoneResults.NetEnergyTable.Heating.ESM = 0.0;
		zoneResults.NetEnergyTable.Cooling.Ref1Area = 0.0;
		zoneResults.NetEnergyTable.Cooling.Ref1 = 0.0;
		zoneResults.NetEnergyTable.Cooling.Ref2Area = 0.0;
		zoneResults.NetEnergyTable.Cooling.Ref2 = 0.0;
		zoneResults.NetEnergyTable.Cooling.ActualArea = 0.0;
		zoneResults.NetEnergyTable.Cooling.Actual = 0.0;
		zoneResults.NetEnergyTable.Cooling.BaseLineArea = 0.0;
		zoneResults.NetEnergyTable.Cooling.BaseLine = 0.0;
		zoneResults.NetEnergyTable.Cooling.ESMArea = 0.0;
		zoneResults.NetEnergyTable.Cooling.ESM = 0.0;
		zoneResults.NetEnergyTable.HeatingVentilation.Ref1Area = 0.0;
		zoneResults.NetEnergyTable.HeatingVentilation.Ref1 = 0.0;
		zoneResults.NetEnergyTable.HeatingVentilation.Ref2Area = 0.0;
		zoneResults.NetEnergyTable.HeatingVentilation.Ref2 = 0.0;
		zoneResults.NetEnergyTable.HeatingVentilation.ActualArea = 0.0;
		zoneResults.NetEnergyTable.HeatingVentilation.Actual = 0.0;
		zoneResults.NetEnergyTable.HeatingVentilation.BaseLineArea = 0.0;
		zoneResults.NetEnergyTable.HeatingVentilation.BaseLine = 0.0;
		zoneResults.NetEnergyTable.HeatingVentilation.ESMArea = 0.0;
		zoneResults.NetEnergyTable.HeatingVentilation.ESM = 0.0;
		zoneResults.NetEnergyTable.CoolingVentilation.Ref1Area = 0.0;
		zoneResults.NetEnergyTable.CoolingVentilation.Ref1 = 0.0;
		zoneResults.NetEnergyTable.CoolingVentilation.Ref2Area = 0.0;
		zoneResults.NetEnergyTable.CoolingVentilation.Ref2 = 0.0;
		zoneResults.NetEnergyTable.CoolingVentilation.ActualArea = 0.0;
		zoneResults.NetEnergyTable.CoolingVentilation.Actual = 0.0;
		zoneResults.NetEnergyTable.CoolingVentilation.BaseLineArea = 0.0;
		zoneResults.NetEnergyTable.CoolingVentilation.BaseLine = 0.0;
		zoneResults.NetEnergyTable.CoolingVentilation.ESMArea = 0.0;
		zoneResults.NetEnergyTable.CoolingVentilation.ESM = 0.0;
		zoneResults.NetEnergyTable.Total.Ref1Area = 0.0;
		zoneResults.NetEnergyTable.Total.Ref1 = 0.0;
		zoneResults.NetEnergyTable.Total.Ref2Area = 0.0;
		zoneResults.NetEnergyTable.Total.Ref2 = 0.0;
		zoneResults.NetEnergyTable.Total.ActualArea = 0.0;
		zoneResults.NetEnergyTable.Total.Actual = 0.0;
		zoneResults.NetEnergyTable.Total.BaseLineArea = 0.0;
		zoneResults.NetEnergyTable.Total.BaseLine = 0.0;
		zoneResults.NetEnergyTable.Total.ESMArea = 0.0;
		zoneResults.NetEnergyTable.Total.ESM = 0.0;
	}

	private static void CalculateNetWithoutInputsEnergyByTechnologies(Results zoneResults, BuildingZone buldingZone)
	{
		double heatedArea = buldingZone.Heating.Area.HeatedArea;
		if (buldingZone.HasHeating)
		{
			zoneResults.NoInputsNetEnergyTable.Heating.Ref1Area = CheckForNaN(buldingZone.HeatingCalculations.HeatingResult.ResulNoInputsNetEnergyRef1);
			zoneResults.NoInputsNetEnergyTable.Heating.Ref1 += CheckForNaN(buldingZone.HeatingCalculations.HeatingResult.ResulNoInputsNetEnergyRef1) * heatedArea;
			zoneResults.NoInputsNetEnergyTable.Heating.Ref2Area = CheckForNaN(buldingZone.HeatingCalculations.HeatingResult.ResulNoInputsNetEnergyRef2);
			zoneResults.NoInputsNetEnergyTable.Heating.Ref2 += CheckForNaN(buldingZone.HeatingCalculations.HeatingResult.ResulNoInputsNetEnergyRef2) * heatedArea;
			zoneResults.NoInputsNetEnergyTable.Heating.ActualArea = CheckForNaN(buldingZone.HeatingCalculations.HeatingResult.ResulNoInputsNetEnergyActual);
			zoneResults.NoInputsNetEnergyTable.Heating.Actual += CheckForNaN(buldingZone.HeatingCalculations.HeatingResult.ResulNoInputsNetEnergyActual) * heatedArea;
			zoneResults.NoInputsNetEnergyTable.Heating.BaseLineArea = CheckForNaN(buldingZone.HeatingCalculations.HeatingResult.ResulNoInputsNetEnergyBaseLine);
			zoneResults.NoInputsNetEnergyTable.Heating.BaseLine += CheckForNaN(buldingZone.HeatingCalculations.HeatingResult.ResulNoInputsNetEnergyBaseLine) * heatedArea;
			zoneResults.NoInputsNetEnergyTable.Heating.ESMArea = CheckForNaN(buldingZone.HeatingCalculations.HeatingResult.ResulNoInputsNetEnergyESM);
			zoneResults.NoInputsNetEnergyTable.Heating.ESM += CheckForNaN(buldingZone.HeatingCalculations.HeatingResult.ResulNoInputsNetEnergyESM) * heatedArea;
			zoneResults.NoInputsNetEnergyTable.HeatingVentilation.Ref1Area = CheckForNaN(buldingZone.HeatingCalculations.VentilationHeating.ResultEnergyForHeatingRef1);
			zoneResults.NoInputsNetEnergyTable.HeatingVentilation.Ref1 += CheckForNaN(buldingZone.HeatingCalculations.VentilationHeating.ResultEnergyForHeatingRef1) * heatedArea;
			zoneResults.NoInputsNetEnergyTable.HeatingVentilation.Ref2Area = CheckForNaN(buldingZone.HeatingCalculations.VentilationHeating.ResultEnergyForHeatingRef2);
			zoneResults.NoInputsNetEnergyTable.HeatingVentilation.Ref2 += CheckForNaN(buldingZone.HeatingCalculations.VentilationHeating.ResultEnergyForHeatingRef2) * heatedArea;
			zoneResults.NoInputsNetEnergyTable.HeatingVentilation.ActualArea = CheckForNaN(buldingZone.HeatingCalculations.VentilationHeating.ResultEnergyForHeatingActual);
			zoneResults.NoInputsNetEnergyTable.HeatingVentilation.Actual += CheckForNaN(buldingZone.HeatingCalculations.VentilationHeating.ResultEnergyForHeatingActual) * heatedArea;
			zoneResults.NoInputsNetEnergyTable.HeatingVentilation.BaseLineArea = CheckForNaN(buldingZone.HeatingCalculations.VentilationHeating.ResultEnergyForHeatingBaseLine);
			zoneResults.NoInputsNetEnergyTable.HeatingVentilation.BaseLine += CheckForNaN(buldingZone.HeatingCalculations.VentilationHeating.ResultEnergyForHeatingBaseLine) * heatedArea;
			zoneResults.NoInputsNetEnergyTable.HeatingVentilation.ESMArea = CheckForNaN(buldingZone.HeatingCalculations.VentilationHeating.ResultEnergyForHeatingESM);
			zoneResults.NoInputsNetEnergyTable.HeatingVentilation.ESM += CheckForNaN(buldingZone.HeatingCalculations.VentilationHeating.ResultEnergyForHeatingESM) * heatedArea;
		}
		if (buldingZone.HasCooling)
		{
			zoneResults.NoInputsNetEnergyTable.Cooling.Ref1Area = CheckForNaN(buldingZone.CoolingCalculations.CoolingResult.ResulNoInputsNetEnergyRef1);
			zoneResults.NoInputsNetEnergyTable.Cooling.Ref1 += CheckForNaN(buldingZone.CoolingCalculations.CoolingResult.ResulNoInputsNetEnergyRef1) * heatedArea;
			zoneResults.NoInputsNetEnergyTable.Cooling.Ref2Area = CheckForNaN(buldingZone.CoolingCalculations.CoolingResult.ResulNoInputsNetEnergyRef2);
			zoneResults.NoInputsNetEnergyTable.Cooling.Ref2 += CheckForNaN(buldingZone.CoolingCalculations.CoolingResult.ResulNoInputsNetEnergyRef2) * heatedArea;
			zoneResults.NoInputsNetEnergyTable.Cooling.ActualArea = CheckForNaN(buldingZone.CoolingCalculations.CoolingResult.ResulNoInputsNetEnergyActual);
			zoneResults.NoInputsNetEnergyTable.Cooling.Actual += CheckForNaN(buldingZone.CoolingCalculations.CoolingResult.ResulNoInputsNetEnergyActual) * heatedArea;
			zoneResults.NoInputsNetEnergyTable.Cooling.BaseLineArea = CheckForNaN(buldingZone.CoolingCalculations.CoolingResult.ResulNoInputsNetEnergyBaseLine);
			zoneResults.NoInputsNetEnergyTable.Cooling.BaseLine += CheckForNaN(buldingZone.CoolingCalculations.CoolingResult.ResulNoInputsNetEnergyBaseLine) * heatedArea;
			zoneResults.NoInputsNetEnergyTable.Cooling.ESMArea = CheckForNaN(buldingZone.CoolingCalculations.CoolingResult.ResulNoInputsNetEnergyESM);
			zoneResults.NoInputsNetEnergyTable.Cooling.ESM += CheckForNaN(buldingZone.CoolingCalculations.CoolingResult.ResulNoInputsNetEnergyESM) * heatedArea;
			zoneResults.NoInputsNetEnergyTable.CoolingVentilation.Ref1Area = CheckForNaN(buldingZone.CoolingCalculations.VentilationCooling.ResultEnergyForCoolingRef1);
			zoneResults.NoInputsNetEnergyTable.CoolingVentilation.Ref1 += CheckForNaN(buldingZone.CoolingCalculations.VentilationCooling.ResultEnergyForCoolingRef1) * heatedArea;
			zoneResults.NoInputsNetEnergyTable.CoolingVentilation.Ref2Area = CheckForNaN(buldingZone.CoolingCalculations.VentilationCooling.ResultEnergyForCoolingRef2);
			zoneResults.NoInputsNetEnergyTable.CoolingVentilation.Ref2 += CheckForNaN(buldingZone.CoolingCalculations.VentilationCooling.ResultEnergyForCoolingRef2) * heatedArea;
			zoneResults.NoInputsNetEnergyTable.CoolingVentilation.ActualArea = CheckForNaN(buldingZone.CoolingCalculations.VentilationCooling.ResultEnergyForCoolingActual);
			zoneResults.NoInputsNetEnergyTable.CoolingVentilation.Actual += CheckForNaN(buldingZone.CoolingCalculations.VentilationCooling.ResultEnergyForCoolingActual) * heatedArea;
			zoneResults.NoInputsNetEnergyTable.CoolingVentilation.BaseLineArea = CheckForNaN(buldingZone.CoolingCalculations.VentilationCooling.ResultEnergyForCoolingBaseLine);
			zoneResults.NoInputsNetEnergyTable.CoolingVentilation.BaseLine += CheckForNaN(buldingZone.CoolingCalculations.VentilationCooling.ResultEnergyForCoolingBaseLine) * heatedArea;
			zoneResults.NoInputsNetEnergyTable.CoolingVentilation.ESMArea = CheckForNaN(buldingZone.CoolingCalculations.VentilationCooling.ResultEnergyForCoolingESM);
			zoneResults.NoInputsNetEnergyTable.CoolingVentilation.ESM += CheckForNaN(buldingZone.CoolingCalculations.VentilationCooling.ResultEnergyForCoolingESM) * heatedArea;
		}
		zoneResults.NoInputsNetEnergyTable.Total.Ref1Area = zoneResults.NoInputsNetEnergyTable.Heating.Ref1Area + zoneResults.NoInputsNetEnergyTable.HeatingVentilation.Ref1Area + zoneResults.NoInputsNetEnergyTable.Cooling.Ref1Area + zoneResults.NoInputsNetEnergyTable.CoolingVentilation.Ref1Area;
		zoneResults.NoInputsNetEnergyTable.Total.Ref2Area = zoneResults.NoInputsNetEnergyTable.Heating.Ref2Area + zoneResults.NoInputsNetEnergyTable.HeatingVentilation.Ref2Area + zoneResults.NoInputsNetEnergyTable.Cooling.Ref2Area + zoneResults.NoInputsNetEnergyTable.CoolingVentilation.Ref2Area;
		zoneResults.NoInputsNetEnergyTable.Total.Ref1 = zoneResults.NoInputsNetEnergyTable.Heating.Ref1 + zoneResults.NoInputsNetEnergyTable.HeatingVentilation.Ref1 + zoneResults.NoInputsNetEnergyTable.Cooling.Ref1 + zoneResults.NoInputsNetEnergyTable.CoolingVentilation.Ref1;
		zoneResults.NoInputsNetEnergyTable.Total.Ref2 = zoneResults.NoInputsNetEnergyTable.Heating.Ref2 + zoneResults.NoInputsNetEnergyTable.HeatingVentilation.Ref2 + zoneResults.NoInputsNetEnergyTable.Cooling.Ref2 + zoneResults.NoInputsNetEnergyTable.CoolingVentilation.Ref2;
		zoneResults.NoInputsNetEnergyTable.Total.ActualArea = zoneResults.NoInputsNetEnergyTable.Heating.ActualArea + zoneResults.NoInputsNetEnergyTable.HeatingVentilation.ActualArea + zoneResults.NoInputsNetEnergyTable.Cooling.ActualArea + zoneResults.NoInputsNetEnergyTable.CoolingVentilation.ActualArea;
		zoneResults.NoInputsNetEnergyTable.Total.Actual = zoneResults.NoInputsNetEnergyTable.Heating.Actual + zoneResults.NoInputsNetEnergyTable.HeatingVentilation.Actual + zoneResults.NoInputsNetEnergyTable.Cooling.Actual + zoneResults.NoInputsNetEnergyTable.CoolingVentilation.Actual;
		zoneResults.NoInputsNetEnergyTable.Total.BaseLine = zoneResults.NoInputsNetEnergyTable.Heating.BaseLine + zoneResults.NoInputsNetEnergyTable.HeatingVentilation.BaseLine + zoneResults.NoInputsNetEnergyTable.Cooling.BaseLine + zoneResults.NoInputsNetEnergyTable.CoolingVentilation.BaseLine;
		zoneResults.NoInputsNetEnergyTable.Total.BaseLineArea = zoneResults.NoInputsNetEnergyTable.Heating.BaseLineArea + zoneResults.NoInputsNetEnergyTable.HeatingVentilation.BaseLineArea + zoneResults.NoInputsNetEnergyTable.Cooling.BaseLineArea + zoneResults.NoInputsNetEnergyTable.CoolingVentilation.BaseLineArea;
		zoneResults.NoInputsNetEnergyTable.Total.ESM = zoneResults.NoInputsNetEnergyTable.Heating.ESM + zoneResults.NoInputsNetEnergyTable.HeatingVentilation.ESM + zoneResults.NoInputsNetEnergyTable.Cooling.ESM + zoneResults.NoInputsNetEnergyTable.CoolingVentilation.ESM;
		zoneResults.NoInputsNetEnergyTable.Total.ESMArea = zoneResults.NoInputsNetEnergyTable.Heating.ESMArea + zoneResults.NoInputsNetEnergyTable.HeatingVentilation.ESMArea + zoneResults.NoInputsNetEnergyTable.Cooling.ESMArea + zoneResults.NoInputsNetEnergyTable.CoolingVentilation.ESMArea;
	}

	private static void ClearNetEnergyWithoutInputs(Results zoneResults)
	{
		zoneResults.NoInputsNetEnergyTable.Heating.Ref1Area = 0.0;
		zoneResults.NoInputsNetEnergyTable.Heating.Ref1 = 0.0;
		zoneResults.NoInputsNetEnergyTable.Heating.Ref2Area = 0.0;
		zoneResults.NoInputsNetEnergyTable.Heating.Ref2 = 0.0;
		zoneResults.NoInputsNetEnergyTable.Heating.ActualArea = 0.0;
		zoneResults.NoInputsNetEnergyTable.Heating.Actual = 0.0;
		zoneResults.NoInputsNetEnergyTable.Heating.BaseLineArea = 0.0;
		zoneResults.NoInputsNetEnergyTable.Heating.BaseLine = 0.0;
		zoneResults.NoInputsNetEnergyTable.Heating.ESMArea = 0.0;
		zoneResults.NoInputsNetEnergyTable.Heating.ESM = 0.0;
		zoneResults.NoInputsNetEnergyTable.Cooling.Ref1 = 0.0;
		zoneResults.NoInputsNetEnergyTable.Cooling.Ref1Area = 0.0;
		zoneResults.NoInputsNetEnergyTable.Cooling.Ref2 = 0.0;
		zoneResults.NoInputsNetEnergyTable.Cooling.Ref2Area = 0.0;
		zoneResults.NoInputsNetEnergyTable.Cooling.ActualArea = 0.0;
		zoneResults.NoInputsNetEnergyTable.Cooling.Actual = 0.0;
		zoneResults.NoInputsNetEnergyTable.Cooling.BaseLineArea = 0.0;
		zoneResults.NoInputsNetEnergyTable.Cooling.BaseLine = 0.0;
		zoneResults.NoInputsNetEnergyTable.Cooling.ESMArea = 0.0;
		zoneResults.NoInputsNetEnergyTable.Cooling.ESM = 0.0;
		zoneResults.NoInputsNetEnergyTable.HeatingVentilation.Ref1 = 0.0;
		zoneResults.NoInputsNetEnergyTable.HeatingVentilation.Ref2 = 0.0;
		zoneResults.NoInputsNetEnergyTable.HeatingVentilation.ActualArea = 0.0;
		zoneResults.NoInputsNetEnergyTable.HeatingVentilation.Actual = 0.0;
		zoneResults.NoInputsNetEnergyTable.HeatingVentilation.BaseLineArea = 0.0;
		zoneResults.NoInputsNetEnergyTable.HeatingVentilation.BaseLine = 0.0;
		zoneResults.NoInputsNetEnergyTable.HeatingVentilation.ESMArea = 0.0;
		zoneResults.NoInputsNetEnergyTable.HeatingVentilation.ESM = 0.0;
		zoneResults.NoInputsNetEnergyTable.CoolingVentilation.Ref1Area = 0.0;
		zoneResults.NoInputsNetEnergyTable.CoolingVentilation.Ref1 = 0.0;
		zoneResults.NoInputsNetEnergyTable.CoolingVentilation.Ref2Area = 0.0;
		zoneResults.NoInputsNetEnergyTable.CoolingVentilation.Ref2 = 0.0;
		zoneResults.NoInputsNetEnergyTable.CoolingVentilation.ActualArea = 0.0;
		zoneResults.NoInputsNetEnergyTable.CoolingVentilation.Actual = 0.0;
		zoneResults.NoInputsNetEnergyTable.CoolingVentilation.BaseLineArea = 0.0;
		zoneResults.NoInputsNetEnergyTable.CoolingVentilation.BaseLine = 0.0;
		zoneResults.NoInputsNetEnergyTable.CoolingVentilation.ESMArea = 0.0;
		zoneResults.NoInputsNetEnergyTable.CoolingVentilation.ESM = 0.0;
		zoneResults.NoInputsNetEnergyTable.Total.Ref1Area = 0.0;
		zoneResults.NoInputsNetEnergyTable.Total.Ref1 = 0.0;
		zoneResults.NoInputsNetEnergyTable.Total.Ref2Area = 0.0;
		zoneResults.NoInputsNetEnergyTable.Total.Ref2 = 0.0;
		zoneResults.NoInputsNetEnergyTable.Total.ActualArea = 0.0;
		zoneResults.NoInputsNetEnergyTable.Total.Actual = 0.0;
		zoneResults.NoInputsNetEnergyTable.Total.BaseLineArea = 0.0;
		zoneResults.NoInputsNetEnergyTable.Total.BaseLine = 0.0;
		zoneResults.NoInputsNetEnergyTable.Total.ESMArea = 0.0;
		zoneResults.NoInputsNetEnergyTable.Total.ESM = 0.0;
	}

	private static void ClearValuesCO2(Results zoneBalanceResult)
	{
		ClearValuesCO2Ref1(zoneBalanceResult);
		ClearValuesCO2Ref2(zoneBalanceResult);
		ClearValuesCO2Actual(zoneBalanceResult);
		ClearValuesCO2BaseLine(zoneBalanceResult);
		ClearValuesCO2ESM(zoneBalanceResult);
	}

	private static void ClearValuesCO2Ref1(Results zoneBalanceResult)
	{
		GeneralResultTable emissionNeededEnergyTable = zoneBalanceResult.EmissionNeededEnergyTable;
		emissionNeededEnergyTable.Total.Ref1 = 0.0;
		emissionNeededEnergyTable.Heating.Ref1 = 0.0;
		emissionNeededEnergyTable.Cooling.Ref1 = 0.0;
		emissionNeededEnergyTable.CoolingVentilation.Ref1 = 0.0;
		emissionNeededEnergyTable.HeatingVentilation.Ref1 = 0.0;
		emissionNeededEnergyTable.BGV.Ref1 = 0.0;
		emissionNeededEnergyTable.BGVPumps.Ref1 = 0.0;
		emissionNeededEnergyTable.FansAndPumps.Ref1 = 0.0;
		emissionNeededEnergyTable.Lights.Ref1 = 0.0;
		emissionNeededEnergyTable.HeatAffectingDevices.Ref1 = 0.0;
		emissionNeededEnergyTable.NonHeatAffectingDevices.Ref1 = 0.0;
		emissionNeededEnergyTable.Other.Ref1 = 0.0;
	}

	private static void ClearValuesCO2Ref2(Results zoneBalanceResult)
	{
		GeneralResultTable emissionNeededEnergyTable = zoneBalanceResult.EmissionNeededEnergyTable;
		emissionNeededEnergyTable.Total.Ref2 = 0.0;
		emissionNeededEnergyTable.Heating.Ref2 = 0.0;
		emissionNeededEnergyTable.Cooling.Ref2 = 0.0;
		emissionNeededEnergyTable.CoolingVentilation.Ref2 = 0.0;
		emissionNeededEnergyTable.HeatingVentilation.Ref2 = 0.0;
		emissionNeededEnergyTable.BGV.Ref2 = 0.0;
		emissionNeededEnergyTable.BGVPumps.Ref2 = 0.0;
		emissionNeededEnergyTable.FansAndPumps.Ref2 = 0.0;
		emissionNeededEnergyTable.Lights.Ref2 = 0.0;
		emissionNeededEnergyTable.HeatAffectingDevices.Ref2 = 0.0;
		emissionNeededEnergyTable.NonHeatAffectingDevices.Ref2 = 0.0;
		emissionNeededEnergyTable.Other.Ref2 = 0.0;
	}

	private static void ClearValuesCO2Actual(Results zoneBalanceResult)
	{
		GeneralResultTable emissionNeededEnergyTable = zoneBalanceResult.EmissionNeededEnergyTable;
		emissionNeededEnergyTable.Total.Actual = 0.0;
		emissionNeededEnergyTable.Heating.Actual = 0.0;
		emissionNeededEnergyTable.Cooling.Actual = 0.0;
		emissionNeededEnergyTable.CoolingVentilation.Actual = 0.0;
		emissionNeededEnergyTable.HeatingVentilation.Actual = 0.0;
		emissionNeededEnergyTable.BGV.Actual = 0.0;
		emissionNeededEnergyTable.BGVPumps.Actual = 0.0;
		emissionNeededEnergyTable.FansAndPumps.Actual = 0.0;
		emissionNeededEnergyTable.Lights.Actual = 0.0;
		emissionNeededEnergyTable.HeatAffectingDevices.Actual = 0.0;
		emissionNeededEnergyTable.NonHeatAffectingDevices.Actual = 0.0;
		emissionNeededEnergyTable.Other.Actual = 0.0;
	}

	private static void ClearValuesCO2BaseLine(Results zoneBalanceResult)
	{
		GeneralResultTable emissionNeededEnergyTable = zoneBalanceResult.EmissionNeededEnergyTable;
		emissionNeededEnergyTable.Total.BaseLine = 0.0;
		emissionNeededEnergyTable.Heating.BaseLine = 0.0;
		emissionNeededEnergyTable.Cooling.BaseLine = 0.0;
		emissionNeededEnergyTable.CoolingVentilation.BaseLine = 0.0;
		emissionNeededEnergyTable.HeatingVentilation.BaseLine = 0.0;
		emissionNeededEnergyTable.BGV.BaseLine = 0.0;
		emissionNeededEnergyTable.BGVPumps.BaseLine = 0.0;
		emissionNeededEnergyTable.FansAndPumps.BaseLine = 0.0;
		emissionNeededEnergyTable.Lights.BaseLine = 0.0;
		emissionNeededEnergyTable.HeatAffectingDevices.BaseLine = 0.0;
		emissionNeededEnergyTable.NonHeatAffectingDevices.BaseLine = 0.0;
		emissionNeededEnergyTable.Other.BaseLine = 0.0;
	}

	private static void ClearValuesCO2ESM(Results zoneBalanceResult)
	{
		GeneralResultTable emissionNeededEnergyTable = zoneBalanceResult.EmissionNeededEnergyTable;
		emissionNeededEnergyTable.Total.ESM = 0.0;
		emissionNeededEnergyTable.Heating.ESM = 0.0;
		emissionNeededEnergyTable.Cooling.ESM = 0.0;
		emissionNeededEnergyTable.CoolingVentilation.ESM = 0.0;
		emissionNeededEnergyTable.HeatingVentilation.ESM = 0.0;
		emissionNeededEnergyTable.BGV.ESM = 0.0;
		emissionNeededEnergyTable.BGVPumps.ESM = 0.0;
		emissionNeededEnergyTable.FansAndPumps.ESM = 0.0;
		emissionNeededEnergyTable.Lights.ESM = 0.0;
		emissionNeededEnergyTable.HeatAffectingDevices.ESM = 0.0;
		emissionNeededEnergyTable.NonHeatAffectingDevices.ESM = 0.0;
		emissionNeededEnergyTable.Other.ESM = 0.0;
	}

	private static void CalculateCO2Emissions(Results zoneBalanceResult, BuildingZone buildZone, bool isBGVused)
	{
		CalculateCO2EmissionsRef1(zoneBalanceResult, buildZone, isBGVused);
		CalculateCO2EmissionsRef2(zoneBalanceResult, buildZone, isBGVused);
		CalculateCO2EmissionsActual(zoneBalanceResult, buildZone, isBGVused);
		CalculateCO2EmissionsBaseLine(zoneBalanceResult, buildZone, isBGVused);
		CalculateCO2EmissionsESM(zoneBalanceResult, buildZone, isBGVused);
	}

	private static void CalculateCO2EmissionsRef1(Results zoneBalanceResult, BuildingZone buildZone, bool isBGVused)
	{
		double heatedArea = buildZone.Heating.Area.HeatedArea;
		double ekoCoeficient = GetEkoCoeficient(buildZone.HeatingCalculations.HeatingResult.Fuel1Ref1, buildZone.HeatingCalculations.HeatingResult.ResultSourceEnergyRef1);
		double ekoCoeficient2 = GetEkoCoeficient(buildZone.HeatingCalculations.HeatingResult.Fuel2Ref1, buildZone.HeatingCalculations.HeatingResult.ResultSourceEnergy2Ref1);
		zoneBalanceResult.EmissionNeededEnergyTable.Heating.Ref1 += (ekoCoeficient + ekoCoeficient2) * heatedArea / 1000000.0;
		ekoCoeficient = GetEkoCoeficient(buildZone.HeatingCalculations.VentilationHeating.Fuel1Ref1, buildZone.HeatingCalculations.VentilationHeating.ResultSourceEnergyRef1);
		ekoCoeficient2 = GetEkoCoeficient(buildZone.HeatingCalculations.VentilationHeating.Fuel2Ref1, buildZone.HeatingCalculations.VentilationHeating.ResultSourceEnergy2Ref1);
		zoneBalanceResult.EmissionNeededEnergyTable.HeatingVentilation.Ref1 += (ekoCoeficient + ekoCoeficient2) * heatedArea / 1000000.0;
		ekoCoeficient = GetEkoCoeficient(buildZone.CoolingCalculations.CoolingResult.Fuel1Ref1, buildZone.CoolingCalculations.CoolingResult.ResultSourceEnergyRef1);
		ekoCoeficient2 = GetEkoCoeficient(buildZone.CoolingCalculations.CoolingResult.Fuel2Ref1, buildZone.CoolingCalculations.CoolingResult.ResultSourceEnergy2Ref1);
		zoneBalanceResult.EmissionNeededEnergyTable.Cooling.Ref1 += (ekoCoeficient + ekoCoeficient2) * heatedArea / 1000000.0;
		ekoCoeficient = GetEkoCoeficient(buildZone.CoolingCalculations.VentilationCooling.Fuel1Ref1, buildZone.CoolingCalculations.VentilationCooling.ResultSourceEnergyRef1);
		ekoCoeficient2 = GetEkoCoeficient(buildZone.CoolingCalculations.VentilationCooling.Fuel2Ref1, buildZone.CoolingCalculations.VentilationCooling.ResultSourceEnergy2Ref1);
		zoneBalanceResult.EmissionNeededEnergyTable.CoolingVentilation.Ref1 += (ekoCoeficient + ekoCoeficient2) * heatedArea / 1000000.0;
		if (isBGVused)
		{
			ekoCoeficient = GetEkoCoeficient(buildZone.HeatingCalculations.HotWaterCalculations.Fuel1Ref1, buildZone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergyRef1);
			ekoCoeficient2 = GetEkoCoeficient(buildZone.HeatingCalculations.HotWaterCalculations.Fuel2Ref1, buildZone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergy2Ref1);
			zoneBalanceResult.EmissionNeededEnergyTable.BGV.Ref1 += (ekoCoeficient + ekoCoeficient2) * heatedArea / 1000000.0;
			double ekoCoeficient3 = GetEkoCoeficient(Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.HotWaterPumps.General.DevicesNeededEnergyRef1);
			zoneBalanceResult.EmissionNeededEnergyTable.BGVPumps.Ref1 += ekoCoeficient3 * heatedArea / 1000000.0;
		}
		else
		{
			zoneBalanceResult.EmissionNeededEnergyTable.BGV.Ref1 = 0.0;
			zoneBalanceResult.EmissionNeededEnergyTable.BGVPumps.Ref1 = 0.0;
		}
		double ekoCoeficient4 = GetEkoCoeficient(Fuel.Fuel1, buildZone.HeatingCalculations.FansAndPumps.CoolNeededEnergyRef1 + buildZone.HeatingCalculations.FansAndPumps.PumpNeededEnergyRef1);
		zoneBalanceResult.EmissionNeededEnergyTable.FansAndPumps.Ref1 += ekoCoeficient4 * heatedArea / 1000000.0;
		ekoCoeficient4 = GetEkoCoeficient(Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.Lights.General.DevicesNeededEnergyRef1);
		zoneBalanceResult.EmissionNeededEnergyTable.Lights.Ref1 += ekoCoeficient4 * heatedArea / 1000000.0;
		ekoCoeficient4 = GetEkoCoeficient(Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.BalancedDevices.General.DevicesNeededEnergyRef1);
		zoneBalanceResult.EmissionNeededEnergyTable.HeatAffectingDevices.Ref1 += ekoCoeficient4 * heatedArea / 1000000.0;
		ekoCoeficient4 = GetEkoCoeficient(Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.NonBalancedDevices.General.DevicesNeededEnergyRef1);
		zoneBalanceResult.EmissionNeededEnergyTable.NonHeatAffectingDevices.Ref1 += ekoCoeficient4 * heatedArea / 1000000.0;
		ekoCoeficient4 = GetEkoCoeficient(Fuel.Fuel1, buildZone.HeatingCalculations.FansAndPumps.OtherResultCoolingRef1);
		zoneBalanceResult.EmissionNeededEnergyTable.Other.Ref1 += ekoCoeficient4 * heatedArea / 1000000.0;
		CalculateTotalCO2Ref1(zoneBalanceResult);
	}

	private static void CalculateTotalCO2Ref1(Results zoneBalanceResult)
	{
		GeneralResultTable emissionNeededEnergyTable = zoneBalanceResult.EmissionNeededEnergyTable;
		emissionNeededEnergyTable.Total.Ref1 = emissionNeededEnergyTable.Heating.Ref1 + emissionNeededEnergyTable.Cooling.Ref1 + emissionNeededEnergyTable.CoolingVentilation.Ref1 + emissionNeededEnergyTable.HeatingVentilation.Ref1 + emissionNeededEnergyTable.FansAndPumps.Ref1 + emissionNeededEnergyTable.BGV.Ref1 + emissionNeededEnergyTable.BGVPumps.Ref1 + emissionNeededEnergyTable.Lights.Ref1 + emissionNeededEnergyTable.HeatAffectingDevices.Ref1 + emissionNeededEnergyTable.NonHeatAffectingDevices.Ref1 + emissionNeededEnergyTable.Other.Ref1;
	}

	private static void CalculateCO2EmissionsRef2(Results zoneBalanceResult, BuildingZone buildZone, bool isBGVused)
	{
		double heatedArea = buildZone.Heating.Area.HeatedArea;
		double ekoCoeficient = GetEkoCoeficient(buildZone.HeatingCalculations.HeatingResult.Fuel1Ref2, buildZone.HeatingCalculations.HeatingResult.ResultSourceEnergyRef2);
		double ekoCoeficient2 = GetEkoCoeficient(buildZone.HeatingCalculations.HeatingResult.Fuel2Ref2, buildZone.HeatingCalculations.HeatingResult.ResultSourceEnergy2Ref2);
		zoneBalanceResult.EmissionNeededEnergyTable.Heating.Ref2 += (ekoCoeficient + ekoCoeficient2) * heatedArea / 1000000.0;
		ekoCoeficient = GetEkoCoeficient(buildZone.HeatingCalculations.VentilationHeating.Fuel1Ref2, buildZone.HeatingCalculations.VentilationHeating.ResultSourceEnergyRef2);
		ekoCoeficient2 = GetEkoCoeficient(buildZone.HeatingCalculations.VentilationHeating.Fuel2Ref2, buildZone.HeatingCalculations.VentilationHeating.ResultSourceEnergy2Ref2);
		zoneBalanceResult.EmissionNeededEnergyTable.HeatingVentilation.Ref2 += (ekoCoeficient + ekoCoeficient2) * heatedArea / 1000000.0;
		ekoCoeficient = GetEkoCoeficient(buildZone.CoolingCalculations.CoolingResult.Fuel1Ref2, buildZone.CoolingCalculations.CoolingResult.ResultSourceEnergyRef2);
		ekoCoeficient2 = GetEkoCoeficient(buildZone.CoolingCalculations.CoolingResult.Fuel2Ref2, buildZone.CoolingCalculations.CoolingResult.ResultSourceEnergy2Ref2);
		zoneBalanceResult.EmissionNeededEnergyTable.Cooling.Ref2 += (ekoCoeficient + ekoCoeficient2) * heatedArea / 1000000.0;
		ekoCoeficient = GetEkoCoeficient(buildZone.CoolingCalculations.VentilationCooling.Fuel1Ref2, buildZone.CoolingCalculations.VentilationCooling.ResultSourceEnergyRef2);
		ekoCoeficient2 = GetEkoCoeficient(buildZone.CoolingCalculations.VentilationCooling.Fuel2Ref2, buildZone.CoolingCalculations.VentilationCooling.ResultSourceEnergy2Ref2);
		zoneBalanceResult.EmissionNeededEnergyTable.CoolingVentilation.Ref2 += (ekoCoeficient + ekoCoeficient2) * heatedArea / 1000000.0;
		if (isBGVused)
		{
			ekoCoeficient = GetEkoCoeficient(buildZone.HeatingCalculations.HotWaterCalculations.Fuel1Ref2, buildZone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergyRef2);
			ekoCoeficient2 = GetEkoCoeficient(buildZone.HeatingCalculations.HotWaterCalculations.Fuel2Ref2, buildZone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergy2Ref2);
			zoneBalanceResult.EmissionNeededEnergyTable.BGV.Ref2 += (ekoCoeficient + ekoCoeficient2) * heatedArea / 1000000.0;
			double ekoCoeficient3 = GetEkoCoeficient(Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.HotWaterPumps.General.DevicesNeededEnergyRef2);
			zoneBalanceResult.EmissionNeededEnergyTable.BGVPumps.Ref2 += ekoCoeficient3 * heatedArea / 1000000.0;
		}
		else
		{
			zoneBalanceResult.EmissionNeededEnergyTable.BGV.Ref2 = 0.0;
			zoneBalanceResult.EmissionNeededEnergyTable.BGVPumps.Ref2 = 0.0;
		}
		double ekoCoeficient4 = GetEkoCoeficient(Fuel.Fuel1, buildZone.HeatingCalculations.FansAndPumps.CoolNeededEnergyRef2 + buildZone.HeatingCalculations.FansAndPumps.PumpNeededEnergyRef2);
		zoneBalanceResult.EmissionNeededEnergyTable.FansAndPumps.Ref2 += ekoCoeficient4 * heatedArea / 1000000.0;
		ekoCoeficient4 = GetEkoCoeficient(Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.Lights.General.DevicesNeededEnergyRef2);
		zoneBalanceResult.EmissionNeededEnergyTable.Lights.Ref2 += ekoCoeficient4 * heatedArea / 1000000.0;
		ekoCoeficient4 = GetEkoCoeficient(Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.BalancedDevices.General.DevicesNeededEnergyRef2);
		zoneBalanceResult.EmissionNeededEnergyTable.HeatAffectingDevices.Ref2 += ekoCoeficient4 * heatedArea / 1000000.0;
		ekoCoeficient4 = GetEkoCoeficient(Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.NonBalancedDevices.General.DevicesNeededEnergyRef2);
		zoneBalanceResult.EmissionNeededEnergyTable.NonHeatAffectingDevices.Ref2 += ekoCoeficient4 * heatedArea / 1000000.0;
		ekoCoeficient4 = GetEkoCoeficient(Fuel.Fuel1, buildZone.HeatingCalculations.FansAndPumps.OtherResultCoolingRef2);
		zoneBalanceResult.EmissionNeededEnergyTable.Other.Ref2 += ekoCoeficient4 * heatedArea / 1000000.0;
		CalculateTotalCO2Ref2(zoneBalanceResult);
	}

	private static void CalculateTotalCO2Ref2(Results zoneBalanceResult)
	{
		GeneralResultTable emissionNeededEnergyTable = zoneBalanceResult.EmissionNeededEnergyTable;
		emissionNeededEnergyTable.Total.Ref2 = emissionNeededEnergyTable.Heating.Ref2 + emissionNeededEnergyTable.Cooling.Ref2 + emissionNeededEnergyTable.CoolingVentilation.Ref2 + emissionNeededEnergyTable.HeatingVentilation.Ref2 + emissionNeededEnergyTable.FansAndPumps.Ref2 + emissionNeededEnergyTable.BGV.Ref2 + emissionNeededEnergyTable.BGVPumps.Ref2 + emissionNeededEnergyTable.Lights.Ref2 + emissionNeededEnergyTable.HeatAffectingDevices.Ref2 + emissionNeededEnergyTable.NonHeatAffectingDevices.Ref2 + emissionNeededEnergyTable.Other.Ref2;
	}

	private static void CalculateCO2EmissionsActual(Results zoneBalanceResult, BuildingZone buildZone, bool isBGVused)
	{
		double heatedArea = buildZone.Heating.Area.HeatedArea;
		if (buildZone.HasHeating)
		{
			double ekoCoeficient = GetEkoCoeficient(buildZone.HeatingCalculations.HeatingResult.Fuel1Actual, buildZone.HeatingCalculations.HeatingResult.ResultSourceEnergyActual);
			double ekoCoeficient2 = GetEkoCoeficient(buildZone.HeatingCalculations.HeatingResult.Fuel2Actual, buildZone.HeatingCalculations.HeatingResult.ResultSourceEnergy2Actual);
			zoneBalanceResult.EmissionNeededEnergyTable.Heating.Actual += (ekoCoeficient + ekoCoeficient2) * heatedArea / 1000000.0;
			ekoCoeficient = GetEkoCoeficient(buildZone.HeatingCalculations.VentilationHeating.Fuel1Actual, buildZone.HeatingCalculations.VentilationHeating.ResultSourceEnergyActual);
			ekoCoeficient2 = GetEkoCoeficient(buildZone.HeatingCalculations.VentilationHeating.Fuel2Actual, buildZone.HeatingCalculations.VentilationHeating.ResultSourceEnergy2Actual);
			zoneBalanceResult.EmissionNeededEnergyTable.HeatingVentilation.Actual += (ekoCoeficient + ekoCoeficient2) * heatedArea / 1000000.0;
		}
		if (buildZone.HasCooling)
		{
			double ekoCoeficient = GetEkoCoeficient(buildZone.CoolingCalculations.CoolingResult.Fuel1Actual, buildZone.CoolingCalculations.CoolingResult.ResultSourceEnergyActual);
			double ekoCoeficient2 = GetEkoCoeficient(buildZone.CoolingCalculations.CoolingResult.Fuel2Actual, buildZone.CoolingCalculations.CoolingResult.ResultSourceEnergy2Actual);
			zoneBalanceResult.EmissionNeededEnergyTable.Cooling.Actual += (ekoCoeficient + ekoCoeficient2) * heatedArea / 1000000.0;
			ekoCoeficient = GetEkoCoeficient(buildZone.CoolingCalculations.VentilationCooling.Fuel1Actual, buildZone.CoolingCalculations.VentilationCooling.ResultSourceEnergyActual);
			ekoCoeficient2 = GetEkoCoeficient(buildZone.CoolingCalculations.VentilationCooling.Fuel2Actual, buildZone.CoolingCalculations.VentilationCooling.ResultSourceEnergy2Actual);
			zoneBalanceResult.EmissionNeededEnergyTable.CoolingVentilation.Actual += (ekoCoeficient + ekoCoeficient2) * heatedArea / 1000000.0;
		}
		if (isBGVused)
		{
			double ekoCoeficient = GetEkoCoeficient(buildZone.HeatingCalculations.HotWaterCalculations.Fuel1Actual, buildZone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergyActual);
			double ekoCoeficient2 = GetEkoCoeficient(buildZone.HeatingCalculations.HotWaterCalculations.Fuel2Actual, buildZone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergy2Actual);
			zoneBalanceResult.EmissionNeededEnergyTable.BGV.Actual += (ekoCoeficient + ekoCoeficient2) * heatedArea / 1000000.0;
			double ekoCoeficient3 = GetEkoCoeficient(Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.HotWaterPumps.General.DevicesNeededEnergyActual);
			zoneBalanceResult.EmissionNeededEnergyTable.BGVPumps.Actual += ekoCoeficient3 * heatedArea / 1000000.0;
		}
		else
		{
			zoneBalanceResult.EmissionNeededEnergyTable.BGV.Actual = 0.0;
			zoneBalanceResult.EmissionNeededEnergyTable.BGVPumps.Actual = 0.0;
		}
		double ekoCoeficient4 = GetEkoCoeficient(Fuel.Fuel1, buildZone.HeatingCalculations.FansAndPumps.CoolNeededEnergyActual + buildZone.HeatingCalculations.FansAndPumps.PumpNeededEnergyActual);
		zoneBalanceResult.EmissionNeededEnergyTable.FansAndPumps.Actual += ekoCoeficient4 * heatedArea / 1000000.0;
		ekoCoeficient4 = GetEkoCoeficient(Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.Lights.General.DevicesNeededEnergyActual);
		zoneBalanceResult.EmissionNeededEnergyTable.Lights.Actual += ekoCoeficient4 * heatedArea / 1000000.0;
		ekoCoeficient4 = GetEkoCoeficient(Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.BalancedDevices.General.DevicesNeededEnergyActual);
		zoneBalanceResult.EmissionNeededEnergyTable.HeatAffectingDevices.Actual += ekoCoeficient4 * heatedArea / 1000000.0;
		ekoCoeficient4 = GetEkoCoeficient(Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.NonBalancedDevices.General.DevicesNeededEnergyActual);
		zoneBalanceResult.EmissionNeededEnergyTable.NonHeatAffectingDevices.Actual += ekoCoeficient4 * heatedArea / 1000000.0;
		ekoCoeficient4 = GetEkoCoeficient(Fuel.Fuel1, buildZone.HeatingCalculations.FansAndPumps.OtherResultCoolingActual);
		zoneBalanceResult.EmissionNeededEnergyTable.Other.Actual += ekoCoeficient4 * heatedArea / 1000000.0;
		CalculateTotalCO2Actual(zoneBalanceResult);
	}

	private static void CalculateTotalCO2Actual(Results zoneBalanceResult)
	{
		GeneralResultTable emissionNeededEnergyTable = zoneBalanceResult.EmissionNeededEnergyTable;
		emissionNeededEnergyTable.Total.Actual = emissionNeededEnergyTable.Heating.Actual + emissionNeededEnergyTable.Cooling.Actual + emissionNeededEnergyTable.CoolingVentilation.Actual + emissionNeededEnergyTable.HeatingVentilation.Actual + emissionNeededEnergyTable.FansAndPumps.Actual + emissionNeededEnergyTable.BGV.Actual + emissionNeededEnergyTable.BGVPumps.Actual + emissionNeededEnergyTable.Lights.Actual + emissionNeededEnergyTable.HeatAffectingDevices.Actual + emissionNeededEnergyTable.NonHeatAffectingDevices.Actual + emissionNeededEnergyTable.Other.Actual;
	}

	private static void CalculateCO2EmissionsBaseLine(Results zoneBalanceResult, BuildingZone buildZone, bool isBGVused)
	{
		double heatedArea = buildZone.Heating.Area.HeatedArea;
		if (buildZone.HasHeating)
		{
			double ekoCoeficient = GetEkoCoeficient(buildZone.HeatingCalculations.HeatingResult.Fuel1BaseLine, buildZone.HeatingCalculations.HeatingResult.ResultSourceEnergyBaseLine);
			double ekoCoeficient2 = GetEkoCoeficient(buildZone.HeatingCalculations.HeatingResult.Fuel2BaseLine, buildZone.HeatingCalculations.HeatingResult.ResultSourceEnergy2BaseLine);
			zoneBalanceResult.EmissionNeededEnergyTable.Heating.BaseLine += (ekoCoeficient + ekoCoeficient2) * heatedArea / 1000000.0;
			ekoCoeficient = GetEkoCoeficient(buildZone.HeatingCalculations.VentilationHeating.Fuel1BaseLine, buildZone.HeatingCalculations.VentilationHeating.ResultSourceEnergyBaseLine);
			ekoCoeficient2 = GetEkoCoeficient(buildZone.HeatingCalculations.VentilationHeating.Fuel2BaseLine, buildZone.HeatingCalculations.VentilationHeating.ResultSourceEnergy2BaseLine);
			zoneBalanceResult.EmissionNeededEnergyTable.HeatingVentilation.BaseLine += (ekoCoeficient + ekoCoeficient2) * heatedArea / 1000000.0;
		}
		if (buildZone.HasCooling)
		{
			double ekoCoeficient = GetEkoCoeficient(buildZone.CoolingCalculations.CoolingResult.Fuel1BaseLine, buildZone.CoolingCalculations.CoolingResult.ResultSourceEnergyBaseLine);
			double ekoCoeficient2 = GetEkoCoeficient(buildZone.CoolingCalculations.CoolingResult.Fuel2BaseLine, buildZone.CoolingCalculations.CoolingResult.ResultSourceEnergy2BaseLine);
			zoneBalanceResult.EmissionNeededEnergyTable.Cooling.BaseLine += (ekoCoeficient + ekoCoeficient2) * heatedArea / 1000000.0;
			ekoCoeficient = GetEkoCoeficient(buildZone.CoolingCalculations.VentilationCooling.Fuel1BaseLine, buildZone.CoolingCalculations.VentilationCooling.ResultSourceEnergyBaseLine);
			ekoCoeficient2 = GetEkoCoeficient(buildZone.CoolingCalculations.VentilationCooling.Fuel2BaseLine, buildZone.CoolingCalculations.VentilationCooling.ResultSourceEnergy2BaseLine);
			zoneBalanceResult.EmissionNeededEnergyTable.CoolingVentilation.BaseLine += (ekoCoeficient + ekoCoeficient2) * heatedArea / 1000000.0;
		}
		if (isBGVused)
		{
			double ekoCoeficient = GetEkoCoeficient(buildZone.HeatingCalculations.HotWaterCalculations.Fuel1BaseLine, buildZone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergyBaseLine);
			double ekoCoeficient2 = GetEkoCoeficient(buildZone.HeatingCalculations.HotWaterCalculations.Fuel2BaseLine, buildZone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergy2BaseLine);
			zoneBalanceResult.EmissionNeededEnergyTable.BGV.BaseLine += (ekoCoeficient + ekoCoeficient2) * heatedArea / 1000000.0;
			double ekoCoeficient3 = GetEkoCoeficient(Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.HotWaterPumps.General.DevicesNeededEnergyBaseLine);
			zoneBalanceResult.EmissionNeededEnergyTable.BGVPumps.BaseLine += ekoCoeficient3 * heatedArea / 1000000.0;
		}
		else
		{
			zoneBalanceResult.EmissionNeededEnergyTable.BGV.BaseLine = 0.0;
			zoneBalanceResult.EmissionNeededEnergyTable.BGVPumps.BaseLine = 0.0;
		}
		double ekoCoeficient4 = GetEkoCoeficient(Fuel.Fuel1, buildZone.HeatingCalculations.FansAndPumps.CoolNeededEnergyBaseLine + buildZone.HeatingCalculations.FansAndPumps.PumpNeededEnergyBaseLine);
		zoneBalanceResult.EmissionNeededEnergyTable.FansAndPumps.BaseLine += ekoCoeficient4 * heatedArea / 1000000.0;
		ekoCoeficient4 = GetEkoCoeficient(Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.Lights.General.DevicesNeededEnergyBaseLine);
		zoneBalanceResult.EmissionNeededEnergyTable.Lights.BaseLine += ekoCoeficient4 * heatedArea / 1000000.0;
		ekoCoeficient4 = GetEkoCoeficient(Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.BalancedDevices.General.DevicesNeededEnergyBaseLine);
		zoneBalanceResult.EmissionNeededEnergyTable.HeatAffectingDevices.BaseLine += ekoCoeficient4 * heatedArea / 1000000.0;
		ekoCoeficient4 = GetEkoCoeficient(Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.NonBalancedDevices.General.DevicesNeededEnergyBaseLine);
		zoneBalanceResult.EmissionNeededEnergyTable.NonHeatAffectingDevices.BaseLine += ekoCoeficient4 * heatedArea / 1000000.0;
		ekoCoeficient4 = GetEkoCoeficient(Fuel.Fuel1, buildZone.HeatingCalculations.FansAndPumps.OtherResultCoolingBaseLine);
		zoneBalanceResult.EmissionNeededEnergyTable.Other.BaseLine += ekoCoeficient4 * heatedArea / 1000000.0;
		CalculateTotalCO2BaseLine(zoneBalanceResult);
	}

	private static void CalculateTotalCO2BaseLine(Results zoneBalanceResult)
	{
		GeneralResultTable emissionNeededEnergyTable = zoneBalanceResult.EmissionNeededEnergyTable;
		emissionNeededEnergyTable.Total.BaseLine = emissionNeededEnergyTable.Heating.BaseLine + emissionNeededEnergyTable.Cooling.BaseLine + emissionNeededEnergyTable.CoolingVentilation.BaseLine + emissionNeededEnergyTable.HeatingVentilation.BaseLine + emissionNeededEnergyTable.FansAndPumps.BaseLine + emissionNeededEnergyTable.BGV.BaseLine + emissionNeededEnergyTable.BGVPumps.BaseLine + emissionNeededEnergyTable.Lights.BaseLine + emissionNeededEnergyTable.HeatAffectingDevices.BaseLine + emissionNeededEnergyTable.NonHeatAffectingDevices.BaseLine + emissionNeededEnergyTable.Other.BaseLine;
	}

	private static void CalculateCO2EmissionsESM(Results zoneBalanceResult, BuildingZone buildZone, bool isBGVused)
	{
		double heatedArea = buildZone.Heating.Area.HeatedArea;
		if (buildZone.HasHeating)
		{
			double ekoCoeficient = GetEkoCoeficient(buildZone.HeatingCalculations.HeatingResult.Fuel1ESM, buildZone.HeatingCalculations.HeatingResult.ResultSourceEnergyESM);
			double ekoCoeficient2 = GetEkoCoeficient(buildZone.HeatingCalculations.HeatingResult.Fuel2ESM, buildZone.HeatingCalculations.HeatingResult.ResultSourceEnergy2ESM);
			zoneBalanceResult.EmissionNeededEnergyTable.Heating.ESM += (ekoCoeficient + ekoCoeficient2) * heatedArea / 1000000.0;
			ekoCoeficient = GetEkoCoeficient(buildZone.HeatingCalculations.VentilationHeating.Fuel1ESM, buildZone.HeatingCalculations.VentilationHeating.ResultSourceEnergyESM);
			ekoCoeficient2 = GetEkoCoeficient(buildZone.HeatingCalculations.VentilationHeating.Fuel2ESM, buildZone.HeatingCalculations.VentilationHeating.ResultSourceEnergy2ESM);
			zoneBalanceResult.EmissionNeededEnergyTable.HeatingVentilation.ESM += (ekoCoeficient + ekoCoeficient2) * heatedArea / 1000000.0;
		}
		if (buildZone.HasCooling)
		{
			double ekoCoeficient = GetEkoCoeficient(buildZone.CoolingCalculations.CoolingResult.Fuel1ESM, buildZone.CoolingCalculations.CoolingResult.ResultSourceEnergyESM);
			double ekoCoeficient2 = GetEkoCoeficient(buildZone.CoolingCalculations.CoolingResult.Fuel2ESM, buildZone.CoolingCalculations.CoolingResult.ResultSourceEnergy2ESM);
			zoneBalanceResult.EmissionNeededEnergyTable.Cooling.ESM += (ekoCoeficient + ekoCoeficient2) * heatedArea / 1000000.0;
			ekoCoeficient = GetEkoCoeficient(buildZone.CoolingCalculations.VentilationCooling.Fuel1ESM, buildZone.CoolingCalculations.VentilationCooling.ResultSourceEnergyESM);
			ekoCoeficient2 = GetEkoCoeficient(buildZone.CoolingCalculations.VentilationCooling.Fuel2ESM, buildZone.CoolingCalculations.VentilationCooling.ResultSourceEnergy2ESM);
			zoneBalanceResult.EmissionNeededEnergyTable.CoolingVentilation.ESM += (ekoCoeficient + ekoCoeficient2) * heatedArea / 1000000.0;
		}
		if (isBGVused)
		{
			double ekoCoeficient = GetEkoCoeficient(buildZone.HeatingCalculations.HotWaterCalculations.Fuel1ESM, buildZone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergyESM);
			double ekoCoeficient2 = GetEkoCoeficient(buildZone.HeatingCalculations.HotWaterCalculations.Fuel2ESM, buildZone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergy2ESM);
			zoneBalanceResult.EmissionNeededEnergyTable.BGV.ESM += (ekoCoeficient + ekoCoeficient2) * heatedArea / 1000000.0;
			double ekoCoeficient3 = GetEkoCoeficient(Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.HotWaterPumps.General.DevicesNeededEnergyESM);
			zoneBalanceResult.EmissionNeededEnergyTable.BGVPumps.ESM += ekoCoeficient3 * heatedArea / 1000000.0;
		}
		else
		{
			zoneBalanceResult.EmissionNeededEnergyTable.BGV.ESM = 0.0;
			zoneBalanceResult.EmissionNeededEnergyTable.BGVPumps.ESM = 0.0;
		}
		double ekoCoeficient4 = GetEkoCoeficient(Fuel.Fuel1, buildZone.HeatingCalculations.FansAndPumps.CoolNeededEnergyESM + buildZone.HeatingCalculations.FansAndPumps.PumpNeededEnergyESM);
		zoneBalanceResult.EmissionNeededEnergyTable.FansAndPumps.ESM += ekoCoeficient4 * heatedArea / 1000000.0;
		ekoCoeficient4 = GetEkoCoeficient(Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.Lights.General.DevicesNeededEnergyESM);
		zoneBalanceResult.EmissionNeededEnergyTable.Lights.ESM += ekoCoeficient4 * heatedArea / 1000000.0;
		ekoCoeficient4 = GetEkoCoeficient(Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.BalancedDevices.General.DevicesNeededEnergyESM);
		zoneBalanceResult.EmissionNeededEnergyTable.HeatAffectingDevices.ESM += ekoCoeficient4 * heatedArea / 1000000.0;
		ekoCoeficient4 = GetEkoCoeficient(Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.NonBalancedDevices.General.DevicesNeededEnergyESM);
		zoneBalanceResult.EmissionNeededEnergyTable.NonHeatAffectingDevices.ESM += ekoCoeficient4 * heatedArea / 1000000.0;
		ekoCoeficient4 = GetEkoCoeficient(Fuel.Fuel1, buildZone.HeatingCalculations.FansAndPumps.OtherResultCoolingESM);
		zoneBalanceResult.EmissionNeededEnergyTable.Other.ESM += ekoCoeficient4 * heatedArea / 1000000.0;
		CalculateTotalCO2ESM(zoneBalanceResult);
	}

	private static void CalculateTotalCO2ESM(Results zoneBalanceResult)
	{
		GeneralResultTable emissionNeededEnergyTable = zoneBalanceResult.EmissionNeededEnergyTable;
		emissionNeededEnergyTable.Total.ESM = emissionNeededEnergyTable.Heating.ESM + emissionNeededEnergyTable.Cooling.ESM + emissionNeededEnergyTable.CoolingVentilation.ESM + emissionNeededEnergyTable.HeatingVentilation.ESM + emissionNeededEnergyTable.FansAndPumps.ESM + emissionNeededEnergyTable.BGV.ESM + emissionNeededEnergyTable.BGVPumps.ESM + emissionNeededEnergyTable.Lights.ESM + emissionNeededEnergyTable.HeatAffectingDevices.ESM + emissionNeededEnergyTable.NonHeatAffectingDevices.ESM + emissionNeededEnergyTable.Other.ESM;
	}

	private static void CalculateSavings(GeneralResultTable emmTable)
	{
		emmTable.Heating.Savings = emmTable.Heating.BaseLine - emmTable.Heating.ESM;
		emmTable.Cooling.Savings = emmTable.Cooling.BaseLine - emmTable.Cooling.ESM;
		emmTable.HeatingVentilation.Savings = emmTable.HeatingVentilation.BaseLine - emmTable.HeatingVentilation.ESM;
		emmTable.CoolingVentilation.Savings = emmTable.CoolingVentilation.BaseLine - emmTable.CoolingVentilation.ESM;
		emmTable.FansAndPumps.Savings = emmTable.FansAndPumps.BaseLine - emmTable.FansAndPumps.ESM;
		emmTable.BGV.Savings = emmTable.BGV.BaseLine - emmTable.BGV.ESM;
		emmTable.BGVPumps.Savings = emmTable.BGVPumps.BaseLine - emmTable.BGVPumps.ESM;
		emmTable.Lights.Savings = emmTable.Lights.BaseLine - emmTable.Lights.ESM;
		emmTable.HeatAffectingDevices.Savings = emmTable.HeatAffectingDevices.BaseLine - emmTable.HeatAffectingDevices.ESM;
		emmTable.NonHeatAffectingDevices.Savings = emmTable.NonHeatAffectingDevices.BaseLine - emmTable.NonHeatAffectingDevices.ESM;
		emmTable.Other.Savings = emmTable.Other.BaseLine - emmTable.Other.ESM;
		emmTable.Total.Savings = emmTable.Total.BaseLine - emmTable.Total.ESM;
	}

	private static void CalculateFuelSavings(GeneralResultTable fuelTable)
	{
		fuelTable.Fuel1.Savings = fuelTable.Fuel1.BaseLine - fuelTable.Fuel1.ESM;
		fuelTable.Fuel2.Savings = fuelTable.Fuel2.BaseLine - fuelTable.Fuel2.ESM;
		fuelTable.Fuel3.Savings = fuelTable.Fuel3.BaseLine - fuelTable.Fuel3.ESM;
		fuelTable.Fuel4.Savings = fuelTable.Fuel4.BaseLine - fuelTable.Fuel4.ESM;
		fuelTable.Fuel5.Savings = fuelTable.Fuel5.BaseLine - fuelTable.Fuel5.ESM;
		fuelTable.Fuel6.Savings = fuelTable.Fuel6.BaseLine - fuelTable.Fuel6.ESM;
		fuelTable.Fuel7.Savings = fuelTable.Fuel7.BaseLine - fuelTable.Fuel7.ESM;
		fuelTable.Fuel8.Savings = fuelTable.Fuel8.BaseLine - fuelTable.Fuel8.ESM;
		fuelTable.Fuel9.Savings = fuelTable.Fuel9.BaseLine - fuelTable.Fuel9.ESM;
		fuelTable.Fuel10.Savings = fuelTable.Fuel10.BaseLine - fuelTable.Fuel10.ESM;
		fuelTable.Fuel11.Savings = fuelTable.Fuel11.BaseLine - fuelTable.Fuel11.ESM;
		fuelTable.Total.Savings = fuelTable.Total.BaseLine - fuelTable.Total.ESM;
	}

	private static void CO2EnergyZoneCalculations(Results zoneBalanceResult, BuildingZone zone, bool isBGVused)
	{
		Co2EnergyCalculationZoneRef1(zoneBalanceResult, zone, isBGVused);
		Co2EnergyCalculationZoneRef2(zoneBalanceResult, zone, isBGVused);
		Co2EnergyCalculationZoneActual(zoneBalanceResult, zone, isBGVused);
		Co2EnergyCalculationZoneBaseLine(zoneBalanceResult, zone, isBGVused);
		Co2EnergyCalculationZoneESM(zoneBalanceResult, zone, isBGVused);
	}

	private static void Co2EnergyCalculationZoneRef1(Results zoneBalanceResult, BuildingZone buildZone, bool isBGVused)
	{
		double heatedArea = buildZone.Heating.Area.HeatedArea;
		GetFuelTypeCo2Ref1(zoneBalanceResult, buildZone.HeatingCalculations.HeatingResult.Fuel1Ref1, buildZone.HeatingCalculations.HeatingResult.ResultSourceEnergyRef1, heatedArea);
		GetFuelTypeCo2Ref1(zoneBalanceResult, buildZone.HeatingCalculations.HeatingResult.Fuel2Ref1, buildZone.HeatingCalculations.HeatingResult.ResultSourceEnergy2Ref1, heatedArea);
		GetFuelTypeCo2Ref1(zoneBalanceResult, buildZone.CoolingCalculations.CoolingResult.Fuel1Ref1, buildZone.CoolingCalculations.CoolingResult.ResultSourceEnergyRef1, heatedArea);
		GetFuelTypeCo2Ref1(zoneBalanceResult, buildZone.CoolingCalculations.CoolingResult.Fuel2Ref1, buildZone.CoolingCalculations.CoolingResult.ResultSourceEnergy2Ref1, heatedArea);
		GetFuelTypeCo2Ref1(zoneBalanceResult, buildZone.HeatingCalculations.VentilationHeating.Fuel1Ref1, buildZone.HeatingCalculations.VentilationHeating.ResultSourceEnergyRef1, heatedArea);
		GetFuelTypeCo2Ref1(zoneBalanceResult, buildZone.HeatingCalculations.VentilationHeating.Fuel2Ref1, buildZone.HeatingCalculations.VentilationHeating.ResultSourceEnergy2Ref1, heatedArea);
		GetFuelTypeCo2Ref1(zoneBalanceResult, buildZone.CoolingCalculations.VentilationCooling.Fuel1Ref1, buildZone.CoolingCalculations.VentilationCooling.ResultSourceEnergyRef1, heatedArea);
		GetFuelTypeCo2Ref1(zoneBalanceResult, buildZone.CoolingCalculations.VentilationCooling.Fuel2Ref1, buildZone.CoolingCalculations.VentilationCooling.ResultSourceEnergy2Ref1, heatedArea);
		GetFuelTypeCo2Ref1(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.FansAndPumps.CoolNeededEnergyRef1, heatedArea);
		GetFuelTypeCo2Ref1(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.FansAndPumps.PumpNeededEnergyRef1, heatedArea);
		GetFuelTypeCo2Ref1(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.Lights.General.DevicesNeededEnergyRef1, heatedArea);
		GetFuelTypeCo2Ref1(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.BalancedDevices.General.DevicesNeededEnergyRef1, heatedArea);
		GetFuelTypeCo2Ref1(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.NonBalancedDevices.General.DevicesNeededEnergyRef1, heatedArea);
		if (isBGVused)
		{
			GetFuelTypeCo2Ref1(zoneBalanceResult, buildZone.HeatingCalculations.HotWaterCalculations.Fuel1Ref1, buildZone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergyRef1, heatedArea);
			GetFuelTypeCo2Ref1(zoneBalanceResult, buildZone.HeatingCalculations.HotWaterCalculations.Fuel1Ref2, buildZone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergy2Ref1, heatedArea);
			GetFuelTypeCo2Ref1(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.HotWaterPumps.General.DevicesNeededEnergyRef1, heatedArea);
		}
		else
		{
			zoneBalanceResult.EmissionNeededEnergyTable.BGV.Ref1 = 0.0;
			zoneBalanceResult.EmissionNeededEnergyTable.BGVPumps.Ref1 = 0.0;
		}
		GetFuelTypeCo2Ref1(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.FansAndPumps.OtherResultCoolingRef1, heatedArea);
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel1.Ref1 = zoneBalanceResult.EmissionEnergySupplyTable.Fuel1.Ref1 * 267.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel2.Ref1 = zoneBalanceResult.EmissionEnergySupplyTable.Fuel2.Ref1 * 202.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel3.Ref1 = zoneBalanceResult.EmissionEnergySupplyTable.Fuel3.Ref1 * 227.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel4.Ref1 = zoneBalanceResult.EmissionEnergySupplyTable.Fuel4.Ref1 * 341.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel5.Ref1 = zoneBalanceResult.EmissionEnergySupplyTable.Fuel5.Ref1 * 364.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel6.Ref1 = zoneBalanceResult.EmissionEnergySupplyTable.Fuel6.Ref1 * 43.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel7.Ref1 = zoneBalanceResult.EmissionEnergySupplyTable.Fuel7.Ref1 * 351.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel8.Ref1 = zoneBalanceResult.EmissionEnergySupplyTable.Fuel8.Ref1 * 819.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel9.Ref1 = zoneBalanceResult.EmissionEnergySupplyTable.Fuel9.Ref1 * 290.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel10.Ref1 = zoneBalanceResult.EmissionEnergySupplyTable.Fuel10.Ref1 * 279.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel11.Ref1 = zoneBalanceResult.EmissionEnergySupplyTable.Fuel11.Ref1 * 354.0 / 1000.0;
	}

	private static void Co2EnergyCalculationZoneRef2(Results zoneBalanceResult, BuildingZone buildZone, bool isBGVused)
	{
		double heatedArea = buildZone.Heating.Area.HeatedArea;
		GetFuelTypeCo2Ref2(zoneBalanceResult, buildZone.HeatingCalculations.HeatingResult.Fuel1Ref2, buildZone.HeatingCalculations.HeatingResult.ResultSourceEnergyRef2, heatedArea);
		GetFuelTypeCo2Ref2(zoneBalanceResult, buildZone.HeatingCalculations.HeatingResult.Fuel2Ref2, buildZone.HeatingCalculations.HeatingResult.ResultSourceEnergy2Ref2, heatedArea);
		GetFuelTypeCo2Ref2(zoneBalanceResult, buildZone.CoolingCalculations.CoolingResult.Fuel1Ref2, buildZone.CoolingCalculations.CoolingResult.ResultSourceEnergyRef2, heatedArea);
		GetFuelTypeCo2Ref2(zoneBalanceResult, buildZone.CoolingCalculations.CoolingResult.Fuel2Ref2, buildZone.CoolingCalculations.CoolingResult.ResultSourceEnergy2Ref2, heatedArea);
		GetFuelTypeCo2Ref2(zoneBalanceResult, buildZone.HeatingCalculations.VentilationHeating.Fuel1Ref2, buildZone.HeatingCalculations.VentilationHeating.ResultSourceEnergyRef2, heatedArea);
		GetFuelTypeCo2Ref2(zoneBalanceResult, buildZone.HeatingCalculations.VentilationHeating.Fuel2Ref2, buildZone.HeatingCalculations.VentilationHeating.ResultSourceEnergy2Ref2, heatedArea);
		GetFuelTypeCo2Ref2(zoneBalanceResult, buildZone.CoolingCalculations.VentilationCooling.Fuel1Ref2, buildZone.CoolingCalculations.VentilationCooling.ResultSourceEnergyRef2, heatedArea);
		GetFuelTypeCo2Ref2(zoneBalanceResult, buildZone.CoolingCalculations.VentilationCooling.Fuel2Ref2, buildZone.CoolingCalculations.VentilationCooling.ResultSourceEnergy2Ref2, heatedArea);
		GetFuelTypeCo2Ref2(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.FansAndPumps.CoolNeededEnergyRef2, heatedArea);
		GetFuelTypeCo2Ref2(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.FansAndPumps.PumpNeededEnergyRef2, heatedArea);
		GetFuelTypeCo2Ref2(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.Lights.General.DevicesNeededEnergyRef2, heatedArea);
		GetFuelTypeCo2Ref2(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.BalancedDevices.General.DevicesNeededEnergyRef2, heatedArea);
		GetFuelTypeCo2Ref2(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.NonBalancedDevices.General.DevicesNeededEnergyRef2, heatedArea);
		if (isBGVused)
		{
			GetFuelTypeCo2Ref2(zoneBalanceResult, buildZone.HeatingCalculations.HotWaterCalculations.Fuel1Ref1, buildZone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergyRef2, heatedArea);
			GetFuelTypeCo2Ref2(zoneBalanceResult, buildZone.HeatingCalculations.HotWaterCalculations.Fuel1Ref2, buildZone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergy2Ref2, heatedArea);
			GetFuelTypeCo2Ref2(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.HotWaterPumps.General.DevicesNeededEnergyRef2, heatedArea);
		}
		else
		{
			zoneBalanceResult.EmissionNeededEnergyTable.BGV.Ref2 = 0.0;
			zoneBalanceResult.EmissionNeededEnergyTable.BGVPumps.Ref2 = 0.0;
		}
		GetFuelTypeCo2Ref2(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.FansAndPumps.OtherResultCoolingRef2, heatedArea);
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel1.Ref2 = zoneBalanceResult.EmissionEnergySupplyTable.Fuel1.Ref2 * 267.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel2.Ref2 = zoneBalanceResult.EmissionEnergySupplyTable.Fuel2.Ref2 * 202.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel3.Ref2 = zoneBalanceResult.EmissionEnergySupplyTable.Fuel3.Ref2 * 227.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel4.Ref2 = zoneBalanceResult.EmissionEnergySupplyTable.Fuel4.Ref2 * 341.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel5.Ref2 = zoneBalanceResult.EmissionEnergySupplyTable.Fuel5.Ref2 * 364.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel6.Ref2 = zoneBalanceResult.EmissionEnergySupplyTable.Fuel6.Ref2 * 43.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel7.Ref2 = zoneBalanceResult.EmissionEnergySupplyTable.Fuel7.Ref2 * 351.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel8.Ref2 = zoneBalanceResult.EmissionEnergySupplyTable.Fuel8.Ref2 * 819.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel9.Ref2 = zoneBalanceResult.EmissionEnergySupplyTable.Fuel9.Ref2 * 290.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel10.Ref2 = zoneBalanceResult.EmissionEnergySupplyTable.Fuel10.Ref2 * 279.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel11.Ref2 = zoneBalanceResult.EmissionEnergySupplyTable.Fuel11.Ref2 * 354.0 / 1000.0;
	}

	private static void Co2EnergyCalculationZoneActual(Results zoneBalanceResult, BuildingZone buildZone, bool isBGVused)
	{
		double heatedArea = buildZone.Heating.Area.HeatedArea;
		if (buildZone.HasHeating)
		{
			GetFuelTypeCo2Actual(zoneBalanceResult, buildZone.HeatingCalculations.HeatingResult.Fuel1Actual, buildZone.HeatingCalculations.HeatingResult.ResultSourceEnergyActual, heatedArea);
			GetFuelTypeCo2Actual(zoneBalanceResult, buildZone.HeatingCalculations.HeatingResult.Fuel2Actual, buildZone.HeatingCalculations.HeatingResult.ResultSourceEnergy2Actual, heatedArea);
			GetFuelTypeCo2Actual(zoneBalanceResult, buildZone.HeatingCalculations.VentilationHeating.Fuel1Actual, buildZone.HeatingCalculations.VentilationHeating.ResultSourceEnergyActual, heatedArea);
			GetFuelTypeCo2Actual(zoneBalanceResult, buildZone.HeatingCalculations.VentilationHeating.Fuel2Actual, buildZone.HeatingCalculations.VentilationHeating.ResultSourceEnergy2Actual, heatedArea);
		}
		if (buildZone.HasCooling)
		{
			GetFuelTypeCo2Actual(zoneBalanceResult, buildZone.CoolingCalculations.CoolingResult.Fuel1Actual, buildZone.CoolingCalculations.CoolingResult.ResultSourceEnergyActual, heatedArea);
			GetFuelTypeCo2Actual(zoneBalanceResult, buildZone.CoolingCalculations.CoolingResult.Fuel2Actual, buildZone.CoolingCalculations.CoolingResult.ResultSourceEnergy2Actual, heatedArea);
			GetFuelTypeCo2Actual(zoneBalanceResult, buildZone.CoolingCalculations.VentilationCooling.Fuel1Actual, buildZone.CoolingCalculations.VentilationCooling.ResultSourceEnergyActual, heatedArea);
			GetFuelTypeCo2Actual(zoneBalanceResult, buildZone.CoolingCalculations.VentilationCooling.Fuel2Actual, buildZone.CoolingCalculations.VentilationCooling.ResultSourceEnergy2Actual, heatedArea);
		}
		GetFuelTypeCo2Actual(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.FansAndPumps.CoolNeededEnergyActual, heatedArea);
		GetFuelTypeCo2Actual(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.FansAndPumps.PumpNeededEnergyActual, heatedArea);
		GetFuelTypeCo2Actual(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.Lights.General.DevicesNeededEnergyActual, heatedArea);
		GetFuelTypeCo2Actual(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.BalancedDevices.General.DevicesNeededEnergyActual, heatedArea);
		GetFuelTypeCo2Actual(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.NonBalancedDevices.General.DevicesNeededEnergyActual, heatedArea);
		if (isBGVused)
		{
			GetFuelTypeCo2Actual(zoneBalanceResult, buildZone.HeatingCalculations.HotWaterCalculations.Fuel1Actual, buildZone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergyActual, heatedArea);
			GetFuelTypeCo2Actual(zoneBalanceResult, buildZone.HeatingCalculations.HotWaterCalculations.Fuel1Actual, buildZone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergy2Actual, heatedArea);
			GetFuelTypeCo2Actual(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.HotWaterPumps.General.DevicesNeededEnergyActual, heatedArea);
		}
		else
		{
			zoneBalanceResult.EmissionNeededEnergyTable.BGV.Actual = 0.0;
			zoneBalanceResult.EmissionNeededEnergyTable.BGVPumps.Actual = 0.0;
		}
		GetFuelTypeCo2Actual(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.FansAndPumps.OtherResultCoolingActual, heatedArea);
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel1.Actual = zoneBalanceResult.EmissionEnergySupplyTable.Fuel1.Actual * 267.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel2.Actual = zoneBalanceResult.EmissionEnergySupplyTable.Fuel2.Actual * 202.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel3.Actual = zoneBalanceResult.EmissionEnergySupplyTable.Fuel3.Actual * 227.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel4.Actual = zoneBalanceResult.EmissionEnergySupplyTable.Fuel4.Actual * 341.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel5.Actual = zoneBalanceResult.EmissionEnergySupplyTable.Fuel5.Actual * 364.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel6.Actual = zoneBalanceResult.EmissionEnergySupplyTable.Fuel6.Actual * 43.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel7.Actual = zoneBalanceResult.EmissionEnergySupplyTable.Fuel7.Actual * 351.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel8.Actual = zoneBalanceResult.EmissionEnergySupplyTable.Fuel8.Actual * 819.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel9.Actual = zoneBalanceResult.EmissionEnergySupplyTable.Fuel9.Actual * 290.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel10.Actual = zoneBalanceResult.EmissionEnergySupplyTable.Fuel10.Actual * 279.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel11.Actual = zoneBalanceResult.EmissionEnergySupplyTable.Fuel11.Actual * 354.0 / 1000.0;
	}

	private static void Co2EnergyCalculationZoneBaseLine(Results zoneBalanceResult, BuildingZone buildZone, bool isBGVused)
	{
		double heatedArea = buildZone.Heating.Area.HeatedArea;
		if (buildZone.HasHeating)
		{
			GetFuelTypeCo2BaseLine(zoneBalanceResult, buildZone.HeatingCalculations.HeatingResult.Fuel1BaseLine, buildZone.HeatingCalculations.HeatingResult.ResultSourceEnergyBaseLine, heatedArea);
			GetFuelTypeCo2BaseLine(zoneBalanceResult, buildZone.HeatingCalculations.HeatingResult.Fuel2BaseLine, buildZone.HeatingCalculations.HeatingResult.ResultSourceEnergy2BaseLine, heatedArea);
			GetFuelTypeCo2BaseLine(zoneBalanceResult, buildZone.HeatingCalculations.VentilationHeating.Fuel1BaseLine, buildZone.HeatingCalculations.VentilationHeating.ResultSourceEnergyBaseLine, heatedArea);
			GetFuelTypeCo2BaseLine(zoneBalanceResult, buildZone.HeatingCalculations.VentilationHeating.Fuel2BaseLine, buildZone.HeatingCalculations.VentilationHeating.ResultSourceEnergy2BaseLine, heatedArea);
		}
		if (buildZone.HasCooling)
		{
			GetFuelTypeCo2BaseLine(zoneBalanceResult, buildZone.CoolingCalculations.CoolingResult.Fuel1BaseLine, buildZone.CoolingCalculations.CoolingResult.ResultSourceEnergyBaseLine, heatedArea);
			GetFuelTypeCo2BaseLine(zoneBalanceResult, buildZone.CoolingCalculations.CoolingResult.Fuel2BaseLine, buildZone.CoolingCalculations.CoolingResult.ResultSourceEnergy2BaseLine, heatedArea);
			GetFuelTypeCo2BaseLine(zoneBalanceResult, buildZone.CoolingCalculations.VentilationCooling.Fuel1BaseLine, buildZone.CoolingCalculations.VentilationCooling.ResultSourceEnergyBaseLine, heatedArea);
			GetFuelTypeCo2BaseLine(zoneBalanceResult, buildZone.CoolingCalculations.VentilationCooling.Fuel2BaseLine, buildZone.CoolingCalculations.VentilationCooling.ResultSourceEnergy2BaseLine, heatedArea);
		}
		GetFuelTypeCo2BaseLine(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.FansAndPumps.CoolNeededEnergyBaseLine, heatedArea);
		GetFuelTypeCo2BaseLine(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.FansAndPumps.PumpNeededEnergyBaseLine, heatedArea);
		GetFuelTypeCo2BaseLine(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.Lights.General.DevicesNeededEnergyBaseLine, heatedArea);
		GetFuelTypeCo2BaseLine(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.BalancedDevices.General.DevicesNeededEnergyBaseLine, heatedArea);
		GetFuelTypeCo2BaseLine(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.NonBalancedDevices.General.DevicesNeededEnergyBaseLine, heatedArea);
		if (isBGVused)
		{
			GetFuelTypeCo2BaseLine(zoneBalanceResult, buildZone.HeatingCalculations.HotWaterCalculations.Fuel1Ref1, buildZone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergyBaseLine, heatedArea);
			GetFuelTypeCo2BaseLine(zoneBalanceResult, buildZone.HeatingCalculations.HotWaterCalculations.Fuel1Ref2, buildZone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergy2BaseLine, heatedArea);
			GetFuelTypeCo2BaseLine(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.HotWaterPumps.General.DevicesNeededEnergyBaseLine, heatedArea);
		}
		else
		{
			zoneBalanceResult.EmissionNeededEnergyTable.BGV.BaseLine = 0.0;
			zoneBalanceResult.EmissionNeededEnergyTable.BGVPumps.BaseLine = 0.0;
		}
		GetFuelTypeCo2BaseLine(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.FansAndPumps.OtherResultCoolingBaseLine, heatedArea);
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel1.BaseLine = zoneBalanceResult.EmissionEnergySupplyTable.Fuel1.BaseLine * 267.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel2.BaseLine = zoneBalanceResult.EmissionEnergySupplyTable.Fuel2.BaseLine * 202.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel3.BaseLine = zoneBalanceResult.EmissionEnergySupplyTable.Fuel3.BaseLine * 227.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel4.BaseLine = zoneBalanceResult.EmissionEnergySupplyTable.Fuel4.BaseLine * 341.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel5.BaseLine = zoneBalanceResult.EmissionEnergySupplyTable.Fuel5.BaseLine * 364.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel6.BaseLine = zoneBalanceResult.EmissionEnergySupplyTable.Fuel6.BaseLine * 43.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel7.BaseLine = zoneBalanceResult.EmissionEnergySupplyTable.Fuel7.BaseLine * 351.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel8.BaseLine = zoneBalanceResult.EmissionEnergySupplyTable.Fuel8.BaseLine * 819.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel9.BaseLine = zoneBalanceResult.EmissionEnergySupplyTable.Fuel9.BaseLine * 290.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel10.BaseLine = zoneBalanceResult.EmissionEnergySupplyTable.Fuel10.BaseLine * 279.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel11.BaseLine = zoneBalanceResult.EmissionEnergySupplyTable.Fuel11.BaseLine * 354.0 / 1000.0;
	}

	private static void Co2EnergyCalculationZoneESM(Results zoneBalanceResult, BuildingZone buildZone, bool isBGVused)
	{
		double heatedArea = buildZone.Heating.Area.HeatedArea;
		if (buildZone.HasHeating)
		{
			GetFuelTypeCo2ESM(zoneBalanceResult, buildZone.HeatingCalculations.HeatingResult.Fuel1ESM, buildZone.HeatingCalculations.HeatingResult.ResultSourceEnergyESM, heatedArea);
			GetFuelTypeCo2ESM(zoneBalanceResult, buildZone.HeatingCalculations.HeatingResult.Fuel2ESM, buildZone.HeatingCalculations.HeatingResult.ResultSourceEnergy2ESM, heatedArea);
			GetFuelTypeCo2ESM(zoneBalanceResult, buildZone.HeatingCalculations.VentilationHeating.Fuel1ESM, buildZone.HeatingCalculations.VentilationHeating.ResultSourceEnergyESM, heatedArea);
			GetFuelTypeCo2ESM(zoneBalanceResult, buildZone.HeatingCalculations.VentilationHeating.Fuel2ESM, buildZone.HeatingCalculations.VentilationHeating.ResultSourceEnergy2ESM, heatedArea);
		}
		if (buildZone.HasCooling)
		{
			GetFuelTypeCo2ESM(zoneBalanceResult, buildZone.CoolingCalculations.CoolingResult.Fuel1ESM, buildZone.CoolingCalculations.CoolingResult.ResultSourceEnergyESM, heatedArea);
			GetFuelTypeCo2ESM(zoneBalanceResult, buildZone.CoolingCalculations.CoolingResult.Fuel2ESM, buildZone.CoolingCalculations.CoolingResult.ResultSourceEnergy2ESM, heatedArea);
			GetFuelTypeCo2ESM(zoneBalanceResult, buildZone.CoolingCalculations.VentilationCooling.Fuel1ESM, buildZone.CoolingCalculations.VentilationCooling.ResultSourceEnergyESM, heatedArea);
			GetFuelTypeCo2ESM(zoneBalanceResult, buildZone.CoolingCalculations.VentilationCooling.Fuel2ESM, buildZone.CoolingCalculations.VentilationCooling.ResultSourceEnergy2ESM, heatedArea);
		}
		GetFuelTypeCo2ESM(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.FansAndPumps.CoolNeededEnergyESM, heatedArea);
		GetFuelTypeCo2ESM(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.FansAndPumps.PumpNeededEnergyESM, heatedArea);
		GetFuelTypeCo2ESM(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.Lights.General.DevicesNeededEnergyESM, heatedArea);
		GetFuelTypeCo2ESM(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.BalancedDevices.General.DevicesNeededEnergyESM, heatedArea);
		GetFuelTypeCo2ESM(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.NonBalancedDevices.General.DevicesNeededEnergyESM, heatedArea);
		if (isBGVused)
		{
			GetFuelTypeCo2ESM(zoneBalanceResult, buildZone.HeatingCalculations.HotWaterCalculations.Fuel1Ref1, buildZone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergyESM, heatedArea);
			GetFuelTypeCo2ESM(zoneBalanceResult, buildZone.HeatingCalculations.HotWaterCalculations.Fuel1Ref2, buildZone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergy2ESM, heatedArea);
			GetFuelTypeCo2ESM(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.HotWaterPumps.General.DevicesNeededEnergyESM, heatedArea);
		}
		else
		{
			zoneBalanceResult.EmissionNeededEnergyTable.BGV.ESM = 0.0;
			zoneBalanceResult.EmissionNeededEnergyTable.BGVPumps.ESM = 0.0;
		}
		GetFuelTypeCo2ESM(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.FansAndPumps.OtherResultCoolingESM, heatedArea);
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel1.ESM = zoneBalanceResult.EmissionEnergySupplyTable.Fuel1.ESM * 267.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel2.ESM = zoneBalanceResult.EmissionEnergySupplyTable.Fuel2.ESM * 202.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel3.ESM = zoneBalanceResult.EmissionEnergySupplyTable.Fuel3.ESM * 227.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel4.ESM = zoneBalanceResult.EmissionEnergySupplyTable.Fuel4.ESM * 341.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel5.ESM = zoneBalanceResult.EmissionEnergySupplyTable.Fuel5.ESM * 364.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel6.ESM = zoneBalanceResult.EmissionEnergySupplyTable.Fuel6.ESM * 43.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel7.ESM = zoneBalanceResult.EmissionEnergySupplyTable.Fuel7.ESM * 351.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel8.ESM = zoneBalanceResult.EmissionEnergySupplyTable.Fuel8.ESM * 819.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel9.ESM = zoneBalanceResult.EmissionEnergySupplyTable.Fuel9.ESM * 290.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel10.ESM = zoneBalanceResult.EmissionEnergySupplyTable.Fuel10.ESM * 279.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel11.ESM = zoneBalanceResult.EmissionEnergySupplyTable.Fuel11.ESM * 354.0 / 1000.0;
	}

	private static void Co2GetFuelTypesBuilding(Results buildingResults, BuildingZone buildZone)
	{
		Co2EnergyCalculationBuildingRef1(buildingResults, buildZone, isBGVused: true);
		Co2EnergyCalculationBuildingRef2(buildingResults, buildZone, isBGVused: true);
		Co2EnergyCalculationBuildingActual(buildingResults, buildZone, isBGVused: true);
		Co2EnergyCalculationBuildingBaseLine(buildingResults, buildZone, isBGVused: true);
		Co2EnergyCalculationBuildingESM(buildingResults, buildZone, isBGVused: true);
	}

	private static void Co2EnergyCalculationBuildingRef1(Results zoneBalanceResult, BuildingZone buildZone, bool isBGVused)
	{
		double heatedArea = buildZone.Heating.Area.HeatedArea;
		GetFuelTypeCo2Ref1(zoneBalanceResult, buildZone.HeatingCalculations.HeatingResult.Fuel1Ref1, buildZone.HeatingCalculations.HeatingResult.ResultSourceEnergyRef1, heatedArea);
		GetFuelTypeCo2Ref1(zoneBalanceResult, buildZone.HeatingCalculations.HeatingResult.Fuel2Ref1, buildZone.HeatingCalculations.HeatingResult.ResultSourceEnergy2Ref1, heatedArea);
		GetFuelTypeCo2Ref1(zoneBalanceResult, buildZone.CoolingCalculations.CoolingResult.Fuel1Ref1, buildZone.CoolingCalculations.CoolingResult.ResultSourceEnergyRef1, heatedArea);
		GetFuelTypeCo2Ref1(zoneBalanceResult, buildZone.CoolingCalculations.CoolingResult.Fuel2Ref1, buildZone.CoolingCalculations.CoolingResult.ResultSourceEnergy2Ref1, heatedArea);
		GetFuelTypeCo2Ref1(zoneBalanceResult, buildZone.HeatingCalculations.VentilationHeating.Fuel1Ref1, buildZone.HeatingCalculations.VentilationHeating.ResultSourceEnergyRef1, heatedArea);
		GetFuelTypeCo2Ref1(zoneBalanceResult, buildZone.HeatingCalculations.VentilationHeating.Fuel2Ref1, buildZone.HeatingCalculations.VentilationHeating.ResultSourceEnergy2Ref1, heatedArea);
		GetFuelTypeCo2Ref1(zoneBalanceResult, buildZone.CoolingCalculations.VentilationCooling.Fuel1Ref1, buildZone.CoolingCalculations.VentilationCooling.ResultSourceEnergyRef1, heatedArea);
		GetFuelTypeCo2Ref1(zoneBalanceResult, buildZone.CoolingCalculations.VentilationCooling.Fuel2Ref1, buildZone.CoolingCalculations.VentilationCooling.ResultSourceEnergy2Ref1, heatedArea);
		GetFuelTypeCo2Ref1(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.FansAndPumps.CoolNeededEnergyRef1, heatedArea);
		GetFuelTypeCo2Ref1(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.FansAndPumps.PumpNeededEnergyRef1, heatedArea);
		GetFuelTypeCo2Ref1(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.Lights.General.DevicesNeededEnergyRef1, heatedArea);
		GetFuelTypeCo2Ref1(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.BalancedDevices.General.DevicesNeededEnergyRef1, heatedArea);
		GetFuelTypeCo2Ref1(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.NonBalancedDevices.General.DevicesNeededEnergyRef1, heatedArea);
		if (isBGVused)
		{
			GetFuelTypeCo2Ref1(zoneBalanceResult, buildZone.HeatingCalculations.HotWaterCalculations.Fuel1Ref1, buildZone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergyRef1, heatedArea);
			GetFuelTypeCo2Ref1(zoneBalanceResult, buildZone.HeatingCalculations.HotWaterCalculations.Fuel2Ref1, buildZone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergy2Ref1, heatedArea);
			GetFuelTypeCo2Ref1(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.HotWaterPumps.General.DevicesNeededEnergyRef1, heatedArea);
		}
		else
		{
			zoneBalanceResult.EmissionNeededEnergyTable.BGV.Ref1 = 0.0;
			zoneBalanceResult.EmissionNeededEnergyTable.BGVPumps.Ref1 = 0.0;
		}
		GetFuelTypeCo2Ref1(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.FansAndPumps.OtherResultCoolingRef1, heatedArea);
	}

	private static void Co2EnergyCalculationBuildingRef2(Results zoneBalanceResult, BuildingZone buildZone, bool isBGVused)
	{
		double heatedArea = buildZone.Heating.Area.HeatedArea;
		GetFuelTypeCo2Ref2(zoneBalanceResult, buildZone.HeatingCalculations.HeatingResult.Fuel1Ref2, buildZone.HeatingCalculations.HeatingResult.ResultSourceEnergyRef2, heatedArea);
		GetFuelTypeCo2Ref2(zoneBalanceResult, buildZone.HeatingCalculations.HeatingResult.Fuel2Ref2, buildZone.HeatingCalculations.HeatingResult.ResultSourceEnergy2Ref2, heatedArea);
		GetFuelTypeCo2Ref2(zoneBalanceResult, buildZone.CoolingCalculations.CoolingResult.Fuel1Ref2, buildZone.CoolingCalculations.CoolingResult.ResultSourceEnergyRef2, heatedArea);
		GetFuelTypeCo2Ref2(zoneBalanceResult, buildZone.CoolingCalculations.CoolingResult.Fuel2Ref2, buildZone.CoolingCalculations.CoolingResult.ResultSourceEnergy2Ref2, heatedArea);
		GetFuelTypeCo2Ref2(zoneBalanceResult, buildZone.HeatingCalculations.VentilationHeating.Fuel1Ref2, buildZone.HeatingCalculations.VentilationHeating.ResultSourceEnergyRef2, heatedArea);
		GetFuelTypeCo2Ref2(zoneBalanceResult, buildZone.HeatingCalculations.VentilationHeating.Fuel2Ref2, buildZone.HeatingCalculations.VentilationHeating.ResultSourceEnergy2Ref2, heatedArea);
		GetFuelTypeCo2Ref2(zoneBalanceResult, buildZone.CoolingCalculations.VentilationCooling.Fuel1Ref2, buildZone.CoolingCalculations.VentilationCooling.ResultSourceEnergyRef2, heatedArea);
		GetFuelTypeCo2Ref2(zoneBalanceResult, buildZone.CoolingCalculations.VentilationCooling.Fuel2Ref2, buildZone.CoolingCalculations.VentilationCooling.ResultSourceEnergy2Ref2, heatedArea);
		GetFuelTypeCo2Ref2(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.FansAndPumps.CoolNeededEnergyRef2, heatedArea);
		GetFuelTypeCo2Ref2(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.FansAndPumps.PumpNeededEnergyRef2, heatedArea);
		GetFuelTypeCo2Ref2(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.Lights.General.DevicesNeededEnergyRef2, heatedArea);
		GetFuelTypeCo2Ref2(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.BalancedDevices.General.DevicesNeededEnergyRef2, heatedArea);
		GetFuelTypeCo2Ref2(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.NonBalancedDevices.General.DevicesNeededEnergyRef2, heatedArea);
		if (isBGVused)
		{
			GetFuelTypeCo2Ref2(zoneBalanceResult, buildZone.HeatingCalculations.HotWaterCalculations.Fuel1Ref2, buildZone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergyRef2, heatedArea);
			GetFuelTypeCo2Ref2(zoneBalanceResult, buildZone.HeatingCalculations.HotWaterCalculations.Fuel2Ref2, buildZone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergy2Ref2, heatedArea);
			GetFuelTypeCo2Ref2(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.HotWaterPumps.General.DevicesNeededEnergyRef2, heatedArea);
		}
		else
		{
			zoneBalanceResult.EmissionNeededEnergyTable.BGV.Ref2 = 0.0;
			zoneBalanceResult.EmissionNeededEnergyTable.BGVPumps.Ref2 = 0.0;
		}
		GetFuelTypeCo2Ref2(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.FansAndPumps.OtherResultCoolingRef2, heatedArea);
	}

	private static void Co2EnergyCalculationBuildingActual(Results zoneBalanceResult, BuildingZone buildZone, bool isBGVused)
	{
		double heatedArea = buildZone.Heating.Area.HeatedArea;
		if (buildZone.HasHeating)
		{
			GetFuelTypeCo2Actual(zoneBalanceResult, buildZone.HeatingCalculations.HeatingResult.Fuel1Actual, buildZone.HeatingCalculations.HeatingResult.ResultSourceEnergyActual, heatedArea);
			GetFuelTypeCo2Actual(zoneBalanceResult, buildZone.HeatingCalculations.HeatingResult.Fuel2Actual, buildZone.HeatingCalculations.HeatingResult.ResultSourceEnergy2Actual, heatedArea);
			GetFuelTypeCo2Actual(zoneBalanceResult, buildZone.HeatingCalculations.VentilationHeating.Fuel1Actual, buildZone.HeatingCalculations.VentilationHeating.ResultSourceEnergyActual, heatedArea);
			GetFuelTypeCo2Actual(zoneBalanceResult, buildZone.HeatingCalculations.VentilationHeating.Fuel2Actual, buildZone.HeatingCalculations.VentilationHeating.ResultSourceEnergy2Actual, heatedArea);
		}
		if (buildZone.HasCooling)
		{
			GetFuelTypeCo2Actual(zoneBalanceResult, buildZone.CoolingCalculations.CoolingResult.Fuel1Actual, buildZone.CoolingCalculations.CoolingResult.ResultSourceEnergyActual, heatedArea);
			GetFuelTypeCo2Actual(zoneBalanceResult, buildZone.CoolingCalculations.CoolingResult.Fuel2Actual, buildZone.CoolingCalculations.CoolingResult.ResultSourceEnergy2Actual, heatedArea);
			GetFuelTypeCo2Actual(zoneBalanceResult, buildZone.CoolingCalculations.VentilationCooling.Fuel1Actual, buildZone.CoolingCalculations.VentilationCooling.ResultSourceEnergyActual, heatedArea);
			GetFuelTypeCo2Actual(zoneBalanceResult, buildZone.CoolingCalculations.VentilationCooling.Fuel2Actual, buildZone.CoolingCalculations.VentilationCooling.ResultSourceEnergy2Actual, heatedArea);
		}
		GetFuelTypeCo2Actual(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.FansAndPumps.CoolNeededEnergyActual, heatedArea);
		GetFuelTypeCo2Actual(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.FansAndPumps.PumpNeededEnergyActual, heatedArea);
		GetFuelTypeCo2Actual(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.Lights.General.DevicesNeededEnergyActual, heatedArea);
		GetFuelTypeCo2Actual(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.BalancedDevices.General.DevicesNeededEnergyActual, heatedArea);
		GetFuelTypeCo2Actual(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.NonBalancedDevices.General.DevicesNeededEnergyActual, heatedArea);
		if (isBGVused)
		{
			GetFuelTypeCo2Actual(zoneBalanceResult, buildZone.HeatingCalculations.HotWaterCalculations.Fuel1Actual, buildZone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergyActual, heatedArea);
			GetFuelTypeCo2Actual(zoneBalanceResult, buildZone.HeatingCalculations.HotWaterCalculations.Fuel2Actual, buildZone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergy2Actual, heatedArea);
			GetFuelTypeCo2Actual(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.HotWaterPumps.General.DevicesNeededEnergyActual, heatedArea);
		}
		else
		{
			zoneBalanceResult.EmissionNeededEnergyTable.BGV.Actual = 0.0;
			zoneBalanceResult.EmissionNeededEnergyTable.BGVPumps.Actual = 0.0;
		}
		GetFuelTypeCo2Actual(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.FansAndPumps.OtherResultCoolingActual, heatedArea);
	}

	private static void Co2EnergyCalculationBuildingBaseLine(Results zoneBalanceResult, BuildingZone buildZone, bool isBGVused)
	{
		double heatedArea = buildZone.Heating.Area.HeatedArea;
		if (buildZone.HasHeating)
		{
			GetFuelTypeCo2BaseLine(zoneBalanceResult, buildZone.HeatingCalculations.HeatingResult.Fuel1BaseLine, buildZone.HeatingCalculations.HeatingResult.ResultSourceEnergyBaseLine, heatedArea);
			GetFuelTypeCo2BaseLine(zoneBalanceResult, buildZone.HeatingCalculations.HeatingResult.Fuel2BaseLine, buildZone.HeatingCalculations.HeatingResult.ResultSourceEnergy2BaseLine, heatedArea);
			GetFuelTypeCo2BaseLine(zoneBalanceResult, buildZone.HeatingCalculations.VentilationHeating.Fuel1BaseLine, buildZone.HeatingCalculations.VentilationHeating.ResultSourceEnergyBaseLine, heatedArea);
			GetFuelTypeCo2BaseLine(zoneBalanceResult, buildZone.HeatingCalculations.VentilationHeating.Fuel2BaseLine, buildZone.HeatingCalculations.VentilationHeating.ResultSourceEnergy2BaseLine, heatedArea);
		}
		if (buildZone.HasCooling)
		{
			GetFuelTypeCo2BaseLine(zoneBalanceResult, buildZone.CoolingCalculations.CoolingResult.Fuel1BaseLine, buildZone.CoolingCalculations.CoolingResult.ResultSourceEnergyBaseLine, heatedArea);
			GetFuelTypeCo2BaseLine(zoneBalanceResult, buildZone.CoolingCalculations.CoolingResult.Fuel2BaseLine, buildZone.CoolingCalculations.CoolingResult.ResultSourceEnergy2BaseLine, heatedArea);
			GetFuelTypeCo2BaseLine(zoneBalanceResult, buildZone.CoolingCalculations.VentilationCooling.Fuel1BaseLine, buildZone.CoolingCalculations.VentilationCooling.ResultSourceEnergyBaseLine, heatedArea);
			GetFuelTypeCo2BaseLine(zoneBalanceResult, buildZone.CoolingCalculations.VentilationCooling.Fuel2BaseLine, buildZone.CoolingCalculations.VentilationCooling.ResultSourceEnergy2BaseLine, heatedArea);
		}
		GetFuelTypeCo2BaseLine(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.FansAndPumps.CoolNeededEnergyBaseLine, heatedArea);
		GetFuelTypeCo2BaseLine(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.FansAndPumps.PumpNeededEnergyBaseLine, heatedArea);
		GetFuelTypeCo2BaseLine(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.Lights.General.DevicesNeededEnergyBaseLine, heatedArea);
		GetFuelTypeCo2BaseLine(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.BalancedDevices.General.DevicesNeededEnergyBaseLine, heatedArea);
		GetFuelTypeCo2BaseLine(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.NonBalancedDevices.General.DevicesNeededEnergyBaseLine, heatedArea);
		if (isBGVused)
		{
			GetFuelTypeCo2BaseLine(zoneBalanceResult, buildZone.HeatingCalculations.HotWaterCalculations.Fuel1BaseLine, buildZone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergyBaseLine, heatedArea);
			GetFuelTypeCo2BaseLine(zoneBalanceResult, buildZone.HeatingCalculations.HotWaterCalculations.Fuel2BaseLine, buildZone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergy2BaseLine, heatedArea);
			GetFuelTypeCo2BaseLine(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.HotWaterPumps.General.DevicesNeededEnergyBaseLine, heatedArea);
		}
		else
		{
			zoneBalanceResult.EmissionNeededEnergyTable.BGV.BaseLine = 0.0;
			zoneBalanceResult.EmissionNeededEnergyTable.BGVPumps.BaseLine = 0.0;
		}
		GetFuelTypeCo2BaseLine(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.FansAndPumps.OtherResultCoolingBaseLine, heatedArea);
	}

	private static void Co2EnergyCalculationBuildingESM(Results zoneBalanceResult, BuildingZone buildZone, bool isBGVused)
	{
		double heatedArea = buildZone.Heating.Area.HeatedArea;
		if (buildZone.HasHeating)
		{
			GetFuelTypeCo2ESM(zoneBalanceResult, buildZone.HeatingCalculations.HeatingResult.Fuel1ESM, buildZone.HeatingCalculations.HeatingResult.ResultSourceEnergyESM, heatedArea);
			GetFuelTypeCo2ESM(zoneBalanceResult, buildZone.HeatingCalculations.HeatingResult.Fuel2ESM, buildZone.HeatingCalculations.HeatingResult.ResultSourceEnergy2ESM, heatedArea);
			GetFuelTypeCo2ESM(zoneBalanceResult, buildZone.HeatingCalculations.VentilationHeating.Fuel1ESM, buildZone.HeatingCalculations.VentilationHeating.ResultSourceEnergyESM, heatedArea);
			GetFuelTypeCo2ESM(zoneBalanceResult, buildZone.HeatingCalculations.VentilationHeating.Fuel2ESM, buildZone.HeatingCalculations.VentilationHeating.ResultSourceEnergy2ESM, heatedArea);
		}
		if (buildZone.HasCooling)
		{
			GetFuelTypeCo2ESM(zoneBalanceResult, buildZone.CoolingCalculations.CoolingResult.Fuel1ESM, buildZone.CoolingCalculations.CoolingResult.ResultSourceEnergyESM, heatedArea);
			GetFuelTypeCo2ESM(zoneBalanceResult, buildZone.CoolingCalculations.CoolingResult.Fuel2ESM, buildZone.CoolingCalculations.CoolingResult.ResultSourceEnergy2ESM, heatedArea);
			GetFuelTypeCo2ESM(zoneBalanceResult, buildZone.CoolingCalculations.VentilationCooling.Fuel1ESM, buildZone.CoolingCalculations.VentilationCooling.ResultSourceEnergyESM, heatedArea);
			GetFuelTypeCo2ESM(zoneBalanceResult, buildZone.CoolingCalculations.VentilationCooling.Fuel2ESM, buildZone.CoolingCalculations.VentilationCooling.ResultSourceEnergy2ESM, heatedArea);
		}
		GetFuelTypeCo2ESM(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.FansAndPumps.CoolNeededEnergyESM, heatedArea);
		GetFuelTypeCo2ESM(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.FansAndPumps.PumpNeededEnergyESM, heatedArea);
		GetFuelTypeCo2ESM(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.Lights.General.DevicesNeededEnergyESM, heatedArea);
		GetFuelTypeCo2ESM(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.BalancedDevices.General.DevicesNeededEnergyESM, heatedArea);
		GetFuelTypeCo2ESM(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.NonBalancedDevices.General.DevicesNeededEnergyESM, heatedArea);
		if (isBGVused)
		{
			GetFuelTypeCo2ESM(zoneBalanceResult, buildZone.HeatingCalculations.HotWaterCalculations.Fuel1ESM, buildZone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergyESM, heatedArea);
			GetFuelTypeCo2ESM(zoneBalanceResult, buildZone.HeatingCalculations.HotWaterCalculations.Fuel2ESM, buildZone.HeatingCalculations.HotWaterCalculations.ResultSourceEnergy2ESM, heatedArea);
			GetFuelTypeCo2ESM(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.LightAndDevices.HotWaterPumps.General.DevicesNeededEnergyESM, heatedArea);
		}
		else
		{
			zoneBalanceResult.EmissionNeededEnergyTable.BGV.ESM = 0.0;
			zoneBalanceResult.EmissionNeededEnergyTable.BGVPumps.ESM = 0.0;
		}
		GetFuelTypeCo2ESM(zoneBalanceResult, Fuel.Fuel1, buildZone.HeatingCalculations.FansAndPumps.OtherResultCoolingESM, heatedArea);
	}

	private static void Co2CalculateEmissionEnergySupplyBuilding(Results buildingResults)
	{
		Co2EnergyCalcBuildingRef1(buildingResults);
		Co2EnergyCalcBuildingRef2(buildingResults);
		Co2EnergyCalcBuildingActual(buildingResults);
		Co2EnergyCalcBuildingBaseLine(buildingResults);
		Co2EnergyCalcBuildingESM(buildingResults);
	}

	private static void Co2EnergyCalcBuildingRef1(Results zoneBalanceResult)
	{
		double conditionedArea = zoneBalanceResult.NeededEnergyTable.ConditionedArea;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel1.Ref1 = zoneBalanceResult.EmissionEnergySupplyTable.Fuel1.Ref1 * 267.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel2.Ref1 = zoneBalanceResult.EmissionEnergySupplyTable.Fuel2.Ref1 * 202.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel3.Ref1 = zoneBalanceResult.EmissionEnergySupplyTable.Fuel3.Ref1 * 227.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel4.Ref1 = zoneBalanceResult.EmissionEnergySupplyTable.Fuel4.Ref1 * 341.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel5.Ref1 = zoneBalanceResult.EmissionEnergySupplyTable.Fuel5.Ref1 * 364.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel6.Ref1 = zoneBalanceResult.EmissionEnergySupplyTable.Fuel6.Ref1 * 43.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel7.Ref1 = zoneBalanceResult.EmissionEnergySupplyTable.Fuel7.Ref1 * 351.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel8.Ref1 = zoneBalanceResult.EmissionEnergySupplyTable.Fuel8.Ref1 * 819.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel9.Ref1 = zoneBalanceResult.EmissionEnergySupplyTable.Fuel9.Ref1 * 290.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel10.Ref1 = zoneBalanceResult.EmissionEnergySupplyTable.Fuel10.Ref1 * 279.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel11.Ref1 = zoneBalanceResult.EmissionEnergySupplyTable.Fuel11.Ref1 * 354.0 / 1000.0;
	}

	private static void Co2EnergyCalcBuildingRef2(Results zoneBalanceResult)
	{
		double conditionedArea = zoneBalanceResult.NeededEnergyTable.ConditionedArea;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel1.Ref2 = zoneBalanceResult.EmissionEnergySupplyTable.Fuel1.Ref2 * 267.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel2.Ref2 = zoneBalanceResult.EmissionEnergySupplyTable.Fuel2.Ref2 * 202.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel3.Ref2 = zoneBalanceResult.EmissionEnergySupplyTable.Fuel3.Ref2 * 227.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel4.Ref2 = zoneBalanceResult.EmissionEnergySupplyTable.Fuel4.Ref2 * 341.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel5.Ref2 = zoneBalanceResult.EmissionEnergySupplyTable.Fuel5.Ref2 * 364.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel6.Ref2 = zoneBalanceResult.EmissionEnergySupplyTable.Fuel6.Ref2 * 43.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel7.Ref2 = zoneBalanceResult.EmissionEnergySupplyTable.Fuel7.Ref2 * 351.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel8.Ref2 = zoneBalanceResult.EmissionEnergySupplyTable.Fuel8.Ref2 * 819.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel9.Ref2 = zoneBalanceResult.EmissionEnergySupplyTable.Fuel9.Ref2 * 290.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel10.Ref2 = zoneBalanceResult.EmissionEnergySupplyTable.Fuel10.Ref2 * 279.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel11.Ref2 = zoneBalanceResult.EmissionEnergySupplyTable.Fuel11.Ref2 * 354.0 / 1000.0;
	}

	private static void Co2EnergyCalcBuildingActual(Results zoneBalanceResult)
	{
		double conditionedArea = zoneBalanceResult.NeededEnergyTable.ConditionedArea;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel1.Actual = zoneBalanceResult.EmissionEnergySupplyTable.Fuel1.Actual * 267.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel2.Actual = zoneBalanceResult.EmissionEnergySupplyTable.Fuel2.Actual * 202.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel3.Actual = zoneBalanceResult.EmissionEnergySupplyTable.Fuel3.Actual * 227.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel4.Actual = zoneBalanceResult.EmissionEnergySupplyTable.Fuel4.Actual * 341.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel5.Actual = zoneBalanceResult.EmissionEnergySupplyTable.Fuel5.Actual * 364.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel6.Actual = zoneBalanceResult.EmissionEnergySupplyTable.Fuel6.Actual * 43.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel7.Actual = zoneBalanceResult.EmissionEnergySupplyTable.Fuel7.Actual * 351.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel8.Actual = zoneBalanceResult.EmissionEnergySupplyTable.Fuel8.Actual * 819.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel9.Actual = zoneBalanceResult.EmissionEnergySupplyTable.Fuel9.Actual * 290.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel10.Actual = zoneBalanceResult.EmissionEnergySupplyTable.Fuel10.Actual * 279.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel11.Actual = zoneBalanceResult.EmissionEnergySupplyTable.Fuel11.Actual * 354.0 / 1000.0;
	}

	private static void Co2EnergyCalcBuildingBaseLine(Results zoneBalanceResult)
	{
		double conditionedArea = zoneBalanceResult.NeededEnergyTable.ConditionedArea;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel1.BaseLine = zoneBalanceResult.EmissionEnergySupplyTable.Fuel1.BaseLine * 267.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel2.BaseLine = zoneBalanceResult.EmissionEnergySupplyTable.Fuel2.BaseLine * 202.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel3.BaseLine = zoneBalanceResult.EmissionEnergySupplyTable.Fuel3.BaseLine * 227.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel4.BaseLine = zoneBalanceResult.EmissionEnergySupplyTable.Fuel4.BaseLine * 341.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel5.BaseLine = zoneBalanceResult.EmissionEnergySupplyTable.Fuel5.BaseLine * 364.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel6.BaseLine = zoneBalanceResult.EmissionEnergySupplyTable.Fuel6.BaseLine * 43.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel7.BaseLine = zoneBalanceResult.EmissionEnergySupplyTable.Fuel7.BaseLine * 351.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel8.BaseLine = zoneBalanceResult.EmissionEnergySupplyTable.Fuel8.BaseLine * 819.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel9.BaseLine = zoneBalanceResult.EmissionEnergySupplyTable.Fuel9.BaseLine * 290.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel10.BaseLine = zoneBalanceResult.EmissionEnergySupplyTable.Fuel10.BaseLine * 279.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel11.BaseLine = zoneBalanceResult.EmissionEnergySupplyTable.Fuel11.BaseLine * 354.0 / 1000.0;
	}

	private static void Co2EnergyCalcBuildingESM(Results zoneBalanceResult)
	{
		double conditionedArea = zoneBalanceResult.NeededEnergyTable.ConditionedArea;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel1.ESM = zoneBalanceResult.EmissionEnergySupplyTable.Fuel1.ESM * 267.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel2.ESM = zoneBalanceResult.EmissionEnergySupplyTable.Fuel2.ESM * 202.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel3.ESM = zoneBalanceResult.EmissionEnergySupplyTable.Fuel3.ESM * 227.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel4.ESM = zoneBalanceResult.EmissionEnergySupplyTable.Fuel4.ESM * 341.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel5.ESM = zoneBalanceResult.EmissionEnergySupplyTable.Fuel5.ESM * 364.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel6.ESM = zoneBalanceResult.EmissionEnergySupplyTable.Fuel6.ESM * 43.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel7.ESM = zoneBalanceResult.EmissionEnergySupplyTable.Fuel7.ESM * 351.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel8.ESM = zoneBalanceResult.EmissionEnergySupplyTable.Fuel8.ESM * 819.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel9.ESM = zoneBalanceResult.EmissionEnergySupplyTable.Fuel9.ESM * 290.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel10.ESM = zoneBalanceResult.EmissionEnergySupplyTable.Fuel10.ESM * 279.0 / 1000.0;
		zoneBalanceResult.EmissionEnergySupplyTable.Fuel11.ESM = zoneBalanceResult.EmissionEnergySupplyTable.Fuel11.ESM * 354.0 / 1000.0;
	}

	private static void Co2EnergyCalculateTotal(Results zoneBalanceResult)
	{
		zoneBalanceResult.EmissionEnergySupplyTable.Total.Ref1 = zoneBalanceResult.EmissionEnergySupplyTable.Fuel1.Ref1 + zoneBalanceResult.EmissionEnergySupplyTable.Fuel2.Ref1 + zoneBalanceResult.EmissionEnergySupplyTable.Fuel3.Ref1 + zoneBalanceResult.EmissionEnergySupplyTable.Fuel4.Ref1 + zoneBalanceResult.EmissionEnergySupplyTable.Fuel5.Ref1 + zoneBalanceResult.EmissionEnergySupplyTable.Fuel6.Ref1 + zoneBalanceResult.EmissionEnergySupplyTable.Fuel7.Ref1 + zoneBalanceResult.EmissionEnergySupplyTable.Fuel8.Ref1 + zoneBalanceResult.EmissionEnergySupplyTable.Fuel9.Ref1 + zoneBalanceResult.EmissionEnergySupplyTable.Fuel10.Ref1 + zoneBalanceResult.EmissionEnergySupplyTable.Fuel11.Ref1;
		zoneBalanceResult.EmissionEnergySupplyTable.Total.Ref2 = zoneBalanceResult.EmissionEnergySupplyTable.Fuel1.Ref2 + zoneBalanceResult.EmissionEnergySupplyTable.Fuel2.Ref2 + zoneBalanceResult.EmissionEnergySupplyTable.Fuel3.Ref2 + zoneBalanceResult.EmissionEnergySupplyTable.Fuel4.Ref2 + zoneBalanceResult.EmissionEnergySupplyTable.Fuel5.Ref2 + zoneBalanceResult.EmissionEnergySupplyTable.Fuel6.Ref2 + zoneBalanceResult.EmissionEnergySupplyTable.Fuel7.Ref2 + zoneBalanceResult.EmissionEnergySupplyTable.Fuel8.Ref2 + zoneBalanceResult.EmissionEnergySupplyTable.Fuel9.Ref2 + zoneBalanceResult.EmissionEnergySupplyTable.Fuel10.Ref2 + zoneBalanceResult.EmissionEnergySupplyTable.Fuel11.Ref2;
		zoneBalanceResult.EmissionEnergySupplyTable.Total.Actual = zoneBalanceResult.EmissionEnergySupplyTable.Fuel1.Actual + zoneBalanceResult.EmissionEnergySupplyTable.Fuel2.Actual + zoneBalanceResult.EmissionEnergySupplyTable.Fuel3.Actual + zoneBalanceResult.EmissionEnergySupplyTable.Fuel4.Actual + zoneBalanceResult.EmissionEnergySupplyTable.Fuel5.Actual + zoneBalanceResult.EmissionEnergySupplyTable.Fuel6.Actual + zoneBalanceResult.EmissionEnergySupplyTable.Fuel7.Actual + zoneBalanceResult.EmissionEnergySupplyTable.Fuel8.Actual + zoneBalanceResult.EmissionEnergySupplyTable.Fuel9.Actual + zoneBalanceResult.EmissionEnergySupplyTable.Fuel10.Actual + zoneBalanceResult.EmissionEnergySupplyTable.Fuel11.Actual;
		zoneBalanceResult.EmissionEnergySupplyTable.Total.BaseLine = zoneBalanceResult.EmissionEnergySupplyTable.Fuel1.BaseLine + zoneBalanceResult.EmissionEnergySupplyTable.Fuel2.BaseLine + zoneBalanceResult.EmissionEnergySupplyTable.Fuel3.BaseLine + zoneBalanceResult.EmissionEnergySupplyTable.Fuel4.BaseLine + zoneBalanceResult.EmissionEnergySupplyTable.Fuel5.BaseLine + zoneBalanceResult.EmissionEnergySupplyTable.Fuel6.BaseLine + zoneBalanceResult.EmissionEnergySupplyTable.Fuel7.BaseLine + zoneBalanceResult.EmissionEnergySupplyTable.Fuel8.BaseLine + zoneBalanceResult.EmissionEnergySupplyTable.Fuel9.BaseLine + zoneBalanceResult.EmissionEnergySupplyTable.Fuel10.BaseLine + zoneBalanceResult.EmissionEnergySupplyTable.Fuel11.BaseLine;
		zoneBalanceResult.EmissionEnergySupplyTable.Total.ESM = zoneBalanceResult.EmissionEnergySupplyTable.Fuel1.ESM + zoneBalanceResult.EmissionEnergySupplyTable.Fuel2.ESM + zoneBalanceResult.EmissionEnergySupplyTable.Fuel3.ESM + zoneBalanceResult.EmissionEnergySupplyTable.Fuel4.ESM + zoneBalanceResult.EmissionEnergySupplyTable.Fuel5.ESM + zoneBalanceResult.EmissionEnergySupplyTable.Fuel6.ESM + zoneBalanceResult.EmissionEnergySupplyTable.Fuel7.ESM + zoneBalanceResult.EmissionEnergySupplyTable.Fuel8.ESM + zoneBalanceResult.EmissionEnergySupplyTable.Fuel9.ESM + zoneBalanceResult.EmissionEnergySupplyTable.Fuel10.ESM + zoneBalanceResult.EmissionEnergySupplyTable.Fuel11.ESM;
	}

	private static void GetFuelTypeCo2Ref1(Results zoneBalanceResult, Fuel fuel, double quantity, double area)
	{
		if (double.IsInfinity(quantity) || double.IsNaN(quantity))
		{
			quantity = 0.0;
		}
		switch (fuel)
		{
		case Fuel.Fuel1:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel8.Ref1 += quantity * area / 1000.0;
			break;
		case Fuel.Fuel2:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel2.Ref1 += quantity * area / 1000.0;
			break;
		case Fuel.Fuel3:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel3.Ref1 += quantity * area / 1000.0;
			break;
		case Fuel.Fuel4:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel4.Ref1 += quantity * area / 1000.0;
			break;
		case Fuel.Fuel5:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel5.Ref1 += quantity * area / 1000.0;
			break;
		case Fuel.Fuel6:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel6.Ref1 += quantity * area / 1000.0;
			break;
		case Fuel.Fuel7:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel7.Ref1 += quantity * area / 1000.0;
			break;
		case Fuel.Fuel8:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel1.Ref1 += quantity * area / 1000.0;
			break;
		case Fuel.Fuel9:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel9.Ref1 += quantity * area / 1000.0;
			break;
		case Fuel.Fuel10:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel10.Ref1 += quantity * area / 1000.0;
			break;
		case Fuel.Fuel11:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel11.Ref1 += quantity * area / 1000.0;
			break;
		}
	}

	private static void GetFuelTypeCo2Ref2(Results zoneBalanceResult, Fuel fuel, double quantity, double area)
	{
		if (double.IsInfinity(quantity) || double.IsNaN(quantity))
		{
			quantity = 0.0;
		}
		switch (fuel)
		{
		case Fuel.Fuel1:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel8.Ref2 += quantity * area / 1000.0;
			break;
		case Fuel.Fuel2:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel2.Ref2 += quantity * area / 1000.0;
			break;
		case Fuel.Fuel3:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel3.Ref2 += quantity * area / 1000.0;
			break;
		case Fuel.Fuel4:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel4.Ref2 += quantity * area / 1000.0;
			break;
		case Fuel.Fuel5:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel5.Ref2 += quantity * area / 1000.0;
			break;
		case Fuel.Fuel6:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel6.Ref2 += quantity * area / 1000.0;
			break;
		case Fuel.Fuel7:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel7.Ref2 += quantity * area / 1000.0;
			break;
		case Fuel.Fuel8:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel1.Ref2 += quantity * area / 1000.0;
			break;
		case Fuel.Fuel9:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel9.Ref2 += quantity * area / 1000.0;
			break;
		case Fuel.Fuel10:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel10.Ref2 += quantity * area / 1000.0;
			break;
		case Fuel.Fuel11:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel11.Ref2 += quantity * area / 1000.0;
			break;
		}
	}

	private static void GetFuelTypeCo2Actual(Results zoneBalanceResult, Fuel fuel, double quantity, double area)
	{
		if (double.IsInfinity(quantity) || double.IsNaN(quantity))
		{
			quantity = 0.0;
		}
		switch (fuel)
		{
		case Fuel.Fuel1:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel8.Actual += quantity * area / 1000.0;
			break;
		case Fuel.Fuel2:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel2.Actual += quantity * area / 1000.0;
			break;
		case Fuel.Fuel3:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel3.Actual += quantity * area / 1000.0;
			break;
		case Fuel.Fuel4:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel4.Actual += quantity * area / 1000.0;
			break;
		case Fuel.Fuel5:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel5.Actual += quantity * area / 1000.0;
			break;
		case Fuel.Fuel6:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel6.Actual += quantity * area / 1000.0;
			break;
		case Fuel.Fuel7:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel7.Actual += quantity * area / 1000.0;
			break;
		case Fuel.Fuel8:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel1.Actual += quantity * area / 1000.0;
			break;
		case Fuel.Fuel9:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel9.Actual += quantity * area / 1000.0;
			break;
		case Fuel.Fuel10:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel10.Actual += quantity * area / 1000.0;
			break;
		case Fuel.Fuel11:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel11.Actual += quantity * area / 1000.0;
			break;
		}
	}

	private static void GetFuelTypeCo2BaseLine(Results zoneBalanceResult, Fuel fuel, double quantity, double area)
	{
		if (double.IsInfinity(quantity) || double.IsNaN(quantity))
		{
			quantity = 0.0;
		}
		switch (fuel)
		{
		case Fuel.Fuel1:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel8.BaseLine += quantity * area / 1000.0;
			break;
		case Fuel.Fuel2:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel2.BaseLine += quantity * area / 1000.0;
			break;
		case Fuel.Fuel3:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel3.BaseLine += quantity * area / 1000.0;
			break;
		case Fuel.Fuel4:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel4.BaseLine += quantity * area / 1000.0;
			break;
		case Fuel.Fuel5:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel5.BaseLine += quantity * area / 1000.0;
			break;
		case Fuel.Fuel6:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel6.BaseLine += quantity * area / 1000.0;
			break;
		case Fuel.Fuel7:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel7.BaseLine += quantity * area / 1000.0;
			break;
		case Fuel.Fuel8:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel1.BaseLine += quantity * area / 1000.0;
			break;
		case Fuel.Fuel9:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel9.BaseLine += quantity * area / 1000.0;
			break;
		case Fuel.Fuel10:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel10.BaseLine += quantity * area / 1000.0;
			break;
		case Fuel.Fuel11:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel11.BaseLine += quantity * area / 1000.0;
			break;
		}
	}

	private static void GetFuelTypeCo2ESM(Results zoneBalanceResult, Fuel fuel, double quantity, double area)
	{
		if (double.IsInfinity(quantity) || double.IsNaN(quantity))
		{
			quantity = 0.0;
		}
		switch (fuel)
		{
		case Fuel.Fuel1:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel8.ESM += quantity * area / 1000.0;
			break;
		case Fuel.Fuel2:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel2.ESM += quantity * area / 1000.0;
			break;
		case Fuel.Fuel3:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel3.ESM += quantity * area / 1000.0;
			break;
		case Fuel.Fuel4:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel4.ESM += quantity * area / 1000.0;
			break;
		case Fuel.Fuel5:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel5.ESM += quantity * area / 1000.0;
			break;
		case Fuel.Fuel6:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel6.ESM += quantity * area / 1000.0;
			break;
		case Fuel.Fuel7:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel7.ESM += quantity * area / 1000.0;
			break;
		case Fuel.Fuel8:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel1.ESM += quantity * area / 1000.0;
			break;
		case Fuel.Fuel9:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel9.ESM += quantity * area / 1000.0;
			break;
		case Fuel.Fuel10:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel10.ESM += quantity * area / 1000.0;
			break;
		case Fuel.Fuel11:
			zoneBalanceResult.EmissionEnergySupplyTable.Fuel11.ESM += quantity * area / 1000.0;
			break;
		}
	}

	private static double GetEkoCoeficient(Fuel fuel, double quantity)
	{
		if (double.IsInfinity(quantity) || double.IsNaN(quantity))
		{
			quantity = 0.0;
		}
		return fuel switch
		{
			Fuel.Fuel1 => quantity * 819.0, 
			Fuel.Fuel2 => quantity * 202.0, 
			Fuel.Fuel3 => quantity * 227.0, 
			Fuel.Fuel4 => quantity * 341.0, 
			Fuel.Fuel5 => quantity * 364.0, 
			Fuel.Fuel6 => quantity * 43.0, 
			Fuel.Fuel7 => quantity * 351.0, 
			Fuel.Fuel8 => quantity * 267.0, 
			Fuel.Fuel9 => quantity * 290.0, 
			Fuel.Fuel10 => quantity * 279.0, 
			Fuel.Fuel11 => quantity * 354.0, 
			_ => 0.0, 
		};
	}

	private static double CalcTotalArea(CalculationInput calcInput)
	{
		return calcInput.General.BuildingResults.TotalAreaElements.TotalHeatedArea;
	}

	private static void GetConditionedArea(CalculationInput calcInput, Results buildingBalanceResult)
	{
		buildingBalanceResult.NeededEnergyTable.ConditionedArea = calcInput.General.BuildingResults.TotalAreaElements.TotalHeatedArea;
		buildingBalanceResult.NetEnergyTable.ConditionedArea = calcInput.General.BuildingResults.TotalAreaElements.TotalHeatedArea;
	}

	private static void SetFuelValue(Results buildingBalanceResult, double area)
	{
		CalculateFuelRef1(buildingBalanceResult, area);
		CalculateFuelRef2(buildingBalanceResult, area);
		CalculateFuelActual(buildingBalanceResult, area);
		CalculateFuelBaseLine(buildingBalanceResult, area);
		CalculateFuelESM(buildingBalanceResult, area);
	}

	private static void CalculateFuelESM(Results buildingBalanceResult, double area)
	{
		buildingBalanceResult.FuelEnergyTable.Fuel8.ESMArea = buildingBalanceResult.FuelEnergyTable.Fuel8.ESMArea / area;
		buildingBalanceResult.FuelEnergyTable.Fuel1.ESMArea = buildingBalanceResult.FuelEnergyTable.Fuel1.ESMArea / area;
		buildingBalanceResult.FuelEnergyTable.Fuel2.ESMArea = buildingBalanceResult.FuelEnergyTable.Fuel2.ESMArea / area;
		buildingBalanceResult.FuelEnergyTable.Fuel3.ESMArea = buildingBalanceResult.FuelEnergyTable.Fuel3.ESMArea / area;
		buildingBalanceResult.FuelEnergyTable.Fuel4.ESMArea = buildingBalanceResult.FuelEnergyTable.Fuel4.ESMArea / area;
		buildingBalanceResult.FuelEnergyTable.Fuel5.ESMArea = buildingBalanceResult.FuelEnergyTable.Fuel5.ESMArea / area;
		buildingBalanceResult.FuelEnergyTable.Fuel6.ESMArea = buildingBalanceResult.FuelEnergyTable.Fuel6.ESMArea / area;
		buildingBalanceResult.FuelEnergyTable.Fuel7.ESMArea = buildingBalanceResult.FuelEnergyTable.Fuel7.ESMArea / area;
		buildingBalanceResult.FuelEnergyTable.Fuel9.ESMArea = buildingBalanceResult.FuelEnergyTable.Fuel9.ESMArea / area;
		buildingBalanceResult.FuelEnergyTable.Fuel10.ESMArea = buildingBalanceResult.FuelEnergyTable.Fuel10.ESMArea / area;
		buildingBalanceResult.FuelEnergyTable.Fuel11.ESMArea = buildingBalanceResult.FuelEnergyTable.Fuel11.ESMArea / area;
	}

	private static void CalculateFuelBaseLine(Results buildingBalanceResult, double area)
	{
		buildingBalanceResult.FuelEnergyTable.Fuel8.BaseLineArea = buildingBalanceResult.FuelEnergyTable.Fuel8.BaseLineArea / area;
		buildingBalanceResult.FuelEnergyTable.Fuel1.BaseLineArea = buildingBalanceResult.FuelEnergyTable.Fuel1.BaseLineArea / area;
		buildingBalanceResult.FuelEnergyTable.Fuel2.BaseLineArea = buildingBalanceResult.FuelEnergyTable.Fuel2.BaseLineArea / area;
		buildingBalanceResult.FuelEnergyTable.Fuel3.BaseLineArea = buildingBalanceResult.FuelEnergyTable.Fuel3.BaseLineArea / area;
		buildingBalanceResult.FuelEnergyTable.Fuel4.BaseLineArea = buildingBalanceResult.FuelEnergyTable.Fuel4.BaseLineArea / area;
		buildingBalanceResult.FuelEnergyTable.Fuel5.BaseLineArea = buildingBalanceResult.FuelEnergyTable.Fuel5.BaseLineArea / area;
		buildingBalanceResult.FuelEnergyTable.Fuel6.BaseLineArea = buildingBalanceResult.FuelEnergyTable.Fuel6.BaseLineArea / area;
		buildingBalanceResult.FuelEnergyTable.Fuel7.BaseLineArea = buildingBalanceResult.FuelEnergyTable.Fuel7.BaseLineArea / area;
		buildingBalanceResult.FuelEnergyTable.Fuel9.BaseLineArea = buildingBalanceResult.FuelEnergyTable.Fuel9.BaseLineArea / area;
		buildingBalanceResult.FuelEnergyTable.Fuel10.BaseLineArea = buildingBalanceResult.FuelEnergyTable.Fuel10.BaseLineArea / area;
		buildingBalanceResult.FuelEnergyTable.Fuel11.BaseLineArea = buildingBalanceResult.FuelEnergyTable.Fuel11.BaseLineArea / area;
	}

	private static void CalculateFuelActual(Results buildingBalanceResult, double area)
	{
		buildingBalanceResult.FuelEnergyTable.Fuel8.ActualArea = buildingBalanceResult.FuelEnergyTable.Fuel8.ActualArea / area;
		buildingBalanceResult.FuelEnergyTable.Fuel1.ActualArea = buildingBalanceResult.FuelEnergyTable.Fuel1.ActualArea / area;
		buildingBalanceResult.FuelEnergyTable.Fuel2.ActualArea = buildingBalanceResult.FuelEnergyTable.Fuel2.ActualArea / area;
		buildingBalanceResult.FuelEnergyTable.Fuel3.ActualArea = buildingBalanceResult.FuelEnergyTable.Fuel3.ActualArea / area;
		buildingBalanceResult.FuelEnergyTable.Fuel4.ActualArea = buildingBalanceResult.FuelEnergyTable.Fuel4.ActualArea / area;
		buildingBalanceResult.FuelEnergyTable.Fuel5.ActualArea = buildingBalanceResult.FuelEnergyTable.Fuel5.ActualArea / area;
		buildingBalanceResult.FuelEnergyTable.Fuel6.ActualArea = buildingBalanceResult.FuelEnergyTable.Fuel6.ActualArea / area;
		buildingBalanceResult.FuelEnergyTable.Fuel7.ActualArea = buildingBalanceResult.FuelEnergyTable.Fuel7.ActualArea / area;
		buildingBalanceResult.FuelEnergyTable.Fuel9.ActualArea = buildingBalanceResult.FuelEnergyTable.Fuel9.ActualArea / area;
		buildingBalanceResult.FuelEnergyTable.Fuel10.ActualArea = buildingBalanceResult.FuelEnergyTable.Fuel10.ActualArea / area;
		buildingBalanceResult.FuelEnergyTable.Fuel11.ActualArea = buildingBalanceResult.FuelEnergyTable.Fuel11.ActualArea / area;
	}

	private static void CalculateFuelRef2(Results buildingBalanceResult, double area)
	{
		buildingBalanceResult.FuelEnergyTable.Fuel8.Ref2Area = buildingBalanceResult.FuelEnergyTable.Fuel8.Ref2Area / area;
		buildingBalanceResult.FuelEnergyTable.Fuel1.Ref2Area = buildingBalanceResult.FuelEnergyTable.Fuel1.Ref2Area / area;
		buildingBalanceResult.FuelEnergyTable.Fuel2.Ref2Area = buildingBalanceResult.FuelEnergyTable.Fuel2.Ref2Area / area;
		buildingBalanceResult.FuelEnergyTable.Fuel3.Ref2Area = buildingBalanceResult.FuelEnergyTable.Fuel3.Ref2Area / area;
		buildingBalanceResult.FuelEnergyTable.Fuel4.Ref2Area = buildingBalanceResult.FuelEnergyTable.Fuel4.Ref2Area / area;
		buildingBalanceResult.FuelEnergyTable.Fuel5.Ref2Area = buildingBalanceResult.FuelEnergyTable.Fuel5.Ref2Area / area;
		buildingBalanceResult.FuelEnergyTable.Fuel6.Ref2Area = buildingBalanceResult.FuelEnergyTable.Fuel6.Ref2Area / area;
		buildingBalanceResult.FuelEnergyTable.Fuel7.Ref2Area = buildingBalanceResult.FuelEnergyTable.Fuel7.Ref2Area / area;
		buildingBalanceResult.FuelEnergyTable.Fuel9.Ref2Area = buildingBalanceResult.FuelEnergyTable.Fuel9.Ref2Area / area;
		buildingBalanceResult.FuelEnergyTable.Fuel10.Ref2Area = buildingBalanceResult.FuelEnergyTable.Fuel10.Ref2Area / area;
		buildingBalanceResult.FuelEnergyTable.Fuel11.Ref2Area = buildingBalanceResult.FuelEnergyTable.Fuel11.Ref2Area / area;
	}

	private static void CalculateFuelRef1(Results buildingBalanceResult, double area)
	{
		buildingBalanceResult.FuelEnergyTable.Fuel8.Ref1Area = buildingBalanceResult.FuelEnergyTable.Fuel8.Ref1Area / area;
		buildingBalanceResult.FuelEnergyTable.Fuel1.Ref1Area = buildingBalanceResult.FuelEnergyTable.Fuel1.Ref1Area / area;
		buildingBalanceResult.FuelEnergyTable.Fuel2.Ref1Area = buildingBalanceResult.FuelEnergyTable.Fuel2.Ref1Area / area;
		buildingBalanceResult.FuelEnergyTable.Fuel3.Ref1Area = buildingBalanceResult.FuelEnergyTable.Fuel3.Ref1Area / area;
		buildingBalanceResult.FuelEnergyTable.Fuel4.Ref1Area = buildingBalanceResult.FuelEnergyTable.Fuel4.Ref1Area / area;
		buildingBalanceResult.FuelEnergyTable.Fuel5.Ref1Area = buildingBalanceResult.FuelEnergyTable.Fuel5.Ref1Area / area;
		buildingBalanceResult.FuelEnergyTable.Fuel6.Ref1Area = buildingBalanceResult.FuelEnergyTable.Fuel6.Ref1Area / area;
		buildingBalanceResult.FuelEnergyTable.Fuel7.Ref1Area = buildingBalanceResult.FuelEnergyTable.Fuel7.Ref1Area / area;
		buildingBalanceResult.FuelEnergyTable.Fuel9.Ref1Area = buildingBalanceResult.FuelEnergyTable.Fuel9.Ref1Area / area;
		buildingBalanceResult.FuelEnergyTable.Fuel10.Ref1Area = buildingBalanceResult.FuelEnergyTable.Fuel10.Ref1Area / area;
		buildingBalanceResult.FuelEnergyTable.Fuel11.Ref1Area = buildingBalanceResult.FuelEnergyTable.Fuel11.Ref1Area / area;
	}

	public static void CalculateHeatingSavings(this CalculationData calcData, Section section, CalculationInput calcInput, BuildingZone zone, CalculationData lightsAndDevicesCalculationData)
	{
		if (buildingZone == null)
		{
			return;
		}
		if (!buildingZone.HasHeating)
		{
			AddSavingsToZone(new List<SavingsData>(), zone, "Отопление");
			return;
		}
		publicCalculationData = calcData;
		CalculationData calculationData = calcData.Clone();
		IList<SavingsData> list = CheckForSavings("Отопление");
		CheckForDifferentFuelSources(calculationData);
		CommonExtensions.AddRange<SavingsData>((ICollection<SavingsData>)list, (IEnumerable<SavingsData>)CheckForFuelSavings("Отопление", calculationData));
		SetSavingsValues(list);
		if (list.Any())
		{
			List<DataRow> source = CreateHeatingVirtualBaseLine(calculationData, section, calcInput, zone, lightsAndDevicesCalculationData);
			List<DataRow> source2 = CreateHeatingVirtualESM(calculationData, section, calcInput, zone, lightsAndDevicesCalculationData);
			virtualBaseLineNetEnergy = source.FirstOrDefault((DataRow o) => o.Tag == "ResultNeededEnergy").Value;
			virtualESMNetEnergy = source2.FirstOrDefault((DataRow o) => o.Tag == "ResultNeededEnergy").Value;
			foreach (SavingsData saving in list)
			{
				calculationData = calcData.Clone();
				Section section2 = EECalcCore.EntityBase<Section>.Deserialize(section.Serialize());
				CheckForDifferentFuelSources(calculationData);
				List<DataRow> list2 = CreateHeatingVirtualBaseLine(calculationData, section2, calcInput, zone, lightsAndDevicesCalculationData).ToList();
				DataRow dataRow = list2.FirstOrDefault((DataRow o) => o.Tag == saving.Tag);
				if (dataRow != null)
				{
					if (saving.Tag.StartsWith("U") || saving.Tag == "g")
					{
						CalculateUsavingType(saving.Tag, section2, section);
					}
					else if (saving.Tag == "WorkingSchedule")
					{
						CopyHeatingWorkingSchedule(section2, section);
					}
					else
					{
						dataRow.Value = saving.Value;
					}
				}
				calculationData = SetBaseLine(list2, calculationData);
				CheckForDifferentFuelSources(calculationData);
				CalculateEnergy(calculationData, section2, zone, calcInput, lightsAndDevicesCalculationData);
				list2 = GetBaseLine(calculationData);
				DataRow dataRow2 = list2.FirstOrDefault((DataRow o) => o.Tag == "ResultNeededEnergy");
				saving.NetEnergy = dataRow2.Value;
				saving.Saving = virtualBaseLineNetEnergy - saving.NetEnergy;
				calculationData = calcData.Clone();
				section2 = EECalcCore.EntityBase<Section>.Deserialize(section.Serialize());
				CheckForDifferentFuelSourcesESM(calculationData);
				List<DataRow> list3 = CreateHeatingVirtualESM(calculationData, section2, calcInput, zone, lightsAndDevicesCalculationData).ToList();
				dataRow = list3.FirstOrDefault((DataRow o) => o.Tag == saving.Tag);
				if (dataRow != null)
				{
					if (saving.Tag.StartsWith("U") || saving.Tag == "g")
					{
						CalculateUsavingTypeESM(saving.Tag, section2, section);
					}
					else if (saving.Tag == "WorkingSchedule")
					{
						CopyHeatingWorkingScheduleESM(section2, section);
					}
					else
					{
						dataRow.Value = saving.OldValue;
					}
				}
				calculationData = SetESM(list3, calculationData);
				CheckForDifferentFuelSourcesESM(calculationData);
				CalculateEnergyESM(calculationData, section2, zone, calcInput, lightsAndDevicesCalculationData);
				list3 = GetESM(calculationData);
				dataRow2 = list3.FirstOrDefault((DataRow o) => o.Tag == "ResultNeededEnergy");
				saving.NetEnergyNMinusOne = dataRow2.Value;
				saving.SavingNMinusOne = saving.NetEnergyNMinusOne - virtualESMNetEnergy;
			}
			double num = list.Sum((SavingsData o) => o.Saving);
			foreach (SavingsData item in list)
			{
				item.Part = item.Saving / num;
			}
			double num2 = virtualBaseLineNetEnergy - virtualESMNetEnergy;
			double num3 = num2 / num;
			foreach (SavingsData item2 in list)
			{
				item2.ActualSaving = num2 * (item2.Saving / num2 * num3 + item2.SavingNMinusOne / num2) / 2.0;
			}
			double num4 = list.Sum((SavingsData o) => o.ActualSaving);
			double num5 = (virtualBaseLineNetEnergy - virtualESMNetEnergy) / num4;
			foreach (SavingsData item3 in list)
			{
				item3.ActualSaving *= num5;
			}
			if (list.Any((SavingsData s) => Convert.ToDouble(s.ActualSaving) > 0.0) && list.Any((SavingsData s) => Convert.ToDouble(s.ActualSaving) < 0.0))
			{
				CheckAndCalculateNegativeSavings(list);
			}
			SetSavingsValues(list);
		}
		AddSavingsToZone(list, zone, "Отопление");
	}

	public static void CheckForDifferentFuelSources(CalculationData tempCalculationdata)
	{
		if (tempCalculationdata.Part1BaseLine < 100.0 && Convert.ToInt32(tempCalculationdata.Part1ESM) == 100)
		{
			tempCalculationdata.Part1ESM = tempCalculationdata.Part1BaseLine;
			tempCalculationdata.Part2ESM = tempCalculationdata.Part2BaseLine;
			tempCalculationdata.TransmitTempEfficiency2ESM = tempCalculationdata.TransmitTempEfficiencyESM;
			tempCalculationdata.SupplyNetEfficiency2ESM = tempCalculationdata.SupplyNetEfficiencyESM;
			tempCalculationdata.Automatic2ESM = tempCalculationdata.AutomaticESM;
			tempCalculationdata.EnergyManagement2ESM = tempCalculationdata.EnergyManagementESM;
			tempCalculationdata.GeneratorHeatEfficiency2ESM = tempCalculationdata.GeneratorHeatEfficiency1ESM;
			tempCalculationdata.GeneratorColdEfficiency2ESM = tempCalculationdata.GeneratorColdEfficiency1ESM;
		}
		if (tempCalculationdata.Part1BaseLine < 100.0 && Convert.ToInt32(tempCalculationdata.Part2ESM) == 100)
		{
			tempCalculationdata.Part1ESM = tempCalculationdata.Part1BaseLine;
			tempCalculationdata.Part2ESM = tempCalculationdata.Part2BaseLine;
			tempCalculationdata.TransmitTempEfficiencyESM = tempCalculationdata.TransmitTempEfficiency2ESM;
			tempCalculationdata.SupplyNetEfficiencyESM = tempCalculationdata.SupplyNetEfficiency2ESM;
			tempCalculationdata.AutomaticESM = tempCalculationdata.Automatic2ESM;
			tempCalculationdata.EnergyManagementESM = tempCalculationdata.EnergyManagement2ESM;
			tempCalculationdata.GeneratorHeatEfficiency1ESM = tempCalculationdata.GeneratorHeatEfficiency2ESM;
			tempCalculationdata.GeneratorColdEfficiency1ESM = tempCalculationdata.GeneratorColdEfficiency2ESM;
		}
		if (tempCalculationdata.Part1ESM < 100.0 && Convert.ToInt32(tempCalculationdata.Part1BaseLine) == 100)
		{
			tempCalculationdata.Part1BaseLine = tempCalculationdata.Part1ESM;
			tempCalculationdata.Part2BaseLine = tempCalculationdata.Part2ESM;
			tempCalculationdata.TransmitTempEfficiency2BaseLine = tempCalculationdata.TransmitTempEfficiencyBaseLine;
			tempCalculationdata.SupplyNetEfficiency2BaseLine = tempCalculationdata.SupplyNetEfficiencyBaseLine;
			tempCalculationdata.Automatic2BaseLine = tempCalculationdata.AutomaticBaseLine;
			tempCalculationdata.EnergyManagement2BaseLine = tempCalculationdata.EnergyManagementBaseLine;
			tempCalculationdata.GeneratorHeatEfficiency2BaseLine = tempCalculationdata.GeneratorHeatEfficiency1BaseLine;
			tempCalculationdata.GeneratorColdEfficiency2BaseLine = tempCalculationdata.GeneratorColdEfficiency1BaseLine;
		}
		if (tempCalculationdata.Part1ESM < 100.0 && Convert.ToInt32(tempCalculationdata.Part2BaseLine) == 100)
		{
			tempCalculationdata.Part1BaseLine = tempCalculationdata.Part1ESM;
			tempCalculationdata.Part2BaseLine = tempCalculationdata.Part2ESM;
			tempCalculationdata.TransmitTempEfficiencyBaseLine = tempCalculationdata.TransmitTempEfficiency2BaseLine;
			tempCalculationdata.SupplyNetEfficiencyBaseLine = tempCalculationdata.SupplyNetEfficiency2BaseLine;
			tempCalculationdata.AutomaticBaseLine = tempCalculationdata.Automatic2BaseLine;
			tempCalculationdata.EnergyManagementBaseLine = tempCalculationdata.EnergyManagement2BaseLine;
			tempCalculationdata.GeneratorHeatEfficiency1BaseLine = tempCalculationdata.GeneratorHeatEfficiency2BaseLine;
			tempCalculationdata.GeneratorColdEfficiency1BaseLine = tempCalculationdata.GeneratorColdEfficiency2BaseLine;
		}
	}

	public static void CheckForDifferentFuelSourcesESM(CalculationData tempCalculationdata)
	{
		if (tempCalculationdata.Part1ESM < 100.0 && Convert.ToInt32(tempCalculationdata.Part1BaseLine) == 100)
		{
			tempCalculationdata.Part1BaseLine = tempCalculationdata.Part1ESM;
			tempCalculationdata.Part2BaseLine = tempCalculationdata.Part2ESM;
			tempCalculationdata.TransmitTempEfficiency2BaseLine = tempCalculationdata.TransmitTempEfficiencyBaseLine;
			tempCalculationdata.SupplyNetEfficiency2BaseLine = tempCalculationdata.SupplyNetEfficiencyBaseLine;
			tempCalculationdata.Automatic2BaseLine = tempCalculationdata.AutomaticBaseLine;
			tempCalculationdata.EnergyManagement2BaseLine = tempCalculationdata.EnergyManagementBaseLine;
			tempCalculationdata.GeneratorHeatEfficiency2BaseLine = tempCalculationdata.GeneratorHeatEfficiency1BaseLine;
			tempCalculationdata.GeneratorColdEfficiency2BaseLine = tempCalculationdata.GeneratorColdEfficiency1BaseLine;
		}
		if (tempCalculationdata.Part1ESM < 100.0 && Convert.ToInt32(tempCalculationdata.Part2BaseLine) == 100)
		{
			tempCalculationdata.Part1BaseLine = tempCalculationdata.Part1ESM;
			tempCalculationdata.Part2BaseLine = tempCalculationdata.Part2ESM;
			tempCalculationdata.TransmitTempEfficiencyBaseLine = tempCalculationdata.TransmitTempEfficiency2BaseLine;
			tempCalculationdata.SupplyNetEfficiencyBaseLine = tempCalculationdata.SupplyNetEfficiency2BaseLine;
			tempCalculationdata.AutomaticBaseLine = tempCalculationdata.Automatic2BaseLine;
			tempCalculationdata.EnergyManagementBaseLine = tempCalculationdata.EnergyManagement2BaseLine;
			tempCalculationdata.GeneratorHeatEfficiency1BaseLine = tempCalculationdata.GeneratorHeatEfficiency2BaseLine;
			tempCalculationdata.GeneratorColdEfficiency1BaseLine = tempCalculationdata.GeneratorColdEfficiency2BaseLine;
		}
		if (tempCalculationdata.Part1BaseLine < 100.0 && Convert.ToInt32(tempCalculationdata.Part1ESM) == 100)
		{
			tempCalculationdata.Part1ESM = tempCalculationdata.Part1BaseLine;
			tempCalculationdata.Part2ESM = tempCalculationdata.Part2BaseLine;
			tempCalculationdata.TransmitTempEfficiency2ESM = tempCalculationdata.TransmitTempEfficiencyESM;
			tempCalculationdata.SupplyNetEfficiency2ESM = tempCalculationdata.SupplyNetEfficiencyESM;
			tempCalculationdata.Automatic2ESM = tempCalculationdata.AutomaticESM;
			tempCalculationdata.EnergyManagement2ESM = tempCalculationdata.EnergyManagementESM;
			tempCalculationdata.GeneratorHeatEfficiency2ESM = tempCalculationdata.GeneratorHeatEfficiency1ESM;
			tempCalculationdata.GeneratorColdEfficiency2ESM = tempCalculationdata.GeneratorColdEfficiency1ESM;
		}
		if (tempCalculationdata.Part1BaseLine < 100.0 && Convert.ToInt32(tempCalculationdata.Part2ESM) == 100)
		{
			tempCalculationdata.Part1ESM = tempCalculationdata.Part1BaseLine;
			tempCalculationdata.Part2ESM = tempCalculationdata.Part2BaseLine;
			tempCalculationdata.TransmitTempEfficiencyESM = tempCalculationdata.TransmitTempEfficiency2ESM;
			tempCalculationdata.SupplyNetEfficiencyESM = tempCalculationdata.SupplyNetEfficiency2ESM;
			tempCalculationdata.AutomaticESM = tempCalculationdata.Automatic2ESM;
			tempCalculationdata.EnergyManagementESM = tempCalculationdata.EnergyManagement2ESM;
			tempCalculationdata.GeneratorHeatEfficiency1ESM = tempCalculationdata.GeneratorHeatEfficiency2ESM;
			tempCalculationdata.GeneratorColdEfficiency1ESM = tempCalculationdata.GeneratorColdEfficiency2ESM;
		}
	}

	public static List<DataRow> CreateHeatingVirtualBaseLine(CalculationData tempCalculationdata, Section section, CalculationInput calcInput, BuildingZone zone, CalculationData lightsAndDevicesCalculationData)
	{
		List<DataRow> baseLine = GetBaseLine(tempCalculationdata);
		DataRow dataRow = baseLine.FirstOrDefault((DataRow o) => o.Tag == "ResulVentilationInputs");
		if (dataRow != null)
		{
			dataRow.Value = tempCalculationdata.ResulVentilationInputsBaseLine;
		}
		DataRow dataRow2 = baseLine.FirstOrDefault((DataRow o) => o.Tag == "ResulLightInputs");
		if (dataRow2 != null)
		{
			dataRow2.Value = tempCalculationdata.ResulLightInputsBaseLine;
		}
		DataRow dataRow3 = baseLine.FirstOrDefault((DataRow o) => o.Tag == "ResulAppliancesInputs");
		if (dataRow3 != null)
		{
			dataRow3.Value = tempCalculationdata.ResulAppliancesInputsBaseLine;
		}
		CalculationData calculationData = SetBaseLine(baseLine, tempCalculationdata);
		CalculateEnergy(calculationData, section, zone, calcInput, lightsAndDevicesCalculationData);
		return GetBaseLine(calculationData);
	}

	public static List<DataRow> CreateHeatingVirtualESM(CalculationData tempCalculationdata, Section section, CalculationInput calcInput, BuildingZone zone, CalculationData lightsAndDevicesCalculationData)
	{
		List<DataRow> eSM = GetESM(tempCalculationdata);
		DataRow dataRow = eSM.FirstOrDefault((DataRow o) => o.Tag == "ResulVentilationInputs");
		if (dataRow != null)
		{
			dataRow.Value = tempCalculationdata.ResulVentilationInputsESM;
		}
		DataRow dataRow2 = eSM.FirstOrDefault((DataRow o) => o.Tag == "ResulLightInputs");
		if (dataRow2 != null)
		{
			dataRow2.Value = tempCalculationdata.ResulLightInputsESM;
		}
		DataRow dataRow3 = eSM.FirstOrDefault((DataRow o) => o.Tag == "ResulAppliancesInputs");
		if (dataRow3 != null)
		{
			dataRow3.Value = tempCalculationdata.ResulAppliancesInputsESM;
		}
		CalculationData calculationData = SetESM(eSM, tempCalculationdata);
		CalculateEnergy(calculationData, section, zone, calcInput, lightsAndDevicesCalculationData);
		return GetESM(calculationData);
	}

	public static void CalculateCoolingSavings(this CalculationData calcData, Section section, CalculationInput calcInput, BuildingZone zone, CalculationData lightsAndDevicesCalculationData, CalculationData ventCool)
	{
		if (buildingZone == null)
		{
			return;
		}
		if (!buildingZone.HasCooling)
		{
			AddSavingsToZone(new List<SavingsData>(), zone, "Охлаждане");
			return;
		}
		publicCalculationData = calcData;
		CalculationData calculationData = calcData.Clone();
		List<MonthlyDays> monthslist = section.CalcPeriod((int)section.CoolingSeason.FirstMonthCool, (int)section.CoolingSeason.LastMonthCool, section.CoolingSeason.FirstDayCool, section.CoolingSeason.LastDayCool);
		IList<SavingsData> list = CheckForSavings("Охлаждане");
		CheckForDifferentFuelSources(calculationData);
		CommonExtensions.AddRange<SavingsData>((ICollection<SavingsData>)list, (IEnumerable<SavingsData>)CheckForFuelSavings("Охлаждане", calculationData));
		SetSavingsValues(list);
		if (list.Any())
		{
			List<DataRow> source = CreateCoolingVirtualBaseLine(calculationData, section, calcInput, lightsAndDevicesCalculationData, monthslist, ventCool);
			List<DataRow> source2 = CreateCoolingVirtualESM(calculationData, section, calcInput, lightsAndDevicesCalculationData, monthslist, ventCool);
			virtualBaseLineNetEnergy = source.FirstOrDefault((DataRow o) => o.Tag == "ResultNeededEnergy").Value;
			virtualESMNetEnergy = source2.FirstOrDefault((DataRow o) => o.Tag == "ResultNeededEnergy").Value;
			foreach (SavingsData saving in list)
			{
				calculationData = calcData.Clone();
				Section section2 = EECalcCore.EntityBase<Section>.Deserialize(section.Serialize());
				CheckForDifferentFuelSources(calculationData);
				List<DataRow> list2 = CreateCoolingVirtualBaseLine(calculationData, section2, calcInput, lightsAndDevicesCalculationData, monthslist, ventCool).ToList();
				DataRow dataRow = list2.FirstOrDefault((DataRow o) => o.Tag == saving.Tag);
				if (saving.Tag.StartsWith("U") || saving.Tag == "g")
				{
					CalculateUsavingType(saving.Tag, section2, section);
				}
				else if (saving.Tag == "WorkingSchedule")
				{
					CopyCoolingWorkingSchedule(section2, section);
				}
				else
				{
					dataRow.Value = saving.Value;
				}
				calculationData = SetBaseLine(list2, calculationData);
				CheckForDifferentFuelSources(calculationData);
				CalculateCoolingEnergyBaseLine(monthslist, calculationData, section2, calcInput, lightsAndDevicesCalculationData, ventCool);
				calculationData.CalculateNeededEnergyCoolingBaseLine();
				list2 = GetBaseLine(calculationData);
				DataRow dataRow2 = list2.FirstOrDefault((DataRow o) => o.Tag == "ResultNeededEnergy");
				saving.NetEnergy = dataRow2.Value;
				saving.Saving = virtualBaseLineNetEnergy - saving.NetEnergy;
				calculationData = calcData.Clone();
				section2 = EECalcCore.EntityBase<Section>.Deserialize(section.Serialize());
				CheckForDifferentFuelSourcesESM(calculationData);
				List<DataRow> list3 = CreateCoolingVirtualESM(calculationData, section2, calcInput, lightsAndDevicesCalculationData, monthslist, ventCool).ToList();
				dataRow = list3.FirstOrDefault((DataRow o) => o.Tag == saving.Tag);
				if (saving.Tag.StartsWith("U") || saving.Tag == "g")
				{
					CalculateUsavingTypeESM(saving.Tag, section2, section);
				}
				else if (saving.Tag == "WorkingSchedule")
				{
					CopyCoolingWorkingScheduleESM(section2, section);
				}
				else
				{
					dataRow.Value = saving.OldValue;
				}
				calculationData = SetESM(list3, calculationData);
				CheckForDifferentFuelSourcesESM(calculationData);
				CalculateCoolingEnergyESM(monthslist, calculationData, section2, calcInput, lightsAndDevicesCalculationData, ventCool);
				calculationData.CalculateNeededEnergyCoolingESM();
				list3 = GetESM(calculationData);
				dataRow2 = list3.FirstOrDefault((DataRow o) => o.Tag == "ResultNeededEnergy");
				saving.NetEnergyNMinusOne = dataRow2.Value;
				saving.SavingNMinusOne = saving.NetEnergyNMinusOne - virtualESMNetEnergy;
			}
			double num = list.Sum((SavingsData o) => o.Saving);
			foreach (SavingsData item in list)
			{
				item.Part = item.Saving / num;
			}
			double num2 = virtualBaseLineNetEnergy - virtualESMNetEnergy;
			double num3 = num2 / num;
			foreach (SavingsData item2 in list)
			{
				item2.ActualSaving = num2 * (item2.Saving / num2 * num3 + item2.SavingNMinusOne / num2) / 2.0;
			}
			double num4 = list.Sum((SavingsData o) => o.ActualSaving);
			double num5 = (virtualBaseLineNetEnergy - virtualESMNetEnergy) / num4;
			foreach (SavingsData item3 in list)
			{
				item3.ActualSaving *= num5;
			}
			if (list.Any((SavingsData s) => Convert.ToDouble(s.ActualSaving) > 0.0) && list.Any((SavingsData s) => Convert.ToDouble(s.ActualSaving) < 0.0))
			{
				CheckAndCalculateNegativeSavings(list);
			}
			double num6 = virtualBaseLineNetEnergy - virtualESMNetEnergy - list.Sum((SavingsData o) => o.ActualSaving);
			SetSavingsValues(list);
		}
		AddSavingsToZone(list, zone, "Охлаждане");
	}

	public static List<DataRow> CreateCoolingVirtualBaseLine(CalculationData tempCalculationdata, Section section, CalculationInput calcInput, CalculationData lightsAndDevicesCalculationData, List<MonthlyDays> monthslist, CalculationData ventCool)
	{
		List<DataRow> baseLine = GetBaseLine(tempCalculationdata);
		DataRow dataRow = baseLine.FirstOrDefault((DataRow o) => o.Tag == "ResulVentilationInputs");
		if (dataRow != null)
		{
			dataRow.Value = tempCalculationdata.ResulVentilationInputsBaseLine;
		}
		DataRow dataRow2 = baseLine.FirstOrDefault((DataRow o) => o.Tag == "ResulCoolingInputs");
		if (dataRow2 != null)
		{
			dataRow2.Value = tempCalculationdata.ResulCoolingInputsBaseLine;
		}
		CalculationData calculationData = SetBaseLine(baseLine, tempCalculationdata);
		CalculateCoolingEnergyBaseLine(monthslist, calculationData, section, calcInput, lightsAndDevicesCalculationData, ventCool);
		return GetBaseLine(calculationData);
	}

	public static List<DataRow> CreateCoolingVirtualESM(CalculationData tempCalculationdata, Section section, CalculationInput calcInput, CalculationData lightsAndDevicesCalculationData, List<MonthlyDays> monthslist, CalculationData ventCool)
	{
		List<DataRow> eSM = GetESM(tempCalculationdata);
		DataRow dataRow = eSM.FirstOrDefault((DataRow o) => o.Tag == "ResulVentilationInputs");
		if (dataRow != null)
		{
			dataRow.Value = tempCalculationdata.ResulVentilationInputsESM;
		}
		DataRow dataRow2 = eSM.FirstOrDefault((DataRow o) => o.Tag == "ResulCoolingInputs");
		if (dataRow2 != null)
		{
			dataRow2.Value = tempCalculationdata.ResulCoolingInputsESM;
		}
		CalculationData calculationData = SetESM(eSM, tempCalculationdata);
		CalculateCoolingEnergyESM(monthslist, calculationData, section, calcInput, lightsAndDevicesCalculationData, ventCool);
		return GetESM(calculationData);
	}

	public static void CalculateVentilationHeatingSavings(this CalculationData calcData, Section section, CalculationInput calcInput, BuildingZone zone, HeatingCalculations heatCalculations)
	{
		if (zone == null)
		{
			return;
		}
		if (!zone.HasHeating)
		{
			AddSavingsToZone(new List<SavingsData>(), zone, "Вентилация - Отопление");
			return;
		}
		publicCalculationData = calcData;
		CalculationData calculationData = calcData.Clone();
		IList<SavingsData> list = CheckForVentilationSavings("Вентилация - Отопление");
		CheckForDifferentFuelSources(calculationData);
		CommonExtensions.AddRange<SavingsData>((ICollection<SavingsData>)list, (IEnumerable<SavingsData>)CheckForFuelSavings("Вентилация - Отопление", calculationData));
		SetVentilationSavingsValues(list);
		if (list.Any())
		{
			List<DataRow> ventilationBaseLine = GetVentilationBaseLine(calcData.Clone());
			virtualBaseLineNetEnergy = ventilationBaseLine.FirstOrDefault((DataRow o) => o.Tag == "ResultNeededEnergy").Value;
			foreach (SavingsData saving in list)
			{
				calculationData = calcData.Clone();
				Section section2 = EECalcCore.EntityBase<Section>.Deserialize(section.Serialize());
				CheckForDifferentFuelSources(calculationData);
				List<DataRow> list2 = GetVentilationBaseLine(calculationData).ToList();
				DataRow dataRow = list2.FirstOrDefault((DataRow o) => o.Tag == saving.Tag);
				if (dataRow != null)
				{
					if (saving.Tag == "WorkingSchedule")
					{
						CopyVentilationHeatingWorkingSchedule(section2, section);
					}
					else
					{
						dataRow.Value = saving.Value;
					}
				}
				calculationData = SetVentilationBaseLine(list2, calculationData);
				CheckForDifferentFuelSources(calculationData);
				calculationData.VentilationHeatEnergyBaseLine(section2, calcInput, heatCalculations);
				calculationData.CalculateVentNeededEnergyBaseLine();
				calculationData.CalculateGeneratorHeatEfficiencyBaseLine();
				list2 = GetVentilationBaseLine(calculationData);
				DataRow dataRow2 = list2.FirstOrDefault((DataRow o) => o.Tag == "ResultNeededEnergy");
				saving.NetEnergy = dataRow2.Value;
				saving.Saving = virtualBaseLineNetEnergy - saving.NetEnergy;
			}
			double num = list.Sum((SavingsData o) => o.Saving);
			foreach (SavingsData item in list)
			{
				item.Part = item.Saving / num;
			}
			calculationData = calcData.Clone();
			Section section3 = EECalcCore.EntityBase<Section>.Deserialize(section.Serialize());
			CheckForDifferentFuelSources(calculationData);
			List<DataRow> list3 = GetVentilationBaseLine(calculationData).ToList();
			foreach (SavingsData saving2 in list)
			{
				DataRow dataRow3 = list3.FirstOrDefault((DataRow o) => o.Tag == saving2.Tag);
				if (dataRow3 != null)
				{
					if (saving2.Tag == "WorkingSchedule")
					{
						CopyVentilationHeatingWorkingSchedule(section3, section);
					}
					else
					{
						dataRow3.Value = saving2.Value;
					}
				}
			}
			calculationData = SetVentilationBaseLine(list3, calculationData);
			CheckForDifferentFuelSources(calculationData);
			calculationData.VentilationHeatEnergyBaseLine(section3, calcInput, heatCalculations);
			calculationData.CalculateVentNeededEnergyBaseLine();
			calculationData.CalculateGeneratorHeatEfficiencyBaseLine();
			list3 = GetVentilationBaseLine(calculationData);
			DataRow dataRow4 = list3.FirstOrDefault((DataRow o) => o.Tag == "ResultNeededEnergy");
			double resultNeededEnergyBaseLine = calcData.ResultNeededEnergyBaseLine;
			if (dataRow4 != null)
			{
				double num2 = resultNeededEnergyBaseLine - dataRow4.Value;
				foreach (SavingsData item2 in list)
				{
					item2.ActualSaving = num2 * item2.Part;
				}
			}
			if (list.Any((SavingsData s) => Convert.ToDouble(s.ActualSaving) > 0.0) && list.Any((SavingsData s) => Convert.ToDouble(s.ActualSaving) < 0.0))
			{
				CheckAndCalculateNegativeSavings(list);
			}
			SetVentilationSavingsValues(list);
		}
		AddSavingsToZone(list, zone, "Вентилация - Отопление");
	}

	public static void CalculateVentilationCoolingSavings(this CalculationData calcData, Section section, CalculationInput calcInput, BuildingZone zone, CoolingCalculations coolCalculations)
	{
		if (zone == null)
		{
			return;
		}
		if (!zone.HasCooling)
		{
			AddSavingsToZone(new List<SavingsData>(), zone, "Вентилация - Охлаждане");
			return;
		}
		publicCalculationData = calcData;
		CalculationData calculationData = calcData.Clone();
		IList<SavingsData> list = CheckForVentilationSavings("Вентилация - Охлаждане");
		CheckForDifferentFuelSources(calculationData);
		CommonExtensions.AddRange<SavingsData>((ICollection<SavingsData>)list, (IEnumerable<SavingsData>)CheckForFuelSavings("Вентилация - Охлаждане", calculationData));
		SetVentilationSavingsValues(list);
		if (list.Any())
		{
			List<DataRow> ventilationBaseLine = GetVentilationBaseLine(calcData.Clone());
			virtualBaseLineNetEnergy = ventilationBaseLine.FirstOrDefault((DataRow o) => o.Tag == "ResultNeededEnergy").Value;
			foreach (SavingsData saving in list)
			{
				calculationData = calcData.Clone();
				Section section2 = EECalcCore.EntityBase<Section>.Deserialize(section.Serialize());
				CheckForDifferentFuelSources(calculationData);
				List<DataRow> list2 = GetVentilationBaseLine(calculationData).ToList();
				DataRow dataRow = list2.FirstOrDefault((DataRow o) => o.Tag == saving.Tag);
				if (dataRow != null)
				{
					if (saving.Tag == "WorkingSchedule")
					{
						CopyVentilationCoolingWorkingSchedule(section2, section);
					}
					else
					{
						dataRow.Value = saving.Value;
					}
				}
				calculationData = SetVentilationBaseLine(list2, calculationData);
				CheckForDifferentFuelSources(calculationData);
				calculationData.VentilationCoolEnergyBaseLine(section2, calcInput, coolCalculations);
				calculationData.CalculateVentCoolNeededEnergyBaseLine();
				calculationData.CalculateGeneratorVentilationCoolEfficiencyBaseLine();
				list2 = GetVentilationBaseLine(calculationData);
				DataRow dataRow2 = list2.FirstOrDefault((DataRow o) => o.Tag == "ResultNeededEnergy");
				saving.NetEnergy = dataRow2.Value;
				saving.Saving = virtualBaseLineNetEnergy - saving.NetEnergy;
			}
			double num = list.Sum((SavingsData o) => o.Saving);
			foreach (SavingsData item in list)
			{
				item.Part = item.Saving / num;
			}
			calculationData = calcData.Clone();
			Section section3 = EECalcCore.EntityBase<Section>.Deserialize(section.Serialize());
			CheckForDifferentFuelSources(calculationData);
			List<DataRow> list3 = GetVentilationBaseLine(calculationData).ToList();
			foreach (SavingsData saving2 in list)
			{
				DataRow dataRow3 = list3.FirstOrDefault((DataRow o) => o.Tag == saving2.Tag);
				if (dataRow3 != null)
				{
					if (saving2.Tag == "WorkingSchedule")
					{
						CopyVentilationCoolingWorkingSchedule(section3, section);
					}
					else
					{
						dataRow3.Value = saving2.Value;
					}
				}
			}
			calculationData = SetVentilationBaseLine(list3, calculationData);
			CheckForDifferentFuelSources(calculationData);
			calculationData.VentilationCoolEnergyBaseLine(section3, calcInput, coolCalculations);
			calculationData.CalculateVentCoolNeededEnergyBaseLine();
			calculationData.CalculateGeneratorVentilationCoolEfficiencyBaseLine();
			list3 = GetVentilationBaseLine(calculationData);
			DataRow dataRow4 = list3.FirstOrDefault((DataRow o) => o.Tag == "ResultNeededEnergy");
			double resultNeededEnergyBaseLine = calcData.ResultNeededEnergyBaseLine;
			if (dataRow4 != null)
			{
				double num2 = resultNeededEnergyBaseLine - dataRow4.Value;
				foreach (SavingsData item2 in list)
				{
					item2.ActualSaving = num2 * item2.Part;
				}
			}
			if (list.Any((SavingsData s) => Convert.ToDouble(s.ActualSaving) > 0.0) && list.Any((SavingsData s) => Convert.ToDouble(s.ActualSaving) < 0.0))
			{
				CheckAndCalculateNegativeSavings(list);
			}
			SetVentilationSavingsValues(list);
		}
		AddSavingsToZone(list, zone, "Вентилация - Охлаждане");
	}

	public static void CalculateFansAndPumpsHeatingSavings(this HeatingCalculations calc, Section section, BuildingZone zone)
	{
		fansAndPumnps = calc.FansAndPumps;
		IList<SavingsData> list = CheckHeatingForFansAndPumpsSavings("Помпи и вентилатори - Отопление");
		SetHeatingFansAndPumpsSavingsValues(list);
		if (list.Any())
		{
			List<MonthlyDays> source = section.CalcPeriod((int)section.HeatingSeason.FirstMonthHeat, (int)section.HeatingSeason.LastMonthHeat, section.HeatingSeason.FirstDayHeat, section.HeatingSeason.LastDayHeat);
			double num = source.Sum((MonthlyDays month) => month.Weeks);
			foreach (SavingsData item in list)
			{
				switch (item.Tag)
				{
				case "VentilatorsHeat":
				{
					double num8 = calc.FansAndPumps.VentilatorsHeatBaseLine * GetWeekHeatingVentilationHoursBaseLine(section) * num / (calc.FansAndPumps.EnergyManagementBaseLine / 100.0) / 1000.0;
					double num9 = calc.FansAndPumps.VentilatorsHeatESM * GetWeekHeatingVentilationHoursEsm(section) * num / (calc.FansAndPumps.EnergyManagementESM / 100.0) / 1000.0;
					item.ActualSaving = num8 - num9;
					break;
				}
				case "PumpVentilation":
				{
					double num6 = calc.FansAndPumps.PumpVentilationBaseLine * GetWeekHeatingVentilationHoursBaseLine(section) * num / (calc.FansAndPumps.EnergyManagementBaseLine / 100.0) / 1000.0;
					double num7 = calc.FansAndPumps.PumpVentilationESM * GetWeekHeatingVentilationHoursEsm(section) * num / (calc.FansAndPumps.EnergyManagementESM / 100.0) / 1000.0;
					item.ActualSaving = num6 - num7;
					break;
				}
				case "PumpHeating":
				{
					double num4 = calc.FansAndPumps.PumpHeatingBaseLine * 24.0 * 7.0 * num / (calc.FansAndPumps.EnergyManagementBaseLine / 100.0) / 1000.0;
					double num5 = calc.FansAndPumps.PumpHeatingESM * 24.0 * 7.0 * num / (calc.FansAndPumps.EnergyManagementESM / 100.0) / 1000.0;
					item.ActualSaving = num4 - num5;
					break;
				}
				case "EnergyManagement":
				{
					double num2 = calc.FansAndPumps.PumpHeatingBaseLine * GetWeekHeatingSeasonHoursBaseLine(section) * num / (calc.FansAndPumps.EnergyManagementBaseLine / 100.0) / 1000.0;
					double num3 = calc.FansAndPumps.PumpHeatingESM * GetWeekHeatingSeasonHoursEsm(section) * num / (calc.FansAndPumps.EnergyManagementESM / 100.0) / 1000.0;
					item.ActualSaving = 0.0;
					break;
				}
				}
			}
			if (list.Any((SavingsData s) => Convert.ToDouble(s.ActualSaving) > 0.0) && list.Any((SavingsData s) => Convert.ToDouble(s.ActualSaving) < 0.0))
			{
				CheckAndCalculateNegativeSavings(list);
			}
			SetHeatingFansAndPumpsSavingsValues(list);
		}
		AddSavingsToZone(list, zone, "Помпи и вентилатори - Отопление");
	}

	public static void SetHeatingFansAndPumpsSavingsValues(IList<SavingsData> savings)
	{
		fansAndPumnps.VentilatorsHeatSavings = GetSaving(savings, "VentilatorsHeat");
		fansAndPumnps.PumpVentilationSavings = GetSaving(savings, "PumpVentilation");
		fansAndPumnps.PumpHeatingSavings = GetSaving(savings, "PumpHeating");
		fansAndPumnps.EnergyManagementSavings = GetSaving(savings, "EnergyManagement");
	}

	public static IList<SavingsData> CheckHeatingForFansAndPumpsSavings(string technology)
	{
		List<SavingsData> list = new List<SavingsData>();
		if (!object.Equals(fansAndPumnps.VentilatorsHeatBaseLine / fansAndPumnps.EnergyManagementBaseLine, fansAndPumnps.VentilatorsHeatESM / fansAndPumnps.EnergyManagementESM))
		{
			SavingsData item = new SavingsData
			{
				Row = "Вентилатори",
				Technology = technology,
				Value = fansAndPumnps.VentilatorsHeatESM,
				Tag = "VentilatorsHeat"
			};
			list.Add(item);
		}
		if (!object.Equals(fansAndPumnps.PumpVentilationBaseLine / fansAndPumnps.EnergyManagementBaseLine, fansAndPumnps.PumpVentilationESM / fansAndPumnps.EnergyManagementESM))
		{
			SavingsData item2 = new SavingsData
			{
				Row = "Помпи вентилация",
				Technology = technology,
				Value = fansAndPumnps.PumpVentilationESM,
				Tag = "PumpVentilation"
			};
			list.Add(item2);
		}
		if (!object.Equals(fansAndPumnps.PumpHeatingBaseLine / fansAndPumnps.EnergyManagementBaseLine, fansAndPumnps.PumpHeatingESM / fansAndPumnps.EnergyManagementESM))
		{
			SavingsData item3 = new SavingsData
			{
				Row = "Помпи отопление",
				Technology = technology,
				Value = fansAndPumnps.PumpHeatingESM,
				Tag = "PumpHeating"
			};
			list.Add(item3);
		}
		if (!object.Equals(fansAndPumnps.EnergyManagementBaseLine, fansAndPumnps.EnergyManagementESM))
		{
			SavingsData item4 = new SavingsData
			{
				Row = "Помпи енергиен мениджмънт",
				Technology = technology,
				Value = fansAndPumnps.EnergyManagementESM,
				Tag = "EnergyManagement"
			};
			list.Add(item4);
		}
		return list;
	}

	public static void CalculateFansAndPumpsCoolingSavings(this HeatingCalculations calc, Section section, BuildingZone zone)
	{
		fansAndPumnps = calc.FansAndPumps;
		IList<SavingsData> list = CheckCoolingForFansAndPumpsSavings("Помпи и вентилатори - Охлаждане");
		SetCoolingFansAndPumpsSavingsValues(list);
		if (list.Any())
		{
			List<MonthlyDays> source = section.CalcPeriod((int)section.CoolingSeason.FirstMonthCool, (int)section.CoolingSeason.LastMonthCool, section.CoolingSeason.FirstDayCool, section.CoolingSeason.LastDayCool);
			double num = source.Sum((MonthlyDays month) => month.Weeks);
			foreach (SavingsData item in list)
			{
				switch (item.Tag)
				{
				case "VentilatorsCool":
				{
					double num12 = calc.FansAndPumps.VentilatorsCoolBaseLine * GetWeekCoolingVentilationHoursBaseLine(section) * num / (calc.FansAndPumps.EnergyManagement2BaseLine / 100.0) / 1000.0;
					double num13 = calc.FansAndPumps.VentilatorsCoolESM * GetWeekCoolingVentilationHoursEsm(section) * num / (calc.FansAndPumps.EnergyManagement2ESM / 100.0) / 1000.0;
					item.ActualSaving = num12 - num13;
					break;
				}
				case "PumpVentilationCool":
				{
					double num10 = calc.FansAndPumps.PumpVentilationCoolBaseLine * GetWeekCoolingVentilationHoursBaseLine(section) * num / (calc.FansAndPumps.EnergyManagement2BaseLine / 100.0) / 1000.0;
					double num11 = calc.FansAndPumps.PumpVentilationCoolESM * GetWeekCoolingVentilationHoursEsm(section) * num / (calc.FansAndPumps.EnergyManagement2ESM / 100.0) / 1000.0;
					item.ActualSaving = num10 - num11;
					break;
				}
				case "VentilatorsOutdoorAirCool":
				{
					double num8 = calc.FansAndPumps.VentilatorsOutdoorAirCoolBaseLine * GetWeekCoolingVentilationHoursBaseLine(section) * num / (calc.FansAndPumps.EnergyManagement2BaseLine / 100.0) / 1000.0;
					double num9 = calc.FansAndPumps.VentilatorsOutdoorAirCoolESM * GetWeekCoolingVentilationHoursEsm(section) * num / (calc.FansAndPumps.EnergyManagement2ESM / 100.0) / 1000.0;
					item.ActualSaving = num8 - num9;
					break;
				}
				case "CoolingPump":
				{
					double num6 = calc.FansAndPumps.CoolingPumpBaseLine * 24.0 * 7.0 * num / (calc.FansAndPumps.EnergyManagement2BaseLine / 100.0) / 1000.0;
					double num7 = calc.FansAndPumps.CoolingPumpESM * 24.0 * 7.0 * num / (calc.FansAndPumps.EnergyManagement2ESM / 100.0) / 1000.0;
					item.ActualSaving = num6 - num7;
					break;
				}
				case "OtherCoolingVentilation":
				{
					double num4 = calc.FansAndPumps.OtherCoolingVentilationBaseLine * GetWeekCoolingSeasonHoursBaseLine(section) * num / 1000.0;
					double num5 = calc.FansAndPumps.OtherCoolingVentilationESM * GetWeekCoolingSeasonHoursEsm(section) * num / 1000.0;
					item.ActualSaving = num4 - num5;
					break;
				}
				case "OtherCooling":
				{
					double num2 = calc.FansAndPumps.OtherCoolingBaseLine * GetWeekCoolingSeasonHoursBaseLine(section) * num / 1000.0;
					double num3 = calc.FansAndPumps.OtherCoolingESM * GetWeekCoolingSeasonHoursEsm(section) * num / 1000.0;
					item.ActualSaving = num2 - num3;
					break;
				}
				}
			}
			SetCoolingFansAndPumpsSavingsValues(list);
		}
		AddSavingsToZone(list, zone, "Помпи и вентилатори - Охлаждане");
	}

	public static void SetCoolingFansAndPumpsSavingsValues(IList<SavingsData> savings)
	{
		fansAndPumnps.VentilatorsCoolSavings = GetSaving(savings, "VentilatorsCool");
		fansAndPumnps.VentilatorsOutdoorAirCoolSavings = GetSaving(savings, "VentilatorsOutdoorAirCool");
		fansAndPumnps.PumpVentilationCoolSavings = GetSaving(savings, "PumpVentilationCool");
		fansAndPumnps.CoolingPumpSavings = GetSaving(savings, "CoolingPump");
		fansAndPumnps.OtherCoolingVentilationSavings = GetSaving(savings, "OtherCoolingVentilation");
		fansAndPumnps.OtherCoolingSavings = GetSaving(savings, "OtherCooling");
	}

	public static IList<SavingsData> CheckCoolingForFansAndPumpsSavings(string technology)
	{
		List<SavingsData> list = new List<SavingsData>();
		if (!object.Equals(fansAndPumnps.VentilatorsCoolBaseLine / fansAndPumnps.EnergyManagement2BaseLine, fansAndPumnps.VentilatorsCoolESM / fansAndPumnps.EnergyManagement2ESM))
		{
			SavingsData item = new SavingsData
			{
				Row = "Вентилатори(вентилация)",
				Technology = technology,
				Value = fansAndPumnps.VentilatorsCoolESM,
				Tag = "VentilatorsCool"
			};
			list.Add(item);
		}
		if (!object.Equals(fansAndPumnps.VentilatorsOutdoorAirCoolBaseLine / fansAndPumnps.EnergyManagement2BaseLine, fansAndPumnps.VentilatorsOutdoorAirCoolESM / fansAndPumnps.EnergyManagement2ESM))
		{
			SavingsData item2 = new SavingsData
			{
				Row = "Вентилатори(вент. с външен въздух без терм. обработка)",
				Technology = technology,
				Value = fansAndPumnps.VentilatorsOutdoorAirCoolESM,
				Tag = "VentilatorsOutdoorAirCool"
			};
			list.Add(item2);
		}
		if (!object.Equals(fansAndPumnps.PumpVentilationCoolBaseLine / fansAndPumnps.EnergyManagement2BaseLine, fansAndPumnps.PumpVentilationCoolESM / fansAndPumnps.EnergyManagement2ESM))
		{
			SavingsData item3 = new SavingsData
			{
				Row = "Помпи вентилация",
				Technology = technology,
				Value = fansAndPumnps.PumpVentilationCoolESM,
				Tag = "PumpVentilationCool"
			};
			list.Add(item3);
		}
		if (!object.Equals(fansAndPumnps.CoolingPumpBaseLine / fansAndPumnps.EnergyManagement2BaseLine, fansAndPumnps.CoolingPumpESM / fansAndPumnps.EnergyManagement2ESM))
		{
			SavingsData item4 = new SavingsData
			{
				Row = "Помпи охлаждане",
				Technology = technology,
				Value = fansAndPumnps.CoolingPumpESM,
				Tag = "CoolingPump"
			};
			list.Add(item4);
		}
		if (!object.Equals(fansAndPumnps.OtherCoolingVentilationBaseLine, fansAndPumnps.OtherCoolingVentilationESM))
		{
			SavingsData item5 = new SavingsData
			{
				Row = "Други (вентилация)",
				Technology = technology,
				Value = fansAndPumnps.OtherCoolingVentilationESM,
				Tag = "OtherCoolingVentilation"
			};
			list.Add(item5);
		}
		if (!object.Equals(fansAndPumnps.OtherCoolingBaseLine, fansAndPumnps.OtherCoolingESM))
		{
			SavingsData item6 = new SavingsData
			{
				Row = "Други (охлаждане)",
				Technology = technology,
				Value = fansAndPumnps.OtherCoolingESM,
				Tag = "OtherCooling"
			};
			list.Add(item6);
		}
		return list;
	}

	public static void CalculateLightsSavings(this CalculationData calcData, Section section, BuildingZone zone)
	{
		publicCalculationData = calcData;
		string technology = $"Осветление";
		LightsAndDevicesPeriods heating = publicCalculationData.Lights.Heating;
		List<MonthlyDays> source = section.CalcPeriod((int)section.HeatingSeason.FirstMonthHeat, (int)section.HeatingSeason.LastMonthHeat, section.HeatingSeason.FirstDayHeat, section.HeatingSeason.LastDayHeat);
		double totalWeeks = source.Sum((MonthlyDays month) => month.Weeks);
		CalculatePeriod(heating, totalWeeks, technology, zone);
		source = section.CalcPeriod((int)section.CoolingSeason.FirstMonthCool, (int)section.CoolingSeason.LastMonthCool, section.CoolingSeason.FirstDayCool, section.CoolingSeason.LastDayCool);
		totalWeeks = source.Sum((MonthlyDays month) => month.Weeks);
		heating = publicCalculationData.Lights.Cooling;
		CalculatePeriod(heating, totalWeeks, technology, zone);
		source = section.CalcPeriod(0, 11, 1, 31);
		totalWeeks = source.Sum((MonthlyDays month) => month.Weeks);
		heating = publicCalculationData.Lights.General;
		CalculatePeriod(heating, totalWeeks, technology, zone);
		calcData.Lights.Heating.DevicesNeededEnergySavings = (calcData.Lights.Heating.DevicesNeededEnergyBaseLine - calcData.Lights.Heating.DevicesNeededEnergyESM).ToString("F3");
		calcData.Lights.Cooling.DevicesNeededEnergySavings = (calcData.Lights.Cooling.DevicesNeededEnergyBaseLine - calcData.Lights.Cooling.DevicesNeededEnergyESM).ToString("F3");
		calcData.Lights.General.DevicesNeededEnergySavings = (calcData.Lights.General.DevicesNeededEnergyBaseLine - calcData.Lights.General.DevicesNeededEnergyESM).ToString("F3");
	}

	public static void CalculateBalancedDevicesSavings(this CalculationData calcData, Section section, BuildingZone zone)
	{
		publicCalculationData = calcData;
		string technology = $"Уреди влияещи на баланса";
		LightsAndDevicesPeriods heating = publicCalculationData.BalancedDevices.Heating;
		List<MonthlyDays> source = section.CalcPeriod((int)section.HeatingSeason.FirstMonthHeat, (int)section.HeatingSeason.LastMonthHeat, section.HeatingSeason.FirstDayHeat, section.HeatingSeason.LastDayHeat);
		double totalWeeks = source.Sum((MonthlyDays month) => month.Weeks);
		CalculatePeriod(heating, totalWeeks, technology, zone);
		source = section.CalcPeriod((int)section.CoolingSeason.FirstMonthCool, (int)section.CoolingSeason.LastMonthCool, section.CoolingSeason.FirstDayCool, section.CoolingSeason.LastDayCool);
		totalWeeks = source.Sum((MonthlyDays month) => month.Weeks);
		heating = publicCalculationData.BalancedDevices.Cooling;
		CalculatePeriod(heating, totalWeeks, technology, zone);
		source = section.CalcPeriod(0, 11, 1, 31);
		totalWeeks = source.Sum((MonthlyDays month) => month.Weeks);
		heating = publicCalculationData.BalancedDevices.General;
		CalculatePeriod(heating, totalWeeks, technology, zone);
		publicCalculationData.BalancedDevices.Heating.DevicesNeededEnergySavings = (publicCalculationData.BalancedDevices.Heating.DevicesNeededEnergyBaseLine - publicCalculationData.BalancedDevices.Heating.DevicesNeededEnergyESM).ToString("F3");
		publicCalculationData.BalancedDevices.Cooling.DevicesNeededEnergySavings = (publicCalculationData.BalancedDevices.Cooling.DevicesNeededEnergyBaseLine - publicCalculationData.BalancedDevices.Cooling.DevicesNeededEnergyESM).ToString("F3");
		publicCalculationData.BalancedDevices.General.DevicesNeededEnergySavings = (publicCalculationData.BalancedDevices.General.DevicesNeededEnergyBaseLine - publicCalculationData.BalancedDevices.General.DevicesNeededEnergyESM).ToString("F3");
	}

	public static void CalculateNonBalancedDevicesSavings(this CalculationData calcData, Section section, BuildingZone zone)
	{
		publicCalculationData = calcData;
		string technology = $"Уреди не влияещи на баланса";
		LightsAndDevicesPeriods heating = publicCalculationData.NonBalancedDevices.Heating;
		List<MonthlyDays> source = section.CalcPeriod((int)section.HeatingSeason.FirstMonthHeat, (int)section.HeatingSeason.LastMonthHeat, section.HeatingSeason.FirstDayHeat, section.HeatingSeason.LastDayHeat);
		double totalWeeks = source.Sum((MonthlyDays month) => month.Weeks);
		CalculatePeriod(heating, totalWeeks, technology, zone);
		source = section.CalcPeriod((int)section.CoolingSeason.FirstMonthCool, (int)section.CoolingSeason.LastMonthCool, section.CoolingSeason.FirstDayCool, section.CoolingSeason.LastDayCool);
		totalWeeks = source.Sum((MonthlyDays month) => month.Weeks);
		heating = publicCalculationData.NonBalancedDevices.Cooling;
		CalculatePeriod(heating, totalWeeks, technology, zone);
		source = section.CalcPeriod(0, 11, 1, 31);
		totalWeeks = source.Sum((MonthlyDays month) => month.Weeks);
		heating = publicCalculationData.NonBalancedDevices.General;
		CalculatePeriod(heating, totalWeeks, technology, zone);
		publicCalculationData.NonBalancedDevices.Heating.DevicesNeededEnergySavings = (publicCalculationData.NonBalancedDevices.Heating.DevicesNeededEnergyBaseLine - publicCalculationData.NonBalancedDevices.Heating.DevicesNeededEnergyESM).ToString("F3");
		publicCalculationData.NonBalancedDevices.Cooling.DevicesNeededEnergySavings = (publicCalculationData.NonBalancedDevices.Cooling.DevicesNeededEnergyBaseLine - publicCalculationData.NonBalancedDevices.Cooling.DevicesNeededEnergyESM).ToString("F3");
		publicCalculationData.NonBalancedDevices.General.DevicesNeededEnergySavings = (publicCalculationData.NonBalancedDevices.General.DevicesNeededEnergyBaseLine - publicCalculationData.NonBalancedDevices.General.DevicesNeededEnergyESM).ToString("F3");
	}

	public static void CalculateHotWaterPumpsSavings(this CalculationData calcData, Section section, BuildingZone zone)
	{
		publicCalculationData = calcData;
		string technology = $"Помпи (БГВ)";
		LightsAndDevicesPeriods heating = publicCalculationData.HotWaterPumps.Heating;
		List<MonthlyDays> source = section.CalcPeriod((int)section.HeatingSeason.FirstMonthHeat, (int)section.HeatingSeason.LastMonthHeat, section.HeatingSeason.FirstDayHeat, section.HeatingSeason.LastDayHeat);
		double totalWeeks = source.Sum((MonthlyDays month) => month.Weeks);
		CalculatePeriod(heating, totalWeeks, technology, zone);
		source = section.CalcPeriod((int)section.CoolingSeason.FirstMonthCool, (int)section.CoolingSeason.LastMonthCool, section.CoolingSeason.FirstDayCool, section.CoolingSeason.LastDayCool);
		totalWeeks = source.Sum((MonthlyDays month) => month.Weeks);
		heating = publicCalculationData.HotWaterPumps.Cooling;
		CalculatePeriod(heating, totalWeeks, technology, zone);
		source = section.CalcPeriod(0, 11, 1, 31);
		totalWeeks = source.Sum((MonthlyDays month) => month.Weeks);
		heating = publicCalculationData.HotWaterPumps.General;
		CalculatePeriod(heating, totalWeeks, technology, zone);
	}

	public static void CalculatePeriod(LightsAndDevicesPeriods period, double totalWeeks, string technology, BuildingZone zone)
	{
		IList<SavingsData> list = CheckLightsAndDevicesSavings(period, technology);
		SetLightsAndDevicesSavingsvalues(period, list);
		if (list.Any())
		{
			foreach (SavingsData item in list)
			{
				string tag = item.Tag;
				if (!(tag == "WorkSchedule"))
				{
					if (tag == "Power")
					{
						double num = period.WorkScheduleBaseLine * period.PowerBaseLine * totalWeeks / 1000.0;
						double num2 = period.WorkScheduleBaseLine * period.PowerESM * totalWeeks / 1000.0;
						item.Saving = num - num2;
					}
				}
				else
				{
					double num3 = period.WorkScheduleBaseLine * period.PowerBaseLine * totalWeeks / 1000.0;
					double num4 = period.WorkScheduleESM * period.PowerBaseLine * totalWeeks / 1000.0;
					item.Saving = num3 - num4;
				}
			}
			double num5 = list.Sum((SavingsData o) => o.Saving);
			foreach (SavingsData item2 in list)
			{
				item2.Part = item2.Saving / num5;
			}
			double num6 = period.WorkScheduleBaseLine * period.PowerBaseLine * totalWeeks / 1000.0 - period.WorkScheduleESM * period.PowerESM * totalWeeks / 1000.0;
			foreach (SavingsData item3 in list)
			{
				item3.ActualSaving = num6 * item3.Part;
			}
			if (list.Any((SavingsData s) => Convert.ToDouble(s.ActualSaving) > 0.0) && list.Any((SavingsData s) => Convert.ToDouble(s.ActualSaving) < 0.0))
			{
				CheckAndCalculateNegativeSavings(list);
			}
			SetLightsAndDevicesSavingsvalues(period, list);
		}
		switch (technology)
		{
		case "Осветление":
			if (period == publicCalculationData.Lights.General)
			{
				AddSavingsToZone(list, zone, technology);
			}
			break;
		case "Уреди влияещи на баланса":
			if (period == publicCalculationData.BalancedDevices.General)
			{
				AddSavingsToZone(list, zone, technology);
			}
			break;
		case "Уреди не влияещи на баланса":
			if (period == publicCalculationData.NonBalancedDevices.General)
			{
				AddSavingsToZone(list, zone, technology);
			}
			break;
		case "Помпи (БГВ)":
			if (period == publicCalculationData.HotWaterPumps.General)
			{
				AddSavingsToZone(list, zone, technology);
			}
			break;
		}
	}

	public static void SetLightsAndDevicesSavingsvalues(LightsAndDevicesPeriods period, IList<SavingsData> savings)
	{
		period.WorkScheduleSavings = GetSaving(savings, "WorkSchedule");
		period.PowerSavings = GetSaving(savings, "Power");
	}

	public static IList<SavingsData> CheckLightsAndDevicesSavings(LightsAndDevicesPeriods period, string technology)
	{
		List<SavingsData> list = new List<SavingsData>();
		if (!object.Equals(period.WorkScheduleBaseLine, period.WorkScheduleESM))
		{
			SavingsData item = new SavingsData
			{
				Row = "Работен режим",
				Technology = technology,
				Value = period.WorkScheduleESM,
				Tag = "WorkSchedule"
			};
			list.Add(item);
		}
		if (!object.Equals(period.PowerBaseLine, period.PowerESM))
		{
			SavingsData item2 = new SavingsData
			{
				Row = "Едновременна мощност",
				Technology = technology,
				Value = period.PowerESM,
				Tag = "Power"
			};
			list.Add(item2);
		}
		return list;
	}

	public static void CalculateHotWaterSavings(this CalculationData calcData, Section section, CalculationInput calcInput)
	{
		publicCalculationData = calcData;
		CalculationData calculationData = calcData.Clone();
		IList<SavingsData> list = CheckForHotWaterSavings("БГВ");
		CheckForDifferentFuelSources(calculationData);
		CommonExtensions.AddRange<SavingsData>((ICollection<SavingsData>)list, (IEnumerable<SavingsData>)CheckForFuelSavings("БГВ", calculationData));
		SetHotWaterSavingsValues(list);
		if (list.Any())
		{
			List<DataRow> hotWaterBaseLine = GetHotWaterBaseLine(calcData.Clone());
			virtualBaseLineNetEnergy = hotWaterBaseLine.FirstOrDefault((DataRow o) => o.Tag == "ResultNeededEnergy").Value;
			foreach (SavingsData saving in list)
			{
				calculationData = calcData.Clone();
				CheckForDifferentFuelSources(calculationData);
				List<DataRow> list2 = GetHotWaterBaseLine(calculationData).ToList();
				DataRow dataRow = list2.FirstOrDefault((DataRow o) => o.Tag == saving.Tag);
				if (dataRow != null)
				{
					dataRow.Value = saving.Value;
				}
				calculationData = SetHotWaterBaseLine(list2, calculationData);
				CheckForDifferentFuelSources(calculationData);
				calculationData.HotWaterCalculationBaseLine(section, calcInput);
				calculationData.CalculateGeneratorHotWaterEfficiencyBaseLine();
				calculationData.CalculateHotWaterNeededEnergyBaseLine();
				list2 = GetHotWaterBaseLine(calculationData);
				DataRow dataRow2 = list2.FirstOrDefault((DataRow o) => o.Tag == "ResultNeededEnergy");
				saving.NetEnergy = dataRow2.Value;
				saving.Saving = virtualBaseLineNetEnergy - saving.NetEnergy;
			}
			double num = list.Sum((SavingsData o) => o.Saving);
			foreach (SavingsData item2 in list)
			{
				item2.Part = item2.Saving / num;
			}
			calculationData = calcData.Clone();
			CheckForDifferentFuelSources(calculationData);
			List<DataRow> list3 = GetHotWaterBaseLine(calculationData).ToList();
			foreach (SavingsData saving2 in list)
			{
				DataRow dataRow3 = list3.FirstOrDefault((DataRow o) => o.Tag == saving2.Tag);
				if (dataRow3 != null)
				{
					dataRow3.Value = saving2.Value;
				}
			}
			calculationData = SetHotWaterBaseLine(list3, calculationData);
			CheckForDifferentFuelSources(calculationData);
			calculationData.HotWaterCalculationBaseLine(section, calcInput);
			calculationData.CalculateGeneratorHotWaterEfficiencyBaseLine();
			calculationData.CalculateHotWaterNeededEnergyBaseLine();
			list3 = GetHotWaterBaseLine(calculationData);
			DataRow dataRow4 = list3.FirstOrDefault((DataRow o) => o.Tag == "ResultNeededEnergy");
			double resultNeededEnergyBaseLine = calcData.ResultNeededEnergyBaseLine;
			if (dataRow4 != null)
			{
				double num2 = resultNeededEnergyBaseLine - dataRow4.Value;
				foreach (SavingsData item3 in list)
				{
					item3.ActualSaving = num2 * item3.Part;
				}
			}
			if (list.Any((SavingsData s) => Convert.ToDouble(s.ActualSaving) > 0.0) && list.Any((SavingsData s) => Convert.ToDouble(s.ActualSaving) < 0.0))
			{
				CheckAndCalculateNegativeSavings(list);
			}
		}
		if (!object.Equals(publicCalculationData.SunEnergyCalculations.BaseLine.SunEnergyResTable.BGVPumpsTotal, publicCalculationData.SunEnergyCalculations.ESM.SunEnergyResTable.BGVPumpsTotal))
		{
			double totalHeatedArea = calcInput.General.BuildingResults.TotalAreaElements.TotalHeatedArea;
			SavingsData item = new SavingsData
			{
				Row = "Енергия от помпи за БГВ (слънце)",
				Technology = "БГВ",
				ActualSaving = publicCalculationData.SunEnergyCalculations.BaseLine.SunEnergyResTable.BGVPumpsTotal / totalHeatedArea - publicCalculationData.SunEnergyCalculations.ESM.SunEnergyResTable.BGVPumpsTotal / totalHeatedArea,
				Tag = "SunEnergy"
			};
			list.Add(item);
		}
		if (list.Any())
		{
			SetHotWaterSavingsValues(list);
		}
		AddSavingsToBuilding(list, calcInput, "БГВ");
	}

	private static IList<SavingsData> CheckForHotWaterSavings(string technology)
	{
		List<SavingsData> list = new List<SavingsData>();
		if (!object.Equals(publicCalculationData.ConsumptionBaseLine, publicCalculationData.ConsumptionESM))
		{
			SavingsData item = new SavingsData
			{
				Row = "Годишно потребление",
				Technology = technology,
				Value = publicCalculationData.ConsumptionESM,
				Tag = "Consumption"
			};
			list.Add(item);
		}
		if (!object.Equals(publicCalculationData.TempDifferenceBaseLine, publicCalculationData.TempDifferenceESM))
		{
			SavingsData item2 = new SavingsData
			{
				Row = "Температурна разлика",
				Technology = technology,
				Value = publicCalculationData.TempDifferenceESM,
				Tag = "TempDifference"
			};
			list.Add(item2);
		}
		if (!object.Equals(publicCalculationData.SunEnergyBaseLine, publicCalculationData.SunEnergyESM))
		{
			SavingsData item3 = new SavingsData
			{
				Row = "Енергия от допълнителни източници",
				Technology = technology,
				Value = publicCalculationData.SunEnergyESM,
				Tag = "SunEnergy"
			};
			list.Add(item3);
		}
		return list;
	}

	private static void SetHotWaterSavingsValues(IList<SavingsData> savings)
	{
		publicCalculationData.ConsumptionSavings = GetSaving(savings, "Consumption");
		publicCalculationData.TempDifferenceSavings = GetSaving(savings, "TempDifference");
		publicCalculationData.SunEnergySavings = GetSaving(savings, "SunEnergy");
		publicCalculationData.Part1Savings = GetSaving(savings, "Part1");
		publicCalculationData.TransmitTempEfficiencySavings = GetSaving(savings, "TransmitTempEfficiency");
		publicCalculationData.SupplyNetEfficiencySavings = GetSaving(savings, "SupplyNetEfficiency");
		publicCalculationData.AutomaticSavings = GetSaving(savings, "Automatic");
		publicCalculationData.EnergyManagementSavings = GetSaving(savings, "EnergyManagement");
		publicCalculationData.GeneratorHeatEfficiency1Savings = GetSaving(savings, "GeneratorHeatEfficiency1");
		publicCalculationData.Part2Savings = GetSaving(savings, "Part2");
		publicCalculationData.TransmitTempEfficiency2Savings = GetSaving(savings, "TransmitTempEfficiency2");
		publicCalculationData.SupplyNetEfficiency2Savings = GetSaving(savings, "SupplyNetEfficiency2");
		publicCalculationData.Automatic2Savings = GetSaving(savings, "Automatic2");
		publicCalculationData.EnergyManagement2Savings = GetSaving(savings, "EnergyManagement2");
		publicCalculationData.GeneratorHeatEfficiency2Savings = GetSaving(savings, "GeneratorHeatEfficiency2");
	}

	private static void AddSavingsToZone(IList<SavingsData> savings, BuildingZone zone, string technology)
	{
		if (zone.ZoneSavings.Any())
		{
			List<ZoneSaving> source = zone.ZoneSavings.ToList();
			foreach (ZoneSaving item2 in source.Where((ZoneSaving zoneSaving) => zoneSaving.Technology == technology))
			{
				zone.ZoneSavings.Remove(item2);
			}
		}
		if (!savings.Any())
		{
			return;
		}
		foreach (SavingsData saving in savings)
		{
			if (saving.ActualSaving != 0.0 && !double.IsNaN(saving.ActualSaving) && !double.IsInfinity(saving.ActualSaving))
			{
				ZoneSaving item = new ZoneSaving
				{
					Technology = saving.Technology,
					Row = saving.Row,
					Tag = saving.Tag,
					Value = saving.Value,
					ActualSaving = saving.ActualSaving.ToString()
				};
				zone.ZoneSavings.Add(item);
			}
		}
	}

	private static void AddSavingsToBuilding(IList<SavingsData> savings, CalculationInput calcInput, string technology)
	{
		if (savings.Any())
		{
			if (calcInput.General.BuildingSavings.Any())
			{
				List<ZoneSaving> source = calcInput.General.BuildingSavings.ToList();
				foreach (ZoneSaving item2 in source.Where((ZoneSaving zoneSaving) => zoneSaving.Technology == technology))
				{
					calcInput.General.BuildingSavings.Remove(item2);
				}
			}
			{
				foreach (SavingsData saving in savings)
				{
					if (saving.ActualSaving != 0.0 && !double.IsNaN(saving.ActualSaving) && !double.IsInfinity(saving.ActualSaving))
					{
						ZoneSaving item = new ZoneSaving
						{
							Technology = saving.Technology,
							Row = saving.Row,
							Tag = saving.Tag,
							Value = saving.Value,
							ActualSaving = saving.ActualSaving.ToString()
						};
						calcInput.General.BuildingSavings.Add(item);
					}
				}
				return;
			}
		}
		if (!calcInput.General.BuildingSavings.Any())
		{
			return;
		}
		List<ZoneSaving> list = calcInput.General.BuildingSavings.Where((ZoneSaving o) => o.Technology == technology).ToList();
		if (!list.Any())
		{
			return;
		}
		foreach (ZoneSaving item3 in list)
		{
			calcInput.General.BuildingSavings.Remove(item3);
		}
	}

	private static void SetSavingsValues(IList<SavingsData> savings)
	{
		publicCalculationData.WorkingScheduleSavings = GetSaving(savings, "WorkingSchedule");
		publicCalculationData.UouterWallsSavings = GetSaving(savings, "UouterWalls");
		publicCalculationData.UwindowsSavings = GetSaving(savings, "Uwindows");
		publicCalculationData.UnontransparentSavings = GetSaving(savings, "Unontransparent");
		publicCalculationData.UfloorSavings = GetSaving(savings, "Ufloor");
		publicCalculationData.gSavings = GetSaving(savings, "g");
		publicCalculationData.UinnerWallsSavings = GetSaving(savings, "UinnerWalls");
		publicCalculationData.UceilingSavings = GetSaving(savings, "Uceiling");
		publicCalculationData.UfloorOtherSavings = GetSaving(savings, "UfloorOther");
		publicCalculationData.InfiltracionSavings = GetSaving(savings, "Infiltracion");
		publicCalculationData.ProjectTemperatureSavings = GetSaving(savings, "ProjectTemperature");
		publicCalculationData.NonProjectTemperatureSavings = GetSaving(savings, "NonProjectTemperature");
		publicCalculationData.ProjectHumiditySavings = GetSaving(savings, "ProjectHumidity");
		publicCalculationData.DebitSavings = GetSaving(savings, "Debit");
		publicCalculationData.Part1Savings = GetSaving(savings, "Part1");
		publicCalculationData.TransmitTempEfficiencySavings = GetSaving(savings, "TransmitTempEfficiency");
		publicCalculationData.SupplyNetEfficiencySavings = GetSaving(savings, "SupplyNetEfficiency");
		publicCalculationData.AutomaticSavings = GetSaving(savings, "Automatic");
		publicCalculationData.EnergyManagementSavings = GetSaving(savings, "EnergyManagement");
		publicCalculationData.GeneratorHeatEfficiency1Savings = GetSaving(savings, "GeneratorHeatEfficiency1");
		publicCalculationData.GeneratorColdEfficiency1Savings = GetSaving(savings, "GeneratorColdEfficiency1");
		publicCalculationData.Part2Savings = GetSaving(savings, "Part2");
		publicCalculationData.TransmitTempEfficiency2Savings = GetSaving(savings, "TransmitTempEfficiency2");
		publicCalculationData.SupplyNetEfficiency2Savings = GetSaving(savings, "SupplyNetEfficiency2");
		publicCalculationData.Automatic2Savings = GetSaving(savings, "Automatic2");
		publicCalculationData.EnergyManagement2Savings = GetSaving(savings, "EnergyManagement2");
		publicCalculationData.GeneratorHeatEfficiency2Savings = GetSaving(savings, "GeneratorHeatEfficiency2");
		publicCalculationData.GeneratorColdEfficiency2Savings = GetSaving(savings, "GeneratorColdEfficiency2");
	}

	private static IList<SavingsData> CheckForSavings(string technology)
	{
		List<SavingsData> list = new List<SavingsData>();
		if (!object.Equals(publicCalculationData.WorkingScheduleBaseLine, publicCalculationData.WorkingScheduleESM))
		{
			SavingsData item = new SavingsData
			{
				Row = "Работен режим",
				Technology = technology,
				OldValue = publicCalculationData.WorkingScheduleBaseLine,
				Value = publicCalculationData.WorkingScheduleESM,
				Tag = "WorkingSchedule"
			};
			list.Add(item);
		}
		if (!object.Equals(publicCalculationData.UouterWallsBaseLine, publicCalculationData.UouterWallsESM) || !object.Equals(publicCalculationData.gBaseLine, publicCalculationData.gESM))
		{
			SavingsData item2 = new SavingsData
			{
				Row = "U външни стени",
				Technology = technology,
				OldValue = publicCalculationData.UouterWallsBaseLine,
				Value = publicCalculationData.UouterWallsESM,
				Tag = "UouterWalls"
			};
			list.Add(item2);
		}
		if (!object.Equals(publicCalculationData.UwindowsBaseLine, publicCalculationData.UwindowsESM))
		{
			SavingsData item3 = new SavingsData
			{
				Row = "U прозорци",
				Technology = technology,
				OldValue = publicCalculationData.UouterWallsBaseLine,
				Value = publicCalculationData.UwindowsESM,
				Tag = "Uwindows"
			};
			list.Add(item3);
		}
		if (!object.Equals(publicCalculationData.UnontransparentBaseLine, publicCalculationData.UnontransparentESM))
		{
			SavingsData item4 = new SavingsData
			{
				Row = "U покрив непрозрачен",
				Technology = technology,
				OldValue = publicCalculationData.UnontransparentBaseLine,
				Value = publicCalculationData.UnontransparentESM,
				Tag = "Unontransparent"
			};
			list.Add(item4);
		}
		if (!object.Equals(publicCalculationData.UfloorBaseLine, publicCalculationData.UfloorESM))
		{
			SavingsData item5 = new SavingsData
			{
				Row = "U под(НПЕ/ОПЕ/външен въздух/земя)",
				Technology = technology,
				OldValue = publicCalculationData.UfloorBaseLine,
				Value = publicCalculationData.UfloorESM,
				Tag = "Ufloor"
			};
			list.Add(item5);
		}
		if (!object.Equals(publicCalculationData.UinnerWallsBaseLine, publicCalculationData.UinnerWallsESM))
		{
			SavingsData item6 = new SavingsData
			{
				Row = "U вътрешни стени",
				Technology = technology,
				OldValue = publicCalculationData.UinnerWallsBaseLine,
				Value = publicCalculationData.UinnerWallsESM,
				Tag = "UinnerWalls"
			};
			list.Add(item6);
		}
		if (!object.Equals(publicCalculationData.UceilingBaseLine, publicCalculationData.UceilingESM))
		{
			SavingsData item7 = new SavingsData
			{
				Row = "U тавани към съседна зона",
				Technology = technology,
				OldValue = publicCalculationData.UceilingBaseLine,
				Value = publicCalculationData.UceilingESM,
				Tag = "Uceiling"
			};
			list.Add(item7);
		}
		if (!object.Equals(publicCalculationData.UfloorOtherBaseLine, publicCalculationData.UfloorOtherESM))
		{
			SavingsData item8 = new SavingsData
			{
				Row = "U под(над друга зона)",
				Technology = technology,
				OldValue = publicCalculationData.UfloorOtherBaseLine,
				Value = publicCalculationData.UfloorOtherESM,
				Tag = "UfloorOther"
			};
			list.Add(item8);
		}
		if (!object.Equals(publicCalculationData.InfiltracionBaseLine, publicCalculationData.InfiltracionESM))
		{
			SavingsData item9 = new SavingsData
			{
				Row = "Инфилтрация",
				Technology = technology,
				OldValue = publicCalculationData.InfiltracionBaseLine,
				Value = publicCalculationData.InfiltracionESM,
				Tag = "Infiltracion"
			};
			list.Add(item9);
		}
		if (!object.Equals(publicCalculationData.ProjectTemperatureBaseLine, publicCalculationData.ProjectTemperatureESM))
		{
			SavingsData item10 = new SavingsData
			{
				Row = "Проектна температура",
				Technology = technology,
				OldValue = publicCalculationData.ProjectTemperatureBaseLine,
				Value = publicCalculationData.ProjectTemperatureESM,
				Tag = "ProjectTemperature"
			};
			list.Add(item10);
		}
		if (!object.Equals(publicCalculationData.NonProjectTemperatureBaseLine, publicCalculationData.NonProjectTemperatureESM))
		{
			SavingsData item11 = new SavingsData
			{
				Row = ((technology == "Охлаждане") ? "Температура с повишение" : "Температура с понижение"),
				Technology = technology,
				OldValue = publicCalculationData.NonProjectTemperatureBaseLine,
				Value = publicCalculationData.NonProjectTemperatureESM,
				Tag = "NonProjectTemperature"
			};
			list.Add(item11);
		}
		if (!object.Equals(publicCalculationData.ProjectHumidityBaseLine, publicCalculationData.ProjectHumidityESM))
		{
			SavingsData item12 = new SavingsData
			{
				Row = "Относителна влажност",
				Technology = technology,
				OldValue = publicCalculationData.ProjectHumidityBaseLine,
				Value = publicCalculationData.ProjectHumidityESM,
				Tag = "ProjectHumidity"
			};
			list.Add(item12);
		}
		if (!object.Equals(publicCalculationData.DebitBaseLine, publicCalculationData.DebitESM))
		{
			SavingsData item13 = new SavingsData
			{
				Row = "Дебит за охлаждане с необработен вън. в-х",
				Technology = technology,
				OldValue = publicCalculationData.DebitBaseLine,
				Value = publicCalculationData.DebitESM,
				Tag = "Debit"
			};
			list.Add(item13);
		}
		return list;
	}

	private static IList<SavingsData> CheckForFuelSavings(string technology, CalculationData tempCalcData)
	{
		List<SavingsData> list = new List<SavingsData>();
		if (!object.Equals(tempCalcData.TransmitTempEfficiencyBaseLine, tempCalcData.TransmitTempEfficiencyESM))
		{
			SavingsData item = new SavingsData
			{
				Row = "Ефективност на отдаване ЕИ1",
				Technology = technology,
				OldValue = tempCalcData.TransmitTempEfficiencyBaseLine,
				Value = tempCalcData.TransmitTempEfficiencyESM,
				Tag = "TransmitTempEfficiency"
			};
			list.Add(item);
		}
		if (!object.Equals(tempCalcData.SupplyNetEfficiencyBaseLine, tempCalcData.SupplyNetEfficiencyESM))
		{
			SavingsData item2 = new SavingsData
			{
				Row = "Ефективност на разпределителната мрежа ЕИ1",
				Technology = technology,
				OldValue = tempCalcData.SupplyNetEfficiencyBaseLine,
				Value = tempCalcData.SupplyNetEfficiencyESM,
				Tag = "SupplyNetEfficiency"
			};
			list.Add(item2);
		}
		if (!object.Equals(tempCalcData.AutomaticBaseLine, tempCalcData.AutomaticESM))
		{
			SavingsData item3 = new SavingsData
			{
				Row = "Автоматично управление ЕИ1",
				Technology = technology,
				OldValue = tempCalcData.AutomaticBaseLine,
				Value = tempCalcData.AutomaticESM,
				Tag = "Automatic"
			};
			list.Add(item3);
		}
		if (!object.Equals(tempCalcData.EnergyManagementBaseLine, tempCalcData.EnergyManagementESM))
		{
			SavingsData item4 = new SavingsData
			{
				Row = "Енергиен мениджмънт(EМ) и поддръжка ЕИ1",
				Technology = technology,
				OldValue = tempCalcData.EnergyManagementBaseLine,
				Value = tempCalcData.EnergyManagementESM,
				Tag = "EnergyManagement"
			};
			list.Add(item4);
		}
		if (!object.Equals(tempCalcData.GeneratorHeatEfficiency1BaseLine, tempCalcData.GeneratorHeatEfficiency1ESM))
		{
			SavingsData item5 = new SavingsData
			{
				Row = "Ефективност на генератора на топлина ЕИ1",
				Technology = technology,
				OldValue = tempCalcData.GeneratorHeatEfficiency1BaseLine,
				Value = tempCalcData.GeneratorHeatEfficiency1ESM,
				Tag = "GeneratorHeatEfficiency1"
			};
			list.Add(item5);
		}
		if (!object.Equals(tempCalcData.GeneratorColdEfficiency1BaseLine, tempCalcData.GeneratorColdEfficiency1ESM))
		{
			SavingsData item6 = new SavingsData
			{
				Row = "Ефективност на генератора на студ ЕИ1",
				Technology = technology,
				OldValue = tempCalcData.GeneratorColdEfficiency1BaseLine,
				Value = tempCalcData.GeneratorColdEfficiency1ESM,
				Tag = "GeneratorColdEfficiency1"
			};
			list.Add(item6);
		}
		if (!object.Equals(tempCalcData.TransmitTempEfficiency2BaseLine, tempCalcData.TransmitTempEfficiency2ESM))
		{
			SavingsData item7 = new SavingsData
			{
				Row = "Ефективност на отдаване ЕИ2",
				Technology = technology,
				OldValue = tempCalcData.TransmitTempEfficiency2BaseLine,
				Value = tempCalcData.TransmitTempEfficiency2ESM,
				Tag = "TransmitTempEfficiency2"
			};
			list.Add(item7);
		}
		if (!object.Equals(tempCalcData.SupplyNetEfficiency2BaseLine, tempCalcData.SupplyNetEfficiency2ESM))
		{
			SavingsData item8 = new SavingsData
			{
				Row = "Ефективност на разпределителната мрежа ЕИ2",
				Technology = technology,
				OldValue = tempCalcData.SupplyNetEfficiency2BaseLine,
				Value = tempCalcData.SupplyNetEfficiency2ESM,
				Tag = "SupplyNetEfficiency2"
			};
			list.Add(item8);
		}
		if (!object.Equals(tempCalcData.Automatic2BaseLine, tempCalcData.Automatic2ESM))
		{
			SavingsData item9 = new SavingsData
			{
				Row = "Автоматично управление ЕИ2",
				Technology = technology,
				OldValue = tempCalcData.Automatic2BaseLine,
				Value = tempCalcData.Automatic2ESM,
				Tag = "Automatic2"
			};
			list.Add(item9);
		}
		if (!object.Equals(tempCalcData.EnergyManagement2BaseLine, tempCalcData.EnergyManagement2ESM))
		{
			SavingsData item10 = new SavingsData
			{
				Row = "Енергиен мениджмънт(EМ) и поддръжка ЕИ2",
				Technology = technology,
				OldValue = tempCalcData.EnergyManagement2BaseLine,
				Value = tempCalcData.EnergyManagement2ESM,
				Tag = "EnergyManagement2"
			};
			list.Add(item10);
		}
		if (!object.Equals(tempCalcData.GeneratorHeatEfficiency2BaseLine, tempCalcData.GeneratorHeatEfficiency2ESM))
		{
			SavingsData item11 = new SavingsData
			{
				Row = "Ефективност на генератора на топлина ЕИ2",
				Technology = technology,
				OldValue = tempCalcData.GeneratorHeatEfficiency2BaseLine,
				Value = tempCalcData.GeneratorHeatEfficiency2ESM,
				Tag = "GeneratorHeatEfficiency2"
			};
			list.Add(item11);
		}
		if (!object.Equals(tempCalcData.GeneratorColdEfficiency2BaseLine, tempCalcData.GeneratorColdEfficiency2ESM))
		{
			SavingsData item12 = new SavingsData
			{
				Row = "Ефективност на генератора на студ ЕИ2",
				Technology = technology,
				OldValue = tempCalcData.GeneratorColdEfficiency2BaseLine,
				Value = tempCalcData.GeneratorColdEfficiency2ESM,
				Tag = "GeneratorColdEfficiency2"
			};
			list.Add(item12);
		}
		return list;
	}

	private static void SetVentilationSavingsValues(IList<SavingsData> savings)
	{
		publicCalculationData.WorkingScheduleSavings = GetSaving(savings, "WorkingSchedule");
		publicCalculationData.DebitSavings = GetSaving(savings, "Debit");
		publicCalculationData.FlowTemperatureSavings = GetSaving(savings, "FlowTemperature");
		publicCalculationData.RelativeHumiditySavings = GetSaving(savings, "RelativeHumidity");
		publicCalculationData.ProjectHumiditySavings = GetSaving(savings, "ProjectHumidity");
		publicCalculationData.FirstRecEfficiencySavings = GetSaving(savings, "FirstRecEfficiency");
		publicCalculationData.SecondRecEfficiencySavings = GetSaving(savings, "SecondRecEfficiency");
		publicCalculationData.HeatingAirDifferenceSavings = GetSaving(savings, "HeatingAirDifference");
		publicCalculationData.MinimumEndTemperatureSavings = GetSaving(savings, "MinimumEndTemperature");
		publicCalculationData.Part1Savings = GetSaving(savings, "Part1");
		publicCalculationData.TransmitTempEfficiencySavings = GetSaving(savings, "TransmitTempEfficiency");
		publicCalculationData.SupplyNetEfficiencySavings = GetSaving(savings, "SupplyNetEfficiency");
		publicCalculationData.AutomaticSavings = GetSaving(savings, "Automatic");
		publicCalculationData.EnergyManagementSavings = GetSaving(savings, "EnergyManagement");
		publicCalculationData.GeneratorHeatEfficiency1Savings = GetSaving(savings, "GeneratorHeatEfficiency1");
		publicCalculationData.GeneratorColdEfficiency1Savings = GetSaving(savings, "GeneratorColdEfficiency1");
		publicCalculationData.Part2Savings = GetSaving(savings, "Part2");
		publicCalculationData.TransmitTempEfficiency2Savings = GetSaving(savings, "TransmitTempEfficiency2");
		publicCalculationData.SupplyNetEfficiency2Savings = GetSaving(savings, "SupplyNetEfficiency2");
		publicCalculationData.Automatic2Savings = GetSaving(savings, "Automatic2");
		publicCalculationData.EnergyManagement2Savings = GetSaving(savings, "EnergyManagement2");
		publicCalculationData.GeneratorHeatEfficiency2Savings = GetSaving(savings, "GeneratorHeatEfficiency2");
		publicCalculationData.GeneratorColdEfficiency2Savings = GetSaving(savings, "GeneratorColdEfficiency2");
	}

	private static IList<SavingsData> CheckForVentilationSavings(string technology)
	{
		List<SavingsData> list = new List<SavingsData>();
		if (!object.Equals(publicCalculationData.WorkingScheduleBaseLine, publicCalculationData.WorkingScheduleESM))
		{
			SavingsData item = new SavingsData
			{
				Row = "Работен режим",
				Technology = technology,
				Value = publicCalculationData.WorkingScheduleESM,
				Tag = "WorkingSchedule"
			};
			list.Add(item);
		}
		if (!object.Equals(publicCalculationData.DebitBaseLine, publicCalculationData.DebitESM))
		{
			SavingsData item2 = new SavingsData
			{
				Row = "Дебит",
				Technology = technology,
				Value = publicCalculationData.DebitESM,
				Tag = "Debit"
			};
			list.Add(item2);
		}
		if (!object.Equals(publicCalculationData.FlowTemperatureBaseLine, publicCalculationData.FlowTemperatureESM))
		{
			SavingsData item3 = new SavingsData
			{
				Row = "Температура на подаване",
				Technology = technology,
				Value = publicCalculationData.FlowTemperatureESM,
				Tag = "FlowTemperature"
			};
			list.Add(item3);
		}
		if (!object.Equals(publicCalculationData.ProjectHumidityBaseLine, publicCalculationData.ProjectHumidityESM))
		{
			SavingsData item4 = new SavingsData
			{
				Row = "Отностителна влажност на подавания въздух",
				Technology = technology,
				Value = publicCalculationData.ProjectHumidityESM,
				Tag = "ProjectHumidity"
			};
			list.Add(item4);
		}
		if (!object.Equals(publicCalculationData.RelativeHumidityBaseLine, publicCalculationData.RelativeHumidityESM))
		{
			SavingsData item5 = new SavingsData
			{
				Row = "Относителна влажност на подавания въздух",
				Technology = technology,
				Value = publicCalculationData.RelativeHumidityESM,
				Tag = "RelativeHumidity"
			};
			list.Add(item5);
		}
		if (!object.Equals(publicCalculationData.FirstRecEfficiencyBaseLine, publicCalculationData.FirstRecEfficiencyESM))
		{
			SavingsData item6 = new SavingsData
			{
				Row = "Ефективност на първа степен на рекуперация",
				Technology = technology,
				Value = publicCalculationData.FirstRecEfficiencyESM,
				Tag = "FirstRecEfficiency"
			};
			list.Add(item6);
		}
		if (!object.Equals(publicCalculationData.SecondRecEfficiencyBaseLine, publicCalculationData.SecondRecEfficiencyESM))
		{
			SavingsData item7 = new SavingsData
			{
				Row = "Ефективност на втора степен на рекуперация",
				Technology = technology,
				Value = publicCalculationData.SecondRecEfficiencyESM,
				Tag = "SecondRecEfficiency"
			};
			list.Add(item7);
		}
		if (!object.Equals(publicCalculationData.HeatingAirDifferenceBaseLine, publicCalculationData.HeatingAirDifferenceESM))
		{
			SavingsData item8 = new SavingsData
			{
				Row = "Темп. разлика на загряване на въздуха във втора степен",
				Technology = technology,
				Value = publicCalculationData.HeatingAirDifferenceESM,
				Tag = "HeatingAirDifference"
			};
			list.Add(item8);
		}
		if (!object.Equals(publicCalculationData.MinimumEndTemperatureBaseLine, publicCalculationData.MinimumEndTemperatureESM))
		{
			SavingsData item9 = new SavingsData
			{
				Row = "Минимална крайна температура на отработения въздух",
				Technology = technology,
				Value = publicCalculationData.MinimumEndTemperatureESM,
				Tag = "MinimumEndTemperature"
			};
			list.Add(item9);
		}
		return list;
	}

	private static void CheckAndCalculateNegativeSavings(IList<SavingsData> savings)
	{
		double num = savings.Where((SavingsData s) => Convert.ToDouble(s.ActualSaving) > 0.0).Sum((SavingsData s) => s.ActualSaving);
		foreach (SavingsData item in savings.Where((SavingsData s) => Convert.ToDouble(s.ActualSaving) > 0.0))
		{
			item.Part = item.ActualSaving / num;
		}
		List<SavingsData> source = savings.Where((SavingsData s) => Convert.ToDouble(s.ActualSaving) < 0.0).ToList();
		double num2 = publicCalculationData.ResultNeededEnergyBaseLine - publicCalculationData.ResultNeededEnergyESM + source.Sum((SavingsData ns) => Math.Abs(ns.ActualSaving));
		foreach (SavingsData item2 in savings.Where((SavingsData s) => Convert.ToDouble(s.ActualSaving) > 0.0))
		{
			item2.ActualSaving = num2 * item2.Part;
		}
	}

	private static double GetValue(IEnumerable<DataRow> baseLine, string tag)
	{
		double result = 0.0;
		DataRow dataRow = baseLine.FirstOrDefault((DataRow o) => o.Tag == tag);
		if (dataRow != null)
		{
			result = dataRow.Value;
		}
		return result;
	}

	private static string GetSaving(IEnumerable<SavingsData> savings, string tag)
	{
		SavingsData savingsData = savings.FirstOrDefault((SavingsData o) => o.Tag == tag);
		if (savingsData != null && (savingsData.ActualSaving < 0.0 || savingsData.ActualSaving > 0.0))
		{
			return savingsData.ActualSaving.ToString("F3");
		}
		return string.Empty;
	}

	private static void CalculateEnergy(CalculationData calcData, Section section, BuildingZone zone, CalculationInput calcInput, CalculationData lightsAndDevicesCalculationData)
	{
		calcData.Calculations(section, calcInput, zone, lightsAndDevicesCalculationData);
		calcData.CalculateNetEnergy();
		calcData.CalculateGeneratorHeatEfficiencyBaseLine();
		calcData.CalculateNeededEnergyBaseLine();
	}

	private static void CalculateEnergyESM(CalculationData calcData, Section section, BuildingZone zone, CalculationInput calcInput, CalculationData lightsAndDevicesCalculationData)
	{
		calcData.Calculations(section, calcInput, zone, lightsAndDevicesCalculationData);
		calcData.CalculateNetEnergy();
		calcData.CalculateGeneratorHeatEfficiencyEsm();
		calcData.CalculateNeededEnergyEsm();
	}

	private static List<DataRow> GetBaseLine(CalculationData tempCalculationData)
	{
		return new List<DataRow>
		{
			new DataRow
			{
				Value = tempCalculationData.WorkingScheduleBaseLine,
				Tag = "WorkingSchedule"
			},
			new DataRow
			{
				Value = tempCalculationData.UouterWallsBaseLine,
				Tag = "UouterWalls"
			},
			new DataRow
			{
				Value = tempCalculationData.UwindowsBaseLine,
				Tag = "Uwindows"
			},
			new DataRow
			{
				Value = tempCalculationData.UnontransparentBaseLine,
				Tag = "Unontransparent"
			},
			new DataRow
			{
				Value = tempCalculationData.UfloorBaseLine,
				Tag = "Ufloor"
			},
			new DataRow
			{
				Value = tempCalculationData.gBaseLine,
				Tag = "g"
			},
			new DataRow
			{
				Value = tempCalculationData.UinnerWallsBaseLine,
				Tag = "UinnerWalls"
			},
			new DataRow
			{
				Value = tempCalculationData.UceilingBaseLine,
				Tag = "Uceiling"
			},
			new DataRow
			{
				Value = tempCalculationData.UfloorOtherBaseLine,
				Tag = "UfloorOther"
			},
			new DataRow
			{
				Value = tempCalculationData.InfiltracionBaseLine,
				Tag = "Infiltracion"
			},
			new DataRow
			{
				Value = tempCalculationData.ProjectTemperatureBaseLine,
				Tag = "ProjectTemperature"
			},
			new DataRow
			{
				Value = tempCalculationData.NonProjectTemperatureBaseLine,
				Tag = "NonProjectTemperature"
			},
			new DataRow
			{
				Value = tempCalculationData.ProjectHumidityBaseLine,
				Tag = "ProjectHumidity"
			},
			new DataRow
			{
				Value = tempCalculationData.DebitBaseLine,
				Tag = "Debit"
			},
			new DataRow
			{
				Value = tempCalculationData.ResulNoInputsNetEnergyBaseLine,
				Tag = "ResulNoInputsNetEnergy"
			},
			new DataRow
			{
				Value = tempCalculationData.ResulCoolingInputsBaseLine,
				Tag = "ResulCoolingInputs"
			},
			new DataRow
			{
				Value = tempCalculationData.ResulVentilationInputsBaseLine,
				Tag = "ResulVentilationInputs"
			},
			new DataRow
			{
				Value = tempCalculationData.ResulLightInputsBaseLine,
				Tag = "ResulLightInputs"
			},
			new DataRow
			{
				Value = tempCalculationData.ResulAppliancesInputsBaseLine,
				Tag = "ResulAppliancesInputs"
			},
			new DataRow
			{
				Value = tempCalculationData.ResulNetEnergyBaseLine,
				Tag = "ResulNetEnergy"
			},
			new DataRow
			{
				Fuel = tempCalculationData.Fuel1BaseLine,
				Tag = "Fuel1"
			},
			new DataRow
			{
				Value = tempCalculationData.Part1BaseLine,
				Tag = "Part1"
			},
			new DataRow
			{
				Value = tempCalculationData.TransmitTempEfficiencyBaseLine,
				Tag = "TransmitTempEfficiency"
			},
			new DataRow
			{
				Value = tempCalculationData.SupplyNetEfficiencyBaseLine,
				Tag = "SupplyNetEfficiency"
			},
			new DataRow
			{
				Value = tempCalculationData.AutomaticBaseLine,
				Tag = "Automatic"
			},
			new DataRow
			{
				Value = tempCalculationData.EnergyManagementBaseLine,
				Tag = "EnergyManagement"
			},
			new DataRow
			{
				Value = tempCalculationData.GeneratorHeatEfficiency1BaseLine,
				Tag = "GeneratorHeatEfficiency1"
			},
			new DataRow
			{
				Value = tempCalculationData.GeneratorColdEfficiency1BaseLine,
				Tag = "GeneratorColdEfficiency1"
			},
			new DataRow
			{
				Value = tempCalculationData.ResultSourceEnergyBaseLine,
				Tag = "ResultSourceEnergy"
			},
			new DataRow
			{
				Fuel = tempCalculationData.Fuel2BaseLine,
				Tag = "Fuel2"
			},
			new DataRow
			{
				Value = tempCalculationData.Part2BaseLine,
				Tag = "Part2"
			},
			new DataRow
			{
				Value = tempCalculationData.TransmitTempEfficiency2BaseLine,
				Tag = "TransmitTempEfficiency2"
			},
			new DataRow
			{
				Value = tempCalculationData.SupplyNetEfficiency2BaseLine,
				Tag = "SupplyNetEfficiency2"
			},
			new DataRow
			{
				Value = tempCalculationData.Automatic2BaseLine,
				Tag = "Automatic2"
			},
			new DataRow
			{
				Value = tempCalculationData.EnergyManagement2BaseLine,
				Tag = "EnergyManagement2"
			},
			new DataRow
			{
				Value = tempCalculationData.GeneratorHeatEfficiency2BaseLine,
				Tag = "GeneratorHeatEfficiency2"
			},
			new DataRow
			{
				Value = tempCalculationData.GeneratorColdEfficiency2BaseLine,
				Tag = "GeneratorColdEfficiency2"
			},
			new DataRow
			{
				Value = tempCalculationData.ResultSourceEnergy2BaseLine,
				Tag = "ResultSourceEnergy2"
			},
			new DataRow
			{
				Value = tempCalculationData.HeatEfficiencyGeneratingBaseLine,
				Tag = "HeatEfficiencyGenerating"
			},
			new DataRow
			{
				Value = tempCalculationData.ResultNeededEnergyBaseLine,
				Tag = "ResultNeededEnergy"
			}
		};
	}

	private static List<DataRow> GetESM(CalculationData tempCalculationData)
	{
		return new List<DataRow>
		{
			new DataRow
			{
				Value = tempCalculationData.WorkingScheduleESM,
				Tag = "WorkingSchedule"
			},
			new DataRow
			{
				Value = tempCalculationData.UouterWallsESM,
				Tag = "UouterWalls"
			},
			new DataRow
			{
				Value = tempCalculationData.UwindowsESM,
				Tag = "Uwindows"
			},
			new DataRow
			{
				Value = tempCalculationData.UnontransparentESM,
				Tag = "Unontransparent"
			},
			new DataRow
			{
				Value = tempCalculationData.UfloorESM,
				Tag = "Ufloor"
			},
			new DataRow
			{
				Value = tempCalculationData.gESM,
				Tag = "g"
			},
			new DataRow
			{
				Value = tempCalculationData.UinnerWallsESM,
				Tag = "UinnerWalls"
			},
			new DataRow
			{
				Value = tempCalculationData.UceilingESM,
				Tag = "Uceiling"
			},
			new DataRow
			{
				Value = tempCalculationData.UfloorOtherESM,
				Tag = "UfloorOther"
			},
			new DataRow
			{
				Value = tempCalculationData.InfiltracionESM,
				Tag = "Infiltracion"
			},
			new DataRow
			{
				Value = tempCalculationData.ProjectTemperatureESM,
				Tag = "ProjectTemperature"
			},
			new DataRow
			{
				Value = tempCalculationData.NonProjectTemperatureESM,
				Tag = "NonProjectTemperature"
			},
			new DataRow
			{
				Value = tempCalculationData.ProjectHumidityESM,
				Tag = "ProjectHumidity"
			},
			new DataRow
			{
				Value = tempCalculationData.DebitESM,
				Tag = "Debit"
			},
			new DataRow
			{
				Value = tempCalculationData.ResulNoInputsNetEnergyESM,
				Tag = "ResulNoInputsNetEnergy"
			},
			new DataRow
			{
				Value = tempCalculationData.ResulCoolingInputsESM,
				Tag = "ResulCoolingInputs"
			},
			new DataRow
			{
				Value = tempCalculationData.ResulVentilationInputsESM,
				Tag = "ResulVentilationInputs"
			},
			new DataRow
			{
				Value = tempCalculationData.ResulLightInputsESM,
				Tag = "ResulLightInputs"
			},
			new DataRow
			{
				Value = tempCalculationData.ResulAppliancesInputsESM,
				Tag = "ResulAppliancesInputs"
			},
			new DataRow
			{
				Value = tempCalculationData.ResulNetEnergyESM,
				Tag = "ResulNetEnergy"
			},
			new DataRow
			{
				Fuel = tempCalculationData.Fuel1ESM,
				Tag = "Fuel1"
			},
			new DataRow
			{
				Value = tempCalculationData.Part1ESM,
				Tag = "Part1"
			},
			new DataRow
			{
				Value = tempCalculationData.TransmitTempEfficiencyESM,
				Tag = "TransmitTempEfficiency"
			},
			new DataRow
			{
				Value = tempCalculationData.SupplyNetEfficiencyESM,
				Tag = "SupplyNetEfficiency"
			},
			new DataRow
			{
				Value = tempCalculationData.AutomaticESM,
				Tag = "Automatic"
			},
			new DataRow
			{
				Value = tempCalculationData.EnergyManagementESM,
				Tag = "EnergyManagement"
			},
			new DataRow
			{
				Value = tempCalculationData.GeneratorHeatEfficiency1ESM,
				Tag = "GeneratorHeatEfficiency1"
			},
			new DataRow
			{
				Value = tempCalculationData.GeneratorColdEfficiency1ESM,
				Tag = "GeneratorColdEfficiency1"
			},
			new DataRow
			{
				Value = tempCalculationData.ResultSourceEnergyESM,
				Tag = "ResultSourceEnergy"
			},
			new DataRow
			{
				Fuel = tempCalculationData.Fuel2ESM,
				Tag = "Fuel2"
			},
			new DataRow
			{
				Value = tempCalculationData.Part2ESM,
				Tag = "Part2"
			},
			new DataRow
			{
				Value = tempCalculationData.TransmitTempEfficiency2ESM,
				Tag = "TransmitTempEfficiency2"
			},
			new DataRow
			{
				Value = tempCalculationData.SupplyNetEfficiency2ESM,
				Tag = "SupplyNetEfficiency2"
			},
			new DataRow
			{
				Value = tempCalculationData.Automatic2ESM,
				Tag = "Automatic2"
			},
			new DataRow
			{
				Value = tempCalculationData.EnergyManagement2ESM,
				Tag = "EnergyManagement2"
			},
			new DataRow
			{
				Value = tempCalculationData.GeneratorHeatEfficiency2ESM,
				Tag = "GeneratorHeatEfficiency2"
			},
			new DataRow
			{
				Value = tempCalculationData.GeneratorColdEfficiency2ESM,
				Tag = "GeneratorColdEfficiency2"
			},
			new DataRow
			{
				Value = tempCalculationData.ResultSourceEnergy2ESM,
				Tag = "ResultSourceEnergy2"
			},
			new DataRow
			{
				Value = tempCalculationData.HeatEfficiencyGeneratingESM,
				Tag = "HeatEfficiencyGenerating"
			},
			new DataRow
			{
				Value = tempCalculationData.ResultNeededEnergyESM,
				Tag = "ResultNeededEnergy"
			}
		};
	}

	private static List<DataRow> GetVentilationBaseLine(CalculationData tempCalculationData)
	{
		return new List<DataRow>
		{
			new DataRow
			{
				Value = tempCalculationData.WorkingScheduleBaseLine,
				Tag = "WorkingSchedule"
			},
			new DataRow
			{
				Value = tempCalculationData.DebitBaseLine,
				Tag = "Debit"
			},
			new DataRow
			{
				Value = tempCalculationData.FlowTemperatureBaseLine,
				Tag = "FlowTemperature"
			},
			new DataRow
			{
				Value = tempCalculationData.RelativeHumidityBaseLine,
				Tag = "RelativeHumidity"
			},
			new DataRow
			{
				Value = tempCalculationData.ProjectHumidityBaseLine,
				Tag = "ProjectHumidity"
			},
			new DataRow
			{
				Value = tempCalculationData.FirstRecEfficiencyBaseLine,
				Tag = "FirstRecEfficiency"
			},
			new DataRow
			{
				Value = tempCalculationData.SecondRecEfficiencyBaseLine,
				Tag = "SecondRecEfficiency"
			},
			new DataRow
			{
				Value = tempCalculationData.HeatingAirDifferenceBaseLine,
				Tag = "HeatingAirDifference"
			},
			new DataRow
			{
				Value = tempCalculationData.MinimumEndTemperatureBaseLine,
				Tag = "MinimumEndTemperature"
			},
			new DataRow
			{
				Value = tempCalculationData.ResultEnergyForHeatingBaseLine,
				Tag = "ResultEnergyForHeating"
			},
			new DataRow
			{
				Fuel = tempCalculationData.Fuel1BaseLine,
				Tag = "Fuel1"
			},
			new DataRow
			{
				Value = tempCalculationData.Part1BaseLine,
				Tag = "Part1"
			},
			new DataRow
			{
				Value = tempCalculationData.TransmitTempEfficiencyBaseLine,
				Tag = "TransmitTempEfficiency"
			},
			new DataRow
			{
				Value = tempCalculationData.SupplyNetEfficiencyBaseLine,
				Tag = "SupplyNetEfficiency"
			},
			new DataRow
			{
				Value = tempCalculationData.AutomaticBaseLine,
				Tag = "Automatic"
			},
			new DataRow
			{
				Value = tempCalculationData.EnergyManagementBaseLine,
				Tag = "EnergyManagement"
			},
			new DataRow
			{
				Value = tempCalculationData.GeneratorHeatEfficiency1BaseLine,
				Tag = "GeneratorHeatEfficiency1"
			},
			new DataRow
			{
				Value = tempCalculationData.GeneratorColdEfficiency1BaseLine,
				Tag = "GeneratorColdEfficiency1"
			},
			new DataRow
			{
				Value = tempCalculationData.ResultSourceEnergyBaseLine,
				Tag = "ResultSourceEnergy"
			},
			new DataRow
			{
				Fuel = tempCalculationData.Fuel2BaseLine,
				Tag = "Fuel2"
			},
			new DataRow
			{
				Value = tempCalculationData.Part2BaseLine,
				Tag = "Part2"
			},
			new DataRow
			{
				Value = tempCalculationData.TransmitTempEfficiency2BaseLine,
				Tag = "TransmitTempEfficiency2"
			},
			new DataRow
			{
				Value = tempCalculationData.SupplyNetEfficiency2BaseLine,
				Tag = "SupplyNetEfficiency2"
			},
			new DataRow
			{
				Value = tempCalculationData.Automatic2BaseLine,
				Tag = "Automatic2"
			},
			new DataRow
			{
				Value = tempCalculationData.EnergyManagement2BaseLine,
				Tag = "EnergyManagement2"
			},
			new DataRow
			{
				Value = tempCalculationData.GeneratorHeatEfficiency2BaseLine,
				Tag = "GeneratorHeatEfficiency2"
			},
			new DataRow
			{
				Value = tempCalculationData.GeneratorColdEfficiency2BaseLine,
				Tag = "GeneratorColdEfficiency2"
			},
			new DataRow
			{
				Value = tempCalculationData.ResultSourceEnergy2BaseLine,
				Tag = "ResultSourceEnergy2"
			},
			new DataRow
			{
				Value = tempCalculationData.HeatEfficiencyGeneratingBaseLine,
				Tag = "HeatEfficiencyGenerating"
			},
			new DataRow
			{
				Value = tempCalculationData.ResulHeatingInputsBaseLine,
				Tag = "ResulHeatingInputs"
			},
			new DataRow
			{
				Value = tempCalculationData.ResulCoolingInputsBaseLine,
				Tag = "ResulCoolingInputs"
			},
			new DataRow
			{
				Value = tempCalculationData.ResultNeededEnergyBaseLine,
				Tag = "ResultNeededEnergy"
			}
		};
	}

	private static CalculationData SetBaseLine(IList<DataRow> baseLine, CalculationData toCalcData)
	{
		try
		{
			toCalcData.WorkingScheduleBaseLine = GetValue(baseLine, "WorkingSchedule");
			toCalcData.UouterWallsBaseLine = GetValue(baseLine, "UouterWalls");
			toCalcData.UwindowsBaseLine = GetValue(baseLine, "Uwindows");
			toCalcData.UnontransparentBaseLine = GetValue(baseLine, "Unontransparent");
			toCalcData.UfloorBaseLine = GetValue(baseLine, "Ufloor");
			toCalcData.gBaseLine = GetValue(baseLine, "g");
			toCalcData.UinnerWallsBaseLine = GetValue(baseLine, "UinnerWalls");
			toCalcData.UceilingBaseLine = GetValue(baseLine, "Uceiling");
			toCalcData.UfloorOtherBaseLine = GetValue(baseLine, "UfloorOther");
			toCalcData.InfiltracionBaseLine = GetValue(baseLine, "Infiltracion");
			toCalcData.ProjectTemperatureBaseLine = GetValue(baseLine, "ProjectTemperature");
			toCalcData.NonProjectTemperatureBaseLine = GetValue(baseLine, "NonProjectTemperature");
			toCalcData.ProjectHumidityBaseLine = GetValue(baseLine, "ProjectHumidity");
			toCalcData.DebitBaseLine = GetValue(baseLine, "Debit");
			toCalcData.ResulNoInputsNetEnergyBaseLine = GetValue(baseLine, "ResulNoInputsNetEnergy");
			toCalcData.ResulCoolingInputsBaseLine = GetValue(baseLine, "ResulCoolingInputs");
			toCalcData.ResulVentilationInputsBaseLine = GetValue(baseLine, "ResulVentilationInputs");
			toCalcData.ResulLightInputsBaseLine = GetValue(baseLine, "ResulLightInputs");
			toCalcData.ResulAppliancesInputsBaseLine = GetValue(baseLine, "ResulAppliancesInputs");
			toCalcData.ResulNetEnergyBaseLine = GetValue(baseLine, "ResulNetEnergy");
			DataRow dataRow = baseLine.FirstOrDefault((DataRow o) => o.Tag == "Fuel1");
			if (dataRow != null)
			{
				toCalcData.Fuel1BaseLine = dataRow.Fuel;
			}
			toCalcData.Part1BaseLine = GetValue(baseLine, "Part1");
			toCalcData.TransmitTempEfficiencyBaseLine = GetValue(baseLine, "TransmitTempEfficiency");
			toCalcData.SupplyNetEfficiencyBaseLine = GetValue(baseLine, "SupplyNetEfficiency");
			toCalcData.AutomaticBaseLine = GetValue(baseLine, "Automatic");
			toCalcData.EnergyManagementBaseLine = GetValue(baseLine, "EnergyManagement");
			toCalcData.GeneratorHeatEfficiency1BaseLine = GetValue(baseLine, "GeneratorHeatEfficiency1");
			toCalcData.GeneratorColdEfficiency1BaseLine = GetValue(baseLine, "GeneratorColdEfficiency1");
			toCalcData.ResultSourceEnergyBaseLine = GetValue(baseLine, "ResultSourceEnergy");
			DataRow dataRow2 = baseLine.FirstOrDefault((DataRow o) => o.Tag == "Fuel2");
			if (dataRow2 != null)
			{
				toCalcData.Fuel2BaseLine = dataRow2.Fuel;
			}
			toCalcData.Part2BaseLine = GetValue(baseLine, "Part2");
			toCalcData.TransmitTempEfficiency2BaseLine = GetValue(baseLine, "TransmitTempEfficiency2");
			toCalcData.SupplyNetEfficiency2BaseLine = GetValue(baseLine, "SupplyNetEfficiency2");
			toCalcData.Automatic2BaseLine = GetValue(baseLine, "Automatic2");
			toCalcData.EnergyManagement2BaseLine = GetValue(baseLine, "EnergyManagement2");
			toCalcData.GeneratorHeatEfficiency2BaseLine = GetValue(baseLine, "GeneratorHeatEfficiency2");
			toCalcData.GeneratorColdEfficiency2BaseLine = GetValue(baseLine, "GeneratorColdEfficiency2");
			toCalcData.ResultSourceEnergy2BaseLine = GetValue(baseLine, "ResultSourceEnergy2");
			toCalcData.HeatEfficiencyGeneratingBaseLine = GetValue(baseLine, "HeatEfficiencyGenerating");
			toCalcData.ResultNeededEnergyBaseLine = GetValue(baseLine, "ResultNeededEnergy");
		}
		catch (Exception)
		{
			throw;
		}
		return toCalcData;
	}

	private static CalculationData SetESM(IList<DataRow> esm, CalculationData toCalcData)
	{
		try
		{
			toCalcData.WorkingScheduleESM = GetValue(esm, "WorkingSchedule");
			toCalcData.UouterWallsESM = GetValue(esm, "UouterWalls");
			toCalcData.UwindowsESM = GetValue(esm, "Uwindows");
			toCalcData.UnontransparentESM = GetValue(esm, "Unontransparent");
			toCalcData.UfloorESM = GetValue(esm, "Ufloor");
			toCalcData.gESM = GetValue(esm, "g");
			toCalcData.UinnerWallsESM = GetValue(esm, "UinnerWalls");
			toCalcData.UceilingESM = GetValue(esm, "Uceiling");
			toCalcData.UfloorOtherESM = GetValue(esm, "UfloorOther");
			toCalcData.InfiltracionESM = GetValue(esm, "Infiltracion");
			toCalcData.ProjectTemperatureESM = GetValue(esm, "ProjectTemperature");
			toCalcData.NonProjectTemperatureESM = GetValue(esm, "NonProjectTemperature");
			toCalcData.ProjectHumidityESM = GetValue(esm, "ProjectHumidity");
			toCalcData.DebitESM = GetValue(esm, "Debit");
			toCalcData.ResulNoInputsNetEnergyESM = GetValue(esm, "ResulNoInputsNetEnergy");
			toCalcData.ResulCoolingInputsESM = GetValue(esm, "ResulCoolingInputs");
			toCalcData.ResulVentilationInputsESM = GetValue(esm, "ResulVentilationInputs");
			toCalcData.ResulLightInputsESM = GetValue(esm, "ResulLightInputs");
			toCalcData.ResulAppliancesInputsESM = GetValue(esm, "ResulAppliancesInputs");
			toCalcData.ResulNetEnergyESM = GetValue(esm, "ResulNetEnergy");
			DataRow dataRow = esm.FirstOrDefault((DataRow o) => o.Tag == "Fuel1");
			if (dataRow != null)
			{
				toCalcData.Fuel1ESM = dataRow.Fuel;
			}
			toCalcData.Part1ESM = GetValue(esm, "Part1");
			toCalcData.TransmitTempEfficiencyESM = GetValue(esm, "TransmitTempEfficiency");
			toCalcData.SupplyNetEfficiencyESM = GetValue(esm, "SupplyNetEfficiency");
			toCalcData.AutomaticESM = GetValue(esm, "Automatic");
			toCalcData.EnergyManagementESM = GetValue(esm, "EnergyManagement");
			toCalcData.GeneratorHeatEfficiency1ESM = GetValue(esm, "GeneratorHeatEfficiency1");
			toCalcData.GeneratorColdEfficiency1ESM = GetValue(esm, "GeneratorColdEfficiency1");
			toCalcData.ResultSourceEnergyESM = GetValue(esm, "ResultSourceEnergy");
			DataRow dataRow2 = esm.FirstOrDefault((DataRow o) => o.Tag == "Fuel2");
			if (dataRow2 != null)
			{
				toCalcData.Fuel2ESM = dataRow2.Fuel;
			}
			toCalcData.Part2ESM = GetValue(esm, "Part2");
			toCalcData.TransmitTempEfficiency2ESM = GetValue(esm, "TransmitTempEfficiency2");
			toCalcData.SupplyNetEfficiency2ESM = GetValue(esm, "SupplyNetEfficiency2");
			toCalcData.Automatic2ESM = GetValue(esm, "Automatic2");
			toCalcData.EnergyManagement2ESM = GetValue(esm, "EnergyManagement2");
			toCalcData.GeneratorHeatEfficiency2ESM = GetValue(esm, "GeneratorHeatEfficiency2");
			toCalcData.GeneratorColdEfficiency2ESM = GetValue(esm, "GeneratorColdEfficiency2");
			toCalcData.ResultSourceEnergy2ESM = GetValue(esm, "ResultSourceEnergy2");
			toCalcData.HeatEfficiencyGeneratingESM = GetValue(esm, "HeatEfficiencyGenerating");
			toCalcData.ResultNeededEnergyESM = GetValue(esm, "ResultNeededEnergy");
		}
		catch (Exception)
		{
			throw;
		}
		return toCalcData;
	}

	private static CalculationData SetVentilationBaseLine(IList<DataRow> baseLine, CalculationData toCalcData)
	{
		try
		{
			toCalcData.WorkingScheduleBaseLine = GetValue(baseLine, "WorkingSchedule");
			toCalcData.DebitBaseLine = GetValue(baseLine, "Debit");
			toCalcData.FlowTemperatureBaseLine = GetValue(baseLine, "FlowTemperature");
			toCalcData.RelativeHumidityBaseLine = GetValue(baseLine, "RelativeHumidity");
			toCalcData.ProjectHumidityBaseLine = GetValue(baseLine, "ProjectHumidity");
			toCalcData.FirstRecEfficiencyBaseLine = GetValue(baseLine, "FirstRecEfficiency");
			toCalcData.SecondRecEfficiencyBaseLine = GetValue(baseLine, "SecondRecEfficiency");
			toCalcData.HeatingAirDifferenceBaseLine = GetValue(baseLine, "HeatingAirDifference");
			toCalcData.MinimumEndTemperatureBaseLine = GetValue(baseLine, "MinimumEndTemperature");
			toCalcData.ResultEnergyForHeatingBaseLine = GetValue(baseLine, "ResultEnergyForHeating");
			toCalcData.ResultEnergyForCoolingBaseLine = GetValue(baseLine, "ResultEnergyForCooling");
			DataRow dataRow = baseLine.FirstOrDefault((DataRow o) => o.Tag == "Fuel1");
			if (dataRow != null)
			{
				toCalcData.Fuel1BaseLine = dataRow.Fuel;
			}
			toCalcData.Part1BaseLine = GetValue(baseLine, "Part1");
			toCalcData.TransmitTempEfficiencyBaseLine = GetValue(baseLine, "TransmitTempEfficiency");
			toCalcData.SupplyNetEfficiencyBaseLine = GetValue(baseLine, "SupplyNetEfficiency");
			toCalcData.AutomaticBaseLine = GetValue(baseLine, "Automatic");
			toCalcData.EnergyManagementBaseLine = GetValue(baseLine, "EnergyManagement");
			toCalcData.GeneratorHeatEfficiency1BaseLine = GetValue(baseLine, "GeneratorHeatEfficiency1");
			toCalcData.GeneratorColdEfficiency1BaseLine = GetValue(baseLine, "GeneratorColdEfficiency1");
			toCalcData.ResultSourceEnergyBaseLine = GetValue(baseLine, "ResultSourceEnergy");
			DataRow dataRow2 = baseLine.FirstOrDefault((DataRow o) => o.Tag == "Fuel2");
			if (dataRow2 != null)
			{
				toCalcData.Fuel2BaseLine = dataRow2.Fuel;
			}
			toCalcData.Part2BaseLine = GetValue(baseLine, "Part2");
			toCalcData.TransmitTempEfficiency2BaseLine = GetValue(baseLine, "TransmitTempEfficiency2");
			toCalcData.SupplyNetEfficiency2BaseLine = GetValue(baseLine, "SupplyNetEfficiency2");
			toCalcData.Automatic2BaseLine = GetValue(baseLine, "Automatic2");
			toCalcData.EnergyManagement2BaseLine = GetValue(baseLine, "EnergyManagement2");
			toCalcData.GeneratorHeatEfficiency2BaseLine = GetValue(baseLine, "GeneratorHeatEfficiency2");
			toCalcData.GeneratorColdEfficiency2BaseLine = GetValue(baseLine, "GeneratorColdEfficiency2");
			toCalcData.ResultSourceEnergy2BaseLine = GetValue(baseLine, "ResultSourceEnergy2");
			toCalcData.HeatEfficiencyGeneratingBaseLine = GetValue(baseLine, "HeatEfficiencyGenerating");
			toCalcData.ResulHeatingInputsBaseLine = GetValue(baseLine, "ResulHeatingInputs");
			toCalcData.ResulCoolingInputsBaseLine = GetValue(baseLine, "ResulCoolingInputs");
			toCalcData.ResultNeededEnergyBaseLine = GetValue(baseLine, "ResultNeededEnergy");
		}
		catch (Exception)
		{
			throw;
		}
		return toCalcData;
	}

	private static List<DataRow> GetHotWaterBaseLine(CalculationData tempCalculationData)
	{
		return new List<DataRow>
		{
			new DataRow
			{
				Value = tempCalculationData.ConsumptionBaseLine,
				Tag = "Consumption"
			},
			new DataRow
			{
				Value = tempCalculationData.TempDifferenceBaseLine,
				Tag = "TempDifference"
			},
			new DataRow
			{
				Value = tempCalculationData.HotWaterBaseLine,
				Tag = "HotWater"
			},
			new DataRow
			{
				Value = tempCalculationData.MixedWaterBaseLine,
				Tag = "MixedWater"
			},
			new DataRow
			{
				Value = tempCalculationData.ResulNetEnergyBaseLine,
				Tag = "ResulNetEnergy"
			},
			new DataRow
			{
				Value = tempCalculationData.SunEnergyBaseLine,
				Tag = "SunEnergy"
			},
			new DataRow
			{
				Value = tempCalculationData.ResultEnergyForHeatingBaseLine,
				Tag = "ResultEnergyForHeating"
			},
			new DataRow
			{
				Fuel = tempCalculationData.Fuel1BaseLine,
				Tag = "Fuel1"
			},
			new DataRow
			{
				Value = tempCalculationData.Part1BaseLine,
				Tag = "Part1"
			},
			new DataRow
			{
				Value = tempCalculationData.TransmitTempEfficiencyBaseLine,
				Tag = "TransmitTempEfficiency"
			},
			new DataRow
			{
				Value = tempCalculationData.SupplyNetEfficiencyBaseLine,
				Tag = "SupplyNetEfficiency"
			},
			new DataRow
			{
				Value = tempCalculationData.AutomaticBaseLine,
				Tag = "Automatic"
			},
			new DataRow
			{
				Value = tempCalculationData.EnergyManagementBaseLine,
				Tag = "EnergyManagement"
			},
			new DataRow
			{
				Value = tempCalculationData.GeneratorHeatEfficiency1BaseLine,
				Tag = "GeneratorHeatEfficiency1"
			},
			new DataRow
			{
				Value = tempCalculationData.ResultSourceEnergyBaseLine,
				Tag = "ResultSourceEnergy"
			},
			new DataRow
			{
				Fuel = tempCalculationData.Fuel2BaseLine,
				Tag = "Fuel2"
			},
			new DataRow
			{
				Value = tempCalculationData.Part2BaseLine,
				Tag = "Part2"
			},
			new DataRow
			{
				Value = tempCalculationData.TransmitTempEfficiency2BaseLine,
				Tag = "TransmitTempEfficiency2"
			},
			new DataRow
			{
				Value = tempCalculationData.SupplyNetEfficiency2BaseLine,
				Tag = "SupplyNetEfficiency2"
			},
			new DataRow
			{
				Value = tempCalculationData.Automatic2BaseLine,
				Tag = "Automatic2"
			},
			new DataRow
			{
				Value = tempCalculationData.EnergyManagement2BaseLine,
				Tag = "EnergyManagement2"
			},
			new DataRow
			{
				Value = tempCalculationData.GeneratorHeatEfficiency2BaseLine,
				Tag = "GeneratorHeatEfficiency2"
			},
			new DataRow
			{
				Value = tempCalculationData.ResultSourceEnergy2BaseLine,
				Tag = "ResultSourceEnergy2"
			},
			new DataRow
			{
				Value = tempCalculationData.HeatEfficiencyGeneratingBaseLine,
				Tag = "HeatEfficiencyGenerating"
			},
			new DataRow
			{
				Value = tempCalculationData.ResultNeededEnergyBaseLine,
				Tag = "ResultNeededEnergy"
			}
		};
	}

	private static CalculationData SetHotWaterBaseLine(IList<DataRow> baseLine, CalculationData toCalcData)
	{
		try
		{
			toCalcData.ConsumptionBaseLine = GetValue(baseLine, "Consumption");
			toCalcData.TempDifferenceBaseLine = GetValue(baseLine, "TempDifference");
			toCalcData.MixedWaterBaseLine = GetValue(baseLine, "MixedWater");
			toCalcData.ResulNetEnergyBaseLine = GetValue(baseLine, "ResulNetEnergy");
			toCalcData.SunEnergyBaseLine = GetValue(baseLine, "SunEnergy");
			toCalcData.ResultEnergyForHeatingBaseLine = GetValue(baseLine, "ResultEnergyForHeating");
			DataRow dataRow = baseLine.FirstOrDefault((DataRow o) => o.Tag == "Fuel1");
			if (dataRow != null)
			{
				toCalcData.Fuel1BaseLine = dataRow.Fuel;
			}
			toCalcData.Part1BaseLine = GetValue(baseLine, "Part1");
			toCalcData.TransmitTempEfficiencyBaseLine = GetValue(baseLine, "TransmitTempEfficiency");
			toCalcData.SupplyNetEfficiencyBaseLine = GetValue(baseLine, "SupplyNetEfficiency");
			toCalcData.AutomaticBaseLine = GetValue(baseLine, "Automatic");
			toCalcData.EnergyManagementBaseLine = GetValue(baseLine, "EnergyManagement");
			toCalcData.GeneratorHeatEfficiency1BaseLine = GetValue(baseLine, "GeneratorHeatEfficiency1");
			toCalcData.ResultSourceEnergyBaseLine = GetValue(baseLine, "ResultSourceEnergy");
			DataRow dataRow2 = baseLine.FirstOrDefault((DataRow o) => o.Tag == "Fuel2");
			if (dataRow2 != null)
			{
				toCalcData.Fuel2BaseLine = dataRow2.Fuel;
			}
			toCalcData.Part2BaseLine = GetValue(baseLine, "Part2");
			toCalcData.TransmitTempEfficiency2BaseLine = GetValue(baseLine, "TransmitTempEfficiency2");
			toCalcData.SupplyNetEfficiency2BaseLine = GetValue(baseLine, "SupplyNetEfficiency2");
			toCalcData.Automatic2BaseLine = GetValue(baseLine, "Automatic2");
			toCalcData.EnergyManagement2BaseLine = GetValue(baseLine, "EnergyManagement2");
			toCalcData.GeneratorHeatEfficiency2BaseLine = GetValue(baseLine, "GeneratorHeatEfficiency2");
			toCalcData.ResultSourceEnergy2BaseLine = GetValue(baseLine, "ResultSourceEnergy2");
			toCalcData.HeatEfficiencyGeneratingBaseLine = GetValue(baseLine, "HeatEfficiencyGenerating");
			toCalcData.ResultNeededEnergyBaseLine = GetValue(baseLine, "ResultNeededEnergy");
		}
		catch (Exception)
		{
			throw;
		}
		return toCalcData;
	}

	private static void CalculateUsavingType(string tag, Section tempSection, Section section)
	{
		switch (tag)
		{
		case "UouterWalls":
			CalculateUOuterWallsSaving(tempSection, section);
			CalculateGsavings(tempSection, section);
			break;
		case "Uwindows":
			CalculateUwindowsSavings(tempSection, section);
			break;
		case "Unontransparent":
			CalculateUnonTransparentSavings(tempSection, section);
			break;
		case "Ufloor":
			CalculateUfloorSavings(tempSection, section);
			break;
		case "UinnerWalls":
			CalculateUInnerWallsSaving(tempSection, section);
			break;
		case "Uceiling":
			CalculateUceilingsavings(tempSection, section);
			break;
		case "UfloorOther":
			CalculateUfloorOthersavings(tempSection, section);
			break;
		}
	}

	private static void CalculateUsavingTypeESM(string tag, Section tempSection, Section section)
	{
		switch (tag)
		{
		case "UouterWalls":
			CalculateUOuterWallsSavingESM(tempSection, section);
			break;
		case "Uwindows":
			CalculateUwindowsSavingsESM(tempSection, section);
			break;
		case "Unontransparent":
			CalculateUnonTransparentSavingsESM(tempSection, section);
			break;
		case "Ufloor":
			CalculateUfloorSavingsESM(tempSection, section);
			break;
		case "g":
			CalculateGsavingsESM(tempSection, section);
			break;
		case "UinnerWalls":
			CalculateUInnerWallsSavingESM(tempSection, section);
			break;
		case "Uceiling":
			CalculateUceilingsavingsESM(tempSection, section);
			break;
		case "UfloorOther":
			CalculateUfloorOthersavingsESM(tempSection, section);
			break;
		}
	}

	private static void CopyHeatingWorkingSchedule(Section tempSection, Section section)
	{
		tempSection.HeatingSeasons.Heating.WorkBaseStart = section.HeatingSeasons.Heating.WorkEsmStart;
		tempSection.HeatingSeasons.Heating.WorkBaseEnd = section.HeatingSeasons.Heating.WorkEsmEnd;
		tempSection.HeatingSeasons.Heating.SatBaseStart = section.HeatingSeasons.Heating.SatEsmStart;
		tempSection.HeatingSeasons.Heating.SatBaseEnd = section.HeatingSeasons.Heating.SatEsmEnd;
		tempSection.HeatingSeasons.Heating.SunBaseStart = section.HeatingSeasons.Heating.SunEsmStart;
		tempSection.HeatingSeasons.Heating.SunBaseEnd = section.HeatingSeasons.Heating.SunEsmEnd;
	}

	private static void CopyHeatingWorkingScheduleESM(Section tempSection, Section section)
	{
		tempSection.HeatingSeasons.Heating.WorkEsmStart = section.HeatingSeasons.Heating.WorkBaseStart;
		tempSection.HeatingSeasons.Heating.WorkEsmEnd = section.HeatingSeasons.Heating.WorkBaseEnd;
		tempSection.HeatingSeasons.Heating.SatEsmStart = section.HeatingSeasons.Heating.SatBaseStart;
		tempSection.HeatingSeasons.Heating.SatEsmEnd = section.HeatingSeasons.Heating.SatBaseEnd;
		tempSection.HeatingSeasons.Heating.SunEsmStart = section.HeatingSeasons.Heating.SunBaseStart;
		tempSection.HeatingSeasons.Heating.SunEsmEnd = section.HeatingSeasons.Heating.SunBaseEnd;
	}

	private static void CopyVentilationHeatingWorkingSchedule(Section tempSection, Section section)
	{
		tempSection.HeatingSeasons.Ventilation.WorkBaseStart = section.HeatingSeasons.Ventilation.WorkEsmStart;
		tempSection.HeatingSeasons.Ventilation.WorkBaseEnd = section.HeatingSeasons.Ventilation.WorkEsmEnd;
		tempSection.HeatingSeasons.Ventilation.SatBaseStart = section.HeatingSeasons.Ventilation.SatEsmStart;
		tempSection.HeatingSeasons.Ventilation.SatBaseEnd = section.HeatingSeasons.Ventilation.SatEsmEnd;
		tempSection.HeatingSeasons.Ventilation.SunBaseStart = section.HeatingSeasons.Ventilation.SunEsmStart;
		tempSection.HeatingSeasons.Ventilation.SunBaseEnd = section.HeatingSeasons.Ventilation.SunEsmEnd;
	}

	private static void CopyCoolingWorkingSchedule(Section tempSection, Section section)
	{
		tempSection.CoolingSeasons.Cooling.WorkBaseStart = section.CoolingSeasons.Cooling.WorkEsmStart;
		tempSection.CoolingSeasons.Cooling.WorkBaseEnd = section.CoolingSeasons.Cooling.WorkEsmEnd;
		tempSection.CoolingSeasons.Cooling.SatBaseStart = section.CoolingSeasons.Cooling.SatEsmStart;
		tempSection.CoolingSeasons.Cooling.SatBaseEnd = section.CoolingSeasons.Cooling.SatEsmEnd;
		tempSection.CoolingSeasons.Cooling.SunBaseStart = section.CoolingSeasons.Cooling.SunEsmStart;
		tempSection.CoolingSeasons.Cooling.SunBaseEnd = section.CoolingSeasons.Cooling.SunEsmEnd;
	}

	private static void CopyCoolingWorkingScheduleESM(Section tempSection, Section section)
	{
		tempSection.CoolingSeasons.Cooling.WorkEsmStart = section.CoolingSeasons.Cooling.WorkBaseStart;
		tempSection.CoolingSeasons.Cooling.WorkEsmEnd = section.CoolingSeasons.Cooling.WorkBaseEnd;
		tempSection.CoolingSeasons.Cooling.SatEsmStart = section.CoolingSeasons.Cooling.SatBaseStart;
		tempSection.CoolingSeasons.Cooling.SatEsmEnd = section.CoolingSeasons.Cooling.SatBaseEnd;
		tempSection.CoolingSeasons.Cooling.SunEsmStart = section.CoolingSeasons.Cooling.SunBaseStart;
		tempSection.CoolingSeasons.Cooling.SunEsmEnd = section.CoolingSeasons.Cooling.SunBaseEnd;
	}

	private static void CopyVentilationCoolingWorkingSchedule(Section tempSection, Section section)
	{
		tempSection.CoolingSeasons.Ventilation.WorkBaseStart = section.CoolingSeasons.Ventilation.WorkEsmStart;
		tempSection.CoolingSeasons.Ventilation.WorkBaseEnd = section.CoolingSeasons.Ventilation.WorkEsmEnd;
		tempSection.CoolingSeasons.Ventilation.SatBaseStart = section.CoolingSeasons.Ventilation.SatEsmStart;
		tempSection.CoolingSeasons.Ventilation.SatBaseEnd = section.CoolingSeasons.Ventilation.SatEsmEnd;
		tempSection.CoolingSeasons.Ventilation.SunBaseStart = section.CoolingSeasons.Ventilation.SunEsmStart;
		tempSection.CoolingSeasons.Ventilation.SunBaseEnd = section.CoolingSeasons.Ventilation.SunEsmEnd;
	}

	private static void CalculateGsavings(Section tempSection, Section section)
	{
		CopyWindowselements(tempSection.NorthWalls, section.NorthWalls);
		CopyWindowselements(tempSection.NorthEastWalls, section.NorthEastWalls);
		CopyWindowselements(tempSection.EastWalls, section.EastWalls);
		CopyWindowselements(tempSection.SouthEastWalls, section.SouthEastWalls);
		CopyWindowselements(tempSection.SouthWalls, section.SouthWalls);
		CopyWindowselements(tempSection.SouthWestWalls, section.SouthWestWalls);
		CopyWindowselements(tempSection.WestWalls, section.WestWalls);
		CopyWindowselements(tempSection.NorthWestWalls, section.NorthWestWalls);
		CopyTrasparentGelements(tempSection.Roof, section.Roof);
	}

	private static void CalculateGsavingsESM(Section tempSection, Section section)
	{
		CopyWindowselementsESM(tempSection.NorthWalls, section.NorthWalls);
		CopyWindowselementsESM(tempSection.NorthEastWalls, section.NorthEastWalls);
		CopyWindowselementsESM(tempSection.EastWalls, section.EastWalls);
		CopyWindowselementsESM(tempSection.SouthEastWalls, section.SouthEastWalls);
		CopyWindowselementsESM(tempSection.SouthWalls, section.SouthWalls);
		CopyWindowselementsESM(tempSection.SouthWestWalls, section.SouthWestWalls);
		CopyWindowselementsESM(tempSection.WestWalls, section.WestWalls);
		CopyWindowselementsESM(tempSection.NorthWestWalls, section.NorthWestWalls);
		CopyTrasparentGelementsESM(tempSection.Roof, section.Roof);
	}

	private static void CopyWindowselements(WallsStates tempWall, WallsStates wall)
	{
		tempWall.Current.WindowG1 = wall.Esm.WindowG1;
		tempWall.Current.WindowG2 = wall.Esm.WindowG2;
		tempWall.Current.WindowG3 = wall.Esm.WindowG3;
		tempWall.Current.WindowG4 = wall.Esm.WindowG4;
		tempWall.Current.WindowG5 = wall.Esm.WindowG5;
		tempWall.Current.WindowG6 = wall.Esm.WindowG6;
		tempWall.Current.AccumulateWindowG = tempWall.Esm.AccumulateWindowG;
	}

	private static void CopyWindowselementsESM(WallsStates tempWall, WallsStates wall)
	{
		tempWall.Esm.WindowG1 = wall.Current.WindowG1;
		tempWall.Esm.WindowG2 = wall.Current.WindowG2;
		tempWall.Esm.WindowG3 = wall.Current.WindowG3;
		tempWall.Esm.WindowG4 = wall.Current.WindowG4;
		tempWall.Esm.WindowG5 = wall.Current.WindowG5;
		tempWall.Esm.WindowG6 = wall.Current.WindowG6;
		tempWall.Esm.AccumulateWindowG = tempWall.Current.AccumulateWindowG;
	}

	private static void CopyTrasparentGelements(RoofStates tempRoof, RoofStates roof)
	{
		tempRoof.Current.TransparentG1 = roof.Esm.TransparentG1;
		tempRoof.Current.TransparentG2 = roof.Esm.TransparentG2;
		tempRoof.Current.TransparentG3 = roof.Esm.TransparentG3;
		tempRoof.Current.TransparentG4 = roof.Esm.TransparentG4;
		tempRoof.Current.TransparentG5 = roof.Esm.TransparentG5;
		tempRoof.Current.TransparentG6 = roof.Esm.TransparentG6;
		tempRoof.Current.TransparentG7 = roof.Esm.TransparentG7;
		tempRoof.Current.TransparentG8 = roof.Esm.TransparentG8;
		tempRoof.Current.TransparentG9 = roof.Esm.TransparentG9;
		tempRoof.Current.AccumulateTransparentG = roof.Esm.AccumulateTransparentG;
	}

	private static void CopyTrasparentGelementsESM(RoofStates tempRoof, RoofStates roof)
	{
		tempRoof.Esm.TransparentG1 = roof.Current.TransparentG1;
		tempRoof.Esm.TransparentG2 = roof.Current.TransparentG2;
		tempRoof.Esm.TransparentG3 = roof.Current.TransparentG3;
		tempRoof.Esm.TransparentG4 = roof.Current.TransparentG4;
		tempRoof.Esm.TransparentG5 = roof.Current.TransparentG5;
		tempRoof.Esm.TransparentG6 = roof.Current.TransparentG6;
		tempRoof.Esm.TransparentG7 = roof.Current.TransparentG7;
		tempRoof.Esm.TransparentG8 = roof.Current.TransparentG8;
		tempRoof.Esm.TransparentG9 = roof.Current.TransparentG9;
		tempRoof.Esm.AccumulateTransparentG = roof.Current.AccumulateTransparentG;
	}

	private static void CalculateUfloorSavings(Section tempSection, Section section)
	{
		CopyFloorElements(tempSection.Floor, section.Floor);
	}

	private static void CalculateUfloorSavingsESM(Section tempSection, Section section)
	{
		CopyFloorElementsESM(tempSection.Floor, section.Floor);
	}

	private static void CopyFloorElements(FloorStates tempfloor, FloorStates floor)
	{
		tempfloor.Current.FloorU1 = floor.Esm.FloorU1;
		tempfloor.Current.FloorU2 = floor.Esm.FloorU2;
		tempfloor.Current.FloorU3 = floor.Esm.FloorU3;
		tempfloor.Current.FloorU4 = floor.Esm.FloorU4;
		tempfloor.Current.FloorU5 = floor.Esm.FloorU5;
		tempfloor.Current.FloorU6 = floor.Esm.FloorU6;
		tempfloor.Current.AccumulateFloorU = floor.Esm.AccumulateFloorU;
	}

	private static void CopyFloorElementsESM(FloorStates tempfloor, FloorStates floor)
	{
		tempfloor.Esm.FloorU1 = floor.Current.FloorU1;
		tempfloor.Esm.FloorU2 = floor.Current.FloorU2;
		tempfloor.Esm.FloorU3 = floor.Current.FloorU3;
		tempfloor.Esm.FloorU4 = floor.Current.FloorU4;
		tempfloor.Esm.FloorU5 = floor.Current.FloorU5;
		tempfloor.Esm.FloorU6 = floor.Current.FloorU6;
		tempfloor.Esm.AccumulateFloorU = floor.Current.AccumulateFloorU;
	}

	private static void CalculateUfloorOthersavings(Section tempSection, Section section)
	{
		CopyOtherFloorElements(tempSection.Floor, section.Floor);
	}

	private static void CalculateUfloorOthersavingsESM(Section tempSection, Section section)
	{
		CopyOtherFloorElementsESM(tempSection.Floor, section.Floor);
	}

	private static void CopyOtherFloorElements(FloorStates tempfloor, FloorStates floor)
	{
		tempfloor.Current.OtherFloorU1 = floor.Esm.OtherFloorU1;
		tempfloor.Current.OtherFloorU2 = floor.Esm.OtherFloorU2;
		tempfloor.Current.OtherFloorU3 = floor.Esm.OtherFloorU3;
		tempfloor.Current.OtherFloorU4 = floor.Esm.OtherFloorU4;
		tempfloor.Current.OtherFloorU5 = floor.Esm.OtherFloorU5;
		tempfloor.Current.OtherFloorU6 = floor.Esm.OtherFloorU6;
		tempfloor.Current.AccumulateOtherFloorU = floor.Esm.AccumulateOtherFloorU;
	}

	private static void CopyOtherFloorElementsESM(FloorStates tempfloor, FloorStates floor)
	{
		tempfloor.Esm.OtherFloorU1 = floor.Current.OtherFloorU1;
		tempfloor.Esm.OtherFloorU2 = floor.Current.OtherFloorU2;
		tempfloor.Esm.OtherFloorU3 = floor.Current.OtherFloorU3;
		tempfloor.Esm.OtherFloorU4 = floor.Current.OtherFloorU4;
		tempfloor.Esm.OtherFloorU5 = floor.Current.OtherFloorU5;
		tempfloor.Esm.OtherFloorU6 = floor.Current.OtherFloorU6;
		tempfloor.Esm.AccumulateOtherFloorU = floor.Current.AccumulateOtherFloorU;
	}

	private static void CalculateUOuterWallsSaving(Section tempSection, Section section)
	{
		CopyOuterWallsElements(tempSection.NorthWalls, section.NorthWalls);
		CopyOuterWallsElements(tempSection.NorthEastWalls, section.NorthEastWalls);
		CopyOuterWallsElements(tempSection.EastWalls, section.EastWalls);
		CopyOuterWallsElements(tempSection.SouthEastWalls, section.SouthEastWalls);
		CopyOuterWallsElements(tempSection.SouthWalls, section.SouthWalls);
		CopyOuterWallsElements(tempSection.SouthWestWalls, section.SouthWestWalls);
		CopyOuterWallsElements(tempSection.WestWalls, section.WestWalls);
		CopyOuterWallsElements(tempSection.NorthWestWalls, section.NorthWestWalls);
	}

	private static void CalculateUOuterWallsSavingESM(Section tempSection, Section section)
	{
		CopyOuterWallsElementsESM(tempSection.NorthWalls, section.NorthWalls);
		CopyOuterWallsElementsESM(tempSection.NorthEastWalls, section.NorthEastWalls);
		CopyOuterWallsElementsESM(tempSection.EastWalls, section.EastWalls);
		CopyOuterWallsElementsESM(tempSection.SouthEastWalls, section.SouthEastWalls);
		CopyOuterWallsElementsESM(tempSection.SouthWalls, section.SouthWalls);
		CopyOuterWallsElementsESM(tempSection.SouthWestWalls, section.SouthWestWalls);
		CopyOuterWallsElementsESM(tempSection.WestWalls, section.WestWalls);
		CopyOuterWallsElementsESM(tempSection.NorthWestWalls, section.NorthWestWalls);
	}

	private static void CalculateUInnerWallsSaving(Section tempSection, Section section)
	{
		CopyInnerWallsElements(tempSection.NorthWalls, section.NorthWalls);
		CopyInnerWallsElements(tempSection.NorthEastWalls, section.NorthEastWalls);
		CopyInnerWallsElements(tempSection.EastWalls, section.EastWalls);
		CopyInnerWallsElements(tempSection.SouthEastWalls, section.SouthEastWalls);
		CopyInnerWallsElements(tempSection.SouthWalls, section.SouthWalls);
		CopyInnerWallsElements(tempSection.SouthWestWalls, section.SouthWestWalls);
		CopyInnerWallsElements(tempSection.WestWalls, section.WestWalls);
		CopyInnerWallsElements(tempSection.NorthWestWalls, section.NorthWestWalls);
	}

	private static void CalculateUInnerWallsSavingESM(Section tempSection, Section section)
	{
		CopyInnerWallsElementsESM(tempSection.NorthWalls, section.NorthWalls);
		CopyInnerWallsElementsESM(tempSection.NorthEastWalls, section.NorthEastWalls);
		CopyInnerWallsElementsESM(tempSection.EastWalls, section.EastWalls);
		CopyInnerWallsElementsESM(tempSection.SouthEastWalls, section.SouthEastWalls);
		CopyInnerWallsElementsESM(tempSection.SouthWalls, section.SouthWalls);
		CopyInnerWallsElementsESM(tempSection.SouthWestWalls, section.SouthWestWalls);
		CopyInnerWallsElementsESM(tempSection.WestWalls, section.WestWalls);
		CopyInnerWallsElementsESM(tempSection.NorthWestWalls, section.NorthWestWalls);
	}

	private static void CopyOuterWallsElements(WallsStates tempWall, WallsStates wall)
	{
		tempWall.Current.OuterU1 = wall.Esm.OuterU1;
		tempWall.Current.OuterU2 = wall.Esm.OuterU2;
		tempWall.Current.OuterU3 = wall.Esm.OuterU3;
		tempWall.Current.OuterU4 = wall.Esm.OuterU4;
		tempWall.Current.OuterU5 = wall.Esm.OuterU5;
		tempWall.Current.OuterU6 = wall.Esm.OuterU6;
		tempWall.Current.AccumulateOuterU = wall.Esm.AccumulateOuterU;
	}

	private static void CopyOuterWallsElementsESM(WallsStates tempWall, WallsStates wall)
	{
		tempWall.Esm.OuterU1 = wall.Current.OuterU1;
		tempWall.Esm.OuterU2 = wall.Current.OuterU2;
		tempWall.Esm.OuterU3 = wall.Current.OuterU3;
		tempWall.Esm.OuterU4 = wall.Current.OuterU4;
		tempWall.Esm.OuterU5 = wall.Current.OuterU5;
		tempWall.Esm.OuterU6 = wall.Current.OuterU6;
		tempWall.Esm.AccumulateOuterU = wall.Current.AccumulateOuterU;
	}

	private static void CopyInnerWallsElements(WallsStates tempWall, WallsStates wall)
	{
		tempWall.Current.InnerU1 = wall.Esm.InnerU1;
		tempWall.Current.InnerU2 = wall.Esm.InnerU2;
		tempWall.Current.InnerU3 = wall.Esm.InnerU3;
		tempWall.Current.InnerU4 = wall.Esm.InnerU4;
		tempWall.Current.InnerU5 = wall.Esm.InnerU5;
		tempWall.Current.InnerU6 = wall.Esm.InnerU6;
		tempWall.Current.AccumulateInnerU = wall.Esm.AccumulateInnerU;
	}

	private static void CopyInnerWallsElementsESM(WallsStates tempWall, WallsStates wall)
	{
		tempWall.Esm.InnerU1 = wall.Current.InnerU1;
		tempWall.Esm.InnerU2 = wall.Current.InnerU2;
		tempWall.Esm.InnerU3 = wall.Current.InnerU3;
		tempWall.Esm.InnerU4 = wall.Current.InnerU4;
		tempWall.Esm.InnerU5 = wall.Current.InnerU5;
		tempWall.Esm.InnerU6 = wall.Current.InnerU6;
		tempWall.Esm.AccumulateInnerU = wall.Current.AccumulateInnerU;
	}

	private static void CalculateUwindowsSavings(Section tempSection, Section section)
	{
		CopyWindowsElements(tempSection.NorthWalls, section.NorthWalls);
		CopyWindowsElements(tempSection.NorthEastWalls, section.NorthEastWalls);
		CopyWindowsElements(tempSection.EastWalls, section.EastWalls);
		CopyWindowsElements(tempSection.SouthEastWalls, section.SouthEastWalls);
		CopyWindowsElements(tempSection.SouthWalls, section.SouthWalls);
		CopyWindowsElements(tempSection.SouthWestWalls, section.SouthWestWalls);
		CopyWindowsElements(tempSection.WestWalls, section.WestWalls);
		CopyWindowsElements(tempSection.NorthWestWalls, section.NorthWestWalls);
	}

	private static void CalculateUwindowsSavingsESM(Section tempSection, Section section)
	{
		CopyWindowsElementsESM(tempSection.NorthWalls, section.NorthWalls);
		CopyWindowsElementsESM(tempSection.NorthEastWalls, section.NorthEastWalls);
		CopyWindowsElementsESM(tempSection.EastWalls, section.EastWalls);
		CopyWindowsElementsESM(tempSection.SouthEastWalls, section.SouthEastWalls);
		CopyWindowsElementsESM(tempSection.SouthWalls, section.SouthWalls);
		CopyWindowsElementsESM(tempSection.SouthWestWalls, section.SouthWestWalls);
		CopyWindowsElementsESM(tempSection.WestWalls, section.WestWalls);
		CopyWindowsElementsESM(tempSection.NorthWestWalls, section.NorthWestWalls);
	}

	private static void CopyWindowsElements(WallsStates tempWall, WallsStates wall)
	{
		tempWall.Current.WindowU1 = wall.Esm.WindowU1;
		tempWall.Current.WindowU2 = wall.Esm.WindowU2;
		tempWall.Current.WindowU3 = wall.Esm.WindowU3;
		tempWall.Current.WindowU4 = wall.Esm.WindowU4;
		tempWall.Current.WindowU5 = wall.Esm.WindowU5;
		tempWall.Current.WindowU6 = wall.Esm.WindowU6;
		tempWall.Current.AccumulateWindowU = tempWall.Esm.AccumulateWindowU;
	}

	private static void CopyWindowsElementsESM(WallsStates tempWall, WallsStates wall)
	{
		tempWall.Esm.WindowU1 = wall.Current.WindowU1;
		tempWall.Esm.WindowU2 = wall.Current.WindowU2;
		tempWall.Esm.WindowU3 = wall.Current.WindowU3;
		tempWall.Esm.WindowU4 = wall.Current.WindowU4;
		tempWall.Esm.WindowU5 = wall.Current.WindowU5;
		tempWall.Esm.WindowU6 = wall.Current.WindowU6;
		tempWall.Esm.AccumulateWindowU = tempWall.Current.AccumulateWindowU;
	}

	private static void CalculateUnonTransparentSavings(Section tempSection, Section section)
	{
		CopyNonTrasparentElements(tempSection.Roof, section.Roof);
	}

	private static void CalculateUnonTransparentSavingsESM(Section tempSection, Section section)
	{
		CopyNonTrasparentElementsESM(tempSection.Roof, section.Roof);
	}

	private static void CopyNonTrasparentElements(RoofStates tempRoof, RoofStates roof)
	{
		tempRoof.Current.NonTransparentU1 = roof.Esm.NonTransparentU1;
		tempRoof.Current.NonTransparentU2 = roof.Esm.NonTransparentU2;
		tempRoof.Current.NonTransparentU3 = roof.Esm.NonTransparentU3;
		tempRoof.Current.NonTransparentU4 = roof.Esm.NonTransparentU4;
		tempRoof.Current.NonTransparentU5 = roof.Esm.NonTransparentU5;
		tempRoof.Current.NonTransparentU6 = roof.Esm.NonTransparentU6;
		tempRoof.Current.NonTransparentU7 = roof.Esm.NonTransparentU7;
		tempRoof.Current.NonTransparentU8 = roof.Esm.NonTransparentU8;
		tempRoof.Current.NonTransparentU9 = roof.Esm.NonTransparentU9;
		tempRoof.Current.AccumulateNonTransparentU = roof.Esm.AccumulateNonTransparentU;
	}

	private static void CopyNonTrasparentElementsESM(RoofStates tempRoof, RoofStates roof)
	{
		tempRoof.Esm.NonTransparentU1 = roof.Current.NonTransparentU1;
		tempRoof.Esm.NonTransparentU2 = roof.Current.NonTransparentU2;
		tempRoof.Esm.NonTransparentU3 = roof.Current.NonTransparentU3;
		tempRoof.Esm.NonTransparentU4 = roof.Current.NonTransparentU4;
		tempRoof.Esm.NonTransparentU5 = roof.Current.NonTransparentU5;
		tempRoof.Esm.NonTransparentU6 = roof.Current.NonTransparentU6;
		tempRoof.Esm.NonTransparentU7 = roof.Current.NonTransparentU7;
		tempRoof.Esm.NonTransparentU8 = roof.Current.NonTransparentU8;
		tempRoof.Esm.NonTransparentU9 = roof.Current.NonTransparentU9;
		tempRoof.Esm.AccumulateNonTransparentU = roof.Current.AccumulateNonTransparentU;
	}

	private static void CalculateUceilingsavings(Section tempSection, Section section)
	{
		CopyCeilingElements(tempSection.Roof, section.Roof);
	}

	private static void CalculateUceilingsavingsESM(Section tempSection, Section section)
	{
		CopyCeilingElementsESM(tempSection.Roof, section.Roof);
	}

	private static void CopyCeilingElements(RoofStates tempRoof, RoofStates roof)
	{
		tempRoof.Current.CeilingU1 = roof.Esm.CeilingU1;
		tempRoof.Current.CeilingU2 = roof.Esm.CeilingU2;
		tempRoof.Current.CeilingU3 = roof.Esm.CeilingU3;
		tempRoof.Current.CeilingU4 = roof.Esm.CeilingU4;
		tempRoof.Current.CeilingU5 = roof.Esm.CeilingU5;
		tempRoof.Current.CeilingU6 = roof.Esm.CeilingU6;
		tempRoof.Current.CeilingU7 = roof.Esm.CeilingU7;
		tempRoof.Current.CeilingU8 = roof.Esm.CeilingU8;
		tempRoof.Current.CeilingU9 = roof.Esm.CeilingU9;
		tempRoof.Current.AccumulateCeilingU = roof.Esm.AccumulateCeilingU;
	}

	private static void CopyCeilingElementsESM(RoofStates tempRoof, RoofStates roof)
	{
		tempRoof.Esm.CeilingU1 = roof.Current.CeilingU1;
		tempRoof.Esm.CeilingU2 = roof.Current.CeilingU2;
		tempRoof.Esm.CeilingU3 = roof.Current.CeilingU3;
		tempRoof.Esm.CeilingU4 = roof.Current.CeilingU4;
		tempRoof.Esm.CeilingU5 = roof.Current.CeilingU5;
		tempRoof.Esm.CeilingU6 = roof.Current.CeilingU6;
		tempRoof.Esm.CeilingU7 = roof.Current.CeilingU7;
		tempRoof.Esm.CeilingU8 = roof.Current.CeilingU8;
		tempRoof.Esm.CeilingU9 = roof.Current.CeilingU9;
		tempRoof.Esm.AccumulateCeilingU = roof.Current.AccumulateCeilingU;
	}

	public static void SetScaleType(Scale investigationMethod, Results buildingResults)
	{
		BuildingScaleTypes buildingScaleType = buildingResults.BuildingScaleType;
		buildingScaleType.PoiterValue = (int)buildingResults.PrimaryEnergyTable.Total.ESM;
		buildingScaleType.PoiterValueBaseLine = (int)buildingResults.PrimaryEnergyTable.Total.BaseLine;
		if (investigationMethod.Type == InvestigationType.ReferentValues)
		{
			double @ref = buildingResults.PrimaryEnergyTable.Total.Ref1;
			double ref2 = buildingResults.PrimaryEnergyTable.Total.Ref2;
			buildingScaleType.AplusValue.Max = (int)(0.25 * ref2);
			buildingScaleType.AValue.Max = (int)(0.5 * ref2);
			buildingScaleType.AValue.Min = (int)(0.25 * ref2);
			buildingScaleType.BValue.Max = (int)ref2;
			buildingScaleType.BValue.Min = (int)(0.5 * ref2 + 1.0);
			buildingScaleType.CValue.Max = (int)(0.5 * (ref2 + @ref));
			buildingScaleType.CValue.Min = (int)ref2 + 1;
			buildingScaleType.DValue.Max = (int)@ref;
			buildingScaleType.DValue.Min = (int)(0.5 * (ref2 + @ref) + 1.0);
			buildingScaleType.EValue.Max = (int)(1.25 * @ref);
			buildingScaleType.EValue.Min = (int)@ref + 1;
			buildingScaleType.FValue.Max = (int)(1.5 * @ref);
			buildingScaleType.FValue.Min = (int)(1.25 * @ref + 1.0);
			buildingScaleType.GValue.Max = (int)(1.5 * @ref);
			buildingScaleType.GValue.Min = (int)(1.5 * @ref);
		}
		else
		{
			buildingScaleType.AplusValue.Max = investigationMethod.Aplus.EPmax;
			buildingScaleType.AValue.Max = investigationMethod.A.EPmax;
			buildingScaleType.AValue.Min = investigationMethod.A.EPmin;
			buildingScaleType.BValue.Max = investigationMethod.B.EPmax;
			buildingScaleType.BValue.Min = investigationMethod.B.EPmin;
			buildingScaleType.CValue.Max = investigationMethod.C.EPmax;
			buildingScaleType.CValue.Min = investigationMethod.C.EPmin;
			buildingScaleType.DValue.Max = investigationMethod.D.EPmax;
			buildingScaleType.DValue.Min = investigationMethod.D.EPmin;
			buildingScaleType.EValue.Max = investigationMethod.E.EPmax;
			buildingScaleType.EValue.Min = investigationMethod.E.EPmin;
			buildingScaleType.FValue.Max = investigationMethod.F.EPmax;
			buildingScaleType.FValue.Min = investigationMethod.F.EPmin;
			buildingScaleType.GValue.Max = investigationMethod.G.EPmax;
			buildingScaleType.GValue.Min = investigationMethod.G.EPmin;
		}
	}

	public static bool CalculateHotWaterNeededPower(this SunEnergyCalculationData sunEnergyCalculationData, Section section, CalculationInput calcInput)
	{
		sunEnergyCalcdata = sunEnergyCalculationData;
		List<double> list = new List<double>();
		List<double> list2 = new List<double>();
		List<double> list3 = new List<double>();
		List<double> list4 = new List<double>();
		List<MonthlyDays> list5 = section.CalcPeriod((int)sunEnergyCalculationData.StartMonth, (int)sunEnergyCalculationData.EndMonth, 1, 31);
		ClearTableValues(sunEnergyCalcdata, section.CalcPeriod(0, 11, 1, 31));
		foreach (MonthlyDays item in list5)
		{
			SunMonth sunMonth = new SunMonth();
			SumCollectorsArea();
			double num = HotWaterNeededPower(sunEnergyCalculationData, item);
			double num2 = HotWaterNeededPowerTotal(sunEnergyCalculationData, item);
			if (Math.Abs(num) < 0.0)
			{
				return false;
			}
			list2.Add(num);
			sunMonth.Qhotwater = num;
			sunMonth.Days = (int)(sunEnergyCalculationData.DaysInWeek * item.Weeks);
			sunMonth.H = SunEnergyPreferencesManager.GetClimateZoneParams(calcInput.General.ClimateZone).SolarRadiation.Months[(int)item.Month].Radiation;
			sunMonth.Tm = SunEnergyPreferencesManager.GetClimateZoneParams(calcInput.General.ClimateZone).SolarRadiation.Months[(int)item.Month].AvgTemp;
			sunMonth.Ht = CalculateParameterHtMonthly(calcInput, item);
			double x = CalculateParameterX(calcInput, item, num2);
			double y = CalculateParameterY(calcInput, item, num2);
			double x2 = CalculateXwithCorrection(x);
			double num3 = CalculateParameterF(x2, y);
			sunMonth.Qsunwater = num3 * num2 / (double)item.TotalDays * (sunEnergyCalculationData.DaysInWeek * item.Weeks);
			list.Add(sunMonth.Qsunwater);
			double num4 = sunMonth.Qsunwater / sunMonth.Qhotwater * 100.0;
			sunMonth.Fm = ((num4 > 100.0) ? 100.0 : num4);
			sunMonth.FmRemain = ((num4 > 100.0) ? (num4 - 100.0) : 0.0);
			list3.Add(sunMonth.Qhotwater * (sunMonth.Fm / 100.0) * ((sunEnergyCalculationData.SerpentineEfficiencyIsUsed ? sunEnergyCalculationData.SerpentineEfficiency : 100.0) / 100.0));
			int num5 = sunMonth.Days * 8;
			list4.Add((double)num5 * sunEnergyCalcdata.PumpsVolume);
			SetTableResults(sunEnergyCalcdata, item.Month, sunMonth);
		}
		double value = list.Aggregate(0.0, (double num10, double item) => num10 + item);
		double num6 = list2.Aggregate(0.0, (double num10, double item) => num10 + item);
		double num7 = list3.Aggregate(0.0, (double num10, double item) => num10 + item);
		double num8 = list4.Aggregate(0.0, (double num10, double item) => num10 + item);
		double d = Math.Round(num7 / num6 * 100.0, 1);
		if (double.IsNaN(d) || double.IsInfinity(d))
		{
			d = 0.0;
		}
		sunEnergyCalcdata.SunEnergyResTable.TotalProportion = d.ToString(CultureInfo.InvariantCulture);
		double d2 = Math.Round(value, 1);
		if (double.IsNaN(d2) || double.IsInfinity(d2))
		{
			d2 = 0.0;
		}
		sunEnergyCalcdata.SunEnergyResTable.TotalSunEnergy = d2.ToString(CultureInfo.InvariantCulture);
		double d3 = Math.Round(num7, 1);
		if (double.IsNaN(d3) || double.IsInfinity(d3))
		{
			d3 = 0.0;
		}
		sunEnergyCalcdata.SunEnergyResTable.BGVSunEnergy = d3.ToString(CultureInfo.InvariantCulture);
		double num9 = Math.Round(num8 * calcInput.General.BuildingResults.TotalAreaElements.TotalHeatedArea / 1000.0, 1);
		if (double.IsNaN(num9) || double.IsInfinity(num9))
		{
			num9 = 0.0;
		}
		sunEnergyCalcdata.SunEnergyResTable.BGVPumpsTotal = num9;
		double totalHeatedArea = calcInput.General.BuildingResults.TotalAreaElements.TotalHeatedArea;
		if (totalHeatedArea <= 0.0 || double.IsInfinity(totalHeatedArea))
		{
			MessageBox.Show("Некоректно попълнени полета за площ!\nМоля проверете \"Входни данни\"");
		}
		else if (!double.IsNaN(num7) && !double.IsInfinity(num7))
		{
			sunEnergyCalcdata.TotalUsedSunEnergy = num7 / totalHeatedArea;
		}
		else
		{
			sunEnergyCalcdata.TotalUsedSunEnergy = 0.0;
		}
		return true;
	}

	private static double CalculateXwithCorrection(double x)
	{
		double acumulatorVolume = sunEnergyCalcdata.AcumulatorVolume;
		double collectorsArea = sunEnergyCalcdata.CollectorsArea;
		double num = acumulatorVolume / collectorsArea;
		if (37.5 < num && num < 300.0)
		{
			num /= 75.0;
			num = Math.Pow(num, -0.25);
			return num * x;
		}
		return x;
	}

	private static void ClearTableValues(SunEnergyCalculationData energyCalcdata, IEnumerable<MonthlyDays> monthsList)
	{
		foreach (MonthlyDays months in monthsList)
		{
			SunEnergyResTable sunEnergyResTable = energyCalcdata.SunEnergyResTable;
			switch (months.Month)
			{
			case Month.January:
				SetNullValues(sunEnergyResTable.January);
				break;
			case Month.February:
				SetNullValues(sunEnergyResTable.February);
				break;
			case Month.March:
				SetNullValues(sunEnergyResTable.March);
				break;
			case Month.April:
				SetNullValues(sunEnergyResTable.April);
				break;
			case Month.May:
				SetNullValues(sunEnergyResTable.May);
				break;
			case Month.June:
				SetNullValues(sunEnergyResTable.June);
				break;
			case Month.July:
				SetNullValues(sunEnergyResTable.July);
				break;
			case Month.August:
				SetNullValues(sunEnergyResTable.August);
				break;
			case Month.September:
				SetNullValues(sunEnergyResTable.September);
				break;
			case Month.October:
				SetNullValues(sunEnergyResTable.October);
				break;
			case Month.November:
				SetNullValues(sunEnergyResTable.November);
				break;
			case Month.December:
				SetNullValues(sunEnergyResTable.December);
				break;
			}
		}
	}

	private static void SetNullValues(SunEnergyResMonth month)
	{
		month.Days = null;
		month.H = null;
		month.Ht = null;
		month.TempM = null;
		month.QhotWater = null;
		month.Fm = null;
		month.QhotWaterSun = null;
		month.FmRemain = null;
	}

	private static void SetTableResults(SunEnergyCalculationData energyCalcdata, Month month, SunMonth sunMonth)
	{
		SunEnergyResTable sunEnergyResTable = energyCalcdata.SunEnergyResTable;
		switch (month)
		{
		case Month.January:
			SetMonthRowValues(sunEnergyResTable.January, sunMonth);
			break;
		case Month.February:
			SetMonthRowValues(sunEnergyResTable.February, sunMonth);
			break;
		case Month.March:
			SetMonthRowValues(sunEnergyResTable.March, sunMonth);
			break;
		case Month.April:
			SetMonthRowValues(sunEnergyResTable.April, sunMonth);
			break;
		case Month.May:
			SetMonthRowValues(sunEnergyResTable.May, sunMonth);
			break;
		case Month.June:
			SetMonthRowValues(sunEnergyResTable.June, sunMonth);
			break;
		case Month.July:
			SetMonthRowValues(sunEnergyResTable.July, sunMonth);
			break;
		case Month.August:
			SetMonthRowValues(sunEnergyResTable.August, sunMonth);
			break;
		case Month.September:
			SetMonthRowValues(sunEnergyResTable.September, sunMonth);
			break;
		case Month.October:
			SetMonthRowValues(sunEnergyResTable.October, sunMonth);
			break;
		case Month.November:
			SetMonthRowValues(sunEnergyResTable.November, sunMonth);
			break;
		case Month.December:
			SetMonthRowValues(sunEnergyResTable.December, sunMonth);
			break;
		}
	}

	private static void SetMonthRowValues(SunEnergyResMonth month, SunMonth sunMonth)
	{
		month.Days = ((sunMonth.Days == 0) ? ((int?)null) : new int?(sunMonth.Days));
		month.H = sunMonth.H;
		month.Ht = sunMonth.Ht;
		month.TempM = sunMonth.Tm;
		month.QhotWater = sunMonth.Qhotwater;
		month.Fm = sunMonth.Fm;
		month.QhotWaterSun = sunMonth.Qsunwater;
		month.QhotWaterSun = sunMonth.Qsunwater;
		month.FmRemain = sunMonth.FmRemain;
	}

	private static void SumCollectorsArea()
	{
		sunEnergyCalcdata.CollectorsArea = sunEnergyCalcdata.AbsorbingSurface * sunEnergyCalcdata.CollectorsCount;
	}

	private static double CalculateParameterF(double x, double y)
	{
		return 1.029 * y - 0.065 * x - 0.245 * Math.Pow(y, 2.0) + 0.0018 * Math.Pow(x, 2.0) + 0.0215 * Math.Pow(y, 3.0);
	}

	private static double CalculateParameterX(CalculationInput calcInput, MonthlyDays month, double neededHotWaterEnergyforMonth)
	{
		double fR = sunEnergyCalcdata.FR;
		double num = CalculateTOAeffect();
		double num2 = 100.0 - PreferencesManager.GetClimateZoneParams(calcInput.General.ClimateZone).SolarRadiation.Months[(int)month.Month].AvgTemp;
		int num3 = month.TotalDays * 24 * 60 * 60;
		double collectorsArea = sunEnergyCalcdata.CollectorsArea;
		double num4 = neededHotWaterEnergyforMonth * 1000.0 / 1.163 * 4187.0;
		double num5 = collectorsArea / num4;
		return fR * num * num2 * (double)num3 * num5;
	}

	private static double CalculateParameterY(CalculationInput calcInput, MonthlyDays month, double neededHotWaterEnergyforMonth)
	{
		double fRta = sunEnergyCalcdata.FRta;
		double num = CalculateTOAeffect();
		double num2 = ((sunEnergyCalcdata.TrasparentCoverings == 1) ? 0.95 : 0.93);
		if (sunEnergyCalcdata.TrasparentCoverings == 2 && (month.Month == Month.June || month.Month == Month.July || month.Month == Month.August))
		{
			num2 = 0.9;
		}
		double num3 = CalculateParameterHtMonthly(calcInput, month);
		int totalDays = month.TotalDays;
		double collectorsArea = sunEnergyCalcdata.CollectorsArea;
		return fRta * num * num2 * num3 * (double)totalDays * (collectorsArea / neededHotWaterEnergyforMonth);
	}

	private static double CalculateTOAeffect()
	{
		double num;
		if (sunEnergyCalcdata.Scheme1Selected || sunEnergyCalcdata.Scheme2Selected)
		{
			num = 1.0;
		}
		else
		{
			double collectorsArea = sunEnergyCalcdata.CollectorsArea;
			double fR = sunEnergyCalcdata.FR;
			double num2 = sunEnergyCalcdata.CollectorDebit * sunEnergyCalcdata.SpecialHeatCapacity;
			double num3 = sunEnergyCalcdata.MTOAEfficiency / 100.0;
			double num4 = sunEnergyCalcdata.MTOADebit * sunEnergyCalcdata.MTOASpecialHeatCapacity;
			double num5 = ((num2 > num4) ? num4 : num2);
			num = 1.0 + collectorsArea * fR / num2 * (num2 / (num3 * num5) - 1.0);
			num = Math.Pow(num, -1.0);
		}
		if (double.IsNaN(num) || double.IsInfinity(num))
		{
			num = 0.0;
		}
		return num;
	}

	private static double HotWaterNeededPower(SunEnergyCalculationData calcData, MonthlyDays month)
	{
		return calcData.WaterUsage * (calcData.HotWaterTemperature - calcData.ColdWaterTemperature) * 1.163 / 1000.0 * (calcData.DaysInWeek * month.Weeks);
	}

	private static double HotWaterNeededPowerTotal(SunEnergyCalculationData calcData, MonthlyDays month)
	{
		return calcData.WaterUsage * (calcData.HotWaterTemperature - calcData.ColdWaterTemperature) * 1.163 / 1000.0 * (double)month.TotalDays;
	}

	private static double DefuseradiationHd(CalculationInput calcInput, MonthlyDays month)
	{
		double cloudiness = SunEnergyPreferencesManager.GetClimateZoneParams(calcInput.General.ClimateZone).SolarRadiation.Months[(int)month.Month].Cloudiness;
		return 1.39 - 4.03 * cloudiness + 5.53 * Math.Pow(cloudiness, 2.0) - 3.11 * Math.Pow(cloudiness, 3.0);
	}

	private static double SunDeclination(Month month)
	{
		int num = 21;
		switch (month)
		{
		case Month.January:
			num = 21;
			break;
		case Month.February:
			num = 52;
			break;
		case Month.March:
			num = 80;
			break;
		case Month.April:
			num = 111;
			break;
		case Month.May:
			num = 141;
			break;
		case Month.June:
			num = 172;
			break;
		case Month.July:
			num = 202;
			break;
		case Month.August:
			num = 233;
			break;
		case Month.September:
			num = 264;
			break;
		case Month.October:
			num = 294;
			break;
		case Month.November:
			num = 325;
			break;
		case Month.December:
			num = 355;
			break;
		}
		return 23.45 * Math.Sin(Math.PI * 2.0 * (double)(284 + num) / 365.0);
	}

	private static double SunsetHour(Month month)
	{
		double num = SunDeclination(month);
		return Math.Acos((0.0 - Math.Tan(0.7382742735936013)) * Math.Tan(num * (Math.PI / 180.0))) * 180.0 / Math.PI;
	}

	private static double SunsetHourPrim(Month month)
	{
		double num = SunDeclination(month);
		double num2 = SunsetHour(month);
		double pitch = sunEnergyCalcdata.Pitch;
		double num3 = SubAngles(42.3, pitch);
		double num4 = Math.Acos(0.0 - Math.Tan(num3 * (Math.PI / 180.0)) * Math.Tan(num * (Math.PI / 180.0))) * 180.0 / Math.PI;
		return (num2 < num4) ? num2 : num4;
	}

	private static double CalculateMonthlyHorizontalRadiation(Month month)
	{
		double pitch = sunEnergyCalcdata.Pitch;
		double num = SubAngles(42.3, pitch);
		double num2 = SunDeclination(month);
		double num3 = SunsetHour(month);
		double num4 = SunsetHourPrim(month);
		double num5 = Math.Cos(num * (Math.PI / 180.0)) * Math.Cos(num2 * (Math.PI / 180.0)) * Math.Sin(num4 * (Math.PI / 180.0)) + Math.PI / 180.0 * num4 * Math.Sin(Math.PI / 180.0 * num) * Math.Sin(Math.PI / 180.0 * num2);
		double num6 = Math.Cos(0.7382742735936013) * Math.Cos(Math.PI / 180.0 * num2) * Math.Sin(num3 * (Math.PI / 180.0)) + Math.PI / 180.0 * num3 * Math.Sin(0.7382742735936013) * Math.Sin(Math.PI / 180.0 * num2);
		return num5 / num6;
	}

	private static double SubAngles(double anglefi, double angleBeta)
	{
		return anglefi - angleBeta;
	}

	private static double CalculateProjectionCoeficient(CalculationInput calcInput, MonthlyDays month)
	{
		double num = DefuseradiationHd(calcInput, month);
		double num2 = CalculateMonthlyHorizontalRadiation(month.Month);
		double pitch = sunEnergyCalcdata.Pitch;
		double impactEnvironment = sunEnergyCalcdata.ImpactEnvironment;
		return (1.0 - num) * num2 + num * ((1.0 + Math.Cos(Math.PI / 180.0 * pitch)) / 2.0) + impactEnvironment * ((1.0 - Math.Cos(Math.PI / 180.0 * pitch)) / 2.0);
	}

	private static double CalculateParameterHtMonthly(CalculationInput calcInput, MonthlyDays month)
	{
		return CalculateProjectionCoeficient(calcInput, month) * SunEnergyPreferencesManager.GetClimateZoneParams(calcInput.General.ClimateZone).SolarRadiation.Months[(int)month.Month].Radiation;
	}

	public static void VentilationCoolEnergyRef1(this CalculationData calcData, Section section, CalculationInput calcInput, CoolingCalculations ventCoolingCalculations)
	{
		List<double> list = new List<double>();
		List<double> list2 = new List<double>();
		List<double> list3 = new List<double>();
		List<double> list4 = new List<double>();
		List<MonthlyDays> list5 = section.CalcPeriod((int)section.CoolingSeason.FirstMonthCool, (int)section.CoolingSeason.LastMonthCool, section.CoolingSeason.FirstDayCool, section.CoolingSeason.LastDayCool);
		foreach (MonthlyDays item in list5)
		{
			CalculateMontlyCoolEnergyRef1(section, calcInput, ventCoolingCalculations, item, out var powHeating, out var powCooling);
			if (!double.IsNaN(powCooling))
			{
				list.Add(powCooling);
			}
			if (!double.IsNaN(powHeating))
			{
				list2.Add(powHeating);
			}
			list4.Add(CalculateWitheringEnergyRef1(section, calcInput, ventCoolingCalculations, item));
			list3.Add(CalculateCoolingInputsRef1(section, ventCoolingCalculations.CoolingResult, ventCoolingCalculations.VentilationCooling, item));
		}
		calcData.ResulCoolingInputsRef1 = list3.Aggregate(0.0, (double num, double item) => num + item);
		ventCoolingCalculations.CoolingResult.ResulVentilationInputsRef1 = calcData.ResulCoolingInputsRef1;
		calcData.ResultEnergyForCoolingRef1 = list.Aggregate(0.0, (double num, double item) => num + item);
		calcData.ResultEnergyForHeatingRef1 = list2.Aggregate(0.0, (double num, double item) => num + item);
		calcData.ResultEnergyForWitheringRef1 = list4.Aggregate(0.0, (double num, double item) => num + item);
	}

	public static void VentilationCoolEnergyRef2(this CalculationData calcData, Section section, CalculationInput calcInput, CoolingCalculations ventCoolingCalculations)
	{
		List<double> list = new List<double>();
		List<double> list2 = new List<double>();
		List<double> list3 = new List<double>();
		List<double> list4 = new List<double>();
		List<MonthlyDays> list5 = section.CalcPeriod((int)section.CoolingSeason.FirstMonthCool, (int)section.CoolingSeason.LastMonthCool, section.CoolingSeason.FirstDayCool, section.CoolingSeason.LastDayCool);
		foreach (MonthlyDays item in list5)
		{
			CalculateMontlyCoolEnergyRef2(section, calcInput, ventCoolingCalculations, item, out var powHeating, out var powCooling);
			if (!double.IsNaN(powCooling))
			{
				list.Add(powCooling);
			}
			if (!double.IsNaN(powHeating))
			{
				list2.Add(powHeating);
			}
			list4.Add(CalculateWitheringEnergyRef2(section, calcInput, ventCoolingCalculations, item));
			list3.Add(CalculateCoolingInputsRef2(section, ventCoolingCalculations.CoolingResult, ventCoolingCalculations.VentilationCooling, item));
		}
		calcData.ResulCoolingInputsRef2 = list3.Aggregate(0.0, (double num, double item) => num + item);
		ventCoolingCalculations.CoolingResult.ResulVentilationInputsRef2 = calcData.ResulCoolingInputsRef2;
		calcData.ResultEnergyForCoolingRef2 = list.Aggregate(0.0, (double num, double item) => num + item);
		calcData.ResultEnergyForHeatingRef2 = list2.Aggregate(0.0, (double num, double item) => num + item);
		calcData.ResultEnergyForWitheringRef2 = list4.Aggregate(0.0, (double num, double item) => num + item);
	}

	public static void VentilationCoolEnergyActual(this CalculationData calcData, Section section, CalculationInput calcInput, CoolingCalculations ventCoolingCalculations)
	{
		List<double> list = new List<double>();
		List<double> list2 = new List<double>();
		List<double> list3 = new List<double>();
		List<double> list4 = new List<double>();
		List<MonthlyDays> list5 = section.CalcPeriod((int)section.CoolingSeason.FirstMonthCool, (int)section.CoolingSeason.LastMonthCool, section.CoolingSeason.FirstDayCool, section.CoolingSeason.LastDayCool);
		foreach (MonthlyDays item in list5)
		{
			CalculateMontlyCoolEnergyActual(section, calcInput, ventCoolingCalculations, item, out var powHeating, out var powCooling);
			if (!double.IsNaN(powCooling))
			{
				list.Add(powCooling);
			}
			if (!double.IsNaN(powHeating))
			{
				list2.Add(powHeating);
			}
			list4.Add(CalculateWitheringEnergyActual(section, calcInput, ventCoolingCalculations, item));
			list3.Add(CalculateCoolingInputs(section, ventCoolingCalculations.CoolingResult, ventCoolingCalculations.VentilationCooling, item));
		}
		calcData.ResulCoolingInputsActual = list3.Aggregate(0.0, (double num, double item) => num + item);
		ventCoolingCalculations.CoolingResult.ResulVentilationInputsActual = calcData.ResulCoolingInputsActual;
		calcData.ResultEnergyForCoolingActual = list.Aggregate(0.0, (double num, double item) => num + item);
		calcData.ResultEnergyForHeatingActual = list2.Aggregate(0.0, (double num, double item) => num + item);
		calcData.ResultEnergyForWitheringActual = list4.Aggregate(0.0, (double num, double item) => num + item);
	}

	public static void VentilationCoolEnergyBaseLine(this CalculationData calcData, Section section, CalculationInput calcInput, CoolingCalculations ventCoolingCalculations)
	{
		List<double> list = new List<double>();
		List<double> list2 = new List<double>();
		List<double> list3 = new List<double>();
		List<double> list4 = new List<double>();
		List<MonthlyDays> list5 = section.CalcPeriod((int)section.CoolingSeason.FirstMonthCool, (int)section.CoolingSeason.LastMonthCool, section.CoolingSeason.FirstDayCool, section.CoolingSeason.LastDayCool);
		foreach (MonthlyDays item in list5)
		{
			CalculateMontlyCoolEnergyBaseLine(section, calcInput, calcData, item, out var powHeating, out var powCooling);
			if (!double.IsNaN(powCooling))
			{
				list.Add(powCooling);
			}
			if (!double.IsNaN(powHeating))
			{
				list2.Add(powHeating);
			}
			list4.Add(CalculateWitheringEnergyBaseLine(section, calcInput, calcData, item));
			list3.Add(CalculateCoolingInputsBaseLine(section, ventCoolingCalculations.CoolingResult, ventCoolingCalculations.VentilationCooling, item));
		}
		calcData.ResulCoolingInputsBaseLine = list3.Aggregate(0.0, (double num, double item) => num + item);
		ventCoolingCalculations.CoolingResult.ResulVentilationInputsBaseLine = calcData.ResulCoolingInputsBaseLine;
		calcData.ResultEnergyForCoolingBaseLine = list.Aggregate(0.0, (double num, double item) => num + item);
		calcData.ResultEnergyForHeatingBaseLine = list2.Aggregate(0.0, (double num, double item) => num + item);
		calcData.ResultEnergyForWitheringBaseLine = list4.Aggregate(0.0, (double num, double item) => num + item);
	}

	public static void VentilationCoolEnergyEsm(this CalculationData calcData, Section section, CalculationInput calcInput, CoolingCalculations ventCoolingCalculations)
	{
		List<double> list = new List<double>();
		List<double> list2 = new List<double>();
		List<double> list3 = new List<double>();
		List<double> list4 = new List<double>();
		List<MonthlyDays> list5 = section.CalcPeriod((int)section.CoolingSeason.FirstMonthCool, (int)section.CoolingSeason.LastMonthCool, section.CoolingSeason.FirstDayCool, section.CoolingSeason.LastDayCool);
		foreach (MonthlyDays item in list5)
		{
			CalculateMontlyCoolEnergyESM(section, calcInput, ventCoolingCalculations, item, out var powHeating, out var powCooling);
			if (!double.IsNaN(powCooling))
			{
				list.Add(powCooling);
			}
			if (!double.IsNaN(powHeating))
			{
				list2.Add(powHeating);
			}
			list4.Add(CalculateWitheringEnergyESM(section, calcInput, ventCoolingCalculations, item));
			list3.Add(CalculateCoolingInputsESM(section, ventCoolingCalculations.CoolingResult, ventCoolingCalculations.VentilationCooling, item));
		}
		calcData.ResulCoolingInputsESM = list3.Aggregate(0.0, (double num, double item) => num + item);
		ventCoolingCalculations.CoolingResult.ResulVentilationInputsESM = calcData.ResulCoolingInputsESM;
		calcData.ResultEnergyForCoolingESM = list.Aggregate(0.0, (double num, double item) => num + item);
		calcData.ResultEnergyForHeatingESM = list2.Aggregate(0.0, (double num, double item) => num + item);
		calcData.ResultEnergyForWitheringESM = list4.Aggregate(0.0, (double num, double item) => num + item);
	}

	public static void GetWeekHoursCoolingReferences(this CalculationData coolingCalc, Section section)
	{
		double num = 5.0 * section.CalcHours(section.CoolingSeasons.Ventilation.WorkBaseStart, section.CoolingSeasons.Ventilation.WorkBaseEnd);
		num += section.CalcHours(section.CoolingSeasons.Ventilation.SunBaseStart, section.CoolingSeasons.Ventilation.SunBaseEnd);
		num = (coolingCalc.WorkingScheduleRef = num + section.CalcHours(section.CoolingSeasons.Ventilation.SatBaseStart, section.CoolingSeasons.Ventilation.SatBaseEnd));
		coolingCalc.WorkingScheduleRef2 = num;
	}

	public static void GetWeekHoursCoolingActual(this CalculationData coolingCalc, Section section)
	{
		double num = 5.0 * section.CalcHours(section.CoolingSeasons.Ventilation.WorkCurrentStart, section.CoolingSeasons.Ventilation.WorkCurrentEnd);
		num += section.CalcHours(section.CoolingSeasons.Ventilation.SunCurrentStart, section.CoolingSeasons.Ventilation.SunCurrentEnd);
		num += section.CalcHours(section.CoolingSeasons.Ventilation.SatCurrentStart, section.CoolingSeasons.Ventilation.SatCurrentEnd);
		coolingCalc.WorkingScheduleActual = num;
	}

	public static void GetWeekHoursCoolingBaseLine(this CalculationData coolingCalc, Section section)
	{
		double num = 5.0 * section.CalcHours(section.CoolingSeasons.Ventilation.WorkBaseStart, section.CoolingSeasons.Ventilation.WorkBaseEnd);
		num += section.CalcHours(section.CoolingSeasons.Ventilation.SunBaseStart, section.CoolingSeasons.Ventilation.SunBaseEnd);
		num += section.CalcHours(section.CoolingSeasons.Ventilation.SatBaseStart, section.CoolingSeasons.Ventilation.SatBaseEnd);
		coolingCalc.WorkingScheduleBaseLine = num;
	}

	public static void GetWeekHoursCoolingEsm(this CalculationData coolingCalc, Section section)
	{
		double num = 5.0 * section.CalcHours(section.CoolingSeasons.Ventilation.WorkEsmStart, section.CoolingSeasons.Ventilation.WorkEsmEnd);
		num += section.CalcHours(section.CoolingSeasons.Ventilation.SunEsmStart, section.CoolingSeasons.Ventilation.SunEsmEnd);
		num += section.CalcHours(section.CoolingSeasons.Ventilation.SatEsmStart, section.CoolingSeasons.Ventilation.SatEsmEnd);
		coolingCalc.WorkingScheduleESM = num;
	}

	public static void CalculateVentCoolNeededEnergyRef1(this CalculationData coolCalc)
	{
		double resultEnergyForCoolingRef = coolCalc.ResultEnergyForCoolingRef1;
		double num = resultEnergyForCoolingRef * coolCalc.Part1Ref1 / 100.0;
		coolCalc.ResultSourceEnergyRef1 = num / (coolCalc.TransmitTempEfficiencyRef1 / 100.0 * (coolCalc.SupplyNetEfficiencyRef1 / 100.0) * (coolCalc.AutomaticRef1 / 100.0) * (coolCalc.EnergyManagementRef1 / 100.0) * (coolCalc.GeneratorColdEfficiency1Ref1 / 100.0));
		if (double.IsInfinity(coolCalc.ResultSourceEnergyRef1) || double.IsNaN(coolCalc.ResultSourceEnergyRef1))
		{
			coolCalc.ResultSourceEnergyRef1 = 0.0;
		}
		double resultEnergyForCoolingRef2 = coolCalc.ResultEnergyForCoolingRef1;
		double num2 = resultEnergyForCoolingRef2 * coolCalc.Part2Ref1 / 100.0;
		coolCalc.ResultSourceEnergy2Ref1 = num2 / (coolCalc.TransmitTempEfficiency2Ref1 / 100.0 * (coolCalc.SupplyNetEfficiency2Ref1 / 100.0) * (coolCalc.Automatic2Ref1 / 100.0) * (coolCalc.EnergyManagement2Ref1 / 100.0) * (coolCalc.GeneratorColdEfficiency2Ref1 / 100.0));
		if (double.IsInfinity(coolCalc.ResultSourceEnergy2Ref1) || double.IsNaN(coolCalc.ResultSourceEnergy2Ref1))
		{
			coolCalc.ResultSourceEnergy2Ref1 = 0.0;
		}
		coolCalc.ResultNeededEnergyRef1 = coolCalc.ResultSourceEnergyRef1 + coolCalc.ResultSourceEnergy2Ref1;
	}

	public static void CalculateVentCoolNeededEnergyRef2(this CalculationData coolCalc)
	{
		double resultEnergyForCoolingRef = coolCalc.ResultEnergyForCoolingRef2;
		double num = resultEnergyForCoolingRef * coolCalc.Part1Ref2 / 100.0;
		coolCalc.ResultSourceEnergyRef2 = num / (coolCalc.TransmitTempEfficiencyRef2 / 100.0 * (coolCalc.SupplyNetEfficiencyRef2 / 100.0) * (coolCalc.AutomaticRef2 / 100.0) * (coolCalc.EnergyManagementRef2 / 100.0) * (coolCalc.GeneratorColdEfficiency1Ref2 / 100.0));
		if (double.IsInfinity(coolCalc.ResultSourceEnergyRef2) || double.IsNaN(coolCalc.ResultSourceEnergyRef2))
		{
			coolCalc.ResultSourceEnergyRef2 = 0.0;
		}
		double resultEnergyForCoolingRef2 = coolCalc.ResultEnergyForCoolingRef2;
		double num2 = resultEnergyForCoolingRef2 * coolCalc.Part2Ref2 / 100.0;
		coolCalc.ResultSourceEnergy2Ref2 = num2 / (coolCalc.TransmitTempEfficiency2Ref2 / 100.0 * (coolCalc.SupplyNetEfficiency2Ref2 / 100.0) * (coolCalc.Automatic2Ref2 / 100.0) * (coolCalc.EnergyManagement2Ref2 / 100.0) * (coolCalc.GeneratorColdEfficiency2Ref2 / 100.0));
		if (double.IsInfinity(coolCalc.ResultSourceEnergy2Ref2) || double.IsNaN(coolCalc.ResultSourceEnergy2Ref2))
		{
			coolCalc.ResultSourceEnergy2Ref2 = 0.0;
		}
		coolCalc.ResultNeededEnergyRef2 = coolCalc.ResultSourceEnergyRef2 + coolCalc.ResultSourceEnergy2Ref2;
	}

	public static void CalculateVentCoolNeededEnergyActual(this CalculationData coolCalc)
	{
		double resultEnergyForCoolingActual = coolCalc.ResultEnergyForCoolingActual;
		double num = resultEnergyForCoolingActual * coolCalc.Part1Actual / 100.0;
		coolCalc.ResultSourceEnergyActual = num / (coolCalc.TransmitTempEfficiencyActual / 100.0 * (coolCalc.SupplyNetEfficiencyActual / 100.0) * (coolCalc.AutomaticActual / 100.0) * (coolCalc.EnergyManagementActual / 100.0) * (coolCalc.GeneratorColdEfficiency1Actual / 100.0));
		if (double.IsInfinity(coolCalc.ResultSourceEnergyActual) || double.IsNaN(coolCalc.ResultSourceEnergyActual))
		{
			coolCalc.ResultSourceEnergyActual = 0.0;
		}
		double resultEnergyForCoolingActual2 = coolCalc.ResultEnergyForCoolingActual;
		double num2 = resultEnergyForCoolingActual2 * coolCalc.Part2Actual / 100.0;
		coolCalc.ResultSourceEnergy2Actual = num2 / (coolCalc.TransmitTempEfficiency2Actual / 100.0 * (coolCalc.SupplyNetEfficiency2Actual / 100.0) * (coolCalc.Automatic2Actual / 100.0) * (coolCalc.EnergyManagement2Actual / 100.0) * (coolCalc.GeneratorColdEfficiency2Actual / 100.0));
		if (double.IsInfinity(coolCalc.ResultSourceEnergy2Actual) || double.IsNaN(coolCalc.ResultSourceEnergy2Actual))
		{
			coolCalc.ResultSourceEnergy2Actual = 0.0;
		}
		coolCalc.ResultNeededEnergyActual = coolCalc.ResultSourceEnergyActual + coolCalc.ResultSourceEnergy2Actual;
	}

	public static void CalculateVentCoolNeededEnergyBaseLine(this CalculationData coolCalc)
	{
		double resultEnergyForCoolingBaseLine = coolCalc.ResultEnergyForCoolingBaseLine;
		double num = resultEnergyForCoolingBaseLine * coolCalc.Part1BaseLine / 100.0;
		coolCalc.ResultSourceEnergyBaseLine = num / (coolCalc.TransmitTempEfficiencyBaseLine / 100.0 * (coolCalc.SupplyNetEfficiencyBaseLine / 100.0) * (coolCalc.AutomaticBaseLine / 100.0) * (coolCalc.EnergyManagementBaseLine / 100.0) * (coolCalc.GeneratorColdEfficiency1BaseLine / 100.0));
		if (double.IsInfinity(coolCalc.ResultSourceEnergyBaseLine) || double.IsNaN(coolCalc.ResultSourceEnergyBaseLine))
		{
			coolCalc.ResultSourceEnergyBaseLine = 0.0;
		}
		double resultEnergyForCoolingBaseLine2 = coolCalc.ResultEnergyForCoolingBaseLine;
		double num2 = resultEnergyForCoolingBaseLine2 * coolCalc.Part2BaseLine / 100.0;
		coolCalc.ResultSourceEnergy2BaseLine = num2 / (coolCalc.TransmitTempEfficiency2BaseLine / 100.0 * (coolCalc.SupplyNetEfficiency2BaseLine / 100.0) * (coolCalc.Automatic2BaseLine / 100.0) * (coolCalc.EnergyManagement2BaseLine / 100.0) * (coolCalc.GeneratorColdEfficiency2BaseLine / 100.0));
		if (double.IsInfinity(coolCalc.ResultSourceEnergy2BaseLine) || double.IsNaN(coolCalc.ResultSourceEnergy2BaseLine))
		{
			coolCalc.ResultSourceEnergy2BaseLine = 0.0;
		}
		coolCalc.ResultNeededEnergyBaseLine = coolCalc.ResultSourceEnergyBaseLine + coolCalc.ResultSourceEnergy2BaseLine;
	}

	public static void CalculateVentCoolNeededEnergyEsm(this CalculationData coolCalc)
	{
		double resultEnergyForCoolingESM = coolCalc.ResultEnergyForCoolingESM;
		double num = resultEnergyForCoolingESM * coolCalc.Part1ESM / 100.0;
		coolCalc.ResultSourceEnergyESM = num / (coolCalc.TransmitTempEfficiencyESM / 100.0 * (coolCalc.SupplyNetEfficiencyESM / 100.0) * (coolCalc.AutomaticESM / 100.0) * (coolCalc.EnergyManagementESM / 100.0) * (coolCalc.GeneratorColdEfficiency1ESM / 100.0));
		if (double.IsInfinity(coolCalc.ResultSourceEnergyESM) || double.IsNaN(coolCalc.ResultSourceEnergyESM))
		{
			coolCalc.ResultSourceEnergyESM = 0.0;
		}
		double resultEnergyForCoolingESM2 = coolCalc.ResultEnergyForCoolingESM;
		double num2 = resultEnergyForCoolingESM2 * coolCalc.Part2ESM / 100.0;
		coolCalc.ResultSourceEnergy2ESM = num2 / (coolCalc.TransmitTempEfficiency2ESM / 100.0 * (coolCalc.SupplyNetEfficiency2ESM / 100.0) * (coolCalc.Automatic2ESM / 100.0) * (coolCalc.EnergyManagement2ESM / 100.0) * (coolCalc.GeneratorColdEfficiency2ESM / 100.0));
		if (double.IsInfinity(coolCalc.ResultSourceEnergy2ESM) || double.IsNaN(coolCalc.ResultSourceEnergy2ESM))
		{
			coolCalc.ResultSourceEnergy2ESM = 0.0;
		}
		coolCalc.ResultNeededEnergyESM = coolCalc.ResultSourceEnergyESM + coolCalc.ResultSourceEnergy2ESM;
		coolCalc.ResultNeededEnergySavings = (coolCalc.ResultNeededEnergyBaseLine - coolCalc.ResultNeededEnergyESM).ToString("F3");
	}

	private static double CalculateCoolingInputsRef1(Section section, CalculationData calcData, CalculationData ventCool, MonthlyDays month)
	{
		double num = 0.0;
		for (int i = section.CoolingSeasons.Ventilation.WorkCurrentStart; i < section.CoolingSeasons.Ventilation.WorkCurrentEnd; i++)
		{
			double num2 = ((i >= section.CoolingSeasons.Cooling.WorkCurrentStart && i < section.CoolingSeasons.Cooling.WorkCurrentEnd) ? calcData.ProjectTemperatureRef1 : calcData.NonProjectTemperatureRef1);
			num += ventCool.DebitRef1 * 0.34 * (num2 - ventCool.FlowTemperatureRef1) / 1000.0;
		}
		double num3 = num * (double)month.WorkDays;
		double num4 = 0.0;
		for (int j = section.CoolingSeasons.Ventilation.SatCurrentStart; j < section.CoolingSeasons.Ventilation.SatCurrentEnd; j++)
		{
			double num5 = ((j >= section.CoolingSeasons.Cooling.SatCurrentStart && j <= section.CoolingSeasons.Cooling.SatCurrentEnd) ? calcData.ProjectTemperatureRef1 : calcData.NonProjectTemperatureRef1);
			num4 += ventCool.DebitRef1 * 0.34 * (num5 - ventCool.FlowTemperatureRef1) / 1000.0;
		}
		double num6 = num4 * (double)month.Saturdays;
		double num7 = 0.0;
		for (int k = section.CoolingSeasons.Ventilation.SunCurrentStart; k < section.CoolingSeasons.Ventilation.SunCurrentEnd; k++)
		{
			double num8 = ((k >= section.CoolingSeasons.Cooling.SunCurrentStart && k <= section.CoolingSeasons.Cooling.SunCurrentEnd) ? calcData.ProjectTemperatureRef1 : calcData.NonProjectTemperatureRef1);
			num7 += ventCool.DebitRef1 * 0.34 * (num8 - ventCool.FlowTemperatureRef1) / 1000.0;
		}
		double num9 = num7 * (double)month.Sundays;
		return num3 + num6 + num9;
	}

	private static double CalculateCoolingInputsRef2(Section section, CalculationData calcData, CalculationData ventCool, MonthlyDays month)
	{
		double num = 0.0;
		for (int i = section.CoolingSeasons.Ventilation.WorkCurrentStart; i < section.CoolingSeasons.Ventilation.WorkCurrentEnd; i++)
		{
			double num2 = ((i >= section.CoolingSeasons.Cooling.WorkCurrentStart && i < section.CoolingSeasons.Cooling.WorkCurrentEnd) ? calcData.ProjectTemperatureRef2 : calcData.NonProjectTemperatureRef2);
			num += ventCool.DebitRef2 * 0.34 * (num2 - ventCool.FlowTemperatureRef2) / 1000.0;
		}
		double num3 = num * (double)month.WorkDays;
		double num4 = 0.0;
		for (int j = section.CoolingSeasons.Ventilation.SatCurrentStart; j < section.CoolingSeasons.Ventilation.SatCurrentEnd; j++)
		{
			double num5 = ((j >= section.CoolingSeasons.Cooling.SatCurrentStart && j <= section.CoolingSeasons.Cooling.SatCurrentEnd) ? calcData.ProjectTemperatureRef2 : calcData.NonProjectTemperatureRef2);
			num4 += ventCool.DebitRef2 * 0.34 * (num5 - ventCool.FlowTemperatureRef2) / 1000.0;
		}
		double num6 = num4 * (double)month.Saturdays;
		double num7 = 0.0;
		for (int k = section.CoolingSeasons.Ventilation.SunCurrentStart; k < section.CoolingSeasons.Ventilation.SunCurrentEnd; k++)
		{
			double num8 = ((k >= section.CoolingSeasons.Cooling.SunCurrentStart && k <= section.CoolingSeasons.Cooling.SunCurrentEnd) ? calcData.ProjectTemperatureRef2 : calcData.NonProjectTemperatureRef2);
			num7 += ventCool.DebitRef2 * 0.34 * (num8 - ventCool.FlowTemperatureRef2) / 1000.0;
		}
		double num9 = num7 * (double)month.Sundays;
		return num3 + num6 + num9;
	}

	private static double CalculateCoolingInputs(Section section, CalculationData calcData, CalculationData ventCool, MonthlyDays month)
	{
		double num = 0.0;
		for (int i = section.CoolingSeasons.Ventilation.WorkCurrentStart; i < section.CoolingSeasons.Ventilation.WorkCurrentEnd; i++)
		{
			double num2 = ((i >= section.CoolingSeasons.Cooling.WorkCurrentStart && i < section.CoolingSeasons.Cooling.WorkCurrentEnd) ? calcData.ProjectTemperatureActual : calcData.NonProjectTemperatureActual);
			num += ventCool.DebitActual * 0.34 * (num2 - ventCool.FlowTemperatureActual) / 1000.0;
		}
		double num3 = num * (double)month.WorkDays;
		double num4 = 0.0;
		for (int j = section.CoolingSeasons.Ventilation.SatCurrentStart; j < section.CoolingSeasons.Ventilation.SatCurrentEnd; j++)
		{
			double num5 = ((j >= section.CoolingSeasons.Cooling.SatCurrentStart && j <= section.CoolingSeasons.Cooling.SatCurrentEnd) ? calcData.ProjectTemperatureActual : calcData.NonProjectTemperatureActual);
			num4 += ventCool.DebitActual * 0.34 * (num5 - ventCool.FlowTemperatureActual) / 1000.0;
		}
		double num6 = num4 * (double)month.Saturdays;
		double num7 = 0.0;
		for (int k = section.CoolingSeasons.Ventilation.SunCurrentStart; k < section.CoolingSeasons.Ventilation.SunCurrentEnd; k++)
		{
			double num8 = ((k >= section.CoolingSeasons.Cooling.SunCurrentStart && k <= section.CoolingSeasons.Cooling.SunCurrentEnd) ? calcData.ProjectTemperatureActual : calcData.NonProjectTemperatureActual);
			num7 += ventCool.DebitActual * 0.34 * (num8 - ventCool.FlowTemperatureActual) / 1000.0;
		}
		double num9 = num7 * (double)month.Sundays;
		return num3 + num6 + num9;
	}

	private static double CalculateCoolingInputsBaseLine(Section section, CalculationData calcData, CalculationData ventCool, MonthlyDays month)
	{
		double num = 0.0;
		for (int i = section.CoolingSeasons.Ventilation.WorkBaseStart; i < section.CoolingSeasons.Ventilation.WorkBaseEnd; i++)
		{
			double num2 = ((i >= section.CoolingSeasons.Cooling.WorkBaseStart && i < section.CoolingSeasons.Cooling.WorkBaseEnd) ? calcData.ProjectTemperatureBaseLine : calcData.NonProjectTemperatureBaseLine);
			num += ventCool.DebitBaseLine * 0.34 * (num2 - ventCool.FlowTemperatureBaseLine) / 1000.0;
		}
		double num3 = num * (double)month.WorkDays;
		double num4 = 0.0;
		for (int j = section.CoolingSeasons.Ventilation.SatBaseStart; j < section.CoolingSeasons.Ventilation.SatBaseEnd; j++)
		{
			double num5 = ((j >= section.CoolingSeasons.Cooling.SatBaseStart && j <= section.CoolingSeasons.Cooling.SatBaseEnd) ? calcData.ProjectTemperatureBaseLine : calcData.NonProjectTemperatureBaseLine);
			num4 += ventCool.DebitBaseLine * 0.34 * (num5 - ventCool.FlowTemperatureBaseLine) / 1000.0;
		}
		double num6 = num4 * (double)month.Saturdays;
		double num7 = 0.0;
		for (int k = section.CoolingSeasons.Ventilation.SunBaseStart; k < section.CoolingSeasons.Ventilation.SunBaseEnd; k++)
		{
			double num8 = ((k >= section.CoolingSeasons.Cooling.SunBaseStart && k <= section.CoolingSeasons.Cooling.SunBaseEnd) ? calcData.ProjectTemperatureBaseLine : calcData.NonProjectTemperatureBaseLine);
			num7 += ventCool.DebitBaseLine * 0.34 * (num8 - ventCool.FlowTemperatureBaseLine) / 1000.0;
		}
		double num9 = num7 * (double)month.Sundays;
		return num3 + num6 + num9;
	}

	private static double CalculateCoolingInputsESM(Section section, CalculationData calcData, CalculationData ventCool, MonthlyDays month)
	{
		double num = 0.0;
		for (int i = section.CoolingSeasons.Ventilation.WorkEsmStart; i < section.CoolingSeasons.Ventilation.WorkEsmEnd; i++)
		{
			double num2 = ((i >= section.CoolingSeasons.Cooling.WorkEsmStart && i < section.CoolingSeasons.Cooling.WorkEsmEnd) ? calcData.ProjectTemperatureESM : calcData.NonProjectTemperatureESM);
			num += ventCool.DebitESM * 0.34 * (num2 - ventCool.FlowTemperatureESM) / 1000.0;
		}
		double num3 = num * (double)month.WorkDays;
		double num4 = 0.0;
		for (int j = section.CoolingSeasons.Ventilation.SatEsmStart; j < section.CoolingSeasons.Ventilation.SatEsmEnd; j++)
		{
			double num5 = ((j >= section.CoolingSeasons.Cooling.SatEsmStart && j <= section.CoolingSeasons.Cooling.SatEsmEnd) ? calcData.ProjectTemperatureESM : calcData.NonProjectTemperatureESM);
			num4 += ventCool.DebitESM * 0.34 * (num5 - ventCool.FlowTemperatureESM) / 1000.0;
		}
		double num6 = num4 * (double)month.Saturdays;
		double num7 = 0.0;
		for (int k = section.CoolingSeasons.Ventilation.SunEsmStart; k < section.CoolingSeasons.Ventilation.SunEsmEnd; k++)
		{
			double num8 = ((k >= section.CoolingSeasons.Cooling.SunEsmStart && k <= section.CoolingSeasons.Cooling.SunEsmEnd) ? calcData.ProjectTemperatureESM : calcData.NonProjectTemperatureESM);
			num7 += ventCool.DebitESM * 0.34 * (num8 - ventCool.FlowTemperatureESM) / 1000.0;
		}
		double num9 = num7 * (double)month.Sundays;
		return num3 + num6 + num9;
	}

	private static List<TempHumidityPerDay> GetDaysHours(ClimateZones climateZone, int month)
	{
		TempHumidityPerDay item = PreferencesManager.GetClimateZoneParams(climateZone).TempHumidity.Months[month].Hours[23];
		List<TempHumidityPerDay> list = new List<TempHumidityPerDay> { item };
		list.AddRange(PreferencesManager.GetClimateZoneParams(climateZone).TempHumidity.Months[month].Hours);
		return list;
	}

	private static void CalculateMontlyCoolEnergyRef1(Section section, CalculationInput calcInput, CoolingCalculations ventCoolCalculations, MonthlyDays month, out double powHeating, out double powCooling)
	{
		double num = 0.0;
		double num2 = 0.0;
		List<TempHumidityPerDay> daysHours = GetDaysHours(calcInput.General.ClimateZone, (int)month.Month);
		for (int i = section.CoolingSeasons.Ventilation.WorkCurrentStart; i < section.CoolingSeasons.Ventilation.WorkCurrentEnd; i++)
		{
			double num3 = ventCoolCalculations.VentilationCooling.DebitRef1 * (CalcRoW(daysHours[i].Temp) * CalculateEntalpia(daysHours[i].Temp, daysHours[i].Humidity) - CalcRoW(ventCoolCalculations.VentilationCooling.FlowTemperatureRef1) * CalculateEntalpia(ventCoolCalculations.VentilationCooling.FlowTemperatureRef1, ventCoolCalculations.VentilationCooling.RelativeHumidityRef1));
			if (num3 < 0.0)
			{
				num2 += Math.Abs(num3);
			}
			else
			{
				num += num3;
			}
		}
		double num4 = num2 / 3600.0 * (double)month.WorkDays;
		double num5 = num / 3600.0 * (double)month.WorkDays;
		double num6 = 0.0;
		double num7 = 0.0;
		for (int j = section.CoolingSeasons.Ventilation.SatCurrentStart; j < section.CoolingSeasons.Ventilation.SatCurrentEnd; j++)
		{
			double num8 = ventCoolCalculations.VentilationCooling.DebitRef1 * (CalcRoW(daysHours[j].Temp) * CalculateEntalpia(daysHours[j].Temp, daysHours[j].Humidity) - CalcRoW(ventCoolCalculations.VentilationCooling.FlowTemperatureRef1) * CalculateEntalpia(ventCoolCalculations.VentilationCooling.FlowTemperatureRef1, ventCoolCalculations.VentilationCooling.RelativeHumidityRef1));
			if (num8 < 0.0)
			{
				num7 += Math.Abs(num8);
			}
			else
			{
				num6 += num8;
			}
		}
		double num9 = num7 / 3600.0 * (double)month.Saturdays;
		double num10 = num6 / 3600.0 * (double)month.Saturdays;
		double num11 = 0.0;
		double num12 = 0.0;
		for (int k = section.CoolingSeasons.Ventilation.SunCurrentStart; k < section.CoolingSeasons.Ventilation.SunCurrentEnd; k++)
		{
			double num13 = ventCoolCalculations.VentilationCooling.DebitRef1 * (CalcRo(daysHours[k].Temp, daysHours[k].Humidity) * CalculateEntalpia(daysHours[k].Temp, daysHours[k].Humidity) - CalcRoW(ventCoolCalculations.VentilationCooling.FlowTemperatureRef1) * CalculateEntalpia(ventCoolCalculations.VentilationCooling.FlowTemperatureRef1, ventCoolCalculations.VentilationCooling.RelativeHumidityRef1));
			if (num13 < 0.0)
			{
				num12 += Math.Abs(num13);
			}
			else
			{
				num11 += num13;
			}
		}
		double num14 = num12 / 3600.0 * (double)month.Sundays;
		double num15 = num11 / 3600.0 * (double)month.Sundays;
		powHeating = num14 + num9 + num4;
		powCooling = num15 + num10 + num5;
	}

	private static double CalculateWitheringEnergyRef1(Section section, CalculationInput calcInput, CoolingCalculations ventCoolCalculations, MonthlyDays month)
	{
		double num = 0.0;
		List<TempHumidityPerDay> daysHours = GetDaysHours(calcInput.General.ClimateZone, (int)month.Month);
		for (int i = section.CoolingSeasons.Ventilation.WorkCurrentStart; i < section.CoolingSeasons.Ventilation.WorkCurrentEnd; i++)
		{
			double num2 = ventCoolCalculations.VentilationCooling.DebitRef1 * (CalcRoW(daysHours[i].Temp) * CalculateWitheringEntalpia(daysHours[i].Temp, daysHours[i].Humidity) - CalcRoW(ventCoolCalculations.VentilationCooling.FlowTemperatureRef1) * CalculateWitheringEntalpia(ventCoolCalculations.VentilationCooling.FlowTemperatureRef1, ventCoolCalculations.VentilationCooling.RelativeHumidityRef1));
			num += num2;
		}
		double num3 = num / 3600.0 * (double)month.WorkDays;
		double num4 = 0.0;
		for (int j = section.CoolingSeasons.Ventilation.SatCurrentStart; j < section.CoolingSeasons.Ventilation.SatCurrentEnd; j++)
		{
			double num5 = ventCoolCalculations.VentilationCooling.DebitRef1 * (CalcRoW(daysHours[j].Temp) * CalculateWitheringEntalpia(daysHours[j].Temp, daysHours[j].Humidity) - CalcRoW(ventCoolCalculations.VentilationCooling.FlowTemperatureRef1) * CalculateWitheringEntalpia(ventCoolCalculations.VentilationCooling.FlowTemperatureRef1, ventCoolCalculations.VentilationCooling.RelativeHumidityRef1));
			num4 += num5;
		}
		double num6 = num4 / 3600.0 * (double)month.Saturdays;
		double num7 = 0.0;
		for (int k = section.CoolingSeasons.Ventilation.SunCurrentStart; k < section.CoolingSeasons.Ventilation.SunCurrentEnd; k++)
		{
			double num8 = ventCoolCalculations.VentilationCooling.DebitRef1 * (CalcRoW(daysHours[k].Temp) * CalculateWitheringEntalpia(daysHours[k].Temp, daysHours[k].Humidity) - CalcRoW(ventCoolCalculations.VentilationCooling.FlowTemperatureRef1) * CalculateWitheringEntalpia(ventCoolCalculations.VentilationCooling.FlowTemperatureRef1, ventCoolCalculations.VentilationCooling.RelativeHumidityRef1));
			num7 += num8;
		}
		double num9 = num7 / 3600.0 * (double)month.Sundays;
		return num3 + num6 + num9;
	}

	private static void CalculateMontlyCoolEnergyRef2(Section section, CalculationInput calcInput, CoolingCalculations ventCoolCalculations, MonthlyDays month, out double powHeating, out double powCooling)
	{
		double num = 0.0;
		double num2 = 0.0;
		List<TempHumidityPerDay> daysHours = GetDaysHours(calcInput.General.ClimateZone, (int)month.Month);
		for (int i = section.CoolingSeasons.Ventilation.WorkCurrentStart; i < section.CoolingSeasons.Ventilation.WorkCurrentEnd; i++)
		{
			double num3 = ventCoolCalculations.VentilationCooling.DebitRef2 * (CalcRoW(daysHours[i].Temp) * CalculateEntalpia(daysHours[i].Temp, daysHours[i].Humidity) - CalcRoW(ventCoolCalculations.VentilationCooling.FlowTemperatureRef2) * CalculateEntalpia(ventCoolCalculations.VentilationCooling.FlowTemperatureRef2, ventCoolCalculations.VentilationCooling.RelativeHumidityRef2));
			if (num3 < 0.0)
			{
				num2 += Math.Abs(num3);
			}
			else
			{
				num += num3;
			}
		}
		double num4 = num2 / 3600.0 * (double)month.WorkDays;
		double num5 = num / 3600.0 * (double)month.WorkDays;
		double num6 = 0.0;
		double num7 = 0.0;
		for (int j = section.CoolingSeasons.Ventilation.SatCurrentStart; j < section.CoolingSeasons.Ventilation.SatCurrentEnd; j++)
		{
			double num8 = ventCoolCalculations.VentilationCooling.DebitRef2 * (CalcRoW(daysHours[j].Temp) * CalculateEntalpia(daysHours[j].Temp, daysHours[j].Humidity) - CalcRoW(ventCoolCalculations.VentilationCooling.FlowTemperatureRef2) * CalculateEntalpia(ventCoolCalculations.VentilationCooling.FlowTemperatureRef2, ventCoolCalculations.VentilationCooling.RelativeHumidityRef2));
			if (num8 < 0.0)
			{
				num7 += Math.Abs(num8);
			}
			else
			{
				num6 += num8;
			}
		}
		double num9 = num7 / 3600.0 * (double)month.Saturdays;
		double num10 = num6 / 3600.0 * (double)month.Saturdays;
		double num11 = 0.0;
		double num12 = 0.0;
		for (int k = section.CoolingSeasons.Ventilation.SunCurrentStart; k < section.CoolingSeasons.Ventilation.SunCurrentEnd; k++)
		{
			double num13 = ventCoolCalculations.VentilationCooling.DebitRef2 * (CalcRoW(daysHours[k].Temp) * CalculateEntalpia(daysHours[k].Temp, daysHours[k].Humidity) - CalcRoW(ventCoolCalculations.VentilationCooling.FlowTemperatureRef2) * CalculateEntalpia(ventCoolCalculations.VentilationCooling.FlowTemperatureRef2, ventCoolCalculations.VentilationCooling.RelativeHumidityRef2));
			if (num13 < 0.0)
			{
				num12 += Math.Abs(num13);
			}
			else
			{
				num11 += num13;
			}
		}
		double num14 = num12 / 3600.0 * (double)month.Sundays;
		double num15 = num11 / 3600.0 * (double)month.Sundays;
		powHeating = num14 + num9 + num4;
		powCooling = num15 + num10 + num5;
	}

	private static double CalculateWitheringEnergyRef2(Section section, CalculationInput calcInput, CoolingCalculations ventCoolCalculations, MonthlyDays month)
	{
		double num = 0.0;
		List<TempHumidityPerDay> daysHours = GetDaysHours(calcInput.General.ClimateZone, (int)month.Month);
		for (int i = section.CoolingSeasons.Ventilation.WorkCurrentStart; i < section.CoolingSeasons.Ventilation.WorkCurrentEnd; i++)
		{
			double num2 = ventCoolCalculations.VentilationCooling.DebitRef2 * (CalcRoW(daysHours[i].Temp) * CalculateWitheringEntalpia(daysHours[i].Temp, daysHours[i].Humidity) - CalcRoW(ventCoolCalculations.VentilationCooling.FlowTemperatureRef2) * CalculateWitheringEntalpia(ventCoolCalculations.VentilationCooling.FlowTemperatureRef2, ventCoolCalculations.VentilationCooling.RelativeHumidityRef2));
			num += num2;
		}
		double num3 = num / 3600.0 * (double)month.WorkDays;
		double num4 = 0.0;
		for (int j = section.CoolingSeasons.Ventilation.SatCurrentStart; j < section.CoolingSeasons.Ventilation.SatCurrentEnd; j++)
		{
			double num5 = ventCoolCalculations.VentilationCooling.DebitRef2 * (CalcRoW(daysHours[j].Temp) * CalculateWitheringEntalpia(daysHours[j].Temp, daysHours[j].Humidity) - CalcRoW(ventCoolCalculations.VentilationCooling.FlowTemperatureRef2) * CalculateWitheringEntalpia(ventCoolCalculations.VentilationCooling.FlowTemperatureRef2, ventCoolCalculations.VentilationCooling.RelativeHumidityRef2));
			num4 += num5;
		}
		double num6 = num4 / 3600.0 * (double)month.Saturdays;
		double num7 = 0.0;
		for (int k = section.CoolingSeasons.Ventilation.SunCurrentStart; k < section.CoolingSeasons.Ventilation.SunCurrentEnd; k++)
		{
			double num8 = ventCoolCalculations.VentilationCooling.DebitRef2 * (CalcRoW(daysHours[k].Temp) * CalculateWitheringEntalpia(daysHours[k].Temp, daysHours[k].Humidity) - CalcRoW(ventCoolCalculations.VentilationCooling.FlowTemperatureRef2) * CalculateWitheringEntalpia(ventCoolCalculations.VentilationCooling.FlowTemperatureRef2, ventCoolCalculations.VentilationCooling.RelativeHumidityRef2));
			num7 += num8;
		}
		double num9 = num7 / 3600.0 * (double)month.Sundays;
		return num3 + num6 + num9;
	}

	private static void CalculateMontlyCoolEnergyActual(Section section, CalculationInput calcInput, CoolingCalculations ventCoolCalculations, MonthlyDays month, out double powHeating, out double powCooling)
	{
		double num = 0.0;
		double num2 = 0.0;
		List<TempHumidityPerDay> daysHours = GetDaysHours(calcInput.General.ClimateZone, (int)month.Month);
		for (int i = section.CoolingSeasons.Ventilation.WorkCurrentStart; i < section.CoolingSeasons.Ventilation.WorkCurrentEnd; i++)
		{
			double num3 = ventCoolCalculations.VentilationCooling.DebitActual * (CalcRo(daysHours[i].Temp, daysHours[i].Humidity) * CalculateEntalpia(daysHours[i].Temp, daysHours[i].Humidity) - CalcRo(ventCoolCalculations.VentilationCooling.FlowTemperatureActual, ventCoolCalculations.VentilationCooling.RelativeHumidityActual) * CalculateEntalpia(ventCoolCalculations.VentilationCooling.FlowTemperatureActual, ventCoolCalculations.VentilationCooling.RelativeHumidityActual));
			if (num3 < 0.0)
			{
				num2 += Math.Abs(num3);
			}
			else
			{
				num += num3;
			}
		}
		double num4 = num2 / 3600.0 * (double)month.WorkDays;
		double num5 = num / 3600.0 * (double)month.WorkDays;
		double num6 = 0.0;
		double num7 = 0.0;
		for (int j = section.CoolingSeasons.Ventilation.SatCurrentStart; j < section.CoolingSeasons.Ventilation.SatCurrentEnd; j++)
		{
			double num8 = ventCoolCalculations.VentilationCooling.DebitActual * (CalcRo(daysHours[j].Temp, daysHours[j].Humidity) * CalculateEntalpia(daysHours[j].Temp, daysHours[j].Humidity) - CalcRo(ventCoolCalculations.VentilationCooling.FlowTemperatureActual, ventCoolCalculations.VentilationCooling.RelativeHumidityActual) * CalculateEntalpia(ventCoolCalculations.VentilationCooling.FlowTemperatureActual, ventCoolCalculations.VentilationCooling.RelativeHumidityActual));
			if (num8 < 0.0)
			{
				num7 += Math.Abs(num8);
			}
			else
			{
				num6 += num8;
			}
		}
		double num9 = num7 / 3600.0 * (double)month.Saturdays;
		double num10 = num6 / 3600.0 * (double)month.Saturdays;
		double num11 = 0.0;
		double num12 = 0.0;
		for (int k = section.CoolingSeasons.Ventilation.SunCurrentStart; k < section.CoolingSeasons.Ventilation.SunCurrentEnd; k++)
		{
			double num13 = ventCoolCalculations.VentilationCooling.DebitActual * (CalcRo(daysHours[k].Temp, daysHours[k].Humidity) * CalculateEntalpia(daysHours[k].Temp, daysHours[k].Humidity) - CalcRo(ventCoolCalculations.VentilationCooling.FlowTemperatureActual, ventCoolCalculations.VentilationCooling.RelativeHumidityActual) * CalculateEntalpia(ventCoolCalculations.VentilationCooling.FlowTemperatureActual, ventCoolCalculations.VentilationCooling.RelativeHumidityActual));
			if (num13 < 0.0)
			{
				num12 += Math.Abs(num13);
			}
			else
			{
				num11 += num13;
			}
		}
		double num14 = num12 / 3600.0 * (double)month.Sundays;
		double num15 = num11 / 3600.0 * (double)month.Sundays;
		powHeating = num14 + num9 + num4;
		powCooling = num15 + num10 + num5;
	}

	private static double CalculateWitheringEnergyActual(Section section, CalculationInput calcInput, CoolingCalculations ventCoolCalculations, MonthlyDays month)
	{
		double num = 0.0;
		List<TempHumidityPerDay> daysHours = GetDaysHours(calcInput.General.ClimateZone, (int)month.Month);
		for (int i = section.CoolingSeasons.Ventilation.WorkCurrentStart; i < section.CoolingSeasons.Ventilation.WorkCurrentEnd; i++)
		{
			double num2 = ventCoolCalculations.VentilationCooling.DebitActual * (CalcRoW(daysHours[i].Temp) * CalculateWitheringEntalpia(daysHours[i].Temp, daysHours[i].Humidity) - CalcRoW(ventCoolCalculations.VentilationCooling.FlowTemperatureActual) * CalculateWitheringEntalpia(ventCoolCalculations.VentilationCooling.FlowTemperatureActual, ventCoolCalculations.VentilationCooling.RelativeHumidityActual));
			num += num2;
		}
		double num3 = num / 3600.0 * (double)month.WorkDays;
		double num4 = 0.0;
		for (int j = section.CoolingSeasons.Ventilation.SatCurrentStart; j < section.CoolingSeasons.Ventilation.SatCurrentEnd; j++)
		{
			double num5 = ventCoolCalculations.VentilationCooling.DebitActual * (CalcRoW(daysHours[j].Temp) * CalculateWitheringEntalpia(daysHours[j].Temp, daysHours[j].Humidity) - CalcRoW(ventCoolCalculations.VentilationCooling.FlowTemperatureActual) * CalculateWitheringEntalpia(ventCoolCalculations.VentilationCooling.FlowTemperatureActual, ventCoolCalculations.VentilationCooling.RelativeHumidityActual));
			num4 += num5;
		}
		double num6 = num4 / 3600.0 * (double)month.Saturdays;
		double num7 = 0.0;
		for (int k = section.CoolingSeasons.Ventilation.SunCurrentStart; k < section.CoolingSeasons.Ventilation.SunCurrentEnd; k++)
		{
			double num8 = ventCoolCalculations.VentilationCooling.DebitActual * (CalcRoW(daysHours[k].Temp) * CalculateWitheringEntalpia(daysHours[k].Temp, daysHours[k].Humidity) - CalcRoW(ventCoolCalculations.VentilationCooling.FlowTemperatureActual) * CalculateWitheringEntalpia(ventCoolCalculations.VentilationCooling.FlowTemperatureActual, ventCoolCalculations.VentilationCooling.RelativeHumidityActual));
			num7 += num8;
		}
		double num9 = num7 / 3600.0 * (double)month.Sundays;
		return num3 + num6 + num9;
	}

	private static void CalculateMontlyCoolEnergyBaseLine(Section section, CalculationInput calcInput, CalculationData calcData, MonthlyDays month, out double powHeating, out double powCooling)
	{
		double num = 0.0;
		double num2 = 0.0;
		List<TempHumidityPerDay> daysHours = GetDaysHours(calcInput.General.ClimateZone, (int)month.Month);
		for (int i = section.CoolingSeasons.Ventilation.WorkBaseStart; i < section.CoolingSeasons.Ventilation.WorkBaseEnd; i++)
		{
			double num3 = calcData.DebitBaseLine * (CalcRo(daysHours[i].Temp, daysHours[i].Humidity) * CalculateEntalpia(daysHours[i].Temp, daysHours[i].Humidity) - CalcRo(calcData.FlowTemperatureBaseLine, calcData.RelativeHumidityBaseLine) * CalculateEntalpia(calcData.FlowTemperatureBaseLine, calcData.RelativeHumidityBaseLine));
			if (num3 < 0.0)
			{
				num2 += Math.Abs(num3);
			}
			else
			{
				num += num3;
			}
		}
		double num4 = num2 / 3600.0 * (double)month.WorkDays;
		double num5 = num / 3600.0 * (double)month.WorkDays;
		double num6 = 0.0;
		double num7 = 0.0;
		for (int j = section.CoolingSeasons.Ventilation.SatBaseStart; j < section.CoolingSeasons.Ventilation.SatBaseEnd; j++)
		{
			double num8 = calcData.DebitBaseLine * (CalcRo(daysHours[j].Temp, daysHours[j].Humidity) * CalculateEntalpia(daysHours[j].Temp, daysHours[j].Humidity) - CalcRo(calcData.FlowTemperatureBaseLine, calcData.RelativeHumidityBaseLine) * CalculateEntalpia(calcData.FlowTemperatureBaseLine, calcData.RelativeHumidityBaseLine));
			if (num8 < 0.0)
			{
				num7 += Math.Abs(num8);
			}
			else
			{
				num6 += num8;
			}
		}
		double num9 = num7 / 3600.0 * (double)month.Saturdays;
		double num10 = num6 / 3600.0 * (double)month.Saturdays;
		double num11 = 0.0;
		double num12 = 0.0;
		for (int k = section.CoolingSeasons.Ventilation.SunBaseStart; k < section.CoolingSeasons.Ventilation.SunBaseEnd; k++)
		{
			double num13 = calcData.DebitBaseLine * (CalcRo(daysHours[k].Temp, daysHours[k].Humidity) * CalculateEntalpia(daysHours[k].Temp, daysHours[k].Humidity) - CalcRo(calcData.FlowTemperatureBaseLine, calcData.RelativeHumidityBaseLine) * CalculateEntalpia(calcData.FlowTemperatureBaseLine, calcData.RelativeHumidityBaseLine));
			if (num13 < 0.0)
			{
				num12 += Math.Abs(num13);
			}
			else
			{
				num11 += num13;
			}
		}
		double num14 = num12 / 3600.0 * (double)month.Sundays;
		double num15 = num11 / 3600.0 * (double)month.Sundays;
		powHeating = num14 + num9 + num4;
		powCooling = num15 + num10 + num5;
	}

	private static double CalculateWitheringEnergyBaseLine(Section section, CalculationInput calcInput, CalculationData calcData, MonthlyDays month)
	{
		double num = 0.0;
		List<TempHumidityPerDay> daysHours = GetDaysHours(calcInput.General.ClimateZone, (int)month.Month);
		for (int i = section.CoolingSeasons.Ventilation.WorkBaseStart; i < section.CoolingSeasons.Ventilation.WorkBaseEnd; i++)
		{
			double num2 = calcData.DebitBaseLine * (CalcRoW(daysHours[i].Temp) * CalculateWitheringEntalpia(daysHours[i].Temp, daysHours[i].Humidity) - CalcRoW(calcData.FlowTemperatureBaseLine) * CalculateWitheringEntalpia(calcData.FlowTemperatureBaseLine, calcData.RelativeHumidityBaseLine));
			num += num2;
		}
		double num3 = num / 3600.0 * (double)month.WorkDays;
		double num4 = 0.0;
		for (int j = section.CoolingSeasons.Ventilation.SatBaseStart; j < section.CoolingSeasons.Ventilation.SatBaseEnd; j++)
		{
			double num5 = calcData.DebitBaseLine * (CalcRoW(daysHours[j].Temp) * CalculateWitheringEntalpia(daysHours[j].Temp, daysHours[j].Humidity) - CalcRoW(calcData.FlowTemperatureBaseLine) * CalculateWitheringEntalpia(calcData.FlowTemperatureBaseLine, calcData.RelativeHumidityBaseLine));
			num4 += num5;
		}
		double num6 = num4 / 3600.0 * (double)month.Saturdays;
		double num7 = 0.0;
		for (int k = section.CoolingSeasons.Ventilation.SunBaseStart; k < section.CoolingSeasons.Ventilation.SunBaseEnd; k++)
		{
			double num8 = calcData.DebitBaseLine * (CalcRoW(daysHours[k].Temp) * CalculateWitheringEntalpia(daysHours[k].Temp, daysHours[k].Humidity) - CalcRoW(calcData.FlowTemperatureBaseLine) * CalculateWitheringEntalpia(calcData.FlowTemperatureBaseLine, calcData.RelativeHumidityBaseLine));
			num7 += num8;
		}
		double num9 = num7 / 3600.0 * (double)month.Sundays;
		return num3 + num6 + num9;
	}

	private static void CalculateMontlyCoolEnergyESM(Section section, CalculationInput calcInput, CoolingCalculations ventCoolCalculations, MonthlyDays month, out double powHeating, out double powCooling)
	{
		double num = 0.0;
		double num2 = 0.0;
		List<TempHumidityPerDay> daysHours = GetDaysHours(calcInput.General.ClimateZone, (int)month.Month);
		for (int i = section.CoolingSeasons.Ventilation.WorkEsmStart; i < section.CoolingSeasons.Ventilation.WorkEsmEnd; i++)
		{
			double num3 = ventCoolCalculations.VentilationCooling.DebitESM * (CalcRo(daysHours[i].Temp, daysHours[i].Humidity) * CalculateEntalpia(daysHours[i].Temp, daysHours[i].Humidity) - CalcRo(ventCoolCalculations.VentilationCooling.FlowTemperatureESM, ventCoolCalculations.VentilationCooling.RelativeHumidityESM) * CalculateEntalpia(ventCoolCalculations.VentilationCooling.FlowTemperatureESM, ventCoolCalculations.VentilationCooling.RelativeHumidityESM));
			if (num3 < 0.0)
			{
				num2 += Math.Abs(num3);
			}
			else
			{
				num += num3;
			}
		}
		double num4 = num2 / 3600.0 * (double)month.WorkDays;
		double num5 = num / 3600.0 * (double)month.WorkDays;
		double num6 = 0.0;
		double num7 = 0.0;
		for (int j = section.CoolingSeasons.Ventilation.SatEsmStart; j < section.CoolingSeasons.Ventilation.SatEsmEnd; j++)
		{
			double num8 = ventCoolCalculations.VentilationCooling.DebitESM * (CalcRo(daysHours[j].Temp, daysHours[j].Humidity) * CalculateEntalpia(daysHours[j].Temp, daysHours[j].Humidity) - CalcRo(ventCoolCalculations.VentilationCooling.FlowTemperatureESM, ventCoolCalculations.VentilationCooling.RelativeHumidityESM) * CalculateEntalpia(ventCoolCalculations.VentilationCooling.FlowTemperatureESM, ventCoolCalculations.VentilationCooling.RelativeHumidityESM));
			if (num8 < 0.0)
			{
				num7 += Math.Abs(num8);
			}
			else
			{
				num6 += num8;
			}
		}
		double num9 = num7 / 3600.0 * (double)month.Saturdays;
		double num10 = num6 / 3600.0 * (double)month.Saturdays;
		double num11 = 0.0;
		double num12 = 0.0;
		for (int k = section.CoolingSeasons.Ventilation.SunEsmStart; k < section.CoolingSeasons.Ventilation.SunEsmEnd; k++)
		{
			double num13 = ventCoolCalculations.VentilationCooling.DebitESM * (CalcRo(daysHours[k].Temp, daysHours[k].Humidity) * CalculateEntalpia(daysHours[k].Temp, daysHours[k].Humidity) - CalcRo(ventCoolCalculations.VentilationCooling.FlowTemperatureESM, ventCoolCalculations.VentilationCooling.RelativeHumidityESM) * CalculateEntalpia(ventCoolCalculations.VentilationCooling.FlowTemperatureESM, ventCoolCalculations.VentilationCooling.RelativeHumidityESM));
			if (num13 < 0.0)
			{
				num12 += Math.Abs(num13);
			}
			else
			{
				num11 += num13;
			}
		}
		double num14 = num12 / 3600.0 * (double)month.Sundays;
		double num15 = num11 / 3600.0 * (double)month.Sundays;
		powHeating = num14 + num9 + num4;
		powCooling = num15 + num10 + num5;
	}

	private static double CalculateWitheringEnergyESM(Section section, CalculationInput calcInput, CoolingCalculations ventCoolCalculations, MonthlyDays month)
	{
		double num = 0.0;
		List<TempHumidityPerDay> daysHours = GetDaysHours(calcInput.General.ClimateZone, (int)month.Month);
		for (int i = section.CoolingSeasons.Ventilation.WorkEsmStart; i < section.CoolingSeasons.Ventilation.WorkEsmEnd; i++)
		{
			double num2 = ventCoolCalculations.VentilationCooling.DebitESM * (CalcRoW(daysHours[i].Temp) * CalculateWitheringEntalpia(daysHours[i].Temp, daysHours[i].Humidity) - CalcRoW(ventCoolCalculations.VentilationCooling.FlowTemperatureESM) * CalculateWitheringEntalpia(ventCoolCalculations.VentilationCooling.FlowTemperatureESM, ventCoolCalculations.VentilationCooling.RelativeHumidityESM));
			num += num2;
		}
		double num3 = num / 3600.0 * (double)month.WorkDays;
		double num4 = 0.0;
		for (int j = section.CoolingSeasons.Ventilation.SatEsmStart; j < section.CoolingSeasons.Ventilation.SatEsmEnd; j++)
		{
			double num5 = ventCoolCalculations.VentilationCooling.DebitESM * (CalcRoW(daysHours[j].Temp) * CalculateWitheringEntalpia(daysHours[j].Temp, daysHours[j].Humidity) - CalcRoW(ventCoolCalculations.VentilationCooling.FlowTemperatureESM) * CalculateWitheringEntalpia(ventCoolCalculations.VentilationCooling.FlowTemperatureESM, ventCoolCalculations.VentilationCooling.RelativeHumidityESM));
			num4 += num5;
		}
		double num6 = num4 / 3600.0 * (double)month.Saturdays;
		double num7 = 0.0;
		for (int k = section.CoolingSeasons.Ventilation.SunEsmStart; k < section.CoolingSeasons.Ventilation.SunEsmEnd; k++)
		{
			double num8 = ventCoolCalculations.VentilationCooling.DebitESM * (CalcRoW(daysHours[k].Temp) * CalculateWitheringEntalpia(daysHours[k].Temp, daysHours[k].Humidity) - CalcRoW(ventCoolCalculations.VentilationCooling.FlowTemperatureESM) * CalculateWitheringEntalpia(ventCoolCalculations.VentilationCooling.FlowTemperatureESM, ventCoolCalculations.VentilationCooling.RelativeHumidityESM));
			num7 += num8;
		}
		double num9 = num7 / 3600.0 * (double)month.Sundays;
		return num3 + num6 + num9;
	}

	private static double CalculateEntalpia(double temp, double humidity)
	{
		double num = CalcAirX(temp, humidity);
		return 1.006 * temp + num * (2501.0 + 1.805 * temp);
	}

	private static double CalculateWitheringEntalpia(double temp, double humidity)
	{
		double num = CalcAirX(temp, humidity);
		return num * (2501.0 + 1.805 * temp);
	}

	public static void VentilationHeatEnergyRef1(this CalculationData calcData, Section section, CalculationInput calcInput, HeatingCalculations heatCalculations)
	{
		List<double> list = new List<double>();
		List<double> list2 = new List<double>();
		List<double> list3 = new List<double>();
		List<MonthlyDays> list4 = section.CalcPeriod((int)section.HeatingSeason.FirstMonthHeat, (int)section.HeatingSeason.LastMonthHeat, section.HeatingSeason.FirstDayHeat, section.HeatingSeason.LastDayHeat);
		foreach (MonthlyDays item in list4)
		{
			double thermoPumpEnergy;
			double num = CalculateMontlyHeatEnergyRef1(calcData, section, calcInput, heatCalculations, item, out thermoPumpEnergy);
			list3.Add(thermoPumpEnergy);
			if (!double.IsNaN(num))
			{
				list.Add(num + thermoPumpEnergy);
			}
			list2.Add(calcData.DebitRef1 * 0.34 * (calcData.FlowTemperatureRef1 - heatCalculations.HeatingResult.ProjectTemperatureRef1) * monthHours / 1000.0);
		}
		if (calcData.SecondRecEfficiencyRef1 > 100.0)
		{
			calcData.ResultSourceEnergyRef1 = list3.Aggregate(0.0, (double num4, double item) => num4 + item);
			double num2 = list.Aggregate(0.0, (double num4, double item) => num4 + item);
			calcData.ResultSourceEnergy2Ref1 = num2 - calcData.ResultSourceEnergyRef1;
			double num3 = calcData.ResultSourceEnergyRef1 / num2 * 100.0;
			if (double.IsInfinity(num3) || double.IsNaN(num3))
			{
				num3 = 100.0;
			}
			calcData.Part1Ref1 = num3;
		}
		calcData.ResulHeatingInputsRef1 = list2.Aggregate(0.0, (double num4, double item) => num4 + item);
		heatCalculations.HeatingResult.ResulVentilationInputsRef1 = calcData.ResulHeatingInputsRef1;
		calcData.ResultEnergyForHeatingRef1 = ((list.Count == list4.Count) ? list.Aggregate(0.0, (double num4, double item) => num4 + item) : 0.0);
	}

	public static void VentilationHeatEnergyRef2(this CalculationData calcData, Section section, CalculationInput calcInput, HeatingCalculations heatCalculations)
	{
		List<double> list = new List<double>();
		List<double> list2 = new List<double>();
		List<double> list3 = new List<double>();
		List<MonthlyDays> list4 = section.CalcPeriod((int)section.HeatingSeason.FirstMonthHeat, (int)section.HeatingSeason.LastMonthHeat, section.HeatingSeason.FirstDayHeat, section.HeatingSeason.LastDayHeat);
		foreach (MonthlyDays item in list4)
		{
			double thermoPumpEnergy;
			double num = CalculateMontlyHeatEnergyRef2(calcData, section, calcInput, heatCalculations, item, out thermoPumpEnergy);
			list3.Add(thermoPumpEnergy);
			if (!double.IsNaN(num))
			{
				list.Add(num + thermoPumpEnergy);
			}
			list2.Add(calcData.DebitRef2 * 0.34 * (calcData.FlowTemperatureRef2 - heatCalculations.HeatingResult.ProjectTemperatureRef2) * monthHours / 1000.0);
		}
		if (calcData.SecondRecEfficiencyRef2 > 100.0)
		{
			calcData.ResultSourceEnergyRef2 = list3.Aggregate(0.0, (double num4, double item) => num4 + item);
			double num2 = list.Aggregate(0.0, (double num4, double item) => num4 + item);
			calcData.ResultSourceEnergy2Ref2 = num2 - calcData.ResultSourceEnergyRef2;
			double num3 = calcData.ResultSourceEnergyRef2 / num2 * 100.0;
			if (double.IsInfinity(num3) || double.IsNaN(num3))
			{
				num3 = 100.0;
			}
			calcData.Part1Ref2 = num3;
		}
		calcData.ResulHeatingInputsRef2 = list2.Aggregate(0.0, (double num4, double item) => num4 + item);
		heatCalculations.HeatingResult.ResulVentilationInputsRef2 = calcData.ResulHeatingInputsRef2;
		calcData.ResultEnergyForHeatingRef2 = ((list.Count == list4.Count) ? list.Aggregate(0.0, (double num4, double item) => num4 + item) : 0.0);
	}

	public static void VentilationHeatEnergyActual(this CalculationData calcData, Section section, CalculationInput calcInput, HeatingCalculations heatCalculations)
	{
		List<double> list = new List<double>();
		List<double> list2 = new List<double>();
		List<double> list3 = new List<double>();
		List<MonthlyDays> list4 = section.CalcPeriod((int)section.HeatingSeason.FirstMonthHeat, (int)section.HeatingSeason.LastMonthHeat, section.HeatingSeason.FirstDayHeat, section.HeatingSeason.LastDayHeat);
		foreach (MonthlyDays item in list4)
		{
			double thermoPumpEnergy;
			double num = CalculateMontlyHeatEnergyActual(calcData, section, calcInput, heatCalculations, item, out thermoPumpEnergy);
			list3.Add(thermoPumpEnergy);
			if (!double.IsNaN(num))
			{
				list.Add(num + thermoPumpEnergy);
			}
			if (item.Month == Month.January)
			{
				section.Area.ETlineData.MonthJanuaryVentilationHeatingEnergy.Actual = num * section.Area.HeatedArea;
			}
			if (item.Month == Month.March)
			{
				section.Area.ETlineData.MonthMarchVentilationHeatingEnergy.Actual = num * section.Area.HeatedArea;
			}
			innerTemp = CalculateAverageVentHeatTempActual(section, heatCalculations, item);
			list2.Add(calcData.DebitActual * 0.34 * (calcData.FlowTemperatureActual - heatCalculations.HeatingResult.ProjectTemperatureActual) * monthHours / 1000.0);
		}
		if (calcData.SecondRecEfficiencyActual > 100.0)
		{
			calcData.ResultSourceEnergyActual = list3.Aggregate(0.0, (double num4, double item) => num4 + item);
			double num2 = list.Aggregate(0.0, (double num4, double item) => num4 + item);
			calcData.ResultSourceEnergy2Actual = num2 - calcData.ResultSourceEnergyActual;
			double num3 = calcData.ResultSourceEnergyActual / num2 * 100.0;
			if (double.IsInfinity(num3) || double.IsNaN(num3))
			{
				num3 = 100.0;
			}
			calcData.Part1Actual = num3;
		}
		calcData.ResulHeatingInputsActual = list2.Aggregate(0.0, (double num4, double item) => num4 + item);
		heatCalculations.HeatingResult.ResulVentilationInputsActual = calcData.ResulHeatingInputsActual;
		calcData.ResultEnergyForHeatingActual = ((list.Count == list4.Count) ? list.Aggregate(0.0, (double num4, double item) => num4 + item) : 0.0);
	}

	public static void VentilationHeatEnergyBaseLine(this CalculationData calcData, Section section, CalculationInput calcInput, HeatingCalculations heatCalculations)
	{
		List<double> list = new List<double>();
		List<double> list2 = new List<double>();
		List<double> list3 = new List<double>();
		List<MonthlyDays> list4 = section.CalcPeriod((int)section.HeatingSeason.FirstMonthHeat, (int)section.HeatingSeason.LastMonthHeat, section.HeatingSeason.FirstDayHeat, section.HeatingSeason.LastDayHeat);
		foreach (MonthlyDays item in list4)
		{
			double thermoPumpEnergy;
			double num = CalculateMontlyHeatEnergyBaseLine(calcData, section, calcInput, heatCalculations, item, out thermoPumpEnergy);
			list3.Add(thermoPumpEnergy);
			if (!double.IsNaN(num))
			{
				list.Add(num + thermoPumpEnergy);
			}
			if (item.Month == Month.January)
			{
				section.Area.ETlineData.MonthJanuaryVentilationHeatingEnergy.BaseLine = num * section.Area.HeatedArea;
			}
			if (item.Month == Month.March)
			{
				section.Area.ETlineData.MonthMarchVentilationHeatingEnergy.BaseLine = num * section.Area.HeatedArea;
			}
			innerTemp = CalculateAverageVentHeatTempBaseLine(section, heatCalculations, item);
			list2.Add(calcData.DebitBaseLine * 0.34 * (calcData.FlowTemperatureBaseLine - heatCalculations.HeatingResult.ProjectTemperatureBaseLine) * monthHours / 1000.0);
		}
		if (calcData.SecondRecEfficiencyBaseLine > 100.0)
		{
			calcData.ResultSourceEnergyBaseLine = list3.Aggregate(0.0, (double num4, double item) => num4 + item);
			double num2 = list.Aggregate(0.0, (double num4, double item) => num4 + item);
			calcData.ResultSourceEnergy2BaseLine = num2 - calcData.ResultSourceEnergyBaseLine;
			double num3 = calcData.ResultSourceEnergyBaseLine / num2 * 100.0;
			if (double.IsInfinity(num3) || double.IsNaN(num3))
			{
				num3 = 100.0;
			}
			calcData.Part1BaseLine = num3;
		}
		calcData.ResulHeatingInputsBaseLine = list2.Aggregate(0.0, (double num4, double item) => num4 + item);
		heatCalculations.HeatingResult.ResulVentilationInputsBaseLine = calcData.ResulHeatingInputsBaseLine;
		calcData.ResultEnergyForHeatingBaseLine = ((list.Count == list4.Count) ? list.Aggregate(0.0, (double num4, double item) => num4 + item) : 0.0);
	}

	public static void VentilationHeatEnergyESM(this CalculationData calcData, Section section, CalculationInput calcInput, HeatingCalculations heatCalculations)
	{
		List<double> list = new List<double>();
		List<double> list2 = new List<double>();
		List<double> list3 = new List<double>();
		List<MonthlyDays> list4 = section.CalcPeriod((int)section.HeatingSeason.FirstMonthHeat, (int)section.HeatingSeason.LastMonthHeat, section.HeatingSeason.FirstDayHeat, section.HeatingSeason.LastDayHeat);
		foreach (MonthlyDays item in list4)
		{
			double thermoPumpEnergy;
			double num = CalculateMontlyHeatEnergyESM(calcData, section, calcInput, heatCalculations, item, out thermoPumpEnergy);
			list3.Add(thermoPumpEnergy);
			if (!double.IsNaN(num))
			{
				list.Add(num + thermoPumpEnergy);
			}
			if (item.Month == Month.January)
			{
				section.Area.ETlineData.MonthJanuaryVentilationHeatingEnergy.ESM = num * section.Area.HeatedArea;
			}
			if (item.Month == Month.March)
			{
				section.Area.ETlineData.MonthMarchVentilationHeatingEnergy.ESM = num * section.Area.HeatedArea;
			}
			innerTemp = CalculateAverageVentHeatTempESM(section, heatCalculations, item);
			list2.Add(calcData.DebitESM * 0.34 * (calcData.FlowTemperatureESM - heatCalculations.HeatingResult.ProjectTemperatureESM) * monthHours / 1000.0);
		}
		if (calcData.SecondRecEfficiencyESM > 100.0)
		{
			calcData.ResultSourceEnergyESM = list3.Aggregate(0.0, (double num4, double item) => num4 + item);
			double num2 = list.Aggregate(0.0, (double num4, double item) => num4 + item);
			calcData.ResultSourceEnergy2ESM = num2 - calcData.ResultSourceEnergyESM;
			double num3 = calcData.ResultSourceEnergyESM / num2 * 100.0;
			if (double.IsInfinity(num3) || double.IsNaN(num3))
			{
				num3 = 100.0;
			}
			calcData.Part1ESM = num3;
		}
		calcData.ResulHeatingInputsESM = list2.Aggregate(0.0, (double num4, double item) => num4 + item);
		heatCalculations.HeatingResult.ResulVentilationInputsESM = calcData.ResulHeatingInputsESM;
		calcData.ResultEnergyForHeatingESM = ((list.Count == list4.Count) ? list.Aggregate(0.0, (double num4, double item) => num4 + item) : 0.0);
	}

	public static void CalculateVentNeededEnergyRef1(this CalculationData heatgCalc)
	{
		if (heatgCalc.SecondRecEfficiencyRef1 > 100.0)
		{
			heatgCalc.ResultSourceEnergyRef1 /= heatgCalc.TransmitTempEfficiencyRef1 / 100.0 * (heatgCalc.SupplyNetEfficiencyRef1 / 100.0) * (heatgCalc.AutomaticRef1 / 100.0) * (heatgCalc.EnergyManagementRef1 / 100.0) * heatgCalc.GeneratorHeatEfficiency1Ref1 / 100.0;
			if (double.IsInfinity(heatgCalc.ResultSourceEnergyRef1) || double.IsNaN(heatgCalc.ResultSourceEnergyRef1))
			{
				heatgCalc.ResultSourceEnergyRef1 = 0.0;
			}
			heatgCalc.ResultSourceEnergy2Ref1 /= heatgCalc.TransmitTempEfficiency2Ref1 / 100.0 * (heatgCalc.SupplyNetEfficiency2Ref1 / 100.0) * (heatgCalc.Automatic2Ref1 / 100.0) * (heatgCalc.EnergyManagement2Ref1 / 100.0) * (heatgCalc.GeneratorHeatEfficiency2Ref1 / 100.0);
			if (double.IsInfinity(heatgCalc.ResultSourceEnergy2Ref1) || double.IsNaN(heatgCalc.ResultSourceEnergy2Ref1))
			{
				heatgCalc.ResultSourceEnergy2Ref1 = 0.0;
			}
			heatgCalc.ResultNeededEnergyRef1 = heatgCalc.ResultSourceEnergyRef1 + heatgCalc.ResultSourceEnergy2Ref1;
			return;
		}
		double num = heatgCalc.ResultEnergyForHeatingRef1 * heatgCalc.Part1Ref1 / 100.0;
		heatgCalc.ResultSourceEnergyRef1 = num / (heatgCalc.TransmitTempEfficiencyRef1 / 100.0 * (heatgCalc.SupplyNetEfficiencyRef1 / 100.0) * (heatgCalc.AutomaticRef1 / 100.0) * (heatgCalc.EnergyManagementRef1 / 100.0) * (heatgCalc.GeneratorHeatEfficiency1Ref1 / 100.0));
		if (double.IsInfinity(heatgCalc.ResultSourceEnergyRef1) || double.IsNaN(heatgCalc.ResultSourceEnergyRef1))
		{
			heatgCalc.ResultSourceEnergyRef1 = 0.0;
		}
		double num2 = heatgCalc.ResultEnergyForHeatingRef1 * heatgCalc.Part2Ref1 / 100.0;
		heatgCalc.ResultSourceEnergy2Ref1 = num2 / (heatgCalc.TransmitTempEfficiency2Ref1 / 100.0 * (heatgCalc.SupplyNetEfficiency2Ref1 / 100.0) * (heatgCalc.Automatic2Ref1 / 100.0) * (heatgCalc.EnergyManagement2Ref1 / 100.0) * (heatgCalc.GeneratorHeatEfficiency2Ref1 / 100.0));
		if (double.IsInfinity(heatgCalc.ResultSourceEnergy2Ref1) || double.IsNaN(heatgCalc.ResultSourceEnergy2Ref1))
		{
			heatgCalc.ResultSourceEnergy2Ref1 = 0.0;
		}
		heatgCalc.ResultNeededEnergyRef1 = heatgCalc.ResultSourceEnergyRef1 + heatgCalc.ResultSourceEnergy2Ref1;
	}

	public static void CalculateVentNeededEnergyRef2(this CalculationData heatgCalc)
	{
		if (heatgCalc.SecondRecEfficiencyRef2 > 100.0)
		{
			heatgCalc.ResultSourceEnergyRef2 /= heatgCalc.TransmitTempEfficiencyRef2 / 100.0 * (heatgCalc.SupplyNetEfficiencyRef2 / 100.0) * (heatgCalc.AutomaticRef2 / 100.0) * (heatgCalc.EnergyManagementRef2 / 100.0) * heatgCalc.GeneratorHeatEfficiency1Ref2 / 100.0;
			if (double.IsInfinity(heatgCalc.ResultSourceEnergyRef2) || double.IsNaN(heatgCalc.ResultSourceEnergyRef2))
			{
				heatgCalc.ResultSourceEnergyRef2 = 0.0;
			}
			heatgCalc.ResultSourceEnergy2Ref2 /= heatgCalc.TransmitTempEfficiency2Ref2 / 100.0 * (heatgCalc.SupplyNetEfficiency2Ref2 / 100.0) * (heatgCalc.Automatic2Ref2 / 100.0) * (heatgCalc.EnergyManagement2Ref2 / 100.0) * (heatgCalc.GeneratorHeatEfficiency2Ref2 / 100.0);
			if (double.IsInfinity(heatgCalc.ResultSourceEnergy2Ref2) || double.IsNaN(heatgCalc.ResultSourceEnergy2Ref2))
			{
				heatgCalc.ResultSourceEnergy2Ref2 = 0.0;
			}
			heatgCalc.ResultNeededEnergyRef2 = heatgCalc.ResultSourceEnergyRef2 + heatgCalc.ResultSourceEnergy2Ref2;
			return;
		}
		double num = heatgCalc.ResultEnergyForHeatingRef2 * heatgCalc.Part1Ref2 / 100.0;
		heatgCalc.ResultSourceEnergyRef2 = num / (heatgCalc.TransmitTempEfficiencyRef2 / 100.0 * (heatgCalc.SupplyNetEfficiencyRef2 / 100.0) * (heatgCalc.AutomaticRef2 / 100.0) * (heatgCalc.EnergyManagementRef2 / 100.0) * (heatgCalc.GeneratorHeatEfficiency1Ref2 / 100.0));
		if (double.IsInfinity(heatgCalc.ResultSourceEnergyRef2) || double.IsNaN(heatgCalc.ResultSourceEnergyRef2))
		{
			heatgCalc.ResultSourceEnergyRef2 = 0.0;
		}
		double num2 = heatgCalc.ResultEnergyForHeatingRef2 * heatgCalc.Part2Ref2 / 100.0;
		heatgCalc.ResultSourceEnergy2Ref2 = num2 / (heatgCalc.TransmitTempEfficiency2Ref2 / 100.0 * (heatgCalc.SupplyNetEfficiency2Ref2 / 100.0) * (heatgCalc.Automatic2Ref2 / 100.0) * (heatgCalc.EnergyManagement2Ref2 / 100.0) * (heatgCalc.GeneratorHeatEfficiency2Ref2 / 100.0));
		if (double.IsInfinity(heatgCalc.ResultSourceEnergy2Ref2) || double.IsNaN(heatgCalc.ResultSourceEnergy2Ref2))
		{
			heatgCalc.ResultSourceEnergy2Ref2 = 0.0;
		}
		heatgCalc.ResultNeededEnergyRef2 = heatgCalc.ResultSourceEnergyRef2 + heatgCalc.ResultSourceEnergy2Ref2;
	}

	public static void CalculateVentNeededEnergyActual(this CalculationData heatgCalc)
	{
		if (heatgCalc.SecondRecEfficiencyActual > 100.0)
		{
			heatgCalc.ResultSourceEnergyActual /= heatgCalc.TransmitTempEfficiencyActual / 100.0 * (heatgCalc.SupplyNetEfficiencyActual / 100.0) * (heatgCalc.AutomaticActual / 100.0) * (heatgCalc.EnergyManagementActual / 100.0) * heatgCalc.GeneratorHeatEfficiency1Actual / 100.0;
			if (double.IsInfinity(heatgCalc.ResultSourceEnergyActual) || double.IsNaN(heatgCalc.ResultSourceEnergyActual))
			{
				heatgCalc.ResultSourceEnergyActual = 0.0;
			}
			heatgCalc.ResultSourceEnergy2Actual /= heatgCalc.TransmitTempEfficiency2Actual / 100.0 * (heatgCalc.SupplyNetEfficiency2Actual / 100.0) * (heatgCalc.Automatic2Actual / 100.0) * (heatgCalc.EnergyManagement2Actual / 100.0) * (heatgCalc.GeneratorHeatEfficiency2Actual / 100.0);
			if (double.IsInfinity(heatgCalc.ResultSourceEnergy2Actual) || double.IsNaN(heatgCalc.ResultSourceEnergy2Actual))
			{
				heatgCalc.ResultSourceEnergy2Actual = 0.0;
			}
			heatgCalc.ResultNeededEnergyActual = heatgCalc.ResultSourceEnergyActual + heatgCalc.ResultSourceEnergy2Actual;
			return;
		}
		double num = heatgCalc.ResultEnergyForHeatingActual * heatgCalc.Part1Actual / 100.0;
		heatgCalc.ResultSourceEnergyActual = num / (heatgCalc.TransmitTempEfficiencyActual / 100.0 * (heatgCalc.SupplyNetEfficiencyActual / 100.0) * (heatgCalc.AutomaticActual / 100.0) * (heatgCalc.EnergyManagementActual / 100.0) * (heatgCalc.GeneratorHeatEfficiency1Actual / 100.0));
		if (double.IsInfinity(heatgCalc.ResultSourceEnergyActual) || double.IsNaN(heatgCalc.ResultSourceEnergyActual))
		{
			heatgCalc.ResultSourceEnergyActual = 0.0;
		}
		double num2 = heatgCalc.ResultEnergyForHeatingActual * heatgCalc.Part2Actual / 100.0;
		heatgCalc.ResultSourceEnergy2Actual = num2 / (heatgCalc.TransmitTempEfficiency2Actual / 100.0 * (heatgCalc.SupplyNetEfficiency2Actual / 100.0) * (heatgCalc.Automatic2Actual / 100.0) * (heatgCalc.EnergyManagement2Actual / 100.0) * (heatgCalc.GeneratorHeatEfficiency2Actual / 100.0));
		if (double.IsInfinity(heatgCalc.ResultSourceEnergy2Actual) || double.IsNaN(heatgCalc.ResultSourceEnergy2Actual))
		{
			heatgCalc.ResultSourceEnergy2Actual = 0.0;
		}
		heatgCalc.ResultNeededEnergyActual = heatgCalc.ResultSourceEnergyActual + heatgCalc.ResultSourceEnergy2Actual;
	}

	public static void CalculateVentNeededEnergyBaseLine(this CalculationData heatgCalc)
	{
		if (heatgCalc.SecondRecEfficiencyBaseLine > 100.0)
		{
			heatgCalc.ResultSourceEnergyBaseLine /= heatgCalc.TransmitTempEfficiencyBaseLine / 100.0 * (heatgCalc.SupplyNetEfficiencyBaseLine / 100.0) * (heatgCalc.AutomaticBaseLine / 100.0) * (heatgCalc.EnergyManagementBaseLine / 100.0) * heatgCalc.GeneratorHeatEfficiency1BaseLine / 100.0;
			if (double.IsInfinity(heatgCalc.ResultSourceEnergyBaseLine) || double.IsNaN(heatgCalc.ResultSourceEnergyBaseLine))
			{
				heatgCalc.ResultSourceEnergyBaseLine = 0.0;
			}
			heatgCalc.ResultSourceEnergy2BaseLine /= heatgCalc.TransmitTempEfficiency2BaseLine / 100.0 * (heatgCalc.SupplyNetEfficiency2BaseLine / 100.0) * (heatgCalc.Automatic2BaseLine / 100.0) * (heatgCalc.EnergyManagement2BaseLine / 100.0) * (heatgCalc.GeneratorHeatEfficiency2BaseLine / 100.0);
			if (double.IsInfinity(heatgCalc.ResultSourceEnergy2BaseLine) || double.IsNaN(heatgCalc.ResultSourceEnergy2BaseLine))
			{
				heatgCalc.ResultSourceEnergy2BaseLine = 0.0;
			}
			heatgCalc.ResultNeededEnergyBaseLine = heatgCalc.ResultSourceEnergyBaseLine + heatgCalc.ResultSourceEnergy2BaseLine;
			return;
		}
		double num = heatgCalc.ResultEnergyForHeatingBaseLine * heatgCalc.Part1BaseLine / 100.0;
		heatgCalc.ResultSourceEnergyBaseLine = num / (heatgCalc.TransmitTempEfficiencyBaseLine / 100.0 * (heatgCalc.SupplyNetEfficiencyBaseLine / 100.0) * (heatgCalc.AutomaticBaseLine / 100.0) * (heatgCalc.EnergyManagementBaseLine / 100.0) * (heatgCalc.GeneratorHeatEfficiency1BaseLine / 100.0));
		if (double.IsInfinity(heatgCalc.ResultSourceEnergyBaseLine) || double.IsNaN(heatgCalc.ResultSourceEnergyBaseLine))
		{
			heatgCalc.ResultSourceEnergyBaseLine = 0.0;
		}
		double num2 = heatgCalc.ResultEnergyForHeatingBaseLine * heatgCalc.Part2BaseLine / 100.0;
		heatgCalc.ResultSourceEnergy2BaseLine = num2 / (heatgCalc.TransmitTempEfficiency2BaseLine / 100.0 * (heatgCalc.SupplyNetEfficiency2BaseLine / 100.0) * (heatgCalc.Automatic2BaseLine / 100.0) * (heatgCalc.EnergyManagement2BaseLine / 100.0) * (heatgCalc.GeneratorHeatEfficiency2BaseLine / 100.0));
		if (double.IsInfinity(heatgCalc.ResultSourceEnergy2BaseLine) || double.IsNaN(heatgCalc.ResultSourceEnergy2BaseLine))
		{
			heatgCalc.ResultSourceEnergy2BaseLine = 0.0;
		}
		heatgCalc.ResultNeededEnergyBaseLine = heatgCalc.ResultSourceEnergyBaseLine + heatgCalc.ResultSourceEnergy2BaseLine;
	}

	public static void CalculateVentNeededEnergyEsm(this CalculationData heatgCalc)
	{
		if (heatgCalc.SecondRecEfficiencyESM > 100.0)
		{
			heatgCalc.ResultSourceEnergyESM /= heatgCalc.TransmitTempEfficiencyESM / 100.0 * (heatgCalc.SupplyNetEfficiencyESM / 100.0) * (heatgCalc.AutomaticESM / 100.0) * (heatgCalc.EnergyManagementESM / 100.0) * heatgCalc.GeneratorHeatEfficiency1ESM / 100.0;
			if (double.IsInfinity(heatgCalc.ResultSourceEnergyESM) || double.IsNaN(heatgCalc.ResultSourceEnergyESM))
			{
				heatgCalc.ResultSourceEnergyESM = 0.0;
			}
			heatgCalc.ResultSourceEnergy2ESM /= heatgCalc.TransmitTempEfficiency2ESM / 100.0 * (heatgCalc.SupplyNetEfficiency2ESM / 100.0) * (heatgCalc.Automatic2ESM / 100.0) * (heatgCalc.EnergyManagement2ESM / 100.0) * (heatgCalc.GeneratorHeatEfficiency2ESM / 100.0);
			if (double.IsInfinity(heatgCalc.ResultSourceEnergy2ESM) || double.IsNaN(heatgCalc.ResultSourceEnergy2ESM))
			{
				heatgCalc.ResultSourceEnergy2ESM = 0.0;
			}
			heatgCalc.ResultNeededEnergyESM = heatgCalc.ResultSourceEnergyESM + heatgCalc.ResultSourceEnergy2ESM;
			return;
		}
		double num = heatgCalc.ResultEnergyForHeatingESM * heatgCalc.Part1ESM / 100.0;
		heatgCalc.ResultSourceEnergyESM = num / (heatgCalc.TransmitTempEfficiencyESM / 100.0 * (heatgCalc.SupplyNetEfficiencyESM / 100.0) * (heatgCalc.AutomaticESM / 100.0) * (heatgCalc.EnergyManagementESM / 100.0) * (heatgCalc.GeneratorHeatEfficiency1ESM / 100.0));
		if (double.IsInfinity(heatgCalc.ResultSourceEnergyESM) || double.IsNaN(heatgCalc.ResultSourceEnergyESM))
		{
			heatgCalc.ResultSourceEnergyESM = 0.0;
		}
		double num2 = heatgCalc.ResultEnergyForHeatingESM * heatgCalc.Part2ESM / 100.0;
		heatgCalc.ResultSourceEnergy2ESM = num2 / (heatgCalc.TransmitTempEfficiency2ESM / 100.0 * (heatgCalc.SupplyNetEfficiency2ESM / 100.0) * (heatgCalc.Automatic2ESM / 100.0) * (heatgCalc.EnergyManagement2ESM / 100.0) * (heatgCalc.GeneratorHeatEfficiency2ESM / 100.0));
		if (double.IsInfinity(heatgCalc.ResultSourceEnergy2ESM) || double.IsNaN(heatgCalc.ResultSourceEnergy2ESM))
		{
			heatgCalc.ResultSourceEnergy2ESM = 0.0;
		}
		heatgCalc.ResultNeededEnergyESM = heatgCalc.ResultSourceEnergyESM + heatgCalc.ResultSourceEnergy2ESM;
		heatgCalc.ResultNeededEnergySavings = (heatgCalc.ResultNeededEnergyBaseLine - heatgCalc.ResultNeededEnergyESM).ToString("F3");
	}

	public static void GetWeekHoursReferences(this CalculationData heatgCalc, Section section)
	{
		double num = 5.0 * section.CalcHours(section.HeatingSeasons.Ventilation.WorkBaseStart, section.HeatingSeasons.Ventilation.WorkBaseEnd);
		double num2 = section.CalcHours(section.HeatingSeasons.Ventilation.SunBaseStart, section.HeatingSeasons.Ventilation.SunBaseEnd);
		if (num2 > 0.0)
		{
			num = num2 + num;
		}
		double num3 = section.CalcHours(section.HeatingSeasons.Ventilation.SatBaseStart, section.HeatingSeasons.Ventilation.SatBaseEnd);
		if (num3 > 0.0)
		{
			num = num3 + num;
		}
		heatgCalc.WorkingScheduleRef = heatgCalc.WorkingScheduleBaseLine;
		heatgCalc.WorkingScheduleRef2 = num;
	}

	public static void GetWeekHoursActual(this CalculationData heatgCalc, Section section)
	{
		double num = 5.0 * section.CalcHours(section.HeatingSeasons.Ventilation.WorkCurrentStart, section.HeatingSeasons.Ventilation.WorkCurrentEnd);
		double num2 = section.CalcHours(section.HeatingSeasons.Ventilation.SunCurrentStart, section.HeatingSeasons.Ventilation.SunCurrentEnd);
		if (num2 > 0.0)
		{
			num = num2 + num;
		}
		double num3 = section.CalcHours(section.HeatingSeasons.Ventilation.SatCurrentStart, section.HeatingSeasons.Ventilation.SatCurrentEnd);
		if (num3 > 0.0)
		{
			num = num3 + num;
		}
		heatgCalc.WorkingScheduleActual = num;
	}

	public static void GetWeekHoursBaseLine(this CalculationData heatgCalc, Section section)
	{
		double num = 5.0 * section.CalcHours(section.HeatingSeasons.Ventilation.WorkBaseStart, section.HeatingSeasons.Ventilation.WorkBaseEnd);
		double num2 = section.CalcHours(section.HeatingSeasons.Ventilation.SunBaseStart, section.HeatingSeasons.Ventilation.SunBaseEnd);
		if (num2 > 0.0)
		{
			num = num2 + num;
		}
		double num3 = section.CalcHours(section.HeatingSeasons.Ventilation.SatBaseStart, section.HeatingSeasons.Ventilation.SatBaseEnd);
		if (num3 > 0.0)
		{
			num = num3 + num;
		}
		heatgCalc.WorkingScheduleBaseLine = num;
	}

	public static void GetWeekHoursESM(this CalculationData heatgCalc, Section section)
	{
		double num = 5.0 * section.CalcHours(section.HeatingSeasons.Ventilation.WorkEsmStart, section.HeatingSeasons.Ventilation.WorkEsmEnd);
		double num2 = section.CalcHours(section.HeatingSeasons.Ventilation.SunEsmStart, section.HeatingSeasons.Ventilation.SunEsmEnd);
		if (num2 > 0.0)
		{
			num = num2 + num;
		}
		double num3 = section.CalcHours(section.HeatingSeasons.Ventilation.SatEsmStart, section.HeatingSeasons.Ventilation.SatEsmEnd);
		if (num3 > 0.0)
		{
			num = num3 + num;
		}
		heatgCalc.WorkingScheduleESM = num;
	}

	private static double CalculateMontlyHeatEnergyRef1(CalculationData calcData, Section section, CalculationInput calcInput, HeatingCalculations ventHeatCalculations, MonthlyDays month, out double thermoPumpEnergy)
	{
		monthHours = GetMonthHoursBaseLine(month, section);
		innerTemp = CalculateAverageVentHeatTempRef1(section, ventHeatCalculations, month);
		double avgTemp = PreferencesManager.GetClimateZoneParams(calcInput.General.ClimateZone).SolarRadiation.Months[(int)month.Month].AvgTemp;
		ObservableCollection<TempHumidityPerDay> hours = PreferencesManager.GetClimateZoneParams(calcInput.General.ClimateZone).TempHumidity.Months[(int)month.Month].Hours;
		double humidity = hours.Select((TempHumidityPerDay w) => w.Humidity).Aggregate(0.0, (double current, double item) => current + item) / (double)hours.Count;
		double num = innerTemp - calcData.FirstRecEfficiencyRef1 / 100.0 * (innerTemp - avgTemp);
		double num2 = innerTemp - num + avgTemp;
		double num3 = num2;
		if (calcData.SecondRecEfficiencyRef1 > 0.0)
		{
			if ((calcData.HeatingAirDifferenceRef1 > 3.0 && calcData.HeatingAirDifferenceRef1 < 8.0) || object.Equals(calcData.HeatingAirDifferenceRef1, 3.0) || object.Equals(calcData.HeatingAirDifferenceRef1, 8.0))
			{
				double num4 = CalcEntalpia(num, humidity, PreferencesManager.GetClimateZoneParams(calcInput.General.ClimateZone).Pb);
				double num5 = CalcEntalpia(calcData.MinimumEndTemperatureRef1, humidity, PreferencesManager.GetClimateZoneParams(calcInput.General.ClimateZone).Pb);
				double num6 = calcData.DebitRef1 * 1.2 * (num4 - num5) * monthHours / 3600.0;
				thermoPumpEnergy = num6 / (1.0 - 100.0 / calcData.SecondRecEfficiencyRef1);
				double num7 = thermoPumpEnergy * 1000.0 / (calcData.DebitRef1 * 0.34 * monthHours);
				if (num7 >= calcData.HeatingAirDifferenceRef1)
				{
					thermoPumpEnergy = calcData.DebitRef1 * 0.34 * calcData.HeatingAirDifferenceRef1 * monthHours / 1000.0;
				}
				if (num7 < calcData.FlowTemperatureRef1 - num2)
				{
					num3 = calcData.FlowTemperatureRef1 - num2 - num7;
					return calcData.DebitRef1 * 0.34 * (calcData.FlowTemperatureRef1 - num3) * monthHours / 1000.0;
				}
				thermoPumpEnergy = calcData.DebitRef1 * 0.34 * (calcData.FlowTemperatureRef1 - num2) * monthHours / 1000.0;
				return 0.0;
			}
			thermoPumpEnergy = 0.0;
			return 0.0;
		}
		thermoPumpEnergy = 0.0;
		return calcData.DebitRef1 * 0.34 * (calcData.FlowTemperatureRef1 - num3) * monthHours / 1000.0;
	}

	private static double CalculateMontlyHeatEnergyRef2(CalculationData calcData, Section section, CalculationInput calcInput, HeatingCalculations ventHeatCalculations, MonthlyDays month, out double thermoPumpEnergy)
	{
		monthHours = GetMonthHoursBaseLine(month, section);
		innerTemp = CalculateAverageVentHeatTempRef2(section, ventHeatCalculations, month);
		double avgTemp = PreferencesManager.GetClimateZoneParams(calcInput.General.ClimateZone).SolarRadiation.Months[(int)month.Month].AvgTemp;
		ObservableCollection<TempHumidityPerDay> hours = PreferencesManager.GetClimateZoneParams(calcInput.General.ClimateZone).TempHumidity.Months[(int)month.Month].Hours;
		double humidity = hours.Select((TempHumidityPerDay w) => w.Humidity).Aggregate(0.0, (double current, double item) => current + item) / (double)hours.Count;
		double num = innerTemp - calcData.FirstRecEfficiencyRef2 / 100.0 * (innerTemp - avgTemp);
		double num2 = innerTemp - num + avgTemp;
		double num3 = num2;
		if (calcData.SecondRecEfficiencyRef2 > 0.0)
		{
			if ((calcData.HeatingAirDifferenceRef2 > 3.0 && calcData.HeatingAirDifferenceRef2 < 8.0) || object.Equals(calcData.HeatingAirDifferenceRef2, 3.0) || object.Equals(calcData.HeatingAirDifferenceRef2, 8.0))
			{
				double num4 = CalcEntalpia(num, humidity, PreferencesManager.GetClimateZoneParams(calcInput.General.ClimateZone).Pb);
				double num5 = CalcEntalpia(calcData.MinimumEndTemperatureRef2, humidity, PreferencesManager.GetClimateZoneParams(calcInput.General.ClimateZone).Pb);
				double num6 = calcData.DebitRef2 * 1.2 * (num4 - num5) * monthHours / 3600.0;
				thermoPumpEnergy = num6 / (1.0 - 100.0 / calcData.SecondRecEfficiencyRef2);
				double num7 = thermoPumpEnergy * 1000.0 / (calcData.DebitRef2 * 0.34 * monthHours);
				if (num7 >= calcData.HeatingAirDifferenceRef2)
				{
					thermoPumpEnergy = calcData.DebitRef2 * 0.34 * calcData.HeatingAirDifferenceRef2 * monthHours / 1000.0;
				}
				if (num7 < calcData.FlowTemperatureRef2 - num2)
				{
					num3 = calcData.FlowTemperatureRef2 - num2 - num7;
					return calcData.DebitRef2 * 0.34 * (calcData.FlowTemperatureRef2 - num3) * monthHours / 1000.0;
				}
				thermoPumpEnergy = calcData.DebitRef2 * 0.34 * (calcData.FlowTemperatureRef2 - num2) * monthHours / 1000.0;
				return 0.0;
			}
			thermoPumpEnergy = 0.0;
			return 0.0;
		}
		thermoPumpEnergy = 0.0;
		return calcData.DebitRef2 * 0.34 * (calcData.FlowTemperatureRef2 - num3) * monthHours / 1000.0;
	}

	private static double CalculateMontlyHeatEnergyActual(CalculationData calcData, Section section, CalculationInput calcInput, HeatingCalculations heatCalculations, MonthlyDays month, out double thermoPumpEnergy)
	{
		monthHours = GetMonthHoursActual(month, section);
		innerTemp = CalculateAverageVentHeatTempActual(section, heatCalculations, month);
		double avgTemp = PreferencesManager.GetClimateZoneParams(calcInput.General.ClimateZone).SolarRadiation.Months[(int)month.Month].AvgTemp;
		ObservableCollection<TempHumidityPerDay> hours = PreferencesManager.GetClimateZoneParams(calcInput.General.ClimateZone).TempHumidity.Months[(int)month.Month].Hours;
		double humidity = hours.Select((TempHumidityPerDay w) => w.Humidity).Aggregate(0.0, (double current, double item) => current + item) / (double)hours.Count;
		double num = innerTemp - calcData.FirstRecEfficiencyActual / 100.0 * (innerTemp - avgTemp);
		double num2 = innerTemp - num + avgTemp;
		double num3 = num2;
		if (calcData.SecondRecEfficiencyActual > 0.0)
		{
			if ((calcData.HeatingAirDifferenceActual > 3.0 && calcData.HeatingAirDifferenceActual < 8.0) || object.Equals(calcData.HeatingAirDifferenceActual, 3.0) || object.Equals(calcData.HeatingAirDifferenceActual, 8.0))
			{
				double num4 = CalcEntalpia(num, humidity, PreferencesManager.GetClimateZoneParams(calcInput.General.ClimateZone).Pb);
				double num5 = CalcEntalpia(calcData.MinimumEndTemperatureActual, humidity, PreferencesManager.GetClimateZoneParams(calcInput.General.ClimateZone).Pb);
				double num6 = calcData.DebitActual * 1.2 * (num4 - num5) * monthHours / 3600.0;
				thermoPumpEnergy = num6 / (1.0 - 100.0 / calcData.SecondRecEfficiencyActual);
				double num7 = thermoPumpEnergy * 1000.0 / (calcData.DebitActual * 0.34 * monthHours);
				if (num7 >= calcData.HeatingAirDifferenceActual)
				{
					thermoPumpEnergy = calcData.DebitActual * 0.34 * calcData.HeatingAirDifferenceActual * monthHours / 1000.0;
				}
				if (num7 < calcData.FlowTemperatureActual - num2)
				{
					num3 = calcData.FlowTemperatureActual - num2 - num7;
					return calcData.DebitActual * 0.34 * (calcData.FlowTemperatureActual - num3) * monthHours / 1000.0;
				}
				thermoPumpEnergy = calcData.DebitActual * 0.34 * (calcData.FlowTemperatureActual - num2) * monthHours / 1000.0;
				return 0.0;
			}
			thermoPumpEnergy = 0.0;
			return 0.0;
		}
		thermoPumpEnergy = 0.0;
		return calcData.DebitActual * 0.34 * (calcData.FlowTemperatureActual - num3) * monthHours / 1000.0;
	}

	private static double CalculateMontlyHeatEnergyBaseLine(CalculationData calcData, Section section, CalculationInput calcInput, HeatingCalculations heatCalculations, MonthlyDays month, out double thermoPumpEnergy)
	{
		monthHours = GetMonthHoursBaseLine(month, section);
		innerTemp = CalculateAverageVentHeatTempBaseLine(section, heatCalculations, month);
		double avgTemp = PreferencesManager.GetClimateZoneParams(calcInput.General.ClimateZone).SolarRadiation.Months[(int)month.Month].AvgTemp;
		ObservableCollection<TempHumidityPerDay> hours = PreferencesManager.GetClimateZoneParams(calcInput.General.ClimateZone).TempHumidity.Months[(int)month.Month].Hours;
		double humidity = hours.Select((TempHumidityPerDay w) => w.Humidity).Aggregate(0.0, (double current, double item) => current + item) / (double)hours.Count;
		double num = innerTemp - calcData.FirstRecEfficiencyBaseLine / 100.0 * (innerTemp - avgTemp);
		double num2 = innerTemp - num + avgTemp;
		double num3 = num2;
		if (calcData.SecondRecEfficiencyBaseLine > 0.0)
		{
			if ((calcData.HeatingAirDifferenceBaseLine > 3.0 && calcData.HeatingAirDifferenceBaseLine < 8.0) || object.Equals(calcData.HeatingAirDifferenceBaseLine, 3.0) || object.Equals(calcData.HeatingAirDifferenceBaseLine, 8.0))
			{
				double num4 = CalcEntalpia(num, humidity, PreferencesManager.GetClimateZoneParams(calcInput.General.ClimateZone).Pb);
				double num5 = CalcEntalpia(calcData.MinimumEndTemperatureBaseLine, humidity, PreferencesManager.GetClimateZoneParams(calcInput.General.ClimateZone).Pb);
				double num6 = calcData.DebitBaseLine * 1.2 * (num4 - num5) * monthHours / 3600.0;
				thermoPumpEnergy = num6 / (1.0 - 100.0 / calcData.SecondRecEfficiencyBaseLine);
				double num7 = thermoPumpEnergy * 1000.0 / (calcData.DebitBaseLine * 0.34 * monthHours);
				if (num7 >= calcData.HeatingAirDifferenceBaseLine)
				{
					thermoPumpEnergy = calcData.DebitBaseLine * 0.34 * calcData.HeatingAirDifferenceBaseLine * monthHours / 1000.0;
				}
				if (num7 < calcData.FlowTemperatureBaseLine - num2)
				{
					num3 = calcData.FlowTemperatureBaseLine - num2 - num7;
					return calcData.DebitBaseLine * 0.34 * (calcData.FlowTemperatureBaseLine - num3) * monthHours / 1000.0;
				}
				thermoPumpEnergy = calcData.DebitBaseLine * 0.34 * (calcData.FlowTemperatureBaseLine - num2) * monthHours / 1000.0;
				return 0.0;
			}
			thermoPumpEnergy = 0.0;
			return 0.0;
		}
		thermoPumpEnergy = 0.0;
		return calcData.DebitBaseLine * 0.34 * (calcData.FlowTemperatureBaseLine - num3) * monthHours / 1000.0;
	}

	private static double CalculateMontlyHeatEnergyESM(CalculationData calcData, Section section, CalculationInput calcInput, HeatingCalculations ventHeatCalculations, MonthlyDays month, out double thermoPumpEnergy)
	{
		monthHours = GetMonthHoursESM(month, section);
		innerTemp = CalculateAverageVentHeatTempESM(section, ventHeatCalculations, month);
		double avgTemp = PreferencesManager.GetClimateZoneParams(calcInput.General.ClimateZone).SolarRadiation.Months[(int)month.Month].AvgTemp;
		ObservableCollection<TempHumidityPerDay> hours = PreferencesManager.GetClimateZoneParams(calcInput.General.ClimateZone).TempHumidity.Months[(int)month.Month].Hours;
		double humidity = hours.Select((TempHumidityPerDay w) => w.Humidity).Aggregate(0.0, (double current, double item) => current + item) / (double)hours.Count;
		double num = innerTemp - calcData.FirstRecEfficiencyESM / 100.0 * (innerTemp - avgTemp);
		double num2 = innerTemp - num + avgTemp;
		double num3 = num2;
		if (calcData.SecondRecEfficiencyESM > 0.0)
		{
			if ((calcData.HeatingAirDifferenceESM > 3.0 && calcData.HeatingAirDifferenceESM < 8.0) || object.Equals(calcData.HeatingAirDifferenceESM, 3.0) || object.Equals(calcData.HeatingAirDifferenceESM, 8.0))
			{
				double num4 = CalcEntalpia(num, humidity, PreferencesManager.GetClimateZoneParams(calcInput.General.ClimateZone).Pb);
				double num5 = CalcEntalpia(calcData.MinimumEndTemperatureESM, humidity, PreferencesManager.GetClimateZoneParams(calcInput.General.ClimateZone).Pb);
				double num6 = calcData.DebitESM * 1.2 * (num4 - num5) * monthHours / 3600.0;
				thermoPumpEnergy = num6 / (1.0 - 100.0 / calcData.SecondRecEfficiencyESM);
				double num7 = thermoPumpEnergy * 1000.0 / (calcData.DebitESM * 0.34 * monthHours);
				if (num7 >= calcData.HeatingAirDifferenceESM)
				{
					thermoPumpEnergy = calcData.DebitESM * 0.34 * calcData.HeatingAirDifferenceESM * monthHours / 1000.0;
				}
				if (num7 < calcData.FlowTemperatureESM - num2)
				{
					num3 = calcData.FlowTemperatureESM - num2 - num7;
					return calcData.DebitESM * 0.34 * (calcData.FlowTemperatureESM - num3) * monthHours / 1000.0;
				}
				thermoPumpEnergy = calcData.DebitESM * 0.34 * (calcData.FlowTemperatureESM - num2) * monthHours / 1000.0;
				return 0.0;
			}
			thermoPumpEnergy = 0.0;
			return 0.0;
		}
		thermoPumpEnergy = 0.0;
		return calcData.DebitESM * 0.34 * (calcData.FlowTemperatureESM - num3) * monthHours / 1000.0;
	}

	private static double CalculateAverageVentHeatTempRef1(Section section, HeatingCalculations heatCalcData, MonthlyDays month)
	{
		return CalculateAverageVentHeatTempBaseLine(section, heatCalcData, month);
	}

	private static double CalculateAverageVentHeatTempRef2(Section section, HeatingCalculations heatCalcData, MonthlyDays month)
	{
		return CalculateAverageVentHeatTempBaseLine(section, heatCalcData, month);
	}

	private static double CalculateAverageVentHeatTempActual(Section section, HeatingCalculations heatCalcData, MonthlyDays month)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		for (int i = section.HeatingSeasons.Ventilation.WorkCurrentStart; i < section.HeatingSeasons.Ventilation.WorkCurrentEnd; i++)
		{
			if (section.HeatingSeasons.Heating.WorkCurrentStart <= i && section.HeatingSeasons.Heating.WorkCurrentEnd > i)
			{
				num2++;
			}
			else
			{
				num3++;
			}
		}
		int num4 = num2 * month.WorkDays;
		int num5 = (num + num3) * month.WorkDays;
		num = 0;
		num2 = 0;
		num3 = 0;
		for (int j = section.HeatingSeasons.Ventilation.SatCurrentStart; j < section.HeatingSeasons.Ventilation.SatCurrentEnd; j++)
		{
			if (section.HeatingSeasons.Heating.SatCurrentStart <= j && section.HeatingSeasons.Heating.SatCurrentEnd > j)
			{
				num2++;
			}
			else
			{
				num3++;
			}
		}
		num4 += num2 * month.Saturdays;
		num5 += (num + num3) * month.Saturdays;
		num = 0;
		num2 = 0;
		num3 = 0;
		for (int k = section.HeatingSeasons.Ventilation.SunCurrentStart; k < section.HeatingSeasons.Ventilation.SunCurrentEnd; k++)
		{
			if (section.HeatingSeasons.Heating.SunCurrentStart <= k && section.HeatingSeasons.Heating.SunCurrentEnd > k)
			{
				num2++;
			}
			else
			{
				num3++;
			}
		}
		num4 += num2 * month.Sundays;
		num5 += (num + num3) * month.Sundays;
		double projectTemperatureActual = heatCalcData.HeatingResult.ProjectTemperatureActual;
		double nonProjectTemperatureActual = heatCalcData.HeatingResult.NonProjectTemperatureActual;
		return ((double)num4 * projectTemperatureActual + (double)num5 * nonProjectTemperatureActual) / (double)(num4 + num5);
	}

	private static double CalculateAverageVentHeatTempBaseLine(Section section, HeatingCalculations heatCalcData, MonthlyDays month)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		for (int i = section.HeatingSeasons.Ventilation.WorkBaseStart; i < section.HeatingSeasons.Ventilation.WorkBaseEnd; i++)
		{
			if (section.HeatingSeasons.Heating.WorkBaseStart <= i && section.HeatingSeasons.Heating.WorkBaseEnd > i)
			{
				num2++;
			}
			else
			{
				num3++;
			}
		}
		int num4 = num2 * month.WorkDays;
		int num5 = (num + num3) * month.WorkDays;
		num = 0;
		num2 = 0;
		num3 = 0;
		for (int j = section.HeatingSeasons.Ventilation.SatBaseStart; j < section.HeatingSeasons.Ventilation.SatBaseEnd; j++)
		{
			if (section.HeatingSeasons.Heating.SatBaseStart <= j && section.HeatingSeasons.Heating.SatBaseEnd > j)
			{
				num2++;
			}
			else
			{
				num3++;
			}
		}
		num4 += num2 * month.Saturdays;
		num5 += (num + num3) * month.Saturdays;
		num = 0;
		num2 = 0;
		num3 = 0;
		for (int k = section.HeatingSeasons.Ventilation.SunBaseStart; k < section.HeatingSeasons.Ventilation.SunBaseEnd; k++)
		{
			if (section.HeatingSeasons.Heating.SunBaseStart <= k && section.HeatingSeasons.Heating.SunBaseEnd > k)
			{
				num2++;
			}
			else
			{
				num3++;
			}
		}
		num4 += num2 * month.Sundays;
		num5 += (num + num3) * month.Sundays;
		double projectTemperatureBaseLine = heatCalcData.HeatingResult.ProjectTemperatureBaseLine;
		double nonProjectTemperatureBaseLine = heatCalcData.HeatingResult.NonProjectTemperatureBaseLine;
		return ((double)num4 * projectTemperatureBaseLine + (double)num5 * nonProjectTemperatureBaseLine) / (double)(num4 + num5);
	}

	private static double CalculateAverageVentHeatTempESM(Section section, HeatingCalculations heatCalcData, MonthlyDays month)
	{
		int num = 0;
		int num2 = 0;
		int num3 = 0;
		for (int i = section.HeatingSeasons.Ventilation.WorkEsmStart; i < section.HeatingSeasons.Ventilation.WorkEsmEnd; i++)
		{
			if (section.HeatingSeasons.Heating.WorkEsmStart <= i && section.HeatingSeasons.Heating.WorkEsmEnd > i)
			{
				num2++;
			}
			else
			{
				num3++;
			}
		}
		int num4 = num2 * month.WorkDays;
		int num5 = (num + num3) * month.WorkDays;
		num = 0;
		num2 = 0;
		num3 = 0;
		for (int j = section.HeatingSeasons.Ventilation.SatEsmStart; j < section.HeatingSeasons.Ventilation.SatEsmEnd; j++)
		{
			if (section.HeatingSeasons.Heating.SatEsmStart <= j && section.HeatingSeasons.Heating.SatEsmEnd > j)
			{
				num2++;
			}
			else
			{
				num3++;
			}
		}
		num4 += num2 * month.Saturdays;
		num5 += (num + num3) * month.Saturdays;
		num = 0;
		num2 = 0;
		num3 = 0;
		for (int k = section.HeatingSeasons.Ventilation.SunEsmStart; k < section.HeatingSeasons.Ventilation.SunEsmEnd; k++)
		{
			if (section.HeatingSeasons.Heating.SunEsmStart <= k && section.HeatingSeasons.Heating.SunEsmEnd > k)
			{
				num2++;
			}
			else
			{
				num3++;
			}
		}
		num4 += num2 * month.Sundays;
		num5 += (num + num3) * month.Sundays;
		double projectTemperatureESM = heatCalcData.HeatingResult.ProjectTemperatureESM;
		double nonProjectTemperatureESM = heatCalcData.HeatingResult.NonProjectTemperatureESM;
		return ((double)num4 * projectTemperatureESM + (double)num5 * nonProjectTemperatureESM) / (double)(num4 + num5);
	}

	private static double GetMonthHoursActual(MonthlyDays month, Section section)
	{
		int num = month.WorkDays * (section.HeatingSeasons.Ventilation.WorkCurrentEnd - section.HeatingSeasons.Ventilation.WorkCurrentStart);
		num = ((month.Sundays > 0) ? (month.Sundays * (section.HeatingSeasons.Ventilation.SunCurrentEnd - section.HeatingSeasons.Ventilation.SunCurrentStart) + num) : num);
		num = ((month.Saturdays > 0) ? (month.Saturdays * (section.HeatingSeasons.Ventilation.SatCurrentEnd - section.HeatingSeasons.Ventilation.SatCurrentStart) + num) : num);
		return num;
	}

	private static double GetMonthHoursBaseLine(MonthlyDays month, Section section)
	{
		int num = month.WorkDays * (section.HeatingSeasons.Ventilation.WorkBaseEnd - section.HeatingSeasons.Ventilation.WorkBaseStart);
		num = ((month.Sundays > 0) ? (month.Sundays * (section.HeatingSeasons.Ventilation.SunBaseEnd - section.HeatingSeasons.Ventilation.SunBaseStart) + num) : num);
		num = ((month.Saturdays > 0) ? (month.Saturdays * (section.HeatingSeasons.Ventilation.SatBaseEnd - section.HeatingSeasons.Ventilation.SatBaseStart) + num) : num);
		return num;
	}

	private static double GetMonthHoursESM(MonthlyDays month, Section section)
	{
		int num = month.WorkDays * (section.HeatingSeasons.Ventilation.WorkEsmEnd - section.HeatingSeasons.Ventilation.WorkEsmStart);
		num = ((month.Sundays > 0) ? (month.Sundays * (section.HeatingSeasons.Ventilation.SunEsmEnd - section.HeatingSeasons.Ventilation.SunEsmStart) + num) : num);
		num = ((month.Saturdays > 0) ? (month.Saturdays * (section.HeatingSeasons.Ventilation.SatEsmEnd - section.HeatingSeasons.Ventilation.SatEsmStart) + num) : num);
		return num;
	}

	private static double CalcEntalpia(double temp, double humidity, double pb)
	{
		double num = 273.15 + temp;
		double num2 = Math.Pow(2.718281828459, 77.345 + 0.0057 * num - 7235.0 / num) / Math.Pow(num, 8.2);
		double num3 = humidity * num2 / 100.0;
		double num4 = 0.62198 * (num3 / (pb - num3));
		return 1.006 * temp + num4 * (1.805 * temp + 2501.0);
	}
}
