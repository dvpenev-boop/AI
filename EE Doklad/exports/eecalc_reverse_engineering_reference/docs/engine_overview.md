# Engine Overview

This package freezes the reverse-engineered EECalc engine reference for clean-room oracle implementation.

The engine is organized into ten reconstruction areas:

| Area | Module | Status |
| --- | --- | --- |
| R1 | MonthlyDays / calendar | Reverse engineered through heating oracle materials. |
| R2 | Heating ventilation / `Hve` / `Qve` | Reverse engineered through heating oracle materials. |
| R3 | Transmission / `Htr` / `Qtr` | Documented in R3 reports. |
| R4 | Heating gains / `Qgn` | Documented in R4 report. |
| R5 | Gamma / Ni / Qnd | Documented in R5 report. |
| R6 | Cooling | Documented in R6 report and cooling oracle report. |
| R7 | Ventilation systems | Documented in R7 reports. |
| R8 | DHW/BGV and solar DHW | Documented in R8 report. |
| R9 | Lighting/devices | Documented in R9 report. |
| R10 | Aggregation, primary energy, CO2, class | Documented in R10 report. |

The master implementation reference is:

```text
analysis/eecalc_master_engine_specification.md
```

Strict parity must preserve confirmed legacy defects and reporting behaviors. Normative corrections belong to future modes only.
