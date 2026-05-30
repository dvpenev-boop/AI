# Source Binding Audit: Heating, Cooling, Ventilation

Scope: existing EECalc reverse-engineering analyses for heating R1-R5, cooling R6, and ventilation R7. This audit documents data-source bindings only. No oracle, production code, XML, or JSON changes were made.

Primary code sources:

- `reference/eecalc-decompiled/EECalcCore.Calculations.PreferencesManager.cs`
- `reference/eecalc-decompiled/EECalcCore.Calculations.SunEnergyPreferencesManager.cs`
- `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs`
- `reference/eecalc-decompiled/EECalcCore.Calculations.InputDataCalc.cs`

Configuration sources:

- `reference/eecalc-config/DefaultParams.xml`
- `reference/eecalc-config/DefaultSunParams.xml`

## 1. Summary

EECalc has two different XML-backed climate/solar configuration paths:

| Manager | XML file | Returned object | Used by analyzed heating/cooling/ventilation paths |
| --- | --- | --- | --- |
| `PreferencesManager` | `Xml/DefaultParams.xml` | `EECalcCore.Preferences.ClimateZone` | Yes |
| `SunEnergyPreferencesManager` | `Xml/DefaultSunParams.xml` | `EECalcCore.SunPreferences.ClimateZone` | No, except solar/DHW cross-reference |

For R1-R7, monthly outdoor average temperature, orientation solar radiation, hourly temperature/humidity, and barometric pressure are bound through:

```text
PreferencesManager.GetClimateZoneParams(calcInput.General.ClimateZone)
```

Therefore these values come from `DefaultParams.xml`, not `DefaultSunParams.xml`.

`DefaultSunParams.xml` is used by the solar hot-water / `SunEnergyCalculations` path through:

```text
SunEnergyPreferencesManager.GetClimateZoneParams(calcInput.General.ClimateZone)
```

## 2. DefaultParams.xml usage

Loader:

```text
PreferencesManager static ctor:
  Path.Combine(Application.StartupPath, "Xml/DefaultParams.xml")
  EntityBase<Parameters>.LoadFromFile(fileName)

GetClimateZoneParams(zone):
  Parameters.ClimateZones.Single(z => z.Number == (int)zone)
```

Source binding:

| XML node | C# property path | Formula/use |
| --- | --- | --- |
| `/Parameters/ClimateZones/ClimateZone/Number` | `ClimateZone.Number` | selected by `calcInput.General.ClimateZone`; XML `Number` is zero-based |
| `/Parameters/ClimateZones/ClimateZone/Title` | `ClimateZone.Title` | label only |
| `/Parameters/ClimateZones/ClimateZone/Pb` | `ClimateZone.Pb` | ventilation heating enthalpy via `CalcEntalpia(..., Pb)` |
| `/Parameters/ClimateZones/ClimateZone/Tdes` | `ClimateZone.Tdes` | not used by the R1-R7 methods audited here |
| `/Parameters/ClimateZones/ClimateZone/deltaTer` | `ClimateZone.deltaTer` | not used by the R1-R7 methods audited here |
| `/Parameters/ClimateZones/ClimateZone/HeatingSeason` | `ClimateZone.HeatingSeason` | not the active source for R1 `MonthlyDays`; R1 uses section calculation-period inputs |
| `/Parameters/ClimateZones/ClimateZone/CoolingSeason` | `ClimateZone.CoolingSeason` | not the active source for R6/R7 `MonthlyDays`; cooling period is passed through section/calc-period inputs |
| `/Parameters/ClimateZones/ClimateZone/SolarRadiation/Months/Month/AvgTemp` | `ClimateZone.SolarRadiation.Months[month].AvgTemp` | heating Qtr/Qve/aH, cooling Qtr/Qinf/Ac, ventilation heating monthly average temperature, ETLine writes |
| `/Parameters/ClimateZones/ClimateZone/SolarRadiation/Months/Month/N` | `SolarRadiationPerMonth.N` | north-facing heating/cooling solar gains |
| `/Parameters/ClimateZones/ClimateZone/SolarRadiation/Months/Month/E` | `SolarRadiationPerMonth.E` | east-facing heating/cooling solar gains |
| `/Parameters/ClimateZones/ClimateZone/SolarRadiation/Months/Month/S` | `SolarRadiationPerMonth.S` | south-facing heating/cooling solar gains |
| `/Parameters/ClimateZones/ClimateZone/SolarRadiation/Months/Month/W` | `SolarRadiationPerMonth.W` | west-facing heating/cooling solar gains |
| `/Parameters/ClimateZones/ClimateZone/SolarRadiation/Months/Month/H` | `SolarRadiationPerMonth.H` | horizontal/roof heating/cooling solar gains |
| `/Parameters/ClimateZones/ClimateZone/TempHumidity/Months/Month/Hours/Temp` | `TempHumidity.Months[month].Hours[hour].Temp` | cooling latent/free-cooling, ventilation cooling, ventilation withering |
| `/Parameters/ClimateZones/ClimateZone/TempHumidity/Months/Month/Hours/Humidity` | `TempHumidity.Months[month].Hours[hour].Humidity` | cooling latent, ventilation heating average humidity, ventilation cooling/withering |

Climate zone mapping:

```text
calcInput.General.ClimateZone enum value == DefaultParams.xml ClimateZone.Number
```

The public ordinance/json zone ids used in current data are one-based. For comparisons against `EE.Doklad/Data/climate_zones.json`, use:

```text
Json ZoneId = DefaultParams.xml Number + 1
```

## 3. DefaultSunParams.xml usage

Loader:

```text
SunEnergyPreferencesManager static ctor:
  Path.Combine(Application.StartupPath, "Xml/DefaultSunParams.xml")
  EntityBase<SunParameters>.LoadFromFile(fileName)

GetClimateZoneParams(zone):
  SunParameters.ClimateZones.Single(z => z.Number == (int)zone)
```

Source binding:

| XML node | C# property path | Formula/use |
| --- | --- | --- |
| `/SunParameters/ClimateZones/ClimateZone/Number` | `SunPreferences.ClimateZone.Number` | selected by `calcInput.General.ClimateZone`; zero-based |
| `/SunParameters/ClimateZones/ClimateZone/SolarRadiation/Months/Month/AvgTemp` | `SunPreferences.SolarRadiation.Months[month].AvgTemp` | solar hot-water `SunMonth.Tm` |
| `/SunParameters/ClimateZones/ClimateZone/SolarRadiation/Months/Month/Radiation` | `SunPreferences.SolarRadiation.Months[month].Radiation` | solar hot-water `SunMonth.H`, `CalculateParameterHtMonthly` |
| `/SunParameters/ClimateZones/ClimateZone/SolarRadiation/Months/Month/Cloudiness` | `SunPreferences.SolarRadiation.Months[month].Cloudiness` | solar hot-water diffuse/cloudiness correction |

`DefaultSunParams.xml` does not supply `N/E/S/W/H`, hourly `TempHumidity`, or `Pb`.

## 4. Heating source binding

### AvgTemp

Heating R2/R3/R5 uses monthly outdoor average temperature from `DefaultParams.xml`:

```text
PreferencesManager.GetClimateZoneParams(climateZone)
  .SolarRadiation.Months[(int)month.Month].AvgTemp
```

Observed uses include:

- R2 degree-hour helpers: `CalcAvgProjectTemp*`, `CalcAvgNonProjectTemp*`.
- R3 `CalculateParameterQtr*` and `CalculateParameterHtr*` dependencies.
- R5 `CalculateaH*` for Actual/BaseLine/ESM/Ref1/Ref2.
- Actual monthly `MonthData.AvgTemp` assignment.

### SolarRadiation N/E/S/W/H

Heating R4 `Qgn` solar gains use `DefaultParams.xml`:

```text
PreferencesManager.GetClimateZoneParams(climateZone)
  .SolarRadiation.Months[(int)month.Month]
```

Mapping:

| Formula input | XML node |
| --- | --- |
| `solarRadiationPerMonth.N` | `DefaultParams.xml` `SolarRadiation/Months/Month/N` |
| `solarRadiationPerMonth.E` | `DefaultParams.xml` `SolarRadiation/Months/Month/E` |
| `solarRadiationPerMonth.S` | `DefaultParams.xml` `SolarRadiation/Months/Month/S` |
| `solarRadiationPerMonth.W` | `DefaultParams.xml` `SolarRadiation/Months/Month/W` |
| `solarRadiationPerMonth.H` | `DefaultParams.xml` `SolarRadiation/Months/Month/H` |
| NE/SE/SW/NW orientations | arithmetic averages `(N+E)/2`, `(S+E)/2`, `(S+W)/2`, `(N+W)/2` |

Section-bound inputs used with this radiation:

- wall/roof transparent areas, g-values, and emissivities from `Section.*Walls.*` and `Section.Roof.*`.
- wall/roof opaque alpha, U, emissivity, and area from `Section.*Walls.*` and `Section.Roof.*`.

Hardcoded constants in solar-gain primitives:

- `0.04`
- `11`
- `4`
- `0.0000000567`
- `283^3`
- vertical loss factor `0.5`
- horizontal loss factor `1.0`

### Humidity if used

Core heating R1-R5 Qtr/Qve/Qgn/Gamma/Ni does not use outdoor humidity.

Heating ventilation in R7 uses humidity from `DefaultParams.xml` hourly `TempHumidity` by averaging all hourly humidity values in the month.

### Climate zone mapping

Heating uses `calcInput.General.ClimateZone` as the zero-based EECalc `ClimateZone.Number`.

`MonthlyDays` for R1 is not sourced from XML climate seasons. It is calculated from section/calc-period inputs by `InputDataCalc.CalcPeriod` and `InputDataCalc.CalculateMonthlyDays`; the reconstructed fixed-year behavior remains an R1 calendar rule, not an XML binding.

## 5. Cooling source binding

### Monthly AvgTemp

Cooling R6 uses `DefaultParams.xml` monthly `AvgTemp` for:

- `CalculateCoolingEnergyRef1/Ref2/Actual` result row `MonthDataCooling.AvgTemp`.
- `CalculateAc*`.
- `CalculateCoolingQtr*`.
- `CalculateQinf*`.

Binding:

```text
PreferencesManager.GetClimateZoneParams(climateZone)
  .SolarRadiation.Months[(int)month.Month].AvgTemp
```

### Hourly TempHumidity

Cooling latent and free-cooling paths use `DefaultParams.xml` hourly weather:

```text
PreferencesManager.GetClimateZoneParams(climateZone)
  .TempHumidity.Months[(int)month.Month].Hours
```

`GetDaysHours` builds a shifted 25-item list by prepending hour 23 before hours 0-23; this is KD-V008 and applies to cooling ventilation and cooling latent/free-cooling style paths that call it.

### SolarRadiation

Cooling `Qsol*` uses the same `DefaultParams.xml` `SolarRadiation.Months[month].N/E/S/W/H` binding as heating R4. `DefaultSunParams.xml` is not used for R6 cooling solar gains.

### Humidity inputs

Outdoor humidity:

- `DefaultParams.xml` `TempHumidity.Months[month].Hours[hour].Humidity`.

Indoor/project humidity:

- `CalculationData.ProjectHumidity*`.
- `CalculationData.RelativeHumidity*` for ventilation supply/flow calculations.

Cooling psychrometric helpers use hardcoded `101325` in `CalcAirX`/`CalcRoW`; they do not use `ClimateZone.Pb`.

Other CalculationData/Section inputs:

- `CalculationData.ProjectTemperature*`, `NonProjectTemperature*`, `FlowTemperature*`, `Infiltracion*`, `HeatCapacity`, lighting/device powers and schedules.
- `Section.Area.HeatedArea`, `Section.Area.HeatedVolume`, cooling schedules, occupant schedules, envelope geometry/U-values.

Hardcoded constants:

- infiltration/ventilation heat coefficient `0.34`.
- cooling `Ac` time constant divisor `15`.
- latent conversion constant `0.6947222222222222`.
- psychrometric constants in `CalcAirX`, `CalcRoW`, and `CalcRo`.

## 6. Ventilation source binding

### Monthly AvgTemp

Heating ventilation R7 uses monthly `AvgTemp` from `DefaultParams.xml`:

```text
PreferencesManager.GetClimateZoneParams(calcInput.General.ClimateZone)
  .SolarRadiation.Months[(int)month.Month].AvgTemp
```

This value drives `CalculateMontlyHeatEnergy*` and the heating-air recovery branches.

### Hourly TempHumidity

Cooling ventilation R7 uses hourly temperature and humidity from `DefaultParams.xml`:

```text
PreferencesManager.GetClimateZoneParams(climateZone)
  .TempHumidity.Months[month].Hours
```

The `GetDaysHours` helper prepends hour 23, then appends hours 0-23.

### Pb

Heating ventilation enthalpy uses:

```text
PreferencesManager.GetClimateZoneParams(calcInput.General.ClimateZone).Pb
```

XML binding:

```text
DefaultParams.xml /Parameters/ClimateZones/ClimateZone/Pb
```

### Humidity

Heating ventilation humidity:

- source: average of `DefaultParams.xml` hourly `TempHumidity.Months[month].Hours[*].Humidity`.
- used in `CalcEntalpia(num, humidity, Pb)` and `CalcEntalpia(MinimumEndTemperature*, humidity, Pb)`.

Cooling ventilation humidity:

- outdoor: `DefaultParams.xml` hourly humidity through shifted `GetDaysHours`.
- supply/flow: `CalculationData.RelativeHumidity*` or `ventCoolCalculations.VentilationCooling.RelativeHumidity*`.

Section/CalculationData inputs:

- schedules from `Section.HeatingSeasons` and `Section.CoolingSeasons`.
- ventilation scalar fields from `CalculationData` or ventilation calculation groups: `Debit*`, `FlowTemperature*`, recovery efficiencies, fuel parts, generator efficiencies, and source-energy fields.
- project/non-project temperatures from heating/cooling calculation data.

## 7. Solar/DHW cross-reference

Solar hot water / DHW uses both XML sources:

| Area | Source manager | XML file | Fields |
| --- | --- | --- | --- |
| `SetTableResults` solar month rows | `SunEnergyPreferencesManager` | `DefaultSunParams.xml` | monthly `Radiation` -> `SunMonth.H`, monthly `AvgTemp` -> `SunMonth.Tm` |
| `CalculateParameterHtMonthly` | `SunEnergyPreferencesManager` | `DefaultSunParams.xml` | monthly `Radiation` |
| cloudiness/diffuse correction | `SunEnergyPreferencesManager` | `DefaultSunParams.xml` | monthly `Cloudiness` |
| `CalculateParameterX` ambient delta | `PreferencesManager` | `DefaultParams.xml` | monthly `AvgTemp` in `100.0 - AvgTemp` |

Heating, cooling, and ventilation solar/climate paths audited here use `DefaultParams.xml`; they do not use `DefaultSunParams.xml`.

## 8. Known data divergences

### KD-DATA-001

Classification: confirmed legacy XML data error, not a calculation/formula error.

The correct ordinance January temperatures are the positive values in `EE.Doklad/Data/climate_zones.json`.
The negative January values in `reference/eecalc-config/DefaultParams.xml` are a technical data-entry/sign error in the legacy EECalc XML.

| ZoneId | Month | DefaultParams.xml AvgTemp | climate_zones.json AvgTemp | Delta |
| --- | --- | ---: | ---: | ---: |
| 1 | January | -1.9 | 1.9 | 3.8 |
| 2 | January | -0.5 | 0.5 | 1.0 |
| 3 | January | -0.1 | 0.1 | 0.2 |

Because R1-R7 use `DefaultParams.xml` for monthly `AvgTemp`, KD-DATA-001 affects:

- heating Qtr/Qve/aH and related net energy in strict legacy mode.
- cooling Qtr/Qinf/Ac and related net energy in strict legacy mode.
- ventilation heating monthly energy and any enthalpy branches that depend on monthly average temperature in strict legacy mode.
- solar/DHW `CalculateParameterX` through `PreferencesManager` where applicable.

`DefaultSunParams.xml` also contains matching negative January `AvgTemp` values for zones 1-3 in the available legacy file. This audit does not reclassify those values separately; solar/DHW provider decisions should be kept aligned with KD-DATA-001 unless a separate solar-data audit proves otherwise.

## 9. Required oracle data providers

A future oracle should not read XML or JSON directly from formula code. It should receive a provider interface that exposes explicit source bindings:

```text
IClimateDataProvider
  GetMonthlyAvgTemp(climateZone, month)
  GetSolarRadiationN/E/S/W/H(climateZone, month)
  GetHourlyTempHumidity(climateZone, month)
  GetPb(climateZone)

ISunEnergyDataProvider
  GetSunMonthlyAvgTemp(climateZone, month)
  GetSunMonthlyRadiation(climateZone, month)
  GetSunMonthlyCloudiness(climateZone, month)
```

Provider responsibilities:

- preserve zero-based EECalc climate zone mapping for legacy calculations.
- expose a documented one-based ordinance mapping when using `EE.Doklad/Data/climate_zones.json`.
- carry the source label into debug rows: `DefaultParams.xml`, `DefaultSunParams.xml`, `CalculationData`, `Section`, or `hardcoded`.
- preserve hardcoded constants separately from climate data.

## 10. Recommendation

Implement three explicit provider modes when oracle work resumes:

| Mode | Climate monthly AvgTemp | Orientation solar N/E/S/W/H | Hourly TempHumidity | Pb | Sun/DHW parameters |
| --- | --- | --- | --- | --- | --- |
| `LegacyEECalcStrict` | `DefaultParams.xml` exactly, including KD-DATA-001 sign error | `DefaultParams.xml` | `DefaultParams.xml` | `DefaultParams.xml` | `DefaultSunParams.xml` exactly |
| `LegacyEECalcCorrectedData` | corrected January values from `EE.Doklad/Data/climate_zones.json` for KD-DATA-001; otherwise legacy-compatible data | legacy-compatible/corrected by explicit data rule | legacy-compatible/corrected by explicit data rule | `DefaultParams.xml` unless corrected data defines Pb | corrected legacy solar data only by explicit rule |
| `CurrentOrdinance` | `EE.Doklad/Data/climate_zones.json` | `EE.Doklad/Data/climate_zones.json` where available | current ordinance/json provider where available | current ordinance/json provider where available | current ordinance solar provider where available |

For EECalc parity, use `LegacyEECalcStrict`.
For corrected legacy-data diagnostics, use `LegacyEECalcCorrectedData`.
For current ordinance validation, use `CurrentOrdinance`.

Do not silently normalize data in formulas. Data corrections belong in provider selection and must be visible in debug output.
