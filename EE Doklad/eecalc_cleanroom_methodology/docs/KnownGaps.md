# Known Gaps and Caveats

## Cooling With Untreated Outdoor Air / Night Ventilation

Status: implemented but not considered 100% finalized.

The clean implementation follows the decompiled EECalc methodology:

```text
Qfree = Debit * 0.34 * (ProjectTemperature - OutdoorHourlyTemperature) / 1000
```

By schedule category:

- Workdays use workday night-ventilation schedule.
- Saturdays use Saturday night-ventilation schedule.
- Sundays use Sunday night-ventilation schedule.
- Holidays use Sunday night-ventilation schedule with non-project temperature.

The decompiled helper for a night schedule behaves as:

```text
start == end -> no hours
start < end  -> start, ..., end - 1
start > end  -> 0, ..., end - 1 and start, ..., 23
```

Example:

```text
23 -> 6 means hours 0, 1, 2, 3, 4, 5, 23
0 -> 0 means no hours
```

Observed issue:

- In some validation scenarios, all major rows match but cooling differs by a small amount.
- The mismatch appears only when the no-treatment/free-cooling contribution is active.
- It is not explained by EI1 conversion, R7 ventilation cooling transfer, climate XML provider, or the main cooling net-energy formula.

Keep this marked as an open validation item before using this part as a final production rule.

## Decompiled Defect Preserved in Diagnostics

The decompiled ventilation latent heat code contains an apparent Saturday after-hours defect where debit is multiplied by itself in one branch. For debit `1.0` this is numerically invisible; for other debit values it can matter.

The verification harness includes decompiled-loop diagnostics where relevant, but production use should decide whether to preserve exact EECalc behavior or correct the defect.

## Combined ECM Allocator

No general combined ECM measure allocator is implemented.

This is intentional. Validated scope is per-component/per-aggregator:

- Envelope.
- Cooling.
- Ventilation.
- Lighting/devices.
- Fans and pumps.
- DHW/BGV.

## UI Rounding

EECalc UI tables show rounded values. Internal `DataRow.Value` can hold more precision than the visible cell. When validating, compare against kWh/year where available and inspect raw diagnostic rows.

