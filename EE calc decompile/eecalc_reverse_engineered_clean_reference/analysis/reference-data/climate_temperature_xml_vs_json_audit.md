# Climate Temperature XML vs JSON Audit

Scope: climate zones only; monthly average temperatures only. No formulas, code, XML, or JSON were modified.

Sources:
- XML: `reference/eecalc-config/DefaultParams.xml`
- JSON requested: `Climate data/climate_zones.json`
- JSON used: `EE.Doklad/Data/climate_zones.json`
- Note: the requested JSON path was not present; the workspace copy at `EE.Doklad/Data/climate_zones.json` was used.

Comparison rule: XML climate zone `Number` is zero-based, so `ZoneId = Number + 1` was compared with JSON `Zones[].Id`.
Delta is `JsonAvgTemp - XmlAvgTemp`. Only non-zero deltas are included.

| ZoneId | ZoneName | Month | XmlAvgTemp | JsonAvgTemp | Delta |
| --- | --- | --- | ---: | ---: | ---: |
| 1 | 1 - Северно Черноморие | Jan | -1.9 | 1.9 | 3.8 |
| 2 | 2 - Добруджа | Jan | -0.5 | 0.5 | 1 |
| 3 | 3 - Северна България – поречието на р. Дунав | Jan | -0.1 | 0.1 | 0.2 |

## Summary

- Differing rows: 3.
- Zones with differences: 3.

## KD-DATA-001 Decision

Classification:

Confirmed legacy XML data error, not a calculation/formula error.

The correct ordinance values are the positive January temperatures in `EE.Doklad/Data/climate_zones.json`.
The negative January values in `reference/eecalc-config/DefaultParams.xml` are a technical data-entry/sign error in the legacy EECalc XML.

| ZoneId | Month | Wrong legacy XML AvgTemp | Correct ordinance AvgTemp |
| --- | --- | ---: | ---: |
| 1 | January | -1.9 | 1.9 |
| 2 | January | -0.5 | 0.5 |
| 3 | January | -0.1 | 0.1 |

Mode decision:

- `LegacyEECalcStrict` preserves `DefaultParams.xml` exactly, including the sign error.
- `LegacyEECalcCorrectedData` uses corrected January values from `EE.Doklad/Data/climate_zones.json`.
- `CurrentOrdinance` uses `EE.Doklad/Data/climate_zones.json`.

No XML or JSON values were modified by this audit.
- Zone 1 (1 - Северно Черноморие): Jan.
- Zone 2 (2 - Добруджа): Jan.
- Zone 3 (3 - Северна България – поречието на р. Дунав): Jan.
