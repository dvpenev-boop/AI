# R7 Ventilation Review Addendum

## Summary

`KD-V007` has been reclassified. Ref1/Ref2 ventilation calculations reusing baseline schedules is expected reference-building behavior, not a bug and not an EECalc quirk.

Reference buildings preserve the baseline operating pattern and replace selected physical parameters. In this phase those parameters include reference temperatures, infiltration-related values, ventilation scalar inputs, and efficiency assumptions. Therefore schedule reuse should be modeled as design behavior in the future ventilation oracle.

## Confirmed KD items

- KD-V001
- KD-V002
- KD-V003
- KD-V004
- KD-V005
- KD-V006
- KD-V008
- KD-V009
- KD-V010
- KD-V011
- KD-V012
- KD-V013
- KD-V014
- KD-V015

## Expected design behaviors

- Ref1/Ref2 reuse baseline schedules.

## Rationale

The original R7 report treated Ref1/Ref2 schedule reuse as a KD candidate because the schedule path differs from the physical parameter path. After review, that difference is intentional: reference-building calculations are not full independent building states. They keep baseline schedules so reference comparisons isolate the effect of selected physical parameters.

Classifying this as expected behavior avoids two mistakes in the future oracle:

- It prevents adding a correction mode for behavior that should remain stable in EECalc-compatible and reference-building calculations.
- It keeps actual KD tracking focused on behaviors that are surprising, lossy, asymmetric, or likely to affect parity unexpectedly.

Documentation updates made:

- Removed `KD-V007` from the R7 KD list.
- Added `Expected Design Behaviors` to the R7 report.
- Added the confirmed R7 KD set and the Ref1/Ref2 schedule behavior note to the known-differences tracker.
