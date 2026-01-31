# Section Toggles & Rendering for Sections 15-20 Implementation

## Overview
Successfully implemented QuestPDF rendering for sections 15-20 with section toggles functionality and automatic filtering of empty sections.

## Changes Made

### 1. Report Model Enhancement (`Report.cs`)
- Added `EnabledSections` property (HashSet<string>) to track which sections should be exported to PDF
- Default behavior: All sections enabled when null/empty
- Added `IsSectionEnabled(SectionType)` helper method for checking if a section is enabled
- This allows future UI implementation for section selection

### 2. Section Registry Infrastructure (`SectionRenderRegistry.cs`)
- Created `SectionDescriptor` class to hold section metadata:
  - Id, Number, Title, Type
  - HasData function (checks if section has content)
  - Render action (PDF rendering logic)
- Created `SectionRenderRegistry` class as a registry for all sections
- This provides extensibility for future section additions

### 3. PDF Generator Service Updates (`PdfGeneratorService.cs`)

#### New Rendering Methods
Implemented dedicated rendering methods for sections 15-20:

**Section 15: Lighting (`GenerateLightingSection`)**
- Renders lighting components table with: Name, Quantity, Power, Hours/Day, Days/Week, Annual Energy
- Shows summary: Total Power, Total Annual Energy, Simultaneous Power

**Section 16/17: Appliances (`GenerateAppliancesSection`)**
- Used for both "Affecting" and "Not Affecting" appliances
- Same table structure as Lighting section
- Shows appliance details and energy consumption

**Section 18: Results (`GenerateResultsSection`)**
- Renders results table with energy consumption by system
- Shows: Energy [kWh/y], Specific [kWh/m²], Primary Energy (non-renewable, renewable, total), CO2 emissions
- Includes summary table with heated area, EP, and totals

**Section 19: Energy Class (`GenerateEnergyClassSection`)**
- Displays building type and energy performance
- Shows calculated energy class (A-G)
- Renders threshold table with color-coded selected class
- Shows class description

**Section 20: Conclusion (`GenerateConclusionSection`)**
- Renders conclusion text (editable by user)
- Uses pre-filled default conclusion text compliant with Bulgarian regulations

#### Section Filtering Logic
Added `ShouldRenderSection(Report, Section)` method that:
- Checks if section is enabled in `Report.EnabledSections`
- Validates section has meaningful data:
  - Lighting: Must have at least one line item
  - Appliances: Must have at least one line item
  - Results: Must have rows
  - Energy Class: Must have building type set
  - Conclusion: Always renders if present
  - Others: Checks for appropriate data presence

#### PDF Generation Flow Updates
- Filters sections before rendering using `ShouldRenderSection`
- Updates page number calculation to skip filtered sections
- Updates TOC generation to exclude filtered sections
- Preserves original section numbering (gaps allowed)

### 4. MainViewModel Updates (`MainViewModel.cs`)
Added initialization for sections 15-17 in `CreateNewReport()`:
- Section 15: Lighting with LightingSectionData
- Section 16: Appliances Affecting with AppliancesSectionData
- Section 17: Appliances Not Affecting with AppliancesSectionData

Sections 18-20 were already initialized.

## Key Features Implemented

### ✅ Section Toggles
- Infrastructure in place via `Report.EnabledSections`
- Default: All sections enabled
- Ready for UI implementation (checkboxes in export dialog)

### ✅ Automatic Empty Section Filtering
- Sections without data are automatically skipped
- No placeholder pages rendered
- Clean PDF output with only meaningful content

### ✅ Original Section Numbering Preserved
- Section titles include original numbers (e.g., "15. Осветление")
- Numbering remains fixed even when sections are skipped
- Example: If sections 11-14 are empty, PDF shows: 10, 15, 16, 17, 18, 19, 20

### ✅ Proper Data Rendering
Each section 15-20 renders with:
- Appropriate table structures
- Summary information
- Formatted numbers (3 decimal places)
- Color-coded headers
- Bold text for totals/important values

## Testing Scenarios

### A) All Sections Enabled with Data
- All sections 15-20 appear in PDF
- Content renders properly with tables and summaries

### B) Section with No Data
- Section automatically skipped
- Does not appear in PDF
- No placeholder page

### C) Section Disabled
- Section skipped (when EnabledSections is implemented in UI)
- Original numbering preserved

### D) Multiple Sections Skipped
- PDF shows only sections with data
- Section numbers maintain gaps (e.g., 6, 15, 19, 20)

## Usage

### Current Behavior (Default)
All sections with data will be exported automatically.

### Future: Enabling/Disabling Sections via UI
```csharp
// Example of how to disable specific sections
report.EnabledSections = new HashSet<string>
{
    "CoverPage",
    "ObjectData",
    "ExternalWalls",
    "Lighting",  // Section 15
    "Results",   // Section 18
    "EnergyClass", // Section 19
    "Conclusion"  // Section 20
    // Sections 16, 17 will be skipped
};

_pdfService.GeneratePdf(report, outputPath);
```

## File Changes Summary
1. `EE.Doklad/Models/Report.cs` - Added EnabledSections
2. `EE.Doklad/Services/SectionRenderRegistry.cs` - New registry infrastructure
3. `EE.Doklad/Services/PdfGeneratorService.cs` - Added rendering + filtering
4. `EE.Doklad/ViewModels/MainViewModel.cs` - Added section initialization

## Acceptance Criteria ✅

- [x] **A)** All sections 15-20 render with data present
- [x] **B)** Sections without data are automatically hidden
- [x] **C)** Section numbering stays fixed (15, 16, 17, 18, 19, 20)
- [x] **D)** No placeholder pages for skipped sections
- [x] Infrastructure for section toggles ready for UI implementation

## Next Steps (Optional Future Enhancements)
1. Add UI dialog for section selection before PDF export
2. Save section selection preferences per report
3. Add section templates/presets
4. Implement more granular data validation

## Notes
- Section titles already contain numbers (defined in `MainViewModel.CreateNewReport`)
- Data models for all sections 15-20 were already implemented
- Implementation follows existing patterns from sections 6-10
- QuestPDF Community license used
