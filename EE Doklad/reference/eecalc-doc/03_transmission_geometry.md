# 03 — Transmission geometry: стени, прозорци, покрив, под, топлинни мостове

## 1. Общ Htr

В `HeatingAndCoolingResultCalc.CalculateParameterHtr(...)`:

```text
Htr = Hd + Hg + Hu
```

където:

```text
Hd = директни външни елементи
Hg = под към земя
Hu = вътрешни/съседни/некондиционирани елементи, коригирани с температурно отношение
```

## 2. Hd — външни елементи

Метод: `CalculateParameterHdCurrent(section)`.

```text
Hd = SumAllDirectionsWallsCurrent(section)
   + SumAllDirectionWindowsCurrent(section)
   + SumNonTrasparentRoof(section.Roof.Current)
   + SumTrasparentRoof(section.Roof.Current)
```

### Външни стени

За всяка ориентация:

```text
Σ(A_outer_i * U_outer_i) + Σ(linear_bridge_i) + Σ(point_bridge_i)
```

В кода:

```text
OuterA_i * OuterU_i
Outer_i.SumL
Outer_i.SumX
```

`TempBridgeCalc.CalculateSums(...)` показва, че линейните топлинни мостове се акумулират така:

```text
TypeSum_i = L_i * Ψ_i
SumL = Σ TypeSum_i
```

`SumX` се подава вече акумулиран от модела.

### Прозорци

```text
H_windows = Σorientation(AccumulateWindowU * AccumulateWindowA)
```

### Покрив, непрозрачни части

```text
H_roof_opaque = Σ(A_i * U_i) + Σ(SumL_i) + Σ(SumX_i)
```

### Покрив, прозрачни части

```text
H_roof_transparent = Σ(A_i * U_i)
```

## 3. Hg — под към земя

Метод: `CalculateParameterHgCurrent(section)`.

```text
Hg = Floor.Current.AccumulateFloorA * Floor.Current.AccumulateFloorU
```

Забележка: детайлното изчисление на U към земя не е в качения код; тук се използва вече изчислена/въведена `AccumulateFloorU`.

## 4. Hu — вътрешни елементи към други зони

Методите:

```text
SumWallDirecrionsHu1(...)
CalcCeilingsParameterHu2(...)
CalcFloorsParameterHu3(...)
```

Общ модел:

```text
Hu_element = A * U * (θavg_inner - θadjacent) / (θavg_inner - Te)
```

Това е температурен редукционен фактор:

```text
b = (θavg_inner - θadjacent) / (θavg_inner - Te)
Hu = A * U * b
```

За стени се използват `InnerA_i`, `InnerU_i`, `InnerW_i`.
За тавани — `CeilingA_i`, `CeilingU_i`, `CeilingW_i`.
За подове към друга зона — `OtherFloorA_i`, `OtherFloorU_i`, `OtherFloorS_i`.

## 5. Агрегиране в helper класовете

### WallsTableCalc

```text
AccumulateOuterA = Σ OuterA_i
AccumulateOuterU = weighted_average(OuterU_i, OuterA_i)
AccumulateOuterE = weighted_average(OuterE_i, OuterA_i)
AccumulateOuterAlfa = weighted_average(OuterAlfa_i, OuterA_i)
AccumulateWindowA = Σ WindowA_i
AccumulateWindowU = weighted_average(WindowU_i, WindowA_i)
AccumulateWindowG = weighted_average(WindowG_i, WindowA_i)
AccumulateWindowE = weighted_average(WindowE_i, WindowA_i)
```

### RoofTableCalc

```text
AccumulateNonTransparentA = Σ NonTransparentA_i
AccumulateNonTransparentU = weighted_average(NonTransparentU_i, A_i)
AccumulateNonTransparentE = weighted_average(NonTransparentE_i, A_i)
AccumulateNonTransparentAlfa = weighted_average(NonTransparentAlfa_i, A_i)
AccumulateTransparentA = Σ TransparentA_i
AccumulateTransparentU = weighted_average(TransparentU_i, TransparentA_i)
AccumulateCeilingA = Σ CeilingA_i
AccumulateCeilingU = weighted_average(CeilingU_i, CeilingA_i)
```

### FloorTableCalc

```text
AccumulateFloorA = Σ FloorA_i
AccumulateFloorU = weighted_average(FloorU_i, FloorA_i)
AccumulateOtherFloorA = Σ OtherFloorA_i
AccumulateOtherFloorU = weighted_average(OtherFloorU_i, OtherFloorA_i)
```

## 6. Рискови места

1. В декомпилирания код има подозрителни редове, например `InnerU = wall.IneerA5` и `CeilingU = roof.CeilingA5`. Това може да е decompiler artifact или реална грешка/obfuscation остатък. Трябва да се валидира чрез контролен проект.
2. `SumWallDirecrionsHu1(...)` в показания декомпилиран код повтаря `section.NorthWalls.Current` за всички посоки. Това също изглежда като decompilation/obfuscation артефакт и трябва да се провери с IL или с runtime резултати.
3. За 1:1 съвпадение с EECalc може да се наложи да се повтори точно дори такава грешна логика, ако тя реално е изпълнявана.
