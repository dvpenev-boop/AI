# Quick Testing Guide - Sections 15-20 PDF Export

## How to Test

### 1. Start the Application
```powershell
cd "e:\AI\EE Doklad"
dotnet run --project EE.Doklad
```

### 2. Create or Open a Report
- Click "Нов доклад" or open existing report
- Navigate through sections using the left panel

### 3. Add Data to Sections 15-20

#### Section 15: Осветление (Lighting)
1. Navigate to "15. Осветление"
2. Click "+" to add lighting items
3. Select lighting component from dropdown
4. Set quantity, hours/day, days/week
5. Observe automatic calculations

#### Section 16: Други разходи влияещи (Appliances Affecting)
1. Navigate to "16. Други разходи влияещи"
2. Click "+" to add appliances
3. Select appliance from dropdown
4. Fill in usage parameters

#### Section 17: Други разходи не влияещи (Appliances Not Affecting)
1. Navigate to "17. Други разходи не влияещи"
2. Same process as section 16

#### Section 18: Резултати сграда (Results)
1. Navigate to "18. Резултати сграда"
2. Fill in energy consumption data
3. Select energy carriers
4. System calculates totals automatically

#### Section 19: Клас на енергопотребление (Energy Class)
1. Navigate to "19. Клас на енергопотребление"
2. Select building type from section 5 (ObjectData)
3. Energy performance (EP) comes from section 18
4. Class is calculated automatically

#### Section 20: Заключение (Conclusion)
1. Navigate to "20. Заключение"
2. Edit the conclusion text as needed
3. Default text is pre-filled

### 4. Export to PDF
1. Click "Експорт в PDF" button (top toolbar)
2. Choose save location
3. PDF will be generated and automatically opened

### 5. Verify PDF Content

#### Check A: All Sections with Data Render
- Open PDF
- Verify sections 15-20 appear
- Check tables are properly formatted
- Verify calculations are correct

#### Check B: Empty Sections Are Skipped
1. Create new report
2. Don't add data to sections 15-17
3. Export PDF
4. Verify: Only sections with data appear (likely 18, 19, 20)
5. Verify: No placeholder pages or "Няма данни" messages

#### Check C: Section Numbering Preserved
- Observe section headers in PDF
- Should show: "15. Осветление", "16. Други разходи влияещи", etc.
- Even if sections 11-14 are empty, numbering should show 10, 15, 16...

#### Check D: Table of Contents (TOC)
- Check TOC page
- Only sections that rendered should appear in TOC
- Page numbers should be correct

## Expected Results

### Section 15: Lighting
**Table columns:**
- № | Наименование | Брой | P₁ [W] | P [kW] | h/day | days/week | E [kWh/y]

**Summary:**
- Обща мощност [kW]
- Обща годишна енергия [kWh/y]
- Едновременна мощност [W/m²]
- Едновременна мощност [W]

### Section 16/17: Appliances
Same structure as Section 15

### Section 18: Results
**Table columns:**
- Система | E [kWh/y] | EP [kWh/m²] | PEnr [kWh] | PEr [kWh] | PEtot [kWh] | CO2e [tCO2]

**Summary:**
- Отопляема площ [m²]
- Годишна специфична енергия (EP) [kWh/m²]
- Обща първична енергия [kWh]
- Обща първична невъзобновяема [kWh]
- Обща първична възобновяема [kWh]
- Общи емисии CO2 [tCO2]

### Section 19: Energy Class
**Parameters:**
- Тип сграда
- Годишна специфична енергия (EP) [kWh/m²]
- Изчислен клас (A-G in bold)

**Threshold Table:**
- Class | Min | Max | Rule | Marker (← for selected class)
- Selected class row highlighted in yellow

### Section 20: Conclusion
Pre-filled text with standard conclusion statements (editable).

## Troubleshooting

### Section Not Appearing in PDF
1. Check if section has data:
   - Lighting/Appliances: At least one line item
   - Results: At least one row filled
   - Energy Class: Building type must be selected
2. Check console for errors

### Numbers Not Formatting
- All numbers should show 3 decimal places
- If showing more/less, check `GenericNumberFormat` constant

### TOC Page Numbers Wrong
- Close and reopen PDF
- Regenerate PDF
- Check console for page count calculation errors

### Section Order Wrong
- Sections follow `Order` property in Report model
- Check that Order values are sequential

## Testing Checklist

- [ ] Section 15 renders with lighting data
- [ ] Section 16 renders with appliances (affecting)
- [ ] Section 17 renders with appliances (not affecting)
- [ ] Section 18 renders with results data
- [ ] Section 19 renders with energy class
- [ ] Section 20 renders with conclusion text
- [ ] Empty sections are automatically skipped
- [ ] Section numbers preserved (15, 16, 17, 18, 19, 20)
- [ ] TOC includes only rendered sections
- [ ] Page numbers in TOC are correct
- [ ] No "Попълнете данните..." placeholders
- [ ] Tables are properly formatted
- [ ] Summary calculations are correct
- [ ] PDF opens automatically after generation

## Known Limitations

1. **Section Selection UI**: Not yet implemented
   - Currently all sections with data are rendered
   - Future: Add dialog to select sections before export

2. **Section Templates**: Not available
   - Could add preset configurations
   - Future enhancement

3. **Validation**: Basic data presence checks
   - More granular validation could be added
   - Future enhancement

## Sample Test Scenario

1. Create new report "Test Report 2026"
2. Fill section 5 (ObjectData) with building info
3. Add 2-3 lighting items to section 15
4. Add 1-2 appliances to section 16
5. Leave section 17 empty
6. Fill section 18 with sample energy data
7. Section 19 will auto-calculate based on 5 and 18
8. Review/edit section 20 conclusion
9. Export PDF
10. Expected: Sections 15, 16, 18, 19, 20 appear; section 17 skipped
11. Numbering: "15. Осветление", "16. Други разходи влияещи", "18. Резултати сграда", etc.
