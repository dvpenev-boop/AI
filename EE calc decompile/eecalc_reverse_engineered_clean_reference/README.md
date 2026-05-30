# EECalc Reverse-Engineered Clean Reference Package

This is a clean reverse-engineering reference package for starting a new project from zero.

It is not a ready production engine. It intentionally contains no parity/test infrastructure, no CSV pipeline, no test fixtures, and no temporary runners.

Included material:

- Decompiled EECalc reference code under `reference/eecalc-decompiled/`.
- Original EECalc XML reference configuration under `reference/eecalc-config/`.
- Reverse-engineering analysis documents under `analysis/`.
- Clean extracted formula, climate-provider, model, and legacy-mapping source under `clean_code/`.

`LegacyEECalcStrict` means XML as-is behavior with preserved known KD differences. It is intended for strict legacy reference compatibility, not corrected ordinance behavior.

Known strict-mode differences preserved by this reference package:

- KD-A001: Fuel1 duplicate total.
- KD-A009: Fuel1/Fuel8 reporting bucket inversion.
- KD-DATA-001: XML January sign error preserved in strict mode.
