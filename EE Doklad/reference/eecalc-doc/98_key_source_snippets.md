# 98 — Key source snippets

Този файл съдържа директни декомпилирани тела на ключови методи за reference.


## CalculateActual — lines 3482-3492

```csharp

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
```


## CalculateParameterQtr — lines 3692-3700

```csharp

	private static double CalculateParameterQtr(CalculationData calculationData, Section section, ClimateZones climateZone, MonthlyDays month, out double parameterHtr)
	{
		double avgTemp = PreferencesManager.GetClimateZoneParams(climateZone).SolarRadiation.Months[(int)month.Month].AvgTemp;
		double averageInnerHeatTemp = CalculateAverageHeatTempCurrent(section, calculationData, month);
		parameterHtr = CalculateParameterHtr(section, avgTemp, averageInnerHeatTemp);
		section.Test.ParameterHtr = parameterHtr;
		return section.Test.ParameterHtr * (CalcAvgProjectTemp(section, climateZone, calculationData, month) + CalcAvgNonProjectTemp(section, climateZone, calculationData, month)) / 1000.0;
	}
```


## CalculateParameterQve — lines 3662-3666

```csharp

	private static double CalculateParameterQve(Section section, ClimateZones climateZone, CalculationData calculationData, MonthlyDays month)
	{
		return CalcParameterHve(section, calculationData) * (CalcAvgProjectTemp(section, climateZone, calculationData, month) + CalcAvgNonProjectTemp(section, climateZone, calculationData, month)) / 1000.0;
	}
```


## CalcParameterHve — lines 3686-3691

```csharp

	private static double CalcParameterHve(Section section, CalculationData calculationData)
	{
		section.Test.ParameterHve = section.Area.HeatedVolume * calculationData.InfiltracionActual * 0.34;
		return section.Test.ParameterHve;
	}
```


## CalcAvgProjectTemp — lines 3667-3675

```csharp

	private static double CalcAvgProjectTemp(Section section, ClimateZones climateZone, CalculationData calculationData, MonthlyDays month)
	{
		double avgTemp = PreferencesManager.GetClimateZoneParams(climateZone).SolarRadiation.Months[(int)month.Month].AvgTemp;
		int num = month.WorkDays * (section.HeatingSeasons.Heating.WorkCurrentEnd - section.HeatingSeasons.Heating.WorkCurrentStart);
		int num2 = month.Sundays * (section.HeatingSeasons.Heating.SunCurrentEnd - section.HeatingSeasons.Heating.SunCurrentStart);
		int num3 = month.Saturdays * (section.HeatingSeasons.Heating.SatCurrentEnd - section.HeatingSeasons.Heating.SatCurrentStart);
		return (calculationData.ProjectTemperatureActual - avgTemp) * (double)(num + num3 + num2);
	}
```


## CalcAvgNonProjectTemp — lines 3676-3685

```csharp

	private static double CalcAvgNonProjectTemp(Section section, ClimateZones climateZone, CalculationData calculationData, MonthlyDays month)
	{
		double avgTemp = PreferencesManager.GetClimateZoneParams(climateZone).SolarRadiation.Months[(int)month.Month].AvgTemp;
		int num = month.WorkDays * (24 - (section.HeatingSeasons.Heating.WorkCurrentEnd - section.HeatingSeasons.Heating.WorkCurrentStart));
		int num2 = month.Saturdays * (24 - (section.HeatingSeasons.Heating.SatCurrentEnd - section.HeatingSeasons.Heating.SatCurrentStart));
		int num3 = month.Sundays * (24 - (section.HeatingSeasons.Heating.SunCurrentEnd - section.HeatingSeasons.Heating.SunCurrentStart));
		int num4 = month.Holydays * 24;
		return (calculationData.NonProjectTemperatureActual - avgTemp) * (double)(num + num2 + num3 + num4);
	}
```


## CalculateParameterQgn — lines 3940-3951

```csharp

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
```


## CalculateParameterNign — lines 3631-3648

```csharp

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
```


## CalculateaH — lines 3649-3661

```csharp

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
```


## CalculateParameterHtr — lines 3701-3711

```csharp

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
```


## CalculateAverageHeatTempCurrent — lines 3823-3836

```csharp

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
```


## CalculateETA — lines 984-1002

```csharp

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
```


## CalculateAc — lines 1023-1032

```csharp

	private static double CalculateAc(CalculationData calculationdata, Section section, ClimateZones climateZone, MonthlyDays month)
	{
		double avgTemp = PreferencesManager.GetClimateZoneParams(climateZone).SolarRadiation.Months[(int)month.Month].AvgTemp;
		double averageInnerCoolTemp = CalculateAverageCoolingTempCurrent(section, calculationdata, month);
		double num = CalculateCoolingHtr(section, avgTemp, averageInnerCoolTemp);
		double num2 = CalculateHinf(section, calculationdata);
		double num3 = section.Area.HeatedArea * section.Area.HeatCapacity / (num + num2);
		return 1.0 + num3 / 15.0;
	}
```


## CalculateQve — lines 1555-1619

```csharp

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
```


## CalculateQinf — lines 1857-1862

```csharp

	private static double CalculateQinf(Section section, ClimateZones climateZone, CalculationData calcData, MonthlyDays month)
	{
		double avgTemp = PreferencesManager.GetClimateZoneParams(climateZone).SolarRadiation.Months[(int)month.Month].AvgTemp;
		return CalculateHinf(section, calcData) * (CalcAvgProjectTempCooling(section, avgTemp, calcData, month) + CalcAvgNonProjectTempCooling(section, avgTemp, calcData, month)) / 1000.0;
	}
```
