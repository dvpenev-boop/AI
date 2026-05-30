# Climate Sources

Strict parity mode uses the legacy XML files exactly as-is:

```text
reference/eecalc-config/DefaultParams.xml
reference/eecalc-config/DefaultSunParams.xml
```

`DefaultParams.xml` supplies:

- monthly average temperatures
- orientation solar radiation `N/E/S/W/H`
- hourly temperature/humidity
- `Pb`

`DefaultSunParams.xml` supplies solar DHW parameters:

- monthly radiation
- monthly average temperature
- cloudiness

## KD-DATA-001

`DefaultParams.xml` contains January sign errors for climate zones 1-3:

| Zone | Legacy XML January | Correct ordinance value |
| --- | ---: | ---: |
| 1 | -1.9 | 1.9 |
| 2 | -0.5 | 0.5 |
| 3 | -0.1 | 0.1 |

For parity validation:

```text
DefaultParams.xml is authoritative.
```

No correction is applied in `LegacyEECalcStrict`.

Correction modes are reserved for future work.
