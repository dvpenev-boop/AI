# Section 12 - Ventilation Implementation

## Overview
This document describes the complete implementation of Section 12 – Ventilation (Вентилация - отопление) according to **Наредба RD-02-20-3** (Bulgarian regulatory methodology).

## Implementation Date
February 2, 2026

## Regulatory Compliance

### Bulgarian Methodology (MANDATORY)
The implementation strictly follows the Bulgarian regulatory methodology as specified in Наредба RD-02-20-3, Section 12, with explicit references to:

- **Formula (3.27)**: Ventilation heat loss coefficient Hᵥₑ
- **Formula (3.28)**: Temperature control coefficient bᵥₑ,ₖ
- **Formula (3.26)**: Monthly ventilation heating energy Qᵥₑ,ₘ
- **Item 3.5**: Supply air temperature determination with heat recovery

### Key Constraints
✅ **Bulgarian methodology only** - Full implementation  
❌ **DIN methodology** - Not implemented (throws NotImplementedException)  
✅ **No internal tables** - All parameters must be input manually  
✅ **No default heat recovery values** - User must specify all efficiencies  
✅ **Single calculation state** - No scenarios (Actual/Normalized/ECM)  
✅ **Heating only** - Ventilation energy for heating purposes only  
✅ **Monthly calculation** - Energy calculated for each month separately

---

## Architecture

### Technology Stack
- **Framework**: WPF (.NET 8.0-windows)
- **Pattern**: MVVM (Model-View-ViewModel)
- **Language**: C# with nullable reference types enabled
- **UI**: XAML with data binding

### Component Structure

```
EE.Doklad/
├── Models/
│   ├── VentilationModels.cs              # Enums and energy source model
│   ├── VentilationSectionData.cs         # Input data model
│   └── VentilationCalculationResult.cs   # Output result model
├── Services/
│   └── BgVentilationCalculator.cs        # Calculation engine (BG methodology)
├── ViewModels/
│   └── VentilationSectionViewModel.cs    # MVVM ViewModel
└── Views/
    ├── VentilationSectionView.xaml       # WPF UI
    └── VentilationSectionView.xaml.cs    # Code-behind
```

---

## Detailed Implementation

### 1. Models

#### VentilationModels.cs
**Purpose**: Core enumerations and energy source model

**Components**:
- `VentilationMethodology` enum
  - `BG` - Bulgarian methodology (fully implemented)
  - `DIN` - German methodology (not implemented, for future use)
  
- `EnergySourceType` enum
  - Electricity, NaturalGas, DistrictHeating, Biomass, Solar, HeatPump, Other
  
- `VentilationEnergySource` class
  - Dual energy source support (ЕИ1 and ЕИ2)
  - Efficiency chain properties:
    - Emission efficiency (отдаване)
    - Distribution efficiency (разпределителна мрежа)
    - Automatic control (автоматично управление)
    - Energy management (енергиен мениджмънт)
    - Generation efficiency (генератор на топлина)
  - Automatic calculation of total efficiency

#### VentilationSectionData.cs
**Purpose**: Input data container (ObservableObject for MVVM)

**Input Parameters**:
- Operating hours per week [h/week]
- Airflow rate per m² [m³/h·m²]
- Supply temperature [°C]
- Relative humidity [%]
- First stage recuperation efficiency [%]
- Second stage recuperation efficiency [%]
- Max temperature difference for second stage [°C] (4-8)
- Min exhaust air temperature [°C] (3-5)
- Energy source 1 (mandatory)
- Energy source 2 (optional)

**Read-Only Properties** (from other sections):
- Heated area [m²] (from Section 5 - Object Data)
- Indoor reference temperature [°C] (from Section 10 - Heating)

#### VentilationCalculationResult.cs
**Purpose**: Output data container

**Contents**:
- Monthly results (12 months):
  - Outdoor temperature
  - Supply temperature (calculated per formula 3.5)
  - Temperature control coefficient bᵥₑ,ₖ (formula 3.28)
  - Monthly ventilation heating energy [kWh]
  
- Annual results:
  - Total ventilation heating energy [kWh/a]
  - Specific ventilation heating energy [kWh/m²·a]
  
- Final energy (with efficiency chain):
  - Final energy from source 1 [kWh/a]
  - Final energy from source 2 [kWh/a]
  - Total final energy [kWh/a]
  - Specific final energy [kWh/m²·a]
  
- Validation and assumptions list

---

### 2. Services

#### BgVentilationCalculator.cs
**Purpose**: Calculation engine implementing Bulgarian methodology

**Constants**:
```csharp
AirDensity_kg_m3 = 1.2              // ρ [kg/m³]
AirSpecificHeat_Wh_kgK = 0.28       // c [Wh/(kg·K)]
```

**Calculation Steps**:

##### Step 1: Ventilation Loss Coefficient Hᵥₑ [W/K]
**Formula (3.27)**:
```
Hᵥₑ = ρ × c × V̇
```
Where:
- ρ = air density [kg/m³]
- c = specific heat capacity [Wh/(kg·K)]
- V̇ = total airflow [m³/h] = airflow per m² × heated area

##### Step 2: Supply Air Temperature θₛᵤₚ [°C]
**Item 3.5**:
```
1. After first stage: θ₁ = θₑ + η₁ × (θᵢ - θₑ)
2. After second stage: θ₂ = θ₁ + η₂ × min(ΔTₘₐₓ, θᵢ - θ₁)
3. Apply limits:
   - θₛᵤₚ ≥ θₘᵢₙ,ₑₓₕₐᵤₛₜ
   - θₛᵤₚ ≤ θᵢ
```

##### Step 3: Temperature Control Coefficient bᵥₑ,ₖ [-]
**Formula (3.28)**:
```
bᵥₑ,ₖ = (θᵢ - θₛᵤₚ) / (θᵢ - θₑ)

Where: 0 ≤ bᵥₑ,ₖ ≤ 1
```

##### Step 4: Monthly Ventilation Heating Energy Qᵥₑ,ₘ [kWh]
**Formula (3.26)**:
```
Qᵥₑ,ₘ = Hᵥₑ × bᵥₑ,ₖ × (θᵢ - θₑ) × tₘ

Only when: θᵢ > θₑ (heating mode)
```

Where:
- tₘ = monthly operating time [h] = (hours/week ÷ 7) × days in month

##### Step 5: Annual Results
```
Annual energy = Σ Qᵥₑ,ₘ (for all 12 months)
Specific energy = Annual energy / Heated area [kWh/m²·a]
```

##### Step 6: Final Energy with Efficiency Chain
```
For each energy source:
  Total efficiency = η_emission × η_distribution × η_control × η_management × η_generation
  Final energy = (Heating energy × share) / Total efficiency
  
Total final energy = Final energy (source 1) + Final energy (source 2)
```

---

### 3. ViewModel

#### VentilationSectionViewModel.cs
**Purpose**: MVVM binding layer between data and view

**Features**:
- Property change notifications
- Input validation and clamping
- Automatic recalculation on data changes
- Binding to read-only data from other sections
- Error message display

**Key Properties**:
- All input parameters (editable)
- Calculated outputs (read-only)
- Error handling

---

### 4. View

#### VentilationSectionView.xaml
**Purpose**: WPF user interface

**Layout Sections**:

1. **Basic Parameters Table**
   - Operating hours per week
   - Airflow rate per m²
   - Supply temperature
   - Relative humidity
   - First/second stage recuperation efficiency
   - Temperature limits

2. **Energy Source 1 (ЕИ1) Table**
   - Energy source type
   - Share [%]
   - All efficiency parameters

3. **Energy Source 2 (ЕИ2) Table** (optional)
   - Can be enabled/disabled
   - Simplified inputs (share and generation efficiency)

4. **Results Table (След ЕСМ)**
   - Heating energy [kWh/m²]
   - Required energy from each source
   - Overall generation efficiency
   - Heating contribution
   - Total required energy

5. **Information Panel**
   - Regulatory references
   - Calculation methodology notes
   - User guidance

**Styling**:
- Red theme for ventilation section (#D32F2F)
- Editable fields: white background, blue border
- Read-only fields: gray background
- Calculated results: highlighted (yellow/green)

---

## Integration with Main Application

### Section Type Registration
Updated `Models/Section.cs`:
```csharp
public enum SectionType
{
    // ... other sections ...
    Ventilation,  // Секция №12
    // ... other sections ...
}
```

---

## Testing Considerations

### Manual Testing Scenarios

#### Scenario 1: No Ventilation
- Set operating hours = 0
- Expected: All energies = 0

#### Scenario 2: No Heat Recovery
- Set both recuperation efficiencies = 0
- Supply temp should equal outdoor temp
- Maximum heating energy

#### Scenario 3: Full Heat Recovery
- Set first stage efficiency = 80%
- Set second stage efficiency = 50%
- Verify supply temperature calculation

#### Scenario 4: Dual Energy Sources
- Enable second energy source
- Set shares: 70% / 30%
- Verify final energy split

#### Scenario 5: Heat Pump (COP > 1)
- Set generation efficiency > 100% (e.g., 300%)
- Verify final energy is less than heating energy

### Edge Cases
- Operating hours = 168 h/week (24/7)
- Zero airflow rate
- Indoor temp = outdoor temp (no heating)
- Very low outdoor temperatures
- Efficiency values at limits (0% and 100%)

---

## Regulatory Formula Reference Summary

| Formula/Item | Description | Implementation Location |
|--------------|-------------|------------------------|
| **(3.27)** | Hᵥₑ = ρ × c × V̇ | `BgVentilationCalculator.CalculateVentilationLossCoefficient()` |
| **(3.28)** | bᵥₑ,ₖ = (θᵢ - θₛᵤₚ) / (θᵢ - θₑ) | `BgVentilationCalculator.CalculateTemperatureControlCoefficient()` |
| **(3.26)** | Qᵥₑ,ₘ = Hᵥₑ × bᵥₑ,ₖ × (θᵢ - θₑ) × tₘ | `BgVentilationCalculator.CalculateMonthlyResults()` |
| **Item 3.5** | Supply air temperature calculation | `BgVentilationCalculator.CalculateSupplyTemperature()` |

---

## Code Comments Convention

All calculation methods include inline comments with formula references:
```csharp
// Наредба RD-02-20-3, Section 12, formula (3.27)
// Наредба RD-02-20-3, Section 12, formula (3.28)
// Наредба RD-02-20-3, Section 12, formula (3.26)
// Наредба RD-02-20-3, Section 12, item 3.5
```

**Format**: Only section number and formula identifier, no legal text in code.

---

## Extensibility

### Future DIN Implementation
The architecture supports adding DIN methodology:
```csharp
public VentilationCalculationResult CalculateDIN(...)
{
    throw new NotImplementedException("DIN методологията за вентилация не е имплементирана.");
}
```

When implementing DIN:
1. Create `DinVentilationCalculator.cs`
2. Implement DIN-specific formulas
3. Update methodology selection in UI
4. Add DIN-specific validation rules

---

## Build Information

**Build Status**: ✅ Success  
**Warnings**: 0  
**Errors**: 0

**Command**:
```powershell
dotnet build EE.Doklad
```

**Output**:
```
Build succeeded in 3.4s
EE.Doklad net8.0-windows succeeded
→ EE.Doklad\bin\Debug\net8.0-windows\EE.Doklad.dll
```

---

## Files Created/Modified

### New Files (7)
1. `Models/VentilationModels.cs` - 107 lines
2. `Models/VentilationSectionData.cs` - 165 lines
3. `Models/VentilationCalculationResult.cs` - 130 lines
4. `Services/BgVentilationCalculator.cs` - 320 lines
5. `ViewModels/VentilationSectionViewModel.cs` - 380 lines
6. `Views/VentilationSectionView.xaml` - 435 lines
7. `Views/VentilationSectionView.xaml.cs` - 15 lines

### Modified Files (1)
1. `Models/Section.cs` - Added `Ventilation` enum value

**Total Lines of Code**: ~1,552 lines

---

## Dependencies

### NuGet Packages (already in project)
- CommunityToolkit.Mvvm (for ObservableObject)
- Windows Presentation Foundation (WPF)

### Internal Dependencies
- `ClimateModels.cs` - Monthly temperature data
- `ObjectDataSectionData.cs` - Heated area, number of people
- `HeatingSectionData.cs` - Indoor reference temperature
- Various converters for XAML data binding

---

## Compliance Checklist

✅ Bulgarian methodology (Наредба RD-02-20-3) fully implemented  
✅ Formula references in code comments (3.26, 3.27, 3.28, item 3.5)  
✅ No internal default tables for ventilation or heat recovery  
✅ All parameters must be input manually  
✅ Single calculation state (no scenarios)  
✅ Monthly calculation with annual aggregation  
✅ Heating energy only (no cooling)  
✅ Dual energy source support with efficiency chain  
✅ Strong typing with nullable enabled  
✅ MVVM architecture pattern  
✅ Separation of concerns (Model/Service/ViewModel/View)  
✅ DIN methodology throws NotImplementedException  
✅ No mixing of DIN logic in BG calculations  
✅ No optimization or "improvement" of regulatory algorithm  

---

## Contact & Maintenance

**Implementation Date**: February 2, 2026  
**Regulatory Version**: Наредба RD-02-20-3  
**Code Language**: C# (.NET 8.0)  
**UI Language**: Bulgarian (Български)

---

## End of Documentation
