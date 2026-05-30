# R8 Domestic Hot Water / BGV Reverse Engineering

Source focus:

- `reference/eecalc-decompiled/EECalcCore.Calculations.TableCalculations.HeatingAndCoolingResultCalc.cs`
- `reference/eecalc-decompiled/EECalcCore.Calculations.SunEnergyPreferencesManager.cs`
- `reference/eecalc-decompiled/EECalcCore.Calculations.PreferencesManager.cs`
- `reference/eecalc-config/DefaultParams.xml`
- `reference/eecalc-config/DefaultSunParams.xml`
- `analysis/docs/01_call_graph.md`
- `analysis/docs/02_method_index.md`
- `analysis/docs/03_formula_catalog.md`

Scope: domestic hot water / BGV demand, ordinary hot water needed/source energy, generator efficiency, savings, solar hot water through `SunEnergyCalculations`, BGV pumps, and building/zone integration hooks. This report is analysis only. No oracle, tests, production comparison, or production code changes are included.

## 1. Summary

EECalc has three separate but connected BGV/DHW paths:

- Ordinary hot water demand and needed/source energy on `CalculationData` (`HotWaterCalculation*`, `CalculateHotWaterNeededEnergy*`, `CalculateGeneratorHotWaterEfficiency*`).
- Solar hot water sizing/result tables on `SunEnergyCalculationData` (`CalculateHotWaterNeededPower`, `CalculateParameterF/X/Y/HtMonthly`, `SetTableResults`, `BGVSunEnergy`, `BGVPumpsTotal`).
- Pump/electricity accounting, split between normal `HotWaterPumps` period calculations and solar `BGVPumpsTotal` building hooks.

Ordinary DHW net energy is calculated from annual consumption and temperature difference. Solar hot water is then subtracted with a zero floor:

```text
ResultEnergyForHeating = max(0, ResulNetEnergy - SunEnergy)
```

Needed/source energy is calculated from `ResultEnergyForHeating`, split into two fuel shares, and divided by supply, automatic, management, and generator heat efficiencies. Unlike heating/ventilation conversions, ordinary hot water does not include `TransmitTempEfficiency` in the needed-energy denominator, even though it appears in baseline rows and savings tags.

Solar hot water is calculated separately with monthly collector equations. Its usable solar energy is written to `SunEnergyResTable.BGVSunEnergy` and `TotalUsedSunEnergy`; solar pump energy is written to `SunEnergyResTable.BGVPumpsTotal`.

## 2. DHW/BGV execution flow

Ordinary hot water flow:

1. Run one of:
   - `HotWaterCalculationReferences`
   - `HotWaterCalculationActual`
   - `HotWaterCalculationBaseLine`
   - `HotWaterCalculationESM`
2. Calculate mixed water from annual consumption and total heated area.
3. Calculate `ResulNetEnergy*` from temperature difference and consumption.
4. Subtract `SunEnergy*` from `ResulNetEnergy*` into `ResultEnergyForHeating*`, with `Math.Max(0, ...)`.
5. Run `CalculateHotWaterNeededEnergy*`.
6. Run `CalculateGeneratorHotWaterEfficiency*`.
7. Building results aggregate BGV only once, from the first building zone, when `isBGVused` is true.

Solar hot water flow:

1. Run `CalculateHotWaterNeededPower` on one `SunEnergyCalculationData` variant.
2. Clear all monthly result rows for the full year.
3. Iterate the configured solar active period, `StartMonth` through `EndMonth`.
4. For each month:
   - Recalculate total collector area.
   - Calculate BGV demand for active days and total month days.
   - Read solar radiation and average temperature from `SunEnergyPreferencesManager`.
   - Calculate `Ht`, `X`, `Y`, corrected `X`, and `F`.
   - Calculate monthly solar hot water energy and pump energy.
   - Write the month into `SunEnergyResTable`.
5. Aggregate `TotalProportion`, `TotalSunEnergy`, `BGVSunEnergy`, `BGVPumpsTotal`, and `TotalUsedSunEnergy`.

BGV pump flow:

1. Normal BGV pumps use `CalculatePeriods*HotWaterPumps` on `HotWaterPumps.Heating`, `.Cooling`, and `.General`.
2. Solar BGV pumps are calculated in `CalculateHotWaterNeededPower` as `BGVPumpsTotal`.
3. Building-level aggregation adds normal BGV pump needed energy and then adds solar `BGVPumpsTotal`.
4. Fuel/electricity integration puts BGV pump energy into Fuel8/electricity table hooks at building aggregation time.

## 3. Full call graph

Ordinary DHW:

```text
HotWaterCalculationReferences
  -> sets Ref1 and Ref2 demand/result energy

HotWaterCalculationActual
  -> sets Actual demand/result energy

HotWaterCalculationBaseLine
  -> sets BaseLine demand/result energy

HotWaterCalculationESM
  -> sets ESM demand/result energy

CalculateHotWaterNeededEnergyRef1/Ref2/Actual/BaseLine/Esm
  -> splits ResultEnergyForHeating by Part1/Part2
  -> divides by source efficiency chains
  -> sets ResultSourceEnergy, ResultSourceEnergy2, ResultNeededEnergy

CalculateGeneratorHotWaterEfficiencyRef1/Ref2/Actual/BaseLine/Esm
  -> weighted generator heat efficiency from source-energy buckets
```

Hot water savings:

```text
CalculateHotWaterSavings
  -> CheckForHotWaterSavings
  -> CheckForDifferentFuelSources
  -> CheckForFuelSavings
  -> SetHotWaterSavingsValues
  -> GetHotWaterBaseLine
  -> SetHotWaterBaseLine
  -> HotWaterCalculationBaseLine
  -> CalculateGeneratorHotWaterEfficiencyBaseLine
  -> CalculateHotWaterNeededEnergyBaseLine
  -> AddSavingsToBuilding
```

Normal BGV pumps:

```text
CalculatePeriodsReferenceHotWaterPumps
  -> CalculateHeatingPeriodRef1HotWaterPumps
  -> CalculateHeatingPeriodRef2HotWaterPumps
  -> CalculateCoolingPeriodRef1HotWaterPumps
  -> CalculateCoolingPeriodRef2HotWaterPumps
  -> CalculateAnnualPeriodRef1HotWaterPumps
  -> CalculateAnnualPeriodRef2HotWaterPumps

CalculatePeriodsActualHotWaterPumps
  -> CalculateAnnualPeriodActualHotWaterPumps
  -> CalculateHeatingPeriodActualHotWaterPumps
  -> CalculateCoolingPeriodActualHotWaterPumps

CalculatePeriodsBaseLineHotWaterPumps
  -> CalculateHeatingPeriodBaseLineHotWaterPumps
  -> CalculateCoolingPeriodBaseLineHotWaterPumps
  -> CalculateAnnualPeriodBaseLineHotWaterPumps

CalculatePeriodsESMHotWaterPumps
  -> CalculateHeatingPeriodESMHotWaterPumps
  -> CalculateCoolingPeriodESMHotWaterPumps
  -> CalculateAnnualPeriodESMHotWaterPumps
```

Solar hot water:

```text
CalculateHotWaterNeededPower
  -> ClearTableValues
  -> SumCollectorsArea
  -> HotWaterNeededPower
  -> HotWaterNeededPowerTotal
  -> CalculateParameterHtMonthly
      -> CalculateProjectionCoeficient
          -> DefuseradiationHd
          -> CalculateMonthlyHorizontalRadiation
              -> SunDeclination
              -> SunsetHour
              -> SunsetHourPrim
                  -> SubAngles
  -> CalculateParameterX
      -> CalculateTOAeffect
  -> CalculateParameterY
      -> CalculateTOAeffect
      -> CalculateParameterHtMonthly
  -> CalculateXwithCorrection
  -> CalculateParameterF
  -> SetTableResults
      -> SetMonthRowValues
```

Building hooks:

```text
BuildingCalculations
  -> UpdateRefsState / UpdateActualState / UpdateBaseLineState / UpdateEsmState
  -> CalculateTotalsNeededEnergyTable
  -> CalculatePrimaryEnergyByTechnologies
  -> GetPrimaryFuelTypeAndValues
  -> GetFuelTypeAndValues
  -> add SunEnergy BGVPumpsTotal to Fuel8 Actual/BaseLine/ESMArea
  -> BuildingCO2Calculations
  -> CalculateTotalFuelEnergy
  -> CalculateTotalVei
```

## 4. Formula catalog

Total heated area used by ordinary DHW:

```text
totalHeatedArea = calcInput.BuildingZones.Sum(zone => zone.Heating.Area.HeatedArea)
```

Mixed water:

```text
MixedWater = Consumption * totalHeatedArea / 1000
```

Ordinary DHW net energy:

```text
ResulNetEnergy = 1.161 * TempDifference * 0.98 * Consumption / 1000
```

Ordinary DHW energy after solar:

```text
ResultEnergyForHeating = max(0, ResulNetEnergy - SunEnergy)
```

Needed/source energy, source 1:

```text
source1Demand = ResultEnergyForHeating * Part1 / 100
ResultSourceEnergy =
  source1Demand /
  (SupplyNetEfficiency/100 * Automatic/100 * EnergyManagement/100 * GeneratorHeatEfficiency1/100)
```

Needed/source energy, source 2:

```text
source2Demand = ResultEnergyForHeating * Part2 / 100
ResultSourceEnergy2 =
  source2Demand /
  (SupplyNetEfficiency2/100 * Automatic2/100 * EnergyManagement2/100 * GeneratorHeatEfficiency2/100)
```

Total needed energy:

```text
ResultNeededEnergy = ResultSourceEnergy + ResultSourceEnergy2
```

Weighted generator hot water efficiency:

```text
HeatEfficiencyGenerating =
  (ResultSourceEnergy * GeneratorHeatEfficiency1
   + ResultSourceEnergy2 * GeneratorHeatEfficiency2)
  / (ResultSourceEnergy + ResultSourceEnergy2)
```

Normal BGV pumps without monthly schedules:

```text
DevicesNeededEnergy = WorkSchedule * Power * totalWeeks / 1000
```

Normal BGV pumps with monthly schedules:

```text
CalcWeekPower =
  (WorkDays * WorkDaysUsedEnergy * 5
   + Saturdays * SaturdaysUsedEnergy
   + Sundays * SundaysUsedEnergy)
  / (WorkDays * 5 + Saturdays + Sundays)

weekRegime = WorkDays * 5 + Saturdays + Sundays
monthlyWeightedEnergy = CalcWeekPower(month) * weekRegime * month.Weeks
DevicesNeededEnergy = sum(monthlyWeightedEnergy) / 1000
```

Solar hot water active-days demand:

```text
HotWaterNeededPower =
  WaterUsage * (HotWaterTemperature - ColdWaterTemperature)
  * 1.163 / 1000
  * (DaysInWeek * month.Weeks)
```

Solar hot water full-month demand:

```text
HotWaterNeededPowerTotal =
  WaterUsage * (HotWaterTemperature - ColdWaterTemperature)
  * 1.163 / 1000
  * month.TotalDays
```

Solar pump total:

```text
monthPumpEnergy = sunMonth.Days * 8 * PumpsVolume
BGVPumpsTotal = round(sum(monthPumpEnergy) * totalHeatedArea / 1000, 1)
```

## 5. Hot water demand formula

Ordinary DHW demand is annual and area-normalized by construction:

```text
ResulNetEnergy = 1.161 * TempDifference * 0.98 * Consumption / 1000
```

Important behavior:

- `Consumption` is not multiplied by total heated area in `ResulNetEnergy`.
- `MixedWater` is multiplied by total heated area, but it is not used in the ordinary DHW energy formula.
- `HotWaterBaseLine` is present as a baseline row tag but is not recalculated in `HotWaterCalculationBaseLine`.
- Solar contribution is subtracted after `ResulNetEnergy` and before needed/source conversion.

Reference behavior:

- Ref1 and Ref2 are both calculated by `HotWaterCalculationReferences`.
- Ref1 uses `ConsumptionRef1`, `TempDifferenceRef1`, `SunEnergyRef1`.
- Ref2 uses `ConsumptionRef2`, `TempDifferenceRef2`, `SunEnergyRef2`.

## 6. Needed energy formula

For each variant, `CalculateHotWaterNeededEnergy*` uses:

```text
ResultEnergyForHeating * PartN / 100
```

Then divides by:

```text
SupplyNetEfficiencyN / 100
* AutomaticN / 100
* EnergyManagementN / 100
* GeneratorHeatEfficiencyN / 100
```

If a source result becomes `NaN` or infinity, EECalc resets that source result to `0`.

ESM additionally writes:

```text
ResultNeededEnergySavings =
  (ResultNeededEnergyBaseLine - ResultNeededEnergyESM).ToString("F3")
```

## 7. Source energy / efficiency conversion

Ordinary hot water has two source-energy buckets:

- `ResultSourceEnergy*` for `Fuel1*`, `Part1*`, and efficiency chain 1.
- `ResultSourceEnergy2*` for `Fuel2*`, `Part2*`, and efficiency chain 2.

The conversion omits `TransmitTempEfficiency*` and `TransmitTempEfficiency2*`, even though these fields are included in `GetHotWaterBaseLine`, `SetHotWaterBaseLine`, and fuel savings values. This differs from several other energy technologies in the same file.

Source-energy values later feed:

- fuel energy tables through `GetFuelType*`.
- primary energy through `GetPrimaryFuelType*`.
- CO2 through `GetFuelTypeCo2*`.
- VEI through `GetVeiBGV` for ESM BGV sources.

## 8. Generator efficiency logic

`CalculateGeneratorHotWaterEfficiency*` calculates weighted generator efficiency from source-energy buckets:

```text
HeatEfficiencyGenerating =
  (ResultSourceEnergy * GeneratorHeatEfficiency1
   + ResultSourceEnergy2 * GeneratorHeatEfficiency2)
  / (ResultSourceEnergy + ResultSourceEnergy2)
```

Each method catches all exceptions and also guards infinity/NaN, setting the result to `0`.

This is a reporting/summary efficiency. It is calculated after source energy is known and does not feed back into `CalculateHotWaterNeededEnergy*`.

## 9. Solar hot water calculation

`CalculateHotWaterNeededPower` is the solar/BGV collector calculation. It works on `SunEnergyCalculationData`, not ordinary `CalculationData`.

Monthly steps:

1. `SumCollectorsArea`:

   ```text
   CollectorsArea = AbsorbingSurface * CollectorsCount
   ```

2. Demand:

   ```text
   qHotWaterActive = HotWaterNeededPower(...)
   qHotWaterTotal = HotWaterNeededPowerTotal(...)
   ```

3. Climate:

   ```text
   H = SunEnergyPreferencesManager.GetClimateZoneParams(zone).SolarRadiation.Months[month].Radiation
   Tm = SunEnergyPreferencesManager.GetClimateZoneParams(zone).SolarRadiation.Months[month].AvgTemp
   Ht = CalculateParameterHtMonthly(...)
   ```

4. Collector parameters:

   ```text
   x = CalculateParameterX(...)
   y = CalculateParameterY(...)
   correctedX = CalculateXwithCorrection(x)
   f = CalculateParameterF(correctedX, y)
   ```

5. Monthly solar hot water:

   ```text
   Qsunwater = f * qHotWaterTotal / month.TotalDays * (DaysInWeek * month.Weeks)
   Fm = min(Qsunwater / Qhotwater * 100, 100)
   FmRemain = max(Qsunwater / Qhotwater * 100 - 100, 0)
   ```

6. Usable solar hot water:

   ```text
   usedSunEnergyMonth =
     Qhotwater * Fm/100 * (SerpentineEfficiencyIsUsed ? SerpentineEfficiency : 100) / 100
   ```

7. Solar pump energy:

   ```text
   BGVPumps month = Days * 8 * PumpsVolume
   ```

Aggregate outputs:

- `TotalProportion = round(sum(usedSunEnergy) / sum(Qhotwater) * 100, 1)` as invariant string.
- `TotalSunEnergy = round(sum(Qsunwater), 1)` as invariant string.
- `BGVSunEnergy = round(sum(usedSunEnergy), 1)` as invariant string.
- `BGVPumpsTotal = round(sum(BGVPumps month) * totalHeatedArea / 1000, 1)`.
- `TotalUsedSunEnergy = sum(usedSunEnergy) / totalHeatedArea`, or `0` on invalid area/NaN.

## 10. Collector area and storage correction

Collector area:

```text
CollectorsArea = AbsorbingSurface * CollectorsCount
```

`CalculateXwithCorrection` applies storage correction only when:

```text
37.5 < AcumulatorVolume / CollectorsArea < 300
```

If active:

```text
correction = (AcumulatorVolume / CollectorsArea / 75)^-0.25
correctedX = correction * x
```

Otherwise:

```text
correctedX = x
```

`CalculateParameterX`:

```text
toa = CalculateTOAeffect()
deltaT = 100 - PreferencesManager.GetClimateZoneParams(zone).SolarRadiation.Months[month].AvgTemp
seconds = month.TotalDays * 24 * 60 * 60
convertedDemand = neededHotWaterEnergyforMonth * 1000 / 1.163 * 4187
X = FR * toa * deltaT * seconds * (CollectorsArea / convertedDemand)
```

`CalculateParameterY`:

```text
coverFactor = 0.95 if TrasparentCoverings == 1 else 0.93
if TrasparentCoverings == 2 and month is June/July/August:
  coverFactor = 0.9

Y = FRta * toa * coverFactor * Ht * totalDays * (CollectorsArea / neededHotWaterEnergyforMonth)
```

`CalculateParameterF`:

```text
F = 1.029*y - 0.065*x - 0.245*y^2 + 0.0018*x^2 + 0.0215*y^3
```

`CalculateTOAeffect`:

- If `Scheme1Selected` or `Scheme2Selected`, returns `1`.
- Otherwise:

  ```text
  collectorFlow = CollectorDebit * SpecialHeatCapacity
  mtoaEfficiency = MTOAEfficiency / 100
  mtoaFlow = MTOADebit * MTOASpecialHeatCapacity
  minFlow = min(collectorFlow, mtoaFlow)
  toa = 1 + CollectorsArea * FR / collectorFlow * (collectorFlow / (mtoaEfficiency * minFlow) - 1)
  toa = toa^-1
  ```

- NaN/infinity returns `0`.

## 11. Pump energy / BGVPumpsTotal

There are two pump concepts:

Normal BGV pumps:

- Stored under `CalculationData.HotWaterPumps`.
- Calculated for heating, cooling, and annual/general periods.
- Uses either simple `WorkSchedule * Power * weeks / 1000` or monthly schedule weighted average.
- Savings are handled by `CalculateHotWaterPumpsSavings` through the generic lights/devices period savings path.

Solar BGV pumps:

- Stored as `SunEnergyCalculations.*.SunEnergyResTable.BGVPumpsTotal`.
- Calculated in `CalculateHotWaterNeededPower` from active solar days, 8 hours/day, `PumpsVolume`, and total heated area.
- Added to building BGV pump needed energy separately from normal `HotWaterPumps`.

Exact Fuel8/electricity hook points:

- Normal BGV pump building fuel table addition:

  ```text
  GetFuelTypeAndValues:
    Fuel8.ActualArea   += ZoneResults.NeededEnergyTable.BGVPumps.ActualArea   * totalArea
    Fuel8.BaseLineArea += ZoneResults.NeededEnergyTable.BGVPumps.BaseLineArea * totalArea
    Fuel8.ESMArea      += ZoneResults.NeededEnergyTable.BGVPumps.ESMArea      * totalArea
    Fuel8.Ref1Area     += ZoneResults.NeededEnergyTable.BGVPumps.Ref1Area     * totalArea
    Fuel8.Ref2Area     += ZoneResults.NeededEnergyTable.BGVPumps.Ref2Area     * totalArea
  ```

- Solar BGV pump Fuel8 addition after zone loop in `BuildingCalculations`:

  ```text
  Fuel8.ActualArea   += FirstZone.SunEnergyCalculations.Actual.BGVPumpsTotal
  Fuel8.BaseLineArea += FirstZone.SunEnergyCalculations.BaseLine.BGVPumpsTotal
  Fuel8.ESMArea      += FirstZone.SunEnergyCalculations.ESM.BGVPumpsTotal
  ```

- Primary-energy fuel type uses electricity/Fuel1 for BGVPumps:

  ```text
  GetPrimaryFuelType*(Fuel.Fuel1, NeededEnergyTable.BGVPumps.*Area, totalArea)
  ```

- CO2 for normal BGV pumps also uses `Fuel.Fuel1`.
- CO2 for solar `BGVPumpsTotal` is added directly with factor `819 / 1000000`.

## 12. Actual / BaseLine / ESM / Ref1 / Ref2 differences

Ordinary DHW:

- Ref1/Ref2 are calculated together and use reference consumption, temperature difference, solar energy, parts, fuels, and efficiencies.
- Actual uses actual consumption, temperature difference, solar energy, parts, fuels, and efficiencies.
- BaseLine uses baseline values and is the savings recalculation target.
- ESM uses ESM values and writes `ResultNeededEnergySavings`.

Normal BGV pumps:

- Ref1/Ref2 use simple period formulas only.
- Actual uses monthly schedules if `HotWaterPumps.ByMonths`, but does not update `PowerActual` or `WorkScheduleActual` from monthly values.
- BaseLine uses monthly schedules if enabled and updates `PowerBaseLine` and `WorkScheduleBaseLine` when values are non-trivial.
- ESM uses monthly schedules if enabled and updates `PowerESM` and `WorkScheduleESM` when values are non-trivial.
- ESM savings strings for pump periods are only written in the non-monthly `else` branch.

Solar:

- Actual/BaseLine/ESM solar calculations are separate `SunEnergyCalculationData` instances.
- Building Ref1/Ref2 BGV pump aggregation reuses Actual solar `BGVPumpsTotal`.
- Building Fuel8 solar pump addition is only for Actual/BaseLine/ESM, not Ref1/Ref2.

Building aggregation:

- BGV itself is taken from the first zone's zone results.
- Solar BGV pump total is taken from the first zone's `HotWaterCalculations.SunEnergyCalculations`.
- Normal BGV pump aggregation sometimes sums all zone results for references, but Actual/BaseLine/ESM paths use first-zone area values in `Update*State`.

## 13. Savings logic

`CalculateHotWaterSavings`:

1. Sets `publicCalculationData = calcData`.
2. Clones calculation data.
3. Builds ordinary hot water savings with `CheckForHotWaterSavings("BGV")`.
4. Calls `CheckForDifferentFuelSources`.
5. Adds fuel-source savings with `CheckForFuelSavings("BGV", calculationData)`.
6. Writes savings fields via `SetHotWaterSavingsValues`.
7. If savings exist:
   - Reads baseline rows with `GetHotWaterBaseLine`.
   - Stores `virtualBaseLineNetEnergy` from the `ResultNeededEnergy` row.
   - For each saving, applies one changed row to a clone.
   - Recalculates baseline hot water demand, generator efficiency, and needed energy.
   - Stores individual `NetEnergy` and `Saving`.
   - Computes `Part = Saving / sum(Saving)`.
   - Applies all savings together to another clone.
   - Recalculates baseline demand, generator efficiency, and needed energy.
   - Distributes combined actual saving by `Part`.
   - Runs `CheckAndCalculateNegativeSavings` if both positive and negative actual savings exist.
8. Independently checks solar `BGVPumpsTotal` BaseLine vs ESM and adds a `SunEnergy` saving:

   ```text
   ActualSaving = BaseLine.BGVPumpsTotal / totalHeatedArea
                - ESM.BGVPumpsTotal / totalHeatedArea
   ```

9. Calls `AddSavingsToBuilding(list, calcInput, "BGV")`.

`CheckForHotWaterSavings` adds ordinary savings for:

- `Consumption`
- `TempDifference`
- `SunEnergy`

Fuel/efficiency savings are added through `CheckForFuelSavings` and then mapped by `SetHotWaterSavingsValues`:

- `Part1`, `SupplyNetEfficiency`, `Automatic`, `EnergyManagement`, `GeneratorHeatEfficiency1`
- `Part2`, `SupplyNetEfficiency2`, `Automatic2`, `EnergyManagement2`, `GeneratorHeatEfficiency2`
- `TransmitTempEfficiency` and `TransmitTempEfficiency2` are also mapped, even though ordinary DHW needed-energy conversion does not use them.

Normal BGV pump savings use `CalculateHotWaterPumpsSavings`, which delegates each period to the generic `CalculatePeriod` lights/devices savings routine.

## 14. Building aggregation hooks

Building-level BGV needed energy:

- `UpdateRefsState` sets `NeededEnergyTable.BGV.Ref1/Ref2` from the first zone.
- `UpdateActualState`, `UpdateBaseLineState`, and `UpdateEsmState` also set BGV from the first zone.
- BGV and BGVPumps are included in total needed-energy tables only when `isBGVused` is true.

Building-level BGV pump needed energy:

- Reference paths sum zone normal BGV pump results and add Actual solar `BGVPumpsTotal`.
- Actual/BaseLine/ESM paths use first-zone `BGVPumps.*Area` and add the matching solar `BGVPumpsTotal`.

Fuel energy:

- Ordinary BGV source energy is added once, guarded by `isFirstBuildingZone`, through `GetFuelType*` calls on hot water source buckets.
- Normal BGV pumps are added to `Fuel8.*Area` from `NeededEnergyTable.BGVPumps.*Area`.
- Solar BGV pumps are added to `Fuel8.ActualArea`, `Fuel8.BaseLineArea`, and `Fuel8.ESMArea` after the zone loop from first-zone `BGVPumpsTotal`.

Primary energy:

- Ordinary BGV uses fuel-specific primary energy coefficients for hot water source buckets.
- BGVPumps use electricity/Fuel1 primary energy path.
- Direct table logic also multiplies `NeededEnergyTable.BGVPumps.*` by `3.0` in one primary-energy path.

VEI:

- `GetVeiBGV` adds VEI only for ESM hot water source buckets.
- Fuel1 contributes only when generator efficiency exceeds 100:

  ```text
  quantity * ((efficiency - 100) / 100)
  ```

- Fuel6/Fuel7 contribute `quantity` to both `VEI` and `GeneralVEI`.
- Solar hot water contribution uses:

  ```text
  min(SunEnergyESM, ResulNetEnergyESM) * totalArea
  ```

## 15. Climate and SunEnergyPreferences dependencies

Ordinary DHW:

- Does not directly use climate data.
- Uses `calcInput.BuildingZones` only to sum heated area for `MixedWater`.

Solar hot water:

- Uses `SunEnergyPreferencesManager.GetClimateZoneParams(calcInput.General.ClimateZone)`.
- `SunEnergyPreferencesManager` loads `Xml/DefaultSunParams.xml` from `Application.StartupPath`; in this repository the authoritative source is `reference/eecalc-config/DefaultSunParams.xml`.
- Monthly `Radiation`, `AvgTemp`, and `Cloudiness` are read from sun preferences.

Mixed preference-manager use:

- `CalculateHotWaterNeededPower` and `CalculateParameterHtMonthly` use `SunEnergyPreferencesManager`.
- `CalculateParameterX` uses `PreferencesManager.GetClimateZoneParams` for average temperature, not `SunEnergyPreferencesManager`.
- `PreferencesManager` loads `Xml/DefaultParams.xml` from `Application.StartupPath`; in this repository the authoritative source is `reference/eecalc-config/DefaultParams.xml`.

Solar geometry constants:

- Latitude is hard-coded as `42.3` degrees in projection math.
- `SunDeclination` uses fixed representative day numbers per month.
- `SunsetHour` uses hard-coded latitude radians `0.7382742735936013`.

Authoritative XML climate-zone mapping:

| XML file | XML node | C# loader/property | Formula use |
| --- | --- | --- | --- |
| `DefaultParams.xml` | `/Parameters/ClimateZones/ClimateZone/Number` | `PreferencesManager.Parameters.ClimateZones.Single(z => z.Number == (int)zone)` | Selects the ordinary climate zone for `CalculateParameterX` and other non-solar climate-dependent calculations. |
| `DefaultParams.xml` | `/Parameters/ClimateZones/ClimateZone/Title` | `ClimateZone.Title` | Human-readable climate zone label; not used directly in R8 formulas. |
| `DefaultParams.xml` | `/Parameters/ClimateZones/ClimateZone/SolarRadiation/Months/Month/AvgTemp` | `PreferencesManager.GetClimateZoneParams(zone).SolarRadiation.Months[month].AvgTemp` | `CalculateParameterX`: `deltaT = 100 - AvgTemp`. |
| `DefaultSunParams.xml` | `/SunParameters/ClimateZones/ClimateZone/Number` | `SunEnergyPreferencesManager.SunParameters.ClimateZones.Single(z => z.Number == (int)zone)` | Selects the solar climate zone for `CalculateHotWaterNeededPower`, `CalculateParameterHtMonthly`, and `DefuseradiationHd`. |
| `DefaultSunParams.xml` | `/SunParameters/ClimateZones/ClimateZone/Title` | `ClimateZone.Title` | Human-readable solar climate zone label; not used directly in R8 formulas. |
| `DefaultSunParams.xml` | `/SunParameters/ClimateZones/ClimateZone/SolarRadiation/Months/Month/AvgTemp` | `SunEnergyPreferencesManager.GetClimateZoneParams(zone).SolarRadiation.Months[month].AvgTemp` | Written to `SunMonth.Tm` and result table `TempM`. |
| `DefaultSunParams.xml` | `/SunParameters/ClimateZones/ClimateZone/SolarRadiation/Months/Month/Radiation` | `SunEnergyPreferencesManager.GetClimateZoneParams(zone).SolarRadiation.Months[month].Radiation` | Written to `SunMonth.H`; also used in `CalculateParameterHtMonthly`: `Ht = projectionCoefficient * Radiation`. |
| `DefaultSunParams.xml` | `/SunParameters/ClimateZones/ClimateZone/SolarRadiation/Months/Month/Cloudiness` | `SunEnergyPreferencesManager.GetClimateZoneParams(zone).SolarRadiation.Months[month].Cloudiness` | `DefuseradiationHd`: `1.39 - 4.03*c + 5.53*c^2 - 3.11*c^3`. |

Climate zone numbers are zero-based in both XML files and match `(int)calcInput.General.ClimateZone`:

| Number | Climate zone title |
| --- | --- |
| 0 | `Клим. зона 1 - Варна` |
| 1 | `Клим. зона 2 - Добрич, Шумен` |
| 2 | `Клим. зона 3 - Русе, Видин` |
| 3 | `Клим. зона 4 - Плевен, В.Търново` |
| 4 | `Клим. зона 5 - Бургас` |
| 5 | `Клим. зона 6 - Пловдив, Ямбол` |
| 6 | `Клим. зона 7 - София` |
| 7 | `Клим. зона 8 - Хасково` |
| 8 | `Клим. зона 9 - Благоевград` |

Solar parameter mapping from XML to formula:

| XML node | C# property | Formula / result |
| --- | --- | --- |
| `DefaultSunParams.xml` `AvgTemp` | `SunMonth.Tm` | Stored in the monthly result row as `TempM`. |
| `DefaultSunParams.xml` `Radiation` | `SunMonth.H` | Stored as `H`; multiplied by projection coefficient to produce `Ht`. |
| `DefaultSunParams.xml` `Radiation` | `CalculateParameterHtMonthly(...).Radiation` | `Ht = CalculateProjectionCoeficient(...) * Radiation`. |
| `DefaultSunParams.xml` `Cloudiness` | `DefuseradiationHd(...).cloudiness` | Diffuse-radiation coefficient polynomial. |
| `DefaultParams.xml` `AvgTemp` | `CalculateParameterX(...).AvgTemp` | `deltaT = 100 - AvgTemp`; note this uses `PreferencesManager`, not `SunEnergyPreferencesManager`. |

Collector parameter mapping:

No collector default nodes were found in `DefaultParams.xml` or `DefaultSunParams.xml` for `Collector`, `Collectors`, `FR`, `FRta`, `AbsorbingSurface`, `AcumulatorVolume`, `MTOA`, `PumpsVolume`, `WaterUsage`, `HotWaterTemperature`, or `ColdWaterTemperature`. These are model/input properties on `SunEnergyCalculationData`, not XML-backed defaults in the provided configuration files.

| C# property | XML source | Formula use |
| --- | --- | --- |
| `AbsorbingSurface` | none found | `CollectorsArea = AbsorbingSurface * CollectorsCount`. |
| `CollectorsCount` | none found | `CollectorsArea = AbsorbingSurface * CollectorsCount`. |
| `CollectorsArea` | derived, none found | Used in `X`, `Y`, `TOAeffect`, and storage correction. |
| `AcumulatorVolume` | none found | `AcumulatorVolume / CollectorsArea` storage correction. |
| `FR` | none found | `X = FR * toa * deltaT * seconds * CollectorsArea / convertedDemand`; also `TOAeffect`. |
| `FRta` | none found | `Y = FRta * toa * coverFactor * Ht * totalDays * CollectorsArea / neededHotWaterEnergyforMonth`. |
| `TrasparentCoverings` | none found | Cover factor: `1 -> 0.95`, otherwise `0.93`, with summer `2 -> 0.9`. |
| `CollectorDebit` | none found | `collectorFlow = CollectorDebit * SpecialHeatCapacity` in `TOAeffect`. |
| `SpecialHeatCapacity` | none found | `collectorFlow = CollectorDebit * SpecialHeatCapacity`. |
| `MTOAEfficiency` | none found | `mtoaEfficiency = MTOAEfficiency / 100`. |
| `MTOADebit` | none found | `mtoaFlow = MTOADebit * MTOASpecialHeatCapacity`. |
| `MTOASpecialHeatCapacity` | none found | `mtoaFlow = MTOADebit * MTOASpecialHeatCapacity`. |
| `WaterUsage` | none found | `HotWaterNeededPower` and `HotWaterNeededPowerTotal`. |
| `HotWaterTemperature` | none found | `HotWaterNeededPower` temperature difference. |
| `ColdWaterTemperature` | none found | `HotWaterNeededPower` temperature difference. |
| `DaysInWeek` | none found | Active-days demand and monthly active day count. |
| `PumpsVolume` | none found | `monthPumpEnergy = Days * 8 * PumpsVolume`. |

## 16. EECalc quirks / KD candidates

KD-HW001: Ordinary DHW needed-energy conversion omits `TransmitTempEfficiency`, despite exposing it in baseline rows and savings fields.

KD-HW002: `HotWaterCalculation*` calculates `MixedWater` using total heated area, but `ResulNetEnergy` uses only `Consumption`, not `MixedWater`.

KD-HW003: `GetHotWaterBaseLine` emits `HotWater`, but `SetHotWaterBaseLine` never writes `HotWaterBaseLine`.

KD-HW004: `CalculateGeneratorHotWaterEfficiencyRef2` uses `ResultSourceEnergyRef2` and `ResultSourceEnergy2Ref2`, which is expected naming but visually easy to confuse with source 2.

KD-HW005: Savings share calculation uses `Part = Saving / sum(Saving)` with no observed zero guard.

KD-HW006: `CalculateHotWaterNeededPower` checks `if (Math.Abs(num) < 0.0) return false;`, which is unreachable for real numeric values.

KD-HW007: `SetMonthRowValues` assigns `QhotWaterSun` twice.

KD-HW008: Solar `BGVPumpsTotal` is rounded after multiplying by total heated area, while ordinary pump calculations are not rounded there.

KD-HW009: Reference building BGVPumps aggregation uses Actual solar `BGVPumpsTotal` for both Ref1 and Ref2.

KD-HW010: `BuildingCalculations` adds solar `BGVPumpsTotal` to `Fuel8.ActualArea/BaseLineArea/ESMArea`, but not to Ref1/Ref2 Fuel8 in the same post-zone hook.

KD-HW011: Actual monthly BGV pump calculations do not update `PowerActual` or `WorkScheduleActual`, while BaseLine and ESM monthly paths update their scalar fields.

KD-HW012: ESM pump savings strings are written only in the non-monthly branch for heating/cooling/general BGV pumps.

KD-HW013: Several building aggregation paths use `First()` zone BGV values instead of summing all zones.

KD-HW014: Solar calculation uses both `SunEnergyPreferencesManager` and `PreferencesManager` for climate-zone solar values.

KD-HW015: `GetVeiBGV` only handles Fuel1, Fuel6, and Fuel7.

KD-HW016: CO2 helper calls in one block use mismatched fuel fields for hot water source 2 in several variants; this needs separate CO2-focused validation before oracle implementation.

## 17. Required input fields

Ordinary DHW per variant:

- `Consumption*`
- `TempDifference*`
- `SunEnergy*`
- `Part1*`, `Part2*`
- `Fuel1*`, `Fuel2*`
- `SupplyNetEfficiency*`, `SupplyNetEfficiency2*`
- `Automatic*`, `Automatic2*`
- `EnergyManagement*`, `EnergyManagement2*`
- `GeneratorHeatEfficiency1*`, `GeneratorHeatEfficiency2*`
- Total heated area from `calcInput.BuildingZones`

Baseline/savings:

- All baseline rows emitted by `GetHotWaterBaseLine`
- ESM values for `Consumption`, `TempDifference`, `SunEnergy`, fuels, shares, and efficiencies
- `SunEnergyCalculations.BaseLine/ESM.SunEnergyResTable.BGVPumpsTotal`
- `calcInput.General.BuildingResults.TotalAreaElements.TotalHeatedArea`

Normal BGV pumps:

- `HotWaterPumps.Heating/Cooling/General.WorkSchedule*`
- `HotWaterPumps.Heating/Cooling/General.Power*`
- `HotWaterPumps.ByMonths`
- Monthly `HotWaterPumps.Actual/BaseLine/Esm` schedules
- Heating season period
- Cooling season period
- Full-year period

Solar hot water:

- `StartMonth`, `EndMonth`
- `WaterUsage`
- `HotWaterTemperature`
- `ColdWaterTemperature`
- `DaysInWeek`
- `AbsorbingSurface`
- `CollectorsCount`
- `CollectorsArea`
- `FR`
- `FRta`
- `AcumulatorVolume`
- `TrasparentCoverings`
- `Pitch`
- `ImpactEnvironment`
- `Scheme1Selected`, `Scheme2Selected`
- `CollectorDebit`, `SpecialHeatCapacity`
- `MTOAEfficiency`, `MTOADebit`, `MTOASpecialHeatCapacity`
- `SerpentineEfficiencyIsUsed`, `SerpentineEfficiency`
- `PumpsVolume`
- `calcInput.General.ClimateZone`
- `calcInput.General.BuildingResults.TotalAreaElements.TotalHeatedArea`

## 18. Proposed oracle design

Do not implement yet.

Suggested future shape:

- `EecalcDhwBgvOracle`
  - `OrdinaryHotWater`
    - `MixedWater`
    - `ResulNetEnergy`
    - `ResultEnergyForHeating`
    - `NeededEnergy`
    - `GeneratorHotWaterEfficiency`
  - `SolarHotWater`
    - `CollectorsArea`
    - `HotWaterNeededPower`
    - `HotWaterNeededPowerTotal`
    - `TOAeffect`
    - `ParameterX`
    - `ParameterY`
    - `XwithCorrection`
    - `ParameterF`
    - `MonthlyTableRows`
    - `BGVSunEnergy`
    - `BGVPumpsTotal`
  - `HotWaterPumps`
    - `PeriodWeeks`
    - `CalcWeekPower`
    - `PeriodPumpEnergy`
  - `Savings`
    - `GetHotWaterBaseLineRows`
    - `SetHotWaterBaseLineRows`
    - `SingleChangeSavings`
    - `BundledSavings`
    - `SolarPumpSavings`
  - `BuildingAggregation`
    - `NeededEnergyBGV`
    - `NeededEnergyBGVPumps`
    - `Fuel8PumpHooks`

Keep ordinary DHW and solar hot water as separate modules. Solar outputs can feed ordinary DHW through `SunEnergy*`, but the collector math should not be embedded in ordinary needed-energy conversion.

Preserve EECalc spelling in model-facing names:

- `ResulNetEnergy`
- `TrasparentCoverings`
- `AcumulatorVolume`
- `DefuseradiationHd`
- `BGVPumpsTotal`

## 19. Minimal fixtures

Fixture A: Ordinary DHW without solar.

- One building zone, known heated area.
- `SunEnergy = 0`.
- One fuel source at 100%, all efficiencies 100.
- Validates `MixedWater`, `ResulNetEnergy`, `ResultEnergyForHeating`, `ResultSourceEnergy`, and `ResultNeededEnergy`.

Fixture B: Ordinary DHW with solar cap.

- `SunEnergy < ResulNetEnergy`.
- Validates subtraction before needed-energy conversion.

Fixture C: Ordinary DHW with solar exceeding demand.

- `SunEnergy > ResulNetEnergy`.
- Validates `ResultEnergyForHeating = 0` and source energy reset behavior.

Fixture D: Two-source hot water.

- `Part1 + Part2 = 100`.
- Different fuel types and generator efficiencies.
- Validates source split and weighted `HeatEfficiencyGenerating`.

Fixture E: Transmit efficiency inertness.

- Change `TransmitTempEfficiencyBaseLine` only.
- Expected ordinary hot water needed energy does not change.

Fixture F: Normal BGV pumps, scalar schedule.

- `ByMonths = false`.
- Heating/cooling/general periods with known week counts.
- Validates `WorkSchedule * Power * weeks / 1000`.

Fixture G: Normal BGV pumps, monthly schedule.

- `ByMonths = true`.
- Non-uniform monthly workday/Saturday/Sunday values.
- Validates `CalcWeekPower`, `weekRegime`, and BaseLine/ESM scalar backfill.

Fixture H: Solar hot water single month.

- One active month.
- Fixed collector area, water usage, temperatures, and climate inputs.
- Validates `X`, `Y`, `F`, `Qsunwater`, `Fm`, `BGVSunEnergy`, and `BGVPumpsTotal`.

Fixture I: Solar storage correction boundary.

- `AcumulatorVolume / CollectorsArea` below, inside, and above `(37.5, 300)`.
- Validates `CalculateXwithCorrection`.

Fixture J: Building Fuel8 hook.

- One zone with normal BGVPumps and solar `BGVPumpsTotal`.
- Validates that normal BGVPumps and solar pumps both reach Fuel8/electricity through their separate hooks.

Fixture K: Multi-zone BGV aggregation.

- Two zones with different BGV/BGVPumps values.
- Validates first-zone behavior vs summed-zone behavior in building aggregation.
